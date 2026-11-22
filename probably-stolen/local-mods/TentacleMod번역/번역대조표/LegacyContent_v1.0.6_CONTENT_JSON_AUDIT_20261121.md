# LegacyContent v1.0.6 (48 sound variant) — Content/content.json bilingual static audit (final update 2026-11-22)

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
| `dialogue.*` | 198 | 198 | 0 |
| `descriptions.nightScenes` | 5 | 5 | 0 |
| `manifest.name` / `manifest.description` | 2 | 2 | 0 |

- The item names, UI tooltips, narrative descriptions, per-faction and per-stage descriptions, perk text, night report, cloned character name patterns, category labels and English/Chinese manifest fields were individually translated where indicated above.
- All 198 translated dialogue lines passed exact named-format-placeholder comparison (including `{0}`), with **zero placeholder mismatches**. Existing Korean lines: 170; Korean translations recovered from the prior conversation and inserted on 2026-11-22: `combos/sister/tsundere` 10, `combos/sister/motherly` 10, `bodyLines/sister` 8.
- The five `labels.bodies[].display` values follow the agreed two-Hangul-syllable rule: `sister`=`로리`, `mature_sister`=`누님`, `wife`=`아내`, `mature_woman`=`숙녀`, `beauty`=`누나`. The operational IDs are unchanged. (`mature_sister`: `성숙한 여성`→`누님`; `wife`: `유부녀`→`아내`; corrected 2026-11-22.)
- Untranslated dialogue strings: **0 of 198**. No dialogue containing Chinese characters remains. Five night-scene lines are separately translated, retaining `{name}`, `{zone}`, `{sensitivity}` placeholders.
- `addressWeights` body IDs and preference weights are not display prose; the original values are preserved and are not counted in the translation numerator.

## Independent checks before packaging
1. Parse source/translated UTF-8 JSON.
2. Assert `dialogue` groups have unchanged identity and order; compare every string's `{...}` placeholders.
3. Count 198 translated/0 unchanged dialogue texts out of 198.
4. Assert 70 changed/2 unchanged description values, 53 changed/0 unchanged label displays, all 5 night scenes untouched.
5. Assert `addressWeights` and all gameplay IDs/selector names identical, `schemaVersion` equal.
6. Only ship external JSON and readme as a patch against the original mod.

## 2026-11-22 final recheck
- Source SHA `2802bb5ee311555ff28572d647ca8ddaa82a743f`; translated JSON blob after applying the final 28 lines: `a917a6d9a612e9c7b83e3aa4a66795095f684b61` (commit `5ed3c134ae4fa7590f9457b07915bfefcfa44bcb`).
- Recursive original/translated JSON comparison: **328** text-only changes (labels 53, dialogue 198, descriptions.entries 70, nightScenes 5, manifest 2); **0** structural/key/array-order mismatches, **0** placeholder mismatches. All operational IDs and data are preserved.
- The remaining unchanged description values are an already-neutral variable-only text and an original blank. These are not untranslated source phrases.
- The pre-2026-11-22 deployment ZIP ending `170of198` is obsolete. Package the latest JSON and README into a newly named ZIP before use.
- Static data verification only; actual game loading/UI compatibility was not tested.

This records the exact completion status of external `Content/content.json` translation. It does not claim the overall mod project or game-runtime compatibility is verified.
