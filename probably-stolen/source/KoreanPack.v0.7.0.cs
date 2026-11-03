// Probably Stolen - generic Korean localization bridge v0.7.0
// Official lifecycle only:
// - No ModLoader patch/reflection.
// - No runtime file/path access.
// - No AssemblyLoad interception.
// - No constructor/.cctor patching.
// - No global game UI/File/resource hooks.
// Target mods are identified only by LoadedMod values delivered by ModHook.OnModAssetsLoaded.

using System;
using System.Collections.Generic;
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
        private const string CatalogPrefix = "pskb__";
        private const string DynamicHarmonyId = "probably_stolen_korean_pack.target_text";

        private sealed class Catalog
        {
            internal string PrimaryId;
            internal readonly HashSet<string> Aliases =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);
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

        private static readonly HashSet<string> AmbiguousAliases =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<Assembly, Catalog> AssemblyCatalogs =
            new Dictionary<Assembly, Catalog>();

        private static readonly Dictionary<LoadedMod, List<SavedValue>> SavedValues =
            new Dictionary<LoadedMod, List<SavedValue>>();

        private static readonly HashSet<string> PatchedMethods =
            new HashSet<string>(StringComparer.Ordinal);

        private static readonly List<LoadedMod> PendingMods =
            new List<LoadedMod>();

        private static Harmony _dynamicHarmony;
        private static bool _enabled;
        private static bool _catalogsReady;

        public void Init(ModManifest manifest)
        {
        }

        public void OnEnable()
        {
            lock (Sync)
            {
                if (_enabled) return;

                Catalogs.Clear();
                AmbiguousAliases.Clear();
                AssemblyCatalogs.Clear();
                SavedValues.Clear();
                PatchedMethods.Clear();
                PendingMods.Clear();

                _dynamicHarmony = new Harmony(DynamicHarmonyId);
                _catalogsReady = false;
                _enabled = true;

                ModHook.OnModAssetsLoaded += OnModAssetsLoaded;

                Debug.Log("[KoreanPack] Enabled. Waiting for official ModHook.OnModAssetsLoaded.");
            }
        }

        public void OnDisable()
        {
            lock (Sync)
            {
                if (!_enabled) return;

                _enabled = false;
                ModHook.OnModAssetsLoaded -= OnModAssetsLoaded;

                RestoreNativeEntries();
                UnpatchDynamic();

                Catalogs.Clear();
                AmbiguousAliases.Clear();
                AssemblyCatalogs.Clear();
                PatchedMethods.Clear();
                PendingMods.Clear();
                _catalogsReady = false;

                Debug.Log("[KoreanPack] Disabled.");
            }
        }

        private static void OnModAssetsLoaded(LoadedMod loaded)
        {
            if (!_enabled || loaded == null || loaded.Manifest == null)
                return;

            lock (Sync)
            {
                try
                {
                    if (string.Equals(loaded.Manifest.ID, OwnModId,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildCatalogsFromOwnLocalization(loaded);
                        _catalogsReady = true;

                        if (PendingMods.Count > 0)
                        {
                            LoadedMod[] pending = PendingMods.ToArray();
                            PendingMods.Clear();

                            foreach (LoadedMod target in pending)
                                ApplyToLoadedMod(target);
                        }

                        return;
                    }

                    if (!_catalogsReady)
                    {
                        if (!PendingMods.Contains(loaded))
                            PendingMods.Add(loaded);
                        return;
                    }

                    ApplyToLoadedMod(loaded);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[KoreanPack] OnModAssetsLoaded failed for " +
                        SafeModName(loaded) + ": " + ex);
                }
            }
        }

        private static void BuildCatalogsFromOwnLocalization(LoadedMod own)
        {
            Catalogs.Clear();
            AmbiguousAliases.Clear();

            if (own.Localization == null)
            {
                Debug.LogWarning("[KoreanPack] Own Localization dictionary is null.");
                return;
            }

            int catalogCount = 0;
            int keyCount = 0;
            int sourceCount = 0;

            foreach (KeyValuePair<string, Dictionary<string, string>> localeFile in own.Localization)
            {
                string fileId = localeFile.Key ?? string.Empty;
                if (!fileId.StartsWith(CatalogPrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                string primary = fileId.Substring(CatalogPrefix.Length).Trim();
                if (string.IsNullOrEmpty(primary) || localeFile.Value == null)
                    continue;

                Catalog catalog = new Catalog { PrimaryId = primary };
                catalog.Aliases.Add(primary);

                foreach (KeyValuePair<string, string> row in localeFile.Value)
                {
                    string rowKey = row.Key ?? string.Empty;
                    string korean = row.Value ?? string.Empty;

                    if (rowKey.StartsWith("M|", StringComparison.Ordinal))
                    {
                        AddAlias(catalog, rowKey.Substring(2));
                        continue;
                    }

                    if (rowKey.StartsWith("W|", StringComparison.Ordinal))
                    {
                        AddAlias(catalog, rowKey.Substring(2));
                        continue;
                    }

                    if (string.IsNullOrEmpty(korean))
                        continue;

                    if (rowKey.StartsWith("K|", StringComparison.Ordinal))
                    {
                        string key = rowKey.Substring(2);
                        if (!string.IsNullOrEmpty(key))
                            catalog.ByKey[key] = korean;
                        continue;
                    }

                    if (rowKey.StartsWith("K64|", StringComparison.Ordinal))
                    {
                        string key = DecodeBase64Url(rowKey.Substring(4));
                        if (!string.IsNullOrEmpty(key))
                            catalog.ByKey[key] = korean;
                        continue;
                    }

                    if (rowKey.StartsWith("S|", StringComparison.Ordinal))
                    {
                        AddExplicitSource(catalog, rowKey.Substring(2), korean);
                        continue;
                    }

                    if (rowKey.StartsWith("S64|", StringComparison.Ordinal))
                    {
                        string source = DecodeBase64Url(rowKey.Substring(4));
                        if (!string.IsNullOrEmpty(source))
                            AddExplicitSource(catalog, source, korean);
                    }
                }

                if (catalog.ByKey.Count == 0 && catalog.ExplicitBySource.Count == 0)
                    continue;

                RegisterCatalogAliases(catalog);
                catalogCount++;
                keyCount += catalog.ByKey.Count;
                sourceCount += catalog.ExplicitBySource.Count;
            }

            Debug.Log("[KoreanPack] Catalogs ready: " + catalogCount +
                " catalog(s), " + keyCount + " key translation(s), " +
                sourceCount + " source translation(s).");
        }

        private static void AddAlias(Catalog catalog, string alias)
        {
            if (catalog == null || string.IsNullOrWhiteSpace(alias))
                return;
            catalog.Aliases.Add(alias.Trim());
        }

        private static void RegisterCatalogAliases(Catalog catalog)
        {
            foreach (string alias in catalog.Aliases)
            {
                if (string.IsNullOrWhiteSpace(alias) ||
                    AmbiguousAliases.Contains(alias))
                    continue;

                Catalog existing;
                if (!Catalogs.TryGetValue(alias, out existing))
                {
                    Catalogs[alias] = catalog;
                    continue;
                }

                if (!ReferenceEquals(existing, catalog))
                {
                    Catalogs.Remove(alias);
                    AmbiguousAliases.Add(alias);
                    Debug.LogWarning("[KoreanPack] Ambiguous catalog alias ignored: " + alias);
                }
            }
        }

        private static string DecodeBase64Url(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                return string.Empty;

            try
            {
                string normalized = encoded.Replace('-', '+').Replace('_', '/');
                switch (normalized.Length % 4)
                {
                    case 2: normalized += "=="; break;
                    case 3: normalized += "="; break;
                }

                byte[] bytes = Convert.FromBase64String(normalized);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static void ApplyToLoadedMod(LoadedMod loaded)
        {
            if (loaded == null || loaded.Manifest == null ||
                string.Equals(loaded.Manifest.ID, OwnModId, StringComparison.OrdinalIgnoreCase))
                return;

            Catalog catalog = LocateCatalog(loaded);
            if (catalog == null)
                return;

            MergeNativeEntries(loaded, catalog);
            RebuildDerivedSourceMap(loaded, catalog);
            PatchTargetAssembly(loaded, catalog);

            Debug.Log("[KoreanPack] Applied to " + SafeModName(loaded) +
                ": " + catalog.ByKey.Count + " key(s), " +
                (catalog.ExplicitBySource.Count + catalog.DerivedBySource.Count) +
                " source mapping(s).");
        }

        private static Catalog LocateCatalog(LoadedMod loaded)
        {
            Catalog catalog;

            if (loaded.Manifest != null &&
                !string.IsNullOrEmpty(loaded.Manifest.ID) &&
                Catalogs.TryGetValue(loaded.Manifest.ID, out catalog))
                return catalog;

            if (loaded.WorkshopId != 0 &&
                Catalogs.TryGetValue(loaded.WorkshopId.ToString(), out catalog))
                return catalog;

            if (loaded.ShadowedWorkshopId != 0 &&
                Catalogs.TryGetValue(loaded.ShadowedWorkshopId.ToString(), out catalog))
                return catalog;

            return null;
        }

        private static string SafeModName(LoadedMod loaded)
        {
            if (loaded == null || loaded.Manifest == null)
                return "<unknown>";

            return string.IsNullOrEmpty(loaded.Manifest.Name)
                ? loaded.Manifest.ID
                : loaded.Manifest.Name;
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

            MergeLocale(loaded, catalog, saved, "ko");
            MergeLocale(loaded, catalog, saved, "ko-KR");
        }

        private static void MergeLocale(LoadedMod loaded, Catalog catalog,
            List<SavedValue> saved, string locale)
        {
            Dictionary<string, string> dictionary;
            if (!loaded.Localization.TryGetValue(locale, out dictionary) || dictionary == null)
            {
                dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
                loaded.Localization[locale] = dictionary;
            }

            foreach (KeyValuePair<string, string> pair in catalog.ByKey)
            {
                bool alreadySaved = false;
                for (int i = 0; i < saved.Count; i++)
                {
                    if (string.Equals(saved[i].Locale, locale,
                            StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(saved[i].Key, pair.Key, StringComparison.Ordinal))
                    {
                        alreadySaved = true;
                        break;
                    }
                }

                if (!alreadySaved)
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

        private static void RebuildDerivedSourceMap(LoadedMod loaded, Catalog catalog)
        {
            catalog.DerivedBySource.Clear();
            catalog.AmbiguousDerivedSources.Clear();

            if (loaded.Localization == null)
                return;

            foreach (KeyValuePair<string, string> translation in catalog.ByKey)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> locale
                    in loaded.Localization)
                {
                    if (IsKoreanCode(locale.Key) || locale.Value == null)
                        continue;

                    string source;
                    if (!locale.Value.TryGetValue(translation.Key, out source) ||
                        string.IsNullOrEmpty(source) ||
                        string.Equals(source, translation.Value, StringComparison.Ordinal))
                        continue;

                    AddDerivedSource(catalog, source, translation.Value);
                }
            }
        }

        private static void AddExplicitSource(Catalog catalog, string source, string korean)
        {
            if (catalog == null || string.IsNullOrEmpty(source) ||
                string.IsNullOrEmpty(korean) ||
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

        private static bool TryTranslateSource(Catalog catalog, string source,
            out string translated)
        {
            translated = null;
            if (catalog == null || string.IsNullOrEmpty(source))
                return false;

            if (catalog.ExplicitBySource.TryGetValue(source, out translated))
                return true;

            return catalog.DerivedBySource.TryGetValue(source, out translated);
        }

        private static void PatchTargetAssembly(LoadedMod loaded, Catalog catalog)
        {
            if (_dynamicHarmony == null || loaded == null ||
                loaded.Assembly == null || catalog == null)
                return;

            Assembly assembly = loaded.Assembly;

            Catalog existing;
            if (AssemblyCatalogs.TryGetValue(assembly, out existing))
            {
                if (!ReferenceEquals(existing, catalog))
                {
                    Debug.LogWarning("[KoreanPack] Assembly already mapped to another catalog: " +
                        assembly.GetName().Name);
                }
                return;
            }

            AssemblyCatalogs[assembly] = catalog;
            PatchDisplayMethodsInAssembly(assembly);
        }

        private static void PatchDisplayMethodsInAssembly(Assembly assembly)
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
                Debug.LogWarning("[KoreanPack] Could not inspect target assembly " +
                    assembly.GetName().Name + ": " + ex.Message);
                return;
            }

            if (types == null)
                return;

            foreach (Type type in types)
            {
                if (type == null || type.IsInterface)
                    continue;

                MethodInfo[] methods;
                try
                {
                    methods = type.GetMethods(
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Static |
                        BindingFlags.Instance |
                        BindingFlags.DeclaredOnly);
                }
                catch
                {
                    continue;
                }

                foreach (MethodInfo method in methods)
                {
                    if (method == null || method.IsAbstract ||
                        method.ContainsGenericParameters || method.IsConstructor)
                        continue;

                    try
                    {
                        if (LooksLikeVisibleStringProducer(method))
                            PatchDynamic(method, null,
                                nameof(SafeStringResultPostfix), "result");

                        if (HasVisibleStringArguments(method))
                            PatchDynamic(method,
                                nameof(SafeVisibleStringArgsPrefix), null, "args");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[KoreanPack] Target text adapter skipped: " +
                            type.FullName + "." + method.Name + " (" + ex.Message + ")");
                    }
                }
            }
        }

        private static bool PatchDynamic(MethodBase method, string prefixName,
            string postfixName, string kind)
        {
            if (_dynamicHarmony == null || method == null)
                return false;

            string id;
            try
            {
                id = method.Module.ModuleVersionId + ":" +
                    method.MetadataToken + ":" + kind;
            }
            catch
            {
                return false;
            }

            if (PatchedMethods.Contains(id))
                return false;

            HarmonyMethod prefix = string.IsNullOrEmpty(prefixName)
                ? null
                : new HarmonyMethod(typeof(KoreanPack), prefixName);

            HarmonyMethod postfix = string.IsNullOrEmpty(postfixName)
                ? null
                : new HarmonyMethod(typeof(KoreanPack), postfixName);

            _dynamicHarmony.Patch(method, prefix: prefix, postfix: postfix);
            PatchedMethods.Add(id);
            return true;
        }

        private static bool HasVisibleStringArguments(MethodInfo method)
        {
            if (method == null || method.IsSpecialName)
                return false;

            try
            {
                if (method.GetMethodBody() == null)
                    return false;
            }
            catch
            {
                return false;
            }

            ParameterInfo[] parameters;
            try
            {
                parameters = method.GetParameters();
            }
            catch
            {
                return false;
            }

            return parameters.Any(p =>
                p.ParameterType == typeof(string) &&
                IsVisibleArgumentName(p.Name, method.Name));
        }

        private static bool LooksLikeVisibleStringProducer(MethodInfo method)
        {
            if (method == null || method.ReturnType != typeof(string))
                return false;

            try
            {
                if (method.GetMethodBody() == null)
                    return false;
            }
            catch
            {
                return false;
            }

            string name = method.Name ?? string.Empty;

            if (LooksLikeLocalizer(method))
                return true;

            if (method.IsSpecialName)
            {
                if (!name.StartsWith("get_", StringComparison.Ordinal))
                    return false;

                return IsVisibleMemberName(name.Substring(4), true);
            }

            ParameterInfo[] parameters;
            try
            {
                parameters = method.GetParameters();
            }
            catch
            {
                return false;
            }

            if (name.StartsWith("<", StringComparison.Ordinal) &&
                parameters.Length == 0)
                return true;

            return LooksLikeDisplayMethodName(name);
        }

        private static bool LooksLikeLocalizer(MethodInfo method)
        {
            string name = (method.Name ?? string.Empty).ToLowerInvariant();
            string typeName = method.DeclaringType == null
                ? string.Empty
                : (method.DeclaringType.FullName ??
                    method.DeclaringType.Name).ToLowerInvariant();

            bool strongName =
                name == "t" ||
                name == "l" ||
                name == "tr" ||
                name == "text" ||
                name == "translate" ||
                name == "localize" ||
                name == "select" ||
                name == "gettext" ||
                name == "getlocalized";

            bool localizerType =
                typeName.Contains("local") ||
                typeName.Contains("text") ||
                typeName.Contains("lang") ||
                typeName.Contains("translation") ||
                typeName.Contains("i18n") ||
                typeName.Contains("l10n");

            if (!strongName && !localizerType)
                return false;

            ParameterInfo[] parameters;
            try
            {
                parameters = method.GetParameters();
            }
            catch
            {
                return false;
            }

            return parameters.Any(p => p.ParameterType == typeof(string)) ||
                   localizerType;
        }

        private static bool IsVisibleArgumentName(string rawParameterName,
            string methodName)
        {
            if (string.IsNullOrEmpty(rawParameterName))
                return false;

            string name = NormalizeName(rawParameterName);
            if (IsProtectedName(name))
                return false;

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

            if (name == "name" || name == "value" || name == "content")
                return LooksLikeDisplayMethodName(methodName);

            return IsVisibleMemberName(rawParameterName, false);
        }

        private static bool LooksLikeDisplayMethodName(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return false;

            string name = raw.ToLowerInvariant();

            string[] protectedWords =
            {
                "identifier", "filepath", "filename", "resource",
                "asset", "prefab", "texture", "sprite", "shader",
                "material", "namespace", "assembly", "guid", "hash",
                "token", "schema"
            };

            if (protectedWords.Any(name.Contains))
                return false;

            string[] displayWords =
            {
                "text", "label", "title", "caption", "description",
                "describe", "display", "tooltip", "prompt", "message",
                "notify", "toast", "banner", "summary", "hint",
                "dialog", "story", "format", "name"
            };

            return displayWords.Any(name.Contains);
        }

        private static bool IsVisibleMemberName(string raw, bool allowPlainName)
        {
            if (string.IsNullOrEmpty(raw))
                return false;

            string name = NormalizeName(raw);
            if (IsProtectedName(name))
                return false;

            if (name == "name")
                return allowPlainName;

            string[] endings =
            {
                "name", "title", "subtitle", "description", "desc",
                "summary", "flavor", "flavortext", "label", "caption",
                "tooltip", "hint", "message", "text", "displaytext",
                "dialogue", "dialog", "story", "details", "content",
                "note", "notes"
            };

            return endings.Any(name.EndsWith);
        }

        private static string NormalizeName(string raw)
        {
            return (raw ?? string.Empty)
                .TrimStart('_')
                .ToLowerInvariant()
                .Replace("_", string.Empty)
                .Replace("-", string.Empty);
        }

        private static bool IsProtectedName(string normalized)
        {
            if (string.IsNullOrEmpty(normalized))
                return false;

            if (normalized == "id" ||
                normalized == "key" ||
                normalized == "path" ||
                normalized == "identifier" ||
                normalized == "type" ||
                normalized == "kind" ||
                normalized == "tag" ||
                normalized == "code" ||
                normalized == "locale" ||
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

        private static void SafeStringResultPostfix(
            MethodBase __originalMethod, ref string __result)
        {
            if (!_enabled || !IsKorean() ||
                __originalMethod == null ||
                __originalMethod.DeclaringType == null ||
                string.IsNullOrEmpty(__result))
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            string translated;
            if (TryTranslateSource(catalog, __result, out translated))
                __result = translated;
        }

        private static void SafeVisibleStringArgsPrefix(
            MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() ||
                __originalMethod == null ||
                __originalMethod.DeclaringType == null ||
                __args == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            ParameterInfo[] parameters;
            try
            {
                parameters = __originalMethod.GetParameters();
            }
            catch
            {
                return;
            }

            for (int i = 0; i < parameters.Length && i < __args.Length; i++)
            {
                if (parameters[i].ParameterType != typeof(string) ||
                    !IsVisibleArgumentName(parameters[i].Name,
                        __originalMethod.Name))
                    continue;

                string original = __args[i] as string;
                if (string.IsNullOrEmpty(original))
                    continue;

                string translated;
                if (TryTranslateSource(catalog, original, out translated))
                    __args[i] = translated;
            }
        }

        private static bool IsKorean()
        {
            try
            {
                return IsKoreanCode(LocHelper.GetCurrentLocaleCode());
            }
            catch
            {
                return false;
            }
        }

        private static bool IsKoreanCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            return string.Equals(code, "ko",
                       StringComparison.OrdinalIgnoreCase) ||
                   code.StartsWith("ko-",
                       StringComparison.OrdinalIgnoreCase) ||
                   code.StartsWith("ko_",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static void RestoreNativeEntries()
        {
            foreach (KeyValuePair<LoadedMod, List<SavedValue>> pair
                in SavedValues)
            {
                LoadedMod loaded = pair.Key;
                if (loaded == null || loaded.Localization == null)
                    continue;

                foreach (SavedValue saved in pair.Value)
                {
                    Dictionary<string, string> dictionary;
                    if (!loaded.Localization.TryGetValue(
                            saved.Locale, out dictionary) ||
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
            if (_dynamicHarmony == null)
                return;

            try
            {
                _dynamicHarmony.UnpatchAll(DynamicHarmonyId);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Dynamic patch cleanup failed: " +
                    ex.Message);
            }

            _dynamicHarmony = null;
        }
    }
}
