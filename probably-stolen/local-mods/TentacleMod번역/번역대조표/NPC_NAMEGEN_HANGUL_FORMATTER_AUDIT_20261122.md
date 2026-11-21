# Tentacle NPC Korean name formatter static patch (2026-11-22)

- Source Content/names.txt retains exact Chinese region identifiers required by the mods.
- Display names in the all 13 regions use Korean rather than Han characters.
- Original NameGen.IsCjk compares Unicode U+4E00..U+9FFF. This patch only changes its upper bound to U+D7A3, including Hangul and avoiding inserted middle dots between Korean family and given names.
- This does not affect Roman-alphabet foreign name separation, default fallbacks or storage keys.

- `ShadowTrade.dll`: unchanged; NameGen.IsCjk upper bound U+D7A3 (Hangul supported).
- `ShadowTrade.dll` NameGen methods: EnsureLoaded, RandomName, IsCjk, .cctor
  - EnsureLoaded: lewd_meme_remake, Content/names.txt, 이름 목록: , 개 지역(중국·일본·한국만), 이름 목록 불러오기 실패: 
  - RandomName: 无名, 无名, ·, 
- `ShadowTrade.dll` NameGen fixed-region string literals: 中国, 日本, 韩国
- `MidnightPawn.dll`: unchanged; NameGen.IsCjk upper bound U+D7A3 (Hangul supported).
- `MidnightPawn.dll` NameGen methods: EnsureLoaded, RandomNameParts, RandomName, SurnameOf, GivenNameOf, FromSurname, IsCjk
  - EnsureLoaded: lewd_meme, Content/names.txt, 이름 목록: , 개 지역, 이름 목록 불러오기 실패: 
  - RandomNameParts: 无, 이름, ·, 
  - RandomName: 无名, 无名, ·, 
  - GivenNameOf: 
  - FromSurname: ·, 
- The precise original method and constants are verified with dnlib; no other intentional IL edits.
- Runtime execution is not assessed here.
