using System;
using System.Globalization;

namespace LewdMemeConfig
{
    /// <summary>
    /// 陷阱纯解析逻辑(Task 12):三档策略捕捉率 CSV → float[3],零游戏类型依赖,
    /// 供测试工程通配链接与 <see cref="TrapPatch"/> 复用。语义对照主 mod 1.6.1
    /// <c>MidnightPawn.Game.TentacleTrap.TACTIC_CHANCES = { 0.1f, 0.2f, 0.3f }</c>:
    /// 逗号分隔恰好 3 值、float.TryParse(InvariantCulture)全过且为有限值
    /// (NaN/±Infinity 拒绝)、每值 clamp [0,1]。
    /// 任何不满足(缺项/多项/非数字/空串/null)→ null = 调用方保持现值不动:
    /// nightly_once 回退主 mod 数组现值、tactic_write 跳过覆写(契约统一,
    /// 与 MaidenRoll.ParseIntRange 的"抛异常"风格不同——此处 null 即"保持",
    /// 失败不降级整条接管,只回退单一读数)。
    /// </summary>
    public static class TrapRules
    {
        /// <summary>
        /// 解析三档策略捕捉率 CSV(Review Focus 4):"0.1,0.2,0.3" → {0.1,0.2,0.3};
        /// 逐段 Trim(容忍手写空格,文档化决策);每值 clamp 到 [0,1]("2,0.2,0.3"
        /// → {1,0.2,0.3});null/空串/段数≠3/任一段 float.TryParse 失败/非有限值
        /// (NaN/±Infinity——TryParse 可成功但 NaN 绕过比较、∞ 仅被 clamp 压回,
        /// 均非合法概率)→ null。
        /// </summary>
        public static float[] ParseTacticChances(string csv)
        {
            if (string.IsNullOrEmpty(csv))
            {
                return null;
            }
            string[] parts = csv.Split(',');
            if (parts.Length != 3)
            {
                return null;
            }
            var values = new float[3];
            for (int i = 0; i < 3; i++)
            {
                if (!float.TryParse(parts[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out values[i]))
                {
                    return null;
                }
                // "NaN"/"Infinity" 能被 TryParse 成功解析:NaN 与任何比较均为 false
                // 会绕过两侧 clamp,∞ 则仅被 clamp 压回——均按非法输入整体拒绝
                if (float.IsNaN(values[i]) || float.IsInfinity(values[i]))
                {
                    return null;
                }
                if (values[i] < 0f) values[i] = 0f;
                if (values[i] > 1f) values[i] = 1f;
            }
            return values;
        }
    }
}
