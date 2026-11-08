# CorpPresence 0.9.5 — static ILSpy audit

- DLL: `/home/runner/work/sub-memory/sub-memory/probably-stolen/mods/3828913695/CorpPresence.dll`
- Size: 7508480 bytes
- SHA256: `22471b95cebb23041ac17d7df69092a8d6060d67cb0d85ef025c7a8e3fadb60e`
- Decompiled C# files: 119
- Generated (UTC): 2026-11-08T01:12:40.995259+00:00
- Scope: **CorpPresence only**. This is a static text audit; no mod/game code was executed.
- IMPORTANT: The matching lines are search leads, NOT proof of run-time patch registration or conflict.

## Harmony attributes and patch installation

Total matching lines: 13

```text
CorpPresence/Hooks.cs:35: Patch(h, AccessTools.Method(typeof(ModConfigWindow), "Open", new Type[1] { typeof(LoadedMod) }, (Type[])null), "ModConfigWindow.Open", typeof(ConfigLocalization), null, "AfterOpen");
CorpPresence/Hooks.cs:43: MethodInfo original = AccessTools.Method(typeof(ItemMouseDragHandler), "EndDrag", new Type[1] { typeof(bool) }, (Type[])null) ?? AccessTools.Method(typeof(ItemMouseDragHandler), "EndDrag", Type.EmptyTypes, (Type[])null);
CorpPresence/Hooks.cs:81: MethodInfo methodInfo = AccessTools.Method(typeof(DialoguePhoneUIManager), "ApplyBubbleScale", Type.EmptyTypes, (Type[])null);
CorpPresence/Hooks.cs:114: Patch(h, AccessTools.Method("AquacultureHelper+TankOccupancy:Fits", new Type[3]
CorpPresence/Hooks.cs:136: Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction", typeof(SilverBlood), null, "AfterTemperatureReaction");
CorpPresence/Hooks.cs:137: Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction.Perfume", typeof(Perfume), null, "AfterTemperatureReaction");
CorpPresence/Hooks.cs:157: Patch(h, AccessTools.Method(typeof(RenderHandler), "LoadFromAtlasRaw", new Type[2]
CorpPresence/Hooks.cs:192: Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
CorpPresence/Hooks.cs:229: MethodInfo methodInfo = AccessTools.Method(typeof(T), method, array, (Type[])null);
CorpPresence/Hooks.cs:236: h.Patch((MethodBase)methodInfo, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMetho
CorpPresence/Hooks.cs:250: h.Patch((MethodBase)original, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMethod)
CorpPresence/Main.cs:71: harmony.UnpatchAll(_harmony.Id);
CorpPresence/UI/InputShield.cs:19: private static readonly MethodInfo ResetDrag = AccessTools.Method(typeof(ItemMouseDragHandler), "OnEventReset", (Type[])null, (Type[])null);
```

## Mechanic and water initialization

Total matching lines: 16

```text
CorpPresence/Canning.cs:1297: return WaterFeatureHelper.GetPurityArrayIndex(WaterHelper.GetWaterPurity(water)) &lt;= 2;
CorpPresence/Canning.cs:1343: if (WaterFeatureHelper.GetPurityArrayIndex(WaterHelper.GetWaterPurity(water)) &gt; 2)
CorpPresence/Canning.cs:1432: WaterFeatureHelper.OnLiquidContainerPourOutOf(water);
CorpPresence/DavreCo.cs:171: ItemCondition conditionFromPurity = WaterFeatureHelper.GetConditionFromPurity(purity);
CorpPresence/Forge.cs:524: EquipmentCondition val = ConditionOf(WaterFeatureHelper.GetPurityArrayIndex(WaterHelper.GetWaterPurity(childItem)));
CorpPresence/GameResources.cs:77: _mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), selectedLocale);
CorpPresence/GameResources.cs:82: throw new InvalidOperationException("Mechanic 表句柄失效，停止公司初始化");
CorpPresence/GameResources.cs:90: throw new InvalidOperationException("Mechanic 表加载失败", _mechanic.OperationException);
CorpPresence/GameResources.cs:95: throw new InvalidOperationException("Mechanic 表语言与游戏启动语言不符");
CorpPresence/GameResources.cs:97: if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
CorpPresence/GameResources.cs:99: throw new InvalidOperationException("Mechanic 表缺少 mech_condition_pure_water");
CorpPresence/Hooks.cs:192: Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
CorpPresence/Perfume.cs:537: WaterFeatureHelper.OnLiquidChanged(container, WaterFeatureHelper.GetWaterFeature(container).IsExposed());
CorpPresence/Perfume.cs:559: WaterFeatureHelper.OnLiquidChanged(container, WaterFeatureHelper.GetWaterFeature(container).IsExposed());
CorpPresence/SilverBlood.cs:914: WaterFeatureHelper.OnLiquidChanged(item, WaterFeatureHelper.GetWaterFeature(item).IsExposed());
CorpPresence/SilverBlood.cs:962: WaterFeatureHelper.OnLiquidChanged(container, WaterFeatureHelper.GetWaterFeature(container).IsExposed());
```

## Localization

Total matching lines: 13

```text
CorpPresence/GameResources.cs:12: private static AsyncOperationHandle&lt;StringTable&gt; _mechanic;
CorpPresence/GameResources.cs:33: _mechanic = default(AsyncOperationHandle&lt;StringTable&gt;);
CorpPresence/GameResources.cs:55: AsyncOperationHandle&lt;LocalizationSettings&gt; initializationOperation = LocalizationSettings.InitializationOperation;
CorpPresence/GameResources.cs:64: Locale selectedLocale = LocalizationSettings.SelectedLocale;
CorpPresence/GameResources.cs:65: if ((Object)(object)selectedLocale == (Object)null)
CorpPresence/GameResources.cs:70: LocaleIdentifier val = selectedLocale.Identifier;
CorpPresence/GameResources.cs:77: _mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), selectedLocale);
CorpPresence/GameResources.cs:97: if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
CorpPresence/GameResources.cs:107: return LocHelper.ResolveStartupLocale() switch
CorpPresence/Goods.cs:80: return text + "\n\n" + LocHelper.BulletLines(L.T(ModuleEffects.Zh, ModuleEffects.En));
CorpPresence/Hooks.cs:78: Patch&lt;StorePhoneClient&gt;(h, "GetLocalizedDisplayName", typeFromHandle, null, "AfterPhoneName");
CorpPresence/L.cs:33: _locale = LocHelper.GetCurrentLocaleCode() ?? "";
CorpPresence/Perfume.cs:1101: return LocHelper.BulletLines(L.T(tuple.Item1, tuple.Item2));
```

## Mod load lifecycle

Total matching lines: 8

```text
CorpPresence/Hooks.cs:262: ModHook.OnHandlingNightlyServicesEarly += _nightly;
CorpPresence/Hooks.cs:270: ModHook.OnHandlingNightlyServicesEarly -= _nightly;
CorpPresence/ItemRegistry.cs:30: ModHook.OnModItemDirectoryInit += _onInit;
CorpPresence/ItemRegistry.cs:38: ModHook.OnModItemDirectoryInit -= _onInit;
CorpPresence/Main.cs:40: public void OnEnable()
CorpPresence/Main.cs:105: public void OnDisable()
CorpPresence/Runtime/ResourceBootstrap.cs:22: ModHook.OnModItemDirectoryInit += _directory;
CorpPresence/Runtime/ResourceBootstrap.cs:29: ModHook.OnModItemDirectoryInit -= _directory;
```

## Pattern counts

```json
{
  "Harmony attributes and patch installation": 13,
  "Mechanic and water initialization": 16,
  "Localization": 13,
  "Mod load lifecycle": 8
}
```
