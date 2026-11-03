# CorpPresence — targeted callsite context

Only excerpts for relevant initialization/Harmony methods; no code was executed.

## CorpPresence/Main.cs

Matching lines: 10; displayed context lines: 56

```text
34: 		_harmony = new Harmony("mod." + manifest.ID);
35: 		Settings.Init(manifest.ID);
36: 	}
37: 
38: 	public void OnEnable()
39: 	{
40: 		if (_runtime.Start(EnableCore))
41: 		{
42: 			_bootstrap.Tick();
43: 		}
44: 	}
45: 
46: 	private void EnableCore()
47: 	{
48: 		_runtime.Own(GameResources.Begin, GameResources.Reset);
49: 		_runtime.Own(ItemRegistry.Install, ItemRegistry.Uninstall);
50: 		_runtime.Own(delegate
51: 		{
52: 			Hooks.InstallCore(_harmony);
53: 		}, delegate
54: 		{
55: 			Harmony harmony = _harmony;
56: 			if (harmony != null)
57: 			{
58: 				harmony.UnpatchAll(_harmony.Id);
59: 			}
60: 		});
68: 	}
69: 
70: 	private void EnableResources()
71: 	{
72: 		_runtime.Own(SilverBlood.InstallResources, SilverBlood.UninstallResources);
73: 		_runtime.Own(Tobacco.RegisterPlant, Tobacco.UnregisterPlant);
74: 		Hooks.InstallResources(_harmony);
75: 		_runtime.Own(Hooks.InstallEvents, Hooks.UninstallEvents);
76: 		_runtime.Own(CorpPresence.Malice.Malice.Install, CorpPresence.Malice.Malice.Uninstall);
77: 		ItemRegistry.Activate();
78: 		_runtime.Own(Noop, Tex.ReleaseUi);
79: 		_runtime.Own(Noop, Theme.ResetCachedFont);
80: 		_runtime.Own(Noop, Portraits.ResetSceneReferences);
81: 		_runtime.Own(Noop, Notebook.Destroy);
82: 		_runtime.Own(Noop, InputShield.Destroy);
83: 		_runtime.Own(Noop, CompanyWindow.ReleaseUi);
84: 		_runtime.Own(CreateHost, DestroyHost);
85: 		ModLog.Info("公司增强 0.9.4 资源阶段已加载（人物：" + string.Join("、", Engine.All.Select((Character c) =&gt; c.Id)) + "）");
86: 	}
87: 
88: 	public void OnDisable()
89: 	{
90: 		_runtime.Stop();
91: 	}
92: 
93: 	private static void Noop()
94: 	{
95: 	}
96: 
```

## CorpPresence/GameResources.cs

Matching lines: 12; displayed context lines: 63

```text
8: namespace CorpPresence;
9: 
10: internal static class GameResources
11: {
12: 	private static AsyncOperationHandle&lt;StringTable&gt; _mechanic;
13: 
14: 	private static bool _requested;
15: 
16: 	private static long _generation;
17: 
18: 	public static bool Ready { get; private set; }
19: 
20: 	public static long Generation =&gt; _generation;
29: 	{
30: 		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
31: 		Ready = false;
32: 		_requested = false;
33: 		_mechanic = default(AsyncOperationHandle&lt;StringTable&gt;);
34: 	}
35: 
36: 	public static bool Poll()
37: 	{
38: 		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
39: 		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
40: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
41: 		//IL_002b: Invalid comparison between Unknown and I4
48: 		{
49: 			return true;
50: 		}
51: 		AsyncOperationHandle&lt;LocalizationSettings&gt; initializationOperation = LocalizationSettings.InitializationOperation;
52: 		if (!initializationOperation.IsValid() || !initializationOperation.IsDone)
53: 		{
54: 			return false;
55: 		}
56: 		if ((int)initializationOperation.Status != 1)
57: 		{
58: 			throw new InvalidOperationException("本地化系统初始化失败", initializationOperation.OperationException);
59: 		}
60: 		if (!_requested)
61: 		{
62: 			_mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), (Locale)null);
63: 			_requested = true;
64: 		}
65: 		if (!_mechanic.IsValid())
66: 		{
67: 			throw new InvalidOperationException("Mechanic 表句柄失效，停止公司初始化");
68: 		}
69: 		if (!_mechanic.IsDone)
70: 		{
71: 			return false;
72: 		}
73: 		if ((int)_mechanic.Status != 1 || (Object)(object)_mechanic.Result == (Object)null)
74: 		{
75: 			throw new InvalidOperationException("Mechanic 表加载失败", _mechanic.OperationException);
76: 		}
77: 		if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
78: 		{
79: 			throw new InvalidOperationException("Mechanic 表缺少 mech_condition_pure_water");
80: 		}
81: 		Ready = true;
82: 		return true;
83: 	}
84: }
```

## CorpPresence/Hooks.cs

Matching lines: 83; displayed context lines: 228

```text
16: 	private static string _reportedId;
17: 
18: 	private static int _revBefore;
19: 
20: 	public static void InstallCore(Harmony h)
21: 	{
22: 		Type typeFromHandle = typeof(Hooks);
23: 		Type typeFromHandle2 = typeof(ItemRegistry);
24: 		Patch&lt;SaveManager&gt;(h, "DecodeNodes", typeFromHandle2, "BeforeDecode", null, new Type[1] { typeof(List&lt;SaveItemNode&gt;) });
25: 		Patch&lt;SaveManager&gt;(h, "DecodeNodes", typeof(ItemTags), null, "AfterDecode", new Type[1] { typeof(List&lt;SaveItemNode&gt;) });
26: 		Patch&lt;SaveManager&gt;(h, "EncodeNode", typeFromHandle2, "BeforeEncode", null, new Type[1] { typeof(GameItem) });
27: 		Patch&lt;PlayerStore&gt;(h, "SaveGame", typeFromHandle, "BeforeSave");
28: 		Patch&lt;PlayerStore&gt;(h, "LoadGame", typeFromHandle, null, "ResetState");
29: 		Patch&lt;PlayerStore&gt;(h, "StartNewGame", typeFromHandle, null, "ResetState");
30: 	}
31: 
32: 	private static void InstallGameplay(Harmony h)
33: 	{
34: 		Type typeFromHandle = typeof(Hooks);
35: 		Type typeFromHandle2 = typeof(ItemRegistry);
36: 		Patch&lt;PlayerStore&gt;(h, "BeginDay", typeFromHandle, null, "AfterBeginDay");
37: 		Patch&lt;ItemMouseDragHandler&gt;(h, "EndDrag", typeof(DragFix), "BeforeEndDrag", "AfterEndDrag");
38: 		Patch&lt;StoreClientManager&gt;(h, "GenerateClient", typeFromHandle, null, "AfterGenerateClient");
39: 		Patch&lt;PlayerStore&gt;(h, "GetNextClient", typeFromHandle, "BeforeNextClient");
40: 		Patch&lt;GameItem&gt;(h, "GetNegociatedValue", typeFromHandle, null, "AfterNegociatedValue");
41: 		Patch&lt;StoreClient&gt;(h, "OnItemSold", typeFromHandle, null, "AfterClientSold", new Type[2]
42: 		{
43: 			typeof(GameItem),
44: 			typeof(int)
45: 		});
46: 		Patch&lt;StoreClient&gt;(h, "OnItemBought", typeFromHandle, null, "AfterClientBought", new Type[2]
47: 		{
48: 			typeof(GameItem),
49: 			typeof(int)
50: 		});
51: 		Patch&lt;PlayerStore&gt;(h, "EndDay", typeFromHandle, "BeforeEndDay");
52: 		Patch&lt;StorePhoneClient&gt;(h, "InitPhoneClientDict", typeFromHandle, null, "AfterInitPhones");
53: 		Patch&lt;StorePhoneClient&gt;(h, "GetCallDialog", typeFromHandle, null, "AfterGetCallDialog");
54: 		Patch&lt;PhoneUIManager&gt;(h, "WillAnswerCall", typeFromHandle, null, "AfterWillAnswer", new Type[1] { typeof(long) });
55: 		Patch&lt;PhoneUIManager&gt;(h, "HandleCall", typeFromHandle, "BeforeHandleCall", null, new Type[1] { typeof(long) });
56: 		Patch&lt;StorePhoneClient&gt;(h, "GetLocalizedDisplayName", typeFromHandle, null, "AfterPhoneName");
57: 		Patch&lt;GameItemElement&gt;(h, "ResolveSpriteByName", typeFromHandle, "BeforeResolveSprite", null, new Type[1] { typeof(string) });
58: 		Patch&lt;GameItemElement&gt;(h, "ResolveItemSprite", typeFromHandle, "BeforeResolveItemSprite");
59: 		Patch&lt;SpriteDict&gt;(h, "GetSprite", typeFromHandle, "BeforeGetSprite", null, new Type[1] { typeof(string) });
60: 		Patch&lt;ItemConditionActionList&gt;(h, "GenuineCigaretteAction", typeof(PastelCo), null, "AfterGenuineAction", new Type[1] { typeof(GameItem) });
61: 		Patch&lt;HusbandryHelper&gt;(h, "CreateButcherCuts", typeof(Forge), "BeforeButcherCuts", "AfterButcherCuts", new Type[1] { typeof(GameItem) });
62: 		Patch&lt;GameItem&gt;(h, "BuildShapeFromSprite", typeof(Goods), "BeforeBuildShape", null, new Type[2]
63: 		{
64: 			typeof(string),
65: 			typeof(string)
66: 		});
67: 		Patch&lt;Image&gt;(h, "IsRaycastLocationValid", typeof(Goods), null, "AfterRaycastValid", new Type[2]
68: 		{
69: 			typeof(Vector2),
70: 			typeof(Camera)
71: 		});
72: 		Patch&lt;AquacultureHelper&gt;(h, "CreateFilletPieces", typeof(SilverBlood), "BeforeFillet", null, new Type[1] { typeof(GameItem) });
73: 		Patch(h, AccessTools.Method("AquacultureHelper+TankOccupancy:Fits", new Type[3]
74: 		{
75: 			typeof(int),
76: 			typeof(int),
77: 			typeof(List&lt;Vector2Int&gt;)
78: 		}, (Type[])null), "AquacultureHelper.TankOccupancy.Fits", typeof(SilverBlood), "BeforeTankFits");
79: 		Patch&lt;AquacultureHelper&gt;(h, "WillSuffocateTonight", typeof(SilverBlood), null, "AfterWillSuffocate", new Type[1] { typeof(GameItem) });
80: 		Patch&lt;AquacultureHelper&gt;(h, "ApplyWaterChecks", typeof(SilverBlood), "BeforeApplyWaterChecks", null, new Type[3]
81: 		{
82: 			typeof(List&lt;GameItem&gt;),
83: 			typeof(GameItem),
84: 			typeof(bool)
85: 		});
86: 		Patch&lt;AquacultureHelper&gt;(h, "TickTank", typeof(SilverBlood), "BeforeTickTank", "AfterTickTank", new Type[6]
87: 		{
88: 			typeof(GameItem),
89: 			typeof(GameGridInventory),
90: 			typeof(GameItem),
91: 			typeof(List&lt;GameSlotInventory&gt;),
92: 			typeof(GameSlotInventory),
93: 			typeof(bool)
94: 		});
95: 		Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction", typeof(SilverBlood), null, "AfterTemperatureReaction");
96: 		Patch&lt;MaterialDirectory&gt;(h, "MetalIngot", typeof(MachineUpgrade), null, "AfterMakeIngot");
97: 		Patch&lt;MaterialDirectory&gt;(h, "Electronic", typeof(MachineUpgrade), null, "AfterMakeElectronic");
98: 		Patch&lt;HealthData&gt;(h, "IsSentinel", typeof(SilverBlood), null, "AfterIsSentinel");
99: 		Patch&lt;StartingPerk&gt;(h, "IsSentinel", typeof(SilverBlood), null, "AfterIsSentinel");
100: 		Patch&lt;StartingPerk&gt;(h, "IsPerkActive", typeof(SilverBlood), null, "AfterIsPerkActive", new Type[1] { typeof(string) });
101: 		Patch&lt;StoreClient&gt;(h, "IsClientBuyingThisItem", typeof(SilverBlood), null, "AfterIsBuying", new Type[1] { typeof(GameItem) });
102: 		Patch&lt;CigaretteUI&gt;(h, "OnInit", typeof(PastelCo), null, "AfterCigaretteUI", new Type[1] { typeof(GameItem) });
103: 		Patch&lt;GameItem&gt;(h, "GetParentShapeModifier", typeof(RifleMods), null, "AfterParentShape", new Type[4]
104: 		{
105: 			typeof(GameItem),
106: 			typeof(SlotMarker),
107: 			typeof(GridShape),
108: 			typeof(TagSystem)
109: 		});
110: 		Patch(h, AccessTools.Method(typeof(RenderHandler), "LoadFromAtlasRaw", new Type[2]
111: 		{
112: 			typeof(string),
113: 			typeof(string)
114: 		}, (Type[])null), "RenderHandler.LoadFromAtlasRaw", typeFromHandle, "BeforeLoadFromAtlas");
115: 		Patch&lt;ModItemDirectory&gt;(h, "InitDirectory", typeFromHandle2, null, "AfterInitDirectory");
116: 		Patch&lt;GameItem&gt;(h, "ActivateSlotItem", typeFromHandle, "BeforeActivateItem");
117: 		Patch&lt;PaperUIManager&gt;(h, "CloseAllPaperUI", typeFromHandle, null, "AfterCloseAllPaper");
118: 		Patch&lt;GameItem&gt;(h, "MayActivateSlotItem", typeFromHandle, null, "AfterCanActivate");
119: 		Patch&lt;PowerHelper&gt;(h, "ChargeBattery", typeFromHandle, "BeforeChargeBattery", "AfterBatteryChange", new Type[1] { typeof(GameItem) });
120: 		Patch&lt;PowerHelper&gt;(h, "UpdateEnergyCreditSprite", typeFromHandle, null, "AfterBatteryChange", new Type[1] { typeof(GameItem) });
121: 		Patch&lt;PowerHelper&gt;(h, "SetPowerSourceAt", typeFromHandle, null, "AfterBatteryChange", new Type[2]
122: 		{
123: 			typeof(GameItem),
124: 			typeof(int)
125: 		});
126: 		Patch&lt;PowerHelper&gt;(h, "DrawPowerSource", typeFromHandle, null, "AfterBatteryChange", new Type[2]
127: 		{
128: 			typeof(GameItem),
129: 			typeof(int)
130: 		});
131: 		Patch&lt;PowerHelper&gt;(h, "ChargeBattery", typeFromHandle, "BeforeChargeBattery", "AfterBatteryChange", new Type[3]
132: 		{
133: 			typeof(GameItem),
134: 			typeof(int),
135: 			typeof(bool)
136: 		});
137: 		Patch&lt;GameItem&gt;(h, "CanActivateSlotItem", typeFromHandle, null, "AfterCanActivate");
138: 		Patch&lt;ScavHelper&gt;(h, "GetMaxScavAttempts", typeFromHandle, null, "AfterMaxScav");
139: 		Patch&lt;ScavHelper&gt;(h, "GetMinorWoundChance", typeFromHandle, null, "AfterMinorWoundChance");
140: 		Patch&lt;ScavHelper&gt;(h, "GetMajorWoundChance", typeFromHandle, null, "AfterWoundChance");
141: 		Patch&lt;HealthData&gt;(h, "GetTotalAddictionPenalty", typeFromHandle, null, "AfterAddictionPenalty");
142: 		Patch&lt;HealthData&gt;(h, "GetLuck", typeFromHandle, null, "AfterGetLuck");
143: 		Patch&lt;PlayerStore&gt;(h, "OnCustomerReported", typeFromHandle, "BeforeReported", "AfterReported");
144: 		Patch&lt;HealthData&gt;(h, "HasNicotineDebuff", typeFromHandle, null, "AfterNicotineDebuff");
145: 		Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
146: 		Patch&lt;StoreClient&gt;(h, "GetTradeRepMultiplier", typeFromHandle, null, "AfterTradeRepMultiplier", new Type[1] { typeof(bool) });
147: 		Patch&lt;PlayerStore&gt;(h, "HandleAfterhourDebuff", typeFromHandle, "BeforeAfterhourDebuff");
148: 		Patch&lt;StorePhoneClient&gt;(h, "ShownInPhoneBook", typeFromHandle, null, "AfterShownInPhoneBook");
149: 		Patch&lt;AmenitiesItemDirectory&gt;(h, "OnPortableWaterPurifierUsed", typeFromHandle, "BeforePurifierUsed", null, new Type[2]
150: 		{
151: 			typeof(GameItem),
152: 			typeof(GameItem)
153: 		});
154: 		Patch&lt;PhoneUIManager&gt;(h, "OpenUI", typeFromHandle, null, "AfterPhoneOpen");
155: 		Type typeFromHandle3 = typeof(InputShield);
156: 		Patch&lt;InputActionManager&gt;(h, "Update", typeFromHandle3, "BeforeGameInput");
157: 		Patch&lt;CustomEventHandler&gt;(h, "Update", typeFromHandle3, "BeforeDoubleClick");
158: 		Patch&lt;StoreUIManager&gt;(h, "CheckRaycast", typeFromHandle3, "BeforeStoreRaycast");
159: 		Patch&lt;StoreUIManager&gt;(h, "Update", typeFromHandle3, null, "AfterStoreUpdate");
160: 		Patch&lt;StoreUIManager&gt;(h, "ResolveEscape", typeFromHandle3, "BeforeEscape");
161: 		Patch&lt;GraphicRaycaster&gt;(h, "Raycast", typeFromHandle3, "BeforeGraphicRaycast", null, new Type[2]
162: 		{
163: 			typeof(PointerEventData),
164: 			typeof(List&lt;RaycastResult&gt;)
165: 		});
166: 	}
167: 
168: 	public static void InstallResources(Harmony h)
169: 	{
170: 		if (!GameResources.Ready)
171: 		{
172: 			throw new InvalidOperationException("液体补丁资源尚未就绪");
173: 		}
174: 		InstallGameplay(h);
175: 		Patch&lt;WaterFeatureHelper&gt;(h, "UpdateWaterFeature", typeof(SilverBlood), null, "AfterUpdateWaterFeature", new Type[2]
176: 		{
177: 			typeof(GameItem),
178: 			typeof(bool)
179: 		});
180: 		Patch&lt;WaterFeatureHelper&gt;(h, "OnLiquidChanged", typeof(SilverBlood), null, "AfterLiquidChanged", new Type[2]
181: 		{
182: 			typeof(GameItem),
183: 			typeof(bool)
184: 		});
185: 		Patch&lt;WaterFeatureHelper&gt;(h, "OnLiquidContainerPourInto", typeof(SilverBlood), null, "AfterPourInto", new Type[3]
186: 		{
187: 			typeof(GameItem),
188: 			typeof(GameItem),
189: 			typeof(bool)
190: 		});
191: 	}
192: 
193: 	private static void Patch&lt;T&gt;(Harmony h, string method, Type owner, string prefix = null, string postfix = null, Type[] args = null)
194: 	{
195: 		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
196: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
197: 		Type[] array = args ?? Type.EmptyTypes;
198: 		MethodInfo methodInfo = AccessTools.Method(typeof(T), method, array, (Type[])null);
199: 		if (methodInfo == null)
200: 		{
201: 			ModLog.Warn("找不到 " + typeof(T).Name + "." + method + "，相关功能停用");
202: 		}
203: 		else
204: 		{
205: 			h.Patch((MethodBase)methodInfo, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMe
206: 		}
207: 	}
208: 
209: 	private static void Patch(Harmony h, MethodInfo original, string label, Type owner, string prefix = null, string postfix = null)
210: 	{
211: 		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
212: 		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
213: 		if (original == null)
214: 		{
215: 			ModLog.Warn("找不到 " + label + "，相关功能停用");
216: 		}
217: 		else
218: 		{
219: 			h.Patch((MethodBase)original, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMeth
220: 		}
221: 	}
222: 
223: 	public static void InstallEvents()
224: 	{
225: 		if (_nightly == null)
226: 		{
227: 			_nightly = delegate
228: 			{
229: 				Guard("每晚服务", Services.OnNightlyServices);
230: 			};
231: 			ModHook.OnHandlingNightlyServicesEarly += _nightly;
232: 		}
233: 	}
234: 
235: 	public static void UninstallEvents()
236: 	{
237: 		if (_nightly != null)
238: 		{
239: 			ModHook.OnHandlingNightlyServicesEarly -= _nightly;
240: 			_nightly = null;
241: 		}
242: 	}
243: 
```

## CorpPresence/L.cs

Matching lines: 1; displayed context lines: 13

```text
26: 			}
27: 			_ready = true;
28: 			try
29: 			{
30: 				string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
31: 				_en = !string.IsNullOrEmpty(currentLocaleCode) && !currentLocaleCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
32: 			}
33: 			catch
34: 			{
35: 				_en = false;
36: 			}
37: 			return _en;
38: 		}
```
