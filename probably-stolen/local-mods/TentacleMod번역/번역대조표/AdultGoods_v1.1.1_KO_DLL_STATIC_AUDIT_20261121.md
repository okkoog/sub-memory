# Probably Adult Goods v1.1.1 — isolated Korean DLL static audit

- Original size: 8166912 bytes; SHA256 60f586bd14b68b12ec1b68382a280957323cecff45e4afbf4d4cd29e637bdacf
- Korean DLL size: 8205312 bytes; SHA256 5c865ed935cbef9bd69a52f4225813dfd37d976f86a00a01793dadbea90e739f
- Original managed resources: 65; rewritten DLL: 67. All 65 original resources byte-identical.
- Two additional embedded locale resources: ProbablyAdultGoods.Localization.ko.csv and .ko-KR.csv (identical 181-key Korean CSV).
- Original EmbeddedAssets.Inject dynamically enumerates .csv resources; InjectCsv derives language keys using BaseName. No localization-loader IL change required. Exactly 10 method-level display tooltip ldstr references patched: DragonCase.ProgressLine (6), DragonCase.OnTooltip (2), DragonDisplayCase.OnTooltip (2). All gameplay identifiers and other method IL strings left untouched.
- CLR MethodDef 370, TypeDef 92 unchanged. Chinese and English original resources byte-identical.
- 104 old translations reused at same keys and identical Chinese text; 37 moved flavor translations reused by exact source-text match; 38 Chinese source descriptions newly translated; 2 values adapted from original English text, not Chinese-to-Korean literal translations.
- Original source DLL and all other mods unchanged. Unity/CLR runtime loading, game UI, saving and compatibility NOT TESTED.
