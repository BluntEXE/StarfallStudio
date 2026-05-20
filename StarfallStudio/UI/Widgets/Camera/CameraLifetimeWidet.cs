using StarfallStudio.Capabilities.Camera;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;

namespace StarfallStudio.UI.Widgets.Camera;

public class CameraLifetimeWidget : Widget<CameraLifetimeCapability>
{
    private readonly ActorSpawnService _actorSpawnService;
    private readonly LightingService _lightingService;

    public CameraLifetimeWidget(CameraLifetimeCapability capability, ActorSpawnService actorSpawnService, LightingService lightingService) : base(capability)
    {
        _actorSpawnService = actorSpawnService;
        _lightingService = lightingService;
    }

    public override string HeaderName => "Lifetime";

    public override WidgetFlags Flags => WidgetFlags.DrawPopup | WidgetFlags.DrawQuickIcons;

    public override void DrawQuickIcons()
    {
        using(ImRaii.Disabled(Capability.IsAllowed == false))
        {
            if(ImStarfallStudio.FontIconButton("CameraLifetime_spawnnew", FontAwesomeIcon.Plus, "Spawn New"))
            {
                ImGui.OpenPopup("UnifiedSpawnMenuPopup");
            }
            SpawnMenuEditor.DrawUnifiedSpawnMenu(_actorSpawnService, Capability.VirtualCameraManager, _lightingService);

            ImGui.SameLine();

            if(ImStarfallStudio.FontIconButton("CameraLifetime_clone", FontAwesomeIcon.Clone, "Clone Camera"))
            {
                Capability.VirtualCameraManager.CloneCamera(Capability.CameraEntity.CameraID);
            }

            ImGui.SameLine();

            using(ImRaii.Disabled(Capability.CameraEntity.CameraID == 0))
            {
                if(ImStarfallStudio.FontIconButton("CameraLifetime_destroy", FontAwesomeIcon.Trash, "Destroy Camera", Capability.CanDestroy))
                {
                    Capability.VirtualCameraManager.DestroyCamera(Capability.CameraEntity.CameraID);
                }

                ImGui.SameLine();

                if(ImStarfallStudio.FontIconButton("CameraLifetime_rename", FontAwesomeIcon.Signature, "Rename"))
                {
                    RenameActorModal.Open(Capability.Entity);
                }
            }

            ImGui.SameLine();

            if(ImStarfallStudio.FontIconButton("CameraLifetime_target", FontAwesomeIcon.Bullseye, "Target Camera"))
            {
                Capability.VirtualCameraManager.SelectCamera(Capability.VirtualCamera);
            }

        }
    }

    public override void DrawPopup()
    {
        if(Capability.IsAllowed == false)
            return;

        if(ImGui.MenuItem("Target###CameraLifetime_target"))
        {
            Capability.VirtualCameraManager.SelectCamera(Capability.VirtualCamera);
        }

        if(ImGui.MenuItem("Clone###CameraLifetime_clone"))
        {
            Capability.VirtualCameraManager.CloneCamera(Capability.CameraEntity.CameraID);
        }

        if(Capability.CanDestroy)
        {
            if(ImGui.BeginMenu("Destroy###actorlifetime_destroy"))
            {
                if(ImGui.MenuItem("Confirm Destruction###CameraLifetime_destroy_confirm"))
                {
                    Capability.VirtualCameraManager.DestroyCamera(Capability.CameraEntity.CameraID);
                }

                ImGui.EndMenu();
            }


            if(ImGui.MenuItem($"Rename {Capability.CameraEntity.FriendlyName}###CameraLifetime_rename"))
            {
                ImGui.CloseCurrentPopup();

                RenameActorModal.Open(Capability.Entity);
            }
        }
    }
}
