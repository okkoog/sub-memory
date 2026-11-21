using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>设备保留原版活动和产出，只增加疲劳快照及透支门禁。</summary>
internal static class OriginalDeviceActivityPatch
{
	private const string Owner = "lewd_meme_remake.compat.device_activity";
	private static readonly Dictionary<GameItem, NightlyMaidenState> States = new Dictionary<GameItem, NightlyMaidenState>();
	private static readonly Dictionary<GameItem, bool> Eligibility = new Dictionary<GameItem, bool>();
	private static int EligibilityDay = -1;
	internal static bool Active { get; private set; }
	internal static void Uninstall()
	{
		CompatOriginal.UnpatchOwner(Owner);
		States.Clear();
		Eligibility.Clear();
		EligibilityDay = -1;
		Active = false;
	}
	internal static int Install()
	{
		Harmony harmony = new Harmony(Owner);
		try
		{
			foreach (string name in new[] { "AlarmClock", "TentacleTrap" })
				harmony.Patch(ModReflect.Target(CompatOriginal.OriginalType("MidnightPawn.Game." + name), "NightlyTickOnce", new[] { typeof(GameItem) }),
					transpiler: new HarmonyMethod(AccessTools.Method(typeof(OriginalDeviceActivityPatch), nameof(Transpiler))),
					finalizer: new HarmonyMethod(AccessTools.Method(typeof(OriginalDeviceActivityPatch), nameof(Finalizer))));
			foreach (string name in new[] { "HasMaidenAlarm", "MaidenNameInAlarm" })
				harmony.Patch(ModReflect.Target(CompatOriginal.OriginalType("MidnightPawn.Game.AlarmClock"), name, Type.EmptyTypes),
					transpiler: new HarmonyMethod(AccessTools.Method(typeof(OriginalDeviceActivityPatch), nameof(QueryTranspiler))));
			Active = true;
			return 4;
		}
		catch (Exception ex)
		{
			harmony.UnpatchAll(Owner);
			Active = false;
			ModCtx.LogWarning("设备疲劳门禁未安装，已保留原版设备规则: " + ex.Message);
			return 0;
		}
	}
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
		int female = FemaleCheck(codes);
		int description = OriginalNightlyPatch.Unique(codes, c => OriginalNightlyPatch.IsCall(c, "MidnightPawn.Game.ItemFactory", "UpdateMaidenDescription"));
		if (description <= female + 1 || codes[description - 1].opcode != codes[female - 1].opcode
			|| !Equals(codes[description - 1].operand, codes[female - 1].operand)) throw new InvalidOperationException("设备角色描述锚点改变");
		codes[female + 1].opcode = OpCodes.Call;
		codes[female + 1].operand = AccessTools.Method(typeof(OriginalDeviceActivityPatch), nameof(Begin));
		codes.InsertRange(description + 1, new[] {
			new CodeInstruction(codes[female - 1].opcode, codes[female - 1].operand),
			new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OriginalDeviceActivityPatch), nameof(Complete)))
		});
		OriginalNightlyPatch.ExpandBranches(codes);
		return codes;
	}
	private static IEnumerable<CodeInstruction> QueryTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
		int female = FemaleCheck(codes);
		codes[female + 1].opcode = OpCodes.Call;
		codes[female + 1].operand = AccessTools.Method(typeof(OriginalDeviceActivityPatch), nameof(CanUse));
		return codes;
	}
	private static int FemaleCheck(List<CodeInstruction> codes)
	{
		int female = OriginalNightlyPatch.Unique(codes, c => c.opcode == OpCodes.Ldstr && Equals(c.operand, "MAIDEN_FEMALE"));
		if (female < 1 || !OriginalNightlyPatch.IsCall(codes[female + 1], "GameItem", "IsTag"))
			throw new InvalidOperationException("设备女性筛选锚点改变");
		return female;
	}
	private static bool CanUse(GameItem item, string tag)
	{
		if (item == null || !item.IsTag(tag)) return false;
		if (!MaidenFatigue.Enabled) return true;
		return EligibilityDay == MaidenFatigue.Today() && Eligibility.TryGetValue(item, out bool allowed)
			? allowed : !MaidenFatigue.ShouldRest(item);
	}
	private static bool Begin(GameItem item, string tag)
	{
		if (item == null || !item.IsTag(tag)) return false;
		int day = MaidenFatigue.Today();
		if (EligibilityDay != day) { Eligibility.Clear(); EligibilityDay = day; }
		bool allowed = !MaidenFatigue.ShouldRest(item);
		Eligibility[item] = allowed;
		if (!allowed)
		{
			MaidenFatigue.RecoverExhausted(item);
			PlayerStore.Instance?.AddNightLog("【透支休养】" + MaidenFatigue.Name(item) + "：本夜设备活动与产出暂停，疲劳恢复 5 点。", "d8a13a");
			return false;
		}
		States[item] = new NightlyMaidenState(item);
		return true;
	}
	private static void Complete(GameItem item)
	{
		if (!States.TryGetValue(item, out NightlyMaidenState state)) return;
		States.Remove(item);
		try
		{
			state.Complete(item);
			ItemFactory.UpdateMaidenValue(item);
			ItemFactory.UpdateMaidenDescription(item);
		}
		catch (Exception ex) { ModCtx.LogWarning("追加设备疲劳失败: " + ex.Message); }
	}
	private static void Finalizer() => States.Clear();

	internal static HashSet<GameItem> CollectMaidens()
	{
		HashSet<GameItem> result = new HashSet<GameItem>();
		List<GameItem> items = EmporiumEntry.Instance?.GetAllItems();
		if (items == null) return result;
		foreach (GameItem machine in items)
		{
			if (machine == null || (machine.identifier != "tentacle_alarm" && machine.identifier != "tentacle_trap")) continue;
			if (machine.identifier == "tentacle_trap" && ItemFactory.GetInt(machine, "TRAP_ENABLED") == 0) continue;
			List<GameGridInventory> grids = TentacleBody.ResolveGrids(machine);
			if (grids == null || grids.Count < 2) continue;
			GameGridInventory input = grids[0];
			if (input?.childItems == null) continue;
			foreach (GameItem item in input.childItems)
				if (item != null && item.IsTag("MAIDEN_FEMALE")) result.Add(item);
		}
		return result;
	}
}

