namespace LewdMemeConfig
{
    /// <summary>
    /// gender_scope(client.gender_scope,int 0–3)面向玩家面板的中文档位标签。
    /// 下标 = 档位值;面板 choices 下拉与 json 持久化的都是标签字符串,
    /// 与注册表 int 值的互转集中在此(spec 2026-11-11 §5"顾客"分区)。
    /// 纯逻辑,不依赖游戏类型。
    /// </summary>
    public static class PlayerChoices
    {
        /// <summary>下标 = 档位:0 关闭 / 1 仅普通顾客 / 2 顾客+特殊顾客 / 3 全体人类角色。</summary>
        public static readonly string[] Labels =
            { "关闭", "仅普通顾客", "顾客+特殊顾客", "全体人类角色" };

        /// <summary>标签 → 档位;未知或 null → 0(玩家手改 json 出现乱值时安全回退)。</summary>
        public static int ToScope(string label)
        {
            if (label == null) return 0;
            for (int i = 0; i < Labels.Length; i++)
                if (Labels[i] == label) return i;
            return 0;
        }

        /// <summary>档位 → 标签;越界 clamp 到 [0, 3]。</summary>
        public static string ToLabel(int scope)
        {
            if (scope < 0) scope = 0;
            if (scope > Labels.Length - 1) scope = Labels.Length - 1;
            return Labels[scope];
        }
    }
}
