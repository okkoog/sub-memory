using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class RimworldSources
    {
        internal const string Id = "probably_rimworld";
        private sealed class Entry { internal string Key, Resource, File; }
        private static Dictionary<string, Entry> entries;
        private static bool warned;
        internal static void Clear() { entries = null; warned = false; }
        private static LoadedMod Owner => ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Manifest?.ID == Id);

        private static Dictionary<string, Entry> Entries(LoadedMod owner)
        {
            if (entries != null) return entries;
            var reader = ModAccess.Type(Id, "ProbablyRimworld.ConfigurationReader");
            var validator = ModAccess.Type(Id, "ProbablyRimworld.ConfigurationValidator");
            if (reader == null || validator == null) return new Dictionary<string, Entry>();
            string path = Path.Combine(owner.FolderPath, "portraits.json");
            if (!File.Exists(path) || new FileInfo(path).Length > 1024 * 1024) return new Dictionary<string, Entry>();
            // A malformed optional mapping is retried on explicit reload rather
            // than reparsed for every final portrait in the same catalog scan.
            entries = new Dictionary<string, Entry>(StringComparer.Ordinal);
            // Reuse the provider's metadata-only approval checks, including its
            // exact runtime key and file binding. Never infer roles from PNG names.
            var parsed = ModReflect.CallStatic(reader, "Parse", File.ReadAllText(path));
            var validated = ModReflect.CallStatic(validator, "Validate", parsed, (Action<string>)(_ => { }));
            var rows = ModReflect.Get(validated, "Dictionary") as IEnumerable;
            var result = new Dictionary<string, Entry>(StringComparer.Ordinal);
            if (rows != null) foreach (var row in rows)
            {
                string key = ModReflect.Get<string>(row, "runtimeKey");
                string resource = ModReflect.Get<string>(row, "resourceKey");
                string relative = ModReflect.Get<string>(row, "relativePath");
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(resource) || string.IsNullOrWhiteSpace(relative)) continue;
                string root = Path.GetFullPath(Path.Combine(owner.FolderPath, "Portraits")) + Path.DirectorySeparatorChar;
                string file = Path.GetFullPath(Path.Combine(owner.FolderPath, relative));
                if (!file.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(file)) continue;
                result[key] = new Entry { Key = key, Resource = resource, File = file };
            }
            return entries = result;
        }

        private static Source SourceFor(LoadedMod owner, Entry entry) => new Source {
            Id = "rimworld:" + entry.Key, File = entry.File, Name = entry.Key, CharacterKey = entry.Key,
            Provider = Id, Group = owner.Manifest.Name ?? Id, MappedCharacter = true
        };

        internal static IEnumerable<Source> Scan()
        {
            var owner = Owner;
            if (owner == null) return Enumerable.Empty<Source>();
            return Entries(owner).Values.Select(e => SourceFor(owner, e)).ToArray();
        }

        internal static Source Restore(string id)
        {
            if (id == null || !id.StartsWith("rimworld:", StringComparison.Ordinal)) return null;
            var owner = Owner;
            if (owner == null) return null;
            return Entries(owner).TryGetValue(id.Substring(9), out var entry) && File.Exists(entry.File) ? SourceFor(owner, entry) : null;
        }

        internal static LoadedMod FinalOwner(string key, Sprite sprite)
        {
            if (ReferenceEquals(sprite, null)) return null;
            var owner = Owner;
            if (owner == null) return null;
            try
            {
                if (!Entries(owner).TryGetValue(key, out var entry)) return null;
                var runtime = ModAccess.Type(Id, "ProbablyRimworld.PortraitRuntime");
                var cache = ModAccess.Static<Dictionary<Sprite, Dictionary<string, Sprite>>>(runtime, "sprites");
                // Inspect values only: these are clones preserving original
                // geometry, not LoadedMod.Sprites. Exact references also respect
                // a different pack winning after Probably Rimworld.
                if (cache != null && cache.Values.Any(byResource => byResource.TryGetValue(entry.Resource, out var replacement) &&
                    ReferenceEquals(replacement, sprite))) return owner;
            }
            catch (Exception ex)
            {
                if (!warned) { warned = true; ModEntry.Warn(new InvalidDataException("边缘当铺最终来源识别失败：" + ex.Message)); }
            }
            return null;
        }
    }
}
