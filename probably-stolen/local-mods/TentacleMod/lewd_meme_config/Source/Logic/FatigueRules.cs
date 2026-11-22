using System;

namespace LewdMemeConfig
{
    /// <summary>
    /// 疲劳核心纯逻辑(ST 1.6.1 Task 5,2026-11-17):ShadowTrade 1.6.1
    /// MidnightPawn.Game/MaidenFatigue.cs 数值面的纯函数半边——prefix 复刻体
    /// (RemakePatches)把 Enabled/LevelMul/Scale 等 remake 运行时状态经委托
    /// 取值后注入,求值内核落此处理(ExpCurve 先例),单测锁定
    /// (FatigueRulesTests)。三函数与源码逐字对齐:
    ///  - <see cref="ValueMul"/>(:210-233):五档售价倍率,档位边界
    ///    ExhaustedAt 90 / WearyAt 80 / TiredAt 60 / 休息充足线 20 = 源码 const,
    ///    保持原版不可调;正常档(20..59)恒 1f 无键;!Enabled → 1f 分支在
    ///    prefix(恒 1f 与档位值无关);
    ///  - <see cref="NightlyGain"/>(:254-268):加权式 vaginal + anal +
    ///    RoundToInt(oral × 0.5f)(口交加权 0.5f 是独立常量,不属面板键)
    ///    → weighted × 基数 × LevelMul × (孕 1.5) × Scale;weighted ≤ 0 与
    ///    gain ≤ 0 两处早退 + Max(1,·) 下限;RoundToInt = 银行家舍入
    ///    (MidpointRounding.ToEven,与 UnityEngine.Mathf.RoundToInt 一致);
    ///  - <see cref="Recovered"/>(:124):透支恢复减法 Max(0, fatigue − recover),
    ///    0 下限;上限 100 由源码 Set 的 clamp 兜底(复刻体经委托直调原版 Set)。
    /// </summary>
    public static class FatigueRules
    {
        /// <summary>休息充足线(StateName「休息充足」上界,源码 :160/:228 的 20)。</summary>
        public const int RestedBelow = 20;

        /// <summary>疲倦线(const TiredAt,源码 :20)。</summary>
        public const int TiredAt = 60;

        /// <summary>疲惫线(const WearyAt,源码 :22)。</summary>
        public const int WearyAt = 80;

        /// <summary>透支线(const ExhaustedAt,源码 :24)。</summary>
        public const int ExhaustedAt = 90;

        /// <summary>五档售价倍率(与 ValueMul :210-233 逐字对齐;Enabled 分支归 prefix)。
        /// 档位边界 = 源码 const:≥90 透支 / ≥80 疲惫 / ≥60 疲倦 / &lt;20 休息充足 /
        /// 其余正常档恒 1f(无键硬编码)。</summary>
        public static float ValueMul(int fatigue, float rested, float tired, float weary, float exhausted)
        {
            if (fatigue >= ExhaustedAt) return exhausted;
            if (fatigue >= WearyAt) return weary;
            if (fatigue >= TiredAt) return tired;
            if (fatigue < RestedBelow) return rested;
            return 1f;
        }

        /// <summary>夜间疲劳增长(与 NightlyGain :254-268 逐字对齐;Enabled 分支归
        /// prefix,LevelMul/Scale 由注入方现取)。加权式 vaginal + anal +
        /// RoundToInt(oral × 0.5f)(口交加权 0.5f = 独立常量,非面板键;
        /// RoundToInt = 银行家舍入);weighted ≤ 0 与 gain ≤ 0 两处早退,
        /// 结果 Max(1,·) 下限。</summary>
        public static int NightlyGain(int vaginal, int oral, int anal, bool pregnant,
            float gainPerWeight, float levelMul, float scale)
        {
            int weighted = Math.Max(0, vaginal) + Math.Max(0, anal)
                + (int)Math.Round((double)Math.Max(0, oral) * 0.5, MidpointRounding.ToEven);
            if (weighted <= 0) return 0;
            float gain = (float)weighted * gainPerWeight * levelMul * (pregnant ? 1.5f : 1f) * scale;
            if (gain <= 0f) return 0;
            return Math.Max(1, (int)Math.Round((double)gain, MidpointRounding.ToEven));
        }

        /// <summary>透支恢复后的疲劳值(:124 Max(0, Get(item) − ExhaustRecover);
        /// 0 下限在此,上限 100 由源码 Set 的 clamp 兜底——复刻体经委托直调原版 Set)。</summary>
        public static int Recovered(int fatigue, int recover)
        {
            return Math.Max(0, fatigue - recover);
        }
    }
}
