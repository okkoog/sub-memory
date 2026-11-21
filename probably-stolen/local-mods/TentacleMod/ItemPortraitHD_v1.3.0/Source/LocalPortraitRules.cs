using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemPortraitHD
{
    internal static class LocalPortraitRules
    {
        internal static readonly string[] Factions = { "上层区", "下层区", "治安部", "黑市", "革命军" };
        internal const string Unrestricted = "不限阵营";
        internal const string ProviderPrefix = "local-folder:";

        internal static bool TryClassify(string relative, out string provider, out string faction)
        {
            provider = faction = null;
            var parts = relative.Replace('\\', '/').Split('/');
            if (parts.Length < 2 || string.IsNullOrWhiteSpace(parts[0])) return false;
            provider = ProviderPrefix + parts[0];
            if (parts.Length < 3 || (!Factions.Contains(parts[1]) && parts[1] != Unrestricted)) return false;
            faction = parts[1];
            return true;
        }

        internal static float Factor(string category, string faction) => !Factions.Contains(faction) ? 0 :
            category == Unrestricted ? .2f : category == faction ? 1 : 0;
        internal static bool IsProvider(string provider) => provider?.StartsWith(ProviderPrefix, StringComparison.Ordinal) == true;
        internal static bool UseLocal(string provider, int chance, float roll) => IsProvider(provider) ||
            (string.IsNullOrEmpty(provider) && PortraitRules.LocalHit(chance, roll));
    }

    // Immutable cumulative weights: built on catalog/weight changes, O(log n) per draw.
    internal sealed class WeightedTable<T>
    {
        private readonly T[] items;
        private readonly double[] cumulative;
        internal double Total { get; }
        internal WeightedTable(IEnumerable<T> candidates, Func<T, double> weight)
        {
            var values = new List<T>();
            var sums = new List<double>();
            double total = 0;
            foreach (var item in candidates)
            {
                double w = weight(item);
                if (w <= 0 || double.IsNaN(w) || double.IsInfinity(w)) continue;
                total += w; values.Add(item); sums.Add(total);
            }
            items = values.ToArray(); cumulative = sums.ToArray(); Total = total;
        }
        internal T Draw(float roll)
        {
            if (items.Length == 0) return default;
            double target = Math.Max(0, Math.Min(1, roll)) * Total;
            int low = 0, high = items.Length - 1;
            while (low < high)
            {
                int mid = low + (high - low) / 2;
                if (target < cumulative[mid]) high = mid; else low = mid + 1;
            }
            return items[low];
        }
    }
}
