using StarfallStudio.Capabilities.Core;
using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Core;
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

        // Overworld actors (0-199): visibility + position handled by direct Alpha write in
        // EntityActorManager. Skip the appearance system — it triggers a game redraw which
        // causes Glamourer IPC failures for unknown NPCs and resets actor position on timeout.
        if(actor.GameObject.IsOverworld())
            return;

        StarfallStudio.Log.Debug($"[ActorContainerCapability] OnActorAttached: hiding ambient actor — {actor.GameObject.Name} idx={actor.GameObject.ObjectIndex}");
        if(actor.TryGetCapability<ActorAppearanceCapability>(out var aac))
            _ = aac.MakeInvisible();
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

        // Move to local player's position (1.5 yalms in front so actor doesn't overlap).
        var localPlayer = _objectTable.LocalPlayer;
        if(localPlayer != null)
        {
            var playerPos = localPlayer.Position;
            var playerRot = localPlayer.Rotation;
            var forward = new System.Numerics.Vector3(MathF.Sin(playerRot), 0, MathF.Cos(playerRot));
            var spawnPos = playerPos + forward * 1.5f;

            actorNative->Position = spawnPos;
            actorNative->DefaultPosition = spawnPos;
            actorNative->Rotation = playerRot;
            actorNative->DefaultRotation = playerRot;

            // Set via ModelPosingCapability so the SetPosition hook maintains the position
            // against the game's per-frame movement system for overworld 0-199 actors.
            // Without this, FFXIV overrides DrawObject.Position back on every tick.
            if(actor.TryGetCapability<ModelPosingCapability>(out var mpc))
            {
                var current = mpc.Transform;
                mpc.Transform = new Transform
                {
                    Position = spawnPos,
                    Rotation = Quaternion.CreateFromYawPitchRoll(playerRot, 0, 0),
                    Scale = current.Scale == Vector3.Zero ? Vector3.One : current.Scale,
                };
            }
            else if(actorNative->DrawObject != null)
            {
                actorNative->DrawObject->Object.Position = spawnPos;
            }
        }

        // Overworld actors: visibility handled by direct Alpha=1f write in EntityActorManager.
        // Skipping appearance system avoids triggering DrawWhenReady (redraw resets position).
        if(actor.GameObject.IsOverworld())
            return;

        // Restore actor to visible.
        if(actor.TryGetCapability<ActorAppearanceCapability>(out var aac) && aac.IsHidden)
            _ = aac.MakeVisible();
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

        if(actor.GameObject.IsOverworld())
        {
            // Overworld actors: detach from hierarchy so the list stays clean.
            // The enforce loop hides them next frame. Re-add via 'Add Overworld Actor'.
            _entityManager.DetachEntity(actor, true);
            return;
        }

        // Spawned actors: hide in-place (stay in list, cheap to re-pin).
        if(actor.TryGetCapability<ActorAppearanceCapability>(out var aac) && !aac.IsHidden)
            _ = aac.MakeInvisible();
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

    private unsafe void OnGPoseStateChanged(bool isGPosing)
    {
        if(!isGPosing)
        {
            // Restore positions for all pinned actors before clearing state.
            // Without this, any actor moved by PinActor stays displaced in the open world.
            foreach(var (index, orig) in _originalPositions)
            {
                var go = _objectTable[index];
                if(go is ICharacter chara)
                {
                    var native = chara.Native();
                    native->Position = orig.Position;
                    native->DefaultPosition = orig.Position;
                    native->Rotation = orig.Rotation;
                    native->DefaultRotation = orig.Rotation;
                    // Restore DrawObject visual position if still loaded.
                    if(native->GameObject.DrawObject != null)
                        native->GameObject.DrawObject->Object.Position = orig.Position;

                    // Clear the ModelPosingCapability override so it doesn't
                    // persist into the next GPose session for this actor.
                    if(_entityManager.TryGetEntity(new EntityId(chara), out var entity)
                        && entity is ActorEntity actorEntity
                        && actorEntity.TryGetCapability<ModelPosingCapability>(out var mpc))
                    {
                        mpc.ResetTransform();
                    }
                }
            }

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
