using System;

// 自制测试 runner 约定（后续所有任务遵守）：
//   static class Check { Eq / Throws / Failures / total }
//   每个测试文件一个 static class XxxTests { public static void Run(); }
//   TestMain.Main 逐个调用，末尾打印 ALL PASS (N checks) / FAILED (n)，退出码 = 失败数。
public static class Check
{
    public static int Failures;
    public static int total;

    public static void Eq<T>(T expected, T actual, string name)
    {
        total++;
        if (Equals(expected, actual)) return;
        Failures++;
        Console.WriteLine("FAIL " + name + ": expected <" + expected + "> got <" + actual + ">");
    }

    public static void Throws<TEx>(Action a, string name) where TEx : Exception
    {
        total++;
        try
        {
            a();
        }
        catch (TEx)
        {
            return; // 命中预期异常
        }
        catch (Exception ex)
        {
            Failures++;
            Console.WriteLine("FAIL " + name + ": expected " + typeof(TEx).Name
                + ", got " + ex.GetType().Name + ": " + ex.Message);
            return;
        }
        Failures++;
        Console.WriteLine("FAIL " + name + ": expected " + typeof(TEx).Name + ", nothing thrown");
    }
}

public static class TestMain
{
    public static int Main()
    {
        ConfigRegistryTests.Run();
        VersionGuardTests.Run();
        GenderDecisionTests.Run();
        RewriteDecisionTests.Run();
        NightlyFormulasTests.Run();
        ExpCurveTests.Run();
        CorruptRulesTests.Run();
        FatigueRulesTests.Run();
        MaidenRollTests.Run();
        LootRulesTests.Run();
        TrapRulesTests.Run();
        ValuePostfixTests.Run();
        SpecialMatcherTests.Run();
        ManualTextRendererTests.Run();
        PlayerChoicesTests.Run();
        PanelKeysTests.Run();
        NumericInputTests.Run();
        PoolRoutingTests.Run();
        HookInstallerTests.Run();
        HookCatalogTests.Run();
        FamilyModsTests.Run();
        RouterTests.Run();
        PanelMarksTests.Run();
        RouteExpectTests.Run();
        WriteThroughEngineTests.Run();
        FamilyPanelKeysTests.Run();
        SelfTestPlanTests.Run();
        SwitchOnContractTests.Run();
        EffectDispatchTests.Run();
        TranspilePlanTests.Run();
        Console.WriteLine(Check.Failures == 0
            ? "ALL PASS (" + Check.total + " checks)"
            : "FAILED (" + Check.Failures + ")");
        return Check.Failures;
    }
}
