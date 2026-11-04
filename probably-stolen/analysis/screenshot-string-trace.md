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

### 跟在队伍后面的黑鸟
- not found in decompiled C#

### 等待
- \`GameCardPK/PerSaveState.cs:97\`
~~~csharp
0094: 		}
0095: 		catch (Exception ex)
0096: 		{
0097: 			Core.Log?.Error("立即保存 MOD 状态失败，将等待游戏下次正常保存：" + ex.Message);
0098: 		}
0099: 	}
0100: }
~~~
- \`GameCardPK/UI/BattlePanel.cs:1711\`
~~~csharp
1708: 	{
1709: 		return phase switch
1710: 		{
1711: 			BattlePhase.PlayerInput => "等待你的行动", 
1712: 			BattlePhase.Resolving => "结算中…", 
1713: 			BattlePhase.EnemyTurn => "敌人行动中…", 
1714: 			BattlePhase.BattleEnd => "战斗结束", 
~~~

### 花钱
- \`GameCardPK/UI/BattlePanel.cs:1838\`
~~~csharp
1835: 					break;
1836: 				case BattleAssistant.PlayerCardShelveResult.Shelved:
1837: 					session.Ctx.Log("战斗结束：" + text4 + "，棋盘上的「" + text3 + "」已被搬上前台柜台。");
1838: 					PanelHost.Message = "战斗失败：棋盘上的卡牌「" + text3 + "」已上柜，要花钱买回来。";
1839: 					break;
1840: 				case BattleAssistant.PlayerCardShelveResult.Destroyed:
1841: 					session.Ctx.Log("战斗结束：" + text4 + "，前台柜台放不下，棋盘上的「" + text3 + "」已销毁（技能条一起没了）。");
~~~

### 付钱
- not found in decompiled C#

### 战斗
- \`GameCardPK/CardDurabilityChecks.cs:12\`
~~~csharp
0009: 	{
0010: 		if (!(ctx.Payload is BattleSession))
0011: 		{
0012: 			Core.Debug("[卡牌PK] 芯片耐久：本次触发没有战斗会话，不扣耐久。");
0013: 		}
0014: 		else
0015: 		{
~~~
- \`GameCardPK/JsonAssets.cs:305\`
~~~csharp
0302: 		}
0303: 		catch (Exception ex26)
0304: 		{
0305: 			Core.Log?.Error("加载战斗奖励配置失败：" + ex26);
0306: 		}
0307: 		try
0308: 		{
~~~
- \`GameCardPK/FlashCardSystem.cs:108\`
~~~csharp
0105: 		}
0106: 		catch (Exception ex)
0107: 		{
0108: 			Core.Debug("[闪卡] 战斗开始时应用闪卡失败（已忽略）：" + ex.Message);
0109: 		}
0110: 	}
0111: 
~~~
- \`GameCardPK/CardDurabilitySystem.cs:145\`
~~~csharp
0142: 		}
0143: 		catch (Exception ex)
0144: 		{
0145: 			Core.Log?.Error("[卡牌PK] 战斗扣芯片耐久失败（已忽略）：" + ex.Message);
0146: 		}
0147: 	}
0148: 
~~~
- \`GameCardPK/SkillText.cs:201\`
~~~csharp
0198: 		}
0199: 		catch (Exception ex)
0200: 		{
0201: 			Core.Debug("[技能文案] 战斗内解析失败（退回原文）：" + ex.Message);
0202: 			return text;
0203: 		}
0204: 	}
~~~
- \`GameCardPK/SkillText.cs:1059\`
~~~csharp
1056: 	{
1057: 		return point switch
1058: 		{
1059: 			SkillStartPoint.BattleStart => "战斗开始时", 
1060: 			SkillStartPoint.RoundStart => "每轮开始", 
1061: 			SkillStartPoint.TurnStart => "自己回合开始", 
1062: 			SkillStartPoint.BeforeAttack => "出手前", 
~~~
- \`GameCardPK/SkillText.cs:1076\`
~~~csharp
1073: 			SkillStartPoint.OnKill => "击杀时", 
1074: 			SkillStartPoint.TurnEnd => "自己回合结束", 
1075: 			SkillStartPoint.RoundEnd => "每轮结束", 
1076: 			SkillStartPoint.BattleEnd => "战斗结束时", 
1077: 			SkillStartPoint.OnArmorBreak => "击碎护甲时", 
1078: 			_ => "触发时", 
1079: 		};
~~~
- \`GameCardPK/Core.cs:28\`
~~~csharp
0025: 		{
0026: 			ModConfig config = ModHelper.GetConfig(manifest.ID);
0027: 			debugEntry = config.Define("debug_logging", false, "调试", (Func<string>)(() => "输出调试日志"), (Func<string>)(() => "命中判定、降级兜底等过程日志。排查问题时打开。"), false);
0028: 			slowLogEntry = config.Define("slow_battle_log", true, "战斗", (Func<string>)(() => "战斗日志逐条播放"), (Func<string>)(() => "关闭后战斗日志一次性全部显示（等价于战斗中右上角那个开关）。"), false);
0029: 			slowLogEntry.OnChanged += delegate(bool v)
0030: 			{
0031: 				BattleLogPlayback.StepMode = v;
~~~
- \`GameCardPK/BattleAssistant.cs:35\`
~~~csharp
0032: 		try
0033: 		{
0034: 			enemyCard.DisableTag(TagDisplay.NewTagDatas["CanBattle"].Id, false);
0035: 			Core.Debug("[卡牌PK] 战胜：已摘掉「可战斗」标记：" + SafeName(enemyCard));
0036: 		}
0037: 		catch (Exception ex)
0038: 		{
~~~
- \`GameCardPK/BattleAssistant.cs:39\`
~~~csharp
0036: 		}
0037: 		catch (Exception ex)
0038: 		{
0039: 			Core.Debug("[卡牌PK] 战胜摘「可战斗」标记失败（已忽略）：" + ex.Message);
0040: 		}
0041: 		return ReturnEnemyCardToCounter();
0042: 	}
~~~
- \`GameCardPK/BattleAssistant.cs:347\`
~~~csharp
0344: 			BattleCardInstance enemy = ToBattleCardInstance(enemyItem, BattleSide.Enemy);
0345: 			if (player == null || enemy == null)
0346: 			{
0347: 				Core.Log?.Error("[卡牌PK] 卡牌数据读取失败，战斗没有开始。");
0348: 				return null;
0349: 			}
0350: 			BattleContext ctx = new BattleContext(BattleDatabase.GetRules());
~~~
- \`GameCardPK/BattleAssistant.cs:377\`
~~~csharp
0374: 		}
0375: 		catch (Exception ex)
0376: 		{
0377: 			Core.Log?.Error("[卡牌PK] 开始战斗失败：" + ex);
0378: 			return null;
0379: 		}
0380: 	}
~~~

### 回合
- \`GameCardPK/SkillText.cs:783\`
~~~csharp
0780: 		{
0781: 			return "永久";
0782: 		}
0783: 		return ((buff != null && buff.RemainTurns > 0) ? buff.RemainTurns : data.Duration) + " 回合";
0784: 	}
0785: 
0786: 	private static void ScanBuffActions(BuffData data, Dictionary<string, string> map)
~~~
- \`GameCardPK/SkillText.cs:1061\`
~~~csharp
1058: 		{
1059: 			SkillStartPoint.BattleStart => "战斗开始时", 
1060: 			SkillStartPoint.RoundStart => "每轮开始", 
1061: 			SkillStartPoint.TurnStart => "自己回合开始", 
1062: 			SkillStartPoint.BeforeAttack => "出手前", 
1063: 			SkillStartPoint.AfterAttack => "出手后", 
1064: 			SkillStartPoint.BeforeDamage => "受到伤害前", 
~~~
- \`GameCardPK/SkillText.cs:1074\`
~~~csharp
1071: 			SkillStartPoint.OnDeath => "阵亡时", 
1072: 			SkillStartPoint.OnNearDeath => "濒死时（保活）", 
1073: 			SkillStartPoint.OnKill => "击杀时", 
1074: 			SkillStartPoint.TurnEnd => "自己回合结束", 
1075: 			SkillStartPoint.RoundEnd => "每轮结束", 
1076: 			SkillStartPoint.BattleEnd => "战斗结束时", 
1077: 			SkillStartPoint.OnArmorBreak => "击碎护甲时", 
~~~
- \`GameCardPK/SkillText.cs:1121\`
~~~csharp
1118: 		case ConditionType.IsAlive:
1119: 			return "目标存活";
1120: 		case ConditionType.TurnIndexCompare:
1121: 			return "第 " + condition.RightValue + " 回合";
1122: 		case ConditionType.LogicAnd:
1123: 			return ConditionText(condition.Children);
1124: 		case ConditionType.LogicOr:
~~~
- \`GameCardPK/BattleSkillInstance.cs:213\`
~~~csharp
0210: 		}
0211: 		if (CooldownRemain > 0)
0212: 		{
0213: 			return "冷却中：" + CooldownRemain + " 回合";
0214: 		}
0215: 		if (Config.MaxUses > 0 && UsedCount >= Config.MaxUses)
0216: 		{
~~~
- \`GameCardPK/BattleSession.cs:170\`
~~~csharp
0167: 			CooldownRemain = 0,
0168: 			RemainUses = -1,
0169: 			Usable = flag,
0170: 			Reason = (flag ? string.Empty : "现在不是你的回合")
0171: 		});
0172: 		if (player != null)
0173: 		{
~~~
- \`GameCardPK/BattleSession.cs:218\`
~~~csharp
0215: 				Ctx.Log("请先选择一个行动。要允许空过请把 battle.json 的 allowPassTurn 设为 true。");
0216: 				return false;
0217: 			}
0218: 			Ctx.Log(current.Name + " 放弃了本回合行动");
0219: 		}
0220: 		_resolveDepth++;
0221: 		try
~~~
- \`GameCardPK/BattleSession.cs:514\`
~~~csharp
0511: 		{
0512: 			TickCooldowns(battleCardInstance);
0513: 		}
0514: 		Ctx.Log("—— 第 " + Ctx.Round + " 轮 · " + ((battleCardInstance.Side == BattleSide.Player) ? "【我方】" : "【敌方】") + battleCardInstance.Name + " 的回合 ——");
0515: 		TurnEvent ev = new TurnEvent
0516: 		{
0517: 			Ctx = Ctx,
~~~
- \`GameCardPK/BattleSession.cs:535\`
~~~csharp
0532: 		}
0533: 		else if (Ctx.Attrs.GetFinalAttr(battleCardInstance, "IMMOBILIZED") > 0)
0534: 		{
0535: 			Ctx.Log("—— " + battleCardInstance.Name + " 被禁锢，本回合无法行动 ——");
0536: 			FinishTurn();
0537: 		}
0538: 		else if (battleCardInstance.Side == BattleSide.Player)
~~~
- \`GameCardPK/UI/BattlePanel.cs:535\`
~~~csharp
0532: 		{
0533: 			UiKit.Box(parent, "Basic.SelBar", x + 6f, y + 8f, 4f, Mathf.Max(6f, h - 16f), UiSkin.Gold);
0534: 		}
0535: 		RegisterHover(x, y, w, h, "普攻\n不消耗，按攻击力结算一次伤害。\n任何回合都能用。");
0536: 	}
0537: 
0538: 	private static void SkillEntry(Transform parent, float x, float y, float w, float h, BattleSkillInstance skill, bool interactive)
~~~
- \`GameCardPK/UI/BattlePanel.cs:660\`
~~~csharp
0657: 				text2 = text2 + "\n触发：" + ((stringBuilder.Length > 0) ? stringBuilder.ToString() : "常驻");
0658: 				if (flag2)
0659: 				{
0660: 					text2 += "（对手的回合也触发）";
0661: 				}
0662: 			}
0663: 			else if (config.PassivePoint != SkillStartPoint.None || config.TriggerOnAnyCard)
~~~
- \`GameCardPK/UI/BattlePanel.cs:669\`
~~~csharp
0666: 				text2 = text2 + "\n触发：" + text5;
0667: 				if (config.TriggerOnAnyCard)
0668: 				{
0669: 					text2 += "（对手的回合也触发）";
0670: 				}
0671: 			}
0672: 		}
~~~

### 对战
- \`GameCardPK/ClubOuting.cs:65\`
~~~csharp
0062: 		catch (Exception ex)
0063: 		{
0064: 			_leaveEarlyHooked = false;
0065: 			Core.Log?.Error("[卡牌PK] 订阅官方离开早钩子失败（离开赛事时对战卡可能不会被销毁）：" + ex);
0066: 		}
0067: 	}
0068: 
~~~
- \`GameCardPK/ChipSystem.cs:96\`
~~~csharp
0093: 			}
0094: 			else if (!ItemOwnership.IsPlayerOwned(card, "卡牌", log: false))
0095: 			{
0096: 				text = "这张卡不归玩家（柜台待售 / 待对战状态）";
0097: 			}
0098: 			else if (HasChip(card))
0099: 			{
~~~
- \`GameCardPK/CardClubSession.cs:158\`
~~~csharp
0155: 		}
0156: 		catch (Exception ex)
0157: 		{
0158: 			Core.Log?.Error("[卡牌PK] 离开赛事时销毁对战卡失败（已忽略）：" + ex);
0159: 		}
0160: 	}
0161: 
~~~
- \`GameCardPK/ChallengeLetter.cs:222\`
~~~csharp
0219: 					}
0220: 					else if (npc.TierIds.Count == 0)
0221: 					{
0222: 						Core.Log?.Error("[战书]「" + challengeItemId + "」难度 " + (i + 1) + " 指向的「" + text + "」没有配 cardStrength ⇒ 他来了也不会带对战卡，玩家打不了。");
0223: 						num2++;
0224: 					}
0225: 				}
~~~
- \`GameCardPK/TagDisplay.cs:19\`
~~~csharp
0016: 	private static Dictionary<string, CustomTagData> BuildTagTable()
0017: 	{
0018: 		Dictionary<string, CustomTagData> dictionary = new Dictionary<string, CustomTagData>();
0019: 		Add(dictionary, "CanBattle", DisplayType.None, "可对战", (ColorPalette)12472652);
0020: 		Add(dictionary, "OutOfPrint", DisplayType.None, "绝版", (ColorPalette)6511905);
0021: 		Add(dictionary, "FirstEdition", DisplayType.None, "初版", (ColorPalette)4365501);
0022: 		Add(dictionary, "Memorial", DisplayType.None, "纪念版", (ColorPalette)11567781);
~~~
- \`GameCardPK/CardStrengthDatabase.cs:161\`
~~~csharp
0158: 		CardStrengthFileDto cardStrengthFileDto = JsonAssets.Load<CardStrengthFileDto>("card_strength.json");
0159: 		if (cardStrengthFileDto == null)
0160: 		{
0161: 			Core.Debug("[卡牌PK] 没读到 card_strength.json：所有 NPC 都不会卖对战卡。");
0162: 			return;
0163: 		}
0164: 		if (cardStrengthFileDto.Strengths == null || cardStrengthFileDto.Strengths.Count == 0)
~~~
- \`GameCardPK/CardNPC.cs:1101\`
~~~csharp
1098: 		}
1099: 		if (num == 0)
1100: 		{
1101: 			Core.Debug("[卡牌PK] 客户 " + npc.Id + " 的对战卡一张都没上成（档位 " + string.Join("/", npc.TierIds) + "，柜台满了？）。");
1102: 			return false;
1103: 		}
1104: 		if (num < npc.TierIds.Count)
~~~
- \`GameCardPK/CardNPC.cs:1106\`
~~~csharp
1103: 		}
1104: 		if (num < npc.TierIds.Count)
1105: 		{
1106: 			Core.Debug("[卡牌PK] 客户 " + npc.Id + " 的对战卡只上成 " + num + "/" + npc.TierIds.Count + " 张。");
1107: 		}
1108: 		return true;
1109: 	}
~~~
- \`GameCardPK/CardNPC.cs:1118\`
~~~csharp
1115: 			GameItem val = EnemyCardGenerator.Generate(tierId);
1116: 			if (val == null)
1117: 			{
1118: 				Core.Debug("[卡牌PK] 客户 " + npc.Id + " 的对战卡没生成出来（档位 " + tierId + "）。");
1119: 				return false;
1120: 			}
1121: 			int num = CardPricing.ReadValue(val);
~~~
- \`GameCardPK/CardNPC.cs:1131\`
~~~csharp
1128: 			}
1129: 			catch (Exception ex)
1130: 			{
1131: 				Core.Debug("[卡牌PK] 对战卡落位回查失败：" + ex.Message);
1132: 			}
1133: 			if (!flag)
1134: 			{
~~~
- \`GameCardPK/CardNPC.cs:1135\`
~~~csharp
1132: 			}
1133: 			if (!flag)
1134: 			{
1135: 				Core.Debug("[卡牌PK] 客户 " + npc.Id + " 的对战卡没上成（柜台满了？）。");
1136: 				return false;
1137: 			}
1138: 			try
~~~
- \`GameCardPK/CardNPC.cs:1144\`
~~~csharp
1141: 			}
1142: 			catch (Exception ex2)
1143: 			{
1144: 				Core.Debug("[卡牌PK] 对战卡归属还原失败：" + ex2.Message);
1145: 			}
1146: 			Core.Debug("[卡牌PK] 客户「" + npc.Name + "」上柜对战卡：" + Core.Clean(val.name) + "（档位 " + tierId + "，基础价值 " + num + "，贴纸 " + num2 + " 张）");
1147: 			return true;
~~~

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

### 淫色深空
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

### 可能是触手
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

### xiaowo_auction_calibrator
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
- \`XIAOWONightAuction/AuctionExclusiveCatalog.cs:74\`
~~~csharp
0071: 			["xiaowo_auction_coilgun"] = Spec("xiaowo_auction_coilgun", ContainerCategory.Contraband, 2, 30000, "静默线圈枪原型", "没有量产记录的低噪声线圈武器原型。", "CONTRABAND_ITEM_TAG", "WEAPON", "FIREARM"),
0072: 			["xiaowo_auction_master_key"] = Spec("xiaowo_auction_master_key", ContainerCategory.Contraband, 3, 65000, "军用权限母钥", "能够伪造多级军用通行凭证的核心密钥。", "CONTRABAND_ITEM_TAG"),
0073: 			["xiaowo_auction_bearing_set"] = Spec("xiaowo_auction_bearing_set", ContainerCategory.Engineering, 1, 1100, "高密度轴承组", "适用于重型设备的高密度精密轴承。", "MATERIAL"),
0074: 			["xiaowo_auction_calibrator"] = Spec("xiaowo_auction_calibrator", ContainerCategory.Engineering, 1, 2500, "工业校准仪", "用于校准精密生产设备的便携仪器。", "TOOL"),
0075: 			["xiaowo_auction_supercoil"] = Spec("xiaowo_auction_supercoil", ContainerCategory.Engineering, 1, 4100, "超导线圈", "封装完好的工业级超导线圈。", "MATERIAL", "MODULE"),
0076: 			["xiaowo_auction_control_core"] = Spec("xiaowo_auction_control_core", ContainerCategory.Engineering, 1, 5700, "原型控制核心", "尚未量产的自动化设备控制核心。", "MODULE", "MACHINE"),
0077: 			["xiaowo_auction_fabricator_core"] = Spec("xiaowo_auction_fabricator_core", ContainerCategory.Engineering, 1, 8500, "自校准制造核心", "能够自行修正加工误差的制造设备核心。", "MODULE"),
~~~
- \`XIAOWONightAuction/AuctionItemLinks.cs:58\`
~~~csharp
0055: 
0056: 	internal static long ValueFor(GameItem item, int fullValue)
0057: 	{
0058: 		if (!(((item != null) ? item.identifier : null) == "xiaowo_auction_calibrator"))
0059: 		{
0060: 			return fullValue;
0061: 		}
~~~
- \`XIAOWONightAuction/AuctionItemLinks.cs:118\`
~~~csharp
0115: 		}
0116: 		EnsureIndependentFeatures(item);
0117: 		SetTag(item, "XIAOWO_AUCTION_NATIVE_LINK_V1", enabled: true);
0118: 		if (item.identifier == "xiaowo_auction_calibrator" && !item.IsTag("XIAOWO_CALIBRATOR_CHARGES"))
0119: 		{
0120: 			SetCharges(item, 3);
0121: 		}
~~~
- \`XIAOWONightAuction/AuctionExclusiveFactory.cs:281\`
~~~csharp
0278: 		{
0279: 			text3 = string.Format(text3, AuctionForgedPass.Remaining(item));
0280: 		}
0281: 		if (spec.Id == "xiaowo_auction_calibrator")
0282: 		{
0283: 			text3 = string.Format(text3, AuctionItemLinks.Remaining(item));
0284: 		}
~~~
- \`XIAOWONightAuction/AuctionItemLinksTargetPatch.cs:10\`
~~~csharp
0007: {
0008: 	private static bool Prefix(GameItem __instance, GameItem targetItem)
0009: 	{
0010: 		if (((__instance != null) ? __instance.identifier : null) == "xiaowo_auction_calibrator")
0011: 		{
0012: 			AuctionItemLinks.Calibrate(__instance, targetItem);
0013: 			return false;
~~~
- \`XIAOWONightAuction/AuctionItemLinksTargetGate.cs:7\`
~~~csharp
0004: {
0005: 	internal static bool Check(GameItem source, GameItem target, ref bool result)
0006: 	{
0007: 		if (((source != null) ? source.identifier : null) == "xiaowo_auction_calibrator")
0008: 		{
0009: 			result = AuctionItemLinks.CalibrationTarget(source, target);
0010: 		}
~~~
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:8\`
~~~csharp
0005: 
0006: internal static class AuctionItemLinkRules
0007: {
0008: 	internal const string Calibrator = "xiaowo_auction_calibrator";
0009: 
0010: 	internal const string Fabricator = "xiaowo_auction_fabricator_core";
0011: 
~~~
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:36\`
~~~csharp
0033: 		case "xiaowo_auction_ceramic_fuse":
0034: 		case "xiaowo_auction_bearing_set":
0035: 			return "common_electronic";
0036: 		case "xiaowo_auction_calibrator":
0037: 			return "common_electronic";
0038: 		case "xiaowo_auction_fabricator_core":
0039: 			return "system_module_quality";
~~~

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

### private static void ApplyLocalizedText
- \`XIAOWONightAuction/AuctionExclusiveFactory.cs:255\`
~~~csharp
0235: 		if (item != null && AuctionExclusiveCatalog.Items.TryGetValue(item.identifier, out var value))
0236: 		{
0237: 			ApplyLocalizedText(item, value);
0238: 		}
0239: 	}
0240: 
0241: 	internal static void RefreshOwnedText()
0242: 	{
0243: 		PlayerStore existing = NativeStore.Existing;
0244: 		List<GameItem> list = ((existing != null) ? existing.FindAllItem(true) : null);
0245: 		if (list == null)
0246: 		{
0247: 			return;
0248: 		}
0249: 		foreach (GameItem item in list)
0250: 		{
0251: 			RefreshLocalizedText(item);
0252: 		}
0253: 	}
0254: 
0255: 	private static void ApplyLocalizedText(GameItem item, AuctionExclusiveSpec spec)
0256: 	{
0257: 		int num;
0258: 		string text;
0259: 		if (AuctionExclusiveCatalog.English.TryGetValue(spec.Id, out (string, string) value))
0260: 		{
0261: 			num = ((!AuctionText.IsChinese(AuctionText.CurrentLocaleCode)) ? 1 : 0);
0262: 			if (num != 0)
0263: 			{
0264: 				(text, _) = value;
0265: 				goto IL_0035;
0266: 			}
0267: 		}
0268: 		else
0269: 		{
0270: 			num = 0;
0271: 		}
0272: 		text = spec.Name;
0273: 		goto IL_0035;
0274: 		IL_0035:
0275: 		string text2 = text;
0276: 		string text3 = ((num != 0) ? value.Item2 : spec.Description);
0277: 		if (spec.Id == "xiaowo_auction_forged_pass")
0278: 		{
0279: 			text3 = string.Format(text3, AuctionForgedPass.Remaining(item));
0280: 		}
0281: 		if (spec.Id == "xiaowo_auction_calibrator")
0282: 		{
0283: 			text3 = string.Format(text3, AuctionItemLinks.Remaining(item));
0284: 		}
0285: 		item.identifierName = text2;
0286: 		item.name = text2;
0287: 		item.shortDescription = text3;
0288: 		item.longDescription = text3;
0289: 		item.flavorText = AuctionText.Select("只在夜间集装箱拍卖中流通的稀有货物。", "Rare goods traded only at the night container auction.");
~~~
- \`XIAOWONightAuction/AuctionContainer.cs:179\`
~~~csharp
0159: 				}
0160: 				goto IL_012d;
0161: 			}
0162: 		}
0163: 		num = 1;
0164: 		goto IL_0102;
0165: 		IL_012d:
0166: 		ClearNativeLockState(item);
0167: 		item.SetContentWindow((PixelWindow)null);
0168: 		if (num != 0)
0169: 		{
0170: 			item.ValidateShapeState();
0171: 		}
0172: 		return;
0173: 		IL_0102:
0174: 		item.shape = new GridShapeBuilder(2, 2).SetDataFill(2, 2, (byte)1).Build();
0175: 		item.modifiedShape = item.shape.Clone();
0176: 		goto IL_012d;
0177: 	}
0178: 
0179: 	private static void ApplyLocalizedText(GameItem item, ContainerData data)
0180: 	{
0181: 		string name = (item.identifierName = AuctionCatalog.CategoryName(data.Category));
0182: 		item.name = name;
0183: 		item.shortDescription = ((data.Index >= data.Contents.Count) ? AuctionText.Get("container_empty", "这个拍卖集装箱已经取空。") : AuctionText.Get("container_remaining", "密封集装箱，剩余 {0} 件。只能取出，不能存入。", data.Contents.Count - data.Index));
0184: 		item.longDescription = item.shortDescription;
0185: 		item.flavorText = AuctionText.Get("container_flavor", "箱门只认拍卖行内部封签。");
0186: 	}
0187: 
0188: 	internal static void RepairForOpenIfNeeded(GameItem item)
0189: 	{
0190: 		if (item != null)
0191: 		{
0192: 			GridShape shape = item.shape;
0193: 			if (((shape != null) ? new int?(shape.width) : ((int?)null)) == 2 && item.shape.height == 2)
0194: 			{
0195: 				GridShape modifiedShape = item.modifiedShape;
0196: 				if (modifiedShape != null && modifiedShape.width == 2 && item.modifiedShape.height == 2 && !item.IsTag("SUPPLY_CRATE") && !item.IsTag("LOCKED_CONTAINER_TAG") && item.contentWindow == null && !(item.spriteAtlasPath != "xiaowo_night_auction"))
0197: 				{
0198: 					return;
0199: 				}
0200: 			}
0201: 		}
0202: 		Repair(item);
0203: 	}
0204: 
0205: 	private static void ClearNativeLockState(GameItem item)
0206: 	{
0207: 		string[] nativeLockTags = NativeLockTags;
0208: 		foreach (string text in nativeLockTags)
0209: 		{
0210: 			item.DisableTag(text, true);
0211: 			TagSystem state = item.state;
0212: 			if (state != null)
0213: 			{
~~~

### Drag onto an owned stuck module
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:117\`
~~~csharp
0114: 		{
0115: 			Describe(array[i], "拖到受损模组或模组提取器上维修，沿用原版电子零件的条件、成功率和消耗。不用于其他设备升级。", "Drag onto a damaged module or module extractor to repair it with native electronic-part conditions, chances and consumption. Does not upgrade other equipment.");
0116: 		}
0117: 		Describe("calibrator", "拖到已卡住的自有模组上，解除卡住状态。剩余次数：{0}/3。每次成功消耗1次，价值按剩余次数降低，耗尽后销毁。", "Drag onto an owned stuck module to release it. Uses remaining: {0}/3. Each successful release uses one charge; value scales with remaining charges. Consumed after the final use.");
0118: 		Describe("fabricator_core", "可安装在原版兼容的模组槽中：品质加成15%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +15% quality. Native slot restrictions, wear and removal rules apply.");
0119: 		Describe("quantum_controller", "可安装在原版兼容的模组槽中：效率加成10%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +10% efficiency. Native slot restrictions, wear and removal rules apply.");
0120: 		array = new string[3] { "colony_painting", "gilt_music_box", "platinum_trophy" };
~~~

### Uses remaining:
- \`XIAOWONightAuction/AuctionExclusiveCatalog.cs:35\`
~~~csharp
0032: 			["xiaowo_auction_century_vintage"] = ("Century Vintage", "A sealed bottle from an old cellar with its label intact."),
0033: 			["xiaowo_auction_orbital_watch"] = ("Orbital Astronomical Watch", "A limited mechanical watch made in an orbital workshop."),
0034: 			["xiaowo_auction_black_diamond"] = ("Stateless Black Diamond", "A giant black diamond with no registered origin."),
0035: 			["xiaowo_auction_forged_pass"] = ("Forged Access Chip", "Opens native keycard-locked crates. Uses remaining: {0}/10. Value scales with remaining uses; consumed after the final use."),
0036: 			["xiaowo_auction_silent_bolt"] = ("Silent Weapon Bolt", "A weapon bolt illegally modified for quieter operation."),
0037: 			["xiaowo_auction_cipher_ledger"] = ("Encrypted Black Ledger", "An encrypted ledger of underground trade routes."),
0038: 			["xiaowo_auction_black_decoder"] = ("Black-Market Decoder", "An illegal decoder that bypasses older electronic locks."),
~~~
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:117\`
~~~csharp
0114: 		{
0115: 			Describe(array[i], "拖到受损模组或模组提取器上维修，沿用原版电子零件的条件、成功率和消耗。不用于其他设备升级。", "Drag onto a damaged module or module extractor to repair it with native electronic-part conditions, chances and consumption. Does not upgrade other equipment.");
0116: 		}
0117: 		Describe("calibrator", "拖到已卡住的自有模组上，解除卡住状态。剩余次数：{0}/3。每次成功消耗1次，价值按剩余次数降低，耗尽后销毁。", "Drag onto an owned stuck module to release it. Uses remaining: {0}/3. Each successful release uses one charge; value scales with remaining charges. Consumed after the final use.");
0118: 		Describe("fabricator_core", "可安装在原版兼容的模组槽中：品质加成15%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +15% quality. Native slot restrictions, wear and removal rules apply.");
0119: 		Describe("quantum_controller", "可安装在原版兼容的模组槽中：效率加成10%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +10% efficiency. Native slot restrictions, wear and removal rules apply.");
0120: 		array = new string[3] { "colony_painting", "gilt_music_box", "platinum_trophy" };
~~~

### Each successful release uses one charge
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:117\`
~~~csharp
0114: 		{
0115: 			Describe(array[i], "拖到受损模组或模组提取器上维修，沿用原版电子零件的条件、成功率和消耗。不用于其他设备升级。", "Drag onto a damaged module or module extractor to repair it with native electronic-part conditions, chances and consumption. Does not upgrade other equipment.");
0116: 		}
0117: 		Describe("calibrator", "拖到已卡住的自有模组上，解除卡住状态。剩余次数：{0}/3。每次成功消耗1次，价值按剩余次数降低，耗尽后销毁。", "Drag onto an owned stuck module to release it. Uses remaining: {0}/3. Each successful release uses one charge; value scales with remaining charges. Consumed after the final use.");
0118: 		Describe("fabricator_core", "可安装在原版兼容的模组槽中：品质加成15%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +15% quality. Native slot restrictions, wear and removal rules apply.");
0119: 		Describe("quantum_controller", "可安装在原版兼容的模组槽中：效率加成10%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +10% efficiency. Native slot restrictions, wear and removal rules apply.");
0120: 		array = new string[3] { "colony_painting", "gilt_music_box", "platinum_trophy" };
~~~

### Consumed after the final use
- \`XIAOWONightAuction/AuctionItemLinkRules.cs:117\`
~~~csharp
0114: 		{
0115: 			Describe(array[i], "拖到受损模组或模组提取器上维修，沿用原版电子零件的条件、成功率和消耗。不用于其他设备升级。", "Drag onto a damaged module or module extractor to repair it with native electronic-part conditions, chances and consumption. Does not upgrade other equipment.");
0116: 		}
0117: 		Describe("calibrator", "拖到已卡住的自有模组上，解除卡住状态。剩余次数：{0}/3。每次成功消耗1次，价值按剩余次数降低，耗尽后销毁。", "Drag onto an owned stuck module to release it. Uses remaining: {0}/3. Each successful release uses one charge; value scales with remaining charges. Consumed after the final use.");
0118: 		Describe("fabricator_core", "可安装在原版兼容的模组槽中：品质加成15%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +15% quality. Native slot restrictions, wear and removal rules apply.");
0119: 		Describe("quantum_controller", "可安装在原版兼容的模组槽中：效率加成10%。保留原版安装限制、磨损和拆卸规则。", "Install in a compatible native module slot: +10% efficiency. Native slot restrictions, wear and removal rules apply.");
0120: 		array = new string[3] { "colony_painting", "gilt_music_box", "platinum_trophy" };
~~~

## 3829085054

### 收纳箱
- \`ItemStorage/BoxItem.cs:19\`
~~~csharp
0016: 
0017: 	internal const string UiTag = "ITEM_STORAGE_BOX_UI_V30";
0018: 
0019: 	internal const string DisplayNameText = "收纳箱";
0020: 
0021: 	internal const string GameItemTypeStorage = "STORAGE";
0022: 
~~~
- \`ItemStorage/BoxItem.cs:468\`
~~~csharp
0465: 			val.identifier = "Dream_Item_Storage";
0466: 			try
0467: 			{
0468: 				val.SetName("收纳箱");
0469: 			}
0470: 			catch
0471: 			{
~~~
- \`ItemStorage/BoxItem.cs:473\`
~~~csharp
0470: 			catch
0471: 			{
0472: 			}
0473: 			val.name = "收纳箱";
0474: 			try
0475: 			{
0476: 				val.SetSprite(string.Empty, "Dream_Item_Storage");
~~~
- \`ItemStorage/BoxItem.cs:480\`
~~~csharp
0477: 			}
0478: 			catch (Exception ex)
0479: 			{
0480: 				Core.Log?.Warning("收纳箱贴图字段设置失败：" + ex.Message);
0481: 			}
0482: 			try
0483: 			{
~~~
- \`ItemStorage/BoxItem.cs:488\`
~~~csharp
0485: 			}
0486: 			catch (Exception ex2)
0487: 			{
0488: 				Core.Log?.Warning("收纳箱 SetShape 2x3 失败：" + ex2.Message);
0489: 			}
0490: 			val.SetValue(600L);
0491: 			try
~~~
- \`ItemStorage/BoxItem.cs:518\`
~~~csharp
0515: 			}
0516: 			catch (Exception ex3)
0517: 			{
0518: 				Core.Log?.Warning("收纳箱 AddContainerTooltipTag 失败：" + ex3.Message);
0519: 			}
0520: 			try
0521: 			{
~~~
- \`ItemStorage/BoxItem.cs:579\`
~~~csharp
0576: 			Func<GameItem> arg = () => Create() ?? throw new InvalidOperationException("BoxItem.Create 返回 null（目录/模板未就绪）");
0577: 			if (!add(arg))
0578: 			{
0579: 				Core.Log?.Warning("收纳箱注册：" + label + ".Add 返回 false");
0580: 			}
0581: 			else
0582: 			{
~~~
- \`ItemStorage/BoxItem.cs:588\`
~~~csharp
0585: 		}
0586: 		catch (Exception ex)
0587: 		{
0588: 			Core.Log?.Error("注册收纳箱失败（" + label + "）：" + ex.Message);
0589: 		}
0590: 	}
0591: 
~~~
- \`ItemStorage/BoxStowAll.cs:262\`
~~~csharp
0259: 		{
0260: 			_tracked.Remove(item4);
0261: 			BoxHotkey.ForgetUid(item4);
0262: 			Core.Log?.Msg("收纳箱（uid " + item4 + "）已被系统销毁（卖出/垃圾桶清空等），其单箱快捷键已释放。");
0263: 		}
0264: 		if (dictionary.Count > 0 || _tracked.Count > 0)
0265: 		{
~~~
- \`ItemStorage/BoxStowAll.cs:270\`
~~~csharp
0267: 		}
0268: 		else if (_hadOwner && ++_zeroStreak >= 5)
0269: 		{
0270: 			ClearKey("玩家拥有的收纳箱已全部被系统销毁（卖出/垃圾桶清空等），释放全箱键供其他功能绑定");
0271: 			BoxRestock.OnAllBoxesDestroyed();
0272: 			_hadOwner = false;
0273: 			_zeroStreak = 0;
~~~
- \`ItemStorage/BoxStowAll.cs:317\`
~~~csharp
0314: 		{
0315: 			if (_hadOwner && _tracked.Count == 0)
0316: 			{
0317: 				ClearKey("玩家已没有任何拥有的收纳箱（Watch 已确认全销毁，按键兜底清除）");
0318: 			}
0319: 			else
0320: 			{
~~~
- \`ItemStorage/BoxStowAll.cs:321\`
~~~csharp
0318: 			}
0319: 			else
0320: 			{
0321: 				Core.Log?.Msg("全箱收纳[" + context + "]：场景里没有可用的收纳箱（可能拿在手上或不在店内），键位保留。");
0322: 			}
0323: 			return;
0324: 		}
~~~

### 一键收纳
- \`ItemStorage/BoxTransfer.cs:478\`
~~~csharp
0475: 			}
0476: 			if (flag6 && !flag7 && preferredSlot < 0)
0477: 			{
0478: 				Core.Log?.Msg("收纳箱 deposit：含水水容器拒绝一键收纳（拖到面板格子松手可存）id=" + text);
0479: 				if (notify)
0480: 				{
0481: 					BoxItem.NotifyUser("含水容器请拖到面板格子存放");
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
- \`ItemStorage/AutoStow.cs:134\`
~~~csharp
0131: 		}
0132: 		catch (Exception ex)
0133: 		{
0134: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳读槽失败：" + ex.Message);
0135: 			return null;
0136: 		}
0137: 	}
~~~
- \`ItemStorage/AutoStow.cs:155\`
~~~csharp
0152: 		}
0153: 		catch (Exception ex)
0154: 		{
0155: 			Core.Log?.Warning("收纳箱[" + context + "] 一键收纳扫描店铺失败：" + ex.Message);
0156: 		}
0157: 		HashSet<GameItem> hashSet = new HashSet<GameItem>();
0158: 		List<GameItem> list2 = new List<GameItem>();
~~~
- \`ItemStorage/AutoStow.cs:188\`
~~~csharp
0185: 				}
0186: 				if (IsFullWaterContainer(item))
0187: 				{
0188: 					Core.Debug("收纳箱[" + context + "] 跳过（含水水容器不一键收纳，防混水污染）：" + text);
0189: 					continue;
0190: 				}
0191: 				try
~~~
- \`ItemStorage/AutoStow.cs:195\`
~~~csharp
0192: 				{
0193: 					if (item.IsTag("ANIMAL_SPECIES_TAG") || item.IsTag("ANIMAL_AGE_TAG"))
0194: 					{
0195: 						Core.Debug("收纳箱[" + context + "] 跳过（活物不一键收纳，玩家可手动拖入）：" + text);
0196: 						continue;
0197: 					}
0198: 				}
~~~
- \`ItemStorage/AutoStow.cs:211\`
~~~csharp
0208: 				{
0209: 					if (BoxItem.IsModuleItem(item) == true)
0210: 					{
0211: 						Core.Debug("收纳箱[" + context + "] 跳过（【模组】标签物品，不一键收纳）：" + text);
0212: 						continue;
0213: 					}
0214: 					if (BoxItem.IsSupplyCrateItem(item) != true)
~~~
- \`ItemStorage/AutoStow.cs:229\`
~~~csharp
0226: 					}
0227: 					if (flag)
0228: 					{
0229: 						Core.Debug("收纳箱[" + context + "] 跳过（锁定物资箱，不一键收纳）：" + text);
0230: 						continue;
0231: 					}
0232: 					Core.Debug("收纳箱[" + context + "] 刷开物资箱参与收纳：" + text);
~~~

### 全箱收纳
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
- \`ItemStorage/BoxStowAll.cs:179\`
~~~csharp
0176: 		{
0177: 			return;
0178: 		}
0179: 		Core.Debug("全箱收纳快捷键 " + ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() + " 触发。");
0180: 		BoxItem.ClickSound();
0181: 		try
0182: 		{
~~~
- \`ItemStorage/BoxStowAll.cs:187\`
~~~csharp
0184: 		}
0185: 		catch (Exception ex)
0186: 		{
0187: 			Core.Log?.Warning("全箱收纳执行异常：" + ex.Message);
0188: 		}
0189: 	}
0190: 
~~~
- \`ItemStorage/BoxStowAll.cs:288\`
~~~csharp
0285: 	{
0286: 		if (!BoxTransfer.IsInStore())
0287: 		{
0288: 			Core.Debug("全箱收纳[" + context + "]：玩家不在店内，忽略。");
0289: 			BoxItem.NotifyUser("请回到店内再收纳");
0290: 			return;
0291: 		}
~~~
- \`ItemStorage/BoxStowAll.cs:310\`
~~~csharp
0307: 		}
0308: 		catch (Exception ex)
0309: 		{
0310: 			Core.Log?.Warning("全箱收纳[" + context + "] 扫描箱子失败：" + ex.Message);
0311: 			return;
0312: 		}
0313: 		if (list.Count == 0)
~~~
- \`ItemStorage/BoxStowAll.cs:321\`
~~~csharp
0318: 			}
0319: 			else
0320: 			{
0321: 				Core.Log?.Msg("全箱收纳[" + context + "]：场景里没有可用的收纳箱（可能拿在手上或不在店内），键位保留。");
0322: 			}
0323: 			return;
0324: 		}
~~~
- \`ItemStorage/BoxStowAll.cs:355\`
~~~csharp
0352: 		}
0353: 		if (num == 0)
0354: 		{
0355: 			Core.Log?.Msg("全箱收纳[" + context + "]：所有收纳箱都是空的，没有可收的目标种类。");
0356: 			BoxItem.NotifyUser("没有可收纳的物品");
0357: 			return;
0358: 		}
~~~
- \`ItemStorage/BoxStowAll.cs:406\`
~~~csharp
0403: 				num4++;
0404: 			}
0405: 		}
0406: 		Core.Debug("全箱收纳[" + context + "] 耗时 " + stopwatch.ElapsedMilliseconds + " ms（收 " + num2 + " 叠 / 候选 " + list2.Count + " 件 / 参与箱 " + num + "）。");
0407: 		if (num2 > 0)
0408: 		{
0409: 			foreach (GameItem item5 in hashSet2)
~~~
- \`ItemStorage/BoxStowAll.cs:430\`
~~~csharp
0427: 			catch
0428: 			{
0429: 			}
0430: 			Core.Log?.Msg("全箱收纳[" + context + "] 完成：收进 " + num2 + " 叠共 " + num3 + " 件，分入 " + hashSet2.Count + " / " + num + " 个收纳箱" + ((num4 > 0) ? ("，" + num4 + " 叠装不下，留原地") : "") + "。");
0431: 		}
0432: 		else
0433: 		{
~~~
- \`ItemStorage/BoxStowAll.cs:434\`
~~~csharp
0431: 		}
0432: 		else
0433: 		{
0434: 			Core.Log?.Msg("全箱收纳[" + context + "]：店里没有能收进收纳箱的物品" + ((num4 > 0) ? ("（" + num4 + " 叠装不下，留原地）") : "") + "。");
0435: 		}
0436: 		if (num3 > 0)
0437: 		{
~~~
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
- \`ItemStorage/BoxWindow.cs:433\`
~~~csharp
0430: 				}
0431: 				else if (!val6.Attach(val17, 2, 4, 48, 24))
0432: 				{
0433: 					Core.Log?.Warning("收纳箱[" + context + "] attach 一键上货按钮失败");
0434: 				}
0435: 			}
0436: 			catch (Exception ex7)
~~~
- \`ItemStorage/BoxWindow.cs:438\`
~~~csharp
0435: 			}
0436: 			catch (Exception ex7)
0437: 			{
0438: 				Core.Log?.Warning("收纳箱[" + context + "] 一键上货按钮构建失败：" + ex7.Message);
0439: 			}
0440: 			SyncHotkeyMap(pack);
0441: 			StoreIn(ButtonsByRef, ButtonsByUid, pack, dictionary2);
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

### 已收进
- \`ItemStorage/BoxStowAll.cs:438\`
~~~csharp
0435: 		}
0436: 		if (num3 > 0)
0437: 		{
0438: 			BoxItem.NotifyUser("已收进 " + num3 + " 件，分派到各收纳箱");
0439: 		}
0440: 		else
0441: 		{
~~~
- \`ItemStorage/AutoStow.cs:104\`
~~~csharp
0101: 		}
0102: 		if (num2 > 0)
0103: 		{
0104: 			BoxItem.NotifyUser("已收进 " + num2 + " 件");
0105: 		}
0106: 		else if (num5 <= 0)
0107: 		{
~~~

### 分派到各收纳箱
- \`ItemStorage/BoxStowAll.cs:438\`
~~~csharp
0435: 		}
0436: 		if (num3 > 0)
0437: 		{
0438: 			BoxItem.NotifyUser("已收进 " + num3 + " 件，分派到各收纳箱");
0439: 		}
0440: 		else
0441: 		{
~~~

### 件，分派
- \`ItemStorage/BoxStowAll.cs:438\`
~~~csharp
0435: 		}
0436: 		if (num3 > 0)
0437: 		{
0438: 			BoxItem.NotifyUser("已收进 " + num3 + " 件，分派到各收纳箱");
0439: 		}
0440: 		else
0441: 		{
~~~

### 收进
- \`ItemStorage/BoxStowAll.cs:430\`
~~~csharp
0427: 			catch
0428: 			{
0429: 			}
0430: 			Core.Log?.Msg("全箱收纳[" + context + "] 完成：收进 " + num2 + " 叠共 " + num3 + " 件，分入 " + hashSet2.Count + " / " + num + " 个收纳箱" + ((num4 > 0) ? ("，" + num4 + " 叠装不下，留原地") : "") + "。");
0431: 		}
0432: 		else
0433: 		{
~~~
- \`ItemStorage/BoxStowAll.cs:434\`
~~~csharp
0431: 		}
0432: 		else
0433: 		{
0434: 			Core.Log?.Msg("全箱收纳[" + context + "]：店里没有能收进收纳箱的物品" + ((num4 > 0) ? ("（" + num4 + " 叠装不下，留原地）") : "") + "。");
0435: 		}
0436: 		if (num3 > 0)
0437: 		{
~~~
- \`ItemStorage/BoxStowAll.cs:438\`
~~~csharp
0435: 		}
0436: 		if (num3 > 0)
0437: 		{
0438: 			BoxItem.NotifyUser("已收进 " + num3 + " 件，分派到各收纳箱");
0439: 		}
0440: 		else
0441: 		{
~~~
- \`ItemStorage/BoxTransfer.cs:420\`
~~~csharp
0417: 				Core.Debug("收纳箱 deposit：工具箱不能存进收纳箱");
0418: 				if (notify)
0419: 				{
0420: 					BoxItem.NotifyUser("工具箱请勿收进收纳箱");
0421: 				}
0422: 				return false;
0423: 			}
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
- \`ItemStorage/AutoStow.cs:104\`
~~~csharp
0101: 		}
0102: 		if (num2 > 0)
0103: 		{
0104: 			BoxItem.NotifyUser("已收进 " + num2 + " 件");
0105: 		}
0106: 		else if (num5 <= 0)
0107: 		{
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
