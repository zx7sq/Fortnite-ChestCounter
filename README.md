<div align="center">

<img src="src/ChestCounter/Assets/ChestIcon.png" alt="Fortnite Chest Counter" width="96">

# Fortnite Chest Counter

[![Latest Release](https://img.shields.io/github/v/release/zx7sq/Fortnite-ChestCounter?style=flat-square&color=f59e0b&label=latest)](https://github.com/zx7sq/Fortnite-ChestCounter/releases/latest)
[![Downloads](https://img.shields.io/github/downloads/zx7sq/Fortnite-ChestCounter/total?style=flat-square&color=30d158)](https://github.com/zx7sq/Fortnite-ChestCounter/releases)
![Windows 10 | 11](https://img.shields.io/badge/Windows-10%20%7C%2011-3a7bd5?style=flat-square)
![EAC Safe](https://img.shields.io/badge/Easy%20Anti--Cheat-100%25%20Safe-30d158?style=flat-square)
[![License](https://img.shields.io/github/license/zx7sq/Fortnite-ChestCounter?style=flat-square&label=license&color=555)](LICENSE)

**The all-in-one Fortnite Treasure Hunters companion: automatic chest counter, seamless minimap HUD, animated Best Chest Route maps & GIF exports, automatic 59-variant Sprite tracker, zero-setup Squad Sync, and live Stats.**

[⬇️ **Download Latest `ChestCounter.exe`**](https://github.com/zx7sq/Fortnite-ChestCounter/releases/latest)

</div>

---

## Why use this fork?

The original Chest Counter counts your chests and writes an HTML report. **This fork turns it into a complete Treasure Hunters grind suite** — faster, cleaner, packed with features you won't find in the original, and still a single standalone `.exe` with zero dependencies.

| Feature | Original | **This Fork (`zx7sq`)** |
|---|:---:|:---:|
| Automatic chest counting & HTML report | ✅ | ✅ |
| **Best Chest Route Map + Animated GIF Export** | ❌ | ✅ Live Chapter 7 map, Top 10 games, numbered chests, **Export GIF** |
| **Automatic Sprites & Variants Tracker (59/59)** | ❌ | ✅ Auto-scans carried, Lv 5, extracted & won Sprites + all 59 images embedded |
| **Zero-Setup Squad Sync** | ❌ *(150 m replay cap)* | ✅ Detects squadmates running the app & syncs their **exact** chests live |
| **Live Fortnite Stats Card (`Stats` tab)** | ❌ | ✅ Season/Lifetime, Mode & Input filters, BP level, Outfit showcase |
| **Upgraded Minimap HUD Overlay** | Basic box | ✅ Borderless HUD, `hellafine.ttf`, real chest icon, 60 fps spin-up & **auto-lock in-game** |
| **Daily & Weekly Chest Averages** | ❌ | ✅ `chests / day` + `chests / week` (at 7+ days) on Home & Report |
| **Treasure Hours Schedule & Reminders** | Report banner only | ✅ Home countdown card in your PC's local timezone/language + 5-min & boot alerts |
| **Discord Rich Presence** | ❌ | ✅ Live session chest count on your Discord profile |
| **Zap Mode (Replay Auto-Cleanup)** | ❌ | ✅ Auto-deletes counted `UnsavedReplay` files to save GBs of disk space |
| **Collapsed-House / Gum-Gum Fruit Chest Fix** | ❌ | ✅ Counts upper-floor chests broken by collapsing walls/builds |
| **Performance & Bloat Trimmed** | — | ✅ O(1) loot indexing, cached HUD brushes, batched saves, English-only |

---

## Feature Highlights

### 🗺️ Best Chest Route Map & Animated GIF Export
- Automatically finds your **#1 highest chests-per-match game** (plus a selector for your **Top 10**) and draws your exact landing-to-end route on the live Chapter 7 island map.
- Plots your **Landing**, **Match End**, directional arrows, and every **numbered chest** opened along the way — hover any chest dot to see its type and timestamp.
- Switch between **Island** and **Zoom Route** views, toggle **POIs** and **Chests**, watch it live with **▶ Replay**, or click **Export GIF** to save a looping animated `.gif` (with a live counting chest badge) straight to your `Downloads` folder ready for Discord or X.

### ✨ Automatic Sprites & Variants Tracker (All 59 Artwork Images Embedded)
- Reads Fortnite's `BP_ExtractionBackpack_Sprite` replay data automatically to track every Sprite and variant (**Base**, **Fortune**, **Cursed**, **Gold**) you carry, level to **Max (Lv 5)**, **extract** mid-match, or **secure from a Victory Royale (`#1`)**.
- Includes all **17 Sprite families** and all **59 official variant images** embedded right inside `ChestCounter.exe` for instant offline loading, plus click-to-toggle for Sprites you collected before installing the app.

### 🤝 Zero-Setup Squad Sync
- Fortnite replays only record teammates within ~150 m of you. With **Squad Sync**, whenever teammates in your same squad are also running this app, it automatically detects them using a one-way SHA-256 hash of your match session + squad — **no accounts, no friend codes, no setup**.
- Counts your synced squadmates' exact chests in a separate **Squad Sync** counter on Home (with live mid-match updates and per-teammate breakdown), leaving your personal total untouched. Toggleable anytime in **Settings**.

### 📊 Live Fortnite Stats Card
- Built-in **Stats** tab powered by `fortnite-api.com` showing Wins, Win Rate, Matches, K/D, Kills/Match, Kills, Play Time, Avg. Match Time, Battle Pass Level & progress, Top placements, Outlived, Score, and Kills/Min.
- Filter by **Season / Lifetime**, **Overall / Solo / Duo / Squad / LTM**, and **All / KBM / Pad**, customize the featured **Outfit**, and keep **Live Watch** on for automatic updates after every match.

### 🎯 Seamless In-Game HUD & Quality of Life
- **Borderless HUD Overlay** tucked cleanly under your minimap with anti-aliased `hellafine.ttf`, real chest icon, smooth 60 fps spin-up numbers, and **auto-lock during live matches** so you never accidentally drag or click it mid-fight.
- **Treasure Hours Schedule & Alerts** displayed in your PC's local timezone and Windows language, with 5-minute warnings, live banners, and Fortnite-boot event reminders.
- **Discord Rich Presence** to flex your grind live on Discord, **Zap Mode** to auto-delete counted `UnsavedReplay` files, and **Week 4 Treasures** with official artwork.

---

## Quick Start

1. **Download [`ChestCounter.exe`](https://github.com/zx7sq/Fortnite-ChestCounter/releases/latest)** — standalone single-file `.exe`, nothing to install.
2. **Enable Replays in Fortnite:** *Settings → Game → Replays → Record Replays: **On*** (required so the app has matches to read).
3. **Use Windowed Fullscreen** in Fortnite (*Settings → Video → Window Mode*) so the HUD overlay renders on top of the game.
4. **Run `ChestCounter.exe`** *(if Windows SmartScreen appears because the exe is unsigned, click **More info → Run anyway**)*.
5. **Play!** Your chests, Sprites, best routes, and synced squad counts update automatically a few seconds after each match.

---

## Overlay Shortcuts

| Shortcut | Action |
|---|---|
| **`Ctrl + Shift + O`** | Show or hide the in-game overlay |
| **`Ctrl + Shift + L`** | Lock or unlock the overlay to reposition it |

---

## Safety & Transparency

- **100% Easy Anti-Cheat Safe:** Only reads Fortnite's saved `.replay` files (`%LOCALAPPDATA%\FortniteGame\Saved\Demos`) in read-only mode. Never reads or modifies Fortnite's memory, process, or game files. No keyboard/mouse hooks, no DLL injection, and no Epic login required.
- **Open Source & Clean Code:** Every line of code in this repository is open and readable. The only encoded values are the built-in Discord Application ID and free `fortnite-api.com` key in [`AppSecrets.cs`](src/ChestCounter/AppSecrets.cs), lightly obfuscated strictly to stop automated bots/scrapers from harvesting and abusing the free API quota — nothing else is obfuscated.

---

## Build from Source

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download) on Windows:

```powershell
dotnet publish src/ChestCounter/ChestCounter.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=none -o dist
```

---

<div align="center">

<sub>Forked from <a href="https://github.com/LED-N/Fortnite-ChestCounter">LED-N/Fortnite-ChestCounter</a> — rebuilt and expanded for the Chapter 7 Treasure Hunters grind.</sub>

</div>
