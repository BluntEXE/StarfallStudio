using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Actor.Extensions;
using StarfallStudio.Game.Core;
using StarfallStudio.Game.GPose;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using NativeCharacter = FFXIVClientStructs.FFXIV.Client.Game.Character.Character;

namespace StarfallStudio.Entities;

public unsafe class EntityActorManager : IDisposable
{
    private readonly EntityManager _entityManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ObjectMonitorService _monitorService;
    private readonly IObjectTable _objects;
    private readonly IFramework _framework;

    private readonly ActorContainerEntity _actorContainerEntity;
    private readonly ActorSpawnService _actorSpawnService;
    private readonly GPoseService _gPoseService;

    // After GPose entry, scan every frame for N frames to catch actors that
    // FFXIV populates across multiple frames (open-world GPose copies structs
    // without calling Character::Initialize, so our CharacterInitialized hook
    // never fires for them).
    private int _gPoseScanFramesRemaining = 0;
    private const int GPoseScanWindowFrames = 90; // ~1.5 s at 60 fps

    public EntityActorManager(EntityManager entityManager, ActorSpawnService actorSpawnService, IServiceProvider serviceProvider, ObjectMonitorService monitorService, IObjectTable objects, IFramework framework, GPoseService gPoseService)
    {
        _entityManager = entityManager;
        _serviceProvider = serviceProvider;
        _monitorService = monitorService;
        _objects = objects;
        _framework = framework;
        _actorSpawnService = actorSpawnService;
        _gPoseService = gPoseService;

        _monitorService.CharacterInitialized += OnCharacterInitialized;
        _monitorService.CharacterDestroyed += OnCharacterDestroyed;
        _gPoseService.OnGPoseStateChange += OnGPoseStateChanged;
        _framework.Update += OnFrameworkUpdate;

        _actorContainerEntity = ActivatorUtilities.CreateInstance<ActorContainerEntity>(_serviceProvider);
    }

    public void AttachContainer()
    {
        _entityManager.AttachEntity(_actorContainerEntity, null);

        PopulateExistingActors();
    }

    private void PopulateExistingActors()
    {
        int newActors = 0;
        foreach(var go in _objects)
        {
            // Only count actors that aren't already registered — AttachActor
            // returns early for already-tracked objects, so the inner loop is cheap.
            bool wasKnown = _entityManager.TryGetEntity(new EntityId(go), out _);
            AttachActor(go, _actorContainerEntity);
            if(!wasKnown && _entityManager.TryGetEntity(new EntityId(go), out _))
                newActors++;
        }

        if(newActors > 0)
            StarfallStudio.Log.Debug($"[EntityActorManager] PopulateExistingActors: attached {newActors} new actor(s)");
    }

    private void AttachActor(IGameObject go, Entity parent)
    {
        if(_entityManager.TryGetEntity(new EntityId(go), out var entity))
        {
            // Already attached to the correct parent
            if(parent.Equals(entity.Parent))
                return;
        }
        else
        {
            // Only characters
            if(!go.Native()->IsCharacter())
                return;

            if(go.ObjectKind == ObjectKind.Ornament) return;

            // TODO: We should allow manipulation of overworld actors too
            if(!go.IsGPose())
                return;

            StarfallStudio.Log.Debug($"[EntityActorManager] Attaching new GPose actor: {go.Name} idx={go.ObjectIndex}");
            entity = ActivatorUtilities.CreateInstance<ActorEntity>(_serviceProvider, go);
        }
        entity.SetSpawnFlags(_actorSpawnService.GetSpawnFlagsByIndex((ushort)(go.ObjectIndex - 200)));

        _entityManager.AttachEntity(entity, parent, true);


        // This is ew, but we need to handle companions here for now.
        // This would be a stack overflow but the parenting check above prevents it.
        HandleCompanions(entity, true);
    }

    private void DetachActor(IGameObject actor)
    {
        if(_entityManager.TryGetEntity(new EntityId(actor), out var entity))
        {
            _entityManager.DetachEntity(entity, true);
        }
    }

    private void HandleCompanions(Entity entity, bool checkParent)
    {
        if(entity is ActorEntity actorEntity)
        {
            var currentActor = actorEntity.GameObject;

            if(currentActor is ICharacter character)
            {
                if(character.HasSpawnedCompanion())
                {
                    var companion = character.Native()->CompanionObject;
                    if(companion != null)
                    {
                        var companionObject = _objects.CreateObjectReference((nint)companion);
                        if(companionObject != null)
                        {
                            AttachActor(companionObject, entity);
                        }
                    }
                    return;
                }

                if(checkParent)
                {
                    var maybeParentId = currentActor.ObjectIndex - 1;
                    if(maybeParentId < 0)
                        return;

                    var maybeParent = _objects[maybeParentId];
                    if(maybeParent == null)
                        return;

                    _entityManager.TryGetEntity(new EntityId(maybeParent), out var maybeParentEntity);

                    if(maybeParentEntity == null)
                        return;

                    HandleCompanions(maybeParentEntity, false);
                }
            }
        }
    }

    private void OnCharacterDestroyed(NativeCharacter* chara)
    {
        var go = _objects.CreateObjectReference((nint)chara);
        if(go != null)
            DetachActor(go);
    }

    private void OnCharacterInitialized(NativeCharacter* chara)
    {
        // We wait for one frame on create to ensure that the actor is fully initialized
        _framework.RunOnTick(() =>
        {
            var go = _objects.CreateObjectReference((nint)chara);
            if(go != null)
                AttachActor(go, _actorContainerEntity);
        });
    }

    private void OnGPoseStateChanged(bool isGPosing)
    {
        if(!isGPosing)
        {
            _gPoseScanFramesRemaining = 0;
            return;
        }

        // In open world (e.g. Limsa), FFXIV populates GPose slots by copying
        // pre-allocated Character structs without calling Initialize.
        // CharacterInitialized never fires for those actors. We open a scan window
        // and pick them up on the IFramework.Update loop until they appear.
        StarfallStudio.Log.Debug($"[EntityActorManager] GPose entered — opening {GPoseScanWindowFrames}-frame actor scan window");
        _gPoseScanFramesRemaining = GPoseScanWindowFrames;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        if(_gPoseScanFramesRemaining <= 0)
            return;

        _gPoseScanFramesRemaining--;
        PopulateExistingActors();

        // FFXIV's GPose initialization resets actor alpha after we set it.
        // Re-enforce alpha=0 every frame for the duration of the scan window.
        EnforceAmbientHide();
    }

    private void EnforceAmbientHide()
    {
        if(!_actorContainerEntity.TryGetCapability<ActorContainerCapability>(out var cap))
            return;

        foreach(var go in _objects)
        {
            if(!go.IsGPose()) continue;
            if(go.ObjectKind == ObjectKind.Ornament) continue;

            // Native() on ICharacter returns StructsCharacter* which has Alpha.
            // Native() on IGameObject returns StructsObject* (base) which does not.
            if(go is not ICharacter chara) continue;

            // Leave pinned/managed actors alone (player and any user-pinned actors)
            if(cap.IsIndexManaged((ushort)go.ObjectIndex)) continue;

            // Direct native write — no async, no diff-check, beats any FFXIV reset
            if(chara.Native()->Alpha != 0f)
            {
                StarfallStudio.Log.Debug($"[EntityActorManager] EnforceAmbientHide: hiding {go.Name} idx={go.ObjectIndex} alpha was {chara.Native()->Alpha}");
                chara.Native()->Alpha = 0f;
            }
        }
    }

    public void Dispose()
    {
        _monitorService.CharacterInitialized -= OnCharacterInitialized;
        _monitorService.CharacterDestroyed -= OnCharacterDestroyed;
        _gPoseService.OnGPoseStateChange -= OnGPoseStateChanged;
        _framework.Update -= OnFrameworkUpdate;
    }
}
