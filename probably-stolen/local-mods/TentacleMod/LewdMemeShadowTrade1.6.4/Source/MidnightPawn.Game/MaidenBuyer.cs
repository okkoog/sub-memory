using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MidnightPawn.Game;

public static class MaidenBuyer
{
	private const string LastBuyerDayKey = "last_buyer_day";

	private const string BuyerCursorKey = "buyer_cursor";

	private const string TagBuyer = "MAIDEN_BUYER";

	private const string PrefPrefix = "MAIDEN_BUYER_PREF|";

	private static readonly string[] MilkIds = new string[4] { "milk_wondrous", "milk_rich", "milk_diluted", "wondrous_milk" };

	private static readonly string[] BodyNames = new string[5] { "御姐", "少妇", "熟女", "少女", "萝莉" };

	private static readonly int[] BodyWeights = new int[5] { 25, 25, 25, 25, 25 };

	private sealed class Check
	{
		public string Label;

		public string Expected;

		public string Actual;

		public bool Pass;
	}

	public static void OnGenerateCustomerVeryLate(StoreClientManager manager)
	{
		try
		{
			if (manager == null || PlayerStore.Instance == null || EmporiumEntry.Instance == null)
			{
				return;
			}
			int today = StoreStation.Instance?.dayCounter ?? (-1);
			if (today < 0)
			{
				return;
			}
			int limit = ModSettings.BuyerPerDay;
			bool sameDay = ModHelper.GetModDataInt("lewd_meme_remake", LastBuyerDayKey, -1) == today;
			int cursor = (sameDay ? ModHelper.GetModDataInt("lewd_meme_remake", BuyerCursorKey, 0) : 0);
			if (cursor >= limit)
			{
				return;
			}
			int milkKinds = CountShowcaseMilkKinds();
			int chance = milkKinds switch
			{
				3 => 100,
				2 => 66,
				1 => 33,
				_ => 0,
			};
			if (chance <= 0)
			{
				ModHelper.SetModDataInt("lewd_meme_remake", LastBuyerDayKey, today);
				ModHelper.SetModDataInt("lewd_meme_remake", BuyerCursorKey, limit);
				ModCtx.Log("收购商：展示柜没有乳汁，今天不来");
				return;
			}
			int num = 0;
			for (int i = cursor; i < limit; i++)
			{
				if (!RNG.Roll(chance))
				{
					ModCtx.Log($"收购商 {i + 1}/{limit}：展示柜有 {milkKinds} 种乳汁，到店判定 {chance}% 未通过");
					continue;
				}
				StoreClient client = CreateBuyer(today, i);
				if (client != null)
				{
					manager.AddClient(client);
					num++;
					ModCtx.Log($"收购商 {i + 1}/{limit} 已加入队列: " + client.displayName + $"（展示柜乳汁 {milkKinds} 种，概率 {chance}%）");
				}
			}
			ModHelper.SetModDataInt("lewd_meme_remake", LastBuyerDayKey, today);
			ModHelper.SetModDataInt("lewd_meme_remake", BuyerCursorKey, limit);
			if (num == 0)
			{
				ModCtx.Log($"收购商：今天 {limit} 个名额全部未通过判定");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("生成收购商失败: " + ex.Message);
		}
	}

	public static void DebugSpawn()
	{
		try
		{
			if (PlayerStore.Instance?.storeClientManager == null)
			{
				ModCtx.LogWarning("调试生成收购商失败：不在营业场景");
				return;
			}
			int today = StoreStation.Instance?.dayCounter ?? 0;
			StoreClient client = CreateBuyer(today, UnityEngine.Random.Range(1000, 9999));
			if (client != null)
			{
				PlayerStore.Instance.storeClientManager.AddClient(client);
				ModCtx.Log("调试收购商已加入队列: " + client.displayName);
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("调试生成收购商失败: " + ex.Message);
		}
	}

	private static StoreClient CreateBuyer(int today, int index)
	{
		string buyerFaction = PickBuyerFaction();
		string displayName = MaidenBuyerText.BuyerDisplayName(buyerFaction);
		StoreClient client = new StoreClient();
		client.identifier = "maiden_buyer_" + today + "_" + index;
		client.displayName = displayName;
		client.sourceModId = "lewd_meme_remake";
		client.clientFaction = buyerFaction;
		client.spriteName = PickBuyerSprite(buyerFaction);
		client.clientIntent = StoreClient.ClientIntent.BUY;
		client.dismissable = true;
		client.acceptRetailMarkup = false;
		client.useClientBudget = false;
		client.cooldown = 1;
		client.clientBuyingIdList.Add("maiden_lower");
		client.AddTag(TagBuyer);
		GeneratePreferences(client, buyerFaction);
		bool youngPreferred = IsYoungBodyPreferred(client);
		string intro = youngPreferred ? MaidenBuyerText.YoungIntroLine(buyerFaction) : MaidenBuyerText.IntroLine(buyerFaction);
		Dialogue node = client.mainDialogue.SetText(displayName, intro);
		foreach (string line in DemandLines(client, buyerFaction))
		{
			node = node.NextDialogue().SetText(displayName, line);
		}
		client.acceptDealDialogue = new Dialogue().SetText(displayName, DefaultDealLine(client));
		client.onItemAddedToWeighted = delegate(GameItem item)
		{
			AddDonation(item);
			RefreshAcceptLine(client, item);
		};
		client.onItemRemovedToWeighted = delegate(GameItem item)
		{
			RemoveDonation(item);
			ResetAcceptLine(client);
		};
		client.onItemBought = delegate(GameItem item, int cost)
		{
			HandleSale(client, item, cost);
		};
		client.OnArrival = delegate
		{
			Toast.Show(MaidenBuyerText.ArrivalToast(displayName, buyerFaction), MaidenBuyerText.ToastTitle);
		};
		client.CompleteClientCreation();
		return client;
	}

	private static string PickBuyerFaction()
	{
		string[] ids = new string[5] { "FACTION_BLACK_MARKET", "FACTION_UPPER_LEVEL", "FACTION_LOWER_LEVEL", "FACTION_SECURITY", "FACTION_REVOLUTION" };
		int[] weights = new int[5] { 40, 25, 15, 10, 10 };
		return ids[PickWeighted(weights)];
	}

	private static string PickBuyerSprite(string faction)
	{
		return faction switch
		{
			"FACTION_BLACK_MARKET" => SpriteDict.GetRandomShadySpriteName(),
			"FACTION_UPPER_LEVEL" => SpriteDict.GetRandomUpperSpriteName(),
			"FACTION_SECURITY" => SpriteDict.GetRandomSecSpriteName(),
			"FACTION_REVOLUTION" => SpriteDict.GetRandomRevSpriteName(),
			_ => SpriteDict.GetRandomLowerLevelSpriteName(),
		};
	}

	private static int CountShowcaseMilkKinds()
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GameItem childItem in EmporiumEntry.Instance.showcaseElement.childItems)
		{
			if (childItem == null)
			{
				continue;
			}
			string identifier = childItem.identifier;
			if (Array.IndexOf(MilkIds, identifier) >= 0)
			{
				hashSet.Add(identifier);
			}
		}
		return hashSet.Count;
	}
	private static void GeneratePreferences(StoreClient client, string buyerFaction)
	{
		bool premium = buyerFaction == "FACTION_BLACK_MARKET" || buyerFaction == "FACTION_UPPER_LEVEL";
		AddPref(client, "F", PickFactionPreference(buyerFaction));
		if (RNG.Roll(10))
		{
			AddPref(client, "J", MaidenTraits.Pick(MaidenTraits.JOBS));
		}
		bool hasBody = false;
		bool hasBreast = false;
		string body = null;
		if (RNG.Roll(50))
		{
			body = PickBody();
			AddPref(client, "B", body);
			hasBody = true;
		}
		else
		{
			AddPref(client, "R", MaidenTraits.Pick(MaidenTraits.BREASTS));
			hasBreast = true;
		}
		if (RNG.Roll(40))
		{
			if (hasBody)
			{
				if (!hasBreast)
				{
					AddPref(client, "R", MaidenTraits.Pick(MaidenTraits.BREASTS));
					hasBreast = true;
				}
			}
			else if (!hasBody)
			{
				body = PickBody();
				AddPref(client, "B", body);
				hasBody = true;
			}
		}
		if (hasBody && MaidenAgeFilter.IsYoungBody(body))
		{
			if (premium && !hasBreast)
			{
				AddPref(client, "R", MaidenTraits.Pick(MaidenTraits.BREASTS));
			}
			return;
		}
		if (!premium && !RNG.Roll(50))
		{
			return;
		}
		int num = RNG.GetRandomInt(0, 99);
		if (!hasBody && num < 15)
		{
			AddPref(client, "B", PickAdultBody());
		}
		else if (!hasBreast && num < 30)
		{
			AddPref(client, "R", MaidenTraits.Pick(MaidenTraits.BREASTS));
		}
		else if (num < 65)
		{
			AddPref(client, "V", PickVirgin());
		}
		else
		{
			AddPref(client, "A", PickActPreference());
		}
	}

	private static string PickFactionPreference(string buyerFaction)
	{
		string[] ids;
		int[] weights;
		switch (buyerFaction)
		{
		case "FACTION_BLACK_MARKET":
			ids = new string[4] { "下层区", "上层区", "治安部", "革命军" };
			weights = new int[4] { 40, 25, 15, 20 };
			break;
		case "FACTION_UPPER_LEVEL":
			ids = new string[4] { "下层区", "黑市", "治安部", "革命军" };
			weights = new int[4] { 35, 30, 15, 20 };
			break;
		case "FACTION_SECURITY":
			ids = new string[4] { "下层区", "上层区", "黑市", "革命军" };
			weights = new int[4] { 40, 20, 20, 20 };
			break;
		case "FACTION_REVOLUTION":
			ids = new string[4] { "下层区", "上层区", "黑市", "治安部" };
			weights = new int[4] { 40, 15, 25, 20 };
			break;
		default:
			ids = new string[4] { "上层区", "黑市", "治安部", "革命军" };
			weights = new int[4] { 20, 40, 15, 25 };
			break;
		}
		return ids[PickWeighted(weights)];
	}

	private static string PickBody()
	{
		return BodyNames[PickWeighted(BodyWeights)];
	}

	private static string PickAdultBody()
	{
		return MaidenAgeFilter.PickNonYoungBody();
	}

	private static string PickVirgin()
	{
		int num = RNG.GetRandomInt(0, 99);
		if (num < 55)
		{
			return "处女";
		}
		if (num < 95)
		{
			return "双处女";
		}
		return "肛门处女";
	}

	private static string PickActPreference()
	{
		string[] array = new string[3] { "ORAL", "SEX", "ANAL" };
		string text = array[RNG.GetRandomIntExculsive(0, array.Length)];
		bool flag = RNG.Roll(85);
		int num = ((!flag) ? PickInt(100, 150, 200) : (text switch
		{
			"ORAL" => PickInt(5, 10, 15, 20),
			"SEX" => PickInt(3, 5, 10, 20),
			_ => PickInt(2, 5, 10, 20),
		}));
		return text + "|" + (flag ? "<" : ">") + "|" + num;
	}

	private static int PickInt(params int[] values)
	{
		return values[RNG.GetRandomIntExculsive(0, values.Length)];
	}

	private static int PickWeighted(int[] weights)
	{
		return RNG.GetOdd(weights);
	}

	private static void AddPref(StoreClient client, string kind, string value)
	{
		string text = PrefPrefix + kind + "|" + value;
		if (!client.tag.Contains(text))
		{
			client.tag.Add(text);
		}
	}

	private static string DefaultDealLine(StoreClient client)
	{
		return IsYoungBodyPreferred(client) ? MaidenBuyerText.YoungDealLine(client.clientFaction) : MaidenBuyerText.DealLine(client.clientFaction);
	}

	/// <summary>验货时按「买家阵营 × 苗床阵营」换一句临场台词；年轻组统一走 MaidenAgeFilter 判定并只给中性词。</summary>
	private static void RefreshAcceptLine(StoreClient client, GameItem item)
	{
		if (client == null || item == null || !item.IsTag("MAIDEN_FEMALE") || !BuyerVisit.CanTrade(client) || client.acceptDealDialoguePlayed)
		{
			return;
		}
		try
		{
			Dialogue dialogue = new Dialogue();
			Dialogue node = dialogue.SetText(client.displayName, FactionPairLine(client.clientFaction, item));
			bool securityRefusal = client.clientFaction == "FACTION_SECURITY" && TagStr(item, "MAIDEN_FACTION") == "治安部";
			if (!MaidenAgeFilter.IsYoung(item) && !securityRefusal)
			{
				node.NextDialogue().SetText(client.displayName, MaidenBuyerText.DealLine(client.clientFaction));
			}
			client.acceptDealDialogue = dialogue;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("刷新收购商成交台词失败: " + ex.Message);
		}
	}

	/// <summary>货被拿下来时恢复默认成交台词。</summary>
	private static void ResetAcceptLine(StoreClient client)
	{
		if (client == null || !BuyerVisit.CanTrade(client) || client.acceptDealDialoguePlayed)
		{
			return;
		}
		try
		{
			client.acceptDealDialogue = new Dialogue().SetText(client.displayName, DefaultDealLine(client));
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("重置收购商成交台词失败: " + ex.Message);
		}
	}

	/// <summary>把开场白、每条要求和收尾话拆成多句，交给游戏原生对话逐句播放。</summary>
	private static List<string> DemandLines(StoreClient client, string buyerFaction)
	{
		bool youngPreferred = IsYoungBodyPreferred(client);
		List<string> lines = new List<string>
		{
			MaidenBuyerText.DemandHeader(youngPreferred, buyerFaction)
		};
		foreach (string item in client.tag)
		{
			if (!item.StartsWith(PrefPrefix, StringComparison.Ordinal))
			{
				continue;
			}
			string[] parts = item.Substring(PrefPrefix.Length).Split(new char[1] { '|' }, StringSplitOptions.None);
			if (parts.Length < 2)
			{
				continue;
			}
			string text = MaidenBuyerText.DescribePreference(parts, buyerFaction, youngPreferred);
			if (!string.IsNullOrEmpty(text))
			{
				lines.Add(MaidenBuyerText.AsSentence(text));
			}
		}
		lines.Add(MaidenBuyerText.DemandTail(youngPreferred, buyerFaction));
		return lines;
	}

	/// <summary>买家看到货时的临场台词：按「买家阵营 × 苗床阵营」取预设。</summary>
	private static string FactionPairLine(string buyerFaction, GameItem item)
	{
		if (item == null)
		{
			return MaidenBuyerText.DealLine(buyerFaction);
		}
		return MaidenBuyerText.FactionPairLine(MaidenBuyerText.FactionName(buyerFaction), TagStr(item, "MAIDEN_FACTION"), MaidenAgeFilter.IsYoung(item));
	}

	private static bool IsYoungBodyPreferred(StoreClient client)
	{
		foreach (string item in client.tag)
		{
			string bodyPrefix = PrefPrefix + "B|";
			if (item.StartsWith(bodyPrefix, StringComparison.Ordinal) &&
				MaidenAgeFilter.IsYoungBody(item.Substring(bodyPrefix.Length)))
			{
				return true;
			}
		}
		return false;
	}

	private static void AddDonation(GameItem item)
	{
		if (item == null || !item.IsTag("MAIDEN_FEMALE"))
		{
			return;
		}
		try
		{
			if (item.FindItemFeatureByID("bribeDonation") == null)
			{
				item.AddItemFeature(ItemFeatureList.Donation());
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("添加 0 元捐赠标签失败: " + ex.Message);
		}
	}

	private static void RemoveDonation(GameItem item)
	{
		if (item == null)
		{
			return;
		}
		try
		{
			ItemFeature itemFeature = item.FindItemFeatureByID("bribeDonation");
			if (itemFeature != null)
			{
				item.RemoveItemFeature(itemFeature);
			}
		}
		catch
		{
		}
	}

	private static long GetSaleBaseValue(GameItem item)
	{
		// 基础价统一使用本模组估值公式的结果；不经游戏物品特征、附加价值或议价估值。
		ItemFactory.UpdateMaidenValue(item);
		return Math.Max(1L, item.unitValue);
	}

	/// <summary>治安部买家收到治安部苗床：用对话翻脸，然后离店。</summary>
	private static void ShowSecurityRefusal(StoreClient client)
	{
		try
		{
			string text = ((client != null && !string.IsNullOrEmpty(client.displayName)) ? client.displayName : MaidenBuyerText.FactionName("FACTION_SECURITY"));
			string[] lines = MaidenBuyerText.SecurityRefusalLines();
			Dialogue dialogue = new Dialogue();
			Dialogue node = dialogue.SetText(text, lines[0]);
			for (int i = 1; i < lines.Length; i++)
			{
				node = node.NextDialogue().SetText(text, lines[i]);
			}
			node.SetEndAction(() => BuyerVisit.FinishDialogue(client));
			// SellItem 尚未付款、转移归属；让原生出售入口在本回调返回后播放，避免中途切换顾客。
			client.acceptDealDialogue = dialogue;
			client.acceptDealDialoguePlayed = false;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("治安部拒绝对话失败: " + ex.Message);
			BuyerVisit.SetDialogue(client, MaidenBuyerText.FirstPurchaseFailedLines());
		}
	}

	private static void HandleSale(StoreClient client, GameItem item, int cost)
	{
		if (client == null || item == null || !client.IsTag(TagBuyer) || !item.IsTag("MAIDEN_FEMALE") || !BuyerVisit.CanTrade(client))
		{
			return;
		}
		bool extra = BuyerVisit.IsExtra(client);
		BuyerVisit.BeginSale(client);
		try
		{
			// 治安部买家收到治安部的苗床：翻脸、离店、本单不结算，声望 -10。
			if (client.clientFaction == "FACTION_SECURITY" && ItemFactory.GetStr(item, "MAIDEN_FACTION") == "治安部")
			{
				BuyerVisit.Close(client);
				SecurityLiaison.AddReputation(-10);
				if (cost > 0)
				{
					ModHelper.AddCredit(-cost, false);
				}
				ModCtx.Log("治安部买家收到治安部苗床：声望 -10，交易取消并离店");
				ShowSecurityRefusal(client);
				return;
			}
			List<Check> checks = extra ? new List<Check>() : Evaluate(client, item);
			int reqCount = checks.Count;
			int matchedCount = 0;
			bool factionMatched = false;
			foreach (Check check in checks)
			{
				if (check.Pass)
				{
					matchedCount++;
					if (check.Label == MaidenBuyerText.LabelFaction)
					{
						factionMatched = true;
					}
				}
			}
			bool allMatch = !extra && (reqCount == 0 || matchedCount == reqCount);
			long baseValue = GetSaleBaseValue(item);
			double matchMul = extra ? 1.0 : PayoutMultiplier(reqCount, matchedCount, factionMatched, allMatch);
			long payout = (long)Math.Round((double)baseValue * matchMul);
			string mulText = matchMul.ToString("0.##");
			// PlayerStore.SellItem 会在本回调结束后另行支付 cost，只补目标成交价与 cost 的差额。
			ModHelper.AddCredit((int)(payout - cost), true);
			SpawnReceipt(client, item, checks, matchedCount, matchMul, allMatch, baseValue, payout);
			Toast.Show(extra ? MaidenBuyerText.ExtraSettlementToast(payout) : MaidenBuyerText.SettlementToast(matchedCount, reqCount, mulText, payout), MaidenBuyerText.SettlementToastTitle);
			ModCtx.Log($"收购商结算: {client.displayName} / {MaidenSummary(item)} / 基础价 {baseValue} / 满足 {matchedCount}/{reqCount} 条 x{mulText}{(allMatch ? "（全满足）" : "")} / 成交 {payout} / 游戏支付 {cost} / 补差 {payout - cost}");
			CommitTraffickingCrime(item, baseValue);
			MaidenLedger.RecordSale(item, client, reqCount, matchedCount, factionMatched, allMatch, baseValue, payout);
			SecurityLiaison.OnMaidenSold(item);
			BuyerVisit.CompleteSale(client, extra, allMatch);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("收购商结算失败: " + ex.Message);
			BuyerVisit.Close(client);
			BuyerVisit.SetDialogue(client, MaidenBuyerText.PurchaseErrorLines());
		}
	}

	private static List<Check> Evaluate(StoreClient client, GameItem item)
	{
		List<Check> list = new List<Check>();
		string faction = TagStr(item, "MAIDEN_FACTION");
		string job = TagStr(item, "MAIDEN_JOB");
		string body = TagStr(item, "MAIDEN_BODY");
		string breast = TagStr(item, "MAIDEN_BREAST");
		foreach (string item2 in client.tag)
		{
			if (!item2.StartsWith(PrefPrefix, StringComparison.Ordinal))
			{
				continue;
			}
			string[] array = item2.Substring(PrefPrefix.Length).Split(new char[1] { '|' }, StringSplitOptions.None);
			if (array.Length < 2)
			{
				continue;
			}
			switch (array[0])
			{
			case "F":
				list.Add(MakeCheck(MaidenBuyerText.LabelFaction, array[1], faction, faction == array[1]));
				break;
			case "J":
				list.Add(MakeCheck(MaidenBuyerText.LabelJob, array[1], job, job == array[1]));
				break;
			case "B":
				list.Add(MakeCheck(MaidenBuyerText.LabelBody, array[1], body, body == array[1]));
				break;
			case "R":
				list.Add(MakeCheck(MaidenBuyerText.LabelBreast, array[1], breast, breast == array[1]));
				break;
			case "V":
				list.Add(MakeCheck(MaidenBuyerText.LabelVirgin, array[1], MaidenBuyerText.VirginState(item.IsTag("MAIDEN_VIRGIN"), item.IsTag("MAIDEN_ANAL_VIRGIN")), VirginMatches(item, array[1])));
				break;
			case "A":
				if (array.Length >= 4)
				{
					string text = MaidenBuyerText.ActLabel(array[1]);
					int num = ActCount(item, array[1]);
					bool pass = ((array[2] == "<") ? (num < ParseInt(array[3])) : (num > ParseInt(array[3])));
					list.Add(MakeCheck(text, MaidenBuyerText.ActExpected(array[2], array[3]), num.ToString(), pass));
				}
				break;
			}
		}
		return list;
	}

	private static Check MakeCheck(string label, string expected, string actual, bool pass)
	{
		return new Check
		{
			Label = label,
			Expected = expected,
			Actual = actual,
			Pass = pass
		};
	}

	private static bool VirginMatches(GameItem item, string requirement)
	{
		bool flag = item.IsTag("MAIDEN_VIRGIN");
		bool flag2 = item.IsTag("MAIDEN_ANAL_VIRGIN");
		return requirement switch
		{
			"双处女" => flag && flag2,
			"肛门处女" => flag2,
			_ => flag,
		};
	}

	private static int ActCount(GameItem item, string act)
	{
		return act switch
		{
			"ORAL" => ItemFactory.GetInt(item, "ORAL_COUNT"),
			"SEX" => ItemFactory.GetInt(item, "SEX_COUNT"),
			_ => ItemFactory.GetInt(item, "ANAL_COUNT"),
		};
	}

	private static int ParseInt(string text)
	{
		if (int.TryParse(text, out var result))
		{
			return result;
		}
		return 0;
	}

	private static string TagStr(GameItem item, string tag)
	{
		string text = ItemFactory.GetStr(item, tag);
		return string.IsNullOrEmpty(text) ? MaidenBuyerText.Unknown : text;
	}

	private static string MaidenSummary(GameItem item)
	{
		string text = ItemFactory.GetStr(item, "MAIDEN_NAME");
		if (string.IsNullOrEmpty(text))
		{
			text = item.name;
		}
		return MaidenBuyerText.MaidenSummary(text, TagStr(item, "MAIDEN_FACTION"), TagStr(item, "MAIDEN_JOB"), TagStr(item, "MAIDEN_BODY"), TagStr(item, "MAIDEN_BREAST"), MaidenBuyerText.VirginState(item.IsTag("MAIDEN_VIRGIN"), item.IsTag("MAIDEN_ANAL_VIRGIN")));
	}

	/// <summary>把苗床成交记成治安部的「贩卖人口」罪名（与 PatchMaidenCrime 共用同一套记账/去重逻辑）。</summary>
	private static void CommitTraffickingCrime(GameItem item, long baseValue)
	{
		MaidenCrime.Record(item, baseValue);
	}

	/// <summary>结算倍率：全满足走上限公式；否则 0.2 起，符合阵营则 0.6 起，每多一条 +0.2。</summary>
	public static double PayoutMultiplier(int reqCount, int matchedCount, bool factionMatched, bool allMatch)
	{
		if (allMatch)
		{
			return 1.0 + 0.2 * Math.Max(0, reqCount - 2);
		}
		int num = Math.Max(0, matchedCount - (factionMatched ? 1 : 0));
		return 0.2 + (factionMatched ? 0.4 : 0.0) + 0.2 * (double)num;
	}

	private static void SpawnReceipt(StoreClient client, GameItem item, List<Check> checks, int matchedCount, double matchMul, bool allMatch, long baseValue, long payout)
	{
		try
		{
			GameItem gameItem = DirectoryMaster.Item("sec_slip");
			if (gameItem == null)
			{
				ModCtx.LogWarning("收购回执生成失败：sec_slip 不存在");
				return;
			}
			string title = MaidenBuyerText.ReceiptTitle;
			string body = BuildReceipt(client, item, checks, matchedCount, matchMul, allMatch, baseValue, payout);
			gameItem.name = MaidenBuyerText.ReceiptTitle;
			gameItem.shortDescription = MaidenBuyerText.ReceiptShortDescription;
			gameItem.flavorText = MaidenBuyerText.ReceiptFlavorText;
			gameItem.unitValue = 0L;
			gameItem.ModifyTag("title", delegate(TagState state)
			{
				state.Enable().SetString(title);
			}, false);
			gameItem.ModifyTag("body", delegate(TagState state)
			{
				state.Enable().SetString(body);
			}, false);
			PlayerStore.Instance.AddDirectSellingItemToTable(gameItem, isOwend: true);
			ModCtx.Log("收购回执已放到柜台");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("生成收购回执失败: " + ex.Message);
		}
	}

	public static void RepairReceipts()
	{
		try
		{
			EmporiumEntry instance = EmporiumEntry.Instance;
			if (instance == null)
			{
				return;
			}
			List<GameItem> allItems = instance.GetAllItems();
			if (allItems == null)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < allItems.Count; i++)
			{
				GameItem val = allItems[i];
				if (val == null || ItemFactory.GetStr(val, "title") != MaidenBuyerText.ReceiptTitle)
				{
					continue;
				}
				val.name = MaidenBuyerText.ReceiptTitle;
				val.shortDescription = MaidenBuyerText.ReceiptShortDescription;
				val.flavorText = MaidenBuyerText.ReceiptFlavorText;
				val.unitValue = 0L;
				num++;
			}
			if (num > 0)
			{
				ModCtx.Log("已修正 " + num + " 张苗床收购回执的名称与描述");
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("修正收购回执失败: " + ex.Message);
		}
	}

	private static string BuildReceipt(StoreClient client, GameItem item, List<Check> checks, int matchedCount, double matchMul, bool allMatch, long baseValue, long payout)
	{
		if (checks.Count == 0 && !allMatch)
		{
			return MaidenBuyerText.ExtraReceipt(client.displayName, MaidenSummary(item), baseValue, payout);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(MaidenBuyerText.ReceiptHeader(client.displayName, MaidenSummary(item)));
		int requirementNumber = 0;
		foreach (Check check in checks)
		{
			stringBuilder.Append(MaidenBuyerText.ReceiptRequirement(++requirementNumber, check.Label, check.Expected, check.Actual, check.Pass));
		}
		stringBuilder.Append(MaidenBuyerText.ReceiptSummary(matchedCount, checks.Count, allMatch));
		stringBuilder.Append(MaidenBuyerText.ReceiptPayout(baseValue, matchMul, payout));
		stringBuilder.Append(MaidenBuyerText.ReceiptBuyerComment(FactionPairLine(client.clientFaction, item)));
		return stringBuilder.ToString();
	}

}
