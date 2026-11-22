using LewdMemeConfig;

// Task 12:TrapRules.ParseTacticChances 纯函数语义(陷阱接管的前置解析)。
// 语义对照主 mod 1.6.1 MidnightPawn.Game.TentacleTrap.TACTIC_CHANCES = {0.1f,0.2f,0.3f}
// 与 PatchHub 契约(Review Focus 4):逗号分隔恰好 3 值、float.TryParse(InvariantCulture)
// 全过、每值 clamp [0,1];任何不满足 → null = 调用方保持现值不动(nightly_once 回退
// 主 mod 数组现值 / tactic_write 跳过覆写)。Trim 决策:逐段 Trim(容忍手写空格,
// " 0.1 , 0.2 , 0.3 " 合法)。TrapPatch 依赖主 mod 类型(运行时 ModReflect)与
// Harmony,住在 Patches/ 不进测试工程;其接管语义由本纯函数测试 + 游戏内验收共同覆盖。
public static class TrapRulesTests
{
    public static void Run()
    {
        // -- happy path:默认串逐值精确(brief 样例) --
        CheckArr(new[] { 0.1f, 0.2f, 0.3f }, TrapRules.ParseTacticChances("0.1,0.2,0.3"),
            "tactic happy path 0.1,0.2,0.3");

        // -- clamp:越界值压回 [0,1],其余段照常解析(brief 样例) --
        CheckArr(new[] { 1f, 0.2f, 0.3f }, TrapRules.ParseTacticChances("2,0.2,0.3"),
            "tactic clamp high 2 -> 1");
        CheckArr(new[] { 0f, 0.2f, 0.3f }, TrapRules.ParseTacticChances("-1,0.2,0.3"),
            "tactic clamp low -1 -> 0");
        CheckArr(new[] { 1f, 1f, 0f }, TrapRules.ParseTacticChances("9,1,-0"),
            "tactic clamp 边界值 1/-0 原样");

        // -- 违规 → null(brief 样例) --
        Check.Eq(true, TrapRules.ParseTacticChances("0.5,0.5") == null,
            "tactic 两值 -> null");
        Check.Eq(true, TrapRules.ParseTacticChances("0.1,0.2,0.3,0.4") == null,
            "tactic 四值 -> null");
        Check.Eq(true, TrapRules.ParseTacticChances("") == null,
            "tactic 空串 -> null");
        Check.Eq(true, TrapRules.ParseTacticChances(null) == null,
            "tactic null 输入 -> null");
        Check.Eq(true, TrapRules.ParseTacticChances("a,b,c") == null,
            "tactic 非数字 -> null");
        Check.Eq(true, TrapRules.ParseTacticChances("0.1,b,0.3") == null,
            "tactic 混入非数字 -> null");

        // -- 非有限值 → null(收尾防护:"NaN"/"Infinity" 能被 float.TryParse 成功解析,
        //    NaN 与任何比较均为 false 会绕过两侧 clamp、Infinity 仅被 clamp 压回 1,
        //    均非合法策略概率,显式按非法输入拒绝)--
        Check.Eq(true, TrapRules.ParseTacticChances("NaN,0.2,0.3") == null,
            "tactic NaN -> null");
        Check.Eq(true, TrapRules.ParseTacticChances("Infinity,0.2,0.3") == null,
            "tactic Infinity -> null");

        // -- Trim 容忍(文档化决策):逐段去空格 --
        CheckArr(new[] { 0.1f, 0.2f, 0.3f }, TrapRules.ParseTacticChances(" 0.1 , 0.2 , 0.3 "),
            "tactic Trim 各段(含首尾空格)");

        // -- InvariantCulture:点号小数,不受系统区域影响 --
        CheckArr(new[] { 0.5f, 0.5f, 0.5f }, TrapRules.ParseTacticChances("0.5,0.5,0.5"),
            "tactic 点号小数解析");
    }

    /// <summary>逐元素比较(Check.Eq 对数组是引用比较,不能直接用);null 传染:false。</summary>
    private static void CheckArr(float[] expected, float[] actual, string name)
    {
        if (actual == null || actual.Length != expected.Length)
        {
            Check.Eq("float[" + expected.Length + "]", actual == null ? "null" : "float[" + actual.Length + "]", name);
            return;
        }
        for (int i = 0; i < expected.Length; i++)
        {
            Check.Eq(expected[i], actual[i], name + " [i=" + i + "]");
        }
    }
}
