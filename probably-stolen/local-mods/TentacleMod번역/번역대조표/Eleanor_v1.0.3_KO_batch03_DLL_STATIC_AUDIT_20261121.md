# Fallen Heiress - Eleanor v1.0.3 — Korean DLL Batch 3 (static test)

- Original SHA256: 4647595ba56fbfc7bcc891167ddc60dd6d81e5e803724b33faf89d2a6855752f
- Patched SHA256: d47f0c328cb8505a51632738f29481475f57ed0f6b4c19b265ed310cb91cdb1c
- Original bytes: 16439808
- Patched bytes: 16443392
- Embedded zh.csv: preserved byte-for-byte
- Embedded en.csv: precisely 296 manually reviewed Korean substitutions across 3 batches; remaining English rows unchanged
- CLR resource names/count: preserved; all non-target resources byte-identical
- MethodDef/TypeDef row counts: preserved
- CSV row count: 1839 unchanged, keys/order unchanged; placeholder tokens matched for each edited row
- Korean game localization follows original non-zh branch (en resource); **English also displays partial Korean when this test DLL is enabled**.
- Real CLR/Unity game load, UI, save, item compatibility: NOT TESTED. This is a partial trial build, not complete translation.
