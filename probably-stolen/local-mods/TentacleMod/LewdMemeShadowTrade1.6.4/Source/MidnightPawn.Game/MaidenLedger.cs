extern alias OriginalMod;
using NativeApi = OriginalMod::MidnightPawn.Api.TentacleApi;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MidnightPawn.Game;

/// <summary>
/// 苗床名册：开局随触手天赋下发，记录每一次卖出的苗床。
/// 档案按存档槽存进 ModData（不再依赖物品，物品丢了也能补发），
/// 打开时按数据变化更新书页：第一页汇总，之后每页 5 条，左侧立绘 + 成交价，右侧详细数据。
/// </summary>
public static class MaidenLedger
{
	public const string MOD_ID = "lewd_meme_remake";

	public const string BOOK_ID = "maiden_ledger";

	public const string ITEM_ID = "maiden_ledger_item";

	public const string FULL_ID = "lewd_meme_remake:maiden_ledger";

	public const string PERK_ID = "lewd_meme_remake.tentacle_origin";

	private const int MaxRecords = 100;

	private const int RecordsPerPage = 5;

	private const float DesignW = 314f;

	private const float DesignH = 402f;

	private const string HEADER = "v1";

	private const string DATA_KEY = "ledger_slot_";
	private static string renderedData;

	public static void EnsureRegistered()
	{
		try
		{
			PaperChromeIcons.EnsureInstalled();
			if (PaperBookRegistry.Get(FULL_ID)?.data != null) return;
			// 注册工厂只需书籍定义；成交历史等到打开时再构建。
			PaperBookData data = BuildBook("");
			if (data != null)
			{
				PaperBookRegistry.Register(MOD_ID, data);
				renderedData = null;
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("苗床名册注册失败: " + ex.Message);
		}
	}

	private static bool pendingCheck;

	/// <summary>读档时打标记，等第一次开卷帘门再检查，避免拖慢读档。</summary>
	public static void MarkPendingCheck()
	{
		pendingCheck = true;
		renderedData = null;
	}

	/// <summary>开店时触发：读档后第一次开业才检查并补发。</summary>
	public static void OnShutterOpened()
	{
		if (!pendingCheck)
		{
			return;
		}
		pendingCheck = false;
		EnsureItemAfterLoad();
	}

	public static void Refresh()
	{
		renderedData = null;
		UpdateBook();
	}

	public static void OpenBook()
	{
		UpdateBook();
		PaperBookRegistry.Open(FULL_ID);
	}

	private static void UpdateBook()
	{
		EnsureRegistered();
		PaperBookRegistry.Entry entry = PaperBookRegistry.Get(FULL_ID);
		if (entry == null) return;
		string raw = ReadRawData();
		if (string.Equals(raw, renderedData, StringComparison.Ordinal)) return;
		PaperBookData data = BuildBook(raw);
		PaperJson.Normalize(data);
		// 只更新内容，避免重新注册书籍时覆盖自有的薄工厂及打开回调。
		entry.data = data;
		if (entry.open != null && entry.open.IsAlive)
		{
			entry.open.ReplaceData(data);
			entry.open.Rebuild();
		}
		renderedData = raw;
	}

	internal static GameItem CreateItem()
	{
		EnsureRegistered();
		PaperBookRegistry.Entry entry = PaperBookRegistry.Get(FULL_ID);
		if (entry == null) return null;
		GameItem item = PaperBookRegistry.CreateItem(entry);
		RepairLedgerItem(item);
		return item;
	}

	public static GameItem SpawnLedger()
	{
		return SpawnLedger(false);
	}

	/// <summary>生成日志。onCounter=true 时放到柜台外侧（顾客放货区），否则放进背包。</summary>
	public static GameItem SpawnLedger(bool onCounter)
	{
		try
		{
			EnsureRegistered();
			GameItem val = ModHelper.SpawnPaperItem(MOD_ID, BOOK_ID);
			if (val == null)
			{
				ModCtx.LogWarning("苗床名册下发失败：找不到物品 " + ITEM_ID);
				return null;
			}
			ModHelper.SetItemOwnerShip(val, true);
			bool flag = false;
			if (onCounter)
			{
				try
				{
					EmporiumEntry instance = EmporiumEntry.Instance;
					if (instance != null && instance.backInvinvElement != null && instance.backInvinvElement.CanTakeAnything())
					{
						PlayerStore.Instance.AddDirectToWeightedTable(val, true);
						flag = (FindLedgerItem() != null);
					}
				}
				catch (Exception ex)
				{
					ModCtx.LogWarning("苗床名册放到柜台失败: " + ex.Message);
				}
			}
			if (!flag)
			{
				if (!NativeApi.DeliverItem(val, "苗床名册")) return null;
			}
			ModCtx.Log(flag ? "苗床名册已放到柜台外侧" : "苗床名册已下发到背包");
			if (FindLedgerItem() == null)
			{
				ModCtx.LogWarning("苗床名册生成后未能在场上找到（可能是背包与柜台都满了），可用调试按钮「生成苗床名册」重试");
			}
			return val;
		}
		catch (Exception ex2)
		{
			ModCtx.LogWarning("苗床名册下发失败: " + ex2.Message);
			return null;
		}
	}

	/// <summary>补全日志物品：图标、DOCUMENT 标签、可读/可激活函数。用于修复旧版本发出但打不开的日志。</summary>
	private static void RepairLedgerItem(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			item.name = "苗床名册";
			if (string.IsNullOrEmpty(item.shortDescription))
			{
				item.shortDescription = "黑色封皮的册子。每一名被你卖出去的苗床，都留了一页在这里。";
			}
			item.unitValue = 0L;
			item.SetSpriteAndShapeFromMod(MOD_ID, "namelist");
			item.SetShape(ItemFactory.RectShape(2, 3));
			item.EnableTag("DOCUMENT", false);
			item.SetGameItemType("DOCUMENT");
			item.EnableTag("IMPORTANT_TAG", false);
			item.canActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
			item.mayActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
			item.onActivateSlotItemFunc = (it, inv, slot) => OpenBook();
			item.onLoaded = RepairLedgerItem;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("修复苗床名册物品失败: " + ex.Message);
		}
	}

	/// <summary>调试：把场上日志物品的图标与占位重新套一遍。</summary>
	public static void RepairExisting()
	{
		RepairExisting(false);
	}

	/// <summary>静默版：读档/开门时用，不弹提示。</summary>
	public static void RepairExistingQuiet()
	{
		RepairExisting(true);
	}

	public static void RepairExisting(bool silent)
	{
		try
		{
			GameItem val = FindLedgerItem();
			if (val == null)
			{
				if (!silent)
				{
					Toast.Show("没有找到苗床名册物品", "苗床名册");
				}
				return;
			}
			RepairLedgerItem(val);
			if (!silent)
			{
				Toast.Show("苗床名册外观已修复（namelist 图标）", "苗床名册");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("修复苗床名册外观失败: " + ex.Message);
		}
	}

	/// <summary>读档后：检测是否持有日志，缺了就补发（老存档没有这个文档）。</summary>
	public static void EnsureItemAfterLoad()
	{
		try
		{
			GameItem val = FindLedgerItem();
			if (val != null)
			{
				return;
			}
			bool flag = Perks.IsPerkActive();
			bool flag2 = HasTentacleBody();
			string text = ModHelper.GetModData(MOD_ID, DataKey(), "");
			bool flag3 = !string.IsNullOrEmpty(text);
			ModCtx.Log($"苗床名册检查: 天赋={flag} 触手本体={flag2} 已有档案={flag3}");
			if (!flag && !flag2 && !flag3)
			{
				return;
			}
			if (SpawnLedger(true) != null)
			{
				ModCtx.Log("苗床名册已补发到柜台外侧");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("补发苗床名册失败: " + ex.Message);
		}
	}

	/// <summary>场上（背包 / 店内 / 容器里）是否已有触手本体，用来判断老存档是否应该补发日志。</summary>
	private static bool HasTentacleBody()
	{
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if (instance == null)
			{
				return false;
			}
			List<GameItem> allItems = instance.GetAllItems();
			if (allItems == null)
			{
				return false;
			}
			for (int i = 0; i < allItems.Count; i++)
			{
				GameItem val = allItems[i];
				if (val != null && (val.identifier == "tentacle_body" || val.IsTag("TENTACLE_BODY_TAG")))
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("查找触手本体失败: " + ex.Message);
		}
		return false;
	}

	public static void RecordSale(GameItem maiden, StoreClient client, int reqCount, int matchedCount, bool factionMatched, bool allMatch, long baseValue, long payout)
	{
		if (maiden == null)
		{
			return;
		}
		try
		{
			string raw = ReadRawData();
			ParseData(raw, out int count, out long totalPayout, out long totalBase, out int match, out List<string> records);
			count++;
			totalPayout += payout;
			totalBase += baseValue;
			if (allMatch)
			{
				match++;
			}
			records.Add(BuildRecord(maiden, client, reqCount, matchedCount, factionMatched, allMatch, baseValue, payout));
			while (records.Count > MaxRecords)
			{
				records.RemoveAt(0);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(HEADER).Append('|').Append(count).Append('|').Append(totalPayout)
				.Append('|')
				.Append(totalBase)
				.Append('|')
				.Append(match);
			for (int i = 0; i < records.Count; i++)
			{
				stringBuilder.Append('\n').Append(records[i]);
			}
			ModHelper.SetModData(MOD_ID, DataKey(), stringBuilder.ToString());
			// 关闭时仅保存记录；正在阅读时保持当前界面实时更新。
			if (PaperBookRegistry.IsOpen(FULL_ID)) Refresh();
			ModCtx.Log($"苗床名册: 记录 {MaidenFatigue.Name(maiden)}（累计 {count} 名 / {totalPayout} 信用点）");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("苗床名册记录失败: " + ex.Message);
		}
	}

	private static PaperBookData BuildBook(string raw)
	{
		PaperBookData data = null;
		try
		{
			data = PaperStarters.NewBook("blank", BOOK_ID, "苗床名册");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("苗床名册模板读取失败，使用默认外观: " + ex.Message);
		}
		if (data == null)
		{
			data = new PaperBookData
			{
				id = BOOK_ID,
				title = "苗床名册",
				background = "notebook_manual"
			};
			data.chrome = PaperStarters.DefaultChrome(new Vector2(DesignW, DesignH), 2f);
		}
		data.id = BOOK_ID;
		data.title = "苗床名册";
		data.designWidth = DesignW;
		data.designHeight = DesignH;
		data.item = new PaperItemData
		{
			id = ITEM_ID,
			name = "苗床名册",
			description = "黑色封皮的册子。每一名被你卖出去的苗床，都留了一页在这里。",
			sprite = "namelist",
			value = 0L,
			important = true
		};
		ParseData(raw, out int count, out long payout, out long baseValue, out int match, out List<string> records);
		if (data.pages == null)
		{
			data.pages = new List<PaperPageData>();
		}
		data.pages.Clear();
		AddSummaryPage(data, count, payout, baseValue, match, records);
		AddRecordPages(data, records);
		return data;
	}

	private static string ReadRawData()
	{
		try
		{
			string text = ModHelper.GetModData(MOD_ID, DataKey(), "");
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			string legacy = ModHelper.GetModData("lewd_meme", DataKey(), "");
			if (!string.IsNullOrEmpty(legacy))
			{
				ModHelper.SetModData(MOD_ID, DataKey(), legacy);
				ModCtx.Log("已从旧版本迁移苗床名册档案");
				return legacy;
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("读取苗床名册档案失败: " + ex.Message);
		}
		return "";
	}

	private static void ParseData(string raw, out int count, out long payout, out long baseValue, out int match, out List<string> records)
	{
		count = 0;
		payout = 0L;
		baseValue = 0L;
		match = 0;
		records = new List<string>();
		if (string.IsNullOrEmpty(raw))
		{
			return;
		}
		string[] array = raw.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			return;
		}
		int num = 0;
		if (array[0].StartsWith(HEADER + "|", StringComparison.Ordinal))
		{
			string[] array2 = array[0].Split('|');
			if (array2.Length >= 5)
			{
				count = ParseInt(array2[1]);
				payout = ParseLong(array2[2]);
				baseValue = ParseLong(array2[3]);
				match = ParseInt(array2[4]);
			}
			num = 1;
		}
		for (int i = num; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				records.Add(array[i]);
			}
		}
	}

	private static void AddSummaryPage(PaperBookData data, int count, long payout, long baseValue, int match, List<string> records)
	{
		PaperPageData page = new PaperPageData
		{
			name = "汇总"
		};
		page.elements.Add(Text(18f, 24f, 278f, 30f, "苗床名册", "title"));
		page.elements.Add(Line(18f, 58f, 296f, 58f, 1f, "c9b79a"));
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("已售出：").Append(count).Append(" 名\n");
		stringBuilder.Append("累计收入：").Append(payout).Append(" 信用点\n");
		stringBuilder.Append("累计基础价值：").Append(baseValue).Append(" 信用点\n");
		long num = ((count > 0) ? (payout / count) : 0);
		stringBuilder.Append("平均成交价：").Append(num).Append(" 信用点 / 名\n");
		int num2 = ((count > 0) ? Mathf.RoundToInt(100f * (float)match / (float)count) : 0);
		stringBuilder.Append("符合要求：").Append(match).Append(" 次（").Append(num2).Append("%）\n");
		if (records.Count > 0)
		{
			string[] array = records[records.Count - 1].Split('|');
			if (array.Length >= 19)
			{
				stringBuilder.Append("\n最近成交：第 ").Append(array[0]).Append(" 天 · ").Append(array[1])
					.Append(" → ")
					.Append(array[18])
					.Append(" 信用点");
			}
		}
		stringBuilder.Append("\n\n每页记录 ").Append(RecordsPerPage).Append(" 名已售出的苗床：\n左侧为立绘与成交价，右侧为详细数据。\n用下方箭头翻页。");
		page.elements.Add(Text(18f, 70f, 278f, 240f, stringBuilder.ToString(), "body"));
		data.pages.Add(page);
	}

	private static void AddRecordPages(PaperBookData data, List<string> records)
	{
		if (records.Count == 0)
		{
			PaperPageData page = new PaperPageData
			{
				name = "记录"
			};
			page.elements.Add(Text(18f, 24f, 278f, 30f, "成交记录", "title"));
			page.elements.Add(Line(18f, 58f, 296f, 58f, 1f, "c9b79a"));
			page.elements.Add(Text(18f, 72f, 278f, 200f, "还没有卖出去过任何苗床。\n\n把她们交给收购商之后，这里会留下每一笔记录。", "body"));
			data.pages.Add(page);
			return;
		}
		int count = records.Count;
		int num = (count + RecordsPerPage - 1) / RecordsPerPage;
		for (int i = 0; i < num; i++)
		{
			int num2 = count - 1 - i * RecordsPerPage;
			PaperPageData page2 = new PaperPageData
			{
				name = "记录 " + (i + 1)
			};
			string text = $"成交记录（{i * RecordsPerPage + 1}–{Mathf.Min(count, (i + 1) * RecordsPerPage)} / {count}）";
			page2.elements.Add(Text(14f, 6f, 286f, 20f, text, "section"));
			for (int j = 0; j < RecordsPerPage; j++)
			{
				int num3 = num2 - j;
				if (num3 < 0)
				{
					break;
				}
				AddRecordRow(page2, records[num3], 26f + (float)j * 62f);
			}
			data.pages.Add(page2);
		}
	}

	private static void AddRecordRow(PaperPageData page, string record, float y)
	{
		string[] array = record.Split('|');
		if (array.Length < 19)
		{
			return;
		}
		string text = PortraitKey((array.Length >= 20) ? array[19] : null);
		if (!string.IsNullOrEmpty(text))
		{
			page.elements.Add(Image(8f, y + 1f, 50f, 50f, text));
		}
		page.elements.Add(Centered(8f, y + 51f, 50f, 10f, PriceText(array), "label"));
		page.elements.Add(Text(64f, y, 240f, 58f, DescribeData(array), "body"));
		page.elements.Add(Line(8f, y + 61f, 304f, y + 61f, 1f, "c9b79a"));
	}

	private static string DescribeData(string[] array)
	{
		int num = ParseInt(array[15]);
		bool flag = ParseInt(array[16]) != 0;
		int num2 = ((array.Length >= 21) ? ParseInt(array[20]) : (flag ? num : 0));
		bool flag2 = array.Length >= 22 && ParseInt(array[21]) != 0;
		bool extraPurchase = num == 0 && !flag;
		double num3 = extraPurchase ? 1.0 : MaidenBuyer.PayoutMultiplier(num, num2, flag2, flag);
		int num4 = ParseInt(array[12]);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("第 ").Append(array[0]).Append(" 天 · ").Append(array[1])
			.Append("（")
			.Append(array[2])
			.Append(" / ")
			.Append(array[3])
			.Append("）\n");
		stringBuilder.Append("体型 ").Append(array[4]).Append(" · 乳量 ").Append(array[5])
			.Append(" · ")
			.Append(array[6])
			.Append(" · 调教 ")
			.Append(array[7])
			.Append('\n');
		stringBuilder.Append("性交 ").Append(array[8]).Append(" · 口交 ").Append(array[9])
			.Append(" · 肛交 ")
			.Append(array[10])
			.Append(" · 泌乳 ")
			.Append(array[11]);
		if (num4 > 0)
		{
			stringBuilder.Append(" · 孕 ").Append(num4).Append('/').Append(TentacleBody.PREGNANCY_DAYS);
		}
		stringBuilder.Append(" · 疲劳 ").Append(array[13]).Append('\n');
		if (extraPurchase)
		{
			stringBuilder.Append(array[14]).Append(" ｜ 追加收购 · 基础估值 ｜ ×1");
		}
		else
		{
			stringBuilder.Append(array[14]).Append(" ｜ 满足 ").Append(num2).Append('/').Append(num).Append(" 条 ｜ ×")
				.Append(num3.ToString("0.##"));
		}
		return stringBuilder.ToString();
	}

	private static string PriceText(string[] array)
	{
		return ParseLong(array[18]).ToString() + "$";
	}

	/// <summary>把少女立绘登记成纸面图，返回 PaperAssets 能识别的 key。</summary>
	private static string PortraitKey(string source)
	{
		if (string.IsNullOrEmpty(source))
		{
			return null;
		}
		try
		{
			LoadedMod val = SpriteInjector.MyMod();
			if (val == null)
			{
				return null;
			}
			string text = "ledger_" + SanitizeKey(source);
			if (val.PaperSprites == null)
			{
				val.PaperSprites = new Dictionary<string, Sprite>();
			}
			if (val.PaperSprites.TryGetValue(text, out var value) && value != null)
			{
				return text;
			}
			string text3 = SpriteInjector.InjectPortraitThumb(source, 128, text);
			if (!string.IsNullOrEmpty(text3) && val.Sprites != null && val.Sprites.TryGetValue(text3, out var value3) && value3 != null)
			{
				val.PaperSprites[text] = value3;
				return text;
			}
			Sprite val2 = SpriteInjector.ResolveSprite(source);
			if (val2 == null)
			{
				return null;
			}
			string text2 = SpriteInjector.InjectResizedSprite(val2, 96, 96, text);
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			if (val.Sprites != null && val.Sprites.TryGetValue(text2, out var value2) && value2 != null)
			{
				val.PaperSprites[text] = value2;
				return text;
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("日志立绘生成失败: " + ex.Message);
		}
		return null;
	}

	private static string BuildRecord(GameItem maiden, StoreClient client, int reqCount, int matchedCount, bool factionMatched, bool allMatch, long baseValue, long payout)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(MaidenFatigue.Today()).Append('|');
		stringBuilder.Append(Sanitize(MaidenFatigue.Name(maiden))).Append('|');
		stringBuilder.Append(Sanitize(TagStr(maiden, "MAIDEN_FACTION"))).Append('|');
		stringBuilder.Append(Sanitize(TagStr(maiden, "MAIDEN_JOB"))).Append('|');
		stringBuilder.Append(Sanitize(TagStr(maiden, "MAIDEN_BODY"))).Append('|');
		stringBuilder.Append(Sanitize(TagStr(maiden, "MAIDEN_BREAST"))).Append('|');
		stringBuilder.Append(Sanitize(VirginState(maiden))).Append('|');
		stringBuilder.Append(ItemFactory.GetInt(maiden, "TRAIN_VALUE")).Append('|');
		stringBuilder.Append(ItemFactory.GetInt(maiden, "SEX_COUNT")).Append('|');
		stringBuilder.Append(ItemFactory.GetInt(maiden, "ORAL_COUNT")).Append('|');
		stringBuilder.Append(ItemFactory.GetInt(maiden, "ANAL_COUNT")).Append('|');
		stringBuilder.Append(ItemFactory.GetInt(maiden, "LACTATION_LV")).Append('|');
		int num = (maiden.IsTag("MAIDEN_PREGNANT") ? ItemFactory.GetInt(maiden, "MAIDEN_PREGNANT_DAYS") : 0);
		stringBuilder.Append(num).Append('|');
		stringBuilder.Append(MaidenFatigue.Get(maiden)).Append('|');
		string text = ((client != null && !string.IsNullOrEmpty(client.displayName)) ? client.displayName : "收购商");
		stringBuilder.Append(Sanitize(text)).Append('|');
		stringBuilder.Append(reqCount).Append('|');
		stringBuilder.Append(allMatch ? 1 : 0).Append('|');
		stringBuilder.Append(baseValue).Append('|');
		stringBuilder.Append(payout).Append('|');
		stringBuilder.Append(Sanitize(ItemFactory.GetStr(maiden, "MAIDEN_SPRITE_SRC"))).Append('|');
		stringBuilder.Append(matchedCount).Append('|');
		stringBuilder.Append(factionMatched ? 1 : 0);
		return stringBuilder.ToString();
	}

	private static string DataKey()
	{
		int num = -1;
		try
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance != null)
			{
				num = instance.saveSlotId;
			}
		}
		catch
		{
		}
		return DATA_KEY + num;
	}

	private static PaperElementData Text(float x, float y, float w, float h, string text, string style)
	{
		return new PaperElementData
		{
			type = "text",
			x = x,
			y = y,
			w = w,
			h = h,
			text = text,
			style = style,
			align = "left",
			valign = "top",
			wrap = true
		};
	}

	private static PaperElementData Centered(float x, float y, float w, float h, string text, string style)
	{
		return new PaperElementData
		{
			type = "text",
			x = x,
			y = y,
			w = w,
			h = h,
			text = text,
			style = style,
			align = "center",
			valign = "middle",
			wrap = false
		};
	}

	private static PaperElementData Image(float x, float y, float w, float h, string sprite)
	{
		return new PaperElementData
		{
			type = "image",
			x = x,
			y = y,
			w = w,
			h = h,
			sprite = sprite,
			fit = "stretch",
			scale = 1f
		};
	}

	private static PaperElementData Line(float x, float y, float x2, float y2, float thickness, string color)
	{
		return new PaperElementData
		{
			type = "line",
			x = x,
			y = y,
			x2 = x2,
			y2 = y2,
			thickness = thickness,
			color = color
		};
	}

	private static GameItem FindLedgerItem()
	{
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if (instance == null)
			{
				return null;
			}
			List<GameItem> allItems = instance.GetAllItems();
			if (allItems == null)
			{
				return null;
			}
			for (int i = 0; i < allItems.Count; i++)
			{
				GameItem val = allItems[i];
				if (val != null && val.identifier == ITEM_ID)
				{
					return val;
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("查找苗床名册失败: " + ex.Message);
		}
		return null;
	}

	private static string TagStr(GameItem item, string tag)
	{
		string text = ItemFactory.GetStr(item, tag);
		return string.IsNullOrEmpty(text) ? "未知" : text;
	}

	private static string VirginState(GameItem item)
	{
		bool flag = item.IsTag("MAIDEN_VIRGIN");
		bool flag2 = item.IsTag("MAIDEN_ANAL_VIRGIN");
		if (flag && flag2)
		{
			return "双处女";
		}
		if (flag)
		{
			return "处女";
		}
		if (flag2)
		{
			return "肛门处女";
		}
		return "非处女";
	}

	private static string Sanitize(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		return text.Replace('|', '/').Replace('\n', ' ').Replace('\r', ' ');
	}

	private static string SanitizeKey(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		for (int i = 0; i < text.Length && stringBuilder.Length < 64; i++)
		{
			char c = text[i];
			bool flag = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_';
			stringBuilder.Append(flag ? c : '_');
		}
		return stringBuilder.ToString();
	}

	private static int ParseInt(string text)
	{
		if (int.TryParse(text, out var result))
		{
			return result;
		}
		return 0;
	}

	private static long ParseLong(string text)
	{
		if (long.TryParse(text, out var result))
		{
			return result;
		}
		return 0L;
	}
}
