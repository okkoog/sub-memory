using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 载体种类(spec §5.5,按需装配):MainHook = Harmony 补丁接管(prefix/postfix,
    /// 装配期解析目标类型);DataWrite = 装配时写入 + Changed 驱动重写的主 mod 静态
    /// 字段/数组动作(无补丁);WriteThrough = 子 mod 官方 ModConfig 写透(M4b,
    /// 无 Harmony——经 WriteThroughCarrier 写对方 ConfigEntry,Router 恒 Own);
    /// SubModHook = 子 mod 方法接管(M5a brothel:对方 const 内联、无配置系统,
    /// prefix 复刻其方法体,装配路径与 MainHook 相同——HookRuntime R-N)。
    /// 让位(remake/IPH/兼容层)是 Router 裁决,不是载体属性。
    /// </summary>
    public enum CarrierKind
    {
        MainHook,
        DataWrite,
        SubModHook,
        WriteThrough,
    }

    /// <summary>
    /// 钩子组定义行(纯数据,无 Install 委托——Task 4 HookRuntime 从各 patch 类的
    /// hub.Add 条目挂装)。Id 采用目录自有命名空间:main.&lt;snake&gt; = 主 mod 家族
    /// 补丁条目、pools.&lt;x&gt; = 数据写入条目、game.&lt;x&gt; = 游戏本体补丁条目
    /// (与 hub 条目 id 的映射归 Task 4)。Keys = 该条目 patch 方法体引用的全部
    /// CfgKeys 成员去重(条件读取也计入;不含面板子项开关键——开关经原键派生);
    /// TargetMod = 版本压制查询键("lewd_meme" = 主 mod 家族单元;null = 游戏本体
    /// 单元,不参与压制)。
    /// </summary>
    public sealed class HookUnitDef
    {
        public string Id;
        public CarrierKind Kind;
        public string[] Keys;
        public string TargetMod;
    }

    /// <summary>
    /// 装配单元 ↔ 配置键位表(M2b,纯数据;唯一事实源 = Source/Patches/*.cs 各
    /// installs.Add 条目的 patch 方法体引用的 CfgKeys 成员)。14 个 patch 类共
    /// 57 个 installs.Add 调用 = 57 个逻辑条目(45 MainHook + 12 SubModHook
    /// brothel 9 + tearl 3;让位豁免分支零产出,见 HookRuntime R5 守卫);另
    /// 3 个 DataWrite 单元(初代 pools.maiden / pools.trap_tactic /
    /// pools.guest_display;T9 后 DataWrite = jobs/fbb/trap_tactic/guest_display
    /// 4 单元)无 install 委托——DataWrite 四单元同为「装配时写主 mod 静态 +
    /// 订阅 Changed 重写、无 Harmony 补丁」形态,不进安装器;其中 2 个
    /// game.* 单元为游戏本体补丁(TargetMod=null,主 mod 缺席也装)。M4b 追加
    /// 19 个 WriteThrough 单元(总 66,时点 MainHook 44——Task 14b +printer
    /// 后 45):每镜像键一单元,TargetMod = 对方 mod(FamilyMods 常量),
    /// 不进安装器(无 Harmony),由 HookRuntime 分发器直转写透载体。M5a 追加
    /// 9 个 SubModHook 单元(总 75):brothel.* 经济接管,TargetMod =
    /// FamilyMods.Brothel,装配路径与 MainHook 相同(HookRuntime R-N),
    /// install 委托 = BrothelPatches。M5b 追加 3 个 tearl.* 单元(总 78):
    /// tearlaments 交易接管,TargetMod = FamilyMods.Tearl,install 委托 =
    /// TearlPatches;CustomCustomer.Create 为 tearl.budget 与 tearl.trade
    /// 两单元共享方法(方法级引用计数裁决物理卸载)。Task 14b 追加 1 个
    /// main.printer 单元(总 79):触手3D打印机五常量接管(主 mod 1.7.1 新增
    /// 线,install 委托 = PrinterPatches,五键共用一单元、七接管点)。
    /// 0.6.0 R4(2026-11-13)追加 4 个 remake.* 单元(总 83):魔改版 1.5.19
    /// 差分数值接管(乳汁差分工厂/夜间产出概率/精液公式/闹钟计数),TargetMod
    /// = FamilyMods.Remake,install 委托 = RemakePatches[R13a:绑定 bump
    /// 1.5.24,接管点零漂移];乳汁单价三键由
    /// remake.milk_items(目录工厂)与 remake.milk_production(夜间覆写)双单元
    /// 共享(LevelCap 双单元先例;两处字面量同值 150/80/50)。
    /// 0.6.0 R7(2026-11-13,让位方法级精审 + 等价恢复)追加 3 单元(总 86):
    /// main.loot_maiden_chance(MainHook——拾荒少女概率 getter 接管,与
    /// main.on_loot_rolled 双消费 loot.scav_maiden_base,键级聚合下 remake 在场
    /// 时经本单元生效)、pools.maiden_traits(DataWrite——四池静态写自 pools.maiden
    /// 拆出,Brood 只覆写 EXP_NEED 故四池恒 Own)、remake.ovulation_price
    /// (SubModHook——postfix on remake ConfigureOvulationShot,主 mod 侧
    /// price_ovulation_shot 让位后的等价覆盖点)。
    /// 0.6.0 R9(2026-11-14,IPH 语义修正 + 写透分区)两处增补(总 103):
    /// ① pools.maiden_traits 拆为 pools.maiden_jobs(JOBS,恒 Own——IPH 工厂
    /// transpiler 零触碰职业抽取)+ pools.maiden_fbb(FACTIONS/BODIES/BREASTS,
    /// 让位 IPH——0x0c 实测其抽取替换接管三池,见 Router R4;DataWrite 4→5);
    /// ② IPH 1.2.17 官方 ModConfig 16 键写透镜像(wt.iph.*,每键一单元,
    /// WriteThrough 19→35)。
    /// 0.6.0 R10(2026-11-14,少女生成属性键复刻到 IPH 分区)追加 1 单元
    /// (总 104):main.maiden_attrs_iph(MainHook,**TargetMod = FamilyMods.Iph**
    /// ——首个非 lewd_meme 目标的 MainHook 单元:版本绑定 IPH,在场且匹配 →
    /// Own 装配,漂移 → VersionBlocked 自动失效,缺席 → 分区不 Define 开关恒
    /// false = Absent 零 hook;postfix on 主 mod ItemFactory.CreateMaidenOfFaction,
    /// 与 IPH 工厂 transpiler 共存——R9 让位原则的例外形态,install 委托 =
    /// MaidenAttrIphPatch,五键共用一单元)。
    /// 0.6.0 R12(2026-11-14,Brood 信息素两键等价恢复)追加 1 单元(总 105):
    /// brood.perfume_mod(SubModHook,TargetMod = FamilyMods.Brood——postfix on
    /// Brood Fragrance.WeakenBasePerfume,顾客进场时其在主 mod ±15 信息素上
    /// 强制 ±10(同 API 同槽位时序后置恒胜)后,同 API 重设面板值;R10 属性键
    /// 同模式:默认 = 主 mod 原字面量 -15/+15,全默认零参与;install 委托 =
    /// BroodPatches,两键共用一单元;主面板旧键 client.semen_* 的 Brood 让位
    /// 行为零改动)。
    /// 0.7.0 Task 6(2026-11-17):main.on_semen_used 复刻体退役 → transpiler 单元
    /// (id 不变,与 TranspilePlan 表内 methodId 同名;Keys 3 → 2,单元改形态不增删)。
    /// 0.7.0 Task 7(2026-11-17):Create 系复刻体退役——alarm/trap/printer/
    /// layout 十单元改形态不增删(总 106 / MainHook 48 不变):价格六键迁
    /// postfix(Create* 尾 ApplyCsv 之后恒胜,方法体集中进 ValuePostfixPatches)、
    /// 数值键改 transpiler(methodId 与单元 id 同名对齐,printer 三 methodId
    /// 保持 T3 名);alarm_nightly_once Keys 4 → 3(semen_output → DataOnlyKeys)、
    /// main.create Keys 5 → 1(布局四键归 build_window)。
    /// 0.7.0 Task 8(2026-11-17):create_maiden 拆段(总 108 / MainHook 50)——
    /// 数值段 attr 五键 → main.maiden_roll、faction_base → main.maiden_faction_base
    /// 两新 transpiler 单元(1.9.2 拆出 ItemFactory.RollAttr/FactionBase 独立方法,
    /// IPH 在场 Own 共存——工厂 transpiler 零触碰两方法,让位表零新行);
    /// 两池段复刻体 Keys 8 → 1(仅 portrait_size),复刻体对齐 1.9.2(无条件处女
    /// Enable + 6 新属性 SetInt);virgin_chance 键全链废弃(面板值键 90 → 89);
    /// main.produce_daughter 复刻体退役 → transpiler(单元 id 不变,methodId 保持
    /// main.produce_daughter_value)。
    /// 0.7.0 Task 9(2026-11-17):A 组真新增 + 经验公式化(总 110 / MainHook 53 /
    /// DataWrite 4)——新增 main.interact_threshold(prefix 复刻
    /// ItemFactory.ComputeInteractMax 四阈值 ≥60 单键,默认 60 放行零漂移)、
    /// main.exp_formula(prefix 复刻 TentacleBody.ExpNeedForLevel 公式三键
    /// [progress.exp_base/divisor/step,默认 100/3/100 = 原版逐字,求值经
    /// Logic/ExpCurve])、main.initial_virgin(postfix on CreateMaidenOfFaction,
    /// 值 false 时 Disable VIRGIN/ANAL_VIRGIN 双 tag;Toggle 自开关反向分派,
    /// 默认 true = 零参与);删除 pools.maiden(DataWrite——1.9.2 已删
    /// TentacleBody.EXP_NEED 静态数组,该单元在 1.9.2 下 ModReflect 取字段必抛
    /// → 装配期 TryDataWrite 拦截跳过(仅记日志,不阻断装配),删除即解决;
    /// Router 让位表 R2 行随之撤销);main.add_body_exp
    /// 复刻体退役 → 改参 prefix(仅 ExpMultiplier 单键:经验公式由
    /// main.exp_formula 接管,升级循环归原方法体[1.9.2 无 cap 条件,10000 次
    /// 防死循环],level_cap 键归 main.level_cap 读 clamp 单元)Keys 3 → 1;
    /// progress.exp_per_level 键全链废弃(面板值键 89 → 93;PoolRouting/
    /// DebugPanel/自检目录联动收缩)。
    /// ST 1.6.1 Task 4(2026-11-17):两单元注销(SubModHook
    /// 18 → 16;单元总数 110 → 109——WriteThrough 净 +1,见下)——
    /// remake.semen_formula(SemenProduction 类整体删除;其第三键
    /// nightly.semen_chance_cap 的跨族等价消费[R7]终结,该键唯一消费回归
    /// main.nightly_transpile,注入点 IL_02c8 本就在 TranspilePlan 该 methodId
    /// 表内,归属迁移 = 删行即成)+ remake.alarm_activity(AddAlarm* 三方法随
    /// 疲劳门禁重写消失;main.alarm_nightly_once 恢复 Own 即替代出口);5 个
    /// remake.* 差分键全链废弃(注册表/镜像面板/目录三面零残留);写透 remake
    /// 7 −3+4 = 8(撤 product_chance/ovulation_guaranteed/virgin_avoidance
    /// ——对方 1.6.1 已删,旧 json 残留由写透键存在性守卫无害化;增桶装
    /// auto_barrel_semen/milk_wondrous/milk_rich/milk_diluted 四键,默认"开"),
    /// WriteThrough 35 → 36——单元总数 110 −2 +1 = 109(每镜像键一单元原则下
    /// 写透 −3+4 净 +1;brief 的「110−2=108」漏算此项)。
    /// ST 1.6.1 Task 5(2026-11-17):疲劳核心两单元(总 111 / SubModHook
    /// 16 → 18)——remake.fatigue_value_mul(ValueMul 售价四档 prefix)+
    /// remake.fatigue_nightly_recover(NightlyGain 夜间基数 + RecoverExhausted
    /// 透支恢复双 prefix);六键默认 = 1.6.1 源码字面量零漂移。
    /// 0.7.1 拾取器(2026-11-18):+1 单元 brood.tag_picker(总 112 /
    /// SubModHook 18 → 19)——Brood 苗床标签编辑器「选中」方式替换,无
    /// Harmony patch 的宿主生命周期单元(Kind 选择理由见该单元行内注释)。
    /// 排序 = patch 类文件序、类内 Register() 序。off-panel 的 9 个布局尺寸键
    /// (layout.* 5 + alarm 2 + trap 2)是注册表键,按消费条目归入对应单元的
    /// Keys(如 main.build_window 读 layout 四键)。面板 90 个值键全部有消费单元
    /// ——DataOnlyKeys 为空(brief 猜测的 names.use_custom_file 经核实由
    /// names.custom 条目消费,非排除项;Task 14b +printer 5)。一致性由
    /// HookCatalogTests 四条锁定。
    /// </summary>
    public static class HookCatalog
    {
        public static readonly IReadOnlyList<HookUnitDef> Units = new[]
        {
            // ---- AlarmPatch(0.7.0 T7:Create 系复刻体退役——价格 postfix +
            //      数值 transpile,四单元 id/计数不变仅形态换)----
            Def("main.create_alarm", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.AlarmPrice),                                           // AlarmPricePostfix(Create* 尾 ApplyCsv 之后恒胜;1.9.2 新类型行/ApplyCsv 随原方法自动存在)
            Def("main.create_tentacle_alarm", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.AlarmTentaclePrice, CfgKeys.AlarmBroodSize, CfgKeys.AlarmOutputSize),
                                                                                    // 价格 postfix(CreateTentacleAlarm)+ BuildWindow transpiler 双接管;brood/output 布局键搭价格开关的车(0.6.0 语义)
            Def("main.alarm_nightly_once", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.AlarmOralPerNight, CfgKeys.AlarmSexPerNight,
                CfgKeys.AlarmAnalPerNight),                                   // TranspileCarrier(NightlyTickOnce 三 AddX 第二参;Keys ≡ TranspilePlan.Points 键集,由 HookCatalogTests 同源互验锁定;semen_output 1.9.2 产出无条件 → DataOnlyKeys,面板行保留)
            Def("main.alarm_try_sell_npc", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.AlarmNpcSellChance),                                   // TranspileCarrier(TrySellToNpc 掷骰阈值 ldc.r4 0.05;三条调用路径共用此方法,remake 让位语义随本单元[Router R1])

            // ---- CorruptPatch(CorruptTentacle 接管 ×8;含 Tolerant postfix initial_charge;
            //      0.7.0 T5:main.try_corrupt 复刻体退役 → main.corrupt_cost
            //      transpiler 单元,id 与 TranspilePlan 表内 methodId 同名——
            //      载体形态换(六点 ldc → getter),单元数不变;
            //      0.7.0 T6:main.on_semen_used 复刻体退役 → transpiler 单元,
            //      id 不变(与表内 methodId 同名,Carrier.Run 的 route 过滤据此
            //      对齐),Keys 3 → 2——daily 移出(复刻体内联的文案消费随
            //      复刻体退役,原方法体 RefreshTooltip 调用归 refresh_tooltip
            //      单元),max/semen ≡ 注入点键集(semen 1 点 + max_charge 2 点)----
            Def("main.get_charge", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptMaxCharge),                                     // GetChargePrefix
            Def("main.set_charge", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptMaxCharge),                                     // SetChargePrefix
            Def("main.refresh_tooltip", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptMaxCharge, CfgKeys.CorruptDaily, CfgKeys.CorruptSemen),
            Def("main.daily_recharge", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptMaxCharge, CfgKeys.CorruptDaily, CfgKeys.CorruptSemen),
            Def("main.on_semen_used", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptMaxCharge, CfgKeys.CorruptSemen),               // TranspileCarrier(OnSemenUsed 三点:semen_recharge Min 首参 1 + max_charge 门槛 blt/截断 sub 2;Keys ≡ TranspilePlan.Points("main.on_semen_used") 键集,由 HookCatalogTests 同源互验锁定;双参签名 + usedSemen 按瓶去重归原方法零复刻)
            Def("main.is_corruptible", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptWhitelist, CfgKeys.CorruptBlacklist),           // IsCorruptiblePrefix(白/黑名单行为型键的唯一载体;T5 增 1.9.2 FindCorruptRule 透传行)
            Def("main.corrupt_cost", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptCost),                                          // TranspileCarrier(TryCorrupt 六点:门槛 bge 1 + 扣减 sub/SetCharge 5;Keys ≡ TranspilePlan.Points("main.corrupt_cost") 键集,由 HookCatalogTests 同源互验锁定;whitelist/blacklist 不随迁——T2 C 节裁决为行为型,载体 = main.is_corruptible 复刻体)
            Def("main.initial_charge", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptInitialCharge, CfgKeys.CorruptMaxCharge),       // InitialChargePostfix(Create postfix)

            // ---- GameGenderPatches(游戏本体 GameOnly ×2;TargetMod=null)----
            Def("game.pickgendered", CarrierKind.MainHook, null,
                CfgKeys.GenderScope),                                          // PickGenderedPrefix(Scope 属性读)
            Def("game.sprite_name", CarrierKind.MainHook, null,
                CfgKeys.GenderScope),                                          // GetGenderFromSpriteNamePostfix

            // ---- GenderRatioPatch(主 mod GenderRatio.OnGenerated 接管 ×1)----
            Def("main.on_generated", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.GenderScope, CfgKeys.FemaleRewriteChance),             // OnGeneratedPrefix

            // ---- LevelCapPatch(TentacleBody.GetBodyLevel / ExpNeedForLevel 接管 ×2;
            //      0.7.0 T9:+main.exp_formula 公式三键——1.9.2 经验表 EXP_NEED
            //      静态数组已删,公式化进 ExpNeedForLevel[public static,
            //      :974-981],prefix 复刻体默认放行零漂移)----
            Def("main.level_cap", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.LevelCap),                                             // GetBodyLevelPrefix(brief 范例行;1.9.2 升级循环无 cap 拦截,读 clamp 封顶)
            Def("main.exp_formula", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.ExpBase, CfgKeys.ExpDivisor, CfgKeys.ExpStep),        // ExpNeedForLevelPrefix(公式三键:base+(lv-1)/divisor*step 整除截断;默认 100/3/100 放行原版;AddBodyExp/GetExpNeed/DebugAddExp 全调用方生效)

            // ---- LootPatch(LootHook.OnLootRolled 接管 ×1 + MaidenChance getter 接管 ×1)----
            Def("main.on_loot_rolled", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.MedKeywords, CfgKeys.MedShotChance, CfgKeys.ScavGroups,
                CfgKeys.ScavMaidenBase, CfgKeys.LootThrottleSeconds),          // LootCfg.Read + Body 内联节流键
            Def("main.loot_maiden_chance", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.ScavMaidenBase),                                      // MaidenChancePrefix(0.6.0 R7 等价恢复:prefix on ModSettings.get_MaidenChance;remake 在场[OnLootRolled 让位、概率路径保留]与主 mod 单装两态都经 getter 生效;与 on_loot_rolled 双消费同键——键级聚合下任一 Own 即生效)

            // ---- MaidenPatch(少女工厂接管 ×2 + 数值 transpiler ×2 + 池写 DataWrite ×3;
            //      0.7.0 T8:create_maiden 拆段——attr 五键迁 main.maiden_roll、
            //      faction_base 迁 main.maiden_faction_base(1.9.2 拆出的
            //      ItemFactory.RollAttr/FactionBase 独立方法,transpiler 注入点
            //      TranspilePlan E 节);两池段复刻体 Keys 8 → 1(仅 portrait_size),
            //      复刻体对齐 1.9.2 行为:无条件处女 Enable 替代掷骰段
            //      [virgin_chance 键废弃] + 6 新属性 SetInt[情感/信任/催眠/交互×2/烙印],
            //      属性三掷/基价改经委托直调主 mod RollAttr/FactionBase/AttrValue
            //      [transpiler 联动:开关开 = 面板键生效,关 = 原版字面量])。
            //      两 transpiler 单元 IPH 在场仍 Own——其工厂 transpiler 只挂
            //      CreateMaidenOfFaction(iph_130 OriginalCompat.cs:58/:102),
            //      RollAttr/FactionBase 零触碰,与 R10 postfix 共存先例同构,
            //      让位表零新行)----
            Def("main.create_maiden", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PortraitSize),                                         // 池段复刻体(仅池抽取 + portrait_size;四池经 PoolField 注册表直读优先,静态回退;IPH 在场让位不变[R4])
            Def("main.create_maiden_of_faction", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PortraitSize),                                         // 同上(两 prefix 共用复刻体;1.9.2 ApplyCsv 核对:CreateMaiden 系不在消费清单,无尾补)
            Def("main.maiden_roll", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.AttrHighChance, CfgKeys.AttrMidChance,
                CfgKeys.AttrHighRange, CfgKeys.AttrMidRange, CfgKeys.AttrLowRange),
                                                                                     // TranspileCarrier(RollAttr:三档概率 2 点 ldc.r4 + 三区间 6 点;Keys ≡ TranspilePlan.Points("main.maiden_roll") 键集,由 HookCatalogTests 同源互验锁定)
            Def("main.maiden_faction_base", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.FactionBase),                                          // TranspileCarrier(FactionBase 三 case 常量:上层区 100/治安部·黑市·革命军合流 50/其他 10;Keys ≡ Points 键集)
            // ---- MaidenInteractPatch(0.7.0 T9 新增:少女互动阈值 prefix 复刻 +
            //      初始处女 postfix,两键各一单元,恒 Own 无让位——家族 mod 零触碰
            //      ComputeInteractMax 的四阈值读段与 CreateMaidenOfFaction 的
            //      处女 Enable 段)----
            Def("main.interact_threshold", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.InteractThreshold),                                    // ComputeInteractMaxPrefix(互动次数上限 = 1 + 情感/信任/催眠/调教达阈值项数;四阈值 ≥60 同源单键,默认 60 放行零漂移)
            Def("main.initial_virgin", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.InitialVirgin),                                        // CreateMaidenOfFactionPostfix(1.9.2 无条件 Enable VIRGIN/ANAL_VIRGIN;值 false → DisableTag 双 tag;Toggle 自开关反向分派[SwitchDispatch T9 条],默认 true = 零参与;与池段复刻体[R4 让位]/R10 attrs postfix 三 postfix 叠加,顺序无冲突语义)
            Def("pools.maiden_jobs", CarrierKind.DataWrite, "lewd_meme",
                CfgKeys.Jobs),                                                 // WriteJobsAtCompose(JOBS 静态写;R9 拆分:IPH 工厂 transpiler 全文零触碰职业抽取[iph_1217 起,iph_130 复核:无 MaidenTraits.JOBS 引用],恒 Own)
            Def("pools.maiden_fbb", CarrierKind.DataWrite, "lewd_meme",
                CfgKeys.Factions, CfgKeys.Bodies,
                CfgKeys.Breasts),                                             // WriteFbbAtCompose(FACTIONS/BODIES/BREASTS 静态写;R9 拆分自 R7 的 pools.maiden_traits:IPH 在场让位——其工厂 transpiler 无条件把体型/乳量 Pick 换成自家抽取[iph_130 OriginalCompat.cs:282/:320,num7/num8 校验 :334 不分兼容模式],阵营默认模式走其权重表[RandomPortraits.PickFaction :289];让位细节见 Router R4)

            // ---- MaidenAttrIphPatch(少女生成属性·IPH 分区复刻,0.6.0 R10)----
            Def("main.maiden_attrs_iph", CarrierKind.MainHook, FamilyMods.Iph,
                CfgKeys.IphAttrHighChance, CfgKeys.IphAttrMidChance,
                CfgKeys.IphAttrHighRange, CfgKeys.IphAttrMidRange,
                CfgKeys.IphAttrLowRange),                                     // CreateMaidenOfFactionPostfix(postfix 与 IPH 工厂 transpiler 共存:iph_130 六处调用替换全在选图/阵营/体型/乳量段,属性掷骰段零引用——R9 让位原则的例外形态;TargetMod = IPH:在场且版本匹配 Own → 装,漂移 VersionBlocked 自动失效[用户裁定],缺席分区不 Define 开关恒 false = Absent 零 hook;全默认零参与不重掷)

            // ---- NightlyPatch(TentacleBody 接管 ×5 + NightlyTick transpile ×2;
            //      0.7.0 T4:main.nightly_tick 复刻体退役,数值面/乳汁面拆两个
            //      transpiler 单元——让位语义不同[乳汁面让 Brood;1.6.1 后夜间
            //      面单元恒 Own,ShadowTrade 插入式共存]故不共单元;
            //      min 系 5 键 1.9.2 IL 折叠无消费点 → DataOnlyKeys,面板行保留;
            //      0.7.0 T7:build_window/create 两布局复刻体退役 → transpiler,
            //      两单元键全为不上面板的布局键 → 开关恒 false 恒不装
            //      [SwitchDispatch 四条分派,与 0.6.0「键不上面板 = 复刻体
            //      读默认恒 no-op」语义等价];produce_daughter 归 Task 8)----
            Def("main.nightly_transpile", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.SexMinBase, CfgKeys.SexMaxBase, CfgKeys.SexPerLevelMax,
                CfgKeys.OralMaxBase, CfgKeys.OralPerLevelMax,
                CfgKeys.AnalMaxBase, CfgKeys.AnalPerLevelMax,
                CfgKeys.OvulateChance, CfgKeys.ConceivePerSex, CfgKeys.GestationDays,
                CfgKeys.LactationCap, CfgKeys.SemenChanceBase, CfgKeys.SemenChancePerTrain,
                CfgKeys.SemenChanceCap),                        // TranspileCarrier(数值面 14 键;Keys ≡ TranspilePlan.Points("main.nightly_transpile") 键集,由 HookCatalogTests 同源互验锁定)
            Def("main.nightly_milk_value", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.MilkValueBase, CfgKeys.MilkValuePerLv), // TranspileCarrier(乳汁面 2 键;与数值面共享 NightlyTick original——替换点不相交,叠加安全;让位 {Brood}[1.6.1:remake 撤让,原版乳汁消费点复活]与数值面分流)
            Def("main.add_body_exp", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.ExpMultiplier),                                       // AddBodyExpPrefix(T9 复刻体退役 → 改参 prefix:amount×multiplier 后放行原方法,默认 1 = 零漂移;升级循环归原方法体[公式经 main.exp_formula 接管],level_cap 键归 main.level_cap)
            Def("main.build_window", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.BroodSize, CfgKeys.OutputSize, CfgKeys.CorruptCols, CfgKeys.ToolCols),
                                                                                    // TranspileCarrier(T7 退役 BuildWindowPrefix/Body;四键 ≡ Points("main.build_window") 键集[含 brood/output ldloc 特形],由 HookCatalogTests 同源互验锁定)
            Def("main.create", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TabsPerLevel),                                         // TranspileCarrier(T7 退役 CreatePrefix/Body;Keys 5 → 1——布局四键归 build_window,原版 Create 调原版 BuildWindow 过 Harmony 链,布局注入对 Create 路径照常生效)
            Def("main.produce_daughter", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.DaughterValue),                                        // TranspileCarrier(0.7.0 T8 复刻体退役——methodId 保持 main.produce_daughter_value[与单元 id 不同名,printer 先例];1.9.2 行为零复刻自动在场:MAIDEN_DAUGHTER tag/NameGen 姓氏继承/unitValue 900 全在原方法体;1.6.1 撤让恒 Own——ShadowTrade 只插 NightlyTick 不碰 ProduceDaughter,原版产女调用点与方法体复活)
            Def("main.night_report", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.UiReportDelay, CfgKeys.UiReportWidth, CfgKeys.UiReportHeight), // Prefix + Delay 回调 ShowReportWindow

            // ---- SpecialAndManualPatches(特殊顾客/驯服文案/名字库/说明书/读档修复 ×5)----
            Def("main.union", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.SpecialExtraIds, CfgKeys.SpecialExtraSprites, CfgKeys.SpecialExtraKeywords),
            Def("main.tame_text", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TrainTameThreshold),                                   // UpdateMaidenDescriptionPrefix + OvulationShotTamePostfix
            Def("main.names_custom", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.NamesUseCustomFile),                                   // EnsureLoadedPrefix
            Def("main.manual_render", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.CorruptCols, CfgKeys.BroodSize, CfgKeys.OutputSize, CfgKeys.ToolCols,
                CfgKeys.SexMinBase, CfgKeys.SexMaxBase,
                CfgKeys.OralMinBase, CfgKeys.OralMaxBase,
                CfgKeys.AnalMinBase, CfgKeys.AnalMaxBase,
                CfgKeys.SemenChanceBase, CfgKeys.MilkValueBase, CfgKeys.MilkValuePerLv,
                CfgKeys.OvulateChance, CfgKeys.GestationDays,
                CfgKeys.CorruptDaily, CfgKeys.CorruptSemen, CfgKeys.CorruptMaxCharge), // ManualKeys(18)
            Def("main.reload_fix", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.UiReloadFixDelay),                                     // DelayRepairPrefix

            // ---- TrapPatch(0.7.0 T7:Create 系复刻体退役,五单元形态换;
            //      策略数组写 DataWrite 不变 + 电源按钮验证见任务报告:
            //      1.9.2 BuildWindow 新增 powerBtn,复刻体退役后原方法跑,
            //      按钮 UI 自动在场)----
            Def("main.create_manhole", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TrapPrice),                                            // TrapPricePostfix(井盖单价;ApplyCsv 尾调之后恒胜)
            Def("main.create_trap", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TrapTrapPrice, CfgKeys.TrapBroodSize, CfgKeys.TrapOutputSize),
                                                                                    // 价格 postfix(CreateTrap)+ BuildWindow transpiler 双接管(alarm 先例;TRAP_ENABLED 写入归原方法)
            Def("main.trap_nightly_once", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TrapSemenChance, CfgKeys.TrapTacticChances),           // TranspileCarrier(NightlyTickOnce:tactic 整序列替换 + semen_chance;Keys ≡ Points 键集;1.6.1 撤让恒 Own——ShadowTrade 设备 transpiler 只替换 IsTag/追加 Complete,注入点无扰动[推演表 20/21])
            Def("main.trap_sell_scav", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TrapNpcSellChanceScav),                                // TranspileCarrier(PatchScavGeneralAlarm::Postfix 的 ldc.r4 0.1;原 Postfix 体照跑,无 prefix 防双跑需求)
            Def("main.trap_sell_thief", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.TrapNpcSellChanceThief),                               // TranspileCarrier(PatchThiefHouseholdAlarm::Postfix 同形)
            Def("pools.trap_tactic", CarrierKind.DataWrite, "lewd_meme",
                CfgKeys.TrapTacticChances),                                    // WriteTacticChances(TACTIC_CHANCES 就地覆写)

            // ---- ValuePostfixPatches(价格/精液三值/池上限/客人格/天赋 ×10)----
            Def("main.price_milk", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PriceMilk),                                            // MilkPricePostfix
            Def("main.price_milk_credit", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PriceMilkCredit),                                      // MilkCreditPostfix
            Def("main.price_semen", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PriceSemen),                                           // SemenPricePostfix
            Def("main.price_tentacle_box", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PriceTentacleBox),                                     // TentacleBoxPricePostfix
            Def("main.price_ovulation_shot", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PriceOvulationShot),                                   // OvulationShotPricePostfix
            Def("main.semen_effects", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.SemenPriceMod, CfgKeys.SemenBonusMod, CfgKeys.SemenRepMult), // Make*/TryApply 三 postfix
            Def("main.pool_size", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.ClientPoolSize),                                       // ClientPoolResolvePrefix
            Def("pools.guest_display", CarrierKind.DataWrite, "lewd_meme",
                CfgKeys.GuestDisplayEnabled),                                  // WriteGuestDisplay(GuestBox.DisplayEnabled 字段写,无补丁)
            Def("main.perk_scav_bonus", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PerkScavBonus),                                        // GetMaxScavAttemptsPostfix(游戏类型目标,主 mod 天赋门控)
            Def("main.perk_daily_females", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PerkDailyFemales),                                     // OnNewDayPrefix + OnNewDayPostfix

            // ---- WriteThrough(子 mod 官方 ModConfig 写透,M4b;每镜像键一单元,
            //      无 Harmony——不进安装器,HookRuntime 分发器直转写透载体;TargetMod
            //      = 对方 mod(FamilyMods.Remake/Brood),在场性由面板条件 Define 把关,
            //      Router 恒 Own(R-F 写透豁免)。ST4:remake 撤 3 键
            //      (product_chance/ovulation_guaranteed/virgin_avoidance——1.6.1
            //      对方已删,旧 json 残留由键存在性守卫无害化)+ 增桶装 4 键
            //      (auto_barrel_*,默认"开"),remake 7 → 8 ----
            Def("wt.remake.fatigue_enabled", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.fatigue_enabled"),
            Def("wt.remake.fatigue_scale", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.fatigue_scale"),
            Def("wt.remake.buyer_per_day", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.buyer_per_day"),
            Def("wt.remake.corruption_box_settlement", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.corruption_box_settlement"),
            Def("wt.remake.auto_barrel_semen", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.auto_barrel_semen"),
            Def("wt.remake.auto_barrel_milk_wondrous", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.auto_barrel_milk_wondrous"),
            Def("wt.remake.auto_barrel_milk_rich", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.auto_barrel_milk_rich"),
            Def("wt.remake.auto_barrel_milk_diluted", CarrierKind.WriteThrough, FamilyMods.Remake,
                "remake.auto_barrel_milk_diluted"),
            Def("wt.brood.eggBaseChance", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.eggBaseChance"),
            Def("wt.brood.humanToEggChance", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.humanToEggChance"),
            Def("wt.brood.whiteGrowChance", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.whiteGrowChance"),
            Def("wt.brood.whiteBodyChance", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.whiteBodyChance"),
            Def("wt.brood.infectMajor", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.infectMajor"),
            Def("wt.brood.infectMinor", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.infectMinor"),
            Def("wt.brood.infectNone", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.infectNone"),
            Def("wt.brood.tentacleSpeechFrequency", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.tentacleSpeechFrequency"),
            Def("wt.brood.tentacleSpeechSpeed", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.tentacleSpeechSpeed"),
            Def("wt.brood.tentacleSpeechHold", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.tentacleSpeechHold"),
            Def("wt.brood.disableRampageEnding", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.disableRampageEnding"),
            Def("wt.brood.disableDevourFlash", CarrierKind.WriteThrough, FamilyMods.Brood,
                "brood.disableDevourFlash"),

            // ---- WriteThrough·IPH(0.6.0 R9,lewd_meme_config 面板镜像 IPH 1.2.17
            //      官方 ModConfig 16 键——17 键扣 hotkey(KeyCode 枚举,Brood 热键
            //      先例)[R13a:bump 1.2.18,17 键零漂移];每镜像键一单元,同 wt.remake/wt.brood 语义:无 Harmony、
            //      不进安装器,HookRuntime 分发器直转写透载体,Router 恒 Own
            //      (R-F 写透豁免:在场性由面板条件 Define 把关,版本治理 = 键
            //      存在性守卫;IPH 换键名时自动 no-op + 日志)----
            Def("wt.iph.original_portrait_selection", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.original_portrait_selection"),
            Def("wt.iph.final_portrait_selection", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.final_portrait_selection"),
            Def("wt.iph.resolution", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.resolution"),
            Def("wt.iph.guest_portrait_hd", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.guest_portrait_hd"),
            Def("wt.iph.keep_guest_names", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.keep_guest_names"),
            Def("wt.iph.keep_mod_guest_names", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.keep_mod_guest_names"),
            Def("wt.iph.name_region", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.name_region"),
            Def("wt.iph.local_portrait_chance", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.local_portrait_chance"),
            Def("wt.iph.weighted_capture_factions", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.weighted_capture_factions"),
            Def("wt.iph.trace_portrait_replacements", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.trace_portrait_replacements"),
            Def("wt.iph.portrait_gender", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.portrait_gender"),
            Def("wt.iph.tool_gender", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.tool_gender"),
            Def("wt.iph.customer_conversion", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.customer_conversion"),
            Def("wt.iph.body_weights_csv", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.body_weights_csv"),
            Def("wt.iph.body_affects_breast_size", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.body_affects_breast_size"),
            Def("wt.iph.scavenger_double_virgin_csv", CarrierKind.WriteThrough, FamilyMods.Iph,
                "iph.scavenger_double_virgin_csv"),

            // ---- SubModHook(掌上青楼 0.10.4 经济接管,M5a;prefix 复刻其 const 内联
            //      方法,install 委托 = Patches/BrothelPatches.cs;TargetMod = brothel →
            //      版本不命中自动 VersionBlocked(Router 既有语义);无让位规则)----
            Def("brothel.budget", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelBudgetBonus),                                      // ServiceMath.EffectiveBudget
            Def("brothel.debt", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelDebtTags, CfgKeys.BrothelDebtChance),              // SellsGoods + PriceHint + PalmPage.PriceHintLine
            Def("brothel.guests", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelGuestsBase, CfgKeys.BrothelGuestsPerUpgrade,
                CfgKeys.BrothelGuestsMax),                                        // GuestCount/GuestsAfterUpgrade/Normalize/EnsurePages/AutoLockAll(上限 5 处)
            Def("brothel.traits", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelTraitsBase, CfgKeys.BrothelTraitsPerUpgrade),      // TraitCategories.RequirementCount
            Def("brothel.fee", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelFeeDefault, CfgKeys.BrothelFeeMax),                // ResolveFee/ClampFee + ShowFeeDialog 文本
            Def("brothel.mastery", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelMasteryThreshold),                                 // MaidenCatalog.TrainOneNight
            Def("brothel.schedule", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelVisitMinDays, CfgKeys.BrothelVisitMaxDays),        // MerchantSchedule.NextVisitDay
            Def("brothel.prices", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelPriceMaidDress, CfgKeys.BrothelPriceCatEars,
                CfgKeys.BrothelPriceCatTailPlug, CfgKeys.BrothelPriceLaceDress,
                CfgKeys.BrothelPriceBunnyEars, CfgKeys.BrothelPriceLaceGarter),   // BrothelItemCatalog.ItemValue(六件服饰)
            Def("brothel.stock", CarrierKind.SubModHook, FamilyMods.Brothel,
                CfgKeys.BrothelStockFirst, CfgKeys.BrothelStockMin,
                CfgKeys.BrothelStockMax),                                         // GearCatalog.RollStock

            // ---- SubModHook·tearlaments(M5b,dui_mian_lan_qiu_ji_tearl v7 交易接管;
            //      install 委托 = Patches/TearlPatches.cs;TargetMod = tearl → 版本
            //      压制同 brothel 语义,无让位规则;Create 为 tearl.budget 与
            //      tearl.trade 两单元共享——方法级引用计数裁决物理卸载)----
            Def("tearl.spawn", CarrierKind.SubModHook, FamilyMods.Tearl,
                CfgKeys.TearlSpawnChance),                                        // TearlamentsMod.HandleCustomerSpawn(私有实例,掷骰 0.3 内联 :61)
            Def("tearl.budget", CarrierKind.SubModHook, FamilyMods.Tearl,
                CfgKeys.TearlBudgetBase, CfgKeys.TearlBudgetRandom),              // CustomCustomer.Create(:53 SetBudget 内联)
            Def("tearl.trade", CarrierKind.SubModHook, FamilyMods.Tearl,
                CfgKeys.TearlSellDiscount, CfgKeys.TearlBuyDiscount,
                CfgKeys.TearlWaterPremium),                                       // Create(:54 buyPriceModifier)+ApplyBuyPriceFeature(:107 -25/:112 +20)

            // ---- SubModHook·暗流交易联动扩展(0.6.0 R4,lewd_meme_remake 1.5.19
            //      差分数值接管[R13a:绑定 bump 1.5.24,接管点零漂移];install
            //      委托 = Patches/RemakePatches.cs;TargetMod
            //      = remake → 版本压制同 brothel/tearl 语义,无让位规则——主 mod 侧
            //      alarm/trap/nightly 让位是「对主 mod 的 hook」,本组接管的是
            //      remake 自家实现,两者并存不冲突;乳汁单价三键双单元共享
            //      (工厂 :486 与夜间覆写 :1224 同值字面量);陷阱三档捕捉率
            //      (trap.tactic_chances)在 remake 在场时不可调——其值内联于
            //      transpile 保留区,接管需复刻 transpile 后方法 = 杀死 remake
            //      改写,裁定不实现,让位注明文案不变)----
            Def("remake.milk_items", CarrierKind.SubModHook, FamilyMods.Remake,
                CfgKeys.RemakeMilkPriceRich, CfgKeys.RemakeMilkPriceWondrous,
                CfgKeys.RemakeMilkPriceDiluted,
                CfgKeys.RemakeMilkFeedDiluted, CfgKeys.RemakeMilkFeedWondrous,
                CfgKeys.RemakeMilkFeedRich, CfgKeys.RemakeMilkDrinkCredit),       // ItemFactory.CreateMilkVariant(单价3+喂奶3+饮用1)
            Def("remake.milk_production", CarrierKind.SubModHook, FamilyMods.Remake,
                CfgKeys.RemakeMilkChanceRichBase, CfgKeys.RemakeMilkChanceRichPerLact,
                CfgKeys.RemakeMilkChanceWondrousBase, CfgKeys.RemakeMilkChanceWondrousPerLact,
                CfgKeys.RemakeMilkChanceDilutedBase, CfgKeys.RemakeMilkChanceDilutedPerLact,
                CfgKeys.RemakeMilkPriceRich, CfgKeys.RemakeMilkPriceWondrous,
                CfgKeys.RemakeMilkPriceDiluted),                                  // TentacleBody.TryProduceMilk(概率6+单价覆写3)
            // ST4 注销:remake.semen_formula(1.6.1 SemenProduction 类整体删除,精液
            // 公式回归主 mod 自家实现;nightly.semen_chance_cap 跨族等价消费[R7]
            // 终结,该键唯一消费 = main.nightly_transpile,注入点 IL_02c8 在
            // TranspilePlan 该 methodId 表内)+ remake.alarm_activity(1.6.1 该类
            // 重写为疲劳门禁,AddAlarm* 三方法不复存在;主 mod 侧
            // main.alarm_nightly_once 恢复 Own 即替代出口)——两单元目录行删除,
            // installs/复刻体随卸(ST2/ST4)
            Def("remake.ovulation_price", CarrierKind.SubModHook, FamilyMods.Remake,
                CfgKeys.RemakeOvulationPrice),                                   // ItemFactory.ConfigureOvulationShot(postfix 只改 unitValue;0.6.0 R7 等价恢复:主 mod 侧 price_ovulation_shot 让位[remake postfix 强制 100]后经 remake 自家 Configure 精确覆盖,默认 100 = 零漂移)
            // ST 1.6.1 Task 5(2026-11-17):疲劳核心两单元(ShadowTrade 1.6.1
            // MidnightPawn.Game/MaidenFatigue.cs 数值面,prefix 复刻;档位边界
            // const 90/80/60/20 保持原版;求值内核 = Logic/FatigueRules 纯函数,
            // 单测 FatigueRulesTests;Enabled/LevelMul/Scale/Get/Set/Today 等
            // remake 运行时成员经 EnsureResolved 委托直调——复刻体方法序与源码
            // 逐字对齐,仅常量换面板读数)
            Def("remake.fatigue_value_mul", CarrierKind.SubModHook, FamilyMods.Remake,
                CfgKeys.RemakeFatigueValueMulRested, CfgKeys.RemakeFatigueValueMulTired,
                CfgKeys.RemakeFatigueValueMulWeary, CfgKeys.RemakeFatigueValueMulExhausted),
                                                                                    // MaidenFatigue.ValueMul prefix(售价四档:<20 休息充足 1.1 / 20-59 正常 1f 硬编码无键 / ≥60 疲倦 0.94 / ≥80 疲惫 0.88 / ≥90 透支 0.8;消费点 ItemFactory.UpdateMaidenValue :769;四键全默认放行原方法)
            Def("remake.fatigue_nightly_recover", CarrierKind.SubModHook, FamilyMods.Remake,
                CfgKeys.RemakeFatigueNightlyGain, CfgKeys.RemakeFatigueExhaustRecover),
                                                                                    // MaidenFatigue.NightlyGain prefix(:265 weighted×0.5×LevelMul×孕×Scale 的基数 0.5f 单键;:260 口交加权 0.5f 为另一常量不接管)+ RecoverExhausted prefix(const ExhaustRecover :28 内联消费点 :124,双消费方 TentacleBody :596 / OriginalDeviceActivityPatch :95 同方法一并接管)

            // ---- SubModHook·巢穴扩展信息素(0.6.0 R12,2026-11-14:主面板
            //      client.semen_price_mod / semen_bonus_mod 两键对 Brood 让位
            //      [WeakenBasePerfume 进场强制 ±10 覆写 ±15]后的等价恢复出口;
            //      install 委托 = Patches/BroodPatches.cs,TargetMod = Brood →
            //      版本压制同 brothel/tearl/remake 语义,无让位规则——本单元
            //      接管的是 Brood 自家方法,主 mod 侧 semen_effects 的 Brood
            //      让位(三键含 semen_rep_mult)不变,两者并存不冲突)----
            Def("brood.perfume_mod", CarrierKind.SubModHook, FamilyMods.Brood,
                CfgKeys.BroodSemenPriceMod, CfgKeys.BroodSemenBonusMod),     // Fragrance.WeakenBasePerfume postfix(进场重设 ±15 系面板值;全默认零参与)

            // ---- 巢穴扩展·苗床标签拾取器(0.7.1,2026-11-18:Brood 标签编辑器的
            //      「选中」方式替换——红灯区放大镜链断裂[红灯区非家族前置 + 主
            //      mod 苗床区拒 magnifier],悬停苗床 + 快捷键直调 Brood
            //      MaidenTagEditor.Remember(internal static,brood_0331
            //      MaidenTagEditor.cs:26-34,经 ModReflect.CallStatic 带 Type[]
            //      重载防歧义;Brood 前置已声明);其余全走 Brood(编辑窗/标签
            //      语义/审计零改动)。**Kind = SubModHook 但无 Harmony patch**——
            //      宿主生命周期单元:install 委托 = EnsureResolved(Brood 类型
            //      装配期一次解析)+ 建 MonoBehaviour 宿主(Update 轮询
            //      Input.GetKeyUp → RenderHandler.RaycastElement<GameItem>[public
            //      static,game/RenderHandler.cs:491]→ 四守卫[打字屏蔽
            //      IsAnyTyping 跳 = StoreUIManager.cs:1476(InputActionManager.cs:108
            //      屏蔽层同款)/ hoveringStoreUI 悬 UI 跳 = HopperHelper.cs:991 /
            //      IsDraggingItem 拖物品跳 = ItemMouseDragHandler.cs:187 /
            //      isDraggingAnyUI 拖窗跳 = StoreUIManager.cs:289]→
            //      IsTag("MAIDEN_FEMALE") 过滤 → Remember + Note 提示),返回空
            //      MethodBase[](空记账合法,Uninstall 遍历 no-op)。四 Kind 中
            //      选 SubModHook 的理由:其装配路径与本单元需求完全同构——③
            //      登记期 TargetMod 缺席拦截(R11-F1,Brood 缺席物理零 hook)、
            //      VersionBlocked 压制(绑定 0.3.31,漂移自动失效)、
            //      ReconcileAll 开关对账(默认关 = 零宿主零轮询)、SetDesired
            //      热装卸、Dead 粘滞 + off→on 重试、自检路由审计自动覆盖;
            //      DataWrite 是装配期动作无开关对账、MainHook/WriteThrough 语义
            //      均不符。宿主销毁挂 HookRuntime.Report 的 Absent/VersionBlocked/
            //      Dead 迁移 + Shutdown 显式(UninstallAll 静默清场不走 Report)。
            //      Keys 两键:enabled = 自开关 Toggle(SwitchDispatch 表驱动 bool
            //      自身,不派生 .enabled)、key = KeyCode 参数键(不驱动装配,
            //      宿主 Update 热读 Enum.TryParse 即时生效)----
            Def("brood.tag_picker", CarrierKind.SubModHook, FamilyMods.Brood,
                CfgKeys.BroodTagPickerEnabled, CfgKeys.BroodTagPickerKey),   // 宿主生命周期单元(见上;install 委托 = Integration/BroodTagPicker.cs,无补丁)

            // ---- PrinterPatches·触手3D打印机(Task 14b,主 mod 1.7.1 新增线;
            //      install 委托 = Patches/PrinterPatches.cs,五键共用一单元;
            //      0.7.0 T7 形态改七接管点:price/needle_price 两键 postfix
            //      (ValuePostfixPatches 方法体,ApplyCsv 之后恒胜)+ grid/
            //      recipe_cost/base_progress 三键 transpiler(TranspilePlan
            //      printer_grids/printer_nightly_transpile/printer_progress
            //      三 methodId——恒 Own 无让位,Route 快照查不到时保守全应用)
            //      + RecipeTip/RefreshDescription 文案两 prefix 保留(1.9.2
            //      与 1.7.1 逐字一致已核,文案跟随配置的 0.6.0 增强行为不回归);
            //      默认全关 = 打印机线纯原版,任一键开关开 → 单元装配、各值经
            //      Current 读数(未开键退默认字面量,零漂移))----
            Def("main.printer", CarrierKind.MainHook, "lewd_meme",
                CfgKeys.PrinterPrice, CfgKeys.PrinterNeedlePrice, CfgKeys.PrinterGrid,
                CfgKeys.PrinterRecipeCost, CfgKeys.PrinterBaseProgress),
        };

        /// <summary>
        /// 面板值键中无任何钩子单元消费的键(显式排除集,测试 ② 的另一半)。
        /// 0.7.0 Task 4 起非空:min 系 5 键在 1.9.2 的 NightlyTick IL 里被编译期
        /// 折叠(min_per=1 并入加法、oral/anal_min_base=0 常量消除——见勘察摘要
        /// 「0.7.0 IL 注入点清单」A 节),无数值注入点可替换。其中 3 个
        /// per_level_min 键无任何单元消费 → 此处;oral_min_base / anal_min_base
        /// 仍被 main.manual_render 消费(说明书渲染读值显示)→ 留在其单元
        /// (键级聚合下任一 Own 即不标,行为与 0.6.0 一致),数值面不生效——
        /// Task 9 已在面板 tooltip 注「1.9.2 该数值已内联/无条件,暂不可调」。
        /// 0.7.0 Task 7 追加 alarm.semen_output(1.9.2 NightlyTickOnce 产出
        /// 无条件每苗床一瓶,无掷骰无计数 ldc,T2 F 节放弃键;原消费单元
        /// alarm_nightly_once 退役复刻体后移出;tooltip 注记 Task 9 已落)。
        /// 面板行全保留(值可存玩家偏好),
        /// 子项开关拨动无单元对账 = no-op 无害。
        /// </summary>
        public static readonly IReadOnlyList<string> DataOnlyKeys = new string[]
        {
            CfgKeys.SexPerLevelMin,
            CfgKeys.OralPerLevelMin,
            CfgKeys.AnalPerLevelMin,
            CfgKeys.AlarmSemenOutput,
        };

        private static HookUnitDef Def(string id, CarrierKind kind, string targetMod, params string[] keys)
        {
            return new HookUnitDef { Id = id, Kind = kind, Keys = keys, TargetMod = targetMod };
        }
    }
}
