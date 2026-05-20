using StarfallStudio.Capabilities.World;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;

namespace StarfallStudio.UI.Widgets.World.Lights;

public class LightLifetimeWidget : Widget<LightLifetimeCapability>
{
    private readonly ActorSpawnService _actorSpawnService;
    private readonly VirtualCameraManager _cameraManager;
    private readonly LightingService _lightingService;

    public LightLifetimeWidget(LightLifetimeCapability lightLifetimeCapability, ActorSpawnService actorSpawnService, VirtualCameraManager cameraManager, LightingService lightingService) : base(lightLifetimeCapability)
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

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_move_to_camera", FontAwesomeIcon.Thumbtack, "Move to Camera"))
        {
            Capability.MoveToCamera();
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_clone", FontAwesomeIcon.Clone, "Clone Light", Capability.CanClone))
        {
            Capability.Clone();
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_destroy", FontAwesomeIcon.Trash, "Destroy Light", Capability.CanDestroy))
        {
            Capability.Destroy();
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_rename", FontAwesomeIcon.Signature, "Rename Light"))
        {
            RenameActorModal.Open(Capability.Entity);
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButtonRight($"lifetimewidget_openAdvaned", FontAwesomeIcon.SquareArrowUpRight, 1, Capability.IsLightWindowOpen ? "Close Light Window" : "Open Light Window"))
        {
            Capability.ToggleLightWindow();
        }
    }

    public override void DrawPopup()
    {
        if(ImGui.MenuItem("Move to Camera###actorlifetime_move_to_camera"))
        {
            Capability.MoveToCamera();
        }

        if(Capability.CanClone)
        {
            if(ImGui.MenuItem("Clone###actorlifetime_clone"))
            {
                Capability.Clone();
            }
        }

        if(Capability.CanDestroy)
        {
            if(ImGui.MenuItem("Destroy###actorlifetime_destroy"))
            {
                Capability.Destroy();
            }
        }

        if(ImGui.MenuItem($"Rename {Capability.Entity.FriendlyName}###actorlifetime_rename"))
        {
            ImGui.CloseCurrentPopup();

            RenameActorModal.Open(Capability.Entity);
        }

        if(ImGui.MenuItem("Open Light Window###actorlifetime_lightwindow"))
        {
            Capability.OpenLightWindow();
        }
    }
}
