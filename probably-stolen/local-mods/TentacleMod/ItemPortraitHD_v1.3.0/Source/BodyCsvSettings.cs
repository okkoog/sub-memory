using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HarmonyLib;

namespace ItemPortraitHD
{
    internal static class BodyCsvSettings
    {
        private static string cachedBodies, cachedWeights, cachedChances;
        private static double[] weights, chances;
        private static string weightStatus, chanceStatus;
        internal static string Order => string.Join("、", AdultBodySources.Bodies);
        internal static string WeightDefault => BodyCsvRules.Defaults(AdultBodySources.Bodies.Length, 20);
        internal static string ChanceDefault => BodyCsvRules.Defaults(AdultBodySources.Bodies.Length, 60);

        internal static ConfigEntry<string> DefineWeights(ModConfig config) => config.Define(
            "body_weights_csv", WeightDefault, "体型来源", () => "成年体型抽取权重（逗号分隔）", (string[])null,
            () => "顺序：" + Order + "。默认 " + WeightDefault + "。支持小数；0 表示不选该项，不能全为 0。只调整这些体型内部的相对比例，整体占比及其他体型概率保持原版。只影响后续新生成物品。" + Status(false));

        internal static ConfigEntry<string> DefineChances(ModConfig config)
        {
            // Read legacy settings once without registering their old sliders.
            // Defaults remain 60; saved values are migrated only when no CSV exists.
            var raw = AccessTools.Field(typeof(ModConfig), "rawValues")?.GetValue(config) as Dictionary<string, string>;
            string migrated = LegacyChances(raw, AdultBodySources.Bodies.Length);
            var entry = config.Define("scavenger_double_virgin_csv", ChanceDefault, "体型来源",
                () => "成年体型双处女概率（%，逗号分隔）", (string[])null,
                () => "顺序：" + Order + "。默认 " + ChanceDefault + "。每项是独立的 0–100% 概率，支持小数，不要求合计 100。仅作用于拾荒生成且属于当前成年体型列表的新角色。" + Status(true));
            if (migrated != null) entry.Set(migrated);
            return entry;
        }

        internal static string LegacyChances(Dictionary<string, string> raw, int count)
        {
            if (raw == null || raw.ContainsKey("scavenger_double_virgin_csv") ||
                !Enumerable.Range(0, count).Any(i => raw.ContainsKey("scavenger_double_virgin_" + i))) return null;
            return string.Join(", ", Enumerable.Range(0, count).Select(i =>
                raw.TryGetValue("scavenger_double_virgin_" + i, out string value) &&
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int chance)
                    ? Math.Max(0, Math.Min(100, chance)).ToString(CultureInfo.InvariantCulture) : "60"));
        }

        private static void Ensure()
        {
            string bodies = string.Join("\0", AdultBodySources.Bodies);
            string weightText = ModEntry.Instance?.BodyWeights?.Value ?? WeightDefault;
            string chanceText = ModEntry.Instance?.BodyVirginChances?.Value ?? ChanceDefault;
            bool changed = cachedBodies != bodies;
            if (changed || weights == null || cachedWeights != weightText)
            {
                weights = BodyCsvRules.Parse(weightText, AdultBodySources.Bodies.Length, false, out weightStatus);
                cachedWeights = weightText;
            }
            if (changed || chances == null || cachedChances != chanceText)
            {
                chances = BodyCsvRules.Parse(chanceText, AdultBodySources.Bodies.Length, true, out chanceStatus);
                cachedChances = chanceText;
            }
            cachedBodies = bodies;
        }

        private static string Status(bool percentages) { Ensure(); return percentages ? chanceStatus : weightStatus; }
        internal static double[] Weights { get { Ensure(); return weights; } }
        internal static float Chance(string body)
        {
            int index = Array.IndexOf(AdultBodySources.Bodies, body);
            if (index < 0) return 0;
            Ensure(); return (float)chances[index];
        }
    }
}
