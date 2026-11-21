using System;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>
/// 新建物品时立刻补上违禁品词条：
/// 排卵诱发剂 I 级 / 触手精液 II 级 / 苗床 III 级。
/// 已由治安部剧情登记为管制物品（RESTRICTED）的不处理。
/// </summary>
[HarmonyPatch(typeof(DirectoryMaster), "Item", new Type[] { typeof(string), typeof(bool) })]
internal static class PatchItemCreateContraband
{
	private static void Postfix(GameItem __result)
	{
		try
		{
			ItemFactory.ApplyContrabandOnCreate(__result);
			ItemFactory.SetupLiquidTargeting(__result);
			ItemFactory.EnsureBarrelCallbacks(__result);
			ItemFactory.ConfigureOvulationShot(__result);
			InitializeMaiden(__result, false);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("新物品初始化补丁失败: " + ex.Message);
		}
	}

	// 同时供物品目录后置补丁与原版新工厂后置补丁调用；重复调用不重抽偏好、不叠加估值。
	internal static void InitializeMaiden(GameItem item, bool applyContraband = true)
	{
		if (item == null || (!item.IsTag("MAIDEN_FEMALE") && item.identifier != "maiden_lower")) return;
		try
		{
			if (applyContraband) ItemFactory.ApplyContrabandOnCreate(item);
			MaidenPrefs.Assign(item);
			ItemFactory.UpdateMaidenValue(item);
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("角色初始化补丁失败: " + ex.Message);
		}
	}
}
