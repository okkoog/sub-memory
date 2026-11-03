# Focused localization code contexts

Generated from ILSpy decompilation; excerpts are limited to localization-relevant callsites.

## 3832048841

### GameCardPK/GameCardFactory.cs:93-121
~~~csharp
0093: 		list.Add(MachineHelper.Panel("核心技能", (PixelElement)(object)val, "skill0", true));
0094: 		for (int i = 0; i < GameCardData.MaxSkillSlots; i++)
0095: 		{
0096: 			int num = i + 1;
0097: 			GameSlotInventory val2 = new GameSlotInventory
0098: 			{
0099: 				identifier = "skill" + num
0100: 			};
0101: 			ContentWindowRegistry.Register(item, "skill" + num, (GameInventory?)(object)val2);
0102: 			list.Add(MachineHelper.Panel("技能槽" + num, (PixelElement)(object)val2, "skill" + num, true));
0103: 		}
0104: 		item.SetContentWindow(MachineHelper.CreateMachineryWindow(true, list.ToArray()));
0105: 	}
0106: 
0107: 	internal static GameItem CreateCard(GameCardData gameCardData, CardVersionSet? versions = null)
0108: 	{
0109: 		if (gameCardData == null)
0110: 		{
0111: 			Core.Log?.Error("[卡牌PK] CreateCard 传入的卡牌数据为空，改用兜底卡。");
0112: 			gameCardData = CardDatabase.GetFirstOrDefault();
0113: 		}
0114: 		if (versions == null)
0115: 		{
0116: 			versions = new CardVersionSet();
0117: 		}
0118: 		bool flag = ApplyForcedVersions(gameCardData, versions);
0119: 		bool isVariant = gameCardData.IsVariant;
0120: 		VariantEntry variantEntry = null;
0121: 		if (versions.AltArt)
~~~

### GameCardPK/GameCardFactory.cs:142-241
~~~csharp
0142: 				else
0143: 				{
0144: 					versions.Add("VariantArt");
0145: 				}
0146: 			}
0147: 		}
0148: 		bool altArt = versions.AltArt;
0149: 		GameItem val = CreateShell();
0150: 		CardIdentity.Set(val, gameCardData.Id);
0151: 		string name = ((altArt && variantEntry != null && variantEntry.Name.Length > 0) ? variantEntry.Name : gameCardData.Name);
0152: 		val.SetName(name);
0153: 		string text = ((altArt && variantEntry != null && variantEntry.Description.Length > 0) ? variantEntry.Description : gameCardData.Description);
0154: 		if (!string.IsNullOrEmpty(text))
0155: 		{
0156: 			val.shortDescription = text;
0157: 		}
0158: 		int num = GameCardData.RollValue(gameCardData.HP, gameCardData.AttributeVariance, Rng);
0159: 		int num2 = GameCardData.RollValue(gameCardData.ATK, gameCardData.AttributeVariance, Rng);
0160: 		int num3 = GameCardData.RollValue(gameCardData.SPD, gameCardData.AttributeVariance, Rng);
0161: 		ChangeTag(TagIds[0], TagNames[0], val, num);
0162: 		ChangeTag(TagIds[1], TagNames[1], val, num2);
0163: 		ChangeTag(TagIds[2], TagNames[2], val, num3);
0164: 		string text2 = SeriesDatabase.DisplayNames(gameCardData.Series);
0165: 		if (!string.IsNullOrEmpty(text2))
0166: 		{
0167: 			ChangeTagString("Series", val, text2);
0168: 		}
0169: 		int num4 = GameCardData.RollSlotCount(gameCardData.SkillSlot, gameCardData.SlotVariance, Rng);
0170: 		ChangeTag("SkillSlotMax", "技能槽上限", val, num4);
0171: 		GameSlotInventory val2 = ContentWindowRegistry.Find(val, "skill0")?.TryCast<GameSlotInventory>();
0172: 		CardSkillData skill = BattleDatabase.GetSkill(gameCardData.GetSkillAt(1));
0173: 		if (skill != null && val2 != null)
0174: 		{
0175: 			GameItem val3 = CreateSkill(skill);
0176: 			val3.EnableTag(TagDisplay.NewTagDatas["CoreSkill"].Id, true);
0177: 			InventoryHelper.TrueAdd(val2, val3);
0178: 		}
0179: 		else
0180: 		{
0181: 			Core.Log?.Error("[卡牌PK] 卡牌 " + gameCardData.Name + " 的核心技能没有配置（或核心槽丢失）：" + gameCardData.GetSkillAt(1));
0182: 		}
0183: 		StampVersionTags(val, versions);
0184: 		CardPricing.Apply(val, num, num2, num3, num4, versions);
0185: 		if (versions.IsMisprint)
0186: 		{
0187: 			try
0188: 			{
0189: 				val.SetGameItemType("misprint_card");
0190: 			}
0191: 			catch (Exception ex)
0192: 			{
0193: 				Core.Debug("[卡牌PK] 设错版类型失败：" + ex.Message);
0194: 			}
0195: 		}
0196: 		Core.Debug("[卡牌PK] 生成卡牌 " + gameCardData.Name + "（" + ItemConfigInjector.BareId(gameCardData.Id) + "）：版本 " + versions.Describe() + "，技能槽上限 " + num4 + "（物理 " + GameCardData.MaxSkillSlots + " 槽），自带技能 " + gameCardData.Skills.Count + " 个，卡牌价值 " + CardPricing.ReadValue(val) + "。");
0197: 		if (altArt || isVariant)
0198: 		{
0199: 			MutationSystem.MarkAltArt(val);
0200: 		}
0201: 		return val;
0202: 	}
0203: 
0204: 	internal static GameItem CreateSkill(CardSkillData cardSkillData)
0205: 	{
0206: 		GameItem val = DirectoryMaster.Item(CardSkillItem, true);
0207: 		if (cardSkillData == null)
0208: 		{
0209: 			Core.Log?.Error("[卡牌PK] CreateSkill 传入的数据为空");
0210: 			return val;
0211: 		}
0212: 		ApplySkillData(val, cardSkillData);
0213: 		return val;
0214: 	}
0215: 
0216: 	internal static void ApplySkillData(GameItem item, CardSkillData cardSkillData)
0217: 	{
0218: 		if (item == null || cardSkillData == null)
0219: 		{
0220: 			return;
0221: 		}
0222: 		item.identifier = cardSkillData.Id;
0223: 		item.SetName(cardSkillData.Name);
0224: 		string text = SkillText.ForItem(cardSkillData);
0225: 		if (!string.IsNullOrEmpty(text))
0226: 		{
0227: 			item.shortDescription = text;
0228: 		}
0229: 		item.longDescription = text;
0230: 		if (cardSkillData.Value <= 0)
0231: 		{
0232: 			return;
0233: 		}
0234: 		try
0235: 		{
0236: 			item.SetValue((long)cardSkillData.Value);
0237: 		}
0238: 		catch (Exception ex)
0239: 		{
0240: 			Core.Debug("[卡牌PK] 技能条价值没写进去（" + cardSkillData.Id + "）：" + ex.Message);
0241: 		}
~~~

### GameCardPK/SkillText.cs:111-139
~~~csharp
0111: 			{
0112: 				break;
0113: 			}
0114: 			int num3 = template.IndexOf("}}", num2 + "{{".Length, StringComparison.Ordinal);
0115: 			if (num3 < 0)
0116: 			{
0117: 				break;
0118: 			}
0119: 			hashSet.Add(template.Substring(num2 + "{{".Length, num3 - num2 - "{{".Length).Trim());
0120: 			num = num3 + "}}".Length;
0121: 		}
0122: 		return hashSet;
0123: 	}
0124: 
0125: 	internal static string ForItem(CardSkillData? data)
0126: 	{
0127: 		if (data == null)
0128: 		{
0129: 			return string.Empty;
0130: 		}
0131: 		string text = data.Description ?? string.Empty;
0132: 		if (!HasPlaceholders(text))
0133: 		{
0134: 			return text;
0135: 		}
0136: 		try
0137: 		{
0138: 			Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.Ordinal);
0139: 			Walk(data, delegate(ActionData action, string key)
~~~

### GameCardPK/SkillText.cs:150-178
~~~csharp
0150: 					{
0151: 						Core.Debug("[技能文案] " + data.Id + " 的 " + key + " 带属性缩放（" + action.Value.ScaleAttr + "），场外只能填基础值 " + cell.Total + "。");
0152: 					}
0153: 				}
0154: 			});
0155: 			return Render(text, map);
0156: 		}
0157: 		catch (Exception ex)
0158: 		{
0159: 			Core.Debug("[技能文案] 场外解析失败（退回原文）：" + ex.Message);
0160: 			return text;
0161: 		}
0162: 	}
0163: 
0164: 	internal static string ForBattle(BattleSkillInstance? skill)
0165: 	{
0166: 		CardSkillData data = skill?.Config;
0167: 		if (data == null)
0168: 		{
0169: 			return string.Empty;
0170: 		}
0171: 		string text = data.Description ?? string.Empty;
0172: 		if (!HasPlaceholders(text))
0173: 		{
0174: 			return text;
0175: 		}
0176: 		try
0177: 		{
0178: 			Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.Ordinal);
~~~

### GameCardPK/SkillText.cs:734-762
~~~csharp
0734: 		foreach (AttrModifier attrModifier in array)
0735: 		{
0736: 			if (attrModifier != null && !string.IsNullOrEmpty(attrModifier.AttrKey))
0737: 			{
0738: 				map[attrModifier.AttrKey] = Signed(attrModifier.Value) + ((attrModifier.ModType == ModifyType.Percent) ? "%" : string.Empty);
0739: 			}
0740: 		}
0741: 		BuffEffectData[] array2 = data.Effects ?? Array.Empty<BuffEffectData>();
0742: 		for (int j = 0; j < array2.Length; j++)
0743: 		{
0744: 			BuffEffectData buffEffectData = array2[j];
0745: 			if (buffEffectData != null)
0746: 			{
0747: 				string text = ((j == 0) ? string.Empty : (":" + (j + 1)));
0748: 				map["point" + text] = PointLabel(buffEffectData.Point);
0749: 				if (buffEffectData.RepeatTimes > 1)
0750: 				{
0751: 					map["times" + text] = buffEffectData.RepeatTimes.ToString();
0752: 				}
0753: 				if (buffEffectData.Conditions != null && buffEffectData.Conditions.Length != 0)
0754: 				{
0755: 					map["cond" + text] = ConditionText(buffEffectData.Conditions);
0756: 				}
0757: 			}
0758: 		}
0759: 		BattleCardInstance owner = buff?.Owner;
0760: 		Walk(data, delegate(ActionData action, string key)
0761: 		{
0762: 			if (IsEventScaledNode(action))
~~~

### GameCardPK/SkillText.cs:1041-1069
~~~csharp
1041: 				}
1042: 				if (buffEffectData2.StackScaling)
1043: 				{
1044: 					flag = true;
1045: 				}
1046: 			}
1047: 		}
1048: 		if (flag && !hashSet.Contains("layers") && text.IndexOf("每层", StringComparison.Ordinal) < 0)
1049: 		{
1050: 			list.Add("效果按层数放大（stackScaling），文案既没有 {{layers}} 也没写「每层」");
1051: 		}
1052: 		return list;
1053: 	}
1054: 
1055: 	internal static string PointLabel(SkillStartPoint point)
1056: 	{
1057: 		return point switch
1058: 		{
1059: 			SkillStartPoint.BattleStart => "战斗开始时", 
1060: 			SkillStartPoint.RoundStart => "每轮开始", 
1061: 			SkillStartPoint.TurnStart => "自己回合开始", 
1062: 			SkillStartPoint.BeforeAttack => "出手前", 
1063: 			SkillStartPoint.AfterAttack => "出手后", 
1064: 			SkillStartPoint.BeforeDamage => "受到伤害前", 
1065: 			SkillStartPoint.AfterDamage => "受到伤害后", 
1066: 			SkillStartPoint.BeforeHeal => "被治疗前", 
1067: 			SkillStartPoint.AfterHeal => "被治疗后", 
1068: 			SkillStartPoint.OnHeal => "被治疗时", 
1069: 			SkillStartPoint.OnBuffAdd => "获得 Buff 时", 
~~~

## 3831837821

### MidnightPawn/Game/MaidenTooltip.cs:4-57
~~~csharp
0004: namespace MidnightPawn.Game;
0005: 
0006: public static class MaidenTooltip
0007: {
0008: 	public static void Register()
0009: 	{
0010: 		ModHook.OnCreateTooltipLate += OnTooltip;
0011: 	}
0012: 
0013: 	public static void Unregister()
0014: 	{
0015: 		ModHook.OnCreateTooltipLate -= OnTooltip;
0016: 	}
0017: 
0018: 	private static void OnTooltip(RichTextBuilder builder, GameItem item)
0019: 	{
0020: 		if (builder == null || item == null)
0021: 		{
0022: 			return;
0023: 		}
0024: 		if (item.IsTag("TENTACLE_BODY_TAG"))
0025: 		{
0026: 			try
0027: 			{
0028: 				int bodyLevel = TentacleBody.GetBodyLevel();
0029: 				int bodyExp = TentacleBody.GetBodyExp();
0030: 				int expNeed = TentacleBody.GetExpNeed();
0031: 				builder.AddEmptyLine(2);
0032: 				builder.AddLine($"触手本体 Lv.{bodyLevel}   经验 {bodyExp} / {expNeed}", false, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0033: 				builder.AddLine("携带它去垃圾场拾荒，有概率捕获少女。", false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0034: 				string modData = ModHelper.GetModData("lewd_meme", "last_report", "");
0035: 				if (!string.IsNullOrEmpty(modData))
0036: 				{
0037: 					builder.AddLine("昨晚：" + modData, false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0038: 				}
0039: 				return;
0040: 			}
0041: 			catch (Exception ex)
0042: 			{
0043: 				ModCtx.LogWarning("触手本体 tooltip 失败: " + ex.Message);
0044: 				return;
0045: 			}
0046: 		}
0047: 		if (!item.IsTag("MAIDEN_FEMALE"))
0048: 		{
0049: 			return;
0050: 		}
0051: 		try
0052: 		{
0053: 			string str = ItemFactory.GetStr(item, "MAIDEN_NAME");
0054: 			string str2 = ItemFactory.GetStr(item, "MAIDEN_FACTION");
0055: 			string str3 = ItemFactory.GetStr(item, "MAIDEN_JOB");
0056: 			string str4 = ItemFactory.GetStr(item, "MAIDEN_BREAST");
0057: 			string str5 = ItemFactory.GetStr(item, "MAIDEN_BODY");
~~~

### MidnightPawn/Game/MaidenTooltip.cs:89-117
~~~csharp
0089: 			if (item.IsTag("MAIDEN_ANAL_VIRGIN"))
0090: 			{
0091: 				stringBuilder.Append("【肛门处女】");
0092: 			}
0093: 			if (item.IsTag("MAIDEN_ANAL_VIRGIN_LOST"))
0094: 			{
0095: 				stringBuilder.Append("【肛门处女丧失】");
0096: 			}
0097: 			builder.AddLine(stringBuilder.ToString(), false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0098: 			int num = ItemFactory.GetInt(item, "TRAIN_VALUE");
0099: 			int num2 = ItemFactory.GetInt(item, "LACTATION_LV");
0100: 			int num3 = ItemFactory.GetInt(item, "ORAL_COUNT");
0101: 			int num4 = ItemFactory.GetInt(item, "SEX_COUNT");
0102: 			int num5 = ItemFactory.GetInt(item, "ANAL_COUNT");
0103: 			builder.AddLine($"调教值 {num}  泌乳等级 {num2}", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0104: 			builder.AddLine($"口交 {num3} 次 · 性交 {num4} 次 · 肛交 {num5} 次", false, (ColorPalette)12092762, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0105: 			if (item.IsTag("MAIDEN_OVULATING"))
0106: 			{
0107: 				builder.AddLine("【排卵期】", false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0108: 			}
0109: 			if (item.IsTag("MAIDEN_PREGNANT"))
0110: 			{
0111: 				int num6 = ItemFactory.GetInt(item, "MAIDEN_PREGNANT_DAYS");
0112: 				builder.AddLine($"【已怀孕】第 {num6}/30 天", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
0113: 			}
0114: 		}
0115: 		catch (Exception ex2)
0116: 		{
0117: 			ModCtx.LogWarning("少女 tooltip 追加失败: " + ex2.Message);
~~~

### MidnightPawn/Game/TentacleBody.cs:45-73
~~~csharp
0045: 			return;
0046: 		}
0047: 		if (ModHelper.GetModDataInt("lewd_meme", "last_settled_day", -1) == today)
0048: 		{
0049: 			ModCtx.Log("结算跳过：第 " + today + " 天已经结算过");
0050: 			return;
0051: 		}
0052: 		SettleAll();
0053: 		ModHelper.SetModDataInt("lewd_meme", "last_settled_day", today);
0054: 		ModCtx.Log("第 " + today + " 天结算完成");
0055: 	}
0056: 
0057: 	public static void SettleAll()
0058: 	{
0059: 		ModCtx.Log("【触手本体结算】第 " + GetToday() + " 天, 已登记本体=" + Instances.Count);
0060: 		if (Instances.Count == 0)
0061: 		{
0062: 			return;
0063: 		}
0064: 		ReportTypes.Clear();
0065: 		ReportMilk = 0;
0066: 		ReportSemen = 0;
0067: 		ReportMaids = 0;
0068: 		int num = 0;
0069: 		GameItem[] array = Instances.ToArray();
0070: 		foreach (GameItem val in array)
0071: 		{
0072: 			if (val == null)
0073: 			{
~~~

### MidnightPawn/Game/TentacleBody.cs:87-117
~~~csharp
0087: 					NightlyTick(val, list[j], output);
0088: 				}
0089: 				catch (Exception ex)
0090: 				{
0091: 					ModCtx.LogWarning("兜底结算失败: " + ex.Message);
0092: 				}
0093: 				if (((GameInventory)list[j]).childItems != null)
0094: 				{
0095: 					num += ((GameInventory)list[j]).childItems.Count;
0096: 				}
0097: 			}
0098: 		}
0099: 		if (num > 0)
0100: 		{
0101: 			AddBodyExp(num);
0102: 		}
0103: 		ShowReport();
0104: 	}
0105: 
0106: 	private static void CollectOutputs()
0107: 	{
0108: 		int num = 0;
0109: 		GameItem[] array = Instances.ToArray();
0110: 		foreach (GameItem val in array)
0111: 		{
0112: 			if (val == null)
0113: 			{
0114: 				continue;
0115: 			}
0116: 			try
0117: 			{
~~~

### MidnightPawn/Game/TentacleBody.cs:135-187
~~~csharp
0135: 					}
0136: 				}
0137: 			}
0138: 			catch (Exception ex)
0139: 			{
0140: 				ModCtx.LogWarning("收取产出失败: " + ex.Message);
0141: 			}
0142: 		}
0143: 		if (num > 0)
0144: 		{
0145: 			ModCtx.Log("产出已收入背包: " + num + " 件");
0146: 		}
0147: 	}
0148: 
0149: 	private static void ShowReport()
0150: 	{
0151: 		if (ReportTypes.Count == 0 && ReportMilk == 0 && ReportSemen == 0)
0152: 		{
0153: 			return;
0154: 		}
0155: 		StringBuilder stringBuilder = new StringBuilder();
0156: 		if (ReportTypes.Count > 0)
0157: 		{
0158: 			int num = 0;
0159: 			foreach (KeyValuePair<string, int> reportType in ReportTypes)
0160: 			{
0161: 				if (num > 0)
0162: 				{
0163: 					stringBuilder.Append("和");
0164: 				}
0165: 				stringBuilder.Append(reportType.Value).Append("个【").Append(reportType.Key)
0166: 					.Append("】");
0167: 				num++;
0168: 			}
0169: 			stringBuilder.Append("在【触手本体】内被你的触手调教了一夜");
0170: 		}
0171: 		else
0172: 		{
0173: 			stringBuilder.Append("触手本体蠕动了整夜");
0174: 		}
0175: 		List<string> list = new List<string>();
0176: 		if (ReportMilk > 0)
0177: 		{
0178: 			list.Add("【神奇乳汁】*" + ReportMilk);
0179: 		}
0180: 		if (ReportSemen > 0)
0181: 		{
0182: 			list.Add("【触手精液】*" + ReportSemen);
0183: 		}
0184: 		if (list.Count > 0)
0185: 		{
0186: 			stringBuilder.Append("，产出了 ").Append(string.Join("，", list.ToArray()));
0187: 		}
~~~

### MidnightPawn/Game/TentacleBody.cs:240-268
~~~csharp
0240: 			ModCtx.Log("夜间报告: 窗口已弹出");
0241: 		}
0242: 		catch (Exception ex)
0243: 		{
0244: 			ModCtx.LogWarning("夜间报告弹窗异常: " + ex.Message);
0245: 		}
0246: 	}
0247: 
0248: 	public static GameItem Create()
0249: 	{
0250: 		List<GameGridInventory> grids;
0251: 		PixelWindow contentWindow = BuildWindow(GetBodyLevel(), out grids);
0252: 		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_body");
0253: 		val.SetContentWindow(contentWindow);
0254: 		val.name = "触手本体";
0255: 		val.shortDescription = "这是什么？这是你的本体！可爱又能干的触手怪！让可爱的少女进入其中让她幸福。将这个太空站改造成为你的个人苗床巢吧。";
0256: 		val.flavorText = "";
0257: 		val.unitValue = 0L;
0258: 		val.SetGameItemType("TOOL");
0259: 		val.EnableTag("STANDARD_MACHINE_TAG", false);
0260: 		val.EnableTag("TENTACLE_BODY_TAG", false);
0261: 		val.EnableTag("IMPORTANT_TAG", false);
0262: 		val.EnableTag("NOT_FOR_RESALE", false);
0263: 		val.EnableTag("ITEM_HIDDEN_TAG", false);
0264: 		string text = SpriteInjector.InjectModTexture("tentacle_body", 48, 48);
0265: 		if (text != null)
0266: 		{
0267: 			val.SetSpriteAndShapeFromMod("lewd_meme", text);
0268: 		}
~~~

### MidnightPawn/Game/TentacleBody.cs:278-306
~~~csharp
0278: 		((GameItemFunc)val).onLoaded = delegate(GameItem it)
0279: 		{
0280: 			if (!Instances.Contains(it))
0281: 			{
0282: 				Instances.Add(it);
0283: 			}
0284: 		};
0285: 		if (!Instances.Contains(val))
0286: 		{
0287: 			Instances.Add(val);
0288: 		}
0289: 		Slots[val] = grids;
0290: 		try
0291: 		{
0292: 			ModCtx.Log($"触手本体 shape = {((val.shape != null) ? val.shape.width : (-1))}x{((val.shape != null) ? val.shape.height : (-1))} (global {((val.shape != null) ? val.shape.globalWidth : (-1))}x{((val.shape != null) ? val.shape.globalHeight : (-1))})");
0293: 		}
0294: 		catch
0295: 		{
0296: 		}
0297: 		return val;
0298: 	}
0299: 
0300: 	private static PixelWindow BuildWindow(int level, out List<GameGridInventory> grids)
0301: 	{
0302: 		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
0303: 		//IL_001b: Expected O, but got Unknown
0304: 		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
0305: 		//IL_008e: Expected O, but got Unknown
0306: 		grids = new List<GameGridInventory>();
~~~

### MidnightPawn/Game/TentacleBody.cs:709-746
~~~csharp
0709: 		}
0710: 		return EXP_NEED[bodyLevel - 1];
0711: 	}
0712: 
0713: 	public static int GetBodyLevel()
0714: 	{
0715: 		return Mathf.Clamp(ModHelper.GetModDataInt("lewd_meme", "body_lv", 1), 1, 99);
0716: 	}
0717: 
0718: 	public static int GetBodyExp()
0719: 	{
0720: 		return ModHelper.GetModDataInt("lewd_meme", "body_exp", 0);
0721: 	}
0722: 
0723: 	private static void AddBodyExp(int amount)
0724: 	{
0725: 		int num = GetBodyLevel();
0726: 		int num2 = GetBodyExp() + amount;
0727: 		int num3 = num;
0728: 		while (num - 1 < EXP_NEED.Length && num2 >= EXP_NEED[num - 1])
0729: 		{
0730: 			num2 -= EXP_NEED[num - 1];
0731: 			num++;
0732: 			ModCtx.Log("触手本体升级到 Lv." + num + "（苗床容量与性爱强度提升）");
0733: 		}
0734: 		if (num > num3)
0735: 		{
0736: 			RebuildAllWindows();
0737: 		}
0738: 		ModHelper.SetModDataInt("lewd_meme", "body_lv", num);
0739: 		ModHelper.SetModDataInt("lewd_meme", "body_exp", num2);
0740: 	}
0741: 
0742: 	private static bool TryProduce(GameGridInventory output, string itemId)
0743: 	{
0744: 		if (output == null)
0745: 		{
0746: 			return false;
~~~

## 3829429358

### XIAOWONightAuction/AuctionText.cs:1-23
~~~csharp
0001: using System;
0002: using System.Collections.Generic;
0003: using System.Text.RegularExpressions;
0004: 
0005: namespace XIAOWONightAuction;
0006: 
0007: internal static class AuctionText
0008: {
0009: 	private static readonly (string Chinese, string English)[] LogPhrases = new(string, string)[64]
0010: 	{
0011: 		("拍卖师：", "Auctioneer: "),
0012: 		("你已退出本箱竞拍，本箱计入今晚的竞拍场次。未扣除箱款。", "You withdrew. This lot counts toward tonight's auctions. No payment was taken."),
0013: 		("AI已完成剩余竞价，省略中间播报。", "AI bidding completed; intermediate announcements skipped."),
0014: 		("退出：资金不足", "Out: insufficient funds"),
0015: 		("退出：超过心理价", "Out: above my limit"),
0016: 		("退出：抬价后收手", "Out: bluff ended"),
0017: 		("退出：不再冒险", "Out: no more risk"),
0018: 		("无人达到起拍价，本箱流拍。", "No bid reached the opening price. Lot unsold."),
0019: 		("无人提交有效报价，本箱流拍。", "No valid bids were submitted. Lot unsold."),
0020: 		("未达到本轮领先条件，进入", "Lead threshold not met. Proceed to "),
0021: 		("竞拍，最多五轮；领先足够即可提前成交。", " auction, up to five rounds. A sufficient lead wins early."),
0022: 		("最多五轮，每轮揭价；领先足够可提前成交。", "Up to five rounds. Bids are revealed each round; a sufficient lead wins early."),
0023: 		("内部线索：确认存在高价值物品。", "Inside tip: high-value goods confirmed."),
~~~

### XIAOWONightAuction/AuctionText.cs:63-91
~~~csharp
0063: 		("赌徒", "Gambler"),
0064: 		("搅局者", "Disruptor"),
0065: 		("暗标", "Sealed"),
0066: 		("公开", "Open"),
0067: 		("流拍", "Unsold"),
0068: 		("你报价 ", "You bid "),
0069: 		("你未报价", "You did not bid"),
0070: 		("报价 ", "bid "),
0071: 		("以 ", " bid "),
0072: 		(" 至 ", " to "),
0073: 		("你", "You"),
0074: 		("：", ": ")
0075: 	};
0076: 
0077: 	private static readonly Dictionary<string, string> English = new Dictionary<string, string>(StringComparer.Ordinal)
0078: 	{
0079: 		["cash"] = "CASH  {0}",
0080: 		["map_entry"] = "CONTAINER AUCTION",
0081: 		["title"] = "NIGHT CONTAINER AUCTION",
0082: 		["return_store"] = "Return to Shop",
0083: 		["join_bid"] = "Enter Auction",
0084: 		["cannot_bid"] = "Unavailable",
0085: 		["category_medical"] = "Medical Container",
0086: 		["category_luxury"] = "Luxury Container",
0087: 		["category_contraband"] = "Contraband Container",
0088: 		["category_engineering"] = "Engineering Container",
0089: 		["container_description"] = "A sealed auction container. Fill its extraction tray; items cannot be deposited.",
0090: 		["container_flavor"] = "The seal answers only to the auction house.",
0091: 		["container_empty"] = "This auction container is empty.",
~~~

### XIAOWONightAuction/AuctionText.cs:115-178
~~~csharp
0115: 
0116: 	internal static bool IsChinese(string localeCode)
0117: 	{
0118: 		if (!string.IsNullOrWhiteSpace(localeCode))
0119: 		{
0120: 			if (!localeCode.Equals("zh", StringComparison.OrdinalIgnoreCase) && !localeCode.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
0121: 			{
0122: 				return localeCode.StartsWith("zh_", StringComparison.OrdinalIgnoreCase);
0123: 			}
0124: 			return true;
0125: 		}
0126: 		return false;
0127: 	}
0128: 
0129: 	internal static string Select(string chinese, string english)
0130: 	{
0131: 		return Select(CurrentLocaleCode, chinese, english);
0132: 	}
0133: 
0134: 	internal static string Select(string localeCode, string chinese, string english)
0135: 	{
0136: 		if (!IsChinese(localeCode))
0137: 		{
0138: 			return english;
0139: 		}
0140: 		return chinese;
0141: 	}
0142: 
0143: 	internal static string BidderName(int index)
0144: 	{
0145: 		return index switch
0146: 		{
0147: 			0 => Select("稳健客", "Cautious"), 
0148: 			1 => Select("赌徒", "Gambler"), 
0149: 			2 => Select("搅局者", "Disruptor"), 
0150: 			_ => Select("你", "You"), 
0151: 		};
0152: 	}
0153: 
0154: 	internal static string DisplayLog(string value)
0155: 	{
0156: 		if (IsChinese(CurrentLocaleCode) || string.IsNullOrEmpty(value))
0157: 		{
0158: 			return value;
0159: 		}
0160: 		value = Regex.Replace(value, "第(\\d+)轮揭价", "Round $1 bids revealed");
0161: 		value = Regex.Replace(value, "第(\\d+)轮", "Round $1");
0162: 		value = value.Replace(" · ", " - ", StringComparison.Ordinal);
0163: 		value = value.Replace("轮。", ".", StringComparison.Ordinal);
0164: 		(string, string)[] logPhrases = LogPhrases;
0165: 		for (int i = 0; i < logPhrases.Length; i++)
0166: 		{
0167: 			(string, string) tuple = logPhrases[i];
0168: 			string item = tuple.Item1;
0169: 			string item2 = tuple.Item2;
0170: 			value = value.Replace(item, item2, StringComparison.Ordinal);
0171: 		}
0172: 		value = value.Replace("。", ".", StringComparison.Ordinal);
0173: 		return value;
0174: 	}
0175: 
0176: 	internal static string Get(string key, string fallback, params object[] args)
0177: 	{
0178: 		string text = ModHelper.GetLocalized("xiaowo_night_auction", key, Array.Empty<object>());
~~~

### XIAOWONightAuction/AuctionPhone.cs:16-44
~~~csharp
0016: 	internal const long Number = 4367L;
0017: 
0018: 	internal const string Atlas = "XIAOWO/NightAuctionPhone";
0019: 
0020: 	private static int _tick;
0021: 
0022: 	private static bool _storeWorkPending;
0023: 
0024: 	private static int _storeWorkFailures;
0025: 
0026: 	private static bool _warned;
0027: 
0028: 	private static PaperUI _paper;
0029: 
0030: 	internal static string ContactName => AuctionText.Select("夜间拍卖行", "Night Auction House");
0031: 
0032: 	internal static bool CardGiven => AuctionState.Read("phone_card_given") == "1";
0033: 
0034: 	internal static int ReadyDay
0035: 	{
0036: 		get
0037: 		{
0038: 			if (!int.TryParse(AuctionState.Read("phone_ready_day"), out var result))
0039: 			{
0040: 				return 0;
0041: 			}
0042: 			return result;
0043: 		}
0044: 	}
~~~

### XIAOWONightAuction/AuctionPhone.cs:121-176
~~~csharp
0121: 		_storeWorkFailures = 0;
0122: 		_warned = false;
0123: 		_tick = 0;
0124: 		Tick();
0125: 	}
0126: 
0127: 	internal static GameItem CreateCard(bool owned)
0128: 	{
0129: 		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
0130: 		GameItem obj = DirectoryMaster.Item("card_pharma", owned) ?? throw new InvalidOperationException("Native business card is unavailable.");
0131: 		obj.identifier = "xiaowo_auction_phone_card";
0132: 		OfficialAssets.SetSprite(obj, "auction_card_closed");
0133: 		obj.shape = new GridShapeBuilder(2, 1).SetDataFill(2, 1, (byte)1).Build();
0134: 		obj.modifiedShape = obj.shape.Clone();
0135: 		LocalizeCard(obj);
0136: 		((GameItemFunc)obj).onLoaded = MonoCallbacks.Bind<Action<GameItem>>(new Action<GameItem>(LocalizeCard));
0137: 		obj.ValidateShapeState();
0138: 		return obj;
0139: 	}
0140: 
0141: 	private static void LocalizeCard(GameItem item)
0142: 	{
0143: 		string identifierName = (item.name = AuctionText.Select("夜间拍卖行名片", "Night Auction Business Card"));
0144: 		item.identifierName = identifierName;
0145: 		identifierName = (item.longDescription = AuctionText.Select("电话：4367。四类收货商仅接受电话预约，次日到店。通常第3天可再预约；预约核心伙伴时缩短为第2天。赠送对应类别的礼物可提升合作关系。", "Phone: 4367. Four specialist buyers visit only by appointment, arriving tomorrow. Book again after 3 days, or 2 days when booking a Trusted Partner. Give matching gifts to improve relations."));
0146: 		item.shortDescription = identifierName;
0147: 		item.flavorText = AuctionText.Select("有货要出，拨这个号码。", "Have goods to sell? Call this number.");
0148: 	}
0149: 
0150: 	internal static void RefreshOwnedText()
0151: 	{
0152: 		PlayerStore instance = PlayerStore.Instance;
0153: 		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
0154: 		if (list == null)
0155: 		{
0156: 			return;
0157: 		}
0158: 		foreach (GameItem item in list)
0159: 		{
0160: 			if (((item != null) ? item.identifier : null) == "xiaowo_auction_phone_card")
0161: 			{
0162: 				LocalizeCard(item);
0163: 			}
0164: 		}
0165: 	}
0166: 
0167: 	internal static bool DeliverCard()
0168: 	{
0169: 		if (CardGiven)
0170: 		{
0171: 			return true;
0172: 		}
0173: 		PlayerStore instance = PlayerStore.Instance;
0174: 		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
0175: 		if (list == null || (Object)(object)EmporiumEntry.Instance == (Object)null)
0176: 		{
~~~

### XIAOWONightAuction/AuctionPhone.cs:235-263
~~~csharp
0235: 		}
0236: 		if (!instance.PhoneClientDict.TryGetValue(4367L, out var value))
0237: 		{
0238: 			value = new StorePhoneClient
0239: 			{
0240: 				dialogFuncId = "xiaowo_night_auction_phone",
0241: 				locID = "xiaowo_night_auction_phone"
0242: 			};
0243: 			instance.PhoneClientDict.Add(4367L, value);
0244: 		}
0245: 		if (value.dialogFuncId != "xiaowo_night_auction_phone")
0246: 		{
0247: 			throw new InvalidOperationException("电话号码4367已被其他联系人占用。");
0248: 		}
0249: 		value.displayName = ContactName;
0250: 		value.locID = "xiaowo_night_auction_phone";
0251: 		value.phoneClientType = (PhoneClientType)0;
0252: 		value.cooldownDuration = ((int.TryParse(AuctionState.Read("phone_call_interval"), out var result) && result == 2) ? 2 : 3);
0253: 		value.currentCooldown = Math.Max(0, ReadyDay - AuctionCatalog.Day);
0254: 		value.phoneState = (PhoneState)((value.currentCooldown > 0) ? 5 : 4);
0255: 		return value;
0256: 	}
0257: 
0258: 	internal static bool TryPending(out int dueDay, out ContainerCategory category)
0259: 	{
0260: 		dueDay = 0;
0261: 		category = ContainerCategory.Medical;
0262: 		string[] array = AuctionState.Read("phone_appointment").Split(',');
0263: 		if (array.Length != 2 || !int.TryParse(array[0], out dueDay) || !int.TryParse(array[1], out var result) || result < 0 || result > 3)
~~~

### XIAOWONightAuction/AuctionPhone.cs:285-391
~~~csharp
0285: 		message = AuctionMerchants.DisplayName(category) + AuctionText.Select("明天到店，预算 ", " will visit tomorrow. Budget: ") + AuctionMerchantRelations.Budget(category).ToString("N0") + AuctionText.Select("¢。第", "¢. Call again on day ") + ReadyDay + AuctionText.Select("天恢复预约。", ".");
0286: 		Core.Log?.Msg("[拍卖电话] 预约 " + category.ToString() + "，到访日=" + (day + 1) + "，下次预约日=" + ReadyDay + ".");
0287: 		return true;
0288: 	}
0289: 
0290: 	private static string UnavailableText()
0291: 	{
0292: 		if (TryPending(out var dueDay, out var category))
0293: 		{
0294: 			return AuctionMerchants.DisplayName(category) + AuctionText.Select("已预约，第", " is booked for day ") + dueDay + AuctionText.Select("天到店。", ".");
0295: 		}
0296: 		return AuctionText.Select("采购团队还在周转，第", "Our buyers are busy. Call again on day ") + ReadyDay + AuctionText.Select("天恢复预约。", ".");
0297: 	}
0298: 
0299: 	internal static Dialogue BuildDialogue()
0300: 	{
0301: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
0302: 		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
0303: 		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
0304: 		EnsureContact();
0305: 		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行服务。请选择：", "Night Auction House services. Please choose:"));
0306: 		AddServiceChoice(val, AuctionText.Select("预约收货商人", "Book a Buyer"), BuildBookingDialogue(), null);
0307: 		AddServiceChoice(val, AuctionText.Select("采购委托", "Procurement Orders"), AuctionCommissions.DialoguePage(), null);
0308: 		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("顾问与档案服务。", "Advisors and records."));
0309: 		Dialogue val3 = new Dialogue().SetText(ContactName, AuctionText.Select("档案服务。", "Records and collections."));
0310: 		AddServiceChoice(val3, AuctionText.Select("藏品收集册", "Collection Book"), Ending(string.Empty), AuctionController.RequestPhoneCollection);
0311: 		AddServiceChoice(val3, AuctionText.Select("合作商人档案", "Buyer Relationships"), Ending(string.Empty), AuctionController.RequestPhoneRelationships);
0312: 		AddServiceChoice(val3, AuctionText.Select("返回", "Back"), val2, null);
0313: 		AddServiceChoice(val2, AuctionText.Select("拍卖行顾问", "Auction Advisors"), AuctionNightMarket.BuildAdvisorDialogue(val2), null);
0314: 		AddServiceChoice(val2, AuctionText.Select("藏品 / 商人档案", "Collection / Buyer Records"), val3, null);
0315: 		AddServiceChoice(val2, AuctionText.Select("返回", "Back"), val, null);
0316: 		AddServiceChoice(val, AuctionText.Select("顾问 / 档案服务", "Advisors / Records"), val2, null);
0317: 		return val;
0318: 	}
0319: 
0320: 	internal static Dialogue BuildBookingDialogue()
0321: 	{
0322: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
0323: 		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
0324: 		EnsureContact();
0325: 		if (!AuctionPhoneRules.CanReserve(AuctionCatalog.Day, ReadyDay, TryPending(out var _, out var _)))
0326: 		{
0327: 			return Ending(UnavailableText());
0328: 		}
0329: 		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行收货专线。需要哪一类采购员？确认后明天到店。", "Night Auction buying desk. Which specialist do you need? Confirm a booking for tomorrow."));
0330: 		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("其他采购类别。", "Other buying categories."));
0331: 		AddBooking(val, ContainerCategory.Medical);
0332: 		AddBooking(val, ContainerCategory.Luxury);
0333: 		AddPhoneChoice(val, AuctionText.Select("违禁品类 / 工程类", "Contraband / Engineering"), val2);
0334: 		AddBooking(val2, ContainerCategory.Contraband);
0335: 		AddBooking(val2, ContainerCategory.Engineering);
0336: 		AddPhoneChoice(val2, AuctionText.Select("返回", "Back"), val);
0337: 		return val;
0338: 	}
0339: 
0340: 	private static void AddBooking(Dialogue dialogue, ContainerCategory category)
0341: 	{
0342: 		Dialogue confirmation = Ending(string.Empty);
0343: 		Action action = MonoCallbacks.Bind<Action>((Action)delegate
0344: 		{
0345: 			Reserve(category, out var message);
0346: 			confirmation.SetText(ContactName, message);
0347: 		});
0348: 		AddPhoneChoice(dialogue, CategoryCaption(category) + " · " + AuctionMerchantRelations.TierName(category), confirmation, action);
0349: 	}
0350: 
0351: 	internal static string CategoryCaption(ContainerCategory category)
0352: 	{
0353: 		return category switch
0354: 		{
0355: 			ContainerCategory.Medical => AuctionText.Select("医疗类", "Medical goods"), 
0356: 			ContainerCategory.Luxury => AuctionText.Select("奢侈品类", "Luxury goods"), 
0357: 			ContainerCategory.Contraband => AuctionText.Select("违禁品类", "Contraband"), 
0358: 			_ => AuctionText.Select("工程类", "Engineering goods"), 
0359: 		};
0360: 	}
0361: 
0362: 	private static void AddPhoneChoice(Dialogue dialogue, string caption, Dialogue next, Action action = null)
0363: 	{
0364: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
0365: 		Dialogue val = new Dialogue().SetText("PLAYER", caption).SetNextDialogue(next);
0366: 		dialogue.AddChoice(ItemDirectory.CreateEmptyItem((string)null), caption, "", val, action, (Func<bool>)null);
0367: 	}
0368: 
0369: 	internal static void AddServiceChoice(Dialogue dialogue, string caption, Dialogue next, Action action)
0370: 	{
0371: 		AddPhoneChoice(dialogue, caption, next, (action == null) ? null : MonoCallbacks.Bind<Action>(action));
0372: 	}
0373: 
0374: 	internal static Dialogue Ending(string text)
0375: 	{
0376: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
0377: 		Dialogue obj = new Dialogue().SetText(ContactName, text);
0378: 		obj.SetEndAction(MonoCallbacks.Bind<Action>((Action)delegate
0379: 		{
0380: 			PhoneUIManager instance = PhoneUIManager.Instance;
0381: 			if (instance != null)
0382: 			{
0383: 				instance.OnPhoneDialogEnd(true);
0384: 			}
0385: 		}));
0386: 		return obj;
0387: 	}
0388: 
0389: 	internal static void HandleVisitors(StoreClientManager manager)
0390: 	{
0391: 		int day = AuctionCatalog.Day;
~~~

## 3829085054

### ItemStorage/BoxWindow.cs:142-170
~~~csharp
0142: 		{
0143: 			bool flag = false;
0144: 			try
0145: 			{
0146: 				flag = pack.IsTag("ITEM_STORAGE_BOX_UI_V30");
0147: 			}
0148: 			catch
0149: 			{
0150: 			}
0151: 			Dictionary<string, AdvButtonElement> dictionary = FindIn(ButtonsByRef, ButtonsByUid, pack);
0152: 			if (pack.contentWindow != null && flag && dictionary != null)
0153: 			{
0154: 				TrackBox(pack);
0155: 				SyncHotkeyMap(pack);
0156: 				RefreshButtons(pack);
0157: 				return true;
0158: 			}
0159: 		}
0160: 		catch
0161: 		{
0162: 		}
0163: 		try
0164: 		{
0165: 			pack.SetContentWindow((PixelWindow)null);
0166: 		}
0167: 		catch
0168: 		{
0169: 		}
0170: 		string text = "收纳箱";
~~~

### ItemStorage/BoxWindow.cs:220-248
~~~csharp
0220: 				Core.Log?.Warning("收纳箱[" + context + "] 标题栏齿轮挂载失败：" + ex.Message);
0221: 			}
0222: 			try
0223: 			{
0224: 				Sprite val4 = SpriteAssets.Get("Dream_Item_Storage_Tidy");
0225: 				if ((Object)(object)val4 != (Object)null)
0226: 				{
0227: 					MachineryButton val5 = new MachineryButton(val4, MakeAction(delegate
0228: 					{
0229: 						try
0230: 						{
0231: 							int num4 = BoxTidy.Tidy(pack);
0232: 							if (num4 > 0)
0233: 							{
0234: 								RefreshButtons(pack);
0235: 								BoxItem.NotifyUser("已整理：挪动 " + num4 + " 件");
0236: 							}
0237: 							else
0238: 							{
0239: 								BoxItem.NotifyUser("目前顺序已经无法整理了");
0240: 							}
0241: 						}
0242: 						catch (Exception ex9)
0243: 						{
0244: 							Core.Log?.Warning("收纳箱[" + context + "] 一键整理失败：" + ex9.Message);
0245: 						}
0246: 					}), 16);
0247: 					val.AddHeaderButton(val5);
0248: 				}
~~~

### ItemStorage/BoxWindow.cs:334-494
~~~csharp
0334: 				}
0335: 			}
0336: 			try
0337: 			{
0338: 				Action action = MakeAction(delegate
0339: 				{
0340: 					try
0341: 					{
0342: 						AutoStow.Run(pack, "按钮");
0343: 					}
0344: 					catch
0345: 					{
0346: 					}
0347: 				});
0348: 				PixelElement val14 = (PixelElement)new MachineryButton("一键收纳", action, 48, 24);
0349: 				if (val14 == null)
0350: 				{
0351: 					Core.Log?.Warning("收纳箱[" + context + "] 一键收纳按钮非 PixelElement");
0352: 				}
0353: 				else if (!val6.Attach(val14, 3, 4, 48, 24))
0354: 				{
0355: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键收纳按钮失败");
0356: 				}
0357: 			}
0358: 			catch (Exception ex4)
0359: 			{
0360: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳按钮构建失败：" + ex4.Message);
0361: 			}
0362: 			try
0363: 			{
0364: 				Action action2 = MakeAction(delegate
0365: 				{
0366: 					try
0367: 					{
0368: 						BoxStowAll.RunAll("按钮");
0369: 					}
0370: 					catch
0371: 					{
0372: 					}
0373: 				});
0374: 				PixelElement val15 = (PixelElement)new MachineryButton("全箱收纳", action2, 48, 24);
0375: 				if (val15 == null)
0376: 				{
0377: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮非 PixelElement");
0378: 				}
0379: 				else if (!val6.Attach(val15, 0, 4, 48, 24))
0380: 				{
0381: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱收纳按钮失败");
0382: 				}
0383: 			}
0384: 			catch (Exception ex5)
0385: 			{
0386: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮构建失败：" + ex5.Message);
0387: 			}
0388: 			try
0389: 			{
0390: 				Action action3 = MakeAction(delegate
0391: 				{
0392: 					try
0393: 					{
0394: 						BoxRestock.RunAllBoxes("按钮");
0395: 					}
0396: 					catch
0397: 					{
0398: 					}
0399: 				});
0400: 				PixelElement val16 = (PixelElement)new MachineryButton("全箱上货", action3, 48, 24);
0401: 				if (val16 == null)
0402: 				{
0403: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮非 PixelElement");
0404: 				}
0405: 				else if (!val6.Attach(val16, 1, 4, 48, 24))
0406: 				{
0407: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱上货按钮失败");
0408: 				}
0409: 			}
0410: 			catch (Exception ex6)
0411: 			{
0412: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮构建失败：" + ex6.Message);
0413: 			}
0414: 			try
0415: 			{
0416: 				Action action4 = MakeAction(delegate
0417: 				{
0418: 					try
0419: 					{
0420: 						BoxRestock.RunOne(pack, "按钮");
0421: 					}
0422: 					catch
0423: 					{
0424: 					}
0425: 				});
0426: 				PixelElement val17 = (PixelElement)new MachineryButton("一键上货", action4, 48, 24);
0427: 				if (val17 == null)
0428: 				{
0429: 					Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮非 PixelElement");
0430: 				}
0431: 				else if (!val6.Attach(val17, 2, 4, 48, 24))
0432: 				{
0433: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键上货按钮失败");
0434: 				}
0435: 			}
0436: 			catch (Exception ex7)
0437: 			{
0438: 				Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮构建失败：" + ex7.Message);
0439: 			}
0440: 			SyncHotkeyMap(pack);
0441: 			StoreIn(ButtonsByRef, ButtonsByUid, pack, dictionary2);
0442: 			StoreIn(LabelsByRef, LabelsByUid, pack, dictionary3);
0443: 			StoreIn(IconsByRef, IconsByUid, pack, dictionary4);
0444: 			StoreIn(ClicksByRef, ClicksByUid, pack, dictionary5);
0445: 			TrackBox(pack);
0446: 			PixelElement val18 = (PixelElement)(object)val6;
0447: 			if (val18 == null)
0448: 			{
0449: 				Core.Log?.Warning("收纳箱[" + context + "]：carrier 不是 PixelElement");
0450: 				return false;
0451: 			}
0452: 			val.Attach(val18);
0453: 			pack.SetContentWindow(val);
0454: 			try
0455: 			{
0456: 				pack.EnableTag("ITEM_STORAGE_BOX_UI_V30", false);
0457: 			}
0458: 			catch
0459: 			{
0460: 			}
0461: 			try
0462: 			{
0463: 				val.Validate();
0464: 			}
0465: 			catch
0466: 			{
0467: 			}
0468: 			RefreshButtons(pack);
0469: 			QueueIconRefresh(pack);
0470: 			Core.Debug("收纳箱[" + context + "]：窗口构建完成（4×4 十六槽全满 + 网格下方横排一键收纳按钮 48×24）");
0471: 			return true;
0472: 		}
0473: 		catch (Exception ex8)
0474: 		{
0475: 			Core.Log?.Warning("收纳箱[" + context + "] EnsureWindow：" + ex8.GetType().Name + ": " + ex8.Message);
0476: 			return false;
0477: 		}
0478: 	}
0479: 
0480: 	internal static void RefreshButtons(GameItem pack)
0481: 	{
0482: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
0483: 		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
0484: 		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
0485: 		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
0486: 		if (pack == null)
0487: 		{
0488: 			return;
0489: 		}
0490: 		try
0491: 		{
0492: 			Dictionary<string, AdvButtonElement> dictionary = FindIn(ButtonsByRef, ButtonsByUid, pack);
0493: 			if (dictionary == null)
0494: 			{
~~~

### ItemStorage/BoxWindow.cs:523-551
~~~csharp
0523: 						}
0524: 						catch
0525: 						{
0526: 						}
0527: 					}
0528: 					catch
0529: 					{
0530: 					}
0531: 				}
0532: 				if (dictionary3 != null && dictionary3.TryGetValue(i.ToString(), out var value2) && (Object)(object)value2 != (Object)null)
0533: 				{
0534: 					UpdateSlotIcon(pack, i, value2);
0535: 				}
0536: 			}
0537: 			UpdateTooltip(pack);
0538: 		}
0539: 		catch (Exception ex)
0540: 		{
0541: 			Core.Log?.Warning("收纳箱 RefreshButtons：" + ex.Message);
0542: 		}
0543: 	}
0544: 
0545: 	internal static void ForceRebuildWindow(GameItem pack, string reason)
0546: 	{
0547: 		if (pack == null || _rebuilding)
0548: 		{
0549: 			return;
0550: 		}
0551: 		_rebuilding = true;
~~~

### ItemStorage/BoxWindow.cs:652-710
~~~csharp
0652: 				}
0653: 			}
0654: 			catch
0655: 			{
0656: 			}
0657: 		}
0658: 		text = Core.Clean(text);
0659: 		if (text.Length > maxChars)
0660: 		{
0661: 			text = text.Substring(0, maxChars);
0662: 		}
0663: 		return text;
0664: 	}
0665: 
0666: 	internal static void UpdateTooltip(GameItem pack)
0667: 	{
0668: 		if (pack == null)
0669: 		{
0670: 			return;
0671: 		}
0672: 		try
0673: 		{
0674: 			try
0675: 			{
0676: 				pack.shortDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
0677: 			}
0678: 			catch (Exception ex)
0679: 			{
0680: 				Core.Log?.Warning("收纳箱 shortDescription 写入：" + ex.Message);
0681: 			}
0682: 			try
0683: 			{
0684: 				string shortDescription = pack.shortDescription;
0685: 				if (!string.Equals(shortDescription, "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。", StringComparison.Ordinal))
0686: 				{
0687: 					Core.Debug("收纳箱 shortDescription 读回不一致：'" + Core.Clean(shortDescription) + "'");
0688: 				}
0689: 			}
0690: 			catch (Exception ex2)
0691: 			{
0692: 				Core.Log?.Warning("收纳箱 shortDescription 读回失败（getter 缺失？）：" + ex2.Message);
0693: 			}
0694: 			try
0695: 			{
0696: 				pack.longDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
0697: 			}
0698: 			catch
0699: 			{
0700: 			}
0701: 			pack.flavorText = string.Empty;
0702: 		}
0703: 		catch (Exception ex3)
0704: 		{
0705: 			Core.Log?.Warning("收纳箱 UpdateTooltip：" + ex3.Message);
0706: 		}
0707: 	}
0708: 
0709: 	private static Action MakeAction(Action action)
0710: 	{
~~~

### ItemStorage/BoxWindow.cs:983-1011
~~~csharp
0983: 				val3.anchorMin = new Vector2(0f, 1f);
0984: 				val3.anchorMax = new Vector2(0f, 1f);
0985: 				val3.pivot = new Vector2(0f, 1f);
0986: 				val3.anchoredPosition = Vector2.zero;
0987: 				val3.sizeDelta = new Vector2(48f, 8f);
0988: 			}
0989: 			Image obj = val2.AddComponent<Image>();
0990: 			((Graphic)obj).color = new Color(1f, 1f, 1f, 0f);
0991: 			((Graphic)obj).raycastTarget = true;
0992: 			SlotClickCatcher slotClickCatcher = val2.AddComponent<SlotClickCatcher>();
0993: 			slotClickCatcher.Pack = pack;
0994: 			slotClickCatcher.Slot = slot;
0995: 			slotClickCatcher.Refresh = delegate
0996: 			{
0997: 				RefreshButtons(pack);
0998: 				Core.Log?.Msg("收纳箱 锁定 UI 已刷新：槽 " + slot + " locked=" + SlotLock.IsLocked(pack, slot));
0999: 			};
1000: 			val2.SetActive(true);
1001: 			Core.LogSink log = Core.Log;
1002: 			if (log != null)
1003: 			{
1004: 				string[] obj2 = new string[11]
1005: 				{
1006: 					"收纳箱[",
1007: 					context,
1008: 					"] 槽 ",
1009: 					slot.ToString(),
1010: 					" 点击层 v3 已构建（rect=",
1011: 					null,
~~~

### ItemStorage/BoxWindow.cs:1770-1798
~~~csharp
1770: 					catch
1771: 					{
1772: 						flag = false;
1773: 					}
1774: 					if (!flag)
1775: 					{
1776: 						IconRefreshItems.RemoveAt(num);
1777: 						IconRefreshAt.RemoveAt(num);
1778: 						IconRetryTried.RemoveAt(num);
1779: 					}
1780: 					else
1781: 					{
1782: 						try
1783: 						{
1784: 							RefreshButtons(val);
1785: 						}
1786: 						catch
1787: 						{
1788: 						}
1789: 						if (!HasUnresolvedIcon(val))
1790: 						{
1791: 							IconRefreshItems.RemoveAt(num);
1792: 							IconRefreshAt.RemoveAt(num);
1793: 							IconRetryTried.RemoveAt(num);
1794: 						}
1795: 						else if (++IconRetryTried[num] >= 40)
1796: 						{
1797: 							Core.Debug("收纳箱 图标重试放弃：'" + (Core.Clean(val.identifier) ?? "?") + "' 仍有未解析贴图（MOD 卸载或贴图键失效）");
1798: 							IconRefreshItems.RemoveAt(num);
~~~

## 3829730904

### BrewingExpansion/Formal/Figurines.cs:1-26
~~~csharp
0001: using System;
0002: using System.Collections.Generic;
0003: 
0004: namespace BrewingExpansion.Formal;
0005: 
0006: internal static class Figurines
0007: {
0008: 	internal static readonly FigurineDefinition[] All = new FigurineDefinition[4]
0009: 	{
0010: 		new FigurineDefinition("be_ichigo_figurine", "黑崎一护", "Ichigo Kurosaki", "橘发少年握紧斩魄刀，像下一秒就要跃出展示柜。守护重要之人的决心，被凝在这小小的身影里。拥有它，店铺吸引力 +100；同款不叠加。", "The orange-haired swordsman grips his blade, ready to leap from the display case. His resolve to protect those dear to him lives in this tiny figure. Adds 100 shop attractiveness; duplicates do not stack."),
0011: 		new FigurineDefinition("be_rukia_figurine", "朽木露琪亚", "Rukia Kuchiki", "黑色死霸装与白色围巾在风里扬起。她把勇气藏在平静的目光中，也把一段相遇留在了这座展示柜里。拥有它，店铺吸引力 +100；同款不叠加。", "Her black robes and white scarf catch an unseen breeze. A quiet gaze holds both courage and the memory of a fateful meeting. Adds 100 shop attractiveness; duplicates do not stack."),
0012: 		new FigurineDefinition("be_uryu_figurine", "石田雨龙", "Uryu Ishida", "眼镜后的目光沉着，灵弓的弦仿佛仍在发亮。他不必高声宣告自己的信念，只需把箭稳稳射向前方。拥有它，店铺吸引力 +100；同款不叠加。", "Behind his glasses, his gaze stays steady and the bowstring seems to glow. He lets a carefully aimed arrow speak for his convictions. Adds 100 shop attractiveness; duplicates do not stack."),
0013: 		new FigurineDefinition("be_iridium_sea_cucumber_figurine", "噢我的天啊是铱星大海参", "Oh My God, It's an Iridium Sea Cucumber", "从铱星的海底捞起一只大海参，连星露谷的渔夫都要惊呼：噢我的天啊！把它摆在店里，路过的人总会停下来看第二眼。拥有它，店铺吸引力 +100；同款不叠加。", "An iridium sea cucumber, hauled up from the depths near Stardew Valley. Even a seasoned fisher would cry, 'Oh my god!' Passersby always stop for a second look. Adds 100 shop attractiveness; duplicates do not stack.", 2)
0014: 	};
0015: 
0016: 	internal static GameItem Create(FigurineDefinition definition)
0017: 	{
0018: 		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
0019: 		GameItem obj = ItemDirectory.CreateEmptyItem(definition.Id);
0020: 		obj.SetName(ModHelper.GetLocalized("brewing_expansion", definition.Id + "_name", Array.Empty<object>()));
0021: 		obj.SetSpriteAndShapeFromMod("brewing_expansion", definition.Id);
0022: 		obj.SetShape(new GridShapeBuilder(2, definition.Height).SetDataFill(2, definition.Height, (byte)1).Build());
0023: 		obj.SetGameItemType("LUXURY_ITEM");
0024: 		obj.SetValue(0L);
0025: 		obj.shortDescription = ModHelper.GetLocalized("brewing_expansion", definition.Id + "_description", Array.Empty<object>());
0026: 		obj.EnableTag("BE_DECORATION", false);
~~~

### BrewingExpansion/Formal/ModEntry.cs:52-85
~~~csharp
0052: 		ModHook.OnWakingUpLate += BrewingBarrel.Wake;
0053: 		ModHook.OnGoingSleepLate += BrewingBarrel.Wake;
0054: 		ModHook.OnGoingSleepLate += SeedIncubator.CompleteNight;
0055: 		ModHook.OnGoingSleepEarly += LiunianCabinet.BeginNight;
0056: 		ModHook.OnGameLoadedLate += LiunianCabinet.Reset;
0057: 		ModHook.OnWakingUpLate += LiunianCabinet.FinishNight;
0058: 		ModHook.OnGoingSleepLate += LiunianCabinet.FinishNight;
0059: 		ModHook.OnGameLoadedLate += MachineConnections.Request;
0060: 		ModHook.OnGameLoadedEarly += FormalVisitors.RegisterFactories;
0061: 		ModHook.OnGameLoadedLate += FormalEvents.Loaded;
0062: 		ModHook.OnGenerateCustomerVeryLate += FormalEvents.Generated;
0063: 		ModHook.OnNewGameStarted += FormalEvents.NewGame;
0064: 		ModHook.OnGoingSleepLate += FormalEvents.AfterNight;
0065: 		FormalVisitors.RegisterFactories();
0066: 		FormalShop.Register();
0067: 		FormalTraits.Register();
0068: 		ModItemDirectory[] array = Resources.FindObjectsOfTypeAll<ModItemDirectory>();
0069: 		foreach (ModItemDirectory directory in array)
0070: 		{
0071: 			Register(directory);
0072: 		}
0073: 		LiquidPatches.Enable(this, log);
0074: 		log.Log((object)"Formal catalogue enabled: machines, water containers, consumables, native music cassettes, Outer Ring visitors and independent night shop.");
0075: 	}
0076: 
0077: 	public void OnDisable()
0078: 	{
0079: 		if (!Enabled)
0080: 		{
0081: 			return;
0082: 		}
0083: 		Enabled = false;
0084: 		LiquidPatches.Disable();
0085: 		ModHook.OnCreateTooltipLate -= WineChemistry.AppendTooltip;
~~~

### BrewingExpansion/Formal/ModEntry.cs:116-144
~~~csharp
0116: 				((Directory<GameItem>)(object)binding.Directory).Remove(binding.Id);
0117: 			}
0118: 		}
0119: 		bindings.Clear();
0120: 		log.Log((object)"Formal foundation disabled; subscriptions and debug actions removed.");
0121: 	}
0122: 
0123: 	private void RegisterDebugFunctions()
0124: 	{
0125: 		if (debugRegistered || !Enabled)
0126: 		{
0127: 			return;
0128: 		}
0129: 		debugRegistered = true;
0130: 		FigurineDefinition[] all = Figurines.All;
0131: 		foreach (FigurineDefinition figurineDefinition in all)
0132: 		{
0133: 			FigurineDefinition item = figurineDefinition;
0134: 			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", item.Id + "_name", Array.Empty<object>()), (Action)delegate
0135: 			{
0136: 				ModHelper.MoveItemToInventory(ModHelper.SpawnItem(item.Id));
0137: 			}, false);
0138: 		}
0139: 		WaterContainerDefinition[] all2 = WaterContainers.All;
0140: 		foreach (WaterContainerDefinition waterContainerDefinition in all2)
0141: 		{
0142: 			WaterContainerDefinition item2 = waterContainerDefinition;
0143: 			ModHelper.AddDebugFunction("brewing_expansion", "Spawn", ModHelper.GetLocalized("brewing_expansion", item2.Id + "_name", Array.Empty<object>()), (Action)delegate
0144: 			{
~~~

### BrewingExpansion/Formal/ModEntry.cs:233-270
~~~csharp
0233: 	{
0234: 		Func<GameItem> create = factory;
0235: 		factory = delegate
0236: 		{
0237: 			GameItem obj = create();
0238: 			MachineGuides.Attach(obj);
0239: 			return obj;
0240: 		};
0241: 		if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(id) && ((Directory<GameItem>)(object)directory).Add(id, factory))
0242: 		{
0243: 			bindings.Add((directory, id, factory));
0244: 		}
0245: 	}
0246: 
0247: 	private void Register(ModItemDirectory directory)
0248: 	{
0249: 		FormalTraitIcons.Register();
0250: 		FigurineDefinition[] all = Figurines.All;
0251: 		foreach (FigurineDefinition figurineDefinition in all)
0252: 		{
0253: 			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(figurineDefinition.Id))
0254: 			{
0255: 				FigurineDefinition item = figurineDefinition;
0256: 				Func<GameItem> func = () => Figurines.Create(item);
0257: 				if (((Directory<GameItem>)(object)directory).Add(item.Id, func))
0258: 				{
0259: 					bindings.Add((directory, item.Id, func));
0260: 				}
0261: 				else
0262: 				{
0263: 					log.Warning((object)("Item registration refused: " + item.Id));
0264: 				}
0265: 			}
0266: 		}
0267: 		WaterContainerDefinition[] all2 = WaterContainers.All;
0268: 		foreach (WaterContainerDefinition waterContainerDefinition in all2)
0269: 		{
0270: 			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(waterContainerDefinition.Id))
~~~
