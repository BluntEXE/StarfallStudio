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

    // Scan for new actors for N frames after GPose entry (FFXIV populates GPose
    // slots across multiple frames without calling Character::Initialize).
    private int _gPoseScanFramesRemaining = 0;
    private const int GPoseScanWindowFrames = 120; // ~2s at 60fps

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
        foreach(var go in _objects)
        {
            AttachActor(go, _actorContainerEntity);
        }
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

        // Open a scan + enforce window on GPose entry.
        // EnforceAmbientHide also runs continuously while IsGPosing (see below).
        StarfallStudio.Log.Information("[EntityActorManager] GPose entered — ambient hide active");
        _gPoseScanFramesRemaining = GPoseScanWindowFrames;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        // Always enforce while in GPose (not just the scan window) — FFXIV can
        // reset Character.Alpha at any point during its initialization sequence.
        if(_gPoseService.IsGPosing)
            EnforceAmbientHide();

        // Scan for new actors during the window after GPose entry
        if(_gPoseScanFramesRemaining > 0)
        {
            _gPoseScanFramesRemaining--;
            PopulateExistingActors();
        }
    }

    private void EnforceAmbientHide()
    {
        if(!_actorContainerEntity.TryGetCapability<ActorContainerCapability>(out var cap))
            return;

        foreach(var go in _objects)
        {
            if(!go.IsGPose()) continue;
            if(go.ObjectKind == ObjectKind.Ornament) continue;

            // ICharacter.Native() returns StructsCharacter* which has Alpha.
            // IGameObject.Native() returns StructsObject* (base) which does not.
            if(go is not ICharacter chara) continue;

            // Skip managed actors (local player copy + user-pinned actors)
            if(cap.IsIndexManaged((ushort)go.ObjectIndex)) continue;

            // Direct native write every frame — beats any FFXIV alpha reset
            if(chara.Native()->Alpha != 0f)
            {
                StarfallStudio.Log.Information($"[EntityActorManager] Hiding ambient actor: {go.Name} idx={go.ObjectIndex} (alpha was {chara.Native()->Alpha})");
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
