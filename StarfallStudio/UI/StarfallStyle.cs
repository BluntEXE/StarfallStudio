using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using System.Numerics;

namespace StarfallStudio.UI;

// Midnight Starfall - gothic dark with amethyst + antique gold
// Deep purples = gothic darkness | Gold accents = starlight/celestial
public static class StarfallStudioStyle
{
    public static bool EnableStyle { get; set; }
    public static bool EnableColor => Config.ConfigurationService.Instance.Configuration.Appearance.EnableStarfallStudioColor;

    static bool _hasPushed = false;
    static bool _hasPushedColor = false;

    // Amethyst accent
    static readonly Vector4 Amethyst       = new(115, 55, 160, 255);
    static readonly Vector4 AmethystHover  = new(92, 42, 130, 255);
    static readonly Vector4 AmethystActive = new(75, 34, 106, 255);
    static readonly Vector4 AmethystDim    = new(115, 55, 160, 140);

    // Star gold accent
    static readonly Vector4 Gold           = new(185, 142, 48, 255);
    static readonly Vector4 GoldDim        = new(155, 118, 38, 200);

    // Dark purple-tinted backgrounds (night sky)
    static readonly Vector4 BgWindow  = new(22, 17, 28, 248);
    static readonly Vector4 BgChild   = new(22, 17, 28, 60);
    static readonly Vector4 BgPopup   = new(28, 21, 36, 252);
    static readonly Vector4 BgFrame   = new(34, 25, 44, 255);
    static readonly Vector4 BgFrameH  = new(52, 38, 65, 255);
    static readonly Vector4 BgFrameA  = new(40, 29, 52, 255);

    // Title bars - deep gothic purple
    static readonly Vector4 TitleBg   = new(28, 17, 38, 232);
    static readonly Vector4 TitleBgA  = new(45, 24, 62, 255);
    static readonly Vector4 TitleBgC  = new(24, 14, 33, 255);

    // Borders - purple-tinted
    static readonly Vector4 Border     = new(58, 40, 72, 255);
    static readonly Vector4 BorderShad = new(0,  0,  0, 100);

    // Scrollbar
    static readonly Vector4 ScrollBg   = new(0, 0, 0, 0);
    static readonly Vector4 ScrollG    = new(72, 46, 92, 255);
    static readonly Vector4 ScrollGH   = new(92, 58, 118, 255);
    static readonly Vector4 ScrollGA   = new(108, 68, 138, 255);

    // Headers (collapsing sections)
    static readonly Vector4 Header  = new(45, 30, 60, 65);
    static readonly Vector4 HeaderH = new(68, 44, 90, 85);
    static readonly Vector4 HeaderA = new(88, 55, 115, 110);

    // Separators - subtle purple, glows on hover
    static readonly Vector4 Sep  = new(68, 46, 85, 105);
    static readonly Vector4 SepH = new(95, 60, 128, 200);

    // Tabs
    static readonly Vector4 Tab    = new(36, 24, 48, 255);
    static readonly Vector4 TabH   = new(72, 46, 102, 255);
    static readonly Vector4 TabUF  = new(33, 22, 44, 255);
    static readonly Vector4 TabUFA = new(85, 50, 120, 255);

    public static void PushStyle()
    {
        if(EnableStyle == false)
            return;

        _hasPushed = true;

        if(EnableColor)
        {
            _hasPushedColor = true;

            PushStyleColor(ImGuiCol.Text,         new(255, 255, 255, 255));
            PushStyleColor(ImGuiCol.TextDisabled,  new(140, 118, 158, 255));

            PushStyleColor(ImGuiCol.WindowBg,  BgWindow);
            PushStyleColor(ImGuiCol.ChildBg,   BgChild);
            PushStyleColor(ImGuiCol.PopupBg,   BgPopup);

            PushStyleColor(ImGuiCol.Border,       Border);
            PushStyleColor(ImGuiCol.BorderShadow, BorderShad);

            PushStyleColor(ImGuiCol.FrameBg,        BgFrame);
            PushStyleColor(ImGuiCol.FrameBgHovered, BgFrameH);
            PushStyleColor(ImGuiCol.FrameBgActive,  BgFrameA);

            PushStyleColor(ImGuiCol.TitleBg,          TitleBg);
            PushStyleColor(ImGuiCol.TitleBgActive,    TitleBgA);
            PushStyleColor(ImGuiCol.TitleBgCollapsed, TitleBgC);

            PushStyleColor(ImGuiCol.MenuBarBg,         new(32, 22, 42, 255));
            PushStyleColor(ImGuiCol.ScrollbarBg,       ScrollBg);
            PushStyleColor(ImGuiCol.ScrollbarGrab,        ScrollG);
            PushStyleColor(ImGuiCol.ScrollbarGrabHovered, ScrollGH);
            PushStyleColor(ImGuiCol.ScrollbarGrabActive,  ScrollGA);

            // Gold checkmark = starlight on dark
            PushStyleColor(ImGuiCol.CheckMark, Gold);

            // Amethyst slider, gold when active
            PushStyleColor(ImGuiCol.SliderGrab,       new(108, 68, 145, 255));
            PushStyleColor(ImGuiCol.SliderGrabActive, GoldDim);

            PushStyleColor(ImGuiCol.Button,        new(255, 255, 255, 18));
            PushStyleColor(ImGuiCol.ButtonHovered, AmethystHover);
            PushStyleColor(ImGuiCol.ButtonActive,  AmethystActive);

            PushStyleColor(ImGuiCol.Header,        Header);
            PushStyleColor(ImGuiCol.HeaderHovered, HeaderH);
            PushStyleColor(ImGuiCol.HeaderActive,  HeaderA);

            PushStyleColor(ImGuiCol.Separator,        Sep);
            PushStyleColor(ImGuiCol.SeparatorHovered, SepH);
            PushStyleColor(ImGuiCol.SeparatorActive,  Amethyst);

            PushStyleColor(ImGuiCol.ResizeGrip,        new(0, 0, 0, 0));
            PushStyleColor(ImGuiCol.ResizeGripHovered, new(0, 0, 0, 0));
            PushStyleColor(ImGuiCol.ResizeGripActive,  Amethyst);

            PushStyleColor(ImGuiCol.Tab,                Tab);
            PushStyleColor(ImGuiCol.TabHovered,         TabH);
            PushStyleColor(ImGuiCol.TabActive,          Amethyst);
            PushStyleColor(ImGuiCol.TabUnfocused,       TabUF);
            PushStyleColor(ImGuiCol.TabUnfocusedActive, TabUFA);

            PushStyleColor(ImGuiCol.DockingPreview,  AmethystDim);
            PushStyleColor(ImGuiCol.DockingEmptyBg,  new(28, 20, 36, 255));

            PushStyleColor(ImGuiCol.PlotLines, new(155, 120, 185, 255));

            PushStyleColor(ImGuiCol.TableHeaderBg,     new(42, 26, 55, 255));
            PushStyleColor(ImGuiCol.TableBorderStrong, new(72, 46, 88, 255));
            PushStyleColor(ImGuiCol.TableBorderLight,  new(55, 34, 68, 255));
            PushStyleColor(ImGuiCol.TableRowBg,    new(0, 0, 0, 0));
            PushStyleColor(ImGuiCol.TableRowBgAlt, new(255, 255, 255, 10));

            // Gold selection = star highlight
            PushStyleColor(ImGuiCol.TextSelectedBg, AmethystDim);
            PushStyleColor(ImGuiCol.DragDropTarget, Gold);

            PushStyleColor(ImGuiCol.NavHighlight,          AmethystDim);
            PushStyleColor(ImGuiCol.NavWindowingDimBg,     new(204, 204, 204, 51));
            PushStyleColor(ImGuiCol.NavWindowingHighlight, new(204, 204, 204, 89));
        }

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding,     new Vector2(6, 6));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding,      new Vector2(4, 3));
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding,       new Vector2(4, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing,       new Vector2(4, 4));
        ImGui.PushStyleVar(ImGuiStyleVar.ItemInnerSpacing,  new Vector2(4, 4));

        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 21.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize,  9.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabMinSize,   20.0f);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize,  1f);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize,  1f);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize,  0.5f);  // subtle frame definition

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding,   8f);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding,    4f * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding,    4f);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding,    5f);
        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarRounding, 4f);
        ImGui.PushStyleVar(ImGuiStyleVar.GrabRounding,      4f * ImGuiHelpers.GlobalScale);
        ImGui.PushStyleVar(ImGuiStyleVar.TabRounding,       4f);
    }

    static void PushStyleColor(ImGuiCol imGuiCol, Vector4 colorVector)
    {
        uint r = (uint)(colorVector.X) & 0xFF;
        uint g = (uint)(colorVector.Y) & 0xFF;
        uint b = (uint)(colorVector.Z) & 0xFF;
        uint a = (uint)(colorVector.W) & 0xFF;

        ImGui.PushStyleColor(imGuiCol, (a << 24) | (b << 16) | (g << 8) | r);
    }

    public static void PopStyle()
    {
        if(_hasPushed)
        {
            _hasPushed = false;

            ImGui.PopStyleVar(19);

            if(_hasPushedColor)
            {
                _hasPushedColor = false;
                ImGui.PopStyleColor(51);
            }
        }
    }
}
