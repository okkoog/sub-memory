# Probably Stolen Korean Mod Localization Bridge

## Translation layout

Put target-mod translations under this mod:

```
Localization/
  <WorkshopID-or-ManifestID>/
    ko.csv
```

Normal rows are native Probably Stolen localization keys:

```csv
some_key,한국어 번역
formatted_key,가격: {0}
```

The bridge injects these rows into the target mod's own `LoadedMod.Localization` after its native CSVs are loaded. Pack translations intentionally override the target mod's Korean row when the same key exists.

## Hard-coded source strings

For mods that use their own language helper instead of native localization, the same `ko.csv` can contain source-string rows:

```csv
"@source:Auction Feed",경매 중계
"@source:Choose a container.",컨테이너를 선택하세요.
```

If the source text contains a comma, quote the whole first field. Double a quote inside a quoted field.

The bridge only patches methods that look like localization helpers (for example `T`, `L`, `Text`, `Select`, `Translate`, `GetLocalized`) inside the target mod's own assemblies. It does not patch arbitrary constructors/static constructors, global UI text setters, global file APIs, or rewrite IL string constants.

When a target mod already has `en.csv`, `zh.csv`, etc., normal key rows also build source aliases automatically by matching the same key across those native locale dictionaries.
