# lewd_meme Korean name-pool / identity policy

This file exists because the 1.3.8 first-pass update accidentally treated `Content/names.txt` as an internal data file and copied the Chinese upstream pool back into the Korean local build.

## Canonical rule

`Content/names.txt` is player-visible data and **must be carried forward from the Korean build**, then reconciled with any upstream structural changes.

- Preserve each region key in column 1 because code uses it as an identifier.
- Koreanize the surname and given-name payloads in columns 2 and 3.
- The 1.3.8 r2 validated pool has 13 region rows.
- CJK characters are allowed in the region-key column only; surname/given-name payloads must contain no CJK characters.
- Do not reintroduce the middle dot `·` between generated surname and given name.
- Current validated Korean pool SHA-256: `27ca53b5556e7c57c3d924231a7e88284a97c7749df4c60fc30e0c3856130faf`.

GitHub historical baseline:
`probably-stolen/localized/MidnightPawn-1.2.0-KO/Mod/Content/names.txt`

The later 2026-11-07 name/separator fix is represented by the library artifact `lewd_meme1.2.0-KO-name-separator-fix.zip`; when available, prefer its curated pool over rebuilding names from scratch.

## DLL rules coupled to the name pool

- `ClientPool.Load` / `ClientPool.Save`: cache prefix `poolL`.
- `NameGen.RandomName`: separator must be a normal ASCII space.
- The cache-prefix bump is deliberate: it invalidates previously cached Chinese/broken customer names once.

## Faction display/value rules

The prior completed Korean DLL translated the full faction values consistently, and r2 follows the same rule:

- `下层区` → `하층구역`
- `上层区` → `상층구역`
- `治安部` → `치안부`
- `黑市` → `암시장`
- `革命军` → `혁명군`

All references to these full values inside the mod must move together. Do not translate shorter external/native matching fragments merely because they contain Chinese.
