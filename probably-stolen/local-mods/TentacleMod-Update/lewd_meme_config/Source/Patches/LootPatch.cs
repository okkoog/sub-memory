using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 掉落 med 拦截(Task 11 建;Task 14b 对齐 1.7.1;0.6.0 R7 曾增
    /// main.loot_maiden_chance getter 接管单元——**0.8.0 退役**:ScavMaidenBase
    /// 功能 ≙ 主 mod 官方键 maiden_loot_chance,由「主 mod 官方设置(拾荒)」
    /// 分区写透键接管(wt.main.maiden_loot_chance,双面板并存);ScavGroups/
    /// LootThrottleSeconds 两键随 2.1.0 拾荒重写机制死——OnLootRolled 以
    /// frameCount 帧节流取代 0.5s 时间节流、ScavengeRoll.Judge() 分档判定
    /// 取代固定概率+组过滤,原复刻体的守卫/节流/掷骰段全部不再复刻):
    /// prefix 接管 <c>MidnightPawn.Game.LootHook.OnLootRolled(LootRollContext)</c>
    /// (静态 void,游戏类型参数可编译引用),形态 = **med 拦截器**:
    ///  1. med 双开关键全关(默认)→ return true 放行 2.1.0 原方法零漂移
    ///     (官方帧节流 + 分档判定 + 官方排卵剂拾荒来源照常);
    ///  2. 任一 med 开关键开启(恢复 1.6.x 医疗组排卵针掉落,opt-in)→
    ///     医疗组命中:掷 1 次 Random.value,中签写 "ovulation_shot" +
    ///     161 基线 L42 文案,return false 吞原方法(1.6.1 语义:命中即返回
    ///     不走拾荒);非医疗组:return true 放行官方拾荒逻辑零干扰。
    /// RNG 消耗:仅医疗组命中时至多 1 次 Random.value;放行路径 0 次前置消耗。
    /// med 双态判定(R-X 沿用):loot.med_keywords.enabled /
    /// loot.med_shot_chance.enabled 任一开 → MedRestore = true(开关键经
    /// Current 读取自带豁免,PatchCtx.Config 缺席时退 false = 恒放行)。
    /// 与原版的行为差异(sanctioned):
    ///  1) 配置读取集中在任何状态写入之前;读失败(键缺失/类型不符)→ 记日志
    ///     放行原方法(尚未写入任何状态,原版重跑安全);
    ///  2) 复刻体异常 → ctx.itemId 已可能半写,不放行原版重跑(原版会重掷 RNG
    ///     覆盖已定物品),吞掉记日志——ctx.itemId 维持已写值/原滚值,引擎照常
    ///     按 ctx 生成(写状态先例:Task 8 SetCharge/DailyRecharge)。
    /// 主 mod 类型(LootHook/ModCtx)一律运行时经 ModReflect 解析,不编译引用
    /// MidnightPawn.*;解析只发生在装配委托内(装配 = 物品目录初始化后,本 mod
    /// 已被加载器登记,ModReflect 合法),绝不进静态构造,两阶段提交(任一成员
    /// 解析失败 → 原样上抛不提交,条目不装)。安装失败(主 mod 更新致签名漂移)
    /// 由 HookInstaller 逐单元隔离记 Dead,该组不装配。
    /// 无协程/Invoke 调用面,不涉及 12/10 并发通道替换。
    /// </summary>
    public static class LootPatch
    {
        // -- 默认 CSV = 主 mod 1.6.1 OnLootRolled 字面量(与 ConfigRegistry.RegisterLoot 逐字相同) --
        internal const string MedDefault = "med,drug,pharma,chem";

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用) --
        private static bool resolved;                                          // 全部成员就位后才置真
        private static System.Type lootHook;                                 // LootHook(补丁目标)
        private static Action<object> mainLog;                                // ModCtx.Log
        private static Action<object> mainLogWarning;                         // ModCtx.LogWarning

        /// <summary>把接管补丁的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.on_loot_rolled", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(lootHook, "OnLootRolled", new[] { typeof(LootRollContext) });
                h.Patch(original, prefix: new HarmonyMethod(OnLootRolledPrefix));
                return new[] { original };
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
            System.Type ctx = FindMainType("ModCtx");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            // 全部成功,一次性提交
            lootHook = hook;
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
        /// 默认值 = 1.6.1 字面量。MedRestore = med 双开关键任一开启(R-X 双态:
        /// 恢复 1.6.x 医疗分支与否;开关键经 Current 读取自带豁免,Config 缺席时
        /// 退 false = 不含医疗分支)。
        /// </summary>
        private sealed class LootCfg
        {
            public bool MedRestore;                        // 医疗恢复分支前置开关(任一 med 键开启)
            public IReadOnlyList<string> MedKeywords;      // 医疗组关键词(默认 4 词 = 1.6.1 字面量)
            public float MedShotChance;                    // 医疗组排卵针概率(1.6.1 原版 0.1f)

            public static LootCfg Read()
            {
                bool medKwOn = Cfg(PanelKeys.ItemSwitchKey(CfgKeys.MedKeywords), false);
                bool medChanceOn = Cfg(PanelKeys.ItemSwitchKey(CfgKeys.MedShotChance), false);
                return new LootCfg
                {
                    MedRestore = medKwOn || medChanceOn,
                    MedKeywords = MaidenRoll.SplitCsv(Cfg(CfgKeys.MedKeywords, MedDefault)),
                    MedShotChance = Cfg(CfgKeys.MedShotChance, 0.1f),
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
            if (ctx == null || !c.MedRestore)
            {
                return true; // 默认态:放行 2.1.0 官方拾荒逻辑零漂移
            }
            try
            {
                // R-X med 恢复分支(opt-in):1.6.1 L36-44 语义逐字——小写化
                // groupId 后关键词 Contains 判定;命中掷 1 次 Random.value,
                // 中签写 "ovulation_shot" + 161 基线 L42 文案;命中(无论掷否)
                // 即吞原方法不走拾荒(161 基线 L45 else 结构);非医疗组 →
                // 放行官方拾荒逻辑(2.1.0 官方判定对全部回调生效,无组过滤)。
                if (LootRules.IsMedGroup(ctx.groupId, c.MedKeywords))
                {
                    if (UnityEngine.Random.value < c.MedShotChance)
                    {
                        ctx.itemId = "ovulation_shot";
                        LogMain("战利品：医疗类里翻到一支排卵诱发剂 (" + ctx.groupId + ")");
                    }
                    return false; // 医疗组命中:吞原方法(1.6.1 语义)
                }
                return true; // 非医疗组:放行官方拾荒
            }
            catch (Exception ex)
            {
                // 写状态(ctx.itemId):不放行原版重跑(原版重掷 RNG 会覆盖已定
                // 物品),吞掉记日志——ctx.itemId 维持已写值/原滚值,引擎照常按
                // ctx 生成。
                LogMainWarning("掉落判定失败(med 拦截): " + ex.Message);
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
