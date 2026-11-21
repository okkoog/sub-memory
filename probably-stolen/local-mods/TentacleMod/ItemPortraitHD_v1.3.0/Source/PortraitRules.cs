using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemPortraitHD
{
    internal static class PortraitRules
    {
        internal static string Normalize(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return "";
            if (key.StartsWith("cp_portrait_", StringComparison.OrdinalIgnoreCase)) key = key.Substring(12);
            return key.Replace("_", "").ToLowerInvariant();
        }

        internal static string Faction(string key)
        {
            string k = Normalize(key);
            if (ExcludedCharacter(key)) return "";
            if (key?.IndexOf("_scav", StringComparison.OrdinalIgnoreCase) >= 0) return "下层区";
            if (Starts(k, "maleupper", "femaleupper")) return "上层区";
            if (Starts(k, "malelower", "femalelower", "malescav", "femalescav")) return "下层区";
            if (Starts(k, "malesec", "femalesec") || k == "sec" ||
                (k.StartsWith("sec", StringComparison.Ordinal) && k.Substring(3).All(char.IsDigit))) return "治安部";
            if (Starts(k, "malebm", "femalebm", "malemerc", "femalemerc")) return "黑市";
            if (Starts(k, "malerev", "femalerev")) return "革命军";
            return "";
        }

        private static bool Starts(string key, params string[] prefixes) => prefixes.Any(p => key.StartsWith(p, StringComparison.Ordinal));
        private static bool ExcludedCharacter(string key) => Starts(Normalize(key), "eleanor", "serpico");
        internal static bool SharedWorker(string key)
        {
            string k = Normalize(key);
            if (ExcludedCharacter(key) || key?.IndexOf("_scav", StringComparison.OrdinalIgnoreCase) >= 0) return false;
            return Starts(k, "malechef", "femalechef", "maleeng", "femaleeng", "malefarmer", "femalefarmer",
                "malemed", "femalemed", "malesci", "femalesci", "miner");
        }
        internal static float FactionWeight(string key, string faction) => SharedWorker(key) ?
            (faction == "上层区" || faction == "下层区" ? .5f : 0) : Faction(key) == faction ? 1 : 0;

        internal static T Weighted<T>(IEnumerable<T> values, Func<T, float> weight, float roll) =>
            new WeightedTable<T>(values, v => weight(v)).Draw(roll);
        internal static float EffectiveRoleWeight(float vanillaWeight, IEnumerable<float> replacementWeights, bool hasReplacements) =>
            hasReplacements ? replacementWeights.Where(w => w >= 0 && !float.IsNaN(w) && !float.IsInfinity(w)).Sum() : Math.Max(0, vanillaWeight);
        internal static bool IsMappedReplacementId(string id, bool mapped) => mapped && id != null &&
            !id.StartsWith("game:", StringComparison.Ordinal) && !id.StartsWith("local:", StringComparison.Ordinal) &&
            !id.StartsWith("final:", StringComparison.Ordinal);
        // Keep the chosen faction and redraw its character. The fourth round
        // falls back to that same character's native portrait.
        internal static T RetryPortrait<K, T>(Func<K> draw, Func<K, T> replacement, Func<K, T> native) where T : class
        {
            for (int round = 0; round < 4; round++)
            {
                K key = draw();
                T source = replacement(key);
                if (source != null) return source;
                if (round == 3) return native(key);
            }
            return null;
        }

        internal static bool Female(string key) => Normalize(key).Contains("female");
        internal static bool LocalHit(int percent, float roll) => percent >= 100 || (percent > 0 && percent > roll * 100f);

    }
}
