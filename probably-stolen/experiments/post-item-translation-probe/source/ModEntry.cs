using System;
using System.Collections.Generic;

namespace PostItemTranslationProbe
{
    // Generic, optional, CSV-only target registry. No foreign mod/item IDs in this DLL.
    public sealed class ModEntry : IMod
    {
        public const string Id = "local.post_item_translation_probe";

        private sealed class Patch
        {
            public Func<GameItem> Original;
            public Func<GameItem> Wrapper;
        }

        private ModLog log;
        private bool enabled, loaded, itemEnabled, paperEnabled, warned;
        private ModItemDirectory directory;
        private readonly List<Translation.Item> items = new List<Translation.Item>();
        private readonly Dictionary<string, List<Translation.Paper>> paper =
            new Dictionary<string, List<Translation.Paper>>(StringComparer.Ordinal);
        private readonly Dictionary<string, Patch> patches =
            new Dictionary<string, Patch>(StringComparer.Ordinal);

        public void Init(ModManifest manifest) { log = new ModLog(manifest); }

        public void OnEnable()
        {
            enabled = true;
            loaded = itemEnabled = paperEnabled = warned = false;
            ModHook.OnModItemDirectoryInit -= DirectoryReady;
            ModHook.OnModItemDirectoryInit += DirectoryReady;
        }

        private void LoadCsv()
        {
            if (loaded) return;
            loaded = true;
            try
            {
                // Official registry, populated after mod initialization. The ID is
                // also the only allowed filename: no index and no arbitrary paths.
                foreach (var mod in ModLoader.Mods)
                {
                    if (mod == null || !mod.IsEnabled || mod.Instance == null ||
                        mod.Manifest == null || string.IsNullOrEmpty(mod.Manifest.ID))
                        continue;
                    string modId = mod.Manifest.ID;
                    if (modId == Id || !ModLoader.IsModEnabled(modId) ||
                        !ModLoader.IsValidModId(modId))
                        continue;

                    string relative = "Translations/" + modId + ".csv";
                    try
                    {
                        if (!ModHelper.ModFileExists(Id, relative)) continue;
                        var parsed = Translation.Parse(modId,
                            ModHelper.ReadModFileText(Id, relative));

                        foreach (var target in parsed.Items) items.Add(target);
                        foreach (var line in parsed.Papers)
                        {
                            string full = modId + ":" + line.BookId;
                            if (!paper.TryGetValue(full, out var group))
                            {
                                group = new List<Translation.Paper>();
                                paper.Add(full, group);
                            }
                            group.Add(line);
                        }
                    }
                    catch (Exception)
                    {
                        // A broken file for one mod never blocks another mod.
                        Warn("One optional mod CSV failed validation; its rows were skipped.");
                    }
                }
            }
            catch (Exception)
            {
                Warn("Mod registry unavailable; no translation changes applied.");
            }

            itemEnabled = items.Count != 0;
            paperEnabled = paper.Count != 0;
            if (itemEnabled)
            {
                ModHook.OnGameLoadedLate -= Check;
                ModHook.OnGameLoadedLate += Check;
            }
            if (paperEnabled)
            {
                ModHook.OnPaperOpened -= PaperOpened;
                ModHook.OnPaperOpened += PaperOpened;
            }
            if (!itemEnabled && !paperEnabled)
            {
                // Truly empty configuration after the one-time discovery check.
                ModHook.OnModItemDirectoryInit -= DirectoryReady;
            }
            else log.Log("CSV targets enabled: items=" + items.Count +
                         " paper=" + paper.Count);
        }

        private void DirectoryReady(ModItemDirectory next)
        {
            if (!enabled || next == null) return;
            LoadCsv();
            if (!itemEnabled) return;

            if (!ReferenceEquals(directory, next))
            {
                Restore();
                directory = next;
            }
            Check();
        }

        private void Check()
        {
            if (!enabled || !itemEnabled || directory == null) return;
            int installed = 0;
            try
            {
                foreach (var target in items)
                {
                    if (!ModLoader.IsModEnabled(target.ModId)) continue;

                    if (patches.TryGetValue(target.ItemId, out var existing))
                    {
                        if (DirectoryMaster.GetFactory<GameItem>(target.ItemId) != existing.Wrapper)
                            Warn("Another mod replaced a wrapped item factory; not overwriting it.");
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
                        GameItem result = captured(); // Exactly one original invocation.
                        if (enabled && owner.IsEnabled)
                        {
                            try { target.Apply(result); }
                            catch (Exception) { Warn("Item translation failed; factory not retried."); }
                        }
                        return result;
                    };

                    // This is a registered factory value, NOT a Harmony/IL patch.
                    directory.factoryDictionary[target.ItemId] = wrapper;
                    patches.Add(target.ItemId,
                        new Patch { Original = captured, Wrapper = wrapper });
                    installed++;
                }
                if (installed != 0) log.Log("CSV item factories wrapped: " + installed);
            }
            catch (Exception) { Warn("Factory registration failed; continuing with original data."); }
        }

        private void PaperOpened(string fullId, PaperUI ui)
        {
            if (!enabled || !paperEnabled || ui == null ||
                !paper.TryGetValue(fullId, out var rules))
                return;

            try
            {
                ApplyPaper(ui, rules);
                // Separate UI instance callback. Handles page flips, no global polling.
                ui.OnPageShown((book, page) =>
                {
                    if (!enabled || !paperEnabled || book == null) return;
                    try { ApplyPaper(book, rules); }
                    catch (Exception) { Warn("Paper page translation failed; original retained."); }
                });
            }
            catch (Exception) { Warn("Paper UI translation failed; original retained."); }
        }

        private static void ApplyPaper(PaperUI ui, List<Translation.Paper> rules)
        {
            if (!ui.IsAlive || ui.Data == null) return;
            var data = ui.Data;
            var page = data.GetPage(ui.CurrentPage);
            if (page == null) return;

            var definitions = new List<PaperElementData>();
            if (data.chrome != null) definitions.AddRange(data.chrome);
            if (page.elements != null) definitions.AddRange(page.elements);
            if (!string.IsNullOrEmpty(page.template))
            {
                var template = data.GetTemplate(page.template);
                if (template != null && template.elements != null)
                    definitions.AddRange(template.elements);
            }

            foreach (var rule in rules)
            {
                int matching = 0;
                bool unexpected = false;
                foreach (var element in definitions)
                {
                    if (element == null || element.id != rule.ElementId) continue;
                    // Only existing static text/button labels; no localized or
                    // runtime-assigned elements and no mutations of book data.
                    if (!(element.IsText || element.IsButton) ||
                        !string.IsNullOrEmpty(element.loc) ||
                        (element.translations != null && element.translations.Count != 0))
                    {
                        unexpected = true;
                        break;
                    }
                    if (ui.ResolveText(element, page) == rule.Original) matching++;
                    else unexpected = true;
                }
                if (!unexpected && matching == 1 && ui.Has(rule.ElementId))
                    ui.SetText(rule.ElementId, rule.Korean);
            }
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
                    catch (Exception) { Warn("Factory restore failed; restart game."); }
                }
            }
            patches.Clear();
            directory = null;
        }

        public void OnDisable()
        {
            enabled = itemEnabled = paperEnabled = loaded = false;
            ModHook.OnModItemDirectoryInit -= DirectoryReady;
            ModHook.OnGameLoadedLate -= Check;
            ModHook.OnPaperOpened -= PaperOpened;
            Restore();
            items.Clear();
            paper.Clear();
        }

        private void Warn(string message)
        {
            if (warned) return;
            warned = true;
            try { log?.Warning(message); } catch (Exception) { }
        }
    }
}
