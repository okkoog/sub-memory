using System;
using LewdMemeConfig;

// Task 10:WriteThroughEngine 快照/恢复纯逻辑(spec §4.5 write-through 键)。
// write 动作是注入的 Action<string>,测试用 spy 闭包计数;值统一 string 进/string 出
// (ConfigEntry.Serialize 语义,游戏侧转换归 Task 11 载体)。编号用例:
// ①写 + 建快照 ②恢复 + 清快照 ③无快照 no-op ④重复 Enable 锚定开启时刻——
// 周期内(无中间 Disable)后续 Enable 只写值、快照保持首个 currentValue(0.6.0
// 修正裁定:值变化重写不再覆盖恢复点,spec §4.5「关 = 恢复开启时的快照」;
// 旧「fresh snapshot each enable」语义废弃——那会让 Disable 恢复成我方上一
// 次写入值,玩家开启前的原值丢失)④b 周期边界:Disable 清除后再次 Enable
// 重新锚定新快照 ⑤currentValue 缺席 → 不写不建快照。
public static class WriteThroughEngineTests
{
    public static void Run()
    {
        // -- ① Enable:write(ourValue) 且建立快照(currentValue) --
        var eng = new WriteThroughEngine();
        int calls = 0;
        string written = null;
        eng.Enable("lewd_meme_remake", "main.price_milk", "150", "120",
            v => { calls++; written = v; });
        Check.Eq(1, calls, "① Enable 调 write 恰一次");
        Check.Eq("120", written, "① write 收到 ourValue");
        Check.Eq(true, eng.HasSnapshot("lewd_meme_remake", "main.price_milk"),
            "① Enable 建立快照");

        // -- ② Disable:write(snapshot) 恢复并清除快照(接 ① 同一实例) --
        int back = 0;
        string restored = null;
        eng.Disable("lewd_meme_remake", "main.price_milk",
            v => { back++; restored = v; });
        Check.Eq(1, back, "② Disable 调 write 恰一次");
        Check.Eq("150", restored, "② 恢复的是接管前快照值");
        Check.Eq(false, eng.HasSnapshot("lewd_meme_remake", "main.price_milk"),
            "② 恢复后快照清除");

        // -- ③ 无快照 Disable:不调 write(纯 no-op;含从未 Enable 与已恢复两态) --
        var eng2 = new WriteThroughEngine();
        int calls3 = 0;
        eng2.Disable("claude.lewdMemeBrood", "main.ghost_unit", _ => calls3++);
        Check.Eq(0, calls3, "③ 从未 Enable 的键 Disable 不调 write");
        Check.Eq(false, eng2.HasSnapshot("claude.lewdMemeBrood", "main.ghost_unit"),
            "③ no-op 后仍无快照");

        // -- ④ 重复 Enable(无中间 Disable):快照锚定开启时刻,不被后续 Enable
        //    覆盖(第二次 Enable = 分发器「值变化且开关 on」路径的等价物:
        //    无论当时对方当前值为何,恢复点保持开启时刻的首个 currentValue;
        //    旧覆盖语义会把它冲成我方上一轮写入后的值,玩家原值丢失)--
        var eng3 = new WriteThroughEngine();
        int calls4 = 0;
        eng3.Enable("lewd_meme_remake", "main.price_milk", "100", "55", _ => calls4++);
        eng3.Enable("lewd_meme_remake", "main.price_milk", "120", "60", _ => calls4++);
        Check.Eq(2, calls4, "④ 两次 Enable 各写一次");
        eng3.Disable("lewd_meme_remake", "main.price_milk",
            v => Check.Eq("100", v, "④ Disable 恢复开启时刻首快照(非后续 Enable 的当前值)"));
        Check.Eq(false, eng3.HasSnapshot("lewd_meme_remake", "main.price_milk"),
            "④ 恢复后快照清除");

        // -- ④b 周期边界:Enable → Disable(恢复+清)→ Enable 重新锚定新快照 --
        var eng5 = new WriteThroughEngine();
        eng5.Enable("claude.lewdMemeBrood", "eggBaseChance", "12", "40", _ => { });
        eng5.Disable("claude.lewdMemeBrood", "eggBaseChance",
            v => Check.Eq("12", v, "④b 第一周期恢复原值 12"));
        // 周期间对方值变动(如玩家在对方面板改过 40):新周期锚定新当前值
        eng5.Enable("claude.lewdMemeBrood", "eggBaseChance", "40", "70", _ => { });
        eng5.Disable("claude.lewdMemeBrood", "eggBaseChance",
            v => Check.Eq("40", v, "④b 第二周期重新锚定(恢复 40,非旧周期 12)"));
        Check.Eq(false, eng5.HasSnapshot("claude.lewdMemeBrood", "eggBaseChance"),
            "④b 第二周期恢复后快照清除");

        // -- ⑤ Enable 时 currentValue 缺席(null,目标键不存在):不写、不建快照 --
        var eng4 = new WriteThroughEngine();
        int calls5 = 0;
        eng4.Enable("item_portrait_hd", "main.create_maiden", null, "42", _ => calls5++);
        Check.Eq(0, calls5, "⑤ 目标键缺席不调 write");
        Check.Eq(false, eng4.HasSnapshot("item_portrait_hd", "main.create_maiden"),
            "⑤ 目标键缺席不建快照");
    }
}
