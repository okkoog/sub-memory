# lewd_meme Korean DLL maintenance

Exact-source-string translation memory for the local Korean DLL build. Do not use IL offsets as long-term keys.

## 1.3.8 r2 audit

This supersedes the first 1.3.8 audit. The initial 1.3.8 package accidentally restored upstream `Content/names.txt` and skipped the five full faction display values. r2 explicitly inherits the previously proven 1.2.0 Korean name/faction handling.

- Upstream archive: `lewd_meme1.3.8.zip`
- Manifest version: `1.3.8`
- Source DLL SHA-256: `fa18eb28e155ab20082e142ed9cd747431caee6e458230198c5a3d51249a3296`
- r2 patched DLL SHA-256: `34c88722234509f914528f163761016933f0a4fb97e01a2c1dbc5e3b86dfa5a9`
- Korean name-pool SHA-256: `27ca53b5556e7c57c3d924231a7e88284a97c7749df4c60fc30e0c3856130faf`
- CLR #US entries: 715
- Chinese-containing unique DLL strings: 406
- translated CJK strings: 367
- additional non-CJK fixes: 2 (`pool_` → `poolL`, `·` → normal space)
- intentionally unchanged internal identifiers: 39
- translated/redirected ldstr references: 487
- modified methods: 150
- placeholder mismatches: 0
- method-body validation: changed method bytes are confined to `ldstr` token operands; non-string logic changes = 0.
- name-pool validation: 13 region rows; region keys retained; CJK characters in actual surname/given-name payload = 0; middle-dot separators = 0.

### Required inherited fixes

- Never copy upstream `Content/names.txt` unchanged into the Korean local build.
- Keep the region keys intact, but Koreanize the display-name pools.
- Keep the customer cache prefix at `poolL` so a broken `pool_` cache is not reused.
- NameGen must join surname/given name with a normal space, not `·`.
- Full faction values must be Korean consistently:
  - `下层区` → `하층구역`
  - `上层区` → `상층구역`
  - `治安部` → `치안부`
  - `黑市` → `암시장`
  - `革命军` → `혁명군`
- Short matching fragments such as `下层`, `上层`, `治安`, `革命`, `卡特` remain internal source identifiers unless the new version proves otherwise.

See `name_pool_policy.md` before packaging any future version.

## Update flow

1. Extract the new upstream `MidnightPawn.dll`.
2. Match exact source strings against `translation_memory.csv`.
3. Review only new/changed player-facing strings.
4. Reapply `name_pool_policy.md`; this is mandatory, not optional cleanup.
5. Patch the new upstream DLL with the shared `patch_dll.py` method.
6. Validate placeholders, remaining Chinese strings, cache prefix, separator, faction values and the name-pool payload.
