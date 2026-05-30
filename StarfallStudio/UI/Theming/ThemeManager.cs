using System.Numerics;

namespace StarfallStudio.UI.Theming;

public static class ThemeManager
{

    public static Theme CurrentTheme { get; set; }

    static ThemeManager()
    {
        CurrentTheme = new Theme
        {
            Name = "Midnight Starfall",
            Accent = new ThemeAccent
            {
                AccentColor        = SetColor(new Vector4(115, 55, 160, 255)),  // amethyst
                AccentColorLight   = SetColor(new Vector4(145, 80, 195, 255)),
                AccentColorStrong  = SetColor(new Vector4(185, 142, 48, 255)), // gold
                AccentColorDim     = SetColor(new Vector4(115, 55, 160, 140)),

                AccentCheckMark     = SetColor(new Vector4(185, 142, 48, 255)), // gold checkmark
                AccentButtonHovered = SetColor(new Vector4(92, 42, 130, 255)),

                AccentTabActive          = SetColor(new Vector4(115, 55, 160, 255)),
                AccentTabUnfocusedActive = SetColor(new Vector4(85, 50, 120, 255)),
            },
            Core = new ThemeCore
            {

            }
        };
    }

    static uint SetColor(Vector4 colorVector)
    {
        uint r = (uint)(colorVector.X) & 0xFF;
        uint g = (uint)(colorVector.Y) & 0xFF;
        uint b = (uint)(colorVector.Z) & 0xFF;
        uint a = (uint)(colorVector.W) & 0xFF;

        return (a << 24) | (b << 16) | (g << 8) | r;
    }
}

public record class Theme
{
    public required string Name;

    public required ThemeAccent Accent;

    public required ThemeCore Core;
}

public record class ThemeAccent
{
    public uint AccentColor = 0;
    public uint AccentColorLight;
    public uint AccentColorStrong;
    public uint AccentColorDim;


    public uint AccentCheckMark;
    public uint AccentButtonHovered;

    public uint AccentTabActive;
    public uint AccentTabUnfocusedActive;
}

public record class ThemeCore
{
    public uint Text;
}
