using System;
using System.Collections.Generic;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>一次交一位；首次合格后追加 3～5 位，追加阶段按基础估值结算。</summary>
internal static class BuyerVisit
{
	private const string ExtraPrefix = "MAIDEN_BUYER_EXTRA|";
	private const string PendingTag = "MAIDEN_BUYER_DIALOGUE_PENDING";
	private static StoreClient SelectionClient;
	private static GameItem Selection;
	internal static bool IsBuyer(StoreClient client) => client != null && client.IsTag("MAIDEN_BUYER");
	internal static bool CanTrade(StoreClient client) => !IsBuyer(client) || (!client.isBusinessComplete && !client.IsTag(PendingTag));
	internal static bool IsExtra(StoreClient client) => GetRemaining(client) >= 0;

	internal static int GetRemaining(StoreClient client)
	{
		if (client == null) return -1;
		foreach (string tag in client.tag)
		{
			if (tag.StartsWith(ExtraPrefix, StringComparison.Ordinal) && int.TryParse(tag.Substring(ExtraPrefix.Length), out int count))
				return Math.Max(0, Math.Min(5, count));
		}
		return -1;
	}

	private static void SetRemaining(StoreClient client, int count)
	{
		client.tag.RemoveAll(tag => tag.StartsWith(ExtraPrefix, StringComparison.Ordinal));
		client.AddTag(ExtraPrefix + count);
	}

	internal static void BeginSale(StoreClient client) => client.AddTag(PendingTag);

	internal static void Close(StoreClient client)
	{
		client.isBusinessComplete = true;
		client.clientBuyingIdList.Clear();
		client.clientBuyingTagList.Clear();
	}

	internal static void FinishDialogue(StoreClient client)
	{
		if (client.isBusinessComplete)
		{
			PlayerStore store = PlayerStore.Instance;
			if (store?.currentClientInstance != null && ReferenceEquals(store.currentClientInstance.GetClientBlueprint(), client))
				store.DismissCurrentClient(false);
		}
		else
		{
			client.tag.Remove(PendingTag);
			if (ReferenceEquals(PlayerStore.Instance?.currentClientInstance?.GetClientBlueprint(), client))
				NegociationUIManager.Instance?.RefreshUI();
		}
	}

	internal static void CompleteSale(StoreClient client, bool extra, bool allMatch)
	{
		string[] lines;
		if (extra)
		{
			int remaining = Math.Max(0, GetRemaining(client) - 1);
			SetRemaining(client, remaining);
			if (remaining == 0) Close(client);
			lines = MaidenBuyerText.ExtraPurchaseLines(remaining);
		}
		else if (allMatch)
		{
			int remaining = RNG.GetRandomIntExculsive(3, 6);
			SetRemaining(client, remaining);
			lines = MaidenBuyerText.FirstPurchasePassedLines(remaining);
		}
		else
		{
			Close(client);
			lines = MaidenBuyerText.FirstPurchaseFailedLines();
		}
		SetDialogue(client, lines);
	}

	internal static void SetDialogue(StoreClient client, string[] lines)
	{
		Dialogue dialogue = new Dialogue();
		Dialogue tail = dialogue.SetText(client.displayName, lines[0]);
		for (int i = 1; i < lines.Length; i++) tail = tail.NextDialogue().SetText(client.displayName, lines[i]);
		tail.SetEndAction(() => FinishDialogue(client));
		// 回调中只准备对话，由原生出售入口在转账、转移物品后播放。
		client.acceptDealDialogue = dialogue;
		client.acceptDealDialoguePlayed = false;
	}

	internal static List<GameItem> LimitOffer(StoreClient client, List<GameItem> items, GameItem selected = null)
	{
		if (!IsBuyer(client)) return items;
		if (!CanTrade(client) || items == null) return new List<GameItem>();
		if (selected != null && items.Contains(selected) && selected.IsTag("MAIDEN_FEMALE")) return new List<GameItem> { selected };
		foreach (GameItem item in items)
			if (item != null && item.IsTag("MAIDEN_FEMALE")) return new List<GameItem> { item };
		return new List<GameItem>();
	}

	internal static void Select(StoreClient client, GameItem selected)
	{
		SelectionClient = client;
		Selection = selected;
	}

	internal static GameItem Preferred(StoreClient client) => ReferenceEquals(client, SelectionClient) ? Selection : null;
}

// 同时限制界面数量、价格合计和实际成交列表。所有其它顾客继续走原流程。
[HarmonyPatch(typeof(NegociationUIManager), "GetSellableBackInvItems")]
internal static class BuyerSingleOfferPatch
{
	private static void Postfix(StoreClient storeClient, ref List<GameItem> __result)
	{
		__result = BuyerVisit.LimitOffer(storeClient, __result, BuyerVisit.Preferred(storeClient) ?? PlayerStore.Instance?.currentNegociatedItem);
	}
}

[HarmonyPatch(typeof(NegociationUIManager), "BuildBuying")]
internal static class BuyerOfferSelectionPatch
{
	private static void Prefix(StoreClient __0, GameItem ___selectedItem) => BuyerVisit.Select(__0, ___selectedItem);
}

[HarmonyPatch(typeof(NegociationUIManager), "OnSellAllClick")]
internal static class BuyerClickSelectionPatch
{
	private static void Prefix(GameItem ___selectedItem) => BuyerVisit.Select(PlayerStore.Instance?.currentClientInstance?.GetClientBlueprint(), ___selectedItem);
}

[HarmonyPatch(typeof(NegociationUIManager), "SellMultiple")]
internal static class BuyerSingleBatchPatch
{
	private static void Prefix(ref List<GameItem> __0, GameItem ___selectedItem)
	{
		StoreClient client = PlayerStore.Instance?.currentClientInstance?.GetClientBlueprint();
		__0 = BuyerVisit.LimitOffer(client, __0, ___selectedItem);
	}
}

[HarmonyPatch(typeof(StoreClient), "IsClientBuyingThisItem", new Type[] { typeof(GameItem), typeof(bool) })]
internal static class BuyerTradeGatePatch
{
	private static bool Prefix(StoreClient __instance, ref bool __result)
	{
		if (BuyerVisit.CanTrade(__instance)) return true;
		__result = false;
		return false;
	}
}

[HarmonyPatch(typeof(PlayerStore), "SellItem")]
internal static class BuyerDirectSaleGatePatch
{
	private static bool Prefix(PlayerStore __instance, GameItem __0)
	{
		StoreClient client = __instance.currentClientInstance?.GetClientBlueprint();
		return !BuyerVisit.IsBuyer(client) || (BuyerVisit.CanTrade(client) && __0 != null && __0.IsTag("MAIDEN_FEMALE"));
	}
}

[HarmonyPatch(typeof(PlayerStore), "SellAllItemFromTable")]
internal static class BuyerLegacyBatchPatch
{
	private static bool Prefix(PlayerStore __instance)
	{
		if (!BuyerVisit.IsBuyer(__instance.currentClientInstance?.GetClientBlueprint())) return true;
		__instance.SellItemFromTable();
		return false;
	}
}
