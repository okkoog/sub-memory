using LewdMemeConfig;

// Task 3:gender_scope 面板档位中文标签 ↔ int 档位映射(Review Focus 4:乱值回退)。
public static class PlayerChoicesTests
{
    public static void Run()
    {
        Check.Eq(4, PlayerChoices.Labels.Length, "4 scope labels");
        Check.Eq(0, PlayerChoices.ToScope("关闭"), "label off");
        Check.Eq(1, PlayerChoices.ToScope("仅普通顾客"), "label clients");
        Check.Eq(2, PlayerChoices.ToScope("顾客+特殊顾客"), "label special");
        Check.Eq(3, PlayerChoices.ToScope("全体人类角色"), "label all");
        Check.Eq(0, PlayerChoices.ToScope("乱七八糟"), "unknown label falls back to 0");
        Check.Eq(0, PlayerChoices.ToScope(null), "null label falls back to 0");
        Check.Eq("全体人类角色", PlayerChoices.ToLabel(3), "label for 3");
        Check.Eq("关闭", PlayerChoices.ToLabel(-1), "clamp low");
        Check.Eq("全体人类角色", PlayerChoices.ToLabel(4), "clamp high");
    }
}
