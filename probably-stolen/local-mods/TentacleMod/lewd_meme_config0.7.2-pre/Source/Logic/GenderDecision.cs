using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 档 3(全女性)的性别判定纯逻辑:零游戏类型依赖(gender 用 int 表达,
    /// 0 Neutral / 1 Male / 2 Female,对应游戏 LocGenderHelper.LocGender 枚举),
    /// 供测试工程通配链接与 GameGenderPatches 复用。
    /// </summary>
    public static class GenderDecision
    {
        /// <summary>
        /// 游戏本体 LocGenderHelper.uniqueSpriteGender 的全部 48 键,逐字抄自
        /// decomp/game/LocGenderHelper.cs(顺序与反编译源一致),含 3 个 Neutral
        /// 键 billy / sha3 / cul1——它们是"具名中性 NPC"改判女性的判定输入。
        /// </summary>
        public static readonly string[] HumanSprites =
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

        private static readonly HashSet<string> humanSprites = new HashSet<string>(HumanSprites);

        /// <summary>
        /// 档 3 之下该角色是否按女性对待:
        /// scope != 3 → 恒 false(本判定只在档 3 生效);
        /// originalGender == 1(Male)→ true(与名字无关);
        /// originalGender == 0(Neutral)且 spriteName 在 HumanSprites(具名 NPC)→ true;
        /// 其余(含 originalGender == 2 Female、Neutral 未具名、空串/null)→ false。
        /// </summary>
        public static bool TreatAsFemale(int originalGender, string spriteName, int scope)
        {
            if (scope != 3) return false;
            if (originalGender == 1) return true;
            return originalGender == 0 && humanSprites.Contains(spriteName);
        }
    }
}
