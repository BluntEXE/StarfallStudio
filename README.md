# Starfall Studio

A GPose toolkit for FFXIV combining [Brio](https://github.com/Etheirys/Brio) and [Ktisis](https://github.com/ktisis-tools/Ktisis) into a single plugin built for venue photography and scene work.

![Starfall Studio](Resources/Images/StarfallStudioIcon.png)

---

## Features

### Posing
- Bone tree with ancestor highlighting
- Bone category filters
- Pose import / export
- Mirror pose
- Freeze animation
- Undo / redo per bone

### Camera
- Camera editor: angle, pan, rotation, FoV, distance
- Delimit camera (removes zoom limits)
- Freecam with WASD + mouse look
- Multiple cameras

### Actors
- Actor list with click-to-select
- Spawn, clone, and destroy actors
- Hide / show actors
- World-space transform gizmo
- **NPC / overworld actor import**: add real NPCs and other players to your scene in open-world GPose. Ambient actors are hidden automatically; selected actors teleport to your position.

### Appearance
- Import / export with per-section toggles (Customize / Gear / Weapons)
- Import NPC appearance
- MCDF import (requires Penumbra + Glamourer)
- Wetness / Wetness Depth editor

### Animation
- Animation control with timeline scrub
- Freeze physics
- Speed multiplier
- Animation search and blend

### World
- Weather and time of day control
- Sky, cloud, and star editor
- Lighting editor
- Festival flags
- Environment effects (particles, fog, wind, rain)

---

## Documentation

**[Full User Guide](GUIDE.md)** covers every panel and feature with a quick-navigation index.

---

## Installation

> **Custom plugin - not in the official Dalamud list.**

1. Open **Dalamud Settings** → **Experimental** tab
2. Under **Custom Plugin Repositories**, add:
   ```
   https://raw.githubusercontent.com/BluntEXE/StarfallStudio/main/repo.json
   ```
3. Click **Save & Close**
4. Open `/xlplugins`, search for **Starfall Studio**, and install

### Requirements
- [Penumbra](https://github.com/xivdev/Penumbra) - required for MCDF
- [Glamourer](https://github.com/Ottermandias/Glamourer) - required for MCDF appearance application
- Customize+ - optional, used for MCDF body scaling

---

## Commands

| Command | Action |
|---|---|
| `/starfall` | Toggle the main window |
| `/starfall window` | Open the main window |
| `/starfall settings` | Open settings |
| `/mcdf` | Toggle the MCDF window |

---

## Notes

### MCDF
Requires Penumbra and Glamourer to be installed and active. The plugin shows a warning if either is missing.

### Pose Import
Pose files use Brio's `.pose` format. Files from Brio are fully compatible.

---

## Credits

Built on top of:

- **[Brio](https://github.com/Etheirys/Brio)** by Minmoose, Asgard and Contributors
- **[Ktisis](https://github.com/ktisis-tools/Ktisis)** by the Ktisis team

Please support the original projects.

---

## License

Inherits the license terms of Brio. See [LICENSE](LICENSE) for details.
