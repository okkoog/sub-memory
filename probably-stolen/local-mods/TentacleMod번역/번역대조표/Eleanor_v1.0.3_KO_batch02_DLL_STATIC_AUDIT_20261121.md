# Fallen Heiress - Eleanor v1.0.3 — Korean DLL Batch 2 (static test)

- Original SHA256: 4647595ba56fbfc7bcc891167ddc60dd6d81e5e803724b33faf89d2a6855752f
- Patched SHA256: c985dfd1271b350a62e33676590b85c4033c0852310d6a44b2e9c93f5826f3f5
- Original bytes: 16439808
- Patched bytes: 16441344
- Embedded zh.csv: preserved byte-for-byte
- Embedded en.csv: precisely 131 reviewed Korean substitutions (batch 1: 55; batch 2: 76); remaining English rows unchanged
- CLR resource names/count: preserved; all non-target resources byte-identical
- MethodDef/TypeDef row counts: preserved
- CSV row count: 1839 unchanged, keys/order unchanged; placeholder tokens matched for each edited row
- Korean game localization follows original non-zh branch (en resource); **English also displays partial Korean when this test DLL is enabled**.
- Real CLR/Unity game load, UI, save, item compatibility: NOT TESTED. This is a partial trial build, not complete translation.
