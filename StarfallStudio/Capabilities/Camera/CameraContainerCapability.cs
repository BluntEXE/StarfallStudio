using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Widgets.Camera;
using StarfallStudio.UI.Windows.Specialized;

namespace StarfallStudio.Capabilities.Camera;

public class CameraContainerCapability : Capability
{
    private readonly VirtualCameraManager _virtualCameraService;
    private readonly GPoseService _gPoseService;
    private readonly CameraWindow _cameraWindow;
    public bool IsAllowed => _gPoseService.IsGPosing;

    public VirtualCameraManager VirtualCameraManager => _virtualCameraService;
    public VirtualCamera CurrentCamera => VirtualCameraManager.CurrentCamera!;

    public CameraContainerCapability(Entity parent, CameraWindow cameraWindow, GPoseService gPoseService, VirtualCameraManager virtualCameraService) : base(parent)
    {
        _gPoseService = gPoseService;
        _virtualCameraService = virtualCameraService;
        _cameraWindow = cameraWindow;

        Widget = new CameraContainerWidget(this);
    }

    public void OpenCameraWindow()
    {
        _cameraWindow.IsOpen = true;
    }

}
