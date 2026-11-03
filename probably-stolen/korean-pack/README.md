# Probably Stolen Korean Mod Localization Bridge — v0.7.0-dev

## Goal

Translations live only in this mod. Target Workshop mods are not edited.

v0.7.0 uses only the game's official `ModHook.OnModAssetsLoaded(LoadedMod)` lifecycle as its entry point.

## Localization layout

The game itself loads this mod's CSV files first:

```
Localization/
  pskb__3828910575.csv
  pskb__3828913695.csv
  pskb__<WorkshopID-or-catalog-id>.csv
```

The bridge reads those already-loaded dictionaries from its own `LoadedMod.Localization`. It does **not** open files or walk directories at runtime.

Rows:

```csv
M|manifest.id,"1"
W|3828910575,"1"
K|native.localization.key,"한국어 번역"
S64|<base64url encoded source string>,"한국어 번역"
```

Supported row types:
- `M|`: manifest ID alias.
- `W|`: Workshop ID alias.
- `K|`: native localization key.
- `K64|`: base64url native key when the key itself needs encoding.
- `S|`: hard-coded source text when it contains no comma/newline.
- `S64|`: base64url hard-coded source text; recommended for source-text translations.

Adding support for another mod requires translation CSV data only; the DLL does not contain a hard-coded target-mod list.

## Runtime strategy

1. `OnEnable` subscribes to `ModHook.OnModAssetsLoaded`.
2. When this Korean pack receives its own assets-loaded event, it builds catalogs only from its already-loaded `LoadedMod.Localization`.
3. Target mods are identified only from the `LoadedMod` passed by the game: `Manifest.ID`, `WorkshopId`, or `ShadowedWorkshopId`.
4. Native key translations are merged into the target's `LoadedMod.Localization["ko"]` and `["ko-KR"]`.
5. For hard-coded text, only the target's own `LoadedMod.Assembly` is inspected after the official assets-loaded event.
6. Candidate display/localizer methods are Harmony-patched only in that target assembly, and replacement occurs only on exact source-string matches.

## Deliberately excluded

v0.7.0 does **not**:
- reference, reflect on, or Harmony-patch `ModLoader`,
- use `loadingMod`,
- read or enumerate runtime files/directories,
- use `AssemblyLoad` interception,
- patch constructors or static constructors,
- use IL string rewriting/transpilers,
- globally patch TMP/TextMeshPro, game UI, File, or embedded-resource APIs.

## Permissions

The manifest declares only:
- `reflection-external`
- `dynamic-code`

There is no `files` capability.

## Verification status

- GitHub Actions compile: passed.
- Existing translation corpus converted into 16 direct Localization CSV files.
- Runtime catalog rows: 12,804 total = 32 aliases + 495 native keys + 12,277 source mappings.
- Converted CSV parse errors: 0.
- Actual in-game runtime validation: not yet completed.

Do not treat this development build as a completed release until in-game tests pass.
