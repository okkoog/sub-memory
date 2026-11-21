using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace ItemPortraitHD
{
    // Read each provider's own mapping, rather than the final globally overridden sprite.
    internal static class ProviderMaps
    {
        private static Source EmbeddedSource(LoadedMod owner, string role, string resource, string name = null)
        {
            var capturedAssembly = owner.Assembly;
            return new Source { Id = "embedded:" + owner.Manifest.ID + ":" + (role ?? "") + ":" + resource,
                CharacterKey = role, Name = name ?? role, Group = owner.Manifest.Name, Provider = owner.Manifest.ID,
                MappedCharacter = true,
                ReadBytes = () => {
                    using (var stream = capturedAssembly.GetManifestResourceStream(resource))
                    using (var output = new MemoryStream())
                    {
                        if (stream == null || stream.Length > 32 * 1024 * 1024) return null;
                        stream.CopyTo(output); return output.ToArray();
                    }
                } };
        }

        internal static Source RestoreEmbedded(string id)
        {
            // Restore the saved full/half variant even if the provider setting changed.
            string prefix = "embedded:" + BaiSheResources.ExtensionId + ":";
            if (!id.StartsWith(prefix, StringComparison.Ordinal)) return null;
            int separator = id.IndexOf(':', prefix.Length);
            if (separator < 0) return null;
            string role = id.Substring(prefix.Length, separator - prefix.Length);
            string resource = id.Substring(separator + 1);
            if (BaiSheResources.Stem(resource) == null) return null;
            var owner = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Manifest?.ID == BaiSheResources.ExtensionId && m.Assembly != null);
            if (owner == null || !owner.Assembly.GetManifestResourceNames().Contains(resource, StringComparer.Ordinal)) return null;
            return EmbeddedSource(owner, string.IsNullOrEmpty(role) ? null : role, resource, role.Length > 0 ? role : BaiSheResources.Stem(resource));
        }

        internal static Dictionary<string, List<Source>> Scan()
        {
            var result = new Dictionary<string, List<Source>>(StringComparer.Ordinal);
            void Embedded(LoadedMod owner, string role, string resource, string name = null)
            {
                Add(EmbeddedSource(owner, role, resource, name));
            }
            void Add(Source source)
            {
                string key = PortraitRules.Normalize(source.CharacterKey);
                if (!result.TryGetValue(key, out var list)) result[key] = list = new List<Source>();
                if (!list.Any(s => s.Id == source.Id)) list.Add(source);
            }
            try { foreach (var source in AllGirlsSources.Scan()) Add(source); }
            catch (Exception ex) { ModEntry.Warn(new InvalidDataException("All Girls 来源映射读取失败：" + ex.Message)); }
            try { foreach (var source in RimworldSources.Scan()) Add(source); }
            catch (Exception ex) { ModEntry.Warn(new InvalidDataException("边缘当铺来源映射读取失败：" + ex.Message)); }
            foreach (var mod in ModLoader.Mods.Where(m => m.IsEnabled && m.Manifest != null))
            {
                try
                {
                    string assembly = mod.Assembly?.GetName().Name;
                    void FileSource(string role, string relative, string id, bool? readable = null)
                    {
                        string root = Path.GetFullPath(mod.FolderPath) + Path.DirectorySeparatorChar;
                        string file = Path.GetFullPath(Path.Combine(root, relative));
                        if (!file.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(file)) return;
                        Add(new Source { Id = id, File = file, Name = role, CharacterKey = role,
                            Group = mod.Manifest.Name, Provider = mod.Manifest.ID,
                            MatchSpriteName = readable.HasValue ? role : null, MatchReadable = readable });
                    }
                    if (assembly == "RetroAnime" || assembly == "RetroAnimeGirls")
                    {
                        string map = Path.Combine(mod.FolderPath, "portrait-map.csv");
                        if (File.Exists(map)) foreach (var entry in MappingRules.Retro(File.ReadAllLines(map)))
                            FileSource(entry.Role, entry.Relative, "retro:" + mod.Manifest.ID + ":" + entry.Role);
                    }
                    if (mod.Manifest.ID == "refined_anime_portraits")
                    {
                        string map = Path.Combine(mod.FolderPath, "portraits.tsv");
                        if (File.Exists(map)) foreach (var entry in MappingRules.Refined(File.ReadAllLines(map)))
                            FileSource(entry.Role, entry.Relative, "mapped:" + mod.Manifest.ID + ":" + entry.Relative, entry.Readable);
                    }
                    if (mod.Manifest.ID == "AllGirls")
                    {
                        string map = Path.Combine(mod.FolderPath, "ApprovedCharacters.json");
                        if (File.Exists(map)) foreach (var entry in MappingRules.Approved(File.ReadAllText(map)))
                            FileSource(entry.Role, entry.Relative, "mapped:" + mod.Manifest.ID + ":" + entry.Relative);
                    }
                    if (mod.Manifest.ID == BaiSheResources.MainId)
                    {
                        var type = ModAccess.Type(mod.Manifest.ID, "DeepSpacePortraits.PortraitMod");
                        if (type == null) continue;
                        var resources = mod.Assembly.GetManifestResourceNames().ToHashSet(StringComparer.Ordinal);
                        var stems = new Dictionary<string, string>(StringComparer.Ordinal);
                        var aliases = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                        void Alias(string role, string resource)
                        {
                            if (!resources.Contains(resource)) return;
                            if (!aliases.TryGetValue(role, out var list)) aliases[role] = list = new List<string>();
                            if (!list.Contains(resource)) list.Add(resource);
                        }
                        var files = ModAccess.Static<Dictionary<string, string>>(type, "Files");
                        if (files == null) continue;
                        var keyFiles = ModAccess.Static<Dictionary<string, string>>(type, "KeyFiles");
                        var augFiles = ModAccess.Static<Dictionary<string, string>>(type, "AugFiles");
                        var duplicates = ModAccess.Static<Dictionary<string, ValueTuple<string, string>>>(type, "DuplicateFiles");
                        var canonical = ModReflect.Target(type, "CanonicalBodyName", new[] { typeof(string) }) == null ? null :
                            ModReflect.StaticMethod<Func<string, string>>(type, "CanonicalBodyName");
                        foreach (string role in RandomPortraits.Keys())
                        {
                            var native = RandomPortraits.Native(role).Resolve();
                            string body = canonical?.Invoke(native ? native.name : role);
                            string stem = null;
                            if (augFiles != null) augFiles.TryGetValue(role, out stem);
                            if (stem == null && keyFiles != null) keyFiles.TryGetValue(role, out stem);
                            if (stem == null && body != null && duplicates != null && duplicates.TryGetValue(body, out var variants))
                            {
                                // The provider distinguishes duplicate sprite names by texture readability.
                                string selectedStem = native && native.texture && native.texture.isReadable ? variants.Item1 : variants.Item2;
                                stem = selectedStem;
                            }
                            if (stem == null && native && files.ContainsKey(native.name)) stem = native.name;
                            if (stem == null && files.ContainsKey(role)) stem = role;
                            if (stem == null && body != null && files.ContainsKey(body)) stem = body;
                            if (stem != null && files.TryGetValue(stem, out var resource))
                            {
                                Alias(role, resource);
                                stems[role] = stem;
                            }
                        }
                        foreach (var pair in aliases) foreach (string resource in pair.Value)
                        {
                            Embedded(mod, pair.Key, resource);
                        }
                        var extension = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled &&
                            m.Manifest?.ID == BaiSheResources.ExtensionId && m.Assembly != null);
                        if (extension != null)
                        {
                            bool halfBody = false;
                            var packType = ModAccess.Type(mod.Manifest.ID, "DeepSpacePortraits.R18Pack");
                            if (packType != null && ModReflect.Target(packType, "get_HalfBodyModeOn", Type.EmptyTypes) != null)
                                halfBody = ModReflect.StaticMethod<Func<bool>>(packType, "get_HalfBodyModeOn")();
                            var extensionResources = extension.Assembly.GetManifestResourceNames().ToHashSet(StringComparer.Ordinal);
                            var mapped = new HashSet<string>(StringComparer.Ordinal);
                            foreach (var pair in stems)
                            {
                                string resource = BaiSheResources.Select(extensionResources, pair.Value, halfBody);
                                if (resource == null) continue;
                                mapped.Add(resource);
                                Embedded(extension, pair.Key, resource, pair.Key + (resource.StartsWith("R18Half/", StringComparison.Ordinal) ? "（半身）" : "（全身）"));
                            }
                            // Extra portraits remain available manually, with no invented faction.
                            foreach (string resource in BaiSheResources.Portraits(extensionResources, halfBody).Where(r => !mapped.Contains(r)))
                                Embedded(extension, null, resource, BaiSheResources.Stem(resource) + "（仅手动）");
                        }
                    }
                }
                catch (Exception ex) { ModEntry.Warn(new InvalidDataException("来源映射读取失败：" + mod.Manifest.Name + " / " + ex.Message)); }
            }
            return result;
        }
    }
}
