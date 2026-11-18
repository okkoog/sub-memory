Appraisal Assistant（鉴定助手）作者 kale
版本 1.1.0：跟随游戏版本更新，优化兼容性。问题可在创意工坊留言或QQ交流群534744496反馈。
真伪详细解析仅用于已核验的原版物品；Mod新增物品保留真伪结果和异常项目标记，不套用原版的具体原因与规则说明。

启用后重启游戏，再载入存档。改造体或通缉顾客到柜台时分别弹出警告；用放大镜看对应物品时弹出独立的真伪或估价窗口。
荧光莓悬停说明也会追加品质与单颗酿酒贡献。不能读取的数据明确显示未知。
估价按游戏注册表顺序匹配，包括Mod新增和替换的品类。原版计算使用独立副本；第三方规则采用有步数限制的只读解释执行，不直接调用未知委托。可验证的标签、物品编号及数值规则自动显示总价，无需逐Mod硬写公式；未知调用、写入、补丁改变或复杂规则暂时显示未知。未知品类不显示原版明细，已有明细与总价不符也会隐藏。FufuSamples 0.6.0真实DLL规则已做离线隔离检查，实际游戏行为待用户测试。
右键点击游戏原生放大镜打开设置，或在主菜单模组设置中调整。两处都有“右键放大镜打开设置”开关。游戏内关闭时先提醒，按“知道了”后生效；叉号或Esc取消。关闭后可在主菜单的“改装件”中找到鉴定助手，点击设置齿轮重新开启；提示中的菜单名称使用对应语言的游戏译文。支持跟随游戏、简体中文、英文、日文、德文、法文、西班牙文、俄文、波兰文；未识别语言默认英文。
三类解析默认开启；官方物品估价计算过程可展开/收起。填入估价默认启用，仅玩家点击填写并播放按钮音效；已有开关设置保留。“启用填入并提交估价”默认关闭，可在主菜单模组设置或助手价值设置开启。开启时仅显示“填入并提交”，不受普通填价开关影响；关闭后再按原填价开关决定是否显示“填入估价”。不改写两个开关的选择。提交仅在玩家点击时填写真实价值并调用游戏原生提交，游戏正常处理报酬、凭条及顾客对话；不绑定回车或自动推进。普通“填入估价”仍只填写。
改造体只解释实际可见动作、头像与已完整出现的台词；身份以游戏判定为准。
声望和结局风险按当前正式难度与特质预览，特殊第三方规则显示未知。
窗口标题栏放大按钮统一切换小/正常/大，位置与大小保存。保留标题栏叉号，无底部关闭按钮。
关闭窗口后，切换物品/顾客或重新读档会再显示；Esc只在本助手设置位于顶层且无更高原生输入时关设置。
权限：不申请额外能力。只在明确的游戏类型上核验原生字段，不查询系统/引擎内部成员；使用隔离输入，不直接执行未知第三方委托。未知自定义物品的虚拟名称不会被调用或猜测。
请停用旧ProbablyStolenInspector再启用本候选，避免两套窗口重复。顾客生成器与字体修复保持独立。
遇到异常请退出游戏，向开发者描述出现在哪一步。

实际扫描和游戏测试状态请查看本项目 docs/STATUS.md；编译不等于运行通过。
Shows customer augmented/wanted status, native authenticity/value and berry quality. Inspect an item with the magnifier.
Right-click the native magnifier for settings. Includes authenticity/value/observed-client explanations,
optional click-to-fill (on by default, preserves saved settings, plays native click feedback), report/outcome previews, and three window sizes.
Optional Fill and submit estimate is off by default. Enabling it replaces the Fill estimate button regardless of the fill-only setting. Disabling it restores the saved fill-only choice. It fills the true value and submits through the native appraisal flow only on a player click. Fill estimate still only fills; no Enter binding or automatic progression.
Unknown or unverified data is displayed as unknown. Disable the old Inspector to avoid duplicate windows.