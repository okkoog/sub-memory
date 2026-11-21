using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>保留原版角色循环，只添加透支门禁、结算后追加及箱内输入。</summary>
internal static class OriginalNightlyPatch
{
	private const string Owner = "lewd_meme_remake.compat.nightly";
	internal static bool Active { get; private set; }
	internal static void Uninstall()
	{
		CompatOriginal.UnpatchOwner(Owner);
		Active = false;
	}

	internal static int Install(Assembly assembly)
	{
		Harmony harmony = new Harmony(Owner);
		try
		{
			Type type = CompatOriginal.OriginalType("MidnightPawn.Game.TentacleBody");
			MethodBase tick = ModReflect.Target(type, "NightlyTick", new[] { typeof(GameItem), typeof(GameGridInventory), typeof(GameGridInventory) });
			MethodBase settle = ModReflect.Target(type, "SettleAll", Type.EmptyTypes);
			if (tick == null || settle == null) throw new MissingMethodException("缺少原版夜间入口");
			harmony.Patch(tick, transpiler: Patch(nameof(TickTranspiler)));
			harmony.Patch(settle, prefix: Patch(nameof(SettlementPrefix)), postfix: Patch(nameof(SettlementPostfix)),
				transpiler: Patch(nameof(ExperienceTranspiler)), finalizer: Patch(nameof(SettlementFinalizer)));
			Active = true;
			ModCtx.Log("夜间追加已接入：复用原版活动、受孕与产出；追加乳汁逐份抵扣原版乳汁并优先入桶，透支角色整夜暂停，整轮完成后整理桶装。");
			return 2;
		}
		catch (Exception ex)
		{
			harmony.UnpatchAll(Owner);
			Active = false;
			ModCtx.LogWarning("夜间追加未安装，已撤销整组补丁并保留原版规则: " + ex.Message);
			return 0;
		}
	}

	private static HarmonyMethod Patch(string name) => new HarmonyMethod(AccessTools.Method(typeof(OriginalNightlyPatch), name));
	private static bool SettlementPrefix(out bool __state) => __state = TentacleBody.BeginOriginalSettlement();
	private static void SettlementPostfix(bool __state, bool __runOriginal)
	{
		if (!__state) return;
		if (__runOriginal) TentacleBody.CompleteOriginalSettlement();
		else TentacleBody.AbortOriginalSettlement();
	}
	private static void SettlementFinalizer(bool __state, Exception __exception)
	{
		if (__state && __exception != null) TentacleBody.AbortOriginalSettlement();
	}
	private static IEnumerable<CodeInstruction> TickTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
		int female = Unique(codes, c => c.opcode == OpCodes.Ldstr && Equals(c.operand, "MAIDEN_FEMALE"));
		int description = Unique(codes, c => IsCall(c, "MidnightPawn.Game.ItemFactory", "UpdateMaidenDescription"));
		int start = female + 3;
		if (female < 1 || !IsCall(codes[female + 1], "GameItem", "IsTag")
			|| (codes[female + 2].opcode != OpCodes.Brfalse && codes[female + 2].opcode != OpCodes.Brfalse_S)
			|| !(codes[female + 2].operand is Label exit) || start + 2 >= description
			|| codes[start + 1].opcode != OpCodes.Ldstr || !Equals(codes[start + 1].operand, "TRAIN_VALUE")
			|| !IsCall(codes[start + 2], "MidnightPawn.Game.ItemFactory", "GetInt")
			|| codes[start].opcode != codes[female - 1].opcode || !Equals(codes[start].operand, codes[female - 1].operand)
			|| codes[description - 1].opcode != codes[start].opcode || !Equals(codes[description - 1].operand, codes[start].operand))
			throw new InvalidOperationException("原版角色门禁或描述锚点改变");
		// 不删除、不改写任何原版活动、受孕、分娩、产出指令。
		codes.InsertRange(description + 1, new[] {
			new CodeInstruction(codes[start].opcode, codes[start].operand), new CodeInstruction(OpCodes.Ldarg_2),
			new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TentacleBody), nameof(TentacleBody.AppendMaidenSettlement)))
		});
		CodeInstruction first = new CodeInstruction(codes[start].opcode, codes[start].operand);
		first.labels.AddRange(codes[start].labels);
		codes[start].labels.Clear();
		codes.InsertRange(start, new[] { first, new CodeInstruction(OpCodes.Ldarg_2),
			new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TentacleBody), nameof(TentacleBody.BeginMaidenSettlement))),
			new CodeInstruction(OpCodes.Brfalse, exit) });
		ExpandBranches(codes);
		return codes;
	}

	private static IEnumerable<CodeInstruction> ExperienceTranspiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
		int call = Unique(codes, c => IsCall(c, "MidnightPawn.Game.TentacleBody", "AddBodyExp"));
		if (call < 4 || codes[call - 4].opcode != OpCodes.Ldloc_0 || codes[call - 3].opcode != OpCodes.Ldc_I4_0
			|| (codes[call - 2].opcode != OpCodes.Ble && codes[call - 2].opcode != OpCodes.Ble_S)
			|| codes[call - 1].opcode != OpCodes.Ldloc_0) throw new InvalidOperationException("原版经验人数锚点改变");
		// 箱内角色加入人数，透支角色不计经验；升级仍由原版执行。
		foreach (int i in new[] { call - 4, call - 1 })
		{
			codes[i].opcode = OpCodes.Call;
			codes[i].operand = AccessTools.Method(typeof(TentacleBody), nameof(TentacleBody.GetSettledMaidenCount));
		}
		// 在全体主苗床遍历完成后、经验和原版报告之前补算，避免依赖某个输入页的回调。
		CodeInstruction supplemental = new CodeInstruction(OpCodes.Call,
			AccessTools.Method(typeof(TentacleBody), nameof(TentacleBody.SettleSupplementaryInputs)));
		supplemental.labels.AddRange(codes[call - 4].labels);
		codes[call - 4].labels.Clear();
		codes.Insert(call - 4, supplemental);
		ExpandBranches(codes);
		return codes;
	}

	internal static int Unique(List<CodeInstruction> codes, Predicate<CodeInstruction> match)
	{
		int found = codes.FindIndex(match);
		if (found < 0 || codes.FindIndex(found + 1, match) >= 0) throw new InvalidOperationException("原版锚点缺失或不唯一");
		return found;
	}
	internal static bool IsCall(CodeInstruction code, string type, string method) =>
		(code.opcode == OpCodes.Call || code.opcode == OpCodes.Callvirt) && code.operand is MethodInfo info
		&& info.DeclaringType.FullName == type && info.Name == method;
	internal static void ExpandBranches(List<CodeInstruction> codes)
	{
		OpCode[] shortCodes = { OpCodes.Br_S, OpCodes.Brfalse_S, OpCodes.Brtrue_S, OpCodes.Beq_S, OpCodes.Bge_S, OpCodes.Bge_Un_S,
			OpCodes.Bgt_S, OpCodes.Bgt_Un_S, OpCodes.Ble_S, OpCodes.Ble_Un_S, OpCodes.Blt_S, OpCodes.Blt_Un_S, OpCodes.Bne_Un_S, OpCodes.Leave_S };
		OpCode[] longCodes = { OpCodes.Br, OpCodes.Brfalse, OpCodes.Brtrue, OpCodes.Beq, OpCodes.Bge, OpCodes.Bge_Un,
			OpCodes.Bgt, OpCodes.Bgt_Un, OpCodes.Ble, OpCodes.Ble_Un, OpCodes.Blt, OpCodes.Blt_Un, OpCodes.Bne_Un, OpCodes.Leave };
		foreach (CodeInstruction code in codes)
		{
			int i = Array.IndexOf(shortCodes, code.opcode);
			if (i >= 0) code.opcode = longCodes[i];
		}
	}
}

