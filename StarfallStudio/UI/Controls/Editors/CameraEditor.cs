using StarfallStudio.Capabilities.Camera;
using StarfallStudio.Config;
using StarfallStudio.Entities.Actor;
using StarfallStudio.Entities.Camera;
using StarfallStudio.Files;
using StarfallStudio.Game.Actor.Extensions;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.Cutscene;
using StarfallStudio.UI.Controls.Core;
using StarfallStudio.UI.Controls.Stateless;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System.IO;
using System.Numerics;

namespace StarfallStudio.UI.Controls.Editors;

public static class CameraEditor
{
    public unsafe static void DrawSpawnMenu(VirtualCameraManager virtualCameraManager)
    {
        using var popup = ImRaii.Popup("DrawSpawnMenuPopup");
        if(popup.Success)
        {
            using(ImRaii.PushColor(ImGuiCol.Button, UIConstants.Transparent))
            {
                if(ImGui.Button("New StarfallStudio Camera"u8, new(155 * ImGuiHelpers.GlobalScale, 0)))
                {
                    virtualCameraManager.CreateCamera(CameraType.Game);
                }

                if(ImGui.Button("New Free-Cam"u8, new(155 * ImGuiHelpers.GlobalScale, 0)))
                {
                    virtualCameraManager.CreateCamera(CameraType.Free);
                }

                //ImGui.Separator();

                //if(ImGui.Button("New Cutscene Camera"))
                //{
                //    virtualCameraManager.CreateCamera(CameraType.Cutscene);
                //}
            }
        }
    }

    public unsafe static void DrawFreeCam(string id, StarfallStudioCameraCapability capability)
    {
        var camera = capability.VirtualCamera;

        using(ImRaii.PushId(id))
        {
            using(ImRaii.Disabled(!capability.IsAllowed))
            {
                var width = -ImGui.CalcTextSize("XXXXXXXx").X;

                if(ImStarfallStudio.ToggelButton("Enable Movement", new Vector2(135, 25), camera.FreeCamValues.IsMovementEnabled, hoverText: camera.FreeCamValues.IsMovementEnabled ?
                    "Disable Free-Cam Movement" : "Enabled Free-Cam Movement"))
                {
                    camera.FreeCamValues.IsMovementEnabled = !camera.FreeCamValues.IsMovementEnabled;
                }

                ImGui.SameLine();

                using(ImRaii.Disabled(camera.FreeCamValues.IsMovementEnabled == false))
                {
                    if(ImStarfallStudio.ToggelFontIconButton("LateralMovement", FontAwesomeIcon.SolarPanel, new Vector2(25, 0), camera.FreeCamValues.Move2D, hoverText: "Lateral Movement"))
                    {
                        camera.FreeCamValues.Move2D = !camera.FreeCamValues.Move2D;
                    }
                }

                ImGui.SameLine();

                if(ImStarfallStudio.FontIconButtonRight("reset", FontAwesomeIcon.Undo, 1f, "Reset Camera", camera.IsOverridden))
                    camera.ResetCamera();

                //
                ImGui.Separator();
                //

                {
                    ImStarfallStudio.Icon(FontAwesomeIcon.ArrowsToCircle);

                    ImGui.SameLine();

                    ImGui.SetNextItemWidth(width);
                    var position = camera.Position;
                    if(ImGui.DragFloat3("Position", ref position, 0.001f))
                        camera.Position = position;
                }

                {
                    ImStarfallStudio.Icon(FontAwesomeIcon.ArrowsSpin);

                    ImGui.SameLine();

                    ImGui.SetNextItemWidth(width);
                    var rotation = camera.Rotation;
                    if(ImStarfallStudio.DragFloat2V3("Pan", ref rotation, -360, 360, "%.3f", true, ImGuiSliderFlags.AlwaysClamp))
                        camera.Rotation = rotation;
                }

                {
                    ImStarfallStudio.Icon(FontAwesomeIcon.Panorama);

                    ImGui.SameLine();

                    var fov = camera.FoV;
                    if(ImStarfallStudio.SliderAngle("##FoV", ref fov, -44, 120, "%.2f", ImGuiSliderFlags.AlwaysClamp))
                        camera.FoV = fov;
                }

                {
                    ImStarfallStudio.Icon(FontAwesomeIcon.CameraRotate);

                    ImGui.SameLine();

                    float pivot = camera.PivotRotation;
                    if(ImStarfallStudio.SliderAngle("##PivotRotation", ref pivot, -180, 180, "%.2f"))
                        camera.PivotRotation = pivot;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetPivotRotation", FontAwesomeIcon.Undo, 1f, "Reset Pivot", camera.PivotRotation != 0))
                        camera.PivotRotation = 0;
                }

                ImGui.Separator();

                {
                    ImStarfallStudio.Icon(FontAwesomeIcon.Walking);

                    ImGui.SameLine();

                    float moveSpeed = camera.FreeCamValues.MovementSpeed;
                    if(ImStarfallStudio.SliderFloat("##MovementSpeed", ref moveSpeed, 0.005f, 0.3f, "%.4f", ImGuiSliderFlags.None, step: 0.001f))
                        camera.FreeCamValues.MovementSpeed = moveSpeed;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetMovementSpeed", FontAwesomeIcon.Undo, 1f, "Reset Movement Speed", moveSpeed != capability._configurationService.Configuration.Interface.DefaultFreeCameraMovementSpeed))
                        camera.FreeCamValues.MovementSpeed = capability._configurationService.Configuration.Interface.DefaultFreeCameraMovementSpeed;
                }

                {
                    ImStarfallStudio.Icon(FontAwesomeIcon.Mouse);

                    ImGui.SameLine();

                    float mouseSpeed = camera.FreeCamValues.MouseSensitivity;
                    if(ImStarfallStudio.SliderFloat("##MouseSensitivity", ref mouseSpeed, 0.001f, 0.2f, "%.4f", ImGuiSliderFlags.None, step: 0.001f))
                        camera.FreeCamValues.MouseSensitivity = mouseSpeed;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetMouseSensitivity", FontAwesomeIcon.Undo, 1f, "Reset MouseSensitivity", mouseSpeed != capability._configurationService.Configuration.Interface.DefaultFreeCameraMouseSensitivity))
                        camera.FreeCamValues.MouseSensitivity = capability._configurationService.Configuration.Interface.DefaultFreeCameraMouseSensitivity;
                }

                ImGui.Separator();

                var delimit = camera.FreeCamValues.DelimitAngle;
                if(ImGui.Checkbox("Delimit Camera Angle", ref delimit))
                    camera.FreeCamValues.DelimitAngle = delimit;
            }
        }
    }

    public unsafe static void DrawStarfallStudioCam(string id, StarfallStudioCameraCapability capability)
    {
        var camera = capability.VirtualCamera;

        using(ImRaii.PushId(id))
        {
            using(ImRaii.Disabled(!capability.IsAllowed))
            {
                if(camera is not null)
                {

                    var width = -ImGui.CalcTextSize("XXXXXXXXXx").X;

                    if(ImStarfallStudio.FontIconButtonRight("reset", FontAwesomeIcon.Undo, 1f, "Reset", camera.IsOverridden))
                        camera.ResetCamera();

                    //
                    ImGui.Separator();
                    //

                    using(ImRaii.Disabled(capability._entityManager.SelectedEntity is not ActorEntity))
                        if(ImStarfallStudio.FontIconButton("recenter_on_selected", FontAwesomeIcon.Bullseye, "Recenter on Selected Actor"))
                        {
                            var entity = capability._entityManager.SelectedEntity;
                            if(entity is ActorEntity actor)
                            {
                                capability.VirtualCamera.SelectedActorName = $"Selected: [ {actor.FriendlyName} ]";
                                camera.TargetOffset = (actor.GameObject.GetDrawObject<DrawObject>()->Object.Position - ((GameObject*)actor.GameObject.Address)->Position);
                            }
                        }

                    ImGui.SameLine();

                    ImStarfallStudio.CenterNextElementWithPadding(40);

                    if(ImGui.BeginCombo($"###CameraContainerActorsWidget_{capability.Entity.Id}_list", capability.VirtualCamera.SelectedActorName))
                    {
                        foreach(var value in capability._entityManager.TryGetAllActors())
                        {
                            if(ImGui.Selectable($"[ {value.FriendlyName} ]"))
                            {
                                capability.VirtualCamera.SelectedActorName = $"Selected: [ {value.FriendlyName} ]";
                                camera.TargetOffset = (value.GameObject.GetDrawObject<DrawObject>()->Object.Position - ((GameObject*)value.GameObject.Address)->Position);
                            }
                        }
                        ImGui.EndCombo();
                    }

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("reset_selected", FontAwesomeIcon.Undo, 1f, "Reset Selected Actor", camera.IsSelectingActor))
                    {
                        camera.TargetOffset = new Vector3(0);
                        camera.SelectedActorName = "Select an actor to track";
                    }

                    {
                        const string offsetText = "Position";

                        ImStarfallStudio.Icon(FontAwesomeIcon.ArrowsToCircle);

                        ImGui.SameLine();

                        ImGui.SetNextItemWidth(width);
                        var position = camera.RealPosition;
                        using(ImRaii.Disabled(true))
                        {
                            ImGui.DragFloat3(offsetText, ref position, 0.001f);
                        }
                    }

                    {
                        const string offsetText = "Offset";

                        ImStarfallStudio.Icon(FontAwesomeIcon.ArrowsUpDownLeftRight);

                        ImGui.SameLine();

                        Vector3 pos = camera.PositionOffset;
                        ImGui.SetNextItemWidth(width);
                        if(ImGui.DragFloat3(offsetText, ref pos, 0.001f))
                            camera.PositionOffset = pos;

                        ImGui.SameLine();

                        if(ImStarfallStudio.FontIconButtonRight("resetPosition", FontAwesomeIcon.Undo, 1f, "Reset Position-Offset", camera.PositionOffset != Vector3.Zero))
                            camera.PositionOffset = Vector3.Zero;
                    }

                    ImGui.Separator();

                    const string rotationText = "Rotation";
                    float rotation = camera.PivotRotation;
                    ImGui.SetNextItemWidth(width);
                    if(ImStarfallStudio.SliderAngle(rotationText, ref rotation, -180, 180, "%.2f"))
                        camera.PivotRotation = rotation;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetRotation", FontAwesomeIcon.Undo, 1f, "Reset", rotation != 0))
                        camera.PivotRotation = 0;

                    const string zoomText = "Zoom";
                    float zoom = camera.Zoom;
                    ImGui.SetNextItemWidth(width);
                    if(ImStarfallStudio.SliderFloat(zoomText, ref zoom, camera.StarfallStudioCamera->Camera.MaxDistance, camera.StarfallStudioCamera->Camera.MinDistance, "%.2f", ImGuiSliderFlags.AlwaysClamp))
                        camera.Zoom = zoom;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetZoom", FontAwesomeIcon.Undo, 1f, "Reset", zoom != 2.5))
                        camera.Zoom = 2.5f;

                    const string fovText = "FoV";
                    float fov = camera.FoV;
                    ImGui.SetNextItemWidth(width);
                    if(ImStarfallStudio.SliderAngle(fovText, ref fov, -44, 120, "%.2f", ImGuiSliderFlags.AlwaysClamp))
                        camera.FoV = fov;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetFoV", FontAwesomeIcon.Undo, 1f, "Reset", fov != 0))
                        camera.FoV = 0f;

                    ImGui.Separator();

                    const string panText = "Pan";
                    Vector2 pan = camera.Pan;
                    ImGui.SetNextItemWidth(width);
                    if(ImGui.DragFloat2(panText, ref pan, 0.001f))
                        camera.Pan = pan;

                    ImGui.SameLine();

                    if(ImStarfallStudio.FontIconButtonRight("resetPan", FontAwesomeIcon.Undo, 1f, "Reset", pan != Vector2.Zero))
                        camera.Pan = Vector2.Zero;

                    const string angleText = "Angle";
                    Vector2 angle = camera.Angle;
                    ImGui.SetNextItemWidth(width);
                    if(ImGui.DragFloat2(angleText, ref angle, 0.001f))
                        camera.Angle = angle;

                    ImGui.Separator();

                    var disable = camera.DisableCollision;
                    if(ImGui.Checkbox("Disable Collision", ref disable))
                        camera.DisableCollision = disable;

                    ImGui.SameLine();

                    var delimit = camera.DelimitCamera;
                    if(ImGui.Checkbox("Delimit Camera", ref delimit))
                        camera.DelimitCamera = delimit;
                }
            }
        }
    }

    //
    private static float MaxItemWidth => ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize("XXXXXXXXXXXXXXXXXX").X;
    private static float LabelStart => MaxItemWidth + ImGui.GetCursorPosX() + (ImGui.GetStyle().FramePadding.X * 2f);

    public unsafe static void DrawStarfallStudioCutscene(string id, StarfallStudioCameraCapability _cameraCapability, CutsceneManager _cutsceneManager, ConfigurationService _configService)
    {
        var cameraValues = _cameraCapability.CameraEntity.VirtualCamera.CutsceneCamValues;

        ImGui.Text("Camera Path ");

        ImGui.InputText(string.Empty, ref cameraValues.CameraPath, 0, ImGuiInputTextFlags.ReadOnly);

        ImGui.SameLine();

        if(ImGui.Button("Browse"))
        {
            UIManager.Instance.FileDialogManager.OpenFileDialog("Browse for XAT Camera File", "XAT Camera File {.xcp}",
                (success, path) =>
                {
                    if(success)
                    {
                        cameraValues.CameraPath = path[0];

                        string? folderPath = Path.GetDirectoryName(cameraValues.CameraPath);
                        if(folderPath is not null)
                        {
                            _configService.Configuration.LastXATPath = folderPath;
                            _configService.Save();

                            _cutsceneManager.CameraPath = new XATCameraFile(new BinaryReader(File.OpenRead(cameraValues.CameraPath)));
                        }
                    }
                    else
                    {
                        cameraValues.CameraPath = string.Empty;
                        _cutsceneManager.CameraPath = null;
                    }
                }, 1, _configService.Configuration.LastXATPath, false);
        }

        ImGui.Separator();

        using(ImRaii.Disabled(string.IsNullOrEmpty(cameraValues.CameraPath)))
        {
            ImGui.Checkbox("Enable FOV", ref _cutsceneManager.CameraSettings.EnableFOV);

            ImGui.Separator();

            ImGui.Text("Disabling FOV will make for a less accurate Camera, but might");
            ImGui.Text("provide for an easer way to support more character sizes without");
            ImGui.Text("changing the Camera's Scale & Offset!");

            ImGui.Separator();

            ImGui.InputFloat3("Camera Scale", ref _cutsceneManager.CameraSettings.Scale);
            ImGui.InputFloat3("Camera Offset", ref _cutsceneManager.CameraSettings.Offset);

            ImGui.Separator();

            ImGui.Checkbox("Loop", ref _cutsceneManager.CameraSettings.Loop);

            ImGui.Checkbox("Hide StarfallStudio On Play  (Press 'Shift + B' to Stop Cutscene)", ref _cutsceneManager.CloseWindowsOnPlay);

            ImGui.Checkbox("###delay_Start", ref _cutsceneManager.DelayStart);
            if(ImGui.IsItemHovered())
                ImGui.SetTooltip("Start Delay");

            ImGui.SameLine();
            ImGui.SetNextItemWidth(MaxItemWidth);

            using(ImRaii.Disabled(_cutsceneManager.DelayStart == false))
            {
                ImGui.InputInt($"###delay_Start_Chek", ref _cutsceneManager.DelayTime, 0, 0);
            }

            ImGui.SameLine();
            ImGui.SetCursorPosX(LabelStart);
            ImGui.Text("Start Delay");

            ImGui.Separator();

            ImGui.Checkbox("Start All Actors Animations On Play", ref _cutsceneManager.StartAllActorAnimationsOnPlay);

            using(ImRaii.Disabled(_cutsceneManager.StartAllActorAnimationsOnPlay == false))
            {
                ImGui.Checkbox("###animation_delay_Start", ref _cutsceneManager.DelayAnimationStart);
                if(ImGui.IsItemHovered())
                    ImGui.SetTooltip("Animation Start Delay");

                ImGui.SameLine();
                ImGui.SetNextItemWidth(MaxItemWidth);

                using(ImRaii.Disabled(_cutsceneManager.DelayAnimationStart == false))
                {
                    ImGui.InputInt($"###animation_delay_Start_Chek", ref _cutsceneManager.DelayAnimationTime, 0, 0);
                }

                ImGui.SameLine();
                ImGui.SetCursorPosX(LabelStart);
                ImGui.Text("Animation Delay");
            }

            ImGui.Separator();

            ImGui.Text("The time-scale for the delay functions are in Milliseconds!");
            ImGui.Text("1000 Milliseconds = 1 Second");

            ImGui.Separator();

            var isrunning = _cutsceneManager.IsRunning;
            using(ImRaii.Disabled(isrunning))
            {
                if(ImGui.Button("Play"))
                {
                    _cutsceneManager.StartPlayback();
                }
            }

            ImGui.SameLine();

            using(ImRaii.Disabled(!isrunning))
            {
                if(ImGui.Button("Stop"))
                {
                    _cutsceneManager.StopPlayback();
                }
            }
        }
    }
}
