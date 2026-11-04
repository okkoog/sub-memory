# CorpPresence 0.9.4 — static ILSpy audit

- DLL: `/home/runner/work/sub-memory/sub-memory/probably-stolen/mods/3828913695/CorpPresence.dll`
- Size: 7012864 bytes
- SHA256: `56ce91c9bb6e68e21877acb5d8e5ec0d0fb603c15e33e8b3703b3fbb449b5d95`
- Decompiled C# files: 115
- Generated (UTC): 2026-11-04T07:23:39.918803+00:00
- Scope: **CorpPresence only**. This is a static text audit; no mod/game code was executed.
- IMPORTANT: The matching lines are search leads, NOT proof of run-time patch registration or conflict.

## Harmony attributes and patch installation

Total matching lines: 23

```text
CorpPresence/Hooks.cs:73: Patch(h, AccessTools.Method("AquacultureHelper+TankOccupancy:Fits", new Type[3]
CorpPresence/Hooks.cs:95: Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction", typeof(SilverBlood), null, "AfterTemperatureReaction");
CorpPresence/Hooks.cs:110: Patch(h, AccessTools.Method(typeof(RenderHandler), "LoadFromAtlasRaw", new Type[2]
CorpPresence/Hooks.cs:145: Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
CorpPresence/Hooks.cs:183: MethodInfo methodInfo = AccessTools.Method(typeof(T), method, array, (Type[])null);
CorpPresence/Hooks.cs:190: h.Patch((MethodBase)methodInfo, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMetho
CorpPresence/Hooks.cs:204: h.Patch((MethodBase)original, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMethod)
CorpPresence/Main.cs:68: harmony.UnpatchAll(_harmony.Id);
CorpPresence/UI/InputShield.cs:19: private static readonly MethodInfo ResetDrag = AccessTools.Method(typeof(ItemMouseDragHandler), "OnEventReset", (Type[])null, (Type[])null);
CorpPresence/WaterCompatibility.cs:73: owner.Patch((MethodBase)methodInfo, (HarmonyMethod)null, (HarmonyMethod)null, val, (HarmonyMethod)null);
CorpPresence/WaterCompatibility.cs:83: owner.Patch((MethodBase)AccessTools.Method(typeof(WaterHelper), "WillAcceptWater", (Type[])null, (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null, new HarmonyMethod(typeof(WaterCompatibility), "AcceptanceTranspile", (Type[])null), (HarmonyMeth
CorpPresence/WaterCompatibility.cs:96: owner.Patch((MethodBase)(AccessTools.Method(type, "RefreshUI", (Type[])null, (Type[])null) ?? throw new MissingMethodException(type.FullName, "RefreshUI")), (HarmonyMethod)null, (HarmonyMethod)null, new HarmonyMethod(typeof(WaterCompatibility), "Liqu
CorpPresence/WaterCompatibility.cs:105: MethodInfo methodInfo = AccessTools.Method(target, method, (Type[])null, (Type[])null) ?? throw new MissingMethodException(target.FullName, method);
CorpPresence/WaterCompatibility.cs:107: owner.Patch((MethodBase)methodInfo, prefix ? val : null, prefix ? null : val, (HarmonyMethod)null, (HarmonyMethod)null);
CorpPresence/WaterCompatibility.cs:167: val.operand = AccessTools.Method(typeof(WaterIdentity), text2, (Type[])null, (Type[])null);
CorpPresence/WaterCompatibility.cs:195: val.operand = AccessTools.Method(typeof(WaterIdentity), "AcceptanceKey", (Type[])null, (Type[])null);
CorpPresence/WaterCompatibility.cs:201: val.operand = AccessTools.Method(typeof(WaterIdentity), "AcceptContains", (Type[])null, (Type[])null);
CorpPresence/WaterCompatibility.cs:226: val.operand = AccessTools.Method(typeof(WaterNames), "LiquidDisplay", (Type[])null, (Type[])null);
CorpPresence/WaterHooks.cs:29: MethodInfo methodInfo = AccessTools.Method(typeof(WaterHelper), "InitLiquidContainerItem", new Type[8]
CorpPresence/WaterHooks.cs:44: _owner.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(typeof(WaterHooks), "AfterNativeContainer", (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
CorpPresence/WaterHooks.cs:62: owner.UnpatchAll(owner.Id);
CorpPresence/WaterHooks.cs:124: MethodInfo methodInfo = AccessTools.Method(typeof(WaterFeatureHelper), method, args, (Type[])null);
CorpPresence/WaterHooks.cs:129: _owner.Patch((MethodBase)methodInfo, (HarmonyMethod)null, new HarmonyMethod(typeof(SilverBlood), callback, (Type[])null), (HarmonyMethod)null, (HarmonyMethod)null);
```

## Mechanic and water initialization

Total matching lines: 24

```text
CorpPresence/DavreCo.cs:170: ItemCondition conditionFromPurity = WaterFeatureHelper.GetConditionFromPurity(purity);
CorpPresence/Forge.cs:538: EquipmentCondition val = ConditionOf(WaterFeatureHelper.GetPurityArrayIndex(WaterHelper.GetWaterPurity(childItem)));
CorpPresence/GameResources.cs:62: _mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), (Locale)null);
CorpPresence/GameResources.cs:67: throw new InvalidOperationException("Mechanic 表句柄失效，停止公司初始化");
CorpPresence/GameResources.cs:75: throw new InvalidOperationException("Mechanic 表加载失败", _mechanic.OperationException);
CorpPresence/GameResources.cs:77: if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
CorpPresence/GameResources.cs:79: throw new InvalidOperationException("Mechanic 表缺少 mech_condition_pure_water");
CorpPresence/Hooks.cs:145: Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
CorpPresence/SilverBlood.cs:868: c.unitValue = WaterFeatureHelper.GetWaterPrice(TotalParts(c)) + num * 6;
CorpPresence/SilverBlood.cs:966: WaterFeatureHelper.OnLiquidChanged(container, WaterFeatureHelper.GetWaterFeature(container).IsExposed());
CorpPresence/WaterCompatibility.cs:39: new Binding("WaterFeatureHelper", "OnWrongItemPlaceWaterClient", 6, 1),
CorpPresence/WaterCompatibility.cs:139: if (val.opcode == OpCodes.Ldsfld && val.operand is FieldInfo fieldInfo && fieldInfo.DeclaringType == typeof(WaterFeatureHelper))
CorpPresence/WaterCompatibility.cs:152: if (methodInfo.DeclaringType == typeof(WaterFeatureHelper) && methodInfo.Name == "GetCurrentWaterDisplay")
CorpPresence/WaterCompatibility.cs:192: if (methodInfo.DeclaringType == typeof(WaterFeatureHelper) && methodInfo.Name == "GetCurrentWaterDisplay")
CorpPresence/WaterContracts.cs:92: if (op == OpCodes.Ldsfld && member is FieldInfo fieldInfo && fieldInfo.DeclaringType == typeof(WaterFeatureHelper) && WaterGrades.FromField(fieldInfo.Name) != null)
CorpPresence/WaterContracts.cs:96: if (member is MethodInfo methodInfo && ((methodInfo.DeclaringType == typeof(WaterFeatureHelper) && methodInfo.Name == "GetCurrentWaterDisplay") || (methodInfo.DeclaringType == typeof(ItemFeature) && (methodInfo.Name == "GetBaseRealDisplay" || methodI
CorpPresence/WaterContracts.cs:110: if (member is MethodInfo methodInfo && methodInfo.DeclaringType == typeof(WaterFeatureHelper) && methodInfo.Name == "GetCurrentWaterDisplay")
CorpPresence/WaterHooks.cs:124: MethodInfo methodInfo = AccessTools.Method(typeof(WaterFeatureHelper), method, args, (Type[])null);
CorpPresence/WaterHooks.cs:127: throw new MissingMethodException("WaterFeatureHelper." + method);
CorpPresence/WaterIdentity.cs:68: return WaterFeatureHelper.GetCurrentWaterDisplay(item);
CorpPresence/WaterNames.cs:11: private static readonly string[] GradeKeys = new string[6] { "mech_condition_pure_water", "mech_condition_high_quality_water", "mech_condition_basewater", "mech_condition_ghostwater", "mech_condition_rustwater", "mech_condition_gutterflow" };
CorpPresence/WaterNames.cs:79: value = LocHelper.GetLocalizedMechanic(key, Array.Empty&lt;object&gt;());
CorpPresence/WaterNames.cs:148: __result = LocHelper.GetLocalizedMechanic("mech_tooltip_labeled", Array.Empty&lt;object&gt;()) + "\n" + ForPurity(val2.customValue);
CorpPresence/WaterNames.cs:178: ((TMP_Text)__instance.labelTMP).text = LocHelper.GetLocalizedMechanic("mech_tooltip_labeled", Array.Empty&lt;object&gt;()) + "\n" + ForPurity(val2.customValue);
```

## Localization

Total matching lines: 11

```text
CorpPresence/GameResources.cs:12: private static AsyncOperationHandle&lt;StringTable&gt; _mechanic;
CorpPresence/GameResources.cs:33: _mechanic = default(AsyncOperationHandle&lt;StringTable&gt;);
CorpPresence/GameResources.cs:51: AsyncOperationHandle&lt;LocalizationSettings&gt; initializationOperation = LocalizationSettings.InitializationOperation;
CorpPresence/GameResources.cs:62: _mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), (Locale)null);
CorpPresence/GameResources.cs:77: if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
CorpPresence/Hooks.cs:56: Patch&lt;StorePhoneClient&gt;(h, "GetLocalizedDisplayName", typeFromHandle, null, "AfterPhoneName");
CorpPresence/L.cs:30: string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
CorpPresence/WaterNames.cs:70: string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
CorpPresence/WaterNames.cs:79: value = LocHelper.GetLocalizedMechanic(key, Array.Empty&lt;object&gt;());
CorpPresence/WaterNames.cs:148: __result = LocHelper.GetLocalizedMechanic("mech_tooltip_labeled", Array.Empty&lt;object&gt;()) + "\n" + ForPurity(val2.customValue);
CorpPresence/WaterNames.cs:178: ((TMP_Text)__instance.labelTMP).text = LocHelper.GetLocalizedMechanic("mech_tooltip_labeled", Array.Empty&lt;object&gt;()) + "\n" + ForPurity(val2.customValue);
```

## Mod load lifecycle

Total matching lines: 8

```text
CorpPresence/Hooks.cs:216: ModHook.OnHandlingNightlyServicesEarly += _nightly;
CorpPresence/Hooks.cs:224: ModHook.OnHandlingNightlyServicesEarly -= _nightly;
CorpPresence/ItemRegistry.cs:81: ModHook.OnModItemDirectoryInit += _onInit;
CorpPresence/ItemRegistry.cs:89: ModHook.OnModItemDirectoryInit -= _onInit;
CorpPresence/Main.cs:38: public void OnEnable()
CorpPresence/Main.cs:103: public void OnDisable()
CorpPresence/Runtime/ResourceBootstrap.cs:22: ModHook.OnModItemDirectoryInit += _directory;
CorpPresence/Runtime/ResourceBootstrap.cs:29: ModHook.OnModItemDirectoryInit -= _directory;
```

## Pattern counts

```json
{
  "Harmony attributes and patch installation": 23,
  "Mechanic and water initialization": 24,
  "Localization": 11,
  "Mod load lifecycle": 8
}
```
