extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MidnightPawn.Game;

public static class TentacleBody
{
	public static readonly List<GameItem> Instances = new List<GameItem>();

	/// <summary>妊娠天数（足月产女）。</summary>
	public const int PREGNANCY_DAYS = 30;

	private static int ReportMaids;
	private static readonly Dictionary<string, int> ReportProducts = new Dictionary<string, int>();
	private static readonly List<string> EventDescriptions = new List<string>();
	private static readonly HashSet<GameGridInventory> NightOutputs = new HashSet<GameGridInventory>();
	private static readonly HashSet<GameItem> SettledMaidens = new HashSet<GameItem>();
	private static readonly Dictionary<GameItem, NightlyMaidenState> MaidenStates = new Dictionary<GameItem, NightlyMaidenState>();
	private static bool SettlementRunning;
	private static bool SettlementCompleting;

	private static readonly List<string> ReportBarrelLines = new List<string>();



	private static int GetToday()
	{
		try
		{
			return StoreStation.Instance?.dayCounter ?? (-1);
		}
		catch
		{
			return -1;
		}
	}

	/// <summary>本体由原版创建并注册，本模组不持有工厂回调；结算/疲劳前从场景里重新登记一遍。</summary>
	public static void RefreshInstances()
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
			Instances.Clear();
			for (int i = 0; i < allItems.Count; i++)
			{
				GameItem item = allItems[i];
				if (IsBodyItem(item) && !Instances.Contains(item))
				{
					Instances.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("刷新触手本体列表失败: " + ex.Message);
		}
	}

	/// <summary>原地检查现有本体便签区，扩为宽 2、高 4 并补齐说明，不重建其他分区。</summary>
	public static void UpgradeLegacyNoteZone()
	{
		try
		{
			RefreshInstances();
			GameItem[] array = Instances.ToArray();
			foreach (GameItem machine in array)
			{
				if (machine == null) continue;
				ZoneSet zones = ResolveZones(machine);
				if (zones == null) continue;
				foreach (GameGridInventory note in zones.Notes)
				{
					CompatOriginal.PrepareNote(note, true);
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("本体便签区检查失败: " + ex.Message);
		}
	}

	/// <summary>收集本体苗床区及按设置启用的侵蚀区苗床箱角色（供疲劳等系统使用）。</summary>
	public static HashSet<GameItem> CollectInputMaidens()
	{
		HashSet<GameItem> result = new HashSet<GameItem>();
		RefreshInstances();
		GameItem[] array = Instances.ToArray();
		foreach (GameItem machine in array)
		{
			if (machine == null)
			{
				continue;
			}
			ZoneSet zones = ResolveZones(machine);
			if (zones == null)
			{
				continue;
			}
			List<GameGridInventory> grids = new List<GameGridInventory>(zones.Inputs);
			grids.AddRange(CollectCorruptionBoxInputs(zones));
			foreach (GameGridInventory input in grids)
			{
				List<GameItem> childItems = ((GameInventory)input).childItems;
				if (childItems == null)
				{
					continue;
				}
				foreach (GameItem item in childItems)
				{
					if (item != null && item.IsTag("MAIDEN_FEMALE"))
					{
						result.Add(item);
					}
				}
			}
		}
		return result;
	}

	/// <summary>只初始化附加统计；原版 SettleAll 随后继续执行。</summary>
	internal static bool BeginOriginalSettlement()
	{
		if (SettlementRunning) return false;
		SettlementRunning = true;
		NightOutputs.Clear();
		SettledMaidens.Clear();
		ReportProducts.Clear();
		MaidenStates.Clear();
		ReportMaids = 0;
		EventDescriptions.Clear();
		ReportBarrelLines.Clear();
		RefreshInstances();
		// 即使当晚没有角色，也整理既有产出库存。
		foreach (GameItem machine in Instances)
		{
			ZoneSet zones = ResolveZones(machine);
			if (zones == null) continue;
			foreach (GameGridInventory output in zones.Outputs) PrepareNightOutput(output);
		}
		return true;
	}

	internal static int GetSettledMaidenCount() => ReportMaids;

	/// <summary>原版遍历结束后整理各产出区，并生成本模组报告便签。</summary>
	internal static void CompleteOriginalSettlement()
	{
		if (!SettlementRunning || SettlementCompleting) return;
		SettlementCompleting = true;
		try
		{
			foreach (GameGridInventory output in NightOutputs)
			{
				try
				{
					FinalizeNightBarrels(output);
				}
				catch (Exception ex)
				{
					ModCtx.LogWarning("附加桶装整理失败: " + ex.Message);
					RecordBarrelFailure("桶装整理", 0, "结算异常：" + ex.Message);
				}
			}
			CreateReportNote();
		}
		catch (Exception ex)
		{
			// 附加报告出错不能阻断原版随后执行的每日充能和结算日期写入。
			ModCtx.LogWarning("附加夜间整理或报告失败: " + ex.Message);
		}
		finally
		{
			SettlementCompleting = false;
			AbortOriginalSettlement();
		}
	}

	internal static void AbortOriginalSettlement()
	{
		NightOutputs.Clear();
		SettledMaidens.Clear();
		MaidenStates.Clear();
		SettlementRunning = false;
	}

	/// <summary>附加苗床页及按设置启用的侵蚀区苗床箱共用已补丁的原循环。</summary>
	internal static void SettleSupplementaryInputs()
	{
		if (!SettlementRunning) return;
		foreach (GameItem machine in Instances.ToArray())
		{
			try
			{
				ZoneSet zones = ResolveZones(machine);
				if (zones == null || zones.Outputs.Count == 0) continue;
				GameGridInventory output = zones.Outputs[0];
				List<GameGridInventory> inputs = new List<GameGridInventory>(zones.Inputs);
				inputs.AddRange(CollectCorruptionBoxInputs(zones));
				HashSet<GameGridInventory> visited = new HashSet<GameGridInventory>();
				foreach (GameGridInventory input in inputs)
				{
					if (input == null || !visited.Add(input)) continue;
					List<GameItem> items = ((GameInventory)input).childItems;
					if (items == null) continue;
					// 共用原版循环和角色去重；主苗床中已处理的角色会在门禁处跳过。
					OriginalMod::MidnightPawn.Game.TentacleBody.NightlyTick(machine, input, output);
				}
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("附加苗床区结算失败: " + ex.Message);
			}
		}
	}

	/// <summary>只读取侵蚀区直接苗床箱的一层存储，不递归任意容器或处理其它分区。</summary>
	private static List<GameGridInventory> CollectCorruptionBoxInputs(ZoneSet zones)
	{
		List<GameGridInventory> result = new List<GameGridInventory>();
		if (!ModSettings.CorruptionBoxSettlement || zones == null) return result;
		HashSet<GameItem> boxes = new HashSet<GameItem>();
		HashSet<GameGridInventory> grids = new HashSet<GameGridInventory>();
		foreach (GameGridInventory corrupt in zones.Corrupts)
		{
			List<GameItem> items = corrupt == null ? null : ((GameInventory)corrupt).childItems;
			if (items == null) continue;
			foreach (GameItem box in items.ToArray())
			{
				if (box == null || box.identifier != "tentacle_box" || !boxes.Add(box)) continue;
				try
				{
					// 游戏按面板和标签页展开存储；读取所有页，不递归箱内其它容器。
					for (int page = 0; page < 32; page++)
					{
						GraphNodeStorage storage = MachineHelper.GetBodyStorage(box, page);
						if (storage == null) break;
						if (storage is GameGridInventory input && grids.Add(input)) result.Add(input);
					}
				}
				catch (Exception ex)
				{
					ModCtx.LogWarning("读取侵蚀区苗床箱失败: " + ex.Message);
				}
			}
		}
		return result;
	}

	private static void PrepareNightOutput(GameGridInventory output)
	{
		if (output == null || !NightOutputs.Add(output)) return;
		ZoneSet zones = new ZoneSet();
		zones.Outputs.Add(output);
		EnsureOutputAcceptsOurMilk(zones);
	}

	private static void RecordNightProduct(string identifier)
	{
		ReportProducts.TryGetValue(identifier, out int count);
		ReportProducts[identifier] = count + 1;
	}

	private static void FinalizeNightBarrels(GameGridInventory output)
	{
		ConvertNightOutputToBarrel(output, "milk_wondrous", ItemFactory.BarrelMilkWondrousId, 3, "桶装神奇乳汁");
		ConvertNightOutputToBarrel(output, "milk_rich", ItemFactory.BarrelMilkRichId, 3, "桶装浓缩乳汁");
		ConvertNightOutputToBarrel(output, "milk_diluted", ItemFactory.BarrelMilkDilutedId, 3, "桶装清甜乳汁");
		ConvertNightOutputToBarrel(output, "tentacle_semen", ItemFactory.BarrelSemenId, 8, "桶装触手精液");
	}

	/// <summary>扫描全部库存；已有桶优先补充，开关只控制新桶。成功放置后才消耗瓶装。</summary>
	private static void ConvertNightOutputToBarrel(GameGridInventory output, string baseItemId, string barrelId, int threshold, string barrelName)
	{
		List<GameItem> children = output?.childItems;
		if (children == null) return;
		List<GameItem> bottles = new List<GameItem>();
		long total = 0;
		foreach (GameItem item in children)
		{
			if (!ItemFactory.IsBarrelSourceItem(item, baseItemId) || item.unitCount <= 0) continue;
			bottles.Add(item);
			total += item.unitCount;
		}
		int remaining = (int)Math.Min(int.MaxValue, total);
		if (remaining == 0) return;
		int converted = 0;
		foreach (GameItem barrel in FindBarrelItems(output, barrelId))
		{
			int before = ItemFactory.GetBarrelUses(barrel);
			int added = ItemFactory.AddBarrelUses(barrel, remaining);
			if (added == 0) continue;
			int consumed = ConsumeBottles(bottles, added);
			if (consumed != added) ItemFactory.SetBarrelUses(barrel, before + consumed);
			remaining -= consumed;
			converted += consumed;
			if (consumed != added || remaining == 0) break;
		}
		while (remaining >= threshold && ModSettings.CreateNightBarrel(barrelId))
		{
			int amount = Math.Min(ItemFactory.BarrelMaxUses, remaining);
			GameItem barrel = ItemFactory.CreateBarrelForOutput(barrelId, amount);
			if (barrel == null)
			{
				RecordBarrelFailure(barrelName, remaining, "创建失败");
				break;
			}
			bool placed = false;
			try { placed = ItemFactory.TryPlaceSafely(barrel, out _, output); }
			finally { if (!placed) barrel.Destroy(); }
			if (!placed)
			{
				RecordBarrelFailure(barrelName, remaining, "产出区、己方柜台和仓库均无可用空间（需要连续 4×4）");
				break;
			}
			int consumed = ConsumeBottles(bottles, amount);
			if (consumed != amount)
			{
				if (consumed == 0) barrel.Destroy();
				else ItemFactory.SetBarrelUses(barrel, consumed);
			}
			remaining -= consumed;
			converted += consumed;
			if (consumed != amount) break;
		}
		if (converted > 0) ReportBarrelLines.Add("【库存装桶】" + barrelName + " +" + converted + " 次（含原有瓶装）");
	}

	private static int ConsumeBottles(List<GameItem> bottles, int amount)
	{
		int consumed = 0;
		try
		{
			for (int i = 0; i < bottles.Count; i++)
			{
				GameItem item = bottles[i];
				if (item == null || item.unitCount <= 0 || item.parentInventory == null) continue;
				int take = Math.Min(amount - consumed, item.unitCount);
				if (take == item.unitCount) { item.Destroy(); bottles[i] = null; }
				else item.SetUnitCount(item.unitCount - take);
				consumed += take;
				if (consumed == amount) break;
			}
		}
		catch (Exception ex)
		{
			RecordBarrelFailure("库存扣除", amount - consumed, ex.Message);
		}
		return consumed;
	}

	private static void RecordBarrelFailure(string barrelName, int remaining, string reason)
	{
		string amount = remaining > 0 ? ("，尚有 " + remaining + " 次未装桶并保留瓶装") : "";
		EventDescriptions.Add("【桶装失败】" + barrelName + "：" + reason + amount + "。");
	}

	private static List<GameItem> FindBarrelItems(GameGridInventory output, string barrelId)
	{
		List<GameItem> result = new List<GameItem>();
		try
		{
			List<GameItem> children = ((GameInventory)output).childItems;
			if (children == null)
			{
				return result;
			}
			foreach (GameItem item in children)
			{
				if (item != null && item.identifier == barrelId)
				{
					result.Add(item);
				}
			}
		}
		catch
		{
		}
		return result;
	}

	private static void CreateReportNote()
	{
		if (ReportProducts.Count == 0 && ReportBarrelLines.Count == 0 && EventDescriptions.Count == 0) return;
		List<string> lines = new List<string>();
		foreach (KeyValuePair<string, int> product in ReportProducts)
		{
			string name = product.Key == "milk_rich" ? "浓缩乳汁" : product.Key == "milk_diluted" ? "清甜乳汁" : "神奇乳汁";
			lines.Add("【本夜追加】" + name + " ×" + product.Value);
		}
		lines.AddRange(ReportBarrelLines);
		lines.AddRange(EventDescriptions);
		string text = string.Join("\n", lines);
		ModCtx.Log("暗流交易夜报: " + text);
		ModHelper.SetModData("lewd_meme_remake", "last_report", "第 " + GetToday() + " 天：" + text);
		PlayerStore.Instance?.AddNightLog("【暗流交易】\n" + text, "e68ab5");
		NightReportNote.Create(GetToday(), text);
	}

	internal static List<GameGridInventory> ResolveGrids(GameItem machine)
	{
		if (machine == null) return null;
		List<GameGridInventory> grids = new List<GameGridInventory>();
		try
		{
			for (int i = 0; i < 32; i++)
			{
				GraphNodeStorage storage = MachineHelper.GetBodyStorage(machine, i);
				if (storage == null) break;
				if (storage is GameGridInventory grid) grids.Add(grid);
			}
		}
		catch (Exception ex) { ModCtx.LogWarning("读取本体存储失败: " + ex.Message); return null; }
		return grids.Count >= 2 ? grids : null;
	}

	private sealed class ZoneSet
	{
		public readonly List<GameGridInventory> Inputs = new List<GameGridInventory>();

		public readonly List<GameGridInventory> Outputs = new List<GameGridInventory>();

		public readonly List<GameGridInventory> Corrupts = new List<GameGridInventory>();

		public readonly List<GameGridInventory> Tools = new List<GameGridInventory>();

		public readonly List<GameGridInventory> Notes = new List<GameGridInventory>();

		public readonly List<GameGridInventory> All = new List<GameGridInventory>();
	}

	/// <summary>按面板角色读取本体分区：corrupt=侵蚀区、input=苗床区、output=产出区（1.2.0 三区布局也能识别）。</summary>
	private static ZoneSet ResolveZones(GameItem machine)
	{
		ZoneSet zoneSet = new ZoneSet();
		if (machine == null)
		{
			return null;
		}
		try
		{
			PixelElement element = machine.contentWindow?.childElement;
			MachineryPanelRow row = element as MachineryPanelRow;
			if (row != null)
			{
				for (int i = 0; i < 32; i++)
				{
					MachineryPanel panel = row.GetPanel(i);
					if (panel == null)
					{
						break;
					}
					string role = panel.role;
					List<GraphNodeStorage> list = new List<GraphNodeStorage>();
					try
					{
						panel.CollectStorage(list);
					}
					catch (Exception ex2)
					{
						ModCtx.LogWarning("读取面板存储失败(" + role + "): " + ex2.Message);
						continue;
					}
					foreach (GraphNodeStorage storage in list)
					{
						GameGridInventory grid = storage as GameGridInventory;
						if (grid == null)
						{
							continue;
						}
						zoneSet.All.Add(grid);
						if (role == "corrupt")
						{
							zoneSet.Corrupts.Add(grid);
						}
						else if (role == "output")
						{
							zoneSet.Outputs.Add(grid);
						}
						else if (role == "input")
						{
							zoneSet.Inputs.Add(grid);
						}
						else if (role == "tools")
						{
							zoneSet.Tools.Add(grid);
						}
						else if (role == "note")
						{
							zoneSet.Notes.Add(grid);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("读取本体分区失败: " + ex.Message);
		}
		if (zoneSet.Inputs.Count == 0 || zoneSet.Outputs.Count == 0)
		{
			List<GameGridInventory> list = ResolveGrids(machine);
			if (list != null && list.Count >= 2)
			{
				zoneSet.All.Clear();
				zoneSet.Inputs.Clear();
				zoneSet.Outputs.Clear();
				zoneSet.Corrupts.Clear();
				zoneSet.Tools.Clear();
				zoneSet.Notes.Clear();
				zoneSet.All.AddRange(list);
				// 与原版 IsNewLayout / BroodOf / OutputOf 一致，不能按网格数量平分，
				// 否则五区布局会把侵蚀区误当苗床区、触手区和便签区误当产出区。
				if (list.Count >= 3)
				{
					zoneSet.Corrupts.Add(list[0]);
					zoneSet.Inputs.Add(list[1]);
					zoneSet.Outputs.Add(list[2]);
					if (list.Count >= 4) zoneSet.Tools.Add(list[3]);
					if (list.Count >= 5) zoneSet.Notes.Add(list[4]);
				}
				else
				{
					zoneSet.Inputs.Add(list[0]);
					zoneSet.Outputs.Add(list[1]);
				}
			}
		}
		EnsureOutputAcceptsOurMilk(zoneSet);
		return (zoneSet.All.Count != 0) ? zoneSet : null;
	}

	private static readonly ConditionalWeakTable<GameGridInventory, object> PatchedOutputs = new ConditionalWeakTable<GameGridInventory, object>();

	/// <summary>原版 1.2.0 的产出区只允许它自己的乳汁；这里把本模组的乳汁也加进允许列表。</summary>
	private static void EnsureOutputAcceptsOurMilk(ZoneSet zones)
	{
		if (zones == null || zones.Outputs == null)
		{
			return;
		}
		foreach (GameGridInventory output in zones.Outputs)
		{
			if (output == null || PatchedOutputs.TryGetValue(output, out _))
			{
				continue;
			}
			try
			{
				GameInventoryFunc inventory = (GameInventoryFunc)(object)output;
				Func<GameItem, GameInventory, bool> previous = inventory.mayInventoryAddItemFunc;
				inventory.mayInventoryAddItemFunc = delegate(GameItem item, GameInventory inv)
				{
					if (item == null)
					{
						return false;
					}
					if (IsProductItem(item))
					{
						return true;
					}
					return previous != null && previous(item, inv);
				};
				PatchedOutputs.Add(output, new object());
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("扩展产出区允许列表失败: " + ex.Message);
			}
		}
	}

	public static bool IsProductItem(GameItem item)
	{
		string identifier = item.identifier;
		return identifier == "tentacle_semen" || identifier == "milk_wondrous" || identifier == "milk_rich" || identifier == "milk_diluted" || identifier == "wondrous_milk" || identifier == ItemFactory.BarrelSemenId || identifier == ItemFactory.BarrelMilkWondrousId || identifier == ItemFactory.BarrelMilkRichId || identifier == ItemFactory.BarrelMilkDilutedId;
	}

	/// <summary>只读取入夜状态；透支时跳过完整原版角色区段，孕期也暂停。</summary>
	internal static bool BeginMaidenSettlement(GameItem maiden, GameGridInventory output)
	{
		PrepareNightOutput(output);
		if (SettlementRunning && !SettledMaidens.Add(maiden)) return false;
		if (MaidenFatigue.ShouldRest(maiden))
		{
			MaidenFatigue.RecoverExhausted(maiden);
			EventDescriptions.Add("【透支休养】" + MaidenFatigue.Name(maiden) + "：本夜活动、双方产出与孕期暂停，疲劳恢复 5 点。");
			return false;
		}
		MaidenStates[maiden] = new NightlyMaidenState(maiden);
		ReportMaids++;
		return true;
	}

	/// <summary>原版活动、孕育、产出之后追加乳汁和疲劳，不回写任何原版活动计数。</summary>
	internal static void AppendMaidenSettlement(GameItem maiden, GameGridInventory output)
	{
		if (!MaidenStates.TryGetValue(maiden, out NightlyMaidenState state)) return;
		MaidenStates.Remove(maiden);
		try
		{
			int lactation = NativeApi.GetLactation(maiden);
			if (lactation >= 1 || NativeApi.IsPregnant(maiden)) TryProduceMilk(output, maiden, lactation);
			state.Complete(maiden);
			MaidenPrefs.Assign(maiden);
			ItemFactory.UpdateMaidenValue(maiden);
			ItemFactory.UpdateMaidenDescription(maiden);
		}
		catch (Exception ex) { ModCtx.LogWarning("追加角色结算失败: " + ex.Message); }
	}

	private static int TryProduceMilk(GameGridInventory output, GameItem maiden, int lactationLevel)
	{
		if (output == null || maiden == null)
		{
			return 0;
		}
		try
		{
			string breast = ItemFactory.GetStr(maiden, "MAIDEN_BREAST");
			string itemId;
			float multiplier;
			float productionChance;
			switch (breast)
			{
			case "贫乳":
				itemId = "milk_rich";
				multiplier = 3f;
				productionChance = 0.3f;
				break;
			case "巨乳":
				itemId = "milk_diluted";
				multiplier = 1f;
				productionChance = 0.7f;
				break;
			default:
				itemId = "milk_wondrous";
				multiplier = 1.6f;
				productionChance = 0.5f;
				break;
			}
			// 泌乳等级从 Lv.2 开始每级 +30% 概率；超过 100% 时按“固定一瓶 + 余数概率多一瓶”处理
			float chance = productionChance + (float)Mathf.Max(0, lactationLevel - 1) * 0.30f;
			int guaranteed = Mathf.FloorToInt(chance);
			float extraChance = chance - guaranteed;
			int bottles = guaranteed + (Random.value < extraChance ? 1 : 0);
			if (bottles <= 0)
			{
				return 0;
			}
			// 泌乳等级不再提高单价：始终按该乳汁的最低档价格产出
			long value = Mathf.RoundToInt(50f * multiplier);
			int made = 0;
			for (int i = 0; i < bottles; i++)
			{
				if (!TryProduceMilkItem(output, itemId, value))
				{
					break;
				}
				RecordNightProduct(itemId);
				made++;
			}
			return made;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("产出乳汁失败: " + ex.Message);
			return 0;
		}
	}

	private static bool TryProduceMilkItem(GameGridInventory output, string itemId, long value)
	{
		if (output == null) return false;
		GameItem source = null;
		GameItem bottle = null;
		int before = 0;
		bool barrelSource = false;
		bool produced = false;
		try
		{
			foreach (GameItem item in output.childItems)
			{
				if (item != null && item.identifier == "wondrous_milk" && item.unitCount > 0)
				{
					source = item;
					before = item.unitCount;
					break;
				}
			}
			if (source == null)
			{
				foreach (GameItem barrel in FindBarrelItems(output, ItemFactory.BarrelMilkWondrousId))
				{
					before = ItemFactory.GetBarrelUses(barrel);
					if (before <= 0) continue;
					source = barrel;
					barrelSource = true;
					break;
				}
			}
			// 先扣除以释放空间。暂存最后一瓶/空桶的原对象，放置失败时原位恢复。
			if (source != null)
			{
				if (barrelSource) ItemFactory.SetBarrelUses(source, before - 1);
				else if (before > 1) source.SetUnitCount(before - 1);
				if (before == 1 && (!barrelSource || itemId != "milk_wondrous") && !output.Expel(source))
					throw new InvalidOperationException("无法移出待扣除乳汁");
			}
			string barrelId = itemId == "milk_rich" ? ItemFactory.BarrelMilkRichId
				: itemId == "milk_diluted" ? ItemFactory.BarrelMilkDilutedId : ItemFactory.BarrelMilkWondrousId;
			foreach (GameItem barrel in FindBarrelItems(output, barrelId))
			{
				if (ItemFactory.AddBarrelUses(barrel, 1) != 1) continue;
				produced = true;
				return true;
			}
			bottle = DirectoryMaster.Item(itemId, true);
			if (bottle == null) return false;
			bottle.unitValue = value;
			produced = GraphUtils.TryAcceptAll(output, bottle, -1) == 0 && bottle.parentInventory != null;
			return produced;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("产出乳汁失败: " + ex.Message);
			return false;
		}
		finally
		{
			if (bottle != null && bottle.parentInventory == null) bottle.Destroy();
			if (source != null)
			{
				if (!produced)
				{
					if (barrelSource) ItemFactory.SetBarrelUses(source, before);
					else source.SetUnitCount(before);
					// Expel 保留对象原来的格位数据；回退不重新搜索空位。
					if (source.parentInventory == null) output.UncheckedAccept(source);
				}
				else if (source.parentInventory == null || (barrelSource && ItemFactory.GetBarrelUses(source) == 0)) source.Destroy();
			}
		}
	}

	/// <summary>清理多余的触手本体：只删除“空的”多余本体，避免连里面的苗床一起删掉。</summary>
	public static int DeduplicateBodies()
	{
		try
		{
			EmporiumEntry entry = EmporiumEntry.Instance;
			if (entry == null)
			{
				return 0;
			}
			List<GameItem> list = new List<GameItem>();
			foreach (GameItem allItem in entry.GetAllItems())
			{
				if (allItem != null && allItem.IsTag("TENTACLE_BODY_TAG"))
				{
					list.Add(allItem);
				}
			}
			if (list.Count <= 1)
			{
				return 0;
			}
			int num = 0;
			for (int i = 1; i < list.Count; i++)
			{
				GameItem gameItem = list[i];
				try
				{
					List<GameItem> list2 = GraphUtils.FindAllChildrenType<GameItem>(gameItem);
					if (list2 != null && list2.Count > 0)
					{
						ModCtx.LogWarning("多余的触手本体里还有 " + list2.Count + " 件物品，跳过不删: " + gameItem.name);
						continue;
					}
					gameItem.Destroy();
					Instances.Remove(gameItem);
					num++;
				}
				catch (Exception ex)
				{
					ModCtx.LogWarning("清理多余触手本体失败: " + ex.Message);
				}
			}
			if (num > 0)
			{
				ModCtx.Log("清理多余触手本体: " + num + " 件");
			}
			return num;
		}
		catch (Exception ex2)
		{
			ModCtx.LogWarning("清理触手本体异常: " + ex2.Message);
			return 0;
		}
	}

	internal static List<GameGridInventory> GetNoteZones(GameItem body) =>
		ResolveZones(body)?.Notes ?? new List<GameGridInventory>();

	public static void ResetCache()
	{
		AbortOriginalSettlement();
		Instances.Clear();
		ModCtx.Log("触手本体缓存已重置");
	}

	private static bool IsBodyItem(GameItem item)
	{
		return item != null && (item.identifier == "tentacle_body" || item.IsTag("TENTACLE_BODY_TAG"));
	}

	public static int GetExpNeed() => NativeApi.GetBodyExpNeed();

	public static int GetBodyLevel()
	{
		// 本地部分前置版本的 API 仍封顶 5；保留原版存储的更高等级，不限制后续版本。
		int stored = GetMigratedInt("body_lv", 1);
		return Mathf.Max(1, Mathf.Max(stored, NativeApi.GetBodyLevel()));
	}

	public static int GetBodyExp()
	{
		GetMigratedInt("body_exp", 0);
		return NativeApi.GetBodyExp();
	}

	/// <summary>新 ID 下没有数据时，从旧 ID（原版 / 旧魔改版）迁移一次。</summary>
	private static int GetMigratedInt(string key, int fallback)
	{
		try
		{
			// 本体属于原版：等级/经验以原版的键为准
			int num2 = ModHelper.GetModDataInt("lewd_meme", key, int.MinValue);
			if (num2 != int.MinValue)
			{
				return num2;
			}
			int num = ModHelper.GetModDataInt("lewd_meme_remake", key, int.MinValue);
			if (num != int.MinValue)
			{
				ModHelper.SetModDataInt("lewd_meme", key, num);
				ModCtx.Log("已把本模组的 " + key + "=" + num + " 迁移到原版数据键");
				return num;
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("读取继承数据失败: " + ex.Message);
		}
		return fallback;
	}

	public static void DebugAddExp(int amount)
	{
		GetBodyLevel();
		GetBodyExp();
		OriginalMod::MidnightPawn.Game.TentacleBody.DebugAddExp(amount);
	}

}
