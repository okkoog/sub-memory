using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    public sealed partial class PortraitWindow : MonoBehaviour
    {
        private bool open;
        private Rect window = new Rect(40, 40, 1000, 690);
        private Vector2 itemScroll, sourceScroll, groupScroll;
        private bool groupOpen;
        private string groupSearch = "";
        private sealed class GroupOption { internal string Name; internal int Count, AdjustableCount, CharacterCount, Tier; }
        private List<GroupOption> groupOptions = new List<GroupOption>();
        private readonly HashSet<string> adjustableSources = new HashSet<string>(StringComparer.Ordinal);
        private bool CanAdjust(Source source) => !FixedFinalWeight(source) && adjustableSources.Contains(source.Id);
        private static bool FixedFinalWeight(Source source) => ModEntry.UseFinalPortraitSelection &&
            (source.Id.StartsWith("game:", StringComparison.Ordinal) || source.Id.StartsWith("final:", StringComparison.Ordinal)) &&
            IsRandomPoolKey(source.CharacterKey ?? source.Name);
        private static bool IsMappedReplacement(Source source) =>
            !string.IsNullOrEmpty(source.CharacterKey) && PortraitRules.IsMappedReplacementId(source.Id, source.MappedCharacter);
        private int summaryRevision = -1, summaryWeights = -1;
        private Dictionary<string, float> roleWeights = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
        private void RefreshRoleWeights()
        {
            if (summaryRevision == Catalog.Revision && summaryWeights == PortraitWeights.Revision) return;
            roleWeights = Catalog.All.Where(IsMappedReplacement).GroupBy(s => s.CharacterKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Sum(s => PortraitWeights.Get(s.Id)), StringComparer.OrdinalIgnoreCase);
            summaryRevision = Catalog.Revision; summaryWeights = PortraitWeights.Revision;
        }
        private bool HasMappedRole(string key) { RefreshRoleWeights(); return roleWeights.ContainsKey(key); }
        private float MappedRoleWeight(string key) { RefreshRoleWeights(); return roleWeights.TryGetValue(key, out var weight) ? weight : 0; }
        private List<Source> visibleSources;
        private int visibleRevision = -1;
        private string visibleSearch, visibleGroup;
        private bool visibleFinal;
        private List<Source> FilteredSources()
        {
            if (visibleSources == null || visibleRevision != Catalog.Revision || visibleSearch != search ||
                visibleGroup != group || visibleFinal != ModEntry.UseFinalPortraitSelection)
            {
                visibleSources = Catalog.All.Where(s => (group == "全部来源" || s.Group == group) &&
                    (s.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    s.Group.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
                    .OrderBy(s => PortraitEligibility.Tier(CanAdjust(s), IsCharacterSource(s))).ToList();
                visibleRevision = Catalog.Revision; visibleSearch = search; visibleGroup = group;
                visibleFinal = ModEntry.UseFinalPortraitSelection;
            }
            return visibleSources;
        }
        private static bool IsRandomPoolKey(string key) => RandomPortraits.IsCaptureKey(key);
        private static bool IsCharacterSource(Source source) => source.Id.StartsWith("game:", StringComparison.Ordinal) ||
            source.Id.StartsWith("final:", StringComparison.Ordinal) ||
            source.MappedCharacter ||
            PortraitRules.Normalize(source.Name).StartsWith("male", StringComparison.Ordinal) ||
            PortraitRules.Normalize(source.Name).StartsWith("female", StringComparison.Ordinal) ||
            PortraitRules.Faction(source.Name) != "" || PortraitRules.SharedWorker(source.Name) ||
            source.Name.IndexOf("_scav", StringComparison.OrdinalIgnoreCase) >= 0 ||
            PortraitEligibility.IsPortrait(source.Name, false);
        private string search = "", itemSearch = "", group = "全部来源", status = "";
        private GameItemElement selected;
        private int page;
        private List<GameItemElement> items = new List<GameItemElement>();
        private List<GameItemElement> currentItems = new List<GameItemElement>();
        private float nextItemRefresh;
        private float nextFinalRefresh, nextCacheCollection;
        private GUIStyle label, button;
        private Font font;
        private float compatRetry;
        private bool weightsDirty;
        private string batchWeight = "1";
        private int catalogRevision = -1;
        private bool bodyTab;
        private int bodyDropdown = -1;
        private Vector2 bodySourceScroll;
        private List<Source> bodySourceOptions = new List<Source>();

        private void Update()
        {
            ProcessImportWork();
            ShapeRules.ProcessOverflow();
            if (Time.unscaledTime >= nextCacheCollection)
            {
                nextCacheCollection = Time.unscaledTime + 2f;
                Portraits.CollectRetired();
            }
            if (open && Time.unscaledTime >= nextItemRefresh)
            {
                nextItemRefresh = Time.unscaledTime + 1f;
                RefreshItems();
            }
            if (Time.unscaledTime >= compatRetry)
            {
                compatRetry = Time.unscaledTime + 2;
                OriginalCompat.Install();
                ReplacementTrace.Install();
            }
            bool wasScanning = FinalPortraits.Scanning;
            FinalPortraits.Tick(open);
            if (open && wasScanning && !FinalPortraits.Scanning)
                status = $"扫描完成，当前共 {Catalog.All.Count} 张候选图片；最终结果已更新，未知来源也可手动使用。";
            if (open && catalogRevision != Catalog.Revision && Time.unscaledTime >= nextFinalRefresh)
            {
                nextFinalRefresh = Time.unscaledTime + .5f;
                Catalog.IncludeTraceSources();
                RefreshGroups(); catalogRevision = Catalog.Revision;
            }
            if (KeyBindHelper.SuppressHotkeys) return;
            if (Input.GetKeyDown(ModEntry.Instance.ManagerKey))
            {
                open = !open;
                if (open) OpenManager();
            }
            if (open && Input.GetKeyDown(KeyCode.Escape))
            { if (importPopup != ImportPopup.None) CancelImportPopup(); else if (bodyDropdown >= 0) bodyDropdown = -1; else if (groupOpen) groupOpen = false; else open = false; }
        }

        private void RefreshItems()
        {
            currentItems = CurrentPortraitItems.Scan(EmporiumEntry.Instance)
                .OrderBy(i => i.name, StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(i => i.uniqueId).ToList();
            items = currentItems.Where(MatchesItemSearch).ToList();
            if (selected == null || !currentItems.Contains(selected) || !MatchesItemSearch(selected))
                selected = items.FirstOrDefault();
        }

        internal void ResetItems()
        {
            itemSearch = "";
            selected = null;
            currentItems.Clear();
            items.Clear();
            itemScroll = Vector2.zero;
            nextItemRefresh = 0;
        }

        private bool MatchesItemSearch(GameItemElement item) => string.IsNullOrWhiteSpace(itemSearch) ||
            (item.name ?? "").IndexOf(itemSearch, StringComparison.CurrentCultureIgnoreCase) >= 0 ||
            item.uniqueId.ToString().IndexOf(itemSearch, StringComparison.OrdinalIgnoreCase) >= 0;

        private void OpenManager()
        {
            if (catalogRevision != Catalog.Revision)
            {
                Catalog.Scan(true);
                RefreshGroups();
                catalogRevision = Catalog.Revision;
            }
            RefreshItems();
            groupOpen = false;
            bodyDropdown = -1;
            importPopup = ImportPopup.None;
            status = $"已读取 {Catalog.All.Count} 张候选立绘、{items.Count} 件物品。";
        }

        private void Reload()
        {
            if (weightsDirty) { PortraitWeights.Save(); weightsDirty = false; }
            PortraitWeights.Load();
            Catalog.Scan(true);
            RefreshGroups();
            catalogRevision = Catalog.Revision;
            groupOpen = false;
            Portraits.Clear();
            ModEntry.Refresh();
            Portraits.CollectRetired();
            RefreshItems();
            page = 0;
            status = $"已读取 {Catalog.All.Count} 张候选立绘、{items.Count} 件物品。";
        }
        private void RefreshGroups()
        {
            visibleSources = null;
            bodySourceOptions = Catalog.All.Where(AdultBodySources.Eligible).GroupBy(s => s.Provider, StringComparer.Ordinal)
                .Select(g => g.First()).OrderBy(s => s.Group, StringComparer.CurrentCultureIgnoreCase).ToList();
            adjustableSources.Clear();
            var mappedRoleKeys = Catalog.All.Where(IsMappedReplacement)
                .Select(s => s.CharacterKey).Where(k => !string.IsNullOrEmpty(k))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var source in Catalog.All.Where(s => PortraitEligibility.CanAdjustWeight(
                s.Id.StartsWith("game:", StringComparison.Ordinal) && IsRandomPoolKey(s.Name),
                IsMappedReplacement(s) && IsRandomPoolKey(s.CharacterKey ?? s.Name),
                LocalPortraits.Eligible(s), mappedRoleKeys.Contains(s.Name))))
                adjustableSources.Add(source.Id);
            groupOptions = Catalog.All.GroupBy(s => s.Group).Select(g => new GroupOption {
                Name = g.Key, Count = g.Count(), AdjustableCount = g.Count(CanAdjust), CharacterCount = g.Count(IsCharacterSource),
                Tier = PortraitEligibility.Tier(g.Any(CanAdjust), g.Any(IsCharacterSource))
            }).OrderBy(g => g.Tier).ThenBy(g => g.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
            if (group != "全部来源" && !groupOptions.Any(g => g.Name == group)) group = "全部来源";
            RefreshImportGroups();
        }

        private Rect GroupPopupRect() => new Rect(267, 132, window.width - 283, Math.Max(90, window.height - 205));

        private void DrawGroupPopup()
        {
            Rect popup = GroupPopupRect();
            GUI.Box(popup, "");
            GUI.Label(new Rect(popup.x + 10, popup.y + 8, 90, 25), "筛选来源", label);
            string next = GUI.TextField(new Rect(popup.x + 100, popup.y + 8, popup.width - 110, 26), groupSearch);
            if (next != groupSearch) { groupSearch = next; groupScroll = Vector2.zero; }
            var options = groupOptions.Where(g => g.Name.IndexOf(groupSearch, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            float viewHeight = popup.height - 48;
            float contentHeight = 34 + options.Count * 34 + 56;
            groupScroll = GUI.BeginScrollView(new Rect(popup.x + 7, popup.y + 41, popup.width - 14, viewHeight),
                groupScroll, new Rect(0, 0, popup.width - 36, contentHeight));
            float rowWidth = popup.width - 40, y = 0;
            void Select(string name) { group = name; page = 0; sourceScroll = Vector2.zero; groupOpen = false; GUI.FocusControl(null); }
            if (GUI.Button(new Rect(0, y, rowWidth, 30), (group == "全部来源" ? "▶ " : "") + "全部来源", button)) Select("全部来源");
            y += 34;
            int? lastSection = null;
            foreach (var option in options)
            {
                int section = option.AdjustableCount > 0 ? 0 : option.CharacterCount > 0 ? 1 : 2;
                if (lastSection != section)
                {
                    GUI.Label(new Rect(5, y, rowWidth - 10, 26), section == 0 ? "五大阵营可调权重" : section == 1 ? "角色立绘，仅手动替换" : "其他图片", label);
                    lastSection = section; y += 28;
                }
                string count = $"可调 {option.AdjustableCount} / 角色 {option.CharacterCount} / 图片 {option.Count}";
                if (GUI.Button(new Rect(0, y, rowWidth, 30), (group == option.Name ? "▶ " : "") + option.Name + "  （" + count + "）", button)) Select(option.Name);
                y += 34;
            }
            GUI.EndScrollView();
        }

        private void OnGUI()
        {
            if (!open) return;
            if (importClearFocus) { GUI.FocusControl(null); importClearFocus = false; }
            if (weightsDirty && (Event.current.rawType == EventType.MouseUp || !open))
            { PortraitWeights.Save(); weightsDirty = false; }
            if (label == null)
            {
                font = Font.CreateDynamicFontFromOSFont(new[] { "Microsoft YaHei", "SimHei", "Arial" }, 16);
                label = new GUIStyle(GUI.skin.label) { font = font, fontSize = 16, wordWrap = true };
                button = new GUIStyle(GUI.skin.button) { font = font, fontSize = 15, wordWrap = true };
            }
            window.width = Math.Min(1060, Screen.width - 24);
            window.height = Math.Min(710, Screen.height - 24);
            window.x = Mathf.Clamp(window.x, 0, Math.Max(0, Screen.width - window.width));
            window.y = Mathf.Clamp(window.y, 0, Math.Max(0, Screen.height - window.height));
            GUI.depth = -1000;
            var oldFont = GUI.skin.font;
            GUI.skin.font = font;
            try { window = GUI.Window(728194, window, Draw, "淫色深空：苗床高清立绘管理器"); }
            finally { GUI.skin.font = oldFont; }
        }
        private void Draw(int id)
        {
            float width = window.width, height = window.height;
            if (groupOpen && Event.current.type == EventType.MouseDown && !GroupPopupRect().Contains(Event.current.mousePosition))
            { groupOpen = false; Event.current.Use(); }
            if (bodyTab && bodyDropdown >= 0 && Event.current.type == EventType.MouseDown &&
                !BodyPopupRect(width, height).Contains(Event.current.mousePosition))
            { bodyDropdown = -1; Event.current.Use(); }
            if (importTab && importPopup != ImportPopup.None && Event.current.type == EventType.MouseDown &&
                !ImportPopupRect(width, height).Contains(Event.current.mousePosition))
            { CancelImportPopup(); Event.current.Use(); }
            bool previousEnabled = GUI.enabled;
            var popupInput = new PopupEventBlock(groupOpen || (bodyTab && bodyDropdown >= 0) || (importTab && importPopup != ImportPopup.None));
            // Popup controls receive input after the underlying controls have been drawn disabled.
            GUI.enabled = previousEnabled && !groupOpen && !(bodyTab && bodyDropdown >= 0) &&
                !(importTab && importPopup != ImportPopup.None);
            if (GUI.Button(new Rect(width - 68, 24, 54, 28), "关闭", button)) open = false;
            if (GUI.Button(new Rect(width - 202, 24, 126, 28),
                FinalPortraits.Scanning ? "继续扫描中…" : "扫描通用来源", button) &&
                !FinalPortraits.Scanning)
            {
                FinalPortraits.StartScan();
                status = "逐帧读取最终图片；关闭管理器即暂停。可追踪的来源同时记录，无法确认的保留为未知来源。";
            }
            GUI.Label(new Rect(16, 26, 116, 28), "纹理清晰度", label);
            if (GUI.Toggle(new Rect(135, 28, 90, 25), ModEntry.Size == 128, "128×128")) ChangeSize("128");
            if (GUI.Toggle(new Rect(235, 28, 90, 25), ModEntry.Size == 256, "256×256")) ChangeSize("256");
            if (GUI.Button(new Rect(345, 24, 108, 28), "重载图片", button)) Reload();
            if (GUI.Button(new Rect(463, 24, 154, 28), "复制 PNG 文件夹", button))
            { GUIUtility.systemCopyBuffer = Path.Combine(ModEntry.Instance.Folder, "Portraits"); status = "文件夹路径已复制。"; }

            GUI.Label(new Rect(630, 24, Math.Max(40, width - 840), 23), "PNG " + ModEntry.Instance.LocalChance.Value + "%", label);
            int chance = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(630, 49, Math.Max(40, width - 840), 16),
                ModEntry.Instance.LocalChance.Value, 0, 100));
            if (chance != ModEntry.Instance.LocalChance.Value)
            { ModEntry.Instance.LocalChance.Set(chance); ModEntry.Instance.Config.Save(); }

            if (GUI.Button(new Rect(16, 64, 100, 29), bodyTab ? "返回立绘" : "体型来源", button))
            { bodyTab = !bodyTab; importTab = false; bodyDropdown = -1; importPopup = ImportPopup.None; }
            if (GUI.Button(new Rect(119, 64, 130, 29), importTab ? "返回立绘" : "导入立绘", button))
            { importTab = !importTab; bodyTab = false; bodyDropdown = -1; importPopup = ImportPopup.None; }
            if (importTab)
            {
                DrawImport(width, height);
                popupInput.Restore(); GUI.enabled = previousEnabled;
                if (importPopup != ImportPopup.None) DrawImportPopup(width, height);
                GUI.DragWindow(new Rect(0, 0, width, 22));
                return;
            }
            if (bodyTab)
            {
                DrawBodySources(width, height, previousEnabled, popupInput);
                GUI.enabled = previousEnabled;
                GUI.DragWindow(new Rect(0, 0, width, 22));
                return;
            }
            GUI.Label(new Rect(119, 99, 130, 29), "快捷键：" + KeyBindHelper.KeyName(ModEntry.Instance.ManagerKey), label);
            float paneHeight = height - 242;
            GUI.Label(new Rect(14, 133, 40, 26), "角色", label);
            string nextItemSearch = GUI.TextField(new Rect(55, 133, 195, 26), itemSearch);
            if (nextItemSearch != itemSearch)
            {
                itemSearch = nextItemSearch;
                items = currentItems.Where(MatchesItemSearch).ToList();
                itemScroll = Vector2.zero;
                if (selected == null || !MatchesItemSearch(selected)) selected = items.FirstOrDefault();
            }
            itemScroll = GUI.BeginScrollView(new Rect(14, 164, 236, paneHeight), itemScroll, new Rect(0, 0, 212, Math.Max(paneHeight, items.Count * 58)));
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (GUI.Button(new Rect(0, i * 58, 210, 52), (ReferenceEquals(item, selected) ? "▶ " : "") + item.name + "\n#" + item.uniqueId + (ShapeRules.IsSquare(item.shape) ? "" : " · 待迁移"), button))
                { selected = item; status = ShapeRules.IsSquare(item.shape) ? "已选择 " + item.name : "正在尝试迁移到柜台、仓库；都放不下时需先腾出空间。"; }
            }
            GUI.EndScrollView();

            float x = 267, paneWidth = width - x - 16;
            GUI.Label(new Rect(x, 64, 55, 25), "搜索", label);
            string next = GUI.TextField(new Rect(x + 55, 65, Math.Max(100, paneWidth - 55), 26), search);
            if (next != search) { search = next; page = 0; }
            float batchWidth = Math.Min(290, paneWidth * .55f);
            float batchX = x + paneWidth - batchWidth;
            if (GUI.Button(new Rect(x, 99, paneWidth - batchWidth - 8, 29), "来源：" + group + "  ▼", button))
            { groupOpen = true; groupSearch = ""; groupScroll = Vector2.zero; GUI.FocusControl(null); }
            if (group != "全部来源" && Catalog.All.Any(s => s.Group == group && CanAdjust(s)))
            {
                GUI.Label(new Rect(batchX, 103, 42, 24), "权重", label);
                batchWeight = GUI.TextField(new Rect(batchX + 43, 100, 49, 27), batchWeight);
                bool batchEnabled = GUI.enabled;
                bool validBatch = float.TryParse(batchWeight, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float batchValue) &&
                    !float.IsNaN(batchValue) && !float.IsInfinity(batchValue) && batchValue >= 0 && batchValue <= 10;
                GUI.enabled = batchEnabled && validBatch && group != "全部来源";
                if (GUI.Button(new Rect(batchX + 98, 99, batchWidth - 98, 29), "一键应用此来源", button))
                {
                    var targets = Catalog.All.Where(s => s.Group == group && CanAdjust(s)).Select(s => s.Id).Distinct().ToList();
                    foreach (string sourceId in targets) PortraitWeights.Set(sourceId, batchValue);
                    PortraitWeights.Save();
                    weightsDirty = false;
                    ShapeRules.LastNotice = "";
                    status = $"已将来源“{group}”的 {targets.Count} 张立绘权重设为 {batchValue:0.###} 并保存（包含搜索结果外和其他页）。";
                }
                GUI.enabled = batchEnabled;
            }

            var sources = FilteredSources();
            const int pageSize = 9;
            int pages = Math.Max(1, (sources.Count + pageSize - 1) / pageSize);
            page = Math.Min(page, pages - 1);
            float bodyHeight = height - 236;
            sourceScroll = GUI.BeginScrollView(new Rect(x, 138, paneWidth, bodyHeight), sourceScroll, new Rect(0, 0, paneWidth - 22, 3 * 185));
            float cardWidth = (paneWidth - 28) / 3;
            int start = page * pageSize;
            for (int i = start; i < Math.Min(start + pageSize, sources.Count); i++)
            {
                var source = sources[i];
                int index = i - start;
                float cx = index % 3 * cardWidth, cy = index / 3 * 185;
                GUI.Box(new Rect(cx, cy, cardWidth - 5, 180), "");
                var sprite = Portraits.Get(source);
                if (sprite) GUI.DrawTexture(new Rect(cx + 8, cy + 5, cardWidth - 21, 98), sprite.texture, ScaleMode.ScaleToFit, true);
                GUI.Label(new Rect(cx + 7, cy + 104, cardWidth - 18, 23), source.Name, label);
                if (FixedFinalWeight(source))
                    GUI.Label(new Rect(cx + 7, cy + 127, cardWidth - 18, 19), "最终来源模式：固定权重 1", label);
                else if (source.Id.StartsWith("game:", StringComparison.Ordinal) && HasMappedRole(source.Name))
                    GUI.Label(new Rect(cx + 7, cy + 127, cardWidth - 18, 19), "替换权重合计 " + MappedRoleWeight(source.Name).ToString("0.0"), label);
                else if (CanAdjust(source))
                {
                    float weight = PortraitWeights.Get(source.Id);
                    GUI.Label(new Rect(cx + 7, cy + 127, 80, 19), "权重 " + weight.ToString("0.0"), label);
                    float newWeight = Mathf.Round(GUI.HorizontalSlider(new Rect(cx + 88, cy + 131, Math.Max(25, cardWidth - 107), 16), weight, 0, 10) * 10) / 10;
                    if (newWeight != weight) { PortraitWeights.Set(source.Id, newWeight); weightsDirty = true; }
                }
                else GUI.Label(new Rect(cx + 7, cy + 127, cardWidth - 18, 19), "仅手动替换", label);
                bool enabled = GUI.enabled;
                GUI.enabled = enabled && selected != null && sprite;
                bool current = selected != null && ModEntry.Get(selected, "IPH_SOURCE") == source.Id;
                if (GUI.Button(new Rect(cx + 7, cy + 147, cardWidth - 19, 26), current ? "已使用" : "使用", button))
                {
                    var target = selected;
                    RefreshItems();
                    if (currentItems.Contains(target))
                    {
                        ModEntry.Set(target, source.Id);
                        target.ValidatePartialSprite();
                        status = "已应用；选择会随游戏存档保存。";
                    }
                    else status = "所选角色已不在当前物品栏中，请重新选择。";
                }
                GUI.enabled = enabled;
            }
            GUI.EndScrollView();
            float footer = height - 87;
            if (GUI.Button(new Rect(x, footer, 62, 28), "上一页", button)) page = Math.Max(0, page - 1);
            GUI.Label(new Rect(x + 73, footer + 3, 150, 25), $"{page + 1} / {pages}  ({sources.Count})", label);
            if (GUI.Button(new Rect(x + 218, footer, 62, 28), "下一页", button)) page = Math.Min(pages - 1, page + 1);
            if (GUI.Button(new Rect(width - 214, footer, 194, 28), "恢复所选物品原立绘", button) && selected != null)
            {
                var target = selected;
                RefreshItems();
                if (currentItems.Contains(target))
                {
                    status = Portraits.TryRestoreOriginal(target, out string error)
                        ? "已恢复原立绘来源，继续使用所选高清档位。" : error;
                }
                else status = "所选角色已不在当前物品栏中，请重新选择。";
            }
            GUI.Label(new Rect(16, height - 49, width - 32, 39),
                (items.Count == 0 ? "当前没有匹配角色；可清空左栏搜索，或切回含目标物品的游戏界面。" :
                string.IsNullOrEmpty(ShapeRules.LastNotice) ? status : ShapeRules.LastNotice) +
                (FinalPortraits.Scanning ? "\n" + FinalPortraits.ScanStatus :
                ReplacementTrace.Enabled ? "\n" + ReplacementTrace.Status : ""), label);
            GUI.DragWindow(new Rect(0, 0, width, 22));
            popupInput.Restore();
            GUI.enabled = previousEnabled;
            if (groupOpen) DrawGroupPopup();
        }
        private void ChangeSize(string size)
        {
            if (ModEntry.Instance.Resolution.Value == size) return;
            ModEntry.Instance.Resolution.Set(size);
            ModEntry.Instance.Config.Save();
            status = "纹理分辨率已切换为 " + size + "×" + size + "；物品尺寸不变。";
        }
        private Rect BodyPopupRect(float width, float height) =>
            new Rect(170, 309, width - 190, Math.Max(60, height - 309 - 110));

        private void DrawBodySources(float width, float height, bool popupEnabled, PopupEventBlock popupInput)
        {
            var mod = ModEntry.Instance;
            bool enabled = GUI.Toggle(new Rect(267, 68, width - 287, 28), mod.BodySourcesEnabled, "开启按成年体型指定来源（仅新生成物品，顾客除外）");
            if (enabled != mod.BodySourcesEnabled) { mod.BodySourcesEnabled = enabled; AdultBodySources.Save(); }
            GUI.Label(new Rect(22, 105, width - 44, 48), "新生成仅使用御姐、少妇、熟女；来源含五阵营替换包及自定义阵营来源。默认不限来源。", label);
            for (int i = 0; i < AdultBodySources.Bodies.Length; i++)
            {
                float y = 162 + i * 46;
                string provider = mod.BodyProviders[i];
                string name = string.IsNullOrEmpty(provider) ? "不限来源" : bodySourceOptions.FirstOrDefault(s => s.Provider == provider)?.Group ?? "来源不可用：" + provider;
                GUI.Label(new Rect(25, y + 5, 140, 29), AdultBodySources.Bodies[i] + " 使用来源", label);
                if (GUI.Button(new Rect(175, y, width - 200, 34), name + "  ▼", button))
                { bodyDropdown = bodyDropdown == i ? -1 : i; bodySourceScroll = Vector2.zero; }
            }
            GUI.Label(new Rect(22, height - 104, width - 44, 78), "指定自定义来源：跳过 PNG 概率，在来源内按权重抽阵营和图片。不限来源：概率命中后合并自定义来源。指定替换包：在当前阵营内选图。无可用图时回退普通流程；体型标签不会改变图片内容。", label);
            popupInput.Restore();
            if (bodyDropdown < 0) return;
            GUI.enabled = popupEnabled;
            Rect popup = BodyPopupRect(width, height);
            GUI.Box(popup, "");
            bodySourceScroll = GUI.BeginScrollView(new Rect(popup.x + 5, popup.y + 4, popup.width - 12, popup.height - 8), bodySourceScroll,
                new Rect(0, 0, width - 228, (bodySourceOptions.Count + 1) * 34));
            void SelectBody(string provider)
            { mod.BodyProviders[bodyDropdown] = provider; AdultBodySources.Save(); bodyDropdown = -1; }
            if (GUI.Button(new Rect(0, 0, width - 235, 30), "不限来源", button)) SelectBody("");
            for (int i = 0; i < bodySourceOptions.Count; i++)
                if (GUI.Button(new Rect(0, (i + 1) * 34, width - 235, 30), bodySourceOptions[i].Group + "  [" + bodySourceOptions[i].Provider + "]", button))
                    SelectBody(bodySourceOptions[i].Provider);
            GUI.EndScrollView();
        }
        private void OnDestroy() { PortraitWeights.Save(); if (font) Destroy(font); }
    }
}
