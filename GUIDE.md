# Starfall Studio — User Guide

> **Quick navigation** — jump to the section you need:
> - [Installation](#installation)
> - [Opening the Plugin](#opening-the-plugin)
> - [Actors Panel](#actors-panel)
> - [Adding Overworld / NPC Actors](#adding-overworld--npc-actors)
> - [Appearance](#appearance)
> - [Extended Appearance](#extended-appearance)
> - [Companion / Mount / Minion / Ornament](#companion--mount--minion--ornament)
> - [Posing & Bone Editing](#posing--bone-editing)
> - [Animation Control](#animation-control)
> - [Dynamic Face Control](#dynamic-face-control)
> - [Camera](#camera)
> - [World — Time & Weather](#world--time--weather)
> - [World — Sky Editor](#world--sky-editor)
> - [World — Environment](#world--environment)
> - [World — Advanced](#world--advanced)
> - [World — Festivals](#world--festivals)
> - [Lighting](#lighting)
> - [Status Effects](#status-effects)
> - [Library](#library)
> - [Projects](#projects)
> - [MCDF Import](#mcdf-import)
> - [Settings](#settings)
> - [Commands Reference](#commands-reference)
> - [Requirements](#requirements)
> - [Tips & Caveats](#tips--caveats)

---

## Installation

Starfall Studio is a custom plugin — not in the official Dalamud list.

1. Open **Dalamud Settings** → **Experimental** tab
2. Under **Custom Plugin Repositories**, add:
   ```
   https://raw.githubusercontent.com/BluntEXE/StarfallStudio/main/repo.json
   ```
3. Click **Save & Close**
4. Open `/xlplugins` → search **Starfall Studio** → Install

---

## Opening the Plugin

| Command | Action |
|---|---|
| `/starfall` | Toggle the main window |
| `/starfall window` | Open the main window |
| `/starfall settings` | Open settings |
| `/mcdf` | Toggle the MCDF import window |

The main window must be open before entering GPose. Most features are only active while GPose is running.

---

## Actors Panel

The **Actors** panel is the scene list — every character in your GPose session appears here.

### Toolbar buttons (top row)

| Button | Action |
|---|---|
| `+` (Plus) | Spawn a new blank actor (no companion slot) |
| `+□` (Plus Square) | Spawn a new actor with a companion slot reserved |
| Cubes | Spawn a prop |
| Clone | Duplicate the selected actor |
| Trash | Destroy selected actor (or **Release** if it is an overworld actor) |
| Bullseye | Set the selected actor as the GPose camera target |
| Folder Tree | Select the actor in the hierarchy panel |
| Bomb | Destroy all spawned actors |

### Actor list

Each row has a `+` / `-` pin toggle on the left:

- **`+` (unmanaged)** — actor is in the list but hidden in-world. Click to pin.
- **`-` (managed/pinned)** — actor is visible in-world. Click to unpin (hides + restores position for overworld actors).

Click an actor name to select it — the right-hand panels update to that actor's controls.

Use the **Filter** box at the top to search by name when the list is long.

### Right-click popup menu

Right-click anywhere in the panel for **New…** submenu (Spawn / Spawn with Companion / Spawn Prop) and **Destroy All Actors** (with confirmation).

---

## Adding Overworld / NPC Actors

In open-world GPose (e.g. at a venue or city), ambient NPCs and other players are hidden by default so they don't clutter your scene. You can selectively pull any of them into your scene.

### How to add an overworld actor

1. Open the **Actors** panel → right-click or use the popup menu → **Add Overworld Actor**
2. A dropdown lists all nearby characters sorted by distance. Select the one you want.
3. The actor teleports to 1.5 yalms in front of you and appears in the actor list as pinned.
4. Use the gizmo or posing tools normally — the actor is now fully controllable.

### Notes

- **Ambient actors are hidden automatically** the moment you enter open-world GPose. You will only see the local player and any actors you have explicitly added.
- **Overworld actors cannot be destroyed** — they are real game characters. The **Trash** button says **Release** and removes them from your scene; the game restores them normally.
- **Unpinning** an overworld actor hides it again and returns it to its original world position.
- **GPose exit** — all actors are returned to their original world positions automatically. Nothing is left displaced.
- Mare Synchronos modded appearances are preserved because the appearance system is not touched for overworld actors.

---

## Appearance

Select an actor in the hierarchy, then open the **Appearance** panel.

### Tabs

| Tab | Contents |
|---|---|
| Appearance | Race, gender, face features, hair, eye colour, skin tone |
| Equipment | Gear slots, dyes, weapons |

### Buttons

| Button | Action |
|---|---|
| Import | Load a `.chara` file onto this actor |
| Export | Save this actor's current appearance as a `.chara` file |
| Import NPC | Browse game NPC appearances and apply one to this actor |
| Advanced Appearance | Open the full appearance editor window (Extended) |
| Load MCDF | Load a Penumbra/Glamourer mod pack (requires MCDF window) |
| Save MCDF | Export current appearance as MCDF |
| Undo (↺) | Reset appearance back to original |
| Redraw (paintbrush) | Force a manual redraw of the actor |

### Show / Hide toggle

Right-click the Appearance panel header → toggle visibility of the actor in-world without removing them from the scene.

---

## Posing & Bone Editing

Select an actor, then use the **Posing** panel or open the **Posing Overlay** for a full-screen bone view.

### Bone tree

- Bones are listed in hierarchy order with expandable groups.
- Click a bone to select it — the transform editor (Position / Rotation / Scale) updates on the right.
- Use the **category filter** buttons at the top to show only body, face, hand, etc. bones.
- **Search** box filters the bone list by name.

### Transform editor

| Control | Action |
|---|---|
| Position / Rotation / Scale sliders | Drag to adjust the selected bone |
| Mirror mode button (link icon) | None / Copy / Mirror — controls whether left/right bones move together |
| IK button | Enable inverse kinematics for eligible bones (limbs) |
| Propagate | Push transform changes down the bone chain |
| Copy & Paste | Copy transform from one bone; paste to another |
| Reset Bone (↺) | Return selected bone to its default transform |

### Gizmo (world-space)

The 3D gizmo appears on the selected actor in-world. Drag the arrows/rings to move or rotate the whole actor. The gizmo position speed can be adjusted via the gauge button in the toolbar.

### Pose import / export

- **Import pose** — load a `.pose` file (Brio-compatible). Options per section: Body / Face / Hands / Weapons / Expression.
- **Export pose** — save current bone positions to a `.pose` file.
- **Mirror pose** — flip the entire pose left-to-right.
- **Undo / Redo** — per-bone history; undo with the toolbar button.

---

## Animation Control

The **Animation Control** panel manages what animation is playing on the selected actor.

### Global controls (top row)

| Button | Action |
|---|---|
| Freeze Physics | Stops cloth/hair physics simulation |
| Actors ▼ | Dropdown: Freeze All / Un-Freeze All / Play All / Stop All |
| Reset (↺) | Reset this actor's animation override |

### Base animation

Search and select an animation from the game library. Press **Play** to apply it, **Stop** to revert.

### Blend animation

A secondary animation slot that overlays on top of the base. Useful for facial expressions layered over body animations.

### Speed control

- Per-slot speed slider — drag to slow down or speed up.
- **Pause** button freezes the animation at the current frame.
- **Delimit Speed** checkbox removes the speed cap.
- Global **Speed Multiplier** applies to all slots at once.

### Timeline scrub

Drag the **Scrub** slider to step through animation frames manually when the animation is paused.

### Lips / Expression

Separate controls for lip sync and facial expression slots.

### Cutscene / Camera path

Browse and play a camera path file to replay a cutscene camera alongside the animation.

---

## Dynamic Face Control

The **Dynamic Face Control** panel gives real-time control over where the actor is looking without editing bones manually.

### Enable

Toggle **Enable Face Control** at the top.

### Look-at target

Choose what the actor tracks:
- **Camera** — actor looks toward the GPose camera
- **Position** — actor looks at a fixed world coordinate
- **Actor** — actor looks at another actor in the scene

### Part toggles

Enable or disable tracking for **Eyes**, **Body**, and **Head** independently. Each has a **Set to camera value** shortcut.

### Reset

**Reset Selected Actor** reverts face control to the original animation-driven look direction.

---

## Camera

### Camera types

| Type | How to create |
|---|---|
| Starfall Studio Camera | Click **New Starfall Studio Camera** in the Camera panel |
| Free-Cam | Click **New Free-Cam** |

### Starfall Studio Camera controls

| Parameter | Description |
|---|---|
| Offset | Shift the camera relative to its current position |
| Angle | Horizontal / vertical angle adjust |
| Pan | Lateral pan |
| Rotation (Pivot) | Roll the camera around its forward axis |
| FoV | Field of view (zoom equivalent) |
| Zoom | Distance from the target |
| Reset (↺) per parameter | Revert that value to default |

### Free-Cam

- **Enable Movement** toggle activates WASD + mouse look.
- **Lateral Movement** (solar panel icon) switches to strafe mode.
- Movement Speed and Mouse Sensitivity sliders are adjustable with individual reset buttons.
- **Reset Camera** reverts the entire camera to its original GPose state.

### Camera path / Cutscene

Browse a camera path file and play it to replay a cutscene camera. Used together with Animation Control for full cutscene recreation.

---

## World — Time & Weather

Controls the game world's time and weather while in GPose.

### Time of Day

- Drag the slider to set the time. The current time is shown as a clock string.
- **Lock Time** (padlock icon) freezes the time so it does not advance.
- Expand for **Day of Month** control.

### Weather

- The current weather icon is displayed. Click it to open the weather selector.
- **Lock Weather** freezes the weather so it does not cycle.
- Enter a **Weather ID** directly in the number box if you know the ID.

---

## World — Environment

Fine-grained control over environmental effects. Switch between tabs:

### Particles

| Control | Description |
|---|---|
| Particle Texture | Click the preview to open the texture selector; pick a dust/particle texture |
| Particle Count (Intensity) | 0.0 – 1.0 slider |
| Particle Size | 0.0 – 20.0 |
| Particle Color | RGBA colour picker |
| Particle Glow | 0.0 – 10.0 |
| Particle Spread | 0.0 – 10.0 |
| Reset Particles (↺) | Revert particles to zone default |

### Rain / Wind / Fog

Additional sliders for rain intensity, wind direction and strength, and fog density/colour (zone-dependent).

---

## World — Festivals

Festival flags activate seasonal decorations and effects in the game world (e.g. Starlight Celebration, Heavensturn).

- The active festival list shows currently enabled flags.
- Enter a **Festival ID** or click **Search** to browse by name.
- **+** adds the selected festival; **−** removes it.
- **Reset (↺)** clears all overrides and restores zone defaults.

---

## Lighting

The **Lighting** panel lets you place and edit up to four virtual lights in the scene.

### Light container toolbar

- **+** Add a new light
- **Trash** Remove selected light

### Per-light controls (Light Editor)

| Parameter | Description |
|---|---|
| Type | Point / Spot / Directional |
| Color | RGB colour picker |
| Intensity | Brightness multiplier |
| Range | How far the light reaches |
| Position | XYZ world-space position (drag or enter values) |
| Rotation | Direction the light faces |
| Falloff | How quickly brightness drops with distance |
| Shadow toggle | Enable/disable shadow casting |

All parameters have individual **Reset** buttons. The light transform also supports the world-space gizmo for positioning in-world.

---

## Status Effects

The **Status Effects** panel lets you apply and remove visual status effect VFX on the selected actor.

- **+** opens the status effect search — find by name.
- **−** removes the currently selected effect.
- **Hide No-VFX** checkbox filters out effects that have no visible particle/animation.
- Effects are listed by name and ID; hover for details.

> Status effects here are visual only — they do not affect gameplay.

---

## Extended Appearance

Opened via the **Advanced Appearance** button in the Appearance panel, or the pop-out icon on the panel header.

### Transparency

- **Alpha** slider (0.0 – 1.0) — make the actor fully transparent (0) to fully opaque (1). Useful for ghost/spirit effects.

### Wetness

| Control | Range | Effect |
|---|---|---|
| Wet | 0.0 – 1.0 | How wet the skin/clothing surface appears |
| Wet Depth | 0.0 – 3.0 | Depth of the wetness effect (how far it penetrates) |

### Tints

Per-slot colour tints for skin, hair, and equipment pieces. Click a colour swatch to open the colour picker.

### Model Shader controls

Accessible under **Advanced Appearance** → shader tabs:

| Tab | Controls |
|---|---|
| Muscle | Muscle definition intensity |
| Body | Body shader parameters (skin specularity, etc.) |
| Hair | Hair shader (gloss, specularity) |
| Other | Miscellaneous shader parameters |

**Reset Extended (↺)** — reverts all extended appearance changes at once.

---

## Companion / Mount / Minion / Ornament

The **Companion** panel appears under a selected actor if they have a companion slot reserved (spawned with **Spawn with Companion**).

- Shows the current companion type: **Mount**, **Minion**, or **Ornament**.
- Right-click the panel header → **Destroy Companion** to remove the companion from this actor.
- Companion appearance and posing can be controlled by selecting the companion entry in the hierarchy (it appears as a child of the parent actor).

> To have a companion available, spawn the actor using **Spawn with Companion** (or the `+□` button in the Actors panel). Plain **Spawn** does not reserve a companion slot.

---

## World — Sky Editor

The **Sky** panel controls the sky dome, stars, moon, clouds, and ambient scene lighting. Tabs: **Sky**, **Stars**, **Clouds**.

### Stars tab

| Control | Range | Description |
|---|---|---|
| Star Count | 0 – 20 | Number of stars visible |
| Star Intensity | 0 – 2.5 | Brightness of stars |
| Moon Color | colour picker | Tint of the moon |
| Moon Brightness | 0 – 1.0 | Brightness of the moon |
| Constellation Count | 0 – 10 | Number of constellation lines |
| Constellation Intensity | 0 – 2.5 | Brightness of constellations |
| Galaxy Intensity | 0 – 10 | Milky Way/galaxy band brightness |

### Sky tab

| Control | Description |
|---|---|
| Sky Texture | Click the preview to open the sky texture selector |
| Sky Texture ID | Enter a texture ID directly |
| Sun Visibility (fog) | How much the sun shows through fog |
| Ambient Temperature | -2.5 to 2.5 — warm/cool colour shift on ambient light |
| Ambient Saturation | 0 – 5 — colour saturation of ambient lighting |
| Ambient Color | RGB picker for overall ambient light colour |
| Sunlight Color | Colour of direct sunlight |
| Moonlight Color | Colour of moonlight |

### Clouds tab

- Cloud texture selector (click preview to browse).
- Cloud side texture selector.
- Cloud density and movement sliders (zone-dependent).

Each tab has an individual **Reset (↺)** button.

---

## World — Advanced

Located in the World panel under the **Advanced** collapsing header.

| Control | Description |
|---|---|
| Freeze Water | Stops water surface animation — useful for still-water reflection shots |

---

## Library

The **Library** is a file browser for poses, appearances, and character files stored on your disk. Open it via the library icon in the main toolbar or from the Appearance / Posing import dialogs.

### Browsing

- Files are grouped by type: **Poses**, **Characters**, **Scenes**, and any custom sources you have added.
- Use the **search box** to filter by filename or tag.
- Press **TAB** to filter by a tag shown in the suggestion bar.
- Click a file to preview it; double-click or press **Import** to apply it to the selected actor.

### Favourites

Click the star icon on any file to add it to favourites. Favourites appear at the top of the list.

### Import options

Click the **cog** icon next to Import to open per-section import options (Body / Face / Hands / Weapons / Expression toggles for poses).

### Adding sources

Go to **Settings → Library** to add custom folder paths as library sources. Click **Scan** (refresh icon) to re-index after adding files.

---

## Projects

> **Beta** — projects saved in the current version may be incompatible with future versions.

The **Project** system saves and restores a full scene: actor positions, appearances, and poses together.

### Saving a project

1. Open the Project window from the main toolbar.
2. Switch to the **Save** tab.
3. Enter a **Project Name** and **Description**.
4. Click **Save Project**.

### Loading a project

1. Open the Project window → **Load** tab.
2. Select a project from the list — name, description, and creation date are shown.
3. Click **Load** to restore the scene.
4. Click **Delete** to permanently remove a project.

---

## MCDF Import

MCDF is a mod pack format that bundles a Penumbra mod with a Glamourer appearance profile.

**Requires:** Penumbra + Glamourer both installed and active.

### Opening

Type `/mcdf` or use the **Load MCDF** button in the Appearance panel.

### Workflow

1. Click **Browse** and select a `.mcdf` file.
2. Choose which actor to apply it to from the actor dropdown.
3. Click **Load** — Penumbra applies the mod, Glamourer applies the appearance.
4. Use **Save MCDF** to export the current actor's appearance + active Penumbra mods into a new `.mcdf` file.

> Note: spawned actors are internally named for Glamourer compatibility. If you see a Glamourer error about an unknown actor, try re-loading the MCDF after the actor has fully initialised.

---

## Settings

Open via `/starfall settings`. Seven tabs:

### General

| Setting | Description |
|---|---|
| Use Library when importing | Opens the Library browser automatically on Import |
| Open Library to last location | Remembers your last library folder |
| Use filename as Actor Name | Sets the actor's display name from the loaded filename |
| Censor Actor Names | Replaces real player names with pseudonyms across the UI |
| Hide Name in Group Pose Settings window | Hides character names in FFXIV's built-in GPose window |
| Enable StarfallStudio Color | Applies the plugin's custom UI colour theme |
| Enable StarfallStudio Scale | Applies custom UI scaling |
| Transform Slider Speed | Default speed for bone transform sliders (Position / Rotation / Scale) |
| Open StarfallStudio behaviour | When the plugin window opens automatically (GPose entry, manual, etc.) |

### IPC

| Setting | Description |
|---|---|
| Enable StarfallStudio IPC | Enables the plugin's inter-plugin API for other plugins to connect |
| Enable StarfallStudio API | Enables the local HTTP API (for external tools) |
| Allow Penumbra Integration | Enable/disable Penumbra IPC (required for MCDF) |
| Allow Glamourer Integration | Enable/disable Glamourer IPC (required for MCDF) |
| Allow Customize+ Integration | Enable/disable Customize+ IPC (body scaling) |

Each third-party integration shows its current status and has a **Refresh** button.

### Posing

| Setting | Description |
|---|---|
| Show in GPose | Show plugin window when entering GPose |
| Show in Cutscenes | Show plugin window during cutscenes |
| Show when UI Hidden | Keep plugin visible when FFXIV UI is hidden |
| Disable GPose Mouse Select | Prevent FFXIV's click-to-target from stealing focus |
| StarfallStudio Target changes with GPose Target | Sync hierarchy selection to GPose camera target |
| GPose Target changes with StarfallStudio Target | Sync GPose camera target to hierarchy selection |
| Select Model Transform on Entity Select | Auto-select the root bone when you click an actor |

### Posing — Overlay

| Setting | Description |
|---|---|
| Overlay Defaults On | Bone overlay visible by default when GPose is entered |
| Make Model Transform bone stand out | Highlights the root/origin bone in the overlay |
| Allow Gizmo Axis Flip | Allows gizmo axes to flip when dragged past centre |
| Hide Gizmo while Advanced Posing | Hides the world gizmo when the bone editor is open |
| Hide Toolbar while Advanced Posing | Hides the posing toolbar when the bone editor is open |
| Show Skeleton Lines | Draws lines between bones in the overlay |
| Show Bone Circles | Draws clickable circles on bones in the overlay |
| Draw skeleton line to edge of bone circle | Extends skeleton lines to the circle edge |
| Hide Skeleton when Gizmo Active | Hides skeleton overlay while dragging the gizmo |
| Overlay Colors | Per-category colour pickers for the bone overlay |

### Library

Configure library source folders — add or remove disk paths that the Library browser scans for pose and character files.

### Auto-Save

| Setting | Description |
|---|---|
| Auto-Save Enabled | Periodically saves the current scene automatically |
| Save Individual Poses | Also saves each actor's pose as a separate `.pose` file |

### Input

Key binding configuration — assign keyboard shortcuts to posing actions (toggle link mode, select all bones, etc.).

### Advanced

Developer and diagnostic options. Not required for normal use.

---

## Commands Reference

| Command | Action |
|---|---|
| `/starfall` | Toggle main window |
| `/starfall window` | Open main window |
| `/starfall settings` | Open settings |
| `/mcdf` | Toggle MCDF window |

---

## Requirements

| Plugin | Required for |
|---|---|
| [Penumbra](https://github.com/xivdev/Penumbra) | MCDF mod loading |
| [Glamourer](https://github.com/Ottermandias/Glamourer) | MCDF appearance application |
| Customize+ | Optional — MCDF body scaling |

Core features (posing, animation, camera, world, overworld actors) work without any additional plugins.

---

## Tips & Caveats

- **Open-world GPose vs instanced GPose** — the plugin detects which mode you are in automatically. Overworld actor importing only applies in open-world GPose (cities, housing, field areas).
- **Mare Synchronos** — other players' modded appearances sync via Mare at actor load time independently of this plugin. Skipping the appearance system for overworld actors intentionally preserves those synced mods.
- **Pose files** use Brio's `.pose` format. Files exported from Brio are fully compatible.
- **Companion slots** — if you need to attach a mount, minion, or ornament to a spawned actor, use **Spawn with Companion** instead of plain **Spawn**.
- **Props** — spawned props share the same actor slots as characters. Appearance editing on props is limited.
- **Undo** is per-bone in the posing panel. There is no global scene undo — be careful with Destroy All.
- **Plugin updates** install automatically via Dalamud when a new version is published.
