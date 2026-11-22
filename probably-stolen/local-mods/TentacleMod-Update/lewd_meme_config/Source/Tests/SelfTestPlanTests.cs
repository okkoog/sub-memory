using System;
using System.Collections.Generic;
using System.Reflection;
using LewdMemeConfig;

// 自检目录契约测试(Task 14,R-U 测试契约):目录是「所有选项所有值」验收的
// 单一事实源——四条契约锁死覆盖与形态;另含 R-W HookInstaller.States 观察口。
public static class SelfTestPlanTests
{
    public static void Run()
    {
        CatalogBijection();
        RouteAuditCoversAllUnits();
        MechanismAndExecutorValid();
        AggregateCorrect();
        InstallerStatesSnapshot();
    }

    /// <summary>契约 ①:值键行 ↔ (HookCatalog 全部单元 Keys ∪ FamilyPanelKeys
    /// 全部 OurKey)双射——每键恰一行,无遗漏无多余;新键漏登记即红。</summary>
    private static void CatalogBijection()
    {
        var expected = new HashSet<string>(StringComparer.Ordinal);
        foreach (HookUnitDef u in HookCatalog.Units)
            foreach (string k in u.Keys)
                expected.Add(k);
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            expected.Add(e.OurKey);

        var actual = new HashSet<string>(StringComparer.Ordinal);
        int valueRows = 0;
        foreach (KeyTest t in SelfTestPlan.KeyTests)
        {
            if (t.Mechanism == TestMechanism.RouteAudit) continue;
            if (t.Mechanism == TestMechanism.LegacyOverlay) continue; // R13b:伪键行不入双射(键不在配置键空间)
            valueRows++;
            actual.Add(t.Key);
        }
        Check.Eq(expected.SetEquals(actual), true, "selftest 目录值键行 = 目录∪镜像键集合(双射)");
        Check.Eq(actual.Count, valueRows, "selftest 值键行无重复(每键恰一行)");
        Check.Eq(valueRows, expected.Count, "selftest 值键行数 = 覆盖集大小");
        // R13b:LegacyOverlay 伪键行不与任何配置键撞名(防未来键空间扩展踩前缀)
        foreach (KeyTest t in SelfTestPlan.KeyTests)
            if (t.Mechanism == TestMechanism.LegacyOverlay)
                Check.Eq(false, expected.Contains(t.Key),
                    "LegacyOverlay 伪键不撞配置键(" + t.Key + ")");
    }

    /// <summary>契约 ①另一半:RouteAudit 行数 = 目录单元数扣除 WriteThrough
    /// (0.06 轮 F5:写透单元无 install 委托,期望/实际恒 NotRegistered 的恒真
    /// 死行不再生成——与值键行[写透往返]统一为一种形态),单元 id 全覆盖。</summary>
    private static void RouteAuditCoversAllUnits()
    {
        var expected = new HashSet<string>(StringComparer.Ordinal);
        int auditable = 0;
        foreach (HookUnitDef u in HookCatalog.Units)
        {
            if (u.Kind == CarrierKind.WriteThrough) continue;
            expected.Add(u.Id);
            auditable++;
        }

        var audited = new HashSet<string>(StringComparer.Ordinal);
        foreach (KeyTest t in SelfTestPlan.KeyTests)
        {
            if (t.Mechanism != TestMechanism.RouteAudit) continue;
            Check.Eq(t.Key.StartsWith(SelfTestPlan.RoutePrefix, StringComparison.Ordinal),
                true, "RouteAudit 行 Key 带 route: 前缀(" + t.Key + ")");
            audited.Add(t.Key.Substring(SelfTestPlan.RoutePrefix.Length));
        }
        Check.Eq(auditable, audited.Count, "RouteAudit 行数 = 可审计单元数(单元数 - WriteThrough)");
        Check.Eq(expected.SetEquals(audited), true, "RouteAudit 覆盖全部可审计单元 id");
        // F5 反向钉住:任何 WriteThrough 单元 id 不再出现在审计行(防回归)
        foreach (HookUnitDef u in HookCatalog.Units)
            if (u.Kind == CarrierKind.WriteThrough)
                Check.Eq(false, audited.Contains(u.Id),
                    "F5: 写透单元无审计行(与写透往返值键行统一为一种形态): " + u.Id);
    }

    /// <summary>契约 ②③:机制 ∈ 枚举;ExecutorId 非空且前缀 ∈ 白名单
    /// (注册表在 Integration 侧,纯侧只锁格式与已知前缀);RequiresPresent
    /// 每段 ∈ FamilyMods 绑定表(防 typo);机制分布锁(InvokeStatic 30
    /// [R13b +brothel 组合键 10;0.7.0 T11 +exp 三键/interact_threshold;
    /// ST5 +ValueMul 四档直测] /
    /// SpawnItem 17[R10 +iph.attr_* 5;T11 +initial_virgin] /
    /// WriteThrough 36[ST4: remake 8−3+4 + Brood 12 + IPH 16] /
    /// LegacyOverlay 7[R13b],动态 = 各集大小)
    /// 防无意识漂移。</summary>
    private static void MechanismAndExecutorValid()
    {
        string[] prefixes = { "route.audit", "invoke.", "spawn.", "wt.", "legacy.", "read." };
        var knownMods = new HashSet<string>(FamilyMods.BoundVersions.Keys, StringComparer.Ordinal);
        var distribution = new Dictionary<TestMechanism, int>();

        foreach (KeyTest t in SelfTestPlan.KeyTests)
        {
            Check.Eq(Enum.IsDefined(typeof(TestMechanism), t.Mechanism), true,
                "机制合法枚举(" + t.Key + ")");
            Check.Eq(string.IsNullOrEmpty(t.ExecutorId), false,
                "ExecutorId 非空(" + t.Key + ")");
            bool prefixOk = false;
            foreach (string p in prefixes)
                if (t.ExecutorId.StartsWith(p, StringComparison.Ordinal)) { prefixOk = true; break; }
            Check.Eq(prefixOk, true, "ExecutorId 前缀白名单(" + t.Key + " → " + t.ExecutorId + ")");
            if (t.RequiresPresent != null)
            {
                foreach (string mod in t.RequiresPresent.Split(','))
                    Check.Eq(knownMods.Contains(mod.Trim()), true,
                        "RequiresPresent 为已知 mod id(" + t.Key + " → " + mod + ")");
            }
            if (!distribution.ContainsKey(t.Mechanism)) distribution[t.Mechanism] = 0;
            distribution[t.Mechanism]++;
        }
        // 分布锁:机制集与 SelfTestPlan 表头注释三方一致(增删键须同步改三处)。
        // F5:RouteAudit = 可审计单元数(单元总数 - WriteThrough;0.7.1 后
        // 274 行 = 76 route + 30 invoke + 17 spawn + 36 wt + 7 legacy + 108 read)
        int auditableUnits = 0;
        foreach (HookUnitDef u in HookCatalog.Units)
            if (u.Kind != CarrierKind.WriteThrough) auditableUnits++;
        Check.Eq(distribution.ContainsKey(TestMechanism.RouteAudit)
            ? distribution[TestMechanism.RouteAudit] : 0, auditableUnits,
            "机制分布:RouteAudit = 单元数 - WriteThrough(F5 后恒真死行清理)");
        Check.Eq(distribution.ContainsKey(TestMechanism.InvokeStatic)
            ? distribution[TestMechanism.InvokeStatic] : 0, SelfTestPlan.InvokeStaticKeys.Count,
            "机制分布:InvokeStatic = InvokeStaticKeys(R13b 后 22:12 + brothel 组合键 10;"
                + "0.7.0 T11 +exp 公式三键 +interact_threshold = 26;ST5 +ValueMul 四档直测 = 30)");
        Check.Eq(distribution.ContainsKey(TestMechanism.InvokeStatic)
            ? distribution[TestMechanism.InvokeStatic] : 0, 30,
            "机制分布字面锁:InvokeStatic = 30(ST 1.6.1 Task 5;键集动态锁之上防无意识漂移)");
        Check.Eq(distribution.ContainsKey(TestMechanism.SpawnItem)
            ? distribution[TestMechanism.SpawnItem] : 0, SelfTestPlan.SpawnItemKeys.Count,
            "机制分布:SpawnItem = SpawnItemKeys(R10 +iph.attr_* 5,五键共用一生成路径;"
                + "0.7.0 T11 +initial_virgin = 17)");
        Check.Eq(distribution.ContainsKey(TestMechanism.SpawnItem)
            ? distribution[TestMechanism.SpawnItem] : 0, 17,
            "机制分布字面锁:SpawnItem = 17(0.7.0 T11;initial_virgin 生成断言双 tag)");
        Check.Eq(distribution.ContainsKey(TestMechanism.LegacyOverlay)
            ? distribution[TestMechanism.LegacyOverlay] : 0, SelfTestPlan.LegacyOverlayKeys.Count,
            "机制分布:LegacyOverlay = LegacyOverlayKeys(R13b,7 行)");
        int wtExpected = 0;
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            if (e.Carrier == CarrierKind.WriteThrough) wtExpected++;
        Check.Eq(distribution.ContainsKey(TestMechanism.WriteThrough)
            ? distribution[TestMechanism.WriteThrough] : 0, wtExpected,
            "机制分布:WriteThrough = 镜像表 WriteThrough 条目数(ST4 后 36:remake 8 + Brood 12 + IPH 16)");
        // ReadPath = 总目录行 - RouteAudit - 四个 Tier2 机制(值键行兜底数连锁)
        Check.Eq(distribution.ContainsKey(TestMechanism.ReadPath)
            ? distribution[TestMechanism.ReadPath] : 0,
            SelfTestPlan.KeyTests.Count - auditableUnits
                - SelfTestPlan.InvokeStaticKeys.Count - SelfTestPlan.SpawnItemKeys.Count
                - wtExpected - SelfTestPlan.LegacyOverlayKeys.Count,
            "机制分布:ReadPath = 目录总行 - RouteAudit - InvokeStatic - SpawnItem - WriteThrough - LegacyOverlay");
        // T10:自检总行数回归锁(ST 1.6.1 Task 5 目录快照 = 75 route + 30 invoke
        // + 17 spawn + 36 wt + 7 legacy + 106 read;AGENTS/验收文档引用此数,
        // 防目录联动漂移。ST5 算术:263 + 2 route(fatigue_value_mul/
        // fatigue_nightly_recover 两新单元)+ 6 值键行(疲劳六键:4 直测
        // + 2 ReadPath)= 271;0.7.1 拾取器:271 + 1 route(brood.tag_picker)
        // + 2 ReadPath(拾取器两键)= 274;0.8.0 主 mod 官方写透 3 键 = 277)
        Check.Eq(277, SelfTestPlan.KeyTests.Count,
            "自检总行数 0.8.0(277 项;273 + tearl.aquashard 1 route + Aquashard 3 值键行)");
    }

    /// <summary>契约 ④:报告聚合正确(pass/fail 计数、SKIP 归属、失败清单)。</summary>
    private static void AggregateCorrect()
    {
        var results = new[]
        {
            new TestResult { Id = "k1", Section = "s", Pass = true, Detail = "ok" },
            new TestResult { Id = "k2", Section = "s", Pass = true, Detail = "SKIP: 目标缺席" },
            new TestResult { Id = "k3", Section = "s", Pass = false, Detail = "期望 X 实际 Y" },
            null,
        };
        TestReport report = SelfTestPlan.Aggregate(results);
        Check.Eq(report.Total, 3, "聚合:Total 不计 null 条目");
        Check.Eq(report.Passed, 2, "聚合:Passed = 2");
        Check.Eq(report.Failed, 1, "聚合:Failed = 1");
        Check.Eq(report.Skipped, 1, "聚合:Skipped ⊆ Passed");
        Check.Eq(report.Failures.Count, 1, "聚合:失败清单恰一条");
        Check.Eq(report.Failures[0].Id, "k3", "聚合:失败清单成员");
    }

    /// <summary>R-W:HookInstaller.States 只读快照观察口(路由审计/调试面板
    /// 消费)——登记即 Absent、ReconcileAll 后按开关 Installed、未登记单元
    /// 查询回 Absent、UninstallAll 复位。</summary>
    private static void InstallerStatesSnapshot()
    {
        var inst = new HookInstaller(
            k => k == "selftest.on",
            _ => false);
        var u1 = new HookUnit { Id = "selftest.u1", Keys = new[] { "selftest.on" },
            Install = _ => new MethodBase[0] };
        var u2 = new HookUnit { Id = "selftest.u2", Keys = new[] { "selftest.off" },
            Install = _ => new MethodBase[0] };
        inst.Register(u1);
        inst.Register(u2);
        Check.Eq(inst.States["selftest.u1"], HookState.Absent, "States:登记即 Absent");

        inst.ReconcileAll((_, __) => { }, null);
        Check.Eq(inst.States["selftest.u1"], HookState.Installed, "States:开关开 → Installed");
        Check.Eq(inst.States["selftest.u2"], HookState.Absent, "States:开关关 → Absent");

        inst.UninstallAll(null);
        Check.Eq(inst.States["selftest.u1"], HookState.Absent, "States:终局撤装复位 Absent");
        Check.Eq(inst.States["selftest.u2"], HookState.Absent, "States:终局撤装复位 Absent(未装单元)");
    }
}
