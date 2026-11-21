using System;
using System.Collections.Generic;
using LewdMemeConfig;

// Task 3:HookCatalog 钩子组键位表一致性锁定(brief 四条)。唯一事实源 =
// Source/Patches/*.cs 各 installs.Add 条目的 patch 方法体引用的 CfgKeys 成员,四条:
//  ① 每个 unit 的 Keys ⊆ ConfigRegistry.CreateDefault() 注册键(且非空、去重);
//  ② PanelKeys 每个值条目键,恰属于 ≥1 unit 的 Keys 或在 HookCatalog.DataOnlyKeys
//     显式排除(DataOnlyKeys 自身不得与 unit 键重叠、必须是面板值键);
//  ③ 每个 ItemSwitchKey(89 个子项开关)都能反查到其原键的归属 unit;
//  ④ 无重复 Id。
// 另钉 brief 三行范例的精度(0.7.0 Task 4 拆分后):main.nightly_transpile =
// TranspilePlan 表内 methodId 同名注入点的全部 14 键(1.9.2 NightlyTick 数值面;
// 旧 main.nightly_tick 22 键扣除 milk 2 → 乳汁面单元、daughter_value 1 →
// produce_daughter、min 系 5 → DataOnlyKeys[IL 折叠无消费点])、
// main.nightly_milk_value = 乳汁 2 键(同 NightlyTick,让位语义独立自成单元)、
// main.level_cap = {LevelCap}、pools.maiden_jobs = {Jobs}、pools.maiden_fbb =
// 阵营/体型/胸部三池 3 键(R9 拆分;0.7.0 T9:pools.maiden 删除——1.9.2 已删
// EXP_NEED 静态数组,经验表改公式三键)。
// 单元计数锁定:15 个 patch 类 63 个 installs.Add 调用 → 63 个逻辑条目
// (46 MainHook + 17 SubModHook;4 个 DataWrite 无 install 委托,装配期直写
// 不进映射;让位豁免分支零产出)——现值 46 MainHook + 4 DataWrite +
// 19 WriteThrough + 17 SubModHook = 86;0.6.0 R9(2026-11-14)pools.maiden_traits
// 拆 2(jobs/fbb,DataWrite 4→5)+ IPH 写透 16(wt.iph.*,WriteThrough 19→35)
// = 103,其中 2 个 game.* 单元(TargetMod=null)为游戏本体补丁;
// M4b 追加 19 个 WriteThrough 单元(总 66,TargetMod = remake/Brood,键集 =
// FamilyPanelKeys 镜像 OurKey 集,由 FamilyPanelKeysTests ⑤ 锁定);M5a 追加
// 9 个 SubModHook 单元(总 75,TargetMod = brothel,键集 = CfgKeys.Brothel*
// 常量全集,由本测试 ⑤' 锁定双射);M5b 追加 3 个 tearl.* SubModHook 单元
// (总 78,TargetMod = tearl,键集 = CfgKeys.Tearl* 常量全集,同由 ⑤' 锁定
// 双射);Task 14b 追加 1 个 main.printer 单元(总 79,键集 = CfgKeys.Printer*
// 常量全集,五键共用一单元、六接管点在同一 install 委托);0.6.0 R4 追加
// 4 个 remake.* SubModHook 单元(总 83,TargetMod = remake,键集 = CfgKeys.
// Remake* 常量全集;乳汁单价三键由 milk_items/milk_production 双单元共享,
// ⑤' 白名单放行——两处字面量同值,LevelCap 双单元先例);0.6.0 R7 追加
// 3 单元(总 86):main.loot_maiden_chance(MainHook,拾荒概率 getter 接管,
// 与 main.on_loot_rolled 双消费 loot.scav_maiden_base——键级聚合下任一 Own
// 即生效,ScavMaidenBase 双消费白名单放行)+ pools.maiden_traits(DataWrite,
// 四池拆分,TargetMod = lewd_meme 不进 ⑤' 家族前缀计数)+ remake.ovulation_price
// (SubModHook,TargetMod = remake,⑤' remake 键计数 +1)。R9:maiden_traits
// 二次拆分为 pools.maiden_jobs(恒 Own)+ pools.maiden_fbb(让位 IPH),
// DataWrite 4→5;另 +16 个 wt.iph.* 写透单元。
// 0.6.0 R10(2026-11-14)追加 1 单元(总 104):main.maiden_attrs_iph
// (MainHook,**TargetMod = FamilyMods.Iph**——首个非 lewd_meme 目标的
// MainHook 单元,IPH 属性五键 postfix 共存补丁,三态治理见 HookCatalog 注释)。
// 0.6.0 R12(2026-11-14)追加 1 单元(总 105):brood.perfume_mod
// (SubModHook,TargetMod = Brood——首个 brood.* 接管单元,信息素两键
// postfix 等价恢复[WeakenBasePerfume 进场重设],SubModHook 17 → 18)。
// 0.7.0 Task 4(2026-11-17):main.nightly_tick 复刻体单元退役,拆
// main.nightly_transpile + main.nightly_milk_value(总 106,MainHook 47 → 48);
// min 系 5 键(sex_per_level_min/oral_min_base/oral_per_level_min/anal_min_base/
// anal_per_level_min)1.9.2 编译期折叠出 IL 消费点 → DataOnlyKeys(面板行保留)。
// 0.7.0 Task 5(2026-11-17):main.try_corrupt 复刻体单元退役 → main.corrupt_cost
// transpiler 单元(id 与 TranspilePlan methodId 同名;单元改形态不增删,总 106 /
// MainHook 48 不变);whitelist/blacklist 两行为型键不随迁,载体 =
// main.is_corruptible 复刻体(T2 C 节裁决)。
// 0.7.0 Task 6(2026-11-17):main.on_semen_used 复刻体退役 → transpiler 单元
// (id 不变,与 TranspilePlan 表内 methodId 同名;Keys 3 → 2,单元改形态不增删)。
// 0.7.0 Task 7(2026-11-17):Create 系复刻体退役(alarm/trap/printer/layout
// 十单元改形态不增删,总 106 / MainHook 48 不变)——价格六键 postfix 化
// (Create* 尾 ApplyCsv 之后恒胜,方法体归 ValuePostfixPatches 集中)、数值键
// transpile 化(methodId 与单元 id 同名对齐);alarm_nightly_once Keys 4 → 3
// (semen_output → DataOnlyKeys,T2 F 节放弃键)、main.create Keys 5 → 1
// (布局四键归 build_window);printer 三 methodId 保持 T3 名(grids/
// nightly_transpile/progress,恒 Own 无让位,Route 快照查不到时保守全应用)。
// 0.7.0 Task 8(2026-11-17):create_maiden 拆段(总 108 / MainHook 50)——
// 数值段 attr 五键 → main.maiden_roll、faction_base → main.maiden_faction_base
// 两新 transpiler 单元;virgin_chance 键全链废弃(面板值键 90 → 89)。
// 0.7.0 Task 9(2026-11-17):A 组真新增三单元 + pools.maiden 删除(总 110 /
// MainHook 53 / DataWrite 4)——main.interact_threshold(prefix 复刻
// ItemFactory.ComputeInteractMax 四阈值 ≥60 单键)、main.exp_formula(prefix
// 复刻 TentacleBody.ExpNeedForLevel 公式三键,100+(lv-1)/3*100 整除截断)、
// main.initial_virgin(postfix on CreateMaidenOfFaction,off 时 Disable 双
// 处女 tag,默认 true = 零参与);pools.maiden 删除(1.9.2 已删 EXP_NEED 静态
// 数组,经验公式化);main.add_body_exp 复刻体退役 → 改参 prefix(Keys 3 → 1
// 仅 ExpMultiplier);progress.exp_per_level 键全链废弃(面板值键 89 → 93)。
// ST 1.6.1 Task 4(2026-11-17):remake.semen_formula(SemenProduction 类删除)
// + remake.alarm_activity(AddAlarm* 三方法随疲劳门禁重写消失)两单元注销
// (109 = 110 − 2 + 1;SubModHook 18 → 16);5 差分键全链废弃;semen_chance_cap
// 跨族消费[R7]终结、回归 main.nightly_transpile 唯一消费;写透 remake
// 7−3+4 = 8(撤 product_chance/ovulation_guaranteed/virgin_avoidance,
// 增桶装 4 键),WriteThrough 35 → 36——单元总数 110 → 109(brief 的「110−2
// = 108」漏算了每镜像键一单元原则下写透 −3+4 的净 +1)。
// ST 1.6.1 Task 5(2026-11-17):疲劳核心六键 +2 单元(111 = 109 + 2;
// SubModHook 16 → 18)——remake.fatigue_value_mul(ValueMul 售价四档,
// prefix 复刻整方法)+ remake.fatigue_nightly_recover(NightlyGain 夜间
// 基数 + RecoverExhausted 透支恢复,prefix 双挂);⑤' remake 键 14 → 20、
// SubModHook 键集 44 → 50。
// 0.7.1 拾取器(2026-11-18):+1 单元 brood.tag_picker(总 112;SubModHook
// 18 → 19)——Brood 苗床标签编辑器「选中」方式替换,Kind = SubModHook 但
// **无 Harmony patch**(宿主生命周期单元:install 委托 = Brood 类型装配期
// 一次解析 + 建 MonoBehaviour 宿主,返回空 MethodBase[] 空记账——Uninstall
// 遍历 no-op,宿主销毁挂 HookRuntime.Report 迁移回调 + Shutdown;四 Kind
// 中 SubModHook 的装配路径与本单元需求完全同构:③ 登记期 TargetMod 缺席
// 拦截[R11-F1]、VersionBlocked 压制[绑定 0.3.31]、ReconcileAll 开关对账、
// SetDesired 热装卸、Dead 粘滞 + off→on 重试、自检路由审计自动覆盖;
// DataWrite 是装配期动作无开关对账、MainHook/WriteThrough 语义均不符);
// Keys 两键:tag_picker_enabled = 自开关 Toggle(SwitchDispatch 表驱动
// bool 自身,不派生 .enabled)、tag_picker_key = KeyCode 参数(不驱动装配,
// 宿主 Update 热读);⑤' brood.* 键 2 → 4、SubModHook 键集 50 → 52。
public static class HookCatalogTests
{
    public static void Run()
    {
        var cfg = ConfigRegistry.CreateDefault();

        // —— ① + ④:键注册性 / 非空 / 去重 / Id 唯一 / TargetMod 形态 ——
        int mainHook = 0, dataWrite = 0, writeThrough = 0, subModHook = 0, gameUnits = 0;
        var ids = new HashSet<string>();
        var covered = new HashSet<string>();
        foreach (var u in HookCatalog.Units)
        {
            Check.Eq(true, ids.Add(u.Id), "④ unique unit id: " + u.Id);
            Check.Eq(true, u.Keys != null && u.Keys.Length > 0, "① unit has keys: " + u.Id);
            Check.Eq(u.Keys.Length, new HashSet<string>(u.Keys).Count, "① unit keys deduplicated: " + u.Id);
            foreach (var k in u.Keys)
            {
                Check.Eq(true, Registered(cfg, k), "① key registered (" + u.Id + "): " + k);
                covered.Add(k);
            }
            if (u.Kind == CarrierKind.MainHook) mainHook++;
            if (u.Kind == CarrierKind.DataWrite) dataWrite++;
            if (u.Kind == CarrierKind.WriteThrough) writeThrough++;
            if (u.Kind == CarrierKind.SubModHook) subModHook++;
            if (u.Id.StartsWith("game.", StringComparison.Ordinal))
            {
                gameUnits++;
                Check.Eq(null, u.TargetMod, "① game unit has no TargetMod: " + u.Id);
            }
            else if (u.Kind == CarrierKind.WriteThrough)
            {
                // M4b 写透单元:TargetMod = 对方子 mod(FamilyMods 常量,Task 3 时
                // parked 的「允许非 lewd_meme 目标」随本任务落地)。
                Check.Eq(true, u.TargetMod == FamilyMods.Remake || u.TargetMod == FamilyMods.Brood
                    || u.TargetMod == FamilyMods.Iph,
                    "① write-through unit targets family sub-mod: " + u.Id);
            }
            else if (u.Kind == CarrierKind.SubModHook)
            {
                // M5a/M5b/R4/R12 接管单元:TargetMod = brothel/tearl/remake/brood
                // 按条目前缀分流(版本压制查询键;无让位规则)
                string expectMod = u.Id.StartsWith("tearl.", StringComparison.Ordinal)
                    ? FamilyMods.Tearl
                    : u.Id.StartsWith("remake.", StringComparison.Ordinal)
                    ? FamilyMods.Remake
                    : u.Id.StartsWith("brood.", StringComparison.Ordinal)
                    ? FamilyMods.Brood : FamilyMods.Brothel;
                Check.Eq(expectMod, u.TargetMod, "① sub-mod hook unit target: " + u.Id);
            }
            else if (u.TargetMod == FamilyMods.Iph)
            {
                // R10:main.maiden_attrs_iph——首个 TargetMod = IPH 的 MainHook 单元
                // (属性五键 postfix 共存补丁;版本绑定 IPH:在场匹配 Own、漂移
                // VersionBlocked、缺席分区不 Define 开关恒 false = Absent 零 hook)
                Check.Eq("main.maiden_attrs_iph", u.Id,
                    "① iph-bound mainhook unit id: " + u.Id);
            }
            else
            {
                Check.Eq("lewd_meme", u.TargetMod, "① family unit targets lewd_meme: " + u.Id);
            }
        }
        Check.Eq(112, HookCatalog.Units.Count, "catalog unit count = 112(0.7.1: +brood.tag_picker)");
        Check.Eq(53, mainHook, "MainHook units = 53(T9: 三新 prefix/postfix 单元)");
        Check.Eq(4, dataWrite, "DataWrite units = 4(T9: pools.maiden 删除——1.9.2 已删 EXP_NEED 字段)");
        Check.Eq(36, writeThrough, "WriteThrough units = 36(ST4: remake 7-3+4=8 + brood 12 + iph 16)");
        Check.Eq(19, subModHook, "SubModHook units = 19 (brothel 9 + tearl 3 + remake 5[ST5: +疲劳两单元] + brood 2[0.7.1: +tag_picker 无补丁宿主单元])");
        Check.Eq(2, gameUnits, "game-only units = 2");

        // —— ②:面板值键双射(unit 消费 ∪ DataOnlyKeys 显式排除)——
        var dataOnly = new HashSet<string>(HookCatalog.DataOnlyKeys);
        int valueEntries = 0;
        foreach (var e in PanelKeys.Entries)
        {
            if (PanelKeys.IsSwitchKey(e.Key)) continue;
            valueEntries++;
            Check.Eq(true, covered.Contains(e.Key) || dataOnly.Contains(e.Key),
                "② panel value key accounted: " + e.Key);
        }
        Check.Eq(93, valueEntries, "② panel value entries = 93(T9: -exp_per_level +公式三键 +interact_threshold +initial_virgin)");
        Check.Eq(89, PanelKeys.Entries.Count - 93, "② panel switch entries = 89(T9 后;值 93 + 开关 89 = 182)");
        foreach (var k in HookCatalog.DataOnlyKeys)
        {
            Check.Eq(true, !covered.Contains(k), "② DataOnlyKey not consumed by units: " + k);
            Check.Eq(true, IsPanelValueKey(k), "② DataOnlyKey is a panel value key: " + k);
        }

        // —— 0.7.0 T8:virgin_chance 全链废弃(1.9.2 CreateMaidenOfFaction 无条件
        //     Enable VIRGIN/ANAL_VIRGIN 替代掷骰段;后续 bool 键 maiden.initial_
        //     virgin 由 Task 9 另建,本任务只删)——注册表/面板/目录三面零残留 ——
        Check.Eq(false, cfg.ContainsKey("maiden.virgin_chance"), "T8 virgin_chance 键不再注册");
        bool panelHasVirgin = false;
        foreach (var e in PanelKeys.Entries)
            if (e.Key == "maiden.virgin_chance" || e.Key == "maiden.virgin_chance.enabled") panelHasVirgin = true;
        Check.Eq(false, panelHasVirgin, "T8 virgin_chance 面板零条目");
        bool unitHasVirgin = false;
        foreach (var u in HookCatalog.Units)
            if (Array.IndexOf(u.Keys, "maiden.virgin_chance") >= 0) unitHasVirgin = true;
        Check.Eq(false, unitHasVirgin, "T8 virgin_chance 无单元消费");

        // —— 0.7.0 T9:exp_per_level 全链废弃(1.9.2 已删 EXP_NEED 静态数组,
        //     经验公式化进 ExpNeedForLevel 三键接管)——注册表/面板/目录三面
        //     零残留 ——
        Check.Eq(false, cfg.ContainsKey("progress.exp_per_level"), "T9 exp_per_level 键不再注册");
        bool panelHasExp = false;
        foreach (var e in PanelKeys.Entries)
            if (e.Key == "progress.exp_per_level" || e.Key == "progress.exp_per_level.enabled") panelHasExp = true;
        Check.Eq(false, panelHasExp, "T9 exp_per_level 面板零条目");
        bool unitHasExp = false;
        foreach (var u in HookCatalog.Units)
            if (Array.IndexOf(u.Keys, "progress.exp_per_level") >= 0) unitHasExp = true;
        Check.Eq(false, unitHasExp, "T9 exp_per_level 无单元消费");

        // —— ST 1.6.1 Task 4:八键全链废弃(撤销写透 3 = remake 1.6.1 删
        //     product_chance/ovulation_guaranteed/virgin_avoidance;差分 5 =
        //     SemenProduction 类删除[semen_base/semen_train_factor]与 AddAlarm*
        //     三方法消失[alarm_*_per_night]——两单元注销的键面收缩)——注册表/
        //     镜像面板/目录三面零残留(T8 virgin_chance/T9 exp_per_level 同例;
        //     写透旧 json 残留由键存在性守卫无害化) ——
        string[] retiredKeys =
        {
            "remake.product_chance", "remake.ovulation_guaranteed", "remake.virgin_avoidance",
            "remake.semen_base", "remake.semen_train_factor",
            "remake.alarm_oral_per_night", "remake.alarm_sex_per_night", "remake.alarm_anal_per_night",
        };
        foreach (string rk in retiredKeys)
        {
            Check.Eq(false, cfg.ContainsKey(rk), "ST4 退役键不再注册: " + rk);
            Check.Eq(false, cfg.ContainsKey(PanelKeys.ItemSwitchKey(rk)), "ST4 退役键开关不再注册: " + rk);
            bool inMirror = false;
            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
                if (e.OurKey == rk) inMirror = true;
            Check.Eq(false, inMirror, "ST4 退役键镜像面板零条目: " + rk);
            bool inUnit = false;
            foreach (HookUnitDef u in HookCatalog.Units)
                if (Array.IndexOf(u.Keys, rk) >= 0) inUnit = true;
            Check.Eq(false, inUnit, "ST4 退役键无单元消费: " + rk);
        }
        Check.Eq(false, ids.Contains("remake.semen_formula"), "ST4 单元注销: remake.semen_formula 目录零行");
        Check.Eq(false, ids.Contains("remake.alarm_activity"), "ST4 单元注销: remake.alarm_activity 目录零行");
        // semen_chance_cap 归属迁移:R7 时代跨族双消费(remake.semen_formula 等价),
        // ST4 单元注销后回归 main.nightly_transpile 唯一消费(IL_02c8 注入点在
        // TranspilePlan 该 methodId 表内,ExpectTranspileKeys 同源互验)
        int capConsumers = 0; string capUnit = null;
        foreach (HookUnitDef u in HookCatalog.Units)
            if (Array.IndexOf(u.Keys, CfgKeys.SemenChanceCap) >= 0) { capConsumers++; capUnit = u.Id; }
        Check.Eq(1, capConsumers, "ST4 semen_chance_cap 唯一消费单元");
        Check.Eq("main.nightly_transpile", capUnit, "ST4 semen_chance_cap 归属 main.nightly_transpile(回归主注入)");

        // —— ③:每个子项开关键反查其原键的归属 unit(T4 放宽:DataOnly 键的
        //     开关拨动无单元可对账 = no-op 无害——min 系 5 键 IL 折叠,面板行
        //     保留但值不被消费)——
        foreach (var k in PanelKeys.NumericKeysWithSwitch)
        {
            Check.Eq(true, covered.Contains(k) || dataOnly.Contains(k),
                "③ switch owner unit exists: " + PanelKeys.ItemSwitchKey(k));
        }
        Check.Eq(89, PanelKeys.NumericKeysWithSwitch.Count, "③ 89 keys carry item switches(T9: -exp_per_level +公式三键 +interact_threshold;initial_virgin 为 Toggle 自开关无子项)");

        // —— brief 范例行精度锁定(0.7.0 T4 拆分:数值面 14 + 乳汁面 2;
        //     min 系 5 键与 daughter_value 不在——前者 DataOnly[IL 折叠],后者归
        //     main.produce_daughter[Task 8 迁移])——
        ExpectKeys("main.nightly_transpile", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.SexMinBase, CfgKeys.SexMaxBase, CfgKeys.SexPerLevelMax,
            CfgKeys.OralMaxBase, CfgKeys.OralPerLevelMax,
            CfgKeys.AnalMaxBase, CfgKeys.AnalPerLevelMax,
            CfgKeys.OvulateChance, CfgKeys.ConceivePerSex, CfgKeys.GestationDays,
            CfgKeys.LactationCap, CfgKeys.SemenChanceBase, CfgKeys.SemenChancePerTrain,
            CfgKeys.SemenChanceCap);
        ExpectKeys("main.nightly_milk_value", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.MilkValueBase, CfgKeys.MilkValuePerLv);
        // T4 同源对齐:两单元 Keys 恰 = TranspilePlan 表内同名 methodId 的注入点
        // 键集(单元拆分 ↔ 注入点表两事实源互验;Deferred 条目不在此两 methodId 内)。
        ExpectTranspileKeys("main.nightly_transpile",
            CfgKeys.SexMinBase, CfgKeys.SexMaxBase, CfgKeys.SexPerLevelMax,
            CfgKeys.OralMaxBase, CfgKeys.OralPerLevelMax,
            CfgKeys.AnalMaxBase, CfgKeys.AnalPerLevelMax,
            CfgKeys.OvulateChance, CfgKeys.ConceivePerSex, CfgKeys.GestationDays,
            CfgKeys.LactationCap, CfgKeys.SemenChanceBase, CfgKeys.SemenChancePerTrain,
            CfgKeys.SemenChanceCap);
        ExpectTranspileKeys("main.nightly_milk_value",
            CfgKeys.MilkValueBase, CfgKeys.MilkValuePerLv);
        ExpectKeys("main.level_cap", CarrierKind.MainHook, "lewd_meme", CfgKeys.LevelCap);
        // 0.7.0 T5:TryCorrupt 复刻体退役 → transpiler 单元(id 与 TranspilePlan
        // 表内 methodId 同名,corrupt_cost 六点注入:门槛 1 + 扣减 5)。Keys 收缩
        // 到 corrupt_cost 单键——whitelist/blacklist 不随迁:T2 C 节裁决为行为型
        // (IsCorruptible 消费链的 Contains 逻辑,无数值 ldc),载体仍是
        // main.is_corruptible 复刻体(其 Keys 原本就含这两键,零改动);
        // max/daily/semen 三键同因(复刻体内联消费随复刻体退役,原方法体内的
        // SetCharge/RefreshTooltip 调用由各自单元接管)
        ExpectKeys("main.corrupt_cost", CarrierKind.MainHook, "lewd_meme", CfgKeys.CorruptCost);
        ExpectTranspileKeys("main.corrupt_cost", CfgKeys.CorruptCost);
        // M2(Fix Round 1):max_charge 已 4+ 单元消费(get/set/refresh/
        // daily/on_semen_used——键级聚合任一 Own 即生效),get/set 侧此前
        // 无范例行锁定,存量缺口顺手补
        ExpectKeys("main.get_charge", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.CorruptMaxCharge);
        ExpectKeys("main.set_charge", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.CorruptMaxCharge);
        // 0.7.0 T6:OnSemenUsed 复刻体退役 → transpiler 单元(id 保持
        // main.on_semen_used,与 TranspilePlan 表内 methodId 同名,Carrier.Run
        // 的 route 过滤据此对齐;1.9.2 双参签名重解析,usedSemen 按瓶去重
        // 语义归原方法体零复刻)。Keys 3 → 2:daily 移出(复刻体内联的
        // RefreshTooltipBody 文案消费随复刻体退役,原方法体的 RefreshTooltip
        // 调用由 main.refresh_tooltip 单元接管);max/semen 两键 ≡ 注入点集
        // (semen_recharge 1 点 + max_charge 2 点——max>100 时只注 semen 会在
        // 100 处提前拒收,T2 顾虑①裁决 = 随单元一并注入)
        ExpectKeys("main.on_semen_used", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.CorruptMaxCharge, CfgKeys.CorruptSemen);
        ExpectTranspileKeys("main.on_semen_used",
            CfgKeys.CorruptMaxCharge, CfgKeys.CorruptSemen);
        ExpectKeys("main.is_corruptible", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.CorruptWhitelist, CfgKeys.CorruptBlacklist);
        // 0.7.0 Task 7:Create 系复刻体退役。alarm_nightly_once 改 transpiler
        // (AlarmClock.NightlyTickOnce 三 AddX 第二参),Keys 4 → 3——semen_output
        // 移出(1.9.2 产出无条件每苗床一瓶,无数值 ldc,T2 F 节放弃键 →
        // DataOnlyKeys,面板行保留);三键 ≡ 注入点集(main.alarm_nightly_once)
        ExpectKeys("main.alarm_nightly_once", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.AlarmOralPerNight, CfgKeys.AlarmSexPerNight, CfgKeys.AlarmAnalPerNight);
        ExpectTranspileKeys("main.alarm_nightly_once",
            CfgKeys.AlarmOralPerNight, CfgKeys.AlarmSexPerNight, CfgKeys.AlarmAnalPerNight);
        // T7:main.create_tentacle_alarm = 价格 postfix(CreateTentacleAlarm,
        // ApplyCsv 尾调在后 → postfix 恒胜)+ BuildWindow transpiler 双接管;
        // brood/output 两布局键(不上面板)留在单元内搭 tentacle_price 开关的车
        ExpectKeys("main.create_tentacle_alarm", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.AlarmTentaclePrice, CfgKeys.AlarmBroodSize, CfgKeys.AlarmOutputSize);
        ExpectTranspileKeys("main.create_tentacle_alarm",
            CfgKeys.AlarmBroodSize, CfgKeys.AlarmOutputSize);
        ExpectKeys("main.create_alarm", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.AlarmPrice);                                   // 价格 postfix(纯 Create* 尾改价)
        ExpectKeys("main.alarm_try_sell_npc", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.AlarmNpcSellChance);                           // transpiler(TrySellToNpc 掷骰阈值)
        ExpectTranspileKeys("main.alarm_try_sell_npc", CfgKeys.AlarmNpcSellChance);
        // T7:trap 系同款——create_manhole/create_trap 价格 postfix(后者另挂
        // BuildWindow transpiler 的 brood/output 两键);nightly_once 两键
        // (tactic 整序列替换 + semen_chance)= 注入点集;sell 两键独立 transpiler
        ExpectKeys("main.create_manhole", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.TrapPrice);
        ExpectKeys("main.create_trap", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.TrapTrapPrice, CfgKeys.TrapBroodSize, CfgKeys.TrapOutputSize);
        ExpectTranspileKeys("main.create_trap",
            CfgKeys.TrapBroodSize, CfgKeys.TrapOutputSize);
        ExpectKeys("main.trap_nightly_once", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.TrapSemenChance, CfgKeys.TrapTacticChances);
        ExpectTranspileKeys("main.trap_nightly_once",
            CfgKeys.TrapSemenChance, CfgKeys.TrapTacticChances);
        ExpectKeys("main.trap_sell_scav", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.TrapNpcSellChanceScav);
        ExpectTranspileKeys("main.trap_sell_scav", CfgKeys.TrapNpcSellChanceScav);
        ExpectKeys("main.trap_sell_thief", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.TrapNpcSellChanceThief);
        ExpectTranspileKeys("main.trap_sell_thief", CfgKeys.TrapNpcSellChanceThief);
        // R7 拆分 + R9 二次拆分 + 0.7.0 T9 收缩:pools.maiden 删除(1.9.2 已删
        // EXP_NEED 字段);JOBS 独立恒 Own(IPH 零触碰职业抽取);fbb 三池让位
        // IPH(Router R4)
        ExpectKeys("pools.maiden_jobs", CarrierKind.DataWrite, "lewd_meme",
            CfgKeys.Jobs);
        ExpectKeys("pools.maiden_fbb", CarrierKind.DataWrite, "lewd_meme",
            CfgKeys.Factions, CfgKeys.Bodies, CfgKeys.Breasts);
        // 0.7.0 T9:A 组真新增三单元键集逐字
        ExpectKeys("main.interact_threshold", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.InteractThreshold);
        ExpectKeys("main.exp_formula", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.ExpBase, CfgKeys.ExpDivisor, CfgKeys.ExpStep);
        ExpectKeys("main.initial_virgin", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.InitialVirgin);
        // T9:add_body_exp 复刻体退役 → 改参 prefix(仅 ExpMultiplier 单键:
        // 公式归 main.exp_formula,升级循环归原方法体,level_cap 归读 clamp 单元)
        ExpectKeys("main.add_body_exp", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.ExpMultiplier);
        // R7 等价恢复:拾荒概率 getter 接管(与 on_loot_rolled 双消费 scav_maiden_base)
        ExpectKeys("main.loot_maiden_chance", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.ScavMaidenBase);
        // R10:IPH 属性五键单元(MainHook 但 TargetMod = IPH,键集逐字)
        ExpectKeys("main.maiden_attrs_iph", CarrierKind.MainHook, FamilyMods.Iph,
            CfgKeys.IphAttrHighChance, CfgKeys.IphAttrMidChance,
            CfgKeys.IphAttrHighRange, CfgKeys.IphAttrMidRange, CfgKeys.IphAttrLowRange);

        // 0.7.0 T8:create_maiden 拆段。数值段 attr 五键 → main.maiden_roll、
        // faction_base → main.maiden_faction_base(两 transpiler 单元与
        // TranspilePlan E 节 methodId 同名,Keys ≡ 注入点键集);两池段复刻体
        // Keys 8 → 1(仅 portrait_size——数值读全走 transpiler getter 与主 mod
        // RollAttr/FactionBase/AttrValue 委托,复刻体不再自掷);virgin_chance
        // 键废弃(1.9.2 无条件处女对齐在池段复刻体内完成)
        ExpectKeys("main.create_maiden", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.PortraitSize);
        ExpectKeys("main.create_maiden_of_faction", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.PortraitSize);
        ExpectKeys("main.maiden_roll", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.AttrHighChance, CfgKeys.AttrMidChance,
            CfgKeys.AttrHighRange, CfgKeys.AttrMidRange, CfgKeys.AttrLowRange);
        ExpectTranspileKeys("main.maiden_roll",
            CfgKeys.AttrHighChance, CfgKeys.AttrMidChance,
            CfgKeys.AttrHighRange, CfgKeys.AttrMidRange, CfgKeys.AttrLowRange);
        ExpectKeys("main.maiden_faction_base", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.FactionBase);
        ExpectTranspileKeys("main.maiden_faction_base", CfgKeys.FactionBase);
        // T8:produce_daughter 复刻体退役 → transpiler(methodId 保持
        // main.produce_daughter_value,与单元 id 不同名——printer 先例;remake
        // 让位随单元[R1]保持;1.9.2 行为[MAIDEN_DAUGHTER tag/姓氏继承/900]
        // 在原方法体照跑 = 零复刻自动在场)
        ExpectTranspileKeys("main.produce_daughter_value", CfgKeys.DaughterValue);

        // 0.7.0 Task 7:layout 系复刻体退役。main.build_window 全四键
        // (corrupt/tool cols ldc + brood/output ldloc 特形)≡ 注入点集;
        // main.create 缩到 tabs_per_level 单键(Keys 5 → 1——布局四键归
        // build_window 单元;原版 Create 调原版 BuildWindow 过 Harmony 链,
        // 布局注入对 Create 路径照常生效)。两单元键全为不上面板的布局键
        // → 开关恒 false 恒不装(SwitchDispatch 四条分派,0.6.0 语义不变)
        ExpectKeys("main.build_window", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.BroodSize, CfgKeys.OutputSize, CfgKeys.CorruptCols, CfgKeys.ToolCols);
        ExpectTranspileKeys("main.build_window",
            CfgKeys.BroodSize, CfgKeys.OutputSize, CfgKeys.CorruptCols, CfgKeys.ToolCols);
        ExpectKeys("main.create", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.TabsPerLevel);
        ExpectTranspileKeys("main.create", CfgKeys.TabsPerLevel);

        // —— ⑤':M5a/M5b SubModHook 单元键集逐字(brothel 9 + tearl 3 全量)+
        //     CfgKeys 双射 ——
        ExpectKeys("brothel.budget", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelBudgetBonus);
        ExpectKeys("brothel.debt", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelDebtTags, CfgKeys.BrothelDebtChance);
        ExpectKeys("brothel.guests", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelGuestsBase, CfgKeys.BrothelGuestsPerUpgrade, CfgKeys.BrothelGuestsMax);
        ExpectKeys("brothel.traits", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelTraitsBase, CfgKeys.BrothelTraitsPerUpgrade);
        ExpectKeys("brothel.fee", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelFeeDefault, CfgKeys.BrothelFeeMax);
        ExpectKeys("brothel.mastery", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelMasteryThreshold);
        ExpectKeys("brothel.schedule", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelVisitMinDays, CfgKeys.BrothelVisitMaxDays);
        ExpectKeys("brothel.prices", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelPriceMaidDress, CfgKeys.BrothelPriceCatEars,
            CfgKeys.BrothelPriceCatTailPlug, CfgKeys.BrothelPriceLaceDress,
            CfgKeys.BrothelPriceBunnyEars, CfgKeys.BrothelPriceLaceGarter);
        ExpectKeys("brothel.stock", CarrierKind.SubModHook, FamilyMods.Brothel,
            CfgKeys.BrothelStockFirst, CfgKeys.BrothelStockMin, CfgKeys.BrothelStockMax);
        ExpectKeys("tearl.spawn", CarrierKind.SubModHook, FamilyMods.Tearl,
            CfgKeys.TearlSpawnChance);
        ExpectKeys("tearl.budget", CarrierKind.SubModHook, FamilyMods.Tearl,
            CfgKeys.TearlBudgetBase, CfgKeys.TearlBudgetRandom);
        ExpectKeys("tearl.trade", CarrierKind.SubModHook, FamilyMods.Tearl,
            CfgKeys.TearlSellDiscount, CfgKeys.TearlBuyDiscount, CfgKeys.TearlWaterPremium);
        // Task 14b:printer 单元键集逐字(五常量 → 五键,共用一单元;0.7.0 T7
        // 形态改七接管点:价格两键 postfix + grid/recipe_cost/base_progress
        // 三 transpiler + RecipeTip/RefreshDescription 文案两 prefix 保留)
        ExpectKeys("main.printer", CarrierKind.MainHook, "lewd_meme",
            CfgKeys.PrinterPrice, CfgKeys.PrinterNeedlePrice, CfgKeys.PrinterGrid,
            CfgKeys.PrinterRecipeCost, CfgKeys.PrinterBaseProgress);
        ExpectTranspileKeys("main.printer_grids", CfgKeys.PrinterGrid);
        ExpectTranspileKeys("main.printer_nightly_transpile", CfgKeys.PrinterRecipeCost);
        ExpectTranspileKeys("main.printer_progress", CfgKeys.PrinterBaseProgress);
        // 0.6.0 R4:remake 差分单元键集逐字(乳汁差分 7 + 夜间产出 9[概率6+单价3]
        // + 精液公式 2 + 闹钟计数 3)
        ExpectKeys("remake.milk_items", CarrierKind.SubModHook, FamilyMods.Remake,
            CfgKeys.RemakeMilkPriceRich, CfgKeys.RemakeMilkPriceWondrous, CfgKeys.RemakeMilkPriceDiluted,
            CfgKeys.RemakeMilkFeedDiluted, CfgKeys.RemakeMilkFeedWondrous, CfgKeys.RemakeMilkFeedRich,
            CfgKeys.RemakeMilkDrinkCredit);
        ExpectKeys("remake.milk_production", CarrierKind.SubModHook, FamilyMods.Remake,
            CfgKeys.RemakeMilkChanceRichBase, CfgKeys.RemakeMilkChanceRichPerLact,
            CfgKeys.RemakeMilkChanceWondrousBase, CfgKeys.RemakeMilkChanceWondrousPerLact,
            CfgKeys.RemakeMilkChanceDilutedBase, CfgKeys.RemakeMilkChanceDilutedPerLact,
            CfgKeys.RemakeMilkPriceRich, CfgKeys.RemakeMilkPriceWondrous, CfgKeys.RemakeMilkPriceDiluted);
        // ST4:remake.semen_formula / remake.alarm_activity 两单元注销——键集
        // 断言随目录行删除(semen 2 键全链废弃,cap 归属迁移见上方归属断言)
        // R7 等价恢复:remake ConfigureOvulationShot postfix(主 mod 侧 price_ovulation_shot 让位)
        ExpectKeys("remake.ovulation_price", CarrierKind.SubModHook, FamilyMods.Remake,
            CfgKeys.RemakeOvulationPrice);
        // ST 1.6.1 Task 5:疲劳核心两单元键集逐字(ShadowTrade 1.6.1
        // MaidenFatigue.cs——value_mul 四档 :210-233 / nightly_gain :265 /
        // exhaust_recover :28 const 内联,消费点 RecoverExhausted :124)
        ExpectKeys("remake.fatigue_value_mul", CarrierKind.SubModHook, FamilyMods.Remake,
            CfgKeys.RemakeFatigueValueMulRested, CfgKeys.RemakeFatigueValueMulTired,
            CfgKeys.RemakeFatigueValueMulWeary, CfgKeys.RemakeFatigueValueMulExhausted);
        ExpectKeys("remake.fatigue_nightly_recover", CarrierKind.SubModHook, FamilyMods.Remake,
            CfgKeys.RemakeFatigueNightlyGain, CfgKeys.RemakeFatigueExhaustRecover);
        // R12 等价恢复:Brood WeakenBasePerfume postfix(主 mod 侧 semen_effects 让位后的
        // 信息素两键出口; semen_rep_mult 维持让位)
        ExpectKeys("brood.perfume_mod", CarrierKind.SubModHook, FamilyMods.Brood,
            CfgKeys.BroodSemenPriceMod, CfgKeys.BroodSemenBonusMod);
        // 0.7.1 拾取器:无 Harmony patch 的宿主生命周期单元(Kind = SubModHook 的
        // 理由见类头注释)——Keys 两键:enabled 自开关(Toggle 经 SwitchDispatch
        // 表驱动 bool 自身,不派生 .enabled)+ key(KeyCode 参数,不驱动装配)
        ExpectKeys("brood.tag_picker", CarrierKind.SubModHook, FamilyMods.Brood,
            CfgKeys.BroodTagPickerEnabled, CfgKeys.BroodTagPickerKey);
        // 双射:CfgKeys 的 Brothel*/Tearl*/Remake*/Brood* 常量全集恰被 18 个 SubModHook
        // 单元键集覆盖(每常量至少出现一次——漏接 = 键无单元;重复出现仅限
        // 白名单——乳汁单价三键在 milk_items[目录工厂 :486]与 milk_production
        // [夜间覆写 :1224]双单元共享,两处字面量同值 150/80/50,LevelCap 双
        // 单元先例),且键集按前缀分流恰为 22 + 6 + 20 + 2。ST4 后无跨族键
        // (R7 的 nightly.semen_chance_cap 跨族等价消费随 remake.semen_formula
        // 注销终结,该键回归 main.nightly_transpile[MainHook]唯一消费)。
        // ST5 新增六键各归一单元(fatigue_value_mul 四档 / fatigue_nightly_
        // recover 两键),无共享。
        var sharedSubModKeys = new HashSet<string>(StringComparer.Ordinal)
        {
            CfgKeys.RemakeMilkPriceRich, CfgKeys.RemakeMilkPriceWondrous, CfgKeys.RemakeMilkPriceDiluted,
        };
        var subHookKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var u in HookCatalog.Units)
            if (u.Kind == CarrierKind.SubModHook)
                foreach (string k in u.Keys)
                {
                    if (!subHookKeys.Add(k))
                        Check.Eq(true, sharedSubModKeys.Contains(k),
                            "⑤' sub-mod key shared only in whitelist: " + k);
                }
        foreach (System.Reflection.FieldInfo f in typeof(CfgKeys).GetFields())
        {
            if (!f.Name.StartsWith("Brothel", StringComparison.Ordinal)
                && !f.Name.StartsWith("Tearl", StringComparison.Ordinal)
                && !f.Name.StartsWith("Remake", StringComparison.Ordinal)) continue;
            string key = (string)f.GetValue(null);
            Check.Eq(true, subHookKeys.Contains(key), "⑤' CfgKeys 家族常量有消费单元: " + key);
        }
        // R12 双射补充:brood.* 键按**键名前缀**匹配(字段名前缀 Brood* 会误伤
        // layout.brood_size 的 BroodSize——它是主面板布局键,非家族接管键)
        foreach (System.Reflection.FieldInfo f in typeof(CfgKeys).GetFields())
        {
            string key = (string)f.GetValue(null);
            if (!key.StartsWith("brood.", StringComparison.Ordinal)) continue;
            Check.Eq(true, subHookKeys.Contains(key), "⑤' CfgKeys 的 brood.* 键有 SubModHook 消费单元: " + key);
        }
        int brothelKeys = 0, tearlKeys = 0, remakeKeys = 0, broodKeys = 0, crossFamily = 0;
        foreach (string k in subHookKeys)
        {
            if (k.StartsWith("brothel.", StringComparison.Ordinal)) brothelKeys++;
            else if (k.StartsWith("tearl.", StringComparison.Ordinal)) tearlKeys++;
            else if (k.StartsWith("remake.", StringComparison.Ordinal)) remakeKeys++;
            else if (k.StartsWith("brood.", StringComparison.Ordinal)) broodKeys++;
            else crossFamily++;
        }
        Check.Eq(22, brothelKeys, "⑤' brothel.* 键 = 22 个 Brothel* 常量");
        Check.Eq(6, tearlKeys, "⑤' tearl.* 键 = 6 个 Tearl* 常量");
        Check.Eq(20, remakeKeys, "⑤' remake.* 键 = 20 个 Remake* 常量(ST4: 19-5;ST5: +疲劳 6)");
        Check.Eq(4, broodKeys, "⑤' brood.* 键 = 4 个 Brood* 常量(R12 信息素两键 + 0.7.1 拾取器两键)");
        Check.Eq(0, crossFamily, "⑤' 跨族共享键 0(ST4: semen_chance_cap 回归 MainHook 唯一消费)");
        Check.Eq(52, subHookKeys.Count, "⑤' SubModHook 键集 = 22 + 6 + 20 + 4(0.7.1: +拾取器两键)");
        // R7 双消费白名单:loot.scav_maiden_base 在 main.on_loot_rolled(复刻体读)
        // 与 main.loot_maiden_chance(getter 接管)双 MainHook 单元——键级聚合下
        // 任一 Own 即生效(remake 在场时 on_loot_rolled Yield、getter Own)。
        int scavConsumers = 0;
        foreach (var u in HookCatalog.Units)
            if (Array.IndexOf(u.Keys, CfgKeys.ScavMaidenBase) >= 0) scavConsumers++;
        Check.Eq(2, scavConsumers, "⑤' scav_maiden_base 双消费(on_loot_rolled + loot_maiden_chance,R7)");
    }

    /// <summary>注册性探针:Current 对未注册键抛 KeyNotFoundException(纯读无副作用)。</summary>
    private static bool Registered(ConfigRegistry cfg, string key)
    {
        try { cfg.Current<object>(key); return true; }
        catch (KeyNotFoundException) { return false; }
    }

    private static bool IsPanelValueKey(string key)
    {
        foreach (var e in PanelKeys.Entries)
        {
            if (PanelKeys.IsSwitchKey(e.Key)) continue;
            if (e.Key == key) return true;
        }
        return false;
    }

    /// <summary>范例行断言:存在性 + Kind + TargetMod + 键集逐项含于(计数一致)。</summary>
    private static void ExpectKeys(string id, CarrierKind kind, string targetMod, params string[] keys)
    {
        HookUnitDef u = null;
        foreach (var x in HookCatalog.Units)
            if (x.Id == id) u = x;
        Check.Eq(true, u != null, "exemplar unit exists: " + id);
        if (u == null) return;
        Check.Eq(kind, u.Kind, "exemplar kind: " + id);
        Check.Eq(targetMod, u.TargetMod, "exemplar TargetMod: " + id);
        Check.Eq(keys.Length, u.Keys.Length, "exemplar key count: " + id);
        var set = new HashSet<string>(u.Keys);
        foreach (var k in keys)
            Check.Eq(true, set.Contains(k), "exemplar key present (" + id + "): " + k);
    }

    /// <summary>T4 同源对齐:methodId 的注入点键集(TranspilePlan.Points)与期望
    /// 键集互验(计数一致 + 互含)——单元 Keys 与注入点表两独立事实源锁定,
    /// 表加键不进单元(或反之)在此显形。</summary>
    private static void ExpectTranspileKeys(string methodId, params string[] keys)
    {
        var planKeys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var e in TranspilePlan.Points(methodId)) planKeys.Add(e.Key);
        Check.Eq(keys.Length, planKeys.Count, "transpile plan key count: " + methodId);
        foreach (var k in keys)
            Check.Eq(true, planKeys.Contains(k), "transpile plan key present (" + methodId + "): " + k);
    }
}
