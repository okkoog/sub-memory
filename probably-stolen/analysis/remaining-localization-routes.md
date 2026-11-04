# Remaining localization route audit

## card

### GameCardPK/SkillText.cs

~~~csharp
00505: 			return BattleLogGenerator.Tag(text, hex);
00506: 		}
00507: 		return text;
00508: 	}
00509: 
00510: 	internal static bool IsEventScaledNode(ActionData action)
00511: 	{
00512: 		if (!IsEventScaled(action.Value) && !IsEventScaled(action.BuffStackValue))
00513: 		{
00514: 			return IsEventScaled(action.RepeatCount);
00515: 		}
00516: 		return true;
00517: 	}
00518: 
00519: 	private static bool IsEventScaled(ValueExpr? value)
00520: 	{
00521: 		if (value != null && !string.IsNullOrEmpty(value.ScaleAttr))
00522: 		{
00523: 			return value.ScaleAttr.StartsWith("Event.", StringComparison.Ordinal);
00524: 		}
00525: 		return false;
00526: 	}
00527: 
00528: 	internal static string AttrLabel(string? key)
00529: 	{
00530: 		switch (key)
00531: 		{
00532: 		case "CurHP":
00533: 		case "HP":
00534: 			return "生命";
00535: 		case "MaxHP":
00536: 			return "最大生命";
00537: 		case "HPPercent":
00538: 			return "生命百分比";
00539: 		case "ATK":
00540: 			return "攻击力";
00541: 		case "SPD":
00542: 			return "速度";
00543: 		case "Armor":
00544: 			return "护甲";
00545: 		case "HIT":
00546: 			return "命中率";
00547: 		case "DODGE":
00548: 			return "闪避";
00549: 		case "EXTRA_HIT":
00550: 			return "额外普攻几率";
00551: 		case "DEBUFF_RES":
00552: 			return "负面抗性";
00553: 		case "DMG_AMP":
00554: 			return "伤害增幅";
00555: 		case "BASIC_DMG_TAKEN_AMP":
00556: 			return "普攻易伤";
00557: 		case "IMMOBILIZED":
00558: 			return "禁锢";
00559: 		case "CoreForce":
00560: 			return "核心技能加成";
00561: 		case "OtherForce":
00562: 			return "技能槽加成";
00563: 		default:
00564: 			if (key.StartsWith("OtherForce", StringComparison.Ordinal))
00565: 			{
00566: 				return "技能槽 " + key.Substring("OtherForce".Length) + " 加成";
00567: 			}
00568: 			break;
00569: 		case null:
00570: 			break;
00571: 		}
00572: 		return key ?? "?";
00573: 	}
00574: 
00575: 	private static BattleCardInstance? ScaleEntity(BattleCardInstance? owner, ValueScaleFrom from)
00576: 	{
00577: 		if (owner == null)
00578: 		{
00579: 			return null;
00580: 		}
00581: 		if (from != ValueScaleFrom.Target)
00582: 		{
00583: 			return owner;
00584: 		}
00585: 		BattleContext ctx = owner.Ctx;
00586: 		if (ctx == null)
00587: 		{
00588: 			return null;
00589: 		}
00590: 		if (owner.Side != BattleSide.Player)
00591: 		{
00592: 			return ctx.Player;
00593: 		}
00594: 		return ctx.Enemy;
00595: 	}
00596: 
00597: 	private static int FinalAttr(BattleCardInstance card, string key, int fallback)
00598: 	{
00599: 		BattleContext ctx = card.Ctx;
00600: 		if (ctx == null)
00601: 		{
00602: 			return fallback;
00603: 		}
00604: 		try
00605: 		{
00606: 			return ctx.Attrs.GetFinalAttr(card, key);
00607: 		}
00608: 		catch (Exception ex)
00609: 		{
00610: 			Core.Debug("[技能文案] 读属性 " + key + " 失败（按 " + fallback + " 算）：" + ex.Message);
00611: 			return fallback;
~~~

### GameCardPK/GameCardFactory.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: 
00004: namespace GameCardPK;
00005: 
00006: internal static class GameCardFactory
00007: {
00008: 	internal static List<Action<TagState>> Pinned = new List<Action<TagState>>();
00009: 
00010: 	internal static readonly List<string> TagIds = new List<string> { "HP", "ATK", "SPD" };
00011: 
00012: 	internal static readonly List<string> TagNames = new List<string> { "生命值", "攻击力", "速度" };
00013: 
00014: 	internal const string GameCardItemId = "game_card";
00015: 
00016: 	internal const string CardSkillItemId = "card_skill";
00017: 
00018: 	internal const string ChessBoardItemId = "chess_board";
00019: 
00020: 	internal const string CardFactoryCardItemId = "card_factory_card";
00021: 
00022: 	internal const string BattleSlotId = "battle_card_slot";
00023: 
00024: 	internal const string RepairGridSlotId = "repair_slot";
00025: 
00026: 	internal const string SkillSlotMaxKey = "SkillSlotMax";
00027: 
00028: 	private static readonly Random Rng = new Random();
00029: 
00030: 	internal static string GameCardItem => ItemConfigInjector.Prefixed("game_card");
00031: 
00032: 	internal static string CardSkillItem => ItemConfigInjector.Prefixed("card_skill");
00033: 
00034: 	internal static string ChessBoardItem => ItemConfigInjector.Prefixed("chess_board");
00035: 
00036: 	internal static bool IsCard(GameItem? item)
00037: 	{
00038: 		if (item == null)
00039: 		{
00040: 			return false;
00041: 		}
00042: 		try
00043: 		{
00044: 			return item.IsTag("game_card");
00045: 		}
00046: 		catch
00047: 		{
00048: 			return false;
00049: 		}
00050: 	}
00051: 
00052: 	internal static GameItem CreateShell()
00053: 	{
00054: 		ItemConfigInjector.ItemConfigData itemConfigData = ItemConfigInjector.FindConfig("game_card");
00055: 		GameItem val;
00056: 		if (itemConfigData != null)
00057: 		{
00058: 			try
00059: 			{
00060: 				val = ItemConfigInjector.CreateItem(itemConfigData);
00061: 			}
00062: 			catch (Exception ex)
00063: 			{
00064: 				Core.Log?.Error("[卡牌PK] 造卡壳失败（改用空物品）：" + ex.Message);
00065: 				val = ItemDirectory.CreateEmptyItem((string)null);
00066: 			}
00067: 		}
00068: 		else
00069: 		{
00070: 			Core.Log?.Error("[卡牌PK] items.json 里没有 game_card 模板，卡牌只能用空物品兜底。");
00071: 			val = ItemDirectory.CreateEmptyItem((string)null);
00072: 		}
00073: 		BuildCardWindow(val);
00074: 		ItemTargetRouter.AttachReceiver(val, ReceiverKind.Card);
00075: 		ChipSystem.TraceShell(val);
00076: 		return val;
00077: 	}
00078: 
00079: 	private static void BuildCardWindow(GameItem item)
00080: 	{
00081: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
~~~

~~~csharp
00135: 					}
00136: 					else
00137: 					{
00138: 						Core.Debug("[卡牌PK]「" + gameCardData.Name + "」没有配变异形态，异画请求降级为普通卡。");
00139: 					}
00140: 					versions.AltArt = false;
00141: 				}
00142: 				else
00143: 				{
00144: 					versions.Add("VariantArt");
00145: 				}
00146: 			}
00147: 		}
00148: 		bool altArt = versions.AltArt;
00149: 		GameItem val = CreateShell();
00150: 		CardIdentity.Set(val, gameCardData.Id);
00151: 		string name = ((altArt && variantEntry != null && variantEntry.Name.Length > 0) ? variantEntry.Name : gameCardData.Name);
00152: 		val.SetName(name);
00153: 		string text = ((altArt && variantEntry != null && variantEntry.Description.Length > 0) ? variantEntry.Description : gameCardData.Description);
00154: 		if (!string.IsNullOrEmpty(text))
00155: 		{
00156: 			val.shortDescription = text;
00157: 		}
00158: 		int num = GameCardData.RollValue(gameCardData.HP, gameCardData.AttributeVariance, Rng);
00159: 		int num2 = GameCardData.RollValue(gameCardData.ATK, gameCardData.AttributeVariance, Rng);
00160: 		int num3 = GameCardData.RollValue(gameCardData.SPD, gameCardData.AttributeVariance, Rng);
00161: 		ChangeTag(TagIds[0], TagNames[0], val, num);
00162: 		ChangeTag(TagIds[1], TagNames[1], val, num2);
00163: 		ChangeTag(TagIds[2], TagNames[2], val, num3);
00164: 		string text2 = SeriesDatabase.DisplayNames(gameCardData.Series);
00165: 		if (!string.IsNullOrEmpty(text2))
00166: 		{
00167: 			ChangeTagString("Series", val, text2);
00168: 		}
00169: 		int num4 = GameCardData.RollSlotCount(gameCardData.SkillSlot, gameCardData.SlotVariance, Rng);
00170: 		ChangeTag("SkillSlotMax", "技能槽上限", val, num4);
00171: 		GameSlotInventory val2 = ContentWindowRegistry.Find(val, "skill0")?.TryCast<GameSlotInventory>();
00172: 		CardSkillData skill = BattleDatabase.GetSkill(gameCardData.GetSkillAt(1));
00173: 		if (skill != null && val2 != null)
00174: 		{
00175: 			GameItem val3 = CreateSkill(skill);
00176: 			val3.EnableTag(TagDisplay.NewTagDatas["CoreSkill"].Id, true);
00177: 			InventoryHelper.TrueAdd(val2, val3);
00178: 		}
00179: 		else
00180: 		{
00181: 			Core.Log?.Error("[卡牌PK] 卡牌 " + gameCardData.Name + " 的核心技能没有配置（或核心槽丢失）：" + gameCardData.GetSkillAt(1));
00182: 		}
00183: 		StampVersionTags(val, versions);
00184: 		CardPricing.Apply(val, num, num2, num3, num4, versions);
00185: 		if (versions.IsMisprint)
00186: 		{
00187: 			try
00188: 			{
00189: 				val.SetGameItemType("misprint_card");
00190: 			}
00191: 			catch (Exception ex)
00192: 			{
00193: 				Core.Debug("[卡牌PK] 设错版类型失败：" + ex.Message);
00194: 			}
00195: 		}
00196: 		Core.Debug("[卡牌PK] 生成卡牌 " + gameCardData.Name + "（" + ItemConfigInjector.BareId(gameCardData.Id) + "）：版本 " + versions.Describe() + "，技能槽上限 " + num4 + "（物理 " + GameCardData.MaxSkillSlots + " 槽），自带技能 " + gameCardData.Skills.Count + " 个，卡牌价值 " + CardPricing.ReadValue(val) + "。");
00197: 		if (altArt || isVariant)
00198: 		{
00199: 			MutationSystem.MarkAltArt(val);
00200: 		}
00201: 		return val;
00202: 	}
00203: 
00204: 	internal static GameItem CreateSkill(CardSkillData cardSkillData)
00205: 	{
00206: 		GameItem val = DirectoryMaster.Item(CardSkillItem, true);
00207: 		if (cardSkillData == null)
00208: 		{
00209: 			Core.Log?.Error("[卡牌PK] CreateSkill 传入的数据为空");
00210: 			return val;
00211: 		}
00212: 		ApplySkillData(val, cardSkillData);
00213: 		return val;
00214: 	}
00215: 
00216: 	internal static void ApplySkillData(GameItem item, CardSkillData cardSkillData)
00217: 	{
00218: 		if (item == null || cardSkillData == null)
00219: 		{
00220: 			return;
00221: 		}
00222: 		item.identifier = cardSkillData.Id;
00223: 		item.SetName(cardSkillData.Name);
00224: 		string text = SkillText.ForItem(cardSkillData);
00225: 		if (!string.IsNullOrEmpty(text))
00226: 		{
00227: 			item.shortDescription = text;
00228: 		}
00229: 		item.longDescription = text;
00230: 		if (cardSkillData.Value <= 0)
00231: 		{
00232: 			return;
00233: 		}
00234: 		try
00235: 		{
00236: 			item.SetValue((long)cardSkillData.Value);
00237: 		}
00238: 		catch (Exception ex)
00239: 		{
00240: 			Core.Debug("[卡牌PK] 技能条价值没写进去（" + cardSkillData.Id + "）：" + ex.Message);
00241: 		}
00242: 	}
00243: 
00244: 	internal static GameItem CreateChessBoard()
00245: 	{
00246: 		return DirectoryMaster.Item(ChessBoardItem, true);
00247: 	}
00248: 
00249: 	internal static void AttachChessBoardWindow(GameItem item)
00250: 	{
00251: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00252: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00253: 		//IL_0011: Expected O, but got Unknown
00254: 		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
00255: 		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
00256: 		//IL_0024: Expected O, but got Unknown
00257: 		GameSlotInventory val = new GameSlotInventory
00258: 		{
00259: 			identifier = "battle_card_slot"
00260: 		};
00261: 		GameGridInventory val2 = new GameGridInventory(4, 4)
00262: 		{
00263: 			identifier = "repair_slot"
00264: 		};
00265: 		item.EnableTag("CAN_USE_ON_NON_OWNED_TAG", true);
~~~

### GameCardPK/SlotAdmission.cs

~~~csharp
00160: 
00161: 	private static bool AcceptStickerBlueprint(GameItem item)
00162: 	{
00163: 		try
00164: 		{
00165: 			if (!item.IsTag("STICKER_BLUEPRINT_TAG"))
00166: 			{
00167: 				return false;
00168: 			}
00169: 		}
00170: 		catch
00171: 		{
00172: 			return false;
00173: 		}
00174: 		return ItemOwnership.IsPlayerOwned(item, "贴纸蓝图", log: false);
00175: 	}
00176: 
00177: 	private static bool AcceptStickerMaterial(GameItem item)
00178: 	{
00179: 		try
00180: 		{
00181: 			if (!WasteTags.IsMaterial(item))
00182: 			{
00183: 				return false;
00184: 			}
00185: 		}
00186: 		catch
00187: 		{
00188: 			return false;
00189: 		}
00190: 		return ItemOwnership.IsPlayerOwned(item, "废纸", log: false);
00191: 	}
00192: 
00193: 	internal static string Describe()
00194: 	{
00195: 		return "槽位准入（官方钩子 mayInventoryAddItemFunc）：技能槽只收技能条（核心槽不许放）/ battle_card_slot 只收自己的、装过芯片且耐久够的卡 / repair_slot 只收自己的游戏卡 / 贴纸打印机的蓝图槽只收贴纸蓝图、投入格只收废纸";
00196: 	}
00197: }
~~~

### GameCardPK/SpecialItemDatabase.cs

~~~csharp
00778: 					if (num5 < 0)
00779: 					{
00780: 						num5 = 0;
00781: 					}
00782: 					if (num5 > 99)
00783: 					{
00784: 						num5 = 99;
00785: 					}
00786: 					_boardCharge[ItemConfigInjector.PrefixedItem(text2)] = num5;
00787: 				}
00788: 				if (item2.Challenge != null)
00789: 				{
00790: 					ChallengeSpec challengeSpec = NormalizeChallengeSpec(text2, item2.Challenge);
00791: 					if (challengeSpec != null)
00792: 					{
00793: 						_challenges[text2] = challengeSpec;
00794: 						Core.Debug("[战书] 参数：" + text2 + "（难度 1~" + challengeSpec.NpcIds.Count + "，依次点名 " + string.Join(" / ", challengeSpec.NpcIds) + "）");
00795: 					}
00796: 				}
00797: 				if (item2.Press != null)
00798: 				{
00799: 					PressSpec pressSpec = NormalizePressSpec(text2, item2.Press);
00800: 					_press[text2] = pressSpec;
00801: 					Core.Debug("[垃圾] 压榨机参数：" + text2 + "（每晚 " + pressSpec.PowerCost + " 电 + " + pressSpec.JunkCost + " 件垃圾 → 每张卡 +" + pressSpec.Repair + " 耐久；槽 " + pressSpec.BatterySlot + " / " + pressSpec.InputSlot + "(" + pressSpec.InputSize + "×" + pressSpec.InputSize + ") / " + pressSpec.OutputSlot + "(" + pressSpec.OutputSize + "×" + pressSpec.OutputSize + ")）");
00802: 				}
00803: 				if (item2.Perch != null)
00804: 				{
00805: 					PerchSpec perchSpec = NormalizePerchSpec(text2, item2.Perch);
00806: 					_perch[text2] = perchSpec;
00807: 					Core.Debug("[乌鸦] 站架参数：" + text2 + "（投入 " + perchSpec.InputSlot + " " + perchSpec.InputSize + "×" + perchSpec.InputSize + " 只收 " + perchSpec.LimitItem + "；产出 " + perchSpec.OutputSlot + " " + perchSpec.OutputSize + "×" + perchSpec.OutputSize + "；表 " + perchSpec.LootTable + "；热度 " + perchSpec.Heat + "）");
00808: 				}
00809: 				if (item2.StickerPrint != null)
00810: 				{
00811: 					StickerPrintSpec stickerPrintSpec = NormalizeStickerPrintSpec(text2, item2.StickerPrint);
00812: 					_stickerPrinters[text2] = stickerPrintSpec;
00813: 					Core.Debug("[贴纸打印机] 参数：" + text2 + "（每晚 " + stickerPrintSpec.PowerCost + " 电，速度 " + stickerPrintSpec.Speed + "；电池 " + stickerPrintSpec.BatterySlot + " / 模块 " + stickerPrintSpec.ModuleSlot + "(7 宽位图) / 蓝图 " + stickerPrintSpec.BlueprintSlot + " / 废纸 " + stickerPrintSpec.InputSlot + "(" + stickerPrintSpec.InputSize + "×" + stickerPrintSpec.InputSize + ") / 产出 " + stickerPrintSpec.OutputSlot + "(" + stickerPrintSpec.OutputSize + "×" + stickerPrintSpec.OutputSize + ")");
00814: 				}
00815: 				string text3 = string.Empty;
00816: 				if (item2.Container != null)
00817: 				{
00818: 					ContainerWindowSpec containerWindowSpec = BuildContainerSpec(text2, item2.Container);
00819: 					_containers[text2] = containerWindowSpec;
00820: 					_containerSlotIds.Add(containerWindowSpec.SlotId);
00821: 					text3 = "；内容窗 " + containerWindowSpec.Width + "×" + containerWindowSpec.Height + "（" + containerWindowSpec.SlotId + "）" + ((containerWindowSpec.AcceptIds.Count > 0) ? ("，只收 " + string.Join("/", containerWindowSpec.AcceptIds)) : string.Empty);
00822: 				}
00823: 				Core.Debug("[卡牌PK] 特殊物品已注册：" + text2 + "（内联属性，" + itemConfigData.Width + "×" + itemConfigData.Height + "，价值 " + itemConfigData.Value + "，每晚修复充能 " + ChargeFor(text2) + text3 + "）");
00824: 			}
00825: 			catch (Exception ex)
00826: 			{
00827: 				num2++;
00828: 				Core.Log?.Error("[卡牌PK] 解析特殊物品失败（" + Core.Clean(item2.Id) + "）：" + ex.Message);
00829: 			}
00830: 		}
00831: 		_registered = num + num3;
00832: 		Core.Log?.Msg("[卡牌PK] 已加载特殊物品 " + num + " 条" + ((num3 > 0) ? ("（另有 " + num3 + " 条已在册）") : "") + ((num2 > 0) ? ("，失败 " + num2 + " 条（见上方警告）") : "") + "。");
00833: 	}
00834: 
00835: 	private static ContainerWindowSpec BuildContainerSpec(string itemId, ContainerDto dto)
00836: 	{
00837: 		ContainerWindowSpec containerWindowSpec = new ContainerWindowSpec
00838: 		{
00839: 			Width = dto.Width,
00840: 			Height = dto.Height,
00841: 			SlotId = Core.Clean(dto.SlotId).ToLowerInvariant(),
00842: 			Title = Core.Clean(dto.Title)
00843: 		};
00844: 		if (containerWindowSpec.Width < 1)
00845: 		{
00846: 			containerWindowSpec.Width = 1;
00847: 		}
00848: 		if (containerWindowSpec.Width > 12)
00849: 		{
00850: 			containerWindowSpec.Width = 12;
00851: 		}
00852: 		if (containerWindowSpec.Height < 1)
00853: 		{
00854: 			containerWindowSpec.Height = 1;
00855: 		}
00856: 		if (containerWindowSpec.Height > 12)
00857: 		{
00858: 			containerWindowSpec.Height = 12;
00859: 		}
00860: 		if (containerWindowSpec.SlotId.Length == 0)
00861: 		{
00862: 			containerWindowSpec.SlotId = itemId + "_grid";
00863: 		}
00864: 		if (containerWindowSpec.Title.Length == 0)
00865: 		{
00866: 			containerWindowSpec.Title = itemId;
00867: 		}
00868: 		if (dto.AcceptIds != null)
00869: 		{
00870: 			foreach (string acceptId in dto.AcceptIds)
00871: 			{
00872: 				string text = ItemConfigInjector.BareId(Core.Clean(acceptId)).ToLowerInvariant();
00873: 				if (text.Length != 0)
00874: 				{
00875: 					containerWindowSpec.AcceptIds.Add(text);
00876: 				}
00877: 			}
00878: 		}
00879: 		return containerWindowSpec;
00880: 	}
00881: 
00882: 	private static StickerPrintSpec NormalizeStickerPrintSpec(string itemId, StickerPrintSpec dto)
~~~

### GameCardPK/TagDisplay.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: 
00004: namespace GameCardPK;
00005: 
00006: internal static class TagDisplay
00007: {
00008: 	private const string P = "game_card_pk.";
00009: 
00010: 	internal static Dictionary<string, CustomTagData> NewTagDatas = BuildTagTable();
00011: 
00012: 	private static readonly HashSet<string> _registeredTags = new HashSet<string>(StringComparer.Ordinal);
00013: 
00014: 	private static bool _initialized;
00015: 
00016: 	private static Dictionary<string, CustomTagData> BuildTagTable()
00017: 	{
00018: 		Dictionary<string, CustomTagData> dictionary = new Dictionary<string, CustomTagData>();
00019: 		Add(dictionary, "CanBattle", DisplayType.None, "可对战", (ColorPalette)12472652);
00020: 		Add(dictionary, "OutOfPrint", DisplayType.None, "绝版", (ColorPalette)6511905);
00021: 		Add(dictionary, "FirstEdition", DisplayType.None, "初版", (ColorPalette)4365501);
00022: 		Add(dictionary, "Memorial", DisplayType.None, "纪念版", (ColorPalette)11567781);
00023: 		Add(dictionary, "Rare", DisplayType.None, "稀有版", (ColorPalette)9672266, string.Empty, (Func<GameItem, string>?)RareSeriesLabel.Format);
00024: 		Add(dictionary, "Damaged", DisplayType.None, "破损版", (ColorPalette)9454675);
00025: 		Add(dictionary, "Tournament", DisplayType.None, "赛事版", (ColorPalette)4367018);
00026: 		Add(dictionary, "VariantArt", DisplayType.None, "异画", (ColorPalette)9672266);
00027: 		Add(dictionary, "MutantForm", DisplayType.None, "变异", (ColorPalette)8407419);
00028: 		Add(dictionary, "HP", DisplayType.Int, "生命值：", (ColorPalette)(-1));
00029: 		Add(dictionary, "ATK", DisplayType.Int, "攻击力：", (ColorPalette)(-1));
00030: 		Add(dictionary, "SPD", DisplayType.Int, "速度：", (ColorPalette)(-1));
00031: 		Add(dictionary, "CoreForce", DisplayType.Int, "核心技能加成：", (ColorPalette)(-1));
00032: 		for (int i = 1; i <= 8; i++)
00033: 		{
00034: 			AddOtherForce(dictionary, i);
00035: 		}
00036: 		Add(dictionary, "CoreSkill", DisplayType.None, "核心技能", (ColorPalette)9672266);
00037: 		Add(dictionary, "Series", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
00038: 		Add(dictionary, "Sticker", DisplayType.Int, "已添加贴纸：", (ColorPalette)9672266);
00039: 		Add(dictionary, "Insurance", DisplayType.None, "保险", (ColorPalette)9672266);
00040: 		Add(dictionary, "HpScale", DisplayType.Int, "生命缩放（%）：", (ColorPalette)(-1));
00041: 		Add(dictionary, "AtkScale", DisplayType.Int, "攻击缩放（%）：", (ColorPalette)(-1));
00042: 		Add(dictionary, "SpdScale", DisplayType.Int, "速度缩放（%）：", (ColorPalette)(-1));
00043: 		Add(dictionary, "Energy", DisplayType.Int, "能量：", (ColorPalette)9672266);
00044: 		Add(dictionary, "SkillSlotMax", DisplayType.Int, "技能槽上限：", (ColorPalette)(-1));
00045: 		Add(dictionary, "Difficulty", DisplayType.Int, "难度：", (ColorPalette)9672266);
00046: 		Add(dictionary, "Waste", DisplayType.Int, "废纸数：", (ColorPalette)9672266);
00047: 		Add(dictionary, "Junk", DisplayType.Int, "垃圾数：", (ColorPalette)9672266);
00048: 		Add(dictionary, "ChipDurability", DisplayType.CurrentMax, "芯片耐久：", (ColorPalette)9672266, "game_card_pk.ChipDurabilityMax");
00049: 		Add(dictionary, "ChipDurabilityMax", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
00050: 		Add(dictionary, "CardId", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
00051: 		Add(dictionary, "WeaponDurability", DisplayType.Int, "武器耐久：", (ColorPalette)9672266);
00052: 		Add(dictionary, "AdvWeaponDurability", DisplayType.Int, "高级武器耐久：", (ColorPalette)9672266);
00053: 		Add(dictionary, "MoodValue", DisplayType.Int, "心情值：", (ColorPalette)9672266);
00054: 		Add(dictionary, "RewardTier", DisplayType.Hidden, string.Empty, (ColorPalette)(-1));
00055: 		Add(dictionary, "AuxEffect", DisplayType.String, "辅助效果：", (ColorPalette)9672266, string.Empty, DragTagLabel.FormatAux);
00056: 		return dictionary;
00057: 	}
00058: 
00059: 	private static void Add(Dictionary<string, CustomTagData> table, string key, DisplayType type, string displayName, ColorPalette color = (ColorPalette)(-1), string secondId = "", Func<string, string>? formatValue = null)
00060: 	{
00061: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
00062: 		table[key] = new CustomTagData("game_card_pk." + key, type, displayName, color, secondId, formatValue);
00063: 	}
00064: 
00065: 	private static void Add(Dictionary<string, CustomTagData> table, string key, DisplayType type, string displayName, ColorPalette color, string secondId, Func<GameItem, string>? composeValue)
00066: 	{
00067: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
00068: 		table[key] = new CustomTagData("game_card_pk." + key, type, displayName, color, secondId)
00069: 		{
00070: 			ComposeValue = composeValue
00071: 		};
00072: 	}
00073: 
00074: 	private static void AddOtherForce(Dictionary<string, CustomTagData> table, int index)
00075: 	{
00076: 		Add(table, "OtherForce" + index, DisplayType.Int, "技能槽" + index + "加成：", (ColorPalette)(-1));
00077: 	}
00078: 
00079: 	internal static void EnsureOtherForceTags(int count)
00080: 	{
00081: 		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
00082: 		if (count < 0)
00083: 		{
00084: 			count = 0;
00085: 		}
00086: 		if (count > 32)
00087: 		{
00088: 			count = 32;
00089: 		}
00090: 		for (int i = 1; i <= count; i++)
00091: 		{
00092: 			string key = "OtherForce" + i;
00093: 			if (!NewTagDatas.ContainsKey(key))
00094: 			{
00095: 				AddOtherForce(NewTagDatas, i);
00096: 				if (_initialized)
00097: 				{
00098: 					CustomTagData customTagData = NewTagDatas[key];
00099: 					TagRegister(customTagData.Id, customTagData.DisplayName, customTagData.DisplayType, customTagData.Color, customTagData.SecondId, customTagData.FormatValue, customTagData.ComposeValue);
00100: 				}
00101: 			}
00102: 		}
00103: 	}
00104: 
00105: 	internal static void InitTagDisplay()
00106: 	{
00107: 		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
00108: 		EnsureOtherForceTags(GameCardData.MaxSkillSlots);
00109: 		if (_initialized)
00110: 		{
00111: 			return;
00112: 		}
00113: 		_initialized = true;
~~~

### GameCardPK/BattleLogGenerator.cs

~~~csharp
00123: 		{
00124: 			int num = ExtraInt(e, "armor");
00125: 			int num2 = ExtraInt(e, "hp");
00126: 			bool flag = e.Action != null && e.Action.DamageMode == DamageMode.ArmorOnly;
00127: 			string text = NameTag(e.Source) + " 对 " + NameTag(e.Target) + " 造成 " + Tag(e.Amount.ToString(), "#D95C4F") + " 点" + (flag ? "破甲" : "伤害");
00128: 			if (flag)
00129: 			{
00130: 				int num3 = ExtraInt(e, "wasted");
00131: 				if (num <= 0)
00132: 				{
00133: 					return text + "（" + Tag("没有护甲，未生效", "#A39F96") + "）";
00134: 				}
00135: 				text = text + "（" + Tag("护甲 -" + num, "#A39F96");
00136: 				if (num3 > 0)
00137: 				{
00138: 					text = text + "，" + Tag("溢出 " + num3 + " 点不伤血", "#A39F96");
00139: 				}
00140: 				return text + "）";
00141: 			}
00142: 			if (num > 0)
00143: 			{
00144: 				text = text + "（" + Tag("护甲 -" + num, "#A39F96");
00145: 				if (num2 > 0)
00146: 				{
00147: 					text = text + "，生命 -" + Tag(num2.ToString(), "#D95C4F");
00148: 				}
00149: 				text += "）";
00150: 			}
00151: 			return text;
00152: 		});
00153: 		g.RegisterAction(SkillActionType.GainArmor, (BattleLogEntry e) => NameTag(e.Target) + " 获得 " + Tag(e.Amount.ToString(), "#A39F96") + " 点护甲");
00154: 		g.RegisterAction(SkillActionType.Heal, (BattleLogEntry e) => NameTag(e.Target) + " 回复 " + Tag(e.Amount.ToString(), "#6FC678") + " 点生命");
00155: 		g.RegisterAction(SkillActionType.ModifyAttr, (BattleLogEntry e) => NameTag(e.Target) + " 的 " + AttrLabel(e.Label) + " " + Tag(Show(e.Amount), "#6FC678"));
00156: 		g.RegisterAction(SkillActionType.DeclareVictory, (BattleLogEntry e) => NameTag(e.Source) + " 在倒下之后击倒了对手，本场仍判其获胜");
00157: 		g.RegisterAction(SkillActionType.PostBattleAttr, (BattleLogEntry e) => NameTag(e.Target) + " 的 " + AttrLabel(e.Label) + " 将在战斗结束后永久 " + Tag(Show(e.Amount), "#A39F96"));
00158: 		g.RegisterAction(SkillActionType.DrainMaxHp, (BattleLogEntry e) => NameTag(e.Source) + " 从 " + NameTag(e.Target) + " 身上偷取了 " + Tag(e.Amount + " 点最大生命值", "#A39F96") + "，自己的最大生命值 " + Tag("+" + e.Amount, "#6FC678"));
00159: 		g.RegisterAction(SkillActionType.TransferBuff, (BattleLogEntry e) => NameTag(e.Source) + " 把 " + NameTag(e.Target) + " 刚获得的 " + (string.IsNullOrEmpty(e.Label) ? "增益" : e.Label) + " ×" + Tag(e.Amount.ToString(), "#6FC678") + " 抢了过来");
00160: 		g.RegisterAction(SkillActionType.AddBuffFromTag, (BattleLogEntry e) => NameTag(e.Target) + " 把 " + AttrLabel(e.Label) + " " + Tag(e.Amount.ToString(), "#6FC678") + " 全部转成了战斗中的层数");
00161: 		g.RegisterAction(SkillActionType.ConsumeBuffStack, (BattleLogEntry e) => NameTag(e.Target) + " 的 " + (string.IsNullOrEmpty(e.Label) ? "增益" : e.Label) + " 消耗了 " + Tag(e.Amount + " 层", "#A39F96"));
00162: 		g.RegisterAction(SkillActionType.DispelDebuff, (BattleLogEntry e) => NameTag(e.Target) + " 清除了 " + Tag(e.Amount + " 层", "#6FC678") + "负面状态");
00163: 		g.RegisterAction(SkillActionType.SetTagFromBuff, (BattleLogEntry e) => NameTag(e.Target) + " 的 " + AttrLabel(e.Label) + " 将在战斗结束后变为 " + Tag(e.Amount.ToString(), "#6FC678"));
00164: 		g.RegisterAction(SkillActionType.SyncBuffStackDiff, (BattleLogEntry e) => NameTag(e.Target) + " 的 " + (string.IsNullOrEmpty(e.Label) ? "增幅" : e.Label) + " 变为 " + Tag(e.Amount + " 层", "#6FC678"));
00165: 		g.Register("buff_add", (BattleLogEntry e) => (!Hidden(e)) ? (NameTag(e.Target) + " 获得 Buff：" + e.Name + " ×" + e.Stacks) : (NameTag(e.Target) + " 的状态变为「" + e.Name + "」"));
00166: 		g.Register("buff_stack", (BattleLogEntry e) => (!Hidden(e)) ? (NameTag(e.Target) + " 的 " + e.Name + " 层数变为 " + e.Stacks) : (NameTag(e.Target) + " 的「" + e.Name + "」状态层数变为 " + e.Stacks));
00167: 		g.Register("buff_remain", (BattleLogEntry e) => NameTag(e.Target) + " 的 " + e.Name + " 剩余 " + e.Stacks + " 层");
00168: 		g.Register("buff_expire", (BattleLogEntry e) => NameTag(e.Target) + " 的 " + e.Name + " 已到期");
00169: 		g.Register("buff_remove", (BattleLogEntry e) => (!Hidden(e)) ? (NameTag(e.Target) + " 失去了 " + e.Name) : (NameTag(e.Target) + " 的「" + e.Name + "」状态已结束"));
00170: 		g.Register("buff_undispellable", (BattleLogEntry e) => e.Name + " 不可被驱散。");
00171: 		g.Register("buff_resist", delegate(BattleLogEntry e)
00172: 		{
00173: 			int amount = e.Amount;
00174: 			int num = ((e.GetExtra("resist_asked") is int num2) ? num2 : (amount + e.Stacks));
00175: 			return (e.Source == null) ? (NameTag(e.Target) + " 受到 " + num + " 层" + e.Name + "（抵抗 " + amount + " 层）") : (NameTag(e.Source) + " 对 " + NameTag(e.Target) + " 施加了 " + num + " 层" + e.Name + "（抵抗 " + amount + " 层）");
00176: 		});
00177: 		static bool Hidden(BattleLogEntry e)
00178: 		{
00179: 			object extra = e.GetExtra("hidden");
00180: 			bool flag = default(bool);
00181: 			int num;
00182: 			if (extra is bool)
00183: 			{
00184: 				flag = (bool)extra;
00185: 				num = 1;
00186: 			}
00187: 			else
00188: 			{
00189: 				num = 0;
00190: 			}
00191: 			return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
00192: 		}
00193: 	}
00194: 
00195: 	private static string Show(int value)
00196: 	{
00197: 		return ((value > 0) ? "+" : string.Empty) + value;
00198: 	}
00199: 
00200: 	private static string AttrLabel(string? key)
00201: 	{
00202: 		switch (key)
00203: 		{
00204: 		case "HP":
00205: 			return "生命值";
00206: 		case "MaxHP":
00207: 			return "最大生命";
00208: 		case "ATK":
00209: 			return "攻击力";
00210: 		case "SPD":
00211: 			return "速度";
00212: 		case "HIT":
00213: 			return "命中率";
00214: 		case "DODGE":
00215: 			return "闪避";
00216: 		case "Energy":
00217: 			return "能量";
00218: 		case "WeaponDurability":
00219: 			return "武器耐久";
00220: 		case "AdvWeaponDurability":
00221: 			return "高级武器耐久";
00222: 		case "MoodValue":
00223: 			return "心情值";
00224: 		case "ChipDurability":
00225: 			return "芯片耐久";
00226: 		case "Waste":
00227: 			return "废纸数";
00228: 		case "Junk":
00229: 			return "垃圾数";
00230: 		default:
00231: 			if (!string.IsNullOrEmpty(key))
00232: 			{
00233: 				return key;
00234: 			}
00235: 			return "属性";
00236: 		}
00237: 	}
00238: 
00239: 	private static int ExtraInt(BattleLogEntry e, string key)
00240: 	{
00241: 		object extra = e.GetExtra(key);
00242: 		if (extra is int)
00243: 		{
00244: 			return (int)extra;
00245: 		}
00246: 		return 0;
00247: 	}
00248: 
00249: 	private static string FormatFallback(BattleLogEntry e)
00250: 	{
00251: 		string text = (string.IsNullOrEmpty(e.Kind) ? "未知事件" : e.Kind);
00252: 		if (text.StartsWith("action:", StringComparison.Ordinal))
00253: 		{
00254: 			text = text.Substring("action:".Length);
00255: 		}
00256: 		string text2 = ((!string.IsNullOrEmpty(e.Name)) ? e.Name : e.Label);
00257: 		if (!string.IsNullOrEmpty(text2))
00258: 		{
00259: 			text = text + "：" + text2;
00260: 		}
00261: 		if (e.Amount != 0)
00262: 		{
00263: 			text = text + "（" + e.Amount + "）";
00264: 		}
00265: 		if (e.Source != null && e.Target != null)
00266: 		{
00267: 			return NameTag(e.Source) + " 对 " + NameTag(e.Target) + " 执行了 " + text;
00268: 		}
00269: 		if (e.Source != null)
00270: 		{
00271: 			return NameTag(e.Source) + " 执行了 " + text;
00272: 		}
00273: 		if (e.Target != null)
00274: 		{
00275: 			return NameTag(e.Target) + " 受到 " + text;
00276: 		}
00277: 		return text;
00278: 	}
00279: 
00280: 	public static string Tag(string? text, string hex)
~~~

### GameCardPK/JunkPressCycle.cs

~~~csharp
00124: 	private static string Settle(GameItem press)
00125: 	{
00126: 		SpecialItemDatabase.PressSpec pressSpec = SpecialItemDatabase.PressFor(Core.Clean(press.identifier));
00127: 		if (pressSpec == null)
00128: 		{
00129: 			return "没有 press 参数";
00130: 		}
00131: 		GameSlotInventory? obj = JunkPressWindow.BatterySlot(press);
00132: 		GameItem val = ((obj != null) ? obj.childItem : null);
00133: 		int num = 0;
00134: 		if (val != null)
00135: 		{
00136: 			try
00137: 			{
00138: 				num = PowerHelper.GetAvailableEnergyFromItem(val, false);
00139: 			}
00140: 			catch (Exception ex)
00141: 			{
00142: 				Core.Debug("[垃圾] 读电量失败（按 0 处理）：" + ex.Message);
00143: 			}
00144: 		}
00145: 		if (num < pressSpec.PowerCost)
00146: 		{
00147: 			Core.Debug("[垃圾] 电量 " + num + " < " + pressSpec.PowerCost + "，本夜不结算（不扣料）。");
00148: 			return "电量 " + num + " < " + pressSpec.PowerCost;
00149: 		}
00150: 		List<GameItem> list = Collect(JunkPressWindow.Grid(press, output: false), JunkTags.IsMaterial);
00151: 		if (list.Count < pressSpec.JunkCost)
00152: 		{
00153: 			Core.Debug("[垃圾] 投入格垃圾 " + list.Count + " < " + pressSpec.JunkCost + "，本夜不结算（不扣电）。");
00154: 			return "垃圾 " + list.Count + " < " + pressSpec.JunkCost;
00155: 		}
00156: 		List<GameItem> list2 = Collect(JunkPressWindow.Grid(press, output: true), IsPlayerGameCard);
00157: 		if (list2.Count == 0)
00158: 		{
00159: 			Core.Debug("[垃圾] 产出格没有游戏卡，本夜不结算（不扣电不扣料）。");
00160: 			return "产出格没有游戏卡";
00161: 		}
00162: 		int num2 = 0;
00163: 		if (pressSpec.PowerCost > 0)
00164: 		{
00165: 			if (val == null)
00166: 			{
00167: 				Core.Debug("[垃圾] 电池槽是空的，本夜不结算。");
00168: 				return "没有电池";
00169: 			}
00170: 			try
00171: 			{
00172: 				num2 = PowerHelper.TryRemoveEnergy(val, pressSpec.PowerCost, false);
00173: 			}
00174: 			catch (Exception ex2)
00175: 			{
00176: 				Core.Log?.Error("[垃圾] 扣电失败（本夜放弃）：" + ex2.Message);
00177: 				return "扣电异常";
00178: 			}
00179: 			if (num2 < pressSpec.PowerCost)
00180: 			{
00181: 				Core.Log?.Error("[垃圾] 只扣到 " + num2 + "/" + pressSpec.PowerCost + " 电（预检通过却扣不动？），本夜放弃且不扣垃圾。");
00182: 				return "扣电不足（" + num2 + "/" + pressSpec.PowerCost + "）";
00183: 			}
00184: 		}
00185: 		int num3 = 0;
00186: 		if (pressSpec.JunkCost > 0)
00187: 		{
00188: 			for (int i = 0; i < list.Count; i++)
00189: 			{
00190: 				if (num3 >= pressSpec.JunkCost)
00191: 				{
00192: 					break;
00193: 				}
00194: 				try
00195: 				{
00196: 					ClubInventory.Discard(list[i]);
00197: 					num3++;
00198: 				}
00199: 				catch (Exception ex3)
00200: 				{
00201: 					Core.Log?.Error("[垃圾] 销毁垃圾失败（已忽略这一件）：" + ex3.Message);
00202: 				}
00203: 			}
00204: 		}
00205: 		int num4 = 0;
00206: 		foreach (GameItem item in list2)
00207: 		{
00208: 			try
00209: 			{
00210: 				if (CardDurabilityChecks.ChargeCard(item, pressSpec.Repair))
00211: 				{
00212: 					num4++;
00213: 				}
00214: 			}
00215: 			catch (Exception ex4)
00216: 			{
00217: 				Core.Log?.Error("[垃圾] 修复耐久失败（已跳过这张卡）：" + ex4.Message);
00218: 			}
00219: 		}
00220: 		Core.Debug("[垃圾] 压榨一炉：扣电 " + num2 + " / 扣垃圾 " + num3 + " / 修复 " + num4 + " 张卡（+" + pressSpec.Repair + " 耐久，产出格共 " + list2.Count + " 张）。");
00221: 		return "扣电 " + num2 + "，扣垃圾 " + num3 + "，修复 " + num4 + "/" + list2.Count + " 张";
00222: 	}
00223: 
00224: 	private static List<GameItem> Collect(GameGridInventory? grid, Func<GameItem, bool> match)
00225: 	{
00226: 		List<GameItem> list = new List<GameItem>();
00227: 		if (grid == null)
00228: 		{
00229: 			return list;
~~~

### GameCardPK/BattleAssistant.cs

~~~csharp
00386: 			Core.Log?.Error("[卡牌PK] 传入的卡物品为空。");
00387: 			return null;
00388: 		}
00389: 		string text = Core.Clean(item.name);
00390: 		if (string.IsNullOrEmpty(text))
00391: 		{
00392: 			text = Core.Clean(item.identifier);
00393: 		}
00394: 		int num = ReadTag(item, "HP");
00395: 		int num2 = ReadTag(item, "ATK");
00396: 		int num3 = ReadTag(item, "SPD");
00397: 		int coreForce = ReadTag(item, "CoreForce");
00398: 		if (side == BattleSide.Enemy)
00399: 		{
00400: 			num = ApplyScale(num, ReadTag(item, "HpScale"));
00401: 			num2 = ApplyScale(num2, ReadTag(item, "AtkScale"));
00402: 			num3 = ApplyScale(num3, ReadTag(item, "SpdScale"));
00403: 		}
00404: 		int maxSkillSlots = GameCardData.MaxSkillSlots;
00405: 		List<int> list = new List<int>(maxSkillSlots);
00406: 		for (int i = 1; i <= maxSkillSlots; i++)
00407: 		{
00408: 			string text2 = "OtherForce" + i;
00409: 			if (!TagDisplay.TryGetTagId(text2, out string id))
00410: 			{
00411: 				Core.Log?.Error("[卡牌PK] 技能槽标签缺失：" + text2 + "（配置 skillSlots=" + maxSkillSlots + "，卡牌 " + text + "），该槽加成按 0 处理。");
00412: 				list.Add(0);
00413: 			}
00414: 			else
00415: 			{
00416: 				list.Add(GameCardFactory.ReadInt(item, id));
00417: 			}
00418: 		}
00419: 		if (num <= 0)
00420: 		{
00421: 			Core.Debug("[卡牌PK] 卡牌生命值读到 " + num + "，战斗会很短：" + text);
00422: 		}
00423: 		BattleCardConfig config = new BattleCardConfig(num, num2, num3, coreForce, list);
00424: 		string text3 = CardIdentity.Of(item);
00425: 		string artId = text3;
00426: 		if (ChipSystem.HasAltArt(item))
00427: 		{
00428: 			string text4 = ItemConfigInjector.BareId(CardDatabase.GetCard(text3)?.Mutant);
00429: 			if (text4.Length > 0)
00430: 			{
00431: 				artId = text4;
00432: 			}
00433: 		}
00434: 		BattleCardInstance battleCardInstance = new BattleCardInstance(config, side, text, text3, artId);
00435: 		battleCardInstance.SetSkillSlotLimit(GameCardFactory.ReadSkillSlotMax(item, GameCardData.MaxSkillSlots));
00436: 		LoadSkillSlots(battleCardInstance, item);
00437: 		SnapshotItemTags(battleCardInstance, item);
00438: 		return battleCardInstance;
00439: 	}
00440: 
00441: 	private static void SnapshotItemTags(BattleCardInstance card, GameItem item)
00442: 	{
00443: 		try
00444: 		{
00445: 			card.ItemTags["Energy"] = ReadTag(item, "Energy");
00446: 			card.ItemTags["WeaponDurability"] = ReadTag(item, "WeaponDurability");
00447: 			card.ItemTags["AdvWeaponDurability"] = ReadTag(item, "AdvWeaponDurability");
00448: 		}
00449: 		catch (Exception ex)
00450: 		{
00451: 			Core.Debug("[卡牌PK] 读取卡牌标签快照失败（按 0 处理）：" + ex.Message);
00452: 		}
00453: 	}
00454: 
00455: 	private static int ApplyScale(int value, int percent)
00456: 	{
00457: 		if (percent == 0)
00458: 		{
00459: 			return value;
00460: 		}
00461: 		long num = (long)value * (long)(100 + percent) / 100;
00462: 		if (num < 1)
00463: 		{
00464: 			num = 1L;
00465: 		}
00466: 		if (num > int.MaxValue)
00467: 		{
00468: 			num = 2147483647L;
00469: 		}
00470: 		return (int)num;
00471: 	}
00472: 
00473: 	private static void LoadSkillSlots(BattleCardInstance card, GameItem item)
00474: 	{
00475: 		try
00476: 		{
00477: 			if (item.contentWindow == null)
00478: 			{
00479: 				Core.Log?.Error("[卡牌PK] 卡牌没有内容面板，读不到技能：" + card.Name);
00480: 				return;
00481: 			}
00482: 			int maxSkillSlotColumns = MaxSkillSlotColumns;
00483: 			for (int i = 0; i < maxSkillSlotColumns; i++)
00484: 			{
00485: 				GameSlotInventory val = GameCardFactory.FindContentInventory(item, "skill" + i)?.TryCast<GameSlotInventory>();
00486: 				if (val == null)
00487: 				{
00488: 					continue;
00489: 				}
00490: 				string text = ResolveSkillId(val, Core.Clean(item.identifier), i);
~~~

### GameCardPK/ChipSystem.cs

~~~csharp
00057: 	{
00058: 		if (TraceEnabled && card != null)
00059: 		{
00060: 			bool flag = false;
00061: 			bool flag2 = false;
00062: 			try
00063: 			{
00064: 				flag = ((GameItemFunc)card).canItemTargetThisFunc != null;
00065: 				flag2 = ((GameItemFunc)card).onItemTargetThisFunc != null;
00066: 			}
00067: 			catch (Exception ex)
00068: 			{
00069: 				Trace("shell-ex", "读卡的接收器委托失败：" + ex.Message);
00070: 				return;
00071: 			}
00072: 			Trace("shell|" + flag + "|" + flag2, "造壳完成：canItemTargetThisFunc=" + (flag ? "已挂" : "空！") + " / onItemTargetThisFunc=" + (flag2 ? "已挂" : "空！"));
00073: 		}
00074: 	}
00075: 
00076: 	internal static void ExplainReject(GameItem? chipItem, GameItem? card)
00077: 	{
00078: 		if (chipItem == null || card == null)
00079: 		{
00080: 			return;
00081: 		}
00082: 		try
00083: 		{
00084: 			ChipData chipData = SpecialItemDatabase.ChipOf(chipItem.identifier);
00085: 			string text;
00086: 			if (chipData == null)
00087: 			{
00088: 				text = "被拖的不是芯片（芯片表里没有 " + ItemConfigInjector.BareId(chipItem.identifier) + "）";
00089: 			}
00090: 			else if (!GameCardFactory.IsCard(card))
00091: 			{
00092: 				text = "落点不是游戏卡（没有 game_card tag）";
00093: 			}
00094: 			else if (!ItemOwnership.IsPlayerOwned(card, "卡牌", log: false))
00095: 			{
00096: 				text = "这张卡不归玩家（柜台待售 / 待对战状态）";
00097: 			}
00098: 			else if (HasChip(card))
00099: 			{
00100: 				text = "这张卡已经有芯片了（一张卡只能装一次）";
00101: 			}
00102: 			else
00103: 			{
00104: 				if (Match(card, chipData.Mutant, out string reason))
00105: 				{
00106: 					return;
00107: 				}
00108: 				text = reason;
00109: 			}
00110: 			Trace("reject|" + ItemConfigInjector.BareId(chipItem.identifier) + "|" + CardIdentity.Of(card) + "|" + text, "拖不上：" + Describe(chipItem) + " → " + Describe(card) + "：" + text);
00111: 		}
00112: 		catch (Exception ex)
00113: 		{
00114: 			Trace("reject-ex|" + Describe(chipItem), "说明拒装原因时出错（已忽略）：" + ex.Message);
00115: 		}
00116: 	}
00117: 
00118: 	internal static bool CanInstall(GameItem? chipItem, GameItem? card)
00119: 	{
00120: 		if (chipItem == null || card == null)
00121: 		{
00122: 			return false;
00123: 		}
00124: 		try
00125: 		{
00126: 			ChipData chipData = SpecialItemDatabase.ChipOf(chipItem.identifier);
00127: 			if (chipData == null)
00128: 			{
00129: 				return false;
00130: 			}
00131: 			if (!GameCardFactory.IsCard(card))
00132: 			{
00133: 				return false;
00134: 			}
00135: 			if (!ItemOwnership.IsPlayerOwned(card, "卡牌", log: false))
00136: 			{
00137: 				return false;
00138: 			}
00139: 			if (HasChip(card))
00140: 			{
00141: 				return false;
00142: 			}
00143: 			string reason;
00144: 			return Match(card, chipData.Mutant, out reason);
00145: 		}
00146: 		catch (Exception ex)
00147: 		{
00148: 			Core.Debug("[芯片] 安装判定失败（按不能装处理）：" + ex.Message);
00149: 			return false;
00150: 		}
00151: 	}
00152: 
00153: 	private static bool Match(GameItem card, bool chipMutant, out string reason)
00154: 	{
00155: 		reason = string.Empty;
00156: 		if (!chipMutant)
00157: 		{
00158: 			return true;
00159: 		}
00160: 		if (HasAltArt(card))
00161: 		{
~~~

~~~csharp
00186: 		}
00187: 	}
00188: 
00189: 	internal static bool HasChip(GameItem? card)
00190: 	{
00191: 		if (card == null)
00192: 		{
00193: 			return false;
00194: 		}
00195: 		try
00196: 		{
00197: 			return card.IsTag(CardDurabilitySystem.FullCurTag);
00198: 		}
00199: 		catch (Exception ex)
00200: 		{
00201: 			Core.Debug("[芯片] 读取芯片标签失败（按没有处理）：" + ex.Message);
00202: 			return false;
00203: 		}
00204: 	}
00205: 
00206: 	internal static bool TryInstall(GameItem? chipItem, GameItem? card)
00207: 	{
00208: 		if (chipItem == null || card == null)
00209: 		{
00210: 			return false;
00211: 		}
00212: 		ChipData chipData = SpecialItemDatabase.ChipOf(chipItem.identifier);
00213: 		if (chipData == null)
00214: 		{
00215: 			Trace("install-notchip|" + Describe(chipItem), "TryInstall：被拖的不是芯片（identifier=" + Core.Clean(chipItem.identifier) + "）");
00216: 			return false;
00217: 		}
00218: 		string text = Core.Clean(card.name);
00219: 		if (!GameCardFactory.IsCard(card))
00220: 		{
00221: 			Trace("install-notcard|" + Describe(card), "TryInstall：落点不是游戏卡：" + Core.Clean(card.identifier));
00222: 			return false;
00223: 		}
00224: 		if (!ItemOwnership.IsPlayerOwned(card, "卡牌"))
00225: 		{
00226: 			Trace("install-notown|" + Describe(card), "TryInstall：这张卡还不归你：" + text);
00227: 			return false;
00228: 		}
00229: 		if (HasChip(card))
00230: 		{
00231: 			Trace("install-haschip|" + Describe(card), "TryInstall：「" + text + "」已经有芯片（" + CardDurabilitySystem.FullCurTag + " 存在）。");
00232: 			return false;
00233: 		}
00234: 		if (!Match(card, chipData.Mutant, out string reason))
00235: 		{
00236: 			Trace("install-mismatch|" + Describe(card), "TryInstall：「" + text + "」装不了「" + chipData.DisplayName + "」：" + reason);
00237: 			return false;
00238: 		}
00239: 		int num = ResolveMax(card);
00240: 		if (num <= 0)
00241: 		{
00242: 			Core.Log?.Error("[芯片] 读不到「" + text + "」的耐久上限，这枚芯片不装、不消耗。");
00243: 			return false;
00244: 		}
00245: 		GameCardFactory.ChangeTag("ChipDurabilityMax", "耐久上限", card, num);
00246: 		GameCardFactory.ChangeTag("ChipDurability", "芯片耐久", card, num);
00247: 		int num2 = GameCardFactory.ReadInt(card, CardDurabilitySystem.FullCurTag);
00248: 		if (num2 != num)
00249: 		{
00250: 			Core.Log?.Error("[芯片]「" + text + "」的芯片耐久没写进去（期望 " + num + " 实际 " + num2 + "），已撤回标签，芯片不消耗。");
00251: 			Rollback(card);
00252: 			return false;
00253: 		}
00254: 		if (!ItemConsumption.Consume(chipItem))
00255: 		{
00256: 			Core.Log?.Error("[芯片] 芯片没能消耗，装着的结果已撤回：" + chipData.DisplayName);
00257: 			Rollback(card);
00258: 			return false;
00259: 		}
00260: 		Core.Debug("[芯片]「" + text + "」装上「" + chipData.DisplayName + "」，芯片耐久 " + num + "/" + num + "。");
00261: 		Trace("install-ok|" + Describe(chipItem) + "|" + Describe(card), "TryInstall 成功：" + Describe(chipItem) + " → " + Describe(card) + "，耐久 " + num + "/" + num);
00262: 		if (chipData.Mutant)
00263: 		{
00264: 			MutationSystem.ApplyOnChipInstall(card);
00265: 		}
00266: 		return true;
00267: 	}
00268: 
00269: 	private static int ResolveMax(GameItem card)
00270: 	{
00271: 		try
00272: 		{
00273: 			return CardDatabase.ResolveDurabilityMax(CardDatabase.GetCard(CardIdentity.Of(card)));
00274: 		}
00275: 		catch (Exception ex)
00276: 		{
00277: 			Core.Debug("[芯片] 回查耐久上限失败（用全局默认）：" + ex.Message);
00278: 			return CardDatabase.DefaultDurabilityMax;
00279: 		}
00280: 	}
00281: 
00282: 	private static void Rollback(GameItem card)
00283: 	{
00284: 		try
00285: 		{
00286: 			card.DisableTag(CardDurabilitySystem.FullCurTag, false);
00287: 		}
00288: 		catch (Exception ex)
00289: 		{
00290: 			Core.Debug("[芯片] 撤回耐久标签失败：" + ex.Message);
~~~

### GameCardPK/StickerSystem.cs

~~~csharp
00024: 			TagKey = tagKey;
00025: 			TagName = tagName;
00026: 			TagOnly = tagOnly;
00027: 			MultiTagPrefix = multiTagPrefix;
00028: 			CapToSkillSlots = capToSkillSlots;
00029: 		}
00030: 	}
00031: 
00032: 	internal sealed class Applied
00033: 	{
00034: 		internal GameItem? Card;
00035: 
00036: 		internal EffectRule? Rule;
00037: 
00038: 		internal int BeforeValue;
00039: 
00040: 		internal int BeforeCount;
00041: 
00042: 		internal int CardValueBefore;
00043: 
00044: 		internal string[] TagIds = Array.Empty<string>();
00045: 
00046: 		internal int[] BeforeValues = Array.Empty<int>();
00047: 
00048: 		internal ConsumeData? Spec;
00049: 	}
00050: 
00051: 	internal const string CountTagKey = "Sticker";
00052: 
00053: 	internal const string CountTagName = "已添加贴纸";
00054: 
00055: 	internal const long FallbackStickerValue = 0L;
00056: 
00057: 	private static readonly EffectRule[] Rules = new EffectRule[7]
00058: 	{
00059: 		new EffectRule(StickerEffect.Hp, "HP", "生命值"),
00060: 		new EffectRule(StickerEffect.Atk, "ATK", "攻击力"),
00061: 		new EffectRule(StickerEffect.Spd, "SPD", "速度"),
00062: 		new EffectRule(StickerEffect.Insurance, "Insurance", "保险", tagOnly: true),
00063: 		new EffectRule(StickerEffect.CoreForce, "CoreForce", "核心技能加成"),
00064: 		new EffectRule(StickerEffect.SlotForce, string.Empty, "技能槽加成", tagOnly: false, "OtherForce"),
00065: 		new EffectRule(StickerEffect.SkillSlot, "SkillSlotMax", "技能槽上限", tagOnly: false, "", capToSkillSlots: true)
00066: 	};
00067: 
00068: 	private static string[] ResolveTagKeys(EffectRule rule)
00069: 	{
00070: 		if (!string.IsNullOrEmpty(rule.MultiTagPrefix))
00071: 		{
00072: 			int num = GameCardData.MaxSkillSlots;
00073: 			if (num < 0)
00074: 			{
00075: 				num = 0;
00076: 			}
00077: 			if (num > 32)
00078: 			{
00079: 				num = 32;
00080: 			}
00081: 			string[] array = new string[num];
00082: 			for (int i = 0; i < num; i++)
00083: 			{
00084: 				array[i] = rule.MultiTagPrefix + (i + 1);
00085: 			}
00086: 			return array;
00087: 		}
00088: 		if (!string.IsNullOrEmpty(rule.TagKey))
00089: 		{
00090: 			return new string[1] { rule.TagKey };
00091: 		}
00092: 		return Array.Empty<string>();
00093: 	}
00094: 
00095: 	internal static EffectRule? RuleFor(StickerEffect effect)
00096: 	{
00097: 		EffectRule[] rules = Rules;
00098: 		foreach (EffectRule effectRule in rules)
00099: 		{
00100: 			if (effectRule.Effect == effect)
00101: 			{
00102: 				return effectRule;
00103: 			}
00104: 		}
00105: 		return null;
00106: 	}
00107: 
00108: 	private static long ResolveStickerValue(GameItem? stickerItem, StickerData? data)
00109: 	{
00110: 		if (data != null && data.Value.HasValue && data.Value.Value > 0)
00111: 		{
00112: 			return data.Value.Value;
00113: 		}
00114: 		long num = CardPricing.ReadItemValue(stickerItem);
00115: 		if (num <= 0)
00116: 		{
00117: 			return 0L;
00118: 		}
00119: 		return num;
00120: 	}
00121: 
00122: 	internal static Applied? TryApply(GameItem? stickerItem, GameItem? target)
00123: 	{
00124: 		if (stickerItem == null || target == null)
00125: 		{
00126: 			return null;
00127: 		}
00128: 		StickerData byItemId = StickerDatabase.GetByItemId(stickerItem.identifier);
00129: 		if (byItemId == null)
00130: 		{
00131: 			return null;
00132: 		}
00133: 		if (!target.IsGameItemType("game_card"))
00134: 		{
~~~

### GameCardPK/AuxEffectEffects.cs

~~~csharp
00086: 						break;
00087: 					}
00088: 					case DragEffectKind.Armor:
00089: 					{
00090: 						int num = effect.Amount * count;
00091: 						if (num != 0)
00092: 						{
00093: 							battleCardInstance.GainArmor(num);
00094: 							ctx.Log("【" + name + "】" + battleCardInstance.Name + " 获得 " + num + " 点护甲。");
00095: 							flag = true;
00096: 						}
00097: 						break;
00098: 					}
00099: 					default:
00100: 						ctx.Log("辅助效果「" + name + "」的效果类型【" + effect.Kind.ToString() + "】还没有实现，已跳过。");
00101: 						break;
00102: 					}
00103: 				}
00104: 				catch (Exception ex)
00105: 				{
00106: 					ctx.Log("辅助效果「" + name + "」生效出错（已跳过）：" + ex.Message);
00107: 				}
00108: 			}
00109: 			if (flag)
00110: 			{
00111: 				ctx.Log("【" + name + "】×" + count + " 已生效（仅本场战斗）。");
00112: 			}
00113: 		}
00114: 	}
00115: 
00116: 	private static string AttrLabel(string? key)
00117: 	{
00118: 		switch (key)
00119: 		{
00120: 		case "HP":
00121: 			return "生命值";
00122: 		case "ATK":
00123: 			return "攻击力";
00124: 		case "SPD":
00125: 			return "速度";
00126: 		default:
00127: 			if (!string.IsNullOrEmpty(key))
00128: 			{
00129: 				return key;
00130: 			}
00131: 			return "属性";
00132: 		}
00133: 	}
00134: 
00135: 	private static string Show(int value)
00136: 	{
00137: 		return ((value > 0) ? "+" : string.Empty) + value;
00138: 	}
00139: }
~~~

### GameCardPK/ActionResolver.cs

~~~csharp
00263: 		}
00264: 	}
00265: 
00266: 	private sealed class DrainMaxHpHandler : IActionHandler
00267: 	{
00268: 		private readonly BattleContext _ctx;
00269: 
00270: 		internal DrainMaxHpHandler(BattleContext ctx)
00271: 		{
00272: 			_ctx = ctx;
00273: 		}
00274: 
00275: 		public void Execute(ActionContext ac)
00276: 		{
00277: 			ActionData actionData = ac.Req?.Data;
00278: 			if (actionData == null)
00279: 			{
00280: 				return;
00281: 			}
00282: 			if (actionData.Amount <= 0)
00283: 			{
00284: 				_ctx.Log("【偷取上限】amount 必须为正数，已跳过。");
00285: 				return;
00286: 			}
00287: 			object owner = ((object)ac.Source) ?? ((object)this);
00288: 			foreach (BattleCardInstance item in _ctx.Targets.Resolve(actionData.Target, ac))
00289: 			{
00290: 				if (item == null || item == ac.Source || !item.IsAlive)
00291: 				{
00292: 					continue;
00293: 				}
00294: 				int overflow;
00295: 				int num = item.ReduceMaxHp(actionData.Amount, owner, out overflow);
00296: 				if (num <= 0)
00297: 				{
00298: 					_ctx.Log("【偷取上限】对手已经没有可偷的最大生命值。");
00299: 					continue;
00300: 				}
00301: 				ac.Source?.RaiseMaxHp(num, owner);
00302: 				_ctx.LogEntry(BattleLogEntry.ActionEntry(SkillActionType.DrainMaxHp, ac, item, num, "MaxHP"));
00303: 				if (overflow > 0)
00304: 				{
00305: 					DamageEvent damageEvent = new DamageEvent
00306: 					{
00307: 						Ctx = _ctx,
00308: 						Source = ac.Source,
00309: 						Target = item,
00310: 						Amount = overflow,
00311: 						DamageType = (actionData.DamageType ?? string.Empty),
00312: 						Skill = ac.Skill
00313: 					};
00314: 					_ctx.Fire(SkillStartPoint.BeforeDamage, damageEvent, immediate: true);
00315: 					if (!damageEvent.Canceled && damageEvent.Amount > 0)
00316: 					{
00317: 						int num2 = (damageEvent.HpDamage = item.ApplyHpLoss(damageEvent.Amount));
00318: 						BattleLogEntry battleLogEntry = BattleLogEntry.ActionEntry(SkillActionType.Damage, ac, item, damageEvent.Amount, damageEvent.DamageType);
00319: 						battleLogEntry.SetExtra("armor", 0);
00320: 						battleLogEntry.SetExtra("hp", num2);
00321: 						_ctx.LogEntry(battleLogEntry);
00322: 						_ctx.Fire(SkillStartPoint.AfterDamage, damageEvent, immediate: false);
00323: 					}
00324: 					if (!item.IsAlive)
00325: 					{
00326: 						_ctx.HandleZeroHp(item, ac.Source);
00327: 					}
00328: 					if (_ctx.StopQueue)
00329: 					{
00330: 						break;
00331: 					}
00332: 				}
00333: 			}
00334: 		}
00335: 	}
00336: 
00337: 	private sealed class LoseHpHandler : IActionHandler
00338: 	{
00339: 		private readonly BattleContext _ctx;
00340: 
00341: 		internal LoseHpHandler(BattleContext ctx)
00342: 		{
00343: 			_ctx = ctx;
00344: 		}
00345: 
00346: 		public void Execute(ActionContext ac)
00347: 		{
00348: 			ActionData actionData = ac.Req?.Data;
00349: 			if (actionData == null)
00350: 			{
00351: 				return;
00352: 			}
00353: 			int num = actionData.Value?.Base ?? 0;
00354: 			num *= ac.Req?.AmountMultiplier ?? 1;
00355: 			if (num <= 0)
00356: 			{
00357: 				_ctx.Log("【真掉血】value.base 必须为正数，已跳过。");
00358: 				return;
00359: 			}
00360: 			foreach (BattleCardInstance item in _ctx.Targets.Resolve(actionData.Target, ac))
00361: 			{
00362: 				if (item != null && item.IsAlive)
00363: 				{
00364: 					DamageEvent damageEvent = new DamageEvent
00365: 					{
00366: 						Ctx = _ctx,
00367: 						Source = ac.Source,
~~~

### GameCardPK/EnemyCardGenerator.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: 
00004: namespace GameCardPK;
00005: 
00006: internal static class EnemyCardGenerator
00007: {
00008: 	private const string CanBattleKey = "CanBattle";
00009: 
00010: 	private static readonly string[] ScaleKeys = new string[3] { "HpScale", "AtkScale", "SpdScale" };
00011: 
00012: 	private static readonly string[] ScaleNames = new string[3] { "生命缩放", "攻击缩放", "速度缩放" };
00013: 
00014: 	internal static GameItem? Generate(string? tierId)
00015: 	{
00016: 		try
00017: 		{
00018: 			CardStrengthData cardStrengthData = CardStrengthDatabase.Get(tierId);
00019: 			if (cardStrengthData == null)
00020: 			{
00021: 				Core.Debug("[卡牌PK] 找不到强度档「" + Core.Clean(tierId) + "」（card_strength.json），没生成对战卡。");
00022: 				return null;
00023: 			}
00024: 			GameCardData gameCardData = PickCard(cardStrengthData);
00025: 			if (gameCardData == null)
00026: 			{
00027: 				Core.Debug("[卡牌PK] 强度档 " + cardStrengthData.Id + " 的卡池 " + cardStrengthData.CardTableId + " 一件都没掷出来，没生成对战卡。");
00028: 				return null;
00029: 			}
00030: 			CardVersionSet cardVersionSet = CardVersionRoller.Roll(cardStrengthData.CardTableId, gameCardData);
00031: 			GameItem val = GameCardFactory.CreateCard(gameCardData, cardVersionSet);
00032: 			if (val == null)
00033: 			{
00034: 				Core.Log?.Error("[卡牌PK] 卡牌工厂没能造出 " + gameCardData.Id + "。");
00035: 				return null;
00036: 			}
00037: 			ApplyScaling(val, cardStrengthData);
00038: 			int num = ApplyStickers(val, cardStrengthData);
00039: 			int num2 = ApplySkills(val, cardStrengthData);
00040: 			EnableTag(val, "CanBattle");
00041: 			GameCardFactory.ChangeTagString("RewardTier", val, cardStrengthData.Id);
00042: 			Core.Debug("[卡牌PK] 生成对战卡：" + Core.Clean(val.name) + "（档位 " + cardStrengthData.Id + "，版本 " + cardVersionSet.Describe() + "，贴纸 " + num + "/" + cardStrengthData.StickerRollCount + "，额外技能 " + num2 + "/" + cardStrengthData.SkillRollCount + "，基础价值 " + CardPricing.ReadValue(val) + "）");
00043: 			return val;
00044: 		}
00045: 		catch (Exception ex)
00046: 		{
00047: 			Core.Log?.Error("[卡牌PK] 生成对战卡失败：" + ex);
00048: 			return null;
00049: 		}
00050: 	}
00051: 
00052: 	private static GameCardData? PickCard(CardStrengthData config)
00053: 	{
00054: 		string text = RollTable(config.CardTableId);
00055: 		if (text.Length > 0)
00056: 		{
00057: 			GameCardData card = CardDatabase.GetCard(text);
00058: 			if (card != null)
00059: 			{
00060: 				return card;
00061: 			}
00062: 			Core.Debug("[卡牌PK] 卡池 " + config.CardTableId + " 掷到了不是卡牌的 id：" + text + "（改用第一张）。");
00063: 		}
00064: 		else
00065: 		{
00066: 			Core.Debug("[卡牌PK] 卡池 " + config.CardTableId + " 没掷出东西，改用 cards.json 的第一张。");
00067: 		}
00068: 		return CardDatabase.GetFirstOrDefault();
00069: 	}
00070: 
00071: 	private static void ApplyScaling(GameItem card, CardStrengthData config)
00072: 	{
00073: 		ApplyOne(card, "HpScale", "生命缩放", config.HpPercent);
00074: 		ApplyOne(card, "AtkScale", "攻击缩放", config.AtkPercent);
00075: 		ApplyOne(card, "SpdScale", "速度缩放", config.SpdPercent);
00076: 	}
00077: 
00078: 	private static void ApplyOne(GameItem card, string key, string name, int percent)
00079: 	{
00080: 		if (percent != 0)
00081: 		{
00082: 			GameCardFactory.ChangeTag(key, name, card, percent);
00083: 			int num = GameCardFactory.ReadTag(card, key);
00084: 			if (num != percent)
00085: 			{
00086: 				Core.Log?.Error("[卡牌PK] 「" + name + "」没写进去（期望 " + percent + " 实际 " + num + "）。");
00087: 			}
00088: 		}
00089: 	}
00090: 
00091: 	internal static void StripScaling(GameItem? card)
00092: 	{
00093: 		if (card == null)
00094: 		{
00095: 			return;
00096: 		}
00097: 		for (int i = 0; i < ScaleKeys.Length; i++)
00098: 		{
00099: 			try
00100: 			{
00101: 				if (TagDisplay.NewTagDatas.TryGetValue(ScaleKeys[i], out CustomTagData value) && value != null && card.IsTag(value.Id))
00102: 				{
00103: 					card.DisableTag(value.Id, false);
00104: 				}
00105: 			}
00106: 			catch (Exception ex)
00107: 			{
00108: 				Core.Debug("[卡牌PK] 摘掉「" + ScaleNames[i] + "」标签失败（已忽略）：" + ex.Message);
00109: 			}
00110: 		}
00111: 		try
00112: 		{
00113: 			if (TagDisplay.NewTagDatas.TryGetValue("RewardTier", out CustomTagData value2) && value2 != null && card.IsTag(value2.Id))
00114: 			{
00115: 				card.DisableTag(value2.Id, false);
00116: 			}
00117: 		}
00118: 		catch (Exception ex2)
00119: 		{
00120: 			Core.Debug("[卡牌PK] 摘掉「奖励档位」标签失败（已忽略）：" + ex2.Message);
00121: 		}
00122: 	}
00123: 
00124: 	private static int ApplyStickers(GameItem card, CardStrengthData config)
00125: 	{
00126: 		int num = config.StickerRollCount;
00127: 		if (num <= 0)
00128: 		{
00129: 			return 0;
00130: 		}
00131: 		int maxPerCard = StickerDatabase.MaxPerCard;
00132: 		if (maxPerCard > 0 && num > maxPerCard)
00133: 		{
00134: 			num = maxPerCard;
00135: 		}
00136: 		if (!TrySetOwned(card, owned: true))
00137: 		{
00138: 			return 0;
00139: 		}
00140: 		int num2 = 0;
00141: 		try
00142: 		{
00143: 			for (int i = 0; i < num; i++)
00144: 			{
~~~

### GameCardPK/CardDatabase.cs

~~~csharp
00173: 				Name = "初版",
00174: 				Calc = "add",
00175: 				Value = 30
00176: 			},
00177: 			new VersionPricingEntry
00178: 			{
00179: 				Tag = "Memorial",
00180: 				Name = "纪念版",
00181: 				Calc = "mul",
00182: 				Value = 50
00183: 			},
00184: 			new VersionPricingEntry
00185: 			{
00186: 				Tag = "VariantArt",
00187: 				Name = "异画版",
00188: 				Calc = "mul",
00189: 				Value = 50
00190: 			},
00191: 			new VersionPricingEntry
00192: 			{
00193: 				Tag = "Rare",
00194: 				Name = "稀有版",
00195: 				Calc = "mul",
00196: 				Value = 50
00197: 			},
00198: 			new VersionPricingEntry
00199: 			{
00200: 				Guard = "flashColor",
00201: 				Name = "闪卡版",
00202: 				Calc = "mul",
00203: 				Value = 70
00204: 			},
00205: 			new VersionPricingEntry
00206: 			{
00207: 				Tag = "Damaged",
00208: 				Name = "破损版",
00209: 				Calc = "mul",
00210: 				Value = -20
00211: 			}
00212: 		};
00213: 	}
00214: 
00215: 	internal static long ApplyVersionChain(long baseValue, CardVersionSet? versions)
00216: 	{
00217: 		long num = ((baseValue < 0) ? 0 : baseValue);
00218: 		if (versions == null || versions.IsEmpty)
00219: 		{
00220: 			return num;
00221: 		}
00222: 		IReadOnlyList<VersionPricingEntry> versionPricing = VersionPricing;
00223: 		for (int i = 0; i < versionPricing.Count; i++)
00224: 		{
00225: 			VersionPricingEntry versionPricingEntry = versionPricing[i];
00226: 			if (versionPricingEntry != null && Matches(versionPricingEntry, versions))
00227: 			{
00228: 				num = ApplyStep(num, versionPricingEntry);
00229: 			}
00230: 		}
00231: 		return num;
00232: 	}
00233: 
00234: 	private static bool Matches(VersionPricingEntry entry, CardVersionSet versions)
00235: 	{
00236: 		if (entry.IsFlashGuard)
00237: 		{
00238: 			return versions.FlashColorId.Length > 0;
00239: 		}
00240: 		return versions.Has(entry.Tag);
00241: 	}
00242: 
00243: 	private static long ApplyStep(long value, VersionPricingEntry entry)
00244: 	{
00245: 		string calcKind = entry.CalcKind;
00246: 		if ((calcKind == null || calcKind.Length != 0) && !(calcKind == "none"))
00247: 		{
00248: 			if (calcKind == "add")
00249: 			{
00250: 				value += entry.Value;
00251: 				if (value >= 0)
00252: 				{
00253: 					return value;
00254: 				}
00255: 				return 0L;
00256: 			}
00257: 			if (entry.Value == 0)
00258: 			{
00259: 				return value;
00260: 			}
00261: 			long num = ((entry.Value > 0) ? ((value * entry.Value + 50) / 100) : (-((value * -entry.Value + 50) / 100)));
00262: 			value += num;
00263: 			if (value >= 0)
00264: 			{
00265: 				return value;
00266: 			}
00267: 			return 0L;
00268: 		}
00269: 		return value;
00270: 	}
00271: 
00272: 	private static void NormalizeVersionPricing(List<VersionPricingEntry> chain)
00273: 	{
00274: 		for (int num = chain.Count - 1; num >= 0; num--)
00275: 		{
00276: 			VersionPricingEntry versionPricingEntry = chain[num];
00277: 			if (versionPricingEntry == null)
~~~

~~~csharp
00344: 			return 3;
00345: 		}
00346: 		if (raw > 16)
00347: 		{
00348: 			Core.Log?.Error("[卡牌PK] cards.json 的 maxAuxEffectsPerCard = " + raw + " 超过上限 " + 16 + "，已夹到上限。");
00349: 			return 16;
00350: 		}
00351: 		return raw;
00352: 	}
00353: 
00354: 	internal static GameCardData? GetCard(string? id)
00355: 	{
00356: 		if (string.IsNullOrEmpty(id))
00357: 		{
00358: 			return null;
00359: 		}
00360: 		EnsureLoaded();
00361: 		string key = ItemConfigInjector.PrefixedItem(id);
00362: 		if (!_cards.TryGetValue(key, out GameCardData value))
00363: 		{
00364: 			return null;
00365: 		}
00366: 		return value;
00367: 	}
00368: 
00369: 	internal static GameCardData GetFirstOrDefault()
00370: 	{
00371: 		EnsureLoaded();
00372: 		if (_order.Count > 0)
00373: 		{
00374: 			return _order[0];
00375: 		}
00376: 		return new GameCardData
00377: 		{
00378: 			Id = ItemConfigInjector.PrefixedItem("game_card"),
00379: 			Name = "游戏卡"
00380: 		};
00381: 	}
00382: 
00383: 	internal static void Register(GameCardData data)
00384: 	{
00385: 		if (data == null || string.IsNullOrEmpty(data.Id))
00386: 		{
00387: 			return;
00388: 		}
00389: 		EnsureLoaded();
00390: 		data.Id = ItemConfigInjector.PrefixedItem(data.Id);
00391: 		if (data.Skills != null)
00392: 		{
00393: 			for (int i = 0; i < data.Skills.Count; i++)
00394: 			{
00395: 				data.Skills[i] = ItemConfigInjector.PrefixedItem(data.Skills[i]);
00396: 			}
00397: 		}
00398: 		if (!_cards.ContainsKey(data.Id))
00399: 		{
00400: 			_order.Add(data);
00401: 		}
00402: 		_cards[data.Id] = data;
00403: 	}
00404: 
00405: 	internal static string? GetDefaultSkill(string? cardId, int slotOneBased)
00406: 	{
00407: 		return GetCard(cardId)?.GetSkillAt(slotOneBased);
00408: 	}
00409: 
00410: 	private static void LoadCardKinds(List<CardKindData>? kinds)
00411: 	{
00412: 		_cardKinds.Clear();
00413: 		if (kinds == null || kinds.Count == 0)
00414: 		{
00415: 			Core.Debug("[卡牌PK] cards.json 没写 cardKinds —— 错版卡 / 破损卡这类造物入口不存在。");
00416: 			return;
00417: 		}
00418: 		foreach (CardKindData kind in kinds)
00419: 		{
00420: 			if (kind == null)
00421: 			{
00422: 				continue;
00423: 			}
00424: 			string text = ItemConfigInjector.BareId(Core.Clean(kind.Id));
00425: 			if (text.Length == 0)
00426: 			{
00427: 				Core.Log?.Error("[卡牌PK] cardKinds 里有一条没写 id，已跳过。");
00428: 				continue;
00429: 			}
00430: 			kind.Id = text;
00431: 			kind.Name = Core.Clean(kind.Name);
00432: 			if (kind.Name.Length == 0)
00433: 			{
00434: 				kind.Name = text;
00435: 			}
00436: 			kind.Description = Core.Clean(kind.Description);
00437: 			kind.Flavor = Core.Clean(kind.Flavor);
00438: 			kind.Sprite = Core.Clean(kind.Sprite);
00439: 			if (kind.Sprite.Length == 0)
00440: 			{
00441: 				kind.Sprite = text;
00442: 			}
00443: 			kind.ItemType = ItemConfigInjector.BareId(Core.Clean(kind.ItemType));
00444: 			kind.ReferenceLootTable = Core.Clean(kind.ReferenceLootTable);
00445: 			if (kind.Weight <= 0)
00446: 			{
00447: 				kind.Weight = 100;
00448: 			}
~~~

### GameCardPK/AuxEffectSystem.cs

~~~csharp
00038: 				return string.Empty;
00039: 			}
00040: 			TagState tagReadonly = card.GetTagReadonly(AuxEffectTag);
00041: 			return Core.Clean((tagReadonly != null) ? tagReadonly.GetString() : null);
00042: 		}
00043: 		catch (Exception ex)
00044: 		{
00045: 			Core.Debug("[辅助效果] 读卡上辅助效果标签失败（按没有处理）：" + ex.Message);
00046: 			return string.Empty;
00047: 		}
00048: 	}
00049: 
00050: 	internal static int CountOf(GameItem? card)
00051: 	{
00052: 		return DragTagCodec.ParseIds(ReadPayload(card)).Count;
00053: 	}
00054: 
00055: 	internal static int CountOfItem(GameItem? card, string itemId)
00056: 	{
00057: 		return DragTagCodec.CountOf(ReadPayload(card), itemId);
00058: 	}
00059: 
00060: 	internal static Applied? TryAttach(GameItem? item, GameItem? card)
00061: 	{
00062: 		if (item == null || card == null)
00063: 		{
00064: 			return null;
00065: 		}
00066: 		AuxEffectData byItemId = AuxEffectTable.GetByItemId(item.identifier);
00067: 		if (byItemId == null)
00068: 		{
00069: 			return null;
00070: 		}
00071: 		if (!GameCardFactory.IsCard(card))
00072: 		{
00073: 			Core.Log?.Msg("[辅助效果] 落点上没有 game_card 标签，不是游戏卡，不处理：" + CardIdentity.Of(card));
00074: 			return null;
00075: 		}
00076: 		if (byItemId.Target != DragReceiver.GameCard)
00077: 		{
00078: 			Core.Log?.Error("[辅助效果] " + byItemId.Id + " 的 target 配成了 " + byItemId.Target.ToString() + "，但辅助效果只能落在游戏卡上，已忽略。");
00079: 			return null;
00080: 		}
00081: 		if (!AuxEffectTable.IsUsable(byItemId))
00082: 		{
00083: 			Core.Debug("[辅助效果] " + byItemId.Id + " 没配齐（effects 为空或 consume 不可用），拖上去也不会有任何效果，不消耗。");
00084: 			return null;
00085: 		}
00086: 		string text = ReadPayload(card);
00087: 		List<string> list = DragTagCodec.ParseIds(text);
00088: 		if (!ItemConsumption.CanConsume(item, byItemId.Consume))
00089: 		{
00090: 			Core.Log?.Msg("[辅助效果]「" + byItemId.DisplayText + "」施加者不满足消耗条件（" + byItemId.Consume.Describe() + "），这次拖拽不生效。");
00091: 			return null;
00092: 		}
00093: 		int maxAuxEffectsPerCard = CardDatabase.MaxAuxEffectsPerCard;
00094: 		if (list.Count >= maxAuxEffectsPerCard)
00095: 		{
00096: 			Core.Log?.Msg("[辅助效果]「" + byItemId.DisplayText + "」拖上去无效：这张卡已经有 " + list.Count + " 条辅助效果，上限 " + maxAuxEffectsPerCard + " 条。");
00097: 			return null;
00098: 		}
00099: 		list.Add(byItemId.ItemId);
00100: 		string text2 = DragTagCodec.FormatIds(list);
00101: 		GameCardFactory.ChangeTagString("AuxEffect", card, text2);
00102: 		string text3 = ReadPayload(card);
00103: 		int count = DragTagCodec.ParseIds(text3).Count;
00104: 		if (!string.Equals(text3, text2, StringComparison.Ordinal) || count != list.Count)
00105: 		{
00106: 			Core.Log?.Error("[辅助效果] 卡上标签没写进去（期望「" + text2 + "」实际「" + text3 + "」，条目 " + list.Count + "→" + count + "），这次附着已撤回，物品留着。");
00107: 			Restore(card, text);
00108: 			return null;
00109: 		}
00110: 		Core.Debug("[辅助效果]「" + byItemId.DisplayText + "」已记在卡上（" + list.Count + "/" + maxAuxEffectsPerCard + "）：" + text2);
00111: 		return new Applied
00112: 		{
00113: 			Card = card,
00114: 			ItemId = byItemId.ItemId,
00115: 			BeforePayload = text,
00116: 			Spec = byItemId.Consume
00117: 		};
00118: 	}
00119: 
00120: 	internal static void Consume(GameItem? item, Applied? applied, ConsumeData? spec = null)
00121: 	{
00122: 		if (item != null && !ItemConsumption.Consume(item, spec))
00123: 		{
00124: 			Core.Debug("[辅助效果] 施加者没能消耗，卡上标记已撤回。");
00125: 			Rollback(applied);
00126: 		}
00127: 	}
00128: 
00129: 	private static void Restore(GameItem? card, string payload)
00130: 	{
00131: 		if (card == null)
00132: 		{
00133: 			return;
00134: 		}
00135: 		try
00136: 		{
00137: 			if (string.IsNullOrEmpty(payload))
00138: 			{
00139: 				if (card.IsTag(AuxEffectTag))
00140: 				{
00141: 					card.DisableTag(AuxEffectTag, false);
00142: 				}
00143: 			}
00144: 			else
00145: 			{
00146: 				GameCardFactory.ChangeTagString("AuxEffect", card, payload);
00147: 			}
~~~

### GameCardPK/UI/BattlePanel.cs

~~~csharp
00500: 				{
00501: 					SkillEntry(parent, x2, y2, num4, num5, battleSkillInstance, interactive);
00502: 				}
00503: 			}
00504: 		}
00505: 	}
00506: 
00507: 	private static void EntryName(Transform parent, string name, float x, float y, float w, float h, Color color)
00508: 	{
00509: 		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
00510: 		float size = Mathf.Clamp(h * 0.34f, 13f, 16f);
00511: 		float h2 = Mathf.Clamp(h - 24f, 17f, 24f);
00512: 		UiKit.Text(parent, name, x + 12f, y + 2f, w - 22f, h2, size, color);
00513: 	}
00514: 
00515: 	private static void BasicEntry(Transform parent, float x, float y, float w, float h)
00516: 	{
00517: 		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
00518: 		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
00519: 		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
00520: 		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
00521: 		UiKit.Block block = UiKit.Clickable(parent, "Basic", x, y, w, h, delegate
00522: 		{
00523: 			Select(-1);
00524: 		}, primary: false, 10f);
00525: 		bool flag = selected == -1;
00526: 		if (flag)
00527: 		{
00528: 			((Graphic)block.Frame).color = UiSkin.Gold;
00529: 		}
00530: 		EntryName(parent, "普攻", x, y, w, h, flag ? UiSkin.Gold : UiSkin.Text);
00531: 		if (flag)
00532: 		{
00533: 			UiKit.Box(parent, "Basic.SelBar", x + 6f, y + 8f, 4f, Mathf.Max(6f, h - 16f), UiSkin.Gold);
00534: 		}
00535: 		RegisterHover(x, y, w, h, "普攻\n不消耗，按攻击力结算一次伤害。\n任何回合都能用。");
00536: 	}
00537: 
00538: 	private static void SkillEntry(Transform parent, float x, float y, float w, float h, BattleSkillInstance skill, bool interactive)
00539: 	{
00540: 		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
00541: 		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
00542: 		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
00543: 		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
00544: 		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
00545: 		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
00546: 		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
00547: 		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
00548: 		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
00549: 		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
00550: 		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
00551: 		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
00552: 		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
00553: 		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
00554: 		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
00555: 		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
00556: 		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
00557: 		CardSkillData config = skill.Config;
00558: 		bool flag = skill.RuntimeKind == SkillKind.Passive;
00559: 		bool flag2 = !flag && skill.IsUsable();
00560: 		string text = (flag2 ? string.Empty : skill.GetUnusableReason());
00561: 		bool flag3 = interactive && flag2 && selected == skill.SlotIndex;
00562: 		if (interactive)
00563: 		{
00564: 			int slot = skill.SlotIndex;
00565: 			UiKit.Block block = UiKit.Clickable(parent, "Skill" + slot, x, y, w, h, delegate
00566: 			{
00567: 				Select(slot);
00568: 			}, primary: false, 10f);
00569: 			if (flag3)
00570: 			{
00571: 				((Graphic)block.Frame).color = UiSkin.Gold;
00572: 			}
00573: 			if (!flag2)
00574: 			{
00575: 				((Selectable)block.Button).interactable = false;
00576: 			}
00577: 		}
00578: 		else
00579: 		{
00580: 			UiKit.Panel(parent, "SkillInfo" + skill.SlotIndex, x, y, w, h, new Color(UiSkin.InkSoft.r, UiSkin.InkSoft.g, UiSkin.InkSoft.b, 0.62f), raycast: false, UiSkin.Corner.All, 10f);
00581: 		}
00582: 		string name = (string.IsNullOrEmpty(config.Name) ? config.Id : config.Name);
00583: 		Color color = ((flag || !flag2) ? UiSkin.TextMuted : (flag3 ? UiSkin.Gold : UiSkin.Text));
00584: 		EntryName(parent, name, x, y, w, h, color);
00585: 		float cursor = x + 12f;
00586: 		float limit = x + w - 10f;
00587: 		float y2 = y + h - 21f;
00588: 		bool flag4 = h >= 39f;
00589: 		if (flag4 && !flag2 && !flag && !string.IsNullOrEmpty(text) && !text.StartsWith("冷却中", StringComparison.Ordinal) && !text.StartsWith("次数已用尽", StringComparison.Ordinal))
00590: 		{
00591: 			ChipAt(parent, "Skill" + skill.SlotIndex + ".Reason", ref cursor, limit, y2, ShortReason(text), UiSkin.InkPress, UiSkin.Bad);
00592: 		}
00593: 		if (flag4 && (config.Cooldown > 0 || config.InitialCooldown > 0))
00594: 		{
00595: 			bool flag5 = skill.CooldownRemain > 0;
00596: 			string text2 = (flag5 ? ("冷却 " + skill.CooldownRemain) : ("CD " + config.Cooldown));
00597: 			ChipAt(parent, "Skill" + skill.SlotIndex + ".Cd", ref cursor, limit, y2, text2, UiSkin.InkPress, flag5 ? UiSkin.Bad : UiSkin.TextMuted);
00598: 		}
00599: 		if (flag4 && config.MaxUses > 0)
00600: 		{
00601: 			int num = Mathf.Max(0, config.MaxUses - skill.UsedCount);
00602: 			ChipAt(parent, "Skill" + skill.SlotIndex + ".Uses", ref cursor, limit, y2, "剩 " + num + "/" + config.MaxUses + " 次", UiSkin.InkPress, UiSkin.Text);
00603: 		}
00604: 		if (flag4 && flag)
~~~

~~~csharp
00756: 		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
00757: 		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
00758: 		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
00759: 		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
00760: 		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
00761: 		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
00762: 		float num = 27f;
00763: 		int num2 = FinalAttr(card, "MaxHP", card.Config.MaxHP);
00764: 		int num3 = FinalAttr(card, "ATK", card.Config.ATK);
00765: 		int num4 = FinalAttr(card, "SPD", card.Config.SPD);
00766: 		int num5 = card.CurrentHP;
00767: 		int num6 = card.CurrentArmor;
00768: 		if (TryViewOf(card, out var view))
00769: 		{
00770: 			num5 = view.Hp;
00771: 			num6 = view.Armor;
00772: 			if (view.MaxHp > 0)
00773: 			{
00774: 				num2 = view.MaxHp;
00775: 			}
00776: 		}
00777: 		string text = num5 + "/" + num2;
00778: 		StatRow(parent, x, y, 100f, "生命", text, UiSkin.Text, 0);
00779: 		if (num6 > 0)
00780: 		{
00781: 			float num7 = x + 48f + TextWidth(text, 19f) + 4f;
00782: 			float w2 = Mathf.Max(24f, x + 158f - 4f - num7);
00783: 			TextMeshProUGUI obj = UiKit.Text(parent, "+" + num6, num7, y + 3f, w2, 22f, 15f, UiSkin.TextMuted);
00784: 			((TMP_Text)obj).enableWordWrapping = false;
00785: 			((TMP_Text)obj).overflowMode = (TextOverflowModes)0;
00786: 			((TMP_Text)obj).alignment = (TextAlignmentOptions)4097;
00787: 		}
00788: 		float percent = ((num2 > 0) ? Mathf.Clamp01((float)num5 / (float)num2) : 0f);
00789: 		HpBar(parent, x + 158f, y + 7f, Mathf.Max(48f, Mathf.Min(96f, w - 158f - 2f)), 12f, card, percent);
00790: 		StatRow(parent, x, y + num, w - 90f, "攻击", num3.ToString(), (num3 == card.Config.ATK) ? UiSkin.Text : UiSkin.GoldBright, num3 - card.Config.ATK);
00791: 		StatRow(parent, x, y + num * 2f, w - 90f, "速度", num4.ToString(), (num4 == card.Config.SPD) ? UiSkin.Text : UiSkin.GoldBright, num4 - card.Config.SPD);
00792: 		return 87f;
00793: 	}
00794: 
00795: 	private static bool TryViewOf(BattleCardInstance card, out BattleCardSnapshot view)
00796: 	{
00797: 		view = default(BattleCardSnapshot);
00798: 		BattleSession session = Session;
00799: 		if (session == null || card.Ctx == null)
00800: 		{
00801: 			return false;
00802: 		}
00803: 		int num = Mathf.Min(logRevealed, session.LogTotal);
00804: 		if (num >= session.LogTotal)
00805: 		{
00806: 			return false;
00807: 		}
00808: 		if (!session.TrySnapshotAt(num, out var player, out var enemy))
00809: 		{
00810: 			return false;
00811: 		}
00812: 		view = ((card.Side == BattleSide.Player) ? player : enemy);
00813: 		return true;
00814: 	}
00815: 
00816: 	private static void RefreshStats()
00817: 	{
00818: 		BattleSession session = Session;
00819: 		if (session != null)
00820: 		{
00821: 			RebuildStats(enemyStatsBox, session.Enemy);
00822: 			RebuildStats(playerStatsBox, session.Player);
00823: 		}
00824: 	}
00825: 
00826: 	private static void RebuildStats(GameObject? box, BattleCardInstance? card)
00827: 	{
00828: 		if (UiKit.Alive((Object?)(object)box) && card != null)
00829: 		{
00830: 			Transform transform = box.transform;
00831: 			for (int num = transform.childCount - 1; num >= 0; num--)
00832: 			{
00833: 				GameObject gameObject = ((Component)transform.GetChild(num)).gameObject;
00834: 				gameObject.SetActive(false);
00835: 				Object.Destroy((Object)(object)gameObject);
00836: 			}
00837: 			StatBlock(transform, 0f, 0f, statsBoxW, card);
00838: 		}
00839: 	}
00840: 
00841: 	private static void StatRow(Transform parent, float x, float y, float valueW, string label, string value, Color valueColor, int delta)
00842: 	{
00843: 		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
00844: 		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
00845: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
00846: 		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
00847: 		UiKit.Text(parent, label, x, y, 44f, 26f, 15f, UiSkin.TextMuted);
00848: 		UiKit.Text(parent, value, x + 48f, y, Mathf.Max(40f, valueW), 26f, 19f, valueColor);
00849: 		if (delta != 0)
00850: 		{
00851: 			float num = TextWidth(value, 19f);
00852: 			string value2 = ((delta > 0) ? "+" : "") + delta;
00853: 			((TMP_Text)UiKit.Text(parent, value2, x + 48f + num + 4f, y + 3f, 40f, 22f, 15f, (delta > 0) ? UiSkin.Good : UiSkin.Bad)).alignment = (TextAlignmentOptions)4097;
00854: 		}
00855: 	}
00856: 
00857: 	private static int FinalAttr(BattleCardInstance card, string key, int fallback)
00858: 	{
00859: 		BattleContext ctx = card.Ctx;
00860: 		if (ctx == null)
~~~

## storage

### ItemStorage/GearPanel.cs

~~~csharp
00098: 		{
00099: 			if (b != null)
00100: 			{
00101: 				b.SetText(text);
00102: 			}
00103: 		}
00104: 		catch
00105: 		{
00106: 		}
00107: 	}
00108: 
00109: 	internal static void ToggleFor(GameItem pack)
00110: 	{
00111: 		if (pack == null || !BoxItem.IsBox(pack))
00112: 		{
00113: 			return;
00114: 		}
00115: 		try
00116: 		{
00117: 			PanelEntry panelEntry = FindIn(pack);
00118: 			if (panelEntry?.Window != null && panelEntry.Window.IsVisible())
00119: 			{
00120: 				panelEntry.Window.Hide();
00121: 				return;
00122: 			}
00123: 			if (panelEntry == null)
00124: 			{
00125: 				panelEntry = BuildPanel(pack);
00126: 			}
00127: 			panelEntry.Window.Show();
00128: 			PlaceWindow(pack, panelEntry.Window);
00129: 			panelEntry.Window.ToFront(false);
00130: 		}
00131: 		catch (Exception ex)
00132: 		{
00133: 			Core.Log?.Warning("收纳箱齿轮面板开关异常：" + ex.Message);
00134: 		}
00135: 	}
00136: 
00137: 	internal static void CloseFor(GameItem pack)
00138: 	{
00139: 		try
00140: 		{
00141: 			PanelEntry panelEntry = FindIn(pack);
00142: 			if (panelEntry?.Window != null)
00143: 			{
00144: 				if (panelEntry.Window.IsVisible())
00145: 				{
00146: 					panelEntry.Window.Hide();
00147: 				}
00148: 				try
00149: 				{
00150: 					Object.Destroy((Object)(object)((Component)panelEntry.Window.handler).gameObject);
00151: 				}
00152: 				catch
00153: 				{
00154: 				}
00155: 				Forget(pack, panelEntry);
00156: 			}
00157: 		}
00158: 		catch
00159: 		{
00160: 		}
00161: 	}
00162: 
00163: 	private static void PlaceWindow(GameItem pack, PixelWindow win)
00164: 	{
00165: 		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
00166: 		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
00167: 		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
00168: 		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
00169: 		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
00170: 		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
00171: 		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
00172: 		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
00173: 		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
00174: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00175: 		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
00176: 		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
00177: 		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
00178: 		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
00179: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
00180: 		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
00181: 		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
00182: 		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
00183: 		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
00184: 		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
00185: 		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
00186: 		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
00187: 		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
00188: 		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
00189: 		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
00190: 		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
00191: 		try
00192: 		{
00193: 			PixelWindow contentWindow = pack.contentWindow;
00194: 			if (contentWindow != null)
00195: 			{
00196: 				RectTransform rectTransform = contentWindow.rectTransform;
00197: 				RectTransform rectTransform2 = win.rectTransform;
00198: 				Rect rect = rectTransform.rect;
00199: 				float num = ((Rect)(ref rect)).width / 2f;
00200: 				rect = rectTransform2.rect;
00201: 				float num2 = num + ((Rect)(ref rect)).width / 2f + 6f;
00202: 				rect = rectTransform.rect;
~~~

~~~csharp
00314: 			catch
00315: 			{
00316: 			}
00317: 		});
00318: 		Attach(val2, val9, 1, 3);
00319: 		MachineryRadioButton nameRef = null;
00320: 		MachineryRadioButton val10 = (nameRef = new MachineryRadioButton("隐藏名字", num2 != 0 && !BoxLabel.IsShownFor(num2), MakeBoolAction(delegate(bool on)
00321: 		{
00322: 			try
00323: 			{
00324: 				if (!BoxItem.IsUsableByPlayer(pack))
00325: 				{
00326: 					BoxItem.NotifyUnowned();
00327: 					SyncNameRadio(pack, nameRef);
00328: 				}
00329: 				else
00330: 				{
00331: 					int num3 = 0;
00332: 					try
00333: 					{
00334: 						num3 = pack.uniqueId;
00335: 					}
00336: 					catch
00337: 					{
00338: 					}
00339: 					bool flag = num3 != 0 && BoxLabel.IsShownFor(num3);
00340: 					if (num3 != 0 && flag == on)
00341: 					{
00342: 						BoxLabel.ToggleFor(num3);
00343: 					}
00344: 					SyncNameRadio(pack, nameRef);
00345: 				}
00346: 			}
00347: 			catch (Exception ex2)
00348: 			{
00349: 				Core.Log?.Warning("收纳箱齿轮面板名字开关异常：" + ex2.Message);
00350: 			}
00351: 		})));
00352: 		Attach(val2, val10, 1, 4);
00353: 		PanelEntry panelEntry = new PanelEntry
00354: 		{
00355: 			Window = val,
00356: 			HotkeyButton = val3,
00357: 			AllKeyButton = val4,
00358: 			RestockKeyButton = val5,
00359: 			AllRestockKeyButton = val6,
00360: 			PullRadio = val7,
00361: 			GuardRadio = val8,
00362: 			RestockGuardRadio = val9,
00363: 			NameRadio = val10
00364: 		};
00365: 		try
00366: 		{
00367: 			PixelElement val11 = (PixelElement)(object)val2;
00368: 			PixelElement val12 = val11 ?? throw new InvalidOperationException("grid 非 PixelElement");
00369: 			val.Attach(val12);
00370: 			val.Validate();
00371: 		}
00372: 		catch (Exception ex)
00373: 		{
00374: 			Core.Log?.Warning("收纳箱齿轮面板 Attach 失败：" + ex.Message);
00375: 		}
00376: 		if (num != 0L)
00377: 		{
00378: 			PanelsByUid[num] = panelEntry;
00379: 		}
00380: 		PanelsByRef[pack] = panelEntry;
00381: 		return panelEntry;
00382: 	}
00383: 
00384: 	private static MachineryRadioButton MakeBoolRadio(GameItem pack, string key, Func<GameItem, string> textOf, Action<bool> syncVisual)
00385: 	{
00386: 		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
00387: 		//IL_0055: Expected O, but got Unknown
00388: 		bool flag = StorageGrid.GetBoxOption(pack, key) == 1;
00389: 		return new MachineryRadioButton(textOf(pack), flag, MakeBoolAction(delegate(bool v)
00390: 		{
00391: 			try
00392: 			{
00393: 				if (!BoxItem.IsUsableByPlayer(pack))
00394: 				{
00395: 					BoxItem.NotifyUnowned();
00396: 					syncVisual(!v);
00397: 				}
00398: 				else
00399: 				{
00400: 					StorageGrid.SetBoxOption(pack, key, v ? 1 : 0);
00401: 					syncVisual(v);
00402: 				}
00403: 			}
00404: 			catch (Exception ex)
00405: 			{
00406: 				Core.Log?.Warning("收纳箱齿轮面板开关异常（" + key + "）：" + ex.Message);
00407: 			}
00408: 		}));
00409: 	}
00410: 
00411: 	private static void SyncNameRadio(GameItem pack, MachineryRadioButton? radio)
00412: 	{
00413: 		if (radio == null)
00414: 		{
00415: 			return;
00416: 		}
00417: 		int num = 0;
00418: 		try
00419: 		{
00420: 			num = pack.uniqueId;
00421: 		}
00422: 		catch
00423: 		{
00424: 		}
00425: 		bool flag = num == 0 || !BoxLabel.IsShownFor(num);
00426: 		try
00427: 		{
00428: 			radio.SetOn(flag, false);
00429: 		}
00430: 		catch
00431: 		{
00432: 		}
00433: 	}
00434: 
00435: 	private static void Attach(GridPixelElement grid, MachineryButton button, int col, int row)
00436: 	{
00437: 		if (button == null || !grid.Attach((PixelElement)(object)button, col, row, 116, 24))
00438: 		{
00439: 			Core.Log?.Warning("收纳箱齿轮面板按钮行 (" + col + "," + row + ") attach 失败");
00440: 		}
00441: 	}
00442: 
00443: 	private static void Attach(GridPixelElement grid, MachineryRadioButton radio, int col, int row)
00444: 	{
00445: 		if (radio == null || !grid.Attach((PixelElement)(object)radio, col, row, 116, 24))
00446: 		{
00447: 			Core.Log?.Warning("收纳箱齿轮面板开关行 (" + col + "," + row + ") attach 失败");
00448: 		}
00449: 	}
00450: 
00451: 	private static void AttachText(GridPixelElement grid, MachineryText text, int col, int row)
00452: 	{
00453: 		if (text == null || !grid.Attach((PixelElement)(object)text, col, row, 116, 14))
00454: 		{
00455: 			Core.Log?.Warning("收纳箱齿轮面板标题行 (" + col + "," + row + ") attach 失败");
00456: 		}
00457: 	}
00458: 
00459: 	private unsafe static string HotkeyText(GameItem pack)
00460: 	{
00461: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00462: 		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
00463: 		KeyCode val = (KeyCode)StorageGrid.GetBoxOption(pack, "HOTKEY");
00464: 		if ((int)val != 0)
00465: 		{
00466: 			return "一键收纳:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
00467: 		}
00468: 		return "一键收纳:点击绑定";
00469: 	}
00470: 
00471: 	private unsafe static string RestockKeyText(GameItem pack)
00472: 	{
00473: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00474: 		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
00475: 		KeyCode val = (KeyCode)StorageGrid.GetBoxOption(pack, "HOTRESTOCK");
00476: 		if ((int)val != 0)
00477: 		{
00478: 			return "一键上货:" + ((object)(*(KeyCode*)(&val))/*cast due to .constrained prefix*/).ToString();
00479: 		}
00480: 		return "一键上货:点击绑定";
00481: 	}
00482: 
00483: 	private unsafe static string AllKeyText()
00484: 	{
00485: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00486: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00487: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00488: 		KeyCode key = BoxStowAll.Key;
00489: 		if ((int)key != 0)
00490: 		{
00491: 			return "全箱收纳:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
00492: 		}
00493: 		return "全箱收纳:点击绑定";
00494: 	}
00495: 
00496: 	private unsafe static string AllRestockKeyText()
00497: 	{
00498: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00499: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00500: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00501: 		KeyCode key = BoxRestock.Key;
00502: 		if ((int)key != 0)
00503: 		{
00504: 			return "全箱上货:" + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString();
00505: 		}
00506: 		return "全箱上货:点击绑定";
00507: 	}
00508: 
00509: 	private static string TitleOf(GameItem pack)
00510: 	{
00511: 		string text = BoxLabel.BaseNameOf(pack);
00512: 		if (string.IsNullOrEmpty(text))
00513: 		{
00514: 			return "设置";
00515: 		}
00516: 		if (text.Length > 3)
00517: 		{
00518: 			text = text.Substring(0, 3);
00519: 		}
00520: 		return text + "·设置";
00521: 	}
00522: 
00523: 	private static string PullText(GameItem pack)
00524: 	{
00525: 		return "吸纳同类";
00526: 	}
00527: 
00528: 	private static string GuardText(GameItem pack)
00529: 	{
00530: 		return "禁止被吸纳";
00531: 	}
00532: 
00533: 	private static string RestockGuardText(GameItem pack)
00534: 	{
00535: 		return "不参与全箱上架";
00536: 	}
00537: 
00538: 	private static Action MakeAction(Action body)
00539: 	{
00540: 		return delegate
00541: 		{
00542: 			try
00543: 			{
00544: 				body();
00545: 			}
00546: 			catch (Exception ex)
00547: 			{
00548: 				Core.Log?.Warning("收纳箱齿轮面板按钮异常：" + ex.Message);
00549: 			}
00550: 		};
00551: 	}
00552: 
00553: 	private static Action<bool> MakeBoolAction(Action<bool> body)
00554: 	{
00555: 		return delegate(bool v)
00556: 		{
00557: 			try
00558: 			{
00559: 				body(v);
00560: 			}
00561: 			catch (Exception ex)
00562: 			{
00563: 				Core.Log?.Warning("收纳箱齿轮面板开关异常：" + ex.Message);
00564: 			}
00565: 		};
00566: 	}
00567: 
00568: 	private static Action MakeCaptureAction(GameItem pack, CaptureMode mode)
00569: 	{
00570: 		return MakeAction(delegate
00571: 		{
00572: 			if (!BoxItem.IsUsableByPlayer(pack))
00573: 			{
00574: 				BoxItem.NotifyUnowned();
00575: 			}
00576: 			else
00577: 			{
00578: 				if (mode == CaptureMode.Single || mode == CaptureMode.RestockSingle)
00579: 				{
00580: 					long num = 0L;
00581: 					try
00582: 					{
00583: 						num = pack.uniqueId;
00584: 					}
00585: 					catch
00586: 					{
00587: 					}
00588: 					if (num == 0L)
00589: 					{
00590: 						return;
00591: 					}
00592: 					_captureUid = num;
00593: 				}
00594: 				else
00595: 				{
00596: 					_captureUid = 0L;
00597: 				}
00598: 				_captureMode = mode;
00599: 				_captureUntil = Time.realtimeSinceStartup + 6f;
00600: 				PanelEntry panelEntry = FindIn(pack);
00601: 				_captureButton = (MachineryButton?)(mode switch
00602: 				{
00603: 					CaptureMode.Single => panelEntry?.HotkeyButton, 
00604: 					CaptureMode.All => panelEntry?.AllKeyButton, 
00605: 					CaptureMode.RestockSingle => panelEntry?.RestockKeyButton, 
00606: 					CaptureMode.RestockAll => panelEntry?.AllRestockKeyButton, 
00607: 					_ => null, 
00608: 				});
00609: 				SafeSetText(_captureButton, "按键绑定中，Esc取消");
00610: 			}
00611: 		});
00612: 	}
00613: 
00614: 	internal static void TickCapture()
00615: 	{
00616: 		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
00617: 		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
00618: 		//IL_00d0: Invalid comparison between Unknown and I4
00619: 		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
00620: 		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
00621: 		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
00622: 		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
00623: 		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
00624: 		if ((Object)(object)_bannerTmp != (Object)null && _bannerShownUntil > 0f && Time.realtimeSinceStartup > _bannerShownUntil)
00625: 		{
00626: 			_bannerShownUntil = 0f;
00627: 			try
00628: 			{
00629: 				((Component)((TMP_Text)_bannerTmp).transform.parent).gameObject.SetActive(false);
00630: 			}
00631: 			catch
00632: 			{
00633: 			}
00634: 		}
00635: 		if (_captureMode == CaptureMode.None)
00636: 		{
00637: 			return;
00638: 		}
00639: 		if (!ButtonAlive(_captureButton))
00640: 		{
00641: 			_captureMode = CaptureMode.None;
00642: 			_captureUid = 0L;
00643: 			EndCaptureFrame();
00644: 			_captureButton = null;
00645: 			Core.Debug("收纳箱快捷键：捕获面板已随场景销毁，捕获态自动复位。");
00646: 			return;
00647: 		}
00648: 		if (Time.realtimeSinceStartup > _captureUntil)
00649: 		{
00650: 			CancelCapture("超时未按键，绑定取消");
00651: 			return;
00652: 		}
00653: 		if (Input.GetKeyDown((KeyCode)27))
00654: 		{
00655: 			if (!TryUnbindByEscape())
00656: 			{
00657: 				CancelCapture("绑定取消");
00658: 			}
00659: 			return;
00660: 		}
00661: 		try
00662: 		{
00663: 			if (KeyBindHelper.IsCapturing)
00664: 			{
00665: 				return;
00666: 			}
00667: 		}
00668: 		catch
00669: 		{
00670: 		}
00671: 		for (int i = 8; i <= 350; i++)
00672: 		{
00673: 			KeyCode val = (KeyCode)i;
00674: 			if ((int)val != 27 && Input.GetKeyDown(val))
00675: 			{
00676: 				switch (_captureMode)
00677: 				{
00678: 				case CaptureMode.Single:
00679: 					TryBindSingle(_captureUid, val);
00680: 					break;
00681: 				case CaptureMode.All:
00682: 					TryBindAll(val);
00683: 					break;
00684: 				case CaptureMode.RestockSingle:
00685: 					TryBindRestockSingle(_captureUid, val);
00686: 					break;
00687: 				case CaptureMode.RestockAll:
00688: 					TryBindRestockAll(val);
00689: 					break;
00690: 				}
00691: 				break;
00692: 			}
00693: 		}
00694: 	}
00695: 
00696: 	private unsafe static string? OfficialConflictText(KeyCode key)
00697: 	{
00698: 		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
00699: 		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
00700: 		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
00701: 		try
00702: 		{
00703: 			foreach (Binding binding in KeyBindHelper.Bindings)
00704: 			{
00705: 				if (binding != null && KeyBindHelper.Get(binding.id) == key)
00706: 				{
00707: 					string text;
00708: 					try
00709: 					{
00710: 						text = binding.label?.Invoke();
00711: 					}
00712: 					catch
00713: 					{
00714: 						text = null;
00715: 					}
00716: 					if (!NullGuard.HasText(text))
00717: 					{
00718: 						text = binding.id;
00719: 					}
00720: 					string text2;
00721: 					try
00722: 					{
00723: 						text2 = LocHelper.GetLocalizedMainMenu("option_keybind_status_conflict", new object[2]
00724: 						{
00725: 							KeyBindHelper.KeyName(key),
00726: 							text
00727: 						});
00728: 					}
00729: 					catch
00730: 					{
00731: 						text2 = "与官方按键冲突";
00732: 					}
00733: 					Core.Debug("收纳箱快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 与官方键位 " + binding.id + " 冲突：" + text2);
00734: 					return text2;
00735: 				}
00736: 			}
00737: 		}
00738: 		catch
00739: 		{
00740: 		}
00741: 		return null;
00742: 	}
00743: 
00744: 	private static void RejectBind(GameItem? pack, CaptureMode mode, string msg, float seconds)
00745: 	{
00746: 		PanelEntry? entry = ((pack != null) ? FindIn(pack) : FindEntryByButton(_captureButton));
00747: 		SafeSetText(_captureButton, RestoreTextOf(mode, pack));
00748: 		_captureButton = null;
00749: 		ShowBanner(entry, msg, seconds);
00750: 		BoxItem.NotifyUser("快捷键绑定失败：" + msg);
00751: 		Core.Log?.Msg("收纳箱：快捷键绑定被拒——" + msg + "。");
00752: 	}
00753: 
00754: 	private static string RestoreTextOf(CaptureMode mode, GameItem? pack)
00755: 	{
00756: 		return mode switch
00757: 		{
00758: 			CaptureMode.Single => (pack != null) ? HotkeyText(pack) : "一键收纳:点击绑定", 
00759: 			CaptureMode.All => AllKeyText(), 
00760: 			CaptureMode.RestockSingle => (pack != null) ? RestockKeyText(pack) : "一键上货:点击绑定", 
00761: 			CaptureMode.RestockAll => AllRestockKeyText(), 
00762: 			_ => "点击绑定", 
00763: 		};
00764: 	}
00765: 
00766: 	private static string? ModConflictText(KeyCode key, GameItem? self, bool restockSelf)
00767: 	{
00768: 		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
00769: 		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
00770: 		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
00771: 		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
00772: 		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
00773: 		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
00774: 		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
00775: 		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
00776: 		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
00777: 		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
00778: 		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
00779: 		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
00780: 		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
00781: 		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
00782: 		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
00783: 		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
00784: 		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
00785: 		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
00786: 		try
00787: 		{
00788: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00789: 			{
00790: 				if (item == null)
00791: 				{
00792: 					continue;
00793: 				}
00794: 				bool flag = self != null && item == self;
00795: 				KeyCode val = (KeyCode)StorageGrid.GetBoxOption(item, "HOTKEY");
00796: 				KeyCode val2 = (KeyCode)StorageGrid.GetBoxOption(item, "HOTRESTOCK");
00797: 				if (flag)
00798: 				{
00799: 					if ((restockSelf || val != key) && (!restockSelf || val2 != key))
00800: 					{
00801: 						if (val == key)
00802: 						{
00803: 							return "该键已被本箱一键收纳占用";
00804: 						}
00805: 						if (val2 == key)
00806: 						{
00807: 							return "该键已被本箱一键上货占用";
00808: 						}
00809: 					}
00810: 				}
00811: 				else
00812: 				{
00813: 					if (val == key)
00814: 					{
00815: 						return "该键已被一键收纳占用";
00816: 					}
00817: 					if (val2 == key)
00818: 					{
00819: 						return "该键已被一键上货占用";
00820: 					}
00821: 				}
00822: 			}
00823: 		}
00824: 		catch
00825: 		{
00826: 		}
00827: 		if (BoxStowAll.Key == key)
00828: 		{
00829: 			return "该键已被全箱收纳占用";
00830: 		}
00831: 		if (BoxRestock.Key == key)
00832: 		{
00833: 			return "该键已被全箱上货占用";
00834: 		}
00835: 		return null;
00836: 	}
00837: 
00838: 	private unsafe static void TryBindSingle(long uid, KeyCode key)
00839: 	{
00840: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00841: 		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
00842: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00843: 		//IL_0069: Expected I4, but got Unknown
00844: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00845: 		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
00846: 		GameItem val = FindByUid(uid);
00847: 		_captureMode = CaptureMode.None;
00848: 		EndCaptureFrame();
00849: 		if (val == null)
00850: 		{
00851: 			SafeSetText(_captureButton, "一键收纳:点击绑定");
00852: 			_captureButton = null;
00853: 			return;
00854: 		}
00855: 		string text = OfficialConflictText(key);
00856: 		if (text != null)
00857: 		{
00858: 			RejectBind(val, CaptureMode.Single, text, 3f);
00859: 			return;
00860: 		}
00861: 		string text2 = ModConflictText(key, val, restockSelf: false);
00862: 		if (text2 != null)
00863: 		{
00864: 			RejectBind(val, CaptureMode.Single, text2, 2.5f);
00865: 			return;
00866: 		}
00867: 		StorageGrid.SetBoxOption(val, "HOTKEY", (int)key);
00868: 		SafeSetText(_captureButton, HotkeyText(val));
00869: 		_captureButton = null;
00870: 		BoxHotkey.NotifyBound(uid, key);
00871: 		BoxHotkey.SuppressUntilRelease(key);
00872: 		Core.Log?.Msg("收纳箱：本箱快捷键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00873: 	}
00874: 
00875: 	private unsafe static void TryBindAll(KeyCode key)
00876: 	{
00877: 		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
00878: 		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
00879: 		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
00880: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00881: 		GameItem pack = FindByUid(_captureUid);
00882: 		_captureMode = CaptureMode.None;
00883: 		EndCaptureFrame();
00884: 		string text = OfficialConflictText(key);
00885: 		if (text != null)
00886: 		{
00887: 			RejectBind(pack, CaptureMode.All, text, 3f);
00888: 			return;
00889: 		}
00890: 		string text2 = ModConflictText(key, null, restockSelf: false);
00891: 		if (text2 != null)
00892: 		{
00893: 			RejectBind(pack, CaptureMode.All, text2, 2.5f);
00894: 			return;
00895: 		}
00896: 		BoxStowAll.SetKey(key);
00897: 		SafeSetText(_captureButton, AllKeyText());
00898: 		_captureButton = null;
00899: 		BoxHotkey.SuppressUntilRelease(key);
00900: 		SyncGlobalKeyButtons();
00901: 		Core.Log?.Msg("收纳箱：全箱收纳键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00902: 	}
00903: 
00904: 	private unsafe static void TryBindRestockSingle(long uid, KeyCode key)
00905: 	{
00906: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00907: 		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
00908: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00909: 		//IL_0069: Expected I4, but got Unknown
00910: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00911: 		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
00912: 		GameItem val = FindByUid(uid);
00913: 		_captureMode = CaptureMode.None;
00914: 		EndCaptureFrame();
00915: 		if (val == null)
00916: 		{
00917: 			SafeSetText(_captureButton, "一键上货:点击绑定");
00918: 			_captureButton = null;
00919: 			return;
00920: 		}
00921: 		string text = OfficialConflictText(key);
00922: 		if (text != null)
00923: 		{
00924: 			RejectBind(val, CaptureMode.RestockSingle, text, 3f);
00925: 			return;
00926: 		}
00927: 		string text2 = ModConflictText(key, val, restockSelf: true);
00928: 		if (text2 != null)
00929: 		{
00930: 			RejectBind(val, CaptureMode.RestockSingle, text2, 2.5f);
00931: 			return;
00932: 		}
00933: 		StorageGrid.SetBoxOption(val, "HOTRESTOCK", (int)key);
00934: 		SafeSetText(_captureButton, RestockKeyText(val));
00935: 		_captureButton = null;
00936: 		BoxHotkey.NotifyBound(uid, key, restock: true);
00937: 		BoxHotkey.SuppressUntilRelease(key);
00938: 		Core.Log?.Msg("收纳箱：本箱上货快捷键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00939: 	}
00940: 
00941: 	private unsafe static void TryBindRestockAll(KeyCode key)
00942: 	{
00943: 		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
00944: 		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
00945: 		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
00946: 		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
00947: 		GameItem pack = FindByUid(_captureUid);
00948: 		_captureMode = CaptureMode.None;
00949: 		EndCaptureFrame();
00950: 		string text = OfficialConflictText(key);
00951: 		if (text != null)
00952: 		{
00953: 			RejectBind(pack, CaptureMode.RestockAll, text, 3f);
00954: 			return;
00955: 		}
00956: 		string text2 = ModConflictText(key, null, restockSelf: true);
00957: 		if (text2 != null)
00958: 		{
00959: 			RejectBind(pack, CaptureMode.RestockAll, text2, 2.5f);
00960: 			return;
00961: 		}
00962: 		BoxRestock.SetKey(key);
00963: 		SafeSetText(_captureButton, AllRestockKeyText());
00964: 		_captureButton = null;
00965: 		BoxHotkey.SuppressUntilRelease(key);
00966: 		SyncGlobalKeyButtons();
00967: 		Core.Log?.Msg("收纳箱：全箱上货键绑定为 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + "。");
00968: 	}
00969: 
00970: 	private static void SyncGlobalKeyButtons()
00971: 	{
00972: 		string text = AllKeyText();
00973: 		string text2 = AllRestockKeyText();
00974: 		foreach (PanelEntry value in PanelsByUid.Values)
00975: 		{
00976: 			if (ButtonAlive(value.AllKeyButton))
00977: 			{
00978: 				SafeSetText(value.AllKeyButton, text);
00979: 			}
00980: 			if (ButtonAlive(value.AllRestockKeyButton))
00981: 			{
00982: 				SafeSetText(value.AllRestockKeyButton, text2);
00983: 			}
00984: 		}
00985: 		foreach (PanelEntry value2 in PanelsByRef.Values)
00986: 		{
00987: 			if (ButtonAlive(value2.AllKeyButton))
00988: 			{
00989: 				SafeSetText(value2.AllKeyButton, text);
00990: 			}
00991: 			if (ButtonAlive(value2.AllRestockKeyButton))
00992: 			{
00993: 				SafeSetText(value2.AllRestockKeyButton, text2);
00994: 			}
00995: 		}
00996: 	}
00997: 
00998: 	private static PanelEntry? FindEntryByButton(MachineryButton? btn)
00999: 	{
01000: 		if (btn == null)
01001: 		{
01002: 			return null;
01003: 		}
01004: 		foreach (PanelEntry value in PanelsByUid.Values)
01005: 		{
01006: 			if (value.AllKeyButton == btn || value.HotkeyButton == btn || value.AllRestockKeyButton == btn || value.RestockKeyButton == btn)
01007: 			{
01008: 				return value;
01009: 			}
01010: 		}
01011: 		foreach (PanelEntry value2 in PanelsByRef.Values)
01012: 		{
01013: 			if (value2.AllKeyButton == btn || value2.HotkeyButton == btn || value2.AllRestockKeyButton == btn || value2.RestockKeyButton == btn)
01014: 			{
01015: 				return value2;
01016: 			}
01017: 		}
01018: 		return null;
01019: 	}
01020: 
01021: 	private static bool TryUnbindByEscape()
01022: 	{
01023: 		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
01024: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
01025: 		CaptureMode captureMode = _captureMode;
01026: 		PanelEntry entry = FindEntryByButton(_captureButton);
01027: 		switch (captureMode)
01028: 		{
01029: 		case CaptureMode.All:
01030: 		case CaptureMode.RestockAll:
01031: 			if (captureMode == CaptureMode.All)
01032: 			{
01033: 				if ((int)BoxStowAll.Key == 0)
01034: 				{
01035: 					return false;
01036: 				}
01037: 				BoxStowAll.UnbindManually();
01038: 			}
01039: 			else
01040: 			{
01041: 				if ((int)BoxRestock.Key == 0)
01042: 				{
01043: 					return false;
01044: 				}
01045: 				BoxRestock.UnbindManually();
01046: 			}
01047: 			EndCaptureFrame();
01048: 			_captureMode = CaptureMode.None;
01049: 			_captureUid = 0L;
01050: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, null));
01051: 			_captureButton = null;
01052: 			SyncGlobalKeyButtons();
01053: 			ShowBanner(entry, (captureMode == CaptureMode.All) ? "全箱收纳快捷键已解除绑定" : "全箱上货快捷键已解除绑定", 2.5f);
01054: 			Core.Log?.Msg("收纳箱：" + ((captureMode == CaptureMode.All) ? "全箱收纳键" : "全箱上货键") + "已由玩家在捕获态按 Esc 主动解绑。");
01055: 			return true;
01056: 		case CaptureMode.Single:
01057: 		case CaptureMode.RestockSingle:
01058: 		{
01059: 			GameItem val = FindByUid(_captureUid);
01060: 			string key = ((captureMode == CaptureMode.Single) ? "HOTKEY" : "HOTRESTOCK");
01061: 			if (val == null || StorageGrid.GetBoxOption(val, key) == 0)
01062: 			{
01063: 				return false;
01064: 			}
01065: 			StorageGrid.SetBoxOption(val, key, 0);
01066: 			BoxHotkey.UnbindUidFor(_captureUid, captureMode == CaptureMode.RestockSingle);
01067: 			EndCaptureFrame();
01068: 			_captureMode = CaptureMode.None;
01069: 			_captureUid = 0L;
01070: 			SafeSetText(_captureButton, RestoreTextOf(captureMode, val));
01071: 			_captureButton = null;
01072: 			ShowBanner(entry, (captureMode == CaptureMode.Single) ? "一键收纳快捷键已解除绑定" : "一键上货快捷键已解除绑定", 2.5f);
01073: 			return true;
01074: 		}
01075: 		default:
01076: 			return false;
01077: 		}
01078: 	}
01079: 
01080: 	private static void CancelCapture(string reason)
01081: 	{
01082: 		CaptureMode captureMode = _captureMode;
01083: 		GameItem pack = ((captureMode == CaptureMode.Single || captureMode == CaptureMode.RestockSingle) ? FindByUid(_captureUid) : null);
01084: 		_captureMode = CaptureMode.None;
01085: 		_captureUid = 0L;
01086: 		EndCaptureFrame();
01087: 		SafeSetText(_captureButton, RestoreTextOf(captureMode, pack));
01088: 		_captureButton = null;
01089: 		Core.Debug("收纳箱快捷键：" + reason);
01090: 	}
01091: 
01092: 	private static void ShowBanner(PanelEntry? entry, string text, float seconds)
01093: 	{
01094: 		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
01095: 		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
01096: 		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
01097: 		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
01098: 		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
01099: 		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
01100: 		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
01101: 		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
01102: 		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
01103: 		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
01104: 		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
01105: 		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
01106: 		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
01107: 		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
01108: 		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
01109: 		//IL_0098: Expected O, but got Unknown
01110: 		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
01111: 		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
01112: 		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
01113: 		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
01114: 		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
01115: 		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
01116: 		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
01117: 		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
01118: 		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
01119: 		try
01120: 		{
01121: 			if (entry?.Window == null || string.IsNullOrEmpty(text))
01122: 			{
01123: 				return;
01124: 			}
01125: 			if ((Object)(object)_bannerTmp != (Object)null && _bannerOwner != entry.Window)
01126: 			{
01127: 				try
01128: 				{
01129: 					Object.Destroy((Object)(object)((Component)((TMP_Text)_bannerTmp).transform.parent).gameObject);
01130: 				}
01131: 				catch
01132: 				{
01133: 				}
01134: 				_bannerTmp = null;
01135: 			}
01136: 			if ((Object)(object)_bannerTmp == (Object)null)
01137: 			{
01138: 				GameObject gameObject = ((Component)entry.Window.handler).gameObject;
01139: 				GameObject val = new GameObject("Dream_GearBanner", new Type[1] { typeof(Image) });
01140: 				RectTransform component = val.GetComponent<RectTransform>();
01141: 				((Transform)component).SetParent(gameObject.transform, false);
01142: 				Vector2 val2 = (component.pivot = new Vector2(0.5f, 0.5f));
01143: 				Vector2 anchorMin = (component.anchorMax = val2);
01144: 				component.anchorMin = anchorMin;
01145: 				float num = 0f;
01146: 				try
01147: 				{
01148: 					Rect rect = entry.Window.rectTransform.rect;
01149: 					num = ((Rect)(ref rect)).height;
01150: 				}
01151: 				catch
01152: 				{
01153: 				}
01154: 				component.anchoredPosition = new Vector2(0f, num / 2f + 21f);
01155: 				component.sizeDelta = new Vector2(256f, 34f);
01156: 				((Transform)component).localScale = Vector3.one;
01157: 				Image component2 = val.GetComponent<Image>();
01158: 				((Graphic)component2).color = Color32.op_Implicit(new Color32((byte)18, (byte)14, (byte)16, (byte)236));
01159: 				((Graphic)component2).raycastTarget = false;
01160: 				GameObject val5 = new GameObject("Text", new Type[1] { typeof(RectTransform) });
01161: 				val5.transform.SetParent((Transform)(object)component, false);
01162: 				RectTransform component3 = val5.GetComponent<RectTransform>();
01163: 				component3.anchorMin = Vector2.zero;
01164: 				component3.anchorMax = Vector2.one;
01165: 				component3.offsetMin = new Vector2(7f, 3f);
01166: 				component3.offsetMax = new Vector2(-7f, -3f);
01167: 				TextMeshProUGUI obj3 = val5.AddComponent<TextMeshProUGUI>();
01168: 				((TMP_Text)obj3).font = BoxLabel.GetTmpFont();
01169: 				((TMP_Text)obj3).fontSize = 13f;
01170: 				((Graphic)obj3).color = Color32.op_Implicit(new Color32((byte)204, (byte)72, (byte)73, byte.MaxValue));
01171: 				((TMP_Text)obj3).alignment = (TextAlignmentOptions)514;
01172: 				((TMP_Text)obj3).enableWordWrapping = true;
01173: 				((Graphic)obj3).raycastTarget = false;
01174: 				_bannerTmp = obj3;
01175: 				_bannerOwner = entry.Window;
01176: 			}
01177: 			((Component)((TMP_Text)_bannerTmp).transform.parent).gameObject.SetActive(true);
01178: 			((TMP_Text)_bannerTmp).text = text;
01179: 			_bannerShownUntil = Time.realtimeSinceStartup + seconds;
01180: 		}
01181: 		catch (Exception ex)
01182: 		{
01183: 			Core.Debug("收纳箱齿轮横幅显示异常：" + ex.Message);
01184: 		}
01185: 	}
01186: 
01187: 	private static PanelEntry? FindIn(GameItem pack)
01188: 	{
01189: 		if (pack == null)
01190: 		{
01191: 			return null;
01192: 		}
01193: 		if (PanelsByRef.TryGetValue(pack, out PanelEntry value))
01194: 		{
01195: 			try
01196: 			{
01197: 				if (PanelsByUid.TryGetValue(pack.uniqueId, out PanelEntry value2) && value2.Window == value.Window)
01198: 				{
01199: 					return AliveOrForget(pack, value);
01200: 				}
01201: 			}
01202: 			catch
01203: 			{
01204: 			}
01205: 		}
01206: 		long num = 0L;
01207: 		try
01208: 		{
01209: 			num = pack.uniqueId;
01210: 		}
01211: 		catch
01212: 		{
01213: 		}
01214: 		if (num != 0L && PanelsByUid.TryGetValue(num, out PanelEntry value3) && value3.Window != null)
01215: 		{
01216: 			if (value3.Window == null || !WindowAlive(value3.Window))
01217: 			{
01218: 				ForgetByEntry(value3);
01219: 				return null;
01220: 			}
01221: 			return value3;
01222: 		}
01223: 		return null;
01224: 	}
01225: 
01226: 	private static bool WindowAlive(PixelWindow win)
01227: 	{
01228: 		try
01229: 		{
01230: 			TreeNodeRender handler = win.handler;
01231: 			return (Object)(object)((handler != null) ? ((Component)handler).gameObject : null) != (Object)null;
01232: 		}
01233: 		catch
01234: 		{
01235: 			return false;
01236: 		}
01237: 	}
01238: 
01239: 	private static PanelEntry? AliveOrForget(GameItem pack, PanelEntry entry)
01240: 	{
01241: 		if (!WindowAlive(entry.Window))
01242: 		{
01243: 			ForgetByEntry(entry);
01244: 			return null;
01245: 		}
01246: 		return entry;
01247: 	}
01248: 
01249: 	private static void ForgetByEntry(PanelEntry entry)
01250: 	{
01251: 		try
01252: 		{
~~~

### ItemStorage/BoxItem.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Reflection;
00004: using HarmonyLib;
00005: using UnityEngine;
00006: 
00007: namespace ItemStorage;
00008: 
00009: internal static class BoxItem
00010: {
00011: 	internal const string StorageId = "Dream_Item_Storage";
00012: 
00013: 	internal const string SpriteKey = "Dream_Item_Storage";
00014: 
00015: 	internal const string LegacySpriteKey = "item_storage";
00016: 
00017: 	internal const string UiTag = "ITEM_STORAGE_BOX_UI_V30";
00018: 
00019: 	internal const string DisplayNameText = "收纳箱";
00020: 
00021: 	internal const string GameItemTypeStorage = "STORAGE";
00022: 
00023: 	private static readonly Dictionary<long, GameItem> _known = new Dictionary<long, GameItem>();
00024: 
00025: 	private static readonly FieldInfo? _destroyedFi = AccessTools.Field(typeof(GameItemElement), "destroyed");
00026: 
00027: 	internal const string NumberTag = "DREAM_ITEM_STORAGE_NUM";
00028: 
00029: 	internal const int NoNumber = -1;
00030: 
00031: 	internal static void ResetRegistry()
00032: 	{
00033: 		_known.Clear();
00034: 	}
00035: 
00036: 	internal static bool IsReallyDestroyed(GameItem? item)
00037: 	{
00038: 		try
00039: 		{
00040: 			GameItemElement val = (GameItemElement)(object)((item is GameItemElement) ? item : null);
00041: 			if (val == null)
00042: 			{
00043: 				return true;
00044: 			}
00045: 			object obj = _destroyedFi?.GetValue(val);
00046: 			bool flag = default(bool);
00047: 			int num;
00048: 			if (obj is bool)
00049: 			{
00050: 				flag = (bool)obj;
00051: 				num = 1;
00052: 			}
00053: 			else
00054: 			{
00055: 				num = 0;
00056: 			}
00057: 			return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
00058: 		}
00059: 		catch
00060: 		{
00061: 			return true;
00062: 		}
00063: 	}
00064: 
00065: 	internal static bool IsBeingDragged(GameItem? item)
00066: 	{
00067: 		if (item == null)
00068: 		{
00069: 			return false;
00070: 		}
00071: 		try
00072: 		{
00073: 			if ((Object)(object)ItemMouseDragHandler.current != (Object)null && ItemMouseDragHandler.current.GetCurrentItem() == item)
00074: 			{
00075: 				return true;
00076: 			}
00077: 		}
00078: 		catch
00079: 		{
00080: 		}
00081: 		try
00082: 		{
00083: 			if ((Object)(object)ItemMultiSelectHandler.current != (Object)null && ItemMultiSelectHandler.current.IsGroupDraggingItem(item))
00084: 			{
00085: 				return true;
00086: 			}
00087: 		}
00088: 		catch
~~~

~~~csharp
00433: 			}
00434: 		}
00435: 		catch
00436: 		{
00437: 			return null;
00438: 		}
00439: 		return false;
00440: 	}
00441: 
00442: 	internal static bool IsUsableByPlayer(GameItem? box)
00443: 	{
00444: 		if (!IsBox(box))
00445: 		{
00446: 			return false;
00447: 		}
00448: 		try
00449: 		{
00450: 			return GeneralHelper.IsItemOwned(box);
00451: 		}
00452: 		catch
00453: 		{
00454: 			return true;
00455: 		}
00456: 	}
00457: 
00458: 	internal static GameItem? Create()
00459: 	{
00460: 		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
00461: 		try
00462: 		{
00463: 			Core.Debug("BoxItem: Create() start");
00464: 			GameItem val = ItemDirectory.CreateEmptyItem("Dream_Item_Storage") ?? throw new InvalidOperationException("CreateEmptyItem('Dream_Item_Storage') null");
00465: 			val.identifier = "Dream_Item_Storage";
00466: 			try
00467: 			{
00468: 				val.SetName("收纳箱");
00469: 			}
00470: 			catch
00471: 			{
00472: 			}
00473: 			val.name = "收纳箱";
00474: 			try
00475: 			{
00476: 				val.SetSprite(string.Empty, "Dream_Item_Storage");
00477: 			}
00478: 			catch (Exception ex)
00479: 			{
00480: 				Core.Log?.Warning("收纳箱贴图字段设置失败：" + ex.Message);
00481: 			}
00482: 			try
00483: 			{
00484: 				val.SetShape(new GridShapeBuilder(2, 3).SetDataFill(2, 3, (byte)1).Build());
00485: 			}
00486: 			catch (Exception ex2)
00487: 			{
00488: 				Core.Log?.Warning("收纳箱 SetShape 2x3 失败：" + ex2.Message);
00489: 			}
00490: 			val.SetValue(600L);
00491: 			try
00492: 			{
00493: 				val.SetGameItemType("STORAGE");
00494: 			}
00495: 			catch
00496: 			{
00497: 			}
00498: 			try
00499: 			{
00500: 				val.EnableTag("NESTABLE_DEVICE_TAG", false);
00501: 			}
00502: 			catch
00503: 			{
00504: 			}
00505: 			try
00506: 			{
00507: 				val.EnableTag("ITEM_STORAGE_BOX_UI_V30", false);
00508: 			}
00509: 			catch
00510: 			{
00511: 			}
00512: 			try
00513: 			{
00514: 				ContainerHelper.AddContainerTooltipTag(val);
00515: 			}
00516: 			catch (Exception ex3)
00517: 			{
00518: 				Core.Log?.Warning("收纳箱 AddContainerTooltipTag 失败：" + ex3.Message);
00519: 			}
00520: 			try
00521: 			{
00522: 				AudioHelper.InitPouch(val);
00523: 			}
00524: 			catch
00525: 			{
00526: 				try
00527: 				{
00528: 					AudioHelper.InitPlasticCard(val);
00529: 				}
00530: 				catch
00531: 				{
00532: 				}
00533: 			}
00534: 			BoxWindow.UpdateTooltip(val);
00535: 			BoxWindow.EnsureWindow(val, "Create");
00536: 			Core.Debug("BoxItem: Create() done");
00537: 			return val;
00538: 		}
00539: 		catch (Exception ex4)
00540: 		{
00541: 			Core.Debug("BoxItem: Create() 早期调用失败（渲染树未就绪？返回 null 等真实调用）：" + ex4.Message);
00542: 			return null;
00543: 		}
00544: 	}
00545: 
00546: 	private static void RegisterCore(string label, Func<bool> has, Func<Func<GameItem>, bool> add)
00547: 	{
00548: 		if (has == null || add == null)
00549: 		{
00550: 			return;
00551: 		}
00552: 		try
00553: 		{
00554: 			if (has())
00555: 			{
00556: 				return;
00557: 			}
00558: 		}
00559: 		catch
00560: 		{
00561: 			return;
00562: 		}
00563: 		try
00564: 		{
00565: 			if (DirectoryMaster.Has<GameItem>("Dream_Item_Storage"))
00566: 			{
00567: 				Core.Debug("已注册物品工厂：Dream_Item_Storage 全局已有，跳过 " + label);
00568: 				return;
00569: 			}
00570: 		}
00571: 		catch
00572: 		{
00573: 		}
00574: 		try
00575: 		{
00576: 			Func<GameItem> arg = () => Create() ?? throw new InvalidOperationException("BoxItem.Create 返回 null（目录/模板未就绪）");
00577: 			if (!add(arg))
00578: 			{
00579: 				Core.Log?.Warning("收纳箱注册：" + label + ".Add 返回 false");
00580: 			}
00581: 			else
00582: 			{
00583: 				Core.Debug("已注册物品工厂：Dream_Item_Storage @ " + label);
00584: 			}
00585: 		}
00586: 		catch (Exception ex)
00587: 		{
00588: 			Core.Log?.Error("注册收纳箱失败（" + label + "）：" + ex.Message);
00589: 		}
00590: 	}
00591: 
00592: 	internal static void Register(MiscItemDirectory dir)
00593: 	{
00594: 		RegisterCore("MiscItemDirectory", () => ((Directory<GameItem>)(object)dir).Has("Dream_Item_Storage"), (Func<GameItem> f) => ((Directory<GameItem>)(object)dir).Add("Dream_Item_Storage", f));
00595: 	}
00596: 
00597: 	internal static void Register(ModItemDirectory dir)
00598: 	{
00599: 		RegisterCore("ModItemDirectory", () => ((Directory<GameItem>)(object)dir).Has("Dream_Item_Storage"), (Func<GameItem> f) => ((Directory<GameItem>)(object)dir).Add("Dream_Item_Storage", f));
00600: 	}
00601: 
00602: 	internal static void Register(ContainerItemDirectory dir)
00603: 	{
00604: 		RegisterCore("ContainerItemDirectory", () => ((Directory<GameItem>)(object)dir).Has("Dream_Item_Storage"), (Func<GameItem> f) => ((Directory<GameItem>)(object)dir).Add("Dream_Item_Storage", f));
00605: 	}
00606: }
~~~

### ItemStorage/WelcomeGift.cs

~~~csharp
00092: 				{
00093: 					return;
00094: 				}
00095: 			}
00096: 			try
00097: 			{
00098: 				GeneralHelper.SetItemOwned(val2, true);
00099: 			}
00100: 			catch
00101: 			{
00102: 			}
00103: 			bool flag = TryPlaceInto(val, val2);
00104: 			bool flag2 = false;
00105: 			if (!flag)
00106: 			{
00107: 				EmporiumEntry instance3 = EmporiumEntry.Instance;
00108: 				flag = TryPlaceInto((instance3 != null) ? instance3.backInvinvElement : null, val2);
00109: 				flag2 = flag;
00110: 			}
00111: 			if (!flag)
00112: 			{
00113: 				try
00114: 				{
00115: 					val2.Destroy();
00116: 				}
00117: 				catch
00118: 				{
00119: 				}
00120: 				_pending = false;
00121: 				Core.Log?.Msg("开局礼：桌面与柜台都放不下，本次未发放（水位未推进，下次进店铺重试）。");
00122: 			}
00123: 			else
00124: 			{
00125: 				PerSaveState.SetInt(instance, "BoxNumberWatermark", 1);
00126: 				_issued = true;
00127: 				Core.Log?.Msg(flag2 ? "开局礼：已发放 0 号收纳箱 ×1（桌面放不下，已放柜台 backInvinvElement，玩家所有）。本存档不再发放。" : "开局礼：已发放 0 号收纳箱 ×1（玩家桌面 24×10，玩家所有）。本存档不再发放。");
00128: 				EnsureNumbersForOwned(boxes);
00129: 			}
00130: 		}
00131: 		catch (Exception ex2)
00132: 		{
00133: 			Core.Debug("开局礼发放失败：" + ex2.Message);
00134: 		}
00135: 	}
00136: 
00137: 	private static bool TryPlaceInto(GameGridInventory? grid, GameItem box)
00138: 	{
00139: 		if (grid == null || box == null)
00140: 		{
00141: 			return false;
00142: 		}
00143: 		try
00144: 		{
00145: 			if (grid != null)
00146: 			{
00147: 				GraphUtils.TryAcceptAll((GraphNodeStorage)(object)grid, box, -1);
00148: 			}
00149: 			else
00150: 			{
00151: 				GraphUtils.TryAcceptAllMid(grid, box, -1);
00152: 			}
00153: 		}
00154: 		catch (Exception ex)
00155: 		{
00156: 			Core.Debug("开局礼放置异常：" + ex.Message);
00157: 		}
00158: 		try
00159: 		{
00160: 			GameInventory parentInventory = box.parentInventory;
00161: 			return (object)((parentInventory is GameGridInventory) ? parentInventory : null) == grid;
00162: 		}
00163: 		catch
00164: 		{
00165: 			return false;
00166: 		}
00167: 	}
00168: 
00169: 	private static void EnsureNumbersForOwned(List<GameItem>? boxes = null)
00170: 	{
00171: 		try
00172: 		{
00173: 			if (boxes == null)
00174: 			{
00175: 				boxes = BoxItem.FindAllBoxes();
00176: 			}
00177: 			foreach (GameItem box in boxes)
00178: 			{
00179: 				if (BoxItem.ReadNumber(box) == -1)
00180: 				{
00181: 					bool flag = true;
00182: 					try
00183: 					{
00184: 						flag = GeneralHelper.IsItemOwned(box);
00185: 					}
00186: 					catch
00187: 					{
00188: 					}
00189: 					if (flag)
00190: 					{
00191: 						BoxItem.WriteNumber(box, BoxItem.NextNumber());
00192: 						Core.Log?.Msg("编号：玩家拥有的无编号箱已补号 → " + BoxItem.ReadNumber(box) + " 号。");
00193: 					}
00194: 				}
00195: 			}
00196: 		}
~~~

### ItemStorage/AutoStow.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Diagnostics;
00004: using System.Text;
00005: using UnityEngine;
00006: 
00007: namespace ItemStorage;
00008: 
00009: internal static class AutoStow
00010: {
00011: 	internal static void Run(GameItem pack, string context)
00012: 	{
00013: 		if (pack == null || !BoxItem.IsBox(pack))
00014: 		{
00015: 			return;
00016: 		}
00017: 		try
00018: 		{
00019: 			Core.Debug("收纳箱[" + context + "] 目标箱 uid=" + pack.uniqueId + "，父链=" + ((pack.parentInventory != null) ? "在容器内" : "【孤儿-幽灵箱】") + "。");
00020: 		}
00021: 		catch
00022: 		{
00023: 		}
00024: 		if (!BoxTransfer.IsInStore())
00025: 		{
00026: 			Core.Debug("收纳箱[" + context + "] 一键收纳：玩家不在店内，忽略。");
00027: 			BoxItem.NotifyUser("请回到店内再收纳");
00028: 			return;
00029: 		}
00030: 		Stopwatch stopwatch = Stopwatch.StartNew();
00031: 		Dictionary<string, int> dictionary = ReadSpace(pack, context);
00032: 		if (dictionary == null)
00033: 		{
00034: 			return;
00035: 		}
00036: 		if (dictionary.Count == 0)
00037: 		{
00038: 			Core.Debug("收纳箱[" + context + "] 一键收纳：盒子里没有物品，不工作。");
00039: 			BoxItem.NotifyUser("没有可收纳的物品");
00040: 			return;
00041: 		}
00042: 		List<GameItem> list = CollectCandidates(new HashSet<string>(dictionary.Keys, StringComparer.Ordinal), context);
00043: 		int num = 0;
00044: 		int num2 = 0;
00045: 		int num3 = 0;
00046: 		int num4 = 0;
00047: 		foreach (GameItem item in list)
00048: 		{
00049: 			if (MoveOneInto(pack, item, dictionary, context, out var movedUnits, out var nakedKilled))
00050: 			{
00051: 				num++;
00052: 				num2 += movedUnits;
00053: 				if (nakedKilled)
00054: 				{
00055: 					num4++;
00056: 				}
00057: 			}
00058: 			else
00059: 			{
00060: 				num3++;
00061: 			}
00062: 		}
00063: 		int num5 = 0;
00064: 		try
00065: 		{
00066: 			if (StorageGrid.GetBoxOption(pack, "PULL") == 1)
00067: 			{
00068: 				num5 = GatherFromBoxes(pack, context);
00069: 			}
00070: 		}
00071: 		catch (Exception ex)
00072: 		{
00073: 			Core.Log?.Warning("收纳箱[" + context + "] 箱间联动异常：" + ex.Message);
00074: 		}
00075: 		Core.Debug("收纳箱[" + context + "] 一键收纳耗时 " + stopwatch.ElapsedMilliseconds + " ms（收 " + num + " 叠 / 候选 " + list.Count + " 件）。");
00076: 		if (num > 0 || num5 > 0)
00077: 		{
00078: 			try
00079: 			{
00080: 				BoxWindow.RefreshButtons(pack);
00081: 			}
00082: 			catch
00083: 			{
00084: 			}
00085: 			try
00086: 			{
00087: 				EmporiumEntry instance = EmporiumEntry.Instance;
00088: 				if (instance != null)
00089: 				{
00090: 					instance.Validate(true);
00091: 				}
00092: 			}
00093: 			catch
00094: 			{
00095: 			}
00096: 			Core.Log?.Msg("收纳箱[" + context + "] 一键收纳完成：收进 " + num + " 叠共 " + num2 + " 件" + ((num5 > 0) ? ("，箱间归拢 " + num5 + " 格") : "") + ((num3 > 0) ? ("，" + num3 + " 叠放不下或不可收，跳过") : "") + ((num4 > 0) ? ("，" + num4 + " 件销毁走裸销毁兜底") : "") + "。");
00097: 		}
00098: 		else
00099: 		{
00100: 			Core.Log?.Msg("收纳箱[" + context + "] 一键收纳：店里没有能收进来的物品" + ((num3 > 0) ? ("（" + num3 + " 叠空间不足或不可收，跳过）") : "") + ((num5 > 0) ? ("；从其他收纳箱归拢 " + num5 + " 格") : "") + "。");
00101: 		}
00102: 		if (num2 > 0)
00103: 		{
00104: 			BoxItem.NotifyUser("已收进 " + num2 + " 件");
00105: 		}
00106: 		else if (num5 <= 0)
00107: 		{
00108: 			BoxItem.NotifyUser("没有可收纳的物品");
00109: 		}
00110: 	}
00111: 
00112: 	internal static Dictionary<string, int>? ReadSpace(GameItem pack, string context)
00113: 	{
00114: 		Dictionary<string, int> dictionary = new Dictionary<string, int>(StringComparer.Ordinal);
00115: 		try
00116: 		{
00117: 			for (int i = 0; i < 16; i++)
00118: 			{
00119: 				string itemId = StorageGrid.GetItemId(pack, i);
00120: 				if (NullGuard.HasText(itemId))
00121: 				{
00122: 					int num = 99 - StorageGrid.GetCount(pack, i);
00123: 					if (num > 0)
00124: 					{
00125: 						dictionary.TryGetValue(itemId, out var value);
00126: 						dictionary[itemId] = value + num;
00127: 					}
00128: 				}
00129: 			}
00130: 			return dictionary;
00131: 		}
00132: 		catch (Exception ex)
00133: 		{
00134: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳读槽失败：" + ex.Message);
00135: 			return null;
00136: 		}
00137: 	}
00138: 
00139: 	internal static List<GameItem> CollectCandidates(HashSet<string> interest, string context)
00140: 	{
00141: 		List<GameItem> list = new List<GameItem>();
00142: 		try
00143: 		{
00144: 			EmporiumEntry instance = EmporiumEntry.Instance;
00145: 			if ((Object)(object)instance != (Object)null)
00146: 			{
00147: 				Collect(instance.invElement, "桌面", list);
00148: 				Collect(instance.frontInvinvElement, "柜台正面", list);
00149: 				Collect(instance.backInvinvElement, "柜台里侧", list);
00150: 				Collect(instance.backInvinvElementCounter, "柜台里侧B", list);
00151: 			}
00152: 		}
00153: 		catch (Exception ex)
00154: 		{
00155: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳扫描店铺失败：" + ex.Message);
00156: 		}
00157: 		HashSet<GameItem> hashSet = new HashSet<GameItem>();
00158: 		List<GameItem> list2 = new List<GameItem>();
00159: 		foreach (GameItem item in list)
00160: 		{
00161: 			try
00162: 			{
00163: 				if (!hashSet.Add(item) || BoxItem.IsBox(item))
00164: 				{
00165: 					continue;
00166: 				}
00167: 				string text = Core.Clean(item.identifier) ?? "?";
00168: 				try
00169: 				{
00170: 					if (!GeneralHelper.IsItemOwned(item))
00171: 					{
00172: 						Core.Debug("收纳箱[" + context + "] 跳过（非玩家所有，疑似上柜/商人货）：" + text);
00173: 						continue;
00174: 					}
00175: 				}
00176: 				catch
00177: 				{
00178: 					Core.Debug("收纳箱[" + context + "] 跳过（归属读取失败）：" + text);
00179: 					goto end_IL_00a8;
00180: 				}
00181: 				if (string.IsNullOrEmpty(text) || !interest.Contains(text))
00182: 				{
00183: 					Core.Debug("收纳箱[" + context + "] 跳过（盒内无此种类，非收纳目标）：" + text);
00184: 					continue;
00185: 				}
00186: 				if (IsFullWaterContainer(item))
00187: 				{
00188: 					Core.Debug("收纳箱[" + context + "] 跳过（含水水容器不一键收纳，防混水污染）：" + text);
00189: 					continue;
00190: 				}
00191: 				try
00192: 				{
00193: 					if (item.IsTag("ANIMAL_SPECIES_TAG") || item.IsTag("ANIMAL_AGE_TAG"))
00194: 					{
00195: 						Core.Debug("收纳箱[" + context + "] 跳过（活物不一键收纳，玩家可手动拖入）：" + text);
00196: 						continue;
00197: 					}
00198: 				}
00199: 				catch
00200: 				{
00201: 				}
00202: 				if (BoxTransfer.HasContentInside(item))
00203: 				{
00204: 					Core.Debug("收纳箱[" + context + "] 跳过（容器内有物品，拒收防内容丢失）：" + text);
00205: 					continue;
00206: 				}
00207: 				try
00208: 				{
00209: 					if (BoxItem.IsModuleItem(item) == true)
00210: 					{
00211: 						Core.Debug("收纳箱[" + context + "] 跳过（【模组】标签物品，不一键收纳）：" + text);
00212: 						continue;
00213: 					}
00214: 					if (BoxItem.IsSupplyCrateItem(item) != true)
00215: 					{
00216: 						goto IL_0268;
00217: 					}
00218: 					bool flag = true;
00219: 					try
00220: 					{
00221: 						flag = item.IsGameItemType("SUPPLY_CRATE");
00222: 					}
00223: 					catch
00224: 					{
00225: 						flag = true;
00226: 					}
00227: 					if (flag)
00228: 					{
00229: 						Core.Debug("收纳箱[" + context + "] 跳过（锁定物资箱，不一键收纳）：" + text);
00230: 						continue;
00231: 					}
00232: 					Core.Debug("收纳箱[" + context + "] 刷开物资箱参与收纳：" + text);
00233: 					goto IL_0268;
00234: 					IL_0268:
00235: 					if (BoxItem.IsNodeItem(item) == true)
00236: 					{
00237: 						Core.Debug("收纳箱[" + context + "] 跳过（【节点】标签物品，不一键收纳）：" + text);
00238: 						continue;
00239: 					}
00240: 				}
00241: 				catch
00242: 				{
00243: 				}
00244: 				try
00245: 				{
00246: 					if (StolenHelper.IsStolenItem(item))
00247: 					{
00248: 						Core.Debug("收纳箱[" + context + "] 跳过（赃物，不一键收纳）：" + text);
00249: 						continue;
00250: 					}
00251: 				}
00252: 				catch
00253: 				{
00254: 				}
00255: 				try
00256: 				{
00257: 					if (WineHelper.IsFermentingOrFinished(item))
00258: 					{
00259: 						Core.Debug("收纳箱[" + context + "] 跳过（酿造中/陈酿的酒瓶，不一键收纳）：" + text);
00260: 						continue;
00261: 					}
00262: 				}
00263: 				catch
00264: 				{
00265: 				}
00266: 				list2.Add(item);
00267: 				end_IL_00a8:;
00268: 			}
00269: 			catch
00270: 			{
00271: 			}
00272: 		}
00273: 		return list2;
00274: 	}
00275: 
00276: 	internal static bool MoveOneInto(GameItem pack, GameItem it, Dictionary<string, int> space, string context, out int movedUnits, out bool nakedKilled)
00277: 	{
00278: 		movedUnits = 0;
00279: 		nakedKilled = false;
00280: 		try
00281: 		{
00282: 			string text = Core.Clean(it.identifier) ?? string.Empty;
00283: 			int num;
00284: 			try
00285: 			{
00286: 				num = Math.Max(1, it.unitCount);
00287: 			}
00288: 			catch
00289: 			{
00290: 				num = 1;
00291: 			}
00292: 			space.TryGetValue(text, out var value);
00293: 			if (num > value)
00294: 			{
00295: 				Core.Debug("收纳箱[" + context + "] 跳过（一叠 " + num + " 件超过剩余空间 " + value + "）：" + text);
00296: 				return false;
00297: 			}
00298: 			List<GraphNodeStorage> list = null;
00299: 			try
00300: 			{
00301: 				list = it.parents;
00302: 			}
00303: 			catch
00304: 			{
00305: 				list = null;
00306: 			}
00307: 			if (list == null || list.Count == 0)
00308: 			{
00309: 				Core.Debug("收纳箱[" + context + "] 跳过（无网格父节点，物品悬空/拖拽中）：" + text);
00310: 				return false;
00311: 			}
00312: 			GameGridInventory val = null;
00313: 			try
00314: 			{
00315: 				GameInventory parentInventory = it.parentInventory;
00316: 				val = (GameGridInventory)(object)((parentInventory is GameGridInventory) ? parentInventory : null);
00317: 			}
00318: 			catch
00319: 			{
00320: 				val = null;
00321: 			}
00322: 			bool flag = false;
00323: 			if (val != null)
00324: 			{
00325: 				try
00326: 				{
00327: 					flag = ((GameInventory)val).Expel(it);
00328: 				}
00329: 				catch (Exception ex)
00330: 				{
00331: 					Core.Log?.Warning("收纳箱[" + context + "] Expel 失败（回退解绑姿势）：" + ex.Message);
00332: 				}
00333: 			}
00334: 			if (!flag && !DetachFromGrid(it, list, context))
00335: 			{
00336: 				Core.Debug("收纳箱[" + context + "] 跳过（网格父节点移除不完整，物品原样保留）：" + text);
00337: 				return false;
00338: 			}
00339: 			if (!BoxTransfer.TryDeposit(pack, it, 0, -1, consume: false, refresh: false, notify: false))
00340: 			{
00341: 				if (val != null)
00342: 				{
00343: 					try
00344: 					{
00345: 						GraphUtils.TryAcceptAllMid(val, it, -1);
00346: 					}
00347: 					catch
00348: 					{
00349: 					}
00350: 				}
00351: 				bool flag2 = false;
00352: 				try
00353: 				{
00354: 					flag2 = it.parentInventory != null;
00355: 				}
00356: 				catch
00357: 				{
00358: 					flag2 = false;
00359: 				}
00360: 				if (!flag2)
00361: 				{
00362: 					bool flag3 = false;
00363: 					try
00364: 					{
00365: 						flag3 = GraphUtils.DestroyIfUnplaced(it, "ItemStorage_AutoStow");
00366: 					}
00367: 					catch
00368: 					{
00369: 						flag3 = false;
00370: 					}
00371: 					if (!flag3)
00372: 					{
00373: 						try
00374: 						{
00375: 							it.Destroy();
00376: 						}
00377: 						catch
00378: 						{
00379: 							try
00380: 							{
00381: 								it.unitCount = 0;
00382: 							}
00383: 							catch
00384: 							{
00385: 							}
00386: 						}
00387: 					}
00388: 				}
00389: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳：'" + text + "' 存入失败，物品已" + (flag2 ? "放回原网格" : "销毁防悬空"));
00390: 				return false;
00391: 			}
00392: 			bool flag4 = false;
00393: 			try
00394: 			{
00395: 				flag4 = GraphUtils.DestroyIfUnplaced(it, "ItemStorage_AutoStow");
00396: 			}
00397: 			catch
00398: 			{
00399: 				flag4 = false;
00400: 			}
00401: 			if (!flag4)
00402: 			{
00403: 				try
00404: 				{
00405: 					List<GraphNodeStorage> parents = it.parents;
00406: 					if (parents != null)
00407: 					{
00408: 						foreach (GraphNodeStorage item in parents)
00409: 						{
00410: 							if (item != null)
00411: 							{
00412: 								try
00413: 								{
00414: 									it.RemoveParent(item);
00415: 								}
00416: 								catch
00417: 								{
00418: 								}
00419: 							}
00420: 						}
00421: 					}
00422: 				}
00423: 				catch
00424: 				{
00425: 				}
00426: 				try
00427: 				{
00428: 					it.Destroy();
00429: 					nakedKilled = true;
00430: 				}
00431: 				catch
00432: 				{
00433: 					try
00434: 					{
00435: 						it.unitCount = 0;
00436: 					}
00437: 					catch
00438: 					{
00439: 					}
00440: 				}
00441: 				Core.Debug("收纳箱[" + context + "] 一键收纳：'" + text + "' 官方销毁未生效，已裸销毁（占位已由 Expel 释放）。");
00442: 			}
00443: 			space[text] = value - num;
00444: 			movedUnits = num;
00445: 			return true;
00446: 		}
00447: 		catch (Exception ex2)
00448: 		{
00449: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳单件失败：" + ex2.Message);
00450: 			return false;
00451: 		}
00452: 	}
00453: 
00454: 	private static int GatherFromBoxes(GameItem pack, string context)
00455: 	{
00456: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
00457: 		for (int i = 0; i < 16; i++)
00458: 		{
00459: 			string text = VariantKeyOf(pack, i);
00460: 			if (text != null)
00461: 			{
00462: 				hashSet.Add(text);
00463: 			}
00464: 		}
00465: 		if (hashSet.Count == 0)
00466: 		{
00467: 			return 0;
00468: 		}
00469: 		List<int> list = StorageGrid.EmptySlots(pack);
00470: 		int num = 0;
00471: 		foreach (GameItem item in BoxItem.FindAllBoxes())
00472: 		{
00473: 			if (item == null || item == pack)
00474: 			{
00475: 				continue;
00476: 			}
00477: 			try
00478: 			{
00479: 				if (!BoxItem.IsUsableByPlayer(item) || StorageGrid.GetBoxOption(item, "GUARD") == 1)
00480: 				{
00481: 					continue;
00482: 				}
00483: 				for (int j = 0; j < 16; j++)
00484: 				{
00485: 					if (SlotLock.IsLocked(item, j))
00486: 					{
00487: 						continue;
00488: 					}
00489: 					string text2 = VariantKeyOf(item, j);
00490: 					if (text2 == null || !hashSet.Contains(text2))
00491: 					{
00492: 						continue;
00493: 					}
00494: 					if (StorageGrid.IsWaterSlot(item, j))
00495: 					{
00496: 						bool flag = true;
00497: 						try
00498: 						{
00499: 							flag = StorageGrid.GetStoredWaterML(item, j) > 0;
00500: 						}
00501: 						catch
00502: 						{
00503: 						}
00504: 						if (flag)
00505: 						{
00506: 							continue;
00507: 						}
00508: 					}
00509: 					if (TagSnapshot.PeekLastAnimal(item, j).HasValue)
00510: 					{
00511: 						continue;
00512: 					}
00513: 					int num2 = StorageGrid.FindMergeTarget(pack, item, j);
00514: 					if (num2 >= 0 && StorageGrid.MergeSlots(item, j, pack, num2))
00515: 					{
00516: 						num++;
00517: 						try
00518: 						{
00519: 							BoxWindow.RefreshButtons(item);
00520: 						}
00521: 						catch
00522: 						{
00523: 						}
00524: 						Core.Debug("收纳箱[" + context + "] 箱间联动：'" + text2 + "' 并入本箱已有叠（源格 " + j + " → 本箱格 " + num2 + "）。");
00525: 					}
00526: 					else
00527: 					{
00528: 						if (list.Count == 0)
00529: 						{
00530: 							continue;
00531: 						}
00532: 						int num3 = list[list.Count - 1];
00533: 						list.RemoveAt(list.Count - 1);
00534: 						if (!StorageGrid.MoveSlotTo(item, j, pack, num3))
00535: 						{
00536: 							list.Add(num3);
00537: 							continue;
00538: 						}
00539: 						num++;
00540: 						try
00541: 						{
00542: 							BoxWindow.RefreshButtons(item);
00543: 						}
00544: 						catch
00545: 						{
00546: 						}
00547: 						Core.Debug("收纳箱[" + context + "] 箱间联动：'" + text2 + "' 整格自其他箱归拢（源格 " + j + " → 本箱格 " + num3 + "）。");
00548: 					}
00549: 				}
00550: 			}
00551: 			catch
00552: 			{
00553: 			}
00554: 		}
00555: 		return num;
00556: 	}
00557: 
00558: 	private static string? VariantKeyOf(GameItem pack, int slot)
00559: 	{
00560: 		string itemId = StorageGrid.GetItemId(pack, slot);
00561: 		if (string.IsNullOrEmpty(itemId))
00562: 		{
00563: 			return null;
00564: 		}
00565: 		bool? slotDeadFlag = StorageGrid.GetSlotDeadFlag(pack, slot);
00566: 		return itemId + "\u0002" + StorageGrid.GetSlotSignature(pack, slot) + "\u0002" + (slotDeadFlag?.ToString() ?? "-");
00567: 	}
00568: 
00569: 	private static void Collect(GameGridInventory? grid, string label, List<GameItem> into)
00570: 	{
00571: 		if (grid == null)
00572: 		{
00573: 			Core.Debug("收纳箱一键收纳扫描：" + label + " 网格为空（未初始化）。");
00574: 			return;
00575: 		}
00576: 		try
00577: 		{
00578: 			int num = 0;
00579: 			StringBuilder stringBuilder = new StringBuilder(128);
00580: 			foreach (GameItem childItem in ((GameInventory)grid).childItems)
00581: 			{
00582: 				if (childItem != null)
00583: 				{
00584: 					into.Add(childItem);
00585: 					num++;
00586: 					stringBuilder.Append(' ').Append(Core.Clean(childItem.identifier));
00587: 				}
00588: 			}
00589: 			Core.Debug("收纳箱一键收纳扫描：" + label + " 共 " + num + " 件：" + ((num > 0) ? stringBuilder.ToString() : "无"));
00590: 		}
00591: 		catch (Exception ex)
00592: 		{
00593: 			Core.Log?.Warning("收纳箱：枚举" + label + "物品失败：" + ex.Message);
00594: 		}
00595: 	}
00596: 
00597: 	private static bool DetachFromGrid(GameItem it, List<GraphNodeStorage> parents, string context)
00598: 	{
00599: 		try
00600: 		{
00601: 			int num = 0;
00602: 			foreach (GraphNodeStorage parent in parents)
00603: 			{
00604: 				if (parent == null)
00605: 				{
00606: 					num++;
00607: 					continue;
00608: 				}
00609: 				try
00610: 				{
00611: 					num += (it.RemoveParent(parent) ? 1 : 0);
00612: 				}
00613: 				catch
00614: 				{
00615: 					num = num;
00616: 				}
00617: 			}
00618: 			if (num < parents.Count)
00619: 			{
00620: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳：物品父节点移除不完整（" + num + "/" + parents.Count + "）。");
00621: 				return false;
00622: 			}
00623: 			return true;
00624: 		}
00625: 		catch (Exception ex)
00626: 		{
00627: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳脱离网格失败：" + ex.Message);
00628: 			return false;
00629: 		}
00630: 	}
00631: 
00632: 	private static bool IsFullWaterContainer(GameItem it)
00633: 	{
00634: 		try
00635: 		{
00636: 			bool flag = false;
00637: 			try
00638: 			{
00639: 				flag = WaterFeatureHelper.GetWaterFeature(it) != null;
00640: 			}
00641: 			catch
00642: 			{
00643: 			}
00644: 			if (!flag)
00645: 			{
00646: 				return false;
00647: 			}
00648: 			bool flag2 = true;
00649: 			try
00650: 			{
00651: 				flag2 = WaterHelper.IsEmpty(it);
00652: 			}
00653: 			catch
00654: 			{
00655: 			}
00656: 			if (!flag2)
00657: 			{
00658: 				return true;
00659: 			}
00660: 			try
00661: 			{
00662: 				if (WaterHelper.GetCurrentCapacityML(it) > 0)
00663: 				{
00664: 					return true;
00665: 				}
00666: 			}
00667: 			catch
00668: 			{
00669: 			}
00670: 			return false;
00671: 		}
00672: 		catch
00673: 		{
00674: 			return false;
00675: 		}
00676: 	}
00677: }
~~~

### ItemStorage/BoxHotkey.cs

~~~csharp
00032: 
00033: 	private static bool IsLiveBox(GameItem box)
00034: 	{
00035: 		try
00036: 		{
00037: 			if (box.parentInventory == null)
00038: 			{
00039: 				return false;
00040: 			}
00041: 			return !BoxItem.IsReallyDestroyed(box);
00042: 		}
00043: 		catch
00044: 		{
00045: 			return false;
00046: 		}
00047: 	}
00048: 
00049: 	private static void RestoreFromBoxes()
00050: 	{
00051: 		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
00052: 		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
00053: 		int num = 0;
00054: 		try
00055: 		{
00056: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00057: 			{
00058: 				num++;
00059: 				try
00060: 				{
00061: 					long num2 = item.uniqueId;
00062: 					int boxOption = StorageGrid.GetBoxOption(item, "HOTKEY");
00063: 					int boxOption2 = StorageGrid.GetBoxOption(item, "HOTRESTOCK");
00064: 					if (boxOption != 0 && (!BoundKeys.TryGetValue(boxOption, out var value) || value.Restock || ResolveBox(value.Uid) == null))
00065: 					{
00066: 						BoundKeys[boxOption] = new HotkeyEntry(num2, restock: false);
00067: 						Core.Debug("快捷键自愈：恢复 " + ((object)(KeyCode)boxOption/*cast due to .constrained prefix*/).ToString() + " → 箱 uid " + num2 + "（一键收纳）" + ((value.Uid != 0L && value.Uid != num2) ? ("（旧 uid " + value.Uid + " 已失效，覆盖）") : "") + "。");
00068: 					}
00069: 					if (boxOption2 != 0 && (!BoundKeys.TryGetValue(boxOption2, out var value2) || !value2.Restock || ResolveBox(value2.Uid) == null))
00070: 					{
00071: 						BoundKeys[boxOption2] = new HotkeyEntry(num2, restock: true);
00072: 						Core.Debug("快捷键自愈：恢复 " + ((object)(KeyCode)boxOption2/*cast due to .constrained prefix*/).ToString() + " → 箱 uid " + num2 + "（一键上货）" + ((value2.Uid != 0L && value2.Uid != num2) ? ("（旧 uid " + value2.Uid + " 已失效，覆盖）") : "") + "。");
00073: 					}
00074: 				}
00075: 				catch
00076: 				{
00077: 				}
00078: 			}
00079: 		}
00080: 		catch
00081: 		{
00082: 		}
00083: 		if (num > 0)
00084: 		{
00085: 			_noBinding = true;
00086: 			Core.Debug("快捷键自愈：本轮扫到 " + num + " 箱，恢复 " + BoundKeys.Count + " 键，停扫。");
00087: 		}
00088: 	}
00089: 
00090: 	internal static void ResetRecovery()
00091: 	{
00092: 		BoundKeys.Clear();
00093: 		_noBinding = false;
00094: 		_nextRestore = 0f;
00095: 		_lastHitBox = null;
00096: 		_lastHitUid = 0L;
00097: 		BoxItem.ResetRegistry();
00098: 		BoxStowAll.ResetRecovery();
00099: 		BoxRestock.ResetRecovery();
00100: 	}
00101: 
00102: 	internal static void SuppressUntilRelease(KeyCode key)
00103: 	{
00104: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00105: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00106: 		_suppressKey = key;
00107: 	}
00108: 
00109: 	internal static bool IsSuppressed(KeyCode key)
00110: 	{
00111: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00112: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00113: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
00114: 		if ((int)_suppressKey != 0)
00115: 		{
00116: 			return _suppressKey == key;
00117: 		}
00118: 		return false;
00119: 	}
00120: 
00121: 	internal static void NotifyBound(long uid, KeyCode key, bool restock = false)
00122: 	{
00123: 		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
00124: 		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
00125: 		//IL_00ac: Expected I4, but got Unknown
00126: 		try
00127: 		{
00128: 			List<int> list = new List<int>();
00129: 			foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00130: 			{
00131: 				if (boundKey.Value.Uid == uid && boundKey.Value.Restock == restock)
00132: 				{
00133: 					list.Add(boundKey.Key);
00134: 				}
00135: 			}
00136: 			foreach (int item in list)
00137: 			{
00138: 				BoundKeys.Remove(item);
00139: 			}
00140: 			if ((int)key != 0)
00141: 			{
00142: 				BoundKeys[(int)key] = new HotkeyEntry(uid, restock);
00143: 			}
00144: 		}
00145: 		catch
00146: 		{
00147: 		}
00148: 	}
00149: 
00150: 	internal static void Tick()
00151: 	{
00152: 		TickSingleBoxKeys();
00153: 		BoxStowAll.Tick();
00154: 		BoxRestock.Tick();
00155: 		BoxStowAll.Watch(BoundKeys.Count);
00156: 	}
00157: 
00158: 	internal static bool UnbindUidFor(long uid, bool restock)
00159: 	{
00160: 		try
00161: 		{
00162: 			List<int> list = new List<int>();
00163: 			foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00164: 			{
00165: 				if (boundKey.Value.Uid == uid && boundKey.Value.Restock == restock)
00166: 				{
00167: 					list.Add(boundKey.Key);
00168: 				}
00169: 			}
00170: 			foreach (int item in list)
00171: 			{
00172: 				BoundKeys.Remove(item);
00173: 			}
00174: 			if (list.Count > 0)
00175: 			{
00176: 				Core.Log?.Msg("快捷键映射：箱 uid " + uid + " 的" + (restock ? "一键上货键" : "一键收纳键") + "已由玩家主动解绑（Esc），释放 " + list.Count + " 个键。");
00177: 			}
00178: 			return list.Count > 0;
00179: 		}
00180: 		catch
00181: 		{
00182: 			return false;
00183: 		}
00184: 	}
00185: 
00186: 	internal static bool ForgetUid(long uid)
00187: 	{
00188: 		try
00189: 		{
00190: 			List<int> list = new List<int>();
00191: 			foreach (KeyValuePair<int, HotkeyEntry> boundKey in BoundKeys)
00192: 			{
00193: 				if (boundKey.Value.Uid == uid)
00194: 				{
00195: 					list.Add(boundKey.Key);
00196: 				}
00197: 			}
00198: 			foreach (int item in list)
00199: 			{
00200: 				BoundKeys.Remove(item);
00201: 			}
00202: 			if (list.Count > 0)
00203: 			{
00204: 				Core.Debug("快捷键映射：箱 uid " + uid + " 已离场，释放 " + list.Count + " 个单箱键。");
00205: 			}
00206: 			return list.Count > 0;
00207: 		}
00208: 		catch
00209: 		{
00210: 			return false;
00211: 		}
00212: 	}
00213: 
00214: 	private static void TickSingleBoxKeys()
00215: 	{
00216: 		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
00217: 		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
00218: 		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
00219: 		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
00220: 		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
00221: 		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
00222: 		if (BoundKeys.Count == 0)
00223: 		{
00224: 			if (_noBinding || Time.unscaledTime < _nextRestore)
00225: 			{
00226: 				return;
00227: 			}
00228: 			_nextRestore = Time.unscaledTime + 2f;
00229: 			RestoreFromBoxes();
00230: 			if (BoundKeys.Count == 0)
00231: 			{
00232: 				return;
00233: 			}
00234: 		}
00235: 		if (GearPanel.IsCapturing)
00236: 		{
00237: 			return;
00238: 		}
00239: 		if ((int)_suppressKey != 0)
00240: 		{
00241: 			try
00242: 			{
00243: 				if (Input.GetKey(_suppressKey))
00244: 				{
00245: 					return;
~~~

~~~csharp
00255: 			if (!Input.GetKeyDown((KeyCode)boundKey.Key))
00256: 			{
00257: 				continue;
00258: 			}
00259: 			string text = ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString();
00260: 			try
00261: 			{
00262: 				if (KeyBindHelper.SuppressHotkeys)
00263: 				{
00264: 					Core.Log?.Msg("快捷键 " + text + " 按下但被官方静默闸拦下（SuppressHotkeys=true，对话框/捕获期标记——若已无对话框仍被拦即官方标记卡住）。");
00265: 					break;
00266: 				}
00267: 			}
00268: 			catch
00269: 			{
00270: 			}
00271: 			try
00272: 			{
00273: 				if (StoreUIManager.IsTextFieldFocused())
00274: 				{
00275: 					Core.Log?.Msg("快捷键 " + text + " 按下但被输入框焦点闸拦下。");
00276: 					break;
00277: 				}
00278: 			}
00279: 			catch
00280: 			{
00281: 			}
00282: 			if (!BoxTransfer.IsInStore())
00283: 			{
00284: 				Core.Log?.Msg("快捷键 " + text + " 按下但判定为店外，忽略。");
00285: 				break;
00286: 			}
00287: 			GameItem val = ResolveBox(boundKey.Value.Uid);
00288: 			if (val == null || !BoxItem.IsBox(val))
00289: 			{
00290: 				Core.Log?.Msg("快捷键 " + text + " 命中但绑定箱失效（uid " + boundKey.Value.Uid + " 找不到活的收纳箱），本帧忽略。");
00291: 			}
00292: 			else if (boundKey.Value.Restock)
00293: 			{
00294: 				Core.Debug("收纳箱快捷键 " + ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString() + " 触发一键上货。");
00295: 				BoxItem.ClickSound();
00296: 				BoxRestock.RunOne(val, "快捷键");
00297: 			}
00298: 			else
00299: 			{
00300: 				Core.Debug("收纳箱快捷键 " + ((object)(KeyCode)boundKey.Key/*cast due to .constrained prefix*/).ToString() + " 触发一键收纳。");
00301: 				BoxItem.ClickSound();
00302: 				AutoStow.Run(val, "快捷键");
00303: 			}
00304: 			break;
00305: 		}
00306: 	}
00307: 
00308: 	private static GameItem? ResolveBox(long uid)
00309: 	{
00310: 		if (_lastHitUid == uid && _lastHitBox != null)
00311: 		{
00312: 			try
00313: 			{
00314: 				if (BoxItem.IsBox(_lastHitBox) && IsLiveBox(_lastHitBox))
00315: 				{
00316: 					return _lastHitBox;
00317: 				}
00318: 			}
00319: 			catch
00320: 			{
00321: 			}
00322: 			_lastHitBox = null;
00323: 		}
00324: 		foreach (GameItem item in BoxItem.FindAllBoxes())
00325: 		{
00326: 			try
00327: 			{
00328: 				if (item.uniqueId != uid || !IsLiveBox(item))
00329: 				{
00330: 					continue;
00331: 				}
00332: 				_lastHitBox = item;
00333: 				_lastHitUid = uid;
00334: 				return item;
00335: 			}
00336: 			catch
00337: 			{
00338: 			}
00339: 		}
00340: 		return null;
00341: 	}
00342: }
~~~

### ItemStorage/BoxWindow.cs

~~~csharp
00004: using System.Text;
00005: using HarmonyLib;
00006: using TMPro;
00007: using UnityEngine;
00008: using UnityEngine.EventSystems;
00009: using UnityEngine.UI;
00010: 
00011: namespace ItemStorage;
00012: 
00013: internal static class BoxWindow
00014: {
00015: 	private sealed class SlotClickCatcher : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
00016: 	{
00017: 		internal GameItem Pack;
00018: 
00019: 		internal int Slot;
00020: 
00021: 		internal Action? Refresh;
00022: 
00023: 		public void OnPointerClick(PointerEventData eventData)
00024: 		{
00025: 			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00026: 			if ((int)eventData.button != 0)
00027: 			{
00028: 				return;
00029: 			}
00030: 			try
00031: 			{
00032: 				if (SlotLock.Toggle(Pack, Slot).HasValue)
00033: 				{
00034: 					Refresh?.Invoke();
00035: 				}
00036: 			}
00037: 			catch (Exception ex)
00038: 			{
00039: 				Core.Log?.Warning("收纳箱 槽 " + Slot + " 锁定点击： " + ex.Message);
00040: 			}
00041: 		}
00042: 	}
00043: 
00044: 	private sealed class RetryState
00045: 	{
00046: 		internal float Time;
00047: 
00048: 		internal int Fails;
00049: 	}
00050: 
00051: 	private static readonly Dictionary<GameItem, Dictionary<string, AdvButtonElement>> ButtonsByRef = new Dictionary<GameItem, Dictionary<string, AdvButtonElement>>();
00052: 
00053: 	private static readonly Dictionary<string, Dictionary<string, AdvButtonElement>> ButtonsByUid = new Dictionary<string, Dictionary<string, AdvButtonElement>>();
00054: 
00055: 	private static readonly Dictionary<GameItem, Dictionary<string, MachineryLabel>> LabelsByRef = new Dictionary<GameItem, Dictionary<string, MachineryLabel>>();
00056: 
00057: 	private static readonly Dictionary<string, Dictionary<string, MachineryLabel>> LabelsByUid = new Dictionary<string, Dictionary<string, MachineryLabel>>();
00058: 
00059: 	private static readonly Dictionary<GameItem, Dictionary<string, Image>> IconsByRef = new Dictionary<GameItem, Dictionary<string, Image>>();
00060: 
00061: 	private static readonly Dictionary<string, Dictionary<string, Image>> IconsByUid = new Dictionary<string, Dictionary<string, Image>>();
00062: 
00063: 	private static readonly Dictionary<GameItem, Dictionary<string, GameObject>> ClicksByRef = new Dictionary<GameItem, Dictionary<string, GameObject>>();
00064: 
00065: 	private static readonly Dictionary<string, Dictionary<string, GameObject>> ClicksByUid = new Dictionary<string, Dictionary<string, GameObject>>();
00066: 
00067: 	private static readonly List<GameItem> TrackedBoxes = new List<GameItem>();
00068: 
00069: 	private static readonly Dictionary<string, Sprite?> SpriteCache = new Dictionary<string, Sprite>();
00070: 
00071: 	private static readonly List<GameItem> IconRefreshItems = new List<GameItem>();
00072: 
00073: 	private static readonly List<float> IconRefreshAt = new List<float>();
00074: 
00075: 	private static readonly List<int> IconRetryTried = new List<int>();
00076: 
00077: 	private const float IconRetryFirst = 0.6f;
00078: 
00079: 	private const float IconRetryInterval = 1.5f;
00080: 
00081: 	private const int IconRetryMax = 40;
00082: 
00083: 	private static bool _rebuilding;
00084: 
00085: 	private static Font? _gameFont;
00086: 
00087: 	private static readonly Color TitleColor = new Color(0.69803923f, 0.41960785f, 26f / 85f, 1f);
00088: 
00089: 	private static readonly FieldInfo? BtnTextColorField = AccessTools.Field(typeof(AdvButtonElement), "buttonTextColorNormal");
00090: 
00091: 	private static readonly Dictionary<string, Func<GameItem, List<ItemSpriteModifier>>?> SlotModDelegates = new Dictionary<string, Func<GameItem, List<ItemSpriteModifier>>>(StringComparer.Ordinal);
00092: 
00093: 	private static Sprite? _placeholder;
00094: 
00095: 	private const int ResolveMaxFails = 2;
00096: 
00097: 	private const float ResolveRetryGap = 60f;
00098: 
00099: 	private static readonly Dictionary<string, RetryState> WarmTried = new Dictionary<string, RetryState>(StringComparer.Ordinal);
00100: 
00101: 	private static readonly Dictionary<string, RetryState> ProbeTried = new Dictionary<string, RetryState>(StringComparer.Ordinal);
00102: 
00103: 	internal static bool EnsureWindow(GameItem pack, string context)
00104: 	{
00105: 		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
00106: 		//IL_017e: Expected O, but got Unknown
00107: 		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
00108: 		//IL_029d: Expected O, but got Unknown
00109: 		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
00110: 		//IL_01b0: Expected O, but got Unknown
00111: 		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
00112: 		//IL_08f7: Expected O, but got Unknown
00113: 		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
00114: 		//IL_023b: Expected O, but got Unknown
00115: 		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
00116: 		//IL_0772: Expected O, but got Unknown
00117: 		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
00118: 		//IL_083e: Expected O, but got Unknown
00119: 		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
00120: 		//IL_0352: Expected O, but got Unknown
00121: 		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
00122: 		//IL_06a6: Expected O, but got Unknown
00123: 		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
00124: 		//IL_0424: Expected O, but got Unknown
00125: 		if (pack == null)
00126: 		{
00127: 			return false;
00128: 		}
00129: 		if (!BoxItem.IsBox(pack))
00130: 		{
00131: 			return false;
00132: 		}
00133: 		try
00134: 		{
00135: 			ContainerHelper.AddContainerTooltipTag(pack);
00136: 		}
00137: 		catch
00138: 		{
00139: 		}
00140: 		GhostSweep.Run(context);
00141: 		try
00142: 		{
00143: 			bool flag = false;
00144: 			try
00145: 			{
00146: 				flag = pack.IsTag("ITEM_STORAGE_BOX_UI_V30");
00147: 			}
00148: 			catch
00149: 			{
00150: 			}
00151: 			Dictionary<string, AdvButtonElement> dictionary = FindIn(ButtonsByRef, ButtonsByUid, pack);
00152: 			if (pack.contentWindow != null && flag && dictionary != null)
00153: 			{
00154: 				TrackBox(pack);
00155: 				SyncHotkeyMap(pack);
00156: 				RefreshButtons(pack);
00157: 				return true;
00158: 			}
00159: 		}
00160: 		catch
00161: 		{
00162: 		}
00163: 		try
00164: 		{
00165: 			pack.SetContentWindow((PixelWindow)null);
00166: 		}
00167: 		catch
00168: 		{
00169: 		}
00170: 		string text = "收纳箱";
00171: 		if (string.IsNullOrEmpty(pack.name) || LooksLikeLocError(pack.name) || string.Equals(pack.name, "收纳盒", StringComparison.Ordinal))
00172: 		{
00173: 			try
00174: 			{
00175: 				pack.SetName(text);
00176: 			}
00177: 			catch
00178: 			{
00179: 			}
00180: 			pack.name = text;
00181: 		}
00182: 		try
00183: 		{
00184: 			if (string.Equals(Core.Clean(pack.spritePath), "item_storage", StringComparison.Ordinal))
00185: 			{
00186: 				pack.SetSprite(string.Empty, "Dream_Item_Storage");
00187: 				Core.Debug("收纳箱：旧贴图键 item_storage → Dream_Item_Storage");
00188: 			}
00189: 		}
00190: 		catch
00191: 		{
00192: 		}
00193: 		try
00194: 		{
00195: 			PixelWindow val = new PixelWindow(true, text, (WindowStyle)1);
00196: 			try
00197: 			{
00198: 				Sprite val2 = SpriteAssets.Get("Dream_Item_Storage_Gear");
00199: 				if ((Object)(object)val2 != (Object)null)
00200: 				{
00201: 					MachineryButton val3 = new MachineryButton(val2, MakeAction(delegate
00202: 					{
00203: 						try
00204: 						{
00205: 							GearPanel.ToggleFor(pack);
00206: 						}
00207: 						catch
00208: 						{
00209: 						}
00210: 					}), 16);
00211: 					val.AddHeaderButton(val3);
00212: 				}
00213: 				else
00214: 				{
00215: 					Core.Debug("收纳箱[" + context + "] 齿轮图标未加载，标题栏按钮跳过。");
00216: 				}
00217: 			}
00218: 			catch (Exception ex)
00219: 			{
00220: 				Core.Log?.Warning("收纳箱[" + context + "] 标题栏齿轮挂载失败：" + ex.Message);
00221: 			}
00222: 			try
00223: 			{
00224: 				Sprite val4 = SpriteAssets.Get("Dream_Item_Storage_Tidy");
00225: 				if ((Object)(object)val4 != (Object)null)
00226: 				{
00227: 					MachineryButton val5 = new MachineryButton(val4, MakeAction(delegate
00228: 					{
00229: 						try
00230: 						{
00231: 							int num4 = BoxTidy.Tidy(pack);
00232: 							if (num4 > 0)
00233: 							{
00234: 								RefreshButtons(pack);
00235: 								BoxItem.NotifyUser("已整理：挪动 " + num4 + " 件");
00236: 							}
00237: 							else
00238: 							{
00239: 								BoxItem.NotifyUser("目前顺序已经无法整理了");
00240: 							}
00241: 						}
00242: 						catch (Exception ex9)
00243: 						{
00244: 							Core.Log?.Warning("收纳箱[" + context + "] 一键整理失败：" + ex9.Message);
00245: 						}
00246: 					}), 16);
00247: 					val.AddHeaderButton(val5);
00248: 				}
00249: 				else
00250: 				{
00251: 					Core.Debug("收纳箱[" + context + "] 整理图标未加载，标题栏按钮跳过。");
00252: 				}
00253: 			}
00254: 			catch (Exception ex2)
00255: 			{
00256: 				Core.Log?.Warning("收纳箱[" + context + "] 标题栏整理按钮挂载失败：" + ex2.Message);
00257: 			}
00258: 			GridPixelElement val6 = new GridPixelElement(4, 5, false);
00259: 			val6.SetSpacing(8, 3);
00260: 			Dictionary<string, AdvButtonElement> dictionary2 = new Dictionary<string, AdvButtonElement>(StringComparer.Ordinal);
00261: 			Dictionary<string, MachineryLabel> dictionary3 = new Dictionary<string, MachineryLabel>(StringComparer.Ordinal);
00262: 			Dictionary<string, Image> dictionary4 = new Dictionary<string, Image>(StringComparer.Ordinal);
00263: 			Dictionary<string, GameObject> dictionary5 = new Dictionary<string, GameObject>(StringComparer.Ordinal);
00264: 			for (int num = 0; num < 16; num++)
00265: 			{
00266: 				int num2 = num % 4;
00267: 				int num3 = num / 4;
00268: 				int slotIndex = num;
00269: 				try
00270: 				{
00271: 					Action click = MakeAction(delegate
00272: 					{
00273: 						try
00274: 						{
00275: 							if (StoragePatch.TryRegisterClickDeposit(pack, slotIndex) == false)
00276: 							{
00277: 								GameGridInventory val19 = (BoxTransfer.AltHeld() ? BoxTransfer.ResolveCounterDrop() : null);
00278: 								BoxTransfer.WithdrawResult withdrawResult = BoxTransfer.TryWithdraw(pack, slotIndex, val19);
00279: 								if (withdrawResult == BoxTransfer.WithdrawResult.NoSpace || withdrawResult == BoxTransfer.WithdrawResult.NotOnGrid)
00280: 								{
00281: 									BoxItem.NotifyUser(BoxTransfer.SpaceShort(val19 != null) + "，无法取出物品");
00282: 								}
00283: 							}
00284: 						}
00285: 						catch
00286: 						{
00287: 						}
00288: 					});
00289: 					TooltipSlotButton tooltipSlotButton = new TooltipSlotButton(pack, slotIndex, click);
00290: 					dictionary2[slotIndex.ToString()] = (AdvButtonElement)(object)tooltipSlotButton;
00291: 					GridPixelElement val7 = new GridPixelElement(1, 2, false);
00292: 					val7.SetSpacing(2, 2);
00293: 					PixelElement val8 = (PixelElement)(object)tooltipSlotButton;
00294: 					if (val8 == null)
00295: 					{
00296: 						Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slotIndex + " 按钮非 PixelElement");
00297: 					}
00298: 					else if (!val7.Attach(val8, 0, 0, 48, 48))
00299: 					{
00300: 						Core.Log?.Warning("收纳箱[" + context + "] attach 按钮 " + slotIndex + " 失败");
00301: 					}
00302: 					MachineryLabel val9 = new MachineryLabel((string)null);
00303: 					val9.SetText(SlotText(pack, slotIndex));
00304: 					val9.SetWidth(48);
00305: 					dictionary3[slotIndex.ToString()] = val9;
00306: 					PixelElement val10 = (PixelElement)(object)val9;
00307: 					if (val10 != null && !val7.Attach(val10, 0, 1, 48, 8))
00308: 					{
00309: 						Core.Log?.Warning("收纳箱[" + context + "] attach 标签 " + slotIndex + " 失败");
00310: 					}
00311: 					GameObject val11 = CreateSlotClickLayer(val9, pack, slotIndex, context);
00312: 					if ((Object)(object)val11 != (Object)null)
00313: 					{
00314: 						dictionary5[slotIndex.ToString()] = val11;
00315: 					}
00316: 					Image val12 = CreateSlotIcon((AdvButtonElement)(object)tooltipSlotButton, context, slotIndex);
00317: 					if ((Object)(object)val12 != (Object)null)
00318: 					{
00319: 						dictionary4[slotIndex.ToString()] = val12;
00320: 					}
00321: 					PixelElement val13 = (PixelElement)(object)val7;
00322: 					if (val13 == null)
00323: 					{
00324: 						Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slotIndex + " 非 PixelElement");
00325: 					}
00326: 					else if (!val6.Attach(val13, num2, num3, 48, 58))
00327: 					{
00328: 						Core.Log?.Warning("收纳箱[" + context + "] attach 槽位 " + slotIndex + " 失败");
00329: 					}
00330: 				}
00331: 				catch (Exception ex3)
00332: 				{
00333: 					Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slotIndex + " 构建失败：" + ex3.Message);
00334: 				}
00335: 			}
00336: 			try
00337: 			{
00338: 				Action action = MakeAction(delegate
00339: 				{
00340: 					try
00341: 					{
00342: 						AutoStow.Run(pack, "按钮");
00343: 					}
00344: 					catch
00345: 					{
00346: 					}
00347: 				});
00348: 				PixelElement val14 = (PixelElement)new MachineryButton("一键收纳", action, 48, 24);
00349: 				if (val14 == null)
00350: 				{
00351: 					Core.Log?.Warning("收纳箱[" + context + "] 一键收纳按钮非 PixelElement");
00352: 				}
00353: 				else if (!val6.Attach(val14, 3, 4, 48, 24))
00354: 				{
00355: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键收纳按钮失败");
00356: 				}
00357: 			}
00358: 			catch (Exception ex4)
00359: 			{
00360: 				Core.Log?.Warning("收纳箱[" + context + "] 一键收纳按钮构建失败：" + ex4.Message);
00361: 			}
00362: 			try
00363: 			{
00364: 				Action action2 = MakeAction(delegate
00365: 				{
00366: 					try
00367: 					{
00368: 						BoxStowAll.RunAll("按钮");
00369: 					}
00370: 					catch
00371: 					{
00372: 					}
00373: 				});
00374: 				PixelElement val15 = (PixelElement)new MachineryButton("全箱收纳", action2, 48, 24);
00375: 				if (val15 == null)
00376: 				{
00377: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮非 PixelElement");
00378: 				}
00379: 				else if (!val6.Attach(val15, 0, 4, 48, 24))
00380: 				{
00381: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱收纳按钮失败");
00382: 				}
00383: 			}
00384: 			catch (Exception ex5)
00385: 			{
00386: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱收纳按钮构建失败：" + ex5.Message);
00387: 			}
00388: 			try
00389: 			{
00390: 				Action action3 = MakeAction(delegate
00391: 				{
00392: 					try
00393: 					{
00394: 						BoxRestock.RunAllBoxes("按钮");
00395: 					}
00396: 					catch
00397: 					{
00398: 					}
00399: 				});
00400: 				PixelElement val16 = (PixelElement)new MachineryButton("全箱上货", action3, 48, 24);
00401: 				if (val16 == null)
00402: 				{
00403: 					Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮非 PixelElement");
00404: 				}
00405: 				else if (!val6.Attach(val16, 1, 4, 48, 24))
00406: 				{
00407: 					Core.Log?.Warning("收纳箱[" + context + "] attach 全箱上货按钮失败");
00408: 				}
00409: 			}
00410: 			catch (Exception ex6)
00411: 			{
00412: 				Core.Log?.Warning("收纳箱[" + context + "] 全箱上货按钮构建失败：" + ex6.Message);
00413: 			}
00414: 			try
00415: 			{
00416: 				Action action4 = MakeAction(delegate
00417: 				{
00418: 					try
00419: 					{
00420: 						BoxRestock.RunOne(pack, "按钮");
00421: 					}
00422: 					catch
00423: 					{
00424: 					}
00425: 				});
00426: 				PixelElement val17 = (PixelElement)new MachineryButton("一键上货", action4, 48, 24);
00427: 				if (val17 == null)
00428: 				{
00429: 					Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮非 PixelElement");
00430: 				}
00431: 				else if (!val6.Attach(val17, 2, 4, 48, 24))
00432: 				{
00433: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键上货按钮失败");
00434: 				}
00435: 			}
00436: 			catch (Exception ex7)
00437: 			{
00438: 				Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮构建失败：" + ex7.Message);
00439: 			}
00440: 			SyncHotkeyMap(pack);
00441: 			StoreIn(ButtonsByRef, ButtonsByUid, pack, dictionary2);
00442: 			StoreIn(LabelsByRef, LabelsByUid, pack, dictionary3);
00443: 			StoreIn(IconsByRef, IconsByUid, pack, dictionary4);
00444: 			StoreIn(ClicksByRef, ClicksByUid, pack, dictionary5);
00445: 			TrackBox(pack);
00446: 			PixelElement val18 = (PixelElement)(object)val6;
00447: 			if (val18 == null)
00448: 			{
00449: 				Core.Log?.Warning("收纳箱[" + context + "]：carrier 不是 PixelElement");
00450: 				return false;
00451: 			}
00452: 			val.Attach(val18);
00453: 			pack.SetContentWindow(val);
00454: 			try
00455: 			{
00456: 				pack.EnableTag("ITEM_STORAGE_BOX_UI_V30", false);
00457: 			}
00458: 			catch
00459: 			{
00460: 			}
00461: 			try
00462: 			{
00463: 				val.Validate();
00464: 			}
00465: 			catch
00466: 			{
00467: 			}
00468: 			RefreshButtons(pack);
00469: 			QueueIconRefresh(pack);
00470: 			Core.Debug("收纳箱[" + context + "]：窗口构建完成（4×4 十六槽全满 + 网格下方横排一键收纳按钮 48×24）");
00471: 			return true;
00472: 		}
00473: 		catch (Exception ex8)
00474: 		{
00475: 			Core.Log?.Warning("收纳箱[" + context + "] EnsureWindow：" + ex8.GetType().Name + ": " + ex8.Message);
00476: 			return false;
00477: 		}
00478: 	}
00479: 
00480: 	internal static void RefreshButtons(GameItem pack)
00481: 	{
00482: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
00483: 		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
00484: 		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
00485: 		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
00486: 		if (pack == null)
00487: 		{
00488: 			return;
00489: 		}
00490: 		try
00491: 		{
00492: 			Dictionary<string, AdvButtonElement> dictionary = FindIn(ButtonsByRef, ButtonsByUid, pack);
00493: 			if (dictionary == null)
00494: 			{
00495: 				if (!_rebuilding)
00496: 				{
00497: 					Core.Debug("收纳箱 RefreshButtons：无按钮引用，重建窗口");
00498: 					ForceRebuildWindow(pack, "RefreshMiss");
00499: 				}
00500: 				return;
00501: 			}
00502: 			StoreIn(ButtonsByRef, ButtonsByUid, pack, dictionary);
00503: 			Dictionary<string, MachineryLabel> dictionary2 = FindIn(LabelsByRef, LabelsByUid, pack);
00504: 			Dictionary<string, Image> dictionary3 = FindIn(IconsByRef, IconsByUid, pack);
00505: 			for (int i = 0; i < 16; i++)
00506: 			{
00507: 				if (dictionary2 != null && dictionary2.TryGetValue(i.ToString(), out var value) && value != null)
00508: 				{
00509: 					try
00510: 					{
00511: 						value.SetText(SlotText(pack, i));
00512: 						try
00513: 						{
00514: 							bool flag = SlotLock.IsLocked(pack, i);
00515: 							value.SetTextColor(flag ? SlotLock.LockTextColor() : SlotLock.LabelColor());
00516: 							try
00517: 							{
00518: 								value.SetPlateColor(flag ? SlotLock.LockPlateColor() : SlotLock.PlateColor());
00519: 							}
00520: 							catch
00521: 							{
00522: 							}
00523: 						}
00524: 						catch
00525: 						{
00526: 						}
00527: 					}
00528: 					catch
00529: 					{
00530: 					}
00531: 				}
00532: 				if (dictionary3 != null && dictionary3.TryGetValue(i.ToString(), out var value2) && (Object)(object)value2 != (Object)null)
00533: 				{
00534: 					UpdateSlotIcon(pack, i, value2);
00535: 				}
00536: 			}
00537: 			UpdateTooltip(pack);
00538: 		}
00539: 		catch (Exception ex)
00540: 		{
00541: 			Core.Log?.Warning("收纳箱 RefreshButtons：" + ex.Message);
00542: 		}
00543: 	}
00544: 
00545: 	internal static void ForceRebuildWindow(GameItem pack, string reason)
00546: 	{
00547: 		if (pack == null || _rebuilding)
00548: 		{
00549: 			return;
00550: 		}
00551: 		_rebuilding = true;
00552: 		try
00553: 		{
00554: 			ForgetIn(ButtonsByRef, ButtonsByUid, pack);
00555: 			ForgetIn(LabelsByRef, LabelsByUid, pack);
00556: 			ForgetIn(IconsByRef, IconsByUid, pack);
00557: 			ForgetIn(ClicksByRef, ClicksByUid, pack);
00558: 			UntrackBox(pack);
00559: 			GearPanel.CloseFor(pack);
00560: 			try
00561: 			{
00562: 				pack.SetContentWindow((PixelWindow)null);
00563: 			}
00564: 			catch
00565: 			{
00566: 			}
00567: 			try
00568: 			{
00569: 				pack.DisableTag("ITEM_STORAGE_BOX_UI_V30", true);
00570: 			}
00571: 			catch
00572: 			{
00573: 				try
00574: 				{
00575: 					pack.DisableTag("ITEM_STORAGE_BOX_UI_V30", false);
00576: 				}
00577: 				catch
00578: 				{
00579: 				}
00580: 			}
00581: 			EnsureWindow(pack, reason);
00582: 		}
00583: 		finally
00584: 		{
00585: 			_rebuilding = false;
00586: 		}
00587: 	}
00588: 
00589: 	internal static string SlotText(GameItem pack, int slot)
00590: 	{
00591: 		string itemId = StorageGrid.GetItemId(pack, slot);
00592: 		if (itemId == null)
00593: 		{
00594: 			return "空";
00595: 		}
00596: 		if (StorageGrid.IsWaterSlot(pack, slot) && StorageGrid.GetStoredWaterML(pack, slot) > 0)
00597: 		{
00598: 			string text = BoxTransfer.WaterConditionName(StorageGrid.GetStoredWaterPurity(pack, slot));
00599: 			if (!string.IsNullOrEmpty(text))
00600: 			{
00601: 				return text + "×" + StorageGrid.GetCount(pack, slot);
00602: 			}
00603: 		}
00604: 		string purity = StorageGrid.GetPurity(pack, slot);
00605: 		if (NullGuard.HasText(purity))
00606: 		{
00607: 			string text2 = BoxTransfer.PurityShortName(purity, 4);
00608: 			if (!string.IsNullOrEmpty(text2))
00609: 			{
00610: 				return text2 + "×" + StorageGrid.GetCount(pack, slot);
00611: 			}
00612: 		}
00613: 		string text3 = StorageGrid.GetDisplayName(pack, slot);
00614: 		if (!NullGuard.HasText(text3))
00615: 		{
00616: 			text3 = DisplayNameForItem(itemId, 4);
00617: 		}
00618: 		else if (text3.Length > 4)
00619: 		{
00620: 			text3 = text3.Substring(0, 4);
00621: 		}
00622: 		return text3 + "×" + StorageGrid.GetCount(pack, slot);
00623: 	}
00624: 
00625: 	internal static string DisplayNameForItem(string itemId, int maxChars = 3)
00626: 	{
00627: 		string text = itemId;
00628: 		try
00629: 		{
00630: 			string localizedItem = LocHelper.GetLocalizedItem("item_" + itemId + "_name", Array.Empty<object>());
00631: 			if (!string.IsNullOrEmpty(localizedItem) && !localizedItem.StartsWith("Translation Error", StringComparison.Ordinal) && !localizedItem.Contains("item_" + itemId + "_name"))
00632: 			{
00633: 				text = localizedItem;
00634: 			}
00635: 		}
00636: 		catch
00637: 		{
00638: 		}
00639: 		if ((object)text == itemId)
00640: 		{
00641: 			try
00642: 			{
00643: 				GameItem val = DirectoryMaster.Item(itemId, false);
00644: 				string text2 = ((val != null) ? val.GetDisplayName(true) : null);
00645: 				if (string.IsNullOrWhiteSpace(text2))
00646: 				{
00647: 					text2 = ((val != null) ? val.name : null);
00648: 				}
00649: 				if (!string.IsNullOrWhiteSpace(text2))
00650: 				{
00651: 					text = text2;
00652: 				}
00653: 			}
00654: 			catch
00655: 			{
00656: 			}
00657: 		}
00658: 		text = Core.Clean(text);
00659: 		if (text.Length > maxChars)
00660: 		{
00661: 			text = text.Substring(0, maxChars);
00662: 		}
00663: 		return text;
00664: 	}
00665: 
00666: 	internal static void UpdateTooltip(GameItem pack)
00667: 	{
00668: 		if (pack == null)
00669: 		{
00670: 			return;
00671: 		}
00672: 		try
00673: 		{
00674: 			try
00675: 			{
00676: 				pack.shortDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
00677: 			}
00678: 			catch (Exception ex)
00679: 			{
00680: 				Core.Log?.Warning("收纳箱 shortDescription 写入：" + ex.Message);
00681: 			}
00682: 			try
00683: 			{
00684: 				string shortDescription = pack.shortDescription;
00685: 				if (!string.Equals(shortDescription, "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。", StringComparison.Ordinal))
00686: 				{
00687: 					Core.Debug("收纳箱 shortDescription 读回不一致：'" + Core.Clean(shortDescription) + "'");
00688: 				}
00689: 			}
00690: 			catch (Exception ex2)
00691: 			{
00692: 				Core.Log?.Warning("收纳箱 shortDescription 读回失败（getter 缺失？）：" + ex2.Message);
00693: 			}
00694: 			try
00695: 			{
00696: 				pack.longDescription = "拖动物品到收纳箱上或拖入格子中可以存入物品，左键取出1件，右键取出5件；含水容器只能通过拖入格子放入，一键收纳不会收纳含水容器，同规格不同水质的水瓶可以混合存放，请注意避免不希望发生的水质混合。";
00697: 			}
00698: 			catch
00699: 			{
00700: 			}
00701: 			pack.flavorText = string.Empty;
00702: 		}
00703: 		catch (Exception ex3)
00704: 		{
00705: 			Core.Log?.Warning("收纳箱 UpdateTooltip：" + ex3.Message);
00706: 		}
00707: 	}
00708: 
00709: 	private static Action MakeAction(Action action)
00710: 	{
00711: 		return action;
00712: 	}
00713: 
00714: 	internal static Font? GameFont()
00715: 	{
00716: 		if ((Object)(object)_gameFont != (Object)null)
00717: 		{
00718: 			try
00719: 			{
00720: 				if ((Object)(object)_gameFont != (Object)null)
00721: 				{
00722: 					return _gameFont;
00723: 				}
00724: 			}
00725: 			catch
00726: 			{
00727: 			}
00728: 			_gameFont = null;
00729: 		}
00730: 		try
00731: 		{
00732: 			Text[] array = Object.FindObjectsOfType<Text>();
00733: 			Text[] array2 = array;
00734: 			foreach (Text val in array2)
00735: 			{
00736: 				try
00737: 				{
00738: 					if ((Object)(object)val != (Object)null && (Object)(object)val.font != (Object)null && (Object)(object)val.font != (Object)null)
00739: 					{
00740: 						_gameFont = val.font;
00741: 						Core.Debug("收纳箱：按钮字体已抓取（来源=场景 Text，扫到 " + array.Length + " 个）。");
00742: 						return _gameFont;
00743: 					}
00744: 				}
00745: 				catch
00746: 				{
00747: 				}
00748: 			}
00749: 		}
00750: 		catch
00751: 		{
00752: 		}
00753: 		try
00754: 		{
00755: 			Font[] array3 = Resources.FindObjectsOfTypeAll<Font>();
00756: 			Font[] array4 = array3;
00757: 			foreach (Font val2 in array4)
00758: 			{
00759: 				try
00760: 				{
00761: 					if (!((Object)(object)val2 == (Object)null) && !((Object)(object)val2 == (Object)null))
00762: 					{
00763: 						string text = ((Object)val2).name ?? string.Empty;
00764: 						if (text.Length != 0 && text.IndexOf("Arial", StringComparison.OrdinalIgnoreCase) < 0 && text.IndexOf("LegacyRuntime", StringComparison.OrdinalIgnoreCase) < 0)
00765: 						{
00766: 							_gameFont = val2;
00767: 							Core.Debug("收纳箱：按钮字体已抓取（来源=内存 Font：" + text + "，共 " + array3.Length + " 个）。");
00768: 							return _gameFont;
00769: 						}
00770: 					}
00771: 				}
00772: 				catch
00773: 				{
00774: 				}
00775: 			}
00776: 		}
00777: 		catch
00778: 		{
00779: 		}
00780: 		Core.Debug("收纳箱：游戏字体抓取失败（场景 Text 与内存 Font 均未拿到）。");
00781: 		return null;
00782: 	}
00783: 
00784: 	private static Color ButtonTextColor(AdvButtonElement? button)
00785: 	{
00786: 		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
00787: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
00788: 		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
00789: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00790: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
00791: 		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
00792: 		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
00793: 		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
00794: 		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
00795: 		try
00796: 		{
00797: 			if (button != null && BtnTextColorField != null && BtnTextColorField.GetValue(button) is Color32 val)
00798: 			{
00799: 				return new Color((float)(int)val.r / 255f, (float)(int)val.g / 255f, (float)(int)val.b / 255f, 1f);
00800: 			}
00801: 		}
00802: 		catch
00803: 		{
00804: 		}
00805: 		return TitleColor;
00806: 	}
00807: 
00808: 	private unsafe static Text? AttachButtonLabel(AdvButtonElement button, string text, string context)
00809: 	{
00810: 		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
00811: 		//IL_004a: Expected O, but got Unknown
00812: 		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
00813: 		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
00814: 		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
00815: 		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
00816: 		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
00817: 		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
00818: 		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
00819: 		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
00820: 		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
00821: 		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
00822: 		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
00823: 		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
00824: 		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
00825: 		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
00826: 		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
00827: 		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
00828: 		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
00829: 		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
00830: 		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
00831: 		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
00832: 		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
00833: 		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
00834: 		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
00835: 		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
00836: 		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
00837: 		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
00838: 		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
00839: 		try
00840: 		{
00841: 			object obj;
00842: 			if (button == null)
00843: 			{
00844: 				obj = null;
00845: 			}
00846: 			else
00847: 			{
00848: 				TreeNodeRender handler = ((SelectableElement)button).handler;
00849: 				obj = ((handler != null) ? handler.rectTransform : null);
00850: 			}
00851: 			RectTransform val = (RectTransform)obj;
00852: 			if ((Object)(object)val == (Object)null)
00853: 			{
00854: 				Core.Debug("收纳箱[" + context + "] 按钮文字：rectTransform=null");
00855: 				return null;
00856: 			}
00857: 			GameObject val2 = new GameObject("item_storage_stow_label");
00858: 			val2.transform.SetParent((Transform)(object)val, false);
00859: 			Transform transform = val2.transform;
00860: 			RectTransform val3 = (RectTransform)(((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>()));
00861: 			Rect rect = val.rect;
00862: 			float num = ((Rect)(ref rect)).width;
00863: 			if (num < 1f)
00864: 			{
00865: 				num = 48f;
00866: 			}
00867: 			rect = val.rect;
00868: 			float num2 = ((Rect)(ref rect)).height;
00869: 			if (num2 < 1f)
00870: 			{
00871: 				num2 = 24f;
00872: 			}
00873: 			val3.anchorMin = new Vector2(0f, 1f);
00874: 			val3.anchorMax = new Vector2(0f, 1f);
00875: 			val3.pivot = new Vector2(0.5f, 0.5f);
00876: 			val3.anchoredPosition = new Vector2(num / 2f, (0f - num2) / 2f);
00877: 			val3.sizeDelta = new Vector2(num, num2);
00878: 			((Transform)val3).localScale = Vector3.one;
00879: 			((Transform)val3).SetAsLastSibling();
00880: 			Text val4 = val2.AddComponent<Text>();
00881: 			val4.font = _gameFont;
00882: 			val4.fontSize = 10;
00883: 			val4.text = text;
00884: 			val4.alignment = (TextAnchor)4;
00885: 			val4.horizontalOverflow = (HorizontalWrapMode)1;
00886: 			val4.verticalOverflow = (VerticalWrapMode)1;
00887: 			((Graphic)val4).color = ButtonTextColor(button);
00888: 			((Graphic)val4).raycastTarget = false;
00889: 			val2.SetActive(true);
00890: 			try
00891: 			{
00892: 				string[] obj2 = new string[18]
00893: 				{
00894: 					"收纳箱[", context, "] 按钮文字诊断：rt.rect=", null, null, null, null, null, null, null,
00895: 					null, null, null, null, null, null, null, null
00896: 				};
00897: 				rect = val.rect;
00898: 				obj2[3] = ((object)(*(Rect*)(&rect))/*cast due to .constrained prefix*/).ToString();
00899: 				obj2[4] = " sizeDelta=";
00900: 				obj2[5] = ((object)val.sizeDelta/*cast due to .constrained prefix*/).ToString();
00901: 				obj2[6] = " pos=";
00902: 				obj2[7] = ((object)val.anchoredPosition/*cast due to .constrained prefix*/).ToString();
00903: 				obj2[8] = " pivot=";
00904: 				obj2[9] = ((object)val.pivot/*cast due to .constrained prefix*/).ToString();
00905: 				obj2[10] = " anchor=";
00906: 				obj2[11] = ((object)val.anchorMin/*cast due to .constrained prefix*/).ToString();
00907: 				obj2[12] = "~";
00908: 				obj2[13] = ((object)val.anchorMax/*cast due to .constrained prefix*/).ToString();
00909: 				obj2[14] = " → 文字 pos=";
00910: 				obj2[15] = ((object)val3.anchoredPosition/*cast due to .constrained prefix*/).ToString();
00911: 				obj2[16] = " size=";
00912: 				obj2[17] = ((object)val3.sizeDelta/*cast due to .constrained prefix*/).ToString();
00913: 				Core.Debug(string.Concat(obj2));
00914: 			}
00915: 			catch
00916: 			{
00917: 			}
00918: 			return val4;
00919: 		}
00920: 		catch (Exception ex)
00921: 		{
00922: 			Core.Log?.Warning("收纳箱[" + context + "] 按钮文字叠层：" + ex.Message);
00923: 			return null;
00924: 		}
00925: 	}
00926: 
00927: 	private static GameObject? CreateSlotClickLayer(MachineryLabel label, GameItem pack, int slot, string context)
00928: 	{
00929: 		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
00930: 		//IL_008d: Expected O, but got Unknown
00931: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
00932: 		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
00933: 		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
00934: 		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
00935: 		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
00936: 		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
00937: 		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
00938: 		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
00939: 		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
00940: 		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
00941: 		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
00942: 		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
00943: 		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
00944: 		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
00945: 		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
00946: 		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
00947: 		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
00948: 		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
00949: 		try
00950: 		{
00951: 			TreeNodeRender handler = label.handler;
00952: 			GameObject val = ((handler != null) ? ((Component)handler).gameObject : null);
00953: 			if ((Object)(object)val == (Object)null)
00954: 			{
00955: 				Core.Debug("收纳箱[" + context + "] 槽 " + slot + " 点击层：标签 handler 为空");
00956: 				return null;
00957: 			}
00958: 			GameObject val2 = new GameObject("item_storage_slot_click_" + slot);
00959: 			val2.transform.SetParent(val.transform, false);
00960: 			Transform transform = val2.transform;
00961: 			RectTransform val3 = (RectTransform)(((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>()));
00962: 			((Transform)val3).localScale = Vector3.one;
00963: 			string text = "text";
00964: 			TMP_Text componentInChildren = val.GetComponentInChildren<TMP_Text>(true);
00965: 			RectTransform val4 = ((componentInChildren != null) ? componentInChildren.rectTransform : null);
00966: 			if ((Object)(object)val4 == (Object)null && val.transform.childCount > 0)
00967: 			{
00968: 				Transform child = val.transform.GetChild(0);
00969: 				val4 = (RectTransform)(object)((child is RectTransform) ? child : null);
00970: 				text = "plate";
00971: 			}
00972: 			if ((Object)(object)val4 != (Object)null)
00973: 			{
00974: 				val3.anchorMin = val4.anchorMin;
00975: 				val3.anchorMax = val4.anchorMax;
00976: 				val3.pivot = val4.pivot;
00977: 				val3.anchoredPosition = val4.anchoredPosition;
00978: 				val3.sizeDelta = val4.sizeDelta + new Vector2(4f, 2f);
00979: 			}
00980: 			else
00981: 			{
00982: 				text = "fallback";
00983: 				val3.anchorMin = new Vector2(0f, 1f);
00984: 				val3.anchorMax = new Vector2(0f, 1f);
00985: 				val3.pivot = new Vector2(0f, 1f);
00986: 				val3.anchoredPosition = Vector2.zero;
00987: 				val3.sizeDelta = new Vector2(48f, 8f);
00988: 			}
00989: 			Image obj = val2.AddComponent<Image>();
00990: 			((Graphic)obj).color = new Color(1f, 1f, 1f, 0f);
00991: 			((Graphic)obj).raycastTarget = true;
00992: 			SlotClickCatcher slotClickCatcher = val2.AddComponent<SlotClickCatcher>();
00993: 			slotClickCatcher.Pack = pack;
00994: 			slotClickCatcher.Slot = slot;
00995: 			slotClickCatcher.Refresh = delegate
00996: 			{
00997: 				RefreshButtons(pack);
00998: 				Core.Log?.Msg("收纳箱 锁定 UI 已刷新：槽 " + slot + " locked=" + SlotLock.IsLocked(pack, slot));
00999: 			};
01000: 			val2.SetActive(true);
01001: 			Core.LogSink log = Core.Log;
01002: 			if (log != null)
01003: 			{
01004: 				string[] obj2 = new string[11]
01005: 				{
01006: 					"收纳箱[",
01007: 					context,
01008: 					"] 槽 ",
01009: 					slot.ToString(),
01010: 					" 点击层 v3 已构建（rect=",
01011: 					null,
01012: 					null,
01013: 					null,
01014: 					null,
01015: 					null,
01016: 					null
01017: 				};
01018: 				Rect rect = val3.rect;
01019: 				obj2[5] = ((Rect)(ref rect)).width.ToString("0.#");
01020: 				obj2[6] = "x";
01021: 				rect = val3.rect;
01022: 				obj2[7] = ((Rect)(ref rect)).height.ToString("0.#");
01023: 				obj2[8] = "，来源=";
01024: 				obj2[9] = text;
01025: 				obj2[10] = "）。";
01026: 				log.Msg(string.Concat(obj2));
01027: 			}
01028: 			return val2;
01029: 		}
01030: 		catch (Exception ex)
01031: 		{
01032: 			Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slot + " 点击层构建：" + ex.Message);
01033: 			return null;
01034: 		}
01035: 	}
01036: 
01037: 	private static Image? CreateSlotIcon(AdvButtonElement button, string context, int slot)
01038: 	{
01039: 		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
01040: 		//IL_0077: Expected O, but got Unknown
01041: 		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
01042: 		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
01043: 		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
01044: 		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
01045: 		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
01046: 		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
01047: 		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
01048: 		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
01049: 		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
01050: 		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
01051: 		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
01052: 		//IL_0186: Expected O, but got Unknown
01053: 		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
01054: 		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
01055: 		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
01056: 		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
01057: 		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
01058: 		try
01059: 		{
01060: 			object obj;
01061: 			if (button == null)
01062: 			{
01063: 				obj = null;
01064: 			}
01065: 			else
01066: 			{
01067: 				TreeNodeRender handler = ((SelectableElement)button).handler;
01068: 				obj = ((handler != null) ? handler.rectTransform : null);
01069: 			}
01070: 			RectTransform val = (RectTransform)obj;
01071: 			if ((Object)(object)val == (Object)null)
01072: 			{
01073: 				Core.Debug("收纳箱[" + context + "] 槽 " + slot + " 图标层：rectTransform=null");
01074: 				return null;
01075: 			}
01076: 			GameObject val2 = new GameObject("item_storage_slot_icon_" + slot);
01077: 			val2.transform.SetParent((Transform)(object)val, false);
01078: 			Transform transform = val2.transform;
01079: 			object obj2 = ((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>());
01080: 			Rect rect = val.rect;
01081: 			float num = ((Rect)(ref rect)).width;
01082: 			if (num < 1f)
01083: 			{
01084: 				num = 48f;
01085: 			}
01086: 			rect = val.rect;
01087: 			float num2 = ((Rect)(ref rect)).height;
01088: 			if (num2 < 1f)
01089: 			{
01090: 				num2 = 48f;
01091: 			}
01092: 			((RectTransform)obj2).anchorMin = new Vector2(0f, 1f);
01093: 			((RectTransform)obj2).anchorMax = new Vector2(0f, 1f);
01094: 			((RectTransform)obj2).pivot = new Vector2(0.5f, 0.5f);
01095: 			((RectTransform)obj2).anchoredPosition = new Vector2(num / 2f, (0f - num2) / 2f);
01096: 			((RectTransform)obj2).sizeDelta = new Vector2(40f, 40f);
01097: 			((Transform)obj2).localScale = Vector3.one;
01098: 			((Transform)obj2).SetAsLastSibling();
01099: 			Image obj3 = val2.AddComponent<Image>();
01100: 			obj3.type = (Type)0;
01101: 			obj3.preserveAspect = true;
01102: 			((Graphic)obj3).raycastTarget = false;
01103: 			GameObject val3 = new GameObject("item_storage_slot_liquid_" + slot);
01104: 			val3.transform.SetParent(val2.transform, false);
01105: 			Transform transform2 = val3.transform;
01106: 			object obj4 = ((object)((transform2 is RectTransform) ? transform2 : null)) ?? ((object)val3.AddComponent<RectTransform>());
01107: 			((RectTransform)obj4).anchorMin = Vector2.zero;
01108: 			((RectTransform)obj4).anchorMax = Vector2.one;
01109: 			((RectTransform)obj4).offsetMin = Vector2.zero;
01110: 			((RectTransform)obj4).offsetMax = Vector2.zero;
01111: 			Image obj5 = val3.AddComponent<Image>();
01112: 			obj5.type = (Type)3;
01113: 			obj5.fillMethod = (FillMethod)1;
01114: 			obj5.fillOrigin = 0;
01115: 			obj5.fillAmount = 0f;
01116: 			obj5.preserveAspect = true;
01117: 			((Graphic)obj5).color = new Color(1f, 1f, 1f, 0.7f);
01118: 			((Graphic)obj5).raycastTarget = false;
01119: 			val3.SetActive(false);
01120: 			val2.SetActive(false);
01121: 			return obj3;
01122: 		}
01123: 		catch (Exception ex)
01124: 		{
01125: 			Core.Log?.Warning("收纳箱[" + context + "] 槽 " + slot + " 图标层构建：" + ex.Message);
01126: 			return null;
01127: 		}
01128: 	}
01129: 
01130: 	private static void UpdateSlotIcon(GameItem pack, int slot, Image icon)
01131: 	{
01132: 		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
01133: 		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
01134: 		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
01135: 		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
01136: 		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
01137: 		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
01138: 		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
01139: 		try
01140: 		{
01141: 			string itemId = StorageGrid.GetItemId(pack, slot);
01142: 			if (itemId == null)
01143: 			{
01144: 				if (((Component)icon).gameObject.activeSelf)
01145: 				{
01146: 					((Component)icon).gameObject.SetActive(false);
01147: 				}
01148: 				return;
01149: 			}
01150: 			string spriteAtlas = StorageGrid.GetSpriteAtlas(pack, slot);
01151: 			string spritePath = StorageGrid.GetSpritePath(pack, slot);
01152: 			Sprite val = ((spriteAtlas != null || spritePath != null) ? LoadSpriteCached(spriteAtlas, spritePath) : null);
01153: 			if ((Object)(object)val == (Object)null && NullGuard.HasText(itemId) && CanWarmNow(itemId))
01154: 			{
01155: 				RetryState retry = GetRetry(WarmTried, itemId);
01156: 				bool flag = true;
01157: 				try
01158: 				{
01159: 					GameItem val2 = null;
01160: 					try
01161: 					{
01162: 						val2 = DirectoryMaster.Item(itemId, true);
01163: 					}
01164: 					catch
01165: 					{
01166: 						flag = false;
01167: 					}
01168: 					try
01169: 					{
01170: 						if (val2 != null)
01171: 						{
01172: 							val2.Destroy();
01173: 						}
01174: 					}
01175: 					catch
01176: 					{
01177: 						flag = false;
01178: 					}
01179: 				}
01180: 				catch
01181: 				{
01182: 					flag = false;
01183: 				}
01184: 				retry.Time = Time.unscaledTime;
01185: 				val = ((spriteAtlas != null || spritePath != null) ? LoadSpriteCached(spriteAtlas, spritePath) : null);
01186: 				if ((Object)(object)val != (Object)null)
01187: 				{
01188: 					WarmTried.Remove(itemId);
01189: 				}
01190: 				else if (flag && ++retry.Fails >= 2)
01191: 				{
01192: 					Core.Debug("收纳箱预热：'" + itemId + "' 两轮预热后仍无贴图，本会话停止重试（对应 MOD 可能已卸载或贴图键失配）。");
01193: 				}
01194: 			}
01195: 			if ((Object)(object)val == (Object)null)
01196: 			{
01197: 				Sprite val3 = PlaceholderSprite();
01198: 				if ((Object)(object)val3 != (Object)null)
01199: 				{
01200: 					if ((Object)(object)icon.sprite != (Object)(object)val3)
01201: 					{
01202: 						icon.sprite = val3;
01203: 					}
01204: 					((Graphic)icon).color = Color.white;
01205: 					if (!((Component)icon).gameObject.activeSelf)
01206: 					{
01207: 						((Component)icon).gameObject.SetActive(true);
01208: 					}
01209: 				}
01210: 				return;
01211: 			}
01212: 			if ((Object)(object)icon.sprite != (Object)(object)val)
01213: 			{
01214: 				icon.sprite = val;
01215: 			}
01216: 			((Graphic)icon).color = Color.white;
01217: 			UpdateSlotModLayers(icon, slot, itemId);
01218: 			Transform val4 = ((Component)icon).transform.Find("item_storage_slot_liquid_" + slot);
01219: 			if ((Object)(object)val4 != (Object)null)
01220: 			{
01221: 				Image component = ((Component)val4).GetComponent<Image>();
01222: 				if ((Object)(object)component != (Object)null)
01223: 				{
01224: 					if ((Object)(object)component.sprite != (Object)(object)val)
01225: 					{
01226: 						component.sprite = val;
01227: 					}
01228: 					if (BoxTransfer.TryGetPoolFill(pack, slot, out var fill) && fill > 0.001f)
01229: 					{
01230: 						component.fillAmount = fill;
01231: 						((Graphic)component).color = (BoxTransfer.TryGetPoolColor(pack, slot, out var col) ? new Color(col.r, col.g, col.b, 0.7f) : new Color(1f, 1f, 1f, 0.7f));
01232: 						if (!((Component)val4).gameObject.activeSelf)
01233: 						{
01234: 							((Component)val4).gameObject.SetActive(true);
01235: 						}
01236: 					}
01237: 					else if (((Component)val4).gameObject.activeSelf)
01238: 					{
01239: 						((Component)val4).gameObject.SetActive(false);
01240: 					}
01241: 				}
01242: 			}
01243: 			SlotLock.UpdateBadge(icon, pack, slot);
01244: 			if (!((Component)icon).gameObject.activeSelf)
01245: 			{
01246: 				((Component)icon).gameObject.SetActive(true);
01247: 			}
01248: 		}
01249: 		catch (Exception ex)
01250: 		{
01251: 			Core.Log?.Warning("收纳箱 UpdateSlotIcon(" + slot + ")：" + ex.Message);
01252: 		}
01253: 	}
01254: 
01255: 	internal static bool IsSpriteResolvable(string? atlas, string? spr)
01256: 	{
01257: 		try
01258: 		{
01259: 			return (Object)(object)LoadSpriteCached(atlas, spr) != (Object)null;
01260: 		}
01261: 		catch
01262: 		{
01263: 			return false;
01264: 		}
01265: 	}
01266: 
01267: 	private static Func<GameItem, List<ItemSpriteModifier>>? GetTemplateModDelegate(string itemId)
01268: 	{
01269: 		if (SlotModDelegates.TryGetValue(itemId, out Func<GameItem, List<ItemSpriteModifier>> value))
01270: 		{
01271: 			return value;
01272: 		}
01273: 		Func<GameItem, List<ItemSpriteModifier>> func = null;
01274: 		try
01275: 		{
01276: 			GameItem val = null;
01277: 			try
01278: 			{
01279: 				val = DirectoryMaster.Item(itemId, true);
01280: 			}
01281: 			catch
01282: 			{
01283: 			}
01284: 			if (val != null)
01285: 			{
01286: 				try
01287: 				{
01288: 					func = ((GameItemFunc)val).getItemSpriteModifierForItemFunc;
01289: 				}
01290: 				catch
01291: 				{
01292: 				}
01293: 				try
01294: 				{
01295: 					val.Destroy();
01296: 				}
01297: 				catch
01298: 				{
01299: 				}
01300: 			}
01301: 		}
01302: 		catch
01303: 		{
01304: 		}
01305: 		SlotModDelegates[itemId] = func;
01306: 		return func;
01307: 	}
01308: 
01309: 	private static void UpdateSlotModLayers(Image icon, int slot, string itemId)
01310: 	{
01311: 		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
01312: 		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
01313: 		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
01314: 		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
01315: 		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
01316: 		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
01317: 		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
01318: 		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
01319: 		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
01320: 		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
~~~

~~~csharp
01374: 			}
01375: 			int num3 = 0;
01376: 			foreach (ItemSpriteModifier item in list)
01377: 			{
01378: 				if ((Object)(object)item.sprite == (Object)null)
01379: 				{
01380: 					num3++;
01381: 					continue;
01382: 				}
01383: 				GameObject val2 = new GameObject("item_storage_slot_mod_" + slot + "_" + num3);
01384: 				val2.transform.SetParent(transform, false);
01385: 				Transform transform2 = val2.transform;
01386: 				RectTransform val3 = (RectTransform)(((object)((transform2 is RectTransform) ? transform2 : null)) ?? ((object)val2.AddComponent<RectTransform>()));
01387: 				val3.anchorMin = Vector2.zero;
01388: 				val3.anchorMax = Vector2.one;
01389: 				val3.offsetMin = Vector2.zero;
01390: 				val3.offsetMax = Vector2.zero;
01391: 				((Transform)val3).localScale = (Vector3)(item.isFlipped ? new Vector3(-1f, 1f, 1f) : Vector3.one);
01392: 				int num4 = (item.orientation % 4 + 4) % 4;
01393: 				if (num4 != 0)
01394: 				{
01395: 					((Transform)val3).localEulerAngles = new Vector3(0f, 0f, 90f * (float)num4);
01396: 				}
01397: 				Image obj2 = val2.AddComponent<Image>();
01398: 				obj2.sprite = item.sprite;
01399: 				obj2.type = (Type)0;
01400: 				obj2.preserveAspect = true;
01401: 				((Graphic)obj2).raycastTarget = false;
01402: 				((Graphic)obj2).color = item.color;
01403: 				val2.SetActive(true);
01404: 				num3++;
01405: 			}
01406: 		}
01407: 		catch (Exception ex)
01408: 		{
01409: 			Core.Debug("收纳箱 叠层渲染槽 " + slot + " '" + itemId + "'：" + ex.Message);
01410: 		}
01411: 	}
01412: 
01413: 	private static Sprite? PlaceholderSprite()
01414: 	{
01415: 		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
01416: 		//IL_002e: Expected O, but got Unknown
01417: 		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
01418: 		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
01419: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
01420: 		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
01421: 		try
01422: 		{
01423: 			if ((Object)(object)_placeholder != (Object)null && IsAliveSprite(_placeholder))
01424: 			{
01425: 				return _placeholder;
01426: 			}
01427: 			Texture2D val = new Texture2D(4, 4, (TextureFormat)4, false);
01428: 			Color32[] array = (Color32[])(object)new Color32[16];
01429: 			for (int i = 0; i < array.Length; i++)
01430: 			{
01431: 				array[i] = new Color32((byte)72, (byte)72, (byte)78, byte.MaxValue);
01432: 			}
01433: 			val.SetPixels32(array);
01434: 			val.Apply(false, true);
01435: 			_placeholder = Sprite.Create(val, new Rect(0f, 0f, 4f, 4f), new Vector2(0.5f, 0.5f), 100f);
01436: 			return _placeholder;
01437: 		}
01438: 		catch
01439: 		{
01440: 			return null;
01441: 		}
01442: 	}
01443: 
01444: 	private static Sprite? LoadSpriteCached(string? atlas, string? spr)
01445: 	{
01446: 		string key = atlas + "|" + spr;
01447: 		if (SpriteCache.TryGetValue(key, out Sprite value))
01448: 		{
01449: 			if (IsAliveSprite(value))
01450: 			{
01451: 				return value;
01452: 			}
01453: 			SpriteCache.Remove(key);
01454: 		}
01455: 		Sprite val = null;
01456: 		if (NullGuard.HasText(spr))
01457: 		{
01458: 			try
01459: 			{
01460: 				if (StoragePatch.ResolvedSpriteByName.TryGetValue(spr, out Sprite value2) && IsAliveSprite(value2))
01461: 				{
01462: 					val = value2;
01463: 				}
01464: 			}
01465: 			catch
01466: 			{
01467: 			}
01468: 		}
01469: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(atlas) && NullGuard.HasText(spr))
01470: 		{
01471: 			try
01472: 			{
01473: 				val = RenderHandler.TryLoadFromAtlas(atlas, spr);
01474: 			}
01475: 			catch (Exception ex)
01476: 			{
01477: 				Core.Debug("收纳箱 TryLoadFromAtlas '" + spr + "'：" + ex.Message);
01478: 			}
01479: 		}
01480: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(atlas) && NullGuard.HasText(spr))
01481: 		{
01482: 			try
01483: 			{
01484: 				LoadedMod val2 = null;
01485: 				List<LoadedMod> mods = ModLoader.Mods;
01486: 				if (mods != null)
01487: 				{
01488: 					for (int i = 0; i < mods.Count; i++)
01489: 					{
01490: 						LoadedMod val3 = mods[i];
01491: 						if (val3?.Manifest != null && string.Equals(val3.Manifest.ID, atlas, StringComparison.Ordinal))
01492: 						{
01493: 							val2 = val3;
01494: 							break;
01495: 						}
01496: 					}
01497: 				}
01498: 				if (val2 != null && val2.Sprites.TryGetValue(spr, out var value3) && IsAliveSprite(value3))
01499: 				{
01500: 					val = value3;
01501: 				}
01502: 			}
01503: 			catch
01504: 			{
01505: 			}
01506: 		}
01507: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(atlas))
01508: 		{
01509: 			int num = atlas.LastIndexOf('/');
01510: 			if (num >= 0 && num < atlas.Length - 1)
01511: 			{
01512: 				string text = atlas.Substring(num + 1);
01513: 				string text2 = atlas.Substring(0, num);
01514: 				try
01515: 				{
01516: 					if (StoragePatch.ResolvedSpriteByName.TryGetValue(text, out Sprite value4) && IsAliveSprite(value4))
01517: 					{
01518: 						val = value4;
01519: 					}
01520: 				}
01521: 				catch
01522: 				{
01523: 				}
01524: 				if ((Object)(object)val == (Object)null)
01525: 				{
01526: 					try
01527: 					{
01528: 						val = RenderHandler.TryLoadFromAtlas(text2, text);
01529: 					}
01530: 					catch
01531: 					{
01532: 					}
01533: 				}
01534: 				if ((Object)(object)val == (Object)null)
01535: 				{
01536: 					try
01537: 					{
01538: 						val = RenderHandler.TryLoadFromAtlas(atlas, text);
01539: 					}
01540: 					catch
01541: 					{
01542: 					}
01543: 				}
01544: 			}
01545: 		}
01546: 		if ((Object)(object)val == (Object)null && NullGuard.HasText(spr) && CanProbeNow(spr))
01547: 		{
01548: 			Sprite val4 = ProbeResolve(spr);
01549: 			if ((Object)(object)val4 != (Object)null)
01550: 			{
01551: 				val = val4;
01552: 				ProbeTried.Remove(spr);
01553: 			}
01554: 			else
01555: 			{
01556: 				RetryState retry = GetRetry(ProbeTried, spr);
01557: 				retry.Time = Time.unscaledTime;
01558: 				if (++retry.Fails >= 2)
01559: 				{
01560: 					Core.Debug("收纳箱探测：'" + spr + "' 两轮探测后仍无贴图，本会话停止重试。");
01561: 				}
01562: 			}
01563: 		}
01564: 		if ((Object)(object)val != (Object)null && StoragePatch.IsUnknownSprite(val))
01565: 		{
01566: 			return null;
01567: 		}
01568: 		if (IsAliveSprite(val))
01569: 		{
01570: 			SpriteCache[key] = val;
01571: 		}
01572: 		return val;
01573: 	}
01574: 
01575: 	private static bool IsAliveSprite(Sprite? sprite)
01576: 	{
01577: 		if ((Object)(object)sprite == (Object)null)
01578: 		{
01579: 			return false;
01580: 		}
01581: 		try
01582: 		{
01583: 			return (Object)(object)sprite != (Object)null;
01584: 		}
01585: 		catch
01586: 		{
01587: 			return false;
01588: 		}
01589: 	}
01590: 
01591: 	private static RetryState GetRetry(Dictionary<string, RetryState> table, string key)
01592: 	{
01593: 		if (!table.TryGetValue(key, out RetryState value))
01594: 		{
01595: 			value = (table[key] = new RetryState());
01596: 		}
01597: 		return value;
01598: 	}
01599: 
01600: 	private static bool CanWarmNow(string itemId)
01601: 	{
01602: 		return CanRetry(WarmTried, itemId);
01603: 	}
01604: 
01605: 	private static bool CanProbeNow(string spr)
01606: 	{
01607: 		return CanRetry(ProbeTried, spr);
01608: 	}
01609: 
01610: 	private static bool CanRetry(Dictionary<string, RetryState> table, string key)
01611: 	{
01612: 		if (table.TryGetValue(key, out RetryState value))
01613: 		{
01614: 			if (value.Fails < 2)
01615: 			{
01616: 				return Time.unscaledTime - value.Time > 60f;
01617: 			}
01618: 			return false;
01619: 		}
01620: 		return true;
01621: 	}
01622: 
01623: 	private static Sprite? ProbeResolve(string spr)
01624: 	{
01625: 		GameItemElement val = FindAnyLiveElement();
01626: 		if (val != null)
01627: 		{
01628: 			try
01629: 			{
~~~

~~~csharp
01749: 	}
01750: 
01751: 	internal static void Tick()
01752: 	{
01753: 		if (IconRefreshItems.Count == 0)
01754: 		{
01755: 			return;
01756: 		}
01757: 		try
01758: 		{
01759: 			float time = Time.time;
01760: 			for (int num = IconRefreshItems.Count - 1; num >= 0; num--)
01761: 			{
01762: 				if (!(time < IconRefreshAt[num]))
01763: 				{
01764: 					GameItem val = IconRefreshItems[num];
01765: 					bool flag = false;
01766: 					try
01767: 					{
01768: 						flag = val != null && val != null;
01769: 					}
01770: 					catch
01771: 					{
01772: 						flag = false;
01773: 					}
01774: 					if (!flag)
01775: 					{
01776: 						IconRefreshItems.RemoveAt(num);
01777: 						IconRefreshAt.RemoveAt(num);
01778: 						IconRetryTried.RemoveAt(num);
01779: 					}
01780: 					else
01781: 					{
01782: 						try
01783: 						{
01784: 							RefreshButtons(val);
01785: 						}
01786: 						catch
01787: 						{
01788: 						}
01789: 						if (!HasUnresolvedIcon(val))
01790: 						{
01791: 							IconRefreshItems.RemoveAt(num);
01792: 							IconRefreshAt.RemoveAt(num);
01793: 							IconRetryTried.RemoveAt(num);
01794: 						}
01795: 						else if (++IconRetryTried[num] >= 40)
01796: 						{
01797: 							Core.Debug("收纳箱 图标重试放弃：'" + (Core.Clean(val.identifier) ?? "?") + "' 仍有未解析贴图（MOD 卸载或贴图键失效）");
01798: 							IconRefreshItems.RemoveAt(num);
01799: 							IconRefreshAt.RemoveAt(num);
01800: 							IconRetryTried.RemoveAt(num);
01801: 						}
01802: 						else
01803: 						{
01804: 							IconRefreshAt[num] = time + 1.5f;
01805: 						}
01806: 					}
01807: 				}
01808: 			}
01809: 		}
01810: 		catch
01811: 		{
01812: 		}
01813: 	}
01814: 
01815: 	private static bool IsOwnTopmost(Vector2 screenPos, GameObject? btnGo, GameObject? iconGo, GameObject? clickGo = null)
01816: 	{
01817: 		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
01818: 		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
01819: 		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
01820: 		//IL_0048: Expected O, but got Unknown
01821: 		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
01822: 		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
01823: 		try
01824: 		{
01825: 			if ((Object)(object)btnGo == (Object)null && (Object)(object)iconGo == (Object)null && (Object)(object)clickGo == (Object)null)
01826: 			{
01827: 				return true;
01828: 			}
01829: 			EventSystem current = EventSystem.current;
01830: 			if ((Object)(object)current == (Object)null)
01831: 			{
01832: 				return true;
01833: 			}
01834: 			PointerEventData val = new PointerEventData(current)
01835: 			{
01836: 				position = screenPos
01837: 			};
01838: 			List<RaycastResult> list = new List<RaycastResult>();
01839: 			current.RaycastAll(val, list);
01840: 			if (list.Count == 0)
01841: 			{
01842: 				return true;
01843: 			}
01844: 			RaycastResult val2 = list[0];
01845: 			GameObject gameObject = ((RaycastResult)(ref val2)).gameObject;
01846: 			if ((Object)(object)gameObject == (Object)null)
01847: 			{
01848: 				return true;
01849: 			}
01850: 			if ((Object)(object)btnGo != (Object)null && ((Object)(object)gameObject == (Object)(object)btnGo || gameObject.transform.IsChildOf(btnGo.transform)))
01851: 			{
01852: 				return true;
01853: 			}
01854: 			if ((Object)(object)iconGo != (Object)null && ((Object)(object)gameObject == (Object)(object)iconGo || gameObject.transform.IsChildOf(iconGo.transform)))
01855: 			{
01856: 				return true;
01857: 			}
01858: 			if ((Object)(object)clickGo != (Object)null && ((Object)(object)gameObject == (Object)(object)clickGo || gameObject.transform.IsChildOf(clickGo.transform)))
01859: 			{
01860: 				return true;
01861: 			}
01862: 			return false;
01863: 		}
01864: 		catch
01865: 		{
01866: 			return true;
~~~

~~~csharp
02015: 									Vector2 zero = Vector2.zero;
02016: 									bool value5 = (Object)(object)val6 != (Object)null && RectTransformUtility.ScreenPointToLocalPointInRectangle(val6, screenPos, val5, ref zero);
02017: 									StringBuilder stringBuilder2 = stringBuilder.Append(" 槽0换算=").Append(value5).Append(" local=(")
02018: 										.Append(zero.x.ToString("0.0"))
02019: 										.Append(',')
02020: 										.Append(zero.y.ToString("0.0"))
02021: 										.Append(')')
02022: 										.Append(" rect=");
02023: 									Rect val8;
02024: 									if (!((Object)(object)val6 != (Object)null))
02025: 									{
02026: 										val3 = default(Rect);
02027: 										val8 = val3;
02028: 									}
02029: 									else
02030: 									{
02031: 										val8 = val6.rect;
02032: 									}
02033: 									stringBuilder2.Append(val8);
02034: 								}
02035: 								catch
02036: 								{
02037: 									stringBuilder.Append(" 槽0换算异常");
02038: 								}
02039: 							}
02040: 						}
02041: 					}
02042: 				}
02043: 				catch
02044: 				{
02045: 				}
02046: 			}
02047: 		}
02048: 		if (stringBuilder != null)
02049: 		{
02050: 			Core.Debug("收纳箱 drop 未命中诊断：鼠标=(" + screenPos.x.ToString("0.0") + "," + screenPos.y.ToString("0.0") + ") 候选盒=" + num + stringBuilder.ToString());
02051: 		}
02052: 		return false;
02053: 	}
02054: 
02055: 	private static bool HitTest(RectTransform rt, Vector2 screenPos)
02056: 	{
02057: 		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
02058: 		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
02059: 		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
02060: 		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
02061: 		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
02062: 		try
02063: 		{
02064: 			Camera val = null;
02065: 			try
02066: 			{
02067: 				Canvas componentInParent = ((Component)rt).GetComponentInParent<Canvas>();
02068: 				if ((Object)(object)componentInParent != (Object)null && (int)componentInParent.renderMode != 0)
02069: 				{
02070: 					val = (((Object)(object)componentInParent.worldCamera != (Object)null) ? componentInParent.worldCamera : Camera.main);
02071: 				}
02072: 			}
02073: 			catch
02074: 			{
02075: 				val = Camera.main;
02076: 			}
02077: 			Vector2 val2 = default(Vector2);
02078: 			int result;
02079: 			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPos, val, ref val2))
02080: 			{
02081: 				Rect rect = rt.rect;
02082: 				result = (((Rect)(ref rect)).Contains(val2) ? 1 : 0);
02083: 			}
02084: 			else
02085: 			{
02086: 				result = 0;
02087: 			}
02088: 			return (byte)result != 0;
02089: 		}
02090: 		catch
02091: 		{
02092: 			return false;
02093: 		}
02094: 	}
02095: 
02096: 	private static void TrackBox(GameItem pack)
02097: 	{
02098: 		try
02099: 		{
02100: 			for (int num = TrackedBoxes.Count - 1; num >= 0; num--)
02101: 			{
02102: 				try
02103: 				{
02104: 					if (TrackedBoxes[num] == pack)
02105: 					{
02106: 						TrackedBoxes.RemoveAt(num);
02107: 					}
02108: 				}
02109: 				catch
02110: 				{
02111: 					TrackedBoxes.RemoveAt(num);
02112: 				}
02113: 			}
02114: 			TrackedBoxes.Add(pack);
02115: 		}
02116: 		catch
02117: 		{
02118: 		}
02119: 	}
~~~

### ItemStorage/SlotLock.cs

~~~csharp
00001: using System;
00002: using UnityEngine;
00003: using UnityEngine.UI;
00004: 
00005: namespace ItemStorage;
00006: 
00007: internal static class SlotLock
00008: {
00009: 	internal static bool IsLocked(GameItem pack, int slot)
00010: 	{
00011: 		string slotLock = StorageGrid.GetSlotLock(pack, slot);
00012: 		if (!NullGuard.HasText(slotLock))
00013: 		{
00014: 			return false;
00015: 		}
00016: 		string itemId = StorageGrid.GetItemId(pack, slot);
00017: 		if (NullGuard.HasText(itemId))
00018: 		{
00019: 			return string.Equals(itemId, slotLock, StringComparison.Ordinal);
00020: 		}
00021: 		return false;
00022: 	}
00023: 
00024: 	internal static bool? Toggle(GameItem pack, int slot)
00025: 	{
00026: 		if (pack == null || !StorageGrid.IsValidSlot(slot))
00027: 		{
00028: 			return null;
00029: 		}
00030: 		try
00031: 		{
00032: 			if (IsLocked(pack, slot))
00033: 			{
00034: 				StorageGrid.SetSlotLock(pack, slot, null);
00035: 				Core.Log?.Msg("收纳箱：格 " + slot + " 已解锁（重新参与自动上货）。");
00036: 				NotifyToggle(pack, slot, locked: false);
00037: 				return false;
00038: 			}
00039: 			string itemId = StorageGrid.GetItemId(pack, slot);
00040: 			if (!NullGuard.HasText(itemId))
00041: 			{
00042: 				BoxItem.NotifyUser("空格子无法锁定，请先放入物品");
00043: 				return null;
00044: 			}
00045: 			if (!BoxItem.IsUsableByPlayer(pack))
00046: 			{
00047: 				BoxItem.NotifyUnowned();
00048: 				return null;
00049: 			}
00050: 			StorageGrid.SetSlotLock(pack, slot, itemId);
00051: 			Core.Log?.Msg("收纳箱：格 " + slot + " '" + itemId + "' 已锁定（不参与自动上货）。");
00052: 			NotifyToggle(pack, slot, locked: true);
00053: 			return true;
00054: 		}
00055: 		catch (Exception ex)
00056: 		{
00057: 			Core.Log?.Warning("收纳箱 SlotLock.Toggle(" + slot + ")：" + ex.Message);
00058: 			return null;
00059: 		}
00060: 	}
00061: 
00062: 	private static void NotifyToggle(GameItem pack, int slot, bool locked)
00063: 	{
00064: 		try
00065: 		{
00066: 			string text = StorageGrid.GetDisplayName(pack, slot);
00067: 			if (!NullGuard.HasText(text))
00068: 			{
00069: 				string itemId = StorageGrid.GetItemId(pack, slot);
00070: 				if (NullGuard.HasText(itemId))
00071: 				{
00072: 					text = BoxWindow.DisplayNameForItem(itemId, int.MaxValue);
00073: 				}
00074: 			}
00075: 			if (NullGuard.HasText(text))
00076: 			{
00077: 				BoxItem.NotifyUser(locked ? (text + " 将不参与自动上货") : (text + " 重新参与自动上货"));
00078: 			}
00079: 		}
00080: 		catch
00081: 		{
00082: 		}
00083: 	}
00084: 
00085: 	internal static Color BadgeColor()
00086: 	{
00087: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00088: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00089: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
00090: 		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
00091: 		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
00092: 		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
00093: 		try
00094: 		{
00095: 			return Color32.op_Implicit(RenderHandler.GetColorFromPalette((ColorPalette)12615037));
00096: 		}
00097: 		catch
00098: 		{
00099: 			return new Color(0.85f, 0.15f, 0.15f, 1f);
00100: 		}
00101: 	}
00102: 
00103: 	internal static Color LockTextColor()
00104: 	{
00105: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00106: 		return Color.white;
00107: 	}
00108: 
00109: 	internal static Color LockPlateColor()
00110: 	{
00111: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00112: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00113: 		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
00114: 		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
00115: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
00116: 		try
00117: 		{
00118: 			return MachineryStyle.LABEL_TEXT;
00119: 		}
00120: 		catch
00121: 		{
00122: 			return new Color(0.13f, 0.12f, 0.14f, 1f);
00123: 		}
00124: 	}
00125: 
00126: 	internal static Color LabelColor()
~~~

~~~csharp
00186: 			}
00187: 			GameObject val2;
00188: 			if ((Object)(object)val == (Object)null)
00189: 			{
00190: 				val2 = new GameObject("item_storage_slot_lock");
00191: 				val2.transform.SetParent(((Component)icon).transform, false);
00192: 				Transform transform = val2.transform;
00193: 				object obj = ((object)((transform is RectTransform) ? transform : null)) ?? ((object)val2.AddComponent<RectTransform>());
00194: 				((RectTransform)obj).anchorMin = new Vector2(0f, 1f);
00195: 				((RectTransform)obj).anchorMax = new Vector2(0f, 1f);
00196: 				((RectTransform)obj).pivot = new Vector2(0f, 1f);
00197: 				((RectTransform)obj).anchoredPosition = new Vector2(2f, -2f);
00198: 				((RectTransform)obj).sizeDelta = new Vector2(6f, 6f);
00199: 				((Transform)obj).localScale = Vector3.one;
00200: 				Image obj2 = val2.AddComponent<Image>();
00201: 				obj2.sprite = null;
00202: 				((Graphic)obj2).color = BadgeColor();
00203: 				((Graphic)obj2).raycastTarget = false;
00204: 			}
00205: 			else
00206: 			{
00207: 				val2 = ((Component)val).gameObject;
00208: 				Image component = val2.GetComponent<Image>();
00209: 				if ((Object)(object)component != (Object)null)
00210: 				{
00211: 					((Graphic)component).color = BadgeColor();
00212: 				}
00213: 			}
00214: 			if (!val2.activeSelf)
00215: 			{
00216: 				val2.SetActive(true);
00217: 			}
00218: 		}
00219: 		catch (Exception ex)
00220: 		{
00221: 			Core.Debug("收纳箱 锁定红点刷新(" + slot + ")：" + ex.Message);
00222: 		}
00223: 	}
00224: }
~~~

### ItemStorage/BoxRestock.cs

~~~csharp
00084: 		{
00085: 			PlayerStore instance = PlayerStore.Instance;
00086: 			if (instance != null)
00087: 			{
00088: 				_loaded = true;
00089: 				int num = PerSaveState.GetInt(instance, "RestockAllHotkey");
00090: 				_key = (KeyCode)((num >= 8 && num <= 350) ? num : 0);
00091: 			}
00092: 		}
00093: 		catch
00094: 		{
00095: 			_loaded = true;
00096: 			_key = (KeyCode)0;
00097: 		}
00098: 	}
00099: 
00100: 	internal static void SetKey(KeyCode key)
00101: 	{
00102: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00103: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00104: 		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
00105: 		//IL_0021: Expected I4, but got Unknown
00106: 		_key = key;
00107: 		_loaded = true;
00108: 		try
00109: 		{
00110: 			PlayerStore instance = PlayerStore.Instance;
00111: 			if (instance != null)
00112: 			{
00113: 				PerSaveState.SetInt(instance, "RestockAllHotkey", (int)key);
00114: 				PerSaveState.Save(instance);
00115: 			}
00116: 		}
00117: 		catch (Exception ex)
00118: 		{
00119: 			Core.Log?.Warning("全箱上货键位写入存档失败：" + ex.Message);
00120: 		}
00121: 	}
00122: 
00123: 	private static void ClearKey(string reason)
00124: 	{
00125: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
00126: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00127: 		if (_loaded && (int)_key == 0)
00128: 		{
00129: 			return;
00130: 		}
00131: 		_key = (KeyCode)0;
00132: 		_loaded = true;
00133: 		try
00134: 		{
00135: 			PlayerStore instance = PlayerStore.Instance;
00136: 			if (instance != null)
00137: 			{
00138: 				PerSaveState.SetInt(instance, "RestockAllHotkey", 0);
00139: 			}
00140: 		}
00141: 		catch
00142: 		{
00143: 		}
00144: 		Core.Log?.Msg("全箱上货键位已解绑：" + reason);
00145: 	}
00146: 
00147: 	internal static void UnbindManually()
00148: 	{
00149: 		ClearKey("玩家在齿轮面板捕获态按 Esc 主动解除绑定");
00150: 	}
00151: 
00152: 	internal static void ResetRecovery()
00153: 	{
00154: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00155: 		_loaded = false;
00156: 		_key = (KeyCode)0;
00157: 	}
00158: 
00159: 	internal static void OnAllBoxesDestroyed()
00160: 	{
00161: 		ClearKey("玩家拥有的收纳箱已全部被系统销毁，释放全箱上货键");
00162: 	}
00163: 
00164: 	internal unsafe static void Tick()
00165: 	{
00166: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00167: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00168: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00169: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
00170: 		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
00171: 		PumpRun();
00172: 		KeyCode key = Key;
00173: 		if ((int)key == 0)
00174: 		{
00175: 			return;
00176: 		}
00177: 		try
00178: 		{
00179: 			if (!Input.GetKeyDown(key))
00180: 			{
00181: 				return;
00182: 			}
00183: 		}
00184: 		catch
00185: 		{
00186: 			return;
00187: 		}
00188: 		if (GearPanel.IsCapturing || BoxHotkey.IsSuppressed(key))
00189: 		{
00190: 			return;
00191: 		}
00192: 		try
00193: 		{
00194: 			if (KeyBindHelper.SuppressHotkeys)
00195: 			{
00196: 				return;
00197: 			}
00198: 		}
00199: 		catch
00200: 		{
00201: 		}
00202: 		try
00203: 		{
00204: 			if (StoreUIManager.IsTextFieldFocused())
00205: 			{
00206: 				return;
00207: 			}
00208: 		}
00209: 		catch
00210: 		{
00211: 		}
00212: 		if (!BoxTransfer.IsInStore())
00213: 		{
00214: 			return;
00215: 		}
00216: 		Core.Debug("全箱上货快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 触发。");
00217: 		BoxItem.ClickSound();
00218: 		try
00219: 		{
00220: 			RunAllBoxes("全箱快捷键");
00221: 		}
00222: 		catch (Exception ex)
00223: 		{
00224: 			Core.Log?.Warning("全箱上货执行异常：" + ex.Message);
00225: 		}
00226: 	}
00227: 
00228: 	internal static void RunOne(GameItem box, string context)
00229: 	{
00230: 		if (box != null && BoxItem.IsBox(box))
00231: 		{
00232: 			RunCore(new List<GameItem> { box }, context, counterScope: false);
00233: 		}
00234: 	}
00235: 
00236: 	internal static void RunAllBoxes(string context)
00237: 	{
00238: 		List<(GameItem, int)> list = new List<(GameItem, int)>();
00239: 		try
00240: 		{
00241: 			foreach (GameItem item2 in BoxItem.FindAllBoxes())
00242: 			{
00243: 				if (item2 != null && BoxItem.IsUsableByPlayer(item2))
00244: 				{
00245: 					list.Add((item2, BoxItem.ReadNumber(item2)));
00246: 				}
00247: 			}
00248: 		}
00249: 		catch (Exception ex)
00250: 		{
00251: 			Core.Log?.Warning("全箱上货[" + context + "] 扫描箱子失败：" + ex.Message);
00252: 			return;
00253: 		}
00254: 		if (list.Count == 0)
00255: 		{
00256: 			BoxItem.NotifyUser("没有可用的收纳箱");
00257: 			return;
00258: 		}
00259: 		list.Sort(delegate((GameItem Box, int Num) a, (GameItem Box, int Num) b)
00260: 		{
00261: 			if (a.Num == b.Num)
00262: 			{
00263: 				return 0;
00264: 			}
00265: 			if (a.Num == -1)
00266: 			{
00267: 				return 1;
00268: 			}
00269: 			return (b.Num == -1) ? (-1) : a.Num.CompareTo(b.Num);
00270: 		});
00271: 		List<GameItem> list2 = new List<GameItem>();
00272: 		int num = 0;
00273: 		foreach (var item3 in list)
00274: 		{
00275: 			GameItem item = item3.Item1;
00276: 			try
00277: 			{
00278: 				if (StorageGrid.GetBoxOption(item, "RESTOCKGUARD") == 1)
00279: 				{
00280: 					num++;
00281: 					continue;
00282: 				}
00283: 			}
00284: 			catch
00285: 			{
00286: 				num++;
00287: 			}
00288: 			list2.Add(item);
00289: 		}
00290: 		if (list2.Count == 0)
00291: 		{
00292: 			BoxItem.NotifyUser("所有收纳箱均设为不参与全箱上架");
00293: 			return;
00294: 		}
00295: 		RunCore(list2, context, counterScope: true);
00296: 		if (num > 0)
00297: 		{
00298: 			Core.Debug("全箱上货[" + context + "]：跳过 " + num + " 个设为不参与的箱。");
00299: 		}
00300: 	}
00301: 
00302: 	private static void RunCore(List<GameItem> boxes, string context, bool counterScope)
00303: 	{
00304: 		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
00305: 		//IL_0082: Invalid comparison between Unknown and I4
00306: 		if (_run != null)
00307: 		{
00308: 			BoxItem.NotifyUser("上货进行中，请稍候");
00309: 			return;
00310: 		}
00311: 		if (!BoxTransfer.IsInStore())
00312: 		{
00313: 			Core.Debug("一键上货[" + context + "]：玩家不在店内，忽略。");
00314: 			BoxItem.NotifyUser("请回到店内再上货");
00315: 			return;
00316: 		}
00317: 		StoreClient val = null;
00318: 		StoreClientInstance val2 = null;
00319: 		try
00320: 		{
00321: 			val2 = PlayerStore.Instance?.currentClientInstance;
00322: 			if (val2 == null)
00323: 			{
00324: 				BoxItem.NotifyUser("必须有顾客在场时才能一键上货");
00325: 				return;
00326: 			}
00327: 			val = val2.GetClientBlueprint();
00328: 			if (val == null)
00329: 			{
00330: 				BoxItem.NotifyUser("必须有顾客在场时才能一键上货");
00331: 				return;
00332: 			}
00333: 			try
00334: 			{
00335: 				if ((int)val.clientIntent != 1)
00336: 				{
00337: 					BoxItem.NotifyUser("该顾客不是来购物的，不支持一键上货");
00338: 					return;
00339: 				}
00340: 			}
00341: 			catch
00342: 			{
00343: 			}
00344: 			if (!val.useClientBudget || val.GetBudget() <= 0)
00345: 			{
00346: 				BoxItem.NotifyUser("该顾客没有预算，不支持一键上货");
00347: 				return;
00348: 			}
00349: 		}
00350: 		catch (Exception ex)
00351: 		{
00352: 			Core.Log?.Warning("一键上货[" + context + "] 顾客判定异常：" + ex.Message);
00353: 			BoxItem.NotifyUser("必须有顾客在场时才能一键上货");
00354: 			return;
00355: 		}
00356: 		GameGridInventory val3 = null;
00357: 		try
00358: 		{
00359: 			EmporiumEntry instance = EmporiumEntry.Instance;
00360: 			val3 = ((instance != null) ? instance.backInvinvElement : null);
00361: 		}
00362: 		catch
00363: 		{
00364: 		}
00365: 		if (val3 == null)
00366: 		{
00367: 			BoxItem.NotifyUser("柜台未就绪，无法上货");
00368: 			return;
00369: 		}
00370: 		int budget = 0;
00371: 		try
00372: 		{
00373: 			budget = val.GetBudget();
00374: 		}
00375: 		catch
00376: 		{
00377: 		}
00378: 		long total = SumCounterValue(val, val3);
00379: 		bool flag = false;
00380: 		try
00381: 		{
00382: 			flag = val.isRequireVariety;
00383: 		}
00384: 		catch
00385: 		{
00386: 		}
00387: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
00388: 		HashSet<string> hashSet2 = new HashSet<string>(StringComparer.Ordinal);
00389: 		if (flag)
00390: 		{
00391: 			try
00392: 			{
00393: 				List<GameItem> childItems = ((GameInventory)val3).childItems;
00394: 				if (childItems != null)
00395: 				{
00396: 					foreach (GameItem item in childItems)
00397: 					{
00398: 						if (item == null)
00399: 						{
00400: 							continue;
00401: 						}
00402: 						try
00403: 						{
00404: 							string identifier = item.identifier;
00405: 							if (NullGuard.HasText(identifier))
00406: 							{
00407: 								hashSet2.Add(identifier);
00408: 							}
00409: 						}
00410: 						catch
00411: 						{
00412: 						}
00413: 					}
00414: 				}
00415: 			}
00416: 			catch
00417: 			{
00418: 			}
00419: 		}
00420: 		List<Candidate> list = new List<Candidate>();
00421: 		HashSet<GameItem> hashSet3 = new HashSet<GameItem>();
00422: 		try
~~~

~~~csharp
00476: 									flag3 = val.IsItemInVarietyList(val4);
00477: 								}
00478: 								catch
00479: 								{
00480: 									flag3 = false;
00481: 								}
00482: 							}
00483: 							try
00484: 							{
00485: 								val4.Destroy();
00486: 							}
00487: 							catch
00488: 							{
00489: 							}
00490: 						}
00491: 					}
00492: 					catch
00493: 					{
00494: 						flag2 = false;
00495: 					}
00496: 					if (flag2 && (!flag || (!flag3 && !hashSet.Contains(itemId) && !hashSet2.Contains(itemId))))
00497: 					{
00498: 						list.Add(new Candidate
00499: 						{
00500: 							Box = box,
00501: 							Slot = i,
00502: 							ItemId = itemId
00503: 						});
00504: 						hashSet3.Add(box);
00505: 					}
00506: 				}
00507: 			}
00508: 		}
00509: 		catch (Exception ex2)
00510: 		{
00511: 			Core.Log?.Warning("一键上货[" + context + "] 候选收集异常：" + ex2.Message);
00512: 		}
00513: 		if (list.Count == 0)
00514: 		{
00515: 			BoxItem.NotifyUser("没有顾客感兴趣的物品可上货");
00516: 			return;
00517: 		}
00518: 		_run = new RestockRun
00519: 		{
00520: 			ClientInst = val2,
00521: 			BP = val,
00522: 			Counter = val3,
00523: 			Context = context,
00524: 			Scope = (counterScope ? "全箱上货" : "一键上货"),
00525: 			Budget = budget,
00526: 			Total = total,
00527: 			Variety = flag,
00528: 			Queue = list,
00529: 			RestockedIds = hashSet,
00530: 			BoxesTouched = hashSet3
00531: 		};
00532: 	}
00533: 
00534: 	private static void PumpRun()
00535: 	{
00536: 		RestockRun run = _run;
00537: 		if (run == null)
00538: 		{
00539: 			return;
00540: 		}
00541: 		Stopwatch stopwatch = Stopwatch.StartNew();
00542: 		double num;
00543: 		try
00544: 		{
00545: 			num = (double)Time.unscaledDeltaTime * 1000.0 * 0.3;
00546: 		}
00547: 		catch
00548: 		{
00549: 			num = 5.0;
00550: 		}
00551: 		if (num > 8.0)
00552: 		{
00553: 			num = 8.0;
00554: 		}
00555: 		int num2 = 0;
00556: 		try
00557: 		{
00558: 			do
00559: 			{
00560: 				if (num2 > 0 && (stopwatch.Elapsed.TotalMilliseconds >= num || num2 >= 4))
00561: 				{
00562: 					return;
00563: 				}
00564: 				if (!BoxTransfer.IsInStore() || BoxTransfer.PeekStore()?.currentClientInstance != run.ClientInst)
00565: 				{
00566: 					Finish(run, aborted: true);
00567: 					return;
00568: 				}
00569: 				Candidate candidate = null;
00570: 				while (run.Queue.Count > 0)
00571: 				{
00572: 					Candidate candidate2 = run.Queue[0];
00573: 					run.Queue.RemoveAt(0);
00574: 					try
00575: 					{
00576: 						if (StorageGrid.GetCount(candidate2.Box, candidate2.Slot) <= 0 || !NullGuard.HasText(StorageGrid.GetItemId(candidate2.Box, candidate2.Slot)))
00577: 						{
00578: 							continue;
00579: 						}
00580: 						goto IL_00f5;
00581: 					}
00582: 					catch
00583: 					{
00584: 					}
00585: 					continue;
00586: 					IL_00f5:
00587: 					candidate = candidate2;
00588: 					break;
00589: 				}
00590: 				if (candidate == null)
00591: 				{
00592: 					Finish(run);
00593: 					return;
~~~

~~~csharp
00760: 			if (val != null)
00761: 			{
00762: 				try
00763: 				{
00764: 					long negociatedValue = val.GetNegociatedValue();
00765: 					if (negociatedValue > num)
00766: 					{
00767: 						num = negociatedValue;
00768: 					}
00769: 				}
00770: 				catch
00771: 				{
00772: 				}
00773: 				try
00774: 				{
00775: 					val.Destroy();
00776: 				}
00777: 				catch
00778: 				{
00779: 				}
00780: 			}
00781: 		}
00782: 		catch
00783: 		{
00784: 		}
00785: 		return num;
00786: 	}
00787: 
00788: 	private static void Finish(RestockRun run, bool aborted = false)
00789: 	{
00790: 		_run = null;
00791: 		foreach (GameItem item in run.BoxesTouched)
00792: 		{
00793: 			try
00794: 			{
00795: 				BoxWindow.RefreshButtons(item);
00796: 			}
00797: 			catch
00798: 			{
00799: 			}
00800: 		}
00801: 		try
00802: 		{
00803: 			EmporiumEntry instance = EmporiumEntry.Instance;
00804: 			if (instance != null)
00805: 			{
00806: 				instance.Validate(true);
00807: 			}
00808: 		}
00809: 		catch
00810: 		{
00811: 		}
00812: 		bool flag = run.BudgetReached || run.SkippedByBudget > 0;
00813: 		string text = ((run.Reverted > 0) ? ("，" + run.Reverted + " 件实值超预算已取回") : "");
00814: 		Core.Log?.Msg(run.Scope + "[" + run.Context + "] 完成：上货 " + run.Placed + " 件" + (run.CounterFull ? "，柜台已满" : "") + (flag ? "，已达顾客预算" : "") + text + "。");
00815: 		if (aborted)
00816: 		{
00817: 			if (run.Placed > 0)
00818: 			{
00819: 				BoxItem.NotifyUser("顾客已离场，上货中止（已上 " + run.Placed + " 件）");
00820: 			}
00821: 		}
00822: 		else if (run.Placed > 0)
00823: 		{
00824: 			if (run.CounterFull)
00825: 			{
00826: 				BoxItem.NotifyUser("柜台已满，自动上货 " + run.Placed + " 件");
00827: 			}
00828: 			else if (flag)
00829: 			{
00830: 				BoxItem.NotifyUser("已达顾客最高预算，自动上货 " + run.Placed + " 件");
00831: 			}
00832: 			else
00833: 			{
00834: 				BoxItem.NotifyUser("自动上货 " + run.Placed + " 件");
00835: 			}
00836: 		}
00837: 		else if (run.CounterFull)
00838: 		{
00839: 			BoxItem.NotifyUser("柜台已满，无法上货");
00840: 		}
00841: 		else
00842: 		{
00843: 			BoxItem.NotifyUser("顾客预算不足，买不起箱中任何物品");
00844: 		}
00845: 	}
00846: 
00847: 	private static long SumCounterValue(StoreClient bp, GameGridInventory counter)
00848: 	{
00849: 		long num = 0L;
00850: 		try
00851: 		{
00852: 			List<GameItem> childItems = ((GameInventory)counter).childItems;
00853: 			if (childItems == null)
00854: 			{
00855: 				return 0L;
00856: 			}
00857: 			foreach (GameItem item in childItems)
00858: 			{
00859: 				if (item == null)
00860: 				{
00861: 					continue;
00862: 				}
00863: 				try
00864: 				{
00865: 					if (bp.IsClientBuyingThisItem(item))
00866: 					{
00867: 						num += item.GetNegociatedValue();
00868: 					}
00869: 				}
00870: 				catch
00871: 				{
00872: 				}
00873: 			}
00874: 		}
00875: 		catch (Exception ex)
00876: 		{
00877: 			Core.Debug("一键上货 柜台总值重算异常：" + ex.Message);
00878: 		}
00879: 		return num;
00880: 	}
00881: }
~~~

### ItemStorage/BoxTidy.cs

~~~csharp
00004: 
00005: internal static class BoxTidy
00006: {
00007: 	internal static int Tidy(GameItem pack)
00008: 	{
00009: 		int num = 0;
00010: 		try
00011: 		{
00012: 			int num2 = -1;
00013: 			for (int i = 0; i < 16; i++)
00014: 			{
00015: 				if (SlotLock.IsLocked(pack, i))
00016: 				{
00017: 					continue;
00018: 				}
00019: 				if (StorageGrid.GetCount(pack, i) <= 0 || !NullGuard.HasText(StorageGrid.GetItemId(pack, i)))
00020: 				{
00021: 					if (num2 < 0)
00022: 					{
00023: 						num2 = i;
00024: 					}
00025: 				}
00026: 				else if (num2 >= 0 && num2 < i)
00027: 				{
00028: 					if (!StorageGrid.MoveSlot(pack, i, num2))
00029: 					{
00030: 						break;
00031: 					}
00032: 					num++;
00033: 					num2 = FindHole(pack, num2 + 1, i);
00034: 				}
00035: 			}
00036: 		}
00037: 		catch (Exception ex)
00038: 		{
00039: 			Core.Log?.Warning("收纳箱 一键整理：" + ex.Message);
00040: 		}
00041: 		return num;
00042: 	}
00043: 
00044: 	private static int FindHole(GameItem pack, int start, int endInclusive)
00045: 	{
00046: 		for (int i = start; i <= endInclusive; i++)
00047: 		{
00048: 			if (!SlotLock.IsLocked(pack, i) && StorageGrid.GetCount(pack, i) <= 0)
00049: 			{
00050: 				return i;
00051: 			}
00052: 		}
00053: 		return -1;
00054: 	}
00055: }
~~~

### ItemStorage/TagSnapshot.cs

~~~csharp
00390: 				if (StorageGrid.FindConditionFeature(item) != null)
00391: 				{
00392: 					return null;
00393: 				}
00394: 			}
00395: 			catch
00396: 			{
00397: 			}
00398: 			TagSystem state = item.state;
00399: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
00400: 			if (dictionary == null)
00401: 			{
00402: 				return null;
00403: 			}
00404: 			string itemId = Core.Clean(item.identifier);
00405: 			long num = 0L;
00406: 			try
00407: 			{
00408: 				num = item.unitValue;
00409: 			}
00410: 			catch
00411: 			{
00412: 			}
00413: 			string text = "";
00414: 			try
00415: 			{
00416: 				text = Core.Clean(item.spritePath) ?? "";
00417: 			}
00418: 			catch
00419: 			{
00420: 			}
00421: 			if (!HasIndividualState(item))
00422: 			{
00423: 				if (num == 0L)
00424: 				{
00425: 					Core.Debug("收纳箱快照：普通物品无个体状态，不拍 '" + (Core.Clean(item.identifier) ?? "?") + "'");
00426: 					return null;
00427: 				}
00428: 				Core.Debug("收纳箱快照：普通物品拍纯价快照 uv=" + num);
00429: 				return string.Empty + "\u00020\u00020\u00020\u00020\u00020\u00020\u00020\u0002\u0002" + num + "\u0002" + text;
00430: 			}
00431: 			StringBuilder stringBuilder = new StringBuilder();
00432: 			foreach (KeyValuePair<string, TagState> item2 in dictionary)
00433: 			{
00434: 				if (Excluded(item2.Key))
00435: 				{
00436: 					continue;
00437: 				}
00438: 				TagState value = item2.Value;
00439: 				if (value != null)
00440: 				{
00441: 					if (stringBuilder.Length > 0)
00442: 					{
00443: 						stringBuilder.Append('\u0003');
00444: 					}
00445: 					stringBuilder.Append(item2.Key).Append('\u0002').Append(value.IsEnabled() ? '1' : '0')
00446: 						.Append('\u0002')
00447: 						.Append(value.valueInt)
00448: 						.Append('\u0002')
00449: 						.Append(value.valueFloat.ToString("R", CultureInfo.InvariantCulture))
00450: 						.Append('\u0002')
00451: 						.Append(value.valueLong)
00452: 						.Append('\u0002')
00453: 						.Append(value.valueDouble.ToString("R", CultureInfo.InvariantCulture))
00454: 						.Append('\u0002')
00455: 						.Append(value.valueBool ? '1' : '0')
00456: 						.Append('\u0002')
00457: 						.Append(value.valueString ?? "")
00458: 						.Append('\u0002')
00459: 						.Append(num)
00460: 						.Append('\u0002')
00461: 						.Append(text);
00462: 				}
00463: 			}
00464: 			try
00465: 			{
00466: 				List<string> gameItemType = item.GetGameItemType();
00467: 				if (!SameTypes(gameItemType, GetTemplateItemTypes(itemId)))
00468: 				{
00469: 					if (stringBuilder.Length > 0)
00470: 					{
00471: 						stringBuilder.Append('\u0003');
00472: 					}
00473: 					stringBuilder.Append("__ITEM_TYPES__").Append('\u0002').Append(string.Join(",", gameItemType ?? new List<string>()));
00474: 				}
00475: 			}
00476: 			catch
00477: 			{
00478: 			}
00479: 			if (stringBuilder.Length == 0)
00480: 			{
00481: 				if (num == 0L)
00482: 				{
00483: 					return null;
00484: 				}
00485: 				stringBuilder.Append('\u0002').Append('0').Append('\u0002')
00486: 					.Append('0')
00487: 					.Append('\u0002')
00488: 					.Append('0')
00489: 					.Append('\u0002')
00490: 					.Append('0')
00491: 					.Append('\u0002')
00492: 					.Append('0')
00493: 					.Append('\u0002')
00494: 					.Append('0')
00495: 					.Append('\u0002')
00496: 					.Append('0')
00497: 					.Append('\u0002')
00498: 					.Append("")
00499: 					.Append('\u0002')
00500: 					.Append(num)
00501: 					.Append('\u0002')
00502: 					.Append(text);
00503: 			}
00504: 			int num2 = 0;
00505: 			for (int i = 0; i < stringBuilder.Length; i++)
00506: 			{
00507: 				if (stringBuilder[i] == '\u0003')
00508: 				{
00509: 					num2++;
00510: 				}
00511: 			}
00512: 			Core.Debug("收纳箱快照：Capture 产出 1 只（tag " + (num2 + 1) + " 个）uv=" + num);
00513: 			return stringBuilder.ToString();
00514: 		}
00515: 		catch (Exception ex)
00516: 		{
00517: 			Core.Debug("收纳箱 快照拍取失败：" + ex.Message);
00518: 			return null;
00519: 		}
00520: 	}
00521: 
00522: 	private static bool RestoreOne(string itemSnap, GameItem item)
00523: 	{
00524: 		bool flag = false;
00525: 		long num = -1L;
00526: 		try
00527: 		{
00528: 			string[] array = itemSnap.Split('\u0003');
00529: 			foreach (string text in array)
00530: 			{
00531: 				try
00532: 				{
00533: 					string[] array2 = text.Split('\u0002');
00534: 					if (array2.Length >= 2 && array2[0] == "__ITEM_TYPES__")
00535: 					{
00536: 						try
00537: 						{
00538: 							item.RemoveAllGameItemType();
00539: 							string text2 = array2[1];
00540: 							if (!string.IsNullOrEmpty(text2))
00541: 							{
00542: 								string[] array3 = text2.Split(',');
00543: 								foreach (string text3 in array3)
00544: 								{
00545: 									if (NullGuard.HasText(text3))
00546: 									{
00547: 										item.SetGameItemType(text3);
00548: 									}
00549: 								}
00550: 							}
00551: 							flag = true;
00552: 						}
00553: 						catch
00554: 						{
00555: 						}
00556: 					}
00557: 					else
00558: 					{
00559: 						if (array2.Length < 9)
00560: 						{
00561: 							continue;
00562: 						}
00563: 						string text4 = array2[0];
00564: 						if (Excluded(text4))
00565: 						{
00566: 							continue;
00567: 						}
00568: 						if (string.IsNullOrEmpty(text4))
00569: 						{
00570: 							goto IL_0199;
00571: 						}
00572: 						bool enabled = array2[1] == "1";
00573: 						TagSystem state = item.state;
00574: 						TagState val = ((state != null) ? state.GetTag(text4) : null);
00575: 						if (val != null)
00576: 						{
00577: 							try
00578: 							{
00579: 								val.SetEnabled(enabled);
00580: 							}
00581: 							catch
00582: 							{
00583: 							}
00584: 							try
00585: 							{
00586: 								val.valueInt = int.Parse(array2[2], CultureInfo.InvariantCulture);
~~~

~~~csharp
00655: 			}
00656: 			return flag;
00657: 		}
00658: 		catch
00659: 		{
00660: 			return false;
00661: 		}
00662: 	}
00663: 
00664: 	internal static string? PopAndRestore(GameItem pack, int slot, GameItem item)
00665: 	{
00666: 		string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00667: 		if (!NullGuard.HasText(snapshotQueue))
00668: 		{
00669: 			return null;
00670: 		}
00671: 		string text = snapshotQueue;
00672: 		bool flag = false;
00673: 		while (!string.IsNullOrEmpty(text))
00674: 		{
00675: 			int num = text.LastIndexOf('\u0001');
00676: 			string text2;
00677: 			if (num >= 0)
00678: 			{
00679: 				text2 = text.Substring(num + 1);
00680: 				text = text.Substring(0, num);
00681: 			}
00682: 			else
00683: 			{
00684: 				text2 = text;
00685: 				text = "";
00686: 			}
00687: 			if (RestoreOne(text2, item))
00688: 			{
00689: 				flag = true;
00690: 				Core.Debug("收纳箱快照：还原 1 只（tag " + text2.Split('\u0003').Length + " 个）");
00691: 				break;
00692: 			}
00693: 			Core.Log?.Warning("收纳箱 快照坏条目已跳过（key='" + ((text2.Length > 24) ? text2.Substring(0, 24) : text2) + "…'）");
00694: 		}
00695: 		if (flag)
00696: 		{
00697: 			try
00698: 			{
00699: 				item.ValidateShapeState();
00700: 			}
00701: 			catch
00702: 			{
00703: 			}
00704: 		}
00705: 		return text;
00706: 	}
00707: 
00708: 	internal static void Enqueue(GameItem pack, int slot, string snap)
00709: 	{
00710: 		try
00711: 		{
00712: 			if (string.IsNullOrEmpty(snap))
00713: 			{
00714: 				return;
00715: 			}
00716: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00717: 			string text = (string.IsNullOrEmpty(snapshotQueue) ? snap : (snapshotQueue + "\u0001" + snap));
00718: 			int num = 1;
00719: 			for (int i = 0; i < text.Length; i++)
00720: 			{
00721: 				if (text[i] == '\u0001')
00722: 				{
00723: 					num++;
00724: 				}
00725: 			}
00726: 			if (num > 2000)
00727: 			{
00728: 				Core.Log?.Warning("收纳箱 快照队列过大：槽 " + slot + " 已 " + num + " 条（软上限 " + 2000 + "，观察哨）");
00729: 			}
00730: 			StorageGrid.SetSnapshotQueue(pack, slot, text);
00731: 		}
00732: 		catch (Exception ex)
00733: 		{
00734: 			Core.Debug("收纳箱 快照入队失败：" + ex.Message);
00735: 		}
00736: 	}
00737: 
00738: 	internal static void SetQueue(GameItem pack, int slot, string? queue)
00739: 	{
00740: 		try
00741: 		{
00742: 			StorageGrid.SetSnapshotQueue(pack, slot, queue);
00743: 		}
00744: 		catch (Exception ex)
00745: 		{
00746: 			Core.Debug("收纳箱 快照写回失败：" + ex.Message);
00747: 		}
00748: 	}
00749: 
00750: 	internal static string? CaptureChemParts(GameItem item)
00751: 	{
00752: 		try
00753: 		{
00754: 			if (StorageGrid.FindConditionFeature(item) == null)
00755: 			{
00756: 				return null;
00757: 			}
00758: 			TagSystem state = item.state;
00759: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
00760: 			if (dictionary == null)
00761: 			{
00762: 				return null;
00763: 			}
00764: 			long value = 0L;
00765: 			try
00766: 			{
00767: 				value = item.unitValue;
00768: 			}
00769: 			catch
00770: 			{
00771: 			}
00772: 			StringBuilder stringBuilder = new StringBuilder();
00773: 			foreach (KeyValuePair<string, TagState> item2 in dictionary)
00774: 			{
00775: 				if (item2.Key == null || !item2.Key.StartsWith("CURRENT_PART_", StringComparison.Ordinal))
00776: 				{
00777: 					continue;
00778: 				}
00779: 				TagState value2 = item2.Value;
00780: 				if (value2 != null)
00781: 				{
00782: 					if (stringBuilder.Length > 0)
00783: 					{
00784: 						stringBuilder.Append('\u0003');
00785: 					}
00786: 					stringBuilder.Append(item2.Key).Append('\u0002').Append(value2.IsEnabled() ? '1' : '0')
00787: 						.Append('\u0002')
00788: 						.Append(value2.valueInt)
00789: 						.Append('\u0002')
00790: 						.Append(value2.valueFloat.ToString("R", CultureInfo.InvariantCulture))
00791: 						.Append('\u0002')
00792: 						.Append(value2.valueLong)
00793: 						.Append('\u0002')
00794: 						.Append(value2.valueDouble.ToString("R", CultureInfo.InvariantCulture))
00795: 						.Append('\u0002')
00796: 						.Append(value2.valueBool ? '1' : '0')
00797: 						.Append('\u0002')
00798: 						.Append(value2.valueString ?? "")
00799: 						.Append('\u0002')
00800: 						.Append(value)
00801: 						.Append('\u0002')
00802: 						.Append("");
00803: 				}
00804: 			}
00805: 			return (stringBuilder.Length == 0) ? null : stringBuilder.ToString();
00806: 		}
00807: 		catch
00808: 		{
00809: 			return null;
00810: 		}
00811: 	}
00812: 
00813: 	internal static bool EnqueueChemParts(GameItem pack, int slot, GameItem item, int put)
00814: 	{
00815: 		try
00816: 		{
00817: 			string text = CaptureChemParts(item);
00818: 			if (text == null)
00819: 			{
00820: 				return false;
00821: 			}
00822: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00823: 			StringBuilder stringBuilder = new StringBuilder(string.IsNullOrEmpty(snapshotQueue) ? "" : snapshotQueue);
00824: 			for (int i = 0; i < put; i++)
00825: 			{
00826: 				if (stringBuilder.Length > 0)
00827: 				{
00828: 					stringBuilder.Append('\u0001');
00829: 				}
00830: 				stringBuilder.Append(text);
00831: 			}
00832: 			StorageGrid.SetSnapshotQueue(pack, slot, stringBuilder.ToString());
00833: 			Core.Debug("收纳箱 deposit 组成入队：槽 " + slot + " '" + (Core.Clean(item.identifier) ?? "?") + "' x" + put + "（组成键 " + text.Split('\u0003').Length + " 项/条）");
00834: 			return true;
00835: 		}
00836: 		catch (Exception ex)
00837: 		{
00838: 			Core.Debug("收纳箱 组成入队失败：" + ex.Message);
00839: 			return false;
00840: 		}
00841: 	}
00842: 
00843: 	internal static string? PeekLastSprite(GameItem pack, int slot)
00844: 	{
00845: 		try
00846: 		{
00847: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00848: 			if (!NullGuard.HasText(snapshotQueue))
00849: 			{
00850: 				return null;
00851: 			}
00852: 			string text = snapshotQueue;
00853: 			int num = snapshotQueue.LastIndexOf('\u0001');
00854: 			if (num >= 0)
00855: 			{
00856: 				text = snapshotQueue.Substring(num + 1);
00857: 			}
00858: 			string[] array = text.Split('\u0003');
00859: 			for (int i = 0; i < array.Length; i++)
00860: 			{
00861: 				string[] array2 = array[i].Split('\u0002');
00862: 				if (array2.Length >= 10 && !string.IsNullOrEmpty(array2[^1]))
00863: 				{
00864: 					return array2[^1];
00865: 				}
00866: 			}
00867: 		}
00868: 		catch
00869: 		{
00870: 		}
00871: 		return null;
00872: 	}
00873: 
00874: 	internal static (string speciesId, string sexe, int age, float weight)? PeekLastAnimal(GameItem pack, int slot)
00875: 	{
00876: 		try
00877: 		{
00878: 			string snapshotQueue = StorageGrid.GetSnapshotQueue(pack, slot);
00879: 			if (!NullGuard.HasText(snapshotQueue))
00880: 			{
00881: 				return null;
00882: 			}
00883: 			string text = snapshotQueue;
00884: 			int num = snapshotQueue.LastIndexOf('\u0001');
00885: 			if (num >= 0)
00886: 			{
00887: 				text = snapshotQueue.Substring(num + 1);
00888: 			}
00889: 			string text2 = null;
00890: 			string text3 = null;
00891: 			int result = -1;
00892: 			float result2 = -1f;
00893: 			string[] array = text.Split('\u0003');
00894: 			for (int i = 0; i < array.Length; i++)
00895: 			{
00896: 				string[] array2 = array[i].Split('\u0002');
00897: 				if (array2.Length >= 8)
00898: 				{
00899: 					switch (array2[0])
00900: 					{
00901: 					case "ANIMAL_SPECIES_TAG":
00902: 						text2 = array2[7];
00903: 						break;
00904: 					case "ANIMAL_SEXE_TAG":
00905: 						text3 = array2[7];
00906: 						break;
00907: 					case "ANIMAL_AGE_TAG":
~~~

### ItemStorage/StorageGrid.cs

~~~csharp
01760: 	internal static int GetBoxOption(GameItem pack, string key, int def = 0)
01761: 	{
01762: 		try
01763: 		{
01764: 			TagSystem state = pack.state;
01765: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
01766: 			string key2 = OptTag(key);
01767: 			if (dictionary == null || !dictionary.ContainsKey(key2))
01768: 			{
01769: 				return def;
01770: 			}
01771: 			return dictionary[key2]?.valueInt ?? def;
01772: 		}
01773: 		catch
01774: 		{
01775: 			return def;
01776: 		}
01777: 	}
01778: 
01779: 	internal static void SetBoxOption(GameItem pack, string key, int value)
01780: 	{
01781: 		try
01782: 		{
01783: 			string tag = OptTag(key);
01784: 			if (value == 0)
01785: 			{
01786: 				DestroyTag(pack, tag);
01787: 			}
01788: 			else
01789: 			{
01790: 				WriteCountTag(pack, tag, value);
01791: 			}
01792: 		}
01793: 		catch (Exception ex)
01794: 		{
01795: 			Core.Log?.Warning("收纳箱 SetBoxOption(" + key + "=" + value + ")：" + ex.Message);
01796: 		}
01797: 	}
01798: 
01799: 	internal static void SetBoxOptionRaw(GameItem pack, string key, int value)
01800: 	{
01801: 		try
01802: 		{
01803: 			WriteCountTag(pack, OptTag(key), value);
01804: 		}
01805: 		catch (Exception ex)
01806: 		{
01807: 			Core.Log?.Warning("收纳箱 SetBoxOptionRaw(" + key + "=" + value + ")：" + ex.Message);
01808: 		}
01809: 	}
01810: 
01811: 	internal static string? GetSlotLock(GameItem pack, int slot)
01812: 	{
01813: 		if (!IsValidSlot(slot))
01814: 		{
01815: 			return null;
01816: 		}
01817: 		try
01818: 		{
01819: 			TagSystem state = pack.state;
01820: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
01821: 			if (dictionary == null || !dictionary.ContainsKey(LockTag(slot)))
01822: 			{
01823: 				return null;
01824: 			}
01825: 			TagState val = dictionary[LockTag(slot)];
01826: 			if (val == null)
01827: 			{
01828: 				return null;
01829: 			}
01830: 			string text = Core.Clean(val.valueString);
01831: 			return string.IsNullOrEmpty(text) ? null : text;
01832: 		}
01833: 		catch
01834: 		{
01835: 			return null;
01836: 		}
01837: 	}
01838: 
01839: 	internal static void SetSlotLock(GameItem pack, int slot, string? itemId)
01840: 	{
01841: 		if (IsValidSlot(slot))
01842: 		{
01843: 			string value = Core.Clean(itemId);
01844: 			if (string.IsNullOrEmpty(value))
01845: 			{
01846: 				DestroyTag(pack, LockTag(slot));
01847: 			}
01848: 			else
01849: 			{
01850: 				WriteTag(pack, LockTag(slot), value);
01851: 			}
01852: 		}
01853: 	}
01854: 
01855: 	internal static bool MoveSlotTo(GameItem src, int s, GameItem dst, int d)
01856: 	{
01857: 		if (!IsValidSlot(s) || !IsValidSlot(d))
01858: 		{
01859: 			return false;
01860: 		}
01861: 		if (string.IsNullOrEmpty(GetItemId(src, s)))
01862: 		{
01863: 			return false;
01864: 		}
01865: 		if (!string.IsNullOrEmpty(GetItemId(dst, d)))
01866: 		{
01867: 			return false;
01868: 		}
01869: 		TagSystem state = src.state;
01870: 		Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
01871: 		TagSystem state2 = dst.state;
01872: 		Dictionary<string, TagState> dictionary2 = ((state2 != null) ? state2.dict : null);
01873: 		if (dictionary == null || dictionary2 == null)
01874: 		{
01875: 			return false;
01876: 		}
~~~

~~~csharp
01993: 			}
01994: 			string snapshotQueue = GetSnapshotQueue(src, s);
01995: 			if (!string.IsNullOrEmpty(snapshotQueue))
01996: 			{
01997: 				var (text3, text4) = TagSnapshot.SplitTail(snapshotQueue, num2);
01998: 				if (!string.IsNullOrEmpty(text4))
01999: 				{
02000: 					string snapshotQueue2 = GetSnapshotQueue(dst, d);
02001: 					SetSnapshotQueue(dst, d, string.IsNullOrEmpty(snapshotQueue2) ? text4 : (snapshotQueue2 + "\u0001" + text4));
02002: 				}
02003: 				SetSnapshotQueue(src, s, string.IsNullOrEmpty(text3) ? null : text3);
02004: 			}
02005: 			int num3 = GetEnergyPool(dst, d) + GetEnergyPool(src, s);
02006: 			if (num3 > 0)
02007: 			{
02008: 				WriteCountTag(dst, EnTag(d), num3);
02009: 			}
02010: 			int num4 = GetDurabilityPool(dst, d) + GetDurabilityPool(src, s);
02011: 			if (num4 > 0)
02012: 			{
02013: 				WriteCountTag(dst, DuTag(d), num4);
02014: 			}
02015: 			int num5 = GetUseCountPool(dst, d) + GetUseCountPool(src, s);
02016: 			if (num5 > 0)
02017: 			{
02018: 				WriteCountTag(dst, UseTag(d), num5);
02019: 			}
02020: 			int useCountMax = GetUseCountMax(src, s);
02021: 			int useCountMax2 = GetUseCountMax(dst, d);
02022: 			if (useCountMax2 == 0 && useCountMax > 0)
02023: 			{
02024: 				WriteCountTag(dst, UseMaxTag(d), useCountMax);
02025: 			}
02026: 			else if (useCountMax > 0 && useCountMax2 > 0 && useCountMax != useCountMax2)
02027: 			{
02028: 				Core.Debug("收纳箱合并：次数上限不一致（源 " + useCountMax + " / 目标 " + useCountMax2 + "），保留目标格值。");
02029: 			}
02030: 			int energyMax = GetEnergyMax(src, s);
02031: 			int energyMax2 = GetEnergyMax(dst, d);
02032: 			if (energyMax2 == 0 && energyMax > 0)
02033: 			{
02034: 				WriteCountTag(dst, EnMaxTag(d), energyMax);
02035: 			}
02036: 			else if (energyMax > 0 && energyMax2 > 0 && energyMax != energyMax2)
02037: 			{
02038: 				Core.Debug("收纳箱合并：电量上限不一致（源 " + energyMax + " / 目标 " + energyMax2 + "），保留目标格值。");
02039: 			}
02040: 			WriteCountTag(dst, CntTag(d), GetCount(dst, d) + num2);
02041: 			int num6 = count - num2;
02042: 			if (num6 <= 0)
02043: 			{
02044: 				ClearSlot(src, s);
02045: 			}
02046: 			else
02047: 			{
02048: 				WriteCountTag(src, CntTag(s), num6);
02049: 			}
02050: 			return true;
02051: 		}
02052: 		catch (Exception ex)
02053: 		{
02054: 			Core.Log?.Warning("StorageGrid.MergeSlots(" + s + "→" + d + ")：" + ex.Message);
02055: 			return false;
02056: 		}
02057: 	}
02058: 
02059: 	internal static List<int> EmptySlots(GameItem pack)
02060: 	{
02061: 		List<int> list = new List<int>();
02062: 		for (int i = 0; i < 16; i++)
02063: 		{
02064: 			if (string.IsNullOrEmpty(GetItemId(pack, i)))
02065: 			{
02066: 				list.Add(i);
02067: 			}
02068: 		}
02069: 		return list;
02070: 	}
02071: }
~~~

### ItemStorage/BoxTransfer.cs

~~~csharp
00062: 			TryRegister(dictionary, () => ItemConditionList.CreateExpiredInjector(), delegate(GameItem it)
00063: 			{
00064: 				InsInjectorHelper.SetExpired(it);
00065: 			}, isOfficialHelper: true, () => PreBuiltItemHelper.CreateRealExpiredInjector());
00066: 			TryRegister(dictionary, () => ItemConditionList.CreateUnusableInjector(), delegate(GameItem it)
00067: 			{
00068: 				InsInjectorHelper.SetUnusable(it);
00069: 			}, isOfficialHelper: true);
00070: 			TryRegister(dictionary, () => ItemConditionList.CreateGenuineCigarette(), delegate(GameItem it)
00071: 			{
00072: 				InsCigaretteHelper.SetGenuine(it);
00073: 			}, isOfficialHelper: true, LaunderGenuineCigarette);
00074: 			TryRegister(dictionary, () => ItemConditionList.CreateCounterfeitCigarette(), delegate(GameItem it)
00075: 			{
00076: 				ItemCondition val = ItemConditionList.CreateCounterfeitCigarette();
00077: 				ItemFeature val2 = StorageGrid.FindConditionFeature(it);
00078: 				if (val2 != null)
00079: 				{
00080: 					val2.SetConditions(val, val);
00081: 				}
00082: 			}, isOfficialHelper: false, () => PreBuiltItemHelper.CounterfeitCigarette());
00083: 			_condMap = dictionary;
00084: 			return dictionary;
00085: 		}
00086: 	}
00087: 
00088: 	internal static void PollAlt()
00089: 	{
00090: 		try
00091: 		{
00092: 			_altState = ((Input.GetKey((KeyCode)308) || Input.GetKey((KeyCode)307)) ? 1 : 0);
00093: 		}
00094: 		catch (Exception ex)
00095: 		{
00096: 			_altState = 0;
00097: 			Core.Log?.Warning("收纳箱 Alt 轮询失败：" + ex.Message);
00098: 		}
00099: 	}
00100: 
00101: 	internal static bool AltHeld()
00102: 	{
00103: 		if (_altState >= 0)
00104: 		{
00105: 			return _altState == 1;
00106: 		}
00107: 		try
00108: 		{
00109: 			return Input.GetKey((KeyCode)308) || Input.GetKey((KeyCode)307);
00110: 		}
00111: 		catch (Exception ex)
00112: 		{
00113: 			Core.Log?.Warning("收纳箱 Alt 读取失败：" + ex.Message);
00114: 			return false;
00115: 		}
00116: 	}
00117: 
00118: 	internal static PlayerStore? PeekStore()
00119: 	{
00120: 		try
00121: 		{
00122: 			object? obj = AccessTools.Field(typeof(PlayerStore), "instance")?.GetValue(null);
00123: 			return (PlayerStore?)((obj is PlayerStore) ? obj : null);
00124: 		}
00125: 		catch
00126: 		{
00127: 			return null;
00128: 		}
00129: 	}
00130: 
00131: 	internal static bool IsInStore()
00132: 	{
00133: 		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
00134: 		try
00135: 		{
00136: 			PlayerStore val = PeekStore();
00137: 			if (val == null)
00138: 			{
00139: 				return false;
00140: 			}
00141: 			if ((int)val.playerLocation != 0)
00142: 			{
00143: 				return false;
00144: 			}
00145: 		}
00146: 		catch
00147: 		{
00148: 		}
00149: 		try
00150: 		{
00151: 			EmporiumEntry instance = EmporiumEntry.Instance;
00152: 			GameGridInventory val2 = ((instance != null) ? instance.backInvinvElement : null);
00153: 			if (val2 == null)
00154: 			{
00155: 				return false;
00156: 			}
00157: 			TreeNodeRender val3 = ((val2 != null) ? ((PixelElement)val2).handler : null);
00158: 			GameObject val4 = (((Object)(object)val3 != (Object)null) ? ((Component)val3).gameObject : null);
00159: 			if ((Object)(object)val4 == (Object)null || !val4.activeInHierarchy)
00160: 			{
00161: 				return false;
00162: 			}
00163: 		}
00164: 		catch
00165: 		{
00166: 		}
00167: 		return true;
00168: 	}
00169: 
00170: 	internal unsafe static GameGridInventory? ResolveCounterDrop()
00171: 	{
00172: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00173: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00174: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00175: 		try
00176: 		{
00177: 			try
00178: 			{
00179: 				PlayerLocation playerLocation = PlayerStore.Instance.playerLocation;
00180: 				if ((int)playerLocation != 0)
00181: 				{
00182: 					Core.Log?.Warning("收纳箱：玩家不在店内（" + ((object)(*(PlayerLocation*)(&playerLocation))/*cast due to .constrained prefix*/).ToString() + "），Alt 直上柜台不可用，本次走原落点");
00183: 					return null;
00184: 				}
00185: 			}
00186: 			catch
00187: 			{
00188: 			}
00189: 			EmporiumEntry instance = EmporiumEntry.Instance;
00190: 			if ((Object)(object)instance == (Object)null)
00191: 			{
00192: 				Core.Log?.Warning("收纳箱：Alt 已按下但店铺场景未就绪，本次走原落点");
00193: 				return null;
00194: 			}
00195: 			GameGridInventory backInvinvElement = instance.backInvinvElement;
00196: 			if (backInvinvElement == null)
00197: 			{
00198: 				Core.Log?.Warning("收纳箱：柜台面 backInvinvElement 不可用（场景未就绪？），本次走原落点");
00199: 				return null;
00200: 			}
00201: 			try
00202: 			{
00203: 				TreeNodeRender val = ((backInvinvElement != null) ? ((PixelElement)backInvinvElement).handler : null);
00204: 				GameObject val2 = (((Object)(object)val != (Object)null) ? ((Component)val).gameObject : null);
00205: 				if ((Object)(object)val2 == (Object)null || !val2.activeInHierarchy)
00206: 				{
00207: 					Core.Log?.Warning("收纳箱：柜台窗口未激活（玩家不在店内），Alt 直上柜台不可用，本次走原落点");
00208: 					return null;
00209: 				}
00210: 			}
00211: 			catch
00212: 			{
00213: 			}
00214: 			return backInvinvElement;
00215: 		}
00216: 		catch (Exception ex)
00217: 		{
00218: 			Core.Log?.Warning("收纳箱：柜台网格读取失败（" + ex.Message + "），本次走原落点");
00219: 			return null;
00220: 		}
00221: 	}
00222: 
00223: 	internal static bool TryDeposit(GameItem pack, GameItem dragged, int context, int preferredSlot = -1, bool consume = true, bool refresh = true, bool notify = true)
00224: 	{
00225: 		if (pack == null || dragged == null)
00226: 		{
00227: 			return false;
00228: 		}
00229: 		if (!BoxItem.IsBox(pack))
00230: 		{
00231: 			return false;
00232: 		}
00233: 		if (!BoxItem.IsUsableByPlayer(pack))
00234: 		{
00235: 			Core.Debug("收纳箱 deposit：目标收纳箱非玩家所有，拒绝存入");
00236: 			if (notify)
00237: 			{
00238: 				BoxItem.NotifyUnowned();
00239: 			}
00240: 			return false;
00241: 		}
00242: 		try
00243: 		{
00244: 			if (!GeneralHelper.IsItemOwned(dragged))
00245: 			{
00246: 				Core.Debug("收纳箱 deposit：物品非玩家所有（未购买的商品），拒绝存入");
00247: 				if (notify)
00248: 				{
00249: 					BoxItem.NotifyUser("请先购买后再存");
00250: 				}
00251: 				return false;
00252: 			}
00253: 		}
00254: 		catch
00255: 		{
00256: 		}
00257: 		string text = Core.Clean(dragged.identifier) ?? string.Empty;
00258: 		if (string.IsNullOrEmpty(text))
00259: 		{
00260: 			return false;
00261: 		}
00262: 		bool flag = true;
00263: 		try
00264: 		{
00265: 			flag = DirectoryMaster.Has<GameItem>(text);
00266: 		}
00267: 		catch
00268: 		{
00269: 			flag = true;
00270: 		}
00271: 		if (!flag)
00272: 		{
00273: 			Core.Debug("收纳箱 deposit：'" + text + "' 不在物品目录中（对应 MOD 可能未加载），拒绝存入");
00274: 			if (notify)
00275: 			{
00276: 				BoxItem.NotifyUser("该物品所属模组未加载，无法存入");
00277: 			}
00278: 			return false;
00279: 		}
00280: 		try
00281: 		{
00282: 			bool? flag2 = BoxItem.IsModuleItem(dragged);
00283: 			if (flag2 == true)
00284: 			{
00285: 				Core.Debug("收纳箱 deposit：'" + text + "' 带【模组】标签（MODULE 类型），拒绝存入");
00286: 				if (notify)
00287: 				{
00288: 					BoxItem.NotifyUser("模组类物品请勿存进收纳箱");
00289: 				}
00290: 				return false;
00291: 			}
00292: 			if (flag2 == false && text.IndexOf("module", StringComparison.OrdinalIgnoreCase) >= 0)
00293: 			{
00294: 				try
00295: 				{
00296: 					List<string> itemTypes = dragged.itemTypes;
00297: 					Core.Debug("收纳箱 deposit 侦查：'" + text + "' 未判中 MODULE，itemTypes=" + ((itemTypes != null) ? itemTypes.ToString() : "null"));
00298: 				}
00299: 				catch
00300: 				{
00301: 				}
00302: 			}
00303: 		}
00304: 		catch
00305: 		{
00306: 		}
00307: 		try
00308: 		{
00309: 			bool? flag3 = BoxItem.IsSupplyCrateItem(dragged);
00310: 			if (flag3 == true)
00311: 			{
00312: 				bool flag4 = true;
00313: 				try
00314: 				{
00315: 					flag4 = dragged.IsGameItemType("SUPPLY_CRATE");
00316: 				}
00317: 				catch
00318: 				{
00319: 					flag4 = true;
00320: 				}
00321: 				if (flag4)
00322: 				{
00323: 					Core.Debug("收纳箱 deposit：'" + text + "' 是锁定物资箱（SUPPLY_CRATE 类型在），拒绝存入");
00324: 					if (notify)
00325: 					{
00326: 						BoxItem.NotifyUser("物资箱请先刷开清空再存");
00327: 					}
00328: 					return false;
00329: 				}
00330: 				Core.Debug("收纳箱 deposit：'" + text + "' 是刷开物资箱（仅 tag 残留），准入交含物闸");
00331: 			}
00332: 			if (flag3 == false && (text.IndexOf("crate", StringComparison.OrdinalIgnoreCase) >= 0 || text.IndexOf("_box", StringComparison.OrdinalIgnoreCase) >= 0))
00333: 			{
00334: 				try
00335: 				{
00336: 					List<string> itemTypes2 = dragged.itemTypes;
00337: 					Core.Debug("收纳箱 deposit 侦查：'" + text + "' 未判中 SUPPLY_CRATE，itemTypes=" + ((itemTypes2 != null) ? itemTypes2.ToString() : "null"));
00338: 				}
00339: 				catch
00340: 				{
00341: 				}
00342: 			}
00343: 		}
00344: 		catch
00345: 		{
00346: 		}
00347: 		try
00348: 		{
00349: 			bool? flag5 = BoxItem.IsNodeItem(dragged);
00350: 			if (flag5 == true)
00351: 			{
00352: 				Core.Debug("收纳箱 deposit：'" + text + "' 带【节点】标签（NODE 类型），拒绝存入");
00353: 				if (notify)
00354: 				{
00355: 					BoxItem.NotifyUser("节点类物品请勿存进收纳箱");
00356: 				}
00357: 				return false;
00358: 			}
00359: 			if (flag5 == false && text.IndexOf("node", StringComparison.OrdinalIgnoreCase) >= 0)
00360: 			{
00361: 				try
00362: 				{
00363: 					List<string> itemTypes3 = dragged.itemTypes;
00364: 					Core.Debug("收纳箱 deposit 侦查：'" + text + "' 未判中 NODE，itemTypes=" + ((itemTypes3 != null) ? itemTypes3.ToString() : "null"));
00365: 				}
00366: 				catch
00367: 				{
00368: 				}
00369: 			}
00370: 		}
00371: 		catch
00372: 		{
00373: 		}
00374: 		try
00375: 		{
00376: 			if (dragged.IsGameItemType("STORAGE"))
00377: 			{
00378: 				Core.Debug("收纳箱 deposit：储藏装置（STORAGE）不能存进收纳箱");
00379: 				if (notify)
00380: 				{
00381: 					BoxItem.NotifyUser("储藏装置请勿放进收纳箱");
00382: 				}
00383: 				return false;
00384: 			}
00385: 		}
00386: 		catch
00387: 		{
00388: 		}
00389: 		if (string.Equals(text, "item_storage", StringComparison.Ordinal))
00390: 		{
00391: 			Core.Debug("收纳箱 deposit：容器不能存进收纳箱");
00392: 			if (notify)
00393: 			{
00394: 				BoxItem.NotifyUser("储藏装置请勿放进收纳箱");
00395: 			}
00396: 			return false;
00397: 		}
00398: 		try
00399: 		{
00400: 			if (dragged.IsGameItemType("MACHINE"))
00401: 			{
00402: 				Core.Debug("收纳箱 deposit：机器不能存进收纳箱");
00403: 				if (notify)
00404: 				{
00405: 					BoxItem.NotifyUser("机器请勿存进收纳箱");
00406: 				}
00407: 				return false;
00408: 			}
00409: 		}
00410: 		catch
00411: 		{
00412: 		}
00413: 		try
00414: 		{
00415: 			if (dragged.IsTag("TOOLBOX_TAG"))
00416: 			{
00417: 				Core.Debug("收纳箱 deposit：工具箱不能存进收纳箱");
00418: 				if (notify)
00419: 				{
00420: 					BoxItem.NotifyUser("工具箱请勿收进收纳箱");
00421: 				}
00422: 				return false;
00423: 			}
00424: 		}
00425: 		catch
00426: 		{
00427: 		}
00428: 		try
00429: 		{
00430: 			if (StolenHelper.IsStolenItem(dragged))
00431: 			{
00432: 				Core.Debug("收纳箱 deposit：赃物（带热度标记）不能存进收纳箱");
00433: 				if (notify)
00434: 				{
00435: 					BoxItem.NotifyUser("赃物请勿存进收纳箱");
00436: 				}
00437: 				return false;
00438: 			}
00439: 		}
00440: 		catch
00441: 		{
00442: 		}
00443: 		try
00444: 		{
00445: 			if (WineHelper.IsFermentingOrFinished(dragged))
00446: 			{
00447: 				Core.Log?.Msg("收纳箱 deposit：酿造中/陈酿的酒不能存进收纳箱 id=" + text);
00448: 				if (notify)
00449: 				{
00450: 					BoxItem.NotifyUser("酿造中或陈酿的酒不能存");
00451: 				}
00452: 				return false;
00453: 			}
00454: 		}
00455: 		catch
00456: 		{
00457: 		}
00458: 		try
00459: 		{
00460: 			bool flag6 = false;
00461: 			try
00462: 			{
00463: 				flag6 = WaterFeatureHelper.GetWaterFeature(dragged) != null;
00464: 			}
00465: 			catch
00466: 			{
00467: 			}
00468: 			bool flag7 = true;
00469: 			try
00470: 			{
00471: 				flag7 = WaterHelper.IsEmpty(dragged);
00472: 			}
00473: 			catch
00474: 			{
00475: 			}
00476: 			if (flag6 && !flag7 && preferredSlot < 0)
00477: 			{
00478: 				Core.Log?.Msg("收纳箱 deposit：含水水容器拒绝一键收纳（拖到面板格子松手可存）id=" + text);
00479: 				if (notify)
00480: 				{
00481: 					BoxItem.NotifyUser("含水容器请拖到面板格子存放");
00482: 				}
00483: 				return false;
00484: 			}
00485: 		}
00486: 		catch
00487: 		{
00488: 		}
00489: 		try
00490: 		{
00491: 			if (HasContentInside(dragged))
00492: 			{
00493: 				Core.Log?.Msg("收纳箱 deposit：容器内有物品，拒收（内容物保护）id=" + text);
00494: 				if (notify)
00495: 				{
00496: 					BoxItem.NotifyUser("物品内部还放着东西，请先清空再存");
00497: 				}
00498: 				return false;
00499: 			}
00500: 		}
00501: 		catch
00502: 		{
00503: 		}
00504: 		try
00505: 		{
00506: 			RealizeTruthBeforeDeposit(dragged);
00507: 		}
00508: 		catch (Exception ex)
00509: 		{
00510: 			Core.Debug("收纳箱 deposit 实化外层异常：" + ex.Message);
00511: 		}
00512: 		int num = 1;
00513: 		try
00514: 		{
00515: 			num = Math.Max(1, dragged.unitCount);
00516: 		}
00517: 		catch
00518: 		{
00519: 		}
00520: 		string text2;
00521: 		try
00522: 		{
00523: 			text2 = ReadItemSignature(dragged);
00524: 		}
00525: 		catch
00526: 		{
00527: 			text2 = "";
00528: 		}
00529: 		try
00530: 		{
00531: 			if (StorageGrid.ReadItemEnergy(dragged).HasValue)
00532: 			{
00533: 				text2 = null;
00534: 			}
00535: 		}
00536: 		catch
00537: 		{
00538: 		}
00539: 		try
00540: 		{
00541: 			if (StorageGrid.ReadItemUseCount(dragged).isUse)
00542: 			{
00543: 				text2 = null;
00544: 			}
00545: 		}
00546: 		catch
00547: 		{
00548: 		}
00549: 		bool flag8 = false;
00550: 		try
00551: 		{
00552: 			if (TagSnapshot.HasIndividualState(dragged))
00553: 			{
00554: 				text2 = null;
00555: 				flag8 = true;
00556: 			}
00557: 		}
00558: 		catch
00559: 		{
00560: 		}
00561: 		bool flag9 = false;
00562: 		try
00563: 		{
00564: 			flag9 = TagSnapshot.IsDead(dragged);
00565: 		}
00566: 		catch
00567: 		{
00568: 		}
00569: 		string text3 = null;
00570: 		try
00571: 		{
00572: 			text3 = Core.Clean(dragged.spritePath);
00573: 		}
00574: 		catch
00575: 		{
00576: 		}
00577: 		string text4 = null;
00578: 		try
00579: 		{
00580: 			text4 = TryReadDepositName(dragged, text);
00581: 		}
00582: 		catch
00583: 		{
00584: 		}
00585: 		bool flag10 = false;
00586: 		try
00587: 		{
00588: 			flag10 = StorageGrid.ReadItemEnergy(dragged).HasValue;
00589: 		}
00590: 		catch
00591: 		{
00592: 		}
00593: 		string text5 = null;
00594: 		try
00595: 		{
00596: 			text5 = TryReadCompSignature(dragged);
00597: 		}
00598: 		catch
00599: 		{
00600: 		}
00601: 		if (context > 0)
00602: 		{
00603: 			try
00604: 			{
00605: 				BoxWindow.EnsureWindow(pack, "Deposit");
00606: 			}
00607: 			catch (Exception ex2)
00608: 			{
00609: 				Core.Log?.Warning("收纳箱 EnsureWindow deposit：" + ex2.Message);
00610: 			}
00611: 		}
00612: 		int num2 = num;
00613: 		int num3 = 0;
00614: 		try
00615: 		{
00616: 			string itemId;
00617: 			int count;
00618: 			if (StorageGrid.IsValidSlot(preferredSlot))
00619: 			{
00620: 				itemId = StorageGrid.GetItemId(pack, preferredSlot);
00621: 				count = StorageGrid.GetCount(pack, preferredSlot);
00622: 				if (itemId == null)
00623: 				{
00624: 					goto IL_0694;
00625: 				}
00626: 				if (string.Equals(itemId, text, StringComparison.Ordinal))
00627: 				{
00628: 					bool num4;
00629: 					if (text2 != null)
00630: 					{
00631: 						num4 = string.Equals(StorageGrid.GetSlotSignature(pack, preferredSlot), text2, StringComparison.Ordinal);
00632: 					}
00633: 					else
00634: 					{
00635: 						if (!flag8 || StorageGrid.GetSlotDeadFlag(pack, preferredSlot) != flag9 || (!flag10 && !string.Equals(StorageGrid.GetSpritePath(pack, preferredSlot), text3, StringComparison.Ordinal)) || !string.Equals(StorageGrid.GetDisplayName(pack, preferredSlot), text4, StringComparison.Ordinal))
00636: 						{
00637: 							goto IL_0a0f;
00638: 						}
00639: 						num4 = string.Equals(StorageGrid.GetCompSig(pack, preferredSlot) ?? "", text5 ?? "", StringComparison.Ordinal);
00640: 					}
00641: 					if (num4)
00642: 					{
00643: 						goto IL_0694;
00644: 					}
00645: 				}
00646: 			}
00647: 			goto IL_0a0f;
00648: 			IL_0694:
00649: 			if (count < 99)
00650: 			{
00651: 				int num5 = Math.Min(99 - count, num2);
00652: 				if (num5 > 0)
00653: 				{
00654: 					StorageGrid.SetSlot(pack, preferredSlot, text, count + num5);
00655: 					if (itemId == null)
00656: 					{
00657: 						StorageGrid.SetDisplayName(pack, preferredSlot, TryReadDepositName(dragged, text));
00658: 						StorageGrid.SetCompSig(pack, preferredSlot, text5);
00659: 						long? unitValue = TryReadUnitValue(dragged);
00660: 						StorageGrid.SetUnitValue(pack, preferredSlot, unitValue);
00661: 						StorageGrid.SetCalorie(pack, preferredSlot, StorageGrid.ReadItemCalorie(dragged));
00662: 						StorageGrid.SetShape(pack, preferredSlot, StorageGrid.ReadItemShape(dragged));
00663: 						StorageGrid.SetSpriteRef(pack, preferredSlot, TryReadSpriteAtlas(dragged), TryReadSpritePath(dragged));
00664: 						StorageGrid.SetTypes(pack, preferredSlot, StorageGrid.ReadItemTypes(dragged));
00665: 						Core.Debug("收纳箱 deposit 贴图记录：en=" + (StorageGrid.ReadItemEnergy(dragged)?.ToString() ?? "无") + " atlas='" + (TryReadSpriteAtlas(dragged) ?? "-") + "' path='" + (TryReadSpritePath(dragged) ?? "-") + "'");
00666: 						StorageGrid.SetPurity(pack, preferredSlot, IsEmptyWaterItem(dragged) ? null : StorageGrid.ReadItemPurity(dragged));
00667: 						StorageGrid.SetSlotCustomName(pack, preferredSlot, (WaterFeatureHelper.GetWaterFeature(dragged) != null) ? Core.Clean(GeneralHelper.GetCustomName(dragged)) : null);
00668: 						InitEnergySlotDisplay(pack, preferredSlot, dragged, text);
00669: 						StorageGrid.SetSnapshotQueue(pack, preferredSlot, null);
00670: 						StorageGrid.SetSlotDeadFlag(pack, preferredSlot, flag9);
00671: 						StorageGrid.SetCrateOpenFlag(pack, preferredSlot, BoxItem.IsSupplyCrateItem(dragged) == true);
00672: 					}
00673: 					DepositPool(pack, preferredSlot, dragged, num5);
00674: 					num2 -= num5;
00675: 					num3 += num5;
00676: 				}
00677: 			}
00678: 			goto IL_0a0f;
00679: 			IL_0a0f:
00680: 			while (num2 > 0)
00681: 			{
00682: 				int num6 = (flag8 ? StorageGrid.FindIndividualSlot(pack, text, flag9, text4, text3, !flag10, text5) : StorageGrid.FindSlotWithSpace(pack, text, text2));
00683: 				if (num6 < 0)
00684: 				{
00685: 					num6 = StorageGrid.FindEmptySlot(pack);
00686: 				}
00687: 				if (num6 >= 0)
00688: 				{
00689: 					int count2 = StorageGrid.GetCount(pack, num6);
00690: 					int num7 = Math.Min(99 - count2, num2);
00691: 					if (num7 > 0)
00692: 					{
00693: 						StorageGrid.SetSlot(pack, num6, text, count2 + num7);
00694: 						if (count2 <= 0)
00695: 						{
00696: 							StorageGrid.SetSlotDeadFlag(pack, num6, flag9);
00697: 							StorageGrid.SetCompSig(pack, num6, text5);
00698: 							StorageGrid.SetDisplayName(pack, num6, TryReadDepositName(dragged, text));
00699: 							long? unitValue2 = TryReadUnitValue(dragged);
00700: 							StorageGrid.SetUnitValue(pack, num6, unitValue2);
00701: 							StorageGrid.SetCalorie(pack, num6, StorageGrid.ReadItemCalorie(dragged));
00702: 							StorageGrid.SetShape(pack, num6, StorageGrid.ReadItemShape(dragged));
00703: 							StorageGrid.SetSpriteRef(pack, num6, TryReadSpriteAtlas(dragged), TryReadSpritePath(dragged));
00704: 							StorageGrid.SetTypes(pack, num6, StorageGrid.ReadItemTypes(dragged));
00705: 							Core.Debug("收纳箱 deposit 贴图记录：en=" + (StorageGrid.ReadItemEnergy(dragged)?.ToString() ?? "无") + " atlas='" + (TryReadSpriteAtlas(dragged) ?? "-") + "' path='" + (TryReadSpritePath(dragged) ?? "-") + "'");
00706: 							StorageGrid.SetPurity(pack, num6, IsEmptyWaterItem(dragged) ? null : StorageGrid.ReadItemPurity(dragged));
00707: 							StorageGrid.SetSlotCustomName(pack, num6, (WaterFeatureHelper.GetWaterFeature(dragged) != null) ? Core.Clean(GeneralHelper.GetCustomName(dragged)) : null);
00708: 							InitEnergySlotDisplay(pack, num6, dragged, text);
00709: 							StorageGrid.SetSnapshotQueue(pack, num6, null);
00710: 							StorageGrid.SetCrateOpenFlag(pack, num6, BoxItem.IsSupplyCrateItem(dragged) == true);
00711: 						}
00712: 						DepositPool(pack, num6, dragged, num7);
00713: 						num2 -= num7;
00714: 						num3 += num7;
00715: 						continue;
00716: 					}
00717: 					break;
00718: 				}
00719: 				break;
00720: 			}
00721: 		}
00722: 		catch (Exception ex3)
00723: 		{
00724: 			Core.Log?.Warning("收纳箱 deposit 分配异常：" + ex3.Message);
00725: 		}
00726: 		if (num3 <= 0)
00727: 		{
00728: 			Core.Debug("收纳箱 deposit '" + text + "'：格子已满或无法分配，未存入");
00729: 			if (notify)
00730: 			{
00731: 				BoxItem.NotifyUser("收纳箱满了");
00732: 			}
00733: 			return false;
00734: 		}
00735: 		int num8 = StorageGrid.TotalCount(pack, text);
00736: 		if (consume)
00737: 		{
00738: 			ConsumeDragged(dragged, num3, num);
00739: 		}
00740: 		if (refresh)
00741: 		{
00742: 			BoxWindow.RefreshButtons(pack);
00743: 			BoxWindow.UpdateTooltip(pack);
00744: 		}
00745: 		Core.Debug("收纳箱 deposit '" + text + "' +" + num3 + " → 全盒 " + num8 + "（手上剩 " + num2 + "）");
00746: 		return true;
00747: 	}
00748: 
00749: 	internal static string SpaceShort(bool counter)
00750: 	{
00751: 		if (!counter)
00752: 		{
00753: 			return "箱子所在空间不足";
00754: 		}
00755: 		return "柜台空间不足";
00756: 	}
00757: 
00758: 	internal static WithdrawResult TryWithdraw(GameItem pack, int slot, GameGridInventory? dropGrid = null, bool refresh = true)
00759: 	{
00760: 		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
00761: 		if (pack == null || !StorageGrid.IsValidSlot(slot))
00762: 		{
00763: 			return WithdrawResult.Empty;
00764: 		}
00765: 		if (!BoxItem.IsBox(pack))
00766: 		{
00767: 			return WithdrawResult.Empty;
00768: 		}
00769: 		if (dropGrid != null)
00770: 		{
00771: 			bool flag = false;
00772: 			try
00773: 			{
00774: 				flag = !IsInStore();
00775: 			}
00776: 			catch
00777: 			{
00778: 			}
00779: 			if (flag)
00780: 			{
00781: 				BoxItem.NotifyUser("店外无法取出到柜台，请回到店内再取");
00782: 				return WithdrawResult.SilentFail;
00783: 			}
00784: 		}
00785: 		if (!BoxItem.IsUsableByPlayer(pack))
00786: 		{
00787: 			Core.Debug("收纳箱 withdraw：收纳箱非玩家所有，拒绝取出");
00788: 			return WithdrawResult.Empty;
00789: 		}
00790: 		string itemId = StorageGrid.GetItemId(pack, slot);
00791: 		if (!NullGuard.HasText(itemId))
00792: 		{
00793: 			try
00794: 			{
00795: 				StorageGrid.ClearSlot(pack, slot);
00796: 			}
00797: 			catch
00798: 			{
00799: 			}
00800: 			Core.Debug("收纳箱 withdraw 槽 " + slot + "：空槽（无货可取），忽略。");
00801: 			return WithdrawResult.Empty;
00802: 		}
00803: 		int count = StorageGrid.GetCount(pack, slot);
00804: 		if (count <= 0)
00805: 		{
00806: 			Core.Debug("收纳箱 withdraw 槽 " + slot + " '" + itemId + "'：数量为 0");
00807: 			return WithdrawResult.Empty;
00808: 		}
00809: 		GameGridInventory val = null;
00810: 		try
00811: 		{
00812: 			GameInventory parentInventory = pack.parentInventory;
00813: 			val = (GameGridInventory)(object)((parentInventory is GameGridInventory) ? parentInventory : null);
00814: 		}
00815: 		catch
00816: 		{
00817: 			val = null;
00818: 		}
00819: 		if (val == null && dropGrid == null)
00820: 		{
00821: 			Core.Debug("收纳箱 withdraw：盒子不在网格上，无法同网格发放");
00822: 			return WithdrawResult.NotOnGrid;
00823: 		}
00824: 		GameItem item = null;
00825: 		string text = null;
00826: 		bool flag2 = false;
00827: 		int v = 0;
00828: 		int v2 = 0;
00829: 		try
00830: 		{
00831: 			item = DirectoryMaster.Item(itemId, true);
00832: 			if (item == null)
00833: 			{
00834: 				bool flag3 = true;
00835: 				try
00836: 				{
00837: 					flag3 = DirectoryMaster.Has<GameItem>(itemId);
00838: 				}
00839: 				catch
00840: 				{
00841: 					flag3 = true;
00842: 				}
00843: 				Core.Log?.Warning("收纳箱 withdraw：'" + itemId + "' 重建失败（目录登记=" + (flag3 ? "有" : "无，所属 MOD 可能未加载") + "，槽位保留不扣账）");
00844: 				if (!flag3)
00845: 				{
00846: 					BoxItem.NotifyUser("该物品所属模组未加载，暂时无法取出");
00847: 				}
00848: 				return WithdrawResult.SilentFail;
00849: 			}
00850: 			try
00851: 			{
00852: 				GeneralHelper.SetItemOwned(item, true);
00853: 			}
00854: 			catch
00855: 			{
00856: 			}
00857: 			try
00858: 			{
00859: 				item.unitCount = 1;
00860: 			}
00861: 			catch
00862: 			{
00863: 			}
00864: 			bool flag4 = false;
00865: 			try
00866: 			{
00867: 				flag4 = WaterFeatureHelper.GetWaterFeature(item) != null;
00868: 			}
00869: 			catch
00870: 			{
00871: 			}
00872: 			bool flag5 = false;
00873: 			try
00874: 			{
00875: 				string purity = StorageGrid.GetPurity(pack, slot);
00876: 				GameItem val2 = TryBuildFactoryItem(itemId, purity);
00877: 				if (val2 != null)
00878: 				{
00879: 					try
00880: 					{
00881: 						item.Destroy();
00882: 					}
00883: 					catch
00884: 					{
00885: 					}
00886: 					item = val2;
00887: 					flag5 = true;
00888: 					try
00889: 					{
00890: 						GeneralHelper.SetItemOwned(item, true);
00891: 					}
00892: 					catch
00893: 					{
00894: 					}
00895: 					try
00896: 					{
00897: 						item.unitCount = 1;
00898: 					}
00899: 					catch
00900: 					{
00901: 					}
00902: 					try
00903: 					{
00904: 						flag4 = WaterFeatureHelper.GetWaterFeature(item) != null;
00905: 					}
00906: 					catch
00907: 					{
00908: 					}
00909: 				}
00910: 			}
00911: 			catch (Exception ex)
00912: 			{
00913: 				Core.Debug("收纳箱 withdraw 工厂直发异常：" + ex.Message + "（退重建路径）");
00914: 			}
00915: 			string displayName = StorageGrid.GetDisplayName(pack, slot);
00916: 			if (!string.IsNullOrEmpty(displayName) && !flag4 && !flag5)
00917: 			{
00918: 				try
00919: 				{
00920: 					item.name = displayName;
00921: 				}
00922: 				catch
00923: 				{
00924: 				}
00925: 			}
00926: 			string spriteAtlas = StorageGrid.GetSpriteAtlas(pack, slot);
00927: 			bool flag6 = false;
00928: 			try
00929: 			{
00930: 				flag6 = StorageGrid.ReadItemEnergy(item).HasValue;
00931: 			}
00932: 			catch
00933: 			{
00934: 			}
00935: 			if (!string.IsNullOrEmpty(spriteAtlas) && !flag6 && !flag4)
00936: 			{
00937: 				try
00938: 				{
00939: 					item.spriteAtlasPath = spriteAtlas;
00940: 				}
00941: 				catch
00942: 				{
00943: 				}
00944: 			}
00945: 			string text2 = StorageGrid.GetSpritePath(pack, slot);
00946: 			try
00947: 			{
00948: 				string text3 = TagSnapshot.PeekLastSprite(pack, slot);
00949: 				if (!string.IsNullOrEmpty(text3))
00950: 				{
00951: 					text2 = text3;
00952: 				}
00953: 			}
00954: 			catch
00955: 			{
00956: 			}
00957: 			bool flag7 = true;
00958: 			if (!string.IsNullOrEmpty(text2) && !flag6 && !flag4)
00959: 			{
00960: 				flag7 = BoxWindow.IsSpriteResolvable(string.IsNullOrEmpty(spriteAtlas) ? null : spriteAtlas, text2);
00961: 				if (!flag7)
00962: 				{
00963: 					Core.Debug("收纳箱 withdraw 贴图：键解析失败跳过写回（防覆盖原生贴图）atlas='" + (Core.Clean(spriteAtlas) ?? "-") + "' spr='" + (text2 ?? "-") + "'");
00964: 				}
00965: 			}
00966: 			if (!string.IsNullOrEmpty(text2) && !flag6 && !flag4 && flag7)
00967: 			{
00968: 				bool flag8 = false;
00969: 				try
00970: 				{
00971: 					item.SetSprite(Core.Clean(spriteAtlas), Core.Clean(text2));
00972: 					flag8 = true;
00973: 				}
00974: 				catch
00975: 				{
00976: 				}
00977: 				if (!flag8)
00978: 				{
00979: 					try
00980: 					{
00981: 						item.spriteAtlasPath = spriteAtlas;
00982: 					}
00983: 					catch
00984: 					{
00985: 					}
00986: 					try
00987: 					{
00988: 						item.spritePath = text2;
00989: 					}
00990: 					catch
00991: 					{
00992: 					}
00993: 				}
00994: 				try
00995: 				{
00996: 					int uniqueId = item.uniqueId;
00997: 					Core.Debug("收纳箱 withdraw 贴图登记：uid=" + uniqueId + " atlas='" + (Core.Clean(spriteAtlas) ?? "-") + "' spr='" + (text2 ?? "-") + "' setSprite=" + flag8);
00998: 					if (uniqueId != 0)
00999: 					{
01000: 						StoragePatch.RegisterWithdrawSprite(uniqueId, Core.Clean(spriteAtlas) ?? "", Core.Clean(text2) ?? "");
01001: 					}
01002: 				}
01003: 				catch
01004: 				{
01005: 				}
01006: 			}
01007: 			long? unitValue = StorageGrid.GetUnitValue(pack, slot);
01008: 			bool flag9 = false;
01009: 			try
01010: 			{
01011: 				flag9 = StorageGrid.IsWaterSlot(pack, slot) && StorageGrid.GetStoredWaterML(pack, slot) > 0;
01012: 			}
01013: 			catch
01014: 			{
01015: 			}
01016: 			if (!flag9)
01017: 			{
01018: 				try
01019: 				{
01020: 					ItemFeature waterFeature = WaterFeatureHelper.GetWaterFeature(item);
01021: 					if (waterFeature != null)
01022: 					{
01023: 						bool flag10 = false;
01024: 						GameItem val3 = null;
01025: 						try
01026: 						{
01027: 							val3 = DirectoryMaster.Item(itemId, true);
01028: 						}
01029: 						catch
01030: 						{
01031: 						}
01032: 						try
01033: 						{
01034: 							flag10 = val3 != null && WaterFeatureHelper.GetWaterFeature(val3) != null;
01035: 						}
01036: 						catch
01037: 						{
01038: 						}
01039: 						try
01040: 						{
01041: 							if (val3 != null)
01042: 							{
01043: 								val3.Destroy();
01044: 							}
01045: 						}
01046: 						catch
01047: 						{
01048: 						}
01049: 						if (!flag10)
01050: 						{
01051: 							string text4 = "-";
01052: 							try
01053: 							{
01054: 								text4 = Core.Clean(waterFeature.identifier) ?? "-";
01055: 							}
01056: 							catch
01057: 							{
01058: 							}
01059: 							try
01060: 							{
01061: 								item.RemoveItemFeatureByID(text4);
01062: 							}
01063: 							catch
01064: 							{
01065: 							}
01066: 							try
01067: 							{
01068: 								WaterFeatureHelper.InitWaterFeature(item, false);
01069: 							}
01070: 							catch
01071: 							{
01072: 							}
01073: 							Core.Debug("收纳箱 feat 残留重置：'" + itemId + "' featId='" + text4 + "'（出厂无 feat 而现物有，按水容器正规化）");
01074: 						}
01075: 					}
01076: 				}
01077: 				catch
01078: 				{
01079: 				}
01080: 			}
01081: 			if (!flag9 && !flag5)
01082: 			{
01083: 				if (unitValue.HasValue && unitValue.Value > 0)
01084: 				{
01085: 					try
01086: 					{
01087: 						item.unitValue = unitValue.Value;
01088: 					}
01089: 					catch
01090: 					{
01091: 					}
01092: 				}
01093: 				else
01094: 				{
01095: 					long num = -1L;
01096: 					try
01097: 					{
01098: 						GameItem val4 = DirectoryMaster.Item(itemId, true);
01099: 						if (val4 != null)
01100: 						{
01101: 							try
01102: 							{
01103: 								num = val4.unitValue;
01104: 							}
01105: 							catch
01106: 							{
01107: 							}
01108: 							try
01109: 							{
01110: 								val4.Destroy();
01111: 							}
01112: 							catch
01113: 							{
01114: 							}
01115: 						}
01116: 					}
01117: 					catch
01118: 					{
01119: 					}
01120: 					if (num > 0)
01121: 					{
01122: 						try
01123: 						{
01124: 							item.unitValue = num;
01125: 						}
01126: 						catch
01127: 						{
01128: 						}
01129: 						Core.Debug("收纳箱 withdraw 价值兜底：'" + itemId + "' 存入记录 " + unitValue.GetValueOrDefault() + " → 出厂模板价 " + num);
01130: 					}
01131: 				}
01132: 			}
01133: 			int? calorie = StorageGrid.GetCalorie(pack, slot);
01134: 			if (calorie.HasValue && calorie.Value > 0)
01135: 			{
01136: 				StorageGrid.WriteItemCalorie(item, calorie.Value);
01137: 			}
01138: 			string shape = StorageGrid.GetShape(pack, slot);
01139: 			if (NullGuard.HasText(shape))
01140: 			{
01141: 				string[] array = shape.Split(',');
01142: 				if (array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2) && result > 0 && result2 > 0)
01143: 				{
01144: 					try
01145: 					{
01146: 						item.SetShape(new GridShapeBuilder(result, result2).SetDataFill(result, result2, (byte)1).Build());
01147: 					}
01148: 					catch (Exception ex2)
01149: 					{
01150: 						Core.Debug("收纳箱 withdraw：占格还原失败 " + shape + "：" + ex2.Message);
01151: 					}
01152: 				}
01153: 			}
01154: 			string purity2 = StorageGrid.GetPurity(pack, slot);
01155: 			if (NullGuard.HasText(purity2) && !flag4 && !flag5)
01156: 			{
01157: 				RestoreCondition(item, purity2);
01158: 			}
01159: 			if (StorageGrid.GetCrateOpenFlag(pack, slot))
01160: 			{
01161: 				try
01162: 				{
01163: 					try
01164: 					{
01165: 						item.ModifyTag("LOCK_STATUS", (Action<TagState>)delegate(TagState t)
01166: 						{
01167: 							t.Enable().SetBool(false);
01168: 						}, false);
01169: 					}
01170: 					catch (Exception ex3)
01171: 					{
01172: 						Core.Debug("收纳箱 crate 还原 LOCK_STATUS：" + ex3.Message);
01173: 					}
01174: 					try
01175: 					{
01176: 						if (item.contentWindow != null)
01177: 						{
01178: 							item.contentWindow.isLocked = false;
01179: 						}
01180: 					}
01181: 					catch
01182: 					{
01183: 					}
01184: 					try
01185: 					{
01186: 						string text5 = item.spritePath ?? "";
01187: 						if (!string.IsNullOrEmpty(text5) && !text5.EndsWith("_u", StringComparison.Ordinal))
01188: 						{
01189: 							item.SetSpriteAndShape(item.spriteAtlasPath, text5 + "_u");
01190: 						}
01191: 					}
01192: 					catch (Exception ex4)
01193: 					{
01194: 						Core.Debug("收纳箱 crate 还原贴图：" + ex4.Message);
01195: 					}
01196: 					try
01197: 					{
01198: 						item.RemoveGameItemType("SUPPLY_CRATE");
01199: 					}
01200: 					catch (Exception ex5)
01201: 					{
01202: 						Core.Debug("收纳箱 crate 还原类型：" + ex5.Message);
01203: 					}
01204: 					try
01205: 					{
01206: 						item.EnableTag("IMPORTANT_TAG", false);
01207: 					}
01208: 					catch (Exception ex6)
01209: 					{
01210: 						Core.Debug("收纳箱 crate 还原 IMPORTANT：" + ex6.Message);
01211: 					}
01212: 					try
01213: 					{
01214: 						item.unitValue = 15L;
01215: 					}
01216: 					catch
01217: 					{
01218: 					}
01219: 					try
01220: 					{
01221: 						MetaHelper.RemoveIgnoreChildValue(item);
01222: 					}
01223: 					catch
01224: 					{
01225: 					}
01226: 					try
01227: 					{
01228: 						if (NullGuard.HasText(item.name))
01229: 						{
01230: 							item.name = item.name.Replace("(" + LocHelper.GetLocalizedMechanic("mech_lock_sealed", Array.Empty<object>()) + ")", "").Trim();
01231: 						}
01232: 					}
01233: 					catch
01234: 					{
01235: 					}
01236: 					try
01237: 					{
01238: 						List<GameItem> list = item.FindAllChildItems();
01239: 						if (list != null && list.Count > 0)
01240: 						{
01241: 							List<GameItem> list2 = new List<GameItem>(list);
01242: 							foreach (GameItem item2 in list2)
01243: 							{
01244: 								try
01245: 								{
01246: 									if (item2 != null)
01247: 									{
01248: 										item2.Destroy();
01249: 									}
01250: 								}
01251: 								catch
01252: 								{
01253: 								}
01254: 							}
01255: 							Core.Log?.Msg("收纳箱 crate 还原：清除重建预填内部货 x" + list2.Count);
01256: 						}
01257: 					}
01258: 					catch
01259: 					{
01260: 					}
01261: 					Core.Debug("收纳箱 crate 还原完成：'" + itemId + "' → 刷开空箱");
01262: 				}
01263: 				catch (Exception ex7)
01264: 				{
01265: 					Core.Debug("收纳箱 crate 还原异常：" + ex7.Message);
01266: 				}
01267: 			}
01268: 			TryRestoreBoughtPrice(pack, slot, item);
01269: 			if (flag4)
01270: 			{
01271: 				try
01272: 				{
01273: 					int itemCapacityUl = StorageGrid.GetItemCapacityUl(pack, slot);
01274: 					if (itemCapacityUl > 0)
01275: 					{
01276: 						TagSystem state = item.state;
01277: 						TagState val5 = ((state != null) ? state.GetTag("LIQUID_CONTAINER_CAPACITY") : null);
01278: 						if (val5 != null)
01279: 						{
01280: 							val5.SetEnabled(true);
01281: 							val5.valueInt = itemCapacityUl;
01282: 							try
01283: 							{
01284: 								item.ValidateShapeState();
01285: 							}
01286: 							catch
01287: 							{
01288: 							}
01289: 							Core.Debug("收纳箱 withdraw 容量还原：" + itemCapacityUl / 1000 + "ml（槽 " + slot + "）");
01290: 						}
01291: 					}
01292: 				}
01293: 				catch
01294: 				{
01295: 				}
01296: 				try
01297: 				{
01298: 					string slotCustomName = StorageGrid.GetSlotCustomName(pack, slot);
01299: 					if (!string.IsNullOrEmpty(slotCustomName))
01300: 					{
01301: 						GeneralHelper.SetCustomName(item, slotCustomName);
01302: 						Core.Debug("收纳箱 withdraw 改名水还原：'" + slotCustomName + "'（槽 " + slot + "）");
01303: 					}
01304: 					else if (GeneralHelper.HasCustomName(item))
01305: 					{
01306: 						GeneralHelper.RemoveCustomName(item);
01307: 						Core.Debug("收纳箱 withdraw 改名水清残留（槽 " + slot + "）");
01308: 					}
01309: 				}
01310: 				catch
01311: 				{
01312: 				}
01313: 				int storedWaterML = StorageGrid.GetStoredWaterML(pack, slot);
01314: 				int storedWaterPurity = StorageGrid.GetStoredWaterPurity(pack, slot);
01315: 				if (storedWaterML > 0)
01316: 				{
01317: 					bool flag11 = false;
01318: 					try
01319: 					{
01320: 						string liquidSnapshot = StorageGrid.GetLiquidSnapshot(pack, slot);
01321: 						if (!string.IsNullOrEmpty(liquidSnapshot))
01322: 						{
01323: 							Dictionary<string, long> dictionary = ParsePool(liquidSnapshot);
01324: 							long num2 = 0L;
01325: 							foreach (long value6 in dictionary.Values)
01326: 							{
01327: 								num2 += value6;
01328: 							}
01329: 							long num3 = -1L;
01330: 							try
01331: 							{
01332: 								TagSystem state2 = item.state;
01333: 								TagState val6 = ((state2 == null) ? null : state2.dict?["LIQUID_CONTAINER_CAPACITY"]);
01334: 								if (val6 != null && val6.valueInt > 0)
01335: 								{
01336: 									num3 = val6.valueInt;
01337: 								}
01338: 							}
01339: 							catch
01340: 							{
01341: 							}
01342: 							if (num3 <= 0)
01343: 							{
01344: 								num3 = num2;
01345: 							}
01346: 							long num4 = Math.Min(num2, num3);
01347: 							int num5 = (int)(num4 / 1000);
01348: 							Dictionary<string, long> dictionary2 = new Dictionary<string, long>();
01349: 							long num6 = 0L;
01350: 							foreach (KeyValuePair<string, long> item3 in dictionary)
01351: 							{
01352: 								long num7 = ((num2 > 0) ? (item3.Value * num4 / num2) : 0);
01353: 								if (num7 > 0)
01354: 								{
01355: 									dictionary2[item3.Key] = num7;
01356: 								}
01357: 								num6 += num7;
01358: 							}
01359: 							long num8 = num4 - num6;
01360: 							if (num8 != 0L)
01361: 							{
01362: 								dictionary2["CURRENT_PART_WATER"] = (dictionary2.TryGetValue("CURRENT_PART_WATER", out var value) ? value : 0) + num8;
01363: 							}
01364: 							long value2;
01365: 							long num9 = (dictionary2.TryGetValue("CURRENT_PART_WATER", out value2) ? value2 : 0);
01366: 							int num10 = (int)((num4 > 0) ? (num9 * 10000 / num4) : 0);
01367: 							TagSystem state3 = item.state;
01368: 							Dictionary<string, TagState> dictionary3 = ((state3 != null) ? state3.dict : null);
01369: 							int num11 = 0;
01370: 							if (dictionary3 != null)
01371: 							{
01372: 								foreach (KeyValuePair<string, TagState> item4 in dictionary3)
01373: 								{
01374: 									if (item4.Key == null || !item4.Key.StartsWith("CURRENT_PART_"))
01375: 									{
01376: 										continue;
~~~

~~~csharp
01434: 							try
01435: 							{
01436: 								WaterHelper.UpdateContainerGoodType(item);
01437: 							}
01438: 							catch
01439: 							{
01440: 							}
01441: 							try
01442: 							{
01443: 								WaterFeatureHelper.GetWaterFeature(item).customIntValue1 = num10;
01444: 							}
01445: 							catch
01446: 							{
01447: 							}
01448: 							int num12 = Rd();
01449: 							int num13 = Rp();
01450: 							flag11 = num12 == num5 && Math.Abs(num13 - num10) <= 5;
01451: 							if (flag11)
01452: 							{
01453: 								foreach (KeyValuePair<string, long> item6 in dictionary2)
01454: 								{
01455: 									if (dictionary.TryGetValue(item6.Key, out var value4))
01456: 									{
01457: 										dictionary[item6.Key] = value4 - item6.Value;
01458: 									}
01459: 								}
01460: 								long num14 = 0L;
01461: 								foreach (long value7 in dictionary.Values)
01462: 								{
01463: 									num14 += value7;
01464: 								}
01465: 								text = SerializePool(dictionary);
01466: 								flag2 = num14 <= 0;
01467: 								v = (int)(num14 / 1000);
01468: 								v2 = (int)((!flag2) ? ((dictionary.TryGetValue("CURRENT_PART_WATER", out var value5) ? value5 : 0) * 10000 / ((num14 > 0) ? num14 : 1)) : 0);
01469: 								Core.Debug("收纳箱水还原成功：切 " + num5 + "ml（池余 " + num14 / 1000 + "ml）→ ml=" + num12 + " pur=" + num13 + "/" + num10 + " 键 " + num11);
01470: 							}
01471: 							else
01472: 							{
01473: 								Core.Log?.Msg("收纳箱水还原失败：切 " + num5 + "ml 写瓶读回 ml=" + num12 + "/" + num5 + " pur=" + num13 + "/" + num10 + "（拒绝取出，池保留）");
01474: 							}
01475: 						}
01476: 						else
01477: 						{
01478: 							try
01479: 							{
01480: 								WaterHelper.AddWater(item, 0, storedWaterML * 1000, false, 0, 0, false);
01481: 							}
01482: 							catch
01483: 							{
01484: 							}
01485: 							try
01486: 							{
01487: 								WaterFeatureHelper.OnLiquidChanged(item, false);
01488: 							}
01489: 							catch
01490: 							{
01491: 							}
01492: 							try
01493: 							{
01494: 								WaterFeatureHelper.UpdateWaterFeature(item, false);
01495: 							}
01496: 							catch
01497: 							{
01498: 							}
01499: 							try
01500: 							{
01501: 								WaterFeatureHelper.UpdateWaterFeatureFake(item, storedWaterPurity);
01502: 							}
01503: 							catch
01504: 							{
01505: 							}
01506: 							try
01507: 							{
01508: 								WaterHelper.UpdateContainerGoodType(item);
01509: 							}
01510: 							catch
01511: 							{
01512: 							}
01513: 							try
01514: 							{
01515: 								WaterFeatureHelper.GetWaterFeature(item).customIntValue1 = storedWaterPurity;
01516: 							}
01517: 							catch
01518: 							{
01519: 							}
01520: 							int num15 = Rd();
01521: 							flag11 = num15 == storedWaterML;
01522: 							Core.Log?.Msg(flag11 ? ("收纳箱水还原(旧格兼容)：ml=" + num15 + "/" + storedWaterML + " pur=" + Rp() + "/" + storedWaterPurity + "（纯度官方随机，近似）") : ("收纳箱水还原失败：AddWater→ml=" + num15 + "/" + storedWaterML + "（拒绝取出，槽位保留）"));
01523: 							if (flag11)
01524: 							{
01525: 								flag2 = true;
01526: 							}
01527: 						}
01528: 					}
01529: 					catch (Exception ex8)
01530: 					{
01531: 						Core.Log?.Msg("收纳箱水还原异常：" + ex8.Message + "（拒绝取出，池保留）");
01532: 					}
01533: 					if (!flag11)
01534: 					{
01535: 						try
01536: 						{
01537: 							item.Destroy();
01538: 						}
01539: 						catch
01540: 						{
01541: 						}
01542: 						return WithdrawResult.SilentFail;
01543: 					}
01544: 					Core.Debug("收纳箱 withdraw：水还原 " + storedWaterML + "ml 纯度 " + storedWaterPurity);
01545: 				}
01546: 				else
01547: 				{
01548: 					int num16 = -1;
01549: 					try
01550: 					{
01551: 						num16 = WaterHelper.GetCurrentCapacityML(item);
01552: 					}
01553: 					catch
01554: 					{
01555: 					}
01556: 					if (num16 > 0)
01557: 					{
01558: 						try
01559: 						{
01560: 							WaterHelper.EmptyContainer(item);
01561: 						}
01562: 						catch
01563: 						{
01564: 						}
01565: 						Core.Debug("收纳箱 withdraw：空瓶格重建瓶异常带水 " + num16 + "ml，已清空");
01566: 					}
01567: 				}
01568: 			}
01569: 			try
01570: 			{
01571: 				string text6 = Core.Clean(item.spritePath) ?? "-";
01572: 				string text7 = "-";
01573: 				try
01574: 				{
01575: 					text7 = item.unitValue.ToString();
01576: 				}
01577: 				catch
01578: 				{
01579: 				}
01580: 				string text8 = StorageGrid.ReadItemCalorie(item)?.ToString() ?? "-";
01581: 				string text9 = "-";
01582: 				try
01583: 				{
01584: 					text9 = ChemicalProductHelper.GetCurrentChemicalConditionId(item) ?? "-";
01585: 				}
01586: 				catch
01587: 				{
01588: 				}
01589: 				Core.Debug("收纳箱 withdraw 外观写回：name='" + (displayName ?? "-") + "' spr='" + (text2 ?? "-") + "→" + text6 + "' baseVal=" + (unitValue?.ToString() ?? "-") + "→" + text7 + " cal=" + (calorie?.ToString() ?? "-") + "→" + text8 + " cond='" + (purity2 ?? "-") + "→" + text9 + "'");
01590: 			}
01591: 			catch
01592: 			{
01593: 			}
01594: 			GameGridInventory val7 = dropGrid ?? val;
01595: 			int num17 = 0;
01596: 			try
01597: 			{
01598: 				GraphNodeStorage val8 = (GraphNodeStorage)(object)val7;
01599: 				num17 = ((val8 == null) ? GraphUtils.TryAcceptAllMid(val7, item, -1) : GraphUtils.TryAcceptAll(val8, item, -1));
01600: 			}
01601: 			catch (Exception ex9)
01602: 			{
01603: 				Core.Debug("收纳箱 TryAcceptAll 不可用（" + ex9.Message + "），退回 Mid");
01604: 				try
01605: 				{
01606: 					num17 = GraphUtils.TryAcceptAllMid(val7, item, -1);
01607: 				}
01608: 				catch (Exception ex10)
01609: 				{
01610: 					Core.Log?.Warning("收纳箱 TryAcceptAllMid：" + ex10.Message);
01611: 				}
01612: 			}
01613: 			bool flag12 = false;
01614: 			try
01615: 			{
01616: 				GameInventory parentInventory2 = item.parentInventory;
01617: 				GameGridInventory val9 = (GameGridInventory)(object)((parentInventory2 is GameGridInventory) ? parentInventory2 : null);
01618: 				flag12 = val9 != null && val9 == val7;
01619: 			}
01620: 			catch
01621: 			{
01622: 				flag12 = false;
01623: 			}
01624: 			if (!flag12)
01625: 			{
01626: 				Core.Debug("收纳箱 withdraw '" + itemId + "'：同网格无空位（TryAcceptAllMid=" + num17 + "），不扣库存");
01627: 				try
01628: 				{
01629: 					item.Destroy();
01630: 				}
01631: 				catch
01632: 				{
01633: 				}
01634: 				return WithdrawResult.NoSpace;
01635: 			}
01636: 			string text10 = null;
01637: 			try
01638: 			{
01639: 				text10 = TagSnapshot.PopAndRestore(pack, slot, item);
01640: 			}
01641: 			catch (Exception ex11)
01642: 			{
01643: 				Core.Debug("收纳箱 withdraw 快照还原异常：" + ex11.Message);
01644: 			}
01645: 			ReplayLoad("机器升级", delegate
01646: 			{
01647: 				MachineHelper.LoadUpgrade(item);
01648: 			});
01649: 			ReplayLoad("水容器贴图", delegate
01650: 			{
01651: 				SpriteHelper.UpdateWaterContainerSprite(item);
01652: 			});
01653: 			ReplayLoad("锁定物品", delegate
01654: 			{
01655: 				GeneralHelper.LoadLockItem(item);
01656: 			});
01657: 			ReplayLoad("可食产出", delegate
01658: 			{
01659: 				PlantTraitList.LoadEdibleProduce(item);
01660: 			});
01661: 			ReplayLoad("水培口味", delegate
01662: 			{
01663: 				PlantTraitList.LoadTasteFeature(item);
01664: 			});
01665: 			ReplayLoad("非自有工具", delegate
01666: 			{
01667: 				GeneralHelper.LoadNonOwnedToolTag(item);
01668: 			});
01669: 			ReplayLoad("危废标签", delegate
01670: 			{
01671: 				GeneralHelper.LoadHazardousWasteTag(item);
01672: 			});
01673: 			ReplayLoad("锁定容器", delegate
01674: 			{
01675: 				LockHelper.LoadLockedContainer(item);
01676: 			});
01677: 			ReplayLoad("损坏机器", delegate
01678: 			{
01679: 				MachineBrokenHelper.LoadBrokenMachine(item);
01680: 			});
01681: 			ReplayLoad("机器OnLoad", delegate
01682: 			{
01683: 				MachineHelper.OnLoad(item);
01684: 			});
01685: 			ReplayLoad("涡轮资格", delegate
01686: 			{
01687: 				MachineTurboBoosterAdv.LoadTurboBoostEligibleTag(item);
01688: 			});
01689: 			ReplayLoad("转换器", delegate
01690: 			{
01691: 				CurrentConverterHelper.OnLoad(item);
01692: 			});
01693: 			try
01694: 			{
01695: 				((GameItemFunc)item).onLoaded?.Invoke(item);
01696: 			}
01697: 			catch (Exception ex12)
01698: 			{
01699: 				Core.Debug("收纳箱 withdraw onLoaded 回调失败：" + ex12.Message);
01700: 			}
01701: 			try
01702: 			{
01703: 				string text11 = Core.Clean(item.spritePath);
01704: 				string text12 = null;
01705: 				try
01706: 				{
01707: 					text12 = HusbandryHelper.GetAnimalSprite(item);
01708: 				}
01709: 				catch
01710: 				{
01711: 				}
01712: 				bool flag13 = HusbandryHelper.TryChangeSprite(item);
01713: 				GameItem obj66 = item;
01714: 				GameItemElement val10 = (GameItemElement)(object)((obj66 is GameItemElement) ? obj66 : null);
01715: 				if (val10 != null)
01716: 				{
01717: 					try
01718: 					{
01719: 						val10.ValidateSelect(true);
01720: 					}
01721: 					catch
01722: 					{
01723: 					}
01724: 				}
01725: 				Core.Debug("收纳箱贴图档：'" + (Core.Clean(item.identifier) ?? "?") + "' 前=" + (text11 ?? "null") + " 官方算=" + (text12 ?? "null") + " changed=" + flag13 + " 后=" + (Core.Clean(item.spritePath) ?? "null") + " 是元素=" + (item is GameItemElement));
01726: 			}
01727: 			catch (Exception ex13)
01728: 			{
01729: 				Core.Debug("收纳箱 withdraw 贴图档重解析异常：" + ex13.Message);
01730: 			}
01731: 			try
01732: 			{
01733: 				if (NullGuard.HasText(text10) && count - 1 > 0)
01734: 				{
01735: 					string text13 = null;
01736: 					string[] array2 = text10.Split('\u0001');
01737: 					foreach (string text14 in array2)
01738: 					{
01739: 						if (!string.IsNullOrEmpty(text14))
01740: 						{
01741: 							text13 = text14;
01742: 						}
01743: 					}
01744: 					if (NullGuard.HasText(text13))
01745: 					{
01746: 						array2 = text13.Split('\u0003');
01747: 						for (int num18 = 0; num18 < array2.Length; num18++)
01748: 						{
01749: 							string[] array3 = array2[num18].Split('\u0002');
01750: 							if (array3.Length >= 10 && !string.IsNullOrEmpty(array3[^1]))
01751: 							{
01752: 								if (!string.Equals(StorageGrid.GetSpritePath(pack, slot), array3[^1], StringComparison.Ordinal))
01753: 								{
01754: 									StorageGrid.SetSpriteRef(pack, slot, StorageGrid.GetSpriteAtlas(pack, slot), array3[^1]);
01755: 								}
01756: 								break;
01757: 							}
01758: 						}
01759: 					}
01760: 				}
01761: 			}
01762: 			catch (Exception ex14)
01763: 			{
01764: 				Core.Debug("收纳箱 withdraw 显示顺延异常：" + ex14.Message);
01765: 			}
01766: 			AllocatePools(pack, slot, item);
01767: 			if (flag2)
01768: 			{
01769: 				StorageGrid.SetLiquidSnapshot(pack, slot, null);
01770: 				StorageGrid.SetStoredWaterML(pack, slot, 0);
01771: 				StorageGrid.SetStoredWaterPurity(pack, slot, 0);
01772: 			}
01773: 			else if (text != null)
01774: 			{
01775: 				StorageGrid.SetLiquidSnapshot(pack, slot, text);
01776: 				StorageGrid.SetStoredWaterML(pack, slot, v);
01777: 				StorageGrid.SetStoredWaterPurity(pack, slot, v2);
01778: 			}
01779: 			int num19 = count - 1;
01780: 			if (num19 > 0)
01781: 			{
01782: 				StorageGrid.SetSlot(pack, slot, itemId, num19);
01783: 			}
01784: 			else
01785: 			{
01786: 				StorageGrid.ClearSlot(pack, slot);
01787: 			}
01788: 			if (text10 != null)
01789: 			{
01790: 				TagSnapshot.SetQueue(pack, slot, text10);
01791: 			}
01792: 			if (refresh)
01793: 			{
01794: 				BoxWindow.RefreshButtons(pack);
01795: 				BoxWindow.UpdateTooltip(pack);
01796: 			}
01797: 			Core.Debug("收纳箱 withdraw 槽 " + slot + " '" + itemId + "' OK → 余 " + num19 + "（同网格发放）");
01798: 			return WithdrawResult.Ok;
01799: 		}
01800: 		catch (Exception ex15)
01801: 		{
01802: 			Core.Log?.Warning("收纳箱 TryWithdraw：" + ex15.Message);
01803: 			try
01804: 			{
01805: 				if (item != null && item.parentInventory == null)
01806: 				{
01807: 					item.Destroy();
01808: 				}
01809: 			}
01810: 			catch
01811: 			{
01812: 			}
01813: 			return WithdrawResult.SilentFail;
01814: 		}
01815: 		int Rd()
01816: 		{
01817: 			try
01818: 			{
01819: 				return WaterHelper.GetCurrentCapacityML(item);
01820: 			}
01821: 			catch
01822: 			{
01823: 				return -1;
01824: 			}
01825: 		}
01826: 		static void ReplayLoad(string name, Action call)
01827: 		{
01828: 			try
01829: 			{
01830: 				call();
01831: 			}
01832: 			catch (Exception ex16)
01833: 			{
01834: 				Core.Debug("收纳箱 withdraw " + name + "重建失败：" + ex16.Message);
01835: 			}
01836: 		}
01837: 		int Rp()
01838: 		{
01839: 			try
01840: 			{
01841: 				return WaterHelper.GetWaterPurity(item);
01842: 			}
01843: 			catch
01844: 			{
01845: 				return -1;
01846: 			}
01847: 		}
01848: 	}
01849: 
01850: 	private static void DepositPool(GameItem pack, int slot, GameItem dragged, int put)
01851: 	{
01852: 		try
01853: 		{
01854: 			string text = Core.Clean(dragged.spritePath);
01855: 			if (!string.IsNullOrEmpty(text) && !string.Equals(StorageGrid.GetSpritePath(pack, slot), text, StringComparison.Ordinal))
01856: 			{
01857: 				StorageGrid.SetSpriteRef(pack, slot, Core.Clean(dragged.spriteAtlasPath), text);
01858: 			}
01859: 		}
01860: 		catch
01861: 		{
01862: 		}
01863: 		try
01864: 		{
01865: 			if (put > 1)
01866: 			{
01867: 				string text2 = TagSnapshot.Capture(dragged);
01868: 				if (text2 != null)
01869: 				{
01870: 					for (int i = 0; i < put; i++)
01871: 					{
01872: 						TagSnapshot.Enqueue(pack, slot, text2);
01873: 					}
01874: 					Core.Debug("收纳箱 deposit 堆叠快照入队：槽 " + slot + " '" + (Core.Clean(dragged.identifier) ?? "?") + "' put=" + put + " 同条 ×" + put + "（条长 " + text2.Length + "）");
01875: 				}
01876: 				else
01877: 				{
01878: 					Core.Debug("收纳箱快照：put=" + put + "（同质无状态堆叠，不拍）'" + (Core.Clean(dragged.identifier) ?? "?") + "'");
01879: 				}
01880: 			}
01881: 			else if (put == 1)
01882: 			{
01883: 				string text3 = TagSnapshot.Capture(dragged);
01884: 				if (text3 != null)
01885: 				{
01886: 					TagSnapshot.Enqueue(pack, slot, text3);
01887: 					Core.Debug("收纳箱 deposit 快照入队：槽 " + slot + " '" + (Core.Clean(dragged.identifier) ?? "?") + "' 长度 " + text3.Length);
01888: 				}
01889: 			}
01890: 		}
01891: 		catch (Exception ex)
01892: 		{
01893: 			Core.Debug("收纳箱 deposit 快照异常：" + ex.Message);
01894: 		}
01895: 		try
01896: 		{
01897: 			TagSnapshot.EnqueueChemParts(pack, slot, dragged, put);
01898: 		}
01899: 		catch (Exception ex2)
01900: 		{
01901: 			Core.Debug("收纳箱 deposit 组成入队异常：" + ex2.Message);
01902: 		}
01903: 		try
01904: 		{
01905: 			int? num = StorageGrid.ReadItemEnergy(dragged);
01906: 			if (num.HasValue)
01907: 			{
01908: 				int num2 = num.Value * put;
01909: 				int v = StorageGrid.GetEnergyPool(pack, slot) + num2;
01910: 				StorageGrid.SetEnergyPool(pack, slot, v);
01911: 				Core.Debug("收纳箱 deposit 池：电量 +" + num2 + "（单件 " + num.Value + " × " + put + "）→ 格池 " + v);
01912: 				RefreshEnergySlotSprite(pack, slot);
01913: 			}
01914: 		}
01915: 		catch
01916: 		{
01917: 		}
01918: 		try
01919: 		{
01920: 			int? num3 = StorageGrid.ReadItemDurability(dragged);
01921: 			if (num3.HasValue)
01922: 			{
01923: 				int num4 = num3.Value * put;
01924: 				int v2 = StorageGrid.GetDurabilityPool(pack, slot) + num4;
01925: 				StorageGrid.SetDurabilityPool(pack, slot, v2);
01926: 				Core.Debug("收纳箱 deposit 池：耐久 +" + num4 + " → 格池 " + v2);
01927: 			}
01928: 		}
01929: 		catch
01930: 		{
01931: 		}
01932: 		try
01933: 		{
01934: 			var (flag, num5, num6) = StorageGrid.ReadItemUseCount(dragged);
01935: 			if (flag)
01936: 			{
01937: 				int num7 = num5 * put;
01938: 				int v3 = StorageGrid.GetUseCountPool(pack, slot) + num7;
01939: 				StorageGrid.SetUseCountPool(pack, slot, v3);
01940: 				if (num6 > 0)
01941: 				{
01942: 					StorageGrid.SetUseCountMax(pack, slot, num6);
01943: 				}
01944: 				Core.Debug("收纳箱 deposit 池：次数 +" + num7 + "（单件 " + num5 + "/" + num6 + " × " + put + "）→ 格池 " + v3);
01945: 			}
01946: 		}
01947: 		catch
01948: 		{
01949: 		}
01950: 		try
01951: 		{
01952: 			int num8 = 0;
01953: 			try
01954: 			{
01955: 				TagSystem state = dragged.state;
01956: 				num8 = ((state == null) ? null : state.dict?["LIQUID_CONTAINER_CAPACITY"])?.valueInt ?? 0;
01957: 			}
01958: 			catch
01959: 			{
01960: 			}
01961: 			if (num8 > 0)
01962: 			{
01963: 				StorageGrid.SetItemCapacityUl(pack, slot, num8);
01964: 			}
01965: 			else
01966: 			{
01967: 				StorageGrid.ClearItemCapacityUl(pack, slot);
01968: 			}
01969: 			bool flag2 = false;
01970: 			try
01971: 			{
01972: 				flag2 = WaterFeatureHelper.GetWaterFeature(dragged) != null;
01973: 			}
01974: 			catch
01975: 			{
01976: 			}
01977: 			if (flag2)
01978: 			{
01979: 				StorageGrid.SetWaterSlot(pack, slot);
01980: 				bool flag3 = true;
01981: 				try
01982: 				{
01983: 					flag3 = WaterHelper.IsEmpty(dragged);
01984: 				}
01985: 				catch
01986: 				{
01987: 				}
01988: 				if (flag3)
01989: 				{
01990: 					if (StorageGrid.GetLiquidSnapshot(pack, slot) == null)
01991: 					{
01992: 						StorageGrid.SetStoredWaterML(pack, slot, 0);
01993: 						StorageGrid.SetStoredWaterPurity(pack, slot, 0);
01994: 					}
01995: 					return;
01996: 				}
01997: 				TagSystem state2 = dragged.state;
01998: 				Dictionary<string, TagState> dictionary = ((state2 != null) ? state2.dict : null);
01999: 				if (dictionary == null)
02000: 				{
02001: 					return;
02002: 				}
02003: 				Dictionary<string, long> dictionary2 = ParsePool(StorageGrid.GetLiquidSnapshot(pack, slot));
02004: 				int num9 = 0;
02005: 				foreach (KeyValuePair<string, TagState> item in dictionary)
02006: 				{
02007: 					if (item.Key != null && item.Key.StartsWith("CURRENT_PART_"))
02008: 					{
02009: 						int num10 = 0;
02010: 						try
02011: 						{
02012: 							num10 = item.Value?.valueInt ?? 0;
02013: 						}
02014: 						catch
02015: 						{
02016: 						}
02017: 						if (num10 > 0)
02018: 						{
02019: 							dictionary2[item.Key] = (dictionary2.TryGetValue(item.Key, out var value) ? value : 0) + num10;
02020: 							num9 += num10;
02021: 						}
02022: 					}
02023: 				}
02024: 				if (num9 <= 0)
02025: 				{
02026: 					return;
02027: 				}
02028: 				long num11 = 0L;
02029: 				foreach (long value3 in dictionary2.Values)
02030: 				{
02031: 					num11 += value3;
02032: 				}
02033: 				long value2;
02034: 				long num12 = (dictionary2.TryGetValue("CURRENT_PART_WATER", out value2) ? value2 : 0);
02035: 				StorageGrid.SetLiquidSnapshot(pack, slot, SerializePool(dictionary2));
02036: 				StorageGrid.SetStoredWaterML(pack, slot, (int)(num11 / 1000));
02037: 				StorageGrid.SetStoredWaterPurity(pack, slot, (int)((num11 > 0) ? (num12 * 10000 / num11) : 0));
02038: 				Core.Debug("收纳箱 deposit 池累加：+" + num9 + "µL → 池总 " + num11 / 1000 + "ml（槽 " + slot + "）");
02039: 			}
02040: 			else
02041: 			{
02042: 				StorageGrid.ClearWaterTags(pack, slot);
02043: 			}
02044: 		}
02045: 		catch
02046: 		{
02047: 		}
02048: 	}
02049: 
02050: 	private static Dictionary<string, long> ParsePool(string? snap)
02051: 	{
02052: 		Dictionary<string, long> dictionary = new Dictionary<string, long>();
02053: 		if (!NullGuard.HasText(snap))
02054: 		{
02055: 			return dictionary;
02056: 		}
02057: 		string[] array = snap.Split(';');
02058: 		foreach (string text in array)
02059: 		{
02060: 			int num = text.IndexOf('=');
02061: 			if (num > 0 && long.TryParse(text.Substring(num + 1), out var result))
02062: 			{
02063: 				string key = text.Substring(0, num);
02064: 				dictionary[key] = (dictionary.TryGetValue(key, out var value) ? value : 0) + result;
02065: 			}
02066: 		}
02067: 		return dictionary;
02068: 	}
02069: 
02070: 	private static string[] GetWaterCondIds()
02071: 	{
02072: 		if (_waterCondIds != null)
02073: 		{
02074: 			return _waterCondIds;
02075: 		}
02076: 		List<string> list = new List<string>();
02077: 		try
02078: 		{
02079: 			list.Add(Core.Clean(ItemConditionList.CreatePureWater()?.identifier) ?? "");
02080: 			list.Add(Core.Clean(ItemConditionList.CreateHighQualityWater()?.identifier) ?? "");
02081: 			list.Add(Core.Clean(ItemConditionList.CreateBaseWater()?.identifier) ?? "");
02082: 			list.Add(Core.Clean(ItemConditionList.CreateGhostWater()?.identifier) ?? "");
02083: 			list.Add(Core.Clean(ItemConditionList.CreateRustWater()?.identifier) ?? "");
02084: 			list.Add(Core.Clean(ItemConditionList.CreateGutterflow()?.identifier) ?? "");
02085: 		}
02086: 		catch
02087: 		{
02088: 		}
02089: 		_waterCondIds = list.ToArray();
02090: 		return _waterCondIds;
02091: 	}
02092: 
02093: 	internal static bool TryGetPoolFill(GameItem pack, int slot, out float fill)
02094: 	{
02095: 		fill = 0f;
02096: 		try
02097: 		{
02098: 			Dictionary<string, long> dictionary = ParsePool(StorageGrid.GetLiquidSnapshot(pack, slot));
02099: 			long num = 0L;
02100: 			foreach (long value in dictionary.Values)
02101: 			{
02102: 				num += value;
02103: 			}
02104: 			if (num <= 0)
02105: 			{
02106: 				return false;
02107: 			}
~~~

~~~csharp
02290: 			return text;
02291: 		}
02292: 		catch
02293: 		{
02294: 			return null;
02295: 		}
02296: 	}
02297: 
02298: 	private static string? TryReadDepositName(GameItem dragged, string itemId)
02299: 	{
02300: 		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
02301: 		try
02302: 		{
02303: 			if (IsEmptyWaterItem(dragged))
02304: 			{
02305: 				GameItem val = null;
02306: 				try
02307: 				{
02308: 					val = DirectoryMaster.Item(itemId, true);
02309: 				}
02310: 				catch
02311: 				{
02312: 				}
02313: 				if (val != null)
02314: 				{
02315: 					string text = TryReadName(val);
02316: 					try
02317: 					{
02318: 						val.Destroy();
02319: 					}
02320: 					catch
02321: 					{
02322: 					}
02323: 					if (!string.IsNullOrEmpty(text))
02324: 					{
02325: 						Core.Debug("收纳箱开格记名：空瓶出厂名 '" + text + "'");
02326: 						return text;
02327: 					}
02328: 				}
02329: 			}
02330: 		}
02331: 		catch
02332: 		{
02333: 		}
02334: 		string text2 = TryReadName(dragged);
02335: 		if (string.IsNullOrEmpty(text2))
02336: 		{
02337: 			return text2;
02338: 		}
02339: 		try
02340: 		{
02341: 			string localizedMechanic = LocHelper.GetLocalizedMechanic("husbandry_stage_unknown", Array.Empty<object>());
02342: 			if (dragged.IsTag("ANIMAL_AGE_TAG"))
02343: 			{
02344: 				string animalGrowthStageDisplay = HusbandryHelper.GetAnimalGrowthStageDisplay(dragged);
02345: 				string localizedMechanic2 = LocHelper.GetLocalizedMechanic("husbandry_stage_infant", Array.Empty<object>());
02346: 				if (!string.IsNullOrEmpty(animalGrowthStageDisplay) && animalGrowthStageDisplay != localizedMechanic && animalGrowthStageDisplay == localizedMechanic2)
02347: 				{
02348: 					return animalGrowthStageDisplay;
02349: 				}
02350: 			}
02351: 			else if (dragged.IsTag("FISH_AGE_TAG"))
02352: 			{
02353: 				string stageDisplay = FishSpecies.GetStageDisplay(AquacultureHelper.GetStage(dragged));
02354: 				string localizedMechanic3 = LocHelper.GetLocalizedMechanic("husbandry_stage_juvenile", Array.Empty<object>());
02355: 				if (!string.IsNullOrEmpty(stageDisplay) && stageDisplay != localizedMechanic && stageDisplay == localizedMechanic3)
02356: 				{
02357: 					return stageDisplay;
02358: 				}
02359: 			}
02360: 		}
02361: 		catch
02362: 		{
02363: 		}
02364: 		return text2;
02365: 	}
02366: 
02367: 	private static string SerializePool(Dictionary<string, long> pool)
02368: 	{
02369: 		StringBuilder stringBuilder = new StringBuilder();
02370: 		foreach (KeyValuePair<string, long> item in pool)
02371: 		{
02372: 			if (item.Value > 0)
02373: 			{
02374: 				if (stringBuilder.Length > 0)
02375: 				{
02376: 					stringBuilder.Append(';');
02377: 				}
02378: 				stringBuilder.Append(item.Key).Append('=').Append(item.Value);
02379: 			}
02380: 		}
02381: 		return stringBuilder.ToString();
02382: 	}
02383: 
02384: 	private static void AllocatePools(GameItem pack, int slot, GameItem item)
02385: 	{
02386: 		try
02387: 		{
02388: 			if (PowerHelper.IsAvailableEnergyFromItem(item))
02389: 			{
02390: 				int num = Rd();
02391: 				PowerHelper.ChargeBattery(item);
02392: 				int num2 = Rd();
02393: 				int energyPool = StorageGrid.GetEnergyPool(pack, slot);
02394: 				int num3 = Math.Min(Math.Max(0, energyPool), num2);
02395: 				int num4 = num2 - num3;
02396: 				if (num4 > 0)
02397: 				{
02398: 					try
02399: 					{
02400: 						PowerHelper.TryRemoveEnergy(item, num4, false);
02401: 					}
02402: 					catch
02403: 					{
02404: 					}
02405: 				}
02406: 				int num5 = Rd();
02407: 				if (num5 == num3)
02408: 				{
02409: 					StorageGrid.SetEnergyPool(pack, slot, energyPool - num3);
02410: 					try
02411: 					{
02412: 						PowerHelper.UpdateEnergyCreditSprite(item);
02413: 					}
02414: 					catch
02415: 					{
02416: 					}
02417: 					try
02418: 					{
02419: 						if (num3 == num2 && num3 > 0 && string.IsNullOrEmpty(StorageGrid.GetFullSprite(pack, slot)))
02420: 						{
02421: 							string text = TrySpriteKey(item);
02422: 							if (text != "-")
02423: 							{
02424: 								StorageGrid.SetFullSprite(pack, slot, text);
02425: 								StorageGrid.SetEnergyMax(pack, slot, num2);
02426: 								Core.Debug("收纳箱池回收：满电 key=" + text + " max=" + num2);
02427: 							}
02428: 						}
02429: 					}
02430: 					catch
02431: 					{
02432: 					}
02433: 					RefreshEnergySlotSprite(pack, slot);
02434: 					Core.Debug("收纳箱 withdraw 池：电量 give=" + num3 + "/" + num2 + " → 格池 " + (energyPool - num3));
02435: 				}
02436: 				else
02437: 				{
02438: 					int num6 = Rd();
02439: 					try
02440: 					{
02441: 						if (num6 > num)
02442: 						{
02443: 							PowerHelper.TryRemoveEnergy(item, num6 - num, false);
02444: 						}
02445: 					}
02446: 					catch
02447: 					{
02448: 					}
02449: 					try
02450: 					{
02451: 						PowerHelper.UpdateEnergyCreditSprite(item);
02452: 					}
02453: 					catch
02454: 					{
02455: 					}
02456: 					Core.Log?.Msg("收纳箱 withdraw 池：电量设值失败 give=" + num3 + " back=" + num5 + " cur0=" + num + "（回退出厂电量，池未扣）");
02457: 				}
02458: 			}
02459: 		}
02460: 		catch (Exception ex)
02461: 		{
02462: 			Core.Log?.Msg("收纳箱 withdraw 池：电量分配失败 " + ex.Message);
02463: 		}
02464: 		try
02465: 		{
02466: 			int maxDurability = DurabilityHelper.GetMaxDurability(item);
02467: 			if (maxDurability > 0)
02468: 			{
02469: 				int durabilityPool = StorageGrid.GetDurabilityPool(pack, slot);
02470: 				int num7 = Math.Min(Math.Max(0, durabilityPool), maxDurability);
02471: 				DurabilityHelper.SetCurrentDurability(item, num7);
02472: 				int num8 = -1;
02473: 				int num9 = -1;
02474: 				try
02475: 				{
02476: 					num8 = DurabilityHelper.GetRealDurability(item);
02477: 				}
02478: 				catch
02479: 				{
02480: 				}
02481: 				try
02482: 				{
02483: 					num9 = DurabilityHelper.GetFakeDurability(item);
02484: 				}
02485: 				catch
02486: 				{
02487: 				}
02488: 				if (num8 == num7 || num9 == num7)
02489: 				{
02490: 					StorageGrid.SetDurabilityPool(pack, slot, durabilityPool - num7);
02491: 					Core.Debug("收纳箱 withdraw 池：耐久 give=" + num7 + "/" + maxDurability + "（real=" + num8 + " fake=" + num9 + "）→ 格池 " + (durabilityPool - num7));
02492: 				}
02493: 				else
02494: 				{
02495: 					Core.Debug("收纳箱 withdraw 池：耐久读回不符 give=" + num7 + " real=" + num8 + " fake=" + num9 + "（池未扣）");
02496: 				}
02497: 			}
02498: 		}
02499: 		catch (Exception ex2)
02500: 		{
02501: 			Core.Debug("收纳箱 withdraw 池：耐久分配失败 " + ex2.Message);
02502: 		}
02503: 		try
02504: 		{
02505: 			var (flag, num10, num11) = StorageGrid.ReadItemUseCount(item);
02506: 			if (!flag || num11 <= 0)
02507: 			{
02508: 				return;
02509: 			}
02510: 			int useCountPool = StorageGrid.GetUseCountPool(pack, slot);
02511: 			int num12 = Math.Min(Math.Max(0, useCountPool), num11);
02512: 			UseCountHelper.SetCurrentUseCount(item, num12);
02513: 			int num13 = -1;
02514: 			try
02515: 			{
02516: 				TagSystem state = item.state;
02517: 				num13 = ((state == null) ? ((int?)null) : state.dict?["CURRENT_USE_COUNT_INT"]?.valueInt) ?? (-1);
02518: 			}
02519: 			catch
02520: 			{
02521: 			}
02522: 			if (num13 == num12 || (num12 == 0 && num13 < 0))
02523: 			{
02524: 				StorageGrid.SetUseCountPool(pack, slot, useCountPool - num12);
02525: 				Core.Debug("收纳箱 withdraw 池：次数 give=" + num12 + "/" + num11 + " → 格池 " + (useCountPool - num12));
02526: 				return;
02527: 			}
02528: 			try
02529: 			{
02530: 				UseCountHelper.SetCurrentUseCount(item, num10);
02531: 			}
02532: 			catch
02533: 			{
02534: 			}
02535: 			Core.Log?.Msg("收纳箱 withdraw 池：次数设值失败 give=" + num12 + " back=" + num13 + " cur0=" + num10 + "（回退出厂次数，池未扣）");
02536: 		}
02537: 		catch (Exception ex3)
02538: 		{
02539: 			Core.Log?.Msg("收纳箱 withdraw 池：次数分配失败 " + ex3.Message);
02540: 		}
02541: 		int Rd()
02542: 		{
02543: 			try
02544: 			{
02545: 				return Math.Max(0, PowerHelper.GetAvailableEnergyFromItem(item, false));
02546: 			}
02547: 			catch
02548: 			{
02549: 				return -1;
02550: 			}
02551: 		}
02552: 	}
02553: 
02554: 	private static string TrySpriteKey(GameItem item)
02555: 	{
02556: 		try
02557: 		{
02558: 			return Core.Clean(item.spritePath) ?? "-";
02559: 		}
02560: 		catch
02561: 		{
02562: 			return "-";
02563: 		}
02564: 	}
02565: 
02566: 	private static void InitEnergySlotDisplay(GameItem pack, int slot, GameItem dragged, string itemId)
02567: 	{
02568: 		try
02569: 		{
02570: 			bool flag = false;
02571: 			try
02572: 			{
02573: 				flag = StorageGrid.ReadItemEnergy(dragged).HasValue;
02574: 			}
02575: 			catch
02576: 			{
02577: 			}
02578: 			if (flag)
02579: 			{
02580: 				StorageGrid.SetOrigSprite(pack, slot, StorageGrid.GetSpritePath(pack, slot));
02581: 				ProbeFullSprite(pack, slot, itemId);
02582: 			}
02583: 		}
02584: 		catch
02585: 		{
02586: 		}
02587: 	}
02588: 
02589: 	private static void ProbeFullSprite(GameItem pack, int slot, string itemId)
02590: 	{
02591: 		try
02592: 		{
02593: 			if (!string.IsNullOrEmpty(StorageGrid.GetFullSprite(pack, slot)))
02594: 			{
02595: 				return;
02596: 			}
02597: 			GameItem val = null;
02598: 			try
02599: 			{
02600: 				val = DirectoryMaster.Item(itemId, true);
02601: 			}
02602: 			catch
02603: 			{
02604: 			}
02605: 			if (val == null)
02606: 			{
02607: 				Core.Debug("收纳箱探针：slot=" + slot + " '" + itemId + "' 重建失败，满电 key 待首次取出回收");
02608: 				return;
02609: 			}
02610: 			string text = TrySpriteKey(val);
02611: 			try
02612: 			{
02613: 				PowerHelper.ChargeBattery(val);
02614: 			}
02615: 			catch
02616: 			{
02617: 			}
02618: 			try
02619: 			{
02620: 				PowerHelper.UpdateEnergyCreditSprite(val);
02621: 			}
02622: 			catch
02623: 			{
02624: 			}
02625: 			string text2 = TrySpriteKey(val);
02626: 			int num = 0;
02627: 			try
02628: 			{
02629: 				num = Math.Max(0, PowerHelper.GetAvailableEnergyFromItem(val, false));
02630: 			}
02631: 			catch
02632: 			{
02633: 			}
02634: 			try
02635: 			{
02636: 				val.Destroy();
02637: 			}
02638: 			catch
02639: 			{
02640: 			}
02641: 			Core.Debug("收纳箱探针：slot=" + slot + " before=" + text + " after=" + text2 + " max=" + num);
02642: 			if (num > 0 && text2 != "-" && !string.IsNullOrEmpty(text2))
02643: 			{
02644: 				StorageGrid.SetFullSprite(pack, slot, text2);
02645: 				StorageGrid.SetEnergyMax(pack, slot, num);
02646: 			}
02647: 		}
02648: 		catch (Exception ex)
02649: 		{
02650: 			Core.Debug("收纳箱探针异常：" + ex.Message);
02651: 		}
02652: 	}
02653: 
02654: 	private static void RefreshEnergySlotSprite(GameItem pack, int slot)
02655: 	{
02656: 		try
02657: 		{
02658: 			string fullSprite = StorageGrid.GetFullSprite(pack, slot);
02659: 			int energyMax = StorageGrid.GetEnergyMax(pack, slot);
02660: 			if (string.IsNullOrEmpty(fullSprite) || energyMax <= 0)
02661: 			{
02662: 				return;
02663: 			}
02664: 			int energyPool = StorageGrid.GetEnergyPool(pack, slot);
02665: 			string text = null;
02666: 			int num = Math.Min(Math.Max(0, energyPool), energyMax);
02667: 			if (num >= energyMax)
02668: 			{
02669: 				text = fullSprite;
02670: 			}
02671: 			else
02672: 			{
02673: 				int num2 = num * 100 / energyMax;
02674: 				int num3 = ((num2 >= 90) ? 100 : ((num2 >= 70) ? 80 : ((num2 >= 50) ? 60 : ((num2 >= 30) ? 40 : ((num2 >= 10) ? 20 : ((num2 >= 1) ? 10 : 0))))));
02675: 				Match match = Regex.Match(fullSprite, "^(.*?)(\\d+)$");
02676: 				if (match.Success)
02677: 				{
02678: 					string text2 = match.Groups[1].Value + num3;
02679: 					if (BoxWindow.IsSpriteResolvable(StorageGrid.GetSpriteAtlas(pack, slot), text2))
02680: 					{
02681: 						text = text2;
02682: 					}
02683: 				}
02684: 			}
02685: 			if (!string.IsNullOrEmpty(text) && !string.Equals(StorageGrid.GetSpritePath(pack, slot), text, StringComparison.Ordinal))
02686: 			{
02687: 				StorageGrid.SetSpriteRef(pack, slot, StorageGrid.GetSpriteAtlas(pack, slot), text);
02688: 				Core.Debug("收纳箱格池贴图：slot=" + slot + " pool=" + energyPool + " next=" + num + "/" + energyMax + " → " + text);
02689: 			}
02690: 		}
02691: 		catch
02692: 		{
02693: 		}
02694: 	}
02695: 
02696: 	private static void RealizeTruthBeforeDeposit(GameItem item)
02697: 	{
02698: 		ItemFeature val = StorageGrid.FindConditionFeature(item);
02699: 		string text = "";
02700: 		try
02701: 		{
02702: 			text = Core.Clean(val?.realCondition?.identifier) ?? "";
02703: 		}
02704: 		catch
02705: 		{
02706: 		}
02707: 		if (string.IsNullOrEmpty(text) || !CondMap.TryGetValue(text, out CondEntry value))
02708: 		{
02709: 			return;
02710: 		}
02711: 		Action<GameItem> apply = value.Apply;
02712: 		bool flag = apply == null;
02713: 		try
02714: 		{
02715: 			ItemFeature val2;
02716: 			if (flag)
02717: 			{
02718: 				try
02719: 				{
02720: 					val2 = ChemicalProductHelper.GetChemicalFeature(item);
02721: 				}
02722: 				catch
02723: 				{
02724: 					val2 = null;
02725: 				}
02726: 			}
02727: 			else
02728: 			{
02729: 				val2 = StorageGrid.FindConditionFeature(item);
02730: 			}
02731: 			string b = "";
02732: 			try
02733: 			{
02734: 				b = Core.Clean(val2?.fakeCondition?.identifier) ?? "";
02735: 			}
02736: 			catch
02737: 			{
02738: 			}
02739: 			long num = -1L;
02740: 			try
02741: 			{
02742: 				num = item.unitValue;
02743: 			}
02744: 			catch
02745: 			{
02746: 			}
02747: 			if (flag)
02748: 			{
02749: 				ItemCondition val3 = value.Factory();
02750: 				if (val3 == null)
02751: 				{
02752: 					return;
02753: 				}
02754: 				bool flag2 = false;
02755: 				try
02756: 				{
02757: 					ChemicalProductHelper.SetPurityFeature(item, val3, val3);
~~~

~~~csharp
02770: 					{
02771: 						return;
02772: 					}
02773: 				}
02774: 			}
02775: 			else
02776: 			{
02777: 				apply?.Invoke(item);
02778: 			}
02779: 			ItemFeature val4;
02780: 			if (flag)
02781: 			{
02782: 				try
02783: 				{
02784: 					val4 = ChemicalProductHelper.GetChemicalFeature(item);
02785: 				}
02786: 				catch
02787: 				{
02788: 					val4 = null;
02789: 				}
02790: 			}
02791: 			else
02792: 			{
02793: 				val4 = StorageGrid.FindConditionFeature(item);
02794: 			}
02795: 			string text2 = "";
02796: 			try
02797: 			{
02798: 				text2 = Core.Clean(val4?.realCondition?.identifier) ?? "";
02799: 			}
02800: 			catch
02801: 			{
02802: 			}
02803: 			if (!string.Equals(text2, text, StringComparison.Ordinal))
02804: 			{
02805: 				Core.Debug("收纳箱 deposit 实化：'" + text + "' 真值读回不符 back='" + text2 + "'（放弃实化，按现状存入）");
02806: 				return;
02807: 			}
02808: 			string a = "";
02809: 			try
02810: 			{
02811: 				a = Core.Clean(val4?.fakeCondition?.identifier) ?? "";
02812: 			}
02813: 			catch
02814: 			{
02815: 			}
02816: 			if (!string.Equals(a, text, StringComparison.Ordinal))
02817: 			{
02818: 				try
02819: 				{
02820: 					ItemCondition val5 = value.Factory();
02821: 					if (val4 != null)
02822: 					{
02823: 						val4.SetConditions(val5, val5);
02824: 					}
02825: 					a = text;
02826: 					Core.Debug("收纳箱 deposit 实化：'" + text + "' 标签归真值（Helper 未动 fake，手动双写）");
02827: 				}
02828: 				catch
02829: 				{
02830: 				}
02831: 			}
02832: 			long num2 = -1L;
02833: 			try
02834: 			{
02835: 				num2 = item.unitValue;
02836: 			}
02837: 			catch
02838: 			{
02839: 			}
02840: 			long num3 = -1L;
02841: 			try
02842: 			{
02843: 				num3 = item.unitBaseValue;
02844: 			}
02845: 			catch
02846: 			{
02847: 			}
02848: 			bool flag3 = !string.Equals(a, b, StringComparison.Ordinal);
02849: 			if (flag)
02850: 			{
02851: 				Core.Debug("收纳箱 deposit 实化：'" + text + "' 化学品缓存 " + num2 + "/基础字段 " + num3 + "（纯度修正走官方动态管线，不写缓存）");
02852: 			}
02853: 			else
02854: 			{
02855: 				if (flag3 && num >= 0 && num2 == num)
02856: 				{
02857: 					string text3 = "";
02858: 					try
02859: 					{
02860: 						text3 = Core.Clean(item.identifier) ?? "";
02861: 					}
02862: 					catch
02863: 					{
02864: 					}
02865: 					int num4 = 0;
02866: 					try
02867: 					{
02868: 						num4 = value.Factory()?.modValueDisplay ?? 0;
02869: 					}
02870: 					catch
02871: 					{
02872: 					}
02873: 					if (num4 == 0)
02874: 					{
02875: 						if (string.Equals(text, "genuineCigarette", StringComparison.Ordinal))
02876: 						{
02877: 							bool flag4 = false;
02878: 							try
02879: 							{
02880: 								TagSystem state = item.state;
02881: 								flag4 = ((state == null) ? ((bool?)null) : state.dict?.ContainsKey("LUXURY_ITEM")) == true;
02882: 							}
02883: 							catch
02884: 							{
02885: 							}
02886: 							long num5 = (flag4 ? 100 : 80);
02887: 							if (num2 > 0 && num2 != num5)
02888: 							{
02889: 								try
02890: 								{
02891: 									item.SetValue(num5);
02892: 								}
02893: 								catch
02894: 								{
02895: 								}
02896: 								Core.Debug("收纳箱 deposit 实化形态归一：'genuineCigarette' 价 " + num2 + " → " + num5 + (flag4 ? "（奢形态）" : "（素形态）") + "（贴标贬值价复位）");
02897: 							}
02898: 							try
02899: 							{
02900: 								string text4 = item.name ?? "";
02901: 								if (!text4.Contains("（赝品）"))
02902: 								{
02903: 									return;
02904: 								}
02905: 								string text5 = "";
02906: 								GameItem val6 = null;
02907: 								try
02908: 								{
02909: 									val6 = DirectoryMaster.Item("packet_red_cigarette", true);
02910: 								}
02911: 								catch
02912: 								{
02913: 								}
02914: 								try
02915: 								{
02916: 									text5 = ((val6 != null) ? val6.name : null) ?? "";
02917: 								}
02918: 								catch
02919: 								{
02920: 								}
02921: 								try
02922: 								{
02923: 									if (val6 != null)
02924: 									{
02925: 										val6.Destroy();
02926: 									}
02927: 								}
02928: 								catch
02929: 								{
02930: 								}
02931: 								if (!string.IsNullOrEmpty(text5))
02932: 								{
02933: 									try
02934: 									{
02935: 										item.name = text5;
02936: 									}
02937: 									catch
02938: 									{
02939: 									}
02940: 									Core.Debug("收纳箱 deposit 实化名字归一：'" + text4 + "' → '" + text5 + "'");
02941: 								}
02942: 								return;
02943: 							}
02944: 							catch
02945: 							{
02946: 								return;
02947: 							}
02948: 						}
02949: 						long num6 = -1L;
02950: 						GameItem val7 = null;
02951: 						try
02952: 						{
02953: 							val7 = DirectoryMaster.Item(text3, true);
02954: 						}
02955: 						catch
02956: 						{
02957: 						}
02958: 						try
02959: 						{
02960: 							num6 = ((val7 != null) ? val7.unitValue : (-1));
02961: 						}
02962: 						catch
02963: 						{
02964: 						}
02965: 						try
02966: 						{
02967: 							if (val7 != null)
02968: 							{
02969: 								val7.Destroy();
02970: 							}
02971: 						}
02972: 						catch
02973: 						{
02974: 						}
02975: 						if (num6 > 0 && num2 != num6)
02976: 						{
02977: 							try
02978: 							{
02979: 								item.SetValue(num6);
02980: 							}
02981: 							catch
02982: 							{
02983: 							}
02984: 							Core.Debug("收纳箱 deposit 实化形态归一：'" + text + "' 价 " + num2 + " → 模板价 " + num6 + "（零差价真值形态，贴标伪装价复位）");
02985: 						}
02986: 						else
02987: 						{
02988: 							Core.Debug("收纳箱 deposit 实化定价：'" + text + "' modDisplay=0 跳过（价已是模板价 " + num2 + "）");
02989: 						}
02990: 						return;
02991: 					}
02992: 					long num7 = -1L;
02993: 					Func<GameItem> priceProbe = value.PriceProbe;
02994: 					if (priceProbe != null)
02995: 					{
02996: 						GameItem val8 = null;
02997: 						try
02998: 						{
02999: 							val8 = priceProbe();
03000: 							if (val8 != null)
03001: 							{
03002: 								try
03003: 								{
03004: 									num7 = val8.unitValue;
03005: 								}
03006: 								catch
03007: 								{
03008: 								}
03009: 							}
03010: 						}
03011: 						catch
03012: 						{
03013: 						}
03014: 						finally
03015: 						{
03016: 							try
03017: 							{
03018: 								if (val8 != null)
03019: 								{
03020: 									val8.Destroy();
03021: 								}
03022: 							}
03023: 							catch
03024: 							{
03025: 							}
03026: 						}
03027: 					}
03028: 					long num8 = -1L;
03029: 					GameItem val9 = null;
03030: 					try
03031: 					{
03032: 						val9 = DirectoryMaster.Item(text3, true);
03033: 					}
03034: 					catch
03035: 					{
03036: 					}
03037: 					if (val9 != null)
03038: 					{
03039: 						try
03040: 						{
03041: 							num8 = val9.unitValue;
03042: 						}
03043: 						catch
03044: 						{
03045: 						}
03046: 						try
03047: 						{
03048: 							val9.Destroy();
03049: 						}
03050: 						catch
03051: 						{
03052: 						}
03053: 					}
03054: 					long num9 = -1L;
03055: 					try
03056: 					{
03057: 						num9 = item.GetBaseValue();
03058: 					}
03059: 					catch
03060: 					{
03061: 					}
03062: 					long num10 = ((num8 > 0) ? ((long)Math.Round((double)num8 * (1.0 + (double)num4 / 100.0))) : (-1));
03063: 					long num11 = ((num7 > 0 && num10 > 0 && num7 != num8) ? num7 : num10);
03064: 					Core.Debug("收纳箱 deposit 实化定价侦查：'" + text + "' 模板价=" + num8 + " GetBaseValue=" + num9 + " modDisplay=" + num4 + " probe价=" + num7 + " 公式价=" + num10 + " 当前=" + num2 + "/基础字段=" + num3);
03065: 					if (num11 <= 0 || num11 == num2)
03066: 					{
03067: 						return;
03068: 					}
03069: 					try
03070: 					{
03071: 						item.SetValue(num11);
03072: 						long num12 = -1L;
03073: 						try
03074: 						{
03075: 							num12 = item.unitValue;
03076: 						}
03077: 						catch
03078: 						{
03079: 						}
03080: 						Core.Debug("收纳箱 deposit 实化定价：'" + text + "' 价值 " + num2 + " → " + num11 + "（写回读回 " + num12 + "）");
03081: 						return;
03082: 					}
03083: 					catch (Exception ex)
03084: 					{
03085: 						Core.Debug("收纳箱 deposit 实化定价失败：'" + text + "' " + ex.Message + "（维持快照价值）");
03086: 						return;
03087: 					}
03088: 				}
03089: 				if (num >= 0 && num2 != num)
03090: 				{
03091: 					Core.Debug("收纳箱 deposit 实化：'" + text + "' 价值 " + num + " → " + num2 + "（官方重算）");
03092: 				}
03093: 			}
03094: 		}
03095: 		catch (Exception ex2)
03096: 		{
03097: 			Core.Debug("收纳箱 deposit 实化异常：" + ex2.Message + "（不影响存入）");
03098: 		}
03099: 	}
03100: 
03101: 	private static GameItem? TryBuildFactoryItem(string itemId, string? storedPur)
03102: 	{
03103: 		if (!NullGuard.HasText(itemId) || !NullGuard.HasText(storedPur))
03104: 		{
03105: 			return null;
03106: 		}
03107: 		int num = storedPur.IndexOf('\u0001');
03108: 		string text = ((num < 0) ? storedPur : storedPur.Substring(0, num));
03109: 		if (string.IsNullOrEmpty(text))
03110: 		{
03111: 			return null;
03112: 		}
03113: 		if (!CondMap.TryGetValue(text, out CondEntry value) || value.PriceProbe == null)
03114: 		{
03115: 			return null;
03116: 		}
03117: 		GameItem val = null;
03118: 		try
03119: 		{
03120: 			val = value.PriceProbe();
03121: 		}
03122: 		catch
03123: 		{
03124: 			return null;
03125: 		}
03126: 		if (val == null)
03127: 		{
03128: 			return null;
03129: 		}
03130: 		string text2 = "";
03131: 		string text3 = "";
03132: 		long num2 = -1L;
03133: 		try
03134: 		{
03135: 			text2 = Core.Clean(val.identifier) ?? "";
03136: 		}
03137: 		catch
03138: 		{
03139: 		}
03140: 		try
03141: 		{
03142: 			text3 = Core.Clean(StorageGrid.FindConditionFeature(val)?.realCondition?.identifier) ?? "";
03143: 		}
03144: 		catch
03145: 		{
03146: 		}
03147: 		try
03148: 		{
03149: 			num2 = val.unitValue;
03150: 		}
03151: 		catch
03152: 		{
03153: 		}
03154: 		bool flag = string.Equals(text2, itemId, StringComparison.Ordinal) && string.Equals(text3, text, StringComparison.Ordinal);
03155: 		Core.Debug("收纳箱 withdraw 工厂直发侦查：'" + text + "' 工厂产物 id='" + text2 + "'(期望'" + itemId + "') real='" + text3 + "' value=" + num2 + " → " + (flag ? "采用（官方产线针）" : "不采用（退重建路径）"));
03156: 		if (!flag)
03157: 		{
03158: 			try
03159: 			{
03160: 				val.Destroy();
03161: 			}
03162: 			catch
03163: 			{
03164: 			}
03165: 			return null;
03166: 		}
03167: 		return val;
03168: 	}
03169: 
03170: 	private static GameItem? LaunderGenuineCigarette()
03171: 	{
03172: 		GameItem val = null;
03173: 		try
03174: 		{
03175: 			val = PreBuiltItemHelper.FakeCigarette();
03176: 		}
03177: 		catch
03178: 		{
03179: 			return null;
03180: 		}
03181: 		if (val == null)
03182: 		{
03183: 			return null;
03184: 		}
03185: 		try
03186: 		{
03187: 			ItemFeature val2 = StorageGrid.FindConditionFeature(val);
03188: 			ItemCondition val3 = ItemConditionList.CreateGenuineCigarette();
03189: 			if (val2 != null && val3 != null)
03190: 			{
03191: 				val2.SetConditions(val3, val3);
03192: 			}
03193: 		}
03194: 		catch
03195: 		{
03196: 		}
03197: 		try
03198: 		{
03199: 			string[] array = new string[5] { "CIGARETTE_SEAL_ISSUE", "CIGARETTE_LOGO_ISSUE", "CIGARETTE_LETTERING_ISSUE", "CIGARETTE_BOX_ISSUE", "CIGARETTE_COLOR_ISSUE" };
03200: 			foreach (string key in array)
03201: 			{
03202: 				TagSystem state = val.state;
03203: 				if (state != null)
03204: 				{
03205: 					state.dict?.Remove(key);
03206: 				}
03207: 			}
03208: 		}
03209: 		catch
03210: 		{
03211: 		}
03212: 		return val;
03213: 	}
03214: 
03215: 	private static void TryRegister(Dictionary<string, CondEntry> map, Func<ItemCondition> factory, Action<GameItem>? apply = null, bool isOfficialHelper = false, Func<GameItem?>? priceProbe = null)
03216: 	{
03217: 		try
03218: 		{
03219: 			string text = Core.Clean(factory()?.identifier);
03220: 			if (!string.IsNullOrEmpty(text))
03221: 			{
03222: 				map[text] = new CondEntry
03223: 				{
03224: 					Factory = factory,
~~~

~~~csharp
03237: 	{
03238: 		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
03239: 		//IL_0327: Expected O, but got Unknown
03240: 		try
03241: 		{
03242: 			if (string.IsNullOrEmpty(stored))
03243: 			{
03244: 				return;
03245: 			}
03246: 			int num = stored.IndexOf('\u0001');
03247: 			string text = ((num < 0) ? stored : stored.Substring(0, num));
03248: 			bool flag = num >= 0;
03249: 			CondEntry condEntry = null;
03250: 			if (CondMap.TryGetValue(text, out CondEntry value))
03251: 			{
03252: 				condEntry = value;
03253: 			}
03254: 			if (condEntry != null)
03255: 			{
03256: 				if (condEntry.Apply != null)
03257: 				{
03258: 					try
03259: 					{
03260: 						condEntry.Apply(item);
03261: 						string text2 = "";
03262: 						ItemFeature val = StorageGrid.FindConditionFeature(item);
03263: 						try
03264: 						{
03265: 							text2 = Core.Clean(val?.realCondition?.identifier) ?? "";
03266: 						}
03267: 						catch
03268: 						{
03269: 						}
03270: 						if (string.Equals(text2, text, StringComparison.Ordinal))
03271: 						{
03272: 							Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（体系专属 Helper 写回）");
03273: 							return;
03274: 						}
03275: 						Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（Helper 写回读回不一致 back=" + text2 + "，落通用克隆）");
03276: 					}
03277: 					catch (Exception ex)
03278: 					{
03279: 						Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（体系 Helper 异常：" + ex.Message + "，落通用克隆）");
03280: 					}
03281: 				}
03282: 				else
03283: 				{
03284: 					ItemCondition val2 = condEntry.Factory();
03285: 					if (val2 != null)
03286: 					{
03287: 						try
03288: 						{
03289: 							ChemicalProductHelper.SetPurityFeature(item, val2, val2);
03290: 							string a = "";
03291: 							try
03292: 							{
03293: 								a = Core.Clean(ChemicalProductHelper.GetChemicalFeature(item)?.realCondition?.identifier) ?? "";
03294: 							}
03295: 							catch
03296: 							{
03297: 							}
03298: 							if (string.Equals(a, text, StringComparison.Ordinal))
03299: 							{
03300: 								Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（SetPurityFeature 三参双写 real+fake）");
03301: 								return;
03302: 							}
03303: 						}
03304: 						catch
03305: 						{
03306: 						}
03307: 						try
03308: 						{
03309: 							ChemicalProductHelper.SetPurityFeature(item, val2);
03310: 							string a2 = "";
03311: 							try
03312: 							{
03313: 								a2 = Core.Clean(ChemicalProductHelper.GetCurrentChemicalConditionId(item)) ?? "";
03314: 							}
03315: 							catch
03316: 							{
03317: 							}
03318: 							if (string.Equals(a2, text, StringComparison.Ordinal))
03319: 							{
03320: 								Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（SetPurityFeature）");
03321: 								return;
03322: 							}
03323: 						}
03324: 						catch
03325: 						{
03326: 						}
03327: 					}
03328: 				}
03329: 			}
03330: 			if (flag)
03331: 			{
03332: 				try
03333: 				{
03334: 					ItemCondition val3 = StorageGrid.DeserializeCondition(stored);
03335: 					if (val3 != null)
03336: 					{
03337: 						ItemCondition val4 = null;
03338: 						try
03339: 						{
03340: 							val4 = condEntry?.Factory?.Invoke();
03341: 						}
03342: 						catch
03343: 						{
03344: 						}
03345: 						if (val4 == null)
03346: 						{
03347: 							val4 = val3;
03348: 						}
03349: 						string text3 = "";
03350: 						try
03351: 						{
03352: 							text3 = Core.Clean(val3.category) ?? "";
03353: 						}
03354: 						catch
03355: 						{
03356: 						}
03357: 						ItemFeature val5 = null;
03358: 						List<ItemFeature> itemFeatures = item.itemFeatures;
03359: 						if (itemFeatures != null)
03360: 						{
03361: 							int num2 = 0;
03362: 							try
03363: 							{
03364: 								num2 = itemFeatures.Count;
03365: 							}
03366: 							catch
03367: 							{
03368: 							}
03369: 							for (int i = 0; i < num2; i++)
03370: 							{
03371: 								ItemFeature val6 = null;
03372: 								try
03373: 								{
03374: 									val6 = itemFeatures[i];
03375: 								}
03376: 								catch
03377: 								{
03378: 									continue;
03379: 								}
03380: 								if (val6 == null)
03381: 								{
03382: 									continue;
03383: 								}
03384: 								string a3 = "";
03385: 								try
03386: 								{
03387: 									a3 = Core.Clean(val6.category) ?? "";
03388: 								}
03389: 								catch
~~~

~~~csharp
03408: 								}
03409: 								goto IL_02fc;
03410: 								IL_02fc:
03411: 								val5 = val6;
03412: 								break;
03413: 							}
03414: 						}
03415: 						bool flag2 = false;
03416: 						if (val5 == null && !string.IsNullOrEmpty(text3))
03417: 						{
03418: 							ItemFeature val7 = new ItemFeature(text3);
03419: 							val7.identifier = val4.identifier;
03420: 							val7.useCondition = true;
03421: 							val7.realCondition = val4;
03422: 							val7.fakeCondition = val4;
03423: 							if (itemFeatures != null)
03424: 							{
03425: 								itemFeatures.Add(val7);
03426: 								val5 = val7;
03427: 								flag2 = true;
03428: 							}
03429: 						}
03430: 						if (val5 != null)
03431: 						{
03432: 							val5.SetConditions(val4, val4);
03433: 							string text4 = "";
03434: 							try
03435: 							{
03436: 								text4 = Core.Clean(val5.realCondition?.identifier) ?? "";
03437: 							}
03438: 							catch
03439: 							{
03440: 							}
03441: 							if (string.Equals(text4, text, StringComparison.Ordinal))
03442: 							{
03443: 								Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（通用克隆 SetConditions" + (flag2 ? ("，新建 feature cat=" + text3) : "") + "）");
03444: 								return;
03445: 							}
03446: 							Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（通用克隆读回不一致 back=" + text4 + "）");
03447: 						}
03448: 						else
03449: 						{
03450: 							Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（无 category 且无现有 feature，无法挂载）");
03451: 						}
03452: 					}
03453: 				}
03454: 				catch
03455: 				{
03456: 				}
03457: 			}
03458: 			try
03459: 			{
03460: 				bool flag3 = text.IndexOf("cigarette", StringComparison.OrdinalIgnoreCase) >= 0;
03461: 				if (flag3 && text.IndexOf("genuine", StringComparison.OrdinalIgnoreCase) >= 0)
03462: 				{
03463: 					InsCigaretteHelper.SetGenuine(item);
03464: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（香烟 SetGenuine）");
03465: 				}
03466: 				else if (flag3 && text.IndexOf("counterfeit", StringComparison.OrdinalIgnoreCase) >= 0)
03467: 				{
03468: 					ItemCondition val8 = ItemConditionList.CreateCounterfeitCigarette();
03469: 					ItemFeature val9 = StorageGrid.FindConditionFeature(item);
03470: 					if (val9 != null)
03471: 					{
03472: 						val9.SetConditions(val8, val8);
03473: 					}
03474: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（香烟赝品双写）");
03475: 				}
03476: 				else if (!flag3 && text.IndexOf("genuine", StringComparison.OrdinalIgnoreCase) >= 0)
03477: 				{
03478: 					InsInjectorHelper.SetGenuine(item);
03479: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetGenuine）");
03480: 				}
03481: 				else if (!flag3 && text.IndexOf("counterfeit", StringComparison.OrdinalIgnoreCase) >= 0)
03482: 				{
03483: 					InsInjectorHelper.SetCounterfeit(item);
03484: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetCounterfeit）");
03485: 				}
03486: 				else if (!flag3 && text.IndexOf("expired", StringComparison.OrdinalIgnoreCase) >= 0)
03487: 				{
03488: 					InsInjectorHelper.SetExpired(item);
03489: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetExpired）");
03490: 				}
03491: 				else if (!flag3 && text.IndexOf("unusable", StringComparison.OrdinalIgnoreCase) >= 0)
03492: 				{
03493: 					InsInjectorHelper.SetUnusable(item);
03494: 					Core.Debug("收纳箱 withdraw：条件还原 '" + text + "'（注射器 SetUnusable）");
03495: 				}
03496: 			}
03497: 			catch
03498: 			{
03499: 			}
03500: 		}
03501: 		catch (Exception ex2)
03502: 		{
03503: 			Core.Debug("收纳箱 withdraw：条件还原失败 '" + stored + "'：" + ex2.Message);
03504: 		}
03505: 	}
03506: 
03507: 	internal static string? PurityShortName(string? stored, int maxChars = 3)
03508: 	{
03509: 		try
03510: 		{
03511: 			if (!NullGuard.HasText(stored))
03512: 			{
03513: 				return null;
03514: 			}
03515: 			int num = stored.IndexOf('\u0001');
03516: 			if (num >= 0)
03517: 			{
03518: 				string text = stored.Substring(num + 1);
03519: 				int num2 = text.IndexOf('\u0001');
03520: 				if (num2 >= 0)
03521: 				{
03522: 					text = text.Substring(0, num2);
03523: 				}
03524: 				text = Core.Clean(text) ?? "";
03525: 				if (!string.IsNullOrEmpty(text))
03526: 				{
03527: 					if (text.Length > maxChars)
03528: 					{
03529: 						text = text.Substring(0, maxChars);
03530: 					}
03531: 					while (text.Length < maxChars)
03532: 					{
03533: 						text += "\u3000";
03534: 					}
03535: 					return text;
03536: 				}
03537: 			}
03538: 			if (CondMap.TryGetValue(stored, out CondEntry value))
03539: 			{
03540: 				string text2 = Core.Clean(value.Factory()?.display);
03541: 				if (string.IsNullOrEmpty(text2))
03542: 				{
03543: 					return null;
03544: 				}
03545: 				if (text2.Length > maxChars)
03546: 				{
03547: 					text2 = text2.Substring(0, maxChars);
03548: 				}
03549: 				while (text2.Length < maxChars)
03550: 				{
03551: 					text2 += "\u3000";
03552: 				}
03553: 				return text2;
03554: 			}
03555: 		}
03556: 		catch
03557: 		{
03558: 		}
03559: 		return null;
03560: 	}
03561: 
03562: 	private static string? TryReadSpriteAtlas(GameItem item)
03563: 	{
03564: 		try
03565: 		{
03566: 			return Core.Clean(item.spriteAtlasPath);
03567: 		}
03568: 		catch
03569: 		{
03570: 			return null;
03571: 		}
03572: 	}
~~~

~~~csharp
03731: 			}
03732: 			catch
03733: 			{
03734: 			}
03735: 			text3 = text2;
03736: 		}
03737: 		else
03738: 		{
03739: 			text3 = TryReadName(item) ?? "";
03740: 		}
03741: 		return text3 + "|" + (hasValue ? "" : (TryReadSpriteAtlas(item) ?? "")) + "|" + (hasValue ? "" : (TryReadSpritePath(item) ?? "")) + "|" + ((hasValue || flag) ? "" : (TryReadUnitValue(item)?.ToString() ?? "")) + "|" + StorageGrid.ReadItemCalorie(item) + "|" + StorageGrid.ReadItemShape(item) + "|" + (flag ? "" : (StorageGrid.ReadItemPurity(item) ?? "")) + text;
03742: 	}
03743: 
03744: 	private static void ConsumeDragged(GameItem dragged, int take, int units)
03745: 	{
03746: 		int num = Math.Max(0, units - take);
03747: 		try
03748: 		{
03749: 			dragged.unitCount = num;
03750: 			if (num <= 0)
03751: 			{
03752: 				try
03753: 				{
03754: 					dragged.Destroy();
03755: 				}
03756: 				catch
03757: 				{
03758: 					try
03759: 					{
03760: 						dragged.unitCount = 0;
03761: 					}
03762: 					catch
03763: 					{
03764: 					}
03765: 				}
03766: 				Core.Debug("收纳箱：消耗拖入物 unitCount→0 + Destroy");
03767: 			}
03768: 			else
03769: 			{
03770: 				Core.Debug("收纳箱：消耗拖入物 unitCount " + units + "→" + num);
03771: 			}
03772: 		}
03773: 		catch (Exception ex)
03774: 		{
03775: 			Core.Log?.Warning("收纳箱 ConsumeDragged：" + ex.Message);
03776: 		}
03777: 	}
03778: 
03779: 	internal static bool HasContentInside(GameItem it)
03780: 	{
03781: 		try
03782: 		{
03783: 			bool flag = false;
03784: 			bool flag2 = false;
03785: 			try
03786: 			{
03787: 				flag = it.GetCurrentChildValue(false, false, false, true) > 0;
03788: 			}
03789: 			catch
03790: 			{
03791: 			}
03792: 			try
03793: 			{
03794: 				List<GameItem> list = it.FindAllChildItems();
03795: 				flag2 = list != null && list.Count > 0;
03796: 			}
03797: 			catch
03798: 			{
03799: 			}
03800: 			if (flag != flag2)
03801: 			{
03802: 				try
03803: 				{
03804: 					Core.Debug("收纳箱 容器内物品双信号不一致：id=" + (Core.Clean(it.identifier) ?? "?") + " 按价值=" + flag + " 按列表=" + flag2);
03805: 				}
03806: 				catch
03807: 				{
03808: 				}
03809: 			}
03810: 			return flag || flag2;
03811: 		}
03812: 		catch
03813: 		{
03814: 			return false;
03815: 		}
03816: 	}
03817: 
03818: 	private static void TryRestoreBoughtPrice(GameItem pack, int slot, GameItem item)
03819: 	{
03820: 		try
03821: 		{
03822: 			try
03823: 			{
03824: 				if (StorageGrid.FindConditionFeature(item) == null)
03825: 				{
03826: 					return;
03827: 				}
03828: 			}
03829: 			catch
03830: 			{
03831: 			}
03832: 			long valueOrDefault = StorageGrid.GetUnitValue(pack, slot).GetValueOrDefault();
03833: 			if (valueOrDefault <= 0)
03834: 			{
03835: 				return;
03836: 			}
03837: 			try
03838: 			{
03839: 				TagSystem state = item.state;
03840: 				if (((state == null) ? ((bool?)null) : state.dict?.ContainsKey("BOUGHT_PRICE_TAG")) == true)
03841: 				{
03842: 					return;
03843: 				}
03844: 			}
03845: 			catch
03846: 			{
03847: 			}
03848: 			string text = "-";
03849: 			try
03850: 			{
03851: 				text = Core.Clean(item.identifier) ?? "-";
03852: 			}
03853: 			catch
03854: 			{
03855: 			}
03856: 			if (TryWriteBoughtTag(item, (int)Math.Min(valueOrDefault, 2147483647L), out string via))
03857: 			{
03858: 				Core.Debug("收纳箱 withdraw 买入价补写（基础价值 " + valueOrDefault + "）：'" + text + "'（" + via + "）——官方标签机激活链依赖该标签");
03859: 			}
03860: 			else
03861: 			{
03862: 				Core.Debug("收纳箱 withdraw 买入价补写失败：'" + text + "' 期望值=" + valueOrDefault + "（标签机改标可能仍报错，日志见写入路径异常）");
03863: 			}
03864: 		}
03865: 		catch
03866: 		{
03867: 		}
03868: 	}
03869: 
03870: 	private static bool TryWriteBoughtTag(GameItem item, int value, out string via)
03871: 	{
03872: 		try
03873: 		{
03874: 			TagSystem state = item.state;
03875: 			Dictionary<string, TagState> dictionary = ((state != null) ? state.dict : null);
03876: 			if (dictionary != null)
03877: 			{
03878: 				if (!dictionary.ContainsKey("BOUGHT_PRICE_TAG"))
03879: 				{
03880: 					try
03881: 					{
03882: 						item.EnableTag("BOUGHT_PRICE_TAG", false);
03883: 					}
03884: 					catch
03885: 					{
03886: 					}
03887: 				}
03888: 				TagState val = (dictionary.ContainsKey("BOUGHT_PRICE_TAG") ? dictionary["BOUGHT_PRICE_TAG"] : null);
03889: 				if (val != null)
03890: 				{
03891: 					try
03892: 					{
03893: 						val.Enable();
03894: 					}
03895: 					catch
03896: 					{
03897: 					}
03898: 					try
03899: 					{
03900: 						val.SetInt(value);
03901: 					}
03902: 					catch
03903: 					{
03904: 					}
03905: 					if (VerifyBoughtTag(item, value))
03906: 					{
03907: 						via = "dict直写";
03908: 						return true;
03909: 					}
03910: 				}
03911: 			}
03912: 		}
03913: 		catch
03914: 		{
03915: 		}
03916: 		try
03917: 		{
03918: 			item.ModifyTag("BOUGHT_PRICE_TAG", (Action<TagState>)delegate(TagState t)
03919: 			{
03920: 				try
03921: 				{
03922: 					if (t != null)
03923: 					{
03924: 						t.Enable();
03925: 					}
03926: 				}
03927: 				catch
03928: 				{
03929: 				}
03930: 				try
03931: 				{
~~~

### ItemStorage/BoxLabel.cs

~~~csharp
00011: {
00012: 	private sealed class Entry
00013: 	{
00014: 		internal int Uid;
00015: 
00016: 		internal string BaseName = "";
00017: 
00018: 		internal float LastApplyAt = -99f;
00019: 
00020: 		internal bool PendingItem;
00021: 
00022: 		internal TMP_Text? Label;
00023: 
00024: 		internal GameItemElement? El;
00025: 
00026: 		internal RectTransform? RootRt;
00027: 
00028: 		internal RectTransform? IconRt;
00029: 	}
00030: 
00031: 	private sealed class Anchor
00032: 	{
00033: 		internal readonly RectTransform Rt;
00034: 
00035: 		internal readonly string Kind;
00036: 
00037: 		internal Anchor(RectTransform rt, string kind)
00038: 		{
00039: 			Rt = rt;
00040: 			Kind = kind;
00041: 		}
00042: 	}
00043: 
00044: 	private const int MaxChars = 3;
00045: 
00046: 	private const string DefaultText = "收纳箱";
00047: 
00048: 	private const float FontRatioVertical = 0.23f;
00049: 
00050: 	private const float FontRatioHorizontal = 0.35f;
00051: 
00052: 	private const float Throttle = 0.1f;
00053: 
00054: 	private const float FindInterval = 2f;
00055: 
00056: 	private const float MissTtl = 10f;
00057: 
00058: 	private const string HideNamesKey = "BoxLabelHideNames";
00059: 
00060: 	private static readonly Vector3[] Corners = (Vector3[])(object)new Vector3[4];
00061: 
00062: 	private static readonly Dictionary<GameItemElement, Entry> Entries = new Dictionary<GameItemElement, Entry>();
00063: 
00064: 	private static readonly Dictionary<int, (GameItem? Box, float At)> ItemCache = new Dictionary<int, (GameItem, float)>();
00065: 
00066: 	private static readonly HashSet<int> DragUids = new HashSet<int>();
00067: 
00068: 	private static readonly HashSet<int> LoggedOnce = new HashSet<int>();
00069: 
00070: 	private static float _nextFindAt;
00071: 
00072: 	private static float _nextTickAt;
00073: 
00074: 	private static float _nextAttachAt;
00075: 
00076: 	private static TMP_FontAsset? _tmpFont;
00077: 
00078: 	private static string HideKeyFor(int uid)
00079: 	{
00080: 		return "BoxLabelHideNames." + uid;
00081: 	}
00082: 
00083: 	internal static bool IsShownFor(int uid)
00084: 	{
00085: 		PlayerStore instance = PlayerStore.Instance;
00086: 		if (instance == null)
00087: 		{
00088: 			return true;
00089: 		}
00090: 		try
00091: 		{
00092: 			return PerSaveState.GetInt(instance, HideKeyFor(uid)) != 1;
00093: 		}
00094: 		catch
00095: 		{
00096: 			return true;
00097: 		}
00098: 	}
00099: 
00100: 	internal static void ToggleFor(int uid)
00101: 	{
00102: 		int num = (IsShownFor(uid) ? 1 : 0);
00103: 		PlayerStore instance = PlayerStore.Instance;
00104: 		try
00105: 		{
00106: 			if (instance != null)
00107: 			{
00108: 				PerSaveState.SetInt(instance, HideKeyFor(uid), num);
00109: 			}
00110: 		}
00111: 		catch
00112: 		{
00113: 		}
00114: 		foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00115: 		{
00116: 			Entry value = entry.Value;
00117: 			if (value != null && value.Uid == uid)
00118: 			{
00119: 				ApplyVisibility(entry.Value);
00120: 			}
00121: 		}
00122: 		Core.Debug("收纳箱 BoxLabel[uid=" + uid + "]：名字显隐切换 → " + ((num == 1) ? "隐藏" : "显示"));
00123: 	}
00124: 
00125: 	internal static void OnDragStart(IEnumerable<GameItem?>? items)
00126: 	{
00127: 		try
00128: 		{
00129: 			DragUids.Clear();
00130: 			if (items != null)
00131: 			{
00132: 				foreach (GameItem item in items)
00133: 				{
00134: 					if (item == null)
00135: 					{
00136: 						continue;
00137: 					}
00138: 					try
00139: 					{
00140: 						if (BoxItem.IsBox(item))
00141: 						{
00142: 							int uniqueId;
00143: 							try
00144: 							{
00145: 								uniqueId = item.uniqueId;
00146: 							}
00147: 							catch
00148: 							{
00149: 								goto end_IL_0021;
00150: 							}
00151: 							if (uniqueId != 0)
00152: 							{
00153: 								DragUids.Add(uniqueId);
00154: 							}
00155: 						}
00156: 						end_IL_0021:;
00157: 					}
00158: 					catch
00159: 					{
00160: 					}
00161: 				}
00162: 			}
00163: 			if (DragUids.Count <= 0)
00164: 			{
00165: 				return;
00166: 			}
00167: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00168: 			{
00169: 				ApplyVisibility(entry.Value);
00170: 			}
00171: 		}
00172: 		catch
00173: 		{
00174: 		}
00175: 	}
00176: 
00177: 	internal static void OnDragEnd()
00178: 	{
00179: 		try
00180: 		{
00181: 			DragUids.Clear();
00182: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00183: 			{
00184: 				ApplyVisibility(entry.Value);
00185: 				RealignEntry(entry.Value);
00186: 			}
00187: 		}
00188: 		catch
00189: 		{
00190: 		}
00191: 	}
~~~

~~~csharp
00223: 				LogOnce(uniqueId, "无挂点：根/图标节点均不可得");
00224: 				return;
00225: 			}
00226: 			if (value == null)
00227: 			{
00228: 				value = new Entry();
00229: 				Entries[el] = value;
00230: 			}
00231: 			value.LastApplyAt = Time.time;
00232: 			value.Uid = uniqueId;
00233: 			value.El = el;
00234: 			value.RootRt = val;
00235: 			value.IconRt = val2;
00236: 			GameItem val3 = ((uniqueId != 0) ? FindBox(uniqueId, "apply") : null);
00237: 			value.PendingItem = uniqueId != 0 && val3 == null;
00238: 			value.BaseName = BaseNameOf(val3);
00239: 			EnsureLabel(val, value);
00240: 			RealignEntry(value);
00241: 			ApplyVisibility(value);
00242: 		}
00243: 		catch (Exception ex2)
00244: 		{
00245: 			LogOnce(-2, "Apply 异常：" + ex2.GetType().Name + " " + ex2.Message);
00246: 		}
00247: 	}
00248: 
00249: 	internal static void Strip(GameItemElement el)
00250: 	{
00251: 		try
00252: 		{
00253: 			if (el != null && Entries.Count != 0 && Entries.TryGetValue(el, out Entry value) && value != null)
00254: 			{
00255: 				DestroyLabel(value.Label);
00256: 				value.Label = null;
00257: 				Entries.Remove(el);
00258: 				Core.Debug("收纳箱 BoxLabel：元素复用给非箱物品，已摘字（uid=" + value.Uid + "）。");
00259: 			}
00260: 		}
00261: 		catch
00262: 		{
00263: 		}
00264: 	}
00265: 
00266: 	internal static void Sweep()
00267: 	{
00268: 		try
00269: 		{
00270: 			if (Entries.Count > 0)
00271: 			{
00272: 				List<GameItemElement> list = null;
00273: 				foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00274: 				{
00275: 					Entry value = entry.Value;
00276: 					if ((Object)(object)value.Label == (Object)null || !Alive((Object?)(object)value.Label) || (Object)(object)value.RootRt == (Object)null || !Alive((Object?)(object)value.RootRt) || (Object)(object)value.IconRt == (Object)null || !Alive((Object?)(object)value.IconRt))
00277: 					{
00278: 						(list ?? (list = new List<GameItemElement>())).Add(entry.Key);
00279: 					}
00280: 				}
00281: 				if (list != null)
00282: 				{
00283: 					foreach (GameItemElement item2 in list)
00284: 					{
00285: 						Entries.Remove(item2);
00286: 					}
00287: 				}
00288: 			}
00289: 			if (ItemCache.Count > 0)
00290: 			{
00291: 				List<int> list2 = null;
00292: 				foreach (KeyValuePair<int, (GameItem, float)> item3 in ItemCache)
00293: 				{
00294: 					GameItem item = item3.Value.Item1;
00295: 					if (item == null || !Alive((object?)item))
00296: 					{
00297: 						(list2 ?? (list2 = new List<int>())).Add(item3.Key);
00298: 					}
00299: 				}
00300: 				if (list2 != null)
00301: 				{
00302: 					foreach (int item4 in list2)
00303: 					{
00304: 						ItemCache.Remove(item4);
00305: 					}
00306: 				}
00307: 			}
00308: 			if (LoggedOnce.Count > 512)
00309: 			{
00310: 				LoggedOnce.Clear();
00311: 			}
00312: 		}
00313: 		catch
00314: 		{
00315: 		}
00316: 	}
00317: 
00318: 	internal static void Tick()
00319: 	{
00320: 		try
00321: 		{
00322: 			if (Time.time >= _nextAttachAt)
00323: 			{
00324: 				_nextAttachAt = Time.time + 10f;
00325: 				SweepAttach();
00326: 			}
00327: 			if (Entries.Count == 0 || Time.time < _nextTickAt)
~~~

~~~csharp
00341: 			if (flag)
00342: 			{
00343: 				_nextFindAt = 0f;
00344: 				foreach (KeyValuePair<GameItemElement, Entry> entry2 in Entries)
00345: 				{
00346: 					Entry value = entry2.Value;
00347: 					if (value.PendingItem)
00348: 					{
00349: 						GameItem val = FindBox(value.Uid, "tick");
00350: 						if (val != null)
00351: 						{
00352: 							value.PendingItem = false;
00353: 							value.BaseName = BaseNameOf(val);
00354: 						}
00355: 					}
00356: 				}
00357: 			}
00358: 			foreach (KeyValuePair<GameItemElement, Entry> entry3 in Entries)
00359: 			{
00360: 				Entry value2 = entry3.Value;
00361: 				if (value2.Uid == 0 && value2.El != null && Alive((object?)value2.El))
00362: 				{
00363: 					int uniqueId;
00364: 					try
00365: 					{
00366: 						uniqueId = ((GameItem)value2.El).uniqueId;
00367: 					}
00368: 					catch
00369: 					{
00370: 						continue;
00371: 					}
00372: 					if (uniqueId != 0)
00373: 					{
00374: 						value2.Uid = uniqueId;
00375: 						value2.PendingItem = true;
00376: 						Core.Debug("收纳箱 BoxLabel：uid 补绑 0→" + uniqueId + "（元素晚绑定）");
00377: 					}
00378: 				}
00379: 			}
00380: 			foreach (KeyValuePair<GameItemElement, Entry> entry4 in Entries)
00381: 			{
00382: 				Entry value3 = entry4.Value;
00383: 				if ((Object)(object)value3.Label == (Object)null)
00384: 				{
00385: 					continue;
00386: 				}
00387: 				if (!value3.PendingItem)
00388: 				{
00389: 					GameItem val2 = CachedBox(value3.Uid);
00390: 					if (val2 == null)
00391: 					{
00392: 						value3.PendingItem = true;
00393: 					}
00394: 					else
00395: 					{
00396: 						value3.BaseName = BaseNameOf(val2);
00397: 					}
00398: 				}
00399: 				RealignEntry(value3);
00400: 				ApplyVisibility(value3);
00401: 			}
00402: 		}
00403: 		catch
00404: 		{
00405: 		}
00406: 	}
00407: 
00408: 	internal static void OnItemRenamed(GameItem? item)
00409: 	{
00410: 		try
00411: 		{
00412: 			if (item == null || Entries.Count == 0)
00413: 			{
00414: 				if (item != null)
00415: 				{
00416: 					SweepAttach();
00417: 				}
00418: 				return;
00419: 			}
00420: 			int uniqueId;
00421: 			try
00422: 			{
00423: 				uniqueId = item.uniqueId;
00424: 			}
00425: 			catch
00426: 			{
00427: 				return;
00428: 			}
00429: 			bool flag = false;
00430: 			foreach (KeyValuePair<GameItemElement, Entry> entry in Entries)
00431: 			{
00432: 				Entry value = entry.Value;
00433: 				if (value.Uid == uniqueId)
00434: 				{
00435: 					flag = true;
00436: 					value.PendingItem = false;
00437: 					value.BaseName = BaseNameOf(item);
00438: 					RealignEntry(value);
00439: 				}
00440: 			}
00441: 			if (!flag)
00442: 			{
00443: 				ItemCache[uniqueId] = (item, Time.time);
00444: 				SweepAttach();
00445: 			}
~~~

~~~csharp
00799: 		}
00800: 	}
00801: 
00802: 	internal static TMP_FontAsset? GetTmpFont()
00803: 	{
00804: 		if ((Object)(object)_tmpFont != (Object)null)
00805: 		{
00806: 			try
00807: 			{
00808: 				if ((Object)(object)_tmpFont != (Object)null)
00809: 				{
00810: 					return _tmpFont;
00811: 				}
00812: 			}
00813: 			catch
00814: 			{
00815: 			}
00816: 			_tmpFont = null;
00817: 		}
00818: 		try
00819: 		{
00820: 			TMP_FontAsset[] array = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
00821: 			if (array != null)
00822: 			{
00823: 				TMP_FontAsset[] array2 = array;
00824: 				foreach (TMP_FontAsset val in array2)
00825: 				{
00826: 					try
00827: 					{
00828: 						if (!((Object)(object)val == (Object)null))
00829: 						{
00830: 							string name = ((Object)val).name;
00831: 							if (!string.IsNullOrEmpty(name) && (name.IndexOf("SC", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("CN", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Chinese", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Han", StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("NotoSansSC", StringComparison.OrdinalIgnoreCase) >= 0))
00832: 							{
00833: 								_tmpFont = val;
00834: 								Core.Debug("收纳箱 BoxLabel：TMP 字体命中（CJK 扫描）：" + name);
00835: 								return _tmpFont;
00836: 							}
00837: 						}
00838: 					}
00839: 					catch
00840: 					{
00841: 					}
00842: 				}
00843: 			}
00844: 		}
00845: 		catch
00846: 		{
00847: 		}
00848: 		try
00849: 		{
00850: 			TextMeshProUGUI[] array3 = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
00851: 			if (array3 != null)
00852: 			{
00853: 				TextMeshProUGUI[] array4 = array3;
00854: 				foreach (TextMeshProUGUI val2 in array4)
00855: 				{
00856: 					try
00857: 					{
00858: 						if ((Object)(object)val2 == (Object)null || (Object)(object)((TMP_Text)val2).font == (Object)null)
00859: 						{
00860: 							continue;
00861: 						}
00862: 						_tmpFont = ((TMP_Text)val2).font;
00863: 						Core.Debug("收纳箱 BoxLabel：TMP 字体兜底（在用组件）：" + ((Object)((TMP_Text)val2).font).name);
00864: 						return _tmpFont;
00865: 					}
00866: 					catch
00867: 					{
00868: 					}
00869: 				}
00870: 			}
00871: 		}
00872: 		catch
00873: 		{
00874: 		}
00875: 		try
00876: 		{
00877: 			TMP_FontAsset[] array5 = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
00878: 			if (array5 != null && array5.Length != 0)
00879: 			{
00880: 				_tmpFont = array5[0];
00881: 				Core.Debug("收纳箱 BoxLabel：TMP 字体最后兜底（第一个资产）");
00882: 				return _tmpFont;
00883: 			}
00884: 		}
00885: 		catch
00886: 		{
00887: 		}
00888: 		return null;
00889: 	}
00890: 
00891: 	private static void RealignEntry(Entry e)
00892: 	{
00893: 		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
00894: 		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
00895: 		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
00896: 		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
00897: 		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
00898: 		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
00899: 		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
00900: 		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
00901: 		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
00902: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
00903: 		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
00904: 		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
00905: 		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
00906: 		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
00907: 		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
00908: 		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
00909: 		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
00910: 		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
00911: 		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
00912: 		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
00913: 		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
00914: 		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
00915: 		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
00916: 		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
00917: 		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
00918: 		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
00919: 		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
00920: 		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
00921: 		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
00922: 		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
00923: 		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
00924: 		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
00925: 		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
00926: 		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
00927: 		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
00928: 		try
00929: 		{
00930: 			TMP_Text label = e.Label;
00931: 			RectTransform iconRt = e.IconRt;
00932: 			RectTransform rootRt = e.RootRt;
00933: 			if ((Object)(object)label == (Object)null || (Object)(object)iconRt == (Object)null || (Object)(object)rootRt == (Object)null || !Alive((Object?)(object)label) || !Alive((Object?)(object)iconRt) || !Alive((Object?)(object)rootRt))
00934: 			{
00935: 				return;
00936: 			}
00937: 			try
00938: 			{
00939: 				iconRt.GetWorldCorners(Corners);
00940: 			}
00941: 			catch
00942: 			{
00943: 				return;
00944: 			}
00945: 			Vector3 val = Corners[0];
00946: 			Vector3 val2 = Corners[2];
00947: 			float num = Mathf.Abs(val2.x - val.x);
00948: 			float num2 = Mathf.Abs(val2.y - val.y);
00949: 			if (num < 1f || num2 < 1f)
00950: 			{
~~~

~~~csharp
00993: 				}
00994: 				if (Mathf.Abs(Quaternion.Dot(((Transform)rectTransform).localRotation, val6)) < 0.999f)
00995: 				{
00996: 					((Transform)rectTransform).localRotation = val6;
00997: 				}
00998: 			}
00999: 		}
01000: 		catch
01001: 		{
01002: 		}
01003: 	}
01004: 
01005: 	private static void ApplyVisibility(Entry? e)
01006: 	{
01007: 		if ((Object)(object)e?.Label == (Object)null)
01008: 		{
01009: 			return;
01010: 		}
01011: 		try
01012: 		{
01013: 			bool flag = e.Uid != 0 && IsShownFor(e.Uid) && !DragUids.Contains(e.Uid);
01014: 			if (((Component)e.Label).gameObject.activeSelf != flag)
01015: 			{
01016: 				((Component)e.Label).gameObject.SetActive(flag);
01017: 			}
01018: 		}
01019: 		catch
01020: 		{
01021: 		}
01022: 	}
01023: 
01024: 	internal static string BaseNameOf(GameItem? box)
01025: 	{
01026: 		if (box == null)
01027: 		{
01028: 			return "收纳箱";
01029: 		}
01030: 		try
01031: 		{
01032: 			if (GeneralHelper.HasCustomName(box))
01033: 			{
01034: 				string text = Core.Clean(GeneralHelper.GetCustomName(box));
01035: 				if (!string.IsNullOrEmpty(text))
01036: 				{
01037: 					string text2 = text.Trim();
01038: 					if (text2.Length > 0)
01039: 					{
01040: 						return text2;
01041: 					}
01042: 				}
01043: 			}
01044: 		}
01045: 		catch
01046: 		{
01047: 		}
01048: 		return "收纳箱";
01049: 	}
01050: 
01051: 	private static string FormatText(string? baseName, bool horizontal)
01052: 	{
01053: 		if (!NullGuard.HasText(baseName))
01054: 		{
01055: 			baseName = "收纳箱";
01056: 		}
01057: 		int num = Math.Min(baseName.Length, 3);
01058: 		if (num <= 0)
01059: 		{
01060: 			return "收纳箱";
01061: 		}
01062: 		if (horizontal)
01063: 		{
01064: 			return baseName.Substring(0, num);
01065: 		}
01066: 		StringBuilder stringBuilder = new StringBuilder();
01067: 		for (int i = 0; i < num; i++)
01068: 		{
01069: 			if (i > 0)
01070: 			{
01071: 				stringBuilder.Append('\n');
01072: 			}
01073: 			stringBuilder.Append(baseName[i]);
01074: 		}
01075: 		return stringBuilder.ToString();
01076: 	}
01077: 
01078: 	private static GameItem? FindBox(int uid, string source)
01079: 	{
01080: 		if (ItemCache.TryGetValue(uid, out (GameItem, float) value))
01081: 		{
01082: 			if (value.Item1 != null && Alive((object?)value.Item1) && SameUid(value.Item1, uid))
01083: 			{
01084: 				return value.Item1;
01085: 			}
01086: 			bool num = Time.time - value.Item2 >= 10f;
01087: 			ItemCache.Remove(uid);
01088: 			if (!num)
01089: 			{
01090: 				return null;
01091: 			}
01092: 		}
01093: 		if (Time.time < _nextFindAt)
01094: 		{
01095: 			return null;
01096: 		}
01097: 		_nextFindAt = Time.time + 2f;
01098: 		try
01099: 		{
01100: 			foreach (GameItem item in BoxItem.FindAllBoxes())
01101: 			{
01102: 				int uniqueId;
01103: 				try
01104: 				{
01105: 					uniqueId = item.uniqueId;
01106: 				}
01107: 				catch
01108: 				{
01109: 					continue;
01110: 				}
01111: 				ItemCache[uniqueId] = (item, Time.time);
01112: 			}
01113: 			Core.Debug("收纳箱 BoxLabel：uid 反查扫描完成（触发 uid=" + uid + "，来源=" + source + "，缓存 " + ItemCache.Count + " 条）。");
01114: 		}
01115: 		catch
01116: 		{
01117: 		}
01118: 		ItemCache.TryGetValue(uid, out (GameItem, float) value2);
01119: 		return value2.Item1;
01120: 	}
01121: 
01122: 	private static bool SameUid(GameItem item, int uid)
01123: 	{
01124: 		try
01125: 		{
01126: 			return item.uniqueId == uid;
01127: 		}
01128: 		catch
01129: 		{
01130: 			return false;
01131: 		}
01132: 	}
01133: 
01134: 	private static GameItem? CachedBox(int uid)
01135: 	{
01136: 		if (ItemCache.TryGetValue(uid, out (GameItem, float) value) && value.Item1 != null && Alive((object?)value.Item1) && SameUid(value.Item1, uid))
01137: 		{
01138: 			return value.Item1;
01139: 		}
01140: 		return null;
01141: 	}
01142: 
01143: 	private static void LogOnce(int uid, string msg)
01144: 	{
01145: 		if (LoggedOnce.Add(uid))
01146: 		{
01147: 			if (LoggedOnce.Count > 512)
01148: 			{
01149: 				LoggedOnce.Clear();
01150: 			}
01151: 			Core.Debug("收纳箱 BoxLabel[uid=" + uid + "]：" + msg);
01152: 		}
01153: 	}
01154: 
01155: 	private static bool Alive(Object? o)
01156: 	{
01157: 		return o != (Object)null;
01158: 	}
01159: 
01160: 	private static bool Alive(object? o)
01161: 	{
01162: 		return o != null;
01163: 	}
01164: 
01165: 	private static bool SameParent(TMP_Text txt, RectTransform parent)
01166: 	{
01167: 		try
01168: 		{
01169: 			Transform transform = txt.transform;
01170: 			Transform val = ((transform != null) ? transform.parent : null);
01171: 			return (Object)(object)val != (Object)null && (Object)(object)parent != (Object)null && (Object)(object)val == (Object)(object)parent;
01172: 		}
01173: 		catch
01174: 		{
01175: 			return false;
01176: 		}
01177: 	}
01178: 
01179: 	private static void DestroyLabel(TMP_Text? txt)
01180: 	{
01181: 		try
01182: 		{
01183: 			if (!((Object)(object)txt == (Object)null))
01184: 			{
01185: 				Object.Destroy((Object)(object)((Component)txt).gameObject);
01186: 			}
01187: 		}
01188: 		catch
01189: 		{
01190: 		}
01191: 	}
01192: }
~~~

### ItemStorage/GhostSweep.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using UnityEngine;
00004: 
00005: namespace ItemStorage;
00006: 
00007: internal static class GhostSweep
00008: {
00009: 	private static float _nextRunAt;
00010: 
00011: 	internal static void Run(string context)
00012: 	{
00013: 		if (Time.time < _nextRunAt)
00014: 		{
00015: 			return;
00016: 		}
00017: 		_nextRunAt = Time.time + 5f;
00018: 		try
00019: 		{
00020: 			EmporiumEntry instance = EmporiumEntry.Instance;
00021: 			if ((Object)(object)instance == (Object)null)
00022: 			{
00023: 				return;
00024: 			}
00025: 			int num = 0;
00026: 			num += Sweep(instance.invElement, "桌面", context);
00027: 			num += Sweep(instance.frontInvinvElement, "柜台正面", context);
00028: 			num += Sweep(instance.backInvinvElement, "柜台里侧", context);
00029: 			num += Sweep(instance.backInvinvElementCounter, "柜台里侧B", context);
00030: 			if (num > 0)
00031: 			{
00032: 				Core.Debug("收纳箱[" + context + "] 空壳清理：移除 " + num + " 个隐形占格残留（数量为 0 / 无 ID）。");
00033: 				try
00034: 				{
00035: 					instance.Validate(true);
00036: 				}
00037: 				catch
00038: 				{
00039: 				}
00040: 			}
00041: 			else
00042: 			{
00043: 				Core.Debug("收纳箱[" + context + "] 空壳清理：网格表面无残留。");
00044: 			}
00045: 			BoxLabel.Sweep();
00046: 			BoxLabel.SweepAttach();
00047: 		}
00048: 		catch (Exception ex)
00049: 		{
00050: 			Core.Log?.Warning("收纳箱[" + context + "] 空壳清理：" + ex.Message);
00051: 		}
00052: 	}
00053: 
00054: 	private static int Sweep(GameGridInventory? grid, string label, string context)
00055: 	{
00056: 		if (grid == null)
00057: 		{
00058: 			return 0;
00059: 		}
00060: 		int num = 0;
00061: 		try
00062: 		{
00063: 			foreach (GameItem childItem in ((GameInventory)grid).childItems)
00064: 			{
00065: 				if (childItem == null)
00066: 				{
00067: 					continue;
00068: 				}
00069: 				try
00070: 				{
00071: 					if (childItem == null)
00072: 					{
00073: 						continue;
00074: 					}
00075: 					int unitCount;
00076: 					try
00077: 					{
00078: 						unitCount = childItem.unitCount;
00079: 					}
00080: 					catch
00081: 					{
00082: 						goto end_IL_0027;
00083: 					}
00084: 					string text;
00085: 					try
00086: 					{
00087: 						text = Core.Clean(childItem.identifier);
00088: 					}
00089: 					catch
00090: 					{
00091: 						goto end_IL_0027;
00092: 					}
00093: 					if (unitCount > 0 && !string.IsNullOrEmpty(text))
00094: 					{
00095: 						continue;
00096: 					}
00097: 					List<GraphNodeStorage> list = null;
00098: 					try
00099: 					{
00100: 						list = childItem.parents;
00101: 					}
00102: 					catch
00103: 					{
00104: 						list = null;
00105: 					}
00106: 					if (list != null)
00107: 					{
00108: 						foreach (GraphNodeStorage item in list)
00109: 						{
00110: 							if (item != null)
00111: 							{
00112: 								try
00113: 								{
00114: 									childItem.RemoveParent(item);
00115: 								}
00116: 								catch
00117: 								{
00118: 								}
00119: 							}
00120: 						}
00121: 					}
00122: 					bool flag = false;
00123: 					try
00124: 					{
00125: 						flag = GraphUtils.DestroyIfUnplaced(childItem, "ItemStorage_Sweep");
00126: 					}
00127: 					catch
00128: 					{
00129: 						flag = false;
00130: 					}
00131: 					if (!flag)
00132: 					{
00133: 						try
00134: 						{
00135: 							childItem.Destroy();
00136: 						}
00137: 						catch
00138: 						{
00139: 							goto end_IL_0027;
00140: 						}
00141: 					}
00142: 					num++;
00143: 					Core.Debug("收纳箱[" + context + "] 空壳清理[" + label + "]：删除 '" + (string.IsNullOrEmpty(text) ? "(无ID)" : text) + "'（数量 " + unitCount + "）。");
00144: 					end_IL_0027:;
00145: 				}
00146: 				catch (Exception ex)
00147: 				{
00148: 					Core.Debug("收纳箱[" + context + "] 空壳清理[" + label + "] 单件失败：" + ex.Message);
00149: 				}
00150: 			}
00151: 		}
00152: 		catch (Exception ex2)
00153: 		{
00154: 			Core.Debug("收纳箱[" + context + "] 空壳扫描[" + label + "]：" + ex2.Message);
00155: 		}
00156: 		return num;
00157: 	}
00158: }
~~~

### ItemStorage/BoxTooltip.cs

~~~csharp
00070: 						{
00071: 							HusbandryHelper.GetSexeDisplay(flag2 ? "MALE" : "FEMALE"),
00072: 							StageDisplay(value.Item3, value2.growthStages),
00073: 							value2.GetDisplayName()
00074: 						});
00075: 						localizedMechanic = LocGenderHelper.Resolve(localizedMechanic, (LocGender)(flag2 ? 1 : 2));
00076: 						val.AddLine(localizedMechanic, true, (ColorPalette)35216, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00077: 						val.AddLine(LocHelper.GetLocalizedMechanic("husbandry_tooltip_age", new object[2]
00078: 						{
00079: 							value.Item3,
00080: 							Mathf.RoundToInt(value.Item4)
00081: 						}), true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00082: 					}
00083: 				}
00084: 				catch
00085: 				{
00086: 				}
00087: 				return val.Build(-1).richTextForeground;
00088: 			}
00089: 			if (StorageGrid.IsWaterSlot(pack, slot))
00090: 			{
00091: 				int storedWaterPurity = StorageGrid.GetStoredWaterPurity(pack, slot);
00092: 				string text3 = null;
00093: 				string text4 = ((float)storedWaterPurity / 100f).ToString("F2", CultureInfo.InvariantCulture) + "%";
00094: 				bool flag3 = HasWaterInstrument();
00095: 				try
00096: 				{
00097: 					text3 = (Core.Clean(WaterFeatureHelper.GetWaterDisplayName(storedWaterPurity)) ?? "").TrimEnd('\u3000', ' ');
00098: 					if (string.IsNullOrEmpty(text3))
00099: 					{
00100: 						text3 = null;
00101: 					}
00102: 				}
00103: 				catch (Exception ex)
00104: 				{
00105: 					Core.Debug("收纳箱水格档位名提取失败：" + ex.Message);
00106: 				}
00107: 				if (!string.IsNullOrEmpty(text3))
00108: 				{
00109: 					val.AddLine(text3 + (flag3 ? ("（" + text4 + "）") : ""), true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00110: 				}
00111: 				if (!NullGuard.HasText(types))
00112: 				{
00113: 					int storedWaterML = StorageGrid.GetStoredWaterML(pack, slot);
00114: 					if (storedWaterML > 0)
00115: 					{
00116: 						val.AddLine(FormatWaterML(storedWaterML), true, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00117: 					}
00118: 				}
00119: 				if (NullGuard.HasText(StorageGrid.GetLiquidSnapshot(pack, slot)) && HasWaterInstrument())
00120: 				{
00121: 					try
00122: 					{
00123: 						string liquidSnapshot = StorageGrid.GetLiquidSnapshot(pack, slot);
00124: 						long num2 = 0L;
00125: 						try
00126: 						{
00127: 							num2 = (long)StorageGrid.GetStoredWaterML(pack, slot) * 1000L;
00128: 						}
00129: 						catch
00130: 						{
00131: 						}
00132: 						string[] array = (liquidSnapshot ?? "").Split(';');
00133: 						foreach (string text5 in array)
00134: 						{
00135: 							int num3 = text5.IndexOf('=');
00136: 							if (num3 <= 0)
00137: 							{
00138: 								continue;
00139: 							}
00140: 							string text6 = text5.Substring(0, num3);
00141: 							if (text6.StartsWith("CURRENT_PART_", StringComparison.Ordinal))
00142: 							{
00143: 								string text7 = text6.Substring("CURRENT_PART_".Length).ToLowerInvariant();
00144: 								string text8;
00145: 								try
00146: 								{
00147: 									text8 = Liquid.GetLiquidFromId(text7)?.displayName ?? text7;
00148: 								}
00149: 								catch
00150: 								{
00151: 									text8 = text7;
00152: 								}
00153: 								int num4 = 0;
00154: 								try
00155: 								{
00156: 									num4 = int.Parse(text5.Substring(num3 + 1), CultureInfo.InvariantCulture);
00157: 								}
00158: 								catch
00159: 								{
00160: 								}
00161: 								string text9;
00162: 								try
00163: 								{
00164: 									text9 = text8 + ": " + WaterScannerUIManager.FormatAmount(num4);
00165: 								}
00166: 								catch
00167: 								{
00168: 									text9 = text8 + ": " + ((double)num4 / 1000.0).ToString("0.##") + " mL";
00169: 								}
00170: 								if (num2 > 0 && num4 > 0)
00171: 								{
00172: 									text9 += string.Format(CultureInfo.InvariantCulture, " ({0:F2}%)", (double)num4 * 100.0 / (double)num2);
00173: 								}
00174: 								val.AddLine("<size=65%>" + text9 + "</size>", true, (ColorPalette)8950922, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
~~~

### ItemStorage/StoragePatch.cs

~~~csharp
00020: 	}
00021: 
00022: 	private struct PendingWithdraw
00023: 	{
00024: 		public GameItem Box;
00025: 
00026: 		public int Slot;
00027: 
00028: 		public GameGridInventory? Drop;
00029: 	}
00030: 
00031: 	[HarmonyPatch]
00032: 	internal static class StorageEndDragPatch
00033: 	{
00034: 		private static MethodBase TargetMethod()
00035: 		{
00036: 			MethodInfo methodInfo = AccessTools.Method(typeof(ItemMouseDragHandler), "EndDrag", (Type[])null, (Type[])null);
00037: 			if (methodInfo == null)
00038: 			{
00039: 				throw new InvalidOperationException("ItemMouseDragHandler.EndDrag 未找到");
00040: 			}
00041: 			return methodInfo;
00042: 		}
00043: 
00044: 		private static void Prefix(ItemMouseDragHandler __instance)
00045: 		{
00046: 			try
00047: 			{
00048: 				GameItem draggedItem = GetDraggedItem(__instance);
00049: 				GameItem lastItem = GetLastItem(__instance);
00050: 				Core.Debug("拖拽诊断：EndDrag Prefix 触发， currentItem='" + SafeId(draggedItem) + "' lastItem='" + SafeId(lastItem) + "'");
00051: 				if (draggedItem != null && lastItem != null && BoxItem.IsBox(lastItem) && !BoxItem.IsBox(draggedItem))
00052: 				{
00053: 					if (IsLabeler(draggedItem))
00054: 					{
00055: 						Core.Debug("收纳箱 EndDrag Prefix：'" + SafeId(draggedItem) + "' 是标签机，让路改名流程不存入");
00056: 						return;
00057: 					}
00058: 					_pendingDeposits.Enqueue(new PendingDeposit
00059: 					{
00060: 						Box = lastItem,
00061: 						Item = draggedItem,
00062: 						Slot = -1
00063: 					});
00064: 					Core.Debug("收纳箱 EndDrag Prefix：登记待存入 '" + SafeId(draggedItem) + "' → 盒（队列 " + _pendingDeposits.Count + "）");
00065: 				}
00066: 			}
00067: 			catch (Exception ex)
00068: 			{
00069: 				Core.Log?.Warning("ItemStorage PrefixEndDragNotePending：" + ex.Message);
00070: 			}
00071: 		}
00072: 	}
00073: 
00074: 	[HarmonyPatch]
00075: 	internal static class ItemSpriteCapturePatch
00076: 	{
00077: 		private static MethodBase TargetMethod()
00078: 		{
00079: 			MethodInfo methodInfo = AccessTools.Method(typeof(GameItemElement), "ResolveSpriteByName", (Type[])null, (Type[])null);
00080: 			if (methodInfo == null)
00081: 			{
00082: 				throw new InvalidOperationException("GameItemElement.ResolveSpriteByName 未找到");
00083: 			}
00084: 			return methodInfo;
00085: 		}
00086: 
00087: 		private static void Postfix(GameItemElement __instance, string name, ref Sprite __result)
00088: 		{
00089: 			try
00090: 			{
00091: 				if (__instance != null)
00092: 				{
00093: 					if (SpriteAssets.IsRegisteredClean((name != null && name.IndexOf('\0') < 0) ? name.Trim() : Core.Clean(name)))
00094: 					{
00095: 						BoxLabel.Apply(__instance);
00096: 					}
00097: 					else
00098: 					{
00099: 						BoxLabel.Strip(__instance);
00100: 					}
00101: 				}
00102: 				string text = ((name != null && name.IndexOf('\0') < 0) ? name.Trim() : Core.Clean(name));
00103: 				if (!string.IsNullOrEmpty(text) && (Object)(object)__result != (Object)null && !IsUnknownSprite(__result))
00104: 				{
00105: 					ResolvedSpriteByName[text] = __result;
00106: 				}
00107: 				if (__instance == null || (Object)(object)__result == (Object)null)
00108: 				{
00109: 					return;
00110: 				}
00111: 				int uniqueId;
00112: 				try
00113: 				{
00114: 					uniqueId = ((GameItem)__instance).uniqueId;
00115: 				}
00116: 				catch
00117: 				{
00118: 					return;
00119: 				}
00120: 				if (uniqueId == 0 || !WithdrawSpriteOverride.TryGetValue(uniqueId, out (string, string, float) value))
00121: 				{
00122: 					return;
00123: 				}
00124: 				string text2 = Core.Clean(value.Item2);
00125: 				if (string.IsNullOrEmpty(text2) || string.Equals(text2, text, StringComparison.Ordinal))
00126: 				{
00127: 					return;
00128: 				}
00129: 				Sprite val = null;
00130: 				if (ResolvedSpriteByName.TryGetValue(text2, out Sprite value2) && IsAlive(value2))
00131: 				{
00132: 					val = value2;
00133: 				}
~~~

~~~csharp
00195: 				{
00196: 					uniqueId = ((GameItem)__instance).uniqueId;
00197: 				}
00198: 				catch
00199: 				{
00200: 					return;
00201: 				}
00202: 				if (uniqueId == 0 || !WithdrawSpriteOverride.TryGetValue(uniqueId, out (string, string, float) value))
00203: 				{
00204: 					return;
00205: 				}
00206: 				string text = Core.Clean(value.Item2);
00207: 				if (string.IsNullOrEmpty(text))
00208: 				{
00209: 					return;
00210: 				}
00211: 				Sprite val = null;
00212: 				if (ResolvedSpriteByName.TryGetValue(text, out Sprite value2) && IsAlive(value2))
00213: 				{
00214: 					val = value2;
00215: 				}
00216: 				if ((Object)(object)val == (Object)null && !string.IsNullOrEmpty(value.Item1))
00217: 				{
00218: 					try
00219: 					{
00220: 						val = RenderHandler.TryLoadFromAtlas(value.Item1, text);
00221: 					}
00222: 					catch
00223: 					{
00224: 					}
00225: 				}
00226: 				if (IsAlive(val))
00227: 				{
00228: 					ResolvedSpriteByName[text] = val;
00229: 					__result = val;
00230: 					Core.Debug("收纳箱 贴图纠偏生效：uid=" + uniqueId + " → '" + text + "'");
00231: 				}
00232: 			}
00233: 			catch
00234: 			{
00235: 			}
00236: 		}
00237: 
00238: 		private static bool IsAlive(Sprite? s)
00239: 		{
00240: 			if ((Object)(object)s == (Object)null)
00241: 			{
00242: 				return false;
00243: 			}
00244: 			try
00245: 			{
00246: 				return (Object)(object)s != (Object)null;
00247: 			}
00248: 			catch
00249: 			{
00250: 				return false;
00251: 			}
00252: 		}
00253: 	}
00254: 
00255: 	private static readonly Queue<PendingDeposit> _pendingDeposits = new Queue<PendingDeposit>();
00256: 
00257: 	private static int _batchSeq;
00258: 
00259: 	private static readonly Queue<PendingWithdraw> _pendingWithdraws = new Queue<PendingWithdraw>();
00260: 
00261: 	private static bool _rightWasDown;
00262: 
00263: 	private static int _wOk;
00264: 
00265: 	private static ItemMouseDragHandler? _dragHandler;
00266: 
00267: 	private static bool _wasDragging;
00268: 
00269: 	private static GameItem? _dragItem;
00270: 
00271: 	private static float _nextDragFindAt;
00272: 
00273: 	private static float _nextMultiFindAt;
00274: 
00275: 	private static ItemMultiSelectHandler? _multiHandler;
00276: 
00277: 	private static bool _groupWasActive;
00278: 
00279: 	private static List<GameItem>? _groupSnapshot;
00280: 
00281: 	private static GameItem? _groupHoverItem;
00282: 
00283: 	private static FieldInfo? _lastItemField;
00284: 
00285: 	private static FieldInfo? _currentRemoveField;
00286: 
00287: 	private static FieldInfo? _maxRemoveField;
00288: 
00289: 	private static FieldInfo? _selectedItemsField;
00290: 
00291: 	private static FieldInfo? _hoverItemField;
00292: 
00293: 	internal static readonly Dictionary<string, Sprite> ResolvedSpriteByName = new Dictionary<string, Sprite>(StringComparer.Ordinal);
00294: 
00295: 	internal static readonly Dictionary<int, (string Atlas, string Spr, float RegTime)> WithdrawSpriteOverride = new Dictionary<int, (string, string, float)>();
00296: 
00297: 	internal static void TickDeferred()
00298: 	{
00299: 		TrackDragSnapshot();
00300: 		TrackGroupDrag();
00301: 		try
00302: 		{
00303: 			List<GameItem> list = null;
00304: 			Dictionary<int, int> dictionary = new Dictionary<int, int>();
00305: 			Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
00306: 			try
00307: 			{
00308: 				while (_pendingDeposits.Count > 0)
00309: 				{
00310: 					PendingDeposit pendingDeposit = _pendingDeposits.Dequeue();
00311: 					if (pendingDeposit.Box == null || pendingDeposit.Item == null)
00312: 					{
00313: 						continue;
00314: 					}
00315: 					bool notify = pendingDeposit.Batch == 0;
00316: 					try
00317: 					{
00318: 						if (BoxTransfer.TryDeposit(pendingDeposit.Box, pendingDeposit.Item, 1, pendingDeposit.Slot, consume: true, refresh: false, notify))
00319: 						{
00320: 							Core.Debug("收纳箱 deferred：存入成功（槽 " + pendingDeposit.Slot + "，'" + SafeId(pendingDeposit.Item) + "'）");
00321: 							if (list == null)
00322: 							{
00323: 								list = new List<GameItem>();
00324: 							}
00325: 							bool flag = false;
00326: 							foreach (GameItem item in list)
00327: 							{
00328: 								if (item == pendingDeposit.Box)
00329: 								{
00330: 									flag = true;
00331: 									break;
00332: 								}
00333: 							}
00334: 							if (!flag)
00335: 							{
00336: 								list.Add(pendingDeposit.Box);
00337: 							}
00338: 							if (pendingDeposit.Batch != 0)
00339: 							{
00340: 								dictionary[pendingDeposit.Batch] = (dictionary.TryGetValue(pendingDeposit.Batch, out var value) ? value : 0) + 1;
00341: 							}
00342: 						}
00343: 						else
00344: 						{
00345: 							Core.Debug("收纳箱 deferred：存入未执行（'" + SafeId(pendingDeposit.Item) + "' 走原生退回，物品应在鼠标原 hover 处）");
00346: 							if (pendingDeposit.Batch != 0)
00347: 							{
00348: 								dictionary2[pendingDeposit.Batch] = (dictionary2.TryGetValue(pendingDeposit.Batch, out var value2) ? value2 : 0) + 1;
00349: 							}
00350: 						}
00351: 					}
00352: 					catch (Exception ex)
00353: 					{
00354: 						Core.Log?.Warning("收纳箱 deferred deposit：" + ex.Message);
00355: 						if (pendingDeposit.Batch != 0)
00356: 						{
00357: 							dictionary2[pendingDeposit.Batch] = (dictionary2.TryGetValue(pendingDeposit.Batch, out var value3) ? value3 : 0) + 1;
00358: 						}
00359: 					}
00360: 				}
00361: 				foreach (KeyValuePair<int, int> item2 in dictionary)
00362: 				{
00363: 					if (!dictionary2.TryGetValue(item2.Key, out var value4))
00364: 					{
00365: 						value4 = 0;
00366: 					}
00367: 					if (item2.Value + value4 >= 2)
00368: 					{
00369: 						if (value4 > 0)
00370: 						{
00371: 							BoxItem.NotifyUser("已存入 " + item2.Value + " 件，" + value4 + " 件未能存入");
00372: 						}
00373: 						else
00374: 						{
00375: 							BoxItem.NotifyUser("已存入 " + item2.Value + " 件");
00376: 						}
00377: 					}
00378: 				}
00379: 				foreach (KeyValuePair<int, int> item3 in dictionary2)
00380: 				{
00381: 					if (!dictionary.ContainsKey(item3.Key) && item3.Value >= 2)
00382: 					{
00383: 						BoxItem.NotifyUser("未能存入 " + item3.Value + " 件");
00384: 					}
00385: 				}
00386: 			}
00387: 			finally
00388: 			{
00389: 				if (list != null)
00390: 				{
00391: 					foreach (GameItem item4 in list)
00392: 					{
00393: 						try
00394: 						{
00395: 							BoxWindow.RefreshButtons(item4);
00396: 						}
00397: 						catch
00398: 						{
00399: 						}
00400: 					}
00401: 				}
00402: 			}
00403: 		}
00404: 		catch (Exception ex2)
00405: 		{
00406: 			Core.Log?.Warning("ItemStorage TickDeferred：" + ex2.Message);
00407: 		}
00408: 		try
00409: 		{
00410: 			if (_pendingWithdraws.Count > 0)
00411: 			{
00412: 				PendingWithdraw pendingWithdraw = _pendingWithdraws.Dequeue();
00413: 				BoxTransfer.WithdrawResult withdrawResult = BoxTransfer.WithdrawResult.SilentFail;
00414: 				try
00415: 				{
00416: 					withdrawResult = BoxTransfer.TryWithdraw(pendingWithdraw.Box, pendingWithdraw.Slot, pendingWithdraw.Drop);
00417: 				}
00418: 				catch (Exception ex3)
00419: 				{
00420: 					Core.Log?.Warning("收纳箱 right-click withdraw：" + ex3.Message);
00421: 				}
00422: 				if (withdrawResult == BoxTransfer.WithdrawResult.Ok)
00423: 				{
00424: 					_wOk++;
00425: 					if (_pendingWithdraws.Count == 0)
00426: 					{
00427: 						_wOk = 0;
00428: 					}
00429: 				}
00430: 				else
00431: 				{
00432: 					int count = _pendingWithdraws.Count;
00433: 					_pendingWithdraws.Clear();
00434: 					if (withdrawResult == BoxTransfer.WithdrawResult.NoSpace || withdrawResult == BoxTransfer.WithdrawResult.NotOnGrid)
00435: 					{
00436: 						BoxItem.NotifyUser(BoxTransfer.SpaceShort(pendingWithdraw.Drop != null) + "，未能完全取出物品");
00437: 					}
00438: 					_wOk = 0;
00439: 					Core.Debug("收纳箱 right-click：取出中断（" + withdrawResult.ToString() + "），余 " + count + " 条作废");
00440: 				}
00441: 			}
00442: 			TrackRightClick();
00443: 		}
00444: 		catch (Exception ex4)
00445: 		{
00446: 			Core.Log?.Warning("ItemStorage 右键取出轮询：" + ex4.Message);
00447: 			_rightWasDown = false;
00448: 			_pendingWithdraws.Clear();
00449: 		}
00450: 	}
00451: 
00452: 	private static void TrackRightClick()
00453: 	{
00454: 		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
00455: 		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
00456: 		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
00457: 		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
00458: 		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
00459: 		bool flag = false;
00460: 		try
00461: 		{
00462: 			flag = Input.GetMouseButtonDown(1);
00463: 		}
00464: 		catch
00465: 		{
00466: 			return;
00467: 		}
00468: 		bool rightWasDown = _rightWasDown;
00469: 		_rightWasDown = flag;
00470: 		if (!flag || rightWasDown)
00471: 		{
00472: 			return;
00473: 		}
00474: 		try
00475: 		{
00476: 			Vector3 mousePosition = Input.mousePosition;
00477: 			if (!BoxWindow.TryHitSlot(new Vector2(mousePosition.x, mousePosition.y), out GameItem box, out int slot) || box == null)
00478: 			{
00479: 				return;
00480: 			}
00481: 			BoxItem.ClickSound();
00482: 			GameGridInventory val = (BoxTransfer.AltHeld() ? BoxTransfer.ResolveCounterDrop() : null);
00483: 			BoxTransfer.WithdrawResult withdrawResult = BoxTransfer.WithdrawResult.SilentFail;
00484: 			try
00485: 			{
00486: 				withdrawResult = BoxTransfer.TryWithdraw(box, slot, val);
00487: 			}
00488: 			catch (Exception ex)
00489: 			{
00490: 				Core.Log?.Warning("收纳箱 right-click 首件：" + ex.Message);
00491: 			}
00492: 			if (_pendingWithdraws.Count == 0)
00493: 			{
00494: 				_wOk = 0;
00495: 			}
00496: 			switch (withdrawResult)
00497: 			{
00498: 			case BoxTransfer.WithdrawResult.NoSpace:
00499: 			case BoxTransfer.WithdrawResult.NotOnGrid:
00500: 				BoxItem.NotifyUser(BoxTransfer.SpaceShort(val != null) + "，无法取出物品");
00501: 				break;
00502: 			case BoxTransfer.WithdrawResult.Ok:
00503: 			{
00504: 				_wOk++;
00505: 				for (int i = 1; i < 5; i++)
00506: 				{
00507: 					_pendingWithdraws.Enqueue(new PendingWithdraw
00508: 					{
00509: 						Box = box,
00510: 						Slot = slot,
00511: 						Drop = val
00512: 					});
00513: 				}
00514: 				Core.Debug("收纳箱 right-click：首件已取，槽 " + slot + " 余 4 件排队（本批已成功 " + _wOk + "）");
00515: 				break;
00516: 			}
00517: 			}
00518: 		}
00519: 		catch (Exception ex2)
00520: 		{
00521: 			Core.Log?.Warning("收纳箱 right-click 判定：" + ex2.Message);
00522: 		}
00523: 	}
00524: 
00525: 	internal static bool? TryRegisterClickDeposit(GameItem pack, int slot)
00526: 	{
00527: 		try
00528: 		{
00529: 			EnsureHandler();
00530: 			if ((Object)(object)_dragHandler == (Object)null)
00531: 			{
00532: 				return false;
00533: 			}
00534: 			if (_dragHandler.IsDraggingItem)
00535: 			{
00536: 				GameItem draggedItem = GetDraggedItem(_dragHandler);
00537: 				if (draggedItem == null)
00538: 				{
00539: 					Core.Debug("收纳箱 click deposit：拖拽中但取不到物品引用，忽略本次点击");
00540: 					return null;
00541: 				}
00542: 				if (BoxItem.IsBox(draggedItem))
00543: 				{
00544: 					Core.Debug("收纳箱 click deposit：手持容器，忽略本次点击（容器不进容器）");
00545: 					return null;
00546: 				}
00547: 				_pendingDeposits.Enqueue(new PendingDeposit
00548: 				{
00549: 					Box = pack,
00550: 					Item = draggedItem,
00551: 					Slot = slot
00552: 				});
00553: 				Core.Debug("收纳箱 click deposit：登记 '" + SafeId(draggedItem) + "' → 槽 " + slot + "（队列 " + _pendingDeposits.Count + "）");
00554: 				return true;
00555: 			}
00556: 			if (_wasDragging)
00557: 			{
00558: 				Core.Debug("收纳箱 click deposit：松手后立即点击，跳过防误取");
00559: 				return null;
00560: 			}
00561: 			return false;
00562: 		}
00563: 		catch (Exception ex)
00564: 		{
00565: 			Core.Log?.Warning("ItemStorage click deposit：" + ex.Message);
00566: 			return null;
00567: 		}
00568: 	}
00569: 
00570: 	private static void TrackDragSnapshot()
00571: 	{
00572: 		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
00573: 		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
00574: 		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
00575: 		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
00576: 		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
00577: 		try
00578: 		{
00579: 			EnsureHandler();
00580: 			if ((Object)(object)_dragHandler == (Object)null)
00581: 			{
00582: 				_wasDragging = false;
00583: 				return;
00584: 			}
00585: 			bool isDraggingItem = _dragHandler.IsDraggingItem;
00586: 			if (isDraggingItem && !_wasDragging)
00587: 			{
00588: 				_dragItem = GetDraggedItem(_dragHandler);
00589: 				BoxLabel.OnDragStart((IEnumerable<GameItem?>?)(object)new GameItem[1] { _dragItem });
00590: 				int num = -1;
00591: 				try
00592: 				{
00593: 					GameItem? dragItem = _dragItem;
00594: 					num = ((dragItem != null) ? dragItem.unitCount : (-1));
00595: 				}
00596: 				catch
00597: 				{
00598: 				}
00599: 				int? removeCount = GetRemoveCount(_dragHandler, "currentRemove");
00600: 				int? removeCount2 = GetRemoveCount(_dragHandler, "maxRemove");
00601: 				Core.Debug("拖拽诊断：拖起 '" + SafeId(_dragItem) + "' unitCount=" + num + " currentRemove=" + (removeCount?.ToString() ?? "?") + " maxRemove=" + (removeCount2?.ToString() ?? "?"));
00602: 			}
00603: 			else if (!isDraggingItem && _wasDragging)
00604: 			{
00605: 				GameItem val = GetDraggedItem(_dragHandler) ?? _dragItem;
00606: 				GameItem lastItem = GetLastItem(_dragHandler);
00607: 				Core.Debug("拖拽诊断：松手 currentItem='" + SafeId(val) + "' lastItem='" + SafeId(lastItem) + "'");
00608: 				try
00609: 				{
00610: 					if (val != null && !BoxItem.IsBox(val))
00611: 					{
00612: 						Vector3 mousePosition = Input.mousePosition;
00613: 						if (BoxWindow.TryHitSlot(new Vector2(mousePosition.x, mousePosition.y), out GameItem box, out int slot, verbose: true) && box != null)
00614: 						{
00615: 							_pendingDeposits.Enqueue(new PendingDeposit
00616: 							{
00617: 								Box = box,
00618: 								Item = val,
00619: 								Slot = slot
00620: 							});
00621: 							Core.Debug("收纳箱 drop：登记 '" + SafeId(val) + "' → 槽 " + slot + "（队列 " + _pendingDeposits.Count + "）");
00622: 						}
00623: 						else
00624: 						{
00625: 							Core.Debug("收纳箱 drop：未命中格子（'" + SafeId(val) + "' 走官方 EndDrag，lastItem 路径）");
00626: 						}
00627: 					}
00628: 				}
00629: 				catch (Exception ex)
00630: 				{
00631: 					Core.Log?.Warning("收纳箱 drop 判定：" + ex.Message);
00632: 				}
00633: 				BoxLabel.OnDragEnd();
00634: 				_dragItem = null;
00635: 			}
00636: 			_wasDragging = isDraggingItem;
00637: 		}
00638: 		catch (Exception ex2)
00639: 		{
00640: 			Core.Log?.Warning("ItemStorage 拖拽轮询：" + ex2.Message);
00641: 			_wasDragging = false;
00642: 			_dragItem = null;
00643: 		}
00644: 	}
00645: 
00646: 	private static void TrackGroupDrag()
00647: 	{
00648: 		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
00649: 		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
00650: 		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
00651: 		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
00652: 		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
00653: 		try
00654: 		{
00655: 			EnsureMultiHandler();
00656: 			if ((Object)(object)_multiHandler == (Object)null)
00657: 			{
00658: 				_groupWasActive = false;
00659: 				_groupSnapshot = null;
00660: 				return;
00661: 			}
00662: 			int num = 0;
00663: 			try
00664: 			{
00665: 				num = GetSelectedItems()?.Count ?? 0;
00666: 			}
00667: 			catch
00668: 			{
00669: 				num = 0;
00670: 			}
00671: 			if (num > 0 && !_groupWasActive)
00672: 			{
00673: 				_groupSnapshot = new List<GameItem>();
00674: 				try
00675: 				{
00676: 					List<GameItem> selectedItems = GetSelectedItems();
00677: 					if (selectedItems != null)
00678: 					{
00679: 						foreach (GameItem item in selectedItems)
00680: 						{
00681: 							try
00682: 							{
00683: 								if (item != null)
00684: 								{
00685: 									_groupSnapshot.Add(item);
00686: 								}
00687: 							}
00688: 							catch
00689: 							{
00690: 							}
00691: 						}
00692: 					}
00693: 				}
00694: 				catch
00695: 				{
00696: 				}
00697: 				_groupWasActive = true;
00698: 				_groupHoverItem = null;
00699: 				BoxLabel.OnDragStart(_groupSnapshot);
00700: 				Core.Debug("拖拽诊断：框选组成立，集合 " + num + " 件（快照 " + _groupSnapshot.Count + "）");
~~~

~~~csharp
00747: 			}
00748: 			_groupWasActive = false;
00749: 			BoxLabel.OnDragEnd();
00750: 			List<GameItem> groupSnapshot = _groupSnapshot;
00751: 			_groupSnapshot = null;
00752: 			if (groupSnapshot == null || groupSnapshot.Count == 0)
00753: 			{
00754: 				return;
00755: 			}
00756: 			try
00757: 			{
00758: 				GameItem groupHoverItem = _groupHoverItem;
00759: 				if (groupHoverItem != null && BoxItem.IsBox(groupHoverItem))
00760: 				{
00761: 					List<GameItem> list2 = new List<GameItem>();
00762: 					foreach (GameItem item3 in groupSnapshot)
00763: 					{
00764: 						if (item3 != null && !BoxItem.IsBox(item3) && !IsLabeler(item3))
00765: 						{
00766: 							list2.Add(item3);
00767: 						}
00768: 					}
00769: 					if (list2.Count > 0)
00770: 					{
00771: 						int batch = ((list2.Count > 1) ? (++_batchSeq) : 0);
00772: 						foreach (GameItem item4 in list2)
00773: 						{
00774: 							_pendingDeposits.Enqueue(new PendingDeposit
00775: 							{
00776: 								Box = groupHoverItem,
00777: 								Item = item4,
00778: 								Slot = -1,
00779: 								Batch = batch
00780: 							});
00781: 						}
00782: 						Core.Debug("收纳箱 group drop：登记 " + list2.Count + " 件 → 盒（批次 " + batch + "，队列 " + _pendingDeposits.Count + "）");
00783: 					}
00784: 					_groupHoverItem = null;
00785: 					return;
00786: 				}
00787: 				Core.Debug("拖拽诊断：组下降沿 hover 缓存='" + SafeId(groupHoverItem) + "'，走鼠标格子判定");
00788: 			}
00789: 			catch
00790: 			{
00791: 			}
00792: 			try
00793: 			{
00794: 				Vector3 mousePosition = Input.mousePosition;
00795: 				if (BoxWindow.TryHitSlot(new Vector2(mousePosition.x, mousePosition.y), out GameItem box, out int slot) && box != null)
00796: 				{
00797: 					List<GameItem> list3 = new List<GameItem>();
00798: 					foreach (GameItem item5 in groupSnapshot)
00799: 					{
00800: 						if (item5 != null && !BoxItem.IsBox(item5))
00801: 						{
00802: 							list3.Add(item5);
00803: 						}
00804: 					}
00805: 					if (list3.Count > 0)
00806: 					{
00807: 						int batch2 = ((list3.Count > 1) ? (++_batchSeq) : 0);
00808: 						foreach (GameItem item6 in list3)
00809: 						{
00810: 							_pendingDeposits.Enqueue(new PendingDeposit
00811: 							{
00812: 								Box = box,
00813: 								Item = item6,
00814: 								Slot = slot,
00815: 								Batch = batch2
00816: 							});
00817: 						}
00818: 						Core.Debug("收纳箱 group drop：登记 " + list3.Count + " 件 → 槽 " + slot + "（批次 " + batch2 + "，队列 " + _pendingDeposits.Count + "）");
00819: 					}
00820: 				}
00821: 			}
00822: 			catch (Exception ex)
00823: 			{
00824: 				Core.Log?.Warning("收纳箱 group drop 判定：" + ex.Message);
00825: 			}
00826: 			_groupHoverItem = null;
00827: 		}
00828: 		catch (Exception ex2)
00829: 		{
00830: 			Core.Log?.Warning("ItemStorage 组拖拽轮询：" + ex2.Message);
00831: 			_groupWasActive = false;
00832: 			_groupSnapshot = null;
00833: 			_groupHoverItem = null;
00834: 		}
00835: 	}
00836: 
00837: 	private static void EnsureMultiHandler()
00838: 	{
00839: 		if (((Object)(object)_multiHandler != (Object)null && IsAliveMulti(_multiHandler)) || Time.time < _nextMultiFindAt)
00840: 		{
00841: 			return;
00842: 		}
00843: 		_nextMultiFindAt = Time.time + 2f;
00844: 		try
00845: 		{
00846: 			_multiHandler = Object.FindObjectOfType<ItemMultiSelectHandler>();
00847: 		}
00848: 		catch
00849: 		{
00850: 			_multiHandler = null;
00851: 		}
00852: 	}
00853: 
00854: 	private static bool IsAliveMulti(ItemMultiSelectHandler handler)
00855: 	{
00856: 		try
00857: 		{
00858: 			return (Object)(object)handler != (Object)null;
00859: 		}
00860: 		catch
00861: 		{
00862: 			return false;
00863: 		}
00864: 	}
00865: 
00866: 	private static void EnsureHandler()
00867: 	{
00868: 		if (((Object)(object)_dragHandler != (Object)null && IsAlive(_dragHandler)) || Time.time < _nextDragFindAt)
00869: 		{
00870: 			return;
00871: 		}
00872: 		_nextDragFindAt = Time.time + 2f;
00873: 		try
00874: 		{
00875: 			_dragHandler = Object.FindObjectOfType<ItemMouseDragHandler>();
00876: 		}
00877: 		catch
00878: 		{
00879: 			_dragHandler = null;
00880: 		}
00881: 	}
00882: 
00883: 	private static bool IsAlive(ItemMouseDragHandler handler)
00884: 	{
00885: 		try
00886: 		{
00887: 			return (Object)(object)handler != (Object)null;
00888: 		}
00889: 		catch
00890: 		{
00891: 			return false;
00892: 		}
00893: 	}
00894: 
00895: 	private static GameItem? GetDraggedItem(ItemMouseDragHandler? handler)
00896: 	{
00897: 		try
00898: 		{
00899: 			return (handler != null) ? handler.GetCurrentItem() : null;
00900: 		}
00901: 		catch (Exception ex)
00902: 		{
00903: 			Core.Debug("收纳箱 GetCurrentItem 读取失败：" + ex.Message);
00904: 			return null;
00905: 		}
00906: 	}
00907: 
00908: 	private static GameItem? GetLastItem(ItemMouseDragHandler? handler)
00909: 	{
00910: 		try
00911: 		{
00912: 			if ((Object)(object)handler == (Object)null)
00913: 			{
00914: 				return null;
00915: 			}
00916: 			if ((object)_lastItemField == null)
00917: 			{
00918: 				_lastItemField = AccessTools.Field(typeof(ItemMouseDragHandler), "lastItem");
00919: 			}
00920: 			object? obj = _lastItemField?.GetValue(handler);
00921: 			return (GameItem?)((obj is GameItem) ? obj : null);
00922: 		}
00923: 		catch (Exception ex)
00924: 		{
00925: 			Core.Debug("收纳箱 lastItem 读取失败：" + ex.Message);
00926: 			return null;
00927: 		}
00928: 	}
00929: 
00930: 	private static int? GetRemoveCount(ItemMouseDragHandler? handler, string name)
00931: 	{
00932: 		try
00933: 		{
00934: 			if ((Object)(object)handler == (Object)null)
00935: 			{
00936: 				return null;
00937: 			}
00938: 			FieldInfo fieldInfo = ((name == "currentRemove") ? (_currentRemoveField ?? (_currentRemoveField = AccessTools.Field(typeof(ItemMouseDragHandler), "currentRemove"))) : (_maxRemoveField ?? (_maxRemoveField = AccessTools.Field(typeof(ItemMouseDragHandler), "maxRemove"))));
00939: 			if (fieldInfo == null)
00940: 			{
00941: 				return null;
00942: 			}
00943: 			return Convert.ToInt32(fieldInfo.GetValue(handler));
00944: 		}
00945: 		catch
00946: 		{
00947: 			return null;
00948: 		}
00949: 	}
00950: 
00951: 	private static List<GameItem>? GetSelectedItems()
00952: 	{
00953: 		try
00954: 		{
00955: 			if ((Object)(object)_multiHandler == (Object)null)
00956: 			{
00957: 				return null;
00958: 			}
00959: 			if ((object)_selectedItemsField == null)
00960: 			{
00961: 				_selectedItemsField = AccessTools.Field(typeof(ItemMultiSelectHandler), "selectedItems");
00962: 			}
00963: 			if (!(_selectedItemsField?.GetValue(_multiHandler) is List<GameItemElement> list))
00964: 			{
00965: 				return null;
00966: 			}
00967: 			List<GameItem> list2 = new List<GameItem>(list.Count);
00968: 			foreach (GameItemElement item in list)
00969: 			{
00970: 				try
00971: 				{
00972: 					if (item != null)
00973: 					{
00974: 						list2.Add((GameItem)(object)item);
00975: 					}
00976: 				}
00977: 				catch
00978: 				{
00979: 				}
00980: 			}
00981: 			return list2;
00982: 		}
00983: 		catch
00984: 		{
00985: 			return null;
00986: 		}
00987: 	}
00988: 
00989: 	private static GameItem? GetGroupHoverItem()
00990: 	{
00991: 		try
00992: 		{
00993: 			if ((Object)(object)_multiHandler == (Object)null)
00994: 			{
~~~

### ItemStorage/BoxModDirectoryPatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: 
00004: namespace ItemStorage;
00005: 
00006: [HarmonyPatch(typeof(ModItemDirectory), "InitDirectory")]
00007: internal static class BoxModDirectoryPatch
00008: {
00009: 	[HarmonyPriority(0)]
00010: 	private static void Postfix(ModItemDirectory __instance)
00011: 	{
00012: 		try
00013: 		{
00014: 			BoxItem.Register(__instance);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Log?.Warning("收纳箱注册异常（ModItemDirectory）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### ItemStorage/JacksonStock.cs

~~~csharp
00089: 				catch
00090: 				{
00091: 					flag = false;
00092: 				}
00093: 				if (flag)
00094: 				{
00095: 					try
00096: 					{
00097: 						MapUIManager instance2 = MapUIManager.Instance;
00098: 						flag2 = (Object)(object)instance2 == (Object)null || instance2.Current == null || string.Equals(Core.Clean(instance2.Current.Id), "dr_jackson", StringComparison.OrdinalIgnoreCase);
00099: 					}
00100: 					catch
00101: 					{
00102: 						flag2 = true;
00103: 					}
00104: 					flag = flag2 && !FrontHasUnownedBox();
00105: 				}
00106: 			}
00107: 			if (!flag || _stockedThisArrival)
00108: 			{
00109: 				return;
00110: 			}
00111: 			_stockedThisArrival = true;
00112: 			GameItem val2 = DirectoryMaster.Item("Dream_Item_Storage", true);
00113: 			PlayerStore instance3 = PlayerStore.Instance;
00114: 			if (val2 == null || instance3 == null || !TryPlace(instance3, val2))
00115: 			{
00116: 				if (flag)
00117: 				{
00118: 					_stockedThisArrival = false;
00119: 				}
00120: 				Core.Debug("博士钩子补箱未落位，交由 Postfix/兜底接管。");
00121: 			}
00122: 			else
00123: 			{
00124: 				Core.Log?.Msg("博士货源已补入：收纳箱 ×1（官方铺货钩子，" + (flag2 ? "拜访博士" : "列博士货物首位") + "）。");
00125: 			}
00126: 		}
00127: 		catch (Exception ex)
00128: 		{
00129: 			_stockedThisArrival = false;
00130: 			Core.Debug("博士钩子补货异常：" + ex.Message);
00131: 		}
00132: 	}
00133: 
00134: 	internal static void OnClientArrived()
00135: 	{
00136: 		try
00137: 		{
00138: 			PlayerStore instance = PlayerStore.Instance;
00139: 			object obj;
00140: 			if (instance == null)
00141: 			{
00142: 				obj = null;
00143: 			}
00144: 			else
00145: 			{
00146: 				StoreClientInstance currentClientInstance = instance.currentClientInstance;
00147: 				obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00148: 			}
00149: 			StoreClient val = (StoreClient)obj;
00150: 			if (val == null || val == _lastClient)
00151: 			{
00152: 				return;
00153: 			}
00154: 			_lastClient = val;
00155: 			if (IsJackson(val))
00156: 			{
00157: 				string text = "";
00158: 				string text2 = "";
00159: 				try
00160: 				{
00161: 					text = Core.Clean(val.identifier) ?? "";
00162: 				}
00163: 				catch
00164: 				{
00165: 				}
00166: 				try
00167: 				{
00168: 					text2 = Core.Clean(val.displayName) ?? "";
00169: 				}
00170: 				catch
00171: 				{
00172: 				}
00173: 				Core.Debug("博士到场：id=" + text + " 名=" + text2 + "。");
00174: 			}
00175: 			_stockedThisArrival = IsScriptedVisit(val) && FrontHasUnownedBox();
00176: 			_fallbackDeferrals = 0;
00177: 			_fallbackCheckAt = Time.time + 3f;
00178: 		}
00179: 		catch (Exception ex)
00180: 		{
00181: 			Core.Debug("博士货源处理失败：" + ex.Message);
00182: 		}
00183: 	}
00184: 
00185: 	internal static void OnMerchantItemAddedToTable()
00186: 	{
00187: 		try
00188: 		{
00189: 			if (_stockedThisArrival)
00190: 			{
00191: 				return;
00192: 			}
00193: 			PlayerStore instance = PlayerStore.Instance;
~~~

~~~csharp
00318: 		return false;
00319: 	}
00320: 
00321: 	private static bool IsScriptedVisit(StoreClient client)
00322: 	{
00323: 		try
00324: 		{
00325: 			string a = (Core.Clean(client.identifier) ?? "").ToLowerInvariant();
00326: 			string[] scriptedVisits = ScriptedVisits;
00327: 			foreach (string b in scriptedVisits)
00328: 			{
00329: 				if (string.Equals(a, b, StringComparison.Ordinal))
00330: 				{
00331: 					return true;
00332: 				}
00333: 			}
00334: 		}
00335: 		catch
00336: 		{
00337: 		}
00338: 		return false;
00339: 	}
00340: 
00341: 	private static bool EnsureStock()
00342: 	{
00343: 		EmporiumEntry instance = EmporiumEntry.Instance;
00344: 		GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
00345: 		if (val == null)
00346: 		{
00347: 			Core.Debug("博士柜台不可读，下次到场重试。");
00348: 			return false;
00349: 		}
00350: 		GameItem val2 = DirectoryMaster.Item("Dream_Item_Storage", true);
00351: 		if (val2 == null)
00352: 		{
00353: 			Core.Debug("收纳箱工厂未就绪，下次到场重试，不留标记。");
00354: 			return false;
00355: 		}
00356: 		PlayerStore instance2 = PlayerStore.Instance;
00357: 		if (instance2 == null)
00358: 		{
00359: 			Core.Debug("PlayerStore 未就绪，下次到场重试。");
00360: 			return false;
00361: 		}
00362: 		int num = 0;
00363: 		while (val.TryFindOneValidInventorySlot(val2, false) == null)
00364: 		{
00365: 			if (!TryMakeRoom(val) || ++num > 8)
00366: 			{
00367: 				Core.Debug("博士柜台放不下收纳箱且无可挤重复货物（已挤 " + num + " 件），下次到场重试。");
00368: 				return false;
00369: 			}
00370: 		}
00371: 		if (TryPlace(instance2, val2))
00372: 		{
00373: 			Core.Log?.Msg("博士货源已补入：收纳箱 ×1（柜台，走购买流程）。");
00374: 			return true;
00375: 		}
00376: 		while (num <= 8 && TryMakeRoom(val))
00377: 		{
00378: 			num++;
00379: 			GameItem val3 = DirectoryMaster.Item("Dream_Item_Storage", true);
00380: 			if (val3 != null && TryPlace(instance2, val3))
00381: 			{
00382: 				Core.Log?.Msg("博士货源已补入：收纳箱 ×1（柜台，走购买流程，挤占 " + num + " 件后自愈）。");
00383: 				return true;
00384: 			}
00385: 		}
00386: 		Core.Debug("博士货源上柜未落位（循环挤占后仍失败），回退标记待重试。");
00387: 		return false;
00388: 	}
00389: 
00390: 	private static bool FrontHasUnownedBox()
00391: 	{
00392: 		EmporiumEntry instance = EmporiumEntry.Instance;
00393: 		GameGridInventory val = ((instance != null) ? instance.frontInvinvElement : null);
00394: 		if (val == null)
00395: 		{
00396: 			return false;
00397: 		}
00398: 		try
00399: 		{
00400: 			foreach (GameItem childItem in ((GameInventory)val).childItems)
00401: 			{
00402: 				if (childItem == null)
00403: 				{
00404: 					continue;
00405: 				}
00406: 				string a = null;
00407: 				try
00408: 				{
00409: 					a = Core.Clean(childItem.identifier);
00410: 				}
00411: 				catch
00412: 				{
00413: 				}
00414: 				if (!string.Equals(a, "Dream_Item_Storage", StringComparison.Ordinal))
00415: 				{
00416: 					continue;
00417: 				}
00418: 				try
00419: 				{
00420: 					if (!GeneralHelper.IsItemOwned(childItem))
00421: 					{
00422: 						return true;
00423: 					}
00424: 				}
00425: 				catch
00426: 				{
00427: 				}
00428: 			}
00429: 		}
00430: 		catch (Exception ex)
00431: 		{
00432: 			Core.Debug("拜访补箱去重检查失败（按未补处理）：" + ex.Message);
00433: 		}
00434: 		return false;
00435: 	}
00436: 
00437: 	private static bool TryPlace(PlayerStore store, GameItem box)
00438: 	{
00439: 		if (box == null)
00440: 		{
00441: 			return false;
00442: 		}
00443: 		try
00444: 		{
00445: 			return store.TryAddDirectSellingItemToTable(box, false, false, false, 0);
00446: 		}
00447: 		catch (Exception ex)
00448: 		{
00449: 			Core.Debug("博士上柜调用异常：" + ex.Message);
00450: 			return false;
00451: 		}
~~~

### ItemStorage/BoxMiscDirectoryPatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: 
00004: namespace ItemStorage;
00005: 
00006: [HarmonyPatch(typeof(MiscItemDirectory), "InitDirectory")]
00007: internal static class BoxMiscDirectoryPatch
00008: {
00009: 	[HarmonyPriority(0)]
00010: 	private static void Postfix(MiscItemDirectory __instance)
00011: 	{
00012: 		try
00013: 		{
00014: 			BoxItem.Register(__instance);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Log?.Warning("收纳箱注册异常（MiscItemDirectory）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

### ItemStorage/BoxStowAll.cs

~~~csharp
00052: 			PlayerStore instance = PlayerStore.Instance;
00053: 			if (instance != null)
00054: 			{
00055: 				_loaded = true;
00056: 				int num = PerSaveState.GetInt(instance, "StowAllHotkey");
00057: 				_key = (KeyCode)((num >= 8 && num <= 350) ? num : 0);
00058: 			}
00059: 		}
00060: 		catch
00061: 		{
00062: 			_loaded = true;
00063: 			_key = (KeyCode)0;
00064: 		}
00065: 	}
00066: 
00067: 	internal static void SetKey(KeyCode key)
00068: 	{
00069: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00070: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
00071: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
00072: 		//IL_0026: Expected I4, but got Unknown
00073: 		_key = key;
00074: 		_loaded = true;
00075: 		ResetWatch();
00076: 		try
00077: 		{
00078: 			PlayerStore instance = PlayerStore.Instance;
00079: 			if (instance != null)
00080: 			{
00081: 				PerSaveState.SetInt(instance, "StowAllHotkey", (int)key);
00082: 				PerSaveState.Save(instance);
00083: 			}
00084: 		}
00085: 		catch (Exception ex)
00086: 		{
00087: 			Core.Log?.Warning("全箱收纳键位写入存档失败：" + ex.Message);
00088: 		}
00089: 	}
00090: 
00091: 	private static void ClearKey(string reason)
00092: 	{
00093: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
00094: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00095: 		if (_loaded && (int)_key == 0)
00096: 		{
00097: 			return;
00098: 		}
00099: 		_key = (KeyCode)0;
00100: 		_loaded = true;
00101: 		try
00102: 		{
00103: 			PlayerStore instance = PlayerStore.Instance;
00104: 			if (instance != null)
00105: 			{
00106: 				PerSaveState.SetInt(instance, "StowAllHotkey", 0);
00107: 			}
00108: 		}
00109: 		catch
00110: 		{
00111: 		}
00112: 		Core.Log?.Msg("全箱收纳键位已自动解绑：" + reason);
00113: 	}
00114: 
00115: 	internal static void UnbindManually()
00116: 	{
00117: 		ClearKey("玩家在齿轮面板捕获态按 Esc 主动解除绑定");
00118: 	}
00119: 
00120: 	internal static void ResetRecovery()
00121: 	{
00122: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
00123: 		_loaded = false;
00124: 		_key = (KeyCode)0;
00125: 		ResetWatch();
00126: 	}
00127: 
00128: 	internal unsafe static void Tick()
00129: 	{
00130: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00131: 		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
00132: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00133: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00134: 		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
00135: 		KeyCode key = Key;
00136: 		if ((int)key == 0)
00137: 		{
00138: 			return;
00139: 		}
00140: 		try
00141: 		{
00142: 			if (!Input.GetKeyDown(key))
00143: 			{
00144: 				return;
00145: 			}
00146: 		}
00147: 		catch
00148: 		{
00149: 			return;
00150: 		}
00151: 		if (GearPanel.IsCapturing || BoxHotkey.IsSuppressed(key))
00152: 		{
00153: 			return;
00154: 		}
00155: 		try
00156: 		{
00157: 			if (KeyBindHelper.SuppressHotkeys)
00158: 			{
00159: 				return;
00160: 			}
00161: 		}
00162: 		catch
00163: 		{
00164: 		}
00165: 		try
00166: 		{
00167: 			if (StoreUIManager.IsTextFieldFocused())
00168: 			{
00169: 				return;
00170: 			}
00171: 		}
00172: 		catch
00173: 		{
00174: 		}
00175: 		if (!BoxTransfer.IsInStore())
00176: 		{
00177: 			return;
00178: 		}
00179: 		Core.Debug("全箱收纳快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 触发。");
00180: 		BoxItem.ClickSound();
00181: 		try
00182: 		{
00183: 			RunAll("全箱快捷键");
00184: 		}
00185: 		catch (Exception ex)
00186: 		{
00187: 			Core.Log?.Warning("全箱收纳执行异常：" + ex.Message);
00188: 		}
00189: 	}
00190: 
00191: 	internal static void Watch(int singleBoundCount)
00192: 	{
00193: 		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
00194: 		if ((singleBoundCount == 0 && (int)Key == 0) || Time.unscaledTime < _nextWatch)
00195: 		{
00196: 			return;
00197: 		}
00198: 		_nextWatch = Time.unscaledTime + 2f;
00199: 		try
00200: 		{
00201: 			if (!BoxTransfer.IsInStore())
00202: 			{
00203: 				ResetWatch();
00204: 				return;
00205: 			}
00206: 		}
00207: 		catch
00208: 		{
00209: 			return;
00210: 		}
00211: 		Dictionary<long, GameItem> dictionary = new Dictionary<long, GameItem>();
00212: 		try
00213: 		{
00214: 			if (PlayerStore.Instance == null)
00215: 			{
00216: 				return;
00217: 			}
00218: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00219: 			{
00220: 				if (item != null && BoxItem.IsBox(item) && BoxItem.IsUsableByPlayer(item))
00221: 				{
00222: 					long num;
00223: 					try
00224: 					{
00225: 						num = item.uniqueId;
00226: 					}
00227: 					catch
00228: 					{
00229: 						continue;
00230: 					}
00231: 					if (num != 0L)
00232: 					{
00233: 						dictionary[num] = item;
00234: 					}
00235: 				}
00236: 			}
00237: 		}
00238: 		catch
00239: 		{
00240: 			return;
00241: 		}
00242: 		foreach (KeyValuePair<long, GameItem> item2 in dictionary)
00243: 		{
00244: 			_tracked[item2.Key] = item2.Value;
00245: 		}
00246: 		if (dictionary.Count > 0)
00247: 		{
00248: 			_hadOwner = true;
00249: 		}
00250: 		List<long> list = new List<long>();
00251: 		foreach (KeyValuePair<long, GameItem> item3 in _tracked)
00252: 		{
00253: 			if (!dictionary.ContainsKey(item3.Key) && BoxItem.IsReallyDestroyed(item3.Value))
00254: 			{
00255: 				list.Add(item3.Key);
00256: 			}
00257: 		}
00258: 		foreach (long item4 in list)
00259: 		{
00260: 			_tracked.Remove(item4);
00261: 			BoxHotkey.ForgetUid(item4);
00262: 			Core.Log?.Msg("收纳箱（uid " + item4 + "）已被系统销毁（卖出/垃圾桶清空等），其单箱快捷键已释放。");
00263: 		}
00264: 		if (dictionary.Count > 0 || _tracked.Count > 0)
00265: 		{
00266: 			_zeroStreak = 0;
00267: 		}
00268: 		else if (_hadOwner && ++_zeroStreak >= 5)
00269: 		{
00270: 			ClearKey("玩家拥有的收纳箱已全部被系统销毁（卖出/垃圾桶清空等），释放全箱键供其他功能绑定");
00271: 			BoxRestock.OnAllBoxesDestroyed();
00272: 			_hadOwner = false;
00273: 			_zeroStreak = 0;
00274: 		}
00275: 	}
00276: 
00277: 	private static void ResetWatch()
00278: 	{
00279: 		_tracked.Clear();
00280: 		_hadOwner = false;
00281: 		_zeroStreak = 0;
00282: 	}
00283: 
00284: 	internal static void RunAll(string context)
00285: 	{
00286: 		if (!BoxTransfer.IsInStore())
00287: 		{
00288: 			Core.Debug("全箱收纳[" + context + "]：玩家不在店内，忽略。");
00289: 			BoxItem.NotifyUser("请回到店内再收纳");
00290: 			return;
00291: 		}
00292: 		Stopwatch stopwatch = Stopwatch.StartNew();
00293: 		List<Participant> list = new List<Participant>();
00294: 		try
00295: 		{
00296: 			foreach (GameItem item in BoxItem.FindAllBoxes())
00297: 			{
00298: 				if (item != null && BoxItem.IsUsableByPlayer(item))
00299: 				{
00300: 					list.Add(new Participant
00301: 					{
00302: 						Box = item,
00303: 						Num = BoxItem.ReadNumber(item)
00304: 					});
00305: 				}
00306: 			}
00307: 		}
00308: 		catch (Exception ex)
00309: 		{
00310: 			Core.Log?.Warning("全箱收纳[" + context + "] 扫描箱子失败：" + ex.Message);
00311: 			return;
00312: 		}
00313: 		if (list.Count == 0)
00314: 		{
00315: 			if (_hadOwner && _tracked.Count == 0)
00316: 			{
00317: 				ClearKey("玩家已没有任何拥有的收纳箱（Watch 已确认全销毁，按键兜底清除）");
00318: 			}
00319: 			else
00320: 			{
00321: 				Core.Log?.Msg("全箱收纳[" + context + "]：场景里没有可用的收纳箱（可能拿在手上或不在店内），键位保留。");
00322: 			}
00323: 			return;
00324: 		}
00325: 		list.Sort(delegate(Participant a, Participant b)
00326: 		{
00327: 			if (a.Num == b.Num)
00328: 			{
00329: 				return 0;
00330: 			}
00331: 			if (a.Num == -1)
00332: 			{
00333: 				return 1;
00334: 			}
00335: 			return (b.Num == -1) ? (-1) : a.Num.CompareTo(b.Num);
00336: 		});
00337: 		int num = 0;
00338: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
00339: 		foreach (Participant item2 in list)
00340: 		{
00341: 			Dictionary<string, int> dictionary = AutoStow.ReadSpace(item2.Box, context);
00342: 			if (dictionary == null || dictionary.Count == 0)
00343: 			{
00344: 				continue;
00345: 			}
00346: 			item2.Space = dictionary;
00347: 			foreach (string key in dictionary.Keys)
00348: 			{
00349: 				hashSet.Add(key);
00350: 			}
00351: 			num++;
00352: 		}
00353: 		if (num == 0)
00354: 		{
00355: 			Core.Log?.Msg("全箱收纳[" + context + "]：所有收纳箱都是空的，没有可收的目标种类。");
00356: 			BoxItem.NotifyUser("没有可收纳的物品");
00357: 			return;
00358: 		}
00359: 		List<GameItem> list2 = AutoStow.CollectCandidates(hashSet, context);
00360: 		int num2 = 0;
00361: 		int num3 = 0;
00362: 		int num4 = 0;
00363: 		HashSet<GameItem> hashSet2 = new HashSet<GameItem>();
00364: 		foreach (GameItem item3 in list2)
00365: 		{
00366: 			string text;
00367: 			try
00368: 			{
00369: 				text = Core.Clean(item3.identifier) ?? string.Empty;
00370: 			}
00371: 			catch
00372: 			{
00373: 				text = string.Empty;
00374: 			}
00375: 			if (string.IsNullOrEmpty(text) || !hashSet.Contains(text))
00376: 			{
00377: 				continue;
00378: 			}
00379: 			int num5;
00380: 			try
00381: 			{
00382: 				num5 = Math.Max(1, item3.unitCount);
00383: 			}
00384: 			catch
00385: 			{
00386: 				num5 = 1;
00387: 			}
00388: 			bool flag = false;
00389: 			foreach (Participant item4 in list)
00390: 			{
00391: 				Dictionary<string, int> space = item4.Space;
00392: 				if (space != null && space.TryGetValue(text, out var value) && num5 <= value && AutoStow.MoveOneInto(item4.Box, item3, space, context, out var movedUnits, out var _))
00393: 				{
00394: 					num2++;
00395: 					num3 += movedUnits;
00396: 					hashSet2.Add(item4.Box);
00397: 					flag = true;
00398: 					break;
00399: 				}
00400: 			}
00401: 			if (!flag)
00402: 			{
00403: 				num4++;
00404: 			}
00405: 		}
00406: 		Core.Debug("全箱收纳[" + context + "] 耗时 " + stopwatch.ElapsedMilliseconds + " ms（收 " + num2 + " 叠 / 候选 " + list2.Count + " 件 / 参与箱 " + num + "）。");
00407: 		if (num2 > 0)
00408: 		{
00409: 			foreach (GameItem item5 in hashSet2)
00410: 			{
00411: 				try
00412: 				{
00413: 					BoxWindow.RefreshButtons(item5);
00414: 				}
00415: 				catch
00416: 				{
00417: 				}
00418: 			}
00419: 			try
00420: 			{
00421: 				EmporiumEntry instance = EmporiumEntry.Instance;
00422: 				if (instance != null)
00423: 				{
00424: 					instance.Validate(true);
00425: 				}
00426: 			}
00427: 			catch
00428: 			{
00429: 			}
00430: 			Core.Log?.Msg("全箱收纳[" + context + "] 完成：收进 " + num2 + " 叠共 " + num3 + " 件，分入 " + hashSet2.Count + " / " + num + " 个收纳箱" + ((num4 > 0) ? ("，" + num4 + " 叠装不下，留原地") : "") + "。");
00431: 		}
00432: 		else
00433: 		{
00434: 			Core.Log?.Msg("全箱收纳[" + context + "]：店里没有能收进收纳箱的物品" + ((num4 > 0) ? ("（" + num4 + " 叠装不下，留原地）") : "") + "。");
00435: 		}
00436: 		if (num3 > 0)
00437: 		{
00438: 			BoxItem.NotifyUser("已收进 " + num3 + " 件，分派到各收纳箱");
00439: 		}
00440: 		else
00441: 		{
00442: 			BoxItem.NotifyUser("没有可收纳的物品");
00443: 		}
00444: 	}
00445: }
~~~

### ItemStorage/ItemStorageApi.cs

~~~csharp
00441: 
00442: 	private static bool DetachFromGrid(GameItem it, List<GraphNodeStorage> parents)
00443: 	{
00444: 		try
00445: 		{
00446: 			int num = 0;
00447: 			foreach (GraphNodeStorage parent in parents)
00448: 			{
00449: 				if (parent == null)
00450: 				{
00451: 					num++;
00452: 					continue;
00453: 				}
00454: 				try
00455: 				{
00456: 					num += (it.RemoveParent(parent) ? 1 : 0);
00457: 				}
00458: 				catch
00459: 				{
00460: 					num = num;
00461: 				}
00462: 			}
00463: 			return num >= parents.Count;
00464: 		}
00465: 		catch (Exception ex)
00466: 		{
00467: 			Core.Log?.Warning("[堆叠收纳API] 脱离网格失败：" + ex.Message);
00468: 			return false;
00469: 		}
00470: 	}
00471: 
00472: 	private static void RefreshBoxUi(GameItem pack)
00473: 	{
00474: 		try
00475: 		{
00476: 			BoxWindow.RefreshButtons(pack);
00477: 		}
00478: 		catch
00479: 		{
00480: 		}
00481: 		try
00482: 		{
00483: 			BoxWindow.UpdateTooltip(pack);
00484: 		}
00485: 		catch
00486: 		{
00487: 		}
00488: 	}
00489: 
00490: 	public static WithdrawStatus TryWithdraw(GameItem? pack, int slot, GameGridInventory? dropGrid = null)
00491: 	{
00492: 		if (pack == null)
00493: 		{
00494: 			return WithdrawStatus.Empty;
00495: 		}
00496: 		try
00497: 		{
00498: 			return BoxTransfer.TryWithdraw(pack, slot, dropGrid) switch
00499: 			{
00500: 				BoxTransfer.WithdrawResult.Ok => WithdrawStatus.Ok, 
00501: 				BoxTransfer.WithdrawResult.NoSpace => WithdrawStatus.NoSpace, 
00502: 				BoxTransfer.WithdrawResult.NotOnGrid => WithdrawStatus.NotOnGrid, 
00503: 				BoxTransfer.WithdrawResult.SilentFail => WithdrawStatus.SilentFail, 
00504: 				_ => WithdrawStatus.Empty, 
00505: 			};
00506: 		}
00507: 		catch (Exception ex)
00508: 		{
00509: 			Core.Log?.Warning("[堆叠收纳API] TryWithdraw(" + slot + ") 异常：" + ex.Message);
00510: 			return WithdrawStatus.SilentFail;
00511: 		}
00512: 	}
00513: }
~~~

### ItemStorage/BoxContainerDirectoryPatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: 
00004: namespace ItemStorage;
00005: 
00006: [HarmonyPatch(typeof(ContainerItemDirectory), "InitDirectory")]
00007: internal static class BoxContainerDirectoryPatch
00008: {
00009: 	[HarmonyPriority(0)]
00010: 	private static void Postfix(ContainerItemDirectory __instance)
00011: 	{
00012: 		try
00013: 		{
00014: 			BoxItem.Register(__instance);
00015: 		}
00016: 		catch (Exception ex)
00017: 		{
00018: 			Core.Log?.Warning("收纳箱注册异常（ContainerItemDirectory）：" + ex.Message);
00019: 		}
00020: 	}
00021: }
~~~

## tentacle

### MidnightPawn/ModEntry.cs

~~~csharp
00001: using System;
00002: using System.Reflection;
00003: using HarmonyLib;
00004: using MidnightPawn.Game;
00005: 
00006: namespace MidnightPawn;
00007: 
00008: public class ModEntry : IMod
00009: {
00010: 	private ModLog log;
00011: 
00012: 	public void Init(ModManifest manifest)
00013: 	{
00014: 		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
00015: 		//IL_000c: Expected O, but got Unknown
00016: 		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
00017: 		log = new ModLog(manifest);
00018: 		ModCtx.Attach(log);
00019: 		ModCtx.Log("淫色模因 · 深空当铺 初始化 (v" + manifest.ModVersion + ")");
00020: 		ModHook.OnModItemDirectoryInit += ItemFactory.Register;
00021: 		ModHook.OnLootRolled += LootHook.OnLootRolled;
00022: 		MaidenTooltip.Register();
00023: 		Perks.Register();
00024: 		try
00025: 		{
00026: 			new Harmony("lewd_meme").PatchAll(Assembly.GetExecutingAssembly());
00027: 			ModCtx.Log("Harmony 补丁已应用");
00028: 		}
00029: 		catch (Exception ex)
00030: 		{
00031: 			ModCtx.LogWarning("Harmony 补丁失败: " + ex.Message);
00032: 		}
00033: 		try
00034: 		{
00035: 			ModHelper.AddDebugFunction("lewd_meme", "Spawn", "生成少女", (Action)LootHook.SpawnMaiden, false);
00036: 			ModHelper.AddDebugFunction("lewd_meme", "Spawn", "生成触手本体", (Action)delegate
00037: 			{
00038: 				GameItem val = ModHelper.SpawnItem("tentacle_body");
00039: 				if (val != null)
00040: 				{
00041: 					ModHelper.MoveItemToInventory(val);
00042: 				}
00043: 			}, false);
00044: 			ModHelper.AddDebugFunction("lewd_meme", "Spawn", "生成乳汁+精液", (Action)delegate
00045: 			{
00046: 				GameItem val = ModHelper.SpawnItem("wondrous_milk");
00047: 				if (val != null)
00048: 				{
00049: 					ModHelper.MoveItemToInventory(val);
00050: 				}
00051: 				GameItem val2 = ModHelper.SpawnItem("tentacle_semen");
00052: 				if (val2 != null)
00053: 				{
00054: 					ModHelper.MoveItemToInventory(val2);
00055: 				}
00056: 			}, false);
00057: 		}
00058: 		catch (Exception ex2)
00059: 		{
00060: 			ModCtx.LogWarning("注册调试按钮失败: " + ex2.Message);
00061: 		}
00062: 	}
00063: 
00064: 	public void OnEnable()
00065: 	{
00066: 		ModCtx.Log("淫色模因 已启用");
00067: 	}
00068: 
00069: 	public void OnDisable()
00070: 	{
00071: 		ModCtx.Log("淫色模因 已停用");
00072: 		try
00073: 		{
00074: 			ModHelper.RemoveDebugFunctions("lewd_meme");
00075: 		}
00076: 		catch
00077: 		{
00078: 		}
00079: 	}
00080: }
~~~

### MidnightPawn/Game/LootHook.cs

~~~csharp
00001: using UnityEngine;
00002: 
00003: namespace MidnightPawn.Game;
00004: 
00005: public static class LootHook
00006: {
00007: 	public static void OnLootRolled(LootRollContext ctx)
00008: 	{
00009: 		if (ctx == null)
00010: 		{
00011: 			return;
00012: 		}
00013: 		string text = ((ctx.groupId == null) ? "" : ctx.groupId.ToLower());
00014: 		if (text.Contains("med") || text.Contains("drug") || text.Contains("pharma") || text.Contains("chem"))
00015: 		{
00016: 			if (Random.value < 0.1f)
00017: 			{
00018: 				ctx.itemId = "ovulation_shot";
00019: 				ModCtx.Log("战利品：医疗类里翻到一支排卵诱发剂 (" + ctx.groupId + ")");
00020: 			}
00021: 		}
00022: 		else if ((ctx.groupId == "dumpingGroundTG" || ctx.groupId == "dumpingGroundFreshTG") && TentacleBody.HasBody())
00023: 		{
00024: 			int bodyLevel = TentacleBody.GetBodyLevel();
00025: 			float num = 0.1f + (float)(bodyLevel - 1) * 0.1f;
00026: 			if (Random.value < num)
00027: 			{
00028: 				ctx.itemId = "maiden_lower";
00029: 				ModCtx.Log("拾荒：携带触手本体，捕获到一个少女 (Lv." + bodyLevel + " 概率 " + num.ToString("P0") + ")");
00030: 			}
00031: 			else if (Random.value < 0.08f)
00032: 			{
00033: 				ctx.itemId = "ovulation_shot";
00034: 				ModCtx.Log("拾荒：捡到一支排卵诱发剂");
00035: 			}
00036: 		}
00037: 	}
00038: 
00039: 	public static void SpawnMaiden()
00040: 	{
00041: 		GameItem val = ModHelper.SpawnItem("maiden_lower");
00042: 		if (val != null)
00043: 		{
00044: 			ModHelper.SetItemOwnerShip(val, true);
00045: 			ModHelper.MoveItemToInventory(val);
00046: 		}
00047: 	}
00048: }
~~~

### MidnightPawn/Game/Perks.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: 
00004: namespace MidnightPawn.Game;
00005: 
00006: public static class Perks
00007: {
00008: 	public const string PERK_TENTACLE = "lewd_meme.tentacle_origin";
00009: 
00010: 	public static void Register()
00011: 	{
00012: 		try
00013: 		{
00014: 			ModHelper.AddStartingPerk("lewd_meme", "lewd_meme.tentacle_origin", 0, (StartingPerkType)0, (Func<string>)(() => "触手起源"), (Func<string>)(() => "你生来就不是人。开局获得【触手本体】，额外获得 3 个特性槽位，每日拾荒次数 +3，且每天会有一位女性顾客被你的气味吸引而来。"), "lewd_meme:tentacle_perk", (string[])null, 3, false);
00015: 			ModHook.OnNewGameStarted += OnNewGameStarted;
00016: 			ModHook.OnWakingUpLate += OnNewDay;
00017: 			ModCtx.Log("起始天赋已注册: 触手起源");
00018: 		}
00019: 		catch (Exception ex)
00020: 		{
00021: 			ModCtx.LogWarning("注册起始天赋失败: " + ex.Message);
00022: 		}
00023: 	}
00024: 
00025: 	private static void OnNewGameStarted(string scenarioId)
00026: 	{
00027: 		try
00028: 		{
00029: 			if (ModHelper.IsStartingPerkActive("lewd_meme.tentacle_origin"))
00030: 			{
00031: 				GameItem val = ModHelper.SpawnItem("tentacle_body");
00032: 				if (val != null)
00033: 				{
00034: 					ModHelper.SetItemOwnerShip(val, true);
00035: 					ModHelper.MoveItemToInventory(val);
00036: 					ModCtx.Log("触手起源：开局获得触手本体");
00037: 				}
00038: 			}
00039: 		}
00040: 		catch (Exception ex)
00041: 		{
00042: 			ModCtx.LogWarning("触手起源发奖失败: " + ex.Message);
00043: 		}
00044: 	}
00045: 
00046: 	private static void OnNewDay()
00047: 	{
00048: 		try
00049: 		{
00050: 			if (!ModHelper.IsStartingPerkActive("lewd_meme.tentacle_origin"))
00051: 			{
00052: 				return;
00053: 			}
00054: 			PlayerStore instance = PlayerStore.Instance;
00055: 			if (instance == null || instance.storeClientManager == null)
00056: 			{
00057: 				return;
00058: 			}
00059: 			StoreClientManager storeClientManager = instance.storeClientManager;
00060: 			StoreClient randomNewMinorClient = storeClientManager.GetRandomNewMinorClient();
00061: 			if (randomNewMinorClient == null)
00062: 			{
00063: 				ModCtx.LogWarning("触手起源：今天没有可用的顾客模板");
00064: 				return;
00065: 			}
00066: 			string randomLowerFemaleSpriteName = SpriteDict.GetRandomLowerFemaleSpriteName();
00067: 			if (!string.IsNullOrEmpty(randomLowerFemaleSpriteName))
00068: 			{
00069: 				randomNewMinorClient.spriteName = randomLowerFemaleSpriteName;
00070: 				randomNewMinorClient.possibleSprites = new List<string> { randomLowerFemaleSpriteName };
00071: 			}
00072: 			randomNewMinorClient.identifier = "lewd_meme_fan_" + Guid.NewGuid().ToString("N").Substring(0, 8);
00073: 			randomNewMinorClient.displayName = NameGen.RandomName();
00074: 			randomNewMinorClient.cooldown = 0;
00075: 			storeClientManager.AddClient(randomNewMinorClient);
00076: 			ModCtx.Log("触手起源：今日吸引来一位女性顾客 —— " + randomNewMinorClient.displayName);
00077: 		}
00078: 		catch (Exception ex)
00079: 		{
00080: 			ModCtx.LogWarning("触手起源吸引顾客失败: " + ex.Message);
00081: 		}
00082: 	}
00083: }
~~~

### MidnightPawn/Game/MaidenTooltip.cs

~~~csharp
00001: using System;
00002: using System.Text;
00003: 
00004: namespace MidnightPawn.Game;
00005: 
00006: public static class MaidenTooltip
00007: {
00008: 	public static void Register()
00009: 	{
00010: 		ModHook.OnCreateTooltipLate += OnTooltip;
00011: 	}
00012: 
00013: 	public static void Unregister()
00014: 	{
00015: 		ModHook.OnCreateTooltipLate -= OnTooltip;
00016: 	}
00017: 
00018: 	private static void OnTooltip(RichTextBuilder builder, GameItem item)
00019: 	{
00020: 		if (builder == null || item == null)
00021: 		{
00022: 			return;
00023: 		}
00024: 		if (item.IsTag("TENTACLE_BODY_TAG"))
00025: 		{
00026: 			try
00027: 			{
00028: 				int bodyLevel = TentacleBody.GetBodyLevel();
00029: 				int bodyExp = TentacleBody.GetBodyExp();
00030: 				int expNeed = TentacleBody.GetExpNeed();
00031: 				builder.AddEmptyLine(2);
00032: 				builder.AddLine($"触手本体 Lv.{bodyLevel}   经验 {bodyExp} / {expNeed}", false, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00033: 				builder.AddLine("携带它去垃圾场拾荒，有概率捕获少女。", false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00034: 				string modData = ModHelper.GetModData("lewd_meme", "last_report", "");
00035: 				if (!string.IsNullOrEmpty(modData))
00036: 				{
00037: 					builder.AddLine("昨晚：" + modData, false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00038: 				}
00039: 				return;
00040: 			}
00041: 			catch (Exception ex)
00042: 			{
00043: 				ModCtx.LogWarning("触手本体 tooltip 失败: " + ex.Message);
00044: 				return;
00045: 			}
00046: 		}
00047: 		if (!item.IsTag("MAIDEN_FEMALE"))
00048: 		{
00049: 			return;
00050: 		}
00051: 		try
00052: 		{
00053: 			string str = ItemFactory.GetStr(item, "MAIDEN_NAME");
00054: 			string str2 = ItemFactory.GetStr(item, "MAIDEN_FACTION");
00055: 			string str3 = ItemFactory.GetStr(item, "MAIDEN_JOB");
00056: 			string str4 = ItemFactory.GetStr(item, "MAIDEN_BREAST");
00057: 			string str5 = ItemFactory.GetStr(item, "MAIDEN_BODY");
00058: 			builder.AddEmptyLine(2);
00059: 			if (!string.IsNullOrEmpty(str))
00060: 			{
00061: 				builder.AddLine("姓名：" + str, false, (ColorPalette)4367018, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00062: 			}
00063: 			StringBuilder stringBuilder = new StringBuilder();
00064: 			stringBuilder.Append("【高级违禁品】【女性角色】");
00065: 			if (!string.IsNullOrEmpty(str2))
00066: 			{
00067: 				stringBuilder.Append("【").Append(str2).Append("】");
00068: 			}
00069: 			if (!string.IsNullOrEmpty(str3))
00070: 			{
00071: 				stringBuilder.Append("【").Append(str3).Append("】");
00072: 			}
00073: 			if (!string.IsNullOrEmpty(str5))
00074: 			{
00075: 				stringBuilder.Append("【").Append(str5).Append("】");
00076: 			}
00077: 			if (!string.IsNullOrEmpty(str4))
00078: 			{
00079: 				stringBuilder.Append("【").Append(str4).Append("】");
00080: 			}
00081: 			if (item.IsTag("MAIDEN_VIRGIN"))
00082: 			{
00083: 				stringBuilder.Append("【处女】");
00084: 			}
00085: 			if (item.IsTag("MAIDEN_VIRGIN_LOST"))
00086: 			{
00087: 				stringBuilder.Append("【处女丧失】");
00088: 			}
00089: 			if (item.IsTag("MAIDEN_ANAL_VIRGIN"))
00090: 			{
00091: 				stringBuilder.Append("【肛门处女】");
00092: 			}
00093: 			if (item.IsTag("MAIDEN_ANAL_VIRGIN_LOST"))
00094: 			{
00095: 				stringBuilder.Append("【肛门处女丧失】");
00096: 			}
00097: 			builder.AddLine(stringBuilder.ToString(), false, (ColorPalette)11567781, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00098: 			int num = ItemFactory.GetInt(item, "TRAIN_VALUE");
00099: 			int num2 = ItemFactory.GetInt(item, "LACTATION_LV");
00100: 			int num3 = ItemFactory.GetInt(item, "ORAL_COUNT");
00101: 			int num4 = ItemFactory.GetInt(item, "SEX_COUNT");
00102: 			int num5 = ItemFactory.GetInt(item, "ANAL_COUNT");
00103: 			builder.AddLine($"调教值 {num}  泌乳等级 {num2}", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00104: 			builder.AddLine($"口交 {num3} 次 · 性交 {num4} 次 · 肛交 {num5} 次", false, (ColorPalette)12092762, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00105: 			if (item.IsTag("MAIDEN_OVULATING"))
00106: 			{
00107: 				builder.AddLine("【排卵期】", false, (ColorPalette)8035658, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
00108: 			}
00109: 			if (item.IsTag("MAIDEN_PREGNANT"))
00110: 			{
00111: 				int num6 = ItemFactory.GetInt(item, "MAIDEN_PREGNANT_DAYS");
00112: 				builder.AddLine($"【已怀孕】第 {num6}/30 天", false, (ColorPalette)12615037, false, false, false, false, (ColorPalette)(-1), (ColorPalette)(-1), (ColorPalette)(-1));
~~~

### MidnightPawn/Game/TentacleBody.cs

~~~csharp
00024: 
00025: 	private static readonly int[] EXP_NEED = new int[6] { 100, 200, 400, 800, 1600, 3200 };
00026: 
00027: 	private static int GetToday()
00028: 	{
00029: 		try
00030: 		{
00031: 			return StoreStation.Instance?.dayCounter ?? (-1);
00032: 		}
00033: 		catch
00034: 		{
00035: 			return -1;
00036: 		}
00037: 	}
00038: 
00039: 	public static void SettleIfNeeded()
00040: 	{
00041: 		int today = GetToday();
00042: 		if (today < 0)
00043: 		{
00044: 			ModCtx.Log("结算跳过：取不到天数");
00045: 			return;
00046: 		}
00047: 		if (ModHelper.GetModDataInt("lewd_meme", "last_settled_day", -1) == today)
00048: 		{
00049: 			ModCtx.Log("结算跳过：第 " + today + " 天已经结算过");
00050: 			return;
00051: 		}
00052: 		SettleAll();
00053: 		ModHelper.SetModDataInt("lewd_meme", "last_settled_day", today);
00054: 		ModCtx.Log("第 " + today + " 天结算完成");
00055: 	}
00056: 
00057: 	public static void SettleAll()
00058: 	{
00059: 		ModCtx.Log("【触手本体结算】第 " + GetToday() + " 天, 已登记本体=" + Instances.Count);
00060: 		if (Instances.Count == 0)
00061: 		{
00062: 			return;
00063: 		}
00064: 		ReportTypes.Clear();
00065: 		ReportMilk = 0;
00066: 		ReportSemen = 0;
00067: 		ReportMaids = 0;
00068: 		int num = 0;
00069: 		GameItem[] array = Instances.ToArray();
00070: 		foreach (GameItem val in array)
00071: 		{
00072: 			if (val == null)
00073: 			{
00074: 				Instances.Remove(val);
00075: 				continue;
00076: 			}
00077: 			List<GameGridInventory> list = ResolveGrids(val);
00078: 			if (list == null || list.Count < 2)
00079: 			{
00080: 				continue;
00081: 			}
00082: 			GameGridInventory output = list[list.Count - 1];
00083: 			for (int j = 0; j < list.Count - 1; j++)
00084: 			{
00085: 				try
00086: 				{
00087: 					NightlyTick(val, list[j], output);
00088: 				}
00089: 				catch (Exception ex)
00090: 				{
00091: 					ModCtx.LogWarning("兜底结算失败: " + ex.Message);
00092: 				}
00093: 				if (((GameInventory)list[j]).childItems != null)
00094: 				{
00095: 					num += ((GameInventory)list[j]).childItems.Count;
00096: 				}
00097: 			}
00098: 		}
00099: 		if (num > 0)
00100: 		{
00101: 			AddBodyExp(num);
00102: 		}
00103: 		ShowReport();
00104: 	}
00105: 
00106: 	private static void CollectOutputs()
00107: 	{
00108: 		int num = 0;
00109: 		GameItem[] array = Instances.ToArray();
00110: 		foreach (GameItem val in array)
00111: 		{
00112: 			if (val == null)
00113: 			{
00114: 				continue;
00115: 			}
00116: 			try
00117: 			{
00118: 				List<GameGridInventory> list = ResolveGrids(val);
00119: 				if (list == null || list.Count < 2)
00120: 				{
00121: 					continue;
00122: 				}
00123: 				GameGridInventory val2 = list[list.Count - 1];
00124: 				if (val2 == null || ((GameInventory)val2).childItems == null || ((GameInventory)val2).childItems.Count == 0)
00125: 				{
00126: 					continue;
00127: 				}
00128: 				GameItem[] array2 = ((GameInventory)val2).childItems.ToArray();
00129: 				foreach (GameItem val3 in array2)
00130: 				{
00131: 					if (val3 != null)
00132: 					{
00133: 						ModHelper.MoveItemToInventory(val3);
00134: 						num++;
00135: 					}
00136: 				}
00137: 			}
00138: 			catch (Exception ex)
00139: 			{
00140: 				ModCtx.LogWarning("收取产出失败: " + ex.Message);
00141: 			}
00142: 		}
00143: 		if (num > 0)
00144: 		{
00145: 			ModCtx.Log("产出已收入背包: " + num + " 件");
00146: 		}
00147: 	}
00148: 
00149: 	private static void ShowReport()
00150: 	{
00151: 		if (ReportTypes.Count == 0 && ReportMilk == 0 && ReportSemen == 0)
00152: 		{
00153: 			return;
00154: 		}
00155: 		StringBuilder stringBuilder = new StringBuilder();
00156: 		if (ReportTypes.Count > 0)
00157: 		{
00158: 			int num = 0;
00159: 			foreach (KeyValuePair<string, int> reportType in ReportTypes)
00160: 			{
00161: 				if (num > 0)
00162: 				{
00163: 					stringBuilder.Append("和");
00164: 				}
00165: 				stringBuilder.Append(reportType.Value).Append("个【").Append(reportType.Key)
00166: 					.Append("】");
00167: 				num++;
00168: 			}
00169: 			stringBuilder.Append("在【触手本体】内被你的触手调教了一夜");
00170: 		}
00171: 		else
00172: 		{
00173: 			stringBuilder.Append("触手本体蠕动了整夜");
00174: 		}
00175: 		List<string> list = new List<string>();
00176: 		if (ReportMilk > 0)
00177: 		{
00178: 			list.Add("【神奇乳汁】*" + ReportMilk);
00179: 		}
00180: 		if (ReportSemen > 0)
00181: 		{
00182: 			list.Add("【触手精液】*" + ReportSemen);
00183: 		}
00184: 		if (list.Count > 0)
00185: 		{
00186: 			stringBuilder.Append("，产出了 ").Append(string.Join("，", list.ToArray()));
00187: 		}
00188: 		else
00189: 		{
00190: 			stringBuilder.Append("，什么也没产出");
00191: 		}
00192: 		string text = stringBuilder.ToString();
00193: 		ModCtx.Log("夜间报告: " + text);
00194: 		try
00195: 		{
00196: 			string text2 = ((list.Count <= 0) ? ("第 " + GetToday() + " 天：调教了 " + ReportMaids + " 个苗床，无产出") : ("第 " + GetToday() + " 天：调教了 " + ReportMaids + " 个苗床，产出 " + string.Join("、", list.ToArray())));
00197: 			ModHelper.SetModData("lewd_meme", "last_report", text2);
00198: 		}
00199: 		catch
00200: 		{
00201: 		}
00202: 		try
00203: 		{
00204: 			if ((Object)(object)ModHook.Instance != (Object)null)
00205: 			{
00206: 				((MonoBehaviour)ModHook.Instance).StartCoroutine(DelayReport(text));
00207: 				ModCtx.Log("夜间报告: 已排队弹窗");
00208: 			}
00209: 			else
00210: 			{
00211: 				ModCtx.LogWarning("夜间报告: ModHook.Instance 为 null，无法弹窗");
00212: 			}
00213: 		}
00214: 		catch (Exception ex)
00215: 		{
00216: 			ModCtx.LogWarning("夜间报告弹窗失败: " + ex.Message);
00217: 		}
00218: 	}
00219: 
00220: 	private static IEnumerator DelayReport(string text)
00221: 	{
00222: 		yield return (object)new WaitForSeconds(1.2f);
00223: 		try
00224: 		{
00225: 			if ((Object)(object)CustomUIManager.Instance == (Object)null)
00226: 			{
00227: 				ModCtx.LogWarning("夜间报告: CustomUIManager.Instance 为 null");
00228: 				yield break;
00229: 			}
00230: 			CustomUIManager.Window("lewd_meme.nightreport", "夜间报告", "overlay").SetSize(480f, 240f).Center()
00231: 				.SetDraggable(true)
00232: 				.SetCloseOnEscape(true)
00233: 				.AddLabel(text, (string)null)
00234: 				.AddSpace(8f)
00235: 				.AddButton("知道了", (Action)delegate
00236: 				{
00237: 					CustomUIManager.Instance.CloseWindow("lewd_meme.nightreport");
00238: 				}, (string)null)
00239: 				.Show();
00240: 			ModCtx.Log("夜间报告: 窗口已弹出");
00241: 		}
00242: 		catch (Exception ex)
00243: 		{
00244: 			ModCtx.LogWarning("夜间报告弹窗异常: " + ex.Message);
00245: 		}
00246: 	}
00247: 
00248: 	public static GameItem Create()
00249: 	{
00250: 		List<GameGridInventory> grids;
00251: 		PixelWindow contentWindow = BuildWindow(GetBodyLevel(), out grids);
00252: 		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_body");
00253: 		val.SetContentWindow(contentWindow);
00254: 		val.name = "触手本体";
00255: 		val.shortDescription = "这是什么？这是你的本体！可爱又能干的触手怪！让可爱的少女进入其中让她幸福。将这个太空站改造成为你的个人苗床巢吧。";
00256: 		val.flavorText = "";
00257: 		val.unitValue = 0L;
00258: 		val.SetGameItemType("TOOL");
00259: 		val.EnableTag("STANDARD_MACHINE_TAG", false);
00260: 		val.EnableTag("TENTACLE_BODY_TAG", false);
00261: 		val.EnableTag("IMPORTANT_TAG", false);
00262: 		val.EnableTag("NOT_FOR_RESALE", false);
00263: 		val.EnableTag("ITEM_HIDDEN_TAG", false);
00264: 		string text = SpriteInjector.InjectModTexture("tentacle_body", 48, 48);
00265: 		if (text != null)
00266: 		{
00267: 			val.SetSpriteAndShapeFromMod("lewd_meme", text);
00268: 		}
00269: 		else
00270: 		{
00271: 			val.SetSpriteAndShapeFromMod("lewd_meme", "tentacle_body");
00272: 		}
00273: 		val.SetShape(ItemFactory.RectShape(3, 3));
00274: 		((GameItemFunc)val).onCycleEndSlotItemFunc = delegate
00275: 		{
00276: 			SettleIfNeeded();
00277: 		};
00278: 		((GameItemFunc)val).onLoaded = delegate(GameItem it)
00279: 		{
00280: 			if (!Instances.Contains(it))
00281: 			{
00282: 				Instances.Add(it);
00283: 			}
00284: 		};
00285: 		if (!Instances.Contains(val))
00286: 		{
00287: 			Instances.Add(val);
00288: 		}
00289: 		Slots[val] = grids;
00290: 		try
00291: 		{
00292: 			ModCtx.Log($"触手本体 shape = {((val.shape != null) ? val.shape.width : (-1))}x{((val.shape != null) ? val.shape.height : (-1))} (global {((val.shape != null) ? val.shape.globalWidth : (-1))}x{((val.shape != null) ? val.shape.globalHeight : (-1))})");
00293: 		}
00294: 		catch
00295: 		{
00296: 		}
00297: 		return val;
00298: 	}
00299: 
00300: 	private static PixelWindow BuildWindow(int level, out List<GameGridInventory> grids)
00301: 	{
00302: 		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
00303: 		//IL_001b: Expected O, but got Unknown
00304: 		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
00305: 		//IL_008e: Expected O, but got Unknown
00306: 		grids = new List<GameGridInventory>();
00307: 		List<MachineryPanel> list = new List<MachineryPanel>();
00308: 		for (int i = 0; i < level; i++)
00309: 		{
00310: 			GameGridInventory val = new GameGridInventory(12, 12);
00311: 			((GameInventoryFunc)val).mayInventoryAddItemFunc = delegate(GameItem it, GameInventory inv)
00312: 			{
00313: 				if (it == null)
00314: 				{
00315: 					return false;
00316: 				}
00317: 				if (GeneralHelper.IsItemOwned(it))
00318: 				{
00319: 					int num;
00320: 					if (!(it.identifier == "maiden_lower"))
00321: 					{
00322: 						num = (it.IsTag("MAIDEN_FEMALE") ? 1 : 0);
00323: 						if (num == 0)
00324: 						{
00325: 							ModCtx.Log("苗床区拒绝: " + it.identifier);
00326: 						}
00327: 					}
00328: 					else
00329: 					{
00330: 						num = 1;
00331: 					}
00332: 					return (byte)num != 0;
00333: 				}
00334: 				return false;
00335: 			};
00336: 			grids.Add(val);
00337: 			list.Add(MachineHelper.Panel("苗床区 " + (i + 1), (PixelElement)(object)val, "input" + i, true));
00338: 		}
00339: 		GameGridInventory val2 = new GameGridInventory(12, 12);
00340: 		((GameInventoryFunc)val2).mayInventoryAddItemFunc = (GameItem it, GameInventory inv) => it != null && (it.identifier == "wondrous_milk" || it.identifier == "tentacle_semen");
00341: 		grids.Add(val2);
00342: 		list.Add(MachineHelper.Panel("产出区", (PixelElement)(object)val2, "output", true));
00343: 		return MachineHelper.CreateMachineryWindow(true, list.ToArray());
00344: 	}
00345: 
00346: 	public static List<GameGridInventory> ResolveGrids(GameItem machine)
00347: 	{
00348: 		if (machine == null)
00349: 		{
00350: 			return null;
00351: 		}
00352: 		if (Slots.TryGetValue(machine, out var value) && value != null && value.Count >= 2)
00353: 		{
00354: 			return value;
00355: 		}
00356: 		List<GameGridInventory> list = new List<GameGridInventory>();
00357: 		try
00358: 		{
00359: 			for (int i = 0; i < 32; i++)
00360: 			{
00361: 				GraphNodeStorage bodyStorage = MachineHelper.GetBodyStorage(machine, i);
~~~

~~~csharp
00697: 				}
00698: 			}
00699: 		}
00700: 		return null;
00701: 	}
00702: 
00703: 	public static int GetExpNeed()
00704: 	{
00705: 		int bodyLevel = GetBodyLevel();
00706: 		if (bodyLevel - 1 >= EXP_NEED.Length)
00707: 		{
00708: 			return 0;
00709: 		}
00710: 		return EXP_NEED[bodyLevel - 1];
00711: 	}
00712: 
00713: 	public static int GetBodyLevel()
00714: 	{
00715: 		return Mathf.Clamp(ModHelper.GetModDataInt("lewd_meme", "body_lv", 1), 1, 99);
00716: 	}
00717: 
00718: 	public static int GetBodyExp()
00719: 	{
00720: 		return ModHelper.GetModDataInt("lewd_meme", "body_exp", 0);
00721: 	}
00722: 
00723: 	private static void AddBodyExp(int amount)
00724: 	{
00725: 		int num = GetBodyLevel();
00726: 		int num2 = GetBodyExp() + amount;
00727: 		int num3 = num;
00728: 		while (num - 1 < EXP_NEED.Length && num2 >= EXP_NEED[num - 1])
00729: 		{
00730: 			num2 -= EXP_NEED[num - 1];
00731: 			num++;
00732: 			ModCtx.Log("触手本体升级到 Lv." + num + "（苗床容量与性爱强度提升）");
00733: 		}
00734: 		if (num > num3)
00735: 		{
00736: 			RebuildAllWindows();
00737: 		}
00738: 		ModHelper.SetModDataInt("lewd_meme", "body_lv", num);
00739: 		ModHelper.SetModDataInt("lewd_meme", "body_exp", num2);
00740: 	}
00741: 
00742: 	private static bool TryProduce(GameGridInventory output, string itemId)
00743: 	{
00744: 		if (output == null)
00745: 		{
00746: 			return false;
00747: 		}
00748: 		try
00749: 		{
00750: 			GameItem val = DirectoryMaster.Item(itemId, true);
00751: 			if (val == null)
00752: 			{
00753: 				return false;
00754: 			}
00755: 			bool num = GraphUtils.CanAccept((GraphNodeStorage)(object)output, val);
00756: 			val.Destroy();
00757: 			if (!num)
00758: 			{
00759: 				return false;
00760: 			}
00761: 			GraphUtils.TryAcceptAll((GraphNodeStorage)(object)output, DirectoryMaster.Item(itemId, true), -1);
00762: 			return true;
00763: 		}
00764: 		catch (Exception ex)
00765: 		{
00766: 			ModCtx.LogWarning("产出 " + itemId + " 失败: " + ex.Message);
00767: 			return false;
00768: 		}
00769: 	}
00770: }
~~~

## auction

### XIAOWONightAuction/AuctionPhoneNativeNamePatch.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace XIAOWONightAuction;
00004: 
00005: [HarmonyPatch(typeof(LocHelper), "GetLocalizedClientName")]
00006: internal static class AuctionPhoneNativeNamePatch
00007: {
00008: 	private static bool Prefix(string key, ref string __result)
00009: 	{
00010: 		if (key != "xiaowo_night_auction_phone")
00011: 		{
00012: 			return true;
00013: 		}
00014: 		__result = AuctionPhone.ContactName;
00015: 		return false;
00016: 	}
00017: }
~~~

### XIAOWONightAuction/AuctionEngine.cs

~~~csharp
00099: 		{
00100: 			list.Add(i);
00101: 		}
00102: 		List<string> list2 = new List<string>();
00103: 		foreach (string id in lot.InspectionItems)
00104: 		{
00105: 			if (list2.Count == 4)
00106: 			{
00107: 				break;
00108: 			}
00109: 			int num = list.FindIndex((int position) => lot.Contents[position] == id);
00110: 			if (num >= 0)
00111: 			{
00112: 				list2.Add(id);
00113: 				list.RemoveAt(num);
00114: 			}
00115: 		}
00116: 		Random random = new Random(lot.Seed ^ 0x31D5A72);
00117: 		while (list2.Count < 4 && list.Count > 0)
00118: 		{
00119: 			int index = random.Next(list.Count);
00120: 			list2.Add(lot.Contents[list[index]]);
00121: 			list.RemoveAt(index);
00122: 		}
00123: 		return list2.ToArray();
00124: 	}
00125: 
00126: 	internal static string PlaceBid(AuctionLot lot, int amount, int cash)
00127: 	{
00128: 		if (lot.Finished || lot.Lost)
00129: 		{
00130: 			return "这场竞拍已经结束。";
00131: 		}
00132: 		if (amount < 0)
00133: 		{
00134: 			return AuctionText.Select("报价不能为负数。", "A bid cannot be negative.");
00135: 		}
00136: 		if (amount > cash)
00137: 		{
00138: 			return "现金不足，无法提交报价。";
00139: 		}
00140: 		return Resolve(lot, amount);
00141: 	}
00142: 
00143: 	internal static string Pass(AuctionLot lot, int cash = int.MaxValue)
00144: 	{
00145: 		if (lot.Finished || lot.Lost)
00146: 		{
00147: 			return "这场竞拍已经结束。";
00148: 		}
00149: 		return Resolve(lot, (lot.PlayerOffer <= cash) ? lot.PlayerOffer : 0);
00150: 	}
00151: 
00152: 	internal static string StageBid(AuctionLot lot, int amount, int cash)
00153: 	{
00154: 		if (lot.Finished || lot.Lost)
00155: 		{
00156: 			return "这场竞拍已经结束。";
00157: 		}
00158: 		if (lot.PendingOffer >= 0)
00159: 		{
00160: 			return "本轮已经提交报价。";
00161: 		}
00162: 		if (amount < 0)
00163: 		{
00164: 			return AuctionText.Select("报价不能为负数。", "A bid cannot be negative.");
00165: 		}
00166: 		if (amount > cash)
00167: 		{
00168: 			return "现金不足，无法提交报价。";
00169: 		}
00170: 		return StageOffer(lot, amount);
00171: 	}
00172: 
00173: 	internal static string StagePass(AuctionLot lot, int cash)
00174: 	{
00175: 		if (lot.Finished || lot.Lost)
00176: 		{
00177: 			return "这场竞拍已经结束。";
00178: 		}
00179: 		if (lot.PendingOffer >= 0)
00180: 		{
00181: 			return "本轮已经提交报价。";
00182: 		}
00183: 		return StageOffer(lot, (lot.PlayerOffer <= cash) ? lot.PlayerOffer : 0);
00184: 	}
00185: 
00186: 	internal static bool Withdraw(AuctionLot lot)
00187: 	{
00188: 		if (lot == null || !lot.Started || lot.Finished || lot.Lost || lot.PendingOffer >= 0)
00189: 		{
00190: 			return false;
00191: 		}
00192: 		lot.Withdrawn = true;
00193: 		lot.PlayerLeading = false;
00194: 		lot.PlayerOffer = 0;
00195: 		if (lot.HighestBidder == 3)
00196: 		{
00197: 			lot.HighestBidder = -1;
00198: 		}
00199: 		lot.BidLog.Add("拍卖师：你已退出本箱竞拍，本箱计入今晚的竞拍场次。未扣除箱款。");
00200: 		do
00201: 		{
00202: 			Resolve(lot, 0, announce: false);
00203: 		}
00204: 		while (!lot.Finished && lot.Round < 5);
00205: 		lot.BidLog.Add("拍卖师：AI已完成剩余竞价，省略中间播报。");
00206: 		lot.BidLog.Add(SettlementLine(lot));
00207: 		TrimLog(lot);
00208: 		return true;
00209: 	}
00210: 
00211: 	private static string StageOffer(AuctionLot lot, int amount)
00212: 	{
00213: 		PrepareRivalIntel(lot, lot.Round + 1, amount, announce: true);
00214: 		lot.PendingOffer = amount;
00215: 		lot.BiddersShown = 0;
00216: 		lot.BidLog.Add("第" + (lot.Round + 1) + "轮 · 你" + ((amount > 0) ? ("报价 " + amount + "¢") : "未报价"));
00217: 		TrimLog(lot);
00218: 		return "已提交";
00219: 	}
00220: 
00221: 	internal static string RevealBidder(AuctionLot lot)
00222: 	{
00223: 		if (lot.PendingOffer < 0 || lot.BiddersShown >= 3)
00224: 		{
00225: 			return "当前没有待播报的报价。";
00226: 		}
00227: 		int num = lot.BiddersShown++;
00228: 		BidPlan bidPlan = PlanRound(lot, lot.Round + 1, lot.PendingOffer);
00229: 		int num2 = bidPlan.Offers[num];
00230: 		string text = Names[num] + "：" + ((bidPlan.Exits[num] != 0) ? ExitLogText(bidPlan.Exits[num]) : (lot.Sealed ? "暗标已提交" : ((num2 > 0) ? (bidPlan.Actions[num] + " " + num2 + "¢") : "本轮未报价")));
00231: 		lot.BidLog.Add("第" + (lot.Round + 1) + "轮 · " + text);
00232: 		TrimLog(lot);
00233: 		return text;
~~~

~~~csharp
00563: 				lot.RivalExitReasons[j] = 0;
00564: 				lot.ActiveMask |= 1 << j;
00565: 			}
00566: 		}
00567: 		if (lot.PendingOffer >= 0)
00568: 		{
00569: 			lot.BiddersShown = 0;
00570: 		}
00571: 	}
00572: 
00573: 	internal static int ExitReason(AuctionLot lot, int rival)
00574: 	{
00575: 		if (lot.PendingOffer < 0 || lot.BiddersShown <= rival)
00576: 		{
00577: 			return lot.RivalExitReasons[rival];
00578: 		}
00579: 		return PlanRound(lot, lot.Round + 1, lot.PendingOffer).Exits[rival];
00580: 	}
00581: 
00582: 	private static string ExitLogText(int reason)
00583: 	{
00584: 		return (AuctionRivalExit)reason switch
00585: 		{
00586: 			AuctionRivalExit.Budget => "退出：资金不足", 
00587: 			AuctionRivalExit.Price => "退出：超过心理价", 
00588: 			AuctionRivalExit.Bluff => "退出：抬价后收手", 
00589: 			AuctionRivalExit.Risk => "退出：不再冒险", 
00590: 			_ => "本轮未报价", 
00591: 		};
00592: 	}
00593: 
00594: 	internal static string ExitText(int reason)
00595: 	{
00596: 		return (AuctionRivalExit)reason switch
00597: 		{
00598: 			AuctionRivalExit.Budget => AuctionText.Select("退出：资金不足", "Out: no funds"), 
00599: 			AuctionRivalExit.Price => AuctionText.Select("退出：超过心理价", "Out: too costly"), 
00600: 			AuctionRivalExit.Bluff => AuctionText.Select("退出：抬价后收手", "Out: bluff done"), 
00601: 			AuctionRivalExit.Risk => AuctionText.Select("退出：不再冒险", "Out: too risky"), 
00602: 			_ => AuctionText.Select("本轮未报价", "No bid this round"), 
00603: 		};
00604: 	}
00605: 
00606: 	internal static string RivalMood(AuctionLot lot, int rival)
00607: 	{
00608: 		AuctionRivalExit auctionRivalExit = (AuctionRivalExit)ExitReason(lot, rival);
00609: 		if (lot.Finished && lot.HighestBidder == rival)
00610: 		{
00611: 			int num = Appraisal(lot, rival);
00612: 			if (lot.CurrentBid <= (long)num * 110L / 100)
00613: 			{
00614: 				if (lot.CurrentBid > (long)num * 75L / 100)
00615: 				{
00616: 					return AuctionText.Select("得标满意", "Pleased winner");
00617: 				}
00618: 				return AuctionText.Select("捡漏满意", "Bargain joy");
00619: 			}
00620: 			return AuctionText.Select("买贵懊恼", "Overpaid");
00621: 		}
00622: 		switch (auctionRivalExit)
00623: 		{
00624: 		default:
00625: 			return AuctionText.Select("冷静收手", "Walked away");
00626: 		case AuctionRivalExit.Budget:
00627: 			return AuctionText.Select("无奈退出", "Out of funds");
00628: 		case AuctionRivalExit.Bluff:
00629: 			return AuctionText.Select("得意收手", "Bluff complete");
00630: 		case AuctionRivalExit.None:
00631: 			if (lot.Finished)
00632: 			{
00633: 				return AuctionText.Select("落标不甘", "Disappointed");
00634: 			}
00635: 			if (lot.LastOffers[rival] > 0 && lot.LastOffers[rival] >= (long)SpendingLimit(lot, rival, rival == 2) * 90L / 100)
00636: 			{
00637: 				return AuctionText.Select("预算吃紧", "Near my limit");
00638: 			}
00639: 			if (lot.HighestBidder == rival && lot.Round > 0)
00640: 			{
00641: 				return AuctionText.Select("守价观望", "Holding the lead");
00642: 			}
00643: 			if (rival == 2 && lot.Round > 0)
00644: 			{
00645: 				return AuctionText.Select("试探抬价", "Testing a bluff");
00646: 			}
00647: 			if (rival == 1 && lot.Round > 0)
00648: 			{
00649: 				return AuctionText.Select("兴奋跟价", "Eager to bid");
00650: 			}
00651: 			if (lot.MemoryModifiers[rival] < 8)
00652: 			{
00653: 				if (lot.MemoryModifiers[rival] > -3)
00654: 				{
00655: 					return AuctionText.Select("平常心", "Steady");
00656: 				}
00657: 				return AuctionText.Select("谨慎观望", "Cautious");
00658: 			}
00659: 			return AuctionText.Select("势在必得", "Determined");
00660: 		}
00661: 	}
00662: 
00663: 	internal static string BidTable(AuctionLot lot, int playerOffer = -1)
00664: 	{
00665: 		int value = ((playerOffer >= 0) ? playerOffer : lot.PlayerOffer);
00666: 		return "你 " + Offer(value) + " / " + Names[0] + " " + Offer(lot.LastOffers[0]) + " / " + Names[1] + " " + Offer(lot.LastOffers[1]) + " / " + Names[2] + " " + Offer(lot.LastOffers[2]);
00667: 	}
00668: 
00669: 	private static string Offer(int value)
00670: 	{
00671: 		if (value <= 0)
00672: 		{
00673: 			return "未报价";
00674: 		}
00675: 		return value + "¢";
00676: 	}
00677: 
00678: 	internal static string LeadRule(AuctionLot lot)
00679: 	{
00680: 		if (lot.Round < 4)
00681: 		{
00682: 			return AuctionText.Select("需领先第二名 ", "Lead runner-up by ") + (double)EarlyLeadTenths[lot.Round] / 10.0 + AuctionText.Select(" 倍", "x");
00683: 		}
00684: 		return AuctionText.Select("最终轮最高价得标", "Highest bid wins final round");
00685: 	}
00686: 
00687: 	internal static string SurfaceClue(AuctionLot lot)
00688: 	{
00689: 		return new Random(lot.Seed ^ 0x295A1B).Next(3) switch
00690: 		{
00691: 			0 => AuctionText.Select("封条完整", "Intact seal"), 
00692: 			1 => AuctionText.Select("外壳磨损", "Worn casing"), 
00693: 			_ => AuctionText.Select("有修补痕迹", "Repaired casing"), 
00694: 		};
00695: 	}
00696: 
00697: 	internal static string FundsHint(AuctionLot lot, int index)
00698: 	{
00699: 		int num = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
00700: 		if (lot.Budgets[index] >= (long)num * 90L / 100)
00701: 		{
00702: 			if (lot.Budgets[index] < (long)num * 140L / 100)
00703: 			{
00704: 				return AuctionText.Select("资金一般", "Moderate funds");
00705: 			}
00706: 			return AuctionText.Select("资金充裕", "Well funded");
00707: 		}
00708: 		return AuctionText.Select("资金有限", "Limited funds");
00709: 	}
00710: 
00711: 	internal static (int Min, int Max, bool Anomaly) ValueRange(AuctionLot lot, int tier)
00712: 	{
00713: 		tier = Math.Clamp(tier, 0, 2);
00714: 		Random random = new Random(lot.Seed ^ ((tier + 1) * 17873));
00715: 		int num = Math.Max(100, (lot.ReferenceValue > 0) ? lot.ReferenceValue : lot.OpeningBid);
00716: 		int num2 = Math.Max(100, lot.EstimatedValue);
00717: 		if (tier == 2)
00718: 		{
00719: 			int num3 = ((num2 >= (long)num * 4L) ? 70 : ((num2 >= (long)num * 2L) ? 12 : 3));
00720: 			if (random.Next(100) < num3)
00721: 			{
00722: 				return (Min: 0, Max: 0, Anomaly: true);
00723: 			}
00724: 		}
00725: 		long num4 = tier switch
00726: 		{
00727: 			0 => (long)num * (long)random.Next(75, 136) / 100, 
00728: 			1 => (long)num2 * (long)random.Next(78, 123) / 100, 
00729: 			_ => (long)num2 * (long)random.Next(91, 110) / 100, 
00730: 		};
00731: 		long num5 = num;
00732: 		long num6 = num5 * tier switch
00733: 		{
00734: 			0 => random.Next(60, 111), 
00735: 			1 => random.Next(28, 60), 
00736: 			_ => random.Next(12, 27), 
00737: 		} / 100;
00738: 		int num7 = ((lot.Venue == AuctionVenue.Advanced) ? 500 : ((lot.Venue == AuctionVenue.Intermediate) ? 100 : 50));
00739: 		long num8 = Math.Max(num7, (num4 - num6 / 2) / num7 * num7);
00740: 		long val = Math.Max(num8 + num7, (num4 + (num6 + 1) / 2 + num7 - 1) / num7 * num7);
00741: 		return (Min: (int)Math.Min(2147483647L, num8), Max: (int)Math.Min(2147483647L, val), Anomaly: false);
00742: 	}
00743: 
00744: 	private static void TrimLog(AuctionLot lot)
00745: 	{
00746: 		if (lot.BidLog.Count > 100)
00747: 		{
00748: 			lot.BidLog.RemoveRange(0, lot.BidLog.Count - 100);
00749: 		}
00750: 	}
00751: }
~~~

### XIAOWONightAuction/AuctionVisitSchedule.cs

~~~csharp
00028: 				Slot = slot,
00029: 				Category = category,
00030: 				DueDay = dueDay
00031: 			});
00032: 		}
00033: 	}
00034: 
00035: 	internal void DelayDue(int day)
00036: 	{
00037: 		foreach (Visit item in Pending)
00038: 		{
00039: 			if (item.DueDay <= day)
00040: 			{
00041: 				item.DueDay = checked(day + 1);
00042: 			}
00043: 		}
00044: 	}
00045: 
00046: 	internal Visit Next(int day)
00047: 	{
00048: 		return (from v in Pending
00049: 			where v.DueDay <= day
00050: 			orderby v.Week, v.Slot
00051: 			select v).FirstOrDefault();
00052: 	}
00053: 
00054: 	internal void Complete(int week, int slot)
00055: 	{
00056: 		Pending.RemoveAll((Visit v) => v.Week == week && v.Slot == slot);
00057: 	}
00058: 
00059: 	internal string Encode()
00060: 	{
00061: 		return string.Join(";", (from v in Pending
00062: 			orderby v.Week, v.Slot
00063: 			select v).Select(delegate(Visit v)
00064: 		{
00065: 			string[] obj = new string[7]
00066: 			{
00067: 				v.Week.ToString(),
00068: 				",",
00069: 				v.Slot.ToString(),
00070: 				",",
00071: 				null,
00072: 				null,
00073: 				null
00074: 			};
00075: 			int category = (int)v.Category;
00076: 			obj[4] = category.ToString();
00077: 			obj[5] = ",";
00078: 			obj[6] = v.DueDay.ToString();
00079: 			return string.Concat(obj);
00080: 		}));
00081: 	}
00082: 
00083: 	internal static AuctionVisitSchedule Decode(string text)
00084: 	{
00085: 		AuctionVisitSchedule auctionVisitSchedule = new AuctionVisitSchedule();
00086: 		string[] array = (text ?? "").Split(';');
00087: 		for (int i = 0; i < array.Length; i++)
00088: 		{
00089: 			string[] array2 = array[i].Split(',');
00090: 			if (array2.Length == 4 && int.TryParse(array2[0], out var result) && int.TryParse(array2[1], out var result2) && int.TryParse(array2[2], out var result3) && int.TryParse(array2[3], out var result4))
00091: 			{
00092: 				auctionVisitSchedule.Remember(result, result2, (ContainerCategory)result3, result4);
00093: 			}
00094: 		}
00095: 		return auctionVisitSchedule;
00096: 	}
00097: }
~~~

### XIAOWONightAuction/AuctionText.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Text.RegularExpressions;
00004: 
00005: namespace XIAOWONightAuction;
00006: 
00007: internal static class AuctionText
00008: {
00009: 	private static readonly (string Chinese, string English)[] LogPhrases = new(string, string)[64]
00010: 	{
00011: 		("拍卖师：", "Auctioneer: "),
00012: 		("你已退出本箱竞拍，本箱计入今晚的竞拍场次。未扣除箱款。", "You withdrew. This lot counts toward tonight's auctions. No payment was taken."),
00013: 		("AI已完成剩余竞价，省略中间播报。", "AI bidding completed; intermediate announcements skipped."),
00014: 		("退出：资金不足", "Out: insufficient funds"),
00015: 		("退出：超过心理价", "Out: above my limit"),
00016: 		("退出：抬价后收手", "Out: bluff ended"),
00017: 		("退出：不再冒险", "Out: no more risk"),
00018: 		("无人达到起拍价，本箱流拍。", "No bid reached the opening price. Lot unsold."),
00019: 		("无人提交有效报价，本箱流拍。", "No valid bids were submitted. Lot unsold."),
00020: 		("未达到本轮领先条件，进入", "Lead threshold not met. Proceed to "),
00021: 		("竞拍，最多五轮；领先足够即可提前成交。", " auction, up to five rounds. A sufficient lead wins early."),
00022: 		("最多五轮，每轮揭价；领先足够可提前成交。", "Up to five rounds. Bids are revealed each round; a sufficient lead wins early."),
00023: 		("内部线索：确认存在高价值物品。", "Inside tip: high-value goods confirmed."),
00024: 		("内部线索：存在高价值物品的迹象。", "Inside tip: signs of high-value goods."),
00025: 		("内部线索：未发现明显高价值物品。", "Inside tip: no clear sign of high-value goods."),
00026: 		("抽检货物：已揭示 ", "Cargo sample: revealed "),
00027: 		(" 件真实物品，详见抽检窗口。", " actual items. See the sample window."),
00028: 		("复鉴失败：读数超出设备量程，可能存在异常高价值货物。", "Rescan failed: reading beyond range; unusually valuable goods possible."),
00029: 		("搅局者已经接近退出价格。", "The disruptor is close to dropping out."),
00030: 		("搅局者更像在抬价，赌徒意愿更强。", "The disruptor may be raising prices; the gambler is more committed."),
00031: 		("搅局者仍有明显抬价空间。", "The disruptor still has room to raise the price."),
00032: 		("这场竞拍已经结束。", "This auction has ended."),
00033: 		("报价低于最低接受价格。", "Bid is below the minimum accepted price."),
00034: 		("现金不足，无法提交报价。", "Not enough cash to submit this bid."),
00035: 		("本轮已经提交报价。", "You already submitted a bid this round."),
00036: 		("当前没有待播报的报价。", "No bids are pending announcement."),
00037: 		("请先等待三位竞拍者报价。", "Wait for all three rivals to bid."),
00038: 		("已提交", "Submitted"),
00039: 		("现金不足", "Insufficient cash"),
00040: 		("该类别没有可用物品池", "No items available for this category"),
00041: 		("本轮未报价", "No bid this round"),
00042: 		("暗标已提交", "Sealed bid submitted"),
00043: 		("守住报价", "Holding bid"),
00044: 		("小幅跟价", "Small raise"),
00045: 		("试探报价", "Opening probe"),
00046: 		("重新报价", "Revised bid"),
00047: 		("购买估值情报，花费 ", "Bought appraisal intel for "),
00048: 		("观望报价", "Waiting with bid"),
00049: 		("独立估价", "Own valuation"),
00050: 		("争抢报价", "Contesting bid"),
00051: 		("试探抬价", "Testing a bluff"),
00052: 		("购买复鉴道具，花费 ", "Bought a rescan tool for "),
00053: 		("未报价", "No bid"),
00054: 		("提前得标。", " won early."),
00055: 		("在最终轮得标。", " won in the final round."),
00056: 		("稳健客预算约 ", "Cautious budget approx. "),
00057: 		("稳健客心理价约 ", "Cautious spending limit approx. "),
00058: 		("情报估值 ", "Intel value "),
00059: 		("复鉴区间 ", "Rescan range "),
00060: 		("情报 · ", "Intel - "),
00061: 		("情报", "Intel"),
00062: 		("稳健客", "Cautious"),
00063: 		("赌徒", "Gambler"),
00064: 		("搅局者", "Disruptor"),
00065: 		("暗标", "Sealed"),
00066: 		("公开", "Open"),
00067: 		("流拍", "Unsold"),
00068: 		("你报价 ", "You bid "),
00069: 		("你未报价", "You did not bid"),
00070: 		("报价 ", "bid "),
00071: 		("以 ", " bid "),
00072: 		(" 至 ", " to "),
00073: 		("你", "You"),
00074: 		("：", ": ")
00075: 	};
00076: 
~~~

~~~csharp
00094: 		["take_only"] = "9x9 extraction tray - take only, no deposits",
00095: 		["fill_output_hint"] = "Extract until the 9x9 tray is full",
00096: 		["container_empty_short"] = "Container emptied",
00097: 		["fill_output"] = "Fill Extraction Tray",
00098: 		["empty_button"] = "Empty"
00099: 	};
00100: 
00101: 	internal static string CurrentLocaleCode
00102: 	{
00103: 		get
00104: 		{
00105: 			try
00106: 			{
00107: 				return LocHelper.GetCurrentLocaleCode() ?? string.Empty;
00108: 			}
00109: 			catch
00110: 			{
00111: 				return string.Empty;
00112: 			}
00113: 		}
00114: 	}
00115: 
00116: 	internal static bool IsChinese(string localeCode)
00117: 	{
00118: 		if (!string.IsNullOrWhiteSpace(localeCode))
00119: 		{
00120: 			if (!localeCode.Equals("zh", StringComparison.OrdinalIgnoreCase) && !localeCode.StartsWith("zh-", StringComparison.OrdinalIgnoreCase))
00121: 			{
00122: 				return localeCode.StartsWith("zh_", StringComparison.OrdinalIgnoreCase);
00123: 			}
00124: 			return true;
00125: 		}
00126: 		return false;
00127: 	}
00128: 
00129: 	internal static string Select(string chinese, string english)
00130: 	{
00131: 		return Select(CurrentLocaleCode, chinese, english);
00132: 	}
00133: 
00134: 	internal static string Select(string localeCode, string chinese, string english)
00135: 	{
00136: 		if (!IsChinese(localeCode))
00137: 		{
00138: 			return english;
00139: 		}
00140: 		return chinese;
00141: 	}
00142: 
00143: 	internal static string BidderName(int index)
00144: 	{
00145: 		return index switch
00146: 		{
00147: 			0 => Select("稳健客", "Cautious"), 
00148: 			1 => Select("赌徒", "Gambler"), 
00149: 			2 => Select("搅局者", "Disruptor"), 
00150: 			_ => Select("你", "You"), 
00151: 		};
00152: 	}
00153: 
00154: 	internal static string DisplayLog(string value)
00155: 	{
00156: 		if (IsChinese(CurrentLocaleCode) || string.IsNullOrEmpty(value))
00157: 		{
00158: 			return value;
00159: 		}
00160: 		value = Regex.Replace(value, "第(\\d+)轮揭价", "Round $1 bids revealed");
00161: 		value = Regex.Replace(value, "第(\\d+)轮", "Round $1");
00162: 		value = value.Replace(" · ", " - ", StringComparison.Ordinal);
00163: 		value = value.Replace("轮。", ".", StringComparison.Ordinal);
00164: 		(string, string)[] logPhrases = LogPhrases;
00165: 		for (int i = 0; i < logPhrases.Length; i++)
00166: 		{
00167: 			(string, string) tuple = logPhrases[i];
00168: 			string item = tuple.Item1;
00169: 			string item2 = tuple.Item2;
00170: 			value = value.Replace(item, item2, StringComparison.Ordinal);
00171: 		}
00172: 		value = value.Replace("。", ".", StringComparison.Ordinal);
00173: 		return value;
00174: 	}
00175: 
00176: 	internal static string Get(string key, string fallback, params object[] args)
00177: 	{
00178: 		string text = ModHelper.GetLocalized("xiaowo_night_auction", key, Array.Empty<object>());
00179: 		if (text == key)
00180: 		{
00181: 			text = ((IsChinese(CurrentLocaleCode) || !English.TryGetValue(key, out var value)) ? fallback : value);
00182: 		}
00183: 		if (args.Length != 0)
00184: 		{
00185: 			return string.Format(text, args);
00186: 		}
00187: 		return text;
00188: 	}
00189: }
~~~

### XIAOWONightAuction/AuctionRivalFunds.cs

~~~csharp
00053: 		for (int i = 0; i < 3; i++)
00054: 		{
00055: 			auctionRivalFunds.Initial[i] = (auctionRivalFunds.Remaining[i] = random.Next(minValue, num + 1));
00056: 		}
00057: 		return auctionRivalFunds;
00058: 	}
00059: 
00060: 	internal bool UpgradeLowAnchor(int seed)
00061: 	{
00062: 		if (AnchorCash >= BaseFloor(Venue))
00063: 		{
00064: 			return false;
00065: 		}
00066: 		AuctionRivalFunds auctionRivalFunds = Create(Venue, BaseFloor(Venue), seed);
00067: 		for (int i = 0; i < 3; i++)
00068: 		{
00069: 			int num = Math.Max(0, auctionRivalFunds.Initial[i] - Initial[i]);
00070: 			Initial[i] += num;
00071: 			Remaining[i] += num;
00072: 		}
00073: 		AnchorCash = auctionRivalFunds.AnchorCash;
00074: 		return true;
00075: 	}
00076: 
00077: 	internal static string LotKey(AuctionLot lot)
00078: 	{
00079: 		return lot.Seed + ":" + lot.Slot;
00080: 	}
00081: 
00082: 	internal void AdoptLegacy(IEnumerable<AuctionLot> lots)
00083: 	{
00084: 		AuctionLot auctionLot = lots.FirstOrDefault((AuctionLot lot) => lot.Started && !lot.Finished && !lot.Lost);
00085: 		if (auctionLot != null)
00086: 		{
00087: 			Legacy = true;
00088: 			Initial = auctionLot.Budgets.Select((int value) => Math.Max(0, value)).ToArray();
00089: 			Remaining = (int[])Initial.Clone();
00090: 		}
00091: 		foreach (AuctionLot item in lots.Where((AuctionLot lot) => lot.Finished))
00092: 		{
00093: 			Settled.Add(LotKey(item));
00094: 		}
00095: 	}
00096: 
00097: 	internal bool Settle(AuctionLot lot)
00098: 	{
00099: 		if (lot.Venue != Venue || !lot.Started)
00100: 		{
00101: 			return false;
00102: 		}
00103: 		string text = LotKey(lot);
00104: 		bool flag = false;
00105: 		if (!IntelPayments.TryGetValue(text, out var value))
00106: 		{
00107: 			value = new int[3];
00108: 		}
00109: 		for (int i = 0; i < 3; i++)
00110: 		{
00111: 			int num = lot.RivalIntelSpent[i] - value[i];
00112: 			if (num > 0)
00113: 			{
00114: 				if (num > Remaining[i])
00115: 				{
00116: 					throw new InvalidOperationException("Rival intelligence exceeds wallet.");
00117: 				}
00118: 				Remaining[i] -= num;
00119: 				value[i] += num;
00120: 				flag = true;
00121: 			}
00122: 		}
00123: 		if (flag)
00124: 		{
00125: 			IntelPayments[text] = value;
00126: 		}
00127: 		if (!lot.Finished || !Settled.Add(text))
00128: 		{
00129: 			return flag;
00130: 		}
00131: 		int highestBidder = lot.HighestBidder;
00132: 		if (highestBidder >= 0 && highestBidder < 3 && lot.CurrentBid > 0)
00133: 		{
00134: 			Remaining[lot.HighestBidder] = (int)Math.Max(0L, (long)Remaining[lot.HighestBidder] - (long)lot.CurrentBid);
00135: 		}
00136: 		return true;
00137: 	}
00138: 
00139: 	internal string Encode()
00140: 	{
00141: 		return string.Join("|", "1", (int)Venue, AnchorCash, string.Join(":", Initial), string.Join(":", Remaining), Legacy ? "1" : "0", string.Join(",", Settled.OrderBy<string, string>((string key) => key, StringComparer.Ordinal)), string.Join(",", from pair in IntelPayments.OrderBy<KeyValuePair<string, int[]>, string>((KeyValuePair<string, int[]> pair) => pair.Key, StringComparer.Ordinal)
00142: 			select pair.Key + "~" + string.Join(":", pair.Value)));
00143: 	}
00144: 
00145: 	internal static AuctionRivalFunds Decode(string encoded)
00146: 	{
00147: 		if (string.IsNullOrEmpty(encoded))
00148: 		{
00149: 			return null;
00150: 		}
00151: 		try
00152: 		{
00153: 			string[] array = encoded.Split('|');
00154: 			if ((array.Length != 7 && array.Length != 8) || array[0] != "1" || (array[5] != "0" && array[5] != "1"))
00155: 			{
00156: 				return null;
00157: 			}
00158: 			int num = int.Parse(array[1]);
00159: 			if (num < 0 || num > 2)
00160: 			{
00161: 				return null;
00162: 			}
00163: 			AuctionRivalFunds auctionRivalFunds = new AuctionRivalFunds
00164: 			{
00165: 				Venue = (AuctionVenue)num,
00166: 				AnchorCash = int.Parse(array[2]),
00167: 				Initial = array[3].Split(':').Select(int.Parse).ToArray(),
00168: 				Remaining = array[4].Split(':').Select(int.Parse).ToArray(),
00169: 				Legacy = (array[5] == "1")
00170: 			};
00171: 			if (auctionRivalFunds.AnchorCash < 0 || auctionRivalFunds.AnchorCash > BaseCap(auctionRivalFunds.Venue) || auctionRivalFunds.Initial.Length != 3 || auctionRivalFunds.Remaining.Length != 3)
00172: 			{
00173: 				return null;
00174: 			}
00175: 			for (int i = 0; i < 3; i++)
00176: 			{
00177: 				if (auctionRivalFunds.Initial[i] < 0 || auctionRivalFunds.Remaining[i] < 0 || auctionRivalFunds.Remaining[i] > auctionRivalFunds.Initial[i] || (!auctionRivalFunds.Legacy && auctionRivalFunds.Initial[i] > BaseCap(auctionRivalFunds.Venue) * 3 / 2))
00178: 				{
00179: 					return null;
00180: 				}
00181: 			}
00182: 			string[] array2 = array[6].Split(',');
00183: 			foreach (string text in array2)
00184: 			{
00185: 				if (!string.IsNullOrEmpty(text))
00186: 				{
00187: 					auctionRivalFunds.Settled.Add(text);
00188: 				}
00189: 			}
00190: 			if (array.Length == 8 && array[7].Length > 0)
00191: 			{
00192: 				array2 = array[7].Split(',');
00193: 				for (int j = 0; j < array2.Length; j++)
00194: 				{
00195: 					string[] array3 = array2[j].Split('~');
00196: 					if (array3.Length != 2 || string.IsNullOrEmpty(array3[0]))
00197: 					{
00198: 						return null;
00199: 					}
00200: 					int[] array4 = array3[1].Split(':').Select(int.Parse).ToArray();
00201: 					if (array4.Length != 3 || array4.Any((int value) => value < 0))
00202: 					{
00203: 						return null;
00204: 					}
00205: 					for (int num2 = 0; num2 < 3; num2++)
00206: 					{
00207: 						if (array4[num2] > auctionRivalFunds.Initial[num2])
00208: 						{
00209: 							return null;
00210: 						}
00211: 					}
00212: 					if (!auctionRivalFunds.IntelPayments.TryAdd(array3[0], array4))
00213: 					{
00214: 						return null;
00215: 					}
00216: 				}
00217: 			}
00218: 			return auctionRivalFunds;
00219: 		}
00220: 		catch (FormatException)
00221: 		{
00222: 			return null;
00223: 		}
00224: 		catch (OverflowException)
00225: 		{
00226: 			return null;
00227: 		}
00228: 	}
00229: }
~~~

### XIAOWONightAuction/AuctionState.cs

~~~csharp
00089: 				string text = keyValuePair.Key.Substring("xiaowo_night_auction.".Length);
00090: 				if (!ModHelper.HasModData("xiaowo_night_auction", text))
00091: 				{
00092: 					ModHelper.SetModData("xiaowo_night_auction", text, keyValuePair.Value);
00093: 				}
00094: 				else if (ModHelper.GetModData("xiaowo_night_auction", text, (string)null) != keyValuePair.Value)
00095: 				{
00096: 					Core.Log?.Warning("[Save migration] Existing official value retained for key " + text + "; legacy value preserved.");
00097: 				}
00098: 			}
00099: 		}
00100: 	}
00101: 
00102: 	internal static bool TryGetVenue(out AuctionVenue venue)
00103: 	{
00104: 		venue = AuctionVenue.Beginner;
00105: 		string[] array = Read("venue_" + AuctionCatalog.Day).Split(',');
00106: 		if (array.Length < 1 || !int.TryParse(array[0], out var result) || result < 0 || result > 2)
00107: 		{
00108: 			return false;
00109: 		}
00110: 		venue = (AuctionVenue)result;
00111: 		return true;
00112: 	}
00113: 
00114: 	internal static bool EnterVenue(AuctionVenue venue, out string error)
00115: 	{
00116: 		AuctionNightMarket.EnsureToday();
00117: 		error = string.Empty;
00118: 		if (TryGetVenue(out var venue2))
00119: 		{
00120: 			if (venue2 == venue)
00121: 			{
00122: 				return true;
00123: 			}
00124: 			error = AuctionText.Select("今晚已经进入“", "Already entered ") + AuctionRules.VenueName(venue2) + AuctionText.Select("”，不能更换场次。", "; you cannot change venues tonight.");
00125: 			return false;
00126: 		}
00127: 		int num = AuctionNightMarket.EntryFee(venue);
00128: 		int cash = PlayerStore.Instance.GetCash();
00129: 		if (cash < num)
00130: 		{
00131: 			error = AuctionText.Select("现金不足，无法支付入场费。", "Not enough cash for the entry fee.");
00132: 			return false;
00133: 		}
00134: 		if (num > 0)
00135: 		{
00136: 			PlayerStore.Instance.ModCash(-num, true);
00137: 		}
00138: 		string suffix = "venue_" + AuctionCatalog.Day;
00139: 		int num2 = (int)venue;
00140: 		Write(suffix, num2 + "," + num);
00141: 		WriteRivalFunds(AuctionRivalFunds.Create(venue, cash, RivalFundsSeed(venue)));
00142: 		return true;
00143: 	}
00144: 
00145: 	private static int RivalFundsSeed(AuctionVenue venue)
00146: 	{
00147: 		string[] obj = new string[5]
00148: 		{
00149: 			PlayerStore.Instance?.runID ?? "slot",
00150: 			":",
00151: 			AuctionCatalog.Day.ToString(),
00152: 			":rival_funds:",
00153: 			null
00154: 		};
00155: 		int num = (int)venue;
00156: 		obj[4] = num.ToString();
00157: 		return StableSeed(string.Concat(obj));
00158: 	}
00159: 
00160: 	private static void WriteRivalFunds(AuctionRivalFunds funds)
00161: 	{
00162: 		Write(RivalFundsKey, funds.Encode());
00163: 	}
00164: 
00165: 	private static AuctionRivalFunds EnsureRivalFunds(AuctionVenue venue, List<AuctionLot> lots)
00166: 	{
00167: 		AuctionRivalFunds auctionRivalFunds = AuctionRivalFunds.Decode(Read(RivalFundsKey));
00168: 		if (auctionRivalFunds != null && auctionRivalFunds.Venue == venue)
00169: 		{
00170: 			if (auctionRivalFunds.UpgradeLowAnchor(RivalFundsSeed(venue)))
00171: 			{
00172: 				foreach (AuctionLot lot in lots)
00173: 				{
00174: 					AuctionEngine.RefreshMigratedFunds(lot, auctionRivalFunds.Remaining);
00175: 				}
00176: 				WriteRivalFunds(auctionRivalFunds);
00177: 			}
00178: 			return auctionRivalFunds;
00179: 		}
00180: 		auctionRivalFunds = AuctionRivalFunds.Create(venue, PlayerStore.Instance.GetCash(), RivalFundsSeed(venue));
00181: 		auctionRivalFunds.AdoptLegacy(lots);
00182: 		WriteRivalFunds(auctionRivalFunds);
00183: 		return auctionRivalFunds;
00184: 	}
00185: 
00186: 	internal static void StartLot(AuctionLot lot, List<AuctionLot> lots)
00187: 	{
00188: 		AuctionRivalFunds auctionRivalFunds = EnsureRivalFunds(lot.Venue, lots);
00189: 		AuctionEngine.Start(lot, auctionRivalFunds.Remaining);
00190: 	}
00191: 
00192: 	internal static int RivalCash(AuctionLot lot, int index)
00193: 	{
00194: 		AuctionRivalFunds auctionRivalFunds = AuctionRivalFunds.Decode(Read(RivalFundsKey));
00195: 		if (auctionRivalFunds == null || auctionRivalFunds.Venue != lot.Venue)
00196: 		{
00197: 			return lot.Budgets[index];
00198: 		}
00199: 		return auctionRivalFunds.Remaining[index];
00200: 	}
~~~

~~~csharp
00335: 			string[] array2 = array[i].Split(',');
00336: 			if (array2.Length >= 9 && array2[0] == AuctionCatalog.Day.ToString() && array2[3] == lot.Seed.ToString())
00337: 			{
00338: 				return;
00339: 			}
00340: 		}
00341: 		Write("history", string.IsNullOrEmpty(text2) ? text : (text2 + ";" + text));
00342: 	}
00343: 
00344: 	internal static void CommitPurchase(AuctionLot lot, List<AuctionLot> lots, List<string> contents)
00345: 	{
00346: 		PlayerStore store = NativeStore.Existing ?? throw new InvalidOperationException("Player save is unavailable.");
00347: 		string text = $"purchase_{AuctionCatalog.Day}_{(int)lot.Venue}_{lot.Slot}_{lot.Seed}";
00348: 		if (!string.IsNullOrEmpty(Read(text)))
00349: 		{
00350: 			lot.Finished = true;
00351: 			lot.PlayerLeading = true;
00352: 			return;
00353: 		}
00354: 		if (lot.CurrentBid < 0 || store.GetCash() < lot.CurrentBid || contents.Count == 0)
00355: 		{
00356: 			throw new InvalidOperationException("Invalid auction purchase.");
00357: 		}
00358: 		PlayerStore val = store;
00359: 		if (val.modData == null)
00360: 		{
00361: 			val.modData = new Dictionary<string, string>();
00362: 		}
00363: 		string value;
00364: 		Dictionary<string, string> dictionary = new string[4]
00365: 		{
00366: 			"pending_deliveries",
00367: 			"lots_v2_" + AuctionCatalog.Day,
00368: 			"history",
00369: 			text
00370: 		}.Select((string k) => "xiaowo_night_auction:" + k).ToArray().ToDictionary((string k) => k, (string k) => (!store.modData.TryGetValue(k, out value)) ? null : value);
00371: 		int cash = store.GetCash();
00372: 		bool finished = lot.Finished;
00373: 		bool playerLeading = lot.PlayerLeading;
00374: 		try
00375: 		{
00376: 			SetPendingDelivery(lot.Category, lot.Seed, contents);
00377: 			lot.PlayerLeading = true;
00378: 			lot.Finished = true;
00379: 			SaveLots(lots);
00380: 			RecordWin(lot);
00381: 			Write(text, lot.CurrentBid.ToString());
00382: 			store.ModCash(-lot.CurrentBid, false);
00383: 		}
00384: 		catch
00385: 		{
00386: 			foreach (KeyValuePair<string, string> item in dictionary)
00387: 			{
00388: 				if (item.Value == null)
00389: 				{
00390: 					store.modData.Remove(item.Key);
00391: 				}
00392: 				else
00393: 				{
00394: 					store.modData[item.Key] = item.Value;
00395: 				}
00396: 			}
00397: 			store.ModCash(cash - store.GetCash(), false);
00398: 			lot.Finished = finished;
00399: 			lot.PlayerLeading = playerLeading;
00400: 			throw;
00401: 		}
00402: 	}
00403: 
00404: 	internal static void RevealHistory(int seed)
00405: 	{
00406: 		List<string> list = Read("history").Split(';').ToList();
00407: 		bool flag = false;
00408: 		for (int i = 0; i < list.Count; i++)
00409: 		{
00410: 			string[] array = list[i].Split(',');
00411: 			if (array.Length == 10 && !(array[3] != seed.ToString()) && !(array[9] == "1"))
00412: 			{
00413: 				array[9] = "1";
00414: 				list[i] = string.Join(",", array);
00415: 				flag = true;
00416: 			}
00417: 		}
00418: 		if (flag)
00419: 		{
00420: 			Write("history", string.Join(";", list));
00421: 		}
00422: 	}
00423: 
00424: 	internal static List<string> History()
00425: 	{
00426: 		List<string> list = new List<string>();
00427: 		string[] array = Read("history").Split(';');
00428: 		for (int i = 0; i < array.Length; i++)
00429: 		{
00430: 			string[] array2 = array[i].Split(',');
00431: 			int result2;
00432: 			int result3;
00433: 			if (array2.Length == 6 && int.TryParse(array2[1], out var result) && result >= 0 && result <= 3)
00434: 			{
00435: 				list.Add(AuctionText.Select("第", "Day ") + array2[0] + AuctionText.Select("天  ", "  ") + AuctionCatalog.CategoryName((ContainerCategory)result) + AuctionText.Select("  成交 ", "  Bid ") + array2[3] + AuctionText.Select("  货值 ", "  Value ") + array2[4] + AuctionText.Select("  盈亏 ", "  Profit ") + array2[5]);
00436: 			}
00437: 			else if ((array2.Length == 9 || array2.Length == 10) && int.TryParse(array2[1], out result2) && result2 >= 0 && result2 <= 2 && int.TryParse(array2[2], out result3) && result3 >= 0 && result3 <= 3)
00438: 			{
00439: 				bool flag = array2.Length == 9 || array2[9] == "1";
00440: 				list.Add(AuctionText.Select("第", "Day ") + array2[0] + AuctionText.Select("天  ", "  ") + AuctionRules.VenueName((AuctionVenue)result2) + "  " + AuctionCatalog.CategoryName((ContainerCategory)result3) + AuctionText.Select("  成交 ", "  Bid ") + array2[4] + (flag ? (AuctionText.Select("  货值 ", "  Value ") + array2[5] + AuctionText.Select("  净收益 ", "  Net ") + array2[6]) : AuctionText.Select("  货值 待揭晓  净收益 待结算", "  Value pending  Net pending")));
00441: 			}
00442: 		}
00443: 		list.Reverse();
00444: 		return list;
00445: 	}
00446: 
00447: 	internal static void MarkPurchased()
00448: 	{
00449: 		Write("purchased_day", AuctionCatalog.Day.ToString());
00450: 	}
00451: 
00452: 	internal static void SetPendingDelivery(ContainerCategory category, int seed, IReadOnlyList<string> contents)
00453: 	{
00454: 		string[] array = new string[5];
00455: 		int num = (int)category;
00456: 		array[0] = num.ToString();
00457: 		array[1] = "~";
00458: 		array[2] = seed.ToString();
00459: 		array[3] = "~";
00460: 		array[4] = string.Join("|", contents);
00461: 		string text = string.Concat(array);
00462: 		string text2 = Read("pending_deliveries");
00463: 		if (!text2.Split(';').Any((string value) => value.Split('~').Length >= 2 && value.Split('~')[1] == seed.ToString()))
00464: 		{
00465: 			Write("pending_deliveries", string.IsNullOrWhiteSpace(text2) ? text : (text2 + ";" + text));
00466: 		}
00467: 	}
00468: 
00469: 	internal static bool TryGetPendingDelivery(out ContainerCategory category, out int seed, out List<string> contents)
00470: 	{
00471: 		category = ContainerCategory.Engineering;
00472: 		seed = 0;
00473: 		contents = new List<string>();
00474: 		string text = Read("pending_deliveries").Split(';').FirstOrDefault((string value) => !string.IsNullOrWhiteSpace(value));
00475: 		if (string.IsNullOrWhiteSpace(text))
00476: 		{
00477: 			text = Read("pending_delivery");
00478: 		}
00479: 		string[] array = text.Split('~');
00480: 		if (array.Length != 3 || !int.TryParse(array[0], out var result) || !int.TryParse(array[1], out seed))
00481: 		{
00482: 			return false;
00483: 		}
00484: 		category = (ContainerCategory)Math.Clamp(result, 0, 3);
00485: 		string[] array2 = array[2].Split('|');
00486: 		foreach (string text2 in array2)
00487: 		{
00488: 			if (!string.IsNullOrWhiteSpace(text2))
00489: 			{
00490: 				contents.Add(text2);
00491: 			}
00492: 		}
00493: 		if (seed != 0)
00494: 		{
00495: 			return contents.Count > 0;
00496: 		}
00497: 		return false;
00498: 	}
00499: 
00500: 	internal static void AcknowledgeDelivery(int seed)
00501: 	{
00502: 		List<string> list = (from value in Read("pending_deliveries").Split(';')
00503: 			where !string.IsNullOrWhiteSpace(value)
00504: 			where value.Split('~').Length < 2 || value.Split('~')[1] != seed.ToString()
00505: 			select value).ToList();
00506: 		if (list.Count > 0)
00507: 		{
00508: 			Write("pending_deliveries", string.Join(";", list));
00509: 		}
~~~

### XIAOWONightAuction/AuctionMerchantRelations.cs

~~~csharp
00054: 				return _pages.TryGetValue(DialogUIManager.Instance.currentDialog, out value);
00055: 			}
00056: 			return false;
00057: 		}
00058: 	}
00059: 
00060: 	private static string Unavailable => T("商人已离开或存档已切换，赠礼未执行。", "The buyer left or the save changed. No gift was given.");
00061: 
00062: 	private static string Key(ContainerCategory category)
00063: 	{
00064: 		int num = (int)category;
00065: 		return "merchant_gifts_" + num;
00066: 	}
00067: 
00068: 	internal static long Gifts(ContainerCategory category)
00069: 	{
00070: 		if (!long.TryParse(AuctionState.Read(Key(category)), out var result))
00071: 		{
00072: 			return 0L;
00073: 		}
00074: 		return Math.Max(0L, result);
00075: 	}
00076: 
00077: 	internal static int Budget(ContainerCategory category)
00078: 	{
00079: 		return AuctionMerchantRules.Budget(Gifts(category));
00080: 	}
00081: 
00082: 	internal static string TierName(ContainerCategory category)
00083: 	{
00084: 		return TierName(AuctionMerchantRules.Tier(Gifts(category)));
00085: 	}
00086: 
00087: 	private static string T(string zh, string en)
00088: 	{
00089: 		return AuctionText.Select(zh, en);
00090: 	}
00091: 
00092: 	internal static string TierName(int tier)
00093: 	{
00094: 		return tier switch
00095: 		{
00096: 			1 => T("熟客", "Regular"), 
00097: 			2 => T("合作伙伴", "Partner"), 
00098: 			3 => T("核心伙伴", "Trusted Partner"), 
00099: 			_ => T("尚未建立合作", "New Contact"), 
00100: 		};
00101: 	}
00102: 
00103: 	internal static string Summary(ContainerCategory category)
00104: 	{
00105: 		long gifts = Gifts(category);
00106: 		return TierName(category) + T(" · 收购加成 +", " · Buying premium +") + AuctionMerchantRules.Premium(gifts) + T("% · 赠礼 ", "% · Gifts ") + gifts.ToString("N0") + "¢";
00107: 	}
00108: 
00109: 	internal static void ApplyPrice(StoreClient client, ContainerCategory category)
00110: 	{
00111: 		int num = 0;
00112: 		List<string> list = new List<string>();
00113: 		if (client.tag != null)
00114: 		{
00115: 			for (int num2 = client.tag.Count - 1; num2 >= 0; num2--)
00116: 			{
00117: 				string text = client.tag[num2];
00118: 				if (text != null && text.StartsWith("XIAOWO_AUCTION_RELATION_PRICE_", StringComparison.Ordinal))
00119: 				{
00120: 					if (int.TryParse(client.tag[num2].Substring("XIAOWO_AUCTION_RELATION_PRICE_".Length), out var result))
00121: 					{
00122: 						num += result;
00123: 					}
00124: 					list.Add(client.tag[num2]);
00125: 				}
00126: 			}
00127: 		}
00128: 		int num3 = AuctionMerchantRules.Premium(Gifts(category));
00129: 		if (list.Count == 1 && list[0] == "XIAOWO_AUCTION_RELATION_PRICE_" + num3)
00130: 		{
00131: 			return;
00132: 		}
00133: 		foreach (string item in list)
00134: 		{
00135: 			client.tag.Remove(item);
00136: 		}
00137: 		client.sellPriceModifier = checked(client.sellPriceModifier - num + num3);
00138: 		client.AddTag("XIAOWO_AUCTION_RELATION_PRICE_" + num3);
00139: 	}
00140: 
00141: 	private static GameItem[] CounterItems()
00142: 	{
00143: 		EmporiumEntry instance = EmporiumEntry.Instance;
00144: 		GameGridInventory counter = ((instance != null) ? instance.backInvinvElement : null);
00145: 		if (counter == null)
00146: 		{
00147: 			return Array.Empty<GameItem>();
00148: 		}
00149: 		return (from i in ((GameInventory)counter).childItems.ToArray()
00150: 			where i != null && GeneralHelper.IsItemOwned(i) && ObjectIdentity.Key(i.parentInventory) == ObjectIdentity.Key(counter)
00151: 			group i by ObjectIdentity.Key(i) into g
00152: 			select g.First()).ToArray();
00153: 	}
00154: 
00155: 	internal static bool AcceptsGift(ContainerCategory category, GameItem item)
00156: 	{
00157: 		if (item != null && !item.IsTag("XIAOWO_AUCTION_GIFT_GIVEN") && GeneralHelper.IsItemOwned(item) && item.unitCount >= 1 && !(item.identifier == "xiaowo_auction_phone_card") && !(item.identifier == "xiaowo_auction_container"))
00158: 		{
~~~

~~~csharp
00165: 				}
00166: 				return AuctionMerchants.BuyingTags(category).Any((string tag) => item.IsTag(tag) || item.IsGameItemType(tag));
00167: 			}
00168: 		}
00169: 		return false;
00170: 	}
00171: 
00172: 	internal static long GiftValue(GameItem item)
00173: 	{
00174: 		return Math.Max(0L, item.GetCurrentValue(false, false, false, false, 0L));
00175: 	}
00176: 
00177: 	private static bool Context(StoreClient client, out ContainerCategory category)
00178: 	{
00179: 		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
00180: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
00181: 		category = ContainerCategory.Medical;
00182: 		if (!Core.IsShuttingDown)
00183: 		{
00184: 			Scene activeScene = SceneManager.GetActiveScene();
00185: 			if (((Scene)(ref activeScene)).name == "InventoryScene" && NativeStore.Existing != null && NativeStore.Existing.isClientArrived && client != null)
00186: 			{
00187: 				StoreClientInstance currentClientInstance = NativeStore.Existing.currentClientInstance;
00188: 				if (ObjectIdentity.Key((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null) == ObjectIdentity.Key(client) && !client.isBusinessComplete)
00189: 				{
00190: 					return AuctionMerchants.TryCategory(client, out category);
00191: 				}
00192: 			}
00193: 		}
00194: 		return false;
00195: 	}
00196: 
00197: 	internal static bool TryGive(StoreClient client, GameItem[] items, out string message)
00198: 	{
00199: 		message = T("赠礼未完成，请重新确认交易台和商人。", "Gift not given. Check the buyer and counter again.");
00200: 		if (_committing || !Context(client, out var category) || items == null || items.Length == 0 || items.Select((GameItem i) => ObjectIdentity.Key(i)).Distinct().Count() != items.Length)
00201: 		{
00202: 			return false;
00203: 		}
00204: 		long gifts = Gifts(category);
00205: 		if (AuctionMerchantRules.Tier(gifts) == 3)
00206: 		{
00207: 			message = T("已经是核心伙伴，无需继续赠礼。", "Already a Trusted Partner. No more gifts are needed.");
00208: 			return false;
00209: 		}
00210: 		GameItem[] counter = CounterItems();
00211: 		if (!items.All((GameItem i) => i != null && counter.Any((GameItem c) => ObjectIdentity.Key(c) == ObjectIdentity.Key(i)) && AcceptsGift(category, i)))
00212: 		{
00213: 			return false;
00214: 		}
00215: 		long[] source = items.Select(GiftValue).ToArray();
00216: 		if (source.Any((long v) => v <= 0) || source.Sum() <= 0)
00217: 		{
00218: 			return false;
00219: 		}
00220: 		long value = source.Sum();
00221: 		(GameItem, GameInventory, GridShape)[] array = items.Select((GameItem i) => (Item: i, Inventory: i.parentInventory, Shape: i.modifiedShape.Clone())).ToArray();
00222: 		List<GameItem> list = new List<GameItem>();
00223: 		_committing = true;
00224: 		try
00225: 		{
00226: 			foreach (GameItem val in items)
00227: 			{
00228: 				if (!val.parentInventory.Expel(val))
00229: 				{
00230: 					throw new InvalidOperationException("Gift could not leave its inventory.");
00231: 				}
00232: 				list.Add(val);
00233: 				GeneralHelper.SetItemOwned(val, false);
00234: 				val.EnableTag("XIAOWO_AUCTION_GIFT_GIVEN", true);
00235: 			}
00236: 			AuctionState.Write(Key(category), AuctionMerchantRules.AddGift(gifts, value).ToString());
00237: 		}
00238: 		catch (Exception ex)
00239: 		{
00240: 			AuctionState.Write(Key(category), gifts.ToString());
00241: 			(GameItem, GameInventory, GridShape)[] array2 = array;
00242: 			for (int num = 0; num < array2.Length; num++)
00243: 			{
00244: 				(GameItem Item, GameInventory Inventory, GridShape Shape) placement = array2[num];
00245: 				if (list.Any((GameItem i) => ObjectIdentity.Key(i) == ObjectIdentity.Key(placement.Item)))
00246: 				{
00247: 					placement.Item.DisableTag("XIAOWO_AUCTION_GIFT_GIVEN", true);
00248: 					GeneralHelper.SetItemOwned(placement.Item, true);
00249: 					if (!NativePlacement.Restore(placement.Item, placement.Inventory, placement.Shape))
00250: 					{
00251: 						EmporiumEntry.Instance.TryAddToPlayerInv(placement.Item);
00252: 					}
00253: 				}
00254: 			}
00255: 			Core.Log?.Error("Merchant gift rolled back: " + ex);
00256: 			return false;
00257: 		}
00258: 		finally
00259: 		{
00260: 			_committing = false;
00261: 		}
00262: 		foreach (GameItem item in list)
00263: 		{
00264: 			item.Destroy();
00265: 		}
00266: 		ApplyPrice(client, category);
00267: 		AuctionMerchants.RefreshDialogue(client);
00268: 		EmporiumEntry.Instance.Validate(true);
00269: 		message = T("赠礼完成。累计赠礼：", "Gift received. Total gifts: ") + gifts.ToString("N0") + " → " + Gifts(category).ToString("N0") + "¢\n" + Summary(category) + T("\n下次到访预算：", "\nNext visit budget: ") + Budget(category).ToString("N0") + "¢";
00270: 		Core.Log?.Msg("Auction merchant gift: " + category.ToString() + ", value=" + value + ", total=" + Gifts(category) + ".");
00271: 		return true;
00272: 	}
00273: 
00274: 	internal static void Dispose()
00275: 	{
00276: 		DialogUIManager instance = DialogUIManager.Instance;
00277: 		if ((Object)(object)instance != (Object)null && instance.currentDialog != null && _pages.TryGetValue(instance.currentDialog, out var _))
00278: 		{
00279: 			instance.EndConversation();
00280: 		}
00281: 		DisposeHud();
00282: 		_pages = new ConditionalWeakTable<Dialogue, object>();
00283: 	}
00284: 
00285: 	private static void Choice(Dialogue page, string caption, Func<Dialogue> next)
00286: 	{
00287: 		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
00288: 		Dialogue reply = new Dialogue().SetText("PLAYER", caption);
00289: 		_pages.Add(reply, new object());
00290: 		page.AddChoice(ItemDirectory.CreateEmptyItem((string)null), caption, "", reply, (Action)delegate
~~~

~~~csharp
00356: 
00357: 	private static Dialogue BackPage(StoreClient client, string text)
00358: 	{
00359: 		Dialogue val = Line(text);
00360: 		ContainerCategory category2;
00361: 		if (Context(client, out var category) && AuctionMerchantRules.Tier(Gifts(category)) < 3)
00362: 		{
00363: 			Choice(val, T("返回合作选项", "Back to Options"), () => (!Context(client, out category2)) ? Line(Unavailable) : Menu(client));
00364: 		}
00365: 		Choice(val, T("开始交易", "Start Trading"), () => (!Context(client, out category2)) ? Line(Unavailable) : Trade(client));
00366: 		return val;
00367: 	}
00368: 
00369: 	private static Dialogue Relationship(StoreClient client)
00370: 	{
00371: 		if (!Context(client, out var category))
00372: 		{
00373: 			return Line(Unavailable);
00374: 		}
00375: 		long num = Gifts(category);
00376: 		int num2 = AuctionMerchantRules.Tier(num);
00377: 		string text = ((num2 < 3) ? (T("\n下一档还需赠礼：", "\nGifts needed for next level: ") + Math.Max(0L, AuctionMerchantRules.Threshold(num2 + 1) - num).ToString("N0") + "¢") : T("\n已满级，无需继续赠礼。", "\nMaximum level. No more gifts needed."));
00378: 		return BackPage(client, "LV " + num2 + " · " + Summary(category) + text + T("\n下次到访预算：", "\nNext visit budget: ") + Budget(category).ToString("N0") + "¢");
00379: 	}
00380: 
00381: 	private static Dialogue PrepareGift(StoreClient client)
00382: 	{
00383: 		if (!Context(client, out var category))
00384: 		{
00385: 			return Line(Unavailable);
00386: 		}
00387: 		if (AuctionMerchantRules.Tier(Gifts(category)) >= 3)
00388: 		{
00389: 			return Relationship(client);
00390: 		}
00391: 		Dialogue obj = Line(T("请把要赠送的自有物品放到称重交易台，再选择“检查赠礼”。只接受", "Place the goods you own on the weighing counter, then choose Check Gift. I only accept ") + AuctionPhone.CategoryCaption(category) + T("。可以直接拖动物品；不会自动出售。容器、名片及其他类别不收。", ". You can drag items now; nothing is sold automatically. No containers, cards or other categories."));
00392: 		Choice(obj, T("检查赠礼", "Check Gift"), () => Confirmation(client));
00393: 		Choice(obj, T("返回合作选项", "Back to Options"), () => (!Context(client, out var _)) ? Line(Unavailable) : Menu(client));
00394: 		Choice(obj, T("开始交易", "Start Trading"), () => (!Context(client, out category2)) ? Line(Unavailable) : Trade(client));
00395: 		return obj;
00396: 	}
00397: 
00398: 	private static Dialogue Confirmation(StoreClient client)
00399: 	{
00400: 		if (!Context(client, out var category))
00401: 		{
00402: 			return Line(Unavailable);
00403: 		}
00404: 		PlayerStore store = NativeStore.Existing;
00405: 		long generation = AuctionLoadLifecycle.Generation;
00406: 		GameItem[] items = CounterItems();
00407: 		ContainerCategory category2;
00408: 		if (items.Length == 0 || items.Any((GameItem i) => !AcceptsGift(category, i) || GiftValue(i) <= 0))
00409: 		{
00410: 			Dialogue obj = Line(T("交易台为空，或含有不接受的物品。请只放上对应类别的自有货物，再检查赠礼。物品没有消耗。", "The counter is empty or contains unsuitable goods. Place only matching goods you own and check again. Nothing was consumed."));
00411: 			Choice(obj, T("重新检查赠礼", "Check Again"), () => Confirmation(client));
00412: 			Choice(obj, T("返回合作选项", "Back to Options"), () => (!Context(client, out category2)) ? Line(Unavailable) : Menu(client));
00413: 			return obj;
00414: 		}
00415: 		(GameItem Item, int Count, long Value)[] snapshot = items.Select((GameItem i) => (Item: i, Count: i.unitCount, Value: GiftValue(i))).ToArray();
00416: 		long before = Gifts(category);
00417: 		long value = snapshot.Sum(((GameItem Item, int Count, long Value) i) => i.Value);
00418: 		long gifts = AuctionMerchantRules.AddGift(before, value);
00419: 		bool used = false;
00420: 		Dialogue obj2 = Line(T("确认赠送交易台上 ", "Give all ") + items.Length + T(" 件物品？\n赠礼价值：", " items on the counter?\nGift value: ") + value.ToString("N0") + "¢\n" + T("累计赠礼：", "Total gifts: ") + before.ToString("N0") + " → " + gifts.ToString("N0") + "¢\nLV " + AuctionMerchantRules.Tier(before) + " → LV " + AuctionMerchantRules.Tier(gifts) + T("\n确认后送出物品，不支付货款。", "\nConfirm to give the goods without payment."));
00421: 		Choice(obj2, T("确认赠送", "Confirm Gift"), delegate
00422: 		{
00423: 			if (used)
00424: 			{
00425: 				return Line(T("本次赠礼已处理，请勿重复确认。", "This gift has already been handled."));
00426: 			}
00427: 			used = true;
00428: 			if (generation != AuctionLoadLifecycle.Generation || store != NativeStore.Existing || !Context(client, out category2))
00429: 			{
00430: 				return Line(Unavailable);
00431: 			}
00432: 			GameItem[] current = CounterItems();
00433: 			if (Gifts(category) != before || current.Length != snapshot.Length || !snapshot.All(((GameItem Item, int Count, long Value) s) => current.Contains(s.Item) && s.Item.unitCount == s.Count && GiftValue(s.Item) == s.Value))
00434: 			{
00435: 				return BackPage(client, T("交易台发生变化，请重新检查赠礼。没有消耗物品。", "The counter changed. Check the gift again. Nothing was consumed."));
00436: 			}
00437: 			TryGive(client, items, out var message);
00438: 			return BackPage(client, message);
00439: 		});
00440: 		Choice(obj2, T("取消赠送", "Cancel Gift"), () => (!Context(client, out category2)) ? Line(Unavailable) : PrepareGift(client));
00441: 		return obj2;
00442: 	}
00443: 
00444: 	internal static void Tick()
00445: 	{
00446: 		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
00447: 		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
00448: 		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
00449: 		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
00450: 		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
00451: 		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
00452: 		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
00453: 		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
00454: 		//IL_0141: Expected O, but got Unknown
00455: 		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
00456: 		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
00457: 		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
00458: 		PlayerStore existing = NativeStore.Existing;
00459: 		object obj;
00460: 		if (existing == null)
00461: 		{
00462: 			obj = null;
00463: 		}
00464: 		else
00465: 		{
00466: 			StoreClientInstance currentClientInstance = existing.currentClientInstance;
00467: 			obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00468: 		}
00469: 		StoreClient client = (StoreClient)obj;
00470: 		StoreClientMono instance = StoreClientMono.Instance;
00471: 		if (!Context(client, out var category) || (Object)(object)instance == (Object)null || (Object)(object)instance.image == (Object)null || !((Renderer)instance.image).enabled || instance.IsMoving())
00472: 		{
00473: 			if ((Object)(object)_hud != (Object)null)
00474: 			{
00475: 				((Component)_hud).gameObject.SetActive(false);
00476: 			}
00477: 			return;
00478: 		}
00479: 		if ((Object)(object)_hud == (Object)null)
00480: 		{
00481: 			CreateHud();
00482: 		}
00483: 		if ((Object)(object)_hud == (Object)null || (Object)(object)_canvas == (Object)null)
00484: 		{
~~~

### XIAOWONightAuction/AuctionRules.cs

~~~csharp
00004: 
00005: internal static class AuctionRules
00006: {
00007: 	internal const int MerchantBudget = 100000;
00008: 
00009: 	internal const int RestrictedNeuralChance = 10;
00010: 
00011: 	internal const int HardItemLimit = 32;
00012: 
00013: 	internal static bool IsRestrictedNeuralEligible(AuctionVenue venue, ContainerCategory category)
00014: 	{
00015: 		if (venue == AuctionVenue.Intermediate || venue == AuctionVenue.Advanced)
00016: 		{
00017: 			if (category != ContainerCategory.Contraband)
00018: 			{
00019: 				return category == ContainerCategory.Engineering;
00020: 			}
00021: 			return true;
00022: 		}
00023: 		return false;
00024: 	}
00025: 
00026: 	internal static bool ShouldIncludeRestrictedNeural(AuctionVenue venue, ContainerCategory category, int lotSeed)
00027: 	{
00028: 		if (IsRestrictedNeuralEligible(venue, category))
00029: 		{
00030: 			return new Random(lotSeed ^ 0x5A17C0DE).Next(100) < 10;
00031: 		}
00032: 		return false;
00033: 	}
00034: 
00035: 	internal static string VenueName(AuctionVenue venue)
00036: 	{
00037: 		return venue switch
00038: 		{
00039: 			AuctionVenue.Beginner => AuctionText.Select("新手场", "Beginner"), 
00040: 			AuctionVenue.Intermediate => AuctionText.Select("中级场", "Intermediate"), 
00041: 			_ => AuctionText.Select("高级场", "Advanced"), 
00042: 		};
00043: 	}
00044: 
00045: 	internal static int EntryFee(AuctionVenue venue)
00046: 	{
00047: 		return venue switch
00048: 		{
00049: 			AuctionVenue.Beginner => 0, 
00050: 			AuctionVenue.Intermediate => 300, 
00051: 			_ => 1500, 
00052: 		};
00053: 	}
00054: 
00055: 	internal static int RecommendedCash(AuctionVenue venue)
00056: 	{
00057: 		return venue switch
00058: 		{
00059: 			AuctionVenue.Beginner => 2000, 
00060: 			AuctionVenue.Intermediate => 8000, 
00061: 			_ => 35000, 
00062: 		};
00063: 	}
00064: 
00065: 	internal static (int Min, int Max) BaselineRange(AuctionVenue venue)
00066: 	{
00067: 		return venue switch
00068: 		{
00069: 			AuctionVenue.Beginner => (Min: 200, Max: 1500), 
00070: 			AuctionVenue.Intermediate => (Min: 1500, Max: 7000), 
00071: 			_ => (Min: 7000, Max: 30000), 
00072: 		};
00073: 	}
00074: 
00075: 	internal static (int Min, int Max) ItemCountRange(AuctionVenue venue)
00076: 	{
00077: 		return venue switch
00078: 		{
00079: 			AuctionVenue.Beginner => (Min: 5, Max: 10), 
00080: 			AuctionVenue.Intermediate => (Min: 8, Max: 16), 
00081: 			_ => (Min: 12, Max: 24), 
00082: 		};
00083: 	}
00084: 
00085: 	internal static int SealedChance(AuctionVenue venue)
00086: 	{
00087: 		return venue switch
00088: 		{
00089: 			AuctionVenue.Beginner => 20, 
00090: 			AuctionVenue.Intermediate => 45, 
00091: 			_ => 65, 
00092: 		};
00093: 	}
00094: 
00095: 	internal static (int Min, int Max) OpeningPercent(AuctionVenue venue)
00096: 	{
00097: 		return venue switch
00098: 		{
00099: 			AuctionVenue.Beginner => (Min: 35, Max: 51), 
00100: 			AuctionVenue.Intermediate => (Min: 45, Max: 61), 
00101: 			_ => (Min: 55, Max: 71), 
00102: 		};
00103: 	}
00104: 
00105: 	internal static AuctionQuality RollQuality(Random random)
00106: 	{
00107: 		int num = random.Next(100);
00108: 		if (num < 14)
00109: 		{
00110: 			return AuctionQuality.Loss;
~~~

### XIAOWONightAuction/AuctionCatalog.cs

~~~csharp
00016: 	internal const string InternalOnlyTag = "XIAOWO_AUCTION_INTERNAL_ONLY";
00017: 
00018: 	internal const string DataTag = "XIAOWO_AUCTION_CONTAINER_DATA";
00019: 
00020: 	private static readonly HashSet<string> ExcludedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
00021: 	{
00022: 		"save_bag", "back_inv_save_bag", "hidden_save_bag", "showcase_save_bag", "sold_save_bag", "sec_box", "evidence_box", "med_box", "eng_box", "service_box",
00023: 		"system_capped_neural_core"
00024: 	};
00025: 
00026: 	private static readonly Dictionary<ContainerCategory, List<string>> Pools = new Dictionary<ContainerCategory, List<string>>();
00027: 
00028: 	private static readonly Dictionary<string, long> ValueCache = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
00029: 
00030: 	private static readonly Dictionary<(ContainerCategory, AuctionVenue), List<string>> BandCache = new Dictionary<(ContainerCategory, AuctionVenue), List<string>>();
00031: 
00032: 	private static readonly HashSet<string> ReleasedNativeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
00033: 
00034: 	private static readonly HashSet<string> NativeModuleIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
00035: 
00036: 	internal static int Day => Math.Max(0, StoreStation.GetDayCounter());
00037: 
00038: 	internal static bool AvailabilityReady
00039: 	{
00040: 		get
00041: 		{
00042: 			EnsureReleasedNativeIds();
00043: 			return ReleasedNativeIds.Count > 0;
00044: 		}
00045: 	}
00046: 
00047: 	internal static string CategoryName(ContainerCategory category)
00048: 	{
00049: 		return category switch
00050: 		{
00051: 			ContainerCategory.Medical => AuctionText.Get("category_medical", "医疗集装箱"), 
00052: 			ContainerCategory.Luxury => AuctionText.Get("category_luxury", "奢侈品集装箱"), 
00053: 			ContainerCategory.Contraband => AuctionText.Get("category_contraband", "违禁品集装箱"), 
00054: 			_ => AuctionText.Get("category_engineering", "工程集装箱"), 
00055: 		};
00056: 	}
00057: 
00058: 	internal static Color CategoryColor(ContainerCategory category)
00059: 	{
00060: 		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
00061: 		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
00062: 		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
00063: 		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
00064: 		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
00065: 		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
00066: 		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
00067: 		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
00068: 		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
00069: 		return (Color)(category switch
00070: 		{
00071: 			ContainerCategory.Medical => new Color(0.4f, 0.72f, 0.64f, 1f), 
00072: 			ContainerCategory.Luxury => new Color(0.78f, 0.62f, 0.25f, 1f), 
00073: 			ContainerCategory.Contraband => new Color(0.68f, 0.26f, 0.28f, 1f), 
00074: 			_ => new Color(0.3f, 0.55f, 0.72f, 1f), 
00075: 		});
00076: 	}
00077: 
00078: 	internal static string SpriteName(ContainerCategory category)
00079: 	{
00080: 		return category switch
00081: 		{
00082: 			ContainerCategory.Medical => "xiaowo_auction_container_medical", 
00083: 			ContainerCategory.Luxury => "xiaowo_auction_container_luxury", 
00084: 			ContainerCategory.Contraband => "xiaowo_auction_container_contraband", 
00085: 			_ => "xiaowo_auction_container_engineering", 
00086: 		};
00087: 	}
00088: 
00089: 	internal static string NativeSpriteName(ContainerCategory category)
00090: 	{
00091: 		return category switch
00092: 		{
00093: 			ContainerCategory.Medical => "med_box", 
00094: 			ContainerCategory.Luxury => "service_box", 
00095: 			ContainerCategory.Contraband => "sec_box", 
00096: 			_ => "eng_box", 
00097: 		};
00098: 	}
00099: 
00100: 	internal static bool TryParseContainerSprite(string spriteName, out ContainerCategory category)
00101: 	{
00102: 		category = ContainerCategory.Engineering;
00103: 		switch (spriteName)
00104: 		{
00105: 		case "xiaowo_auction_container_medical":
00106: 			category = ContainerCategory.Medical;
00107: 			break;
00108: 		case "xiaowo_auction_container_luxury":
00109: 			category = ContainerCategory.Luxury;
00110: 			break;
00111: 		case "xiaowo_auction_container_contraband":
00112: 			category = ContainerCategory.Contraband;
00113: 			break;
00114: 		default:
00115: 			if (spriteName != "xiaowo_auction_container_engineering")
00116: 			{
00117: 				return false;
00118: 			}
00119: 			break;
00120: 		}
00121: 		return true;
00122: 	}
00123: 
~~~

~~~csharp
00189: 		foreach (string item2 in identifierList)
00190: 		{
00191: 			if (string.IsNullOrWhiteSpace(item2) || item2 == "xiaowo_auction_container" || ExcludedIds.Contains(item2) || AuctionExclusiveCatalog.IsExclusive(item2) || hashSet.Contains(item2) || !ReleasedNativeIds.Contains(item2))
00192: 			{
00193: 				continue;
00194: 			}
00195: 			try
00196: 			{
00197: 				GameItem val = DirectoryMaster.Item(item2, false);
00198: 				if (!IsEligible(val))
00199: 				{
00200: 					continue;
00201: 				}
00202: 				if (val.IsTag("MODULE"))
00203: 				{
00204: 					NativeModuleIds.Add(item2);
00205: 				}
00206: 				foreach (ContainerCategory value2 in Enum.GetValues(typeof(ContainerCategory)))
00207: 				{
00208: 					if (Matches(value2, val))
00209: 					{
00210: 						Pools[value2].Add(item2);
00211: 					}
00212: 				}
00213: 			}
00214: 			catch
00215: 			{
00216: 			}
00217: 		}
00218: 		Core.Log?.Msg("Auction pools cached: " + string.Join(", ", new ContainerCategory[4]
00219: 		{
00220: 			ContainerCategory.Medical,
00221: 			ContainerCategory.Luxury,
00222: 			ContainerCategory.Contraband,
00223: 			ContainerCategory.Engineering
00224: 		}.Select((ContainerCategory category) => category.ToString() + "=" + Pools[category].Count)));
00225: 	}
00226: 
00227: 	private static void EnsureReleasedNativeIds()
00228: 	{
00229: 		if (ReleasedNativeIds.Count > 0)
00230: 		{
00231: 			return;
00232: 		}
00233: 		foreach (string tableId in LootRegistry.TableIds)
00234: 		{
00235: 			List<LootEntry> tableEntries = LootRegistry.GetTableEntries(tableId);
00236: 			if (tableEntries == null)
00237: 			{
00238: 				continue;
00239: 			}
00240: 			for (int i = 0; i < tableEntries.Count; i++)
00241: 			{
00242: 				string text = tableEntries[i]?.id;
00243: 				if (!string.IsNullOrWhiteSpace(text))
00244: 				{
00245: 					ReleasedNativeIds.Add(text);
00246: 				}
00247: 			}
00248: 		}
00249: 		Core.Log?.Msg("Retail loot whitelist cached: " + ReleasedNativeIds.Count + " item(s); other MOD directory IDs excluded.");
00250: 	}
00251: 
00252: 	internal static List<string> GenerateContents(AuctionLot lot)
00253: 	{
00254: 		if (lot == null)
00255: 		{
00256: 			return new List<string>();
00257: 		}
00258: 		if (lot.Contents.Count > 0)
00259: 		{
00260: 			return new List<string>(lot.Contents);
00261: 		}
00262: 		List<string> list = null;
00263: 		long num = long.MaxValue;
00264: 		(int, int) tuple = AuctionRules.ItemCountRange(lot.Venue);
00265: 		int num2 = Math.Clamp(lot.ItemCount, tuple.Item1, tuple.Item2);
00266: 		int num3 = AuctionNightMarket.MainPercent(lot.Category);
00267: 		bool flag = AuctionRules.ShouldIncludeRestrictedNeural(lot.Venue, lot.Category, lot.Seed) && IsAvailableItem("system_capped_neural_core");
00268: 		int num4 = AuctionRules.RollExclusiveTier(lot.Venue, lot.Quality, lot.Seed);
00269: 		int num5 = 0;
00270: 		if (lot.Quality == AuctionQuality.Jackpot && num4 >= 2)
00271: 		{
00272: 			int num6 = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Tier == 3).Min((AuctionExclusiveSpec spec) => spec.Value);
00273: 			int num7 = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Tier == 2).Min((AuctionExclusiveSpec spec) => spec.Value);
00274: 			if (num4 == 3 && lot.TargetValue >= (long)num6 * 13L / 10)
00275: 			{
00276: 				num5 = 3;
00277: 			}
00278: 			else if (lot.TargetValue >= (long)num7 * 13L / 10)
00279: 			{
00280: 				num5 = 2;
00281: 			}
00282: 		}
00283: 		for (int num8 = 0; num8 < 128; num8++)
00284: 		{
00285: 			Random random = new Random(lot.Seed ^ ((num8 + 1) * 104729));
00286: 			int num9 = ((num8 == 0) ? num2 : random.Next(tuple.Item1, tuple.Item2 + 1));
00287: 			int num10 = (flag ? random.Next(num9) : (-1));
00288: 			int num11 = AuctionRules.ExclusiveSlots(lot.Quality, num9, random);
00289: 			HashSet<int> hashSet = new HashSet<int>();
00290: 			while (hashSet.Count < num11)
00291: 			{
00292: 				hashSet.Add(random.Next(num9));
00293: 			}
~~~

~~~csharp
00351: 	internal static long ItemValue(string id)
00352: 	{
00353: 		if (AuctionExclusiveCatalog.Items.TryGetValue(id ?? string.Empty, out var value))
00354: 		{
00355: 			return value.Value;
00356: 		}
00357: 		if (id != null && ValueCache.TryGetValue(id, out var value2))
00358: 		{
00359: 			return value2;
00360: 		}
00361: 		long num;
00362: 		try
00363: 		{
00364: 			long val = 1L;
00365: 			GameItem obj = DirectoryMaster.Item(id, false);
00366: 			num = Math.Max(val, (obj != null) ? obj.GetCurrentValue(false, false, false, false, 0L) : 0);
00367: 		}
00368: 		catch
00369: 		{
00370: 			num = 1L;
00371: 		}
00372: 		if (id != null)
00373: 		{
00374: 			ValueCache[id] = num;
00375: 		}
00376: 		return num;
00377: 	}
00378: 
00379: 	private static string ChooseExclusive(ContainerCategory category, int maxTier, int requiredTier, long desired, Dictionary<string, int> usage, Random random)
00380: 	{
00381: 		List<AuctionExclusiveSpec> source = AuctionExclusiveCatalog.Candidates(category, maxTier);
00382: 		if (requiredTier > 0)
00383: 		{
00384: 			source = source.Where((AuctionExclusiveSpec spec) => spec.Tier == requiredTier).ToList();
00385: 		}
00386: 		return ChooseClosest(source.Select((AuctionExclusiveSpec spec) => spec.Id), desired, usage, random, 4);
00387: 	}
00388: 
00389: 	private static string ChooseRegular(ContainerCategory category, AuctionVenue venue, AuctionQuality quality, int exclusiveTier, long desired, Dictionary<string, int> usage, Random random)
00390: 	{
00391: 		List<string> list = new List<string>(ValueBand(category, venue));
00392: 		list.AddRange(from spec in AuctionExclusiveCatalog.SupplementalCandidates(category)
00393: 			select spec.Id);
00394: 		if (quality != AuctionQuality.Loss && quality != AuctionQuality.Hidden && (venue != AuctionVenue.Beginner || quality >= AuctionQuality.Profit))
00395: 		{
00396: 			int maxTier = ((quality == AuctionQuality.Random) ? 1 : exclusiveTier);
00397: 			list.AddRange(from spec in AuctionExclusiveCatalog.Candidates(category, maxTier)
00398: 				select spec.Id);
00399: 		}
00400: 		return ChooseClosest(list, desired, usage, random, 3);
00401: 	}
00402: 
00403: 	private static string ChooseClosest(IEnumerable<string> source, long desired, Dictionary<string, int> usage, Random random, int duplicateLimit)
00404: 	{
00405: 		List<string> list = source.Distinct<string>(StringComparer.OrdinalIgnoreCase).ToList();
00406: 		string best = null;
00407: 		long num = long.MaxValue;
00408: 		foreach (string item in list)
00409: 		{
00410: 			if (!usage.TryGetValue(item, out var value) || value < duplicateLimit)
00411: 			{
00412: 				long num2 = ItemValue(item);
00413: 				int num3 = random.Next(0, (int)Math.Min(2147483647L, Math.Max(2L, desired / 5)));
00414: 				long num4 = Math.Abs(num2 - desired) + num3;
00415: 				if (num4 < num)
00416: 				{
00417: 					best = item;
00418: 					num = num4;
00419: 				}
00420: 			}
00421: 		}
00422: 		if (best != null && NativeModuleIds.Contains(best))
00423: 		{
00424: 			int value2;
00425: 			List<string> list2 = list.Where((string id) => NativeModuleIds.Contains(id) && ItemValue(id) == ItemValue(best) && (!usage.TryGetValue(id, out value2) || value2 < duplicateLimit)).ToList();
00426: 			int leastUsed = list2.Min((string id) => usage.TryGetValue(id, out value2) ? value2 : 0);
00427: 			list2.RemoveAll((string id) => (usage.TryGetValue(id, out value2) ? value2 : 0) != leastUsed);
00428: 			return list2[random.Next(list2.Count)];
00429: 		}
00430: 		return best;
00431: 	}
00432: 
00433: 	private static List<string> ValueBand(ContainerCategory category, AuctionVenue venue)
00434: 	{
00435: 		if (BandCache.TryGetValue((category, venue), out var value))
00436: 		{
00437: 			return value;
00438: 		}
00439: 		List<string> list = BuildPool(category).OrderBy(ItemValue).ToList();
00440: 		if (list.Count <= 2)
00441: 		{
00442: 			BandCache[(category, venue)] = list;
00443: 			return list;
00444: 		}
00445: 		double num = venue switch
00446: 		{
00447: 			AuctionVenue.Intermediate => 0.25, 
00448: 			AuctionVenue.Beginner => 0.0, 
00449: 			_ => 0.55, 
00450: 		};
00451: 		double num2 = venue switch
00452: 		{
00453: 			AuctionVenue.Intermediate => 0.85, 
00454: 			AuctionVenue.Beginner => 0.55, 
00455: 			_ => 1.0, 
~~~

### XIAOWONightAuction/AuctionUpdateDriver.cs

~~~csharp
00001: using System;
00002: using UnityEngine;
00003: 
00004: namespace XIAOWONightAuction;
00005: 
00006: internal sealed class AuctionUpdateDriver : MonoBehaviour
00007: {
00008: 	private string _locale;
00009: 
00010: 	private int _nextLocaleCheck;
00011: 
00012: 	private void Update()
00013: 	{
00014: 		if (Core.IsShuttingDown)
00015: 		{
00016: 			return;
00017: 		}
00018: 		try
00019: 		{
00020: 			AuctionLoadLifecycle.Tick();
00021: 			if (!Core.StoreReady)
00022: 			{
00023: 				return;
00024: 			}
00025: 			AuctionController.Tick();
00026: 			WildeExpressEvidence.Tick();
00027: 			AuctionPhone.Tick();
00028: 			AuctionMerchantRelations.Tick();
00029: 			if (Time.frameCount >= _nextLocaleCheck)
00030: 			{
00031: 				_nextLocaleCheck = Time.frameCount + 30;
00032: 				string currentLocaleCode = AuctionText.CurrentLocaleCode;
00033: 				if (!(_locale == currentLocaleCode))
00034: 				{
00035: 					AuctionController.RefreshLocalizedText();
00036: 					AuctionExclusiveFactory.RefreshOwnedText();
00037: 					AuctionContainer.RefreshOwnedText();
00038: 					AuctionPhone.RefreshOwnedText();
00039: 					AuctionMerchants.RefreshLocalizedDialogue(PlayerStore.Instance.storeClientManager);
00040: 					_locale = currentLocaleCode;
00041: 				}
00042: 			}
00043: 		}
00044: 		catch (Exception ex)
00045: 		{
00046: 			Core.Log?.Error("Official update failed: " + ex);
00047: 			((Behaviour)this).enabled = false;
00048: 		}
00049: 	}
00050: }
~~~

### XIAOWONightAuction/AuctionMerchantDialoguePatch.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace XIAOWONightAuction;
00004: 
00005: [HarmonyPatch(typeof(StoreClient), "StartMainDialogue")]
00006: internal static class AuctionMerchantDialoguePatch
00007: {
00008: 	private static void Prefix(StoreClient __instance)
00009: 	{
00010: 		AuctionPhone.OnMerchantArrived(__instance);
00011: 		AuctionMerchants.OnMerchantArrived(__instance);
00012: 		AuctionMerchants.RefreshDialogue(__instance);
00013: 	}
00014: }
~~~

### XIAOWONightAuction/WildeExpressEvidence.cs

~~~csharp
00008: 
00009: namespace XIAOWONightAuction;
00010: 
00011: internal static class WildeExpressEvidence
00012: {
00013: 	internal const string UpgradeId = "EVIDENCE_REMOVAL";
00014: 
00015: 	internal const string ServiceId = "xiaowo_auction_express_evidence";
00016: 
00017: 	internal const string IconKey = "auction_express_cleanup";
00018: 
00019: 	private static readonly FieldInfo FrameField = AccessTools.Field(typeof(WildNetworkUI), "frame");
00020: 
00021: 	private static readonly FieldInfo SelectionField = AccessTools.Field(typeof(WildNetworkUI), "selectedId");
00022: 
00023: 	private static readonly FieldInfo TabsField = AccessTools.Field(typeof(WildNetworkUI), "tabs");
00024: 
00025: 	private static readonly FieldInfo CurrentTabField = AccessTools.Field(typeof(WildNetworkUI), "currentTab");
00026: 
00027: 	private static bool _registered;
00028: 
00029: 	private static WildNetworkFrame _frame;
00030: 
00031: 	private static string _selection;
00032: 
00033: 	private static int _nextRefresh;
00034: 
00035: 	internal static void RegisterForLoadedStore()
00036: 	{
00037: 		if (!Core.StoreReady)
00038: 		{
00039: 			return;
00040: 		}
00041: 		if (!_registered)
00042: 		{
00043: 			ModHelper.AddNetworkUpgrade("xiaowo_night_auction", "xiaowo_auction_express_evidence", 0, "Informant", (Func<string>)(() => AuctionText.Select("加急清证", "Express evidence cleanup")), (Func<string>)Description, (Action)null, (string)null, "xiaowo_night_auction:auction_express_cleanup", (string[])null, 0, (Func<string>)null, (Func<string>)null, false);
00044: 			_registered = true;
00045: 		}
00046: 		Dictionary<string, NetworkUpgrade> networkUpgrade = PlayerStore.Instance.networkUpgrade;
00047: 		if (networkUpgrade != null && !networkUpgrade.ContainsKey("xiaowo_auction_express_evidence"))
00048: 		{
00049: 			networkUpgrade.Add("xiaowo_auction_express_evidence", NetworkUpgrade.GetDefinition("xiaowo_auction_express_evidence").CopyForRuntime());
00050: 		}
00051: 	}
00052: 
00053: 	internal static void Unregister()
00054: 	{
00055: 		Dispose();
00056: 		if (_registered)
00057: 		{
00058: 			ModHelper.RemoveNetworkUpgrades("xiaowo_night_auction");
00059: 		}
00060: 		_registered = false;
00061: 	}
00062: 
00063: 	private static string Description()
00064: 	{
00065: 		return AuctionText.Select("在原版清除证据服务的冷却期间，用现金加急销毁已收集的证据并终止当前调查。原版服务可以使用时，本服务不可购买。\n\n费用为原版当前价格，加上按剩余冷却天数递减的加急费，最高加收3000信用点。只扣现金，不消耗王尔德的恩惠。\n\n每天最多加急一次；使用后同样重置原版14天冷却，并保留原版费用增长规则。", "Pay cash to destroy collected evidence and stop the current investigation while standard cleanup is cooling down. Unavailable when standard cleanup is ready.\n\nPay the current standard cost plus a surcharge of up to 3000 credits, falling with the remaining cooldown. Cash only; no Wilde favor is spent.\n\nOnce per day. Resets the standard 14-day cooldown and preserves its normal cost increase.");
00066: 	}
00067: 
00068: 	internal static int Quote(int nativeCost, int remaining, int duration)
00069: 	{
00070: 		return ExpressEvidencePricing.Quote(nativeCost, remaining, duration);
00071: 	}
00072: 
00073: 	internal static int CurrentPrice()
00074: 	{
00075: 		if (!Core.StoreReady)
00076: 		{
00077: 			return 0;
00078: 		}
00079: 		NetworkUpgrade upgrade = GetUpgrade(PlayerStore.Instance);
00080: 		if (upgrade != null)
00081: 		{
00082: 			return Quote(upgrade.GetCost(), upgrade.cooldownCurrent, upgrade.cooldownDuration);
00083: 		}
00084: 		return 0;
00085: 	}
00086: 
00087: 	internal static bool CanPurchase()
00088: 	{
00089: 		if (!Core.StoreReady)
00090: 		{
00091: 			return false;
00092: 		}
00093: 		PlayerStore instance = PlayerStore.Instance;
00094: 		NetworkUpgrade upgrade = GetUpgrade(instance);
00095: 		if (upgrade != null)
00096: 		{
00097: 			return ExpressEvidencePricing.Available(upgrade.state, upgrade.cooldownCurrent, upgrade.IsHavePrerequisite(), demoLocked: false, UsedToday(), instance.GetCash(), CurrentPrice());
00098: 		}
00099: 		return false;
00100: 	}
00101: 
00102: 	internal static void Tick()
00103: 	{
00104: 		if (!Core.StoreReady || !WildNetworkUI.IsOpen)
00105: 		{
00106: 			Dispose();
00107: 		}
00108: 		else if (!(_selection != "xiaowo_auction_express_evidence") && Time.frameCount >= _nextRefresh)
00109: 		{
00110: 			_nextRefresh = Time.frameCount + 10;
00111: 			RefreshButton();
00112: 		}
00113: 	}
00114: 
00115: 	internal static void Selected(string id)
00116: 	{
00117: 		_selection = id;
00118: 		_frame = (WildNetworkFrame)((id == "xiaowo_auction_express_evidence" && Core.StoreReady && WildNetworkUI.IsOpen) ? /*isinst with value type is only supported in some contexts*/: null);
00119: 		RefreshButton();
00120: 	}
00121: 
00122: 	internal static void PageChanged()
00123: 	{
00124: 		Dispose();
00125: 	}
00126: 
00127: 	private static bool IsServiceTab()
00128: 	{
00129: 		List<Tab> list = TabsField?.GetValue(null) as List<Tab>;
00130: 		int num = (CurrentTabField?.GetValue(null) as int?) ?? (-1);
00131: 		if (list == null || num < 0 || num >= list.Count)
00132: 		{
00133: 			return false;
00134: 		}
00135: 		foreach (Card card in list[num].cards)
00136: 		{
00137: 			foreach (Cell cell in card.cells)
00138: 			{
00139: 				if (cell.id == "xiaowo_auction_express_evidence")
00140: 				{
00141: 					return true;
00142: 				}
00143: 			}
00144: 		}
00145: 		return false;
00146: 	}
00147: 
00148: 	private static void RefreshButton()
00149: 	{
00150: 		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
00151: 		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
00152: 		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
00153: 		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
00154: 		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
00155: 		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
00156: 		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
00157: 		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
00158: 		if (Core.StoreReady && WildNetworkUI.IsOpen && !(_selection != "xiaowo_auction_express_evidence") && Object.op_Implicit((Object)(object)_frame))
00159: 		{
00160: 			bool flag = CanPurchase();
00161: 			((Selectable)_frame.useButton).interactable = flag;
00162: 			((TMP_Text)_frame.useLabel).text = AuctionText.Select("加急清证", "Express cleanup");
00163: 			((TMP_Text)_frame.usePrice).text = CurrentPrice() + "¢";
00164: 			((Graphic)_frame.useLabel).color = (flag ? WildNetworkStyle.BUTTON_TEXT : WildNetworkStyle.WithAlpha(WildNetworkStyle.BUTTON_TEXT, 0.3f));
00165: 			((Graphic)_frame.usePrice).color = (flag ? WildNetworkStyle.BUTTON_TEXT : WildNetworkStyle.BUTTON_TEXT_UNAVAILABLE);
00166: 			if (Object.op_Implicit((Object)(object)_frame.useInnerBorder))
00167: 			{
00168: 				((Graphic)_frame.useInnerBorder).color = (flag ? WildNetworkStyle.BUTTON_INNER : WildNetworkStyle.BUTTON_INNER_UNAVAILABLE);
00169: 			}
00170: 			NetworkUpgrade upgrade = GetUpgrade(PlayerStore.Instance);
00171: 			string text = (UsedToday() ? AuctionText.Select("今日已加急，请明天再来。", "Already used today. Come back tomorrow.") : ((upgrade == null || upgrade.cooldownCurrent <= 0) ? AuctionText.Select("请先使用原版清除证据服务。", "Use standard evidence cleanup first.") : ((!flag) ? AuctionText.Select("现金不足或原版服务尚未解锁。", "Not enough cash, or standard cleanup is unavailable.") : "")));
00172: 			((TMP_Text)_frame.infoMessage).text = text;
00173: 			((Graphic)_frame.infoMessage).color = WildNetworkStyle.INFO_WARNING;
00174: 			((Component)_frame.infoMessage).gameObject.SetActive(text.Length > 0);
00175: 			((TMP_Text)_frame.infoCooldown).text = ((upgrade != null && upgrade.cooldownCurrent > 0) ? (AuctionText.Select("原版冷却剩余：", "Standard cooldown remaining: ") + upgrade.cooldownCurrent + AuctionText.Select("天", " days")) : "");
00176: 			((Component)_frame.infoCooldown).gameObject.SetActive(((TMP_Text)_frame.infoCooldown).text.Length > 0);
00177: 		}
00178: 	}
00179: 
00180: 	internal static bool BeforeNativePurchase()
00181: 	{
00182: 		if (SelectionField?.GetValue(null) as string != "xiaowo_auction_express_evidence")
00183: 		{
00184: 			return true;
00185: 		}
00186: 		try
00187: 		{
00188: 			Purchase();
00189: 		}
00190: 		catch (Exception ex)
00191: 		{
00192: 			Core.Log?.Error("Express evidence removal failed: " + ex);
00193: 		}
00194: 		return false;
00195: 	}
00196: 
00197: 	internal static bool Purchase()
00198: 	{
00199: 		if (!Core.StoreReady || !WildNetworkUI.IsOpen || _selection != "xiaowo_auction_express_evidence" || !IsServiceTab() || SelectionField?.GetValue(null) as string != "xiaowo_auction_express_evidence" || !Object.op_Implicit((Object)(object)_frame) || !((Component)_frame.useButton).gameObject.activeInHierarchy || !CanPurchase())
00200: 		{
00201: 			return false;
00202: 		}
00203: 		PlayerStore instance = PlayerStore.Instance;
00204: 		if (instance.secData == null)
00205: 		{
00206: 			return false;
00207: 		}
00208: 		NetworkUpgrade upgrade = GetUpgrade(instance);
00209: 		int num = CurrentPrice();
00210: 		upgrade.Unlock();
00211: 		instance.ModCash(-num, true);
00212: 		AuctionState.Write("express_evidence_day", AuctionCatalog.Day.ToString());
00213: 		RunStats.OnWildUpgradeBought(num);
00214: 		if (Object.op_Implicit((Object)(object)EmporiumAudioPlayer.Instance))
00215: 		{
00216: 			EmporiumAudioPlayer.Instance.PlayRegister();
00217: 		}
00218: 		WildNetworkUI.RefreshAll();
00219: 		Core.Log?.Msg("Express evidence removal purchased for " + num + " credits.");
00220: 		return true;
00221: 	}
00222: 
00223: 	private static NetworkUpgrade GetUpgrade(PlayerStore store)
00224: 	{
00225: 		if (store.networkUpgrade == null || !store.networkUpgrade.TryGetValue("EVIDENCE_REMOVAL", out var value))
00226: 		{
00227: 			return null;
00228: 		}
00229: 		return value;
00230: 	}
00231: 
00232: 	private static bool UsedToday()
00233: 	{
00234: 		if (int.TryParse(AuctionState.Read("express_evidence_day"), out var result))
00235: 		{
00236: 			return result == AuctionCatalog.Day;
00237: 		}
00238: 		return false;
00239: 	}
00240: 
00241: 	internal static void Dispose()
00242: 	{
00243: 		_frame = null;
00244: 		_selection = null;
~~~

### XIAOWONightAuction/AuctionItemRegistration.cs

~~~csharp
00001: using System;
00002: using UnityEngine;
00003: 
00004: namespace XIAOWONightAuction;
00005: 
00006: internal static class AuctionItemRegistration
00007: {
00008: 	internal static void Register(ModItemDirectory directory)
00009: 	{
00010: 		if ((Object)(object)directory == (Object)null)
00011: 		{
00012: 			return;
00013: 		}
00014: 		int num = 0;
00015: 		if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey("xiaowo_auction_container") && ((Directory<GameItem>)(object)directory).Add("xiaowo_auction_container", (Func<GameItem>)(() => AuctionContainer.Create(owned: false, ContainerCategory.Engineering, 1, Array.Empty<string>()))))
00016: 		{
00017: 			num++;
00018: 		}
00019: 		foreach (string key in AuctionExclusiveCatalog.Items.Keys)
00020: 		{
00021: 			if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey(key))
00022: 			{
00023: 				string captured = key;
00024: 				if (((Directory<GameItem>)(object)directory).Add(key, (Func<GameItem>)(() => AuctionExclusiveFactory.Create(captured, owned: false))))
00025: 				{
00026: 					num++;
00027: 				}
00028: 			}
00029: 		}
00030: 		if (!((Directory<GameItem>)(object)directory).factoryDictionary.ContainsKey("xiaowo_auction_phone_card"))
00031: 		{
00032: 			((Directory<GameItem>)(object)directory).Add("xiaowo_auction_phone_card", (Func<GameItem>)(() => AuctionPhone.CreateCard(owned: false)));
00033: 		}
00034: 		Core.Log?.Msg("Auction item directory registered: added=" + num + " total=" + (AuctionExclusiveCatalog.Items.Count + 1) + ".");
00035: 	}
00036: }
~~~

### XIAOWONightAuction/AuctionPhoneCardSpritePatch.cs

~~~csharp
00001: using System;
00002: using HarmonyLib;
00003: using UnityEngine;
00004: 
00005: namespace XIAOWONightAuction;
00006: 
00007: [HarmonyPatch(typeof(RenderHandler), "LoadFromAtlas", new Type[]
00008: {
00009: 	typeof(string),
00010: 	typeof(string)
00011: })]
00012: internal static class AuctionPhoneCardSpritePatch
00013: {
00014: 	private static bool Prefix(string __0, string __1, ref Sprite __result)
00015: 	{
00016: 		if (__0 != "XIAOWO/NightAuctionPhone")
00017: 		{
00018: 			return true;
00019: 		}
00020: 		__result = AuctionPhone.Sprite(__1);
00021: 		return false;
00022: 	}
00023: }
~~~

### XIAOWONightAuction/AuctionContainer.cs

~~~csharp
00073: 
00074: 	private static readonly string[] NativeLockTags = new string[5] { "LOCKED_CONTAINER_TAG", "LOCK_STATUS", "LOCK_ID", "LOCK_ID_SER", "SUPPLY_CRATE" };
00075: 
00076: 	internal static bool InternalFillActive { get; private set; }
00077: 
00078: 	internal static void RefreshOwnedText()
00079: 	{
00080: 		PlayerStore instance = PlayerStore.Instance;
00081: 		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
00082: 		if (list == null)
00083: 		{
00084: 			return;
00085: 		}
00086: 		foreach (GameItem item in list)
00087: 		{
00088: 			if (item != null && !(item.identifier != "xiaowo_auction_container"))
00089: 			{
00090: 				ContainerData data = ReadData(item);
00091: 				ApplyLocalizedText(item, data);
00092: 				if (Views.TryGetValue(ObjectIdentity.Key(item), out var value) && value.Window != null)
00093: 				{
00094: 					UpdateWindow(item, value);
00095: 				}
00096: 			}
00097: 		}
00098: 	}
00099: 
00100: 	internal static GameItem Create(bool owned, ContainerCategory category, int seed, IReadOnlyList<string> contents)
00101: 	{
00102: 		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
00103: 		GameItem obj = DirectoryMaster.Item("service_box", owned) ?? throw new InvalidOperationException("Native container template service_box is unavailable.");
00104: 		GeneralHelper.SetItemOwned(obj, owned);
00105: 		obj.identifier = "xiaowo_auction_container";
00106: 		string name = (obj.identifierName = AuctionCatalog.CategoryName(category));
00107: 		obj.name = name;
00108: 		obj.shortDescription = AuctionText.Get("container_description", "拍卖场购得的密封集装箱。可连续填充提取区，无法向内存放物品。");
00109: 		obj.flavorText = AuctionText.Get("container_flavor", "箱门只认拍卖行内部封签。");
00110: 		obj.longDescription = obj.shortDescription;
00111: 		obj.SetValue(100L);
00112: 		obj.unitCount = 1;
00113: 		ApplySprite(obj, category);
00114: 		obj.shape = new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build();
00115: 		obj.modifiedShape = obj.shape.Clone();
00116: 		ClearNativeLockState(obj);
00117: 		obj.EnableTag("CONTAINER_ITEM_TAG", false);
00118: 		SetData(obj, category, seed, 0, contents);
00119: 		ApplyLocalizedText(obj, ReadData(obj));
00120: 		obj.SetContentWindow((PixelWindow)null);
00121: 		((GameItemFunc)obj).onLoaded = delegate(GameItem loaded)
00122: 		{
00123: 			Repair(loaded);
00124: 		};
00125: 		obj.ValidateShapeState();
00126: 		return obj;
00127: 	}
00128: 
00129: 	internal static void Repair(GameItem item)
00130: 	{
00131: 		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
00132: 		if (item == null || item.identifier != "xiaowo_auction_container")
00133: 		{
00134: 			return;
00135: 		}
00136: 		ContainerData containerData = ReadData(item);
00137: 		if (!AuctionLoadLifecycle.IsLoading && AuctionCatalog.RepairRemainingContents(containerData.Category, containerData.Seed, containerData.Index, containerData.Contents))
00138: 		{
00139: 			SetData(item, containerData.Category, containerData.Seed, containerData.Index, containerData.Contents);
00140: 			Core.Log?.Warning("Replaced unavailable item identifiers in a saved auction container.");
00141: 		}
00142: 		if (!AuctionLoadLifecycle.IsLoading && containerData.Contents.Count > 0)
00143: 		{
00144: 			AuctionState.AcknowledgeDelivery(containerData.Seed);
00145: 		}
00146: 		ApplyLocalizedText(item, containerData);
00147: 		ApplySprite(item, containerData.Category);
00148: 		GridShape shape = item.shape;
00149: 		int num;
00150: 		if (shape != null && shape.width == 2 && item.shape.height == 2)
00151: 		{
00152: 			GridShape modifiedShape = item.modifiedShape;
00153: 			if (modifiedShape != null && modifiedShape.width == 2)
00154: 			{
00155: 				num = ((item.modifiedShape.height != 2) ? 1 : 0);
00156: 				if (num != 0)
00157: 				{
00158: 					goto IL_0102;
00159: 				}
00160: 				goto IL_012d;
00161: 			}
00162: 		}
00163: 		num = 1;
00164: 		goto IL_0102;
00165: 		IL_012d:
00166: 		ClearNativeLockState(item);
00167: 		item.SetContentWindow((PixelWindow)null);
00168: 		if (num != 0)
00169: 		{
00170: 			item.ValidateShapeState();
00171: 		}
00172: 		return;
00173: 		IL_0102:
00174: 		item.shape = new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build();
00175: 		item.modifiedShape = item.shape.Clone();
00176: 		goto IL_012d;
00177: 	}
00178: 
00179: 	private static void ApplyLocalizedText(GameItem item, ContainerData data)
00180: 	{
00181: 		string name = (item.identifierName = AuctionCatalog.CategoryName(data.Category));
00182: 		item.name = name;
00183: 		item.shortDescription = ((data.Index >= data.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", data.Contents.Count - data.Index));
00184: 		item.longDescription = item.shortDescription;
00185: 		item.flavorText = AuctionText.Get("container_flavor", "箱门只认拍卖行内部封签。");
00186: 	}
00187: 
00188: 	internal static void RepairForOpenIfNeeded(GameItem item)
00189: 	{
00190: 		if (item != null)
00191: 		{
00192: 			GridShape shape = item.shape;
00193: 			if (((shape != null) ? new int?(shape.width) : ((int?)null)) == 2 && item.shape.height == 2)
00194: 			{
00195: 				GridShape modifiedShape = item.modifiedShape;
00196: 				if (modifiedShape != null && modifiedShape.width == 2 && item.modifiedShape.height == 2 && !item.IsTag("SUPPLY_CRATE") && !item.IsTag("LOCKED_CONTAINER_TAG") && item.contentWindow == null && !(item.spriteAtlasPath != "xiaowo_night_auction"))
00197: 				{
00198: 					return;
00199: 				}
00200: 			}
00201: 		}
00202: 		Repair(item);
00203: 	}
00204: 
00205: 	private static void ClearNativeLockState(GameItem item)
00206: 	{
00207: 		string[] nativeLockTags = NativeLockTags;
00208: 		foreach (string text in nativeLockTags)
00209: 		{
00210: 			item.DisableTag(text, true);
00211: 			TagSystem state = item.state;
00212: 			if (state != null)
00213: 			{
00214: 				state.DisableTag(text);
00215: 			}
00216: 			TagSystem modifiedState = item.modifiedState;
00217: 			if (modifiedState != null)
00218: 			{
00219: 				modifiedState.DisableTag(text);
00220: 			}
00221: 		}
00222: 	}
00223: 
00224: 	private static void ApplySprite(GameItem item, ContainerCategory category)
00225: 	{
00226: 		OfficialAssets.SetSprite(item, AuctionCatalog.SpriteName(category));
00227: 	}
00228: 
00229: 	internal static string Describe(GameItem item)
00230: 	{
00231: 		ContainerData containerData = ReadData(item);
00232: 		return AuctionCatalog.CategoryName(containerData.Category) + " remaining=" + Math.Max(0, containerData.Contents.Count - containerData.Index) + "/" + containerData.Contents.Count + " index=" + containerData.Index + " seed=" + containerData.Seed;
00233: 	}
00234: 
00235: 	internal static string NextItemId(GameItem item)
00236: 	{
00237: 		ContainerData containerData = ReadData(item);
00238: 		if (containerData.Index >= containerData.Contents.Count)
00239: 		{
00240: 			return string.Empty;
00241: 		}
00242: 		return containerData.Contents[containerData.Index];
00243: 	}
00244: 
00245: 	private static WindowView EnsureWindow(GameItem item)
00246: 	{
00247: 		if (Views.TryGetValue(ObjectIdentity.Key(item), out var value) && value.Window != null && ObjectIdentity.Key(value.Window) != IntPtr.Zero)
00248: 		{
00249: 			UpdateWindow(item, value);
00250: 			return value;
00251: 		}
00252: 		return BuildWindow(item);
00253: 	}
00254: 
00255: 	private static WindowView BuildWindow(GameItem item)
00256: 	{
00257: 		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
00258: 		//IL_0022: Expected O, but got Unknown
00259: 		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
00260: 		//IL_002a: Expected O, but got Unknown
00261: 		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
00262: 		//IL_0033: Expected O, but got Unknown
00263: 		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
00264: 		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
00265: 		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
00266: 		//IL_0063: Expected O, but got Unknown
00267: 		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
00268: 		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
00269: 		//IL_00f3: Expected O, but got Unknown
00270: 		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
00271: 		//IL_0107: Expected O, but got Unknown
00272: 		ContainerData containerData = ReadData(item);
00273: 		GridPixelElement val = new GridPixelElement(1, 5, true);
00274: 		TagElement val2 = new TagElement(-1, true);
00275: 		TagElement val3 = new TagElement(-1, true);
00276: 		Attach(val, val2, 0, 0);
00277: 		Attach(val, val3, 0, 1);
00278: 		GameGridInventory val4 = new GameGridInventory(9, 9)
00279: 		{
00280: 			identifier = "xiaowo_auction_locked_intake",
00281: 			renderDynamicBackground = true
00282: 		};
00283: 		val4.ForceRecomputeDynamicBackground();
00284: 		ContainerHelper.AllowOnlyTaggedItemsOr((GameInventory)(object)val4, new string[1] { "XIAOWO_AUCTION_INTERNAL_ONLY" }, true);
00285: 		Attach(val, val4, 0, 2);
00286: 		TagElement val5 = new TagElement(-1, true).SetText(AuctionText.Get("take_only", "9×9 提取窗口 · 只能取出，禁止存入"), 10000, (ColorPalette)12822958);
00287: 		Attach(val, val5, 0, 3);
00288: 		ButtonElement val6 = new ButtonElement(AuctionText.Get("fill_output", "填充提取区"), 144, -1, (Action)delegate
00289: 		{
00290: 			Dispense(item);
00291: 		}, (Color)0, "interface_click");
00292: 		Attach(val, val6, 0, 4);
00293: 		PixelWindow val7 = new PixelWindow(true, (string)null, (WindowStyle)0);
00294: 		val7.SetTitle(AuctionCatalog.CategoryName(containerData.Category));
00295: 		val7.Attach((PixelElement)(object)val);
00296: 		WindowView windowView = new WindowView(item, val7, val4, val2, val3, val5, val6);
00297: 		Views[ObjectIdentity.Key(item)] = windowView;
00298: 		UpdateWindow(item, windowView);
00299: 		return windowView;
00300: 	}
00301: 
00302: 	internal static void OpenTransientWindow(GameItem item, Vector2 mousePosition)
00303: 	{
00304: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
00305: 		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
00306: 		if (!Core.IsShuttingDown && !AuctionLoadLifecycle.IsLoading)
00307: 		{
00308: 			WindowView windowView = EnsureWindow(item);
00309: 			RestoreOutput(windowView);
00310: 			windowView.Window.Show(mousePosition, Vector2.zero, true);
00311: 		}
00312: 	}
00313: 
00314: 	internal static void CaptureLoadedOutputs(PlayerStore store)
00315: 	{
00316: 		if (Core.IsShuttingDown || AuctionLoadLifecycle.IsLoading || Views.Count == 0 || store == null || ObjectIdentity.Key(store) == IntPtr.Zero)
00317: 		{
00318: 			return;
00319: 		}
00320: 		List<GameItem> list = store.FindAllItem(true);
00321: 		if (list == null)
00322: 		{
00323: 			return;
00324: 		}
00325: 		foreach (GameItem item in list)
00326: 		{
00327: 			if (!(((item != null) ? item.identifier : null) != "xiaowo_auction_container") && Views.TryGetValue(ObjectIdentity.Key(item), out var value) && value.OutputRestored)
00328: 			{
00329: 				CaptureOutput(value, store);
00330: 			}
00331: 		}
00332: 	}
00333: 
00334: 	internal static void ClearViews()
00335: 	{
00336: 		Views.Clear();
00337: 	}
00338: 
00339: 	private static void CaptureOutput(WindowView view, PlayerStore store = null)
00340: 	{
00341: 		if (Core.IsShuttingDown)
00342: 		{
00343: 			return;
00344: 		}
00345: 		if (store == null)
00346: 		{
00347: 			store = (PlayerStore.HasInstance ? PlayerStore.Instance : null);
00348: 		}
00349: 		if (store == null || ObjectIdentity.Key(store) == IntPtr.Zero)
00350: 		{
00351: 			throw new InvalidOperationException("Cannot snapshot auction cargo without an existing player store.");
00352: 		}
00353: 		OutputSnapshot outputSnapshot = new OutputSnapshot();
00354: 		TagState tagReadonly = view.Container.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
00355: 		outputSnapshot.Manifest = ((tagReadonly != null) ? tagReadonly.GetString() : null) ?? string.Empty;
00356: 		OutputSnapshot outputSnapshot2 = outputSnapshot;
00357: 		foreach (GameItem childItem in ((GameInventory)view.Output).childItems)
~~~

~~~csharp
00451: 				finally
00452: 				{
00453: 					InternalFillActive = false;
00454: 				}
00455: 			}
00456: 		}
00457: 		Core.Log?.Warning("Auction extraction snapshot no longer matches its repaired manifest; restoring remaining cargo.");
00458: 		view.OutputRestored = true;
00459: 		Dispense(view.Container);
00460: 	}
00461: 
00462: 	private static void Attach(GridPixelElement root, TagElement element, int x, int y)
00463: 	{
00464: 		root.Attach((PixelElement)(object)element, x, y);
00465: 	}
00466: 
00467: 	private static void Attach(GridPixelElement root, GameSlotInventory element, int x, int y)
00468: 	{
00469: 		root.Attach((PixelElement)(object)element, x, y);
00470: 	}
00471: 
00472: 	private static void Attach(GridPixelElement root, GameGridInventory element, int x, int y)
00473: 	{
00474: 		root.Attach((PixelElement)(object)element, x, y);
00475: 	}
00476: 
00477: 	private static void Attach(GridPixelElement root, ButtonElement element, int x, int y)
00478: 	{
00479: 		root.Attach((PixelElement)(object)element, x, y);
00480: 	}
00481: 
00482: 	private static void UpdateWindow(GameItem item, WindowView view)
00483: 	{
00484: 		ContainerData containerData = ReadData(item);
00485: 		int num = Math.Max(0, containerData.Contents.Count - containerData.Index);
00486: 		view.RemainingLabel.SetText(AuctionText.Get("remaining_count", "剩余物品：{0} / {1}", num, containerData.Contents.Count), 10000, (ColorPalette)12822958);
00487: 		view.HintLabel.SetText((num > 0) ? AuctionText.Get("fill_output_hint", "将持续提取，直到 9×9 窗口放不下") : AuctionText.Get("container_empty_short", "集装箱已经取空"), 10000, (ColorPalette)12822958);
00488: 		view.TakeOnlyLabel.SetText(AuctionText.Get("take_only", "9×9 提取窗口 · 只能取出，禁止存入"), 10000, (ColorPalette)12822958);
00489: 		view.Button.Set((num > 0) ? AuctionText.Get("fill_output", "填充提取区") : AuctionText.Get("empty_button", "已取空"), 144, -1);
00490: 		view.Window.SetTitle(AuctionCatalog.CategoryName(containerData.Category));
00491: 		view.Output.ForceRecomputeDynamicBackground();
00492: 		view.Window.Validate();
00493: 		CenterButtonText(view.Button);
00494: 	}
00495: 
00496: 	private static void CenterButtonText(ButtonElement button)
00497: 	{
00498: 		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
00499: 		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
00500: 		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
00501: 		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
00502: 		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
00503: 		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
00504: 		object? obj = AccessTools.Field(typeof(ButtonElement), "buttonNode")?.GetValue(button);
00505: 		object? obj2 = ((obj is TreeNodeRender) ? obj : null);
00506: 		RectTransform val = ((obj2 != null) ? ((TreeNodeRender)obj2).rectTransform : null);
00507: 		object? obj3 = AccessTools.Field(typeof(ButtonElement), "textNode")?.GetValue(button);
00508: 		TreeNodeRender val2 = (TreeNodeRender)((obj3 is TreeNodeRender) ? obj3 : null);
00509: 		RectTransform val3 = ((val2 != null) ? val2.rectTransform : null);
00510: 		if (!((Object)(object)val == (Object)null) && !((Object)(object)val3 == (Object)null))
00511: 		{
00512: 			((Transform)val3).SetParent((Transform)(object)val, false);
00513: 			val3.anchorMin = Vector2.zero;
00514: 			val3.anchorMax = Vector2.one;
00515: 			val3.pivot = new Vector2(0.5f, 0.5f);
00516: 			val3.anchoredPosition = Vector2.zero;
00517: 			val3.sizeDelta = Vector2.zero;
00518: 			((Transform)val3).localScale = Vector3.one;
00519: 			if ((Object)(object)val2.text != (Object)null)
00520: 			{
00521: 				((TMP_Text)val2.text).alignment = (TextAlignmentOptions)514;
00522: 			}
00523: 		}
00524: 	}
00525: 
00526: 	internal static void Dispense(GameItem container)
00527: 	{
00528: 		if (Core.IsShuttingDown || AuctionLoadLifecycle.IsLoading)
00529: 		{
00530: 			return;
00531: 		}
00532: 		try
00533: 		{
00534: 			WindowView windowView = EnsureWindow(container);
00535: 			RestoreOutput(windowView);
00536: 			ContainerData containerData = ReadData(container);
00537: 			if (containerData.Index >= containerData.Contents.Count)
00538: 			{
00539: 				UpdateWindow(container, windowView);
00540: 				return;
00541: 			}
00542: 			int num = 0;
00543: 			int num2 = OutputUnitCount(windowView.Output);
00544: 			int num3 = containerData.Index + num2;
00545: 			while (num3 < containerData.Contents.Count)
00546: 			{
00547: 				string text = containerData.Contents[num3];
00548: 				GameItem val = DirectoryMaster.Item(text, true);
00549: 				if (val == null)
00550: 				{
00551: 					throw new InvalidOperationException("物品目录中不存在 " + text);
00552: 				}
00553: 				val.unitCount = 1;
00554: 				GameItem val2 = val;
00555: 				bool flag = false;
00556: 				try
00557: 				{
00558: 					GeneralHelper.SetItemOwned(val, true);
00559: 					val.EnableTag("XIAOWO_AUCTION_INTERNAL_ONLY", false);
00560: 					try
00561: 					{
00562: 						InternalFillActive = true;
00563: 						SlotMarker val3 = windowView.Output.TryFindOneValidInventorySlot(val, false);
00564: 						if (val3 != null && val3.TryAcceptOnce(-1) > 0)
00565: 						{
00566: 							flag = true;
00567: 							if (val3.targetItem != null && ObjectIdentity.Key(val3.targetItem) != ObjectIdentity.Key(val))
00568: 							{
00569: 								val2 = val3.targetItem;
00570: 								val2.EnableTag("XIAOWO_AUCTION_INTERNAL_ONLY", false);
00571: 							}
00572: 						}
00573: 					}
00574: 					finally
00575: 					{
00576: 						InternalFillActive = false;
00577: 					}
00578: 				}
00579: 				finally
00580: 				{
00581: 					if (!flag)
00582: 					{
00583: 						ClearOutputSeal(val);
00584: 						GeneralHelper.SetItemOwned(val, false);
00585: 					}
00586: 				}
00587: 				if (!flag)
00588: 				{
00589: 					break;
00590: 				}
00591: 				if (val2.parentInventory == null && (val2.parents == null || val2.parents.Count == 0))
00592: 				{
00593: 					throw new InvalidOperationException("物品未正确写入 9×9 提取窗口：" + text);
00594: 				}
00595: 				AuctionCollection.Discover(text);
00596: 				num3++;
00597: 				num++;
00598: 			}
00599: 			container.shortDescription = ((containerData.Index >= containerData.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", containerData.Contents.Count - containerData.Index));
00600: 			container.longDescription = container.shortDescription;
00601: 			CaptureOutput(windowView);
00602: 			UpdateWindow(container, windowView);
00603: 			Core.Log?.Msg("Auction container staged " + num + " item(s) in the 9x9 extraction window (" + OutputUnitCount(windowView.Output) + " staged, " + containerData.Index + "/" + containerData.Contents.Count + " claimed).");
00604: 		}
00605: 		catch (Exception ex)
00606: 		{
00607: 			Core.Log?.Warning("Could not dispense auction container item: " + ex.Message);
00608: 		}
00609: 	}
00610: 
00611: 	internal static int OutputCount(GameItem container)
00612: 	{
00613: 		if (container == null || !Views.TryGetValue(ObjectIdentity.Key(container), out var value))
00614: 		{
00615: 			return -1;
00616: 		}
00617: 		return OutputCount(value.Output);
00618: 	}
00619: 
00620: 	internal static string OutputItemId(GameItem container)
00621: 	{
00622: 		if (container != null && Views.TryGetValue(ObjectIdentity.Key(container), out var value))
00623: 		{
00624: 			GameGridInventory output = value.Output;
00625: 			if (((output != null) ? ((GameInventory)output).childItems : null) != null && ((GameInventory)value.Output).childItems.Count != 0)
00626: 			{
00627: 				GameItem obj = ((GameInventory)value.Output).childItems[0];
00628: 				return ((obj != null) ? obj.identifier : null) ?? string.Empty;
00629: 			}
00630: 		}
00631: 		return string.Empty;
00632: 	}
00633: 
00634: 	internal static List<GameItem> OutputItemsForTest(GameItem container)
00635: 	{
00636: 		List<GameItem> list = new List<GameItem>();
00637: 		if (container != null && Views.TryGetValue(ObjectIdentity.Key(container), out var value))
00638: 		{
00639: 			GameGridInventory output = value.Output;
00640: 			if (((output != null) ? ((GameInventory)output).childItems : null) != null)
00641: 			{
00642: 				{
00643: 					foreach (GameItem childItem in ((GameInventory)value.Output).childItems)
00644: 					{
00645: 						if (childItem != null)
00646: 						{
00647: 							list.Add(childItem);
00648: 						}
00649: 					}
00650: 					return list;
00651: 				}
00652: 			}
00653: 		}
00654: 		return list;
00655: 	}
00656: 
00657: 	internal static string OutputDiagnostics(GameItem container)
00658: 	{
00659: 		List<string> list = new List<string>();
00660: 		foreach (GameItem item in OutputItemsForTest(container))
00661: 		{
00662: 			list.Add((item.identifier ?? "<null-id>") + ":uid=" + item.uniqueId + ",parent=" + ((item.parentInventory == null) ? "null" : (item.parentInventory.identifier ?? "<null-inventory-id>")));
00663: 		}
00664: 		return string.Join("\n", list);
00665: 	}
00666: 
00667: 	internal static void RemoveViewForTest(GameItem container)
00668: 	{
~~~

~~~csharp
00773: 			int num = Math.Max(1, item.unitCount);
00774: 			for (int i = 0; i < num; i++)
00775: 			{
00776: 				if (containerData.Index >= containerData.Contents.Count)
00777: 				{
00778: 					break;
00779: 				}
00780: 				int num2 = -1;
00781: 				for (int j = containerData.Index; j < containerData.Contents.Count; j++)
00782: 				{
00783: 					if (!(containerData.Contents[j] != item.identifier))
00784: 					{
00785: 						num2 = j;
00786: 						break;
00787: 					}
00788: 				}
00789: 				if (num2 < 0)
00790: 				{
00791: 					break;
00792: 				}
00793: 				List<string> contents = containerData.Contents;
00794: 				int index = containerData.Index;
00795: 				List<string> contents2 = containerData.Contents;
00796: 				int index2 = num2;
00797: 				string value = containerData.Contents[num2];
00798: 				string value2 = containerData.Contents[containerData.Index];
00799: 				contents[index] = value;
00800: 				contents2[index2] = value2;
00801: 				containerData.Index++;
00802: 			}
00803: 			SetData(value3.Container, containerData.Category, containerData.Seed, containerData.Index, containerData.Contents);
00804: 			if (containerData.Index >= containerData.Contents.Count)
00805: 			{
00806: 				AuctionState.RevealHistory(containerData.Seed);
00807: 			}
00808: 			value3.Container.shortDescription = ((containerData.Index >= containerData.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", containerData.Contents.Count - containerData.Index));
00809: 			value3.Container.longDescription = value3.Container.shortDescription;
00810: 			CaptureOutput(value3);
00811: 			UpdateWindow(value3.Container, value3);
00812: 			break;
00813: 		}
00814: 	}
00815: 
00816: 	private static int OutputCount(GameGridInventory output)
00817: 	{
00818: 		return ((output == null) ? ((int?)null) : ((GameInventory)output).childItems?.Count).GetValueOrDefault();
00819: 	}
00820: 
00821: 	private static int OutputUnitCount(GameGridInventory output)
00822: 	{
00823: 		if (((output != null) ? ((GameInventory)output).childItems : null) == null)
00824: 		{
00825: 			return 0;
00826: 		}
00827: 		int num = 0;
00828: 		foreach (GameItem childItem in ((GameInventory)output).childItems)
00829: 		{
00830: 			if (childItem != null)
00831: 			{
00832: 				num += Math.Max(1, childItem.unitCount);
00833: 			}
00834: 		}
00835: 		return num;
00836: 	}
00837: 
00838: 	private static ContainerData ReadData(GameItem item)
00839: 	{
00840: 		object obj;
00841: 		if (item == null)
00842: 		{
00843: 			obj = null;
00844: 		}
00845: 		else
00846: 		{
00847: 			TagState tagReadonly = item.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
00848: 			obj = ((tagReadonly != null) ? tagReadonly.GetString() : null);
00849: 		}
00850: 		string text = (string)obj;
00851: 		if (string.IsNullOrWhiteSpace(text))
00852: 		{
00853: 			return new ContainerData();
00854: 		}
00855: 		string[] array = text.Split('~');
00856: 		if (array.Length != 4)
00857: 		{
00858: 			return new ContainerData();
00859: 		}
00860: 		ContainerData containerData = new ContainerData();
00861: 		int.TryParse(array[0], out var result);
00862: 		int.TryParse(array[1], out containerData.Seed);
00863: 		int.TryParse(array[2], out containerData.Index);
00864: 		containerData.Category = (ContainerCategory)Math.Clamp(result, 0, 3);
00865: 		string[] array2 = array[3].Split('|');
00866: 		foreach (string text2 in array2)
00867: 		{
00868: 			if (!string.IsNullOrWhiteSpace(text2))
00869: 			{
00870: 				containerData.Contents.Add(text2);
00871: 			}
00872: 		}
00873: 		containerData.Index = Math.Clamp(containerData.Index, 0, containerData.Contents.Count);
00874: 		return containerData;
00875: 	}
00876: 
00877: 	private static void SetData(GameItem item, ContainerCategory category, int seed, int index, IReadOnlyList<string> contents)
~~~

### XIAOWONightAuction/AuctionPhoneNamePatch.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace XIAOWONightAuction;
00004: 
00005: [HarmonyPatch(typeof(StorePhoneClient), "GetLocalizedDisplayName")]
00006: internal static class AuctionPhoneNamePatch
00007: {
00008: 	private static bool Prefix(StorePhoneClient __instance, ref string __result)
00009: 	{
00010: 		if (__instance.dialogFuncId != "xiaowo_night_auction_phone")
00011: 		{
00012: 			return true;
00013: 		}
00014: 		__result = AuctionPhone.ContactName;
00015: 		return false;
00016: 	}
00017: }
~~~

### XIAOWONightAuction/AuctionCommissions.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Linq;
00004: 
00005: namespace XIAOWONightAuction;
00006: 
00007: internal static class AuctionCommissions
00008: {
00009: 	internal static AuctionOrder Active => AuctionProgressEncoding.Decode<AuctionOrder>(AuctionState.Read("order_active"));
00010: 
00011: 	private static HashSet<string> Used => AuctionProgressEncoding.Decode<HashSet<string>>(AuctionState.Read("orders_used"));
00012: 
00013: 	internal static bool Available(AuctionOrder order)
00014: 	{
00015: 		return !Used.Contains(order.Id);
00016: 	}
00017: 
00018: 	internal static List<AuctionOrder> Offers()
00019: 	{
00020: 		int day = AuctionCatalog.Day;
00021: 		int num = Math.Max(0, (day - 1) / 7);
00022: 		List<AuctionOrder> list = new List<AuctionOrder>();
00023: 		int category;
00024: 		for (category = 0; category < 4; category++)
00025: 		{
00026: 			Random random = new Random(AuctionPhoneRules.StableSeed((PlayerStore.Instance?.runID ?? "run") + ":order:" + num + ":" + category));
00027: 			AuctionOrder auctionOrder = new AuctionOrder
00028: 			{
00029: 				Id = num + "_" + category,
00030: 				Category = (ContainerCategory)category,
00031: 				Required = random.Next(3, 6),
00032: 				RewardCap = 2000
00033: 			};
00034: 			if (num % 3 == 2)
00035: 			{
00036: 				AuctionExclusiveSpec[] array = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Category == (ContainerCategory)category && spec.Tier == 1).ToArray();
00037: 				auctionOrder.ItemId = array[random.Next(array.Length)].Id;
00038: 				auctionOrder.Required = 1;
00039: 				auctionOrder.RewardCap = 2500;
00040: 			}
00041: 			list.Add(auctionOrder);
00042: 		}
00043: 		return list;
00044: 	}
00045: 
00046: 	internal static bool Accept(ContainerCategory category)
00047: 	{
00048: 		AuctionOrder active = Active;
00049: 		if (active.Id.Length > 0 && !active.Claimed && (AuctionOrderRules.Complete(active) || !AuctionOrderRules.Expired(active, AuctionCatalog.Day)))
00050: 		{
00051: 			return false;
00052: 		}
00053: 		AuctionOrder auctionOrder = Offers().First((AuctionOrder offer) => offer.Category == category);
00054: 		HashSet<string> used = Used;
00055: 		if (!used.Add(auctionOrder.Id))
00056: 		{
00057: 			return false;
00058: 		}
00059: 		auctionOrder.AcceptedDay = AuctionCatalog.Day;
00060: 		auctionOrder.Deadline = auctionOrder.AcceptedDay + 6;
00061: 		AuctionState.Write("orders_used", AuctionProgressEncoding.Encode(used));
00062: 		Save(auctionOrder);
00063: 		return true;
00064: 	}
00065: 
00066: 	internal static void Save(AuctionOrder order)
00067: 	{
00068: 		AuctionState.Write("order_active", AuctionProgressEncoding.Encode(order));
00069: 	}
00070: 
00071: 	internal static bool Claim()
00072: 	{
00073: 		AuctionOrder order = Active;
00074: 		if (!AuctionOrderRules.Complete(order) || order.Claimed)
00075: 		{
00076: 			return false;
00077: 		}
00078: 		order.Claimed = true;
00079: 		Save(order);
00080: 		return AuctionRewards.Pay(AuctionOrderRules.Reward(order), delegate
00081: 		{
00082: 			order.Claimed = false;
00083: 			Save(order);
00084: 		});
00085: 	}
00086: 
00087: 	internal static void Cancel()
00088: 	{
00089: 		AuctionOrder active = Active;
00090: 		if (!AuctionOrderRules.Complete(active) || active.Claimed)
00091: 		{
00092: 			AuctionState.Delete("order_active");
00093: 		}
00094: 	}
00095: 
00096: 	internal static void OnSale(StoreClient client, GameItem item, int paid)
00097: 	{
00098: 		if (client == null || item == null)
00099: 		{
00100: 			return;
00101: 		}
00102: 		AuctionOrder active = Active;
00103: 		if (!(client.identifier != AuctionMerchants.ClientId(active.Category)))
00104: 		{
00105: 			AuctionExclusiveSpec value;
00106: 			bool matchesCategory = (AuctionExclusiveCatalog.Items.TryGetValue(item.identifier, out value) ? (value.Category == active.Category) : AuctionMerchants.BuyingTags(active.Category).Any((string tag) => item.IsGameItemType(tag) || item.IsTag(tag)));
00107: 			if (AuctionOrderRules.Credit(active, AuctionCatalog.Day, active.Category, item.identifier, item.uniqueId, Math.Max(1, item.unitCount), paid, matchesCategory))
00108: 			{
00109: 				Save(active);
00110: 			}
00111: 		}
00112: 	}
00113: 
00114: 	internal static string Target(AuctionOrder order)
00115: 	{
00116: 		if (order.ItemId.Length <= 0 || !AuctionExclusiveCatalog.Items.TryGetValue(order.ItemId, out var value))
00117: 		{
00118: 			return AuctionPhone.CategoryCaption(order.Category);
00119: 		}
00120: 		return AuctionCollection.Name(value);
00121: 	}
00122: 
00123: 	internal static string Description(AuctionOrder order, bool active)
00124: 	{
00125: 		string text = Target(order) + "  " + (active ? (order.Progress + "/") : "") + order.Required + AuctionText.Select(" 件", " items");
00126: 		text = text + AuctionText.Select("\n售给：", "\nBuyer: ") + AuctionMerchants.DisplayName(order.Category);
00127: 		text = text + AuctionText.Select("\n额外奖励：有效成交金额的 20%，上限 ", "\nBonus: 20% of eligible sales, capped at ") + order.RewardCap + "¢";
00128: 		if (active)
00129: 		{
00130: 			text = text + AuctionText.Select("\n截止：第 ", "\nDeadline: day ") + order.Deadline + AuctionText.Select("天", "") + (AuctionOrderRules.Complete(order) ? AuctionText.Select(" · 已完成", " · Completed") : (AuctionOrderRules.Expired(order, AuctionCatalog.Day) ? AuctionText.Select(" · 已过期", " · Expired") : ""));
00131: 		}
00132: 		else if (order.ItemId.Length == 0)
00133: 		{
00134: 			text += AuctionText.Select("\n每件成交单价至少 100¢", "\nAt least 100¢ per unit");
00135: 		}
00136: 		return text;
00137: 	}
00138: 
00139: 	internal static Dialogue DialoguePage(ContainerCategory first = ContainerCategory.Medical)
00140: 	{
00141: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00142: 		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
00143: 		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
00144: 		AuctionOrder active = Active;
00145: 		Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, AuctionText.Select("本周采购委托", "Weekly Procurement Orders"));
00146: 		if (active.Id.Length > 0 && !active.Claimed && (AuctionOrderRules.Complete(active) || !AuctionOrderRules.Expired(active, AuctionCatalog.Day)))
00147: 		{
00148: 			val.SetText(AuctionPhone.ContactName, AuctionPhone.CategoryCaption(active.Category) + " " + active.Progress + "/" + active.Required + AuctionText.Select("，截止第 ", "; due day ") + active.Deadline + AuctionText.Select(" 天。", "."));
00149: 			if (AuctionOrderRules.Complete(active))
00150: 			{
00151: 				Dialogue response = AuctionPhone.Ending("");
00152: 				AuctionPhone.AddServiceChoice(val, AuctionText.Select("领取奖励 ", "Claim reward ") + AuctionOrderRules.Reward(active) + "¢", response, delegate
00153: 				{
00154: 					response.SetText(AuctionPhone.ContactName, Claim() ? AuctionText.Select("委托已结算。", "Order settled.") : AuctionText.Select("奖励已经领取。", "Reward already claimed."));
00155: 				});
00156: 			}
00157: 			else
00158: 			{
00159: 				Dialogue val2 = new Dialogue().SetText(AuctionPhone.ContactName, AuctionText.Select("取消后，本周这一单不能重新接取。", "A cancelled order cannot be accepted again this week."));
00160: 				AuctionPhone.AddServiceChoice(val2, AuctionText.Select("确认取消", "Confirm Cancel"), AuctionPhone.Ending(AuctionText.Select("委托已取消。", "Order cancelled.")), Cancel);
00161: 				AuctionPhone.AddServiceChoice(val2, AuctionText.Select("保留委托", "Keep Order"), AuctionPhone.Ending(""), null);
00162: 				AuctionPhone.AddServiceChoice(val, AuctionText.Select("取消委托", "Cancel Order"), val2, null);
00163: 			}
00164: 			AuctionPhone.AddServiceChoice(val, AuctionText.Select("结束通话", "End Call"), AuctionPhone.Ending(""), null);
00165: 			return val;
00166: 		}
00167: 		List<AuctionOrder> list = Offers();
00168: 		val.SetText(AuctionPhone.ContactName, AuctionText.Select("一次接取一单，7 天内完成，额外奖励 20%。", "One active order, 7 days to complete, 20% bonus."));
00169: 		for (int num = (int)first; num < (int)(first + 2); num++)
00170: 		{
00171: 			AuctionOrder order = list[num];
00172: 			if (Available(order))
00173: 			{
00174: 				Dialogue val3 = new Dialogue().SetText(AuctionPhone.ContactName, Target(order) + " ×" + order.Required + AuctionText.Select("，奖励上限 ", "; bonus cap ") + order.RewardCap + "¢");
00175: 				Dialogue response2 = AuctionPhone.Ending("");
00176: 				AuctionPhone.AddServiceChoice(val3, AuctionText.Select("接取委托", "Accept Order"), response2, delegate
00177: 				{
00178: 					response2.SetText(AuctionPhone.ContactName, Accept(order.Category) ? (AuctionText.Select("已接取，截止第 ", "Accepted; due day ") + Active.Deadline + AuctionText.Select(" 天。", ".")) : AuctionText.Select("该委托已接取，或已有其他委托。", "Order unavailable, or another order is active."));
00179: 				});
00180: 				AuctionPhone.AddServiceChoice(val3, AuctionText.Select("结束通话", "End Call"), AuctionPhone.Ending(""), null);
00181: 				AuctionPhone.AddServiceChoice(val, AuctionText.Select("查看：", "View: ") + AuctionPhone.CategoryCaption(order.Category), val3, null);
00182: 			}
00183: 		}
00184: 		AuctionPhone.AddServiceChoice(val, (first == ContainerCategory.Medical) ? AuctionText.Select("其他委托", "Other Orders") : AuctionText.Select("结束通话", "End Call"), (first == ContainerCategory.Medical) ? OtherPage() : AuctionPhone.Ending(AuctionText.Select("重新拨打可查看其他委托。", "Call again to see other orders.")), null);
00185: 		return val;
00186: 	}
00187: 
00188: 	private static Dialogue OtherPage()
00189: 	{
00190: 		return DialoguePage(ContainerCategory.Contraband);
00191: 	}
00192: }
~~~

### XIAOWONightAuction/AuctionPhone.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using UnityEngine;
00004: using UnityEngine.SceneManagement;
00005: 
00006: namespace XIAOWONightAuction;
00007: 
00008: internal static class AuctionPhone
00009: {
00010: 	internal const string CardId = "xiaowo_auction_phone_card";
00011: 
00012: 	internal const string ContactId = "xiaowo_night_auction_phone";
00013: 
00014: 	internal const string IntroSource = "xiaowo_night_auction_phone_intro";
00015: 
00016: 	internal const long Number = 4367L;
00017: 
00018: 	internal const string Atlas = "XIAOWO/NightAuctionPhone";
00019: 
00020: 	private static int _tick;
00021: 
00022: 	private static bool _storeWorkPending;
00023: 
00024: 	private static int _storeWorkFailures;
00025: 
00026: 	private static bool _warned;
00027: 
00028: 	private static PaperUI _paper;
00029: 
00030: 	internal static string ContactName => AuctionText.Select("夜间拍卖行", "Night Auction House");
00031: 
00032: 	internal static bool CardGiven => AuctionState.Read("phone_card_given") == "1";
00033: 
00034: 	internal static int ReadyDay
00035: 	{
00036: 		get
00037: 		{
00038: 			if (!int.TryParse(AuctionState.Read("phone_ready_day"), out var result))
00039: 			{
00040: 				return 0;
00041: 			}
00042: 			return result;
00043: 		}
00044: 	}
00045: 
00046: 	internal static bool IsVisitDay(int day)
00047: 	{
00048: 		if (!(AuctionState.Read("phone_visit_day") == day.ToString()))
00049: 		{
00050: 			if (TryPending(out var dueDay, out var _))
00051: 			{
00052: 				return AuctionPhoneRules.VisitDue(day, dueDay);
00053: 			}
00054: 			return false;
00055: 		}
00056: 		return true;
00057: 	}
00058: 
00059: 	private static string VisitSource(int dueDay)
00060: 	{
00061: 		return "xiaowo_night_auction_phone_visit_" + dueDay;
00062: 	}
00063: 
00064: 	internal static void Tick()
00065: 	{
00066: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
00067: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
00068: 		if (!_storeWorkPending || _tick-- > 0)
00069: 		{
00070: 			return;
00071: 		}
00072: 		_tick = 60;
00073: 		Scene activeScene = SceneManager.GetActiveScene();
00074: 		if (((Scene)(ref activeScene)).name != "InventoryScene" || (Object)(object)EmporiumEntry.Instance == (Object)null)
00075: 		{
00076: 			return;
00077: 		}
00078: 		try
00079: 		{
00080: 			PlayerStore instance = PlayerStore.Instance;
00081: 			if (instance?.storeClientManager == null)
00082: 			{
00083: 				return;
00084: 			}
00085: 			if (!AuctionPhoneRules.Eligible(AuctionCatalog.Day))
00086: 			{
00087: 				_storeWorkPending = false;
00088: 				return;
00089: 			}
00090: 			if (!CardGiven && (AuctionCatalog.Day > 8 || AuctionState.Read("phone_card_pending") == "1"))
00091: 			{
00092: 				DeliverCard();
00093: 			}
00094: 			if (CardGiven)
00095: 			{
00096: 				EnsureContact();
00097: 			}
00098: 			HandleVisitors(instance.storeClientManager);
00099: 			_warned = false;
00100: 			_storeWorkPending = false;
00101: 		}
00102: 		catch (Exception ex)
00103: 		{
00104: 			_storeWorkFailures++;
00105: 			if (!_warned)
00106: 			{
00107: 				Core.Log?.Warning("[拍卖电话] 待店铺初始化后重试: " + ex.Message);
00108: 			}
00109: 			_warned = true;
00110: 			if (_storeWorkFailures >= 3)
00111: 			{
00112: 				_storeWorkPending = false;
00113: 				Core.Log?.Error("[拍卖电话] 本次店铺事件重试已达上限；下次读档、醒来或回店时重新检查。" + ex);
00114: 			}
00115: 		}
00116: 	}
00117: 
00118: 	internal static void OnStoreReady()
00119: 	{
00120: 		_storeWorkPending = true;
00121: 		_storeWorkFailures = 0;
00122: 		_warned = false;
00123: 		_tick = 0;
00124: 		Tick();
00125: 	}
00126: 
00127: 	internal static GameItem CreateCard(bool owned)
00128: 	{
00129: 		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
00130: 		GameItem obj = DirectoryMaster.Item("card_pharma", owned) ?? throw new InvalidOperationException("Native business card is unavailable.");
00131: 		obj.identifier = "xiaowo_auction_phone_card";
00132: 		OfficialAssets.SetSprite(obj, "auction_card_closed");
00133: 		obj.shape = new GridShapeBuilder(2, 1).SetDataFill(2, 1, (byte)1).Build();
00134: 		obj.modifiedShape = obj.shape.Clone();
00135: 		LocalizeCard(obj);
00136: 		((GameItemFunc)obj).onLoaded = MonoCallbacks.Bind<Action<GameItem>>(new Action<GameItem>(LocalizeCard));
00137: 		obj.ValidateShapeState();
00138: 		return obj;
00139: 	}
00140: 
00141: 	private static void LocalizeCard(GameItem item)
00142: 	{
00143: 		string identifierName = (item.name = AuctionText.Select("夜间拍卖行名片", "Night Auction Business Card"));
00144: 		item.identifierName = identifierName;
00145: 		identifierName = (item.longDescription = AuctionText.Select("电话：4367。四类收货商仅接受电话预约，次日到店。通常第3天可再预约；预约核心伙伴时缩短为第2天。赠送对应类别的礼物可提升合作关系。", "Phone: 4367. Four specialist buyers visit only by appointment, arriving tomorrow. Book again after 3 days, or 2 days when booking a Trusted Partner. Give matching gifts to improve relations."));
00146: 		item.shortDescription = identifierName;
00147: 		item.flavorText = AuctionText.Select("有货要出，拨这个号码。", "Have goods to sell? Call this number.");
00148: 	}
00149: 
00150: 	internal static void RefreshOwnedText()
00151: 	{
00152: 		PlayerStore instance = PlayerStore.Instance;
00153: 		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
00154: 		if (list == null)
00155: 		{
00156: 			return;
00157: 		}
00158: 		foreach (GameItem item in list)
00159: 		{
00160: 			if (((item != null) ? item.identifier : null) == "xiaowo_auction_phone_card")
00161: 			{
00162: 				LocalizeCard(item);
00163: 			}
00164: 		}
00165: 	}
00166: 
00167: 	internal static bool DeliverCard()
00168: 	{
00169: 		if (CardGiven)
00170: 		{
00171: 			return true;
00172: 		}
00173: 		PlayerStore instance = PlayerStore.Instance;
00174: 		List<GameItem> list = ((instance != null) ? instance.FindAllItem(true) : null);
00175: 		if (list == null || (Object)(object)EmporiumEntry.Instance == (Object)null)
00176: 		{
00177: 			return false;
00178: 		}
00179: 		foreach (GameItem item in list)
00180: 		{
00181: 			if (((item != null) ? item.identifier : null) == "xiaowo_auction_phone_card" && item.parentInventory != null)
00182: 			{
00183: 				FinishDelivery();
00184: 				return true;
00185: 			}
00186: 		}
00187: 		AuctionState.Write("phone_card_pending", "1");
00188: 		GameItem val = CreateCard(owned: true);
00189: 		try
00190: 		{
00191: 			EmporiumEntry.Instance.TryAddToPlayerInv(val);
00192: 			if (val.parentInventory == null && (val.parents == null || val.parents.Count == 0))
00193: 			{
00194: 				GeneralHelper.SetItemOwned(val, false);
00195: 				return false;
00196: 			}
00197: 			FinishDelivery();
00198: 			Core.Log?.Msg("[拍卖电话] 名片已发放，号码4367，占格2x1。");
00199: 			return true;
00200: 		}
00201: 		catch
00202: 		{
00203: 			if (val.parentInventory == null)
00204: 			{
00205: 				GeneralHelper.SetItemOwned(val, false);
00206: 			}
00207: 			throw;
00208: 		}
00209: 	}
00210: 
00211: 	private static void FinishDelivery()
00212: 	{
00213: 		AuctionState.Write("phone_card_given", "1");
00214: 		AuctionState.Delete("phone_card_pending");
00215: 		EnsureContact();
00216: 	}
00217: 
00218: 	internal static StorePhoneClient EnsureContact()
00219: 	{
00220: 		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
00221: 		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
00222: 		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
00223: 		//IL_0058: Expected O, but got Unknown
00224: 		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
00225: 		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
00226: 		PlayerStore instance = PlayerStore.Instance;
00227: 		if (instance == null || !CardGiven)
00228: 		{
00229: 			return null;
00230: 		}
00231: 		PlayerStore val = instance;
~~~

~~~csharp
00239: 			{
00240: 				dialogFuncId = "xiaowo_night_auction_phone",
00241: 				locID = "xiaowo_night_auction_phone"
00242: 			};
00243: 			instance.PhoneClientDict.Add(4367L, value);
00244: 		}
00245: 		if (value.dialogFuncId != "xiaowo_night_auction_phone")
00246: 		{
00247: 			throw new InvalidOperationException("电话号码4367已被其他联系人占用。");
00248: 		}
00249: 		value.displayName = ContactName;
00250: 		value.locID = "xiaowo_night_auction_phone";
00251: 		value.phoneClientType = (PhoneClientType)0;
00252: 		value.cooldownDuration = ((int.TryParse(AuctionState.Read("phone_call_interval"), out var result) && result == 2) ? 2 : 3);
00253: 		value.currentCooldown = Math.Max(0, ReadyDay - AuctionCatalog.Day);
00254: 		value.phoneState = (PhoneState)((value.currentCooldown > 0) ? 5 : 4);
00255: 		return value;
00256: 	}
00257: 
00258: 	internal static bool TryPending(out int dueDay, out ContainerCategory category)
00259: 	{
00260: 		dueDay = 0;
00261: 		category = ContainerCategory.Medical;
00262: 		string[] array = AuctionState.Read("phone_appointment").Split(',');
00263: 		if (array.Length != 2 || !int.TryParse(array[0], out dueDay) || !int.TryParse(array[1], out var result) || result < 0 || result > 3)
00264: 		{
00265: 			return false;
00266: 		}
00267: 		category = (ContainerCategory)result;
00268: 		return dueDay > 0;
00269: 	}
00270: 
00271: 	internal static bool Reserve(ContainerCategory category, out string message)
00272: 	{
00273: 		int day = AuctionCatalog.Day;
00274: 		if (!CardGiven || category < ContainerCategory.Medical || category > ContainerCategory.Engineering || !AuctionPhoneRules.CanReserve(day, ReadyDay, TryPending(out var dueDay, out var _)))
00275: 		{
00276: 			message = UnavailableText();
00277: 			return false;
00278: 		}
00279: 		string text = (day + 1).ToString();
00280: 		dueDay = (int)category;
00281: 		AuctionState.Write("phone_appointment", text + "," + dueDay);
00282: 		AuctionState.Write("phone_call_interval", AuctionMerchantRules.CallInterval(AuctionMerchantRelations.Gifts(category)).ToString());
00283: 		AuctionState.Write("phone_ready_day", AuctionPhoneRules.ReadyDay(day, AuctionMerchantRelations.Gifts(category)).ToString());
00284: 		EnsureContact();
00285: 		message = AuctionMerchants.DisplayName(category) + AuctionText.Select("明天到店，预算 ", " will visit tomorrow. Budget: ") + AuctionMerchantRelations.Budget(category).ToString("N0") + AuctionText.Select("¢。第", "¢. Call again on day ") + ReadyDay + AuctionText.Select("天恢复预约。", ".");
00286: 		Core.Log?.Msg("[拍卖电话] 预约 " + category.ToString() + "，到访日=" + (day + 1) + "，下次预约日=" + ReadyDay + ".");
00287: 		return true;
00288: 	}
00289: 
00290: 	private static string UnavailableText()
00291: 	{
00292: 		if (TryPending(out var dueDay, out var category))
00293: 		{
00294: 			return AuctionMerchants.DisplayName(category) + AuctionText.Select("已预约，第", " is booked for day ") + dueDay + AuctionText.Select("天到店。", ".");
00295: 		}
00296: 		return AuctionText.Select("采购团队还在周转，第", "Our buyers are busy. Call again on day ") + ReadyDay + AuctionText.Select("天恢复预约。", ".");
00297: 	}
00298: 
00299: 	internal static Dialogue BuildDialogue()
00300: 	{
00301: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00302: 		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
00303: 		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
00304: 		EnsureContact();
00305: 		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行服务。请选择：", "Night Auction House services. Please choose:"));
00306: 		AddServiceChoice(val, AuctionText.Select("预约收货商人", "Book a Buyer"), BuildBookingDialogue(), null);
00307: 		AddServiceChoice(val, AuctionText.Select("采购委托", "Procurement Orders"), AuctionCommissions.DialoguePage(), null);
00308: 		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("顾问与档案服务。", "Advisors and records."));
00309: 		Dialogue val3 = new Dialogue().SetText(ContactName, AuctionText.Select("档案服务。", "Records and collections."));
00310: 		AddServiceChoice(val3, AuctionText.Select("藏品收集册", "Collection Book"), Ending(string.Empty), AuctionController.RequestPhoneCollection);
00311: 		AddServiceChoice(val3, AuctionText.Select("合作商人档案", "Buyer Relationships"), Ending(string.Empty), AuctionController.RequestPhoneRelationships);
00312: 		AddServiceChoice(val3, AuctionText.Select("返回", "Back"), val2, null);
00313: 		AddServiceChoice(val2, AuctionText.Select("拍卖行顾问", "Auction Advisors"), AuctionNightMarket.BuildAdvisorDialogue(val2), null);
00314: 		AddServiceChoice(val2, AuctionText.Select("藏品 / 商人档案", "Collection / Buyer Records"), val3, null);
00315: 		AddServiceChoice(val2, AuctionText.Select("返回", "Back"), val, null);
00316: 		AddServiceChoice(val, AuctionText.Select("顾问 / 档案服务", "Advisors / Records"), val2, null);
00317: 		return val;
00318: 	}
00319: 
00320: 	internal static Dialogue BuildBookingDialogue()
00321: 	{
00322: 		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
00323: 		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
00324: 		EnsureContact();
00325: 		if (!AuctionPhoneRules.CanReserve(AuctionCatalog.Day, ReadyDay, TryPending(out var _, out var _)))
00326: 		{
00327: 			return Ending(UnavailableText());
00328: 		}
00329: 		Dialogue val = new Dialogue().SetText(ContactName, AuctionText.Select("夜间拍卖行收货专线。需要哪一类采购员？确认后明天到店。", "Night Auction buying desk. Which specialist do you need? Confirm a booking for tomorrow."));
00330: 		Dialogue val2 = new Dialogue().SetText(ContactName, AuctionText.Select("其他采购类别。", "Other buying categories."));
00331: 		AddBooking(val, ContainerCategory.Medical);
00332: 		AddBooking(val, ContainerCategory.Luxury);
00333: 		AddPhoneChoice(val, AuctionText.Select("违禁品类 / 工程类", "Contraband / Engineering"), val2);
00334: 		AddBooking(val2, ContainerCategory.Contraband);
00335: 		AddBooking(val2, ContainerCategory.Engineering);
00336: 		AddPhoneChoice(val2, AuctionText.Select("返回", "Back"), val);
00337: 		return val;
00338: 	}
00339: 
00340: 	private static void AddBooking(Dialogue dialogue, ContainerCategory category)
00341: 	{
00342: 		Dialogue confirmation = Ending(string.Empty);
00343: 		Action action = MonoCallbacks.Bind<Action>((Action)delegate
00344: 		{
00345: 			Reserve(category, out var message);
00346: 			confirmation.SetText(ContactName, message);
00347: 		});
00348: 		AddPhoneChoice(dialogue, CategoryCaption(category) + " · " + AuctionMerchantRelations.TierName(category), confirmation, action);
00349: 	}
00350: 
00351: 	internal static string CategoryCaption(ContainerCategory category)
00352: 	{
00353: 		return category switch
00354: 		{
00355: 			ContainerCategory.Medical => AuctionText.Select("医疗类", "Medical goods"), 
00356: 			ContainerCategory.Luxury => AuctionText.Select("奢侈品类", "Luxury goods"), 
00357: 			ContainerCategory.Contraband => AuctionText.Select("违禁品类", "Contraband"), 
00358: 			_ => AuctionText.Select("工程类", "Engineering goods"), 
00359: 		};
00360: 	}
00361: 
00362: 	private static void AddPhoneChoice(Dialogue dialogue, string caption, Dialogue next, Action action = null)
00363: 	{
00364: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00365: 		Dialogue val = new Dialogue().SetText("PLAYER", caption).SetNextDialogue(next);
00366: 		dialogue.AddChoice(ItemDirectory.CreateEmptyItem((string)null), caption, "", val, action, (Func<bool>)null);
00367: 	}
00368: 
00369: 	internal static void AddServiceChoice(Dialogue dialogue, string caption, Dialogue next, Action action)
00370: 	{
00371: 		AddPhoneChoice(dialogue, caption, next, (action == null) ? null : MonoCallbacks.Bind<Action>(action));
00372: 	}
00373: 
00374: 	internal static Dialogue Ending(string text)
00375: 	{
00376: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00377: 		Dialogue obj = new Dialogue().SetText(ContactName, text);
00378: 		obj.SetEndAction(MonoCallbacks.Bind<Action>((Action)delegate
00379: 		{
00380: 			PhoneUIManager instance = PhoneUIManager.Instance;
00381: 			if (instance != null)
00382: 			{
00383: 				instance.OnPhoneDialogEnd(true);
00384: 			}
00385: 		}));
00386: 		return obj;
00387: 	}
00388: 
00389: 	internal static void HandleVisitors(StoreClientManager manager)
00390: 	{
00391: 		int day = AuctionCatalog.Day;
00392: 		if (manager?.clientStack == null || !AuctionPhoneRules.Eligible(day))
00393: 		{
00394: 			return;
00395: 		}
00396: 		AuctionMerchants.HandleDailyVisitors(manager);
00397: 		if (!CardGiven && day == 8)
00398: 		{
00399: 			if (!ContainsSource(manager, "xiaowo_night_auction_phone_intro"))
00400: 			{
00401: 				manager.AddClient(AuctionMerchants.CreateClient(manager, AuctionPhoneRules.Presenter(PlayerStore.Instance.runID), "xiaowo_night_auction_phone_intro"));
00402: 			}
00403: 			if (ContainsSource(manager, "xiaowo_night_auction_phone_intro"))
00404: 			{
00405: 				AuctionState.Write("phone_intro_queued", "1");
00406: 			}
00407: 		}
00408: 		if (TryPending(out var dueDay, out var category) && AuctionPhoneRules.VisitDue(day, dueDay))
00409: 		{
00410: 			string text = VisitSource(dueDay);
00411: 			if (AuctionState.Read("phone_visit_delivered") == text)
00412: 			{
00413: 				AuctionState.Write("phone_visit_day", day.ToString());
00414: 				AuctionState.Delete("phone_appointment");
00415: 			}
00416: 			else
00417: 			{
00418: 				AuctionState.Write("phone_visit_day", day.ToString());
00419: 				PlayerStore instance = PlayerStore.Instance;
00420: 				object obj;
00421: 				if (instance == null)
00422: 				{
00423: 					obj = null;
00424: 				}
00425: 				else
00426: 				{
00427: 					StoreClientInstance currentClientInstance = instance.currentClientInstance;
00428: 					obj = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00429: 				}
00430: 				StoreClient val = (StoreClient)obj;
00431: 				bool flag = val?.eventSourceId == text;
00432: 				if (flag)
00433: 				{
00434: 					EnsureBookedCategory(val, manager, category, text);
00435: 				}
00436: 				for (int i = 0; i < manager.clientStack.Count; i++)
00437: 				{
00438: 					StoreClient val2 = manager.clientStack[i];
00439: 					if (!(val2?.eventSourceId != text))
00440: 					{
00441: 						if (flag)
00442: 						{
00443: 							manager.clientStack.RemoveAt(i--);
00444: 							continue;
00445: 						}
00446: 						EnsureBookedCategory(val2, manager, category, text);
00447: 						flag = true;
00448: 					}
00449: 				}
00450: 				if (!flag)
00451: 				{
00452: 					manager.AddClient(AuctionMerchants.CreateClient(manager, category, text));
00453: 				}
00454: 				if (ContainsSource(manager, text))
00455: 				{
00456: 					AuctionState.Write("phone_visit_queued", text);
00457: 				}
00458: 			}
00459: 		}
00460: 		AuctionMerchants.HandleDailyVisitors(manager);
00461: 	}
00462: 
00463: 	internal static void OnMerchantArrived(StoreClient client)
00464: 	{
00465: 		if (client == null || !TryPending(out var dueDay, out var category) || !AuctionPhoneRules.VisitDue(AuctionCatalog.Day, dueDay) || client.eventSourceId != VisitSource(dueDay))
00466: 		{
00467: 			return;
00468: 		}
00469: 		EnsureBookedCategory(client, PlayerStore.Instance?.storeClientManager, category, client.eventSourceId);
00470: 		if (client.identifier != AuctionMerchants.ClientId(category))
00471: 		{
00472: 			return;
00473: 		}
00474: 		AuctionState.Write("phone_visit_delivered", client.eventSourceId);
00475: 		AuctionState.Write("phone_visit_day", AuctionCatalog.Day.ToString());
00476: 		List<StoreClient> list = PlayerStore.Instance?.storeClientManager?.clientStack;
00477: 		if (list != null)
00478: 		{
00479: 			for (int num = list.Count - 1; num >= 0; num--)
00480: 			{
00481: 				if (list[num]?.eventSourceId == client.eventSourceId)
00482: 				{
00483: 					list.RemoveAt(num);
00484: 				}
00485: 			}
00486: 		}
00487: 		AuctionState.Delete("phone_appointment");
00488: 		Core.Log?.Msg("[拍卖电话] 预约商人已进店: " + client.displayName + ".");
00489: 	}
00490: 
00491: 	private static void EnsureBookedCategory(StoreClient client, StoreClientManager manager, ContainerCategory category, string source)
00492: 	{
00493: 		if (!(client.identifier == AuctionMerchants.ClientId(category)))
00494: 		{
00495: 			AuctionMerchants.ConfigureClient(client, manager, category, source);
00496: 			Core.Log?.Warning("[拍卖电话] 已纠正预约商人的类别: " + category.ToString() + ".");
00497: 		}
00498: 	}
00499: 
00500: 	private static bool ContainsSource(StoreClientManager manager, string source)
00501: 	{
00502: 		PlayerStore instance = PlayerStore.Instance;
00503: 		object obj;
00504: 		if (instance == null)
00505: 		{
00506: 			obj = null;
00507: 		}
00508: 		else
00509: 		{
00510: 			StoreClientInstance currentClientInstance = instance.currentClientInstance;
00511: 			obj = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.eventSourceId);
00512: 		}
00513: 		if ((string?)obj == source)
00514: 		{
00515: 			return true;
00516: 		}
00517: 		foreach (StoreClient item in manager.clientStack)
00518: 		{
00519: 			if (item?.eventSourceId == source)
00520: 			{
00521: 				return true;
00522: 			}
00523: 		}
00524: 		return false;
00525: 	}
00526: 
00527: 	internal static Dialogue WrapIntroduction(StoreClient client, Dialogue regular)
00528: 	{
00529: 		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
00530: 		if (client.eventSourceId != "xiaowo_night_auction_phone_intro" || CardGiven)
00531: 		{
00532: 			return regular;
00533: 		}
00534: 		return new Dialogue().SetText(client.displayName, AuctionText.Select("这是夜间拍卖行的名片，号码4367。打电话告诉我们你要出哪一类货，采购员第二天来；每次预约后要间隔两天。", "Here's our Night Auction card: 4367. Call to choose a buying specialist for tomorrow. Leave two days between bookings.")).SetStartAction(MonoCallbacks.Bind<Action>((Action)delegate
00535: 		{
00536: 			DeliverCard();
00537: 		})).SetNextDialogue(regular);
00538: 	}
00539: 
00540: 	internal static Sprite Sprite(string name)
00541: 	{
00542: 		return OfficialAssets.Get(name);
00543: 	}
00544: 
00545: 	internal static void OpenCard()
00546: 	{
00547: 		if (!Core.StoreReady)
00548: 		{
00549: 			return;
00550: 		}
00551: 		PaperUI paper = _paper;
00552: 		if (paper != null)
00553: 		{
00554: 			paper.Close();
00555: 		}
00556: 		_paper = ModHelper.CreatePaper("xiaowo_night_auction", "phone_card", "", 680f, 448f);
00557: 		_paper.Data.designWidth = 340f;
00558: 		_paper.Data.designHeight = 224f;
00559: 		_paper.SetChrome(true, false, false, false).AddPage((string)null, (string)null).AddImage(0f, 0f, 340f, 191f, "xiaowo_night_auction:auction_card_open", "fit", (string)null)
00560: 			.AddButton(100f, 196f, 140f, 24f, AuctionText.Select("拨打 4367", "Call 4367"), (Action)delegate
00561: 			{
00562: 				PaperUI paper2 = _paper;
00563: 				if (paper2 != null)
00564: 				{
00565: 					paper2.Hide();
00566: 				}
00567: 				EnsureContact();
00568: 				PhoneUIManager.Instance.OpenUI();
00569: 				PhoneUIManager.Instance.AutoCall(4367L);
00570: 			}, (string)null, (string)null);
00571: 		_paper.Show();
00572: 	}
00573: 
00574: 	internal static void Dispose()
00575: 	{
00576: 		PaperUI paper = _paper;
00577: 		if (paper != null)
00578: 		{
00579: 			paper.Close();
00580: 		}
00581: 		_paper = null;
00582: 		_storeWorkPending = false;
00583: 		_storeWorkFailures = 0;
00584: 	}
00585: }
~~~

### XIAOWONightAuction/AuctionMerchants.cs

~~~csharp
00154: 	{
00155: 		foreach (ContainerCategory value in Enum.GetValues(typeof(ContainerCategory)))
00156: 		{
00157: 			if (client?.identifier == ClientId(value))
00158: 			{
00159: 				category = value;
00160: 				return true;
00161: 			}
00162: 		}
00163: 		category = ContainerCategory.Medical;
00164: 		return false;
00165: 	}
00166: 
00167: 	internal static bool IsMatchingExclusive(StoreClient client, GameItem item)
00168: 	{
00169: 		if (client != null && item != null)
00170: 		{
00171: 			return IsMatchingExclusive(client.identifier, item.identifier);
00172: 		}
00173: 		return false;
00174: 	}
00175: 
00176: 	internal static bool IsMatchingExclusive(string clientId, string itemId)
00177: 	{
00178: 		if (!AuctionExclusiveCatalog.Items.TryGetValue(itemId ?? string.Empty, out var value))
00179: 		{
00180: 			return false;
00181: 		}
00182: 		return string.Equals(clientId, ClientId(value.Category), StringComparison.Ordinal);
00183: 	}
00184: 
00185: 	internal static string DisplayName(ContainerCategory category)
00186: 	{
00187: 		return category switch
00188: 		{
00189: 			ContainerCategory.Medical => AuctionText.Select("夜拍医疗采购员", "Night Auction Medic"), 
00190: 			ContainerCategory.Luxury => AuctionText.Select("夜拍藏品经纪", "Night Auction Curator"), 
00191: 			ContainerCategory.Contraband => AuctionText.Select("夜拍黑市掮客", "Night Auction Broker"), 
00192: 			_ => AuctionText.Select("夜拍工程承包商", "Night Auction Engineer"), 
00193: 		};
00194: 	}
00195: 
00196: 	internal static string[] BuyingTags(ContainerCategory category)
00197: 	{
00198: 		return category switch
00199: 		{
00200: 			ContainerCategory.Medical => new string[1] { "MEDICAL" }, 
00201: 			ContainerCategory.Luxury => new string[3] { "LUXURY_ITEM", "ALCOHOL", "WINE_CONTAINER_TAG" }, 
00202: 			ContainerCategory.Contraband => new string[4] { "CONTRABAND", "CONTRABAND_ITEM_TAG", "WEAPON", "FIREARM" }, 
00203: 			_ => new string[6] { "MACHINE", "STANDARD_MACHINE_TAG", "MODULE", "MODULE_TAG", "MATERIAL", "TOOL" }, 
00204: 		};
00205: 	}
00206: 
00207: 	private static string SelectUnusedSprite(StoreClientManager manager, ContainerCategory category)
00208: 	{
00209: 		List<string> list = SpriteCandidates(category);
00210: 		HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
00211: 		string text = null;
00212: 		if (manager != null)
00213: 		{
00214: 			try
00215: 			{
00216: 				PlayerStore instance = PlayerStore.Instance;
00217: 				object obj;
00218: 				if (instance == null)
00219: 				{
00220: 					obj = null;
00221: 				}
00222: 				else
00223: 				{
00224: 					StoreClientInstance currentClientInstance = instance.currentClientInstance;
00225: 					obj = ((currentClientInstance == null) ? null : currentClientInstance.GetClientBlueprint()?.spriteName);
00226: 				}
00227: 				text = (string)obj;
00228: 			}
00229: 			catch
00230: 			{
00231: 			}
00232: 		}
00233: 		if (!string.IsNullOrEmpty(text))
00234: 		{
00235: 			hashSet.Add(text);
00236: 		}
00237: 		List<StoreClient> list2 = manager?.clientStack;
00238: 		if (list2 != null)
00239: 		{
00240: 			for (int i = 0; i < list2.Count; i++)
00241: 			{
00242: 				string text2 = list2[i]?.spriteName;
00243: 				if (!string.IsNullOrEmpty(text2))
00244: 				{
00245: 					hashSet.Add(text2);
00246: 				}
00247: 			}
00248: 		}
00249: 		for (int num = list.Count - 1; num >= 0; num--)
00250: 		{
00251: 			if (!hashSet.Contains(list[num]))
00252: 			{
00253: 				return list[num];
00254: 			}
00255: 		}
00256: 		if (list.Count <= 0)
00257: 		{
00258: 			return FallbackSprite(category);
00259: 		}
00260: 		return list[list.Count - 1];
00261: 	}
~~~

~~~csharp
00380: 			client = null;
00381: 		}
00382: 		else
00383: 		{
00384: 			StoreClientInstance currentClientInstance = instance.currentClientInstance;
00385: 			client = ((currentClientInstance != null) ? currentClientInstance.GetClientBlueprint() : null);
00386: 		}
00387: 		RepairSavedBuyerFaction((StoreClient)client);
00388: 		List<StoreClient> list = manager?.clientStack;
00389: 		if (list != null)
00390: 		{
00391: 			for (int i = 0; i < list.Count; i++)
00392: 			{
00393: 				RepairSavedBuyerFaction(list[i]);
00394: 			}
00395: 		}
00396: 	}
00397: 
00398: 	private static void RepairSavedBuyerFaction(StoreClient client)
00399: 	{
00400: 		if (client != null && client.identifier == ClientId(ContainerCategory.Contraband) && client.clientFaction != "FACTION_BLACK_MARKET")
00401: 		{
00402: 			StoreClientFactionSetup.InitBlackMarket(client);
00403: 		}
00404: 	}
00405: 
00406: 	private static void ApplyDialogue(StoreClient client, ContainerCategory category)
00407: 	{
00408: 		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
00409: 		//IL_01b0: Expected O, but got Unknown
00410: 		string text = client.GetBudget().ToString("N0") + "¢";
00411: 		string[] array = category switch
00412: 		{
00413: 			ContainerCategory.Medical => new string[3]
00414: 			{
00415: 				AuctionText.Select("拍卖场介绍我来的。我只收医疗用品和药物。", "The auction house sent me. I buy medical supplies and medicine."),
00416: 				AuctionText.Select("货要能用，来路不用解释。我的预算是 ", "If it works, I won't ask where it came from. My budget is ") + text + AuctionText.Select("。", "."),
00417: 				AuctionText.Select("把合适的东西放上交易台，我们按原来的规矩成交。", "Put suitable goods on the counter. We'll trade as usual.")
00418: 			}, 
00419: 			ContainerCategory.Luxury => new string[3]
00420: 			{
00421: 				AuctionText.Select("我替收藏圈看货，只对真正的奢侈品和好酒感兴趣。", "I buy for collectors. Only genuine luxuries and fine wine interest me."),
00422: 				AuctionText.Select("今晚带了 ", "I brought ") + text + AuctionText.Select("，成色好的东西不会压价。", ". I pay fairly for goods in good condition."),
00423: 				AuctionText.Select("把藏品放上交易台，让我看看它值不值得带走。", "Put the collectible on the counter. Let me see it.")
00424: 			}, 
00425: 			ContainerCategory.Contraband => new string[3]
00426: 			{
00427: 				AuctionText.Select("拍卖场说你手里有些不能见光的货。", "The auction house says you have goods best kept out of sight."),
00428: 				AuctionText.Select("违禁品、武器和非法药物我都收，预算 ", "I buy contraband, weapons and illegal drugs. Budget: ") + text + AuctionText.Select("。", "."),
00429: 				AuctionText.Select("别谈来源，只谈价格。把东西放上来。", "No questions about origins. Put the goods on the counter.")
00430: 			}, 
00431: 			_ => new string[3]
00432: 			{
00433: 				AuctionText.Select("我负责替工程队收设备、模组和材料。", "I buy equipment, modules and materials for an engineering crew."),
00434: 				AuctionText.Select("预算 ", "Budget: ") + text + AuctionText.Select("，只要东西还能工作就可以谈。", ". If it still works, we can talk."),
00435: 				AuctionText.Select("把工程货放上交易台，我会逐件验收。", "Put your engineering goods on the counter. I'll check each one.")
00436: 			}, 
00437: 		};
00438: 		ref string reference = ref array[2];
00439: 		reference = reference + "\n" + AuctionMerchantRelations.Summary(category);
00440: 		Dialogue val = (client.mainDialogue = new Dialogue());
00441: 		for (int i = 0; i < array.Length; i++)
00442: 		{
00443: 			val.SetText(client.displayName, array[i]);
00444: 			if (i + 1 < array.Length)
00445: 			{
00446: 				val = val.NextDialogue();
00447: 			}
00448: 		}
00449: 		client.mainDialogue = AuctionPhone.WrapIntroduction(client, AuctionMerchantRelations.ArrivalDialogue(client, category, client.mainDialogue));
00450: 	}
00451: 
00452: 	private static bool ContainsEventSource(List<StoreClient> clients, string source)
00453: 	{
00454: 		if (clients == null)
00455: 		{
00456: 			return false;
00457: 		}
00458: 		for (int i = 0; i < clients.Count; i++)
00459: 		{
00460: 			if (string.Equals(clients[i]?.eventSourceId, source, StringComparison.Ordinal))
00461: 			{
00462: 				return true;
00463: 			}
00464: 		}
00465: 		return false;
00466: 	}
00467: }
~~~

### XIAOWONightAuction/AuctionPhoneCardActivatePatch.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace XIAOWONightAuction;
00004: 
00005: [HarmonyPatch(typeof(GameItem), "ActivateSlotItem")]
00006: internal static class AuctionPhoneCardActivatePatch
00007: {
00008: 	private static bool Prefix(GameItem __instance)
00009: 	{
00010: 		if (__instance.identifier != "xiaowo_auction_phone_card")
00011: 		{
00012: 			return true;
00013: 		}
00014: 		AuctionPhone.OpenCard();
00015: 		return false;
00016: 	}
00017: }
~~~

### XIAOWONightAuction/AuctionGhostIdentity.cs

~~~csharp
00017: 
00018: 	private static ConfirmPanelUIManager _dialog;
00019: 
00020: 	private static Action _confirmAction;
00021: 
00022: 	private static Action _cancelAction;
00023: 
00024: 	private static ConfirmPanelUIManager _labelDialog;
00025: 
00026: 	private static string _originalYes;
00027: 
00028: 	private static string _originalNo;
00029: 
00030: 	private static readonly List<(Behaviour Component, bool Enabled)> LabelLocalizers = new List<(Behaviour, bool)>();
00031: 
00032: 	internal static void OpenDialog(ConfirmPanelUIManager dialog, string body, Action yes = null, Action no = null, bool confirmation = false, string yesCaption = null)
00033: 	{
00034: 		dialog.OpenPanel(body, yes, no, false);
00035: 		_labelDialog = dialog;
00036: 		_originalYes = ((TMP_Text)dialog.yesTMP).text;
00037: 		_originalNo = ((TMP_Text)dialog.noTMP).text;
00038: 		TextMeshProUGUI[] array = (TextMeshProUGUI[])(object)new TextMeshProUGUI[2] { dialog.yesTMP, dialog.noTMP };
00039: 		for (int i = 0; i < array.Length; i++)
00040: 		{
00041: 			Behaviour[] components = ((Component)array[i]).GetComponents<Behaviour>();
00042: 			foreach (Behaviour val in components)
00043: 			{
00044: 				string fullName = ((object)val).GetType().FullName;
00045: 				if (!(fullName != "TMPLocalizer") || !(fullName != "UnityEngine.Localization.Components.LocalizeStringEvent"))
00046: 				{
00047: 					LabelLocalizers.Add((val, val.enabled));
00048: 					val.enabled = false;
00049: 				}
00050: 			}
00051: 		}
00052: 		((TMP_Text)dialog.yesTMP).text = yesCaption ?? (confirmation ? AuctionText.Select("使用", "Use") : AuctionText.Select("确定", "OK"));
00053: 		((TMP_Text)dialog.noTMP).text = (confirmation ? AuctionText.Select("取消", "Cancel") : AuctionText.Select("关闭", "Close"));
00054: 	}
00055: 
00056: 	internal static void RestoreLabels(ConfirmPanelUIManager dialog)
00057: 	{
00058: 		if ((Object)(object)_labelDialog == (Object)null || (Object)(object)_labelDialog != (Object)(object)dialog)
00059: 		{
00060: 			return;
00061: 		}
00062: 		if (Object.op_Implicit((Object)(object)_labelDialog))
00063: 		{
00064: 			((TMP_Text)_labelDialog.yesTMP).text = _originalYes;
00065: 			((TMP_Text)_labelDialog.noTMP).text = _originalNo;
00066: 		}
00067: 		foreach (var labelLocalizer in LabelLocalizers)
00068: 		{
00069: 			if ((Object)(object)labelLocalizer.Component != (Object)null && Object.op_Implicit((Object)(object)labelLocalizer.Component))
00070: 			{
00071: 				labelLocalizer.Component.enabled = labelLocalizer.Enabled;
00072: 			}
00073: 		}
00074: 		LabelLocalizers.Clear();
00075: 		_labelDialog = null;
00076: 	}
00077: 
00078: 	private static bool Check(GameItem item, out PlayerStore store, out int[] amounts, out string reason)
00079: 	{
00080: 		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
00081: 		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
00082: 		store = null;
00083: 		amounts = null;
00084: 		reason = AuctionText.Select("请在店铺内使用。", "Use this item in your shop.");
00085: 		Scene activeScene = SceneManager.GetActiveScene();
00086: 		if (((Scene)(ref activeScene)).name != "InventoryScene" || (Object)(object)EmporiumEntry.Instance == (Object)null)
00087: 		{
00088: 			return false;
00089: 		}
00090: 		store = PlayerStore.Instance;
00091: 		reason = AuctionText.Select("只能使用你拥有且尚未消耗的主板。", "You must own an unused board.");
00092: 		if (store == null || ((item != null) ? item.identifier : null) != "xiaowo_auction_ghost_identity" || item.IsTag("XIAOWO_GHOST_IDENTITY_USED") || !GeneralHelper.IsItemOwned(item))
00093: 		{
00094: 			return false;
00095: 		}
00096: 		List<GameItem> list = store.FindAllItem(true);
00097: 		bool flag = false;
00098: 		if (list != null)
00099: 		{
00100: 			foreach (GameItem item2 in list)
00101: 			{
00102: 				if (ObjectIdentity.Key(item2) == ObjectIdentity.Key(item))
00103: 				{
00104: 					flag = true;
00105: 					break;
00106: 				}
00107: 			}
00108: 		}
00109: 		if (!flag)
00110: 		{
00111: 			return false;
00112: 		}
00113: 		reason = AuctionText.Select("治安档案尚未就绪，主板未消耗。", "Security records are not ready. The board was not consumed.");
00114: 		SecData secData = store.secData;
00115: 		if (secData?.crimeAmount == null || secData.crimeTypes == null || secData.crimeAmount.Count != secData.crimeTypes.Count)
00116: 		{
00117: 			return false;
00118: 		}
00119: 		amounts = new int[secData.crimeAmount.Count];
00120: 		long num = 0L;
00121: 		for (int i = 0; i < amounts.Length; i++)
00122: 		{
00123: 			amounts[i] = secData.crimeAmount[i];
00124: 			if (amounts[i] < 0)
00125: 			{
00126: 				return false;
00127: 			}
00128: 			num += amounts[i];
00129: 		}
00130: 		if (num > int.MaxValue || secData.GetTotalCrimeValue() != num)
00131: 		{
00132: 			return false;
00133: 		}
00134: 		reason = AuctionText.Select("执法处置已启动，无法使用主板撤销。", "Enforcement has started and cannot be cancelled with this board.");
00135: 		if (secData.state != 0 || secData.evidenceLevel >= 100)
00136: 		{
00137: 			return false;
00138: 		}
00139: 		reason = AuctionText.Select("当前没有可清除的调查进度或犯罪金额，主板未消耗。", "No investigation progress or reducible crime amount. The board was not consumed.");
00140: 		return GhostIdentityRules.Available(secData.evidenceLevel, secData.state, num);
00141: 	}
00142: 
00143: 	internal static bool RequestUse(GameItem item)
00144: 	{
00145: 		ConfirmPanelUIManager dialog = ((IEnumerable<ConfirmPanelUIManager>)Object.FindObjectsOfType<ConfirmPanelUIManager>(true)).FirstOrDefault((Func<ConfirmPanelUIManager, bool>)delegate(ConfirmPanelUIManager candidate)
00146: 		{
00147: 			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
00148: 			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
00149: 			if ((Object)(object)candidate.panel != (Object)null)
00150: 			{
00151: 				Scene scene = ((Component)candidate).gameObject.scene;
00152: 				return ((Scene)(ref scene)).name == "InventoryScene";
00153: 			}
00154: 			return false;
00155: 		});
00156: 		if ((Object)(object)dialog == (Object)null || dialog.panel.activeInHierarchy)
00157: 		{
00158: 			return false;
00159: 		}
00160: 		if (!Check(item, out var store, out var amounts, out var reason))
00161: 		{
00162: 			OpenDialog(dialog, reason);
00163: 			return false;
00164: 		}
00165: 		_pending = item;
00166: 		_dialog = dialog;
00167: 		string run = store.runID;
00168: 		int slot = store.saveSlotId;
00169: 		IntPtr pointer = ObjectIdentity.Key(store);
00170: 		int num = amounts.Sum();
00171: 		string body = AuctionText.Select("使用幽灵身份主板？\n调查进度归零，涉嫌犯罪金额减少50%。\n金额：" + num + " -> " + (num - num / 2) + "¢\n消耗一件；不能撤销已启动的执法处置。", "Use Ghost Identity Board?\nReset investigation; halve crime amount.\nAmount: " + num + " -> " + (num - num / 2) + "¢\nUses 1 board. Cannot cancel enforcement.");
00172: 		_confirmAction = MonoCallbacks.Bind<Action>((Action)delegate
00173: 		{
00174: 			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
00175: 			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
00176: 			if (!(ObjectIdentity.Key(_pending) != ObjectIdentity.Key(item)))
00177: 			{
00178: 				_pending = null;
00179: 				Scene activeScene = SceneManager.GetActiveScene();
00180: 				if (!(((Scene)(ref activeScene)).name != "InventoryScene") && !(ObjectIdentity.Key(PlayerStore.Instance) != pointer) && PlayerStore.Instance.saveSlotId == slot && !(PlayerStore.Instance.runID != run))
00181: 				{
00182: 					try
00183: 					{
00184: 						TryUse(item, out var message);
00185: 						OpenDialog(dialog, message);
00186: 					}
00187: 					catch (Exception ex)
00188: 					{
00189: 						Core.Log?.Error("[幽灵身份主板] 使用失败: " + ex);
00190: 						OpenDialog(dialog, AuctionText.Select("使用失败，请查看日志。", "Use failed. Check the log."));
00191: 					}
00192: 				}
00193: 			}
00194: 		});
00195: 		_cancelAction = MonoCallbacks.Bind<Action>((Action)delegate
00196: 		{
00197: 			_pending = null;
00198: 		});
00199: 		OpenDialog(dialog, body, _confirmAction, _cancelAction, confirmation: true);
00200: 		return true;
00201: 	}
00202: 
00203: 	internal static bool TryUse(GameItem item, out string message)
00204: 	{
00205: 		if (!Check(item, out var store, out var amounts, out message))
00206: 		{
00207: 			return false;
00208: 		}
00209: 		SecData secData = store.secData;
00210: 		int[] array = GhostIdentityRules.Half(amounts);
00211: 		int evidenceLevel = secData.evidenceLevel;
00212: 		GameInventory parentInventory = item.parentInventory;
00213: 		if (parentInventory == null)
00214: 		{
00215: 			message = AuctionText.Select("请先将主板放入你的库存，未消耗。", "Place the board in your inventory first. It was not consumed.");
00216: 			return false;
00217: 		}
00218: 		GridShape shape = item.modifiedShape.Clone();
00219: 		bool flag = false;
00220: 		try
00221: 		{
00222: 			flag = parentInventory.Expel(item);
00223: 			if (!flag)
00224: 			{
00225: 				message = AuctionText.Select("无法从当前容器取出主板，未消耗。", "Cannot remove the board from this container. It was not consumed.");
00226: 				return false;
00227: 			}
00228: 			for (int i = 0; i < array.Length; i++)
00229: 			{
00230: 				secData.crimeAmount[i] = array[i];
00231: 			}
00232: 			secData.evidenceLevel = 0;
00233: 			item.EnableTag("XIAOWO_GHOST_IDENTITY_USED", true);
00234: 			GeneralHelper.SetItemOwned(item, false);
00235: 			item.Destroy();
00236: 		}
00237: 		catch
00238: 		{
00239: 			for (int j = 0; j < amounts.Length; j++)
00240: 			{
00241: 				secData.crimeAmount[j] = amounts[j];
00242: 			}
00243: 			secData.evidenceLevel = evidenceLevel;
00244: 			item.DisableTag("XIAOWO_GHOST_IDENTITY_USED", true);
00245: 			if (flag)
00246: 			{
00247: 				GeneralHelper.SetItemOwned(item, true);
00248: 				if (!NativePlacement.Restore(item, parentInventory, shape))
00249: 				{
00250: 					EmporiumEntry.Instance.TryAddToPlayerInv(item);
00251: 				}
00252: 			}
00253: 			throw;
00254: 		}
00255: 		message = AuctionText.Select("身份记录已处理。调查进度归零，涉嫌犯罪金额：", "Identity records cleared. Investigation progress reset; current crime amount: ") + amounts.Sum() + " -> " + array.Sum() + "¢";
00256: 		Core.Log?.Msg("[幽灵身份主板] 已消耗一件，犯罪金额 " + amounts.Sum() + " -> " + array.Sum() + ", 证据进度 " + evidenceLevel + " -> 0。");
00257: 		return true;
00258: 	}
00259: 
00260: 	internal static void Dispose()
00261: 	{
00262: 		if (_pending != null && (Object)(object)_dialog?.panel != (Object)null && _dialog.panel.activeInHierarchy)
00263: 		{
00264: 			_dialog.OnNoClick();
00265: 		}
00266: 		RestoreLabels(_labelDialog);
00267: 		_pending = null;
00268: 		_dialog = null;
00269: 		_confirmAction = (_cancelAction = null);
00270: 	}
00271: }
~~~

### XIAOWONightAuction/AuctionLoadLifecycle.cs

~~~csharp
00050: 	internal static void RequestNewGame()
00051: 	{
00052: 		BeginLoad();
00053: 		_newGame = true;
00054: 	}
00055: 
00056: 	internal static void Loaded(PlayerStore store)
00057: 	{
00058: 		Dispose();
00059: 		_store = store;
00060: 		_run = store.runID;
00061: 		_slot = store.saveSlotId;
00062: 		Generation++;
00063: 		_scan = (_warm = true);
00064: 		_retryAt = Time.frameCount + 1;
00065: 		AuctionState.MigrateKeys();
00066: 		Core.Log?.Msg($"[Load] generation={Generation}; slot={_slot}; run={_run}; official late callback.");
00067: 	}
00068: 
00069: 	internal static void Tick()
00070: 	{
00071: 		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
00072: 		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
00073: 		if (Core.IsShuttingDown || !PlayerStore.HasInstance)
00074: 		{
00075: 			return;
00076: 		}
00077: 		Scene activeScene = SceneManager.GetActiveScene();
00078: 		if (((Scene)(ref activeScene)).name != "InventoryScene")
00079: 		{
00080: 			return;
00081: 		}
00082: 		if (_newGame && (Object)(object)EmporiumEntry.Instance != (Object)null)
00083: 		{
00084: 			Loaded(PlayerStore.Instance);
00085: 			AuctionPhone.OnStoreReady();
00086: 		}
00087: 		if (IsLoading || _store == null)
00088: 		{
00089: 			return;
00090: 		}
00091: 		PlayerStore instance = PlayerStore.Instance;
00092: 		if (instance != _store || instance.runID != _run || instance.saveSlotId != _slot)
00093: 		{
00094: 			Dispose();
00095: 		}
00096: 		else
00097: 		{
00098: 			if (Time.frameCount < _retryAt)
00099: 			{
00100: 				return;
00101: 			}
00102: 			if (_scan)
00103: 			{
00104: 				try
00105: 				{
00106: 					_scanAttempts++;
00107: 					foreach (GameItem item in instance.FindAllItem(true))
00108: 					{
00109: 						if (item != null && (AuctionExclusiveCatalog.Items.ContainsKey(item.identifier) || item.identifier == "xiaowo_auction_container"))
00110: 						{
00111: 							Pending.Enqueue(new Work
00112: 							{
00113: 								Item = item,
00114: 								Id = item.identifier,
00115: 								UniqueId = item.uniqueId
00116: 							});
00117: 						}
00118: 					}
00119: 					_scan = false;
00120: 				}
00121: 				catch (Exception arg)
00122: 				{
00123: 					Pending.Clear();
00124: 					_scan = _scanAttempts < 3;
00125: 					_retryAt = Time.frameCount + 30;
00126: 					Core.Log?.Warning($"[Load] generation={Generation}; stage=snapshot; attempt={_scanAttempts}/3; {arg}");
00127: 					return;
00128: 				}
00129: 			}
00130: 			int num = Math.Min(8, Pending.Count);
00131: 			for (int i = 0; i < num; i++)
00132: 			{
00133: 				Work work = Pending.Dequeue();
00134: 				try
00135: 				{
00136: 					work.Attempts++;
00137: 					if (work.Item.identifier != work.Id || work.Item.uniqueId != work.UniqueId)
00138: 					{
00139: 						throw new InvalidOperationException("Item identity changed");
00140: 					}
00141: 					if (work.Id == "xiaowo_auction_container")
00142: 					{
00143: 						AuctionContainer.Repair(work.Item);
00144: 					}
00145: 					else
00146: 					{
00147: 						AuctionExclusiveFactory.Repair(work.Item);
00148: 					}
00149: 					if (GeneralHelper.IsItemEffectivelyOwned(work.Item))
00150: 					{
00151: 						AuctionCollection.Discover(work.Id);
00152: 					}
00153: 					if (AuctionExclusiveCatalog.Items.TryGetValue(work.Id, out var value) && AuctionExclusiveFactory.NeedsRepair(work.Item, value))
00154: 					{
~~~

### XIAOWONightAuction/AuctionExclusiveFactory.cs

~~~csharp
00226: 		if (shape != null && shape.width == spec.Width)
00227: 		{
00228: 			return shape.height == spec.Height;
00229: 		}
00230: 		return false;
00231: 	}
00232: 
00233: 	internal static void RefreshLocalizedText(GameItem item)
00234: 	{
00235: 		if (item != null && AuctionExclusiveCatalog.Items.TryGetValue(item.identifier, out var value))
00236: 		{
00237: 			ApplyLocalizedText(item, value);
00238: 		}
00239: 	}
00240: 
00241: 	internal static void RefreshOwnedText()
00242: 	{
00243: 		PlayerStore existing = NativeStore.Existing;
00244: 		List<GameItem> list = ((existing != null) ? existing.FindAllItem(true) : null);
00245: 		if (list == null)
00246: 		{
00247: 			return;
00248: 		}
00249: 		foreach (GameItem item in list)
00250: 		{
00251: 			RefreshLocalizedText(item);
00252: 		}
00253: 	}
00254: 
00255: 	private static void ApplyLocalizedText(GameItem item, AuctionExclusiveSpec spec)
00256: 	{
00257: 		int num;
00258: 		string text;
00259: 		if (AuctionExclusiveCatalog.English.TryGetValue(spec.Id, out (string, string) value))
00260: 		{
00261: 			num = ((!AuctionText.IsChinese(AuctionText.CurrentLocaleCode)) ? 1 : 0);
00262: 			if (num != 0)
00263: 			{
00264: 				(text, _) = value;
00265: 				goto IL_0035;
00266: 			}
00267: 		}
00268: 		else
00269: 		{
00270: 			num = 0;
00271: 		}
00272: 		text = spec.Name;
00273: 		goto IL_0035;
00274: 		IL_0035:
00275: 		string text2 = text;
00276: 		string text3 = ((num != 0) ? value.Item2 : spec.Description);
00277: 		if (spec.Id == "xiaowo_auction_forged_pass")
00278: 		{
00279: 			text3 = string.Format(text3, AuctionForgedPass.Remaining(item));
00280: 		}
00281: 		if (spec.Id == "xiaowo_auction_calibrator")
00282: 		{
00283: 			text3 = string.Format(text3, AuctionItemLinks.Remaining(item));
00284: 		}
00285: 		item.identifierName = text2;
00286: 		item.name = text2;
00287: 		item.shortDescription = text3;
00288: 		item.longDescription = text3;
00289: 		item.flavorText = AuctionText.Select("只在夜间集装箱拍卖中流通的稀有货物。", "Rare goods traded only at the night container auction.");
00290: 	}
00291: }
~~~

### XIAOWONightAuction/AuctionCollection.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Linq;
00004: using UnityEngine;
00005: using UnityEngine.SceneManagement;
00006: 
00007: namespace XIAOWONightAuction;
00008: 
00009: internal static class AuctionCollection
00010: {
00011: 	private static int _tick;
00012: 
00013: 	internal static HashSet<string> Found => AuctionProgressEncoding.Decode<HashSet<string>>(AuctionState.Read("collection_found"));
00014: 
00015: 	private static HashSet<string> Claimed => AuctionProgressEncoding.Decode<HashSet<string>>(AuctionState.Read("collection_claimed"));
00016: 
00017: 	internal static string Name(AuctionExclusiveSpec spec)
00018: 	{
00019: 		if (AuctionText.IsChinese(AuctionText.CurrentLocaleCode) || !AuctionExclusiveCatalog.English.TryGetValue(spec.Id, out (string, string) value))
00020: 		{
00021: 			return spec.Name;
00022: 		}
00023: 		return value.Item1;
00024: 	}
00025: 
00026: 	internal static void Discover(string id)
00027: 	{
00028: 		if (AuctionExclusiveCatalog.IsExclusive(id) && PlayerStore.Instance != null)
00029: 		{
00030: 			HashSet<string> found = Found;
00031: 			if (found.Add(id))
00032: 			{
00033: 				AuctionState.Write("collection_found", AuctionProgressEncoding.Encode(found));
00034: 			}
00035: 		}
00036: 	}
00037: 
00038: 	internal static void ScanOwned()
00039: 	{
00040: 		if (PlayerStore.Instance == null)
00041: 		{
00042: 			return;
00043: 		}
00044: 		HashSet<string> found = Found;
00045: 		bool flag = false;
00046: 		foreach (GameItem item in PlayerStore.Instance.FindAllItem(true))
00047: 		{
00048: 			if (item != null && AuctionExclusiveCatalog.IsExclusive(item.identifier) && GeneralHelper.IsItemEffectivelyOwned(item))
00049: 			{
00050: 				flag |= found.Add(item.identifier);
00051: 			}
00052: 		}
00053: 		if (flag)
00054: 		{
00055: 			AuctionState.Write("collection_found", AuctionProgressEncoding.Encode(found));
00056: 		}
00057: 	}
00058: 
00059: 	internal static void Tick()
00060: 	{
00061: 		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
00062: 		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
00063: 		if (_tick-- > 0)
00064: 		{
00065: 			return;
00066: 		}
00067: 		_tick = 600;
00068: 		Scene activeScene = SceneManager.GetActiveScene();
00069: 		if (((Scene)(ref activeScene)).name != "InventoryScene" || (Object)(object)EmporiumEntry.Instance == (Object)null)
00070: 		{
00071: 			return;
00072: 		}
00073: 		try
00074: 		{
00075: 			ScanOwned();
00076: 		}
00077: 		catch (Exception ex)
00078: 		{
00079: 			Core.Log?.Warning("Auction collection scan deferred: " + ex.Message);
00080: 		}
00081: 	}
00082: 
00083: 	internal static int Count(ContainerCategory category)
00084: 	{
00085: 		return AuctionExclusiveCatalog.Items.Values.Count((AuctionExclusiveSpec spec) => spec.Category == category && Found.Contains(spec.Id));
00086: 	}
00087: 
00088: 	internal static bool IsClaimed(int category)
00089: 	{
00090: 		return Claimed.Contains(category.ToString());
00091: 	}
00092: 
00093: 	internal static bool CanClaim(int category)
00094: 	{
00095: 		if (category < 0 || category > 4 || Claimed.Contains(category.ToString()))
00096: 		{
00097: 			return false;
00098: 		}
00099: 		HashSet<string> found = Found;
00100: 		return AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => category == 4 || spec.Category == (ContainerCategory)category).All((AuctionExclusiveSpec spec) => found.Contains(spec.Id));
00101: 	}
00102: 
00103: 	internal static bool Claim(int category)
00104: 	{
00105: 		if (!CanClaim(category))
00106: 		{
00107: 			return false;
00108: 		}
00109: 		HashSet<string> claimed = Claimed;
00110: 		claimed.Add(category.ToString());
00111: 		AuctionState.Write("collection_claimed", AuctionProgressEncoding.Encode(claimed));
00112: 		return AuctionRewards.Pay((category == 4) ? 5000 : 2000, delegate
00113: 		{
00114: 			claimed.Remove(category.ToString());
00115: 			AuctionState.Write("collection_claimed", AuctionProgressEncoding.Encode(claimed));
00116: 		});
00117: 	}
00118: 
00119: 	internal static Dialogue DialoguePage(bool second = false)
00120: 	{
00121: 		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
00122: 		int num = (second ? 2 : 0);
00123: 		Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, string.Join(" / ", from i in Enumerable.Range(num, 2)
00124: 			select AuctionPhone.CategoryCaption((ContainerCategory)i) + " " + Count((ContainerCategory)i) + "/" + AuctionExclusiveCatalog.CategoryCount((ContainerCategory)i)));
00125: 		for (int num2 = num; num2 < num + 2; num2++)
00126: 		{
00127: 			int category = num2;
00128: 			Dialogue response = AuctionPhone.Ending("");
00129: 			AuctionPhone.AddServiceChoice(val, AuctionPhone.CategoryCaption((ContainerCategory)num2) + AuctionText.Select("领奖", " Reward"), response, delegate
00130: 			{
00131: 				response.SetText(AuctionPhone.ContactName, Claim(category) ? AuctionText.Select("奖励已领取：2000¢。", "Reward claimed: 2000¢.") : AuctionText.Select("尚未集齐，或奖励已领取。", "Not complete, or reward already claimed."));
00132: 			});
00133: 		}
00134: 		if (!second)
00135: 		{
00136: 			AuctionPhone.AddServiceChoice(val, AuctionText.Select("其他分类", "Other Categories"), DialoguePage(second: true), null);
00137: 		}
00138: 		else
00139: 		{
00140: 			Dialogue response2 = AuctionPhone.Ending("");
00141: 			AuctionPhone.AddServiceChoice(val, AuctionText.Select("全册领奖", "Full Book Reward"), response2, delegate
00142: 			{
00143: 				response2.SetText(AuctionPhone.ContactName, Claim(4) ? AuctionText.Select("奖励已领取：5000¢。", "Reward claimed: 5000¢.") : AuctionText.Select("尚未集齐，或奖励已领取。", "Not complete, or reward already claimed."));
00144: 			});
00145: 		}
00146: 		return val;
00147: 	}
00148: }
~~~

### XIAOWONightAuction/AuctionPhoneRules.cs

~~~csharp
00001: namespace XIAOWONightAuction;
00002: 
00003: internal static class AuctionPhoneRules
00004: {
00005: 	internal const int IntroductionDay = 8;
00006: 
00007: 	internal const int CallInterval = 3;
00008: 
00009: 	internal static bool Eligible(int day)
00010: 	{
00011: 		return day >= 8;
00012: 	}
00013: 
00014: 	internal static int ReadyDay(int calledDay)
00015: 	{
00016: 		return checked(calledDay + 3);
00017: 	}
00018: 
00019: 	internal static int ReadyDay(int calledDay, long gifts)
00020: 	{
00021: 		return checked(calledDay + AuctionMerchantRules.CallInterval(gifts));
00022: 	}
00023: 
00024: 	internal static bool CanReserve(int day, int readyDay, bool pending)
00025: 	{
00026: 		if (Eligible(day) && day >= readyDay)
00027: 		{
00028: 			return !pending;
00029: 		}
00030: 		return false;
00031: 	}
00032: 
00033: 	internal static bool VisitDue(int day, int dueDay)
00034: 	{
00035: 		if (dueDay > 0)
00036: 		{
00037: 			return day >= dueDay;
00038: 		}
00039: 		return false;
00040: 	}
00041: 
00042: 	internal static int StableSeed(string value)
00043: 	{
00044: 		int num = 17;
00045: 		string text = value ?? "";
00046: 		foreach (char c in text)
00047: 		{
00048: 			num = num * 31 + c;
00049: 		}
00050: 		return num & 0x7FFFFFFF;
00051: 	}
00052: 
00053: 	internal static ContainerCategory Presenter(string runId)
00054: 	{
00055: 		int num = 17;
00056: 		string text = runId ?? string.Empty;
00057: 		foreach (char c in text)
00058: 		{
00059: 			num = num * 31 + c;
00060: 		}
00061: 		return (ContainerCategory)((uint)num % 4u);
00062: 	}
00063: }
~~~

### XIAOWONightAuction/Core.cs

~~~csharp
00031: 			{
00032: 				Scene activeScene = SceneManager.GetActiveScene();
00033: 				if (((Scene)(ref activeScene)).name == "InventoryScene")
00034: 				{
00035: 					return !AuctionLoadLifecycle.IsLoading;
00036: 				}
00037: 			}
00038: 			return false;
00039: 		}
00040: 	}
00041: 
00042: 	public void Init(ModManifest manifest)
00043: 	{
00044: 		Log = new AuctionLogger(manifest);
00045: 	}
00046: 
00047: 	public void OnEnable()
00048: 	{
00049: 		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
00050: 		//IL_01f0: Expected O, but got Unknown
00051: 		if (!_enabled)
00052: 		{
00053: 			Stopwatch stopwatch = Stopwatch.StartNew();
00054: 			IsShuttingDown = false;
00055: 			_enabled = true;
00056: 			ModHook.OnModItemDirectoryInit += AuctionItemRegistration.Register;
00057: 			ModHook.OnModAssetsLoaded += OfficialAssets.Loaded;
00058: 			ModHook.OnLootTablesLoaded += AuctionCatalog.InvalidatePools;
00059: 			ModHook.OnGameLoadedInit += BeforeLoad;
00060: 			ModHook.OnGameLoadedLate += AfterLoad;
00061: 			ModHook.OnNewGameStarted += NewGame;
00062: 			ModHook.OnGenerateCustomerLate += GeneratedCustomers;
00063: 			ModHook.OnWakingUpLate += AfterWake;
00064: 			ModHook.OnReturningStoreLate += Returned;
00065: 			SceneManager.sceneUnloaded += SceneUnloaded;
00066: 			Location = ModHelper.AddAfterhourLocation("xiaowo_night_auction", "xiaowo_night_auction", (Func<string>)(() => AuctionText.Get("map_entry", "集装箱拍卖")), (Action)delegate
00067: 			{
00068: 				AuctionController.Open();
00069: 				Location.panelObject = AuctionController.Root;
00070: 			}, (Action)delegate
00071: 			{
00072: 				AuctionController.Close(returnToStore: false);
00073: 			}, (Action<CustomUIBuilder>)null, "xiaowo_night_auction:auction_destination", "xiaowo_night_auction:auction_background", (string)null, (Func<bool>)null, (Func<bool>)null);
00074: 			AuctionNightMarket.Register();
00075: 			_driver = new GameObject("XIAOWONightAuction.OfficialDriver");
00076: 			Object.DontDestroyOnLoad((Object)(object)_driver);
00077: 			_driver.AddComponent<AuctionUpdateDriver>();
00078: 			Log.Msg(string.Format("Official Mono enabled in {0:F2} ms; Harmony owner=mod.{1} (loader-managed, no manual pass). Resources/items await official callbacks.", stopwatch.Elapsed.TotalMilliseconds, "xiaowo_night_auction"));
00079: 		}
00080: 	}
00081: 
00082: 	private static void BeforeLoad()
00083: 	{
00084: 		AuctionController.Dispose(returnToStore: false);
00085: 		AuctionPhone.Dispose();
00086: 		AuctionMerchantRelations.Dispose();
00087: 		WildeExpressEvidence.Dispose();
00088: 		AuctionLoadLifecycle.BeginLoad();
00089: 	}
00090: 
00091: 	private static void AfterLoad()
00092: 	{
00093: 		if (!IsShuttingDown && PlayerStore.HasInstance)
00094: 		{
00095: 			AuctionLoadLifecycle.Loaded(PlayerStore.Instance);
00096: 			AuctionController.EnsureRuntime();
00097: 			AfterWake();
00098: 		}
00099: 	}
00100: 
00101: 	private static void NewGame(string _)
00102: 	{
00103: 		AuctionLoadLifecycle.RequestNewGame();
00104: 	}
00105: 
00106: 	private static void GeneratedCustomers(StoreClientManager manager)
00107: 	{
00108: 		AuctionMerchants.HandleDailyVisitors(manager);
00109: 		AuctionPhone.HandleVisitors(manager);
00110: 	}
00111: 
00112: 	private static void AfterWake()
00113: 	{
00114: 		if (StoreReady)
00115: 		{
00116: 			WildeExpressEvidence.RegisterForLoadedStore();
00117: 			AuctionNightMarket.EnsureToday();
00118: 			AuctionPhone.OnStoreReady();
00119: 			AuctionMerchants.RepairSavedBuyerFactions(PlayerStore.Instance.storeClientManager);
00120: 		}
00121: 	}
00122: 
00123: 	private static void Returned()
00124: 	{
00125: 		if (StoreReady)
00126: 		{
00127: 			AuctionController.TryDeliverPendingFromHook();
00128: 			AuctionPhone.OnStoreReady();
00129: 		}
00130: 	}
00131: 
00132: 	private static void SceneUnloaded(Scene scene)
00133: 	{
00134: 		if (!(((Scene)(ref scene)).name != "InventoryScene"))
00135: 		{
00136: 			RunCleanupStep("auction UI", delegate
00137: 			{
00138: 				AuctionController.Dispose(returnToStore: false);
00139: 			});
00140: 			RunCleanupStep("identity dialog", AuctionGhostIdentity.Dispose);
00141: 			RunCleanupStep("phone", AuctionPhone.Dispose);
00142: 			RunCleanupStep("relations", AuctionMerchantRelations.Dispose);
00143: 			RunCleanupStep("evidence button", WildeExpressEvidence.Dispose);
00144: 			RunCleanupStep("load generation", AuctionLoadLifecycle.Dispose);
00145: 		}
00146: 	}
00147: 
00148: 	public void OnDisable()
00149: 	{
00150: 		if (_enabled)
00151: 		{
00152: 			IsShuttingDown = true;
00153: 			_enabled = false;
00154: 			ModHook.OnModItemDirectoryInit -= AuctionItemRegistration.Register;
00155: 			ModHook.OnModAssetsLoaded -= OfficialAssets.Loaded;
00156: 			ModHook.OnLootTablesLoaded -= AuctionCatalog.InvalidatePools;
00157: 			ModHook.OnGameLoadedInit -= BeforeLoad;
00158: 			ModHook.OnGameLoadedLate -= AfterLoad;
00159: 			ModHook.OnNewGameStarted -= NewGame;
00160: 			ModHook.OnGenerateCustomerLate -= GeneratedCustomers;
00161: 			ModHook.OnWakingUpLate -= AfterWake;
00162: 			ModHook.OnReturningStoreLate -= Returned;
00163: 			SceneManager.sceneUnloaded -= SceneUnloaded;
00164: 			RunCleanupStep("auction UI", delegate
00165: 			{
00166: 				AuctionController.Dispose(returnToStore: false);
00167: 			});
00168: 			RunCleanupStep("identity dialog", AuctionGhostIdentity.Dispose);
00169: 			RunCleanupStep("phone", AuctionPhone.Dispose);
00170: 			RunCleanupStep("relations", AuctionMerchantRelations.Dispose);
00171: 			RunCleanupStep("load generation", AuctionLoadLifecycle.Dispose);
00172: 			RunCleanupStep("evidence service", WildeExpressEvidence.Unregister);
00173: 			RunCleanupStep("UI style", AuctionUiStyle.Dispose);
00174: 			RunCleanupStep("assets", OfficialAssets.Clear);
00175: 			RunCleanupStep("night market events", AuctionNightMarket.Unregister);
00176: 			RunCleanupStep("location", delegate
00177: 			{
00178: 				ModHelper.RemoveAfterhourLocations("xiaowo_night_auction");
00179: 			});
00180: 			Location = null;
00181: 			if (Object.op_Implicit((Object)(object)_driver))
00182: 			{
00183: 				Object.Destroy((Object)(object)_driver);
00184: 			}
00185: 			_driver = null;
00186: 			Log?.Msg("Official Mono disabled; callbacks detached; no inventory created during teardown.");
00187: 		}
00188: 	}
00189: 
00190: 	internal static bool RunCleanupStep(string name, Action action)
00191: 	{
00192: 		try
00193: 		{
00194: 			action();
00195: 			return true;
00196: 		}
00197: 		catch (Exception ex)
00198: 		{
00199: 			Log?.Warning("Cleanup [" + name + "]: " + ex);
00200: 			return false;
00201: 		}
00202: 	}
00203: }
~~~

### XIAOWONightAuction/AuctionNightMarket.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Linq;
00004: using Newtonsoft.Json;
00005: 
00006: namespace XIAOWONightAuction;
00007: 
00008: internal static class AuctionNightMarket
00009: {
00010: 	private static bool _buying;
00011: 
00012: 	private static string DayKey => "market_day_" + AuctionCatalog.Day;
00013: 
00014: 	internal static AuctionMarketDay Current
00015: 	{
00016: 		get
00017: 		{
00018: 			if (NativeStore.Existing == null)
00019: 			{
00020: 				return null;
00021: 			}
00022: 			AuctionMarketDay auctionMarketDay = Read<AuctionMarketDay>(DayKey);
00023: 			if (!AuctionNightMarketRules.Valid(auctionMarketDay, AuctionCatalog.Day))
00024: 			{
00025: 				return null;
00026: 			}
00027: 			return auctionMarketDay;
00028: 		}
00029: 	}
00030: 
00031: 	private static string T(string zh, string en)
00032: 	{
00033: 		return AuctionText.Select(zh, en);
00034: 	}
00035: 
00036: 	private static string EventId(AuctionMarketEvent kind)
00037: 	{
00038: 		int num = (int)kind;
00039: 		return "xiaowo_night_auction:market_" + num;
00040: 	}
00041: 
00042: 	private static string AdvisorId(AuctionVenue venue)
00043: 	{
00044: 		int num = (int)venue;
00045: 		return "xiaowo_night_auction:advisor_" + num;
00046: 	}
00047: 
00048: 	private static string AdvisorKey(AuctionVenue venue)
00049: 	{
00050: 		int num = (int)venue;
00051: 		return "advisor_" + num;
00052: 	}
00053: 
00054: 	private static TValue Read<TValue>(string key) where TValue : class
00055: 	{
00056: 		//IL_0024: Expected O, but got Unknown
00057: 		try
00058: 		{
00059: 			string text = AuctionState.Read(key);
00060: 			return string.IsNullOrEmpty(text) ? null : JsonConvert.DeserializeObject<TValue>(text);
00061: 		}
00062: 		catch (JsonException ex)
00063: 		{
00064: 			JsonException ex2 = ex;
00065: 			Core.Log?.Warning("[Night market] Invalid " + key + ": " + ((Exception)(object)ex2).Message);
00066: 			return null;
00067: 		}
00068: 	}
00069: 
00070: 	internal static AuctionAdvisorContract Contract(AuctionVenue venue)
00071: 	{
00072: 		if (NativeStore.Existing == null)
00073: 		{
00074: 			return null;
00075: 		}
00076: 		AuctionAdvisorContract auctionAdvisorContract = Read<AuctionAdvisorContract>(AdvisorKey(venue));
00077: 		if (!AuctionNightMarketRules.Active(auctionAdvisorContract, venue, AuctionCatalog.Day))
00078: 		{
00079: 			return null;
00080: 		}
00081: 		return auctionAdvisorContract;
00082: 	}
00083: 
00084: 	internal static void Register()
00085: 	{
00086: 		foreach (AuctionMarketEvent value in Enum.GetValues(typeof(AuctionMarketEvent)))
00087: 		{
00088: 			if (value != AuctionMarketEvent.None)
00089: 			{
00090: 				AuctionMarketEvent captured = value;
00091: 				ModHelper.AddAfterhourEvent("xiaowo_night_auction", EventId(value), "xiaowo_night_auction", (Func<string>)(() => Title(captured)), 1, (Func<string>)(() => Description(captured)), (IEnumerable<AfterhourEffect>)(object)new AfterhourEffect[1] { ModHelper.AfterhourEffect((Func<string>)(() => Effect(captured)), (string)null, 0f, (AfterhourEffectPolarity)((value != AuctionMarketEvent.Maintenance) ? 1 : 2)) }, 0f, (AfterhourEventTone?)null, (Func<bool>)null, (Action)null, (Action)null, (IEnumerable<AfterhourWeekday>)null, false);
00092: 			}
00093: 		}
00094: 		foreach (AuctionVenue value2 in Enum.GetValues(typeof(AuctionVenue)))
00095: 		{
00096: 			AuctionVenue captured2 = value2;
00097: 			ModHelper.AddAfterhourEvent("xiaowo_night_auction", AdvisorId(value2), "xiaowo_night_auction", (Func<string>)(() => AuctionRules.VenueName(captured2) + T("拍卖行顾问", " Advisor")), 3, (Func<string>)(() => T("已购买的情报顾问服务。", "Your purchased appraisal service.")), (IEnumerable<AfterhourEffect>)(object)new AfterhourEffect[1] { ModHelper.AfterhourEffect((Func<string>)(() => T("玩家情报费用优惠30%；与驻场优惠取较高者。", "Player intel costs 30% less; best discount applies.")), (string)null, 0f, (AfterhourEffectPolarity)1) }, 0f, (AfterhourEventTone?)null, (Func<bool>)null, (Action)null, (Action)null, (IEnumerable<AfterhourWeekday>)null, false);
00098: 		}
00099: 	}
00100: 
00101: 	internal static void Unregister()
00102: 	{
~~~

~~~csharp
00224: 				Paid = num
00225: 			};
00226: 			AuctionState.Write(AdvisorKey(venue), JsonConvert.SerializeObject((object)auctionAdvisorContract));
00227: 			if (!ModHelper.StartAfterhourEvent(AdvisorId(venue), 3))
00228: 			{
00229: 				throw new InvalidOperationException("Official advisor event could not start.");
00230: 			}
00231: 			existing.ModCash(-num, false);
00232: 			message = AuctionRules.VenueName(venue) + T("顾问已开通，情报费用优惠30%。有效至第", " advisor active: 30% off intel through day ") + auctionAdvisorContract.Ends + T("天（含当天）。", " inclusive.");
00233: 			return true;
00234: 		}
00235: 		catch
00236: 		{
00237: 			if (string.IsNullOrEmpty(value))
00238: 			{
00239: 				AuctionState.Delete(AdvisorKey(venue));
00240: 			}
00241: 			else
00242: 			{
00243: 				AuctionState.Write(AdvisorKey(venue), value);
00244: 			}
00245: 			existing.ModCash(cash - existing.GetCash(), false);
00246: 			ModHelper.EndAfterhourEvent(AdvisorId(venue));
00247: 			throw;
00248: 		}
00249: 		finally
00250: 		{
00251: 			_buying = false;
00252: 		}
00253: 	}
00254: 
00255: 	internal static Dialogue BuildAdvisorDialogue(Dialogue serviceRoot)
00256: 	{
00257: 		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
00258: 		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
00259: 		Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, T("拍卖行顾问：仅为所选场次提供情报优惠30%，购买当天计入3天有效期；与驻场优惠取较高者。", "Auction advisors: 30% off player intel in the selected venue for 3 days including today. Best discount applies."));
00260: 		Dialogue val2 = new Dialogue().SetText(AuctionPhone.ContactName, T("高级场顾问服务。服务不会预约商人。", "Advanced advisor services. This does not book a buyer."));
00261: 		AddAdvisorChoice(val, AuctionVenue.Beginner, val);
00262: 		AddAdvisorChoice(val, AuctionVenue.Intermediate, val);
00263: 		AuctionPhone.AddServiceChoice(val, T("高级场 / 返回", "Advanced / Back"), val2, null);
00264: 		AddAdvisorChoice(val2, AuctionVenue.Advanced, val);
00265: 		AuctionPhone.AddServiceChoice(val2, T("返回顾问列表", "Advisor List"), val, null);
00266: 		AuctionPhone.AddServiceChoice(val2, T("返回服务菜单", "Service Menu"), serviceRoot, null);
00267: 		return val;
00268: 	}
00269: 
00270: 	private static void AddAdvisorChoice(Dialogue root, AuctionVenue venue, Dialogue list)
00271: 	{
00272: 		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
00273: 		AuctionAdvisorContract auctionAdvisorContract = Contract(venue);
00274: 		if (auctionAdvisorContract != null)
00275: 		{
00276: 			AuctionPhone.AddServiceChoice(root, AuctionRules.VenueName(venue) + T("：有效期剩余", ": active, ") + (auctionAdvisorContract.Ends - AuctionCatalog.Day + 1) + T("天", " days left"), AuctionPhone.Ending(T("服务仍然有效，不会重复购买。", "Your contract is active. No duplicate purchase.")), null);
00277: 			return;
00278: 		}
00279: 		int num = AuctionNightMarketRules.AdvisorPrice(venue);
00280: 		Dialogue val = new Dialogue().SetText(AuctionPhone.ContactName, AuctionRules.VenueName(venue) + T("顾问费用 ", " advisor: ") + num + T("¢。购买当天起3天，玩家情报费用优惠30%。仅适用于该场次；是否确认支付？", "¢ for 3 days including today. Player intel costs 30% less in this venue only. Confirm payment?"));
00281: 		Dialogue result = AuctionPhone.Ending("");
00282: 		AuctionPhone.AddServiceChoice(val, T("确认支付 ", "Pay ") + num + "¢", result, delegate
00283: 		{
00284: 			BuyAdvisor(venue, out var message);
00285: 			result.SetText(AuctionPhone.ContactName, message);
00286: 		});
00287: 		AuctionPhone.AddServiceChoice(val, T("取消", "Cancel"), list, null);
00288: 		AuctionPhone.AddServiceChoice(root, AuctionRules.VenueName(venue) + " · " + num + "¢", val, null);
00289: 	}
00290: 
00291: 	internal static string Summary(AuctionVenue? venue = null)
00292: 	{
00293: 		AuctionMarketEvent auctionMarketEvent = Current?.Kind ?? AuctionMarketEvent.None;
00294: 		string text = ((auctionMarketEvent == AuctionMarketEvent.None) ? T("今晚无特殊行情", "No special market tonight") : (Title(auctionMarketEvent) + " · " + Effect(auctionMarketEvent)));
00295: 		if (venue.HasValue)
00296: 		{
00297: 			AuctionAdvisorContract auctionAdvisorContract = Contract(venue.Value);
00298: 			if (auctionAdvisorContract != null)
00299: 			{
00300: 				text = text + T(" | 顾问优惠30%，剩余", " | Advisor −30%, ") + (auctionAdvisorContract.Ends - AuctionCatalog.Day + 1) + T("天", "d left");
00301: 				goto IL_0163;
00302: 			}
00303: 		}
00304: 		if (!venue.HasValue)
00305: 		{
00306: 			foreach (AuctionVenue value in Enum.GetValues(typeof(AuctionVenue)))
00307: 			{
00308: 				AuctionAdvisorContract auctionAdvisorContract2 = Contract(value);
00309: 				if (auctionAdvisorContract2 != null)
00310: 				{
00311: 					text = text + " | " + AuctionRules.VenueName(value) + T("顾问", " advisor") + " " + (auctionAdvisorContract2.Ends - AuctionCatalog.Day + 1) + T("天", "d");
00312: 				}
00313: 			}
00314: 		}
00315: 		goto IL_0163;
00316: 		IL_0163:
00317: 		return text;
00318: 	}
00319: 
00320: 	internal static string Title(AuctionMarketEvent kind)
00321: 	{
00322: 		return kind switch
00323: 		{
00324: 			AuctionMarketEvent.Appraiser => T("鉴定师驻场", "Appraiser on Site"), 
00325: 			AuctionMarketEvent.CategoryFocus => AuctionPhone.CategoryCaption((ContainerCategory)(Current?.Category ?? 0)) + T("专场", " Showcase"), 
00326: 			AuctionMarketEvent.AdmissionSale => T("入场优惠", "Admission Sale"), 
00327: 			AuctionMarketEvent.ManifestLeak => T("货单泄露", "Leaked Manifest"), 
00328: 			AuctionMarketEvent.CautiousBuyer => T("买家观望", "Cautious Buyer"), 
00329: 			AuctionMarketEvent.Maintenance => T("检测设备检修", "Scanner Maintenance"), 
00330: 			_ => T("普通拍卖夜", "Regular Auction Night"), 
00331: 		};
00332: 	}
00333: 
00334: 	private static string Description(AuctionMarketEvent kind)
00335: 	{
00336: 		return kind switch
00337: 		{
00338: 			AuctionMarketEvent.CategoryFocus => T("本周分类专场；其余三类箱子照常开放。", "This week's category showcase; all four categories remain available."), 
00339: 			AuctionMarketEvent.ManifestLeak => T("一份货单提前流出，可在对应箱子查看两件真实货物。", "A leaked manifest reveals two real items in one marked container."), 
00340: 			AuctionMarketEvent.CautiousBuyer => T("一位买家收紧采购计划，但仍会独立判断和报价。", "One buyer is more cautious but still makes independent bids."), 
00341: 			AuctionMarketEvent.Maintenance => T("高级复鉴设备正在检修，其他情报服务正常开放。", "Advanced scanners need maintenance; other intel services remain available."), 
00342: 			_ => T("拍卖行今晚提供特别安排。", "Special arrangements at tonight's auction."), 
00343: 		};
00344: 	}
00345: 
00346: 	private static string Effect(AuctionMarketEvent kind)
00347: 	{
00348: 		return kind switch
00349: 		{
00350: 			AuctionMarketEvent.Appraiser => T("玩家和AI情报费优惠20%", "20% off intel for you and AI"), 
00351: 			AuctionMarketEvent.CategoryFocus => T("对应箱主类别90%，混合10%；不保证盈利", "Featured box: 90% main, 10% mixed; no profit guarantee"), 
00352: 			AuctionMarketEvent.AdmissionSale => T("入场费优惠20%，新手场免费", "20% off entry; beginner remains free"), 
00353: 			AuctionMarketEvent.ManifestLeak => T("标记箱免费公开2件货物及单件价值", "Two free item reveals in the marked box"), 
00354: 			AuctionMarketEvent.CautiousBuyer => T("一位AI心理价上限降低15%", "One AI's bidding limit is 15% lower"), 
00355: 			AuctionMarketEvent.Maintenance => T("仅高级复鉴费增加20%", "Advanced reappraisal costs 20% more"), 
00356: 			_ => "", 
00357: 		};
00358: 	}
00359: 
00360: 	internal static string[] LeakItems(AuctionLot lot)
00361: 	{
00362: 		AuctionMarketDay current = Current;
00363: 		if (current == null || current.Kind != AuctionMarketEvent.ManifestLeak || current.LeakSlot != lot.Slot)
00364: 		{
00365: 			return Array.Empty<string>();
00366: 		}
00367: 		string text = "market_leak_" + current.Day + "_" + lot.Seed;
00368: 		List<string> list = Read<List<string>>(text);
00369: 		if (list != null && list.Count == Math.Min(2, lot.Contents.Count))
00370: 		{
00371: 			List<string> remaining = new List<string>(lot.Contents);
00372: 			if (list.All((string id) => remaining.Remove(id)))
00373: 			{
00374: 				return list.ToArray();
00375: 			}
00376: 		}
00377: 		List<int> list2 = Enumerable.Range(0, lot.Contents.Count).ToList();
00378: 		Random random = new Random(lot.Seed ^ current.Seed ^ 0x516E7);
00379: 		List<string> list3 = new List<string>();
00380: 		while (list2.Count > 0 && list3.Count < 2)
00381: 		{
00382: 			int index = random.Next(list2.Count);
00383: 			list3.Add(lot.Contents[list2[index]]);
00384: 			list2.RemoveAt(index);
00385: 		}
00386: 		AuctionState.Write(text, JsonConvert.SerializeObject((object)list3));
00387: 		return list3.ToArray();
00388: 	}
00389: }
~~~

### XIAOWONightAuction/AuctionController.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Linq;
00004: using TMPro;
00005: using UnityEngine;
00006: using UnityEngine.Events;
00007: using UnityEngine.SceneManagement;
00008: using UnityEngine.UI;
00009: 
00010: namespace XIAOWONightAuction;
00011: 
00012: internal static class AuctionController
00013: {
00014: 	private enum Page
00015: 	{
00016: 		None,
00017: 		Venues,
00018: 		VenueConfirm,
00019: 		Lots,
00020: 		Stage,
00021: 		Intel,
00022: 		Inspection,
00023: 		Manifest,
00024: 		Withdraw,
00025: 		Settlement,
00026: 		History,
00027: 		Orders,
00028: 		Collection,
00029: 		Relationships
00030: 	}
00031: 
00032: 	private static CustomUIWindow _officialWindow;
00033: 
00034: 	internal const string RootName = "XIAOWONightAuctionRoot";
00035: 
00036: 	private static GameObject _root;
00037: 
00038: 	private static RectTransform _layout;
00039: 
00040: 	private static TextMeshProUGUI _cash;
00041: 
00042: 	private static TextMeshProUGUI _title;
00043: 
00044: 	private static TextMeshProUGUI _status;
00045: 
00046: 	private static TextMeshProUGUI _eventSummary;
00047: 
00048: 	private static TextMeshProUGUI _sessionSummary;
00049: 
00050: 	private static AuctionLot _manifestLot;
00051: 
00052: 	private static ScrollRect _broadcastScroll;
00053: 
00054: 	private static float _broadcastPosition;
00055: 
00056: 	private static float _nextAnnouncementAt;
00057: 
00058: 	private static bool _fastAnnouncements;
00059: 
00060: 	private static bool _stageVisible;
00061: 
00062: 	private static ContainerCategory _bookCategory;
00063: 
00064: 	private static bool _cancelOrderPending;
00065: 
00066: 	private static Page _page;
00067: 
00068: 	private static AuctionVenue _confirmVenue;
00069: 
00070: 	private static bool _settlementWon;
00071: 
00072: 	private static AuctionLot _settlementLot;
00073: 
00074: 	private static readonly List<GameObject> Dynamic = new List<GameObject>();
00075: 
00076: 	private static List<AuctionLot> _lots = new List<AuctionLot>();
00077: 
00078: 	private static AuctionLot _selected;
00079: 
00080: 	private static AuctionVenue _venue;
00081: 
~~~

~~~csharp
00125: 
00126: 	private static int ResolvedAuctions => _lots.Count((AuctionLot lot) => lot.Finished || lot.Lost);
00127: 
00128: 	private static bool SessionComplete => ResolvedAuctions >= 3;
00129: 
00130: 	internal static bool IsOpen
00131: 	{
00132: 		get
00133: 		{
00134: 			if (_outsideStarted)
00135: 			{
00136: 				return (Object)(object)_root != (Object)null;
00137: 			}
00138: 			return false;
00139: 		}
00140: 	}
00141: 
00142: 	internal static bool IsPhoneCollectionOpen
00143: 	{
00144: 		get
00145: 		{
00146: 			if (_phoneCollection)
00147: 			{
00148: 				return (Object)(object)_root != (Object)null;
00149: 			}
00150: 			return false;
00151: 		}
00152: 	}
00153: 
00154: 	internal static IReadOnlyList<AuctionLot> TestLots => _lots;
00155: 
00156: 	internal static AuctionVenue TestVenue => _venue;
00157: 
00158: 	private static string T(string chinese, string english)
00159: 	{
00160: 		return AuctionText.Select(chinese, english);
00161: 	}
00162: 
00163: 	internal static void RequestPhoneCollection()
00164: 	{
00165: 		RequestPhonePanel(relationships: false);
00166: 	}
00167: 
00168: 	internal static void RequestPhoneRelationships()
00169: 	{
00170: 		RequestPhonePanel(relationships: true);
00171: 	}
00172: 
00173: 	private static void RequestPhonePanel(bool relationships)
00174: 	{
00175: 		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
00176: 		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
00177: 		PlayerStore instance = PlayerStore.Instance;
00178: 		if (!((Object)(object)_root != (Object)null) && _collectionRequestedAt < 0 && !_outsideStarted && instance != null && !((Object)(object)EmporiumEntry.Instance == (Object)null))
00179: 		{
00180: 			Scene activeScene = SceneManager.GetActiveScene();
00181: 			if (!(((Scene)(ref activeScene)).name != "InventoryScene"))
00182: 			{
00183: 				_collectionStore = instance;
00184: 				_collectionSlot = instance.saveSlotId;
00185: 				_collectionRun = instance.runID;
00186: 				_phoneRelationships = relationships;
00187: 				_collectionRequestedAt = Time.frameCount;
00188: 			}
00189: 		}
00190: 	}
00191: 
00192: 	private static bool CollectionContextMatches()
00193: 	{
00194: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
00195: 		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
00196: 		PlayerStore instance = PlayerStore.Instance;
00197: 		Scene activeScene = SceneManager.GetActiveScene();
00198: 		if (((Scene)(ref activeScene)).name == "InventoryScene" && instance != null && _collectionStore != null && (Object)(object)EmporiumEntry.Instance != (Object)null && instance == _collectionStore && instance.saveSlotId == _collectionSlot)
00199: 		{
00200: 			return instance.runID == _collectionRun;
00201: 		}
00202: 		return false;
00203: 	}
00204: 
00205: 	private static string CollectionRevision()
00206: 	{
00207: 		return AuctionState.Read("collection_found") + "|" + AuctionState.Read("collection_claimed");
00208: 	}
00209: 
00210: 	private static void OpenPhoneCollection()
00211: 	{
00212: 		_collectionRequestedAt = -1;
00213: 		if (!CollectionContextMatches() || (Object)(object)_root != (Object)null || _outsideStarted)
00214: 		{
00215: 			return;
00216: 		}
00217: 		try
00218: 		{
00219: 			PhoneUIManager instance = PhoneUIManager.Instance;
00220: 			if (instance != null)
00221: 			{
00222: 				instance.CloseUI();
00223: 			}
00224: 			OverlayHandler current = OverlayHandler.current;
00225: 			if (current != null)
00226: 			{
00227: 				current.HideTooltip();
00228: 			}
00229: 			_phoneCollection = true;
~~~

~~~csharp
00716: 		{
00717: 			val2.transform.SetAsFirstSibling();
00718: 		}
00719: 		Stretch(val2.GetComponent<RectTransform>());
00720: 		Image component = val2.GetComponent<Image>();
00721: 		((Graphic)component).color = new Color(0.025f, 0.028f, 0.035f, 0.992f);
00722: 		if (!_phoneCollection)
00723: 		{
00724: 			component.sprite = OfficialAssets.Get("auction_background");
00725: 			((Graphic)component).color = Color.white;
00726: 		}
00727: 		if (_phoneCollection)
00728: 		{
00729: 			component.sprite = null;
00730: 			((Graphic)component).color = Frame;
00731: 			GameObject obj3 = NewObject("BookSurface", (Transform)(object)_layout, typeof(Image));
00732: 			SetRect(obj3.GetComponent<RectTransform>(), new Rect(4f, 4f, 952f, 532f));
00733: 			((Graphic)obj3.GetComponent<Image>()).color = Surface;
00734: 			((Graphic)obj3.GetComponent<Image>()).raycastTarget = false;
00735: 		}
00736: 		((Graphic)component).raycastTarget = false;
00737: 		if (_phoneCollection)
00738: 		{
00739: 			GameObject obj4 = NewObject("Header", (Transform)(object)_layout, typeof(Image));
00740: 			SetRect(obj4.GetComponent<RectTransform>(), new Rect(24f, 458f, 912f, 58f));
00741: 			((Graphic)obj4.GetComponent<Image>()).color = SurfaceRaised;
00742: 			((Graphic)obj4.GetComponent<Image>()).raycastTarget = false;
00743: 			GameObject obj5 = NewObject("HeaderAccent", (Transform)(object)_layout, typeof(Image));
00744: 			SetRect(obj5.GetComponent<RectTransform>(), new Rect(24f, 458f, 6f, 58f));
00745: 			((Graphic)obj5.GetComponent<Image>()).color = new Color(0.69f, 0.38f, 0.25f, 1f);
00746: 			((Graphic)obj5.GetComponent<Image>()).raycastTarget = false;
00747: 			GameObject obj6 = NewObject("StatusBar", (Transform)(object)_layout, typeof(Image));
00748: 			SetRect(obj6.GetComponent<RectTransform>(), new Rect(72f, 430f, 816f, 24f));
00749: 			((Graphic)obj6.GetComponent<Image>()).color = new Color(0.055f, 0.06f, 0.07f, 0.98f);
00750: 			((Graphic)obj6.GetComponent<Image>()).raycastTarget = false;
00751: 			_title = CreateLabel("Title", _phoneCollection ? PhonePanelTitle() : AuctionText.Get("title", "夜间集装箱拍卖"), 26f, new Vector2(44f, -29f), new Vector2(500f, 30f), (FontStyles)1);
00752: 			_eventSummary = (_phoneCollection ? null : CreateLabel("NightMarketSummary", "", 11f, new Vector2(44f, -62f), new Vector2(668f, 18f), (FontStyles)0));
00753: 			if ((Object)(object)_eventSummary != (Object)null)
00754: 			{
00755: 				((TMP_Text)_eventSummary).enableWordWrapping = false;
00756: 				((TMP_Text)_eventSummary).overflowMode = (TextOverflowModes)1;
00757: 			}
00758: 			_cash = CreateLabel("Cash", AuctionText.Get("cash", "现金  {0}", PlayerStore.Instance.GetCash()), 18f, new Vector2((float)(_phoneCollection ? 670 : 720), -34f), new Vector2(196f, 34f), (FontStyles)1);
00759: 			((TMP_Text)_cash).alignment = (TextAlignmentOptions)260;
00760: 			_status = CreateLabel("Status", "", 13f, new Vector2(82f, -87f), new Vector2(796f, 22f), (FontStyles)0);
00761: 			((TMP_Text)_status).alignment = (TextAlignmentOptions)514;
00762: 		}
00763: 		else
00764: 		{
00765: 			BuildAuctionHeader();
00766: 		}
00767: 		if (_phoneCollection)
00768: 		{
00769: 			CreateButton("CollectionClose", "X", new Rect(892f, 478f, 28f, 28f), delegate
00770: 			{
00771: 				Close(returnToStore: false);
00772: 			}, enabled: true);
00773: 		}
00774: 		_officialWindow.Show();
00775: 		PositionLayout();
00776: 	}
00777: 
00778: 	private static string MainMix(AuctionLot lot)
00779: 	{
00780: 		int num = AuctionNightMarket.MainPercent(lot.Category);
00781: 		return T("主类 ", "Main ") + num + T("% / 混合 ", "% / mixed ") + (100 - num) + "%";
00782: 	}
00783: 
00784: 	private static Sprite PreviewSprite(GameItem item)
00785: 	{
00786: 		if (!item.useSpriteFromMod)
00787: 		{
00788: 			return RenderHandler.LoadFromAtlas(item.spriteAtlasPath, item.spritePath);
00789: 		}
00790: 		return CustomUIManager.Instance.GetModSprite(item.spriteAtlasPath, item.spritePath, true);
00791: 	}
00792: 
00793: 	private static void RenderManifest(AuctionLot lot)
00794: 	{
00795: 		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
00796: 		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
00797: 		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
00798: 		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
00799: 		_manifestLot = lot;
00800: 		_page = Page.Manifest;
00801: 		ClearDynamic();
00802: 		string[] array = AuctionNightMarket.LeakItems(lot);
00803: 		Dynamic.Add(CreatePanel("ManifestPanel", new Rect(174f, 108f, 612f, 306f), AuctionCatalog.CategoryColor(lot.Category)));
00804: 		AddStageLabel("ManifestTitle", T("泄露货单 · 两件真实货物", "Leaked Manifest · Two Actual Items"), 21f, 202f, -139f, 556f, 32f, (FontStyles)1);
00805: 		for (int i = 0; i < array.Length; i++)
00806: 		{
00807: 			GameItem val = DirectoryMaster.Item(array[i], false);
00808: 			int num = 238 + i * 250;
00809: 			GameObject val2 = NewObject("ManifestSprite" + i, (Transform)(object)_layout, typeof(Image));
00810: 			SetRect(val2.GetComponent<RectTransform>(), new Rect((float)(num + 45), 236f, 100f, 100f));
00811: 			Image component = val2.GetComponent<Image>();
00812: 			component.sprite = PreviewSprite(val);
00813: 			component.preserveAspect = true;
00814: 			((Graphic)component).raycastTarget = false;
00815: 			Dynamic.Add(val2);
00816: 			AddStageLabel("ManifestName" + i, val.name, 15f, num, -315f, 200f, 44f, (FontStyles)1);
00817: 			AddStageLabel("ManifestValue" + i, T("单件参考价值 ", "Unit value ") + AuctionCatalog.ItemValue(array[i]) + "¢", 13f, num, -365f, 200f, 28f, (FontStyles)0);
00818: 		}
00819: 		Dynamic.Add(CreateButton("ManifestBack", T("返回四箱", "Four Lots"), new Rect(390f, 55f, 180f, 36f), delegate
00820: 		{
00821: 			RenderLots(T("货单仅公开部分货物，不代表箱内总价值。", "The manifest reveals only part of the cargo, not total value."));
00822: 		}, enabled: true));
00823: 		SetStatus(T("免费线索，与付费四件抽检独立；每次查看内容相同。", "Free clues, separate from the paid four-item sample; repeated views are identical."));
00824: 	}
00825: 
00826: 	private static void ReturnLobby()
00827: 	{
~~~

~~~csharp
00936: 		Dynamic.Add(CreateButton("RelationshipClose", T("关闭档案", "Close Records"), new Rect(708f, 28f, 200f, 32f), delegate
00937: 		{
00938: 			Close(returnToStore: false);
00939: 		}, enabled: true));
00940: 	}
00941: 
00942: 	private static void RenderCollection(ContainerCategory category)
00943: 	{
00944: 		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
00945: 		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
00946: 		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
00947: 		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
00948: 		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
00949: 		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
00950: 		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
00951: 		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
00952: 		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
00953: 		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
00954: 		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
00955: 		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
00956: 		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
00957: 		_page = Page.Collection;
00958: 		if (_bookCategory != category)
00959: 		{
00960: 			_bookPage = 0;
00961: 		}
00962: 		_bookCategory = category;
00963: 		ClearDynamic();
00964: 		AuctionCollection.ScanOwned();
00965: 		_collectionRevision = CollectionRevision();
00966: 		HashSet<string> found = AuctionCollection.Found;
00967: 		SetStatus(T("藏品收集册  ·  ", "Collection Book  ·  ") + found.Count(AuctionExclusiveCatalog.IsExclusive) + "/" + AuctionExclusiveCatalog.Items.Count);
00968: 		for (int i = 0; i < 4; i++)
00969: 		{
00970: 			ContainerCategory tab = (ContainerCategory)i;
00971: 			Dynamic.Add(CreateButton("CollectionTab" + i, AuctionPhone.CategoryCaption(tab) + " " + AuctionCollection.Count(tab) + "/" + AuctionExclusiveCatalog.CategoryCount(tab), new Rect((float)(54 + i * 218), 390f, 200f, 32f), delegate
00972: 			{
00973: 				RenderCollection(tab);
00974: 			}, tab != category));
00975: 		}
00976: 		AuctionExclusiveSpec[] array = AuctionExclusiveCatalog.Items.Values.Where((AuctionExclusiveSpec spec) => spec.Category == category).ToArray();
00977: 		int num = Math.Max(1, (array.Length + 8 - 1) / 8);
00978: 		_bookPage = Math.Clamp(_bookPage, 0, num - 1);
00979: 		int num2 = _bookPage * 8;
00980: 		for (int num3 = num2; num3 < Math.Min(array.Length, num2 + 8); num3++)
00981: 		{
00982: 			AuctionExclusiveSpec auctionExclusiveSpec = array[num3];
00983: 			bool flag = found.Contains(auctionExclusiveSpec.Id);
00984: 			int num4 = num3 - num2;
00985: 			int num5 = 54 + num4 % 4 * 218;
00986: 			int num6 = ((num4 < 4) ? 242 : 110);
00987: 			Dynamic.Add(CreatePanel("CollectionItem" + num3, new Rect((float)num5, (float)num6, 200f, 124f), AuctionCatalog.CategoryColor(category)));
00988: 			GameObject val = NewObject("CollectionSprite" + num3, (Transform)(object)_layout, typeof(Image));
00989: 			SetRect(val.GetComponent<RectTransform>(), new Rect((float)(num5 + 10), (float)(num6 + 38), 72f, 72f));
00990: 			Image component = val.GetComponent<Image>();
00991: 			component.sprite = OfficialAssets.Get(auctionExclusiveSpec.Id);
00992: 			component.preserveAspect = true;
00993: 			((Graphic)component).raycastTarget = false;
00994: 			((Graphic)component).color = (Color)(flag ? Color.white : new Color(0.025f, 0.025f, 0.025f, 1f));
00995: 			Dynamic.Add(val);
00996: 			TextMeshProUGUI val2 = CreateLabel("CollectionName" + num3, flag ? AuctionCollection.Name(auctionExclusiveSpec) : "???", 13f, new Vector2((float)(num5 + 88), (float)(-(540 - num6 - 116))), new Vector2(102f, 78f), (FontStyles)1);
00997: 			((TMP_Text)val2).enableAutoSizing = true;
00998: 			((TMP_Text)val2).fontSizeMin = 9f;
00999: 			((TMP_Text)val2).fontSizeMax = 13f;
01000: 			((TMP_Text)val2).ForceMeshUpdate(false, false);
01001: 			Dynamic.Add(FindLayoutChild(((TMP_Text)val2).transform));
01002: 			AddStageLabel("CollectionValue" + num3, flag ? (T("基础价值 ", "Base value ") + auctionExclusiveSpec.Value + "¢") : T("未发现", "Undiscovered"), 12f, num5 + 12, -(540 - num6 - 29), 180f, 22f, (FontStyles)0);
01003: 		}
01004: 		Dynamic.Add(CreateButton("CollectionPrevious", T("上一页", "Previous"), new Rect(330f, 74f, 110f, 24f), delegate
01005: 		{
01006: 			_bookPage--;
01007: 			RenderCollection(category);
01008: 		}, _bookPage > 0));
01009: 		AddStageLabel("CollectionPage", T("第 ", "Page ") + (_bookPage + 1) + "/" + num, 12f, 448f, -444f, 128f, 22f, (FontStyles)0);
01010: 		Dynamic.Add(CreateButton("CollectionNext", T("下一页", "Next"), new Rect(586f, 74f, 110f, 24f), delegate
01011: 		{
01012: 			_bookPage++;
01013: 			RenderCollection(category);
01014: 		}, _bookPage + 1 < num));
01015: 		Dynamic.Add(CreateButton("CollectionReward", AuctionCollection.IsClaimed((int)category) ? T("分类奖励已领取", "Category Reward Claimed") : T("分类奖励 2000¢", "Category Reward 2000¢"), new Rect(54f, 28f, 218f, 32f), delegate
01016: 		{
01017: 			AuctionCollection.Claim((int)category);
01018: 			RenderCollection(category);
01019: 		}, AuctionCollection.CanClaim((int)category)));
01020: 		Dynamic.Add(CreateButton("CollectionFullReward", AuctionCollection.IsClaimed(4) ? T("全册奖励已领取", "Full Book Reward Claimed") : T("全册奖励 5000¢", "Full Book Reward 5000¢"), new Rect(294f, 28f, 218f, 32f), delegate
01021: 		{
01022: 			AuctionCollection.Claim(4);
01023: 			RenderCollection(category);
01024: 		}, AuctionCollection.CanClaim(4)));
01025: 		Dynamic.Add(CreateButton("CollectionBack", _phoneCollection ? T("关闭收集册", "Close Book") : T("返回拍卖大厅", "Auction Lobby"), new Rect(708f, 28f, 200f, 32f), delegate
01026: 		{
01027: 			if (_phoneCollection)
01028: 			{
01029: 				Close(returnToStore: false);
01030: 			}
01031: 			else
01032: 			{
01033: 				ReturnLobby();
01034: 			}
01035: 		}, enabled: true));
01036: 	}
01037: 
01038: 	private static void SelectLot(AuctionLot lot)
01039: 	{
01040: 		if (!SessionComplete && !lot.Finished && !lot.Lost && !_lots.Any((AuctionLot other) => other != lot && other.Started && !other.Finished && !other.Lost))
~~~

~~~csharp
01117: 		if (selected.PendingOffer >= 0 && _nextAnnouncementAt <= 0f)
01118: 		{
01119: 			_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
01120: 		}
01121: 		Dynamic.Add(CreatePanel("Seats", FromTop(24f, 138f, 164f, 347f), Surface));
01122: 		StageText("SeatTitle", T("竞拍席位", "Bidders"), 11f, 36f, 149f, 140f, 20f, AuctionUiStyle.Gold, bold: true);
01123: 		StageText("PlayerSeat", T("你    你的席位", "You · Your seat"), 11f, 36f, 181f, 140f, 20f, Color.white, bold: true);
01124: 		Dynamic.Add(CreatePanel("PlayerBidBubble", FromTop(36f, 207f, 140f, 27f), SurfaceRaised));
01125: 		StageText("PlayerBidText", (selected.PendingOffer >= 0 && selected.PendingOffer > 0) ? (T("已报 ", "Bid ") + Money(selected.PendingOffer)) : ((selected.PlayerOffer > 0) ? (T("上轮 ", "Last ") + Money(selected.PlayerOffer)) : T("本轮尚未报价", "No bid this round")), 10f, 42f, 212f, 128f, 18f, AuctionUiStyle.Muted);
01126: 		StageText("PlayerHint", T("可自由输入报价金额", "Enter any amount"), 8f, 36f, 238f, 140f, 14f, AuctionUiStyle.Muted);
01127: 		for (int i = 0; i < 3; i++)
01128: 		{
01129: 			int num = 263 + i * 72;
01130: 			Dynamic.Add(CreateAccent("SeatDivider" + i, FromTop(36f, num - 8, 140f, 1f), AuctionUiStyle.Border));
01131: 			int num2 = ((selected.PendingOffer >= 0 && selected.BiddersShown > i && !selected.Sealed) ? AuctionEngine.PreviewOffers(selected, selected.Round + 1)[i] : 0);
01132: 			int num3 = AuctionEngine.ExitReason(selected, i);
01133: 			string text = ((num3 != 0) ? AuctionEngine.ExitText(num3) : ((selected.PendingOffer >= 0 && selected.BiddersShown > i) ? (selected.Sealed ? T("暗标已提交", "Sealed bid") : ((num2 > 0) ? (T("报价 ", "Bid ") + Money(num2)) : T("本轮未报价", "No bid"))) : ((selected.Round > 0 && selected.LastOffers[i] > 0) ? (T("报价 ", "Bid ") + Money(selected.LastOffers[i])) : T("等待发言", "Waiting"))));
01134: 			GameObject val = NewObject("BidderPortrait" + i, (Transform)(object)_layout, typeof(Image));
01135: 			SetRect(val.GetComponent<RectTransform>(), FromTop(36f, num, 22f, 26f));
01136: 			Image component = val.GetComponent<Image>();
01137: 			component.preserveAspect = true;
01138: 			((Graphic)component).raycastTarget = false;
01139: 			try
01140: 			{
01141: 				SpriteDict instance = SpriteDict.Instance;
01142: 				component.sprite = ((instance != null) ? instance.GetSprite((new string[3] { "joe", "merchant", "landlord" })[i]) : null);
01143: 			}
01144: 			catch
01145: 			{
01146: 			}
01147: 			if ((Object)(object)component.sprite == (Object)null)
01148: 			{
01149: 				((Graphic)component).color = SurfaceRaised;
01150: 			}
01151: 			Dynamic.Add(val);
01152: 			StageText("Seat" + i, AuctionText.BidderName(i), 10f, 64f, num + 4, 112f, 18f, Color.white, bold: true);
01153: 			bool flag = selected.PendingOffer >= 0 && selected.BiddersShown == i + 1;
01154: 			bool flag2 = !selected.Sealed && selected.HighestBidder == i;
01155: 			Dynamic.Add(CreatePanel("BidderSpeech" + i, FromTop(46f, num + 29, 130f, 24f), (flag2 || flag) ? AuctionUiStyle.Gold : AuctionUiStyle.Border));
01156: 			StageText("BidderSpeechText" + i, text, 10f, 52f, num + 33, 118f, 17f, (flag2 || flag) ? AuctionUiStyle.Gold : AuctionUiStyle.Muted);
01157: 			StageText("BidderMood" + i, AuctionRivals.Mood(selected, i), 8f, 36f, num + 56, 140f, 13f, AuctionUiStyle.Muted);
01158: 		}
01159: 		Dynamic.Add(CreatePanel("AuctionStage", FromTop(200f, 138f, 490f, 347f), Surface));
01160: 		StageText("LotHeading", AuctionCatalog.CategoryName(selected.Category), 13f, 212f, 149f, 266f, 23f, Color.white, bold: true);
01161: 		((TMP_Text)StageText("RoundHeading", (selected.Sealed ? T("暗标竞拍", "Sealed") : T("公开竞拍", "Open")) + " · " + T("第 ", "Round ") + (selected.Round + 1) + " / 5", 10f, 482f, 151f, 196f, 20f, AuctionUiStyle.Gold, bold: true)).alignment = (TextAlignmentOptions)260;
01162: 		Dynamic.Add(CreateContainerImage("SealedContainer", selected.Category, FromTop(212f, 207f, 112f, 100f)));
01163: 		StageText("ExteriorClue", AuctionEngine.SurfaceClue(selected), 12f, 336f, 211f, 342f, 31f, Color.white, bold: true);
01164: 		string text2 = (((selected.IntelMask & 2) != 0) ? AuctionText.DisplayLog(AuctionEngine.Intel(selected, ((selected.IntelMask & 0x10) == 0) ? 1 : 4)) : T("估值线索：尚未购买", "Value clue: not purchased"));
01165: 		StageText("Clues", text2 + "\n" + MainMix(selected) + T(" · 件数未知\n", " · Count unknown\n") + AuctionEngine.LeadRule(selected), 10f, 336f, 249f, 342f, 60f, AuctionUiStyle.Muted);
01166: 		Dynamic.Add(CreateAccent("DialogueRule", FromTop(212f, 321f, 466f, 1f), AuctionUiStyle.Border));
01167: 		StageText("DialogueTitle", T("竞拍播报", "Auction Feed"), 11f, 212f, 332f, 250f, 20f, Color.white, bold: true);
01168: 		Dynamic.Add(CreateButton("AnnouncementSpeed", _fastAnnouncements ? T("报价速度 ×4", "Speed ×4") : T("报价速度 ×1", "Speed ×1"), FromTop(588f, 330f, 90f, 20f), ToggleAnnouncementSpeed, enabled: true));
01169: 		CreateBroadcast(selected);
01170: 		AddIntelButton(1, FromTop(212f, 453f, 114f, 26f), T("估值区间", "Value Range"));
01171: 		AddIntelButton(2, FromTop(336f, 453f, 114f, 26f), T("四件抽检", "Sample Four"));
01172: 		Dynamic.Add(CreateButton("MoreIntel", T("更多情报", "More Intel"), FromTop(460f, 453f, 114f, 26f), RenderIntelMenu, selected.PendingOffer < 0));
01173: 		Dynamic.Add(CreatePanel("BidTools", FromTop(702f, 138f, 234f, 347f), Surface));
01174: 		StageText("BidTitle", T("本轮自由报价", "Your Bid This Round"), 12f, 714f, 149f, 210f, 22f, AuctionUiStyle.Gold, bold: true);
01175: 		Dynamic.Add(CreatePanel("BidDisplay", FromTop(714f, 179f, 210f, 42f), Frame));
01176: 		((TMP_Text)StageText("BidValue", (selected.PendingOffer < 0) ? ((_bidInput.Length == 0) ? "0" : _bidInput) : ((selected.PendingOffer > 0) ? selected.PendingOffer.ToString("N0") : "—"), 23f, 722f, 183f, 194f, 38f, Color.white)).alignment = (TextAlignmentOptions)260;
01177: 		StageText("MultiplierBase", T("倍率基准 ", "Base ") + Money(AuctionEngine.MultiplierBase(selected)) + T(" · 现金上限 ", " · Cash ") + Money(PlayerStore.Instance.GetCash()), 8f, 714f, 228f, 210f, 23f, AuctionUiStyle.Muted);
01178: 		bool flag3 = selected.PendingOffer < 0;
01179: 		int[] array = new int[4] { 11, 13, 16, 20 };
01180: 		for (int j = 0; j < array.Length; j++)
01181: 		{
01182: 			int m = array[j];
01183: 			Dynamic.Add(CreateButton("BidMultiplier" + m, (m == 20) ? "×2" : ("×1." + (m - 10)), FromTop(714 + j * 54, 259f, 48f, 23f), delegate
01184: 			{
01185: 				SetBidMultiplier(m);
01186: 			}, flag3));
01187: 		}
01188: 		for (int num4 = 1; num4 <= 9; num4++)
01189: 		{
01190: 			int d = num4;
01191: 			Dynamic.Add(CreateButton("Digit" + d, d.ToString(), FromTop(714 + (d - 1) % 3 * 72, 290 + (d - 1) / 3 * 31, 66f, 27f), delegate
01192: 			{
01193: 				AddDigit(d);
01194: 			}, flag3));
01195: 		}
01196: 		Dynamic.Add(CreateButton("Clear", T("清空", "Clear"), FromTop(714f, 383f, 66f, 27f), delegate
01197: 		{
01198: 			_bidInput = string.Empty;
01199: 			RenderStage();
01200: 		}, flag3));
01201: 		Dynamic.Add(CreateButton("Digit0", "0", FromTop(786f, 383f, 66f, 27f), delegate
01202: 		{
01203: 			AddDigit(0);
01204: 		}, flag3));
01205: 		Dynamic.Add(CreateButton("Backspace", T("退格", "Del"), FromTop(858f, 383f, 66f, 27f), Backspace, flag3));
01206: 		Dynamic.Add(CreateButton("Bid", flag3 ? T("提交本轮报价", "Submit Bid") : T("等待对手报价…", "Rivals are bidding…"), FromTop(714f, 419f, 210f, 27f), Bid, flag3));
01207: 		Dynamic.Add(CreateButton("Withdraw", T("放弃本箱竞拍", "Withdraw from This Lot"), FromTop(714f, 453f, 210f, 26f), RenderWithdrawConfirm, flag3));
01208: 		Dynamic.Add(CreateAccent("FooterRule", FromTop(24f, 499f, 912f, 1f), AuctionUiStyle.Border));
01209: 		Dynamic.Add(CreateButton("OrdersLocked", T("采购委托", "Orders"), FromTop(24f, 511f, 100f, 28f), delegate
01210: 		{
01211: 		}, enabled: false));
01212: 		Dynamic.Add(CreateButton("CollectionLocked", T("藏品收集册", "Collection"), FromTop(136f, 511f, 100f, 28f), delegate
01213: 		{
01214: 		}, enabled: false));
01215: 		Dynamic.Add(CreateButton("HistoryLocked", T("历史账本", "Ledger"), FromTop(248f, 511f, 100f, 28f), delegate
01216: 		{
01217: 		}, enabled: false));
01218: 		Dynamic.Add(CreateButton("Abandon", (selected.PlayerOffer > 0 && selected.PlayerOffer <= PlayerStore.Instance.GetCash()) ? T("保持上轮报价", "Keep Last Bid") : T("本轮不出价", "Pass This Round"), FromTop(366f, 511f, 114f, 28f), Abandon, flag3));
01219: 		StageText("FooterHint", T("播报可滚动查看 · 不打断自动报价", "Scroll the feed · Rival bids continue automatically"), 8f, 492f, 519f, 320f, 18f, AuctionUiStyle.Muted);
01220: 		Dynamic.Add(CreateButton("Back", T("返回四箱", "Four Lots"), FromTop(824f, 511f, 112f, 28f), delegate
01221: 		{
01222: 			RenderLots(T("选择一个集装箱。", "Choose a container."));
01223: 		}, enabled: true));
01224: 		SetStatus((message == null) ? T("报价与成交操作分开，允许自由输入，不提高最低报价。", "Enter any amount, then submit. No rising minimum bid.") : AuctionText.DisplayLog(message));
01225: 	}
01226: 
01227: 	private static TextMeshProUGUI StageText(string name, string text, float size, float x, float top, float width, float height, Color color, bool bold = false)
01228: 	{
01229: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
01230: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
01231: 		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
01232: 		TextMeshProUGUI val = CreateLabel(name, text, size, new Vector2(x, 0f - top), new Vector2(width, height), (FontStyles)(bold ? 1 : 0));
01233: 		((Graphic)val).color = color;
01234: 		Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
01235: 		return val;
01236: 	}
01237: 
01238: 	private static void AddIntelButton(int kind, Rect bounds, string caption)
01239: 	{
01240: 		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
01241: 		AuctionLot selected = _selected;
01242: 		bool flag = (selected.IntelMask & (1 << kind)) != 0;
01243: 		string text = ((!flag) ? (caption + " " + AuctionNightMarket.IntelText(selected, kind)) : ((kind == 2) ? T("查看抽检", "View Sample") : ((kind == 1) ? T("估值已查询", "Value Purchased") : T("已查询", "Purchased"))));
01244: 		Dynamic.Add(CreateButton("Intel" + kind, text, bounds, delegate
01245: 		{
01246: 			BuyIntel(kind);
01247: 		}, selected.PendingOffer < 0 && (!flag || kind == 2) && (kind != 4 || (selected.IntelMask & 2) != 0)));
01248: 	}
01249: 
01250: 	private static void RenderIntelMenu()
01251: 	{
01252: 		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
01253: 		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
01254: 		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
01255: 		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
01256: 		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
01257: 		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
01258: 		if (_selected != null && _selected.PendingOffer < 0)
01259: 		{
01260: 			_page = Page.Intel;
01261: 			ClearDynamic();
01262: 			Dynamic.Add(CreatePanel("IntelMenu", FromTop(220f, 157f, 520f, 300f), Surface));
01263: 			StageText("IntelMenuTitle", T("更多情报", "More Intel"), 20f, 244f, 177f, 472f, 32f, Color.white, bold: true);
01264: 			string[] array = new string[3]
01265: 			{
01266: 				T("探查对手心理价", "Estimate Rival Limit"),
01267: 				T("识别搅局抬价", "Assess Bluffing"),
01268: 				T("重新鉴定估值", "Reappraise Value")
01269: 			};
01270: 			int[] array2 = new int[3] { 0, 3, 4 };
01271: 			for (int i = 0; i < array2.Length; i++)
01272: 			{
01273: 				AddIntelButton(array2[i], FromTop(244f, 233 + i * 48, 472f, 34f), array[i]);
01274: 			}
01275: 			StageText("IntelMenuHint", T("情报结果保存在竞拍播报中；复鉴需先查询估值。", "Intel is recorded in the feed. Reappraisal requires Value Range."), 10f, 244f, 382f, 472f, 30f, AuctionUiStyle.Muted);
01276: 			Dynamic.Add(CreateButton("IntelMenuBack", T("返回竞拍", "Back to Bidding"), FromTop(244f, 417f, 180f, 28f), delegate
01277: 			{
01278: 				RenderStage();
01279: 			}, enabled: true));
01280: 			SetStatus(T("情报费用及使用规则保持不变。", "Existing intel costs and rules apply."));
01281: 		}
01282: 	}
01283: 
01284: 	private static void Bid()
01285: 	{
01286: 		AuctionLot selected = _selected;
01287: 		if (selected == null || selected.Finished || selected.Lost || SessionComplete)
01288: 		{
01289: 			return;
01290: 		}
01291: 		if (!int.TryParse(_bidInput, out var result))
01292: 		{
01293: 			RenderStage(T("请用数字键盘输入完整报价。", "Enter a complete bid using the keypad."));
~~~

~~~csharp
01334: 		case "继续":
01335: 			AuctionState.SaveLots(_lots);
01336: 			_bidInput = string.Empty;
01337: 			RenderStage(T("本轮已揭价；未达到提前成交条件，继续下一轮。", "Bids revealed. No early winner; next round."));
01338: 			break;
01339: 		default:
01340: 			RenderStage(text);
01341: 			break;
01342: 		}
01343: 	}
01344: 
01345: 	private static void CompletePurchase(AuctionLot lot)
01346: 	{
01347: 		if (lot == null || !lot.PlayerLeading || lot.Lost)
01348: 		{
01349: 			return;
01350: 		}
01351: 		try
01352: 		{
01353: 			if (PlayerStore.Instance.GetCash() < lot.CurrentBid)
01354: 			{
01355: 				throw new InvalidOperationException("现金不足");
01356: 			}
01357: 			List<string> list = ((lot.Contents.Count > 0) ? new List<string>(lot.Contents) : AuctionCatalog.GenerateContents(lot));
01358: 			if (list.Count == 0)
01359: 			{
01360: 				throw new InvalidOperationException("该类别没有可用物品池");
01361: 			}
01362: 			lot.EstimatedValue = AuctionCatalog.ContentsValue(list);
01363: 			AuctionState.CommitPurchase(lot, _lots, list);
01364: 			Core.Log?.Msg("Auction won: category=" + lot.Category.ToString() + ", price=" + lot.CurrentBid + ", items=" + list.Count + ".");
01365: 		}
01366: 		catch (Exception ex)
01367: 		{
01368: 			lot.Finished = false;
01369: 			RenderStage(T("无法完成成交：", "Purchase failed: ") + AuctionText.DisplayLog(ex.Message));
01370: 			Core.Log?.Warning("Auction purchase failed: " + ex.Message);
01371: 			return;
01372: 		}
01373: 		RenderSettlement(lot, won: true);
01374: 	}
01375: 
01376: 	private static void Abandon()
01377: 	{
01378: 		if (_selected != null && _selected.PendingOffer < 0)
01379: 		{
01380: 			string text = AuctionEngine.StagePass(_selected, PlayerStore.Instance.GetCash());
01381: 			if (text == "已提交")
01382: 			{
01383: 				AuctionState.SaveLots(_lots);
01384: 			}
01385: 			if (text == "已提交")
01386: 			{
01387: 				_nextAnnouncementAt = Time.unscaledTime + AnnouncementInterval;
01388: 			}
01389: 			RenderStage((text == "已提交") ? T("已保持原价或跳过本轮，等待对手依次报价。", "Bid kept or passed. Waiting for rivals.") : text);
01390: 		}
01391: 	}
01392: 
01393: 	private static void RenderWithdrawConfirm()
01394: 	{
01395: 		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
01396: 		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
01397: 		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
01398: 		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
01399: 		if (_selected != null && _selected.PendingOffer < 0 && !_selected.Finished && !_selected.Lost)
01400: 		{
01401: 			_page = Page.Withdraw;
01402: 			ClearDynamic();
01403: 			_stageVisible = false;
01404: 			Dynamic.Add(CreatePanel("WithdrawPanel", new Rect(174f, 150f, 612f, 245f), AuctionCatalog.CategoryColor(_selected.Category)));
01405: 			AddStageLabel("WithdrawTitle", T("放弃本箱竞拍？", "Withdraw from this lot?"), 23f, 214f, -166f, 530f, 35f, (FontStyles)1);
01406: 			AddStageLabel("WithdrawDetails", T("确认后不可继续竞拍这只集装箱，也不会扣除箱款。\nAI会继续竞价，并直接显示最终结果。\n已支付的入场费和情报费不退；本箱计入今晚的三场竞拍。", "You cannot rejoin this lot. No container payment is taken.\nAI bidding continues; the final result appears immediately.\nEntry and intel fees are not refunded; this counts as one auction."), 15f, 214f, -220f, 530f, 88f, (FontStyles)0);
01407: 			Dynamic.Add(CreateButton("ConfirmWithdraw", T("确认放弃", "Confirm Withdraw"), new Rect(250f, 181f, 190f, 39f), Withdraw, enabled: true));
01408: 			Dynamic.Add(CreateButton("CancelWithdraw", T("继续竞拍", "Continue"), new Rect(520f, 181f, 190f, 39f), delegate
01409: 			{
01410: 				RenderStage();
01411: 			}, enabled: true));
01412: 			SetStatus(T("放弃本箱竞拍需要确认；尚未提交新一轮报价。", "Confirm withdrawal before submitting the next bid."));
01413: 		}
01414: 	}
01415: 
01416: 	private static void Withdraw()
01417: 	{
01418: 		AuctionLot selected = _selected;
01419: 		if (!AuctionEngine.Withdraw(selected))
01420: 		{
01421: 			RenderStage(T("当前不能退出竞拍，请等待本轮揭价。", "Wait for this round's bids before withdrawing."));
01422: 			return;
01423: 		}
01424: 		_nextAnnouncementAt = 0f;
01425: 		AuctionState.SaveLots(_lots);
01426: 		RenderSettlement(selected, won: false);
01427: 	}
01428: 
01429: 	private static void AddDigit(int digit)
01430: 	{
01431: 		string text = _bidInput + digit;
01432: 		if (text.Length <= 9 && int.TryParse(text, out var _))
01433: 		{
01434: 			_bidInput = text.TrimStart('0');
01435: 			if (_bidInput.Length == 0)
01436: 			{
01437: 				_bidInput = "0";
01438: 			}
~~~

~~~csharp
01596: 		Dynamic.Add(CreateButton("InspectionBack", T("返回竞拍", "Back to Bidding"), new Rect(384f, 108f, 192f, 28f), delegate
01597: 		{
01598: 			RenderStage();
01599: 		}, enabled: true));
01600: 		SetStatus(T("抽检结果已保存，再次查看不收费。", "Sample saved. Viewing it again is free."));
01601: 	}
01602: 
01603: 	private static void RenderSettlement(AuctionLot lot, bool won)
01604: 	{
01605: 		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
01606: 		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
01607: 		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
01608: 		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
01609: 		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
01610: 		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
01611: 		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
01612: 		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
01613: 		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
01614: 		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
01615: 		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
01616: 		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
01617: 		_page = Page.Settlement;
01618: 		_settlementLot = lot;
01619: 		_settlementWon = won;
01620: 		ClearDynamic();
01621: 		_stageVisible = false;
01622: 		_nextAnnouncementAt = 0f;
01623: 		Color color = (won ? new Color(0.33f, 0.69f, 0.49f) : new Color(0.66f, 0.37f, 0.4f));
01624: 		Dynamic.Add(CreatePanel("SettlementPanel", new Rect(174f, 112f, 612f, 310f), won ? new Color(0.33f, 0.69f, 0.49f) : new Color(0.66f, 0.37f, 0.4f)));
01625: 		Dynamic.Add(CreateAccent("SettlementAccent", new Rect(177f, 389f, 606f, 30f), color));
01626: 		string text = (lot.Withdrawn ? T("已放弃竞拍", "Withdrawn") : ((won && lot.Quality == AuctionQuality.Hidden) ? T("异常货柜成交", "Exceptional Lot Won") : (won ? T("竞拍成交", "Auction Won") : T("竞拍落标", "Outbid"))));
01627: 		AddStageLabel("SettlementTitle", text, 23f, 214f, -132f, 532f, 36f, (FontStyles)1);
01628: 		Dynamic.Add(CreateContainerImage("SettlementCrate", lot.Category, new Rect(210f, 266f, 190f, 110f)));
01629: 		AddStageLabel("SettlementStamp", lot.Withdrawn ? T("已退出", "OUT") : (won ? T("成  交", "SOLD") : T("落  标", "LOST")), 16f, 268f, -251f, 90f, 26f, (FontStyles)1);
01630: 		AddStageLabel("SettlementRivalMoods", string.Join("\n", from i in Enumerable.Range(0, 3)
01631: 			select AuctionText.BidderName(i) + " · " + AuctionRivals.Mood(lot, i) + " · " + AuctionState.RivalCash(lot, i) + "¢"), 9f, 214f, -283f, 192f, 52f, (FontStyles)0);
01632: 		int num = AuctionState.VenueFee(lot.Venue);
01633: 		string text2 = ((lot.HighestBidder < 0) ? T("无人出价，本箱流拍", "No bids. Lot unsold") : (T("得标者  ", "Winner  ") + AuctionText.BidderName(lot.HighestBidder) + T("\n最终成交价  ", "\nWinning bid  ") + lot.CurrentBid + " ¢"));
01634: 		string text3 = (won ? (T("获得  ", "Received  ") + AuctionCatalog.CategoryName(lot.Category) + " ×1\n" + T("内部物品  ", "Items inside  ") + lot.Contents.Count + T(" 件\n", "\n") + T("第 ", "Round ") + lot.Round + T(" 轮成交  ", " price  ") + lot.CurrentBid + " ¢\n" + T("当晚入场 / 本箱情报  ", "Entry / intel  ") + num + " / " + lot.IntelSpent + " ¢\n" + T("总货值与盈亏  开箱后揭晓", "Value and profit revealed after opening")) : (lot.Withdrawn ? (T("AI已完成剩余竞价\n最终轮次  ", "AI bidding completed\nFinal round  ") + lot.Round + "\n" + text2 + T("\n箱款未扣除\n入场费与情报费不退", "\nNo container payment\nEntry and intel fees are not refunded")) : (T("最终轮次  ", "Final round  ") + lot.Round + "\n" + text2 + T("\n未扣除箱款", "\nNo container payment"))));
01635: 		AddStageLabel("SettlementSummary", text3, 14f, 430f, -174f, 318f, 160f, (FontStyles)1);
01636: 		Dynamic.Add(CreatePanel("SettlementDialogue", new Rect(210f, 132f, 540f, 68f), color));
01637: 		AddStageLabel("SettlementLog", T("竞拍播报\n", "Auction Feed\n") + RecentLog(lot), 11f, 222f, -346f, 516f, 51f, (FontStyles)0);
01638: 		Dynamic.Add(CreateButton("Continue", SessionComplete ? T("查看本晚结果", "Tonight's Results") : T("继续下一场", "Next Auction"), new Rect(390f, 66f, 180f, 38f), delegate
01639: 		{
01640: 			RenderLots(SessionComplete ? T("今晚三场竞拍已经结束。", "All three auctions are complete.") : T("选择下一只集装箱。", "Choose the next container."));
01641: 		}, enabled: true));
01642: 		SetStatus((lot.Withdrawn ? T("你已退出；AI竞价结束，结果已公布。", "You withdrew. AI bidding is complete; the result is shown.") : (won ? T("本箱已登记；回店取货，全部开出后揭晓盈亏。", "Lot registered. Collect it at the shop; profit appears after opening.") : T("本箱由其他竞拍者获得。", "Another bidder won this lot."))) + T("  本晚进度 ", "  Tonight ") + ResolvedAuctions + " / 3.");
01643: 	}
01644: 
01645: 	private static void RenderHistory()
01646: 	{
01647: 		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
01648: 		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
01649: 		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
01650: 		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
01651: 		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
01652: 		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
01653: 		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
01654: 		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
01655: 		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
01656: 		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
01657: 		_page = Page.History;
01658: 		ClearDynamic();
01659: 		Dynamic.Add(CreatePanel("HistoryPanel", FromTop(24f, 138f, 912f, 347f), Surface));
01660: 		AddStageLabel("HistoryTitle", T("历史账本", "Auction Ledger"), 22f, 42f, -149f, 800f, 35f, (FontStyles)1);
01661: 		int[] array = new int[6] { 42, 178, 370, 480, 590, 721 };
01662: 		int[] array2 = new int[6] { 128, 184, 102, 102, 123, 193 };
01663: 		string[] array3 = ((!AuctionText.IsChinese(AuctionText.CurrentLocaleCode)) ? new string[6] { "Day / Venue", "Container", "Price", "Cargo Value", "Net Profit", "Status" } : new string[6] { "日期 / 场次", "集装箱", "成交价", "实际货值", "净收益", "状态" });
01664: 		Dynamic.Add(CreateAccent("HistoryRule", FromTop(42f, 210f, 876f, 1f), AuctionUiStyle.Border));
01665: 		for (int i = 0; i < 6; i++)
01666: 		{
01667: 			AddStageLabel("HistoryColumn" + i, array3[i], 12f, array[i], -187f, array2[i], 20f, (FontStyles)1);
01668: 		}
01669: 		List<(string[], long, bool)> list = new List<(string[], long, bool)>();
01670: 		foreach (string item in AuctionState.Read("history").Split(';').AsEnumerable()
01671: 			.Reverse())
01672: 		{
01673: 			string[] array4 = item.Split(',');
01674: 			int result3;
01675: 			int result4;
01676: 			if (array4.Length == 6 && int.TryParse(array4[1], out var result) && result >= 0 && result <= 3)
01677: 			{
01678: 				long.TryParse(array4[5], out var result2);
01679: 				list.Add((new string[6]
01680: 				{
01681: 					T("第 ", "Day ") + array4[0],
01682: 					AuctionCatalog.CategoryName((ContainerCategory)result),
01683: 					array4[3],
01684: 					array4[4],
01685: 					array4[5],
01686: 					T("已结算", "Settled")
01687: 				}, result2, true));
01688: 			}
01689: 			else if ((array4.Length == 9 || array4.Length == 10) && int.TryParse(array4[1], out result3) && result3 >= 0 && result3 <= 2 && int.TryParse(array4[2], out result4) && result4 >= 0 && result4 <= 3)
01690: 			{
01691: 				bool flag = array4.Length == 9 || array4[9] == "1";
01692: 				long.TryParse(array4[6], out var result5);
01693: 				list.Add((new string[6]
01694: 				{
01695: 					array4[0] + " · " + AuctionRules.VenueName((AuctionVenue)result3),
01696: 					AuctionCatalog.CategoryName((ContainerCategory)result4),
01697: 					array4[4],
01698: 					flag ? array4[5] : T("待揭晓", "Pending"),
01699: 					flag ? array4[6] : "—",
01700: 					flag ? T("已结算", "Settled") : T("尚未开完", "Unopened cargo")
01701: 				}, result5, flag));
01702: 			}
01703: 			if (list.Count == 8)
01704: 			{
01705: 				break;
01706: 			}
01707: 		}
01708: 		if (list.Count == 0)
01709: 		{
01710: 			AddStageLabel("HistoryEmpty", T("暂无成交记录。", "No auction records yet."), 15f, 42f, -234f, 876f, 30f, (FontStyles)0);
01711: 		}
01712: 		for (int j = 0; j < list.Count; j++)
01713: 		{
01714: 			int num = 222 + j * 28;
01715: 			for (int k = 0; k < 6; k++)
01716: 			{
01717: 				TextMeshProUGUI val = CreateLabel("HistoryCell" + j + "_" + k, list[j].Item1[k], 12f, new Vector2((float)array[k], (float)(-num)), new Vector2((float)array2[k], 23f), (FontStyles)0);
01718: 				((TMP_Text)val).enableAutoSizing = true;
01719: 				((TMP_Text)val).fontSizeMin = 8f;
01720: 				((TMP_Text)val).fontSizeMax = 12f;
01721: 				((TMP_Text)val).enableWordWrapping = false;
01722: 				((TMP_Text)val).overflowMode = (TextOverflowModes)1;
01723: 				if ((k == 3 || k == 5) && !list[j].Item3)
01724: 				{
01725: 					((Graphic)val).color = AuctionUiStyle.Gold;
01726: 				}
01727: 				if (k == 4 && list[j].Item3)
01728: 				{
01729: 					((Graphic)val).color = ((list[j].Item2 < 0) ? new Color(0.82f, 0.58f, 0.55f) : new Color(0.61f, 0.78f, 0.69f));
01730: 				}
01731: 				Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
01732: 			}
01733: 		}
01734: 		Dynamic.Add(CreateButton("HistoryBack", AuctionState.TryGetVenue(out var _) ? T("返回四箱", "Four Lots") : T("返回场次选择", "Choose Venue"), FromTop(716f, 511f, 220f, 28f), ReturnLobby, enabled: true));
01735: 		SetStatus(T("最近八次成交；货值与净收益在全部开箱后揭晓，费用口径保持原规则。", "Last eight purchases. Value and net profit reveal after opening; existing fee rules apply."));
01736: 	}
01737: 
01738: 	private static void AddStageLabel(string name, string text, float size, float x, float y, float width, float height, FontStyles style = (FontStyles)0)
01739: 	{
01740: 		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
01741: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
01742: 		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
01743: 		TextMeshProUGUI val = CreateLabel(name, text, size, new Vector2(x, y), new Vector2(width, height), style);
01744: 		Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
01745: 	}
01746: 
01747: 	private static string RecentLog(AuctionLot lot)
01748: 	{
01749: 		int count = lot.BidLog.Count;
01750: 		if (count != 0)
01751: 		{
01752: 			return string.Join("\n", lot.BidLog.GetRange(Math.Max(0, count - 2), Math.Min(2, count)).Select(AuctionText.DisplayLog));
01753: 		}
01754: 		return T("等待报价", "Waiting for bids");
01755: 	}
01756: 
01757: 	private static string BidDialogue(AuctionLot lot)
01758: 	{
01759: 		if (lot.BidLog.Count != 0)
01760: 		{
01761: 			return string.Join("\n", lot.BidLog.Select(AuctionText.DisplayLog));
01762: 		}
01763: 		return AuctionText.DisplayLog("拍卖师：最多五轮，每轮揭价；领先足够可提前成交。");
01764: 	}
01765: 
01766: 	private static GameObject CreateNativeSpeechBubble(string name, Rect bounds, bool speaking)
01767: 	{
01768: 		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
01769: 		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
01770: 		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
01771: 		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
01772: 		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
01773: 		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
01774: 		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
01775: 		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
01776: 		GameObject val = NewObject(name, (Transform)(object)_layout, typeof(Image));
01777: 		SetRect(val.GetComponent<RectTransform>(), bounds);
01778: 		Image component = val.GetComponent<Image>();
01779: 		try
01780: 		{
01781: 			Image val2 = DialogUIManager.Instance?.clientBubble?.backgroundImage;
01782: 			if ((Object)(object)val2 != (Object)null && (Object)(object)val2.sprite != (Object)null)
01783: 			{
01784: 				component.sprite = val2.sprite;
01785: 				component.type = val2.type;
01786: 				((Graphic)component).color = (Color)(speaking ? Color.white : new Color(0.78f, 0.78f, 0.78f, 1f));
01787: 			}
01788: 			else
01789: 			{
01790: 				((Graphic)component).color = (speaking ? SurfaceRaised : Frame);
01791: 			}
01792: 		}
01793: 		catch
01794: 		{
01795: 			((Graphic)component).color = (speaking ? SurfaceRaised : Frame);
01796: 		}
01797: 		((Graphic)component).raycastTarget = false;
01798: 		return val;
01799: 	}
01800: 
01801: 	private static void CreateBroadcast(AuctionLot lot)
01802: 	{
01803: 		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
01804: 		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
01805: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
01806: 		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
01807: 		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
01808: 		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
01809: 		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
01810: 		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
01811: 		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
01812: 		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
01813: 		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
01814: 		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
01815: 		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
01816: 		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
01817: 		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
01818: 		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
01819: 		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
01820: 		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
01821: 		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
01822: 		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
01823: 		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
01824: 		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
01825: 		GameObject val = NewObject("BroadcastScroll", (Transform)(object)_layout, typeof(ScrollRect));
01826: 		SetRect(val.GetComponent<RectTransform>(), FromTop(212f, 358f, 466f, 85f));
01827: 		Dynamic.Add(val);
01828: 		GameObject val2 = NewObject("BroadcastViewport", val.transform, typeof(Image), typeof(RectMask2D));
01829: 		RectTransform component = val2.GetComponent<RectTransform>();
01830: 		component.anchorMin = Vector2.zero;
01831: 		component.anchorMax = Vector2.one;
01832: 		component.offsetMin = Vector2.zero;
~~~

~~~csharp
01851: 		GameObject val5 = NewObject("BroadcastScrollbar", val.transform, typeof(Image), typeof(Scrollbar));
01852: 		RectTransform component4 = val5.GetComponent<RectTransform>();
01853: 		component4.anchorMin = new Vector2(1f, 0f);
01854: 		component4.anchorMax = new Vector2(1f, 1f);
01855: 		component4.pivot = new Vector2(1f, 0.5f);
01856: 		component4.anchoredPosition = Vector2.zero;
01857: 		component4.sizeDelta = new Vector2(10f, 0f);
01858: 		((Graphic)val5.GetComponent<Image>()).color = new Color(0.08f, 0.09f, 0.1f, 1f);
01859: 		GameObject val6 = NewObject("BroadcastHandle", val5.transform, typeof(Image));
01860: 		Stretch(val6.GetComponent<RectTransform>());
01861: 		Image component5 = val6.GetComponent<Image>();
01862: 		((Graphic)component5).color = new Color(0.65f, 0.63f, 0.52f, 1f);
01863: 		Scrollbar component6 = val5.GetComponent<Scrollbar>();
01864: 		component6.handleRect = val6.GetComponent<RectTransform>();
01865: 		((Selectable)component6).targetGraphic = (Graphic)(object)component5;
01866: 		component6.direction = (Direction)2;
01867: 		_broadcastScroll = val.GetComponent<ScrollRect>();
01868: 		_broadcastScroll.viewport = component;
01869: 		_broadcastScroll.content = component3;
01870: 		_broadcastScroll.horizontal = false;
01871: 		_broadcastScroll.vertical = true;
01872: 		_broadcastScroll.movementType = (MovementType)2;
01873: 		_broadcastScroll.verticalScrollbar = component6;
01874: 		Canvas.ForceUpdateCanvases();
01875: 		_broadcastScroll.verticalNormalizedPosition = _broadcastPosition;
01876: 	}
01877: 
01878: 	private static string LeaderName(AuctionLot lot)
01879: 	{
01880: 		if (lot.HighestBidder != 3)
01881: 		{
01882: 			if (lot.HighestBidder < 0 || lot.HighestBidder >= AuctionEngine.Names.Length)
01883: 			{
01884: 				return T("暂无", "None");
01885: 			}
01886: 			return AuctionText.BidderName(lot.HighestBidder);
01887: 		}
01888: 		return T("你", "You");
01889: 	}
01890: 
01891: 	private static string ClueRange(AuctionLot lot)
01892: 	{
01893: 		(int, int, bool) tuple = AuctionEngine.ValueRange(lot, 0);
01894: 		return tuple.Item1 + " - " + tuple.Item2;
01895: 	}
01896: 
01897: 	private static string Risk(AuctionLot lot)
01898: 	{
01899: 		if (lot.Category != ContainerCategory.Contraband)
01900: 		{
01901: 			if (lot.Category != ContainerCategory.Medical)
01902: 			{
01903: 				return T("中等", "Medium");
01904: 			}
01905: 			return T("较低", "Low");
01906: 		}
01907: 		return T("较高", "High");
01908: 	}
01909: 
01910: 	private static void TryDeliverPending()
01911: 	{
01912: 		if (!Core.StoreReady || (Object)(object)EmporiumEntry.Instance == (Object)null)
01913: 		{
01914: 			return;
01915: 		}
01916: 		for (int i = 0; i < 3; i++)
01917: 		{
01918: 			if (!AuctionState.TryGetPendingDelivery(out var category, out var seed, out var contents))
01919: 			{
01920: 				break;
01921: 			}
01922: 			if (PlayerStore.Instance.FindAllItem(true).Any(delegate(GameItem val2)
01923: 			{
01924: 				if (((val2 != null) ? val2.identifier : null) == "xiaowo_auction_container")
01925: 				{
01926: 					TagState tagReadonly = val2.GetTagReadonly("XIAOWO_AUCTION_CONTAINER_DATA");
01927: 					return ((tagReadonly == null) ? null : tagReadonly.GetString()?.Split('~').ElementAtOrDefault(1)) == seed.ToString();
01928: 				}
01929: 				return false;
01930: 			}))
01931: 			{
01932: 				AuctionState.AcknowledgeDelivery(seed);
01933: 				continue;
01934: 			}
01935: 			GameItem val = null;
01936: 			try
01937: 			{
01938: 				val = AuctionContainer.Create(owned: true, category, seed, contents);
01939: 				EmporiumEntry.Instance.TryAddToPlayerInv(val);
01940: 				if (val.parentInventory == null && (val.parents == null || val.parents.Count == 0))
01941: 				{
01942: 					GeneralHelper.SetItemOwned(val, false);
01943: 					Core.Log?.Warning("Auction delivery is waiting for 2x2 inventory space.");
01944: 					break;
01945: 				}
01946: 				AuctionState.AcknowledgeDelivery(seed);
01947: 				Core.Log?.Msg("Auction container delivered at the store: category=" + category.ToString() + ", items=" + contents.Count + ".");
01948: 			}
01949: 			catch (Exception ex)
01950: 			{
01951: 				if (val != null)
01952: 				{
01953: 					GeneralHelper.SetItemOwned(val, false);
01954: 				}
01955: 				Core.Log?.Warning("Auction delivery deferred: " + ex.Message);
~~~

~~~csharp
02138: 			float verticalNormalizedPosition = _broadcastScroll.verticalNormalizedPosition;
02139: 			Rect rect = _broadcastScroll.content.rect;
02140: 			float height = ((Rect)(ref rect)).height;
02141: 			rect = _broadcastScroll.viewport.rect;
02142: 			_broadcastPosition = ((height <= ((Rect)(ref rect)).height + 1f || verticalNormalizedPosition < 0.04f) ? 0f : verticalNormalizedPosition);
02143: 			_broadcastScroll = null;
02144: 		}
02145: 		foreach (GameObject item in Dynamic)
02146: 		{
02147: 			if ((Object)(object)item != (Object)null)
02148: 			{
02149: 				Object.Destroy((Object)(object)item);
02150: 			}
02151: 		}
02152: 		Dynamic.Clear();
02153: 	}
02154: 
02155: 	private static GameObject FindLayoutChild(Transform transform)
02156: 	{
02157: 		if ((Object)(object)transform == (Object)null)
02158: 		{
02159: 			return null;
02160: 		}
02161: 		while ((Object)(object)transform.parent != (Object)null && (Object)(object)transform.parent != (Object)(object)_layout)
02162: 		{
02163: 			transform = transform.parent;
02164: 		}
02165: 		return ((Component)transform).gameObject;
02166: 	}
02167: 
02168: 	internal static void TestEnterVenue(AuctionVenue venue)
02169: 	{
02170: 		EnterVenue(venue);
02171: 	}
02172: 
02173: 	internal static void TestSelect(int index)
02174: 	{
02175: 		SelectLot(_lots[index]);
02176: 	}
02177: 
02178: 	internal static void TestBid()
02179: 	{
02180: 		_bidInput = AuctionEngine.SuggestedBid(_selected).ToString();
02181: 		Bid();
02182: 	}
02183: 
02184: 	internal static void TestBidAmount(int amount)
02185: 	{
02186: 		_bidInput = amount.ToString();
02187: 		Bid();
02188: 	}
02189: 
02190: 	internal static void TestPass()
02191: 	{
02192: 		Abandon();
02193: 	}
02194: 
02195: 	internal static void TestAdvance()
02196: 	{
02197: 		AdvanceAuction();
02198: 	}
02199: 
02200: 	internal static void TestTickAnnouncement()
02201: 	{
02202: 		_nextAnnouncementAt = 0f;
02203: 		_tick = 0;
02204: 		Tick();
02205: 	}
02206: 
02207: 	internal static void TestBuyIntel(int kind)
02208: 	{
02209: 		BuyIntel(kind);
02210: 	}
02211: 
02212: 	private static void SetButton(GameObject obj, string text, Action action)
02213: 	{
02214: 		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
02215: 		//IL_002e: Expected O, but got Unknown
02216: 		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
02217: 		//IL_0045: Expected O, but got Unknown
02218: 		Button val = obj.GetComponent<Button>() ?? obj.GetComponentInChildren<Button>(true);
02219: 		SetButtonText(obj, text);
02220: 		if (!((Object)(object)val == (Object)null))
02221: 		{
02222: 			val.onClick = new ButtonClickedEvent();
02223: 			((UnityEvent)val.onClick).AddListener(new UnityAction(action.Invoke));
02224: 		}
02225: 	}
02226: 
02227: 	private static void SetButtonText(GameObject obj, string text)
02228: 	{
02229: 		TextMeshProUGUI[] componentsInChildren = obj.GetComponentsInChildren<TextMeshProUGUI>(true);
02230: 		for (int i = 0; i < componentsInChildren.Length; i++)
02231: 		{
02232: 			((TMP_Text)componentsInChildren[i]).text = text;
02233: 		}
02234: 	}
02235: 
02236: 	private static GameObject NewObject(string name, Transform parent, params Type[] components)
02237: 	{
02238: 		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
02239: 		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
02240: 		//IL_0043: Expected O, but got Unknown
02241: 		Type[] array = new Type[components.Length + 1];
02242: 		array[0] = typeof(RectTransform);
~~~

~~~csharp
02268: 	}
02269: 
02270: 	private static void Stretch(RectTransform rect)
02271: 	{
02272: 		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
02273: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
02274: 		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
02275: 		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
02276: 		rect.anchorMin = Vector2.zero;
02277: 		rect.anchorMax = Vector2.one;
02278: 		rect.offsetMin = Vector2.zero;
02279: 		rect.offsetMax = Vector2.zero;
02280: 	}
02281: 
02282: 	private static string MarketCaption()
02283: 	{
02284: 		AuctionVenue venue;
02285: 		string text = AuctionNightMarket.Summary(AuctionState.TryGetVenue(out venue) ? new AuctionVenue?(venue) : ((AuctionVenue?)null));
02286: 		int num = text.IndexOf(" · ", StringComparison.Ordinal);
02287: 		string text2 = ((num >= 0) ? text.Substring(0, num) : text);
02288: 		string text3 = ((num >= 0) ? text.Substring(num + 3) : T("外观与对手行为不代表箱内真实价值。", "Exterior clues and rival behaviour do not guarantee cargo value."));
02289: 		return "<color=#D8B778>" + T("今晚行情 · ", "Tonight · ") + text2 + "</color>    <color=#B0B1B7>" + text3 + "</color>";
02290: 	}
02291: 
02292: 	private static string Money(int value)
02293: 	{
02294: 		return value.ToString("N0") + " ¢";
02295: 	}
02296: 
02297: 	private static string CashCaption()
02298: 	{
02299: 		if (!_phoneCollection)
02300: 		{
02301: 			return Money(PlayerStore.Instance.GetCash());
02302: 		}
02303: 		return AuctionText.Get("cash", "现金  {0}", PlayerStore.Instance.GetCash());
02304: 	}
02305: 
02306: 	private static Rect FromTop(float x, float top, float width, float height)
02307: 	{
02308: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
02309: 		return new Rect(x, 540f - top - height, width, height);
02310: 	}
02311: 
02312: 	private static void BuildAuctionHeader()
02313: 	{
02314: 		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
02315: 		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
02316: 		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
02317: 		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
02318: 		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
02319: 		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
02320: 		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
02321: 		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
02322: 		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
02323: 		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
02324: 		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
02325: 		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
02326: 		GameObject obj = NewObject("BackgroundShade", _root.transform, typeof(Image), typeof(LayoutElement));
02327: 		obj.GetComponent<LayoutElement>().ignoreLayout = true;
02328: 		Stretch(obj.GetComponent<RectTransform>());
02329: 		((Graphic)obj.GetComponent<Image>()).color = new Color(0.015f, 0.02f, 0.035f, 0.7f);
02330: 		((Graphic)obj.GetComponent<Image>()).raycastTarget = false;
02331: 		obj.transform.SetSiblingIndex(1);
02332: 		HeaderLabel("Brand", "NIGHT CONTAINER AUCTION", 9f, 24f, 14f, 450f, 17f, AuctionUiStyle.Gold, (FontStyles)1);
02333: 		_title = HeaderLabel("Title", T("小窝夜间集装箱拍卖", "Night Container Auction"), 20f, 24f, 32f, 510f, 31f, Color.white, (FontStyles)1);
02334: 		_sessionSummary = HeaderLabel("SessionSummary", "", 10f, 555f, 24f, 175f, 39f, AuctionUiStyle.Muted, (FontStyles)0);
02335: 		HeaderLabel("CashCaption", T("可用现金", "Available cash"), 9f, 744f, 21f, 190f, 17f, AuctionUiStyle.Muted, (FontStyles)0);
02336: 		_cash = HeaderLabel("Cash", CashCaption(), 19f, 744f, 38f, 190f, 28f, Color.white, (FontStyles)1);
02337: 		GameObject obj2 = NewObject("HeaderRule", (Transform)(object)_layout, typeof(Image));
02338: 		SetRect(obj2.GetComponent<RectTransform>(), FromTop(24f, 70f, 912f, 1f));
02339: 		((Graphic)obj2.GetComponent<Image>()).color = AuctionUiStyle.Gold;
02340: 		((Graphic)obj2.GetComponent<Image>()).raycastTarget = false;
02341: 		CreatePanel("MarketStrip", FromTop(24f, 82f, 912f, 27f), SurfaceRaised);
02342: 		_eventSummary = HeaderLabel("NightMarketSummary", "", 10f, 36f, 87f, 888f, 18f, AuctionUiStyle.Gold, (FontStyles)0);
02343: 		((TMP_Text)_eventSummary).enableWordWrapping = false;
02344: 		((TMP_Text)_eventSummary).overflowMode = (TextOverflowModes)1;
02345: 		_status = HeaderLabel("Status", "", 9f, 24f, 114f, 912f, 14f, AuctionUiStyle.Muted, (FontStyles)0);
02346: 		((TMP_Text)_status).enableWordWrapping = false;
02347: 		((TMP_Text)_status).overflowMode = (TextOverflowModes)1;
02348: 		UpdateAuctionHeader();
02349: 	}
02350: 
02351: 	private static TextMeshProUGUI HeaderLabel(string name, string text, float size, float x, float y, float width, float height, Color color, FontStyles style = (FontStyles)0)
02352: 	{
02353: 		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
02354: 		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
02355: 		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
02356: 		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
02357: 		TextMeshProUGUI obj = CreateLabel(name, text, size, new Vector2(x, 0f - y), new Vector2(width, height), style);
02358: 		((Graphic)obj).color = color;
02359: 		return obj;
02360: 	}
02361: 
02362: 	private static void UpdateAuctionHeader()
02363: 	{
02364: 		if (!((Object)(object)_sessionSummary == (Object)null))
02365: 		{
02366: 			((TMP_Text)_sessionSummary).text = T("第 ", "Day ") + AuctionCatalog.Day + T(" 天 · 夜间\n", " · Night\n") + (AuctionState.TryGetVenue(out var venue) ? (AuctionRules.VenueName(venue) + " · " + T("本晚 ", "Tonight ") + Math.Min(ResolvedAuctions + 1, 3) + " / " + 3 + T(" 场", " lots")) : T("选择今晚场次", "Choose tonight's venue"));
02367: 		}
02368: 	}
02369: 
02370: 	private static void RenderLots(string message)
02371: 	{
02372: 		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
~~~

~~~csharp
02390: 			RenderVenues();
02391: 			return;
02392: 		}
02393: 		_page = Page.Lots;
02394: 		ClearDynamic();
02395: 		_stageVisible = false;
02396: 		_nextAnnouncementAt = 0f;
02397: 		SetStatus(message);
02398: 		AddStageLabel("LotsHeading", T("本晚集装箱", "Tonight's Containers"), 15f, 24f, -131f, 480f, 27f, (FontStyles)1);
02399: 		TextMeshProUGUI val = CreateLabel("LotsHint", T("四箱中选择 · 最多完成三场 · 仅公开外观线索", "Choose from four · Up to three auctions · Exterior clues only"), 9f, new Vector2(520f, -139f), new Vector2(416f, 18f), (FontStyles)0);
02400: 		((TMP_Text)val).alignment = (TextAlignmentOptions)260;
02401: 		((Graphic)val).color = AuctionUiStyle.Muted;
02402: 		Dynamic.Add(FindLayoutChild(((TMP_Text)val).transform));
02403: 		for (int i = 0; i < _lots.Count; i++)
02404: 		{
02405: 			AuctionLot lot = _lots[i];
02406: 			int num = 24 + i % 2 * 462;
02407: 			int num2 = 163 + i / 2 * 158;
02408: 			Dynamic.Add(CreatePanel("LotPanel" + i, FromTop(num, num2, 450f, 145f), Surface));
02409: 			Dynamic.Add(CreateAccent("LotEdge" + i, FromTop(num + 1, num2 + 10, 2f, 125f), AuctionCatalog.CategoryColor(lot.Category)));
02410: 			Dynamic.Add(CreateContainerImage("LotCrate" + i, lot.Category, FromTop(num + 14, num2 + 25, 110f, 99f)));
02411: 			AddStageLabel("LotTitle" + i, AuctionCatalog.CategoryName(lot.Category), 16f, num + 142, -(num2 + 12), 175f, 25f, (FontStyles)1);
02412: 			string text = ((lot.PlayerLeading && lot.Finished) ? T("你已得标", "You won") : (lot.Withdrawn ? T("已放弃", "Withdrawn") : (lot.Lost ? (T("得标者 ", "Winner ") + LeaderName(lot)) : ((lot.PendingOffer >= 0) ? T("本轮播报中", "Announcing bids") : (lot.Started ? (T("第 ", "Round ") + (lot.Round + 1) + "/5") : T("等待竞拍", "Awaiting auction"))))));
02413: 			TextMeshProUGUI val2 = CreateLabel("LotState" + i, text, 8f, new Vector2((float)(num + 320), (float)(-(num2 + 19))), new Vector2(112f, 16f), (FontStyles)0);
02414: 			((TMP_Text)val2).alignment = (TextAlignmentOptions)260;
02415: 			((Graphic)val2).color = AuctionUiStyle.Gold;
02416: 			((TMP_Text)val2).enableAutoSizing = true;
02417: 			((TMP_Text)val2).fontSizeMin = 7f;
02418: 			((TMP_Text)val2).fontSizeMax = 8f;
02419: 			Dynamic.Add(FindLayoutChild(((TMP_Text)val2).transform));
02420: 			TextMeshProUGUI val3 = CreateLabel("LotText" + i, AuctionEngine.SurfaceClue(lot) + "\n" + T("风险 ", "Risk ") + Risk(lot) + " · " + MainMix(lot) + "\n" + T("货物与价值保持隐藏", "Cargo and value remain hidden"), 9.5f, new Vector2((float)(num + 142), (float)(-(num2 + 49))), new Vector2(288f, 60f), (FontStyles)0);
02421: 			((Graphic)val3).color = AuctionUiStyle.Muted;
02422: 			Dynamic.Add(FindLayoutChild(((TMP_Text)val3).transform));
02423: 			AuctionLot auctionLot = _lots.FirstOrDefault((AuctionLot other) => other.Started && !other.Finished && !other.Lost);
02424: 			bool flag = !SessionComplete && !lot.Finished && !lot.Lost && (auctionLot == null || auctionLot == lot);
02425: 			Dynamic.Add(CreateButton("Select" + i, (!flag) ? AuctionText.Get("cannot_bid", "不可竞拍") : (lot.Started ? T("继续竞拍", "Resume") : T("查看并竞拍", "View & Bid")), FromTop(num + 142, num2 + 112, 132f, 27f), delegate
02426: 			{
02427: 				SelectLot(lot);
02428: 			}, flag));
02429: 			if (AuctionNightMarket.LeakItems(lot).Length != 0)
02430: 			{
02431: 				Dynamic.Add(CreateButton("Manifest" + i, T("泄露货单", "Leaked Manifest"), FromTop(num + 286, num2 + 112, 146f, 27f), delegate
02432: 				{
02433: 					RenderManifest(lot);
02434: 				}, enabled: true));
02435: 			}
02436: 		}
02437: 		AddLobbyServices();
02438: 	}
02439: 
02440: 	private static void AddLobbyServices()
02441: 	{
02442: 		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
02443: 		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
02444: 		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
02445: 		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
02446: 		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
02447: 		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
02448: 		Dynamic.Add(CreateAccent("FooterRule", FromTop(24f, 499f, 912f, 1f), AuctionUiStyle.Border));
02449: 		Dynamic.Add(CreateButton("Orders", T("采购委托", "Orders"), FromTop(24f, 511f, 100f, 28f), delegate
02450: 		{
02451: 			_cancelOrderPending = false;
02452: 			RenderOrders();
02453: 		}, enabled: true));
02454: 		Dynamic.Add(CreateButton("Collection", T("藏品收集册", "Collection Book"), FromTop(136f, 511f, 100f, 28f), delegate
02455: 		{
02456: 			RenderCollection(_bookCategory);
02457: 		}, enabled: true));
02458: 		Dynamic.Add(CreateButton("History", T("历史账本", "Auction Ledger"), FromTop(248f, 511f, 100f, 28f), RenderHistory, enabled: true));
02459: 		Dynamic.Add(CreateButton("Leave", AuctionText.Get("return_store", "返回店铺"), FromTop(824f, 511f, 112f, 28f), delegate
02460: 		{
02461: 			Close();
02462: 		}, enabled: true));
02463: 		AddStageLabel("FooterHint", T("场次、现金和竞拍进度持续保留", "Venue, cash and auction progress are preserved"), 8.5f, 366f, -519f, 440f, 17f, (FontStyles)0);
02464: 	}
02465: 
02466: 	private static GameObject CreateButton(string name, string text, Rect bounds, Action action, bool enabled)
02467: 	{
02468: 		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
02469: 		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
02470: 		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
02471: 		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
02472: 		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
02473: 		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
02474: 		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
02475: 		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
02476: 		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
02477: 		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
02478: 		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
02479: 		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
02480: 		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
02481: 		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
02482: 		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
02483: 		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
02484: 		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
02485: 		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
02486: 		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
02487: 		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
02488: 		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
02489: 		//IL_0259: Expected O, but got Unknown
02490: 		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
02491: 		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
02492: 		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
02493: 		GameObject val = NewObject(name, (Transform)(object)_layout, typeof(Image), typeof(Button));
02494: 		SetRect(val.GetComponent<RectTransform>(), bounds);
02495: 		bool flag = name.StartsWith("Select") || name.StartsWith("ChooseVenue") || name == "ConfirmVenue" || name == "Bid" || name == "Continue";
02496: 		((Graphic)AuctionUiStyle.RoundedImage(val, flag ? AuctionUiStyle.Gold : AuctionUiStyle.Border)).raycastTarget = true;
02497: 		GameObject obj = NewObject("ButtonOutline", val.transform, typeof(Image));
02498: 		Stretch(obj.GetComponent<RectTransform>());
02499: 		obj.GetComponent<RectTransform>().offsetMin = Vector2.one;
02500: 		obj.GetComponent<RectTransform>().offsetMax = -Vector2.one;
02501: 		AuctionUiStyle.RoundedImage(obj, Frame);
02502: 		GameObject obj2 = NewObject("ButtonFace", val.transform, typeof(Image));
02503: 		Stretch(obj2.GetComponent<RectTransform>());
02504: 		obj2.GetComponent<RectTransform>().offsetMin = Vector2.one * 2f;
02505: 		obj2.GetComponent<RectTransform>().offsetMax = Vector2.one * -2f;
02506: 		Image targetGraphic = AuctionUiStyle.RoundedImage(obj2, flag ? AuctionUiStyle.Gold : SurfaceRaised);
02507: 		Button component = val.GetComponent<Button>();
02508: 		((Selectable)component).targetGraphic = (Graphic)(object)targetGraphic;
02509: 		ColorBlock colors = default(ColorBlock);
02510: 		((ColorBlock)(ref colors)).normalColor = Color.white;
02511: 		((ColorBlock)(ref colors)).highlightedColor = new Color(1.12f, 1.12f, 1.12f);
02512: 		((ColorBlock)(ref colors)).selectedColor = Color.white;
02513: 		((ColorBlock)(ref colors)).pressedColor = new Color(0.75f, 0.75f, 0.75f);
02514: 		((ColorBlock)(ref colors)).disabledColor = new Color(0.4f, 0.4f, 0.43f);
02515: 		((ColorBlock)(ref colors)).colorMultiplier = 1f;
02516: 		((ColorBlock)(ref colors)).fadeDuration = 0.08f;
02517: 		((Selectable)component).colors = colors;
02518: 		((Selectable)component).interactable = enabled;
02519: 		((UnityEvent)component.onClick).AddListener((UnityAction)delegate
02520: 		{
02521: 			Core.Log?.Msg("Auction action: " + name);
02522: 			action();
02523: 		});
02524: 		TextMeshProUGUI obj3 = CreateChildLabel(val.transform, text, name.StartsWith("Digit") ? 16 : ((((Rect)(ref bounds)).height <= 30f) ? 10 : 13), (FontStyles)(flag ? 1 : 0));
02525: 		((TMP_Text)obj3).alignment = (TextAlignmentOptions)514;
02526: 		((Graphic)obj3).color = (Color)((!enabled) ? AuctionUiStyle.Muted : (flag ? Frame : new Color(0.94f, 0.92f, 0.87f)));
02527: 		return val;
02528: 	}
~~~

### XIAOWONightAuction/AuctionPhoneDialoguePatch.cs

~~~csharp
00001: using HarmonyLib;
00002: 
00003: namespace XIAOWONightAuction;
00004: 
00005: [HarmonyPatch(typeof(StorePhoneClient), "GetCallDialog")]
00006: internal static class AuctionPhoneDialoguePatch
00007: {
00008: 	private static bool Prefix(StorePhoneClient __instance, ref Dialogue __result)
00009: 	{
00010: 		if (__instance.dialogFuncId != "xiaowo_night_auction_phone")
00011: 		{
00012: 			return true;
00013: 		}
00014: 		__result = AuctionPhone.BuildDialogue();
00015: 		return false;
00016: 	}
00017: }
~~~

## corp

### CorpPresence/L.cs

~~~csharp
00001: using System;
00002: using UnityEngine;
00003: 
00004: namespace CorpPresence;
00005: 
00006: internal static class L
00007: {
00008: 	private static int _frame = -1;
00009: 
00010: 	private static bool _en;
00011: 
00012: 	private static bool _ready;
00013: 
00014: 	public static bool En
00015: 	{
00016: 		get
00017: 		{
00018: 			if (_frame == Time.frameCount)
00019: 			{
00020: 				return _en;
00021: 			}
00022: 			_frame = Time.frameCount;
00023: 			if (!_ready && (Object)(object)FontLoader.Instance == (Object)null)
00024: 			{
00025: 				return _en = false;
00026: 			}
00027: 			_ready = true;
00028: 			try
00029: 			{
00030: 				string currentLocaleCode = LocHelper.GetCurrentLocaleCode();
00031: 				_en = !string.IsNullOrEmpty(currentLocaleCode) && !currentLocaleCode.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
00032: 			}
00033: 			catch
00034: 			{
00035: 				_en = false;
00036: 			}
00037: 			return _en;
00038: 		}
00039: 	}
00040: 
00041: 	public static string T(string zh, string en)
00042: 	{
00043: 		if (!En)
00044: 		{
00045: 			return zh;
00046: 		}
00047: 		return en;
00048: 	}
00049: 
00050: 	public static string[] T(string[] zh, string[] en)
00051: 	{
00052: 		if (!En)
00053: 		{
00054: 			return zh;
00055: 		}
00056: 		return en;
00057: 	}
00058: }
~~~

### CorpPresence/Services.cs

~~~csharp
00499: 			int num = 0;
00500: 			int num2 = 0;
00501: 			foreach (GameItem item in ((GameInventory)val).childItems.ToList())
00502: 			{
00503: 				string text = Goods.IdOf(item);
00504: 				if (text != null && text.StartsWith("crate_"))
00505: 				{
00506: 					GraphUtils.EmporiumTryAdd((GraphNodeStorage)(object)EmporiumEntry.Instance.invElement, item);
00507: 					if ((object)item.parentInventory == val)
00508: 					{
00509: 						num2++;
00510: 					}
00511: 					else
00512: 					{
00513: 						num++;
00514: 					}
00515: 				}
00516: 			}
00517: 			if (num > 0)
00518: 			{
00519: 				NightLog(L.T($"柜台上的 {num} 只送货箱已收进背包。", $"{num} delivery crate(s) left on the counter were moved into your bag."));
00520: 			}
00521: 			if (num2 > 0)
00522: 			{
00523: 				NightLog(L.T($"背包放不下，柜台上还有 {num2} 只送货箱，过夜会被清掉。", $"Your bag is full: {num2} delivery crate(s) remain on the counter and will be cleared overnight."), "9c3738");
00524: 			}
00525: 		}
00526: 		catch (Exception error)
00527: 		{
00528: 			WarnNightService("收起送货箱", error);
00529: 		}
00530: 	}
00531: 
00532: 	private static void Nightly(Company c, CompanyState s, int fee, string kind, int day)
00533: 	{
00534: 		string text = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.T("街坊互助", "Mutual Aid") : L.T("下层客流", "Lower Foot Traffic")) : L.T("贵宾引荐", "VIP Referral"));
00535: 		Visitor item = new Visitor
00536: 		{
00537: 			Day = day + 1,
00538: 			Kind = kind
00539: 		};
00540: 		string text2 = Pay(c, fee, kind);
00541: 		if (text2 != null)
00542: 		{
00543: 			if (kind == "traffic")
00544: 			{
00545: 				s.TrafficOn = false;
00546: 			}
00547: 			else
00548: 			{
00549: 				s.VipOn = false;
00550: 			}
00551: 			NightLog(L.T(c.Name + "的" + text + "未结算，已暂停：" + text2, c.Name + " " + text + " paused: " + text2), "9c3738");
00552: 			try
00553: 			{
00554: 				ModLog.Info(c.Name + "的" + text + "已暂停：" + text2);
00555: 				return;
00556: 			}
00557: 			catch
00558: 			{
00559: 				return;
00560: 			}
00561: 		}
00562: 		s.Visitors.Add(item);
00563: 		string text3 = ((!(kind == "vip")) ? ((c is SpacerCo) ? L.T("明天会多来几位星际客", "More Spacers will come by tomorrow") : L.T("明天会多来几位下层区顾客", "More Lower Levels customers will come by tomorrow")) : L.T("明天会有一位上层区贵宾光顾店铺", "An Upper Levels VIP will visit tomorrow"));
00564: 		NightLog(L.T($"{c.Name}{text}已付 {fee} 信用点，{text3}。", $"{c.Name} {text}: paid {fee} credits. {text3}."));
00565: 		try
00566: 		{
00567: 			ModLog.Info($"{c.Name}{text}：扣 {fee}₵，第 {day + 1} 天生效");
00568: 		}
00569: 		catch
00570: 		{
00571: 		}
00572: 	}
00573: 
00574: 	private static void Rebate(Company c, CompanyState s, int tomorrow)
00575: 	{
00576: 		if (s.DealerJoined && tomorrow > s.DealerSinceDay && (tomorrow - s.DealerSinceDay) % PastelCo.RebatePeriod == 0)
00577: 		{
00578: 			double num = Math.Round(s.WeekSales * 0.1);
00579: 			if (double.IsNaN(num) || double.IsInfinity(num) || num > 2147483647.0 || num < -2147483648.0)
00580: 			{
00581: 				throw new InvalidOperationException("返点金额无效，保留原销售额");
00582: 			}
00583: 			int num2 = (int)num;
00584: 			if (num2 > 0)
00585: 			{
00586: 				s.Orders.Add(new Order
00587: 				{
00588: 					Day = tomorrow,
00589: 					Kind = "rebate",
00590: 					Cash = num2
00591: 				});
00592: 				s.WeekSales = 0.0;
00593: 				NightLog(L.T($"{c.Name}返点 {num2} 信用点，送货员明天带来。", $"{c.Name} rebate: {num2} credits. The courier brings it tomorrow."), "5c6f46");
00594: 			}
00595: 			else
00596: 			{
00597: 				s.WeekSales = 0.0;
00598: 			}
00599: 			try
00600: 			{
00601: 				ModLog.Info($"{c.Name}本周返点 {num2}₵");
00602: 			}
00603: 			catch
~~~

### CorpPresence/UI/Lay.cs

~~~csharp
00001: using UnityEngine;
00002: 
00003: namespace CorpPresence.UI;
00004: 
00005: internal sealed class Lay
00006: {
00007: 	public const float Gap = 6f;
00008: 
00009: 	public const float LineH = 22f;
00010: 
00011: 	private readonly float _x0;
00012: 
00013: 	private float _rowH;
00014: 
00015: 	public float X { get; private set; }
00016: 
00017: 	public float Y { get; private set; }
00018: 
00019: 	public float Width { get; }
00020: 
00021: 	public float Right => _x0 + Width;
00022: 
00023: 	public float Remaining => Right - X;
00024: 
00025: 	public Lay(float x, float y, float width)
00026: 	{
00027: 		_x0 = (X = x);
00028: 		Y = y;
00029: 		Width = width;
00030: 	}
00031: 
00032: 	public Rect Take(float w, float h = 22f)
00033: 	{
00034: 		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
00035: 		Rect result = new Rect(X, Y, w, h);
00036: 		X += w + 6f;
00037: 		if (h > _rowH)
00038: 		{
00039: 			_rowH = h;
00040: 		}
00041: 		return result;
00042: 	}
00043: 
00044: 	public Rect Fill(float h = 22f)
00045: 	{
00046: 		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
00047: 		return Take(Mathf.Max(10f, Remaining), h);
00048: 	}
00049: 
00050: 	public void Space(float w)
00051: 	{
00052: 		X += w;
00053: 	}
00054: 
00055: 	public void NewLine(float extra = 0f)
00056: 	{
00057: 		Y += Mathf.Max(_rowH, 22f) + extra;
00058: 		X = _x0;
00059: 		_rowH = 0f;
00060: 	}
00061: 
00062: 	public void Down(float h)
00063: 	{
00064: 		if (X > _x0)
00065: 		{
00066: 			NewLine();
00067: 		}
00068: 		Y += h;
00069: 	}
00070: 
00071: 	public void Label(string text, GUIStyle style, float w, Color? color = null, float h = 22f)
00072: 	{
00073: 		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
00074: 		Ui.Label(Take(w, h), text, style, color);
~~~

### CorpPresence/UI/MenuLabels.cs

~~~csharp
00001: using System;
00002: 
00003: namespace CorpPresence.UI;
00004: 
00005: internal static class MenuLabels
00006: {
00007: 	private static readonly string[][] Chinese = new string[12][]
00008: 	{
00009: 		new string[5] { "正品订货", "体力增强剂申领", "赝品换正品", "标本", "品牌目录" },
00010: 		new string[4] { "订货", "利维坦净水器", "罐装设备", "品牌目录" },
00011: 		new string[5] { "制式订货", "高端配件", "返厂翻新", "锻造", "品牌目录" },
00012: 		new string[4] { "邮购订货", "下层合作商", "下层客流", "品牌目录" },
00013: 		new string[3] { "今天进货吗？", "又是我！", "货多，随便挑。" },
00014: 		new string[3] { "军械订货", "地下销赃网络", "任务" },
00015: 		new string[2] { "军械订货", "任务" },
00016: 		new string[1] { "和解" },
00017: 		new string[1] { "营救奥丁" },
00018: 		new string[4] { "贵宾引荐", "实验室采购单", "会员礼遇", "品牌目录" },
00019: 		new string[3] { "码头补给", "街坊互助", "品牌目录" },
00020: 		new string[6] { "开源军械", "地下药房", "码头补给", "街坊互助", "合作阵线", "品牌目录" }
00021: 	};
00022: 
00023: 	private static readonly string[][] English = new string[12][]
00024: 	{
00025: 		new string[5] { "Genuine Orders", "Stamina Booster Request", "Fake-for-Genuine Swap", "Specimens", "Brand Catalog" },
00026: 		new string[4] { "Orders", "Leviathan Purifier", "Canning Equipment", "Brand Catalog" },
00027: 		new string[5] { "Standard Issue Orders", "High-End Attachments", "Factory Refurbishment", "Forging", "Brand Catalog" },
00028: 		new string[4] { "Mail Order", "Lower Dealer", "Lower Foot Traffic", "Brand Catalog" },
00029: 		new string[3] { "Stocking up today?", "Me again!", "Plenty of goods. Take your pick." },
00030: 		new string[3] { "Arms Orders", "Fence Network", "Tasks" },
00031: 		new string[2] { "Arms Orders", "Tasks" },
00032: 		new string[1] { "Make Peace" },
00033: 		new string[1] { "Rescue Odin" },
00034: 		new string[4] { "VIP Referral", "Lab Purchase Order", "Member Perks", "Brand Catalog" },
00035: 		new string[3] { "Dock Supplies", "Mutual Aid", "Brand Catalog" },
00036: 		new string[6] { "Open-Source Arms", "Back-Alley Pharmacy", "Dock Supplies", "Mutual Aid", "Joint Front", "Brand Catalog" }
00037: 	};
00038: 
00039: 	private static string[] _lastBase;
00040: 
00041: 	private static string _lastExtra;
00042: 
00043: 	private static string[] _lastCombined;
00044: 
00045: 	public static string[] Get(int index, bool english)
00046: 	{
00047: 		return (english ? English : Chinese)[index];
00048: 	}
00049: 
00050: 	public static string[] Append(string[] basis, string extra)
00051: 	{
00052: 		if (extra == null)
00053: 		{
00054: 			return basis;
00055: 		}
00056: 		if (basis == _lastBase && extra == _lastExtra)
00057: 		{
00058: 			return _lastCombined;
00059: 		}
00060: 		string[] array = new string[basis.Length + 1];
00061: 		Array.Copy(basis, array, basis.Length);
00062: 		array[basis.Length] = extra;
00063: 		_lastBase = basis;
00064: 		_lastExtra = extra;
00065: 		return _lastCombined = array;
00066: 	}
00067: }
~~~

### CorpPresence/UI/CompanyWindow.cs

~~~csharp
02376: 			if (value > 0)
02377: 			{
02378: 				Cart[catalogEntry2] = value;
02379: 			}
02380: 			else
02381: 			{
02382: 				Cart.Remove(catalogEntry2);
02383: 			}
02384: 		}
02385: 	}
02386: 
02387: 	private static void PastelBribeCell(Rect cell)
02388: 	{
02389: 		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
02390: 		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
02391: 		Theme theme = _theme;
02392: 		GUI.Label(new Rect(((Rect)(ref cell)).x + 6f, ((Rect)(ref cell)).y + 34f, ((Rect)(ref cell)).width - 12f, 22f), L.T("打点主管", "Pay off the supervisor"), theme.Center);
02393: 		bool enabled = GUI.enabled;
02394: 		GUI.enabled = Cash >= PastelCo.Bribe;
02395: 		if (Ui.Button(new Rect(((Rect)(ref cell)).x + 8f, ((Rect)(ref cell)).yMax - 30f, ((Rect)(ref cell)).width - 16f, 26f), L.En ? $"Pay {PastelCo.Bribe}₵" : $"补交 {PastelCo.Bribe}₵", theme.BtnHotStyle))
02396: 		{
02397: 			Toast(PastelCo.PayBribe(deliver: true) ? L.T("分装机明天送到。", "The packaging machine arrives tomorrow.") : L.T("现金不够", "Not enough cash"));
02398: 		}
02399: 		GUI.enabled = enabled;
02400: 	}
02401: 
02402: 	private static void DrawPastelDealer(Rect r)
02403: 	{
02404: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
02405: 		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
02406: 		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
02407: 		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
02408: 		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
02409: 		Theme theme = _theme;
02410: 		CompanyState state = State;
02411: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层合作商", "Lower Dealer"), theme.Title);
02412: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 120f), L.T("成为帕斯泰尔的特约经销点。\n\n", "Become a pastel partner outlet.\n\n") + (L.En ? $"· Regular rebate: every {PastelCo.RebatePeriod} days, {0.1:P0} of what you sold in pastel products, paid back by the courier.\n" : $"· 定期返点：每 {PastelCo.RebatePeriod} 天，把这段时间你卖出的帕斯泰尔产品按售价的 {0.1:P0} 返现，由送货员带来。\n") + L.T("· 解锁“下层客流”：帕斯泰尔会把下层区的熟客介绍到你店里。", "· Unlocks “Lower Foot Traffic”: pastel sends Lower Levels regulars to your shop."), theme.Wrap);
02413: 		if (!state.DealerJoined)
02414: 		{
02415: 			if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, 220f, 34f), L.En ? $"Join (one-time {PastelCo.DealerFee}₵)" : $"加盟（一次性 {PastelCo.DealerFee}₵）", theme.BtnHotStyle))
02416: 			{
02417: 				Toast(Services.JoinDealer(_company) ?? L.T("欢迎加入帕斯泰尔！", "Welcome to pastel!"));
02418: 			}
02419: 		}
02420: 		else
02421: 		{
02422: 			int num = Day - state.DealerSinceDay;
02423: 			int num2 = PastelCo.RebatePeriod - num % PastelCo.RebatePeriod;
02424: 			GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, ((Rect)(ref r)).width - 20f, 24f), L.En ? $"Dealer for {num} days. Sold {state.WeekSales:0}₵ of pastel products this period." : $"已加盟 {num} 天。本期已卖出 {state.WeekSales:0}₵ 的帕斯泰尔产品。", theme.Body);
02425: 			GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 208f, ((Rect)(ref r)).width - 20f, 24f), L.En ? $"Expected rebate {state.WeekSales * 0.1:0}₵, settles in {num2} days." : $"预计返点 {state.WeekSales * 0.1:0}₵，{num2} 天后结算。", theme.Body);
02426: 		}
02427: 	}
02428: 
02429: 	private static void DrawPastelTraffic(Rect r)
02430: 	{
02431: 		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
02432: 		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
02433: 		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
02434: 		Theme theme = _theme;
02435: 		CompanyState state = State;
02436: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层客流", "Lower Foot Traffic"), theme.Title);
02437: 		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 100f), (L.En ? $"{PastelCo.TrafficFee}₵ per night. pastel sends Lower Levels regulars over: the next day, 1–2 extra Lower Levels customers come looking for food and daily goods.\n\nPauses automatically when you are short on cash." : $"每晚 {PastelCo.TrafficFee}₵。帕斯泰尔把下层区的熟客介绍过来：第二天会额外来 1–2 位想买食物、日用品的下层区顾客。\n\n现金不够时自动暂停。") + ((Cash >= PastelCo.TrafficFee) ? "" : (L.En ? $"\n(You have {Cash}₵, not enough for tonight's fee.)" : $"\n（你现在的现金 {Cash}₵，不够今晚的费用。）")), theme.Wrap);
02438: 		if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 160f, 180f, 34f), state.TrafficOn ? L.T("已开启（点击关闭）", "On (click to turn off)") : L.T("开启", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
02439: 		{
02440: 			state.TrafficOn = !state.TrafficOn;
02441: 			ModLog.Info("下层客流：" + (state.TrafficOn ? L.T("开启", "Turn on") : "关闭"));
02442: 			Toast(state.TrafficOn ? L.T("从今晚开始介绍熟客", "Regulars start coming from tonight") : L.T("已关闭", "Turned off"));
02443: 		}
02444: 	}
02445: 
02446: 	private static void DrawPastelFooter(Rect r)
02447: 	{
02448: 		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
02449: 		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
02450: 		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
02451: 		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
02452: 		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
02453: 		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
02454: 		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
02455: 		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
02456: 		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
02457: 		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
02458: 		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
02459: 		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
02460: 		Theme theme = _theme;
02461: 		CompanyState state = State;
02462: 		GUI.Box(r, GUIContent.none, theme.CellBox);
02463: 		Texture2D val = Portraits.Texture(PastelCo.HemingwayOpen() ? "pastel_milo_hemingway" : "pastel_milo", "maleLower_0");
02464: 		Rect r2 = default(Rect);
02465: 		((Rect)(ref r2))._002Ector(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 6f, 130f, 95f);
02466: 		Ui.Fill(r2, Tex.Hex(13223594));
02467: 		if ((Object)(object)val != (Object)null)
02468: 		{
02469: 			Ui.Image(r2, val);
02470: 		}
02471: 		GUI.Label(new Rect(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 104f, 130f, 22f), L.T("米洛", "Milo"), theme.Center);
02472: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 10f, 300f, 22f), "“" + MiloLines[Day % MiloLines.Length] + "”", theme.Body);
02473: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 40f, 300f, 22f), L.En ? $"Cooperation {state.Coop}  ·  {TierLabel(state)}" : $"合作度 {state.Coop}  ·  {TierLabel(state)}", theme.Body);
02474: 		CoopBar(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 66f, 300f, 12f));
02475: 		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 82f, 300f, 20f), NextTierHint(), theme.Small);
02476: 		if (_module != 0 || !PastelUnlocked(0, out var _))
02477: 		{
02478: 			return;
02479: 		}
02480: 		int num = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => PastelUnitPrice(kv.Key) * kv.Value);
02481: 		GUI.Label(new Rect(((Rect)(ref r)).xMax - 180f, ((Rect)(ref r)).y + 12f, 170f, 22f), L.En ? $"Total {num}₵" : $"合计 {num}₵", theme.Price);
02482: 		GUI.Label(new Rect(((Rect)(ref r)).xMax - 180f, ((Rect)(ref r)).y + 36f, 170f, 22f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
02483: 		bool enabled = num > 0 && num <= Cash && Services.CanOrderToday(_company);
02484: 		bool enabled2 = GUI.enabled;
02485: 		GUI.enabled = enabled;
02486: 		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, ((Rect)(ref r)).y + 64f, 150f, 36f), L.T("下单", "Order"), theme.BtnHotStyle))
02487: 		{
02488: 			string text = Services.PlaceOrder(_company, Cart, PastelUnitPrice);
02489: 			if (text == null)
02490: 			{
02491: 				Cart.Clear();
02492: 				Toast(L.T("已下单！", "Ordered! ") + _company.FrontDeskDone);
02493: 			}
02494: 			else
02495: 			{
02496: 				Toast(text);
02497: 			}
02498: 		}
02499: 		GUI.enabled = enabled2;
02500: 	}
02501: 
02502: 	private static int SolanaNeed(int i)
02503: 	{
02504: 		if (i >= SolanaModuleTier.Length)
02505: 		{
~~~
