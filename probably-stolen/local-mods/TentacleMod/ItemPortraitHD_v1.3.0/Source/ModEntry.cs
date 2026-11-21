using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ItemPortraitHD
{
    public sealed class ModEntry : IMod
    {
        internal const string Id = "item_portrait_hd";
        internal static ModEntry Instance;
        internal ConfigEntry<string> Resolution, Hotkey;
        internal ConfigEntry<int> LocalChance;
        internal ConfigEntry<bool> OriginalPortraitSelection;
        internal ConfigEntry<bool> FinalPortraitSelection;
        internal ConfigEntry<bool> WeightedFactions, TraceReplacements, GuestHD, KeepGuestNames, KeepModGuestNames;
        internal ConfigEntry<string> NameRegion;
        internal bool BodySourcesEnabled;
        internal ConfigEntry<string> BodyWeights;
        internal ConfigEntry<bool> BodyBreastEnabled;
        internal ConfigEntry<string> BodyVirginChances;
        internal string[] BodyProviders;
        internal ConfigEntry<string> PortraitGender, ToolGender, CustomerConversion;
        internal ModConfig Config;
        internal PortraitWindow Window;
        internal bool Active;
        private GameObject host;
        internal string Folder;

        public void Init(ModManifest manifest)
        {
            Instance = this;
            Config = ModHelper.GetConfig(Id);
            AdultBodySources.Load();
            // Register first so this compatibility switch is at the top of settings.
            OriginalPortraitSelection = Config.Define("original_portrait_selection", false, "【立绘兼容模式】", () => "仅高清重建：立绘完全由原模组选取",
                () => "默认关闭。开启后关闭本模组所有自动立绘抽取：阵营与角色权重、替换包权重、文件夹 PNG 概率和体型来源分配均不参与选图。保留原模组实际选中的图像，即使来源未知也按下方 128/256 档位重建；默认 256。F7 手动换图、完整 4×4 占格及点击区域、名字和属性设置仍保留。只改变后续选图，不重新随机已有角色。");
            OriginalPortraitSelection.OnChanged += _ => { OriginalCompat.ReconfigureSelection(); RandomPortraits.ResetPending(); Refresh(); };
            FinalPortraitSelection = Config.Define("final_portrait_selection", false, "【立绘兼容模式】", () => "选图仅使用最终生效立绘（固定权重）",
                () => "默认关闭。优先级：仅高清重建 > 体型指定来源 > 自定义 PNG 概率 > 本模式。前两类来源仍使用自己的权重；落到本模式后每个角色基础权重固定为 1，共享职业按阵营分摊，不读取可调权重。按角色键名分阵营，选角后直接读取游戏当前最终生效图片，支持未知来源替换包；无替换则使用原图。陷阱保留部署阵营，顾客仍由原模组选图。仅影响后续生成。");
            FinalPortraitSelection.OnChanged += _ => { RandomPortraits.ResetPending(); Catalog.TraceChanged(); };
            Resolution = Config.Define("resolution", "256", "立绘", () => "物品立绘纹理分辨率",
                new[] { "128", "256" }, () => "纹理清晰度；游戏内显示尺寸和占格不变。立即生效。");
            Hotkey = Config.Define("hotkey", "F7", "立绘", () => "立绘管理器快捷键",
                null,
                () => "默认 F7。在游戏选项的“苗床高清立绘”分区点击快捷键按钮后按新键；Esc 取消。");
            Resolution.OnChanged += _ => { Portraits.Clear(); Refresh(); Portraits.CollectRetired(); };
            GuestHD = Config.Define("guest_portrait_hd", true, "立绘", () => "顾客栏已选立绘高清显示",
                () => "原模组自行选图和生成路径，只在显示时使用其实际选中的源图生成 128/256 高清版本；不参与捕获权重。首次转换仍有开销，之后复用缓存。");
            GuestHD.OnChanged += _ => Refresh();
            KeepGuestNames = Config.Define("keep_guest_names", false, "名字兼容", () => "阻止原模组给游戏原版顾客改名",
                () => "默认关闭。只阻止 lewd_meme 覆盖游戏原版顾客的显示名字，以游戏记录的顾客来源为空为准。换用模组立绘的原版顾客仍属于此类。与下方模组顾客开关独立；不会还原已经被修改的名字。");
            KeepModGuestNames = Config.Define("keep_mod_guest_names", false, "名字兼容", () => "阻止原模组给模组顾客改名",
                () => "默认关闭。只阻止 lewd_meme 覆盖有模组来源记录的顾客名字，包括 lewd_meme 自身及其他模组新增顾客。与上方游戏原版顾客开关独立；两项全开才保护全部顾客。保留选图和其他行为，不恢复已经改过的名字。");
            NameRegion = Config.Define("name_region", "不限制（原版）", "名字兼容", () => "名字地区限制",
                new[] { "不限制（原版）", "仅日本", "仅中国", "仅中日韩" },
                () => "作用于原模组统一名字生成器，包含拾荒新生成物品和顾客角色池新建名字。默认不限制；不修改已有名字和复用池中的旧名字。与阻止来访顾客改名独立生效。");
            LocalChance = Config.Define("local_portrait_chance", 0, "立绘", () => "文件夹立绘替换概率（%）",
                0, 100, () => "体型未指定来源时，按此概率使用 Portraits/来源名/阵营 内 PNG，默认 0%。合并所有自定义来源，先按总权重抽阵营，再按权重选图；不限阵营图片向每个阵营贡献 1/5 权重。指定自定义来源时跳过此概率。未命中或无可用图时回退普通流程。仅影响新物品。");
            WeightedFactions = Config.Define("weighted_capture_factions", true, "立绘", () => "根据立绘数量调整捕获权重",
                () => "默认开启：按各阵营可抽取角色的总权重抽阵营；共享职业在上下城区各计半份。关闭时保留原模组的阵营抽取方式。多 Mod 替换同一角色不增加该角色权重。");
            TraceReplacements = Config.Define("trace_portrait_replacements", true, "立绘", () => "通用立绘替换追踪（实验）",
                () => "监听 SpriteDict.GetSprite 的 Harmony 替换入口，记录各模组实际输出。F7 的扫描按钮同时更新最终图片，关闭本开关仍可扫描最终图片。直接覆盖字典的图片可读取，但不保证识别写入者；已有专用适配优先，不重复计算权重。");
            TraceReplacements.OnChanged += _ => { ReplacementTrace.Install(); Catalog.Scan(true); };
            PortraitGender = Config.Define("portrait_gender", "不限性别", "性别兼容",
                () => "新生成物品的立绘范围", new[] { "仅女性（原版）", "不限性别" },
                () => "在相同阵营的角色池中筛选性别，再查找该角色的 Mod 替换图。文件夹抽取不筛选文件名性别；已有物品不自动换图。");
            ToolGender = Config.Define("tool_gender", "不限性别", "性别兼容",
                () => "原模组道具作用性别", new[] { "仅女性（原版）", "不限性别" },
                () => "不限性别：解除 lewd_meme 对触手精液的女性顾客限制，保留原有效果、顾客存在检查和后续处理。不会更改游戏人物性别。立即生效。");
            CustomerConversion = Config.Define("customer_conversion", "原版行为", "性别兼容",
                () => "原模组普通顾客女性化", new[] { "原版行为", "保留游戏原分布" },
                () => "原模组会将部分普通男性顾客换成女性立绘。选择保留游戏原分布可跳过这一步；仅影响以后生成的顾客。");
            // Sections appear in first-registration order; register the entire body section last.
            BodyWeights = BodyCsvSettings.DefineWeights(Config);
            BodyBreastEnabled = Config.Define("body_affects_breast_size", true, "体型来源", () => "根据体型影响乳量",
                () => "默认开启。新生成苗床使用自定义体型-乳量概率：萝莉 60/35/5、少女 50/40/10、御姐 20/45/35、少妇 20/40/40、熟女 10/40/50（贫乳/普乳/巨乳）。关闭时恢复原模组规则：三种乳量各 1/3，且与体型独立。只影响新生成物品。");
            BodyVirginChances = BodyCsvSettings.DefineChances(Config);
        }

        public void OnEnable()
        {
            Active = true;
            ModHelper.AddGameOptionSection(Id, "portraits", () => "苗床高清立绘");
            ModHelper.AddGameOptionKeyBind(Id, "portraits", "hotkey", () => "立绘管理器快捷键",
                () => ManagerKey, key => { Hotkey.Set(key.ToString()); Config.Save(); },
                () => "打开或关闭立绘管理器。默认 F7；Esc 取消改键。");
            Folder = ModLoader.Mods.FirstOrDefault(m => m.Manifest?.ID == Id)?.FolderPath;
            if (string.IsNullOrEmpty(Folder)) Folder = Path.GetDirectoryName(typeof(ModEntry).Assembly.Location);
            System.IO.Directory.CreateDirectory(Path.Combine(Folder, "Portraits"));
            PortraitWeights.Load();
            // The game's ModLoader already applies this assembly's Harmony patches
            // before Init. Applying them again would duplicate every callback.
            host = new GameObject("ItemPortraitHD");
            Object.DontDestroyOnLoad(host);
            Window = host.AddComponent<PortraitWindow>();
            ModHook.OnGameLoadedNormal += OnLoaded;
            ModHook.OnModItemDirectoryInit += OnDirectoriesReady;
            ModHook.OnModAssetsLoaded += OnAssetsLoaded;
            InitializeIntegrations();
        }

        private static void InitializeIntegrations()
        {
            // Init and first OnEnable run before the loader registers this mod.
            if (!ModAccess.Ready || Instance?.Active != true) return;
            OriginalCompat.Install();
            ReplacementTrace.Install();
            Catalog.Scan();
        }

        private static void OnDirectoriesReady(ModItemDirectory _) => InitializeIntegrations();

        private static void OnAssetsLoaded(LoadedMod mod)
        {
            if (mod?.Manifest?.ID == Id) InitializeIntegrations();
        }

        private void OnLoaded()
        {
            FinalPortraits.StopScan();
            Window?.ResetItems();
            RandomPortraits.InvalidateNative();
            ReplacementTrace.OnLoaded();
            ShapeRules.ResetQueue();
            // The dependency may already have rebuilt and published these sprites
            // during its load callback. Destroying them leaves its registry and
            // injector cache holding dead Unity objects. Texture cache entries
            // contain sources, not save items, and can be reused across saves.
            Portraits.RetryUnavailable();
            InitializeIntegrations();
            Refresh();
        }

        public void OnDisable()
        {
            ModHelper.RemoveGameOptions(Id);
            FinalPortraits.StopScan();
            PortraitWeights.Save();
            Active = false;
            OriginalCompat.Remove();
            ReplacementTrace.Remove();
            ModAccess.Clear();
            ShapeRules.ResetQueue();
            ModHook.OnGameLoadedNormal -= OnLoaded;
            ModHook.OnModItemDirectoryInit -= OnDirectoriesReady;
            ModHook.OnModAssetsLoaded -= OnAssetsLoaded;
            Refresh();
            Portraits.Clear();
            if (host) Object.Destroy(host);
        }

        internal static int Size => Instance?.Resolution?.Value == "128" ? 128 : 256;
        internal KeyCode ManagerKey => Enum.TryParse(Hotkey.Value, out KeyCode key) &&
            Enum.IsDefined(typeof(KeyCode), key) && key != KeyCode.None && !KeyBindHelper.IsReserved(key) ? key : KeyCode.F7;
        internal static bool UseOriginalPortraitSelection => Instance?.Active == true && Instance.OriginalPortraitSelection?.Value == true;
        internal static bool UseFinalPortraitSelection => Instance?.Active == true && !UseOriginalPortraitSelection &&
            Instance.FinalPortraitSelection?.Value == true;
        internal static bool Target(GameItem item) => item != null && item.IsTag("MAIDEN_FEMALE") && !GuestPortraits.IsGuest(item);
        internal static string Get(GameItem item, string key) => item?.GetTagReadonly(key)?.GetString() ?? "";
        internal static void Set(GameItem item, string value) => item.ModifyTag("IPH_SOURCE", t => t.SetString(value), false);
        internal static void Warn(Exception ex) => UnityEngine.Debug.LogWarning("[ItemPortraitHD] " + ex.Message);

        internal static void Refresh()
        {
            foreach (var element in Seen.Live())
            {
                try
                {
                    if (ShapeRules.Ensure(element)) element.ValidateShapeState();
                    element.ValidatePartialSprite();
                }
                catch (Exception ex) { Warn(ex); }
            }
        }
    }

    // GameItemElement inherits GameItem. Keep weak references so visiting inventories
    // does not keep an old save's item graph alive.
    internal static class Seen
    {
        private static readonly List<WeakReference<GameItemElement>> items = new List<WeakReference<GameItemElement>>();
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<GameItemElement, object> known =
            new System.Runtime.CompilerServices.ConditionalWeakTable<GameItemElement, object>();
        internal static void Add(GameItemElement item)
        {
            if (known.TryGetValue(item, out _)) return;
            known.Add(item, new object());
            items.Add(new WeakReference<GameItemElement>(item));
        }
        internal static List<GameItemElement> Live()
        {
            var list = new List<GameItemElement>();
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (!items[i].TryGetTarget(out var item) || item.background == null || !item.background.image)
                { if (item != null) known.Remove(item); items.RemoveAt(i); continue; }
                list.Add(item);
            }
            return list;
        }
    }

    [HarmonyPatch(typeof(GameItemElement), "ResolveItemSprite")]
    internal static class ResolvePatch
    {
        [HarmonyPrefix]
        private static bool Prefix(GameItemElement __instance, ref Sprite __result)
        {
            if (ModEntry.Instance?.Active == true && GuestPortraits.IsGuest(__instance))
            {
                Seen.Add(__instance);
                var guest = GuestPortraits.For(__instance);
                if (!guest) return true;
                __result = guest; return false;
            }
            if (ModEntry.Instance?.Active != true || !ModEntry.Target(__instance)) return true;
            Seen.Add(__instance);
            try
            {
                var sprite = Portraits.For(__instance);
                if (!sprite) return true;
                __result = sprite;
                return false;
            }
            catch (Exception ex) { ModEntry.Warn(ex); return true; }
        }
    }

    [HarmonyPatch]
    internal static class DisplaySizePatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(GameItemElement), "ValidatePartialSprite");
            yield return AccessTools.Method(typeof(GameItemElement), "Validate");
            yield return AccessTools.Method(typeof(GameItemElement), "ValidateSelectLocked");
        }
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        private static void Postfix(GameItemElement __instance)
        {
            if (ModEntry.Instance?.Active != true || (!ModEntry.Target(__instance) && !GuestPortraits.IsGuest(__instance))) return;
            FitItem(__instance);
        }
        internal static void FitItem(GameItemElement item)
        {
            if (item == null) return;
            if (GuestPortraits.IsGuest(item))
            {
                Fit(item.background?.image); Fit(item.fakeBackground?.image);
                return;
            }
            Normalize(item.background?.image, item);
            Normalize(item.fakeBackground?.image, item);
            Fit(item.background?.image);
            Fit(item.fakeBackground?.image);
            // The original constructor uses a positive alpha hit threshold. Zero
            // makes transparent pixels participate in the existing click/drag events.
            if (item.background?.image)
            {
                item.background.image.alphaHitTestMinimumThreshold = 0;
                item.background.image.raycastTarget = true;
            }
        }
        private static readonly HashSet<int> fallbackWarnings = new HashSet<int>();
        private static void Normalize(Image image, GameItemElement item)
        {
            if (!image || !image.sprite || Portraits.Owns(image.sprite)) return;
            var input = image.sprite;
            var normalized = Portraits.Get(new Source { Id = "display:" + input.GetInstanceID(), Name = input.name, Resolve = () => input });
            if (!normalized) return;
            image.sprite = normalized;
            if (fallbackWarnings.Count < 128 && fallbackWarnings.Add(input.GetInstanceID()))
                UnityEngine.Debug.Log("[ItemPortraitHD] 显示兜底转换: " + item.spritePath + " / " + input.name +
                    " rect=" + input.rect + " ppu=" + input.pixelsPerUnit);
        }
        internal static void Fit(Image image)
        {
            if (!image || !image.sprite || !Portraits.Owns(image.sprite)) return;
            image.type = Image.Type.Simple;
            // UI.Image.SetNativeSize uses the canvas reference PPU, not necessarily
            // the game's item PPU. Explicitly preserve the original 64 pixel canvas.
            float canvas = UISettings.current != null ? UISettings.current.gridSize * 4 : 64;
            var rect = image.rectTransform;
            rect.localScale = Vector3.one;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas);
        }
    }

    // Some game/other-mod paths call SetNativeSize after item validation, including
    // drag previews. Clamp at that operation as well; only our generated Sprites
    // are affected, so the original GuestBox and unrelated items are excluded.
    [HarmonyPatch(typeof(Image), "SetNativeSize")]
    internal static class PortraitNativeSizePatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        private static void Postfix(Image __instance)
        {
            if (ModEntry.Instance?.Active == true) DisplaySizePatch.Fit(__instance);
        }
    }

    internal sealed class Source
    {
        internal string Id, Group, Name, File;
        internal string Provider;
        internal string LocalFaction;
        internal Func<Sprite> Resolve;
        internal Func<byte[]> ReadBytes;
        internal string MatchSpriteName;
        internal bool? MatchReadable;
        internal bool MappedCharacter;
        internal string CharacterKey;
    }

    internal static class Catalog
    {
        internal static int Revision { get; private set; }
        internal static int LocalRevision { get; private set; }
        internal static readonly List<Source> All = new List<Source>();
        private static readonly Dictionary<string, Source> byId = new Dictionary<string, Source>(StringComparer.Ordinal);
        internal static Source Find(string id) => id != null && byId.TryGetValue(id, out var s) ? s : null;
        internal static void TraceChanged() { Revision++; }
        internal static void IncludeTraceSources()
        {
            // Use the existing replacement filter so a traced result never adds a
            // second weight for a provider that already has an explicit mapping.
            foreach (string key in ReplacementTrace.Sources.Select(s => s.CharacterKey).Distinct(StringComparer.Ordinal).ToArray())
                foreach (var source in RandomPortraits.Replacements(key))
                { source.MappedCharacter = true; source.CharacterKey = key; Add(source); }
        }
        internal static void ObserveFinal(string key, Sprite sprite)
        {
            string id = "final:" + key;
            var native = RandomPortraits.Native(key).Resolve();
            var existing = Find(id);
            if (!sprite)
            {
                if (existing != null) { All.Remove(existing); byId.Remove(id); Revision++; }
                return;
            }
            bool original = native && ReferenceEquals(sprite, native);
            string name = key + (original ? "（原图）" : "（最终生效）");
            if (existing != null && existing.Name == name && ReferenceEquals(existing.Resolve?.Invoke(), sprite)) return;
            // Original portraits are valid final results too. Keep them visible
            // without attributing them to a provider or scanning every mod for one.
            var owner = original ? null : FindFinalOwner(key, sprite);
            string group = original ? "最终生效立绘 / 原图（未检测到替换）" :
                owner == null ? "最终生效立绘（来源未识别）" : "最终生效立绘 / " + (owner.Manifest.Name ?? owner.Manifest.ID);
            if (existing == null)
                Add(new Source { Id = id, Name = name, Group = group,
                    Provider = owner?.Manifest.ID, CharacterKey = key, Resolve = () => sprite });
            else
            {
                existing.Resolve = () => sprite;
                existing.Name = name; existing.Group = group; existing.Provider = owner?.Manifest.ID;
            }
            Revision++;
        }
        private static void Add(Source source)
        {
            if (byId.TryGetValue(source.Id, out var existing))
            {
                existing.MappedCharacter |= source.MappedCharacter;
                if (!string.IsNullOrEmpty(source.Provider)) existing.Provider = source.Provider;
                if (!string.IsNullOrEmpty(source.CharacterKey)) existing.CharacterKey = source.CharacterKey;
                if (source.File != null && existing.File == null) existing.File = source.File;
                return;
            }
            All.Add(source); byId.Add(source.Id, source);
        }
        internal static void Scan(bool includeMapped = false)
        {
            LocalRevision++;
            Revision++;
            RandomPortraits.InvalidateSources();
            All.Clear(); byId.Clear();
            try
            {
                if (SpriteDict.Instance && SpriteDict.Instance.spriteDictionary is Dictionary<string, Sprite> dict)
                    foreach (var key in dict.Keys.OrderBy(k => k, StringComparer.Ordinal))
                    {
                        var captured = key;
                        Add(new Source { Id = "game:" + key, Name = key, Group = "游戏基础", MappedCharacter = true, CharacterKey = key,
                            Resolve = () => RandomPortraits.Native(captured).Resolve() });
                        // Character getter hooks can replace the result without changing
                        // the backing dictionary. Observe their final result for the UI too.
                        var liveSprite = LocalPortraitRules.Factions.Any(f => PortraitRules.FactionWeight(key, f) > 0)
                            ? SpriteDict.Instance.GetSprite(key) : dict[key];
                        ObserveFinal(key, liveSprite);
                    }
                foreach (var mod in ModLoader.Mods.Where(m => m.IsEnabled && m.Manifest != null &&
                    m.Manifest.ID != ModEntry.Id && m.Manifest.ID != "lewd_meme" && m.Manifest.ID != "lewd_meme_remake"))
                {
                    var capturedMod = mod;
                    if (mod.Sprites != null)
                        foreach (var key in mod.Sprites.Keys.OrderBy(k => k, StringComparer.Ordinal))
                        {
                            var captured = key;
                            Add(new Source { Id = "mod:" + mod.Manifest.ID + ":" + key,
                                Group = mod.Manifest.Name ?? mod.Manifest.ID, Name = key, Provider = mod.Manifest.ID,
                                Resolve = () => capturedMod.IsEnabled && capturedMod.Sprites.TryGetValue(captured, out var s) ? s : null });
                        }
                    // Read user packs without copying any images or code from their manager.
                    var packs = Path.Combine(mod.FolderPath, "packs");
                    if (System.IO.Directory.Exists(packs)) ScanFiles(packs, "pack:" + mod.Manifest.ID + ":", "头像包 / " + mod.Manifest.Name);
                }
                ScanLocalFiles(Path.Combine(ModEntry.Instance.Folder, "Portraits"));
                if (includeMapped)
                {
                    foreach (var source in All.Where(s => s.Id.StartsWith("game:", StringComparison.Ordinal)).ToArray())
                        foreach (var replacement in RandomPortraits.Replacements(source.Name))
                        { replacement.MappedCharacter = true; replacement.CharacterKey = source.Name; Add(replacement); }
                    foreach (var source in RandomPortraits.ManualSources()) Add(source);
                }
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }
        private static void ScanFiles(string root, string prefix, string group)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(root, "*.png", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal))
            {
                var relative = file.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/');
                Add(new Source { Id = prefix + relative, Group = group, Name = relative, File = file });
            }
        }
        private static LoadedMod FindFinalOwner(string key, Sprite sprite)
        {
            var allGirlsOwner = AllGirlsSources.FinalOwner(key, sprite);
            if (allGirlsOwner != null) return allGirlsOwner;
            var rimworldOwner = RimworldSources.FinalOwner(key, sprite);
            if (rimworldOwner != null) return rimworldOwner;
            var mods = ModLoader.Mods.Where(m => m.IsEnabled && m.Manifest != null && m.Assembly != null).ToList();
            foreach (var mod in mods.Where(m => m.Manifest.ID == "local_space_pawn_portraits"))
            {
                try
                {
                    var type = ModAccess.Type(mod.Manifest.ID, "SpacePawnPortraits.PortraitMod");
                    if (type == null) continue;
                    var active = ModAccess.Static<object>(type, "active");
                    var bindings = active == null ? null : ModReflect.Get(active, "bindings") as System.Collections.IEnumerable;
                    if (bindings == null) continue;
                    foreach (var binding in bindings)
                    {
                        if (binding == null) continue;
                        string boundKey = ModReflect.Get<string>(binding, "Key");
                        var replacement = ModReflect.Get<Sprite>(binding, "Replacement");
                        if (boundKey == key && ReferenceEquals(replacement, sprite)) return mod;
                    }
                }
                catch (Exception ex) { ModEntry.Warn(ex); }
            }

            var namedOwner = mods.OrderByDescending(m => m.Assembly.GetName().Name.Length)
                .FirstOrDefault(m => sprite.name != null &&
                    sprite.name.StartsWith(m.Assembly.GetName().Name + "/", StringComparison.Ordinal));
            return namedOwner ?? ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Manifest != null && m.Sprites != null &&
                m.Sprites.Values.Any(s => ReferenceEquals(s, sprite)));
        }
        private static void ScanLocalFiles(string root)
        {
            if (!System.IO.Directory.Exists(root)) return;
            foreach (var file in System.IO.Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                .Where(f => string.Equals(Path.GetExtension(f), ".png", StringComparison.OrdinalIgnoreCase)).OrderBy(f => f, StringComparer.Ordinal))
            {
                var relative = file.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/');
                Add(LocalSource(file, relative));
            }
        }
        private static Source LocalSource(string file, string relative)
        {
            LocalPortraitRules.TryClassify(relative, out var provider, out var faction);
            string sourceName = relative.IndexOf('/') > 0 ? relative.Substring(0, relative.IndexOf('/')) : null;
            return new Source { Id = "local:" + relative, File = file, Name = relative, Provider = provider, LocalFaction = faction,
                Group = sourceName == null ? "我的 PNG（未分类）" : "自定义 / " + sourceName };
        }
        internal static void RegisterImported(string file)
        {
            string root = Path.GetFullPath(Path.Combine(ModEntry.Instance.Folder, "Portraits")) + Path.DirectorySeparatorChar;
            string absolute = Path.GetFullPath(file);
            if (!absolute.StartsWith(root, StringComparison.OrdinalIgnoreCase)) throw new IOException("图片不在 Portraits 内。");
            Add(LocalSource(absolute, absolute.Substring(root.Length).Replace('\\', '/')));
            LocalRevision++;
            Revision++;
        }
        internal static Source Original(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            if (key.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal)) return RandomPortraits.Find(key);
            int colon = key.IndexOf(':');
            if (colon > 0)
            {
                var found = Find("mod:" + key);
                if (found != null) return found;
                string modId = key.Substring(0, colon), spriteKey = key.Substring(colon + 1);
                return new Source { Id = "mod:" + key, Resolve = () =>
                {
                    var mod = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Manifest?.ID == modId);
                    return mod?.Sprites != null && mod.Sprites.TryGetValue(spriteKey, out var sprite) ? sprite : null;
                }};
            }
            // Catalog's game: cards deliberately resolve serialized originals.
            // Automatic restoration/injection must instead consume the live getter.
            // Use a separate cache identity so F7 original previews cannot collide.
            return new Source { Id = "effective:" + key,
                Resolve = () => SpriteDict.Instance ? SpriteDict.Instance.GetSprite(key) : null };
        }
    }
}

