# Tentacle NPC Korean name formatter static patch (2026-11-22)

- Source Content/names.txt retains exact Chinese region identifiers required by the mods.
- Display names in the three selected regions use Korean rather than Han characters.
- Original NameGen.IsCjk compares Unicode U+4E00..U+9FFF. This patch only changes its upper bound to U+D7A3, including Hangul and avoiding inserted middle dots between Korean family and given names.
- This does not affect Roman-alphabet foreign name separation, default fallbacks or storage keys.

- `ShadowTrade.dll`: 0 matched NameGen.IsCjk upper bound; no matching original method, binary unchanged.
- `MidnightPawn.dll`: 0 matched NameGen.IsCjk upper bound; no matching original method, binary unchanged.
- The precise original method and constants are verified with dnlib; no other intentional IL edits.
- Runtime execution is not assessed here.
