using System.Collections.Generic;
using LewdMemeConfig;

// Task 9:MaidenRoll 纯函数语义(少女生成接管的前置判定与池写入守卫)。
// 断言值 = 主 mod v1.3.8 反编译字面量(ItemFactory.cs L466-506):
//   RollAttr:<0.01 → [70,80]、<0.06 → [60,70]、否则 [10,60](累计阈值,区间含端点);
//   AttrValue:≤60 原值、≤80: 60+(v-60)×5、>80: 160+(v-80)×10(断点 60/80);
//   FactionBase:上层区 100 / 治安部·黑市·革命军 50 / 其他 10;
//   池写入守卫:空/空白 CSV → 跳过写保持原版池。
// MaidenPatch 依赖主 mod 类型(运行时 ModReflect)与 Harmony,住在 Patches/ 不进测试工程;
// 其接管语义由本纯函数测试 + Task 14 游戏内验收共同覆盖。
public static class MaidenRollTests
{
    // v1.3.8 默认档:高 1% [70,80]、中 5% [60,70]、低 94% [10,60]
    private const double HighC = 0.01, MidC = 0.06;
    private static readonly (int lo, int hi) High = (70, 80), Mid = (60, 70), Low = (10, 60);

    // ConfigRegistry.RegisterMaiden 的 maiden.faction_base 默认串(逐字)
    private const string FactionCsv = "上层区:100,治安部:50,黑市:50,革命军:50,其他:10";

    private static bool InRange(int v, int lo, int hi) => v >= lo && v <= hi;

    public static void Run()
    {
        // -- RollAttr:三档区间选择(brief 固定 roll 断言落区间) --
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.005, HighC, MidC, High, Mid, Low), 70, 80),
            "RollAttr(0.005) -> [70,80] (roll<1%)");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.03, HighC, MidC, High, Mid, Low), 60, 70),
            "RollAttr(0.03) -> [60,70] (roll<6% 累计阈值)");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.5, HighC, MidC, High, Mid, Low), 10, 60),
            "RollAttr(0.5) -> [10,60] (低档)");
        // 阈值边界:原版为严格小于(value < 0.01f / value < 0.06f)
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.0, HighC, MidC, High, Mid, Low), 70, 80),
            "RollAttr(0.0) -> 高档 (0 < 0.01)");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.01, HighC, MidC, High, Mid, Low), 60, 70),
            "RollAttr(0.01) -> 中档 (0.01 < 0.01 为假,边界归中)");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.06, HighC, MidC, High, Mid, Low), 10, 60),
            "RollAttr(0.06) -> 低档 (0.06 < 0.06 为假,边界归低)");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.999999, HighC, MidC, High, Mid, Low), 10, 60),
            "RollAttr(0.999999) -> 低档上界不越界");
        Check.Eq(70, MaidenRoll.RollAttr(0.0, HighC, MidC, High, Mid, Low),
            "RollAttr(0.0) 档内小数位 0 -> 恰为 lo=70");
        // 自定义档:阈值/区间全可配
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.4, 0.5, 0.7, (90, 99), Mid, Low), 90, 99),
            "RollAttr 自定义 highChance=0.5 -> [90,99]");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.55, 0.5, 0.7, High, (80, 85), Low), 80, 85),
            "RollAttr 自定义 midChance=0.7 -> [80,85]");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.99, HighC, MidC, High, Mid, Low), 10, 60),
            "RollAttr(0.99) -> 低档");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.5, 0.0, 0.0, High, Mid, Low), 10, 60),
            "RollAttr 零概率高档/中档 -> 全落低档");
        Check.Eq(true, InRange(MaidenRoll.RollAttr(0.0, 1.0, 1.0, High, Mid, Low), 70, 80),
            "RollAttr highChance=1 -> 恒高档");

        // -- AttrPrice:原版 60/80 断点曲线(brief 三断言 + 边界) --
        Check.Eq(50, MaidenRoll.AttrPrice(50, 60, 80), "AttrPrice(50,60,80) = 50 (≤60 原值)");
        Check.Eq(110, MaidenRoll.AttrPrice(70, 60, 80), "AttrPrice(70,60,80) = 60+(70-60)×5 = 110");
        Check.Eq(260, MaidenRoll.AttrPrice(90, 60, 80), "AttrPrice(90,60,80) = 160+(90-80)×10 = 260");
        Check.Eq(10, MaidenRoll.AttrPrice(10, 60, 80), "AttrPrice(10,60,80) = 10 (低位透传)");
        Check.Eq(60, MaidenRoll.AttrPrice(60, 60, 80), "AttrPrice(60,60,80) = 60 (knee1 边界走原值)");
        Check.Eq(65, MaidenRoll.AttrPrice(61, 60, 80), "AttrPrice(61,60,80) = 65 (×5 段第一步)");
        Check.Eq(160, MaidenRoll.AttrPrice(80, 60, 80), "AttrPrice(80,60,80) = 160 (knee2 边界 = 60+20×5)");
        Check.Eq(170, MaidenRoll.AttrPrice(81, 60, 80), "AttrPrice(81,60,80) = 170 (×10 段第一步)");
        // 断点参数化(调用点固定 60/80,公式本身支持任意 knee)
        Check.Eq(50, MaidenRoll.AttrPrice(50, 50, 100), "AttrPrice(50,50,100) = 50 (knee1 原值)");
        Check.Eq(175, MaidenRoll.AttrPrice(75, 50, 100), "AttrPrice(75,50,100) = 50+25×5 = 175");
        Check.Eq(300, MaidenRoll.AttrPrice(100, 50, 100), "AttrPrice(100,50,100) = 50+50×5 = 300");
        Check.Eq(500, MaidenRoll.AttrPrice(120, 50, 100), "AttrPrice(120,50,100) = 300+20×10 = 500");

        // -- AttrDelta(R10 属性重掷差量,postfix 差量修正的单一事实源;R2 防掩蔽:
        //    恒等性 = AttrPrice 分段斜率 1/5/10 恒正 → 严格递增 → old≠new ⇒ Δ≠0) --
        Check.Eq(0L, MaidenRoll.AttrDelta(70, 70, 70, 70, 70, 70, 60, 80),
            "AttrDelta old==new → Δ=0(幂等)");
        Check.Eq(150L, MaidenRoll.AttrDelta(70, 70, 70, 80, 80, 80, 60, 80),
            "AttrDelta (70,70,70)→(80,80,80) = 3×(160−110) = 150");
        Check.Eq(-150L, MaidenRoll.AttrDelta(60, 60, 60, 10, 10, 10, 60, 80),
            "AttrDelta (60×3)→(10×3) = 3×(10−60) = −150(向下差量为负)");
        Check.Eq(545L, MaidenRoll.AttrDelta(50, 61, 81, 70, 80, 120, 60, 80),
            "AttrDelta 跨段混合(50→70:+60 / 61→80:+95 / 81→120:+390 → 总 +545)");
        // 防掩蔽恒等性(任一 old≠new ⇒ Δ≠0)——覆盖三段斜率各一例 + 混合
        Check.Eq(true, MaidenRoll.AttrDelta(10, 60, 60, 11, 60, 60, 60, 80) != 0,
            "AttrDelta old≠new(低位段 10→11)⇒ Δ≠0");
        Check.Eq(true, MaidenRoll.AttrDelta(70, 70, 70, 71, 70, 70, 60, 80) != 0,
            "AttrDelta old≠new(×5 段 70→71)⇒ Δ≠0");
        Check.Eq(true, MaidenRoll.AttrDelta(90, 90, 90, 91, 90, 90, 60, 80) != 0,
            "AttrDelta old≠new(×10 段 90→91)⇒ Δ≠0");
        Check.Eq(true, MaidenRoll.AttrDelta(10, 80, 60, 10, 80, 61, 60, 80) != 0,
            "AttrDelta old≠new(单属性低位 60→61 跨 knee)⇒ Δ≠0");
        Check.Eq(0L, MaidenRoll.AttrDelta(125, 100, 105, 125, 100, 105, 60, 80),
            "AttrDelta old 全等 new(测试区间值)→ Δ=0(合法:RNG 恰同值的唯一 Δ=0 路径)");

        // -- FactionBase:CSV 字典查表,未命中走 fallback(原版 default → 10) --
        var bases = new Dictionary<string, int> { { "上层区", 100 } };
        Check.Eq(100, MaidenRoll.FactionBase("上层区", bases, 10), "FactionBase(上层区) = 100");
        Check.Eq(10, MaidenRoll.FactionBase("游客区", bases, 10), "FactionBase(游客区) = fallback 10");
        Check.Eq(7, MaidenRoll.FactionBase("教会", bases, 7), "FactionBase 未命中 -> 自定义 fallback 7");
        Check.Eq(7, MaidenRoll.FactionBase("上层区", null, 7), "FactionBase null 字典 -> fallback");
        Check.Eq(7, MaidenRoll.FactionBase(null, bases, 7), "FactionBase null 阵营 -> fallback");
        // 默认 CSV 解析结果对五个原版阵营逐项等值(上层区100/治安部50/黑市50/革命军50/下层区10)
        var (defBases, defFallback) = MaidenRoll.ParseFactionBases(FactionCsv, 10);
        Check.Eq(10, defFallback, "默认 faction_base CSV 的 fallback = 10 (其他:10)");
        Check.Eq(4, defBases.Count, "默认 faction_base CSV 解析出 4 项 (其他 并入 fallback 不占表项)");
        Check.Eq(100, MaidenRoll.FactionBase("上层区", defBases, defFallback), "默认 CSV: 上层区 = 100");
        Check.Eq(50, MaidenRoll.FactionBase("治安部", defBases, defFallback), "默认 CSV: 治安部 = 50");
        Check.Eq(50, MaidenRoll.FactionBase("黑市", defBases, defFallback), "默认 CSV: 黑市 = 50");
        Check.Eq(50, MaidenRoll.FactionBase("革命军", defBases, defFallback), "默认 CSV: 革命军 = 50");
        Check.Eq(10, MaidenRoll.FactionBase("下层区", defBases, defFallback), "默认 CSV: 下层区 -> fallback 10");
        Check.Eq(false, defBases.ContainsKey("其他"), "默认 CSV: 其他 键不进表(仅作 fallback)");
        // 解析容错
        var (b2, f2) = MaidenRoll.ParseFactionBases("甲:5,其他:9", 3);
        Check.Eq(9, f2, "自定义 CSV: 其他:9 -> fallback 9");
        Check.Eq(5, MaidenRoll.FactionBase("甲", b2, f2), "自定义 CSV: 甲 = 5");
        var (b3, f3) = MaidenRoll.ParseFactionBases("甲:5", 3);
        Check.Eq(3, f3, "无 其他 项 -> fallback 取参数");
        var (b4, f4) = MaidenRoll.ParseFactionBases(" 甲 : 5 ", 3);
        Check.Eq(5, MaidenRoll.FactionBase("甲", b4, f4), "CSV 名与值两侧 Trim");
        var (b5, f5) = MaidenRoll.ParseFactionBases("", 10);
        Check.Eq(0, b5.Count, "空 CSV -> 空表");
        Check.Eq(10, f5, "空 CSV -> fallback 取参数");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseFactionBases("甲", 3),
            "faction_base 项缺 :值 -> FormatException");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseFactionBases("甲:x", 3),
            "faction_base 值非整数 -> FormatException");

        // -- PoolValid:池写入守卫(Review Focus 4) --
        Check.Eq(false, MaidenRoll.PoolValid(""), "PoolValid(\"\") = false");
        Check.Eq(true, MaidenRoll.PoolValid("a,b"), "PoolValid(\"a,b\") = true");
        Check.Eq(false, MaidenRoll.PoolValid(null), "PoolValid(null) = false");
        Check.Eq(false, MaidenRoll.PoolValid("   "), "PoolValid(空白) = false");
        Check.Eq(false, MaidenRoll.PoolValid(",, ,"), "PoolValid(仅空项) = false");
        Check.Eq(true, MaidenRoll.PoolValid("单"), "PoolValid(单项) = true");
        Check.Eq(true, MaidenRoll.PoolValid(" a , b "), "PoolValid(带空格) = true");

        // -- SplitCsv / ParseIntRange:patch 侧 Phase A 解析语义 --
        Check.Eq("a,b,c", string.Join(",", MaidenRoll.SplitCsv("a,b,c")), "SplitCsv 保序");
        Check.Eq("a,b", string.Join(",", MaidenRoll.SplitCsv(" a , , b ,")), "SplitCsv trim 去空");
        Check.Eq(0, MaidenRoll.SplitCsv(null).Count, "SplitCsv(null) -> 空");
        Check.Eq(0, MaidenRoll.SplitCsv(" , ").Count, "SplitCsv(全空白) -> 空");
        Check.Eq((70, 80), MaidenRoll.ParseIntRange("70,80"), "ParseIntRange(70,80)");
        Check.Eq((10, 60), MaidenRoll.ParseIntRange("10,60"), "ParseIntRange(10,60)");
        Check.Eq((60, 70), MaidenRoll.ParseIntRange(" 60 , 70 "), "ParseIntRange trim");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseIntRange("70"),
            "区间缺第二项 -> FormatException");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseIntRange("a,b"),
            "区间非整数 -> FormatException");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseIntRange("1,2,3"),
            "区间多于两项 -> FormatException");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseIntRange(""),
            "空区间串 -> FormatException");

        // ---- Task 6(更多可配置项):面板 CSV 容错——数值元组型整串回退默认,绝不抛 ----
        // 玩家在面板文本框敲 "7O,8O" 之类:回退默认区间,单键降级不连坐整条接管。
        Check.Eq((70, 80), MaidenRoll.ParseIntRangeSafe("7O,8O", "70,80", out var d1), "IntRangeSafe 非数值回退默认");
        Check.Eq(true, d1, "IntRangeSafe 非数值 usedDefault");
        Check.Eq((10, 60), MaidenRoll.ParseIntRangeSafe("70", "10,60", out var d2), "IntRangeSafe 项数不足回退默认");
        Check.Eq(true, d2, "IntRangeSafe 项数不足 usedDefault");
        Check.Eq((70, 80), MaidenRoll.ParseIntRangeSafe("70,80", "10,60", out var d3), "IntRangeSafe 合法照用");
        Check.Eq(false, d3, "IntRangeSafe 合法 usedDefault=false");
        Check.Eq((10, 60), MaidenRoll.ParseIntRangeSafe("", "10,60", out var d4), "IntRangeSafe 空串回退默认");
        Check.Eq(true, d4, "IntRangeSafe 空串 usedDefault");
        Check.Eq((70, 80), MaidenRoll.ParseIntRangeSafe(" 70 , 80 ", "10,60", out var d5), "IntRangeSafe trim 照用");
        Check.Eq(false, d5, "IntRangeSafe trim usedDefault=false");
        // 默认串自身非法(理论不可达:编译期常量)→ 终极回退 (0,0),仍不抛
        Check.Eq((0, 0), MaidenRoll.ParseIntRangeSafe("x", "y", out var d6), "IntRangeSafe 默认串也非法 -> (0,0)");
        Check.Eq(true, d6, "IntRangeSafe 终极回退 usedDefault");

        // 阵营基价 Safe:非法回退默认表
        var (fb2, ff2) = MaidenRoll.ParseFactionBasesSafe("上层区:1O0", "上层区:100,其他:10", 10);
        Check.Eq(100, fb2["上层区"], "FactionBasesSafe 非法回退默认表");
        Check.Eq(10, ff2, "FactionBasesSafe 非法 fallback 取默认表其他项");
        Check.Eq(0, MaidenRoll.ParseFactionBasesSafe("甲", "丙", 7).Item1.Count, "FactionBasesSafe 非法默认表 -> 空表");
        Check.Eq(7, MaidenRoll.ParseFactionBasesSafe("甲", "丙", 7).Item2, "FactionBasesSafe 非法默认表 fallback 取参数");
        var (fb3, ff3) = MaidenRoll.ParseFactionBasesSafe("黑市:20,其他:5", "上层区:100", 10);
        Check.Eq(20, fb3["黑市"], "FactionBasesSafe 合法照用");
        Check.Eq(5, ff3, "FactionBasesSafe 合法串的「其他」项生效");

        // exp_per_level 纯解析(自 MaidenPatch.ParseIntCsv 迁入;行为锁定:
        // 非数值项抛 FormatException,由 WriteExpNeed catch 回退原版数组)
        Check.Eq("100,100", string.Join(",", MaidenRoll.ParseIntList("100,100")), "ParseIntList 两项");
        Check.Eq("1,2,3", string.Join(",", MaidenRoll.ParseIntList(" 1 , 2 ,3 ")), "ParseIntList trim");
        Check.Eq(0, MaidenRoll.ParseIntList("").Length, "ParseIntList 空串 -> 空数组(调用方 PoolValid 先行守卫)");
        Check.Throws<System.FormatException>(() => MaidenRoll.ParseIntList("100,x"), "ParseIntList 非数值抛(调用方回退)");
    }
}
