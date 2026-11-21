using System;

namespace LewdMemeConfig
{
    /// <summary>
    /// 值直测(Tier2 SpawnItem/InvokeStatic)的期望模式(R6-F2,0.6.0 验收):
    /// 主 mod 侧键在 remake/Brood/IPH 在场时单元让位(Router 裁 Yield → 不装),
    /// 物品/静态直调回退**原版字面量**是设计态的正确行为——自检器生成期望前
    /// 必须先看该键所属单元的路由快照,把「让位回退到原版」变成正向断言,
    /// 而不是按「开关开 → 改值生效」硬套(0x08 六项假 FAIL 的根因)。
    /// 分派规则(任务 R6 定稿):
    ///  1. 任一所属单元 Dead → Skip:装配失败的暴露面在 Tier1 路由审计
    ///     (开关开却 Dead = 审计 FAIL),值直测对 Dead 无确定期望,不双重记账;
    ///  2. 任一所属单元 VersionBlocked → Skip:版本压制面板已注明
    ///     (「***版本不匹配,该项不生效」),值直测同样无确定期望;
    ///  3. 任一所属单元 Own → Own(现状断言:开关开 → 改值生效);
    ///  4. 其余(全部 Yield)→ Yield:断言原版字面量,detail 注明让位 owner。
    /// Dead/VersionBlocked 先于 Own:一键多单元混合 Own+Dead 的组合在目录中
    /// 不存在(键的单元归属不跨族),该序只为让异常态优先显式跳过。
    /// 纯函数(只依赖 RouteDecision):SelfTestRunner 组装 UnitRoute 后调用;
    /// EffectDispatchTests 锁分派表。参照 SwitchDispatch 先例(F1 抽出同形)。
    /// </summary>
    internal enum EffectExpect
    {
        /// <summary>有 Own 单元 → 断言改值生效(开关开 → 热对账装上)。</summary>
        Own,
        /// <summary>全部单元 Yield → 断言原版字面量(让位回退是正确行为)。</summary>
        Yield,
        /// <summary>Dead/VersionBlocked → 跳过(原因入 note;不算失败)。</summary>
        Skip,
    }

    /// <summary>分派纯函数(Logic/:零游戏类型,测试工程通配编译)。</summary>
    internal static class EffectDispatch
    {
        /// <summary>单个所属单元的路由态(Id 仅入 note 文案,不参与判定)。</summary>
        internal readonly struct UnitRoute
        {
            public readonly string Id;
            public readonly RouteDecision Route;
            public readonly bool Dead;
            internal UnitRoute(string id, RouteDecision route, bool dead)
            {
                Id = id; Route = route; Dead = dead;
            }
        }

        /// <summary>
        /// 键的期望模式。units = 该键所属的全部 MainHook/SubModHook 单元快照;
        /// 空数组(键无 hook 单元,如纯 DataWrite/WriteThrough 键)→ Own 维持
        /// 现状断言(这类键不经 WithSetup 脚手架,到达即防御性兜底)。
        /// note:Skip/Yield 的原因文案(调用方拼进 detail)。
        /// </summary>
        internal static EffectExpect Decide(UnitRoute[] units, out string note)
        {
            note = null;
            if (units == null || units.Length == 0) return EffectExpect.Own;
            bool anyYield = false;
            foreach (UnitRoute u in units)
            {
                if (u.Dead)
                {
                    note = "单元 " + u.Id + " 装配失败(Dead)";
                    return EffectExpect.Skip;
                }
                if (u.Route == RouteDecision.VersionBlocked)
                {
                    note = "单元 " + u.Id + " 版本压制(面板已注明)";
                    return EffectExpect.Skip;
                }
                if (u.Route == RouteDecision.Own) return EffectExpect.Own;
                anyYield = true;
            }
            // 到此 units 全为 Yield(anyYield 必真;防御性双检)
            return anyYield ? EffectExpect.Yield : EffectExpect.Own;
        }
    }
}
