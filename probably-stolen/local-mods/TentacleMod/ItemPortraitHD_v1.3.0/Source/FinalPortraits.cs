using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    // Classification uses original character keys; rendering consumes the final
    // GetSprite result after every enabled provider's hooks have run.
    internal static class FinalPortraits
    {
        internal sealed class Pool
        {
            internal readonly Dictionary<string, WeightedTable<string>> Characters;
            internal readonly WeightedTable<string> Factions;
            internal Pool(IEnumerable<string> keys, bool femaleOnly)
            {
                var roles = keys.Distinct(StringComparer.Ordinal)
                    .Where(k => !femaleOnly || PortraitRules.Female(k)).ToArray();
                Characters = LocalPortraitRules.Factions.ToDictionary(f => f,
                    f => new WeightedTable<string>(roles, k => PortraitRules.FactionWeight(k, f)));
                Factions = new WeightedTable<string>(LocalPortraitRules.Factions, f => Characters[f].Total);
            }
        }

        private static Pool pool;
        private static IReadOnlyList<string> registered;
        private static bool female;
        private static readonly Queue<string> scan = new Queue<string>();
        private static int scanTotal, scanDone, scanErrors;
        internal static bool Scanning => scan.Count > 0;
        internal static string ScanStatus => scanTotal > 0 ? "最终图片 " + scanDone + "/" + scanTotal + (scanErrors > 0 ? "，失败 " + scanErrors : "") : "";

        internal static void StartScan()
        {
            ReplacementTrace.Install();
            scan.Clear(); scanDone = scanErrors = 0;
            foreach (string key in RandomPortraits.Keys()) scan.Enqueue(key);
            scanTotal = scan.Count;
        }

        internal static void StopScan() { scan.Clear(); scanTotal = scanDone = scanErrors = 0; }

        internal static void Tick(bool managerOpen)
        {
            if (!managerOpen || !Scanning || !SpriteDict.Instance) return;
            string key = scan.Dequeue();
            try { Catalog.ObserveFinal(key, SpriteDict.Instance.GetSprite(key)); }
            catch (Exception ex) { scanErrors++; ModEntry.Warn(ex); }
            scanDone++;
        }

        private static Pool Current()
        {
            bool femaleOnly = ModEntry.Instance.PortraitGender.Value != "不限性别";
            var keys = RandomPortraits.CaptureKeys();
            if (pool == null || !ReferenceEquals(registered, keys) || female != femaleOnly)
            {
                pool = new Pool(keys, femaleOnly);
                registered = keys; female = femaleOnly;
            }
            return pool;
        }

        internal static string DrawFaction(float roll) => Current().Factions.Draw(roll);
        internal static string DrawCharacter(Pool table, string faction, float roll) =>
            faction != null && table.Characters.TryGetValue(faction, out var roles) ? roles.Draw(roll) : null;

        internal static Source SourceFor(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            Sprite sprite = SpriteDict.Instance ? SpriteDict.Instance.GetSprite(key) : null;
            // A missing/failed live result must not silently bypass providers by
            // switching to the serialized original. Pick can try another role.
            if (!sprite) return null;
            // Freeze the actual selection. A provider may choose a different image
            // on its next GetSprite call; display and encoding must reuse this one.
            return new Source { Id = "final:" + key, Name = key + "（最终生效）", CharacterKey = key,
                Group = "最终生效立绘", Resolve = () => sprite };
        }

        internal static Source Pick(string faction)
        {
            var table = Current();
            for (int attempt = 0; attempt < 4; attempt++)
            {
                string key = DrawCharacter(table, faction, UnityEngine.Random.value);
                if (key == null) return null;
                var source = SourceFor(key);
                if (source != null && Portraits.Get(source)) return source;
            }
            return null;
        }
    }
}
