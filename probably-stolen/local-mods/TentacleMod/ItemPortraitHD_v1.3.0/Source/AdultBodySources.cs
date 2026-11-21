using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ItemPortraitHD
{
    internal static class AdultBodySources
    {
        internal static readonly string[] Bodies = {"萝莉", "少女", "御姐", "少妇", "熟女" };
        private sealed class Settings { public bool Enabled; public string[] Providers; }
        private static string settingsPath;
        internal static readonly string[] Breasts = { "贫乳", "普乳", "巨乳" };
        private static readonly float[,] BreastProbabilities =
        {
            { .60f, .35f, .05f },
            { .50f, .40f, .10f },
            { .20f, .45f, .35f },
            { .20f, .40f, .40f },
            { .10f, .40f, .50f }
        };

        internal static void Load()
        {
            var mod = ModEntry.Instance;
            mod.BodySourcesEnabled = false;
            mod.BodyProviders = new string[Bodies.Length];
            settingsPath = Path.Combine(ModHelper.GetModDataFolder(ModEntry.Id), "BodySources.json");
            try
            {
                if (!File.Exists(settingsPath)) return;
                var saved = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
                if (saved == null) return;
                mod.BodySourcesEnabled = saved.Enabled;
                if (saved.Providers != null)
                    Array.Copy(saved.Providers, mod.BodyProviders, Math.Min(saved.Providers.Length, mod.BodyProviders.Length));
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }

        internal static void Save()
        {
            try
            {
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(settingsPath));
                File.WriteAllText(settingsPath, JsonConvert.SerializeObject(new Settings
                { Enabled = ModEntry.Instance.BodySourcesEnabled, Providers = ModEntry.Instance.BodyProviders }, Formatting.Indented));
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }

        internal static string PickBody(string[] original)
        {
            return BodyCsvRules.Draw(original, Bodies, BodyCsvSettings.Weights, UnityEngine.Random.value);
        }

        internal static string PickBreastForBody(string[] original, string body)
        {
            var mod = ModEntry.Instance;
            if (mod?.BodyBreastEnabled?.Value != true)
                return original[UnityEngine.Random.Range(0, original.Length)];

            int bodyIndex = Array.IndexOf(new[] { "萝莉", "少女", "御姐", "少妇", "熟女" }, body);
            if (bodyIndex < 0) return original[UnityEngine.Random.Range(0, original.Length)];
            return DrawBreast(bodyIndex, UnityEngine.Random.value);
        }

        internal static string DrawBreast(int bodyIndex, float roll)
        {
            if (bodyIndex < 0 || bodyIndex >= BreastProbabilities.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(bodyIndex));
            float target = Math.Max(0, Math.Min(1, roll));
            for (int i = 0; i < Breasts.Length; i++)
            {
                target -= BreastProbabilities[bodyIndex, i];
                if (target < 0) return Breasts[i];
            }
            return Breasts[Breasts.Length - 1];
        }

        internal static bool PoolKey(string key) => RandomPortraits.IsCaptureKey(key);
        internal static bool Eligible(Source source) => source != null && !string.IsNullOrEmpty(source.Provider) &&
            (LocalPortraits.Eligible(source) ||
            (PortraitRules.IsMappedReplacementId(source.Id, source.MappedCharacter) && PoolKey(source.CharacterKey)));
        internal static string Provider(string body)
        {
            var mod = ModEntry.Instance;
            int index = Array.IndexOf(Bodies, body);
            return mod?.BodySourcesEnabled == true && index >= 0 ? mod.BodyProviders[index] ?? "" : "";
        }

        internal static bool IsAdultBody(string body) => Bodies.Contains(body);

        internal static bool ShouldBeDoubleVirgin(string body, float chance, float roll)
        {
            if (!IsAdultBody(body)) return false;
            chance = Math.Max(0, Math.Min(100, chance));
            return chance == 100 || (chance > 0 && roll < chance / 100f);
        }

        internal static List<Source> Filter(IEnumerable<Source> sources, string provider) => sources
            .Where(s => string.Equals(s.Provider, provider, StringComparison.Ordinal)).ToList();
    }
}
