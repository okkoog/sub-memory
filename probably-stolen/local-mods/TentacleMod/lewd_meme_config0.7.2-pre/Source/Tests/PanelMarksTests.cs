using System;
using System.Collections.Generic;
using System.Reflection;
using LewdMemeConfig;

// Task 8(M3d):PanelMarks 面板标记表(纯)——Router.DecideAll 裁决 → 面板键 → 标记文案。
// 契约(spec §4.6/§5.4 + 控制裁定 R-C):
//  ① 版本不匹配目标的组内每个值键与派生开关键 → 恒 "版本不匹配,该项不生效"
//  ② 让位组 → Router.YieldNote(单元)("启用<owner 显示名>,该配置不生效",
//     0.06 轮 F4 文案修订;面板渲染侧让位文案另做绿色小字弱化,F2——版本文案
//     保持原样式,IsVersionMark 为渲染分流探针);
//  ③ 正常组(Own)→ null;未知键恒 null;
//  ④ 同键版本×让位 → 版本优先(R5:VersionBlocked 压过 Yield——经 DecideAll 的单元级
//     优先级成立:主 mod 版本不符时让位单元整体判 VersionBlocked,标记随版本);
//  ⑤ Rebuild 只写自身内存表(非破坏性红线):两次 Rebuild 间 For 随输入可变(无残留),
//     公开面恰 Rebuild/For 两个方法(除 Rebuild 外无写入入口)——结构锁法见报告。
//  F5 键级聚合:一键多消费单元时全部单元均被压制/让位才标记(任一 Own 即不标)
//     ——混合消费键(gender_scope 的 game.*×2 + main.on_generated;nightly 组与
//     manual_render 反界单元共享键;brood_size 与非让位布局单元共享)在部分让位
//     场景不标;全让位键照标,文案取目录序首个让位单元(多 owner 固定序)。
// 渲染(PanelRowPatch prefix 读 For)归游戏侧,构建绿即验收;效果归 Task 15。
public static class PanelMarksTests
{
    // spec §4.6 固定文案(R5 修订:去「***」前缀;与实现各持一份防漂移)。
    private const string VersionText = "版本不匹配,该项不生效";

    public static void Run()
    {
        // ① 版本不匹配(仅主 mod 在场、版本不符,其余家族缺席):54 个 lewd_meme
        //    目标单元(56 总数 - 2 game.*;R7 +loot_maiden_chance/maiden_traits,
        //    R9 maiden_traits 拆 2,T4 nightly_tick 拆 2,T9 +三新单元
        //    -pools.maiden)的每个键
        //    (含派生开关键)都返回固定文案。例外 gender_scope(F5 混合消费键:
        //    main.on_generated 被压制,game.* 两单元 TargetMod=null 照常 Own →
        //    该键不标,下方单独断言)。
        IReadOnlyDictionary<string, FamilyState> wrongMain = Fam(null, new[] { FamilyMods.MainMod });
        PanelMarks.Rebuild(wrongMain);
        int mainUnits = 0;
        foreach (HookUnitDef u in HookCatalog.Units)
        {
            if (u.TargetMod != FamilyMods.MainMod) continue;
            mainUnits++;
            if (u.Keys == null) continue;
            foreach (string key in u.Keys)
            {
                if (key == CfgKeys.GenderScope) continue; // F5 混合键,下方单测
                Check.Eq(VersionText, PanelMarks.For(key),
                    "版本不匹配: " + key + " 标记");
                Check.Eq(VersionText, PanelMarks.For(PanelKeys.ItemSwitchKey(key)),
                    "版本不匹配: " + key + " 派生开关键标记");
            }
        }
        Check.Eq(54, mainUnits, "lewd_meme 目标单元恰 54(T9 后;56 总数 - 2 game.*)");
        // ST4:semen_chance_cap 跨族例外终结——remake.semen_formula 注销后
        // 该键唯一消费 main.nightly_transpile(TargetMod=lewd_meme),主 mod 版本
        // 不命中即标(与组内其余键一致,不再跳过)
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.SemenChanceCap),
            "ST4: cap 归属 nightly_transpile 唯一消费——版本压制照标(跨族豁免终结)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.GenderScope),
            "F5 混合键: main.on_generated 压制但 game.*×2 Own 照常 → gender_scope 不标");

        // ② 让位组:单 owner 直出、DataWrite 让位、勘察反界 null、Brood/IPH 各自独立。
        //    (0.06 轮 F4 文案修订:「启用<owner>,该配置不生效」;R7 恢复的
        //    create_alarm/trap_tactic 等 8 单元不再标让位。)
        IReadOnlyDictionary<string, FamilyState> remake = Fam(new[] { FamilyMods.Remake }, null);
        PanelMarks.Rebuild(remake);
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AlarmPrice),
            "R7 恢复: create_alarm Own → alarm_price 不标(remake 零触碰创建链)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AlarmNpcSellChance),
            "R7 恢复: alarm_try_sell_npc Own → npc_sell_chance 不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.TrapTacticChances),
            "R7 恢复: DataWrite pools.trap_tactic Own → tactic_chances 不标(数组活引用在 transpile 保留区照读)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.ScavMaidenBase),
            "R7 等价恢复: on_loot_rolled Yield 但 loot_maiden_chance Own → scav_maiden_base 不标(键级聚合任一 Own 即不标)");
        Check.Eq((string)null, PanelMarks.For(PanelKeys.ItemSwitchKey(CfgKeys.ScavMaidenBase)),
            "R7 等价恢复: scav_maiden_base 派生开关键同不标");
        Check.Eq("启用暗流交易,该配置不生效", PanelMarks.For(CfgKeys.PriceOvulationShot),
            "remake 让位: ovulation_shot(评审补充键;R7 后等价点在联动分区 remake.ovulation_price,主面板键仍标)");
        // T10 补 + 1.6.1 更新:让位行的标记面全覆盖(9 行;缺的补齐;期望文案
        // 硬编码与实现对峙)。1.6.1 翻转:remake 在场仅 ovulation_shot 一键标
        // 绿字(alarm/trap/nightly/produce_daughter/milk 面单元恢复 Own)。
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AlarmOralPerNight),
            "1.6.1 恢复: alarm_nightly_once Own → oral_per_night 不标(插入式共存)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AlarmSexPerNight),
            "1.6.1 恢复: alarm_nightly_once(sex)不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AlarmAnalPerNight),
            "1.6.1 恢复: alarm_nightly_once(anal)不标");
        Check.Eq((string)null, PanelMarks.For(PanelKeys.ItemSwitchKey(CfgKeys.AlarmSexPerNight)),
            "1.6.1 恢复: alarm_nightly_once 派生开关键同不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.TrapSemenChance),
            "1.6.1 恢复: trap_nightly_once Own → semen_chance 不标(tactic_chances 混合 pools.trap_tactic Own 亦不标)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.LactationCap),
            "1.6.1 恢复: nightly_transpile Own → lactation_cap 不标(唯一消费单元撤让)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.PriceMilkCredit),
            "1.6.1: price_milk_credit 改 Brood 独有——remake 单独在场 Own 不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.LevelCap),
            "remake 在场: level_cap 无标记(Own)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.PriceSemen),
            "remake 在场: price_semen 无标记(勘察反界)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.GuestDisplayEnabled),
            "remake 在场: guest_display 无标记(DataWrite 恒 Own)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.BroodSize),
            "F5: brood_size 混合消费(create_tentacle_alarm[R7 恢复 Own]+ build_window/create/manual_render Own)不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.DaughterValue),
            "1.6.1: produce_daughter 恢复 Own → daughter_value 不标(撤让后无让位单元)");

        IReadOnlyDictionary<string, FamilyState> brood = Fam(new[] { FamilyMods.Brood }, null);
        PanelMarks.Rebuild(brood);
        Check.Eq((string)null, PanelMarks.For(CfgKeys.ExpBase),
            "T9: exp 公式三键不让 Brood(1.9.2 EXP_NEED 字段已删,Brood 直写失去落点,公式接管恒 Own)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.Factions),
            "R9 拆分: factions 属 pools.maiden_fbb(owners=[IPH],Brood 不在 owner 表),brood 在场不标;静态写经 WriteFbbAtCompose 照常");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.Jobs),
            "R9 拆分: jobs 属 pools.maiden_jobs(恒 Own),brood 在场不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.SexPerLevelMin),
            "T4: sex_per_level_min 已折叠 DataOnly(1.9.2 IL 无消费点)——brood 让位亦无标记面;tooltip 注记归 Task 9");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.MilkValueBase),
            "F5: milk_value_base 混合消费(nightly_milk_value 让位 + manual_render Own 反界)不标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.DaughterValue),
            "F5: daughter_value 唯一消费 produce_daughter 恒 Own(1.6.1 撤让)不标");
        Check.Eq("启用巢穴扩展,该配置不生效", PanelMarks.For(CfgKeys.SemenPriceMod),
            "brood 让位: semen_effects(Task 7 审计)");
        // T10 补:semen_effects 三键全标(alarm 键反向对照:brood 不让 alarm 单元 → 不标)
        Check.Eq("启用巢穴扩展,该配置不生效", PanelMarks.For(CfgKeys.SemenBonusMod),
            "brood 让位: semen_effects 三键(bonus_mod 同单元)");
        Check.Eq("启用巢穴扩展,该配置不生效", PanelMarks.For(CfgKeys.SemenRepMult),
            "brood 让位: semen_effects 三键(rep_mult 同单元)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AlarmOralPerNight),
            "brood 反向对照: alarm_nightly_once 恒 Own(1.6.1 撤让,无 owner)→ 不标");
        // T10 补:乳汁两键混合消费(nightly_milk_value 让位 + manual_render Own)双键都不标
        Check.Eq((string)null, PanelMarks.For(CfgKeys.MilkValuePerLv),
            "F5: milk_value_per_lv 混合消费(nightly_milk_value 让位 + manual_render Own 反界)不标(与 base 同)");
        // T10 补:经验公式三键让位删除后(T9 撤 pools.maiden 的 R2 行)标记一致性
        Check.Eq((string)null, PanelMarks.For(CfgKeys.ExpDivisor),
            "T9: exp_divisor 不让 Brood(公式三键恒 Own,标记一致)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.ExpStep),
            "T9: exp_step 不让 Brood(公式三键恒 Own,标记一致)");
        // T10 补 + 1.6.1 更新:milk 两键 Brood 在场单 owner 全列(remake 撤让后
        // owner 集只剩 Brood,文案不再拼双 owner)
        Check.Eq("启用巢穴扩展,该配置不生效", PanelMarks.For(CfgKeys.PriceMilk),
            "brood 让位: price_milk(1.6.1 后 Brood 独有)");
        Check.Eq("启用巢穴扩展,该配置不生效", PanelMarks.For(CfgKeys.PriceMilkCredit),
            "brood 让位: price_milk_credit 同上");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.PortraitSize),
            "brood 在场: IPH 单元键无标记(owner 条件独立)");

        IReadOnlyDictionary<string, FamilyState> iph = Fam(new[] { FamilyMods.Iph }, null);
        PanelMarks.Rebuild(iph);
        Check.Eq("启用物品高清立绘管理器,该配置不生效", PanelMarks.For(CfgKeys.PortraitSize),
            "iph 让位: portrait_size");
        // R9:fbb 三池键让位标注(0x0c 实测:阵营/胸部池 CSV 失效,体型池仅抽签域
        // 收窄——统一诚实标注);JOBS 不标(iph_1217 零触碰职业抽取)
        Check.Eq("启用物品高清立绘管理器,该配置不生效", PanelMarks.For(CfgKeys.Factions),
            "R9 让位: factions 属 pools.maiden_fbb(IPH 工厂 transpiler 替换阵营抽取)");
        Check.Eq("启用物品高清立绘管理器,该配置不生效", PanelMarks.For(CfgKeys.Bodies),
            "R9 让位: bodies(PickBody 替换,权重归其 body_weights_csv)");
        Check.Eq("启用物品高清立绘管理器,该配置不生效", PanelMarks.For(CfgKeys.Breasts),
            "R9 让位: breasts(PickBreastForBody 默认走其体型-乳量矩阵)");
        Check.Eq("启用物品高清立绘管理器,该配置不生效", PanelMarks.For(PanelKeys.ItemSwitchKey(CfgKeys.Bodies)),
            "R9 让位: bodies 派生开关键同标");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.Jobs),
            "R9 不标: jobs 属 pools.maiden_jobs 恒 Own(IPH 零触碰)");

        IReadOnlyDictionary<string, FamilyState> dual = Fam(new[] { FamilyMods.Remake, FamilyMods.Brood }, null);
        PanelMarks.Rebuild(dual);
        Check.Eq("启用巢穴扩展,该配置不生效", PanelMarks.For(CfgKeys.PriceMilk),
            "1.6.1 后 price_milk 唯一 owner = Brood(双在场不再拼 remake;全让位键)");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.SexMinBase),
            "1.6.1: nightly_transpile 恢复 Own → sex_min_base 双消费全 Own 不标");

        // ③ 正常组:空场全 null;未知键恒 null。
        IReadOnlyDictionary<string, FamilyState> empty = Fam(null, null);
        PanelMarks.Rebuild(empty);
        Check.Eq((string)null, PanelMarks.For(CfgKeys.LevelCap), "空场: level_cap null");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.GenderScope), "空场: gender_scope null");
        Check.Eq((string)null, PanelMarks.For(CfgKeys.InitialVirgin), "空场: initial_virgin null(Own 单元)");
        Check.Eq((string)null, PanelMarks.For("main.nope"), "未知键恒 null");

        // ④ 同键版本×让位 → 版本优先:remake 在场 + 主 mod 版本不符 → 让位单元键
        //    返回版本文案,YieldNote 被压过。
        IReadOnlyDictionary<string, FamilyState> remakeWrong =
            Fam(new[] { FamilyMods.Remake }, new[] { FamilyMods.MainMod });
        PanelMarks.Rebuild(remakeWrong);
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.PriceOvulationShot),
            "版本×让位: 版本文案胜出(ovulation_shot,仍让位单元)");
        Check.Eq(true, PanelMarks.For(CfgKeys.PriceOvulationShot) != Router.YieldNote("main.price_ovulation_shot"),
            "版本×让位: YieldNote 被压过(ovulation_shot)");
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.SexMinBase),
            "版本×让位: 版本文案胜出(nightly)");
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.AlarmPrice),
            "版本×让位: R7 恢复单元 create_alarm 亦 VersionBlocked(版本压制压过 Own),版本文案");

        // ④' R10:main.maiden_attrs_iph(TargetMod = IPH)三态的标记面——在场且
        //     版本匹配 → Own 五键不标(分区可见、补丁可装);版本漂移 →
        //     VersionBlocked 五键灰字「版本不匹配,该项不生效」(用户裁定的
        //     自动失效;数值行与派生开关键都标);IPH 缺席 → Own 不标(分区
        //     整体不 Define,玩家看不到,无标记面)。
        IReadOnlyDictionary<string, FamilyState> iphOnly = Fam(new[] { FamilyMods.Iph }, null);
        PanelMarks.Rebuild(iphOnly);
        Check.Eq((string)null, PanelMarks.For(CfgKeys.IphAttrHighChance),
            "R10: iph 在场匹配 → attrs 五键不标(Own,共存装配)");
        Check.Eq((string)null, PanelMarks.For(PanelKeys.ItemSwitchKey(CfgKeys.IphAttrLowRange)),
            "R10: iph 在场匹配 → attrs 派生开关键不标");
        IReadOnlyDictionary<string, FamilyState> iphDrift = Fam(null, new[] { FamilyMods.Iph });
        PanelMarks.Rebuild(iphDrift);
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.IphAttrHighChance),
            "R10: iph 版本漂移 → attrs 五键灰字(版本文案)");
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.IphAttrMidChance),
            "R10: iph 版本漂移 → mid_chance 灰字");
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.IphAttrHighRange),
            "R10: iph 版本漂移 → high_range 灰字");
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.IphAttrMidRange),
            "R10: iph 版本漂移 → mid_range 灰字");
        Check.Eq(VersionText, PanelMarks.For(CfgKeys.IphAttrLowRange),
            "R10: iph 版本漂移 → low_range 灰字");
        Check.Eq(VersionText, PanelMarks.For(PanelKeys.ItemSwitchKey(CfgKeys.IphAttrHighChance)),
            "R10: iph 版本漂移 → attrs 派生开关键同标");
        Check.Eq(true, PanelMarks.IsVersionMark(PanelMarks.For(CfgKeys.IphAttrHighChance)),
            "R10: attrs 漂移标记被识别为版本文案(灰色渲染)");
        // 主面板 attr_* 五键(0.7.0 T8 拆段后归 main.maiden_roll,Own 共存单元,
        // 不在让位表):IPH 漂移/在场均不标——数值经 RollAttr transpiler 照常生效,
        // 让位绿字随拆段消失(0.6.0 时它属 main.create_maiden 的 R4 让位面)
        Check.Eq((string)null, PanelMarks.For(CfgKeys.AttrHighChance),
            "T8: 主面板 attr_high_chance 归 maiden_roll(Own)→ 无让位注记(iph 漂移场景)");

        // ⑤ 非破坏性红线(结构锁):表内容完全由 Rebuild 输入决定(可翻转、无残留);
        //    公开面恰 Rebuild/For——除 Rebuild(只接受家族快照)外无任何写入入口,
        //    类不引用 ConfigRegistry/ModConfig(纯逻辑,不进注册表与 json)。
        //    渲染分流探针(F2):IsVersionMark 对固定版本文案 true、让位文案 false。
        Check.Eq(true, PanelMarks.IsVersionMark(VersionText), "F2: 版本文案被识别(保持原样式)");
        Check.Eq(true, PanelMarks.IsVersionMark(PanelMarks.VersionMismatchText), "F2: 常量自识别");
        Check.Eq(false, PanelMarks.IsVersionMark(Router.YieldNote("main.on_loot_rolled")),
            "F2: 让位文案不被误判(绿色小字路径;1.6.1 后 trap_nightly_once 撤让,换仍让位的 on_loot_rolled)");
        Check.Eq(false, PanelMarks.IsVersionMark(null), "F2: null 非版本文案");
        PanelMarks.Rebuild(remake);
        Check.Eq(true, PanelMarks.For(CfgKeys.PriceOvulationShot) != null, "重灌: remake 标记恢复(ovulation_shot)");
        PanelMarks.Rebuild(empty);
        Check.Eq((string)null, PanelMarks.For(CfgKeys.PriceOvulationShot), "重灌: 空场清空旧标记(无残留)");
        MethodInfo[] surface = typeof(PanelMarks)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        Check.Eq(2, surface.Length, "公开静态方法恰 2 个(Rebuild/For,无其他写入 API)");
        int seen = 0;
        foreach (MethodInfo m in surface)
            if (m.Name == "Rebuild" || m.Name == "For") seen++;
        Check.Eq(2, seen, "方法名恰为 Rebuild 与 For");
        Check.Eq(0, typeof(PanelMarks)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Length,
            "无公开字段");
        Check.Eq(0, typeof(PanelMarks)
            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Length,
            "无公开属性");
    }

    // —— 构造辅助(镜像 RouterTests.Fam):全 8 家族键的快照。ok = Present 且版本命中;
    //    wrong = Present 但版本不符(9.9.9);其余缺席(Version=null / VersionOk=false)。——
    private static IReadOnlyDictionary<string, FamilyState> Fam(string[] ok, string[] wrong)
    {
        var map = new Dictionary<string, FamilyState>(StringComparer.Ordinal);
        foreach (string id in new[]
        {
            FamilyMods.MainMod, FamilyMods.Brood, FamilyMods.Remake, FamilyMods.Brothel,
            FamilyMods.Tearl, FamilyMods.Legacy, FamilyMods.Crisp, FamilyMods.Iph,
        })
        {
            map[id] = new FamilyState { ModId = id, Present = false, Version = null, VersionOk = false };
        }
        if (ok != null)
        {
            foreach (string id in ok)
                map[id] = new FamilyState { ModId = id, Present = true, Version = FamilyMods.BoundVersions[id], VersionOk = true };
        }
        if (wrong != null)
        {
            foreach (string id in wrong)
                map[id] = new FamilyState { ModId = id, Present = true, Version = "9.9.9", VersionOk = false };
        }
        return map;
    }
}
