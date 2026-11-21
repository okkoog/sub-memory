using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 自检执行器(Task 14,R-V,游戏侧;R13b +LegacyOverlay/组合键直测):
    /// SelfTestPlan 目录逐行对应的执行器注册表(executorId → 无参闭包)+ 各域
    /// runner。三级自检的执行语义:
    ///  - Tier1 路由审计:期望态(Router 装配期快照 × 开关 × 载体)vs 安装器
    ///    实际态(States 快照)——只读,零临时写,零游戏状态依赖;
    ///  - Tier2 静态/物品/写透/legacy叠加:测试期临时写(拨开关键 → 热对账装上 → 设
    ///    非默认测试值 → 断言 → finally 复原开关与值)。拨开关与玩家面板操作
    ///    同路径(registry.Changed → HookRuntime.OnSwitchChanged → 安装器对账),
    ///    测的就是真实装配链;复原后状态与测试前一致(零行为漂移红线)。
    ///    R13b:①多键组合单元(brothel guests/traits/visit/stock)经 WithSetupN
    ///    全套设值——prefix「任一非默认即接管」+ 其余键保持玩家原值会污染期望,
    ///    逐键全套设值后期望唯一;②委托参数注入确定性(RollStock/NextVisitDay 的
    ///    randomRange、GuestCount 的 isUnlocked 传 det 委托,RNG 静态变可直断);
    ///    ③LegacyOverlay = SpawnItem + 文本叠加断言(legacy 1.0.6 五工厂 postfix
    ///    与我方生成接管/价格键的共存验证,legacy 内容未加载 SKIP);
    ///  - Tier3 读取路径:开关 on → 消费单元 Installed 且 Current 读回一致,
    ///    完整行为链归游戏内手动验收。
    /// 存档安全红线:SpawnItem 路径逐物品 MoveItemToTrash 清理(垃圾桶 = 丢弃,
    /// 不留永久物品);WriteThrough 往返结束 Disable 恢复快照(finally 兜底);
    /// 我方开关与值全部复原(等于默认时玩家层自动移除 = 回到未干预态)。
    /// 目标缺席/装配未就绪 = SKIP(Pass 计入、Detail 带 SKIP 前缀):
    /// 缺席即未装配是设计态,不是缺陷;在场且版本绑定却失败才是 FAIL。
    /// 每条结果由调用方(DebugPanel)同步 Player.log 一行。
    /// </summary>
    internal static class SelfTestRunner
    {
        private static readonly Dictionary<string, Func<TestResult>> Executors = BuildExecutors();

        /// <summary>执行单行(目录行 → 结果;Section 统一按机制填充分区标签;
        /// 未注册执行器 = 目录与注册表失配,直接 FAIL)。</summary>
        internal static TestResult Run(KeyTest t)
        {
            TestResult r;
            Func<TestResult> exec;
            if (!Executors.TryGetValue(t.ExecutorId, out exec))
                r = Fail(t.Key, "실행기가 등록되지 않음: " + t.ExecutorId);
            else
            {
                try { r = exec(); }
                catch (Exception ex) { r = Fail(t.Key, "실행기에서 예외 발생: " + ex.Message); }
            }
            r.Section = SelfTestPlan.SectionOf(t.Mechanism);
            return r;
        }

        /// <summary>面板分按钮过滤:Tier1 / Tier2(静态+物品+写透+legacy叠加)/ Tier3 / 全量。</summary>
        internal static List<KeyTest> Tier(int tier)
        {
            var list = new List<KeyTest>();
            foreach (KeyTest t in SelfTestPlan.KeyTests)
            {
                bool hit = tier == 0
                    || (tier == 1 && t.Mechanism == TestMechanism.RouteAudit)
                    || (tier == 2 && (t.Mechanism == TestMechanism.InvokeStatic
                        || t.Mechanism == TestMechanism.SpawnItem
                        || t.Mechanism == TestMechanism.WriteThrough
                        || t.Mechanism == TestMechanism.LegacyOverlay))
                    || (tier == 3 && t.Mechanism == TestMechanism.ReadPath);
                if (hit) list.Add(t);
            }
            return list;
        }

        // ------------------------------------------------------------------
        // 期望态 / 实际态(Tier1 与面板①区共用,装配期快照同源零偏差)
        // ------------------------------------------------------------------

        /// <summary>装配是否就绪(自检前置;主菜单/让位时 Installer 未建 → SKIP)。</summary>
        internal static bool Ready
        {
            get { return HookRuntime.Installer != null && HookRuntime.RouteSnapshot != null; }
        }

        /// <summary>
        /// 单元期望态:RouteExpect.Expected(纯函数内核,F1 抽出)的快照包装——
        /// 载体前置过滤 + Ready 检查 + 采集 RouteSnapshot/R5ExemptUnits/开关聚合。
        /// 开关聚合经 <see cref="HookRuntime.SwitchOnFor"/>(F4:与装配路径同源,
        /// 平行复刻已收敛)。R5 豁免单元(remake/IPH 让位分支无 install 委托)在
        /// decision == VersionBlocked 时与登记过滤对齐返回 NotRegistered(F1:主
        /// mod 版本不命中 × remake/IPH 在场组合下,Router 裁 VersionBlocked 但
        /// ③ 不登记,期望态若仍报 VersionBlocked 则审计假红)。SubModHook 目标
        /// 缺席单元(R11-F1:③ 登记期拦截,AbsentTargetUnits 同一装配期快照)
        /// 期望态 NotRegistered——跨族键(主面板 Define、消费单元含缺席目标)开关
        /// 开也一样:缺席单元物理零 hook 是设计态,0x0f 根因即期望态旧报
        /// Installed vs 实际 Dead 的假红。与 HookRuntime 装配登记过滤、
        /// HookInstaller TargetState 同语义(Dead 由实际态暴露:开关开却
        /// Dead = FAIL)。
        /// </summary>
        internal static string ExpectedState(HookUnitDef def)
        {
            if (def.Kind != CarrierKind.MainHook && def.Kind != CarrierKind.SubModHook)
                return "NotRegistered";
            if (!Ready) return "未装配";
            bool anySwitchOn = false;
            if (def.Keys != null)
                foreach (string k in def.Keys)
                    if (HookRuntime.SwitchOnFor(k)) { anySwitchOn = true; break; }
            return RouteExpect.Expected(def, HookRuntime.RouteSnapshot[def.Id],
                anySwitchOn, IsR5Exempt(def.Id), IsTargetAbsent(def));
        }

        /// <summary>单元是否在装配期 R5 豁免集合内(让位分支无委托,登记期跳过;
        /// 未装配快照缺失按非豁免——外层 Ready 检查已先行)。</summary>
        private static bool IsR5Exempt(string unitId)
        {
            var exempt = HookRuntime.R5ExemptUnits;
            if (exempt == null) return false;
            foreach (string id in exempt)
                if (id == unitId) return true;
            return false;
        }

        /// <summary>单元是否属「TargetMod 缺席 → ③ 登记期拦截」集合(R11-F1;
        /// 与 HookRuntime 拦截同一装配期快照,非 SubModHook 恒 false;未装配
        /// 快照缺失按非缺席——外层 Ready 检查已先行)。</summary>
        private static bool IsTargetAbsent(HookUnitDef def)
        {
            if (def.Kind != CarrierKind.SubModHook) return false;
            var absent = HookRuntime.AbsentTargetUnits;
            if (absent == null) return false;
            foreach (string id in absent)
                if (id == def.Id) return true;
            return false;
        }

        /// <summary>单元实际态:安装器 States 快照(未装配整体「未装配」)。</summary>
        internal static string ActualState(HookUnitDef def)
        {
            if (HookRuntime.Installer == null) return "未装配";
            HookState st;
            return HookRuntime.Installer.States.TryGetValue(def.Id, out st)
                ? st.ToString() : "NotRegistered";
        }

        // ------------------------------------------------------------------
        // 执行器注册(逐行闭包:route.audit[MainHook+SubModHook 单元,ST5 后
        // 75] + invoke×30
        // [R13b +brothel 组合键 10;0.7.0 T11 +exp 三键/interact_threshold;
        // ST5 +疲劳售价四档]
        // + spawn×17[R10 +iph.attr_* 5;T11 +initial_virgin] + wt×[写透镜像数,
        // ST4 后 36:remake 8 + Brood 12 + IPH 16] + legacy×7[R13b LegacyOverlay]
        // + read×[值键行 - 前五者;ST5 后 106];
        // F5 后 WriteThrough 单元不再生成 route.audit 行——写透键的活自检
        // 在 wt.* 值键行)
        // ------------------------------------------------------------------

        private static Dictionary<string, Func<TestResult>> BuildExecutors()
        {
            var map = new Dictionary<string, Func<TestResult>>(StringComparer.Ordinal);

            // ---- Tier1:路由审计(每单元一闭包;WriteThrough 单元无目录行,
            //      同步不注册——与 SelfTestPlan.Build 的过滤同一裁定,F5)----
            foreach (HookUnitDef def in HookCatalog.Units)
            {
                if (def.Kind == CarrierKind.WriteThrough) continue;
                HookUnitDef captured = def;
                map["route.audit:" + def.Id] = () => RouteAuditOne(captured);
            }

            // ---- Tier2a:InvokeStatic(确定性纯静态直调;委托每次执行现解析,
            //      解析异常 = FAIL 而非 SKIP——在场且版本绑定却解析失败是真 bug)----

            map["invoke." + CfgKeys.LevelCap] = () => WithSetup(FamilyMods.MainMod,
                CfgKeys.LevelCap, 1, key =>
                {
                    // 弱断言:cap=1 时 clamp(body_lv,1,1) 恒 1(body_lv 原值任意);
                    // 玩家 body_lv 恰为 1 时未接管也返回 1(无区分度,明细注明)
                    int r = ModReflect.StaticMethod<Func<int>>(
                        MainType("TentacleBody"), "GetBodyLevel")();
                    return r == 1
                        ? Pass(key, "GetBodyLevel()==1(cap=1; body_lv=1이면 차이를 구분할 수 없음)")
                        : Fail(key, "GetBodyLevel 예상 1 / 실제 " + r);
                });

            // ---- Tier2a(0.7.0 T11):exp 公式三键(main.exp_formula prefix 消费
            //      多键,WithSetupN 三键联设——期望由测试值唯一决定;直调
            //      ExpNeedForLevel(7):原版 = 100+6/3*100 = 300,三行各有区分度)----

            string[] expKeys = { CfgKeys.ExpBase, CfgKeys.ExpDivisor, CfgKeys.ExpStep };

            map["invoke." + CfgKeys.ExpBase] = () => WithSetupN(FamilyMods.MainMod,
                expKeys, new object[] { 200, 3, 100 }, CfgKeys.ExpBase, key =>
                {
                    // 200 + 6/3*100 = 400(base 独占;原版 300)
                    // 委托执行期现解析(解析异常 = FAIL,与 brothel 同口径)
                    int r = ModReflect.StaticMethod<Func<int, int>>(
                        MainType("TentacleBody"), "ExpNeedForLevel")(7);
                    return r == 400 ? Pass(key, "ExpNeedForLevel(7)=" + r + "(base=200)")
                        : Fail(key, "ExpNeedForLevel(7) 예상 400 / 실제 " + r);
                });

            map["invoke." + CfgKeys.ExpDivisor] = () => WithSetupN(FamilyMods.MainMod,
                expKeys, new object[] { 100, 2, 100 }, CfgKeys.ExpDivisor, key =>
                {
                    // 100 + 6/2*100 = 400(除数收紧;原版 6/3 → 300)
                    int r = ModReflect.StaticMethod<Func<int, int>>(
                        MainType("TentacleBody"), "ExpNeedForLevel")(7);
                    return r == 400 ? Pass(key, "ExpNeedForLevel(7)=" + r + "(divisor=2)")
                        : Fail(key, "ExpNeedForLevel(7) 예상 400 / 실제 " + r);
                });

            map["invoke." + CfgKeys.ExpStep] = () => WithSetupN(FamilyMods.MainMod,
                expKeys, new object[] { 100, 3, 50 }, CfgKeys.ExpStep, key =>
                {
                    // 100 + 6/3*50 = 200(增量减半;原版 300)
                    int r = ModReflect.StaticMethod<Func<int, int>>(
                        MainType("TentacleBody"), "ExpNeedForLevel")(7);
                    return r == 200 ? Pass(key, "ExpNeedForLevel(7)=" + r + "(step=50)")
                        : Fail(key, "ExpNeedForLevel(7) 예상 200 / 실제 " + r);
                });

            map["invoke." + CfgKeys.InteractThreshold] = () => WithSpawnSetup(
                FamilyMods.MainMod, CfgKeys.InteractThreshold, 0, key =>
                {
                    // ComputeInteractMax(GameItem) 需少女作参数:生成 maiden_lower
                    // 后差分断言——阈值 0:四项读值(GetInt 非负)各 ≥0 恒真 → 1+4=5;
                    // 阈值 1000:新生成少女四项远小于 1000 → 恒 1。两界期望唯一,
                    // 不依赖具体初值(0 与 1000 都 ≠ 60 → 均走我方复刻体 = 直测接管态)
                    var compute = ModReflect.StaticMethod<Func<GameItem, int>>(
                        MainType("ItemFactory"), "ComputeInteractMax");
                    GameItem maid = ModHelper.SpawnItem("maiden_lower");
                    if (maid == null) return Fail(key, "아이템 목록에 maiden_lower가 등록되지 않음(저장 파일에서 실행 필요)");
                    try
                    {
                        int loose = compute(maid);
                        SetInt(CfgKeys.InteractThreshold, 1000);
                        int tight = compute(maid);
                        return loose == 5 && tight == 1
                            ? Pass(key, "ComputeInteractMax 임계값 0→" + loose + " / 1000→" + tight + "(차이 검증)")
                            : Fail(key, "임계값 차이: 0→" + loose + "(예상 5) / 1000→" + tight + "(예상 1)");
                    }
                    finally
                    {
                        ModHelper.MoveItemToTrash(maid);
                    }
                });

            // ---- Tier2a(ST 1.6.1 Task 5):疲劳售价四档(remake.fatigue_value_mul
            //      单元「任一非默认即接管」,WithSetupN 四键联设——非主测键取
            //      原版默认,期望由测试值唯一决定;直调 MaidenFatigue.ValueMul,
            //      档位代表值 10/60/80/90 各主测一键;对方写透键 fatigue_enabled
            //      = "끄기" 时 Enabled=false → ValueMul 恒 1,查 get_Enabled SKIP)----

            string[] fatigueMulKeys =
            {
                CfgKeys.RemakeFatigueValueMulRested, CfgKeys.RemakeFatigueValueMulTired,
                CfgKeys.RemakeFatigueValueMulWeary, CfgKeys.RemakeFatigueValueMulExhausted,
            };

            map["invoke." + CfgKeys.RemakeFatigueValueMulRested] = () => WithSetupN(FamilyMods.Remake,
                fatigueMulKeys, new object[] { 1.3f, 0.94f, 0.88f, 0.8f },
                CfgKeys.RemakeFatigueValueMulRested,
                key => FatigueMulCase(key, 10, 1.3f, "충분히 쉰 상태(<20)"));

            map["invoke." + CfgKeys.RemakeFatigueValueMulTired] = () => WithSetupN(FamilyMods.Remake,
                fatigueMulKeys, new object[] { 1.1f, 0.5f, 0.88f, 0.8f },
                CfgKeys.RemakeFatigueValueMulTired,
                key => FatigueMulCase(key, 60, 0.5f, "피곤한 상태(≥60)"));

            map["invoke." + CfgKeys.RemakeFatigueValueMulWeary] = () => WithSetupN(FamilyMods.Remake,
                fatigueMulKeys, new object[] { 1.1f, 0.94f, 0.6f, 0.8f },
                CfgKeys.RemakeFatigueValueMulWeary,
                key => FatigueMulCase(key, 80, 0.6f, "피로한 상태(≥80)"));

            map["invoke." + CfgKeys.RemakeFatigueValueMulExhausted] = () => WithSetupN(FamilyMods.Remake,
                fatigueMulKeys, new object[] { 1.1f, 0.94f, 0.88f, 0.5f },
                CfgKeys.RemakeFatigueValueMulExhausted,
                key => FatigueMulCase(key, 90, 0.5f, "과로 상태(≥90)"));

            map["invoke." + CfgKeys.BrothelBudgetBonus] = () => WithSetup(FamilyMods.Brothel,
                CfgKeys.BrothelBudgetBonus, 25, key =>
                {
                    // 200*(100+25*2)/100 = 300(原版 10 → 240)
                    int r = BrothelFunc<Func<int, int, int>>("ServiceMath", "EffectiveBudget")(200, 2);
                    return r == 300 ? Pass(key, "EffectiveBudget(200,2)=" + r + "(보정 +25%)")
                        : Fail(key, "EffectiveBudget(200,2) 예상 300 / 실제 " + r);
                });

            map["invoke." + CfgKeys.BrothelDebtTags] = () => WithSetup(FamilyMods.Brothel,
                CfgKeys.BrothelDebtTags, 3, key =>
                {
                    // 线降到 3:matches=3 时 "debt"(原版 unsellable)、可负债(原版拒绝)
                    string hint = BrothelFunc<Func<int, int, int, string>>("ServiceMath", "PriceHint")(300, 200, 3);
                    bool sell = BrothelFunc<Func<int, int, int, int, bool>>("ServiceMath", "SellsGoods")(300, 200, 3, 0);
                    return hint == "debt" && sell
                        ? Pass(key, "PriceHint(300,200,3)=debt이며 SellsGoods(...,0)=true(외상 기준 3)")
                        : Fail(key, "외상 기준 3: hint=" + hint + " sell=" + sell);
                });

            map["invoke." + CfgKeys.BrothelDebtChance] = () => WithSetup(FamilyMods.Brothel,
                CfgKeys.BrothelDebtChance, 100, key =>
                {
                    var sells = BrothelFunc<Func<int, int, int, int, bool>>("ServiceMath", "SellsGoods");
                    bool all = sells(300, 200, 5, 99);   // 99 < 100 → 必买(原版 false)
                    SetInt(CfgKeys.BrothelDebtChance, 0);
                    bool none = sells(300, 200, 5, 0);   // 0 < 0 → 必拒
                    return all && !none
                        ? Pass(key, "확률 100→반드시 구매 / 0→반드시 거절")
                        : Fail(key, "확률 경계: 100% 구매=" + all + " 0% 구매=" + none);
                });

            map["invoke." + CfgKeys.BrothelFeeDefault] = () => WithSetup(FamilyMods.Brothel,
                CfgKeys.BrothelFeeDefault, 200, key =>
                {
                    // 无费用标签 → 默认值(原版 120)
                    int r = BrothelFunc<Func<bool, int, int>>("MaidenTraits", "ResolveFee")(false, 0);
                    return r == 200 ? Pass(key, "ResolveFee(false,0)=" + r)
                        : Fail(key, "ResolveFee 예상 200 / 실제 " + r);
                });

            map["invoke." + CfgKeys.BrothelFeeMax] = () => WithSetup(FamilyMods.Brothel,
                CfgKeys.BrothelFeeMax, 500, key =>
                {
                    var clamp = BrothelFunc<Func<int, int>>("MaidenTraits", "ClampFee");
                    var resolve = BrothelFunc<Func<bool, int, int>>("MaidenTraits", "ResolveFee");
                    int c = clamp(9999);                 // 钳到 500(原版 9999)
                    int r = resolve(true, 9999);         // 有标签路径同钳
                    return c == 500 && r == 500 ? Pass(key, "ClampFee(9999)=" + c)
                        : Fail(key, "상한 500: Clamp=" + c + " Resolve=" + r);
                });

            SpawnPrice(map, CfgKeys.BrothelPriceMaidDress, "lewd_meme_brothel.gear_maid_dress", 360);
            SpawnPrice(map, CfgKeys.BrothelPriceCatEars, "lewd_meme_brothel.gear_cat_ears", 280);
            SpawnPrice(map, CfgKeys.BrothelPriceCatTailPlug, "lewd_meme_brothel.gear_cat_tail_plug", 320);
            SpawnPrice(map, CfgKeys.BrothelPriceLaceDress, "lewd_meme_brothel.gear_lace_dress", 340);
            SpawnPrice(map, CfgKeys.BrothelPriceBunnyEars, "lewd_meme_brothel.gear_bunny_ears", 275);
            SpawnPrice(map, CfgKeys.BrothelPriceLaceGarter, "lewd_meme_brothel.gear_lace_garter", 250);

            // ---- Tier2a(R13b):brothel 组合键 10(guests/traits/visit/stock 四组,
            //      WithSetupN 全套设值——prefix 消费多键且「任一非默认即接管」,
            //      逐键全套设值杜绝玩家既有非默认值污染期望;委托参数传确定性
            //      委托,输出唯一)----

            string[] guestKeys = { CfgKeys.BrothelGuestsBase, CfgKeys.BrothelGuestsPerUpgrade, CfgKeys.BrothelGuestsMax };

            map["invoke." + CfgKeys.BrothelGuestsBase] = () => WithSetupN(FamilyMods.Brothel,
                guestKeys, new object[] { 4, 2, 20 }, CfgKeys.BrothelGuestsBase, key =>
                {
                    // Clamp(4 + 2*0, Min(4,20), 20) = 4(基座独占增量;原版 2 → 2)
                    int r = BrothelFunc<Func<int, int>>("GuestCapacity", "GuestsAfterUpgrade")(0);
                    return r == 4 ? Pass(key, "GuestsAfterUpgrade(0)=" + r + "(base=4,per=2)")
                        : Fail(key, "GuestsAfterUpgrade(0) 예상 4 / 실제 " + r);
                });

            map["invoke." + CfgKeys.BrothelGuestsPerUpgrade] = () => WithSetupN(FamilyMods.Brothel,
                guestKeys, new object[] { 2, 5, 20 }, CfgKeys.BrothelGuestsPerUpgrade, key =>
                {
                    // Clamp(2 + 5*2, 2, 20) = 12(增量独占;原版 2 → 6)
                    int r = BrothelFunc<Func<int, int>>("GuestCapacity", "GuestsAfterUpgrade")(2);
                    return r == 12 ? Pass(key, "GuestsAfterUpgrade(2)=" + r + "(per=5)")
                        : Fail(key, "GuestsAfterUpgrade(2) 예상 12 / 실제 " + r);
                });

            map["invoke." + CfgKeys.BrothelGuestsMax] = () => WithSetupN(FamilyMods.Brothel,
                guestKeys, new object[] { 2, 2, 6 }, CfgKeys.BrothelGuestsMax, key =>
                {
                    // 上界钳:AfterUpgrade(9) = Clamp(20, 2, 6) = 6;GuestCount 满扩建
                    // = Clamp(2+2*3, 2, 6) = 6(isUnlocked 恒 true → 3 项扩建全开)
                    var afterUpgrade = BrothelFunc<Func<int, int>>("GuestCapacity", "GuestsAfterUpgrade");
                    var guestCount = BrothelFunc<Func<Func<string, bool>, int, int>>("GuestCapacity", "GuestCount");
                    int a = afterUpgrade(9);
                    int g = guestCount(_ => true, 0);
                    return a == 6 && g == 6
                        ? Pass(key, "상한 6: GuestsAfterUpgrade(9)=" + a + " GuestCount(전체 확장)=" + g)
                        : Fail(key, "상한 6: AfterUpgrade(9)=" + a + " GuestCount=" + g);
                });

            string[] traitKeys = { CfgKeys.BrothelTraitsBase, CfgKeys.BrothelTraitsPerUpgrade };

            map["invoke." + CfgKeys.BrothelTraitsBase] = () => WithSetupN(FamilyMods.Brothel,
                traitKeys, new object[] { 5, 1 }, CfgKeys.BrothelTraitsBase, key =>
                {
                    // Clamp(5 + 1*0, Min(5,8), 8) = 5(基座;原版 3)
                    int r = BrothelFunc<Func<int, int>>("TraitCategories", "RequirementCount")(0);
                    return r == 5 ? Pass(key, "RequirementCount(0)=" + r + "(base=5)")
                        : Fail(key, "RequirementCount(0) 예상 5 / 실제 " + r);
                });

            map["invoke." + CfgKeys.BrothelTraitsPerUpgrade] = () => WithSetupN(FamilyMods.Brothel,
                traitKeys, new object[] { 3, 4 }, CfgKeys.BrothelTraitsPerUpgrade, key =>
                {
                    // Clamp(3 + 4*2, 3, 8) = 8(上界 All.Length=8 钳;原版 1 → 5)
                    int r = BrothelFunc<Func<int, int>>("TraitCategories", "RequirementCount")(2);
                    return r == 8 ? Pass(key, "RequirementCount(2)=" + r + "(per=4, 상한 8로 제한)")
                        : Fail(key, "RequirementCount(2) 예상 8 / 실제 " + r);
                });

            string[] visitKeys = { CfgKeys.BrothelVisitMinDays, CfgKeys.BrothelVisitMaxDays };

            map["invoke." + CfgKeys.BrothelVisitMinDays] = () => WithSetupN(FamilyMods.Brothel,
                visitKeys, new object[] { 6, 9 }, CfgKeys.BrothelVisitMinDays, key =>
                {
                    // det 取下界:10 + det(6, 10) = 16(min=6;原版掷骰 4..9)
                    int lo = -1, hi = -1;
                    Func<int, int, int> det = (a, b) => { lo = a; hi = b; return a; };
                    int r = BrothelFunc<Func<int, Func<int, int, int>, int>>("MerchantSchedule", "NextVisitDay")(10, det);
                    return r == 16 && lo == 6
                        ? Pass(key, "NextVisitDay(10,det)=" + r + "(min=6, 무작위 추첨 범위 [" + lo + "," + hi + "))")
                        : Fail(key, "min=6:NextVisitDay=" + r + " 무작위 추첨 범위 [" + lo + "," + hi + ")");
                });

            map["invoke." + CfgKeys.BrothelVisitMaxDays] = () => WithSetupN(FamilyMods.Brothel,
                visitKeys, new object[] { 4, 7 }, CfgKeys.BrothelVisitMaxDays, key =>
                {
                    // det 取下界:10 + det(4, 8) = 14;捕获上界 8 = max+1(排除上界还原)
                    int lo = -1, hi = -1;
                    Func<int, int, int> det = (a, b) => { lo = a; hi = b; return a; };
                    int r = BrothelFunc<Func<int, Func<int, int, int>, int>>("MerchantSchedule", "NextVisitDay")(10, det);
                    return r == 14 && hi == 8
                        ? Pass(key, "NextVisitDay(10,det)=" + r + "(max=7 → 추첨 범위 상한 " + hi + ")")
                        : Fail(key, "max=7:NextVisitDay=" + r + " 추첨 범위 상한 실제 " + hi);
                });

            string[] stockKeys = { CfgKeys.BrothelStockFirst, CfgKeys.BrothelStockMin, CfgKeys.BrothelStockMax };

            map["invoke." + CfgKeys.BrothelStockFirst] = () => WithSetupN(FamilyMods.Brothel,
                stockKeys, new object[] { 5, 3, 5 }, CfgKeys.BrothelStockFirst, key =>
                {
                    // 首访分支:按目录序取前 5 件(first=5;原版固定 3 件)
                    IReadOnlyList<string> r = BrothelFunc<Func<int, Func<int, int, int>, IReadOnlyList<string>>>(
                        "GearCatalog", "RollStock")(1, DetLow);
                    var all = BrothelAllGearIds();
                    return r != null && r.Count == 5 && ListStartsWith(r, all)
                        ? Pass(key, "RollStock(1)=목록에서 앞의 " + r.Count + " 개(first=5)")
                        : Fail(key, "RollStock(1) 예상 5개 / 실제 " + (r == null ? -1 : r.Count));
                });

            map["invoke." + CfgKeys.BrothelStockMin] = () => WithSetupN(FamilyMods.Brothel,
                stockKeys, new object[] { 3, 4, 5 }, CfgKeys.BrothelStockMin, key =>
                {
                    // det 取下界:num = det(4, 6) = 4;循环 index = det(0, n) = 0
                    // → 无放回抽取恒取剩余首件 = 目录序前 4 件(min=4)
                    IReadOnlyList<string> r = BrothelFunc<Func<int, Func<int, int, int>, IReadOnlyList<string>>>(
                        "GearCatalog", "RollStock")(2, DetLow);
                    var all = BrothelAllGearIds();
                    return r != null && r.Count == 4 && ListStartsWith(r, all)
                        ? Pass(key, "RollStock(2)=목록에서 앞의 " + r.Count + " 개(min=4)")
                        : Fail(key, "RollStock(2) 예상 4개 / 실제 " + (r == null ? -1 : r.Count));
                });

            map["invoke." + CfgKeys.BrothelStockMax] = () => WithSetupN(FamilyMods.Brothel,
                stockKeys, new object[] { 3, 3, 6 }, CfgKeys.BrothelStockMax, key =>
                {
                    // R14:det 只记首调(件数掷骰 det(min, max+1) = det(3, 7),
                    // 返回下界 → num=3、循环 index=det(0,剩余池长) 恒 0 取剩余首件)。
                    // 旧实现「每次调用都记录」捕获的是最后一次掷骰——复刻体循环逐件
                    // 掷下标 det(0, 6/5/4)(池 = brothel AllItemIds 恒 6 件,
                    // 0.10.4 基线 GearCatalog.Definitions 六键静态表,与主 mod 版本
                    // 无关),末次 det(0,4) 覆盖 hi=4 → 0x12 首跑即 FAIL(0x0f-0x11
                    // brothel 缺席全组 SKIP,从未真跑过,非数值漂移,纯断言脆弱)。
                    bool seen = false;
                    int lo = -1, hi = -1;
                    Func<int, int, int> det = (a, b) =>
                    {
                        if (seen) return a;
                        lo = a; hi = b; seen = true;
                        return a;
                    };
                    var r = BrothelFunc<Func<int, Func<int, int, int>, IReadOnlyList<string>>>(
                        "GearCatalog", "RollStock")(2, det);
                    return r != null && r.Count == 3 && lo == 3 && hi == 7
                        ? Pass(key, "RollStock(2)=" + r.Count + " 개(개수 추첨 범위 [" + lo + "," + hi
                            + ") = max+1로 설정해 상한 제외 규칙 복원)")
                        : Fail(key, "max=6: 개수 " + (r == null ? -1 : r.Count)
                            + " 개수 추첨 범위 실제 [" + lo + "," + hi + ")");
                });

            // ---- Tier2b:SpawnItem(目录工厂生成 → 断言 → 清理)----

            // vanilla(= 让位回退值):1.6.1 翻转后 remake 侧仅 ovulation 让位
            // (Router.YieldOwners R1;milk 改 Brood 独有——双装时 vanilla 到达,
            // 单 remake 装时 EffectMode 走改值断言);price.semen/tentacle_box/
            // printer×2 恒 Own(勘察反界),vanilla 仅为防未来让位兜底(= 主 mod 工厂原价)
            SpawnUnitValue(map, CfgKeys.PriceMilk, "wondrous_milk", 255, 180);       // ItemFactory.cs:148
            SpawnUnitValue(map, CfgKeys.PriceSemen, "tentacle_semen", 123, 90);      // ItemFactory.cs:208
            SpawnUnitValue(map, CfgKeys.PriceTentacleBox, "tentacle_box", 234, 200); // ItemFactory.cs:274
            SpawnUnitValue(map, CfgKeys.PriceOvulationShot, "ovulation_shot", 150, 100); // remake 覆写(ItemFactory.cs:576)
            SpawnUnitValue(map, CfgKeys.AlarmPrice, "alarm_clock", 99, 75);          // AlarmClock.cs:29
            SpawnUnitValue(map, CfgKeys.AlarmTentaclePrice, "tentacle_alarm", 188, 150); // AlarmClock.cs:88
            SpawnUnitValue(map, CfgKeys.TrapPrice, "manhole_cover", 111, 100);       // TentacleTrap.cs:35
            SpawnUnitValue(map, CfgKeys.TrapTrapPrice, "tentacle_trap", 222, 200);   // TentacleTrap.cs:69
            SpawnUnitValue(map, CfgKeys.PrinterPrice, "tentacle_printer", 555, 500);     // Task 14b;TentaclePrinter.cs:108
            SpawnUnitValue(map, CfgKeys.PrinterNeedlePrice, "empty_syringe", 25, 15);   // Task 14b;TentaclePrinter.cs:35

            map["spawn." + CfgKeys.CorruptInitialCharge] = () =>
            {
                // 充能断言值受 max_charge 钳制(Min(initial,max)),期望值按当前
                // max 现算,不依赖玩家对 max_charge 的既有设置
                TestResult Body(string key)
                {
                    int maxNow = HookRuntime.Registry.Current<int>(CfgKeys.CorruptMaxCharge);
                    int expected = Math.Min(50, maxNow);
                    var getInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(
                        MainType("ItemFactory"), "GetInt");
                    return SpawnAndAssert("corrupt_tentacle", key, 50,
                        it => (long)getInt(it, "TENTACLE_CHARGE"),
                        "TENTACLE_CHARGE 예상 " + expected + "(Min(50, max=" + maxNow + "))",
                        expected);
                }
                return WithSpawnSetup(FamilyMods.MainMod, CfgKeys.CorruptInitialCharge, 50, Body);
            };

            // R10:IPH 属性五键(maiden_lower 生成 → MAIDEN_FORCE/INTEL/CHARM 三
            // tag 的区间含入断言;五键共用一生成路径,每键一行 = 目录值键行契约)
            SpawnIphAttr(map, CfgKeys.IphAttrHighChance);
            SpawnIphAttr(map, CfgKeys.IphAttrMidChance);
            SpawnIphAttr(map, CfgKeys.IphAttrHighRange);
            SpawnIphAttr(map, CfgKeys.IphAttrMidRange);
            SpawnIphAttr(map, CfgKeys.IphAttrLowRange);

            // 0.7.0 T11:initial_virgin 生成面直测(键值 false = postfix 接管态,
            // TurnOn 反向拨 false 经 SwitchDispatch T9 条装配——与玩家面板
            // 「关闭开关」同一条链;断言双 tag 移除 = 键值语义的正向证据)
            map["spawn." + CfgKeys.InitialVirgin] = () => WithSpawnSetup(
                FamilyMods.MainMod, CfgKeys.InitialVirgin, false, key =>
                {
                    GameItem item = ModHelper.SpawnItem("maiden_lower");
                    if (item == null) return Fail(key, "아이템 목록에 maiden_lower가 등록되지 않음(저장 파일에서 실행 필요)");
                    try
                    {
                        bool virgin = item.IsTag("MAIDEN_VIRGIN");
                        bool anal = item.IsTag("MAIDEN_ANAL_VIRGIN");
                        return !virgin && !anal
                            ? Pass(key, "initial_virgin=false → MAIDEN_VIRGIN/ANAL_VIRGIN 태그 2개 제거됨(생성할 때부터 처녀 아님)")
                            : Fail(key, "MAIDEN_VIRGIN=" + virgin + " MAIDEN_ANAL_VIRGIN=" + anal
                                + "(postfix가 적용되지 않았나? 1.9.2 원본은 두 태그를 무조건 활성화함)");
                    }
                    finally
                    {
                        ModHelper.MoveItemToTrash(item);
                    }
                });

            // ---- Tier2c:WriteThrough(镜像键往返;按 FamilyPanelKeys 写透条目注册,
                //      ST4 后 36 键 = remake 8[7−3+4 桶装] + Brood 12 + IPH 16)----

            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            {
                if (e.Carrier != CarrierKind.WriteThrough) continue;
                FamilyPanelEntry captured = e;
                map["wt." + e.OurKey] = () => WriteThroughRoundtrip(captured);
            }

            // ---- Tier2d:LegacyOverlay(R13b:legacy 1.0.6 ContentFactoryPostfixes
            //      五工厂 postfix 文本叠加 × 我方键的共存验证,7 行;
            //      ExecutorId = 伪键本身,已带 legacy. 前缀)----

            map["legacy.milk"] = () => LegacyOverlayPrice("legacy.milk",
                CfgKeys.PriceMilk, 255, 180, "wondrous_milk");
            map["legacy.semen"] = () => LegacyOverlayPrice("legacy.semen",
                CfgKeys.PriceSemen, 123, 90, "tentacle_semen");
            map["legacy.ovulation"] = () => LegacyOverlayPrice("legacy.ovulation",
                CfgKeys.PriceOvulationShot, 150, 100, "ovulation_shot");
            map["legacy.tentacle_body"] = LegacyTentacleBodyOverlay;
            map["legacy.maiden_pools"] = LegacyMaidenPoolsOverlay;
            map["legacy.maiden_iph"] = () => IphAttrSpawn("legacy.maiden_iph", true);
            map["legacy.maiden_overlay"] = LegacyMaidenOverlayEvidence;

            // ---- Tier3:ReadPath(兜底;按目录 ReadPath 行注册)----

            foreach (KeyTest t in SelfTestPlan.KeyTests)
            {
                if (t.Mechanism != TestMechanism.ReadPath) continue;
                string key = t.Key;
                map["read." + key] = () => ReadPathOne(key);
            }
            return map;
        }

        // ------------------------------------------------------------------
        // Tier1 路由审计
        // ------------------------------------------------------------------

        private static TestResult RouteAuditOne(HookUnitDef def)
        {
            if (!Ready) return Skip(def.Id, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            string expected = ExpectedState(def), actual = ActualState(def);
            return expected == actual
                ? Pass(def.Id, expected)
                : Fail(def.Id, "예상 " + expected + " / 실제 " + actual);
        }

        // ------------------------------------------------------------------
        // Tier2 共用脚手架(拨开关 → 设值 → body → finally 复原)
        // R6-F2 让位感知:生成期望前先查该键所属单元的路由快照
        // (EffectDispatch.Decide)——Own → 改值断言(现状)/ 全 Yield →
        // 原版字面量断言(让位回退是正确行为,正向化)/ Dead·VersionBlocked
        // → SKIP(暴露面在 Tier1 路由审计,值直测不双重记账)。
        // ------------------------------------------------------------------

        /// <summary>键的值生效模式:所属 MainHook/SubModHook 单元的路由快照
        /// (route × Dead)经 EffectDispatch 纯函数分派(Logic/,单测锁定)。</summary>
        private static EffectExpect EffectMode(string key, out string note)
        {
            var routes = new List<EffectDispatch.UnitRoute>();
            foreach (HookUnitDef def in HookCatalog.Units)
            {
                if (def.Keys == null || Array.IndexOf(def.Keys, key) < 0) continue;
                if (def.Kind != CarrierKind.MainHook && def.Kind != CarrierKind.SubModHook) continue;
                bool dead = HookRuntime.Installer != null
                    && HookRuntime.Installer.States.TryGetValue(def.Id, out HookState st)
                    && st == HookState.Dead;
                routes.Add(new EffectDispatch.UnitRoute(def.Id, HookRuntime.RouteSnapshot[def.Id], dead));
            }
            return EffectDispatch.Decide(routes.ToArray(), out note);
        }

        /// <summary>键的让位 owner 显示名(Router.YieldOwners 展平,固定序 /
        /// 连接,与 YieldNote 文案同源;无让位单元返回 null)。</summary>
        private static string YieldOwnerText(string key)
        {
            var names = new List<string>();
            foreach (HookUnitDef def in HookCatalog.Units)
            {
                if (def.Keys == null || Array.IndexOf(def.Keys, key) < 0) continue;
                if (!Router.YieldOwners.TryGetValue(def.Id, out string[] owners)) continue;
                foreach (string o in owners)
                    if (!names.Contains(Router.OwnerDisplay(o))) names.Add(Router.OwnerDisplay(o));
            }
            return names.Count == 0 ? null : string.Join("/", names);
        }

        /// <summary>InvokeStatic 执行脚手架:在场+版本判定 → 拨开关(热对账装上)
        /// → 设测试值 → body 断言 → finally 复原值与开关。yieldBody:让位时的
        /// 替代断言(null = 让位中 Skip——静态直调的「原版回退」无面板期望结构,
        /// 如 GetBodyLevel 的原值无确定性;恒 Own 的键不受影响)。</summary>
        private static TestResult WithSetup(string modId, string key, object testValue,
            Func<string, TestResult> body, Func<string, TestResult> yieldBody = null)
        {
            if (!Ready) return Skip(key, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            if (!ModUsable(modId)) return Skip(key, modId + " 대상이 없거나 버전이 일치하지 않음");
            EffectExpect mode = EffectMode(key, out string note);
            if (mode == EffectExpect.Skip) return Skip(key, note);
            if (mode == EffectExpect.Yield && yieldBody == null)
                return Skip(key, "적용을 양보함(" + YieldOwnerText(key) + "에서 처리). 정적 직접 검사에서 확정된 원본 예상값이 없음");
            object original = CurrentBoxed(key);
            Action restoreSwitch = TurnOn(key);
            try
            {
                SetValueBoxed(key, testValue);
                return mode == EffectExpect.Yield ? yieldBody(key) : body(key);
            }
            catch (Exception ex)
            {
                return Fail(key, "직접 호출 중 예외(메서드 시그니처가 변경됐나?): " + ex.Message);
            }
            finally
            {
                SetValueBoxed(key, original);
                restoreSwitch();
            }
        }

        /// <summary>SpawnItem 执行脚手架(同 WithSetup;body 内生成+断言,清理在
        /// SpawnItem 兜底)。yieldBody:让位时的原版字面量断言(SpawnUnitValue
        /// 注册处生成;null = 让位中 Skip)。</summary>
        private static TestResult WithSpawnSetup(string modId, string key, object testValue,
            Func<string, TestResult> body, Func<string, TestResult> yieldBody = null)
        {
            if (!Ready) return Skip(key, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            if (!ModUsable(modId)) return Skip(key, modId + " 대상이 없거나 버전이 일치하지 않음");
            EffectExpect mode = EffectMode(key, out string note);
            if (mode == EffectExpect.Skip) return Skip(key, note);
            if (mode == EffectExpect.Yield && yieldBody == null)
                return Skip(key, "적용을 양보함(" + YieldOwnerText(key) + "에서 처리). 이 키는 원본 검증 대상으로 설정되지 않음");
            object original = CurrentBoxed(key);
            Action restoreSwitch = TurnOn(key);
            try
            {
                SetValueBoxed(key, testValue);
                return mode == EffectExpect.Yield ? yieldBody(key) : body(key);
            }
            catch (Exception ex)
            {
                return Fail(key, "생성 중 예외(저장 파일에서 실행 필요): " + ex.Message);
            }
            finally
            {
                SetValueBoxed(key, original);
                restoreSwitch();
            }
        }

        /// <summary>
        /// 多键 InvokeStatic 脚手架(R13b:brothel 组合键单元——prefix 消费多键且
        /// 「任一非默认即接管」,单测主测键时其余键若保持玩家既有非默认值会污染
        /// 期望,故逐键全套设值[含非主测键的确定性测试值],期望由测试值唯一决定;
        /// 非主测键一律取原版默认字面量 = 与前置断言口径一致)。流程:IphAttrSpawn
        /// 先例——先记录全部原值/开关,逐一拨开关(热对账装上)再设值,body 断言,
        /// finally 先值后开关逐一复原(等于默认时玩家层自动移除 = 回到未干预态)。
        /// </summary>
        private static TestResult WithSetupN(string modId, string[] keys, object[] testValues,
            string mainKey, Func<string, TestResult> body)
        {
            if (!Ready) return Skip(mainKey, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            if (!ModUsable(modId)) return Skip(mainKey, modId + " 대상이 없거나 버전이 일치하지 않음");
            foreach (string k in keys)
            {
                EffectExpect mode = EffectMode(k, out string note);
                if (mode != EffectExpect.Own)
                    return Skip(mainKey, k + " " + (note ?? "기능 양보/적용 차단 상태라 복합 키의 예상값을 확정할 수 없음"));
            }
            var originals = new object[keys.Length];
            var restores = new Action[keys.Length];
            try
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    originals[i] = CurrentBoxed(keys[i]);
                    restores[i] = TurnOn(keys[i]);
                    SetValueBoxed(keys[i], testValues[i]);
                }
                return body(mainKey);
            }
            catch (Exception ex)
            {
                return Fail(mainKey, "직접 호출 중 예외(메서드 시그니처가 변경됐나?): " + ex.Message);
            }
            finally
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    SetValueBoxed(keys[i], originals[i]);
                }
                for (int i = 0; i < keys.Length; i++)
                {
                    restores[i]?.Invoke();
                }
            }
        }

        /// <summary>生成物品 → 断言 → 清理(MoveItemToTrash = 丢弃,不留永久物品)。
        /// diagUnitValue(R15 临时诊断,复跑取数后移除):断言读数后间隔一次
        /// DirectoryMaster.Item("empty_syringe") 对照生成,再读目标 unitValue——
        /// 定位「生成后被下一次目录调用覆盖」(v1 正确而 v2 变 200 = 高度魔幻的
        /// 后置覆盖;v1 直接 200 = postfix 没跑或没写,对照行看 price postfix 探针)。
        /// 诊断块自带 try/catch(异常只记日志),断言行为零变化。</summary>
        private static TestResult SpawnAndAssert(string itemId, string key, object testValue,
            Func<GameItem, long> read, string expectNote, long expected, bool diagUnitValue = false)
        {
            GameItem item = ModHelper.SpawnItem(itemId);
            if (item == null) return Fail(key, "아이템 목록에 등록되지 않음: " + itemId);
            try
            {
                long actual = read(item);
                if (diagUnitValue)
                {
                    try
                    {
                        GameItem control = DirectoryMaster.Item("empty_syringe");
                        long after = item.unitValue;
                        long controlValue = control == null ? -1L : control.unitValue;
                        UnityEngine.Debug.Log("[lewd_meme_config][diag] spawn " + itemId + " first=" + actual
                            + " 비교 후=" + after + " 비교 대상 아이템 empty_syringe=" + controlValue);
                        if (control != null) ModHelper.MoveItemToTrash(control);
                    }
                    catch (Exception ex)
                    {
                        UnityEngine.Debug.Log("[lewd_meme_config][diag] spawn " + itemId
                            + " 비교 생성 중 예외: " + ex.Message);
                    }
                }
                return actual == expected ? Pass(key, itemId + " " + actual + "(" + expectNote + ")")
                    : Fail(key, itemId + " " + expectNote + ", 실제 " + actual);
            }
            finally
            {
                ModHelper.MoveItemToTrash(item);
            }
        }

        /// <summary>unitValue 断言的 SpawnItem 注册(price.* / alarm.* / trap.*;
        /// R6-F2 双期望:vanillaValue = 让位回退的字面量——除 ovulation_shot 外
        /// 均等于面板默认值(= 主 mod 工厂原价),ovulation_shot 让位时 remake 的
        /// ConfigureOvulationShot postfix 强制 100(remake_1519 ItemFactory.cs:576,
        /// 主 mod 工厂原价 120),故显式传值不读默认)。</summary>
        private static void SpawnUnitValue(Dictionary<string, Func<TestResult>> map,
            string key, string itemId, int testValue, int vanillaValue)
        {
            map["spawn." + key] = () => WithSpawnSetup(FamilyMods.MainMod, key, testValue,
                k => SpawnAndAssert(itemId, k, testValue, it => it.unitValue,
                    "unitValue 예상 " + testValue, testValue, diagUnitValue: true),
                k => SpawnAndAssert(itemId, k, vanillaValue, it => it.unitValue,
                    "적용을 양보함(" + YieldOwnerText(k) + "에서 처리). 원본값 복귀 검증 " + vanillaValue,
                    vanillaValue, diagUnitValue: true));
        }

        /// <summary>
        /// R10:IPH 属性键的 SpawnItem 执行器注册(五键共用 IphAttrSpawn 一条
        /// 生成路径,每键一行 = 目录值键行契约的主测键各异)。
        /// </summary>
        private static void SpawnIphAttr(Dictionary<string, Func<TestResult>> map, string key)
        {
            map["spawn." + key] = () => IphAttrSpawn(key);
        }

        /// <summary>
        /// R10 断言设计(低波动,用户裁定「RNG 断言只做区间边界含入检查」):
        /// 测试值把三档区间整体设到原版 [10,80] 之外的 [120,125]/[110,112]/
        /// [100,105](概率 0.5/0.9 同步设非默认)——postfix 生效时三属性 tag
        /// 必落三档并集内(掷哪一档随机,断言不依赖具体档位/具体值);未装上
        /// (装配失败/让位/压制)则原版掷骰落 [10,80],与并集零重叠 → 必检出,
        /// 检出率 100%。F2(复审):五键各配独立子项开关,PlayerLayerAllowed
        /// 只放行开关已开的键——只开主测键的 .enabled 会让其余四键玩家层被压
        /// 回默认,配置整体未生效、断言必挂;故拨开阶段<b>五开关齐开</b>,
        /// finally 逐一恢复原态(值与开关,等于默认时玩家层自动移除 = 回到
        /// 未干预态,零行为漂移红线;复原序 = 先值后开关,WithSetup 先例)。
        /// R13b:legacyOverlay = true 时为 legacy.maiden_iph 行——前置追加
        /// legacy 可用 + 内容目录已加载(CreateMaiden postfix 叠加前提),Pass
        /// 前追加文本叠加断言(name/shortDescription 非空)。
        /// </summary>
        private static TestResult IphAttrSpawn(string key, bool legacyOverlay = false)
        {
            if (!Ready) return Skip(key, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            if (!ModUsable(FamilyMods.MainMod)) return Skip(key, "lewd_meme 모드가 없거나 버전이 일치하지 않음");
            if (!ModUsable(FamilyMods.Iph)) return Skip(key, "item_portrait_hd 모드가 없거나 버전이 일치하지 않음");
            if (legacyOverlay)
            {
                if (!ModUsable(FamilyMods.Legacy)) return Skip(key, "lewd_meme_legacy_content 모드가 없거나 버전이 일치하지 않음");
                if (!LegacyContentLoaded()) return Skip(key, "legacy 콘텐츠 목록을 불러오지 못함(추가 적용 기능이 활성화되지 않음)");
            }
            // legacy 变体:key 是伪键(无所属单元,EffectMode 恒 Own),换五键之一
            // 检查 main.maiden_attrs_iph 的 Dead/压制态(与属性组行同口径)
            EffectExpect mode = EffectMode(legacyOverlay ? CfgKeys.IphAttrHighChance : key, out string note);
            if (mode != EffectExpect.Own) return Skip(key, note ?? "다른 모드에 양보했거나 적용이 차단됨");
            ConfigRegistry reg = HookRuntime.Registry;
            string[] keys =
            {
                CfgKeys.IphAttrHighChance, CfgKeys.IphAttrMidChance,
                CfgKeys.IphAttrHighRange, CfgKeys.IphAttrMidRange, CfgKeys.IphAttrLowRange,
            };
            var originals = new object[keys.Length];
            var switches = new string[keys.Length];
            var switchWas = new bool[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                originals[i] = CurrentBoxed(keys[i]);
                switches[i] = PanelKeys.ItemSwitchKey(keys[i]);
                switchWas[i] = reg.Current<bool>(switches[i]);
            }
            try
            {
                reg.SetPlayerValue(CfgKeys.IphAttrHighChance, 0.5f);
                reg.SetPlayerValue(CfgKeys.IphAttrMidChance, 0.9f);
                reg.SetPlayerValue(CfgKeys.IphAttrHighRange, "120,125");
                reg.SetPlayerValue(CfgKeys.IphAttrMidRange, "110,112");
                reg.SetPlayerValue(CfgKeys.IphAttrLowRange, "100,105");
                for (int i = 0; i < keys.Length; i++)
                    reg.SetPlayerValue(switches[i], true);   // F2:五开关齐开 → 热对账装 postfix
                var getInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(
                    MainType("ItemFactory"), "GetInt");
                GameItem item = ModHelper.SpawnItem("maiden_lower");
                if (item == null) return Fail(key, "아이템 목록에 maiden_lower가 등록되지 않음(저장 파일에서 실행 필요)");
                try
                {
                    int force = getInt(item, "MAIDEN_FORCE");
                    int intel = getInt(item, "MAIDEN_INTEL");
                    int charm = getInt(item, "MAIDEN_CHARM");
                    bool ok = InIphTestRanges(force) && InIphTestRanges(intel) && InIphTestRanges(charm);
                    string detail = "능력치 태그 3개 " + force + "/" + intel + "/" + charm
                        + (ok ? " 모두 설정한 세 구간의 합집합에 포함됨" : " 설정한 세 구간의 합집합 밖임(다시 추첨되지 않았나?)")
                        + " [100,105]/[110,112]/[120,125](변동이 적은 검증: 구간에 포함되는지만 확인,"
                        + "어느 구간을 뽑았는지는 확인하지 않음. 원본 구간 [10,80]은 합집합 밖이므로 패치가 누락되면 감지됨)";
                    // F2-R2 差量防掩蔽断言(游戏侧拿不到差量前值——postfix 在工厂
                    // 返回前跑,SpawnItem 返回时已是终值;经 MaidenAttrIphPatch 的
                    // 观察口记账断言,读点掩蔽必然表现为 Δ≡0 被第三条钉死[R3:
                    // 判据 = tag 落并集(rerolled,观察口外部事实),非观察口 Old*])
                    string deltaNote;
                    bool deltaOk = IphDeltaCheck(item.unitValue, force, intel, charm, out deltaNote);
                    if (!deltaOk) return Fail(key, detail + "; 차이값 검사 실패: " + deltaNote);
                    TestResult result = ok
                        ? Pass(key, detail + ";" + deltaNote)
                        : Fail(key, detail);
                    // R13b legacy 变体:属性重掷与 legacy CreateMaiden postfix 文本
                    // 叠加共存(生成链 = 原版体 + IPH transpiler + 我方 postfix +
                    // legacy postfix,任一环破坏文本即 FAIL)
                    if (legacyOverlay && result.Pass && !SelfTestPlan.IsSkip(result.Detail))
                        result = ItemTextOk(key, item, "능력치 재추첨과 텍스트 추가 기능이 함께 적용됨");
                    return result;
                }
                finally
                {
                    ModHelper.MoveItemToTrash(item);
                }
            }
            catch (Exception ex)
            {
                return Fail(key, "생성 중 예외(저장 파일에서 실행 필요): " + ex.Message);
            }
            finally
            {
                for (int i = 0; i < keys.Length; i++) SetValueBoxed(keys[i], originals[i]);
                for (int i = 0; i < keys.Length; i++) reg.SetPlayerValue(switches[i], switchWas[i]);
            }
        }

        /// <summary>R10 测试值三档并集:[100,105] ∪ [110,112] ∪ [120,125]。</summary>
        private static bool InIphTestRanges(int v)
        {
            return (v >= 100 && v <= 105) || (v >= 110 && v <= 112) || (v >= 120 && v <= 125);
        }

        /// <summary>
        /// F2-R2 差量防掩蔽检查(观察口 = MaidenAttrIphPatch.LastRerollTrace):
        /// ① unitValue == OldUnitValue + Δ(差量已正确应用);② tag 与观察口
        /// 记录一致(防观察口污染);③ 重掷已发生(tag 落测试区间并集)而 Δ=0
        /// → FAIL。③ 的判据是 R3 破除循环豁免:R2 版用 !same(取自观察口自身
        /// 的 Old*==New*),而读点回归(SetInt 后读 old)恰把 Old* 抹成 New* ⇒
        /// same≡true ⇒ 防线对自己要防的回归失明——观察口不可自证,改用
        /// rerolled = tag 落并集(观察口外部的事实)。零假阳性是结构保证而非
        /// 概率的:rerolled ⇒ new ∈ [100,125] 系,old ∈ 原版 [10,80] 系,
        /// 值域零重叠 ⇒ new≠old 恒真,AttrPrice 分段斜率恒正(严格递增)⇒
        /// Δ≠0 必然——Δ=0 且 rerolled 只能是读点掩蔽或 AttrDelta 公式回归
        /// (公式由纯逻辑侧 MaidenRollTests 的 AttrDelta 恒等性测试锁定)。
        /// 观察口为 null(remake 在场差量跳过 / 本单元未参与差量)= 设计态,
        /// 跳过并注明。
        /// </summary>
        private static bool IphDeltaCheck(long unitValue, int force, int intel, int charm, out string note)
        {
            MaidenAttrIphPatch.AttrRerollTrace? t = MaidenAttrIphPatch.LastRerollTrace;
            if (t == null)
            {
                note = "차이 기록 없음(암거래가 있을 때 건너뛰는 것이 정상: 가격은 암거래 공식에 따르며, 태그 검증으로 재추첨 자체는 이미 확인함)";
                return true;
            }
            MaidenAttrIphPatch.AttrRerollTrace tr = t.Value;
            if (unitValue != tr.OldUnitValue + tr.Delta)
            {
                note = "unitValue=" + unitValue + " ≠ 관측된 OldUnitValue+Δ = " + (tr.OldUnitValue + tr.Delta)
                    + "(차이값이 올바르게 반영되지 않음)";
                return false;
            }
            bool tagMatchesTrace = tr.NewForce == force && tr.NewIntel == intel && tr.NewCharm == charm;
            if (!tagMatchesTrace)
            {
                note = "tag(" + force + "/" + intel + "/" + charm + ")와 관측 기록("
                    + tr.NewForce + "/" + tr.NewIntel + "/" + tr.NewCharm + ")이 일치하지 않음(관측 기록이 오염됐나?)";
                return false;
            }
            // ③ 重掷已发生的独立证据:tag 落测试区间并集(与原版 [10,80] 零重叠)。
            //    R3:!same(观察口 Old*==New*,可被读点回归抹平 = 观察口自证)→
            //    rerolled(tag 并集,观察口外部事实,不可被抹平)。
            bool rerolled = InIphTestRanges(force) && InIphTestRanges(intel) && InIphTestRanges(charm);
            if (tr.Delta == 0 && rerolled)
            {
                note = "능력치를 다시 추첨했지만(태그가 구간에 포함됨) Δ=0임 — 값 조회 지점이 가려졌거나 AttrDelta 계산식에 문제가 생김"
                    + "(값 구간이 겹치지 않으므로 new≠old가 보장되고 Δ=0일 수 없음)";
                return false;
            }
            note = "차이값 Δ=" + tr.Delta + " 일치하게 적용됨";
            return true;
        }

        /// <summary>brothel 服饰价(ItemValue 纯静态直调,经 SpawnItem 免——无对应
        /// 目录物品,ItemValue 即唯一断言面)。</summary>
        private static void SpawnPrice(Dictionary<string, Func<TestResult>> map,
            string key, string gearId, int testValue)
        {
            map["invoke." + key] = () => WithSetup(FamilyMods.Brothel, key, testValue, k =>
            {
                long r = BrothelFunc<Func<string, long>>("BrothelItemCatalog", "ItemValue")(gearId);
                return r == testValue ? Pass(k, "ItemValue(" + gearId + ")=" + r)
                    : Fail(k, "ItemValue 예상 " + testValue + " 실제 " + r);
            });
        }

        // ------------------------------------------------------------------
        // Tier2d LegacyOverlay(R13b:legacy 1.0.6 ContentFactoryPostfixes 的
        // CreateMaiden/CreateMilk/CreateSemen/CreateOvulationShot/TentacleBody.Create
        // 五工厂 postfix 文本叠加 × 我方生成接管/价格键的共存验证)
        // ------------------------------------------------------------------

        /// <summary>确定性掷骰委托:恒取下界(RollStock/NextVisitDay 的委托参数
        /// 注入确定性——randomRange(min, max+1) = min,抽取 index = det(0, n) = 0
        /// → 无放回抽取恒取剩余首件 = 目录序前缀,输出唯一)。</summary>
        private static readonly Func<int, int, int> DetLow = (a, b) => a;

        /// <summary>legacy 的 GearCatalog.AllItemIds 活引用(目录序 6 件;
        /// 与 BrothelPatches.EnsureResolved 同一 ModReflect 通道,执行期现取)。</summary>
        private static IReadOnlyList<string> BrothelAllGearIds()
        {
            return ModReflect.GetStatic<IReadOnlyList<string>>(BrothelType("GearCatalog"), "AllItemIds");
        }

        /// <summary>head 是否为 all 的等值前缀(长度即 head.Length)。</summary>
        private static bool ListStartsWith(IReadOnlyList<string> head, IReadOnlyList<string> all)
        {
            if (all == null || head.Count > all.Count) return false;
            for (int i = 0; i < head.Count; i++)
                if (head[i] != all[i]) return false;
            return true;
        }

        /// <summary>主 mod ItemFactory.GetStr(生成物的字符串 tag 读取;执行期现解析)。</summary>
        private static Func<GameItem, string, string> MainGetStr()
        {
            return ModReflect.StaticMethod<Func<GameItem, string, string>>(
                MainType("ItemFactory"), "GetStr");
        }

        /// <summary>legacy 内容目录已加载(LegacyContentCatalog.Current != null;
        /// 为 null 时其 ContentFactoryPostfixes 全跳过,叠加面不存在 = 设计态 SKIP)。
        /// 类型经 ModReflect.Types(legacy) 按名解析(前置已声明,访问合法)。</summary>
        private static bool LegacyContentLoaded()
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Legacy))
                if (t.Name == "LegacyContentCatalog")
                    return ModReflect.GetStatic<object>(t, "Current") != null;
            return false;
        }

        /// <summary>LegacyOverlay 行共用守卫:装配就绪 + 主 mod/legacy 可用(在场且
        /// 版本绑定)+ legacy 内容目录已加载;任一不满足 → SKIP(设计态);null = 通过。
        /// 内容目录未加载时 legacy 的工厂 postfix 全跳过,「叠加共存」无从谈起。</summary>
        private static TestResult LegacyGuard(string key)
        {
            if (!Ready) return Skip(key, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            if (!ModUsable(FamilyMods.MainMod)) return Skip(key, "lewd_meme 모드가 없거나 버전이 일치하지 않음");
            if (!ModUsable(FamilyMods.Legacy)) return Skip(key, "lewd_meme_legacy_content 모드가 없거나 버전이 일치하지 않음");
            if (!LegacyContentLoaded()) return Skip(key, "legacy 콘텐츠 목록을 불러오지 못함(추가 적용 기능이 활성화되지 않음)");
            return null;
        }

        /// <summary>文本叠加断言:name/shortDescription 非空——legacy ApplyText 的
        /// 短描经 NonEmpty(fallback 原值)保底,叠加链完好时必非空;空 = 叠加链
        /// 破坏(任一 postfix 抹掉文本)。flavorText 原版可为空(tentacle_body),
        /// 只记明细不硬断言。</summary>
        private static TestResult ItemTextOk(string key, GameItem item, string note)
        {
            bool nameOk = !string.IsNullOrEmpty(item.name);
            bool descOk = !string.IsNullOrEmpty(item.shortDescription);
            if (!nameOk || !descOk)
                return Fail(key, "텍스트 추가 기능으로 문자열 손상: name=\"" + item.name
                    + "\" shortDescription=\"" + item.shortDescription + "\"");
            string flavor = string.IsNullOrEmpty(item.flavorText) ? "비어 있음" : "비어 있지 않음";
            return Pass(key, "「" + item.name + "」" + note + ";flavorText " + flavor);
        }

        /// <summary>生成 → 我方值断言 + 文本叠加断言(清理走 SpawnItem 契约)。</summary>
        private static TestResult SpawnOverlayAssert(string key, string itemId,
            Func<GameItem, long> read, long expected, string note)
        {
            GameItem item = ModHelper.SpawnItem(itemId);
            if (item == null) return Fail(key, "아이템 목록에 등록되지 않음: " + itemId);
            try
            {
                long actual = read(item);
                if (actual != expected)
                    return Fail(key, itemId + " " + note + ", 실제 " + actual);
                return ItemTextOk(key, item, note);
            }
            finally
            {
                ModHelper.MoveItemToTrash(item);
            }
        }

        /// <summary>价格三行(CreateMilk/CreateSemen/CreateOvulationShot postfix 文本
        /// + 我方价格键 unitValue;WithSpawnSetup 复用——Own 断言测试值,Yield
        /// 断言让位回退字面量,legacy 守卫在前)。</summary>
        private static TestResult LegacyOverlayPrice(string key, string priceKey,
            int testValue, int vanillaValue, string itemId)
        {
            TestResult guard = LegacyGuard(key);
            if (guard != null) return guard;
            return WithSpawnSetup(FamilyMods.MainMod, priceKey, testValue,
                k => SpawnOverlayAssert(k, itemId, it => it.unitValue,
                    testValue, "unitValue 예상 " + testValue + "(" + priceKey + ")"),
                k => SpawnOverlayAssert(k, itemId, it => it.unitValue,
                    vanillaValue, "기능 양보 후 원본 unitValue로 복귀 " + vanillaValue));
        }

        /// <summary>tentacle_body 行(TentacleBody.Create postfix 文本 + main.create
        /// 布局键:tabs_per_level=3 → 储藏页签 = Max(0, bodyLevel×3-1),bodyLevel
        /// 现调主 mod GetBodyLevel 同源计算,不依赖玩家等级)。
        /// R14 根因修正:tabs_per_level 是 9 布局键之一(注册但不上面板,无子项
        /// 开关)——SwitchDispatch 第四条恒 false = main.create 正常玩法永不装配
        /// (设计态,0.6.0 验收 R1 既有语义),TurnOn 对布局键是 no-op,WithSpawnSetup
        /// 的「拨开关 → 热对账」通道对此键不存在(0x12 FAIL 实为单元 Absent 下
        /// 跑原版 Create,Tier1 6739 行 main.create PASS Absent 为证)。本行改走
        /// 期望态 overlay 直拨(HookInstaller.SetDesired——与开关 Changed 分发
        /// 同一对账入口,测的仍是真实装配链):设值 → SetDesired(true) 装上 →
        /// 断言 → finally SetDesired(false) + 值复原(拉回 Absent,零行为漂移)。
        /// 次序关键(先值后拨):值写入触发 Changed → OnSwitchChanged 对非开关键
        /// 回写 SetDesired(key, SwitchOnFor= false),先拨会被冲掉。</summary>
        private static TestResult LegacyTentacleBodyOverlay()
        {
            const string key = "legacy.tentacle_body";
            TestResult guard = LegacyGuard(key);
            if (guard != null) return guard;
            EffectExpect mode = EffectMode(CfgKeys.TabsPerLevel, out string note);
            if (mode != EffectExpect.Own) return Skip(key, note ?? "다른 모드에 양보했거나 적용이 차단됨");
            HookUnitDef createDef = null;
            foreach (HookUnitDef def in HookCatalog.Units)
                if (def.Kind == CarrierKind.MainHook && def.Keys != null
                    && Array.IndexOf(def.Keys, CfgKeys.TabsPerLevel) >= 0)
                { createDef = def; break; }
            if (createDef == null) return Fail(key, "tabs_per_level에 MainHook 소비 단위가 없음(목록 불일치)");
            var inst = HookRuntime.Installer;
            object original = CurrentBoxed(CfgKeys.TabsPerLevel);
            try
            {
                SetValueBoxed(CfgKeys.TabsPerLevel, 3);
                inst.SetDesired(CfgKeys.TabsPerLevel, true); // 期望态 overlay:main.create 热对账装上
                string state = ActualState(createDef);
                if (state != HookState.Installed.ToString())
                    return Fail(key, createDef.Id + " 예상 상태를 직접 변경한 뒤 실제 " + state
                        + "(transpile 일치 실패. Player.log의 Dead 행 확인)");
                int bodyLevel = ModReflect.StaticMethod<Func<int>>(
                    MainType("TentacleBody"), "GetBodyLevel")();
                int expected = Math.Max(0, bodyLevel * 3 - 1);
                var getInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(
                    MainType("ItemFactory"), "GetInt");
                return SpawnOverlayAssert(key, "tentacle_body",
                    it => getInt(it, "STORAGE_EXPANSION_TABS_INT"), expected,
                    "탭 수 = bodyLevel(" + bodyLevel + ")×3-1(tabs_per_level=3)");
            }
            catch (Exception ex)
            {
                return Fail(key, "생성 중 예외(저장 파일에서 실행 필요): " + ex.Message);
            }
            finally
            {
                inst.SetDesired(CfgKeys.TabsPerLevel, false);
                SetValueBoxed(CfgKeys.TabsPerLevel, original);
            }
        }

        /// <summary>maiden_pools 行(CreateMaiden postfix 文本 + maiden.factions 单池
        /// 生效)。IPH 在场时 main.create_maiden/pools.maiden_fbb 双让位,阵营走其
        /// 权重表(玩家出口 = IPH 写透分区)→ SKIP(设计态,非失败)。</summary>
        private static TestResult LegacyMaidenPoolsOverlay()
        {
            const string key = "legacy.maiden_pools";
            TestResult guard = LegacyGuard(key);
            if (guard != null) return guard;
            if (ModPresent(FamilyMods.Iph))
                return Skip(key, "IPH가 설치됨: create_maiden/fbb 후보군을 양보하고 세력은 IPH 가중치표로 결정됨(결과는 IPH에 기록)");
            return WithSpawnSetup(FamilyMods.MainMod, CfgKeys.Factions, "黑市", k =>
                SpawnOverlayAssert(k, "maiden_lower",
                    it => MainGetStr()(it, "MAIDEN_FACTION") == "黑市" ? 1 : 0, 1,
                    "세력 후보군을 '黑市' 하나로 설정한 결과 적용됨(maiden.factions)"));
        }

        /// <summary>maiden_overlay 行(叠加双向证据):我方 maiden.factions 单池生效
        /// (MAIDEN_FACTION == 池值)+ legacy Apply 的叠加面存活——MAIDEN_ADULT tag
        /// (原版生成不打此 tag,legacy 唯一来源,反编译核实)+ MAIDEN_PERSONALITY
        /// 非空(legacy 写入)。我方键不破坏 legacy 叠加、legacy 叠加不破坏我方键,
        /// 双向各一条断言。IPH 在场同 maiden_pools 行 SKIP。</summary>
        private static TestResult LegacyMaidenOverlayEvidence()
        {
            const string key = "legacy.maiden_overlay";
            TestResult guard = LegacyGuard(key);
            if (guard != null) return guard;
            if (ModPresent(FamilyMods.Iph))
                return Skip(key, "IPH가 설치됨: create_maiden/fbb 후보군을 양보하고 세력은 IPH 가중치표로 결정됨(결과는 IPH에 기록)");
            return WithSpawnSetup(FamilyMods.MainMod, CfgKeys.Factions, "黑市", k =>
            {
                GameItem item = ModHelper.SpawnItem("maiden_lower");
                if (item == null) return Fail(k, "아이템 목록에 maiden_lower가 등록되지 않음");
                try
                {
                    var getStr = MainGetStr();
                    string faction = getStr(item, "MAIDEN_FACTION");
                    bool adultOk = item.IsTag("MAIDEN_ADULT");
                    string personality = getStr(item, "MAIDEN_PERSONALITY");
                    if (faction != "黑市")
                        return Fail(k, "MAIDEN_FACTION 예상 黑市 / 실제 " + faction + "(이쪽 후보군 설정이 적용되지 않음)");
                    if (!adultOk || string.IsNullOrEmpty(personality))
                        return Fail(k, "legacy 추가 적용 기능 누락: MAIDEN_ADULT=" + adultOk
                            + " MAIDEN_PERSONALITY=\"" + personality + "\"(해당 postfix가 적용되지 않았나?)");
                    return ItemTextOk(k, item, "이쪽 후보군 '黑市' 적용 및 legacy 추가 적용 기능 유지(ADULT/PERSONALITY)");
                }
                finally
                {
                    ModHelper.MoveItemToTrash(item);
                }
            });
        }

        // ------------------------------------------------------------------
        // Tier2c WriteThrough 往返
        // ------------------------------------------------------------------

        /// <summary>
        /// 写透往返:目标在场才执行。路径 A(我方开关原关):设非默认值 → 拨开
        /// 关(Changed → HookRuntime 分发器 → Carrier.Enable:快照+写对方)→
        /// 断言对方条目 == 我方值 → 拨关(Disable:恢复快照)→ 断言对方 == 开启
        /// 前原值。路径 B(开关原开,会话已有快照):只测值变化重写路径(设新值
        /// → 对方 == 新值 → 复原值 → 对方 == 原值),不拨关——防止测试吞掉玩家
        /// 现有快照锚点。finally 复原我方值与开关原态。注:往返期间对方配置被
        /// 瞬态改写并触发对方 Save(disable 后写回原值),报告明细已注明。
        /// </summary>
        private static TestResult WriteThroughRoundtrip(FamilyPanelEntry e)
        {
            string key = e.OurKey;
            if (!Ready) return Skip(key, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            if (!ModUsable(e.TargetMod)) return Skip(key, e.TargetMod + " 대상이 없거나 버전이 일치하지 않음");

            ConfigRegistry reg = HookRuntime.Registry;
            string switchKey = PanelKeys.ItemSwitchKey(key);
            object original = CurrentBoxed(key);
            bool switchWasOn = reg.Current<bool>(switchKey);
            object testValue = NextMirrorValue(e, original);

            try
            {
                if (switchWasOn)
                {
                    SetValueBoxed(key, testValue);
                    string theirs1 = ReadTarget(e);
                    if (!SameText(theirs1, testValue))
                        return Fail(key, "설정 다시 쓰기 경로: 상대 모드=" + theirs1 + " 이쪽 설정값 예상 " + Describe(testValue));
                    SetValueBoxed(key, original);
                    string theirs2 = ReadTarget(e);
                    return SameText(theirs2, original)
                        ? Pass(key, "기존에 켜진 스위치의 설정을 바꿨다가 되돌려도 값이 일치함(저장한 원래 값에 영향 없음)")
                        : Fail(key, "원래 상태로 되돌려 쓰기: 상대 모드=" + theirs2 + " 예상 " + Describe(original));
                }
                string before = ReadTarget(e);
                SetValueBoxed(key, testValue);
                reg.SetPlayerValue(switchKey, true);    // → Enable(快照 before + 写对方)
                string enabled = ReadTarget(e);
                if (!SameText(enabled, testValue))
                    return Fail(key, "활성화한 뒤 상대 모드=" + enabled + " 이쪽 설정값 예상 " + Describe(testValue));
                reg.SetPlayerValue(switchKey, false);   // → Disable(恢复快照 before)
                string restored = ReadTarget(e);
                return SameText(restored, before)
                    ? Pass(key, "활성화하면 상대 모드에 기록하고 비활성화하면 저장한 기존 값으로 복원됨(변경·복원 과정에서 상대 설정값을 일시적으로 수정함)")
                    : Fail(key, "비활성화한 뒤 상대 모드=" + restored + " 저장한 기존 값 예상 " + (before ?? "null"));
            }
            catch (Exception ex)
            {
                return Fail(key, "설정값 동기화·복원 중 예외: " + ex.Message);
            }
            finally
            {
                SetValueBoxed(key, original);
                reg.SetPlayerValue(switchKey, switchWasOn);
            }
        }

        /// <summary>对方条目当前序列化值(目标/键缺席 = null;不写不动)。</summary>
        private static string ReadTarget(FamilyPanelEntry e)
        {
            ModConfig config = ModConfig.Find(e.TargetMod);
            ConfigEntry entry = config == null ? null : config.Get(e.TargetKey);
            return entry == null ? null : entry.Serialize();
        }

        /// <summary>镜像键测试值:按 Kind 取「合法且 ≠ 当前值」(Choices 取首个
        /// 非默认档;int 取范围内邻值;bool 取反;Input CSV 取确定性合法测试串)。</summary>
        private static object NextMirrorValue(FamilyPanelEntry e, object current)
        {
            if (e.Kind == PanelKind.Choice)
            {
                string cur = current as string;
                foreach (string c in e.Choices)
                    if (c != cur) return c;
                return current;
            }
            if (e.Kind == PanelKind.SliderInt)
            {
                int v = (int)current;
                return v < e.MaxInt ? v + 1 : e.MinInt;
            }
            if (e.Kind == PanelKind.Input)
            {
                // R9(IPH CSV 键):5 项确定性合法串——权重键非负且非全 0(首项 30),
                // 概率键各项 ∈ 0-100,双键同串合法;恰等于当前值时换首项 40。
                // 项数 ≤ 对方体型数(5),对方 Parse 缺项补默认、超项整串回退
                // (BodyCsvRules.Parse),5 项恰中。复原写回玩家原串(finally)。
                string probe = "30, 20, 20, 20, 20";
                return probe == (current as string) ? (object)"40, 20, 20, 20, 20" : probe;
            }
            return !((bool)current);
        }

        /// <summary>我方值的对方侧序列化形态(ConfigEntry.Format,invariant;
        /// 与 HookRuntime.MirrorEnable 的写透契约同一序列化)。</summary>
        private static bool SameText(string theirs, object ourValue)
        {
            if (theirs == null) return false;
            string ours = ourValue is string s ? ConfigEntry.Format(s)
                : ourValue is int i ? ConfigEntry.Format(i)
                : ourValue is float f ? ConfigEntry.Format(f)
                : ConfigEntry.Format((bool)ourValue);
            return string.Equals(theirs, ours, StringComparison.Ordinal);
        }

        private static string Describe(object v)
        {
            return v == null ? "null" : v.ToString();
        }

        // ------------------------------------------------------------------
        // Tier3 ReadPath(开关 on → 消费单元 Installed + Current 读回一致)
        // ------------------------------------------------------------------

        private static TestResult ReadPathOne(string key)
        {
            if (!Ready) return Skip(key, "패치 준비 전(저장 파일에 들어간 뒤 다시 실행)");
            foreach (string mod in Requires(key).Split(','))
                if (!ModPresent(mod.Trim()))
                    return Skip(key, mod.Trim() + " 없음");

            // 消费单元中「应装」的(MainHook/SubModHook 且非让位;VersionBlocked
            // 保留在列——期望态语义会把它对成 VersionBlocked,全部压制 → SKIP)
            var loadable = new List<HookUnitDef>();
            foreach (HookUnitDef def in HookCatalog.Units)
            {
                if (def.Keys == null || Array.IndexOf(def.Keys, key) < 0) continue;
                if (def.Kind != CarrierKind.MainHook && def.Kind != CarrierKind.SubModHook) continue;
                if (HookRuntime.RouteSnapshot[def.Id] != RouteDecision.Yield) loadable.Add(def);
            }
            if (loadable.Count == 0) return Skip(key, "이 키를 사용하는 구성 단위가 모두 다른 모드에 양보함");
            bool allBlocked = true;
            foreach (HookUnitDef def in loadable)
                if (HookRuntime.RouteSnapshot[def.Id] != RouteDecision.VersionBlocked)
                { allBlocked = false; break; }
            if (allBlocked) return Skip(key, "버전이 일치하지 않아 적용 차단됨(패널에 안내 표시)");

            object original = CurrentBoxed(key);
            Action restoreSwitch = TurnOn(key);
            try
            {
                foreach (HookUnitDef def in loadable)
                {
                    string expected = ExpectedState(def), actual = ActualState(def);
                    if (expected != actual)
                        return Fail(key, "구성 단위 " + def.Id + " 예상 " + expected + " 실제 " + actual);
                }
                object test = Mutate(key, original);
                object now = CurrentBoxed(key);
                return Equals(now, test)
                    ? Pass(key, "스위치를 켜면 구성 단위가 Installed 상태이며 Current를 다시 읽어도 값이 일치함")
                    : Fail(key, "Current 예상 " + Describe(test) + " 실제 " + Describe(now));
            }
            catch (Exception ex)
            {
                return Fail(key, "값 조회 경로에서 예외 발생: " + ex.Message);
            }
            finally
            {
                SetValueBoxed(key, original);
                restoreSwitch();
            }
        }

        /// <summary>值键的确定性变形(int/float +1、bool 取反;CSV 首段数字 +1、
        /// 其余加后缀——池写转发对解析失败逐项隔离记日志,复原时写回原值)。</summary>
        private static object Mutate(string key, object original)
        {
            var reg = HookRuntime.Registry;
            if (original is int i)
            {
                reg.SetPlayerValue(key, i + 1);
                return i + 1;
            }
            if (original is float f)
            {
                reg.SetPlayerValue(key, f + 1f);
                return f + 1f;
            }
            if (original is bool b)
            {
                reg.SetPlayerValue(key, !b);
                return !b;
            }
            string s = original as string;
            if (string.IsNullOrEmpty(s))
            {
                reg.SetPlayerValue(key, "selftest");
                return "selftest";
            }
            string[] parts = s.Split(',');
            if (parts.Length > 1)
            {
                parts[0] = int.TryParse(parts[0], out int n) ? (n + 1).ToString() : parts[0] + "_t";
                string joined = string.Join(",", parts);
                reg.SetPlayerValue(key, joined);
                return joined;
            }
            string single = int.TryParse(s, out int m) ? (m + 1).ToString() : s + "_t";
            reg.SetPlayerValue(key, single);
            return single;
        }

        // ------------------------------------------------------------------
        // 配置读写/开关支撑(临时写并复原的通道)
        // ------------------------------------------------------------------

        /// <summary>目录行的在场要求(SelfTestPlan 同源计算)。</summary>
        private static string Requires(string key)
        {
            return FamilyPanelKeys.TryGet(key, out FamilyPanelEntry e)
                ? e.TargetMod : FamilyMods.MainMod;
        }

        /// <summary>在场且版本命中绑定表(直测/写透的 SKIP 判定)。</summary>
        private static bool ModUsable(string modId)
        {
            return HookRuntime.FamilySnapshot.TryGetValue(modId, out FamilyState st)
                && st.Present && st.VersionOk;
        }

        /// <summary>仅在场(ReadPath 判定;版本压制由期望态语义处理)。</summary>
        private static bool ModPresent(string modId)
        {
            return HookRuntime.FamilySnapshot.TryGetValue(modId, out FamilyState st)
                && st.Present;
        }

        /// <summary>拨开(有子开关拨子开关;Choice/Toggle 键设非零/真档),
        /// 返回复原委托(finally 调用,回到拨前态)。F4:与 SwitchOnFor 同源分派
        /// (ContainsKey 显式判定,零异常驱动)——9 布局尺寸键无开关不上面板,
        /// 期望态恒 false、无可拨,返回 no-op 委托(其消费单元期望态 Absent ==
        /// 实际态,断言路径照常通过;旧 KNF 回退对 int 键是 InvalidCastException,
        /// Tier3 105 键含布局键必炸)。</summary>
        private static Action TurnOn(string key)
        {
            var reg = HookRuntime.Registry;
            string switchKey = PanelKeys.ItemSwitchKey(key);
            if (reg.ContainsKey(switchKey))
            {
                bool was = reg.Current<bool>(switchKey);
                reg.SetPlayerValue(switchKey, true);
                return () => reg.SetPlayerValue(switchKey, was);
            }
            if (key == CfgKeys.GenderScope)
            {
                int was = reg.Current<int>(key);
                reg.SetPlayerValue(key, 1);
                return () => reg.SetPlayerValue(key, was);
            }
            if (key == CfgKeys.GuestDisplayEnabled || key == CfgKeys.NamesUseCustomFile)
            {
                bool wasB = reg.Current<bool>(key);
                reg.SetPlayerValue(key, true);
                return () => reg.SetPlayerValue(key, wasB);
            }
            // 0.7.1 拾取器:SubModHook 的 Toggle 自开关(表驱动分流,与
            // SwitchDispatch.IsOn 同条件——键名不散写)——「拨开」= 设 true
            if (FamilyPanelKeys.TryGet(key, out FamilyPanelEntry self)
                && ConfigRegistry.SelfSwitchForm(self)
                && self.Kind == PanelKind.Toggle)
            {
                bool wasSelf = reg.Current<bool>(key);
                reg.SetPlayerValue(key, true);
                return () => reg.SetPlayerValue(key, wasSelf);
            }
            // 0.7.0 T9:初始处女反向自开关(默认 true = 原版零参与)——「拨开」
            // = 拨 false(SwitchDispatch 反向分派下 IsOn 为 true = 单元应装)
            if (key == CfgKeys.InitialVirgin)
            {
                bool wasVirgin = reg.Current<bool>(key);
                reg.SetPlayerValue(key, false);
                return () => reg.SetPlayerValue(key, wasVirgin);
            }
            return () => { }; // 布局尺寸键等无开关键:无期望态可拨,no-op 复原
        }

        /// <summary>Current 装箱读取(注册表类型全集 int/float/bool/string 试探;
        /// KeyNotFoundException 穿透=真未注册,InvalidCastException 换下一类型)。
        /// R12 起 internal:DebugPanel ③区快捷开关同源复用(零平行复刻)。</summary>
        internal static object CurrentBoxed(string key)
        {
            var reg = HookRuntime.Registry;
            try { return reg.Current<int>(key); }
            catch (InvalidCastException) { }
            try { return reg.Current<float>(key); }
            catch (InvalidCastException) { }
            try { return reg.Current<bool>(key); }
            catch (InvalidCastException) { }
            return reg.Current<string>(key);
        }

        /// <summary>装箱写回(按运行时类型分派 SetPlayerValue 重载)。R12 起
        /// internal:DebugPanel ③区快捷开关同源复用(零平行复刻)。</summary>
        internal static void SetValueBoxed(string key, object v)
        {
            var reg = HookRuntime.Registry;
            if (v is int i) reg.SetPlayerValue(key, i);
            else if (v is float f) reg.SetPlayerValue(key, f);
            else if (v is bool b) reg.SetPlayerValue(key, b);
            else if (v is string s) reg.SetPlayerValue(key, s);
            else throw new InvalidOperationException("지원하지 않는 설정값 자료형: " + v.GetType().Name);
        }

        private static void SetInt(string key, int v)
        {
            HookRuntime.Registry.SetPlayerValue(key, v);
        }

        // ------------------------------------------------------------------
        // 类型/委托解析(ModReflect;MainModResolver.MainModId 常量同源)
        // ------------------------------------------------------------------

        /// <summary>主 mod 前置类型按名解析(调用前提 = ModUsable(主 mod) 已过)。</summary>
        private static System.Type MainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name) return t;
            throw new MissingMemberException("본편 모드가 다음 타입을 제공하지 않음: " + name);
        }

        /// <summary>brothel 前置类型按名解析(调用前提 = ModUsable(brothel) 已过)。</summary>
        private static System.Type BrothelType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Brothel))
                if (t.Name == name) return t;
            throw new MissingMemberException("brothel 모드가 다음 타입을 제공하지 않음: " + name);
        }

        private static T BrothelFunc<T>(string typeName, string method) where T : Delegate
        {
            return ModReflect.StaticMethod<T>(BrothelType(typeName), method);
        }

        /// <summary>remake(ShadowTrade)前置类型按名解析(与 BrothelType 同模式;
        /// 调用前提 = ModUsable(remake) 已过——WithSetupN 脚手架先行判定)。</summary>
        private static System.Type RemakeType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Remake))
                if (t.Name == name) return t;
            throw new MissingMemberException("lewd_meme_remake 모드가 다음 타입을 제공하지 않음: " + name);
        }

        /// <summary>ST5 疲劳售价四档直测断言体:对方疲劳系统关闭时 ValueMul 恒 1
        /// (写透键 fatigue_enabled = "끄기" → Enabled=false,源码 :212 早退 1f),
        /// 无区分度 → SKIP 设计态;开启时直调 ValueMul(代表值)断言面板测试值
        /// (委托执行期现解析,解析异常 = FAIL,与 brothel/invoke.* 同口径)。</summary>
        private static TestResult FatigueMulCase(string key, int fatigue, float expected, string note)
        {
            System.Type t = RemakeType("MaidenFatigue");
            bool enabled = ModReflect.StaticMethod<Func<bool>>(t, "get_Enabled")();
            if (!enabled) return Skip(key, "상대 모드 피로 시스템이 꺼져 있음(fatigue_enabled=끔). ValueMul은 항상 1");
            float r = ModReflect.StaticMethod<Func<int, float>>(t, "ValueMul")(fatigue);
            return r == expected
                ? Pass(key, "ValueMul(" + fatigue + ")=" + r + "(" + note + ")")
                : Fail(key, "ValueMul(" + fatigue + ") 예상 " + expected + " 실제 " + r + "(" + note + ")");
        }

        // ------------------------------------------------------------------
        // 结果构造
        // ------------------------------------------------------------------

        private static TestResult Pass(string id, string detail)
        {
            return new TestResult
            { Id = id, Section = "ok", Pass = true, Detail = detail };
        }

        private static TestResult Skip(string id, string detail)
        {
            return new TestResult
            { Id = id, Section = "skip", Pass = true, Detail = SelfTestPlan.SkipMark + ": " + detail };
        }

        private static TestResult Fail(string id, string detail)
        {
            return new TestResult
            { Id = id, Section = "fail", Pass = false, Detail = detail };
        }
    }
}
