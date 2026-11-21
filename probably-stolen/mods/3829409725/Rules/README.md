# Pipette reaction rules

本模组的 `Rules/hydra.json` 使用 **operation / 参数**，不包含公式字符串。
公式实现集中在 `FormulaRecipes.cs`；规则文件只负责模式元数据和阶段选择。

## v4：operation 规则

```json
{
  "modes": [
    {
      "id": "acid",
      "implementation": "custom",
      "detect": "share",
      "labelKey": "hydra_mode_acid",
      "mainLiquid": "acid",
      "priority": 100,
      "minShare": 0.5,
      "constants": {
        "wastePerAcid": 40,
        "microbePerAcid": 100
      },
      "stages": [
        { "id": "mineral", "operation": "acid/mineral" },
        { "id": "microbe", "operation": "acid/microbe" },
        { "id": "stabilizer", "operation": "acid/stabilizer" },
        { "id": "protein", "operation": "acid/protein" },
        { "id": "neutralize", "operation": "acid/neutralize" }
      ]
    }
  ]
}
```

### 字段

- `id`：模式 id；
- `mainLiquid`：主成分；
- `detect`：`share` / `substrate` / `fanta`；
- `minShare`：主成分占比；
- `priority`：规则优先级；
- `constants`：数值常量；
- `stages[].id`：阶段 id；
- `stages[].operation`：代码中的 operation 配方键，例如 `acid/mineral`；
- `temperatureMin/Max`：阶段显式温度条件，用于原因提示。

### 内置 operation

当前内置 21 个阶段：

```text
acid/mineral
acid/microbe
acid/stabilizer
acid/protein
acid/neutralize

base/mineral
base/microbe
base/stabilizer
base/protein
base/neutralize

reducing/nomore
reducing/methylol
lipobond/fat
lipobond/nomore
lipobond/topup
chlorine/chlorine
dream/extract
nightmare/extract
coffee/extract
fanta/fanta
alcohol/topup
```

公式、转移量、消耗/产出和温度判断都在 `FormulaRecipes.cs` 中实现。

## 第三方规则

- 当前规则由代码统一编译；
- 规则文件不带版本字段；
- 第三方不需要引用 `Pen_mod_pipette.dll`；
- 使用 `ModHelper.ReadModFileText` 和 `ModHelper.GetLocalized` 读取规则与文案。

## 第三方 patch

外部数据 Mod 可以只提供 `manifest.xml` + `PipetteReactions.json`，通过 `patches` 修改已有规则：

```json
{
  "patches": [
    {
      "mode": "reducing",
      "stage": "nomore",
      "set": { "temperatureMin": 44, "temperatureMax": 55 }
    },
    {
      "mode": "fanta",
      "stage": "fanta",
      "remove": ["temperatureMin", "temperatureMax"]
    }
  ]
}
```

支持：

- `set`：覆盖字段；
- `remove`：删除字段；
- `replace`：整体替换 stage；
- `addStage` + `before` / `after`：新增 stage。

补丁在所有规则加载后应用；失败不会修改原规则；多个 Mod 按加载顺序覆盖。
