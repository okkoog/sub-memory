using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 少女生成接管 + 生成事件 + 池写入(Task 9/14;0.7.0 Task 8 拆段):
    ///  <list type="bullet">
    ///   <item>main.create_maiden(MainHook 单元,开关开且版本受支持时装配)——
    ///     prefix 接管主 mod <c>MidnightPawn.Game.ItemFactory</c> 私有静态
    ///     <c>CreateMaiden()</c>(经 <c>dir.Add("maiden_lower", CreateMaiden)</c>
    ///     注册为少女工厂),返回 false 跳过原方法,重写按反编译 v1.9.2 逐行
    ///     复刻(无参版即 <c>return CreateMaidenOfFaction(null)</c> 薄包装,
    ///     prefix 复刻体传 null)。<b>0.7.0 T8 拆段后仅池段</b>:池抽取
    ///     (FACTIONS/JOBS/BODIES/BREASTS 经 PoolField 注册表直读优先)+
    ///     立绘尺寸 maiden.portrait_size(原版 64×64);属性三掷/阵营基价/
    ///     属性价格曲线改经委托直调主 mod RollAttr/FactionBase/AttrValue
    ///     (1.9.2 拆出的独立方法——本 mod 的 maiden_roll/maiden_faction_base
    ///     transpiler 挂其上,复刻体经委托调用 = 与原版 call 同一 patched 方法,
    ///     数值键经 transpiler 单元独立生效);1.9.2 行为对齐:无条件
    ///     Enable VIRGIN/ANAL_VIRGIN(原 0.6 掷骰段删除,maiden.virgin_chance
    ///     键废弃)+ 6 新属性 SetInt(AFFECTION/TRUST/HYPNOSIS=0,
    ///     INTERACT_CUR/MAX=1,BRAND=0,反编译 :132-:137);方法尾无 ApplyCsv
    ///     (CreateMaiden 系不在 ApplyCsv 消费清单——wondrous_milk/tentacle_
    ///     semen/tentacle_box/tentacle_manual/ovulation_shot 五 id 而已,核对
    ///     结论见 task-8-report)。</item>
    ///   <item>main.maiden_roll / main.maiden_faction_base(0.7.0 T8 新增,
    ///       transpiler 单元)——1.9.2 把属性掷骰/阵营基价拆为
    ///     <c>ItemFactory.RollAttr()</c>/<c>FactionBase(string)</c> 独立 public
    ///     static,注入点(TranspilePlan E 节)把三档概率/区间与三 case 常量换
    ///     getter 现读;默认值 = 1.9.2 字面量零漂移。IPH 在场仍 Own:其工厂
    ///     transpiler 只挂 CreateMaidenOfFaction(iph_130 OriginalCompat.cs:58/
    ///     :102),RollAttr/FactionBase 零触碰——R10 postfix 共存先例同构,
    ///     让位表零新行。</item>
    ///   <item>main.create_maiden_of_faction(MainHook 单元,Task 14)——prefix 接管
    ///     1.6.1 新增 public static
    ///     <c>CreateMaidenOfFaction(string)</c>(陷阱每夜捕获经其生成,原先绕过
    ///     无参版接管,api 配置/事件对该路径不生效)。faction 非空直用(跳过
    ///     Pick(FACTIONS)),null/空走池(对齐 1.9.2 L93-96);配置键与 Phase A/B
    ///     模式同 create_maiden。</item>
    ///   <item>pools.maiden_jobs / pools.maiden_fbb(DataWrite
    ///     载体,无补丁,不进 HookInstaller;R9 拆分,T9 后 DataWrite 仅此两池写):
    ///     jobs 单元 = JOBS(MaidenTraits.JOBS,IPH 工厂
    ///     transpiler 零触碰职业抽取,恒 Own);fbb 单元 = FACTIONS/BODIES/
    ///     BREASTS 三池(MaidenTraits 静态数组,IPH 在场时 route Yield 不写
    ///     ——其工厂 transpiler 把三池 Pick 替换成自家抽取,详见 Router R4 条;
    ///     玩家出口 = IPH 写透分区 body_weights_csv / body_affects_breast_size)。
    ///     任一池键变更经 HookRuntime 分发器
    ///     (OnConfigChanged)重写对应字段。R3:静态写入只做等长重复覆写(CSV
    ///     循环填充到原数组长度,见写入通道注释),服务未接管路径(主 mod 内部
    ///     消费/IPH 让位)与 GetExpNeed 显示;接管路径的生成与升级循环直读注册
    ///     (PoolField),不依赖静态写入成败。PoolValid 为
    ///     false(空/空白 CSV)→ 跳过写入保持原版池;
    ///     等长填充后与字段现值逐项相同 → 跳过(默认安装零写入,红线成立时
    ///     不碰主 mod 状态)。阵营基价 maiden.faction_base 无静态写入,
    ///     MaidenCfg.Read 直接 Current(注册表直读,R3-F3 核查确认)。
    ///     pools.maiden(EXP_NEED 经验表)已随 0.7.0 T9 删除——1.9.2 删了
    ///     TentacleBody.EXP_NEED 静态数组,经验公式化进 ExpNeedForLevel,
    ///     接管归 LevelCapPatch 的 main.exp_formula 单元;progress.exp_per_level
    ///     键全链废弃。</item>
    ///  </list>
    /// 防双发(builder check 结论):create_maiden prefix 返回 false 跳过原版
    /// CreateMaiden() → 其内部对 CreateMaidenOfFaction(null) 的调用不会发生;两
    /// prefix 共用的复刻体 CreateMaidenBody 内联完整生成逻辑,绝不回调主 mod
    /// CreateMaidenOfFaction(否则经新 prefix 二次进入 = 事件双发 + 接管重跑);
    /// 陷阱路径(TrapPatch 复刻体经委托调用主 mod CreateMaidenOfFaction)恰好经
    /// 新 prefix 进入共用体一次 = 配置生效 + 事件单发。
    /// 写入通道(R3 两级合法通道):initonly(static readonly)字段的
    /// <c>ModReflect.SetStatic</c> 直接抛 <c>ModReflectAccessException
    /// ("... is read-only")</c>(反编译 ModReflect.WriteMember 实证),
    /// R2 曾引入 Harmony 的反射原语字段 setter 置换引用(⓪级)——12/10
    /// 扫描器按「反射原语访问引擎内部」判定 reflection-external,**mod 激活
    /// 被直接拒绝**,已全数回滚(合法通道只有 ModReflect,反射原语 API
    /// 零残留)。现行阶梯,每级写后逐元素回读校验,失败降级:
    /// ① SetStatic 引用置换(CSV 原样长度;主 mod 未来版本去 readonly 则
    /// 在此级自动生效)→ ② GetStatic 取活引用 + 数组元素等长写(字段引用
    /// 只读但数组内容可变;目标长度 = 原数组长度,CSV N 项 → 元素[i] =
    /// csv[i % N] 循环重复填充——池是均匀抽签,重复填充 = 只出这些项,
    /// 语义无损;CSV 项数 &gt; 原长 → 截断取前 原长 项并记日志说明,完整
    /// 长度经消费方复刻体注册表直读生效)→ 全败聚合记日志。
    /// WriteJobsAtCompose / WriteFbbAtCompose 每次执行后必打一行聚合摘要
    /// (各池项数 + 级别[SetStatic/等长重复覆写]/一致/失败),零日志盲区不再有
    /// (pools.maiden 的 WriteExpAtCompose 已随 T9 删除)。
    /// 与原版的行为差异(sanctioned,详见 task-9-report.md;0.7.0 T8 拆段后
    /// RNG 语义回归原版):
    ///  1) 配置读取集中在任何状态写入之前(Phase A);读失败(键缺失/类型不符/CSV
    ///     格式错)→ 记日志放行原方法;复刻体(Phase B)异常 → 物品已半途创建,
    ///     不能放行原版重跑(会产出第二个少女),置 null 记日志(同 Task 7 Create 先例);
    ///  2) RNG:0.6.0 复刻体曾经 MaidenRoll.RollAttr 用单次 value 档内小数位取值
    ///     (分布等价、流消耗 3 次/少女);T8 拆段后属性三掷直调主 mod RollAttr
    ///     ——RNG 消耗次序/次数与原版逐指令一致(每属性 value 选档 + Range 取值
    ///     两次),MaidenRoll 纯函数仍服务 IPH 分区 postfix 与 transpile getter;
    ///  3) 无键常量 as-is 复刻:名字/描述/flavor 文案、"MAIDEN_LOWER"(下层区)/
    ///     "MAIDEN_YOUNG"(少女·萝莉)/"MAIDEN_BIG_BREAST"(巨乳)三条件 enable、
    ///     NativeFactionTag/JobTagOf("MP_JOB_" 前缀)、1.9.2 新增 6 属性 tag
    ///     清零/置一(情感/信任/催眠/烙印 0,交互当前/上限 1)、六个计数 tag 清零、
    ///     回退形状 2×5、"portrait_" 立绘导出名、SpriteFor→InjectSprite→
    ///     PickAnyPortraitSource→InjectPortraitTrimmed 回退链、onLoaded→
    ///     EnsureMaidenSprite 均逐行保留(委托回原实现,零漂移);
    ///     "lewd_meme" 目录字面量改 MainModResolver.MainModId。
    /// 主 mod 类型(ItemFactory/MaidenTraits/MaidenIdentity/SpriteInjector/NameGen/
    /// TentacleBody/ModCtx)一律运行时经 ModReflect 解析成委托/字段访问,不编译引用
    /// MidnightPawn.*;解析只发生在装配委托内(装配 = 物品目录初始化后,
    /// 本 mod 已被加载器登记,ModReflect 合法),绝不进静态构造,两阶段提交(任一
    /// 成员解析失败 → 原样上抛不提交,三条目一致不装)。无协程/Invoke 调用面,
    /// 不涉及 12/10 并发通道替换。
    /// </summary>
    public static class MaidenPatch
    {
        // -- 池默认 CSV = 主 mod v1.3.8 MaidenTraits 四数组逐字(1.3.8→1.6.1 零漂移;
        //    0.7.0 T9:EXP_NEED 经验表删除——1.9.2 已删该静态数组,经验公式化进
        //    ExpNeedForLevel,见 LevelCapPatch 的 main.exp_formula 单元)--
        internal const string FactionsDefault = "下层区,上层区,治安部,黑市,革命军";
        internal const string JobsDefault =
            "拾荒者,矿工,雇佣兵,工程师,护士,医生,厨师,农场主,小偷,商人,清洁员,技师,文员,走私者,货运员,酒保,维修工,研究员,学徒,艺人";
        internal const string BodiesDefault = "萝莉,少女,御姐,少妇,熟女";
        internal const string BreastsDefault = "贫乳,普乳,巨乳";

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用) --
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type itemFactory;                              // ItemFactory(方法/补丁目标)
        private static System.Type maidenTraits;                              // MaidenTraits(池字段)
        private static Action<GameItem, string> ifEnable;                     // ItemFactory.Enable(private)
        private static Action<GameItem, string, string> ifSetStr;             // ItemFactory.SetStr
        private static Action<GameItem, string, int> ifSetInt;                // ItemFactory.SetInt
        private static Func<GameItem, string, string> ifGetStr;               // ItemFactory.GetStr(事件快照读)
        private static Func<string, string> ifJobTagOf;                       // ItemFactory.JobTagOf
        private static Func<int> ifRollAttr;                                  // ItemFactory.RollAttr(1.9.2 拆出,T8 直调联动 transpiler)
        private static Func<string, int> ifFactionBase;                       // ItemFactory.FactionBase(同上)
        private static Func<int, int> ifAttrValue;                            // ItemFactory.AttrValue(60/80 断点曲线,固定常量不做键)
        private static Action<GameItem> ifUpdateMaidenDescription;            // ItemFactory.UpdateMaidenDescription
        private static Action<GameItem, string> ifLogShape;                   // ItemFactory.LogShape
        private static Func<int, int, GridShape> ifRectShape;                 // ItemFactory.RectShape
        private static Func<string> randomName;                               // NameGen.RandomName
        private static MethodBase nameParts;                                  // 0.8.0:NameGen.RandomNameParts(out, out)(2.1.0 姓/名拆分)
        private static MethodBase consumeCapture;                             // 0.8.0:NpcExtras.ConsumePendingCapture(out, out)
        private static Action<GameItem, string, string> writeCapture;         // 0.8.0:NpcExtras.WriteCapture
        private static Action<GameItem> panelOpen;                            // 0.8.0:MaidenPanel.Open(双击面板)
        private static Func<string, string> identityOfFaction;                // 0.8.1:ItemFactory.IdentityOfFaction(2.3.1 身份字段)
        private static Func<string[], string> traitsPick;                     // MaidenTraits.Pick
        private static Func<string, string> nativeFactionTag;                 // MaidenIdentity.NativeFactionTag
        private static Func<string, string, string> spriteFor;                // MaidenIdentity.SpriteFor
        private static Func<string, int, int, string> injectSprite;           // MaidenIdentity.InjectSprite
        private static Func<string> pickAnyPortraitSource;                    // SpriteInjector.PickAnyPortraitSource
        private static Func<string, int, int, string> injectPortraitTrimmed;  // SpriteInjector.InjectPortraitTrimmed
        private static Action<string, string> exportInjected;                 // SpriteInjector.ExportInjected
        private static Func<GameItem, bool> ensureMaidenSprite;               // SpriteInjector.EnsureMaidenSprite
        private static Action<object> mainLog;                                // ModCtx.Log
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning

        /// <summary>把四条单元的装配委托登记进映射(catalog-id → install;两条
        /// 池段 prefix + 两条数值 transpiler,均属 maiden 家族)。
        /// pools.maiden_jobs / pools.maiden_fbb 是 DataWrite 载体,不进映射也不进
        /// HookInstaller(R7/R9):装配期一次写入走 WriteJobsAtCompose /
        /// WriteFbbAtCompose,Changed 重写经 HookRuntime
        /// 分发器按两单元各自 route 守卫后转发 OnConfigChanged。Harmony 实例由
        /// 安装器转交。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            // 0.5.1 作者裁定:物品立绘管理器(item_portrait_hd)在场时,少女生成
            // 接管让位——它的工厂 transpiler(体型/乳量/选图/高清注入挂钩)只存在于
            // 原方法体,本 mod 的接管 prefix(return false)会短路原方法即打断
            // 其挂钩链(立绘回退原版/错乱,工房「少女当铺」同开时实测复现)。
            // 让位代价(0.7.0 T8 拆段后收窄):立绘尺寸/池抽取接管不生效;属性
            // 分布与阵营基价经 maiden_roll/maiden_faction_base transpiler 照常
            // 生效(Own 共存);池写(DataWrite,不挂方法)照常——经其
            // PickBody(original) 参数继续生效。
            // 0.6.0 起让位裁决由 Router 统一裁定(IPH 在场 → 两池段单元 Yield,
            // 不装配);下方分支退化为双保险:Router 拦截后不再可达,保留无害。
            // 数值两 transpiler 单元在让位分支之外恒登记(Own 共存,R10 同构;
            // 若误入让位分支,② 完备性守卫会立即警告且永不装配)。
            if (IphCompat.Active)
            {
                LogMain("物品立绘管理器在场,少女生成接管让位(原方法由其挂钩链全程接管;"
                    + "立绘尺寸/池抽取配置不生效;属性分布/阵营基价经数值注入照常生效,"
                    + "池类 CSV 照常写入)。");
            }
            else
            {
                installs.Add("main.create_maiden", h =>
                {
                    EnsureResolved();
                    MethodBase original = ModReflect.Target(itemFactory, "CreateMaiden", Type.EmptyTypes);
                    h.Patch(original, prefix: new HarmonyMethod(CreateMaidenPrefix));
                    return new[] { original };
                });
                installs.Add("main.create_maiden_of_faction", h =>
                {
                    EnsureResolved();
                    MethodBase original = ModReflect.Target(itemFactory, "CreateMaidenOfFaction", new[] { typeof(string) });
                    h.Patch(original, prefix: new HarmonyMethod(CreateMaidenOfFactionPrefix));
                    return new[] { original };
                });
            }
            // 0.7.0 T8:数值段两 transpiler 单元(1.9.2 拆出的独立方法;与
            // TranspilePlan E 节 methodId 同名,Carrier.Run 的让位过滤与登记期
            // 同源;IPH 在场仍 Own 共存——工厂 transpiler 零触碰两方法)。
            installs.Add("main.maiden_roll", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "RollAttr", Type.EmptyTypes);
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
            installs.Add("main.maiden_faction_base", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "FactionBase", new[] { typeof(string) });
                TranspileCarrier.PatchTranspiler(h, original);
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(四条单元共享;任一成员解析抛出 → 本方法原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 接管与池写入一致不装,杜绝半解析
        /// 状态下装出运行时 NRE 的 prefix——两阶段提交,同 Task 7/8)。失败由
        /// HookInstaller 逐单元隔离记 Dead(池写则由 WriteJobsAtCompose /
        /// WriteFbbAtCompose 记日志)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type factory = FindMainType("ItemFactory");
            System.Type traits = FindMainType("MaidenTraits");
            System.Type identity = FindMainType("MaidenIdentity");
            System.Type sprites = FindMainType("SpriteInjector");
            System.Type names = FindMainType("NameGen");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedEnable = ModReflect.StaticMethod<Action<GameItem, string>>(factory, "Enable");
            var resolvedSetStr = ModReflect.StaticMethod<Action<GameItem, string, string>>(factory, "SetStr");
            var resolvedSetInt = ModReflect.StaticMethod<Action<GameItem, string, int>>(factory, "SetInt");
            var resolvedGetStr = ModReflect.StaticMethod<Func<GameItem, string, string>>(factory, "GetStr");
            var resolvedJobTag = ModReflect.StaticMethod<Func<string, string>>(factory, "JobTagOf");
            var resolvedRollAttr = ModReflect.StaticMethod<Func<int>>(factory, "RollAttr");
            var resolvedFactionBase = ModReflect.StaticMethod<Func<string, int>>(factory, "FactionBase");
            var resolvedAttrValue = ModReflect.StaticMethod<Func<int, int>>(factory, "AttrValue");
            var resolvedUpdate = ModReflect.StaticMethod<Action<GameItem>>(factory, "UpdateMaidenDescription");
            var resolvedLogShape = ModReflect.StaticMethod<Action<GameItem, string>>(factory, "LogShape");
            var resolvedRect = ModReflect.StaticMethod<Func<int, int, GridShape>>(factory, "RectShape");
            var resolvedRandomName = ModReflect.StaticMethod<Func<string>>(names, "RandomName");
            // 0.8.0:2.1.0 新成员(out 双参反射 Invoke;NpcExtras/MaidenPanel 为
            // 1.9.3/1.11.0 新类型,绑定 2.1.0 下必在,解析失败 = Dead)
            System.Type npcExtras = FindMainType("NpcExtras");
            System.Type maidenPanel = FindMainType("MaidenPanel");
            MethodBase resolvedNameParts = ModReflect.Target(names, "RandomNameParts",
                new[] { typeof(string).MakeByRefType(), typeof(string).MakeByRefType() });
            MethodBase resolvedConsumeCapture = ModReflect.Target(npcExtras, "ConsumePendingCapture",
                new[] { typeof(string).MakeByRefType(), typeof(string).MakeByRefType() });
            var resolvedWriteCapture = ModReflect.StaticMethod<Action<GameItem, string, string>>(npcExtras, "WriteCapture");
            var resolvedPanelOpen = ModReflect.StaticMethod<Action<GameItem>>(maidenPanel, "Open");
            // 0.8.1:2.3.1 立绘名字修复——CreateMaidenOfFaction 新增按势力派生
            // 身份字段(private static,ModReflect 合法);MAIDEN_JOB 的
            // JobOfFaction 双写后被随机职业覆盖(终值与复刻体现状一致,不复制)
            var resolvedIdentityOf = ModReflect.StaticMethod<Func<string, string>>(factory, "IdentityOfFaction");
            var resolvedPick = ModReflect.StaticMethod<Func<string[], string>>(traits, "Pick");
            var resolvedNativeTag = ModReflect.StaticMethod<Func<string, string>>(identity, "NativeFactionTag");
            var resolvedSpriteFor = ModReflect.StaticMethod<Func<string, string, string>>(identity, "SpriteFor");
            var resolvedInject = ModReflect.StaticMethod<Func<string, int, int, string>>(identity, "InjectSprite");
            var resolvedPickAny = ModReflect.StaticMethod<Func<string>>(sprites, "PickAnyPortraitSource");
            var resolvedTrim = ModReflect.StaticMethod<Func<string, int, int, string>>(sprites, "InjectPortraitTrimmed");
            var resolvedExport = ModReflect.StaticMethod<Action<string, string>>(sprites, "ExportInjected");
            var resolvedEnsure = ModReflect.StaticMethod<Func<GameItem, bool>>(sprites, "EnsureMaidenSprite");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            itemFactory = factory;
            maidenTraits = traits;
            ifEnable = resolvedEnable;
            ifSetStr = resolvedSetStr;
            ifSetInt = resolvedSetInt;
            ifGetStr = resolvedGetStr;
            ifJobTagOf = resolvedJobTag;
            ifRollAttr = resolvedRollAttr;
            ifFactionBase = resolvedFactionBase;
            ifAttrValue = resolvedAttrValue;
            ifUpdateMaidenDescription = resolvedUpdate;
            ifLogShape = resolvedLogShape;
            ifRectShape = resolvedRect;
            randomName = resolvedRandomName;
            nameParts = resolvedNameParts;
            consumeCapture = resolvedConsumeCapture;
            writeCapture = resolvedWriteCapture;
            panelOpen = resolvedPanelOpen;
            identityOfFaction = resolvedIdentityOf;
            traitsPick = resolvedPick;
            nativeFactionTag = resolvedNativeTag;
            spriteFor = resolvedSpriteFor;
            injectSprite = resolvedInject;
            pickAnyPortraitSource = resolvedPickAny;
            injectPortraitTrimmed = resolvedTrim;
            exportInjected = resolvedExport;
            ensureMaidenSprite = resolvedEnsure;
            mainLog = resolvedLog;
            mainLogWarning = resolvedLogWarning;
            resolved = true;
        }

        /// <summary>在前置 lewd_meme 暴露的类型里按名查找(同名不同命名空间亦命中)。</summary>
        private static System.Type FindMainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("主 mod lewd_meme 未暴露类型 " + name);
        }

        // ------------------------------------------------------------------
        // CreateMaiden / CreateMaidenOfFaction:少女工厂接管(两 prefix 共用复刻体)
        // ------------------------------------------------------------------

        /// <summary>
        /// CreateMaiden 配置快照(Phase A 一次读全;任何键读取/解析失败 → 放行原方法,
        /// 此时尚未写入任何游戏状态)。0.7.0 T8 拆段后仅剩立绘尺寸——attr 三档/
        /// 区间与阵营基价已迁 main.maiden_roll / main.maiden_faction_base
        /// transpiler 单元(复刻体经委托直调主 mod 方法联动),处女掷骰段随
        /// 1.9.2 无条件 Enable 语义删除(maiden.virgin_chance 键废弃)。
        /// </summary>
        private sealed class MaidenCfg
        {
            public int PortraitSize;                            // 立绘尺寸(64)

            public static MaidenCfg Read()
            {
                return new MaidenCfg
                {
                    PortraitSize = Cfg(CfgKeys.PortraitSize, 64),
                };
            }
        }

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool CreateMaidenPrefix(ref GameItem __result)
        {
            MaidenCfg c;
            try
            {
                c = MaidenCfg.Read();
            }
            catch (Exception ex)
            {
                LogMainWarning("少女生成配置读取失败,放行原版: " + ex.Message);
                return true; // 尚未写入任何状态,原版重跑安全
            }
            try
            {
                __result = CreateMaidenBody(null, c);
            }
            catch (Exception ex)
            {
                // 物品已半途创建,不能放行原版重跑(会产出第二个少女);置 null 并记日志。
                LogMainWarning("少女创建失败(接管复刻): " + ex.Message);
                __result = null;
            }
            return false;
        }

        /// <summary>陷阱直调路径的接管(1.6.1 CreateMaidenOfFaction(string)):
        /// 配置键与 Phase A/B/null 结果模式同 create_maiden;faction 非空直用,
        /// null/空走池(对齐 1.6.1 L66-69)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool CreateMaidenOfFactionPrefix(string faction, ref GameItem __result)
        {
            MaidenCfg c;
            try
            {
                c = MaidenCfg.Read();
            }
            catch (Exception ex)
            {
                LogMainWarning("少女生成配置读取失败,放行原版: " + ex.Message);
                return true; // 尚未写入任何状态,原版重跑安全
            }
            try
            {
                __result = CreateMaidenBody(faction, c);
            }
            catch (Exception ex)
            {
                // 物品已半途创建,不能放行原版重跑(会产出第二个少女);置 null 并记日志。
                LogMainWarning("少女创建失败(接管复刻): " + ex.Message);
                __result = null;
            }
            return false;
        }

        /// <summary>逐行复刻 v1.9.2 CreateMaidenOfFaction(= v1.3.8 CreateMaiden 的
        /// faction 参数化版):faction 为 null/空 → Pick(FACTIONS) 走池,非空直用
        /// (1.9.2 L93-96);差异仅立绘尺寸 maiden.portrait_size(原版 64)——属性
        /// 三掷/阵营基价/属性价格经委托直调主 mod RollAttr/FactionBase/AttrValue
        /// (T8 拆段:数值键经 transpiler 单元独立生效),其余文案与流程 as-is
        /// (1.9.2 新行为:无条件处女 Enable + 6 新属性 SetInt)。</summary>
        private static GameItem CreateMaidenBody(string faction, MaidenCfg c)
        {
            GameItem val = ItemDirectory.CreateEmptyItem("maiden_lower");
            val.name = "少女";
            val.shortDescription = "被塞进束缚囊的女孩。还活着，还会挣扎。";
            val.SetGameItemType("CONTRABAND");
            // 0.8.0:2.1.0 起姓名三字段同源(RandomNameParts out 双参——2.0.2
            // 修复「三字段对不上」;姓/名一定来自同一地区姓名库)
            object[] np = new object[2];
            string text = (string)nameParts.Invoke(null, np);
            string surname = (string)np[0];
            string given = (string)np[1];
            if (string.IsNullOrEmpty(faction))
            {
                faction = traitsPick(PoolField("FACTIONS"));
            }
            string text3 = traitsPick(PoolField("JOBS"));
            // 0.5.0 IPH 兼容(spec §4.1 B):物品立绘管理器在场时体型先出、
            // 经其 AdultBodySources 抽取(权重/偏好/成人源生效;original 参数 =
            // 主 mod 池数组,本 mod 池写继续生效);乳量按体型(PickBreastForBody)。
            // 缺席/解析失败时保持原版顺序(先乳量后体型,RNG 流零漂移)。
            // T8 注:IPH 在场时本复刻体经 Router R4 让位不装配,此分支为双保险。
            string text4, text5;
            if (IphCompat.Active)
            {
                text5 = IphCompat.PickBody(PoolField("BODIES"));
                text4 = IphCompat.PickBreastForBody(PoolField("BREASTS"), text5);
            }
            else
            {
                text4 = traitsPick(PoolField("BREASTS"));
                text5 = traitsPick(PoolField("BODIES"));
            }
            ifSetStr(val, "MAIDEN_NAME", text);
            ifSetStr(val, "MAIDEN_SURNAME", surname);   // 0.8.0:2.1.0 姓字段
            ifSetStr(val, "MAIDEN_GIVENNAME", given);   // 0.8.0:2.1.0 名字段
            ifSetInt(val, "MAIDEN_NAME_MODE", 1);       // 0.8.0:2.1.0 名称显示模式(1=姓名)
            ifSetStr(val, "MAIDEN_FACTION", faction);
            ifSetStr(val, "MAIDEN_IDENTITY", identityOfFaction(faction)); // 0.8.1:2.3.1 按势力派生身份(默认拾荒客(普通)等)
            ifSetStr(val, "MAIDEN_JOB", text3);
            ifSetStr(val, "MAIDEN_BREAST", text4);
            ifSetStr(val, "MAIDEN_BODY", text5);
            val.name = faction + text5;
            val.flavorText = text + " · " + faction + text3;
            // T8 拆段:属性三掷直调主 mod RollAttr(1.9.2 L107-109 的 call 等价;
            // maiden_roll transpiler 挂其上 → 开关开 = 面板键分布,关 = 原版字面量;
            // RNG 消耗次序回到原版每属性两次)
            int num = ifRollAttr();
            int num2 = ifRollAttr();
            int num3 = ifRollAttr();
            ifSetInt(val, "MAIDEN_FORCE", num);
            ifSetInt(val, "MAIDEN_INTEL", num2);
            ifSetInt(val, "MAIDEN_CHARM", num3);
            // 阵营基价/属性价格曲线同经委托直调主 mod(1.9.2 L113 的
            // FactionBase(faction) + AttrValue×3;maiden_faction_base transpiler
            // 挂 FactionBase;AttrValue 的 60/80 断点与 5/10 斜率为固定常量不做键)
            val.unitValue = ifFactionBase(faction)
                + ifAttrValue(num)
                + ifAttrValue(num2)
                + ifAttrValue(num3);
            ifEnable(val, "MAIDEN_FEMALE");
            if (faction == "下层区")
            {
                ifEnable(val, "MAIDEN_LOWER");
            }
            // 1.9.2 行为对齐(L119-120):处女双 tag 无条件 Enable(原 0.6 掷骰段
            // 随 maiden.virgin_chance 键废弃删除;bool 键 maiden.initial_virgin
            // 由 Task 9 另建)
            ifEnable(val, "MAIDEN_VIRGIN");
            ifEnable(val, "MAIDEN_ANAL_VIRGIN");
            if (text5 == "少女" || text5 == "萝莉")
            {
                ifEnable(val, "MAIDEN_YOUNG");
            }
            if (text4 == "巨乳")
            {
                ifEnable(val, "MAIDEN_BIG_BREAST");
            }
            ifEnable(val, nativeFactionTag(faction));
            ifEnable(val, "MP_JOB_" + ifJobTagOf(text3));
            ifSetInt(val, "TRAIN_VALUE", 0);
            // 1.9.2 新增 6 属性(L131-137,紧随 TRAIN_VALUE):情感/信任/催眠清零,
            // 交互当前/上限置一,烙印清零
            ifSetInt(val, "MAIDEN_AFFECTION", 0);
            ifSetInt(val, "MAIDEN_TRUST", 0);
            ifSetInt(val, "MAIDEN_HYPNOSIS", 0);
            ifSetInt(val, "MAIDEN_INTERACT_CUR", 1);
            ifSetInt(val, "MAIDEN_INTERACT_MAX", 1);
            ifSetInt(val, "MAIDEN_BRAND", 0);
            ifSetInt(val, "ORAL_COUNT", 0);
            ifSetInt(val, "SEX_COUNT", 0);
            ifSetInt(val, "ANAL_COUNT", 0);
            ifSetInt(val, "LACTATION_LV", 0);
            string text6 = spriteFor(faction, text3);
            // 0.5.0 IPH 兼容:它在场时注入走 InjectPortraitTrimmed——其方法级
            // InjectPrefix 对任何调用方生效(高清发布,无 64px 中间产物);
            // 缺席时保持原版 InjectSprite(两函数实现不等价,反编译已核对,
            // 故本替换仅在 iphCompat 分支)。
            string text7 = IphCompat.Active
                ? injectPortraitTrimmed(text6, c.PortraitSize, c.PortraitSize)
                : injectSprite(text6, c.PortraitSize, c.PortraitSize);
            string text8 = text6;
            if (text7 == null)
            {
                text8 = pickAnyPortraitSource();
                text7 = injectPortraitTrimmed(text8, c.PortraitSize, c.PortraitSize);
            }
            if (text7 != null)
            {
                val.SetSpriteAndShapeFromMod(MainModResolver.MainModId, text7);
                ifSetStr(val, "MAIDEN_SPRITE_SRC", text8);
                LogMain("少女立绘注入: " + text8 + " -> " + text7);
                exportInjected(text7, "portrait_" + text8 + ".png");
            }
            else
            {
                LogMainWarning("少女立绘注入失败");
                val.SetShape(ifRectShape(2, 5));
            }
            val.onLoaded = delegate (GameItem it)
            {
                ensureMaidenSprite(it);
            };
            // 0.8.0:2.1.0 新增段(1.11.0 NPC 机制)——捕捉记录(垃圾场拾荒/陷阱
            // 捕获时 SetPendingCapture 挂起,此处消费写进少女)与双击苗床面板
            // (canActivate/mayActivate 恒放行,onActivate 开面板;委托经
            // GameItemFunc 游戏类型直设)
            object[] cap = new object[2];
            if ((bool)consumeCapture.Invoke(null, cap))
            {
                writeCapture(val, (string)cap[0], (string)cap[1]);
            }
            GameItemFunc func = (GameItemFunc)val;
            func.canActivateSlotItemFunc = (GameItem _, GameInventory __, SlotMarker ___) => true;
            func.mayActivateSlotItemFunc = (GameItem _, GameInventory __, SlotMarker ___) => true;
            func.onActivateSlotItemFunc = delegate (GameItem it, GameInventory _, SlotMarker __)
            {
                panelOpen(it);
            };
            ifUpdateMaidenDescription(val);
            ifLogShape(val, "少女");
            return val;
        }

        /// <summary>池字段活引用读取(每次生成现读,池写入/重写即时生效)。
        /// R3-F3:优先直读注册表池键——开关开 = 玩家 CSV 拆数组(原样长度,不截断);
        /// 开关关/未注入 = Current 返回默认串 = 原版池逐字;注册表读抛/空 CSV 时
        /// 回退主 mod 静态池活引用(池写等长重复覆写生效后与之同步)。生成路径
        /// 由此不依赖静态写入成败——「开池开关 + 开生成接管」即生效。均匀抽签
        /// 语义:CSV N 项原样数组与静态等长重复填充的抽签分布一致(重复填充 =
        /// 只出这些项),CSV 项数 &gt; 原长时注册表侧不截断(静态侧截断,写入时
        /// 已记日志说明)。</summary>
        private static string[] PoolField(string fieldName)
        {
            var meta = PoolMeta(fieldName);
            if (meta != null)
            {
                try
                {
                    string csv = Cfg(meta.Value.Key, meta.Value.Def);
                    if (MaidenRoll.PoolValid(csv))
                    {
                        return ToArray(MaidenRoll.SplitCsv(csv));
                    }
                }
                catch (Exception)
                {
                    // 注册表缺席/键读失败 → 回退主 mod 静态池活引用
                }
            }
            return ModReflect.GetStatic<string[]>(maidenTraits, fieldName);
        }

        /// <summary>池字段名 → (注册表键, 默认 CSV = 原版池逐字);null = 无
        /// 对应键(四池全覆盖,防御分支回退静态)。</summary>
        private static (string Key, string Def)? PoolMeta(string fieldName)
        {
            switch (fieldName)
            {
                case "FACTIONS": return (CfgKeys.Factions, FactionsDefault);
                case "JOBS": return (CfgKeys.Jobs, JobsDefault);
                case "BODIES": return (CfgKeys.Bodies, BodiesDefault);
                case "BREASTS": return (CfgKeys.Breasts, BreastsDefault);
                default: return null;
            }
        }

        // ------------------------------------------------------------------
        // 池/EXP_NEED 静态字段写入(DataWrite 载体:装配期一次 + Changed 分发重写)
        // ------------------------------------------------------------------

        /// <summary>
        /// pools.maiden_jobs 装配期一次写入(HookRuntime 主 mod 在场且版本受支持
        /// 且 route Own 时调用;R9 拆分自 R7 的 WriteTraitsAtCompose,与 fbb 单元
        /// 独立守卫):JOBS 写入 MaidenTraits 静态数组;IPH 工厂 transpiler 全文
        /// 零触碰职业抽取(iph_1217 无 MaidenTraits.JOBS 引用),恒 Own 写入。
        /// 默认安装零写入(红线);失败聚合记日志不阻断装配。每次执行后聚合打
        /// 一行摘要(R2-F1)。
        /// </summary>
        internal static void WriteJobsAtCompose()
        {
            EnsureResolved();
            var failures = new List<string>();
            var results = new List<(string Name, string Desc, string Level)>();
            results.Add(PoolResult("jobs",
                WritePool(CfgKeys.Jobs, "JOBS", "少女职业池", JobsDefault, failures)));
            LogMain(FormatPoolSummary(results));
            if (failures.Count > 0)
            {
                LogMainWarning("maiden 职业池写入失败: " + string.Join("; ", failures.ToArray()));
            }
        }

        /// <summary>
        /// pools.maiden_fbb 装配期一次写入(R9 拆分:FACTIONS/BODIES/BREASTS 三池;
        /// IPH 在场时该单元 route Yield 不写——其工厂 transpiler 把三池 Pick 替换成
        /// 自家抽取,详见 Router R4 条)。默认安装零写入(红线);失败聚合记日志
        /// 不阻断装配,后续重写经 HookRuntime 分发器转发 OnConfigChanged。
        /// </summary>
        internal static void WriteFbbAtCompose()
        {
            EnsureResolved();
            var failures = new List<string>();
            var results = new List<(string Name, string Desc, string Level)>();
            results.Add(PoolResult("factions",
                WritePool(CfgKeys.Factions, "FACTIONS", "少女阵营池", FactionsDefault, failures)));
            results.Add(PoolResult("bodies",
                WritePool(CfgKeys.Bodies, "BODIES", "少女体型池", BodiesDefault, failures)));
            results.Add(PoolResult("breasts",
                WritePool(CfgKeys.Breasts, "BREASTS", "少女胸部池", BreastsDefault, failures)));
            LogMain(FormatPoolSummary(results));
            if (failures.Count > 0)
            {
                LogMainWarning("maiden 三池(fbb)写入 " + failures.Count + "/3 失败: "
                    + string.Join("; ", failures.ToArray()));
            }
        }

        /// <summary>单池写入结果 → 聚合行条目(desc = 摘要,level = 写入级别,
        /// null = 未发生真实写入:一致/空跳过/解析失败/失败)。</summary>
        private static (string Name, string Desc, string Level) PoolResult(
            string name, (string Desc, string Level) r)
        {
            return (name, r.Desc, r.Level);
        }

        /// <summary>聚合摘要行:「池写入: factions=1 项/jobs=一致/…(级别:等长重复覆写)」;
        /// 无任何真实写入时尾部记「(零写入)」。</summary>
        private static string FormatPoolSummary(List<(string Name, string Desc, string Level)> results)
        {
            var parts = new List<string>();
            var levels = new List<string>();
            foreach (var r in results)
            {
                parts.Add(r.Name + "=" + r.Desc);
                if (r.Level != null && !levels.Contains(r.Level)) levels.Add(r.Level);
            }
            string tail = levels.Count > 0
                ? "(级别:" + string.Join("·", levels.ToArray()) + ")"
                : "(零写入)";
            return "池写入: " + string.Join("/", parts.ToArray()) + " " + tail;
        }

        /// <summary>
        /// 单池写入:PoolValid 守卫(空/空白 → 跳过保持原版)→ 等长重复填充后
        /// 与现值逐项相同 → 跳过(默认安装零写入)→ 两级合法通道(SetStatic →
        /// 等长重复覆写,见 TryWritePoolField)。所有出口均记日志,真实失败聚合
        /// 进 failures;返回 (摘要, 级别) 供聚合行。
        /// </summary>
        private static (string Desc, string Level) WritePool(string key, string fieldName,
            string label, string def, List<string> failures)
        {
            string csv = Cfg(key, def);
            if (!MaidenRoll.PoolValid(csv))
            {
                LogMainWarning(label + "配置为空,跳过写入保持原版: " + fieldName);
                return ("空跳过", null);
            }
            string[] items = ToArray(MaidenRoll.SplitCsv(csv));
            string[] current = ModReflect.GetStatic<string[]>(maidenTraits, fieldName);
            string[] filled = current != null ? FillCycled(items, current.Length) : items;
            if (current != null && items.Length > current.Length)
            {
                LogCycledTruncate(label, fieldName, items.Length, current.Length);
            }
            if (Same(current, filled))
            {
                LogMain(label + "与原版一致,跳过写入: " + fieldName);
                return ("一致", null);
            }
            string level = TryWritePoolField(maidenTraits, fieldName, label, items, filled, current, failures);
            return (level != null ? filled.Length + " 项" : "失败", level);
        }

        /// <summary>
        /// 两级写入阶梯(R3):① ModReflect.SetStatic 引用置换(CSV 原样长度;
        /// 主 mod 现行四池/EXP_NEED 均 static readonly → 此级恒抛降级,留作
        /// 未来版本去 readonly 后的自动升级通道)→ ② GetStatic 取活引用 +
        /// 数组元素等长写(filled = CSV 循环重复填充到原数组长度);全败聚合进
        /// failures 并记日志。每级写后逐元素回读校验(VerifyWritten),失败降级。
        /// 返回成功级别名,全败返回 null。
        /// </summary>
        private static string TryWritePoolField(System.Type type, string fieldName, string label,
            Array items, Array filled, Array current, List<string> failures)
        {
            // ① SetStatic(主 mod 未来版本改非 readonly 则在此级成功)
            try
            {
                ModReflect.SetStatic(type, fieldName, items);
                if (VerifyWritten(type, fieldName, items))
                {
                    LogMain(label + "已写入(SetStatic 引用置换 " + items.Length + " 项): " + fieldName);
                    return "SetStatic";
                }
            }
            catch (ModReflectAccessException)
            {
                // 反编译实证:ModReflect.WriteMember 对 initonly 字段抛 "... is read-only"
                // (MaidenTraits 四池 public static readonly / EXP_NEED private
                // static readonly)→ 降级就地覆写(常规路径)。
            }
            catch (Exception ex)
            {
                failures.Add(fieldName + ": " + ex.Message);
                LogMainWarning(label + "写入失败: " + ex.Message);
                return null;
            }
            // ② GetStatic 活引用等长重复覆写:字段引用只读但数组内容可变;
            // 写入长度 = 原数组长度(filled 已按 csv[i % N] 循环填充,语义见类注释)
            if (current == null)
            {
                failures.Add(fieldName + ": 字段现值为 null 且 SetStatic 不可用,无合法写入通道");
                LogMainWarning(label + "字段现值为 null 且 SetStatic 不可用,保持原版: " + fieldName);
                return null;
            }
            Array.Copy(filled, current, current.Length);
            if (VerifyWritten(type, fieldName, filled))
            {
                LogMain(label + "已就地重复覆写 " + current.Length + " 项(等长,CSV "
                    + items.Length + " 项循环填充): " + fieldName);
                return "等长重复覆写";
            }
            failures.Add(fieldName + ": 就地覆写后回读校验失败");
            LogMainWarning(label + "就地覆写回读校验失败,保持原版: " + fieldName);
            return null;
        }

        /// <summary>CSV 项循环填充到 length 长度:元素[i] = items[i % items.Length]
        /// (池是均匀抽签,「黑市」1 项 → 全 5 格黑市 = 只出黑市,语义无损);
        /// items.Length &gt; length → 截断取前 length 项(调用方已记日志)。</summary>
        private static T[] FillCycled<T>(T[] items, int length)
        {
            var filled = new T[length];
            for (int i = 0; i < length; i++)
            {
                filled[i] = items[i % items.Length];
            }
            return filled;
        }

        /// <summary>CSV 项数 &gt; 原数组长度时的截断说明(等长通道只写前 原长 项;
        /// 消费方复刻体注册表直读不受此限)。</summary>
        private static void LogCycledTruncate(string label, string fieldName, int configured, int original)
        {
            LogMainWarning(label + "配置 " + configured + " 项 > 原版 " + original
                + " 项,静态写入只取前 " + original + " 项(initonly 字段引用不可置换;"
                + "完整长度经消费方复刻体注册表直读生效): " + fieldName);
        }

        /// <summary>写后回读校验(R3 逐元素):GetStatic 取回字段现值,引用相等
        /// (SetStatic 置换成功)或逐元素一致(就地覆写成功)即通过——防假成功。</summary>
        private static bool VerifyWritten(System.Type type, string fieldName, Array target)
        {
            object back = ModReflect.GetStatic(type, fieldName);
            if (ReferenceEquals(back, target)) return true;
            var arr = back as Array;
            if (arr == null || target == null || arr.Length != target.Length) return false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (!Equals(arr.GetValue(i), target.GetValue(i))) return false;
            }
            return true;
        }

        // ------------------------------------------------------------------
        // Changed 重写处理器(HookRuntime 分发器转发;R10:订阅/退订归分发器,
        // 本类不再自持 registry 引用)
        // ------------------------------------------------------------------

        /// <summary>任一池键变更 → 只重写对应字段(异常吞掉记日志)。R9 拆分参数
        /// (0.7.0 T9:allowExp 已随 pools.maiden 删除——1.9.2 EXP_NEED 字段消失,
        /// 经验公式化):allowJobs = pools.maiden_jobs(JOBS)route
        /// Own(IPH 零触碰职业抽取,防御性参数恒 true 语义);allowFbb =
        /// pools.maiden_fbb(FACTIONS/BODIES/BREASTS)route Own(IPH 在场让位时
        /// false——其工厂 transpiler 替换三池抽取,静态写在默认路径无消费方,
        /// 玩家出口 = IPH 写透分区 body_weights_csv / body_affects_breast_size)。
        /// 两单元的分发由 HookRuntime 按各自 route 独立守卫后单次转发。</summary>
        internal static void OnConfigChanged(string key, bool allowJobs, bool allowFbb)
        {
            try
            {
                var failures = new List<string>();
                if (allowJobs && key == CfgKeys.Jobs)
                {
                    WritePool(key, "JOBS", "少女职业池", JobsDefault, failures);
                }
                if (allowFbb)
                {
                    if (key == CfgKeys.Factions) WritePool(key, "FACTIONS", "少女阵营池", FactionsDefault, failures);
                    else if (key == CfgKeys.Bodies) WritePool(key, "BODIES", "少女体型池", BodiesDefault, failures);
                    else if (key == CfgKeys.Breasts) WritePool(key, "BREASTS", "少女胸部池", BreastsDefault, failures);
                }
                if (allowJobs || allowFbb)
                {
                    // 0.5.0 池即时回滚:开关键变化 → 受影响池重写。CSV 来源 Cfg() =
                    // Current(开关已压)——任一层关 → 默认串 → 写回默认数组;全开且
                    // CSV≠默认 → 写玩家值。子项路由见 PoolRouting(T9 后四池,
                    // exp_per_level 已废弃)。
                    // R7:受影响池属让位单元(R9 让位侧 fbb)时跳过。
                    var pools = PoolRouting.PoolsAffected(key);
                    foreach (var pool in pools)
                    {
                        if (pool == CfgKeys.Jobs) { if (allowJobs) WritePoolOrExp(pool, failures); }
                        else if (allowFbb) WritePoolOrExp(pool, failures);
                    }
                }
                if (failures.Count > 0)
                {
                    LogMainWarning("maiden 池重写 " + failures.Count + " 项失败(" + key + "): "
                        + string.Join("; ", failures.ToArray()));
                }
            }
            catch (Exception ex)
            {
                LogMainWarning("maiden 池重写失败(" + key + "): " + ex.Message);
            }
        }

        /// <summary>池键 → 重写入(四池走 WritePool;exp_per_level 分支已随 T9 废弃)。</summary>
        private static void WritePoolOrExp(string poolKey, List<string> failures)
        {
            if (poolKey == CfgKeys.Factions) WritePool(poolKey, "FACTIONS", "少女阵营池", FactionsDefault, failures);
            else if (poolKey == CfgKeys.Jobs) WritePool(poolKey, "JOBS", "少女职业池", JobsDefault, failures);
            else if (poolKey == CfgKeys.Bodies) WritePool(poolKey, "BODIES", "少女体型池", BodiesDefault, failures);
            else if (poolKey == CfgKeys.Breasts) WritePool(poolKey, "BREASTS", "少女胸部池", BreastsDefault, failures);
        }

        // ------------------------------------------------------------------
        // 共用小件
        // ------------------------------------------------------------------

        private static string[] ToArray(IReadOnlyList<string> items)
        {
            var arr = new string[items.Count];
            for (int i = 0; i < items.Count; i++) arr[i] = items[i];
            return arr;
        }

        private static bool Same(string[] a, string[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }

        /// <summary>配置读取:注册表缺席(未注入/停用)时回退原版字面量默认值。</summary>
        private static T Cfg<T>(string key, T def)
        {
            return PatchCtx.Config != null ? PatchCtx.Config.Current<T>(key) : def;
        }

        /// <summary>日志走主 mod ModCtx(保持原版前缀与格式);委托缺席时退回 Debug.Log。</summary>
        private static void LogMain(object msg)
        {
            if (mainLog != null) mainLog(msg);
            else UnityEngine.Debug.Log("[lewd_meme_config→主mod] " + msg);
        }

        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else UnityEngine.Debug.LogWarning("[lewd_meme_config→主mod] " + msg);
        }

        /// <summary>面板 CSV 容错回退的日志去重(Task 6):同键每会话至多一条,
        /// 防止高频路径(每次生成少女)刷屏。</summary>
        private static readonly HashSet<string> cfgWarnedOnce = new HashSet<string>();
        internal static void LogCfgOnce(string key, string msg)
        {
            if (cfgWarnedOnce.Add(key)) LogMainWarning(msg);
        }
    }
}
