using System;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>
/// 小物件（精液 / 排卵针 / 乳汁 / 名片）整块可抓取：只调背景图的 alpha 命中阈值。
/// 苗床的 2×4 覆盖层点选区已移除，苗床点击/拖动交回游戏原始判定。
/// </summary>
[HarmonyPatch(typeof(GameItemElement), "Validate")]
public static class PatchLooseItemHitTest
{
	private static void Postfix(GameItemElement __instance)
	{
		try
		{
			if (__instance == null || !IsLooseItem(__instance))
			{
				return;
			}
			TreeNodeRender background = __instance.background;
			if (background == null || background.image == null)
			{
				return;
			}
			background.image.raycastTarget = true;
			if (background.image.alphaHitTestMinimumThreshold > 0f)
			{
				background.image.alphaHitTestMinimumThreshold = 0f;
			}
		}
		catch (NullReferenceException)
		{
			// UI 节点尚未初始化时的瞬态异常，静默跳过
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("小物件点击区域设置失败: " + ex.Message);
		}
	}

	private static bool IsLooseItem(GameItemElement element)
	{
		return element.IsID("tentacle_semen") || element.IsID("ovulation_shot") || element.IsID("milk_rich") || element.IsID("milk_wondrous") || element.IsID("milk_diluted") || element.IsID("wondrous_milk") || element.IsID(SecurityLiaison.CardItemId) || element.IsID(NightReportNote.ItemId);
	}
}
