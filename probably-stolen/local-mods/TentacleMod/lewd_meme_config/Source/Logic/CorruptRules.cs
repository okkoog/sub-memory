using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 侵蚀触手的纯判定逻辑(Task 8):充能钳制与白/黑名单关键词匹配,零游戏类型
    /// 依赖,供测试工程通配链接与 <see cref="CorruptPatch"/> 复用。语义对照主 mod
    /// v1.3.8 <c>MidnightPawn.Game.CorruptTentacle</c>:
    ///  充能钳制 = 原版 <c>Mathf.Clamp(v, 0, MAX_CHARGE)</c>(GetCharge/SetCharge
    ///  及其裸字面量 100);关键词匹配 = IsCorruptible 的 identifier 子串(原版先
    ///  ToLowerInvariant 再 Contains,等价大小写不敏感子串)与 name 含关键词
    ///  (原版直接 Contains,大小写敏感)。全默认关键词(csvBase 常量在 patch 侧)
    ///  时判定结果与原版逐项一致 = 验收红线。
    /// </summary>
    public static class CorruptRules
    {
        /// <summary>充能钳制到 [0, max],语义 = Mathf.Clamp(charge, 0, max):
        /// &lt;0 → 0;&gt;max → max;界内原样(max 为配置值,负 max 时与 Mathf 同样
        /// 得负值,不额外发明下限保护)。</summary>
        public static int ClampCharge(int charge, int max)
        {
            if (charge < 0) return 0;
            if (charge > max) return max;
            return charge;
        }

        /// <summary>
        /// 两段 CSV 合并:各自按逗号切分、逐项 Trim、去空,再去重保序
        /// (base 在前,extra 追加;重复项保留首次出现)。null 输入视为空串。
        /// </summary>
        public static IReadOnlyList<string> MergeKeys(string csvBase, string csvExtra)
        {
            var merged = new List<string>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            AddCsv(merged, seen, csvBase);
            AddCsv(merged, seen, csvExtra);
            return merged;
        }

        private static void AddCsv(List<string> merged, HashSet<string> seen, string csv)
        {
            if (string.IsNullOrEmpty(csv)) return;
            foreach (string raw in csv.Split(','))
            {
                string key = raw.Trim();
                if (key.Length == 0) continue;
                if (seen.Add(key)) merged.Add(key);
            }
        }

        /// <summary>
        /// 任一 key 命中即真:key 为 identifier 子串(不区分大小写,等价原版
        /// ToLowerInvariant 后 Contains)或 displayName 含 key(序数、大小写敏感,
        /// 等价原版 name.Contains)。identifier/displayName 为 null 视为空串。
        /// </summary>
        public static bool Matches(string identifier, string displayName, IEnumerable<string> keys)
        {
            if (keys == null) return false;
            string id = identifier ?? "";
            string name = displayName ?? "";
            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(key)) continue;
                if (id.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                if (name.IndexOf(key, StringComparison.Ordinal) >= 0) return true;
            }
            return false;
        }

        /// <summary>
        /// 白名单判定 = MergeKeys(csvBase, csvExtra) 后 Matches。与 Blacklisted
        /// 同构(区别只在调用方给的 csvBase 常量与配置键)。
        /// </summary>
        public static bool Whitelisted(string id, string name, string csvBase, string csvExtra)
        {
            return Matches(id, name, MergeKeys(csvBase, csvExtra));
        }

        /// <summary>黑名单判定 = MergeKeys(csvBase, csvExtra) 后 Matches。</summary>
        public static bool Blacklisted(string id, string name, string csvBase, string csvExtra)
        {
            return Matches(id, name, MergeKeys(csvBase, csvExtra));
        }

        /// <summary>
        /// 外部侵蚀规则透传判定(0.7.0 Task 5,对照主 mod 1.9.2 IsCorruptible:
        /// 打印机精确 id 白名单之后、crate 关键词白名单之前插
        /// <c>TentacleApi.FindCorruptRule(it) != null → true</c>)。次序事实:
        /// 黑名单判定在外部规则之前(黑名单追加项压过外部 mod 注册的侵蚀规则),
        /// 关键词白名单殿后。findCorruptRule 为复刻体装配期经 ModReflect 解析的
        /// 委托(<c>Func&lt;GameItem, object&gt;</c>,协变绑定返回 CorruptRule;
        /// 测试侧以 mock 委托钉本纯逻辑半边);null 委托或 null 返回均视为
        /// 未命中——委托缺席(主 mod 版本漂移防御)时退化为不透传,原判定链
        /// 其余环节不受影响。
        /// </summary>
        public static bool ExternalRuleHits<T>(Func<T, object> findCorruptRule, T target)
        {
            return findCorruptRule != null && findCorruptRule(target) != null;
        }
    }
}
