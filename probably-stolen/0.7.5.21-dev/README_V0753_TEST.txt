Probably Stolen Korean Pack v0.7.5.3-dev
Display-boundary refinement test

Baseline:
- v0.7.5 architecture retained.
- corrected supplemental CSV loader format retained.
- Night Auction success path retained.

New changes:
1. Saved mod item display-name bridge
   - Patches GameItem.GetDisplayName only.
   - Uses the game's own item-owner lookup to select the target mod catalog.
   - Does not modify identifiers, save data, or player custom names.
   - Intended to cover saved titles such as 触手本体 and 游戏卡棋盘.

2. GameCardPK customer type
   - Internal type value 游戏卡 remains unchanged.
   - Only TypeHelper.GetTypeDisplayName("游戏卡") output is localized.
   - Previous tuple/list mutation route is no longer registered.

3. ItemStorage notifications
   - BoxItem.NotifyUser(message) display argument only.
   - Adds auto-stocking lock/unlock notification templates.

4. MidnightPawn
   - Adds complete known faction/job/body/breast display vocabulary.
   - Adds missing dynamic maiden description and tooltip source mappings.
   - Existing final-tooltip bridge remains the render boundary.

5. Brewing Expansion / Nini
   - Adds native K|be_nini_* localization keys used by ModText.Get.
   - No new Brewing runtime hook is required for phone dialogue.

6. CorpPresence
   - Uses the existing GUI.Label wrapper for DrawSolana* methods.
   - Adds line-by-line fallback for mixed Korean + dynamic English labels.
   - Adds Pastel rebate and Solana dynamic text mappings.
   - Does not patch SilverBlood.AddLiquid or other known-dangerous Corp paths.

Validation:
- Build succeeded with 0 warnings / 0 errors.
- Probably Stolen ModScanner: HARD_COUNT=0 / SCANNER_EXIT=0.
- Supplemental CSV parser validation: skipped=0 / quoted-key=0.
- No third-party mod files are included or modified.
