# Tentacle NameGen UI fallback and diagnostics (2026-11-22)

- Only String ldstr in class NameGen and named display/log methods were changed.
- Region filter strings 中国/日本/韩国 and file paths remain exact originals.
- Existing Hangul IsCjk check preserved.

- MidnightPawn.dll / RandomNameParts / `无` → `무`
- MidnightPawn.dll / RandomName / `无名` → `무명`
- MidnightPawn.dll / RandomName / `无名` → `무명`
- MidnightPawn.dll: 3 newly translated IL strings; region constant occurrences 0 preserved in source before writing
- ShadowTrade.dll / RandomName / `无名` → `무명`
- ShadowTrade.dll / RandomName / `无名` → `무명`
- ShadowTrade.dll: 2 newly translated IL strings; region constant occurrences 3 preserved in source before writing
