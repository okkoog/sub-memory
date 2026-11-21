using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace ItemPortraitHD
{
    // Observe individual Harmony callbacks, not just the final GetSprite result.
    // Never replay a provider callback in isolation: it may rely on other patches
    // or have side effects unrelated to its returned portrait.
    internal static class ReplacementTrace
    {
        private const string PatchId = ModEntry.Id + ".replacement_trace";
        private sealed class Hook { internal LoadedMod Mod; internal int Argument, Id; }
        private sealed class Observation { internal Source Source; internal Sprite Sprite; internal LoadedMod Mod; }
        private static readonly Dictionary<MethodBase, Hook> hooks = new Dictionary<MethodBase, Hook>();
        private static readonly Dictionary<int, Hook> hookIds = new Dictionary<int, Hook>();
        private static int nextHookId;
        private static readonly HashSet<MethodBase> rejected = new HashSet<MethodBase>();
        private static readonly HashSet<string> undeclared = new HashSet<string>(StringComparer.Ordinal);
        private static readonly Dictionary<string, Observation> observations = new Dictionary<string, Observation>(StringComparer.Ordinal);
        private static readonly Dictionary<string, Sprite> natives = new Dictionary<string, Sprite>(StringComparer.Ordinal);
        private static Harmony harmony;
        private static MethodInfo lookup;
        private static bool scoped;
        [ThreadStatic] private static string character;
        internal static bool Enabled => ModEntry.Instance?.Active == true && ModEntry.Instance.TraceReplacements?.Value == true;
        internal static string Status => "追踪 " + hooks.Count + " 个替换入口 / " + observations.Count + " 张图" +
            (undeclared.Count > 0 ? "；" + undeclared.Count + " 个来源未列入前置，无法追踪" : "");
        internal static IEnumerable<Source> Sources => Enabled ? observations.Values.Where(o => o.Mod.IsEnabled && o.Sprite).Select(o => o.Source) : Enumerable.Empty<Source>();

        internal static void Install()
        {
            if (!ModAccess.Ready) return;
            if (!Enabled) { if (harmony != null) Remove(); return; }
            lookup = lookup ?? AccessTools.Method(typeof(SpriteDict), "GetSprite", new[] { typeof(string) });
            if (lookup == null) return;
            harmony = harmony ?? new Harmony(PatchId);
            var info = Harmony.GetPatchInfo(lookup);
            bool changed = false;
            if (info != null)
                foreach (var patch in info.Prefixes.Concat(info.Postfixes))
                {
                    var method = patch.PatchMethod;
                    if (method == null || hooks.ContainsKey(method) || rejected.Contains(method)) continue;
                    var mod = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Assembly == method.DeclaringType?.Assembly);
                    if (mod?.Manifest == null || mod.Manifest.ID == ModEntry.Id || mod.Manifest.ID == "lewd_meme" || mod.Manifest.ID == "lewd_meme_remake") continue;
                    int argument = TraceSignatures.ResultArgument(method);
                    bool returned = method.ReturnType == typeof(Sprite);
                    if (argument < 0 && !returned) { rejected.Add(method); continue; }
                    if (!ModAccess.Allowed(mod.Manifest.ID))
                    {
                        if (undeclared.Add(mod.Manifest.ID))
                            ModEntry.Warn(new InvalidOperationException("通用追踪需要将来源 " + mod.Manifest.ID +
                                " 列入 manifest.xml 的 Prerequisites；当前仅保留其公开图片/映射和最终生效图片。"));
                        continue;
                    }
                    try
                    {
                        var hook = new Hook { Mod = mod, Argument = argument, Id = ++nextHookId };
                        hooks[method] = hook; hookIds[hook.Id] = hook;
                        var target = ModReflect.Target(method.DeclaringType, method.Name,
                            method.GetParameters().Select(p => p.ParameterType).ToArray());
                        if (target == null || !target.Equals(method)) throw new MissingMethodException("替换入口未通过受限接口验证。");
                        harmony.Patch(target, transpiler: new HarmonyMethod(typeof(ReplacementTrace), nameof(InstrumentReplacement)));
                        changed = true;
                    }
                    catch (Exception ex)
                    {
                        harmony.Unpatch(method, HarmonyPatchType.All, PatchId);
                        if (hooks.TryGetValue(method, out var rejectedHook)) hookIds.Remove(rejectedHook.Id);
                        hooks.Remove(method); rejected.Add(method);
                        ModEntry.Warn(new InvalidOperationException("通用追踪无法监听 " + mod.Manifest.Name + "/" + method.Name + ": " + ex.Message));
                    }
                }
            if (!scoped || changed)
            {
                // Rebuild the caller after detouring its callbacks, including when
                // another mod installed its patches later in the loading sequence.
                harmony.Unpatch(lookup, HarmonyPatchType.All, PatchId);
                harmony.Patch(lookup,
                    prefix: new HarmonyMethod(typeof(ReplacementTrace), nameof(BeginLookup)) { priority = Priority.First },
                    finalizer: new HarmonyMethod(typeof(ReplacementTrace), nameof(EndLookup)) { priority = Priority.Last });
                scoped = true;
            }
        }

        private static void BeginLookup(string __0, out string __state)
        { __state = character; character = Enabled ? __0 : null; }
        private static void EndLookup(string __state) { character = __state; }

        private static IEnumerable<CodeInstruction> InstrumentReplacement(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase __originalMethod)
        {
            var hook = hooks[__originalMethod];
            return TraceInstrumentation.Wrap(instructions, generator, __originalMethod, hook.Argument, hook.Id, typeof(Sprite),
                AccessTools.Method(typeof(ReplacementTrace), nameof(RecordReplacement)));
        }

        private static void RecordReplacement(int id, Sprite before, Sprite after)
        { if (character != null && hookIds.TryGetValue(id, out var hook)) Observe(hook, before, after); }

        private static void Observe(Hook hook, Sprite before, Sprite after)
        {
            try
            {
                if (!Enabled || !hook.Mod.IsEnabled || ReferenceEquals(before, after) || !after || Portraits.Owns(after)) return;
                if (!natives.TryGetValue(character, out var native)) natives[character] = native = RandomPortraits.Native(character).Resolve();
                if (ReferenceEquals(native, after)) return;
                string id = TraceSignatures.SourceId(hook.Mod.Manifest.ID, character);
                if (observations.TryGetValue(id, out var existing))
                {
                    bool revived = !existing.Sprite;
                    existing.Sprite = after;
                    if (revived) { RandomPortraits.InvalidateReplacement(character); Catalog.TraceChanged(); }
                    return;
                }
                // Holding a provider-owned Sprite is cheap; conversion stays lazy.
                // The stable ID records the last observed variant of this role.
                var observation = new Observation { Sprite = after, Mod = hook.Mod };
                observation.Source = new Source { Id = id, Provider = hook.Mod.Manifest.ID,
                    Group = hook.Mod.Manifest.Name ?? hook.Mod.Manifest.ID, Name = character + "（通用追踪）",
                    MappedCharacter = true, CharacterKey = character,
                    Resolve = () => Enabled && observation.Mod.IsEnabled ? observation.Sprite : null };
                observations.Add(id, observation);
                RandomPortraits.InvalidateReplacement(character);
                Catalog.TraceChanged();
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }

        internal static Source Find(string id)
        {
            if (!Enabled || !TraceSignatures.TryParseSource(id, out var provider, out var key)) return null;
            if (!observations.ContainsKey(id) && SpriteDict.Instance)
            {
                // A saved trace ID can be restored only if that provider is still
                // observable. Do not attribute the final winner to a missing mod.
                try { SpriteDict.Instance.GetSprite(key); }
                catch (Exception ex) { ModEntry.Warn(ex); }
            }
            return observations.TryGetValue(id, out var found) && found.Mod.IsEnabled ? found.Source : null;
        }

        internal static void OnLoaded() { natives.Clear(); }

        internal static void Remove()
        {
            harmony?.UnpatchAll(PatchId); harmony = null; scoped = false;
            hooks.Clear(); hookIds.Clear(); rejected.Clear(); observations.Clear(); natives.Clear(); character = null;
            undeclared.Clear();
            RandomPortraits.InvalidateReplacements();
        }
    }
}
