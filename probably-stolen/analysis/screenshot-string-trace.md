# Screenshot string trace

## 3832048841

### 食腐乌鸦
- not found in decompiled C#

### 破损版
- \`GameCardPK/TagDisplay.cs:24\`
~~~csharp
0021: 		Add(dictionary, "FirstEdition", DisplayType.None, "初版", (ColorPalette)4365501);
0022: 		Add(dictionary, "Memorial", DisplayType.None, "纪念版", (ColorPalette)11567781);
0023: 		Add(dictionary, "Rare", DisplayType.None, "稀有版", (ColorPalette)9672266, string.Empty, (Func<GameItem, string>?)RareSeriesLabel.Format);
0024: 		Add(dictionary, "Damaged", DisplayType.None, "破损版", (ColorPalette)9454675);
0025: 		Add(dictionary, "Tournament", DisplayType.None, "赛事版", (ColorPalette)4367018);
0026: 		Add(dictionary, "VariantArt", DisplayType.None, "异画", (ColorPalette)9672266);
0027: 		Add(dictionary, "MutantForm", DisplayType.None, "变异", (ColorPalette)8407419);
~~~
- \`GameCardPK/CardDatabase.cs:208\`
~~~csharp
0205: 			new VersionPricingEntry
0206: 			{
0207: 				Tag = "Damaged",
0208: 				Name = "破损版",
0209: 				Calc = "mul",
0210: 				Value = -20
0211: 			}
~~~

### 游戏卡
- \`GameCardPK/SlotAdmission.cs:195\`
~~~csharp
0192: 
0193: 	internal static string Describe()
0194: 	{
0195: 		return "槽位准入（官方钩子 mayInventoryAddItemFunc）：技能槽只收技能条（核心槽不许放）/ battle_card_slot 只收自己的、装过芯片且耐久够的卡 / repair_slot 只收自己的游戏卡 / 贴纸打印机的蓝图槽只收贴纸蓝图、投入格只收废纸";
0196: 	}
0197: }
~~~
- \`GameCardPK/JunkPressCycle.cs:159\`
~~~csharp
0156: 		List<GameItem> list2 = Collect(JunkPressWindow.Grid(press, output: true), IsPlayerGameCard);
0157: 		if (list2.Count == 0)
0158: 		{
0159: 			Core.Debug("[垃圾] 产出格没有游戏卡，本夜不结算（不扣电不扣料）。");
0160: 			return "产出格没有游戏卡";
0161: 		}
0162: 		int num2 = 0;
~~~
- \`GameCardPK/JunkPressCycle.cs:160\`
~~~csharp
0157: 		if (list2.Count == 0)
0158: 		{
0159: 			Core.Debug("[垃圾] 产出格没有游戏卡，本夜不结算（不扣电不扣料）。");
0160: 			return "产出格没有游戏卡";
0161: 		}
0162: 		int num2 = 0;
0163: 		if (pressSpec.PowerCost > 0)
~~~
- \`GameCardPK/ChipSystem.cs:92\`
~~~csharp
0089: 			}
0090: 			else if (!GameCardFactory.IsCard(card))
0091: 			{
0092: 				text = "落点不是游戏卡（没有 game_card tag）";
0093: 			}
0094: 			else if (!ItemOwnership.IsPlayerOwned(card, "卡牌", log: false))
0095: 			{
~~~
- \`GameCardPK/ChipSystem.cs:221\`
~~~csharp
0218: 		string text = Core.Clean(card.name);
0219: 		if (!GameCardFactory.IsCard(card))
0220: 		{
0221: 			Trace("install-notcard|" + Describe(card), "TryInstall：落点不是游戏卡：" + Core.Clean(card.identifier));
0222: 			return false;
0223: 		}
0224: 		if (!ItemOwnership.IsPlayerOwned(card, "卡牌"))
~~~
- \`GameCardPK/CardDatabase.cs:379\`
~~~csharp
0376: 		return new GameCardData
0377: 		{
0378: 			Id = ItemConfigInjector.PrefixedItem("game_card"),
0379: 			Name = "游戏卡"
0380: 		};
0381: 	}
0382: 
~~~
- \`GameCardPK/AuxEffectSystem.cs:73\`
~~~csharp
0070: 		}
0071: 		if (!GameCardFactory.IsCard(card))
0072: 		{
0073: 			Core.Log?.Msg("[辅助效果] 落点上没有 game_card 标签，不是游戏卡，不处理：" + CardIdentity.Of(card));
0074: 			return null;
0075: 		}
0076: 		if (byItemId.Target != DragReceiver.GameCard)
~~~
- \`GameCardPK/AuxEffectSystem.cs:78\`
~~~csharp
0075: 		}
0076: 		if (byItemId.Target != DragReceiver.GameCard)
0077: 		{
0078: 			Core.Log?.Error("[辅助效果] " + byItemId.Id + " 的 target 配成了 " + byItemId.Target.ToString() + "，但辅助效果只能落在游戏卡上，已忽略。");
0079: 			return null;
0080: 		}
0081: 		if (!AuxEffectTable.IsUsable(byItemId))
~~~

### 集换式卡牌
- \`GameCardPK/CardClubUnlockData.cs:7\`
~~~csharp
0004: {
0005: 	public string Id { get; set; } = "CARD_PK_CLUB";
0006: 
0007: 	public string Title { get; set; } = "集换式卡牌";
0008: 
0009: 	public string Subtitle { get; set; } = "推荐";
0010: 
~~~
- \`GameCardPK/CardFactoryConfig.cs:48\`
~~~csharp
0045: 			}
0046: 			if (string.IsNullOrWhiteSpace(cardFactoryFile.Unlock.Subtitle))
0047: 			{
0048: 				cardFactoryFile.Unlock.Subtitle = "集换式卡牌";
0049: 				Core.Debug("[卡牌厂] unlock.subtitle 为空，已按「集换式卡牌」处理（保证与集换式卡牌同组）。");
0050: 			}
0051: 			if (cardFactoryFile.Unlock.Cost < 0)
~~~
- \`GameCardPK/CardFactoryConfig.cs:49\`
~~~csharp
0046: 			if (string.IsNullOrWhiteSpace(cardFactoryFile.Unlock.Subtitle))
0047: 			{
0048: 				cardFactoryFile.Unlock.Subtitle = "集换式卡牌";
0049: 				Core.Debug("[卡牌厂] unlock.subtitle 为空，已按「集换式卡牌」处理（保证与集换式卡牌同组）。");
0050: 			}
0051: 			if (cardFactoryFile.Unlock.Cost < 0)
0052: 			{
~~~
- \`GameCardPK/CardFactoryUnlockData.cs:11\`
~~~csharp
0008: 
0009: 	public string Title { get; set; } = "卡牌厂";
0010: 
0011: 	public string Subtitle { get; set; } = "集换式卡牌";
0012: 
0013: 	public string Description { get; set; } = string.Empty;
0014: 
~~~

### 跟在队伍后面的黑鸟
- not found in decompiled C#

## 3831837821

### 触手本体
- \`MidnightPawn/ModEntry.cs:36\`
~~~csharp
0033: 		try
0034: 		{
0035: 			ModHelper.AddDebugFunction("lewd_meme", "Spawn", "生成少女", (Action)LootHook.SpawnMaiden, false);
0036: 			ModHelper.AddDebugFunction("lewd_meme", "Spawn", "生成触手本体", (Action)delegate
0037: 			{
0038: 				GameItem val = ModHelper.SpawnItem("tentacle_body");
0039: 				if (val != null)
~~~
- \`MidnightPawn/Game/LootHook.cs:29\`
~~~csharp
0026: 			if (Random.value < num)
0027: 			{
0028: 				ctx.itemId = "maiden_lower";
0029: 				ModCtx.Log("拾荒：携带触手本体，捕获到一个少女 (Lv." + bodyLevel + " 概率 " + num.ToString("P0") + ")");
0030: 			}
0031: 			else if (Random.value < 0.08f)
0032: 			{
~~~
- \`MidnightPawn/Game/Perks.cs:14\`
~~~csharp
0011: 	{
0012: 		try
0013: 		{
0014: 			ModHelper.AddStartingPerk("lewd_meme", "lewd_meme.tentacle_origin", 0, (StartingPerkType)0, (Func<string>)(() => "触手起源"), (Func<string>)(() => "你生来就不是人。开局获得【触手本体】，额外获得 3 个特性槽位，每日拾荒次数 +3，且每天会有一位女性顾客被你的气味吸引而来。"), "lewd_meme:tentacle_perk", (string[])null, 3, false);
0015: 			ModHook.OnNewGameStarted += OnNewGameStarted;
0016: 			ModHook.OnWakingUpLate += OnNewDay;
0017: 			ModCtx.Log("起始天赋已注册: 触手起源");
~~~
- \`MidnightPawn/Game/Perks.cs:36\`
~~~csharp
0033: 				{
0034: 					ModHelper.SetItemOwnerShip(val, true);
0035: 					ModHelper.MoveItemToInventory(val);
0036: 					ModCtx.Log("触手起源：开局获得触手本体");
0037: 				}
0038: 			}
0039: 		}
~~~
- \`MidnightPawn/Game/MaidenTooltip.cs:32\`
~~~csharp
0029: 				int bodyExp = TentacleBody.GetBodyExp();
0030: 				int expNeed = TentacleBody.GetExpNeed();
0031: 				builder.AddEmptyLine(2);
0032: 				builder.AddLine($"触手本体 Lv.{bodyLevel}   经验 {bodyExp} / {expNeed}", false, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0033: 				builder.AddLine("携带它去垃圾场拾荒，有概率捕获少女。", false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0034: 				string modData = ModHelper.GetModData("lewd_meme", "last_report", "");
0035: 				if (!string.IsNullOrEmpty(modData))
~~~
- \`MidnightPawn/Game/MaidenTooltip.cs:43\`
~~~csharp
0040: 			}
0041: 			catch (Exception ex)
0042: 			{
0043: 				ModCtx.LogWarning("触手本体 tooltip 失败: " + ex.Message);
0044: 				return;
0045: 			}
0046: 		}
~~~
- \`MidnightPawn/Game/TentacleBody.cs:59\`
~~~csharp
0056: 
0057: 	public static void SettleAll()
0058: 	{
0059: 		ModCtx.Log("【触手本体结算】第 " + GetToday() + " 天, 已登记本体=" + Instances.Count);
0060: 		if (Instances.Count == 0)
0061: 		{
0062: 			return;
~~~
- \`MidnightPawn/Game/TentacleBody.cs:169\`
~~~csharp
0166: 					.Append("】");
0167: 				num++;
0168: 			}
0169: 			stringBuilder.Append("在【触手本体】内被你的触手调教了一夜");
0170: 		}
0171: 		else
0172: 		{
~~~
- \`MidnightPawn/Game/TentacleBody.cs:173\`
~~~csharp
0170: 		}
0171: 		else
0172: 		{
0173: 			stringBuilder.Append("触手本体蠕动了整夜");
0174: 		}
0175: 		List<string> list = new List<string>();
0176: 		if (ReportMilk > 0)
~~~
- \`MidnightPawn/Game/TentacleBody.cs:254\`
~~~csharp
0251: 		PixelWindow contentWindow = BuildWindow(GetBodyLevel(), out grids);
0252: 		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_body");
0253: 		val.SetContentWindow(contentWindow);
0254: 		val.name = "触手本体";
0255: 		val.shortDescription = "这是什么？这是你的本体！可爱又能干的触手怪！让可爱的少女进入其中让她幸福。将这个太空站改造成为你的个人苗床巢吧。";
0256: 		val.flavorText = "";
0257: 		val.unitValue = 0L;
~~~
- \`MidnightPawn/Game/TentacleBody.cs:292\`
~~~csharp
0289: 		Slots[val] = grids;
0290: 		try
0291: 		{
0292: 			ModCtx.Log($"触手本体 shape = {((val.shape != null) ? val.shape.width : (-1))}x{((val.shape != null) ? val.shape.height : (-1))} (global {((val.shape != null) ? val.shape.globalWidth : (-1))}x{((val.shape != null) ? val.shape.globalHeight : (-1))})");
0293: 		}
0294: 		catch
0295: 		{
~~~
- \`MidnightPawn/Game/TentacleBody.cs:732\`
~~~csharp
0729: 		{
0730: 			num2 -= EXP_NEED[num - 1];
0731: 			num++;
0732: 			ModCtx.Log("触手本体升级到 Lv." + num + "（苗床容量与性爱强度提升）");
0733: 		}
0734: 		if (num > num3)
0735: 		{
~~~

### 调教值
- \`MidnightPawn/Game/ItemFactory.cs:237\`
~~~csharp
0234: 				}, false);
0235: 				SetInt(target, "MAIDEN_PREGNANT_DAYS", 0);
0236: 				Toast.Show(text2, "排卵诱发剂");
0237: 				ModCtx.Log("排卵诱发剂生效：" + text + " 进入排卵期 (调教值 " + num + ")");
0238: 				ModHelper.PlaySound("lewd_meme", "semen_apply");
0239: 				self.Destroy();
0240: 			}
~~~
- \`MidnightPawn/Game/MaidenTooltip.cs:103\`
~~~csharp
0100: 			int num3 = ItemFactory.GetInt(item, "ORAL_COUNT");
0101: 			int num4 = ItemFactory.GetInt(item, "SEX_COUNT");
0102: 			int num5 = ItemFactory.GetInt(item, "ANAL_COUNT");
0103: 			builder.AddLine($"调教值 {num}  泌乳等级 {num2}", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0104: 			builder.AddLine($"口交 {num3} 次 · 性交 {num4} 次 · 肛交 {num5} 次", false, (ColorPalette)12092762, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0105: 			if (item.IsTag("MAIDEN_OVULATING"))
0106: 			{
~~~

### 泌乳等级
- \`MidnightPawn/Game/MaidenTooltip.cs:103\`
~~~csharp
0100: 			int num3 = ItemFactory.GetInt(item, "ORAL_COUNT");
0101: 			int num4 = ItemFactory.GetInt(item, "SEX_COUNT");
0102: 			int num5 = ItemFactory.GetInt(item, "ANAL_COUNT");
0103: 			builder.AddLine($"调教值 {num}  泌乳等级 {num2}", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0104: 			builder.AddLine($"口交 {num3} 次 · 性交 {num4} 次 · 肛交 {num5} 次", false, (ColorPalette)12092762, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0105: 			if (item.IsTag("MAIDEN_OVULATING"))
0106: 			{
~~~
- \`MidnightPawn/Game/TentacleBody.cs:560\`
~~~csharp
0557: 						num3 = Mathf.Min(3, num3 + 1);
0558: 						ItemFactory.SetInt(val, "LACTATION_LV", num3);
0559: 						flag = true;
0560: 						ModCtx.Log(val.name + " 怀孕了（泌乳等级 +1）");
0561: 					}
0562: 					else
0563: 					{
~~~

### 口交
- \`MidnightPawn/Game/MaidenTooltip.cs:104\`
~~~csharp
0101: 			int num4 = ItemFactory.GetInt(item, "SEX_COUNT");
0102: 			int num5 = ItemFactory.GetInt(item, "ANAL_COUNT");
0103: 			builder.AddLine($"调教值 {num}  泌乳等级 {num2}", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0104: 			builder.AddLine($"口交 {num3} 次 · 性交 {num4} 次 · 肛交 {num5} 次", false, (ColorPalette)12092762, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0105: 			if (item.IsTag("MAIDEN_OVULATING"))
0106: 			{
0107: 				builder.AddLine("【排卵期】", false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
~~~

### 触手精液
- \`MidnightPawn/Game/ItemFactory.cs:150\`
~~~csharp
0147: 	private static GameItem CreateSemen()
0148: 	{
0149: 		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_semen");
0150: 		val.name = "触手精液";
0151: 		val.shortDescription = "黏糊糊的浊白精液混杂了些许的女性爱液，充满了触手的信息素，对女性充满了吸引力，但是男性闻起来却是作呕的腥臭味。";
0152: 		val.flavorText = "别问里面有什么，问就是模因。";
0153: 		val.unitValue = 90L;
~~~
- \`MidnightPawn/Game/TentacleEffects.cs:43\`
~~~csharp
0040: 			category = "tentacleSemenBuyBonus",
0041: 			valueStage = (ValueStage)1
0042: 		};
0043: 		val.SetPublicDisplay("触手精液的诱惑");
0044: 		val.SetActualDisplay("触手精液的诱惑");
0045: 		val.isFeatureMatch = true;
0046: 		val.SetPreExposedValueMod(15, true);
~~~
- \`MidnightPawn/Game/TentacleEffects.cs:44\`
~~~csharp
0041: 			valueStage = (ValueStage)1
0042: 		};
0043: 		val.SetPublicDisplay("触手精液的诱惑");
0044: 		val.SetActualDisplay("触手精液的诱惑");
0045: 		val.isFeatureMatch = true;
0046: 		val.SetPreExposedValueMod(15, true);
0047: 		return val;
~~~
- \`MidnightPawn/Game/TentacleEffects.cs:68\`
~~~csharp
0065: 			category = "tentacleSemenInfluence",
0066: 			valueStage = (ValueStage)1
0067: 		};
0068: 		val.SetPublicDisplay("触手精液的影响");
0069: 		val.SetActualDisplay("触手精液的影响");
0070: 		val.isFeatureMatch = true;
0071: 		val.SetPreExposedValueMod(-15, true);
~~~
- \`MidnightPawn/Game/TentacleEffects.cs:69\`
~~~csharp
0066: 			valueStage = (ValueStage)1
0067: 		};
0068: 		val.SetPublicDisplay("触手精液的影响");
0069: 		val.SetActualDisplay("触手精液的影响");
0070: 		val.isFeatureMatch = true;
0071: 		val.SetPreExposedValueMod(-15, true);
0072: 		return val;
~~~
- \`MidnightPawn/Game/TentacleEffects.cs:103\`
~~~csharp
0100: 			{
0101: 				instance.Play2("perfume_spray");
0102: 			}
0103: 			ModCtx.Log("触手精液生效：买入价 -15%，声望 +15%（已登记顾客）");
0104: 			string text = (string.IsNullOrEmpty(val.displayName) ? "她" : val.displayName);
0105: 			string[] array = new string[3]
0106: 			{
~~~
- \`MidnightPawn/Game/TentacleEffects.cs:116\`
~~~csharp
0113: 		}
0114: 		catch (Exception ex)
0115: 		{
0116: 			ModCtx.LogWarning("触手精液生效失败: " + ex.Message);
0117: 			return false;
0118: 		}
0119: 	}
~~~
- \`MidnightPawn/Game/PatchClientArea.cs:49\`
~~~csharp
0046: 		}
0047: 		catch (Exception ex)
0048: 		{
0049: 			ModCtx.LogWarning("触手精液特性挂载失败: " + ex.Message);
0050: 		}
0051: 	}
0052: }
~~~
- \`MidnightPawn/Game/TentacleBody.cs:182\`
~~~csharp
0179: 		}
0180: 		if (ReportSemen > 0)
0181: 		{
0182: 			list.Add("【触手精液】*" + ReportSemen);
0183: 		}
0184: 		if (list.Count > 0)
0185: 		{
~~~

### 淫色深空
- \`MidnightPawn/ModCtx.cs:22\`
~~~csharp
0019: 		}
0020: 		else
0021: 		{
0022: 			Debug.Log((object)("[淫色深空：可能是触手] " + msg));
0023: 		}
0024: 	}
0025: 
~~~
- \`MidnightPawn/ModCtx.cs:34\`
~~~csharp
0031: 		}
0032: 		else
0033: 		{
0034: 			Debug.LogWarning((object)("[淫色深空：可能是触手] " + msg));
0035: 		}
0036: 	}
0037: 
~~~
- \`MidnightPawn/ModCtx.cs:46\`
~~~csharp
0043: 		}
0044: 		else
0045: 		{
0046: 			Debug.LogError((object)("[淫色深空：可能是触手] " + msg));
0047: 		}
0048: 	}
0049: }
~~~
- \`MidnightPawn/ModConst.cs:7\`
~~~csharp
0004: {
0005: 	public const string MOD_ID = "lewd_meme";
0006: 
0007: 	public const string MOD_NAME = "淫色深空：可能是触手";
0008: 
0009: 	public const string ITEM_TENTACLE_BODY = "tentacle_body";
0010: 
~~~

### 可能是触手
- \`MidnightPawn/ModCtx.cs:22\`
~~~csharp
0019: 		}
0020: 		else
0021: 		{
0022: 			Debug.Log((object)("[淫色深空：可能是触手] " + msg));
0023: 		}
0024: 	}
0025: 
~~~
- \`MidnightPawn/ModCtx.cs:34\`
~~~csharp
0031: 		}
0032: 		else
0033: 		{
0034: 			Debug.LogWarning((object)("[淫色深空：可能是触手] " + msg));
0035: 		}
0036: 	}
0037: 
~~~
- \`MidnightPawn/ModCtx.cs:46\`
~~~csharp
0043: 		}
0044: 		else
0045: 		{
0046: 			Debug.LogError((object)("[淫色深空：可能是触手] " + msg));
0047: 		}
0048: 	}
0049: }
~~~
- \`MidnightPawn/ModConst.cs:7\`
~~~csharp
0004: {
0005: 	public const string MOD_ID = "lewd_meme";
0006: 
0007: 	public const string MOD_NAME = "淫色深空：可能是触手";
0008: 
0009: 	public const string ITEM_TENTACLE_BODY = "tentacle_body";
0010: 
~~~

## 3829429358

### Sterile Gauze Pack
- \`XIAOWONightAuction/AuctionNewItems.cs:9\`
~~~csharp
0006: {
0007: 	internal static void Add(Dictionary<string, AuctionExclusiveSpec> items, Dictionary<string, (string Name, string Description)> english)
0008: 	{
0009: 		Add(items, english, ContainerCategory.Medical, "sterile_gauze", 120, 1, 1, 1, "hemostatic_foam", "无菌纱布包", "独立封装的医疗纱布，适合出售给医疗物资收购商。", "Sterile Gauze Pack", "Individually sealed medical gauze for medical supply buyers.", "junk");
0010: 		Add(items, english, ContainerCategory.Medical, "burn_gel", 260, 1, 1, 2, "neural_repair", "冷敷凝胶管", "用于临床冷敷的密封凝胶管。", "Cooling Gel Tube", "A sealed tube of clinical cooling gel.", "junk");
0011: 		Add(items, english, ContainerCategory.Medical, "field_dressing", 480, 1, 2, 1, "nano_suture", "战地止血敷料", "双击使用，按原版止血绷带的条件和效果处理伤势，使用后消耗。", "Field Hemostatic Dressing", "Double-click to use. Uses the native hemostatic bandage conditions and wound treatment; consumed on use.", "hemostatic_bandage_item");
0012: 		Add(items, english, ContainerCategory.Medical, "rescue_salts", 750, 1, 1, 1, "hemostatic_foam", "急救醒神盐", "沿用原版嗅盐的恢复意识效果与使用条件；仅在原版允许的场合使用，使用后消耗。", "Rescue Smelling Salts", "Uses native smelling salt recovery effects and restrictions; usable only where the native item permits it, and consumed on use.", "smelling_salt");
~~~

### Double-click to treat wounds
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:109\`
~~~csharp
0106: 				value.NativeTemplate = text;
0107: 			}
0108: 		}
0109: 		Describe("sterile_gauze", "双击使用，按原版普通绷带的条件处理伤势，使用后消耗。", "Double-click to treat wounds with the native ordinary bandage rules. Consumed on use.");
0110: 		Describe("hemostatic_foam", "双击使用，按原版止血绷带的条件稳定伤势，使用后消耗。", "Double-click to stabilize wounds with native hemostatic bandage rules. Consumed on use.");
0111: 		Describe("neural_repair", "双击使用，按原版Synaprest药剂随机解除一种成瘾；没有成瘾时不可使用，成功使用后消耗。", "Double-click to remove one random addiction using native Synaprest rules. Unusable without an addiction; consumed on use.");
0112: 		string[] array = new string[3] { "ceramic_fuse", "braided_cable", "bearing_set" };
~~~

### Rare goods traded only at the night container auction
- \`XIAOWONightAuction/AuctionExclusiveFactory.cs:289\`
~~~csharp
0286: 		item.name = text2;
0287: 		item.shortDescription = text3;
0288: 		item.longDescription = text3;
0289: 		item.flavorText = AuctionText.Select("只在夜间集装箱拍卖中流通的稀有货物。", "Rare goods traded only at the night container auction.");
0290: 	}
0291: }
~~~

### Night Auction
- \`XIAOWONightAuction/AuctionPhone.cs:30\`
~~~csharp
0027: 
0028: 	private static PaperUI _paper;
0029: 
0030: 	internal static string ContactName => AuctionText.Select("夜间拍卖行", "Night Auction House");
0031: 
0032: 	internal static bool CardGiven => AuctionState.Read("phone_card_given") == "1";
0033: 
~~~
- \`XIAOWONightAuction/AuctionPhone.cs:143\`
~~~csharp
0140: 
0141: 	private static void LocalizeCard(GameItem item)
0142: 	{
0143: 		string identifierName = (item.name = AuctionText.Select("夜间拍卖行名片", "Night Auction Business Card"));
0144: 		item.identifierName = identifierName;
0145: 		identifierName = (item.longDescription = AuctionText.Select("电话：4367。四类收货商仅接受电话预约，次日到店。通常第3天可再预约；预约核心伙伴时缩短为第2天。赠送对应类别的礼物可提升合作关系。", "Phone: 4367. Four specialist buyers visit only by appointment, arriving tomorrow. Book again after 3 days, or 2 days when booking a Trusted Partner. Give matching gifts to improve relations."));
0146: 		item.shortDescription = identifierName;
~~~
- \`XIAOWONightAuction/AuctionPhone.cs:305\`
~~~csharp
0302: 		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
0303: 		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
0304: 		EnsureContact();
0305: 		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行服务。请选择：", "Night Auction House services. Please choose:"));
0306: 		AddServiceChoice(val, AuctionText.Select("预约收货商人", "Book a Buyer"), BuildBookingDialogue(), null);
0307: 		AddServiceChoice(val, AuctionText.Select("采购委托", "Procurement Orders"), AuctionCommissions.DialoguePage(), null);
0308: 		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("顾问与档案服务。", "Advisors and records."));
~~~
- \`XIAOWONightAuction/AuctionPhone.cs:329\`
~~~csharp
0326: 		{
0327: 			return Ending(UnavailableText());
0328: 		}
0329: 		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行收货专线。需要哪一类采购员？确认后明天到店。", "Night Auction buying desk. Which specialist do you need? Confirm a booking for tomorrow."));
0330: 		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("其他采购类别。", "Other buying categories."));
0331: 		AddBooking(val, ContainerCategory.Medical);
0332: 		AddBooking(val, ContainerCategory.Luxury);
~~~
- \`XIAOWONightAuction/AuctionPhone.cs:534\`
~~~csharp
0531: 		{
0532: 			return regular;
0533: 		}
0534: 		return new Dialogue().SetText(client.displayName, AuctionText.Select("这是夜间拍卖行的名片，号码4367。打电话告诉我们你要出哪一类货，采购员第二天来；每次预约后要间隔两天。", "Here's our Night Auction card: 4367. Call to choose a buying specialist for tomorrow. Leave two days between bookings.")).SetStartAction(MonoCallbacks.Bind<Action>((Action)delegate
0535: 		{
0536: 			DeliverCard();
0537: 		})).SetNextDialogue(regular);
~~~
- \`XIAOWONightAuction/AuctionMerchants.cs:189\`
~~~csharp
0186: 	{
0187: 		return category switch
0188: 		{
0189: 			ContainerCategory.Medical => AuctionText.Select("夜拍医疗采购员", "Night Auction Medic"), 
0190: 			ContainerCategory.Luxury => AuctionText.Select("夜拍藏品经纪", "Night Auction Curator"), 
0191: 			ContainerCategory.Contraband => AuctionText.Select("夜拍黑市掮客", "Night Auction Broker"), 
0192: 			_ => AuctionText.Select("夜拍工程承包商", "Night Auction Engineer"), 
~~~
- \`XIAOWONightAuction/AuctionMerchants.cs:190\`
~~~csharp
0187: 		return category switch
0188: 		{
0189: 			ContainerCategory.Medical => AuctionText.Select("夜拍医疗采购员", "Night Auction Medic"), 
0190: 			ContainerCategory.Luxury => AuctionText.Select("夜拍藏品经纪", "Night Auction Curator"), 
0191: 			ContainerCategory.Contraband => AuctionText.Select("夜拍黑市掮客", "Night Auction Broker"), 
0192: 			_ => AuctionText.Select("夜拍工程承包商", "Night Auction Engineer"), 
0193: 		};
~~~
- \`XIAOWONightAuction/AuctionMerchants.cs:191\`
~~~csharp
0188: 		{
0189: 			ContainerCategory.Medical => AuctionText.Select("夜拍医疗采购员", "Night Auction Medic"), 
0190: 			ContainerCategory.Luxury => AuctionText.Select("夜拍藏品经纪", "Night Auction Curator"), 
0191: 			ContainerCategory.Contraband => AuctionText.Select("夜拍黑市掮客", "Night Auction Broker"), 
0192: 			_ => AuctionText.Select("夜拍工程承包商", "Night Auction Engineer"), 
0193: 		};
0194: 	}
~~~
- \`XIAOWONightAuction/AuctionMerchants.cs:192\`
~~~csharp
0189: 			ContainerCategory.Medical => AuctionText.Select("夜拍医疗采购员", "Night Auction Medic"), 
0190: 			ContainerCategory.Luxury => AuctionText.Select("夜拍藏品经纪", "Night Auction Curator"), 
0191: 			ContainerCategory.Contraband => AuctionText.Select("夜拍黑市掮客", "Night Auction Broker"), 
0192: 			_ => AuctionText.Select("夜拍工程承包商", "Night Auction Engineer"), 
0193: 		};
0194: 	}
0195: 
~~~

### Industrial Calibrator
- \`XIAOWONightAuction/AuctionExclusiveCatalog.cs:43\`
~~~csharp
0040: 			["xiaowo_auction_coilgun"] = ("Silent Coilgun Prototype", "An unproduced prototype of a low-noise coil weapon."),
0041: 			["xiaowo_auction_master_key"] = ("Military Master Key", "A core key capable of forging military access credentials."),
0042: 			["xiaowo_auction_bearing_set"] = ("High-Density Bearings", "Precision bearings for heavy industrial equipment."),
0043: 			["xiaowo_auction_calibrator"] = ("Industrial Calibrator", "A portable instrument for precision machine calibration."),
0044: 			["xiaowo_auction_supercoil"] = ("Superconducting Coil", "A fully sealed industrial superconducting coil."),
0045: 			["xiaowo_auction_control_core"] = ("Prototype Control Core", "An unreleased automation control core."),
0046: 			["xiaowo_auction_fabricator_core"] = ("Self-Calibrating Fabricator", "A fabrication core that corrects its own machining errors."),
~~~

### industrial_calibrator
- not found in decompiled C#

### A portable instrument for precision machine calibration.
- \`XIAOWONightAuction/AuctionExclusiveCatalog.cs:43\`
~~~csharp
0040: 			["xiaowo_auction_coilgun"] = ("Silent Coilgun Prototype", "An unproduced prototype of a low-noise coil weapon."),
0041: 			["xiaowo_auction_master_key"] = ("Military Master Key", "A core key capable of forging military access credentials."),
0042: 			["xiaowo_auction_bearing_set"] = ("High-Density Bearings", "Precision bearings for heavy industrial equipment."),
0043: 			["xiaowo_auction_calibrator"] = ("Industrial Calibrator", "A portable instrument for precision machine calibration."),
0044: 			["xiaowo_auction_supercoil"] = ("Superconducting Coil", "A fully sealed industrial superconducting coil."),
0045: 			["xiaowo_auction_control_core"] = ("Prototype Control Core", "An unreleased automation control core."),
0046: 			["xiaowo_auction_fabricator_core"] = ("Self-Calibrating Fabricator", "A fabrication core that corrects its own machining errors."),
~~~

## 3829085054

### 收纳箱
- \`ItemStorage/GearPanel.cs:133\`
~~~csharp
0130: 		}
0131: 		catch (Exception ex)
0132: 		{
0133: 			Core.Log?.Warning("收纳箱齿轮面板开关异常：" + ex.Message);
0134: 		}
0135: 	}
0136: 
~~~
- \`ItemStorage/GearPanel.cs:349\`
~~~csharp
0346: 			}
0347: 			catch (Exception ex2)
0348: 			{
0349: 				Core.Log?.Warning("收纳箱齿轮面板名字开关异常：" + ex2.Message);
0350: 			}
0351: 		})));
0352: 		Attach(val2, val10, 1, 4);
~~~
- \`ItemStorage/GearPanel.cs:374\`
~~~csharp
0371: 		}
0372: 		catch (Exception ex)
0373: 		{
0374: 			Core.Log?.Warning("收纳箱齿轮面板 Attach 失败：" + ex.Message);
0375: 		}
0376: 		if (num != 0L)
0377: 		{
~~~
- \`ItemStorage/GearPanel.cs:406\`
~~~csharp
0403: 			}
0404: 			catch (Exception ex)
0405: 			{
0406: 				Core.Log?.Warning("收纳箱齿轮面板开关异常（" + key + "）：" + ex.Message);
0407: 			}
0408: 		}));
0409: 	}
~~~
- \`ItemStorage/GearPanel.cs:439\`
~~~csharp
0436: 	{
0437: 		if (button == null || !grid.Attach((PixelElement)(object)button, col, row, 116, 24))
0438: 		{
0439: 			Core.Log?.Warning("收纳箱齿轮面板按钮行 (" + col + "," + row + ") attach 失败");
0440: 		}
0441: 	}
0442: 
~~~
- \`ItemStorage/GearPanel.cs:447\`
~~~csharp
0444: 	{
0445: 		if (radio == null || !grid.Attach((PixelElement)(object)radio, col, row, 116, 24))
0446: 		{
0447: 			Core.Log?.Warning("收纳箱齿轮面板开关行 (" + col + "," + row + ") attach 失败");
0448: 		}
0449: 	}
0450: 
~~~
- \`ItemStorage/GearPanel.cs:455\`
~~~csharp
0452: 	{
0453: 		if (text == null || !grid.Attach((PixelElement)(object)text, col, row, 116, 14))
0454: 		{
0455: 			Core.Log?.Warning("收纳箱齿轮面板标题行 (" + col + "," + row + ") attach 失败");
0456: 		}
0457: 	}
0458: 
~~~
- \`ItemStorage/GearPanel.cs:548\`
~~~csharp
0545: 			}
0546: 			catch (Exception ex)
0547: 			{
0548: 				Core.Log?.Warning("收纳箱齿轮面板按钮异常：" + ex.Message);
0549: 			}
0550: 		};
0551: 	}
~~~
- \`ItemStorage/GearPanel.cs:563\`
~~~csharp
0560: 			}
0561: 			catch (Exception ex)
0562: 			{
0563: 				Core.Log?.Warning("收纳箱齿轮面板开关异常：" + ex.Message);
0564: 			}
0565: 		};
0566: 	}
~~~
- \`ItemStorage/GearPanel.cs:645\`
~~~csharp
0642: 			_captureUid = 0L;
0643: 			EndCaptureFrame();
0644: 			_captureButton = null;
0645: 			Core.Debug("收纳箱快捷键：捕获面板已随场景销毁，捕获态自动复位。");
0646: 			return;
0647: 		}
0648: 		if (Time.realtimeSinceStartup > _captureUntil)
~~~
- \`ItemStorage/GearPanel.cs:733\`
~~~csharp
0730: 					{
0731: 						text2 = "与官方按键冲突";
0732: 					}
0733: 					Core.Debug("收纳箱快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 与官方键位 " + binding.id + " 冲突：" + text2);
0734: 					return text2;
0735: 				}
0736: 			}
~~~
- \`ItemStorage/GearPanel.cs:751\`
~~~csharp
0748: 		_captureButton = null;
0749: 		ShowBanner(entry, msg, seconds);
0750: 		BoxItem.NotifyUser("快捷键绑定失败：" + msg);
0751: 		Core.Log?.Msg("收纳箱：快捷键绑定被拒——" + msg + "。");
0752: 	}
0753: 
0754: 	private static string RestoreTextOf(CaptureMode mode, GameItem? pack)
~~~

### 一键收纳
- \`ItemStorage/GearPanel.cs:466\`
~~~csharp
0463: 		KeyCode val = (KeyCode)StorageGrid.GetBoxOption(pack, "HOTKEY");
0464: 		if ((int)val != 0)
0465: 		{
0466: 			return "一键收纳:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
0467: 		}
0468: 		return "一键收纳:点击绑定";
0469: 	}
~~~
- \`ItemStorage/GearPanel.cs:468\`
~~~csharp
0465: 		{
0466: 			return "一键收纳:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
0467: 		}
0468: 		return "一键收纳:点击绑定";
0469: 	}
0470: 
0471: 	private unsafe static string RestockKeyText(GameItem pack)
~~~
- \`ItemStorage/GearPanel.cs:758\`
~~~csharp
0755: 	{
0756: 		return mode switch
0757: 		{
0758: 			CaptureMode.Single => (pack != null) ? HotkeyText(pack) : "一键收纳:点击绑定", 
0759: 			CaptureMode.All => AllKeyText(), 
0760: 			CaptureMode.RestockSingle => (pack != null) ? RestockKeyText(pack) : "一键上货:点击绑定", 
0761: 			CaptureMode.RestockAll => AllRestockKeyText(), 
~~~
- \`ItemStorage/GearPanel.cs:803\`
~~~csharp
0800: 					{
0801: 						if (val == key)
0802: 						{
0803: 							return "该键已被本箱一键收纳占用";
0804: 						}
0805: 						if (val2 == key)
0806: 						{
~~~
- \`ItemStorage/GearPanel.cs:815\`
~~~csharp
0812: 				{
0813: 					if (val == key)
0814: 					{
0815: 						return "该键已被一键收纳占用";
0816: 					}
0817: 					if (val2 == key)
0818: 					{
~~~
- \`ItemStorage/GearPanel.cs:851\`
~~~csharp
0848: 		EndCaptureFrame();
0849: 		if (val == null)
0850: 		{
0851: 			SafeSetText(_captureButton, "一键收纳:点击绑定");
0852: 			_captureButton = null;
0853: 			return;
0854: 		}
~~~
- \`ItemStorage/GearPanel.cs:1072\`
~~~csharp
1069: 			_captureUid = 0L;
1070: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, val));
1071: 			_captureButton = null;
1072: 			ShowBanner(entry, (captureMode == CaptureMode.Single) ? "一键收纳快捷键已解除绑定" : "一键上货快捷键已解除绑定", 2.5f);
1073: 			return true;
1074: 		}
1075: 		default:
~~~
- \`ItemStorage/AutoStow.cs:26\`
~~~csharp
0023: 		}
0024: 		if (!BoxTransfer.IsInStore())
0025: 		{
0026: 			Core.Debug("收纳箱[" + context + "] 一键收纳：玩家不在店内，忽略。");
0027: 			BoxItem.NotifyUser("请回到店内再收纳");
0028: 			return;
0029: 		}
~~~
- \`ItemStorage/AutoStow.cs:38\`
~~~csharp
0035: 		}
0036: 		if (dictionary.Count == 0)
0037: 		{
0038: 			Core.Debug("收纳箱[" + context + "] 一键收纳：盒子里没有物品，不工作。");
0039: 			BoxItem.NotifyUser("没有可收纳的物品");
0040: 			return;
0041: 		}
~~~
- \`ItemStorage/AutoStow.cs:75\`
~~~csharp
0072: 		{
0073: 			Core.Log?.Warning("收纳箱[" + context + "] 箱间联动异常：" + ex.Message);
0074: 		}
0075: 		Core.Debug("收纳箱[" + context + "] 一键收纳耗时 " + stopwatch.ElapsedMilliseconds + " ms（收 " + num + " 叠 / 候选 " + list.Count + " 件）。");
0076: 		if (num > 0 || num5 > 0)
0077: 		{
0078: 			try
~~~
- \`ItemStorage/AutoStow.cs:96\`
~~~csharp
0093: 			catch
0094: 			{
0095: 			}
0096: 			Core.Log?.Msg("收纳箱[" + context + "] 一键收纳完成：收进 " + num + " 叠共 " + num2 + " 件" + ((num5 > 0) ? ("，箱间归拢 " + num5 + " 格") : "") + ((num3 > 0) ? ("，" + num3 + " 叠放不下或不可收，跳过") : "") + ((num4 > 0) ? ("，" + num4 + " 件销毁走裸销毁兜底") : "") + "。");
0097: 		}
0098: 		else
0099: 		{
~~~
- \`ItemStorage/AutoStow.cs:100\`
~~~csharp
0097: 		}
0098: 		else
0099: 		{
0100: 			Core.Log?.Msg("收纳箱[" + context + "] 一键收纳：店里没有能收进来的物品" + ((num3 > 0) ? ("（" + num3 + " 叠空间不足或不可收，跳过）") : "") + ((num5 > 0) ? ("；从其他收纳箱归拢 " + num5 + " 格") : "") + "。");
0101: 		}
0102: 		if (num2 > 0)
0103: 		{
~~~

### 全箱收纳
- \`ItemStorage/GearPanel.cs:491\`
~~~csharp
0488: 		KeyCode key = BoxStowAll.Key;
0489: 		if ((int)key != 0)
0490: 		{
0491: 			return "全箱收纳:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
0492: 		}
0493: 		return "全箱收纳:点击绑定";
0494: 	}
~~~
- \`ItemStorage/GearPanel.cs:493\`
~~~csharp
0490: 		{
0491: 			return "全箱收纳:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
0492: 		}
0493: 		return "全箱收纳:点击绑定";
0494: 	}
0495: 
0496: 	private unsafe static string AllRestockKeyText()
~~~
- \`ItemStorage/GearPanel.cs:829\`
~~~csharp
0826: 		}
0827: 		if (BoxStowAll.Key == key)
0828: 		{
0829: 			return "该键已被全箱收纳占用";
0830: 		}
0831: 		if (BoxRestock.Key == key)
0832: 		{
~~~
- \`ItemStorage/GearPanel.cs:901\`
~~~csharp
0898: 		_captureButton = null;
0899: 		BoxHotkey.SuppressUntilRelease(key);
0900: 		SyncGlobalKeyButtons();
0901: 		Core.Log?.Msg("收纳箱：全箱收纳键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
0902: 	}
0903: 
0904: 	private unsafe static void TryBindRestockSingle(long uid, KeyCode key)
~~~
- \`ItemStorage/GearPanel.cs:1053\`
~~~csharp
1050: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, null));
1051: 			_captureButton = null;
1052: 			SyncGlobalKeyButtons();
1053: 			ShowBanner(entry, (captureMode == CaptureMode.All) ? "全箱收纳快捷键已解除绑定" : "全箱上货快捷键已解除绑定", 2.5f);
1054: 			Core.Log?.Msg("收纳箱：" + ((captureMode == CaptureMode.All) ? "全箱收纳键" : "全箱上货键") + "已由玩家在捕获态按 Esc 主动解绑。");
1055: 			return true;
1056: 		case CaptureMode.Single:
~~~
- \`ItemStorage/GearPanel.cs:1054\`
~~~csharp
1051: 			_captureButton = null;
1052: 			SyncGlobalKeyButtons();
1053: 			ShowBanner(entry, (captureMode == CaptureMode.All) ? "全箱收纳快捷键已解除绑定" : "全箱上货快捷键已解除绑定", 2.5f);
1054: 			Core.Log?.Msg("收纳箱：" + ((captureMode == CaptureMode.All) ? "全箱收纳键" : "全箱上货键") + "已由玩家在捕获态按 Esc 主动解绑。");
1055: 			return true;
1056: 		case CaptureMode.Single:
1057: 		case CaptureMode.RestockSingle:
~~~
- \`ItemStorage/BoxWindow.cs:374\`
~~~csharp
0371: 					{
0372: 					}
0373: 				});
0374: 				PixelElement val15 = (PixelElement)new MachineryButton("全箱收纳", action2, 48, 24);
0375: 				if (val15 == null)
0376: 				{
0377: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮非 PixelElement");
~~~
- \`ItemStorage/BoxWindow.cs:377\`
~~~csharp
0374: 				PixelElement val15 = (PixelElement)new MachineryButton("全箱收纳", action2, 48, 24);
0375: 				if (val15 == null)
0376: 				{
0377: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮非 PixelElement");
0378: 				}
0379: 				else if (!val6.Attach(val15, 0, 4, 48, 24))
0380: 				{
~~~
- \`ItemStorage/BoxWindow.cs:381\`
~~~csharp
0378: 				}
0379: 				else if (!val6.Attach(val15, 0, 4, 48, 24))
0380: 				{
0381: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱收纳按钮失败");
0382: 				}
0383: 			}
0384: 			catch (Exception ex5)
~~~
- \`ItemStorage/BoxWindow.cs:386\`
~~~csharp
0383: 			}
0384: 			catch (Exception ex5)
0385: 			{
0386: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮构建失败：" + ex5.Message);
0387: 			}
0388: 			try
0389: 			{
~~~
- \`ItemStorage/BoxStowAll.cs:87\`
~~~csharp
0084: 		}
0085: 		catch (Exception ex)
0086: 		{
0087: 			Core.Log?.Warning("全箱收纳键位写入存档失败：" + ex.Message);
0088: 		}
0089: 	}
0090: 
~~~
- \`ItemStorage/BoxStowAll.cs:112\`
~~~csharp
0109: 		catch
0110: 		{
0111: 		}
0112: 		Core.Log?.Msg("全箱收纳键位已自动解绑：" + reason);
0113: 	}
0114: 
0115: 	internal static void UnbindManually()
~~~

### 全箱上货
- \`ItemStorage/GearPanel.cs:504\`
~~~csharp
0501: 		KeyCode key = BoxRestock.Key;
0502: 		if ((int)key != 0)
0503: 		{
0504: 			return "全箱上货:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
0505: 		}
0506: 		return "全箱上货:点击绑定";
0507: 	}
~~~
- \`ItemStorage/GearPanel.cs:506\`
~~~csharp
0503: 		{
0504: 			return "全箱上货:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
0505: 		}
0506: 		return "全箱上货:点击绑定";
0507: 	}
0508: 
0509: 	private static string TitleOf(GameItem pack)
~~~
- \`ItemStorage/GearPanel.cs:833\`
~~~csharp
0830: 		}
0831: 		if (BoxRestock.Key == key)
0832: 		{
0833: 			return "该键已被全箱上货占用";
0834: 		}
0835: 		return null;
0836: 	}
~~~
- \`ItemStorage/GearPanel.cs:967\`
~~~csharp
0964: 		_captureButton = null;
0965: 		BoxHotkey.SuppressUntilRelease(key);
0966: 		SyncGlobalKeyButtons();
0967: 		Core.Log?.Msg("收纳箱：全箱上货键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
0968: 	}
0969: 
0970: 	private static void SyncGlobalKeyButtons()
~~~
- \`ItemStorage/GearPanel.cs:1053\`
~~~csharp
1050: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, null));
1051: 			_captureButton = null;
1052: 			SyncGlobalKeyButtons();
1053: 			ShowBanner(entry, (captureMode == CaptureMode.All) ? "全箱收纳快捷键已解除绑定" : "全箱上货快捷键已解除绑定", 2.5f);
1054: 			Core.Log?.Msg("收纳箱：" + ((captureMode == CaptureMode.All) ? "全箱收纳键" : "全箱上货键") + "已由玩家在捕获态按 Esc 主动解绑。");
1055: 			return true;
1056: 		case CaptureMode.Single:
~~~
- \`ItemStorage/GearPanel.cs:1054\`
~~~csharp
1051: 			_captureButton = null;
1052: 			SyncGlobalKeyButtons();
1053: 			ShowBanner(entry, (captureMode == CaptureMode.All) ? "全箱收纳快捷键已解除绑定" : "全箱上货快捷键已解除绑定", 2.5f);
1054: 			Core.Log?.Msg("收纳箱：" + ((captureMode == CaptureMode.All) ? "全箱收纳键" : "全箱上货键") + "已由玩家在捕获态按 Esc 主动解绑。");
1055: 			return true;
1056: 		case CaptureMode.Single:
1057: 		case CaptureMode.RestockSingle:
~~~
- \`ItemStorage/BoxWindow.cs:400\`
~~~csharp
0397: 					{
0398: 					}
0399: 				});
0400: 				PixelElement val16 = (PixelElement)new MachineryButton("全箱上货", action3, 48, 24);
0401: 				if (val16 == null)
0402: 				{
0403: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮非 PixelElement");
~~~
- \`ItemStorage/BoxWindow.cs:403\`
~~~csharp
0400: 				PixelElement val16 = (PixelElement)new MachineryButton("全箱上货", action3, 48, 24);
0401: 				if (val16 == null)
0402: 				{
0403: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮非 PixelElement");
0404: 				}
0405: 				else if (!val6.Attach(val16, 1, 4, 48, 24))
0406: 				{
~~~
- \`ItemStorage/BoxWindow.cs:407\`
~~~csharp
0404: 				}
0405: 				else if (!val6.Attach(val16, 1, 4, 48, 24))
0406: 				{
0407: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱上货按钮失败");
0408: 				}
0409: 			}
0410: 			catch (Exception ex6)
~~~
- \`ItemStorage/BoxWindow.cs:412\`
~~~csharp
0409: 			}
0410: 			catch (Exception ex6)
0411: 			{
0412: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮构建失败：" + ex6.Message);
0413: 			}
0414: 			try
0415: 			{
~~~
- \`ItemStorage/BoxRestock.cs:119\`
~~~csharp
0116: 		}
0117: 		catch (Exception ex)
0118: 		{
0119: 			Core.Log?.Warning("全箱上货键位写入存档失败：" + ex.Message);
0120: 		}
0121: 	}
0122: 
~~~
- \`ItemStorage/BoxRestock.cs:144\`
~~~csharp
0141: 		catch
0142: 		{
0143: 		}
0144: 		Core.Log?.Msg("全箱上货键位已解绑：" + reason);
0145: 	}
0146: 
0147: 	internal static void UnbindManually()
~~~

### 一键上货
- \`ItemStorage/GearPanel.cs:478\`
~~~csharp
0475: 		KeyCode val = (KeyCode)StorageGrid.GetBoxOption(pack, "HOTRESTOCK");
0476: 		if ((int)val != 0)
0477: 		{
0478: 			return "一键上货:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
0479: 		}
0480: 		return "一键上货:点击绑定";
0481: 	}
~~~
- \`ItemStorage/GearPanel.cs:480\`
~~~csharp
0477: 		{
0478: 			return "一键上货:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
0479: 		}
0480: 		return "一键上货:点击绑定";
0481: 	}
0482: 
0483: 	private unsafe static string AllKeyText()
~~~
- \`ItemStorage/GearPanel.cs:760\`
~~~csharp
0757: 		{
0758: 			CaptureMode.Single => (pack != null) ? HotkeyText(pack) : "一键收纳:点击绑定", 
0759: 			CaptureMode.All => AllKeyText(), 
0760: 			CaptureMode.RestockSingle => (pack != null) ? RestockKeyText(pack) : "一键上货:点击绑定", 
0761: 			CaptureMode.RestockAll => AllRestockKeyText(), 
0762: 			_ => "点击绑定", 
0763: 		};
~~~
- \`ItemStorage/GearPanel.cs:807\`
~~~csharp
0804: 						}
0805: 						if (val2 == key)
0806: 						{
0807: 							return "该键已被本箱一键上货占用";
0808: 						}
0809: 					}
0810: 				}
~~~
- \`ItemStorage/GearPanel.cs:819\`
~~~csharp
0816: 					}
0817: 					if (val2 == key)
0818: 					{
0819: 						return "该键已被一键上货占用";
0820: 					}
0821: 				}
0822: 			}
~~~
- \`ItemStorage/GearPanel.cs:917\`
~~~csharp
0914: 		EndCaptureFrame();
0915: 		if (val == null)
0916: 		{
0917: 			SafeSetText(_captureButton, "一键上货:点击绑定");
0918: 			_captureButton = null;
0919: 			return;
0920: 		}
~~~
- \`ItemStorage/GearPanel.cs:1072\`
~~~csharp
1069: 			_captureUid = 0L;
1070: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, val));
1071: 			_captureButton = null;
1072: 			ShowBanner(entry, (captureMode == CaptureMode.Single) ? "一键收纳快捷键已解除绑定" : "一键上货快捷键已解除绑定", 2.5f);
1073: 			return true;
1074: 		}
1075: 		default:
~~~
- \`ItemStorage/BoxHotkey.cs:72\`
~~~csharp
0069: 					if (boxOption2 != 0 && (!BoundKeys.TryGetValue(boxOption2, out var value2) || !value2.Restock || ResolveBox(value2.Uid) == null))
0070: 					{
0071: 						BoundKeys[boxOption2] = new HotkeyEntry(num2, restock: true);
0072: 						Core.Debug("快捷键自愈：恢复 " + ((object)(KeyCode)boxOption2/*cast due to .constrained prefix*/).ToString() + " → 箱 uid " + num2 + "（一键上货）" + ((value2.Uid != 0L && value2.Uid != num2) ? ("（旧 uid " + value2.Uid + " 已失效，覆盖）") : "") + "。");
0073: 					}
0074: 				}
0075: 				catch
~~~
- \`ItemStorage/BoxHotkey.cs:176\`
~~~csharp
0173: 			}
0174: 			if (list.Count > 0)
0175: 			{
0176: 				Core.Log?.Msg("快捷键映射：箱 uid " + uid + " 的" + (restock ? "一键上货键" : "一键收纳键") + "已由玩家主动解绑（Esc），释放 " + list.Count + " 个键。");
0177: 			}
0178: 			return list.Count > 0;
0179: 		}
~~~
- \`ItemStorage/BoxHotkey.cs:294\`
~~~csharp
0291: 			}
0292: 			else if (boundKey.Value.Restock)
0293: 			{
0294: 				Core.Debug("收纳箱快捷键 " + ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString() + " 触发一键上货。");
0295: 				BoxItem.ClickSound();
0296: 				BoxRestock.RunOne(val, "快捷键");
0297: 			}
~~~
- \`ItemStorage/BoxWindow.cs:426\`
~~~csharp
0423: 					{
0424: 					}
0425: 				});
0426: 				PixelElement val17 = (PixelElement)new MachineryButton("一键上货", action4, 48, 24);
0427: 				if (val17 == null)
0428: 				{
0429: 					Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮非 PixelElement");
~~~
- \`ItemStorage/BoxWindow.cs:429\`
~~~csharp
0426: 				PixelElement val17 = (PixelElement)new MachineryButton("一键上货", action4, 48, 24);
0427: 				if (val17 == null)
0428: 				{
0429: 					Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮非 PixelElement");
0430: 				}
0431: 				else if (!val6.Attach(val17, 2, 4, 48, 24))
0432: 				{
~~~

### 拖动物品到收纳箱上
- \`ItemStorage/BoxWindow.cs:676\`
~~~csharp
0673: 		{
0674: 			try
0675: 			{
0676: 				pack.shortDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
0677: 			}
0678: 			catch (Exception ex)
0679: 			{
~~~
- \`ItemStorage/BoxWindow.cs:685\`
~~~csharp
0682: 			try
0683: 			{
0684: 				string shortDescription = pack.shortDescription;
0685: 				if (!string.Equals(shortDescription, "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。", StringComparison.Ordinal))
0686: 				{
0687: 					Core.Debug("收纳箱 shortDescription 读回不一致：'" + Core.Clean(shortDescription) + "'");
0688: 				}
~~~
- \`ItemStorage/BoxWindow.cs:696\`
~~~csharp
0693: 			}
0694: 			try
0695: 			{
0696: 				pack.longDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
0697: 			}
0698: 			catch
0699: 			{
~~~

## 3829730904

### Uryu Ishida
- \`BrewingExpansion/Formal/Figurines.cs:12\`
~~~csharp
0009: 	{
0010: 		new FigurineDefinition("be_ichigo_figurine", "黑崎一护", "Ichigo Kurosaki", "橘发少年握紧斩魄刀，像下一秒就要跃出展示柜。守护重要之人的决心，被凝在这小小的身影里。拥有它，店铺吸引力 +100；同款不叠加。", "The orange-haired swordsman grips his blade, ready to leap from the display case. His resolve to protect those dear to him lives in this tiny figure. Adds 100 shop attractiveness; duplicates do not stack."),
0011: 		new FigurineDefinition("be_rukia_figurine", "朽木露琪亚", "Rukia Kuchiki", "黑色死霸装与白色围巾在风里扬起。她把勇气藏在平静的目光中，也把一段相遇留在了这座展示柜里。拥有它，店铺吸引力 +100；同款不叠加。", "Her black robes and white scarf catch an unseen breeze. A quiet gaze holds both courage and the memory of a fateful meeting. Adds 100 shop attractiveness; duplicates do not stack."),
0012: 		new FigurineDefinition("be_uryu_figurine", "石田雨龙", "Uryu Ishida", "眼镜后的目光沉着，灵弓的弦仿佛仍在发亮。他不必高声宣告自己的信念，只需把箭稳稳射向前方。拥有它，店铺吸引力 +100；同款不叠加。", "Behind his glasses, his gaze stays steady and the bowstring seems to glow. He lets a carefully aimed arrow speak for his convictions. Adds 100 shop attractiveness; duplicates do not stack."),
0013: 		new FigurineDefinition("be_iridium_sea_cucumber_figurine", "噢我的天啊是铱星大海参", "Oh My God, It's an Iridium Sea Cucumber", "从铱星的海底捞起一只大海参，连星露谷的渔夫都要惊呼：噢我的天啊！把它摆在店里，路过的人总会停下来看第二眼。拥有它，店铺吸引力 +100；同款不叠加。", "An iridium sea cucumber, hauled up from the depths near Stardew Valley. Even a seasoned fisher would cry, 'Oh my god!' Passersby always stop for a second look. Adds 100 shop attractiveness; duplicates do not stack.", 2)
0014: 	};
0015: 
~~~

### Ichigo Kurosaki
- \`BrewingExpansion/Formal/Figurines.cs:10\`
~~~csharp
0007: {
0008: 	internal static readonly FigurineDefinition[] All = new FigurineDefinition[4]
0009: 	{
0010: 		new FigurineDefinition("be_ichigo_figurine", "黑崎一护", "Ichigo Kurosaki", "橘发少年握紧斩魄刀，像下一秒就要跃出展示柜。守护重要之人的决心，被凝在这小小的身影里。拥有它，店铺吸引力 +100；同款不叠加。", "The orange-haired swordsman grips his blade, ready to leap from the display case. His resolve to protect those dear to him lives in this tiny figure. Adds 100 shop attractiveness; duplicates do not stack."),
0011: 		new FigurineDefinition("be_rukia_figurine", "朽木露琪亚", "Rukia Kuchiki", "黑色死霸装与白色围巾在风里扬起。她把勇气藏在平静的目光中，也把一段相遇留在了这座展示柜里。拥有它，店铺吸引力 +100；同款不叠加。", "Her black robes and white scarf catch an unseen breeze. A quiet gaze holds both courage and the memory of a fateful meeting. Adds 100 shop attractiveness; duplicates do not stack."),
0012: 		new FigurineDefinition("be_uryu_figurine", "石田雨龙", "Uryu Ishida", "眼镜后的目光沉着，灵弓的弦仿佛仍在发亮。他不必高声宣告自己的信念，只需把箭稳稳射向前方。拥有它，店铺吸引力 +100；同款不叠加。", "Behind his glasses, his gaze stays steady and the bowstring seems to glow. He lets a carefully aimed arrow speak for his convictions. Adds 100 shop attractiveness; duplicates do not stack."),
0013: 		new FigurineDefinition("be_iridium_sea_cucumber_figurine", "噢我的天啊是铱星大海参", "Oh My God, It's an Iridium Sea Cucumber", "从铱星的海底捞起一只大海参，连星露谷的渔夫都要惊呼：噢我的天啊！把它摆在店里，路过的人总会停下来看第二眼。拥有它，店铺吸引力 +100；同款不叠加。", "An iridium sea cucumber, hauled up from the depths near Stardew Valley. Even a seasoned fisher would cry, 'Oh my god!' Passersby always stop for a second look. Adds 100 shop attractiveness; duplicates do not stack.", 2)
~~~

### Rukia Kuchiki
- \`BrewingExpansion/Formal/Figurines.cs:11\`
~~~csharp
0008: 	internal static readonly FigurineDefinition[] All = new FigurineDefinition[4]
0009: 	{
0010: 		new FigurineDefinition("be_ichigo_figurine", "黑崎一护", "Ichigo Kurosaki", "橘发少年握紧斩魄刀，像下一秒就要跃出展示柜。守护重要之人的决心，被凝在这小小的身影里。拥有它，店铺吸引力 +100；同款不叠加。", "The orange-haired swordsman grips his blade, ready to leap from the display case. His resolve to protect those dear to him lives in this tiny figure. Adds 100 shop attractiveness; duplicates do not stack."),
0011: 		new FigurineDefinition("be_rukia_figurine", "朽木露琪亚", "Rukia Kuchiki", "黑色死霸装与白色围巾在风里扬起。她把勇气藏在平静的目光中，也把一段相遇留在了这座展示柜里。拥有它，店铺吸引力 +100；同款不叠加。", "Her black robes and white scarf catch an unseen breeze. A quiet gaze holds both courage and the memory of a fateful meeting. Adds 100 shop attractiveness; duplicates do not stack."),
0012: 		new FigurineDefinition("be_uryu_figurine", "石田雨龙", "Uryu Ishida", "眼镜后的目光沉着，灵弓的弦仿佛仍在发亮。他不必高声宣告自己的信念，只需把箭稳稳射向前方。拥有它，店铺吸引力 +100；同款不叠加。", "Behind his glasses, his gaze stays steady and the bowstring seems to glow. He lets a carefully aimed arrow speak for his convictions. Adds 100 shop attractiveness; duplicates do not stack."),
0013: 		new FigurineDefinition("be_iridium_sea_cucumber_figurine", "噢我的天啊是铱星大海参", "Oh My God, It's an Iridium Sea Cucumber", "从铱星的海底捞起一只大海参，连星露谷的渔夫都要惊呼：噢我的天啊！把它摆在店里，路过的人总会停下来看第二眼。拥有它，店铺吸引力 +100；同款不叠加。", "An iridium sea cucumber, hauled up from the depths near Stardew Valley. Even a seasoned fisher would cry, 'Oh my god!' Passersby always stop for a second look. Adds 100 shop attractiveness; duplicates do not stack.", 2)
0014: 	};
~~~
