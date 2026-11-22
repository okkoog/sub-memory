using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 一个子 mod 面板条目的定义(纯数据,复用 PanelEntry 思路但独立成表,
    /// 控制器 R-G:不修改 PanelKeys.cs 本体)。OurKey = 短前缀 + TargetKey
    /// ("remake.*" / "brood.*"),子项开关键沿用 PanelKeys.ItemSwitchKey 规则
    /// (OurKey + ".enabled",默认关 = 装上即纯原版)。Default = 对方官方默认值
    /// 的强类型镜像(string/int/bool,与 Kind 对应),同一值进 ConfigRegistry
    /// 注册段(R-I)与本表(默认值以勘察摘要为单一事实源,反编译逐字复核)。
    /// Carrier(M5a 起区分,R10 后三类)区分条目载体:WriteThrough = 写透对方
    /// 官方配置键(remake/Brood,TargetKey 指向对方键);SubModHook = 对方无
    /// 配置系统、由本 mod 经 Harmony 方法接管其逻辑(brothel,TargetKey 留空,
    /// TargetMod 仅作在场条件 Define 与版本压制查询键);MainHook(R10,唯一
    /// iph.attr_* 五键)= 接管**主 mod** 方法的共存补丁(TargetKey 留空,
    /// TargetMod = IPH 作在场条件 Define 与版本绑定——在场且匹配才 Own,
    /// 漂移 VersionBlocked 自动失效)。
    /// </summary>
    internal sealed class FamilyPanelEntry
    {
        public string OurKey;      // 本 mod 面板键(如 "remake.fatigue_enabled")
        public string TargetMod;   // 对方 mod ID(FamilyMods.Remake / FamilyMods.Brood / FamilyMods.Brothel)
        public string TargetKey;   // 对方 ModConfig 键名(如 "fatigue_enabled";SubModHook 条目不用,留 null)
        public CarrierKind Carrier; // 载体:WriteThrough = 写透官方配置;SubModHook = 方法接管(brothel)
        public PanelKind Kind;     // 面板控件形态(Choice / Toggle / SliderInt)
        public string Section;     // 分区名(暗流交易联动 / 巢穴扩展联动 / 掌上青楼联动)
        public string Label;
        public string Tooltip;     // WriteThrough 必含写透注记;SubModHook 必含接管注记
        public string[] Choices;   // 仅 Choice(= 对方游戏侧档位串,逐字;中文别名解析归对方,不复制)
        public int MinInt;         // 仅 SliderInt(WriteThrough = 对方 Define 下限;SubModHook = 本 mod 自定面板范围)
        public int MaxInt;         // 仅 SliderInt(同上,上限)
        public float MinFloat;     // 仅 SliderFloat(SubModHook 自定面板范围,M5b tearl.spawn_chance)
        public float MaxFloat;     // 仅 SliderFloat(同上,上限)
        public object Default;     // 强类型默认(装箱 string / int / float / bool,与 Kind 对应)
    }

    /// <summary>
    /// 子 mod 官方 ModConfig 键的镜像定义表(M4b 写透的面板半边,纯数据;
    /// 游戏侧载体 = Integration/WriteThroughCarrier.cs,分发 = HookRuntime)。
    /// 单一事实源 = 2026-11-12 家族勘察摘要 + 反编译逐字复核:
    ///  - remake 1.6.1 ModSettings.cs(ShadowTrade 源码):ST4 后 8 键全部
    ///    string Choices——存续 4(fatigue_enabled/fatigue_scale[PercentChoices
    ///    6 档]/buyer_per_day[{1,2,3}——勘察摘要的「1-5」系笔误]/corruption_
    ///    box_settlement)+ 桶装 4(auto_barrel_semen/milk_wondrous/_rich/
    ///    _diluted,DefineBarrel :34-37 ToggleChoices {"开","关"} 默认"开",
    ///    消费方 CreateNightBarrel 判"非关即开");ST4 撤 product_chance/
    ///    ovulation_guaranteed/virgin_avoidance(1.6.1 对方已删,旧 json 残留
    ///    由键存在性守卫无害化);
    ///  - Brood 0.3.31 Settings.cs:115-132:官方 14 键中 12 键镜像(热键
    ///    testPanelKey/dockToggleKey 为 KeyCode 捕获,属对方面板特性,不镜像,
    ///    spec §5.4):10 int 带 Define 范围 + 2 bool;
    ///  - brothel 0.10.4 无任何配置系统,数值 const 内联 → 22 键 SubModHook
    ///    方法接管(M5a,R-K/R-L:9 个目录单元,默认值 = Rules/* 反编译字面量,
    ///    MinInt/MaxInt 为本 mod 自定面板元数据,TargetKey 留空);
    ///  - tearl v7 同无配置系统,交易数值 const 内联 → 6 键 SubModHook 方法接管
    ///    (M5b,R-O/R-P:3 个目录单元,默认值 = Tearlaments/* 反编译字面量,
    ///    spawn_chance 为 float(SliderFloat),TargetKey 留空);
    ///  - remake 1.5.19 差分数值(0.6.0 R4,2026-11-13):乳汁差分/产出概率
    ///    const 内联 → SubModHook 方法接管(默认值 = remake_1519 反编译字面量
    ///    [R8 复核 remake_1521 逐字一致;ST2 复核 1.6.1 源码零漂移];概率/系数
    ///    为 float(SliderFloat),TargetKey 留空;主 mod 侧对应单元 1.6.1 起共存
    ///    Own/price_milk 让 Brood,与本组 remake 自家差分接管互不干扰);
    ///    0.6.0 R7 追加 remake.ovulation_price(等价恢复,见 CfgKeys);ST4
    ///    注销 semen_formula(SemenProduction 类整体删除)+ alarm_activity
    ///    (AddAlarm* 三方法消失)两单元,差分键 19 → 14;ST5(2026-11-17)
    ///    追加疲劳核心六键(ShadowTrade 1.6.1 MaidenFatigue 数值面 prefix
    ///    复刻:售价四档/夜间基数/透支恢复,默认 = 源码字面量),差分 14 → 20。
    ///  - IPH 1.2.17(0.6.0 R9,2026-11-14):官方 ModConfig 17 键全 WriteThrough
    ///    镜像,hotkey(KeyCode 枚举,Brood 热键先例)跳过 → 16 键;默认值/
    ///    Choices/范围 = iph_1217 逐字[R13a 复核 iph_1218;1.3.0 复核
    ///    iph_130:16 键键名/默认/Choices/CSV 形态零漂移,hotkey 已迁官方
    ///    KeyBind 控件——不镜像裁定更优](键清单与 1215 有漂移:body_preference
    ///    与 scavenger_double_virgin_0..4 已并为两个 CSV 单键,见分区注释);
    ///    body_weights_csv / scavenger_double_virgin_csv 无 Choices,以
    ///    PanelKind.Input 文本框形态镜像(string 直通写透)。
    ///    0.6.0 R10(2026-11-14)追加 5 键(Carrier = MainHook,共 21):少女生成
    ///    属性复刻 iph.attr_*(postfix on 主 mod ItemFactory.CreateMaidenOfFaction,
    ///    与 IPH 工厂 transpiler 共存——R9 让位的例外形态,见分区注释;TargetMod
    ///    = IPH 版本绑定漂移自动失效;默认 = 主 mod 1.7.1 RollAttr 字面量)。
    ///    0.6.0 R12(2026-11-14)追加 2 键(共 89):Brood 信息素两键
    ///    brood.semen_price_mod / semen_bonus_mod(Carrier = SubModHook,
    ///    TargetMod = Brood——postfix on Brood Fragrance.WeakenBasePerfume,
    ///    主面板 client.semen_* 让位后的等价恢复出口,R10 同模式默认 = 主 mod
    ///    原字面量,见巢穴扩展分区注释)。
    /// 并发语义(spec §4.5,被动 last-writer-wins):开启期间以本面板为准
    /// (我方写入立即生效),期间对方自家的修改以对方为准,直到本面板下一次
    /// 改动;从不订阅对方 OnChanged。快照仅会话内存,不落盘。
    /// 镜像初值不强行同步:对方已有非默认值(玩家上周改过)时,本面板仍以
    /// 自家存储的值为准——tooltip 注明。目标缺席 = 分区整体不 Define
    /// (PanelConfig.DefineFamily 条件 Define,R-I),零写零订阅。
    /// </summary>
    internal static class FamilyPanelKeys
    {
        internal const string RemakeSection = "暗流交易联动";
        internal const string BroodSection = "巢穴扩展联动";
        internal const string BrothelSection = "掌上青楼联动";
        internal const string TearlSection = "tearlaments 联动";
        internal const string IphSection = "物品高清立绘管理器联动";
        // 0.8.0:主 mod 自身官方设置(拾荒分区 3 键)的写透镜像分区——TargetMod
        // = MainMod,在场过滤/写透载体/开关派生全链路复用(Family.Probe 首位即
        // 主 mod;WriteThroughCarrier 的 ModConfig.Find("lewd_meme") 通用)。
        internal const string MainModSection = "主 mod 官方设置(拾荒)";

        /// <summary>写透注记(控制器逐字要求,拼进每条 WriteThrough 条目的 Tooltip)。</summary>
        internal const string ThroughNote = "开启后以本面板为准,关闭恢复开启前对方值";

        /// <summary>接管注记(M5a,拼进每条 SubModHook 条目的 Tooltip:接管的是
        /// 对方方法逻辑而非对方配置——brothel 无配置系统)。</summary>
        internal const string TakeoverNote = "开启后以本面板值接管,关闭恢复其原版行为";

        /// <summary>remake 百分比档位(PercentChoices,ModSettings.cs:28 逐字)。</summary>
        internal static readonly string[] PercentChoices =
            { "50%", "75%", "100%", "150%", "200%", "300%" };

        /// <summary>remake 开/关档位(fatigue_enabled 等 4 键共用)。</summary>
        internal static readonly string[] OnOffChoices = { "开", "关" };

        /// <summary>remake 收购商档位(BuyerChoices,ModSettings.cs:30 逐字)。</summary>
        internal static readonly string[] BuyerChoices = { "1", "2", "3" };

        internal static readonly IReadOnlyList<FamilyPanelEntry> Entries = new[]
        {
            // ---- 暗流交易联动(remake 写透 8 键[ST4: 7−3+4],全部 string Choices;
            //      ST4 撤 product_chance/ovulation_guaranteed/virgin_avoidance——
            //      对方 1.6.1 已删这三键,旧 json 残留由写透键存在性守卫无害化;
            //      新增桶装 4 键 = 1.6.1 ModSettings.cs:34-37 DefineBarrel,
            //      默认"开",关闭只禁止自动新建、已有桶装仍自动补充)----
            new FamilyPanelEntry
            {
                OurKey = "remake.fatigue_enabled", TargetMod = FamilyMods.Remake,
                TargetKey = "fatigue_enabled", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "疲劳系统(暗流交易)", Choices = OnOffChoices, Default = "开",
                Tooltip = "暗流交易疲劳系统开关(开=按原版实际活动次数累积疲劳,入夜透支时暂停该角色全部夜间活动、双方产出、受孕与孕期,恢复 5 点;关=停用疲劳修正及门禁);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.fatigue_scale", TargetMod = FamilyMods.Remake,
                TargetKey = "fatigue_scale", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "疲劳累积速度(暗流交易)", Choices = PercentChoices, Default = "100%",
                Tooltip = "暗流交易疲劳增长倍率档位(100% 为默认,只调增速不改原版活动次数);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.buyer_per_day", TargetMod = FamilyMods.Remake,
                TargetKey = "buyer_per_day", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "每日收购商上限(暗流交易)", Choices = BuyerChoices, Default = "1",
                Tooltip = "暗流交易每天最多来访的苗床收购商数(默认 1);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.corruption_box_settlement", TargetMod = FamilyMods.Remake,
                TargetKey = "corruption_box_settlement", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "侵蚀区苗床箱夜间结算(暗流交易)", Choices = OnOffChoices, Default = "关",
                Tooltip = "暗流交易侵蚀区苗床箱内角色调用原版本体结算(产出进本体产出区、计入原版报告与经验,并追加其乳汁与疲劳;其它容器不参与,默认关);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_semen", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_semen", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "自动新建桶装:精液(暗流交易)", Choices = OnOffChoices, Default = "开",
                Tooltip = "暗流交易夜间结算后扫描产出区,同类瓶装至少 8 瓶时自动新建桶装(默认开;关闭只禁止自动新建,已有桶装仍会自动补充);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_milk_wondrous", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_milk_wondrous", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "自动新建桶装:神奇乳汁(暗流交易)", Choices = OnOffChoices, Default = "开",
                Tooltip = "暗流交易夜间结算后扫描产出区,神奇乳汁至少 3 瓶时自动新建桶装(默认开;关闭只禁止自动新建);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_milk_rich", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_milk_rich", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "自动新建桶装:浓缩乳汁(暗流交易)", Choices = OnOffChoices, Default = "开",
                Tooltip = "暗流交易夜间结算后扫描产出区,浓缩乳汁至少 3 瓶时自动新建桶装(默认开;关闭只禁止自动新建);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_milk_diluted", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_milk_diluted", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "自动新建桶装:清甜乳汁(暗流交易)", Choices = OnOffChoices, Default = "开",
                Tooltip = "暗流交易夜间结算后扫描产出区,清甜乳汁至少 3 瓶时自动新建桶装(默认开;关闭只禁止自动新建);" + ThroughNote,
            },

            // ---- 暗流交易联动·差分接管(0.6.0 R4,remake 1.5.19 const 内联 → SubModHook
            //      方法接管;默认值 = remake_1519 反编译字面量逐字[R8 复核
            //      remake_1521 逐字一致];MinInt/MaxInt 与
            //      MinFloat/MaxFloat 为本 mod 自定面板元数据(对方无 Define 可镜像);
            //      TargetKey 留空)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkPriceRich, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "浓缩乳汁单价(暗流交易)", MinInt = 0, MaxInt = 999999, Default = 150,
                Tooltip = "贫乳苗床的浓缩乳汁单价(默认 150;夜间产出与目录工厂两处同步接管);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkPriceWondrous, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "神奇乳汁单价(暗流交易)", MinInt = 0, MaxInt = 999999, Default = 80,
                Tooltip = "普通乳房苗床的神奇乳汁单价(默认 80);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkPriceDiluted, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "清甜乳汁单价(暗流交易)", MinInt = 0, MaxInt = 999999, Default = 50,
                Tooltip = "巨乳苗床的清甜乳汁单价(默认 50);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkFeedDiluted, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "清甜乳汁喂奶缓解(暗流交易)", MinInt = 0, MaxInt = 100, Default = 20,
                Tooltip = "把清甜乳汁喂给苗床时缓解的疲劳值(默认 20);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkFeedWondrous, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "神奇乳汁喂奶缓解(暗流交易)", MinInt = 0, MaxInt = 100, Default = 30,
                Tooltip = "把神奇乳汁喂给苗床时缓解的疲劳值(默认 30);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkFeedRich, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "浓缩乳汁喂奶缓解(暗流交易)", MinInt = 0, MaxInt = 100, Default = 25,
                Tooltip = "把浓缩乳汁喂给苗床时缓解的疲劳值(默认 25);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkDrinkCredit, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "饮用乳汁信用(暗流交易)", MinInt = 0, MaxInt = 999999, Default = 30,
                Tooltip = "直接饮用一瓶差分乳汁获得的信用点(默认 30,瓶子随之销毁);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceRichBase, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "浓缩乳汁产出概率(基础)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "贫乳苗床每晚产出浓缩乳汁的概率基础值(默认 0.3);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceRichPerLact, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "浓缩乳汁产出概率(每级)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "浓缩乳汁概率的每级泌乳增量(默认 0.3,原版三档共用系数);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceWondrousBase, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "神奇乳汁产出概率(基础)", MinFloat = 0f, MaxFloat = 1f, Default = 0.5f,
                Tooltip = "普通乳房苗床每晚产出神奇乳汁的概率基础值(默认 0.5);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceWondrousPerLact, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "神奇乳汁产出概率(每级)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "神奇乳汁概率的每级泌乳增量(默认 0.3);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceDilutedBase, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "清甜乳汁产出概率(基础)", MinFloat = 0f, MaxFloat = 1f, Default = 0.7f,
                Tooltip = "巨乳苗床每晚产出清甜乳汁的概率基础值(默认 0.7);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceDilutedPerLact, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "清甜乳汁产出概率(每级)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "清甜乳汁概率的每级泌乳增量(默认 0.3);" + TakeoverNote,
            },
            // ST4(2026-11-17)全链废弃 5 键条目:remake.semen_base /
            // remake.semen_train_factor(1.6.1 SemenProduction 类整体删除,精液
            // 公式回归主 mod——面板出口 = 主面板 nightly.semen_chance_* 三键)
            // + remake.alarm_oral/sex/anal_per_night(OriginalDeviceActivityPatch
            // 重写为疲劳门禁,AddAlarm* 三方法消失——主 mod 侧闹钟三键恢复生效)。
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeOvulationPrice, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "排卵诱发剂单价(暗流交易)", MinInt = 0, MaxInt = 999999, Default = 100,
                Tooltip = "暗流交易在场时排卵诱发剂的单价(其内部强制 100,本面板在其后精确覆盖;默认 100 = 无漂移;主面板 price.ovulation_shot 在暗流交易在场时不生效);" + TakeoverNote,
            },
            // ---- 暗流交易联动·疲劳核心接管(ST 1.6.1 Task 5,2026-11-17:ShadowTrade
            //      1.6.1 疲劳系统数值面,SubModHook prefix 复刻;默认 = 源码
            //      MidnightPawn.Game/MaidenFatigue.cs 字面量零漂移;档位语义名按
            //      StateName,边界 const 90/80/60/20 保持原版不可调)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulRested, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "疲劳售价倍率:休息充足(暗流交易)", MinFloat = 0f, MaxFloat = 5f, Default = 1.1f,
                Tooltip = "疲劳低于 20 的苗床售价倍率(默认 1.1 = +10%;写透键 fatigue_enabled 关闭疲劳系统时恒 1);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulTired, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "疲劳售价倍率:疲倦(暗流交易)", MinFloat = 0f, MaxFloat = 5f, Default = 0.94f,
                Tooltip = "疲劳 60-79 的苗床售价倍率(默认 0.94 = -6%;20-59 正常档恒 1 不可调);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulWeary, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "疲劳售价倍率:疲惫(暗流交易)", MinFloat = 0f, MaxFloat = 5f, Default = 0.88f,
                Tooltip = "疲劳 80-89 的苗床售价倍率(默认 0.88 = -12%);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulExhausted, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "疲劳售价倍率:透支(暗流交易)", MinFloat = 0f, MaxFloat = 5f, Default = 0.8f,
                Tooltip = "疲劳 ≥90(透支)的苗床售价倍率(默认 0.8 = -20%;透支同时暂停该角色本夜全部活动,门禁不受本键影响);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueNightlyGain, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "夜间疲劳基数(暗流交易)", MinFloat = 0f, MaxFloat = 5f, Default = 0.5f,
                Tooltip = "每晚按行为加权次数(性交+肛交+口交×0.5)累积疲劳的基数(默认 0.5;0 = 不累积;实际增量还乘本体等级系数、孕期 1.5 与写透键 fatigue_scale 的累积速度);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueExhaustRecover, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "透支恢复量(暗流交易)", MinInt = 0, MaxInt = 100, Default = 5,
                Tooltip = "透支苗床夜间结算后恢复的疲劳值(默认 5;场外苗床每日自然恢复 25 保持原版);" + TakeoverNote,
            },

            // ---- 巢穴扩展联动(Brood 12 键:10 int 带范围 + 2 bool)----
            new FamilyPanelEntry
            {
                OurKey = "brood.eggBaseChance", TargetMod = FamilyMods.Brood,
                TargetKey = "eggBaseChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "产卵基础概率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 12,
                Tooltip = "巢穴扩展产卵基础概率 %(默认 12);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.humanToEggChance", TargetMod = FamilyMods.Brood,
                TargetKey = "humanToEggChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "人类转产卵概率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 30,
                Tooltip = "巢穴扩展人类转产卵概率 %(默认 30);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.whiteGrowChance", TargetMod = FamilyMods.Brood,
                TargetKey = "whiteGrowChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "白色触手生长概率(巢穴扩展)", MinInt = 1, MaxInt = 80, Default = 12,
                Tooltip = "巢穴扩展白色触手生长概率 %(默认 12,范围 1-80);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.whiteBodyChance", TargetMod = FamilyMods.Brood,
                TargetKey = "whiteBodyChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "白色体型概率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 10,
                Tooltip = "巢穴扩展白色体型概率 %(默认 10);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.infectMajor", TargetMod = FamilyMods.Brood,
                TargetKey = "infectMajor", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "重度感染概率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 30,
                Tooltip = "巢穴扩展重度感染概率 %(默认 30);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.infectMinor", TargetMod = FamilyMods.Brood,
                TargetKey = "infectMinor", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "轻度感染概率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 15,
                Tooltip = "巢穴扩展轻度感染概率 %(默认 15);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.infectNone", TargetMod = FamilyMods.Brood,
                TargetKey = "infectNone", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "无感染概率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 5,
                Tooltip = "巢穴扩展无感染概率 %(默认 5);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.tentacleSpeechFrequency", TargetMod = FamilyMods.Brood,
                TargetKey = "tentacleSpeechFrequency", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "触手语音频率(巢穴扩展)", MinInt = 0, MaxInt = 100, Default = 20,
                Tooltip = "巢穴扩展触手语音频率 %(默认 20);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.tentacleSpeechSpeed", TargetMod = FamilyMods.Brood,
                TargetKey = "tentacleSpeechSpeed", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "触手语音语速(巢穴扩展)", MinInt = 25, MaxInt = 300, Default = 100,
                Tooltip = "巢穴扩展触手语音语速 %(默认 100,范围 25-300);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.tentacleSpeechHold", TargetMod = FamilyMods.Brood,
                TargetKey = "tentacleSpeechHold", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "触手语音停留(巢穴扩展)", MinInt = 1, MaxInt = 15, Default = 4,
                Tooltip = "巢穴扩展触手语音停留秒数(默认 4,范围 1-15);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.disableRampageEnding", TargetMod = FamilyMods.Brood,
                TargetKey = "disableRampageEnding", Kind = PanelKind.Toggle, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "禁用狂暴结局(巢穴扩展)", Default = false,
                Tooltip = "禁用巢穴扩展的狂暴结局(默认关);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.disableDevourFlash", TargetMod = FamilyMods.Brood,
                TargetKey = "disableDevourFlash", Kind = PanelKind.Toggle, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "禁用吞噬闪光(巢穴扩展)", Default = false,
                Tooltip = "禁用巢穴扩展的吞噬闪光特效(默认关);" + ThroughNote,
            },

            // ---- 巢穴扩展联动·信息素接管(0.6.0 R12,2026-11-14:主面板
            //      client.semen_price_mod / semen_bonus_mod 两键在 Brood 在场时
            //      让位(WeakenBasePerfume 进场强制 ±10)后的等价恢复出口;
            //      Carrier = SubModHook——postfix on Brood Fragrance.
            //      WeakenBasePerfume(brood_0331 BroodMainCycle.TentacleChild/
            //      Fragrance.cs:30-47,运行时唯一入口 = OnClientArea :19 无条件
            //      调;:103 是 RegisterTests 测试代码),Brood 写 ±10 后同 API
            //      同槽位重设面板值——进场时序恒胜(R12 复审零风险清单,R10
            //      属性键同模式:默认 = 主 mod 原字面量 -15/+15,全默认零参与;
            //      TargetKey 留空,TargetMod = Brood 作在场条件 Define 与版本
            //      绑定[0.3.31,漂移自动 VersionBlocked])----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BroodSemenPriceMod, TargetMod = FamilyMods.Brood,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = BroodSection,
                Label = "精液买价修正(巢穴扩展在场)", MinFloat = -100f, MaxFloat = 999f, Default = -15f,
                Tooltip = "顾客进场时触手精液买价修正 %(默认 -15 = 主 mod 原值;巢穴扩展在场会把主面板同名键压到 -10,本键在其后接管;主面板旧键在巢穴扩展在场时不生效);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BroodSemenBonusMod, TargetMod = FamilyMods.Brood,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = BroodSection,
                Label = "精液卖价加成(巢穴扩展在场)", MinFloat = -100f, MaxFloat = 999f, Default = 15f,
                Tooltip = "顾客进场时触手精液卖价加成 %(默认 +15 = 主 mod 原值;巢穴扩展在场会把主面板同名键压到 +10,本键在其后接管;主面板旧键在巢穴扩展在场时不生效);" + TakeoverNote,
            },

            // ---- 巢穴扩展联动·苗床标签拾取器(0.7.1,2026-11-18:Brood 标签编辑器的
            //      「选中」方式替换——红灯区放大镜链断裂[红灯区非家族前置,大量玩家
            //      未装;主 mod 苗床区拒绝 magnifier 交互,0x13.log:6668-6683 实证],
            //      悬停苗床 + 快捷键直调 MaidenTagEditor.Remember(item)
            //      [brood_0331 MaidenTagEditor.cs:26-34],其余全走 Brood——玩家照旧
            //      开 Brood Home 面板点「编辑苗床标签」;编辑逻辑/标签语义/审计零改动。
            //      Carrier = SubModHook 但**无 Harmony patch**:目录单元
            //      brood.tag_picker = 宿主生命周期单元(install 委托建 MonoBehaviour
            //      宿主,返回空记账;Kind 选择理由见 HookCatalog 注释)。两键形态:
            //      enabled = Toggle 自开关(SubModHook Toggle 经 SwitchDispatch 表驱动
            //      bool 自身,不派生 .enabled 行——面板恰 +1 行)、key = EnumKeyCode
            //      (游戏原生 DefineEnum<KeyCode> 下拉,Brood 热键先例
            //      Settings.cs:130;注册表层存 KeyCode 名串往返,面板恰 +1 行)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BroodTagPickerEnabled, TargetMod = FamilyMods.Brood,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.Toggle, Section = BroodSection,
                Label = "苗床标签拾取器(巢穴扩展)", Default = false,
                Tooltip = "开启后悬停苗床(格子内)按下方快捷键即可选中它,供巢穴扩展 Home 面板的『编辑苗床标签』使用——不再依赖红灯区的放大镜检视链;关闭则零宿主零轮询,恢复其原版行为(仅放大镜可选);开启后以本面板值接管,关闭恢复其原版行为",
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BroodTagPickerKey, TargetMod = FamilyMods.Brood,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.EnumKeyCode, Section = BroodSection,
                Label = "拾取快捷键(巢穴扩展)", Default = "U",
                Tooltip = "避开 F7/F8/F9/Home/[ ] 等已占用键;悬停苗床按此键选中,再开 Brood 面板(Home)点『编辑苗床标签』(默认 U,改动即时生效;键位本身无启停语义,上方开关才控制拾取器);开启后以本面板值接管,关闭恢复其原版行为",
            },

            // ---- 掌上青楼联动(M5a,brothel 0.10.4 无配置系统 → SubModHook 方法接管;
            //      默认值 = 反编译 LewdMemeBrothel.Rules 字面量,逐字;MinInt/MaxInt 为
            //      本 mod 自定面板范围(对方无 Define 可镜像);TargetKey 留空)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelBudgetBonus, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "标签预算加成(掌上青楼)", MinInt = 0, MaxInt = 999, Default = 10,
                Tooltip = "客人预算按每个匹配标签上浮的百分比(默认 10);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelDebtTags, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "负债线标签数(掌上青楼)", MinInt = 0, MaxInt = 20, Default = 5,
                Tooltip = "匹配标签达到该数,超预算货品才可能被负债买下(默认 5);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelDebtChance, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "负债买下概率(掌上青楼)", MinInt = 0, MaxInt = 100, Default = 50,
                Tooltip = "达到负债线后货品被买下的百分比概率(默认 50);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelGuestsBase, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "预约基础人数(掌上青楼)", MinInt = 1, MaxInt = 50, Default = 2,
                Tooltip = "每日预约客人的基础人数(默认 2);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelGuestsPerUpgrade, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "每次扩建增量(掌上青楼)", MinInt = 0, MaxInt = 50, Default = 2,
                Tooltip = "每完成一次房间扩建增加的预约人数(默认 2);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelGuestsMax, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "每日上限(掌上青楼)", MinInt = 1, MaxInt = 50, Default = 20,
                Tooltip = "预约客人与页签的每日上限(默认 20,消费于 5 处);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelTraitsBase, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "基础标签数(掌上青楼)", MinInt = 1, MaxInt = 8, Default = 3,
                Tooltip = "客人需求的基础标签数(默认 3,上限 8 类);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelTraitsPerUpgrade, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "标签扩建增量(掌上青楼)", MinInt = 0, MaxInt = 8, Default = 1,
                Tooltip = "每完成一次房间扩建增加的需求标签数(默认 1);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelFeeDefault, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "默认过夜费(掌上青楼)", MinInt = 0, MaxInt = 9999, Default = 120,
                Tooltip = "未单独设置过夜费时的取值(默认 120);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelFeeMax, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "过夜费上限(掌上青楼)", MinInt = 1, MaxInt = 999999, Default = 9999,
                Tooltip = "过夜费的钳制上限,输入框提示随动(默认 9999);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelMasteryThreshold, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "精通阈值(掌上青楼)", MinInt = 1, MaxInt = 9999, Default = 30,
                Tooltip = "单类服务次数达到该值获得精通标签(默认 30);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelVisitMinDays, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "来访最小间隔日(掌上青楼)", MinInt = 1, MaxInt = 99, Default = 4,
                Tooltip = "商人两次来访的最小间隔天数(默认 4);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelVisitMaxDays, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "来访最大间隔日(掌上青楼)", MinInt = 1, MaxInt = 99, Default = 9,
                Tooltip = "商人两次来访的最大间隔天数(默认 9,原版掷骰 4..9);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceMaidDress, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "女仆装售价(掌上青楼)", MinInt = 0, MaxInt = 999999, Default = 260,
                Tooltip = "服饰商的女仆装单价(默认 260);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceCatEars, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "猫耳售价(掌上青楼)", MinInt = 0, MaxInt = 999999, Default = 180,
                Tooltip = "服饰商的猫耳单价(默认 180);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceCatTailPlug, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "猫尾肛塞售价(掌上青楼)", MinInt = 0, MaxInt = 999999, Default = 220,
                Tooltip = "服饰商的猫尾肛塞单价(默认 220);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceLaceDress, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "蕾丝裙售价(掌上青楼)", MinInt = 0, MaxInt = 999999, Default = 240,
                Tooltip = "服饰商的蕾丝裙单价(默认 240);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceBunnyEars, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "兔耳售价(掌上青楼)", MinInt = 0, MaxInt = 999999, Default = 175,
                Tooltip = "服饰商的兔耳单价(默认 175);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceLaceGarter, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "蕾丝吊袜带售价(掌上青楼)", MinInt = 0, MaxInt = 999999, Default = 150,
                Tooltip = "服饰商的蕾丝吊袜带单价(默认 150);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelStockFirst, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "首访上架件数(掌上青楼)", MinInt = 0, MaxInt = 6, Default = 3,
                Tooltip = "商人首次来访上架的固定件数(默认 3,按目录顺序取前 N 件);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelStockMin, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "后续最小件数(掌上青楼)", MinInt = 0, MaxInt = 6, Default = 3,
                Tooltip = "后续来访随机上架件数下限(默认 3);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelStockMax, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "后续最大件数(掌上青楼)", MinInt = 0, MaxInt = 6, Default = 5,
                Tooltip = "后续来访随机上架件数上限(默认 5,原版掷骰 3..5);" + TakeoverNote,
            },

            // ---- tearlaments 联动(M5b,tearl v7 无配置系统 → SubModHook 方法接管;
            //      默认值 = 反编译 Tearlaments/* 字面量逐字(R-O);MinInt/MaxInt 为
            //      本 mod 自定面板范围(对方无 Define 可镜像);TargetKey 留空)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlSpawnChance, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = TearlSection,
                Label = "出现概率(tearlaments)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "Kitkallos 每次普通顾客生成时的出现概率(默认 0.3);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlBudgetBase, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "收购预算基础(tearlaments)", MinInt = 0, MaxInt = 1000000, Default = 150000,
                Tooltip = "Kitkallos 收购预算的基础值(默认 150000);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlBudgetRandom, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "收购预算随机增量(tearlaments)", MinInt = 0, MaxInt = 1000000, Default = 50000,
                Tooltip = "收购预算在基础值上追加的随机量上限(默认 50000,含上界);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlSellDiscount, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "卖出折扣(tearlaments)", MinInt = 0, MaxInt = 100, Default = 10,
                Tooltip = "Kitkallos 购入你摊位货品的整体折扣 %(默认 10,即买价 -10%);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlBuyDiscount, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "收购少女折扣(tearlaments)", MinInt = 0, MaxInt = 100, Default = 25,
                Tooltip = "Kitkallos 收购少女物品时附加的折扣 %(默认 25,即 -25%);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlWaterPremium, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "收购水溢价(tearlaments)", MinInt = 0, MaxInt = 100, Default = 20,
                Tooltip = "Kitkallos 收购接受的水时附加的溢价 %(默认 20,即 +20%);" + TakeoverNote,
            },
            // 0.8.0:Aquashard 差分三键(tearl 15 新数值面;transpiler 形态,
            // 消费点 = tearl 自家 CustomItem.CreateAquashard / BroodTribute.
            // AfterNightlyTick,注入点/锚点见 TranspilePlan G 节)
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlAquashardValue, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "水晶单价(tearlaments)", MinInt = 1, MaxInt = 9999, Default = 800,
                Tooltip = "Aquashard Crystal 的单价(tearl 15 原版 800;巢穴区每夜进贡产出物);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlAquashardChance, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = TearlSection,
                Label = "水晶每晚产出率(tearlaments)", MinFloat = 0f, MaxFloat = 1f, Default = 1f,
                Tooltip = "巢穴区每位 Tearlaments 少女每夜产出 Aquashard 的概率(tearl 15 原版 1.0 = 100%;manifest 描述的 20% 已过时,以代码为准);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlAquashardUnits, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "水晶每少女产量(tearlaments)", MinInt = 0, MaxInt = 10, Default = 1,
                Tooltip = "产出判定命中时每位 Tearlaments 少女产出的 Aquashard 数量(tearl 15 原版 1);" + TakeoverNote,
            },

            // ---- 物品高清立绘管理器联动(0.6.0 R9,IPH 1.2.17 写透镜像[R13a:
            //      bump 1.2.18,17 键零漂移;1.3.0 复核 iph_130:16 键零漂移,
            //      hotkey 迁官方 KeyBind 控件——不镜像更优,仍跳过],全部
            //      WriteThrough;默认值/Choices/范围 = iph_1217 反编译逐字
            //      (R13a 复核 iph_1218/1.3.0 复核 iph_130 一致):ModEntry.cs
            //      Init 15 键 + BodyCsvSettings.DefineWeights/DefineChances
            //      2 键 = 17 键,hotkey(1218 前 KeyCode 枚举 Choices,130 起
            //      官方 KeyBind 控件)按 Brood 热键先例跳过 → 16 键镜像。
            //      ⚠️ 键清单与 1215 不同:body_preference 与 scavenger_double_
            //      virgin_0..4(5 个 int)已于 1.2.17 迁移为 body_weights_csv /
            //      scavenger_double_virgin_csv 两个 CSV 单键(BodyCsvSettings.
            //      cs:42/49,LegacyChances 兼容读旧键迁移)——两键无 Choices,
            //      以 Input 文本框形态镜像(主面板 maiden 四池 CSV 同形态);
            //      CSV 默认串 = BodyCsvRules.Defaults(5, 20.0/60.0) 即
            //      "20, 20, 20, 20, 20" / "60, 60, 60, 60, 60",项序 = 萝莉,
            //      少女,御姐,少妇,熟女(AdultBodySources.Bodies)。这两个键
            //      也是 IPH 在场时主面板阵营/体型/胸部三池让位后的玩家出口)----
            new FamilyPanelEntry
            {
                OurKey = "iph.original_portrait_selection", TargetMod = FamilyMods.Iph,
                TargetKey = "original_portrait_selection", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "仅高清重建(立绘管理器)", Default = false,
                Tooltip = "关闭本模组所有自动立绘抽取,立绘完全由主 mod 选取,只做高清重建(默认关);使用注入式立绘包(少女当铺等)时勿开启本项;" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.final_portrait_selection", TargetMod = FamilyMods.Iph,
                TargetKey = "final_portrait_selection", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "仅用最终生效立绘(立绘管理器)", Default = false,
                Tooltip = "选图只使用游戏当前最终生效立绘,固定权重不读可调权重(默认关);使用注入式立绘包(少女当铺等)时勿开启本项;" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.resolution", TargetMod = FamilyMods.Iph,
                TargetKey = "resolution", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "立绘分辨率(立绘管理器)", Choices = new[] { "128", "256" }, Default = "256",
                Tooltip = "立绘纹理分辨率档位(默认 256;显示尺寸与占格不变,改后清缓存立即生效);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.guest_portrait_hd", TargetMod = FamilyMods.Iph,
                TargetKey = "guest_portrait_hd", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "顾客立绘高清显示(立绘管理器)", Default = true,
                Tooltip = "顾客栏立绘按其实际选中的源图生成 128/256 高清版(默认开,不参与捕获权重);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.keep_guest_names", TargetMod = FamilyMods.Iph,
                TargetKey = "keep_guest_names", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "阻止改游戏顾客名(立绘管理器)", Default = false,
                Tooltip = "阻止主 mod 覆盖游戏原版顾客的显示名字(默认关,不还原已改名字);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.keep_mod_guest_names", TargetMod = FamilyMods.Iph,
                TargetKey = "keep_mod_guest_names", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "阻止改模组顾客名(立绘管理器)", Default = false,
                Tooltip = "阻止主 mod 覆盖有模组来源记录的顾客名字(默认关,与上项独立);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.name_region", TargetMod = FamilyMods.Iph,
                TargetKey = "name_region", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "名字地区限制(立绘管理器)", Choices = new[] { "不限制（原版）", "仅日本", "仅中国", "仅中日韩" }, Default = "不限制（原版）",
                Tooltip = "主 mod 统一名字生成器的地区限制档位(默认不限制,只影响新建名字);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.local_portrait_chance", TargetMod = FamilyMods.Iph,
                TargetKey = "local_portrait_chance", Kind = PanelKind.SliderInt, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "文件夹立绘概率(立绘管理器)", MinInt = 0, MaxInt = 100, Default = 0,
                Tooltip = "体型未指定来源时按此概率 % 使用 Portraits 文件夹内 PNG(默认 0);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.weighted_capture_factions", TargetMod = FamilyMods.Iph,
                TargetKey = "weighted_capture_factions", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "按立绘数调捕获权重(立绘管理器)", Default = true,
                Tooltip = "默认开:按各阵营可抽取角色的总权重抽阵营;关闭时回退主 mod 阵营池抽取(主面板阵营池 CSV 那时仍有消费方);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.trace_portrait_replacements", TargetMod = FamilyMods.Iph,
                TargetKey = "trace_portrait_replacements", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "立绘替换追踪(立绘管理器)", Default = true,
                Tooltip = "监听各模组的立绘替换入口并记录实际输出,供 F7 管理器扫描(默认开);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.portrait_gender", TargetMod = FamilyMods.Iph,
                TargetKey = "portrait_gender", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "立绘性别范围(立绘管理器)", Choices = new[] { "仅女性（原版）", "不限性别" }, Default = "不限性别",
                Tooltip = "新生成物品在角色池中筛选的性别范围(默认不限);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.tool_gender", TargetMod = FamilyMods.Iph,
                TargetKey = "tool_gender", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "道具作用性别(立绘管理器)", Choices = new[] { "仅女性（原版）", "不限性别" }, Default = "不限性别",
                Tooltip = "不限性别时解除主 mod 触手精液对女性顾客的限制(默认不限,立即生效);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.customer_conversion", TargetMod = FamilyMods.Iph,
                TargetKey = "customer_conversion", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "顾客女性化(立绘管理器)", Choices = new[] { "原版行为", "保留游戏原分布" }, Default = "原版行为",
                Tooltip = "主 mod 会把部分普通男性顾客换成女性立绘,选保留原分布可跳过(默认原版行为);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.body_weights_csv", TargetMod = FamilyMods.Iph,
                TargetKey = "body_weights_csv", Kind = PanelKind.Input, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "成年体型权重 CSV(立绘管理器)", Default = "20, 20, 20, 20, 20",
                Tooltip = "成年体型抽取权重,逗号分隔,项序 萝莉,少女,御姐,少妇,熟女(默认全 20 = 均匀;0 = 不选该项,不能全 0;IPH 对权重 0 保留小概率均匀抽取——整串全 0 回退默认、抽签域内全 0 时改均匀抽取,非硬排除,IPH 设计;只调整这些体型内部的相对比例)。主面板体型池让位后体型分布的出口即此键;" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.body_affects_breast_size", TargetMod = FamilyMods.Iph,
                TargetKey = "body_affects_breast_size", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "体型影响乳量(立绘管理器)", Default = true,
                Tooltip = "默认开:新生成苗床按体型-乳量概率矩阵出乳量;关闭时恢复主 mod 原版三乳量各 1/3。主面板胸部池让位后乳量分布的出口即此键;" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.scavenger_double_virgin_csv", TargetMod = FamilyMods.Iph,
                TargetKey = "scavenger_double_virgin_csv", Kind = PanelKind.Input, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "体型双处女概率 CSV(立绘管理器)", Default = "60, 60, 60, 60, 60",
                Tooltip = "拾荒生成的成年体型双处女概率 %,逗号分隔,项序同上(默认全 60,每项独立 0-100,不要求合计 100);" + ThroughNote,
            },

            // ---- 物品高清立绘管理器联动·属性复刻(0.6.0 R10,2026-11-14:少女生成
            //      属性五键;Carrier = MainHook——postfix on 主 mod ItemFactory.
            //      CreateMaidenOfFaction,与 IPH 工厂 transpiler 共存(让位原则的
            //      例外形态:IPH 的 FactoryTranspiler 六处调用替换全部在选图/阵营/
            //      体型/乳量段[iph_130 OriginalCompat.cs:282/:289/:299,308/
            //      :314/:320/:326],属性掷骰段 RollAttr/MAIDEN_FORCE/INTEL/CHARM/
            //      AttrValue/unitValue 全文零引用——postfix 不短路原方法即不打断
            //      其挂钩链,transpiler 不改属性段即不与重掷冲突,正交共存)。
            //      TargetMod = IPH 走版本绑定:在场且版本匹配 → Own 装配;版本漂移
            //      → VersionBlocked 自动失效(用户裁定,防 IPH 未来接管属性段时
            //      冲突);缺席 → 分区不 Define、开关恒 false = Absent 零 hook。
            //      默认值 = 主 mod 1.7.1 ItemFactory.RollAttr 字面量逐字;TargetKey
            //      留空(本 mod 自有键,非写透)。全默认(含开关开但值=默认)→
            //      postfix 零参与不动产物 = 精确零漂移;任一键 ≠ 默认 → 独立重掷
            //      三属性 tag 并差量修正 unitValue(原文 :87 行,AttrValue 断点
            //      60/80 固定常量,主面板 create_maiden 先例)后直调主 mod
            //      UpdateMaidenDescription 刷新描述)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrHighChance, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.SliderFloat, Section = IphSection,
                Label = "高属性概率(立绘管理器在场)", MinFloat = 0f, MaxFloat = 1f, Default = 0.01f,
                Tooltip = "属性掷骰落进高档区间的概率(默认 1% = 原版;主面板同名键在立绘管理器在场时让位,本键经共存补丁继续生效,仅立绘管理器在场且版本匹配时可用);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrMidChance, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.SliderFloat, Section = IphSection,
                Label = "中属性概率(立绘管理器在场)", MinFloat = 0f, MaxFloat = 1f, Default = 0.06f,
                Tooltip = "属性掷骰落进中档区间的累计阈值(默认 6% = 原版,含高档;低档为其余);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrHighRange, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.Input, Section = IphSection,
                Label = "高属性区间 CSV(立绘管理器在场)", Default = "70,80",
                Tooltip = "高属性掷骰区间「下,上」(默认 70,80 = 原版,两端含入);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrMidRange, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.Input, Section = IphSection,
                Label = "中属性区间 CSV(立绘管理器在场)", Default = "60,70",
                Tooltip = "中属性掷骰区间「下,上」(默认 60,70 = 原版,两端含入);" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrLowRange, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.Input, Section = IphSection,
                Label = "低属性区间 CSV(立绘管理器在场)", Default = "10,60",
                Tooltip = "低属性掷骰区间「下,上」(默认 10,60 = 原版,两端含入);" + TakeoverNote,
            },

            // ---- 主 mod 官方设置写透(0.8.0;主 mod ModSettings.cs :91-93 的
            //      「拾荒」分区 3 键逐字镜像——默认值/范围 = 官方 Define 原值;
            //      写透 = 双面板并存:本面板开启后以本面板为准,关闭恢复开启前
            //      对方值;主 mod 官方面板同键改动经 Save 落 json,本面板读回
            //      一致;ScavMaidenBase 退役后此 3 键为拾荒概率唯一调参面)----
            new FamilyPanelEntry
            {
                OurKey = "main.maiden_loot_chance", TargetMod = FamilyMods.MainMod,
                TargetKey = "maiden_loot_chance", Kind = PanelKind.SliderFloat, Section = MainModSection, Carrier = CarrierKind.WriteThrough,
                Label = "拾荒抓苗床基准线", Default = 0.05f, MinFloat = 0.05f, MaxFloat = 0.25f,
                Tooltip = "拾荒抓苗床的概率基准线(官方键 maiden_loot_chance,默认 0.05)。周末/垃圾场事件/连续空手在它上面加成,手里苗床越多越难抓——实际概率见触手本体悬停;" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "main.maiden_enough_line", TargetMod = FamilyMods.MainMod,
                TargetKey = "maiden_enough_line", Kind = PanelKind.SliderInt, Section = MainModSection, Carrier = CarrierKind.WriteThrough,
                Label = "苗床充足线", Default = 12, MinInt = 2, MaxInt = 30,
                Tooltip = "手里的苗床少于这个数:掷 1 次;达到:掷 3 次要中 2 次;达到两倍:掷 3 次要全中(官方键 maiden_enough_line,默认 12);" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "main.ovulation_loot_chance", TargetMod = FamilyMods.MainMod,
                TargetKey = "ovulation_loot_chance", Kind = PanelKind.SliderFloat, Section = MainModSection, Carrier = CarrierKind.WriteThrough,
                Label = "捡到排卵诱发剂的概率", Default = 0.04f, MinFloat = 0f, MaxFloat = 0.1f,
                Tooltip = "带着触手本体拾荒、没抓到苗床时顺手捡到一支排卵诱发剂的概率,设 0 就只能靠触手3D打印机生产(官方键 ovulation_loot_chance,默认 0.04;垃圾场来源,与本 mod「掉落」分区医疗组掉落键来源不同、并存不冲突);" + ThroughNote,
            },
        };

        private static Dictionary<string, FamilyPanelEntry> index; // 惰性(HookRuntime 分发用)

        /// <summary>OurKey → 条目查找(HookRuntime 分发器的镜像域判定);未知键 false。</summary>
        internal static bool TryGet(string ourKey, out FamilyPanelEntry entry)
        {
            if (index == null)
            {
                var map = new Dictionary<string, FamilyPanelEntry>(Entries.Count, System.StringComparer.Ordinal);
                foreach (FamilyPanelEntry e in Entries) map.Add(e.OurKey, e);
                index = map;
            }
            return index.TryGetValue(ourKey, out entry);
        }
    }
}
