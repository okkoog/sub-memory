# Localization / 本地化翻译说明

## 中文

本模组使用正式版游戏的标准本地化目录与键值表，无须修改或反编译 DLL。

1. 复制 `Localization/en.csv`，将副本命名为目标语言代码，例如 `Localization/ru.csv`。
2. 只翻译每行第一个逗号之后的文字。左侧 `fre.*` 是永久键，请保持原样。
3. 使用 UTF-8 保存，将文件放入模组的 `Localization` 文件夹，完整重启游戏。
4. 在游戏中选择对应语言。文件名必须匹配游戏的语言代码；本模组不会自行向原版语言选择器添加新语言。为原版制作的翻译与本模组使用相同代码即可。

发布包附带 `en.csv`、`zh.csv`、`zh-CN.csv`、`zh-Hans.csv`；后三份是简体中文的兼容代码。`LocalizationKeys.tsv` 列出全部键、中英文原文及来源，便于在表格软件中查找上下文。翻译时以 CSV 为准，TSV 仅供参考。

```csv
# 注释行以井号开头；不要添加 Key,Value 表头
fre.faction_journal_5ef8d64b,"势力档案"
fre.hotkey_0_89eab54b,"热键 {0}"
```

格式约定：

- 一条翻译占一个物理行。换行写成字面字符 `\n`，不要在单元格内插入真正的换行。
- 值统一用双引号包住；文字中的双引号写成两个双引号。逗号可保留在引号内。
- 保留 `{0}`、`{1:0.##}` 等参数，也保留 `{类别}`、`{短缺}` 等对白插槽。可以调整参数位置，不可删除、改名或增加参数编号。
- 保留 `<color=...>` 等富文本标记。需要表达花括号本身时，遵循原文的 `{{`、`}}` 写法。
- 不翻译势力 ID、物品 ID、电话号码或存档键；CSV 只包含展示文本。
- 不必一次翻完。缺少条目或空白条目会回退；错误的参数插槽也会回退并记录一次警告，不会令委托、交易或面板格式化失败。

模组按“当前语言代码 → 父语言代码 → 英文 → 内置文本”查找。例如 `ru-RU` 可使用 `ru.csv`。中文缺译会使用内置中文。游戏自己的 `ModHelper.GetLocalized` 则遵循游戏的原生回退规则。

对其他模组或翻译工具，标准读取接口是：

```csharp
ModHelper.GetLocalized("faction_reputation_expansion",
    "fre.hotkey_0_89eab54b", new object[] { "F6" });
```

键不会因日常改文案、调整显示顺序或增加语言而改变。新版本可增加新键；旧翻译文件中的已有键继续有效。更新模组前备份自己增加的语言文件，合并新条目后恢复。Steam 更新可能覆盖发行版自带的中英文文件。翻译资源不写入玩家存档。

维护源码时修改 `assets/localization/catalog.json` 中现有键的 `zh`/`en` 内容，不要按新文案重新生成键。`source` 是旧版数据定义使用的固定别名，与键一样保持不变；新增代码优先直接使用键。`tools/build_localization.py` 生成发行 CSV 和索引，不会更改玩家安装目录中的文件。

## English

This mod uses the full game's native `Localization/<locale-code>.csv` convention. Translators do not need to modify the DLL.

1. Copy `Localization/en.csv` to your target locale, for example `Localization/ru.csv`.
2. Translate only the value after the first comma. Keep every `fre.*` key unchanged.
3. Save as UTF-8, place the file in the mod's `Localization` directory and fully restart the game.
4. Select the matching language in the game. The filename must match its locale code. This mod does not add new entries to the base game's language selector; use the same code as your base-game translation.

The release includes English and Simplified Chinese tables (`en`, `zh`, `zh-CN`, `zh-Hans`). `LocalizationKeys.tsv` provides a searchable key index with both source languages and context. Edit the CSV files; the TSV is a reference only.

- Keep one entry per physical line. Encode line breaks as literal `\n`.
- Quote values with double quotes and double any quotes inside the value. Commas inside a quoted value are supported. Do not add a `Key,Value` header. Lines starting with `#` are comments.
- Preserve indexed placeholders such as `{0}` and `{1:0.##}`, dialogue slots such as `{类别}` and `{短缺}`, rich-text tags and escaped braces. Placeholders may move within a sentence, but their identities must not change.
- Do not translate gameplay IDs, save keys or telephone numbers.
- Partial translations are supported. Missing or empty entries fall back. Invalid placeholders also fall back and produce a single warning per key and locale.

The mod resolves the selected locale, its parent locale, English and finally embedded text. Chinese falls back to embedded Chinese. The native `ModHelper.GetLocalized` API uses the game's own fallback rules. Other tools can read the same public keys through that API, using mod ID `faction_reputation_expansion`.

Keys remain stable when copy, display order or languages change. New releases may add keys, while existing translations retain their keys. Back up custom language files before updating and merge any newly added entries. Steam updates may overwrite bundled English/Chinese files. Translation resources are never written into player saves.

Source maintainers should edit the `zh`/`en` fields of existing entries in `assets/localization/catalog.json` without replacing their keys. The `source` field is an immutable compatibility alias for older data definitions; keep it unchanged as well. New code should reference keys directly. `tools/build_localization.py` exports release CSVs and the index; it never writes to a player's installed translation files.

Native convention: [Probably Stolen Modding Guide](https://github.com/dragonpan2/probably-stolen-modding/blob/main/ModdingGuide.zh-CN.md).
