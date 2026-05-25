using StarfallStudio.Config;
using StarfallStudio.Files;
using StarfallStudio.Resources;
using StarfallStudio.UI.Controls.Stateless;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace StarfallStudio.UI.Windows;

public class UpdateWindow : Window
{
    //
    // Some code found here is inspired by CharacterSelect+

    private bool _scrollToTop = false;
    private float _closeButtonWidth => 310f * ImGuiHelpers.GlobalScale;

    private readonly List<string> _supporters = [];
    private readonly List<string> _contributors = [];
    private readonly ChangelogFile _changelogFile;

    public UpdateWindow() : base($"★ {StarfallStudio.Name} - Changelog [{ConfigurationService.Instance.Version}]###brio_welcomewindow", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking)
    {
        Namespace = "brio_welcomewindow_namespace";

        Size = new Vector2(710, 745);

        ShowCloseButton = true;
        AllowClickthrough = false;
        AllowPinning = false;

        //

        string? line;

        var kofiStream = ResourceProvider.Instance.GetRawResourceStream("Changelog.kofi.txt");
        var patreonStream = ResourceProvider.Instance.GetRawResourceStream("Changelog.patreon.txt");
        var contributorsStream = ResourceProvider.Instance.GetRawResourceStream("Changelog.contributors.txt");

        using var streamReader = new StreamReader(kofiStream, Encoding.UTF8, true, 128);
        while((line = streamReader.ReadLine()) is not null)
            _supporters.Add(line);

        using var streamReader2 = new StreamReader(patreonStream, Encoding.UTF8, true, 128);
        while((line = streamReader2.ReadLine()) is not null)
            _supporters.Add(line);

        using var streamReader3 = new StreamReader(contributorsStream, Encoding.UTF8, true, 128);
        while((line = streamReader3.ReadLine()) is not null)
            _contributors.Add(line);

        //

        var changelogStream = ResourceProvider.Instance.GetRawResourceStream("Changelog.changelog.yaml");
        using var streamReader4 = new StreamReader(changelogStream, Encoding.UTF8, true, 128);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var yaml = streamReader4.ReadToEnd();
        _changelogFile = deserializer.Deserialize<ChangelogFile>(yaml);
    }

    public override void OnOpen()
    {
        _scrollToTop = true;
    }
    public override void PreDraw()
    {
        ImGui.SetNextWindowPos(new Vector2((ImGui.GetIO().DisplaySize.X - Size!.Value.X) / 2, (ImGui.GetIO().DisplaySize.Y - Size!.Value.Y) / 2), ImGuiCond.Appearing);

        base.PreDraw();
    }

    //

    int selected = 0;
    public override void Draw()
    {
        var windowPos = ImGui.GetWindowPos();
        var windowPadding = ImGui.GetStyle().WindowPadding;

        var headerWidth = 1000f - (windowPadding.X * 2 * ImGuiHelpers.GlobalScale);
        var headerHeight = 500f * ImGuiHelpers.GlobalScale;

        var headerStart = windowPos + new Vector2(1, 25);

        // Image

        var image = ResourceProvider.Instance.GetResourceImage($"Images.StarfallStudioIcon.png");

        // Cap height so the image doesn't consume the whole window
        var imageAspect = (float)image.Width / image.Height;
        var scaledHeight = 120f * ImGuiHelpers.GlobalScale;
        var scaledWidth = scaledHeight * imageAspect;

        // Center horizontally
        var windowWidth = ImGui.GetWindowSize().X;
        var imagePos = new Vector2(windowPos.X + (windowWidth - scaledWidth) / 2f, headerStart.Y + 4);

        var drawList = ImGui.GetWindowDrawList();
        drawList.AddImage(image.Handle, imagePos, imagePos + new Vector2(scaledWidth, scaledHeight));

        headerStart = new Vector2(headerStart.X, headerStart.Y + scaledHeight + 8);
        var headerEnd = headerStart + new Vector2(headerWidth, headerHeight);

        // Background
        DrawBackground(headerStart, headerEnd);

        // Cursor line up
        ImGui.SetCursorScreenPos(headerStart);

        // Keep this for correct cursor line up
        ImGui.Separator();

        // Buttons
        var segmentSize = ImGui.GetWindowSize().X / 3.15f;
        var buttonSize = new Vector2(segmentSize, ImGui.GetTextLineHeight() * 1.7f);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

        using(ImRaii.PushColor(ImGuiCol.Button, new Vector4(0, 224, 148, 200) / 255))
            if(ImGui.Button("Support on Ko-fi", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://ko-fi.com/ehnocure", UseShellExecute = true });
        ImGui.SameLine();

        using(ImRaii.PushColor(ImGuiCol.Button, new Vector4(29, 161, 242, 200) / 255))
            if(ImGui.Button("View on GitHub", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://github.com/BluntEXE/StarfallStudio", UseShellExecute = true });
        ImGui.SameLine();

        using(ImRaii.PushColor(ImGuiCol.Button, new Vector4(200, 80, 80, 200) / 255))
            if(ImGui.Button("Submit Feedback", buttonSize))
                Process.Start(new ProcessStartInfo { FileName = "https://github.com/BluntEXE/StarfallStudio/issues/new", UseShellExecute = true });

        // Tagline Test
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
        ImGui.TextColored(new Vector4(0.4f, 0.9f, 0.4f, 1.0f), _changelogFile.Tagline);
        ImGui.SameLine();
        ImGui.TextColored(new Vector4(0.75f, 0.75f, 0.85f, 1.0f), $"  -  {_changelogFile.Subline}");
        ImStarfallStudio.VerticalPadding(10);

        // Selector
        ImStarfallStudio.ButtonSelectorStrip("brio_changelog_selector", new Vector2(ImStarfallStudio.GetRemainingWidth(), ImStarfallStudio.GetLineHeight()), ref selected, [" Changelog ", " About "]);

        if(selected == 0)
        {
            using(ImRaii.PushColor(ImGuiCol.ChildBg, 0))
            using(var c = ImRaii.Child("###brio_changelog", new Vector2(ImGui.GetWindowHeight() - 55 * ImGuiHelpers.GlobalScale, ImStarfallStudio.GetRemainingHeight() - 44), false,
                Flags = ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
                if(c.Success)
                {
                    if(_scrollToTop)
                    {
                        _scrollToTop = false;
                        ImGui.SetScrollHereY(0);
                    }

                    foreach(var entry in _changelogFile.Changelog)
                        DrawChangelogTemplate(entry);

                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Spacing();
                    ImGui.Spacing();

                    ImStarfallStudio.VerticalPadding(15);
                }
        }
        else
        {
            using(ImRaii.PushColor(ImGuiCol.ChildBg, 0))
            using(var c = ImRaii.Child("###brio_about", new Vector2(ImGui.GetWindowHeight() - 55 * ImGuiHelpers.GlobalScale, ImStarfallStudio.GetRemainingHeight() - 44), false,
                Flags = ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
                if(c.Success)
                    DrawAbout();
        }

        ImGui.SetCursorPosX((ImGui.GetWindowSize().X - _closeButtonWidth) / 2);
        if(ImStarfallStudio.Button("Close", FontAwesomeIcon.SquareXmark, new Vector2(_closeButtonWidth, 0), centerTest: true, tooltip: "To open this window again click the `Information` button on the StarfallStudio Scene Manager!"))
        {
            IsOpen = false;
        }
    }

    private void DrawChangelogTemplate(ChangelogEntry entry)
    {
        var currentColor = entry.IsCurrent == true ? new Vector4(0.5f, 0.9f, 0.5f, 1.0f) : new Vector4(0.75f, 0.75f, 0.85f, 1.0f);
        bool isCurrent = entry.IsCurrent ?? false;

        // Dev Message
        if(entry.Message.IsNullOrEmpty() is false)
        {
            if(CollapsingHeader($" {entry.Name} –- {entry.Date} ", $" {entry.Tagline} ", currentColor, isCurrent))
            {
                ImStarfallStudio.VerticalPadding(10);

                ImGui.Text(entry.Message);

                ImStarfallStudio.VerticalPadding(10);
            }
            return;
        }

        if(CollapsingHeader($" {entry.Name} –- {entry.Date} ", $"  –-  {entry.Tagline} ", currentColor, isCurrent))
        {
            ImStarfallStudio.VerticalPadding(10);

            foreach(var item in entry.Versions)
            {
                DrawFeature(FontAwesomeIcon.None, item.Number, new Vector4(0.5f, 0.9f, 0.5f, 1.0f));

                foreach(var subItem in item.Items)
                {
                    ImGui.BulletText(subItem);
                }
            }

            ImStarfallStudio.VerticalPadding(10);
        }
    }

    public void DrawAbout()
    {
        ImStarfallStudio.VerticalPadding(8);

        ImGui.TextColored(new Vector4(0.85f, 0.85f, 1.0f, 1.0f), "Starfall Studio");
        ImGui.SameLine();
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.7f, 1.0f), $"v{ConfigurationService.Instance.Version}  -  by Ehno");

        ImStarfallStudio.VerticalPadding(6);

        ImGui.TextWrapped("A combined GPose toolkit built on the foundations of Brio and Ktisis.");

        ImStarfallStudio.VerticalPadding(10);
        ImGui.Separator();
        ImStarfallStudio.VerticalPadding(10);

        ImGui.TextColored(new Vector4(0.7f, 0.85f, 1.0f, 1.0f), "Built on:");
        ImStarfallStudio.VerticalPadding(4);
        ImGui.BulletText("Brio by Minmoose (AsgardXIV)");
        ImGui.BulletText("Ktisis by Ashadow700");

        ImStarfallStudio.VerticalPadding(10);
        ImGui.Separator();
        ImStarfallStudio.VerticalPadding(10);

        var colWidth = ImGui.GetContentRegionAvail().X / 2f - ImGui.GetStyle().ItemSpacing.X;
        var colHeight = ImStarfallStudio.GetRemainingHeight();

        using(var left = ImRaii.Child("###about_brio", new Vector2(colWidth, colHeight)))
        {
            if(left.Success)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.85f, 1.0f, 1.0f), "Original Brio Contributors:");
                ImStarfallStudio.VerticalPadding(4);
                foreach(var item in _contributors)
                    ImGui.BulletText(item);
            }
        }

        ImGui.SameLine();

        using(var right = ImRaii.Child("###about_ktisis", new Vector2(colWidth, colHeight)))
        {
            if(right.Success)
            {
                ImGui.TextColored(new Vector4(0.7f, 0.85f, 1.0f, 1.0f), "Original Ktisis Contributors:");
                ImStarfallStudio.VerticalPadding(4);
                ImGui.BulletText("Ashadow700");
                ImGui.BulletText("BobTheBob9");
                ImGui.BulletText("Fayti1703");
                ImGui.BulletText("perchbirdd");
                ImGui.BulletText("Emyka");
                ImGui.BulletText("sudojunior");
                ImGui.BulletText("hackmud-dtr");
                ImGui.BulletText("Caraxi");
            }
        }
    }

    //
    // some code found here is modified and from CharacterSelect+
    // https://github.com/IcarusXIV/Character-Select- (link is includes the -)
    //

    private static void DrawBackground(Vector2 headerStart, Vector2 headerEnd)
    {
        var drawList = ImGui.GetWindowDrawList();
        uint gradientTop = ImGui.GetColorU32(new Vector4(0.2f, 0.4f, 0.8f, 0.15f));
        uint gradientBottom = ImGui.GetColorU32(new Vector4(0.1f, 0.1f, 0.2f, 0.05f));
        drawList.AddRectFilledMultiColor(headerStart, headerEnd * ImGuiHelpers.GlobalScale, gradientTop, gradientTop, gradientBottom, gradientBottom);
    }

    private static bool CollapsingHeader(string title, string subTitle, Vector4 titleColor, bool defaultOpen)
    {
        var flags = defaultOpen ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None;

        bool isOpen = false;
        using(ImRaii.PushColor(ImGuiCol.Text, titleColor))
        {
            isOpen = ImGui.CollapsingHeader(title, flags);
        }

        ImGui.SameLine();

        ImGui.TextColored(new Vector4(0.75f, 0.75f, 0.85f, 1.0f), subTitle);

        return isOpen;
    }

    private static void DrawFeature(FontAwesomeIcon icon, string title, Vector4 accentColor)
    {
        var drawList = ImGui.GetWindowDrawList();
        var startPos = ImGui.GetCursorScreenPos();

        // Feature section background
        var backgroundMin = startPos + new Vector2(-10, -5);
        var backgroundMax = startPos + new Vector2(ImGui.GetContentRegionAvail().X + 10, 25);
        drawList.AddRectFilled(backgroundMin, backgroundMax, ImGui.GetColorU32(new Vector4(0.12f, 0.12f, 0.15f, 0.6f)), 4f);
        drawList.AddRectFilled(backgroundMin, backgroundMin + new Vector2(3, backgroundMax.Y - backgroundMin.Y), ImGui.GetColorU32(accentColor), 2f);

        ImGui.Spacing();
        ImStarfallStudio.Icon(icon);
        ImGui.SameLine();
        ImGui.TextColored(accentColor, title);
        ImGui.Spacing();
    }
}
