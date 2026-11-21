using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ItemPortraitHD;

static class OriginalSelectionRegression
{
    const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    static List<CodeInstruction> IL(MethodBase method) => PatchProcessor.GetOriginalInstructions(method, (ILGenerator)null);
    static bool Call(CodeInstruction i, string name) => i.operand is MethodInfo m && m.Name == name;

    internal static void Run(string dependencyPath, Action<bool, string> check)
    {
        var hd = typeof(ModEntry).Assembly;
        var original = Assembly.LoadFrom(dependencyPath);
        var entry = new ModEntry();
        var instanceField = typeof(ModEntry).GetField("Instance", Static);
        var oldEntry = instanceField.GetValue(null);
        var registered = (List<LoadedMod>)typeof(ModLoader).GetField("mods", Static).GetValue(null);
        var previous = registered.ToArray();
        var compat = hd.GetType("ItemPortraitHD.OriginalCompat");
        var selected = hd.GetType("ItemPortraitHD.OriginalSelectedPortraits");
        var flag = (ConfigEntry<bool>)Activator.CreateInstance(typeof(ConfigEntry<bool>), Instance,
            null, new object[]{false}, null);
        try
        {
            registered.Clear();
            registered.Add(new LoadedMod { Manifest = new ModManifest { ID = "item_portrait_hd", Prerequisites = new List<string>{"lewd_meme"} },
                Assembly = hd, IsEnabled = true });
            registered.Add(new LoadedMod { Manifest = new ModManifest { ID = "lewd_meme" }, Assembly = original, IsEnabled = true });
            instanceField.SetValue(null, entry);
            typeof(ModEntry).GetField("Active", Instance).SetValue(entry, true);
            typeof(ModEntry).GetField("OriginalPortraitSelection", Instance).SetValue(entry, flag);
            bool Mode() => (bool)typeof(ModEntry).GetProperty("UseOriginalPortraitSelection", Static).GetValue(null);
            check(!Mode(), "original-selection compatibility mode defaults off");
            flag.Set(true);
            check(Mode(), "enabled configuration activates original-selection mode");
            var resolver = compat.GetMethod("UseDependencyResolver", Static);
            bool DependencyResolver(string source) => (bool)resolver.Invoke(null, new object[] { source });
            foreach (var source in new[] { "iph|embedded:baishe_r18_pack:femaleRev5:R18Full/femaleRev_4.png",
                "iph|local:example/下层区/portrait.png", "iph|final:femaleLower_0", "iph|vanilla:femaleUpper1" })
                check(!DependencyResolver(source), "saved managed source stays manager-resolvable in original-only mode: " + source);
            check(DependencyResolver("femaleLower1") && DependencyResolver("cute_portraits:cp_portrait_femaleLower1"),
                "original-only mode preserves dependency resolution for native and mod sprite keys");
            flag.Set(false);
            check(!DependencyResolver("iph|final:femaleLower_0") && !DependencyResolver("femaleLower1"),
                "normal mode retains manager injection routing");
            flag.Set(true);
            var portraitType = hd.GetType("ItemPortraitHD.Portraits");
            var load = IL(typeof(ModEntry).GetMethod("OnLoaded", Instance));
            check(load.Any(i => Call(i, "RetryUnavailable")) && !load.Any(i => Call(i, "Clear") &&
                ((MethodInfo)i.operand).DeclaringType == portraitType),
                "save loading retries unavailable sources without destroying published textures");
            var retry = IL(portraitType.GetMethod("RetryUnavailable", Static));
            check(retry.Any(i => i.operand is FieldInfo f && f.Name == "failed") &&
                !retry.Any(i => Call(i, "Release") || Call(i, "Destroy")),
                "retry resets failed source state without invalidating live sprites");
            var restore = IL(portraitType.GetMethod("TryRestoreOriginal", Static));
            check(restore.FindIndex(i => Call(i, "OriginalFor")) < restore.FindIndex(i => Call(i, "Set")) &&
                restore.Count(i => Call(i, "Set")) == 2,
                "restore validates original availability before clearing override and rolls back on refresh failure");
            var window = IL(typeof(PortraitWindow).GetMethod("Draw", Instance));
            check(window.Any(i => Call(i, "TryRestoreOriginal")),
                "F7 restore button uses availability-checked restoration");
            var init = IL(typeof(ModEntry).GetMethod("Init"));
            int keyIndex = init.FindIndex(i => Equals(i.operand, "original_portrait_selection"));
            check(keyIndex >= 0 && init[keyIndex+1].opcode == OpCodes.Ldc_I4_0 &&
                keyIndex < init.FindIndex(i => Equals(i.operand, "resolution")),
                "new default-off setting is registered before resolution and all existing sections");

            var factoryType = original.GetType("MidnightPawn.Game.ItemFactory");
            var fixedFactory = factoryType.GetMethod("CreateMaidenOfFaction", Static, null, new[] { typeof(string) }, null);
            var legacyFactory = factoryType.GetMethod("CreateMaiden", Static);
            var factory = (MethodBase)compat.GetMethod("ResolveFactory", Static).Invoke(null, null);
            check(factory == (fixedFactory ?? legacyFactory), "factory target resolved by actual method signature");
            if (fixedFactory != null)
                check(IL(legacyFactory).Any(i => Equals(i.operand, fixedFactory)), "scavenging entry delegates to the patched factory");
            var transform = compat.GetMethod("TransformFactory", Static);
            var input = IL(factory);
            var output = ((IEnumerable<CodeInstruction>)transform.Invoke(null, new object[]{IL(factory), true})).ToList();
            string[] Names(IEnumerable<CodeInstruction> code) => code.Where(i => i.operand is MethodInfo)
                .Select(i => (MethodInfo)i.operand).Where(m => m.DeclaringType?.FullName == "MidnightPawn.Game.MaidenIdentity" ||
                    m.DeclaringType?.FullName == "MidnightPawn.Game.SpriteInjector").Select(m => m.Name).ToArray();
            check(Names(input).SequenceEqual(Names(output)), "compatibility mode preserves every original primary/fallback selection and injection call");
            check(output.Count == input.Count + 1 && output.Count(i => Call(i, "PickBody")) == 1 &&
                output.Count(i => Call(i, "PickBreastForBody")) == 1,
                "independent attribute hooks remain in original-selection mode");
            check(!output.Any(i => i.operand is MethodInfo m && m.DeclaringType?.Name == "RandomPortraits"),
                "compatibility factory contains no manager faction, character, provider or PNG random selection");
            int factionIndex = output.FindIndex(i => i.operand is FieldInfo f && f.Name == "FACTIONS");
            int bodyPick = output.FindIndex(i => Call(i, "PickBody"));
            check(factionIndex >= 0 && Call(output[factionIndex+1], "Pick") && bodyPick > factionIndex &&
                bodyPick < output.FindIndex(i => Call(i, "PickBreastForBody")),
                "original faction picker is preserved and body is initialized before body-dependent attributes");
            var normal = ((IEnumerable<CodeInstruction>)transform.Invoke(null, new object[]{IL(factory), false})).ToList();
            int SelectionHooks(List<CodeInstruction> code) =>
                code.Count(i => Call(i, "PickForItem") || Call(i, "PickPrimary") || Call(i, "PickFallback"));
            int expectedHooks = input.Any(i => Call(i, "SpriteFor")) ? 2 : 1;
            check(SelectionHooks(normal) == expectedHooks && normal.Count(i => Call(i, "PickFaction")) == 1 &&
                !normal.Any(i => Call(i, "SpriteFor")), "mode off retains existing weighted selection on both paths");
            if (fixedFactory != null)
            {
                compat.GetMethod("BindFactorySelection", Static).Invoke(null, null);
                check(new[] { "identitySprite", "anyPortrait", "identityInject", "trimmedInject" }.All(field =>
                    compat.GetField(field, Static).GetValue(null) is Delegate hook && hook.Method.DeclaringType.Assembly == original),
                    "all fallback selection delegates bind through the checked game API");
                int guard = normal.FindIndex(i => Call(i, "IsNullOrEmpty"));
                check(normal.FindIndex(i => Call(i, "PickBody")) < guard && guard < normal.FindIndex(i => Call(i, "PickFaction")) &&
                    normal.Count(i => i.opcode == OpCodes.Starg_S || i.opcode == OpCodes.Starg) == 1,
                    "explicit faction skips only the weighted faction branch and still initializes the body");
                var begin = compat.GetMethod("BeginFactory", Static);
                var end = compat.GetMethod("EndFactory", Static);
                var scope = new object[] { "黑市", false };
                begin.Invoke(null, scope);
                try
                {
                    compat.GetField("identitySprite", Static).SetValue(null, (Func<string, string, string>)((f, j) => f + ":" + j));
                    compat.GetField("anyPortrait", Static).SetValue(null, (Func<string>)(() => "fallback"));
                    compat.GetField("identityInject", Static).SetValue(null, (Func<string, int, int, string>)((p, w, h) => "native:" + p));
                    compat.GetField("trimmedInject", Static).SetValue(null, (Func<string, int, int, string>)((p, w, h) => "managed:" + p));
                    check((bool)scope[1] &&
                        (string)compat.GetMethod("PickPrimary", Static).Invoke(null, new object[] { "黑市", "商人", null }) == "黑市:商人" &&
                        (string)compat.GetMethod("PickFallback", Static).Invoke(null, new object[] { null }) == "fallback",
                        "fixed faction keeps native primary and fallback selectors without reading the weighted pool");
                    var injectIdentity = compat.GetMethod("InjectIdentity", Static);
                    check((string)injectIdentity.Invoke(null, new object[] { "native_key", 64, 64 }) == "native:native_key" &&
                        (string)injectIdentity.Invoke(null, new object[] { "iph|chosen", 64, 64 }) == "managed:iph|chosen",
                        "native and manager source IDs keep their respective injector paths");
                }
                finally { end.Invoke(null, new[] { scope[1] }); }
                check(!(bool)compat.GetProperty("FixedFaction", Static).GetValue(null), "fixed faction scope ends with the factory call");
                var finalFlag = (ConfigEntry<bool>)Activator.CreateInstance(typeof(ConfigEntry<bool>), Instance,
                    null, new object[] { true }, null);
                typeof(ModEntry).GetField("FinalPortraitSelection", Instance).SetValue(entry, finalFlag);
                flag.Set(false);
                try
                {
                    check(compat.GetMethod("PickPrimary", Static).Invoke(null, new object[] { "黑市", "商人", null }) == null &&
                        compat.GetMethod("PickFallback", Static).Invoke(null, new object[] { null }) == null,
                        "final mode cannot escape a failed selection into native identity or arbitrary mod portrait pools");
                    check((string)compat.GetMethod("InjectIdentity", Static).Invoke(null, new object[] { "femaleLower1", 64, 64 }) == "managed:femaleLower1",
                        "final-mode plain character fallback uses the live trimmed resolver instead of legacy cp_portrait precedence");
                }
                finally { finalFlag.Set(false); flag.Set(true); }
                var router = hd.GetType("ItemPortraitHD.RandomPortraits");
                var assigned = router.GetMethod("PickAssigned", Static);
                var bodies = (string[])hd.GetType("ItemPortraitHD.AdultBodySources").GetField("Bodies", Static).GetValue(null);
                var providers = new string[bodies.Length];
                providers[Array.IndexOf(bodies, "御姐")] = "test-provider";
                providers[Array.IndexOf(bodies, "少妇")] = "local-folder:test";
                typeof(ModEntry).GetField("BodyProviders", Instance).SetValue(entry, providers);
                flag.Set(false);
                check(assigned.Invoke(null, new object[] { "黑市", "御姐" }) == null,
                    "disabled body routing keeps fixed faction native selection");
                typeof(ModEntry).GetField("BodySourcesEnabled", Instance).SetValue(entry, true);
                var bodyProvider = hd.GetType("ItemPortraitHD.AdultBodySources").GetMethod("Provider", Static);
                check((string)bodyProvider.Invoke(null, new object[] { "御姐" }) == "test-provider" &&
                    (string)bodyProvider.Invoke(null, new object[] { "少妇" }) == "local-folder:test" &&
                    assigned.Invoke(null, new object[] { "黑市", "熟女" }) == null &&
                    assigned.Invoke(null, new object[] { "黑市", "unknown-body" }) == null,
                    "enabled routing requires an eligible body and an explicitly selected provider");
                flag.Set(true);
                check(assigned.Invoke(null, new object[] { "黑市", "御姐" }) == null,
                    "original selection compatibility switch takes priority over fixed faction body routing");
                var assignmentCode = IL(assigned);
                check(assignmentCode.Any(i => Call(i, "PickInFaction")) && assignmentCode.Any(i => Call(i, "PickProvider")) &&
                    !assignmentCode.Any(i => Call(i, "PickFaction") || Call(i, "PickFactionCore") || Call(i, "UseLocal")),
                    "fixed faction supports both local and replacement providers without faction or PNG chance draws");
                var routingCode = IL(compat.GetMethod("PickSelected", Static));
                check(routingCode.Any(i => Call(i, "get_FixedFaction")) && routingCode.Any(i => Call(i, "PickAssigned")) &&
                    routingCode.Any(i => Equals(i.operand, "MAIDEN_FACTION")) && routingCode.Any(i => Equals(i.operand, "MAIDEN_BODY")),
                    "factory forwards its actual fixed faction and drawn body into provider routing");
                foreach (var name in new[] { "PickPrimary", "PickFallback" })
                    check(IL(compat.GetMethod(name, Static)).Any(i => Call(i, "PickSelected")),
                        "both original selection paths respect assigned body provider: " + name);
            }

            // Unity's bundled MonoMod emitter cannot emit patches under this
            // net8 test host. Exercise repeated transpilation and inspect the
            // exact-method unpatch call; in-game Harmony emission remains a
            // separate Unity runtime verification.
            var factoryTranspiler = compat.GetMethod("FactoryTranspiler", Static);
            foreach (bool enabled in new[]{false, true, false, true})
            {
                flag.Set(enabled);
                var current = ((IEnumerable<CodeInstruction>)factoryTranspiler.Invoke(null, new object[]{IL(factory)})).ToList();
                check(SelectionHooks(current) == (enabled ? 0 : expectedHooks) &&
                    current.Count(i => Call(i, "PickFaction")) == (enabled ? 0 : 1),
                    "configuration flip regenerates the requested selection pipeline: " + enabled);
            }
            var reconfigure = IL(compat.GetMethod("ReconfigureSelection", Static));
            check(reconfigure.Any(i => i.operand is MethodInfo m && m.Name == "Unpatch" &&
                m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType == typeof(MethodInfo)) &&
                !reconfigure.Any(i => Call(i, "UnpatchAll")),
                "reconfiguration removes only the exact selection transpiler, preserving other patches");

            var injector = original.GetType("MidnightPawn.Game.SpriteInjector");
            var guest = hd.GetType("ItemPortraitHD.GuestPortraits");
            foreach (var spec in new[]{new[]{"InjectPortraitFromName", "CaptureSource", "GetSprite"},
                new[]{"InjectPortraitTrimmed", "CaptureTrimmed", "ResolveSprite"}})
            {
                var before = IL(injector.GetMethod(spec[0], Static));
                var rewritten = ((IEnumerable<CodeInstruction>)guest.GetMethod(spec[1], Static)
                    .Invoke(null, new object[]{IL(injector.GetMethod(spec[0], Static))})).ToList();
                check(rewritten.Count == before.Count + 3 && rewritten.Count(i => Call(i, "RenderSelected")) == 1 &&
                    rewritten.Count(i => Call(i, "MakeSelected")) == 1 && !rewritten.Any(i => Call(i, "RenderTrimFit") || Call(i, "MakeSprite")),
                    "selected source and correctly scaled output are intercepted exactly once: " + spec[0]);
                check(rewritten.Count(i => Call(i, spec[2])) == before.Count(i => Call(i, spec[2])) &&
                    rewritten.FindIndex(i => Call(i, "Remember")) < rewritten.FindIndex(i => Call(i, "RenderSelected")),
                    "original resolver including unknown replacement output is preserved before HD rendering: " + spec[0]);
                var replacement = (MethodInfo)rewritten.First(i => Call(i, "RenderSelected")).operand;
                var originalRender = (MethodInfo)before.First(i => Call(i, "RenderTrimFit")).operand;
                check(replacement.ReturnType == originalRender.ReturnType && replacement.GetParameters().Select(p => p.ParameterType)
                    .SequenceEqual(originalRender.GetParameters().Select(p => p.ParameterType)),
                    "render boundary replacement preserves IL stack signature: " + spec[0]);
            }
            var rewrite = selected.GetMethod("Rewrite", Static);
            bool rejected = false;
            try { ((IEnumerable<CodeInstruction>)rewrite.Invoke(null, new object[]{new[]{new CodeInstruction(OpCodes.Ret)}})).ToList(); }
            catch (InvalidOperationException) { rejected = true; }
            check(rejected, "changed dependency render layout is rejected instead of silently producing oversized portraits");
            string PathFor(string source, bool trim) => (string)selected.GetMethod("CachePath", Static).Invoke(null, new object[]{source,trim});
            check(PathFor("provider:Portrait A", true) == "maiden_provider_Portrait_A" &&
                PathFor("provider:Portrait A", false) == "maiden_provider_PortraitA",
                "legacy cache invalidation follows both original injection path conventions");
            var render = IL(selected.GetMethod("RenderSelected", Static));
            check(render.Count(i => Call(i, "Get") && ((MethodInfo)i.operand).DeclaringType?.Name == "Portraits") == 1 &&
                !render.Any(i => i.operand is MethodInfo m && (m.DeclaringType?.Name == "Catalog" || m.DeclaringType?.Name == "RandomPortraits")),
                "unknown selected sprites use shared render/cache without provider recognition or random draws");
            check(render.FindIndex(i => Call(i, "Owns")) < render.FindIndex(i => Call(i, "Get")) && render.Any(i =>
                i.opcode == OpCodes.Callvirt && i.operand is MethodInfo m && m.Name == "set_Item"),
                "already-normalized sprites are reused and tracked until original Sprite construction");
            var make = IL(selected.GetMethod("MakeSelected", Static));
            check(make.Any(i => Call(i, "TryGetValue")) && make.Any(i => Call(i, "Remove")) && !make.Any(i => Call(i, "Create")),
                "original injection receives the exact normalized sprite, avoiding wrong PPU or an extra render");
            var portraits = hd.GetType("ItemPortraitHD.Portraits");
            var display = IL(portraits.GetMethod("OriginalFor", Static));
            int catalogScan = display.FindIndex(i => Call(i, "Scan"));
            int exactIndex = display.FindIndex(i => Call(i, "For") && ((MethodInfo)i.operand).DeclaringType == selected);
            int observedIndex = display.FindIndex(i => Call(i, "Observed"));
            check(exactIndex >= 0 && exactIndex < catalogScan && observedIndex >= 0 && observedIndex < catalogScan,
                "unmodified item display bypasses catalog inference in compatibility mode");
            var rebuild = IL(selected.GetMethod("For", Static));
            check(!rebuild.Any(i => i.operand is MethodInfo m && m.DeclaringType?.Name == "Catalog") &&
                rebuild.Any(i => i.operand is FieldInfo f && f.Name == "restoreAttempts"),
                "save-load reconstruction uses saved original source and limits repeated failed restore attempts");
            check(Footprint.IsSquare(Footprint.Square()), "full 4x4 item footprint remains enabled independently of portrait selection");
            Console.WriteLine("Original-selection compatibility regressions complete.");
        }
        finally
        {
            hd.GetType("ItemPortraitHD.ModAccess").GetMethod("Clear", Static).Invoke(null, null);
            instanceField.SetValue(null, oldEntry);
            registered.Clear(); registered.AddRange(previous);
        }
    }
}
