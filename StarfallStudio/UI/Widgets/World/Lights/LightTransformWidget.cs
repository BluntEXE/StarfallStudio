using StarfallStudio.Capabilities.World;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Widgets.Core;

namespace StarfallStudio.UI.Widgets.World.Lights;

public class LightTransformWidget(LightTransformCapability lightGizmoCapability) : Widget<LightTransformCapability>(lightGizmoCapability)
{
    public override string HeaderName => "Light Transform";

    public override WidgetFlags Flags => WidgetFlags.DrawBody | WidgetFlags.DefaultOpen | WidgetFlags.CanHide;


    bool state = false;
    public unsafe override void DrawBody()
    {
        LightEditor.DrawLightTransform(Capability, ref state);
    }
}
