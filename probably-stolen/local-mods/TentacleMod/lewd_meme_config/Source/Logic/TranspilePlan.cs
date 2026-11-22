using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LewdMemeConfig
{
    /// <summary>
    /// 锚点指令描述(transpiler 模式匹配的最小单元):一条 IL 指令的
    /// 「操作码 + 操作数」约束。操作数语义按运行时类型分派
    /// (见 <see cref="TranspileCarrier"/> 匹配器,此处只存数据):
    ///  <list type="bullet">
    ///   <item>null —— 要求 CodeInstruction.operand == null(短形 ldc.i4.n /
    ///     conv.* / stind.ref / ret 等无操作数指令)</item>
    ///   <item>int / long / float / double / string 装箱字面量 —— 数值/字符串
    ///     相等(ldc.i4 / ldc.i4.s / ldc.i8 / ldc.r4 / ldstr;字节宽度差异由
    ///     匹配器统一数值化,sbyte/int 互认)</item>
    ///   <item><see cref="LocalRef"/> —— ldloc/ldloca/stloc 系:operand 为
    ///     LocalBuilder,按 LocalIndex 比较(局部槽位是跨版本最稳的结构锚,
    ///     1.9.2 反编译窗口里的 ldloc.s 7 即此)</item>
    ///   <item><see cref="MemberRef"/>("DeclaringTypeName.MemberName")——
    ///     call/callvirt/newobj/ldfld/stfld/ldsfld/ldftn 系:operand 为
    ///     MemberInfo,按 DeclaringType.Name + "." + Name 比较(ctor 的
    ///     MemberName 为 ".ctor";不匹配签名——窗口上下文已足够)</item>
    ///   <item><see cref="Any"/> 哨兵 —— 操作数通配(仅允许出现在 Before/
    ///     After 窗口中约束操作码,如分支目标 Label;Match 段禁用通配,
    ///     防错删)</item>
    ///  </list>
    /// </summary>
    public sealed class AnchorInstr
    {
        public OpCode Opcode;
        public object Operand;
    }

    /// <summary>局部槽位锚(ldloc*/stloc*/ldloca*):按 LocalBuilder.LocalIndex 匹配。</summary>
    public sealed class LocalRef
    {
        public int Index;
    }

    /// <summary>成员锚(call/callvirt/newobj/ldfld/stfld/ldsfld/ldftn):按 "DeclaringType.Name.MemberName" 匹配。</summary>
    public sealed class MemberRef
    {
        public string Name;
    }

    /// <summary>
    /// 锚点构造辅助(仅本表使用,压缩表行):I(OpCode[,operand]) = 字面量/
    /// 无操作数指令;L(OpCode,slot) = 局部槽位;M(OpCode,"Type.Member") =
    /// 成员;W(OpCode) = 窗口内操作数通配(分支跳转等)。
    /// </summary>
    internal static class Anchor
    {
        /// <summary>窗口内操作数通配哨兵(分支目标 Label / ExceptionBlock 等)。</summary>
        public static readonly object Any = new object();

        public static AnchorInstr I(OpCode op)
        {
            return new AnchorInstr { Opcode = op, Operand = null };
        }

        public static AnchorInstr I(OpCode op, object operand)
        {
            return new AnchorInstr { Opcode = op, Operand = operand };
        }

        public static AnchorInstr L(OpCode op, int slot)
        {
            return new AnchorInstr { Opcode = op, Operand = new LocalRef { Index = slot } };
        }

        public static AnchorInstr M(OpCode op, string member)
        {
            return new AnchorInstr { Opcode = op, Operand = new MemberRef { Name = member } };
        }

        public static AnchorInstr W(OpCode op)
        {
            return new AnchorInstr { Opcode = op, Operand = Any };
        }
    }

    /// <summary>
    /// 注入点替换形态(决定 TranspileCarrier 注入哪组 getter 调用指令)。
    /// 选择说明:统一入口 = 「ldstr key + call 形态化 getter」而非每键强类型
    /// getter——45 键的 getter 全部集中在 TranspileValues 的 9 个形态方法里,
    /// 新键只加表行不加方法;int/float/bool/string 类型由形态方法签名承载
    /// (DirectInt → int、DirectFloat → float、CSV 键 → string 解析),
    /// 替换指令的栈类型因此静态正确。
    /// </summary>
    public enum PointForm
    {
        /// <summary>ldc → ldstr key; call int GetInt(key)。int 直读。</summary>
        DirectInt,

        /// <summary>ldc → ldstr key; call float GetFloat(key)。float 直读。</summary>
        DirectFloat,

        /// <summary>倒数形态(nightly.conceive_per_sex):原 ldc.r4 100 是除数,
        /// cfg 语义为受孕率(默认 0.01)→ getter 返回 1f/cfg,默认恒 100 零漂移;
        /// cfg ≤ 0 返回 float.MaxValue(除尽→概率≈0 = 永不受孕的保守读法)。</summary>
        ReciprocalFloat,

        /// <summary>组合形态(nightly.semen_chance_per_train):原 ldc.r4 0.1 是
        /// train/10×0.1 的尾乘子,cfg 语义为每点调教贡献(默认 0.01)→ 返回 cfg*10f。</summary>
        PerTrainScaled,

        /// <summary>printer.recipe_cost 折叠形态:原 ldc.i4 150 = 15×10(进度
        /// 存储 ×10),cfg 默认 15 → getter 返回 cfg*10。</summary>
        RecipeCostScaled,

        /// <summary>CSV 区间下界(maiden.attr_*_range):面板 "lo,hi" 闭区间,
        /// 下界注入点 = Random::Range 第一参 → 返回 lo。</summary>
        RangeLow,

        /// <summary>CSV 区间上界:Random::Range 上界排除 → IL 常量 = hi+1
        /// (默认 "70,80" → ldc 81),getter 返回 hi+1。</summary>
        RangeHigh,

        /// <summary>阵营基价(maiden.faction_base):Arg = CSV 查询名;
        /// 未命中回落「其他」项(三 case 共享点固定查「治安部」——整组值恒等
        /// 的既定裁决,勘察摘要 E 节)。</summary>
        FactionValue,

        /// <summary>陷阱策略捕捉率(trap.tactic_chances)整序列替换形态:
        /// Match = [ldsfld TACTIC_CHANCES; ldloc.s 档位; ldelem.r4] 三条,
        /// 替换为 [原 ldloc 指令(档位); call float GetTacticChance(int)]——
        /// 读 CSV 第档位值,不可 patch &lt;clinit&gt;(一次性快照违反即时生效)。</summary>
        TacticChance,
    }

    /// <summary>
    /// 单个注入点:Match = 要匹配并整体替换的连续指令序列(标准 ldc→getter
    /// 形态长度 1;序列替换形态如 TacticChance 长度 3);Before / After =
    /// Match 段前后必须「紧邻且按序」出现的窗口指令(null / 空数组 = 不检查
    /// 该侧;窗口内允许 <see cref="Anchor.Any"/> 通配)。Count = 期望匹配次数
    /// (必须恰好命中,多/少都抛 <see cref="TranspileMatchException"/>——
    /// 数量本身也是锚:版本漂移新增/丢失同形 ldc 时拒装记 Dead,绝不静默
    /// 错替)。Arg = 形态辅助参数(FactionValue 的阵营名)。
    /// 锚定铁律(T2):一律前后文窗口锚定,禁按 IL_XXXX 偏移匹配——偏移只对
    /// 1.9.2 这一份 DLL 有效,窗口内容才是跨版本指纹。
    /// </summary>
    public sealed class TranspilePoint
    {
        public AnchorInstr[] Match;
        public AnchorInstr[] Before;
        public AnchorInstr[] After;
        public int Count = 1;
        public PointForm Form;
        public string Arg;
    }

    /// <summary>
    /// 一条 transpile 配置:Key = CfgKeys 键名(注册表面板键,不另立键);
    /// Default = 1.9.2 反编译常量(交付校验用,与 ConfigRegistry 默认值同源
    /// 同值——本表不改变注册表默认,仅作逐字对照基准);MethodId = 消费
    /// 单元标识(与 HookCatalog 单元 id 同命名空间,Task 4-8 装配委托按此
    /// 取注入点);Target = 补丁目标方法键("TypeName::MethodName",
    /// TranspileCarrier 按 original 反查 methodId 用);Deferred = 占位标记
    /// (ldloc 特形等本任务不交付的形态,有键与默认值、不产出 Points)。
    /// </summary>
    public sealed class TranspileEntry
    {
        public string Key;
        public object Default;
        public string MethodId;
        public string Target;
        public bool Deferred;
        public TranspilePoint[] Points;
    }

    /// <summary>
    /// transpiler 注入点未按期望匹配(缺失/多出/目标方法不在表内)。
    /// 抛出语义:install 委托内 harmony.Patch 应用 transpiler 时即时求值,
    /// 异常从 Patch 传播 → HookInstaller.Reconcile 的逐单元 try/catch 捕获 →
    /// 该单元记 Dead(LastDeadReason 记本消息)不炸游戏、不阻断其余单元;
    /// 开关 off→on 可显式重试。任何注入点未命中即整体拒绝 = 宁可 Dead
    /// 不静默半改(半改的数值方法比原版更糟)。
    /// </summary>
    public sealed class TranspileMatchException : Exception
    {
        public TranspileMatchException(string message) : base(message) { }
    }

    /// <summary>
    /// transpiler 注入点表(0.7.0 Task 3,本 mod 首个 Harmony transpiler 形态
    /// 的数据面;消费方 = Source/Integration/TranspileCarrier.cs,装配方 =
    /// Task 4-8 各迁移单元)。纯数据 + 查询,零游戏/Harmony 依赖
    /// (System.Reflection.Emit.OpCode 仅是操作码常量载体),进测试工程
    /// Logic/ 通配。
    /// 事实源:家族勘察摘要「0.7.0 IL 注入点清单」(2026-11-17,Task 2;
    /// 基线 lewd_meme_192 全程序集 IL dump 33841 行,已存
    /// %TEMP%\opencode\decomp\lewd_meme_192\il-full.txt)。表中每个窗口都
    /// 对该 dump 逐指令核对过;主 mod 更新时先重 dump 再按窗口重对,
    /// 锚不中自动 Dead(本表的失效模式是安全的)。
    /// 1.6.1 共存重锚(2026-11-17):4 个窗口的短跳转锚(ovulate_chance
    /// After / gestation_days After / milk_value_base Before /
    /// trap.semen_chance After)被 ShadowTrade ExpandBranches 短→长改写,
    /// 已置空改由 Match 唯一值锚定——窗口须对「ShadowTrade 插入后的 IL」
    /// 与原 IL 双形态同时成立,详见勘察摘要「1.6.1 让位翻转·IL 共存推演」。
    /// 46 键 = 44 标准 ldc→getter + 2 ldloc 特形(layout.brood_size /
    /// output_size 共享单 ldc 独立化,Task 7 交付;Deferred 占位机制保留
    /// 供未来形态)。放弃键 6 个(sex_per_level_
    /// min / oral_min_base / oral_per_level_min / anal_min_base /
    /// anal_per_level_min / alarm.semen_output,IL 折叠或无数值消费点)
    /// 与 postfix(C')6 键 / prefix 零迁移 3 键不在此表(载体不同)。
    /// </summary>
    public static class TranspilePlan
    {
        // —— 目标方法键(TranspileCarrier 以 original.DeclaringType.Name +
        //    "::" + original.Name 反查;NightlyTick 挂两个 methodId——数值面
        //    与乳汁面让位语义不同[乳汁让位 Brood],两单元各挂各的 transpiler,
        //    替换点不相交,叠加安全) ——
        private const string TargetNightlyTick = "TentacleBody::NightlyTick";
        private const string TargetTryCorrupt = "CorruptTentacle::TryCorrupt";
        private const string TargetOnSemenUsed = "CorruptTentacle::OnSemenUsed";
        private const string TargetProduceDaughter = "TentacleBody::ProduceDaughter";
        private const string TargetCreate = "TentacleBody::Create";
        private const string TargetBuildWindow = "TentacleBody::BuildWindow";
        private const string TargetRollAttr = "ItemFactory::RollAttr";
        private const string TargetFactionBase = "ItemFactory::FactionBase";
        private const string TargetAlarmTick = "AlarmClock::NightlyTickOnce";
        private const string TargetAlarmBuild = "AlarmClock::BuildWindow";
        private const string TargetAlarmSell = "AlarmClock::TrySellToNpc";
        private const string TargetTrapBuild = "TentacleTrap::BuildWindow";
        private const string TargetTrapTick = "TentacleTrap::NightlyTickOnce";
        private const string TargetScavSell = "PatchScavGeneralAlarm::Postfix";
        private const string TargetThiefSell = "PatchThiefHouseholdAlarm::Postfix";
        private const string TargetPrinterBuild = "TentaclePrinter::BuildWindow";
        private const string TargetPrinterTick = "TentaclePrinter::NightlyTickOnce";
        private const string TargetPrinterProgress = "TentaclePrinter::DailyProgress";
        // 0.8.0 G 节:tearl 15 Aquashard 差分(tearl 前置类型,TranspileCarrier
        // 反查键 = DeclaringType.Name + "::" + Name,tearl 类型照常命中)
        private const string TargetAquashardCreate = "CustomItem::CreateAquashard";
        private const string TargetBroodTributeTick = "BroodTribute::AfterNightlyTick";

        // ---- 快捷构造(压缩表行;P = 单点,Count 恒 1 的默认) ----
        private static TranspilePoint P(PointForm form, AnchorInstr[] match,
            AnchorInstr[] before, AnchorInstr[] after)
        {
            return new TranspilePoint { Form = form, Match = match, Before = before, After = after };
        }

        private static TranspilePoint[] PointsOf(params TranspilePoint[] pts)
        {
            return pts;
        }

        /// <summary>
        /// 全量注入点表(46 条,Task 7 起 Deferred 清零)。行序 = 勘察摘要
        /// A–F 节序;注释里的 IL_XXXX 偏移仅为对 dump 的人工检索坐标,
        /// 不参与匹配。
        /// </summary>
        public static readonly IReadOnlyList<TranspileEntry> Entries = new[]
        {
            // ================= A. TentacleBody.NightlyTick(数值面 14 键)=================
            // 局部槽位(dump NightlyTick 段):4=train 5=sexCount 6=lactation
            // 7=bodyLevel 8=sexMin 9=sexMax 10=oralMin 11=oralMax 12=analMax
            // 13=sexRolls 19=semenChance 21=conceive 22=pregnantDays 23=milkValue
            new TranspileEntry
            {
                Key = CfgKeys.SexMinBase, Default = 5,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_0070 ldc.i4.5:GetBodyLevel 存槽 7 之后、ldloc.7-1 之前
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_5) },
                    new[] { Anchor.M(OpCodes.Call, "TentacleBody.GetBodyLevel"), Anchor.L(OpCodes.Stloc_S, 7) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.SexMaxBase, Default = 10,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_0078 ldc.i4.s 10:stloc.8(第一组结果落槽)之后
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_S, 10) },
                    new[] { Anchor.L(OpCodes.Stloc_S, 8) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub),
                            Anchor.I(OpCodes.Ldc_I4_3), Anchor.I(OpCodes.Mul) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.SexPerLevelMax, Default = 3,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_007e ldc.i4.3:ldloc.7-1 之后、mul 之前(方法内另一 ldc.i4.3
                // 是 IL_022f lactation_cap,After=[Min] 窗口区分)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_3) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub) },
                    new[] { Anchor.I(OpCodes.Mul), Anchor.I(OpCodes.Add), Anchor.L(OpCodes.Stloc_S, 9) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.OralMaxBase, Default = 2,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_0089 ldc.i4.2:stloc.10(L-1 基值落槽)之后;方法内四处
                // ldc.i4.2 靠槽位窗口两两区分(见 oral/anal per_level_max)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_2) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub),
                            Anchor.L(OpCodes.Stloc_S, 10) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub),
                            Anchor.I(OpCodes.Ldc_I4_2), Anchor.I(OpCodes.Mul) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.OralPerLevelMax, Default = 2,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_008e ldc.i4.2:sub 后 mul 前,结果落槽 11
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_2) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub) },
                    new[] { Anchor.I(OpCodes.Mul), Anchor.I(OpCodes.Add), Anchor.L(OpCodes.Stloc_S, 11) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AnalMaxBase, Default = 2,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_0097 ldc.i4.2:第三组首 ldc(L-1 已算好在栈上,无 stloc 间隔),
                // After 同形第四组(ldloc.7-1)——与 oral_max_base(IL_0089)的
                // 区分:其前隔 stloc.10,本点直接前文就是 ldloc.7-1
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_2) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub),
                            Anchor.I(OpCodes.Ldc_I4_2), Anchor.I(OpCodes.Mul) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AnalPerLevelMax, Default = 2,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_009c ldc.i4.2:结果落槽 12(与 oral 的槽 11 区分)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_2) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 7), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Sub) },
                    new[] { Anchor.I(OpCodes.Mul), Anchor.I(OpCodes.Add), Anchor.L(OpCodes.Stloc_S, 12) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.OvulateChance, Default = 1f / 7f,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_017f ldc.r4 0.14285715(方法内唯一):Random.value 之后、
                // bge.un.s 之前(1f/7f 编译期折叠产物,但独立存在可直接换)。
                // 1.6.1 共存重锚:原 After=[bge.un.s] 被 ShadowTrade(暗流交易
                // = lewd_meme_remake 1.6.1)TickTranspiler 的 ExpandBranches
                // 扩为 bge.un,双形态不可兼锚 → After 置空,唯一性由 Match 值
                // (方法内 1 处)+ Before=[Random.get_value] 承担(勘察摘要
                // 「1.6.1 让位翻转·IL 共存推演」)
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 1f / 7f) },
                    new[] { Anchor.M(OpCodes.Call, "Random.get_value") },
                    null)),
            },
            new TranspileEntry
            {
                Key = CfgKeys.ConceivePerSex, Default = 0.01f,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_01d4 ldc.r4 100(倒数形态):ldloc.13 conv.r4 之后、div 之前,
                // div+Clamp01 窗口(方法内唯一);getter 返回 1f/cfg → 默认恒 100
                Points = PointsOf(P(PointForm.ReciprocalFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 100f) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 13), Anchor.I(OpCodes.Conv_R4) },
                    new[] { Anchor.I(OpCodes.Div), Anchor.M(OpCodes.Call, "Mathf.Clamp01") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.GestationDays, Default = 30,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_0291 ldc.i4.s 30(方法内唯一):孕天数 +1 落槽 22 之后、blt 之前。
                // 1.6.1 共存重锚:原 After=[blt.s] 被 ShadowTrade ExpandBranches
                // 扩为 blt → After 置空,唯一性由 Match 值(方法内 1 处)+
                // Before=[ldloc.s 22] 承担
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_S, 30) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 22) },
                    null)),
            },
            new TranspileEntry
            {
                Key = CfgKeys.LactationCap, Default = 3,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_022f ldc.i4.3:SetInt(MAIDEN_PREGNANT_DAYS,0) 之后、
                // Min(3, lactation+1) 首参
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_3) },
                    new[] { Anchor.I(OpCodes.Ldc_I4_0), Anchor.M(OpCodes.Call, "ItemFactory.SetInt") },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 6), Anchor.I(OpCodes.Ldc_I4_1), Anchor.I(OpCodes.Add),
                            Anchor.M(OpCodes.Call, "Mathf.Min") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.SemenChanceBase, Default = 0.4f,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_02cd ldc.r4 0.4:cap(ldc.r4 1)之后、和式首加数之前
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.4f) },
                    new[] { Anchor.I(OpCodes.Ldc_R4, 1f) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 4), Anchor.I(OpCodes.Conv_R4) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.SemenChancePerTrain, Default = 0.01f,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_02db ldc.r4 0.1(组合形态 train/10×0.1;方法内唯一):
                // ldc.r4 10 div 之后、mul 之前;getter 返回 cfg*10f → 默认恒 0.1
                Points = PointsOf(P(PointForm.PerTrainScaled,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.1f) },
                    new[] { Anchor.I(OpCodes.Ldc_R4, 10f), Anchor.I(OpCodes.Div) },
                    new[] { Anchor.I(OpCodes.Mul), Anchor.I(OpCodes.Add) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.SemenChanceCap, Default = 1f,
                MethodId = "main.nightly_transpile", Target = TargetNightlyTick,
                // IL_02c8 ldc.r4 1(方法内唯一):Min 首参;直接前文是
                // stsfld ReportMilk(乳汁报表 +1 落库)之后、base(ldc.r4 0.4)之前
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 1f) },
                    new[] { Anchor.M(OpCodes.Stsfld, "TentacleBody.ReportMilk") },
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.4f) })),
            },

            // ================= B. TentacleBody.NightlyTick 乳汁两键(让位 Brood)=================
            new TranspileEntry
            {
                Key = CfgKeys.MilkValueBase, Default = 100,
                MethodId = "main.nightly_milk_value", Target = TargetNightlyTick,
                // IL_02a8 ldc.i4.s 100(方法内唯一):条件汇合后、TryProduceMilk 值式首。
                // 1.6.1 共存重锚:原 Before=[brfalse.s] 被 ShadowTrade
                // ExpandBranches 扩为 brfalse → Before 置空,唯一性由 Match 值
                // (方法内 1 处)+ After=[ldloc.s 6; ldc.i4.s 40; mul] 承担
                // (After 第 2 条是 milk_value_per_lv 的 Match——跨键互指,
                // 两遍式第一遍原流求值天然支持)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_S, 100) },
                    null,
                    new[] { Anchor.L(OpCodes.Ldloc_S, 6), Anchor.I(OpCodes.Ldc_I4_S, 40), Anchor.I(OpCodes.Mul) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.MilkValuePerLv, Default = 40,
                MethodId = "main.nightly_milk_value", Target = TargetNightlyTick,
                // IL_02ac ldc.i4.s 40(方法内唯一):base 之后、mul 之前
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_S, 40) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 6) },
                    new[] { Anchor.I(OpCodes.Mul), Anchor.I(OpCodes.Add), Anchor.L(OpCodes.Stloc_S, 23) })),
            },

            // ================= C. CorruptTentacle 两方法(corrupt_cost 六点 +
            //     on_semen_used 三点:semen_recharge 1 + max_charge 2[T6 补齐])=================
            new TranspileEntry
            {
                Key = CfgKeys.CorruptCost, Default = 100,
                MethodId = "main.corrupt_cost", Target = TargetTryCorrupt,
                // 门槛 1 处(IL_0029:ldloc.0; ldc; bge.s)+ 扣减 3 处统一模式
                // (ldloc.0; ldc; sub; SetCharge)。0.8.0:1.10.0 三分支重复体
                // (alarm/printer/manhole TryConvert)合并为委托 func,扣减点
                // 6→3(IL_00ea/01ae/0270,2.1.0 IL dump 实核);不收 2 处仅入
                // 文本的 100:Toast Int32.ToString(IL_004f)与 TryConvert 委托
                // 第 4 参(IL_01a2,callvirt Func——合并后仅 1 处)——两锚均
                // 不命中,天然跳过。
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 100) },
                        Before = new[] { Anchor.I(OpCodes.Ldloc_0) },
                        After = new[] { Anchor.W(OpCodes.Bge_S) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 100) },
                        Before = new[] { Anchor.I(OpCodes.Ldloc_0) },
                        After = new[] { Anchor.I(OpCodes.Sub), Anchor.M(OpCodes.Call, "CorruptTentacle.SetCharge") },
                        Count = 3,
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.CorruptSemen, Default = 10,
                MethodId = "main.on_semen_used", Target = TargetOnSemenUsed,
                // IL_0052 ldc.i4.s 10(方法内唯一):Min(10, 100-charge) 首参。
                // After 窗口首条 ldc.i4.s 100 是 max_charge 截断点的 Match 指令
                // ——跨键窗口互指,匹配器两遍式(第一遍全原流求值)天然支持
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_S, 10) },
                    new[] { Anchor.I(OpCodes.Ldc_I4_0), Anchor.I(OpCodes.Ret) },
                    new[] { Anchor.I(OpCodes.Ldc_I4_S, 100), Anchor.I(OpCodes.Ldloc_1), Anchor.I(OpCodes.Sub),
                            Anchor.M(OpCodes.Call, "Mathf.Min") })),
            },
            new TranspileEntry
            {
                // 0.7.0 T6(max_charge 两消费点随单元注入,T2 顾虑①裁决:
                // 只注 semen_recharge 而 max > 100 时,门槛 ldc 100 会在
                // 100 处提前拒收——两点补齐后满判定/截断与钳制上限同源)。
                // 方法内 ldc.i4.s 100 实有 4 处:门槛/截断(本条注入)+
                // IL_00a8(toast「/100」拼接)+ IL_0105(ModCtx.Log 日志
                // 拼接)——两纯文本点不命中窗口(前文 ldc.i4.5、后续
                // stloc.3 短格式落槽 3)天然跳过(同 corrupt_cost 的
                // 「仅入文本不收」取舍)
                Key = CfgKeys.CorruptMaxCharge, Default = 100,
                MethodId = "main.on_semen_used", Target = TargetOnSemenUsed,
                // 门槛 1(IL_003d:ldloc.1; ldc.i4.s 100; blt.s——charge < max
                // 才吸取,blt 严格小于 = 已饱拒收含等号)+ 截断 1(IL_0054:
                // Min(semen, max-charge) 的减法项,缺口封顶)
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 100) },
                        Before = new[] { Anchor.I(OpCodes.Ldloc_1) },
                        After = new[] { Anchor.W(OpCodes.Blt_S) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 100) },
                        Before = new[] { Anchor.I(OpCodes.Ldc_I4_S, 10) },
                        After = new[] { Anchor.I(OpCodes.Ldloc_1), Anchor.I(OpCodes.Sub),
                                        Anchor.M(OpCodes.Call, "Mathf.Min") },
                    }),
            },

            // ================= D. ProduceDaughter / Create / BuildWindow =================
            new TranspileEntry
            {
                Key = CfgKeys.DaughterValue, Default = 900,
                MethodId = "main.produce_daughter_value", Target = TargetProduceDaughter,
                // methodId ≠ 单元 id(单元 id = main.produce_daughter):让位过滤对
                // 查不到的 id 保守全应用,其让位语义完全依赖登记期不注册;未来若
                // 为此单元加让位,必须先改名对齐单元 id(机制详见 TranspileCarrier.Run 注)
                // IL_0085 ldc.i4 900(方法内唯一)+ conv.i8(保留在原流,int→i8
                // 转换对 getter 的 int 结果同样合法,只换 ldc 一条)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4, 900) },
                    new[] { Anchor.I(OpCodes.Ldloc_0) },
                    new[] { Anchor.I(OpCodes.Conv_I8), Anchor.M(OpCodes.Callvirt, "GameItem.set_unitValue") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TabsPerLevel, Default = 2,
                MethodId = "main.create", Target = TargetCreate,
                // IL_00c4 ldc.i4.2:闭包字段 panels = bodyLevel×2 的乘子;mul 后
                // stfld(编译器生成名不稳,通配)+ ldstr "STORAGE_EXPANSION_
                // TABS_INT" 是绝对稳定的尾锚(Create 内其余 ldc.i4.2 不收)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_2) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 5), Anchor.I(OpCodes.Ldloc_0) },
                    new[] { Anchor.I(OpCodes.Mul), Anchor.W(OpCodes.Stfld),
                            Anchor.I(OpCodes.Ldloc_3), Anchor.I(OpCodes.Ldstr, "STORAGE_EXPANSION_TABS_INT") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.CorruptCols, Default = 3,
                MethodId = "main.build_window", Target = TargetBuildWindow,
                // IL_000a ldc.i4.3:侵蚀区 ctor 首参;直接前文是 IL_0009
                // stind.ref(grids out 参数落库)——(1.9.2 布局重构:固定 5 面板、
                // level 零消费;方法首 ldc.i4.s 16/stloc.0 与本点之间隔着
                // ldarg.1/newobj List/stind.ref,不能作 Before 窗口)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_3) },
                    new[] { Anchor.I(OpCodes.Stind_Ref) },
                    new[] { Anchor.I(OpCodes.Ldloc_0), Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.ToolCols, Default = 3,
                MethodId = "main.build_window", Target = TargetBuildWindow,
                // IL_00a9 ldc.i4.3:第四组(触手区)ctor 首参;与腐蚀组的区分 =
                // 前文(腐蚀组紧随方法首 ldc16/stloc,触手组前是 List.Add)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_3) },
                    new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add") },
                    new[] { Anchor.I(OpCodes.Ldloc_0), Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor") })),
            },
            new TranspileEntry
            {
                // 0.7.0 T7 交付(Deferred 解除):1.9.2 苗床/产出区共享
                // IL_0000 的单一 ldc.i4.s 16(num 局部,槽 0)。独立化 =
                // 把苗床区 ctor(IL_003f/0040)连续两条 ldloc.0 整体替换为
                // 两组 [ldstr; call GetInt](ReplaceAt 的 DirectInt 按 Match
                // 段长度逐条等价替换——Match 长度 2 是首个多指令形态);
                // 短形 ldloc.0 无操作数,操作码本身即槽位锚。产出区
                // (IL_0074/0075)同形,靠 After 尾 stloc 槽位区分(brood
                // 落槽 2 / output 落槽 3,alarm/trap grids 先例同款);
                // 方法内另两处单条 ldloc.0(corrupt/tool 区 ctor 第二参,
                // 前邻 ldc.i4.3)不命中双连序列,天然跳过
                Key = CfgKeys.BroodSize, Default = 16,
                MethodId = "main.build_window", Target = TargetBuildWindow,
                Points = PointsOf(new TranspilePoint
                {
                    Form = PointForm.DirectInt,
                    Match = new[] { Anchor.I(OpCodes.Ldloc_0), Anchor.I(OpCodes.Ldloc_0) },
                    Before = new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add") },
                    After = new[] { Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"),
                                    Anchor.I(OpCodes.Stloc_2) },
                }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.OutputSize, Default = 16,
                MethodId = "main.build_window", Target = TargetBuildWindow,
                // 产出区 ctor(IL_0074/0075)两参 ldloc.0;After 尾 stloc.3
                Points = PointsOf(new TranspilePoint
                {
                    Form = PointForm.DirectInt,
                    Match = new[] { Anchor.I(OpCodes.Ldloc_0), Anchor.I(OpCodes.Ldloc_0) },
                    Before = new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add") },
                    After = new[] { Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"),
                                    Anchor.I(OpCodes.Stloc_3) },
                }),
            },

            // ================= E. maiden 数值段(1.9.2 拆为独立方法)=================
            new TranspileEntry
            {
                Key = CfgKeys.AttrHighChance, Default = 0.01f,
                MethodId = "main.maiden_roll", Target = TargetRollAttr,
                // IL_0007 ldc.r4 0.01:Random.value 落槽 0 之后、bge.un.s 之前
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.01f) },
                    new[] { Anchor.I(OpCodes.Ldloc_0) },
                    new[] { Anchor.W(OpCodes.Bge_Un_S) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AttrMidChance, Default = 0.06f,
                MethodId = "main.maiden_roll", Target = TargetRollAttr,
                // IL_0019 ldc.r4 0.06(累计阈值形态:high ≤ v < mid → 中档)
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.06f) },
                    new[] { Anchor.I(OpCodes.Ldloc_0) },
                    new[] { Anchor.W(OpCodes.Bge_Un_S) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AttrHighRange, Default = "70,80",
                MethodId = "main.maiden_roll", Target = TargetRollAttr,
                // IL_000e/IL_0010 ldc.i4.s 70 / 81:Random.Range(70,81) =
                // 闭区间 [70,80](上界排除 → IL 常量 = 面板 hi+1)
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.RangeLow,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 70) },
                        Before = new[] { Anchor.W(OpCodes.Bge_Un_S) },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_S, 81), Anchor.M(OpCodes.Call, "Random.Range") },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.RangeHigh,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 81) },
                        Before = new[] { Anchor.I(OpCodes.Ldc_I4_S, 70) },
                        After = new[] { Anchor.M(OpCodes.Call, "Random.Range"), Anchor.I(OpCodes.Ret) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AttrMidRange, Default = "60,70",
                MethodId = "main.maiden_roll", Target = TargetRollAttr,
                // IL_0020/IL_0022 ldc.i4.s 60 / 71(闭区间 [60,70])
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.RangeLow,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 60) },
                        Before = new[] { Anchor.W(OpCodes.Bge_Un_S) },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_S, 71), Anchor.M(OpCodes.Call, "Random.Range") },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.RangeHigh,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 71) },
                        Before = new[] { Anchor.I(OpCodes.Ldc_I4_S, 60) },
                        After = new[] { Anchor.M(OpCodes.Call, "Random.Range"), Anchor.I(OpCodes.Ret) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AttrLowRange, Default = "10,60",
                MethodId = "main.maiden_roll", Target = TargetRollAttr,
                // IL_002a ldc.i4.s 10:方法尾部,消费点即默认出口;直接前文是
                // 第二组的 ret(不是前两组的 bge.un.s——第三组走完前两组已返回)
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.RangeLow,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 10) },
                        Before = new[] { Anchor.I(OpCodes.Ret) },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_S, 61), Anchor.M(OpCodes.Call, "Random.Range") },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.RangeHigh,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 61) },
                        Before = new[] { Anchor.I(OpCodes.Ldc_I4_S, 10) },
                        After = new[] { Anchor.M(OpCodes.Call, "Random.Range"), Anchor.I(OpCodes.Ret) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.FactionBase, Default = "上层区:100,治安部:50,黑市:50,革命军:50,其他:10",
                MethodId = "main.maiden_faction_base", Target = TargetFactionBase,
                // IL_000d(上层区 100)/ IL_0037(治安部·黑市·革命军三 case 合流
                // 50,共享单 ldc → 整组值恒等,固定查「治安部」)/ IL_003a(default 10 =
                // CSV「其他」项);三处 After=[ret] + 目标值唯一
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.FactionValue, Arg = "上层区",
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 100) },
                        Before = new[] { Anchor.W(OpCodes.Brfalse_S) },
                        After = new[] { Anchor.I(OpCodes.Ret) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.FactionValue, Arg = "治安部",
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 50) },
                        Before = new[] { Anchor.W(OpCodes.Brfalse_S) },
                        After = new[] { Anchor.I(OpCodes.Ret) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.FactionValue, Arg = "其他",
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 10) },
                        // IL_003a default 出口:直接前文是三 case 组的 ret
                        Before = new[] { Anchor.I(OpCodes.Ret) },
                        After = new[] { Anchor.I(OpCodes.Ret) },
                    }),
            },

            // ================= F. AlarmClock / TentacleTrap / TentaclePrinter =================
            new TranspileEntry
            {
                Key = CfgKeys.AlarmOralPerNight, Default = 1,
                MethodId = "main.alarm_nightly_once", Target = TargetAlarmTick,
                // IL_0056 ldc.i4.1:AddOral 第二参(1.9.2 三 Add 各自独立 ldc,
                // call 目标即区分锚;同方法其余 ldc.i4.1 不在 Add 序列)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_1) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 5) },
                    new[] { Anchor.M(OpCodes.Call, "DeflowerTracker.AddOral") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AlarmSexPerNight, Default = 1,
                MethodId = "main.alarm_nightly_once", Target = TargetAlarmTick,
                // IL_005e ldc.i4.1:AddSex 第二参
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_1) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 5) },
                    new[] { Anchor.M(OpCodes.Call, "DeflowerTracker.AddSex"), Anchor.I(OpCodes.Pop) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AlarmAnalPerNight, Default = 1,
                MethodId = "main.alarm_nightly_once", Target = TargetAlarmTick,
                // IL_0067 ldc.i4.1:AddAnal 第二参
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_1) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 5) },
                    new[] { Anchor.M(OpCodes.Call, "DeflowerTracker.AddAnal"), Anchor.I(OpCodes.Pop) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AlarmBroodSize, Default = 4,
                MethodId = "main.create_tentacle_alarm", Target = TargetAlarmBuild,
                // IL_0007/0008 ldc.i4.4 ×2:第一组 GameGridInventory(4,4) 苗床区
                // (两参同值,见 AlarmPatch 复刻体 BuildWindowBody);第二组
                // (IL_003c/d)归 output_size——After 尾的 stloc 槽位(0/1)区分
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_4) },
                        Before = new[] { Anchor.I(OpCodes.Stind_Ref) },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_4),
                                        Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_0) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_4) },
                        Before = new[] { Anchor.I(OpCodes.Stind_Ref), Anchor.I(OpCodes.Ldc_I4_4) },
                        After = new[] { Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_0) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AlarmOutputSize, Default = 4,
                MethodId = "main.create_tentacle_alarm", Target = TargetAlarmBuild,
                // IL_003c/003d ldc.i4.4 ×2:第二组 GameGridInventory(4,4) 产出区
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_4) },
                        Before = new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add") },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_4),
                                        Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_1) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_4) },
                        Before = new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add"), Anchor.I(OpCodes.Ldc_I4_4) },
                        After = new[] { Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_1) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.AlarmNpcSellChance, Default = 0.05f,
                MethodId = "main.alarm_try_sell_npc", Target = TargetAlarmSell,
                // IL_0005 ldc.r4 0.05(方法内唯一):Random.value 之后、blt.un.s 之前
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.05f) },
                    new[] { Anchor.M(OpCodes.Call, "Random.get_value") },
                    new[] { Anchor.W(OpCodes.Blt_Un_S) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TrapBroodSize, Default = 12,
                MethodId = "main.create_trap", Target = TargetTrapBuild,
                // IL_0014/0016 ldc.i4.s 12 ×2:第一组(12,12) 苗床区,结果落槽 1
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12) },
                        Before = new[] { Anchor.I(OpCodes.Stind_Ref) },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12),
                                        Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_1) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12) },
                        Before = new[] { Anchor.I(OpCodes.Stind_Ref), Anchor.I(OpCodes.Ldc_I4_S, 12) },
                        After = new[] { Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_1) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TrapOutputSize, Default = 12,
                MethodId = "main.create_trap", Target = TargetTrapBuild,
                // IL_004b/004d ldc.i4.s 12 ×2:第二组(12,12) 产出区,结果落槽 2
                Points = PointsOf(
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12) },
                        Before = new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add") },
                        After = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12),
                                        Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_2) },
                    },
                    new TranspilePoint
                    {
                        Form = PointForm.DirectInt,
                        Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12) },
                        Before = new[] { Anchor.M(OpCodes.Callvirt, "List`1.Add"), Anchor.I(OpCodes.Ldc_I4_S, 12) },
                        After = new[] { Anchor.M(OpCodes.Newobj, "GameGridInventory..ctor"), Anchor.I(OpCodes.Stloc_2) },
                    }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TrapTacticChances, Default = "0.1,0.2,0.3",
                MethodId = "main.trap_nightly_once", Target = TargetTrapTick,
                // IL_0097-009f:ldsfld TACTIC_CHANCES; ldloc.s 4(档位); ldelem.r4
                // 整序列替换 → [原 ldloc 指令; call GetTacticChance(int)](读 CSV
                // 第档位值;数组是 <clinit> 一次性快照,违反即时生效,不 patch)
                Points = PointsOf(new TranspilePoint
                {
                    Form = PointForm.TacticChance,
                    Match = new[]
                    {
                        Anchor.M(OpCodes.Ldsfld, "TentacleTrap.TACTIC_CHANCES"),
                        Anchor.L(OpCodes.Ldloc_S, 4),
                        Anchor.I(OpCodes.Ldelem_R4),
                    },
                    Before = new[] { Anchor.L(OpCodes.Stloc_S, 5) },
                    After = new[] { Anchor.L(OpCodes.Stloc_S, 6) },
                }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TrapSemenChance, Default = 0.5f,
                MethodId = "main.trap_nightly_once", Target = TargetTrapTick,
                // IL_01e9 ldc.r4 0.5(方法内唯一):Random.value 之后、bge.s 之前。
                // 1.6.1 共存重锚:原 After=[bge.s] 被 ShadowTrade 设备 transpiler
                // 的 ExpandBranches 扩为 bge → After 置空,唯一性由 Match 值
                // (方法内 1 处)+ Before=[Random.get_value] 承担
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.5f) },
                    new[] { Anchor.M(OpCodes.Call, "Random.get_value") },
                    null)),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TrapNpcSellChanceScav, Default = 0.1f,
                MethodId = "main.trap_sell_scav", Target = TargetScavSell,
                // PatchScavGeneralAlarm::Postfix IL_0005 ldc.r4 0.1:主 mod 自有
                // patch 类的 Postfix 方法,transpile 目标合法(Harmony 链)
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.1f) },
                    new[] { Anchor.M(OpCodes.Call, "AlarmClock.TrySellToNpc") },
                    new[] { Anchor.M(OpCodes.Call, "TentacleTrap.TrySellToNpc") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TrapNpcSellChanceThief, Default = 0.05f,
                MethodId = "main.trap_sell_thief", Target = TargetThiefSell,
                // PatchThiefHouseholdAlarm::Postfix IL_0005 同形
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 0.05f) },
                    new[] { Anchor.M(OpCodes.Call, "AlarmClock.TrySellToNpc") },
                    new[] { Anchor.M(OpCodes.Call, "TentacleTrap.TrySellToNpc") })),
            },
            // ⚠ 以下 printer 三 methodId(grids/nightly_transpile/progress)均与
            //   单元 id main.printer 不同名:methodId ≠ 单元 id——让位过滤对查不到
            //   的 id 保守全应用,其让位语义完全依赖登记期不注册;未来若为此单元
            //   加让位,必须先改名对齐单元 id(机制详见 TranspileCarrier.Run 注)
            new TranspileEntry
            {
                Key = CfgKeys.PrinterGrid, Default = 12,
                MethodId = "main.printer_grids", Target = TargetPrinterBuild,
                // IL_0014/16/4b/4d/82/84:三组 (12,12) 六处 ldc.i4.s 12 全集
                // (窗口为空 = 按值全集替换;Count=6 锁数量,漂移即拒装)
                Points = PointsOf(new TranspilePoint
                {
                    Form = PointForm.DirectInt,
                    Match = new[] { Anchor.I(OpCodes.Ldc_I4_S, 12) },
                    Count = 6,
                }),
            },
            new TranspileEntry
            {
                Key = CfgKeys.PrinterRecipeCost, Default = 15,
                MethodId = "main.printer_nightly_transpile", Target = TargetPrinterTick,
                // IL_0022 ldc.i4 150(方法内唯一):完工阈值 = 15×10 折叠(进度
                // 存储 ×10);getter 返回 cfg*10 → 默认恒 150。另 GetStatusText
                // 的两处纯文本 15 不收(判定口径:仅入文本的数值不收)
                Points = PointsOf(P(PointForm.RecipeCostScaled,
                    new[] { Anchor.I(OpCodes.Ldc_I4, 150) },
                    new[] { Anchor.I(OpCodes.Ldc_I4_0), Anchor.I(OpCodes.Stloc_2) },
                    new[] { Anchor.I(OpCodes.Stloc_3) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.PrinterBaseProgress, Default = 3f,
                MethodId = "main.printer_progress", Target = TargetPrinterProgress,
                // IL_0070 ldc.r4 3(方法内唯一):IntelBonus 之前的基础进度加数
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 3f) },
                    new[] { Anchor.I(OpCodes.Ldloc_2) },
                    new[] { Anchor.L(OpCodes.Ldloc_S, 5), Anchor.M(OpCodes.Call, "TentaclePrinter.IntelBonus"),
                            Anchor.I(OpCodes.Add) })),
            },

            // ================= G. tearl 15 Aquashard 差分(0.8.0;transpiler 首次
            //     用于子 mod 方法——tearl 是前置,patch 目标合法;注入点 = tearlaments
            //     dll IL dump,%TEMP%\opencode\decomp\tearl_15_il\tearlaments.il)=================
            new TranspileEntry
            {
                Key = CfgKeys.TearlAquashardValue, Default = 800,
                MethodId = "tearl.aquashard", Target = TargetAquashardCreate,
                // CreateAquashard IL_0032 ldc.i4 800(方法内唯一——ApplyItemSprite 的
                // 1/2 是 ldc.i4.1/ldc.i4.2 短形式,不同 opcode);After = conv.i8
                // 桥接 long(getter 返 int,conv 照跑)锚定尾部 ldsfld GameItemTypes。
                // 锚串 = DeclaringType.Name + "." + 成员名(嵌套类 Name 无声明方
                // 前缀,Final Review C1:含 '/' 即永不匹配 → 装配必 Dead)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4, 800) },
                    new[] { Anchor.M(OpCodes.Call, "CustomItem.LocalizedText") },
                    new[] { Anchor.I(OpCodes.Conv_I8),
                            Anchor.M(OpCodes.Ldsfld, "Aquashard.GameItemTypes") })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TearlAquashardChance, Default = 1f,
                MethodId = "tearl.aquashard", Target = TargetBroodTributeTick,
                // AfterNightlyTick IL_0000 ldc.r4 1 → stloc.0(Chance num;方法内
                // ldc.r4 1 两处——赋值与 IL_0071 概率上界比较,stloc.0 锚定区分;
                // 方法首指令 Before 为空窗)
                Points = PointsOf(P(PointForm.DirectFloat,
                    new[] { Anchor.I(OpCodes.Ldc_R4, 1f) },
                    new AnchorInstr[0],
                    new[] { Anchor.I(OpCodes.Stloc_0) })),
            },
            new TranspileEntry
            {
                Key = CfgKeys.TearlAquashardUnits, Default = 1,
                MethodId = "tearl.aquashard", Target = TargetBroodTributeTick,
                // AfterNightlyTick IL_0006 ldc.i4.1 → stloc.1(Units num2;ldc.i4.1
                // 多处——IL_0094 循环计数 num3++ 后随 add,stloc.1 锚定区分;
                // Before = 前条 stloc.0)
                Points = PointsOf(P(PointForm.DirectInt,
                    new[] { Anchor.I(OpCodes.Ldc_I4_1) },
                    new[] { Anchor.I(OpCodes.Stloc_0) },
                    new[] { Anchor.I(OpCodes.Stloc_1) })),
            },
        };

        /// <summary>全部 methodId(表序去重;与 HookCatalog 单元 id 同命名空间)。</summary>
        public static readonly IReadOnlyList<string> MethodIds = BuildMethodIds();

        // methodId → 非 Deferred 条目(查询缓存;Deferred 条目不产出注入点)
        private static readonly Dictionary<string, List<TranspileEntry>> byMethod =
            BuildIndex();

        // 目标方法键 → methodId 列表(TranspileCarrier 按 original 反查;
        // NightlyTick = [nightly_transpile, nightly_milk_value] 双单元)
        private static readonly Dictionary<string, List<string>> byTarget =
            BuildTargetIndex();

        // 键 → 默认值(getter 的注册表缺席兜底;装配期 PatchCtx.Config 必已
        // 注入,此通道纯防御)
        private static readonly Dictionary<string, object> defaultsByKey =
            BuildDefaults();

        private static List<string> BuildMethodIds()
        {
            var ids = new List<string>();
            foreach (var e in Entries)
                if (!ids.Contains(e.MethodId)) ids.Add(e.MethodId);
            return ids;
        }

        private static Dictionary<string, List<TranspileEntry>> BuildIndex()
        {
            var idx = new Dictionary<string, List<TranspileEntry>>();
            foreach (var e in Entries)
            {
                if (e.Deferred) continue;
                if (!idx.TryGetValue(e.MethodId, out var list))
                    idx[e.MethodId] = list = new List<TranspileEntry>();
                list.Add(e);
            }
            return idx;
        }

        private static Dictionary<string, List<string>> BuildTargetIndex()
        {
            var idx = new Dictionary<string, List<string>>();
            foreach (string id in MethodIds)
            {
                string target = TargetOf(id);
                if (!idx.TryGetValue(target, out var list))
                    idx[target] = list = new List<string>();
                list.Add(id);
            }
            return idx;
        }

        private static Dictionary<string, object> BuildDefaults()
        {
            var idx = new Dictionary<string, object>();
            foreach (var e in Entries) idx[e.Key] = e.Default;
            return idx;
        }

        /// <summary>
        /// 某 transpiler 单元的注入点集。未知 methodId / 只含 Deferred 条目的
        /// methodId 返回空数组(TranspileCarrier 对空集 no-op;Deferred 机制
        /// 保留供未来形态,当前表内零条)。
        /// 返回内部缓存实例,消费方不得改写。
        /// </summary>
        public static TranspileEntry[] Points(string methodId)
        {
            return byMethod.TryGetValue(methodId, out var list)
                ? list.ToArray()
                : new TranspileEntry[0];
        }

        /// <summary>methodId → 目标方法键("TypeName::MethodName");未知 id 返回 null。</summary>
        public static string TargetOf(string methodId)
        {
            foreach (var e in Entries)
                if (e.MethodId == methodId) return e.Target;
            return null;
        }

        /// <summary>
        /// 目标方法键 → 全部消费 methodId(同一 original 挂多个 transpiler
        /// 单元的反查入口;NightlyTick 先例)。未知目标返回空数组。
        /// </summary>
        public static string[] MethodIdsFor(string target)
        {
            return byTarget.TryGetValue(target, out var list) ? list.ToArray() : new string[0];
        }

        /// <summary>键 → 表内默认值(1.9.2 常量);未知键返回 null。</summary>
        public static object DefaultOf(string key)
        {
            return defaultsByKey.TryGetValue(key, out object v) ? v : null;
        }
    }
}
