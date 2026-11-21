using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 掉落接管(Task 11 建,目录单元 main.on_loot_rolled,开关开且版本受支持时
    /// 装配;Task 14b 对齐主 mod 1.7.1;0.6.0 R7 增 main.loot_maiden_chance
    /// getter 接管单元,见 MaidenChancePrefix):prefix 接管主 mod
    /// <c>MidnightPawn.Game.LootHook.OnLootRolled(LootRollContext)</c>(静态 void,
    /// 游戏类型参数可编译引用),返回 false 跳过原方法,重写按反编译 1.7.1
    /// <c>LootHook.cs L30-54</c> 逐行复刻:
    /// null/拾荒组名守卫(原版 L32-35:ctx == null 或 groupId 不等于
    /// "dumpingGroundTG"/"dumpingGroundFreshTG" 之一 → 直接返回)→ 0.5s 节流
    /// (L37-42:lastGroupId/lastRollTime 静态,api 自持镜像)→ HasBody 检查
    /// (L43-47,false 记「拾荒：没带触手本体，跳过少女判定」后返回)→ 少女
    /// 掷骰(L48-53,至多 1 次 Random.value)。医疗组排卵针分支已在 1.7.0 从
    /// 原版删除(161 基线 L36-44 → 171 基线无),loot.med_keywords /
    /// loot.med_shot_chance 双键随 Task 14b 重定位为 opt-in「恢复 1.6.x 医疗组
    /// 排卵针掉落」:任一 med 开关键开启 → 复刻体在守卫前前置医疗分支(1.6.1
    /// 语义逐字:keywords CSV 判定 + 概率掷骰 + "ovulation_shot" + 161 基线
    /// L42 文案「战利品：医疗类里翻到一支排卵诱发剂 (组名)」,命中(无论掷否)
    /// 即返回不走拾荒——161 基线 L45 else 结构);双键全关(默认)→ 复刻体
    /// 不含医疗分支,与 1.7.1 原版逐字一致 = 验收红线(R-X 双态零漂移)。
    /// 拾荒组名 loot.scav_groups(原版 "dumpingGroundTG"/"dumpingGroundFreshTG"
    /// 两个 ==,经 LootRules.InScavGroups 精确匹配)、少女概率
    /// loot.scav_maiden_base(原版 ModSettings.MaidenChance,默认 0.05f,getter
    /// 内部 clamp 0.05-0.25 且失败回 0.05)。
    /// RNG 消耗次序与短路结构逐行保留:医疗分支开启时至多 1 次 Random.value
    /// (仅医疗组命中时);拾荒分支组名判定 → 节流(命中 0 次,窗口 =
    /// loot.throttle_seconds,原版 ROLL_COOLDOWN 内联 0.5f;0=禁用,
    /// now-lastRollTime &lt; 0f 恒 false 天然达成,无特判)→ HasBody 独立 if
    /// (false 记日志后返回,0 次)→ 至多 1 次 Random.value(少女)。
    /// 少女概率取值链(spec Current 语义):子 mod override &gt; api 面板 &gt;
    /// 主 mod 面板动态层(ModEntry 注入 ModSettings.MaidenChance getter,每次
    /// 读取现求值,getter 内部已 clamp 0.05-0.25 且失败回 0.05,见 ModSettings,
    /// api 不双重钳制;provider 异常由 ConfigRegistry.Current 内部吞掉跳层)
    /// &gt; 默认 0.05。无键常量 as-is 复刻:"maiden_lower"/"ovulation_shot" 两个
    /// 物品 id 与两条 ModCtx.Log 文案(含 ToString("P0") 文化敏感格式化,
    /// 同原版调用)。
    /// med 双态判定(R-X):两开关键 loot.med_keywords.enabled /
    /// loot.med_shot_chance.enabled 任一开 → MedRestore = true(开关键经
    /// Current 读取自带豁免,PatchCtx.Config 缺席时退 false = 无医疗分支);
    /// 关键词/概率各自经 Current 读数(未开启的键玩家层被压 → 退默认
    /// med,drug,pharma,chem / 0.1 = 1.6.1 字面量)。
    /// 与原版的行为差异(sanctioned):
    ///  1) 配置读取集中在任何状态写入之前(Phase A);读失败(键缺失/类型不符)→
    ///     记日志放行原方法;复刻体(Phase B)异常 → ctx.itemId 已可能半写,
    ///     不放行原版重跑(原版会重掷 RNG 覆盖已定物品),吞掉记日志——
    ///     ctx.itemId 维持已写值/原滚值,引擎照常按 ctx 生成(写状态先例:
    ///     Task 8 SetCharge/DailyRecharge);
    ///  2) 原版 LootHook.MaidenChance getter 的 try/catch 回退 0.05f 由配置链
    ///     等价承担:动态层 provider(即该 getter)抛异常 → Current 跳层退默认
    ///     0.05f,与 getter 回退值相等,零行为差;主 mod 缺席时动态层不注入
    ///     (ModEntry 守卫),同样退默认;
    ///  3) med 恢复分支开启时,医疗组命中短路掉拾荒守卫(1.6.1 else 结构)——
    ///     若医疗关键词误含拾荒组名,该组走医疗分支不掷少女,与 1.6.1 一致。
    /// 主 mod 类型(LootHook/TentacleBody/ModCtx)一律运行时经 ModReflect 解析成
    /// 委托/类型引用,不编译引用 MidnightPawn.*;TentacleBody.HasBody 经委托
    /// 直调原实现(零漂移)。解析只发生在装配委托内(装配 = 物品目录初始化后,
    /// 本 mod 已被加载器登记,ModReflect 合法),绝不进静态构造,两阶段提交
    /// (任一成员解析失败 → 原样上抛不提交,条目不装)。安装失败(主 mod 更新
    /// 致签名漂移)由 HookInstaller 逐单元隔离记 Dead,该组不装配。
    /// 无协程/Invoke 调用面,不涉及 12/10 并发通道替换。
    /// </summary>
    public static class LootPatch
    {
        // -- 默认 CSV = 主 mod 1.6.1 OnLootRolled 字面量(与 ConfigRegistry.RegisterLoot 逐字相同) --
        internal const string MedDefault = "med,drug,pharma,chem";
        internal const string ScavDefault = "dumpingGroundTG,dumpingGroundFreshTG";

        // -- 1.6.1/1.7.1 节流静态(api 自持,镜像原版 LootHook L9-13 私有静态) --
        private static string lastGroupId;                 // 上次判定的拾荒组名(初始 null)
        private static float lastRollTime = -999f;         // 上次判定时刻(Time.time,初始 -999 = 首掷恒不节流)

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用) --
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type lootHook;                                 // LootHook(补丁目标)
        private static System.Type modSettings;                               // ModSettings(getter 补丁目标;MidnightPawn 命名空间,与 remake 同名类经 Types("lewd_meme") 天然防串)
        private static Func<bool> bodyHasBody;                                // TentacleBody.HasBody
        private static Action<object> mainLog;                                // ModCtx.Log
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning

        /// <summary>把接管补丁的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。
        /// main.loot_maiden_chance(0.6.0 R7 等价恢复):prefix 接管
        /// <c>ModSettings.get_MaidenChance</c>()——remake 在场时 OnLootRolled
        /// 虽让位,但其 transpiler 只 NOP 限流、概率路径(LootHook 私有属性 →
        /// 本 getter)原样保留,getter 接管即令 loot.scav_maiden_base 生效;
        /// 主 mod 单装时与复刻体同读面板值(同值双保险)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.on_loot_rolled", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(lootHook, "OnLootRolled", new[] { typeof(LootRollContext) });
                h.Patch(original, prefix: new HarmonyMethod(OnLootRolledPrefix));
                return new[] { original };
            });
            installs.Add("main.loot_maiden_chance", h =>
            {
                EnsureResolved();
                MethodBase getter = ModReflect.Target(modSettings, "get_MaidenChance", Type.EmptyTypes);
                h.Patch(getter, prefix: new HarmonyMethod(MaidenChancePrefix));
                return new[] { getter };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(任一成员解析抛出 → 本方法原样上抛、不提交任何
        /// 字段,条目不装,杜绝半解析状态下装出运行时 NRE 的 prefix——两阶段提交,
        /// 同 Task 7/8/9/10)。失败由 HookInstaller 逐单元隔离记 Dead,该组不装配。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type hook = FindMainType("LootHook");
            System.Type body = FindMainType("TentacleBody");
            System.Type ctx = FindMainType("ModCtx");
            System.Type settings = FindMainType("ModSettings");
            var resolvedHas = ModReflect.StaticMethod<Func<bool>>(body, "HasBody");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            lootHook = hook;
            modSettings = settings;
            bodyHasBody = resolvedHas;
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

        /// <summary>
        /// OnLootRolled 配置快照(Phase A 一次读全;任何键读取失败 → 放行原方法,
        /// 此时尚未写入任何游戏状态)。CSV 经 MaidenRoll.SplitCsv(切分/Trim/去空);
        /// 默认值 = 1.6.1 字面量(med 双键,恢复分支语义)/ 1.7.1 字面量(拾荒)。
        /// MedRestore = med 双开关键任一开启(R-X 双态:恢复 1.6.x 医疗分支与否;
        /// 开关键经 Current 读取自带豁免,Config 缺席时退 false = 不含医疗分支)。
        /// </summary>
        private sealed class LootCfg
        {
            public bool MedRestore;                        // 医疗恢复分支前置开关(任一 med 键开启)
            public IReadOnlyList<string> MedKeywords;      // 医疗组关键词(默认 4 词 = 1.6.1 字面量)
            public float MedShotChance;                    // 医疗组排卵针概率(1.6.1 原版 0.1f)
            public IReadOnlyList<string> ScavGroups;       // 拾荒组名(默认 2 组)
            public float ScavMaidenBase;                   // 少女概率(原版 ModSettings.MaidenChance,默认 0.05f;取值链含主 mod 面板动态层)

            public static LootCfg Read()
            {
                bool medKwOn = Cfg(PanelKeys.ItemSwitchKey(CfgKeys.MedKeywords), false);
                bool medChanceOn = Cfg(PanelKeys.ItemSwitchKey(CfgKeys.MedShotChance), false);
                return new LootCfg
                {
                    MedRestore = medKwOn || medChanceOn,
                    MedKeywords = MaidenRoll.SplitCsv(Cfg(CfgKeys.MedKeywords, MedDefault)),
                    MedShotChance = Cfg(CfgKeys.MedShotChance, 0.1f),
                    ScavGroups = MaidenRoll.SplitCsv(Cfg(CfgKeys.ScavGroups, ScavDefault)),
                    ScavMaidenBase = Cfg(CfgKeys.ScavMaidenBase, 0.05f),
                };
            }
        }

        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool OnLootRolledPrefix(LootRollContext ctx)
        {
            LootCfg c;
            try
            {
                c = LootCfg.Read();
            }
            catch (Exception ex)
            {
                LogMainWarning("掉落配置读取失败,放行原版: " + ex.Message);
                return true; // 尚未写入任何状态,原版重跑安全
            }
            try
            {
                OnLootRolledBody(ctx, c);
            }
            catch (Exception ex)
            {
                // 写状态(ctx.itemId):不放行原版重跑(原版重掷 RNG 会覆盖已定物品),
                // 吞掉记日志——ctx.itemId 维持已写值/原滚值,引擎照常按 ctx 生成。
                LogMainWarning("掉落判定失败(接管复刻): " + ex.Message);
            }
            return false;
        }

        /// <summary>复刻 1.7.1 OnLootRolled(LootHook.cs L30-54)主体;med 恢复
        /// 分支(开启时,161 基线 L36-44 逐字语义)前置在守卫之前。差异仅五配置
        /// (恢复开关/关键词/概率/组名/少女概率)、节流静态自持与捕获事件(见类
        /// 注释),守卫次序、RNG 消耗点、短路结构、物品 id 与两条日志文案 as-is。</summary>
        private static void OnLootRolledBody(LootRollContext ctx, LootCfg c)
        {
            if (ctx == null)
            {
                return;
            }
            // R-X med 恢复分支(opt-in,默认不含):1.6.1 L36-44 语义逐字——
            // 小写化 groupId 后关键词 Contains 判定;命中掷 1 次 Random.value,
            // 中签写 "ovulation_shot" + 161 基线 L42 文案;命中(无论掷否)即
            // 返回,不走拾荒(161 基线 L45 else 结构)。
            if (c.MedRestore && LootRules.IsMedGroup(ctx.groupId, c.MedKeywords))
            {
                if (UnityEngine.Random.value < c.MedShotChance)
                {
                    ctx.itemId = "ovulation_shot";
                    LogMain("战利品：医疗类里翻到一支排卵诱发剂 (" + ctx.groupId + ")");
                }
                return;
            }
            // 原版 1.7.1 L32-35:拾荒组名守卫(raw == 两个组名之一,经
            // InScavGroups 精确匹配;不中提前返回,零 RNG 消耗)
            if (!LootRules.InScavGroups(ctx.groupId, c.ScavGroups))
            {
                return;
            }
            // 原版 L36-42:节流在 HasBody 之前——命中直接返回(零 RNG 消耗),
            // 未命中先更新静态再继续。窗口 = loot.throttle_seconds
            // (原版 ROLL_COOLDOWN 内联 0.5f;0=禁用,now-lastRollTime < 0f
            // 恒 false 天然达成,无特判)
            float now = UnityEngine.Time.time;
            if (LootRules.Throttled(ctx.groupId, lastGroupId, now, lastRollTime,
                    Cfg(CfgKeys.LootThrottleSeconds, 0.5f)))
            {
                return;
            }
            lastGroupId = ctx.groupId;
            lastRollTime = now;
            if (!bodyHasBody())
            {
                LogMain("拾荒：没带触手本体，跳过少女判定");
                return;
            }
            float maidenChance = c.ScavMaidenBase;
            if (UnityEngine.Random.value < maidenChance)
            {
                ctx.itemId = "maiden_lower";
                LogMain("拾荒：捕获到一个少女（概率 " + maidenChance.ToString("P0") + "）");
            }
        }

        // ------------------------------------------------------------------
        // main.loot_maiden_chance — ModSettings.MaidenChance getter(等价恢复)
        // ------------------------------------------------------------------

        /// <summary>
        /// 主 mod <c>ModSettings.get_MaidenChance</c>(lewd_meme_171
        /// MidnightPawn/ModSettings.cs:16-31:try → Clamp(entry.Value, 0.05, 0.25)
        /// → catch 回退 0.05)的接管 prefix,0.6.0 R7 等价恢复:remake 在场时
        /// LootHook.OnLootRolled 让位但其 transpiler(OriginalLootPatch.cs:30-104)
        /// 只 NOP 限流 18 条,概率路径(LootHook 私有 MaidenChance 属性 → 本
        /// getter)原样保留——getter 接管即令 loot.scav_maiden_base 在双态
        /// (remake 在场 / 主 mod 单装)下都生效;主 mod 单装时复刻体
        /// (on_loot_rolled)与 getter 同读面板值,同值双保险。
        /// 取值走 <see cref="ConfigRegistry.PlayerOrDefault{T}"/>(玩家层直读,
        /// **不经 Current 全链**——动态层 provider 即本 getter,全链会递归);
        /// 返回值不做 0.05-0.25 钳制:本面板范围 0-1 且明示「可超出主 mod 面板
        /// 上限」,getter 的 clamp 是主 mod 官方面板 Define 的输入校验,接管后
        /// 以本面板域为准(消费方仅 Random.value 比较与 ToString 展示,无契约
        /// 破坏)。PatchCtx 缺席(mod 停用后)→ 放行原 getter,零漂移。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool MaidenChancePrefix(ref float __result)
        {
            ConfigRegistry cfg = PatchCtx.Config;
            if (cfg == null) return true; // 未注入:原 getter 原样运行(其 clamp/回退语义保留)
            try
            {
                __result = cfg.PlayerOrDefault<float>(CfgKeys.ScavMaidenBase);
                return false;
            }
            catch (Exception ex)
            {
                // 读值异常(键缺失/类型漂移):不放行原 getter——开关开着时期望面板值,
                // 放行会静默回主 mod 官方配置;退 getter 的 catch 回退值 0.05 同原版
                LogMainWarning("拾荒少女概率读取失败,退原版回退值 0.05: " + ex.Message);
                __result = 0.05f;
                return false;
            }
        }

        // ------------------------------------------------------------------
        // 共用小件
        // ------------------------------------------------------------------

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
    }
}
