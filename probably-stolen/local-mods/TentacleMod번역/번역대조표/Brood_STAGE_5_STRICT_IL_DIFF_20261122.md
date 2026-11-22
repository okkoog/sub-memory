# Brood five stage labels — strict before/after IL and resource diff

- Reference commit: `f9340a1e7f9f12a1c71f5a2d11e814cd7805b0a2` (prepatch 516,096-byte Korean DLL)
- TypeDef count: 278 unchanged
- MethodDef count: 1741 unchanged
- FieldDef count: 1346 unchanged
- Managed resources: 0 unchanged, embedded bytes identical
- All method instruction opcodes and operand values identical except the five listed `ldstr` display labels in `RegisterTests`.
- Intended display-string changes: 萝莉 → 少女 -> 로리 → 소녀; 少女 → 御姐 -> 소녀 → 누님; 御姐 → 少妇 -> 누님 → 새댁; 少妇 → 熟女 -> 새댁 → 숙녀; 熟女为止 -> 숙녀까지.
- Unexpected IL changes: 0
- Note: This compares parsed CIL semantics and resources; binary layout and metadata offsets can differ with dnlib reserialization.
- Actual game/CLR execution untested.
