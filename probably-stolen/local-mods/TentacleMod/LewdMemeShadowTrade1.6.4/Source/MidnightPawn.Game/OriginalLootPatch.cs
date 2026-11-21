using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace MidnightPawn.Game;

/// <summary>每个有效掉落独立判定捕获，移除按游戏时间及掉落组限流的区段。</summary>
internal static class OriginalLootPatch
{
	private const string Owner = "lewd_meme_remake.compat.loot";
	internal static bool Active { get; private set; }
	internal static void Uninstall()
	{
		CompatOriginal.UnpatchOwner(Owner);
		Active = false;
	}

	internal static int Install()
	{
		Harmony harmony = new Harmony(Owner);
		try
		{
			Type type = CompatOriginal.OriginalType("MidnightPawn.Game.LootHook");
			if (type == null) return 0;
			MethodBase target = ModReflect.Target(type, "OnLootRolled", new[] { typeof(LootRollContext) });
			harmony.Patch(target, transpiler: new HarmonyMethod(AccessTools.Method(typeof(OriginalLootPatch), nameof(Transpiler))));
			Active = true;
			ModCtx.Log("拾荒捕获时间限流已移除：每个有效掉落独立按原版概率判定，本体要求保持原版");
			return 1;
		}
		catch (Exception ex)
		{
			harmony.UnpatchAll(Owner);
			Active = false;
			ModCtx.LogWarning("拾荒限流移除失败，保留原版捕获规则: " + ex.Message);
			return 0;
		}
	}

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
	{
		if (__originalMethod.DeclaringType.FullName != "MidnightPawn.Game.LootHook"
			|| __originalMethod.Name != "OnLootRolled" || !__originalMethod.IsStatic)
			throw new InvalidOperationException("拾荒补丁目标不匹配");
		List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
		int start = codes.FindIndex(c => c.opcode == OpCodes.Call && c.operand is MethodInfo m
			&& m.DeclaringType == typeof(UnityEngine.Time) && m.Name == "get_time");
		const int length = 18;
		if (start < 0 || start + length >= codes.Count
			|| codes.FindIndex(start + 1, c => c.operand is MethodInfo m && m.DeclaringType == typeof(UnityEngine.Time) && m.Name == "get_time") >= 0)
			throw new InvalidOperationException("拾荒时间锚点缺失或不唯一");
		// 1.7.1 删除医疗掉落分支后，局部变量由 string/float/float 缩为 float/float。
		// 只接受已核验的两种布局，冷却区段和控制流仍逐条严格验证。
		IList<LocalVariableInfo> locals = __originalMethod.GetMethodBody().LocalVariables;
		bool compact = locals.Count == 2 && locals[0].LocalType == typeof(float) && locals[1].LocalType == typeof(float);
		bool legacy = locals.Count == 3 && locals[0].LocalType == typeof(string)
			&& locals[1].LocalType == typeof(float) && locals[2].LocalType == typeof(float);
		if (!compact && !legacy) throw new InvalidOperationException("拾荒局部变量布局改变");
		string storeTime = compact ? "stloc.0" : "stloc.1";
		string loadTime = compact ? "ldloc.0" : "ldloc.1";
		string[] expected = { "call", storeTime, "ldarg.0", "ldfld", "ldsfld", "call", "brfalse.s", loadTime,
			"ldsfld", "sub", "ldc.r4", "bge.un.s", "ret", "ldarg.0", "ldfld", "stsfld", loadTime, "stsfld" };
		for (int i = 0; i < length; i++)
			if (codes[start + i].opcode.Name != expected[i] || codes[start + i].blocks.Count != 0)
				throw new InvalidOperationException("拾荒限流区段结构改变");
		foreach (int i in new[] { 3, 14 })
			if (!(codes[start + i].operand is FieldInfo f) || f.DeclaringType != typeof(LootRollContext) || f.Name != "groupId")
				throw new InvalidOperationException("拾荒掉落组字段改变");
		foreach (int i in new[] { 4, 8, 15, 17 })
		{
			string name = i == 4 || i == 15 ? "lastGroupId" : "lastRollTime";
			if (!(codes[start + i].operand is FieldInfo f) || f.DeclaringType != __originalMethod.DeclaringType || f.Name != name)
				throw new InvalidOperationException("拾荒冷却字段改变");
		}
		if (!(codes[start + 5].operand is MethodInfo equals) || equals.DeclaringType != typeof(string) || equals.Name != "op_Equality"
			|| !Equals(codes[start + 10].operand, 0.5f)
			|| !(codes[start + 6].operand is Label first) || !codes[start + 13].labels.Contains(first)
			|| !(codes[start + 11].operand is Label second) || !codes[start + 13].labels.Contains(second)
			|| codes[start + length].opcode != OpCodes.Call || !(codes[start + length].operand is MethodInfo body)
			|| body.DeclaringType.Assembly != __originalMethod.DeclaringType.Assembly
			|| body.DeclaringType.FullName != "MidnightPawn.Game.TentacleBody" || body.Name != "HasBody")
			throw new InvalidOperationException("拾荒冷却边界或本体检查改变");
		// 保留指令对象及标签，避免此前进入冷却区段的白名单分支失去目标。
		for (int i = start; i < start + length; i++)
		{
			codes[i].opcode = OpCodes.Nop;
			codes[i].operand = null;
		}
		return codes;
	}
}
