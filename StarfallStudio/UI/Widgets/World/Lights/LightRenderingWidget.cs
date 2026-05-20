using StarfallStudio.Capabilities.World;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;

namespace StarfallStudio.UI.Widgets.World.Lights;

public class LightRenderingWidget(LightRenderingCapability lightRenderingCapability) : Widget<LightRenderingCapability>(lightRenderingCapability)
{
    public override string HeaderName => "Light Properties";

    public override WidgetFlags Flags => WidgetFlags.DefaultOpen | WidgetFlags.DrawBody | WidgetFlags.DrawPopup | WidgetFlags.CanHide;

    public override void DrawPopup()
    {
        var togglenText = Capability.GameLight.IsVisible ? $"Turn OFF {Capability.Entity.FriendlyName}" : $"Turn ON {Capability.Entity.FriendlyName}";
        if(ImGui.MenuItem($"{togglenText}###togglelight"))
        {
            Capability.GameLight.ToggleLight();
        }
    }

    public unsafe override void DrawBody()
    {
        LightEditor.DrawLightProperties(Capability);

        ImStarfallStudio.VerticalPadding(5);

        if(ImGui.CollapsingHeader("Advanced Shadows Settings"u8, ImGuiTreeNodeFlags.None))
        {
            LightEditor.DrawAdvancedShadows(Capability);
        }

        if(ImGui.CollapsingHeader("Advanced Settings"u8, ImGuiTreeNodeFlags.None))
        {
            LightEditor.DrawAdvancedSettings(Capability);
        }
    }
}
