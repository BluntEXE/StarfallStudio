using StarfallStudio.Game.Types;
using Dalamud.Bindings.ImGui;
using System.Numerics;

namespace StarfallStudio.UI.Controls.Stateless;

public static partial class ImStarfallStudio
{
    public static bool BorderedWeatherGameIcon(string id, WeatherUnion union, bool showText = true, ImGuiButtonFlags flags = ImGuiButtonFlags.MouseButtonLeft, Vector2? size = null)
    {
        var (description, icon) = union.Match(
           weather => ($"{weather.Name}\n{weather.RowId}\nType: {weather.Description}", (uint)weather.Icon),
           none => ("None", (byte)0)
        );

        if(!showText)
        {
            description = string.Empty;
        }

        return BorderedGameIcon(id, icon, "Images.Weather.png", description, flags, size);
    }
}
