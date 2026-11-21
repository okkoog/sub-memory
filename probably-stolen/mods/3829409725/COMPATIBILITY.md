# Pipette Mod Compatibility Guide

## 1. Overview

| Item | Value |
|---|---|
| Main mod ID | `Pen_mod_pipette` |
| Game version | `0.46D` |
| Main assembly | `C60314247483` |
| Rule file | `Rules/hydra.json` |
| External patch file | `PipetteReactions.json` |
| Patch type | JSON data patch |

At startup the Pipette Mod:

1. loads its own `Rules/hydra.json`;
2. scans enabled mods for `PipetteReactions.json`;
3. loads their `modes`;
4. applies their `patches`;
5. recompiles and validates the affected reactions.

Patches only change rules in memory. They never write back to `hydra.json` or any source file.

---

## 2. Requirements for a Patch Mod

### 2.1 A carrier assembly is mandatory

The official `ModLoader` requires every mod manifest to declare at least one DLL:

```xml
<Assemblies>
  <Assembly>YourPatchMod.dll</Assembly>
</Assemblies>
```

A pure data mod containing only:

```text
manifest.xml
PipetteReactions.json
```

will not be recognized by the game, and the Pipette Mod cannot scan it.

### 2.2 Recommended minimal carrier DLL

The patch mod may ship an empty `IMod` implementation:

```csharp
public sealed class PatchCarrier : IMod
{
    public void Init(ModManifest manifest) { }
    public void OnEnable() { }
    public void OnDisable() { }
}
```

Requirements:

- do not reference `Pen_mod_pipette.dll`;
- do not use reflection;
- do not modify vanilla code;
- do not register items;
- keep all real rules in `PipetteReactions.json`.

### 2.3 Prerequisite

Declare the Pipette Mod as a prerequisite:

```xml
<Prerequisites>
  <Mod>Pen_mod_pipette</Mod>
</Prerequisites>
```

---

## 3. Patch File Location

```text
Mods/<PatchModFolder>/PipetteReactions.json
```

The Pipette Mod reads this file from every enabled mod at startup.

---

## 4. Patch File Structure

Root object:

```json
{
  "patches": []
}
```

`apiVersion` is no longer used and must be omitted.

Each patch requires:

```text
mode
```

Optional fields:

```text
stage
set
remove
replace
addStage
before
after
```

---

## 5. Supported Patch Operations

### 5.1 Modify existing fields

```json
{
  "mode": "reducing",
  "stage": "nomore",
  "set": {
    "temperatureMin": 44,
    "temperatureMax": 55
  }
}
```

Notes:

- `set` performs a recursive merge, not a full-stage replacement;
- use it for temperature, ratios, target floors, and similar fields.

### 5.2 Remove fields

```json
{
  "mode": "fanta",
  "stage": "fanta",
  "remove": ["temperatureMin", "temperatureMax"]
}
```

### 5.3 Replace a stage

```json
{
  "mode": "reducing",
  "stage": "nomore",
  "replace": {
    "id": "nomore",
    "kind": "reaction",
    "substrate": {
      "liquid": "dream_cap_extract",
      "locations": ["target"]
    },
    "reagent": {
      "liquid": "reducing_agent",
      "locations": ["pipette"]
    },
    "temperatureMin": 44,
    "temperatureMax": 55
  }
}
```

Notes:

- `replace` replaces the entire stage;
- always include `id`;
- the replacement must pass formula compilation and stage validation.

### 5.4 Add a stage

```json
{
  "mode": "chlorine",
  "addStage": {
    "id": "heavy_metal",
    "label": "Heavy Metal",
    "kind": "reaction",
    "substrate": {
      "liquid": "water",
      "locations": ["target"]
    },
    "reagent": {
      "liquid": "chlorine",
      "locations": ["target", "pipette"]
    },
    "targets": {
      "heavy_metal": {
        "floor": {
          "type": "absolute",
          "value": 0
        }
      }
    },
    "potency": 1
  },
  "before": "decontaminate"
}
```

Insertion:

- `before`: insert before the named stage;
- `after`: insert after the named stage;
- neither: append to the end.

---

## 6. Load Order and Conflicts

### 6.1 Order

```text
Pipette Mod built-in rules
→ external modes
→ external patches
→ recompile / validate
```

### 6.2 Conflicts

- multiple mods patching the same field: later mod wins in load order;
- unknown `mode`: log a warning and skip;
- unknown `stage`: log a warning and skip;
- duplicate `addStage` id: log a warning and skip;
- compilation or validation failure: reject that patch and keep the original rule.

---

## 7. Legacy Rules

- rule files no longer contain `apiVersion`;
- no version field is read or validated;
- old version fields do not cause rejection;
- existing stages are compiled from `kind` and their fields;
- third-party patch mods do not reference the Pipette Mod assembly.

---

## 8. Logs

Successful patch:

```text
pipette patch applied mode=chlorine stage=heavy_metal from=Pen_mod_pipette_patch_test
```

Failed patch:

```text
pipette patch skipped from=...: unknown mode ...
pipette patch rejected from=...: invalid mode ...
```

Troubleshooting order:

1. is the patch mod enabled?
2. does its `manifest.xml` declare a DLL?
3. is `PipetteReactions.json` in the mod root?
4. does the log show applied/skipped/rejected?
5. did you restart the game after changing files?

---

## 9. Known Limitations

- stages are still processed one operation at a time; multiple stages do not run in one pour;
- patches apply only to the current startup and are reloaded on restart;
- disabling or removing a patch mod restores the original rules;
- unknown JSON fields are ignored;
- patches cannot execute arbitrary code;
- pure data mods without a DLL are not supported;
- patches never modify the original `hydra.json`.

---

## 10. Test Patch Mod

Test mod:

```text
Pen_mod_pipette_patch_test
```

Current test coverage:

1. add a `heavy_metal` stage to `chlorine`;
2. consume heavy metal at `1µL chlorine : 1µL heavy metal`;
3. change the `decontaminate` chemical-contaminant floor to `absolute 0` for full removal.

This mod is for patch validation only and is not required for normal play.
