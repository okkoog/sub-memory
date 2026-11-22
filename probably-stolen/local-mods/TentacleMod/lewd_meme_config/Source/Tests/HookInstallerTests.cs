using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LewdMemeConfig;

// Task 2:HookInstaller 状态机(brief 7 用例 ①–⑦ + Fix Round 1 用例 ⑧,
// 语义 = spec §5.2/§5.3/§5.7/§4.2;⑨ = 0.6.0 验收 R1 热修复 F2:期望态求值
// 异常的逐单元隔离)。
// 与 PatchHubTests 同一边界约定:Harmony 实例一律传 null(假 install 不使用它),
// UninstallAll/对账卸载在 null harmony 下只清记账不撤销(实现有守卫,同 PatchHub
// 先例)——net8 测试 runner 上游戏自带 0Harmony 2.4.2 的 MonoMod wrapper 生成不可用
// (已诊断:ILGenerator.MarkSequencePoint 缺失),真实装卸语义由 Task 14 游戏内验收。
// 因此断言走三条真实行为通道:install 委托调用计数、状态迁移上报序列、
// 记账清理后的重装行为;另以引用相等钉住 Harmony 实例原样转交契约(②)。
public static class HookInstallerTests
{
    private const string TestId = "lewd_meme_config.tests.hookinstaller";

    public static void Run()
    {
        Case1_AllOffReconcileDoesNotInstall();
        Case2_AnyKeyOnInstallsIdempotent();
        Case3_InstallThenUninstallBothPaths();
        Case4_DeadStickyUntilExplicitOffOn();
        Case5_SetDesiredBeforeAssemblyOnlyRecords();
        Case6_VersionBlockedNeverInstalls();
        Case7_SetDesiredNeverWritesSwitchSource();
        Case8_SharedOriginalRefcountsUninstall();
        Case9_SwitchOnThrowIsolatesUnit();
        Case10_TranspileMismatchDeadRetriable();
    }

    // —— 记账载荷用的真实 MethodBase(不真 Patch,仅证明清单随单元流转)——

    private static class Target
    {
        public static void A() { }
        public static void B() { }
        public static void C() { }

        public static readonly MethodBase AM = typeof(Target).GetMethod(nameof(A));
        public static readonly MethodBase BM = typeof(Target).GetMethod(nameof(B));
        public static readonly MethodBase CM = typeof(Target).GetMethod(nameof(C));
    }

    /// <summary>可变开关数据源 spy:读走字典;唯一的写通道记录写日志——
    /// 断言「实现不回写数据源」用(⑦)。</summary>
    private sealed class SwitchSource
    {
        public readonly Dictionary<string, bool> Values = new Dictionary<string, bool>();
        public readonly List<string> Writes = new List<string>();

        public SwitchSource(params (string key, bool on)[] init)
        {
            foreach (var (key, on) in init) Values[key] = on;
        }

        public Func<string, bool> Reader() => key => Values.TryGetValue(key, out var v) && v;

        /// <summary>测试专用写通道;实现代码拿不到它,任何写都必须留下日志。</summary>
        public void Set(string key, bool on)
        {
            Writes.Add(key + "=" + on);
            Values[key] = on;
        }
    }

    /// <summary>状态迁移上报 spy:记录 (unitId, 新状态) 序列。</summary>
    private sealed class ReportSpy
    {
        public readonly List<(string Unit, HookState State)> Fired = new List<(string, HookState)>();

        public Action<HookUnit, HookState> Handler => (u, s) => Fired.Add((u.Id, s));

        public HookState Last => Fired[Fired.Count - 1].State;
    }

    private static HookUnit Unit(string id, string[] keys, Func<Harmony, MethodBase[]> install)
    {
        return new HookUnit { Id = id, Keys = keys, Install = install };
    }

    // ①注册后全关 ReconcileAll 不调 install(不装、无状态上报、重复对账仍如此)。
    private static void Case1_AllOffReconcileDoesNotInstall()
    {
        var source = new SwitchSource(("k1", false), ("k2", false));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int calls = 0;
        Func<Harmony, MethodBase[]> counting = _ => { calls++; return new MethodBase[0]; };
        installer.Register(Unit("u.off1", new[] { "k1" }, counting));
        installer.Register(Unit("u.off2", new[] { "k2", "k3" }, counting));
        var spy = new ReportSpy();
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(0, calls, "① 全关:install 委托未被调用");
        Check.Eq(0, spy.Fired.Count, "① 全关:无状态迁移上报");
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(0, calls, "① 重复对账:仍不装");
        Check.Eq(0, spy.Fired.Count, "① 重复对账:仍无上报");
    }

    // ②任一键开→装:多键单元任一键开即应装;Harmony 实例原样转交;幂等
    //(重复对账不重复装、无新上报);UninstallAll 清记账后同开关态可重装。
    private static void Case2_AnyKeyOnInstallsIdempotent()
    {
        var source = new SwitchSource(("a", true), ("b", false));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int calls = 0;
        Harmony received = null;
        var harmony = new Harmony(TestId);
        installer.Register(new HookUnit
        {
            Id = "u.multi",
            Keys = new[] { "a", "b" },
            Install = h => { calls++; received = h; return new MethodBase[] { Target.AM }; },
        });
        var spy = new ReportSpy();
        installer.ReconcileAll(spy.Handler, harmony);
        Check.Eq(1, calls, "② 任一键开:install 委托执行一次");
        Check.Eq(("u.multi", HookState.Installed), spy.Fired[0], "② 上报 (unit, Installed)");
        Check.Eq(true, (object)received == (object)harmony, "② Harmony 实例原样转交 install 委托");
        installer.ReconcileAll(spy.Handler, harmony);
        Check.Eq(1, calls, "② 幂等:重复对账不重复装");
        Check.Eq(1, spy.Fired.Count, "② 幂等:无新上报");
        installer.UninstallAll(null); // 撤销本体归 Task 14;此处只清记账复位状态
        installer.ReconcileAll(spy.Handler, harmony);
        Check.Eq(2, calls, "② UninstallAll 清记账后可重装");
        Check.Eq(2, spy.Fired.Count, "② 重装再次上报 Installed");
        Check.Eq(HookState.Installed, spy.Last, "② 重装后仍 Installed");
    }

    // ③全关→装后卸:两条卸载路径都按记账清理——装配期对账(数据源全关 +
    // ReconcileAll)与 UninstallAll(记账非空);卸后重装证明记账已清、状态已复位。
    private static void Case3_InstallThenUninstallBothPaths()
    {
        var source = new SwitchSource(("a", true));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int calls = 0;
        installer.Register(new HookUnit
        {
            Id = "u.cycle",
            Keys = new[] { "a" },
            Install = _ => { calls++; return new MethodBase[] { Target.BM, Target.CM }; },
        });
        var spy = new ReportSpy();
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, calls, "③ 初装");
        Check.Eq(HookState.Installed, spy.Last, "③ 上报 Installed");
        // 卸载路径一(装配期对账):数据源全关 → ReconcileAll 按记账卸载
        source.Set("a", false);
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, calls, "③ 全关对账:不重装");
        Check.Eq(HookState.Absent, spy.Last, "③ 上报 Absent");
        // 重装走热路径:SetDesired 立即对账
        source.Set("a", true);
        installer.SetDesired("a", true);
        Check.Eq(2, calls, "③ 热装:SetDesired 立时装");
        Check.Eq(HookState.Installed, spy.Last, "③ 热装上报 Installed");
        // 卸载路径二:UninstallAll(记账非空 → 记账清理;撤销本体归 Task 14 游戏内验收)
        installer.UninstallAll(null);
        // UninstallAll 后记账已清、状态已复位:同开关态再对账会重装(不是「已装」跳过)
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(3, calls, "③ UninstallAll 后可重装(记账已清)");
        Check.Eq(HookState.Installed, spy.Last, "③ 重装上报 Installed");
    }

    // ④Dead 粘滞:失败→Dead;开关仍开重复对账不自动重试;显式 off 不构成尝试、
    // Dead 经 Absent 解锁;off→on 恰好再试一次,仍失败→仍 Dead,继续粘滞。
    private static void Case4_DeadStickyUntilExplicitOffOn()
    {
        var source = new SwitchSource(("k", true));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int calls = 0;
        installer.Register(new HookUnit
        {
            Id = "u.dead",
            Keys = new[] { "k" },
            Install = _ => { calls++; throw new InvalidOperationException("boom"); },
        });
        var spy = new ReportSpy();
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, calls, "④ 首次尝试");
        Check.Eq(HookState.Dead, spy.Last, "④ 失败→Dead");
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, calls, "④ Dead 粘滞:开关仍开不自动重试");
        Check.Eq(1, spy.Fired.Count, "④ 粘滞:无新上报");
        installer.SetDesired("k", false);
        Check.Eq(1, calls, "④ 显式 off:不构成安装尝试");
        Check.Eq(HookState.Absent, spy.Last, "④ 显式 off:Dead→Absent 解锁");
        installer.SetDesired("k", true);
        Check.Eq(2, calls, "④ off→on:恰好再试一次");
        Check.Eq(HookState.Dead, spy.Last, "④ 再失败→仍 Dead");
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(2, calls, "④ 仍 Dead:继续粘滞");
        Check.Eq(3, spy.Fired.Count, "④ 上报序列 Dead→Absent→Dead");
    }

    // ⑤装配前 SetDesired 不触发 install(只记期望态);装配期统一对账时,
    // 期望态 overlay 压过仍为关的数据源 → 装。
    private static void Case5_SetDesiredBeforeAssemblyOnlyRecords()
    {
        var source = new SwitchSource(("k", false));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int calls = 0;
        installer.Register(new HookUnit
        {
            Id = "u.pre",
            Keys = new[] { "k" },
            Install = _ => { calls++; return new[] { Target.AM }; },
        });
        var spy = new ReportSpy();
        installer.SetDesired("k", true);
        Check.Eq(0, calls, "⑤ 装配前 SetDesired 不触发 install");
        Check.Eq(0, spy.Fired.Count, "⑤ 装配前无状态上报");
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, calls, "⑤ 期望态在装配期生效(数据源关仍装)");
        Check.Eq(HookState.Installed, spy.Last, "⑤ 上报 Installed");
    }

    // ⑥VersionBlocked 恒不装:压制=干净态,上报 VersionBlocked;开关 off/on 拨动
    // 与重复对账都不越狱(不装、不改态)。
    private static void Case6_VersionBlockedNeverInstalls()
    {
        var source = new SwitchSource(("k", true));
        var installer = new HookInstaller(source.Reader(), targetMod => true);
        int calls = 0;
        installer.Register(new HookUnit
        {
            Id = "u.blocked",
            TargetMod = "lewd_meme",
            Keys = new[] { "k" },
            Install = _ => { calls++; return new MethodBase[0]; },
        });
        var spy = new ReportSpy();
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(0, calls, "⑥ 压制:不装");
        Check.Eq(1, spy.Fired.Count, "⑥ 恰一次上报");
        Check.Eq(("u.blocked", HookState.VersionBlocked), spy.Fired[0], "⑥ 上报 VersionBlocked");
        installer.SetDesired("k", false);
        Check.Eq(0, calls, "⑥ off 拨动:仍不装");
        Check.Eq(1, spy.Fired.Count, "⑥ off 拨动:状态不变");
        installer.SetDesired("k", true);
        Check.Eq(0, calls, "⑥ on 拨动:仍不装(不越狱)");
        Check.Eq(1, spy.Fired.Count, "⑥ on 拨动:状态不变");
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(0, calls, "⑥ 重复对账:仍不装");
        Check.Eq(1, spy.Fired.Count, "⑥ 重复对账:状态不变");
    }

    // ⑦SetDesired 不触碰 switchOn 数据源:反复拨动后零写、值原样;对账期期望态
    // overlay(k=on)压过数据源(k=off)照常装,装后数据源仍然原样。
    private static void Case7_SetDesiredNeverWritesSwitchSource()
    {
        var source = new SwitchSource(("k", false), ("other", true));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int calls = 0;
        installer.Register(new HookUnit
        {
            Id = "u.spy",
            Keys = new[] { "k", "other" },
            Install = _ => { calls++; return new[] { Target.AM }; },
        });
        installer.SetDesired("k", true);
        installer.SetDesired("k", false);
        installer.SetDesired("k", true);
        Check.Eq(0, source.Writes.Count, "⑦ SetDesired 未写开关数据源(零写)");
        Check.Eq(false, source.Values["k"], "⑦ 数据源 k 未被回写");
        Check.Eq(true, source.Values["other"], "⑦ 旁键未被触碰");
        installer.ReconcileAll((_, _) => { }, null);
        Check.Eq(1, calls, "⑦ 期望态生效:装配期照常装");
        Check.Eq(false, source.Values["k"], "⑦ 安装后数据源仍 false");
        Check.Eq(true, source.Values["other"], "⑦ 旁键仍 true");
        Check.Eq(0, source.Writes.Count, "⑦ 全程零写");
    }

    // ⑧方法级引用计数(spec §4.2 钩子组语义,Fix Round 1):两单元共享同一 original
    // ——组内任一在装,该方法不物理卸载;全组卸载才 Unpatch 恰一次;重开可再装。
    // Unpatch 观察走 internal UnpatchDecisions 计数(HookInstaller 源码直编进测试
    // 程序集,internal 直读):net8 runner 上真实 Harmony.Unpatch 亦因 MonoMod wrapper
    // 生成缺陷抛 MissingMethodException(实测),spy 子类不可行(Unpatch 非虚方法),
    // 计数在唯一 Unpatch 调用点的决策分支累加,与物理调用一一对应(harmony == null
    // 时决策照计、调用被 null 守卫跳过)。
    private static void Case8_SharedOriginalRefcountsUninstall()
    {
        var source = new SwitchSource(("a", true), ("b", true));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int callsA = 0, callsB = 0;
        installer.Register(Unit("u.priceA", new[] { "a" }, _ => { callsA++; return new MethodBase[] { Target.AM }; }));
        installer.Register(Unit("u.priceB", new[] { "b" }, _ => { callsB++; return new MethodBase[] { Target.AM }; }));
        var spy = new ReportSpy();

        // ① 两开:两单元各自装配,同一 original 计数 = 2,零卸载决策
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, callsA, "⑧① 两开:单元 A 装配");
        Check.Eq(1, callsB, "⑧① 两开:单元 B 装配");
        Check.Eq(0, installer.UnpatchDecisions, "⑧① 两开:零卸载决策");

        // ② 关 A:original 由 B 持有(计数 2→1)→ 不物理卸载;A→Absent,B 无上报(仍 Installed)
        source.Set("a", false);
        installer.SetDesired("a", false);
        Check.Eq(0, installer.UnpatchDecisions, "⑧② 关 A:共享 original 不卸载(B 仍持有)");
        Check.Eq(("u.priceA", HookState.Absent), spy.Fired[spy.Fired.Count - 1], "⑧② 关 A:A 上报 Absent");
        Check.Eq(false, spy.Fired.Contains(("u.priceB", HookState.Absent)), "⑧② 关 A:B 仍 Installed(无 Absent 上报)");

        // ③ 关 B:末位持有者(计数 1→0)→ 恰一次物理卸载决策;B→Absent
        source.Set("b", false);
        installer.SetDesired("b", false);
        Check.Eq(1, installer.UnpatchDecisions, "⑧③ 关 B:全组卸载,恰一次卸载决策");
        Check.Eq(("u.priceB", HookState.Absent), spy.Fired[spy.Fired.Count - 1], "⑧③ 关 B:B 上报 Absent");

        // ④ 重开 A:记账已清,可再装(计数回 1),无新卸载决策
        source.Set("a", true);
        installer.SetDesired("a", true);
        Check.Eq(2, callsA, "⑧④ 重开 A:install 委托再次执行");
        Check.Eq(HookState.Installed, spy.Last, "⑧④ 重开 A:上报 Installed");
        Check.Eq(1, installer.UnpatchDecisions, "⑧④ 重开 A:无新卸载决策");
    }

    // ⑨期望态求值炸的逐单元隔离(0.6.0 验收 R1 F2,0x01.log 根因回归锁):
    // switchOn 委托对某键抛 InvalidCastException → 该单元 Dead、后续单元照常
    // Installed、ReconcileAll 不抛、LastDeadReason 记录异常消息。炸的单元排在
    // 前面(catalog 序先于好单元),证明循环未被中断。
    private static void Case9_SwitchOnThrowIsolatesUnit()
    {
        HookInstaller.LastDeadReason = null;
        Func<string, bool> throwing = key =>
        {
            if (key == "boom") throw new InvalidCastException("布局键 bool cast 炸弹");
            return key == "ok";
        };
        var installer = new HookInstaller(throwing, _ => false);
        int goodCalls = 0;
        installer.Register(Unit("u.bad", new[] { "boom" },
            _ => throw new InvalidOperationException("求值先炸,install 不可达")));
        installer.Register(Unit("u.good", new[] { "ok" },
            _ => { goodCalls++; return new MethodBase[0]; }));
        var spy = new ReportSpy();

        bool threw = false;
        try { installer.ReconcileAll(spy.Handler, null); }
        catch { threw = true; }
        Check.Eq(false, threw, "⑨ ReconcileAll 不抛(单单元求值炸不中断循环)");
        Check.Eq(1, goodCalls, "⑨ 后续单元照常装配");
        Check.Eq(HookState.Installed, installer.States["u.good"], "⑨ 好单元态 = Installed");
        Check.Eq(HookState.Dead, installer.States["u.bad"], "⑨ 求值炸单元态 = Dead");
        Check.Eq(true, HookInstaller.LastDeadReason != null
            && HookInstaller.LastDeadReason.Length > 0, "⑨ LastDeadReason 非空");
        Check.Eq(true, (HookInstaller.LastDeadReason ?? "").Contains("布局键"),
            "⑨ LastDeadReason 记录的是求值异常消息");
        Check.Eq(("u.bad", HookState.Dead), spy.Fired[0], "⑨ Dead 经 SetState 上报");

        // LastDeadReason 是「最近一次」:好单元正常装配不覆盖,再次 Dead 覆盖
        installer.SetDesired("ok", false);
        installer.SetDesired("boom", false);
        Check.Eq(true, HookInstaller.LastDeadReason.Contains("布局键"),
            "⑨ 正常迁移不清除 LastDeadReason(最近一次 Dead)");
        HookInstaller.LastDeadReason = null;
    }

    // ⑩0.7.0 Task 4:TranspileMatchException(注入点未按期望命中,transpiler
    // 在 harmony.Patch 生成 IL 时即时求值抛出)从 install 委托传播 → 该单元
    // Dead 粘滞、同批其余单元照装(逐单元隔离)、LastDeadReason 记异常消息
    // (grep 友好:消息含键名/methodId/期望实得计数)、off→on 显式重试恰一次
    // ——Task 3 裁决 4「链路成立、炸不到游戏、不会被吞」的测试侧钉住;
    // 共享 original 的另一单元(Case ⑧ 先例的 NightlyTick 双单元形态)不受
    // 连带,照常 Installed。
    private static void Case10_TranspileMismatchDeadRetriable()
    {
        HookInstaller.LastDeadReason = null;
        var source = new SwitchSource(("mismatch", true), ("peer", true));
        var installer = new HookInstaller(source.Reader(), _ => false);
        int badCalls = 0, peerCalls = 0;
        installer.Register(Unit("u.transpile_bad", new[] { "mismatch" }, _ =>
        {
            badCalls++;
            throw new TranspileMatchException(
                "transpiler 注入点未按期望命中: nightly.gestation_days @ main.nightly_transpile(期望 1 处,实得 0 处;目标方法 IL 与 1.9.2 基线不符)");
        }));
        installer.Register(Unit("u.transpile_peer", new[] { "peer" }, _ =>
        {
            peerCalls++;
            return new MethodBase[] { Target.AM };
        }));
        var spy = new ReportSpy();
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, badCalls, "⑩ 匹配失败单元:install 尝试一次");
        Check.Eq(1, peerCalls, "⑩ 共享 original 的同伴单元照常装配(不连带)");
        Check.Eq(HookState.Dead, installer.States["u.transpile_bad"], "⑩ TranspileMatchException → Dead");
        Check.Eq(HookState.Installed, installer.States["u.transpile_peer"], "⑩ 同伴单元态 = Installed");
        Check.Eq(true, (HookInstaller.LastDeadReason ?? "").Contains("main.nightly_transpile"),
            "⑩ LastDeadReason 含 methodId(grep 友好)");
        // Dead 粘滞 + off→on 重试恰一次
        installer.ReconcileAll(spy.Handler, null);
        Check.Eq(1, badCalls, "⑩ Dead 粘滞:重复对账不自动重试");
        source.Set("mismatch", false);
        installer.SetDesired("mismatch", false);
        source.Set("mismatch", true);
        installer.SetDesired("mismatch", true);
        Check.Eq(2, badCalls, "⑩ off→on:显式重试恰一次");
        Check.Eq(HookState.Dead, installer.States["u.transpile_bad"], "⑩ 仍不命中 → 仍 Dead");
        HookInstaller.LastDeadReason = null;
    }
}
