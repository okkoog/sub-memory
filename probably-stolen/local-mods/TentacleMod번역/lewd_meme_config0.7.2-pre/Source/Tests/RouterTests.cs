using System;
using System.Collections.Generic;
using LewdMemeConfig;

// Task 6(M3b):Router 让位裁决(纯逻辑)——在场 × 版本 → Own/Yield/VersionBlocked。
// 规则契约(spec §5.4 六条,单元映射已对勘察事实固化;0.6.0 R7 方法级精审;
// 0.7.0 Task 4 nightly 拆分;ST 1.6.1 让位翻转[ShadowTrade 波次 Task 3,
// 2026-11-17]:ShadowTrade 1.6.1[mod ID 仍是 lewd_meme_remake]撤掉 1.5.x 的
// 夜间活动接管,改为对主 mod 三个 Nightly 方法的**插入式共存 transpiler**
// (只插入不删除,原版 NightlyTick/ProduceDaughter/NightlyTickOnce 消费点
// 复活)——R1 自 9 单元缩至 2,乳汁三单元 owner 组 [Remake, Brood] →
// [Brood];IL 命中依据 = 勘察摘要「1.6.1 让位翻转·IL 共存推演」节
// [21 注入点 = 17 无扰动 + 4 重锚,重锚已进 TranspilePlan]):
//  R1 remake 在场 → 2 单元 Yield:main.on_loot_rolled(OnLootRolled 被
//     transpiler NOP 掉 0.5s 限流,复刻体自带节流 = 反对其「无限流」设计;
//     概率键 scav_maiden_base 经 main.loot_maiden_chance 等价恢复[R7])+
//     main.price_ovulation_shot(remake postfix 强制单价 100;remake.ovulation_price
//     等价恢复[R7])。
//     1.6.1 恢复 Own 的 4 单元(夜间面注入点对改写后 IL 全部可命中):
//     main.nightly_transpile / main.produce_daughter / main.alarm_nightly_once /
//     main.trap_nightly_once——0.6/0.7 时代「transpiler 删消费点区段/prefix
//     跳过被改写方法体」的让位前提随 1.6.1 插入式共存而消失。历史恢复
//     (勿回退):R7 恢复的 8 单元(create_alarm/create_tentacle_alarm/
//     alarm_try_sell_npc/create_manhole/create_trap/trap_sell_scav/
//     trap_sell_thief/pools.trap_tactic)。
//     反向钉住(勘察:不让位):main.price_semen(目录仍走主 mod CreateSemen 且
//     remake 不覆价)/ main.price_tentacle_box(remake 零接触)/ main.add_body_exp
//     (ExpMultiplier ∉ NightlyTick 组,DebugAddExp 路径残留有效)/ main.tame_text /
//     main.manual_render(键为混合集)/ main.night_report(remake 仅改 2 个标签)。
//  R2 Brood 在场 → 4 单元 Yield(T9:6 → 5 删 pools.maiden;1.6.1:milk 三单元
//     自 R1∩R2 移入,remake 撤让后 Brood 成唯一 owner——竞争写者只剩其夜间
//     unitValue 重算):
//     main.nightly_milk_value(乳汁面 2 键;数值面 transpile 不随 Brood 让位)
//     + main.price_milk/milk_credit + main.semen_effects(Task 7 审计新增:三键
//     全接管——±10 信息素压制
//     Fragrance.cs:39/43 + 声望倍率强制 pre×1.1 BaseLink.cs:74/452-481)。
//  R3(1.6.1 后撤销)main.price_milk/milk_credit 曾是 remake/Brood 并集;remake
//     侧撤让后归 R2 独有,并集规则不复存在。
//  R4 IPH 在场 → main.create_maiden / main.create_maiden_of_faction(0.5.1 让位
//     先例;portrait_size 键恰在此两单元)+ pools.maiden_fbb(R9:阵营/体型/胸部
//     三池静态写让位——其工厂 transpiler 替换三池 Pick,0x0c 实测默认路径无
//     消费方;JOBS 零替换 → pools.maiden_jobs 恒 Own,经验表在 pools.maiden
//     均与 IPH 让位无关)。R10 例外形态:main.maiden_attrs_iph(TargetMod=IPH)
//     **不让位**——postfix 与 IPH 工厂 transpiler 正交共存(iph_1217 属性段
//     零引用),三态治理走版本压制通道(在场匹配 Own/漂移 VersionBlocked/
//     缺席 Own),见 ⑥'。
//  R5 版本压制:单元 TargetMod(lewd_meme)在场但版本不符 → VersionBlocked 压过
//     一切(含 Yield);TargetMod=null(game.*)永不压制;owner(remake/Brood/IPH)
//     版本漂移不改变让位——Present 即让位,对方已接管语义;TargetMod 缺席不压制
//     (回落正常裁决,缺席由装配层 resolver 处理)。
//  R6 YieldNote(0x06 轮 F4 文案修订):"启用<owner 显示名>,该配置不生效";双
//     owner 按固定序用 / 连接(暗流交易、巢穴扩展、物品高清立绘管理器;ST6 显示名);非让位
//     单元(含未知 id)返回 null。
// DataWrite 语义:无让位规则命中、无版本压制时恒 Own(pools.guest_display /
// pools.maiden_jobs / pools.trap_tactic[R7 恢复;T9 后 DataWrite 共 4])；
// pools.maiden_fbb(R9/R4)的 Yield 是键级规则的显式覆盖。
public static class RouterTests
{
    // —— 期望的让位单元全集(9 项;ST 1.6.1 让位翻转:R1 9 → 2 + milk 三单元
    //    移入 R2,13 → 9[R1 独有 2 + R2 独有 4 + R4 3];Task 8 布线与
    //    Task 7 审计消费同一份清单)——
    private static readonly string[] RemakeOnly =
    {
        "main.on_loot_rolled",
        "main.price_ovulation_shot",
    };
    // 1.6.1:milk 三单元自 R1∩R2 移入(TickTranspiler 只插入不删除,原版
    // NightlyTick/CreateMilk 消费点复活,remake 撤让);semen_effects 自 T7
    private static readonly string[] BroodOnly =
    {
        "main.nightly_milk_value", "main.price_milk", "main.price_milk_credit",
        "main.semen_effects",
    };
    private static readonly string[] IphOnly =
    {
        "main.create_maiden", "main.create_maiden_of_faction",
        // R9:三池抽取替换让位(OriginalCompat.cs:287/:325/:294,num7/num8 :339 不分模式)
        "pools.maiden_fbb",
    };

    public static void Run()
    {
        // ① 空场(全家族缺席):111 单元全 Own;DataWrite 无命中恒 Own;game.* 恒 Own;
        //    WriteThrough 恒 Own(R-F,与在场无关——见 ⑫);SubModHook(brothel.*/
        //    tearl.*/remake.*)无让位规则,同恒 Own(Task 14b +main.printer、
        //    0.6.0 R4 +remake.* 4、R7 +remake.ovulation_price、ST5 +remake 疲劳
        //    两单元,与 R7 +getter/
        //    四池拆分 2 个主 mod 单元,无让位规则亦恒 Own;R10 +main.maiden_
        //    attrs_iph[TargetMod=IPH,缺席不压制 Own——面板不 Define 开关恒 false
        //    = Absent 零 hook 由装配层承担];0.7.0 Task 4 nightly_tick 拆 2;
        //    Task 9 +interact_threshold/exp_formula/initial_virgin 三 Own 单元
        //    -pools.maiden)。
        IReadOnlyDictionary<string, RouteDecision> none = Router.DecideAll(Fam(null, null));
        Check.Eq(112, HookCatalog.Units.Count, "catalog has 112 units(0.7.1: 111 +brood.tag_picker)");
        Check.Eq(112, none.Count, "DecideAll covers all 112 units");
        foreach (HookUnitDef u in HookCatalog.Units)
            Check.Eq(RouteDecision.Own, none[u.Id], "empty family: " + u.Id + " Own");
        Check.Eq(RouteDecision.Own, Router.Decide("pools.guest_display", Fam(null, null)),
            "DataWrite guest_display Own without suppression");

        // ② R1 双方向:remake 在场 → 2 单元 Yield(1.6.1 翻转:9 → 2);
        //    夜间面 4 单元 + 乳汁 3 单元恢复 Own;勘察反界 + R7 恢复单元仍 Own。
        IReadOnlyDictionary<string, FamilyState> remake = Fam(new[] { FamilyMods.Remake }, null);
        foreach (string id in RemakeOnly)
            Check.Eq(RouteDecision.Yield, Router.Decide(id, remake), "remake present: " + id + " Yield");
        string[] remakeStillOwn =
        {
            "main.price_semen", "main.price_tentacle_box", "main.add_body_exp",
            "main.tame_text", "main.manual_render", "main.night_report",
            "main.create_maiden",
            // Task 9:A 组三新单元恒 Own(ComputeInteractMax/ExpNeedForLevel/
            // CreateMaidenOfFaction 处女段,家族 mod 零触碰)
            "main.interact_threshold", "main.exp_formula", "main.initial_virgin",
            // Task 7 审计反向钉住:semen_effects 只让 Brood(remake 不碰
            // MakeSemenFeature/MakeBuyBonusFeature/TryApplySemen 的写入语义——
            // SetupLiquidTargeting 仅包 targeting 回调且保留 previous)。
            "main.semen_effects",
            // 1.6.1 恢复(夜间面 4 单元):ShadowTrade transpiler 只插入不删除,
            // 原版消费点复活,本 mod 注入点对改写后 IL 全部可命中(IL 推演表
            // 17 无扰动 + 4 重锚)——remake 在场照常装配。
            "main.nightly_transpile", "main.produce_daughter",
            "main.alarm_nightly_once", "main.trap_nightly_once",
            // 1.6.1 恢复(乳汁 3 单元):remake 夜间乳汁不再覆写 unitValue,
            // owner 组缩为 [Brood]——仅 remake 在场时 Own。
            "main.nightly_milk_value", "main.price_milk", "main.price_milk_credit",
            // R7 方法级精审恢复的 8 单元:remake patch 清单全文检索零触碰
            // 创建/上架链(CompatOriginal 只改夜间行为/准入/掉落限流);
            // pools.trap_tactic 的数组活引用在 transpile 保留区照读(捕捉掷骰
            // 消费方健在,数组写有效)。
            "main.create_alarm", "main.create_tentacle_alarm", "main.alarm_try_sell_npc",
            "main.create_manhole", "main.create_trap",
            "main.trap_sell_scav", "main.trap_sell_thief",
            "pools.trap_tactic",
            // R7 等价恢复:getter 接管单元 remake 在场恰是工作前提(OnLootRolled
            // 让位但概率路径保留)。
            "main.loot_maiden_chance",
            // 0.6.0 R4:remake.* 差分接管单元无让位规则——remake 在场恰是它们的
            // 工作前提(接管 remake 自家实现;让位表管的是 main.* 对主 mod 的 hook)。
            // ST4:semen_formula/alarm_activity 已注销,余 milk_items/milk_production/
            // ovulation_price 三单元;ST5 +疲劳核心两单元(ValueMul/NightlyGain/
            // RecoverExhausted 接管 remake 自家疲劳实现)
            "remake.milk_items", "remake.milk_production",
            "remake.ovulation_price",
            "remake.fatigue_value_mul", "remake.fatigue_nightly_recover",
        };
        foreach (string id in remakeStillOwn)
            Check.Eq(RouteDecision.Own, Router.Decide(id, remake), "remake present: " + id + " still Own");
        // 缺席方向:remake 不在 → R1 单元回落 Own(1.6.1 后四单元任何时候都 Own)。
        IReadOnlyDictionary<string, FamilyState> lonely = Fam(new[] { FamilyMods.Crisp }, null);
        Check.Eq(RouteDecision.Own, Router.Decide("main.nightly_transpile", lonely), "remake absent: nightly_transpile Own");
        Check.Eq(RouteDecision.Own, Router.Decide("main.nightly_milk_value", lonely), "remake absent: nightly_milk_value Own");
        Check.Eq(RouteDecision.Own, Router.Decide("main.trap_nightly_once", lonely), "remake absent: trap_nightly_once Own");
        Check.Eq(RouteDecision.Own, Router.Decide("main.price_ovulation_shot", lonely), "remake absent: ovulation_shot Own");

        // ③ 评审补充假键所在单元必须进 Yield 集(单列钉住)。
        Check.Eq(RouteDecision.Yield, Router.Decide("main.price_ovulation_shot", remake),
            "review key price_ovulation_shot yields to remake");
        IReadOnlyDictionary<string, FamilyState> brood = Fam(new[] { FamilyMods.Brood }, null);
        Check.Eq(RouteDecision.Yield, Router.Decide("main.nightly_milk_value", brood),
            "review key milk_value_* unit yields to brood");
        // 0.7.0 拆分反向钉住:数值面不随 Brood 让位(1.6.1 后数值面更无任何
        // owner——恒 Own;乳汁面独立让 Brood)。
        Check.Eq(RouteDecision.Own, Router.Decide("main.nightly_transpile", brood),
            "数值面 transpile 恒 Own(1.6.1 后无 owner;乳汁面独立让 Brood)");

        // ④ R2 双方向:Brood 在场 → 4 单元 Yield(1.6.1:milk 三单元移入 R2 +
        //    semen_effects;T9:pools.maiden 已删);缺席 → Own。
        foreach (string id in BroodOnly)
            Check.Eq(RouteDecision.Yield, Router.Decide(id, brood), "brood present: " + id + " Yield");
        string[] broodStillOwn = { "main.price_ovulation_shot", "main.create_maiden", "pools.trap_tactic",
            // R9 拆分:jobs/fbb 两单元均无 Brood 让位规则(T9 后 Brood 让位面
            // 仅剩 nightly_milk_value/price 两键 semen_effects——EXP_NEED 已删)
            "pools.maiden_jobs", "pools.maiden_fbb" };
        foreach (string id in broodStillOwn)
            Check.Eq(RouteDecision.Own, Router.Decide(id, brood), "brood present: " + id + " still Own");
        // Task 7 新增规则的双方向:semen_effects 对 Brood 让位、对 remake/其余在场不让。
        Check.Eq(RouteDecision.Own, Router.Decide("main.semen_effects", remake),
            "brood absent (remake only): semen_effects Own");
        Check.Eq(RouteDecision.Own, Router.Decide("main.semen_effects", lonely),
            "brood absent (crisp only): semen_effects Own");

        // ⑤ R3(1.6.1 后撤销):price_milk/milk_credit 曾为 remake/Brood 并集,
        //    remake 侧撤让后归 R2 独有——remake 单独在场 Own,Brood 单独在场 Yield。
        Check.Eq(RouteDecision.Own, Router.Decide("main.price_milk", remake),
            "1.6.1: price_milk 对 remake 不再让(TickTranspiler 只插入不删除)");
        Check.Eq(RouteDecision.Own, Router.Decide("main.price_milk_credit", remake),
            "1.6.1: price_milk_credit 对 remake 不再让");
        Check.Eq(RouteDecision.Yield, Router.Decide("main.price_milk", brood), "R2: price_milk yields to brood alone");
        Check.Eq(RouteDecision.Yield, Router.Decide("main.price_milk_credit", brood), "R2: price_milk_credit yields to brood alone");

        // ⑥ R4 双方向:IPH 在场 → 两 maiden 单元 Yield + 注明;池写照常(0.5.1 先例)。
        IReadOnlyDictionary<string, FamilyState> iph = Fam(new[] { FamilyMods.Iph }, null);
        foreach (string id in IphOnly)
        {
            Check.Eq(RouteDecision.Yield, Router.Decide(id, iph), "iph present: " + id + " Yield");
            Check.Eq(true, !string.IsNullOrEmpty(Router.YieldNote(id)), "iph present: " + id + " note non-empty");
        }
        // T9:pools.maiden 已删(1.9.2 EXP_NEED 字段消失,经验公式化)——
        // 让位表零该行,Brood 在场对成长公式键无影响
        Check.Eq(RouteDecision.Own, Router.Decide("main.exp_formula", brood),
            "T9: exp_formula 不让 Brood(EXP_NEED 静态写已随字段消失)");
        // R9:fbb 三池让位、jobs 恒 Own(IPH 工厂 transpiler 零触碰职业抽取)
        Check.Eq(RouteDecision.Yield, Router.Decide("pools.maiden_fbb", iph),
            "iph present: pools.maiden_fbb Yield(三池抽取被其替换)");
        Check.Eq(RouteDecision.Own, Router.Decide("pools.maiden_jobs", iph),
            "iph present: pools.maiden_jobs Own(JOBS 零替换)");
        Check.Eq(RouteDecision.Own, Router.Decide("pools.maiden_fbb", remake),
            "iph absent: pools.maiden_fbb Own");
        Check.Eq(RouteDecision.Own, Router.Decide("main.create_maiden", remake), "iph absent: create_maiden Own");
        // T8:拆段后的 maiden 数值两单元与 IPH 共存(Own)——iph_1218 工厂
        // transpiler 只挂 CreateMaidenOfFaction(OriginalCompat.cs:62/:110),
        // RollAttr/FactionBase 是独立 public static 方法、其改写全集零触碰
        // (remake_1524/brood_0331 同样零命中)→ 让位表零新行,R10 postfix
        // 共存先例同构(transpiler 改 RollAttr 的 ldc,IPH 替换的是
        // CreateMaidenOfFaction 内的池抽取 call,两者正交)
        Check.Eq(RouteDecision.Own, Router.Decide("main.maiden_roll", iph),
            "T8: iph present → maiden_roll Own(共存,RollAttr 零触碰)");
        Check.Eq(RouteDecision.Own, Router.Decide("main.maiden_faction_base", iph),
            "T8: iph present → maiden_faction_base Own(共存)");
        Check.Eq((string)null, Router.YieldNote("main.maiden_roll"),
            "T8: maiden_roll 无让位注明");
        Check.Eq((string)null, Router.YieldNote("main.maiden_faction_base"),
            "T8: maiden_faction_base 无让位注明");

        // ⑥' R10 三态钉住:main.maiden_attrs_iph(MainHook,TargetMod = IPH,
        //     不在 YieldOwners——让位原则的例外形态,postfix 与 IPH 工厂
        //     transpiler 共存):在场且版本匹配 → Own(开关开即装);版本漂移 →
        //     VersionBlocked 自动失效(用户裁定,防 IPH 未来接管属性段时冲突);
        //     缺席 → Own(分区不 Define 开关恒 false = Absent 零 hook)。
        Check.Eq(RouteDecision.Own, Router.Decide("main.maiden_attrs_iph", iph),
            "R10: iph present+matched → attrs postfix Own(共存装配)");
        IReadOnlyDictionary<string, FamilyState> iphDrift = Fam(
            new[] { FamilyMods.MainMod }, new[] { FamilyMods.Iph });
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("main.maiden_attrs_iph", iphDrift),
            "R10: iph version drift → attrs postfix VersionBlocked(自动失效)");
        Check.Eq(RouteDecision.Own, Router.Decide("main.maiden_attrs_iph", remake),
            "R10: iph absent → attrs postfix Own(开关恒 false = Absent 零 hook)");
        Check.Eq((string)null, Router.YieldNote("main.maiden_attrs_iph"),
            "R10: attrs postfix 无让位注明(共存形态,不 Yield)");

        // ⑦ R5 版本压制:TargetMod 在场但版本不符 → VersionBlocked 压过一切。
        IReadOnlyDictionary<string, FamilyState> mainWrong = Fam(new[] { FamilyMods.Remake, FamilyMods.Iph },
            new[] { FamilyMods.MainMod });
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("main.on_loot_rolled", mainWrong),
            "version压制压过 Yield(on_loot_rolled;remake 在场仍让位)");
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("main.nightly_milk_value", mainWrong),
            "version压制(nightly_milk_value;1.6.1 后 remake 不让、Brood 缺席,普通压制面)");
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("main.create_maiden", mainWrong),
            "version压制压过 Yield(create_maiden)");
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("main.level_cap", mainWrong),
            "version压制 Own 单元(level_cap)");
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("pools.guest_display", mainWrong),
            "version压制 DataWrite 单元(guest_display)");
        Check.Eq(RouteDecision.Own, Router.Decide("game.pickgendered", mainWrong),
            "game.* 不受主 mod 版本压制(pickgendered)");
        Check.Eq(RouteDecision.Own, Router.Decide("game.sprite_name", mainWrong),
            "game.* 不受主 mod 版本压制(sprite_name)");
        // 主 mod 缺席 → 不压制,回落正常裁决(Own/Yield;缺席处理归装配层)。
        // 1.6.1:nightly 组对 remake 撤让——mainAbsent(仅 remake 在场)下
        // nightly_transpile/nightly_milk_value 均 Own,让位仅剩 on_loot_rolled。
        IReadOnlyDictionary<string, FamilyState> mainAbsent = Fam(new[] { FamilyMods.Remake }, null);
        Check.Eq(RouteDecision.Own, Router.Decide("main.nightly_transpile", mainAbsent),
            "main mod absent: no suppression;1.6.1 后 remake 撤让 → Own");
        Check.Eq(RouteDecision.Own, Router.Decide("main.nightly_milk_value", mainAbsent),
            "main mod absent: no suppression;Brood 缺席 → Own(remake 已撤让)");
        Check.Eq(RouteDecision.Yield, Router.Decide("main.on_loot_rolled", mainAbsent),
            "main mod absent: no suppression, on_loot_rolled still Yield(remake)");
        // owner 版本漂移:Present 即让位(对方已接管语义,不构成 Own 的理由)。
        IReadOnlyDictionary<string, FamilyState> remakeWrong = Fam(new[] { FamilyMods.MainMod },
            new[] { FamilyMods.Remake });
        Check.Eq(RouteDecision.Yield, Router.Decide("main.on_loot_rolled", remakeWrong),
            "remake version drift: still Yield");
        // 1.6.1 恢复单元反向钉住:撤让后无让位规则,版本漂移不改变 Own。
        Check.Eq(RouteDecision.Own, Router.Decide("main.trap_nightly_once", remakeWrong),
            "1.6.1: remake version drift → trap_nightly_once Own(撤让)");
        // R4 方向钉住:remake.* 接管单元的 TargetMod = remake,remake 版本漂移 →
        // VersionBlocked(压制自身装配;不构成对 main.* 让位的干扰)
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("remake.milk_items", remakeWrong),
            "remake version drift: remake.* units blocked");
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("remake.ovulation_price", remakeWrong),
            "remake version drift: remake.ovulation_price blocked(ST4 后 trio 之一)");
        // R12 方向钉住:brood.perfume_mod(SubModHook,TargetMod = Brood)——Brood
        // 版本漂移 → VersionBlocked(bound 0.3.31 钉住 WeakenBasePerfume 签名);
        // Brood 在场且命中 → Own(postfix 等价恢复,主面板 semen_effects 让位不变)
        IReadOnlyDictionary<string, FamilyState> broodWrong = Fam(
            new[] { FamilyMods.MainMod }, new[] { FamilyMods.Brood });
        Check.Eq(RouteDecision.VersionBlocked, Router.Decide("brood.perfume_mod", broodWrong),
            "R12: brood version drift → perfume_mod VersionBlocked");
        Check.Eq(RouteDecision.Own, Router.Decide("brood.perfume_mod", brood),
            "R12: brood present+matched → perfume_mod Own(让位不变:semen_effects 仍 Yield)");
        Check.Eq(RouteDecision.Yield, Router.Decide("main.semen_effects", brood),
            "R12: 主面板 semen_effects 对 Brood 让位行为零改动");
        IReadOnlyDictionary<string, FamilyState> iphWrong = Fam(new[] { FamilyMods.MainMod },
            new[] { FamilyMods.Iph });
        Check.Eq(RouteDecision.Yield, Router.Decide("main.create_maiden", iphWrong),
            "iph version drift: still Yield");

        // ⑧ R6 YieldNote 文案(0.06 轮 F4 修订:「启用<owner>,该配置不生效」):
        //    单 owner 直出;非让位单元 null(1.6.1 恢复的 nightly/produce_daughter/
        //    alarm/trap 四单元与 R7 恢复的 create_alarm/trap_tactic 等均无注明;
        //    milk 三单元改 Brood 独有后不再拼双 owner)。
        Check.Eq("활성화된 암거래 때문에 이 설정은 적용되지 않습니다", Router.YieldNote("main.on_loot_rolled"), "note remake-only");
        Check.Eq("활성화된 암거래 때문에 이 설정은 적용되지 않습니다", Router.YieldNote("main.price_ovulation_shot"), "note remake-only (ovulation, R7 保留)");
        Check.Eq("활성화된 둥지 확장 때문에 이 설정은 적용되지 않습니다", Router.YieldNote("main.semen_effects"), "note brood-only (semen_effects, Task 7)");
        Check.Eq("활성화된 아이템 고해상도 초상화 관리자 때문에 이 설정은 적용되지 않습니다", Router.YieldNote("main.create_maiden"), "note iph-only");
        Check.Eq((string)null, Router.YieldNote("main.nightly_transpile"), "note null for 1.6.1 恢复单元(nightly_transpile)");
        Check.Eq((string)null, Router.YieldNote("main.produce_daughter"), "note null for 1.6.1 恢复单元(produce_daughter)");
        Check.Eq((string)null, Router.YieldNote("main.alarm_nightly_once"), "note null for 1.6.1 恢复单元(alarm_nightly_once)");
        Check.Eq((string)null, Router.YieldNote("main.trap_nightly_once"), "note null for 1.6.1 恢复单元(trap_nightly_once)");
        Check.Eq("활성화된 둥지 확장 때문에 이 설정은 적용되지 않습니다", Router.YieldNote("main.nightly_milk_value"), "note brood-only(1.6.1: milk 三单元改 Brood 独有)");
        Check.Eq("활성화된 둥지 확장 때문에 이 설정은 적용되지 않습니다", Router.YieldNote("main.price_milk"), "note brood-only price_milk(1.6.1)");
        Check.Eq((string)null, Router.YieldNote("main.create_alarm"), "note null for R7-restored unit (create_alarm)");
        Check.Eq((string)null, Router.YieldNote("pools.trap_tactic"), "note null for R7-restored DataWrite (trap_tactic)");
        Check.Eq((string)null, Router.YieldNote("main.loot_maiden_chance"), "note null for R7 getter unit");
        Check.Eq((string)null, Router.YieldNote("main.level_cap"), "note null for Own unit");
        Check.Eq((string)null, Router.YieldNote("game.pickgendered"), "note null for game unit");
        Check.Eq((string)null, Router.YieldNote("pools.guest_display"), "note null for DataWrite unit");
        Check.Eq((string)null, Router.YieldNote("main.nope"), "note null for unknown unit");

        // ⑨ DecideAll 全在场:Yield 9 / Own 102(111 单元 = 54 个 lewd_meme 目标
        //   单元[45 Own + 9 Yield = R1 2(on_loot_rolled/price_ovulation_shot)
        //   + R2 4(milk 三 + semen_effects)+ R4 3]+ 2 game.* + main.maiden_
        //   attrs_iph(R10,TargetMod=IPH 在场匹配 → Own)+ 36 WriteThrough
        //   [ST4: remake 8−3+4 + Brood 12 + IPH 16] + 18 SubModHook[brothel 9 +
        //   tearl 3 + remake 5(ST4: -semen_formula -alarm_activity;ST5: +疲劳
        //   核心两单元)+ brood 1(R12)];1.6.1 翻转:13 Yield → 9——R1 9→2 +
        //   milk 三单元改 Brood 独有[R1∩R2 行消失];ST4:110−2+1 = 109,
        //   ST5:+2 = 111)
        //   /VersionBlocked 0,且与 Decide 逐单元一致。
        IReadOnlyDictionary<string, FamilyState> all = Fam(
            new[] { FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Brood, FamilyMods.Iph }, null);
        IReadOnlyDictionary<string, RouteDecision> table = Router.DecideAll(all);
        int yields = 0, owns = 0, blocked = 0;
        foreach (RouteDecision d in table.Values)
        {
            if (d == RouteDecision.Yield) yields++;
            else if (d == RouteDecision.Own) owns++;
            else blocked++;
        }
        Check.Eq(9, yields, "all present: 9 Yield units(1.6.1: R1 9→2 + milk 3 改 Brood,13 → 9)");
        Check.Eq(103, owns, "all present: 103 Own units (112 - 9 Yield;0.7.1: +brood.tag_picker)");
        Check.Eq(0, blocked, "all present: 0 VersionBlocked");
        foreach (KeyValuePair<string, RouteDecision> kv in table)
            Check.Eq(Router.Decide(kv.Key, all), kv.Value, "DecideAll agrees with Decide: " + kv.Key);

        // ⑬ M5a/M5b/R4/R12:SubModHook 单元(brothel.*/tearl.*/remake.*/brood.*)的
        //    版本压制链路——TargetMod 自动经 ①(Present && !VersionOk → VersionBlocked)
        //    生效,无特判;无让位规则(YieldOwners 不含四族,remake.*/brood.* 接管的
        //    是对方自家差分实现——主 mod 侧让位单元是另一批 main.* 单元),
        //    缺席不压制(可装性归装配层)。R12 起按各单元自身 TargetMod 构造
        //    在场两态(brood.* 目标 = Brood,不再随 brothel/tearl/remake 三件套)。
        IReadOnlyDictionary<string, FamilyState> brothelAbsent = Fam(null, null);
        int brothelUnits = 0;
        foreach (HookUnitDef u in HookCatalog.Units)
        {
            if (u.Kind != CarrierKind.SubModHook) continue;
            brothelUnits++;
            IReadOnlyDictionary<string, FamilyState> wrong = Fam(
                new[] { FamilyMods.MainMod }, new[] { u.TargetMod });
            IReadOnlyDictionary<string, FamilyState> ok = Fam(
                new[] { FamilyMods.MainMod, u.TargetMod }, null);
            Check.Eq(RouteDecision.VersionBlocked, Router.Decide(u.Id, wrong),
                "⑬ 版本不符: " + u.Id + " VersionBlocked");
            Check.Eq(RouteDecision.Own, Router.Decide(u.Id, ok),
                "⑬ 版本命中: " + u.Id + " Own");
            Check.Eq(RouteDecision.Own, Router.Decide(u.Id, brothelAbsent),
                "⑬ 缺席不压制: " + u.Id + " Own");
            Check.Eq((string)null, Router.YieldNote(u.Id),
                "⑬ 接管单元无让位注明: " + u.Id);
        }
        Check.Eq(19, brothelUnits, "⑬ SubModHook 单元恰 19(brothel 9 + tearl 3 + remake 5[ST5: +疲劳两单元] + brood 2[R12 信息素 + 0.7.1 拾取器])");

        // ⑩ 防御:未知 unitId → ArgumentException;family 缺键(空表)不 NRE、按缺席处理。
        Check.Throws<ArgumentException>(delegate
        {
            Router.Decide("main.nope", Fam(null, null));
        }, "Decide rejects unknown unit id");
        IReadOnlyDictionary<string, FamilyState> empty = new Dictionary<string, FamilyState>();
        Check.Eq(RouteDecision.Own, Router.Decide("main.nightly_transpile", empty), "empty family map: treated as absent");
        Check.Eq(RouteDecision.Own, Router.Decide("main.level_cap", empty), "empty family map: no NRE for target");

        // ⑪ 表完整性防御(Task 7 授权新增):让位表/YieldNote 消费的 unitId 必须
        //    ⊆ HookCatalog(YieldNote 经 YieldOwners 查表,同一键集)——抓 typo
        //    (Task 6 遗留:表键拼错只会静默永不命中,让位失效无报错)。
        var catalogIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (HookUnitDef u in HookCatalog.Units) catalogIds.Add(u.Id);
        Check.Eq(true, Router.YieldOwners.Count > 0, "yield table non-empty");
        foreach (KeyValuePair<string, string[]> kv in Router.YieldOwners)
        {
            Check.Eq(true, catalogIds.Contains(kv.Key), "yield id in catalog: " + kv.Key);
            Check.Eq(true, kv.Value != null && kv.Value.Length > 0, "yield owners non-empty: " + kv.Key);
            // owner 显示名可解析(OwnerNames 缺项会让 YieldNote 抛 KeyNotFound)
            Check.Eq(true, Router.YieldNote(kv.Key) != null, "yield note resolvable: " + kv.Key);
        }
        // 让位单元数 = 分组全集(R1 独有 2 + R2 独有 4[milk 三 + semen_effects]
        // + R4 3 = 9,1.6.1 翻转:13 → 9——删 R1 夜间面 4 行 + milk 三单元自
        // R1∩R2 移入 R2),与 ⑨ 的 DecideAll 计数互为印证。
        Check.Eq(RemakeOnly.Length + BroodOnly.Length + IphOnly.Length,
            Router.YieldOwners.Count, "yield table size equals group sums");

        // ⑭ 0.7.0 Task 4:nightly 组两 transpiler 单元的让位矩阵(remake×brood×iph
        //    在场组合全枚举 8 态 × 两单元;iph 与 nightly 无关——R4 只辖 maiden
        //    两单元与 fbb 池,任何组合不改变两单元裁决)。1.6.1 合成语义:
        //    nightly_transpile 无任何 owner(remake 撤让)恒 Own;nightly_milk_value
        //    仅 Brood 在场让(其夜间重算 unitValue 同源覆盖,让位避免双写)。
        string[] matrixMods = { FamilyMods.Remake, FamilyMods.Brood, FamilyMods.Iph };
        for (int mask = 0; mask < 8; mask++)
        {
            var present = new List<string> { FamilyMods.MainMod };
            for (int bit = 0; bit < 3; bit++)
                if ((mask & (1 << bit)) != 0) present.Add(matrixMods[bit]);
            IReadOnlyDictionary<string, FamilyState> fam = Fam(present.ToArray(), null);
            bool hasBrood = (mask & 2) != 0;
            string label = string.Join("+", present.ToArray());
            Check.Eq(RouteDecision.Own,
                Router.Decide("main.nightly_transpile", fam),
                "⑭ 矩阵[" + label + "]: nightly_transpile 恒 Own(1.6.1 撤让)");
            Check.Eq(hasBrood ? RouteDecision.Yield : RouteDecision.Own,
                Router.Decide("main.nightly_milk_value", fam),
                "⑭ 矩阵[" + label + "]: nightly_milk_value 仅 Brood 让位");
        }

        // ⑫ R-F 写透豁免:WriteThrough 单元恒 Own——不 Yield(全在场仍 Own)、
        //    不 VersionBlocked(目标在场且版本不匹配仍 Own;版本治理 = 面板条件
        //    Define 的在场性 + 载体的键存在性守卫,spec §5.7 明文豁免)。
        //    ST4 样本键:auto_barrel_semen(桶装新键)/ fatigue_enabled(存续键)。
        Check.Eq(RouteDecision.Own, Router.Decide("wt.remake.auto_barrel_semen", all),
            "R-F: 全在场写透单元仍 Own(remake 桶装新键)");
        Check.Eq(RouteDecision.Own, Router.Decide("wt.brood.eggBaseChance", all),
            "R-F: 全在场写透单元仍 Own(brood)");
        IReadOnlyDictionary<string, FamilyState> mirrorWrong = Fam(
            new[] { FamilyMods.MainMod }, new[] { FamilyMods.Remake, FamilyMods.Brood });
        Check.Eq(RouteDecision.Own, Router.Decide("wt.remake.fatigue_enabled", mirrorWrong),
            "R-F: 目标在场版本不匹配,写透单元仍 Own(remake)");
        Check.Eq(RouteDecision.Own, Router.Decide("wt.brood.disableRampageEnding", mirrorWrong),
            "R-F: 目标在场版本不匹配,写透单元仍 Own(brood)");
        Check.Eq(RouteDecision.Own, Router.Decide("wt.remake.fatigue_scale", remake),
            "R-F: remake 在场版本命中,写透单元仍 Own");
        Check.Eq((string)null, Router.YieldNote("wt.remake.fatigue_enabled"),
            "R-F: 写透单元无让位注明(YieldNote null)");

        // ⑮ T10:全量让位矩阵——主 mod 恒在(版本恒命中)× 16 在场组合 × 全 111 单元,
        //     断言实质非恒真:期望让位集由测试自持的三组清单(RemakeOnly/BroodOnly/
        //     IphOnly;1.6.1 翻转后 R1∩R2 组撤销)独立推导,与 Router.YieldOwners
        //     无共享状态;组合面覆盖 remake×Brood×IPH 分叉(1-8)+ 无让位规则四族
        //     (brothel/tearl/legacy/crisp,9-16)+ 无关族与让位族叠加(13-16)。
        //     零冲突钉住:非让位族加入不得改变任何单元裁决(13/14/15/16 与其让位
        //     族投影组合 2/3/4/8 的裁决表全等)。
        string[][] combos =
        {
            new[] { FamilyMods.MainMod },                                              // 1 基线
            new[] { FamilyMods.MainMod, FamilyMods.Remake },                           // 2
            new[] { FamilyMods.MainMod, FamilyMods.Brood },                            // 3
            new[] { FamilyMods.MainMod, FamilyMods.Iph },                              // 4
            new[] { FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Brood },         // 5
            new[] { FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Iph },           // 6
            new[] { FamilyMods.MainMod, FamilyMods.Brood, FamilyMods.Iph },            // 7
            new[] { FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Brood, FamilyMods.Iph }, // 8
            new[] { FamilyMods.MainMod, FamilyMods.Brothel },                          // 9
            new[] { FamilyMods.MainMod, FamilyMods.Tearl },                            // 10
            new[] { FamilyMods.MainMod, FamilyMods.Legacy },                           // 11
            new[] { FamilyMods.MainMod, FamilyMods.Crisp },                            // 12
            new[] { FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Brothel, FamilyMods.Tearl }, // 13
            new[] { FamilyMods.MainMod, FamilyMods.Brood, FamilyMods.Legacy, FamilyMods.Crisp },   // 14
            new[] { FamilyMods.MainMod, FamilyMods.Iph, FamilyMods.Brothel, FamilyMods.Legacy },   // 15
            new[] { FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Brood, FamilyMods.Iph,
                    FamilyMods.Brothel, FamilyMods.Tearl, FamilyMods.Legacy, FamilyMods.Crisp },   // 16
        };
        // 投影对照:组合 13/14/15/16 去掉无关族后 = 组合 2/3/4/8(裁决表必须全等)
        int[] projectTo = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 2, 3, 7 };
        var catalogSet = new HashSet<string>(StringComparer.Ordinal);
        foreach (HookUnitDef u in HookCatalog.Units) catalogSet.Add(u.Id);
        IReadOnlyDictionary<string, RouteDecision> snapshot8 = null;
        var snapshots = new IReadOnlyDictionary<string, RouteDecision>[16];
        for (int ci = 0; ci < combos.Length; ci++)
        {
            string[] present = combos[ci];
            IReadOnlyDictionary<string, FamilyState> fam = Fam(present, null);
            bool hasRemake = Array.IndexOf(present, FamilyMods.Remake) >= 0;
            bool hasBrood = Array.IndexOf(present, FamilyMods.Brood) >= 0;
            bool hasIph = Array.IndexOf(present, FamilyMods.Iph) >= 0;
            // 期望让位集(测试自持清单独立推导,owner 在场才让)
            var expectedYield = new HashSet<string>(StringComparer.Ordinal);
            if (hasRemake) expectedYield.UnionWith(RemakeOnly);
            if (hasBrood) expectedYield.UnionWith(BroodOnly);
            if (hasIph) expectedYield.UnionWith(IphOnly);
            Check.Eq(true, expectedYield.IsSubsetOf(catalogSet),
                "⑮[" + ci + "] 期望让位集 ⊆ 目录(清单自洽)");
            string label = string.Join("+", present);
            IReadOnlyDictionary<string, RouteDecision> t = Router.DecideAll(fam);
            snapshots[ci] = t;
            if (ci == 7) snapshot8 = t;
            Check.Eq(112, t.Count, "⑮[" + label + "] DecideAll 覆盖 112 单元");
            foreach (HookUnitDef u in HookCatalog.Units)
            {
                RouteDecision expect = expectedYield.Contains(u.Id)
                    ? RouteDecision.Yield : RouteDecision.Own;
                Check.Eq(expect, t[u.Id], "⑮[" + label + "]: " + u.Id);
            }
            // 让位计数与期望集同大小(防期望集漏项被 Own 淹没)
            int yieldCount = 0;
            foreach (RouteDecision d in t.Values) if (d == RouteDecision.Yield) yieldCount++;
            Check.Eq(expectedYield.Count, yieldCount, "⑮[" + label + "] Yield 计数 = 期望集大小");
            // game.*/WriteThrough 恒 Own 抽查(R-F 与 TargetMod=null 的在场无关性)
            Check.Eq(RouteDecision.Own, t["game.pickgendered"], "⑮[" + label + "] game.* 恒 Own");
            Check.Eq(RouteDecision.Own, t["wt.iph.resolution"], "⑮[" + label + "] WriteThrough 恒 Own");
            // 投影对照:无关族叠加零效应(裁决表全等)
            if (projectTo[ci] >= 0)
            {
                bool identical = snapshots[ci].Count == snapshots[projectTo[ci]].Count;
                if (identical)
                    foreach (KeyValuePair<string, RouteDecision> kv in snapshots[ci])
                        if (snapshots[projectTo[ci]][kv.Key] != kv.Value) { identical = false; break; }
                Check.Eq(true, identical,
                    "⑮[" + label + "] 无关族叠加零冲突(与投影组合裁决表全等)");
            }
        }
        Check.Eq(true, snapshot8 != null, "⑮ 投影基线(组合 8)在册");
    }

    // —— 构造辅助:全 8 家族键的快照。ok = Present 且版本命中;wrong = Present 但
    //    版本不符(9.9.9);其余缺席(Version=null / VersionOk=false)。——
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
