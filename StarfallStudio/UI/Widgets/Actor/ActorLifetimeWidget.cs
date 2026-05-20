using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;

namespace StarfallStudio.UI.Widgets.Actor;

public class ActorLifetimeWidget : Widget<ActorLifetimeCapability>
{
    private readonly ActorSpawnService _actorSpawnService;
    private readonly VirtualCameraManager _cameraManager;
    private readonly LightingService _lightingService;

    public ActorLifetimeWidget(ActorLifetimeCapability capability, ActorSpawnService actorSpawnService, VirtualCameraManager cameraManager, LightingService lightingService) : base(capability)
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

        if(ImStarfallStudio.FontIconButton("lifetimewidget_spawn_prop", FontAwesomeIcon.Cubes, "Spawn Prop"))
        {
            Capability.SpawnNewProp(false);
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_move_to_camera", FontAwesomeIcon.Thumbtack, "Move to Camera"))
        {
            Capability.MoveToCamera();
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_clone", FontAwesomeIcon.Clone, "Clone", Capability.CanClone))
        {
            Capability.Clone(false);
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_target", FontAwesomeIcon.Bullseye, "Target"))
        {
            Capability.Target();
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_destroy", FontAwesomeIcon.Trash, "Destroy", Capability.CanDestroy))
        {
            Capability.Destroy();
        }

        ImGui.SameLine();

        if(ImStarfallStudio.FontIconButton("lifetimewidget_rename", FontAwesomeIcon.Signature, "Rename"))
        {
            RenameActorModal.Open(Capability.Actor);
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
                Capability.Clone(true);
            }
        }

        if(Capability.CanDestroy)
        {
            if(ImGui.BeginMenu("Destroy###actorlifetime_destroy"))
            {
                if(ImGui.MenuItem("Confirm Destruction###actorlifetime_destroy_confirm"))
                {
                    Capability.Destroy();
                }

                ImGui.EndMenu();
            }
        }

        if(ImGui.MenuItem($"Rename {Capability.Actor.FriendlyName}###actorlifetime_rename"))
        {
            ImGui.CloseCurrentPopup();

            RenameActorModal.Open(Capability.Actor);
        }

        if(ImGui.MenuItem("Target###actorlifetime_target"))
        {
            Capability.Target();
        }

    }
}
