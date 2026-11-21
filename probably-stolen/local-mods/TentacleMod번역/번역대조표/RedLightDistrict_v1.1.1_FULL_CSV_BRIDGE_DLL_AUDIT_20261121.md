# RedLightDistrict v1.1.1 full CSV-bridge Korean DLL static audit

- Original: 152970752 bytes / SHA256 4f4f20a00e4754302c53879bfaeb7a7503df10e2f05ff6ad3744be31088159f8.
- Revised: 153228288 bytes / SHA256 6089ad1d83d1a3131cc9e141d9c4dd7d66f8f2da6b0afde7010db403b4def03f.
- All 3619 active Chinese original IL caller sites were independently matched by original method signature, IL offset and literal, then rewritten to call a locale bridge. No global #US string replacement.
- Runtime lookup: RedLightDistrict.Korean.CsvLookup.Read reads LoadedMod.Localization['ko'] via IDictionary. The owner LoadedMod reference is captured in original RedLightDistrictMod.OnModAssetsLoaded callback. Missing, empty, or unavailable values fall back to per-site original Chinese; no mod files read or external reflection used.
- External KO CSV: 3512 translated caller keys, 107 protected identifier/comparison/saved-header keys. Korean translation is not baked into DLL, so CSV updates work after game loads its localization.
- Original 1617 managed resources and all their bytes are unchanged; CLR TypeDef +1, MethodDef +1, Field +1 (bridge only).
- The previous translation folder's 214-UI-only direct IL patch DLL did not contain this full CSV bridge and has been superseded. The new DLL must be installed WITH Localization/ko.csv.
- No actual Unity/CLR game startup, mod loader, save data or compatibility test was performed. OnModAssetsLoaded may occur after some early string evaluation, which will then use the original Chinese fallback.
