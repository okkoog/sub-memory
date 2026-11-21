using System.Collections.Generic;
using LewdMemeConfig;

// Task 5(开关体系,作者裁定修订:仅子项开关):池类键即时回滚的路由表——
// 子项开关键 Changed → 该池。语义:开关关 → Current 已压回默认 CSV →
// 池重写即写回默认数组(「关闭=纯原版」对 4 个写进主 mod 静态字段的键
// 无例外洞,spec §4.2 作者裁定)。总/分区开关已移除(Apply 模型无层级联动)。
// 0.7.0 T9:EXP_NEED 池移除(1.9.2 已删 TentacleBody.EXP_NEED 静态数组,
// 经验公式化进 ExpNeedForLevel 三键;progress.exp_per_level 键全链废弃)。
public static class PoolRoutingTests
{
    public static void Run()
    {
        // 池键自身的子项开关 → 该池
        var one = PoolRouting.PoolsAffected(CfgKeys.Bodies + ".enabled");
        Check.Eq(1, one.Count, "item switch affects its pool");
        Check.Eq(CfgKeys.Bodies, one[0], "item switch is BODIES");
        Check.Eq(0, PoolRouting.PoolsAffected("progress.exp_per_level.enabled").Count,
            "T9: exp_per_level 开关不再路由任何池(键废弃)");

        // 无关键:普通值键、值键本体 → 空
        Check.Eq(0, PoolRouting.PoolsAffected("price.milk").Count, "plain value key affects nothing");
        Check.Eq(0, PoolRouting.PoolsAffected("ui.enabled").Count, "unregistered switch-like key affects nothing");
        Check.Eq(0, PoolRouting.PoolsAffected(CfgKeys.Bodies).Count, "pool value key itself routes nothing (only its switch)");
        Check.Eq(0, PoolRouting.PoolsAffected("nightly.sex_min_base").Count, "nightly value key affects nothing");
    }

    private static bool Contains(IReadOnlyList<string> list, string key)
    {
        foreach (var k in list) if (k == key) return true;
        return false;
    }
}
