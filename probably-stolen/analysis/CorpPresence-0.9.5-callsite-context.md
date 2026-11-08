# CorpPresence — targeted callsite context

Only excerpts for relevant initialization/Harmony methods; no code was executed.

## CorpPresence/Main.cs

Matching lines: 13; displayed context lines: 75

```text
36: 		_harmony = new Harmony("mod." + manifest.ID);
37: 		Settings.Init(manifest.ID);
38: 	}
39: 
40: 	public void OnEnable()
41: 	{
42: 		bool flag;
43: 		try
44: 		{
45: 			flag = _runtime.Start(EnableCore);
46: 		}
47: 		catch (Exception error)
48: 		{
49: 			ReportStartupRollback(error);
50: 			throw;
51: 		}
57: 
58: 	private void EnableCore()
59: 	{
60: 		_runtime.Own(ExternalTranslations.Reload, ExternalTranslations.Clear);
61: 		_runtime.Own(GameResources.Begin, GameResources.Reset);
62: 		_runtime.Own(Noop, LoadedItemRepair.Reset);
63: 		_runtime.Own(ItemRegistry.Install, ItemRegistry.Uninstall);
64: 		_runtime.Own(delegate
65: 		{
66: 			Hooks.InstallCore(_harmony);
67: 		}, delegate
68: 		{
69: 			Harmony harmony = _harmony;
70: 			if (harmony != null)
71: 			{
72: 				harmony.UnpatchAll(_harmony.Id);
73: 			}
74: 		});
82: 	}
83: 
84: 	private void EnableResources()
85: 	{
86: 		_runtime.Own(SilverBlood.InstallResources, SilverBlood.UninstallResources);
87: 		_runtime.Own(Tobacco.RegisterPlant, Tobacco.UnregisterPlant);
88: 		_runtime.Own(Perfume.InstallResources, Perfume.UninstallResources);
89: 		Hooks.InstallResources(_harmony);
90: 		_runtime.Own(Hooks.InstallEvents, Hooks.UninstallEvents);
91: 		_runtime.Own(CorpPresence.Malice.Malice.Install, CorpPresence.Malice.Malice.Uninstall);
92: 		_runtime.Own(Noop, CatalogPricing.Reset);
93: 		_runtime.Own(Noop, Crates.Reset);
94: 		ItemRegistry.Activate();
95: 		_runtime.Own(Noop, Tex.ReleaseUi);
96: 		_runtime.Own(Noop, Theme.ResetCachedFont);
97: 		_runtime.Own(Noop, Portraits.ResetSceneReferences);
98: 		_runtime.Own(Noop, PhoneDialogueLayout.Reset);
99: 		_runtime.Own(Noop, Notebook.Destroy);
100: 		_runtime.Own(Noop, InputShield.Destroy);
101: 		_runtime.Own(Noop, CompanyWindow.ReleaseUi);
102: 		_runtime.Own(CreateHost, DestroyHost);
103: 		ModLog.Info("公司增强 1.0.0 资源阶段已加载（人物：" + string.Join("、", Engine.All.Select((Character c) =&gt; c.Id)) + "）");
104: 	}
105: 
106: 	public void OnDisable()
107: 	{
108: 		_runtime.Stop();
109: 	}
110: 
111: 	private void ReportStartupRollback(Exception error)
112: 	{
113: 		_runtime.Stop();
114: 		try
115: 		{
116: 			ModLog.Error("公司增强初始化失败，已执行运行时回滚：" + error);
117: 		}
118: 		catch
119: 		{
120: 		}
121: 		RollbackNotice.Queue();
122: 	}
```

## CorpPresence/GameResources.cs

Matching lines: 14; displayed context lines: 78

```text
8: namespace CorpPresence;
9: 
10: internal static class GameResources
11: {
12: 	private static AsyncOperationHandle&lt;StringTable&gt; _mechanic;
13: 
14: 	private static string _requestedLocale;
15: 
16: 	private static long _generation;
17: 
18: 	public static bool Ready { get; private set; }
19: 
20: 	public static long Generation =&gt; _generation;
29: 	{
30: 		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
31: 		Ready = false;
32: 		_requestedLocale = null;
33: 		_mechanic = default(AsyncOperationHandle&lt;StringTable&gt;);
34: 	}
35: 
36: 	public static bool Poll()
37: 	{
38: 		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
39: 		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
40: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
41: 		//IL_002b: Invalid comparison between Unknown and I4
52: 		{
53: 			return true;
54: 		}
55: 		AsyncOperationHandle&lt;LocalizationSettings&gt; initializationOperation = LocalizationSettings.InitializationOperation;
56: 		if (!initializationOperation.IsValid() || !initializationOperation.IsDone)
57: 		{
58: 			return false;
59: 		}
60: 		if ((int)initializationOperation.Status != 1)
61: 		{
62: 			throw new InvalidOperationException("本地化系统初始化失败", initializationOperation.OperationException);
63: 		}
64: 		Locale selectedLocale = LocalizationSettings.SelectedLocale;
65: 		if ((Object)(object)selectedLocale == (Object)null)
66: 		{
67: 			return false;
68: 		}
73: 			return false;
74: 		}
75: 		if (_requestedLocale != text)
76: 		{
77: 			_mechanic = ((LocalizedDatabase&lt;StringTable, StringTableEntry&gt;)(object)LocalizationSettings.StringDatabase).GetTableAsync(TableReference.op_Implicit("Mechanic"), selectedLocale);
78: 			_requestedLocale = text;
79: 		}
80: 		if (!_mechanic.IsValid())
81: 		{
82: 			throw new InvalidOperationException("Mechanic 表句柄失效，停止公司初始化");
83: 		}
84: 		if (!_mechanic.IsDone)
85: 		{
86: 			return false;
87: 		}
88: 		if ((int)_mechanic.Status != 1 || (Object)(object)_mechanic.Result == (Object)null)
89: 		{
90: 			throw new InvalidOperationException("Mechanic 表加载失败", _mechanic.OperationException);
91: 		}
92: 		val = ((LocalizationTable)_mechanic.Result).LocaleIdentifier;
93: 		if (((LocaleIdentifier)(ref val)).Code != text)
94: 		{
95: 			throw new InvalidOperationException("Mechanic 表语言与游戏启动语言不符");
96: 		}
97: 		if (((DetailedLocalizationTable&lt;StringTableEntry&gt;)(object)_mechanic.Result).GetEntry("mech_condition_pure_water") == null)
98: 		{
99: 			throw new InvalidOperationException("Mechanic 表缺少 mech_condition_pure_water");
100: 		}
101: 		Ready = true;
102: 		return true;
103: 	}
104: 
105: 	private static string ResolveMenuStartupLocale()
106: 	{
107: 		return LocHelper.ResolveStartupLocale() switch
```

## CorpPresence/Hooks.cs

Matching lines: 95; displayed context lines: 254

```text
21: 
22: 	[ThreadStatic]
23: 	private static int _pendingVendingWater;
24: 
25: 	public static void InstallCore(Harmony h)
26: 	{
27: 		Type typeFromHandle = typeof(Hooks);
28: 		Type typeFromHandle2 = typeof(ItemRegistry);
29: 		Patch&lt;SaveManager&gt;(h, "DecodeNodes", typeFromHandle2, "BeforeDecode", null, new Type[1] { typeof(List&lt;SaveItemNode&gt;) });
30: 		Patch&lt;SaveManager&gt;(h, "DecodeNodes", typeof(ItemTags), null, "AfterDecode", new Type[1] { typeof(List&lt;SaveItemNode&gt;) });
31: 		Patch&lt;SaveManager&gt;(h, "EncodeNode", typeFromHandle2, "BeforeEncode", null, new Type[1] { typeof(GameItem) });
32: 		Patch&lt;PlayerStore&gt;(h, "SaveGame", typeFromHandle, "BeforeSave");
33: 		Patch&lt;PlayerStore&gt;(h, "LoadGame", typeFromHandle, "BeforeStoreTransition", "AfterLoadGame");
34: 		Patch&lt;PlayerStore&gt;(h, "StartNewGame", typeFromHandle, "BeforeStoreTransition", "ResetState");
35: 		Patch(h, AccessTools.Method(typeof(ModConfigWindow), "Open", new Type[1] { typeof(LoadedMod) }, (Type[])null), "ModConfigWindow.Open", typeof(ConfigLocalization), null, "AfterOpen");
36: 	}
37: 
38: 	private static void InstallGameplay(Harmony h)
39: 	{
40: 		Type typeFromHandle = typeof(Hooks);
41: 		Type typeFromHandle2 = typeof(ItemRegistry);
42: 		Patch&lt;PlayerStore&gt;(h, "BeginDay", typeFromHandle, null, "AfterBeginDay");
43: 		MethodInfo original = AccessTools.Method(typeof(ItemMouseDragHandler), "EndDrag", new Type[1] { typeof(bool) }, (Type[])null) ?? AccessTools.Method(typeof(ItemMouseDragHandler), "EndDrag", Type.EmptyTypes, (Type[])null);
44: 		Patch(h, original, "ItemMouseDragHandler.EndDrag", typeof(DragFix), "BeforeEndDrag", "AfterEndDrag");
45: 		Patch&lt;StoreClientManager&gt;(h, "GenerateClient", typeFromHandle, null, "AfterGenerateClient");
46: 		Patch&lt;PlayerStore&gt;(h, "GetNextClient", typeFromHandle, "BeforeNextClient");
47: 		Patch&lt;GameItem&gt;(h, "GetNegociatedValue", typeFromHandle, null, "AfterNegociatedValue");
48: 		Patch&lt;StoreClient&gt;(h, "ClientExposeFeature", typeof(Rebels), "BeforeClientExposeFeature", null, new Type[1] { typeof(GameItem) });
49: 		Patch&lt;StoreClient&gt;(h, "OnItemSold", typeFromHandle, null, "AfterClientSold", new Type[2]
50: 		{
51: 			typeof(GameItem),
52: 			typeof(int)
53: 		});
54: 		Patch&lt;StoreClient&gt;(h, "OnItemBought", typeFromHandle, null, "AfterClientBought", new Type[2]
55: 		{
56: 			typeof(GameItem),
57: 			typeof(int)
58: 		});
59: 		Patch&lt;RunStats&gt;(h, "OnItemSold", typeFromHandle, null, "AfterCounterWaterSale", new Type[4]
60: 		{
61: 			typeof(GameItem),
62: 			typeof(int),
63: 			typeof(int),
64: 			typeof(StoreClient)
65: 		});
66: 		Patch&lt;VendingMachineHelper&gt;(h, "TrackMislabelledSale", typeFromHandle, null, "AfterVendingItemSale", new Type[2]
67: 		{
68: 			typeof(GameItem),
69: 			typeof(int)
70: 		});
71: 		Patch&lt;RunStats&gt;(h, "OnVendingSale", typeFromHandle, null, "AfterVendingSale", new Type[1] { typeof(int) });
72: 		Patch&lt;RunStats&gt;(h, "OnFountainSale", typeFromHandle, null, "AfterFountainWaterSale", new Type[1] { typeof(int) });
73: 		Patch&lt;PlayerStore&gt;(h, "EndDay", typeFromHandle, "BeforeEndDay");
74: 		Patch&lt;StorePhoneClient&gt;(h, "InitPhoneClientDict", typeFromHandle, null, "AfterInitPhones");
75: 		Patch&lt;StorePhoneClient&gt;(h, "GetCallDialog", typeFromHandle, null, "AfterGetCallDialog");
76: 		Patch&lt;PhoneUIManager&gt;(h, "WillAnswerCall", typeFromHandle, null, "AfterWillAnswer", new Type[1] { typeof(long) });
77: 		Patch&lt;PhoneUIManager&gt;(h, "HandleCall", typeFromHandle, "BeforeHandleCall", null, new Type[1] { typeof(long) });
78: 		Patch&lt;StorePhoneClient&gt;(h, "GetLocalizedDisplayName", typeFromHandle, null, "AfterPhoneName");
79: 		Patch&lt;DialoguePhoneUIManager&gt;(h, "DisplayClientText", typeof(PhoneDialogueLayout), null, "AfterClientText", new Type[1] { typeof(Dialogue) });
80: 		Patch&lt;DialoguePhoneUIManager&gt;(h, "DisplayerPlayerText", typeof(PhoneDialogueLayout), null, "AfterPlayerText", new Type[1] { typeof(Dialogue) });
81: 		MethodInfo methodInfo = AccessTools.Method(typeof(DialoguePhoneUIManager), "ApplyBubbleScale", Type.EmptyTypes, (Type[])null);
82: 		if (methodInfo != null)
83: 		{
84: 			Patch(h, methodInfo, "DialoguePhoneUIManager.ApplyBubbleScale", typeof(PhoneDialogueLayout), "Reset", "AfterBubbleScale");
85: 		}
86: 		Patch&lt;DialogUIManager&gt;(h, "ScaleByDialogSpeed", typeof(ReadingPace), null, "AfterDialoguePause", new Type[1] { typeof(float) });
87: 		Patch&lt;StoreUIManager&gt;(h, "GetLocaleSpeedMultiplier", typeof(ReadingPace), null, "AfterTypewriterSpeed");
88: 		Patch&lt;ReputationUIManager&gt;(h, "Notify", typeof(ReadingPace), null, "AfterNotification", new Type[2]
89: 		{
90: 			typeof(string),
91: 			typeof(string)
92: 		});
93: 		Patch&lt;ReputationUIManager&gt;(h, "NotifyReputation", typeof(ReadingPace), null, "AfterNotification", new Type[2]
94: 		{
95: 			typeof(string),
96: 			typeof(int)
97: 		});
98: 		Patch&lt;GameItemElement&gt;(h, "ResolveSpriteByName", typeFromHandle, "BeforeResolveSprite", null, new Type[1] { typeof(string) });
99: 		Patch&lt;GameItemElement&gt;(h, "ResolveItemSprite", typeFromHandle, "BeforeResolveItemSprite");
100: 		Patch&lt;SpriteDict&gt;(h, "GetSprite", typeFromHandle, "BeforeGetSprite", null, new Type[1] { typeof(string) });
101: 		Patch&lt;ItemConditionActionList&gt;(h, "GenuineCigaretteAction", typeof(PastelCo), null, "AfterGenuineAction", new Type[1] { typeof(GameItem) });
102: 		Patch&lt;HusbandryHelper&gt;(h, "CreateButcherCuts", typeof(Forge), "BeforeButcherCuts", "AfterButcherCuts", new Type[1] { typeof(GameItem) });
103: 		Patch&lt;GameItem&gt;(h, "BuildShapeFromSprite", typeof(Goods), "BeforeBuildShape", null, new Type[2]
104: 		{
105: 			typeof(string),
106: 			typeof(string)
107: 		});
108: 		Patch&lt;Image&gt;(h, "IsRaycastLocationValid", typeof(Goods), null, "AfterRaycastValid", new Type[2]
109: 		{
110: 			typeof(Vector2),
111: 			typeof(Camera)
112: 		});
113: 		Patch&lt;AquacultureHelper&gt;(h, "CreateFilletPieces", typeof(SilverBlood), "BeforeFillet", null, new Type[1] { typeof(GameItem) });
114: 		Patch(h, AccessTools.Method("AquacultureHelper+TankOccupancy:Fits", new Type[3]
115: 		{
116: 			typeof(int),
117: 			typeof(int),
118: 			typeof(List&lt;Vector2Int&gt;)
119: 		}, (Type[])null), "AquacultureHelper.TankOccupancy.Fits", typeof(SilverBlood), "BeforeTankFits");
120: 		Patch&lt;AquacultureHelper&gt;(h, "WillSuffocateTonight", typeof(SilverBlood), null, "AfterWillSuffocate", new Type[1] { typeof(GameItem) });
121: 		Patch&lt;AquacultureHelper&gt;(h, "ApplyWaterChecks", typeof(SilverBlood), "BeforeApplyWaterChecks", null, new Type[3]
122: 		{
123: 			typeof(List&lt;GameItem&gt;),
124: 			typeof(GameItem),
125: 			typeof(bool)
126: 		});
127: 		Patch&lt;AquacultureHelper&gt;(h, "TickTank", typeof(SilverBlood), "BeforeTickTank", "AfterTickTank", new Type[6]
128: 		{
129: 			typeof(GameItem),
130: 			typeof(GameGridInventory),
131: 			typeof(GameItem),
132: 			typeof(List&lt;GameSlotInventory&gt;),
133: 			typeof(GameSlotInventory),
134: 			typeof(bool)
135: 		});
136: 		Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction", typeof(SilverBlood), null, "AfterTemperatureReaction");
137: 		Patch(h, AccessTools.Method(typeof(ChemicalHelper), "HandleTemperatureReaction", new Type[1] { typeof(GameItem) }, (Type[])null), "ChemicalHelper.HandleTemperatureReaction.Perfume", typeof(Perfume), null, "AfterTemperatureReaction");
138: 		Patch&lt;MaterialDirectory&gt;(h, "MetalIngot", typeof(MachineUpgrade), null, "AfterMakeIngot");
139: 		Patch&lt;MachineHelper&gt;(h, "GetBodyStorage", typeof(Canning), null, "AfterMachineBodyStorage", new Type[2]
140: 		{
141: 			typeof(GameItem),
142: 			typeof(int)
143: 		});
144: 		Patch&lt;MaterialDirectory&gt;(h, "Electronic", typeof(MachineUpgrade), null, "AfterMakeElectronic");
145: 		Patch&lt;HealthData&gt;(h, "IsSentinel", typeof(SilverBlood), null, "AfterIsSentinel");
146: 		Patch&lt;StartingPerk&gt;(h, "IsSentinel", typeof(SilverBlood), null, "AfterIsSentinel");
147: 		Patch&lt;StartingPerk&gt;(h, "IsPerkActive", typeof(SilverBlood), null, "AfterIsPerkActive", new Type[1] { typeof(string) });
148: 		Patch&lt;StoreClient&gt;(h, "IsClientBuyingThisItem", typeof(SilverBlood), null, "AfterIsBuying", new Type[1] { typeof(GameItem) });
149: 		Patch&lt;CigaretteUI&gt;(h, "OnInit", typeof(PastelCo), null, "AfterCigaretteUI", new Type[1] { typeof(GameItem) });
150: 		Patch&lt;GameItem&gt;(h, "GetParentShapeModifier", typeof(RifleMods), null, "AfterParentShape", new Type[4]
151: 		{
152: 			typeof(GameItem),
153: 			typeof(SlotMarker),
154: 			typeof(GridShape),
155: 			typeof(TagSystem)
156: 		});
157: 		Patch(h, AccessTools.Method(typeof(RenderHandler), "LoadFromAtlasRaw", new Type[2]
158: 		{
159: 			typeof(string),
160: 			typeof(string)
161: 		}, (Type[])null), "RenderHandler.LoadFromAtlasRaw", typeFromHandle, "BeforeLoadFromAtlas");
162: 		Patch&lt;ModItemDirectory&gt;(h, "InitDirectory", typeFromHandle2, null, "AfterInitDirectory");
163: 		Patch&lt;GameItem&gt;(h, "ActivateSlotItem", typeFromHandle, "BeforeActivateItem");
164: 		Patch&lt;PaperUIManager&gt;(h, "CloseAllPaperUI", typeFromHandle, null, "AfterCloseAllPaper");
165: 		Patch&lt;GameItem&gt;(h, "MayActivateSlotItem", typeFromHandle, null, "AfterCanActivate");
166: 		Patch&lt;PowerHelper&gt;(h, "ChargeBattery", typeFromHandle, "BeforeChargeBattery", "AfterBatteryChange", new Type[1] { typeof(GameItem) });
167: 		Patch&lt;PowerHelper&gt;(h, "UpdateEnergyCreditSprite", typeFromHandle, null, "AfterBatteryChange", new Type[1] { typeof(GameItem) });
168: 		Patch&lt;PowerHelper&gt;(h, "SetPowerSourceAt", typeFromHandle, null, "AfterBatteryChange", new Type[2]
169: 		{
170: 			typeof(GameItem),
171: 			typeof(int)
172: 		});
173: 		Patch&lt;PowerHelper&gt;(h, "DrawPowerSource", typeFromHandle, null, "AfterBatteryChange", new Type[2]
174: 		{
175: 			typeof(GameItem),
176: 			typeof(int)
177: 		});
178: 		Patch&lt;PowerHelper&gt;(h, "ChargeBattery", typeFromHandle, "BeforeChargeBattery", "AfterBatteryChange", new Type[3]
179: 		{
180: 			typeof(GameItem),
181: 			typeof(int),
182: 			typeof(bool)
183: 		});
184: 		Patch&lt;GameItem&gt;(h, "CanActivateSlotItem", typeFromHandle, null, "AfterCanActivate");
185: 		Patch&lt;ScavHelper&gt;(h, "GetMaxScavAttempts", typeFromHandle, null, "AfterMaxScav");
186: 		Patch&lt;ScavHelper&gt;(h, "GetMinorWoundChance", typeFromHandle, null, "AfterMinorWoundChance");
187: 		Patch&lt;ScavHelper&gt;(h, "GetMajorWoundChance", typeFromHandle, null, "AfterWoundChance");
188: 		Patch&lt;HealthData&gt;(h, "GetTotalAddictionPenalty", typeFromHandle, null, "AfterAddictionPenalty");
189: 		Patch&lt;HealthData&gt;(h, "GetLuck", typeFromHandle, null, "AfterGetLuck");
190: 		Patch&lt;PlayerStore&gt;(h, "OnCustomerReported", typeFromHandle, "BeforeReported", "AfterReported");
191: 		Patch&lt;HealthData&gt;(h, "HasNicotineDebuff", typeFromHandle, null, "AfterNicotineDebuff");
192: 		Patch(h, AccessTools.Method(typeof(ShowcaseHelper), "HandleDisplayCase", Type.EmptyTypes, (Type[])null), "ShowcaseHelper.HandleDisplayCase", typeFromHandle, null, "AfterDisplayCase");
193: 		Patch&lt;StoreClient&gt;(h, "GetTradeRepMultiplier", typeFromHandle, null, "AfterTradeRepMultiplier", new Type[1] { typeof(bool) });
194: 		Patch&lt;PlayerStore&gt;(h, "HandleAfterhourDebuff", typeFromHandle, "BeforeAfterhourDebuff");
195: 		Patch&lt;StorePhoneClient&gt;(h, "ShownInPhoneBook", typeFromHandle, null, "AfterShownInPhoneBook");
196: 		Patch&lt;AmenitiesItemDirectory&gt;(h, "OnPortableWaterPurifierUsed", typeFromHandle, "BeforePurifierUsed", null, new Type[2]
197: 		{
198: 			typeof(GameItem),
199: 			typeof(GameItem)
200: 		});
201: 		Patch&lt;PhoneUIManager&gt;(h, "OpenUI", typeFromHandle, null, "AfterPhoneOpen");
202: 		Type typeFromHandle3 = typeof(InputShield);
203: 		Patch&lt;InputActionManager&gt;(h, "Update", typeFromHandle3, "BeforeGameInput");
204: 		Patch&lt;CustomEventHandler&gt;(h, "Update", typeFromHandle3, "BeforeDoubleClick");
205: 		Patch&lt;StoreUIManager&gt;(h, "CheckRaycast", typeFromHandle3, "BeforeStoreRaycast");
206: 		Patch&lt;StoreUIManager&gt;(h, "Update", typeFromHandle3, null, "AfterStoreUpdate");
207: 		Patch&lt;StoreUIManager&gt;(h, "ResolveEscape", typeFromHandle3, "BeforeEscape");
208: 		Patch&lt;GraphicRaycaster&gt;(h, "Raycast", typeFromHandle3, "BeforeGraphicRaycast", null, new Type[2]
209: 		{
210: 			typeof(PointerEventData),
211: 			typeof(List&lt;RaycastResult&gt;)
212: 		});
213: 	}
214: 
215: 	public static void InstallResources(Harmony h)
216: 	{
217: 		if (!GameResources.Ready)
218: 		{
219: 			throw new InvalidOperationException("液体补丁资源尚未就绪");
220: 		}
221: 		InstallGameplay(h);
222: 	}
223: 
224: 	private static void Patch&lt;T&gt;(Harmony h, string method, Type owner, string prefix = null, string postfix = null, Type[] args = null)
225: 	{
226: 		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
227: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
228: 		Type[] array = args ?? Type.EmptyTypes;
229: 		MethodInfo methodInfo = AccessTools.Method(typeof(T), method, array, (Type[])null);
230: 		if (methodInfo == null)
231: 		{
232: 			ModLog.Warn("找不到 " + typeof(T).Name + "." + method + "，相关功能停用");
233: 		}
234: 		else
235: 		{
236: 			h.Patch((MethodBase)methodInfo, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMe
237: 		}
238: 	}
239: 
240: 	private static void Patch(Harmony h, MethodInfo original, string label, Type owner, string prefix = null, string postfix = null)
241: 	{
242: 		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
243: 		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
244: 		if (original == null)
245: 		{
246: 			ModLog.Warn("找不到 " + label + "，相关功能停用");
247: 		}
248: 		else
249: 		{
250: 			h.Patch((MethodBase)original, (prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, prefix, (Type[])null), (postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(owner, postfix, (Type[])null), (HarmonyMethod)null, (HarmonyMeth
251: 		}
252: 	}
253: 
254: 	public static void InstallEvents()
255: 	{
256: 		if (_nightly == null)
257: 		{
258: 			_nightly = delegate
259: 			{
260: 				Guard("每晚服务", Services.OnNightlyServices);
261: 			};
262: 			ModHook.OnHandlingNightlyServicesEarly += _nightly;
263: 		}
264: 	}
265: 
266: 	public static void UninstallEvents()
267: 	{
268: 		if (_nightly != null)
269: 		{
270: 			ModHook.OnHandlingNightlyServicesEarly -= _nightly;
271: 			_nightly = null;
272: 		}
273: 	}
274: 
```

## CorpPresence/L.cs

Matching lines: 59; displayed context lines: 221

```text
3: using UnityEngine;
4: 
5: namespace CorpPresence;
6: 
7: internal static class L
8: {
9: 	private static int _frame = -1;
10: 
11: 	private static bool _en;
12: 
13: 	private static bool _ready;
14: 
15: 	private static string _locale = "";
16: 
17: 	private static int _translationVersion = -1;
18: 
19: 	public static bool En
20: 	{
21: 		get
22: 		{
23: 			int version = ExternalTranslations.Version;
24: 			if (_frame == Time.frameCount && _translationVersion == version)
25: 			{
26: 				return _en;
27: 			}
28: 			_frame = Time.frameCount;
29: 			_translationVersion = version;
30: 			if (!_ready && (Object)(object)FontLoader.Instance == (Object)null)
31: 			{
32: 				_locale = "";
33: 				return _en = false;
34: 			}
35: 			_ready = true;
36: 			try
37: 			{
38: 				_locale = ExternalTranslations.ResolveLocale(LocHelper.GetCurrentLocaleCode() ?? "");
39: 				_en = _locale.Length != 0 && !TextTranslations.IsBuiltInLocale(_locale, "zh");
40: 			}
41: 			catch
42: 			{
43: 				_locale = "";
44: 				_en = false;
45: 			}
46: 			return _en;
47: 		}
48: 	}
49: 
50: 	internal static string Locale
51: 	{
52: 		get
53: 		{
54: 			_ = En;
55: 			return _locale;
56: 		}
57: 	}
58: 
59: 	public static string T(string zh, string en)
60: 	{
61: 		if (!En)
62: 		{
63: 			return zh;
64: 		}
65: 		return TextTranslations.Select(en, _locale);
66: 	}
67: 
68: 	public static string K(string key, string zh, string en)
69: 	{
70: 		if (En)
71: 		{
72: 			return TextTranslations.SelectKey(key, en, _locale);
73: 		}
74: 		if (!ExternalTranslations.TryGet(key, en, _locale, out var value))
75: 		{
76: 			return zh;
77: 		}
78: 		return value;
79: 	}
80: 
81: 	internal static string K(string key, string en)
82: 	{
83: 		return TextTranslations.SelectKey(key, en, Locale);
84: 	}
85: 
86: 	internal static string K(string key, FormattableString en)
87: 	{
88: 		string locale = Locale;
89: 		if (!ExternalTranslations.TryFormat(key, en, locale, out var value))
90: 		{
91: 			return FormatBuiltIn(key, en, locale);
92: 		}
93: 		return value;
94: 	}
95: 
96: 	private static string FormatBuiltIn(string key, FormattableString en, string locale)
97: 	{
98: 		string text = TextTranslations.SelectBuiltInKey(key, en.Format, locale);
99: 		if (string.Equals(text, en.Format, StringComparison.Ordinal))
100: 		{
101: 			return en.ToString();
102: 		}
103: 		try
104: 		{
105: 			return string.Format(text, en.GetArguments());
106: 		}
107: 		catch (FormatException)
108: 		{
109: 			return en.ToString();
110: 		}
111: 	}
112: 
113: 	public static string K(string key, string zh, FormattableString en)
114: 	{
115: 		bool en2 = En;
116: 		if (ExternalTranslations.TryFormat(key, en, _locale, out var value))
117: 		{
118: 			return value;
119: 		}
120: 		if (!en2)
121: 		{
122: 			return zh;
123: 		}
124: 		return FormatBuiltIn(key, en, _locale);
125: 	}
126: 
127: 	public static string K(string key, FormattableString zh, FormattableString en)
128: 	{
129: 		bool en2 = En;
130: 		if (ExternalTranslations.TryFormat(key, en, _locale, out var value))
131: 		{
132: 			return value;
133: 		}
134: 		if (!en2)
135: 		{
136: 			return zh.ToString();
137: 		}
138: 		return FormatBuiltIn(key, en, _locale);
139: 	}
140: 
141: 	internal static string F(string key, string zh, string en, params object[] arguments)
142: 	{
143: 		if (En)
144: 		{
145: 			return TextTranslations.FormatKey(key, en, _locale, arguments);
146: 		}
147: 		FormattableString english = FormattableStringFactory.Create(en, arguments);
148: 		if (!ExternalTranslations.TryFormat(key, english, _locale, out var value))
149: 		{
150: 			return string.Format(zh, arguments);
151: 		}
152: 		return value;
153: 	}
154: 
155: 	internal static string KRendered(string key, string zh, FormattableString en)
156: 	{
157: 		bool en2 = En;
158: 		if (ExternalTranslations.TryFormat(key, en, _locale, out var value))
159: 		{
160: 			return value;
161: 		}
162: 		if (!en2)
163: 		{
164: 			return zh;
165: 		}
166: 		return TextTranslations.Select(en.ToString(), _locale);
167: 	}
168: 
169: 	internal static string Translate(string en)
170: 	{
171: 		return TextTranslations.Select(en, Locale);
172: 	}
173: 
174: 	internal static string Translate(FormattableString en)
175: 	{
176: 		string text = Translate(en.Format);
177: 		if (string.Equals(text, en.Format, StringComparison.Ordinal))
178: 		{
179: 			return en.ToString();
180: 		}
181: 		try
182: 		{
183: 			return string.Format(text, en.GetArguments());
184: 		}
185: 		catch (FormatException)
186: 		{
187: 			return en.ToString();
188: 		}
189: 	}
190: 
191: 	public static string[] T(string[] zh, string[] en)
192: 	{
193: 		if (!En)
194: 		{
195: 			return zh;
196: 		}
197: 		if (!TextTranslations.HasLocale(_locale) || en == null)
198: 		{
199: 			return en;
200: 		}
201: 		return Array.ConvertAll(en, (string value) =&gt; TextTranslations.Select(value, _locale));
202: 	}
203: 
204: 	internal static string[] K(string[] keys, string[] zh, string[] en)
205: 	{
206: 		bool en2 = En;
207: 		if (!en2 && (!ExternalTranslations.HasLocale(_locale) || zh == null || en == null))
208: 		{
209: 			return zh;
210: 		}
211: 		if (!TextTranslations.HasLocale(_locale) || en == null)
212: 		{
213: 			return en;
214: 		}
215: 		string[] array = new string[en.Length];
216: 		for (int i = 0; i &lt; en.Length; i++)
217: 		{
218: 			string key = ((keys != null && i &lt; keys.Length) ? keys[i] : null);
219: 			array[i] = (en2 ? TextTranslations.SelectKey(key, en[i], _locale) : (ExternalTranslations.TryGet(key, en[i], _locale, out var value) ? value : zh[i]));
220: 		}
221: 		return array;
222: 	}
223: }
```

## CorpPresence/Runtime/ResourceBootstrap.cs

Matching lines: 6; displayed context lines: 43

```text
16: 		_complete = complete;
17: 		_abort = abort;
18: 		_directory = delegate
19: 		{
20: 			Tick();
21: 		};
22: 		ModHook.OnModItemDirectoryInit += _directory;
23: 	}
24: 
25: 	public void Detach()
26: 	{
27: 		if (_directory != null)
28: 		{
29: 			ModHook.OnModItemDirectoryInit -= _directory;
30: 		}
31: 		_directory = null;
32: 		_complete = null;
33: 		_abort = null;
34: 	}
35: 
36: 	private void Update()
37: 	{
38: 		Tick();
39: 	}
40: 
41: 	public void Tick()
42: 	{
43: 		if (_complete == null)
44: 		{
45: 			return;
46: 		}
47: 		Action&lt;Exception&gt; abort = _abort;
48: 		try
49: 		{
50: 			if (GameResources.Poll())
51: 			{
52: 				Action complete = _complete;
53: 				_complete = null;
54: 				complete();
55: 			}
56: 		}
57: 		catch (Exception obj)
58: 		{
```

## CorpPresence/ItemRegistry.cs

Matching lines: 7; displayed context lines: 54

```text
21: 	private static int _reportedRegisteredCount;
22: 
23: 	private static bool _hasReportedSummary;
24: 
25: 	public static void Install()
26: 	{
27: 		if (_onInit == null)
28: 		{
29: 			_onInit = Register;
30: 			ModHook.OnModItemDirectoryInit += _onInit;
31: 		}
32: 	}
33: 
34: 	public static void Uninstall()
35: 	{
36: 		if (_onInit != null)
37: 		{
38: 			ModHook.OnModItemDirectoryInit -= _onInit;
39: 			_onInit = null;
40: 		}
41: 		if ((Object)(object)_directory != (Object)null)
42: 		{
43: 			foreach (string item in Registered)
44: 			{
45: 				if (Factories.TryGetValue(item, out var value) && ((Directory&lt;GameItem&gt;)(object)_directory).factoryDictionary.TryGetValue(item, out var value2) && (object)value == value2)
46: 				{
55: 		_reportedRegisteredCount = 0;
56: 		_hasReportedSummary = false;
57: 	}
58: 
59: 	public static void Activate()
60: 	{
61: 		if (!GameResources.Ready)
62: 		{
63: 			throw new InvalidOperationException("公司物品资源尚未就绪");
64: 		}
65: 		ModItemDirectory pendingDirectory = _pendingDirectory;
66: 		_pendingDirectory = null;
67: 		if ((Object)(object)pendingDirectory != (Object)null)
68: 		{
69: 			Register(pendingDirectory);
80: 		if ((Object)(object)directory == (Object)null)
81: 		{
82: 			return;
83: 		}
84: 		if (!GameResources.Ready)
85: 		{
86: 			_pendingDirectory = directory;
87: 			return;
88: 		}
89: 		if (_directory != directory)
90: 		{
91: 			_directory = directory;
92: 			Registered.Clear();
```


## Native game class cross-check

Static ILSpy output from user's uploaded Managed assemblies; no runtime behavior asserted unless code supports it.

### Assembly-CSharp.dll / ModLoader

- Exit: 0
Total lines: 1414, matching lines: 46
~~~text
40: 	private float nextGuardCheck;
41: 
42: 	private static readonly byte[] IModNameBytes = new byte[5] { 73, 77, 111, 100, 0 };
43: 
44: 	public static ModLoader Instance { get; private set; }
45: 
46: 	public static List&lt;LoadedMod&gt; Mods { get; private set; } = new List&lt;LoadedMod&gt;();
47: 
48: 	public static HashSet&lt;string&gt; EnabledMods { get; private set; } = new HashSet&lt;string&gt;();
49: 
50: 	public static string ModsPath =&gt; Path.Combine(Application.dataPath, "..", "Mods");
51: 
52: 	public static bool RestartRequired { get; private set; }
53: 
54: 	public static event Action OnTrustRefreshed;
55: 
56: 	public static void MarkRestartRequired()
143: 		ModTrustList.LoadCache();
144: 		ModTrustList.OnChanged -= ReapplyListVerdicts;
145: 		ModTrustList.OnChanged += ReapplyListVerdicts;
146: 		ModWorkshop.OnDetailsUpdated -= ReapplyListVerdicts;
147: 		ModWorkshop.OnDetailsUpdated += ReapplyListVerdicts;
148: 		ModSteamGuard.Install();
149: 		LoadModOrder();
150: 		LoadAllMods();
151: 		LoadWorkshopMods();
152: 		ApplySavedOrder();
153: 		ValidatePrerequisites();
154: 		LoadModAssets();
155: 		LogAudit();
156: 		StartCoroutine(ModTrustList.Fetch());
157: 	}
158: 
159: 	private void Start()
160: 	{
161: 		if (!(Instance != this) && (_rejectedLocalFolders &gt; 0 || Mods.Any((LoadedMod m) =&gt; m.LoadError != null && m.Source == ModSource.Local)) && QuantumConsole.Instance != null)
162: 		{
163: 			QuantumConsole.Instance.Activate();
164: 		}
165: 	}
166: 
167: 	private void Update()
168: 	{
169: 		if (!(Instance != this))
170: 		{
171: 			ModSteamGuard.Drain();
197: 				ex = ex3;
198: 			}
199: 		}
200: 		return ex.GetType().Name + ": " + ex.Message;
201: 	}
202: 
203: 	private static void ReportLoadError(LoadedMod mod, Exception e, string context)
204: 	{
205: 		if (mod.LoadError == null)
206: 		{
207: 			mod.LoadError = DescribeException(e);
208: 		}
209: 		Debug.LogError($"[{GetModLabel(mod.Manifest)}] Error: {DescribeException(e)} ({context})\n{e}");
210: 	}
211: 
212: 	private void LoadModOrder()
213: 	{
214: 		if (ES3.KeyExists("modOrder"))
215: 		{
216: 			_modOrder = ES3.Load&lt;List&lt;string&gt;&gt;("modOrder");
217: 			EnabledMods = new HashSet&lt;string&gt;(_modOrder);
218: 			UnityEngine.Debug.Log($"[ModLoader] {EnabledMods.Count} mod(s) enabled");
219: 		}
220: 	}
221: 
222: 	public static void SetModEnabled(string id, bool enabled)
256: 			if (!list.Contains(item))
257: 			{
258: 				list.Add(item);
259: 			}
260: 		}
261: 		_modOrder = list;
262: 		Mods.Sort(delegate(LoadedMod a, LoadedMod b)
263: 		{
264: 			int num = _modOrder.IndexOf(a.Manifest.ID);
265: 			int num2 = _modOrder.IndexOf(b.Manifest.ID);
266: 			if (num &lt; 0)
267: 			{
268: 				num = int.MaxValue;
269: 			}
270: 			if (num2 &lt; 0)
271: 			{
272: 				num2 = int.MaxValue;
384: 
385: 	private static List&lt;string&gt; ListedAssemblyNames(ModManifest manifest)
386: 	{
387: 		return manifest.Assemblies.Select(Path.GetFileNameWithoutExtension).ToList();
388: 	}
389: 
390: 	public static ModEnableCheck CheckEnable(LoadedMod mod)
391: 	{
392: 		ModEnableCheck modEnableCheck = new ModEnableCheck
393: 		{
394: 			Mod = mod
395: 		};
396: 		try
397: 		{
398: 			return CheckEnableCore(mod, modEnableCheck);
399: 		}
400: 		catch (Exception ex)
402: 			Debug.LogError($"[{GetModLabel(mod.Manifest)}] Enable check failed: {ex.Message}\n{ex}");
403: 			modEnableCheck.Refusal = LocHelper.GetLocalizedUIOrDefault("ui_mods_enable_unreadable", "{0} could not be scanned: {1}", GetModLabel(mod.Manifest), ex.Message);
404: 			return modEnableCheck;
405: 		}
406: 	}
407: 
408: 	private static ModEnableCheck CheckEnableCore(LoadedMod mod, ModEnableCheck check)
409: 	{
410: 		string modLabel = GetModLabel(mod.Manifest);
411: 		check.Fingerprint = ModFingerprint.Compute(mod.FolderPath, mod.Manifest);
412: 		if (mod.WorkshopId != 0L && !ModWorkshop.TryGetOwner(mod.WorkshopId, out var _))
413: 		{
414: 			ModWorkshop.RequestDetails(new ulong[1] { mod.WorkshopId });
415: 		}
416: 		List&lt;string&gt; list = (mod.UnlistedDlls = UnlistedAssemblyFiles(mod.FolderPath, mod.Manifest));
417: 		if (list.Count &gt; 0)
418: 		{
481: 			return check;
482: 		}
483: 		check.NeedsPrompt = mod.Manifest.Capabilities.Count &gt; 0 && check.Verdict != ModListVerdict.Verified;
484: 		return check;
485: 	}
486: 
487: 	public static void ApproveAndEnable(LoadedMod mod, ModEnableCheck check)
488: 	{
489: 		if (check != null && check.Refusal == null && check.Mod == mod)
490: 		{
491: 			mod.TrustRecord = ModTrust.Approve(mod, check.Fingerprint, check.Scan.Capabilities);
492: 			mod.Fingerprint = check.Fingerprint;
493: 			mod.ListVerdict = check.Verdict;
494: 			mod.LastScan = check.Scan;
495: 			mod.TrustState = ModTrustState.Match;
496: 			mod.TrustNote = null;
497: 			SetModEnabled(mod.Manifest.ID, enabled: true);
498: 			UnityEngine.Debug.Log($"[ModLoader] '{mod.Manifest.ID}' approved: hash {check.Fingerprint.Combined}, capabilities {ModCapability.FormatIds(check.Scan.Capabilities)}, list {check.Verdict}");
499: 		}
500: 	}
501: 
502: 	public static bool SetDeveloperMode(LoadedMod mod, bool on)
503: 	{
504: 		if (mod.Source != ModSource.Local)
505: 		{
506: 			return false;
507: 		}
508: 		bool num = ModTrust.SetDeveloperMode(mod.Manifest.ID, on);
509: 		if (num)
510: 		{
511: 			mod.TrustRecord = ModTrust.Get(mod.Manifest.ID);
512: 		}
579: 	}
580: 
581: 	private static void ReapplyListVerdicts()
582: 	{
583: 		try
584: 		{
585: 			foreach (LoadedMod mod in Mods)
586: 			{
587: 				if (mod.Fingerprint == null || mod.TrustState == ModTrustState.Blocked || mod.TrustState == ModTrustState.Stopped || !IsModEnabled(mod.Manifest.ID))
588: 				{
589: 					continue;
590: 				}
591: 				string reason;
592: 				ModListVerdict modListVerdict = (mod.ListVerdict = ModTrustList.Check(mod.Manifest.ID, mod.Fingerprint, mod.WorkshopId, WorkshopOwner(mod.WorkshopId), out reason));
593: 				if (modListVerdict == ModListVerdict.Blocked)
594: 				{
595: 					bool flag = mod.HarmonyInstance != null;
616: 		{
617: 			Debug.LogError($"[ModLoader] Re-check against the trust list failed: {ex.Message}\n{ex}");
618: 		}
619: 		ModLoader.OnTrustRefreshed?.Invoke();
620: 	}
621: 
622: 	private static void StopRunningMod(LoadedMod mod, bool callOnDisable)
623: 	{
624: 		string modLabel = GetModLabel(mod.Manifest);
625: 		if (callOnDisable && mod.Instance != null)
626: 		{
627: 			try
628: 			{
629: 				mod.Instance.OnDisable();
630: 			}
631: 			catch (Exception ex)
632: 			{
648: 		mod.IsEnabled = false;
649: 		RestartRequired = true;
650: 	}
651: 
652: 	internal static void OnSteamGuardTripped(string modId, string call, bool processLaunch)
653: 	{
654: 		LoadedMod loadedMod = ((modId != null) ? Mods.Find((LoadedMod m) =&gt; m.Manifest.ID == modId) : null);
655: 		if (loadedMod == null)
656: 		{
657: 			Debug.LogError("[ModSteamGuard] Blocked " + call + " from code that belongs to no mod; no mod was stopped.");
658: 			return;
659: 		}
660: 		string modLabel = GetModLabel(loadedMod.Manifest);
661: 		string text = (processLaunch ? LocHelper.GetLocalizedUIOrDefault("ui_mods_note_stopped_process", "it tried to start another program, which mods are never allowed to do") : LocHelper.GetLocalizedUIOrDefault("ui_mods_note_stopped", "it tried to use y
662: 		if (loadedMod.TrustState != ModTrustState.Stopped)
663: 		{
664: 			ModFingerprint modFingerprint = loadedMod.Fingerprint ?? ModFingerprint.Compute(loadedMod.FolderPath, loadedMod.Manifest);
672: 		Debug.LogError(string.Format("[{0}] Stopped by ModSteamGuard: blocked {1} (workshop={2}, owner={3}, hash={4}). Report this mod to the developers.", modLabel, call, loadedMod.WorkshopId, WorkshopOwner(loadedMod.WorkshopId), loadedMod.Fingerprint?.Co
673: 	}
674: 
675: 	private static void LogAudit()
676: 	{
677: 		UnityEngine.Debug.Log("[ModLoader] audit: game " + Application.version + ", trust list: " + ModTrustList.Describe());
678: 		foreach (LoadedMod mod in Mods)
679: 		{
680: 			string text = mod.Fingerprint?.Combined ?? "-";
681: 			string text2 = ((mod.LastScan != null) ? ModCapability.FormatIds(mod.LastScan.Capabilities) : ((mod.TrustRecord != null) ? ModCapability.FormatIds(mod.TrustRecord.detected) : ("declared " + ModCapability.FormatIds(mod.Manifest.Capabilities))));
682: 			UnityEngine.Debug.Log(string.Format("[ModLoader] audit: {0} v{1} source={2} workshop={3} owner={4} running={5} trust={6}{7} caps={8} hash={9}{10}{11}", mod.Manifest.ID, mod.Manifest.ModVersion, mod.Source, mod.WorkshopId, WorkshopOwner(mod.Worksho
683: 		}
684: 	}
685: 
686: 	private static ModManifest ParseManifest(string folderPath)
687: 	{
688: 		string problem;
828: 		}
829: 		string[] directories = System.IO.Directory.GetDirectories(ModsPath);
830: 		UnityEngine.Debug.Log($"[ModLoader] Found {directories.Length} mod folder(s) in {ModsPath}");
831: 		string[] array = directories;
832: 		foreach (string folderPath in array)
833: 		{
834: 			LoadModFromFolder(folderPath, ModSource.Local, 0uL);
835: 		}
836: 		UnityEngine.Debug.Log($"[ModLoader] === {Mods.Count} mod(s) registered, {Mods.Count((LoadedMod m) =&gt; m.IsEnabled)} enabled ===");
837: 	}
838: 
839: 	private void ApplySavedOrder()
840: 	{
841: 		if (_modOrder.Count == 0)
842: 		{
843: 			return;
844: 		}
845: 		Mods.Sort(delegate(LoadedMod a, LoadedMod b)
846: 		{
847: 			int num = _modOrder.IndexOf(a.Manifest.ID);
848: 			int num2 = _modOrder.IndexOf(b.Manifest.ID);
849: 			if (num &lt; 0)
850: 			{
851: 				num = int.MaxValue;
852: 			}
853: 			if (num2 &lt; 0)
854: 			{
855: 				num2 = int.MaxValue;
867: 		}
868: 		ModWorkshop.EnsureCallbacks();
869: 		List&lt;(ulong, string)&gt; installedItems = ModWorkshop.GetInstalledItems();
870: 		UnityEngine.Debug.Log($"[ModLoader] Found {installedItems.Count} installed Workshop item(s), {ModWorkshop.PendingDownloads} still downloading");
871: 		foreach (var (workshopId, folderPath) in installedItems)
872: 		{
873: 			LoadModFromFolder(folderPath, ModSource.Workshop, workshopId);
874: 		}
875: 		ModWorkshop.RequestDetails(installedItems.Select&lt;(ulong, string), ulong&gt;(((ulong id, string folder) i) =&gt; i.id));
876: 	}
877: 
878: 	public static void ForgetMod(string id)
879: 	{
880: 		SetModEnabled(id, enabled: false);
881: 		Mods.RemoveAll((LoadedMod m) =&gt; m.Manifest.ID == id);
882: 	}
883: 
884: 	public static List&lt;string&gt; ListedAssemblyPaths(string folderPath, ModManifest manifest)
885: 	{
886: 		return manifest.Assemblies.Select((string a) =&gt; Path.GetFullPath(Path.Combine(folderPath, a))).ToList();
887: 	}
888: 
889: 	public static List&lt;string&gt; UnlistedAssemblyFiles(string folderPath, ModManifest manifest)
890: 	{
891: 		HashSet&lt;string&gt; listed = new HashSet&lt;string&gt;(ListedAssemblyPaths(folderPath, manifest), StringComparer.OrdinalIgnoreCase);
967: 		{
968: 			_rejectedLocalFolders++;
969: 		}
970: 		return false;
971: 	}
972: 
973: 	private void LoadModFromFolder(string folderPath, ModSource source = ModSource.Local, ulong workshopId = 0uL)
974: 	{
975: 		ModManifest manifest = ParseManifest(folderPath);
976: 		if (manifest == null)
977: 		{
978: 			return;
979: 		}
980: 		LoadedMod loadedMod = Mods.FirstOrDefault((LoadedMod m) =&gt; m.Manifest.ID == manifest.ID);
981: 		if (loadedMod != null && loadedMod.FolderPath != folderPath)
982: 		{
983: 			int num;
984: 			if (loadedMod.Source == ModSource.Local)
985: 			{
986: 				num = ((source == ModSource.Workshop) ? 1 : 0);
987: 				if (num != 0 && loadedMod.ShadowedWorkshopId == 0L)
988: 				{
989: 					loadedMod.ShadowedWorkshopId = workshopId;
990: 				}
1007: 			if (list.Count &gt; 0)
1008: 			{
1009: 				Debug.LogWarning(string.Format("[{0}] {1} DLL(s) in the mod folder are not listed in &lt;Assemblies&gt; and are never loaded: {2}", GetModLabel(manifest), list.Count, string.Join(", ", list)));
1010: 			}
1011: 			if (!num2)
1012: 			{
1013: 				LoadedMod loadedMod2 = new LoadedMod
1014: 				{
1015: 					Manifest = manifest,
1016: 					FolderPath = folderPath,
1017: 					IsEnabled = false,
1018: 					Source = source,
1019: 					WorkshopId = workshopId,
1020: 					UnlistedDlls = list
1021: 				};
1022: 				if (ModTrust.Stopped.Any((ModTrust.StoppedEntry s) =&gt; s.id == manifest.ID))
1023: 				{
1052: 					State = ModTrustState.Changed,
1053: 					Note = LocHelper.GetLocalizedUIOrDefault("ui_mods_note_check_failed", "its files could not be checked")
1054: 				};
1055: 			}
1056: 			if (!trustGate.Load)
1057: 			{
1058: 				Mods.Add(new LoadedMod
1059: 				{
1060: 					Manifest = manifest,
1061: 					FolderPath = folderPath,
1062: 					IsEnabled = false,
1063: 					Source = source,
1064: 					WorkshopId = workshopId,
1065: 					UnlistedDlls = list,
1066: 					TrustState = trustGate.State,
1067: 					TrustNote = trustGate.Note,
1068: 					Fingerprint = trustGate.Fingerprint,
1078: 			try
1079: 			{
1080: 				foreach (string item2 in ListedAssemblyPaths(folderPath, manifest))
1081: 				{
1082: 					try
1083: 					{
1084: 						LoadModDll(item2, folderPath, manifest, source, workshopId);
1085: 					}
1086: 					catch (Exception item)
1087: 					{
1088: 						list2.Add((Path.GetFileName(item2), item));
1089: 					}
1090: 				}
1091: 			}
1092: 			finally
1093: 			{
1094: 				loadingMod = null;
1095: 			}
1096: 			LoadedMod loadedMod3 = Mods.LastOrDefault((LoadedMod m) =&gt; m.Manifest == manifest);
1097: 			if (loadedMod3 == null)
1098: 			{
1099: 				loadedMod3 = new LoadedMod
1100: 				{
1101: 					Manifest = manifest,
1102: 					FolderPath = folderPath,
1103: 					IsEnabled = false,
1104: 					Source = source,
1105: 					WorkshopId = workshopId
1106: 				};
1107: 				Mods.Add(loadedMod3);
1108: 			}
1109: 			loadedMod3.UnlistedDlls = list;
1126: 				loadedMod3.LoadError = "No class in the mod's DLLs implements IMod";
1127: 				Debug.LogError("[" + GetModLabel(manifest) + "] Error: " + loadedMod3.LoadError + ". The mod is not loaded.");
1128: 			}
1129: 		}
1130: 	}
1131: 
1132: 	private void LoadModDll(string dllPath, string modFolder, ModManifest manifest, ModSource source = ModSource.Local, ulong workshopId = 0uL)
1133: 	{
1134: 		string fileName = Path.GetFileName(dllPath);
1135: 		UnityEngine.Debug.Log("[ModLoader] Loading: " + fileName);
1136: 		AssemblyName assemblyName = AssemblyName.GetAssemblyName(dllPath);
1137: 		if (IsGameAssemblyName(assemblyName.Name))
1138: 		{
1139: 			throw new InvalidOperationException(fileName + " has the assembly name '" + assemblyName.Name + "', which belongs to the game or its frameworks; a mod cannot ship its own copy");
1140: 		}
1141: 		Assembly assembly = Assembly.LoadFrom(dllPath);
1142: 		List&lt;Type&gt; list = (from t in assembly.GetTypes()
1159: 		try
1160: 		{
1161: 			harmony.PatchAll(assembly);
1162: 			int num = harmony.GetPatchedMethods().Count();
1163: 			UnityEngine.Debug.Log($"[ModLoader]   Harmony: {num} method(s) patched");
1164: 			mod.Init(manifest);
1165: 			mod.OnEnable();
1166: 			flag = true;
1167: 		}
1168: 		catch (Exception ex2)
1169: 		{
1170: 			ex = ex2;
1171: 			try
1172: 			{
1173: 				harmony.UnpatchAll(text);
1174: 			}
1175: 			catch (Exception ex3)
1176: 			{
1177: 				Debug.LogWarning("[ModLoader] Cleanup unpatch failed for '" + manifest.Name + "': " + ex3.Message);
1178: 			}
1179: 			harmony = null;
1180: 		}
1181: 		LoadedMod loadedMod = new LoadedMod
1182: 		{
1183: 			Instance = mod,
1184: 			Manifest = manifest,
1185: 			Assembly = assembly,
1186: 			HarmonyInstance = harmony,
1187: 			FolderPath = modFolder,
1188: 			DllPath = dllPath,
1189: 			IsEnabled = flag,
1190: 			Source = source,
1191: 			WorkshopId = workshopId
1200: 
1201: 	private void ValidatePrerequisites()
1202: 	{
1203: 		HashSet&lt;string&gt; hashSet = new HashSet&lt;string&gt;(from m in Mods
...trimmed...
~~~

### Assembly-CSharp.dll / WaterFeatureHelper

- Exit: 0
Total lines: 603, matching lines: 31
~~~text
1: using System;
2: 
3: public class WaterFeatureHelper
4: {
5: 	public static string PURE_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_pure_water");
6: 
7: 	public static string HIGH_QUALITY_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_high_quality_water");
8: 
9: 	public static string BASE_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_basewater");
10: 
11: 	public static string GHOSTWATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_ghostwater");
12: 
13: 	public static string RUST_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_rustwater");
14: 
15: 	public static string GUTTERFLOW_WATER_DISPLAY = LocHelper.GetLocalizedMechanic("mech_condition_gutterflow");
16: 
17: 	public static string[] WATER_DISPLAY = new string[6] { PURE_WATER_DISPLAY, HIGH_QUALITY_WATER_DISPLAY, BASE_WATER_DISPLAY, GHOSTWATER_DISPLAY, RUST_WATER_DISPLAY, GUTTERFLOW_WATER_DISPLAY };
18: 
19: 	public static readonly LocalizedLines WATER_DISPLAY_LOCALIZED = LocalizedLines.Mechanic("mech_condition_pure_water", "mech_condition_high_quality_water", "mech_condition_basewater", "mech_condition_ghostwater", "mech_condition_rustwater", "mech_cond
20: 
21: 	public const int PURE_WATER_INDEX = 0;
22: 
23: 	public const int HIGH_QUALITY_WATER_INDEX = 1;
24: 
25: 	public const int BASE_WATER_INDEX = 2;
26: 
27: 	public const int GHOST_WATER_INDEX = 3;
28: 
29: 	public const int RUST_WATER_INDEX = 4;
78: 	{
79: 		if (!gameItem.IsTag("WINE_CONTAINER_TAG"))
80: 		{
81: 			int totalVolume = WaterHelper.GetTotalVolume(gameItem);
82: 			_ = GetWaterFeature(gameItem).customIntValue1;
83: 			gameItem.unitValue = GetWaterPrice(totalVolume);
84: 			UpdateWaterFeature(gameItem, GetWaterFeature(gameItem).IsExposed());
85: 			if (WaterHelper.IsEmpty(gameItem))
86: 			{
87: 				GetWaterFeature(gameItem).SetExposed(value: false);
88: 				gameItem.DisableTag("STRIP_TESTED_TAG");
89: 			}
90: 			SpriteHelper.UpdateWaterContainerSprite(gameItem);
91: 		}
92: 	}
93: 
94: 	public static void OnLiquidContainerPourInto(GameItem target, GameItem source, bool wasEmpty)
138: 			waterFeature3.SetActualDisplay(GetWaterDisplayName(waterPurity2) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(waterPurity2)] + " Purity)");
139: 			waterFeature3.fakeCondition = GetConditionFromPurity(customIntValue);
140: 			waterFeature3.realCondition = GetConditionFromPurity(waterPurity2);
141: 			target.unitValue = GetWaterPrice(totalVolume2);
142: 			flag = false;
143: 		}
144: 		UpdateWaterFeature(target, flag);
145: 		SpriteHelper.UpdateWaterContainerSprite(target);
146: 		SpriteHelper.UpdateWaterContainerSprite(source);
147: 	}
148: 
149: 	public static void OnLiquidChanged(GameItem target, bool exposeFeature)
150: 	{
151: 		WaterHelper.UpdateContainerGoodType(target);
152: 		target.DisableTag("STRIP_TESTED_TAG", skipValidate: true);
153: 		int totalVolume = WaterHelper.GetTotalVolume(target);
154: 		int customIntValue = GetWaterFeature(target).customIntValue1;
221: 
222: 	public static ItemCondition GetConditionFromPurity(int purity)
223: 	{
224: 		return ItemConditionList.conditionWaterPurity[GetPurityArrayIndex(purity)];
225: 	}
226: 
227: 	public static void UpdateWaterFeature(GameItem gameItem, bool isExpose)
228: 	{
229: 		ItemFeature waterFeature = GetWaterFeature(gameItem);
230: 		int customIntValue = waterFeature.customIntValue1;
231: 		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
232: 		int totalVolume = WaterHelper.GetTotalVolume(gameItem);
233: 		waterFeature.isDisabled = WaterHelper.IsEmpty(gameItem);
234: 		WaterHelper.ResetLastScanResult(gameItem);
235: 		if (isExpose)
236: 		{
237: 			waterFeature.customIntValue1 = waterPurity;
268: 		ItemFeature waterFeature = GetWaterFeature(gameItem);
269: 		waterFeature.customIntValue1 = presumedPurity;
270: 		waterFeature.fakeCondition = GetConditionFromPurity(presumedPurity);
271: 		waterFeature.SetPublicDisplay(GetWaterDisplayName(presumedPurity) + " (" + WATER_VALUE_DISPLAY[GetPurityArrayIndex(presumedPurity)] + " Purity)");
272: 		waterFeature.SetExposed(value: false);
273: 		gameItem.unitValue = GetWaterPrice(WaterHelper.GetTotalVolume(gameItem));
274: 		UpdateWaterFeature(gameItem, isExpose: false);
275: 	}
276: 
277: 	public static void UpdateWaterFeatureFake(GameItem gameItem, int fakePurity)
278: 	{
279: 		int totalVolume = WaterHelper.GetTotalVolume(gameItem);
280: 		int waterPrice = GetWaterPrice(totalVolume);
281: 		gameItem.unitValue = waterPrice;
282: 		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
283: 		int waterPrice2 = GetWaterPrice(totalVolume);
284: 		ItemFeature waterFeature = GetWaterFeature(gameItem);
285: 		waterFeature.fakeCondition = GetConditionFromPurity(fakePurity);
286: 		waterFeature.realCondition = GetConditionFromPurity(waterPurity);
287: 		waterFeature.customIntValue1 = fakePurity;
300: 		if (parentItem == null)
301: 		{
302: 			return null;
303: 		}
304: 		int waterPurity = WaterHelper.GetWaterPurity(parentItem);
305: 		int customIntValue = itemFeature.customIntValue1;
306: 		UpdateWaterFeatureDialog(dialogue, waterPurity, customIntValue, itemFeature);
307: 		return dialogue;
308: 	}
309: 
310: 	public static string GetWaterDisplayName(GameItem gameItem)
311: 	{
312: 		return GetWaterDisplayName(WaterHelper.GetWaterPurity(gameItem));
313: 	}
314: 
315: 	public static string GetCurrentWaterDisplay(GameItem gameItem)
316: 	{
346: 		{
347: 			if (purity &gt;= WATER_PURITIES[i])
348: 			{
349: 				return WATER_DISPLAY_LOCALIZED[i];
350: 			}
351: 		}
352: 		return LocHelper.GetLocalizedMechanic("mech_unknown_water");
353: 	}
354: 
355: 	public static int GetWaterIndex(GameItem gameItem)
356: 	{
357: 		int waterPurity = WaterHelper.GetWaterPurity(gameItem);
358: 		for (int i = 0; i &lt; WATER_PURITIES.Length; i++)
359: 		{
360: 			if (waterPurity &gt;= WATER_PURITIES[i])
361: 			{
362: 				return i;
464: 	{
465: 		if (!item.IsTag("WINE_CONTAINER_TAG") && item.IsTag("LIQUID_CONTAINER_TAG") && WaterHelper.GetTotalVolume(item) != 0)
466: 		{
467: 			ItemFeature waterFeature = GetWaterFeature(item);
468: 			if (waterFeature.IsFeatureDiscovered())
469: 			{
470: 				builder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_tested"), RenderHandler.ColorPalette.Blue);
471: 				builder.AddLine(waterFeature.GetActualDisplay());
472: 			}
473: 		}
474: 	}
475: 
476: 	public static void UpdateWaterFeatureDialog(Dialogue dialogue, int actualPurity, int presumePurity, ItemFeature itemFeature)
477: 	{
478: 		if (actualPurity == presumePurity)
479: 		{
480: 			return;
481: 		}
482: 		int purityArrayIndex = GetPurityArrayIndex(actualPurity);
483: 		if (actualPurity &gt; presumePurity)
484: 		{
485: 			dialogue.SetStartAction(delegate
486: 			{
487: 				if (PlayerStore.Instance.currentClientInstance != null)
488: 				{
489: 					StoreReputation.ModReputation(PlayerStore.Instance.currentClientInstance.GetClientBlueprint().clientFaction, 3);
490: 				}
491: 				itemFeature.ExposeFeature();
492: 			}).SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_higher_player")).NextDialogue()
493: 				.SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_higher_client"));
494: 			return;
495: 		}
496: 		dialogue.SetStartAction(delegate
497: 		{
498: 			itemFeature.ExposeFeature();
499: 		});
500: 		if (WATER_DISPLAY[purityArrayIndex] == RUST_WATER_DISPLAY)
501: 		{
502: 			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_rust_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_rust_client"));
503: 		}
504: 		else if (WATER_DISPLAY[purityArrayIndex] == GUTTERFLOW_WATER_DISPLAY)
505: 		{
506: 			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_gutter_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_gutter_client"));
507: 		}
508: 		else if (WATER_DISPLAY[purityArrayIndex] == GHOSTWATER_DISPLAY)
509: 		{
510: 			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_ghost_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_ghost_client"));
511: 		}
512: 		else if (WATER_DISPLAY[purityArrayIndex] == BASE_WATER_DISPLAY)
513: 		{
514: 			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_base_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_ghost_client"));
515: 		}
516: 		else
517: 		{
518: 			dialogue.SetText("PLAYER", LocHelper.GetLocalizedMechanic("water_incorrect_player")).NextDialogue().SetText("CLIENT", LocHelper.GetLocalizedMechanic("water_incorrect_client"));
519: 		}
520: 	}
521: 
522: 	public static void OnWrongItemPlaceWaterClient(StoreClient storeClient, GameItem targetItem, string wrongItemString = "")
523: 	{
524: 		if (storeClient.TrySayRemoveFilterLine(targetItem) || !GeneralHelper.IsItemOwned(targetItem))
525: 		{
526: 			return;
527: 		}
528: 		Dialogue dialogue = new Dialogue();
533: 			DialogUIManager.Instance.StartDialogue(dialogue);
534: 			return;
535: 		}
536: 		string currentWaterDisplay = GetCurrentWaterDisplay(targetItem);
537: 		if (currentWaterDisplay.Equals(PURE_WATER_DISPLAY))
538: 		{
539: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_pure"));
540: 		}
541: 		else if (currentWaterDisplay.Equals(HIGH_QUALITY_WATER_DISPLAY))
542: 		{
543: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_high_quality"));
544: 		}
545: 		else if (currentWaterDisplay.Equals(BASE_WATER_DISPLAY))
546: 		{
547: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_base"));
548: 		}
549: 		else if (currentWaterDisplay.Equals(GHOSTWATER_DISPLAY))
550: 		{
551: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_ghost"));
552: 		}
553: 		else if (currentWaterDisplay.Equals(RUST_WATER_DISPLAY))
554: 		{
555: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_rust"));
556: 		}
557: 		else if (currentWaterDisplay.Equals(GUTTERFLOW_WATER_DISPLAY))
558: 		{
559: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_gutter"));
560: 		}
561: 		else
562: 		{
563: 			dialogue.SetText(storeClient.displayName, LocHelper.GetLocalizedMechanic("water_wrong_default"));
564: 		}
565: 		DialogUIManager.Instance.StartDialogue(dialogue);
566: 	}
567: 
568: 	public static void OnWaterLabelUpdated(GameItem currentWaterContainer)
569: 	{
570: 		UpdateWaterFeature(currentWaterContainer, isExpose: true);
571: 	}
572: 
573: 	public static void OnWaterLabelUpdated(ItemFeature itemFeature, int dropdownValue)
574: 	{
575: 		int customIntValue = 0;
576: 		switch (dropdownValue)
577: 		{
578: 		case 0:
579: 			customIntValue = 9990;
580: 			break;
~~~

### Assembly-CSharp.dll / LocHelper

- Exit: 0
Total lines: 400, matching lines: 25
~~~text
23: 
24: 	private static string Get(string table, string key, object[] arguments)
25: 	{
26: 		string text;
27: 		try
28: 		{
29: 			text = ((arguments != null && arguments.Length != 0) ? LocalizationSettings.StringDatabase.GetLocalizedString(table, key, arguments) : LocalizationSettings.StringDatabase.GetLocalizedString(table, key, null, FallbackBehavior.UseProjectSettings));
30: 		}
31: 		catch (Exception ex)
32: 		{
33: 			Debug.LogError("[LocHelper] " + table + " / " + key + " threw while formatting: " + ex.Message);
34: 			text = null;
35: 		}
36: 		if (text != null)
37: 		{
38: 			return text;
39: 		}
40: 		string text2 = LocalizationSettings.StringDatabase.GetTable(table)?.GetEntry(key)?.Value;
41: 		Debug.LogError("&lt;color=orange&gt;[LocHelper] LOCALIZATION FORMAT FAILED: " + table + " / " + key + " " + $"(locale {GetCurrentLocaleCode()}, {((arguments != null) ? arguments.Length : 0)} argument(s)).&lt;/color&gt; " + "Showing the raw entry text instead. 
42: 		return text2 ?? key;
43: 	}
44: 
45: 	public static string GetLocalized(string table, string key)
46: 	{
47: 		return Get(table, key, null);
48: 	}
49: 
50: 	public static string GetLocalizedNewGame(string key)
51: 	{
52: 		return Get("NewGame", key, null);
53: 	}
54: 
55: 	public static string GetLocalizedMainMenu(string key, params object[] arguments)
56: 	{
57: 		return Get("MainMenu", key, arguments);
58: 	}
59: 
60: 	public static string GetLocalizedUI(string key, params object[] arguments)
61: 	{
62: 		return Get("UI", key, arguments);
63: 	}
64: 
65: 	public static bool HasLocalizedUI(string key)
66: 	{
67: 		return HasEntry("UI", key);
68: 	}
69: 
70: 	public static bool HasLocalizedNetworkUpgrade(string key)
78: 		{
79: 			return false;
80: 		}
81: 		return LocalizationSettings.StringDatabase.GetTable(table)?.GetEntry(key) != null;
82: 	}
83: 
84: 	public static string GetLocalizedOrDefault(string table, string key, string englishFallback, params object[] arguments)
85: 	{
86: 		if (HasEntry(table, key))
87: 		{
88: 			return Get(table, key, arguments);
89: 		}
90: 		if (missingKeyLogged.Add(table + "/" + key))
91: 		{
92: 			Debug.Log("[LocHelper] " + table + " table has no entry '" + key + "' yet, showing the English fallback (see docs/pending-localization.md).");
93: 		}
94: 		if (arguments != null && arguments.Length != 0)
102: 				return englishFallback;
103: 			}
104: 		}
105: 		return englishFallback;
106: 	}
107: 
108: 	public static string GetLocalizedUIOrDefault(string key, string englishFallback, params object[] arguments)
109: 	{
110: 		return GetLocalizedOrDefault("UI", key, englishFallback, arguments);
111: 	}
112: 
113: 	public static string GetLocalizedMechanicOrDefault(string key, string englishFallback, params object[] arguments)
114: 	{
115: 		return GetLocalizedOrDefault("Mechanic", key, englishFallback, arguments);
116: 	}
117: 
118: 	public static string BulletLine(string text)
119: 	{
120: 		return GetLocalizedMechanic("mech_list_item", text);
121: 	}
122: 
123: 	public static string BulletLines(string text)
124: 	{
125: 		if (string.IsNullOrEmpty(text))
126: 		{
127: 			return text;
128: 		}
129: 		List&lt;string&gt; list = new List&lt;string&gt;();
130: 		string[] array = text.Replace("\r\n", "\n").Split('\n');
136: 				list.Add(BulletLine(text2));
137: 			}
138: 		}
139: 		return string.Join("\n", list);
140: 	}
141: 
142: 	public static string GetLocalizedInteractable(string key, params object[] arguments)
143: 	{
144: 		return Get("Interactable", key, arguments);
145: 	}
146: 
147: 	public static string GetLocalizedNetworkUpgrade(string key, params object[] arguments)
148: 	{
149: 		return Get("NetworkUpgrade", key, arguments);
150: 	}
151: 
152: 	public static string GetLocalizedName(string key)
153: 	{
154: 		return Get("ClientName", key, null);
155: 	}
156: 
157: 	public static string GetLocalizedDialoguePhone(string key, params object[] arguments)
158: 	{
159: 		return Get("PhoneDialogue", key, arguments);
160: 	}
161: 
162: 	public static string GetLocalizedClientName(string key)
163: 	{
164: 		return Get("ClientName", key, null);
165: 	}
166: 
167: 	public static string GetLocalizedDialogue(string key, params object[] arguments)
168: 	{
169: 		return Get("Dialogue", key, arguments);
170: 	}
171: 
172: 	public static string GetLocalizedItem(string key, params object[] arguments)
173: 	{
174: 		return Get("Item", key, arguments);
175: 	}
176: 
177: 	public static string GetLocalizedMechanic(string key, params object[] arguments)
178: 	{
179: 		return Get("Mechanic", key, arguments);
180: 	}
181: 
182: 	public static string GetLocalizedPerkTable(string key, params object[] arguments)
183: 	{
184: 		return Get("PerkTable", key, arguments);
185: 	}
186: 
187: 	public static string GetLocalizedRepPerkTable(string key, params object[] arguments)
188: 	{
189: 		return Get("RepPerkTable", key, arguments);
190: 	}
191: 
192: 	public static bool TryParseMissingTranslation(string text, out string table, out string key)
193: 	{
194: 		table = null;
195: 		key = null;
196: 		if (string.IsNullOrEmpty(text))
197: 		{
231: 		{
232: 			return text;
233: 		}
234: 		string text2 = Get(table, key, null);
235: 		if (text2 != text)
236: 		{
237: 			Debug.Log("[LocHelper] Repaired saved missing translation " + table + " / " + key + " (locale " + GetCurrentLocaleCode() + ").");
238: 		}
239: 		return text2;
240: 	}
241: 
242: 	public static string GetCurrentLocaleCode()
243: 	{
244: 		Locale selectedLocale = LocalizationSettings.SelectedLocale;
245: 		if (!(selectedLocale != null))
246: 		{
247: 			return "";
248: 		}
249: 		return selectedLocale.Identifier.Code;
250: 	}
251: 
252: 	public static string WrapInLocalizedQuotes(string text)
253: 	{
254: 		switch (GetCurrentLocaleCode())
255: 		{
256: 		case "es":
257: 		case "it":
258: 		case "ru":
259: 		case "pl":
260: 			return "«" + text + "»";
261: 		case "fr":
262: 			return "«\u00a0" + text + "\u00a0»";
263: 		case "de":
264: 			return "»" + text + "«";
~~~

### Assembly-CSharp.dll / ModHook

- Exit: 0
Total lines: 430, matching lines: 9
~~~text
4: using UnityEngine;
5: 
6: public class ModHook : MonoBehaviour
7: {
8: 	public static ModHook Instance { get; private set; }
9: 
10: 	public static event Action&lt;ModItemDirectory&gt; OnModItemDirectoryInit;
11: 
12: 	public static event Action&lt;StoreClientManager&gt; OnGenerateCustomerVeryEarly;
13: 
14: 	public static event Action&lt;StoreClientManager&gt; OnGenerateCustomerEarly;
15: 
16: 	public static event Action&lt;StoreClientManager&gt; OnGenerateCustomerNormal;
17: 
18: 	public static event Action&lt;StoreClientManager&gt; OnGenerateCustomerLate;
19: 
20: 	public static event Action&lt;StoreClientManager&gt; OnGenerateCustomerVeryLate;
21: 
22: 	public static event Action OnGameLoadedInit;
23: 
24: 	public static event Action OnGameLoadedEarly;
25: 
26: 	public static event Action OnGameLoadedNormal;
27: 
28: 	public static event Action OnGameLoadedLate;
29: 
30: 	public static event Action OnShutterOpenedEarly;
31: 
32: 	public static event Action OnShutterOpenedLate;
33: 
34: 	public static event Action OnShutterClosedEarly;
35: 
36: 	public static event Action OnShutterClosedLate;
64: 	public static event Action&lt;RichTextBuilder, GameItem&gt; OnCreateTooltipLate;
65: 
66: 	public static event Action&lt;RichTextBuilder, GameItem&gt; OnCreateDevTooltipEarly;
67: 
68: 	public static event Action&lt;RichTextBuilder, GameItem&gt; OnCreateDevTooltipLate;
69: 
70: 	public static event Action&lt;LoadedMod&gt; OnModAssetsLoaded;
71: 
72: 	public static event Action&lt;string&gt; OnModConfigApplied;
73: 
74: 	public static event Action OnLootTablesLoaded;
75: 
76: 	public static event Action&lt;LootRollContext&gt; OnLootRolled;
77: 
78: 	public static event Action&lt;string, PaperUI&gt; OnPaperOpened;
79: 
80: 	public static event Action&lt;string&gt; OnNetworkUpgradeUnlocked;
205: 		Assembly assembly = method.DeclaringType?.Assembly;
206: 		LoadedMod loadedMod = ModSourceHelper.FindModByAssembly(assembly);
207: 		string text = ((loadedMod != null) ? loadedMod.Manifest.Name : (assembly?.GetName().Name ?? "unknown assembly"));
208: 		Debug.LogError($"[ModHook] '{hookName}' handler {method.DeclaringType?.FullName}.{method.Name} from '{text}' threw: {e}");
209: 	}
210: 
211: 	internal static void FireOnModItemDirectoryInit(ModItemDirectory directory)
212: 	{
213: 		SafeInvoke("OnModItemDirectoryInit", ModHook.OnModItemDirectoryInit, directory);
214: 	}
215: 
216: 	internal static void FireOnGenerateCustomerVeryEarly(StoreClientManager manager)
217: 	{
218: 		SafeInvokeGenerateCustomer("OnGenerateCustomerVeryEarly", ModHook.OnGenerateCustomerVeryEarly, manager);
219: 	}
220: 
221: 	internal static void FireOnGenerateCustomerEarly(StoreClientManager manager)
222: 	{
223: 		SafeInvokeGenerateCustomer("OnGenerateCustomerEarly", ModHook.OnGenerateCustomerEarly, manager);
245: 
246: 	internal static void FireOnGameLoadedEarly()
247: 	{
248: 		SafeInvoke("OnGameLoadedEarly", ModHook.OnGameLoadedEarly);
249: 	}
250: 
251: 	internal static void FireOnGameLoadedNormal()
252: 	{
253: 		SafeInvoke("OnGameLoadedNormal", ModHook.OnGameLoadedNormal);
254: 	}
255: 
256: 	internal static void FireOnGameLoadedLate()
257: 	{
258: 		SafeInvoke("OnGameLoadedLate", ModHook.OnGameLoadedLate);
259: 	}
260: 
261: 	internal static void FireOnShutterOpenedEarly()
262: 	{
263: 		SafeInvoke("OnShutterOpenedEarly", ModHook.OnShutterOpenedEarly);
355: 
356: 	internal static void FireOnCreateDevTooltipLate(RichTextBuilder builder, GameItem item)
357: 	{
358: 		SafeInvoke("OnCreateDevTooltipLate", ModHook.OnCreateDevTooltipLate, builder, item);
359: 	}
360: 
361: 	internal static void FireOnModAssetsLoaded(LoadedMod mod)
362: 	{
363: 		SafeInvoke("OnModAssetsLoaded", ModHook.OnModAssetsLoaded, mod);
364: 	}
365: 
366: 	internal static void FireOnModConfigApplied(string modId)
367: 	{
368: 		SafeInvoke("OnModConfigApplied", ModHook.OnModConfigApplied, modId);
369: 	}
370: 
371: 	internal static void FireOnLootTablesLoaded()
372: 	{
373: 		SafeInvoke("OnLootTablesLoaded", ModHook.OnLootTablesLoaded);
~~~

### Assembly-CSharp.dll / ModHelper

- Exit: 0
Total lines: 1182, matching lines: 1
~~~text
85: 
86: 	public static Locale GetCurrentLocale()
87: 	{
88: 		return LocalizationSettings.SelectedLocale;
89: 	}
90: 
91: 	public static string GetLocalized(string modId, string key, params object[] args)
92: 	{
93: 		LoadedMod loadedMod = ModLoader.Mods.Find((LoadedMod m) =&gt; m.Manifest.ID == modId);
94: 		string text = null;
95: 		if (loadedMod != null)
96: 		{
97: 			Locale selectedLocale = LocalizationSettings.SelectedLocale;
98: 			string key2 = ((selectedLocale != null) ? selectedLocale.Identifier.Code : "en");
99: 			Dictionary&lt;string, string&gt; value3;
100: 			string value4;
101: 			if (loadedMod.Localization.TryGetValue(key2, out var value) && value.TryGetValue(key, out var value2))
~~~

### Unity.Localization.dll / UnityEngine.Localization.Settings.LocalizationSettings

- Exit: 0
Total lines: 459, matching lines: 53
~~~text
41: 	private MetadataCollection m_Metadata = new MetadataCollection();
42: 
43: 	[SerializeField]
44: 	internal LocaleIdentifier m_ProjectLocaleIdentifier = "en";
45: 
46: 	[SerializeField]
47: 	private bool m_InitializeSynchronously;
48: 
49: 	internal AsyncOperationHandle&lt;LocalizationSettings&gt; m_InitializingOperationHandle;
50: 
51: 	private AsyncOperationHandle&lt;Locale&gt; m_SelectedLocaleAsync;
52: 
53: 	private Locale m_ProjectLocale;
54: 
55: 	private CallbackArray&lt;Action&lt;Locale&gt;&gt; m_SelectedLocaleChanged;
56: 
57: 	private static LocalizationSettings s_Instance;
58: 
59: 	internal bool HasSelectedLocaleChangedSubscribers =&gt; m_SelectedLocaleChanged.Length != 0;
60: 
61: 	public static bool HasSettings
62: 	{
63: 		get
64: 		{
65: 			if (s_Instance == null)
66: 			{
67: 				s_Instance = GetInstanceDontCreateDefault();
68: 			}
69: 			return s_Instance != null;
70: 		}
71: 	}
72: 
73: 	public static AsyncOperationHandle&lt;LocalizationSettings&gt; InitializationOperation =&gt; Instance.GetInitializationOperation();
74: 
75: 	public static LocalizationSettings Instance
76: 	{
77: 		get
78: 		{
79: 			if (s_Instance == null)
80: 			{
81: 				s_Instance = GetOrCreateSettings();
82: 			}
83: 			return s_Instance;
125: 			Instance.SetStringDatabase(value);
126: 		}
127: 	}
128: 
129: 	public static MetadataCollection Metadata =&gt; Instance.GetMetadata();
130: 
131: 	public static Locale SelectedLocale
132: 	{
133: 		get
134: 		{
135: 			return Instance.GetSelectedLocale();
136: 		}
137: 		set
138: 		{
139: 			Instance.SetSelectedLocale(value);
140: 		}
141: 	}
142: 
143: 	public static AsyncOperationHandle&lt;Locale&gt; SelectedLocaleAsync =&gt; Instance.GetSelectedLocaleAsync();
144: 
145: 	public static Locale ProjectLocale
146: 	{
147: 		get
148: 		{
149: 			if (Instance.m_ProjectLocale == null || Instance.m_ProjectLocale.Identifier != Instance.m_ProjectLocaleIdentifier)
150: 			{
151: 				Instance.m_ProjectLocale = AvailableLocales?.GetLocale(Instance.m_ProjectLocaleIdentifier);
152: 			}
153: 			return Instance.m_ProjectLocale;
156: 		{
157: 			Instance.m_ProjectLocale = value;
158: 			Instance.m_ProjectLocaleIdentifier = ((value != null) ? value.Identifier : default(LocaleIdentifier));
159: 		}
160: 	}
161: 
162: 	public static bool InitializeSynchronously
163: 	{
164: 		get
165: 		{
166: 			return Instance.m_InitializeSynchronously;
167: 		}
168: 		set
169: 		{
170: 			Instance.m_InitializeSynchronously = value;
171: 		}
172: 	}
173: 
174: 	internal bool IsPlayingOrWillChangePlaymode =&gt; true;
175: 
176: 	internal bool IsPlaying =&gt; Application.isPlaying;
177: 
178: 	internal virtual RuntimePlatform Platform =&gt; Application.platform;
179: 
180: 	public event Action&lt;Locale&gt; OnSelectedLocaleChanged
181: 	{
182: 		add
183: 		{
184: 			m_SelectedLocaleChanged.Add(value);
185: 		}
186: 		remove
187: 		{
188: 			m_SelectedLocaleChanged.RemoveByMovingTail(value);
189: 		}
190: 	}
191: 
192: 	public static event Action&lt;Locale&gt; SelectedLocaleChanged
193: 	{
194: 		add
195: 		{
196: 			Instance.OnSelectedLocaleChanged += value;
197: 		}
198: 		remove
199: 		{
200: 			Instance.OnSelectedLocaleChanged -= value;
201: 		}
202: 	}
203: 
204: 	private void OnEnable()
205: 	{
206: 		if (s_Instance == null)
207: 		{
208: 			s_Instance = this;
209: 		}
210: 	}
214: 		if (!HasSettings)
215: 		{
216: 			throw new Exception("There is no active LocalizationSettings.\n " + error);
217: 		}
218: 	}
219: 
220: 	public virtual AsyncOperationHandle&lt;LocalizationSettings&gt; GetInitializationOperation()
221: 	{
222: 		if (!m_InitializingOperationHandle.IsValid())
223: 		{
224: 			InitializationOperation initializationOperation = GenericPool&lt;UnityEngine.Localization.InitializationOperation&gt;.Get();
225: 			initializationOperation.Init(this);
226: 			m_InitializingOperationHandle = AddressablesInterface.ResourceManager.StartOperation(initializationOperation, default(AsyncOperationHandle));
227: 			if (!m_InitializingOperationHandle.IsDone && m_InitializeSynchronously && IsPlaying)
228: 			{
229: 				m_InitializingOperationHandle.WaitForCompletion();
230: 			}
231: 		}
232: 		return m_InitializingOperationHandle;
233: 	}
234: 
235: 	public List&lt;IStartupLocaleSelector&gt; GetStartupLocaleSelectors()
236: 	{
237: 		return m_StartupSelectors;
278: 		m_AssetDatabase?.OnLocaleChanged(locale);
279: 		if (m_InitializingOperationHandle.IsValid())
280: 		{
281: 			AddressablesInterface.SafeRelease(m_InitializingOperationHandle);
282: 			m_InitializingOperationHandle = default(AsyncOperationHandle&lt;LocalizationSettings&gt;);
283: 		}
284: 		if (GetInitializationOperation().Status == AsyncOperationStatus.Succeeded)
285: 		{
286: 			InvokeSelectedLocaleChanged(locale);
287: 		}
288: 		else
289: 		{
290: 			ComponentSingleton&lt;LocalizationBehaviour&gt;.Instance.StartCoroutine(InitializeAndCallSelectedLocaleChangedCoroutine(locale));
291: 		}
292: 	}
293: 
294: 	private IEnumerator InitializeAndCallSelectedLocaleChangedCoroutine(Locale locale)
295: 	{
296: 		yield return m_InitializingOperationHandle;
297: 		InvokeSelectedLocaleChanged(locale);
298: 	}
299: 
300: 	private void InvokeSelectedLocaleChanged(Locale locale)
301: 	{
302: 		try
303: 		{
304: 			m_SelectedLocaleChanged.LockForChanges();
305: 			int length = m_SelectedLocaleChanged.Length;
306: 			if (length == 1)
307: 			{
308: 				m_SelectedLocaleChanged.SingleDelegate(locale);
309: 			}
310: 			else if (length &gt; 1)
311: 			{
312: 				Action&lt;Locale&gt;[] multiDelegates = m_SelectedLocaleChanged.MultiDelegates;
313: 				for (int i = 0; i &lt; length; i++)
314: 				{
315: 					multiDelegates[i](locale);
316: 				}
317: 			}
318: 		}
319: 		catch (Exception exception)
320: 		{
321: 			Debug.LogException(exception);
322: 		}
323: 		m_SelectedLocaleChanged.UnlockForChanges();
324: 	}
325: 
326: 	private Locale SelectActiveLocale()
327: 	{
328: 		if (m_AvailableLocales == null)
329: 		{
330: 			Debug.LogError("AvailableLocales is null, can not pick a Locale.");
331: 			return null;
332: 		}
333: 		if (m_AvailableLocales.Locales == null)
371: 			}
372: 			Debug.LogError(value.ToString(), this);
373: 		}
374: 		return null;
375: 	}
376: 
377: 	public void SetSelectedLocale(Locale locale)
378: 	{
379: 		if (m_SelectedLocaleAsync.IsValid() && (object)m_SelectedLocaleAsync.Result == locale)
380: 		{
381: 			return;
382: 		}
383: 		GetInitializationOperation();
384: 		if ((!(locale == null) || !IsPlayingOrWillChangePlaymode) && (!m_SelectedLocaleAsync.IsValid() || (object)m_SelectedLocaleAsync.Result != locale))
385: 		{
386: 			if (m_SelectedLocaleAsync.IsValid())
387: 			{
388: 				AddressablesInterface.Release(m_SelectedLocaleAsync);
389: 			}
390: 			m_SelectedLocaleAsync = AddressablesInterface.ResourceManager.CreateCompletedOperation(locale, null);
391: 			SendLocaleChangedEvents(locale);
392: 		}
393: 	}
394: 
395: 	public virtual AsyncOperationHandle&lt;Locale&gt; GetSelectedLocaleAsync()
396: 	{
397: 		if (!m_SelectedLocaleAsync.IsValid())
398: 		{
399: 			if (m_AvailableLocales is IPreloadRequired { PreloadOperation: { IsDone: false } } preloadRequired)
400: 			{
401: 				m_SelectedLocaleAsync = AddressablesInterface.ResourceManager.CreateChainOperation(preloadRequired.PreloadOperation, (AsyncOperationHandle op) =&gt; AddressablesInterface.ResourceManager.CreateCompletedOperation(SelectActiveLocale(), null));
402: 			}
403: 			else
404: 			{
405: 				m_SelectedLocaleAsync = AddressablesInterface.ResourceManager.CreateCompletedOperation(SelectActiveLocale(), null);
406: 			}
407: 		}
408: 		return m_SelectedLocaleAsync;
409: 	}
410: 
411: 	public virtual Locale GetSelectedLocale()
412: 	{
413: 		AsyncOperationHandle&lt;Locale&gt; selectedLocaleAsync = GetSelectedLocaleAsync();
414: 		if (selectedLocaleAsync.IsDone)
415: 		{
416: 			return selectedLocaleAsync.Result;
417: 		}
418: 		return selectedLocaleAsync.WaitForCompletion();
419: 	}
420: 
421: 	public virtual void OnLocaleRemoved(Locale locale)
422: 	{
423: 		if (m_SelectedLocaleAsync.IsValid() && (object)m_SelectedLocaleAsync.Result == locale)
424: 		{
425: 			AddressablesInterface.Release(m_SelectedLocaleAsync);
426: 			m_SelectedLocaleAsync = default(AsyncOperationHandle&lt;Locale&gt;);
427: 		}
428: 	}
429: 
430: 	public void ResetState()
431: 	{
432: 		m_SelectedLocaleAsync = default(AsyncOperationHandle&lt;Locale&gt;);
433: 		m_InitializingOperationHandle = default(AsyncOperationHandle&lt;LocalizationSettings&gt;);
434: 		(m_AvailableLocales as IReset)?.ResetState();
435: 		((IReset)m_AssetDatabase)?.ResetState();
436: 		((IReset)m_StringDatabase)?.ResetState();
437: 	}
438: 
439: 	public static LocalizationSettings GetInstanceDontCreateDefault()
440: 	{
441: 		if (s_Instance != null)
442: 		{
443: 			return s_Instance;
444: 		}
445: 		return Object.FindObjectOfType&lt;LocalizationSettings&gt;();
446: 	}
447: 
448: 	private static LocalizationSettings GetOrCreateSettings()
449: 	{
450: 		LocalizationSettings localizationSettings = GetInstanceDontCreateDefault();
451: 		if (localizationSettings == null)
452: 		{
453: 			Debug.LogWarning("Could not find localization settings. Default will be used.");
454: 			localizationSettings = ScriptableObject.CreateInstance&lt;LocalizationSettings&gt;();
455: 			localizationSettings.name = "Default Localization Settings";
456: 		}
457: 		return localizationSettings;
458: 	}
459: }
~~~
