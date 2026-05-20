using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Core;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Widgets.Actor;
using Dalamud.Game.ClientState.Objects.Types;
using System;

namespace StarfallStudio.Capabilities.Actor;

public class ActorContainerCapability : Capability
{
    private readonly EntityManager _entityManager;
    private readonly ActorSpawnService _actorSpawnService;
    private readonly TargetService _targetService;
    private readonly GPoseService _gPoseService;

    public bool CanControlCharacters => _gPoseService.IsGPosing;

    public ActorContainerCapability(ActorContainerEntity parent, EntityManager entityManager, ActorSpawnService actorSpawnService, TargetService targetService, GPoseService gPoseService) : base(parent)
    {
        _entityManager = entityManager;
        _actorSpawnService = actorSpawnService;
        _targetService = targetService;
        _gPoseService = gPoseService;
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
}
