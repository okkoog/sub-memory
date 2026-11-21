# Probably Adult Goods v1.1.1 — isolated Korean DLL static audit

- Original size: 8166912 bytes; SHA256 60f586bd14b68b12ec1b68382a280957323cecff45e4afbf4d4cd29e637bdacf
- Korean DLL size: 8205312 bytes; SHA256 7a89daaf6d778da5b7d89d194182f3fdb12467b1f68dce6117b96fc51dc03a4d
- Original managed resources: 65; rewritten DLL: 67. All 65 original resources byte-identical.
- Two additional embedded locale resources: ProbablyAdultGoods.Localization.ko.csv and .ko-KR.csv (identical 181-key Korean CSV).
- Original EmbeddedAssets.Inject dynamically enumerates .csv resources; InjectCsv derives language keys using BaseName. No IL patch required.
- CLR MethodDef 370, TypeDef 92 unchanged. Chinese and English original resources byte-identical.
- 104 old translations reused at same keys and identical Chinese text; 37 moved flavor translations reused by exact source-text match; 38 Chinese source descriptions newly translated; 2 values adapted from original English text, not Chinese-to-Korean literal translations.
- Original source DLL and all other mods unchanged. Unity/CLR runtime loading, game UI, saving and compatibility NOT TESTED.
