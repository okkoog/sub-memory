using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ItemPortraitHD
{
    public sealed partial class PortraitWindow
    {
        private enum ImportPopup { None, Browse, Destination }
        private sealed class ImportGroup { internal string Name; internal List<Source> Images; internal int PortraitCount; }
        private bool importTab, importOtherImages, importClearFocus;
        private ImportPopup importPopup;
        private List<ImportGroup> importGroups = new List<ImportGroup>();
        private List<Source> importImages = new List<Source>();
        private List<string> importTargets = new List<string>();
        private string importGroup = "", importSearch = "", importPopupSearch = "", importNewName = "", importTarget = "";
        private string importFaction = LocalPortraitRules.Unrestricted;
        private string importStatus = "选择一个模组来源，点“加入”，在弹窗中选择目标来源和阵营并确认。";
        private int importPage;
        private Vector2 importGridScroll, importPopupScroll;
        private Source importCandidate, queuedImport;
        private string queuedTarget, queuedFaction, queuedCreate;
        private readonly Dictionary<string, string> importedLabels = new Dictionary<string, string>(StringComparer.Ordinal);
        private string PortraitRoot => Path.Combine(ModEntry.Instance.Folder, "Portraits");

        // Disabled controls may still process scroll events in IMGUI. Hide input
        // from every background control, then restore it only for the top popup.
        private sealed class PopupEventBlock
        {
            private readonly Event current;
            private readonly EventType original;
            private readonly bool blocked;
            internal PopupEventBlock(bool active)
            {
                current = Event.current; original = current.type;
                blocked = active && (current.isMouse || current.isKey || original == EventType.ScrollWheel || original == EventType.ContextClick);
                if (blocked) current.type = EventType.Ignore;
            }
            internal void Restore() { if (blocked) current.type = original; }
        }

        private void RefreshImportGroups()
        {
            importGroups = Catalog.All.Where(PortraitImporter.Eligible).GroupBy(s => s.Group, StringComparer.Ordinal)
                .Select(g => new ImportGroup { Name = g.Key, Images = g.OrderByDescending(IsCharacterSource)
                    .ThenBy(s => s.Name, StringComparer.CurrentCultureIgnoreCase).ToList(), PortraitCount = g.Count(IsCharacterSource) })
                .OrderByDescending(g => g.PortraitCount > 0).ThenBy(g => g.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
            if (!importGroups.Any(g => g.Name == importGroup)) importGroup = importGroups.FirstOrDefault()?.Name ?? "";
            FilterImportImages();
        }
        private void FilterImportImages()
        {
            var group = importGroups.FirstOrDefault(g => g.Name == importGroup);
            importImages = (group?.Images ?? new List<Source>()).Where(s => (importOtherImages || IsCharacterSource(s)) &&
                (s.Name.IndexOf(importSearch, StringComparison.OrdinalIgnoreCase) >= 0 ||
                (s.CharacterKey ?? "").IndexOf(importSearch, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();
            importPage = 0; importGridScroll = Vector2.zero;
        }
        private void RefreshImportTargets()
        {
            importTargets = System.IO.Directory.Exists(PortraitRoot) ? System.IO.Directory.EnumerateDirectories(PortraitRoot)
                .Select(Path.GetFileName).OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase).ToList() : new List<string>();
        }
        private void OpenImportPopup(ImportPopup next)
        {
            importPopup = next; importPopupSearch = ""; importPopupScroll = Vector2.zero; GUI.FocusControl(null);
        }
        private void CancelImportPopup()
        { importPopup = ImportPopup.None; importCandidate = null; importClearFocus = true; }
        private Rect ImportPopupRect(float width, float height) => new Rect(26, 145, width - 52, Math.Max(140, height - 205));

        private void ProcessImportWork()
        {
            // File I/O and catalog changes happen in Update, never inside an IMGUI scroll view.
            if (queuedCreate != null)
            {
                string name = queuedCreate; queuedCreate = null;
                try
                {
                    PortraitImporter.CreateProvider(PortraitRoot, name); RefreshImportTargets();
                    importTarget = name; importNewName = "";
                    importPopupScroll = Vector2.zero;
                    importStatus = "已创建来源“" + name + "”，选择阵营后点确认加入。";
                }
                catch (Exception ex) { importStatus = "创建失败：" + ex.Message; }
            }
            if (queuedImport == null) return;
            var source = queuedImport; queuedImport = null;
            try
            {
                string file = PortraitImporter.Import(source, PortraitRoot, queuedTarget, queuedFaction, out bool existed);
                Catalog.RegisterImported(file);
                int previousPage = importPage; Vector2 previousScroll = importGridScroll;
                RefreshGroups(); catalogRevision = Catalog.Revision;
                importPage = Math.Min(previousPage, Math.Max(0, (importImages.Count - 1) / 9)); importGridScroll = previousScroll;
                string destination = queuedTarget + "/" + queuedFaction;
                importedLabels[source.Id] = destination;
                importStatus = (existed ? "已存在相同图片，未重复保存：" : "已加入：") + destination + "。已生效，无需重载。";
            }
            catch (Exception ex) { importStatus = "导入失败：" + ex.Message; }
        }

        private void DrawImport(float width, float height)
        {
            bool busy = queuedImport != null || queuedCreate != null;
            bool enabled = GUI.enabled; GUI.enabled = enabled && !busy;
            if (GUI.Button(new Rect(20, 103, width - 40, 32), "图片来源：" + (importGroup == "" ? "暂无来源" : importGroup) + "  ▼", button))
                OpenImportPopup(ImportPopup.Browse);
            GUI.Label(new Rect(20, 146, 52, 25), "搜索", label);
            string next = GUI.TextField(new Rect(75, 143, Math.Max(80, width - 350), 28), importSearch);
            bool others = GUI.Toggle(new Rect(width - 257, 147, 237, 25), importOtherImages, "显示其他图片（含未识别）");
            if (next != importSearch || others != importOtherImages)
            { importSearch = next; importOtherImages = others; FilterImportImages(); }

            const int pageSize = 9;
            int pages = Math.Max(1, (importImages.Count + pageSize - 1) / pageSize);
            importPage = Math.Min(importPage, pages - 1);
            float viewport = Math.Max(90, height - 275), cardHeight = Math.Max(139, viewport / 3), cardWidth = (width - 68) / 3;
            importGridScroll = GUI.BeginScrollView(new Rect(20, 181, width - 40, viewport), importGridScroll,
                new Rect(0, 0, width - 62, 3 * cardHeight));
            int start = importPage * pageSize;
            for (int i = start; i < Math.Min(start + pageSize, importImages.Count); i++)
            {
                var source = importImages[i]; int index = i - start;
                float x = index % 3 * cardWidth, y = index / 3 * cardHeight;
                GUI.Box(new Rect(x, y, cardWidth - 5, cardHeight - 5), "");
                var sprite = Portraits.Get(source);
                if (sprite) GUI.DrawTexture(new Rect(x + 8, y + 5, cardWidth - 21, cardHeight - 71), sprite.texture, ScaleMode.ScaleToFit, true);
                GUI.Label(new Rect(x + 8, y + cardHeight - 64, cardWidth - 20, 26), source.Name, label);
                bool cardEnabled = GUI.enabled; GUI.enabled = cardEnabled && sprite;
                if (GUI.Button(new Rect(x + 8, y + cardHeight - 34, cardWidth - 21, 25),
                    importedLabels.ContainsKey(source.Id) ? "再次加入…" : "加入…", button))
                {
                    importCandidate = source;
                    try
                    {
                        RefreshImportTargets();
                        if (!importTargets.Contains(importTarget)) importTarget = "";
                        OpenImportPopup(ImportPopup.Destination);
                    }
                    catch (Exception ex) { importStatus = "读取目标来源失败：" + ex.Message; importCandidate = null; }
                }
                GUI.enabled = cardEnabled;
            }
            GUI.EndScrollView();
            if (importImages.Count == 0)
                GUI.Label(new Rect(40, 200, width - 80, 70), "当前没有符合条件的图片。可切换来源、清除搜索，或开启“显示其他图片”；通用追踪图片需先扫描。", label);
            float footer = height - 84;
            if (GUI.Button(new Rect(20, footer, 80, 28), "上一页", button)) { importPage = Math.Max(0, importPage - 1); importGridScroll = Vector2.zero; }
            GUI.Label(new Rect(113, footer + 3, width - 236, 26), $"{importPage + 1} / {pages} · {importImages.Count} 张 · 按当前配置保存 {ModEntry.Size}×{ModEntry.Size} 预处理 PNG", label);
            if (GUI.Button(new Rect(width - 100, footer, 80, 28), "下一页", button)) { importPage = Math.Min(pages - 1, importPage + 1); importGridScroll = Vector2.zero; }
            GUI.Label(new Rect(20, height - 47, width - 40, 42), busy ? "正在处理所选图片…" : importStatus, label);
            GUI.enabled = enabled;
        }

        private void DrawImportPopup(float width, float height)
        {
            Rect popup = ImportPopupRect(width, height);
            // Snapshot the popup mode so one click cannot activate another view.
            ImportPopup stage = importPopup;
            GUI.Box(popup, "");
            string title = stage == ImportPopup.Browse ? "选择模组图片来源" : "加入“" + importCandidate?.Name + "” → 选择来源与阵营";
            GUI.Label(new Rect(popup.x + 12, popup.y + 10, popup.width - 110, 45), title, label);
            if (GUI.Button(new Rect(popup.xMax - 87, popup.y + 10, 75, 28), "取消", button)) CancelImportPopup();
            float contentY = popup.y + 61;
            if (stage == ImportPopup.Destination)
            {
                float left = popup.x + 14, leftWidth = (popup.width - 48) * .53f;
                float right = left + leftWidth + 20, rightWidth = popup.xMax - right - 14;
                GUI.Label(new Rect(left, contentY, leftWidth, 25), "目标自定义来源（可选空目录）", label);
                GUI.Label(new Rect(right, contentY, rightWidth, 25), "目标阵营目录", label);
                string next = GUI.TextField(new Rect(left, contentY + 30, leftWidth, 28), importPopupSearch);
                if (next != importPopupSearch) { importPopupSearch = next; importPopupScroll = Vector2.zero; }
                importNewName = GUI.TextField(new Rect(left, contentY + 68, Math.Max(40, leftWidth - 142), 28), importNewName);
                bool enabled = GUI.enabled; GUI.enabled = enabled && queuedCreate == null && !string.IsNullOrWhiteSpace(importNewName);
                if (GUI.Button(new Rect(left + leftWidth - 132, contentY + 68, 132, 28), "新建并选择", button)) queuedCreate = importNewName;
                GUI.enabled = enabled;
                var targets = importTargets.Where(n => n.IndexOf(importPopupSearch, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                float listY = contentY + 106, listHeight = Math.Max(35, popup.yMax - listY - 70);
                importPopupScroll = GUI.BeginScrollView(new Rect(left, listY, leftWidth, listHeight), importPopupScroll,
                    new Rect(0, 0, leftWidth - 22, Math.Max(35, targets.Count * 37)));
                for (int i = 0; i < targets.Count; i++)
                    if (GUI.Button(new Rect(0, i * 37, leftWidth - 26, 32), (importTarget == targets[i] ? "▶ " : "") + targets[i], button)) importTarget = targets[i];
                if (targets.Count == 0) GUI.Label(new Rect(5, 0, leftWidth - 32, 62), "没有匹配的目标来源，可在上方新建。", label);
                GUI.EndScrollView();
                for (int i = 0; i < PortraitImportRules.Categories.Length; i++)
                {
                    float colWidth = (rightWidth - 10) / 2;
                    string faction = PortraitImportRules.Categories[i];
                    if (GUI.Button(new Rect(right + i % 2 * (colWidth + 10), contentY + 30 + i / 2 * 48, colWidth, 38),
                        (importFaction == faction ? "▶ " : "") + faction, button)) importFaction = faction;
                }
                GUI.Label(new Rect(right, contentY + 184, rightWidth, 90), "同时选好来源和阵营后确认。会记住上次选择，方便连续加入多张图片。", label);
                GUI.Label(new Rect(left, popup.yMax - 57, popup.width - 200, 48), "加入到：" +
                    (importTarget == "" ? "尚未选择来源" : importTarget + " / " + importFaction), label);
                GUI.enabled = enabled && queuedImport == null && queuedCreate == null && importCandidate != null && importTargets.Contains(importTarget);
                if (GUI.Button(new Rect(popup.xMax - 170, popup.yMax - 50, 154, 35), "确认加入", button))
                {
                    queuedImport = importCandidate; queuedTarget = importTarget; queuedFaction = importFaction;
                    CancelImportPopup();
                }
                GUI.enabled = enabled;
            }
            else
            {
                string next = GUI.TextField(new Rect(popup.x + 14, contentY, popup.width - 28, 28), importPopupSearch);
                if (next != importPopupSearch) { importPopupSearch = next; importPopupScroll = Vector2.zero; }
                contentY += 38;
                var choices = importGroups.Where(g => g.Name.IndexOf(importPopupSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(g => new KeyValuePair<string, string>(g.Name, g.Name + $"  （角色 {g.PortraitCount} / 全部 {g.Images.Count}）")).ToList();
                float available = Math.Max(30, popup.yMax - contentY - 18);
                importPopupScroll = GUI.BeginScrollView(new Rect(popup.x + 10, contentY, popup.width - 20, available), importPopupScroll,
                    new Rect(0, 0, popup.width - 44, Math.Max(35, choices.Count * 37)));
                for (int i = 0; i < choices.Count; i++)
                {
                    if (!GUI.Button(new Rect(0, i * 37, popup.width - 48, 32), choices[i].Value, button)) continue;
                    importGroup = choices[i].Key; FilterImportImages(); CancelImportPopup();
                    break;
                }
                if (choices.Count == 0) GUI.Label(new Rect(8, 0, popup.width - 64, 34), "没有匹配的来源。", label);
                GUI.EndScrollView();
            }
            // The entire popup owns the current pointer/scroll event, even blank space.
            if (Event.current.isMouse || Event.current.type == EventType.ScrollWheel || Event.current.type == EventType.ContextClick) Event.current.Use();
        }
    }
}
