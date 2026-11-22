using LewdMemeConfig;

// Task 6:RewriteDecision 纯函数语义(性别改写 4 档接管的前置判定)。
// GenderRatioPatch 依赖主 mod 类型(运行时 ModReflect)与 Harmony,住在 Patches/
// 不进测试工程;其"scope 3 放行原方法 / scope 2 跳过 IsAny / 档 2 女池兜底"
// 的门控语义由本真值表 + Task 14 游戏内验收共同覆盖。
// 档位语义(spec §6):
//   0 off(原版:chance 概率改写普通顾客,默认 0.75)
//   1    普通顾客 100% 改写(保留 SpecialClients.IsAny 排除)
//   2    连特殊顾客一起改写(不查 IsAny)
//   3    全女性(本判定恒 false——由 Task 5 游戏本体两点接管)
public static class RewriteDecisionTests
{
    public static void Run()
    {
        // -- ClampScope:<0 → 0,>3 → 3,界内原样(Review Focus 3) --
        Check.Eq(0, RewriteDecision.ClampScope(-1), "clamp -1 -> 0");
        Check.Eq(3, RewriteDecision.ClampScope(4), "clamp 4 -> 3");
        Check.Eq(2, RewriteDecision.ClampScope(2), "clamp 2 -> 2");
        Check.Eq(0, RewriteDecision.ClampScope(0), "clamp 0 -> 0");
        Check.Eq(1, RewriteDecision.ClampScope(1), "clamp 1 -> 1");
        Check.Eq(3, RewriteDecision.ClampScope(3), "clamp 3 -> 3");
        Check.Eq(0, RewriteDecision.ClampScope(int.MinValue), "clamp int.MinValue -> 0");
        Check.Eq(3, RewriteDecision.ClampScope(int.MaxValue), "clamp int.MaxValue -> 3");

        // -- ShouldRewrite 真值表:4 档 × isSpecial × roll --

        // scope 0:!isSpecial && roll < chance
        Check.Eq(true, RewriteDecision.ShouldRewrite(0, false, 0.75f, 0.10), "s0 normal roll0.10 -> true");
        Check.Eq(false, RewriteDecision.ShouldRewrite(0, false, 0.75f, 0.80), "s0 normal roll0.80 -> false");
        Check.Eq(true, RewriteDecision.ShouldRewrite(0, false, 0.75f, 0.749), "s0 roll<chance boundary -> true");
        Check.Eq(false, RewriteDecision.ShouldRewrite(0, false, 0.75f, 0.75), "s0 roll==chance -> false (strict <)");
        Check.Eq(false, RewriteDecision.ShouldRewrite(0, true, 0.75f, 0.10), "s0 special -> false");
        Check.Eq(false, RewriteDecision.ShouldRewrite(0, true, 1f, 0.0), "s0 special chance1 -> false");
        Check.Eq(true, RewriteDecision.ShouldRewrite(0, false, 1f, 0.999), "s0 normal chance1 -> true");
        Check.Eq(false, RewriteDecision.ShouldRewrite(0, false, 0f, 0.0), "s0 normal chance0 roll0 -> false");
        Check.Eq(true, RewriteDecision.ShouldRewrite(0, false, 0f, -0.5), "s0 chance0 negative roll -> true (纯 < 语义,不 clamp)");

        // scope 1:!isSpecial(roll 与 chance 均无关)
        Check.Eq(true, RewriteDecision.ShouldRewrite(1, false, 0.75f, 0.999), "s1 normal roll high -> true");
        Check.Eq(true, RewriteDecision.ShouldRewrite(1, false, 0f, 1.0), "s1 normal chance0 -> true");
        Check.Eq(false, RewriteDecision.ShouldRewrite(1, true, 0.75f, 0.0), "s1 special -> false");
        Check.Eq(false, RewriteDecision.ShouldRewrite(1, true, 1f, 0.0), "s1 special chance1 -> false");

        // scope 2:恒 true(含特殊顾客;roll 与 chance 均无关)
        Check.Eq(true, RewriteDecision.ShouldRewrite(2, true, 0.75f, 0.999), "s2 special -> true");
        Check.Eq(true, RewriteDecision.ShouldRewrite(2, false, 0.75f, 0.999), "s2 normal -> true");
        Check.Eq(true, RewriteDecision.ShouldRewrite(2, true, 0f, 1.0), "s2 special chance0 -> true");

        // scope 3:恒 false(全女性归游戏本体两点,本判定不干预)
        Check.Eq(false, RewriteDecision.ShouldRewrite(3, false, 1f, 0.0), "s3 normal -> false");
        Check.Eq(false, RewriteDecision.ShouldRewrite(3, true, 1f, 0.0), "s3 special -> false");
        Check.Eq(false, RewriteDecision.ShouldRewrite(3, false, 1f, 0.10), "s3 normal roll passed -> false");
    }
}
