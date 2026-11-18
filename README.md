<div align="center">

<img src="src/ChestCounter/Assets/ChestIcon.png" alt="Chest Icon" width="96">

# Fortnite Chest Counter

![Windows 10 | 11](https://img.shields.io/badge/Windows-10%20%7C%2011-3a7bd5?style=flat-square) [![license](https://img.shields.io/github/license/LED-N/Fortnite-ChestCounter?style=flat-square&label=license&color=555)](LICENSE)

**Automatically counts every chest you open in Fortnite and shows a clean overlay right under your minimap.**

</div>

---

## What it does

- **Tracks every chest automatically** after each match finishes (normal chests, rare chests, buried chests, and collapsed-house chests).
- **Shows a seamless in-game overlay** under your minimap with your total chests, last match, session count, and treasures.
- **Keeps your full history & stats** in a sleek dark-mode app and HTML report.

> It only counts chests **you** open or break — it does not show chest locations on the map.

## What's new in this fork

- **Seamless HUD Overlay:** Sits cleanly under the minimap with no ugly box, anti-aliased custom font (`hellafine.ttf`), custom chest icon, and numbers that smoothly spin up when updated.
- **Auto-Locks During Matches:** Locks automatically while you are in a game so you can never accidentally drag or click it in a fight.
- **Gum-Gum Fruit / Collapsed-House Fix:** Upper-floor chests broken by collapsing walls/houses now count properly toward your total.
- **Sleek Dark UI:** Apple-inspired black/dark-grey frosted interface with smooth animations.
- **Zero Dependencies:** Built as a standalone single-file `ChestCounter.exe` (no extra `.NET` install needed).

## Quick start

1. **Turn on Replays in Fortnite:** *Settings → Game → Replays → Record Replays: **On*** (required so the app has matches to read).
2. **Use Windowed Fullscreen** in Fortnite (*Settings → Video → Window Mode*) so the overlay can appear on top of the game.
3. **Run `ChestCounter.exe`** (if Windows SmartScreen pops up, click *More info → Run anyway*).
4. **Play!** Your chest count updates automatically a few seconds after each match ends.

## Overlay shortcuts

| Shortcut | Action |
|---|---|
| **Ctrl + Shift + O** | Show or hide the overlay |
| **Ctrl + Shift + L** | Lock or unlock the overlay to move it |

## Is it safe? (Anti-Cheat)

**Yes — 100% Easy Anti-Cheat safe:**
- Only reads Fortnite's saved `.replay` files (`%LOCALAPPDATA%\FortniteGame\Saved\Demos`) in read-only mode.
- Never opens, reads, or modifies Fortnite's memory, process, or game files.
- No keyboard/mouse hooks, no DLL injection, and no account login required.

## Build from source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) on Windows:

```powershell
dotnet publish src/ChestCounter/ChestCounter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o dist
```
