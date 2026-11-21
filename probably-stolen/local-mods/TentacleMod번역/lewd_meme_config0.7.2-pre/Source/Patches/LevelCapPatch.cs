using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LewdMemeConfig
{
    /// <summary>
    /// 等级上限 + 升级经验公式接管(更多可配置项 Task 7 / 0.7.0 Task 9,两个目录
    /// 单元 main.level_cap / main.exp_formula,开关开且版本受支持时装配):
    ///  <list type="bullet">
    ///   <item>main.level_cap — prefix 接管主 mod TentacleBody.GetBodyLevel()
    ///     (public static int,1.9.2 :1096-1099),cap = progress.level_cap。
    ///     原版一行:return Mathf.Clamp(ModHelper.GetModDataInt("lewd_meme",
    ///     "body_lv", 1), 1, 5);与原版的差异(sanctioned):
    ///     1) cap == 5(默认)时直接放行原方法——默认值零漂移,不进复刻体;
    ///     2) cap ≠ 5 时返回 false 跳过原方法,复刻体仅把 clamp 上限换为 cap,
    ///        逐字对齐;1.9.2 语义注记:升级循环(AddBodyExp)不再有 cap 拦截
    ///        (仅 10000 次防死循环),body_lv 可超上限存档,读取与显示经本
    ///        prefix clamp 封顶——升停语义移交给读 clamp。</item>
    ///   <item>main.exp_formula(0.7.0 T9 新增)— prefix 接管
    ///     TentacleBody.ExpNeedForLevel(int)(public static,1.9.2 :974-981):
    ///     <code>if (lv &lt; 1) lv = 1; return 100 + (lv - 1) / 3 * 100;</code>
    ///     三键 progress.exp_base / exp_divisor / exp_step(默认 100/3/100 =
    ///     原版逐字)→ 默认三值直接放行原方法零漂移;任一偏离 → 复刻体经
    ///     <see cref="ExpCurve.ExpNeed"/> 求值(整数除法向零截断)。1.9.2 已删
    ///     EXP_NEED 静态数组,经验表唯一事实源即本方法——AddBodyExp 升级循环 /
    ///     GetExpNeed 显示 / DebugAddExp 日志全调用方一致生效。</item>
    ///  </list>
    /// 主 mod 类型(TentacleBody)运行时经 ModReflect 解析,不编译引用 MidnightPawn.*;
    /// 解析只发生在装配委托内;安装失败由 HookInstaller 逐单元隔离记 Dead,不阻断其余。
    /// </summary>
    public static class LevelCapPatch
    {
        /// <summary>把两条接管单元的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("main.level_cap", h =>
            {
                // 无参静态方法,signature 省略(null = 按名唯一匹配)
                MethodBase original = ModReflect.Target(
                    FindMainType("TentacleBody"), "GetBodyLevel");
                h.Patch(original, prefix: new HarmonyMethod(GetBodyLevelPrefix));
                return new[] { original };
            });
            installs.Add("main.exp_formula", h =>
            {
                // ExpNeedForLevel(int lv),1.9.2 :974-981(0.7.0 T9)
                MethodBase original = ModReflect.Target(
                    FindMainType("TentacleBody"), "ExpNeedForLevel", new[] { typeof(int) });
                h.Patch(original, prefix: new HarmonyMethod(ExpNeedForLevelPrefix));
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
        /// 接管前缀。默认(5)放行原方法;否则复刻原版一行、clamp 上限换 cap。
        /// cap 经 PatchCtx.Config 实时读取(未注入/停用兜底 5 = 原版行为)。
        /// ModHelper.GetModDataInt 为游戏类型直接编译引用(非 ModReflect 范畴)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool GetBodyLevelPrefix(ref int __result)
        {
            int cap = PatchCtx.Config?.Current<int>(CfgKeys.LevelCap) ?? 5;
            if (cap == 5) return true; // 默认零漂移:原方法原样运行

            __result = Mathf.Clamp(ModHelper.GetModDataInt("lewd_meme", "body_lv", 1), 1, cap);
            return false; // 复刻体已定结果,跳过原方法
        }

        /// <summary>
        /// 升级经验公式前缀(0.7.0 T9)。默认三键(100/3/100 = 1.9.2 原版逐字)
        /// 放行原方法——默认零漂移;任一偏离 → 复刻体经 <see cref="ExpCurve.ExpNeed"/>
        /// 求值(lv&lt;1 钳 1 与整数除法截断都在纯函数内,与原方法逐字对齐)。
        /// 读配置失败(键缺失/类型不符)→ 放行原方法(尚未写任何状态,安全)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ExpNeedForLevelPrefix(int lv, ref int __result)
        {
            int expBase, expDivisor, expStep;
            try
            {
                var cfg = PatchCtx.Config;
                expBase = cfg?.Current<int>(CfgKeys.ExpBase) ?? 100;
                expDivisor = cfg?.Current<int>(CfgKeys.ExpDivisor) ?? 3;
                expStep = cfg?.Current<int>(CfgKeys.ExpStep) ?? 100;
            }
            catch (Exception)
            {
                return true; // 配置读取失败:放行原版公式
            }
            if (expBase == 100 && expDivisor == 3 && expStep == 100) return true; // 默认零漂移
            if (expDivisor < 1) expDivisor = 1; // 除零防御(面板 Define 1-10 外的脏值钳回 1)
            __result = ExpCurve.ExpNeed(lv, expBase, expDivisor, expStep);
            return false; // 复刻体已定结果,跳过原方法
        }
    }
}
