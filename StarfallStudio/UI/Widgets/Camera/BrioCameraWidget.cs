using StarfallStudio.Capabilities.Camera;
using StarfallStudio.Entities.Camera;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;

namespace StarfallStudio.UI.Widgets.Camera;

public class StarfallStudioCameraWidget(StarfallStudioCameraCapability capability) : Widget<StarfallStudioCameraCapability>(capability)
{
    public override string HeaderName => "Camera Editor";

    public override WidgetFlags Flags => WidgetFlags.DrawBody | WidgetFlags.DefaultOpen | WidgetFlags.HasAdvanced;

    public unsafe override void DrawBody()
    {
        if(Capability.CameraEntity.CameraType == CameraType.Free)
        {
            CameraEditor.DrawFreeCam("camera_widget_editor", Capability);
        }
        else if(Capability.CameraEntity.CameraType == CameraType.Cutscene)
        {
            if(ImGui.Button("Open Camera Window"))
            {
                Capability.ShowCameraWindow();
            }
            ImStarfallStudio.TextCentered("Open the Camera Window to edit play a Cutscene ", ImGui.GetWindowContentRegionMax().X);

        }
        else
        {
            CameraEditor.DrawStarfallStudioCam("camera_widget_editor", Capability);
        }
    }

    public override void ToggleAdvancedWindow() => Capability.ShowCameraWindow();
}
