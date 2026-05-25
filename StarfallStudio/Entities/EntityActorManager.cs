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
using System.Collections.Generic;
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

    // Scan for new actors for N frames after GPose entry.
    private int _gPoseScanFramesRemaining = 0;
    private const int GPoseScanWindowFrames = 120;

    // In open world GPose, ambient actors stay at their overworld indices (0-199).
    // Track which ones we hid so we can restore alpha on GPose exit.
    private readonly HashSet<ushort> _hiddenOverworldIndices = [];

    // Whether we're in open-world GPose (no 201+ actor copies exist).
    private bool _isOpenWorldGPose = false;

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
            AttachActor(go, _actorContainerEntity);
    }

    private void AttachActor(IGameObject go, Entity parent)
    {
        if(_entityManager.TryGetEntity(new EntityId(go), out var entity))
        {
            if(parent.Equals(entity.Parent))
                return;
        }
        else
        {
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
        HandleCompanions(entity, true);
    }

    private void DetachActor(IGameObject actor)
    {
        if(_entityManager.TryGetEntity(new EntityId(actor), out var entity))
            _entityManager.DetachEntity(entity, true);
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
                            AttachActor(companionObject, entity);
                    }
                    return;
                }

                if(checkParent)
                {
                    var maybeParentId = currentActor.ObjectIndex - 1;
                    if(maybeParentId < 0) return;

                    var maybeParent = _objects[maybeParentId];
                    if(maybeParent == null) return;

                    _entityManager.TryGetEntity(new EntityId(maybeParent), out var maybeParentEntity);
                    if(maybeParentEntity == null) return;

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
            RestoreOverworldActors();
            _gPoseScanFramesRemaining = 0;
            _isOpenWorldGPose = false;
            return;
        }

        // Determine if we're in open-world GPose by checking whether any 201+ actor slots
        // are populated. In instanced content, FFXIV copies actors to 201+.
        // In open world, ambient actors stay at their original 0-199 indices.
        _isOpenWorldGPose = !HasGPoseSlotActors();

        StarfallStudio.Log.Information(
            $"[EntityActorManager] GPose entered — {(_isOpenWorldGPose ? "open world" : "instanced")} mode");

        _gPoseScanFramesRemaining = GPoseScanWindowFrames;
    }

    private bool HasGPoseSlotActors()
    {
        foreach(var go in _objects)
        {
            if(go.IsGPose() && go is ICharacter)
                return true;
        }
        return false;
    }

    private void OnFrameworkUpdate(IFramework framework)
    {
        if(_gPoseService.IsGPosing)
            EnforceAmbientHide();

        if(_gPoseScanFramesRemaining > 0)
        {
            _gPoseScanFramesRemaining--;
            PopulateExistingActors();
        }
    }

    private void EnforceAmbientHide()
    {
        if(_isOpenWorldGPose)
            EnforceOverworldHide();
        else
            EnforceGPoseSlotHide();
    }

    // Open world: ambient actors remain at indices 0-199. Hide all of them except the
    // local player (identified by name match with IClientState.LocalPlayer).
    private void EnforceOverworldHide()
    {
        var localPlayer = _objects.LocalPlayer;
        var localName = localPlayer?.Name.TextValue;

        foreach(var go in _objects)
        {
            if(!go.IsOverworld()) continue;
            if(go.ObjectKind == ObjectKind.Ornament) continue;
            if(go is not ICharacter chara) continue;

            // Keep local player visible
            if(localName != null && go.Name.TextValue == localName) continue;

            if(chara.Native()->Alpha != 0f)
            {
                _hiddenOverworldIndices.Add((ushort)go.ObjectIndex);
                StarfallStudio.Log.Information(
                    $"[EntityActorManager] Open-world hide: {go.Name} idx={go.ObjectIndex} alpha={chara.Native()->Alpha}");
                chara.Native()->Alpha = 0f;
            }
        }
    }

    // Instanced content: FFXIV copies actors to slots 201+. Hide unmanaged ones.
    private void EnforceGPoseSlotHide()
    {
        if(!_actorContainerEntity.TryGetCapability<ActorContainerCapability>(out var cap))
            return;

        foreach(var go in _objects)
        {
            if(!go.IsGPose()) continue;
            if(go.ObjectKind == ObjectKind.Ornament) continue;
            if(go is not ICharacter chara) continue;
            if(cap.IsIndexManaged((ushort)go.ObjectIndex)) continue;

            if(chara.Native()->Alpha != 0f)
            {
                StarfallStudio.Log.Information(
                    $"[EntityActorManager] GPose-slot hide: {go.Name} idx={go.ObjectIndex} alpha={chara.Native()->Alpha}");
                chara.Native()->Alpha = 0f;
            }
        }
    }

    // Restore alpha on GPose exit for any overworld actors we hid.
    private void RestoreOverworldActors()
    {
        foreach(var idx in _hiddenOverworldIndices)
        {
            var go = _objects[idx];
            if(go is ICharacter chara)
                chara.Native()->Alpha = 1f;
        }

        if(_hiddenOverworldIndices.Count > 0)
            StarfallStudio.Log.Information($"[EntityActorManager] Restored {_hiddenOverworldIndices.Count} overworld actors on GPose exit");

        _hiddenOverworldIndices.Clear();
    }

    public void Dispose()
    {
        RestoreOverworldActors();
        _monitorService.CharacterInitialized -= OnCharacterInitialized;
        _monitorService.CharacterDestroyed -= OnCharacterDestroyed;
        _gPoseService.OnGPoseStateChange -= OnGPoseStateChanged;
        _framework.Update -= OnFrameworkUpdate;
    }
}
