using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 档 3(全女性)的游戏本体补丁,目录单元 game.pickgendered / game.sprite_name
    /// (TargetMod = null 纯游戏代码目标,主 mod 缺席也可装):
    ///  - PickGenderedPrefix:男女池 50/50 抽脸改为只从女池(poolB)抽,跳过原方法;
    ///  - GetGenderFromSpriteNamePostfix:男性/具名中性 NPC 的本地化性别改判女性。
    /// 仅在 client.gender_scope == 3 时生效,其余档位原样放行(前缀 return true /
    /// 后缀不动 __result)。目标方法均为游戏 Assembly-CSharp 的 public static,
    /// 编译引用合法(12 月规则下游戏代码是合法 patch 目标)。
    /// scope 经 PatchCtx.Config 读取——ModEntry 在 HookRuntime 装配之前注入,
    /// 装配完成时必非 null;仍以 ?. 兜底(默认 0 = 不生效)。
    /// </summary>
    public static class GameGenderPatches
    {
        // 收口与 GenderRatioPatch.OnGeneratedPrefix 同一权威:scope 先经 RewriteDecision.ClampScope
        // 收敛到 [0,3] 再比较(Review Focus 3:gender_scope=4 → 3)——否则接管侧按 clamp 判 3
        // 放行原方法,而本游戏侧按裸值 4 判"非 3"失活,裂缝间玩家拿到原版 0.75 改写。
        // ClampScope 的纯函数语义由 RewriteDecisionTests 固定(Patches 不进测试工程)。
        private static int Scope => RewriteDecision.ClampScope(PatchCtx.Config?.Current<int>(CfgKeys.GenderScope) ?? 0);

        /// <summary>把两个补丁的装配委托登记进映射(catalog-id → install;开关开且
        /// 非压制时由 HookInstaller 调用,Harmony 实例由安装器转交)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("game.pickgendered", h =>
            {
                MethodInfo original = typeof(ClientSpriteVariety).GetMethod(
                    nameof(ClientSpriteVariety.PickGendered),
                    BindingFlags.Public | BindingFlags.Static);
                if (original == null)
                    throw new MissingMethodException(nameof(ClientSpriteVariety), nameof(ClientSpriteVariety.PickGendered));
                h.Patch(original, prefix: new HarmonyMethod(PickGenderedPrefix));
                return new[] { original };
            });
            installs.Add("game.sprite_name", h =>
            {
                MethodInfo original = typeof(LocGenderHelper).GetMethod(
                    nameof(LocGenderHelper.GetGenderFromSpriteName),
                    BindingFlags.Public | BindingFlags.Static);
                if (original == null)
                    throw new MissingMethodException(nameof(LocGenderHelper), nameof(LocGenderHelper.GetGenderFromSpriteName));
                h.Patch(original, postfix: new HarmonyMethod(GetGenderFromSpriteNamePostfix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 档 3:跳过原 50/50 逻辑,直接调用游戏 public static Pick 从女池抽脸
        /// (调用方按 (malePool, femalePool) 传参,poolB 即女池);其余档位放行原方法。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool PickGenderedPrefix(List<string> poolA, List<string> poolB, ref string __result)
        {
            if (Scope != 3) return true;
            __result = ClientSpriteVariety.Pick(poolB);
            return false;
        }

        /// <summary>
        /// 档 3:按 <see cref="GenderDecision.TreatAsFemale"/> 把 Male 与具名 Neutral
        /// 的判定结果改写为 Female;Female 与未具名 Neutral 原样保留。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void GetGenderFromSpriteNamePostfix(string spriteName, ref LocGenderHelper.LocGender __result)
        {
            if (Scope == 3 && GenderDecision.TreatAsFemale((int)__result, spriteName, 3))
                __result = LocGenderHelper.LocGender.Female;
        }
    }
}
