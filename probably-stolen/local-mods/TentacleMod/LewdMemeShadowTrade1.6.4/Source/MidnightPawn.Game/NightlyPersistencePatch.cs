using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>
/// 游戏先 EndNight、BeginDay、SaveGame，再打开晨报并触发 OnWakingUpLate。
/// 所有夜间持久状态必须在 SaveGame 前完成；起床事件不承担状态结算。
/// </summary>
[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.EndNight))]
internal static class NightlyPersistencePatch
{
	[HarmonyPriority(Priority.Last)]
	private static void Postfix(bool __runOriginal)
	{
		if (!__runOriginal) return;
		MaidenFatigue.OnNightSettled();
	}
}

/// <summary>电话本在次日初始化完成后同步，随后由游戏的正常自动存档保存。</summary>
[HarmonyPatch(typeof(PlayerStore), nameof(PlayerStore.BeginDay))]
internal static class DailyPersistencePatch
{
	[HarmonyPriority(Priority.Last)]
	private static void Postfix(bool __runOriginal)
	{
		if (__runOriginal) SecurityLiaison.OnWakeUp();
	}
}
