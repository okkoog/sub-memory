using LewdMemeConfig;

// ST 1.6.1 Task 5(2026-11-17):疲劳核心六键的纯逻辑半边——Logic/FatigueRules
// 与 ShadowTrade 1.6.1 MidnightPawn.Game/MaidenFatigue.cs 数值面逐字对齐:
//  - ValueMul 五档:≥90 透支 / ≥80 疲惫 / ≥60 疲倦 / <20 休息充足 / 其余正常
//    (1f 硬编码无键);边界 const 90/80/60/20 逐点断言;默认四值
//    1.1/0.94/0.88/0.8 = 原版零漂移;
//  - NightlyGain 加权式:vaginal + anal + RoundToInt(oral × 0.5f)(银行家
//    舍入)→ ×基数 ×等级系数 ×(孕 1.5)×速度;weighted ≤ 0 / gain ≤ 0 两处
//    早退 + Max(1,·) 下限;基数默认 0.5 = 原版;口交加权 0.5f 独立常量非键;
//  - Recovered:恢复减法 0 下限(默认 5)。
// 面板条目/键集/计数锁在 FamilyPanelKeysTests/HookCatalogTests;自检目录
// 分布锁在 SelfTestPlanTests(InvokeStatic +4 = ValueMul 直测,见
// SelfTestRunner 执行器)。
public static class FatigueRulesTests
{
    public static void Run()
    {
        // —— ValueMul:默认四值(= 1.6.1 原版字面量)档位边界逐点 ——
        Check.Eq(1.1f, FatigueRules.ValueMul(0, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(0) = 休息充足 1.1(下界)");
        Check.Eq(1.1f, FatigueRules.ValueMul(19, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(19) = 休息充足 1.1(<20 边界)");
        Check.Eq(1f, FatigueRules.ValueMul(20, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(20) = 正常档 1f(≥20,无键硬编码)");
        Check.Eq(1f, FatigueRules.ValueMul(59, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(59) = 正常档 1f");
        Check.Eq(0.94f, FatigueRules.ValueMul(60, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(60) = 疲倦 0.94(≥60 边界)");
        Check.Eq(0.94f, FatigueRules.ValueMul(79, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(79) = 疲倦 0.94");
        Check.Eq(0.88f, FatigueRules.ValueMul(80, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(80) = 疲惫 0.88(≥80 边界)");
        Check.Eq(0.88f, FatigueRules.ValueMul(89, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(89) = 疲惫 0.88");
        Check.Eq(0.8f, FatigueRules.ValueMul(90, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(90) = 透支 0.8(≥90 边界)");
        Check.Eq(0.8f, FatigueRules.ValueMul(100, 1.1f, 0.94f, 0.88f, 0.8f),
            "ValueMul(100) = 透支 0.8(疲劳上限)");
        // 面板自定义值(接管态):仅目标档跟随,其余档与正常档不受连坐
        Check.Eq(1.5f, FatigueRules.ValueMul(5, 1.5f, 0.94f, 0.88f, 0.8f),
            "ValueMul 自定义休息充足 1.5");
        Check.Eq(0.6f, FatigueRules.ValueMul(65, 1.1f, 0.6f, 0.88f, 0.8f),
            "ValueMul 自定义疲倦 0.6");
        Check.Eq(0.7f, FatigueRules.ValueMul(85, 1.1f, 0.94f, 0.7f, 0.8f),
            "ValueMul 自定义疲惫 0.7");
        Check.Eq(0.5f, FatigueRules.ValueMul(95, 1.1f, 0.94f, 0.88f, 0.5f),
            "ValueMul 自定义透支 0.5");
        Check.Eq(1f, FatigueRules.ValueMul(30, 1.5f, 0.6f, 0.7f, 0.5f),
            "ValueMul 正常档恒 1f(自定义四值不连坐)");

        // —— NightlyGain:加权 + 基数乘式(默认基数 0.5 = 原版)——
        // weighted = 2 + 1 + RoundToInt(3×0.5=1.5)=2(银行家舍入)= 5;
        // gain = 5×0.5 = 2.5 → RoundToInt 2(ToEven)
        Check.Eq(2, FatigueRules.NightlyGain(2, 3, 1, false, 0.5f, 1f, 1f),
            "NightlyGain(2,3,1) = 2(weighted 5×0.5=2.5→2)");
        Check.Eq(0, FatigueRules.NightlyGain(0, 0, 0, false, 0.5f, 1f, 1f),
            "NightlyGain 零活动 → 0(weighted ≤ 0 早退)");
        Check.Eq(0, FatigueRules.NightlyGain(-3, -2, -1, false, 0.5f, 1f, 1f),
            "NightlyGain 负输入钳 0 → weighted 0 → 0");
        // weighted = 1 + 1 + RoundToInt(0.5)=0 = 2;gain = 2×0.5×1.5 = 1.5 → 2
        Check.Eq(2, FatigueRules.NightlyGain(1, 1, 1, true, 0.5f, 1f, 1f),
            "NightlyGain 孕 1.5:2×0.5×1.5=1.5→2(ToEven)");
        Check.Eq(4, FatigueRules.NightlyGain(4, 0, 0, false, 0.5f, 1f, 2f),
            "NightlyGain 累积速度 2:4×0.5×2=4");
        Check.Eq(0, FatigueRules.NightlyGain(2, 0, 0, false, 0f, 1f, 1f),
            "NightlyGain 基数 0 → gain ≤ 0 早退");
        Check.Eq(0, FatigueRules.NightlyGain(4, 0, 0, false, 0.5f, 0f, 1f),
            "NightlyGain 等级系数 0(11 级起不累积)→ 0");
        Check.Eq(1, FatigueRules.NightlyGain(1, 0, 0, false, 0.1f, 1f, 1f),
            "NightlyGain 小数下限:1×0.1=0.1→Max(1,0)=1");
        Check.Eq(12, FatigueRules.NightlyGain(6, 0, 0, false, 2f, 1f, 1f),
            "NightlyGain 自定义基数 2:6×2=12");
        // 口交加权 0.5f 独立常量(非键):oral=5 → weighted = round(2.5)=2
        Check.Eq(1, FatigueRules.NightlyGain(0, 5, 0, false, 0.5f, 1f, 1f),
            "NightlyGain 口交加权 0.5f 硬编码:5×0.5→2×0.5=1");

        // —— Recovered:透支恢复减法(默认 5 = 原版)——
        Check.Eq(95, FatigueRules.Recovered(100, 5), "Recovered(100,5) = 95");
        Check.Eq(3, FatigueRules.Recovered(8, 5), "Recovered(8,5) = 3");
        Check.Eq(0, FatigueRules.Recovered(3, 5), "Recovered(3,5) = 0(0 下限)");
        Check.Eq(0, FatigueRules.Recovered(0, 5), "Recovered(0,5) = 0");
        Check.Eq(90, FatigueRules.Recovered(100, 10), "Recovered 自定义 10:100→90");
        Check.Eq(100, FatigueRules.Recovered(100, 0), "Recovered 0 = 不变(基数 0 可选)");

        // —— 键常量与默认值注册(ConfigRegistry 表驱动,FamilyPanelKeys
        //     进表即注册;此处锁常量串与默认值,防漂移)——
        Check.Eq("remake.fatigue_value_mul_rested", CfgKeys.RemakeFatigueValueMulRested,
            "CfgKeys 休息充足档键名");
        Check.Eq("remake.fatigue_value_mul_tired", CfgKeys.RemakeFatigueValueMulTired,
            "CfgKeys 疲倦档键名");
        Check.Eq("remake.fatigue_value_mul_weary", CfgKeys.RemakeFatigueValueMulWeary,
            "CfgKeys 疲惫档键名");
        Check.Eq("remake.fatigue_value_mul_exhausted", CfgKeys.RemakeFatigueValueMulExhausted,
            "CfgKeys 透支档键名");
        Check.Eq("remake.fatigue_nightly_gain", CfgKeys.RemakeFatigueNightlyGain,
            "CfgKeys 夜间基数键名");
        Check.Eq("remake.fatigue_exhaust_recover", CfgKeys.RemakeFatigueExhaustRecover,
            "CfgKeys 透支恢复键名");
        var cfg = ConfigRegistry.CreateDefault();
        Check.Eq(1.1f, TryCurF(cfg, CfgKeys.RemakeFatigueValueMulRested), "注册表默认:休息充足 1.1");
        Check.Eq(0.94f, TryCurF(cfg, CfgKeys.RemakeFatigueValueMulTired), "注册表默认:疲倦 0.94");
        Check.Eq(0.88f, TryCurF(cfg, CfgKeys.RemakeFatigueValueMulWeary), "注册表默认:疲惫 0.88");
        Check.Eq(0.8f, TryCurF(cfg, CfgKeys.RemakeFatigueValueMulExhausted), "注册表默认:透支 0.8");
        Check.Eq(0.5f, TryCurF(cfg, CfgKeys.RemakeFatigueNightlyGain), "注册表默认:夜间基数 0.5");
        Check.Eq(5, TryCurI(cfg, CfgKeys.RemakeFatigueExhaustRecover), "注册表默认:透支恢复 5");
    }

    /// <summary>注册表安全读取(键未注册 → NaN/-1,断言红而非击穿 runner)。</summary>
    private static float TryCurF(ConfigRegistry cfg, string key)
    {
        try { return cfg.Current<float>(key); }
        catch (System.Collections.Generic.KeyNotFoundException) { return float.NaN; }
    }

    private static int TryCurI(ConfigRegistry cfg, string key)
    {
        try { return cfg.Current<int>(key); }
        catch (System.Collections.Generic.KeyNotFoundException) { return -1; }
    }
}
