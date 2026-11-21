extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MidnightPawn.Game;

public static class ItemFactory
{
	public const string BarrelSemenId = "barrel_semen";

	public const string BarrelMilkWondrousId = "barrel_milk_wondrous";

	public const string BarrelMilkRichId = "barrel_milk_rich";

	public const string BarrelMilkDilutedId = "barrel_milk_diluted";

	public const int BarrelMaxUses = 1000;

	private const string MilkOwnSpriteTag = "MILK_REMAKE_OWN_SPRITE";

	public static void Register(ModItemDirectory dir)
	{
		// 原版是前置：本体 / 苗床 / 神奇乳汁 / 精液 / 排卵诱发剂全部由原版注册；
		// 本模组只新增自己的物品，排卵针使用效果沿用原版。
		Directory<GameItem> directory = (Directory<GameItem>)(object)dir;
		directory.Set("milk_wondrous", (Func<GameItem>)CreateMilk);
		directory.Set("milk_rich", (Func<GameItem>)CreateRichMilk);
		directory.Set("milk_diluted", (Func<GameItem>)CreateDilutedMilk);
		directory.Set(BarrelSemenId, (Func<GameItem>)CreateSemenBarrel);
		directory.Set(BarrelMilkWondrousId, (Func<GameItem>)CreateWondrousMilkBarrel);
		directory.Set(BarrelMilkRichId, (Func<GameItem>)CreateRichMilkBarrel);
		directory.Set(BarrelMilkDilutedId, (Func<GameItem>)CreateDilutedMilkBarrel);
		directory.Set("tentacle_manual_remake", (Func<GameItem>)CreateManual);
		directory.Set(SecurityLiaison.CardItemId, (Func<GameItem>)CreatePhoneCard);
		directory.Set(NightReportNote.ItemId, (Func<GameItem>)NightReportNote.CreateItem);
		// 原生纸书工厂负责基础属性；名册只绑定动态内容和旧档恢复。
		directory.Set(MaidenLedger.ITEM_ID, (Func<GameItem>)MaidenLedger.CreateItem);
		ModCtx.Log("物品注册：本模组新增三种乳汁、四种桶装、说明便签、名片、夜间报告便签；基础物品由原版提供");
	}

	public static GridShape RectShape(int w, int h)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return new GridShapeBuilder(w, h).SetDataFill(w, h, (byte)1).Build();
	}

	private static void Enable(GameItem item, string tag)
	{
		item.ModifyTag(tag, (Action<TagState>)delegate(TagState s)
		{
			s.Enable();
		}, false);
	}

	public static void LogShape(GameItem item, string tag)
	{
		try
		{
			int num = (((Object)(object)UISettings.current != (Object)null) ? UISettings.current.gridSize : (-1));
			GridShape shape = item.shape;
			ModCtx.Log(string.Format("SHAPE {0}: gridSize={1} shape={2}x{3} global={4}x{5} sprite={6}", tag, num, (shape != null) ? shape.width : (-1), (shape != null) ? shape.height : (-1), (shape != null) ? shape.globalWidth : (-1), (shape != null) ? shape.globalHeight : (-1), item.spritePath ?? "null"));
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("LogShape 失败: " + ex.Message);
		}
	}

	private static GameItem CreateManual()
	{
		GameItem val = ItemDirectory.CreateEmptyItem("tentacle_manual_remake");
		val.name = "触手本体 · 暗流交易说明";
		val.flavorText = "字迹歪歪扭扭，一看就是用触手写的。";
		val.unitValue = 0L;
		val.SetGameItemType("TOOL");
		RefreshManualDescription(val);
		val.onLoaded = RefreshManualDescription;
		val.SetSpriteAndShapeFromMod("lewd_meme", "item_note");
		val.SetShape(RectShape(2, 2));
		val.EnableTag("IMPORTANT_TAG", false);
		val.EnableTag("NOT_FOR_RESALE", false);
		LogShape(val, "暗流交易说明");
		return val;
	}

	internal static void RefreshManualDescription(GameItem item)
	{
		if (item == null || item.identifier != "tentacle_manual_remake")
		{
			return;
		}
		item.name = "触手本体 · 暗流交易说明";
		item.shortDescription = "【夜间结算】\n活动、受孕、孕期、分娩和产出沿用原版 1.9.2，随后追加本模组三种乳汁。每份追加先扣产出区一瓶原版神奇乳汁，无瓶装则扣神奇乳汁桶一次；无可扣除也照常产出。优先直接补入已有同类桶，桶满才放瓶装，放置失败退回所扣来源；同一桶先扣再补时次数净变化为 0。泌乳等级提高追加概率，超过 100% 后可能多份；单价固定。侵蚀区苗床箱结算默认关闭，开启后读取全部存储页并直接调用原版循环，主苗床区为空也参与，计入原版报告与经验。\n\n"
			+ "【疲劳】\n本体、开启箱内结算的苗床箱、闹钟及开启的陷阱按原版实际次数追加疲劳，其它位置每天恢复 25 点。所有设备共用本体等级系数：1～10 级依次为 1、0.8、0.65、0.5、0.4、0.3、0.25、0.2、0.15、0.1；11 级起不增长。疲劳只修正本模组估值，不修改行为强度或受孕概率。入夜疲劳达到 90 时，整名角色本夜结算暂停：无活动、无双方产出、无受孕判定，已有孕期与分娩也暂停；恢复 5 点，当晚不会重新活动。关闭疲劳系统后不拦截。喂乳汁最低降至 40。\n\n"
			+ "【桶装】\n追加产出后扫描整个本体产出区，精液至少 8 瓶、每种本模组乳汁至少 3 瓶时可新建对应桶装；原版与本模组神奇乳汁合计按 3 瓶装入神奇乳汁桶。四类物品各有独立开关，只禁止自动新建；已有桶仍自动补充。按实际堆叠数量换算，桶上限 1000 次。新桶无法放置则保留瓶装；追加夜报含直接入桶产量，库存装桶另记瓶装转换量。原版夜报乳汁为原版生成量，未扣后续替换，并非最终库存。\n\n"
			+ "【交易与使用】\n展示乳汁种类影响收购商到店概率，成交记入名册。桶装点击后输入数量批量取出，成功一瓶扣一次；普通液体可拖到对应桶补充。治安部终章识别原版女儿标识，旧版出生标识读档时兼容补齐。\n\n"
			+ "详细机制和开发说明见模组目录 README.md 与 docs。";
	}

	private static GameItem CreateMilk()
	{
		return CreateMilkVariant("milk_wondrous", "神奇乳汁", "从苗床身上挤出来的乳白液体。入口甘甜，后劲是热的。", "上层区的太太们为了它愿意多付三成。");
	}

	private static GameItem CreateRichMilk()
	{
		return CreateMilkVariant("milk_rich", "浓缩乳汁", "从贫乳苗床身上挤出的浓稠乳汁，营养密度很高。入口浓烈，后劲绵长。", "贫乳的乳汁才是真正滋补的东西。");
	}

	private static GameItem CreateDilutedMilk()
	{
		return CreateMilkVariant("milk_diluted", "清甜乳汁", "从巨乳苗床身上挤出的乳汁，产量充足，口感清甜。", "一大瓶，喝着痛快。");
	}

	private static GameItem CreateSemenBarrel()
	{
		return CreateBarrelItem(BarrelSemenId, "桶装触手精液", "tentacle_semen", "桶装触手精液。使用后输入要取出的瓶数，放不下时停止，次数用完后桶消失。", "一整桶，够你用很久。", 2);
	}

	private static GameItem CreateWondrousMilkBarrel()
	{
		return CreateBarrelItem(BarrelMilkWondrousId, "桶装神奇乳汁", "milk_wondrous", "桶装神奇乳汁。使用后输入要取出的瓶数，放不下时停止，次数用完后桶消失。", "这一桶够上层区的太太们喝一阵子。", 0);
	}

	private static GameItem CreateRichMilkBarrel()
	{
		return CreateBarrelItem(BarrelMilkRichId, "桶装浓缩乳汁", "milk_rich", "桶装浓缩乳汁。使用后输入要取出的瓶数，放不下时停止，次数用完后桶消失。", "贫乳的精华，都在这里。", 0);
	}

	private static GameItem CreateDilutedMilkBarrel()
	{
		return CreateBarrelItem(BarrelMilkDilutedId, "桶装清甜乳汁", "milk_diluted", "桶装清甜乳汁。使用后输入要取出的瓶数，放不下时停止，次数用完后桶消失。", "一大桶，喝着痛快。", 0);
	}

	private static GameItem CreateBarrelItem(string identifier, string displayName, string baseItemId, string description, string flavor, int contrabandLevel)
	{
		GameItem val = ItemDirectory.CreateEmptyItem(identifier);
		val.name = displayName;
		val.shortDescription = description;
		val.flavorText = flavor;
		val.unitValue = 0L;
		val.SetGameItemType("LUXURY_ITEM");
		if (baseItemId == "tentacle_semen")
		{
			val.SetGameItemType("CONTRABAND");
			if (contrabandLevel > 0)
			{
				ContrabandHelper.InitContrabandItem(val, contrabandLevel);
			}
		}
		else
		{
			val.SetGameItemType("BREVAGE");
			val.SetGameItemType("TREAT");
			val.SetGameItemType("FOOD");
		}
		val.SetSpriteAndShapeFromMod("lewd_meme_remake", identifier);
		val.SetShape(RectShape(4, 4));
		// 桶装必须像普通物品一样归玩家所有，否则物品栏会拒绝、点击/拖动也会被认为不可用
		ModHelper.SetItemOwnerShip(val, true);
		UseCountHelper.InitUseCountItem(val, BarrelMaxUses, transferable: true, destroyOnEmpty: true, useAutoValue: true, 0, 1);
		SetBarrelUses(val, 1);
		SetupBarrelCallbacks(val, baseItemId);
		SecurityLiaison.ApplyRegisteredIfNeeded(val);
		return val;
	}

	/// <summary>桶装使用：输入数量后逐瓶放置，成功一瓶才扣一次使用次数。</summary>
	private static void SetupBarrelCallbacks(GameItem barrel, string baseItemId)
	{
		barrel.shortDescription = barrel.name + "。使用后输入要取出的瓶数；空间不足时停止，未取出的部分保留在桶中，次数用完后桶消失。";
		barrel.canActivateSlotItemFunc = (GameItem s, GameInventory _, SlotMarker _) => s != null && GeneralHelper.IsItemOwned(s) && UseCountHelper.CanUse(s);
		barrel.mayActivateSlotItemFunc = barrel.canActivateSlotItemFunc;
		barrel.onActivateSlotItemFunc = delegate(GameItem curItem, GameInventory curInv, SlotMarker marker)
		{
			try
			{
				BarrelDispense.Open(curItem, baseItemId);
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("桶装使用失败: " + ex.Message);
			}
		};
		// 充能方向只走“普通液体 → 桶装”：回调挂在液体（拖拽方）上，
		// 避免同时触发桶装侧回调导致一次拖拽加两次次数。
	}

	private static readonly ConditionalWeakTable<GameItem, object> LiquidTargetingReady = new ConditionalWeakTable<GameItem, object>();

	private static readonly ConditionalWeakTable<GameItem, object> BarrelCallbackReady = new ConditionalWeakTable<GameItem, object>();

	/// <summary>把普通精液 / 乳汁注册为“可以拖到对应桶装上充能”。</summary>
	public static void SetupLiquidTargeting(GameItem item)
	{
		if (item == null || LiquidTargetingReady.TryGetValue(item, out _))
		{
			return;
		}
		string baseItemId;
		string barrelId;
		switch (item.identifier)
		{
		case "tentacle_semen":
			baseItemId = "tentacle_semen";
			barrelId = BarrelSemenId;
			break;
		case "milk_wondrous":
		case "wondrous_milk":
			baseItemId = "milk_wondrous";
			barrelId = BarrelMilkWondrousId;
			break;
		case "milk_rich":
			baseItemId = "milk_rich";
			barrelId = BarrelMilkRichId;
			break;
		case "milk_diluted":
			baseItemId = "milk_diluted";
			barrelId = BarrelMilkDilutedId;
			break;
		default:
			return;
		}
		try
		{
			LiquidTargetingReady.Add(item, new object());
			Func<GameItem, GameItem, bool> previousMay = item.mayThisTargetItemFunc;
			Func<GameItem, GameItem, bool> previousCan = item.canThisTargetItemFunc;
			Action<GameItem, GameItem> previousOn = item.onThisTargetItemFunc;
			bool isSemen = baseItemId == "tentacle_semen";

			// Func 多播委托只返回最后一个回调的结果。按目标分流才能扩展桶装，
			// 同时保留液体原有的苗床、触手及其他目标判定。
			item.mayThisTargetItemFunc = (GameItem self, GameItem target) =>
			{
				if (target != null && target.identifier == barrelId)
				{
					return CanChargeBarrel(self, target);
				}
				return previousMay != null ? previousMay(self, target) : !isSemen || IsCorruptTentacle(target);
			};
			item.canThisTargetItemFunc = (GameItem self, GameItem target) =>
			{
				if (target != null && target.identifier == barrelId)
				{
					return CanChargeBarrel(self, target);
				}
				return previousCan != null ? previousCan(self, target) : !isSemen || IsCorruptTentacle(target);
			};
			item.onThisTargetItemFunc = delegate(GameItem self, GameItem target)
			{
				if (self == null || target == null)
				{
					return;
				}
				if (target.identifier == barrelId)
				{
					AddBarrelCharge(target, self, baseItemId);
					return;
				}
				if (previousOn != null)
				{
					previousOn(self, target);
				}
				else if (isSemen && IsCorruptTentacle(target))
				{
					if (OriginalMod::MidnightPawn.Game.CorruptTentacle.OnSemenUsed(target, self))
					{
						ConsumeOne(self);
					}
				}
			};
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("注册液体充能回调失败: " + ex.Message);
		}
	}

	private static bool CanChargeBarrel(GameItem liquid, GameItem barrel)
	{
		return liquid != null && barrel != null && GetBarrelUses(barrel) < BarrelMaxUses && GeneralHelper.IsItemOwned(liquid, barrel);
	}

	/// <summary>读档后补挂桶装回调（存档里的物品不会重新走工厂）。</summary>
	public static void EnsureBarrelCallbacks(GameItem item)
	{
		if (item == null || BarrelCallbackReady.TryGetValue(item, out _))
		{
			return;
		}
		string baseItemId;
		if (item.identifier == BarrelSemenId)
		{
			baseItemId = "tentacle_semen";
		}
		else if (item.identifier == BarrelMilkWondrousId)
		{
			baseItemId = "milk_wondrous";
		}
		else if (item.identifier == BarrelMilkRichId)
		{
			baseItemId = "milk_rich";
		}
		else if (item.identifier == BarrelMilkDilutedId)
		{
			baseItemId = "milk_diluted";
		}
		else
		{
			return;
		}
		try
		{
			BarrelCallbackReady.Add(item, new object());
			SetupBarrelCallbacks(item, baseItemId);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("补挂桶装回调失败: " + ex.Message);
		}
	}

	private static void AddBarrelCharge(GameItem barrel, GameItem source, string baseItemId)
	{
		try
		{
			if (barrel == null || source == null || !IsBarrelSourceItem(source, baseItemId) || GetBarrelUses(barrel) >= BarrelMaxUses)
			{
				return;
			}
			if (!GeneralHelper.IsItemOwned(barrel, source))
			{
				return;
			}
			SetBarrelUses(barrel, GetBarrelUses(barrel) + 1);
			ConsumeOne(source);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("桶装加次数失败: " + ex.Message);
		}
	}

	private static bool IsCorruptTentacle(GameItem item)
	{
		return item != null && item.identifier == "corrupt_tentacle";
	}

	internal static bool IsBarrelSourceItem(GameItem item, string baseItemId)
	{
		if (item == null)
		{
			return false;
		}
		if (item.identifier == baseItemId)
		{
			return true;
		}
		return baseItemId == "milk_wondrous" && item.identifier == "wondrous_milk";
	}

	public static bool IsBarrelIdentifier(string identifier)
	{
		return identifier == BarrelSemenId || identifier == BarrelMilkWondrousId || identifier == BarrelMilkRichId || identifier == BarrelMilkDilutedId;
	}

	public static int GetBarrelUses(GameItem barrel)
	{
		if (barrel == null || !barrel.IsTag("USE_COUNT_ITEM_TAG"))
		{
			return 0;
		}
		try
		{
			TagState tag = barrel.GetTagReadonly("CURRENT_USE_COUNT_INT");
			return tag?.GetInt() ?? 0;
		}
		catch
		{
			return 0;
		}
	}

	public static void SetBarrelUses(GameItem barrel, int uses)
	{
		if (barrel == null)
		{
			return;
		}
		UseCountHelper.SetCurrentUseCount(barrel, Mathf.Clamp(uses, 0, BarrelMaxUses));
	}

	/// <summary>给已有桶装增加次数，返回实际加入的次数。</summary>
	public static int AddBarrelUses(GameItem barrel, int amount)
	{
		if (barrel == null || amount <= 0)
		{
			return 0;
		}
		int current = GetBarrelUses(barrel);
		int added = Mathf.Min(amount, Mathf.Max(0, BarrelMaxUses - current));
		if (added > 0) UseCountHelper.ReplenishUseCount(barrel, added);
		return Mathf.Max(0, GetBarrelUses(barrel) - current);
	}

	/// <summary>按 ID 创建桶装并设置次数。</summary>
	public static GameItem CreateBarrelForOutput(string barrelId, int uses)
	{
		GameItem val = null;
		switch (barrelId)
		{
		case BarrelSemenId:
			val = CreateSemenBarrel();
			break;
		case BarrelMilkWondrousId:
			val = CreateWondrousMilkBarrel();
			break;
		case BarrelMilkRichId:
			val = CreateRichMilkBarrel();
			break;
		case BarrelMilkDilutedId:
			val = CreateDilutedMilkBarrel();
			break;
		}
		if (val != null)
		{
			SetBarrelUses(val, uses);
		}
		return val;
	}

	private static GameItem CreateMilkVariant(string identifier, string displayName, string description, string flavor)
	{
		GameItem val = ItemDirectory.CreateEmptyItem(identifier);
		val.name = displayName;
		val.shortDescription = description;
		val.flavorText = flavor;
		val.unitValue = ((identifier == "milk_rich") ? 150L : ((identifier == "milk_wondrous") ? 80L : 50L));
		val.SetGameItemType("LUXURY_ITEM");
		val.SetGameItemType("BREVAGE");
		val.SetGameItemType("TREAT");
		val.SetGameItemType("FOOD");
		string text = ((identifier == "milk_wondrous" || identifier == "wondrous_milk") ? "milk_medium" : ((identifier == "milk_rich") ? "milk_high" : "milk_low"));
		val.SetSpriteAndShapeFromMod("lewd_meme_remake", text);
		val.SetShape(RectShape(2, 3));
		val.EnableTag(MilkOwnSpriteTag, false);
		ModCtx.Log("乳汁图标: " + text + "（2×3）");
		((GameItemFunc)val).canActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
		((GameItemFunc)val).mayActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
		((GameItemFunc)val).onActivateSlotItemFunc = delegate(GameItem it, GameInventory inv, SlotMarker slot)
		{
			ModHelper.AddCredit(30, true);
			ModCtx.Log("饮下一瓶" + displayName);
			it.Destroy();
		};
		int relief = ((identifier == "milk_wondrous" || identifier == "wondrous_milk") ? 30 : ((identifier == "milk_rich") ? 25 : 20));
		((GameItemFunc)val).mayThisTargetItemFunc = (GameItem self, GameItem target) => target != null && target.IsTag("MAIDEN_FEMALE");
		((GameItemFunc)val).canThisTargetItemFunc = (GameItem self, GameItem target) => target != null && target.IsTag("MAIDEN_FEMALE");
		((GameItemFunc)val).onThisTargetItemFunc = delegate(GameItem self, GameItem target)
		{
			if (target != null && target.IsTag("MAIDEN_FEMALE") && MaidenFatigue.Feed(target, relief, displayName))
			{
				ConsumeOne(self);
			}
		};
		LogShape(val, displayName);
		return val;
	}

	/// <summary>治安部线人的名片：2×1，点开显示电话号码图片。</summary>
	private static GameItem CreatePhoneCard()
	{
		GameItem val = ItemDirectory.CreateEmptyItem(SecurityLiaison.CardItemId);
		val.name = "治安部线人名片";
		val.shortDescription = "一张廉价的名片，正面只印了一个号码：26535。";
		val.flavorText = "背面用铅笔写着：想通了再打。";
		val.unitValue = 0L;
		val.SetGameItemType("DOCUMENT");
		Enable(val, "IMPORTANT_TAG");
		val.SetSpriteAndShapeFromMod("lewd_meme_remake", "phone_card");
		val.SetShape(RectShape(2, 1));
		((GameItemFunc)val).canActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
		((GameItemFunc)val).mayActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
		((GameItemFunc)val).onActivateSlotItemFunc = delegate
		{
			SecurityLiaison.OpenCardWindow();
		};
		LogShape(val, "名片");
		return val;
	}


	/// <summary>只追加价格及剧情属性，使用效果保留原版。</summary>
	public static void ConfigureOvulationShot(GameItem item)
	{
		if (item == null || item.identifier != "ovulation_shot") return;
		item.unitValue = 100L;
		EnsureContraband(item, 1);
	}
	/// <summary>
	/// 读档后补齐剧情管制奖励及精液/排卵针/苗床的违禁属性，保留桶装回调恢复。
	/// 修复成功后才写一条汇总日志；没有需要修复的物品则不写日志。
	/// </summary>
	public static void RepairContrabandOnLoad()
	{
		try
		{
			EmporiumEntry entry = EmporiumEntry.Instance;
			if (entry == null)
			{
				return;
			}
			List<GameItem> allItems = entry.GetAllItems();
			if (allItems == null)
			{
				return;
			}
			HashSet<GameItem> seen = new HashSet<GameItem>();
			int maidens = 0;
			int semen = 0;
			int ovulation = 0;
			foreach (GameItem item in allItems)
			{
				if (item == null || !seen.Add(item))
				{
					continue;
				}
				MigrateLegacyDaughter(item);
				SetupLiquidTargeting(item);
				ConfigureOvulationShot(item);
				SecurityLiaison.ApplyRegisteredIfNeeded(item);
				if (IsBarrelIdentifier(item.identifier))
				{
					// 旧版本生成的桶装可能没有归属，导致物品栏拒绝、无法点击使用/拖动
					if (!GeneralHelper.IsItemOwned(item))
					{
						ModHelper.SetItemOwnerShip(item, true);
					}
					EnsureBarrelCallbacks(item);
					continue;
				}
				// 治安部剧情已经登记为管制物品的，绝对不能再补违禁品词条
				if ((item.IsTag("MAIDEN_FEMALE") && SecurityLiaison.MaidenRegistered) || (item.identifier == "tentacle_semen" && SecurityLiaison.SemenRegistered) || (item.identifier == "ovulation_shot" && SecurityLiaison.OvulationRegistered))
				{
					continue;
				}
				int level;
				int category;
				if (item.IsTag("MAIDEN_FEMALE"))
				{
					level = 3;
					category = 0;
				}
				else if (item.identifier == "tentacle_semen")
				{
					level = 2;
					category = 1;
				}
				else if (item.identifier == "ovulation_shot")
				{
					level = 1;
					category = 2;
				}
				else
				{
					continue;
				}
				if (item.IsGameItemType("RESTRICTED_1") || item.IsGameItemType("RESTRICTED_2") || item.IsGameItemType("RESTRICTED_3"))
				{
					continue;
				}
				// 只在词条/等级/类型不完整时修复；已有完整原版违禁品属性则不重复处理
				if (item.IsTag("CONTRABAND_ITEM_TAG") && item.IsGameItemType("CONTRABAND") && GetContrabandLevel(item) == level)
				{
					continue;
				}
				ContrabandHelper.InitContrabandItem(item, level);
				// 修复成功才计数/写日志
				if (item.IsTag("CONTRABAND_ITEM_TAG") && item.IsGameItemType("CONTRABAND") && GetContrabandLevel(item) == level)
				{
					if (category == 0)
					{
						maidens++;
					}
					else if (category == 1)
					{
						semen++;
					}
					else
					{
						ovulation++;
					}
				}
			}
			if (maidens + semen + ovulation > 0)
			{
				ModCtx.Log($"违禁品词条补全（读档一次）：苗床 {maidens} / 触手精液 {semen} / 排卵诱发剂 {ovulation}");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("违禁品词条补全失败: " + ex.Message);
		}
	}

	/// <summary>物品刚被创建时调用：立刻补上对应等级的违禁品词条（已登记管制的不动）。</summary>
	public static void ApplyContrabandOnCreate(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			int level;
			if (item.IsTag("MAIDEN_FEMALE") || item.identifier == "maiden_lower")
			{
				level = 3;
			}
			else if (item.identifier == "tentacle_semen" || item.identifier == BarrelSemenId)
			{
				level = 2;
			}
			else if (item.identifier == "ovulation_shot")
			{
				level = 1;
			}
			else
			{
				return;
			}
			// 治安部剧情已登记的物品保持管制状态，不重新挂违禁品
			SecurityLiaison.ApplyRegisteredIfNeeded(item);
			if (item.IsGameItemType("RESTRICTED_1") || item.IsGameItemType("RESTRICTED_2") || item.IsGameItemType("RESTRICTED_3"))
			{
				return;
			}
			if (!item.IsTag("CONTRABAND_ITEM_TAG") || !item.IsGameItemType("CONTRABAND") || GetContrabandLevel(item) != level)
			{
				ContrabandHelper.InitContrabandItem(item, level);
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("新物品补违禁词条失败: " + ex.Message);
		}
	}

	public static void EnsureContraband(GameItem item, int level)
	{
		try
		{
			if (item == null)
			{
				return;
			}
			if (item.IsGameItemType("RESTRICTED_1") || item.IsGameItemType("RESTRICTED_2") || item.IsGameItemType("RESTRICTED_3"))
			{
				return;
			}
			if (!item.IsTag("CONTRABAND_ITEM_TAG") || GetContrabandLevel(item) != level)
			{
				ContrabandHelper.InitContrabandItem(item, level);
				ModCtx.Log($"违禁等级修正: {item.identifier} -> {level} 级");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("修正违禁等级失败: " + ex.Message);
		}
	}

	private static int GetContrabandLevel(GameItem item)
	{
		try
		{
			TagState tagReadonly = item.GetTagReadonly("CONTRABAND_LEVEL");
			if (tagReadonly != null)
			{
				return tagReadonly.GetInt();
			}
		}
		catch
		{
		}
		return 0;
	}

	internal static void MigrateLegacyDaughter(GameItem item)
	{
		if (item != null && item.IsTag(ModConst.TAG_MAIDEN_BORN) && !NativeApi.IsDaughter(item))
			item.ModifyTag("MAIDEN_DAUGHTER", s => s.Enable(), false);
	}

	internal static void MigrateLegacyDaughters()
	{
		List<GameItem> items = EmporiumEntry.Instance?.GetAllItems();
		if (items == null) return;
		foreach (GameItem item in items) MigrateLegacyDaughter(item);
	}

	public static void SetInt(GameItem item, string tag, int value)
	{
		item.ModifyTag(tag, (Action<TagState>)delegate(TagState s)
		{
			s.Enable();
			s.SetInt(value);
		}, false);
	}

	public static int GetInt(GameItem item, string tag)
	{
		if (item == null)
		{
			return 0;
		}
		TagState tagReadonly = item.GetTagReadonly(tag);
		if (tagReadonly != null)
		{
			return tagReadonly.GetInt();
		}
		return 0;
	}

	public static void SetStr(GameItem item, string tag, string value)
	{
		item.ModifyTag(tag, (Action<TagState>)delegate(TagState s)
		{
			s.Enable();
			s.SetString(value);
		}, false);
	}

	public static string GetStr(GameItem item, string tag)
	{
		if (item == null)
		{
			return "";
		}
		TagState tagReadonly = item.GetTagReadonly(tag);
		if (tagReadonly != null)
		{
			return tagReadonly.GetString();
		}
		return "";
	}

	public static void UpdateMaidenValue(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			long value = 300L + (long)GetInt(item, "TRAIN_VALUE") * 2L;
			if (item.IsTag("MAIDEN_VIRGIN"))
			{
				value += 200L;
			}
			if (item.IsTag("MAIDEN_ANAL_VIRGIN"))
			{
				value += 50L;
			}
			float num = MaidenFatigue.ValueMul(MaidenFatigue.Get(item));
			if (num < 0.999f || num > 1.001f)
			{
				value = (long)Math.Round((double)value * (double)num);
			}
			item.unitValue = Math.Max(1L, value);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("更新少女价值失败: " + ex.Message);
		}
	}

	public static void CopyFactionTags(GameItem source, GameItem target)
	{
		if (source == null || target == null)
		{
			return;
		}
		string[] array = new string[5] { "FACTION_LOWER_LEVEL", "FACTION_UPPER_LEVEL", "FACTION_SECURITY", "FACTION_BLACK_MARKET", "FACTION_REVOLUTION" };
		foreach (string tag in array)
		{
			if (source.IsTag(tag))
			{
				target.ModifyTag(tag, (Action<TagState>)delegate(TagState s)
				{
					s.Enable();
				}, false);
			}
			else
			{
				target.DisableTag(tag, false);
			}
		}
	}

	public static void UpdateMaidenDescription(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			item.shortDescription = MaidenText.Describe(item);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("更新少女描述失败: " + ex.Message);
		}
	}

	/// <summary>为放回母亲网格的结果提供真实区域或容器名称。</summary>
	internal static string DescribePlacementArea(GameGridInventory inventory)
	{
		try
		{
			EmporiumEntry entry = EmporiumEntry.Instance;
			if (entry != null)
			{
				if (inventory == entry.invElement) return "仓库";
				if (inventory == entry.backInvinvElement) return "己方柜台";
			}
			GameItem container = inventory?.GetParentItem();
			if (container != null && !string.IsNullOrWhiteSpace(container.name))
				return "【" + container.name + "】内（母亲所在分区）";
		}
		catch { }
		return "母亲所在区域";
	}

	/// <summary>
	/// 安全放置：优先指定区域，然后己方柜台和仓库；不投放到外出或顾客区域。
	/// TryAcceptAll 返回剩余数量，返回 0 且物品已有归属容器才算成功。
	/// 不使用 GraphUtils.EmporiumTryAdd —— 它在 CanAccept 与 TryAcceptAll 不一致时会直接销毁物品。
	/// </summary>
	public static bool TryPlaceSafely(GameItem item, out string where, GameGridInventory preferredInput = null)
	{
		where = null;
		if (item == null)
		{
			return false;
		}
		List<KeyValuePair<GraphNodeStorage, string>> nodes = new List<KeyValuePair<GraphNodeStorage, string>>();
		if (preferredInput != null)
		{
			nodes.Add(new KeyValuePair<GraphNodeStorage, string>(preferredInput, DescribePlacementArea(preferredInput)));
		}
		try
		{
			EmporiumEntry entry = EmporiumEntry.Instance;
			if (entry != null)
			{
				nodes.Add(new KeyValuePair<GraphNodeStorage, string>((GraphNodeStorage)(object)entry.backInvinvElement, "己方柜台"));
				nodes.Add(new KeyValuePair<GraphNodeStorage, string>((GraphNodeStorage)(object)entry.invElement, "仓库"));
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("读取商店容器失败: " + ex.Message);
		}
		foreach (KeyValuePair<GraphNodeStorage, string> pair in nodes)
		{
			if (pair.Key == null)
			{
				continue;
			}
			try
			{
				if (!GraphUtils.CanAccept(pair.Key, item))
				{
					continue;
				}
				int remaining = GraphUtils.TryAcceptAll(pair.Key, item, -1);
				if (remaining == 0 && item.parentInventory != null)
				{
					where = pair.Value;
					return true;
				}
			}
			catch (Exception ex2)
			{
				ModCtx.LogWarning("放入 " + pair.Value + " 失败: " + ex2.Message);
			}
		}
		return false;
	}

	/// <summary>消耗一件物品：可堆叠时减 1，否则销毁。</summary>
	public static void ConsumeOne(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			if (item.unitCount > 1)
			{
				item.SetUnitCount(item.unitCount - 1);
			}
			else
			{
				item.Destroy();
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("消耗物品失败: " + ex.Message);
		}
	}
}


