using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ItemPortraitHD
{
    internal static class NameCompatibility
    {
        private static bool Active => ModEntry.Instance?.Active == true;
        private static void ApplyName(StoreClient client, string name)
        {
            if (client == null) return;
            bool keep = Active && (IsModGuest(client)
                ? ModEntry.Instance.KeepModGuestNames?.Value == true
                : ModEntry.Instance.KeepGuestNames?.Value == true);
            if (!keep) client.displayName = name;
        }

        // Use the game's provenance field, not the sprite's provider: a base-game
        // customer with a portrait replacement is still a base-game customer.
        private static bool IsModGuest(StoreClient client) => !string.IsNullOrWhiteSpace(client.sourceModId);

        // Both pool reuse and the origin's daily visitor overwrite displayName.
        // Intercept only that assignment, preserving portrait selection and arrival.
        internal static IEnumerable<CodeInstruction> GuestNames(IEnumerable<CodeInstruction> instructions)
        {
            int count = 0;
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Stfld && instruction.operand is FieldInfo field &&
                    field.DeclaringType == typeof(StoreClient) && field.Name == "displayName")
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = AccessTools.Method(typeof(NameCompatibility), nameof(ApplyName));
                    count++;
                }
                yield return instruction;
            }
            if (count != 1) throw new InvalidOperationException("顾客改名接口不匹配。");
        }

        internal static List<string[]> FilterRegions(List<string[]> regions)
        {
            return SelectRegions(regions, Active ? ModEntry.Instance.NameRegion?.Value : null);
        }

        internal static List<string[]> SelectRegions(List<string[]> regions, string restriction)
        {
            if (regions == null || (restriction != "仅日本" && restriction != "仅中国" && restriction != "仅中日韩")) return regions;
            return regions.Where(r => r != null && r.Length >= 3 &&
                (restriction == "仅日本" ? r[0] == "日本" : restriction == "仅中国" ? r[0] == "中国" :
                r[0] == "中国" || r[0] == "日本" || r[0] == "韩国")).ToList();
        }

        internal static IEnumerable<CodeInstruction> RegionalNames(IEnumerable<CodeInstruction> instructions)
        {
            int count = 0;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (instruction.opcode == OpCodes.Ldsfld && instruction.operand is FieldInfo field &&
                    field.Name == "regions" && field.DeclaringType?.FullName == "MidnightPawn.Game.NameGen")
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NameCompatibility), nameof(FilterRegions)));
                    count++;
                }
            }
            if (count != 4) throw new InvalidOperationException("名字地区筛选接口不匹配。");
        }
    }
}
