Probably Stolen Korean Pack v0.7.5 — Supplemental Format Fix

Runtime:
- Exact restored/user-confirmed v0.7.5 DLL and manifest.
- No new runtime patching code.

Data-only fix:
- Six pskb__supp_*.csv files were rewritten to the actual
  ModLoader.LoadLocalization parser format.
- Keys are unquoted.
- Source/key strings containing ASCII commas use S64/K64.
- Embedded newlines use literal \n instead of physical multiline CSV rows.

Reason:
Probably Stolen's localization loader is not a full CSV parser:
it splits each physical line at the first comma and only unquotes the VALUE.
Quoted keys such as "S|收纳箱" therefore never matched KoreanPack's S| parser.

No third-party mod files are modified.
