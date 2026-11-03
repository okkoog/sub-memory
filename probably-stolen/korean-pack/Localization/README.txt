v0.7.0 format

Put one catalog CSV directly in this Localization folder.

Filename:
  pskb__<WorkshopID-or-catalog-id>.csv

Rows:
  M|<ManifestID>,"1"
  W|<WorkshopID>,"1"
  K|<native localization key>,"한국어"
  K64|<base64url key>,"한국어"
  S|<hard-coded source without comma/newline>,"한국어"
  S64|<base64url source>,"한국어"

The game loads these CSV files into this mod's LoadedMod.Localization.
The bridge does not read files itself.
