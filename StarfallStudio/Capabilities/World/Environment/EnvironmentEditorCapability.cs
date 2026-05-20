using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World;

namespace StarfallStudio.Capabilities.World;

public class EnvironmentEditorCapability : Capability
{
    public EnvironmentService Environment { get; }

    public EnvironmentEditorCapability(Entity parent, EnvironmentService weatherService) : base(parent)
    {
        Environment = weatherService;

        Widget = new EnvironmentEditorWidget(this);
    }

}
