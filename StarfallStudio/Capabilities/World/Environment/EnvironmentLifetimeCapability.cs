using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World;
using StarfallStudio.UI.Windows.Specialized;

namespace StarfallStudio.Capabilities.World;

public class EnvironmentLifetimeCapability : LightCapability
{
    private readonly LightingService _lightingService;
    private readonly LightWindow _lightWindow;

    public EnvironmentLifetimeCapability(Entity parent, ActorSpawnService actorSpawnService, VirtualCameraManager cameraManager, LightingService lightingService, LightWindow lightWindow) : base(parent)
    {
        _lightingService = lightingService;
        _lightWindow = lightWindow;

        this.Widget = new EnvLifetimeWidget(this, actorSpawnService, cameraManager, lightingService);
    }

    public bool IsLightWindowOpen => _lightWindow.IsOpen;

    public void ToggleLightWindow()
    {
        _lightWindow.IsOpen = !_lightWindow.IsOpen;
    }
    public void OpenLightWindow()
    {
        _lightWindow.IsOpen = true;
    }

}
