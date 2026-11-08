# 香烟拓展维护指南

本项目使用官方 IMod 加载器，酿酒拓展为必需前置。工坊发布基线与验证证据见 [AGENTS.md](AGENTS.md) 顶部的最新发布记录。截至 2026-11-06，工坊版本为 0.5.3.7。

## 改什么，去哪里

| 修改目标 | 入口 |
| --- | --- |
| 启用/禁用、原生事件、物品目录注册 | Mod.cs；目录工厂只在此注册 |
| 物品介绍、机器说明、固定按钮与台词 | Localization/ 下的七种语言表，Core/ModText.cs 调用官方语言接口 |
| 语言变化后的自有文字刷新 | Core/LanguageRuntime.cs；UI/LocalizedViews.cs登记自有窗口 |
| 物品提示底部的模组来源署名 | Mod.cs的运行时清单名称；沿用StartupReadiness门控和LanguageRuntime刷新，名称来自七语te_mod_name，停用恢复原名 |
| 材料售价、占格、素材与语言键 | Configuration/ItemDefinitions.cs |
| 每包用料、耗电、切丝产出、公司价格与贴牌概率、委托奖励 | Configuration/GameplaySettings.cs |
| 果味品种、身份、等级映射 | Items/Rules.cs |
| 物品创建、品质、读取刷新 | Items/Items.Factory.cs、Items.Quality.cs、Items.Refresh.cs |
| 动态物品说明的拼接、文字刷新登记 | Items/ItemText.cs；物品存档标签在ItemState.cs |
| 烘干、制烟、过夜、消耗回滚 | Machines/；ProductionTransaction在Manufacturing.cs |
| 委托入口与每日调度 | Orders.cs，保留原调用入口 |
| 委托存档数据、读写、生成、交货、奖励 | Commissions/OrderModels、OrderRepository、OrderGeneration、OrderSubmission、OrderRewards |
| 委托奖励种子稀有度权重与选种 | Configuration/GameplaySettings.cs、SeedRewardRules.cs；OrderCatalog按实际可用工厂提供品种与等级 |
| 委托人酒质门槛 | GameplaySettings.MinimumCommissionWineScore、TobaccoOrder.MinimumWineScore；所有选货／扣货检查和详情必须传入对应委托门槛 |
| 每日3烟3酒及二选一保底 | OrderGeneration.DailySlot／Guaranteed、OrderBook.DailyOfferVersion；AlternativeRequirements以首条有提交记录的要求锁定路线，RequireFlavored排除基础烟。交付统一使用TobaccoOrder.MatchesLine，完成判断兼容既有AND委托 |
| 特殊委托资格、烟酒目录 | Commissions/SpecialCommissions.cs、OrderCatalog.cs |
| 委托窗口布局、贴图、桌上机体 | UI/Commissions/ |
| 顾客、烟贩、辰子、商店、喜好和预算 | Trading/ |
| 印章、贴牌、烟况与化学成分 | Items/与UI/Inspection/ |
| 故事、鉴定手册与页码 | UI/Reading/ |
| 英文故事正文与中文原稿保留 | UI/Reading/LocalizedStoryPage.cs；Localization中的te_story_*_page_* |
| 酿酒联动与同步核对步骤 | Integration/Brewing/README.md |
| 日志、安全会话、库存与素材辅助 | Core/ |
| 自有物品提示的字宽测量和临时排版恢复 | UI/ItemTooltipLayout.cs，Mod.cs 订阅官方晚期提示事件 |

`Items`分部文件属于同一类型，供现有回调和原生补丁继续使用；按创建/品质/刷新拆分，避免改变调用身份。`Orders`作为薄入口转交各职责模块，存档模型独立保留；OrderLegacy仅保留历史测试调用，不注册访客/柜台交货。

## 文案规则

CSV 每个键一条物理行，段落换行写字面量 `\n`。七种语言的键集合必须一致，`{0}` 等占位符须对应同一格式参数。特定语言有意省略参数时记录例外。文字由官方接口按启动语言解析，英文作为其它语言的回退。贴图路径、存档键和物品 ID 属于程序标识。

当前维护中文、英文、俄语、法语、日语、德语及波兰语。固定界面、动态委托、台词及故事集中到语言表。名称、数量单位、期限与化学数值使用完整格式模板，方便译者调整语序。原生 CSV 会裁剪引号外的空白，引号内的首尾空白会保留。规则表中数值变更时，同时核对七语说明。构建前运行 `python scripts/verify_tobacco_structure.py` 检查七语文案和构建边界。动态拼接的键与实际呈现按回归清单验证。

中文故事继续使用作者原有页面贴图；其它语言用同一书本内的实时文字分页，保留三本10／17／13页的结构和原有物品身份。故事译文中的固定宽度软换行应重新排版，保留原文内容；不能靠持续缩小字体塞入页面。鉴定手册按zh/en选择原有图册，新语言暂用英文图版，不能将CSV接入描述为所有图片文字已本地化。语言事件仅刷新自己的物品和窗口，不重建库存、不改变价值与品质，也不增加Update扫描或全局本地化补丁。

自建实时文字优先使用`ModText.UiFont`，通过原版`FontLoader.GetFontByRole`按玩家语言取字体；不能直接用默认西文字体绘制日文。故事标题固定27、正文固定24，标题单行／双行按实际高度分配正文空间。自有中文／日文书本页码的字体覆盖须可重复刷新，并在关闭时恢复原字体与材质；不修改其它书本或全局字体。机器说明仍用原版纸张界面，检查缺字和长文边界。

冷启动中英文分别验证。运行中更换语言时，当前测试组合仍可能触发原版StoreCalendar取词条的无效句柄错误；移除香烟新语言监听的对照中也能复现。不能把这一对照扩大解释成纯原版验证，或宣称运行中切换已全面修复。玩家更改语言后应完全退出重启。

## 维护边界

- 修改某个功能只改它的目录；共享设备修复首先在前置处理，香烟只保留自己的适配。
- 调整界面不改变原生容器的保存顺序；调整文案不改变物品身份与数值。
- 制作、交货与奖励必须确认实际消耗/放置后记录结果；失败和柜台满时可重试，防止重复支付。
- 使用官方事件、物品局部回调；无持续Update扫描。需要原生补丁时说明具体接口缺口。
- `LegacyExcluded`不参与生产编译，临时运行审计组件放在research，发布包只包含生产DLL。
- 运行测试按修改范围选择。涉及结构或存档时，使用相关已发布的生产 DLL 生成隔离样本，退出进程后在新 DLL 上读取、保存，再重启复读。

## 正式版回归清单

| 改动范围 | 复测路径与通过条件 |
| --- | --- |
| 品质、标签、喜好与交易 | 分别走原版摆放预览、确认鉴定、重新报价和最终成交。覆盖单件、批量、随箱、锁后排除与取消，核对付款、归属和声望。 |
| 委托选货与奖励 | 覆盖同名不同价、嵌套和锁后库存、确认前移货、部分提交、柜台满后重试。核对实际扣除、进度和奖励次数。 |
| 商店库存、机器与工具 | 分别使用货架、待结账、已拥有及原生读档入口。用盒装卷烟纸覆盖未付款拒收、结账后入机与加工耗纸，再测缺料、缺电、成品区满和回调取消。 |
| 持久化结构与过夜 | 用首个正式工坊版或受影响的正式发布版生成针对性样本，保留 ID、UID、槽位、标签、归属、电量与委托进度。跨进程验证升级、保存和重载，核对多机结算与切档后状态。 |
| 游戏更新、前置、语言或界面 | 核对受影响的精确 API 签名、逐清单权限、启动和依赖加载。对改动语言验证实际词条与布局，对点击、拖放和穿透分别记录实际输入方式。 |
| 资源与发布包 | 用干净暂存目录核对关键精灵键在 `Textures/Items`，排除审计组件。已获授权发布后，重新下载核对文件哈希与前置关系。 |

每项记录样本、入口、预期、实际、失败收尾和未覆盖范围。DLL 及游戏环境按实际测试记录绑定。界面只跑回调时，结果标为回调检查。

详细工程规则见AGENTS.md，迁移历史见ENGINEERING.md与MIGRATION_REVIEW.md。历史记录不代替当前构建的验收结果。
