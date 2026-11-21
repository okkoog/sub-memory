using System;
using LewdMemeConfig;

// Task 3:VersionGuard 纯函数语义(null→false;"1.9.2"→true;其余→false)。
// Task 5:受支持版本随主 mod 1.6.1 适配更新为 "1.6.1"(精确匹配语义不变);
// Task 14b(2026-11-13):1.6.1 → 1.7.1(旧基线弃置,lewd_meme_161 存档)。
// 0.7.0 Task 1(2026-11-17):1.7.1 → 1.9.2(旧基线弃置,lewd_meme_171 存档)。
// MainModResolver 依赖运行时 ModReflect/ModLoader,无法脱离游戏单测,
// 由 Task 14 游戏内验收(§7.2 主 mod 缺席 / §7.3 版本不符)覆盖。
public static class VersionGuardTests
{
    public static void Run()
    {
        Check.Eq("1.9.2", VersionGuard.SupportedMainModVersion, "supported version constant");
        Check.Eq(true, VersionGuard.IsSupported("1.9.2"), "exact match supported");
        Check.Eq(false, VersionGuard.IsSupported("1.6.1"), "previous baseline (1.6.1) dropped by 14b");
        Check.Eq(false, VersionGuard.IsSupported("1.7.0"), "older sibling version unsupported");
        Check.Eq(false, VersionGuard.IsSupported("1.7.2"), "non-matching version unsupported");
        Check.Eq(false, VersionGuard.IsSupported(null), "null unsupported");
        Check.Eq(false, VersionGuard.IsSupported(""), "empty unsupported");
    }
}
