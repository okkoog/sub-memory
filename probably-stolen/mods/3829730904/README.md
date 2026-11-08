# 酿酒拓展

本机开发版本：3.6.8；工坊当前版本3.6.7.9。使用游戏自带的模组加载器，无需 MelonLoader。兼容维护范围从酿酒拓展正式版首个公开发布版本开始；不兼容 Demo/MelonLoader 旧档，也不提供其存档迁移。

当前提供 8 份语言表、每份 551 项文本，覆盖七种游戏语言和中文简繁选项。

外环区全部 15 个正负声望阶段补齐对应的风味描述，原生声望页面底部不再出现缺失翻译提示。现有效果、数值、解锁进度及香烟联动说明保持不变。

## 3.6.8 开发版：原浆品质总分

鼠标查看原浆时显示实际保存的品质总分，支持负数和零分。已有总分标签的旧原浆同样显示；未配置的空白调试原浆不显示虚构分数。不改变酿酒计算、物品估值或存档结构。玩家反馈的读档异常仍在调查，本改动不是该问题的修复。

## 3.6.8 开发版：流年界面兼容

修正流年创建时将模块区内容强制当成原生投料框的问题。模块区或存放区被外部布局包装时，保留原有库存对象及回调，关闭这部分网格替换优化；普通原生布局继续使用原有优化。容量、陈化和冰镇机制、耗电、说明书与库存节点顺序不变。修正因此导致的酿酒商店仅上架前五台机器的问题。已在受控包装条件下复现原异常并验证修正，使用玩家存档副本做对照；尚未完整复现玩家的62模组组合，原环境仍需回测。未添加跳过存档物品的处理。

## 3.6.7.9 开发版：访客与夜间商店

酿酒商店新上架的 xuxu 酿造桶基础价值从 2800 降为 2000，继续应用原版难度加价与当天折扣。流年仍为 6666。已经保存的酿造桶保留原有估值和内容，不强制重写玩家资产。

四十七无证书时的基础种子数量从 3 颗改为 4 颗，初级／中级／高级证书仍为 7／9／13 颗，声望及特性的加减不变。酿酒灯牌现在沿用原版灯牌标签，可收进档案箱；旧正式版存档中的灯牌读档时也会补齐标签。只有展示在橱窗里的灯牌才招来四十七。

酿酒商店接入官方夜间地点事件系统：每天 20% 概率出现仅限当天的价格降低 10% 活动，地图卡片显示原版折扣提示。沿用原版价格阶段，与难度加价相加计算（例如普通难度 +15% 与活动 −10% 合计 +5%），不修改物品基础价值，不影响泥泥收购、白天四十七或博士商店。事件持续时间与读档恢复由原版管理。

## 3.6.7.9 开发版：孵化器果实品质

孵化器产出的酿酒果实和原版荧光莓逐颗抽取 −1、1 或 2 分，按持有的最高酿酒证书决定概率。无证书为 30%／50%／20%，初级为 10%／60%／30%，中级为 5%／60%／35%，高级为 0%／30%／70%。品质可在普通物品提示查看。仅新孵化的果实抽取品质，已有果实保留保存的分数。

## 3.6.7.9 水培试验：巨大果实投料

原浆机在普通投料和原浆输出之间增加了 3×3 的巨大果实槽，一次可放入一颗。手动或自动浓缩一颗巨大果实可得到一份对应原浆。四颗普通果实仍走原投料区；原有的电池、投料、原浆输出和说明书库存顺序保留。此批次只接入巨大果实和原浆机加工，水培收获与种子改造仍在研究。

## 3.6.7.8：中文简繁选项

在主菜单「模组」选择酿酒拓展，点击齿轮，开启「中文使用繁體」，点击应用并完全退出、重启游戏。关闭则使用简体。该选项仅影响游戏选中文时的酿酒文本，其他语言继续跟随游戏；设置通过官方ModConfig单独保存，不写入物品状态或改变存档结构。

## 3.6.7.7：恢复烟酒行情事件

玉兔怀袖保留20×15格（300格）容量，界面显示7行，可用滚轮或右侧滚动条查看其余格子，避免外出背包界面遮挡商店柜台。保留原生库存节点编号、物品UID和已有位置；其他背包不变。

恢复烟酒资源短缺（+50%）、烟酒资源过剩（-50%）、上层区派对狂欢（+25%）及上层区烟酒过剩（-25%）四种事件。直接注册原版普通事件池，权重各5，持续3天、冷却7天，四者互斥；原版负责抽取、计价及保存。不新增事件抽取补丁，不更改已有物品和存档结构。

## 3.6.7.6：泥泥识别原版香烟标签

修正泥泥的额外分类筛选漏掉原版CIGARETTE_TAG的问题，恢复收购原版香烟和使用该标签的香烟拓展成品烟。保留原生黑名单及其它拒收检查，不改变其他顾客、物品类型、价格、ID或存档字段。以前测试只覆盖模拟类型，现改用真实原版与香烟拓展工厂产物验证。

## 3.6.7.5：公开孵化配方接口

向联动模组提供标准服务接口，允许注册和注销自己的孵化配方，不必引用酿酒DLL或反射内部类型。保留原版与酿酒配方的优先权、配额、耗电和事务检查；不调整现有物品、玩法数值或存档结构。用于配套香烟0.5.3.3移除反射权限，旧版香烟仍可使用原有适配。

支持中文、英文、俄语、法语、日语、德语和波兰语，跟随原版语言设置。切换语言后请完全退出游戏再启动，让已创建的物品、对话和原版液体成分缓存使用同一语言。俄语、法语、日语、德语和波兰语初译由作者通过DeepSeek提供，语言接入及代码提示使用官方接口。

作者：xuxu。QQ交流群：1125708289；QQ交流2群：980900157。

## 3.6.7.4 传输线、文案与商店修复

- 更新13款酒的五语言风味介绍，保留原有配方、价格、属性及存档记录。

- “xuxu赞助”新开局额外赠送一块满电的原版普通电池（100电量）；已有存档不追补开局奖励。

- 酿酒商店出售商品跟随原版博士商店的难度加价：休闲0%、普通15%、困难50%、挑战100%；原生交易面板显示供货商加价，不影响泥泥收购和白天四十七交易。

- 新获得的酿酒证书直接发到柜台，已归玩家所有；柜台放不下时保留进度，腾出位置后于下次交易、泥泥来访或读档时补发。原有证书位置、领取记录及1500/3000/8000的逐级累计要求不变。

- 关闭自动打开议价界面时，点击酿酒商店里的四十七可打开原生交易界面；保留玩家的自动打开设置。沿用原版博士商店的StoreTablet交易操作，不新增全局补丁。
- 修正外语介绍中可能缺字的括号，保留DeepSeek新增介绍与风味文字；修复桂花酒、杨梅酒CSV中的跨行格式，统一使用游戏支持的转义换行。五仁月饼目前没有食用操作，介绍改为可售卖或收藏。


- 按“来源设备 → 酿造桶”分组显示连接，包括原浆浓缩器、酒瓶打印机和储水容器。各类来源分别编号，已连接的酿造桶统一编号，方便分辨不同设备。
- 跟随原版改名工具的自定义名称显示连接对象；自动编号不写进或覆盖玩家的自定义名称。连接、断开、出售、销毁和读档后更新提示，多根传输线保持同步。
- 最多展开3组来源、每组3个桶，超出时显示省略及剩余组数。未连接时明确提示。按实际字宽排版，避免中外文混排提前换行，五种语言均支持。
- 沿用已有连接记录、物品ID和库存序列化顺序，不改变实际供水、原浆传输、打印或酿造规则。来源编号与提示快照仅在内存中维护；鼠标查看提示时不遍历玩家整个库存，也不轮询改名。

<!-- catalogue-introduction:start -->
## 模组内容

从培育原料、调配酿酒，到自动生产、陈酿收藏和烟酒交易，体验一套完整的酿酒经营玩法。目前新增180种物品，包含：

【专属开局：新人酿酒师】带着普通酵母×4、空酒瓶×4、清酿澄水箱、酿酒灯牌、水霓虹灯牌、普通过滤器和内含10支试纸的水质检测试剂盒，开启酿酒生涯。

【5项开局特质】
一心为酒：出售烟酒价格提高20%；每拥有一瓶成品酒再提高2%，最多额外提高80%，但出售其他物品价格降低25%。
酩酊大醉：烟酒交易价格提高30%，但每晚随机失去两件库存物品。
厌恶种子：四十七每次上架的种子减少2颗。
xuxu赞助：开局获得一台水质扫描仪、一台种子孵化器和一块满电的原版普通电池（100电量）。
xuxu音乐：开局获得一个xuxu磁带捆绑包。

【种植与酿酒】16种新增种子、18种果实与特殊酿酒原料、57款配方酒，以及10款收藏酒，合计67款新增酒。尝试单一原料、混合配方与特殊酵母，酿出不同风味的酒；酒会保留酿造用水的快感与毒性，并可继续陈酿、冰镇和收藏。

【生产与自动化】6台核心机器：种子孵化器、原浆浓缩器、xuxu酿造桶、酒瓶打印机、xuxu水瓶打印机和流年酒柜。孵化器还支持7种原版种子；水瓶打印机可升级，解锁6L、12L和18L水瓶。使用xuxu传输线连接原浆机、酒瓶打印机、水容器与酿造桶，自动输送原浆、空瓶和补水；流年可消耗电量为酒加速陈化并冰镇，使用模块进一步提升性能。各机器自带详细使用说明。

【净水与储水】全套净水储水设备：8款储水容器，容量涵盖6L至60L；3款可每晚恢复耐久的滤芯和3款便携净水器。自由搭配储水、过滤与酿酒供水设备。

【电池与收纳】3档每晚自动充能的xuxu电池；多功能收纳仓、xuxu迷你储存区、四十七「小」暗格等3款储存设备，以及5款背包。三款储存设备均可使用原版扩建套件，每次增加一页、最多5页；多功能收纳仓和小暗格可躲过治安部搜查，收纳仓与迷你储存区还可切换外观。

【独立NPC与商店】新增泥泥、四十七及独立的四十七酿酒商店。挂上酿酒灯牌吸引四十七，购买种子、配方和设备；取得泥泥名片后，可打电话预约上门收购烟酒，对话会进入录音记录。交易积累外环区声望，与泥泥或四十七每次交易最多增加10点，逐步获得酿酒证书、提高种子供应和NPC预算，奖励延伸至200声望；隐瞒问题香烟的品相可能被泥泥识破并扣声望。

【成长与实用道具】3档酿酒证书、8份配方、酿酒手册、3款酵母反应器与特殊酵母，为种子培育和酿酒生产提供更多选择。

【收藏与趣味内容】8种盲盒、月饼礼盒及磁带套装，涵盖种子、收藏酒、容器、手办和中秋主题奖励；4款增加店铺吸引力的手办、5盘音乐磁带、3款用于影响交易或降低犯罪记录的香水，以及徽章、灯牌、月饼和节日装饰。

作者：xuxu
QQ交流群：1125708289
QQ交流2群：980900157
<!-- catalogue-introduction:end -->

中秋盲盒每5天可购买一次，沿用上次购买日期计算。种子孵化器支持原版7种种子（包括兼作种子的咖啡豆）：每消耗一颗产出2个常规对应作物，hydroreed种子产出2个原版滤芯。本模组种子仍产出1个对应果实。每日处理上限按种子颗数计，每个成功产出的夜晚整台消耗12kJ；放不下完整两件产物时保留种子且不额外扣电。

多功能收纳仓、xuxu迷你储存区、四十七「小」暗格可使用原版储藏区扩建套件：每次增加一页，总共最多5页，每页保持原有容量。支持原版翻页、页名和保存恢复，原有物品和外观切换不受影响。

多功能收纳仓和四十七「小」暗格的物品介绍明确标注：存放在里面的物品可以躲过治安部的搜查；扩建后的所有页面同样生效。迷你储存区保持普通储存属性。

## 本次修正

- 3.6.7.3同批新增法语、日语支持：五种语言各504项文本，沿用官方语言表自动识别。核对并修正机器品质、电池容量、补货天数、原料与特质加成措辞，统一日语机器用词，并调整法语长说明与原版字体不支持的字符。切换语言后仍需保存并完全退出游戏再启动。

- 3.6.7.3：加入俄语物品、机器说明、对话、开局特质与外环区声望文本。电话冷却提示改用带天数占位符的完整句子，覆盖同日再次致电时的5天提示；修正流年质量、电池容量用词和部分署名的字体显示。清理原浆机、酒瓶打印机和流年占位行的英文状态前缀。保留所有物品身份、库存结构和玩法数值。 本批进一步将代码中的85项文本统一迁入官方语言表，以固定键名和完整句子占位符读取；移除按中文原句匹配的俄语字典及手写语言分支。保留已有译文，五种语言各504项，缺项使用官方英文回退规则。切换游戏语言后请保存并完全退出游戏，再重新启动。
- 3.6.7.2：泥泥收购判定兼容已核对的单参数与双参数原版接口，避免部分正式版安装在注册补丁时找不到方法而整体加载失败；保持原版拒收和其他顾客规则。原版模组舱扩建套件不能对本模组物品使用，不消耗套件或改变机器价值；储藏区扩建套件保持原功能。旧流年模块扩建记录在物品均处于原6×20范围内时就地恢复；若有越界物品，保留原布局与内容，须先移回原区域后再读档恢复。
- 3.6.7.1：修复4747号码下残留旧泥泥标识的电话联系人，恢复正式版名称及接听回调，保留原联系人冷却、状态和记录。联系人存在时即使名片不在手中也能修正显示，但预约仍需持有名片。名称查询仅对泥泥跳过原生ClientName中不存在的键；其他联系人保持原规则。
- 3.6.7：在注册液体适配前，等待原生主菜单应用玩家语言选择，再异步确认该语言及回退语言的Mechanic表；加载期间语言变化会重新等待，防止提前缓存默认英文成分名。不重建液体、不修改玩家语言或存档。更新后需完全退出游戏再启动。
- 3.6.6：加水、清空与液体传输的适配不再随启动PatchAll提前注册，改为确认原生本地化初始化及Mechanic语言表（含原生回退语言）异步加载成功后，使用官方加载器的补丁所有者注册。禁用时取消等待并只移除自己的液体适配，重新启用不会重复安装。不改物品身份、存档结构或水中化学成分。更新后需完全退出游戏再启动。

- formal.19：修复“新人酿酒师”开局只赠送空试剂盒的问题，改为使用原版满盒构造方法，内含10支未使用的试纸。已有普通空盒不自动装满。

- formal.18：调试生成按钮在官方物品目录初始化、文本加载完成后登记，避免启动时找不到名称的警告；180个物品仍正常注册，没有延迟生成工厂。
- formal.18：自定义酒倒空时，在原版内部刷新图片之前切回原版空瓶图源；修复发酵中读档误把原版图集当作模组查找的问题。倒空后可重新酿酒；其他补丁取消倒空或抛出异常时，保留未改变的原贴图来源。
- 全部180个物品完成生成、原生恢复及重复加载通知检查；初始化专项1989项通过。确认低电量与已存水量不被重置，机器说明书不重复生成，自有加载和夜间等回调不因重复加载而增加。

此前formal.15的修正继续保留：

- 六台机器的最右侧均有原版纸条栏，并自带对应使用说明：孵化器、原浆浓缩器、酿造桶、酒瓶打印机、水瓶打印机、流年。悬停查看完整用法，支持中英文，现有机器空说明栏也会补上；不覆盖玩家自行放入的纸条。流年说明末尾显示当前质量、机器陈化天数及耗电。
- 原浆手动不耗电、自动每份8kJ；流年基础耗电24kJ，每125质量额外陈化1天，但耗电由模块附带属性另算，并非每125质量固定加电。各机器说明均按实际代码核对。临时测试记录见research/formal-machine-guides-20261029。

此前formal.14的修正继续保留：

- 泥泥只在持有名片并通过电话确认上门服务后加入队列，移除适配版误加的每周自动来访。读档跳过此前未预约的泥泥来访记录，保留当天有效电话预约与其他访客；灯牌仍只吸引四十七。最新专项验证见research/formal-nini-call-20261029/verification.json。

此前formal.13的修正继续保留：

- 酿酒商店45件商品按指定参考图固定摆放，传输线竖放。买走或限购未上架的商品保留空位，其余商品不会自动挤位，补货回到原格位。
- 盲盒优先在所在的这一层开出奖励，不再递归搜索附近收纳仓和机器的内部。店内同层不够时才尝试店内主库存；外出时空间不足则保留盒子并撤回未完成奖励。
- 三档xuxu电池接入物品自身的原生过夜回调，初级、中级、高级每晚分别回复6/8/12kJ，零电版也能回复；通过原版计算限制在容量以内。
- 流年存放区与模块区下沿对齐，酒总价值说明移到存放区下方，内部容量与物品位置不变。
- 三款滤芯和三款净水器的介绍开头明确标出每晚恢复耐久与上限。保持原设计：滤芯60/80/100，净水器20/30/100。
- 文字文件改用正式版支持的换行转义，修复长介绍被逐行读取而截断的问题，保留原有完整文案。

此前formal.12的修正继续保留：

- xuxu酿造桶恢复为自己的48×48贴图，空闲和发酵状态均采用原设计，修复旧大木桶贴图放大覆盖界面。
- 180个自定义物品注册ID均以be_开头，自有贴图键也统一前缀。补齐16种果实、16种种子、50款酒和1种特殊酵母的旧ID；配方、商店、酿造和存取引用同步更新。原版依赖保持原版ID。
- 夜间出行的四十七酿酒商店卡片加入夜间四十七，与实际商店使用相同摆放参数，不覆盖其他地点。
- 此前正式版测试档中可确认属于本模组的旧物品，在原版读取阶段转换为新ID，保留UID、内容、酒龄、组成和连接。未重新注册旧ID或xiaowo别名，不以裸ID认领其他模组物品；试玩版存档不在转换范围。

## 本次接入

- 泥泥、四十七、电话两选项、原版逐字对话及录音历史、五天电话冷却、证书奖励、种子与配方上架。
- 独立的酿酒商店与背景。全部45件商品进入时直接摆在原版商店格子里，点击实物后使用原版交易界面购买；没有分类选购面板。保留原版商店的返回控件。
- 外环区正声望每满 20 点，四十七到店多带 1 颗种子，有预算的顾客购买预算增加 10%。奖励累加至 200 声望，最多多带 10 颗种子、顾客预算增加 100%。负声望有各档限制和刺客事件。泥泥与四十七每次交易最多获得 10 点外环区声望，批量买卖共用该上限。
- 泥泥识别隐瞒的赝品香烟及瑕疵标签，识破后扣5声望并重新报价；如实标签不扣，同件物品不重复处罚。
- 自定义电池、三款滤芯、三款便携净水器、四款额外水容器、背包、隐藏仓、可换外观仓、盲盒、收藏酒、香水、节日物品、名片、徽章、手册及配方，共65项本阶段物品定义。
- 五盘自带音乐磁带，使用官方音频目录与原版磁带播放机制。
- 新人酿酒师起始方案、音乐及赞助开局奖励、一心为酒、酩酊大醉、厌恶种子。特质通过原版价格明细参与计算；两个加成分别乘算。
- 直接入瓶的果实配方、特殊酵母与反应器、酒中原水的毒性及快感保留、收藏酒陈酿。

此前已迁移的手办、6/12/18L水瓶、60L澄水桶、水瓶打印机、原浆浓缩器、酿造桶、酒瓶打印机、双向选择传输线、种子孵化器和流年继续保留。

## 修复与存取

- 电池路径随真实电量更新，原版充电器预览也读取同一路径。
- 名片恢复2×1，五仁月饼恢复3×3，盲盒和节日装饰保留正确原版分类。
- 换顾客时清除上一位的香水价格影响；四十七离场只清理未售商品。
- 原版存档保存顾客追踪信息，但不保存运行时队列。本模组把自己的待来访访客、当前四十七未售商品保存到本局ModData，再按同一天恢复，不序列化其他模组的访客。
- 透明区域点击/拖动通过每件物品自己的图片配置处理，不改动Unity或原版物品的全局点击规则。自动测试覆盖创建、恢复、换图、真实旋转翻转及原版允许的多页移动；原版拒绝的容器嵌套仍然拒绝。
- 57款配方酒均使用各自正常注册的be_物品ID，原生发酵、饮用、化学组成与陈酿机制继续保留。倒空后恢复原版wine_bottle空瓶ID和行为。

## 安装

正式发布的代码文件名为BrewingExpansion.dll，与manifest.xml、Textures、Sounds和Localization一起保留在brewing_expansion文件夹中。

把brewing_expansion整个文件夹放到正式版游戏的Mods目录，在游戏内模组页面启用并重启。当前本机目录为D:/steam/steamapps/common/Probably Stolen/Mods/brewing_expansion。

本包仅包含生产DLL、资源和说明，不包含临时测试辅助程序。F9需要游戏自己的开发者工具开关；正常游戏不依赖F9。

## 验证范围

本阶段目录与机制、真实夜间商店出入采购、电话录音、标签识别、酵母与净化、电量及透明命中已进行正式版运行测试。另用独立测试槽位实际SaveGame、退出进程、重新启动并通过主菜单LoadGame，检查物品UID、各机器状态、酒龄价值、连接、电话、声望、ModData及访客。formal.12重新覆盖全部180个物品的创建与恢复、83个旧ID转换、酿造桶实际渲染尺寸和出行卡片。formal.13专项补测商店固定格位、真实购买后开箱与返回、零电电池与六款耐久物品的实际过夜、流年渲染边界。最新结果和失败探针记录见工程research/formal-shop-layout-20261029/verification.json；前阶段记录在research/formal-id-art-20261029和research/formal-remaining-20261029。

自动Image命中测试不等同于人工鼠标拖动或所有分辨率的视觉验收。没有据此宣称与所有第三方正式版模组兼容。试玩版不兼容本包。

独立的“xuxu的MP3 1.1”本地音乐播放器是另一工程，未包含于本包；五盘磁带不替代它的本地MP3功能。


## formal.23 修复

修复三档 xuxu 电池放入原版充能器后，充能器外观显示问号的问题。实际电量及充放电规则不变。包含 formal.19 至 formal.22 的累计修复。

Fixed the native charger showing question-mark placeholders for all three xuxu battery tiers. Actual charge and charging rules are unchanged. Includes the earlier formal.19–22 fixes.


## formal.24 流年性能优化

使用原版公开库存扩展点，合并流年模块区、存放区的重复显示刷新；关闭窗口时不再为隐藏界面反复计算总价值和重画内容。模块规则、效果、耗电和陈化结算仍调用原版方法，本次没有新增全局Harmony补丁。遇到外来工厂或库存渲染补丁时，创建流年会保留原生库存路径。

该次更新以formal.23样本验证正式版存档兼容；当前维护范围以本文件开头说明为准。已用真正formal.23生成的存档验证更新读取、保存、退出并重开；保留物品UID、内部栏位顺序、说明书、摆放、酒龄、价值、电量和模块。包含formal.23充能器电池贴图及此前累计修复。

Improved Liunian rendering through the native inventory extension point, with fewer repeated redraws and no new global Harmony patches. Native module, power and aging rules remain in use. Verified loading a real formal.23 save, saving with formal.24, and reopening it in another game process. Includes the charger battery artwork fix and earlier cumulative fixes.


## formal.25 外出背包选择贴图

修复五款模组背包在外出选择界面变成问号的问题，包含玉兔怀袖、樱雪囊、电子幽灵、牢力士和猫又又小挎包。仅在原版创建选择图标时，为本模组背包改用官方贴图读取接口；不改原背包及其内容，也不替换选择和外出规则。验证了各背包选择、外出返回、内容保留、透明区域点击判定、估值，以及formal.24存档更新读取、保存和重开。包含充能器电池修复、流年性能优化和此前累计修复。

Fixed question-mark icons for all five mod bags in the native departure backpack picker. The narrow adapter uses the official mod sprite API for display proxies, while keeping native selection and travel behavior. Verified travel and return, contents, transparent-pixel hit testing, prices, and loading/re-saving/reopening a real formal.24 save. Includes the charger battery fix and Liunian performance improvements.


## formal.26 储藏容器说明

统一多功能收纳仓、xuxu迷你储存区、四十七「小」暗格的中英文扩建说明，置于物品介绍结尾：支持原版储藏区扩建套件，每个增加1页，最多5页，每页空间及已有内容保留。保留原有介绍和暗格的搜查保护说明。扩建机制和种子标签本次均未修改；生产DLL与formal.25相同。包含此前累计修复。

Standardized the expansion-kit notes at the end of the descriptions of the three supported storage items in both languages. Each vanilla kit adds one tab, up to five in total, retaining page dimensions and contents. Storage mechanics and seed categories are unchanged; the production DLL is identical to formal.25.


## formal.27 种子分类与存档兼容

本模组16种种子仅保留“种子”分类，去掉“食物”和“材料”。使用原版物品读取回调修正当前正式版旧档中的分类与对应标签，保留身份、数量、价值、位置和其他标签；不修改原版种子的分类。孵化器仍按明确配方接收种子，普通材料不能进入种子输入区；原版咖啡豆等7种种植原料仍可孵化。原版荧光莓每颗种子产出2个，初始品质固定2分，此项规则未改动。

All 16 custom seeds now use only the Seed category. Native item restoration normalizes the previous formal save categories without changing identity, contents, placement or unrelated tags. Vanilla categories remain unchanged. The incubator accepts only supported recipes, including the seven vanilla growing ingredients; ordinary materials are rejected by its seed input. Vanilla bloomberry seeds yield two berries at a fixed initial score of 2; that existing rule is unchanged. Includes prior cumulative fixes.


## formal.28 酒液化学成分与提示修复

补回配方酒及收藏酒的快感、毒性数值与程度提示，使用官方OnCreateTooltipLate接口，不新增全局提示补丁。手动与酿造桶保留每瓶实际750ml用水中的非水化学成分，读档恢复已有活性成分的原生反馈识别。原版酒不变，空瓶重用会清理上一批成分。旧正式版已保存的成分完整保留；之前已经被丢弃且未存下的成分无法凭空补回。含此前累计修复。

Restored euphoria/toxicity values and severity descriptions for custom recipe and collector wines through the official tooltip hook. Manual and barrel fermentation retain dissolved non-water chemicals from each actual 750ml portion. Loading restores native feedback recognition for existing active wine doses. Vanilla wines remain unchanged; resetting a custom bottle clears the previous retained dose. Existing saved composition is preserved; previously discarded unsaved chemicals cannot be reconstructed. Includes cumulative fixes.


## formal.29 本地化加载时序修复

移除WineHelper.InitWineBottle的Harmony补丁，避免模组加载处理该原版方法时提前触发Liquid静态初始化和同步本地化读取。改为自有酒创建/酿造时接入原生读档回调，保留贴图、化学成分及既有存档身份。没有强制修改语言、强制提前初始化原版液体、添加全局本地化补丁或额外权限。更新后请完全退出游戏再启动，已失败的静态初始化不能靠返回主菜单恢复。

Removed the Harmony patch on WineHelper.InitWineBottle, avoiding the reported premature Liquid/localization initialization path during patch installation. Native load restoration is now attached only when custom wines are created or fermented. Existing artwork, composition and saved identities are preserved. No forced language change, early native initialization, global localization patch or new permissions. Fully restart the game after updating; a failed static initializer persists for the lifetime of the process. Includes formal.28 chemistry/tooltip fixes and prior updates.


## formal.30 酿酒证书修复

- formal.30：泥泥的证书进度按原版显示的基础价值累计，补计此前遗漏的延后价值部分。保留原来的逐级门槛：初级1500，随后再累计3000领取中级，再累计8000领取高级；不按议价后的成交价计算。
- 已持有的证书可在读档、泥泥到店及达标时核对发放记录，避免重复扣除初级门槛。空间不足时保留进度，腾出位置后在下次交易、泥泥到店或读档时补发；发放失败不阻断原版交易。保留原证书ID、UID、历史记录和库存内容，已发过但卖掉的证书不重复赠送。
- 本包保留formal.29的加载时序修复及formal.28的酒成分继承/快感毒性提示，没有加入原版日历语言切换试验。历史版本未记录的基础价值差额无法凭空追溯补回。

Nini now counts the native displayed base value toward brewing certificates. Existing owned certificates reconcile missing receipts. Pending awards retain credit and retry on the next trade, arrival, or load after space becomes available. Reward failures do not interrupt native sale settlement. Sequential additional thresholds remain 1500, 3000, and 8000. Preserves existing formal saves and certificate identities. Previously unrecorded historical value cannot be reconstructed. No new global trade or localization patch.

## formal.31 游戏更新兼容修复

游戏build26317870新增了StoreClient.IsClientBuyingThisItem(GameItem,bool)重载。原先只按方法名定位的泥泥收购类别补丁因匹配不唯一而中断整个模组初始化，导致已存物品无法正确恢复并出现问号。现明确指定新版核心重载的参数；一参数入口会调用该核心重载，因此两条入口均保持泥泥只收购烟酒的限制。原版拒收判定保留，其他顾客不受此类别限制影响。

不更改物品ID、贴图文件、存档结构、库存顺序或证书进度。新版游戏实机检查：180种物品的新建和原生序列化恢复、266张物品贴图及收购判定共1016项通过；实际formal.30测试档读取17项通过。未在玩家原存档上试错。使用更新后的模组后需完全退出游戏并重启。工坊发布状态以单独的发布记录为准。

Pins Nini's category adapter to the new explicit StoreClient.IsClientBuyingThisItem(GameItem,bool) overload, preventing an ambiguous Harmony target from aborting mod initialization after game build26317870. Native refusals and other customers remain intact. Item IDs, authored sprites, save structure, inventory order and certificate progress are unchanged. Native checks cover all180 item factories and graph restores, all266 registered sprites, both purchase entry points and an actual formal.30 save fixture. Fully restart the game after replacing the package.

## 3.6.5 配方阅读保护与交易平衡

- 尚未拥有对应酿酒配方时，普通与详细悬停提示均不显示配方正文；购买后立即可读，现有已购买配方及酿酒手册中的配方继续保留。
- 厌恶种子改为四十七每次少上架2颗种子，保留特质身份及原来的特质点数。
- 泥泥与四十七每次交易获得的外环区声望最多10点，批量买卖合并计算；保留原版倍率，小额收益不提高，负声望惩罚不受该正收益上限影响。现有声望、证书和库存不重置。
- 恢复常规版本号递增，本次为3.6.5；继续包含此前formal.31的游戏更新兼容修复。

Unowned brewing recipes hide their contents in both normal and advanced hover tooltips; purchased recipes remain readable, including those stored in the brewing manual. Seed Aversion now removes two seeds from Forty-Seven's stock per visit. Outer Ring reputation gains from each Nini or Forty-Seven transaction are capped at10, shared across the entire bulk purchase or sale after native multipliers; smaller gains and negative penalties retain their normal rules. Existing reputation, certificates and saved items remain intact. Regular version numbering resumes at3.6.5, including the prior game-update compatibility repair.
