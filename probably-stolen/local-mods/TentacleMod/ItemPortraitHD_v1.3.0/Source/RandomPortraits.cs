using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class RandomPortraits
    {
        internal const string Prefix = "iph|";
        private static readonly Dictionary<string, Source> selected = new Dictionary<string, Source>();
        private static readonly Dictionary<string, List<Source>> replacementCache = new Dictionary<string, List<Source>>(StringComparer.Ordinal);
        private static Dictionary<string, List<Source>> mappedFiles;
        private static SpriteDict nativeOwner;
        private static readonly Dictionary<string, Sprite> nativeSprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        private static string[] captureKeys;
        private static HashSet<string> captureKeySet;
        private static readonly System.Reflection.FieldInfo[] nativeFields = typeof(SpriteDict).GetFields(System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).Where(f => f.FieldType == typeof(List<SpriteDict.SpriteEntry>)).ToArray();
        internal static void InvalidateSources()
        {
            // Final selections are frozen Sprite snapshots; stable provider IDs are re-resolved.
            foreach (string id in selected.Keys.Where(id => !id.StartsWith("final:", StringComparison.Ordinal)).ToArray())
                selected.Remove(id);
            InvalidateReplacements(); mappedFiles = null; AllGirlsSources.Clear(); RimworldSources.Clear();
        }
        internal static void InvalidateReplacements() { replacementCache.Clear(); replacementRevision++; }
        internal static void InvalidateReplacement(string key) { replacementCache.Remove(key); replacementRevision++; }
        internal static void InvalidateNative() { nativeOwner = null; nativeSprites.Clear(); captureKeys = null; captureKeySet = null; }
        private static readonly string[] factions = LocalPortraitRules.Factions;
        private static int replacementRevision;
        private static int poolRevision = -1, poolWeights = -1;
        private static IReadOnlyList<string> poolKeys;
        private static bool poolFemale;
        private static Dictionary<string, List<Source>> poolReplacements;
        private static Dictionary<string, WeightedTable<string>> roleTables;
        private static WeightedTable<string> factionTable;
        [ThreadStatic] private static Source pending;
        [ThreadStatic] private static string pendingFaction;
        internal static void ResetPending() { pending = null; pendingFaction = null; }

        internal static Source Find(string id)
        {
            if (!id.StartsWith(Prefix, StringComparison.Ordinal)) return null;
            id = id.Substring(Prefix.Length);
            if (id.StartsWith("trace:", StringComparison.Ordinal)) return ReplacementTrace.Find(id);
            if (selected.TryGetValue(id, out var source)) return source;
            if (id.StartsWith("vanilla:", StringComparison.Ordinal)) return Native(id.Substring(8));
            if (id.StartsWith("final:", StringComparison.Ordinal)) return FinalPortraits.SourceFor(id.Substring(6));
            source = Catalog.Find(id);
            if (source != null) return source;
            source = AllGirlsSources.Restore(id);
            if (source != null) { selected[id] = source; return source; }
            source = RimworldSources.Restore(id);
            if (source != null) { selected[id] = source; return source; }
            source = ProviderMaps.RestoreEmbedded(id);
            if (source != null) { selected[id] = source; return source; }
            if (mappedFiles == null) mappedFiles = ProviderMaps.Scan();
            source = mappedFiles.Values.SelectMany(s => s).FirstOrDefault(s => s.Id == id);
            if (source != null) { selected[id] = source; return source; }
            return null;
        }

        internal static List<string> Keys()
        {
            if (SpriteDict.Instance && SpriteDict.Instance.spriteDictionary is Dictionary<string, Sprite> dict)
                return dict.Where(p => p.Value).Select(p => p.Key).OrderBy(k => k, StringComparer.Ordinal).ToList();
            return new List<string>();
        }

        // Scanning/restoring uses the live dictionary above. Automatic selection
        // only uses registered game roles, never provider-added keys or aliases.
        internal static string[] BuildCaptureKeys(IEnumerable<string> registered) => registered
            .Where(k => !string.IsNullOrEmpty(k) && factions.Any(f => PortraitRules.FactionWeight(k, f) > 0))
            .Distinct(StringComparer.Ordinal).OrderBy(k => k, StringComparer.Ordinal).ToArray();

        internal static IReadOnlyList<string> CaptureKeys()
        {
            EnsureNativeSprites();
            return captureKeys ?? Array.Empty<string>();
        }

        internal static bool IsCaptureKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            EnsureNativeSprites();
            return captureKeySet?.Contains(key) == true;
        }

        internal static IEnumerable<Source> ManualSources()
        {
            if (mappedFiles == null) mappedFiles = ProviderMaps.Scan();
            return mappedFiles.TryGetValue("", out var sources) ? sources : Enumerable.Empty<Source>();
        }

        internal static Source Native(string key) => new Source { Id = "vanilla:" + key,
            Name = key, Group = "游戏原版", Resolve = () => NativeSprite(key) };

        private static Sprite NativeSprite(string key)
        {
            EnsureNativeSprites();
            return nativeSprites.TryGetValue(key, out var sprite) ? sprite : null;
        }

        private static void EnsureNativeSprites()
        {
            var owner = SpriteDict.Instance;
            if (!owner) { InvalidateNative(); return; }
            if (!ReferenceEquals(owner, nativeOwner))
            {
                nativeOwner = owner; nativeSprites.Clear();
                foreach (var field in nativeFields)
                    if (field.GetValue(owner) is List<SpriteDict.SpriteEntry> entries)
                        foreach (var entry in entries)
                            if (entry != null && entry.sprite && !nativeSprites.ContainsKey(entry.key)) nativeSprites.Add(entry.key, entry.sprite);
                captureKeys = BuildCaptureKeys(nativeSprites.Keys);
                captureKeySet = new HashSet<string>(captureKeys, StringComparer.Ordinal);
            }
        }

        internal static string PickFaction(string[] original, string body)
        {
            using (PerformanceTrace.Measure("捕获/阵营选角")) return PickFactionCore(original, body);
        }

        private static string PickFactionCore(string[] original, string body)
        {
            pending = null; pendingFaction = null;
            if (ModEntry.Instance?.Active != true)
                return original[UnityEngine.Random.Range(0, original.Length)];
            if (ModEntry.UseFinalPortraitSelection) return PickFinalFaction(original, body);
            if (Catalog.All.Count == 0) Catalog.Scan();
            string provider = AdultBodySources.Provider(body);
            // An explicit local provider wins over the folder probability; an explicit
            // replacement mod keeps its own routing. Unrestricted bodies merge all folders.
            bool localProvider = LocalPortraitRules.IsProvider(provider);
            if (LocalPortraitRules.UseLocal(provider, ModEntry.Instance.LocalChance.Value, UnityEngine.Random.value))
            {
                pending = LocalPortraits.Pick(localProvider ? provider : "", out pendingFaction);
                if (pending != null) return pendingFaction;
            }
            bool femaleOnly = ModEntry.Instance.PortraitGender.Value != "不限性别";
            // Do not enumerate replacement providers for thousands of unrelated
            // item/icon keys which can never enter any of the five factions.
            EnsurePool(CaptureKeys(), femaleOnly);
            string DrawFaction() => (ModEntry.Instance.WeightedFactions.Value ?
                factionTable.Draw(UnityEngine.Random.value) : null) ?? original[UnityEngine.Random.Range(0, original.Length)];

            pendingFaction = DrawFaction();
            pending = PortraitRules.RetryPortrait(
                () => roleTables.TryGetValue(pendingFaction, out var roles) ? roles.Draw(UnityEngine.Random.value) : null,
                key => {
                    if (key == null) return null;
                    var replacements = poolReplacements[key];
                    return replacements.Count > 0 ? ValidRandom(replacements) : ReadNative(key);
                }, ReadNative);
            return pendingFaction;
        }

        private static void EnsurePool(IReadOnlyList<string> registered, bool femaleOnly)
        {
            if (ReferenceEquals(poolKeys, registered) && poolFemale == femaleOnly &&
                poolRevision == replacementRevision && poolWeights == PortraitWeights.Revision) return;
            var keys = registered.Where(k => !femaleOnly || PortraitRules.Female(k)).ToArray();
            poolReplacements = keys.ToDictionary(k => k, Replacements);
            var weights = keys.ToDictionary(k => k, k => PortraitRules.EffectiveRoleWeight(
                PortraitWeights.Get("game:" + k), poolReplacements[k].Select(s => PortraitWeights.Get(s.Id)), poolReplacements[k].Count > 0));
            roleTables = factions.ToDictionary(f => f, f =>
                new WeightedTable<string>(keys, k => PortraitRules.FactionWeight(k, f) * (double)weights[k]));
            factionTable = new WeightedTable<string>(factions, f => roleTables[f].Total);
            poolKeys = registered; poolFemale = femaleOnly;
            poolRevision = replacementRevision; poolWeights = PortraitWeights.Revision;
        }

        private static Source ReadNative(string key)
        {
            if (key == null) return null;
            var source = Native(key);
            return Portraits.Get(source) ? source : null;
        }

        internal static string Pick(GameItem item)
        {
            var source = pending;
            var faction = pendingFaction;
            pending = null; pendingFaction = null;
            string actualFaction = ModEntry.Get(item, "MAIDEN_FACTION");
            // The factory may enter its fallback after the first injection has
            // consumed pending. Keep that retry in the final-selection pipeline,
            // instead of escaping into the dependency's unrelated portrait pool.
            if (source == null || faction != actualFaction)
                return ModEntry.UseFinalPortraitSelection
                    ? PickAssigned(actualFaction, ModEntry.Get(item, "MAIDEN_BODY")) : null;
            string provider = AdultBodySources.Provider(ModEntry.Get(item, "MAIDEN_BODY"));
            if (!ModEntry.UseFinalPortraitSelection && !string.IsNullOrEmpty(provider) && !LocalPortraitRules.IsProvider(provider) &&
                source?.Id.StartsWith("local:", StringComparison.Ordinal) != true)
                source = PickProvider(faction, provider) ?? source;
            return Remember(source);
        }

        // Traps already have a deployment faction. Route an explicitly assigned adult
        // provider within that faction, without drawing or changing the faction again.
        internal static string PickAssigned(string faction, string body)
        {
            if (ModEntry.Instance?.Active != true || ModEntry.UseOriginalPortraitSelection) return null;
            string provider = AdultBodySources.Provider(body);
            Source source = null;
            if (!string.IsNullOrEmpty(provider))
            {
                EnsureCatalog();
                source = LocalPortraitRules.IsProvider(provider)
                    ? LocalPortraits.PickInFaction(provider, faction) : PickProvider(faction, provider);
            }
            if (source == null && ModEntry.UseFinalPortraitSelection)
                source = PickFinalInFaction(faction);
            return Remember(source);
        }

        private static Source PickFinalInFaction(string faction)
        {
            if (PortraitRules.LocalHit(ModEntry.Instance.LocalChance.Value, UnityEngine.Random.value))
            {
                EnsureCatalog();
                var source = LocalPortraits.PickInFaction("", faction);
                if (source != null) return source;
            }
            return FinalPortraits.Pick(faction);
        }

        private static void EnsureCatalog() { if (Catalog.All.Count == 0) Catalog.Scan(); }

        private static string PickFinalFaction(string[] original, string body)
        {
            string provider = AdultBodySources.Provider(body);
            // Resolve explicit providers before consulting the PNG chance or final portraits.
            if (!string.IsNullOrEmpty(provider))
            {
                EnsureCatalog();
                if (LocalPortraitRules.IsProvider(provider))
                    pending = LocalPortraits.Pick(provider, out pendingFaction);
                else
                {
                    bool femaleOnly = ModEntry.Instance.PortraitGender.Value != "不限性别";
                    var roles = CaptureKeys().Where(k => !femaleOnly || PortraitRules.Female(k))
                        .Select(k => new { Key = k, Sources = AdultBodySources.Filter(Replacements(k), provider) }).ToList();
                    var faction = ModEntry.Instance.WeightedFactions.Value
                        ? PortraitRules.Weighted(factions, f => roles.Sum(r => PortraitRules.FactionWeight(r.Key, f) *
                            r.Sources.Sum(s => PortraitWeights.Get(s.Id))), UnityEngine.Random.value)
                        : original[UnityEngine.Random.Range(0, original.Length)];
                    pendingFaction = faction;
                    if (faction != null) pending = PickProvider(faction, provider);
                }
                if (pending != null) return pendingFaction;
            }
            if (PortraitRules.LocalHit(ModEntry.Instance.LocalChance.Value, UnityEngine.Random.value))
            {
                EnsureCatalog();
                pending = LocalPortraits.Pick("", out pendingFaction);
                if (pending != null) return pendingFaction;
            }
            pendingFaction = (ModEntry.Instance.WeightedFactions.Value
                ? FinalPortraits.DrawFaction(UnityEngine.Random.value) : null) ??
                original[UnityEngine.Random.Range(0, original.Length)];
            pending = FinalPortraits.Pick(pendingFaction);
            return pendingFaction;
        }

        private static Source PickProvider(string faction, string provider)
        {
            bool femaleOnly = ModEntry.Instance.PortraitGender.Value != "不限性别";
            var roles = CaptureKeys().Where(k => (!femaleOnly || PortraitRules.Female(k)) && PortraitRules.FactionWeight(k, faction) > 0)
                .Select(k => new { Key = k, Sources = AdultBodySources.Filter(Replacements(k), provider) }).ToList();
            while (roles.Count > 0)
            {
                var role = PortraitRules.Weighted(roles, r => PortraitRules.FactionWeight(r.Key, faction) *
                    r.Sources.Sum(s => PortraitWeights.Get(s.Id)), UnityEngine.Random.value);
                if (role == null) break;
                var source = ValidRandom(role.Sources);
                if (source != null) return source;
                roles.Remove(role);
            }
            return null;
        }

        private static Source ValidRandom(List<Source> sources)
        {
            sources = sources.Where(s => PortraitWeights.Get(s.Id) > 0).ToList();
            while (sources.Count > 0)
            {
                var source = PortraitRules.Weighted(sources, s => PortraitWeights.Get(s.Id), UnityEngine.Random.value);
                if (Portraits.Get(source)) return source;
                sources.Remove(source);
            }
            return null;
        }

        private static string Remember(Source source)
        {
            if (source == null) return null;
            selected[source.Id] = source;
            return Prefix + source.Id;
        }

        internal static List<Source> Replacements(string key)
        {
            if (replacementCache.TryGetValue(key, out var cached)) return cached;
            var sources = FindReplacements(key);
            replacementCache[key] = sources;
            return sources;
        }

        private static List<Source> FindReplacements(string key)
        {
            var result = new List<Source>();
            string normalized = PortraitRules.Normalize(key);
            if (mappedFiles == null) mappedFiles = ProviderMaps.Scan();
            if (mappedFiles.TryGetValue(normalized, out var mapped)) result.AddRange(mapped.Where(s => s.MatchSpriteName == null));
            var native = Native(key).Resolve();
            if (native && mappedFiles.TryGetValue(PortraitRules.Normalize(native.name), out var byName))
                result.AddRange(byName.Where(s => s.MatchSpriteName != null &&
                    MappingRules.MatchesSlot(s.MatchSpriteName, s.MatchReadable, native.name, native.texture && native.texture.isReadable)));
            foreach (var mod in ModLoader.Mods.Where(m => m.IsEnabled && m.Manifest != null &&
                m.Manifest.ID != ModEntry.Id && m.Manifest.ID != "lewd_meme" && m.Manifest.ID != "lewd_meme_remake"))
            {
                if (mod.Sprites == null) continue;
                foreach (var pair in mod.Sprites.Where(p => p.Value && PortraitRules.Normalize(p.Key) == normalized))
                {
                    var capturedMod = mod; string spriteKey = pair.Key;
                    if (result.Any(s => s.Id == "mod:" + mod.Manifest.ID + ":" + spriteKey)) continue;
                    result.Add(new Source { Id = "mod:" + mod.Manifest.ID + ":" + spriteKey,
                        Provider = mod.Manifest.ID, Name = spriteKey, Group = mod.Manifest.Name, CharacterKey = key,
                        Resolve = () => capturedMod.IsEnabled && capturedMod.Sprites.TryGetValue(spriteKey, out var s) ? s : null });
                }
            }
            // Prefer persistent provider files/maps. A trace is a fallback for a
            // provider-role pair, never another weight for that same replacement.
            var mappedProviders = result.Where(s => s.Provider != null).Select(s => s.Provider).ToHashSet(StringComparer.Ordinal);
            result.AddRange(ReplacementTrace.Sources.Where(s => s.CharacterKey == key && !mappedProviders.Contains(s.Provider)));
            return result.GroupBy(s => s.File ?? s.Id, StringComparer.OrdinalIgnoreCase).Select(g => g.First()).ToList();
        }
    }
}
