using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Config;
using StarfallStudio.Entities;
using StarfallStudio.Game.Posing;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Widgets.Core;

namespace StarfallStudio.UI.Widgets.Actor;

public class ActionTimelineWidget(ActionTimelineCapability capability, EntityManager entityManager, PhysicsService physicsService, ConfigurationService configService) : Widget<ActionTimelineCapability>(capability)
{
    public override string HeaderName => "Animation Control";

    public override WidgetFlags Flags => Capability.Actor.IsProp ? WidgetFlags.None : WidgetFlags.DrawBody | WidgetFlags.HasAdvanced;

    private readonly ActionTimelineEditor _editor = new(null!, null!, entityManager, physicsService, configService);

    public override void DrawBody()
    {
        _editor.Draw(false, Capability);
    }

    public override void ToggleAdvancedWindow()
    {
        UIManager.Instance.ToggleActionTimelineWindow();
    }
}
