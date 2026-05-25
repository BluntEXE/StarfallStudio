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
using System.Numerics;

namespace StarfallStudio.Capabilities.Actor;

public class ActorContainerCapability : Capability
{
    private readonly EntityManager _entityManager;
    private readonly ActorSpawnService _actorSpawnService;
    private readonly TargetService _targetService;
    private readonly GPoseService _gPoseService;
    private readonly IObjectTable _objectTable;

    public bool CanControlCharacters => _gPoseService.IsGPosing;

    // Actors shown in hierarchy and visible in-world.
    private readonly HashSet<ushort> _managedActorIndices = [];

    // Original positions captured at pin-time so we can restore on unpin.
    private readonly Dictionary<ushort, (Vector3 Position, float Rotation)> _originalPositions = [];

    public ActorContainerCapability(ActorContainerEntity parent, EntityManager entityManager, ActorSpawnService actorSpawnService, TargetService targetService, GPoseService gPoseService, IObjectTable objectTable) : base(parent)
    {
        _entityManager = entityManager;
        _actorSpawnService = actorSpawnService;
        _targetService = targetService;
        _gPoseService = gPoseService;
        _objectTable = objectTable;

        _gPoseService.OnGPoseStateChange += OnGPoseStateChanged;

        Widget = new ActorContainerWidget(this);
    }

    // Fired when the user selects an overworld actor from the dropdown in open-world GPose.
    // EntityActorManager subscribes to handle attaching + pinning.
    public event Action<ICharacter>? OverworldActorAddRequested;

    public bool IsManaged(ActorEntity actor) =>
        _managedActorIndices.Contains((ushort)actor.GameObject.ObjectIndex);

    public bool IsIndexManaged(ushort index) =>
        _managedActorIndices.Contains(index);

    // Called by ActorEntity.OnAttached.
    // Auto-manages the local player's own actor; hides everyone else.
    public void OnActorAttached(ActorEntity actor)
    {
        var localPlayer = _objectTable.LocalPlayer;
        if(localPlayer != null && actor.GameObject.Name.TextValue == localPlayer.Name.TextValue)
        {
            StarfallStudio.Log.Debug($"[ActorContainerCapability] OnActorAttached: keeping player actor visible — {actor.GameObject.Name} idx={actor.GameObject.ObjectIndex}");
            _managedActorIndices.Add((ushort)actor.GameObject.ObjectIndex);
            return;
        }

        StarfallStudio.Log.Debug($"[ActorContainerCapability] OnActorAttached: hiding ambient actor — {actor.GameObject.Name} idx={actor.GameObject.ObjectIndex}");
        // All other ambient actors start hidden. Show() sets Transparency=0 (invisible).
        if(actor.TryGetCapability<ActorAppearanceCapability>(out var aac))
            _ = aac.Show();
        else
            StarfallStudio.Log.Warning($"[ActorContainerCapability] OnActorAttached: no ActorAppearanceCapability on {actor.GameObject.Name} idx={actor.GameObject.ObjectIndex}");
    }

    public unsafe void PinActor(ActorEntity actor)
    {
        var index = (ushort)actor.GameObject.ObjectIndex;

        // Capture current position before moving so we can restore it on unpin.
        var actorNative = actor.GameObject.Native();
        _originalPositions[index] = (actorNative->Position, actorNative->Rotation);

        _managedActorIndices.Add(index);

        // Move to local player's position.
        var localPlayer = _objectTable.LocalPlayer;
        if(localPlayer != null)
        {
            var playerNative = localPlayer.Native();
            actorNative->Position = playerNative->Position;
            actorNative->DefaultPosition = playerNative->Position;
            actorNative->Rotation = playerNative->Rotation;
            actorNative->DefaultRotation = playerNative->Rotation;
        }

        // Make visible. Hide() sets Transparency=1 (visible).
        if(actor.TryGetCapability<ActorAppearanceCapability>(out var aac) && aac.IsHidden)
            _ = aac.Hide();
    }

    public unsafe void UnpinActor(ActorEntity actor)
    {
        var index = (ushort)actor.GameObject.ObjectIndex;
        _managedActorIndices.Remove(index);

        // Restore original position if we recorded one.
        if(_originalPositions.TryGetValue(index, out var orig))
        {
            var actorNative = actor.GameObject.Native();
            actorNative->Position = orig.Position;
            actorNative->DefaultPosition = orig.Position;
            actorNative->Rotation = orig.Rotation;
            actorNative->DefaultRotation = orig.Rotation;
            _originalPositions.Remove(index);
        }

        // Hide. Show() sets Transparency=0 (invisible).
        if(actor.TryGetCapability<ActorAppearanceCapability>(out var aac) && !aac.IsHidden)
            _ = aac.Show();
    }

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
                _entityManager.SetSelectedEntity(characterId);
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
                _entityManager.SetSelectedEntity(character!);
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
                _entityManager.SetSelectedEntity(character!);
        }
    }

    public void DestroyCharacter(ActorEntity entity)
    {
        var index = (ushort)entity.GameObject.ObjectIndex;
        _managedActorIndices.Remove(index);
        _originalPositions.Remove(index);

        if(entity.GameObject.IsOverworld())
        {
            // Overworld actors are real game characters — cannot despawn them.
            // Detach from hierarchy; enforce loop will hide them again next frame.
            _entityManager.DetachEntity(entity, true);
            return;
        }

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
                    _entityManager.SetSelectedEntity(chara);
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
                && o.ObjectIndex != 200
                && _validOverworldKinds.Contains(o.ObjectKind))
            .GroupBy(o => o.ObjectKind == ObjectKind.Pc ? o.Name.TextValue : o.Name.TextValue + "_" + o.ObjectIndex)
            .Select(g => g.OrderByDescending(o => o.ObjectIndex >= ActorTableHelpers.GPoseStart).First())
            .OfType<ICharacter>()
            .OrderBy(o => o.YalmDistanceX * o.YalmDistanceX + o.YalmDistanceZ * o.YalmDistanceZ)
            .ToList();
    }

    public unsafe void AddOverworldActorToGPose(ICharacter character)
    {
        // Fire first (before position move) so EntityActorManager can capture original position via PinActor.
        OverworldActorAddRequested?.Invoke(character);

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
        // Do not set GPoseTarget — camera would snap to the actor.
        // User can target manually via the bullseye button.
    }

    private void OnGPoseStateChanged(bool isGPosing)
    {
        if(!isGPosing)
        {
            _managedActorIndices.Clear();
            _originalPositions.Clear();
        }
    }

    public override void Dispose()
    {
        _gPoseService.OnGPoseStateChange -= OnGPoseStateChanged;
        base.Dispose();
    }
}
