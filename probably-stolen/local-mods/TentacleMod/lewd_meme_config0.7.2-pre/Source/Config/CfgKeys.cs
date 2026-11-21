namespace LewdMemeConfig
{
    /// <summary>
    /// 配置键常量（patch 点与服务接口统一引用，禁止散写字符串）。
    /// 键名沿用旧草案 LewdMemeApiConfig.cs（已并入本文件），
    /// 并按 spec §5.1 增补 client.gender_scope 与 ui.* 四键。
    /// 默认值 = 主 mod (MidnightPawn.dll v1.7.1,Task 14b 起) 硬编码字面量
    /// （nightly/corrupt/alarm/trap 等两版零漂移；loot.med_* 双键默认 = 1.6.1
    /// 字面量——1.7.0 已删医疗组分支,双键为「恢复 1.6.x」opt-in;
    /// printer.* 五键 = 1.7.1 新增线），登记于 ConfigRegistry.CreateDefault()。
    /// </summary>
    public static class CfgKeys
    {
        // -- 夜间结算(接管 TentacleBody.NightlyTick) --
        public const string SexMinBase          = "nightly.sex_min_base";
        public const string SexPerLevelMin      = "nightly.sex_per_level_min";
        public const string SexMaxBase          = "nightly.sex_max_base";
        public const string SexPerLevelMax      = "nightly.sex_per_level_max";
        public const string OralMinBase         = "nightly.oral_min_base";
        public const string OralPerLevelMin     = "nightly.oral_per_level_min";
        public const string OralMaxBase         = "nightly.oral_max_base";
        public const string OralPerLevelMax     = "nightly.oral_per_level_max";
        public const string AnalMinBase         = "nightly.anal_min_base";
        public const string AnalPerLevelMin     = "nightly.anal_per_level_min";
        public const string AnalMaxBase         = "nightly.anal_max_base";
        public const string AnalPerLevelMax     = "nightly.anal_per_level_max";
        public const string OvulateChance       = "nightly.ovulate_chance";
        public const string ConceivePerSex      = "nightly.conceive_per_sex";
        public const string GestationDays       = "nightly.gestation_days";
        public const string LactationCap        = "nightly.lactation_cap";
        public const string MilkValueBase       = "nightly.milk_value_base";
        public const string MilkValuePerLv      = "nightly.milk_value_per_lv";
        public const string SemenChanceBase     = "nightly.semen_chance_base";
        public const string SemenChancePerTrain = "nightly.semen_chance_per_train";
        public const string SemenChanceCap      = "nightly.semen_chance_cap";
        public const string DaughterValue       = "nightly.daughter_value";
        public const string ExpMultiplier       = "nightly.exp_multiplier";
        public const string TrainTameThreshold  = "nightly.train_tame_threshold";

        // -- 布局与成长(接管 TentacleBody.BuildWindow / AddBodyExp / ExpNeedForLevel) --
        public const string BroodSize           = "layout.brood_size";
        public const string OutputSize          = "layout.output_size";
        public const string CorruptCols         = "layout.corrupt_cols";
        public const string ToolCols            = "layout.tool_cols";
        public const string TabsPerLevel        = "layout.tabs_per_level";
        // 0.7.0 T9:exp_per_level 键废弃(1.9.2 已删 EXP_NEED 静态数组,经验表
        // 公式化)→ 公式三键(progress.exp_base / exp_divisor / exp_step,
        // 单元 main.exp_formula 的 prefix 复刻体经 Logic/ExpCurve 求值)
        public const string ExpBase             = "progress.exp_base";       // 公式基础值(原版 100)
        public const string ExpDivisor          = "progress.exp_divisor";    // 每 N 级一档(原版 3,整数除法)
        public const string ExpStep             = "progress.exp_step";       // 每档增量(原版 100)
        public const string LevelCap            = "progress.level_cap";   // 等级上限(1.9.2 GetBodyLevel clamp 1-5;面板 1-999,升级循环无上限拦截、读取与显示按此封顶[T9 语义注记])

        // -- 侵蚀性触手(接管 CorruptTentacle 五方法) --
        public const string CorruptMaxCharge    = "corrupt.max_charge";
        public const string CorruptInitialCharge= "corrupt.initial_charge"; // 初始充能(1.6.1 起未变,1.7.1 同:Create 内 SetCharge(100);超上限由 max_charge 钳制)
        public const string CorruptDaily        = "corrupt.daily_recharge";
        public const string CorruptSemen        = "corrupt.semen_recharge";
        public const string CorruptCost         = "corrupt.corrupt_cost";
        public const string CorruptWhitelist    = "corrupt.whitelist_extra";
        public const string CorruptBlacklist    = "corrupt.blacklist_extra";

        // -- 掉落(接管 LootHook.OnLootRolled,Task 14b 对齐 1.7.1;med 双键 =
        //    「恢复 1.6.x 医疗组排卵针掉落」opt-in,默认值取 1.6.1 字面量)--
        public const string MedKeywords         = "loot.med_keywords";
        public const string MedShotChance       = "loot.med_shot_chance";
        public const string ScavGroups          = "loot.scav_groups";
        public const string ScavMaidenBase      = "loot.scav_maiden_base";      // 1.6.1/1.7.1:恒定 0.05(主 mod 面板动态层接管),不再随等级;per_level/shot_chance 键已随 1.6.1 移除并清理
        public const string LootThrottleSeconds = "loot.throttle_seconds";      // 掉落节流窗口(1.6.1/1.7.1 ROLL_COOLDOWN 内联 0.5s;面板 0-5,0=禁用节流)

        // -- 顾客系统 --
        public const string FemaleRewriteChance = "client.female_rewrite_chance";
        public const string GenderScope         = "client.gender_scope"; // 全女性 4 档(spec §6,int 0–3,默认 0)
        public const string ClientPoolSize      = "client.pool_size";
        public const string SemenPriceMod       = "client.semen_price_mod";
        public const string SemenBonusMod       = "client.semen_bonus_mod";
        public const string SemenRepMult        = "client.semen_rep_mult";
        public const string GuestDisplayEnabled = "guest.display_enabled";
        public const string PerkDailyFemales    = "perk.daily_female_clients";
        public const string PerkScavBonus       = "perk.scav_bonus";

        // -- 少女生成(0.7.0 T8:virgin_chance 键废弃——1.9.2 无条件 Enable
        //    VIRGIN/ANAL_VIRGIN 替代掷骰段;attr 五键/faction_base 迁
        //    main.maiden_roll / main.maiden_faction_base transpiler 单元;
        //    0.7.0 T9:新增 interact_threshold 互动阈值与 initial_virgin
        //    初始处女 bool 两键) --
        public const string AttrHighChance      = "maiden.attr_high_chance";
        public const string AttrMidChance       = "maiden.attr_mid_chance";   // 累计阈值
        public const string AttrHighRange       = "maiden.attr_high_range";  // "70,80"
        public const string AttrMidRange        = "maiden.attr_mid_range";   // "60,70"
        public const string AttrLowRange        = "maiden.attr_low_range";   // "10,60"
        public const string FactionBase         = "maiden.faction_base";     // "名:值,..."
        public const string InteractThreshold   = "maiden.interact_threshold"; // 互动次数上限的属性阈值(ItemFactory.ComputeInteractMax 四阈值 ≥60,默认 60 = 原版)
        public const string InitialVirgin       = "maiden.initial_virgin";   // 初始处女(1.9.2 无条件 Enable 双 tag;false = postfix Disable,Toggle 自开关反向分派)
        public const string PortraitSize        = "maiden.portrait_size";
        public const string Factions            = "maiden.factions";
        public const string Jobs                = "maiden.jobs";
        public const string Bodies              = "maiden.bodies";
        public const string Breasts             = "maiden.breasts";

        // -- 特殊顾客与名字 --
        public const string SpecialExtraIds     = "special.extra_ids";
        public const string SpecialExtraSprites = "special.extra_sprites";
        public const string SpecialExtraKeywords= "special.extra_keywords";
        public const string NamesUseCustomFile  = "names.use_custom_file";

        // -- 价格(Postfix ItemFactory.Create*) --
        public const string PriceMilk           = "price.milk";
        public const string PriceMilkCredit     = "price.milk_credit";
        public const string PriceSemen          = "price.semen";
        public const string PriceTentacleBox    = "price.tentacle_box";
        public const string PriceOvulationShot  = "price.ovulation_shot";

        // -- 开关体系(0.5.0):子项开关键由 PanelKeys.ItemSwitchKey 命名规则生成 --

        // -- UI(夜间报告窗口与读档修复;清单 A) --
        public const string UiReportWidth       = "ui.report_width";
        public const string UiReportHeight      = "ui.report_height";
        public const string UiReportDelay       = "ui.report_delay";
        public const string UiReloadFixDelay    = "ui.reload_fix_delay";

        // -- 闹钟(AlarmClock 复刻,1.6.1;默认 = 1.6.1 字面量,见 ConfigRegistry.RegisterAlarm) --
        public const string AlarmPrice          = "alarm.price";                // CreateAlarm 单价 75(postfix,0.7.0 T7)
        public const string AlarmTentaclePrice  = "alarm.tentacle_price";       // CreateTentacleAlarm 单价 150(postfix,0.7.0 T7)
        public const string AlarmBroodSize      = "alarm.brood_size";           // 触手闹钟苗床区 4(不上面板,同 layout.* 惯例;transpile,0.7.0 T7)
        public const string AlarmOutputSize     = "alarm.output_size";          // 触手闹钟产出区 4(不上面板;transpile,0.7.0 T7)
        public const string AlarmOralPerNight   = "alarm.oral_per_night";       // NightlyTickOnce 每夜口交 +1
        public const string AlarmSexPerNight    = "alarm.sex_per_night";        // NightlyTickOnce 每夜性交 +1
        public const string AlarmAnalPerNight   = "alarm.anal_per_night";       // NightlyTickOnce 每夜肛交 +1
        public const string AlarmSemenOutput    = "alarm.semen_output";         // 放弃键:1.9.2 产出无条件每苗床一瓶,无数值 ldc(DataOnlyKeys,面板行保留)
        public const string AlarmNpcSellChance  = "alarm.npc_sell_chance";      // TrySellToNpc 上架率 0.05(三条调用路径共用)

        // -- 陷阱(TentacleTrap 复刻,1.6.1;默认 = 1.6.1 字面量,见 ConfigRegistry.RegisterTrap) --
        public const string TrapPrice               = "trap.price";                 // CreateManhole 单价 100
        public const string TrapTrapPrice           = "trap.trap_price";            // CreateTrap 单价 200
        public const string TrapBroodSize           = "trap.brood_size";            // 陷阱苗床区 12(不上面板)
        public const string TrapOutputSize          = "trap.output_size";           // 陷阱产出区 12(不上面板)
        public const string TrapTacticChances       = "trap.tactic_chances";        // 三档策略捕捉率 CSV "0.1,0.2,0.3"(不上面板,Task 12 解析)
        public const string TrapSemenChance         = "trap.semen_chance";          // NightlyTickOnce 苗床产精液率 0.5
        public const string TrapNpcSellChanceScav   = "trap.npc_sell_chance_scav";  // 拾荒井盖上架率 0.10(PatchScavGeneralAlarm Postfix)
        public const string TrapNpcSellChanceThief  = "trap.npc_sell_chance_thief"; // 小偷井盖上架率 0.05(PatchThiefHouseholdAlarm Postfix)

        // -- 触手3D打印机(TentaclePrinter 复刻,1.7.1 新增线,Task 14b;默认 = 1.7.1
        //    字面量,见 ConfigRegistry.RegisterPrinter 与 Patches/PrinterPatches.cs)--
        public const string PrinterPrice         = "printer.price";          // CreatePrinter 单价 500(PRINTER_PRICE)
        public const string PrinterNeedlePrice   = "printer.needle_price";   // CreateNeedle 空针管单价 15(NEEDLE_PRICE)
        public const string PrinterGrid          = "printer.grid";           // 三区(材料/苗床/产出)边长 12(GRID;BuildWindow 三 GameGridInventory)
        public const string PrinterRecipeCost    = "printer.recipe_cost";    // 每支空针管所需打印进度 15(RECIPE_COST;进度存储 ×10)
        public const string PrinterBaseProgress  = "printer.base_progress";  // 每苗床每夜基础进度 3(BASE_PROGRESS;DailyProgress)

        // -- 掌上青楼(0.6.0 M5a:lewd_meme_brothel 0.10.4 经济接管,SubModHook 载体;
        //    数值经 FamilyPanelKeys「掌上青楼联动」分区注册,默认 = 反编译 Rules 字面量) --
        public const string BrothelBudgetBonus      = "brothel.budget_bonus";       // 标签预算加成 %(ServiceMath.EffectiveBudget,原版 10)
        public const string BrothelDebtTags         = "brothel.debt_tags";          // 负债线标签数(ServiceMath.SellsGoods/PriceHint+PalmPage.PriceHintLine,原版 5)
        public const string BrothelDebtChance       = "brothel.debt_chance";        // 负债买下概率 %(ServiceMath.SellsGoods,原版 50)
        public const string BrothelGuestsBase       = "brothel.guests_base";        // 预约基础(GuestCapacity.GuestCount,原版 2)
        public const string BrothelGuestsPerUpgrade = "brothel.guests_per_upgrade"; // 每次扩建增量(GuestCount/GuestsAfterUpgrade,原版 2)
        public const string BrothelGuestsMax        = "brothel.guests_max";         // 每日上限(原版 20,消费于 5 处:GuestCount/GuestsAfterUpgrade/BrothelState.Normalize/PalmWindow.EnsurePages/PalmBrothelService.AutoLockAll)
        public const string BrothelTraitsBase       = "brothel.traits_base";        // 基础标签数(TraitCategories.RequirementCount,原版 3)
        public const string BrothelTraitsPerUpgrade = "brothel.traits_per_upgrade"; // 扩建增量(同上,原版隐式 ×1)
        public const string BrothelFeeDefault       = "brothel.fee_default";        // 默认过夜费(MaidenTraits.ResolveFee,原版 120)
        public const string BrothelFeeMax           = "brothel.fee_max";            // 过夜费上限(MaidenTraits.ClampFee/ResolveFee+MaidenMenuUi 提示文本,原版 9999)
        public const string BrothelMasteryThreshold = "brothel.mastery_threshold";  // 精通阈值(MaidenCatalog.TrainOneNight,原版 30)
        public const string BrothelVisitMinDays     = "brothel.visit_min_days";     // 来访最小间隔日(MerchantSchedule.NextVisitDay,原版 4)
        public const string BrothelVisitMaxDays     = "brothel.visit_max_days";     // 来访最大间隔日(同上,原版上界排除 → 实得 4..9,键值 9)
        public const string BrothelPriceMaidDress   = "brothel.price_maid_dress";   // 女仆装售价(BrothelItemCatalog.ItemValue,原版 260)
        public const string BrothelPriceCatEars     = "brothel.price_cat_ears";     // 猫耳售价(同上,原版 180)
        public const string BrothelPriceCatTailPlug = "brothel.price_cat_tail_plug";// 猫尾肛塞售价(同上,原版 220)
        public const string BrothelPriceLaceDress   = "brothel.price_lace_dress";   // 蕾丝裙售价(同上,原版 240)
        public const string BrothelPriceBunnyEars   = "brothel.price_bunny_ears";   // 兔耳售价(同上,原版 175)
        public const string BrothelPriceLaceGarter  = "brothel.price_lace_garter";  // 蕾丝吊袜带售价(同上,原版 150)
        public const string BrothelStockFirst       = "brothel.stock_first";        // 首访上架件数(GearCatalog.RollStock,原版固定 3 件)
        public const string BrothelStockMin         = "brothel.stock_min";          // 后续最小件数(同上,原版 randomRange(3,6) 下界 3)
        public const string BrothelStockMax         = "brothel.stock_max";          // 后续最大件数(同上,原版上界排除 → 实得 3..5,键值 5)

        // -- tearlaments(0.6.0 M5b:dui_mian_lan_qiu_ji_tearl v7 交易接管,SubModHook 载体;
        //    数值经 FamilyPanelKeys「tearlaments 联动」分区注册,默认 = 反编译 tearl_7 字面量。
        //    tearl 无配置系统,常量在消费点全部 const 内联——改 TearlamentsConfig 字段无效)--
        public const string TearlSpawnChance        = "tearl.spawn_chance";         // 特殊顾客出现概率(TearlamentsMod.HandleCustomerSpawn 掷骰,原版 0.3)
        public const string TearlBudgetBase         = "tearl.budget_base";          // 收购预算基础(CustomCustomer.Create SetBudget,原版 150000)
        public const string TearlBudgetRandom       = "tearl.budget_random";        // 收购预算随机增量上限(同上 RNG.GetRandomInt(0,·) 含上界,原版 50000)
        public const string TearlSellDiscount       = "tearl.sell_discount";        // 卖出折扣 %(Create 的 buyPriceModifier = -10,原版 10)
        public const string TearlBuyDiscount        = "tearl.buy_discount";         // 收购少女折扣 %(ApplyBuyPriceFeature,原版 25 → -25)
        public const string TearlWaterPremium       = "tearl.water_premium";        // 收购水溢价 %(同上,原版 +20)

        // -- 暗流交易联动扩展(0.6.0 R4,2026-11-13:lewd_meme_remake 1.5.19 差分数值
        //    接管,SubModHook 载体,TargetMod = remake;数值经 FamilyPanelKeys
        //    「暗流交易联动」分区注册,默认 = remake_1519 反编译字面量[R8 复核
        //    remake_1521 逐字一致]。主 mod 侧对应
        //    单元(alarm/trap/nightly)1.6.1 起共存恒 Own、price_milk 让
        //    Brood——本组接管的是 remake 自家的差分实现,与主 mod 侧让位
        //    互不干扰)--
        public const string RemakeMilkPriceRich          = "remake.milk_price_rich";           // 浓缩乳汁单价(ItemFactory.CreateMilkVariant 工厂 :486,原版 150;夜间产出侧 TentacleBody.TryProduceMilk :1224 以 50×3 同值覆写,两单元共享此键)
        public const string RemakeMilkPriceWondrous      = "remake.milk_price_wondrous";       // 神奇乳汁单价(同上,原版 80 = 50×1.6)
        public const string RemakeMilkPriceDiluted       = "remake.milk_price_diluted";        // 清甜乳汁单价(同上,原版 50 = 50×1)
        public const string RemakeMilkFeedDiluted        = "remake.milk_feed_diluted";         // 清甜乳汁喂奶疲劳缓解(CreateMilkVariant :518-532 switch default 档,原版 20)
        public const string RemakeMilkFeedWondrous       = "remake.milk_feed_wondrous";        // 神奇乳汁喂奶缓解(同 switch milk_wondrous/wondrous_milk 档,原版 30)
        public const string RemakeMilkFeedRich           = "remake.milk_feed_rich";            // 浓缩乳汁喂奶缓解(同 switch milk_rich 档,原版 25)
        public const string RemakeMilkDrinkCredit        = "remake.milk_drink_credit";         // 饮用一瓶乳汁的信用(CreateMilkVariant :514 onActivateSlotItemFunc 内联 AddCredit,原版 30)
        public const string RemakeMilkChanceRichBase     = "remake.milk_chance_rich_base";     // 贫乳→浓缩 产出概率基础(TentacleBody.TryProduceMilk :1214,原版 0.3)
        public const string RemakeMilkChanceRichPerLact  = "remake.milk_chance_rich_per_lactation";   // 同档每级泌乳增量(:1216 的 0.3f 为三档共用常量,拆独立键默认均 0.3)
        public const string RemakeMilkChanceWondrousBase = "remake.milk_chance_wondrous_base"; // 普乳→神奇 概率基础(:1206,原版 0.5)
        public const string RemakeMilkChanceWondrousPerLact = "remake.milk_chance_wondrous_per_lactation"; // 同档每级增量(共用 0.3)
        public const string RemakeMilkChanceDilutedBase  = "remake.milk_chance_diluted_base";  // 巨乳→清甜 概率基础(:1201,原版 0.7)
        public const string RemakeMilkChanceDilutedPerLact = "remake.milk_chance_diluted_per_lactation";   // 同档每级增量(共用 0.3)
        // ST4(2026-11-17)全链废弃:remake.semen_base / remake.semen_train_factor
        // (1.6.1 SemenProduction 类整体删除,精液公式回归主 mod 自家实现——
        // 面板出口 = 主面板 nightly.semen_chance_* 三键)+ remake.alarm_oral/
        // sex/anal_per_night(OriginalDeviceActivityPatch 重写为疲劳门禁,AddAlarm*
        // 三方法消失——主 mod 侧 main.alarm_nightly_once 恢复 Own 即替代出口)。
        public const string RemakeOvulationPrice         = "remake.ovulation_price";           // 排卵诱发剂单价(ItemFactory.ConfigureOvulationShot :576 unitValue=100,0.6.0 R7 等价恢复——主 mod 侧 price.ovulation_shot 对 remake 让位后的接管点,默认 100 = 零漂移)
        // ST 1.6.1 Task 5(2026-11-17):疲劳核心六键(ShadowTrade 1.6.1 疲劳系统
        // 数值面,SubModHook 载体,默认 = MidnightPawn.Game/MaidenFatigue.cs 字面量
        // 零漂移;档位语义名按源码 StateName:≥90 透支 / ≥80 疲惫 / ≥60 疲倦 /
        // <20 休息充足 / 其余正常[1f 硬编码无键];边界 const ExhaustedAt/WearyAt/
        // TiredAt/20 保持原版不可调)
        public const string RemakeFatigueValueMulRested    = "remake.fatigue_value_mul_rested";    // 休息充足档(<20)售价倍率(ValueMul :228,原版 1.1)
        public const string RemakeFatigueValueMulTired     = "remake.fatigue_value_mul_tired";     // 疲倦档(≥60)售价倍率(ValueMul :224,原版 0.94)
        public const string RemakeFatigueValueMulWeary     = "remake.fatigue_value_mul_weary";     // 疲惫档(≥80)售价倍率(ValueMul :220,原版 0.88)
        public const string RemakeFatigueValueMulExhausted = "remake.fatigue_value_mul_exhausted"; // 透支档(≥90)售价倍率(ValueMul :216,原版 0.8)
        public const string RemakeFatigueNightlyGain       = "remake.fatigue_nightly_gain";        // 夜间疲劳基数(NightlyGain :265 weighted×0.5×LevelMul×孕×Scale 的 0.5f,原版 0.5;:260 口交加權 0.5f 是另一常量,不在此键)
        public const string RemakeFatigueExhaustRecover    = "remake.fatigue_exhaust_recover";     // 透支恢复量(RecoverExhausted :124 消费 const ExhaustRecover :28,原版 5)

        // -- 少女生成属性·IPH 分区(0.6.0 R10,2026-11-14:少女生成属性五键复刻入
        //    IPH 分区;主 mod 侧 attr_* 让位 IPH 后的玩家出口,postfix 共存形态——
        //    载体 = main.maiden_attrs_iph 单元[MainHook,TargetMod = IPH:版本绑定
        //    IPH,漂移自动失效;缺席分区不 Define 开关恒 false = Absent 零 hook];
        //    键是本 mod 自有注册表键(非写透,无对方 TargetKey),默认值 = 主 mod
        //    1.7.1 ItemFactory.RollAttr 反编译字面量逐字)--
        public const string IphAttrHighChance = "iph.attr_high_chance";   // 高档概率(RollAttr :507 0.01f;掷骰 value < 该值 → 高档区间)
        public const string IphAttrMidChance  = "iph.attr_mid_chance";    // 中档累计阈值(:511 0.06f;high ≤ value < 该值 → 中档,其余低档)
        public const string IphAttrHighRange  = "iph.attr_high_range";    // 高档区间 CSV(:509 Range(70,81),int 上界排除 = 闭区间 "70,80")
        public const string IphAttrMidRange   = "iph.attr_mid_range";     // 中档区间 CSV(:513 Range(60,71) = "60,70")
        public const string IphAttrLowRange   = "iph.attr_low_range";     // 低档区间 CSV(:515 Range(10,61) = "10,60")

        // -- 巢穴扩展·信息素两键(0.6.0 R12,2026-11-14:Brood 顾客进场时
        //    Fragrance.WeakenBasePerfume 用同一 API 同一槽位把主 mod ±15 信息素
        //    强制压到 ±10——主面板 client.semen_* 两键让位后的玩家出口。载体 =
        //    brood.perfume_mod 单元[SubModHook,postfix on Brood WeakenBasePerfume,
        //    Brood 写完 ±10 后同 API 重设面板值,进场时序恒胜;R10 属性键同模式:
        //    默认 = 主 mod 1.7.1 原字面量,全默认零参与]。槽位配对以反编译为准
        //    (lewd_meme_171 TentacleEffects.cs:78-101/:53-76):MakeSemenFeature →
        //    tentacleSemenInfluence(−15 = price)、MakeBuyBonusFeature →
        //    tentacleSemenBuyBonus(+15 = bonus)——与主面板旧键同一配对) --
        public const string BroodSemenPriceMod = "brood.semen_price_mod"; // 触手精液买价修正 %(→ tentacleSemenInfluence,主 mod 原字面量 -15;负=降价)
        public const string BroodSemenBonusMod = "brood.semen_bonus_mod"; // 触手精液卖价加成 %(→ tentacleSemenBuyBonus,主 mod 原字面量 +15)

        // -- 巢穴扩展·苗床标签拾取器两键(0.7.1,2026-11-18:Brood 标签编辑器的
        //    「选中」方式替换——红灯区放大镜链断裂[红灯区非家族前置 + 主 mod
        //    苗床区拒 magnifier 交互],改为悬停苗床 + 快捷键直调 Brood
        //    MaidenTagEditor.Remember(brood_0331 MaidenTagEditor.cs:26-34,
        //    internal static 单一签名);其余全走 Brood(编辑窗/标签语义/审计
        //    零改动)。载体 = brood.tag_picker 单元[SubModHook 形态但无 Harmony
        //    patch——宿主生命周期单元,见 HookCatalog 注释]--
        public const string BroodTagPickerEnabled = "brood.tag_picker_enabled"; // 拾取器开关(Toggle 自开关:SubModHook Toggle 经 SwitchDispatch 表驱动 bool 自身,不派生 .enabled;默认 false = 零宿主零轮询)
        public const string BroodTagPickerKey = "brood.tag_picker_key";         // 拾取快捷键(KeyCode 名串,默认 "U" = KeyCode.U,全家族+游戏绑定零冲突;面板走游戏原生 DefineEnum<KeyCode>[Brood 热键先例 Settings.cs:130],注册表层存名串、宿主 Update 热读 Enum.TryParse 即时生效)
    }
}
