using System;
using System.Collections.Generic;
using LewdMemeConfig;

// SwitchOn 契约测试(F5,0.6.0 验收 R1 热修复——替代 SwitchOnRepro 诊断打印形态):
// 分派单一事实源 = Logic/SwitchDispatch.cs(HookRuntime.SwitchOnFor 的实现体,
// 装配/审计/拨开三方共用),此处对 ConfigRegistry 直测四条分派 + HookCatalog
// 全键零异常遍历,锁定 0x01.log 根因(9 布局键经 KNF 回退 (bool)(object)int →
// InvalidCastException)不再回归。
public static class SwitchOnContractTests
{
    public static void Run()
    {
        var registry = ConfigRegistry.CreateDefault();

        // ① HookCatalog.Units 全部键(含镜像键)经 SwitchDispatch.IsOn 求值:
        //    零异常且默认全关(旧实现 9 键抛 ICE)
        var seen = new HashSet<string>(StringComparer.Ordinal);
        int keyCount = 0;
        foreach (var def in HookCatalog.Units)
            foreach (var key in def.Keys)
            {
                if (!seen.Add(key)) continue;
                keyCount++;
                bool on = true, threw = false;
                try { on = SwitchDispatch.IsOn(registry, key); }
                catch { threw = true; }
                Check.Eq(false, threw, "switch① 全键求值零异常: " + key);
                // 默认态:唯一例外 = guest.display_enabled(布尔自开关,默认 true,④ 专测)
                bool expectOn = key == CfgKeys.GuestDisplayEnabled;
                Check.Eq(expectOn, on, "switch① 默认态 " + (expectOn ? "true" : "false") + ": " + key);
            }
        Check.Eq(true, keyCount > 100, "switch① 目录键并集规模合理(实际 " + keyCount + ")");

        // ② 9 布局尺寸键恒 false(无开关不上面板,不驱动装配——搭同单元其他键的车)
        string[] layoutKeys =
        {
            CfgKeys.BroodSize, CfgKeys.OutputSize, CfgKeys.CorruptCols,
            CfgKeys.ToolCols, CfgKeys.TabsPerLevel,
            CfgKeys.AlarmBroodSize, CfgKeys.AlarmOutputSize,
            CfgKeys.TrapBroodSize, CfgKeys.TrapOutputSize,
        };
        foreach (string k in layoutKeys)
            Check.Eq(false, SwitchDispatch.IsOn(registry, k), "switch② 布局键恒 false: " + k);

        // ③ gender_scope(Choice 无子开关):默认 0 → false;玩家层 2 → true
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.GenderScope),
            "switch③ gender_scope 默认 0 → false");
        registry.SetPlayerValue(CfgKeys.GenderScope, 2);
        Check.Eq(true, SwitchDispatch.IsOn(registry, CfgKeys.GenderScope),
            "switch③ gender_scope 玩家层 2 → true");
        registry.SetPlayerValue(CfgKeys.GenderScope, 0);

        // ④ 布尔自开关键(Toggle 无子开关):bool 直读;T9 增 initial_virgin
        //    反向分派(默认 true = 1.9.2 原版无条件处女 → IsOn false 零参与;
        //    false = 接管装配,postfix Disable 双处女 tag)
        Check.Eq(true, SwitchDispatch.IsOn(registry, CfgKeys.GuestDisplayEnabled),
            "switch④ guest.display_enabled 默认 true");
        registry.SetPlayerValue(CfgKeys.GuestDisplayEnabled, false);
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.GuestDisplayEnabled),
            "switch④ guest.display_enabled false → false");
        registry.SetPlayerValue(CfgKeys.GuestDisplayEnabled, true);
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.NamesUseCustomFile),
            "switch④ names.use_custom_file 默认 false");
        registry.SetPlayerValue(CfgKeys.NamesUseCustomFile, true);
        Check.Eq(true, SwitchDispatch.IsOn(registry, CfgKeys.NamesUseCustomFile),
            "switch④ names.use_custom_file true → true");
        registry.SetPlayerValue(CfgKeys.NamesUseCustomFile, false);
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.InitialVirgin),
            "switch④ initial_virgin 默认 true → 反向 false(原版行为零参与)");
        registry.SetPlayerValue(CfgKeys.InitialVirgin, false);
        Check.Eq(true, SwitchDispatch.IsOn(registry, CfgKeys.InitialVirgin),
            "switch④ initial_virgin false → 反向 true(接管装配)");
        registry.SetPlayerValue(CfgKeys.InitialVirgin, true);
        // 0.7.1 拾取器:SubModHook 的 Toggle 自开关(表驱动分流,键名不散写;
        // guest.display_enabled 同语义但属主面板键面走显式枚举)——bool 直读
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.BroodTagPickerEnabled),
            "switch④ 拾取器自开关默认 false(零宿主零轮询)");
        registry.SetPlayerValue(CfgKeys.BroodTagPickerEnabled, true);
        Check.Eq(true, SwitchDispatch.IsOn(registry, CfgKeys.BroodTagPickerEnabled),
            "switch④ 拾取器自开关 true → true(装宿主)");
        registry.SetPlayerValue(CfgKeys.BroodTagPickerEnabled, false);
        // 拾取键位是纯参数(KeyCode 名串,不驱动装配)——IsOn 恒 false
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.BroodTagPickerKey),
            "switch④ 拾取键位参数键恒 false(不驱动装配)");

        // ⑤ 有子项开关键:随开关(主面板值键 + 镜像键同路径)
        // ST4:镜像样本换 remake.fatigue_scale(product_chance 已撤写透)
        Check.Eq(false, SwitchDispatch.IsOn(registry, CfgKeys.PriceMilk),
            "switch⑤ price.milk 开关默认关 → false");
        registry.SetPlayerValue(PanelKeys.ItemSwitchKey(CfgKeys.PriceMilk), true);
        Check.Eq(true, SwitchDispatch.IsOn(registry, CfgKeys.PriceMilk),
            "switch⑤ price.milk 开关开 → true");
        registry.SetPlayerValue(PanelKeys.ItemSwitchKey(CfgKeys.PriceMilk), false);
        Check.Eq(false, SwitchDispatch.IsOn(registry, "remake.fatigue_scale"),
            "switch⑤ 镜像键 remake.fatigue_scale 默认关 → false");
        registry.SetPlayerValue(PanelKeys.ItemSwitchKey("remake.fatigue_scale"), true);
        Check.Eq(true, SwitchDispatch.IsOn(registry, "remake.fatigue_scale"),
            "switch⑤ 镜像键开关开 → true");
        registry.SetPlayerValue(PanelKeys.ItemSwitchKey("remake.fatigue_scale"), false);

        // ⑥ ContainsKey 契约(F1):defaults 查询——布局键有键无开关、未注册键 false
        Check.Eq(true, registry.ContainsKey(CfgKeys.BroodSize), "switch⑥ 布局键已注册");
        Check.Eq(false, registry.ContainsKey(PanelKeys.ItemSwitchKey(CfgKeys.BroodSize)),
            "switch⑥ 布局键无子项开关");
        Check.Eq(true, registry.ContainsKey(PanelKeys.ItemSwitchKey(CfgKeys.PriceMilk)),
            "switch⑥ 面板值键有子项开关");
        Check.Eq(false, registry.ContainsKey(PanelKeys.ItemSwitchKey(CfgKeys.BroodTagPickerEnabled)),
            "switch⑥ 拾取器自开关无子项开关(0.7.1)");
        Check.Eq(false, registry.ContainsKey("no.such.key"), "switch⑥ 未注册键 false");
    }
}
