using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using HarmonyLib;

namespace LewdMemeConfig
{
    /// <summary>
    /// 特殊顾客追加 / 驯服阈值 / names 自定义文件 / 说明书渲染 / 读档修复延迟批
    /// (Task 12)。条目清单(catalog-id → patch;均为 MainHook 单元,开关开且
    /// 版本受支持时装配):
    ///  <list type="bullet">
    ///   <item>main.union(prefix <c>SpecialClients.IsAny</c>)——在
    ///     原版三路白名单(id 集合/立绘/名字)之上并集 <see cref="SpecialMatcher"/>.
    ///     IsExtraSpecial(special.extra_ids 精确 / extra_sprites 子串 /
    ///     extra_keywords 名字子串);命中 → __result=true 跳原方法,未命中 →
    ///     放行原版(默认空 CSV 恒未命中 = 零漂移;主 mod 更新扩充白名单时原版
    ///     判定仍被咨询,故 Tolerant)。</item>
    ///   <item>main.tame_text <c>ItemFactory.UpdateMaidenDescription</c>
    ///     prefix + <c>CreateOvulationShot</c> postfix——调教值 60 的两处文案分支
    ///     换读 nightly.train_tame_threshold:前者默认 60 放行原版(零漂移),
    ///     ≠60 走逐行复刻体;后者字面量在 onThisTargetItemFunc 委托内(编译器
    ///     生成 lambda,无法稳定定位,同 Task 10 price.milk_credit 先例),postfix
    ///     换装等价委托(默认 60 不换装,原版委托原样保留)。</item>
    ///   <item>main.names_custom(prefix <c>NameGen.EnsureLoaded</c>)——
    ///     names.use_custom_file=true 时名字库改读
    ///     ModData/lewd_meme_config/names_custom.txt(游戏文件 API
    ///     ModHelper.ModDataFileExists/ReadModDataFileText,零能力;不落
    ///     System.IO),TSV 解析逐行复刻原版(切 \n → Trim → 跳空行 → 切 \t →
    ///     ≥3 列);写入主 mod 静态字段 regions/loaded(经 ModReflect.SetStatic,
    ///     List&lt;string[]&gt; 泛型实例跨程序集同一)。文件缺席/为空/无有效行/
    ///     读取异常 → 日志警告 + 放行原版(回退 Content/names.txt,绝不抛出)。
    ///     读档语义同原版:首个 RandomName 调用时加载一次,之后会话内缓存。</item>
    ///   <item>main.manual_render(postfix <c>ItemFactory.CreateNote</c>)——
    ///     说明书描述换 <see cref="ManualTextRenderer"/>.Render(模板 = 主 mod
    ///     v1.3.8 原文,数值位换 {cfg:key});创建点渲染,即时反映当前配置(含
    ///     子 mod override)。默认配置渲染与原文逐字一致(测试红线);配置读取
    ///     失败/注册表未注入 → 保留原描述不重写(原描述即主 mod 硬编码默认,
    ///     等价于默认渲染,零漂移)。</item>
    ///   <item>main.reload_fix(prefix 主 mod <c>ModEntry.DelayRepair</c>,
    ///     Task 7 移交)——原版:OnGameLoaded 里
    ///     <c>ModHook.Instance.StartCoroutine(DelayRepair())</c>(12/10 拒绝面)
    ///     + 协程内 WaitForSeconds(1.5f) 后跑六段修复(1.6.1 增闹钟/陷阱
    ///     登记两段)。接管:prefix 截获
    ///     DelayRepair() 的求值,延迟读 ui.reload_fix_delay(钳 ≥0),修复体经
    ///     ModHelper.Delay(本 mod id,延迟,回调)排队(12/10 合法延迟通道,
    ///     mod 停用自动取消);__result 换成立即完结的空迭代器并跳过原方法——
    ///     StartCoroutine(空) 12/10 前无害跑完、不二次修复;若主 mod 未更新且
    ///     该 StartCoroutine 调用被新规则拒绝,异常由 OnGameLoaded 既有 try/catch
    ///     吞掉记日志,本补丁已在实参求值时完成排队(调度不丢)。延迟后的六段
    ///     修复体逐段复刻(EnsureLayout/RefreshMilkSprites/RepairAllAfterLoad/
    ///     CorruptTentacle/AlarmClock/TentacleTrap 三 RescanAll 各自 try-catch +
    ///     原版日志文案;前四段经 ModReflect.CallStatic、闹钟/陷阱两段经本类
    ///     自解析 Action 委托直调——1.6.1 ModEntry.cs L178-230)。
    ///     与原版差异(sanctioned):ModHelper.Delay 走 WaitForSecondsRealtime
    ///     (同 Task 7 夜报裁定),不受 timescale 影响。</item>
    ///  </list>
    /// 与原版的行为差异总则(sanctioned):配置读取集中在任何状态写入之前
    /// (Phase A);读失败 → 记日志放行原版/跳过修正;修正体(Phase B)异常
    /// 吞掉记日志。主 mod 类型(SpecialClients/ItemFactory/NameGen/ModEntry/
    /// TentacleBody/SpriteInjector/CorruptTentacle/AlarmClock/TentacleTrap/
    /// Toast/ModCtx)一律运行时经
    /// ModReflect 解析成委托/类型引用,不编译引用 MidnightPawn.*;游戏类型
    /// (StoreClient/GameItem/TagState/ModHelper)直接编译引用(Toast 在主 mod
    /// 在装配委托内(装配 = 物品目录初始化后,本 mod 已被加载器登记,
    /// ModReflect 合法),绝不进静态构造,两阶段提交(任一成员解析失败 → 原样
    /// 上抛不提交,五个条目一致不装,同 Task 7/8/9/10)。安装失败由 HookInstaller
    /// 逐单元隔离记 Dead,不阻断其余补丁。除 DelayRepair 调度面(换 ModHelper.Delay)
    /// 外无协程/Invoke 调用面。
    /// </summary>
    public static class SpecialAndManualPatches
    {
        /// <summary>names.custom 的自定义名字库路径(ModData/&lt;本 mod id&gt;/ 下,相对路径)。</summary>
        private const string CustomNamesPath = "names_custom.txt";

        // -- 主 mod 静态成员的运行时缓存(install 期经 ModReflect 填充,patch 只做直接调用) --
        private static bool resolved;                                  // 全部成员就位后才置真
        private static System.Type specialClients;                     // SpecialClients(IsAny 并集)
        private static System.Type itemFactory;                        // ItemFactory(说明书/驯服/修复)
        private static System.Type nameGen;                            // NameGen(names 重定向)
        private static System.Type mainEntry;                          // 主 mod ModEntry(DelayRepair)
        private static System.Type tentacleBody;                       // TentacleBody(修复体 EnsureLayout)
        private static System.Type spriteInjector;                     // SpriteInjector(修复体 RepairAllAfterLoad)
        private static System.Type corruptTentacle;                    // CorruptTentacle(修复体 RescanAll)
        private static System.Type toast;                              // Toast(主 mod,排卵诱发剂弹窗)
        private static Func<GameItem, string, string> ifGetStr;        // 主 mod ItemFactory.GetStr
        private static Func<GameItem, string, int> ifGetInt;           // 主 mod ItemFactory.GetInt
        private static Action<GameItem, string, int> ifSetInt;         // 主 mod ItemFactory.SetInt
        private static Action<string, string> toastShow;               // 主 mod Toast.Show(text, title)
        private static Action<object> mainLog;                         // ModCtx.Log
        private static Action<object> mainLogWarning;                  // ModCtx.LogWarning
        private static Action alarmRescanAll;                          // AlarmClock.RescanAll(修复段五直调)
        private static Action trapRescanAll;                           // TentacleTrap.RescanAll(修复段六直调)

        /// <summary>把五条目的装配委托登记进映射(catalog-id → install;Harmony
        /// 实例由安装器转交,返回本次补丁的 original 清单供记账)。</summary>
        public static void Register(Dictionary<string, Func<Harmony, MethodBase[]>> installs)
        {
            // -- 特殊顾客追加白名单(在原版 IsAny 之上并集) --
            installs.Add("main.union", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(specialClients, "IsAny",
                    new[] { typeof(StoreClient) });
                h.Patch(original, prefix: new HarmonyMethod(IsAnyPrefix));
                return new[] { original };
            });

            // -- 驯服阈值文案分支(UpdateMaidenDescription 接管 + ovulation_shot 委托换装) --
            installs.Add("main.tame_text", h =>
            {
                EnsureResolved();
                MethodBase desc = ModReflect.Target(itemFactory, "UpdateMaidenDescription",
                        new[] { typeof(GameItem) });
                MethodBase shot = ModReflect.Target(itemFactory, "CreateOvulationShot", Type.EmptyTypes);
                h.Patch(desc, prefix: new HarmonyMethod(UpdateMaidenDescriptionPrefix));
                h.Patch(shot, postfix: new HarmonyMethod(OvulationShotTamePostfix));
                return new[] { desc, shot };
            });

            // -- names 自定义文件重定向 --
            installs.Add("main.names_custom", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(nameGen, "EnsureLoaded", Type.EmptyTypes);
                h.Patch(original, prefix: new HarmonyMethod(EnsureLoadedPrefix));
                return new[] { original };
            });

            // -- 说明书渲染(创建点按当前配置渲染模板) --
            installs.Add("main.manual_render", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(itemFactory, "CreateNote", Type.EmptyTypes);
                h.Patch(original, postfix: new HarmonyMethod(CreateNotePostfix));
                return new[] { original };
            });

            // -- 读档修复延迟接管(Task 7 移交;接管语义:跳过原协程体,延迟修复体
            //    经 ModHelper.Delay 排队) --
            installs.Add("main.reload_fix", h =>
            {
                EnsureResolved();
                MethodBase original = ModReflect.Target(mainEntry, "DelayRepair", Type.EmptyTypes);
                h.Patch(original, prefix: new HarmonyMethod(DelayRepairPrefix));
                return new[] { original };
            });
        }

        /// <summary>
        /// 幂等解析全部主 mod 成员(五个条目共享;任一成员解析抛出 → 本方法原样
        /// 上抛、不提交任何字段,后续条目重试同样失败 → 全部条目一致不装,杜绝
        /// 半解析状态下装出运行时 NRE 的 patch——两阶段提交,同 Task 7/8/9/10)。
        /// 失败由 HookInstaller 逐单元隔离记 Dead,该组不装配。
        /// </summary>
        private static void EnsureResolved()
        {
            if (resolved) return;
            System.Type clients = FindMainType("SpecialClients");
            System.Type factory = FindMainType("ItemFactory");
            System.Type names = FindMainType("NameGen");
            System.Type entry = FindMainType("ModEntry");
            System.Type body = FindMainType("TentacleBody");
            System.Type injector = FindMainType("SpriteInjector");
            System.Type corrupt = FindMainType("CorruptTentacle");
            System.Type alarmClock = FindMainType("AlarmClock");
            System.Type trap = FindMainType("TentacleTrap");
            System.Type toastType = FindMainType("Toast");
            System.Type ctx = FindMainType("ModCtx");
            var resolvedGetStr = ModReflect.StaticMethod<Func<GameItem, string, string>>(factory, "GetStr");
            var resolvedGetInt = ModReflect.StaticMethod<Func<GameItem, string, int>>(factory, "GetInt");
            var resolvedSetInt = ModReflect.StaticMethod<Action<GameItem, string, int>>(factory, "SetInt");
            var resolvedToastShow = ModReflect.StaticMethod<Action<string, string>>(toastType, "Show");
            var resolvedLog = ModReflect.StaticMethod<Action<object>>(ctx, "Log");
            var resolvedLogWarning = ModReflect.StaticMethod<Action<object>>(ctx, "LogWarning");
            var resolvedAlarmRescan = ModReflect.StaticMethod<Action>(alarmClock, "RescanAll");
            var resolvedTrapRescan = ModReflect.StaticMethod<Action>(trap, "RescanAll");
            // 全部成功,一次性提交
            specialClients = clients;
            itemFactory = factory;
            nameGen = names;
            mainEntry = entry;
            tentacleBody = body;
            spriteInjector = injector;
            corruptTentacle = corrupt;
            toast = toastType;
            ifGetStr = resolvedGetStr;
            ifGetInt = resolvedGetInt;
            ifSetInt = resolvedSetInt;
            toastShow = resolvedToastShow;
            mainLog = resolvedLog;
            mainLogWarning = resolvedLogWarning;
            alarmRescanAll = resolvedAlarmRescan;
            trapRescanAll = resolvedTrapRescan;
            resolved = true;
        }

        /// <summary>在前置 lewd_meme 暴露的类型里按名查找(同名不同命名空间亦命中;
        /// "ModEntry" 是主 mod 的 MidnightPawn.ModEntry,本程序集同名类不在其列)。</summary>
        private static System.Type FindMainType(string name)
        {
            foreach (System.Type t in ModReflect.Types(MainModResolver.MainModId))
                if (t.Name == name)
                    return t;
            throw new MissingMemberException("主 mod lewd_meme 未暴露类型 " + name);
        }

        // ------------------------------------------------------------------
        // special.union:IsAny 并集前缀
        // ------------------------------------------------------------------

        /// <summary>
        /// IsAny(client) OR IsExtraSpecial(...):追加命中 → __result=true 跳过
        /// 原方法(true OR x = true);未命中/读失败 → 放行原版。client=null 时
        /// 原版恒 true,直接放行(并集无增量)。默认空 CSV → IsExtraSpecial
        /// 恒 false → 恒放行原版 = 零漂移。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool IsAnyPrefix(StoreClient client, ref bool __result)
        {
            if (client == null) return true; // 原版:null → true,无需并集
            string csvIds, csvSprites, csvKeywords;
            try
            {
                csvIds = Cfg(CfgKeys.SpecialExtraIds, "");
                csvSprites = Cfg(CfgKeys.SpecialExtraSprites, "");
                csvKeywords = Cfg(CfgKeys.SpecialExtraKeywords, "");
            }
            catch (Exception ex)
            {
                LogMainWarning("特殊顾客追加名单读取失败,放行原版: " + ex.Message);
                return true;
            }
            if (SpecialMatcher.IsExtraSpecial(client.identifier, client.spriteName,
                    client.displayName, csvIds, csvSprites, csvKeywords))
            {
                __result = true;
                return false;
            }
            return true;
        }

        // ------------------------------------------------------------------
        // maiden.tame_text:UpdateMaidenDescription 接管 + ovulation_shot 委托换装
        // ------------------------------------------------------------------

        /// <summary>
        /// prefix:默认(阈值 = 60)放行原版(零漂移);≠60 跳过原方法走复刻体。
        /// 复刻体异常吞掉记日志(原版方法体本身整体 try-catch,文案相同),不放行
        /// 原版——shortDescription 已可能写入一半,但该字段是幂等赋值,重放只会
        /// 用旧阈值重写同一段文案,跳过即可。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool UpdateMaidenDescriptionPrefix(GameItem item)
        {
            int threshold;
            try
            {
                threshold = Cfg(CfgKeys.TrainTameThreshold, 60);
            }
            catch (Exception ex)
            {
                LogMainWarning("驯服阈值读取失败,放行原版: " + ex.Message);
                return true;
            }
            if (threshold == 60) return true; // 默认 = 原版字面量 60
            try
            {
                UpdateMaidenDescriptionBody(item, threshold);
            }
            catch (Exception ex)
            {
                LogMainWarning("更新少女描述失败: " + ex.Message); // 原版 catch 同文案
            }
            return false;
        }

        /// <summary>逐行复刻 v1.3.8 UpdateMaidenDescription;差异仅 60 → threshold。
        /// 文案常量 = 反编译 L520-530 逐字。</summary>
        private static void UpdateMaidenDescriptionBody(GameItem item, int threshold)
        {
            if (item == null)
            {
                return;
            }
            string name = ifGetStr(item, "MAIDEN_NAME");
            string faction = ifGetStr(item, "MAIDEN_FACTION");
            if (ifGetInt(item, "TRAIN_VALUE") >= threshold)
            {
                item.shortDescription = name + "已经被你调教得服服帖帖的，离开了你的精液就无法生存。";
                return;
            }
            string text = "被触手捕捉的" + name + "，被触手捆绑得动弹不得，成了你的专属物品。";
            item.shortDescription = faction switch
            {
                "上层区" => text + "上层人的高贵身份在这里只能让你变得更加兴奋。",
                "治安部" => text + "曾经拿枪指着别人的人，现在连合上腿都做不到。",
                "黑市" => text + "黑市里什么都能买卖，包括她自己。",
                "革命军" => text + "她高喊着解放，却先被你解放了。",
                _ => text + "她本来就是这座空间站底层的一块肉。",
            };
        }

        /// <summary>排卵诱发剂 &lt;60 分支文案(反编译 L288 逐字;含 U+201C/U+201D 弯引号、
        /// U+2665 ♥、ASCII ~;两段共用同款尾巴)。</summary>
        private const string OvuAngryText =
            "恶狠狠地盯着你，被触手牢牢缠绕捆绑的四肢不断发力，张嘴想要继续威胁咒骂你，而同时你的一根触手卷起排卵诱发剂，快速地扎在了她的后颈，并且将其全部注入。\n\n“呃啊♥~~你这怪物♥~~对我♥~~嗯♥~~用了♥~~什么♥~~噫噫噫♥~~”";

        private const string OvuTameText =
            "目光迷离地望着着你，主动手指抓握、双腿缠绕着你的触手，不断在上面摩挲，张嘴呼出一道香风，主动张开双臂，对你露出最为脆弱且不设防的小腹，让你的一根触手卷起排卵诱发剂，快速地扎在了她的小腹，并且将其全部注入子宫。\n\n“嗯啊♥~~主人♥~~来吧♥~~我想要怀上♥~~嗯♥~~主人的女儿♥~~噫噫噫♥~~”";

        private const string OvuTailText =
            "的身体在你触手的包裹下左右扭动，小腹不断起伏抽搐，直接进入了排卵期。";

        /// <summary>
        /// postfix:阈值字面量 60 在原版 onThisTargetItemFunc 委托体内(编译器生成
        /// lambda,无法稳定定位,同 Task 10 price.milk_credit 先例)。默认 60 →
        /// 不换装(原版委托原样保留,零漂移);≠60 → 换装等价委托(唯一差异:
        /// 分支阈值读配置;日志出口由 mainLog 委托复刻同文案,ModCtx 直连)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void OvulationShotTamePostfix(GameItem __result)
        {
            if (__result == null) return;
            int threshold;
            try
            {
                threshold = Cfg(CfgKeys.TrainTameThreshold, 60);
            }
            catch (Exception ex)
            {
                LogMainWarning("排卵诱发剂阈值读取失败,保持原版委托: " + ex.Message);
                return;
            }
            if (threshold == 60) return; // 默认:原版委托(字面量 60)零漂移
            __result.onThisTargetItemFunc = delegate (GameItem self, GameItem target)
            {
                if (target != null && target.IsTag("MAIDEN_FEMALE"))
                {
                    string name = ifGetStr(target, "MAIDEN_NAME");
                    if (string.IsNullOrEmpty(name))
                    {
                        name = target.name;
                    }
                    int trainValue = ifGetInt(target, "TRAIN_VALUE");
                    string message = (trainValue < threshold)
                        ? name + OvuAngryText + name + OvuTailText
                        : name + OvuTameText + name + OvuTailText;
                    target.ModifyTag("MAIDEN_OVULATING", delegate (TagState s) { s.Enable(); }, false);
                    ifSetInt(target, "MAIDEN_PREGNANT_DAYS", 0);
                    toastShow(message, "排卵诱发剂");
                    LogMain("排卵诱发剂生效：" + name + " 进入排卵期 (调教值 " + trainValue + ")");
                    ModHelper.PlaySound(MainModResolver.MainModId, "semen_apply");
                    self.Destroy();
                }
            };
        }

        // ------------------------------------------------------------------
        // names.custom:EnsureLoaded 重定向(ModData 自定义名字库)
        // ------------------------------------------------------------------

        /// <summary>
        /// prefix:names.use_custom_file=false → 放行原版;true 且尚未加载 →
        /// 读 ModData/lewd_meme_config/names_custom.txt,成功写入 regions/loaded 并
        /// 跳过原版;文件缺席/为空/无有效行/任何异常 → 日志警告 + 放行原版
        /// (回退 Content/names.txt)。已加载(loaded=true,含本补丁先前写入)→
        /// 放行原版(原方法同样 no-op,会话内缓存语义同原版)。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool EnsureLoadedPrefix()
        {
            bool useCustom;
            try
            {
                useCustom = Cfg(CfgKeys.NamesUseCustomFile, false);
            }
            catch (Exception ex)
            {
                LogMainWarning("名字库开关读取失败,放行原版: " + ex.Message);
                return true;
            }
            if (!useCustom)
            {
                return true;
            }
            try
            {
                if (ModReflect.GetStatic<bool>(nameGen, "loaded"))
                {
                    return true;
                }
                if (LoadCustomNames())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogMainWarning("自定义名字库加载失败,回退原版: " + ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 读 ModData/lewd_meme_config/names_custom.txt 并按原版 TSV 语义解析
        /// (切 \n → Trim → 跳空行 → 切 \t → ≥3 列入列),成功写入主 mod
        /// NameGen 静态字段并返回 true;任一步不成立返回 false(调用方回退原版)。
        /// 文件访问走游戏文件 API(零能力),不用 System.IO。
        /// </summary>
        private static bool LoadCustomNames()
        {
            if (!ModHelper.ModDataFileExists(ModEntry.Id, CustomNamesPath))
            {
                LogMainWarning("自定义名字库不存在(ModData/" + ModEntry.Id + "/" + CustomNamesPath
                    + "),回退原版 Content/names.txt");
                return false;
            }
            string text = ModHelper.ReadModDataFileText(ModEntry.Id, CustomNamesPath);
            if (string.IsNullOrEmpty(text))
            {
                LogMainWarning("自定义名字库为空,回退原版 Content/names.txt");
                return false;
            }
            List<string[]> regions = new List<string[]>();
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.Length != 0)
                {
                    string[] columns = line.Split('\t');
                    if (columns.Length >= 3)
                    {
                        regions.Add(columns);
                    }
                }
            }
            if (regions.Count == 0)
            {
                LogMainWarning("自定义名字库无有效行(需 TSV: 地区<TAB>姓列表<TAB>名列表),回退原版");
                return false;
            }
            ModReflect.SetStatic(nameGen, "regions", regions);
            ModReflect.SetStatic(nameGen, "loaded", true);
            LogMain("自定义名字库已加载: " + regions.Count + " 个地区 (ModData/" + ModEntry.Id + "/" + CustomNamesPath + ")");
            return true;
        }

        // ------------------------------------------------------------------
        // manual.render:CreateNote postfix(模板 + 当前配置渲染)
        // ------------------------------------------------------------------

        /// <summary>说明书模板引用的全部配置键(创建点先整表读取,任一失败 →
        /// 保留原描述不重写,杜绝半渲染的占位符/数值混排)。</summary>
        private static readonly string[] ManualKeys =
        {
            CfgKeys.CorruptCols, CfgKeys.BroodSize, CfgKeys.OutputSize, CfgKeys.ToolCols,
            CfgKeys.SexMinBase, CfgKeys.SexMaxBase,
            CfgKeys.OralMinBase, CfgKeys.OralMaxBase,
            CfgKeys.AnalMinBase, CfgKeys.AnalMaxBase,
            CfgKeys.SemenChanceBase, CfgKeys.MilkValueBase, CfgKeys.MilkValuePerLv,
            CfgKeys.OvulateChance, CfgKeys.GestationDays,
            CfgKeys.CorruptDaily, CfgKeys.CorruptSemen, CfgKeys.CorruptMaxCharge,
        };

        /// <summary>
        /// postfix:整表读取 ManualKeys(注册表未注入 → 保留原描述——原描述即
        /// 主 mod 硬编码默认值,与默认渲染逐字一致,零漂移);全部到手后按
        /// ManualTextRenderer.Template 渲染(键感知格式化,InvariantCulture)。
        /// 渲染异常同样保留原描述。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPostfix]
        private static void CreateNotePostfix(GameItem __result)
        {
            if (__result == null) return;
            ConfigRegistry cfg = PatchCtx.Config;
            if (cfg == null) return; // 未注入:原描述 = 默认渲染,零漂移
            var values = new Dictionary<string, string>();
            try
            {
                foreach (string key in ManualKeys)
                {
                    values[key] = ManualTextRenderer.FormatValue(key, cfg.Current<object>(key));
                }
            }
            catch (Exception ex)
            {
                LogMainWarning("说明书配置读取失败,保留原版描述: " + ex.Message);
                return;
            }
            try
            {
                __result.shortDescription = ManualTextRenderer.Render(
                    ManualTextRenderer.TemplateKo,
                    key => values.TryGetValue(key, out string value) ? value : null);
            }
            catch (Exception ex)
            {
                LogMainWarning("说明书渲染失败,保留原版描述: " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // ui.reload_fix:DelayRepair 调度接管(协程 → ModHelper.Delay)
        // ------------------------------------------------------------------

        /// <summary>
        /// prefix(截获 OnGameLoaded 的 StartCoroutine(DelayRepair()) 实参求值):
        /// 读 ui.reload_fix_delay(钳 ≥0)→ ModHelper.Delay 排队修复体;__result
        /// 换成立即完结的空迭代器并跳过原协程体(防止 1.5s 后二次修复)。
        /// 排队失败仍接管(不回放原协程:12/10 后 StartCoroutine 路径不可靠),
        /// 记日志由玩家可见。空迭代器让 12/10 前的 StartCoroutine 调用无害跑完。
        /// </summary>
        [HarmonyPriority(400)]
        [HarmonyPrefix]
        private static bool DelayRepairPrefix(ref IEnumerator __result)
        {
            float delay;
            try
            {
                delay = Cfg(CfgKeys.UiReloadFixDelay, 1.5f);
            }
            catch (Exception ex)
            {
                LogMainWarning("读档修复延迟读取失败,沿用原版 1.5s: " + ex.Message);
                delay = 1.5f;
            }
            if (delay < 0f) delay = 0f;
            try
            {
                ModHelper.Delay(ModEntry.Id, delay, RunReloadRepair);
                LogMain("读档修复: 已按 ui.reload_fix_delay="
                    + delay.ToString("0.###", CultureInfo.InvariantCulture) + "s 排队");
            }
            catch (Exception ex)
            {
                LogMainWarning("读档修复排队失败: " + ex.Message);
            }
            __result = EmptyCoroutine();
            return false;
        }

        /// <summary>立即完结的空迭代器:喂给原版 StartCoroutine 调用点,使其无害。</summary>
        private static IEnumerator EmptyCoroutine()
        {
            yield break;
        }

        /// <summary>
        /// 原 DelayRepair 协程体 yield 1.5s 之后的部分,逐段复刻 v1.6.1
        /// (ModEntry.cs L178-230):六段修复各自 try-catch,日志文案与原版
        /// 逐字相同(前四段自 v1.3.8 起不变);前四段经 ModReflect.CallStatic、
        /// 闹钟/陷阱两段经 EnsureResolved 自解析的 Action 委托直调,均回原
        /// 实现(零漂移)。
        /// </summary>
        private static void RunReloadRepair()
        {
            TryRepair(tentacleBody, "EnsureLayout", "本体布局检查异常");
            TryRepair(itemFactory, "RefreshMilkSprites", "刷新乳汁图标异常");
            TryRepair(spriteInjector, "RepairAllAfterLoad", "读档修复异常");
            TryRepair(corruptTentacle, "RescanAll", "触手登记异常");
            TryRepair(alarmRescanAll, "闹钟登记异常");
            TryRepair(trapRescanAll, "陷阱登记异常");
        }

        /// <summary>单段修复:调用 + 原版同文案异常日志(吞掉,不阻断后续段)。</summary>
        private static void TryRepair(System.Type target, string method, string warnLabel)
        {
            try
            {
                ModReflect.CallStatic(target, method);
            }
            catch (Exception ex)
            {
                LogMainWarning(warnLabel + ": " + ex.Message);
            }
        }

        /// <summary>单段修复(委托直调版):闹钟/陷阱两段用 EnsureResolved 解析的
        /// Action 委托直接调用,异常文案模式与类型版一致(吞掉,不阻断后续段)。</summary>
        private static void TryRepair(Action step, string warnLabel)
        {
            try
            {
                step();
            }
            catch (Exception ex)
            {
                LogMainWarning(warnLabel + ": " + ex.Message);
            }
        }

        // ------------------------------------------------------------------
        // 共用小件
        // ------------------------------------------------------------------

        /// <summary>配置读取:注册表缺席(未注入/停用)时回退原版字面量默认值。</summary>
        private static T Cfg<T>(string key, T def)
        {
            return PatchCtx.Config != null ? PatchCtx.Config.Current<T>(key) : def;
        }

        /// <summary>日志走主 mod ModCtx(保持原版前缀与格式);委托缺席时退回 Debug.Log。</summary>
        private static void LogMain(object msg)
        {
            if (mainLog != null) mainLog(msg);
            else UnityEngine.Debug.Log("[lewd_meme_config→主mod] " + msg);
        }

        private static void LogMainWarning(object msg)
        {
            if (mainLogWarning != null) mainLogWarning(msg);
            else UnityEngine.Debug.LogWarning("[lewd_meme_config→主mod] " + msg);
        }
    }
}
