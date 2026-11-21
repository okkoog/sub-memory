using LewdMemeConfig;

// R6-F2:值直测期望模式分派(EffectDispatch.Decide)——让位键断言原版回退的
// 正向化修复:主 mod 价格键在 remake/Brood 在场时单元让位(路由快照 Yield),
// 自检器生成期望前必须按所属单元裁决分派,而非按「开关开 → 改值生效」硬套。
// 本文件锁分派表全分支 + 优先级序(Dead/VersionBlocked 先于 Own)+ 防御兜底
// (空数组维持 Own)。键的单元归属由 HookCatalogTests/RouterTests 另行锁定,
// 此处用合成 UnitRoute 直测纯函数(与 RouteExpectTests 同形)。
public static class EffectDispatchTests
{
    public static void Run()
    {
        var own = new[] { new EffectDispatch.UnitRoute("main.level_cap", RouteDecision.Own, false) };
        var yield1 = new[] { new EffectDispatch.UnitRoute("main.price_milk", RouteDecision.Yield, false) };
        var yield2 = new[]
        {
            new EffectDispatch.UnitRoute("main.create_alarm", RouteDecision.Yield, false),
            new EffectDispatch.UnitRoute("main.alarm_nightly_once", RouteDecision.Yield, false),
        };
        var dead = new[] { new EffectDispatch.UnitRoute("remake.milk_items", RouteDecision.Own, true) };
        var blocked = new[] { new EffectDispatch.UnitRoute("main.price_milk", RouteDecision.VersionBlocked, false) };
        var mixedYieldDead = new[]
        {
            new EffectDispatch.UnitRoute("main.create_trap", RouteDecision.Yield, false),
            new EffectDispatch.UnitRoute("main.trap_nightly_once", RouteDecision.Yield, true),
        };

        string note;
        // 核心修复行为:全部 Yield → Yield(原版回退正向断言,0x08 六项假 FAIL 的对症)
        Check.Eq("Yield", EffectDispatch.Decide(yield1, out note).ToString(), "单单元 Yield → Yield");
        Check.Eq("Yield", EffectDispatch.Decide(yield2, out note).ToString(), "多单元全 Yield → Yield");

        // 现状回归:Own → Own(改值生效断言不变)
        Check.Eq("Own", EffectDispatch.Decide(own, out note).ToString(), "Own → Own(现状断言)");

        // 异常态 SKIP(暴露面在 Tier1 路由审计,值直测不双重记账)
        Check.Eq("Skip", EffectDispatch.Decide(dead, out note).ToString(), "Dead → Skip");
        Check.Eq(true, note != null && note.Contains("remake.milk_items") && note.Contains("Dead"),
            "Dead note 带单元 id 与原因: " + note);
        Check.Eq("Skip", EffectDispatch.Decide(blocked, out note).ToString(), "VersionBlocked → Skip");

        // 优先级:Dead 压过 Yield(一键多单元的异常态优先显式跳过)
        Check.Eq("Skip", EffectDispatch.Decide(mixedYieldDead, out note).ToString(),
            "Yield+Dead 混合 → Skip(Dead 优先)");
        Check.Eq(true, note != null && note.Contains("trap_nightly_once"),
            "混合 note 指向 Dead 单元: " + note);

        // 防御兜底:空数组(键无 hook 单元;DataWrite/WriteThrough 键不经此路径)
        Check.Eq("Own", EffectDispatch.Decide(new EffectDispatch.UnitRoute[0], out note).ToString(),
            "空单元集 → Own(防御性现状断言)");
        Check.Eq("Own", EffectDispatch.Decide(null, out note).ToString(), "null 单元集 → Own");

        // 真实目录抽检:R6 六 FAIL 键的让位单元在 YieldOwners 表内(分派输入面
        // 成立;1.6.1 翻转后 alarm_nightly/trap_nightly 撤让,抽检换成仍让位的单元)
        Check.Eq(true, Router.YieldOwners.ContainsKey("main.on_loot_rolled")
            && Router.YieldOwners.ContainsKey("main.price_milk")
            && Router.YieldOwners.ContainsKey("main.price_ovulation_shot")
            && Router.YieldOwners.ContainsKey("main.nightly_milk_value"),
            "让位键的单元均在让位表(on_loot/milk×2/ovulation;1.6.1 翻转后剩余 9 单元)");
    }
}
