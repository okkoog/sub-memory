using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// postfix 结果修正批(Task 10)——价格/精液效果/角色池上限/客人格开关/天赋数值。
    /// 条目清单(catalog-id → patch;MainHook 单元,开关开且版本受支持时装配)+
    /// pools.guest_display(DataWrite 载体,不进 HookInstaller):
    ///  <list type="bullet">
    ///   <item>main.price_milk(postfix <c>ItemFactory.CreateMilk</c>)——乳汁
    ///     单价 unitValue(原版 180)。</item>
    ///   <item>main.price_milk_credit(postfix <c>ItemFactory.CreateMilk</c>)——
    ///     饮奶 AddCredit(30) 字面量在 onActivateSlotItemFunc 委托内(编译器生成
    ///     lambda,无法稳定定位),postfix 换装等价委托:AddCredit(cfg)+主 mod 日志
    ///     +Destroy();默认 30 时不换装(原版委托原样保留,零漂移)。</item>
    ///   <item>main.price_semen / main.price_tentacle_box / main.price_ovulation_shot
    ///     (postfix Create*)——unitValue 90 / 200 / 120。</item>
    ///   <item>0.7.0 T7:Create 系价格六键同款 postfix 集中于此
    ///     (<see cref="AlarmPricePostfix"/> 六方法)——alarm.price 75 /
    ///     alarm.tentacle_price 150 / trap.price 100 / trap.trap_price 200 /
    ///     printer.price 500 / printer.needle_price 15;install 委托随各消费
    ///     单元留 AlarmPatch/TrapPatch/PrinterPatches(目标类型解析各归各类),
    ///     载体语义与本批价格表一致:Create* 方法体照跑(1.9.2 新类型行与
    ///     ApplyCsv 尾调自动在场),postfix 在整个方法之后改 __result.unitValue,
    ///     恒胜方法体内的 ApplyCsv 覆价(T2 C' 节 IL 实证)。</item>
    ///   <item>main.semen_effects(postfix ×3)——买价修正
    ///     MakeSemenFeature.SetPreExposedValueMod(-15)(client.semen_price_mod)、
    ///     卖价加成 MakeBuyBonusFeature.SetPreExposedValueMod(15)
    ///     (client.semen_bonus_mod)、声望倍率 TryApplySemen 的
    ///     baseline×1.15(client.semen_rep_mult,postfix 按 repBaselines 基线重写,
    ///     默认 1.15 时跳过——原版已写同值)。</item>
    ///   <item>main.pool_size(prefix <c>ClientPool.Resolve</c>)——
    ///     MAX_PER_CATEGORY 为 const(写不进),接管唯一消费点:复用概率
    ///     count/3、满池阈值 count&gt;=3、入池上限 count&lt;3 三处字面量;默认 3
    ///     放行原版(零漂移),≠3 走逐行复刻体(Load/ApplyTo/Save/New 经
    ///     ModReflect 调原实现,RollAttr/RandomName 经委托,NameGen RNG 流一致)。</item>
    ///   <item>pools.guest_display(DataWrite,无补丁)——<c>GuestBox.DisplayEnabled</c>
    ///     为 public static bool 字段:装配时写入 + Changed 经 HookRuntime 分发器
    ///     重写(同 maiden 池写先例);默认 true 与现值一致 → 零写入。</item>
    ///   <item>main.perk_scav_bonus(postfix 游戏 <c>ScavHelper.GetMaxScavAttempts</c>,
    ///     Priority.Low 保证排在主 mod PatchScavAttempts.Postfix 之后)——主 mod
    ///     postfix 已 +3,本 postfix 补差 (cfg−3) 使总额 = perk.scav_bonus;
    ///     默认 3 → +0;perk 未激活时主 mod 不加,本 postfix 同门控不加。</item>
    ///   <item>main.perk_daily_females(prefix+postfix <c>Perks.OnNewDay</c>,
    ///     即 ModHook.OnWakingUpLate 处理器)——0 = prefix 跳过(关闭);1(默认)=
    ///     放行原版每日 1 位(零漂移);N&gt;1 = 原版跑 1 位 + postfix 逐位复刻体
    ///     补 N−1 位(OnNewDay 主体逐行复刻,NameGen 经委托)。</item>
    ///  </list>
    /// 孤儿键(本批不实现,报告登记由 controller 路由):perk.trait_slots——
    /// 字面量 3 是 <c>Perks.Register</c> 里 <c>ModHelper.AddStartingPerk(...,
    /// extraPerkSlots:3,...)</c> 的注册参数,且消费点全在主菜单新游戏界面
    /// (<c>NewGamePerksUI.MaxPerkCount</c> 静态 + <c>PerkUIController.SelectPerk/
    /// DeselectPerk</c> 实例),本 mod 装配点(物品目录初始化,游戏场景)无法
    /// 保证覆盖应用启动后的首个新游戏天赋界面;CfgKeys/ConfigRegistry 亦未登记
    /// 该键(配置层无入口,Task 4 清单 G 只建了 scav/daily 两键)。
    /// 与原版的行为差异(sanctioned):
    ///  1) 配置读取集中在任何状态写入之前(Phase A);读失败(键缺失/类型不符)
    ///     → 记日志放行原版/跳过修正;postfix 修正体(Phase B)异常吞掉记日志,
    ///     绝不回滚原版已写入的状态;pool_size 接管体 Phase B 异常 → 不放行原版
    ///     (防半写池数据被原版重放)置 null 记日志(原版调用方 GuestBox 仅透传
    ///     ApplyTo,不读返回值字段,消费安全);
    ///  2) milk_credit ≠30 时换装委托丢弃原 lambda 的 ModCtx.Log 调用点,由
    ///     mainLog 委托复刻同文案(日志出口与原版一致);
    ///  3) 取整方向统一传统四舍五入(ValueMath,半点远离零)。
    /// 主 mod 类型(ItemFactory/TentacleEffects/ClientPool/GuestBox/Perks/NameGen/
    /// ModCtx)一律运行时经 ModReflect 解析成委托/类型引用,不编译引用
    /// MidnightPawn.*;游戏类型(GameItem/ItemFeature/StoreClient/ScavHelper/
    /// SpriteDict/ModHelper)直接编译引用。解析只发生在装配委托内
    /// (装配 = 物品目录初始化后,本 mod 已被加载器登记,ModReflect 合法),
    /// 绝不进静态构造,两阶段提交(任一成员解析失败 → 原样上抛不提交,全部
    /// 条目一致不装,同 Task 7/8/9)。安装失败由 HookInstaller 逐单元隔离记
    /// Dead,不阻断其余补丁。无协程/Invoke 调用面,不涉及 12/10 并发通道替换。
    /// </summary>
    public static class ValuePostfixPatches
    {
        /// <summary>主 mod Perks.PERK_TENTACLE 常量复刻(天赋判定门控用)。</summary>
        internal const string PerkId = "lewd_meme.tentacle_origin";

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,patch 只做直接调用) --
        private static bool resolved;                                  // 全部成员就位后才置真
        private static System.Type itemFactory;                        // ItemFactory(价格 postfix 目标 + RollAttr)
        private static System.Type tentacleEffects;                    // TentacleEffects(精液效果三目标)
        private static System.Type clientPool;                         // ClientPool(Resolve 接管 + Load/ApplyTo/Save)
        private static System.Type characterData;                      // ClientPool.CharacterData(嵌套,New+字段)
        private static System.Type guestBox;                           // GuestBox(DisplayEnabled 字段)
        private static System.Type perks;                              // Perks(OnNewDay 接管)
        private static Func<string> randomName;                        // NameGen.RandomName
        private static MethodBase randomNameParts;                     // 0.8.0:NameGen.RandomNameParts(out, out)(2.1.0 姓/名拆分)
        private static Func<int> rollAttr;                             // ItemFactory.RollAttr
        private static Action<object> mainLog;                         // ModCtx.Log
        private static Action<object> mainLogWarning;                  // ModCtx.LogWarning

        /// <summary>把九条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。pools.guest_display
        /// 是 DataWrite 载体,不进映射也不进 HookInstaller(R7):装配期一次写入走
        /// WriteGuestDisplayAtCompose,Changed 重写经 HookRuntime 分发器转发
        /// OnConfigChanged。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            // -- 价格表(ItemFactory.Create* postfix,清单 D) --
            installs.Add("main.price_milk", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateMilk", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(MilkPricePostfix));
                return new[] { original };
            });
            installs.Add("main.price_milk_credit", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateMilk", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(MilkCreditPostfix));
                return new[] { original };
            });
            installs.Add("main.price_semen", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateSemen", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(SemenPricePostfix));
                return new[] { original };
            });
            installs.Add("main.price_tentacle_box", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateTentacleBox", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(TentacleBoxPricePostfix));
                return new[] { original };
            });
            installs.Add("main.price_ovulation_shot", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateOvulationShot", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(OvulationShotPricePostfix));
                return new[] { original };
            });

            // -- 0.7.0 Task 7:Create 系价格 postfix 的方法体集中在此(六个
            //    AlarmPricePostfix 以下的方法);install 委托随各消费单元留在
            //    AlarmPatch(create_alarm/create_tentacle_alarm)、TrapPatch
            //    (create_manhole/create_trap)、PrinterPatches(main.printer)——
            //    目标类型解析各归各类,本类不新增解析职责。载体语义 =
            //    price.milk 五键同款:Create* 方法体照跑(1.9.2 的 TENTACLE_
            //    CORRUPT/TENTACLE_TOOL 新类型行、ApplyCsv 尾调全部自动在场),
            //    postfix 在整个方法之后执行改 __result.unitValue,尾部
            //    ApplyCsv 的 CSV 覆价被恒定压过(T2 C' 节 IL 实证时序)--

            // -- 触手精液三值(TentacleEffects,清单 F) --
            installs.Add("main.semen_effects", h =>
            {
                EnsureResolved();
                MethodBase feat = ModReflect.Target(tentacleEffects, "MakeSemenFeature", Type.EmptyTypes);
                MethodBase bonus = ModReflect.Target(tentacleEffects, "MakeBuyBonusFeature", Type.EmptyTypes);
                MethodBase apply = ModReflect.Target(tentacleEffects, "TryApplySemen", new[] { typeof(GameItem) });
                h.Patch(feat, postfix: new HarmonyMethod(MakeSemenFeaturePostfix));
                h.Patch(bonus, postfix: new HarmonyMethod(MakeBuyBonusFeaturePostfix));
                h.Patch(apply, postfix: new HarmonyMethod(TryApplySemenPostfix));
                return new[] { feat, bonus, apply };
            });

            // -- 角色池上限(ClientPool.Resolve 接管,MAX_PER_CATEGORY const 写不进) --
            installs.Add("main.pool_size", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(clientPool, "Resolve",
                    new[] { typeof(string), typeof(StoreClient) });
                h.Patch(original, prefix: new HarmonyMethod(ClientPoolResolvePrefix));
                return new[] { original };
            });

            // -- 拾荒次数(游戏 ScavHelper postfix 之 postfix,总额 = perk.scav_bonus) --
            installs.Add("main.perk_scav_bonus", h =>
            {
                MethodInfo original = typeof(ScavHelper).GetMethod(
                    nameof(ScavHelper.GetMaxScavAttempts),
                    BindingFlags.Public | BindingFlags.Static);
                if (original == null)
                    throw new MissingMethodException(nameof(ScavHelper), nameof(ScavHelper.GetMaxScavAttempts));
                // Priority.Low:恒排主 mod PatchScavAttempts.Postfix(Normal)之后,
                // 不依赖双方补丁的注册先后顺序。
                h.Patch(original, postfix: new HarmonyMethod(GetMaxScavAttemptsPostfix, Priority.Low));
                return new[] { original };
            });

            // -- 每日吸引女客(Perks.OnNewDay 接管:0=关/N=1 原版/N>1 追加) --
            installs.Add("main.perk_daily_females", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(perks, "OnNewDay", Type.EmptyTypes);
                h.Patch(original,
                    prefix: new HarmonyMethod(OnNewDayPrefix),
                    postfix: new HarmonyMethod(OnNewDayPostfix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(全部条目共享;任一成员解析抛出 → 本方法原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 全部条目一致不装,杜绝半解析状态
        /// 下装出运行时 NRE 的 patch——两阶段提交,同 Task 7/8/9)。失败由 HookInstaller
        /// 逐单元隔离记 Dead,该组不装配。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type factory = FindMainType("ItemFactory");
            System.Type effects = FindMainType("TentacleEffects");
            System.Type pool = FindMainType("ClientPool");
            System.Type data = FindMainType("CharacterData");   // 嵌套类 ClientPool.CharacterData
            System.Type guest = FindMainType("GuestBox");
            System.Type perkType = FindMainType("Perks");
            System.Type names = FindMainType("NameGen");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedRandomName = ModReflect.StaticMethod<Func<string>>(names, "RandomName");
            // 0.8.0:2.1.0 新增姓/名拆分(out 双参,泛型委托拿不到 → MethodBase
            // 反射 Invoke);解析失败 = Dead(绑定 2.1.0 下必在)
            MethodBase resolvedNameParts = ModReflect.Target(names, "RandomNameParts",
                new[] { typeof(string).MakeByRefType(), typeof(string).MakeByRefType() });
            var resolvedRollAttr = ModReflect.StaticMethod<Func<int>>(factory, "RollAttr");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            itemFactory = factory;
            tentacleEffects = effects;
            clientPool = pool;
            characterData = data;
            guestBox = guest;
            perks = perkType;
            randomName = resolvedRandomName;
            randomNameParts = resolvedNameParts;
            rollAttr = resolvedRollAttr;
            mainLog = resolvedLog;
            mainLogWarning = resolvedLogWarning;
            resolved = true;
        }

        /// <summary>在前置 lewd_meme 暴露的类型里按名查找(同名不同命名空间亦命中)。</summary>
        private static System.Type FindMainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("主 mod lewd_meme 未暴露类型 " + name);
        }

        // ------------------------------------------------------------------
        // 价格表:Create* postfix 改 unitValue(默认值 = 原版字面量,同值重写无漂移)
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void MilkPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.PriceMilk, 180);
            }
            catch (Exception ex)
            {
                LogMainWarning("乳汁价格修正失败: " + ex.Message);
            }
        }

        /// <summary>
        /// 饮奶信用:字面量 30 在原版 onActivateSlotItemFunc 委托体内(编译器生成
        /// lambda),postfix 在物品创建点换装等价委托。默认 30 → 不换装(原版委托
        /// 原样保留);≠30 → AddCredit(cfg) + mainLog(同文案) + Destroy()。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void MilkCreditPostfix(GameItem __result)
        {
            if (__result == null) return;
            int credit;
            try
            {
                credit = Cfg(CfgKeys.PriceMilkCredit, 30);
            }
            catch (Exception ex)
            {
                LogMainWarning("乳汁饮用信用配置读取失败,保持原版委托: " + ex.Message);
                return;
            }
            if (credit == 30) return; // 默认:原版委托(AddCredit(30)+Log+Destroy)零漂移
            __result.onActivateSlotItemFunc = delegate (GameItem it, GameInventory inv, SlotMarker slot)
            {
                ModHelper.AddCredit(credit, true);
                LogMain("饮下一瓶神奇乳汁");
                it.Destroy();
            };
        }

        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void SemenPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.PriceSemen, 90);
            }
            catch (Exception ex)
            {
                LogMainWarning("精液价格修正失败: " + ex.Message);
            }
        }

        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void TentacleBoxPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.PriceTentacleBox, 200);
            }
            catch (Exception ex)
            {
                LogMainWarning("苗床箱价格修正失败: " + ex.Message);
            }
        }

        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void OvulationShotPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.PriceOvulationShot, 120);
            }
            catch (Exception ex)
            {
                LogMainWarning("排卵诱发剂价格修正失败: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 0.7.0 Task 7:Create 系价格六键(alarm 2 + trap 2 + printer 2)。
        // 0.6.0 期由 Create* prefix 复刻体整方法接管(逐行复刻换单价),1.9.2
        // 方法体漂移(新类型行/ApplyCsv 尾调)令复刻面失控——退役后原方法照跑,
        // postfix 只改 __result.unitValue 一处,签名/文案/新行为全部零复刻;
        // postfix 在方法尾部 ApplyCsv 之后执行,CSV 覆价恒被压过(T2 C' 实证)。
        // install 委托:alarm 两键在 AlarmPatch、trap 两键在 TrapPatch、
        // printer 两键在 PrinterPatches(各消费单元的 install 内引用下列方法)。
        // ------------------------------------------------------------------

        /// <summary>alarm.price(75):普通闹钟单价。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        internal static void AlarmPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.AlarmPrice, 75);
            }
            catch (Exception ex)
            {
                LogMainWarning("闹钟价格修正失败: " + ex.Message);
            }
        }

        /// <summary>alarm.tentacle_price(150):触手闹钟单价。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        internal static void AlarmTentaclePricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.AlarmTentaclePrice, 150);
            }
            catch (Exception ex)
            {
                LogMainWarning("触手闹钟价格修正失败: " + ex.Message);
            }
        }

        /// <summary>trap.price(100):井盖单价。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        internal static void TrapPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.TrapPrice, 100);
            }
            catch (Exception ex)
            {
                LogMainWarning("井盖价格修正失败: " + ex.Message);
            }
        }

        /// <summary>trap.trap_price(200):触手落穴陷阱单价。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        internal static void TrapTrapPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.TrapTrapPrice, 200);
            }
            catch (Exception ex)
            {
                LogMainWarning("落穴陷阱价格修正失败: " + ex.Message);
            }
        }

        /// <summary>printer.price(500):触手3D打印机单价。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        internal static void PrinterPricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.PrinterPrice, 500);
            }
            catch (Exception ex)
            {
                LogMainWarning("触手打印机价格修正失败: " + ex.Message);
            }
        }

        /// <summary>printer.needle_price(15):空针管单价。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        internal static void PrinterNeedlePricePostfix(GameItem __result)
        {
            if (__result == null) return;
            try
            {
                __result.unitValue = Cfg(CfgKeys.PrinterNeedlePrice, 15);
            }
            catch (Exception ex)
            {
                LogMainWarning("空针管价格修正失败: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 触手精液三值:买价修正 / 卖价加成 / 声望倍率(每顾客一次,原版门控)
        // ------------------------------------------------------------------

        /// <summary>MakeSemenFeature(-15):重设 SetPreExposedValueMod(默认 -15 同值)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void MakeSemenFeaturePostfix(ItemFeature __result)
        {
            if (__result == null) return;
            try
            {
                __result.SetPreExposedValueMod(
                    ValueMath.PercentToInt(Cfg(CfgKeys.SemenPriceMod, -15f)), true);
            }
            catch (Exception ex)
            {
                LogMainWarning("精液买价修正失败: " + ex.Message);
            }
        }

        /// <summary>MakeBuyBonusFeature(+15):重设 SetPreExposedValueMod(默认 15 同值)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void MakeBuyBonusFeaturePostfix(ItemFeature __result)
        {
            if (__result == null) return;
            try
            {
                __result.SetPreExposedValueMod(
                    ValueMath.PercentToInt(Cfg(CfgKeys.SemenBonusMod, 15f)), true);
            }
            catch (Exception ex)
            {
                LogMainWarning("精液卖价加成修正失败: " + ex.Message);
            }
        }

        /// <summary>
        /// TryApplySemen:原版在首次生效时写 tradeReputationMultiplier = baseline×1.15
        /// (baseline 存 repBaselines 字典)。postfix 仅在 __result=true(本次生效)且
        /// 配置 ≠1.15 时按同一 baseline 重写为 baseline×cfg;默认 1.15 → 原版已写同值,
        /// 跳过(零漂移)。baseline 经 repBaselines(readonly 字段,只读)精确取回,
        /// 不做除法反推,避免浮点误差。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void TryApplySemenPostfix(GameItem item, bool __result)
        {
            if (!__result) return;
            try
            {
                float mult = Cfg(CfgKeys.SemenRepMult, 1.15f);
                if (mult == 1.15f) return; // 默认:原版已写 activeBaseRep×1.15
                // 0.8.0 迁移:1.10.0 起 repBaselines 全局字典删除(同类顾客加成
                // 传染修复),改 activeClient/activeBaseRep 单顾客静态字段
                // (MarkAffected 进店记基线,ClearAffected 离店恢复);postfix
                // 重写 = baseline×mult,与原方法尾部 ×1.15 同一落点
                StoreClient client = ModReflect.GetStatic<StoreClient>(tentacleEffects, "activeClient");
                if (client == null) return;
                double baseline = ModReflect.GetStatic<double>(tentacleEffects, "activeBaseRep");
                client.tradeReputationMultiplier = baseline * mult;
            }
            catch (Exception ex)
            {
                LogMainWarning("精液声望倍率修正失败: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // ClientPool.Resolve 接管:count/3、count>=3、count<3 三处字面量 → pool_size
        // ------------------------------------------------------------------

        /// <summary>
        /// prefix:默认(钳制后 = 3)放行原版;≠3 跳过原方法走复刻体。
        /// 复刻体 Phase B 异常 → 不放行原版(半写池数据防重放),__result 置 null
        /// (原版调用方 GuestBox.Show 不读返回值成员,只依赖 ApplyTo 副作用)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ClientPoolResolvePrefix(string category, StoreClient client, ref object __result)
        {
            int size;
            try
            {
                size = ValueMath.PoolSizeClamp(Cfg(CfgKeys.ClientPoolSize, 3));
            }
            catch (Exception ex)
            {
                LogMainWarning("角色池上限配置读取失败,放行原版: " + ex.Message);
                return true;
            }
            if (size == 3) return true; // 默认 = 原版 MAX_PER_CATEGORY=3
            try
            {
                __result = ResolveBody(category, client, size);
            }
            catch (Exception ex)
            {
                LogMainWarning("角色池复用接管失败: " + ex.Message);
                __result = null;
            }
            return false;
        }

        /// <summary>
        /// 逐行复刻 v1.3.8 ClientPool.Resolve;差异仅三处 3 → size(概率分母 /
        /// 满池阈值 / 入池上限)。List&lt;CharacterData&gt; 与 CharacterData 均主 mod
        /// 类型:经 IList 非泛型接口消费(list 为 List&lt;T&gt;,实现 IList),字段
        /// 读写经 ModReflect.Get/Set,Load/ApplyTo/Save/New/RollAttr/RandomName 全部
        /// 回原实现(零漂移);Random.value/Range 消费点与原版一致(短路次序保留)。
        /// </summary>
        private static object ResolveBody(string category, StoreClient client, int size)
        {
            IList list = ModReflect.CallStatic(clientPool, "Load", category) as IList;
            int count = (list != null) ? list.Count : 0;
            if (count > 0)
            {
                float prob = (float)count / (float)size;
                if (count >= size || UnityEngine.Random.value < prob)
                {
                    object reused = list[UnityEngine.Random.Range(0, count)];
                    ModReflect.CallStatic(clientPool, "ApplyTo", client, reused);
                    LogMain("顾客复用池内角色: [" + category + "] " + ModReflect.Get<string>(reused, "name"));
                    return reused;
                }
            }
            object fresh = ModReflect.New(characterData);
            // 0.8.0:2.1.0 起姓名三字段同源(RandomNameParts out 双参 → name/
            // surname/given 一体写入,2.0.2 修复「三字段对不上」);反射 Invoke
            // out 参(args 数组回收出参)
            object[] np = new object[2];
            string fullName = (string)randomNameParts.Invoke(null, np);
            ModReflect.Set(fresh, "name", fullName);
            ModReflect.Set(fresh, "surname", (string)np[0]);
            ModReflect.Set(fresh, "given", (string)np[1]);
            ModReflect.Set(fresh, "sprite", client.spriteName);
            ModReflect.Set(fresh, "force", rollAttr());
            ModReflect.Set(fresh, "intel", rollAttr());
            ModReflect.Set(fresh, "charm", rollAttr());
            if (count < size)
            {
                list.Add(fresh);
                ModReflect.CallStatic(clientPool, "Save", category, list);
            }
            ModReflect.CallStatic(clientPool, "ApplyTo", client, fresh);
            LogMain("顾客新建角色: [" + category + "] " + ModReflect.Get<string>(fresh, "name")
                + " 力" + ModReflect.Get<int>(fresh, "force")
                + " 智" + ModReflect.Get<int>(fresh, "intel")
                + " 魅" + ModReflect.Get<int>(fresh, "charm"));
            return fresh;
        }

        // ------------------------------------------------------------------
        // GuestBox.DisplayEnabled:静态字段写入(DataWrite 载体:装配期一次 + Changed 分发重写)
        // ------------------------------------------------------------------

        /// <summary>
        /// guest.display_enabled → GuestBox.DisplayEnabled(public static bool,非
        /// initonly,SetStatic 直接可写)。同值跳过(默认 true 零写入);写入失败
        /// 记日志(装配编排层逐条隔离)。
        /// </summary>
        private static void WriteGuestDisplay()
        {
            bool enabled = Cfg(CfgKeys.GuestDisplayEnabled, true);
            if (ModReflect.GetStatic<bool>(guestBox, "DisplayEnabled") == enabled)
            {
                LogMain("客人格显示开关与现值一致,跳过写入: " + enabled);
                return;
            }
            ModReflect.SetStatic(guestBox, "DisplayEnabled", enabled);
            LogMain("客人格显示开关已写入: " + enabled);
        }

        /// <summary>
        /// pools.guest_display 装配期一次写入(HookRuntime 主 mod 在场且版本受支持时
        /// 调用);后续重写经 HookRuntime 分发器转发 OnConfigChanged。
        /// </summary>
        internal static void WriteGuestDisplayAtCompose()
        {
            EnsureResolved();
            WriteGuestDisplay();
        }

        // ------------------------------------------------------------------
        // Changed 重写处理器(HookRuntime 分发器转发;R10:订阅/退订归分发器,
        // 本类不再自持 registry 引用)
        // ------------------------------------------------------------------

        /// <summary>guest.display_enabled 变更 → 重写字段(异常吞掉记日志)。</summary>
        internal static void OnConfigChanged(string key)
        {
            if (key != CfgKeys.GuestDisplayEnabled) return;
            try
            {
                WriteGuestDisplay();
            }
            catch (Exception ex)
            {
                LogMainWarning("客人格显示开关重写失败(" + key + "): " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // Perks.OnNewDay:每日吸引女客(0=关 / 1=原版 / N>1 追加 N-1)
        // ------------------------------------------------------------------

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool OnNewDayPrefix()
        {
            int n;
            try
            {
                n = Cfg(CfgKeys.PerkDailyFemales, 1);
            }
            catch (Exception ex)
            {
                LogMainWarning("每日女客配置读取失败,放行原版: " + ex.Message);
                return true;
            }
            if (n < 0) n = 0;
            if (n == 0) return false; // 关闭:整段跳过(perk 未激活时原版同样 no-op,可观测行为一致)
            return true;              // n≥1:原版跑 1 位;n>1 由 postfix 补足
        }

        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void OnNewDayPostfix()
        {
            try
            {
                int n = Cfg(CfgKeys.PerkDailyFemales, 1);
                if (n <= 1) return;
                if (!ModHelper.IsStartingPerkActive(PerkId)) return; // 原版门控同款
                for (int i = 0; i < n - 1; i++)
                {
                    AddFemaleClient();
                }
                LogMain("触手起源：今日追加吸引 " + (n - 1) + " 位女性顾客");
            }
            catch (Exception ex)
            {
                LogMainWarning("触手起源追加女客失败: " + ex.Message);
            }
        }

        /// <summary>逐行复刻 v1.3.8 Perks.OnNewDay 主体(游戏类型直连,NameGen 经委托);
        /// 差异仅"每次调用生成一位"(原版每次调用一位,复用同体)。</summary>
        private static void AddFemaleClient()
        {
            PlayerStore instance = PlayerStore.Instance;
            if (instance == null || instance.storeClientManager == null)
            {
                return;
            }
            StoreClientManager storeClientManager = instance.storeClientManager;
            StoreClient randomNewMinorClient = storeClientManager.GetRandomNewMinorClient();
            if (randomNewMinorClient == null)
            {
                LogMainWarning("触手起源：今天没有可用的顾客模板");
                return;
            }
            string randomLowerFemaleSpriteName = SpriteDict.GetRandomLowerFemaleSpriteName();
            if (!string.IsNullOrEmpty(randomLowerFemaleSpriteName))
            {
                randomNewMinorClient.spriteName = randomLowerFemaleSpriteName;
                randomNewMinorClient.possibleSprites = new List<string> { randomLowerFemaleSpriteName };
            }
            randomNewMinorClient.identifier = "lewd_meme_fan_" + Guid.NewGuid().ToString("N").Substring(0, 8);
            randomNewMinorClient.displayName = randomName();
            randomNewMinorClient.cooldown = 0;
            storeClientManager.AddClient(randomNewMinorClient);
            LogMain("触手起源：今日吸引来一位女性顾客 —— " + randomNewMinorClient.displayName);
        }

        // ------------------------------------------------------------------
        // ScavHelper.GetMaxScavAttempts:postfix 的 postfix(总额 = perk.scav_bonus)
        // ------------------------------------------------------------------

        /// <summary>
        /// 主 mod PatchScavAttempts.Postfix 在 perk 激活时已 +3 且先于本 postfix
        /// (Priority.Low,patch 期指定——恒排主 mod Normal postfix 之后,R8 例外:
        /// 既有显式优先级保持不动,同 GenderRatioPatch 先例)执行;本补丁补差
        /// (cfg−3) 使总加成 = perk.scav_bonus(默认 3 → +0)。perk 未激活:主 mod
        /// 不加,本补丁同门控不加。
        /// </summary>
        [HarmonyPostfix]
        private static void GetMaxScavAttemptsPostfix(ref int __result)
        {
            try
            {
                if (!ModHelper.IsStartingPerkActive(PerkId)) return;
                __result += Cfg(CfgKeys.PerkScavBonus, 3) - 3;
            }
            catch (Exception ex)
            {
                LogMainWarning("拾荒次数修正失败: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件
        // ------------------------------------------------------------------

        /// <summary>配置读取:注册表缺席(未注入/停用)时回退原版字面量默认值。</summary>
        private static T Cfg<T>(string key, T def)
        {
            return PatchCtx.Config != null ? PatchCtx.Config.Current<T>(key) : def;
        }

        /// <summary>日志走主 mod ModCtx(保持原版前缀与格式);委托缺席时退回 Debug.Log。</summary>
        private static void LogMain(object msg)
        {
            if (mainLog != null) mainLog(msg);
            else UnityEngine.Debug.Log("[lewd_meme_config→主mod] " + msg);
        }

        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else UnityEngine.Debug.LogWarning("[lewd_meme_config→主mod] " + msg);
        }
    }
}
