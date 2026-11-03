# CorpPresence 0.9.4 — static ILSpy audit

- DLL: `/home/runner/work/sub-memory/sub-memory/probably-stolen/mods/3828913695/CorpPresence.dll`
- Size: 6961664 bytes
- SHA256: `7e34d05ad2269a0a0403fd5dcd3cea78f8b629af47f956a8366a807eea4ff668`
- Decompiled C# files: 104
- Generated (UTC): 2026-11-03T15:36:04.742586+00:00
- Scope: **CorpPresence only**. This is a static text audit; no mod/game code was executed.
- IMPORTANT: The matching lines are search leads, NOT proof of run-time patch registration or conflict.

## Harmony attributes and patch installation

Total matching lines: 9

```text
CorpPresence/Hooks.cs:73: Patch(h, AccessTools.Method("AquacultureHelper+TankOccupancy:Fits", new Type[3]
CorpPresence/Hooks.cs:95: Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction", typeof(SilverBlood), null, "AfterTemperatureReaction");
CorpPresence/Hooks.cs:110: Patch(h, AccessTools.Method(typeof(RenderHandler), "LoadFromAtlasRaw", new Type[2]
CorpPresence/Hooks.cs:145: Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
CorpPresence/Hooks.cs:198: MethodInfo methodInfo = AccessTools.Method(typeof(T), method, array, (Type[])null);
CorpPresence/Hooks.cs:205: h.Patch((MethodBase)methodInfo, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMetho
CorpPresence/Hooks.cs:219: h.Patch((MethodBase)original, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMethod)
CorpPresence/Main.cs:58: harmony.UnpatchAll(_harmony.Id);
CorpPresence/UI/InputShield.cs:19: private static readonly MethodInfo ResetDrag = AccessTools.Method(typeof(ItemMouseDragHandler), "OnEventReset", (Type[])null, (Type[])null);
```

## Mechanic and water initialization

Total matching lines: 13

```text
CorpPresence/DavreCo.cs:170: ItemCondition conditionFromPurity = WaterFeatureHelper.GetConditionFromPurity(purity);
CorpPresence/Forge.cs:515: EquipmentCondition val = ConditionOf(WaterFeatureHelper.GetPurityArrayIndex(WaterHelper.GetWaterPurity(childItem)));
CorpPresence/GameResources.cs:62: _mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), (Locale)null);
CorpPresence/GameResources.cs:67: throw new InvalidOperationException("Mechanic 表句柄失效，停止公司初始化");
CorpPresence/GameResources.cs:75: throw new InvalidOperationException("Mechanic 表加载失败", _mechanic.OperationException);
CorpPresence/GameResources.cs:77: if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
CorpPresence/GameResources.cs:79: throw new InvalidOperationException("Mechanic 表缺少 mech_condition_pure_water");
CorpPresence/Hooks.cs:145: Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
CorpPresence/Hooks.cs:175: Patch&lt;WaterFeatureHelper&gt;(h, "UpdateWaterFeature", typeof(SilverBlood), null, "AfterUpdateWaterFeature", new Type[2]
CorpPresence/Hooks.cs:180: Patch&lt;WaterFeatureHelper&gt;(h, "OnLiquidChanged", typeof(SilverBlood), null, "AfterLiquidChanged", new Type[2]
CorpPresence/Hooks.cs:185: Patch&lt;WaterFeatureHelper&gt;(h, "OnLiquidContainerPourInto", typeof(SilverBlood), null, "AfterPourInto", new Type[3]
CorpPresence/SilverBlood.cs:866: c.unitValue = WaterFeatureHelper.GetWaterPrice(TotalParts(c)) + num * 6;
CorpPresence/SilverBlood.cs:964: WaterFeatureHelper.OnLiquidChanged(container, WaterFeatureHelper.GetWaterFeature(container).IsExposed());
```

## Localization

Total matching lines: 7

```text
CorpPresence/GameResources.cs:12: private static AsyncOperationHandle&lt;StringTable&gt; _mechanic;
CorpPresence/GameResources.cs:33: _mechanic = default(AsyncOperationHandle&lt;StringTable&gt;);
CorpPresence/GameResources.cs:51: AsyncOperationHandle&lt;LocalizationSettings&gt; initializationOperation = LocalizationSettings.InitializationOperation;
CorpPresence/GameResources.cs:62: _mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), (Locale)null);
CorpPresence/GameResources.cs:77: if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
CorpPresence/Hooks.cs:56: Patch&lt;StorePhoneClient&gt;(h, "GetLocalizedDisplayName", typeFromHandle, null, "AfterPhoneName");
CorpPresence/L.cs:30: string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
```

## Mod load lifecycle

Total matching lines: 8

```text
CorpPresence/Hooks.cs:231: ModHook.OnHandlingNightlyServicesEarly += _nightly;
CorpPresence/Hooks.cs:239: ModHook.OnHandlingNightlyServicesEarly -= _nightly;
CorpPresence/ItemRegistry.cs:26: ModHook.OnModItemDirectoryInit += _onInit;
CorpPresence/ItemRegistry.cs:34: ModHook.OnModItemDirectoryInit -= _onInit;
CorpPresence/Main.cs:38: public void OnEnable()
CorpPresence/Main.cs:88: public void OnDisable()
CorpPresence/Runtime/ResourceBootstrap.cs:22: ModHook.OnModItemDirectoryInit += _directory;
CorpPresence/Runtime/ResourceBootstrap.cs:29: ModHook.OnModItemDirectoryInit -= _directory;
```

## Pattern counts

```json
{
  "Harmony attributes and patch installation": 9,
  "Mechanic and water initialization": 13,
  "Localization": 7,
  "Mod load lifecycle": 8
}
```
