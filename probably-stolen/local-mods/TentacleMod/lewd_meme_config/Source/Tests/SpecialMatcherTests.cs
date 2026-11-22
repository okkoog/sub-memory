using System;
using LewdMemeConfig;

// Task 12:SpecialMatcher 纯函数语义(special.union 前缀的判定核心)。
// 语义对照 brief:追加 id 精确(OrdinalIgnoreCase,对齐主 mod MinorIds 集合的
// 比较器)/ 立绘子串(token 是 spriteName 的子串,不区分大小写,对齐主 mod
// IsSpecialSprite 的 lower-Contains 方向:主 mod 双向模糊,本追加按 brief 只取
// "立绘子串"单向;CSV 切分复用 CorruptRules.MergeKeys)/ 名字关键词子串
// (displayName.Contains,序数大小写敏感,对齐主 mod IsSpecialName 的
// NameKeywords 循环)。三路 CSV 全空 → 恒 false(默认零漂移)。
// SpecialAndManualPatches 依赖 Harmony 与主 mod 类型(ModReflect),住在
// Patches/ 不进测试工程;接管语义由本纯函数测试 + Task 14 游戏内验收共同覆盖。
public static class SpecialMatcherTests
{
    public static void Run()
    {
        // -- brief Step 1 真值表 --
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("my_npc", "", "", "my_npc", "", ""),
            "extra id exact (brief)");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("", "heroine1", "", "", "heroine", ""),
            "extra sprite substring (brief)");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("", "", "卢的储物柜", "", "", "卢"),
            "extra keyword in displayName (brief)");
        // 全空 csv 一律 false(identifier/sprite/display 任一非空也不追加)
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("stationEngineerMinor", "jackson", "卢", "", "", ""),
            "all-empty csvs -> false");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("", "", "", "", "", ""),
            "everything empty -> false");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial(null, null, null, "", "", ""),
            "null fields + empty csvs -> false");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("my_npc", "", "", null, null, null),
            "null csvs -> false");

        // -- id 精确(非子串),大小写不敏感 --
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("my_npc_extra", "", "", "my_npc", "", ""),
            "id is exact not substring");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("xxmy_npcxx", "", "", "my_npc", "", ""),
            "id no fuzzy mid-hit");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("MY_NPC", "", "", "my_npc", "", ""),
            "id case-insensitive (主 mod MinorIds 为 OrdinalIgnoreCase)");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("npc2", "", "", "npc1,npc2", "", ""),
            "id second csv entry hits");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("my_npc", "", "", " my_npc ,, ", "", ""),
            "id csv trims + drops empties (MergeKeys 语义)");

        // -- 立绘子串单向(token ⊂ spriteName),不区分大小写 --
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("", "HEROINE1", "", "", "heroine", ""),
            "sprite case-insensitive");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("", "h", "", "", "heroine", ""),
            "sprite directional: token must be inside spriteName");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("heroine", "", "", "", "", ""),
            "sprite csv never tested against identifier");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("", "special_8", "", "", "special_1,special_8", ""),
            "sprite second csv entry hits");

        // -- 名字关键词:displayName.Contains,序数大小写敏感 --
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("", "ABC", "", "", "", "abc"),
            "keyword ordinal case-sensitive (主 mod NameKeywords 直接 Contains)");
        Check.Eq(true, SpecialMatcher.IsExtraSpecial("", "", "治安部物证柜", "", "", "物证柜"),
            "keyword substring mid-name");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("", "", "", "", "", "卢"),
            "keyword never tested against null displayName");
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("卢", "", "", "", "", "卢"),
            "keyword never tested against identifier");

        // -- 三路互不串扰 --
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("heroine", "heroine", "heroine", "npc", "sprite", "kw"),
            "no cross-field hit");

        // -- 默认配置(空 CSV)在 special.union 前缀中的净效果:
        //    IsExtraSpecial 恒 false → 恒放行原版 IsAny = 零漂移 --
        Check.Eq(false, SpecialMatcher.IsExtraSpecial("stationEngineerMinor", "wantedposter", "塞尔皮科", "", "", ""),
            "main-mod whitelisted client + empty extras -> false (union falls through)");
    }
}
