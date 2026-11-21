# Compatibility Notes for Other Mods

This document describes the two compatibility interfaces exposed by
`Pen_mod_QoL_Tweaks`:

1. `PEN_MOD_QOL_BOXSELECT_EXCLUDE`: opt an item out of this mod's box-selection batch flow.
2. `PowerHelper.CanDrawPowerSource` / `PowerHelper.DrawPowerSource`: compatibility points used by automatic battery replacement.

---

## 1. `PEN_MOD_QOL_BOXSELECT_EXCLUDE`

### Purpose

If an item has this tag:

```csharp
PEN_MOD_QOL_BOXSELECT_EXCLUDE
```

this mod will not treat it as a box-selection batch tool and will not take over
its box-selection behavior.

It only affects this mod's batch handling. It does not change an item's normal
single-use behavior, `MayTarget`, `CanTarget`, `Target`, or liquid-container
capabilities.

### Recommended usage

Other mods should not reference this mod's assembly just to call
`BatchToolCompat`. Use the string tag directly:

```csharp
item.EnableTag("PEN_MOD_QOL_BOXSELECT_EXCLUDE", false);
```

This creates no external assembly dependency and requires no reflection lookup
of this mod's types.

### When to add it

Add the tag when the item is created and again when an item is rebuilt after
loading a save.

Do not mark only transient UI state; this mod reads the item tag.

### Where it is checked

When deciding whether a tool participates in box-selection batching, this mod
first checks:

```csharp
item.IsTag("PEN_MOD_QOL_BOXSELECT_EXCLUDE")
```

This tag therefore takes priority over the later tool-type checks.

### Suitable cases

Use it for:

- Tools that must not be taken over by this mod's box-selection batch flow;
- Custom tools whose per-item target logic would break if run in a batch;
- Items that implement their own box-selection or multi-select flow.

Do not add this tag to ordinary batteries, liquids, or labels. It only affects
"whether this mod takes over the tool as a batch tool"; it does not prevent
automatic battery replacement.

### Result

With the tag:

```text
Box-select the tool
-> this mod does not record it as a batch tool
-> no QoL batch branch runs
-> vanilla behavior remains
```

---

## 2. `CanDrawPowerSource` / `DrawPowerSource`

### 2.1 The two hooks this mod observes

```csharp
PowerHelper.CanDrawPowerSource(GameItem gameItem, int amount)
PowerHelper.DrawPowerSource(GameItem gameItem, int amount)
```

- `CanDrawPowerSource`: the check phase;
- `DrawPowerSource`: the actual power-consumption phase.

Listening only to `DrawPowerSource` is not enough: some machines, turbines, and
pre-check paths return immediately when `CanDrawPowerSource` is false and never
reach Draw.

### 2.2 Behavior of `CanDrawPowerSource`

When all of the following are true:

- `gameItem` is a `power_source_item`;
- it is in a machine/tool battery slot recognized by this mod;
- the battery is not pinned;
- vanilla returns `false`;
- this mod's previous search did not exhaust candidates;

this mod attempts to find a replacement battery.

When one is found:

```text
swap with GraphUtils.SwapItemsInPlace
-> the old battery returns to the new battery's previous slot
-> this mod changes __result to the replacement's CanDraw result
```

Other mods may therefore observe a different `CanDrawPowerSource` result after
this mod has swapped the battery.

When none is found:

```text
mark this battery + slot as exhausted
-> leave the result false
-> do not repeat a full-inventory search on later CanDraw calls
```

This cache only records "search exhausted". It never overwrites a real false
with an old true.

### 2.3 Behavior of `DrawPowerSource`

#### Battery has enough power

Vanilla logic runs normally; this mod does not change the result.

#### Battery is low

This mod tries a replacement and then:

```csharp
__result = PowerHelper.DrawPowerSource(replacement, amount);
```

It returns `false` to skip the original battery's vanilla draw.

Other Harmony patches may therefore observe:

```text
outer DrawPowerSource(old) -> skipped
inner DrawPowerSource(new) -> actually runs
```

There may be **two DrawPowerSource calls in one machine cycle**. Patches in
other mods must allow this reentrancy and must not assume one Draw per cycle.

#### Draw after a CanDraw replacement

If `CanDrawPowerSource` already swapped the battery earlier in the same frame,
a later `DrawPowerSource` for the old battery may be redirected to the new one.

If another mod caches the old `GameItem` reference, it should re-read
`parentInventory` / `childItem` instead of continuing to use a battery that has
left the slot.

### 2.4 Battery-slot recognition

This mod recognizes:

```csharp
battery.parentInventory as GameSlotInventory
```

and requires:

```csharp
slot.childItem == battery
```

For the machine side it first matches:

```csharp
MachineHelper.GetBatterySlot(host) == slot
```

Custom machines may fall back to:

```csharp
host.contentWindow != null
```

If these conditions are not met, this mod will not replace the battery even
when `CanDrawPowerSource` / `DrawPowerSource` returns false.

An empty battery slot does not trigger replacement. This mod does not install a
battery into an empty slot.

### 2.5 Pinned-battery protection

This mod skips:

```csharp
ItemPinHelper.IsPinned(item)
```

This includes:

- A pinned battery already in a machine/tool: it is not removed;
- A pinned candidate in storage: it is not taken as a replacement.

Other mods that want to protect a battery can use the vanilla pin mechanism.

### 2.6 Preferred calls for other mods

Prefer:

```csharp
PowerHelper.CanDrawPowerSource(battery, amount)
PowerHelper.DrawPowerSource(battery, amount)
```

or the vanilla wrappers:

```csharp
MachineHelper.CanPower(machine, batterySlot)
MachineHelper.TryDrawCyclePower(machine, batterySlot)
```

Do not bypass these interfaces by writing directly to:

```csharp
power_source_item_energy
```

Writing the energy tag directly:

- Does not trigger low-power battery replacement;
- Does not refresh the "search exhausted" state;
- Leaves this mod and other logic that depends on vanilla power events with stale state.
