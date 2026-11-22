using LewdMemeConfig;

// 0.7.0 Task 9:升级经验公式纯函数(1.9.2 TentacleBody.ExpNeedForLevel :974-981
// 逐字复刻的求值半边;prefix 复刻体与面板三键共用此实现):
//   ExpNeed(lv) = base + (lv-1)/divisor*step   —— (lv-1)/divisor 为整数除法
// 原版字面量:100 + (lv-1)/3*100(lv<1 钳回 1)。三键默认 = 原版逐字零漂移,
// lv1=100 / lv4=200 / lv7=300 / lv10=400(每 3 级 +100);断言锁整除截断语义
// (lv5=200、lv6=200——4/3 与 5/3 同为 1)与 lv0/负数钳制。
public static class ExpCurveTests
{
    public static void Run()
    {
        // —— 默认三键(100/3/100)= 1.9.2 原版逐档 ——
        Check.Eq(100, ExpCurve.ExpNeed(1, 100, 3, 100), "lv1 = 100(默认键 = 原版)");
        Check.Eq(100, ExpCurve.ExpNeed(2, 100, 3, 100), "lv2 = 100(1/3 截断为 0)");
        Check.Eq(100, ExpCurve.ExpNeed(3, 100, 3, 100), "lv3 = 100(2/3 截断为 0)");
        Check.Eq(200, ExpCurve.ExpNeed(4, 100, 3, 100), "lv4 = 200(3/3 = 1 档)");
        Check.Eq(200, ExpCurve.ExpNeed(5, 100, 3, 100), "lv5 = 200(4/3 截断为 1)");
        Check.Eq(200, ExpCurve.ExpNeed(6, 100, 3, 100), "lv6 = 200(5/3 截断为 1)");
        Check.Eq(300, ExpCurve.ExpNeed(7, 100, 3, 100), "lv7 = 300(6/3 = 2 档)");
        Check.Eq(400, ExpCurve.ExpNeed(10, 100, 3, 100), "lv10 = 400(9/3 = 3 档)");

        // —— lv 钳制:原版 lv<1 → lv=1(返回基础值,不产生负步进)——
        Check.Eq(100, ExpCurve.ExpNeed(0, 100, 3, 100), "lv0 钳回 lv1 = 基础值");
        Check.Eq(100, ExpCurve.ExpNeed(-5, 100, 3, 100), "负 lv 钳回 lv1 = 基础值");

        // —— 自定义三键:公式形状(base + 档数×step)——
        Check.Eq(50, ExpCurve.ExpNeed(1, 50, 2, 30), "自定义: lv1 = base");
        Check.Eq(80, ExpCurve.ExpNeed(3, 50, 2, 30), "自定义: lv3 = 50+1×30");
        Check.Eq(110, ExpCurve.ExpNeed(5, 50, 2, 30), "自定义: lv5 = 50+2×30(4/2=2 档)");
        // step = 0:恒基础值(面板范围 0-1000 合法,平曲线)
        Check.Eq(100, ExpCurve.ExpNeed(99, 100, 3, 0), "step=0 → 恒基础值(平曲线)");
        // divisor = 1:每级 +step(线性)
        Check.Eq(100 + 9 * 50, ExpCurve.ExpNeed(10, 100, 1, 50), "divisor=1 → 每级线性 +step");

        // —— 注册表默认值(brief Step 1:interact_threshold 默认 60 / 公式三键默认)——
        var cfg = ConfigRegistry.CreateDefault();
        Check.Eq(100, cfg.Current<int>(CfgKeys.ExpBase), "progress.exp_base 默认 100");
        Check.Eq(3, cfg.Current<int>(CfgKeys.ExpDivisor), "progress.exp_divisor 默认 3");
        Check.Eq(100, cfg.Current<int>(CfgKeys.ExpStep), "progress.exp_step 默认 100");
        Check.Eq(60, cfg.Current<int>(CfgKeys.InteractThreshold), "maiden.interact_threshold 默认 60");
        Check.Eq(true, cfg.Current<bool>(CfgKeys.InitialVirgin), "maiden.initial_virgin 默认 true(= 1.9.2 无条件处女)");
        Check.Eq(false, cfg.ContainsKey("progress.exp_per_level"), "exp_per_level 键已废弃不再注册");
    }
}
