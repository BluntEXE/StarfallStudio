using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Config;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Posing;
using StarfallStudio.UI.Widgets.Actor;
using System.Collections.Generic;

namespace StarfallStudio.Capabilities.Actor;

public class ActorDebugCapability : ActorCharacterCapability
{

    public bool IsDebug => _configService.IsDebug;

    private readonly ConfigurationService _configService;
    private readonly ActorVFXService _vfxService;
    private readonly SkeletonService _skeletonService;

    public SkeletonService SkeletonService => _skeletonService;

    public ActorDebugCapability(ActorEntity parent, SkeletonService skeletonService, ConfigurationService configService, ActorVFXService actorVFXService) : base(parent)
    {
        _configService = configService;
        _vfxService = actorVFXService;
        _skeletonService = skeletonService;

        Widget = new ActorDebugWidget(this);
    }

    public Dictionary<string, int> SkeletonStacks
    {
        get
        {
            if(Entity.TryGetCapability<SkeletonPosingCapability>(out var capability))
                return capability.PoseInfo.StackCounts;

            return [];
        }
    }

    public ActorVFXService VFXService => _vfxService;
}
