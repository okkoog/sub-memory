using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using LewdMemeConfig;

// 0.7.0 Task 3:transpiler 基础设施的数据面锁定(brief 三断言 + 结构守卫)。
// 事实源 = 家族勘察摘要「0.7.0 IL 注入点清单」(2026-11-17,基线 lewd_meme_192,
// 46 个 transpile 键:44 标准 ldc→getter + 2 ldloc 特形[T7 交付,占位已解除])。
//  ① 每 Entry.Key 必须存在于 ConfigRegistry 面板注册表(ContainsKey,键面/
//     默认值/类型的单一事实源仍是 ConfigRegistry —— transpile 面不另立键);
//  ② Entry.Default 与 1.9.2 反编译常量逐字相等(下方硬编码期望表逐键抄自
//     勘察摘要 A–F 六节;ovulate_chance 为 1f/7f 折叠产物 0.14285715);
//  ③ Points(methodId) 返回集无重复 Key,且跨 methodId 全局无重复
//     (一个键只允许一个 transpiler 消费;T7 起 Deferred 已清零)。
// 结构守卫:总键数 46;NightlyTick 双单元反查
// (main.nightly_transpile 14 + main.nightly_milk_value 2,让位语义不同故拆);
// Targets 双向一致(TargetOf ↔ MethodIdsFor)。
public static class TranspilePlanTests
{
    public static void Run()
    {
        var cfg = ConfigRegistry.CreateDefault();

        // —— 期望表:键 → 1.9.2 反编译常量(勘察摘要 A–F 节逐键抄录) ——
        var expected = new Dictionary<string, object>
        {
            // A 节 TentacleBody.NightlyTick(main.nightly_transpile 14 键;
            // daughter_value 在 ProduceDaughter,独立 methodId)
            [CfgKeys.SexMinBase] = 5,
            [CfgKeys.SexMaxBase] = 10,
            [CfgKeys.SexPerLevelMax] = 3,
            [CfgKeys.OralMaxBase] = 2,
            [CfgKeys.OralPerLevelMax] = 2,
            [CfgKeys.AnalMaxBase] = 2,
            [CfgKeys.AnalPerLevelMax] = 2,
            [CfgKeys.OvulateChance] = 1f / 7f,
            [CfgKeys.ConceivePerSex] = 0.01f,
            [CfgKeys.GestationDays] = 30,
            [CfgKeys.LactationCap] = 3,
            [CfgKeys.SemenChanceBase] = 0.4f,
            [CfgKeys.SemenChancePerTrain] = 0.01f,
            [CfgKeys.SemenChanceCap] = 1f,
            // A/D 节 ProduceDaughter
            [CfgKeys.DaughterValue] = 900,
            // B 节 乳汁两键(main.nightly_milk_value,让位 Brood)
            [CfgKeys.MilkValueBase] = 100,
            [CfgKeys.MilkValuePerLv] = 40,
            // C 节 CorruptTentacle(T6:max_charge 两点随 on_semen_used 单元)
            [CfgKeys.CorruptCost] = 100,
            [CfgKeys.CorruptSemen] = 10,
            [CfgKeys.CorruptMaxCharge] = 100,
            // D 节 布局(layout.brood/output 为 Deferred 占位,见下)
            [CfgKeys.TabsPerLevel] = 2,
            [CfgKeys.CorruptCols] = 3,
            [CfgKeys.ToolCols] = 3,
            // E 节 maiden 数值段(RollAttr/FactionBase,1.9.2 拆出)
            [CfgKeys.AttrHighChance] = 0.01f,
            [CfgKeys.AttrMidChance] = 0.06f,
            [CfgKeys.AttrHighRange] = "70,80",
            [CfgKeys.AttrMidRange] = "60,70",
            [CfgKeys.AttrLowRange] = "10,60",
            [CfgKeys.FactionBase] = "上层区:100,治安部:50,黑市:50,革命军:50,其他:10",
            // F 节 alarm
            [CfgKeys.AlarmOralPerNight] = 1,
            [CfgKeys.AlarmSexPerNight] = 1,
            [CfgKeys.AlarmAnalPerNight] = 1,
            [CfgKeys.AlarmBroodSize] = 4,
            [CfgKeys.AlarmOutputSize] = 4,
            [CfgKeys.AlarmNpcSellChance] = 0.05f,
            // F 节 trap
            [CfgKeys.TrapBroodSize] = 12,
            [CfgKeys.TrapOutputSize] = 12,
            [CfgKeys.TrapTacticChances] = "0.1,0.2,0.3",
            [CfgKeys.TrapSemenChance] = 0.5f,
            [CfgKeys.TrapNpcSellChanceScav] = 0.1f,
            [CfgKeys.TrapNpcSellChanceThief] = 0.05f,
            // F 节 printer
            [CfgKeys.PrinterGrid] = 12,
            [CfgKeys.PrinterRecipeCost] = 15,
            [CfgKeys.PrinterBaseProgress] = 3f,
            // ldloc special form (T7 delivered: 1.9.2 BuildWindow shared ldc 16 independent-ized to two keys)
            [CfgKeys.BroodSize] = 16,
            [CfgKeys.OutputSize] = 16,
        };

        // —— ① ②:键注册性 + 默认值逐字相等(同时锁总数与键集) ——
        Check.Eq(46, TranspilePlan.Entries.Count, "transpile 键总数 46(T7 ldloc 特形交付后全量)");
        foreach (var e in TranspilePlan.Entries)
        {
            Check.Eq(true, cfg.ContainsKey(e.Key), "① 注册表含键: " + e.Key);
            Check.Eq(true, expected.ContainsKey(e.Key), "② 期望表含键: " + e.Key);
            if (expected.ContainsKey(e.Key))
                Check.Eq(true, Equals(expected[e.Key], e.Default), "② 默认值=1.9.2 常量: " + e.Key);
            // M4(Fix Round 1):表默认值与注册表默认值互验——两默认值源(表 =
            // 1.9.2 常量抄录、注册表 = ConfigRegistry.Register* 段)互为独立
            // 抄录,任一侧手滑即在此红;Current<object> 走默认层(测试无玩家值)
            Check.Eq(true, Equals(e.Default, cfg.Current<object>(e.Key)),
                "M4 表默认=注册表默认: " + e.Key);
        }
        // 反向:期望表无多余键(总数 + 唯一性下已保证,显式锁)
        var seen = new HashSet<string>();
        foreach (var e in TranspilePlan.Entries)
            Check.Eq(true, seen.Add(e.Key), "③ 表内键唯一: " + e.Key);
        Check.Eq(expected.Count, seen.Count, "② 期望表无遗漏/多余(46)");

        // —— Deferred 恰 0(0.7.0 Task 7 解除:layout.brood_size/output_size
        //      的 ldloc 特形落地——1.9.2 BuildWindow 苗床/产出区共享 ldc.i4.s 16
        //      单 ldc,独立化 = 把各区 ctor 前的两条 ldloc.0(隐含 num=16)整体
        //      替换为两组 [ldstr; call GetInt],Match 段长度 2 是首个多指令
        //      ldc/ldloc 等价替换形态) ——
        int deferred = 0;
        foreach (var e in TranspilePlan.Entries)
            if (e.Deferred) deferred++;
        Check.Eq(0, deferred, "Deferred 占位已清零(T7 ldloc 特形交付)");
        Check.Eq(false, IsDeferred(CfgKeys.BroodSize), "已解除: " + CfgKeys.BroodSize);
        Check.Eq(false, IsDeferred(CfgKeys.OutputSize), "已解除: " + CfgKeys.OutputSize);

        // —— ③ Points 无重复 + 并集恰 46 + 非 Deferred 条目结构完整 ——
        var union = new HashSet<string>();
        foreach (string id in TranspilePlan.MethodIds)
        {
            TranspileEntry[] pts = TranspilePlan.Points(id);
            Check.Eq(true, pts != null && pts.Length > 0, "Points 非空: " + id);
            var local = new HashSet<string>();
            foreach (var e in pts)
            {
                Check.Eq(true, local.Add(e.Key), "③ 单元内无重复键: " + id + " / " + e.Key);
                Check.Eq(true, union.Add(e.Key), "③ 跨单元无重复键: " + e.Key);
                Check.Eq(true, e.Points != null && e.Points.Length > 0,
                    "注入点非空: " + e.Key);
                foreach (var p in e.Points)
                    Check.Eq(true, p.Count >= 1, "期望匹配数≥1: " + e.Key);
            }
        }
        Check.Eq(46, union.Count, "Points 并集 = 46(T7 占位解除后全量)");

        // —— 0.7.0 T7:Create 系 methodId 与 HookCatalog 单元 id 同名对齐
        //     (Carrier.Run 的让位过滤按单元 id 查 RouteSnapshot,同名 = 过滤
        //     与登记期裁决同一数据源;改名 7 个,produce_daughter_value/
        //     printer 三名/maiden 两名保持) ——
        string[] renamed =
        {
            "main.create", "main.build_window",
            "main.alarm_nightly_once", "main.create_tentacle_alarm", "main.alarm_try_sell_npc",
            "main.create_trap", "main.trap_nightly_once",
        };
        foreach (string id in renamed)
            Check.Eq(true, ((ICollection<string>)TranspilePlan.MethodIds).Contains(id),
                "T7 methodId 对齐单元 id: " + id);
        Check.Eq("TentacleBody::Create", TranspilePlan.TargetOf("main.create"),
            "T7 create 目标(TentacleBody.Create 页签乘子)");
        Check.Eq("TentacleBody::BuildWindow", TranspilePlan.TargetOf("main.build_window"),
            "T7 build_window 目标");

        // —— 0.7.0 T8:maiden 数值两 methodId(保持 T3 名;新建同名单元对齐,
        //     Carrier.Run 的让位过滤与登记期同源)——
        Check.Eq(true, ((ICollection<string>)TranspilePlan.MethodIds).Contains("main.maiden_roll"),
            "T8 methodId: main.maiden_roll");
        Check.Eq(true, ((ICollection<string>)TranspilePlan.MethodIds).Contains("main.maiden_faction_base"),
            "T8 methodId: main.maiden_faction_base");
        Check.Eq("ItemFactory::RollAttr", TranspilePlan.TargetOf("main.maiden_roll"),
            "T8 maiden_roll 目标(1.9.2 拆出的独立方法)");
        Check.Eq("ItemFactory::FactionBase", TranspilePlan.TargetOf("main.maiden_faction_base"),
            "T8 maiden_faction_base 目标");

        // —— 0.7.0 T7:ldloc 特形结构锁定(layout.brood_size/output_size)——
        //     Match 段 = 连续两条 ldloc.0(1.9.2 BuildWindow num 局部隐含 16,
        //     GameGridInventory ctor 双参),替换 = 两组 [ldstr; call GetInt]
        //     (ReplaceAt 的 DirectInt 按 Match 长度逐条等价替换);两区靠
        //     After 窗口 stloc.2 / stloc.3 区分(brood 槽 2 / output 槽 3),
        //     与 alarm/trap grids 的 stloc 槽位区分先例同款。
        TranspileEntry brood = FindEntry(CfgKeys.BroodSize);
        TranspileEntry output = FindEntry(CfgKeys.OutputSize);
        Check.Eq(true, brood != null && output != null, "T7 brood/output 条目在场");
        if (brood != null && output != null)
        {
            foreach (TranspileEntry e in new[] { brood, output })
            {
                Check.Eq("main.build_window", e.MethodId, "T7 ldloc 特形归属 build_window: " + e.Key);
                Check.Eq(1, e.Points.Length, "T7 单注入点: " + e.Key);
                TranspilePoint p = e.Points[0];
                Check.Eq(2, p.Match.Length, "T7 Match 段长度 2(ldloc 双连): " + e.Key);
                Check.Eq(PointForm.DirectInt, p.Form, "T7 形态 DirectInt(逐条替换): " + e.Key);
                Check.Eq(OpCodes.Ldloc_0.Value, p.Match[0].Opcode.Value,
                    "T7 Match[0] = ldloc.0: " + e.Key);
                Check.Eq(OpCodes.Ldloc_0.Value, p.Match[1].Opcode.Value,
                    "T7 Match[1] = ldloc.0: " + e.Key);
                Check.Eq(true, p.Match[0].Operand == null && p.Match[1].Operand == null,
                    "T7 短形 ldloc 零操作数: " + e.Key);
            }
            Check.Eq(OpCodes.Stloc_2.Value, brood.Points[0].After[1].Opcode.Value,
                "T7 brood 区 After 尾 = stloc.2(苗床区槽位)");
            Check.Eq(OpCodes.Stloc_3.Value, output.Points[0].After[1].Opcode.Value,
                "T7 output 区 After 尾 = stloc.3(产出区槽位)");
        }


        // —— Targets 双向一致 + NightlyTick 双单元反查 ——
        Check.Eq(true, ((ICollection<string>)TranspilePlan.MethodIds).Contains("main.nightly_transpile"),
            "methodId: main.nightly_transpile");
        Check.Eq(true, ((ICollection<string>)TranspilePlan.MethodIds).Contains("main.nightly_milk_value"),
            "methodId: main.nightly_milk_value");
        string nightly = TranspilePlan.TargetOf("main.nightly_transpile");
        Check.Eq("TentacleBody::NightlyTick", nightly, "NightlyTick 目标键");
        Check.Eq(nightly, TranspilePlan.TargetOf("main.nightly_milk_value"),
            "乳汁两键同目标方法");
        string[] nightlyIds = TranspilePlan.MethodIdsFor(nightly);
        Check.Eq(2, nightlyIds.Length, "NightlyTick 挂两个 transpiler 单元");
        foreach (string id in TranspilePlan.MethodIds)
        {
            string t = TranspilePlan.TargetOf(id);
            Check.Eq(true, t != null, "TargetOf 非空: " + id);
            bool found = Array.IndexOf(TranspilePlan.MethodIdsFor(t), id) >= 0;
            Check.Eq(true, found, "MethodIdsFor 反查含自身: " + id);
        }

        // —— 0.7.0 T6:on_semen_used 单元结构(OnSemenUsed 复刻体退役,1.9.2
        //     双参签名 + usedSemen 按瓶去重语义归原方法体照跑 = 零复刻;
        //     注入 = semen_recharge 1 点 + max_charge 2 点[门槛 blt / 截断
        //     sub],max>100 时只注 semen 会在 100 处提前拒收——语义完整性
        //     裁决见勘察摘要 C 节顾虑注记) ——
        Check.Eq(true, ((ICollection<string>)TranspilePlan.MethodIds).Contains("main.on_semen_used"),
            "methodId: main.on_semen_used");
        Check.Eq("CorruptTentacle::OnSemenUsed", TranspilePlan.TargetOf("main.on_semen_used"),
            "OnSemenUsed 目标键");
        Check.Eq(1, TranspilePlan.MethodIdsFor("CorruptTentacle::OnSemenUsed").Length,
            "OnSemenUsed 只挂一个 transpiler 单元");
        TranspileEntry[] osu = TranspilePlan.Points("main.on_semen_used");
        Check.Eq(2, osu.Length, "on_semen_used 两键(semen + max)");
        int osuSemenPts = 0, osuMaxPts = 0;
        foreach (var e in osu)
        {
            foreach (var p in e.Points)
            {
                Check.Eq(PointForm.DirectInt, p.Form, "on_semen_used 注入形态 int: " + e.Key);
                if (e.Key == CfgKeys.CorruptSemen) osuSemenPts++;
                if (e.Key == CfgKeys.CorruptMaxCharge) osuMaxPts++;
            }
        }
        Check.Eq(1, osuSemenPts, "semen_recharge 1 点(Min 首参)");
        Check.Eq(2, osuMaxPts, "max_charge 2 点(门槛 + 截断)");

        // —— DefaultOf 回退通道(getter 的 Config 缺席兜底与表同源) ——
        Check.Eq(900, TranspilePlan.DefaultOf(CfgKeys.DaughterValue), "DefaultOf int");
        Check.Eq(0.01f, TranspilePlan.DefaultOf(CfgKeys.ConceivePerSex), "DefaultOf float");
        Check.Eq("0.1,0.2,0.3", TranspilePlan.DefaultOf(CfgKeys.TrapTacticChances),
            "DefaultOf string");

        // —— 1.6.1 让位翻转·IL 共存重锚锁定(ShadowTrade = lewd_meme_remake
        //      1.6.1 的 TickTranspiler/设备 Transpiler 以 ExpandBranches 把
        //      全方法短跳转扩长;4 个以短跳转为窗口锚的注入点在双装形态下
        //      必 miss → 扰动侧窗口置空,锚定责任移交 Match 唯一值 + 对侧
        //      稳定序列。此块防回退:置空被悄悄改回跳转锚 = 双装必 Dead) ——
        Check.Eq(true, IsWindowCleared(CfgKeys.OvulateChance, after: true),
            "1.6.1 ovulate_chance After 置空(原 bge.un.s 扩长)");
        Check.Eq(true, IsWindowCleared(CfgKeys.GestationDays, after: true),
            "1.6.1 gestation_days After 置空(原 blt.s 扩长)");
        Check.Eq(true, IsWindowCleared(CfgKeys.MilkValueBase, after: false),
            "1.6.1 milk_value_base Before 置空(原 brfalse.s 扩长)");
        Check.Eq(true, IsWindowCleared(CfgKeys.TrapSemenChance, after: true),
            "1.6.1 trap.semen_chance After 置空(原 bge.s 扩长)");
        // 保留侧窗口不空(锚定责任在,若连对侧也空则该点退化为裸值匹配)
        Check.Eq(true, !IsWindowCleared(CfgKeys.OvulateChance, after: false),
            "1.6.1 ovulate_chance Before 保留");
        Check.Eq(true, !IsWindowCleared(CfgKeys.MilkValueBase, after: true),
            "1.6.1 milk_value_base After 保留");
    }

    /// <summary>注入点某侧窗口是否已置空(null/空数组 = 匹配器不检查)。</summary>
    private static bool IsWindowCleared(string key, bool after)
    {
        TranspileEntry e = FindEntry(key);
        if (e == null || e.Points.Length != 1) return false;
        AnchorInstr[] w = after ? e.Points[0].After : e.Points[0].Before;
        return w == null || w.Length == 0;
    }

    private static bool IsDeferred(string key)
    {
        foreach (var e in TranspilePlan.Entries)
            if (e.Key == key) return e.Deferred;
        return false;
    }

    /// <summary>按键名取表条目(测试探针;缺席返回 null)。</summary>
    private static TranspileEntry FindEntry(string key)
    {
        foreach (var e in TranspilePlan.Entries)
            if (e.Key == key) return e;
        return null;
    }
}
