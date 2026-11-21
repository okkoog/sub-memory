# Fallen Heiress - Eleanor v1.0.3 — Korean DLL Batch 13 (static test)

- Original SHA256: 4647595ba56fbfc7bcc891167ddc60dd6d81e5e803724b33faf89d2a6855752f
- Patched SHA256: 3674e6582d564aaf9b39883dc377a1135d9ebbf0f931a3608d1f39541ffd9996
- Original bytes: 16439808
- Patched bytes: 16458752
- Embedded zh.csv: preserved byte-for-byte
- Embedded en.csv: precisely 1553 manually reviewed Korean substitutions across 13 batches; remaining English rows unchanged
- CLR resource names/count: preserved; all non-target resources byte-identical
- MethodDef/TypeDef row counts: preserved
- CSV row count: 1839 unchanged, keys/order unchanged; placeholder tokens matched for each edited row
- Korean game localization follows original non-zh branch (en resource); **English also displays partial Korean when this test DLL is enabled**.
- Real CLR/Unity game load, UI, save, item compatibility: NOT TESTED. This is a partial trial build, not complete translation.
