using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 掉落的纯判定逻辑(Task 11 建,Task 8 对齐 1.6.1,Task 14b 基线声明更新):
    /// 医疗组关键词 / 拾荒组精确匹配 / 拾荒节流窗口,零游戏类型依赖,
    /// 供测试工程通配链接与 <see cref="LootPatch"/> 复用。
    /// 语义对照主 mod <c>MidnightPawn.Game.LootHook.OnLootRolled</c>——
    /// Task 14b 起双基线:IsMedGroup 服务 161 恢复分支(医疗组排卵针掉落已于
    /// 1.7.0 从原版删除,lewd_meme_161 L36-44;med 双键 = opt-in 恢复 1.6.x 行为,
    /// 关键词/概率默认 = 1.6.1 字面量);InScavGroups/Throttled 对齐 171
    /// (lewd_meme_171 L30-54,与 161 L30-70 的拾荒/节流段零漂移):
    ///  医疗组 = 原版 <c>text = groupId == null ? "" : groupId.ToLower()</c> 后
    ///  <c>text.Contains(kw)</c> 任一命中(小写化只作用于 groupId,关键词按原样
    ///  比较——原版关键词即小写字面量;空/null 关键词跳过,防 "" 恒真);
    ///  拾荒组 = 原版 <c>groupId == "dumpingGroundTG" || groupId == "dumpingGroundFreshTG"</c>
    ///  (raw groupId 精确相等,大小写敏感、非子串,null 不命中);
    ///  节流 = 171 L37-42 <c>ctx.groupId == lastGroupId &amp;&amp; time - lastRollTime &lt; 0.5f</c>
    ///  → 跳过判定(同组 0.5s 内不重复掷骰);少女概率为恒定读数
    ///  (ModSettings.MaidenChance,1.3.13/14 已移除 per-level 加成与 8% 拾荒
    ///  排卵针分支,概率计算不再入本类)。全默认(默认 CSV/默认概率)时判定
    ///  与原版逐项一致 = 验收红线。
    /// </summary>
    public static class LootRules
    {
        /// <summary>
        /// 医疗组判定:groupId 小写化(原版 ToLower,null → "")后含任一关键词
        /// (序数 Contains,按列表次序短路,等价原版 || 链)。keywords 为 null
        /// 或空 → 恒 false;null/空关键词项跳过。
        /// </summary>
        public static bool IsMedGroup(string groupId, IEnumerable<string> keywords)
        {
            if (keywords == null) return false;
            string text = ((groupId == null) ? "" : groupId.ToLower());
            foreach (string keyword in keywords)
            {
                if (string.IsNullOrEmpty(keyword)) continue;
                if (text.Contains(keyword)) return true;
            }
            return false;
        }

        /// <summary>
        /// 拾荒组判定:raw groupId 与任一组名精确相等(序数 ==,大小写敏感,
        /// 等价原版对 "dumpingGroundTG"/"dumpingGroundFreshTG" 的 == 比较;
        /// 不做小写化/子串匹配)。groupIds 为 null 或空 → 恒 false;
        /// groupId 为 null 不命中任何项;null/空组名项跳过。
        /// </summary>
        public static bool InScavGroups(string groupId, IEnumerable<string> groupIds)
        {
            if (groupIds == null || groupId == null) return false;
            foreach (string g in groupIds)
            {
                if (string.IsNullOrEmpty(g)) continue;
                if (groupId == g) return true;
            }
            return false;
        }

        /// <summary>
        /// 拾荒节流判定:同组(raw 序数相等,等价原版 <c>ctx.groupId == lastGroupId</c>)
        /// 且距上次判定不足 window 秒 → true(调用方跳过本次判定,零 RNG 消耗;
        /// 未节流时调用方负责更新 lastGroupId/lastRollTime)。
        /// 原版 L51-55:<c>ctx.groupId == lastGroupId &amp;&amp; time - lastRollTime
        /// &lt; 0.5f</c>;首次(lastGroupId 为 null)与异组恒 false;窗口边界严格
        /// 小于(差恰等于 window 不节流,同原版 &lt;)。
        /// </summary>
        public static bool Throttled(string groupId, string lastGroupId, float now, float lastRollTime, float window)
        {
            return groupId == lastGroupId && now - lastRollTime < window;
        }
    }
}
