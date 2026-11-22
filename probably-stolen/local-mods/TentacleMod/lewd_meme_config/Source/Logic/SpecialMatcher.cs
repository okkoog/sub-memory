using System;

namespace LewdMemeConfig
{
    /// <summary>
    /// 特殊顾客追加判定的纯逻辑(Task 12):在主 mod v1.3.8
    /// <c>MidnightPawn.Game.SpecialClients</c> 三路白名单(id 集合 / 立绘集合+
    /// 双向模糊子串 / 名字精确+关键词)之上,为 special.extra_* 三个 CSV 键提供
    /// 并集判定。三路语义对照原版同类匹配:
    ///  追加 id = 精确相等(OrdinalIgnoreCase,对齐原版 MinorIds 的比较器,
    ///  非子串);追加立绘 = 子串(token 是 spriteName 的子串,不区分大小写,
    ///  原版 IsSpecialSprite 为 lower 后双向 Contains,本追加按 brief 只取
    /// "立绘子串"单向);追加关键词 = displayName 含 token(序数、大小写敏感,
    ///  对齐原版 IsSpecialName 的 NameKeywords 直接 Contains)。
    /// CSV 切分复用 <see cref="CorruptRules.MergeKeys"/>(逗号切分、逐项 Trim、
    /// 去空;base 传 null 即纯 extra)。三路 CSV 全空/null → 恒 false =
    /// special.union 前缀默认零漂移的验收红线。零游戏类型,Logic/ 通配进测试工程。
    /// </summary>
    public static class SpecialMatcher
    {
        /// <summary>
        /// 追加特殊顾客判定:extra id 精确命中 <paramref name="identifier"/>,
        /// 或 extra 立绘 token 是 <paramref name="spriteName"/> 的子串,或
        /// <paramref name="displayName"/> 含 extra 关键词。字段 null/空视为
        /// 该路不参与;三路 CSV 全空恒 false。
        /// </summary>
        public static bool IsExtraSpecial(string identifier, string spriteName, string displayName,
                                          string csvIds, string csvSprites, string csvKeywords)
        {
            if (!string.IsNullOrEmpty(identifier))
            {
                foreach (string id in CorruptRules.MergeKeys(null, csvIds))
                {
                    if (identifier.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(spriteName))
            {
                foreach (string sprite in CorruptRules.MergeKeys(null, csvSprites))
                {
                    if (spriteName.IndexOf(sprite, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(displayName))
            {
                foreach (string keyword in CorruptRules.MergeKeys(null, csvKeywords))
                {
                    if (displayName.IndexOf(keyword, StringComparison.Ordinal) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
