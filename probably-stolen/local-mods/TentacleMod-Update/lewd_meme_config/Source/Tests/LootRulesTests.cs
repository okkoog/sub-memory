using System.Collections.Generic;
using LewdMemeConfig;

// Task 11:LootRules 纯函数语义(掉落接管的前置判定);Task 8 对齐主 mod 1.6.1;
// 0.8.0:InScavGroups/Throttled 两函数随 2.1.0 拾荒重写退役(帧节流 +
// ScavengeRoll 分档判定,组过滤/时间节流机制死),本测试剩 IsMedGroup
// (med 拦截器唯一消费,语义不变——医疗组:groupId 小写化后 Contains 任一
// 关键词,原版 ToLower 后逐个 Contains,小写化只作用于 groupId)。
// LootPatch 依赖主 mod 类型(运行时 ModReflect)与 Harmony,住在 Patches/ 不进测试工程;
// 其接管语义由本纯函数测试 + 游戏内验收共同覆盖。
public static class LootRulesTests
{
    // 与 ConfigRegistry.RegisterLoot 默认串 / LootPatch MedDefault 逐字相同(1.6.1 字面量)
    private const string MedKw = "med,drug,pharma,chem";

    public static void Run()
    {
        var kw = new[] { "med", "drug", "pharma", "chem" };

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
    }
}
