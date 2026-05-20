using StarfallStudio.API;
using StarfallStudio.API.Interface;
using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Entities;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Actor.Extensions;
using StarfallStudio.Game.Core;
using StarfallStudio.Game.GPose;
using StarfallStudio.MCDF.Game.Services;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;

namespace StarfallStudio.IPC.API;

public unsafe class ActorAPI(ActorSpawnService actorSpawnService, MCDFService mCDFService, IFramework framework, GPoseService gPoseService, EntityManager entityManager) : IActor
{
    private readonly MCDFService _mCDFService = mCDFService;
    private readonly GPoseService _gPoseService = gPoseService;
    private readonly ActorSpawnService _actorSpawnService = actorSpawnService;
    private readonly EntityManager _entityManager = entityManager;
    private readonly IFramework _framework = framework;

    public bool Despawn(IGameObject gameObject)
    {
        if(_gPoseService.IsGPosing == false) return false;

        return _actorSpawnService.DestroyObject(gameObject);
    }

    public bool Exists(IGameObject gameObject)
    {
        if(_gPoseService.IsGPosing == false) return false;

        return _entityManager.EntityExists(gameObject.Native());
    }

    public IGameObject[]? GetAllActors()
    {
        if(_gPoseService.IsGPosing == false) return null;

        return [.. _entityManager.TryGetAllActorsAsGameObject()];
    }

    public StarfallStudioApiResult LoadMCDF(IGameObject gameObject, string path)
    {
        if(_gPoseService.IsGPosing == false) return StarfallStudioApiResult.IsNotInGPose;

        if(_entityManager.TryGetEntity(gameObject.Native(), out var entity))
        {
            if(entity.TryGetCapability<ActorAppearanceCapability>(out var actorAppearanceCapability))
            {
                return actorAppearanceCapability.LoadMCDF(path).Result;
            }
        }

        return StarfallStudioApiResult.UnknownError;
    }

    public StarfallStudioApiResult SaveMCDF(IGameObject gameObject, string path, string description)
    {
        if(_gPoseService.IsGPosing == false) return StarfallStudioApiResult.IsNotInGPose;

        if(_entityManager.TryGetEntity(gameObject.Native(), out var entity))
        {
            if(entity.TryGetCapability<ActorAppearanceCapability>(out var actorAppearanceCapability))
            {
                return actorAppearanceCapability.SaveMcdf(path, description).Result;
            }
        }

        return StarfallStudioApiResult.UnknownError;
    }

    public IGameObject? Spawn(global::StarfallStudio.API.Enums.SpawnFlags spawnFlags, bool selectInHierarchy, bool spawnFrozen)
    {
        if(_gPoseService.IsGPosing == false) return null;

        var flags = (SpawnFlags)(ulong)spawnFlags;

        if(_actorSpawnService.CreateCharacter(out var character, flags, disableSpawnCompanion: !flags.HasFlag(SpawnFlags.ReserveCompanionSlot)))
        {
            if(selectInHierarchy)
            {
                _entityManager.SetSelectedEntity(character);
            }

            if(spawnFrozen)
            {
                _framework.RunUntilSatisfied(
                    () => character.Native()->IsReadyToDraw(),
                    (__) =>
                    {
                        if(_entityManager.TryGetEntity(character.Native(), out var entity))
                        {
                            if(entity.TryGetCapability<ActionTimelineCapability>(out var actionTimeline))
                            {
                                actionTimeline.SetOverallSpeedOverride(0);
                            }
                        }
                    },
                    100,
                    dontStartFor: 2
                );
            }

            return character;
        }

        return null;
    }
}
