using StarfallStudio.Capabilities.Actor;
using StarfallStudio.Capabilities.Posing;
using StarfallStudio.Game.Actor.Appearance;
using StarfallStudio.Resources;
using StarfallStudio.UI.Controls.Editors;
using StarfallStudio.UI.Controls.Selectors;
using StarfallStudio.UI.Controls.Stateless;
using StarfallStudio.UI.Widgets.Core;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System.Numerics;

namespace StarfallStudio.UI.Widgets.Actor;

public class ActorAppearanceWidget(ActorAppearanceCapability capability) : Widget<ActorAppearanceCapability>(capability)
{
    public override string HeaderName => Capability.Actor.IsProp ? "Change Prop" : "Appearance";

    public override WidgetFlags Flags => Capability.Actor.IsProp ? WidgetFlags.DefaultOpen | WidgetFlags.DrawBody | WidgetFlags.DrawPopup | WidgetFlags.DrawQuickIcons
        : WidgetFlags.DefaultOpen | WidgetFlags.DrawBody | WidgetFlags.DrawQuickIcons | WidgetFlags.DrawPopup | WidgetFlags.HasAdvanced;

    private static readonly GearSelector _gearSelector = new("gear_selector");
    private const ActorEquipSlot _propSlots = ActorEquipSlot.Prop;
    private Vector2 IconSize => new(ImGui.GetTextLineHeight() * 3.9f);

    // Import mode - matches Ktisis's per-actor Import & Export checkboxes
    private bool _importCustomize = true;
    private bool _importEquipment = true;
    private bool _importWeapons = false;

    private AppearanceImportOptions ImportOptions
    {
        get
        {
            var opts = AppearanceImportOptions.ExtendedAppearance;
            if(_importCustomize) opts |= AppearanceImportOptions.Customize;
            if(_importEquipment) opts |= AppearanceImportOptions.Equipment;
            if(_importWeapons) opts |= AppearanceImportOptions.Weapon;
            return opts;
        }
    }

    public override void DrawBody()
    {
        if(Capability.Actor.IsProp)
        {
            DrawPropLoadAppearance();
            using var child1 = ImRaii.Child($"###appearance_child", new Vector2(0, 33 * ImGuiHelpers.GlobalScale), true);
            if(child1.Success)
                AppearanceEditorCommon.DrawPenumbraCollectionSwitcher(Capability);
        }
        else
        {
            DrawLoadAppearance();
            float size = (34 * ((Capability.HasCustomizePlusIntegration ? 1 : 0) + (Capability.HasPenumbraIntegration ? 1 : 0) + (Capability.HasGlamourerIntegration ? 1 : 0))) * ImGuiHelpers.GlobalScale;

            if(size != 0)
            {
                using var child1 = ImRaii.Child($"###appearance_child", new Vector2(0, size), true, ImGuiWindowFlags.NoScrollbar);
                if(child1.Success)
                    drawBody();
            }
            else
                drawBody();

            void drawBody()
            {
                AppearanceEditorCommon.DrawPenumbraCollectionSwitcher(Capability);
                AppearanceEditorCommon.DrawGlamourerDesignSwitcher(Capability);
                AppearanceEditorCommon.DrawCustomizePlusProfileSwitcher(Capability);
            }
        }
    }

    private void DrawPropLoadAppearance()
    {
        var currentAppearance = Capability.CurrentAppearance;
        var originalAppearance = Capability.OriginalAppearance;

        if(ImStarfallStudio.FontIconButton("attachweapon", FontAwesomeIcon.Retweet, "Reload Prop"))
        {
            Capability.AttachWeapon();
            Capability.Actor.GetCapability<PosingCapability>().LoadResourcesPose("Data.StarfallStudioPropPose.pose");
        }
        ImGui.SameLine();

        bool didChange = DrawReset(ref currentAppearance, originalAppearance);

        ImGui.Separator();

        didChange |= DrawPropSlot(ref currentAppearance, ref currentAppearance.Weapons.OffHand, ActorEquipSlot.Prop | ActorEquipSlot.OffHand);

        if(didChange)
            _ = Capability.SetAppearance(currentAppearance, AppearanceImportOptions.All);
    }

    private bool DrawReset(ref ActorAppearance currentAppearance, ActorAppearance originalAppearance)
    {
        bool didChange = false;

        bool equipChanged = !currentAppearance.Equipment.Equals(originalAppearance.Equipment) || !currentAppearance.Weapons.Equals(originalAppearance.Weapons) || !currentAppearance.Runtime.Equals(originalAppearance.Runtime);
        if(ImStarfallStudio.FontIconButtonRight("reset_equipment", FontAwesomeIcon.Undo, 1, "Reset Equipment", equipChanged))
        {
            currentAppearance.Equipment = originalAppearance.Equipment;
            currentAppearance.Weapons = originalAppearance.Weapons;
            currentAppearance.Runtime = originalAppearance.Runtime;
            didChange |= true;
        }

        return didChange;
    }

    private bool DrawPropSlot(ref ActorAppearance appearance, ref WeaponModelId equip, ActorEquipSlot slot)
    {
        bool didChange = false;

        var fallback = slot.GetEquipSlotFallback();

        int equipId = equip.Id;
        int equipVariant = equip.Variant;
        int equipType = equip.Type;

        var model = GameDataProvider.Instance.ModelDatabase.GetModelById(equip, _propSlots);

        using(ImRaii.PushId("DrawPropSlot"))
        {
            if(ImStarfallStudio.BorderedGameIcon("##icon", model?.Icon ?? 0, fallback, size: IconSize))
            {
                _gearSelector.SetGearSelect(model, _propSlots);
                ImGui.OpenPopup("gear_popup");
            }

            ImGui.SameLine();

            using(var group = ImRaii.Group())
            {
                {
                    string description = $"{model?.Name ?? "Unknown"}";

                    ImGui.Text(description);

                    ImGui.SetNextItemWidth(ImGui.CalcTextSize("XXXXX").X);
                    if(ImGui.InputInt("##id", ref equipId, 0, 0, default, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        equip.Id = (ushort)equipId;
                        didChange |= true;
                    }

                    ImGui.SameLine();

                    ImGui.SetNextItemWidth(ImGui.CalcTextSize("XXXXX").X);
                    if(ImGui.InputInt("##type", ref equipType, 0, 0, default, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        equip.Type = (ushort)equipType;
                        didChange |= true;
                    }

                    ImGui.SameLine();

                    ImGui.SetNextItemWidth(ImGui.CalcTextSize("XXXXX").X);
                    if(ImGui.InputInt("##variant", ref equipVariant, 0, 0, default, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        equip.Variant = (byte)equipVariant;
                        didChange |= true;
                    }

                    using(var gearPopup = ImRaii.Popup("gear_popup"))
                    {
                        if(gearPopup.Success)
                        {
                            _gearSelector.Draw();
                            if(_gearSelector.SoftSelectionChanged && _gearSelector.SoftSelected != null)
                            {
                                equip.Value = _gearSelector.SoftSelected.ModelId;
                                didChange |= true;
                            }
                            if(_gearSelector.SelectionChanged)
                                ImGui.CloseCurrentPopup();

                        }
                    }
                }
            }

        }

        return didChange;
    }

    private void DrawLoadAppearance()
    {
        // Advanced appearance editor + MCDF + Reset - always visible
        if(ImStarfallStudio.FontIconButton("advanced_appearance", FontAwesomeIcon.UserEdit, "Advanced Appearance"))
            ToggleAdvancedWindow();

        ImGui.SameLine();

        using(ImRaii.Disabled(Capability.CanMCDF is false))
        {
            using(ImRaii.Disabled(Capability.IsSelf || Capability.IsAnyMCDFLoading))
            {
                if(ImStarfallStudio.FontIconButton("load_mcdf", FontAwesomeIcon.CloudDownloadAlt, "Load MCDF"))
                    FileUIHelpers.ShowImportMCDFModal(Capability);
                ImGui.SameLine();
            }
            if(Capability.IsSelf)
                ImStarfallStudio.AttachToolTip("Can not load a MCDF on your Player Character. Spawn an Actor to load a MCDF.");
            if(Capability.IsAnyMCDFLoading)
                ImStarfallStudio.AttachToolTip("Another MCDF is loading, Please wait for it to finish.");

            using(ImRaii.Disabled(Capability.HasMCDF))
            {
                if(ImStarfallStudio.FontIconButton("save_mcdf", FontAwesomeIcon.CloudUploadAlt, "Save MCDF"))
                    FileUIHelpers.ShowExportMCDFModal(Capability);
                ImGui.SameLine();
            }
            if(Capability.HasMCDF)
                ImStarfallStudio.AttachToolTip("Can not save a MCDF of a Actor that has a MCDF loaded. Reset this Actor to save a MCDF.");
        }

        if(ImStarfallStudio.FontIconButtonRight("reset_appearance", FontAwesomeIcon.Undo, 1, "Reset", Capability.IsAppearanceOverridden))
            _ = Capability.ResetAppearance();

        // Import & Export - collapsing, matches Ktisis workflow
        if(ImGui.CollapsingHeader("Import & Export"))
        {
            ImGui.Spacing();

            // Mode checkboxes - two groups like Ktisis
            ImGui.BeginGroup();
            ImGui.Text("Appearance");
            ImGui.Checkbox("Customize##imp", ref _importCustomize);
            ImGui.EndGroup();

            ImGui.SameLine();

            ImGui.BeginGroup();
            ImGui.Text("Equipment");
            ImGui.Checkbox("Gear##imp", ref _importEquipment);
            ImGui.SameLine();
            ImGui.Checkbox("Weapons##imp", ref _importWeapons);
            ImGui.EndGroup();

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // .chara import / export
            if(ImGui.Button("Import##chara"))
                FileUIHelpers.ShowImportCharacterModal(Capability, ImportOptions);

            ImGui.SameLine();

            if(ImGui.Button("Export##chara"))
                FileUIHelpers.ShowExportCharacterModal(Capability);

            ImGui.Spacing();

            // NPC import - uses same mode options, matches Ktisis "Import NPC" button
            if(ImGui.Button("Import NPC"))
            {
                AppearanceEditorCommon.ResetNPCSelector();
                ImGui.OpenPopup("widget_npc_selector");
            }

            ImGui.Spacing();
        }

        using(var popup = ImRaii.Popup("widget_npc_selector"))
        {
            if(popup.Success)
            {
                if(AppearanceEditorCommon.DrawNPCSelector(Capability, ImportOptions))
                    ImGui.CloseCurrentPopup();
            }
        }
    }

    public override void DrawPopup()
    {
        var toggele = Capability.IsHidden ? "Show" : "Hide";
        if(ImGui.MenuItem($"{toggele} {Capability.Actor.FriendlyName}###Appearance_popup_toggle"))
            Capability.ToggleVisibility();
    }

    public override void DrawQuickIcons()
    {
        if(ImStarfallStudio.FontIconButton("redrawwidget_redraw", FontAwesomeIcon.PaintBrush, "Redraw"))
        {
            _ = Capability.Redraw();
        }
    }

    public override void ToggleAdvancedWindow()
    {
        UIManager.Instance.ToggleAppearanceWindow();
    }
}
