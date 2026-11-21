using LewdMemeConfig;

// Task 1(开关体系与 IPH 兼容):数值文本框的解析/钳制/格式化纯函数。
// 动机:游戏滑槽显示数值不准(玩家反馈),面板改精确文本输入;
// InvariantCulture 全程(与 CSV 键解析一致),失败不抛(容错契约)。
public static class NumericInputTests
{
    public static void Run()
    {
        // -- Format:当前生效值 → 文本框初始串(InvariantCulture 往返) --
        Check.Eq("70", NumericInput.Format(70), "Format int 70");
        Check.Eq("0.5", NumericInput.Format(0.5f), "Format float 0.5");
        Check.Eq("-15", NumericInput.Format(-15), "Format int negative");
        Check.Eq("1", NumericInput.Format(1f), "Format float whole trims .0");

        // -- TryParseInt:Trim 容忍 --
        int vi;
        Check.Eq(true, NumericInput.TryParseInt(" 70 ", 0, 2000, out vi), "int trim parse ok");
        Check.Eq(70, vi, "int trim value");

        // 非法输入:false 不抛(空串/null/非数值/小数给 int)
        Check.Eq(false, NumericInput.TryParseInt("7O", 0, 2000, out vi), "int 7O rejected");
        Check.Eq(false, NumericInput.TryParseInt("", 0, 2000, out vi), "int empty rejected");
        Check.Eq(false, NumericInput.TryParseInt(null, 0, 2000, out vi), "int null rejected");
        Check.Eq(false, NumericInput.TryParseInt("1.5", 0, 2000, out vi), "int 1.5 rejected");

        // 越界钳制到 [min,max](对齐原滑块防呆)
        Check.Eq(true, NumericInput.TryParseInt("9999", 0, 2000, out vi), "int overrange clamps ok");
        Check.Eq(2000, vi, "int overrange clamps to max");
        Check.Eq(true, NumericInput.TryParseInt("-5", 0, 10, out vi), "int underrange clamps ok");
        Check.Eq(0, vi, "int underrange clamps to min");
        Check.Eq(true, NumericInput.TryParseInt("30", 1, 120, out vi), "int inrange untouched");
        Check.Eq(30, vi, "int inrange value");

        // -- TryParseFloat --
        float vf;
        Check.Eq(true, NumericInput.TryParseFloat("1.5", 0f, 5f, out vf), "float 1.5 ok");
        Check.Eq(1.5f, vf, "float 1.5 value");
        Check.Eq(false, NumericInput.TryParseFloat("abc", 0f, 5f, out vf), "float abc rejected");
        Check.Eq(true, NumericInput.TryParseFloat("7", 0f, 5f, out vf), "float integer-form ok");
        Check.Eq(5f, vf, "float 7 overrange clamps to max 5");
        Check.Eq(true, NumericInput.TryParseFloat(" 0.25 ", 0.05f, 0.25f, out vf), "float trim ok");
        Check.Eq(0.25f, vf, "float boundary value untouched");
        Check.Eq(false, NumericInput.TryParseFloat("NaN", 0f, 1f, out vf), "float NaN rejected");
        Check.Eq(false, NumericInput.TryParseFloat("Infinity", 0f, 1f, out vf), "float Infinity rejected");

        // -- RangeNote:面板 ⓘ tooltip 的范围/类型标注(玩家可见文案,InvariantCulture) --
        Check.Eq("[范围 0-999999,整数]", NumericInput.RangeNoteInt(0, 999999), "RangeNote int plain");
        Check.Eq("[范围 1-999999,整数]", NumericInput.RangeNoteInt(1, 999999), "RangeNote int min1");
        Check.Eq("[范围 -100-999,小数]", NumericInput.RangeNoteFloat(-100f, 999f), "RangeNote float negative min");
        Check.Eq("[范围 0-1,小数]", NumericInput.RangeNoteFloat(0f, 1f), "RangeNote float probability");
        Check.Eq("[范围 0.05-1,小数]", NumericInput.RangeNoteFloat(0.05f, 1f), "RangeNote float fractional min");
    }
}
