using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 暗流交易联动扩展·差分数值接管(0.6.0 R4,目录单元 remake.*;ST 1.6.1
    /// Task 5 后 5 单元 = milk_items/milk_production/ovulation_price[R7 等价
    /// 恢复]/fatigue_value_mul/fatigue_nightly_recover,Kind = SubModHook,
    /// TargetMod = lewd_meme_remake,绑定版本 1.6.1
    /// [ShadowTrade 适配 Task 2:mod 更名 ShadowTrade/暗流交易,基线 = 其源码;
    /// 接管点存续三方法 = CreateMilkVariant/TryProduceMilk/ConfigureOvulationShot
    /// 数值面零漂移;semen_formula[SemenProduction 类整体删除]与
    /// alarm_activity[AddAlarm* 三方法随疲劳门禁重写消失]两单元已于 Task 4
    /// 注销——目录行/键面/installs/复刻体全清,历史实现见 git 历史,见
    /// FamilyMods.BoundVersions 注释])。remake 的差分实现(乳汁三档)全部
    /// const 内联于方法体,改字段无效,故逐方法 prefix:方法体与 remake 基线
    /// 逐字对齐(1.6.1 起基线 = ShadowTrade 源码,1.5.x 历史零漂移复核见
    /// FamilyMods.BoundVersions 注释),仅把常量换成
    /// <c>PatchCtx.Config.Current&lt;int/float&gt;</c> 读数(默认值 = 原版字面量)。
    /// 单元 → 方法(ShadowTrade 1.6.4 源码行号):
    ///  <list type="bullet">
    ///   <item>remake.milk_items — ItemFactory.cs:438 CreateMilkVariant(私有静态,
    ///     三个目录工厂 CreateMilk/CreateRichMilk/CreateDilutedMilk 的共用底座):
    ///     单价 150/80/50(:444 三元)+ 喂奶缓解 20/30/25(:462 三元)+
    ///     饮用 +30 信用(:456 onActivateSlotItemFunc 闭包内联)——1.6.1 数值
    ///     面零漂移(复刻体 switch 形 = 1.5.x 反编译形态,与 1.6.1 源码三元
    ///     语义等价)</item>
    ///   <item>remake.milk_production — TentacleBody.cs:627 TryProduceMilk
    ///     (私有静态):三档概率 0.3/0.5/0.7(:636-651 档位映射)+ 每级泌乳
    ///     系数 0.3(:658,原版三档共用一个常量,面板拆三键默认均 0.3;1.6.1
    ///     起概率不再整体乘 ProductMultiplier——该配置已删)+
    ///     产出单价覆写 50×档位倍率(:667,恰等于 milk_items 的 150/80/50——
    ///     单价三键由两单元共享,任一单元在装即接管其路径)+ 逐瓶记账
    ///     RecordNightProduct(:675,1.6.1 起单参 (string),旧签名
    ///     (GameGridInventory,string) 断链)</item>
    ///   <item>remake.fatigue_value_mul — MaidenFatigue.cs:210 ValueMul
    ///     (public static 纯函数,消费点 ItemFactory.UpdateMaidenValue :769):
    ///     售价四档 1.1/0.94/0.88/0.8,正常档(20..59)恒 1f 无键、档位边界
    ///     const 90/80/60/20 保持原版;求值内核 Logic/FatigueRules
    ///     [FatigueRulesTests 单测],!Enabled → 1f 分支保持原序</item>
    ///   <item>remake.fatigue_nightly_recover — MaidenFatigue.cs:254
    ///     NightlyGain(:265 基数 0.5f 单键;:260 口交加权 0.5f 为另一常量
    ///     不接管)+ :119 RecoverExhausted(internal,const ExhaustRecover
    ///     :28 内联消费点 :124;双消费方 TentacleBody :601 /
    ///     OriginalDeviceActivityPatch :95 调同一方法,patch 一次全接管;
    ///     同日重入 tag 私有 const 以字面量逐字复刻,方法体无 try-catch
    ///     保持)——一单元双 prefix</item>
    ///   <item>remake.semen_formula — 已随 1.6.1 SemenProduction 类整体删除
    ///     注销(Task 2 删 installs/复刻体,Task 4 删目录行与 2 键面板条目;
    ///     精液公式回归主 mod 自家实现,面板出口 = 主面板
    ///     nightly.semen_chance_* 三键,cap 键唯一消费 = main.nightly_transpile)</item>
    ///   <item>remake.alarm_activity — 已随 1.6.1 疲劳门禁重写注销
    ///     (OriginalDeviceActivityPatch 重写为 Begin/Complete,AddAlarmOral/
    ///     Sex/Anal 三方法不复存在;Task 4 删 installs/复刻体/目录行与 3 键
    ///     面板条目;主 mod 侧闹钟计数出口 = main.alarm_nightly_once
    ///     [transpiler,1.6.1 撤让恢复 Own])</item>
    ///  </list>
    /// 零漂移红线:各 prefix 先比较「全部消费键 == 原版字面量」,相等即放行
    /// 原方法(装上且默认值 = 原版行为);默认全关 = 单元不装(穿透原则)。
    /// 取值兜底:PatchCtx 未注入 / mod 停用时 ?? 原版常量(等价放行)。
    /// ⚠ 同名类型防串(remake 与主 mod 的 MidnightPawn(.Game) 命名空间全同,
    /// ItemFactory/TentacleBody/ModCtx/ModSettings 同名同签名):一律经
    /// <c>ModReflect.Types(FamilyMods.Remake)</c> 的类型集按 Name 解析——该
    /// 枚举按加载器登记的 mod→程序集映射返回,天然只含 remake 程序集;
    /// <see cref="FindRemakeType"/> 另锚定「全部解析类型同属一个程序集」
    /// (ModSettings 等同名类型主 mod 在场,解析成功即证未串族,锚定程序集
    /// 必为 remake),加载器行为漂移时装配期即炸(记
    /// Dead),绝不装出错族方法。游戏类型(GameItem/GameGridInventory/
    /// ItemDirectory/ModHelper/GridShape/SlotMarker)直接编译引用。
    /// 解析只发生在装配委托内(两阶段提交:EnsureResolved 任一成员失败 →
    /// 原样上抛不提交,3 个条目一致不装);安装失败(版本漂移致签名变)由
    /// HookInstaller 逐单元隔离记 Dead。主 mod 侧对应单元(main.alarm_* /
    /// main.trap_* / main.nightly_transpile / main.nightly_milk_value /
    /// main.price_milk)的 remake 让位不变
    /// ——那是「对主 mod 的 hook 让位」,本组接管 remake 自家实现,两者并存。
    /// R6-F1 修正:ModCtx.Log/LogWarning 委托签名 Action&lt;object&gt;(remake
    /// 1521 原文 ModCtx.cs:14/26 参数是 object(与 1519 相同,文件未变);R4 误抄主 mod patch 的
    /// Action&lt;string&gt; → StaticMethod 签名精确匹配失败 → 4 单元全部 Dead、
    /// 18 个差分键不生效)。其余委托成员(RectShape/LogShape/ConsumeOne/GetStr/
    /// Feed/TryProduceMilkItem/RecordNightProduct)已逐个对 1521 签名
    /// 复核无误(R8 复核:全部漂移仅为反编译行号偏移;1.6.1 断链同步:
    /// get_ProductMultiplier 与 GetInt 委托随复刻体退役删除、
    /// RecordNightProduct 改单参 Action&lt;string&gt;)。
    /// </summary>
    public static class RemakePatches
    {
        // -- remake 类型/成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用)--
        private static bool resolved;
        private static System.Type remakeItemFactory;     // MidnightPawn.Game.ItemFactory(remake 程序集)
        private static System.Type remakeTentacleBody;    // MidnightPawn.Game.TentacleBody(remake 程序集)
        private static System.Type remakeModCtx;          // MidnightPawn.ModCtx(remake 程序集)
        private static System.Type remakeMaidenFatigue;   // MaidenFatigue(remake 程序集)
        private static System.Type remakeModSettings;     // MidnightPawn.Game.ModSettings(remake 程序集)
        private static Assembly remakeAssembly;           // 防串锚(全部解析类型必须同属此程序集)

        // -- 委托缓存(静态方法,含私有/internal——ModReflect.StaticMethod 合法)--
        private static Func<int, int, GridShape> ifRectShape;                 // ItemFactory.RectShape(:44 public)
        private static Action<GameItem, string> ifLogShape;                   // ItemFactory.LogShape(:58 public)
        private static Action<GameItem> ifConsumeOne;                         // ItemFactory.ConsumeOne(:900 public)
        private static Func<GameItem, string, string> ifGetStr;               // ItemFactory.GetStr(:740;1.6.1 GetInt 委托随 semen_formula 复刻体退役删除)
        // ⚠ 委托参数是 object 非 string:remake ModCtx.Log/LogWarning 签名为
        // (object msg)(remake_1521 ModCtx.cs:14/26,主 mod 同名类才是 (string)),
        // StaticMethod 按委托参数精确匹配,Action<string> 解析必炸(R6-F1:4 单元
        // Dead 的根因)。调用点传 string 隐式装箱,与原调用形态逐字兼容。
        private static Action<object> ctxLog;                                 // ModCtx.Log
        private static Action<object> ctxLogWarning;                          // ModCtx.LogWarning
        private static Func<GameItem, int, string, bool> fatigueFeed;         // MaidenFatigue.Feed(:355)
        private static Func<GameGridInventory, string, long, bool> tbTryProduceMilkItem; // TentacleBody.TryProduceMilkItem(1.6.4 :687 私有,签名未变)
        // 1.6.1 断链同步:RecordNightProduct 签名 (GameGridInventory,string) →
        // (string)(TentacleBody.cs:273,调用点 :675 只传 itemId,库存参数已删)。
        private static Action<string> tbRecordNightProduct;                   // TentacleBody.RecordNightProduct(:273 私有)
        // -- ST 1.6.1 Task 5(疲劳核心):MaidenFatigue 运行时成员 + remake
        //    ItemFactory 的 tag/描述面(属性 getter 经 get_* 方法名解析,
        //    ModReflect.MethodsNamed 枚举含之;Get/Set/LevelMul/Today 为 public
        //    static,RecoverExhausted 的 tag 面走 ItemFactory 委托——ExhaustedAt
        //    等档位 const 保持原版字面量,不入委托)--
        private static Func<bool> fatigueEnabled;            // MaidenFatigue.get_Enabled(:36 FatigueEnabled 兜底 true)
        private static Func<float> fatigueScale;             // MaidenFatigue.get_Scale(:51 FatigueScale 兜底 1f)
        private static Func<float> fatigueLevelMul;          // MaidenFatigue.LevelMul(:236 本体等级系数,11 级起 0)
        private static Func<int> fatigueToday;               // MaidenFatigue.Today(:134 StoreStation.dayCounter,未就绪 -1)
        private static Func<GameItem, int> fatigueGet;       // MaidenFatigue.Get(:66 clamp 0..100)
        private static Action<GameItem, int> fatigueSet;     // MaidenFatigue.Set(:82 写前 clamp 0..100)
        private static Func<GameItem, string, int> ifGetInt;          // ItemFactory.GetInt(:715)
        private static Action<GameItem, string, int> ifSetInt;        // ItemFactory.SetInt(:706)
        private static Action<GameItem> ifUpdateMaidenValue;          // ItemFactory.UpdateMaidenValue(:752)
        private static Action<GameItem> ifUpdateMaidenDescription;    // ItemFactory.UpdateMaidenDescription(:805)

        /// <summary>把 5 个条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账;remake.semen_formula
        /// 已随 1.6.1 SemenProduction 类删除退役、remake.alarm_activity 已随
        /// AddAlarm* 三方法消失注销[Task 4],均不在此登记;ST Task 5 新增疲劳
        /// 核心两单元——fatigue_nightly_recover 一单元双 prefix,两 original
        /// 供方法级引用计数记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("remake.milk_items", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(remakeItemFactory, "CreateMilkVariant",
                    new[] { typeof(string), typeof(string), typeof(string), typeof(string) });
                h.Patch(m, prefix: new HarmonyMethod(CreateMilkVariantPrefix));
                return new[] { m };
            });
            installs.Add("remake.milk_production", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(remakeTentacleBody, "TryProduceMilk",
                    new[] { typeof(GameGridInventory), typeof(GameItem), typeof(int) });
                h.Patch(m, prefix: new HarmonyMethod(TryProduceMilkPrefix));
                return new[] { m };
            });
            installs.Add("remake.ovulation_price", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(remakeItemFactory, "ConfigureOvulationShot",
                    new[] { typeof(GameItem) });
                h.Patch(m, postfix: new HarmonyMethod(ConfigureOvulationShotPostfix));
                return new[] { m };
            });
            // ST 1.6.1 Task 5:疲劳核心两单元(ValueMul public static 纯函数 +
            // NightlyGain public static / RecoverExhausted internal static,
            // 签名对源码逐字符;ModReflect.Target 合法覆盖 private/internal)
            installs.Add("remake.fatigue_value_mul", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(remakeMaidenFatigue, "ValueMul",
                    new[] { typeof(int) });
                h.Patch(m, prefix: new HarmonyMethod(ValueMulPrefix));
                return new[] { m };
            });
            installs.Add("remake.fatigue_nightly_recover", h =>
            {
                EnsureResolved();
                MethodBase gain = ModReflect.Target(remakeMaidenFatigue, "NightlyGain",
                    new[] { typeof(int), typeof(int), typeof(int), typeof(bool) });
                MethodBase recover = ModReflect.Target(remakeMaidenFatigue, "RecoverExhausted",
                    new[] { typeof(GameItem) });
                h.Patch(gain, prefix: new HarmonyMethod(NightlyGainPrefix));
                h.Patch(recover, prefix: new HarmonyMethod(RecoverExhaustedPrefix));
                return new[] { gain, recover };
            });
        }

        /// <summary>
        /// 幂等解析全部 remake 成员(5 个条目共享;任一成员解析抛出 → 原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 5 个补丁一致不装,杜绝半解析
        /// 状态下装出运行时 NRE 的 prefix——两阶段提交,同 TearlPatches/BrothelPatches)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type ifactory = FindRemakeType("ItemFactory");
            System.Type tbody = FindRemakeType("TentacleBody");
            System.Type ctx = FindRemakeType("ModCtx");
            System.Type fatigue = FindRemakeType("MaidenFatigue");
            System.Type settings = FindRemakeType("ModSettings");
            // 委托(含私有/internal 成员;RectShape/LogShape/ConsumeOne/GetStr
            // 为 public static,其余私有——ModReflect.StaticMethod 均合法)。
            // 1.6.1 断链同步:SemenProduction 类已删(FindRemakeType 不再解析)、
            // OriginalDeviceActivityPatch 已重写为疲劳门禁、解析随 alarm_activity
            // 注销一并删除[Task 4]、get_ProductMultiplier/GetInt 委托随复刻体
            // 退役删除、RecordNightProduct 改单参。
            var rRect = ModReflect.StaticMethod<Func<int, int, GridShape>>(ifactory, "RectShape");
            var rLogShape = ModReflect.StaticMethod<Action<GameItem, string>>(ifactory, "LogShape");
            var rConsume = ModReflect.StaticMethod<Action<GameItem>>(ifactory, "ConsumeOne");
            var rGetStr = ModReflect.StaticMethod<Func<GameItem, string, string>>(ifactory, "GetStr");
            var rLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var rWarn = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            var rFeed = ModReflect.StaticMethod<Func<GameItem, int, string, bool>>(fatigue, "Feed");
            var rMilkItem = ModReflect.StaticMethod<Func<GameGridInventory, string, long, bool>>(tbody, "TryProduceMilkItem");
            var rRecord = ModReflect.StaticMethod<Action<string>>(tbody, "RecordNightProduct");
            // ST 1.6.1 Task 5(疲劳核心):MaidenFatigue 运行时成员(属性经
            // get_* 方法名解析)+ remake ItemFactory 的 tag/描述面;签名对
            // ShadowTrade 1.6.1 源码逐字符(见字段注释行号)
            var rFatigueEnabled = ModReflect.StaticMethod<Func<bool>>(fatigue, "get_Enabled");
            var rFatigueScale = ModReflect.StaticMethod<Func<float>>(fatigue, "get_Scale");
            var rFatigueLevelMul = ModReflect.StaticMethod<Func<float>>(fatigue, "LevelMul");
            var rFatigueToday = ModReflect.StaticMethod<Func<int>>(fatigue, "Today");
            var rFatigueGet = ModReflect.StaticMethod<Func<GameItem, int>>(fatigue, "Get");
            var rFatigueSet = ModReflect.StaticMethod<Action<GameItem, int>>(fatigue, "Set");
            var rIfGetInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(ifactory, "GetInt");
            var rIfSetInt = ModReflect.StaticMethod<Action<GameItem, string, int>>(ifactory, "SetInt");
            var rIfUpdValue = ModReflect.StaticMethod<Action<GameItem>>(ifactory, "UpdateMaidenValue");
            var rIfUpdDesc = ModReflect.StaticMethod<Action<GameItem>>(ifactory, "UpdateMaidenDescription");
            // 全部成功,一次性提交
            remakeItemFactory = ifactory; remakeTentacleBody = tbody;
            remakeModCtx = ctx; remakeMaidenFatigue = fatigue; remakeModSettings = settings;
            ifRectShape = rRect; ifLogShape = rLogShape; ifConsumeOne = rConsume;
            ifGetStr = rGetStr;
            ctxLog = rLog; ctxLogWarning = rWarn;
            fatigueFeed = rFeed;
            tbTryProduceMilkItem = rMilkItem; tbRecordNightProduct = rRecord;
            fatigueEnabled = rFatigueEnabled; fatigueScale = rFatigueScale;
            fatigueLevelMul = rFatigueLevelMul; fatigueToday = rFatigueToday;
            fatigueGet = rFatigueGet; fatigueSet = rFatigueSet;
            ifGetInt = rIfGetInt; ifSetInt = rIfSetInt;
            ifUpdateMaidenValue = rIfUpdValue; ifUpdateMaidenDescription = rIfUpdDesc;
            resolved = true;
        }

        /// <summary>在前置 lewd_meme_remake 暴露的类型里按名查找。防串断言:全部
        /// 解析类型必须同属一个程序集(锚定首个命中;ItemFactory/TentacleBody/
        /// ModCtx/ModSettings 在主 mod 有全同命名空间同名副本,ModSettings 等
        /// 同名类型解析成功即证未串族,锚定程序集必为 remake 程序集)——串入时
        /// 立即抛,装配期记 Dead,绝不装错族方法。</summary>
        private static System.Type FindRemakeType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Remake))
                if (t.Name == name)
                {
                    if (remakeAssembly == null) remakeAssembly = t.Assembly;
                    else if (remakeAssembly != t.Assembly)
                        throw new MissingMemberException(
                            "remake 类型程序集不一致(疑似串到主 mod 同名类型): " + name);
                    return t;
                }
            throw new MissingMemberException("前置 lewd_meme_remake 未暴露类型 " + name);
        }

        /// <summary>配置读取兜底:PatchCtx 未注入(mod 停用后)→ 原版常量(等价放行)。</summary>
        private static int Cfg(string key, int originalDefault)
        {
            return PatchCtx.Config?.Current<int>(key) ?? originalDefault;
        }

        /// <summary>float 版取值兜底(概率/系数键)。</summary>
        private static float CfgF(string key, float originalDefault)
        {
            return PatchCtx.Config?.Current<float>(key) ?? originalDefault;
        }

        // ------------------------------------------------------------------
        // remake.milk_items — ItemFactory.CreateMilkVariant
        // ------------------------------------------------------------------

        /// <summary>ItemFactory.cs:438-474(私有静态,三个乳汁工厂的共用底座;
        /// 1.6.1 数值面零漂移,仅源码行号偏移):构造空物品 → 差分单价三元
        /// (:444,rich 150 / wondrous 80 / 其余 50)→ 四类型标签 →
        /// 图标档映射(low/high/medium)→ 2×3 形状(经 RectShape
        /// 委托)→ 激活委托(饮用 +30 信用 AddCredit 内联 :456,闭包经 ctxLog/
        /// ModHelper 直调)→ 喂奶缓解三元(:462,default 20 / rich 25 /
        /// wondrous 30,relief 进委托闭包;复刻体 switch 形 = 1.5.x 反编译
        /// 形态,与 1.6.1 源码三元语义等价)→ 目标委托(MaidenFatigue.Feed 经
        /// 委托 + ConsumeOne 经委托)→ LogShape。三组常量 →
        /// remake.milk_price_*/milk_feed_*/milk_drink_credit(默认 150/80/50、
        /// 20/30/25、30)。七键全默认 → 放行原方法(零漂移)。委托字段经
        /// GameItem 的 GameItemFunc 继承面直接赋值(游戏类型,编译引用)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool CreateMilkVariantPrefix(string identifier, string displayName,
            string description, string flavor, ref GameItem __result)
        {
            int priceRich = Cfg(CfgKeys.RemakeMilkPriceRich, 150);
            int priceWondrous = Cfg(CfgKeys.RemakeMilkPriceWondrous, 80);
            int priceDiluted = Cfg(CfgKeys.RemakeMilkPriceDiluted, 50);
            int feedDiluted = Cfg(CfgKeys.RemakeMilkFeedDiluted, 20);
            int feedWondrous = Cfg(CfgKeys.RemakeMilkFeedWondrous, 30);
            int feedRich = Cfg(CfgKeys.RemakeMilkFeedRich, 25);
            int drinkCredit = Cfg(CfgKeys.RemakeMilkDrinkCredit, 30);
            if (priceRich == 150 && priceWondrous == 80 && priceDiluted == 50
                && feedDiluted == 20 && feedWondrous == 30 && feedRich == 25
                && drinkCredit == 30)
            {
                return true; // 默认零漂移:原方法原样运行
            }

            GameItem obj = ItemDirectory.CreateEmptyItem(identifier);
            obj.name = displayName;
            obj.shortDescription = description;
            obj.flavorText = flavor;
            obj.unitValue = ((identifier == "milk_rich") ? priceRich
                : ((identifier == "milk_wondrous") ? priceWondrous : priceDiluted));
            obj.SetGameItemType("LUXURY_ITEM");
            obj.SetGameItemType("BREVAGE");
            obj.SetGameItemType("TREAT");
            obj.SetGameItemType("FOOD");
            string text;
            switch (identifier)
            {
                default:
                    text = "milk_low";
                    break;
                case "milk_rich":
                    text = "milk_high";
                    break;
                case "milk_wondrous":
                case "wondrous_milk":
                    text = "milk_medium";
                    break;
            }
            obj.SetSpriteAndShapeFromMod("lewd_meme_remake", text);
            obj.SetShape(ifRectShape(2, 3));
            obj.EnableTag("MILK_REMAKE_OWN_SPRITE", false);
            ctxLog("乳汁图标: " + text + "(2×3)");
            obj.canActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
            obj.mayActivateSlotItemFunc = (GameItem it, GameInventory inv, SlotMarker slot) => true;
            obj.onActivateSlotItemFunc = delegate (GameItem it, GameInventory inv, SlotMarker slot)
            {
                ModHelper.AddCredit(drinkCredit, true);
                ctxLog("饮下一瓶" + displayName);
                it.Destroy();
            };
            int relief;
            switch (identifier)
            {
                default:
                    relief = feedDiluted; // default 档 = 清甜(milk_diluted 及未知)
                    break;
                case "milk_rich":
                    relief = feedRich;
                    break;
                case "milk_wondrous":
                case "wondrous_milk":
                    relief = feedWondrous;
                    break;
            }
            obj.mayThisTargetItemFunc = (GameItem self, GameItem target) => target != null && target.IsTag("MAIDEN_FEMALE");
            obj.canThisTargetItemFunc = (GameItem self, GameItem target) => target != null && target.IsTag("MAIDEN_FEMALE");
            obj.onThisTargetItemFunc = delegate (GameItem self, GameItem target)
            {
                if (target != null && target.IsTag("MAIDEN_FEMALE")
                    && fatigueFeed(target, relief, displayName))
                {
                    ifConsumeOne(self);
                }
            };
            ifLogShape(obj, displayName);
            __result = obj;
            return false;
        }

        // ------------------------------------------------------------------
        // remake.milk_production — TentacleBody.TryProduceMilk
        // ------------------------------------------------------------------

        /// <summary>TentacleBody.cs:627-685(私有静态):乳房档位映射(:636-651
        /// 贫乳→milk_rich 倍率 3 概率 0.3 / 巨乳→milk_diluted 倍率 1 概率 0.7 /
        /// 其余→milk_wondrous 倍率 1.6 概率 0.5)→ 概率公式(:658
        /// 基础 + max(0, 泌乳等级-1) × 0.3,1.6.1 起不再整体乘 ProductMultiplier
        /// ——该配置已删,乘子行随断链同步删除)→ 期望取整掷骰 →
        /// 产出单价(:667 RoundToInt(50f × 倍率)
        /// = 150/80/50,恰等于 milk_items 的单价三键 → 共享键,接管后直接写面板
        /// 单价)→ 逐瓶 TryProduceMilkItem(:687,签名未变)+
        /// RecordNightProduct(:675,1.6.1 起单参 (string),委托直调)。
        /// 概率 6 键 + 单价 3 键全默认 → 放行原方法(零漂移)。异常路径(:683
        /// LogWarning + return 0)逐字保留。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool TryProduceMilkPrefix(GameGridInventory output, GameItem maiden,
            int lactationLevel, ref int __result)
        {
            float richBase = CfgF(CfgKeys.RemakeMilkChanceRichBase, 0.3f);
            float richPer = CfgF(CfgKeys.RemakeMilkChanceRichPerLact, 0.3f);
            float wondrousBase = CfgF(CfgKeys.RemakeMilkChanceWondrousBase, 0.5f);
            float wondrousPer = CfgF(CfgKeys.RemakeMilkChanceWondrousPerLact, 0.3f);
            float dilutedBase = CfgF(CfgKeys.RemakeMilkChanceDilutedBase, 0.7f);
            float dilutedPer = CfgF(CfgKeys.RemakeMilkChanceDilutedPerLact, 0.3f);
            int priceRich = Cfg(CfgKeys.RemakeMilkPriceRich, 150);
            int priceWondrous = Cfg(CfgKeys.RemakeMilkPriceWondrous, 80);
            int priceDiluted = Cfg(CfgKeys.RemakeMilkPriceDiluted, 50);
            if (richBase == 0.3f && richPer == 0.3f
                && wondrousBase == 0.5f && wondrousPer == 0.3f
                && dilutedBase == 0.7f && dilutedPer == 0.3f
                && priceRich == 150 && priceWondrous == 80 && priceDiluted == 50)
            {
                return true; // 默认零漂移:原方法原样运行
            }

            if (output == null || maiden == null)
            {
                __result = 0;
                return false;
            }
            try
            {
                string str = ifGetStr(maiden, "MAIDEN_BREAST");
                string text;
                long value;
                float baseChance;
                float perLactation;
                if (!(str == "贫乳"))
                {
                    if (str == "巨乳")
                    {
                        text = "milk_diluted";
                        value = priceDiluted;
                        baseChance = dilutedBase;
                        perLactation = dilutedPer;
                    }
                    else
                    {
                        text = "milk_wondrous";
                        value = priceWondrous;
                        baseChance = wondrousBase;
                        perLactation = wondrousPer;
                    }
                }
                else
                {
                    text = "milk_rich";
                    value = priceRich;
                    baseChance = richBase;
                    perLactation = richPer;
                }
                // :658 概率公式(1.6.1 断链同步:ProductMultiplier 配置已删,
                // 乘子行随之删除;产物概率档位仍由面板概率 6 键治理)
                float num3 = baseChance + (float)UnityEngine.Mathf.Max(0, lactationLevel - 1) * perLactation;
                int num4 = UnityEngine.Mathf.FloorToInt(num3);
                float num5 = num3 - (float)num4;
                int num6 = num4 + ((UnityEngine.Random.value < num5) ? 1 : 0);
                if (num6 <= 0)
                {
                    __result = 0;
                    return false;
                }
                int num7 = 0;
                for (int i = 0; i < num6; i++)
                {
                    if (!tbTryProduceMilkItem(output, text, value))
                    {
                        break;
                    }
                    tbRecordNightProduct(text);
                    num7++;
                }
                __result = num7;
                return false;
            }
            catch (Exception ex)
            {
                ctxLogWarning("产出乳汁失败: " + ex.Message);
                __result = 0;
                return false;
            }
        }

        // ------------------------------------------------------------------
        // remake.semen_formula — 已随 1.6.1 SemenProduction 类删除退役
        // (installs/复刻体删除于 ShadowTrade 适配 Task 2;目录注销与键面
        //  收缩[semen_base/semen_train_factor 及 semen_chance_cap 键归属
        //  迁移]归后续任务,历史实现见 git 历史)
        // ------------------------------------------------------------------

        // ------------------------------------------------------------------
        // remake.ovulation_price — ItemFactory.ConfigureOvulationShot(等价恢复)
        // ------------------------------------------------------------------

        /// <summary>
        /// remake ItemFactory.ConfigureOvulationShot(ShadowTrade 1.6.1
        /// MidnightPawn.Game/ItemFactory.cs:500-505,public static void,参数名
        /// item):remake 的 PatchItemCreateContraband 对主 mod
        /// CreateOvulationShot postfix → 本方法(PatchItemCreateContraband.cs:21),
        /// :503 <c>item.unitValue = 100L</c> 强制单价——主 mod 侧
        /// main.price_ovulation_shot 让位(两 postfix 同目标执行序不定,数值
        /// 叠加竞争)后的等价接管点:postfix 追加在 remake 写 100 之后,只改
        /// unitValue(其 identifier 守卫早退[:502 非 ovulation_shot 即 return,
        /// 1.5.x 的去重字典 OvulationConfigured 已撤]与 EnsureContraband
        /// 一概不动,早退路径后 postfix 照跑,装配后所有经此方法
        /// 的物品价签一致)。默认 100 = remake 字面量,同值覆写零漂移;开关关
        /// = 单元不装 = remake 原行为。postfix 参数名按名注入须与基线一致
        /// (item)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void ConfigureOvulationShotPostfix(GameItem item)
        {
            if (item == null) return;
            try
            {
                int price = Cfg(CfgKeys.RemakeOvulationPrice, 100);
                if (price == 100) return; // 默认零漂移:remake 已写同值,不重复写
                item.unitValue = price;
            }
            catch (Exception ex)
            {
                ctxLogWarning("排卵针单价接管失败: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // remake.fatigue_value_mul — MaidenFatigue.ValueMul(ST 1.6.1 Task 5)
        // ------------------------------------------------------------------

        /// <summary>MaidenFatigue.cs:210-233(public static float,参数名 fatigue;
        /// 消费点 = ItemFactory.UpdateMaidenValue :769 售价惩罚段):!Enabled → 1f
        /// → ≥90 透支 0.8f → ≥80 疲惫 0.88f → ≥60 疲倦 0.94f → &lt;20 休息充足
        /// 1.1f → 其余正常档 1f。复刻体经 <see cref="FatigueRules.ValueMul"/>
        /// 求值(档位边界 const 90/80/60/20 保持原版);!Enabled 分支在原方法体
        /// 首行、与档位值无关,复刻保持同序。四键全默认 → 放行原方法(零漂移)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ValueMulPrefix(int fatigue, ref float __result)
        {
            float rested = CfgF(CfgKeys.RemakeFatigueValueMulRested, 1.1f);
            float tired = CfgF(CfgKeys.RemakeFatigueValueMulTired, 0.94f);
            float weary = CfgF(CfgKeys.RemakeFatigueValueMulWeary, 0.88f);
            float exhausted = CfgF(CfgKeys.RemakeFatigueValueMulExhausted, 0.8f);
            if (rested == 1.1f && tired == 0.94f && weary == 0.88f && exhausted == 0.8f)
            {
                return true; // 默认零漂移:原方法原样运行
            }
            if (!fatigueEnabled())
            {
                __result = 1f;
                return false;
            }
            __result = FatigueRules.ValueMul(fatigue, rested, tired, weary, exhausted);
            return false;
        }

        // ------------------------------------------------------------------
        // remake.fatigue_nightly_recover — NightlyGain / RecoverExhausted
        // (ST 1.6.1 Task 5 双 prefix,两方法一体装配)
        // ------------------------------------------------------------------

        /// <summary>MaidenFatigue.cs:254-268(public static int,参数名
        /// vaginal/oral/anal/pregnant;消费点 = NightlyMaidenState.cs:21):
        /// !Enabled → 0 → 加权 max(0,vaginal)+max(0,anal)+RoundToInt(max(0,oral)
        /// ×0.5f)(口交加权 0.5f 为独立常量,不接管)→ weighted ≤ 0 早退 →
        /// gain = weighted × <b>基数</b> × LevelMul × (孕 1.5) × Scale →
        /// gain ≤ 0 早退 → Max(1,RoundToInt)。求值内核 =
        /// <see cref="FatigueRules.NightlyGain"/>(LevelMul/Scale 经委托现取,
        /// 复刻体与原方法同序)。基数默认 0.5 → 放行原方法(零漂移)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool NightlyGainPrefix(int vaginal, int oral, int anal, bool pregnant,
            ref int __result)
        {
            float gainPerWeight = CfgF(CfgKeys.RemakeFatigueNightlyGain, 0.5f);
            if (gainPerWeight == 0.5f)
            {
                return true; // 默认零漂移:原方法原样运行
            }
            if (!fatigueEnabled())
            {
                __result = 0;
                return false;
            }
            __result = FatigueRules.NightlyGain(vaginal, oral, anal, pregnant,
                gainPerWeight, fatigueLevelMul(), fatigueScale());
            return false;
        }

        /// <summary>MaidenFatigue.cs:119-127(internal static void,参数名 item;
        /// 双消费方 TentacleBody.cs:601 / OriginalDeviceActivityPatch.cs:95 调同一
        /// 方法,patch 一次全接管):同日重入早退(ExhaustedRestDayTag 私有
        /// const,复刻用字面量 "SHADOWTRADE_EXHAUSTED_REST_DAY" = 源码 :112
        /// 逐字)→ 写休息日 tag → Set(Max(0, Get − ExhaustRecover))(减法内核 =
        /// <see cref="FatigueRules.Recovered"/>,0 下限;上限 100 由原版 Set 内
        /// clamp 兜底,经委托直调)→ UpdateMaidenValue/UpdateMaidenDescription。
        /// 方法体无 try-catch,复刻保持。恢复量默认 5 → 放行原方法(零漂移)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool RecoverExhaustedPrefix(GameItem item)
        {
            const string RestDayTag = "SHADOWTRADE_EXHAUSTED_REST_DAY";
            int recover = Cfg(CfgKeys.RemakeFatigueExhaustRecover, 5);
            if (recover == 5)
            {
                return true; // 默认零漂移:原方法原样运行
            }
            int day = fatigueToday();
            if (day >= 0 && item.IsTag(RestDayTag) && ifGetInt(item, RestDayTag) == day)
            {
                return false; // 原方法首两行的同日重入早退(等价 return)
            }
            if (day >= 0) ifSetInt(item, RestDayTag, day);
            fatigueSet(item, FatigueRules.Recovered(fatigueGet(item), recover));
            ifUpdateMaidenValue(item);
            ifUpdateMaidenDescription(item);
            return false;
        }

        // ------------------------------------------------------------------
        // remake.alarm_activity — 已随 1.6.1 疲劳门禁重写退役
        // (1.6.1 OriginalDeviceActivityPatch 整体重写为 Begin/Complete 门禁,
        //  AddAlarmOral/Sex/Anal 三方法不复存在;installs 与参数重写复刻体
        //  删除于 ShadowTrade 适配 Task 4,目录行与 3 键面板条目同批注销,
        //  历史实现见 git 历史;主 mod 侧闹钟计数出口 = main.alarm_nightly_once
        //  [transpiler,1.6.1 撤让恢复 Own])
        // ------------------------------------------------------------------
    }
}
