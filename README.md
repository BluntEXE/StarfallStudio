# ★ Starfall Studio

A combined GPose toolkit for FFXIV, merging the best of [Brio](https://github.com/Etheirys/Brio) and [Ktisis](https://github.com/ktisis-tools/Ktisis) into a single cohesive plugin designed for venue photography and scene building.

![Starfall Studio](StarfallStudio/Resources/Images/StarfallStudioIcon.png)

---

## Features

### Posing
- Bone tree list (hierarchical, Ktisis-style) with ancestor highlighting
- Bone category filters
- Pose import / export
- Mirror pose
- Freeze animation
- Undo / redo per bone

### Camera
- Full camera editor — angle, pan, rotation, FoV, distance
- Delimit camera (remove zoom limits)
- Freecam with WASD movement + mouse look
- Multiple cameras

### Actors
- Actor list with click-to-select
- Spawn additional actors
- Clone actors
- Hide / show actors
- Model transform gizmo (world-space repositioning)
- **NPC / overworld actor import** — add real NPCs and other players to your scene in open-world GPose; ambient actors are hidden automatically, selected actors teleport to your position

### Appearance
- Import & Export with per-section checkboxes (Customize / Gear / Weapons)
- Import NPC appearance
- MCDF import (requires Penumbra + Glamourer)
- Wetness / Wetness Depth editor

### Animation
- Animation control with timeline
- Freeze physics
- Speed multiplier
- Animation search and blend

### World
- Weather control
- Time of day
- Lighting editor
- Festival flags

---

## Installation

> **This is a custom plugin — it is not in the official Dalamud plugin list.**

1. Open **Dalamud Settings** → **Experimental** tab
2. Under **Custom Plugin Repositories**, add:
   ```
   https://raw.githubusercontent.com/BluntEXE/StarfallStudio/main/repo.json
   ```
3. Click **Save & Close**
4. Open `/xlplugins` → search for **Starfall Studio** → Install

### Requirements
- [Penumbra](https://github.com/xivdev/Penumbra) — required for MCDF
- [Glamourer](https://github.com/Ottermandias/Glamourer) — required for MCDF appearance application
- Customize+ — optional, used for MCDF body scaling

---

## Commands

| Command | Action |
|---|---|
| `/starfall` | Toggle the main Starfall Studio window |
| `/starfall window` | Open the main window |
| `/starfall settings` | Open settings |
| `/mcdf` | Toggle the MCDF window |

---

## Notes

### MCDF
MCDF import requires Penumbra and Glamourer to be installed and active. The plugin will show a warning if either is unavailable.

### Pose Import
Pose files use Brio's `.pose` format (JSON). Files from Brio are fully compatible.

---

## Credits

Starfall Studio is built on top of the excellent work by:

- **[Brio](https://github.com/Etheirys/Brio)** by Minmoose, Asgard and Contributors — the foundation of this plugin
- **[Ktisis](https://github.com/ktisis-tools/Ktisis)** by the Ktisis team — camera editor workflow and bone tree inspiration

Please support the original projects if you use them.

---

## License

This project inherits the license terms of Brio. See [LICENSE](LICENSE) for details.
