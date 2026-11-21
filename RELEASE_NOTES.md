Download **`ChestCounter.exe`** below and run it — nothing to install and no extra dependencies needed.

> **Turn on replays in Fortnite first:** *Settings → Game → Replays → Record Replays: **On***. The app only reads your saved replay files in read-only mode — 100% Easy Anti-Cheat safe.

**What's new in 3.1.0**
- **Squad Sync (Separate Squad Chest Counter):**
  - Automatically detects teammates in your same Fortnite squad who are also running ChestCounter (zero setup, keyed by a one-way SHA-256 hash of your match session + squad).
  - Counts your synced squadmates' exact opened chests in a dedicated **Squad Sync** tile on Home (with live teammate breakdown during active matches and combined squad total).
  - Can be toggled on/off anytime in **Settings → Squad Sync**.

**What's new in 3.0.1**
- **Localized Treasure Hours:** Treasure Hours on the Home card, alerts, and Report now automatically match your PC's local timezone (with timezone name & UTC offset shown) and your Windows language/regional date & 12h/24h clock format.

**What's new in 3.0.0**
- **Live Fortnite Stats Tab:**
  - Built-in player stats card showing Wins, Win Rate, Matches, K/D, Kills/Match, Kills, Play Time, Avg. Match Time, Battle Pass Level & progress, Top placements, Outlived, Score, and Kills/Min.
  - Filter by **Season / Lifetime**, **Overall / Solo / Duo / Squad / LTM**, and **All / KBM / Pad**.
  - Featured **Outfit showcase** with quick presets (Midas, Marigold, Peely, Aura, Renegade Raider) or any custom Fortnite outfit search.
  - **Live Watch** auto-refreshes stats every 30s and whenever a match finishes, with smooth number roll-in animations.
- **Daily & Weekly Chest Averages:**
  - Added **chests / day** on the Home tab and Report, plus **chests / week** automatically once you have 7+ days of data.
- **Transparency Note (`AppSecrets.cs`):**
  - Only the built-in Discord Rich Presence App ID and Fortnite-API.com key are lightly obfuscated (XOR + bit rotation) strictly to stop automated bots/scrapers from harvesting and abusing the free API quota. Everything else in the app is completely open and readable source code — nothing malicious.

*Windows may show "Windows protected your PC" because the exe is unsigned — click **More info → Run anyway**.*
