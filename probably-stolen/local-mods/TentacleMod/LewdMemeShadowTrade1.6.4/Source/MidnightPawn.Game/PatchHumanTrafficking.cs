using System;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>
/// 苗床成交统一记「贩卖人口」（HUMAN_TRAFFICKING）。
/// 原版卖给任意顾客都会走 SecData.CheckSoldContraband，所以在这里统一记账，
/// 既覆盖原版顾客，也覆盖本 mod 的收购商（用 tag 去重，避免重复计数）。
/// </summary>
public static class MaidenCrime
{
	private const string RECORDED_TAG = "MAIDEN_CRIME_RECORDED";

	public static void Record(GameItem item, long valueHint = 0L)
	{
		if (item == null || !item.IsTag("MAIDEN_FEMALE"))
		{
			return;
		}
		try
		{
			if (item.IsTag(RECORDED_TAG))
			{
				return;
			}
			item.EnableTag(RECORDED_TAG);
			if (SecurityLiaison.MaidenRegistered)
			{
				ModCtx.Log("治安部罪名: 跳过（苗床已登记为管制物品）");
				return;
			}
			int num = (int)item.GetNegociatedValue();
			if (num <= 0)
			{
				num = (int)valueHint;
			}
			if (num <= 0)
			{
				num = (int)item.GetBaseValue();
			}
			if (num <= 0)
			{
				num = 1;
			}
			num = SecurityLiaison.ConsumeCrimeCredit(num);
			if (num <= 0)
			{
				ModCtx.Log("治安部罪名: 已被「归档」额度全部抵扣");
				return;
			}
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null || instance.secData == null)
			{
				return;
			}
			instance.secData.CommitCrime("HUMAN_TRAFFICKING", num);
			ModCtx.Log($"治安部罪名: 贩卖人口（案值 {num}）");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("记录贩卖人口罪名失败: " + ex.Message);
		}
	}
}

/// <summary>苗床不走原版“违禁品走私”罪名，改为记「贩卖人口」。</summary>
[HarmonyPatch(typeof(SecData), "CheckSoldContraband")]
public static class PatchMaidenCrime
{
	private static bool Prefix(GameItem gameItem)
	{
		try
		{
			if (gameItem != null && gameItem.IsTag("MAIDEN_FEMALE"))
			{
				MaidenCrime.Record(gameItem);
				return false;
			}
		}
		catch
		{
		}
		return true;
	}
}

/// <summary>让治安部档案里显示「贩卖人口」而不是“其它犯罪”。</summary>
[HarmonyPatch(typeof(SecData), "GetCrimeString")]
public static class PatchCrimeString
{
	private static bool Prefix(string crimeID, ref string __result)
	{
		if (crimeID == "HUMAN_TRAFFICKING")
		{
			__result = "贩卖人口";
			return false;
		}
		return true;
	}
}
