using System.Collections.Generic;
using LewdMemeConfig;

// Task 11:LootRules 纯函数语义(掉落接管的前置判定);Task 8 对齐主 mod 1.6.1。
// 语义对照主 mod 1.6.1 MidnightPawn.Game.LootHook.OnLootRolled(反编译 L30-70):
//   医疗组:groupId 小写化后 Contains 任一关键词(原版 ToLower 后逐个 Contains,
//     关键词为小写字面量 → 关键词按原样参与比较,小写化只作用于 groupId);
//   拾荒组:raw groupId 与组名精确相等(原版 ==,大小写敏感、非子串);
//   节流:同组(raw ==)且 now-lastRollTime < 0.5s → 跳过判定(原版 L51-55,
//     首次 lastRollTime=-999 恒不节流;窗口严格小于,恰等于不节流);
//   少女概率:恒定读数(原版 ModSettings.MaidenChance,内部 clamp 0.05-0.25,
//     已不随触手等级——per-level 与 8% 拾荒排卵针分支于 1.3.13/14 移除)。
// LootPatch 依赖主 mod 类型(运行时 ModReflect)与 Harmony,住在 Patches/ 不进测试工程;
// 其接管语义由本纯函数测试 + 游戏内验收共同覆盖。
public static class LootRulesTests
{
    // 与 ConfigRegistry.RegisterLoot 默认串 / LootPatch MedDefault/ScavDefault 逐字相同(1.6.1 字面量)
    private const string MedKw = "med,drug,pharma,chem";
    private const string ScavGs = "dumpingGroundTG,dumpingGroundFreshTG";

    public static void Run()
    {
        var kw = new[] { "med", "drug", "pharma", "chem" };
        var gs = new[] { "dumpingGroundTG", "dumpingGroundFreshTG" };

        // -- IsMedGroup:groupId 小写化后含任一关键词(brief 样例 + 全关键词逐项) --
        Check.Eq(true, LootRules.IsMedGroup("MedKit", kw), "med MedKit (brief 样例,小写化)");
        Check.Eq(false, LootRules.IsMedGroup("weapons", kw), "med weapons -> false (brief 样例)");
        Check.Eq(true, LootRules.IsMedGroup("chemLab", kw), "med chemLab");
        Check.Eq(true, LootRules.IsMedGroup("DrugStore", kw), "med DrugStore");
        Check.Eq(true, LootRules.IsMedGroup("PHARMACRATE", kw), "med PHARMACRATE");
        Check.Eq(true, LootRules.IsMedGroup("bigsupplybox", new[] { "med", "supplybox" }), "med custom keywords hit");
        Check.Eq(false, LootRules.IsMedGroup("medkit", null), "med null keywords -> false");
        Check.Eq(false, LootRules.IsMedGroup(null, kw), "med null groupId -> false(原版 null→\"\"不含任何词)");
        Check.Eq(false, LootRules.IsMedGroup("", kw), "med empty groupId -> false");
        Check.Eq(false, LootRules.IsMedGroup("anything", new string[] { null, "" }), "med null/empty keyword skipped");
        Check.Eq(false, LootRules.IsMedGroup("MedKit", new[] { "MED" }), "med keyword 原样比较(小写化只在 groupId,与原版小写字面量语义一致)");
        Check.Eq(false, LootRules.IsMedGroup("medkit", new string[0]), "med empty keywords -> false");

        // -- InScavGroups:raw groupId 精确等于任一组名(brief 样例 + 大小写/子串/null) --
        Check.Eq(true, LootRules.InScavGroups("dumpingGroundTG", gs), "scav dumpingGroundTG (brief 样例)");
        Check.Eq(true, LootRules.InScavGroups("dumpingGroundFreshTG", gs), "scav dumpingGroundFreshTG");
        Check.Eq(false, LootRules.InScavGroups("dumpingGroundX", gs), "scav dumpingGroundX -> false (brief 样例)");
        Check.Eq(false, LootRules.InScavGroups("DumpingGroundTG", gs), "scav 大小写敏感(原版 ==)");
        Check.Eq(false, LootRules.InScavGroups("dumpingGroundTG_a", gs), "scav 精确匹配非子串");
        Check.Eq(false, LootRules.InScavGroups("medkit", gs), "scav 医疗组名不命中拾荒组");
        Check.Eq(true, LootRules.InScavGroups("custom", new[] { "custom" }), "scav custom group exact");
        Check.Eq(false, LootRules.InScavGroups(null, gs), "scav null groupId -> false");
        Check.Eq(false, LootRules.InScavGroups("dumpingGroundTG", null), "scav null groups -> false");
        Check.Eq(false, LootRules.InScavGroups("dumpingGroundTG", new string[] { null, "" }), "scav null/empty entry skipped");

        // -- Throttled:同组(raw ==)且 now-lastRollTime < window(原版 L51-55,brief 样例 + 边界) --
        Check.Eq(true, LootRules.Throttled("dumpingGroundTG", "dumpingGroundTG", 100f, 99.7f, 0.5f), "throttle 同组 0.3s -> true (brief 样例)");
        Check.Eq(false, LootRules.Throttled("dumpingGroundTG", "dumpingGroundTG", 100f, 99.4f, 0.5f), "throttle 同组 0.6s -> false (brief 样例)");
        Check.Eq(false, LootRules.Throttled("dumpingGroundTG", "dumpingGroundFreshTG", 100f, 100f, 0.5f), "throttle 异组任意时刻 -> false (brief 样例)");
        Check.Eq(false, LootRules.Throttled("dumpingGroundTG", null, 0f, -999f, 0.5f), "throttle 首次(lastRollTime=-999) -> false (brief 样例)");
        Check.Eq(false, LootRules.Throttled("g", "g", 10f, 9.5f, 0.5f), "throttle 差恰等于窗口 -> false(严格小于,同原版 <)");
        // -- Task 8(更多可配置项):loot.throttle_seconds = 0 禁用节流(锁定用例)--
        // now - lastRollTime < 0f 恒 false(时间单调),无特判;0.3s 内 cooldown 0.5 对照见上
        Check.Eq(false, LootRules.Throttled("g", "g", 10f, 10f, 0f), "cooldown 0 disables throttle");
        Check.Eq(false, LootRules.Throttled("g", "g", 10f, 9.99f, 0f), "cooldown 0 disables throttle (any elapsed)");
    }
}
