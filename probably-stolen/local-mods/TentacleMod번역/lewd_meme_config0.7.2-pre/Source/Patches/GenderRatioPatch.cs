using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 性别改写 4 档接管(Task 6,目录单元 main.on_generated,开关开且版本受支持时装配):
    /// prefix 接管主 mod GenderRatio.OnGenerated(StoreClientManager),返回 false
    /// 跳过原方法,重写逻辑按反编译 v1.3.8 逐行复刻。与原版差异仅三点:
    ///  1) 概率/特殊顾客排除按 <see cref="RewriteDecision.ShouldRewrite"/> 的 4 档语义:
    ///     档 0 = 原版(chance 概率改普通顾客);档 1 = 普通顾客 100%;
    ///     档 2 = 连特殊顾客一起改写(不查 IsAny);档 3 = 本 prefix 直接放行原方法
    ///     (return true)——全女性归 Task 5 游戏本体两点,原方法的 0.75 改写因
    ///     GetClientGender 已判 Female 自然 no-op;
    ///  2) 档 2 之下 MaidenIdentity.SpriteFor 返回空时,按 clientFaction 关键词从
    ///     游戏本体女池随机兜底(<see cref="FallbackFemaleSprite"/>,controller 裁定);
    ///  3) 其余逐行同原版(含 try/catch 与 ModCtx 日志格式)。
    /// 主 mod 类型(GenderRatio / MaidenIdentity / SpecialClients / ModCtx)一律运行时
    /// 经 ModReflect 解析成委托缓存,不编译引用 MidnightPawn.*;解析只发生在装配
    /// 委托内(装配 = 物品目录初始化后,本 mod 已被加载器登记,ModReflect 合法),
    /// 绝不进静态构造。安装失败(主 mod 更新致签名漂移)由 HookInstaller 逐单元
    /// 隔离记 Dead,不阻断其余补丁。
    /// </summary>
    public static class GenderRatioPatch
    {
        // -- 主 mod 静态方法的运行时委托缓存(install 时经 ModReflect 填充,prefix 只做直接调用) --
        private static Func<string, string> factionCn;          // MaidenIdentity.FactionCn
        private static Func<string, string> factionFromSprite;  // MaidenIdentity.FactionFromSprite
        private static Func<string, string> jobCn;              // MaidenIdentity.JobCn
        private static Func<string, string, string> spriteFor;  // MaidenIdentity.SpriteFor
        private static Func<StoreClient, bool> isAny;           // SpecialClients.IsAny
        private static Action<object> mainLog;                  // ModCtx.Log
        private static Action<object> mainLogWarning;           // ModCtx.LogWarning

        /// <summary>把接管补丁的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.on_generated", h =>
            {
                // ModReflect 惰性解析:只在此处(install = 物品目录初始化后)执行,
                // 静态字段默认 null,不触发任何静态构造期的 ModReflect 查找。
                System.Type maiden = FindMainType("MaidenIdentity");
                System.Type special = FindMainType("SpecialClients");
                System.Type ctx = FindMainType("ModCtx");
                factionCn = ModReflect.StaticMethod<Func<string, string>>(maiden, "FactionCn");
                factionFromSprite = ModReflect.StaticMethod<Func<string, string>>(maiden, "FactionFromSprite");
                jobCn = ModReflect.StaticMethod<Func<string, string>>(maiden, "JobCn");
                spriteFor = ModReflect.StaticMethod<Func<string, string, string>>(maiden, "SpriteFor");
                isAny = ModReflect.StaticMethod<Func<StoreClient, bool>>(special, "IsAny");
                mainLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
                mainLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");

                // 原方法:主 mod GenderRatio.OnGenerated(StoreClientManager)。签名用游戏
                // 类型精确匹配(declared prerequisite,Harmony 合法目标)。
                MethodBase original = ModReflect.Target(
                    FindMainType("GenderRatio"), "OnGenerated", new[] { typeof(StoreClientManager) });
                // 0.5.0:bool prefix 返回 false 会跳过其后未运行的 prefix——抢先
                // 高优先级,保证本 mod 复刻全权(ItemPortraitHD 的 ConvertPrefix 在
                // 其「顾客转换」开启时的共存以游戏内双开验收为准,异常即 A 兜底)
                h.Patch(original, prefix: new HarmonyMethod(OnGeneratedPrefix) { priority = Priority.High });
                return new[] { original };
            });
        }

        /// <summary>在前置 lewd_meme 暴露的类型里按名查找(同名不同命名空间亦命中)。</summary>
        private static System.Type FindMainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("본체 모드 lewd_meme에 타입이 노출되지 않음: " + name);
        }

        /// <summary>
        /// 接管前缀。档 3 放行原方法;档 0–2 跳过原方法并自任改写。
        /// scope 经 PatchCtx.Config 实时读取(ModEntry 在装配之前注入;未注入/
        /// 停用后 ?. 兜底默认 0 = 原版行为)。优先级保持 patch 期指定的
        /// Priority.High(R8 例外,0.5.1 语义:bool prefix 抢先防跳序)。
        /// </summary>
        [HarmonyPrefix]
        private static bool OnGeneratedPrefix(StoreClientManager mgr)
        {
            int scope = RewriteDecision.ClampScope(PatchCtx.Config?.Current<int>(CfgKeys.GenderScope) ?? 0);
            if (scope == 3) return true; // 不干预:全女性归 Task 5 游戏本体两点

            try
            {
                // 以下逐行复刻反编译 v1.3.8(差异仅 ShouldRewrite 门控 + 档 2 兜底)
                if (mgr == null || mgr.clientStack == null || mgr.clientStack.Count == 0)
                    return false;
                StoreClient val = mgr.clientStack[mgr.clientStack.Count - 1];
                if (val == null || (int)LocGenderHelper.GetClientGender(val) == 2)
                    return false; // 已是女性不改写(原版性别闸门,全档保留)

                // 档 0/1 保留特殊顾客排除(原版 IsAny);档 2 连特殊顾客一起改写(不查 IsAny)
                bool isSpecial = scope <= 1 && isAny(val);
                float chance = PatchCtx.Config?.Current<float>(CfgKeys.FemaleRewriteChance) ?? 0.75f;
                // 仅档 0 的概率分支消耗 Random.value——与原版 && 短路一致:
                // 特殊/女性顾客在原版根本走不到 Random.value,不抽随机数以保持 RNG 流对齐
                double roll = scope == 0 && !isSpecial ? UnityEngine.Random.value : 0.0;
                if (!RewriteDecision.ShouldRewrite(scope, isSpecial, chance, roll))
                    return false;

                string faction = !string.IsNullOrEmpty(val.clientFaction)
                    ? factionCn(val.clientFaction)
                    : factionFromSprite(val.spriteName);
                string job = jobCn(!string.IsNullOrEmpty(val.identifier) ? val.identifier : val.spriteName);
                string sprite = spriteFor(faction, job);
                if (string.IsNullOrEmpty(sprite) && scope == 2)
                    sprite = FallbackFemaleSprite(val.clientFaction); // 档 2 专属兜底
                if (!string.IsNullOrEmpty(sprite))
                {
                    val.spriteName = sprite;
                    val.possibleSprites = new List<string> { sprite };
                    LogMain("고객 성별을 여성으로 변경: " + val.displayName + "（" + faction + job + " → " + sprite + "）");
                }
            }
            catch (Exception ex)
            {
                LogMainWarning("성별 변경 실패: " + ex.Message);
            }
            return false; // 档 0–2:本 prefix 全权改写,恒跳过原方法
        }

        /// <summary>
        /// 档 2 兜底:SpriteFor 为空时按 clientFaction 关键词从游戏本体女池随机取
        /// (controller 裁定的映射,大小写不敏感,默认 Scav):
        ///   UPPER → SpriteDict.GetRandomUpperFemaleSpriteName
        ///   SEC   → SpriteDict.GetRandomSecFemaleSpriteName
        ///   BLACK → SpriteDict.GetRandomBMFemaleSpriteName   (FACTION_BLACK_MARKET)
        ///   REV   → SpriteDict.GetRandomRevFemaleSpriteName
        ///   其余(LOWER/TOURIST/CHURCH/CARTEL/空,游戏无对应女池)→ GetRandomScavFemaleSpriteName
        /// </summary>
        private static string FallbackFemaleSprite(string clientFaction)
        {
            string f = clientFaction ?? "";
            if (f.IndexOf("UPPER", StringComparison.OrdinalIgnoreCase) >= 0)
                return SpriteDict.GetRandomUpperFemaleSpriteName();
            if (f.IndexOf("SEC", StringComparison.OrdinalIgnoreCase) >= 0)
                return SpriteDict.GetRandomSecFemaleSpriteName();
            if (f.IndexOf("BLACK", StringComparison.OrdinalIgnoreCase) >= 0)
                return SpriteDict.GetRandomBMFemaleSpriteName();
            if (f.IndexOf("REV", StringComparison.OrdinalIgnoreCase) >= 0)
                return SpriteDict.GetRandomRevFemaleSpriteName();
            return SpriteDict.GetRandomScavFemaleSpriteName();
        }

        /// <summary>日志走主 mod ModCtx(保持原版前缀与格式);委托缺席时退回 Debug.Log。</summary>
        private static void LogMain(object msg)
        {
            if (mainLog != null) mainLog(msg);
            else Debug.Log("[lewd_meme_config→본체 모드] " + msg);
        }

        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else Debug.LogWarning("[lewd_meme_config→본체 모드] " + msg);
        }
    }
}
