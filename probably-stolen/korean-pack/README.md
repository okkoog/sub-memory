# Probably Stolen Korean Mod Localization Bridge — v0.6.4-dev

This branch is the development rewrite of the generic Korean localization bridge.

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

It also accepts native two-column rows:

```csv
some_key,한국어 번역
```

and explicit source rows:

```csv
"@source:Auction Feed",경매 중계
```

Quoted commas, escaped quotes, and physical newlines inside quoted CSV fields are supported.

## Runtime strategy

1. Inject Korean key translations into the target mod's own `LoadedMod.Localization`.
2. Patch target-mod localization/display helper methods only after the assembly is identified as belonging to a translated mod.
3. Translate only exact source-string matches from that target mod's catalog.
4. Adapt persistent/display-data paths such as `GameItem` metadata, `ModHelper` registration calls, and notifications when the calling target mod is identifiable.

The implementation deliberately does **not** patch constructors or static constructors, does not globally intercept TMP/TextMeshPro rendering, does not hook global File/embedded-resource reads, and does not use IL string rewriting/transpilers.

## Status

GitHub Actions compilation is verified. The existing 16-file / 12,740-row translation corpus parses successfully with the new reader. Actual in-game runtime validation is still required before this is treated as a completed release.
