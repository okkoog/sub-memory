# Probably Adult Goods v1.1.1 Chinese-source restoration (2026-11-22)

- Exactly 2 Korean description values changed; all 181 keys and the remaining 179 values retained.
- The original v1.1.1 Chinese and English embedded localization resources were not changed.
- Korean renderings were reused from the original-Chinese-aligned corresponding *_flavor_alt entries in the existing reviewed v1.1.3 Korean file.
- Standalone v1.1.1 and integrated v1.1.3 rewritten DLLs contain identical updated Korean resources in both ko/ko-KR aliases.
- dnlib read-back verified TypeDef/MethodDef resource counts, full unchanged method-IL instruction snapshots, and all non-Korean resource bytes unchanged.
- In-game Unity/CLR loading, save-game and native payload were NOT tested.

| Key | Previous basis | New basis |
|---|---|---|
| `dildo_red_flavor` | English source | Original Chinese via `dildo_red_flavor_alt` |
| `dildo_pink_slim_flavor` | English source | Original Chinese via `dildo_pink_slim_flavor_alt` |

## Relevant output files
- `3842259965/Localization/ko.csv`
- `3842259965/probably_adult_goods.dll`
- `3843142171/fallen_heiress_eleanor.dll`
- `ProbablyStolen_AdultGoods_v1.1.1_DLL_KO_181_ChineseRestored_20261122.zip`
- `ProbablyStolen_Integrated_Eleanor_AdultGoods_v1.1.3_KO_ChineseRestored_20261122.zip`
