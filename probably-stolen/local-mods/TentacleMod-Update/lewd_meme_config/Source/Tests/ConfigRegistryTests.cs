using System;
using System.Collections.Generic;
using LewdMemeConfig;

public static class ConfigRegistryTests
{
    public static void Run()
    {
        var r = ConfigRegistry.CreateDefault();
        // 默认值（抽查 spec 权威值）
        Check.Eq(0.75f, r.Current<float>(CfgKeys.FemaleRewriteChance), "default female_rewrite_chance");
        Check.Eq(30, r.Current<int>(CfgKeys.GestationDays), "default gestation_days");
        Check.Eq(0, r.Current<int>(CfgKeys.GenderScope), "default gender_scope");
        Check.Eq(480, r.Current<int>(CfgKeys.UiReportWidth), "default ui.report_width");
        Check.Eq(1.2f, r.Current<float>(CfgKeys.UiReportDelay), "default ui.report_delay");
        // Review Focus 1: 类型不匹配抛(写入层共用 ValidateWrite,经玩家层探针)
        Check.Throws<InvalidCastException>(() => r.SetPlayerValue(CfgKeys.GestationDays, 1.5f), "type mismatch throws");
        // Review Focus 2: 未注册键
        Check.Throws<KeyNotFoundException>(() => r.Current<float>("no.such.key"), "unknown key Current throws");
        Check.Throws<KeyNotFoundException>(() => r.SetPlayerValue("no.such.key", 1), "unknown key SetPlayerValue throws");

        // ---- 三层取值链(spec §3:playerValues → mainModPanel → defaults)----
        var r4 = ConfigRegistry.CreateDefault();
        r4.SetPlayerValue(CfgKeys.ConceivePerSex + ".enabled", true);  // 0.5.0:开关默认关,玩家层用例显式开子项开关
        Check.Eq(0.01f, r4.Current<float>(CfgKeys.ConceivePerSex), "layer default");
        r4.SetMainModPanelProvider(CfgKeys.ConceivePerSex, () => 0.15f);        // 面板动态层
        Check.Eq(0.15f, r4.Current<float>(CfgKeys.ConceivePerSex), "panel provider beats default");
        r4.SetPlayerValue(CfgKeys.ConceivePerSex, 0.2f);                        // 玩家压过面板
        Check.Eq(0.2f, r4.Current<float>(CfgKeys.ConceivePerSex), "player beats panel provider");
        // 玩家撤(写回默认 = 未干预)→ 面板层
        r4.SetPlayerValue(CfgKeys.ConceivePerSex, 0.01f);
        Check.Eq(0.15f, r4.Current<float>(CfgKeys.ConceivePerSex), "player=default falls to panel");
        // provider 撤(null)→ 默认
        r4.SetMainModPanelProvider<float>(CfgKeys.ConceivePerSex, null);
        Check.Eq(0.01f, r4.Current<float>(CfgKeys.ConceivePerSex), "remove provider restores default");

        // provider 抛异常:Current 不抛,跳层退默认
        var r5 = ConfigRegistry.CreateDefault();
        r5.SetMainModPanelProvider<float>(CfgKeys.ConceivePerSex, () => throw new InvalidOperationException("boom"));
        Check.Eq(0.01f, r5.Current<float>(CfgKeys.ConceivePerSex), "provider exception falls to default");

        // provider 返回值不缓存:每次 Current 现求值
        var r6 = ConfigRegistry.CreateDefault();
        int calls = 0;
        r6.SetMainModPanelProvider(CfgKeys.ConceivePerSex, () => { calls++; return 0.15f; });
        r6.Current<float>(CfgKeys.ConceivePerSex);
        r6.Current<float>(CfgKeys.ConceivePerSex);
        Check.Eq(2, calls, "provider evaluated per read, not cached");

        // provider 写入校验与其他层一致:类型不符抛 InvalidCastException,未注册键抛 KeyNotFoundException
        Check.Throws<InvalidCastException>(
            () => r6.SetMainModPanelProvider(CfgKeys.GestationDays, () => 1.5f), "provider type mismatch throws");
        Check.Throws<KeyNotFoundException>(
            () => r6.SetMainModPanelProvider("no.such.key", () => 1), "provider unknown key throws");

        // provider 注入/移除按 key 触发 Changed(写入即触发,与其他层一致)
        var r7 = ConfigRegistry.CreateDefault();
        int firedPanel = 0; r7.Changed += k => { if (k == CfgKeys.ConceivePerSex) firedPanel++; };
        r7.SetMainModPanelProvider(CfgKeys.ConceivePerSex, () => 0.15f);
        r7.SetMainModPanelProvider<float>(CfgKeys.ConceivePerSex, null);
        Check.Eq(2, firedPanel, "provider set/remove fires Changed");

        // 玩家层写入即触发 Changed
        var r8 = ConfigRegistry.CreateDefault();
        int fired2 = 0; r8.Changed += k => { if (k == CfgKeys.PriceMilk) fired2++; };
        r8.SetPlayerValue(CfgKeys.PriceMilk, 300);
        Check.Eq(1, fired2, "player write fires Changed");

        // ---- Task 2(1.6.1):loot 键变更 + alarm/trap 17 新键注册与默认值 ----
        var rt = ConfigRegistry.CreateDefault();
        // loot:0.8.0 ScavMaidenBase/ScavGroups/LootThrottleSeconds 退役(官方接管/机制死),
        // 分层语义夹具换 MedShotChance(默认 0.1 = 1.6.1 字面量)
        Check.Eq(0.1f, rt.Current<float>(CfgKeys.MedShotChance), "default loot.med_shot_chance = 1.6.1 字面量(0.8.0 夹具键替换)");
        Check.Throws<KeyNotFoundException>(() => rt.Current<float>("loot.scav_maiden_per_level"),
            "scav_maiden_per_level 不再注册(1.6.1 移除)");
        Check.Throws<KeyNotFoundException>(() => rt.Current<float>("loot.scav_shot_chance"),
            "scav_shot_chance 不再注册(1.6.1 移除)");
        // alarm 9(int 8 + float 1)
        Check.Eq(75, rt.Current<int>(CfgKeys.AlarmPrice), "default alarm.price");
        Check.Eq(150, rt.Current<int>(CfgKeys.AlarmTentaclePrice), "default alarm.tentacle_price");
        Check.Eq(4, rt.Current<int>(CfgKeys.AlarmBroodSize), "default alarm.brood_size");
        Check.Eq(4, rt.Current<int>(CfgKeys.AlarmOutputSize), "default alarm.output_size");
        Check.Eq(1, rt.Current<int>(CfgKeys.AlarmOralPerNight), "default alarm.oral_per_night");
        Check.Eq(1, rt.Current<int>(CfgKeys.AlarmSexPerNight), "default alarm.sex_per_night");
        Check.Eq(1, rt.Current<int>(CfgKeys.AlarmAnalPerNight), "default alarm.anal_per_night");
        Check.Eq(1, rt.Current<int>(CfgKeys.AlarmSemenOutput), "default alarm.semen_output");
        Check.Eq(0.05f, rt.Current<float>(CfgKeys.AlarmNpcSellChance), "default alarm.npc_sell_chance");
        // trap 8(int 4 + float 3 + string 1)
        Check.Eq(100, rt.Current<int>(CfgKeys.TrapPrice), "default trap.price");
        Check.Eq(200, rt.Current<int>(CfgKeys.TrapTrapPrice), "default trap.trap_price");
        Check.Eq(12, rt.Current<int>(CfgKeys.TrapBroodSize), "default trap.brood_size");
        Check.Eq(12, rt.Current<int>(CfgKeys.TrapOutputSize), "default trap.output_size");
        Check.Eq("0.1,0.2,0.3", rt.Current<string>(CfgKeys.TrapTacticChances), "default trap.tactic_chances");
        Check.Eq(0.5f, rt.Current<float>(CfgKeys.TrapSemenChance), "default trap.semen_chance");
        Check.Eq(0.1f, rt.Current<float>(CfgKeys.TrapNpcSellChanceScav), "default trap.npc_sell_chance_scav");
        Check.Eq(0.05f, rt.Current<float>(CfgKeys.TrapNpcSellChanceThief), "default trap.npc_sell_chance_thief");
        // 类型锁:写入类型 ≠ 注册类型即抛(int/float/string 各一探针,经玩家层)
        Check.Throws<InvalidCastException>(() => rt.SetPlayerValue(CfgKeys.AlarmPrice, 0.5f),
            "alarm.price 注册为 int,float 写入拒");
        Check.Throws<InvalidCastException>(() => rt.SetPlayerValue(CfgKeys.TrapSemenChance, 1),
            "trap.semen_chance 注册为 float,int 写入拒");
        Check.Throws<InvalidCastException>(() => rt.SetPlayerValue(CfgKeys.TrapTacticChances, 1),
            "trap.tactic_chances 注册为 string,int 写入拒");

        // ---- Task 4(更多可配置项):全新键注册与默认值(0.8.0:throttle 键已退役)----
        var r3 = ConfigRegistry.CreateDefault();
        Check.Eq(999, r3.Current<int>("progress.level_cap"), "level_cap default 999(0.8.0:2.1.0 官方取消 5 级上限,999 = 不封顶等价)");
        Check.Eq(100, r3.Current<int>("corrupt.initial_charge"), "initial_charge default 100");

        // ---- Task 5:玩家层 string 写入(Input 文本框流转,语义对齐数值键)----
        var r9 = ConfigRegistry.CreateDefault();
        r9.SetPlayerValue(CfgKeys.MedKeywords + ".enabled", true);    // 0.5.0:开关默认关,string 玩家层用例显式开
        int firedStr = 0; r9.Changed += k => { if (k == CfgKeys.MedKeywords) firedStr++; };
        r9.SetPlayerValue(CfgKeys.MedKeywords, "med2");                       // ≠ 默认 → 写入
        Check.Eq("med2", r9.Current<string>(CfgKeys.MedKeywords), "string player write readable");
        Check.Eq(1, firedStr, "string player write fires Changed");
        r9.SetPlayerValue(CfgKeys.MedKeywords, "med,drug,pharma,chem");       // = 默认 → 移除层
        Check.Eq(true, !firedStr.Equals(0) && r9.Current<string>(CfgKeys.MedKeywords) == "med,drug,pharma,chem",
            "string player = default removes layer");
        Check.Throws<InvalidCastException>(() => r9.SetPlayerValue(CfgKeys.GestationDays, "30"),
            "string player write type mismatch throws");

        // ---- Task 3(开关体系,作者裁定修订:仅子项开关):只压玩家层,动态层穿透 ----
        var rs = ConfigRegistry.CreateDefault();
        Check.Eq(false, rs.Current<bool>(CfgKeys.PriceMilk + ".enabled"), "item switch default false");
        rs.SetPlayerValue(CfgKeys.PriceMilk, 300);
        Check.Eq(180, rs.Current<int>(CfgKeys.PriceMilk), "item switch off masks player layer");
        rs.SetPlayerValue(CfgKeys.PriceMilk + ".enabled", true);
        Check.Eq(300, rs.Current<int>(CfgKeys.PriceMilk), "item switch on: player layer visible");
        // 开关键豁免(防死锁):开关键自身的玩家层永不被压(无更上层开关可压它,锁定语义)
        rs.SetPlayerValue(CfgKeys.PriceMilk + ".enabled", false);
        Check.Eq(false, rs.Current<bool>(CfgKeys.PriceMilk + ".enabled"), "switch key exempt from masking");
        Check.Eq(180, rs.Current<int>(CfgKeys.PriceMilk), "switch off again masks player");
        // 无子项开关的键(Choice/Toggle):玩家层直通
        rs.SetPlayerValue(CfgKeys.GuestDisplayEnabled, false);
        Check.Eq(false, rs.Current<bool>(CfgKeys.GuestDisplayEnabled), "no-item-switch key visible directly");
        // 动态层不受开关管辖
        rs.SetMainModPanelProvider(CfgKeys.MedShotChance, () => 0.2f);
        rs.SetPlayerValue(CfgKeys.MedShotChance + ".enabled", false);
        Check.Eq(0.2f, rs.Current<float>(CfgKeys.MedShotChance), "mainmod panel layer ignores switches");
        // 关再开:玩家层值仍在(遮蔽不清除)
        rs.SetPlayerValue(CfgKeys.PriceMilk + ".enabled", true);
        Check.Eq(300, rs.Current<int>(CfgKeys.PriceMilk), "player value survives switch toggle cycle");

        // ---- R7 PlayerOrDefault(getter 接管前缀的防递归通道:玩家层/压层 → 默认,
        //      绝不触碰动态层——动态层 provider 即被接管 getter 本身,走全链必递归)----
        var rp = ConfigRegistry.CreateDefault();
        // ① 无干预 + 无动态层 → 默认 0.05
        Check.Eq(0.1f, rp.PlayerOrDefault<float>(CfgKeys.MedShotChance), "player-or-default: no layer → default");
        // ② 动态层存在也不读(与 Current 的核心差异):provider 若是被接管 getter,
        //    走 Current 全链 = getter → Current → 动态层 → getter 无限递归
        int providerCalls = 0;
        rp.SetMainModPanelProvider(CfgKeys.MedShotChance, () => { providerCalls++; return 0.2f; });
        Check.Eq(0.1f, rp.PlayerOrDefault<float>(CfgKeys.MedShotChance), "player-or-default: skips dynamic layer");
        Check.Eq(0, providerCalls, "player-or-default: dynamic provider not invoked");
        Check.Eq(0.2f, rp.Current<float>(CfgKeys.MedShotChance), "Current still reads dynamic layer");
        // ③ 开关开 + 玩家值 → 玩家层(优先于动态层,与 Current 同)
        rp.SetPlayerValue(CfgKeys.MedShotChance + ".enabled", true);
        rp.SetPlayerValue(CfgKeys.MedShotChance, 0.4f);
        Check.Eq(0.4f, rp.PlayerOrDefault<float>(CfgKeys.MedShotChance), "player-or-default: player layer when switch on");
        // ④ 开关关 → 压玩家层退默认(不退动态层——getter 前缀在开关关时已卸载,
        //    此通道读数仅用于防御,默认 0.05 = getter 的 catch 回退值,零漂移)
        rp.SetPlayerValue(CfgKeys.MedShotChance + ".enabled", false);
        Check.Eq(0.1f, rp.PlayerOrDefault<float>(CfgKeys.MedShotChance), "player-or-default: switch off → default (not dynamic)");
        // ⑤ 未注册键与 Current 同契约
        Check.Throws<KeyNotFoundException>(() => rp.PlayerOrDefault<float>("no.such.key"),
            "player-or-default unknown key throws");
    }
}
