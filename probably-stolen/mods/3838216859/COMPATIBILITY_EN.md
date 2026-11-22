# Integrated Chips: Compatibility Notes

This document covers four kinds of integration:

1. How to author a Desequencer chip that this mod recognizes;
2. How to author a 3D Printer chip that this mod recognizes;
3. Two-way drag and absorption rules;
4. Cases that will NOT be recognized.

> **No reflection and no extra permissions are required.**
> You do not need `reflection-external`, `files`, `native` or any other capability, and you do not need to reference this mod's assembly.
> The only patch in this mod is a narrow Harmony prefix on `GameItem.MayTarget / CanTarget / Target`. It exists because the vanilla desequencer chip installs its own chip-swap hook that rejects reverse drags; the prefix only takes over when the target is one of the two integrated chips. It does not change how third-party chips are written and never modifies your items.

The integrated chip reads **the tags on the dragged item itself** at the moment of the drag. There is no chip whitelist, so chips added by third-party mods (installed before or after this mod, in any load order) work without adapter code.

## 1. Authoring a Desequencer chip

### Required

| tag | type | notes |
|---|---|---|
| `DESEQUENCER_TARGET_ID_TAG` | string | Item id of the desequencer target. It must be a **registered** item of type `ACCESS_CARD` and must not be `blank_keycard` (the vanilla desequencer output slot only accepts keycards). |

### Optional

| tag | type | default |
|---|---|---|
| `PROGRESS_ITEM_TAG` + `PROGRESS_ITEM_TARGET_TAG` | tag / int | Vanilla work amount. Missing means 0 (the machine completes instantly). |

Use the vanilla API:

```csharp
item.ModifyTag("DESEQUENCER_TARGET_ID_TAG", s => s.Enable().SetString("my_keycard"));

// Work amount + PROGRESS_ITEM_TAG (second argument is the work amount)
MachineProgressHelper.InitProgressSourceItem(item, 100);

// Same drag sound as the vanilla chip
AudioHelper.InitSmallModule(item);
```

Notes:

- The item does not need to be declared as a `MODULE` and needs no "integrated chip" tag;
- The integrated chip writes its own derived `DESEQUENCER_TARGET_ID_TAG` / `PROGRESS_ITEM_*` tags; do not pre-write target values onto an integrated chip.

## 2. Authoring a 3D Printer chip

### Required

| tag | type | notes |
|---|---|---|
| `PRINTER_CHIP_TAG` | tag | Admission tag for the vanilla printer chip slot. |
| `PROGRESS_ITEM_TAG` | tag | Job marker. |
| `PROGRESS_ITEM_TARGET_ITEM_ID_TAG` | string | Output item id. Must be **registered**. |

### Tags that are snapshotted and restored

The integrated chip saves every enabled tag on the source chip whose name starts with `PRINTER_`, `PROGRESS_` or `REQUIRED_` (except `PROGRESS_ITEM_SHOW_DEFAULT_TOOLTIP`) and restores them when the job is selected. Common ones:

```text
PRINTER_CHIP_TAG
PRINTER_MELEE_CHIP_TAG / PRINTER_FRAME_CHIP_TAG / PRINTER_GUN_PART_CHIP_TAG
PROGRESS_ITEM_TAG
PROGRESS_ITEM_TARGET_TAG                    // work amount
PROGRESS_ITEM_TARGET_ITEM_ID_TAG            // output item id
PROGRESS_ITEM_TARGET_ITEM_COUNT_TAG         // output count
PROGRESS_ITEM_USE_ADDITIONAL_ITEMS           // materials required
REQUIRED_ITEM_TYPE_COUNT_TAG
REQUIRED_ITEM_ID_<i> / REQUIRED_ITEM_COUNT_<i>
PROGRESS_ITEM_REQUIRED_MACHINE_TAG           // required machine module, e.g. "CNC Module"
PROGRESS_ITEM_REQUIRED_MACHINE_TAG_COUNT
PROGRESS_ITEM_REQUIRED_MACHINE_TAG_<i>
```

Use the vanilla API (equivalent to the vanilla printer chip):

```csharp
item.EnableTag("PRINTER_CHIP_TAG");

// Output item + work amount + output count (last argument is the count)
MachineProgressHelper.InitProgressSourceItem(item, 24, "my_printed_item", true, 2);

// Materials (single / dual)
MachineProgressHelper.SetSingleAdditionalRequiredItem(item, "printer_plastic", 2);
MachineProgressHelper.SetDualAdditionalRequiredItem(
    item,
    (itemID: "scrap_metal", count: 3),
    (itemID: "common_electronic", count: 1));

// Required machine module (e.g. CNC)
MachineProgressHelper.AddRequiredMachineTag(item, "CNC Module");

AudioHelper.InitSmallModule(item);
```

Notes:

- Special types (melee / frame / gun part) are handled by vanilla `FinishPrinting`; as long as the matching `PRINTER_*_CHIP_TAG` is kept in the snapshot, the batch and quality logic works;
- The output count lives in `PROGRESS_ITEM_TARGET_ITEM_COUNT_TAG` and is used after switching jobs;
- The three prefixes above use the protocol snapshot. **All other tags** (for example Gunsmith Plus `GP_CHIP_*`, custom upgrade/quality tags, dynamic chips' extra tags) are stored through the instance-tag channel `PEN_PC_TAGS_<i>` / `PEN_IC_TAGS_<i>`: apart from `PEN_*`, ownership/lock/pin/container tags and `PRINTER_/PROGRESS_/REQUIRED_`, every tag keeps its type, value, enabled state and display name, and is restored on activation/unload.

## 3. Two-way drag and absorption rules

- **Both directions absorb**: drag a chip onto the integrated chip, or drag the integrated chip onto a chip;
- Reverse absorption works even inside the same inventory (the vanilla chip-swap hook is bypassed by this mod's narrow patch);
- **Only player-owned chips are absorbed**: the item needs `IS_OWNED_TAG` and its ancestor containers must also belong to the player; unowned chips on counters/shelves are rejected;
- **Entry identity is the source chip id.** Dragging the same id again (even with a different upgrade level/quality/other tags) shows **"This chip's data is already included"** and does **not** consume, replace or add an entry; the first absorbed instance data is kept;
- Two integrated chips never absorb each other;
- A desequencer chip's target must be a registered `ACCESS_CARD`; a printer chip's output item must be registered;
- If the source mod is uninstalled, the corresponding entries are **purged on the next save load** (including target items/materials that no longer exist), so the save never keeps dangling item ids; when no entry is usable the chip becomes inert and never feeds an invalid job to a machine.

## 4. Cases that are NOT recognized

The following are ignored or rejected:

1. Describing the target with your own tag instead of `DESEQUENCER_TARGET_ID_TAG` (desequencer) or `PRINTER_CHIP_TAG` + `PROGRESS_ITEM_TARGET_ITEM_ID_TAG` (printer);
2. A desequencer target that is not an `ACCESS_CARD`, is `blank_keycard`, or is not registered;
3. A printer chip with no output item id, or whose output item is not registered;
4. Non-protocol tags are preserved too; only `PEN_*`, ownership/lock/pin/container tags are intentionally excluded (they describe the item's context and must not be copied);
5. Items of type `MACHINE`, items with `CONTAINER_TAG`, or items with `DESEQUENCER_STATE_TAG` (a desequencer that has run a night carries target tags itself);
6. Chips not owned by the player (counter/shelf/unowned containers);
7. Items that already carry `PEN_IC_COUNT` or `PEN_PC_COUNT` (i.e. the other integrated chip, including copies of this mod's items);
8. Expecting "forced compatibility" through reflection, private-method hooks or extra permissions — this mod does not read those interfaces.

The compatibility surface of this mod is exactly:

```text
DESEQUENCER_TARGET_ID_TAG
PRINTER_CHIP_TAG
PROGRESS_* / REQUIRED_* / PRINTER_* vanilla protocol family
The vanilla MachineProgressHelper API
```

None of it needs reflection or extra permissions.
