using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 少女生成属性·IPH 分区复刻(0.6.0 R10,目录单元 main.maiden_attrs_iph;
    /// MainHook 载体,TargetMod = <see cref="FamilyMods.Iph"/> ——本目录首个非
    /// lewd_meme 目标的 MainHook 单元)。用户裁定的形态:主 mod 侧 attr_* 五键
    /// (main.create_maiden 单元)在 IPH 在场时让位(Router R4,工厂 transpiler
    /// 挂钩链全程接管原方法体,prefix 短路会打断其立绘替换),属性分布因此失焦
    /// ——本补丁把同一组数值复刻进「物品高清立绘管理器联动」分区,以 <b>postfix</b>
    /// 形态与 IPH 共存,让玩家在 IPH 在场时仍有属性出口。
    /// 0.7.0 T8 注:主面板 attr 五键已拆往 main.maiden_roll transpiler(IPH
    /// 在场 Own 共存,「失焦」前提收窄为立绘/池段),本单元语义不变——
    /// 叠加态见红线 bullet 的 T8 注记。
    /// <list type="bullet">
    ///  <item>共存依据(让位原则的例外形态):IPH 的工厂补丁 = transpiler +
    ///    BeginFactory prefix + EndFactory postfix(iph_130 OriginalCompat.cs:63,
    ///    ResolveFactory 优先 CreateMaidenOfFaction[:102]);TransformFactory
    ///    (:229-338)的全部替换点——BODIES.Pick→PickBody(:282)/FACTIONS.Pick→
    ///    PickFaction(:289)/SpriteFor→PickPrimary|PickForItem(:299,:308)/
    ///    InjectSprite→InjectIdentity|InjectPortraitTrimmed(:314)/PickAnyPortrait
    ///    Source→PickFallback(:326)/BREASTS.Pick→PickBreastForBody(:320)——
    ///    集中在选图/阵营/体型/乳量段;属性掷骰段(RollAttr / MAIDEN_FORCE /
    ///    MAIDEN_INTEL / MAIDEN_CHARM / AttrValue / unitValue)在 iph_130 全文
    ///    零引用。postfix 不短路原方法(其挂钩链照常跑完)只在产物上重掷属性
    ///    tag,与 transpiler 的 IL 改写层正交,天然共存。</item>
    ///  <item>三态语义(Router ① 版本压制的既有通道,无特判):IPH 在场且版本
    ///    命中绑定表 → Own,开关开即装;IPH 版本漂移 → VersionBlocked 自动压制
    ///    (用户明确要求,防 IPH 未来接管属性段时与本补丁冲突,面板五键灰字
    ///    「版本不匹配,该项不生效」);IPH 缺席 → 分区整体不 Define(在场条件
    ///    Define)、开关恒读默认 false → 单元 Absent 零 hook——缺席时属性出口
    ///    回到主面板 maiden.attr_* 五键(main.create_maiden 单元照常 Own)。</item>
    ///  <item>postfix 语义:全默认(五键读回均 = 主 mod 1.7.1 字面量,含开关开
    ///    但值=默认)→ 零参与——不动产物、不消耗随机数,精确零漂移;任一键 ≠
    ///    默认 → 按三档概率(iph.attr_high/mid_chance,低档 = 补集)独立重掷
    ///    MAIDEN_FORCE / MAIDEN_INTEL / MAIDEN_CHARM 三 tag(区间读
    ///    iph.attr_{high,mid,low}_range CSV 键)。掷骰段复刻对照主 mod 1.7.1
    ///    ItemFactory.cs:81-87(三次 RollAttr() → SetInt 三 tag → unitValue =
    ///    FactionBase + AttrValue×3):三档划分与区间/取整/分布语义经
    ///    <see cref="MaidenRoll.RollAttr"/> 纯函数(累计阈值 + 档内取值,与
    ///    主面板 main.create_maiden 复刻体同一实现——sanctioned 差异:原版每
    ///    属性消耗两次随机数,纯函数单 roll,默认阈值下三档概率与档内分布均
    ///    与原版一致,见 MaidenPatch 类注释);重掷后经委托直调主 mod
    ///    UpdateMaidenDescription 刷新描述(签名 public static void
    ///    UpdateMaidenDescription(GameItem),lewd_meme_171 ItemFactory.cs:546
    ///    逐字符核对;委托直调先例 = TrapPatch)。unitValue:<b>remake 缺席时</b>
    ///    做<b>差量修正</b>(新三属性 AttrValue 之和 − 旧三属性之和,断点 60/80
    ///    与斜率 5/10 为原版固定常量,主面板先例「AttrPrice 的 60/80 不做键」)
    ///    ——只接管属性段,阵营基价部分保持原方法已算值,不引入 FactionBase
    ///    的第二事实源;<b>remake 在场时跳过差量</b>(身价归 remake 的既有
    ///    裁定,见下条 F1)。</item>
    ///  <item>F1(2026-11-14 复审,remake 同场差量错价):remake 在场时其
    ///    InitializeMaiden(remake_1521 PatchItemCreateContraband.cs:21 →
    ///    :29-45,Harmony postfix on DirectoryMaster.Item(string,bool),
    ///    identifier == "maiden_lower" 即重定)与 UpdateMaidenValue
    ///    (remake_1521 ItemFactory.cs:772-800:unitValue = max(1, 300 +
    ///    TRAIN_VALUE×2 + 处女 200 + 肛处女 50,再乘疲劳倍率)——<b>不含
    ///    属性</b>)重定少女生成尾部与交易/疲劳/夜间结算的身价
    ///    (MaidenBuyer.cs:521 / MaidenFatigue.cs:397,457,537 / remake
    ///    TentacleBody.cs:1051,1096,1159 / OriginalDeviceActivityPatch.cs:406);
    ///    主 mod 公式(FactionBase + AttrValue×3)的差量基准在 remake 语境下
    ///    不可靠——生成时刻若身价已是 remake 基数(300 系),差量会凭空扣减
    ///    ΣAttrPrice(旧)(约 −30~−180,且无 max(1) 钳制)。裁定:装配期快照
    ///    ModHelper.IsModRunning(FamilyMods.Remake)(install 期一次,会话内
    ///    静态,与 HookRuntime.family 快照同语义——绝不在每次生成的 postfix
    ///    里查)为真即跳过差量,只重掷属性 tag + 刷描述;身价语义整体归
    ///    remake 公式,与主面板 price_ovulation_shot 让位 remake 后经
    ///    remake.ovulation_price 接管的「数值归 owner」裁定同构。</item>
    ///  <item>R2(2026-11-14 复审第二轮,Critical 教训):old 三属性的
    ///    <c>ifGetInt</c> 读取点必须在 <c>ifSetInt</c>×3 之<b>前</b>——两者走
    ///    同一 TagState 存储,先写后读 = 读回新值,差量恒等 no-op(remake 缺席
    ///    + IPH 在场的主流组合下属性重掷后身价不随动)。现行结构:进入重掷
    ///    分支先无条件读 old 三值(remake 在场也读,一次 ModData 读开销换
    ///    读取点无条件正确),再 RollAttr/SetInt,再差量;Δ 经
    ///    <see cref="MaidenRoll.AttrDelta"/> 单一事实源(纯逻辑侧恒等性测试:
    ///    AttrPrice 分段斜率 1/5/10 恒正 → old≠new ⇒ Δ≠0),游戏侧经
    ///    <see cref="LastRerollTrace"/> 观察口断言「Δ 一致应用 + tag 一致 +
    ///    重掷已发生(tag 落测试并集)⇒ Δ≠0」(读点掩蔽回归的钉子;R3:第三条
    ///    判据用 tag 并集而非观察口 Old*==New*——读点回归恰把 Old* 抹成 New*,
    ///    观察口不可自证,SelfTestRunner.IphDeltaCheck)。</item>
    ///  <item>红线:主面板既有 attr_* 五键(main.create_maiden 单元)行为零改动
    ///    ——IPH 缺席照常生效、在场照常让位绿字;本单元五键独立(iph.attr_*,
    ///    不并入旧键的单元/标记);与旧单元 prefix 目标虽同为
    ///    CreateMaidenOfFaction,但两单元恒不同时装(IPH 在场 ⇔ 旧让位新可装;
    ///    IPH 缺席 ⇔ 新开关恒 false 不装),无叠写;处女概率不做(IPH 分区
    ///    scavenger_double_virgin_csv 已覆盖)、阵营/体型/乳量不做(R9 池键
    ///    让位治理)。0.7.0 T8 注:主面板 attr 五键已拆往 main.maiden_roll
    ///    transpiler(IPH 在场 Own 共存)——本 postfix 与其可同装,叠加态良性:
    ///    RollAttr 先按主面板键掷(transpiler 层),本 postfix 检查 iph 键 ≠
    ///    默认时重掷并恒胜(postfix 后置);两套键各属其面板分区,玩家自选,
    ///    单开任一各自生效,与「IPH 在场时 IPH 分区为属性出口」的 R10 语义
    ///    方向一致。</item>
    /// </list>
    /// 主 mod 类型(ItemFactory/ModCtx)一律运行时经 ModReflect 解析成委托,
    /// 不编译引用 MidnightPawn.*;解析只发生在装配委托内(装配 = 物品目录初始
    /// 化后,本 mod 已被加载器登记,ModReflect 合法),两阶段提交(任一成员
    /// 解析失败 → 原样上抛不提交,单元记 Dead,开关 off→on 可重试)。安装失败
    /// (主 mod 更新致签名漂移)由 HookInstaller 逐单元隔离。postfix 修正体异常
    /// → 吞掉记日志保留原属性(半重掷优于毁产物,ValuePostfixPatches 先例),
    /// 绝不回滚原方法已写入的状态。
    /// </summary>
    public static class MaidenAttrIphPatch
    {
        // 主 mod 原文掷骰段字面量(lewd_meme_171 ItemFactory.RollAttr :504-516):
        // value < 0.01f → Range(70,81) 即闭区间 [70,80];< 0.06f → [60,70];
        // 否则 [10,60](Random.Range(int,int) 上界排除,CSV 键值即闭区间两端)。
        internal const float HighChanceDefault = 0.01f;
        internal const float MidChanceDefault = 0.06f;
        internal const string HighRangeDefault = "70,80";
        internal const string MidRangeDefault = "60,70";
        internal const string LowRangeDefault = "10,60";

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,postfix 只做直接调用)--
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type itemFactory;                              // ItemFactory(补丁目标)
        private static Action<GameItem, string, int> ifSetInt;                // ItemFactory.SetInt(:342)
        private static Func<GameItem, string, int> ifGetInt;                  // ItemFactory.GetInt(:351)
        private static Action<GameItem> ifUpdateMaidenDescription;            // ItemFactory.UpdateMaidenDescription(:546,委托直调刷新描述)
        private static Action<object> mainLog;                                // ModCtx.Log
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning
        private static bool remakeActive;                                     // 装配期快照:remake 在场(F1 跳过身价差量;install 期一次,会话内静态)

        /// <summary>最近一次差量修正的记账(R2 观察口,HookRuntime R-W 先例:
        /// 自检执行器只读消费,不改变补丁语义)。remake 在场差量跳过时观察口为
        /// null(设计态标记)。F2-R2 防掩蔽:游戏侧拿不到差量前值(postfix 在
        /// 工厂返回前跑,SpawnItem 返回时已是终值),自检经此观察口断言「Δ 已
        /// 正确应用」与「重掷已发生(tag 落测试并集)⇒ Δ≠0」——读点掩蔽(先
        /// SetInt 后 GetInt)必然表现为 Δ≡0;R3:第三条判据用 tag 并集
        /// (rerolled,观察口外部事实——读点回归恰把 Old* 抹成 New*,观察口
        /// 自身的 old/new 比较会被抹平 = 循环豁免),纯逻辑侧 MaidenRoll.
        /// AttrDelta 测试锁公式。</summary>
        internal readonly struct AttrRerollTrace
        {
            public readonly long OldUnitValue;   // 差量前 unitValue(原方法已算值)
            public readonly int OldForce, OldIntel, OldCharm;
            public readonly int NewForce, NewIntel, NewCharm;
            public readonly long Delta;          // AttrDelta(old…, new…, 60, 80)

            internal AttrRerollTrace(long oldUnitValue,
                int oldForce, int oldIntel, int oldCharm,
                int newForce, int newIntel, int newCharm, long delta)
            {
                OldUnitValue = oldUnitValue;
                OldForce = oldForce; OldIntel = oldIntel; OldCharm = oldCharm;
                NewForce = newForce; NewIntel = newIntel; NewCharm = newCharm;
                Delta = delta;
            }
        }

        /// <summary>最近一次重掷的差量记账(可 null:未重掷/读配置失败/remake 差量跳过)。</summary>
        internal static AttrRerollTrace? LastRerollTrace => lastRerollTrace;
        private static AttrRerollTrace? lastRerollTrace;

        /// <summary>把接管单元的装配委托登记进映射(catalog-id → install)。
        /// 与 MaidenPatch.Register 不同,本委托<b>不经</b> IPH 让位早退——单元
        /// 恰以 IPH 在场为工作前提(三态治理在 Router/installer 层:TargetMod =
        /// IPH 的版本压制 + 面板条件 Define 的缺席 no-op),IPH 缺席时照常登记、
        /// 开关恒 false 对账 Absent,零 hook。Harmony 实例由安装器转交。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.maiden_attrs_iph", h =>
            {
                EnsureResolved();
                // F1(2026-11-14 复审):装配期快照 remake 在场——install 委托在
                // Compose 的 ReconcileAll 期执行(= 装配期;热装卸 off→on 重试
                // 时重取,ModHelper 运行集合会话内静态,与 HookRuntime.family
                // 快照同语义),绝不在每次生成的 postfix 里查。
                remakeActive = ModHelper.IsModRunning(FamilyMods.Remake);
                MethodBase original = ModReflect.Target(itemFactory, "CreateMaidenOfFaction", new[] { typeof(string) });
                h.Patch(original, postfix: new HarmonyMethod(CreateMaidenOfFactionPostfix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(两阶段提交,同 MaidenPatch.EnsureResolved:
        /// 任一成员解析抛出 → 原样上抛不提交任何字段;失败由 HookInstaller 逐单元
        /// 隔离记 Dead)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type factory = FindMainType("ItemFactory");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedSetInt = ModReflect.StaticMethod<Action<GameItem, string, int>>(factory, "SetInt");
            var resolvedGetInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(factory, "GetInt");
            var resolvedUpdate = ModReflect.StaticMethod<Action<GameItem>>(factory, "UpdateMaidenDescription");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            itemFactory = factory;
            ifSetInt = resolvedSetInt;
            ifGetInt = resolvedGetInt;
            ifUpdateMaidenDescription = resolvedUpdate;
            mainLog = resolvedLog;
            mainLogWarning = resolvedLogWarning;
            resolved = true;
        }

        /// <summary>在前置 lewd_meme 暴露的类型里按名查找(同名不同命名空间亦命中;
        /// 主 mod 是前置,ModReflect 访问合法)。</summary>
        private static System.Type FindMainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("主 mod lewd_meme 未暴露类型 " + name);
        }

        /// <summary>CreateMaidenOfFaction 的配置快照(一次读全;读回全部等于
        /// 主 mod 原文字面量 → AnyOverride = false → postfix 零参与)。开关语义
        /// 经 ConfigRegistry.PlayerLayerAllowed 天然承担:任一子项开关关 → 该键
        /// 玩家层被压 → Current 回默认;「开关键」本身不是本表的读取对象。</summary>
        private sealed class AttrIphCfg
        {
            public double HighChance, MidChance;            // 三档累计阈值(1% / 6%,低档 = 补集)
            public (int lo, int hi) High, Mid, Low;         // 三档区间([70,80]/[60,70]/[10,60])
            public bool AnyOverride;                        // 任一键 ≠ 原文字面量

            public static AttrIphCfg Read()
            {
                float highChance = Cfg(CfgKeys.IphAttrHighChance, HighChanceDefault);
                float midChance = Cfg(CfgKeys.IphAttrMidChance, MidChanceDefault);
                (int lo, int hi) high = ReadRangeSafe(CfgKeys.IphAttrHighRange, HighRangeDefault);
                (int lo, int hi) mid = ReadRangeSafe(CfgKeys.IphAttrMidRange, MidRangeDefault);
                (int lo, int hi) low = ReadRangeSafe(CfgKeys.IphAttrLowRange, LowRangeDefault);
                return new AttrIphCfg
                {
                    HighChance = highChance,
                    MidChance = midChance,
                    High = high,
                    Mid = mid,
                    Low = low,
                    AnyOverride = highChance != HighChanceDefault || midChance != MidChanceDefault
                        || high != (70, 80) || mid != (60, 70) || low != (10, 60),
                };
            }

            private static (int lo, int hi) ReadRangeSafe(string key, string defaultCsv)
            {
                (int lo, int hi) r = MaidenRoll.ParseIntRangeSafe(Cfg(key, defaultCsv), defaultCsv, out bool usedDefault);
                if (usedDefault)
                    MaidenPatch.LogCfgOnce(key, key + " 解析失败,回退默认 \"" + defaultCsv + "\"(面板 CSV 容错)");
                return r;
            }
        }

        /// <summary>
        /// postfix on 主 mod <c>ItemFactory.CreateMaidenOfFaction(string)</c>
        /// (含 CreateMaiden() 无参版的内部调用——IL call 指令指向同一方法,
        /// patched 版本对所有调用方生效,与 IPH transpiler 同理)。全默认零参与;
        /// 任一 ≠ 默认 → 重掷三属性 tag + 差量修正 unitValue + 刷新描述。
        /// </summary>
        [HarmonyPostfix]
        private static void CreateMaidenOfFactionPostfix(GameItem __result)
        {
            if (__result == null) return; // 原方法失败(半途异常产物为 null),零参与
            AttrIphCfg c;
            try
            {
                c = AttrIphCfg.Read();
            }
            catch (Exception ex)
            {
                LogMainWarning("少女生成属性配置读取失败,保持原产物: " + ex.Message);
                return;
            }
            if (!c.AnyOverride) return; // 全默认 = 零参与(精确零漂移红线)
            try
            {
                // ⚠ 教训(R2 复审,Critical):old 三值的读取点必须在 SetInt×3 之前——
                // SetInt/GetInt 走同一 TagState 存储,先写后读 = 读回新值,差量恒等
                // no-op(remake 缺席 + IPH 在场主流组合下属性重掷后身价不随动)。
                // remake 在场也读(一次 ModData 读开销,换读取点无条件正确的结构;
                // 裁定「读就是」)。
                int oldForce = ifGetInt(__result, "MAIDEN_FORCE");
                int oldIntel = ifGetInt(__result, "MAIDEN_INTEL");
                int oldCharm = ifGetInt(__result, "MAIDEN_CHARM");
                int force = MaidenRoll.RollAttr(UnityEngine.Random.value, c.HighChance, c.MidChance, c.High, c.Mid, c.Low);
                int intel = MaidenRoll.RollAttr(UnityEngine.Random.value, c.HighChance, c.MidChance, c.High, c.Mid, c.Low);
                int charm = MaidenRoll.RollAttr(UnityEngine.Random.value, c.HighChance, c.MidChance, c.High, c.Mid, c.Low);
                ifSetInt(__result, "MAIDEN_FORCE", force);
                ifSetInt(__result, "MAIDEN_INTEL", intel);
                ifSetInt(__result, "MAIDEN_CHARM", charm);
                lastRerollTrace = null; // 每次重掷先清(remake 差量跳过时保持 null = 设计态标记)
                if (!remakeActive)
                {
                    // unitValue 差量修正(原文 :87 unitValue = FactionBase + AttrValue×3,
                    // 三属性重掷后价格必须随动——只动 AttrValue 部分,阵营基价保持原方法
                    // 已算值;Δ 经 MaidenRoll.AttrDelta 单一事实源,断点 60/80、斜率 5/10
                    // = 原版固定常量,主面板先例不做键)。F1:remake 在场时跳过——其
                    // InitializeMaiden/UpdateMaidenValue 用不含属性的公式重定 unitValue,
                    // 主 mod 公式的差量基准不可靠,详见类注释 F1 条。
                    long delta = MaidenRoll.AttrDelta(oldForce, oldIntel, oldCharm, force, intel, charm, 60, 80);
                    __result.unitValue += delta;
                    lastRerollTrace = new AttrRerollTrace(__result.unitValue - delta,
                        oldForce, oldIntel, oldCharm, force, intel, charm, delta);
                }
                ifUpdateMaidenDescription(__result); // 刷新描述(含属性数值行,委托直调先例 = TrapPatch)
                LogMain("少女生成属性重掷(立绘管理器共存补丁): 力量/智力/魅力 = "
                    + force + "/" + intel + "/" + charm
                    + (remakeActive ? "(暗流交易在场,身价归其公式,差量跳过)" : ""));
            }
            catch (Exception ex)
            {
                // 半重掷优于毁产物:已写入的 tag 保留(下次生成自愈),绝不回滚原方法状态
                LogMainWarning("少女生成属性重掷失败,保留原属性: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件(同 MaidenPatch;独立持有,不依赖其 install 期解析缓存——
        // IPH 在场时 MaidenPatch.Register 让位早退,resolved 恒 false)
        // ------------------------------------------------------------------

        private static T Cfg<T>(string key, T def)
        {
            return PatchCtx.Config != null ? PatchCtx.Config.Current<T>(key) : def;
        }

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
    }
}
