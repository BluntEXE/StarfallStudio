using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Core;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Widgets.Actor;
using StarfallStudio.Game.Actor.Extensions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarfallStudio.Capabilities.Actor;

public class ActorContainerCapability : Capability
{
    private readonly EntityManager _entityManager;
    private readonly ActorSpawnService _actorSpawnService;
    private readonly TargetService _targetService;
    private readonly GPoseService _gPoseService;
    private readonly IObjectTable _objectTable;

    public bool CanControlCharacters => _gPoseService.IsGPosing;

    // Tracks actors the user explicitly spawned or pinned - shown in the default (managed-only) view.
    private readonly HashSet<ushort> _managedActorIndices = [];

    public ActorContainerCapability(ActorContainerEntity parent, EntityManager entityManager, ActorSpawnService actorSpawnService, TargetService targetService, GPoseService gPoseService, IObjectTable objectTable) : base(parent)
    {
        _entityManager = entityManager;
        _actorSpawnService = actorSpawnService;
        _targetService = targetService;
        _gPoseService = gPoseService;
        _objectTable = objectTable;
        Widget = new ActorContainerWidget(this);
    }

    public bool IsManaged(ActorEntity actor) =>
        _managedActorIndices.Contains((ushort)actor.GameObject.ObjectIndex);

    public void PinActor(ActorEntity actor) =>
        _managedActorIndices.Add((ushort)actor.GameObject.ObjectIndex);

    public void UnpinActor(ActorEntity actor) =>
        _managedActorIndices.Remove((ushort)actor.GameObject.ObjectIndex);

    public void SelectActorInHierarchy(ActorEntity entity)
    {
        _entityManager.SetSelectedEntity(entity);
    }

    public (EntityId, ICharacter) CreateCharacter(bool enableAttachments, bool targetNewInHierarchy, bool forceSpawnActorWithoutCompanion = false)
    {
        SpawnFlags flags = SpawnFlags.Default;
        if(enableAttachments)
            flags |= SpawnFlags.ReserveCompanionSlot;

        if(_actorSpawnService.CreateCharacter(out var chara, flags, disableSpawnCompanion: forceSpawnActorWithoutCompanion))
        {
            _managedActorIndices.Add((ushort)chara.ObjectIndex);
            EntityId characterId = new EntityId(chara);
            if(targetNewInHierarchy)
            {
                _entityManager.SetSelectedEntity(characterId);
            }
            return (characterId, chara);
        }

        throw new Exception("Failed to create character");
    }

    public (EntityId, ICharacter) CreateProp(bool selectInHierarchy)
    {
        if(_actorSpawnService.SpawnNewProp(out ICharacter? character))
        {
            _managedActorIndices.Add((ushort)character!.ObjectIndex);
            EntityId characterId = new EntityId(character!);
            if(selectInHierarchy)
            {
                _entityManager.SetSelectedEntity(character!);
            }
            return (characterId, character!);
        }

        throw new Exception("Failed to create prop");
    }

    public void SpawnNewProp(bool selectInHierarchy)
    {
        if(_actorSpawnService.SpawnNewProp(out ICharacter? character))
        {
            _managedActorIndices.Add((ushort)character!.ObjectIndex);
            if(selectInHierarchy)
            {
                _entityManager.SetSelectedEntity(character!);
            }
        }
    }

    public void DestroyCharacter(ActorEntity entity)
    {
        _managedActorIndices.Remove((ushort)entity.GameObject.ObjectIndex);
        _actorSpawnService.DestroyObject(entity.GameObject);
    }

    public void CloneActor(ActorEntity entity, bool targetNewInHierarchy)
    {
        if(entity.GameObject is ICharacter character)
        {
            if(_actorSpawnService.CloneCharacter(character, out var chara))
            {
                _managedActorIndices.Add((ushort)chara.ObjectIndex);
                if(targetNewInHierarchy)
                {
                    _entityManager.SetSelectedEntity(chara);
                }
            }
        }
    }

    public void DestroyAll()
    {
        _actorSpawnService.ClearAll();
    }

    public void Target(ActorEntity entity)
    {
        _targetService.GPoseTarget = entity.GameObject;
    }

    public void SelectInHierarchy(ActorEntity entity)
    {
        _entityManager.SetSelectedEntity(entity);
    }

    private static readonly HashSet<ObjectKind> _validOverworldKinds = [
        ObjectKind.Pc,
        ObjectKind.BattleNpc,
        ObjectKind.EventNpc,
        ObjectKind.Mount,
        ObjectKind.Companion,
    ];

    public unsafe List<ICharacter> GetOverworldActors()
    {
        return _objectTable
            .Where(o =>
                o.IsValid()
                && o is ICharacter
                && o.ObjectIndex != 200                        // skip GPose special slot
                && _validOverworldKinds.Contains(o.ObjectKind)
                && o.Native()->DrawObject != null)             // must have a loaded model
            // for players: prefer GPose copy (index >= 200) over the overworld duplicate
            .GroupBy(o => o.ObjectKind == ObjectKind.Pc ? o.Name.TextValue : o.Name.TextValue + "_" + o.ObjectIndex)
            .Select(g => g.OrderByDescending(o => o.ObjectIndex >= ActorTableHelpers.GPoseStart).First())
            .OfType<ICharacter>()
            .OrderBy(o => o.YalmDistanceX * o.YalmDistanceX + o.YalmDistanceZ * o.YalmDistanceZ)
            .ToList();
    }

    public unsafe void AddOverworldActorToGPose(ICharacter character)
    {
        var localPlayer = _objectTable.LocalPlayer;
        if(localPlayer != null)
        {
            var playerNative = localPlayer.Native();
            var characterNative = character.Native();
            characterNative->Position = playerNative->Position;
            characterNative->DefaultPosition = playerNative->Position;
            characterNative->Rotation = playerNative->Rotation;
            characterNative->DefaultRotation = playerNative->Rotation;
        }
        _targetService.GPoseTarget = character;
    }
}
