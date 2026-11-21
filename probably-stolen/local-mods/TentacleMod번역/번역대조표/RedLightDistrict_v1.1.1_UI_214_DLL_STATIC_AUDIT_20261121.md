# RedLightDistrict v1.1.1 partial UI Korean DLL audit

- Original bytes: 152970752; SHA256 4f4f20a00e4754302c53879bfaeb7a7503df10e2f05ff6ad3744be31088159f8
- Patched bytes: 152968192; SHA256 c3ca6b733e3d17d45a007e893377788002085f9cbac057bd63610e958d20f77e
- Source Chinese IL strings: 3133 unique / 3619 live references
- Human-reviewed UI strings translated: 214 unique, 262 specific IL caller sites
- All original managed resources identical; TypeDef/MethodDef row counts preserved
- Internal identifiers, registrations, file names, and non-UI callers of shared strings are not mass replaced
- This is an incomplete UI translation; 2919 unique originally Chinese strings were not in the current translation batch (some are debug/identifier strings).
- Unity/CLR loading, save data and compatibility NOT TESTED.
