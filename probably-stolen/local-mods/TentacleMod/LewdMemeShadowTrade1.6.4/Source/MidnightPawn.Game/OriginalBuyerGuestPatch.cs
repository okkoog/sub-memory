using System;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>收购商不生成原版客人格；普通顾客继续沿用原版入口。</summary>
internal static class OriginalBuyerGuestPatch
{
	internal static int Install(Harmony harmony)
	{
		try
		{
			Type type = CompatOriginal.OriginalType("MidnightPawn.Game.GuestBox");
			if (type == null) return 0;
			var show = ModReflect.Target(type, "Show", Type.EmptyTypes);
			if (show == null || ModReflect.Target(type, "Hide", Type.EmptyTypes) == null) return 0;
			harmony.Patch(show, prefix: new HarmonyMethod(AccessTools.Method(typeof(OriginalBuyerGuestPatch), nameof(Prefix))));
			return 1;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("收购商客人格屏蔽安装失败: " + ex.Message);
			return 0;
		}
	}

	private static bool Prefix()
	{
		if (!BuyerVisit.IsBuyer(PlayerStore.Instance?.currentClientInstance?.GetClientBlueprint())) return true;
		// Show 原本先 Hide；跳过显示时仍需清理上一位顾客的窗口和临时物品。
		CompatOriginal.InvokeOriginalStatic("MidnightPawn.Game.GuestBox", "Hide");
		return false;
	}
}
