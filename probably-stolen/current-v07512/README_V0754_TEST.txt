Probably Stolen Korean Pack v0.7.5.4-dev
Remaining-output refinement test

Confirmed baseline retained:
- v0.7.5 architecture
- corrected supplemental CSV loader format
- Night Auction working translation path
- existing stable Corp adapter restrictions

New targeted fixes:
1. ItemStorage
   - Adds remaining notification source mappings.
   - Keeps existing Korean storage window/button translations.
   - Box floor-label fix now also replaces the Chinese-only TMP font with a
     Hangul-capable font already used by the running game UI.

2. MidnightPawn / Tentacle
   - Adds dedicated daily/night report handling for:
     夜间报告 / dynamic body / 知道了
   - Report parser translates dynamic captured maiden types and products from
     the KoreanPack catalog; Korean sentence templates remain in CSV.

3. GameCardPK
   - CardNPC.BuildClient return object is translated only as display/dialogue data.
   - Functional buying tags, budgets, identifiers and internal type 游戏卡 remain unchanged.
   - ClienteleUIManager final rendered text gets display-only 游戏卡 -> 게임 카드.
   - Expanded customer names/dialogues from the prepared Korean supplemental catalog.

4. CorpPresence
   - DrawSpacerMutual added only to the existing safe GUI.Label wrapper.
   - CompanyVisitors.Courier return client/dialogue translated as display data.
   - RecorderUIManager final visible history text is translated without modifying
     stored identifiers or Corp mechanics.
   - Mutual-aid and courier dialogue mappings added.

5. Shared client display
   - DialogUIManager.DisplayClientText is translated at the final display/history
     boundary only for known target clients.

Validation:
- .NET build succeeded.
- Probably Stolen ModScanner: HARD_COUNT=0 / SCANNER_EXIT=0.
- Supplemental parser validation: skipped=0 / quoted-key=0 for all six files.
- ZIP integrity passed.
- No third-party mod files are included or modified.

Runtime verification is still required.
