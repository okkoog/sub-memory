# Water condition identifier safety patch (2026-11-13)

Test build of the existing 35,840-byte ProbablyStolenKoreanPack.dll. Only KoreanPack DLL modified. No third-party mods changed.

In WaterNameCompatibility.RepairCondition, remove the IL instruction sequence that writes back ItemCondition.identifier. Keep all existing display-string translations and other features unchanged.

Binary patch (7 bytes at file offset 10625):
- Before: 03 07 7D F5 00 00 0A
- After:  00 00 00 00 00 00 00

Original Git blob SHA: 5c9196e44cfb147ba773dc613a2f993cb3e5fc08
Patched Git blob SHA: 28648f02cbc37525ca112d6c5d300a1078bdad5f
Patched assembly size: 35,840 bytes.

Both GitHub locations (mods/3834117650 and 번역 모드) contain identical patched DLLs on this test branch. All other binaries, original modules and CSVs stay unchanged. Main and Steam are not changed.

Test with the affected save: verify Basic Water / Purified Water displaying and selling. No in-game runtime verification was performed here.
