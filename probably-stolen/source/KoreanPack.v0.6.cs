// Probably Stolen - generic Korean localization bridge.
// Safe architecture: native Localization injection + narrowly-scoped mod localizer helper postfixes.
// No constructor/.cctor patching, no global UI hooks, no File/Assembly resource hooks, no IL string rewriting.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace ProbablyStolenKoreanPack
{
    public sealed class KoreanPack : IMod
    {
        private const string OwnModId = "probably_stolen_korean_pack";
        private const string DynamicHarmonyId = "probably_stolen_korean_pack.safe_localizers";
        private const string SourcePrefix = "@source:";

        private sealed class Catalog
        {
            internal string DirectoryId;
            internal readonly Dictionary<string, string> ByKey =
                new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly Dictionary<string, string> ExplicitBySource =
                new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly Dictionary<string, string> DerivedBySource =
                new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly HashSet<string> AmbiguousDerivedSources =
                new HashSet<string>(StringComparer.Ordinal);
        }

        private sealed class SavedValue
        {
            internal string Locale;
            internal string Key;
            internal bool Existed;
            internal string Value;
        }

        private static readonly object Sync = new object();
        private static readonly Dictionary<string, Catalog> Catalogs =
            new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Assembly, Catalog> AssemblyCatalogs =
            new Dictionary<Assembly, Catalog>();
        private static readonly Dictionary<LoadedMod, List<SavedValue>> SavedValues =
            new Dictionary<LoadedMod, List<SavedValue>>();
        private static readonly HashSet<string> PatchedMethods =
            new HashSet<string>(StringComparer.Ordinal);

        private static Harmony _dynamicHarmony;
        private static bool _enabled;
        private static ModManifest _manifest;

        public void Init(ModManifest manifest)
        {
            _manifest = manifest;
        }

        public void OnEnable()
        {
            lock (Sync)
            {
                if (_enabled) return;

                string folder = Path.GetDirectoryName(typeof(KoreanPack).Assembly.Location);
                if (string.IsNullOrEmpty(folder))
                    throw new DirectoryNotFoundException("Korean pack assembly folder");

                Catalogs.Clear();
                AssemblyCatalogs.Clear();
                SavedValues.Clear();
                PatchedMethods.Clear();

                LoadCatalogs(Path.Combine(folder, "Localization"));
                _dynamicHarmony = new Harmony(DynamicHarmonyId);
                _enabled = true;

                Debug.Log("[KoreanPack] Enabled. Translation catalogs: " + Catalogs.Count);
            }
        }

        public void OnDisable()
        {
            lock (Sync)
            {
                if (!_enabled) return;
                _enabled = false;

                RestoreNativeEntries();
                UnpatchDynamic();

                Catalogs.Clear();
                AssemblyCatalogs.Clear();
                PatchedMethods.Clear();

                Debug.Log("[KoreanPack] Disabled.");
            }
        }

        // The game loads every mod DLL and calls OnEnable first. Only afterwards does
        // ModLoader.LoadModAssets call LoadLocalization(mod). A postfix here therefore:
        //   1) runs after the target's native CSVs have been loaded,
        //   2) also runs for mods that have no Localization directory (early return still gets a postfix),
        //   3) runs before OnModAssetsLoaded handlers build most localized runtime UI/data.
        [HarmonyPatch(typeof(ModLoader), "LoadLocalization")]
        private static class LoadLocalizationPatch
        {
            [HarmonyPostfix]
            private static void Postfix(LoadedMod __0)
            {
                if (!_enabled || __0 == null) return;
                try
                {
                    ApplyToLoadedMod(__0);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[KoreanPack] Localization bridge failed for " +
                        SafeModName(__0) + ": " + ex);
                }
            }
        }

        private static void ApplyToLoadedMod(LoadedMod loaded)
        {
            if (loaded.Manifest == null ||
                string.Equals(loaded.Manifest.ID, OwnModId, StringComparison.OrdinalIgnoreCase))
                return;

            Catalog catalog = LocateCatalog(loaded);
            if (catalog == null) return;

            lock (Sync)
            {
                MergeNativeEntries(loaded, catalog);
                RebuildDerivedSourceMap(loaded, catalog);
                PatchModLocalizers(loaded, catalog);
            }

            Debug.Log("[KoreanPack] Applied " + catalog.ByKey.Count + " key translation(s) and " +
                (catalog.ExplicitBySource.Count + catalog.DerivedBySource.Count) +
                " source translation(s) to " + SafeModName(loaded) + ".");
        }

        private static string SafeModName(LoadedMod loaded)
        {
            if (loaded == null) return "<null>";
            if (loaded.Manifest == null) return "<unknown>";
            return string.IsNullOrEmpty(loaded.Manifest.Name) ? loaded.Manifest.ID : loaded.Manifest.Name;
        }

        private static Catalog LocateCatalog(LoadedMod loaded)
        {
            Catalog catalog;
            if (loaded == null || loaded.Manifest == null) return null;

            if (!string.IsNullOrEmpty(loaded.Manifest.ID) &&
                Catalogs.TryGetValue(loaded.Manifest.ID, out catalog))
                return catalog;

            if (loaded.WorkshopId != 0 &&
                Catalogs.TryGetValue(loaded.WorkshopId.ToString(), out catalog))
                return catalog;

            if (!string.IsNullOrEmpty(loaded.FolderPath))
            {
                string folderName = Path.GetFileName(loaded.FolderPath.TrimEnd(
                    Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                if (!string.IsNullOrEmpty(folderName) &&
                    Catalogs.TryGetValue(folderName, out catalog))
                    return catalog;
            }

            return null;
        }

        private static void LoadCatalogs(string localizationRoot)
        {
            if (!System.IO.Directory.Exists(localizationRoot))
            {
                Debug.Log("[KoreanPack] Localization folder does not exist: " + localizationRoot);
                return;
            }

            foreach (string directory in System.IO.Directory.GetDirectories(localizationRoot))
            {
                string koPath = Path.Combine(directory, "ko.csv");
                if (!File.Exists(koPath)) continue;

                Catalog catalog = new Catalog
                {
                    DirectoryId = Path.GetFileName(directory)
                };

                int lineNumber = 0;
                foreach (string rawLine in File.ReadAllLines(koPath, Encoding.UTF8))
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(rawLine) ||
                        rawLine.TrimStart().StartsWith("#", StringComparison.Ordinal))
                        continue;

                    string key;
                    string value;
                    if (!TryParseTwoColumnCsv(rawLine, out key, out value))
                    {
                        Debug.LogWarning("[KoreanPack] Invalid CSV row: " + koPath +
                            " line " + lineNumber);
                        continue;
                    }

                    if (key.StartsWith(SourcePrefix, StringComparison.Ordinal))
                    {
                        string source = key.Substring(SourcePrefix.Length);
                        if (!string.IsNullOrEmpty(source))
                            catalog.ExplicitBySource[source] = value;
                    }
                    else if (!string.IsNullOrEmpty(key))
                    {
                        catalog.ByKey[key] = value;
                    }
                }

                if (catalog.ByKey.Count == 0 && catalog.ExplicitBySource.Count == 0)
                    continue;

                Catalogs[catalog.DirectoryId] = catalog;
            }
        }

        // Compatible with the game's key,value format, while additionally allowing a quoted
        // @source key that itself contains commas. Everything after the first unquoted comma
        // belongs to the value, matching ModLoader's tolerant behavior.
        private static bool TryParseTwoColumnCsv(string line, out string key, out string value)
        {
            key = null;
            value = null;
            if (line == null) return false;

            bool quoted = false;
            int separator = -1;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }
                    quoted = !quoted;
                }
                else if (c == ',' && !quoted)
                {
                    separator = i;
                    break;
                }
            }

            if (separator < 0) return false;
            key = DecodeCsvField(line.Substring(0, separator));
            value = DecodeCsvField(line.Substring(separator + 1)).Replace("\\n", "\n");
            return !string.IsNullOrEmpty(key);
        }

        private static string DecodeCsvField(string field)
        {
            string text = (field ?? string.Empty).Trim();
            if (text.Length >= 2 && text[0] == '"' && text[text.Length - 1] == '"')
                text = text.Substring(1, text.Length - 2).Replace("\"\"", "\"");
            return text;
        }

        private static IEnumerable<string> KoreanLocaleCodes()
        {
            yield return "ko";
            yield return "ko-KR";

            string current = CurrentLocaleCode();
            if (IsKoreanCode(current) &&
                !string.Equals(current, "ko", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(current, "ko-KR", StringComparison.OrdinalIgnoreCase))
                yield return current;
        }

        private static string CurrentLocaleCode()
        {
            try
            {
                return LocHelper.GetCurrentLocaleCode();
            }
            catch
            {
                return null;
            }
        }

        private static bool IsKorean()
        {
            return IsKoreanCode(CurrentLocaleCode());
        }

        private static bool IsKoreanCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            return string.Equals(code, "ko", StringComparison.OrdinalIgnoreCase) ||
                   code.StartsWith("ko-", StringComparison.OrdinalIgnoreCase) ||
                   code.StartsWith("ko_", StringComparison.OrdinalIgnoreCase);
        }

        private static void MergeNativeEntries(LoadedMod loaded, Catalog catalog)
        {
            if (loaded.Localization == null)
                loaded.Localization = new Dictionary<string, Dictionary<string, string>>(
                    StringComparer.OrdinalIgnoreCase);

            List<SavedValue> saved;
            if (!SavedValues.TryGetValue(loaded, out saved))
            {
                saved = new List<SavedValue>();
                SavedValues[loaded] = saved;
            }

            foreach (string locale in KoreanLocaleCodes().Distinct(StringComparer.OrdinalIgnoreCase))
            {
                Dictionary<string, string> dictionary;
                if (!loaded.Localization.TryGetValue(locale, out dictionary) || dictionary == null)
                {
                    dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
                    loaded.Localization[locale] = dictionary;
                }

                foreach (KeyValuePair<string, string> pair in catalog.ByKey)
                {
                    if (!saved.Any(x => string.Equals(x.Locale, locale, StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(x.Key, pair.Key, StringComparison.Ordinal)))
                    {
                        string old;
                        bool existed = dictionary.TryGetValue(pair.Key, out old);
                        saved.Add(new SavedValue
                        {
                            Locale = locale,
                            Key = pair.Key,
                            Existed = existed,
                            Value = old
                        });
                    }
                    dictionary[pair.Key] = pair.Value;
                }
            }
        }

        private static void RebuildDerivedSourceMap(LoadedMod loaded, Catalog catalog)
        {
            catalog.DerivedBySource.Clear();
            catalog.AmbiguousDerivedSources.Clear();
            if (loaded.Localization == null) return;

            foreach (KeyValuePair<string, string> translation in catalog.ByKey)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> locale in loaded.Localization)
                {
                    if (IsKoreanCode(locale.Key) || locale.Value == null) continue;

                    string source;
                    if (!locale.Value.TryGetValue(translation.Key, out source) ||
                        string.IsNullOrEmpty(source) ||
                        string.Equals(source, translation.Value, StringComparison.Ordinal))
                        continue;

                    AddDerivedSource(catalog, source, translation.Value);
                }
            }
        }

        private static void AddDerivedSource(Catalog catalog, string source, string korean)
        {
            if (catalog.ExplicitBySource.ContainsKey(source) ||
                catalog.AmbiguousDerivedSources.Contains(source))
                return;

            string existing;
            if (!catalog.DerivedBySource.TryGetValue(source, out existing))
            {
                catalog.DerivedBySource[source] = korean;
                return;
            }

            if (!string.Equals(existing, korean, StringComparison.Ordinal))
            {
                catalog.DerivedBySource.Remove(source);
                catalog.AmbiguousDerivedSources.Add(source);
            }
        }

        private static bool TryTranslateSource(Catalog catalog, string source, out string translated)
        {
            translated = null;
            if (catalog == null || string.IsNullOrEmpty(source)) return false;

            if (catalog.ExplicitBySource.TryGetValue(source, out translated))
                return true;

            return catalog.DerivedBySource.TryGetValue(source, out translated);
        }

        // Patch only methods that look like a mod's own localization helper.
        // This intentionally excludes constructors, static constructors, arbitrary methods,
        // game/global UI setters, file APIs and IL string rewriting.
        private static void PatchModLocalizers(LoadedMod loaded, Catalog catalog)
        {
            if (_dynamicHarmony == null || loaded == null || string.IsNullOrEmpty(loaded.FolderPath))
                return;

            string root;
            try
            {
                root = Path.GetFullPath(loaded.FolderPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                    Path.DirectorySeparatorChar;
            }
            catch
            {
                return;
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly == null || assembly == typeof(KoreanPack).Assembly) continue;

                string location;
                try { location = assembly.Location; }
                catch { continue; }
                if (string.IsNullOrEmpty(location)) continue;

                string fullLocation;
                try { fullLocation = Path.GetFullPath(location); }
                catch { continue; }

                if (!fullLocation.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    continue;

                AssemblyCatalogs[assembly] = catalog;
                PatchLocalizerHelpersInAssembly(assembly);
            }
        }

        private static void PatchLocalizerHelpersInAssembly(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Could not inspect " + assembly.GetName().Name +
                    ": " + ex.Message);
                return;
            }

            if (types == null) return;
            foreach (Type type in types)
            {
                if (type == null || type.IsInterface) continue;

                MethodInfo[] methods;
                try
                {
                    methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                }
                catch
                {
                    continue;
                }

                foreach (MethodInfo method in methods)
                {
                    if (!LooksLikeSafeLocalizer(method)) continue;

                    string id;
                    try { id = method.Module.ModuleVersionId + ":" + method.MetadataToken; }
                    catch { continue; }

                    if (PatchedMethods.Contains(id)) continue;

                    try
                    {
                        HarmonyMethod postfix = new HarmonyMethod(typeof(KoreanPack),
                            nameof(SafeLocalizerPostfix));
                        _dynamicHarmony.Patch(method, postfix: postfix);
                        PatchedMethods.Add(id);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[KoreanPack] Localizer helper patch skipped: " +
                            type.FullName + "." + method.Name + " (" + ex.Message + ")");
                    }
                }
            }
        }

        private static bool LooksLikeSafeLocalizer(MethodInfo method)
        {
            if (method == null || method.ReturnType != typeof(string) ||
                method.IsAbstract || method.ContainsGenericParameters)
                return false;

            try
            {
                if (method.GetMethodBody() == null) return false;
            }
            catch
            {
                return false;
            }

            string name = method.Name ?? string.Empty;
            string lowerName = name.ToLowerInvariant();
            string typeName = method.DeclaringType == null
                ? string.Empty
                : (method.DeclaringType.FullName ?? method.DeclaringType.Name).ToLowerInvariant();

            bool strongName =
                lowerName == "t" ||
                lowerName == "l" ||
                lowerName == "tr" ||
                lowerName == "text" ||
                lowerName == "translate" ||
                lowerName == "localize" ||
                lowerName == "select" ||
                lowerName == "gettext" ||
                lowerName == "getlocalized";

            bool localizerType =
                typeName.Contains("local") ||
                typeName.Contains("text") ||
                typeName.Contains("lang") ||
                typeName.Contains("translation") ||
                typeName.Contains("i18n") ||
                typeName.Contains("l10n");

            if (!strongName && !localizerType) return false;

            ParameterInfo[] parameters;
            try { parameters = method.GetParameters(); }
            catch { return false; }

            bool hasStringParameter = parameters.Any(p => p.ParameterType == typeof(string));
            if (!hasStringParameter && !localizerType) return false;

            // Avoid property/event plumbing even inside a type whose name happens to contain "text".
            if (method.IsSpecialName) return false;

            return true;
        }

        private static void SafeLocalizerPostfix(MethodBase __originalMethod, ref string __result)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null || string.IsNullOrEmpty(__result))
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out catalog))
                return;

            string translated;
            if (TryTranslateSource(catalog, __result, out translated))
                __result = translated;
        }

        private static void RestoreNativeEntries()
        {
            foreach (KeyValuePair<LoadedMod, List<SavedValue>> pair in SavedValues)
            {
                LoadedMod loaded = pair.Key;
                if (loaded == null || loaded.Localization == null) continue;

                foreach (SavedValue saved in pair.Value)
                {
                    Dictionary<string, string> dictionary;
                    if (!loaded.Localization.TryGetValue(saved.Locale, out dictionary) ||
                        dictionary == null)
                        continue;

                    if (saved.Existed)
                        dictionary[saved.Key] = saved.Value;
                    else
                        dictionary.Remove(saved.Key);
                }
            }
            SavedValues.Clear();
        }

        private static void UnpatchDynamic()
        {
            if (_dynamicHarmony == null) return;
            try
            {
                MethodInfo unpatch = typeof(Harmony).GetMethod("UnpatchAll",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static,
                    null, new[] { typeof(string) }, null);

                if (unpatch != null)
                    unpatch.Invoke(unpatch.IsStatic ? null : (object)_dynamicHarmony,
                        new object[] { DynamicHarmonyId });
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Dynamic patch cleanup failed: " + ex.Message);
            }
            _dynamicHarmony = null;
        }
    }
}
