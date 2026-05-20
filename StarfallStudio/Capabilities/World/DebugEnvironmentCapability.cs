using StarfallStudio.Capabilities.Core;
using StarfallStudio.Config;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.GPose;
using StarfallStudio.Game.World;
using StarfallStudio.IPC;
using StarfallStudio.UI.Widgets.World;

namespace StarfallStudio.Capabilities.World;

public class DebugEnvironmentCapability : Capability
{
    public bool IsDebug => _configService.IsDebug;

    public DynamisService DynamisIPC => _dynamisIPC;
    public EnvironmentService Environment => _environmentService;

    public GPoseService GPoseService => _gPoseService;

    public readonly EnvironmentService _environmentService;
    private readonly ConfigurationService _configService;
    private readonly DynamisService _dynamisIPC;
    private readonly GPoseService _gPoseService;

    public DebugEnvironmentCapability(Entity parent, GPoseService gPoseService, EnvironmentService environmentService, DynamisService dynamisIPC, ConfigurationService configService) : base(parent)
    {
        _environmentService = environmentService;

        _configService = configService;
        _dynamisIPC = dynamisIPC;
        _gPoseService = gPoseService;

        Widget = new DebugEnvironmentWidget(this);
    }
}
