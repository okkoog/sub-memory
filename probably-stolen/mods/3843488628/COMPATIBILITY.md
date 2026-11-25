# Compatibility Guide (`Pen_mod_Request_UI`)

For **authors of other mods**. This document covers three things:

1. what this mod **recognizes** on the commission panel (code-level rules);
2. which **interfaces** this mod exposes;
3. how to write an integration that **keeps vanilla behaviour intact when this mod is not installed**.

> This mod takes over the vanilla "Market → Commissions" panel: it renders a two-column grid with its own scrolling,
> a left-hand source/faction filter rail, and third-party commissions as vanilla-styled cards.
> When this mod is disabled, the panel is fully restored to vanilla (see section 6).

---

## 1. Overview

There are three kinds of cards on the panel:

| Kind | Detection | Handling by this mod |
|---|---|---|
| **Vanilla item commission** | `StoreRequestElement` with `storeRequest != null` | Moved into a cell (`rb_cell`), decorated (reward / preview / expiry), filtered by faction, counted as "ready" |
| **Framework commission** | Rendered by this mod from registered data; name prefix `rb_fw_`, carries a `FrameworkCard` component | Fully owned by this mod (content, button, click) |
| **Foreign card** | `StoreRequestElement` with `storeRequest == null` and **no** `FrameworkCard` | **Not wrapped, not filtered, not counted**; its `PCUIManager.gridLayout` is a "hidden data area", so it is **not displayed** |

The "hidden data area" is the key behaviour: while the panel is taken over, the vanilla `gridLayout` gets
`CanvasGroup { alpha = 0, blocksRaycasts = false, interactable = false }`, and every visible card is rendered
inside this mod's own `rb_content`.

---

## 2. What this mod recognizes

### 2.1 Vanilla item commissions
- Component: `StoreRequestElement`;
- `storeRequest != null`;
- Created by the game in `PCUIManager.OnChange`; this mod only reparents them into its own cells (it does not change
  their data or the delivery logic).

### 2.2 Framework commissions (rendered by this mod)
- GameObject name prefix: `rb_fw_`;
- Component: `FrameworkCard`, with fields `Key / CommissionId / FactionId / SourceId / SourceName`;
- **Adapters must skip these while scanning the panel**, otherwise they will pick up cards this mod already renders.

### 2.3 Foreign cards
All of the following:
- component `StoreRequestElement`;
- `storeRequest == null`;
- no `FrameworkCard`.

Handling: **not wrapped, not filtered, not counted** — left to its own author. Because `gridLayout` is a hidden
data area, such cards are not displayed.

> Conclusion: to show cards in this mod's UI, use the interface in section 3 (framework commissions);
> do not inject self-drawn cards into `gridLayout`.

### 2.4 Source groups
- each `SourceSpec.Id` becomes one group in the left rail, with group key `source:<Id>`;
- position: **below "Other"**; multiple sources are ordered by display text;
- a source group contributes to the "current commissions" count only — **never to "ready to complete"**.

### 2.5 Counting rules
- "current commissions" = vanilla item commissions + framework commissions;
- "ready to complete" = **vanilla item commissions only** that can currently be handed in (judged from the player's items).

### 2.6 Injected node names (internal convention)
Objects injected by this mod carry the `rb_` prefix:

| Node | Purpose |
|---|---|
| `rb_viewport` | Self-built viewport (mask + own `ScrollRect`) |
| `rb_content` | Visible content container (two-column `GridLayoutGroup`, auto height) |
| `rb_cell_<n>` | Cell (one slot per card) |
| `rb_fw_<key>` | Framework commission card |
| `rb_rail` | Left rail frame |
| `rb_rail_view` / `rb_rail_sbar` | Left rail scroll area / scrollbar |
| `rb_scrollbar` | Right-hand scrollbar (clone of the vanilla one) |
| `rb_hint` | Empty-list hint bar |

Their purpose:
1. **ownership** — takeover and restore only touch these nodes, never vanilla or third-party objects;
2. **adapter exclusion** — e.g. skip `rb_fw_` while scanning for foreign cards;
3. **debuggability** — instantly recognizable in the hierarchy.

These names are an **internal convention**: you may rely on them, but they are not guaranteed across major versions.

---

## 3. Available interface

Namespace `PenRequestUI`, static class `CommissionApi`.

### 3.1 Methods

| Member | Signature | Notes |
|---|---|---|
| `IsAvailable` | `bool` | Whether this mod is enabled (`false` when absent/disabled) |
| `Register` | `bool Register(string modId, CommissionSpec spec)` | Register a single fixed commission |
| `RegisterSource` | `bool RegisterSource(SourceSpec source)` | Register a named dynamic source (recommended) |
| `RegisterSource` | `bool RegisterSource(string modId, Func<IEnumerable<CommissionSpec>> source)` | Simple source (modId is used as the source name) |
| `Unregister` | `void Unregister(string modId, string commissionId)` | Remove one entry |
| `UnregisterAll` | `void UnregisterAll(string modId)` | Remove every registration of that mod |
| `RequestRefresh` | `void RequestRefresh()` | Ask for a redraw (deferred and coalesced, never synchronous) |

### 3.2 `SourceSpec`

```csharp
public sealed class SourceSpec
{
    public string Id;                                  // stable source id -> left rail group "source:<Id>"
    public Func<string> DisplayName;                   // group label (follows the game language)
    public Func<IEnumerable<CommissionSpec>> Snapshot; // called once per refresh; pure query, no side effects
}
```

### 3.3 `CommissionSpec`

Structured fields:

| Field | Type | Notes |
|---|---|---|
| `Id` | `string` | Unique within the source |
| `Title` | `Func<string>` | Required |
| `Description` | `Func<string>` | Description |
| `FactionId` | `Func<string>` | `FACTION_*`; empty/invalid → "Other" group |
| `RequesterName` | `Func<string>` | Rendered as the vanilla "from xx" line |
| `DaysLeft` | `Func<int>` | Days remaining; `< 0` hides it |
| `CreditReward` | `Func<int>` | Credit reward (display only) |
| `ReputationReward` | `Func<int>` | Reputation reward (display only) |
| `PreviewItemId` | `Func<string>` | Optional: use a vanilla item icon as preview |
| `IsReady` | `Func<bool>` | Whether the button is usable; must be a pure query |
| `StateLabel` | `Func<string>` | Optional: state text (shown on the "requirement" line) |
| `ButtonLabel` | `Func<string>` | Optional: button text (defaults to vanilla wording) |
| `Submit` | `Action<string>` | Invoked on click (argument = `Id`) |

Verbatim override fields (higher priority than the structured fields):

| Field | Notes |
|---|---|
| `TitleText` / `DescriptionText` / `RequesterText` | Provide the final text of that line |
| `DurationText` / `RequirementText` / `RewardText` | Provide the whole "duration / requirement / reward" block (`\n` for multiple lines) |

Action callback with a result (higher priority than `Submit`):

```csharp
public Func<string, string> SubmitJson;   // returns {"ok":true,"code":"ok","message":""}
```

- `ok = true` → this mod redraws once, deferred;
- `ok = false` → only logged, **no redraw** (call `RequestRefresh()` yourself when your action is done);
- exception → `framework submit failed` is logged, no redraw.

### 3.4 Constraints

- every `Func` must be a **pure query** (it may be called several times per frame while rendering);
- at most **200** entries per source; the rest are dropped with a warning;
- entries of a disabled/unloaded mod disappear automatically;
- `RequestRefresh()` coalesces multiple requests within one frame into a single redraw.

---

## 4. Two integration paths

### Path A: compile-time integration (recommended, most robust)

Do **not** hard-depend on this mod in `manifest.xml` (see section 5). Reference the DLL at compile time, then check
availability at runtime before registering:

```csharp
PenRequestUI.CommissionApi.RegisterSource(new PenRequestUI.SourceSpec
{
    Id = "my_mod.commissions",
    DisplayName = () => "My commissions",
    Snapshot = BuildSpecs
});
```

### Path B: capture-style adapter (when the other mod must not be changed)

For mods that inject self-drawn cards into `gridLayout`, a **separate adapter mod** scans those cards after
`PCUIManager.OnChange`, copies their text and `acceptButton.onClick`, and registers them with this mod through `SourceSpec`.

Key rules (summarized from real adapter work):

1. **timing** — schedule one deferred capture (about 0.05 s) from the `PCUIManager.OnChange` postfix, after the other
   mod has finished building its cards;
2. **only the right cards** — `StoreRequestElement` with `storeRequest == null`, and the GameObject name must **not**
   start with `rb_fw_` (those are framework cards rendered by this mod and must be skipped);
3. **read, never write** — only read text fields and `acceptButton.onClick` (`UnityEvent`); do **not** touch the other
   card's `activeSelf`, do not reparent it, do not destroy it (while this mod owns the panel, the vanilla grid is invisible anyway);
4. **deterministic label source** — read the button text the same way the other mod does (e.g. the label TMP inside the
   button); if it cannot be read, fall back to a fixed value that matches that mod's semantics instead of the vanilla default;
5. **debounce** — compute a content signature and call `RequestRefresh()` only when it changes; merge bursts within the
   same frame into one refresh;
6. **fail soft** — when nothing can be captured (panel not ready, other mod not rendered yet) **keep the previous data**;
   never clear it;
7. **unregister** — call `UnregisterAll` when the adapter is disabled so the entries disappear immediately.

---

## 5. Writing an *optional* integration (vanilla behaviour without this mod)

This is the most important section. Goal: **installing or removing this mod must not change vanilla behaviour or yours**.

### 5.0 First decide which kind of integrator you are

| Type | Characteristics | Declare `<Prerequisites>`? |
|---|---|---|
| **Generic mod** | Merely "optionally coexists" with this mod (has its own complete experience, works with or without it) | ❌ No — follow 5.1–5.5 |
| **Dedicated adapter** | Exists for "this mod + one specific mod"; meaningless without the host | ✅ May declare `<Prerequisites><Mod>Pen_mod_Request_UI</Mod></Prerequisites>` |

> Even for a dedicated adapter: **every call site still needs `try/catch`** (the host may be disabled by the user),
> and before registering you must check both `ModHelper.IsModRunning("Pen_mod_Request_UI")` and
> `PenRequestUI.CommissionApi.IsAvailable`.

### 5.1 Generic mods: no hard dependency
- **Do not** write `<Prerequisites><Mod>Pen_mod_Request_UI</Mod></Prerequisites>` in `manifest.xml`;
- treat it as an **optional dependency**: bind if present, otherwise keep your own logic.

### 5.2 Every call must fail safely

If you reference the DLL at compile time but the player does not have this mod installed, the first JIT touching those
types may throw `TypeLoadException`. Therefore **wrap every call site in `try/catch`** and treat failure as "no host";
before registering, check both `ModHelper.IsModRunning("Pen_mod_Request_UI")` and `PenRequestUI.CommissionApi.IsAvailable`:

```csharp
private const string HostId = "Pen_mod_Request_UI";
private bool _hostBound;

public void OnEnable()
{
    TryBindHost();          // on failure do nothing: keep your own UI and behaviour
}

private bool TryBindHost()
{
    try
    {
        if (!ModHelper.IsModRunning(HostId)) return false;
        if (!PenRequestUI.CommissionApi.IsAvailable) return false;

        PenRequestUI.CommissionApi.RegisterSource(new PenRequestUI.SourceSpec
        {
            Id = ModContext.Id + ".commissions",
            DisplayName = () => "My commissions",
            Snapshot = BuildSpecs
        });
        _hostBound = true;
        return true;
    }
    catch
    {
        _hostBound = false;   // not installed / version mismatch / any exception -> fall back
        return false;
    }
}

public void OnDisable()
{
    if (!_hostBound) return;
    try { PenRequestUI.CommissionApi.UnregisterAll(ModContext.Id); } catch { }
    _hostBound = false;
}
```

### 5.3 Fallback rules (when not bound)

- do not hide your own cards, do not modify the vanilla layout, do not change any data;
- do not disable yourself because detection failed — keep running exactly as without this mod;
- if your UI is unrelated to the commission panel, the presence of this mod should be imperceptible.

### 5.4 If your UI works by injecting cards into `gridLayout`

- while this mod owns the panel, `gridLayout` is a hidden data area → your cards are **not displayed**;
- two ways to coexist:
  1. switch to the interface in section 3 and let this mod render your data (recommended);
  2. write a separate adapter that extracts the data and hands it to this mod (see section 4, Path B);
- after this mod is disabled, `gridLayout` becomes visible again and your cards show as before — so **never cache
  references to nodes created by this mod**.

### 5.5 Things not to rely on

- `rb_*` nodes (internal convention, see 2.6);
- the parent of `gridLayout` (it is moved into `rb_viewport` while taken over, and restored afterwards);
- reparenting vanilla objects, or modifying vanilla `StoreRequestElement` data;
- reading the panel asynchronously after `OnChange` and holding references long-term (the panel rebuilds cards).

---

## 6. What this mod changes in the panel (for conflict analysis)

While taken over:

1. vanilla `gridLayout` is moved into `rb_viewport` and gets `CanvasGroup(alpha=0)` → hidden data area;
2. the vanilla `ScrollRect` is disabled and its scrollbar hidden; scrolling is handled by this mod (scrollbar cloned from vanilla);
3. a new `rb_content` (two-column `GridLayoutGroup`) hosts all visible cards;
4. vanilla item commission cards are moved from `gridLayout` into `rb_cell` (centred, uniformly scaled);
5. a left rail `rb_rail` (source/faction filter, counts, scrolling) and `rb_hint` (empty-list hint) are created;
6. panel buttons (close, etc.) are brought to the front so they are not covered by new nodes.

On disable, in order: cards are moved back into `gridLayout` (1:1 scale and anchors restored) → `CanvasGroup` removed →
`rb_content`/`rb_cell`/`rb_rail`/`rb_viewport`/own scrollbar destroyed → `gridLayout` parent, anchors,
`GridLayoutGroup` parameters, vanilla `ScrollRect` and scrollbar state restored.

---

## 7. Harmony patches

| Target | Kind | Purpose |
|---|---|---|
| `PCUIManager.OnChange` | Prefix + Postfix | Prefix: purge invalid entries; Postfix: take over the panel structure and sync cards |
| `StoreRequest.HandleMinorScriptedRequest` | Prefix | Fixes the vanilla scripted-request date check (can be disabled in config) |
| `StoreRequestElement.OnClick` | Prefix | Skips vanilla delivery logic when `storeRequest == null` (prevents a null-reference from aborting the click) |

Interaction with other mods that also patch `PCUIManager.OnChange`: ordering follows Harmony priority. This mod's
takeover does not depend on whoever runs first, but before changing the card hierarchy, check whether this mod has taken
over (does `gridLayout` have an `rb_viewport` ancestor?).

---

## 8. Compatibility matrix

| Other mod type | Result | Notes |
|---|---|---|
| Data-layer (adds real `StoreRequest` to `PlayerStore.storeRequests`) | ✅ Works out of the box | Goes through the vanilla path: wrapping, decoration, filtering, ready count |
| Framework (registers commissions through this mod's API) | ✅ Official path | See section 3 |
| UI-injecting (pushes self-drawn cards into `gridLayout`) | ⚠️ Cards are not displayed | Use the API, or write an adapter |
| Contract-based (e.g. `board.v1.list/action` + presenter) | ❌ Not implemented | This mod does not consume such contracts |
| Behaviour-rewriting (patches vanilla ordering/visibility) | ⚠️ Verify individually | May conflict with re-layout / hidden grid |

**Known limitations**
1. this mod exposes no `ModHelper` runtime service — third parties must use the compile-time API or a capture-style adapter;
2. contract-style integrations (such as `board.v1`) are not implemented;
3. cards injected by UI-injecting mods are hidden by the hidden data area.

---

## 9. Configuration

| Key | Default | Notes |
|---|---|---|
| `urgent_days` | `2` | Cards show an expiry warning when days remaining ≤ this value |
| `purge_invalid_requests` | on | When opening the panel, removes broken vanilla commissions (`requiredItem` empty / count < 1 / duration ≤ 0) |
| `fix_scripted_requests` | on | Fixes the vanilla scripted-request date check |
| `debug` | off | Enables debug logging (English) |

---

## 10. Troubleshooting (log keys)

| Symptom | Log | Meaning |
|---|---|---|
| Panel taken over? | `body='…' … content='rb_content' alpha=0` | `alpha=0` means the hidden data area is active |
| Left rail rebuilt? | `rail update n=… created=… removed=…` | Normally `created=0 removed=0` |
| Light refresh | `soft sync: …` / `soft sync: no framework change …` | Only framework-side changes trigger re-layout |
| Button label | `button label target='…' 'old' -> 'new'`, `locale off on '…'` | The latter means localization override is disabled for that field |
| Framework entries | `framework providers=… entries=… skippedNotRunning=…` | Number of registered sources and entries |
| Click submit | `framework submit id=…`, `action ok|failed id=… code=…` | `failed` means the provider returned `ok=false` |

---

## 11. Version and promise

- Supported game version: `1.0.*` (see `manifest.xml`);
- this document describes the **current implementation**: interface signatures, field names, node names and config keys
  follow this document;
- `rb_*` node names and log texts are internal details and may change between versions; interface fields are kept
  backward compatible where possible, and breaking changes are noted in the workshop update notes.
