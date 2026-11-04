using System;
using System.Collections.Generic;

namespace GameCardPK;

internal static class GameCardFactory
{
	internal static List<Action<TagState>> Pinned = new List<Action<TagState>>();

	internal static readonly List<string> TagIds = new List<string> { "HP", "ATK", "SPD" };

	internal static readonly List<string> TagNames = new List<string> { "生命值", "攻击力", "速度" };

	internal const string GameCardItemId = "game_card";

	internal const string CardSkillItemId = "card_skill";

	internal const string ChessBoardItemId = "chess_board";

	internal const string CardFactoryCardItemId = "card_factory_card";

	internal const string BattleSlotId = "battle_card_slot";

	internal const string RepairGridSlotId = "repair_slot";

	internal const string SkillSlotMaxKey = "SkillSlotMax";

	private static readonly Random Rng = new Random();

	internal static string GameCardItem => ItemConfigInjector.Prefixed("game_card");

	internal static string CardSkillItem => ItemConfigInjector.Prefixed("card_skill");

	internal static string ChessBoardItem => ItemConfigInjector.Prefixed("chess_board");

	internal static bool IsCard(GameItem? item)
	{
		if (item == null)
		{
			return false;
		}
		try
		{
			return item.IsTag("game_card");
		}
		catch
		{
			return false;
		}
	}

	internal static GameItem CreateShell()
	{
		ItemConfigInjector.ItemConfigData itemConfigData = ItemConfigInjector.FindConfig("game_card");
		GameItem val;
		if (itemConfigData != null)
		{
			try
			{
				val = ItemConfigInjector.CreateItem(itemConfigData);
			}
			catch (Exception ex)
			{
				Core.Log?.Error("[卡牌PK] 造卡壳失败（改用空物品）：" + ex.Message);
				val = ItemDirectory.CreateEmptyItem((string)null);
			}
		}
		else
		{
			Core.Log?.Error("[卡牌PK] items.json 里没有 game_card 模板，卡牌只能用空物品兜底。");
			val = ItemDirectory.CreateEmptyItem((string)null);
		}
		BuildCardWindow(val);
		ItemTargetRouter.AttachReceiver(val, ReceiverKind.Card);
		ChipSystem.TraceShell(val);
		return val;
	}

	private static void BuildCardWindow(GameItem item)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		List<MachineryPanel> list = new List<MachineryPanel>();
		GameSlotInventory val = new GameSlotInventory
		{
			identifier = "skill0"
		};
		ContentWindowRegistry.Register(item, "skill0", (GameInventory?)(object)val);
		list.Add(MachineHelper.Panel("核心技能", (PixelElement)(object)val, "skill0", true));
		for (int i = 0; i < GameCardData.MaxSkillSlots; i++)
		{
			int num = i + 1;
			GameSlotInventory val2 = new GameSlotInventory
			{
				identifier = "skill" + num
			};
			ContentWindowRegistry.Register(item, "skill" + num, (GameInventory?)(object)val2);
			list.Add(MachineHelper.Panel("技能槽" + num, (PixelElement)(object)val2, "skill" + num, true));
		}
		item.SetContentWindow(MachineHelper.CreateMachineryWindow(true, list.ToArray()));
	}

	internal static GameItem CreateCard(GameCardData gameCardData, CardVersionSet? versions = null)
	{
		if (gameCardData == null)
		{
			Core.Log?.Error("[卡牌PK] CreateCard 传入的卡牌数据为空，改用兜底卡。");
			gameCardData = CardDatabase.GetFirstOrDefault();
		}
		if (versions == null)
		{
			versions = new CardVersionSet();
		}
		bool flag = ApplyForcedVersions(gameCardData, versions);
		bool isVariant = gameCardData.IsVariant;
		VariantEntry variantEntry = null;
		if (versions.AltArt)
		{
			if (isVariant)
			{
				versions.Add("VariantArt");
			}
			else
			{
				variantEntry = MutationEffectTable.GetVariant(gameCardData.Mutant);
				if (variantEntry == null)
				{
					if (flag)
					{
						Core.Log?.Error("[卡牌PK]「" + gameCardData.Name + "」的 cards[].versions 要求必定异画，但它没有 mutant 指针（或指向的形态不存在）⇒ 这次只能出普通卡。");
					}
					else
					{
						Core.Debug("[卡牌PK]「" + gameCardData.Name + "」没有配变异形态，异画请求降级为普通卡。");
					}
					versions.AltArt = false;
				}
				else
				{
					versions.Add("VariantArt");
				}
			}
		}
		bool altArt = versions.AltArt;
		GameItem val = CreateShell();
		CardIdentity.Set(val, gameCardData.Id);
		string name = ((altArt && variantEntry != null && variantEntry.Name.Length > 0) ? variantEntry.Name : gameCardData.Name);
		val.SetName(name);
		string text = ((altArt && variantEntry != null && variantEntry.Description.Length > 0) ? variantEntry.Description : gameCardData.Description);
		if (!string.IsNullOrEmpty(text))
		{
			val.shortDescription = text;
		}
		int num = GameCardData.RollValue(gameCardData.HP, gameCardData.AttributeVariance, Rng);
		int num2 = GameCardData.RollValue(gameCardData.ATK, gameCardData.AttributeVariance, Rng);
		int num3 = GameCardData.RollValue(gameCardData.SPD, gameCardData.AttributeVariance, Rng);
		ChangeTag(TagIds[0], TagNames[0], val, num);
		ChangeTag(TagIds[1], TagNames[1], val, num2);
		ChangeTag(TagIds[2], TagNames[2], val, num3);
		string text2 = SeriesDatabase.DisplayNames(gameCardData.Series);
		if (!string.IsNullOrEmpty(text2))
		{
			ChangeTagString("Series", val, text2);
		}
		int num4 = GameCardData.RollSlotCount(gameCardData.SkillSlot, gameCardData.SlotVariance, Rng);
		ChangeTag("SkillSlotMax", "技能槽上限", val, num4);
		GameSlotInventory val2 = ContentWindowRegistry.Find(val, "skill0")?.TryCast<GameSlotInventory>();
		CardSkillData skill = BattleDatabase.GetSkill(gameCardData.GetSkillAt(1));
		if (skill != null && val2 != null)
		{
			GameItem val3 = CreateSkill(skill);
			val3.EnableTag(TagDisplay.NewTagDatas["CoreSkill"].Id, true);
			InventoryHelper.TrueAdd(val2, val3);
		}
		else
		{
			Core.Log?.Error("[卡牌PK] 卡牌 " + gameCardData.Name + " 的核心技能没有配置（或核心槽丢失）：" + gameCardData.GetSkillAt(1));
		}
		StampVersionTags(val, versions);
		CardPricing.Apply(val, num, num2, num3, num4, versions);
		if (versions.IsMisprint)
		{
			try
			{
				val.SetGameItemType("misprint_card");
			}
			catch (Exception ex)
			{
				Core.Debug("[卡牌PK] 设错版类型失败：" + ex.Message);
			}
		}
		Core.Debug("[卡牌PK] 生成卡牌 " + gameCardData.Name + "（" + ItemConfigInjector.BareId(gameCardData.Id) + "）：版本 " + versions.Describe() + "，技能槽上限 " + num4 + "（物理 " + GameCardData.MaxSkillSlots + " 槽），自带技能 " + gameCardData.Skills.Count + " 个，卡牌价值 " + CardPricing.ReadValue(val) + "。");
		if (altArt || isVariant)
		{
			MutationSystem.MarkAltArt(val);
		}
		return val;
	}

	internal static GameItem CreateSkill(CardSkillData cardSkillData)
	{
		GameItem val = DirectoryMaster.Item(CardSkillItem, true);
		if (cardSkillData == null)
		{
			Core.Log?.Error("[卡牌PK] CreateSkill 传入的数据为空");
			return val;
		}
		ApplySkillData(val, cardSkillData);
		return val;
	}

	internal static void ApplySkillData(GameItem item, CardSkillData cardSkillData)
	{
		if (item == null || cardSkillData == null)
		{
			return;
		}
		item.identifier = cardSkillData.Id;
		item.SetName(cardSkillData.Name);
		string text = SkillText.ForItem(cardSkillData);
		if (!string.IsNullOrEmpty(text))
		{
			item.shortDescription = text;
		}
		item.longDescription = text;
		if (cardSkillData.Value <= 0)
		{
			return;
		}
		try
		{
			item.SetValue((long)cardSkillData.Value);
		}
		catch (Exception ex)
		{
			Core.Debug("[卡牌PK] 技能条价值没写进去（" + cardSkillData.Id + "）：" + ex.Message);
		}
	}

	internal static GameItem CreateChessBoard()
	{
		return DirectoryMaster.Item(ChessBoardItem, true);
	}

	internal static void AttachChessBoardWindow(GameItem item)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		GameSlotInventory val = new GameSlotInventory
		{
			identifier = "battle_card_slot"
		};
		GameGridInventory val2 = new GameGridInventory(4, 4)
		{
			identifier = "repair_slot"
		};
		item.EnableTag("CAN_USE_ON_NON_OWNED_TAG", true);
		((GameItemFunc)item).mayItemTargetItemFunc = (GameItem src, GameItem tgt) => EndDragDispatcher.CanBattleStart(src, tgt);
		((GameItemFunc)item).canItemTargetItemFunc = (GameItem src, GameItem tgt) => EndDragDispatcher.CanBattleStart(src, tgt);
		((GameItemFunc)item).onItemTargetItemFunc = delegate(GameItem src, GameItem tgt)
		{
			EndDragDispatcher.StartBattleFromTarget(src, tgt);
		};
		ContentWindowRegistry.Register(item, "battle_card_slot", (GameInventory?)(object)val);
		ContentWindowRegistry.Register(item, "repair_slot", (GameInventory?)(object)val2);
		item.SetContentWindow(MachineHelper.CreateMachineryWindow(true, (MachineryPanel[])(object)new MachineryPanel[2]
		{
			MachineHelper.Panel("参战卡牌", (PixelElement)(object)val, "battle", true),
			MachineHelper.Panel("修复卡牌", (PixelElement)(object)val2, "repair", true)
		}));
	}

	internal static GameInventory? FindContentInventory(GameItem? board, string identifier)
	{
		if (board == null || string.IsNullOrEmpty(identifier))
		{
			return null;
		}
		try
		{
			GameInventory val = ContentWindowRegistry.Find(board, identifier);
			if (val != null)
			{
				return val;
			}
			PixelWindow contentWindow = board.contentWindow;
			if (contentWindow == null)
			{
				return null;
			}
			PixelElement childElement = contentWindow.childElement;
			if (childElement == null)
			{
				return null;
			}
			GridPixelElement val2 = childElement.TryCast<GridPixelElement>();
			if (val2 == null)
			{
				return null;
			}
			int gridWidth = val2.gridWidth;
			int gridHeight = val2.gridHeight;
			if (gridWidth <= 0 || gridHeight <= 0)
			{
				return null;
			}
			for (int i = 0; i < gridWidth; i++)
			{
				for (int j = 0; j < gridHeight; j++)
				{
					PixelElement val3 = null;
					try
					{
						val3 = val2.GetElement(i, j);
					}
					catch
					{
						continue;
					}
					if (val3 != null)
					{
						GameInventory val4 = val3.TryCast<GameInventory>();
						if (val4 != null && string.Equals(Core.Clean(val4.identifier), identifier, StringComparison.OrdinalIgnoreCase))
						{
							return val4;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Core.Debug("[卡牌PK] 读棋盘内容窗失败（" + identifier + "）：" + ex.Message);
		}
		return null;
	}

	internal static GameItem? FindBattleCardOnBoard(GameItem? board)
	{
		if (board == null)
		{
			return null;
		}
		GameSlotInventory val = FindContentInventory(board, "battle_card_slot")?.TryCast<GameSlotInventory>();
		if (val != null)
		{
			GameItem childItem = val.childItem;
			if (childItem != null)
			{
				return childItem;
			}
		}
		try
		{
			List<GameItem> list = board.FindAllChildItems();
			if (list == null)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameItem val2 = list[i];
				if (val2 != null && val2.IsGameItemType("game_card"))
				{
					GameInventory parentInventory = val2.parentInventory;
					if (parentInventory != null && string.Equals(Core.Clean(parentInventory.identifier), "battle_card_slot", StringComparison.OrdinalIgnoreCase))
					{
						return val2;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Core.Debug("[卡牌PK] 扫描棋盘参战卡失败：" + ex.Message);
		}
		return null;
	}

	internal static GameGridInventory? FindRepairGrid(GameItem? board)
	{
		return FindContentInventory(board, "repair_slot")?.TryCast<GameGridInventory>();
	}

	private static bool ApplyForcedVersions(GameCardData data, CardVersionSet versions)
	{
		bool result = false;
		List<string> versions2 = data.Versions;
		if (versions2 == null || versions2.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < versions2.Count; i++)
		{
			string text = Core.Clean(versions2[i]);
			if (text.Length == 0)
			{
				Core.Log?.Error("[卡牌PK]「" + data.Name + "」的 cards[].versions 里有一条空 tag，已忽略。");
			}
			else if (string.Equals(text, "VariantArt", StringComparison.OrdinalIgnoreCase))
			{
				versions.AltArt = true;
				result = true;
			}
			else
			{
				versions.Add(text);
			}
		}
		return result;
	}

	private static void StampVersionTags(GameItem item, CardVersionSet versions)
	{
		if (item == null || versions == null)
		{
			return;
		}
		IReadOnlyList<string> tags = versions.Tags;
		for (int i = 0; i < tags.Count; i++)
		{
			string text = tags[i];
			if (!(text == "VariantArt"))
			{
				if (!TagDisplay.NewTagDatas.TryGetValue(text, out CustomTagData value) || value == null || string.IsNullOrEmpty(value.Id))
				{
					Core.Log?.Error("[卡牌PK] 版本 tag 没登记：" + text + "（这张卡的这一版只算价、不显示）。");
				}
				else
				{
					ChangeTag(text, value.DisplayName, item, 1);
				}
			}
		}
	}

	internal static void ChangeTag(string id, string name, GameItem gameItem, int v)
	{
		if (gameItem != null)
		{
			WriteIntTag(gameItem, "game_card_pk." + id, v);
		}
	}

	internal static bool ChangeTagRaw(string fullTag, GameItem? gameItem, int v)
	{
		if (gameItem == null || string.IsNullOrEmpty(fullTag))
		{
			return false;
		}
		return WriteIntTag(gameItem, fullTag, v);
	}

	private static bool WriteIntTag(GameItem gameItem, string tag, int v)
	{
		if (gameItem == null)
		{
			return false;
		}
		gameItem.EnableTag(tag, true);
		Action<TagState> action = delegate(TagState t)
		{
			if (t != null)
			{
				t.SetInt(v);
			}
		};
		if (action == null)
		{
			Core.Log?.Error("[卡牌PK] 标签回调转换失败，属性没写进去：" + tag);
			return false;
		}
		Pinned.Add(action);
		ModifyTagSafe(gameItem, tag, action);
		int num = ReadInt(gameItem, tag);
		if (num != v)
		{
			Core.Log?.Error("[卡牌PK] 标签写入后回读不一致：" + tag + " 期望 " + v + " 实际 " + num);
			return false;
		}
		return true;
	}

	internal static void ChangeTagString(string id, GameItem gameItem, string v)
	{
		if (gameItem != null)
		{
			WriteStringTag(gameItem, "game_card_pk." + id, v);
		}
	}

	internal static bool ChangeTagStringRaw(string fullTag, GameItem? gameItem, string v)
	{
		if (gameItem == null || string.IsNullOrEmpty(fullTag))
		{
			return false;
		}
		return WriteStringTag(gameItem, fullTag, v);
	}

	private static bool WriteStringTag(GameItem gameItem, string tag, string v)
	{
		if (gameItem == null)
		{
			return false;
		}
		gameItem.EnableTag(tag, true);
		Action<TagState> action = delegate(TagState t)
		{
			if (t != null)
			{
				t.SetString(v);
			}
		};
		if (action == null)
		{
			Core.Log?.Error("[卡牌PK] 标签回调转换失败，文本没写进去：" + tag);
			return false;
		}
		Pinned.Add(action);
		ModifyTagSafe(gameItem, tag, action);
		TagState tagReadonly = gameItem.GetTagReadonly(tag);
		string text = ((tagReadonly != null) ? tagReadonly.GetString() : null) ?? string.Empty;
		if (!string.Equals(text, v, StringComparison.Ordinal))
		{
			Core.Log?.Error("[卡牌PK] 文本标签写入后回读不一致：" + tag + " 期望「" + v + "」实际「" + text + "」");
			return false;
		}
		return true;
	}

	private static void ModifyTagSafe(GameItem gameItem, string tag, Action<TagState> act)
	{
		try
		{
			gameItem.ModifyTag(tag, act, false);
		}
		catch (Exception ex)
		{
			Core.Log?.Error("[卡牌PK] ModifyTag 校验抛异常（已放行，未用 skipValidate 重写）：" + tag + " — " + ex.Message);
		}
	}

	internal static int ReadInt(GameItem? item, string tag)
	{
		if (item == null || string.IsNullOrEmpty(tag))
		{
			return 0;
		}
		if (!item.IsTag(tag))
		{
			return 0;
		}
		TagState tagReadonly = item.GetTagReadonly(tag);
		if (tagReadonly == null)
		{
			return 0;
		}
		return tagReadonly.GetInt();
	}

	internal static int ReadTag(GameItem? item, string key)
	{
		if (!TagDisplay.NewTagDatas.TryGetValue(key, out CustomTagData value) || value == null)
		{
			return 0;
		}
		return ReadInt(item, value.Id);
	}

	internal static bool TryReadSkillSlotMax(GameItem? item, out int value)
	{
		value = 0;
		if (item == null)
		{
			return false;
		}
		if (!TagDisplay.NewTagDatas.TryGetValue("SkillSlotMax", out CustomTagData value2) || value2 == null)
		{
			return false;
		}
		if (!item.IsTag(value2.Id))
		{
			return false;
		}
		TagState tagReadonly = item.GetTagReadonly(value2.Id);
		if (tagReadonly == null)
		{
			return false;
		}
		int num = tagReadonly.GetInt();
		if (num < 0)
		{
			num = 0;
		}
		if (num > GameCardData.MaxSkillSlots)
		{
			num = GameCardData.MaxSkillSlots;
		}
		value = num;
		return true;
	}

	internal static int ReadSkillSlotMax(GameItem? item, int fallback)
	{
		if (TryReadSkillSlotMax(item, out var value))
		{
			return value;
		}
		if (fallback < 0)
		{
			fallback = 0;
		}
		if (fallback > GameCardData.MaxSkillSlots)
		{
			fallback = GameCardData.MaxSkillSlots;
		}
		return fallback;
	}
}
