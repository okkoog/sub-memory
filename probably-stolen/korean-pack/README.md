# Probably Stolen Korean Mod Localization Bridge — v0.6.6-dev

This branch contains the current development rewrite of the generic Korean localization bridge.

## Goal

Translations live only in this mod. Target Workshop mods are not edited.

```
Localization/
  <WorkshopID-or-ManifestID>/
    ko.csv
```

Adding a new translation should normally require only a new/updated `ko.csv`, not a DLL code change.

## Supported CSV formats

Existing four-column translation files are supported without migration:

```csv
key,source,ko,mode
some_native_key,Original text,한국어,key
,Hard-coded source,한국어,text
some_native_key,Original text,한국어,both
```

Native two-column rows are also accepted:

```csv
some_key,한국어 번역
```

Explicit source rows are also accepted:

```csv
"@source:Auction Feed",경매 중계
```

Quoted commas, escaped quotes, and physical newlines inside quoted CSV fields are supported.

## Runtime strategy

1. Inject Korean key translations into the target mod's own `LoadedMod.Localization`.
2. Before the game's official `LoadLocalization` phase, do not patch target-mod assemblies. Early translation through game-owned registration/model APIs identifies the caller by assembly path only.
3. After `ModLoader.LoadLocalization` begins, target-mod localization/display helper methods may be patched, but only for translated mod assemblies.
4. Replace only exact source-string matches from that target mod's catalog.
5. Ignore identifier-like string parameters such as IDs, keys, paths, resources, assets, prefab names and similar technical strings.

## Startup safety

Before `ModLoader.LoadLocalization`, locale detection reads the game's persisted ES3 locale key instead of `LocalizationSettings.SelectedLocale`.

This avoids touching Unity Localization/Addressables during another mod's `OnEnable` startup window. After the official localization phase begins, the bridge may use the game's current selected locale normally.

## Deliberately excluded

The rewrite does **not**:

- patch constructors or static constructors,
- use IL string rewriting/transpilers,
- globally intercept TMP/TextMeshPro rendering,
- hook global File reads,
- hook embedded-resource reads,
- patch target-mod assemblies from `AssemblyLoad` before the official localization lifecycle.

## Verification status

- GitHub Actions compile: **passed** for v0.6.6.
- Existing translation corpus: **16 CSV files / 12,740 rows**.
- CSV parse errors: **0**.
- Existing translation CSV hashes compared with v0.5.44 package: **all 16 identical**.
- Actual in-game runtime validation: **not yet completed**.

Do not treat this development build as a completed release until the in-game tests pass.
