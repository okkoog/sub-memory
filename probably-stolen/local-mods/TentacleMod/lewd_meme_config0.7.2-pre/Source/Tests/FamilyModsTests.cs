using LewdMemeConfig;

// Task 5(M3a):FamilyMods 绑定版本表(纯)——8 个家族 mod 的精确版本绑定与匹配语义。
// 语义契约:VersionMatches = 精确字符串相等(不做语义化比较),actual 为 null/空 → false。
// Family.Probe 依赖 ModLoader(游戏侧通道),不可单测,由游戏内验收覆盖。
public static class FamilyModsTests
{
    public static void Run()
    {
        // 8 个 ID 常量声明序(BoundVersions 键集合的期望 = 恰这 8 个)。
        string[] ids =
        {
            FamilyMods.MainMod, FamilyMods.Brood, FamilyMods.Remake, FamilyMods.Brothel,
            FamilyMods.Tearl, FamilyMods.Legacy, FamilyMods.Crisp, FamilyMods.Iph,
        };

        // ① 8 个绑定版本逐一与自身精确匹配。
        foreach (string id in ids)
            Check.Eq(true, FamilyMods.VersionMatches(id, FamilyMods.BoundVersions[id]),
                id + " bound version matches itself");

        // ② "9.9.9" 一律不匹配(精确相等,无语义化比较)。
        foreach (string id in ids)
            Check.Eq(false, FamilyMods.VersionMatches(id, "9.9.9"),
                id + " wrong version rejected");

        // ③ null 一律不匹配。
        foreach (string id in ids)
            Check.Eq(false, FamilyMods.VersionMatches(id, null),
                id + " null version rejected");

        // 空串不匹配(绑定约束:null/empty actual → false;manifest ModVersion
        // 经加载器 Trim 后可能为空串,同样视为不命中)。
        foreach (string id in ids)
            Check.Eq(false, FamilyMods.VersionMatches(id, ""),
                id + " empty version rejected");

        // ④ 表恰 8 项且键与 8 个 ID 常量一致(字典键天然无重复;Count==8
        // 叠加逐键 ContainsKey ⇒ 恰好这 8 项、无多余键)。
        Check.Eq(8, FamilyMods.BoundVersions.Count, "bound table has exactly 8 entries");
        foreach (string id in ids)
            Check.Eq(true, FamilyMods.BoundVersions.ContainsKey(id),
                id + " present in bound table");

        // 防漂移:lewd_meme 行与 VersionGuard 的主 mod 版本常量刻意同值
        // (前者服务家族钩子按需装配,后者服务既有主 mod 判定),钉住两者。
        Check.Eq(VersionGuard.SupportedMainModVersion,
            FamilyMods.BoundVersions[FamilyMods.MainMod],
            "main mod row pinned to VersionGuard");

        // ⑤ R5 防再犯加固:①~③ 用 BoundVersions[id] 自匹配,字典值错了照样绿
        //    (上波 bump 只改注释未改字典值即此假绿漏网)。此处加显式期望表
        //    逐行对照——bump 版本时<b>字典值与本表两处同改</b>,任一处漏改即红。
        string[] expected =
        {
            "1.9.2", "0.3.31", "1.6.4", "0.10.4",
            "7", "1.0.6", "1.0.0", "1.3.0",
        };
        for (int i = 0; i < ids.Length; i++)
            Check.Eq(expected[i], FamilyMods.BoundVersions[ids[i]],
                ids[i] + " bound version pinned to " + expected[i]);

        // 形态断言:非空且纯「数字段(.数字段)*」(tearl manifest 版本为单段
        // "7",故不强制两段以上;拒绝空串/placeholder/非数字垃圾)。
        foreach (string id in ids)
            Check.Eq(true, VersionShapeOk(FamilyMods.BoundVersions[id]),
                id + " bound version shape ok");

        // ⑥ R13b:BoundVersionsSummary = manifest Description 手抄串的事实源
        //    (显示名/顺序/分隔符钉死;bump 版本时 manifest 两处同改)。
        //    注意段序 = 任务定稿的玩家向顺序(remake 在 Brood 前,与上方 ids
        //    声明序不同)。
        string[] summaryOrder =
        {
            FamilyMods.MainMod, FamilyMods.Remake, FamilyMods.Brood, FamilyMods.Brothel,
            FamilyMods.Tearl, FamilyMods.Legacy, FamilyMods.Crisp, FamilyMods.Iph,
        };
        string[] displayOrder =
        {
            "lewd_meme", "暗流交易", "巢穴扩展", "掌上青楼",
            "tearlaments", "旧内容恢复", "触手bug修复", "物品高清立绘",
        };
        string summary = FamilyMods.BoundVersionsSummary();
        Check.Eq(true, summary.StartsWith("绑定版本:", System.StringComparison.Ordinal),
            "summary 以「绑定版本:」起头");
        string[] parts = summary.Substring("绑定版本:".Length)
            .Split(new[] { " / " }, System.StringSplitOptions.None);
        Check.Eq(displayOrder.Length, parts.Length, "summary 恰 8 段(每 mod 一段)");
        for (int i = 0; i < displayOrder.Length && i < parts.Length; i++)
            Check.Eq(displayOrder[i] + " " + FamilyMods.BoundVersions[summaryOrder[i]], parts[i],
                "summary 段 " + i + " = 显示名 + 绑定版本(" + displayOrder[i] + ")");
    }

    /// <summary>绑定版本形态检查:非空、只含数字与点、至少一个数字、
    /// 不以点开头/结尾、无连续点(配合 ⑤ 形态断言)。</summary>
    private static bool VersionShapeOk(string v)
    {
        if (string.IsNullOrEmpty(v)) return false;
        bool digit = false;
        foreach (char c in v)
        {
            if (c >= '0' && c <= '9') { digit = true; continue; }
            if (c != '.') return false;
        }
        return digit
            && v[0] != '.' && v[v.Length - 1] != '.'
            && v.IndexOf("..", System.StringComparison.Ordinal) < 0;
    }
}
