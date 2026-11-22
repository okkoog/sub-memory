using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 钩子单元状态(spec §5.7):Absent=未装;Installed=已装;
    /// Dead=装配失败粘滞(不自动重试);VersionBlocked=目标版本压制(干净态)。
    /// </summary>
    public enum HookState
    {
        Absent,
        Installed,
        Dead,
        VersionBlocked,
    }

    /// <summary>
    /// 钩子组定义(Task 3 catalog 数据行、Task 4 patch 类改形目标):纯数据。
    /// Install 委托内部做 ModReflect 解析 + Patch,返回本次补丁的 original 清单(记账);
    /// Harmony 实例由安装器转交,纯逻辑不 new Harmony。
    /// </summary>
    public sealed class HookUnit
    {
        /// <summary>单元名,如 "main.nightly_transpile" / "brothel.economy"(日志与面板标记用)。</summary>
        public string Id;

        /// <summary>版本压制查询键(按单元查 FamilyState;DataWrite 类单元可空 = 不查)。</summary>
        public string TargetMod;

        /// <summary>消费的配置键(含派生开关键的原键):任一键期望开 = 应装,全关 = 应卸。</summary>
        public string[] Keys;

        /// <summary>装配动作:返回记账清单;抛异常 = 装配失败(该单元记 Dead,逐单元隔离)。</summary>
        public Func<Harmony, MethodBase[]> Install;
    }

    /// <summary>
    /// 钩子组状态机(spec §5.2/§5.3/§5.7,替代 PatchHub 的按需装卸核心;纯逻辑可单测)。
    /// 目标态裁决(每单元):TargetMod 被版本压制 → 恒 VersionBlocked(拨动开关不越狱);
    /// 否则 Keys 任一期望开 → Installed、全关 → Absent。对账把单元向目标态迁移:
    /// 装配经单元 Install 委托(逐单元 try/catch,失败→Dead 并隔离,不阻断其余),
    /// 卸载按方法级引用计数(spec §4.2 钩子组语义:共享同一 MethodBase 的单元,
    /// 组内任一在装则该方法不物理卸载,全组卸载才撤销)——harmony.Unpatch(original,
    /// HarmonyPatchType.All, harmony.Id) 逐方法撤销。
    /// Dead 粘滞:开关仍开时重复对账不自动重试;显式 off 使目标态经 Absent 过渡解锁,
    /// off→on = 新的装配意图,允许再试一次。
    /// install 回调 = 状态迁移上报:对账导致单元状态变化时以新状态回调一次
    /// (装=Installed / 失败=Dead / 卸=Absent / 压制=VersionBlocked),无变化不回调;
    /// 装配日志「装配 N 项」由消费方统计 Installed 上报得出。
    /// 对账器生命周期:ReconcileAll 建立对账器(绑定 Harmony 与回调),UninstallAll
    /// 撤全部记账并解除——装配期之前 SetDesired 只记期望态,不触发装、不可能 Dead
    /// (主菜单期 ModReflect 未就绪的时序性失败不会把功能废到重启)。
    /// switchOn / versionBlocked 是只读数据源,状态机绝不回写(期望态 overlay 内部保存)。
    /// </summary>
    public sealed class HookInstaller
    {
        private readonly Func<string, bool> switchOn;
        private readonly Func<string, bool> versionBlocked;
        private readonly List<HookUnit> units = new List<HookUnit>();
        // key → 消费该键的单元(装配后 SetDesired 只对账受影响单元)
        private readonly Dictionary<string, List<HookUnit>> unitsByKey =
            new Dictionary<string, List<HookUnit>>();
        // 记账:unit → 本次装配返回的 original 清单(逐方法 Unpatch 的依据)
        private readonly Dictionary<HookUnit, MethodBase[]> accounting =
            new Dictionary<HookUnit, MethodBase[]>();
        // 方法级引用计数(spec §4.2 钩子组语义):original → 在装单元数。
        // 共享同一 MethodBase 的单元(如价格与文案两单元各自 postfix 同一 CreateX)
        // 构成跨单元钩子组:组内任一在装,该方法不物理卸载;全组卸载才 Unpatch。
        private readonly Dictionary<MethodBase, int> methodRefs =
            new Dictionary<MethodBase, int>();
        /// <summary>测试观察口:决定物理 Unpatch 的次数(方法级引用计数归零的次数)。
        /// 生产代码不读取;harmony == null 时决策照样计数(调用被 null 守卫跳过)。</summary>
        internal int UnpatchDecisions;

        /// <summary>最近一次单元转 Dead 的原因(ex.Message;静态、全局最近一次,
        /// 诊断用)。F2:期望态求值(switchOn/versionBlocked 委托)与装配/卸载的
        /// 异常都记录在此——0x01.log 根因即 TargetState 经 switchOn 求值炸
        /// InvalidCastException 曾把 ReconcileAll 整体打断。</summary>
        internal static string LastDeadReason;
        private readonly Dictionary<HookUnit, HookState> states =
            new Dictionary<HookUnit, HookState>();
        // 期望态 overlay:SetDesired 写入;未记录的键回读 switchOn 数据源
        private readonly Dictionary<string, bool> desired = new Dictionary<string, bool>();
        // 对账器(ReconcileAll 建立,UninstallAll 解除);harmony 可为 null(测试场景:
        // 假 install 不使用它,卸载守卫跳过撤销)
        private Action<HookUnit, HookState> report;
        private Harmony harmony;
        private bool reconcilerReady;

        /// <summary>switchOn = 注册表开关键态;versionBlocked = 按单元 TargetMod 查压制。</summary>
        public HookInstaller(Func<string, bool> switchOn, Func<string, bool> versionBlocked)
        {
            this.switchOn = switchOn;
            this.versionBlocked = versionBlocked;
        }

        /// <summary>登记钩子组,初始态 Absent;同一引用重复登记幂等(不重复进键索引)。</summary>
        public void Register(HookUnit unit)
        {
            if (units.Contains(unit)) return;
            units.Add(unit);
            if (unit.Keys != null)
                foreach (var k in unit.Keys)
                {
                    if (!unitsByKey.ContainsKey(k)) unitsByKey[k] = new List<HookUnit>();
                    unitsByKey[k].Add(unit);
                }
            states[unit] = HookState.Absent;
        }

        /// <summary>
        /// 观察口(R-W,Task 14 调试面板/路由审计):unitId → 当前态只读快照。
        /// 每次访问分配新字典(消费方安全持有,不受后续对账影响);纯簿记,
        /// 不触发对账、不上报。测试 SelfTestPlanTests.InstallerStatesSnapshot 锁定。
        /// </summary>
        internal IReadOnlyDictionary<string, HookState> States
        {
            get
            {
                var snapshot = new Dictionary<string, HookState>(units.Count, StringComparer.Ordinal);
                foreach (HookUnit u in units) snapshot[u.Id] = CurrentState(u);
                return snapshot;
            }
        }

        /// <summary>
        /// 装配期统一对账:建立对账器后逐单元向目标态迁移——按应装调 install 委托装配、
        /// 按应卸/压制按记账卸载,只装「开关开 且 非压制」的组。
        /// </summary>
        public void ReconcileAll(Action<HookUnit, HookState> install, Harmony harmony)
        {
            report = install;
            this.harmony = harmony;
            reconcilerReady = true;
            foreach (var u in units) Reconcile(u);
        }

        /// <summary>
        /// 开关 Changed 入口:记录期望态;对账器已建立时立即对账该键所属单元(热装卸),
        /// 未建立(装配期之前)只记期望态。不触碰 switchOn 数据源。
        /// </summary>
        public void SetDesired(string key, bool on)
        {
            desired[key] = on;
            if (!reconcilerReady) return;
            if (!unitsByKey.ContainsKey(key)) return;
            foreach (var u in unitsByKey[key]) Reconcile(u);
        }

        /// <summary>
        /// 终局撤装:逐单元按记账 Unpatch(original, All, harmony.Id),清记账、
        /// 状态复位 Absent,并解除对账器(其后 SetDesired 回到只记期望态)。
        /// 静默清场,不走状态上报。
        /// </summary>
        public void UninstallAll(Harmony harmony)
        {
            foreach (var u in units) Uninstall(u, harmony);
            states.Clear();
            desired.Clear(); // 期望态 overlay 随状态一并复位,防终局撤装后残留过期目标态
            report = null;
            this.harmony = null;
            reconcilerReady = false;
        }

        // —— 内部:单单元对账 ——
        // F2(验收 R1):try/catch 覆盖整个单元对账(期望态求值 + 装/卸 + 簿记),
        // 任何一步炸 → 该单元 Dead、循环继续——ReconcileAll/SetDesired 绝不再被
        // 单单元异常中断(0x01.log:switchOn 委托对 int 布局键抛 InvalidCastException
        // 曾令 catalog #2 之后的全部单元不装配)。Dead 原因存 LastDeadReason 经
        // SetState(Dead) 上报侧打日志;上报 handler 自身不抛(R2 既有约定)。

        private void Reconcile(HookUnit u)
        {
            try
            {
                var target = TargetState(u);
                var current = CurrentState(u);
                // Dead 粘滞:应装但上次失败 → 不自动重试;显式 off 使目标态经 Absent 后解锁
                if (current == HookState.Dead && target == HookState.Installed) return;
                if (target == current) return;
                if (target == HookState.Installed)
                {
                    MethodBase[] acc;
                    try { acc = u.Install(harmony) ?? new MethodBase[0]; }
                    catch (Exception ex)
                    {
                        // 逐单元隔离(spec §5.2 失败语义):该组记 Dead,不阻断其余
                        LastDeadReason = ex.Message;
                        SetState(u, HookState.Dead);
                        return;
                    }
                    accounting[u] = acc;
                    foreach (var m in acc)
                        if (m != null)
                            methodRefs[m] = methodRefs.TryGetValue(m, out int n) ? n + 1 : 1;
                    SetState(u, HookState.Installed);
                    return;
                }
                // Absent / VersionBlocked:干净态,先撤已装补丁
                Uninstall(u, harmony);
                SetState(u, target);
            }
            catch (Exception ex)
            {
                // 期望态求值 / 卸载 / 簿记路径的兜底隔离:同样只废该单元
                LastDeadReason = ex.Message;
                SetState(u, HookState.Dead);
            }
        }

        private HookState TargetState(HookUnit u)
        {
            if (u.TargetMod != null && versionBlocked(u.TargetMod)) return HookState.VersionBlocked;
            if (u.Keys != null)
                foreach (var k in u.Keys)
                {
                    // 期望态 overlay 优先,未记录的键回读数据源
                    var on = desired.ContainsKey(k) ? desired[k] : switchOn(k);
                    if (on) return HookState.Installed;
                }
            return HookState.Absent;
        }

        private HookState CurrentState(HookUnit u)
        {
            return states.ContainsKey(u) ? states[u] : HookState.Absent;
        }

        private void SetState(HookUnit u, HookState state)
        {
            if (CurrentState(u) == state) return;
            states[u] = state;
            if (report != null) report(u, state);
        }

        /// <summary>按记账逐方法撤销(12 月规则:Unpatch 必须传自己的 id);无记账 = no-op。
        /// 方法级引用计数裁决:先减,组内仍有持有者(&gt;0)→ 跳过 Unpatch 只清本单元记账;
        /// 减到 0(末位持有者)→ harmony.Unpatch(m, All, id) 并移除计数。
        /// harmony == null 时只清记账与计数不撤销(测试 / 未初始化场景安全,同
        /// PatchHub.UninstallAll 先例);决策计数照常累加(UnpatchDecisions)。</summary>
        private void Uninstall(HookUnit u, Harmony harmony)
        {
            if (!accounting.ContainsKey(u)) return;
            var acc = accounting[u];
            accounting.Remove(u);
            if (acc == null) return;
            foreach (var m in acc)
            {
                if (m == null) continue;
                methodRefs.TryGetValue(m, out int refs);
                if (refs > 1)
                {
                    methodRefs[m] = refs - 1;   // 组内仍有单元在装:不物理卸载
                    continue;
                }
                methodRefs.Remove(m);           // 末位持有者:计数归零
                UnpatchDecisions++;
                if (harmony != null) harmony.Unpatch(m, HarmonyPatchType.All, harmony.Id);
            }
        }
    }
}
