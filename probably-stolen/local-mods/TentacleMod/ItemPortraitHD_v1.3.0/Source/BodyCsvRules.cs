using System;
using System.Globalization;
using System.Linq;

namespace ItemPortraitHD
{
    // Pure parsing and sampling rules. The caller supplies the eligible body
    // names; this class never keeps a second, hard-coded classification list.
    internal static class BodyCsvRules
    {
        internal static string Defaults(int count, double value) => string.Join(", ",
            Enumerable.Repeat(value.ToString(CultureInfo.InvariantCulture), count));

        internal static double[] Parse(string csv, int count, bool percentages, out string status)
        {
            double fallback = percentages ? 60 : 20;
            var values = Enumerable.Repeat(fallback, count).ToArray();
            status = "";
            if (count == 0) return values;
            string[] cells = (csv ?? "").Replace('，', ',').Split(',');
            if (cells.Length > count) status = "输入项数超过当前体型数，使用默认值。";
            else for (int i = 0; i < cells.Length; i++)
            {
                if (!double.TryParse(cells[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ||
                    double.IsNaN(value) || double.IsInfinity(value) || value < 0 || (percentages && value > 100))
                { status = percentages ? "概率须为 0–100 的数字，使用默认值。" : "权重须为非负有限数字，使用默认值。"; break; }
                values[i] = value;
            }
            if (status.Length == 0 && !percentages && values.All(v => v == 0))
                status = "体型权重不能全部为 0，使用默认值。";
            if (status.Length != 0) return Enumerable.Repeat(fallback, count).ToArray();
            if (cells.Length < count) status = "缺少的末尾项目已补默认值。";
            return values;
        }

        internal static double[] Distribution(string[] original, string[] eligible, double[] configured)
        {
            if (original == null || original.Length == 0) throw new ArgumentException("原模组体型列表为空。", nameof(original));
            var result = Enumerable.Repeat(1d, original.Length).ToArray();
            int count = 0;
            double max = 0;
            for (int i = 0; i < original.Length; i++)
            {
                int index = Array.IndexOf(eligible, original[i]);
                if (index < 0) continue;
                count++;
                max = Math.Max(max, configured[index]);
            }
            if (count == 0 || max == 0) return result;
            double total = 0;
            for (int i = 0; i < original.Length; i++)
            {
                int index = Array.IndexOf(eligible, original[i]);
                if (index >= 0) total += configured[index] / max;
            }
            // Preserve the total probability of the eligible subset. Only its
            // internal proportions change; every unlisted body keeps weight 1.
            for (int i = 0; i < original.Length; i++)
            {
                int index = Array.IndexOf(eligible, original[i]);
                if (index >= 0) result[i] = (configured[index] / max) / total * count;
            }
            return result;
        }

        internal static string Draw(string[] original, string[] eligible, double[] configured, float roll)
        {
            var weights = Distribution(original, eligible, configured);
            double target = (float.IsNaN(roll) ? 0 : Math.Max(0, Math.Min(1, roll))) * weights.Sum();
            string last = null;
            for (int i = 0; i < original.Length; i++)
            {
                if (weights[i] <= 0) continue;
                last = original[i];
                target -= weights[i];
                if (target < 0) return original[i];
            }
            return last;
        }
    }
}
