using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class GuestPortraits
    {
        private static readonly Dictionary<string, Source> sources = new Dictionary<string, Source>(StringComparer.Ordinal);
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<GameItem, object> guests =
            new System.Runtime.CompilerServices.ConditionalWeakTable<GameItem, object>();
        [ThreadStatic] private static int building;
        internal static bool Building => building > 0;
        internal static void Begin() => building++;
        internal static void End() => building = Math.Max(0, building - 1);
        private static void Mark(GameItem item) { if (item != null) guests.GetValue(item, _ => new object()); }
        internal static bool IsGuest(GameItem item) => item != null && (guests.TryGetValue(item, out _) ||
            (item.useSpriteFromMod && item.spriteAtlasPath == "lewd_meme" && item.spritePath?.StartsWith("guestp_", StringComparison.Ordinal) == true));
        internal static bool Enabled => ModEntry.Instance?.Active == true && ModEntry.Instance.GuestHD?.Value == true;

        // Called on a cache miss with the exact selected Sprite. The ordinary
        // mode keeps its 64px render; compatibility mode replaces only rendering
        // and Sprite construction while retaining injection and returned paths.
        private static void Remember(Sprite input, string path)
        {
            if (!input || string.IsNullOrEmpty(path)) return;
            OriginalSelectedPortraits.Remember(input, path);
            sources[path] = new Source { Id = "guest-selected:" + path + ":" + input.GetInstanceID(), Name = path, Resolve = () => input };
        }

        internal static Sprite For(GameItem item)
        {
            if (!Enabled || !IsGuest(item) || !sources.TryGetValue(item.spritePath, out var source)) return null;
            var sprite = Portraits.Get(source);
            return sprite;
        }

        internal static IEnumerable<CodeInstruction> CaptureSource(IEnumerable<CodeInstruction> instructions)
            => OriginalSelectedPortraits.Rewrite(Capture(instructions, OpCodes.Ldloc_2));
        internal static IEnumerable<CodeInstruction> CaptureTrimmed(IEnumerable<CodeInstruction> instructions)
            => OriginalSelectedPortraits.Rewrite(Capture(instructions, OpCodes.Ldloc_0));

        private static IEnumerable<CodeInstruction> Capture(IEnumerable<CodeInstruction> instructions, OpCode pathLocal)
        {
            var code = instructions.ToList();
            int matched = 0;
            for (int i = 0; i < code.Count; i++)
            {
                yield return code[i];
                if (i + 3 >= code.Count || code[i].opcode != OpCodes.Ldloc_1 || code[i + 1].opcode != OpCodes.Ldarg_1 ||
                    code[i + 2].opcode != OpCodes.Ldarg_2 || !(code[i + 3].operand is MethodInfo method) ||
                    method.Name != "RenderTrimFit" || method.DeclaringType?.FullName != "MidnightPawn.Game.SpriteInjector") continue;
                // Leave the original selected Sprite on the evaluation stack.
                yield return new CodeInstruction(OpCodes.Dup);
                yield return new CodeInstruction(pathLocal);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GuestPortraits), nameof(Remember)));
                matched++;
            }
            if (matched != 1) throw new InvalidOperationException("顾客立绘源记录接口不匹配，保留原模组顾客生成。");
        }

        internal static IEnumerable<CodeInstruction> MarkItem(IEnumerable<CodeInstruction> instructions)
        {
            bool marked = false;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (!marked && instruction.opcode == OpCodes.Stloc_0)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GuestPortraits), nameof(Mark)));
                    marked = true;
                }
            }
            if (!marked) throw new InvalidOperationException("顾客物品标记接口不匹配。");
        }
    }
}
