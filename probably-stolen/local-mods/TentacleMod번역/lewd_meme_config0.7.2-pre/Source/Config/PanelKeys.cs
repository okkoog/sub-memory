using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>面板控件形态(数值键保留 Slider 枚举承载 min/max 钳制元数据,
    /// Define 时统一走 string 文本框——NumericInput;Input = CSV 文本框;
    /// EnumKeyCode = KeyCode 枚举下拉[0.7.1 拾取器:游戏原生 DefineEnum&lt;KeyCode&gt;
    /// 通道,Brood 热键先例 Settings.cs:130;注册表层存 KeyCode 名串往返转换,
    /// 仅 FamilyPanelKeys 使用——主面板暂无此形态])。</summary>
    internal enum PanelKind { Toggle, SliderInt, SliderFloat, Choice, Input, EnumKeyCode }

    /// <summary>
    /// 一个玩家面板条目的定义(纯数据)。值条目(90,Task 14b +printer 5)语义同
    /// 0.4.0;开关条目(87 子项)是 0.5.0 开关体系的面板形态(Kind=Toggle,
    /// Key = 开关键名)。SectionId = 分区英文标识(开关体系映射用,每条目必填)。
    /// 默认值不在此重复——Define 时从 ConfigRegistry 同源读取(验收红线:三处同源)。
    /// </summary>
    internal sealed class PanelEntry
    {
        public string Key; public PanelKind Kind; public string Section; public string Label; public string Tooltip;
        public string SectionId;                              // 分区英文 id(开关映射)
        public int MinInt; public int MaxInt;                 // SliderInt(文本框钳制用)
        public float MinFloat; public float MaxFloat;         // SliderFloat(同上)
        public string[] Choices;                              // 仅 Choice(gender_scope)
    }

    /// <summary>
    /// 玩家面板键定义表(0.5.0):182 条目 = 93 值 + 89 子项开关,13 分区编译期固定
    /// (Task 14b 解除面板冻结:仅追加「打印机」分区 5 值 + 5 开关;loot.med_* 双键
    /// 随 1.7.1 适配重定位为「恢复 1.6.x 医疗组排卵针掉落」opt-in,默认关 = 1.9.2
    /// 原版零漂移;0.7.0 T8:virgin_chance 两行废弃;T9:exp_per_level 两行废弃,
    /// +成长公式三键 +互动阈值 +初始处女——max_charge/min 系放弃键等 tooltip
    /// 注记随 T9 落地)。完整性由 PanelKeysTests 锁定。总开关/分区开关已按作者裁定移除
    /// ——游戏面板是 Apply 模型(控件互相独立、程序化改其他开关无法刷新显示),
    /// 层级联动 UI 上无法实现;开关语义(spec §4.2 修订,0.6.0 三层取值链):每键
    /// 子开关默认关,只压玩家层(主 mod 面板动态层照常穿透、默认层恒可及——0.5.x
    /// 的 override 层已随服务面拆除删除),开关键自身豁免(ConfigRegistry.PlayerLayerAllowed)。
    /// 排除原则不变:9 个布局尺寸键不上面板(误改破坏主 mod 布局逻辑)。
    /// </summary>
    internal static class PanelKeys
    {
        // -- 分区英文 id(开关键与 SectionIdOf 映射的唯一事实源) --
        internal const string SecClient  = "client";
        internal const string SecPrice   = "price";
        internal const string SecNightly = "nightly";
        internal const string SecProgress= "progress";
        internal const string SecLoot    = "loot";
        internal const string SecMaiden  = "maiden";
        internal const string SecSpecial = "special";
        internal const string SecCorrupt = "corrupt";
        internal const string SecAlarm   = "alarm";
        internal const string SecTrap    = "trap";
        internal const string SecUi      = "ui";
        internal const string SecPerk    = "perk";
        internal const string SecPrinter = "printer";

        internal const string SectionSwitchSuffix = ".enabled";

        public static readonly IReadOnlyList<PanelEntry> Entries = new[]
        {

            // -- 顾客(7 值 + 5 子项 = 12)--
            new PanelEntry { Key = CfgKeys.GenderScope, Kind = PanelKind.Choice, Section = "손님", SectionId = SecClient,
                Label = "전원 여성 세계", Tooltip = "全女性世界档位(0=原版行为,详见档位说明)", Choices = PlayerChoices.Labels },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.FemaleRewriteChance), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 여성으로 변경할 확률", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.FemaleRewriteChance, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "여성으로 변경할 확률", Tooltip = "女性改写概率(档位“关闭”时的基础概率)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = CfgKeys.GuestDisplayEnabled, Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "특별 손님 메뉴 표시", Tooltip = "特殊顾客入口显示" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ClientPoolSize), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 손님 후보군 규모", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ClientPoolSize, Kind = PanelKind.SliderInt, Section = "손님", SectionId = SecClient,
                Label = "손님 후보군 규모", Tooltip = "每日随机顾客池规模", MinInt = 1, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenPriceMod), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 정액 매입가 보정", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SemenPriceMod, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "정액 매입가 보정", Tooltip = "精液买价修正 %(负=降价)", MinFloat = -100f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenBonusMod), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 정액 판매가 보너스", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SemenBonusMod, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "정액 판매가 보너스", Tooltip = "精液卖价加成 %", MinFloat = -100f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenRepMult), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 정액 평판 배율", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SemenRepMult, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "정액 평판 배율", Tooltip = "精液声望倍率", MinFloat = 0.1f, MaxFloat = 999f },

            // -- 价格(5 值 + 5 子项 = 10)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceMilk), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 유즙 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PriceMilk, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "유즙 판매가", Tooltip = "乳汁售价(remake / 巢穴扩展启用时此配置不生效:remake 接管乳汁产出,Brood 创建后重算价值)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceMilkCredit), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 유즙 음용 크레딧", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PriceMilkCredit, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "유즙 음용 크레딧", Tooltip = "乳汁饮用信用(remake / 巢穴扩展启用时此配置不生效)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceSemen), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 정액 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PriceSemen, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "정액 판매가", Tooltip = "精液售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceTentacleBox), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 촉수 상자 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PriceTentacleBox, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "촉수 상자 판매가", Tooltip = "触手箱售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceOvulationShot), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 배란 주사 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PriceOvulationShot, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "배란 주사 판매가", Tooltip = "排卵针售价", MinInt = 0, MaxInt = 999999 },

            // -- 夜间结算(24 值 + 24 子项 = 48)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexMinBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 하한 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SexMinBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 하한 (기본)", Tooltip = "每夜性交次数下限 = 基础 + (触手等级-1)×每级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexPerLevelMin), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 하한 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SexPerLevelMin, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 하한 (레벨당)", Tooltip = "性交次数下限的每触手等级增量(1.9.2 该数值已内联进上限常量,暂不可调)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexMaxBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 상한 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SexMaxBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 상한 (기본)", Tooltip = "每夜性交次数上限 = 基础 + (触手等级-1)×每级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexPerLevelMax), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 상한 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SexPerLevelMax, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 상한 (레벨당)", Tooltip = "性交次数上限的每触手等级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralMinBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 하한 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.OralMinBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 하한 (기본)", Tooltip = "每夜口交次数下限 = 基础 + (触手等级-1)×每级增量(1.9.2 该数值已内联/无条件,暂不可调)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralPerLevelMin), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 하한 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.OralPerLevelMin, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 하한 (레벨당)", Tooltip = "口交次数下限的每触手等级增量(1.9.2 该数值已内联进上限常量,暂不可调)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralMaxBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 상한 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.OralMaxBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 상한 (기본)", Tooltip = "每夜口交次数上限 = 基础 + (触手等级-1)×每级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralPerLevelMax), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 상한 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.OralPerLevelMax, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 상한 (레벨당)", Tooltip = "口交次数上限的每触手等级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalMinBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 하한 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AnalMinBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 하한 (기본)", Tooltip = "每夜肛交次数下限 = 基础 + (触手等级-1)×每级增量(1.9.2 该数值已内联/无条件,暂不可调)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalPerLevelMin), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 하한 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AnalPerLevelMin, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 하한 (레벨당)", Tooltip = "肛交次数下限的每触手等级增量(1.9.2 该数值已内联进上限常量,暂不可调)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalMaxBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 상한 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AnalMaxBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 상한 (기본)", Tooltip = "每夜肛交次数上限 = 基础 + (触手等级-1)×每级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalPerLevelMax), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 상한 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AnalPerLevelMax, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 상한 (레벨당)", Tooltip = "肛交次数上限的每触手等级增量", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OvulateChance), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 배란기 진입 확률", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.OvulateChance, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "배란기 진입 확률", Tooltip = "少女每晚进入排卵期的概率(默认 1/7)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ConceivePerSex), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 1회 임신 확률 상한", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ConceivePerSex, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "1회 임신 확률 상한", Tooltip = "受孕概率 = 次数×该值(clamp 0-1)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.GestationDays), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 임신 기간 (일)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.GestationDays, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "임신 기간 (일)", Tooltip = "妊娠天数", MinInt = 1, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.LactationCap), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 하루 유즙 분비 횟수 상한", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.LactationCap, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "하루 유즙 분비 횟수 상한", Tooltip = "少女每日泌乳次数上限", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MilkValueBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 유즙 단가 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.MilkValueBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "유즙 단가 (기본)", Tooltip = "乳汁售价 = 基础 + 触手等级×每级加成", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MilkValuePerLv), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 유즙 단가 (레벨당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.MilkValuePerLv, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "유즙 단가 (레벨당)", Tooltip = "乳汁单价的每触手等级加成", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenChanceBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 정액 생산 확률 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SemenChanceBase, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "정액 생산 확률 (기본)", Tooltip = "精液产出概率 = min(上限, 基础 + 调教值×每点)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenChancePerTrain), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 정액 생산 확률 (조교 수치당)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SemenChancePerTrain, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "정액 생산 확률 (조교 수치당)", Tooltip = "精液产出概率的每点调教值增量", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenChanceCap), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 정액 생산 확률 상한", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SemenChanceCap, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "정액 생산 확률 상한", Tooltip = "精液产出概率的上限", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.DaughterValue), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 딸 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.DaughterValue, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "딸 판매가", Tooltip = "产出的少女(女儿)售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpMultiplier), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 촉수 경험치 배율", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ExpMultiplier, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "촉수 경험치 배율", Tooltip = "触手经验获取倍率", MinFloat = 0.1f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrainTameThreshold), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 조교 복종 임계값", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrainTameThreshold, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "조교 복종 임계값", Tooltip = "调教值达到该阈值算驯服", MinInt = 0, MaxInt = 999999 },

            // -- 成长(5 值 + 5 子项 = 10;0.7.0 T9:exp_per_level CSV 两行废弃
            //    ——1.9.2 已删 EXP_NEED 静态数组,经验表公式化进 ExpNeedForLevel,
            //    由公式三键接管[默认 100/3/100 = 原版逐字];level_cap 面板范围
            //    收窄 1-999,语义注记更新:1.9.2 升级循环无上限拦截[10000 次
            //    防死循环],读取与显示经 GetBodyLevel clamp 按此值封顶)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.LevelCap), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨 상한", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.LevelCap, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨 상한", Tooltip = "触手等级上限(原版 5;升级不再被上限拦截,等级读取与显示按此值封顶;每级所需经验由下方公式三键决定)", MinInt = 1, MaxInt = 999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpBase), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨업 경험치 (기본)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ExpBase, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨업 경험치 (기본)", Tooltip = "每级升级所需经验 = 基础 + 档数×每档增量,档数 = (等级-1)÷分级步长向下取整(默认 100,原版公式逐字)", MinInt = 10, MaxInt = 1000 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpDivisor), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨업 경험치 (구간 길이)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ExpDivisor, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨업 경험치 (구간 길이)", Tooltip = "每 N 级升一档经验需求(默认 3:lv1-3 需 100,lv4-6 需 200,lv7-9 需 300)", MinInt = 1, MaxInt = 10 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpStep), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨업 경험치 (구간당 증가량)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ExpStep, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨업 경험치 (구간당 증가량)", Tooltip = "每升一档增加的升级经验(默认 100;设 0 = 恒为基础值的平曲线)", MinInt = 0, MaxInt = 1000 },

            // -- 掉落(5 值 + 5 子项 = 10;med 双键 Task 14b 重定位:1.7.0 起原版已
            //    删除医疗组排卵针掉落分支,两键改为「恢复 1.6.x 行为」的 opt-in,
            //    任一 med 开关键开启 = 复刻体前置医疗分支(1.6.1 语义),全关(默认)
            //    = 纯 1.9.2 原版零漂移,LootHook 1.9.2 零改动)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MedShotChance), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "복원: 의료 그룹 배란 주사 드롭 (1.7.0부터 기본 게임에서 제거됨)", Tooltip = "开启=在掉落判定前恢复 1.6.x 的医疗组排卵针掉落分支(概率用下方值);关闭=1.9.2 原版行为(医疗组不再掉排卵针)" },
            new PanelEntry { Key = CfgKeys.MedShotChance, Kind = PanelKind.SliderFloat, Section = "드롭", SectionId = SecLoot,
                Label = "의료 그룹 배란 주사 드롭 확률", Tooltip = "恢复开启时,医疗物资组掉排卵针的概率(1.6.x 原版 0.1);恢复关闭时无效;仅对组名含医疗关键词的掉落源生效(垃圾场拾荒组不触发,与 1.6.1 原版一致)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ScavMaidenBase), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "사용: 폐품 수집 중 소녀 발견 확률", Tooltip = "开启=本面板值接管主 mod 的拾荒概率读取(含暗流交易在场时;主 mod 官方面板的拾荒概率不再生效);关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ScavMaidenBase, Kind = PanelKind.SliderFloat, Section = "드롭", SectionId = SecLoot,
                Label = "폐품 수집 중 소녀 발견 확률", Tooltip = "拾荒遇少女概率(可超出主 mod 面板的 0.05–0.25 上限;原版 0.05,1=必捕获;开启后以本面板为准——暗流交易在场时经其保留的概率路径同样生效", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MedKeywords), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "복원: 의료 그룹 배란 주사 드롭 (사용자 지정 키워드)", Tooltip = "开启=恢复 1.6.x 的医疗组排卵针掉落分支并按下方关键词判定医疗组;关闭=1.9.2 原版行为" },
            new PanelEntry { Key = CfgKeys.MedKeywords, Kind = PanelKind.Input, Section = "드롭", SectionId = SecLoot,
                Label = "의료 그룹 키워드 (CSV)", Tooltip = "恢复开启时判定为医疗物资的物品组关键词,逗号分隔(1.6.x 默认 med,drug,pharma,chem);恢复关闭时无效;仅对组名含医疗关键词的掉落源生效(垃圾场拾荒组不触发,与 1.6.1 原版一致)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ScavGroups), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "사용: 폐품 수집 그룹명 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.ScavGroups, Kind = PanelKind.Input, Section = "드롭", SectionId = SecLoot,
                Label = "폐품 수집 그룹명 (CSV)", Tooltip = "触发拾荒判定的地形组名,逗号分隔(默认 dumpingGroundTG,dumpingGroundFreshTG)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.LootThrottleSeconds), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "사용: 드롭 최소 간격 (초)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.LootThrottleSeconds, Kind = PanelKind.SliderFloat, Section = "드롭", SectionId = SecLoot,
                Label = "드롭 최소 간격 (초)", Tooltip = "同组连续拾荒的最小间隔(0=禁用节流)", MinFloat = 0f, MaxFloat = 999f },

            // -- 少女生成(13 值 + 12 子项 = 25;T8:virgin_chance 两行废弃删除——
            //    1.9.2 无条件处女,数值段五键/faction_base 拆往 transpiler 单元后
            //    IPH 在场也生效[共存 Own],tooltip 不再注明立绘管理器不生效;
            //    池三键与 portrait_size 的让位注明保留[仍在 create_maiden 单元];
            //    T9:+互动阈值一对 +初始处女 Toggle[自开关反向分派,关闭=接管])--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrHighChance), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 높은 능력치 등장 확률", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AttrHighChance, Kind = PanelKind.SliderFloat, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "높은 능력치 등장 확률", Tooltip = "属性掷骰落进高档区间的概率(默认 1%;经主 mod 属性掷骰方法注入,物品立绘管理器在场同样生效;其分区的属性五键可同开,后处理恒胜)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrMidChance), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 중간 능력치 등장 확률", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AttrMidChance, Kind = PanelKind.SliderFloat, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "중간 능력치 등장 확률", Tooltip = "属性掷骰落进中档区间的累计阈值概率(默认 6%,含高档;经主 mod 属性掷骰方法注入,物品立绘管理器在场同样生效;其分区的属性五键可同开,后处理恒胜)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrHighRange), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 높은 능력치 범위 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AttrHighRange, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "높은 능력치 범위 (CSV)", Tooltip = "高属性掷骰区间「下,上」(默认 70,80;经主 mod 属性掷骰方法注入,物品立绘管理器在场同样生效;其分区的属性五键可同开,后处理恒胜)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrMidRange), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 중간 능력치 범위 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AttrMidRange, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "중간 능력치 범위 (CSV)", Tooltip = "中属性掷骰区间「下,上」(默认 60,70;经主 mod 属性掷骰方法注入,物品立绘管理器在场同样生效;其分区的属性五键可同开,后处理恒胜)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrLowRange), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 낮은 능력치 범위 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AttrLowRange, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "낮은 능력치 범위 (CSV)", Tooltip = "低属性掷骰区间「下,上」(默认 10,60;经主 mod 属性掷骰方法注入,物品立绘管理器在场同样生效;其分区的属性五键可同开,后处理恒胜)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.FactionBase), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 세력별 기본가 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.FactionBase, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "세력별 기본가 (CSV)", Tooltip = "阵营:基价 键值对,逗号分隔(默认 上层区:100,治安部:50,黑市:50,革命军:50,其他:10;经主 mod 基价方法注入,物品立绘管理器在场同样生效)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.InteractThreshold), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 상호작용 상한 능력치 임계값", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.InteractThreshold, Kind = PanelKind.SliderInt, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "상호작용 상한 능력치 임계값", Tooltip = "少女可互动次数上限 = 1 + 情感/信任/催眠/调教值各自达到该阈值的项数(默认 60,原版四阈值同源)", MinInt = 0, MaxInt = 100 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Factions), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 세력 후보군 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.Factions, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "세력 후보군 (CSV)", Tooltip = "随机阵营池,逗号分隔(默认 下层区,上层区,治安部,黑市,革命军;物品立绘管理器在场时不生效,默认其按立绘数量加权抽阵营)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Jobs), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 직업 후보군 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.Jobs, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "직업 후보군 (CSV)", Tooltip = "随机职业池,逗号分隔(默认 20 项,见主 mod)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Bodies), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 체형 후보군 (CSV)", Tooltip = "关闭=该值用原版默认(物品立绘管理器在场时该配置不生效)" },
            new PanelEntry { Key = CfgKeys.Bodies, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "체형 후보군 (CSV)", Tooltip = "随机体型池,逗号分隔(默认 萝莉,少女,御姐,少妇,熟女;物品立绘管理器在场时不生效,体型分布改由联动分区的「成年体型权重 CSV」接管)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Breasts), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 가슴 크기 후보군 (CSV)", Tooltip = "关闭=该值用原版默认(物品立绘管理器在场时该配置不生效)" },
            new PanelEntry { Key = CfgKeys.Breasts, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "가슴 크기 후보군 (CSV)", Tooltip = "随机胸部池,逗号分隔(默认 贫乳,普乳,巨乳;物品立绘管理器在场时不生效,乳量分布改由联动分区的「体型影响乳量」接管)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PortraitSize), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 초상화 크기", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PortraitSize, Kind = PanelKind.SliderInt, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "초상화 크기", Tooltip = "生成立绘的像素尺寸(默认 64;物品立绘管理器启用时不生效)", MinInt = 16, MaxInt = 999999 },
            new PanelEntry { Key = CfgKeys.InitialVirgin, Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "시작 시 처녀 여부", Tooltip = "开启(默认)= 生成的少女保持处女与肛处女(1.9.2 原版行为);关闭 = 生成的少女不再带处女标记(影响初夜溢价类身价与相关文案)" },

            // -- 特殊顾客与名字(4 值 + 3 子项 = 7)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SpecialExtraIds), Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용: 특별 손님 ID 추가 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SpecialExtraIds, Kind = PanelKind.Input, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "특별 손님 ID 추가 (CSV)", Tooltip = "追加进特殊顾客白名单的物品 id,逗号分隔(默认空=不追加)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SpecialExtraSprites), Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용: 특별 손님 초상화 추가 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SpecialExtraSprites, Kind = PanelKind.Input, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "특별 손님 초상화 추가 (CSV)", Tooltip = "追加进特殊顾客立绘池的路径,逗号分隔(默认空=不追加)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SpecialExtraKeywords), Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용: 특별 손님 키워드 추가 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.SpecialExtraKeywords, Kind = PanelKind.Input, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "특별 손님 키워드 추가 (CSV)", Tooltip = "命中特殊顾客判定的追加关键词,逗号分隔(默认空=不追加)" },
            new PanelEntry { Key = CfgKeys.NamesUseCustomFile, Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용자 지정 이름 목록 사용", Tooltip = "启用 ModData/lewd_meme_config/names_custom.txt 自定义名字库(从兼容层切换需手动复制该文件)" },

            // -- 侵蚀触手(7 值 + 7 子项 = 14)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptMaxCharge), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 충전량 상한", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptMaxCharge, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 충전량 상한", Tooltip = "侵蚀充能上限;上限>100 需同时启用『读取充能』『写入充能』两开关,否则充能实际封顶 100", MinInt = 10, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptInitialCharge), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 초기 충전량", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptInitialCharge, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "초기 충전량", Tooltip = "侵蚀触手创建时的初始充能(超出上限会被充能上限钳制;默认 100)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptDaily), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 하루 침식 충전 회복량", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptDaily, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "하루 침식 충전 회복량", Tooltip = "每日侵蚀回充", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptSemen), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 정액 사용 시 침식 충전 회복량", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptSemen, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "정액 사용 시 침식 충전 회복량", Tooltip = "精液侵蚀回充", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptCost), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 1회 소모량", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptCost, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 1회 소모량", Tooltip = "单次侵蚀消耗;消耗 > 淫力上限(侵蚀充能上限)时充能永远不够,侵蚀永不触发", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptWhitelist), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 허용 목록 추가 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptWhitelist, Kind = PanelKind.Input, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 허용 목록 추가 (CSV)", Tooltip = "可被侵蚀物品白名单的追加项,逗号分隔(默认空=不追加);名单仅在『启用:侵蚀白名单追加(CSV)』开关开启时生效(开启 = 装配侵蚀判定接管单元;开其他侵蚀开关不装配名单判定)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptBlacklist), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 차단 목록 추가 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.CorruptBlacklist, Kind = PanelKind.Input, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 차단 목록 추가 (CSV)", Tooltip = "不可侵蚀物品黑名单的追加项,逗号分隔(默认空=不追加);名单仅在『启用:侵蚀黑名单追加(CSV)』开关开启时生效(开启 = 装配侵蚀判定接管单元;开其他侵蚀开关不装配名单判定)" },

            // -- 闹钟(7 值 + 7 子项 = 14)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmPrice), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 알람 시계 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmPrice, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "알람 시계 판매가", Tooltip = "闹钟售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmTentaclePrice), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 촉수 알람 시계 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmTentaclePrice, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "촉수 알람 시계 판매가", Tooltip = "触手闹钟售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmNpcSellChance), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 알람 시계 판매 목록 등장 확률", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmNpcSellChance, Kind = PanelKind.SliderFloat, Section = "알람 시계", SectionId = SecAlarm,
                Label = "알람 시계 판매 목록 등장 확률", Tooltip = "闹钟被 NPC 上架的概率(全部路径)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmOralPerNight), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 밤마다 구강 성교 횟수", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmOralPerNight, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "밤마다 구강 성교 횟수", Tooltip = "闹钟每夜口交次数", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmSexPerNight), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 밤마다 성교 횟수", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmSexPerNight, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "밤마다 성교 횟수", Tooltip = "闹钟每夜性交次数", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmAnalPerNight), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 밤마다 항문 성교 횟수", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmAnalPerNight, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "밤마다 항문 성교 횟수", Tooltip = "闹钟每夜肛交次数", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmSemenOutput), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 정액 생산 병 수", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.AlarmSemenOutput, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "정액 생산 병 수", Tooltip = "闹钟每夜精液产出瓶数(1.9.2 产出无条件每苗床一瓶,暂不可调)", MinInt = 0, MaxInt = 999999 },

            // -- 陷阱(6 值 + 6 子项 = 12)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapPrice), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 맨홀 뚜껑 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrapPrice, Kind = PanelKind.SliderInt, Section = "함정", SectionId = SecTrap,
                Label = "맨홀 뚜껑 판매가", Tooltip = "井盖售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapTrapPrice), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 포획 함정 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrapTrapPrice, Kind = PanelKind.SliderInt, Section = "함정", SectionId = SecTrap,
                Label = "포획 함정 판매가", Tooltip = "捕捉陷阱售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapNpcSellChanceScav), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 맨홀 뚜껑 판매 목록 등장 확률 (폐품 수집)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrapNpcSellChanceScav, Kind = PanelKind.SliderFloat, Section = "함정", SectionId = SecTrap,
                Label = "맨홀 뚜껑 판매 목록 등장 확률 (폐품 수집)", Tooltip = "拾荒者买走井盖的上架概率", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapNpcSellChanceThief), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 맨홀 뚜껑 판매 목록 등장 확률 (도둑)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrapNpcSellChanceThief, Kind = PanelKind.SliderFloat, Section = "함정", SectionId = SecTrap,
                Label = "맨홀 뚜껑 판매 목록 등장 확률 (도둑)", Tooltip = "小偷买走井盖的上架概率", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapSemenChance), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 숙주 정액 생산 확률", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrapSemenChance, Kind = PanelKind.SliderFloat, Section = "함정", SectionId = SecTrap,
                Label = "숙주 정액 생산 확률", Tooltip = "陷阱苗床产精液概率", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapTacticChances), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 전략별 포획 확률 (CSV)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.TrapTacticChances, Kind = PanelKind.Input, Section = "함정", SectionId = SecTrap,
                Label = "전략별 포획 확률 (CSV)", Tooltip = "三档捕捉策略的捕捉概率,逗号分隔(默认 0.1,0.2,0.3)" },

            // -- 界面(4 值 + 4 子项 = 8)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReportWidth), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 야간 보고 창 너비", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.UiReportWidth, Kind = PanelKind.SliderInt, Section = "화면", SectionId = SecUi,
                Label = "야간 보고 창 너비", Tooltip = "夜报窗口宽", MinInt = 200, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReportHeight), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 야간 보고 창 높이", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.UiReportHeight, Kind = PanelKind.SliderInt, Section = "화면", SectionId = SecUi,
                Label = "야간 보고 창 높이", Tooltip = "夜报窗口高", MinInt = 120, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReportDelay), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 야간 보고 표시 지연 (초)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.UiReportDelay, Kind = PanelKind.SliderFloat, Section = "화면", SectionId = SecUi,
                Label = "야간 보고 표시 지연 (초)", Tooltip = "夜报弹出延迟(秒)", MinFloat = 0f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReloadFixDelay), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 불러오기 복구 지연 (초)", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.UiReloadFixDelay, Kind = PanelKind.SliderFloat, Section = "화면", SectionId = SecUi,
                Label = "불러오기 복구 지연 (초)", Tooltip = "读档修复延迟(秒)", MinFloat = 0f, MaxFloat = 999f },

            // -- 天赋(2 值 + 2 子项 = 4)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PerkDailyFemales), Kind = PanelKind.Toggle, Section = "특성", SectionId = SecPerk,
                Label = "사용: 촉수의 기원 하루 여성 손님 수", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PerkDailyFemales, Kind = PanelKind.SliderInt, Section = "특성", SectionId = SecPerk,
                Label = "촉수의 기원 하루 여성 손님 수", Tooltip = "触手起源每日女客数(0=关闭)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PerkScavBonus), Kind = PanelKind.Toggle, Section = "특성", SectionId = SecPerk,
                Label = "사용: 폐품 수집 특성 보너스", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PerkScavBonus, Kind = PanelKind.SliderInt, Section = "특성", SectionId = SecPerk,
                Label = "폐품 수집 특성 보너스", Tooltip = "拾荒天赋加成", MinInt = 0, MaxInt = 999999 },

            // -- 打印机(5 值 + 5 子项 = 10;主 mod 1.7.1 新增「触手3D打印机」线,
            //    Task 14b 接管——分区语境为主 mod 参数,非子 mod 联动;默认全关 =
            //    打印机线纯原版)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterPrice), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 촉수 프린터 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PrinterPrice, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "촉수 프린터 판매가", Tooltip = "触手3D打印机的售价(侵蚀 3D 打印机所得)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterNeedlePrice), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 빈 주사기 판매가", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PrinterNeedlePrice, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "빈 주사기 판매가", Tooltip = "打印机产出的空针管售价", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterGrid), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 프린터 구역 한 변의 칸 수", Tooltip = "关闭=该值用原版默认(仅影响新造打印机)" },
            new PanelEntry { Key = CfgKeys.PrinterGrid, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "프린터 구역 한 변의 칸 수", Tooltip = "材料/苗床/产出三区的方格边长(原版 12×12;仅影响新造的打印机,已放置的不变;小于 12 可能影响材料识别)", MinInt = 12, MaxInt = 64 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterRecipeCost), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 빈 주사기 필요 진행도", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PrinterRecipeCost, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "빈 주사기 필요 진행도", Tooltip = "每造一支空针管消耗的打印进度(每份耗材 + 触手精液抽取原料;原版 15)", MinInt = 1, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterBaseProgress), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 숙주 1명당 기본 진행도", Tooltip = "关闭=该值用原版默认" },
            new PanelEntry { Key = CfgKeys.PrinterBaseProgress, Kind = PanelKind.SliderFloat, Section = "프린터", SectionId = SecPrinter,
                Label = "숙주 1명당 기본 진행도", Tooltip = "苗床区每名少女每夜提供的基础打印进度(智力加成另算;原版 3)", MinFloat = 0f, MaxFloat = 999f },
        };

        // ---- 开关键命名与映射 API(Task 3/4/5 消费)----

        internal static string SectionSwitchKey(string sectionId) => sectionId + SectionSwitchSuffix;
        internal static string ItemSwitchKey(string key) => key + ".enabled";

        /// <summary>是否任一层开关键(master / section.enabled / key.enabled)。</summary>
        internal static bool IsSwitchKey(string key)
        {
            return key.EndsWith(SectionSwitchSuffix, System.StringComparison.Ordinal);
        }

        private static List<string> keysWithSwitch;            // 有子项开关的值键(惰性)

        /// <summary>配有子项开关的值键(87 个:排除 Choice/Toggle 3 键——它们
        /// 本身即开关语义,不受开关管辖;Task 14b +printer 5)。</summary>
        internal static IReadOnlyList<string> NumericKeysWithSwitch
        {
            get { BuildMapsIfNeeded(); return keysWithSwitch; }
        }

        private static void BuildMapsIfNeeded()
        {
            if (keysWithSwitch != null) return;
            var withSwitch = new List<string>();
            foreach (var e in Entries)
            {
                if (IsSwitchKey(e.Key)) continue;
                if (e.Kind == PanelKind.SliderInt || e.Kind == PanelKind.SliderFloat || e.Kind == PanelKind.Input)
                    withSwitch.Add(e.Key);
            }
            keysWithSwitch = withSwitch;
        }
    }
}
