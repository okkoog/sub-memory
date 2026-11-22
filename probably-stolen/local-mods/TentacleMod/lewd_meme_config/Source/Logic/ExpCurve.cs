namespace LewdMemeConfig
{
    /// <summary>
    /// 触手本体升级经验公式(0.7.0 Task 9,纯函数):1.9.2 把经验表
    /// TentacleBody.EXP_NEED(static readonly int[],0.6.0 的 pools.maiden
    /// DataWrite 写入目标)替换为公式 <c>ExpNeedForLevel(int lv)</c>
    /// (lewd_meme_192 TentacleBody.cs:974-981 逐字):
    /// <code>lv &lt; 1 → lv = 1; return 100 + (lv-1)/3*100;</code>
    /// ——(lv-1)/divisor 为整数除法(向零截断)。本类是求值半边的单一事实源:
    /// main.exp_formula prefix 复刻体(LevelCapPatch)与测试(ExpCurveTests)
    /// 共用;三键 progress.exp_base / exp_divisor / exp_step 默认 100/3/100
    /// = 原版逐字零漂移。零游戏类型(Logic/ 通配进测试工程)。
    /// </summary>
    public static class ExpCurve
    {
        /// <summary>等级 lv 的升级所需经验:base + (lv-1)/divisor*step。
        /// lv &lt; 1 钳回 1(原版语义:返回基础值,不产生负步进);
        /// divisor 由面板 Define 保证 ≥ 1(除零防御归调用侧的面板范围)。</summary>
        public static int ExpNeed(int lv, int baseVal, int divisor, int step)
        {
            if (lv < 1) lv = 1;
            return baseVal + (lv - 1) / divisor * step;
        }
    }
}
