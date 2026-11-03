// Probably Stolen - generic Korean localization bridge, v0.5.41 build candidate.
//
// Contract:
//   * All translations live under THIS mod's Localization/<workshop-or-mod-id>/ko.csv.
//   * No changes to third-party files or third-party assemblies.
//   * The game-native LoadedMod.Localization dictionary receives the pack's entries.
//   * Mod-independent registration adapters translate display metadata at assignment time.
//   * No fixed list of target mods, no verified-methods.csv, no global UI-render interception,
//     no IL ldstr rewrite, and no patching of the mod's own DLL on disk.
//
// The bridge is designed for the actual ModLoader/LoadedMod/ModHelper/GameItem
// signatures in the user-provided Probably Stolen Managed assemblies.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HarmonyLib;
using UnityEngine.Localization.Settings;

namespace ProbablyStolenKoreanPack
{
    public sealed class KoreanPack : IMod
    {
        private const string OwnModId = "probably_stolen_korean_pack";
        private const string PatchOwner = "probably_stolen_korean_pack.generic_bridge";
        private static readonly object Sync = new object();

        private sealed class Catalog
        {
            internal string DirectoryId;
            internal string DeclaredModId;
            internal string ResolvedModId;
            internal readonly Dictionary<string, string> ByKey = new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly Dictionary<string, string> BySource = new Dictionary<string, string>(StringComparer.Ordinal);
            internal readonly HashSet<string> AmbiguousSource = new HashSet<string>(StringComparer.Ordinal);
            internal int RowCount;
        }

        private sealed class SavedKey
        {
            internal string LocaleCode;
            internal string Key;
            internal bool Existed;
            internal string Value;
            internal SavedKey(string localeCode, string key, bool existed, string value)
            { LocaleCode = localeCode; Key = key; Existed = existed; Value = value; }
        }

        private static readonly Dictionary<string, Catalog> ByDeclaredId =
            new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Catalog> ByDirectoryId =
            new Dictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);
        // Includes installed mods whose manifest ID differs from their workshop
        // number or on-disk folder name. Populated by the actual game loader.
        private static readonly ConcurrentDictionary<string, Catalog> ByRuntimeId =
            new ConcurrentDictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, Catalog> ByModRoot =
            new ConcurrentDictionary<string, Catalog>(StringComparer.OrdinalIgnoreCase);
        // AssemblyLoad can run on non-main threads while model/UI hooks read this map.
        private static readonly ConcurrentDictionary<Assembly, Catalog> ByAssembly =
            new ConcurrentDictionary<Assembly, Catalog>();
        private static readonly Dictionary<LoadedMod, List<SavedKey>> OriginalKeys =
            new Dictionary<LoadedMod, List<SavedKey>>();
        // Removing just our keys is not enough: an absent locale dictionary
        // created by this pack must be removed again when the pack disables.
        private static readonly Dictionary<LoadedMod, HashSet<string>> CreatedLocales =
            new Dictionary<LoadedMod, HashSet<string>>();
        // Keep each Harmony patch KIND separately. A setter can need both a
        // parameter prefix and an IL transpiler; a single patched-method flag
        // must not suppress the second adapter.
        private const int PrefixPatch = 1;
        private const int PostfixPatch = 2;
        private const int TranspilerPatch = 4;
        private const int FinalizerPatch = 8;
        private static readonly Dictionary<string, int> HookedMethods =
            new Dictionary<string, int>(StringComparer.Ordinal);

        // ModLoader loads an assembly and calls OnEnable before publishing the
        // LoadedMod. These thread-local hints bridge that early initialization.
        [ThreadStatic] private static Catalog PendingCatalog;
        [ThreadStatic] private static string PendingFolder;
        [ThreadStatic] private static string PendingRuntimeId;
        [ThreadStatic] private static Catalog PendingPreviousRuntimeCatalog;
        [ThreadStatic] private static bool PendingHadPreviousRuntime;
        [ThreadStatic] private static string PendingRootKey;
        [ThreadStatic] private static Catalog PendingPreviousRootCatalog;
        [ThreadStatic] private static bool PendingHadPreviousRoot;
        // A mod's OnEnable can itself trigger another mod load. Preserve the
        // outer owner while the nested load is active, including on exceptions.
        private sealed class PendingLoadSnapshot
        {
            internal Catalog Catalog;
            internal string Folder;
            internal string RuntimeId;
            internal Catalog PreviousRuntimeCatalog;
            internal bool HadPreviousRuntime;
            internal string RootKey;
            internal Catalog PreviousRootCatalog;
            internal bool HadPreviousRoot;
        }
        [ThreadStatic] private static Stack<PendingLoadSnapshot> PendingLoadStack;

        private static Harmony _harmony;
        private static ModLog _log;
        private static bool _enabled;
        private static int _hookCount;
        private static bool _commonHooksInstalled;
        private static int _modWriteMethods;
        private static readonly OpCode[] OneByteOpCodes = new OpCode[256];
        private static readonly OpCode[] TwoByteOpCodes = new OpCode[256];
        private static readonly MethodInfo TranslateWriteValueMethod =
            AccessTools.Method(typeof(KoreanPack), nameof(TranslateWriteValue));
        private static readonly MethodInfo TranslateWriteArrayMethod =
            AccessTools.Method(typeof(KoreanPack), nameof(TranslateWriteArray));

        static KoreanPack()
        {
            foreach (FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType != typeof(OpCode)) continue;
                OpCode opcode = (OpCode)field.GetValue(null);
                ushort code = unchecked((ushort)opcode.Value);
                if ((code & 0xff00) == 0xfe00) TwoByteOpCodes[code & 0xff] = opcode;
                else if ((code & 0xff00) == 0) OneByteOpCodes[code] = opcode;
            }
        }

        public void Init(ModManifest manifest) { _log = new ModLog(manifest); }

        public void OnEnable()
        {
            lock (Sync)
            {
                if (_enabled) return;
                string ownFolder = Path.GetDirectoryName(typeof(KoreanPack).Assembly.Location);
                if (string.IsNullOrEmpty(ownFolder)) throw new DirectoryNotFoundException("KoreanPack assembly directory");
                ByRuntimeId.Clear();
                ByModRoot.Clear();
                LoadCatalogs(Path.Combine(ownFolder, "Localization"));
                _harmony = new Harmony(PatchOwner);
                _enabled = true;
                try
                {
                    InstallCommonHooks();
                    AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
                    ModHook.OnGameLoadedNormal += OnGameLoaded;
                    RefreshLoadedMods();
                    Log("Generic localization enabled: " + ByDirectoryId.Count +
                        " folders; hooks: " + _hookCount +
                        "; localized mod write methods: " + _modWriteMethods);
                }
                catch
                {
                    _enabled = false;
                    AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
                    ModHook.OnGameLoadedNormal -= OnGameLoaded;
                    // If startup failed after merging native dictionaries, roll
                    // back our entries before discarding ownership mappings.
                    RestoreNativeEntries();
                    UnpatchOwned();
                    HookedMethods.Clear();
                    ByAssembly.Clear();
                    ByRuntimeId.Clear();
                    ByModRoot.Clear();
                    _hookCount = 0;
                    _modWriteMethods = 0;
                    _commonHooksInstalled = false;
                    throw;
                }
            }
        }

        public void OnDisable()
        {
            lock (Sync)
            {
                if (!_enabled) return;
                _enabled = false;
                AppDomain.CurrentDomain.AssemblyLoad -= OnAssemblyLoad;
                ModHook.OnGameLoadedNormal -= OnGameLoaded;
                RestoreNativeEntries();
                UnpatchOwned();
                HookedMethods.Clear();
                ByAssembly.Clear();
                ByRuntimeId.Clear();
                ByModRoot.Clear();
                _hookCount = 0;
                _modWriteMethods = 0;
                _commonHooksInstalled = false;
            }
        }

        // The actual game first enables mods and later calls LoadLocalization for each one.
        // Re-apply AFTER the game's loader so original CSVs cannot overwrite our ko entries.
        private static void InstallCommonHooks()
        {
            if (_commonHooksInstalled) return;
            MethodInfo localeLoader = AccessTools.Method(typeof(ModLoader), "LoadLocalization");
            if (localeLoader == null) throw new MissingMethodException("ModLoader.LoadLocalization");
            RequirePatch(localeLoader, null, nameof(AfterGameLocalization),
                "ModLoader.LoadLocalization");
            // The game's native CSV loader runs AFTER each mod's OnEnable.
            // Resolve keys from this pack even during an early GetLocalized call,
            // while still merging the same entries into LoadedMod.Localization.
            int nativeLookupHooks = 0;
            foreach (MethodInfo nativeLookup in typeof(ModHelper).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (nativeLookup.Name != "GetLocalized" ||
                    nativeLookup.ReturnType != typeof(string)) continue;
                ParameterInfo[] args = nativeLookup.GetParameters();
                if (args.Length != 3 || args[0].ParameterType != typeof(string) ||
                    args[1].ParameterType != typeof(string) ||
                    args[2].ParameterType != typeof(object[])) continue;
                if (Patch(nativeLookup, null, nameof(NativeLookupPostfix)))
                    nativeLookupHooks++;
            }
            if (nativeLookupHooks == 0)
                throw new InvalidOperationException("Missing required ModHelper.GetLocalized bridge");

            MethodInfo modDllLoader = AccessTools.Method(typeof(ModLoader), "LoadModDll");
            // A postfix never runs if LoadModDll throws. Clear pending
            // ownership in a finalizer on BOTH successful and failed loads.
            RequirePatch(modDllLoader, nameof(BeforeModDllLoad), null,
                "ModLoader.LoadModDll", null, nameof(AfterModDllLoad));

            MethodInfo assetLoader = AccessTools.Method(typeof(ModLoader), "LoadModAssets");
            if (assetLoader != null) Patch(assetLoader, null, nameof(AfterModAssets));

            // Shared game model metadata setters, not UI renderers.
            foreach (string setterName in new [] {
                "set_name", "set_shortDescription", "set_longDescription",
                "set_flavorText", "set_customText"
            })
            {
                MethodInfo setter = AccessTools.Method(typeof(GameItem), setterName);
                if (setter != null) Patch(setter, nameof(GameItemMetadataPrefix), null);
            }

            // The game's public registration API: use semantic *display* parameters only.
            foreach (MethodInfo method in typeof(ModHelper).GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                if (!IsRegistrationMethod(method)) continue;
                if (!method.GetParameters().Any(p => p.ParameterType == typeof(string) && IsDisplayParameter(p.Name))) continue;
                Patch(method, nameof(GameRegistrationPrefix), null);
            }
            // Data-resource adaptation is performed when the resource is read, never
            // when a rendered UI label is updated.
            MethodInfo embedded = typeof(Assembly).GetMethod("GetManifestResourceStream",
                new[] { typeof(string) });
            if (embedded != null) Patch(embedded, null, nameof(EmbeddedJsonPostfix));
            foreach (MethodInfo reader in typeof(File).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                ParameterInfo[] parameters = reader.GetParameters();
                if (reader.Name == "ReadAllText" && reader.ReturnType == typeof(string) &&
                    parameters.Length >= 1 && parameters[0].ParameterType == typeof(string))
                    Patch(reader, null, nameof(FileJsonPostfix));
                else if (reader.Name == "ReadAllBytes" && reader.ReturnType == typeof(byte[]) &&
                    parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
                    Patch(reader, null, nameof(FileJsonBytesPostfix));
            }
            foreach (MethodInfo reader in typeof(ModHelper).GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                if (reader.Name == "ReadModFileText" && reader.ReturnType == typeof(string))
                    Patch(reader, null, nameof(ModFileJsonPostfix));
                else if (reader.Name == "ReadModFileBytes" && reader.ReturnType == typeof(byte[]))
                    Patch(reader, null, nameof(ModFileJsonBytesPostfix));
            }
            _commonHooksInstalled = true;
        }

        private static bool IsRegistrationMethod(MethodInfo method)
        {
            if (method == null || method.IsAbstract || method.ContainsGenericParameters) return false;
            string name = method.Name;
            return name.StartsWith("Add", StringComparison.Ordinal)
                || name.StartsWith("Register", StringComparison.Ordinal)
                || name.StartsWith("Create", StringComparison.Ordinal)
                || name.StartsWith("Modify", StringComparison.Ordinal)
                || name.StartsWith("Set", StringComparison.Ordinal)
                || name.StartsWith("Update", StringComparison.Ordinal);
        }

        private static bool IsDisplayParameter(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string lower = name.ToLowerInvariant();
            return lower == "name" || lower == "displayname" || lower == "label"
                || lower == "title" || lower == "subtitle" || lower == "subtitlelabel"
                || lower == "description" || lower == "tooltip" || lower == "hint"
                || lower == "notes" || lower == "valuetext" || lower == "text"
                || lower == "displaytext" || lower == "shortdescription"
                || lower == "longdescription" || lower == "flavortext";
        }

        // The native localization lookup and mod-loading lifecycle are required
        // infrastructure. If one fails to attach, do not claim the pack is
        // enabled: OnEnable rolls back all patches and merged dictionary entries.
        private static void RequirePatch(MethodBase method, string prefixName,
            string postfixName, string description, string transpilerName = null,
            string finalizerName = null)
        {
            if (method == null || !Patch(method, prefixName, postfixName,
                    transpilerName, finalizerName))
                throw new InvalidOperationException("Required localization hook failed: " + description);
        }

        private static bool Patch(MethodBase method, string prefixName, string postfixName,
            string transpilerName = null, string finalizerName = null)
        {
            if (method == null) return false;
            string identity = method.Module.ModuleVersionId + ":" + method.MetadataToken;
            int previousMask;
            if (!HookedMethods.TryGetValue(identity, out previousMask)) previousMask = 0;
            int requestedMask = (prefixName == null ? 0 : PrefixPatch) |
                (postfixName == null ? 0 : PostfixPatch) |
                (transpilerName == null ? 0 : TranspilerPatch) |
                (finalizerName == null ? 0 : FinalizerPatch);
            int newMask = requestedMask & ~previousMask;
            if (newMask == 0) return false;
            try
            {
                HarmonyMethod prefix = (newMask & PrefixPatch) == 0 ? null :
                    new HarmonyMethod(typeof(KoreanPack), prefixName);
                HarmonyMethod postfix = (newMask & PostfixPatch) == 0 ? null :
                    new HarmonyMethod(typeof(KoreanPack), postfixName);
                HarmonyMethod transpiler = (newMask & TranspilerPatch) == 0 ? null :
                    new HarmonyMethod(typeof(KoreanPack), transpilerName);
                HarmonyMethod finalizer = (newMask & FinalizerPatch) == 0 ? null :
                    new HarmonyMethod(typeof(KoreanPack), finalizerName);
                _harmony.Patch(method, prefix: prefix, postfix: postfix,
                    transpiler: transpiler, finalizer: finalizer);
                HookedMethods[identity] = previousMask | newMask;
                if (previousMask == 0) _hookCount++;
                return true;
            }
            catch (Exception ex)
            {
                Error("Unable to hook " + (method.DeclaringType == null ?
                    method.Name : method.DeclaringType.FullName + "." + method.Name) + ": " + ex.Message);
                return false;
            }
        }

        private static void UnpatchOwned()
        {
            if (_harmony == null) return;
            MethodInfo method = typeof(Harmony).GetMethod("UnpatchAll",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance,
                null, new [] { typeof(string) }, null);
            if (method != null) method.Invoke(method.IsStatic ? null : (object)_harmony,
                new object[] { PatchOwner });
        }

        private static Catalog FindCatalogByModId(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            Catalog catalog;
            if (ByRuntimeId.TryGetValue(id, out catalog)) return catalog;
            if (ByDeclaredId.TryGetValue(id, out catalog)) return catalog;
            return ByDirectoryId.TryGetValue(id, out catalog) ? catalog : null;
        }

        private static string NormalizedDirectory(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder)) return null;
            try
            {
                return Path.GetFullPath(folder).TrimEnd(Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);
            }
            catch { return null; }
        }

        private static void RegisterModRoot(string folder, Catalog catalog)
        {
            string normalized = NormalizedDirectory(folder);
            if (normalized != null && catalog != null) ByModRoot[normalized] = catalog;
        }

        private static void ClearPendingModLoad()
        {
            PendingCatalog = null;
            PendingFolder = null;
            PendingRuntimeId = null;
            PendingPreviousRuntimeCatalog = null;
            PendingHadPreviousRuntime = false;
            PendingRootKey = null;
            PendingPreviousRootCatalog = null;
            PendingHadPreviousRoot = false;
        }

        private static void EnterModLoadContext()
        {
            if (PendingLoadStack == null) PendingLoadStack = new Stack<PendingLoadSnapshot>();
            PendingLoadStack.Push(new PendingLoadSnapshot {
                Catalog = PendingCatalog,
                Folder = PendingFolder,
                RuntimeId = PendingRuntimeId,
                PreviousRuntimeCatalog = PendingPreviousRuntimeCatalog,
                HadPreviousRuntime = PendingHadPreviousRuntime,
                RootKey = PendingRootKey,
                PreviousRootCatalog = PendingPreviousRootCatalog,
                HadPreviousRoot = PendingHadPreviousRoot
            });
            ClearPendingModLoad();
        }

        private static void ExitModLoadContext()
        {
            ClearPendingModLoad();
            if (PendingLoadStack == null || PendingLoadStack.Count == 0) return;
            PendingLoadSnapshot saved = PendingLoadStack.Pop();
            PendingCatalog = saved.Catalog;
            PendingFolder = saved.Folder;
            PendingRuntimeId = saved.RuntimeId;
            PendingPreviousRuntimeCatalog = saved.PreviousRuntimeCatalog;
            PendingHadPreviousRuntime = saved.HadPreviousRuntime;
            PendingRootKey = saved.RootKey;
            PendingPreviousRootCatalog = saved.PreviousRootCatalog;
            PendingHadPreviousRoot = saved.HadPreviousRoot;
        }

        private static void RollbackPendingModMappings()
        {
            Catalog mapped;
            if (PendingCatalog != null && PendingRuntimeId != null &&
                ByRuntimeId.TryGetValue(PendingRuntimeId, out mapped) &&
                object.ReferenceEquals(mapped, PendingCatalog))
            {
                if (PendingHadPreviousRuntime)
                    ByRuntimeId[PendingRuntimeId] = PendingPreviousRuntimeCatalog;
                else
                    ByRuntimeId.TryRemove(PendingRuntimeId, out mapped);
            }
            if (PendingCatalog != null && PendingRootKey != null &&
                ByModRoot.TryGetValue(PendingRootKey, out mapped) &&
                object.ReferenceEquals(mapped, PendingCatalog))
            {
                if (PendingHadPreviousRoot)
                    ByModRoot[PendingRootKey] = PendingPreviousRootCatalog;
                else
                    ByModRoot.TryRemove(PendingRootKey, out mapped);
            }
        }

        // Arguments verified against ModLoader.LoadModDll in Assembly-CSharp:
        // dllPath, modFolder, manifest, source, workshopId.
        private static void BeforeModDllLoad(object[] __args)
        {
            EnterModLoadContext();
            if (!_enabled || __args == null || __args.Length < 5) return;
            string folder = __args[1] as string;
            ModManifest manifest = __args[2] as ModManifest;
            Catalog catalog = manifest == null ? null : FindCatalogByModId(manifest.ID);
            if (catalog == null && __args[4] != null)
                ByDirectoryId.TryGetValue(__args[4].ToString(), out catalog);
            if (catalog == null && !string.IsNullOrEmpty(folder))
                ByDirectoryId.TryGetValue(Path.GetFileName(folder.TrimEnd(
                    Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), out catalog);
            if (catalog == null || string.IsNullOrEmpty(folder)) return;
            PendingCatalog = catalog;
            try
            {
                PendingRootKey = NormalizedDirectory(folder);
                if (PendingRootKey == null) { ClearPendingModLoad(); return; }
                PendingFolder = PendingRootKey + Path.DirectorySeparatorChar;
                PendingHadPreviousRoot = ByModRoot.TryGetValue(PendingRootKey,
                    out PendingPreviousRootCatalog);
                ByModRoot[PendingRootKey] = catalog;
                if (manifest != null && !string.IsNullOrEmpty(manifest.ID))
                {
                    PendingRuntimeId = manifest.ID;
                    PendingHadPreviousRuntime = ByRuntimeId.TryGetValue(PendingRuntimeId,
                        out PendingPreviousRuntimeCatalog);
                    ByRuntimeId[PendingRuntimeId] = catalog;
                }
            }
            catch (Exception ex)
            {
                RollbackPendingModMappings();
                ClearPendingModLoad();
                Error("Could not set up mod load context: " + ex.Message);
            }
        }

        private static Exception AfterModDllLoad(Exception __exception)
        {
            // Failing assemblies can remain in the AppDomain; do not leave the
            // failed mod's workshop/manifest alias active for subsequent loads.
            if (__exception != null) RollbackPendingModMappings();
            ExitModLoadContext();
            // Propagate the game's original exception, never swallow it.
            return __exception;
        }

        private static Catalog CatalogDuringModLoad(Assembly assembly)
        {
            if (assembly == null || PendingCatalog == null ||
                string.IsNullOrEmpty(PendingFolder)) return null;
            string location;
            try { location = assembly.Location; }
            catch { return null; }
            if (string.IsNullOrEmpty(location)) return null;
            try
            {
                return Path.GetFullPath(location).StartsWith(PendingFolder,
                    StringComparison.OrdinalIgnoreCase) ? PendingCatalog : null;
            }
            catch { return null; }
        }

        private static void NativeLookupPostfix(string __0, string __1,
            object[] __2, ref string __result)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0) ||
                string.IsNullOrEmpty(__1)) return;
            Catalog catalog = FindCatalogByModId(__0);
            if (catalog == null) return;
            string korean;
            if (!catalog.ByKey.TryGetValue(__1, out korean)) return;
            // GetLocalized supports params object[] for formatted texts. Match
            // its formatting contract without changing unrelated native lookups.
            try
            {
                __result = (__2 != null && __2.Length > 0)
                    ? string.Format(korean, __2) : korean;
            }
            catch (FormatException)
            {
                __result = korean;
            }
        }

        private static void AfterGameLocalization(object[] __args)
        {
            if (!_enabled) return;
            LoadedMod loaded = (__args != null && __args.Length > 0) ? __args[0] as LoadedMod : null;
            if (loaded != null) MergeNativeEntries(loaded, true);
        }

        private static void AfterModAssets() { RefreshLoadedMods(); }
        private static void OnGameLoaded() { RefreshLoadedMods(); }

        private static void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!_enabled || args == null || args.LoadedAssembly == null) return;
            try
            {
                // The game invokes IMod.OnEnable BEFORE adding the new mod to ModLoader.Mods.
                // Map by the loaded DLL's containing Workshop folder immediately, so the
                // generic data setters also work during the target mod's first OnEnable.
                Catalog early = FindCatalogByAssemblyLocation(args.LoadedAssembly) ??
                    CatalogDuringModLoad(args.LoadedAssembly);
                if (early != null)
                {
                    lock (Sync)
                    {
                        ByAssembly[args.LoadedAssembly] = early;
                        PatchModOwnedModelSetters(args.LoadedAssembly);
                        PatchModOwnedWriteSites(args.LoadedAssembly);
                    }
                }
                // The periodic lifecycle refresh resolves declared IDs and other
                // assemblies after the loader registers the LoadedMod.
            }
            catch (Exception ex) { Error("Assembly-load bridge: " + ex.Message); }
        }

        private static void RefreshLoadedMods()
        {
            if (!_enabled || ModLoader.Mods == null) return;
            lock (Sync)
            {
                foreach (LoadedMod loaded in ModLoader.Mods)
                {
                    if (loaded == null || loaded.Manifest == null ||
                        string.Equals(loaded.Manifest.ID, OwnModId, StringComparison.OrdinalIgnoreCase)) continue;
                    Catalog catalog = LocateCatalog(loaded);
                    if (catalog == null) continue;
                    catalog.ResolvedModId = loaded.Manifest.ID;
                    ByRuntimeId[loaded.Manifest.ID] = catalog;
                    RegisterModRoot(loaded.FolderPath, catalog);
                    MergeNativeEntries(loaded);
                    if (loaded.Assembly != null)
                    {
                        ByAssembly[loaded.Assembly] = catalog;
                        PatchModOwnedModelSetters(loaded.Assembly);
                        PatchModOwnedWriteSites(loaded.Assembly);
                    }
                }
                // Secondary assemblies are resolved by file location when available.
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (ByAssembly.ContainsKey(assembly)) continue;
                    Catalog catalog = FindCatalogByAssemblyLocation(assembly);
                    if (catalog == null) continue;
                    ByAssembly[assembly] = catalog;
                    PatchModOwnedModelSetters(assembly);
                    PatchModOwnedWriteSites(assembly);
                }
            }
        }

        private static Catalog LocateCatalog(LoadedMod loaded)
        {
            if (loaded == null || loaded.Manifest == null) return null;
            Catalog found = FindCatalogByModId(loaded.Manifest.ID);
            if (found != null) return found;
            // A new mod can be supported with only Localization/<WorkshopId>/ko.csv:
            // the CSV need not contain any hard-coded game assembly name.
            if (ByDirectoryId.TryGetValue(loaded.WorkshopId.ToString(), out found)) return found;
            if (!string.IsNullOrEmpty(loaded.FolderPath) &&
                ByDirectoryId.TryGetValue(Path.GetFileName(loaded.FolderPath.TrimEnd(
                    Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), out found)) return found;
            return null;
        }

        private static Catalog FindCatalogByAssemblyLocation(Assembly assembly)
        {
            if (assembly == null) return null;
            string location;
            try { location = assembly.Location; } catch { return null; }
            if (string.IsNullOrEmpty(location)) return null;

            // Works even before the game adds the LoadedMod to ModLoader.Mods.
            // The game's loader uses Assembly.LoadFrom(path) for each mod DLL.
            string current = Path.GetDirectoryName(location);
            for (int depth = 0; depth < 5 && !string.IsNullOrEmpty(current); depth++)
            {
                Catalog direct;
                if (ByDirectoryId.TryGetValue(Path.GetFileName(current), out direct)) return direct;
                string parent = Path.GetDirectoryName(current);
                if (string.Equals(parent, current, StringComparison.OrdinalIgnoreCase)) break;
                current = parent;
            }

            if (ModLoader.Mods != null)
            {
                foreach (LoadedMod loaded in ModLoader.Mods)
                {
                    if (loaded == null || loaded.Manifest == null || string.IsNullOrEmpty(loaded.FolderPath)) continue;
                    string folder;
                    try { folder = Path.GetFullPath(loaded.FolderPath).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar; }
                    catch { continue; }
                    if (!Path.GetFullPath(location).StartsWith(folder, StringComparison.OrdinalIgnoreCase)) continue;
                    return LocateCatalog(loaded);
                }
            }
            return null;
        }

        private static List<string> KoreanLocaleCodes()
        {
            // Register both the neutral and common Korean locale identifier.
            // The selected locale may be null until after mod initialization.
            var locales = new List<string> { "ko", "ko-KR" };
            try
            {
                var selected = LocalizationSettings.SelectedLocale;
                string code = selected == null ? null : selected.Identifier.Code;
                if (!string.IsNullOrEmpty(code) &&
                    code.StartsWith("ko-", StringComparison.OrdinalIgnoreCase) &&
                    !locales.Any(x => string.Equals(x, code, StringComparison.OrdinalIgnoreCase)))
                    locales.Add(code);
            }
            catch { /* The native ko dictionary remains available. */ }
            return locales;
        }

        private static void MergeNativeEntries(LoadedMod loaded, bool afterNativeLoad = false)
        {
            if (!_enabled || loaded == null || loaded.Manifest == null || loaded.Localization == null) return;
            Catalog catalog = LocateCatalog(loaded);
            if (catalog == null) return;
            lock (Sync)
            {
                List<SavedKey> saved;
                if (!OriginalKeys.TryGetValue(loaded, out saved))
                {
                    saved = new List<SavedKey>();
                    OriginalKeys[loaded] = saved;
                }
                foreach (string localeCode in KoreanLocaleCodes())
                {
                    Dictionary<string, string> target;
                    bool localeExisted = loaded.Localization.TryGetValue(localeCode, out target);
                    if (!localeExisted || target == null)
                    {
                        if (!localeExisted)
                        {
                            HashSet<string> created;
                            if (!CreatedLocales.TryGetValue(loaded, out created))
                            {
                                created = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                CreatedLocales[loaded] = created;
                            }
                            created.Add(localeCode);
                        }
                        target = new Dictionary<string, string>(StringComparer.Ordinal);
                        loaded.Localization[localeCode] = target;
                    }
                    var originals = new Dictionary<string, SavedKey>(StringComparer.Ordinal);
                    foreach (SavedKey previous in saved)
                        if (string.Equals(previous.LocaleCode, localeCode, StringComparison.OrdinalIgnoreCase))
                            originals[previous.Key] = previous;
                    foreach (KeyValuePair<string, string> kv in catalog.ByKey)
                    {
                        string previous;
                        bool existed = target.TryGetValue(kv.Key, out previous);
                        SavedKey original;
                        if (!originals.TryGetValue(kv.Key, out original))
                        {
                            original = new SavedKey(localeCode, kv.Key, existed, previous);
                            originals[kv.Key] = original;
                            saved.Add(original);
                        }
                        else if (afterNativeLoad && (!existed || previous != kv.Value))
                        {
                            // Native LoadLocalization runs after OnEnable; if it
                            // replaced the dictionary, remember its latest values.
                            original.Existed = existed;
                            original.Value = previous;
                        }
                        target[kv.Key] = kv.Value;
                    }
                }
            }
        }

        private static void RestoreNativeEntries()
        {
            foreach (KeyValuePair<LoadedMod, List<SavedKey>> pair in OriginalKeys)
            {
                try
                {
                    LoadedMod mod = pair.Key;
                    if (mod == null || mod.Localization == null) continue;
                    Catalog catalog = LocateCatalog(mod);
                    if (catalog == null) continue;
                    foreach (SavedKey old in pair.Value)
                    {
                        Dictionary<string, string> target;
                        if (!mod.Localization.TryGetValue(old.LocaleCode, out target) || target == null) continue;
                        string existing, ours;
                        if (!catalog.ByKey.TryGetValue(old.Key, out ours) ||
                            !target.TryGetValue(old.Key, out existing) || existing != ours) continue;
                        if (old.Existed) target[old.Key] = old.Value;
                        else target.Remove(old.Key);
                    }
                    HashSet<string> created;
                    if (CreatedLocales.TryGetValue(mod, out created))
                    {
                        foreach (string localeCode in created)
                        {
                            Dictionary<string, string> current;
                            if (mod.Localization.TryGetValue(localeCode, out current) &&
                                current != null && current.Count == 0)
                                mod.Localization.Remove(localeCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // One mod's destroyed localization state must not prevent
                    // cleanup of other mods or unpatching the bridge.
                    Error("Native localization rollback: " + ex.Message);
                }
            }
            OriginalKeys.Clear();
            CreatedLocales.Clear();
        }

        // Some mods register immutable display metadata before the game's locale is
        // initialized. When no locale has been chosen yet, allow this Korean pack
        // to translate initial registrations rather than silently losing them.
        // An explicitly selected non-Korean locale still disables adapters.
        private static bool IsKorean()
        {
            try
            {
                var selected = LocalizationSettings.SelectedLocale;
                if (selected == null) return true;
                string code = selected.Identifier.Code;
                return string.IsNullOrEmpty(code) ||
                    string.Equals(code, "ko", StringComparison.OrdinalIgnoreCase) ||
                    code.StartsWith("ko-", StringComparison.OrdinalIgnoreCase);
            }
            catch { return true; }
        }

        // Common game-owned data model setter. Identify WHICH mod is creating the item
        // from its call stack; no per-mod factory or per-mod method list is required.
        private static void GameItemMetadataPrefix(ref string __0)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0)) return;
            Catalog catalog = GetCallerCatalog();
            if (catalog != null) __0 = TranslateAssignment(catalog, __0);
        }

        // Mod-owned property setters: automatically discovered in every loaded mod assembly.
        // Deliberately excludes renderer and UI/view types.
        private static void PatchModOwnedModelSetters(Assembly assembly)
        {
            if (assembly == null || !ByAssembly.ContainsKey(assembly)) return;
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types; }
            catch (Exception ex) { Error("Could not enumerate types in " + assembly.GetName().Name + ": " + ex.Message); return; }
            if (types == null) return;
            foreach (Type type in types)
            {
                if (type == null || type.IsInterface || IsViewType(type.Name)) continue;
                MethodInfo[] typeMethods;
                try
                {
                    typeMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                }
                catch (Exception ex)
                {
                    Error("Skipping inaccessible mod type " + type.FullName + ": " + ex.Message);
                    continue;
                }
                foreach (MethodInfo method in typeMethods)
                {
                    try
                    {
                    if (method.IsAbstract || method.ContainsGenericParameters) continue;
                    ParameterInfo[] args = method.GetParameters();
                    if (method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal) &&
                        args.Length == 1 && IsDisplayProperty(method.Name.Substring(4)))
                    {
                        if (args[0].ParameterType == typeof(string))
                        {
                            Patch(method, nameof(ModModelMetadataPrefix), null);
                            continue;
                        }
                        if (args[0].ParameterType == typeof(string[]))
                        {
                            Patch(method, nameof(ModModelArrayPrefix), null);
                            continue;
                        }
                    }
                    // Read-only names and descriptions may have been initialized
                    // before this pack. Translate their returned value at its
                    // owning method, not at a game-global UI renderer.
                    if (args.Length != 0 ||
                        !(IsSafeTextReturnMethod(method.Name) ||
                          IsVisibleStringMember(method.Name))) continue;
                    if (method.ReturnType == typeof(string))
                        Patch(method, null, nameof(ModOwnedTextResultPostfix));
                    else if (method.ReturnType == typeof(string[]) &&
                        (IsVisibleReturnMethod(method.Name) ||
                         IsVisibleStringMember(method.Name)))
                        Patch(method, null, nameof(ModOwnedTextArrayPostfix));
                    }
                    catch (Exception ex)
                    {
                        // A malformed reflection signature in one third-party
                        // method must not disable all the others in this mod.
                        Error("Skipping inaccessible mod accessor " + method.Name + ": " + ex.Message);
                    }
                }
            }
        }

        // Mod-owned IL bridge. Data-field stores, display calls, and format-string
        // composition use the SAME CSV lookup. A dynamic string's localization
        // format is substituted before its arguments are inserted by the game.
        // Unlike a global UI-renderer hook, only the mod's call sites are patched.
        // Neither original ldstr constants nor files on disk are rewritten.
        private static void PatchModOwnedWriteSites(Assembly assembly)
        {
            if (assembly == null || !ByAssembly.ContainsKey(assembly)) return;
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex) { types = ex.Types; }
            catch (Exception ex) { Error("Field-store type scan: " + ex.Message); return; }
            if (types == null) return;
            foreach (Type type in types)
            {
                // View types must be scanned too: a mod may call TMP_Text.set_text
                // directly inside its own UI class without storing any data field.
                if (type == null || type.IsInterface) continue;
                var methods = new List<MethodBase>();
                try
                {
                    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
                    methods.AddRange(type.GetMethods(flags));
                    methods.AddRange(type.GetConstructors(flags));
                    if (type.TypeInitializer != null && !methods.Contains(type.TypeInitializer))
                        methods.Add(type.TypeInitializer);
                }
                catch { continue; }
                foreach (MethodBase method in methods)
                {
                    try
                    {
                        if (method.IsAbstract || method.ContainsGenericParameters ||
                            method.GetMethodImplementationFlags().HasFlag(MethodImplAttributes.InternalCall)) continue;
                        if (!HasDisplayWriteSite(method)) continue;
                        if (Patch(method, null, null, nameof(ModWriteSiteTranspiler)))
                            _modWriteMethods++;
                    }
                    catch (Exception ex)
                    {
                        Error("Skipping malformed mod method " + method.Name + ": " + ex.Message);
                    }
                }
            }
        }

        private static bool IsDisplayField(FieldInfo field)
        {
            if (field == null || (field.FieldType != typeof(string) &&
                field.FieldType != typeof(string[]))) return false;
            // An arbitrary Unity/TMP UI text backing field is not a data-registration site.
            Type owner = field.DeclaringType;
            if (owner != null && (IsViewType(owner.Name) ||
                (owner.Namespace != null && (owner.Namespace.StartsWith("UnityEngine.UI", StringComparison.Ordinal)
                || owner.Namespace.StartsWith("TMPro", StringComparison.Ordinal))))) return false;
            string name = field.Name;
            if (name.Length >= 3 && name[0] == '<' && name.Contains(">"))
                name = name.Substring(1, name.IndexOf('>') - 1);
            if (name.StartsWith("m_", StringComparison.OrdinalIgnoreCase)) name = name.Substring(2);
            else if (name.StartsWith("_", StringComparison.Ordinal)) name = name.Substring(1);
            return IsDisplayProperty(name) || IsDisplayParameter(name);
        }

        private static bool HasDisplayWriteSite(MethodBase method)
        {
            MethodBody body;
            try { body = method.GetMethodBody(); }
            catch { return false; }
            if (body == null) return false;
            byte[] il = body.GetILAsByteArray();
            if (il == null) return false;
            int offset = 0;
            while (offset < il.Length)
            {
                OpCode op;
                byte code = il[offset++];
                if (code == 0xfe)
                {
                    if (offset >= il.Length) return false;
                    op = TwoByteOpCodes[il[offset++]];
                }
                else op = OneByteOpCodes[code];
                int size;
                switch (op.OperandType)
                {
                    case OperandType.InlineNone: size = 0; break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar: size = 1; break;
                    case OperandType.InlineVar: size = 2; break;
                    case OperandType.InlineBrTarget:
                    case OperandType.InlineField:
                    case OperandType.InlineI:
                    case OperandType.InlineMethod:
                    case OperandType.InlineSig:
                    case OperandType.InlineString:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                    case OperandType.ShortInlineR: size = 4; break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR: size = 8; break;
                    case OperandType.InlineSwitch:
                        if (offset + 4 > il.Length) return false;
                        int count = BitConverter.ToInt32(il, offset);
                        if (count < 0 || count > (il.Length - offset - 4) / 4) return false;
                        size = 4 + count * 4;
                        break;
                    default: return false;
                }
                if (offset + size > il.Length) return false;
                if ((op == OpCodes.Stfld || op == OpCodes.Stsfld) && size == 4)
                {
                    try
                    {
                        int token = BitConverter.ToInt32(il, offset);
                        FieldInfo field = method.Module.ResolveField(token,
                            method.DeclaringType != null && method.DeclaringType.IsGenericType ?
                                method.DeclaringType.GetGenericArguments() : null,
                            (method as MethodInfo) != null && ((MethodInfo)method).IsGenericMethod
                                ? ((MethodInfo)method).GetGenericArguments() : null);
                        if (IsDisplayField(field)) return true;
                    }
                    catch { /* Unresolvable generic/token: leave method alone. */ }
                }
                else if ((op == OpCodes.Call || op == OpCodes.Callvirt ||
                          op == OpCodes.Newobj) && size == 4)
                {
                    try
                    {
                        int token = BitConverter.ToInt32(il, offset);
                        MethodBase called = method.Module.ResolveMethod(token,
                            method.DeclaringType != null && method.DeclaringType.IsGenericType ?
                                method.DeclaringType.GetGenericArguments() : null,
                            (method as MethodInfo) != null && ((MethodInfo)method).IsGenericMethod
                                ? ((MethodInfo)method).GetGenericArguments() : null);
                        if (IsDisplayCallsite(called)) return true;
                    }
                    catch { /* Unresolvable call: leave method alone. */ }
                }
                offset += size;
            }
            return false;
        }

        // Only recognized display sinks are intercepted. Many Unity overloads put
        // their display string BEFORE GUIStyle, layout options, or format arguments;
        // requiring it to be the last parameter silently dropped those calls.
        // Never infer a display sink from an arbitrary method named Set/Write.
        private static int[] GetDisplayCallIndices(MethodBase method)
        {
            if (method == null || method.DeclaringType == null ||
                method.ContainsGenericParameters ||
                (method.CallingConvention & CallingConventions.VarArgs) != 0)
                return new int[0];
            ParameterInfo[] args;
            try { args = method.GetParameters(); }
            catch { return new int[0]; }
            if (args.Length == 0 || args.Length > 24) return new int[0];
            // By-ref/out values cannot safely be spilled to ordinary IL locals
            // in every callsite, so leave such calls unchanged.
            if (args.Any(a => a.ParameterType.IsByRef || a.ParameterType.IsPointer ||
                a.ParameterType.ContainsGenericParameters ||
                a.ParameterType == typeof(TypedReference) ||
                a.ParameterType.FullName == "System.ArgIterator" ||
                a.ParameterType == typeof(RuntimeArgumentHandle))) return new int[0];
            Type owner = method.DeclaringType;
            string ns = owner.Namespace ?? "";
            string type = owner.Name;
            string name = method.Name;
            var selected = new List<int>();

            if (ns.StartsWith("TMPro", StringComparison.Ordinal) ||
                ns.StartsWith("UnityEngine.UI", StringComparison.Ordinal) ||
                ns.StartsWith("UnityEngine.UIElements", StringComparison.Ordinal))
            {
                if ((name == "set_text" || name == "SetText") &&
                    args[0].ParameterType == typeof(string)) selected.Add(0);
            }
            else if (ns == "UnityEngine" && (type == "TextMesh" ||
                type == "GUIContent" || type == "GUI" || type == "GUILayout"))
            {
                if (type == "TextMesh" && name == "set_text" && args.Length == 1 &&
                    args[0].ParameterType == typeof(string)) selected.Add(0);
                else if (type == "GUIContent" && (name == ".ctor" || name == "Temp" ||
                                                   name == "set_text" || name == "set_tooltip"))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i].ParameterType != typeof(string)) continue;
                        if (name == "set_text" || name == "set_tooltip" ||
                            string.Equals(args[i].Name, "text", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(args[i].Name, "tooltip", StringComparison.OrdinalIgnoreCase))
                            selected.Add(i);
                    }
                }
                else if ((type == "GUI" || type == "GUILayout") &&
                    (name == "Label" || name == "Box" || name == "Button" ||
                     name == "Toggle" || name == "RepeatButton"))
                {
                    // Only the display text (not arbitrary string arguments).
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i].ParameterType != typeof(string)) continue;
                        if (string.Equals(args[i].Name, "text", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(args[i].Name, "label", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(args[i].Name, "content", StringComparison.OrdinalIgnoreCase))
                            selected.Add(i);
                    }
                }
            }
            else if (IsViewType(type) && (name == "SetText" || name == "set_text" ||
                                            name == "SetLabel" || name == "SetCaption") &&
                     args[0].ParameterType == typeof(string))
                selected.Add(0);
            else if (IsNamedDisplayDelivery(name) &&
                     !ns.StartsWith("System", StringComparison.Ordinal) &&
                     !ns.StartsWith("Microsoft", StringComparison.Ordinal))
            {
                // Third-party and game-specific presentation services often use
                // descriptive method/parameter names rather than Unity setters.
                // Apply only to semantically labelled string arguments, only at
                // the callsite of a loaded mod, with exact CSV lookup.
                for (int i = 0; i < args.Length; i++)
                    if (args[i].ParameterType == typeof(string) &&
                        IsDisplayParameter(args[i].Name) &&
                        !string.Equals(args[i].Name, "name", StringComparison.OrdinalIgnoreCase))
                        selected.Add(i);
            }

            return selected.ToArray();
        }

        private static bool IsNamedDisplayDelivery(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            string[] names = { "Show", "Display", "Notify", "Toast", "Popup",
                "PopUp", "Prompt", "Present", "SetMessage", "AddMessage",
                "SetLabel", "SetCaption", "SetTitle", "SetTooltip",
                "SetDescription", "SetText", "UpdateText", "UpdateLabel",
                "UpdateDescription", "ShowDialog", "AddNotification" };
            foreach (string candidate in names)
                if (name.StartsWith(candidate, StringComparison.Ordinal)) return true;
            return false;
        }

        // Dynamic text is localized at its COMPOSITION SITE. When a mod calls
        // string.Format, StringBuilder.AppendFormat, or FormattableStringFactory.Create,
        // replace only the format-string argument from the mod's CSV BEFORE the
        // game inserts runtime values. There is no regex matching of completed text,
        // independent template table, or global output hook.
        private static int[] GetLocalizationFormatIndices(MethodBase method)
        {
            if (method == null || method.DeclaringType == null ||
                method.ContainsGenericParameters ||
                (method.CallingConvention & CallingConventions.VarArgs) != 0)
                return new int[0];
            ParameterInfo[] args;
            try { args = method.GetParameters(); }
            catch { return new int[0]; }
            if (args.Length < 2 || args.Length > 24 ||
                args.Any(p => p.ParameterType.IsByRef || p.ParameterType.IsPointer ||
                    p.ParameterType.ContainsGenericParameters ||
                    p.ParameterType == typeof(TypedReference) ||
                    p.ParameterType.FullName == "System.ArgIterator" ||
                    p.ParameterType == typeof(RuntimeArgumentHandle)))
                return new int[0];

            Type owner = method.DeclaringType;
            string methodName = method.Name;
            bool isFormat = owner == typeof(string) && methodName == "Format";
            bool isAppendFormat = owner == typeof(StringBuilder) && methodName == "AppendFormat";
            bool isFormattable = owner.FullName ==
                "System.Runtime.CompilerServices.FormattableStringFactory" &&
                methodName == "Create";
            if (!isFormat && !isAppendFormat && !isFormattable) return new int[0];
            // All supported overloads have a format parameter either at 0 or
            // immediately after the IFormatProvider parameter. No other parameter
            // is translated; the caller's values and ordering remain unchanged.
            int formatIndex = args[0].ParameterType == typeof(string) ? 0 :
                args[0].ParameterType == typeof(IFormatProvider) &&
                args[1].ParameterType == typeof(string) ? 1 : -1;
            return formatIndex >= 0 && args.Length > formatIndex + 1
                ? new[] { formatIndex } : new int[0];
        }

        private static bool IsDisplayCallsite(MethodBase method)
        {
            return GetDisplayCallIndices(method).Length > 0 ||
                GetLocalizationFormatIndices(method).Length > 0;
        }

        private static bool IsILPrefix(OpCode op)
        {
            return op == OpCodes.Tailcall || op == OpCodes.Constrained ||
                   op == OpCodes.Volatile || op == OpCodes.Unaligned ||
                   op == OpCodes.Readonly || op.Name == "no.";
        }

        // Harmony re-emits method IL with the original receiver/arguments on the stack.
        // For stfld/stsfld the string is on top; for calls, a display string may be
        // anywhere in the parameter list. Spill only the explicit parameters to
        // temporary locals, reload them in order, and translate chosen string args.
        // The instance receiver (if any) remains untouched beneath those args.
        private static IEnumerable<CodeInstruction> ModWriteSiteTranspiler(
            IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
        {
            Catalog catalog = null;
            if (original != null) ByAssembly.TryGetValue(original.Module.Assembly, out catalog);
            if (catalog == null || TranslateWriteValueMethod == null || generator == null)
                return instructions;
            var source = new List<CodeInstruction>(instructions);
            var rewritten = new List<CodeInstruction>(source.Count);
            for (int index = 0; index < source.Count; index++)
            {
                CodeInstruction code = source[index];
                FieldInfo field = code.operand as FieldInfo;
                bool fieldWrite = (code.opcode == OpCodes.Stfld ||
                    code.opcode == OpCodes.Stsfld) && IsDisplayField(field);
                MethodBase called = code.operand as MethodBase;
                bool isCall = code.opcode == OpCodes.Call || code.opcode == OpCodes.Callvirt ||
                    code.opcode == OpCodes.Newobj;
                int[] displayArgs = isCall ? GetDisplayCallIndices(called) : new int[0];
                int[] formatArgs = isCall ? GetLocalizationFormatIndices(called) : new int[0];
                int[] localizedArgs = displayArgs.Concat(formatArgs).Distinct().ToArray();
                // Prefix opcodes must remain adjacent to the instruction they modify.
                bool prefixed = index > 0 && IsILPrefix(source[index - 1].opcode);
                // Harmony's exception-block markers define EH region BOUNDARIES.
                // Inserting spills at a begin/end/filter/catch/finally boundary can
                // change handler membership or make stack validation fail. Leave
                // that particular sink unchanged; all other sinks still translate.
                bool exceptionBoundary = code.blocks != null && code.blocks.Count != 0;
                if (!prefixed && !exceptionBoundary && fieldWrite)
                {
                    MethodInfo translator = field.FieldType == typeof(string[])
                        ? TranslateWriteArrayMethod : TranslateWriteValueMethod;
                    if (translator != null)
                    {
                        var modId = new CodeInstruction(OpCodes.Ldstr, catalog.DirectoryId);
                        TransferBranchLabels(code, modId);
                        rewritten.Add(modId);
                        rewritten.Add(new CodeInstruction(OpCodes.Call, translator));
                    }
                }
                else if (!prefixed && !exceptionBoundary && localizedArgs.Length > 0)
                {
                    ParameterInfo[] args = called.GetParameters();
                    var locals = new LocalBuilder[args.Length];
                    var inserted = new List<CodeInstruction>();
                    // Pop explicit parameters only; do not pop an instance receiver.
                    for (int i = args.Length - 1; i >= 0; i--)
                    {
                        locals[i] = generator.DeclareLocal(args[i].ParameterType);
                        inserted.Add(new CodeInstruction(OpCodes.Stloc, locals[i]));
                    }
                    for (int i = 0; i < args.Length; i++)
                    {
                        inserted.Add(new CodeInstruction(OpCodes.Ldloc, locals[i]));
                        if (Array.IndexOf(localizedArgs, i) >= 0)
                        {
                            inserted.Add(new CodeInstruction(OpCodes.Ldstr, catalog.DirectoryId));
                            inserted.Add(new CodeInstruction(OpCodes.Call, TranslateWriteValueMethod));
                        }
                    }
                    if (inserted.Count > 0)
                    {
                        TransferBranchLabels(code, inserted[0]);
                        rewritten.AddRange(inserted);
                    }
                }
                rewritten.Add(code);
            }
            return rewritten;
        }

        private static readonly FieldInfo HarmonyLabelsField =
            typeof(CodeInstruction).GetField("labels", BindingFlags.Public | BindingFlags.Instance);

        private static void TransferBranchLabels(CodeInstruction original, CodeInstruction first)
        {
            // The game's Harmony exposes List<Label> from its own runtime assembly.
            // Access that list through the non-generic IList interface to avoid
            // importing an incompatible mscorlib/System.Reflection.Emit.Label type
            // when compiling this netstandard2.1 bridge.
            if (HarmonyLabelsField == null)
                throw new MissingFieldException(typeof(CodeInstruction).FullName, "labels");
            var before = HarmonyLabelsField.GetValue(original) as System.Collections.IList;
            var after = HarmonyLabelsField.GetValue(first) as System.Collections.IList;
            if (before == null || after == null)
                throw new InvalidOperationException("Harmony CodeInstruction labels unavailable");
            // Preserve branch targets on the new first instruction, so jumping
            // into a patched callsite still executes the localization step.
            for (int i = 0; i < before.Count; i++) after.Add(before[i]);
            before.Clear();
        }

        private static string TranslateWriteValue(string value, string directoryId)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(value)) return value;
            Catalog catalog;
            return ByDirectoryId.TryGetValue(directoryId, out catalog)
                ? TranslateAssignment(catalog, value) : value;
        }

        private static string[] TranslateWriteArray(string[] value, string directoryId)
        {
            if (!_enabled || !IsKorean() || value == null) return value;
            Catalog catalog;
            return ByDirectoryId.TryGetValue(directoryId, out catalog)
                ? TranslateArrayAssignment(catalog, value) : value;
        }

        // Single point of translation for property setters, fields, game registration,
        // mod-owned UI text deliveries and decoded JSON display values.
        private static string TranslateAssignment(Catalog catalog, string value)
        {
            return catalog == null || string.IsNullOrEmpty(value) ? value : ReplaceSource(catalog, value);
        }

        // Preserve array identity unless at least one displayed entry actually changes.
        // Original source arrays and non-display identifiers are never mutated in place.
        private static string[] TranslateArrayAssignment(Catalog catalog, string[] values)
        {
            if (catalog == null || values == null) return values;
            string[] replacement = null;
            for (int i = 0; i < values.Length; i++)
            {
                string previous = values[i];
                if (string.IsNullOrEmpty(previous)) continue;
                string next = TranslateAssignment(catalog, previous);
                if (string.Equals(previous, next, StringComparison.Ordinal)) continue;
                if (replacement == null) replacement = (string[])values.Clone();
                replacement[i] = next;
            }
            return replacement ?? values;
        }

        private static bool IsViewType(string name)
        {
            return name.EndsWith("UI", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("View", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Window", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Panel", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Renderer", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Text", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Tooltip", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith("Dialog", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDisplayProperty(string name)
        {
            return IsVisibleStringMember(name);
        }

        private static bool IsVisibleStringMember(string rawName)
        {
            if (string.IsNullOrEmpty(rawName)) return false;
            string name = rawName.TrimStart('_').ToLowerInvariant().Replace("_", "").Replace("-", "");
            if (name.StartsWith("m", StringComparison.Ordinal) && name.Length > 1 &&
                rawName.StartsWith("m_", StringComparison.Ordinal))
                name = name.Substring(1);
            // A translated UI label must never silently become a resource identifier.
            if (name.EndsWith("id", StringComparison.Ordinal) ||
                name.EndsWith("key", StringComparison.Ordinal) ||
                name.EndsWith("path", StringComparison.Ordinal) ||
                name.EndsWith("identifier", StringComparison.Ordinal) ||
                name.Contains("filename") || name.Contains("filepath") ||
                name.Contains("resourcename") || name.Contains("assetname") ||
                name.Contains("prefabname") || name.Contains("classname") ||
                name.Contains("typename") || name.Contains("internalname") ||
                name.Contains("texturename") || name.Contains("spritename") ||
                name.Contains("shadername") || name.Contains("materialname") ||
                name.Contains("animationname") || name.Contains("audioname") ||
                name.Contains("soundname") || name.Contains("sourcename") ||
                name.Contains("targetname") || name.Contains("rootname") ||
                name.Contains("objectname") || name.Contains("tagname")) return false;
            string[] endings = {
                "name", "title", "subtitle", "description", "desc", "summary", "flavor",
                "label", "caption", "tooltip", "hint", "message", "text",
                "displaytext", "flavortext", "dialogue", "dialog", "story",
                "details", "content", "note", "notes", "names", "titles",
                "descriptions", "labels", "tooltips", "messages", "texts",
                "contents", "captions", "hints", "dialogues", "stories"
            };
            // Mod data tables often carry English/Chinese arrays side-by-side
            // (NamesZh, DescriptionsEn, LabelCn). Treat these as display members
            // only when the base name itself is display-labelled. Never infer
            // translatability from the language suffix alone.
            string baseName = name;
            foreach (string suffix in new [] { "chinese", "english", "zh", "cn", "en" })
            {
                if (!name.EndsWith(suffix, StringComparison.Ordinal) ||
                    name.Length <= suffix.Length) continue;
                baseName = name.Substring(0, name.Length - suffix.Length);
                break;
            }
            foreach (string end in endings)
                if (name.EndsWith(end, StringComparison.Ordinal) ||
                    baseName.EndsWith(end, StringComparison.Ordinal)) return true;
            return false;
        }

        private static bool IsSafeTextReturnMethod(string methodName)
        {
            if (string.IsNullOrEmpty(methodName)) return false;
            // Translation still requires an EXACT source in that mod's own CSV.
            // Read-only string getters with custom names (e.g. get_FrontDesk)
            // otherwise escape the setter/field and normal display adapters.
            if (!(methodName.StartsWith("get_", StringComparison.Ordinal) ||
                  methodName.StartsWith("Get", StringComparison.Ordinal) ||
                  methodName.StartsWith("Build", StringComparison.Ordinal) ||
                  methodName.StartsWith("Compose", StringComparison.Ordinal) ||
                  methodName.StartsWith("Format", StringComparison.Ordinal))) return false;
            string name = methodName.ToLowerInvariant();
            string[] internalNames = { "identifier", "id", "key", "path", "folder", "file",
                "resource", "asset", "prefab", "texture", "sprite", "shader", "material",
                "assembly", "namespace", "guid", "hash", "version", "token", "url",
                "schema", "type", "config", "source", "target", "internal" };
            foreach (string word in internalNames)
                if (name.Contains(word)) return false;
            return true;
        }

        private static bool IsVisibleReturnMethod(string methodName)
        {
            if (string.IsNullOrEmpty(methodName)) return false;
            string member = methodName;
            if (member.StartsWith("get_", StringComparison.Ordinal))
                member = member.Substring(4);
            else if (member.StartsWith("Get", StringComparison.Ordinal) ||
                     member.StartsWith("Build", StringComparison.Ordinal) ||
                     member.StartsWith("Compose", StringComparison.Ordinal) ||
                     member.StartsWith("Format", StringComparison.Ordinal))
                member = member.Substring(member.StartsWith("Compose", StringComparison.Ordinal) ? 7 :
                    member.StartsWith("Format", StringComparison.Ordinal) ? 6 :
                    member.StartsWith("Build", StringComparison.Ordinal) ? 5 : 3);
            else return false;
            return IsVisibleStringMember(member);
        }

        private static void ModOwnedTextResultPostfix(MethodBase __originalMethod, ref string __result)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__result) || __originalMethod == null) return;
            Catalog catalog;
            if (ByAssembly.TryGetValue(__originalMethod.Module.Assembly, out catalog))
                __result = TranslateAssignment(catalog, __result);
        }

        private static void ModOwnedTextArrayPostfix(MethodBase __originalMethod, ref string[] __result)
        {
            if (!_enabled || !IsKorean() || __result == null || __originalMethod == null) return;
            Catalog catalog;
            if (ByAssembly.TryGetValue(__originalMethod.Module.Assembly, out catalog))
                __result = TranslateArrayAssignment(catalog, __result);
        }

        private static void ModModelMetadataPrefix(MethodBase __originalMethod, ref string __0)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0) || __originalMethod == null) return;
            Catalog catalog;
            if (ByAssembly.TryGetValue(__originalMethod.Module.Assembly, out catalog))
                __0 = TranslateAssignment(catalog, __0);
        }

        private static void ModModelArrayPrefix(MethodBase __originalMethod, ref string[] __0)
        {
            if (!_enabled || !IsKorean() || __0 == null || __originalMethod == null) return;
            Catalog catalog;
            if (ByAssembly.TryGetValue(__originalMethod.Module.Assembly, out catalog))
                __0 = TranslateArrayAssignment(catalog, __0);
        }

        // ModHelper registration APIs label/title/description arguments, not generalized UI text.
        private static void GameRegistrationPrefix(MethodBase __originalMethod, object[] __args)
        {
            if (!_enabled || !IsKorean() || __originalMethod == null || __args == null) return;
            ParameterInfo[] parameters = __originalMethod.GetParameters();
            Catalog catalog = null;
            for (int i = 0; i < parameters.Length && i < __args.Length; i++)
            {
                if (string.Equals(parameters[i].Name, "modId", StringComparison.OrdinalIgnoreCase))
                {
                    string id = __args[i] as string;
                    if (id != null) catalog = FindCatalogByModId(id);
                    break;
                }
            }
            if (catalog == null) catalog = GetCallerCatalog();
            if (catalog == null) return;
            for (int i = 0; i < parameters.Length && i < __args.Length; i++)
            {
                if (parameters[i].ParameterType != typeof(string) || !IsDisplayParameter(parameters[i].Name)) continue;
                string original = __args[i] as string;
                if (!string.IsNullOrEmpty(original)) __args[i] = TranslateAssignment(catalog, original);
            }
        }

        private static Catalog GetCallerCatalog()
        {
            // Stack usage is confined to game data setters/registration calls, NOT output rendering.
            StackTrace trace = new StackTrace(2, false);
            for (int i = 0; i < trace.FrameCount; i++)
            {
                MethodBase method = trace.GetFrame(i).GetMethod();
                if (method == null || method.DeclaringType == null) continue;
                Assembly assembly = method.DeclaringType.Assembly;
                Catalog catalog;
                if (ByAssembly.TryGetValue(assembly, out catalog)) return catalog;
                // The current mod can call game data APIs during OnEnable,
                // before ModLoader.Mods publishes its LoadedMod. Bind only when
                // the frame's assembly actually belongs to the pending folder.
                catalog = CatalogDuringModLoad(assembly);
                if (catalog != null) return catalog;
            }
            return null;
        }

        private static string ReplaceSource(Catalog catalog, string original)
        {
            if (catalog == null || string.IsNullOrEmpty(original) ||
                catalog.AmbiguousSource.Contains(original)) return original;
            string translated;
            return catalog.BySource.TryGetValue(original, out translated)
                ? translated : original;
        }

        // Optional JSON data bridge: updates the in-memory JSON text at resource-load
        // time. Only display-labelled fields are considered and only exact CSV sources
        // are translated. Does not touch the file on disk.
        // A CSV may intentionally store a full translated resource as a key,
        // e.g. key=Content/names.txt, source=Content/names.txt, ko=<file content>,
        // mode=key. This requires no replacement asset inside third-party mods.
        private static string ResourceOverride(Catalog catalog, string path)
        {
            if (catalog == null || string.IsNullOrEmpty(path)) return null;
            string normalized = path.Replace('\\', '/').TrimStart('/');
            string text;
            if (catalog.ByKey.TryGetValue(normalized, out text)) return text;
            if (!Path.IsPathRooted(path)) return null;
            string full;
            try { full = Path.GetFullPath(path); } catch { return null; }
            foreach (KeyValuePair<string, Catalog> root in ByModRoot)
            {
                if (!object.ReferenceEquals(root.Value, catalog)) continue;
                string prefix = root.Key + Path.DirectorySeparatorChar;
                if (!full.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;
                string relative = full.Substring(prefix.Length).Replace('\\', '/');
                if (catalog.ByKey.TryGetValue(relative, out text)) return text;
            }
            return null;
        }

        private static void FileJsonPostfix(string __0, ref string __result)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0) || __result == null) return;
            bool json = __0.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
            bool plainText = __0.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
            if (!json && !plainText) return;
            Catalog catalog = CatalogForFileRead(__0);
            if (catalog != null)
            {
                string wholeResource = ResourceOverride(catalog, __0);
                __result = wholeResource ?? (json
                    ? RewriteJson(catalog, __result) : RewritePlainText(catalog, __result));
            }
        }

        private static void ModFileJsonPostfix(object[] __args, ref string __result)
        {
            if (!_enabled || !IsKorean() || __result == null || __args == null || __args.Length < 2) return;
            string path = __args[1] as string;
            if (string.IsNullOrEmpty(path)) return;
            bool json = path.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
            bool plainText = path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
            if (!json && !plainText) return;
            string owner = __args[0] as string;
            // The API names the resource's owner explicitly. Never fall back
            // to the caller's catalog when another (untranslated) mod is read.
            Catalog catalog = !string.IsNullOrEmpty(owner)
                ? FindCatalogByModId(owner) : GetCallerCatalog();
            if (catalog != null)
            {
                string wholeResource = ResourceOverride(catalog, path);
                __result = wholeResource ?? (json
                    ? RewriteJson(catalog, __result) : RewritePlainText(catalog, __result));
            }
        }

        private static void FileJsonBytesPostfix(string __0, ref byte[] __result)
        {
            if (!_enabled || !IsKorean() || string.IsNullOrEmpty(__0) || __result == null) return;
            bool json = __0.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
            bool plainText = __0.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
            if (!json && !plainText) return;
            Catalog catalog = CatalogForFileRead(__0);
            if (catalog != null) __result = RewriteTextResourceBytes(catalog, __result, json,
                ResourceOverride(catalog, __0));
        }

        private static void ModFileJsonBytesPostfix(object[] __args, ref byte[] __result)
        {
            if (!_enabled || !IsKorean() || __result == null || __args == null || __args.Length < 2) return;
            string path = __args[1] as string;
            if (string.IsNullOrEmpty(path)) return;
            bool json = path.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
            bool plainText = path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
            if (!json && !plainText) return;
            string owner = __args[0] as string;
            // The API names the resource's owner explicitly. Never fall back
            // to the caller's catalog when another (untranslated) mod is read.
            Catalog catalog = !string.IsNullOrEmpty(owner)
                ? FindCatalogByModId(owner) : GetCallerCatalog();
            if (catalog != null) __result = RewriteTextResourceBytes(catalog, __result, json,
                ResourceOverride(catalog, path));
        }

        // Decode only supported text encodings, and return original bytes when
        // JSON parsing or text decoding fails; never corrupt binary resources.
        private static byte[] RewriteJsonBytes(Catalog catalog, byte[] original)
        {
            return RewriteTextResourceBytes(catalog, original, true);
        }

        private static byte[] RewriteTextResourceBytes(Catalog catalog, byte[] original,
            bool json, string wholeResource = null)
        {
            if (catalog == null || original == null || original.Length == 0 ||
                original.Length > 32 * 1024 * 1024) return original;
            try
            {
                int offset = 0;
                Encoding encoding = new UTF8Encoding(false, true);
                if (original.Length >= 3 && original[0] == 0xef &&
                    original[1] == 0xbb && original[2] == 0xbf)
                    offset = 3;
                else if (original.Length >= 2 && original[0] == 0xff && original[1] == 0xfe)
                { encoding = Encoding.Unicode; offset = 2; }
                else if (original.Length >= 2 && original[0] == 0xfe && original[1] == 0xff)
                { encoding = Encoding.BigEndianUnicode; offset = 2; }
                string raw = encoding.GetString(original, offset, original.Length - offset);
                string rewritten = wholeResource ??
                    (json ? RewriteJson(catalog, raw) : RewritePlainText(catalog, raw));
                if (string.Equals(raw, rewritten, StringComparison.Ordinal)) return original;
                byte[] body = encoding.GetBytes(rewritten);
                if (offset == 0) return body;
                byte[] output = new byte[offset + body.Length];
                Buffer.BlockCopy(original, 0, output, 0, offset);
                Buffer.BlockCopy(body, 0, output, offset, body.Length);
                return output;
            }
            catch (Exception ex)
            {
                Error("JSON byte resource bridge: " + ex.Message);
                return original;
            }
        }

        private static void EmbeddedJsonPostfix(Assembly __instance, string __0, ref Stream __result)
        {
            if (!_enabled || !IsKorean() || __instance == null || __result == null ||
                string.IsNullOrEmpty(__0) || !__0.EndsWith(".json", StringComparison.OrdinalIgnoreCase)) return;
            Catalog catalog;
            if (!ByAssembly.TryGetValue(__instance, out catalog)) return;
            try
            {
                byte[] bytes;
                using (var temporary = new MemoryStream())
                {
                    __result.CopyTo(temporary);
                    bytes = temporary.ToArray();
                }
                byte[] rewritten = RewriteJsonBytes(catalog, bytes);
                if (ReferenceEquals(rewritten, bytes))
                {
                    if (__result.CanSeek) __result.Position = 0;
                    else { __result.Dispose(); __result = new MemoryStream(bytes, false); }
                    return;
                }
                __result.Dispose();
                __result = new MemoryStream(rewritten, false);
            }
            catch (Exception ex) { Error("JSON embedded resource bridge: " + ex.Message); }
        }

        private static bool IsFileWithinRoot(string absoluteFile, string folder)
        {
            string root = NormalizedDirectory(folder);
            return !string.IsNullOrEmpty(root) && !string.IsNullOrEmpty(absoluteFile) &&
                absoluteFile.StartsWith(root + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase);
        }

        private static Catalog CatalogForFileRead(string path)
        {
            // A relative path may resolve to ANOTHER mod's data directory, even
            // when that mod has no Korean catalog. Never fall back to the caller
            // in that case or a translated mod will alter the other mod's data.
            Catalog byPath = CatalogForDataPath(path);
            if (byPath != null) return byPath;
            string full;
            try
            {
                full = Path.GetFullPath(path);
                if (Path.IsPathRooted(path)) return null;
            }
            catch { return null; }
            try
            {
                if (ModLoader.Mods != null)
                {
                    foreach (LoadedMod loaded in ModLoader.Mods)
                    {
                        if (loaded == null || string.IsNullOrEmpty(loaded.FolderPath) ||
                            !IsFileWithinRoot(full, loaded.FolderPath)) continue;
                        // This may correctly return null for an untranslated owner.
                        return LocateCatalog(loaded);
                    }
                }
            }
            catch (Exception ex)
            {
                // Unknown ownership is not permission to rewrite a resource.
                Error("Could not determine relative-file mod owner: " + ex.Message);
                return null;
            }
            return GetCallerCatalog();
        }

        private static Catalog CatalogForDataPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            string full, current;
            try
            {
                full = Path.GetFullPath(path);
                current = Path.GetDirectoryName(full);
            }
            catch { return null; }
            // A file can be inside an untranslated mod whose subfolder happens
            // to look like a translated Workshop ID. Respect the actual owner
            // before examining any directory-name aliases.
            try
            {
                if (ModLoader.Mods != null)
                    foreach (LoadedMod loaded in ModLoader.Mods)
                    {
                        if (loaded == null || string.IsNullOrEmpty(loaded.FolderPath) ||
                            !IsFileWithinRoot(full, loaded.FolderPath)) continue;
                        return LocateCatalog(loaded); // null is intentional for untranslated mods
                    }
            }
            catch (Exception ex)
            {
                Error("Could not determine absolute-file mod owner: " + ex.Message);
                return null;
            }
            for (int depth = 0; depth < 6 && !string.IsNullOrEmpty(current); depth++)
            {
                Catalog catalog;
                string normalized = NormalizedDirectory(current);
                if (normalized != null && ByModRoot.TryGetValue(normalized, out catalog)) return catalog;
                if (ByDirectoryId.TryGetValue(Path.GetFileName(current), out catalog)) return catalog;
                string parent = Path.GetDirectoryName(current);
                if (string.Equals(parent, current, StringComparison.OrdinalIgnoreCase)) break;
                current = parent;
            }
            return null;
        }

        // A CSV can also localize data-driven plain-text resources (notably
        // dynamically generated name lists) without shipping a replacement file.
        // Only full source lines that exist in this mod's own CSV are replaced.
        private static string RewritePlainText(Catalog catalog, string original)
        {
            if (catalog == null || string.IsNullOrEmpty(original) || catalog.BySource.Count == 0)
                return original;
            var output = new StringBuilder(original.Length);
            bool changed = false;
            int start = 0;
            while (start < original.Length)
            {
                int end = start;
                while (end < original.Length && original[end] != '\r' && original[end] != '\n') end++;
                string line = original.Substring(start, end - start);
                string korean;
                if (!catalog.AmbiguousSource.Contains(line) &&
                    catalog.BySource.TryGetValue(line, out korean) && korean != line)
                {
                    output.Append(korean);
                    changed = true;
                }
                else output.Append(line);
                if (end < original.Length)
                {
                    output.Append(original[end]);
                    if (original[end] == '\r' && end + 1 < original.Length &&
                        original[end + 1] == '\n') { output.Append('\n'); end++; }
                    end++;
                }
                start = end;
            }
            return changed ? output.ToString() : original;
        }

        private static string RewriteJson(Catalog catalog, string original)
        {
            if (catalog == null || string.IsNullOrEmpty(original) || catalog.BySource.Count == 0) return original;
            try
            {
                JToken root = JToken.Parse(original);
                bool changed = RewriteJsonToken(catalog, root, null, false);
                return changed ? root.ToString(Formatting.None) : original;
            }
            catch (Exception) { return original; } // Non-JSON or incompatible resource: untouched.
        }

        private static bool RewriteJsonToken(Catalog catalog, JToken token, string field,
            bool inTextDictionary)
        {
            if (token is JObject)
            {
                bool changed = false;
                bool inMessages = inTextDictionary || IsTextDictionaryField(field);
                foreach (JProperty property in ((JObject)token).Properties())
                    changed |= RewriteJsonToken(catalog, property.Value, property.Name, inMessages);
                return changed;
            }
            if (token is JArray)
            {
                bool changed = false;
                foreach (JToken child in (JArray)token)
                    changed |= RewriteJsonToken(catalog, child, field, inTextDictionary);
                return changed;
            }
            if (token.Type != JTokenType.String || IsProtectedJsonField(field) ||
                (!inTextDictionary && !IsDisplayJsonField(field))) return false;
            JValue value = token as JValue;
            if (value == null) return false;
            string before = value.Value as string;
            if (string.IsNullOrEmpty(before)) return false;
            string after = TranslateAssignment(catalog, before);
            if (after == before) return false;
            value.Value = after;
            return true;
        }

        private static bool IsProtectedJsonField(string field)
        {
            if (string.IsNullOrEmpty(field)) return false;
            string normalized = field.TrimStart('_').ToLowerInvariant().Replace("_", "").Replace("-", "");
            // Nested `texts` dictionaries sometimes contain metadata alongside
            // display strings. Never translate their identifying fields.
            string[] protectedNames = { "id", "key", "modid", "itemid", "namekey",
                "path", "filepath", "filename", "resource", "resourcename", "asset",
                "assetname", "prefab", "prefabname", "sprite", "icon", "texture",
                "shader", "material", "animation", "sound", "audio", "guid", "hash",
                "version", "type", "kind", "tag", "code", "locale", "language",
                "culture", "class", "classname", "namespace" };
            foreach (string candidate in protectedNames)
                if (normalized == candidate) return true;
            return normalized.EndsWith("id", StringComparison.Ordinal) ||
                normalized.EndsWith("key", StringComparison.Ordinal) ||
                normalized.EndsWith("path", StringComparison.Ordinal) ||
                normalized.EndsWith("filename", StringComparison.Ordinal);
        }

        private static bool IsTextDictionaryField(string field)
        {
            return string.Equals(field, "texts", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, "messages", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, "sentences", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDisplayJsonField(string field)
        {
            return IsVisibleStringMember(field) ||
                string.Equals(field, "texts", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, "messages", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, "sentences", StringComparison.OrdinalIgnoreCase);
        }

        private static void LoadCatalogs(string folder)
        {
            if (!System.IO.Directory.Exists(folder)) throw new DirectoryNotFoundException(folder);
            ByDeclaredId.Clear();
            ByDirectoryId.Clear();
            foreach (string directory in System.IO.Directory.GetDirectories(folder))
            {
                try
                {
                string csv = Path.Combine(directory, "ko.csv");
                if (!File.Exists(csv)) continue;
                Catalog catalog = new Catalog { DirectoryId = Path.GetFileName(directory) };
                List<List<string>> rows = ReadCsv(csv);
                int start = 0;
                if (rows.Count > 0 && rows[0].Count == 1 &&
                    rows[0][0].StartsWith("# mod_id=", StringComparison.Ordinal))
                {
                    catalog.DeclaredModId = rows[0][0].Substring("# mod_id=".Length).Trim();
                    start++;
                }
                for (int i = start; i < rows.Count; i++)
                {
                    List<string> row = rows[i];
                    if (row.Count == 0 || row[0].StartsWith("#", StringComparison.Ordinal)) continue;
                    // The English word "key" is valid source text in a new
                    // two-column CSV. Skip only an actual column header.
                    bool wideHeader = row.Count >= 4 && row[0] == "key" &&
                        row[1] == "source" && row[2] == "ko" && row[3] == "mode";
                    bool narrowHeader = row.Count == 2 && row[0] == "source" && row[1] == "ko";
                    if (wideHeader || narrowHeader) continue;
                    if (row.Count < 2) throw new InvalidDataException("CSV needs 2 or more fields: " + csv + " line " + (i + 1));
                    // Two-column files are accepted for future mods: source,ko.
                    // The source also serves as the native key, so one CSV supports both paths.
                    string key = row[0].Trim();
                    string source = row.Count == 2 ? DecodeText(row[0]) : DecodeText(row[1]);
                    string korean = row.Count == 2 ? DecodeText(row[1]) : DecodeText(row[2]);
                    string mode = row.Count >= 4 ? row[3].Trim().ToLowerInvariant() : "both";
                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(korean)) continue;
                    if (mode != "both" && mode != "key" && mode != "text")
                        throw new InvalidDataException("Unknown mode " + mode + " in " + csv + " line " + (i + 1));
                    string previous;
                    if (mode != "text")
                    {
                        if (catalog.ByKey.TryGetValue(key, out previous) && previous != korean)
                            throw new InvalidDataException("Conflicting translation key " + catalog.DirectoryId + ":" + key);
                        catalog.ByKey[key] = korean;
                    }
                    if (mode != "key" && !string.IsNullOrEmpty(source))
                    {
                        if (catalog.BySource.TryGetValue(source, out previous) && previous != korean)
                            catalog.AmbiguousSource.Add(source);
                        else catalog.BySource[source] = korean;
                    }
                    catalog.RowCount++;
                }
                foreach (string ambiguous in catalog.AmbiguousSource) catalog.BySource.Remove(ambiguous);
                // Validate both identifiers before publishing either mapping;
                // a rejected catalog must not become partially discoverable.
                if (ByDirectoryId.ContainsKey(catalog.DirectoryId))
                    throw new InvalidDataException("Duplicate localization folder " + catalog.DirectoryId);
                if (!string.IsNullOrEmpty(catalog.DeclaredModId) &&
                    ByDeclaredId.ContainsKey(catalog.DeclaredModId))
                    throw new InvalidDataException("Duplicate mod_id: " + catalog.DeclaredModId);
                ByDirectoryId[catalog.DirectoryId] = catalog;
                if (!string.IsNullOrEmpty(catalog.DeclaredModId))
                    ByDeclaredId[catalog.DeclaredModId] = catalog;
                }
                catch (Exception ex)
                {
                    // A newly added malformed CSV must not disable translations
                    // for every previously installed mod. Report the bad folder.
                    Error("Localization folder skipped: " + directory + ": " + ex.Message);
                }
            }
        }

        private static string DecodeText(string s)
        {
            return s == null ? null : s.Replace("\\n", "\n");
        }

        private static List<List<string>> ReadCsv(string path)
        {
            string input = File.ReadAllText(path, new UTF8Encoding(false));
            if (input.Length > 0 && input[0] == '\uFEFF') input = input.Substring(1);
            var rows = new List<List<string>>();
            var row = new List<string>();
            var field = new StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch == '"')
                {
                    if (inQuotes && i + 1 < input.Length && input[i + 1] == '"') { field.Append('"'); i++; }
                    else inQuotes = !inQuotes;
                }
                else if (ch == ',' && !inQuotes)
                { row.Add(field.ToString()); field.Length = 0; }
                else if ((ch == '\r' || ch == '\n') && !inQuotes)
                {
                    if (ch == '\r' && i + 1 < input.Length && input[i + 1] == '\n') i++;
                    row.Add(field.ToString()); field.Length = 0;
                    if (row.Count > 1 || row[0].Length != 0) rows.Add(row);
                    row = new List<string>();
                }
                else field.Append(ch);
            }
            if (inQuotes) throw new InvalidDataException("Unclosed CSV quotation: " + path);
            if (row.Count > 0 || field.Length > 0) { row.Add(field.ToString()); rows.Add(row); }
            return rows;
        }

        private static void Log(string message) { if (_log != null) _log.Log(message); }
        private static void Error(string message) { if (_log != null) _log.Error(message); }
    }
}