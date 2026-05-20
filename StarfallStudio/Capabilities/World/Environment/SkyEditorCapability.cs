using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World;

namespace StarfallStudio.Capabilities.World;

public class SkyEditorCapability : Capability
{
    public EnvironmentService Environment => _environmentService;

    public readonly EnvironmentService _environmentService;

    public SkyEditorCapability(Entity parent, EnvironmentService weatherService) : base(parent)
    {
        _environmentService = weatherService;

        Widget = new SkyEditorWidget(this);
    }
}
