using System;

namespace LewdMemeConfig
{
    /// <summary>
    /// 数值换算纯函数(Task 10)。ApplyPercent 为 brief 规定的唯一可测换算
    /// (value×(1+percent/100),四舍五入);PercentToInt 供
    /// client.semen_price_mod/bonus_mod(float 键)喂给游戏侧
    /// <c>ItemFeature.SetPreExposedValueMod(int, bool)</c>;PoolSizeClamp 供
    /// client.pool_size 接管(概率分母/入池上限,下限 1)。
    /// 取整方向 = 传统四舍五入(.5 远离零,MidpointRounding.AwayFromZero,
    /// 非 Unity Mathf.Round 的银行家舍入——brief 明文"四舍五入")。
    /// 零游戏类型(Logic/ 通配进测试工程),纯 System 数学。
    /// </summary>
    public static class ValueMath
    {
        /// <summary>
        /// 百分比修正:round(value × (1 + percent/100))。
        /// 默认档示例(ApplyPercent 语义锚点):100→-15% = 85、200→+15% = 230、
        /// 90→0% = 90;半点(如 100×1.125=112.5)远离零取整 → 113。
        /// </summary>
        public static int ApplyPercent(int value, float percent)
        {
            double adjusted = value * (1.0 + (double)percent / 100.0);
            return (int)Math.Round(adjusted, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 百分比配置值(float)→ 整数百分比(SetPreExposedValueMod 参数类型)。
        /// 默认 -15f/15f → -15/15 与主 mod 字面量一致;半点远离零(-12.5 → -13)。
        /// </summary>
        public static int PercentToInt(float percent)
        {
            return (int)Math.Round((double)percent, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 角色池上限钳制:n &lt; 1 视为 1(ClientPool.Resolve 接管体里既是
        /// 复用概率分母又是入池上限,0/负数会产生概率 ∞ 或永不入池的悖论语义;
        /// 1 = 池内恒定复用唯一角色,语义自洽)。
        /// </summary>
        public static int PoolSizeClamp(int n)
        {
            return n < 1 ? 1 : n;
        }
    }
}
