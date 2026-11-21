using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 路由裁决结果(spec §5.2/§5.4):Own = 本 mod 装配该单元;Yield = 语义所有者
    /// 是在场子 mod,永不装配,面板注明归属;VersionBlocked = 单元的 TargetMod 在场
    /// 但版本不命中绑定表(§4.6 压制,干净态)。TargetMod 缺席不产生压制(是否可装
    /// 由装配层 resolver 裁定,Router 只裁决让位与压制)。
    /// </summary>
    public enum RouteDecision
    {
        Own,
        Yield,
        VersionBlocked,
    }

    /// <summary>
    /// 让位裁决表(M3b,纯逻辑:System.* only + Config/ 的 HookCatalog/FamilyMods)。
    /// 输入 = 家族在场快照(Task 5 FamilyState),输出 = 逐装配单元的 Own/Yield/
    /// VersionBlocked。规则 = spec §5.4 六条,单元映射已对七份反编译勘察固化:
    /// <list type="bullet">
    ///  <item>R1 remake 在场(2 单元 Yield;ST 1.6.1[2026-11-17]让位翻转:
    ///    9 → 2——ShadowTrade 1.6.1[mod ID 仍是 lewd_meme_remake]撤掉 1.5.x
    ///    的夜间活动接管,改为对主 mod 三个 Nightly 方法的插入式共存
    ///    transpiler(只插入不删除,原版 NightlyTick/ProduceDaughter/
    ///    NightlyTickOnce 消费点复活),推演依据勘察摘要「1.6.1 让位翻转·
    ///    IL 共存推演」节[21 注入点 = 17 无扰动 + 4 重锚,已进 TranspilePlan]):
    ///    main.on_loot_rolled(OnLootRolled 被 transpiler NOP 掉 0.5s 限流,
    ///    复刻体自带节流 = 反对其「无限流」设计;概率键 scav_maiden_base 经
    ///    main.loot_maiden_chance 等价恢复[R7])+ main.price_ovulation_shot
    ///    (remake postfix 强制单价 100,CompatOriginal.cs:366;remake.
    ///    ovulation_price SubModHook 等价恢复[R7])。1.6.1 恢复 Own 的 7 单元
    ///    (勿回退):nightly 面 4 = main.nightly_transpile / produce_daughter /
    ///    alarm_nightly_once / trap_nightly_once(0.6/0.7 时代「transpiler 删
    ///    消费点区段/prefix 跳过被改写方法体」的让位前提随插入式共存消失)
    ///    + 乳汁 3 = main.nightly_milk_value / price_milk / milk_credit(remake
    ///    夜间乳汁不再覆写 unitValue,竞争写者只剩 Brood → 改 R2 独有)。
    ///    勘察反界(不让位):main.price_semen(夜间 TryProduce 经目录仍调主
    ///    mod CreateSemen 且 remake 不覆价,本 mod postfix 继续有效)/
    ///    main.price_tentacle_box(remake 零接触)/ main.add_body_exp
    ///    (ExpMultiplier ∉ NightlyTick 22 键组,DebugAddExp 路径残留有效)/
    ///    main.tame_text / main.manual_render(键为混合集)/ main.night_report
    ///    (remake ShowReport 仅改 2 个标签,增强型)/ **R7 恢复 8 单元**
    ///    (create_alarm / create_tentacle_alarm / alarm_try_sell_npc /
    ///    create_manhole / create_trap / trap_sell_scav / trap_sell_thief/
    ///    pools.trap_tactic)。</item>
    ///  <item>R2 Brood 在场(4 单元 Yield;T9:pools.maiden 删除;1.6.1:milk
    ///    三单元自 R1∩R2 移入,remake 撤让后 Brood 成唯一 owner):
    ///    main.nightly_milk_value(nightly.milk_value_base/per_level 所在
    ///    单元,0.7.0 T4 拆分后乳汁面自成单元——数值面 transpile 不随 Brood
    ///    让位;Brood 夜间重算 unitValue 同源覆盖)+ main.price_milk/
    ///    milk_credit(Brood 同式重算)+
    ///    main.semen_effects(Task 7 审计新增:三键全接管——client.semen_price_mod /
    ///    semen_bonus_mod 的 ItemFeature 在顾客进场时被 Fragrance.WeakenBasePerfume
    ///    强制 ±10(Fragrance.cs:39/43);client.semen_rep_mult 的激活链被
    ///    AttachSemen 包装、倍率强制 pre×1.1(BaseLink.cs:74 Wrap + 452-481),
    ///    baseline×cfg 增幅方向必被覆盖)。</item>
    ///  <item>R3(1.6.1 撤销)曾为「remake 或 Brood 在场 → main.price_milk/
    ///    milk_credit 并集」;remake 侧撤让后该两单元归 R2 独有,并集规则
    ///    不复存在(行删除,0.5.0 注记归档)。</item>
    ///  <item>R4 IPH 在场(3 单元 Yield):main.create_maiden / main.create_maiden_of_faction
    ///    (0.5.1 让位先例:工厂 transpiler 挂钩链全程接管原方法体;portrait_size 键
    ///    恒在此两单元)+ pools.maiden_fbb(0.6.0 R9 拆分修正:阵营/体型/胸部三池
    ///    静态写在 IPH 在场时不再可靠生效——其工厂 transpiler 把 BODIES.Pick 换成
    ///    AdultBodySources.PickBody(iph_1217 OriginalCompat.cs:287,经 BodyCsvRules
    ///    Draw 以主 mod 池为抽签域但相对权重归 body_weights_csv)、BREASTS.Pick 换成
    ///    PickBreastForBody(:325,默认 body_affects_breast_size 开走其体型-乳量
    ///    矩阵,不读主 mod 池)、FACTIONS.Pick 换成 RandomPortraits.PickFaction
    ///    (:294,默认 weighted_capture_factions 开走其自家权重表,仅关闭时回退
    ///    original);num7/num8 校验(:339)不分兼容模式——original_portrait_
    ///    selection 下体型/乳量替换仍强制。0x0c 实测:阵营池与胸部池 CSV 失效、
    ///    体型池仅抽签域收窄生效;玩家需求出口 = IPH 写透分区(R9)的
    ///    body_weights_csv / body_affects_breast_size 键。JOBS 零替换
    ///    (iph_1217 全文无 MaidenTraits.JOBS 引用)→ pools.maiden_jobs 恒 Own。
    ///    经验公式键不在让位面[T9 后与 IPH 无关])。</item>
    ///  <item>R5 优先级:单元 TargetMod(主 mod 家族单元 = lewd_meme)在场且版本不
    ///    命中 → VersionBlocked 压过一切(含 Yield);TargetMod=null 的 game.* 单元
    ///    永不压制。owner(remake/Brood/IPH)侧版本漂移不改变让位——Present 即让位
    ///    (对方已接管语义,版本漂移不构成 Own 的理由)。</item>
    ///  <item>R6 YieldNote(0.6.0 R4 文案修订):"启用&lt;owner 显示名&gt;,该
    ///    配置不生效";多 owner 按固定序(暗流交易、巢穴扩展、物品高清立绘
    ///    管理器)用 / 连接;非让位单元返回 null。
    ///    显示名与面板分区名一致(Task 11 定稿,暂用 spec §5.4 命名)。</item>
    /// </list>
    /// DataWrite 单元语义:无让位规则命中、无版本压制时恒 Own(pools.guest_display
    /// / pools.maiden_jobs / pools.trap_tactic——T9 后 DataWrite 共 4);
    /// pools.maiden_fbb 的 Yield 是 R4 键级规则的显式覆盖(具体规则压过
    /// 「恒 Own」的一般描述)。
    /// WriteThrough 单元恒 Own(R-F 写透豁免:版本治理 = 键存在性守卫,不 Yield、
    /// 不 VersionBlocked)。
    /// </summary>
    public static class Router
    {
        /// <summary>owner mod ID → 面板显示名(与 Task 11 分区名一致;暂按 spec §5.4)。</summary>
        private static readonly IReadOnlyDictionary<string, string> OwnerNames =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { FamilyMods.Remake, "암거래" },
                { FamilyMods.Brood, "둥지 확장" },
                { FamilyMods.Iph, "아이템 고해상도 초상화 관리자" },
            };

        /// <summary>
        /// 让位单元表:unitId → owner mod ID 数组(序即 YieldNote 拼接序:暗流交易、
        /// 巢穴扩展、物品高清立绘管理器)。9 项 = R1 独有 2 + R2 独有 4 + R4 3
        /// (0.6.0 R7 方法级精审:R1 自 16 缩定;0.6.0 R9:pools.maiden_traits
        /// 拆分,R4 增 pools.maiden_fbb,12 → 13;0.7.0 T4:main.nightly_tick
        /// 拆 main.nightly_transpile + main.nightly_milk_value,13 → 14;0.7.0
        /// T9:pools.maiden 删除,14 → 13;ST 1.6.1[2026-11-17]:R1 9 → 2 +
        /// milk 三单元 owner 组 [Remake, Brood] → [Brood],13 → 9——ShadowTrade
        /// 1.6.1 对主 mod Nightly 系改为插入式共存 transpiler(只插入不删除,
        /// 原版消费点复活),夜间面 4 单元撤让、乳汁 3 单元改 Brood 独有,推演
        /// 依据勘察摘要「1.6.1 让位翻转·IL 共存推演」节)。
        /// internal:测试防御用(Task 7 ⑪:键集 ⊆ HookCatalog,抓 typo)。
        /// </summary>
        internal static readonly IReadOnlyDictionary<string, string[]> YieldOwners =
            new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                // ---- R1 remake(2;1.6.1 让位翻转:9 → 2——夜间面 4 单元
                //      [nightly_transpile/produce_daughter/alarm_nightly_once/
                //      trap_nightly_once]随插入式共存撤让恢复 Own;milk 三单元
                //      移入 R2。剩掉落限流与排卵单价两行——remake 对其语义
                //      仍是接管[NOP 限流 / postfix 强制 100]而非共存)----
                { "main.on_loot_rolled", new[] { FamilyMods.Remake } },
                { "main.price_ovulation_shot", new[] { FamilyMods.Remake } },
                // ---- R2 Brood(4;1.6.1:milk 三单元自 R1∩R2 移入——remake 夜间
                //      乳汁不再覆写 unitValue,竞争写者只剩 Brood 的重算;
                //      semen_effects = Task 7 审计;pools.maiden 已随 1.9.2
                //      EXP_NEED 字段删除而撤出[T9])----
                { "main.nightly_milk_value", new[] { FamilyMods.Brood } },
                { "main.price_milk", new[] { FamilyMods.Brood } },
                { "main.price_milk_credit", new[] { FamilyMods.Brood } },
                { "main.semen_effects", new[] { FamilyMods.Brood } },
                // ---- R4 IPH(3;R9 增 pools.maiden_fbb:阵营/体型/胸部三池的抽取
                //      被其工厂 transpiler 替换接管,静态写在默认路径无消费方,
                //      证据与边界见类注释 R4 条)----
                { "main.create_maiden", new[] { FamilyMods.Iph } },
                { "main.create_maiden_of_faction", new[] { FamilyMods.Iph } },
                { "pools.maiden_fbb", new[] { FamilyMods.Iph } },
            };

        /// <summary>unitId → 目录定义的索引(装配期 O(1) 查找;未知 id 抛参数异常)。</summary>
        private static readonly Dictionary<string, HookUnitDef> Defs = BuildDefs();

        private static Dictionary<string, HookUnitDef> BuildDefs()
        {
            var defs = new Dictionary<string, HookUnitDef>(StringComparer.Ordinal);
            foreach (HookUnitDef u in HookCatalog.Units) defs.Add(u.Id, u);
            return defs;
        }

        /// <summary>
        /// 单单元裁决。优先级:⓪ WriteThrough 单元恒 Own(R-F 写透豁免,spec §5.7
        /// 明文:写透不是 hook,版本治理 = 键存在性守卫,不 Yield、不 VersionBlocked;
        /// 在场性由面板条件 Define 把关);① TargetMod 在场且版本不命中 →
        /// VersionBlocked(压过一切);② 让位表内任一 owner 在场 → Yield(owner
        /// 版本漂移不影响,Present 即让位);③ 默认 Own。family 缺键按缺席处理
        /// (不 NRE);未知 unitId 抛 ArgumentException(上游 typo 早炸)。
        /// </summary>
        public static RouteDecision Decide(string unitId, IReadOnlyDictionary<string, FamilyState> family)
        {
            HookUnitDef def;
            if (!Defs.TryGetValue(unitId, out def))
                throw new ArgumentException("알 수 없는 패치 구성 요소 ID: " + unitId, "unitId");

            // ⓪ 写透豁免(R-F):写对方官方配置键,抗版本漂移靠键存在性守卫
            //    (Get(key)==null → no-op),不参与让位/版本压制裁决。
            if (def.Kind == CarrierKind.WriteThrough)
                return RouteDecision.Own;

            // ① 版本压制(§4.6):目标在场但版本不命中绑定表。game.* 单元
            //    TargetMod=null,永不压制。
            if (def.TargetMod != null && IsPresent(def.TargetMod, family)
                && !IsVersionOk(def.TargetMod, family))
                return RouteDecision.VersionBlocked;

            // ② 让位(spec §5.4):任一 owner 在场即让位(子 mod 版本漂移时对方
            //    依旧接管语义,让位仍成立)。
            string[] owners;
            if (YieldOwners.TryGetValue(unitId, out owners))
            {
                foreach (string owner in owners)
                    if (IsPresent(owner, family)) return RouteDecision.Yield;
            }

            // ③ 默认 Own(DataWrite 无命中时恒 Own)。
            return RouteDecision.Own;
        }

        /// <summary>
        /// owner mod ID → 面板显示名(internal:SelfTestRunner 让位断言的 detail
        /// 文案「让位中(由 &lt;owner&gt; 接管),断言原版回退」,R6-F2)。
        /// 未知 id 抛 KeyNotFoundException(调用方只传 YieldOwners 出现过的 id)。
        /// </summary>
        internal static string OwnerDisplay(string modId)
        {
            return OwnerNames[modId];
        }

        /// <summary>
        /// 让位注明文案(面板 label 用):"启用&lt;owner 显示名&gt;,该配置不生效"
        /// (0.6.0 R4 文案修订:从「&lt;owner&gt;在场,由此让位」改为玩家视角的
        /// 不生效提示);多 owner 按固定序 / 连接(该单元的任一 owner 在场均触发
        /// 让位,列出全部潜在 owner 是准确表述)。非让位单元(含未知 id)返回
        /// null。无 family 参数——文案只描述单元的让位条件,是否显示由调用方结合
        /// 当前裁决(Decide == Yield)决定。
        /// </summary>
        public static string YieldNote(string unitId)
        {
            string[] owners;
            if (!YieldOwners.TryGetValue(unitId, out owners)) return null;
            var names = new string[owners.Length];
            for (int i = 0; i < owners.Length; i++) names[i] = OwnerNames[owners[i]];
            return "활성화된 " + string.Join("/", names) + " 때문에 이 설정은 적용되지 않습니다";
        }

        /// <summary>
        /// 批量裁决(Task 8 compose 入口):遍历 HookCatalog.Units 全部 110 个 id
        /// (53 MainHook + 4 DataWrite + 35 WriteThrough + 18 SubModHook,0.7.0 终态;
        /// 其中 main.maiden_attrs_iph 为 MainHook 但 TargetMod = IPH,不在 YieldOwners——
        /// 三态[在场匹配 Own/漂移 VersionBlocked/缺席 Own]全经 ① 版本压制与面板
        /// 条件 Define 达成),返回完整 unitId → decision 裁决表(WriteThrough 恒
        /// Own,R-F)。
        /// </summary>
        public static IReadOnlyDictionary<string, RouteDecision> DecideAll(
            IReadOnlyDictionary<string, FamilyState> family)
        {
            var table = new Dictionary<string, RouteDecision>(HookCatalog.Units.Count, StringComparer.Ordinal);
            foreach (HookUnitDef u in HookCatalog.Units) table.Add(u.Id, Decide(u.Id, family));
            return table;
        }

        private static bool IsPresent(string modId, IReadOnlyDictionary<string, FamilyState> family)
        {
            FamilyState st;
            return family.TryGetValue(modId, out st) && st.Present;
        }

        private static bool IsVersionOk(string modId, IReadOnlyDictionary<string, FamilyState> family)
        {
            FamilyState st;
            return family.TryGetValue(modId, out st) && st.VersionOk;
        }
    }
}
