# LegacyContent v1.0.6 (48 sound variant) — Content/content.json bilingual static audit

## Origin and file selection
- Source (unchanged): `TentacleMod/LewdMemeLegacyContent-1.0.6-48sounds/Content/content.json`; source Git blob `2802bb5ee311555ff28572d647ca8ddaa82a743f`.
- Updated: `TentacleMod번역/LewdMemeLegacyContent-1.0.6-48sounds/Content/content.json`.
- Original `LewdMemeLegacyContent.dll` was already translated in an earlier sequential DLL batch. This is **additional external content loaded from JSON**, not a claim to have rebuilt the DLL again.
- Preserved unchanged `schemaVersion`, `addressWeights`, selector IDs, all `bodyId`, `personalityId`, `kind` and JSON key order. No original files modified.

## Strict per-field inventory

| Location | Total entries | Korean changed | Original preserved |
| --- | ---: | ---: | ---: |
| `descriptions.entries` | 72 | 70 | 2 (one variable-only layout already language-neutral, one originally empty string) |
| `labels.*[].display` | 53 | 53 | 0 |
| `dialogue.*` | 198 | 170 | 28 (`combos/sister/tsundere` 10; `combos/sister/motherly` 10; `bodyLines/sister` 8) |
| `descriptions.nightScenes` | 5 | 5 | 0 |
| `manifest.name` / `manifest.description` | 2 | 2 | 0 |

- The item names, UI tooltips, narrative descriptions, per-faction and per-stage descriptions, perk text, night report, cloned character name patterns, category labels and English/Chinese manifest fields were individually translated where indicated above.
- All 170 changed dialogue lines underwent exact named-format-placeholder comparison (including `{0}`), with **zero placeholder mismatches**.
- Label `bodies/sister` displays `로리` in Korean; its operational ID `sister` is untouched.
- 28 remaining original dialogue lines are explicitly counted as untranslated and **must not** be presented as complete. The 5 adult night-scene lines were separately translated, with exact `{name}`, `{zone}`, `{sensitivity}` placeholder comparison.
- `addressWeights` body IDs and preference weights are not display prose; the original values are preserved and are not counted in the translation numerator.

## Independent checks before packaging
1. Parse source/translated UTF-8 JSON.
2. Assert `dialogue` groups have unchanged identity and order; compare every string's `{...}` placeholders.
3. Count 170 translated/28 unchanged dialogue texts out of 198.
4. Assert 70 changed/2 unchanged description values, 53 changed/0 unchanged label displays, all 5 night scenes untouched.
5. Assert `addressWeights` and all gameplay IDs/selector names identical, `schemaVersion` equal.
6. Only ship external JSON and readme as a patch against the original mod.

This records precise work done, not a claim of full translation or a need for the assistant to test the user's game.
