using System;
using System.Collections.Generic;
using LewdMemeConfig;

// F1:路由审计期望态纯函数(RouteExpect.Expected)——R5 豁免 × VersionBlocked
// 组合的假红修复:主 mod 版本不命中且 remake/IPH 在场时,Router 对豁免单元裁
// VersionBlocked(R5:版本压过让位),但其 install 委托在让位分支零产出、
// HookRuntime ③ 登记期跳过 → 实际态 NotRegistered,期望态必须与登记过滤对齐
// (修复前返回 VersionBlocked → 审计假 FAIL)。其余分支为回归锁定(与
// HookRuntime 登记过滤 / HookInstaller TargetState 同语义)。真实目录单元取自
// HookCatalog(豁免目标 create_maiden、T4 后非豁免的 nightly_transpile、R11
// 缺席目标 remake.ovulation_price[ST4: 原样本 semen_formula 已注销]与对照组各一)。
// 0.7.0 T4:main.nightly_transpile/main.nightly_milk_value 恒产装配委托
// (remake 让位改走 Router ③ Yield 过滤,无 Register 让位分支)——豁免通道
// 只剩 IPH 两单元(MaidenPatch 先例);remake 在场 × 主 mod 版本不命中时
// nightly 组 = VersionBlocked(照常登记 + installer 压制),不再是 NotRegistered。
// ST 1.6.1(Task 3,2026-11-17):让位表翻转后 nightly_transpile 等四单元已无
// Yield 态,但 Expected 是纯函数(decision 由调用方直传)——本组断言仍以
// RouteDecision.Yield 字面量锁定让位分派的登记过滤语义,nightly 单元同时
// 兼作 VersionBlocked/登记通道的回归样本。
// ST 1.6.1(Task 4,2026-11-17):semen_formula 注销后 R11 缺席目标样本换
// remake.ovulation_price(同 TargetMod=remake 的 SubModHook,缺席拦截语义
// 不依赖具体单元);跨族键防线(nightly.semen_chance_cap × semen_formula)
// 随单元注销终结——该键唯一消费 main.nightly_transpile[MainHook],不再触发
// 缺席目标对账路径。
public static class RouteExpectTests
{
    public static void Run()
    {
        HookUnitDef nightly = Find("main.nightly_transpile");
        HookUnitDef maiden = Find("main.create_maiden");
        HookUnitDef levelCap = Find("main.level_cap");
        HookUnitDef pool = Find("pools.maiden_fbb"); // T9:pools.maiden 已删,DataWrite 对照换 fbb 池
        HookUnitDef mirror = Find("wt.remake.fatigue_enabled"); // ST4:product_chance 已撤写透,换存续键
        HookUnitDef absent = Find("remake.ovulation_price");    // ST4:semen_formula 已注销,R11 缺席样本换血

        // F1 核心组合:豁免 × VersionBlocked → NotRegistered(与登记过滤一致)。
        // T4 后 nightly 组非豁免(委托恒产出):同组合下照常登记、installer
        // 压制 → VersionBlocked(下方对照组),行为差异在此钉住。
        Check.Eq("NotRegistered", RouteExpect.Expected(maiden, RouteDecision.VersionBlocked, false, true),
            "F1: create_maiden 豁免×VersionBlocked(IPH 在场+主 mod 版本不命中)→ NotRegistered");

        // 对照:同单元非豁免(remake/IPH 缺席,委托照常产出并登记)→ VersionBlocked
        Check.Eq("VersionBlocked", RouteExpect.Expected(nightly, RouteDecision.VersionBlocked, true, false),
            "F1 对照: 非豁免×VersionBlocked → VersionBlocked(登记照常)");
        Check.Eq("VersionBlocked", RouteExpect.Expected(levelCap, RouteDecision.VersionBlocked, false, false),
            "F1 对照: 普通单元×VersionBlocked → VersionBlocked");

        // 回归:让位 / 正常装卸 / 载体过滤(与旧 ExpectedState 语义一致)
        Check.Eq("NotRegistered", RouteExpect.Expected(nightly, RouteDecision.Yield, true, false),
            "Yield → NotRegistered(让位不登记)");
        Check.Eq("NotRegistered", RouteExpect.Expected(nightly, RouteDecision.Yield, true, true),
            "Yield×豁免 → NotRegistered(豁免仅改 VersionBlocked 分支)");
        Check.Eq("Installed", RouteExpect.Expected(levelCap, RouteDecision.Own, true, false),
            "Own+任一开关开 → Installed");
        Check.Eq("Absent", RouteExpect.Expected(levelCap, RouteDecision.Own, false, false),
            "Own+全关 → Absent");
        Check.Eq("NotRegistered", RouteExpect.Expected(pool, RouteDecision.Own, true, false),
            "DataWrite → NotRegistered(不进安装器)");
        Check.Eq("NotRegistered", RouteExpect.Expected(mirror, RouteDecision.Own, true, false),
            "WriteThrough → NotRegistered(不进安装器)");

        // R11-F1:SubModHook 目标缺席 × 开关开 → NotRegistered(0x0f 根因:
        // 跨族键开关拨动触发缺席目标的 ModReflect 对账炸 Dead;③ 登记期拦截后
        // 实际态 NotRegistered,期望态须对齐——开关开也一样。ST4:样本换
        // remake.ovulation_price,原样本 semen_formula 已注销[跨族键防线随之
        // 终结——cap 回归 MainHook 唯一消费],缺席拦截逻辑单元无关)
        Check.Eq("NotRegistered", RouteExpect.Expected(absent, RouteDecision.Own, true, false, true),
            "R11: 缺席目标单元×开关开 → NotRegistered");
        Check.Eq("NotRegistered", RouteExpect.Expected(absent, RouteDecision.Own, false, false, true),
            "R11: 缺席目标单元×全关 → NotRegistered");
        // 对照:目标在场 → 正常装卸语义(缺席拦截不越界)
        Check.Eq("Installed", RouteExpect.Expected(absent, RouteDecision.Own, true, false, false),
            "R11 对照: 目标在场×开关开 → Installed");
        Check.Eq("Installed", RouteExpect.Expected(levelCap, RouteDecision.Own, true, false, true),
            "R11 对照: MainHook 不受 targetAbsent 影响(缺席走装配层语义)");
    }

    private static HookUnitDef Find(string id)
    {
        foreach (HookUnitDef u in HookCatalog.Units)
            if (u.Id == id) return u;
        throw new KeyNotFoundException("测试目录缺单元: " + id);
    }
}
