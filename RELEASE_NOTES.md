Download **`ChestCounter.exe`** below and run it — nothing to install and no extra dependencies needed.

> **Turn on replays in Fortnite first:** *Settings → Game → Replays → Record Replays: **On***. The app only reads your saved replay files in read-only mode — 100% Easy Anti-Cheat safe.

**What's new in 2.0.2**
- **Performance Optimizations:**
  - Cached HUD overlay fonts, brushes, and text formats so 60 fps spin-up animations have minimal overhead.
  - Pre-indexed replay loot lookups (`O(1)` instead of linear scans) for faster post-match replay analysis.
  - Batched `data.json` disk writes during multi-replay scans.
  - Cached Fortnite process checks and paused UI background timers/polling while minimized or hidden.
- **Trimmed & English-Only:** Removed unused extra languages, legacy screenshots, and unused embedded fonts for a cleaner, smaller repo and app.

*Windows may show "Windows protected your PC" because the exe is unsigned — click **More info → Run anyway**.*
