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
                Label = "전원 여성 세계", Tooltip = "전원 여성 세계 단계(0=원본 게임 동작, 자세한 내용은 단계 설명 참조)", Choices = PlayerChoices.Labels },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.FemaleRewriteChance), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 여성으로 변경할 확률", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.FemaleRewriteChance, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "여성으로 변경할 확률", Tooltip = "여성으로 변경할 확률('끄기' 단계에서 적용하는 기본 확률)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = CfgKeys.GuestDisplayEnabled, Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "특별 손님 메뉴 표시", Tooltip = "특별 손님 메뉴의 표시 여부" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ClientPoolSize), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 손님 후보군 규모", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ClientPoolSize, Kind = PanelKind.SliderInt, Section = "손님", SectionId = SecClient,
                Label = "손님 후보군 규모", Tooltip = "매일 무작위로 선정되는 손님 후보군의 규모", MinInt = 1, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenPriceMod), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 정액 매입가 보정", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SemenPriceMod, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "정액 매입가 보정", Tooltip = "정액 매입가 보정 %(음수=가격 인하)", MinFloat = -100f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenBonusMod), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 정액 판매가 보너스", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SemenBonusMod, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "정액 판매가 보너스", Tooltip = "정액 판매가 추가 보너스 %", MinFloat = -100f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenRepMult), Kind = PanelKind.Toggle, Section = "손님", SectionId = SecClient,
                Label = "사용: 정액 평판 배율", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SemenRepMult, Kind = PanelKind.SliderFloat, Section = "손님", SectionId = SecClient,
                Label = "정액 평판 배율", Tooltip = "정액 평판 배율", MinFloat = 0.1f, MaxFloat = 999f },

            // -- 价格(5 值 + 5 子项 = 10)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceMilk), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 유즙 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PriceMilk, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "유즙 판매가", Tooltip = "유즙 판매가(remake / 둥지 확장 사용 중에는 적용되지 않음: remake가 유즙 생산을 담당하고, Brood가 생성 후 가치를 다시 계산함)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceMilkCredit), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 유즙 음용 크레딧", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PriceMilkCredit, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "유즙 음용 크레딧", Tooltip = "유즙 음용 시 획득 크레딧(remake / 둥지 확장 사용 중에는 적용되지 않음)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceSemen), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 정액 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PriceSemen, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "정액 판매가", Tooltip = "정액 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceTentacleBox), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 촉수 상자 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PriceTentacleBox, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "촉수 상자 판매가", Tooltip = "촉수 상자 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PriceOvulationShot), Kind = PanelKind.Toggle, Section = "가격", SectionId = SecPrice,
                Label = "사용: 배란 주사 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PriceOvulationShot, Kind = PanelKind.SliderInt, Section = "가격", SectionId = SecPrice,
                Label = "배란 주사 판매가", Tooltip = "배란 주사 판매가", MinInt = 0, MaxInt = 999999 },

            // -- 夜间结算(24 值 + 24 子项 = 48)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexMinBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 하한 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SexMinBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 하한 (기본)", Tooltip = "매일 밤 성교 횟수 하한 = 기본값 + (촉수 레벨-1)×레벨당 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexPerLevelMin), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 하한 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SexPerLevelMin, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 하한 (레벨당)", Tooltip = "촉수 레벨당 성교 횟수 하한 증가량(1.9.2에서는 이 값이 상한 상수에 포함되어 당분간 조정할 수 없음)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexMaxBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 상한 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SexMaxBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 상한 (기본)", Tooltip = "매일 밤 성교 횟수 상한 = 기본값 + (촉수 레벨-1)×레벨당 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SexPerLevelMax), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 성교 횟수 상한 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SexPerLevelMax, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "성교 횟수 상한 (레벨당)", Tooltip = "촉수 레벨당 성교 횟수 상한 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralMinBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 하한 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.OralMinBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 하한 (기본)", Tooltip = "매일 밤 구강 성교 횟수 하한 = 기본값 + (촉수 레벨-1)×레벨당 증가량(1.9.2에서는 값이 코드에 직접 포함되거나 무조건 적용되어 당분간 조정할 수 없음)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralPerLevelMin), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 하한 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.OralPerLevelMin, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 하한 (레벨당)", Tooltip = "촉수 레벨당 구강 성교 횟수 하한 증가량(1.9.2에서는 이 값이 상한 상수에 포함되어 당분간 조정할 수 없음)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralMaxBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 상한 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.OralMaxBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 상한 (기본)", Tooltip = "매일 밤 구강 성교 횟수 상한 = 기본값 + (촉수 레벨-1)×레벨당 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OralPerLevelMax), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 구강 성교 횟수 상한 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.OralPerLevelMax, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "구강 성교 횟수 상한 (레벨당)", Tooltip = "촉수 레벨당 구강 성교 횟수 상한 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalMinBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 하한 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AnalMinBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 하한 (기본)", Tooltip = "매일 밤 항문 성교 횟수 하한 = 기본값 + (촉수 레벨-1)×레벨당 증가량(1.9.2에서는 값이 코드에 직접 포함되거나 무조건 적용되어 당분간 조정할 수 없음)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalPerLevelMin), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 하한 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AnalPerLevelMin, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 하한 (레벨당)", Tooltip = "촉수 레벨당 항문 성교 횟수 하한 증가량(1.9.2에서는 이 값이 상한 상수에 포함되어 당분간 조정할 수 없음)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalMaxBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 상한 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AnalMaxBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 상한 (기본)", Tooltip = "매일 밤 항문 성교 횟수 상한 = 기본값 + (촉수 레벨-1)×레벨당 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AnalPerLevelMax), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 항문 성교 횟수 상한 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AnalPerLevelMax, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "항문 성교 횟수 상한 (레벨당)", Tooltip = "촉수 레벨당 항문 성교 횟수 상한 증가량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.OvulateChance), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 배란기 진입 확률", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.OvulateChance, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "배란기 진입 확률", Tooltip = "소녀가 매일 밤 배란기에 진입할 확률(기본값 1/7)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ConceivePerSex), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 1회 임신 확률 상한", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ConceivePerSex, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "1회 임신 확률 상한", Tooltip = "임신 확률 = 횟수×이 값(0~1 범위로 제한)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.GestationDays), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 임신 기간 (일)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.GestationDays, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "임신 기간 (일)", Tooltip = "임신 기간(일)", MinInt = 1, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.LactationCap), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 하루 유즙 분비 횟수 상한", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.LactationCap, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "하루 유즙 분비 횟수 상한", Tooltip = "소녀의 하루 유즙 분비 횟수 상한", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MilkValueBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 유즙 단가 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.MilkValueBase, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "유즙 단가 (기본)", Tooltip = "유즙 판매가 = 기본값 + 촉수 레벨×레벨당 추가 금액", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MilkValuePerLv), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 유즙 단가 (레벨당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.MilkValuePerLv, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "유즙 단가 (레벨당)", Tooltip = "촉수 레벨당 유즙 단가 추가 금액", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenChanceBase), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 정액 생산 확률 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SemenChanceBase, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "정액 생산 확률 (기본)", Tooltip = "정액 생산 확률 = min(상한, 기본값 + 조교 수치×수치당 증가량)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenChancePerTrain), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 정액 생산 확률 (조교 수치당)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SemenChancePerTrain, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "정액 생산 확률 (조교 수치당)", Tooltip = "조교 수치 1당 정액 생산 확률 증가량", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SemenChanceCap), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 정액 생산 확률 상한", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SemenChanceCap, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "정액 생산 확률 상한", Tooltip = "정액 생산 확률의 상한", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.DaughterValue), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 딸 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.DaughterValue, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "딸 판매가", Tooltip = "생산된 소녀(딸)의 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpMultiplier), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 촉수 경험치 배율", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ExpMultiplier, Kind = PanelKind.SliderFloat, Section = "야간 정산", SectionId = SecNightly,
                Label = "촉수 경험치 배율", Tooltip = "촉수 경험치 획득 배율", MinFloat = 0.1f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrainTameThreshold), Kind = PanelKind.Toggle, Section = "야간 정산", SectionId = SecNightly,
                Label = "사용: 조교 복종 임계값", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrainTameThreshold, Kind = PanelKind.SliderInt, Section = "야간 정산", SectionId = SecNightly,
                Label = "조교 복종 임계값", Tooltip = "조교 수치가 이 임계값에 도달하면 복종한 것으로 판정", MinInt = 0, MaxInt = 999999 },

            // -- 成长(5 值 + 5 子项 = 10;0.7.0 T9:exp_per_level CSV 两行废弃
            //    ——1.9.2 已删 EXP_NEED 静态数组,经验表公式化进 ExpNeedForLevel,
            //    由公式三键接管[默认 100/3/100 = 原版逐字];level_cap 面板范围
            //    收窄 1-999,语义注记更新:1.9.2 升级循环无上限拦截[10000 次
            //    防死循环],读取与显示经 GetBodyLevel clamp 按此值封顶)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.LevelCap), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨 상한", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.LevelCap, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨 상한", Tooltip = "촉수 레벨 상한(원본 5; 레벨업 자체는 상한으로 차단하지 않고 레벨 조회와 표시만 이 값으로 제한함; 레벨별 필요 경험치는 아래 수식의 설정 3개로 결정)", MinInt = 1, MaxInt = 999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpBase), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨업 경험치 (기본)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ExpBase, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨업 경험치 (기본)", Tooltip = "레벨업에 필요한 경험치 = 기본값 + 구간 수×구간당 증가량, 구간 수 = (레벨-1)÷구간 길이의 내림값(기본 100, 원본 수식과 동일)", MinInt = 10, MaxInt = 1000 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpDivisor), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨업 경험치 (구간 길이)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ExpDivisor, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨업 경험치 (구간 길이)", Tooltip = "N레벨마다 필요 경험치 구간이 한 단계 상승(기본 3: lv1-3은 100, lv4-6은 200, lv7-9는 300 필요)", MinInt = 1, MaxInt = 10 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ExpStep), Kind = PanelKind.Toggle, Section = "성장", SectionId = SecProgress,
                Label = "사용: 레벨업 경험치 (구간당 증가량)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ExpStep, Kind = PanelKind.SliderInt, Section = "성장", SectionId = SecProgress,
                Label = "레벨업 경험치 (구간당 증가량)", Tooltip = "구간이 한 단계 올라갈 때 추가되는 레벨업 경험치(기본 100; 0으로 설정하면 항상 기본값으로 고정되는 평탄한 곡선)", MinInt = 0, MaxInt = 1000 },

            // -- 掉落(5 值 + 5 子项 = 10;med 双键 Task 14b 重定位:1.7.0 起原版已
            //    删除医疗组排卵针掉落分支,两键改为「恢复 1.6.x 行为」的 opt-in,
            //    任一 med 开关键开启 = 复刻体前置医疗分支(1.6.1 语义),全关(默认)
            //    = 纯 1.9.2 原版零漂移,LootHook 1.9.2 零改动)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MedShotChance), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "복원: 의료 그룹 배란 주사 드롭 (1.7.0부터 기본 게임에서 제거됨)", Tooltip = "켜기=드롭 판정 전에 1.6.x의 의료 그룹 배란 주사 드롭 분기를 복원(확률은 아래 값 사용); 끄기=1.9.2 원본 동작(의료 그룹에서 배란 주사가 더는 드롭되지 않음)" },
            new PanelEntry { Key = CfgKeys.MedShotChance, Kind = PanelKind.SliderFloat, Section = "드롭", SectionId = SecLoot,
                Label = "의료 그룹 배란 주사 드롭 확률", Tooltip = "복원 기능이 켜졌을 때 의료 물자 그룹에서 배란 주사가 드롭될 확률(1.6.x 원본 0.1); 복원 기능이 꺼지면 무효. 그룹명에 의료 키워드가 포함된 드롭 출처에만 적용(쓰레기장 폐품 수집 그룹 제외, 1.6.1 원본과 동일)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ScavMaidenBase), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "사용: 폐품 수집 중 소녀 발견 확률", Tooltip = "켜기=이 패널의 값이 본편 모드의 폐품 수집 확률 읽기를 대신함(암거래 모드 사용 중에도 적용되며, 본편 공식 패널의 폐품 수집 확률은 더는 적용되지 않음); 끄기=원본 기본값 사용" },
            new PanelEntry { Key = CfgKeys.ScavMaidenBase, Kind = PanelKind.SliderFloat, Section = "드롭", SectionId = SecLoot,
                Label = "폐품 수집 중 소녀 발견 확률", Tooltip = "폐품 수집 중 소녀 발견 확률(본편 모드 패널의 0.05~0.25 상한을 초과하도록 설정 가능; 원본 0.05, 1=반드시 포획; 켜면 이 패널의 값을 우선하며 암거래 모드 사용 중에도 암거래가 유지하는 확률 경로를 통해 똑같이 적용됨)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.MedKeywords), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "복원: 의료 그룹 배란 주사 드롭 (사용자 지정 키워드)", Tooltip = "켜기=1.6.x의 의료 그룹 배란 주사 드롭 분기를 복원하고 아래 키워드로 의료 그룹을 판정; 끄기=1.9.2 원본 동작" },
            new PanelEntry { Key = CfgKeys.MedKeywords, Kind = PanelKind.Input, Section = "드롭", SectionId = SecLoot,
                Label = "의료 그룹 키워드 (CSV)", Tooltip = "복원 기능을 켰을 때 의료 물자 그룹으로 판정할 아이템 그룹 키워드. 쉼표로 구분(1.6.x 기본값 med,drug,pharma,chem); 복원을 끄면 무효. 그룹명에 의료 키워드가 포함된 드롭 출처에만 적용(쓰레기장 폐품 수집 그룹 제외, 1.6.1 원본과 동일)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.ScavGroups), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "사용: 폐품 수집 그룹명 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.ScavGroups, Kind = PanelKind.Input, Section = "드롭", SectionId = SecLoot,
                Label = "폐품 수집 그룹명 (CSV)", Tooltip = "폐품 수집 판정을 발동시키는 지형 그룹명. 쉼표로 구분(기본값 dumpingGroundTG,dumpingGroundFreshTG)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.LootThrottleSeconds), Kind = PanelKind.Toggle, Section = "드롭", SectionId = SecLoot,
                Label = "사용: 드롭 최소 간격 (초)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.LootThrottleSeconds, Kind = PanelKind.SliderFloat, Section = "드롭", SectionId = SecLoot,
                Label = "드롭 최소 간격 (초)", Tooltip = "같은 그룹에서 연속으로 폐품을 수집할 때 필요한 최소 간격(0=빈도 제한 해제)", MinFloat = 0f, MaxFloat = 999f },

            // -- 少女生成(13 值 + 12 子项 = 25;T8:virgin_chance 两行废弃删除——
            //    1.9.2 无条件处女,数值段五键/faction_base 拆往 transpiler 单元后
            //    IPH 在场也生效[共存 Own],tooltip 不再注明立绘管理器不生效;
            //    池三键与 portrait_size 的让位注明保留[仍在 create_maiden 单元];
            //    T9:+互动阈值一对 +初始处女 Toggle[自开关反向分派,关闭=接管])--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrHighChance), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 높은 능력치 등장 확률", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AttrHighChance, Kind = PanelKind.SliderFloat, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "높은 능력치 등장 확률", Tooltip = "능력치 무작위 판정이 상위 구간에 들어갈 확률(기본 1%; 본편 모드의 능력치 판정 메서드에 주입되며 초상화 관리자 사용 중에도 적용됨; 관리자 쪽 능력치 설정 5개와 동시에 켜면 후처리인 이 값이 항상 우선)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrMidChance), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 중간 능력치 등장 확률", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AttrMidChance, Kind = PanelKind.SliderFloat, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "중간 능력치 등장 확률", Tooltip = "능력치 무작위 판정이 중간 구간에 들어갈 누적 임계 확률(기본 6%, 상위 구간 포함; 본편 모드의 능력치 판정 메서드에 주입되며 초상화 관리자 사용 중에도 적용됨; 관리자 쪽 능력치 설정 5개와 동시에 켜면 후처리인 이 값이 항상 우선)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrHighRange), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 높은 능력치 범위 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AttrHighRange, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "높은 능력치 범위 (CSV)", Tooltip = "상위 능력치 판정 구간 '하한,상한'(기본 70,80; 본편 모드의 능력치 판정 메서드에 주입되며 초상화 관리자 사용 중에도 적용됨; 관리자 쪽 능력치 설정 5개와 동시에 켜면 후처리인 이 값이 항상 우선)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrMidRange), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 중간 능력치 범위 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AttrMidRange, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "중간 능력치 범위 (CSV)", Tooltip = "중간 능력치 판정 구간 '하한,상한'(기본 60,70; 본편 모드의 능력치 판정 메서드에 주입되며 초상화 관리자 사용 중에도 적용됨; 관리자 쪽 능력치 설정 5개와 동시에 켜면 후처리인 이 값이 항상 우선)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AttrLowRange), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 낮은 능력치 범위 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AttrLowRange, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "낮은 능력치 범위 (CSV)", Tooltip = "하위 능력치 판정 구간 '하한,상한'(기본 10,60; 본편 모드의 능력치 판정 메서드에 주입되며 초상화 관리자 사용 중에도 적용됨; 관리자 쪽 능력치 설정 5개와 동시에 켜면 후처리인 이 값이 항상 우선)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.FactionBase), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 세력별 기본가 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.FactionBase, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "세력별 기본가 (CSV)", Tooltip = "세력:기본가 형식의 키-값 쌍. 쉼표로 구분(기본값 上层区:100,治安部:50,黑市:50,革命军:50,其他:10; 본편 모드의 기본가 메서드에 주입되며 초상화 관리자 사용 중에도 적용됨)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.InteractThreshold), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 상호작용 상한 능력치 임계값", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.InteractThreshold, Kind = PanelKind.SliderInt, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "상호작용 상한 능력치 임계값", Tooltip = "소녀의 상호작용 횟수 상한 = 1 + 감정/신뢰/최면/조교 수치가 각각 이 임계값에 도달한 항목 수(기본 60, 원본에서 네 임계값은 모두 동일한 값 사용)", MinInt = 0, MaxInt = 100 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Factions), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 세력 후보군 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.Factions, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "세력 후보군 (CSV)", Tooltip = "무작위 세력 후보군. 쉼표로 구분(기본값 下层区,上层区,治安部,黑市,革命军; 초상화 관리자 사용 중에는 적용되지 않으며 관리자는 초상화 수에 가중치를 둬 세력을 추첨함)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Jobs), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 직업 후보군 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.Jobs, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "직업 후보군 (CSV)", Tooltip = "무작위 직업 후보군. 쉼표로 구분(기본 20개 항목, 본편 모드 참조)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Bodies), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 체형 후보군 (CSV)", Tooltip = "끄면 이 값은 원본 기본값을 사용합니다(초상화 관리자 사용 중에는 이 설정이 적용되지 않음)." },
            new PanelEntry { Key = CfgKeys.Bodies, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "체형 후보군 (CSV)", Tooltip = "무작위 체형 후보군. 쉼표로 구분(기본값 萝莉,少女,御姐,少妇,熟女; 초상화 관리자 사용 중에는 적용되지 않으며, 체형 분포는 연동 설정의 '성인 체형 가중치 CSV'가 담당함)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.Breasts), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 가슴 크기 후보군 (CSV)", Tooltip = "끄면 이 값은 원본 기본값을 사용합니다(초상화 관리자 사용 중에는 이 설정이 적용되지 않음)." },
            new PanelEntry { Key = CfgKeys.Breasts, Kind = PanelKind.Input, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "가슴 크기 후보군 (CSV)", Tooltip = "무작위 가슴 크기 후보군. 쉼표로 구분(기본값 贫乳,普乳,巨乳; 초상화 관리자 사용 중에는 적용되지 않으며, 가슴 크기 분포는 연동 설정의 '체형에 따른 유즙량'이 담당함)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PortraitSize), Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "사용: 초상화 크기", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PortraitSize, Kind = PanelKind.SliderInt, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "초상화 크기", Tooltip = "생성하는 초상화의 픽셀 크기(기본 64; 초상화 관리자 사용 중에는 적용되지 않음)", MinInt = 16, MaxInt = 999999 },
            new PanelEntry { Key = CfgKeys.InitialVirgin, Kind = PanelKind.Toggle, Section = "소녀 생성", SectionId = SecMaiden,
                Label = "시작 시 처녀 여부", Tooltip = "켜기(기본값)=생성된 소녀에게 처녀와 항문 처녀 상태가 유지됨(1.9.2 원본 동작); 끄기=생성된 소녀에게 처녀 상태 표식을 더는 부여하지 않음(첫날밤 프리미엄 관련 판매가와 설명에 영향)" },

            // -- 特殊顾客与名字(4 值 + 3 子项 = 7)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SpecialExtraIds), Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용: 특별 손님 ID 추가 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SpecialExtraIds, Kind = PanelKind.Input, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "특별 손님 ID 추가 (CSV)", Tooltip = "특별 손님 허용 목록에 추가할 아이템 ID. 쉼표로 구분(기본값 비어 있음=추가 안 함)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SpecialExtraSprites), Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용: 특별 손님 초상화 추가 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SpecialExtraSprites, Kind = PanelKind.Input, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "특별 손님 초상화 추가 (CSV)", Tooltip = "특별 손님 초상화 후보군에 추가할 경로. 쉼표로 구분(기본값 비어 있음=추가 안 함)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.SpecialExtraKeywords), Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용: 특별 손님 키워드 추가 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.SpecialExtraKeywords, Kind = PanelKind.Input, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "특별 손님 키워드 추가 (CSV)", Tooltip = "특별 손님 판정에 추가로 일치시킬 키워드. 쉼표로 구분(기본값 비어 있음=추가 안 함)" },
            new PanelEntry { Key = CfgKeys.NamesUseCustomFile, Kind = PanelKind.Toggle, Section = "특별 손님 및 이름", SectionId = SecSpecial,
                Label = "사용자 지정 이름 목록 사용", Tooltip = "ModData/lewd_meme_config/names_custom.txt에 저장된 사용자 지정 이름 목록을 사용(호환 계층에서 전환하는 경우 이 파일을 수동으로 복사해야 함)" },

            // -- 侵蚀触手(7 值 + 7 子项 = 14)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptMaxCharge), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 충전량 상한", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptMaxCharge, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 충전량 상한", Tooltip = "침식 충전량 상한. 상한을 100보다 크게 설정하려면 '충전량 읽기'와 '충전량 쓰기' 스위치를 모두 켜야 함. 그렇지 않으면 실제 충전량은 100에서 제한됨", MinInt = 10, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptInitialCharge), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 초기 충전량", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptInitialCharge, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "초기 충전량", Tooltip = "침식 촉수를 생성할 때의 초기 충전량(상한을 초과하면 충전량 상한으로 제한; 기본 100)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptDaily), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 하루 침식 충전 회복량", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptDaily, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "하루 침식 충전 회복량", Tooltip = "하루마다 회복하는 침식 충전량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptSemen), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 정액 사용 시 침식 충전 회복량", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptSemen, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "정액 사용 시 침식 충전 회복량", Tooltip = "정액 사용 시 회복하는 침식 충전량", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptCost), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 1회 소모량", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptCost, Kind = PanelKind.SliderInt, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 1회 소모량", Tooltip = "침식 1회 소모량. 소모량이 음력 상한(침식 충전량 상한)보다 크면 충전량이 항상 부족해 침식이 절대 발동하지 않음", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptWhitelist), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 허용 목록 추가 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptWhitelist, Kind = PanelKind.Input, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 허용 목록 추가 (CSV)", Tooltip = "침식할 수 있는 아이템 허용 목록의 추가 항목. 쉼표로 구분(기본값 비어 있음=추가 안 함); '사용: 침식 허용 목록 추가(CSV)' 스위치를 켜야 목록이 적용됨(켜기=침식 판정 교체 모듈 설치; 다른 침식 스위치만 켜서는 목록 판정 모듈이 설치되지 않음)" },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.CorruptBlacklist), Kind = PanelKind.Toggle, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "사용: 침식 차단 목록 추가 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.CorruptBlacklist, Kind = PanelKind.Input, Section = "침식 촉수", SectionId = SecCorrupt,
                Label = "침식 차단 목록 추가 (CSV)", Tooltip = "침식할 수 없는 아이템 차단 목록의 추가 항목. 쉼표로 구분(기본값 비어 있음=추가 안 함); '사용: 침식 차단 목록 추가(CSV)' 스위치를 켜야 목록이 적용됨(켜기=침식 판정 교체 모듈 설치; 다른 침식 스위치만 켜서는 목록 판정 모듈이 설치되지 않음)" },

            // -- 闹钟(7 值 + 7 子项 = 14)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmPrice), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 알람 시계 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmPrice, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "알람 시계 판매가", Tooltip = "알람 시계 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmTentaclePrice), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 촉수 알람 시계 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmTentaclePrice, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "촉수 알람 시계 판매가", Tooltip = "촉수 알람 시계 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmNpcSellChance), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 알람 시계 판매 목록 등장 확률", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmNpcSellChance, Kind = PanelKind.SliderFloat, Section = "알람 시계", SectionId = SecAlarm,
                Label = "알람 시계 판매 목록 등장 확률", Tooltip = "NPC가 알람 시계를 판매 목록에 올릴 확률(모든 경로)", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmOralPerNight), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 밤마다 구강 성교 횟수", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmOralPerNight, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "밤마다 구강 성교 횟수", Tooltip = "알람 시계의 매일 밤 구강 성교 횟수", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmSexPerNight), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 밤마다 성교 횟수", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmSexPerNight, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "밤마다 성교 횟수", Tooltip = "알람 시계의 매일 밤 성교 횟수", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmAnalPerNight), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 밤마다 항문 성교 횟수", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmAnalPerNight, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "밤마다 항문 성교 횟수", Tooltip = "알람 시계의 매일 밤 항문 성교 횟수", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.AlarmSemenOutput), Kind = PanelKind.Toggle, Section = "알람 시계", SectionId = SecAlarm,
                Label = "사용: 정액 생산 병 수", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.AlarmSemenOutput, Kind = PanelKind.SliderInt, Section = "알람 시계", SectionId = SecAlarm,
                Label = "정액 생산 병 수", Tooltip = "알람 시계의 매일 밤 정액 생산 병 수(1.9.2에서는 조건 없이 숙주 1명당 1병 생산하므로 당분간 조정 불가)", MinInt = 0, MaxInt = 999999 },

            // -- 陷阱(6 值 + 6 子项 = 12)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapPrice), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 맨홀 뚜껑 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrapPrice, Kind = PanelKind.SliderInt, Section = "함정", SectionId = SecTrap,
                Label = "맨홀 뚜껑 판매가", Tooltip = "맨홀 뚜껑 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapTrapPrice), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 포획 함정 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrapTrapPrice, Kind = PanelKind.SliderInt, Section = "함정", SectionId = SecTrap,
                Label = "포획 함정 판매가", Tooltip = "포획 함정 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapNpcSellChanceScav), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 맨홀 뚜껑 판매 목록 등장 확률 (폐품 수집)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrapNpcSellChanceScav, Kind = PanelKind.SliderFloat, Section = "함정", SectionId = SecTrap,
                Label = "맨홀 뚜껑 판매 목록 등장 확률 (폐품 수집)", Tooltip = "폐품 수집상이 맨홀 뚜껑을 사 갈 때의 판매 목록 등장 확률", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapNpcSellChanceThief), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 맨홀 뚜껑 판매 목록 등장 확률 (도둑)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrapNpcSellChanceThief, Kind = PanelKind.SliderFloat, Section = "함정", SectionId = SecTrap,
                Label = "맨홀 뚜껑 판매 목록 등장 확률 (도둑)", Tooltip = "도둑이 맨홀 뚜껑을 사 갈 때의 판매 목록 등장 확률", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapSemenChance), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 숙주 정액 생산 확률", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrapSemenChance, Kind = PanelKind.SliderFloat, Section = "함정", SectionId = SecTrap,
                Label = "숙주 정액 생산 확률", Tooltip = "함정 속 숙주의 정액 생산 확률", MinFloat = 0f, MaxFloat = 1f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.TrapTacticChances), Kind = PanelKind.Toggle, Section = "함정", SectionId = SecTrap,
                Label = "사용: 전략별 포획 확률 (CSV)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.TrapTacticChances, Kind = PanelKind.Input, Section = "함정", SectionId = SecTrap,
                Label = "전략별 포획 확률 (CSV)", Tooltip = "포획 전략 3단계의 포획 확률. 쉼표로 구분(기본값 0.1,0.2,0.3)" },

            // -- 界面(4 值 + 4 子项 = 8)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReportWidth), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 야간 보고 창 너비", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.UiReportWidth, Kind = PanelKind.SliderInt, Section = "화면", SectionId = SecUi,
                Label = "야간 보고 창 너비", Tooltip = "야간 보고 창 너비", MinInt = 200, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReportHeight), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 야간 보고 창 높이", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.UiReportHeight, Kind = PanelKind.SliderInt, Section = "화면", SectionId = SecUi,
                Label = "야간 보고 창 높이", Tooltip = "야간 보고 창 높이", MinInt = 120, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReportDelay), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 야간 보고 표시 지연 (초)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.UiReportDelay, Kind = PanelKind.SliderFloat, Section = "화면", SectionId = SecUi,
                Label = "야간 보고 표시 지연 (초)", Tooltip = "야간 보고 팝업 지연 시간(초)", MinFloat = 0f, MaxFloat = 999f },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.UiReloadFixDelay), Kind = PanelKind.Toggle, Section = "화면", SectionId = SecUi,
                Label = "사용: 불러오기 복구 지연 (초)", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.UiReloadFixDelay, Kind = PanelKind.SliderFloat, Section = "화면", SectionId = SecUi,
                Label = "불러오기 복구 지연 (초)", Tooltip = "저장 파일 불러오기 시 복구 지연 시간(초)", MinFloat = 0f, MaxFloat = 999f },

            // -- 天赋(2 值 + 2 子项 = 4)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PerkDailyFemales), Kind = PanelKind.Toggle, Section = "특성", SectionId = SecPerk,
                Label = "사용: 촉수의 기원 하루 여성 손님 수", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PerkDailyFemales, Kind = PanelKind.SliderInt, Section = "특성", SectionId = SecPerk,
                Label = "촉수의 기원 하루 여성 손님 수", Tooltip = "촉수의 기원에서 매일 등장하는 여성 손님 수(0=끄기)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PerkScavBonus), Kind = PanelKind.Toggle, Section = "특성", SectionId = SecPerk,
                Label = "사용: 폐품 수집 특성 보너스", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PerkScavBonus, Kind = PanelKind.SliderInt, Section = "특성", SectionId = SecPerk,
                Label = "폐품 수집 특성 보너스", Tooltip = "폐품 수집 특성 보너스", MinInt = 0, MaxInt = 999999 },

            // -- 打印机(5 值 + 5 子项 = 10;主 mod 1.7.1 新增「触手3D打印机」线,
            //    Task 14b 接管——分区语境为主 mod 参数,非子 mod 联动;默认全关 =
            //    打印机线纯原版)--
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterPrice), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 촉수 프린터 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PrinterPrice, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "촉수 프린터 판매가", Tooltip = "촉수 3D 프린터 판매가(침식 3D 프린터로 획득한 제품)", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterNeedlePrice), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 빈 주사기 판매가", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PrinterNeedlePrice, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "빈 주사기 판매가", Tooltip = "프린터에서 생산한 빈 주사기 판매가", MinInt = 0, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterGrid), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 프린터 구역 한 변의 칸 수", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다(새로 만든 프린터에만 영향)." },
            new PanelEntry { Key = CfgKeys.PrinterGrid, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "프린터 구역 한 변의 칸 수", Tooltip = "재료/숙주/생산물 세 구역의 칸 한 변 길이(원본 12×12; 새로 만든 프린터에만 적용되며 설치된 프린터는 바뀌지 않음; 12보다 작으면 재료 인식에 영향을 줄 수 있음)", MinInt = 12, MaxInt = 64 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterRecipeCost), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 빈 주사기 필요 진행도", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PrinterRecipeCost, Kind = PanelKind.SliderInt, Section = "프린터", SectionId = SecPrinter,
                Label = "빈 주사기 필요 진행도", Tooltip = "빈 주사기 1개 생산에 필요한 프린터 진행도(소모품 1개분 + 촉수 정액을 원료로 추출; 원본 15)", MinInt = 1, MaxInt = 999999 },
            new PanelEntry { Key = ItemSwitchKey(CfgKeys.PrinterBaseProgress), Kind = PanelKind.Toggle, Section = "프린터", SectionId = SecPrinter,
                Label = "사용: 숙주 1명당 기본 진행도", Tooltip = "끄면 이 값에는 원본 기본값이 적용됩니다." },
            new PanelEntry { Key = CfgKeys.PrinterBaseProgress, Kind = PanelKind.SliderFloat, Section = "프린터", SectionId = SecPrinter,
                Label = "숙주 1명당 기본 진행도", Tooltip = "숙주 구역의 소녀 1명이 매일 밤 제공하는 기본 프린터 진행도(지능 보너스는 별도 계산; 원본 3)", MinFloat = 0f, MaxFloat = 999f },
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
