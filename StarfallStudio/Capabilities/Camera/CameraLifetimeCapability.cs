using StarfallStudio.Entities.Camera;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.GPose;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.Camera;

namespace StarfallStudio.Capabilities.Camera;

public class CameraLifetimeCapability : CameraCapability
{
    private readonly VirtualCameraManager _virtualCameraManager;

    public VirtualCameraManager VirtualCameraManager => _virtualCameraManager;

    public CameraLifetimeCapability(CameraEntity parent, GPoseService gPoseService, VirtualCameraManager virtualCameraManager, ActorSpawnService actorSpawnService, LightingService lightingService) : base(parent, gPoseService)
    {
        _virtualCameraManager = virtualCameraManager;

        Widget = new CameraLifetimeWidget(this, actorSpawnService, lightingService);
    }

    public bool CanDestroy => CameraEntity.CameraID != 0;
}
