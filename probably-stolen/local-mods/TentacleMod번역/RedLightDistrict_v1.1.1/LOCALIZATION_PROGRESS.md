# Red Light District v1.1.1 — external Korean Localization progress

- Source catalog: original TentacleMod/RedLightDistrict_v1.1.1/Localization/cn.csv (untouched)
- Total active IL occurrence keys: 3619 (includes diagnostics, internal strings and logs)
- Korean external ko.csv translations: 2584; untranslated/fallback occurrences: 1035
- Reviewed historical source strings: 295; mapped and selected IL sites: 351; unique selected historical keys: 351
- New manually reviewed direct keys: 2234; original user-approved seed entries preserved: 22; previously generated keys retained as fallback: 2485
- Comparison/identifier sites withheld: 3; reviewed strings at non-UI sites withheld: 57
- Untranslated keys retain original Chinese through the mod fallback. Key format, variables, line breaks, CSV quoting, original seed translations validated.
- Original TalentIntel.TypeName exact-name parser: 10 interface strings intentionally left Chinese to preserve boss filtering (see IL audit).
- No DLL files edited. Interim ZIP packaging is disabled until translation is complete.
- Actual game runtime / save data compatibility NOT TESTED; line breaks serialized as literal backslash-n for the game parser.

## Manually reviewed new key batches

- manual_direct_batch01_20261121.tsv: 51 entries
- manual_direct_batch02_20261121.tsv: 99 entries
- manual_direct_batch03_20261121.tsv: 80 entries
- manual_direct_batch04_20261121.tsv: 82 entries
- manual_direct_batch05_20261121.tsv: 55 entries
- manual_direct_batch06_20261121.tsv: 21 entries
- manual_direct_batch07_20261121.tsv: 17 entries
- manual_direct_batch08_20261121.tsv: 10 entries
- manual_direct_batch09_20261121.tsv: 34 entries
- manual_direct_batch10_20261121.tsv: 12 entries
- manual_direct_batch11_20261121.tsv: 2 entries
- manual_direct_batch12_20261121.tsv: 26 entries
- manual_direct_batch13_20261121.tsv: 23 entries
- manual_direct_batch14_20261121.tsv: 35 entries
- manual_direct_batch15_20261121.tsv: 39 entries
- manual_direct_batch16_20261121.tsv: 18 entries
- manual_direct_batch17_20261121.tsv: 14 entries
- manual_direct_batch18_20261121.tsv: 46 entries
- manual_direct_batch19_20261121.tsv: 50 entries
- manual_direct_batch20_20261121.tsv: 41 entries
- manual_direct_batch21_20261121.tsv: 60 entries
- manual_direct_batch22_20261121.tsv: 100 entries
- manual_direct_batch23_20261121.tsv: 84 entries
- manual_direct_batch24_20261121.tsv: 107 entries
- manual_direct_batch25_20261121.tsv: 99 entries
- manual_direct_batch26_20261121.tsv: 68 entries
- manual_direct_batch27_20261121.tsv: 100 entries
- manual_direct_batch28_20261121.tsv: 72 entries
- manual_direct_batch29_20261121.tsv: 60 entries
- manual_direct_batch30_20261121.tsv: 88 entries
- manual_direct_batch31_20261121.tsv: 52 entries
- manual_direct_batch32_20261121.tsv: 96 entries
- manual_direct_batch33_20261121.tsv: 77 entries
- manual_direct_batch34_20261121.tsv: 80 entries
- manual_direct_batch35_20261121.tsv: 85 entries
- manual_direct_batch36_20261121.tsv: 157 entries
- manual_direct_batch37_20261121.tsv: 99 entries

## Complete per-key audit in source order

- diagnostic-review: 519
- protected-internal: 55
- protected-review: 44
- translated: 2584
- untranslated-review: 417

KEY_COVERAGE_AUDIT_20261121.tsv covers all 3619 keys once. UNTRANSLATED_REVIEW_QUEUE_20261121.tsv holds unresolved manual-review cases. These triage classifications DO NOT prove that diagnostic or protected keys can never appear in the UI.
