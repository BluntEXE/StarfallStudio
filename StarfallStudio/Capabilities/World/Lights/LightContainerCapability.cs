using StarfallStudio.Entities.Core;
using StarfallStudio.Game.GPose;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World.Lights;
using StarfallStudio.UI.Windows.Specialized;

namespace StarfallStudio.Capabilities.World;

public class LightContainerCapability : LightCapability
{
    private readonly LightingService _lightingService;
    private readonly GPoseService _gPoseService;
    private readonly LightWindow _lightWindow;

    public bool IsAllowed => _gPoseService.IsGPosing;

    public LightingService LightingService => _lightingService;

    public LightContainerCapability(Entity parent, GPoseService gPoseService, LightWindow lightWindow, LightingService lightingService) : base(parent)
    {
        _lightingService = lightingService;
        _lightWindow = lightWindow;
        _gPoseService = gPoseService;

        Widget = new LightContainerWidget(this);
    }

    public void OpenLightWindow()
    {
        _lightWindow.IsOpen = true;
    }
}
