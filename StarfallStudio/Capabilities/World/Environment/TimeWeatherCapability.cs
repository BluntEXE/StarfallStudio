using StarfallStudio.Capabilities.Core;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Widgets.World;

namespace StarfallStudio.Capabilities.World;

public class TimeWeatherCapability : Capability
{
    public EnvironmentService EnvironmentService { get; }
    public TimeService TimeService { get; }

    public TimeWeatherCapability(Entity parent, TimeService timeService, EnvironmentService weatherService) : base(parent)
    {
        EnvironmentService = weatherService;
        TimeService = timeService;

        Widget = new TimeWeatherWidget(this);
    }
}
