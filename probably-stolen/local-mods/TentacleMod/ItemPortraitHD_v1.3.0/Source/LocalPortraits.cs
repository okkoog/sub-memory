using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class LocalPortraits
    {
        private sealed class Pool
        {
            internal readonly Dictionary<string, WeightedTable<Source>> Images;
            internal readonly WeightedTable<string> Factions;
            internal Pool(IEnumerable<Source> sources)
            {
                var entries = sources.ToArray();
                Images = LocalPortraitRules.Factions.ToDictionary(f => f, f => new WeightedTable<Source>(entries,
                    s => PortraitWeights.Get(s.Id) * (double)LocalPortraitRules.Factor(s.LocalFaction, f)));
                Factions = new WeightedTable<string>(LocalPortraitRules.Factions, f => Images[f].Total);
            }
        }
        private static int catalogRevision = -1, weightsRevision = -1;
        private static readonly HashSet<string> failed = new HashSet<string>(StringComparer.Ordinal);
        private static readonly Dictionary<string, Pool> pools = new Dictionary<string, Pool>(StringComparer.Ordinal);
        internal static bool Eligible(Source source) => source != null && source.Id?.StartsWith("local:", StringComparison.Ordinal) == true &&
            LocalPortraitRules.IsProvider(source.Provider) && (source.LocalFaction == LocalPortraitRules.Unrestricted ||
            LocalPortraitRules.Factions.Contains(source.LocalFaction));

        private static void EnsurePools()
        {
            bool changed = catalogRevision != Catalog.LocalRevision;
            if (!changed && weightsRevision == PortraitWeights.Revision) return;
            if (changed) failed.Clear();
            pools.Clear();
            var candidates = Catalog.All.Where(s => Eligible(s) && !failed.Contains(s.Id)).ToArray();
            pools.Add("", new Pool(candidates));
            foreach (var group in candidates.GroupBy(s => s.Provider, StringComparer.Ordinal)) pools.Add(group.Key, new Pool(group));
            catalogRevision = Catalog.LocalRevision; weightsRevision = PortraitWeights.Revision;
        }

        internal static Source Draw(string provider, float factionRoll, float imageRoll, out string faction)
        {
            EnsurePools(); faction = null;
            if (!pools.TryGetValue(provider ?? "", out var pool)) return null;
            faction = pool.Factions.Draw(factionRoll);
            return faction == null ? null : pool.Images[faction].Draw(imageRoll);
        }

        internal static Source Pick(string provider, out string faction)
        {
            // Limit first-use decoding of broken files; valid decoded images use Portraits' shared HD cache.
            for (int attempt = 0; attempt < 4; attempt++)
            {
                var source = Draw(provider, UnityEngine.Random.value, UnityEngine.Random.value, out faction);
                if (source == null) return null;
                if (Portraits.Get(source)) return source;
                failed.Add(source.Id); weightsRevision = -1;
            }
            faction = null; return null;
        }

        internal static Source DrawInFaction(string provider, string faction, float imageRoll)
        {
            EnsurePools();
            return faction != null && pools.TryGetValue(provider ?? "", out var pool) &&
                pool.Images.TryGetValue(faction, out var images) ? images.Draw(imageRoll) : null;
        }

        internal static Source PickInFaction(string provider, string faction)
        {
            for (int attempt = 0; attempt < 4; attempt++)
            {
                var source = DrawInFaction(provider, faction, UnityEngine.Random.value);
                if (source == null) return null;
                if (Portraits.Get(source)) return source;
                failed.Add(source.Id); weightsRevision = -1;
            }
            return null;
        }
    }
}
