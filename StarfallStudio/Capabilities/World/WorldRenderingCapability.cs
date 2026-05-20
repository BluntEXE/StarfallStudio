using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World;

namespace StarfallStudio.Capabilities.World;

public class WorldRenderingCapability : Capability
{
    public WorldRenderingService WorldRenderingService { get; }

    public WorldRenderingCapability(Entity parent, WorldRenderingService service) : base(parent)
    {
        WorldRenderingService = service;
        Widget = new WorldRenderingWidget(this);
    }
}
