using StarfallStudio.Config;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Input;
using StarfallStudio.UI.Widgets.World.Lights;
using StarfallStudio.UI.Windows.Specialized;

namespace StarfallStudio.Capabilities.World;

public class LightRenderingCapability : LightCapability
{
    public bool HasOverride
    {
        get => true;
    }

    public int SelectedLightType = -1;

    //

    private readonly PosingTransformWindow _overlayTransformWindow;
    private readonly PosingOverlayWindow _overlayWindow;
    private readonly GameInputService _gameInputService;
    private readonly ConfigurationService _configurationService;

    public LightRenderingCapability(
        Entity parent,
        GameInputService gameInputService,
        ConfigurationService configurationService,
        PosingTransformWindow overlayTransformWindow,
        PosingOverlayWindow window)
        : base(parent)
    {
        _overlayWindow = window;
        _gameInputService = gameInputService;
        _overlayTransformWindow = overlayTransformWindow;
        _configurationService = configurationService;

        Widget = new LightRenderingWidget(this);
    }

    public void Reset(bool generateSnapshot = true, bool reset = true, bool clearHistStack = true)
    {

    }
}
