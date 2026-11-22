using System;
using System.Collections.Generic;
using LewdMemeConfig;

// Task 10:ValueMath 纯函数语义(brief Step 1 三断言 + 换算/钳制辅助)。
// ValuePostfixPatches(Patches/)依赖游戏/Harmony/主 mod ModReflect 类型,不进测试工程;
// 其 postfix 直读注册表的保真由本组断言(半点取整方向、池上限钳制、T7 价格六键
// 默认值与载体负向断言)+ Task 14 游戏内验收共同覆盖。
public static class ValuePostfixTests
{
    public static void Run()
    {
        var cfg = ConfigRegistry.CreateDefault();

        // -- ApplyPercent(brief Step 1 原样三断言:value×(1+percent/100),四舍五入) --
        Check.Eq(85, ValueMath.ApplyPercent(100, -15f), "ApplyPercent(100,-15)=85");
        Check.Eq(230, ValueMath.ApplyPercent(200, 15f), "ApplyPercent(200,15)=230");
        Check.Eq(90, ValueMath.ApplyPercent(90, 0f), "ApplyPercent(90,0)=90");

        // -- 四舍五入方向:传统四舍五入(.5 远离零,非银行家舍入) --
        // 12.5/100=0.125 为二进制精确值,100×1.125=112.5 恰落半点
        Check.Eq(113, ValueMath.ApplyPercent(100, 12.5f), "ApplyPercent half rounds away from zero");
        Check.Eq(88, ValueMath.ApplyPercent(100, -12.5f), "ApplyPercent negative half rounds away from zero");
        // 整数结果不受取整方向影响
        Check.Eq(45, ValueMath.ApplyPercent(50, -10f), "ApplyPercent(50,-10)=45");

        // -- PercentToInt:client.semen_price_mod/bonus_mod(float)→ SetPreExposedValueMod(int) --
        Check.Eq(-15, ValueMath.PercentToInt(-15f), "PercentToInt -15");
        Check.Eq(15, ValueMath.PercentToInt(15f), "PercentToInt 15");
        Check.Eq(0, ValueMath.PercentToInt(0f), "PercentToInt 0");
        Check.Eq(-13, ValueMath.PercentToInt(-12.5f), "PercentToInt -12.5 away from zero");
        Check.Eq(13, ValueMath.PercentToInt(12.5f), "PercentToInt 12.5 away from zero");

        // -- PoolSizeClamp:client.pool_size 下限 1(0/负数视为 1,概率分母不为 0) --
        Check.Eq(3, ValueMath.PoolSizeClamp(3), "PoolSizeClamp 3 default");
        Check.Eq(1, ValueMath.PoolSizeClamp(1), "PoolSizeClamp 1");
        Check.Eq(1, ValueMath.PoolSizeClamp(0), "PoolSizeClamp 0 floors to 1");
        Check.Eq(1, ValueMath.PoolSizeClamp(-5), "PoolSizeClamp -5 floors to 1");
        Check.Eq(7, ValueMath.PoolSizeClamp(7), "PoolSizeClamp 7 passes through");

        // -- 0.7.0 Task 7:Create 系价格六键 postfix 化的纯逻辑基石 --
        // ① 默认值 = 1.9.2 常量逐字(postfix 读 Cfg(key, def) 的回退面:
        //    注册表缺席/读失败时 rewrite 同值 = 零漂移;实测值断言归游戏内
        //    自检 SpawnItem,postfix 在 Create* 尾部 ApplyCsv 之后执行恒胜);
        // ② 六键不得出现在 TranspilePlan 表内(价格 = postfix 载体,
        //    transpile 改价会被 ApplyCsv 尾调覆盖,T2 C' 节实证)。
        var priceDefaults = new Dictionary<string, int>
        {
            [CfgKeys.AlarmPrice] = 75,              // CreateAlarm
            [CfgKeys.AlarmTentaclePrice] = 150,     // CreateTentacleAlarm
            [CfgKeys.TrapPrice] = 100,              // CreateManhole(井盖)
            [CfgKeys.TrapTrapPrice] = 200,          // CreateTrap
            [CfgKeys.PrinterPrice] = 500,           // CreatePrinter
            [CfgKeys.PrinterNeedlePrice] = 15,      // CreateNeedle(空针管)
        };
        foreach (var kv in priceDefaults)
        {
            Check.Eq(kv.Value, cfg.Current<int>(kv.Key),
                "T7 价格键默认 = 1.9.2 常量: " + kv.Key);
            Check.Eq(true, TranspilePlan.DefaultOf(kv.Key) == null,
                "T7 价格键不在 transpile 表(postfix 载体): " + kv.Key);
        }
        Check.Eq(6, priceDefaults.Count, "T7 价格 postfix 恰六键(alarm2+trap2+printer2)");
   }
}
