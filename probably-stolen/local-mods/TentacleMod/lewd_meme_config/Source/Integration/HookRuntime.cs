using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 装配运行期(M2c 按需装配的接线点,整体替代旧 PatchHub + Bootstrap 双件):
    /// 目录(HookCatalog,Task 3)+ 装配委托映射(15 个 patch 类的 Register 产出,
    /// catalog-id → install 委托)+ 状态机(HookInstaller,Task 2)在此合流。
    /// 职责:
    ///  - <see cref="ComposeAndInstall"/>:建映射 → 完备性守卫(R5)→ 家族快照与
    ///    Router 批量裁决(R-A/R-B:Probe 一次存字段)→ 面板标记装配期复核(R-C)→
    ///    按目录登记 MainHook/SubModHook 单元(让位单元不登记;SubModHook 目标
    ///    缺席不登记[R11-F1,防跨族键开关触发缺席目标装配 Dead];DataWrite 行不
    ///    进安装器,R7)→ ReconcileAll 只装「开关开 且 非版本压制」的单元(默认
    ///    全关 = 0 项,零 hook)→ DataWrite 装配期一次写入(让位/压制单元不写)→
    ///    收尾装配日志(R11)→ Changed 分发器单一订阅;
    ///  - <see cref="OnSwitchChanged"/>:registry.Changed 的唯一分发器——MainHook
    ///    热装卸对账(开关键 → 原键期望态)+ DataWrite 重写处理器转发(池写 4 键
    ///    等既有 Changed 驱动的数据写并入同一分发器,spec §5.2/§5.3)+ 镜像键写透
    ///    分发(R-J/M4b:WriteThrough 单元不进安装器,直转 WriteThroughCarrier);
    ///  - <see cref="Shutdown"/>:UninstallAll + 写透快照恢复(RestoreAll)+
    ///    分发器退订(DataWrite 处理器随之退订)+ 面板三件套/上下文清理(R10)。
    /// 重入护栏(R1):composed 置位后重复派发的 OnModItemDirectoryInit 直接 no-op。
    /// 静态字段缓存安装器/harmony/注册表(防加载器重建 ModEntry 实例,0.5.0 先例)。
    /// 开关派生(F1 分派单一事实源 = Logic/SwitchDispatch.cs):89 值键查各自
    /// 子项开关键(PanelKeys.ItemSwitchKey,默认关)+ 87 镜像键(含 R9 iph.*
    /// 写透与 R10 iph.attr_* 属性五键)同路径;无子项
    /// 开关的三键自身即开关语义(gender_scope 档位 ≠0、guest.display_enabled /
    /// names.use_custom_file 布尔,0.5.0 开关体系裁定);9 布局尺寸键恒 false
    /// (不上面板不驱动装配,0.6.0 验收 R1 修订——旧 KNF 回退对 int 键是
    /// InvalidCastException)。
    /// </summary>
    internal static class HookRuntime
    {
        // IPH 让位豁免的目录条目(MaidenPatch.Register 让位分支不产委托,0.5.1 语义;
        // 0.7.0 T4:nightly 组两单元恒产委托、remake 让位改走 Router ③ Yield 过滤,
        // NightlyTickId 豁免通道随之拆除——remake 在场 × 主 mod 版本不命中时该组
        // 照常登记、installer 压制 = VersionBlocked,与 Router 裁决一致不再假红)
        private const string CreateMaidenId = "main.create_maiden";
        private const string CreateMaidenOfFactionId = "main.create_maiden_of_faction";

        private static HookInstaller installer;
        private static Harmony harmony;          // 主补丁实例(id = ModEntry.Id,卸载按此 id)
        private static ConfigRegistry registry;  // 装配期注册表快照(分发器退订用)
        private static Action<string> log;       // 日志出口(Shutdown 置空)
        private static bool mainModSupported;    // 装配期快照:主 mod 在场且版本受支持(DataWrite 守卫/转发短路)
        private static IReadOnlyDictionary<string, FamilyState> family;  // R-B:家族快照(版本压制查询;Probe 一次存字段,会话内静态)
        private static IReadOnlyDictionary<string, RouteDecision> route; // R-A:Router 批量裁决(让位登记过滤 / DataWrite 门控)
        private static HashSet<string> r5Exempt; // R5 豁免单元集合(F1,装配期快照;见 Compose 派生与 R5ExemptUnits)
        private static List<string> absentUnits;  // R11-F1:TargetMod 缺席未注册的 SubModHook 单元(装配期快照;③ 拦截时记账)
        private static bool composed;            // R1 重入护栏

        /// <summary>已装配标记(ModEntry 的重入守卫读,镜像旧 hub != null 判定)。</summary>
        internal static bool Composed => composed;

        // ---- R-W 观察口(Task 14 调试面板/自检只读消费;不改变装配语义)----
        // 装配期快照的只读暴露:路由审计的期望态计算与安装器实际态对账以此为准,
        // 与装配路径同源零偏差。未装配(主菜单/让位)时全部 null,消费方按
        // 「装配未就绪」SKIP。

        /// <summary>安装器实例(States 实际态查询);未装配为 null。</summary>
        internal static HookInstaller Installer => installer;

        /// <summary>装配期家族快照(版本压制/在场判定);未装配为 null。</summary>
        internal static IReadOnlyDictionary<string, FamilyState> FamilySnapshot => family;

        /// <summary>装配期 Router 批量裁决快照(让位过滤);未装配为 null。</summary>
        internal static IReadOnlyDictionary<string, RouteDecision> RouteSnapshot => route;

        /// <summary>R5 豁免单元集合(装配期快照,只读):IPH 在场 →
        /// main.create_maiden 两接管——这些单元的 install 委托在让位分支不产出、
        /// 登记期(③)被跳过,实际态恒 NotRegistered(F1:自检期望态在 Router 裁
        /// VersionBlocked[R5 版本压过让位]时须与登记过滤对齐,否则主 mod 版本
        /// 不命中 × IPH 在场组合下审计假红;T4:remake → nightly 组豁免已随让位
        /// 分支拆除,remake 组改走 Router ③ Yield 过滤恒产委托)。未装配为 null。</summary>
        internal static IReadOnlyCollection<string> R5ExemptUnits => r5Exempt;

        /// <summary>R11-F1:TargetMod 缺席而未注册的 SubModHook 单元(装配期
        /// 快照,只读):缺席目标的 install 委托内 ModReflect.Types(对方) 必抛
        /// 'not a running mod' → Dead(0x0f 根因:跨族键主面板恒 Define,其开关
        /// 拨动即触发对账,「面板不 Define → 开关恒 false」防线对跨族键失效),
        /// ③ 登记期直接拦截——不装不卸、物理零 hook,实际态恒 NotRegistered,
        /// 自检期望态据此对齐。未装配为 null。</summary>
        internal static IReadOnlyCollection<string> AbsentTargetUnits => absentUnits;

        /// <summary>装配期注册表快照(自检临时写/复原的通道);未装配为 null。</summary>
        internal static ConfigRegistry Registry => registry;

        /// <summary>
        /// 装配编排(ModEntry.OnDirectoriesReady 尾段调用,时序:让位复核 →
        /// IphCompat.Resolve → PatchCtx.Config 注入 → 主 mod 面板动态层 → 本方法)。
        /// 重复派发 no-op(R1);编排层异常只记日志,已装单元不受影响(卸载归 Shutdown)。
        /// </summary>
        public static void ComposeAndInstall(ConfigRegistry cfg, MainModResolveResult resolve, Action<string> logAction)
        {
            if (composed) return; // R1:重入护栏(替代旧 hub != null 幂等标记)
            composed = true;
            registry = cfg;
            log = logAction;
            mainModSupported = resolve.MainModSupported;
            try
            {
                Compose(cfg, resolve);
            }
            catch (Exception ex)
            {
                // 完整堆栈入 Player.Log(ex.ToString 含 inner);消息行保 grep 友好
                log?.Invoke("装配编排失败(已装单元不受影响,停用流程照常撤装): " + ex.Message);
                UnityEngine.Debug.LogException(ex);
            }
        }

        /// <summary>
        /// registry.Changed 分发器(ComposeAndInstall 内订阅一次,Shutdown 退订):
        /// 子项开关键(.enabled)→ 把开关态记为原键期望态并热装卸对账;值键变更 →
        /// 重估该键期望态(开关未开则无状态变化);随后转发 DataWrite 重写处理器
        /// (maiden 四池/EXP_NEED、trap 策略数组、guest 显示开关;开关语义由
        /// PlayerLayerAllowed 压 Current 天然承担,0.5.0 行为不变,R7)。
        /// 镜像键域(R-J,M4b):WriteThrough 单元不进安装器,在此直转写透载体——
        /// 开关 on → Carrier.Enable(快照 + 写当前镜像值)、off → Carrier.Disable
        /// (恢复快照);值变化且开关 on → Carrier.Enable(engine 锚定语义:只写
        /// 新值,恢复点保持在开启时刻,0.6.0 修正),
        /// 开关 off 的值变化无动作(玩家层被压,值仅记录)。
        /// </summary>
        public static void OnSwitchChanged(string key)
        {
            HookInstaller inst = installer;
            ConfigRegistry cfg = registry;
            if (inst == null || cfg == null) return; // 未装配 / 已 Shutdown
            try
            {
                bool isSwitch = PanelKeys.IsSwitchKey(key);
                string raw = isSwitch
                    ? key.Substring(0, key.Length - PanelKeys.SectionSwitchSuffix.Length)
                    : key;
                // 镜像键域分流(R-J/M4b):镜像 OurKey 前缀(remake.*/brood.*/iph.*)与主面板
                // 键零重叠;早退 = 不落入安装器对账与 DataWrite 转发(镜像键不属
                // 于任何 MainHook/DataWrite 单元的 Keys,行为本就为零,显式早退
                // 防 SetDesired 对未知键的歧义)。M5a:仅 WriteThrough 条目走写透;
                // SubModHook 条目(brothel.*)不在此分流——其键属于 brothel.* 单元,
                // 落入下方安装器对账(与 MainHook 键同路径热装卸)。
                if (FamilyPanelKeys.TryGet(raw, out FamilyPanelEntry mirror)
                    && mirror.Carrier == CarrierKind.WriteThrough)
                {
                    if (isSwitch)
                    {
                        if (cfg.Current<bool>(key)) MirrorEnable(mirror);
                        else WriteThroughCarrier.Disable(mirror, log);
                    }
                    else if (SwitchOnFor(raw))
                    {
                        MirrorEnable(mirror);
                    }
                    return;
                }
                if (isSwitch)
                {
                    // 子项开关键:剥离 .enabled 后缀对账原键(单元按原键建索引)
                    inst.SetDesired(raw, cfg.Current<bool>(key));
                }
                else
                {
                    inst.SetDesired(key, SwitchOnFor(key));
                }
            }
            catch (Exception ex)
            {
                log?.Invoke("开关对账失败(" + key + "): " + ex.Message);
            }
            // DataWrite 分发(R-A:Yield / 版本压制的 DataWrite 单元不订阅、不写;
            // 主 mod 缺席/版本压制时无事可写——EnsureResolved 未跑过,静态字段全
            // null,直接短路)。R9 拆分 + T9 收缩:maiden 的职业池
            // (pools.maiden_jobs)与 fbb 三池(pools.maiden_fbb)按各自 route
            // 独立守卫,单次转发分流(IPH 在场 = fbb 三池让位跳过、
            // 职业池照写;pools.maiden 经验表已随 T9 删除——1.9.2 EXP_NEED
            // 字段消失,经验公式化)
            if (!mainModSupported || route == null) return;
            if (route["pools.maiden_jobs"] == RouteDecision.Own
                || route["pools.maiden_fbb"] == RouteDecision.Own)
            {
                MaidenPatch.OnConfigChanged(key,
                    route["pools.maiden_jobs"] == RouteDecision.Own,
                    route["pools.maiden_fbb"] == RouteDecision.Own);
            }
            if (route["pools.trap_tactic"] == RouteDecision.Own) TrapPatch.OnConfigChanged(key);
            if (route["pools.guest_display"] == RouteDecision.Own) ValuePostfixPatches.OnConfigChanged(key);
        }

        /// <summary>
        /// 终局撤装(仅退出游戏时经 ModEntry.OnDisable 调用):安装器按记账逐方法
        /// Unpatch(自身 id)→ 写透快照恢复(spec §5.2:有未恢复快照的镜像键写回
        /// 对方开启前原值,无快照纯 no-op)→ 退订 Changed 分发器(DataWrite 处理器
        /// 经分发器转发,随之一并退订,R10)→ 面板三件套撤装(独立 harmony id)→
        /// PatchCtx 与解析器日志出口清理。静态字段全部复位,允许同会话重启用时
        /// 重新装配。
        /// </summary>
        public static void Shutdown()
        {
            try
            {
                installer?.UninstallAll(harmony);
            }
            catch (Exception ex)
            {
                log?.Invoke("lewd_meme_config 卸载补丁失败: " + ex.Message);
            }
            WriteThroughCarrier.RestoreAll(log);  // M4b:写透键恢复快照(逐键隔离)
            BroodTagPicker.HostOff();  // 0.7.1 拾取器宿主兜底销毁(UninstallAll 静默清场不走 Report)
            if (registry != null) registry.Changed -= OnSwitchChanged;
            PanelRowPatch.Uninstall();  // 0.5.1 面板三件套(Init 期装,独立 id 恒装)
            PatchCtx.Config = null;     // patch 层读值兜底回默认档 0(全放行原版)
            MainModResolver.Logger = null;
            installer = null;
            harmony = null;
            registry = null;
            log = null;
            mainModSupported = false;
            family = null;
            route = null;
            r5Exempt = null;
            absentUnits = null;
            composed = false;
        }

        // ------------------------------------------------------------------
        // 装配编排内部
        // ------------------------------------------------------------------

        private static void Compose(ConfigRegistry cfg, MainModResolveResult resolve)
        {
            // ⓪ 家族快照 + Router 批量裁决 + 面板标记(R-A/R-B/R-C,0.6.0 版本严格
            //    绑定):Probe 一次存字段(会话内静态——mod 版本会话内不变);标记表
            //    装配期复核(与 Init 期同源同值,启用集合会话内不变,纯防御性重灌)。
            //    两道闸口分工:Router 管 Yield(登记期过滤),installer 的
            //    versionBlocked delegate 管版本压制(照常登记、对账期自然压制)。
            family = Family.ProbeMap();
            route = Router.DecideAll(family);
            PanelMarks.Rebuild(family);

            // ① 装配委托映射:14 个 patch 类产出 catalog-id → install 委托
            //    (FindMainType/ModReflect.Target 全部封在委托内,装配期才解析;
            //    顺序承旧 BuildHub,与 HookCatalog 的条目序各自独立)
            var installs = new Dictionary<string, Func<Harmony, MethodBase[]>>();
            GameGenderPatches.Register(installs);
            GenderRatioPatch.Register(installs);
            NightlyPatch.Register(installs);  // T4:nightly 组两 transpiler 单元恒登记(remake 让位走 Router ③)
            AlarmPatch.Register(installs);
            TrapPatch.Register(installs);
            CorruptPatch.Register(installs);
            MaidenPatch.Register(installs);    // IPH 在场让位 create_maiden 两接管(R9)
            MaidenAttrIphPatch.Register(installs); // R10:属性五键 postfix(IPH 在场为工作前提,不经让位早退)
            MaidenInteractPatch.Register(installs); // 0.7.0 T9:互动阈值 prefix + 初始处女 postfix
            LevelCapPatch.Register(installs);
            ValuePostfixPatches.Register(installs);
            LootPatch.Register(installs);
            SpecialAndManualPatches.Register(installs);
            BrothelPatches.Register(installs);  // M5a:掌上青楼 9 单元(brothel.*,SubModHook)
            TearlPatches.Register(installs);    // M5b:tearlaments 3 单元(tearl.*,SubModHook)
            RemakePatches.Register(installs);   // 0.6.0 R4:暗流交易差分单元(remake.*,SubModHook;ST4: -semen_formula -alarm_activity)
            BroodPatches.Register(installs);    // 0.6.0 R12:巢穴扩展信息素 1 单元(brood.perfume_mod,SubModHook)
            PrinterPatches.Register(installs);  // Task 14b:触手3D打印机 1 单元(main.printer,1.7.1)
            BroodTagPicker.Register(installs);  // 0.7.1:苗床标签拾取器 1 单元(brood.tag_picker,无补丁宿主生命周期)

            // ①' R5 豁免快照(F1):② 守卫与 R5ExemptUnits 暴露的单一事实源——
            //     从既有让位分支常量派生(IPH → maiden 两接管),勿在他处复制判定;
            //     让位分支零产出 ⇒ 豁免单元 ③ 不登记,自检期望态对豁免单元在
            //     VersionBlocked 裁决下对齐 NotRegistered。(T4:remake → nightly
            //     组的派生已随 NightlyTick 让位分支拆除而移除)
            var exempt = new HashSet<string>(StringComparer.Ordinal);
            if (IphCompat.Active)
            {
                exempt.Add(CreateMaidenId);
                exempt.Add(CreateMaidenOfFactionId);
            }
            r5Exempt = exempt;

            // ② 完备性守卫(R5;R-R 扩至 SubModHook):目录 MainHook 与 SubModHook
            //    条目必须有装配委托,缺失只警告不崩溃;让位分支的预期缺席豁免
            //    (①' 派生,remake/IPH 在场时对应条目无委托是让位,让位日志由各
            //    patch 类的 Register 自行打出)。
            var missing = new List<string>();
            foreach (HookUnitDef def in HookCatalog.Units)
            {
                if (def.Kind != CarrierKind.MainHook && def.Kind != CarrierKind.SubModHook) continue;
                if (installs.ContainsKey(def.Id)) continue;
                if (r5Exempt.Contains(def.Id)) continue;
                missing.Add(def.Id);
            }
            if (missing.Count > 0)
            {
                log?.Invoke("⚠ 装配映射不完备:以下目录条目缺装配委托,不参与装卸: "
                    + string.Join(", ", missing.ToArray()));
            }

            // ③ 登记单元 + 建立对账器(R-A 让位装配过滤):Router 判 Yield 的单元
            //    不注册进安装器(不装不卸);VersionBlocked 单元照常注册——由
            //    installer 的 versionBlocked delegate 自然压制。对账只装「开关开
            //    且 非压制」的组——默认全关时全部 Absent,install 委托零调用
            //    (零 hook 红线)。R-N(M5a):SubModHook 单元(brothel.*)与
            //    MainHook 同路径登记装配;DataWrite/WriteThrough 行不进安装器
            //    (R7:Install 委托为 null 会对账 NRE)。R11-F1:SubModHook 的
            //    TargetMod 缺席 → 不注册——三道闸口各司其职:Router 管 Yield
            //    (缺席目标的单元恒 Own,语义归属不失)、versionBlocked delegate
            //    管在场但版本漂移、本拦截管缺席(install 委托内 ModReflect.Types
            //    (对方) 对缺席 mod 必抛 → Dead;拦截动机的历史形态是跨族键
            //    [nightly.semen_chance_cap 主面板恒 Define、曾兼由
            //    remake.semen_formula 消费——ST4 单元注销后跨族键终结,该键
            //    唯一消费 main.nightly_transpile;拦截本身保留,缺席目标的
            //    SubModHook 单元开关拨动仍需防对账炸 Dead)。「面板不 Define →
            //    开关恒 false」防线同理)。
            //    幂等:会话内 mod 集不变,一次判定成立。
            installer = new HookInstaller(SwitchOnFor, VersionBlocked);
            var absentTarget = new List<string>();
            foreach (HookUnitDef def in HookCatalog.Units)
            {
                if (def.Kind != CarrierKind.MainHook && def.Kind != CarrierKind.SubModHook) continue;
                if (!installs.ContainsKey(def.Id)) continue;    // 让位/映射缺失条目不登记
                if (route[def.Id] == RouteDecision.Yield) continue; // R-A:让位单元不登记(不装不卸)
                if (def.Kind == CarrierKind.SubModHook && !TargetPresent(def.TargetMod))
                {
                    absentTarget.Add(def.Id);                   // R11-F1:目标缺席不注册(快照记账供期望态)
                    continue;
                }
                installer.Register(new HookUnit
                {
                    Id = def.Id,
                    TargetMod = def.TargetMod,
                    Keys = def.Keys,
                    Install = installs[def.Id],
                });
            }
            absentUnits = absentTarget;
            if (absentTarget.Count > 0)
            {
                // 与让位日志同风格的一行聚合(⑤ 前置,登记决策的直接证据)
                log?.Invoke("SubModHook 目标缺席未注册: " + absentTarget.Count + " 项("
                    + string.Join(", ", absentTarget.ToArray())
                    + ")(物理零 hook,装上对方后重启生效)");
            }
            harmony = new Harmony(ModEntry.Id);

            // ③' Changed 分发器订阅前移(F3,验收 R1):原序在 ReconcileAll 之后,
            //     装配段异常中止会连热装卸能力一起埋掉(⑥ 永不执行)。订阅先于
            //     reconcile——即使对账段再炸(现由 F2 逐单元隔离兜底),开关拨动
            //     仍能驱动已登记单元的热对账。重复订阅不存在(ComposeAndInstall 的
            //     composed 护卫单一入口),Shutdown 照常退订。
            cfg.Changed += OnSwitchChanged;

            var installed = new List<string>();
            installer.ReconcileAll((unit, state) => Report(unit, state, installed), harmony);

            // ④ DataWrite 装配期一次写入(pools.maiden_jobs[职业池,R9 拆分,
            //    IPH 零触碰恒 Own] / pools.maiden_fbb[阵营/体型/胸部三池,R9 拆分,
            //    IPH 在场让位] / pools.trap_tactic / pools.guest_display;主 mod
            //    在场且版本受支持才写,后续重写走分发器;R-A:Yield 单元不写、
            //    不订阅——pools.maiden_fbb 让位 IPH(其工厂 transpiler 替换三池
            //    抽取,Router R4),条件经 Router 独立判定;T9:pools.maiden
            //    经验表写入已删除——1.9.2 删 EXP_NEED 字段,经验公式化)
            if (mainModSupported)
            {
                TryDataWrite("pools.maiden_jobs", MaidenPatch.WriteJobsAtCompose);
                TryDataWrite("pools.maiden_fbb", MaidenPatch.WriteFbbAtCompose);
                TryDataWrite("pools.trap_tactic", TrapPatch.WriteTacticChancesAtCompose);
                TryDataWrite("pools.guest_display", ValuePostfixPatches.WriteGuestDisplayAtCompose);
            }

            // ④' 镜像键初始对账(R-J,M4b):开关已 on 的镜像键执行一次 Enable
            //     (与 Hook 单元同构的装配期对账)。目标缺席的分区未 Define →
            //     无玩家层值 → 开关恒读默认 false,此处天然跳过(缺席 no-op
            //     红线);在场但键缺席由载体的存在性守卫兜底(日志 + no-op)。
            //     M5a:SubModHook 条目(brothel.*)不写透——其单元已经 ③ 的
            //     ReconcileAll 按开关对账安装,无需镜像初使能。
            foreach (FamilyPanelEntry mirror in FamilyPanelKeys.Entries)
            {
                if (mirror.Carrier != CarrierKind.WriteThrough) continue;
                try
                {
                    if (SwitchOnFor(mirror.OurKey)) MirrorEnable(mirror);
                }
                catch (Exception ex)
                {
                    log?.Invoke("镜像键初始对账失败(" + mirror.OurKey + "): " + ex.Message);
                }
            }

            // ⑤ 装配日志(R11):N = Installed 上报计数;默认全关时 N==0 且列表为空。
            //    PanelRowPatch 独立 harmony id、Init 期恒装,不在本计数内。F2 逐单元
            //    隔离后单单元求值/装配失败不再中止 Compose,本行必然到达。
            log?.Invoke("lewd_meme_config 装配:" + installed.Count + " 项("
                + string.Join(", ", installed.ToArray()) + ")");
        }

        /// <summary>
        /// 状态迁移上报(装=Installed / 失败=Dead / 卸=Absent / 压制=VersionBlocked,
        /// 无变化不回调):装配期 Installed 计数入列表(R11 的 N),Dead/卸载/压制
        /// 记日志。R2:上报 handler 绝不抛——对账循环不能被记日志/簿记异常打断。
        /// </summary>
        private static void Report(HookUnit unit, HookState state, List<string> installed)
        {
            try
            {
                switch (state)
                {
                    case HookState.Installed:
                        installed.Add(unit.Id);
                        break;
                    case HookState.Dead:
                        // 0.7.1 拾取器:空记账单元的装配失败也走宿主销毁
                        // (install 委托 = 解析 + 建宿主,抛出点之前可能已建);
                        // 非该单元恒 no-op
                        BroodTagPicker.OnUnitGone(unit.Id);
                        // F2:附最近一次 Dead 原因(SetState 同步回调,时序上即本单元
                        // 刚捕获的 ex.Message;静态最近一次,诊断用)
                        log?.Invoke("单元 " + unit.Id + " 装配失败(Dead): "
                            + (HookInstaller.LastDeadReason ?? "(无原因记录)")
                            + "(开关 off→on 可重试)");
                        break;
                    case HookState.Absent:
                        // 0.7.1 拾取器:空记账单元无物理卸载动作,宿主销毁挂在此
                        // 迁移回调(Shutdown 另有显式兜底——UninstallAll 静默清场
                        // 不走 Report);非该单元恒 no-op
                        BroodTagPicker.OnUnitGone(unit.Id);
                        log?.Invoke("钩子已卸载: " + unit.Id);
                        break;
                    case HookState.VersionBlocked:
                        BroodTagPicker.OnUnitGone(unit.Id);
                        log?.Invoke("钩子目标版本不受支持,压制不装配: " + unit.Id);
                        break;
                }
            }
            catch (Exception ex)
            {
                try { log?.Invoke("装配上报处理失败(" + unit.Id + "): " + ex.Message); }
                catch { /* 日志出口自身异常:静默,绝不上抛打断对账(R2) */ }
            }
        }

        /// <summary>DataWrite 装配期写入的逐条隔离(R-A:让位/版本压制的单元不写,
        /// 直接跳过;失败只记日志,不阻断装配)。</summary>
        private static void TryDataWrite(string id, Action write)
        {
            if (route != null && route[id] != RouteDecision.Own) return;
            try
            {
                write();
            }
            catch (Exception ex)
            {
                log?.Invoke("数据写入失败(" + id + "): " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 安装器数据源(switchOn / versionBlocked,只读,状态机绝不回写)
        // ------------------------------------------------------------------

        /// <summary>
        /// 原键的期望开关态(F1 提为 internal,期望态求值的同源入口——
        /// SelfTestRunner 审计/Tier3 与调试面板①区一律经此,不得平行复刻分派):
        /// 分派单一事实源 = <see cref="SwitchDispatch.IsOn"/>(四条显式分派,
        /// 零异常驱动,9 布局键恒 false)。开关键经 Current 读取自带豁免(防死锁),
        /// 动态层/玩家层语义与面板一致。镜像键(remake.*/brood.*)同样经
        /// ItemSwitchKey 查子项开关(ConfigRegistry 镜像注册段,R-I)。
        /// 未装配(registry 快照为 null)恒 false——消费方(Ready 检查)本就
        /// 不该走到这里,守卫纯防炸。
        /// </summary>
        internal static bool SwitchOnFor(string key)
        {
            ConfigRegistry cfg = registry;
            return cfg != null && SwitchDispatch.IsOn(cfg, key);
        }

        /// <summary>
        /// 镜像键启用(R-J):读注册表当前值(开关 on,玩家层可见;PlayerLayerAllowed
        /// 语义与主面板一致)→ string(ConfigEntry.Format 公开静态,invariant,与
        /// engine 的 string 往返契约对称)→ 载体 Enable(快照对方现值 + 写入)。
        /// 装配期初始对账与分发器共用此路径。R9:Input 形态(IPH 的 CSV 文本键)
        /// 与 Choice 同走 string 直通。
        /// </summary>
        private static void MirrorEnable(FamilyPanelEntry e)
        {
            string ourValue;
            if (e.Kind == PanelKind.Choice || e.Kind == PanelKind.Input)
                ourValue = ConfigEntry.Format(registry.Current<string>(e.OurKey));
            else if (e.Kind == PanelKind.SliderInt)
                ourValue = ConfigEntry.Format(registry.Current<int>(e.OurKey));
            else if (e.Kind == PanelKind.SliderFloat)
                // 0.8.0(0x19 修复):SliderFloat 写透形态随 wt.main.* 首次出现——
                // 原分派漏 float 落 else 的 bool 读取 → InvalidCastException
                // (main.maiden_loot_chance/ovulation_loot_chance 初始对账与
                // 写透往返全挂);Write 侧(float TryParse)本就支持,仅读侧补
                ourValue = ConfigEntry.Format(registry.Current<float>(e.OurKey));
            else
                ourValue = ConfigEntry.Format(registry.Current<bool>(e.OurKey));
            WriteThroughCarrier.Enable(e, ourValue, log);
        }

        /// <summary>版本压制查询(R-B,0.6.0 版本严格绑定):按单元 TargetMod 查装配
        /// 期家族快照——目标在场(Present)且版本不命中绑定表(!VersionOk)→ 压制;
        /// TargetMod = null(游戏本体单元)恒 false;目标缺席不压制(MainHook 的
        /// 可装性由 install 委托自行裁决;SubModHook 缺席在 ③ 登记期拦截[R11-F1],
        /// 不进安装器)。快照在 ComposeAndInstall 时取一次存字段(会话内静态——
        /// mod 版本会话内不变)。</summary>
        private static bool VersionBlocked(string targetMod)
        {
            if (targetMod == null || family == null) return false;
            return family.TryGetValue(targetMod, out FamilyState st)
                && st.Present && !st.VersionOk;
        }

        /// <summary>家族快照在场查询(R11-F1 ③ 登记拦截的缺席判定;装配期
        /// family 字段已就绪,与 VersionBlocked 同一快照零偏差)。</summary>
        private static bool TargetPresent(string modId)
        {
            if (modId == null || family == null) return false;
            return family.TryGetValue(modId, out FamilyState st) && st.Present;
        }
    }
}
