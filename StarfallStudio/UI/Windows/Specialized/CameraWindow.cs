using StarfallStudio.Capabilities.Camera;
using StarfallStudio.Config;
using StarfallStudio.Entities;
using StarfallStudio.Entities.Camera;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.Cutscene;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using System;
using System.Linq;

namespace StarfallStudio.UI.Windows.Specialized;

public class CameraWindow : Window, IDisposable
{
    private readonly EntityManager _entityManager;
    private readonly GPoseService _gPoseService;
    private readonly VirtualCameraManager _virtualCameraService;
    private readonly CutsceneManager _cutsceneManager;
    private readonly ConfigurationService _configService;

    public CameraWindow(EntityManager entityManager, GPoseService gPoseService, CutsceneManager cutsceneManager, ConfigurationService configService, VirtualCameraManager virtualCameraService) : base($"★ {StarfallStudio.Name} - Camera###starfall_camera_window")
    {
        Namespace = "starfall_camera_namespace";

        _entityManager = entityManager;
        _gPoseService = gPoseService;
        _virtualCameraService = virtualCameraService;
        _cutsceneManager = cutsceneManager;
        _configService = configService;

        WindowSizeConstraints constraints = new()
        {
            MinimumSize = new(250, 300),
            MaximumSize = new(355, 400)
        };
        this.SizeConstraints = constraints;

        _gPoseService.OnGPoseStateChange += OnGPoseStateChange;
    }

    public override void Draw()
    {
        ImStarfallStudio.VerticalPadding(2);

        // Camera selector row: [dropdown] [+ new] [freecam toggle] - matches Ktisis workflow
        var avail = ImGui.GetContentRegionAvail().X;
        var style = ImGui.GetStyle();
        var iconBtnWidth = ImGui.GetFrameHeight() + style.FramePadding.X * 2;
        var comboWidth = avail - (iconBtnWidth + style.ItemSpacing.X) * 2;

        ImGui.SetNextItemWidth(comboWidth);
        if(ImGui.BeginCombo("###setCamera"u8, $"{_virtualCameraService.SelectedCameraEntity?.FriendlyName}"))
        {
            var list = _virtualCameraService.SpawnedCameraEntities;
            list.Add(_virtualCameraService.GetDefaultCamera()!);
            foreach(var value in list)
            {
                if(ImGui.Selectable($"Camera: [ {value.FriendlyName} ] [ {value.CameraType.ToString().ToUpper()} ]"))
                    _virtualCameraService.SelectedCameraEntity = value;
            }
            ImGui.EndCombo();
        }
        ImStarfallStudio.AttachToolTip("Current Camera");

        ImGui.SameLine();
        if(ImStarfallStudio.FontIconButton("newgamecam", Dalamud.Interface.FontAwesomeIcon.Plus, "Create new camera"))
            _virtualCameraService.CreateCamera(CameraType.Game);

        ImGui.SameLine();
        var isFreecam = _virtualCameraService.SelectedCameraEntity?.CameraType == CameraType.Free;
        if(ImStarfallStudio.FontIconButton("freecamtoggle", Dalamud.Interface.FontAwesomeIcon.Camera,
            isFreecam ? "Exit free camera" : "Enter free camera"))
        {
            if(isFreecam)
            {
                var freecamId = _virtualCameraService.SelectedCameraEntity!.CameraID;
                var def = _virtualCameraService.GetDefaultCamera();
                if(def is not null)
                {
                    _virtualCameraService.SelectedCameraEntity = def;
                    _virtualCameraService.SelectCamera(def.VirtualCamera);
                }
                _virtualCameraService.DestroyCamera(freecamId);
            }
            else
            {
                _virtualCameraService.CreateCamera(CameraType.Free);
            }
        }

        ImGui.Separator();

        if(_virtualCameraService.SelectedCameraEntity is null || _virtualCameraService.SelectedCameraEntity.IsAttached == false)
        {
            _virtualCameraService.SelectedCameraEntity = _virtualCameraService.CamerasCount > 0
                ? _virtualCameraService.SpawnedCameraEntities.First()
                : null;
        }

        //
        // Hedder

        if(_virtualCameraService.SelectedCameraEntity is null)
        {
            _virtualCameraService.SelectedCameraEntity = _virtualCameraService.GetDefaultCamera();
        }

        if(!_virtualCameraService!.SelectedCameraEntity!.TryGetCapability<StarfallStudioCameraCapability>(out var camStarfallStudioCap))
        {
            return;
        }

        //
        // Body

        switch(camStarfallStudioCap.CameraEntity.CameraType)
        {
            case CameraType.Free:
                WindowName = $"★ {StarfallStudio.Name} - Camera (Free)###starfall_camera_window";
                CameraEditor.DrawFreeCam("camera_widget_editor", camStarfallStudioCap);
                break;
            case CameraType.Cutscene:
                WindowName = $"★ {StarfallStudio.Name} - Camera (Cutscene)###starfall_camera_window";
                CameraEditor.DrawStarfallStudioCutscene("camera_widget_editor", camStarfallStudioCap, _cutsceneManager, _configService);
                break;
            case CameraType.Game:
            case CameraType.Default:
                WindowName = $"★ {StarfallStudio.Name} - Camera (Game)###starfall_camera_window";
                CameraEditor.DrawStarfallStudioCam("camera_widget_editor", camStarfallStudioCap);
                break;
        }


    }

    private void OnGPoseStateChange(bool newState)
    {
        if(!newState)
            IsOpen = false;
    }

    public void Dispose()
    {
        _gPoseService.OnGPoseStateChange -= OnGPoseStateChange;
    }
}
