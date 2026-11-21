using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    // all_girls 0.1.3 is a different provider from the older AllGirls package.
    // Its Collect method exposes both built-in and localization-based mappings,
    // including extension owners and the current wanted4 variant.
    internal static class AllGirlsSources
    {
        internal const string Id = "all_girls";
        private sealed class Entry { internal LoadedMod Owner; internal string Key, File; }
        private static List<Entry> entries;
        private static bool ownerWarning;

        internal static bool IsFamily(LoadedMod mod) => mod?.Manifest != null &&
            (mod.Manifest.ID == Id || mod.Manifest.Prerequisites?.Contains(Id) == true);

        internal static void Clear() { entries = null; ownerWarning = false; }

        private static Type ProviderType() => ModLoader.Mods.Any(m => m.IsEnabled && m.Manifest?.ID == Id)
            ? ModAccess.Type(Id, "AllGirls.Portraits") : null;

        private static List<Entry> Entries()
        {
            if (entries != null) return entries;
            var result = new List<Entry>();
            var type = ProviderType();
            if (type == null) return result;
            var collected = ModReflect.CallStatic(type, "Collect") as IEnumerable;
            if (collected == null) return result;
            foreach (object row in collected)
            {
                string ownerId = ModReflect.Get<string>(row, "Owner");
                string key = ModReflect.Get<string>(row, "Key");
                string file = ModReflect.Get<string>(row, "File");
                var owner = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && IsFamily(m) && m.Manifest.ID == ownerId);
                if (owner == null || string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(file)) continue;
                result.Add(new Entry { Owner = owner, Key = key, File = file });
            }
            return entries = result;
        }

        private static Source SourceFor(LoadedMod owner, string key, string file)
        {
            if (!owner.IsEnabled || string.IsNullOrWhiteSpace(file)) return null;
            string path = null;
            if (!string.IsNullOrEmpty(owner.FolderPath))
            {
                string root = Path.GetFullPath(Path.Combine(owner.FolderPath, "Textures", "Items")) + Path.DirectorySeparatorChar;
                string candidate = Path.GetFullPath(Path.Combine(root, file + ".png"));
                if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase)) return null;
                if (System.IO.File.Exists(candidate)) path = candidate;
            }
            if (path == null && (owner.Sprites == null || !owner.Sprites.ContainsKey(file))) return null;
            return new Source {
                // Match the public sprite card ID: mapping and raw image must not
                // count as two weighted replacements for the same provider/role.
                Id = "mod:" + owner.Manifest.ID + ":" + file, File = path,
                Name = key ?? file, CharacterKey = key, Provider = owner.Manifest.ID,
                Group = owner.Manifest.Name ?? owner.Manifest.ID, MappedCharacter = !string.IsNullOrEmpty(key),
                Resolve = () => owner.IsEnabled && owner.Sprites != null && owner.Sprites.TryGetValue(file, out var sprite) ? sprite : null
            };
        }

        internal static IEnumerable<Source> Scan()
        {
            foreach (var entry in Entries())
            {
                var source = SourceFor(entry.Owner, entry.Key, entry.File);
                if (source != null) yield return source;
            }
        }

        internal static Source Restore(string id)
        {
            if (id == null || !id.StartsWith("mod:", StringComparison.Ordinal)) return null;
            int separator = id.IndexOf(':', 4);
            if (separator < 0) return null;
            string ownerId = id.Substring(4, separator - 4), file = id.Substring(separator + 1);
            if (!ModLoader.Mods.Any(m => m.IsEnabled && m.Manifest?.ID == Id)) return null;
            var owner = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && IsFamily(m) && m.Manifest.ID == ownerId);
            // A saved file can be restored before the runtime mapping is populated.
            // Do not choose another character or provider during restoration.
            return owner == null ? null : SourceFor(owner, null, file);
        }

        internal static LoadedMod FinalOwner(string key, Sprite sprite)
        {
            if (ReferenceEquals(sprite, null)) return null;
            try
            {
                var type = ProviderType();
                if (type == null) return null;
                var built = ModAccess.Static<Dictionary<string, Sprite>>(type, "built");
                if (built == null) return null;
                foreach (var entry in Entries())
                    if (entry.Owner.IsEnabled && entry.Key == key &&
                        built.TryGetValue(entry.Owner.Manifest.ID + "/" + entry.File, out var scaled) && ReferenceEquals(scaled, sprite))
                        return entry.Owner;
            }
            catch (Exception ex)
            {
                // An optional provider changing its internals must not stop the
                // rest of the catalog from loading, or log once for every image.
                if (!ownerWarning) { ownerWarning = true; ModEntry.Warn(new InvalidDataException("All Girls 最终来源识别失败：" + ex.Message)); }
            }
            return null;
        }
    }
}
