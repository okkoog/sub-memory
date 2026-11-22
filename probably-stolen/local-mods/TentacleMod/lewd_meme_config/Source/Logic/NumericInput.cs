using System.Globalization;

namespace LewdMemeConfig
{
    /// <summary>
    /// 数值文本框的解析/钳制/格式化纯函数(开关体系与 IPH 兼容 Task 1)。
    /// 动机:游戏滑槽显示数值不准(玩家反馈),面板数值键改精确文本输入;
    /// 本类承担 string ⇄ 数值 的全部转换,InvariantCulture 全程(与 CSV 键
    /// 解析一致)。容错契约:解析失败返回 false 不抛(调用方回退默认+日志一次);
    /// 解析成功钳制到 [min,max](对齐原滑块防呆语义)。零游戏类型依赖,
    /// 测试工程通配编译覆盖。
    /// </summary>
    public static class NumericInput
    {
        /// <summary>当前生效值 → 文本框初始串(InvariantCulture,整值 float 去 .0)。</summary>
        public static string Format(int v) => v.ToString(CultureInfo.InvariantCulture);

        public static string Format(float v) => v.ToString(CultureInfo.InvariantCulture);

        /// <summary>文本 → int:Trim 后 InvariantCulture 解析;null/空/非整数 false;
        /// 成功钳制 [min,max]。失败时 out 置 min。</summary>
        public static bool TryParseInt(string s, int min, int max, out int v)
        {
            if (!string.IsNullOrEmpty(s) &&
                int.TryParse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
            {
                if (v < min) v = min;
                if (v > max) v = max;
                return true;
            }
            v = min;
            return false;
        }

        /// <summary>文本 → float:Trim 后解析;null/空/非数/NaN/±Infinity false
        /// (NaN 与任何比较均为 false 会绕过两侧钳制,∞ 仅被压回——均按非法拒,
        /// 同 TrapRules.ParseTacticChances 先例);成功钳制 [min,max]。</summary>
        public static bool TryParseFloat(string s, float min, float max, out float v)
        {
            if (!string.IsNullOrEmpty(s) &&
                float.TryParse(s.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out v) &&
                !float.IsNaN(v) && !float.IsInfinity(v))
            {
                if (v < min) v = min;
                if (v > max) v = max;
                return true;
            }
            v = min;
            return false;
        }

        /// <summary>面板 ⓘ tooltip 的范围/类型标注(玩家可见,InvariantCulture 格式化)。</summary>
        public static string RangeNoteInt(int min, int max)
        {
            return "[范围 " + Format(min) + "-" + Format(max) + ",整数]";
        }

        public static string RangeNoteFloat(float min, float max)
        {
            return "[范围 " + Format(min) + "-" + Format(max) + ",小数]";
        }
    }
}
