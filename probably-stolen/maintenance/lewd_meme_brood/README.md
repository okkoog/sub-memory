# lewdMemeBrood Korean maintenance

Korean localization maintenance data for `claude.lewdMemeBrood` / `LewdMemeBrood.dll`.

## 0.3.20 audit

- Upstream archive: `lewdMemeBrood v0320.rar`
- Manifest version: `0.3.20`
- Source DLL SHA-256: `271fa5f34e2fada27f0d0c31ba36ee1c4efe32e854230bf991cd2f903b18c972`
- Patched DLL SHA-256: `8aa1e3d46062de8e0b4081ea2c69984cff2a2b5a60a4318c663210cfd1ab5b13`
- CLR #US entries: 1199
- Chinese-containing unique DLL strings: 269
- DLL translated: 258
- DLL internal identifiers intentionally unchanged: 11 (`萝莉`, `少女`, `御姐`, `少妇`, `熟女`, `贫乳`, `普乳`, `巨乳`, `白`, `紫`, `侵蚀`)
- Localization keys: 434/434
- Interaction lines: 135/135
- placeholder mismatches: 0
- interaction `kind`/`tags` changes: 0
- method-body validation: changed method bytes are confined to `ldstr` token operands; non-string logic changes = 0.

## Files

- `dll_translation_memory.csv` — exact source→Korean DLL string memory plus skip status.
- `localization_ko.csv` — canonical 434-key Korean localization.
- `interaction_translation_memory.csv` — exact interaction source→Korean memory while preserving kind/tags.

## Update flow

1. Extract the new upstream DLL, Localization files, and `interactionLines.json`.
2. Reuse exact source strings/keys from these memory files.
3. Translate only new or changed entries.
4. Keep matching/tag identifiers unchanged.
5. Patch only hard-coded display/debug strings in the new DLL.
6. Validate placeholders, interaction tags, and `ldstr`-only code changes.