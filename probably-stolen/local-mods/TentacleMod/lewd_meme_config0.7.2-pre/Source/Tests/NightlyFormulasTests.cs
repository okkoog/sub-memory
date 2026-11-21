using System;
using LewdMemeConfig;

// Task 7:NightlyFormulas 纯函数语义(反编译 v1.3.8 TentacleBody.NightlyTick 字面量)。
// 0.7.0 Task 4:NightlyTick prefix 复刻体退役,公式的消费载体 = main.nightly_transpile /
// main.nightly_milk_value 两 transpiler 单元的注入点(数值仍在本类断言锁形,注入点
// 换掉原 IL 常量后方法体照跑);NightlyPatch(Patches/)依赖游戏/Harmony/主 mod
// ModReflect 类型,不进测试工程,保真由本组断言(原版字面量三组 ActRange + 四公式
// + 表内默认值互验)+ 游戏内验收共同覆盖。
public static class NightlyFormulasTests
{
    public static void Run()
    {
        // -- ActRange:原版三组值(NightlyTick L998-1028 的区间公式) --
        // 性交 [5+(L-1)×1, 10+(L-1)×3]:L1→(5,10)、L3→(7,16)
        Check.Eq((5, 10), NightlyFormulas.ActRange(5, 1, 10, 3, 1), "ActRange sex L1");
        Check.Eq((7, 16), NightlyFormulas.ActRange(5, 1, 10, 3, 3), "ActRange sex L3");
        // 口交 [(L-1)×1, 2+(L-1)×2]:L2→(1,4)(肛交同式)
        Check.Eq((1, 4), NightlyFormulas.ActRange(0, 1, 2, 2, 2), "ActRange oral L2");
        // max<min 时 max=min(brief 修正语义)
        Check.Eq((6, 6), NightlyFormulas.ActRange(6, 0, 5, 0, 1), "ActRange max<min flat");
        Check.Eq((5, 5), NightlyFormulas.ActRange(3, 2, 2, 0, 2), "ActRange max<min crossed");
        // 等级外推:公式无下限保护(原版 bodyLevel 由 GetBodyLevel 的 clamp 恒 ≥1,
        // level=0 仅按 base+(level-1)×per 自然外推为 (4,7))
        Check.Eq((4, 7), NightlyFormulas.ActRange(5, 1, 10, 3, 0), "ActRange level 0 extrapolates");

        // -- ConceiveChance:min(cap, count×perSex),clamp [0,1](原版 Mathf.Clamp01(num8/100f)) --
        Check.Eq(1f, NightlyFormulas.ConceiveChance(100, 0.01f, 1f), "ConceiveChance 100 sex");
        Check.Eq(0.5f, NightlyFormulas.ConceiveChance(50, 0.01f, 1f), "ConceiveChance 50 sex");
        Check.Eq(0f, NightlyFormulas.ConceiveChance(0, 0.01f, 1f), "ConceiveChance 0 sex");
        Check.Eq(1f, NightlyFormulas.ConceiveChance(999, 0.01f, 1f), "ConceiveChance capped at 1");

        // -- MilkValue:100 + 泌乳等级 × 40 --
        Check.Eq(220, NightlyFormulas.MilkValue(100, 40, 3), "MilkValue lv3");
        Check.Eq(100, NightlyFormulas.MilkValue(100, 40, 0), "MilkValue lv0");

        // -- SemenChance:min(cap, base + train×perTrain)(原版 min(1, 0.4+调教/10×0.1)) --
        Check.Eq(1f, NightlyFormulas.SemenChance(0.4f, 0.01f, 60, 1f), "SemenChance train 60");
        Check.Eq(0.4f, NightlyFormulas.SemenChance(0.4f, 0.01f, 0, 1f), "SemenChance train 0");

        // -- LactationAfterGain:min(cap, current+gain)(原版 Mathf.Min(3, num+1)) --
        Check.Eq(3, NightlyFormulas.LactationAfterGain(3, 1, 3), "LactationAfterGain at cap");
        Check.Eq(2, NightlyFormulas.LactationAfterGain(1, 1, 3), "LactationAfterGain below cap");

        // -- GridSize:(edge, edge),供 layout.brood_size/output_size --
        Check.Eq((16, 16), NightlyFormulas.GridSize(16), "GridSize 16");
        Check.Eq((1, 1), NightlyFormulas.GridSize(1), "GridSize 1");

        // -- 0.7.0 Task 4:公式默认参数 ↔ TranspilePlan 表内默认值同源互验
        //    (两 transpiler 单元的注入点 getter 兜底读 TranspilePlan.DefaultOf,
        //    默认 = 1.9.2 反编译字面量;此处把上组公式断言的字面量与表逐字对齐,
        //    改表不改公式[或反向]在此显形)--
        Check.Eq(5, TranspilePlan.DefaultOf(CfgKeys.SexMinBase), "transpile 默认: sex_min_base = 5");
        Check.Eq(10, TranspilePlan.DefaultOf(CfgKeys.SexMaxBase), "transpile 默认: sex_max_base = 10");
        Check.Eq(3, TranspilePlan.DefaultOf(CfgKeys.SexPerLevelMax), "transpile 默认: sex_per_level_max = 3");
        Check.Eq(2, TranspilePlan.DefaultOf(CfgKeys.OralMaxBase), "transpile 默认: oral_max_base = 2");
        Check.Eq(2, TranspilePlan.DefaultOf(CfgKeys.OralPerLevelMax), "transpile 默认: oral_per_level_max = 2");
        Check.Eq(2, TranspilePlan.DefaultOf(CfgKeys.AnalMaxBase), "transpile 默认: anal_max_base = 2");
        Check.Eq(2, TranspilePlan.DefaultOf(CfgKeys.AnalPerLevelMax), "transpile 默认: anal_per_level_max = 2");
        Check.Eq(1f / 7f, TranspilePlan.DefaultOf(CfgKeys.OvulateChance), "transpile 默认: ovulate_chance = 1/7");
        Check.Eq(0.01f, TranspilePlan.DefaultOf(CfgKeys.ConceivePerSex), "transpile 默认: conceive_per_sex = 0.01");
        Check.Eq(30, TranspilePlan.DefaultOf(CfgKeys.GestationDays), "transpile 默认: gestation_days = 30");
        Check.Eq(3, TranspilePlan.DefaultOf(CfgKeys.LactationCap), "transpile 默认: lactation_cap = 3");
        Check.Eq(0.4f, TranspilePlan.DefaultOf(CfgKeys.SemenChanceBase), "transpile 默认: semen_chance_base = 0.4");
        Check.Eq(0.01f, TranspilePlan.DefaultOf(CfgKeys.SemenChancePerTrain), "transpile 默认: semen_chance_per_train = 0.01");
        Check.Eq(1f, TranspilePlan.DefaultOf(CfgKeys.SemenChanceCap), "transpile 默认: semen_chance_cap = 1");
        Check.Eq(100, TranspilePlan.DefaultOf(CfgKeys.MilkValueBase), "transpile 默认: milk_value_base = 100");
        Check.Eq(40, TranspilePlan.DefaultOf(CfgKeys.MilkValuePerLv), "transpile 默认: milk_value_per_lv = 40");
        // 形态还原:倒数形态默认 0.01 → getter 返 1/cfg ≈ 100(原版除数字面量);
        // PerTrain 形态默认 0.01 → 返 cfg×10 ≈ 0.1(原版尾乘子)。浮点容差
        // 1e-3(0.01f 无精确十进制表示,误差 ~2e-6 是表示层噪声,非语义漂移)。
        Check.Eq(true, Math.Abs(1f / (float)TranspilePlan.DefaultOf(CfgKeys.ConceivePerSex) - 100f) < 1e-3f,
            "transpile 形态: 倒数默认还原原版除数 100");
        Check.Eq(true, Math.Abs((float)TranspilePlan.DefaultOf(CfgKeys.SemenChancePerTrain) * 10f - 0.1f) < 1e-3f,
            "transpile 形态: PerTrain 默认还原原版尾乘子 0.1");
    }
}
