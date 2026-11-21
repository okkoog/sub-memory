using System;
using System.Collections.Generic;

namespace LewdMemeConfig
{
    /// <summary>
    /// 纯逻辑配置注册表：内置默认值 + 三层取值链（spec 2026-11-11 §3，
    /// 0.6.0 服务面拆除后子 mod override 层已删除）。
    /// Current&lt;T&gt;(key)：玩家面板值（仅当 ≠ 默认值 = 已干预）
    ///   → 主 mod 面板动态层（provider 每读现求值，抛异常时跳层）
    ///   → 默认值；未注册键抛 <see cref="KeyNotFoundException"/>。
    /// 玩家层经 <see cref="SetPlayerValue"/> 由面板转发层（PanelConfig）写入，
    /// 玩家值等于默认值时视为未干预（移除该层）；
    /// 主 mod 面板层经 <see cref="SetMainModPanelProvider{T}(string, Func{T})"/> 注入（面板转发）。
    /// 不依赖任何游戏类型（纯 System.Collections），可脱离游戏单测。
    /// 默认值 = 主 mod v1.7.1 反编译字面量（Task 14b 基线切换;nightly/corrupt/
    /// alarm/trap 等两版零漂移;loot.med_* = 1.6.1 字面量[恢复语义]、printer.* =
    /// 1.7.1 新增）;全默认（无玩家干预）时行为与原版逐项一致 = 验收红线。
    /// </summary>
    public sealed class ConfigRegistry
    {
        private readonly Dictionary<string, (object def, Type type)> defaults =
            new Dictionary<string, (object, Type)>();
        // 主 mod 面板动态层:key → provider;每次 Current 现求值不缓存,provider 异常时跳层退默认
        private readonly Dictionary<string, Delegate> mainModPanel =
            new Dictionary<string, Delegate>();
        // 玩家面板层:key → 值;仅存 ≠ 默认值的键(= 默认视为未干预,面板"恢复默认"即回到此状态)
        private readonly Dictionary<string, object> playerValues =
            new Dictionary<string, object>();

        /// <summary>
        /// 玩家层与主 mod 面板层变化时按 key 触发;写入即触发,
        /// 不看 Current 是否被更高层遮蔽(消费者弱契约:收到通知重读 Current)。
        /// </summary>
        public event Action<string> Changed;

        private ConfigRegistry() { }

        /// <summary>
        /// 创建内置全部默认值的注册表(值键 = 面板 93[T9:-exp_per_level +公式三键
        /// +interact_threshold +initial_virgin]+ 面板外布局尺寸 9;另:子项开关 89、
        /// 子 mod 镜像值键 91(ST5 后:remake 8+20 / brood 12+2 / brothel 22 /
        /// tearl 6 / iph 16 写透 + 5 属性 MainHook)各配子项开关,均经
        /// FamilyPanelKeys 表注册)。
        /// </summary>
        public static ConfigRegistry CreateDefault()
        {
            var r = new ConfigRegistry();
            r.RegisterNightly();
            r.RegisterLayout();
            r.RegisterCorrupt();
            r.RegisterLoot();
            r.RegisterAlarm();
            r.RegisterTrap();
            r.RegisterPrinter();
            r.RegisterClient();
            r.RegisterMaiden();
            r.RegisterSpecial();
            r.RegisterPrices();
            r.RegisterUi();
            r.RegisterSwitches();
            r.RegisterMirrorKeys();
            return r;
        }

        /// <summary>
        /// 子 mod 面板键注册(M4b R-I + M5a/M5b):FamilyPanelKeys 条目按 Kind 对应的
        /// CLR 类型注册(Choice → string、SliderInt → int、SliderFloat → float、
        /// Toggle → bool;默认值 = 对方官方默认(WriteThrough)或反编译字面量
        /// (SubModHook/brothel/tearl),单一事实源 = FamilyPanelKeys 表),开关沿用
        /// RegisterSwitches 模式(OurKey + ".enabled",默认全 false)。注册是
        /// 无条件的——在场过滤只发生在面板条件 Define(R-I);目标缺席时键无玩家层值,
        /// 开关恒读默认 false,写透/装配自然零动作(缺席 no-op 红线)。
        /// </summary>
        private void RegisterMirrorKeys()
        {
            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
            {
                if (e.Default is string s) Register(e.OurKey, s);
                else if (e.Default is int i) Register(e.OurKey, i);
                else if (e.Default is float f) Register(e.OurKey, f);
                else if (e.Default is bool b) Register(e.OurKey, b);
                else throw new System.InvalidOperationException("연동 설정 키의 기본값 자료형을 지원하지 않음: " + e.OurKey);
            }
            // 0.7.1:自开关 Toggle / EnumKeyCode 纯参数条目不派生 .enabled
            //(SelfSwitchForm 见下;派生会被 SwitchDispatch 第 1 条抢先命中恒
            // false,拾取器单元永不装)
            foreach (FamilyPanelEntry e in FamilyPanelKeys.Entries)
                if (!SelfSwitchForm(e))
                    Register(PanelKeys.ItemSwitchKey(e.OurKey), false);
        }

        /// <summary>
        /// 镜像条目是否为「不派生 .enabled 子项」形态(0.7.1 拾取器):SubModHook 的
        /// Toggle = 自开关(bool 自身即开关,经 SwitchDispatch 表驱动分流——若派生
        /// .enabled 会被其第 1 条抢先命中恒 false,单元永不装);EnumKeyCode = 纯
        /// 参数键(键位无启停语义,不驱动装配,宿主热读)。此判定是
        /// ConfigRegistry / SwitchDispatch / PanelConfig / SelfTestRunner 四处
        /// 分流的同一语义,键名不散写。
        /// </summary>
        internal static bool SelfSwitchForm(FamilyPanelEntry e)
        {
            return e.Carrier == CarrierKind.SubModHook
                && (e.Kind == PanelKind.Toggle || e.Kind == PanelKind.EnumKeyCode);
        }

        /// <summary>
        /// 开关键注册(0.5.0 开关体系,spec §4.2 修订):87 个子项开关,默认全
        /// false(装上=纯原版)。总/分区开关已按作者裁定移除(游戏面板 Apply 模型,
        /// 层级联动 UI 无法实现)。键名由 PanelKeys 命名规则生成,单一事实源。
        /// </summary>
        private void RegisterSwitches()
        {
            foreach (string key in PanelKeys.NumericKeysWithSwitch)
                Register(PanelKeys.ItemSwitchKey(key), false);
        }

        private void Register<T>(string key, T def)
        {
            defaults[key] = (def, typeof(T));
        }

        /// <summary>defaults 查询(键注册即 true,不看玩家/动态层)。SwitchDispatch
        /// 显式分派用(F1,验收 R1):区分「无子项开关的注册表键」与「未注册键」
        /// 不再依赖 KeyNotFoundException——后者回退 bool 读取对 int 布局键是
        /// InvalidCastException(0x01.log 三症状共同根因)。纯逻辑,附测试
        /// (SwitchOnContractTests ⑥)。</summary>
        public bool ContainsKey(string key)
        {
            return defaults.ContainsKey(key);
        }

        public T Current<T>(string key)
        {
            if (!defaults.TryGetValue(key, out var entry))
                throw new KeyNotFoundException("등록되지 않은 설정 키: " + key);
            // 开关检查(只压玩家层):子项开关关 → 跳过玩家层直查动态层;
            // 开关键自身豁免(防死锁:开关关着也能再打开)。
            if (!PlayerLayerAllowed(key))
                goto mainModLayer;
            if (playerValues.TryGetValue(key, out var p)) return (T)p;
            mainModLayer:
            if (mainModPanel.TryGetValue(key, out var d))
            {
                try { return (T)((Func<T>)d)(); }
                catch { /* provider 失效:跳层退默认,不向读取方传播 */ }
            }
            return (T)entry.def;
        }

        /// <summary>
        /// 玩家层直读(0.6.0 R7):玩家层(受开关压层语义)→ 默认值,**不经主 mod
        /// 面板动态层**。供 ModSettings.MaidenChance getter 接管前缀
        /// (main.loot_maiden_chance)专用——该 getter 自身就是动态层的 provider
        /// (ModEntry.TryInjectMainModPanel 注入),走 Current 全链会形成
        /// getter → Current → 动态层 → getter 的无限递归;此通道与 Current 的
        /// 差异仅在跳过动态层,开关压层(PlayerLayerAllowed)语义一致。
        /// 未注册键抛 KeyNotFoundException(与 Current 同契约)。
        /// </summary>
        internal T PlayerOrDefault<T>(string key)
        {
            if (!defaults.TryGetValue(key, out var entry))
                throw new KeyNotFoundException("등록되지 않은 설정 키: " + key);
            if (PlayerLayerAllowed(key) && playerValues.TryGetValue(key, out var p))
                return (T)p;
            return (T)entry.def;
        }

        /// <summary>
        /// 玩家层可见性(0.5.0 开关体系,spec §4.2 修订:仅子项开关):该键注册了
        /// 子项开关键且其关 → 压玩家层。开关状态直查 playerValues(不走 Current,
        /// 防递归)。开关键自身恒 true(豁免,防死锁)。
        /// </summary>
        internal bool PlayerLayerAllowed(string key)
        {
            if (PanelKeys.IsSwitchKey(key)) return true;
            string itemSwitch = PanelKeys.ItemSwitchKey(key);
            if (defaults.ContainsKey(itemSwitch) && !SwitchOn(itemSwitch)) return false;
            return true;
        }

        private bool SwitchOn(string switchKey)
        {
            return playerValues.TryGetValue(switchKey, out var v) && v is bool b && b;
        }

        /// <summary>
        /// 主 mod 面板动态层写入（面板转发专用，Task 8 注入）：provider 每次读取现求值、
        /// 不缓存；provider 抛异常时该层视为无值（跳层退默认）；provider 传 null = 移除该键
        /// 的面板层。注入/移除与其他层写入一致，按 key 触发 Changed。
        /// </summary>
        internal void SetMainModPanelProvider<T>(string key, Func<T> provider)
        {
            ValidateWrite(key, typeof(T));
            if (provider == null) mainModPanel.Remove(key);
            else mainModPanel[key] = provider;
            Changed?.Invoke(key);
        }

        /// <summary>
        /// 玩家面板层写入（PanelConfig 转发专用）。值等于默认值 → 视为未干预（移除该层），
        /// 否则写入；写入即触发 Changed（游戏侧 OnChanged 已过滤无变化写入）。
        /// </summary>
        internal void SetPlayerValue<T>(string key, T value) where T : struct
        {
            ValidateWrite(key, typeof(T));
            if (Equals(defaults[key].def, value)) playerValues.Remove(key);
            else playerValues[key] = value;
            Changed?.Invoke(key);
        }

        /// <summary>
        /// 玩家面板层写入（string 文本框;PanelConfig 转发专用）。string 不是 struct,
        /// 与泛型重载并列;语义一致:值等于默认值 → 视为未干预（移除该层）。
        /// </summary>
        internal void SetPlayerValue(string key, string value)
        {
            ValidateWrite(key, typeof(string));
            if (Equals(defaults[key].def, value)) playerValues.Remove(key);
            else playerValues[key] = value;
            Changed?.Invoke(key);
        }

        // 写入校验：先键存在（KeyNotFoundException），再类型一致（InvalidCastException）。
        private void ValidateWrite(string key, Type t)
        {
            if (!defaults.TryGetValue(key, out var entry))
                throw new KeyNotFoundException("등록되지 않은 설정 키: " + key);
            if (t != entry.type)
                throw new InvalidCastException(
                    "설정 키 " + key + " 자료형 불일치: 등록형 " + entry.type.Name + ", 입력형 " + t.Name);
        }

        // ------------------------------------------------------------------
        // 默认值注册段（值 = 主 mod 反编译字面量,基线 v1.7.1[Task 14b];nightly
        // 数值两版零漂移，见 主mod硬编码参数清单.md A–I）
        // ------------------------------------------------------------------

        private void RegisterNightly()
        {
            // 公式:次数 = base + (等级-1) × per（清单 A NightlyTick，按 bodyLevel = L）
            Register(CfgKeys.SexMinBase, 5);            // 性交 [5+(L-1), 10+(L-1)×3]
            Register(CfgKeys.SexPerLevelMin, 1);
            Register(CfgKeys.SexMaxBase, 10);
            Register(CfgKeys.SexPerLevelMax, 3);
            Register(CfgKeys.OralMinBase, 0);           // 口交/肛交 [L-1, 2+(L-1)×2]
            Register(CfgKeys.OralPerLevelMin, 1);
            Register(CfgKeys.OralMaxBase, 2);
            Register(CfgKeys.OralPerLevelMax, 2);
            Register(CfgKeys.AnalMinBase, 0);
            Register(CfgKeys.AnalPerLevelMin, 1);
            Register(CfgKeys.AnalMaxBase, 2);
            Register(CfgKeys.AnalPerLevelMax, 2);
            Register(CfgKeys.OvulateChance, 1f / 7f);   // 进入排卵期概率 1/7 每晚
            Register(CfgKeys.ConceivePerSex, 0.01f);    // 受孕 = clamp01(次数/100)
            Register(CfgKeys.GestationDays, 30);
            Register(CfgKeys.LactationCap, 3);
            Register(CfgKeys.MilkValueBase, 100);       // 乳汁单价 100 + 等级 × 40
            Register(CfgKeys.MilkValuePerLv, 40);
            Register(CfgKeys.SemenChanceBase, 0.4f);    // min(1, 0.4 + 调教值/10 × 0.1)
            Register(CfgKeys.SemenChancePerTrain, 0.01f);
            Register(CfgKeys.SemenChanceCap, 1f);
            Register(CfgKeys.DaughterValue, 900);
            Register(CfgKeys.ExpMultiplier, 1f);
            Register(CfgKeys.TrainTameThreshold, 60);   // 清单 I:调教值阈值
        }

        private void RegisterLayout()
        {
            // 清单 A 布局:苗床/产出区 16×16;侵蚀区、触手区 3×16;储藏页签 bodyLevel × 2
            Register(CfgKeys.BroodSize, 16);
            Register(CfgKeys.OutputSize, 16);
            Register(CfgKeys.CorruptCols, 3);
            Register(CfgKeys.ToolCols, 3);
            Register(CfgKeys.TabsPerLevel, 2);
            // 0.7.0 T9:EXP_NEED 静态数组已随 1.9.2 删除,经验表公式化
            // (ExpNeedForLevel:100 + (lv-1)/3*100 整除截断)→ 公式三键接管,
            // exp_per_level CSV 键废弃
            Register(CfgKeys.ExpBase, 100);
            Register(CfgKeys.ExpDivisor, 3);
            Register(CfgKeys.ExpStep, 100);
            // 主 mod 字面量(1.6.1 起未变,1.9.2 同):GetBodyLevel clamp 1-5
            // (等级上限,更多可配置项全新键;1.9.2 升级循环无上限拦截,读 clamp 封顶)
            Register(CfgKeys.LevelCap, 5);
        }

        private void RegisterCorrupt()
        {
            // 清单 B:MAX_CHARGE/DAILY_RECHARGE/SEMEN_CHARGE/CORRUPT_COST;*_extra 为追加项,默认空
            Register(CfgKeys.CorruptMaxCharge, 100);
            Register(CfgKeys.CorruptDaily, 10);
            Register(CfgKeys.CorruptSemen, 10);
            Register(CfgKeys.CorruptCost, 100);
            // 主 mod 字面量(1.6.1 起未变,1.7.1 同):Create 内 SetCharge(100)(初始充能,更多可配置项全新键)
            Register(CfgKeys.CorruptInitialCharge, 100);
            Register(CfgKeys.CorruptWhitelist, "");
            Register(CfgKeys.CorruptBlacklist, "");
        }

        private void RegisterLoot()
        {
            // Task 14b(1.7.1 适配):医疗组排卵针分支已于 1.7.0 从原版删除——
            // med 双键重定位为「恢复 1.6.x」opt-in,默认值 = 1.6.1 字面量
            // (关键词四词 + 概率 0.10);拾荒组→少女 0.05(1.6.1/1.7.1 一致,
            // 不再随等级;scav_maiden_per_level/scav_shot_chance 已随 1.6.1 移除停注)
            Register(CfgKeys.MedKeywords, "med,drug,pharma,chem");
            Register(CfgKeys.MedShotChance, 0.1f);
            Register(CfgKeys.ScavGroups, "dumpingGroundTG,dumpingGroundFreshTG");
            Register(CfgKeys.ScavMaidenBase, 0.05f);
            // 主 mod 1.6.1/1.7.1 同字面量:ROLL_COOLDOWN 内联 0.5(掉落节流窗口,
            // 0=禁用,更多可配置项全新键)
            Register(CfgKeys.LootThrottleSeconds, 0.5f);
        }

        private void RegisterAlarm()
        {
            // 1.6.1 字面量:CreateAlarm 单价 75;CreateTentacleAlarm 单价 150、苗床/产出区 4×4;
            // NightlyTickOnce 每夜口交/性交/肛交各 +1、精液产出 1 瓶;TrySellToNpc 上架率 0.05(三条调用路径共用)
            Register(CfgKeys.AlarmPrice, 75);
            Register(CfgKeys.AlarmTentaclePrice, 150);
            Register(CfgKeys.AlarmBroodSize, 4);
            Register(CfgKeys.AlarmOutputSize, 4);
            Register(CfgKeys.AlarmOralPerNight, 1);
            Register(CfgKeys.AlarmSexPerNight, 1);
            Register(CfgKeys.AlarmAnalPerNight, 1);
            Register(CfgKeys.AlarmSemenOutput, 1);
            Register(CfgKeys.AlarmNpcSellChance, 0.05f);
        }

        private void RegisterTrap()
        {
            // 1.6.1 字面量:CreateManhole 单价 100;CreateTrap 单价 200、苗床/产出区 12×12;
            // NightlyTickOnce 三档策略捕捉率 {0.1,0.2,0.3}(CSV,Task 12 解析)、苗床产精液率 0.5;
            // 拾荒/小偷井盖上架率 0.10/0.05(主mod补丁类 Postfix 复刻,Task 12)
            Register(CfgKeys.TrapPrice, 100);
            Register(CfgKeys.TrapTrapPrice, 200);
            Register(CfgKeys.TrapBroodSize, 12);
            Register(CfgKeys.TrapOutputSize, 12);
            Register(CfgKeys.TrapTacticChances, "0.1,0.2,0.3");
            Register(CfgKeys.TrapSemenChance, 0.5f);
            Register(CfgKeys.TrapNpcSellChanceScav, 0.1f);
            Register(CfgKeys.TrapNpcSellChanceThief, 0.05f);
        }

        private void RegisterPrinter()
        {
            // 主 mod 1.7.1 字面量(反编译 lewd_meme_171/TentaclePrinter.cs L9-17):
            // PRINTER_PRICE=500 / NEEDLE_PRICE=15 / GRID=12 / RECIPE_COST=15 /
            // BASE_PROGRESS=3f。CreateDefault 的键计数注释(91 键)随 Task 14b +5。
            Register(CfgKeys.PrinterPrice, 500);
            Register(CfgKeys.PrinterNeedlePrice, 15);
            Register(CfgKeys.PrinterGrid, 12);
            Register(CfgKeys.PrinterRecipeCost, 15);
            Register(CfgKeys.PrinterBaseProgress, 3f);
        }

        private void RegisterClient()
        {
            // 清单 F/G:改写 0.75;池上限 3;精液 -15/+15/×1.15;GuestBox 显示开;触手起源 +1 女/日、+3 拾荒
            Register(CfgKeys.FemaleRewriteChance, 0.75f);
            Register(CfgKeys.GenderScope, 0);           // spec §6:0–3,默认 0 = 原版行为
            Register(CfgKeys.ClientPoolSize, 3);
            Register(CfgKeys.SemenPriceMod, -15f);
            Register(CfgKeys.SemenBonusMod, 15f);
            Register(CfgKeys.SemenRepMult, 1.15f);
            Register(CfgKeys.GuestDisplayEnabled, true);
            Register(CfgKeys.PerkDailyFemales, 1);
            Register(CfgKeys.PerkScavBonus, 3);
        }

        private void RegisterMaiden()
        {
            // 清单 C:属性 <1% → [70,80]、<6% → [60,70]、否则 [10,60];阵营基价;立绘 64;四池。
            // 0.7.0 T8:virgin_chance 键废弃(1.9.2 无条件处女 Enable,原 0.6 掷骰段删除)
            // → 0.7.0 T9:bool 键 maiden.initial_virgin 落地(默认 true = 原版行为)
            Register(CfgKeys.AttrHighChance, 0.01f);
            Register(CfgKeys.AttrMidChance, 0.06f);
            Register(CfgKeys.AttrHighRange, "70,80");
            Register(CfgKeys.AttrMidRange, "60,70");
            Register(CfgKeys.AttrLowRange, "10,60");
            Register(CfgKeys.FactionBase, "上层区:100,治安部:50,黑市:50,革命军:50,其他:10");
            // 0.7.0 T9 新增:互动阈值(ComputeInteractMax 四阈值 ≥60)与初始处女
            // bool(1.9.2 无条件 Enable 双 tag;false = postfix Disable)
            Register(CfgKeys.InteractThreshold, 60);
            Register(CfgKeys.InitialVirgin, true);
            Register(CfgKeys.PortraitSize, 64);
            Register(CfgKeys.Factions, "下层区,上层区,治安部,黑市,革命军");
            Register(CfgKeys.Jobs, "拾荒者,矿工,雇佣兵,工程师,护士,医生,厨师,农场主,小偷,商人,清洁员,技师,文员,走私者,货运员,酒保,维修工,研究员,学徒,艺人");
            Register(CfgKeys.Bodies, "萝莉,少女,御姐,少妇,熟女");
            Register(CfgKeys.Breasts, "贫乳,普乳,巨乳");
        }

        private void RegisterSpecial()
        {
            // 清单 H:追加项默认空（原版白名单 22 id / 31 立绘 / 关键词在 patch 侧作 csvBase 常量）
            Register(CfgKeys.SpecialExtraIds, "");
            Register(CfgKeys.SpecialExtraSprites, "");
            Register(CfgKeys.SpecialExtraKeywords, "");
            Register(CfgKeys.NamesUseCustomFile, false);
        }

        private void RegisterPrices()
        {
            // 清单 D
            Register(CfgKeys.PriceMilk, 180);
            Register(CfgKeys.PriceMilkCredit, 30);
            Register(CfgKeys.PriceSemen, 90);
            Register(CfgKeys.PriceTentacleBox, 200);
            Register(CfgKeys.PriceOvulationShot, 120);
        }

        private void RegisterUi()
        {
            // 清单 A UI:夜报窗口 480×240、延迟 1.2s;读档修复延迟 1.5s
            Register(CfgKeys.UiReportWidth, 480);
            Register(CfgKeys.UiReportHeight, 240);
            Register(CfgKeys.UiReportDelay, 1.2f);
            Register(CfgKeys.UiReloadFixDelay, 1.5f);
        }
    }
}
