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
                Label = "피로 시스템 (암거래)", Choices = OnOffChoices, Default = "开",
                Tooltip = "암거래 피로 시스템 스위치(켜기=원본의 실제 활동 횟수에 따라 피로 누적; 밤이 시작될 때 과로 상태이면 해당 캐릭터의 모든 야간 활동, 양쪽의 생산, 임신 및 임신 기간 진행을 중단하고 피로 5 회복; 끄기=피로 보정과 활동 제한 모두 비활성화)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.fatigue_scale", TargetMod = FamilyMods.Remake,
                TargetKey = "fatigue_scale", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "피로 누적 속도 (암거래)", Choices = PercentChoices, Default = "100%",
                Tooltip = "암거래 피로 증가 속도 배율 단계(기본 100%; 증가 속도만 조정하며 원본 활동 횟수는 변경하지 않음)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.buyer_per_day", TargetMod = FamilyMods.Remake,
                TargetKey = "buyer_per_day", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "하루 매입상 수 상한 (암거래)", Choices = BuyerChoices, Default = "1",
                Tooltip = "암거래에서 하루에 방문할 수 있는 숙주 매입상 최대 인원(기본 1)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.corruption_box_settlement", TargetMod = FamilyMods.Remake,
                TargetKey = "corruption_box_settlement", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "침식 구역 숙주 상자 야간 정산 (암거래)", Choices = OnOffChoices, Default = "关",
                Tooltip = "암거래 침식 구역의 숙주 상자 속 캐릭터를 원본 본체의 야간 정산으로 처리(생산물이 본체 생산 구역에 들어가고 원본 보고서 및 경험치에 포함되며 해당 캐릭터의 유즙과 피로도 추가로 반영; 다른 보관함은 대상 아님; 기본 꺼짐)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_semen", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_semen", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "정액 통 자동 생성 (암거래)", Choices = OnOffChoices, Default = "开",
                Tooltip = "암거래의 야간 정산 후 생산 구역을 확인해 같은 종류의 병이 최소 8개면 통 포장을 자동으로 새로 생성(기본 켜짐; 끄면 신규 생성만 금지하며 기존 통 포장에는 계속 자동 보충)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_milk_wondrous", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_milk_wondrous", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "신비한 유즙 통 자동 생성 (암거래)", Choices = OnOffChoices, Default = "开",
                Tooltip = "암거래의 야간 정산 후 생산 구역을 확인해 신비한 유즙이 최소 3병이면 통 포장을 자동으로 새로 생성(기본 켜짐; 끄면 신규 생성만 금지)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_milk_rich", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_milk_rich", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "농축 유즙 통 자동 생성 (암거래)", Choices = OnOffChoices, Default = "开",
                Tooltip = "암거래의 야간 정산 후 생산 구역을 확인해 농축 유즙이 최소 3병이면 통 포장을 자동으로 새로 생성(기본 켜짐; 끄면 신규 생성만 금지)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "remake.auto_barrel_milk_diluted", TargetMod = FamilyMods.Remake,
                TargetKey = "auto_barrel_milk_diluted", Kind = PanelKind.Choice, Section = RemakeSection, Carrier = CarrierKind.WriteThrough,
                Label = "맑고 달콤한 유즙 통 자동 생성 (암거래)", Choices = OnOffChoices, Default = "开",
                Tooltip = "암거래의 야간 정산 후 생산 구역을 확인해 맑고 달콤한 유즙이 최소 3병이면 통 포장을 자동으로 새로 생성(기본 켜짐; 끄면 신규 생성만 금지)" + ThroughNote,
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
                Label = "농축 유즙 단가 (암거래)", MinInt = 0, MaxInt = 999999, Default = 150,
                Tooltip = "작은 가슴의 숙주가 생산하는 농축 유즙의 단가(기본 150; 야간 생산과 아이템 목록 생성기 두 경로에서 동시에 적용)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkPriceWondrous, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "신비한 유즙 단가 (암거래)", MinInt = 0, MaxInt = 999999, Default = 80,
                Tooltip = "보통 크기 가슴의 숙주가 생산하는 신비한 유즙의 단가(기본 80)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkPriceDiluted, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "맑고 달콤한 유즙 단가 (암거래)", MinInt = 0, MaxInt = 999999, Default = 50,
                Tooltip = "큰 가슴의 숙주가 생산하는 맑고 달콤한 유즙의 단가(기본 50)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkFeedDiluted, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "맑고 달콤한 유즙 급여 시 피로 회복량 (암거래)", MinInt = 0, MaxInt = 100, Default = 20,
                Tooltip = "맑고 달콤한 유즙을 숙주에게 먹일 때 감소하는 피로도(기본 20)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkFeedWondrous, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "신비한 유즙 급여 시 피로 회복량 (암거래)", MinInt = 0, MaxInt = 100, Default = 30,
                Tooltip = "신비한 유즙을 숙주에게 먹일 때 감소하는 피로도(기본 30)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkFeedRich, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "농축 유즙 급여 시 피로 회복량 (암거래)", MinInt = 0, MaxInt = 100, Default = 25,
                Tooltip = "농축 유즙을 숙주에게 먹일 때 감소하는 피로도(기본 25)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkDrinkCredit, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "유즙 음용 시 획득 크레딧 (암거래)", MinInt = 0, MaxInt = 999999, Default = 30,
                Tooltip = "종류별 유즙 중 한 병을 직접 마실 때 획득하는 크레딧(기본 30, 병은 동시에 소멸)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceRichBase, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "농축 유즙 생산 확률 (기본)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "작은 가슴의 숙주가 매일 밤 농축 유즙을 생산할 기본 확률(기본 0.3)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceRichPerLact, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "농축 유즙 생산 확률 (레벨당)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "농축 유즙 생산 확률의 수유 레벨당 증가량(기본 0.3, 원본의 세 종류가 공유하는 계수)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceWondrousBase, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "신비한 유즙 생산 확률 (기본)", MinFloat = 0f, MaxFloat = 1f, Default = 0.5f,
                Tooltip = "보통 크기 가슴의 숙주가 매일 밤 신비한 유즙을 생산할 기본 확률(기본 0.5)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceWondrousPerLact, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "신비한 유즙 생산 확률 (레벨당)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "신비한 유즙 생산 확률의 수유 레벨당 증가량(기본 0.3)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceDilutedBase, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "맑고 달콤한 유즙 생산 확률 (기본)", MinFloat = 0f, MaxFloat = 1f, Default = 0.7f,
                Tooltip = "큰 가슴의 숙주가 매일 밤 맑고 달콤한 유즙을 생산할 기본 확률(기본 0.7)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeMilkChanceDilutedPerLact, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "맑고 달콤한 유즙 생산 확률 (레벨당)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "맑고 달콤한 유즙 생산 확률의 수유 레벨당 증가량(기본 0.3)" + TakeoverNote,
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
                Label = "배란 유도제 단가 (암거래)", MinInt = 0, MaxInt = 999999, Default = 100,
                Tooltip = "암거래 모드가 있을 때의 배란 유도제 단가(모드 내부에서 100으로 강제 설정하지만 이 패널이 그 이후 정확히 덮어씀; 기본 100=원본값 유지; 암거래 사용 중에는 메인 패널의 price.ovulation_shot이 적용되지 않음)" + TakeoverNote,
            },
            // ---- 暗流交易联动·疲劳核心接管(ST 1.6.1 Task 5,2026-11-17:ShadowTrade
            //      1.6.1 疲劳系统数值面,SubModHook prefix 复刻;默认 = 源码
            //      MidnightPawn.Game/MaidenFatigue.cs 字面量零漂移;档位语义名按
            //      StateName,边界 const 90/80/60/20 保持原版不可调)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulRested, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "피로도별 판매가 배율: 충분히 휴식함 (암거래)", MinFloat = 0f, MaxFloat = 5f, Default = 1.1f,
                Tooltip = "피로도 20 미만인 숙주의 판매가 배율(기본 1.1=+10%; 직접 연동 설정 fatigue_enabled로 피로 시스템을 끄면 항상 1)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulTired, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "피로도별 판매가 배율: 피곤 (암거래)", MinFloat = 0f, MaxFloat = 5f, Default = 0.94f,
                Tooltip = "피로도 60~79인 숙주의 판매가 배율(기본 0.94=-6%; 20~59의 정상 단계는 항상 1로 고정되며 조정 불가)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulWeary, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "피로도별 판매가 배율: 탈진 (암거래)", MinFloat = 0f, MaxFloat = 5f, Default = 0.88f,
                Tooltip = "피로도 80~89인 숙주의 판매가 배율(기본 0.88=-12%)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueValueMulExhausted, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "피로도별 판매가 배율: 과로 (암거래)", MinFloat = 0f, MaxFloat = 5f, Default = 0.8f,
                Tooltip = "피로도 90 이상(과로)인 숙주의 판매가 배율(기본 0.8=-20%; 과로 상태는 해당 캐릭터의 그날 밤 활동을 모두 중단시키며 이 설정은 활동 제한에 영향을 주지 않음)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueNightlyGain, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = RemakeSection,
                Label = "야간 피로도 기본 증가량 (암거래)", MinFloat = 0f, MaxFloat = 5f, Default = 0.5f,
                Tooltip = "매일 밤 가중 활동 횟수(성교+항문 성교+구강 성교×0.5)에 곱해 피로를 누적하는 기본값(기본 0.5; 0=피로 누적 없음; 실제 증가량에는 본체 레벨 계수, 임신 기간 1.5배, 직접 연동 설정 fatigue_scale의 누적 속도도 곱해짐)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.RemakeFatigueExhaustRecover, TargetMod = FamilyMods.Remake,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = RemakeSection,
                Label = "과로 상태 회복량 (암거래)", MinInt = 0, MaxInt = 100, Default = 5,
                Tooltip = "과로 상태의 숙주가 야간 정산 후 회복하는 피로도(기본 5; 해당 구역 밖 숙주의 하루 자연 회복량 25는 원본 그대로 유지)" + TakeoverNote,
            },

            // ---- 巢穴扩展联动(Brood 12 键:10 int 带范围 + 2 bool)----
            new FamilyPanelEntry
            {
                OurKey = "brood.eggBaseChance", TargetMod = FamilyMods.Brood,
                TargetKey = "eggBaseChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "산란 기본 확률 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 12,
                Tooltip = "둥지 확장의 기본 산란 확률 %(기본 12)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.humanToEggChance", TargetMod = FamilyMods.Brood,
                TargetKey = "humanToEggChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "인간 임신의 알 임신 전환 확률 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 30,
                Tooltip = "둥지 확장에서 인간이 산란으로 전환될 확률 %(기본 30)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.whiteGrowChance", TargetMod = FamilyMods.Brood,
                TargetKey = "whiteGrowChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "흰 촉수 성장 확률 (둥지 확장)", MinInt = 1, MaxInt = 80, Default = 12,
                Tooltip = "둥지 확장의 흰 촉수 성장 확률 %(기본 12, 범위 1~80)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.whiteBodyChance", TargetMod = FamilyMods.Brood,
                TargetKey = "whiteBodyChance", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "흰 촉수 체형 변화 확률 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 10,
                Tooltip = "둥지 확장의 흰색 체형 확률 %(기본 10)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.infectMajor", TargetMod = FamilyMods.Brood,
                TargetKey = "infectMajor", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "중증 감염 확률 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 30,
                Tooltip = "둥지 확장의 중증 감염 확률 %(기본 30)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.infectMinor", TargetMod = FamilyMods.Brood,
                TargetKey = "infectMinor", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "경증 감염 확률 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 15,
                Tooltip = "둥지 확장의 경증 감염 확률 %(기본 15)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.infectNone", TargetMod = FamilyMods.Brood,
                TargetKey = "infectNone", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "무감염 확률 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 5,
                Tooltip = "둥지 확장의 감염 없음 확률 %(기본 5)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.tentacleSpeechFrequency", TargetMod = FamilyMods.Brood,
                TargetKey = "tentacleSpeechFrequency", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "촉수 발언 빈도 (둥지 확장)", MinInt = 0, MaxInt = 100, Default = 20,
                Tooltip = "둥지 확장의 촉수 음성 빈도 %(기본 20)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.tentacleSpeechSpeed", TargetMod = FamilyMods.Brood,
                TargetKey = "tentacleSpeechSpeed", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "촉수 발언 속도 (둥지 확장)", MinInt = 25, MaxInt = 300, Default = 100,
                Tooltip = "둥지 확장의 촉수 음성 속도 %(기본 100, 범위 25~300)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.tentacleSpeechHold", TargetMod = FamilyMods.Brood,
                TargetKey = "tentacleSpeechHold", Kind = PanelKind.SliderInt, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "촉수 말풍선 유지 시간 (둥지 확장)", MinInt = 1, MaxInt = 15, Default = 4,
                Tooltip = "둥지 확장의 촉수 음성 표시 시간(초)(기본 4, 범위 1~15)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.disableRampageEnding", TargetMod = FamilyMods.Brood,
                TargetKey = "disableRampageEnding", Kind = PanelKind.Toggle, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "폭주 결말 비활성화 (둥지 확장)", Default = false,
                Tooltip = "둥지 확장의 폭주 결말을 비활성화(기본 꺼짐)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "brood.disableDevourFlash", TargetMod = FamilyMods.Brood,
                TargetKey = "disableDevourFlash", Kind = PanelKind.Toggle, Section = BroodSection, Carrier = CarrierKind.WriteThrough,
                Label = "삼키기 섬광 비활성화 (둥지 확장)", Default = false,
                Tooltip = "둥지 확장의 삼키기 섬광 효과를 비활성화(기본 꺼짐)" + ThroughNote,
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
                Label = "정액 매입가 보정 (둥지 확장 사용 중)", MinFloat = -100f, MaxFloat = 999f, Default = -15f,
                Tooltip = "손님 입장 시 촉수 정액 매입가 보정 %(기본 -15=본편 모드 원래 값; 둥지 확장이 있으면 메인 패널의 같은 설정을 -10으로 강제하므로 이 설정이 그 뒤에 적용됨; 둥지 확장이 있을 때 메인 패널의 기존 설정은 적용되지 않음)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BroodSemenBonusMod, TargetMod = FamilyMods.Brood,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = BroodSection,
                Label = "정액 판매가 보너스 (둥지 확장 사용 중)", MinFloat = -100f, MaxFloat = 999f, Default = 15f,
                Tooltip = "손님 입장 시 촉수 정액 판매가 추가 보너스 %(기본 +15=본편 모드 원래 값; 둥지 확장이 있으면 메인 패널의 같은 설정을 +10으로 강제하므로 이 설정이 그 뒤에 적용됨; 둥지 확장이 있을 때 메인 패널의 기존 설정은 적용되지 않음)" + TakeoverNote,
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
                Label = "숙주 태그 선택 도구 (둥지 확장)", Default = false,
                Tooltip = "켜면 숙주가 들어 있는 칸에 커서를 놓고 아래 단축키를 눌러 선택할 수 있으며 둥지 확장의 Home 패널에 있는 '숙주 태그 편집'에서 사용함. 붉은 거리의 돋보기 검사 기능에 더는 의존하지 않음. 끄면 별도 호스트와 반복 확인 없이 돋보기로만 선택하는 원본 동작으로 복귀. 켜면 이 패널의 값이 대신 적용되고 끄면 원본 동작으로 복귀",
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BroodTagPickerKey, TargetMod = FamilyMods.Brood,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.EnumKeyCode, Section = BroodSection,
                Label = "선택 단축키 (둥지 확장)", Default = "U",
                Tooltip = "이미 사용 중인 F7/F8/F9/Home/[ ] 등의 키는 피할 것. 숙주에 커서를 올리고 이 키로 선택한 다음 Brood 패널(Home)의 '숙주 태그 편집'을 누르면 됨(기본 U, 변경 즉시 적용; 이 키 자체는 활성화 기능이 없고 위 스위치가 선택 도구를 제어함). 켜면 이 패널의 값이 대신 적용되고 끄면 원본 동작으로 복귀",
            },

            // ---- 掌上青楼联动(M5a,brothel 0.10.4 无配置系统 → SubModHook 方法接管;
            //      默认值 = 反编译 LewdMemeBrothel.Rules 字面量,逐字;MinInt/MaxInt 为
            //      本 mod 自定面板范围(对方无 Define 可镜像);TargetKey 留空)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelBudgetBonus, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "태그당 손님 예산 증가율 (휴대용 매음굴)", MinInt = 0, MaxInt = 999, Default = 10,
                Tooltip = "손님의 요구 태그가 하나 일치할 때마다 예산이 증가하는 비율 %(기본 10)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelDebtTags, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "외상 구매 최소 태그 일치 수 (휴대용 매음굴)", MinInt = 0, MaxInt = 20, Default = 5,
                Tooltip = "일치 태그가 이 수에 도달해야 예산 초과 상품을 외상으로 살 가능성이 생김(기본 5)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelDebtChance, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "외상 구매 확률 (휴대용 매음굴)", MinInt = 0, MaxInt = 100, Default = 50,
                Tooltip = "외상 구매 기준에 도달한 뒤 상품을 구매할 확률 %(기본 50)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelGuestsBase, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "기본 예약 손님 수 (휴대용 매음굴)", MinInt = 1, MaxInt = 50, Default = 2,
                Tooltip = "하루 예약 손님 기본 인원(기본 2)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelGuestsPerUpgrade, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "시설 확장당 예약 손님 증가 수 (휴대용 매음굴)", MinInt = 0, MaxInt = 50, Default = 2,
                Tooltip = "방 하나를 확장할 때마다 추가되는 예약 손님 인원(기본 2)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelGuestsMax, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "하루 예약 손님 상한 (휴대용 매음굴)", MinInt = 1, MaxInt = 50, Default = 20,
                Tooltip = "예약 손님과 탭의 하루 상한(기본 20, 5곳에서 사용됨)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelTraitsBase, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "기본 요구 태그 수 (휴대용 매음굴)", MinInt = 1, MaxInt = 8, Default = 3,
                Tooltip = "손님이 요구하는 기본 태그 수(기본 3, 최대 8종류)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelTraitsPerUpgrade, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "확장당 요구 태그 증가 수 (휴대용 매음굴)", MinInt = 0, MaxInt = 8, Default = 1,
                Tooltip = "방 하나를 확장할 때마다 늘어나는 요구 태그 수(기본 1)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelFeeDefault, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "기본 숙박비 (휴대용 매음굴)", MinInt = 0, MaxInt = 9999, Default = 120,
                Tooltip = "숙박비를 따로 설정하지 않았을 때 사용하는 값(기본 120)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelFeeMax, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "숙박비 상한 (휴대용 매음굴)", MinInt = 1, MaxInt = 999999, Default = 9999,
                Tooltip = "숙박비를 제한하는 상한. 입력 상자 안내도 함께 바뀜(기본 9999)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelMasteryThreshold, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "숙련 달성 임계값 (휴대용 매음굴)", MinInt = 1, MaxInt = 9999, Default = 30,
                Tooltip = "한 종류의 서비스 횟수가 이 값에 도달하면 숙련 태그 획득(기본 30)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelVisitMinDays, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "상인 방문 최소 간격 (일) (휴대용 매음굴)", MinInt = 1, MaxInt = 99, Default = 4,
                Tooltip = "상인이 연속 방문하는 최소 간격(일)(기본 4)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelVisitMaxDays, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "상인 방문 최대 간격 (일) (휴대용 매음굴)", MinInt = 1, MaxInt = 99, Default = 9,
                Tooltip = "상인이 연속 방문하는 최대 간격(일)(기본 9, 원본 무작위 추첨 범위 4..9)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceMaidDress, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "메이드복 판매가 (휴대용 매음굴)", MinInt = 0, MaxInt = 999999, Default = 260,
                Tooltip = "의류 상인이 판매하는 메이드복 단가(기본 260)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceCatEars, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "고양이 귀 판매가 (휴대용 매음굴)", MinInt = 0, MaxInt = 999999, Default = 180,
                Tooltip = "의류 상인이 판매하는 고양이 귀 단가(기본 180)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceCatTailPlug, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "고양이 꼬리 애널 플러그 판매가 (휴대용 매음굴)", MinInt = 0, MaxInt = 999999, Default = 220,
                Tooltip = "의류 상인이 판매하는 고양이 꼬리 애널 플러그 단가(기본 220)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceLaceDress, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "레이스 드레스 판매가 (휴대용 매음굴)", MinInt = 0, MaxInt = 999999, Default = 240,
                Tooltip = "의류 상인이 판매하는 레이스 드레스 단가(기본 240)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceBunnyEars, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "토끼 귀 판매가 (휴대용 매음굴)", MinInt = 0, MaxInt = 999999, Default = 175,
                Tooltip = "의류 상인이 판매하는 토끼 귀 단가(기본 175)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelPriceLaceGarter, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "레이스 가터벨트 판매가 (휴대용 매음굴)", MinInt = 0, MaxInt = 999999, Default = 150,
                Tooltip = "의류 상인이 판매하는 레이스 가터벨트 단가(기본 150)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelStockFirst, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "첫 방문 진열 상품 수 (휴대용 매음굴)", MinInt = 0, MaxInt = 6, Default = 3,
                Tooltip = "상인이 처음 방문했을 때 진열하는 고정 상품 수(기본 3, 아이템 목록 순서대로 처음 N개 선택)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelStockMin, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "이후 방문 최소 상품 수 (휴대용 매음굴)", MinInt = 0, MaxInt = 6, Default = 3,
                Tooltip = "이후 방문 시 무작위로 진열하는 상품 수 하한(기본 3)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.BrothelStockMax, TargetMod = FamilyMods.Brothel,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = BrothelSection,
                Label = "이후 방문 최대 상품 수 (휴대용 매음굴)", MinInt = 0, MaxInt = 6, Default = 5,
                Tooltip = "이후 방문 시 무작위로 진열하는 상품 수 상한(기본 5, 원본 추첨 범위 3..5)" + TakeoverNote,
            },

            // ---- tearlaments 联动(M5b,tearl v7 无配置系统 → SubModHook 方法接管;
            //      默认值 = 反编译 Tearlaments/* 字面量逐字(R-O);MinInt/MaxInt 为
            //      本 mod 自定面板范围(对方无 Define 可镜像);TargetKey 留空)----
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlSpawnChance, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderFloat, Section = TearlSection,
                Label = "등장 확률 (tearlaments)", MinFloat = 0f, MaxFloat = 1f, Default = 0.3f,
                Tooltip = "Kitkallos가 일반 손님이 생성될 때마다 등장할 확률(기본 0.3)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlBudgetBase, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "매입 기본 예산 (tearlaments)", MinInt = 0, MaxInt = 1000000, Default = 150000,
                Tooltip = "Kitkallos의 매입 기본 예산(기본 150000)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlBudgetRandom, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "매입 예산 무작위 증가량 (tearlaments)", MinInt = 0, MaxInt = 1000000, Default = 50000,
                Tooltip = "매입 예산의 기본값에 추가되는 무작위 금액의 상한(기본 50000, 상한 포함)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlSellDiscount, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "판매 할인율 (tearlaments)", MinInt = 0, MaxInt = 100, Default = 10,
                Tooltip = "Kitkallos가 내 가게 상품을 매입할 때 적용되는 전체 할인율 %(기본 10, 즉 매입가 -10%)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlBuyDiscount, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "소녀 매입 할인율 (tearlaments)", MinInt = 0, MaxInt = 100, Default = 25,
                Tooltip = "Kitkallos가 소녀 아이템을 매입할 때 추가로 적용되는 할인율 %(기본 25, 즉 -25%)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.TearlWaterPremium, TargetMod = FamilyMods.Tearl,
                Carrier = CarrierKind.SubModHook, Kind = PanelKind.SliderInt, Section = TearlSection,
                Label = "물 매입 추가 가격 (tearlaments)", MinInt = 0, MaxInt = 100, Default = 20,
                Tooltip = "Kitkallos가 매입 대상 물을 살 때 추가로 지불하는 할증률 %(기본 20, 즉 +20%)" + TakeoverNote,
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
                Label = "고해상도 재생성만 사용 (초상화 관리자)", Default = false,
                Tooltip = "이 모드가 수행하는 모든 자동 초상화 추첨을 끄고 본편 모드에 초상화 선택을 완전히 맡기며 고해상도 재생성만 수행(기본 꺼짐); 주입형 초상화 팩(소녀 전당포 등)을 사용할 때는 켜지 말 것" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.final_portrait_selection", TargetMod = FamilyMods.Iph,
                TargetKey = "final_portrait_selection", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "최종 적용 초상화만 사용 (초상화 관리자)", Default = false,
                Tooltip = "초상화를 선택할 때 현재 게임에서 최종 적용된 이미지로만 추첨하고 고정 가중치만 사용하며 조정 가능한 가중치는 읽지 않음(기본 꺼짐); 주입형 초상화 팩(소녀 전당포 등)을 사용할 때는 켜지 말 것" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.resolution", TargetMod = FamilyMods.Iph,
                TargetKey = "resolution", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "초상화 해상도 (초상화 관리자)", Choices = new[] { "128", "256" }, Default = "256",
                Tooltip = "초상화 텍스처 해상도 단계(기본 256; 표시 크기와 차지하는 칸 수는 바뀌지 않으며 변경 후 캐시를 지우면 즉시 적용됨)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.guest_portrait_hd", TargetMod = FamilyMods.Iph,
                TargetKey = "guest_portrait_hd", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "손님 초상화 고해상도 표시 (초상화 관리자)", Default = true,
                Tooltip = "손님 목록 초상화를 실제로 선택된 원본 이미지에서 128/256 해상도 버전으로 생성(기본 켜짐, 포획 가중치에는 관여하지 않음)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.keep_guest_names", TargetMod = FamilyMods.Iph,
                TargetKey = "keep_guest_names", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "게임 손님 이름 변경 방지 (초상화 관리자)", Default = false,
                Tooltip = "본편 모드가 게임 기본 손님의 표시 이름을 덮어쓰지 못하도록 함(기본 꺼짐, 이미 바뀐 이름은 복구하지 않음)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.keep_mod_guest_names", TargetMod = FamilyMods.Iph,
                TargetKey = "keep_mod_guest_names", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "모드 손님 이름 변경 방지 (초상화 관리자)", Default = false,
                Tooltip = "본편 모드가 모드에서 생성되었다는 기록이 있는 손님의 이름을 덮어쓰지 못하도록 함(기본 꺼짐, 위 설정과 독립적)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.name_region", TargetMod = FamilyMods.Iph,
                TargetKey = "name_region", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "이름 지역 제한 (초상화 관리자)", Choices = new[] { "不限制（原版）", "仅日本", "仅中国", "仅中日韩" }, Default = "不限制（原版）",
                Tooltip = "본편 모드의 공용 이름 생성기에서 사용할 지역 제한 단계(기본 제한 없음, 새로 만드는 이름에만 영향)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.local_portrait_chance", TargetMod = FamilyMods.Iph,
                TargetKey = "local_portrait_chance", Kind = PanelKind.SliderInt, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "폴더 초상화 선택 확률 (초상화 관리자)", MinInt = 0, MaxInt = 100, Default = 0,
                Tooltip = "체형별 출처가 지정되지 않은 경우 Portraits 폴더의 PNG를 사용할 확률 %(기본 0)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.weighted_capture_factions", TargetMod = FamilyMods.Iph,
                TargetKey = "weighted_capture_factions", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "초상화 수에 따라 포획 가중치 조정 (초상화 관리자)", Default = true,
                Tooltip = "기본 켜짐: 각 세력에서 추첨 가능한 캐릭터의 총 가중치에 따라 세력을 선택함. 끄면 본편 모드의 세력 후보군 추첨으로 되돌림(이때는 메인 패널의 세력 후보군 CSV가 실제로 사용됨)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.trace_portrait_replacements", TargetMod = FamilyMods.Iph,
                TargetKey = "trace_portrait_replacements", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "초상화 교체 추적 (초상화 관리자)", Default = true,
                Tooltip = "모드별 초상화 교체 진입점을 감시하고 실제 출력 결과를 기록해 F7 관리자에서 검색하도록 함(기본 켜짐)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.portrait_gender", TargetMod = FamilyMods.Iph,
                TargetKey = "portrait_gender", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "초상화 성별 적용 범위 (초상화 관리자)", Choices = new[] { "仅女性（原版）", "不限性别" }, Default = "不限性别",
                Tooltip = "새로 생성되는 아이템이 캐릭터 후보군에서 검색할 성별 범위(기본 제한 없음)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.tool_gender", TargetMod = FamilyMods.Iph,
                TargetKey = "tool_gender", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "아이템 적용 대상 성별 (초상화 관리자)", Choices = new[] { "仅女性（原版）", "不限性别" }, Default = "不限性别",
                Tooltip = "성별 제한이 없을 때 본편 모드의 촉수 정액이 여성 손님에게만 작용하는 제한을 해제(기본 제한 없음, 즉시 적용)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.customer_conversion", TargetMod = FamilyMods.Iph,
                TargetKey = "customer_conversion", Kind = PanelKind.Choice, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "손님 여성화 (초상화 관리자)", Choices = new[] { "原版行为", "保留游戏原分布" }, Default = "原版行为",
                Tooltip = "본편 모드는 일부 일반 남성 손님을 여성 초상화로 바꿈. 기존 분포 유지 옵션을 선택하면 이 처리를 건너뛸 수 있음(기본 원본 동작)" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.body_weights_csv", TargetMod = FamilyMods.Iph,
                TargetKey = "body_weights_csv", Kind = PanelKind.Input, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "성인 체형 가중치 CSV (초상화 관리자)", Default = "20, 20, 20, 20, 20",
                Tooltip = "성인 체형의 추첨 가중치. 쉼표로 구분하며 항목 순서는 萝莉,少女,御姐,少妇,熟女(기본값 모두 20=균등; 0=해당 항목을 선택하지 않음. 단 전부 0으로 설정할 수 없고, IPH는 가중치 0도 낮은 확률의 균등 추첨 대상으로 유지함. 전체 문자열이 0이면 기본값 복귀, 추첨 범위에서 전부 0이면 균등 추첨하며 강제 제외는 아님. IPH 설계; 해당 체형 사이의 상대적인 비율만 조정). 메인 패널의 체형 후보군을 양보한 뒤에는 이 설정이 체형 분포를 결정함" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.body_affects_breast_size", TargetMod = FamilyMods.Iph,
                TargetKey = "body_affects_breast_size", Kind = PanelKind.Toggle, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "체형에 따른 유즙량 조정 (초상화 관리자)", Default = true,
                Tooltip = "기본 켜짐: 새로 생성된 숙주의 유즙량을 체형-유즙량 확률 행렬로 결정함. 끄면 본편 모드의 원래 값인 유즙량 3종류 각각 1/3 확률로 복귀. 메인 패널의 가슴 크기 후보군을 양보한 뒤에는 이 설정이 유즙량 분포를 결정함" + ThroughNote,
            },
            new FamilyPanelEntry
            {
                OurKey = "iph.scavenger_double_virgin_csv", TargetMod = FamilyMods.Iph,
                TargetKey = "scavenger_double_virgin_csv", Kind = PanelKind.Input, Section = IphSection, Carrier = CarrierKind.WriteThrough,
                Label = "체형별 이중 처녀 확률 CSV (초상화 관리자)", Default = "60, 60, 60, 60, 60",
                Tooltip = "폐품 수집으로 생성되는 성인 체형의 이중 처녀 확률 %. 쉼표로 구분하며 항목 순서는 위와 같음(기본값 모두 60, 각 항목은 독립적인 0~100 범위, 합계가 100일 필요 없음)" + ThroughNote,
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
                Label = "높은 능력치 확률 (초상화 관리자 사용 중)", MinFloat = 0f, MaxFloat = 1f, Default = 0.01f,
                Tooltip = "능력치 무작위 판정이 상위 구간에 들어갈 확률(기본 1%=원본; 초상화 관리자 사용 중에는 메인 패널의 같은 설정을 양보하지만 이 설정은 공존 패치로 계속 적용되며, 초상화 관리자가 있고 버전이 일치할 때만 사용 가능)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrMidChance, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.SliderFloat, Section = IphSection,
                Label = "중간 능력치 확률 (초상화 관리자 사용 중)", MinFloat = 0f, MaxFloat = 1f, Default = 0.06f,
                Tooltip = "능력치 무작위 판정이 중간 구간에 들어갈 누적 임계값(기본 6%=원본, 상위 구간 포함; 하위 구간은 나머지 확률)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrHighRange, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.Input, Section = IphSection,
                Label = "높은 능력치 범위 CSV (초상화 관리자 사용 중)", Default = "70,80",
                Tooltip = "상위 능력치 판정 범위 '하한,상한'(기본 70,80=원본, 양쪽 끝 포함)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrMidRange, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.Input, Section = IphSection,
                Label = "중간 능력치 범위 CSV (초상화 관리자 사용 중)", Default = "60,70",
                Tooltip = "중간 능력치 판정 범위 '하한,상한'(기본 60,70=원본, 양쪽 끝 포함)" + TakeoverNote,
            },
            new FamilyPanelEntry
            {
                OurKey = CfgKeys.IphAttrLowRange, TargetMod = FamilyMods.Iph,
                Carrier = CarrierKind.MainHook, Kind = PanelKind.Input, Section = IphSection,
                Label = "낮은 능력치 범위 CSV (초상화 관리자 사용 중)", Default = "10,60",
                Tooltip = "하위 능력치 판정 범위 '하한,상한'(기본 10,60=원본, 양쪽 끝 포함)" + TakeoverNote,
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
