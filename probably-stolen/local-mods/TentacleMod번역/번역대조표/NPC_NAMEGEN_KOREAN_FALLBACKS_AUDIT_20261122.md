# Tentacle NameGen UI fallback and diagnostics (2026-11-22)

- Only String ldstr in class NameGen and named display/log methods were changed.
- Region filter strings 中国/日本/韩国 and file paths remain exact originals.
- Existing Hangul IsCjk check preserved.

- MidnightPawn.dll / RandomNameParts / `이름` → `명`
- MidnightPawn.dll: 1 newly translated IL strings; region constant occurrences 0 preserved in source before writing
- ShadowTrade.dll: 0 newly translated IL strings; region constant occurrences 3 preserved in source before writing
