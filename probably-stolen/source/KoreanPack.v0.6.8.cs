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
            internal string DeclaredId;
            internal readonly Dictionary<string, string> ByKey =
                new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly Dictionary<string, string> ExplicitBySource =
                new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly HashSet<string> AmbiguousExplicitSources =
                new HashSet<string>(StringComparer.Ordinal);
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
        private static bool _officialLocalizationPhaseReached;
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
                _officialLocalizationPhaseReached = false;

                LoadCatalogs(Path.Combine(folder, "Localization"));
                _dynamicHarmony = new Harmony(DynamicHarmonyId);
                PatchCommonDataHooks();
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
                _officialLocalizationPhaseReached = false;

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
            [HarmonyPrefix]
            private static void Prefix()
            {
                // ModLoader reaches this only after every mod's Init/OnEnable phase.
                // From this point onward using the game's selected locale is safe enough
                // for localization work; before this point we only read the saved locale code.
                _officialLocalizationPhaseReached = true;
            }

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

        [HarmonyPatch(typeof(ModHelper), "GetLocalized", new Type[]
        {
            typeof(string),
            typeof(string),
            typeof(object[])
        })]
        private static class NativeLookupPatch
        {
            [HarmonyPostfix]
            private static void Postfix(string __0, string __1, object[] __2, ref string __result)
            {
                if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0) || string.IsNullOrEmpty(__1))
                    return;

                Catalog catalog;
                if (!Catalogs.TryGetValue(__0, out catalog))
                    return;

                string korean;
                if (!catalog.ByKey.TryGetValue(__1, out korean))
                    return;

                if (__2 != null && __2.Length > 0)
                {
                    try
                    {
                        __result = string.Format(korean, __2);
                    }
                    catch (FormatException)
                    {
                        __result = korean;
                    }
                }
                else
                {
                    __result = korean;
                }
            }
        }

        private static Catalog CatalogFromAssemblyPath(string assemblyPath)
        {
            string full;
            try
            {
                full = Path.GetFullPath(assemblyPath);
            }
            catch
            {
                return null;
            }

            DirectoryInfo directory;
            try
            {
                directory = new FileInfo(full).Directory;
            }
            catch
            {
                return null;
            }

            // Workshop layout is .../workshop/content/4348910/<workshop id>/...
            // Local mods commonly use their manifest ID or project name as a folder.
            // Walk only a small number of ancestors to avoid matching unrelated paths.
            for (int depth = 0; directory != null && depth < 5; depth++, directory = directory.Parent)
            {
                Catalog catalog;
                if (!string.IsNullOrEmpty(directory.Name) &&
                    Catalogs.TryGetValue(directory.Name, out catalog))
                    return catalog;
            }

            return null;
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

                int recordNumber = 0;
                foreach (List<string> row in ReadCsvRecords(koPath))
                {
                    recordNumber++;
                    if (row == null || row.Count == 0) continue;

                    string first = (row[0] ?? string.Empty).Trim().TrimStart('\ufeff');
                    if (string.IsNullOrWhiteSpace(first)) continue;

                    if (first.StartsWith("#", StringComparison.Ordinal))
                    {
                        const string idPrefix = "# mod_id=";
                        if (first.StartsWith(idPrefix, StringComparison.Ordinal))
                            catalog.DeclaredId = first.Substring(idPrefix.Length).Trim();
                        continue;
                    }

                    if (row.Count >= 4 &&
                        string.Equals(first, "key", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((row[1] ?? string.Empty).Trim(), "source", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((row[2] ?? string.Empty).Trim(), "ko", StringComparison.OrdinalIgnoreCase) &&
                        string.Equals((row[3] ?? string.Empty).Trim(), "mode", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (row.Count == 2 &&
                        ((string.Equals(first, "key", StringComparison.OrdinalIgnoreCase) &&
                          string.Equals((row[1] ?? string.Empty).Trim(), "ko", StringComparison.OrdinalIgnoreCase)) ||
                         (string.Equals(first, "source", StringComparison.OrdinalIgnoreCase) &&
                          string.Equals((row[1] ?? string.Empty).Trim(), "ko", StringComparison.OrdinalIgnoreCase))))
                        continue;

                    if (row.Count >= 4)
                    {
                        string key = DecodeTranslationText(row[0]);
                        string source = DecodeTranslationText(row[1]);
                        string korean = DecodeTranslationText(row[2]);
                        string mode = (row[3] ?? string.Empty).Trim().ToLowerInvariant();

                        if (string.IsNullOrEmpty(korean)) continue;

                        if ((mode == "key" || mode == "both") && !string.IsNullOrEmpty(key))
                            catalog.ByKey[key] = korean;

                        if ((mode == "text" || mode == "both") && !string.IsNullOrEmpty(source))
                            AddExplicitSource(catalog, source, korean);

                        continue;
                    }

                    if (row.Count >= 2)
                    {
                        string key = DecodeTranslationText(row[0]).Trim();
                        string value = DecodeTranslationText(row[1]);
                        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) continue;

                        if (key.StartsWith(SourcePrefix, StringComparison.Ordinal))
                        {
                            string source = key.Substring(SourcePrefix.Length);
                            if (!string.IsNullOrEmpty(source))
                                AddExplicitSource(catalog, source, value);
                        }
                        else
                        {
                            catalog.ByKey[key] = value;
                        }

                        continue;
                    }

                    Debug.LogWarning("[KoreanPack] Invalid CSV record: " + koPath +
                        " record " + recordNumber);
                }

                if (catalog.ByKey.Count == 0 && catalog.ExplicitBySource.Count == 0)
                    continue;

                Catalogs[catalog.DirectoryId] = catalog;
                if (!string.IsNullOrWhiteSpace(catalog.DeclaredId))
                    Catalogs[catalog.DeclaredId] = catalog;
            }
        }

        private static string DecodeTranslationText(string value)
        {
            return (value ?? string.Empty).Replace("\\n", "\n");
        }

        private static void AddExplicitSource(Catalog catalog, string source, string korean)
        {
            if (catalog == null || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(korean) ||
                catalog.AmbiguousExplicitSources.Contains(source))
                return;

            string existing;
            if (!catalog.ExplicitBySource.TryGetValue(source, out existing))
            {
                catalog.ExplicitBySource[source] = korean;
                return;
            }

            if (!string.Equals(existing, korean, StringComparison.Ordinal))
            {
                catalog.ExplicitBySource.Remove(source);
                catalog.AmbiguousExplicitSources.Add(source);
            }
        }

        // Parses RFC-4180 style CSV records, including quoted commas, escaped quotes,
        // and physical newlines inside quoted fields. This keeps the existing
        // key,source,ko,mode translation corpus usable without migration.
        private static List<List<string>> ReadCsvRecords(string path)
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            List<List<string>> rows = new List<List<string>>();
            List<string> row = new List<string>();
            StringBuilder field = new StringBuilder();
            bool quoted = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '"')
                {
                    if (quoted && i + 1 < text.Length && text[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        quoted = !quoted;
                    }
                    continue;
                }

                if (c == ',' && !quoted)
                {
                    row.Add(field.ToString());
                    field.Length = 0;
                    continue;
                }

                if ((c == '\r' || c == '\n') && !quoted)
                {
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                        i++;

                    row.Add(field.ToString());
                    field.Length = 0;

                    if (row.Count > 1 || !string.IsNullOrWhiteSpace(row[0]))
                        rows.Add(row);

                    row = new List<string>();
                    continue;
                }

                field.Append(c);
            }

            if (field.Length > 0 || row.Count > 0)
            {
                row.Add(field.ToString());
                if (row.Count > 1 || !string.IsNullOrWhiteSpace(row[0]))
                    rows.Add(row);
            }

            if (quoted)
                throw new InvalidDataException("Unterminated quoted CSV field: " + path);

            return rows;
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
            // LocHelper.GetCurrentLocaleCode reads LocalizationSettings.SelectedLocale.
            // Prior to ModLoader.LoadLocalization that can start/touch Unity Localization
            // during another mod's OnEnable, which is exactly the unsafe startup window
            // this rewrite is designed to avoid.
            if (_officialLocalizationPhaseReached)
            {
                try
                {
                    string current = LocHelper.GetCurrentLocaleCode();
                    if (!string.IsNullOrWhiteSpace(current))
                        return current;
                }
                catch
                {
                }
            }

            // The game itself persists its chosen locale under this ES3 key.
            // Reading it does not touch Unity Localization/Addressables.
            try
            {
                if (ES3.KeyExists(LocHelper.LOCALE_SAVE_KEY))
                    return ES3.LoadString(LocHelper.LOCALE_SAVE_KEY, "en");
            }
            catch
            {
            }

            return string.Empty;
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
                    if (method == null || method.IsAbstract || method.ContainsGenericParameters)
                        continue;

                    try
                    {
                        if (LooksLikeVisibleStringProducer(method))
                            PatchDynamic(method, null, nameof(SafeStringResultPostfix), "result");

                        if (HasVisibleStringArguments(method))
                            PatchDynamic(method, nameof(SafeVisibleStringArgsPrefix), null, "args");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[KoreanPack] Safe text adapter skipped: " +
                            type.FullName + "." + method.Name + " (" + ex.Message + ")");
                    }
                }
            }
        }

        private static void PatchCommonDataHooks()
        {
            if (_dynamicHarmony == null) return;

            // GameItem metadata is persistent game data, not a per-frame renderer.
            foreach (MethodInfo method in typeof(GameItem).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.Instance))
            {
                if (!method.Name.StartsWith("set_", StringComparison.Ordinal)) continue;
                ParameterInfo[] p;
                try { p = method.GetParameters(); }
                catch { continue; }
                if (p.Length == 1 && p[0].ParameterType == typeof(string) &&
                    IsVisibleMemberName(method.Name.Substring(4), allowPlainName: true))
                    PatchDynamic(method, nameof(CommonDisplayStringPrefix), null, "game-model");
            }

            // Public mod registration APIs are one-time/occasional data registration sites.
            foreach (MethodInfo method in typeof(ModHelper).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (!IsRegistrationMethod(method)) continue;
                ParameterInfo[] p;
                try { p = method.GetParameters(); }
                catch { continue; }
                if (p.Any(x => x.ParameterType == typeof(string) &&
                    IsRegistrationDisplayParameter(x.Name)))
                    PatchDynamic(method, nameof(GameRegistrationPrefix), null, "registration");
            }

            // Notifications are event-time UI messages, not a global text renderer.
            foreach (MethodInfo method in typeof(StoreUIManager).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.Instance))
            {
                if (!string.Equals(method.Name, "Notify", StringComparison.Ordinal)) continue;
                ParameterInfo[] p;
                try { p = method.GetParameters(); }
                catch { continue; }
                if (p.Any(x => x.ParameterType == typeof(string) &&
                    IsVisibleArgumentName(x.Name, method.Name)))
                    PatchDynamic(method, nameof(CommonVisibleArgsPrefix), null, "notify");
            }
        }

        private static bool PatchDynamic(MethodBase method, string prefixName,
            string postfixName, string kind)
        {
            if (_dynamicHarmony == null || method == null) return false;

            string id;
            try
            {
                id = method.Module.ModuleVersionId + ":" + method.MetadataToken + ":" + kind;
            }
            catch
            {
                return false;
            }

            if (PatchedMethods.Contains(id)) return false;

            try
            {
                HarmonyMethod prefix = string.IsNullOrEmpty(prefixName) ? null :
                    new HarmonyMethod(typeof(KoreanPack), prefixName);
                HarmonyMethod postfix = string.IsNullOrEmpty(postfixName) ? null :
                    new HarmonyMethod(typeof(KoreanPack), postfixName);
                _dynamicHarmony.Patch(method, prefix: prefix, postfix: postfix);
                PatchedMethods.Add(id);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Patch skipped: " +
                    method.DeclaringType?.FullName + "." + method.Name + " (" + ex.Message + ")");
                return false;
            }
        }

        private static bool IsRegistrationMethod(MethodInfo method)
        {
            if (method == null || method.IsAbstract || method.ContainsGenericParameters)
                return false;

            string name = method.Name ?? string.Empty;
            return name.StartsWith("Add", StringComparison.Ordinal) ||
                   name.StartsWith("Register", StringComparison.Ordinal) ||
                   name.StartsWith("Create", StringComparison.Ordinal) ||
                   name.StartsWith("Modify", StringComparison.Ordinal) ||
                   name.StartsWith("Set", StringComparison.Ordinal) ||
                   name.StartsWith("Update", StringComparison.Ordinal);
        }

        private static bool IsRegistrationDisplayParameter(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return false;
            string name = NormalizeMemberName(raw);
            switch (name)
            {
                case "name":
                case "displayname":
                case "label":
                case "title":
                case "subtitle":
                case "description":
                case "shortdescription":
                case "longdescription":
                case "flavortext":
                case "tooltip":
                case "hint":
                case "message":
                case "text":
                case "displaytext":
                case "caption":
                case "summary":
                    return true;
                default:
                    return false;
            }
        }

        private static bool HasVisibleStringArguments(MethodInfo method)
        {
            if (method == null || method.IsAbstract || method.ContainsGenericParameters)
                return false;

            try
            {
                if (method.GetMethodBody() == null) return false;
            }
            catch
            {
                return false;
            }

            ParameterInfo[] parameters;
            try { parameters = method.GetParameters(); }
            catch { return false; }

            return parameters.Any(p => p.ParameterType == typeof(string) &&
                IsVisibleArgumentName(p.Name, method.Name));
        }

        private static bool IsVisibleArgumentName(string rawParameterName, string methodName)
        {
            if (string.IsNullOrEmpty(rawParameterName)) return false;
            string name = NormalizeMemberName(rawParameterName);

            if (IsProtectedMemberName(name)) return false;

            switch (name)
            {
                case "text":
                case "message":
                case "label":
                case "title":
                case "subtitle":
                case "caption":
                case "description":
                case "shortdescription":
                case "longdescription":
                case "flavortext":
                case "tooltip":
                case "prompt":
                case "summary":
                case "hint":
                case "displayname":
                case "displaytext":
                case "story":
                case "dialogue":
                case "dialog":
                case "notes":
                    return true;
            }

            // Plain "name", "value", "content" are too generic unless the method
            // itself clearly denotes a display operation.
            if (name == "name" || name == "value" || name == "content")
                return LooksLikeDisplayMethodName(methodName);

            return IsVisibleMemberName(rawParameterName, allowPlainName: false);
        }

        private static bool LooksLikeVisibleStringProducer(MethodInfo method)
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
            ParameterInfo[] parameters;
            try { parameters = method.GetParameters(); }
            catch { return false; }

            if (LooksLikeSafeLocalizer(method)) return true;

            if (method.IsSpecialName)
            {
                if (!name.StartsWith("get_", StringComparison.Ordinal)) return false;
                return IsVisibleMemberName(name.Substring(4), allowPlainName: true);
            }

            // C# compiler-generated parameterless Func<string> bodies are common for
            // mod config labels/descriptions. Exact source matching still gates replacement.
            if (name.StartsWith("<", StringComparison.Ordinal) && parameters.Length == 0)
                return true;

            return LooksLikeDisplayMethodName(name);
        }

        private static bool LooksLikeSafeLocalizer(MethodInfo method)
        {
            if (method == null || method.ReturnType != typeof(string) ||
                method.IsAbstract || method.ContainsGenericParameters)
                return false;

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
            if (method.IsSpecialName) return false;

            return true;
        }

        private static bool LooksLikeDisplayMethodName(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return false;
            string name = raw.ToLowerInvariant();

            string[] protectedWords =
            {
                "identifier", "filepath", "filename", "resource", "asset", "prefab",
                "texture", "sprite", "shader", "material", "namespace", "assembly",
                "guid", "hash", "token", "schema"
            };
            if (protectedWords.Any(name.Contains)) return false;

            string[] displayWords =
            {
                "text", "label", "title", "caption", "description", "describe",
                "display", "tooltip", "prompt", "message", "notify", "toast",
                "banner", "summary", "hint", "dialog", "story", "format", "name"
            };
            return displayWords.Any(name.Contains);
        }

        private static bool IsVisibleMemberName(string raw, bool allowPlainName)
        {
            if (string.IsNullOrEmpty(raw)) return false;
            string name = NormalizeMemberName(raw);
            if (IsProtectedMemberName(name)) return false;
            if (name == "name") return allowPlainName;

            string[] endings =
            {
                "name", "title", "subtitle", "description", "desc", "summary",
                "flavor", "flavortext", "label", "caption", "tooltip", "hint",
                "message", "text", "displaytext", "dialogue", "dialog", "story",
                "details", "content", "note", "notes"
            };
            return endings.Any(name.EndsWith);
        }

        private static string NormalizeMemberName(string raw)
        {
            return (raw ?? string.Empty).TrimStart('_').ToLowerInvariant()
                .Replace("_", string.Empty).Replace("-", string.Empty);
        }

        private static bool IsProtectedMemberName(string normalized)
        {
            if (string.IsNullOrEmpty(normalized)) return false;

            if (normalized == "id" || normalized == "key" || normalized == "path" ||
                normalized == "identifier" || normalized == "type" || normalized == "kind" ||
                normalized == "tag" || normalized == "code" || normalized == "locale" ||
                normalized == "language")
                return true;

            return normalized.EndsWith("id", StringComparison.Ordinal) ||
                   normalized.EndsWith("key", StringComparison.Ordinal) ||
                   normalized.EndsWith("path", StringComparison.Ordinal) ||
                   normalized.Contains("filename") ||
                   normalized.Contains("filepath") ||
                   normalized.Contains("resourcename") ||
                   normalized.Contains("assetname") ||
                   normalized.Contains("prefabname") ||
                   normalized.Contains("spritename") ||
                   normalized.Contains("texturename");
        }

        private static void SafeStringResultPostfix(MethodBase __originalMethod, ref string __result)
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

        private static void SafeVisibleStringArgsPrefix(MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null || __args == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(__originalMethod.DeclaringType.Assembly, out catalog))
                return;

            TranslateVisibleArguments(__originalMethod, __args, catalog, registrationRules: false);
        }

        private static void CommonDisplayStringPrefix(ref string __0)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0)) return;
            Catalog catalog = GetCallerCatalog();
            if (catalog == null) return;

            string translated;
            if (TryTranslateSource(catalog, __0, out translated))
                __0 = translated;
        }

        private static void CommonVisibleArgsPrefix(MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null || __args == null)
                return;

            Catalog catalog = GetCallerCatalog();
            if (catalog == null) return;
            TranslateVisibleArguments(__originalMethod, __args, catalog, registrationRules: false);
        }

        private static void GameRegistrationPrefix(MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null || __args == null)
                return;

            Catalog catalog = null;
            ParameterInfo[] parameters;
            try { parameters = __originalMethod.GetParameters(); }
            catch { return; }

            for (int i = 0; i < parameters.Length && i < __args.Length; i++)
            {
                if (!string.Equals(parameters[i].Name, "modId", StringComparison.OrdinalIgnoreCase))
                    continue;
                string id = __args[i] as string;
                if (!string.IsNullOrEmpty(id) && Catalogs.TryGetValue(id, out catalog))
                    break;
            }

            if (catalog == null)
                catalog = GetCallerCatalog();
            if (catalog == null) return;

            TranslateVisibleArguments(__originalMethod, __args, catalog, registrationRules: true);
        }

        private static void TranslateVisibleArguments(MethodBase method, object[] args,
            Catalog catalog, bool registrationRules)
        {
            ParameterInfo[] parameters;
            try { parameters = method.GetParameters(); }
            catch { return; }

            for (int i = 0; i < parameters.Length && i < args.Length; i++)
            {
                if (parameters[i].ParameterType != typeof(string)) continue;

                bool visible = registrationRules
                    ? IsRegistrationDisplayParameter(parameters[i].Name)
                    : IsVisibleArgumentName(parameters[i].Name, method.Name);
                if (!visible) continue;

                string original = args[i] as string;
                if (string.IsNullOrEmpty(original)) continue;

                string translated;
                if (TryTranslateSource(catalog, original, out translated))
                    args[i] = translated;
            }
        }

        private static Catalog GetCallerCatalog()
        {
            try
            {
                string loadingFolder;
                ModManifest loadingManifest;
                Catalog loadingCatalog = null;
                if (TryGetCurrentlyLoadingMod(out loadingFolder, out loadingManifest) &&
                    loadingManifest != null && !string.IsNullOrEmpty(loadingManifest.ID))
                {
                    Catalogs.TryGetValue(loadingManifest.ID, out loadingCatalog);
                }

                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(2, false);
                for (int i = 0; i < trace.FrameCount; i++)
                {
                    MethodBase method = trace.GetFrame(i).GetMethod();
                    if (method == null || method.DeclaringType == null) continue;

                    Assembly assembly = method.DeclaringType.Assembly;
                    if (assembly == null || assembly == typeof(KoreanPack).Assembly)
                        continue;

                    Catalog catalog;
                    if (AssemblyCatalogs.TryGetValue(assembly, out catalog))
                        return catalog;

                    string location;
                    try { location = assembly.Location; }
                    catch { continue; }
                    if (string.IsNullOrWhiteSpace(location)) continue;

                    catalog = CatalogFromAssemblyPath(location);

                    // Folder-name lookup may fail for a local mod whose directory name
                    // differs from its manifest ID. During LoadModDll the game exposes the
                    // exact current mod folder+manifest in loadingMod. Use that fallback
                    // only when this stack-frame assembly actually lives under that folder.
                    if (catalog == null && loadingCatalog != null &&
                        IsPathInsideFolder(location, loadingFolder))
                        catalog = loadingCatalog;

                    if (catalog != null)
                    {
                        AssemblyCatalogs[assembly] = catalog;
                        return catalog;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        private static bool TryGetCurrentlyLoadingMod(out string folder, out ModManifest manifest)
        {
            folder = null;
            manifest = null;
            try
            {
                FieldInfo field = typeof(ModLoader).GetField("loadingMod",
                    BindingFlags.NonPublic | BindingFlags.Static);
                if (field == null) return false;

                object boxedTuple = field.GetValue(null);
                if (boxedTuple == null) return false;

                Type tupleType = boxedTuple.GetType();
                FieldInfo folderField = tupleType.GetField("Item1");
                FieldInfo manifestField = tupleType.GetField("Item2");
                if (folderField == null || manifestField == null) return false;

                folder = folderField.GetValue(boxedTuple) as string;
                manifest = manifestField.GetValue(boxedTuple) as ModManifest;
                return !string.IsNullOrWhiteSpace(folder) && manifest != null;
            }
            catch
            {
                folder = null;
                manifest = null;
                return false;
            }
        }

        private static bool IsPathInsideFolder(string filePath, string folder)
        {
            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(folder))
                return false;

            try
            {
                string fullFile = Path.GetFullPath(filePath);
                string fullFolder = Path.GetFullPath(folder)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                    Path.DirectorySeparatorChar;
                return fullFile.StartsWith(fullFolder, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
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
