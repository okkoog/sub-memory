using System;
using System.Collections.Generic;
using LewdMemeConfig;

// Task 5:GenderDecision 纯函数语义 + HumanSprites 名单完整性。
// 48 键名单在测试里独立抄自 decomp/game/LocGenderHelper.cs 的 uniqueSpriteGender
// (与实现互为校对),排序后逐位比对——缺一/错拼/多一/重复都会失败。
// GameGenderPatches 依赖游戏/Harmony 类型,住在 Patches/ 不进测试工程,
// 其"仅档 3 生效"的短路语义由本真值表 + Task 14 游戏内验收共同覆盖。
public static class GenderDecisionTests
{
    public static void Run()
    {
        // -- 名单完整性:decomp 独立第二份抄录(顺序与反编译源一致) --
        string[] decompKeys =
        {
            "eleanor1", "eleanor2", "eleanor3", "eleanor_scav", "eleanor_rev",
            "mentor", "sawyer", "joe", "george", "serpico",
            "nellis", "grandpa_cheng", "landlord", "officer_lun", "merchant",
            "shadyMerchant", "admin", "greytide", "scavElder", "waffle_enjoyer",
            "case_enjoyer", "serpico_scav", "serpico_rev", "courrier", "inventor",
            "retired_farmer", "retired_chemist", "retired_aquafarmer", "cartel_handler", "blackmailer",
            "wanted1", "wanted2", "wanted3", "wanted4", "wanted5",
            "lou_sec", "wanted6", "wanted7", "wanted8", "wanted9",
            "wanted_bait1", "trailer_scav1", "trailer_scav4", "trailer_scav5", "trailer_rev1",
            "billy", "sha3", "cul1",
        };
        Check.Eq(48, decompKeys.Length, "decomp key count sanity");
        Check.Eq(48, GenderDecision.HumanSprites.Length, "HumanSprites count");
        var expected = new List<string>(decompKeys);
        var actual = new List<string>(GenderDecision.HumanSprites);
        expected.Sort(StringComparer.Ordinal);
        actual.Sort(StringComparer.Ordinal);
        for (int i = 0; i < Math.Min(expected.Count, actual.Count); i++)
            Check.Eq(expected[i], actual[i], "HumanSprites sorted[" + i + "]");
        // 3 个 Neutral 键必须在名单里(档 3 对具名中性 NPC 改判的关键输入)
        Check.Eq(true, Array.IndexOf(GenderDecision.HumanSprites, "billy") >= 0, "neutral billy listed");
        Check.Eq(true, Array.IndexOf(GenderDecision.HumanSprites, "sha3") >= 0, "neutral sha3 listed");
        Check.Eq(true, Array.IndexOf(GenderDecision.HumanSprites, "cul1") >= 0, "neutral cul1 listed");

        // -- TreatAsFemale 真值表(brief Step 1 逐条) --
        Check.Eq(true, GenderDecision.TreatAsFemale(1, "mentor", 3), "scope3 male -> true");
        Check.Eq(false, GenderDecision.TreatAsFemale(2, "eleanor1", 3), "scope3 female -> false");
        Check.Eq(true, GenderDecision.TreatAsFemale(0, "billy", 3), "scope3 neutral named -> true");
        Check.Eq(false, GenderDecision.TreatAsFemale(0, "crate_icon", 3), "scope3 neutral unnamed -> false");
        Check.Eq(false, GenderDecision.TreatAsFemale(0, "", 3), "scope3 empty sprite -> false");
        // original==1 的判定与名字无关(规格:original==1 → true,无条件)
        Check.Eq(true, GenderDecision.TreatAsFemale(1, "", 3), "scope3 male empty sprite -> true");
        Check.Eq(false, GenderDecision.TreatAsFemale(0, null, 3), "scope3 null sprite -> false");
        // 越界 gender 值不当作男性也不当作具名中性
        Check.Eq(false, GenderDecision.TreatAsFemale(3, "mentor", 3), "scope3 out-of-range gender -> false");
        Check.Eq(false, GenderDecision.TreatAsFemale(-1, "billy", 3), "scope3 negative gender -> false");
        // scope 0/1/2 一律 false
        Check.Eq(false, GenderDecision.TreatAsFemale(1, "mentor", 0), "scope0 male -> false");
        Check.Eq(false, GenderDecision.TreatAsFemale(1, "mentor", 1), "scope1 male -> false");
        Check.Eq(false, GenderDecision.TreatAsFemale(1, "mentor", 2), "scope2 male -> false");
        Check.Eq(false, GenderDecision.TreatAsFemale(0, "billy", 2), "scope2 neutral named -> false");
        Check.Eq(false, GenderDecision.TreatAsFemale(2, "eleanor1", 0), "scope0 female -> false");
    }
}
