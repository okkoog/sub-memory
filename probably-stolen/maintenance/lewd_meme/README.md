# lewd_meme Korean DLL maintenance

Exact-source-string translation memory for the local Korean DLL build. Do not use IL offsets as long-term keys.

## 1.3.8 audit

- Upstream archive: `lewd_meme1.3.8.zip`
- Manifest version: `1.3.8`
- Source DLL SHA-256: `fa18eb28e155ab20082e142ed9cd747431caee6e458230198c5a3d51249a3296`
- Patched DLL SHA-256: `300571649d22080b0bbb8fc433cc70aba0be7000956b0e7ece52810015a39643`
- CLR #US entries: 715
- Chinese-containing unique strings: 406
- translated: 362
- intentionally unchanged internal identifiers: 44
- translated ldstr references: 443
- placeholder mismatches: 0
- method-body validation: changed method bytes are confined to `ldstr` token operands; non-string logic changes = 0.

## Update flow

1. Extract the new upstream `MidnightPawn.dll`.
2. Match exact source strings against `translation_memory.csv` and `skip_internal.csv`.
3. Review only new/changed player-facing strings.
4. Append reviewed source→Korean pairs.
5. Patch the new upstream DLL with the shared `patch_dll.py` method.
6. Validate placeholders and remaining Chinese strings.