Faction Reputation Expansion v1.5.3 - Harmony compatibility test patch
Workshop ID 3828971731
Fix scope: one call in ThiefCrateFirstPatch / ReorderStock's generated predicate.
Replaced missing HarmonyLib.CodeInstructionExtensions.LoadsConstant(CodeInstruction, string)
with a helper comparing the instruction opcode (ldstr) and literal operand.
No item/reputation/save processing logic was intentionally changed.
Installation:
  1. Quit the game. Back up saves and the ORIGINAL FactionReputationExpansion.dll.
  2. Replace only FactionReputationExpansion.dll in the EXISTING installed
     Workshop mod folder 4348910/3828971731.
  3. Do NOT install this ZIP as an additional separate mod.
  4. Game runtime testing is NOT completed. Test store opening with a
     backed-up save; do not save unless behavior is verified.
  5. Steam Workshop updates may restore the original DLL.
Revert by restoring your backed-up original DLL.
