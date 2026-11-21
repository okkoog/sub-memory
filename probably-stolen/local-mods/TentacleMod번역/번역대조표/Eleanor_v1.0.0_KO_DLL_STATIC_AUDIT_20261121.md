# Eleanor v1.0.0 Korean DLL static audit

- Original SHA256 2bd5865fb14bcde883876c82fe93655d89578031776f9d4e102cfd59cdad12c4, 16418816 bytes.
- Korean version SHA256 0b32263288db6ffd987c40fec966cfc8c98ec8381e77348a4a46466a2f2baa11, 16582144 bytes.
- Original 46 managed resources byte-identical; 1 added resource EleanorEmbedded/Localization/ko.csv.
- Original CLR MethodDef 1448, TypeDef 216 unchanged; 14 IL instructions inserted for ko + ko-KR.
- Source values match v1.0.3 except two original changes: purity_wilde_visit_1_6 (保释的费用可不便宜 -> 보석금이 만만치 않다네.), purity_wilde_visit_1_15 (就交500信用点 -> 500크레딧만 내게.). Missing 12 v1.0.3-only keys are deliberately absent.
- 1541 original active Korean translated rows; 286 development placeholder rows unchanged; source zh/en and all other original CLR resources preserved.
- Tested ZIP CRC; original DLL untouched. Game CLR/Unity loading, saved files, and compatibility NOT TESTED.
