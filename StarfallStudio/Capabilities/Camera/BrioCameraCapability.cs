using StarfallStudio.Config;
using StarfallStudio.Entities;
using StarfallStudio.Entities.Camera;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Widgets.Camera;
using StarfallStudio.UI.Windows.Specialized;

namespace StarfallStudio.Capabilities.Camera;

public class StarfallStudioCameraCapability : CameraCapability
{
    private readonly CameraWindow _cameraWindow;
    private readonly VirtualCameraManager _virtualCameraService;
    public readonly ConfigurationService _configurationService;
    public readonly EntityManager _entityManager;

    public StarfallStudioCameraCapability(CameraEntity parent, EntityManager entityManager, VirtualCameraManager virtualCameraService, GPoseService gPoseService, CameraWindow cameraWindow, ConfigurationService configService) : base(parent, gPoseService)
    {
        _virtualCameraService = virtualCameraService;
        _cameraWindow = cameraWindow;
        _entityManager = entityManager;

        _configurationService = configService;

        Widget = new StarfallStudioCameraWidget(this);
    }

    public override void OnEntitySelected()
    {
        _virtualCameraService.SelectCamera(VirtualCamera);
    }

    public void ShowCameraWindow()
    {
        _cameraWindow.IsOpen = true;
    }
}
