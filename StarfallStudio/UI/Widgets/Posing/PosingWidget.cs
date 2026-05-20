using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Input;
using StarfallStudio.UI.Controls.Core;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using System.Numerics;

namespace StarfallStudio.UI.Widgets.Posing;

public class PosingWidget(PosingCapability capability) : Widget<PosingCapability>(capability)
{
    public override string HeaderName => "Posing";

    public override WidgetFlags Flags => Capability.Actor.IsProp
        ? (WidgetFlags.DefaultOpen | WidgetFlags.DrawBody)
        : (WidgetFlags.DrawBody | WidgetFlags.HasAdvanced | WidgetFlags.DefaultOpen);

    private readonly PosingTransformEditor _posingTransformEditor = new();
    private readonly BoneSearchControl _boneSearchEditor = new();

    public override void DrawBody()
    {
        DrawButtons();

        // Transform table - always visible, like Ktisis
        using var child = ImRaii.Child("###posing_transform_child", new Vector2(0, 165 * ImGuiHelpers.GlobalScale), true,
            ImGuiWindowFlags.AlwaysAutoResize);
        if(child.Success)
        {
            PosingEditorCommon.DrawSelectionName(Capability);
            _posingTransformEditor.Draw("posing_widget_transform", Capability, true);
        }

        // Bone Categories - collapsing, matches Ktisis
        if(!Capability.Actor.IsProp && ImGui.CollapsingHeader("Bone Categories"))
        {
            PosingEditorCommon.DrawBoneFilterEditor(Capability.PosingService.OverlayFilter, Capability.PosingService);
        }

        // Bone List - collapsing tree, Ktisis-style
        if(!Capability.Actor.IsProp && ImGui.CollapsingHeader("Bone List"))
        {
            BoneTreeControl.Draw("posing_widget_bonetree", Capability);
        }
    }

    private void DrawButtons()
    {
        if(Capability.Actor.TryGetCapability<ActionTimelineCapability>(out var timelineCapability) == false)
            return;

        var btnSize = new Vector2(ImGui.GetFrameHeight());

        // Overlay toggle (show/hide skeleton) - matches Ktisis show skeleton button
        var overlayOpen = Capability.OverlayOpen;
        if(ImStarfallStudio.FontIconButton("overlay",
            overlayOpen ? FontAwesomeIcon.EyeSlash : FontAwesomeIcon.Eye,
            overlayOpen ? "Hide Overlay" : "Show Overlay"))
            Capability.OverlayOpen = !overlayOpen;

        ImGui.SameLine();

        if(Capability.Actor.IsProp == false)
        {
            // Sibling link / mirror mode - matches Ktisis sibling link
            PosingEditorCommon.DrawMirrorModeSelect(Capability, btnSize);

            ImGui.SameLine();

            // Import pose
            if(ImStarfallStudio.FontIconButton("import", FontAwesomeIcon.FileDownload, "Import Pose"))
                ImGui.OpenPopup("DrawImportPoseMenuPopup");

            FileUIHelpers.DrawImportPoseMenuPopup("postingWidget", Capability);

            ImGui.SameLine();

            // Export pose
            if(ImStarfallStudio.FontIconButton("export", FontAwesomeIcon.Save, "Save Pose"))
                FileUIHelpers.ShowExportPoseModal(Capability);

            ImGui.SameLine();

            // Bone search
            if(ImStarfallStudio.FontIconButton("bone_search", FontAwesomeIcon.Search, "Bone Search"))
                ImGui.OpenPopup("widget_bone_search_popup");
        }

        ImGui.SameLine();

        // Undo
        if(ImStarfallStudio.FontIconButton("undo", FontAwesomeIcon.Backward, "Undo", Capability.CanUndo)
            || (InputManagerService.ActionKeysPressedLastFrame(InputAction.Posing_Undo) && Capability.CanUndo))
            Capability.Undo();

        ImGui.SameLine();

        // Redo
        if(ImStarfallStudio.FontIconButton("redo", FontAwesomeIcon.Forward, "Redo", Capability.CanRedo)
            || (InputManagerService.ActionKeysPressedLastFrame(InputAction.Posing_Redo) && Capability.CanRedo))
            Capability.Redo();

        ImGui.SameLine();

        // Mirror Pose - Brio feature, keep
        if(ImStarfallStudio.FontIconButton("mirror_pose", FontAwesomeIcon.Repeat, "Mirror Pose"))
            Capability.MirrorPose();

        ImGui.SameLine();

        // Freeze
        if(Capability.Actor.IsProp == false)
        {
            if(ImStarfallStudio.ToggelFontIconButton("freezeActor", FontAwesomeIcon.Snowflake, new Vector2(0),
                timelineCapability.SpeedMultiplier == 0,
                hoverText: timelineCapability.SpeedMultiplierOverride == 0 ? "Un-Freeze Character" : "Freeze Character")
                || InputManagerService.ActionKeysPressedLastFrame(InputAction.Posing_Freeze))
            {
                if(timelineCapability.SpeedMultiplierOverride == 0)
                    timelineCapability.ResetOverallSpeedOverride();
                else
                    timelineCapability.SetOverallSpeedOverride(0f);
            }
            ImGui.SameLine();
        }

        // Reset (right-aligned)
        if(ImStarfallStudio.FontIconButtonRight("reset", FontAwesomeIcon.Undo, 1, "Reset Pose", Capability.HasOverride()))
            ImGui.OpenPopup("widget_reset_pose_popup");

        // Popups
        using(var popup = ImRaii.Popup("widget_reset_pose_popup", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if(popup.Success) DrawResetMenu();
        }

        using(var popup = ImRaii.Popup("widget_bone_search_popup", ImGuiWindowFlags.AlwaysAutoResize))
        {
            if(popup.Success) _boneSearchEditor.Draw("widget_bone_search", Capability);
        }
    }

    private void DrawResetMenu()
    {
        using(ImRaii.PushStyle(ImGuiStyleVar.ButtonTextAlign, new Vector2(0, 0.5f)))
        using(ImRaii.PushColor(ImGuiCol.Button, UIConstants.Transparent))
        {
            var buttonSize = new Vector2(155 * ImGuiHelpers.GlobalScale, 0);

            if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Undo, "Reset Pose", buttonSize))
            {
                Capability.Reset(false, false);
                ImGui.CloseCurrentPopup();
            }

            using(ImRaii.Disabled(!Capability.HasOverride(Capability.SkeletonPosing.FilterNonFaceBones)))
            {
                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.ChildReaching, "Reset Body", buttonSize))
                {
                    Capability.Snapshot(false, reconcile: false);
                    Capability.SkeletonPosing.PoseInfo.Clear(Capability.SkeletonPosing.FilterNonFaceBones);
                    ImGui.CloseCurrentPopup();
                }
            }

            using(ImRaii.Disabled(!Capability.HasOverride(Capability.SkeletonPosing.FilterFaceBones)))
            {
                if(ImStarfallStudio.DrawIconButton(FontAwesomeIcon.Smile, "Reset Face", buttonSize))
                {
                    Capability.SkeletonPosing.PoseInfo.Clear(Capability.SkeletonPosing.FilterFaceBones);
                    ImGui.CloseCurrentPopup();
                }
            }
        }
    }

    public override void ToggleAdvancedWindow()
    {
        UIManager.Instance.ToggleGraphicalPosingWindow();
    }
}
