# lewd_meme_remake Korean DLL maintenance

Hard-coded-string Korean localization maintenance data for `lewd_meme_remake` / `MidnightPawnRemake.dll`.

## Translation memory

- Previous reviewed memory: 874 source → Korean pairs
- 1.4.1 newly reviewed: 77 pairs
- 1.5.0 newly reviewed: 84 pairs
- Current total: **1035 pairs**
- Canonical file: `translation_memory.csv`

Do not use IL offsets or metadata token numbers as long-term translation keys. Recompiles can move them. Exact source strings are the reuse key.

## 1.4.1 audit

- Upstream archive: `lewd_meme_remake1.4.1.zip`
- Included manifest version: `1.4.0` (upstream package mismatch; left unchanged)
- Source DLL SHA-256: `8728138396761d7014dd64b9556f23760a7033de8e63eae853a8d92ddafc7246`
- Source CLR #US entries: 1143
- Chinese-containing source strings: 816
- Reused from prior memory: 736
- New/changed reviewed strings: 80
  - translated: 77
  - intentionally untranslated internal identifiers: 3
- Internal identifiers kept verbatim: `中国`, `日本`, `韩国`
- Patched DLL SHA-256: `6820dda8cfbd239bca7278591f5e9e0814b8bacee4b2d1ea1585d89f01c053ca`
- Patched DLL size: 273920 bytes
- Static validation: remaining Chinese ldstr references = exactly the 3 internal region identifiers above.

## 1.5.0 audit

- Upstream archive: `lewd_meme_remake1.5.0.zip`
- Included manifest version: `1.5.0`
- Source DLL SHA-256: `a3c1da513dce704645bdad50d9112991f4f48aefec1448f755390c76406b4552`
- Source CLR #US entries: 1195
- Chinese-containing source strings: 845
- Reused from existing translation memory: 758
- New/changed reviewed strings: 87
  - translated: 84
  - intentionally untranslated internal identifiers: 3
- Internal identifiers kept verbatim: `中国`, `日本`, `韩国`
- Patched DLL SHA-256: `03bec3ee81e9e452be6b7bb33342b566a27f1cfeb6818e2f268b50fbc0f4b63e`
- Patched DLL size: 298496 bytes
- Placeholder mismatches: 0
- Static validation: remaining Chinese CLR user strings = exactly the 3 internal region identifiers above.
- Method-body validation: all changed method bytes are confined to `ldstr` token operands; non-string logic changes = 0.

## Files

- `translation_memory.csv` — canonical reusable source → Korean translation memory.
- `reviewed_1.4.1.csv` — all 80 changed/new strings and review status.
- `reviewed_1.5.0.csv` — all 87 changed/new strings and review status.
- `skip_internal.csv` — strings that must remain untranslated because they are runtime identifiers.
- `translation_memory_tool.py` — update/diff helper.
- `patch_dll.py` — dependency-free DLL #US rebuild/patch helper.

## Future update flow

1. Extract the new upstream `MidnightPawnRemake.dll`.
2. Compare its source strings against `translation_memory.csv` and `skip_internal.csv`.
3. Translate only genuinely new/changed player-facing strings.
4. Append reviewed pairs to `translation_memory.csv`.
5. Patch the new upstream DLL with `patch_dll.py`.
6. Verify placeholders and ensure any remaining Chinese references are only approved internal identifiers.
