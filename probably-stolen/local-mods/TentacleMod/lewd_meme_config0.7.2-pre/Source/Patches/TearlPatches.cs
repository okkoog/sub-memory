using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// tearlaments 交易接管(M5b,目录单元 tearl.* ×3,Kind = SubModHook,TargetMod
    /// = dui_mian_lan_qiu_ji_tearl,绑定版本 7;开关开且版本命中时装配,装配路径
    /// 与 MainHook 相同——HookRuntime R-N/R-R)。tearl 与 brothel 同样无任何配置
    /// 系统,交易数值全部在 TearlamentsConfig.Customer 声明为 const——编译期已在
    /// 消费点内联(改字段无效),故逐方法 prefix 复刻:方法体与反编译 tearl_7
    /// 逐字对齐(各复刻体注释标 file:line),仅把常量换成
    /// <c>PatchCtx.Config.Current&lt;int/float&gt;</c> 读数(默认值 = 原版字面量)。
    /// 单元 → 方法(decomp 相对路径行号):
    ///  <list type="bullet">
    ///   <item>tearl.spawn — TearlamentsMod.cs:59 HandleCustomerSpawn(私有实例,
    ///     ModHook.OnGenerateCustomerNormal 处理器;掷骰 0.3 内联于 :61)</item>
    ///   <item>tearl.budget — CustomCustomer.cs:18 Create(预算 150000 + 随机
    ///     0..50000 内联于 :53)</item>
    ///   <item>tearl.trade — CustomCustomer.cs:18 Create(:54 buyPriceModifier=-10)
    ///     + :103 ApplyBuyPriceFeature(:107 收购折扣 -25 / :112 水溢价 +20)——
    ///     两个方法一个单元(refcount 跨方法);幂等 RemoveItemFeatureByID 先行
    ///     逻辑在 ApplyClientPriceFeature(:130-150),该方法不 patch、复刻体经
    ///     委托直调原实现,幂等语义原样保留</item>
    ///  </list>
    /// 零漂移红线:各 prefix 先比较「全部消费键 == 原版字面量」,相等即放行原方法
    /// (装上且默认值 = 原版行为);默认全关 = 单元不装(穿透原则)。取值兜底:
    /// PatchCtx 未注入 / mod 停用时 ?? 原版常量(等价放行)。
    /// 形态取舍(报告记载):HandleCustomerSpawn 是具名私有实例方法(事件 += 方法组
    /// 订阅,非匿名 lambda),可直接 Target——prefix 复刻整段判定与去重扫描;
    /// spawn 概率掷骰 Random.value 的短路求值(manager == null 时不掷骰)逐字保留。
    /// Create 为 tearl.budget 与 tearl.trade 两单元共享方法:两单元共用同一
    /// <see cref="CreatePrefix"/>(读全三键,任一非默认即接管、全默认放行),
    /// 挂载经 <see cref="PatchCreateOnce"/> 幂等化(同一 original 不重复挂自己的
    /// prefix;末位引用计数 Unpatch 后可重新挂载),两单元的 install 委托都把
    /// Create 记入记账清单——方法级引用计数裁决物理卸载(Task 4 机制)。
    /// 类型分工(AlarmPatch/BrothelPatches 先例):tearl 类型一律运行时经
    /// <c>ModReflect.Types(FamilyMods.Tearl)</c> 按 Name 解析(manifest 已声明
    /// 前置,缺席仅 warning——此时装配失败记 Dead,不装),不编译引用其 DLL;
    /// 游戏类型(StoreClient/StoreClientManager/GameItem/RNG/ItemFeature/
    /// ClientIntent/ModLog/StoreClientFactionSetup 等)直接编译引用。解析只发生
    /// 在装配委托内(两阶段提交:EnsureResolved 任一成员失败 → 原样上抛不提交,
    /// 3 个条目一致不装);安装失败(版本漂移致签名变)由 HookInstaller 逐单元
    /// 隔离记 Dead。
    /// </summary>
    public static class TearlPatches
    {
        // -- tearl 类型/成员的运行时缓存(install 期经 ModReflect 填充,prefix 只做直接调用)--
        private static bool resolved;
        private static System.Type tearlMod;          // TearlamentsMod(HandleCustomerSpawn 宿主)
        private static System.Type customCustomer;    // Tearlaments.CustomCustomer
        private static StoreClient.ClientIntent customerIntent; // TearlamentsConfig.Customer.Intent(readonly 静态,装配期读)
        private static string[] customerAcceptedTags; // TearlamentsConfig.Customer.AcceptedTags(同上)

        // -- 委托缓存(静态方法,含私有/internal——ModReflect.StaticMethod 合法)--
        private static Func<StoreClient> ccCreate;                       // CustomCustomer.Create
        private static Action<StoreClient> ccConfigureDialogue;          // ConfigureDialogue(私有)
        private static Action<StoreClient> ccConfigureItemAcceptance;    // ConfigureItemAcceptance(私有,内联 willAccept/onItemAdded 委托)
        private static Func<GameItem, bool> ccIsAcceptedWater;           // IsAcceptedWater(私有)
        private static Action<GameItem, string, int> ccApplyClientPriceFeature; // ApplyClientPriceFeature(私有,幂等)
        private static Func<GameItem, string, bool> ctHas;               // CustomTag.Has
        private static Action<StoreClient, string[]> ctConfigureBuyingTags; // CustomTag.ConfigureCustomerBuyingTags
        private static Func<string, string, string> locText;             // Localized.Text(internal)

        /// <summary>把 3 个条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            installs.Add("tearl.spawn", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(tearlMod, "HandleCustomerSpawn",
                    new[] { typeof(StoreClientManager) });
                h.Patch(m, prefix: new HarmonyMethod(HandleCustomerSpawnPrefix));
                return new[] { m };
            });
            installs.Add("tearl.budget", h =>
            {
                EnsureResolved();
                MethodBase m = ModReflect.Target(customCustomer, "Create", Type.EmptyTypes);
                PatchCreateOnce(h, m);
                return new[] { m };
            });
            installs.Add("tearl.trade", h =>
            {
                EnsureResolved();
                MethodBase create = ModReflect.Target(customCustomer, "Create", Type.EmptyTypes);
                MethodBase buyPrice = ModReflect.Target(customCustomer, "ApplyBuyPriceFeature",
                    new[] { typeof(GameItem) });
                PatchCreateOnce(h, create);
                h.Patch(buyPrice, prefix: new HarmonyMethod(ApplyBuyPriceFeaturePrefix));
                return new[] { create, buyPrice };
            });
        }

        /// <summary>
        /// 幂等解析全部 tearl 成员(3 个条目共享;任一成员解析抛出 → 原样上抛、
        /// 不提交任何字段,后续条目重试同样失败 → 3 个补丁一致不装,杜绝半解析
        /// 状态下装出运行时 NRE 的 prefix——两阶段提交,同 AlarmPatch/BrothelPatches)。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type tm = FindTearlType("TearlamentsMod");
            System.Type cc = FindTearlType("CustomCustomer");
            System.Type ct = FindTearlType("CustomTag");
            System.Type loc = FindTearlType("Localized");
            System.Type cust = FindTearlType("Customer"); // TearlamentsConfig.Customer(嵌套,Name 即 Customer)
            // 静态数据(readonly,读一次缓存;非 const,原版同样经字段读取——身份稳定)
            var rIntent = (StoreClient.ClientIntent)ModReflect.GetStatic(cust, "Intent");
            var rTags = ModReflect.GetStatic<string[]>(cust, "AcceptedTags");
            // 委托(含私有/internal 成员)
            var rCreate = ModReflect.StaticMethod<Func<StoreClient>>(cc, "Create");
            var rDialogue = ModReflect.StaticMethod<Action<StoreClient>>(cc, "ConfigureDialogue");
            var rAcceptance = ModReflect.StaticMethod<Action<StoreClient>>(cc, "ConfigureItemAcceptance");
            var rIsWater = ModReflect.StaticMethod<Func<GameItem, bool>>(cc, "IsAcceptedWater");
            var rApplyFeature = ModReflect.StaticMethod<Action<GameItem, string, int>>(cc, "ApplyClientPriceFeature");
            var rHas = ModReflect.StaticMethod<Func<GameItem, string, bool>>(ct, "Has");
            var rBuyingTags = ModReflect.StaticMethod<Action<StoreClient, string[]>>(ct, "ConfigureCustomerBuyingTags");
            var rText = ModReflect.StaticMethod<Func<string, string, string>>(loc, "Text");
            // 全部成功,一次性提交
            tearlMod = tm; customCustomer = cc;
            customerIntent = rIntent; customerAcceptedTags = rTags;
            ccCreate = rCreate; ccConfigureDialogue = rDialogue; ccConfigureItemAcceptance = rAcceptance;
            ccIsAcceptedWater = rIsWater; ccApplyClientPriceFeature = rApplyFeature;
            ctHas = rHas; ctConfigureBuyingTags = rBuyingTags; locText = rText;
            resolved = true;
        }

        /// <summary>在前置 dui_mian_lan_qiu_ji_tearl 暴露的类型里按名查找(嵌套类型
        /// 以 Name 命中,如 TearlamentsConfig.Customer;缺席/未运行时 ModReflect.Types
        /// 抛 → install 失败记 Dead)。</summary>
        private static System.Type FindTearlType(string name)
        {
            foreach (System.Type t in ModReflect.Types(FamilyMods.Tearl))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("前置 dui_mian_lan_qiu_ji_tearl 未暴露类型 " + name);
        }

        /// <summary>配置读取兜底:PatchCtx 未注入(mod 停用后)→ 原版常量(等价放行)。</summary>
        private static int Cfg(string key, int originalDefault)
        {
            return PatchCtx.Config?.Current<int>(key) ?? originalDefault;
        }

        /// <summary>float 版取值兜底(tearl.spawn_chance)。</summary>
        private static float CfgF(string key, float originalDefault)
        {
            return PatchCtx.Config?.Current<float>(key) ?? originalDefault;
        }

        /// <summary>Create 的跨单元共享挂载:tearl.budget 与 tearl.trade 都消费
        /// Create,共用同一 CreatePrefix(读全三键)。Harmony 对「同一 original
        /// 重复挂同一 prefix」行为无契约保证,故探测自身 id 已在场则跳过;两单元
        /// 记账清单都含 Create——末位引用计数归零 Unpatch 后,再次 install 时此
        /// 处探测不再命中、照常重挂。</summary>
        private static void PatchCreateOnce(Harmony h, MethodBase m)
        {
            // 本 Harmony 版 GetPatchInfo 返回 Patches(Prefixes 只读集合,
            // Patch.owner 为只读字段;同补丁重复挂载无契约保证,故显式幂等)
            Patches info = Harmony.GetPatchInfo(m);
            if (info != null)
            {
                foreach (Patch p in info.Prefixes)
                {
                    if (p.owner == ModEntry.Id) return; // 已由本 mod 挂载(共享前缀),不重复挂
                }
            }
            h.Patch(m, prefix: new HarmonyMethod(CreatePrefix));
        }

        // ------------------------------------------------------------------
        // tearl.spawn — TearlamentsMod.HandleCustomerSpawn
        // ------------------------------------------------------------------

        /// <summary>TearlamentsMod.cs:59-78(私有实例,OnGenerateCustomerNormal
        /// 处理器):manager 空或 Random.value &gt; 0.3 → 直接返回(短路:manager
        /// 为空时不掷骰,RNG 流与原版一致);clientStack 去重扫描(identifier
        /// "tearlaments_kitkallos" 已在队 → 返回);Create → AddClient → 日志。
        /// 0.3(SpawnChance const 内联 :61)→ tearl.spawn_chance(默认 0.3)。
        /// log 为实例字段,经 ModReflect 读出后按游戏类型 ModLog 直调。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool HandleCustomerSpawnPrefix(object __instance, StoreClientManager manager)
        {
            float chance = CfgF(CfgKeys.TearlSpawnChance, 0.3f);
            if (chance == 0.3f) return true; // 默认零漂移:原方法原样运行

            if (manager == null || UnityEngine.Random.value > chance)
            {
                return false;
            }
            foreach (StoreClient item in manager.clientStack)
            {
                if (item != null && item.identifier == "tearlaments_kitkallos")
                {
                    return false;
                }
            }
            StoreClient val = ccCreate();
            if (val != null)
            {
                manager.AddClient(val);
                ModLog log = (ModLog)ModReflect.Get(__instance, "log");
                log.Log("Queued custom customer: tearlaments_kitkallos");
            }
            return false;
        }

        // ------------------------------------------------------------------
        // tearl.budget + tearl.trade — CustomCustomer.Create(共享前缀)
        // ------------------------------------------------------------------

        /// <summary>CustomCustomer.cs:18-60(公共静态):构造 StoreClient(标识/
        /// 显示名/精灵——字符串 const 内联,保留字面量)→ InitLowerLevelCitizen →
        /// Intent/AcceptingContrabandOverride/noAcceptingContraband/
        /// acceptRetailMarkup → SetBudget(150000 + RNG.GetRandomInt(0, 50000))
        /// → buyPriceModifier = -10 → 配置购买标签 → 对话/收纳(私有方法经委托
        /// 直调原实现)→ CompleteClientCreation。150000/50000 →
        /// tearl.budget_base/budget_random(默认 150000/50000,RNG 上界含端点与
        /// 原版一致);10(MaidenSellDiscountPercent)→ tearl.sell_discount
        /// (默认 10,赋值取负还原原版 -10)。三键全默认 → 放行原方法(零漂移)。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool CreatePrefix(ref StoreClient __result)
        {
            int budgetBase = Cfg(CfgKeys.TearlBudgetBase, 150000);
            int budgetRandom = Cfg(CfgKeys.TearlBudgetRandom, 50000);
            int sellDiscount = Cfg(CfgKeys.TearlSellDiscount, 10);
            if (budgetBase == 150000 && budgetRandom == 50000 && sellDiscount == 10)
            {
                return true; // 默认零漂移:原方法原样运行
            }

            StoreClient val = new StoreClient
            {
                identifier = "tearlaments_kitkallos",
                displayName = locText("tearlaments_kitkallos_name", "Tearlaments Kitkallos"),
                spriteName = "tearlaments_kitkallos_female"
            };
            StoreClientFactionSetup.InitLowerLevelCitizen(val);
            val.clientIntent = customerIntent;
            val.AcceptingContrabandOverride = true;
            val.noAcceptingContraband = false;
            val.acceptRetailMarkup = false;
            val.SetBudget(budgetBase + RNG.GetRandomInt(0, budgetRandom));
            val.buyPriceModifier = -sellDiscount;
            ctConfigureBuyingTags(val, customerAcceptedTags);
            ccConfigureDialogue(val);
            ccConfigureItemAcceptance(val);
            val.CompleteClientCreation(false);
            __result = val;
            return false;
        }

        // ------------------------------------------------------------------
        // tearl.trade — CustomCustomer.ApplyBuyPriceFeature
        // ------------------------------------------------------------------

        /// <summary>CustomCustomer.cs:103-114(私有静态,onItemAddedToWeighted
        /// 回调内逐件触发):MAIDEN_FEMALE(CustomTag.Has 经委托)→ 少女收购折扣
        /// -25;否则 IsAcceptedWater(私有,委托直调)→ 水溢价 +20。两值经
        /// ApplyClientPriceFeature(委托直调原实现——其幂等 RemoveItemFeatureByID
        /// 先行 + 逐字段构建 ItemFeature 的逻辑 :130-150 原样运行,不接管)。
        /// 25/20(MaidenBuyDiscountPercent/WaterBuyPremiumPercent const 内联
        /// :107/:112)→ tearl.buy_discount/water_premium(默认 25/20)。
        /// percent 局部变量形态与原版逐字对齐。</summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool ApplyBuyPriceFeaturePrefix(GameItem item)
        {
            int buyDiscount = Cfg(CfgKeys.TearlBuyDiscount, 25);
            int waterPremium = Cfg(CfgKeys.TearlWaterPremium, 20);
            if (buyDiscount == 25 && waterPremium == 20) return true; // 默认零漂移

            if (ctHas(item, "MAIDEN_FEMALE"))
            {
                int percent = -buyDiscount;
                ccApplyClientPriceFeature(item, "tearlamentsMaidenBuyDiscount", percent);
            }
            else if (ccIsAcceptedWater(item))
            {
                ccApplyClientPriceFeature(item, "tearlamentsWaterPremium", waterPremium);
            }
            return false;
        }
    }
}
