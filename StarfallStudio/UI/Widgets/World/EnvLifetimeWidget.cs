using StarfallStudio.Capabilities.World;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;

namespace StarfallStudio.UI.Widgets.World;

public class EnvLifetimeWidget : Widget<EnvironmentLifetimeCapability>
{
    private readonly ActorSpawnService _actorSpawnService;
    private readonly VirtualCameraManager _cameraManager;
    private readonly LightingService _lightingService;

    public EnvLifetimeWidget(EnvironmentLifetimeCapability environmentLifetimeCapability, ActorSpawnService actorSpawnService, VirtualCameraManager cameraManager, LightingService lightingService) : base(environmentLifetimeCapability)
    {
        _actorSpawnService = actorSpawnService;
        _cameraManager = cameraManager;
        _lightingService = lightingService;
    }

    public override string HeaderName => "Lifetime";

    public override WidgetFlags Flags => WidgetFlags.DrawPopup | WidgetFlags.DrawQuickIcons;

    public override void DrawQuickIcons()
    {
        if(ImStarfallStudio.FontIconButton("lifetimewidget_spawnnew", FontAwesomeIcon.Plus, "Spawn New"))
        {
            ImGui.OpenPopup("UnifiedSpawnMenuPopup");
        }
        SpawnMenuEditor.DrawUnifiedSpawnMenu(_actorSpawnService, _cameraManager, _lightingService);

        //ImGui.SameLine();

        //if(ImStarfallStudio.FontIconButtonRight($"lifetimewidget_openAdvaned", FontAwesomeIcon.SquareArrowUpRight, 1, Capability.IsLightWindowOpen ? "Close Light Window" : "Open Light Window"))
        //{
        //    Capability.ToggleLightWindow();
        //}
    }

    public override void DrawPopup()
    {

    }
}
