using System;
using System.Collections.Generic;

namespace PostItemTranslationProbe
{
    // Targets are exclusively read from Translations/items.csv.
    public sealed class ModEntry : IMod
    {
        public const string Id = "local.post_item_translation_probe";
        private ModLog log;
        private bool enabled, armed, warned;
        private ModItemDirectory directory;
        private readonly List<Translation.TargetItem> active = new List<Translation.TargetItem>();
        private readonly Dictionary<string, Patch> patches = new Dictionary<string, Patch>(StringComparer.Ordinal);

        private sealed class Patch
        {
            public Func<GameItem> Original;
            public Func<GameItem> Wrapper;
        }

        public void Init(ModManifest manifest) { log = new ModLog(manifest); }

        public void OnEnable()
        {
            enabled = true;
            // A short-lived bootstrap is required to read our own mod CSV using the game's file API.
            ModHook.OnModAssetsLoaded -= Assets;
            ModHook.OnModAssetsLoaded += Assets;
        }

        private void Assets(LoadedMod loaded)
        {
            if (loaded == null || loaded.Manifest == null || loaded.Manifest.ID != Id) return;
            ModHook.OnModAssetsLoaded -= Assets;
            if (!enabled) return;
            try
            {
                var document = Translation.Parse(ModHelper.ReadModFileText(Id, "Translations/items.csv"));
                active.Clear();
                foreach (var target in document.Items)
                    if (ModLoader.IsModEnabled(target.ModId) && ModHelper.IsModRunning(target.ModId))
                        active.Add(target);
                if (active.Count == 0) return;
                armed = true;
                ModHook.OnModItemDirectoryInit -= DirectoryReady;
                ModHook.OnModItemDirectoryInit += DirectoryReady;
                ModHook.OnGameLoadedLate -= Check;
                ModHook.OnGameLoadedLate += Check;
            }
            catch (Exception) { Warn("CSV loading or activation failed. Factories unchanged."); }
        }

        private void DirectoryReady(ModItemDirectory next)
        {
            if (!ReferenceEquals(directory, next))
            {
                Restore();
                directory = next;
            }
            Check();
        }

        private void Check()
        {
            if (!enabled || !armed || directory == null) return;
            int installed = 0;
            try
            {
                foreach (var target in active)
                {
                    if (!ModHelper.IsModRunning(target.ModId)) continue;
                    if (patches.TryGetValue(target.ItemId, out var patch))
                    {
                        if (DirectoryMaster.GetFactory<GameItem>(target.ItemId) != patch.Wrapper)
                            Warn("Another mod replaced a wrapped factory; not taking it back.");
                        continue;
                    }
                    var original = DirectoryMaster.GetFactory<GameItem>(target.ItemId);
                    if (original == null ||
                        !directory.factoryDictionary.TryGetValue(target.ItemId, out var local) ||
                        original != local) continue;

                    var owner = ModSourceHelper.FindModByDelegate(original);
                    if (owner == null || owner.Manifest == null ||
                        !owner.IsEnabled || owner.Manifest.ID != target.ModId) continue;

                    var captured = original;
                    Func<GameItem> wrapper = () =>
                    {
                        var item = captured(); // Original exactly once, including null/exception cases.
                        if (enabled && armed && owner.IsEnabled)
                        {
                            try { target.Apply(item); }
                            catch (Exception) { Warn("Translation failed. Original factory not retried."); }
                        }
                        return item;
                    };
                    directory.factoryDictionary[target.ItemId] = wrapper;
                    patches.Add(target.ItemId, new Patch { Original = captured, Wrapper = wrapper });
                    installed++;
                }
                if (installed != 0) log.Log("CSV factory wrappers installed: " + installed);
            }
            catch (Exception) { Warn("Factory check failed; proceeding without further changes."); }
        }

        private void Restore()
        {
            if (directory != null)
            {
                foreach (var entry in patches)
                {
                    try
                    {
                        if (directory.factoryDictionary.TryGetValue(entry.Key, out var current) &&
                            current == entry.Value.Wrapper)
                            directory.factoryDictionary[entry.Key] = entry.Value.Original;
                    }
                    catch (Exception) { Warn("Factory restoration failed; restart the game."); }
                }
            }
            patches.Clear();
            directory = null;
        }

        public void OnDisable()
        {
            enabled = false;
            armed = false;
            ModHook.OnModAssetsLoaded -= Assets;
            ModHook.OnModItemDirectoryInit -= DirectoryReady;
            ModHook.OnGameLoadedLate -= Check;
            Restore();
            active.Clear();
        }

        private void Warn(string message)
        {
            if (warned) return;
            warned = true;
            try { log?.Warning(message); } catch (Exception) { }
        }
    }
}
