namespace LewdMemeConfig
{
    /// <summary>
    /// 原键开关态分派(F1,0.6.0 验收 R1 热修复):SwitchOn 的旧实现经
    /// KeyNotFoundException 回退 <c>Current&lt;bool&gt;(key)</c>——对无子项开关的
    /// 非布尔注册表键是 (bool)(object)int → InvalidCastException,同一根因炸出
    /// 三症状(ReconcileAll 在 catalog #2 中止 → 装配全灭;订阅在 ReconcileAll 后
    /// → 热装卸失能;DebugPanel 期望态每帧求值 → GUILayout 中断只剩窗口框)。
    /// 显式分派,零异常驱动:
    ///  1. 有子项开关键(89 面板值键[T8/T9:virgin_chance 废弃、exp_per_level
    ///     废弃、+exp 三键/interact_threshold,93 值 - 4 无开关键]+ 91 镜像键
    ///     [ST5 后实测,与 ConfigRegistry.CreateDefault 注释同口径:36 WriteThrough
    ///     + 50 SubModHook + R10 的 iph.attr_* 5 MainHook;0.7.1 拾取器两键
    ///     SelfSwitch 不在此——Toggle 走 1b、EnumKeyCode 纯参数不驱动],
    ///     ConfigRegistry 查询)→ 读开关
    ///     <c>key.enabled</c> 的 bool(默认关);
    ///  1b. <c>brood.tag_picker_enabled</c>(0.7.1 拾取器:SubModHook 的 Toggle
    ///     自开关,表驱动分流——枚举参数键 EnumKeyCode 不经此条)→ 布尔自身;
    ///  2. <c>gender_scope</c>(Choice,无子开关)→ 档位 int ≠ 0;
    ///  3. <c>guest.display_enabled</c> / <c>names.use_custom_file</c>(Toggle,
    ///     无子开关)→ 布尔自身;<c>maiden.initial_virgin</c>(Toggle,无子开关,
    ///     0.7.0 T9)→ 布尔自身**取反**(默认 true = 1.9.2 原版无条件处女 →
    ///     IsOn false 零参与;false = 接管装配,postfix Disable 双处女 tag——
    ///     反向语义:值偏离默认才是接管意图,与「默认零 patch」红线一致);
    ///  4. 其余 = 9 个布局尺寸键(layout.brood_size / output_size / corrupt_cols /
    ///     tool_cols / tabs_per_level + alarm.brood_size / output_size +
    ///     trap.brood_size / output_size——注册表键但不上面板,不可玩家干预)→
    ///     恒 false:不驱动装配,搭同单元其他键的车(main.build_window /
    ///     main.create 这类全布局键单元恒不装,与 0.5.x 语义一致:键不上面板 =
    ///     复刻体读默认 = 恒 no-op)。
    /// 纯函数(只依赖 ConfigRegistry):HookRuntime.SwitchOnFor(装配)、
    /// SelfTestRunner(审计期望态/Tier3 拨开)与 SwitchOnContractTests(契约测试)
    /// 三方共用,消平行复刻(终审 parked ⑤ 收敛)。
    /// </summary>
    internal static class SwitchDispatch
    {
        /// <summary>原键的期望开关态(分派单一事实源;registry 由调用方保证非 null)。</summary>
        internal static bool IsOn(ConfigRegistry registry, string key)
        {
            string switchKey = PanelKeys.ItemSwitchKey(key);
            if (registry.ContainsKey(switchKey))
                return registry.Current<bool>(switchKey);
            // 0.7.1 拾取器:SubModHook 的 Toggle 自开关键(bool 自身即开关,
            // guest.display_enabled 同语义——那三键属主面板键面走下方显式枚举,
            // 此族走表驱动分流,键名不散写;EnumKeyCode 纯参数键不经此条 → 恒 false)
            if (FamilyPanelKeys.TryGet(key, out FamilyPanelEntry self)
                && ConfigRegistry.SelfSwitchForm(self)
                && self.Kind == PanelKind.Toggle)
                return registry.Current<bool>(key);
            if (key == CfgKeys.GenderScope)
                return registry.Current<int>(key) != 0;
            if (key == CfgKeys.GuestDisplayEnabled || key == CfgKeys.NamesUseCustomFile)
                return registry.Current<bool>(key);
            if (key == CfgKeys.InitialVirgin)
                return !registry.Current<bool>(key); // T9 反向:默认 true=原版零参与,false=接管
            return false; // 9 布局尺寸键等无开关非布尔的注册表键:不驱动装配
        }
    }
}
