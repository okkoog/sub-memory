# Brood five stage self-test labels — Korean patch and IL validation

- Target: `lewdMemeBrood v0331/LewdMemeBrood.dll`
- Verified callsites: `BroodMainCycle.TentacleChild.TentacleUse/<>c::<RegisterTests>b__8_0` calling `BroodMainCycle.Check::That(Boolean,String)`.
- Stage labels 5/5 replaced only as the reporting argument; no `MAIDEN_BODY` comparison values were changed.
- Input SHA256: `c30d1ef003b5f5c5baeaef9fa7031f19aff295f770e8ffce5492e199facf72f7`
- Output SHA256: `70b85c118c739c69d7c4307a171cf20f9e64beaa37c1bb48e5642f9a73d8fae8`

| Chinese label | Korean label | IL callsite instances |
|---|---|---:|
| 萝莉 → 少女 | 로리 → 소녀 | 1 |
| 少女 → 御姐 | 소녀 → 누님 | 1 |
| 御姐 → 少妇 | 누님 → 새댁 | 1 |
| 少妇 → 熟女 | 새댁 → 숙녀 | 1 |
| 熟女为止 | 숙녀까지 | 1 |

- Static dnlib read-back checks: all old five display strings absent, each new Korean display string appears once, six essential comparison keys still exist.
- Original mod DLL untouched. Actual Unity/CLR game execution not tested.
