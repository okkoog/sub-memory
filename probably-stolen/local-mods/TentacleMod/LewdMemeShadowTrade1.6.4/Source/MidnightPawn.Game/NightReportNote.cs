using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace MidnightPawn.Game;

/// <summary>当晚报告保存为独立物品；只有玩家阅读时才创建原生纸书。</summary>
internal static class NightReportNote
{
    internal const string ItemId = "night_report_note_remake";
    private const string ModId = "lewd_meme_remake";
    private const string BookId = "night_report_note_reader";
    private const string DayTag = "REMAKE_REPORT_DAY";
    private const string BodyTag = "REMAKE_REPORT_BODY";
    private const int CharactersPerLine = 20;
    private const int LinesPerPage = 13;
    private const float DesignW = 314f;
    private const float DesignH = 402f;

    internal static GameItem CreateItem()
    {
        GameItem item = ItemDirectory.CreateEmptyItem(ItemId);
        Configure(item);
        item.onLoaded = Configure;
        return item;
    }

    private static string Title(int day) => "第" + day + "天夜间报告【暗流交易】";

    private static void Configure(GameItem item)
    {
        if (item == null) return;
        item.name = Title(ItemFactory.GetInt(item, DayTag));
        item.shortDescription = "记录当晚结算的手写便签。点击阅读，可翻页。";
        item.unitValue = 0L;
        item.SetGameItemType("DOCUMENT");
        item.SetSprite("Items/items_misc", "handnote");
        item.SetShape(ItemFactory.RectShape(2, 2));
        item.EnableTag("DOCUMENT", false);
        item.EnableTag("NOT_FOR_RESALE", false);
        item.mayStackItemFunc = (a, b) => false;
        item.canActivateSlotItemFunc = (it, inv, slot) => true;
        item.mayActivateSlotItemFunc = (it, inv, slot) => true;
        // 使用回调收到的实际物品，避免克隆或读档后读到另一张便签。
        item.onActivateSlotItemFunc = (it, inv, slot) => Open(it);
    }

    internal static void Create(int day, string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return;
        GameItem item = null;
        try
        {
            item = DirectoryMaster.Item(ItemId, true);
            if (item == null) throw new InvalidOperationException("报告便签尚未注册");
            ItemFactory.SetInt(item, DayTag, day);
            ItemFactory.SetStr(item, BodyTag, body);
            Configure(item);
            ModHelper.SetItemOwnerShip(item, true);
            if (TryPlace(item, out string where))
            {
                ModCtx.Log(Title(day) + "已放入" + where);
                return;
            }
            item.Destroy();
            ModCtx.LogWarning("夜间报告便签：柜台、仓库和垃圾桶都放不下，本次报告已放弃。");
        }
        catch (Exception ex)
        {
            if (item != null && item.parentInventory == null) item.Destroy();
            ModCtx.LogWarning("夜间报告便签生成失败，本次不重试: " + ex.Message);
        }
    }

    internal static bool TryPlace(GameItem item, out string where)
    {
        // 通用放置只尝试己方柜台和仓库；报告额外以垃圾桶兜底。
        if (ItemFactory.TryPlaceSafely(item, out where)) return true;
        GameGridInventory trash = EmporiumEntry.Instance?.trashcanInvElement;
        if (trash == null || !GraphUtils.CanAccept(trash, item)) return false;
        int remaining = GraphUtils.TryAcceptAll(trash, item, -1);
        if (remaining != 0 || item.parentInventory == null) return false;
        where = "垃圾桶";
        return true;
    }

    private static void Open(GameItem item)
    {
        if (item == null) return;
        try
        {
            PaperChromeIcons.EnsureInstalled();
            PaperBookRegistry.Register(ModId, BuildBook(ItemFactory.GetInt(item, DayTag), ItemFactory.GetStr(item, BodyTag)));
            // 单独的书籍 ID 与名册分离，反复打开或切换日期只更新同一个阅读器。
            PaperBookRegistry.Open(ModId + ":" + BookId)?.FlipTo(0);
        }
        catch (Exception ex)
        {
            ModCtx.LogWarning("打开夜间报告便签失败: " + ex.Message);
        }
    }

    internal static PaperBookData BuildBook(int day, string body)
    {
        PaperBookData data = null;
        try
        {
            data = PaperStarters.NewBook("blank", BookId, Title(day));
        }
        catch (Exception ex)
        {
            ModCtx.LogWarning("夜间报告纸书模板读取失败，使用默认纸面: " + ex.Message);
        }
        if (data == null) data = new PaperBookData { background = "notebook_manual" };
        data.id = BookId;
        data.title = Title(day);
        data.designWidth = DesignW;
        data.designHeight = DesignH;
        data.chrome = PaperStarters.DefaultChrome(new Vector2(DesignW, DesignH), 2f);
        data.item = null;
        data.pages = new List<PaperPageData>();
        List<List<string>> pages = Paginate(body);
        for (int p = 0; p < pages.Count; p++)
        {
            PaperPageData page = new PaperPageData { name = "第" + (p + 1) + "页" };
            page.elements.Add(Text(18, 26, 278, 28, Title(day), 14f, "title"));
            for (int row = 0; row < pages[p].Count; row++)
                page.elements.Add(Text(22, 70 + row * 21, 270, 21, pages[p][row], 12f, "body"));
            data.pages.Add(page);
        }
        return data;
    }

    // 手动折行，每行独立布局，不让原生自动换行把正文挤进页脚/翻页按钮。
    // 中英文均按一字符格保守预留；按文本元素切分，避免拆开代理对与组合字符。
    internal static List<List<string>> Paginate(string body)
    {
        List<string> lines = new List<string>();
        string text = string.IsNullOrEmpty(body) ? "（报告内容为空）" : body;
        foreach (string raw in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
        {
            StringBuilder line = new StringBuilder();
            TextElementEnumerator elements = StringInfo.GetTextElementEnumerator(raw.Replace("\t", "    "));
            int count = 0;
            while (elements.MoveNext())
            {
                if (count == CharactersPerLine)
                {
                    lines.Add(line.ToString());
                    line.Clear();
                    count = 0;
                }
                line.Append(elements.GetTextElement());
                count++;
            }
            lines.Add(line.ToString());
        }
        List<List<string>> pages = new List<List<string>>();
        for (int start = 0; start < lines.Count; start += LinesPerPage)
            pages.Add(lines.GetRange(start, Math.Min(LinesPerPage, lines.Count - start)));
        return pages;
    }

    private static PaperElementData Text(float x, float y, float w, float h, string text, float size, string style)
    {
        return new PaperElementData
        {
            type = "text", x = x, y = y, w = w, h = h, text = text,
            fontSize = size, style = style, align = "left", valign = "top", wrap = false
        };
    }
}
