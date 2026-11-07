# lewd_meme_remake Korean DLL maintenance

This directory stores translation memory for the hard-coded-string DLL localization of `lewd_meme_remake` / `MidnightPawnRemake.dll`.

## Baseline

- Previous source package: `lewd_meme_remake1.3.1.zip`
- Previous source DLL SHA-256: `c940622cecd975b245be75fdf575492ad33aa91e752716f0560714ac94f2a16c`
- Previous Korean DLL SHA-256: `02dbd7a521f5512264181ffc2a3611b47d7fe16a6f6dd6ba85d030a9a4952482`
- Baseline CLR `#US` entries: 1238
- Validated Chinese → Korean replacements: 874

The baseline original and Korean DLL have exactly the same `1238` CLR user-string slots, so the 874 changed pairs can be recovered deterministically by index.

## Current update audit

- Uploaded package filename: `lewd_meme_remake1.4.1.zip`
- Package manifest `<ModVersion>`: `1.4.0`
- New source DLL SHA-256: `8728138396761d7014dd64b9556f23760a7033de8e63eae853a8d92ddafc7246`
- New CLR `#US` entries: 1143
- Chinese-containing entries: 816
- Exact translations reused from baseline memory: 736
- New/changed Chinese entries requiring review: 80

Note: the archive filename says `1.4.1`, while its included manifest currently declares `1.4.0`.

## Files

- `translation_memory.csv` — exact source → Korean pairs recovered from the previous original/Korean DLL pair.
- `pending_1.4.1.csv` — only new or changed Chinese strings not found in the baseline translation memory.
- `translation_memory_tool.py` — dependency-free extractor/diff tool for future updates.

## Future workflow

1. Keep `translation_memory.csv` as the canonical translation memory.
2. Extract the new upstream `MidnightPawnRemake.dll`.
3. Match exact source strings against the translation memory.
4. Translate only rows emitted as new/changed.
5. Append reviewed new pairs to the translation memory.
6. Rebuild the Korean DLL from the new upstream DLL using the complete translation memory.
7. Validate format placeholders and remaining player-facing Chinese strings.

Do not use IL offsets or metadata token numbers as the long-term translation key. Upstream recompiles can move them. The exact source string is the stable reuse key; method/offset data may be used only as an audit aid.