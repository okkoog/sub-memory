using System;
using UnityEngine;

namespace MidnightPawn.Game;

public static class ModSettings
{
	public const string SECTION = "概率设置";
	public const string GAMEPLAY_SECTION = "玩法设置";
	public const string FATIGUE_KEY = "fatigue_enabled";
	public const string FATIGUE_SCALE_KEY = "fatigue_scale";
	public const string BUYER_PER_DAY_KEY = "buyer_per_day";
	public const string CORRUPTION_BOX_SETTLEMENT_KEY = "corruption_box_settlement";
	private static readonly string[] PercentChoices = { "50%", "75%", "100%", "150%", "200%", "300%" };
	private static readonly string[] ToggleChoices = { "开", "关" };
	private static ConfigEntry<string> fatigueEntry, fatigueScaleEntry, buyerPerDayEntry, corruptionBoxSettlementEntry;
	private static ConfigEntry<string> semenBarrelEntry, wondrousBarrelEntry, richBarrelEntry, dilutedBarrelEntry;
	private static bool initialized;

	public static void Init()
	{
		if (initialized) return;
		try
		{
			ModConfig config = ModHelper.GetConfig(ModConst.MOD_ID);
			fatigueEntry = config.Define(FATIGUE_KEY, "开", GAMEPLAY_SECTION, () => "疲劳系统", ToggleChoices,
				() => "按原版实际活动次数追加疲劳，只修正本模组估值。入夜透支时暂停该角色全部夜间活动、双方产出、受孕和孕期（包括分娩），恢复 5 点。关闭后停用疲劳修正及门禁。");
			fatigueScaleEntry = config.Define(FATIGUE_SCALE_KEY, "100%", GAMEPLAY_SECTION, () => "疲劳累积速度", PercentChoices,
				() => "只调整疲劳增长速度。所有设备共用本体等级系数，1～10 级逐级降低，11 级起为 0。不改变原版活动次数或受孕概率。");
			buyerPerDayEntry = config.Define(BUYER_PER_DAY_KEY, "1", SECTION, () => "每日收购商上限", new[] { "1", "2", "3" },
				() => "每位独立按橱窗乳汁种类判定到店概率，3 种 100% / 2 种 66% / 1 种 33%。");
			corruptionBoxSettlementEntry = config.Define(CORRUPTION_BOX_SETTLEMENT_KEY, "关", GAMEPLAY_SECTION, () => "侵蚀区苗床箱参与夜间结算", ToggleChoices,
				() => "直接放在本体侵蚀区的苗床箱内角色调用原版本体结算，产出进入所属本体产出区，计入原版报告与经验，并追加本模组乳汁和疲劳。透支角色整夜暂停。其它容器及深层嵌套不参与。");
			semenBarrelEntry = DefineBarrel(config, "auto_barrel_semen", "触手精液", 8);
			wondrousBarrelEntry = DefineBarrel(config, "auto_barrel_milk_wondrous", "神奇乳汁", 3);
			richBarrelEntry = DefineBarrel(config, "auto_barrel_milk_rich", "浓缩乳汁", 3);
			dilutedBarrelEntry = DefineBarrel(config, "auto_barrel_milk_diluted", "清甜乳汁", 3);
			initialized = true;
		}
		catch (Exception ex) { ModCtx.LogWarning("注册设置失败: " + ex.Message); }
	}
	private static ConfigEntry<string> DefineBarrel(ModConfig config, string key, string name, int threshold) =>
		config.Define(key, "开", GAMEPLAY_SECTION, () => "自动新建桶装：" + name, ToggleChoices,
			() => "夜间追加结算后扫描整个产出区，同类瓶装至少 " + threshold + " 瓶时新建桶装。关闭只禁止自动新建；产出区已有对应桶装仍会自动补充，手动操作保留。");
	private static bool On(ConfigEntry<string> entry, bool fallback = true) =>
		entry == null ? fallback : !string.Equals(entry.Value?.Trim(), "关", StringComparison.Ordinal);
	public static bool FatigueEnabled => On(fatigueEntry);
	public static bool CorruptionBoxSettlement => corruptionBoxSettlementEntry != null
		&& string.Equals(corruptionBoxSettlementEntry.Value?.Trim(), "开", StringComparison.Ordinal);
	internal static bool CreateNightBarrel(string id) => id switch
	{
		ItemFactory.BarrelSemenId => On(semenBarrelEntry),
		ItemFactory.BarrelMilkWondrousId => On(wondrousBarrelEntry),
		ItemFactory.BarrelMilkRichId => On(richBarrelEntry),
		ItemFactory.BarrelMilkDilutedId => On(dilutedBarrelEntry),
		_ => false
	};
	public static float FatigueScale => fatigueScaleEntry?.Value?.Trim() switch
	{
		"宽松" => 0.6f,
		"严苛" => 1.5f,
		_ => ParsePercent(fatigueScaleEntry?.Value) / 100f
	};
	public static int BuyerPerDay => int.TryParse(buyerPerDayEntry?.Value?.Trim(), out int count) ? Mathf.Clamp(count, 1, 5) : 1;
	private static int ParsePercent(string value) => int.TryParse(value?.Trim().TrimEnd('%'), out int result) ? Mathf.Clamp(result, 0, 500) : 100;
}

