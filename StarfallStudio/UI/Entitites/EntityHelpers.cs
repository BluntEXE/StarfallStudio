using StarfallStudio.Entities.Core;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace StarfallStudio.UI.Entitites;

public static class EntityHelpers
{
    public static void DrawEntitySection(Entity? entity)
    {
        if(entity is not null && entity.IsAttached)
        {
            var capabilities = entity.Capabilities;

            if(entity.IsLoading)
            {
                DrawSpinner();
            }

            using(ImRaii.Disabled(entity.IsLoading))
            {
                using(ImRaii.PushId($"quickicons_{entity.Id}"))
                {
                    WidgetHelpers.DrawQuickIcons(capabilities);
                }

                using(ImRaii.PushId($"bodies_{entity.Id}"))
                {
                    WidgetHelpers.DrawBodies(capabilities);
                }
            }
        }
    }

    private static float spinnerAngle = 0;
    public static void DrawSpinner()
    {
        var cursor = ImGui.GetCursorPos();
        ImGui.SetCursorPosX((ImGui.GetWindowWidth() / 2) - 24);
        ImGui.SetCursorPosY((ImGui.GetWindowHeight() / 2) - 24);
        ImStarfallStudio.Spinner(ref spinnerAngle);
        ImGui.SetCursorPos(cursor);
    }
}
