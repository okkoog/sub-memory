using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 池类键即时回滚路由(0.5.0 开关体系,spec §4.2 修订):子项开关键 Changed →
    /// 该池重写。配合消费方(MaidenPatch.OnConfigChanged)语义:开关关 →
    /// Current 已压回默认 CSV → 池重写即写回默认数组——「关闭=纯原版」对 4 个
    /// 装配期写进主 mod 静态字段(MaidenTraits.四池)的键
    /// 无例外洞。零游戏类型依赖,测试工程通配编译覆盖。
    /// (总/分区开关已按作者裁定移除——游戏面板 Apply 模型无法实现层级联动。
    /// 0.7.0 T9:EXP_NEED 池移除——1.9.2 已删 TentacleBody.EXP_NEED 静态数组,
    /// 经验公式化进 ExpNeedForLevel,progress.exp_per_level 键全链废弃。)
    /// </summary>
    public static class PoolRouting
    {
        /// <summary>全部池键(四池)。</summary>
        public static readonly IReadOnlyList<string> PoolKeys = new[]
        {
            CfgKeys.Factions, CfgKeys.Jobs, CfgKeys.Bodies, CfgKeys.Breasts,
        };

        private static readonly Dictionary<string, string> itemSwitchToPool = BuildItemSwitchMap();

        private static Dictionary<string, string> BuildItemSwitchMap()
        {
            var map = new Dictionary<string, string>();
            foreach (var k in PoolKeys) map[PanelKeys.ItemSwitchKey(k)] = k;
            return map;
        }

        /// <summary>开关键(子项)→ 受影响池键;非开关键 → 空。
        /// (总/分区开关已按作者裁定移除——游戏面板 Apply 模型无法实现层级联动。)。</summary>
        public static IReadOnlyList<string> PoolsAffected(string changedKey)
        {
            if (itemSwitchToPool.TryGetValue(changedKey, out var pool)) return new[] { pool };
            return new string[0];
        }
    }
}
