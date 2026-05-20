using StarfallStudio.Capabilities.Camera;
using StarfallStudio.Entities.Core;
using StarfallStudio.Game.Camera;
using StarfallStudio.Game.Input;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Theming;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StarfallStudio.Entities.Camera;

public class CameraContainerEntity(IServiceProvider provider) : Entity("cameras", provider)
{
    private readonly VirtualCameraManager _virtualCameraManager = provider.GetRequiredService<VirtualCameraManager>();
    private readonly GameInputService _gameInputService = provider.GetRequiredService<GameInputService>();

    public override string FriendlyName => "Cameras";

    public override FontAwesomeIcon Icon => FontAwesomeIcon.Camera;

    public override int ContextButtonCount => 2;

    public override EntityFlags Flags => EntityFlags.DefaultOpen | EntityFlags.HasContextButton;

    public override void DrawContextButton()
    {
        using(ImRaii.PushColor(ImGuiCol.Button, ThemeManager.CurrentTheme.Accent.AccentColor))
        {
            var lockIcon = IsLocked ? FontAwesomeIcon.Lock : FontAwesomeIcon.Unlock;
            var lockToolTip = IsLocked ? "Unlock Cameras" : "Lock Cameras";
            if(ImStarfallStudio.FontIconButtonRight($"###{Id}_cameras_lock", lockIcon, 2f, lockToolTip, bordered: false))
            {
                IsLocked = !IsLocked;
            }

            ImGui.SameLine();

            string toolTip = $"New Camera";
            if(ImStarfallStudio.FontIconButtonRight($"###{Id}_cameras_contextButton", FontAwesomeIcon.Plus, 1f, toolTip, bordered: false))
            {
                ImGui.OpenPopup("DrawSpawnMenuPopup");
            }
            CameraEditor.DrawSpawnMenu(_virtualCameraManager);
        }
    }

    public override void OnAttached()
    {
        AddCapability(ActivatorUtilities.CreateInstance<CameraContainerCapability>(_serviceProvider, this));
    }

    public override void OnSelected()
    {
        _gameInputService.AllowEscape = true;
        base.OnSelected();
    }

    public override void OnChildAttached() => SortChildren();
    public override void OnChildDetached() => SortChildren();

    private void SortChildren() =>
        _children.Sort(static (a, b) => string.Compare(a.Id.Unique, b.Id.Unique, StringComparison.Ordinal));

}
