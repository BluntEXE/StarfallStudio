# Starfall Studio - User Guide

**Quick navigation:**
- [Installation](#installation)
- [Opening the Plugin](#opening-the-plugin)
- [Actors Panel](#actors-panel)
- [Adding Overworld / NPC Actors](#adding-overworld--npc-actors)
- [Appearance](#appearance)
- [Extended Appearance](#extended-appearance)
- [Companion / Mount / Minion / Ornament](#companion--mount--minion--ornament)
- [Posing & Bone Editing](#posing--bone-editing)
- [Animation Control](#animation-control)
- [Dynamic Face Control](#dynamic-face-control)
- [Camera](#camera)
- [World - Time & Weather](#world---time--weather)
- [World - Sky Editor](#world---sky-editor)
- [World - Environment](#world---environment)
- [World - Advanced](#world---advanced)
- [World - Festivals](#world---festivals)
- [Lighting](#lighting)
- [Status Effects](#status-effects)
- [Library](#library)
- [Projects](#projects)
- [MCDF Import](#mcdf-import)
- [Settings](#settings)
- [Commands Reference](#commands-reference)
- [Requirements](#requirements)
- [Tips & Caveats](#tips--caveats)

---

## Installation

Custom plugin - not in the official Dalamud list.

1. Open **Dalamud Settings** → **Experimental** tab
2. Under **Custom Plugin Repositories**, add:
   ```
   https://raw.githubusercontent.com/BluntEXE/StarfallStudio/main/repo.json
   ```
3. Click **Save & Close**
4. Open `/xlplugins`, search **Starfall Studio**, and install

---

## Opening the Plugin

| Command | Action |
|---|---|
| `/starfall` | Toggle the main window |
| `/starfall window` | Open the main window |
| `/starfall settings` | Open settings |
| `/mcdf` | Toggle the MCDF import window |

The main window must be open before entering GPose. Most features only activate while GPose is running.

---

## Actors Panel

The **Actors** panel is the scene list. Every character in your GPose session appears here.

### Toolbar buttons

| Button | Action |
|---|---|
| `+` (Plus) | Spawn a new blank actor (no companion slot) |
| `+□` (Plus Square) | Spawn a new actor with a companion slot reserved |
| Cubes | Spawn a prop |
| Clone | Duplicate the selected actor |
| Trash | Destroy selected actor (shows **Release** for overworld actors) |
| Bullseye | Set the selected actor as the GPose camera target |
| Folder Tree | Select the actor in the hierarchy panel |
| Bomb | Destroy all spawned actors |

### Actor list

Each row has a `+` / `-` pin toggle on the left:

- **`+` (unmanaged):** actor is in the list but hidden in-world. Click to pin and show.
- **`-` (pinned):** actor is visible in-world. Click to unpin and hide.

Click an actor name to select it. The right-hand panels update to show that actor's controls.

Use the **Filter** box to search by name.

### Right-click menu

Right-click anywhere in the panel for the **New...** submenu (Spawn / Spawn with Companion / Spawn Prop) and **Destroy All Actors** (requires confirmation).

---

## Adding Overworld / NPC Actors

In open-world GPose (venues, cities, housing areas), ambient NPCs and other players are hidden by default. You can pull any of them into your scene individually.

### How to add an overworld actor

1. Open the **Actors** panel, right-click, and select **Add Overworld Actor**
2. A dropdown lists nearby characters sorted by distance. Pick one.
3. The actor teleports to 1.5 yalms in front of you and appears in the list as pinned.
4. Use the gizmo or posing tools normally.

### Notes

- **Ambient actors are hidden automatically** when you enter open-world GPose. Only the local player and explicitly added actors are visible.
- **Overworld actors cannot be destroyed** - they are real game characters. The **Release** button removes them from your scene; the game restores them normally.
- **Unpinning** an overworld actor hides it and returns it to its original world position.
- **On GPose exit**, all actors return to their original world positions. Nothing stays displaced.
- Modded appearances on overworld actors are preserved. The plugin does not trigger the appearance system for overworld actors, so any active mods are left untouched.

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
| Export | Save this actor's appearance as a `.chara` file |
| Import NPC | Browse NPC appearances and apply one to this actor |
| Advanced Appearance | Open the extended appearance editor |
| Load MCDF | Load a Penumbra/Glamourer mod pack |
| Save MCDF | Export current appearance as MCDF |
| Undo (↺) | Reset appearance to original |
| Redraw (paintbrush) | Force a manual redraw |

### Show / Hide toggle

Right-click the Appearance panel header to toggle the actor's visibility in-world without removing them from the scene.

---

## Extended Appearance

Open via **Advanced Appearance** in the Appearance panel.

### Transparency

**Alpha** slider (0.0 to 1.0): 0 is fully transparent, 1 is fully opaque. Useful for ghost or spirit effects.

### Wetness

| Control | Range | Effect |
|---|---|---|
| Wet | 0.0 to 1.0 | How wet the skin / clothing surface appears |
| Wet Depth | 0.0 to 3.0 | Depth of the wetness effect |

### Tints

Per-slot colour tints for skin, hair, and equipment. Click a colour swatch to open the picker.

### Model Shader tabs

| Tab | Controls |
|---|---|
| Muscle | Muscle definition intensity |
| Body | Skin specularity and body shader parameters |
| Hair | Hair gloss and specularity |
| Other | Miscellaneous shader parameters |

**Reset Extended (↺)** reverts all extended appearance changes at once.

---

## Companion / Mount / Minion / Ornament

The **Companion** panel appears under a selected actor if they have a companion slot reserved.

- Shows the current companion type: **Mount**, **Minion**, or **Ornament**.
- Right-click the panel header and select **Destroy Companion** to remove it.
- Select the companion's entry in the hierarchy to pose or edit its appearance separately.

To have a companion slot available, spawn the actor with **Spawn with Companion** (the `+□` button). Plain **Spawn** does not reserve a companion slot.

---

## Posing & Bone Editing

Select an actor, then use the **Posing** panel or open the **Posing Overlay** for a full-screen bone view.

### Bone tree

- Bones are listed in hierarchy order with expandable groups.
- Click a bone to select it. The transform editor updates on the right.
- Use the **category filter** buttons to show only body, face, hand, or other bone groups.
- The **Search** box filters by bone name.

### Transform editor

| Control | Action |
|---|---|
| Position / Rotation / Scale sliders | Drag to adjust the selected bone |
| Mirror mode (link icon) | None / Copy / Mirror - controls whether left/right bones move together |
| IK | Enable inverse kinematics for eligible bones |
| Propagate | Push transform changes down the bone chain |
| Copy & Paste | Copy a transform from one bone and paste it to another |
| Reset Bone (↺) | Return the selected bone to its default |

### Gizmo (world-space)

The 3D gizmo appears on the selected actor in-world. Drag the arrows or rings to move or rotate the whole actor. Adjust gizmo speed with the gauge button in the toolbar.

### Pose import / export

- **Import pose:** load a `.pose` file (Brio-compatible). Per-section options: Body / Face / Hands / Weapons / Expression.
- **Export pose:** save current bone positions to a `.pose` file.
- **Mirror pose:** flip the pose left-to-right.
- **Undo / Redo:** per-bone history.

---

## Animation Control

The **Animation Control** panel manages what animation plays on the selected actor.

### Global controls

| Button | Action |
|---|---|
| Freeze Physics | Stops cloth and hair physics |
| Actors dropdown | Freeze All / Un-Freeze All / Play All / Stop All |
| Reset (↺) | Reset this actor's animation override |

### Base animation

Search and select an animation from the game library. Press **Play** to apply it, **Stop** to revert.

### Blend animation

A secondary slot that plays on top of the base animation. Useful for layering facial expressions over body animations.

### Speed control

- Per-slot speed slider: slow down or speed up.
- **Pause** freezes the animation at the current frame.
- **Delimit Speed** removes the speed cap.
- **Speed Multiplier** applies to all slots at once.

### Timeline scrub

Drag the **Scrub** slider to step through frames manually while paused.

### Lips / Expression

Separate controls for lip sync and facial expression animation slots.

### Cutscene / Camera path

Browse and play a camera path file alongside the animation for full cutscene recreation.

---

## Dynamic Face Control

Controls where the actor is looking without manually editing face bones.

### Enable

Toggle **Enable Face Control** at the top of the panel.

### Look-at target

- **Camera:** actor looks toward the GPose camera
- **Position:** actor looks at a fixed world coordinate
- **Actor:** actor looks at another actor in the scene

### Part toggles

Enable or disable tracking for **Eyes**, **Body**, and **Head** independently. Each has a **Set to camera value** shortcut.

### Reset

**Reset Selected Actor** reverts face control to the animation-driven look direction.

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
| Angle | Horizontal / vertical angle |
| Pan | Lateral pan |
| Rotation (Pivot) | Roll around the forward axis |
| FoV | Field of view |
| Zoom | Distance from the target |
| Reset (↺) per parameter | Revert that value to default |

### Free-Cam

- **Enable Movement** activates WASD + mouse look.
- **Lateral Movement** (solar panel icon) switches to strafe mode.
- Movement Speed and Mouse Sensitivity have individual sliders and reset buttons.
- **Reset Camera** reverts to the original GPose camera state.

### Camera path / Cutscene

Browse a camera path file and play it back. Use together with Animation Control for full cutscene recreation.

---

## World - Time & Weather

Controls the game world's time and weather while in GPose.

### Time of Day

- Drag the slider to set the time.
- **Lock Time** (padlock) freezes the clock.
- Expand for **Day of Month** control.

### Weather

- Click the weather icon to open the weather selector.
- **Lock Weather** stops the weather from cycling.
- Enter a **Weather ID** directly in the number box if you know it.

---

## World - Sky Editor

Controls the sky dome, stars, moon, clouds, and ambient scene lighting.

### Stars tab

| Control | Range | Description |
|---|---|---|
| Star Count | 0 to 20 | Number of stars |
| Star Intensity | 0 to 2.5 | Star brightness |
| Moon Color | colour picker | Moon tint |
| Moon Brightness | 0 to 1.0 | Moon brightness |
| Constellation Count | 0 to 10 | Number of constellation lines |
| Constellation Intensity | 0 to 2.5 | Constellation brightness |
| Galaxy Intensity | 0 to 10 | Milky Way band brightness |

### Sky tab

| Control | Description |
|---|---|
| Sky Texture | Click the preview to open the texture selector |
| Sky Texture ID | Enter a texture ID directly |
| Sun Visibility | How much the sun shows through fog |
| Ambient Temperature | -2.5 to 2.5: warm/cool colour shift |
| Ambient Saturation | 0 to 5: colour saturation of ambient light |
| Ambient Color | RGB picker for ambient light colour |
| Sunlight Color | Direct sunlight colour |
| Moonlight Color | Moonlight colour |

### Clouds tab

- Cloud texture and cloud side texture selectors.
- Cloud density and movement sliders (zone-dependent).

Each tab has a **Reset (↺)** button.

---

## World - Environment

Fine-grained control over environmental effects.

### Particles

| Control | Description |
|---|---|
| Particle Texture | Click the preview to browse textures |
| Particle Count | 0.0 to 1.0 |
| Particle Size | 0.0 to 20.0 |
| Particle Color | RGBA colour picker |
| Particle Glow | 0.0 to 10.0 |
| Particle Spread | 0.0 to 10.0 |
| Reset Particles (↺) | Revert to zone default |

### Rain / Wind / Fog

Sliders for rain intensity, wind direction and strength, and fog density / colour. Values are zone-dependent.

---

## World - Advanced

| Control | Description |
|---|---|
| Freeze Water | Stops water surface animation. Useful for still-water reflection shots. |

---

## World - Festivals

Festival flags activate seasonal decorations in the game world (Starlight Celebration, Heavensturn, etc.).

- The active list shows currently enabled flags.
- Enter a **Festival ID** or click **Search** to browse by name.
- **+** adds the selected festival; **-** removes it.
- **Reset (↺)** clears all overrides and restores zone defaults.

---

## Lighting

Place and edit up to four virtual lights in the scene.

### Light toolbar

- **+** adds a new light
- **Trash** removes the selected light

### Per-light controls

| Parameter | Description |
|---|---|
| Type | Point / Spot / Directional |
| Color | RGB colour picker |
| Intensity | Brightness multiplier |
| Range | How far the light reaches |
| Position | XYZ world-space position |
| Rotation | Direction the light faces |
| Falloff | How quickly brightness drops with distance |
| Shadow | Enable / disable shadow casting |

All parameters have **Reset** buttons. The light position also supports the world-space gizmo.

---

## Status Effects

Apply and remove visual status effect VFX on the selected actor.

- **+** opens the status effect search.
- **-** removes the selected effect.
- **Hide No-VFX** filters out effects with no visible particles or animation.
- Effects are listed by name and ID. Hover for details.

Status effects here are visual only and do not affect gameplay.

---

## Library

A file browser for poses, appearances, and character files stored on your disk. Open it from the library icon in the main toolbar or from the Appearance / Posing import dialogs.

### Browsing

- Files are grouped by type: **Poses**, **Characters**, **Scenes**, and custom sources.
- Use the **search box** to filter by filename or tag.
- Press **TAB** to filter by a suggested tag.
- Click a file to preview it. Double-click or press **Import** to apply it to the selected actor.

### Favourites

Click the star icon on any file to favourite it. Favourites appear at the top of the list.

### Import options

Click the **cog** icon next to Import to open per-section toggles (Body / Face / Hands / Weapons / Expression for poses).

### Adding sources

Go to **Settings → Library** to add custom folder paths. Click **Scan** to re-index after adding files.

---

## Projects

> **Beta:** projects saved in the current version may be incompatible with future versions.

Saves and restores a full scene: actor positions, appearances, and poses together.

### Saving a project

1. Open the Project window from the main toolbar.
2. Switch to the **Save** tab.
3. Enter a **Project Name** and **Description**.
4. Click **Save Project**.

### Loading a project

1. Open the Project window and switch to the **Load** tab.
2. Select a project. Name, description, and creation date are shown.
3. Click **Load** to restore the scene, or **Delete** to remove it permanently.

---

## MCDF Import

MCDF bundles a Penumbra mod with a Glamourer appearance profile. Requires both to be installed and active.

### Opening

Type `/mcdf` or use the **Load MCDF** button in the Appearance panel.

### Workflow

1. Click **Browse** and select a `.mcdf` file.
2. Choose which actor to apply it to.
3. Click **Load**. Penumbra applies the mod; Glamourer applies the appearance.
4. Use **Save MCDF** to export the current actor's appearance and active Penumbra mods as a new `.mcdf` file.

If you see a Glamourer error about an unknown actor, try re-loading the MCDF after the actor has fully initialised.

---

## Settings

Open via `/starfall settings`. Seven tabs:

### General

| Setting | Description |
|---|---|
| Use Library when importing | Opens the Library browser automatically on import |
| Open Library to last location | Remembers your last library folder |
| Use filename as Actor Name | Sets the actor's display name from the loaded filename |
| Censor Actor Names | Replaces real player names with pseudonyms |
| Hide Name in Group Pose Settings window | Hides character names in FFXIV's built-in GPose window |
| Enable StarfallStudio Color | Applies the plugin's custom UI colour theme |
| Enable StarfallStudio Scale | Applies custom UI scaling |
| Transform Slider Speed | Default speed for bone transform sliders |
| Open StarfallStudio behaviour | When the plugin window opens automatically |

### IPC

| Setting | Description |
|---|---|
| Enable StarfallStudio IPC | Enables the inter-plugin API |
| Enable StarfallStudio API | Enables the local HTTP API for external tools |
| Allow Penumbra Integration | Required for MCDF |
| Allow Glamourer Integration | Required for MCDF |
| Allow Customize+ Integration | Optional, for body scaling |

Each integration shows its current status and has a **Refresh** button.

### Posing

| Setting | Description |
|---|---|
| Show in GPose | Show the plugin window on GPose entry |
| Show in Cutscenes | Show the plugin window during cutscenes |
| Show when UI Hidden | Keep the plugin visible when FFXIV UI is hidden |
| Disable GPose Mouse Select | Stop FFXIV's click-to-target from stealing focus |
| StarfallStudio Target changes with GPose Target | Sync hierarchy selection to GPose camera target |
| GPose Target changes with StarfallStudio Target | Sync GPose camera target to hierarchy selection |
| Select Model Transform on Entity Select | Auto-select the root bone when you click an actor |

### Posing - Overlay

| Setting | Description |
|---|---|
| Overlay Defaults On | Bone overlay visible by default on GPose entry |
| Make Model Transform bone stand out | Highlights the root bone in the overlay |
| Allow Gizmo Axis Flip | Allows gizmo axes to flip when dragged past centre |
| Hide Gizmo while Advanced Posing | Hides the world gizmo when the bone editor is open |
| Hide Toolbar while Advanced Posing | Hides the posing toolbar when the bone editor is open |
| Show Skeleton Lines | Draws lines between bones |
| Show Bone Circles | Draws clickable circles on bones |
| Draw skeleton line to edge of bone circle | Extends skeleton lines to the circle edge |
| Hide Skeleton when Gizmo Active | Hides the skeleton overlay while dragging the gizmo |
| Overlay Colors | Per-category colour pickers for the bone overlay |

### Library

Add or remove disk paths that the Library browser scans for pose and character files.

### Auto-Save

| Setting | Description |
|---|---|
| Auto-Save Enabled | Periodically saves the current scene |
| Save Individual Poses | Also saves each actor's pose as a separate `.pose` file |

### Input

Assign keyboard shortcuts to posing actions (toggle link mode, select all bones, etc.).

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
| Customize+ | Optional - MCDF body scaling |

Core features (posing, animation, camera, world, overworld actors) work without any additional plugins.

---

## Tips & Caveats

- **Open-world vs instanced GPose:** the plugin detects the mode automatically. Overworld actor importing only works in open-world GPose (cities, housing, field areas).
- **Modded appearances:** the plugin does not trigger the appearance system for overworld actors, so any modded gear or visuals active on those actors are left untouched.
- **Pose files** use Brio's `.pose` format. Files from Brio are fully compatible.
- **Companion slots:** to attach a mount, minion, or ornament to a spawned actor, use **Spawn with Companion**. Plain **Spawn** does not reserve a companion slot.
- **Props** share the same actor slots as characters. Appearance editing on props is limited.
- **Undo** is per-bone. There is no global scene undo - be careful with Destroy All.
- Plugin updates install automatically via Dalamud.
