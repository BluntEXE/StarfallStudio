using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Core;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Widgets.Actor;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;

namespace StarfallStudio.Capabilities.Actor;

public class ActorContainerCapability : Capability
{
    private readonly EntityManager _entityManager;
    private readonly ActorSpawnService _actorSpawnService;
    private readonly TargetService _targetService;
    private readonly GPoseService _gPoseService;
    private readonly IObjectTable _objectTable;

    public bool CanControlCharacters => _gPoseService.IsGPosing;

    public ActorContainerCapability(ActorContainerEntity parent, EntityManager entityManager, ActorSpawnService actorSpawnService, TargetService targetService, GPoseService gPoseService, IObjectTable objectTable) : base(parent)
    {
        _entityManager = entityManager;
        _actorSpawnService = actorSpawnService;
        _targetService = targetService;
        _gPoseService = gPoseService;
        _objectTable = objectTable;
        Widget = new ActorContainerWidget(this);
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
            if(selectInHierarchy)
            {
                _entityManager.SetSelectedEntity(character!);
            }
        }
    }

    public void DestroyCharacter(ActorEntity entity)
    {
        _actorSpawnService.DestroyObject(entity.GameObject);
    }

    public void CloneActor(ActorEntity entity, bool targetNewInHierarchy)
    {
        if(entity.GameObject is ICharacter character)
        {
            if(_actorSpawnService.CloneCharacter(character, out var chara))
            {
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

    public List<ICharacter> GetOverworldActors()
    {
        var result = new List<ICharacter>();
        for(int i = ActorTableHelpers.OverworldStart; i <= ActorTableHelpers.OverworldEnd; i++)
        {
            var obj = _objectTable[i];
            if(obj is ICharacter character && obj.IsValid())
                result.Add(character);
        }
        return result;
    }

    public void AddOverworldActorToGPose(ICharacter character)
    {
        _gPoseService.AddCharacterToGPose(character);
        _entityManager.SetSelectedEntity(character);
    }
}
