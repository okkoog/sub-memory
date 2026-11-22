using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 自检机制(Task 14,R-U;R13b +LegacyOverlay):三级自检的执行途径分类。
    /// RouteAudit = Tier1 路由审计(单元级:期望态 vs 安装器实际态);
    /// InvokeStatic = Tier2 静态直测(ModReflect 直调主/子 mod 纯静态,临时写配置
    /// → 断言输出 → 复原;仅收确定性纯函数,RNG/跨天状态一律 ReadPath;
    /// R13b 起 brothel 组合键单元[guests/traits/visit/stock 四组 10 键]经
    /// WithSetupN 全套设值 + 传入确定性委托后亦可直断,升入本机制);
    /// SpawnItem = Tier2 物品直测(ModHelper.SpawnItem 走目录工厂 → 断言 unitValue/
    /// charge → MoveItemToTrash 清理);
    /// WriteThrough = Tier2 写透往返(镜像键 enable → 对方条目 == 我方值 →
    /// disable → 快照恢复;目标在场才执行,缺席 SKIP);
    /// LegacyOverlay = Tier2 legacy 叠加直测(R13b:legacy 1.0.6
    /// ContentFactoryPostfixes 五工厂 postfix 文本叠加与我方生成接管/价格键
    /// 的共存验证——SpawnItem + name/shortDescription 非空 + 我方键生效;
    /// 行 Key 为 legacy.* 伪键,不在配置键空间);
    /// ReadPath = Tier3 兜底(开关 on → 消费单元 Installed 且 Current 反映面板值;
    /// 完整行为链归游戏内手动验收)。
    /// </summary>
    public enum TestMechanism
    {
        RouteAudit,
        InvokeStatic,
        SpawnItem,
        WriteThrough,
        LegacyOverlay,
        ReadPath,
    }

    /// <summary>
    /// 自检目录行(R-U):Key = RouteAudit 行取 "route:"+单元 id,LegacyOverlay 行取
    /// legacy.* 伪键(见 LegacyOverlayKeys),其余行 = 配置键;
    /// ExecutorId = SelfTestRunner 执行器注册表的键(逐行闭包,前缀白名单
    /// {route.audit / invoke.* / spawn.* / wt.* / legacy.* / read.*} 由测试契约锁定);
    /// RequiresPresent = 执行前须在场的 mod id(逗号分隔,null = 无条件;
    /// 目标缺席时执行器 SKIP 而非 FAIL——缺席即未装配是设计态,不是缺陷)。
    /// </summary>
    public sealed class KeyTest
    {
        public string Key;
        public TestMechanism Mechanism;
        public string ExecutorId;
        public string RequiresPresent;
    }

    /// <summary>
    /// 单条自检结果(R-U):Id = 目录行 Key;Section = 分区标签(SectionOf);
    /// Pass = 通过(SKIP 计入 Pass,Detail 以 "SKIP" 起头);
    /// Detail = 断言明细/失败原因(面板失败明细区与 Player.log 同步)。
    /// </summary>
    public sealed class TestResult
    {
        public string Id;
        public string Section;
        public bool Pass;
        public string Detail;
    }

    /// <summary>
    /// 自检报告聚合(R-U):Total/Passed/Failed/Skipped 计数 + 失败明细清单;
    /// Skipped ⊆ Passed(目标缺席/装配未就绪的设计态跳过,不视为失败);
    /// Summary() 供面板与日志输出一行摘要。
    /// </summary>
    public sealed class TestReport
    {
        public int Total;
        public int Passed;
        public int Failed;
        public int Skipped;
        public readonly List<TestResult> Failures = new List<TestResult>();

        public string Summary()
        {
            return "共 " + Total + " 项:通过 " + Passed + "(其中 SKIP " + Skipped
                + ")/ 失败 " + Failed;
        }
    }

    /// <summary>
    /// 自检目录(Task 14,纯逻辑,R-U):「每配置键一行 + 每装配单元一行」的完整
    /// 目录,是「所有选项所有值」验收的单一事实源——覆盖集 = HookCatalog 全部
    /// 单元 Keys 的并集 ∪ FamilyPanelKeys 全部 OurKey(双射由 SelfTestPlanTests
    /// 契约 ① 锁定,新键漏登记即测试红)+ LegacyOverlay 伪键行(R13b,7 行,
    /// 契约 ① 按 RouteAudit 同例跳过;分布锁见 LegacyOverlayKeys)。
    /// 机制判定(R-V 裁定原则:能确定性断言输出值的才 Tier2,含 RNG/需要跨天
    /// 状态的进 Tier3——宁 ReadPath 勿假直测):
    ///  - InvokeStatic 30 键(ST5 后):progress.level_cap(GetBodyLevel,弱断言
    ///    详见 runner 注释)+ brothel 21 键(EffectiveBudget/SellsGoods+PriceHint/
    ///    ClampFee+ResolveFee/ItemValue×6 + R13b 组合键 10:guests×3/traits×2/
    ///    visit×2/stock×3——目标静态接受委托参数,传确定性委托后输出唯一;
    ///    mastery_threshold 的 TrainOneNight 写 tag 有状态,仍 ReadPath)
    ///    + 0.7.0 T11 新键 4:exp 公式三键(main.exp_formula prefix 消费,
    ///    WithSetupN 三键联设直调 ExpNeedForLevel,期望由测试值唯一决定)
    ///    + interact_threshold(ComputeInteractMax 需 GameItem——生成
    ///    maiden_lower 后直调,阈值 0/1000 差分断言,不依赖初值,详见 runner)
    ///    + ST 1.6.1 Task 5 疲劳售价四键(ValueMul 纯函数四键联设;对方
    ///    fatigue_enabled = "关" 时执行器查 get_Enabled SKIP);
    ///  - SpawnItem 17 键(0.7.0 T11 后):price.milk/semen/tentacle_box/
    ///    ovulation_shot(目录工厂 wondrous_milk/tentacle_semen/tentacle_box/
    ///    ovulation_shot 的 unitValue)+ corrupt.initial_charge(corrupt_tentacle
    ///    的 TENTACLE_CHARGE)+ alarm 两键 + trap 两键 + printer 两键
    ///    (tentacle_printer/empty_syringe 的 unitValue,Task 14b;grid/recipe_cost/
    ///    base_progress 消费在转换/进度逻辑 → ReadPath)+ iph.attr_* 五键
    ///    (R10:maiden_lower 生成后读三属性 tag,断言 = 配置三档并集的区间含入
    ///    低波动检查,见 SpawnItemKeys 注释)+ initial_virgin(T11:生成
    ///    maiden_lower 断言 MAIDEN_VIRGIN/MAIDEN_ANAL_VIRGIN 双 tag 状态 = 键值
    ///    ——postfix 形态的生成面直测,TurnOn 反向拨 false 即装配)。
    ///    price.milk_credit 不入 SpawnItem:其接管面是
    ///    onActivateSlotItemFunc 委托(激活时 AddCredit+Destroy,执行副作用 =
    ///    改存档资金并毁物,不可安全断言)→ ReadPath 兜底,此为降级记载;
    ///  - WriteThrough 键 = FamilyPanelKeys 中 Carrier == WriteThrough 的全部
    ///    OurKey(ST4 后 36:remake 8[7−3+4 桶装] + Brood 12 + IPH 16);
    ///  - 其余键 ReadPath(夜结算 22 键组/布局/疲劳/收购商/RNG 掷骰类
    ///    tearl 全部/brothel mastery[TrainOneNight 写 tag 有状态]/
    ///    remake 差分 14 键[乳汁差分委托闭包/夜间产出掷骰——均含
    ///    RNG 或跨天/存档状态依赖,宁兜底勿假直测;R13b 把 brothel 的
    ///    RollStock/NextVisitDay/guests/traits 四组 10 键经确定性委托注入
    ///    升入 InvokeStatic 后,「掷骰类」仅剩真实 RNG 键;ST4 撤
    ///    semen_formula 2 键 + alarm_activity 3 键,差分 19 → 14]
    ///    + ST5 疲劳 2 键[夜间基数依赖本体等级系数 LevelMul、透支恢复写
    ///    GameItem tag 有状态——各升半档不可靠,宁兜底勿假直测])。
    /// 目录顺序:RouteAudit(MainHook+SubModHook 单元,承 HookCatalog 条目序;
    /// 0x06 轮 F5:WriteThrough 单元不再生成审计行——无 install 委托,期望/实际
    /// 恒 NotRegistered 是恒真死行,且与值键行[写透往返]形成 wt.&lt;key&gt;/
    /// &lt;key&gt; 双行,统一为值键行一种形态)→ 值键行(键名字典序,
    /// Ordinal)→ LegacyOverlay 行(R13b,LegacyOverlayKeys 声明序)——
    /// 确定性排列,报告与日志可复现比对。
    /// </summary>
    public static class SelfTestPlan
    {
        /// <summary>RouteAudit 行 Key 前缀("route:"+unitId;剥离即单元 id)。</summary>
        internal const string RoutePrefix = "route:";

        /// <summary>SKIP 标记(Detail 起头;Aggregate 据此统计 Skipped)。</summary>
        internal const string SkipMark = "SKIP";

        /// <summary>
        /// InvokeStatic 集(0.7.0 T11 后 + ST5 共 30 键):经反编译/源码核实的
        /// 确定性纯静态目标。增删键须同步改 SelfTestRunner 的执行器注册与
        /// SelfTestPlanTests 的分布锁。
        /// R13b 新增 brothel 组合键 10(guests 三键/traits 两键/visit 两键/stock
        /// 三键):目标静态接受委托参数(GuestCount 的 isUnlocked / NextVisitDay 与
        /// RollStock 的 randomRange)——传确定性委托后输出唯一,升入直测
        /// (mastery_threshold 的 TrainOneNight 写 GameItem tag = 有状态,保持 ReadPath)。
        /// 0.7.0 T11 新增 4:exp 公式三键(main.exp_formula prefix 消费多键且
        /// 「任一非默认即接管」,WithSetupN 三键联设——期望由测试值唯一决定,
        /// 非主测键取原版默认字面量;直调 ExpNeedForLevel 断言公式合成值)
        /// + interact_threshold(ComputeInteractMax 签名含 GameItem——经
        /// SpawnItem 生成 maiden_lower 后直调,阈值 0 → 1+4=5、1000 → 1 的
        /// 差分断言:新生成少女四项读值远小于 1000 且非负,两界期望唯一,
        /// 不依赖具体初值;执行器归 WithSpawnSetup 系 = 需在存档内运行)。
        /// ST 1.6.1 Task 5 新增 4:疲劳售价四档(remake.fatigue_value_mul_*
        /// 单元四键联设——WithSetupN 主测一键、其余取原版默认,直调
        /// MaidenFatigue.ValueMul(档位代表值 10/60/80/90)断言;对方写透键
        /// fatigue_enabled = "关" 时 ValueMul 恒 1,执行器先查 get_Enabled,
        /// false → SKIP 设计态。夜间基数[LevelMul 依赖本体等级]与透支恢复
        /// [写 GameItem tag 有状态]宁兜底 → ReadPath)。
        /// </summary>
        internal static readonly HashSet<string> InvokeStaticKeys =
            new HashSet<string>(StringComparer.Ordinal)
            {
                CfgKeys.LevelCap,                       // TentacleBody.GetBodyLevel
                CfgKeys.ExpBase,                        // T11 TentacleBody.ExpNeedForLevel(三键联设)
                CfgKeys.ExpDivisor,                     // T11 同上
                CfgKeys.ExpStep,                        // T11 同上
                CfgKeys.InteractThreshold,              // T11 ItemFactory.ComputeInteractMax(生成 maiden 差分断言)
                CfgKeys.BrothelBudgetBonus,             // ServiceMath.EffectiveBudget
                CfgKeys.BrothelDebtTags,                // SellsGoods + PriceHint
                CfgKeys.BrothelDebtChance,              // SellsGoods
                CfgKeys.BrothelFeeDefault,              // MaidenTraits.ResolveFee
                CfgKeys.BrothelFeeMax,                  // ClampFee + ResolveFee
                CfgKeys.BrothelPriceMaidDress,          // BrothelItemCatalog.ItemValue ×6
                CfgKeys.BrothelPriceCatEars,
                CfgKeys.BrothelPriceCatTailPlug,
                CfgKeys.BrothelPriceLaceDress,
                CfgKeys.BrothelPriceBunnyEars,
                CfgKeys.BrothelPriceLaceGarter,
                CfgKeys.BrothelGuestsBase,              // R13b GuestCapacity.GuestsAfterUpgrade/GuestCount(三键联设)
                CfgKeys.BrothelGuestsPerUpgrade,        // R13b 同上
                CfgKeys.BrothelGuestsMax,               // R13b 同上(双断言 GuestsAfterUpgrade+GuestCount)
                CfgKeys.BrothelTraitsBase,              // R13b TraitCategories.RequirementCount(两键联设)
                CfgKeys.BrothelTraitsPerUpgrade,        // R13b 同上
                CfgKeys.BrothelVisitMinDays,            // R13b MerchantSchedule.NextVisitDay(det 委托,两键联设)
                CfgKeys.BrothelVisitMaxDays,            // R13b 同上(捕获区间上界 = max+1)
                CfgKeys.BrothelStockFirst,              // R13b GearCatalog.RollStock(det 委托,三键联设)
                CfgKeys.BrothelStockMin,                // R13b 同上
                CfgKeys.BrothelStockMax,                // R13b 同上(捕获区间上界 = max+1)
                CfgKeys.RemakeFatigueValueMulRested,    // ST5 MaidenFatigue.ValueMul(四键联设,档位代表值 10)
                CfgKeys.RemakeFatigueValueMulTired,     // ST5 同上(代表值 60)
                CfgKeys.RemakeFatigueValueMulWeary,     // ST5 同上(代表值 80)
                CfgKeys.RemakeFatigueValueMulExhausted, // ST5 同上(代表值 90)
            };

        /// <summary>
        /// SpawnItem 集(0.7.0 T11 后 17 键):目录工厂可生成且断言面确定
        /// (unitValue/charge/属性 tag/处女双 tag)、清理路径安全
        /// (MoveItemToTrash)的键。milk_credit 降级 ReadPath(表头注释);
        /// Task 14b +printer 两键(tentacle_printer/empty_syringe 目录工厂直测,
        /// grid/recipe_cost/base_progress 消费在转换/进度逻辑 → ReadPath);
        /// R10 +iph.attr_* 五键(maiden_lower 工厂生成后读 MAIDEN_FORCE/INTEL/CHARM
        /// 三 tag,断言 = 三档并集区间含入的低波动检查:测试值整体逃出原版
        /// [10,80] 域,未装上 postfix 则必落并集外——检出率 100%,掷档随机性
        /// 不影响断言,执行器见 SelfTestRunner);T11 +initial_virgin(生成
        /// maiden_lower 断言 MAIDEN_VIRGIN/MAIDEN_ANAL_VIRGIN 双 tag 已移除
        /// ——键值 false = postfix 接管态;TurnOn 反向拨 false 经 SwitchDispatch
        /// T9 条装配,与玩家面板「关闭开关」同一条链)。
        /// </summary>
        internal static readonly HashSet<string> SpawnItemKeys =
            new HashSet<string>(StringComparer.Ordinal)
            {
                CfgKeys.PriceMilk,              // wondrous_milk.unitValue
                CfgKeys.PriceSemen,             // tentacle_semen.unitValue
                CfgKeys.PriceTentacleBox,       // tentacle_box.unitValue
                CfgKeys.PriceOvulationShot,     // ovulation_shot.unitValue
                CfgKeys.CorruptInitialCharge,   // corrupt_tentacle TENTACLE_CHARGE
                CfgKeys.AlarmPrice,             // alarm_clock.unitValue
                CfgKeys.AlarmTentaclePrice,     // tentacle_alarm.unitValue
                CfgKeys.TrapPrice,              // manhole_cover.unitValue
                CfgKeys.TrapTrapPrice,          // tentacle_trap.unitValue
                CfgKeys.PrinterPrice,           // tentacle_printer.unitValue(CreatePrinter 工厂)
                CfgKeys.PrinterNeedlePrice,     // empty_syringe.unitValue(CreateNeedle 工厂)
                CfgKeys.IphAttrHighChance,      // R10:maiden_lower 三属性 tag
                CfgKeys.IphAttrMidChance,       // R10:同上(五键共一生成路径,主测键各异)
                CfgKeys.IphAttrHighRange,       // R10:同上
                CfgKeys.IphAttrMidRange,        // R10:同上
                CfgKeys.IphAttrLowRange,        // R10:同上
                CfgKeys.InitialVirgin,          // T11:maiden_lower 处女双 tag 状态
            };

        /// <summary>
        /// LegacyOverlay 行集(R13b,7 行):legacy 1.0.6 ContentFactoryPostfixes 的
        /// 五工厂 postfix(CreateMaiden/CreateMilk/CreateSemen/CreateOvulationShot/
        /// TentacleBody.Create)文本叠加 × 我方可断言键面的共存验证。行 Key 为
        /// 伪键(不在配置键空间,双射契约按 RouteAudit 同例跳过):
        ///  - milk/semen/ovulation 三行 = Create* postfix 文本 + 我方价格键
        ///    unitValue(price.milk/semen/ovulation_shot;SpawnItem 执行器的
        ///    legacy 在场变体——叠加断言 = name/shortDescription 非空);
        ///  - tentacle_body 行 = TentacleBody.Create postfix 文本 + main.create
        ///    布局键(tabs_per_level → STORAGE_EXPANSION_TABS_INT);
        ///  - maiden_pools 行 = CreateMaiden postfix × maiden.factions 单池
        ///    (IPH 缺席才 Own,在场时 fbb/生成让位 → SKIP);
        ///  - maiden_iph 行 = CreateMaiden postfix × iph.attr_* 五键(IphAttrSpawn
        ///    的 legacy 在场变体;RequiresPresent = legacy+iph 双在场);
        ///  - maiden_overlay 行 = 叠加双向证据:我方 factions 单池生效 + legacy
        ///    Apply 的 MAIDEN_ADULT tag(原版无此 tag,唯一来源)+ MAIDEN_PERSONALITY
        ///    非空(legacy 写入)。
        /// </summary>
        internal static readonly HashSet<string> LegacyOverlayKeys =
            new HashSet<string>(StringComparer.Ordinal)
            {
                "legacy.milk",
                "legacy.semen",
                "legacy.ovulation",
                "legacy.tentacle_body",
                "legacy.maiden_pools",
                "legacy.maiden_iph",
                "legacy.maiden_overlay",
            };

        /// <summary>自检目录(RouteAudit 行 + 值键行;构造见类注释)。</summary>
        public static readonly IReadOnlyList<KeyTest> KeyTests = Build();

        /// <summary>机制 → 分区标签(TestResult.Section 的单一来源,面板按它分组)。</summary>
        internal static string SectionOf(TestMechanism mechanism)
        {
            if (mechanism == TestMechanism.RouteAudit) return "Tier1·路由审计";
            if (mechanism == TestMechanism.InvokeStatic) return "Tier2·静态直测";
            if (mechanism == TestMechanism.SpawnItem) return "Tier2·物品直测";
            if (mechanism == TestMechanism.WriteThrough) return "Tier2·写透往返";
            if (mechanism == TestMechanism.LegacyOverlay) return "Tier2·legacy叠加";
            return "Tier3·读取路径";
        }

        /// <summary>Detail 是否 SKIP(目标缺席/装配未就绪的设计态跳过)。</summary>
        internal static bool IsSkip(string detail)
        {
            return detail != null
                && detail.StartsWith(SkipMark, StringComparison.Ordinal);
        }

        /// <summary>
        /// 结果序列 → 报告聚合(计数与失败清单;契约 ④ 的被测对象)。
        /// 输入为 null 的条目直接跳过(执行器异常兜底产物不进统计)。
        /// </summary>
        public static TestReport Aggregate(IEnumerable<TestResult> results)
        {
            var report = new TestReport();
            foreach (TestResult r in results)
            {
                if (r == null) continue;
                report.Total++;
                if (r.Pass)
                {
                    report.Passed++;
                    if (IsSkip(r.Detail)) report.Skipped++;
                }
                else
                {
                    report.Failed++;
                    report.Failures.Add(r);
                }
            }
            return report;
        }

        // ------------------------------------------------------------------
        // 目录构造(纯:HookCatalog + FamilyPanelKeys 两表推导,零手写键名)
        // ------------------------------------------------------------------

        private static List<KeyTest> Build()
        {
            var list = new List<KeyTest>();

            // Tier1:每装配单元一行(ExecutorId = "route.audit:"+unitId,以
            // "route.audit" 为前缀满足 R-U;runner 逐单元闭包注册)。
            // 0x06 轮 F5:WriteThrough 单元不生成——无 install 委托,期望/实际
            // 恒 NotRegistered(恒真死行),且与值键行(写透往返)形成
            // wt.<key>/<key> 双行;写透单元的真自检由值键行承担,统一为一种形态
            foreach (HookUnitDef u in HookCatalog.Units)
            {
                if (u.Kind == CarrierKind.WriteThrough) continue;
                list.Add(new KeyTest
                {
                    Key = RoutePrefix + u.Id,
                    Mechanism = TestMechanism.RouteAudit,
                    ExecutorId = "route.audit:" + u.Id,
                    RequiresPresent = null, // 路由审计只读装配期快照,无在场要求
                });
            }

            // 值键行:覆盖集 = 目录单元 Keys 并集 ∪ 镜像表 OurKey(双射契约 ①)
            var covered = new HashSet<string>(StringComparer.Ordinal);
            foreach (HookUnitDef u in HookCatalog.Units)
                foreach (string k in u.Keys)
                    covered.Add(k);
            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
                covered.Add(e.OurKey);

            var keys = new List<string>(covered);
            keys.Sort(StringComparer.Ordinal);
            foreach (string key in keys)
            {
                TestMechanism mechanism;
                string executorId;
                if (InvokeStaticKeys.Contains(key))
                {
                    mechanism = TestMechanism.InvokeStatic;
                    executorId = "invoke." + key;
                }
                else if (SpawnItemKeys.Contains(key))
                {
                    mechanism = TestMechanism.SpawnItem;
                    executorId = "spawn." + key;
                }
                else if (FamilyPanelKeys.TryGet(key, out FamilyPanelEntry e)
                    && e.Carrier == CarrierKind.WriteThrough)
                {
                    mechanism = TestMechanism.WriteThrough;
                    executorId = "wt." + key;
                }
                else
                {
                    mechanism = TestMechanism.ReadPath;
                    executorId = "read." + key;
                }
                list.Add(new KeyTest
                {
                    Key = key,
                    Mechanism = mechanism,
                    ExecutorId = executorId,
                    RequiresPresent = RequiresFor(key),
                });
            }

            // LegacyOverlay 行(R13b):legacy.* 伪键,固定序(键集声明序);RequiresPresent
            // = legacy(maiden_iph 行另加 iph——五属性键的执行器要求双在场)。
            // ExecutorId = 伪键本身(已带 legacy. 前缀,满足白名单)。
            foreach (string key in LegacyOverlayKeys)
            {
                list.Add(new KeyTest
                {
                    Key = key,
                    Mechanism = TestMechanism.LegacyOverlay,
                    ExecutorId = key,
                    RequiresPresent = key == "legacy.maiden_iph"
                        ? FamilyMods.Legacy + "," + FamilyMods.Iph
                        : FamilyMods.Legacy,
                });
            }
            return list;
        }

        /// <summary>
        /// 值键行的在场要求:镜像键(remake.*/brood.*/brothel.*/tearl.*)= 对方
        /// TargetMod;主面板键 = 主 mod(全部消费单元含 TargetMod == lewd_meme 的
        /// 条目,gender_scope 虽另有 game.* 单元,main.on_generated 同键消费——
        /// 主 mod 缺席即整体 SKIP 属设计态,game.* 两单元的期望/实际由 Tier1 覆盖)。
        /// </summary>
        private static string RequiresFor(string key)
        {
            if (FamilyPanelKeys.TryGet(key, out FamilyPanelEntry e))
                return e.TargetMod;
            return FamilyMods.MainMod;
        }
    }
}
