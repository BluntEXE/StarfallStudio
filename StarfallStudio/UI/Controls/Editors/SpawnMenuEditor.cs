using StarfallStudio.Entities.Camera;
using StarfallStudio.Game.Actor;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.World;
using StarfallStudio.UI.Controls.Core;
using StarfallStudio.UI.Controls.Stateless;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using System.Numerics;

namespace StarfallStudio.UI.Controls.Editors;

public static class SpawnMenuEditor
{
    private const float MenuWidth = 220f;

    public static void DrawUnifiedSpawnMenu(
        ActorSpawnService? actorSpawnService = null,
        VirtualCameraManager? cameraManager = null,
        LightingService? lightingService = null)
    {
        using var popup = ImRaii.Popup("UnifiedSpawnMenuPopup");
        if(!popup.Success)
            return;

        var buttonSize = new Vector2(MenuWidth * ImGuiHelpers.GlobalScale, 0);

        using(ImRaii.PushColor(ImGuiCol.Button, UIConstants.Transparent))
        {
            // Actor spawn
            if(actorSpawnService != null)
            {
                ImGui.Text("Actors");
                ImGui.Separator();

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.User, "Spawn New Actor", buttonSize))
                {
                    actorSpawnService.CreateCharacter(out _, SpawnFlags.Default, true);
                    ImGui.CloseCurrentPopup();
                }

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.PlusSquare, "Spawn with Companion Slot", buttonSize))
                {
                    actorSpawnService.CreateCharacter(out _, SpawnFlags.WithCompanionSlot, false);
                    ImGui.CloseCurrentPopup();
                }

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Cubes, "Spawn Prop", buttonSize))
                {
                    actorSpawnService.SpawnNewProp(out _);
                    ImGui.CloseCurrentPopup();
                }
            }

            // Camera spawn
            if(cameraManager != null)
            {
                if(actorSpawnService != null)
                    ImGui.Spacing();

                ImGui.Text("Cameras");
                ImGui.Separator();

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Camera, "New StarfallStudio Camera", buttonSize))
                {
                    cameraManager.CreateCamera(CameraType.Game);
                    ImGui.CloseCurrentPopup();
                }

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Video, "New Free-Cam", buttonSize))
                {
                    cameraManager.CreateCamera(CameraType.Free);
                    ImGui.CloseCurrentPopup();
                }
            }

            // Light spawn
            if(lightingService != null)
            {
                if(actorSpawnService != null || cameraManager != null)
                    ImGui.Spacing();

                ImGui.Text("Lights");
                ImGui.Separator();

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Lightbulb, "Spawn Spot Light", buttonSize))
                {
                    lightingService.SpawnLight(LightType.SpotLight);
                    ImGui.CloseCurrentPopup();
                }

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Circle, "Spawn Point Light", buttonSize))
                {
                    lightingService.SpawnLight(LightType.PointLight);
                    ImGui.CloseCurrentPopup();
                }

                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Square, "Spawn Flat Light", buttonSize))
                {
                    lightingService.SpawnLight(LightType.FlatLight);
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }
}

