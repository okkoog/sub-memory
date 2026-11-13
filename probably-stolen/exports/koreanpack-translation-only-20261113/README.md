# Translation-only test DLL — Probably Stolen Korean Pack

Original: `probably-stolen/mods/3834117650/ProbablyStolenKoreanPack.dll`, blob `5c9196e44cfb147ba773dc613a2f993cb3e5fc08`, 35840 bytes.

Changes: Functionally disable **all non-translation features** while leaving the Localization CSV loading and mod-loaded translation insertion logic untouched.
- Disable FishingStartPerkCompatibility.Enable.
- Disable WaterNameCompatibility.Enable (no water table, water tooltip, or water name repairs).
- Disable late game-loaded labeler/condition cache repair subscription.
- Replace the auto-registered CoreGameWaterFeaturePostfix with a no-op (it may still appear as one registered Harmony patch).

The non-translation methods remain physically in this DLL but are inactive. This is a verified IL bytecode patch, not source-code recompilation.

**Untested in-game.** Back up your installed KoreanPack DLL before replacing it. Keep manifest and Localization CSV unchanged. This is for testing only; do not publish to Workshop until verified. Original DLL and main branch are not overwritten.
