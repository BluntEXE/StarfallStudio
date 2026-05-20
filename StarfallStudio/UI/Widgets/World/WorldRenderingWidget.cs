using StarfallStudio.Capabilities.World;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;

namespace StarfallStudio.UI.Widgets.World;

public class WorldRenderingWidget(WorldRenderingCapability worldRenderingCapability) : Widget<WorldRenderingCapability>(worldRenderingCapability)
{
    public override string HeaderName => "Advanced";
    public override WidgetFlags Flags => WidgetFlags.DrawBody;

    public override void DrawBody()
    {
        var isWaterFrozen = Capability.WorldRenderingService.IsWaterFrozen;

        if(ImGui.Checkbox("Freeze Water", ref isWaterFrozen))
        {
            Capability.WorldRenderingService.IsWaterFrozen = isWaterFrozen;
        }
    }
}
