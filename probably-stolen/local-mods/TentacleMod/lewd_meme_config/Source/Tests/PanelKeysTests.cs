using System.Collections.Generic;
using LewdMemeConfig;

// 0.5.0 Task 2(开关体系,作者裁定修订:仅子项开关):面板键定义表完整性锁
// (Task 14b:177 条目 13 分区 = 90 值 + 87 子项开关——解除冻结仅追加「打印机」
// 分区 5 值 + 5 开关;loot.med_* 双键随 1.7.1 适配重定位为「恢复 1.6.x 医疗组
// 排卵针掉落」opt-in,label/tooltip 更新;0.7.0 T8:virgin_chance 两行废弃;
// 0.7.0 T9:exp_per_level 两行废弃,新增公式三键/互动阈值/初始处女五键——
// 183 条目 = 93 值 + 89 子项开关)。值条目断言沿用;开关条目锁表结构
// 与序列;开关键默认值(false)由 ConfigRegistryTests 锁(注册在 ConfigRegistry 侧)。
// 总/分区开关已移除——游戏面板 Apply 模型(控件互相独立),层级联动无法实现。
// 例外(ruling):追加语义 CSV 键(special.extra_* / corrupt.*_extra)注册默认
// 即空串 = 「默认不追加任何项」(原版行为),空串是合法默认,仅断言非 null。
public static class PanelKeysTests
{
    private static readonly HashSet<string> AppendOnlyCsvKeys = new HashSet<string>
    {
        "special.extra_ids", "special.extra_sprites", "special.extra_keywords",
        "corrupt.whitelist_extra", "corrupt.blacklist_extra",
    };

    public static void Run()
    {
        var defaults = ConfigRegistry.CreateDefault();
        Check.Eq(176, PanelKeys.Entries.Count, "176 panel entries(0.8.0: 182 − 退役 3 键[scav_maiden_base/scav_groups/throttle_seconds]×值+开关 = 90 值 + 86 开关)");
        var sectionCounts = new Dictionary<string, int>();
        foreach (var e in PanelKeys.Entries)
        {
            Check.Eq(true, !string.IsNullOrEmpty(e.Section), "section set: " + e.Key);
            Check.Eq(true, !string.IsNullOrEmpty(e.Label), "label set: " + e.Key);
            Check.Eq(true, !string.IsNullOrEmpty(e.Tooltip), "tooltip set: " + e.Key);
            if (PanelKeys.IsSwitchKey(e.Key))
            {
                // 开关条目:Toggle 形态,值校验归 ConfigRegistry(注册侧)
                Check.Eq(PanelKind.Toggle, e.Kind, "switch entry is Toggle: " + e.Key);
            }
            else if (e.Kind == PanelKind.SliderInt)
            {
                int def = defaults.Current<int>(e.Key);
                Check.Eq(true, def >= e.MinInt && def <= e.MaxInt, "int default in range: " + e.Key);
            }
            else if (e.Kind == PanelKind.SliderFloat)
            {
                float def = defaults.Current<float>(e.Key);
                Check.Eq(true, def >= e.MinFloat && def <= e.MaxFloat, "float default in range: " + e.Key);
            }
            else if (e.Kind == PanelKind.Toggle) defaults.Current<bool>(e.Key);
            else if (e.Kind == PanelKind.Choice) Check.Eq(string.Join(",", PlayerChoices.Labels), string.Join(",", e.Choices), "choice uses PlayerChoices: " + e.Key);
            else if (e.Kind == PanelKind.Input)
            {
                string def = defaults.Current<string>(e.Key);
                Check.Eq(true, def != null, "input default not null: " + e.Key);
                if (!AppendOnlyCsvKeys.Contains(e.Key))
                    Check.Eq(true, !string.IsNullOrEmpty(def), "input default non-empty: " + e.Key);
            }
            if (!sectionCounts.ContainsKey(e.Section)) sectionCounts[e.Section] = 0;
            sectionCounts[e.Section]++;
        }
        Check.Eq(13, sectionCounts.Count, "13 sections");
        Check.Eq(12, Get(sectionCounts, "顾客"), "section 顾客 has 12");
        Check.Eq(10, Get(sectionCounts, "价格"), "section 价格 has 10");
        Check.Eq(48, Get(sectionCounts, "夜间结算"), "section 夜间结算 has 48");
        Check.Eq(8, Get(sectionCounts, "成长"), "section 成长 has 8(T9: -exp_per_level +公式三键)");
        Check.Eq(4, Get(sectionCounts, "掉落"), "section 掉落 has 4(0.8.0: 10 − 退役 3 键[官方接管/机制死]×值+开关)");
        Check.Eq(25, Get(sectionCounts, "少女生成"), "section 少女生成 has 25(T9: +互动阈值 +初始处女)");
        Check.Eq(7, Get(sectionCounts, "特殊顾客与名字"), "section 特殊顾客与名字 has 7");
        Check.Eq(14, Get(sectionCounts, "侵蚀触手"), "section 侵蚀触手 has 14");
        Check.Eq(14, Get(sectionCounts, "闹钟"), "section 闹钟 has 14");
        Check.Eq(12, Get(sectionCounts, "陷阱"), "section 陷阱 has 12");
        Check.Eq(8, Get(sectionCounts, "界面"), "section 界面 has 8");
        Check.Eq(4, Get(sectionCounts, "天赋"), "section 天赋 has 4");
        Check.Eq(10, Get(sectionCounts, "打印机"), "section 打印机 has 10 (Task 14b)");

        // -- 开关键命名 API --
        Check.Eq(CfgKeys.PriceMilk + ".enabled", PanelKeys.ItemSwitchKey(CfgKeys.PriceMilk), "item switch key form");
        Check.Eq(true, PanelKeys.IsSwitchKey("price.milk.enabled"), "IsSwitchKey item");
        Check.Eq(false, PanelKeys.IsSwitchKey("price.milk"), "IsSwitchKey plain key false");

        // 子项开关键集:89 个(T9: -exp_per_level +公式三键 +interact_threshold),
        // 排除 Choice/Toggle 4 键(3 旧 + T9 新 initial_virgin——Toggle 自开关反向分派)
        Check.Eq(86, PanelKeys.NumericKeysWithSwitch.Count, "86 keys with item switch(0.8.0: 89 − 3)");
        Check.Eq(false, ContainsKey(PanelKeys.NumericKeysWithSwitch, CfgKeys.GenderScope), "gender_scope has no item switch");
        Check.Eq(false, ContainsKey(PanelKeys.NumericKeysWithSwitch, CfgKeys.GuestDisplayEnabled), "guest_display has no item switch");
        Check.Eq(false, ContainsKey(PanelKeys.NumericKeysWithSwitch, CfgKeys.NamesUseCustomFile), "names_use_custom has no item switch");
        Check.Eq(false, ContainsKey(PanelKeys.NumericKeysWithSwitch, CfgKeys.InitialVirgin), "initial_virgin has no item switch(Toggle 自开关)");
        Check.Eq(true, ContainsKey(PanelKeys.NumericKeysWithSwitch, CfgKeys.PriceMilk), "price.milk has item switch");

        // -- 序列:子项开关紧邻其值前;分区开关/总开关不存在(作者裁定移除)--
        Check.Eq(CfgKeys.GenderScope, PanelKeys.Entries[0].Key, "first entry is gender_scope (no master switch)");
        int idxFrc = FindIndex(CfgKeys.FemaleRewriteChance);
        Check.Eq(PanelKeys.ItemSwitchKey(CfgKeys.FemaleRewriteChance), PanelKeys.Entries[idxFrc - 1].Key, "item switch precedes its value entry");
        int idxCap = FindIndex(CfgKeys.LevelCap);
        Check.Eq(PanelKeys.ItemSwitchKey(CfgKeys.LevelCap), PanelKeys.Entries[idxCap - 1].Key, "level_cap switch precedes value");
        int idxGd = FindIndex(CfgKeys.GuestDisplayEnabled);
        Check.Eq(false, PanelKeys.IsSwitchKey(PanelKeys.Entries[idxGd - 1].Key), "toggle value has no preceding item switch");

        // -- 1.6.1 / 0.4.0 遗留断言(值条目不变;0.8.0:scav_maiden_base/
        //    throttle_seconds 两键退役删除,断言改为不存在) --
        Check.Eq(true, Find("loot.scav_shot_chance") == null, "scav_shot_chance removed from panel");
        Check.Eq(true, Find("loot.scav_maiden_base") == null, "0.8.0 scav_maiden_base removed(官方写透接管)");
        Check.Eq(true, Find("loot.throttle_seconds") == null, "0.8.0 throttle_seconds removed(2.1.0 帧节流机制死)");
        Check.Eq(true, Find("loot.scav_groups") == null, "0.8.0 scav_groups removed(2.1.0 无组过滤)");
        var lc = Find(CfgKeys.LevelCap);
        Check.Eq(PanelKind.SliderInt, lc.Kind, "level_cap is SliderInt");
        Check.Eq(1, lc.MinInt, "level_cap MinInt 1");
        Check.Eq(999, lc.MaxInt, "level_cap MaxInt 999(T9: 1.9.2 升级循环无上限拦截,读 clamp 封顶)");
        Check.Eq(true, lc.Tooltip.Contains("等级上限"), "level_cap tooltip mentions 等级上限");
        var ic = Find(CfgKeys.CorruptInitialCharge);
        Check.Eq(PanelKind.SliderInt, ic.Kind, "initial_charge is SliderInt");
        Check.Eq(0, ic.MinInt, "initial_charge MinInt 0");
        Check.Eq(999999, ic.MaxInt, "initial_charge MaxInt 999999 (上限已放宽)");
        Check.Eq(PanelKind.Input, Find(CfgKeys.TrapTacticChances).Kind, "tactic_chances is Input");
        Check.Eq(true, Find("layout.brood_size") == null, "layout.brood_size not on panel");

        // -- 乳汁注明(remake/Brood 在场不生效) --
        Check.Eq(true, Find(CfgKeys.PriceMilk).Tooltip.Contains("不生效"), "price.milk tooltip notes ineffective-with-remake");
        Check.Eq(true, Find(CfgKeys.PriceMilkCredit).Tooltip.Contains("不生效"), "price.milk_credit tooltip notes ineffective-with-remake");

        // -- Task 14b:med 双键重定位为「恢复 1.6.x 医疗组排卵针掉落」opt-in
        //    (1.7.0 起原版已移除;开关默认关 = 1.9.2 原版零漂移,LootHook 1.9.2 零改动)--
        var medSw = Find(PanelKeys.ItemSwitchKey(CfgKeys.MedShotChance));
        Check.Eq(true, medSw != null, "med_shot_chance switch entry exists");
        Check.Eq(true, medSw != null && medSw.Label.StartsWith("恢复:医疗组排卵针掉落"),
            "med_shot_chance switch label carries restore semantics");
        Check.Eq(true, medSw != null && medSw.Tooltip.Contains("2.1.0"), "med_shot_chance switch tooltip names 2.1.0 vanilla(0.8.0)");
        var medKwSw = Find(PanelKeys.ItemSwitchKey(CfgKeys.MedKeywords));
        Check.Eq(true, medKwSw != null && medKwSw.Label.StartsWith("恢复:医疗组排卵针掉落"),
            "med_keywords switch label carries restore semantics");
        Check.Eq(true, medKwSw != null && medKwSw.Tooltip.Contains("2.1.0"), "med_keywords switch tooltip names 2.1.0 vanilla(0.8.0)");

        // -- Task 14b:printer 分区五行值键(默认 = 1.7.1 字面量)--
        Check.Eq(PanelKind.SliderInt, Find(CfgKeys.PrinterPrice).Kind, "printer.price is SliderInt");
        Check.Eq(500, defaults.Current<int>(CfgKeys.PrinterPrice), "printer.price default 500");
        Check.Eq(15, defaults.Current<int>(CfgKeys.PrinterNeedlePrice), "printer.needle_price default 15");
        Check.Eq(12, defaults.Current<int>(CfgKeys.PrinterGrid), "printer.grid default 12");
        Check.Eq(12, Find(CfgKeys.PrinterGrid).MinInt, "printer.grid MinInt 12(F6:ResolveGrids 扫描上界 12,小于 12 材料识别失败)");
        Check.Eq(15, defaults.Current<int>(CfgKeys.PrinterRecipeCost), "printer.recipe_cost default 15");
        Check.Eq(3f, defaults.Current<float>(CfgKeys.PrinterBaseProgress), "printer.base_progress default 3f");
        Check.Eq(true, Find(CfgKeys.PrinterGrid).Tooltip.Contains("新造"), "printer.grid tooltip notes new-machines-only");

        // -- 0.7.0 T5:corrupt_cost 边界注记(消耗 > 充能上限时充能永远不够,
        //    门槛恒拒 = 侵蚀永不触发) --
        Check.Eq(true, Find(CfgKeys.CorruptCost).Tooltip.Contains("永不触发"),
            "corrupt_cost tooltip notes never-triggers-above-max");

        // -- 0.7.0 T9 tooltip 注记三件(brief 前置遗留)--
        // ① T2 放弃 6 键:1.9.2 数值已内联/无条件,暂不可调(面板行保留)
        Check.Eq(true, Find(CfgKeys.SexPerLevelMin).Tooltip.Contains("暂不可调"),
            "T9: sex_per_level_min tooltip notes folded-in-1.9.2");
        Check.Eq(true, Find(CfgKeys.OralMinBase).Tooltip.Contains("暂不可调"),
            "T9: oral_min_base tooltip notes folded-in-1.9.2");
        Check.Eq(true, Find(CfgKeys.OralPerLevelMin).Tooltip.Contains("暂不可调"),
            "T9: oral_per_level_min tooltip notes folded-in-1.9.2");
        Check.Eq(true, Find(CfgKeys.AnalMinBase).Tooltip.Contains("暂不可调"),
            "T9: anal_min_base tooltip notes folded-in-1.9.2");
        Check.Eq(true, Find(CfgKeys.AnalPerLevelMin).Tooltip.Contains("暂不可调"),
            "T9: anal_per_level_min tooltip notes folded-in-1.9.2");
        Check.Eq(true, Find(CfgKeys.AlarmSemenOutput).Tooltip.Contains("暂不可调"),
            "T9: alarm.semen_output tooltip notes unconditional-in-1.9.2");
        // ② T6:max_charge > 100 需同开读取/写入两开关(否则充能实际封顶 100)
        Check.Eq(true, Find(CfgKeys.CorruptMaxCharge).Tooltip.Contains("读取充能"),
            "T9: max_charge tooltip notes get/set switch pair requirement");
        // ③ T8:主面板 attr 五键 ↔ IPH 分区属性五键互指(可同开,后处理恒胜)
        Check.Eq(true, Find(CfgKeys.AttrHighChance).Tooltip.Contains("后处理恒胜"),
            "T9: attr_high_chance tooltip notes iph-section co-enable");
        Check.Eq(true, Find(CfgKeys.AttrLowRange).Tooltip.Contains("后处理恒胜"),
            "T9: attr_low_range tooltip notes iph-section co-enable");
    }

    private static bool ContainsKey(IReadOnlyList<string> list, string key)
    {
        foreach (var k in list) if (k == key) return true;
        return false;
    }

    private static PanelEntry Find(string key)
    {
        foreach (var e in PanelKeys.Entries)
            if (e.Key == key) return e;
        return null;
    }

    private static int FindIndex(string key)
    {
        for (int i = 0; i < PanelKeys.Entries.Count; i++)
            if (PanelKeys.Entries[i].Key == key) return i;
        return -1;
    }

    private static int Get(Dictionary<string, int> d, string key)
    {
        return d.TryGetValue(key, out var n) ? n : 0;
    }
}
