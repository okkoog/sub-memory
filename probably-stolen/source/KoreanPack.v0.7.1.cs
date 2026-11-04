// Probably Stolen - generic Korean localization bridge v0.7.9 (v0.7.5-safe targeted literals)
// Official lifecycle only:
// - No ModLoader patch/reflection.
// - No runtime file/path access.
// - No AssemblyLoad interception.
// - No constructor/.cctor patching.
// - No global game UI/File/resource hooks.
// Target mods are identified only by LoadedMod values delivered by ModHook.OnModAssetsLoaded.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
            internal readonly List<TemplateRule> ExplicitTemplates =
                new List<TemplateRule>();
            internal readonly List<TemplateRule> DerivedTemplates =
                new List<TemplateRule>();
        }

        private sealed class TemplateRule
        {
            internal string Source;
            internal string Korean;
            internal string[] Literals;
            internal string[] Tokens;
        }

        private sealed class SavedValue
        {
            internal string Locale;
            internal string Key;
            internal bool Existed;
            internal string Value;
        }

        private sealed class MemberMutation
        {
            internal object Target;
            internal FieldInfo Field;
            internal PropertyInfo Property;
            internal string Original;
        }

        private sealed class MutationFrame
        {
            internal readonly List<MemberMutation> Mutations = new List<MemberMutation>();
        }

        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            internal static readonly ReferenceComparer Instance = new ReferenceComparer();
            public new bool Equals(object x, object y) { return ReferenceEquals(x, y); }
            public int GetHashCode(object obj) { return RuntimeHelpers.GetHashCode(obj); }
        }

        [ThreadStatic]
        private static Stack<MutationFrame> _mutationFrames;

        private static readonly object Sync = new object();

        private static readonly Dictionary<string, Catalog> Catalogs =
            new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> AmbiguousAliases =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<Assembly, Catalog> AssemblyCatalogs =
            new Dictionary<Assembly, Catalog>();

        private static readonly Dictionary<string, Catalog> RuntimeCatalogs =
            new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> FinalTooltipHitLogged =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static readonly HashSet<string> FinalTooltipMissLogged =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private static FieldInfo _richTextSlicesField;
        private static readonly Dictionary<Type, FieldInfo> SliceTextFields =
            new Dictionary<Type, FieldInfo>();
        private static readonly Dictionary<Type, FieldInfo> SliceLengthFields =
            new Dictionary<Type, FieldInfo>();

        private static readonly Dictionary<LoadedMod, List<SavedValue>> SavedValues =
            new Dictionary<LoadedMod, List<SavedValue>>();

        private static readonly HashSet<string> PatchedMethods =
            new HashSet<string>(StringComparer.Ordinal);

        private static readonly HashSet<string> DiagnosticOnce =
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
                RuntimeCatalogs.Clear();
                FinalTooltipHitLogged.Clear();
                FinalTooltipMissLogged.Clear();
                SliceTextFields.Clear();
                SliceLengthFields.Clear();
                _richTextSlicesField = null;
                SavedValues.Clear();
                PatchedMethods.Clear();
                DiagnosticOnce.Clear();
                PendingMods.Clear();

                _dynamicHarmony = new Harmony(DynamicHarmonyId);
                _catalogsReady = false;
                _enabled = true;

                ModHook.OnModAssetsLoaded += OnModAssetsLoaded;
                PatchFinalTooltipBridge();

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
                RuntimeCatalogs.Clear();
                FinalTooltipHitLogged.Clear();
                FinalTooltipMissLogged.Clear();
                PatchedMethods.Clear();
                DiagnosticOnce.Clear();
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
                        try
                        {
                            Debug.Log("[KoreanPack] Active locale code: '" +
                                (LocHelper.GetCurrentLocaleCode() ?? "<null>") + "'.");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogWarning("[KoreanPack] Locale probe failed: " + ex.Message);
                        }

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
                    string korean = DecodeEscapedCatalogText(row.Value ?? string.Empty);

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
                        AddExplicitSource(catalog, DecodeEscapedCatalogText(rowKey.Substring(2)), korean);
                        continue;
                    }

                    if (rowKey.StartsWith("S64|", StringComparison.Ordinal))
                    {
                        string source = DecodeBase64Url(rowKey.Substring(4));
                        if (!string.IsNullOrEmpty(source))
                            AddExplicitSource(catalog, DecodeEscapedCatalogText(source), korean);
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
            if (catalog == null) return;

            string[] incomingAliases = catalog.Aliases.ToArray();
            Catalog target = null;

            foreach (string alias in incomingAliases)
            {
                if (string.IsNullOrWhiteSpace(alias) ||
                    AmbiguousAliases.Contains(alias))
                    continue;

                Catalog existing;
                if (!Catalogs.TryGetValue(alias, out existing))
                    continue;

                if (target == null)
                    target = existing;
                else if (!ReferenceEquals(target, existing))
                {
                    // One supplement accidentally points at two already-distinct
                    // targets. Keep the targets separate and ignore that alias.
                    Catalogs.Remove(alias);
                    AmbiguousAliases.Add(alias);
                    Debug.LogWarning("[KoreanPack] Ambiguous catalog alias ignored: " + alias);
                }
            }

            if (target != null && !ReferenceEquals(target, catalog))
            {
                MergeCatalog(target, catalog);
                catalog = target;
            }

            foreach (string alias in catalog.Aliases.ToArray())
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

        private static void MergeCatalog(Catalog target, Catalog incoming)
        {
            if (target == null || incoming == null ||
                ReferenceEquals(target, incoming))
                return;

            foreach (string alias in incoming.Aliases)
                target.Aliases.Add(alias);

            bool targetIsSupplement = IsSupplementCatalog(target);
            bool incomingIsSupplement = IsSupplementCatalog(incoming);
            bool incomingWins = incomingIsSupplement && !targetIsSupplement;

            foreach (KeyValuePair<string, string> pair in incoming.ByKey)
            {
                string existing;
                if (!target.ByKey.TryGetValue(pair.Key, out existing) || incomingWins)
                    target.ByKey[pair.Key] = pair.Value;
            }

            foreach (KeyValuePair<string, string> pair in incoming.ExplicitBySource)
            {
                string existing;
                if (!target.ExplicitBySource.TryGetValue(pair.Key, out existing))
                    target.ExplicitBySource[pair.Key] = pair.Value;
                else if (incomingWins)
                    target.ExplicitBySource[pair.Key] = pair.Value;
            }

            // If the catalog that was registered first is itself the supplement,
            // its values already have priority; the base catalog only fills gaps.
            target.AmbiguousExplicitSources.Clear();
            RebuildExplicitTemplates(target);
        }

        private static bool IsSupplementCatalog(Catalog catalog)
        {
            return catalog != null &&
                !string.IsNullOrEmpty(catalog.PrimaryId) &&
                catalog.PrimaryId.StartsWith("supp_", StringComparison.OrdinalIgnoreCase);
        }

        private static void RebuildExplicitTemplates(Catalog catalog)
        {
            if (catalog == null) return;
            catalog.ExplicitTemplates.Clear();
            foreach (KeyValuePair<string, string> pair in catalog.ExplicitBySource)
                AddTemplateRule(catalog.ExplicitTemplates, pair.Key, pair.Value);
        }

        private static string DecodeEscapedCatalogText(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value
                .Replace("\\r\\n", "\n")
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t");
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

            RegisterRuntimeCatalogAliases(loaded, catalog);
            MergeNativeEntries(loaded, catalog);
            RebuildDerivedSourceMap(loaded, catalog);
            PatchTargetAssembly(loaded, catalog);

            Debug.Log("[KoreanPack] Applied to " + SafeModName(loaded) +
                ": " + catalog.ByKey.Count + " key(s), " +
                (catalog.ExplicitBySource.Count + catalog.DerivedBySource.Count) +
                " source mapping(s).");
        }


        private static void RegisterRuntimeCatalogAliases(
            LoadedMod loaded, Catalog catalog)
        {
            if (loaded == null || loaded.Manifest == null || catalog == null)
                return;

            Action<string> add = alias =>
            {
                if (string.IsNullOrWhiteSpace(alias))
                    return;
                RuntimeCatalogs[alias.Trim()] = catalog;
            };

            add(loaded.Manifest.ID);
            add(loaded.Manifest.Name);
            if (loaded.WorkshopId != 0)
                add(loaded.WorkshopId.ToString());
            if (loaded.ShadowedWorkshopId != 0)
                add(loaded.ShadowedWorkshopId.ToString());
        }

        private static Catalog LocateCatalogForItem(GameItem item, out string runtimeId)
        {
            runtimeId = string.Empty;
            if (item == null)
                return null;

            try
            {
                Catalog catalog;

                if (item.IsTag("MOD_ID_TAG"))
                {
                    runtimeId = item.GetTagReadonly("MOD_ID_TAG").GetString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(runtimeId))
                    {
                        if (RuntimeCatalogs.TryGetValue(runtimeId, out catalog))
                            return catalog;
                        if (Catalogs.TryGetValue(runtimeId, out catalog))
                            return catalog;
                    }
                }

                if (item.IsTag("MOD_NAME_TAG"))
                {
                    string modName = item.GetTagReadonly("MOD_NAME_TAG").GetString();
                    if (!string.IsNullOrWhiteSpace(modName))
                    {
                        if (RuntimeCatalogs.TryGetValue(modName, out catalog))
                        {
                            if (string.IsNullOrEmpty(runtimeId))
                                runtimeId = modName;
                            return catalog;
                        }

                        if (Catalogs.TryGetValue(modName, out catalog))
                        {
                            if (string.IsNullOrEmpty(runtimeId))
                                runtimeId = modName;
                            return catalog;
                        }
                    }
                }

                // Mirror the game's own ModSourceHelper.GetItemModName fallback.
                // Many workshop items do not carry MOD_ID_TAG/MOD_NAME_TAG at all;
                // the game still knows their source by looking up the registered
                // DirectoryMaster factory delegate. Use the same ownership path
                // so those items can select the correct Korean catalog.
                Delegate factory =
                    DirectoryMaster.GetFactory<GameItem>(item.identifier);
                LoadedMod owner = ModSourceHelper.FindModByDelegate(factory);

                if (owner != null && owner.Manifest != null)
                {
                    runtimeId = owner.Manifest.ID ?? string.Empty;

                    catalog = LocateCatalog(owner);
                    if (catalog != null)
                    {
                        RegisterRuntimeCatalogAliases(owner, catalog);
                        return catalog;
                    }

                    if (!string.IsNullOrWhiteSpace(runtimeId))
                    {
                        if (RuntimeCatalogs.TryGetValue(runtimeId, out catalog))
                            return catalog;
                        if (Catalogs.TryGetValue(runtimeId, out catalog))
                            return catalog;
                    }

                    string ownerName = owner.Manifest.Name;
                    if (!string.IsNullOrWhiteSpace(ownerName))
                    {
                        if (RuntimeCatalogs.TryGetValue(ownerName, out catalog))
                            return catalog;
                        if (Catalogs.TryGetValue(ownerName, out catalog))
                            return catalog;
                    }

                    if (owner.WorkshopId != 0)
                    {
                        string workshopId = owner.WorkshopId.ToString();
                        if (RuntimeCatalogs.TryGetValue(workshopId, out catalog))
                            return catalog;
                        if (Catalogs.TryGetValue(workshopId, out catalog))
                            return catalog;
                    }

                    if (owner.ShadowedWorkshopId != 0)
                    {
                        string workshopId = owner.ShadowedWorkshopId.ToString();
                        if (RuntimeCatalogs.TryGetValue(workshopId, out catalog))
                            return catalog;
                        if (Catalogs.TryGetValue(workshopId, out catalog))
                            return catalog;
                    }
                }
            }
            catch (Exception ex)
            {
                string key = "owner-lookup:" + item.identifier;
                if (FinalTooltipMissLogged.Add(key))
                {
                    Debug.LogWarning("[KoreanPack] Item owner lookup failed for " +
                        item.identifier + ": " + ex.Message);
                }
            }

            return null;
        }

        private static void PatchFinalTooltipBridge()
        {
            try
            {
                MethodInfo method = typeof(ModSourceHelper).GetMethod(
                    "CreateTooltip",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new Type[] { typeof(RichTextBuilder), typeof(GameItem) },
                    null);

                if (method == null)
                {
                    Debug.LogWarning("[KoreanPack] Final tooltip bridge target not found.");
                    return;
                }

                if (PatchDynamic(method, null,
                    nameof(SafeFinalItemTooltipPostfix),
                    "final-item-tooltip"))
                {
                    Debug.Log("[KoreanPack] Final item tooltip bridge enabled.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Final tooltip bridge failed: " +
                    ex.Message);
            }
        }

        private static void SafeFinalItemTooltipPostfix(
            RichTextBuilder builder, GameItem item)
        {
            if (!_enabled || !IsKorean() || builder == null || item == null)
                return;

            string runtimeId;
            Catalog catalog = LocateCatalogForItem(item, out runtimeId);
            if (catalog == null)
            {
                if (!string.IsNullOrEmpty(runtimeId) &&
                    FinalTooltipMissLogged.Add(runtimeId))
                {
                    Debug.LogWarning("[KoreanPack] Final tooltip catalog miss: " +
                        runtimeId);
                }
                return;
            }

            int translated = TranslateFinalTooltipSlices(catalog, builder);
            if (translated > 0)
            {
                string key = string.IsNullOrEmpty(runtimeId)
                    ? catalog.PrimaryId
                    : runtimeId;
                if (FinalTooltipHitLogged.Add(key))
                {
                    Debug.Log("[KoreanPack] Final tooltip translation active: " +
                        key + " (" + translated + " slice(s) on first hit).");
                }
            }
        }

        private static int TranslateFinalTooltipSlices(
            Catalog catalog, RichTextBuilder builder)
        {
            if (catalog == null || builder == null)
                return 0;

            try
            {
                if (_richTextSlicesField == null)
                {
                    _richTextSlicesField = typeof(RichTextBuilder).GetField(
                        "slices",
                        BindingFlags.Instance | BindingFlags.NonPublic);
                }

                if (_richTextSlicesField == null)
                    return 0;

                IEnumerable slices = _richTextSlicesField.GetValue(builder)
                    as IEnumerable;
                if (slices == null)
                    return 0;

                int changed = 0;
                foreach (object slice in slices)
                {
                    if (slice == null)
                        continue;

                    Type type = slice.GetType();
                    FieldInfo textField;
                    if (!SliceTextFields.TryGetValue(type, out textField))
                    {
                        textField = type.GetField("text",
                            BindingFlags.Instance |
                            BindingFlags.Public |
                            BindingFlags.NonPublic);
                        SliceTextFields[type] = textField;
                    }

                    if (textField == null || textField.FieldType != typeof(string))
                        continue;

                    string original = textField.GetValue(slice) as string;
                    string translated;
                    if (!TryTranslateTooltipSlice(
                            catalog, original, out translated))
                        continue;

                    if (string.Equals(original, translated,
                        StringComparison.Ordinal))
                        continue;

                    textField.SetValue(slice, translated);

                    FieldInfo lengthField;
                    if (!SliceLengthFields.TryGetValue(type, out lengthField))
                    {
                        lengthField = type.GetField("length",
                            BindingFlags.Instance |
                            BindingFlags.NonPublic);
                        SliceLengthFields[type] = lengthField;
                    }

                    if (lengthField != null &&
                        lengthField.FieldType == typeof(int))
                    {
                        lengthField.SetValue(slice, -1);
                    }

                    changed++;
                }

                return changed;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Final tooltip translation skipped: " +
                    ex.Message);
                return 0;
            }
        }

        private static bool TryTranslateTooltipSlice(
            Catalog catalog, string source, out string translated)
        {
            translated = source;
            if (catalog == null || string.IsNullOrEmpty(source))
                return false;

            if (TryTranslateSource(catalog, source, out translated))
                return true;

            int begin = 0;
            int end = source.Length;
            while (begin < end && char.IsWhiteSpace(source[begin])) begin++;
            while (end > begin && char.IsWhiteSpace(source[end - 1])) end--;

            if (begin > 0 || end < source.Length)
            {
                string core = source.Substring(begin, end - begin);
                string korean;
                if (TryTranslateSource(catalog, core, out korean))
                {
                    translated = source.Substring(0, begin) +
                        korean + source.Substring(end);
                    return true;
                }
            }

            // Composite tooltip lines such as "生命值：148" or
            // "[游戏卡][错版卡]" keep values/punctuation around short CJK labels.
            // Replace only exact catalog entries inside contiguous CJK runs.
            bool changed = false;
            string composite = Regex.Replace(source,
                @"[\u3400-\u9fff]+",
                match =>
                {
                    string korean;
                    if (TryTranslateSource(catalog, match.Value, out korean) &&
                        !string.Equals(korean, match.Value,
                            StringComparison.Ordinal))
                    {
                        changed = true;
                        return korean;
                    }
                    return match.Value;
                });

            if (changed)
            {
                translated = composite;
                return true;
            }

            return false;
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
            MergeLocale(loaded, catalog, saved, "kr");
            MergeLocale(loaded, catalog, saved, "kr-KR");

            try
            {
                string current = LocHelper.GetCurrentLocaleCode();
                if (!string.IsNullOrWhiteSpace(current))
                    MergeLocale(loaded, catalog, saved, current.Trim());
            }
            catch
            {
            }
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
            catalog.DerivedTemplates.Clear();

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
                AddTemplateRule(catalog.ExplicitTemplates, source, korean);
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
                AddTemplateRule(catalog.DerivedTemplates, source, korean);
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

            if (TryTranslateTemplate(catalog.ExplicitTemplates, source, out translated))
                return true;

            if (catalog.DerivedBySource.TryGetValue(source, out translated))
                return true;

            return TryTranslateTemplate(catalog.DerivedTemplates, source, out translated);
        }

        private static void AddTemplateRule(List<TemplateRule> rules,
            string source, string korean)
        {
            if (rules == null || string.IsNullOrEmpty(source) ||
                string.IsNullOrEmpty(korean))
                return;

            MatchCollection matches = Regex.Matches(source,
                @"\{\{[^{}]+\}\}|\{(?:\d+|[A-Za-z_][A-Za-z0-9_]*)\}");
            if (matches.Count == 0)
                return;

            var literals = new List<string>();
            var tokens = new List<string>();
            int position = 0;

            foreach (Match match in matches)
            {
                literals.Add(source.Substring(position, match.Index - position));
                tokens.Add(match.Value);
                position = match.Index + match.Length;
            }
            literals.Add(source.Substring(position));

            // Consecutive placeholders have no stable boundary at runtime.
            for (int i = 1; i < literals.Count - 1; i++)
                if (literals[i].Length == 0)
                    return;

            rules.Add(new TemplateRule
            {
                Source = source,
                Korean = korean,
                Literals = literals.ToArray(),
                Tokens = tokens.ToArray()
            });
        }

        private static bool TryTranslateTemplate(List<TemplateRule> rules,
            string source, out string translated)
        {
            translated = null;
            if (rules == null || string.IsNullOrEmpty(source))
                return false;

            foreach (TemplateRule rule in rules)
            {
                if (rule == null || rule.Literals == null ||
                    rule.Tokens == null || rule.Literals.Length != rule.Tokens.Length + 1)
                    continue;

                string first = rule.Literals[0] ?? string.Empty;
                if (!source.StartsWith(first, StringComparison.Ordinal))
                    continue;

                int position = first.Length;
                var captures = new Dictionary<string, string>(StringComparer.Ordinal);
                bool matched = true;

                for (int i = 0; i < rule.Tokens.Length; i++)
                {
                    string nextLiteral = rule.Literals[i + 1] ?? string.Empty;
                    int next;

                    if (nextLiteral.Length == 0)
                    {
                        if (i != rule.Tokens.Length - 1)
                        {
                            matched = false;
                            break;
                        }
                        next = source.Length;
                    }
                    else
                    {
                        next = source.IndexOf(nextLiteral, position,
                            StringComparison.Ordinal);
                        if (next < 0)
                        {
                            matched = false;
                            break;
                        }
                    }

                    string captured = source.Substring(position, next - position);
                    string previous;
                    if (captures.TryGetValue(rule.Tokens[i], out previous) &&
                        !string.Equals(previous, captured, StringComparison.Ordinal))
                    {
                        matched = false;
                        break;
                    }

                    captures[rule.Tokens[i]] = captured;
                    position = next + nextLiteral.Length;
                }

                if (!matched || position != source.Length)
                    continue;

                string result = rule.Korean;
                foreach (KeyValuePair<string, string> capture in captures)
                    result = result.Replace(capture.Key, capture.Value);

                translated = result;
                return !string.Equals(source, translated, StringComparison.Ordinal);
            }

            return false;
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
            if (assembly == null)
                return;

            string assemblyName = assembly.GetName().Name ?? string.Empty;
            int patched = 0;

            // v0.7.1 safety rule:
            // Never scan/patch every display-looking method in a target assembly.
            // Harmony compiles the patched method, and that can trigger unrelated
            // game type initializers while Unity Localization is still loading.
            // Only the static-analysis-confirmed text boundaries below are allowed.
            try
            {
                if (assemblyName.Equals("GameCardPK", StringComparison.OrdinalIgnoreCase))
                {
                    patched += PatchKnownType(assembly, "GameCardPK.GameCardFactory",
                        new KnownPatch("CreateCard", KnownPatchKind.GameItemResult),
                        new KnownPatch("CreateCard", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("CreateSkill", KnownPatchKind.GameItemResult),
                        new KnownPatch("CreateSkill", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("ApplySkillData", KnownPatchKind.ObjectArgs),
                        new KnownPatch("ApplySkillData", KnownPatchKind.CatalogLiterals));
                    patched += PatchKnownType(assembly, "GameCardPK.SkillText",
                        new KnownPatch("ForItem", KnownPatchKind.StringResult),
                        new KnownPatch("ForBattle", KnownPatchKind.StringResult),
                        new KnownPatch("PointLabel", KnownPatchKind.StringResult));
                    patched += PatchKnownType(assembly, "GameCardPK.ShowcaseBuyerSystem",
                        new KnownPatch("AppendBuyerCustomer", KnownPatchKind.TupleListArgs),
                        new KnownPatch("AppendBuyerCustomer", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("AppendSellerPrediction", KnownPatchKind.TupleListArgs),
                        new KnownPatch("AppendSellerPrediction", KnownPatchKind.CatalogLiterals));
                }
                else if (assemblyName.Equals("MidnightPawn", StringComparison.OrdinalIgnoreCase))
                {
                    patched += PatchKnownType(assembly, "MidnightPawn.Game.TentacleBody",
                        new KnownPatch("Create", KnownPatchKind.GameItemResult),
                        new KnownPatch("Create", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("BuildWindow", KnownPatchKind.PixelWindowResult),
                        new KnownPatch("BuildWindow", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("ShowReport", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("DelayReport", KnownPatchKind.VisibleStringArgs),
                        new KnownPatch("DelayReport", KnownPatchKind.CatalogLiterals));
                    patched += PatchKnownType(assembly, "MidnightPawn.Game.MaidenTooltip",
                        new KnownPatch("OnTooltip", KnownPatchKind.ObjectArgs),
                        new KnownPatch("OnTooltip", KnownPatchKind.CatalogLiterals));
                    patched += PatchKnownType(assembly, "MidnightPawn.Game.ItemFactory",
                        new KnownPatch("UpdateMaidenDescription", KnownPatchKind.ObjectArgs),
                        new KnownPatch("UpdateMaidenDescription", KnownPatchKind.CatalogLiterals));
                    patched += PatchKnownType(assembly, "MidnightPawn.Game.MaidenTooltip");
                    patched += PatchKnownType(assembly, "MidnightPawn.Game.ItemFactory");
                }
                else if (assemblyName.Equals("ItemStorage", StringComparison.OrdinalIgnoreCase))
                {
                    patched += PatchKnownType(assembly, "ItemStorage.BoxItem",
                        new KnownPatch("Create", KnownPatchKind.GameItemResult));
                    patched += PatchKnownType(assembly, "ItemStorage.BoxWindow",
                        new KnownPatch("UpdateTooltip", KnownPatchKind.ObjectArgs),
                        new KnownPatch("UpdateTooltip", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("EnsureWindow", KnownPatchKind.StaticVisibleState),
                        new KnownPatch("EnsureWindow", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("RefreshButtons", KnownPatchKind.StaticVisibleState),
                        new KnownPatch("RefreshButtons", KnownPatchKind.CatalogLiterals));
                    patched += PatchKnownType(assembly, "ItemStorage.BoxLabel",
                        new KnownPatch("BaseNameOf", KnownPatchKind.StringResult),
                        new KnownPatch("BaseNameOf", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("FormatText", KnownPatchKind.StringResult),
                        new KnownPatch("FormatText", KnownPatchKind.CatalogLiterals),
                        new KnownPatch("Apply", KnownPatchKind.StaticVisibleState),
                        new KnownPatch("Tick", KnownPatchKind.StaticVisibleState),
                        new KnownPatch("OnItemRenamed", KnownPatchKind.StaticVisibleState));
                }
                else if (assemblyName.Equals("XIAOWONightAuction", StringComparison.OrdinalIgnoreCase))
                {
                    patched += PatchKnownType(assembly, "XIAOWONightAuction.AuctionText",
                        new KnownPatch("Select", KnownPatchKind.BilingualResult),
                        new KnownPatch("DisplayLog", KnownPatchKind.StringResult),
                        new KnownPatch("Get", KnownPatchKind.StringResult));
                    patched += PatchKnownType(assembly, "XIAOWONightAuction.AuctionExclusiveFactory",
                        new KnownPatch("ApplyLocalizedText", KnownPatchKind.ObjectArgs));
                    patched += PatchKnownType(assembly, "XIAOWONightAuction.AuctionPhone",
                        new KnownPatch("CreateCard", KnownPatchKind.GameItemResult),
                        new KnownPatch("LocalizeCard", KnownPatchKind.ObjectArgs),
                        new KnownPatch("BuildDialogue", KnownPatchKind.DialogueResult),
                        new KnownPatch("BuildBookingDialogue", KnownPatchKind.DialogueResult),
                        new KnownPatch("Ending", KnownPatchKind.DialogueResult));
                }
                else if (assemblyName.Equals("BrewingExpansion", StringComparison.OrdinalIgnoreCase))
                {
                    patched += PatchKnownType(assembly, "BrewingExpansion.Formal.Figurines",
                        new KnownPatch("Create", KnownPatchKind.GameItemResult));
                    patched += PatchKnownType(assembly, "BrewingExpansion.Formal.FormalVisitors",
                        new KnownPatch("PhoneDialogue", KnownPatchKind.DialogueResult),
                        new KnownPatch("Nini", KnownPatchKind.StoreClientResult));
                }
                else if (assemblyName.Equals("CorpPresence", StringComparison.OrdinalIgnoreCase))
                {
                    // Important: patch ONLY CorpPresence.L.T.
                    // Do not patch SilverBlood.AddLiquid or any other Corp method:
                    // compiling AddLiquid caused Liquid..cctor to run during asset loading,
                    // corrupting Unity Localization operation handles.
                    patched += PatchKnownType(assembly, "CorpPresence.L",
                        new KnownPatch("T", KnownPatchKind.BilingualResult));
                    patched += PatchKnownType(assembly, "CorpPresence.UI.MenuLabels",
                        new KnownPatch("Get", KnownPatchKind.StringArrayResult));
                    patched += PatchKnownType(assembly, "CorpPresence.UI.Ui",
                        new KnownPatch("Button", KnownPatchKind.VisibleStringArgs),
                        new KnownPatch("Label", KnownPatchKind.VisibleStringArgs));
                    patched += PatchKnownType(assembly, "CorpPresence.UI.CompanyWindow",
                        new KnownPatch("DrawPastel", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawPastelOrder", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("PastelBribeCell", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawPastelDealer", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawPastelTraffic", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawPastelFooter", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawDavre", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawDavreOrder", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawDavreLeviathan", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawDavreCanning", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawSpacer", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawSpacerMutual", KnownPatchKind.GuiLabelTranspiler),
                        new KnownPatch("DrawSpacerCredits", KnownPatchKind.GuiLabelTranspiler));
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[KoreanPack] Safe target adapter failed for " +
                    assemblyName + ": " + ex.Message);
            }

            if (patched > 0)
                Debug.Log("[KoreanPack] Safe target adapter: " + assemblyName +
                    " -> " + patched + " patch(es).");
        }

        private enum KnownPatchKind
        {
            StringResult,
            StringArrayResult,
            BilingualResult,
            VisibleStringArgs,
            GuiLabelTranspiler,
            LocalizerResult,
            GameItemResult,
            DialogueResult,
            PixelWindowResult,
            StoreClientResult,
            ObjectArgs,
            TupleListArgs,
            StaticDisplayState,
            StaticVisibleState,
            CatalogLiterals
        }

        private sealed class KnownPatch
        {
            internal readonly string MethodName;
            internal readonly KnownPatchKind Kind;

            internal KnownPatch(string methodName, KnownPatchKind kind)
            {
                MethodName = methodName;
                Kind = kind;
            }
        }

        private static int PatchKnownType(
            Assembly assembly, string typeName, params KnownPatch[] specs)
        {
            if (assembly == null || specs == null || specs.Length == 0)
                return 0;

            Type type;
            try
            {
                type = assembly.GetType(typeName, false);
            }
            catch
            {
                return 0;
            }

            if (type == null)
                return 0;

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
                return 0;
            }

            int patched = 0;

            foreach (KnownPatch spec in specs)
            {
                if (spec == null || string.IsNullOrEmpty(spec.MethodName))
                    continue;

                foreach (MethodInfo method in methods)
                {
                    if (method == null || method.IsAbstract ||
                        method.ContainsGenericParameters || method.IsConstructor ||
                        !string.Equals(method.Name, spec.MethodName,
                            StringComparison.Ordinal))
                        continue;

                    try
                    {
                        switch (spec.Kind)
                        {
                            case KnownPatchKind.StringResult:
                                if (method.ReturnType == typeof(string) &&
                                    PatchDynamic(method, null,
                                        nameof(SafeStringResultPostfix),
                                        "safe-string-result"))
                                    patched++;
                                break;

                            case KnownPatchKind.StringArrayResult:
                                if (method.ReturnType == typeof(string[]) &&
                                    PatchDynamic(method, null,
                                        nameof(SafeStringArrayResultPostfix),
                                        "safe-string-array-result"))
                                    patched++;
                                break;

                            case KnownPatchKind.BilingualResult:
                                if (method.ReturnType == typeof(string))
                                {
                                    if (PatchDynamic(method, null,
                                        nameof(SafeBilingualStringResultPostfix),
                                        "safe-bilingual-string"))
                                        patched++;
                                }
                                else if (method.ReturnType == typeof(string[]))
                                {
                                    if (PatchDynamic(method, null,
                                        nameof(SafeBilingualStringArrayResultPostfix),
                                        "safe-bilingual-array"))
                                        patched++;
                                }
                                break;

                            case KnownPatchKind.VisibleStringArgs:
                                if (HasVisibleStringArguments(method) &&
                                    PatchDynamic(method,
                                        nameof(SafeVisibleStringArgsPrefix),
                                        null,
                                        "safe-visible-string-args"))
                                    patched++;
                                break;

                            case KnownPatchKind.GuiLabelTranspiler:
                                if (PatchDynamicTranspiler(method,
                                    nameof(SafeCorpGuiLabelTranspiler),
                                    "safe-corp-gui-label"))
                                    patched++;
                                break;

                            case KnownPatchKind.LocalizerResult:
                                if (method.ReturnType == typeof(string))
                                {
                                    if (PatchDynamic(method, null,
                                        nameof(SafeStringResultPostfix),
                                        "safe-localizer-string"))
                                        patched++;
                                }
                                else if (method.ReturnType == typeof(string[]))
                                {
                                    if (PatchDynamic(method, null,
                                        nameof(SafeStringArrayResultPostfix),
                                        "safe-localizer-array"))
                                        patched++;
                                }
                                break;

                            case KnownPatchKind.GameItemResult:
                                if (method.ReturnType == typeof(GameItem) &&
                                    PatchDynamic(method, null,
                                        nameof(SafeGameItemResultPostfix),
                                        "safe-gameitem-result"))
                                    patched++;
                                break;

                            case KnownPatchKind.DialogueResult:
                                if (method.ReturnType == typeof(Dialogue) &&
                                    PatchDynamic(method, null,
                                        nameof(SafeDialogueResultPostfix),
                                        "safe-dialogue-result"))
                                    patched++;
                                break;

                            case KnownPatchKind.PixelWindowResult:
                                if (method.ReturnType == typeof(PixelWindow) &&
                                    PatchDynamic(method, null,
                                        nameof(SafePixelWindowResultPostfix),
                                        "safe-pixelwindow-result"))
                                    patched++;
                                break;

                            case KnownPatchKind.StoreClientResult:
                                if (method.ReturnType == typeof(StoreClient) &&
                                    PatchDynamic(method, null,
                                        nameof(SafeStoreClientResultPostfix),
                                        "safe-storeclient-result"))
                                    patched++;
                                break;

                            case KnownPatchKind.ObjectArgs:
                                if (HasObjectArguments(method) &&
                                    PatchDynamic(method, null,
                                        nameof(SafeObjectArgsPostfix),
                                        "safe-object-args"))
                                    patched++;
                                break;

                            case KnownPatchKind.TupleListArgs:
                                if (PatchDynamic(method, null,
                                    nameof(SafeTupleListArgsPostfix),
                                    "safe-tuple-list-args"))
                                    patched++;
                                break;

                            case KnownPatchKind.StaticDisplayState:
                                if (PatchDynamic(method, null,
                                    nameof(SafeTargetStaticStatePostfix),
                                    "safe-static-display-state"))
                                    patched++;
                                break;

                            case KnownPatchKind.StaticVisibleState:
                                if (PatchDynamic(method, null,
                                    nameof(SafeTargetVisibleStaticStatePostfix),
                                    "safe-static-visible-state"))
                                    patched++;
                                break;

                            case KnownPatchKind.CatalogLiterals:
                                if (PatchDynamicTranspiler(method,
                                    nameof(SafeCatalogLiteralTranspiler),
                                    "safe-catalog-literals"))
                                    patched++;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("[KoreanPack] Safe patch skipped: " +
                            typeName + "." + method.Name + " (" +
                            ex.Message + ")");
                    }
                }
            }

            return patched;
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

        private static bool PatchDynamicTranspiler(
            MethodBase method, string transpilerName, string kind)
        {
            if (_dynamicHarmony == null || method == null ||
                string.IsNullOrEmpty(transpilerName))
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

            HarmonyMethod transpiler =
                new HarmonyMethod(typeof(KoreanPack), transpilerName);
            _dynamicHarmony.Patch(method, transpiler: transpiler);
            PatchedMethods.Add(id);
            return true;
        }

        private static IEnumerable<CodeInstruction> SafeCatalogLiteralTranspiler(
            IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            MethodInfo helper = typeof(KoreanPack).GetMethod(
                nameof(TranslateCatalogLiteral),
                BindingFlags.Static | BindingFlags.NonPublic);

            string assemblyName = string.Empty;
            try
            {
                if (original != null && original.DeclaringType != null)
                    assemblyName = original.DeclaringType.Assembly.GetName().Name ?? string.Empty;
            }
            catch
            {
            }

            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;

                if (helper == null || instruction == null ||
                    instruction.opcode != OpCodes.Ldstr ||
                    !(instruction.operand is string) ||
                    string.IsNullOrEmpty((string)instruction.operand))
                    continue;

                yield return new CodeInstruction(OpCodes.Ldstr, assemblyName);
                yield return new CodeInstruction(OpCodes.Call, helper);
            }
        }

        private static string TranslateCatalogLiteral(
            string source, string assemblyName)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(source))
                return source;

            string alias = null;
            if (string.Equals(assemblyName, "ItemStorage", StringComparison.OrdinalIgnoreCase))
                alias = "Dream_Item_Storage";
            else if (string.Equals(assemblyName, "MidnightPawn", StringComparison.OrdinalIgnoreCase))
                alias = "lewd_meme";
            else if (string.Equals(assemblyName, "GameCardPK", StringComparison.OrdinalIgnoreCase))
                alias = "game_card_pk";
            else if (string.Equals(assemblyName, "XIAOWONightAuction", StringComparison.OrdinalIgnoreCase))
                alias = "xiaowo_night_auction";
            else if (string.Equals(assemblyName, "BrewingExpansion", StringComparison.OrdinalIgnoreCase))
                alias = "brewing_expansion";

            if (string.IsNullOrEmpty(alias))
                return source;

            Catalog catalog;
            if ((!RuntimeCatalogs.TryGetValue(alias, out catalog) &&
                 !Catalogs.TryGetValue(alias, out catalog)) ||
                catalog == null)
                return source;

            string translated;
            if (TryTranslateTooltipSlice(catalog, source, out translated))
                return translated;

            return source;
        }

        private static IEnumerable<CodeInstruction> SafeCorpGuiLabelTranspiler(
            IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo wrapper = typeof(KoreanPack).GetMethod(
                nameof(CorpGuiLabel),
                BindingFlags.Static | BindingFlags.NonPublic);

            foreach (CodeInstruction instruction in instructions)
            {
                if (wrapper != null)
                {
                    MethodInfo called = instruction.operand as MethodInfo;
                    if (called != null &&
                        called.DeclaringType == typeof(GUI) &&
                        string.Equals(called.Name, "Label",
                            StringComparison.Ordinal))
                    {
                        ParameterInfo[] parameters = called.GetParameters();
                        if (parameters.Length == 3 &&
                            parameters[0].ParameterType == typeof(Rect) &&
                            parameters[1].ParameterType == typeof(string) &&
                            parameters[2].ParameterType == typeof(GUIStyle))
                        {
                            instruction.operand = wrapper;
                        }
                    }
                }

                yield return instruction;
            }
        }

        private static void CorpGuiLabel(
            Rect position, string text, GUIStyle style)
        {
            GUI.Label(position,
                TranslateKnownCatalogText("CorpPresence", text), style);
        }

        private static string TranslateKnownCatalogText(
            string alias, string source)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(source))
                return source;

            Catalog catalog;
            if ((!RuntimeCatalogs.TryGetValue(alias, out catalog) &&
                 !Catalogs.TryGetValue(alias, out catalog)) ||
                catalog == null)
                return source;

            string translated;
            if (TryTranslateTooltipSlice(catalog, source, out translated))
                return translated;

            return source;
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

        private static bool HasStringArguments(MethodInfo method)
        {
            if (method == null || method.IsSpecialName)
                return false;
            try
            {
                if (method.GetMethodBody() == null) return false;
                return method.GetParameters().Any(p => p.ParameterType == typeof(string));
            }
            catch
            {
                return false;
            }
        }

        private static bool HasObjectArguments(MethodInfo method)
        {
            if (method == null || method.IsSpecialName)
                return false;
            try
            {
                if (method.GetMethodBody() == null) return false;
                foreach (ParameterInfo p in method.GetParameters())
                {
                    Type t = p.ParameterType;
                    if (t.IsByRef) t = t.GetElementType();
                    if (t == null || t == typeof(string) || t.IsValueType ||
                        t == typeof(Type) || typeof(Delegate).IsAssignableFrom(t))
                        continue;
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private static bool LooksLikeDisplayCarrierMethod(MethodInfo method)
        {
            if (method == null || method.DeclaringType == null)
                return false;

            string name = NormalizeName(method.Name);
            string typeName = NormalizeName(method.DeclaringType.Name);
            string combined = typeName + " " + name;

            string[] words =
            {
                "window", "panel", "tooltip", "button", "label", "text",
                "dialog", "display", "item", "card", "factory", "create",
                "build", "register", "refresh", "update", "apply", "localize",
                "catalog", "phone", "shop", "machine", "ui"
            };

            return words.Any(combined.Contains);
        }

        private static void SafeStringArgsPrefix(
            MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null || __args == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            ParameterInfo[] parameters;
            try { parameters = __originalMethod.GetParameters(); }
            catch { return; }

            for (int i = 0; i < parameters.Length && i < __args.Length; i++)
            {
                if (parameters[i].ParameterType != typeof(string))
                    continue;

                string parameterName = NormalizeName(parameters[i].Name);
                if (IsProtectedName(parameterName))
                    continue;

                string original = __args[i] as string;
                if (string.IsNullOrEmpty(original))
                    continue;

                // Named visible parameters are always eligible. For unknown/value
                // parameters, require text-like content so an ASCII identifier is
                // never changed merely because it happens to share a catalog row.
                if (!IsVisibleArgumentName(parameters[i].Name,
                        __originalMethod.Name) &&
                    !LooksLikeHumanText(original))
                    continue;

                string translated;
                if (TryTranslateSource(catalog, original, out translated))
                    __args[i] = translated;
            }
        }

        private static bool LooksLikeHumanText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                char ch = value[i];
                if ((ch >= '\u3400' && ch <= '\u9fff') ||
                    char.IsWhiteSpace(ch) ||
                    char.IsPunctuation(ch))
                    return true;
            }

            // Pure ASCII token-like values are treated as identifiers by default.
            return false;
        }

        private static void SafeObjectArgsPrefix(
            MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null || __args == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            if (_mutationFrames == null)
                _mutationFrames = new Stack<MutationFrame>();

            MutationFrame frame = new MutationFrame();
            _mutationFrames.Push(frame);

            Assembly targetAssembly = __originalMethod.DeclaringType.Assembly;
            HashSet<object> visited =
                new HashSet<object>(ReferenceComparer.Instance);
            int budget = 160;

            for (int i = 0; i < __args.Length && budget > 0; i++)
                TranslateObjectGraph(catalog, __args[i], targetAssembly,
                    true, true, frame, visited, 0, ref budget);
        }

        private static void SafeObjectArgsPostfix(
            MethodBase __originalMethod, object[] __args)
        {
            MutationFrame frame = null;
            if (_mutationFrames != null && _mutationFrames.Count > 0)
                frame = _mutationFrames.Pop();

            RestoreMutations(frame);

            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null || __args == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            // After the call, mutate only game/display carrier arguments. Target
            // mod config/data objects were temporary inputs and stay restored.
            Assembly targetAssembly = __originalMethod.DeclaringType.Assembly;
            HashSet<object> visited =
                new HashSet<object>(ReferenceComparer.Instance);
            int budget = 180;

            for (int i = 0; i < __args.Length && budget > 0; i++)
                TranslateObjectGraph(catalog, __args[i], targetAssembly,
                    false, false, null, visited, 0, ref budget);
        }

        private static void SafeTupleListArgsPostfix(
            MethodBase __originalMethod, object[] __args)
        {
            LogPatchHit(__originalMethod);

            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null || __args == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            for (int a = 0; a < __args.Length; a++)
            {
                IList list = __args[a] as IList;
                if (list == null || list.IsReadOnly)
                    continue;

                int count;
                try { count = Math.Min(list.Count, 160); }
                catch { continue; }

                for (int n = 0; n < count; n++)
                {
                    object boxed;
                    try { boxed = list[n]; }
                    catch { continue; }
                    if (boxed == null) continue;

                    Type type = boxed.GetType();
                    if (!type.IsValueType ||
                        !(type.FullName ?? string.Empty).StartsWith(
                            "System.ValueTuple", StringComparison.Ordinal))
                        continue;

                    bool changed = false;
                    FieldInfo[] fields;
                    try
                    {
                        fields = type.GetFields(
                            BindingFlags.Instance | BindingFlags.Public);
                    }
                    catch
                    {
                        continue;
                    }

                    foreach (FieldInfo field in fields)
                    {
                        if (field == null || field.FieldType != typeof(string))
                            continue;

                        string original;
                        try { original = field.GetValue(boxed) as string; }
                        catch { continue; }

                        string translated;
                        if (string.IsNullOrEmpty(original) ||
                            !TryTranslateSource(catalog, original, out translated) ||
                            string.Equals(original, translated,
                                StringComparison.Ordinal))
                            continue;

                        try
                        {
                            field.SetValue(boxed, translated);
                            changed = true;
                        }
                        catch
                        {
                        }
                    }

                    if (changed)
                    {
                        try { list[n] = boxed; }
                        catch { }
                    }
                }
            }
        }

        private static void SafePixelWindowResultPostfix(
            MethodBase __originalMethod, PixelWindow __result)
        {
            TranslateCarrierResult(__originalMethod, __result);
        }

        private static void SafeDialogueResultPostfix(
            MethodBase __originalMethod, Dialogue __result)
        {
            TranslateCarrierResult(__originalMethod, __result);
        }

        private static void SafeStoreClientResultPostfix(
            MethodBase __originalMethod, StoreClient __result)
        {
            TranslateCarrierResult(__originalMethod, __result);
        }

        private static void TranslateCarrierResult(
            MethodBase originalMethod, object result)
        {
            LogPatchHit(originalMethod);

            if (!_enabled || !IsKorean() || result == null ||
                originalMethod == null || originalMethod.DeclaringType == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    originalMethod.DeclaringType.Assembly, out catalog))
                return;

            HashSet<object> visited =
                new HashSet<object>(ReferenceComparer.Instance);
            int budget = 240;
            TranslateObjectGraph(catalog, result,
                originalMethod.DeclaringType.Assembly,
                false, false, null, visited, 0, ref budget);
        }

        private static void SafeGameItemResultPostfix(
            MethodBase __originalMethod, GameItem __result)
        {
            LogPatchHit(__originalMethod);

            if (!_enabled || !IsKorean() || __result == null ||
                __originalMethod == null || __originalMethod.DeclaringType == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            HashSet<object> visited =
                new HashSet<object>(ReferenceComparer.Instance);
            int budget = 120;
            TranslateObjectGraph(catalog, __result,
                __originalMethod.DeclaringType.Assembly,
                false, false, null, visited, 0, ref budget);
        }

        private static void SafeTargetStaticStatePostfix(
            MethodBase __originalMethod)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            Type type = __originalMethod.DeclaringType;
            FieldInfo[] fields;
            try
            {
                fields = type.GetFields(BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);
            }
            catch
            {
                return;
            }

            HashSet<object> visited =
                new HashSet<object>(ReferenceComparer.Instance);
            int budget = 220;

            foreach (FieldInfo field in fields)
            {
                if (budget <= 0 || field == null || field.IsLiteral)
                    break;

                object value;
                try { value = field.GetValue(null); }
                catch { continue; }

                TranslateObjectGraph(catalog, value, type.Assembly,
                    false, false, null, visited, 0, ref budget);
            }
        }

        private static void SafeTargetVisibleStaticStatePostfix(
            MethodBase __originalMethod)
        {
            LogPatchHit(__originalMethod);

            if (!_enabled || !IsKorean() || __originalMethod == null ||
                __originalMethod.DeclaringType == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            Type type = __originalMethod.DeclaringType;
            FieldInfo[] fields;
            try
            {
                fields = type.GetFields(BindingFlags.Static |
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);
            }
            catch
            {
                return;
            }

            HashSet<object> visited =
                new HashSet<object>(ReferenceComparer.Instance);
            int budget = 260;

            foreach (FieldInfo field in fields)
            {
                if (budget <= 0 || field == null || field.IsLiteral)
                    break;

                object value;
                try { value = field.GetValue(null); }
                catch { continue; }

                // v0.7.5-safe rebuild:
                // only explicitly whitelisted UI-holder types use permanent
                // visible-string translation. No IL literal replacement.
                TranslateObjectGraph(catalog, value, type.Assembly,
                    true, false, null, visited, 0, ref budget);
            }
        }

        private static void RestoreMutations(MutationFrame frame)
        {
            if (frame == null) return;

            for (int i = frame.Mutations.Count - 1; i >= 0; i--)
            {
                MemberMutation mutation = frame.Mutations[i];
                try
                {
                    if (mutation.Field != null)
                        mutation.Field.SetValue(mutation.Target, mutation.Original);
                    else if (mutation.Property != null)
                        mutation.Property.SetValue(mutation.Target,
                            mutation.Original, null);
                }
                catch
                {
                }
            }
        }

        private static void TranslateObjectGraph(
            Catalog catalog,
            object value,
            Assembly targetAssembly,
            bool allowTargetOwnedStrings,
            bool recordMutations,
            MutationFrame frame,
            HashSet<object> visited,
            int depth,
            ref int budget)
        {
            if (catalog == null || value == null || budget <= 0 || depth > 3)
                return;

            Type type = value.GetType();
            if (type == typeof(string) || type.IsPrimitive || type.IsEnum ||
                type == typeof(decimal) || typeof(Delegate).IsAssignableFrom(type))
                return;

            if (!type.IsValueType)
            {
                if (!visited.Add(value)) return;
                budget--;
            }

            IDictionary dictionary = value as IDictionary;
            if (dictionary != null)
            {
                int count = 0;
                foreach (DictionaryEntry entry in dictionary)
                {
                    if (++count > 80 || budget <= 0) break;
                    TranslateObjectGraph(catalog, entry.Value, targetAssembly,
                        allowTargetOwnedStrings, recordMutations, frame,
                        visited, depth + 1, ref budget);
                }
                return;
            }

            IEnumerable enumerable = value as IEnumerable;
            if (enumerable != null && !(value is string))
            {
                int count = 0;
                try
                {
                    foreach (object child in enumerable)
                    {
                        if (++count > 80 || budget <= 0) break;
                        TranslateObjectGraph(catalog, child, targetAssembly,
                            allowTargetOwnedStrings, recordMutations, frame,
                            visited, depth + 1, ref budget);
                    }
                }
                catch
                {
                }
                return;
            }

            bool targetOwned = type.Assembly == targetAssembly;
            bool carrier = IsDisplayCarrierType(type);

            if (!targetOwned && !carrier)
                return;
            if (targetOwned && !allowTargetOwnedStrings && !carrier)
            {
                // Still descend into collection/carrier fields below, but never
                // permanently rewrite the target mod's own config string fields.
            }

            FieldInfo[] fields;
            try
            {
                fields = type.GetFields(BindingFlags.Instance |
                    BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch
            {
                fields = Array.Empty<FieldInfo>();
            }

            foreach (FieldInfo field in fields)
            {
                if (field == null || field.IsStatic || budget <= 0)
                    continue;

                object child;
                try { child = field.GetValue(value); }
                catch { continue; }

                if (field.FieldType == typeof(string))
                {
                    if ((!targetOwned || allowTargetOwnedStrings || carrier) &&
                        IsVisibleMemberName(field.Name, true))
                    {
                        string original = child as string;
                        string translated;
                        if (!string.IsNullOrEmpty(original) &&
                            TryTranslateSource(catalog, original, out translated) &&
                            !string.Equals(original, translated,
                                StringComparison.Ordinal))
                        {
                            try
                            {
                                if (recordMutations && frame != null)
                                    frame.Mutations.Add(new MemberMutation
                                    {
                                        Target = value,
                                        Field = field,
                                        Original = original
                                    });
                                field.SetValue(value, translated);
                            }
                            catch
                            {
                            }
                        }
                    }
                    continue;
                }

                if (ShouldDescendInto(field.FieldType, targetAssembly))
                    TranslateObjectGraph(catalog, child, targetAssembly,
                        allowTargetOwnedStrings, recordMutations, frame,
                        visited, depth + 1, ref budget);
            }

            PropertyInfo[] properties;
            try
            {
                properties = type.GetProperties(BindingFlags.Instance |
                    BindingFlags.Public);
            }
            catch
            {
                properties = Array.Empty<PropertyInfo>();
            }

            foreach (PropertyInfo property in properties)
            {
                if (property == null || budget <= 0 ||
                    property.GetIndexParameters().Length != 0)
                    continue;

                MethodInfo getter = property.GetGetMethod(false);
                MethodInfo setter = property.GetSetMethod(false);
                if (getter == null)
                    continue;

                if (property.PropertyType == typeof(string))
                {
                    if (setter == null ||
                        (targetOwned && !allowTargetOwnedStrings && !carrier) ||
                        !IsVisibleMemberName(property.Name, true))
                        continue;

                    string original;
                    try { original = property.GetValue(value, null) as string; }
                    catch { continue; }

                    string translated;
                    if (string.IsNullOrEmpty(original) ||
                        !TryTranslateSource(catalog, original, out translated) ||
                        string.Equals(original, translated, StringComparison.Ordinal))
                        continue;

                    try
                    {
                        if (recordMutations && frame != null)
                            frame.Mutations.Add(new MemberMutation
                            {
                                Target = value,
                                Property = property,
                                Original = original
                            });
                        property.SetValue(value, translated, null);
                    }
                    catch
                    {
                    }
                    continue;
                }

                if (!ShouldDescendInto(property.PropertyType, targetAssembly) ||
                    !carrier)
                    continue;

                object child;
                try { child = property.GetValue(value, null); }
                catch { continue; }

                TranslateObjectGraph(catalog, child, targetAssembly,
                    allowTargetOwnedStrings, recordMutations, frame,
                    visited, depth + 1, ref budget);
            }
        }

        private static bool ShouldDescendInto(Type type, Assembly targetAssembly)
        {
            if (type == null || type == typeof(string) || type.IsPrimitive ||
                type.IsEnum || type == typeof(decimal) ||
                typeof(Delegate).IsAssignableFrom(type))
                return false;

            return type.Assembly == targetAssembly ||
                   typeof(IEnumerable).IsAssignableFrom(type) ||
                   IsDisplayCarrierType(type);
        }

        private static bool IsDisplayCarrierType(Type type)
        {
            if (type == null) return false;
            string name = ((type.FullName ?? type.Name) ?? string.Empty)
                .ToLowerInvariant();

            return name.Contains("gameitem") ||
                   name.Contains("richtext") ||
                   name.Contains("machinery") ||
                   name.Contains("button") ||
                   name.Contains("label") ||
                   name.Contains("tooltip") ||
                   name.Contains("dialog") ||
                   name.Contains("pixelelement") ||
                   name.Contains("pixelwindow") ||
                   name.Contains("storeclient") ||
                   name.Contains("textmesh") ||
                   name.EndsWith(".text", StringComparison.Ordinal) ||
                   name.Contains("uielement");
        }

        private static void SafeBilingualStringResultPostfix(
            MethodBase __originalMethod, object[] __args, ref string __result)
        {
            if (!_enabled || !IsKorean() ||
                __originalMethod == null ||
                __originalMethod.DeclaringType == null)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            string translated;
            if (__args != null)
            {
                for (int i = 0; i < __args.Length; i++)
                {
                    string candidate = __args[i] as string;
                    if (!string.IsNullOrEmpty(candidate) &&
                        TryTranslateSource(catalog, candidate, out translated))
                    {
                        __result = translated;
                        return;
                    }
                }
            }

            if (!string.IsNullOrEmpty(__result) &&
                TryTranslateSource(catalog, __result, out translated))
                __result = translated;
        }

        private static void SafeBilingualStringArrayResultPostfix(
            MethodBase __originalMethod, object[] __args, ref string[] __result)
        {
            if (!_enabled || !IsKorean() ||
                __originalMethod == null ||
                __originalMethod.DeclaringType == null ||
                __result == null || __result.Length == 0)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            for (int i = 0; i < __result.Length; i++)
            {
                string translated;
                bool done = false;

                if (__args != null)
                {
                    for (int a = 0; a < __args.Length; a++)
                    {
                        string[] candidates = __args[a] as string[];
                        if (candidates == null || i >= candidates.Length)
                            continue;

                        string candidate = candidates[i];
                        if (!string.IsNullOrEmpty(candidate) &&
                            TryTranslateSource(catalog, candidate, out translated))
                        {
                            __result[i] = translated;
                            done = true;
                            break;
                        }
                    }
                }

                if (done)
                    continue;

                string original = __result[i];
                if (!string.IsNullOrEmpty(original) &&
                    TryTranslateSource(catalog, original, out translated))
                    __result[i] = translated;
            }
        }

        private static void SafeStringArrayResultPostfix(
            MethodBase __originalMethod, ref string[] __result)
        {
            if (!_enabled || !IsKorean() ||
                __originalMethod == null ||
                __originalMethod.DeclaringType == null ||
                __result == null || __result.Length == 0)
                return;

            Catalog catalog;
            if (!AssemblyCatalogs.TryGetValue(
                    __originalMethod.DeclaringType.Assembly, out catalog))
                return;

            for (int i = 0; i < __result.Length; i++)
            {
                string original = __result[i];
                if (string.IsNullOrEmpty(original))
                    continue;

                string translated;
                if (TryTranslateSource(catalog, original, out translated))
                    __result[i] = translated;
            }
        }

        private static void SafeStringResultPostfix(
            MethodBase __originalMethod, ref string __result)
        {
            LogPatchHit(__originalMethod);

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
            if (TryTranslateTooltipSlice(catalog, __result, out translated))
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

        private static void LogPatchHit(MethodBase method)
        {
            if (method == null || method.DeclaringType == null)
                return;

            string key = method.DeclaringType.FullName + "." + method.Name;
            if (!DiagnosticOnce.Add(key))
                return;

            string locale = "<unknown>";
            try { locale = LocHelper.GetCurrentLocaleCode() ?? "<null>"; }
            catch { }

            Debug.Log("[KoreanPack] Patch hit: " + key +
                " locale='" + locale + "' korean=" + IsKoreanCode(locale));
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
                       StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(code, "kr",
                       StringComparison.OrdinalIgnoreCase) ||
                   code.StartsWith("kr-",
                       StringComparison.OrdinalIgnoreCase) ||
                   code.StartsWith("kr_",
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
