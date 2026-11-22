using System;
using System.Collections.Generic;
using LewdMemeConfig;

// Task 11(M4b)+ Task 12(M5a)+ Task 13(M5b)+ 0.6.0 R4(2026-11-13)+
// R9(2026-11-14)+ R10(2026-11-14)+ ST4(2026-11-17)+ ST5(2026-11-17)+
// 0.7.1 拾取器(2026-11-18):
// FamilyPanelKeys 子 mod 面板纯表(remake 8 写透[ST4: 7−3+4 桶装]+ 20 差分
// 接管[ST4: 19−5;R7 +ovulation_price;ST5: +疲劳核心 6]+ Brood 12 写透 +
// brothel 22 + tearl 6 接管键 + IPH 16 写透
// [R9:17 键扣 hotkey;body_preference/scavenger_double_virgin_0..4 已并成两个
// CSV 单键]+ IPH 属性 5 共存 MainHook[R10]+ Brood 拾取器 2[0.7.1:自开关
// Toggle + EnumKeyCode 键位,SubModHook 无补丁宿主单元],见下])契约:
//  ① 每条目 OurKey/Label/Section 非空、TargetMod ∈ FamilyMods 常量;WriteThrough
//     条目 OurKey = 短前缀 + TargetKey("remake.*"/"brood.*")、Tooltip 含写透注记
//     (「开启后以本面板为准,关闭恢复开启前对方值」,控制器逐字要求);SubModHook
//     条目(brothel/tearl)TargetKey 留空、Tooltip 含接管注记;
//  ② 默认值表与勘察摘要逐一相等(精确值;单一事实源 = 2026-11-12 家族勘察
//     摘要 + 反编译复核:remake ModSettings.cs:176-182 全 string Choices、
//     Brood Settings.cs:115-132 = 10 int 带范围 + 2 bool(热键 2 项不镜像)、
//     brothel Rules/* const 内联字面量 10/5/50/2/2/20/3/1/120/9999/30/4/9/
//     260/180/220/240/175/150/3/3/5、tearl Tearlaments/* const 内联字面量
//     0.3/150000/50000/10/25/20——spawn_chance 为 float)、IPH iph_1217
//     ModEntry.cs Init + BodyCsvSettings 逐字(R9;两个 CSV 键 Input 形态,
//     默认串 = BodyCsvRules.Defaults(5, 20.0/60.0),项序 萝莉,少女,御姐,
//     少妇,熟女);
//  ③ 开关键派生同 PanelKeys.ItemSwitchKey 规则,且 ConfigRegistry 已注册
//     (默认 false = 装上即纯原版),值键按 Kind 的 CLR 类型可读;
//  ④ 条目数 28+16+22+6+21=93,分区归属精确(「暗流交易联动」28[写透 8 + 差分 20,
//     ST4 撤 3 写透 + 4 桶装 − 5 差分;ST5 +疲劳核心 6]/「巢穴扩展联动」16
//     [12 写透 + 2 信息素接管 R12 + 2 拾取器 0.7.1] /「掌上青楼联动」22 /
//     「tearlaments 联动」6 /「物品高清立绘管理器联动」21[16 写透 + 5 属性共存,
//     R10]);
//  ⑤ 目录耦合:HookCatalog 的 WriteThrough 单元键集恰为 WriteThrough 条目
//     OurKey 集;SubModHook 单元键集恰为 brothel+tearl+remake 接管条目
//     OurKey 集(ST4: 跨族键例外 +1[semen_chance_cap × semen_formula,R7]
//     随单元注销终结,回归恰等);R10 的
//     MainHook 镜像条目(iph.attr_* 5 键)恰为 main.maiden_attrs_iph 单元键集。
public static class FamilyPanelKeysTests
{
    public static void Run()
    {
        // —— ① 形态与守卫字段(按载体分流;R10 后 MainHook 条目 = iph.attr_* 五键) ——
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
        {
            Check.Eq(true, !string.IsNullOrEmpty(e.OurKey), "① OurKey 非空");
            Check.Eq(true, !string.IsNullOrEmpty(e.Label), "① Label 非空: " + e.OurKey);
            Check.Eq(true, !string.IsNullOrEmpty(e.Section), "① Section 非空: " + e.OurKey);
            Check.Eq(true, e.TargetMod == FamilyMods.Remake || e.TargetMod == FamilyMods.Brood
                    || e.TargetMod == FamilyMods.Brothel || e.TargetMod == FamilyMods.Tearl
                    || e.TargetMod == FamilyMods.Iph || e.TargetMod == FamilyMods.MainMod,
                "① TargetMod ∈ 家族常量(0.8.0 含主 mod 官方设置写透): " + e.OurKey);
            if (e.Carrier == CarrierKind.SubModHook)
            {
                string expectMod = e.OurKey.StartsWith("tearl.", StringComparison.Ordinal)
                    ? FamilyMods.Tearl
                    : e.OurKey.StartsWith("remake.", StringComparison.Ordinal)
                    ? FamilyMods.Remake
                    : e.OurKey.StartsWith("brood.", StringComparison.Ordinal)
                    ? FamilyMods.Brood : FamilyMods.Brothel;
                Check.Eq(expectMod, e.TargetMod, "① 接管条目指向对应子 mod: " + e.OurKey);
                Check.Eq(null, e.TargetKey, "① 接管条目无对方配置键: " + e.OurKey);
                Check.Eq(true, e.OurKey.StartsWith("brothel.", StringComparison.Ordinal)
                        || e.OurKey.StartsWith("tearl.", StringComparison.Ordinal)
                        || e.OurKey.StartsWith("remake.", StringComparison.Ordinal)
                        || e.OurKey.StartsWith("brood.", StringComparison.Ordinal),
                    "① 接管条目 OurKey 用 brothel.*/tearl.*/remake.*/brood.* 前缀: " + e.OurKey);
                Check.Eq(true, e.Tooltip != null
                    && e.Tooltip.Contains(FamilyPanelKeys.TakeoverNote),
                    "① Tooltip 含接管注记: " + e.OurKey);
            }
            else if (e.Carrier == CarrierKind.MainHook)
            {
                // R10:IPH 属性五键(postfix on 主 mod CreateMaidenOfFaction,与
                // IPH 工厂 transpiler 共存)——TargetMod = IPH 作在场条件 Define
                // 与版本绑定;无对方配置键可指(TargetKey 留空同 SubModHook 惯例);
                // Tooltip 含接管注记(开 = 本面板值重掷属性,关 = 原版掷骰)
                Check.Eq(FamilyMods.Iph, e.TargetMod, "① R10 共存条目指向 IPH: " + e.OurKey);
                Check.Eq(null, e.TargetKey, "① R10 共存条目无对方配置键: " + e.OurKey);
                Check.Eq(true, e.OurKey.StartsWith("iph.attr_", StringComparison.Ordinal),
                    "① R10 共存条目 OurKey 用 iph.attr_* 前缀: " + e.OurKey);
                Check.Eq(true, e.Tooltip != null
                    && e.Tooltip.Contains(FamilyPanelKeys.TakeoverNote),
                    "① Tooltip 含接管注记: " + e.OurKey);
            }
            else
            {
                Check.Eq(CarrierKind.WriteThrough, e.Carrier, "① 载体 ∈ {WriteThrough,SubModHook,MainHook}: " + e.OurKey);
                Check.Eq(true, !string.IsNullOrEmpty(e.TargetKey), "① TargetKey 非空: " + e.OurKey);
                string prefix = e.TargetMod == FamilyMods.Remake ? "remake."
                    : e.TargetMod == FamilyMods.Brood ? "brood."
                    : e.TargetMod == FamilyMods.MainMod ? "main." : "iph.";
                Check.Eq(true, e.OurKey == prefix + e.TargetKey,
                    "① OurKey = 短前缀+TargetKey: " + e.OurKey);
                Check.Eq(true, e.Tooltip != null
                    && e.Tooltip.Contains("开启后以本面板为准,关闭恢复开启前对方值"),
                    "① Tooltip 含写透注记: " + e.OurKey);
            }
        }

        // —— ② 默认值逐字(勘察摘要 remake 4 存续键 + ST4 桶装 4 键 + Brood 12)
        //    + Kind↔CLR 类型形态 ——
        Expect("remake.fatigue_enabled", "开", PanelKind.Choice);
        Expect("remake.fatigue_scale", "100%", PanelKind.Choice);
        Expect("remake.buyer_per_day", "1", PanelKind.Choice);
        Expect("remake.corruption_box_settlement", "关", PanelKind.Choice);
        // ST4 桶装 4 键(ShadowTrade 1.6.1 ModSettings.cs:34-37 DefineBarrel,
        // 全部 string Choices {"开","关"} 默认 "开";消费方 CreateNightBarrel
        // 的 On(entry) 判"非关即开",与我方 Choice 镜像逐字兼容)
        Expect("remake.auto_barrel_semen", "开", PanelKind.Choice);
        Expect("remake.auto_barrel_milk_wondrous", "开", PanelKind.Choice);
        Expect("remake.auto_barrel_milk_rich", "开", PanelKind.Choice);
        Expect("remake.auto_barrel_milk_diluted", "开", PanelKind.Choice);
        Expect("brood.eggBaseChance", 12, PanelKind.SliderInt);
        Expect("brood.humanToEggChance", 30, PanelKind.SliderInt);
        Expect("brood.whiteGrowChance", 12, PanelKind.SliderInt);
        Expect("brood.whiteBodyChance", 10, PanelKind.SliderInt);
        Expect("brood.infectMajor", 30, PanelKind.SliderInt);
        Expect("brood.infectMinor", 15, PanelKind.SliderInt);
        Expect("brood.infectNone", 5, PanelKind.SliderInt);
        Expect("brood.tentacleSpeechFrequency", 20, PanelKind.SliderInt);
        Expect("brood.tentacleSpeechSpeed", 100, PanelKind.SliderInt);
        Expect("brood.tentacleSpeechHold", 4, PanelKind.SliderInt);
        Expect("brood.disableRampageEnding", false, PanelKind.Toggle);
        Expect("brood.disableDevourFlash", false, PanelKind.Toggle);
        // R12 等价恢复:信息素两键(Brood WeakenBasePerfume 进场重设;默认 =
        // 主 mod 1.7.1 TentacleEffects 原字面量 -15/+15[lewd_meme_171
        // TentacleEffects.cs:99/:74],与主面板 client.semen_* 同值)
        Expect(CfgKeys.BroodSemenPriceMod, -15f, PanelKind.SliderFloat);
        Expect(CfgKeys.BroodSemenBonusMod, 15f, PanelKind.SliderFloat);
        // 0.7.1 拾取器两键(键默认值两断言):enabled = Toggle 自开关默认关
        // (开 = 装宿主;关 = 零 GameObject 零轮询,按需装配红线)、key = "U"
        // (KeyCode 名串;默认 U 在全家族[Brood testPanelKey=Home/dock=End、
        // IPH hotkey=F7]与游戏绑定中零冲突)
        Expect(CfgKeys.BroodTagPickerEnabled, false, PanelKind.Toggle);
        Expect(CfgKeys.BroodTagPickerKey, "U", PanelKind.EnumKeyCode);
        // brothel 22 键默认值 = Rules/* 反编译字面量逐字(M5a,R-L 零漂移)
        Expect(CfgKeys.BrothelBudgetBonus, 10, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelDebtTags, 5, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelDebtChance, 50, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelGuestsBase, 2, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelGuestsPerUpgrade, 2, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelGuestsMax, 20, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelTraitsBase, 3, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelTraitsPerUpgrade, 1, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelFeeDefault, 120, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelFeeMax, 9999, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelMasteryThreshold, 30, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelVisitMinDays, 4, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelVisitMaxDays, 9, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelPriceMaidDress, 260, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelPriceCatEars, 180, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelPriceCatTailPlug, 220, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelPriceLaceDress, 240, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelPriceBunnyEars, 175, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelPriceLaceGarter, 150, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelStockFirst, 3, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelStockMin, 3, PanelKind.SliderInt);
        Expect(CfgKeys.BrothelStockMax, 5, PanelKind.SliderInt);
        // tearl 6 键默认值 = tearl_7 反编译字面量逐字(M5b,R-O/R-P 零漂移):
        // 掷骰 0.3(TearlamentsMod.cs:61)+ 预算 150000/50000(CustomCustomer.cs:53)
        // + 折扣/溢价 10/25/20(CustomCustomer.cs:54/107/112)
        Expect(CfgKeys.TearlSpawnChance, 0.3f, PanelKind.SliderFloat);
        Expect(CfgKeys.TearlBudgetBase, 150000, PanelKind.SliderInt);
        Expect(CfgKeys.TearlBudgetRandom, 50000, PanelKind.SliderInt);
        Expect(CfgKeys.TearlSellDiscount, 10, PanelKind.SliderInt);
        Expect(CfgKeys.TearlBuyDiscount, 25, PanelKind.SliderInt);
        Expect(CfgKeys.TearlWaterPremium, 20, PanelKind.SliderInt);
        // remake 差分 18 键默认值 = remake_1519 反编译字面量逐字(0.6.0 R4 零漂移):
        // 单价 150/80/50(ItemFactory.cs:486)+ 喂奶 20/30/25(:518-532 switch)+
        // 饮用 30(:514)+ 概率 0.3/0.5/0.7 与每级 0.3×3(TentacleBody.cs:1195-1216,
        // 每级系数原版三档共用一个 0.3f)+ 精液 0.2/0.002(SemenProduction.cs:9)+
        // 闹钟 1/1/1(主 mod AlarmClock.cs:371-373 的 AddOral/Sex/Anal(item,1))
        Expect(CfgKeys.RemakeMilkPriceRich, 150, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkPriceWondrous, 80, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkPriceDiluted, 50, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkFeedDiluted, 20, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkFeedWondrous, 30, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkFeedRich, 25, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkDrinkCredit, 30, PanelKind.SliderInt);
        Expect(CfgKeys.RemakeMilkChanceRichBase, 0.3f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeMilkChanceRichPerLact, 0.3f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeMilkChanceWondrousBase, 0.5f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeMilkChanceWondrousPerLact, 0.3f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeMilkChanceDilutedBase, 0.7f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeMilkChanceDilutedPerLact, 0.3f, PanelKind.SliderFloat);
        // ST4:semen_base/semen_train_factor(SemenProduction 类删除)与 alarm
        // 三键(AddAlarm* 消失)全链废弃——默认值断言随条目删除(HookCatalogTests
        // 的退役键零残留断言覆盖)。
        // R7 等价恢复:排卵针单价(remake ConfigureOvulationShot :576 强制 100,
        // postfix 覆盖;默认 100 = 零漂移)
        Expect(CfgKeys.RemakeOvulationPrice, 100, PanelKind.SliderInt);
        // ST 1.6.1 Task 5:疲劳核心六键默认值 = ShadowTrade 1.6.1
        // MidnightPawn.Game/MaidenFatigue.cs 字面量逐字(售价四档 :210-233
        // ValueMul 1.1/0.94/0.88/0.8 + 夜间基数 :265 的 0.5f + 透支恢复
        // const ExhaustRecover :28 = 5,消费点 :124)
        Expect(CfgKeys.RemakeFatigueValueMulRested, 1.1f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeFatigueValueMulTired, 0.94f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeFatigueValueMulWeary, 0.88f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeFatigueValueMulExhausted, 0.8f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeFatigueNightlyGain, 0.5f, PanelKind.SliderFloat);
        Expect(CfgKeys.RemakeFatigueExhaustRecover, 5, PanelKind.SliderInt);
        // R9:IPH 16 键默认值 = iph_1217 逐字(ModEntry.cs Init 15 - hotkey
        // + BodyCsvSettings 2;键清单与 1215 漂移:body_preference 与
        // scavenger_double_virgin_0..4 已并为两个 CSV 单键,BodyCsvSettings.cs:42/49)
        Expect("iph.original_portrait_selection", false, PanelKind.Toggle);
        Expect("iph.final_portrait_selection", false, PanelKind.Toggle);
        Expect("iph.resolution", "256", PanelKind.Choice);
        Expect("iph.guest_portrait_hd", true, PanelKind.Toggle);
        Expect("iph.keep_guest_names", false, PanelKind.Toggle);
        Expect("iph.keep_mod_guest_names", false, PanelKind.Toggle);
        Expect("iph.name_region", "不限制（原版）", PanelKind.Choice);
        Expect("iph.local_portrait_chance", 0, PanelKind.SliderInt);
        Expect("iph.weighted_capture_factions", true, PanelKind.Toggle);
        Expect("iph.trace_portrait_replacements", true, PanelKind.Toggle);
        Expect("iph.portrait_gender", "不限性别", PanelKind.Choice);
        Expect("iph.tool_gender", "不限性别", PanelKind.Choice);
        Expect("iph.customer_conversion", "原版行为", PanelKind.Choice);
        Expect("iph.body_weights_csv", "20, 20, 20, 20, 20", PanelKind.Input);
        Expect("iph.body_affects_breast_size", true, PanelKind.Toggle);
        Expect("iph.scavenger_double_virgin_csv", "60, 60, 60, 60, 60", PanelKind.Input);
        // R10:IPH 属性五键(共存 MainHook)默认值 = 主 mod 1.7.1 ItemFactory.
        // RollAttr 反编译字面量逐字(:507 0.01f / :511 0.06f / :509/:513/:515
        // Range(70,81)/Range(60,71)/Range(10,61) 的 int 上界排除 = 闭区间 CSV)
        Expect(CfgKeys.IphAttrHighChance, 0.01f, PanelKind.SliderFloat);
        Expect(CfgKeys.IphAttrMidChance, 0.06f, PanelKind.SliderFloat);
        Expect(CfgKeys.IphAttrHighRange, "70,80", PanelKind.Input);
        Expect(CfgKeys.IphAttrMidRange, "60,70", PanelKind.Input);
        Expect(CfgKeys.IphAttrLowRange, "10,60", PanelKind.Input);

        // Choices 精确档位(= 对方游戏侧档位串,逐字;中文别名映射是 remake 内部
        // 解析,不复制——控制器 R-G 裁定)。
        ExpectChoices("remake.fatigue_scale",
            "50%", "75%", "100%", "150%", "200%", "300%");
        ExpectChoices("remake.buyer_per_day", "1", "2", "3");
        ExpectChoices("remake.fatigue_enabled", "开", "关");
        ExpectChoices("remake.corruption_box_settlement", "开", "关");
        // ST4 桶装 4 键(DefineBarrel ToggleChoices 逐字)
        ExpectChoices("remake.auto_barrel_semen", "开", "关");
        ExpectChoices("remake.auto_barrel_milk_wondrous", "开", "关");
        ExpectChoices("remake.auto_barrel_milk_rich", "开", "关");
        ExpectChoices("remake.auto_barrel_milk_diluted", "开", "关");
        // ST4 tooltip 重写 ×2:1.6.1 语义漂移(fatigue_enabled = 透支门禁,
        // corruption_box_settlement = 调原版循环+追加)——文案断言锁关键词
        Check.Eq(true, Find("remake.fatigue_enabled").Tooltip.Contains("透支"),
            "ST4 fatigue_enabled tooltip 注明透支门禁语义");
        Check.Eq(true, Find("remake.corruption_box_settlement").Tooltip.Contains("原版"),
            "ST4 corruption_box_settlement tooltip 注明原版结算+追加语义");

        // ---- 0.8.0:主 mod 官方设置写透 3 键(默认值/范围 = 主 mod ModSettings
        //      :91-93 Define 逐字镜像;单测钉 OurKey/TargetMod/TargetKey/四元组)----
        FamilyPanelEntry mlc = Find("main.maiden_loot_chance");
        Check.Eq(FamilyMods.MainMod, mlc.TargetMod, "0.8.0 maiden_loot_chance 目标 = 主 mod");
        Check.Eq("maiden_loot_chance", mlc.TargetKey, "0.8.0 maiden_loot_chance 对方键名逐字");
        Check.Eq(CarrierKind.WriteThrough, mlc.Carrier, "0.8.0 maiden_loot_chance 载体 = 写透");
        Check.Eq(0.05f, mlc.Default, "0.8.0 maiden_loot_chance 默认 0.05(官方 Define)");
        Check.Eq(0.05f, mlc.MinFloat, "0.8.0 maiden_loot_chance 下限 0.05");
        Check.Eq(0.25f, mlc.MaxFloat, "0.8.0 maiden_loot_chance 上限 0.25");
        FamilyPanelEntry mel = Find("main.maiden_enough_line");
        Check.Eq(FamilyMods.MainMod, mel.TargetMod, "0.8.0 maiden_enough_line 目标 = 主 mod");
        Check.Eq("maiden_enough_line", mel.TargetKey, "0.8.0 maiden_enough_line 对方键名逐字");
        Check.Eq(12, mel.Default, "0.8.0 maiden_enough_line 默认 12(官方 Define)");
        Check.Eq(2, mel.MinInt, "0.8.0 maiden_enough_line 下限 2");
        Check.Eq(30, mel.MaxInt, "0.8.0 maiden_enough_line 上限 30");
        FamilyPanelEntry olc = Find("main.ovulation_loot_chance");
        Check.Eq(FamilyMods.MainMod, olc.TargetMod, "0.8.0 ovulation_loot_chance 目标 = 主 mod");
        Check.Eq("ovulation_loot_chance", olc.TargetKey, "0.8.0 ovulation_loot_chance 对方键名逐字");
        Check.Eq(0.04f, olc.Default, "0.8.0 ovulation_loot_chance 默认 0.04(官方 Define)");
        Check.Eq(0f, olc.MinFloat, "0.8.0 ovulation_loot_chance 下限 0");
        Check.Eq(0.1f, olc.MaxFloat, "0.8.0 ovulation_loot_chance 上限 0.1");
        Check.Eq(true, mlc.Tooltip.Contains("写透") || mlc.Tooltip.Contains("本面板"),
            "0.8.0 写透注记在 tooltip(双面板并存说明)");
        // R9:IPH Choices 逐字(name_region 全角括号串逐字照抄,ModEntry.cs:115)
        ExpectChoices("iph.resolution", "128", "256");
        ExpectChoices("iph.name_region", "不限制（原版）", "仅日本", "仅中国", "仅中日韩");
        ExpectChoices("iph.portrait_gender", "仅女性（原版）", "不限性别");
        ExpectChoices("iph.tool_gender", "仅女性（原版）", "不限性别");
        ExpectChoices("iph.customer_conversion", "原版行为", "保留游戏原分布");

        // SliderInt 范围:WriteThrough = 对方 Define 范围(反编译 (int? min, int? max)
        // 逐字);SubModHook(brothel)= 本 mod 自定面板元数据(R-L,随 ② 一并锁定)。
        ExpectRange("brood.eggBaseChance", 0, 100);
        ExpectRange("brood.humanToEggChance", 0, 100);
        ExpectRange("brood.whiteGrowChance", 1, 80);
        ExpectRange("brood.whiteBodyChance", 0, 100);
        ExpectRange("brood.infectMajor", 0, 100);
        ExpectRange("brood.infectMinor", 0, 100);
        ExpectRange("brood.infectNone", 0, 100);
        ExpectRange("brood.tentacleSpeechFrequency", 0, 100);
        ExpectRange("brood.tentacleSpeechSpeed", 25, 300);
        ExpectRange("brood.tentacleSpeechHold", 1, 15);
        // R12:信息素两键 SliderFloat 范围(与主面板 client.semen_* 同 -100..999)
        ExpectRangeFloat(CfgKeys.BroodSemenPriceMod, -100f, 999f);
        ExpectRangeFloat(CfgKeys.BroodSemenBonusMod, -100f, 999f);
        ExpectRange(CfgKeys.BrothelBudgetBonus, 0, 999);
        ExpectRange(CfgKeys.BrothelDebtTags, 0, 20);
        ExpectRange(CfgKeys.BrothelDebtChance, 0, 100);
        ExpectRange(CfgKeys.BrothelGuestsBase, 1, 50);
        ExpectRange(CfgKeys.BrothelGuestsPerUpgrade, 0, 50);
        ExpectRange(CfgKeys.BrothelGuestsMax, 1, 50);
        ExpectRange(CfgKeys.BrothelTraitsBase, 1, 8);
        ExpectRange(CfgKeys.BrothelTraitsPerUpgrade, 0, 8);
        ExpectRange(CfgKeys.BrothelFeeDefault, 0, 9999);
        ExpectRange(CfgKeys.BrothelFeeMax, 1, 999999);
        ExpectRange(CfgKeys.BrothelMasteryThreshold, 1, 9999);
        ExpectRange(CfgKeys.BrothelVisitMinDays, 1, 99);
        ExpectRange(CfgKeys.BrothelVisitMaxDays, 1, 99);
        ExpectRange(CfgKeys.BrothelPriceMaidDress, 0, 999999);
        ExpectRange(CfgKeys.BrothelPriceCatEars, 0, 999999);
        ExpectRange(CfgKeys.BrothelPriceCatTailPlug, 0, 999999);
        ExpectRange(CfgKeys.BrothelPriceLaceDress, 0, 999999);
        ExpectRange(CfgKeys.BrothelPriceBunnyEars, 0, 999999);
        ExpectRange(CfgKeys.BrothelPriceLaceGarter, 0, 999999);
        ExpectRange(CfgKeys.BrothelStockFirst, 0, 6);
        ExpectRange(CfgKeys.BrothelStockMin, 0, 6);
        ExpectRange(CfgKeys.BrothelStockMax, 0, 6);
        ExpectRangeFloat(CfgKeys.TearlSpawnChance, 0f, 1f);
        ExpectRange(CfgKeys.TearlBudgetBase, 0, 1000000);
        ExpectRange(CfgKeys.TearlBudgetRandom, 0, 1000000);
        ExpectRange(CfgKeys.TearlSellDiscount, 0, 100);
        ExpectRange(CfgKeys.TearlBuyDiscount, 0, 100);
        ExpectRange(CfgKeys.TearlWaterPremium, 0, 100);
        // remake 差分范围(SubModHook = 本 mod 自定面板元数据,对方无 Define 可镜像)
        ExpectRange(CfgKeys.RemakeMilkPriceRich, 0, 999999);
        ExpectRange(CfgKeys.RemakeMilkPriceWondrous, 0, 999999);
        ExpectRange(CfgKeys.RemakeMilkPriceDiluted, 0, 999999);
        ExpectRange(CfgKeys.RemakeMilkFeedDiluted, 0, 100);
        ExpectRange(CfgKeys.RemakeMilkFeedWondrous, 0, 100);
        ExpectRange(CfgKeys.RemakeMilkFeedRich, 0, 100);
        ExpectRange(CfgKeys.RemakeMilkDrinkCredit, 0, 999999);
        ExpectRangeFloat(CfgKeys.RemakeMilkChanceRichBase, 0f, 1f);
        ExpectRangeFloat(CfgKeys.RemakeMilkChanceRichPerLact, 0f, 1f);
        ExpectRangeFloat(CfgKeys.RemakeMilkChanceWondrousBase, 0f, 1f);
        ExpectRangeFloat(CfgKeys.RemakeMilkChanceWondrousPerLact, 0f, 1f);
        ExpectRangeFloat(CfgKeys.RemakeMilkChanceDilutedBase, 0f, 1f);
        ExpectRangeFloat(CfgKeys.RemakeMilkChanceDilutedPerLact, 0f, 1f);
        // ST4:semen/alarm 五键条目已废弃——范围断言随删
        ExpectRange(CfgKeys.RemakeOvulationPrice, 0, 999999);
        // ST5:疲劳核心六键范围(倍率/基数 0-5、恢复 0-100;SubModHook 自定
        // 面板元数据——对方无 Define 可镜像)
        ExpectRangeFloat(CfgKeys.RemakeFatigueValueMulRested, 0f, 5f);
        ExpectRangeFloat(CfgKeys.RemakeFatigueValueMulTired, 0f, 5f);
        ExpectRangeFloat(CfgKeys.RemakeFatigueValueMulWeary, 0f, 5f);
        ExpectRangeFloat(CfgKeys.RemakeFatigueValueMulExhausted, 0f, 5f);
        ExpectRangeFloat(CfgKeys.RemakeFatigueNightlyGain, 0f, 5f);
        ExpectRange(CfgKeys.RemakeFatigueExhaustRecover, 0, 100);
        ExpectRange("iph.local_portrait_chance", 0, 100);
        // R10:属性概率两键的 SliderFloat 范围(与主面板 maiden.attr_* 同 0-1)
        ExpectRangeFloat(CfgKeys.IphAttrHighChance, 0f, 1f);
        ExpectRangeFloat(CfgKeys.IphAttrMidChance, 0f, 1f);

        // —— ③ 开关键派生 + 注册表可读性 ——
        var cfg = ConfigRegistry.CreateDefault();
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
        {
            // 0.7.1:SubModHook 的自开关 Toggle / EnumKeyCode 条目不派生 .enabled
            // (自开关 Toggle 的开关 = bool 自身[SwitchDispatch 表驱动];键位是
            // 纯参数无启停语义——派生 .enabled 会被 SwitchDispatch 第 1 条抢先
            // 命中恒 false,单元永不装)。其余条目:.enabled 注册且默认关。
            bool selfForm = e.Carrier == CarrierKind.SubModHook
                && (e.Kind == PanelKind.Toggle || e.Kind == PanelKind.EnumKeyCode);
            if (selfForm)
            {
                Check.Eq(false, cfg.ContainsKey(PanelKeys.ItemSwitchKey(e.OurKey)),
                    "③ 自开关/纯参数条目不派生 .enabled: " + e.OurKey);
            }
            else
            {
                string sw = PanelKeys.ItemSwitchKey(e.OurKey);
                try
                {
                    Check.Eq(false, cfg.Current<bool>(sw),
                        "③ 镜像开关注册且默认关: " + sw);
                }
                catch (KeyNotFoundException)
                {
                    Check.Eq(true, false, "③ 镜像开关键已注册: " + sw);
                }
            }
            try
            {
                if (e.Kind == PanelKind.Choice || e.Kind == PanelKind.Input
                    || e.Kind == PanelKind.EnumKeyCode) cfg.Current<string>(e.OurKey);
                else if (e.Kind == PanelKind.SliderInt) cfg.Current<int>(e.OurKey);
                else if (e.Kind == PanelKind.SliderFloat) cfg.Current<float>(e.OurKey);
                else cfg.Current<bool>(e.OurKey);
                Check.Eq(true, true, "③ 值键按 Kind 类型可读: " + e.OurKey);
            }
            catch (Exception ex)
            {
                Check.Eq(true, false, "③ 值键可读(" + e.OurKey + "): " + ex.Message);
            }
        }

        // —— ④ 条目数与分区归属 ——
        Check.Eq(99, FamilyPanelKeys.Entries.Count, "④ 条目 28+16+22+9+21+3=99(0.8.0: 96 + tearl Aquashard 3)");
        int remakeCount = 0, broodCount = 0, brothelCount = 0, tearlCount = 0, iphCount = 0, mainCount = 0;
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
        {
            if (e.TargetMod == FamilyMods.MainMod)
            {
                mainCount++;
                Check.Eq("主 mod 官方设置(拾荒)", e.Section, "④ 主 mod 写透分区名: " + e.OurKey);
            }
            else if (e.TargetMod == FamilyMods.Remake)
            {
                remakeCount++;
                Check.Eq("暗流交易联动", e.Section, "④ remake 分区名: " + e.OurKey);
            }
            else if (e.TargetMod == FamilyMods.Brood)
            {
                broodCount++;
                Check.Eq("巢穴扩展联动", e.Section, "④ brood 分区名: " + e.OurKey);
            }
            else if (e.TargetMod == FamilyMods.Tearl)
            {
                tearlCount++;
                Check.Eq("tearlaments 联动", e.Section, "④ tearl 分区名: " + e.OurKey);
            }
            else if (e.TargetMod == FamilyMods.Iph)
            {
                iphCount++;
                Check.Eq("物品高清立绘管理器联动", e.Section, "④ iph 分区名: " + e.OurKey);
            }
            else
            {
                brothelCount++;
                Check.Eq("掌上青楼联动", e.Section, "④ brothel 分区名: " + e.OurKey);
            }
        }
        Check.Eq(28, remakeCount, "④ remake 条目 28(写透 8[ST4: 7−3+4]+ 差分接管 20[ST4: 19−5;R7 +ovulation_price;ST5 +疲劳核心 6])");
        Check.Eq(16, broodCount, "④ brood 条目 16(写透 12 + 信息素接管 2[R12] + 拾取器 2[0.7.1])");
        Check.Eq(22, brothelCount, "④ brothel 接管 22 键");
        Check.Eq(9, tearlCount, "④ tearl 接管 9 键(0.8.0: 6 + Aquashard 3)");
        Check.Eq(21, iphCount, "④ iph 条目 21(写透 16[R9,17 键扣 hotkey]+ 属性共存 5[R10])");
        Check.Eq(3, mainCount, "④ 主 mod 官方写透 3 键(0.8.0;ModSettings 拾荒分区镜像)");

        // —— ⑤ 目录耦合:载体单元键集 == 同载体条目 OurKey 集(R10 后 MainHook
        //     镜像条目的键集 = main.maiden_attrs_iph 单元,与 WriteThrough/
        //     SubModHook 三者各自独立) ——
        var wtKeys = new HashSet<string>(StringComparer.Ordinal);
        var smKeys = new HashSet<string>(StringComparer.Ordinal);
        var mhKeys = new HashSet<string>(StringComparer.Ordinal);
        int wtUnits = 0, smUnits = 0, mhUnits = 0;
        foreach (HookUnitDef u in HookCatalog.Units)
        {
            if (u.Kind == CarrierKind.WriteThrough)
            {
                wtUnits++;
                foreach (string k in u.Keys) wtKeys.Add(k);
            }
            else if (u.Kind == CarrierKind.SubModHook)
            {
                smUnits++;
                foreach (string k in u.Keys) smKeys.Add(k);
            }
            else if (u.Kind == CarrierKind.MainHook
                && u.TargetMod != FamilyMods.MainMod && u.TargetMod != null)
            {
                // R10:家族绑定 MainHook 单元(main.maiden_attrs_iph,TargetMod=IPH)
                mhUnits++;
                foreach (string k in u.Keys) mhKeys.Add(k);
            }
        }
        Check.Eq(39, wtUnits, "⑤ WriteThrough 单元恰 39(0.8.0: 36 + wt.main.* 3)");
        Check.Eq(20, smUnits, "⑤ SubModHook 单元恰 20(0.8.0: 19 + tearl.aquashard)");
        Check.Eq(1, mhUnits, "⑤ 家族绑定 MainHook 单元恰 1(main.maiden_attrs_iph,R10)");
        var wtEntryKeys = new HashSet<string>(StringComparer.Ordinal);
        var smEntryKeys = new HashSet<string>(StringComparer.Ordinal);
        var mhEntryKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
        {
            if (e.Carrier == CarrierKind.WriteThrough) wtEntryKeys.Add(e.OurKey);
            else if (e.Carrier == CarrierKind.SubModHook) smEntryKeys.Add(e.OurKey);
            else mhEntryKeys.Add(e.OurKey);
        }
        Check.Eq(wtEntryKeys.Count, wtKeys.Count, "⑤ 写透单元键集与镜像表同大");
        foreach (string k in wtEntryKeys)
            Check.Eq(true, wtKeys.Contains(k), "⑤ 镜像键有写透单元: " + k);
        // ST4:R7 时代的跨族等价消费(nightly.semen_chance_cap × remake.semen_formula)
        // 随单元注销终结——SubModHook 键集与接管条目表恰等(双射,无 +1 例外)
        Check.Eq(smEntryKeys.Count, smKeys.Count,
            "⑤ 接管单元键集 = 条目表恰等(ST4: 跨族键例外终结,R7 的 +1 撤销)");
        foreach (string k in smEntryKeys)
            Check.Eq(true, smKeys.Contains(k), "⑤ 接管键有 SubModHook 单元: " + k);
        // R10:属性五键条目 ↔ main.maiden_attrs_iph 单元键集恰等(双射)
        Check.Eq(mhEntryKeys.Count, mhKeys.Count, "⑤ R10 共存条目与单元键集同大");
        foreach (string k in mhEntryKeys)
            Check.Eq(true, mhKeys.Contains(k), "⑤ R10 共存键有 MainHook 单元: " + k);

        // —— 查找 API:命中 / 未知键 false ——
        FamilyPanelEntry hit;
        Check.Eq(true, FamilyPanelKeys.TryGet("remake.auto_barrel_semen", out hit)
            && hit.TargetKey == "auto_barrel_semen", "TryGet 命中(ST4 桶装新键)");
        Check.Eq(false, FamilyPanelKeys.TryGet("remake.nope", out _), "TryGet 未知键 false");
        Check.Eq(false, FamilyPanelKeys.TryGet("price.milk", out _), "TryGet 主面板键不误命中");
    }

    /// <summary>逐字默认值断言 + Kind↔Default 的 CLR 类型形态一致。</summary>
    private static void Expect(string ourKey, object def, PanelKind kind)
    {
        FamilyPanelEntry e = Find(ourKey);
        Check.Eq(kind, e.Kind, "② Kind: " + ourKey);
        Check.Eq(def, e.Default, "② 默认值逐字: " + ourKey);
        bool typeOk = def is string ? e.Default is string
            : def is int ? e.Default is int
            : def is float ? e.Default is float
            : e.Default is bool;
        Check.Eq(true, typeOk, "② Default 装箱类型与期望一致: " + ourKey);
    }

    private static void ExpectChoices(string ourKey, params string[] choices)
    {
        FamilyPanelEntry e = Find(ourKey);
        Check.Eq(choices.Length, e.Choices != null ? e.Choices.Length : 0,
            "② Choices 档数: " + ourKey);
        if (e.Choices == null) return;
        for (int i = 0; i < choices.Length; i++)
            Check.Eq(choices[i], e.Choices[i], "② Choices[" + i + "]: " + ourKey);
    }

    private static void ExpectRange(string ourKey, int min, int max)
    {
        FamilyPanelEntry e = Find(ourKey);
        Check.Eq(min, e.MinInt, "② SliderInt min: " + ourKey);
        Check.Eq(max, e.MaxInt, "② SliderInt max: " + ourKey);
    }

    private static void ExpectRangeFloat(string ourKey, float min, float max)
    {
        FamilyPanelEntry e = Find(ourKey);
        Check.Eq(min, e.MinFloat, "② SliderFloat min: " + ourKey);
        Check.Eq(max, e.MaxFloat, "② SliderFloat max: " + ourKey);
    }

    private static FamilyPanelEntry Find(string ourKey)
    {
        foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            if (e.OurKey == ourKey) return e;
        throw new KeyNotFoundException("测试期望的镜像键不在表中: " + ourKey);
    }
}
