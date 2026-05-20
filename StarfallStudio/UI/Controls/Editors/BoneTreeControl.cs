using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Game.Posing;
using StarfallStudio.Game.Posing.Skeletons;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace StarfallStudio.UI.Controls.Editors;

public static class BoneTreeControl
{
    public static void Draw(string id, PosingCapability posing)
    {
        var skeleton = posing.SkeletonPosing.CharacterSkeleton;
        if(skeleton == null)
        {
            ImGui.TextDisabled("No skeleton available.");
            return;
        }

        var lineHeight = ImGui.GetTextLineHeight();
        using var frame = ImRaii.Child($"{id}_frame", new Vector2(-1, lineHeight * 14), true,
            ImGuiWindowFlags.HorizontalScrollbar);
        if(!frame.Success) return;

        // Visible roots: bones whose parent is null or hidden
        foreach(var bone in skeleton.Bones)
        {
            if(bone.IsHidden) continue;
            if(bone.Parent != null && !bone.Parent.IsHidden) continue;
            DrawBoneNode(posing, bone, PoseInfoSlot.Character);
        }
    }

    private static void DrawBoneNode(PosingCapability posing, Bone bone, PoseInfoSlot slot)
    {
        var visibleChildren = bone.Children.Where(c => !c.IsHidden).ToList();
        var boneId = new BonePoseInfoId(bone.Name, bone.PartialId, slot);
        var isSelected = posing.IsBoneSelected(boneId);
        var isAncestor = !isSelected && IsAncestorOfSelected(bone, posing);

        var flags = ImGuiTreeNodeFlags.SpanAvailWidth;
        flags |= visibleChildren.Count > 0
            ? ImGuiTreeNodeFlags.OpenOnArrow
            : ImGuiTreeNodeFlags.Leaf;
        if(isSelected) flags |= ImGuiTreeNodeFlags.Selected;

        // Highlight bones that are ancestors of the selected bone
        if(isAncestor)
            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetStyle().Colors[(int)ImGuiCol.CheckMark]);

        bool open = ImGui.TreeNodeEx($"{bone.Name}_{bone.PartialId}", flags, bone.FriendlyName);

        if(isAncestor)
            ImGui.PopStyleColor();

        if(ImGui.IsItemClicked(ImGuiMouseButton.Left) && !ImGui.IsItemToggledOpen())
            posing.SetBoneSelection(boneId, false);

        if(open)
        {
            foreach(var child in visibleChildren)
                DrawBoneNode(posing, child, slot);
            ImGui.TreePop();
        }
    }

    private static bool IsAncestorOfSelected(Bone bone, PosingCapability posing)
    {
        foreach(var child in bone.Children)
        {
            var childId = new BonePoseInfoId(child.Name, child.PartialId, PoseInfoSlot.Character);
            if(posing.IsBoneSelected(childId)) return true;
            if(IsAncestorOfSelected(child, posing)) return true;
        }
        return false;
    }
}
