namespace LewdMemeConfig
{
    /// <summary>
    /// 路由审计期望态的纯函数内核(F1 抽出,Logic/ 通配进测试工程):
    /// SelfTestRunner.ExpectedState 的判定半边——载体过滤(DataWrite/WriteThrough
    /// 不进安装器,恒 NotRegistered)、让位(无委托不登记)、R5 豁免 × 版本压制
    /// (豁免单元的 install 委托在让位分支不产出、HookRuntime ③ 登记期跳过;Router
    /// 仍可能裁 VersionBlocked[R5 版本压过让位],此时实际态 NotRegistered,期望
    /// 须与登记过滤对齐——否则主 mod 版本不命中 × remake/IPH 在场组合下审计假红)、
    /// SubModHook 目标缺席(R11-F1:TargetMod 缺席的单元 ③ 登记期拦截不注册,
    /// 期望态 NotRegistered 与实际态对齐——否则跨族键[主面板 Define、消费单元
    /// 含缺席目标的 SubModHook;历史形态 = nightly.semen_chance_cap ×
    /// remake.semen_formula,ST4 单元注销后跨族键终结,拦截语义保留]
    /// 开关拨动触发对账 → install 委托内 ModReflect.Types(对方) 缺席必抛 → Dead
    /// → 审计假红,0x0f 根因)、开关聚合(任一消费键开 → Installed、全关 →
    /// Absent)。纯输入(单元定义 / 裁决 / 开关聚合 / 豁免旗标 / 缺席旗标),
    /// 零游戏依赖可单测;快照采集(RouteSnapshot / R5ExemptUnits /
    /// AbsentTargetUnits / SwitchOnOf)归 SelfTestRunner 的包装半边。
    /// </summary>
    internal static class RouteExpect
    {
        /// <summary>期望态字符串(与 HookState.ToString 对账口径;「未装配」由
        /// 包装半边的 Ready 检查承担,纯函数只见已装配裁决)。targetAbsent 仅对
        /// SubModHook 生效(MainHook 目标缺席走装配层 resolver 语义,非登记拦截),
        /// 缺省 false = 既有调用(测试回归行)零改动。</summary>
        internal static string Expected(HookUnitDef def, RouteDecision decision,
            bool anySwitchOn, bool r5Exempt, bool targetAbsent = false)
        {
            if (def.Kind != CarrierKind.MainHook && def.Kind != CarrierKind.SubModHook)
                return "NotRegistered";
            if (decision == RouteDecision.Yield) return "NotRegistered";
            // R11-F1:SubModHook 目标缺席 → HookRuntime ③ 不注册(物理零 hook),
            // 实际态恒 NotRegistered,期望态对齐(开关开也一样——缺席单元无装配面)
            if (targetAbsent && def.Kind == CarrierKind.SubModHook) return "NotRegistered";
            if (decision == RouteDecision.VersionBlocked)
                return r5Exempt ? "NotRegistered" : "VersionBlocked";
            return anySwitchOn ? "Installed" : "Absent";
        }
    }
}
