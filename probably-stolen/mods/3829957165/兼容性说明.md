# 与其他 Mod 的兼容说明

本文只说明两项兼容接口：

1. `PEN_MOD_QOL_BOXSELECT_EXCLUDE`：让物品退出本 Mod 的框选批量；
2. `PowerHelper.CanDrawPowerSource` / `DrawPowerSource`：自动换电池的触发兼容。

---

## 一、`PEN_MOD_QOL_BOXSELECT_EXCLUDE`

### 作用

物品带有：

```csharp
PEN_MOD_QOL_BOXSELECT_EXCLUDE
```

这个 tag 后，本 Mod 不会把它识别为框选批量工具，也不会接管它的框选行为。

只影响本 Mod 的批量接管，不改变物品原本的单体使用、`MayTarget`、`CanTarget`、`Target` 或液体容器能力。

### 推荐写法

其它 Mod 不要引用本 Mod 的程序集来调用 `BatchToolCompat`。直接使用字符串 tag：

```csharp
item.EnableTag("PEN_MOD_QOL_BOXSELECT_EXCLUDE", false);
```

这样不产生外部程序集依赖，也不需要反射查找本 Mod 类型。

### 添加时机

在物品创建后、以及读档重建物品后，确保 tag 存在。

不要只在 UI 临时状态上标记；本 Mod 只读取物品 tag。

### 判定位置

本 Mod 在判断某个工具是否参与框选批量时，第一步检查物品是否带有该 tag：

```csharp
item.IsTag("PEN_MOD_QOL_BOXSELECT_EXCLUDE")
```

因此该 tag 的优先级高于后续的工具类型判断。

### 适用对象

适合：

- 不希望被 QoL 框选批量接管的工具；
- 有特殊单体目标逻辑、批量执行会破坏状态的自定义工具；
- 需要自己实现框选/多选流程的物品。

不要把这个 tag 加到普通电池、液体或标签物品上；它只影响“工具是否被本 Mod 接管”，不会让物品免疫自动换电池。

### 结果

带 tag 后：

```text
框选该工具
→ 本 Mod 不记录为批量工具
→ 不执行 QoL 批量分支
→ 保持原版行为
```

---

## 二、`CanDrawPowerSource` / `DrawPowerSource` 触发兼容

### 1. 本 Mod 监听的两个入口

```csharp
PowerHelper.CanDrawPowerSource(GameItem gameItem, int amount)
PowerHelper.DrawPowerSource(GameItem gameItem, int amount)
```

- `CanDrawPowerSource`：判断阶段；
- `DrawPowerSource`：实际扣电阶段。

只接 `DrawPowerSource` 不够：部分机器、涡轮和预检查路径会在 `CanDrawPowerSource` 返回 false 后直接 `return`，根本不会执行 Draw。

### 2. `CanDrawPowerSource` 的行为

当同时满足：

- `gameItem` 是 `power_source_item`；
- 位于本 Mod 可识别的机器/工具电池槽；
- 电池未固定；
- 原版返回 `false`；
- 本 Mod 上次搜索没有耗尽；

本 Mod 会尝试寻找替代电池。

找到时：

```text
用 GraphUtils.SwapItemsInPlace 交换
→ 旧电池回到新电池原位置
→ 本 Mod 修改 __result 为替换电池的 CanDraw 结果
```

其它 Mod 随后看到的 `CanDrawPowerSource` 返回值，可能是替换后的结果。

找不到时：

```text
标记该电池+槽位搜索耗尽
→ 保持 false
→ 后续 CanDraw 不再重复全库存搜索
```

注意：这里的“缓存”只缓存“搜索已耗尽”，不会用旧的 true 覆盖真实的 false。

### 3. `DrawPowerSource` 的行为

#### 电池足够

原版逻辑正常执行；本 Mod 不改变返回值。

#### 电池不足

本 Mod 尝试替换，然后：

```csharp
__result = PowerHelper.DrawPowerSource(replacement, amount);
```

并返回 `false`，跳过原电池的原版扣电。

因此其它 Harmony 补丁可能观察到：

```text
外层 DrawPowerSource(old) → 被跳过
内层 DrawPowerSource(new) → 实际执行
```

一次机器周期里可能出现**两次 DrawPowerSource 调用**。其它 Mod 的补丁必须允许这种重入，不能假设一次周期只有一次 Draw。

#### CanDraw 已替换后的 Draw

同一帧内，如果 `CanDrawPowerSource` 已经换过电池，后续对旧电池的 `DrawPowerSource` 可能被本 Mod 重定向到新电池。

其它 Mod 如果缓存了旧 `GameItem`，应重新读取 `parentInventory` / `childItem`，不要继续使用已经离开电池槽的旧引用。

### 4. 电池槽识别条件

本 Mod 识别：

```csharp
battery.parentInventory as GameSlotInventory
```

并要求：

```csharp
slot.childItem == battery
```

机器侧优先匹配：

```csharp
MachineHelper.GetBatterySlot(host) == slot
```

自定义机器可回退到：

```csharp
host.contentWindow != null
```

不满足这些条件时，`CanDrawPowerSource` / `DrawPowerSource` 即使返回 false，本 Mod 也不会换电池。

空电池槽不会触发替换；本 Mod 不负责给空槽装电。

### 5. 固定电池保护

本 Mod 会跳过：

```csharp
ItemPinHelper.IsPinned(item)
```

包括：

- 机器/工具中已固定的旧电池：不换出；
- 库存中已固定的候选电池：不拿去替换。

其它 Mod 如果要保留电池，使用原版固定机制即可。

### 6. 其它 Mod 应使用的调用方式

推荐：

```csharp
PowerHelper.CanDrawPowerSource(battery, amount)
PowerHelper.DrawPowerSource(battery, amount)
```

或原版包装：

```csharp
MachineHelper.CanPower(machine, batterySlot)
MachineHelper.TryDrawCyclePower(machine, batterySlot)
```

不要绕过这些接口直接写：

```csharp
power_source_item_energy
```

直接写电量 tag 会：

- 不触发电量不足替换；
- 不刷新“搜索耗尽”状态；
- 让本 Mod 和其它依赖原版电源事件的逻辑看到过期状态。

