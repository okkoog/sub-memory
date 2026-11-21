# Eleanor v1.0.3 — isolated Korean locale DLL static audit

- Original SHA256: 4647595ba56fbfc7bcc891167ddc60dd6d81e5e803724b33faf89d2a6855752f
- Patched SHA256: 51fb9b043ce34c0ec46498a1fefb4bf0150438011e72e8273df320b19286ab5a
- Original bytes: 16439808; Korean DLL bytes: 16605184
- All original managed resources are byte-identical, including original Chinese and English CSV
- One added Korean CLR resource: EleanorEmbedded/Localization/ko.csv
- Original 1839 CSV keys and order preserved; 1553 manually translated values; 286 intentionally inactive development placeholders unmodified
- Original CLR MethodDef/TypeDef counts retained
- InstallLocalization method: 14 IL instructions inserted to load new KO resource and register both ko and ko-KR; original en and zh references retained
- ZIP CRC32 verified. No source DLL was changed.
- Actual Unity/CLR runtime, save files, and inter-mod compatibility remain untested.
