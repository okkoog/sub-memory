# Probably Stolen Korean Mod Localization Bridge — v0.6.8-dev

## Goal

Translations live only in this mod. Target Workshop mods are not edited.

```
Localization/
  <WorkshopID-or-ManifestID>/
    ko.csv
```

Adding a new translation should normally require only a new/updated `ko.csv`, not a DLL code change.

## Supported CSV formats

The bridge accepts the existing four-column corpus without migration:

```csv
key,source,ko,mode
some_native_key,Original text,한국어,key
,Hard-coded source,한국어,text
some_native_key,Original text,한국어,both
```

It also accepts native two-column rows and explicit `@source:` rows. Quoted commas, escaped quotes, and physical newlines inside quoted CSV fields are supported.

## Runtime strategy

1. Inject Korean key translations into the target mod's own `LoadedMod.Localization`.
2. Before the game's official `LoadLocalization` phase, do **not** patch target-mod assemblies.
3. Early translation through game-owned registration/model APIs identifies the caller by assembly path; local mods whose folder name differs from manifest ID may additionally use the game's current private `loadingMod` tuple for identification.
4. That manifest-ID fallback is accepted only when the caller assembly is physically inside the current `loadingMod` folder.
5. After `ModLoader.LoadLocalization` begins, target-mod localization/display helper methods may be patched, but only for the translated target assembly.
6. Replace only exact source-string matches from that target mod's catalog.
7. Identifier-like parameters such as IDs, keys, paths, resources, assets and prefab names are excluded.

## Startup safety

Before `ModLoader.LoadLocalization`, locale detection reads the game's persisted ES3 locale key instead of `LocalizationSettings.SelectedLocale`.

After the official localization phase begins, the bridge may use the game's selected locale normally.

## Deliberately excluded

The rewrite does **not**:

- patch constructors or static constructors,
- use IL string rewriting/transpilers,
- subscribe to `AssemblyLoad` to pre-patch target mods,
- globally intercept TMP/TextMeshPro rendering,
- hook global File reads,
- hook embedded-resource reads.

## Verification status

- GitHub Actions compile: **passed** for v0.6.8.
- Build metadata note: repeated builds are stable within one job, but different GitHub Actions runs can still produce different PE metadata. The distributed test ZIP therefore uses the exact DLL snapshot committed on the development branch.
- Existing translation corpus: **16 CSV files / 12,740 rows**.
- CSV parse errors: **0**.
- Existing translation CSV hashes compared with v0.5.44 package: **all 16 identical**.
- Representative DLL static audit performed for CorpPresence, Proficiency, ItemStorage, PCExpansion and GameCardPK.
- Actual in-game runtime validation: **not yet completed**.

Do not treat this development build as a completed release until the in-game tests pass.
