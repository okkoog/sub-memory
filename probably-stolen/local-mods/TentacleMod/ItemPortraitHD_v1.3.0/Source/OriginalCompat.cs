using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace ItemPortraitHD
{
    // These patches are installed after the dependency is loaded. The original DLL
    // is never changed and is not needed to compile this project.
    internal static class OriginalCompat
    {
        private const string PatchId = ModEntry.Id + ".original_compat";
        private static Harmony harmony;
        private static MethodBase factory;
        private static Func<string, string, string> identitySprite;
        private static Func<string> anyPortrait;
        private static Func<string, int, int, string> identityInject, trimmedInject;
        [ThreadStatic] private static int fixedFactionDepth;
        internal static bool FixedFaction => fixedFactionDepth > 0;
        private static bool installed;
        private static bool Enabled => ModEntry.Instance?.Active == true &&
            ModLoader.Mods.Any(m => m.IsEnabled && m.Manifest?.ID == "lewd_meme" && m.Assembly?.GetName().Name == "MidnightPawn");

        internal static void Install()
        {
            if (installed || !Enabled || !ModAccess.Ready) return;
            harmony = new Harmony(PatchId);
            const string injector = "SpriteInjector";
            var trimPrefix = Hook(typeof(OriginalCompat), nameof(InjectPrefix));
            bool trimReady = InstallPatch(injector, "InjectPortraitTrimmed", prefix: trimPrefix);
            if (trimReady)
            {
                TryInstall("生成立绘", () =>
                {
                    var target = ResolveFactory();
                    bool hasFaction = target.Name == "CreateMaidenOfFaction";
                    if (hasFaction) BindFactorySelection();
                    Patch(target,
                        prefix: hasFaction ? Hook(typeof(OriginalCompat), nameof(BeginFactory)) : null,
                        postfix: Hook(typeof(OriginalCompat), nameof(Created)),
                        transpiler: Hook(typeof(OriginalCompat), nameof(FactoryTranspiler)),
                        finalizer: hasFaction ? Hook(typeof(OriginalCompat), nameof(EndFactory)) : null);
                    factory = target;
                });
            }

            InstallPatch(injector, "ExportInjected", prefix: Hook(typeof(OriginalCompat), nameof(ExportPrefix)));
            InstallPatch("TentacleEffects", "TryApplySemen", transpiler: Hook(typeof(OriginalCompat), nameof(ToolTranspiler)));
            InstallPatch("GenderRatio", "OnGenerated", prefix: Hook(typeof(OriginalCompat), nameof(ConvertPrefix)));
            InstallPatch("ClientPool", "ApplyTo", transpiler: Hook(typeof(NameCompatibility), nameof(NameCompatibility.GuestNames)));
            InstallPatch("Perks", "OnNewDay", transpiler: Hook(typeof(NameCompatibility), nameof(NameCompatibility.GuestNames)));
            InstallPatch("NameGen", "RandomName", transpiler: Hook(typeof(NameCompatibility), nameof(NameCompatibility.RegionalNames)));
            InstallPatch("GuestBox", "BuildGuestItem", prefix: Hook(typeof(GuestPortraits), nameof(GuestPortraits.Begin)),
                transpiler: Hook(typeof(GuestPortraits), nameof(GuestPortraits.MarkItem)),
                finalizer: Hook(typeof(GuestPortraits), nameof(GuestPortraits.End)));
            if (TryInstall("原模组选图重建", () => OriginalSelectedPortraits.Bind(OriginalType(injector))))
            {
                InstallPatch(injector, "InjectPortraitFromName",
                    prefix: Hook(typeof(OriginalSelectedPortraits), nameof(OriginalSelectedPortraits.PrepareFromName)),
                    transpiler: Hook(typeof(GuestPortraits), nameof(GuestPortraits.CaptureSource)));
                InstallPatch(injector, "InjectPortraitTrimmed",
                    transpiler: Hook(typeof(GuestPortraits), nameof(GuestPortraits.CaptureTrimmed)));
            }
            installed = true;
        }

        private static Type OriginalType(string name) => ModAccess.Type("lewd_meme", "MidnightPawn.Game." + name);
        private static HarmonyMethod Hook(Type type, string name) => new HarmonyMethod(type, name);

        internal static MethodBase ResolveFactory()
        {
            var type = OriginalType("ItemFactory");
            try { return ModReflect.Target(type, "CreateMaidenOfFaction", new[] { typeof(string) }); }
            catch (ModReflectAccessException) { return ModReflect.Target(type, "CreateMaiden", Type.EmptyTypes); }
        }

        private static void BindFactorySelection()
        {
            var identity = OriginalType("MaidenIdentity");
            var sprites = OriginalType("SpriteInjector");
            identitySprite = ModReflect.StaticMethod<Func<string, string, string>>(identity, "SpriteFor");
            identityInject = ModReflect.StaticMethod<Func<string, int, int, string>>(identity, "InjectSprite");
            anyPortrait = ModReflect.StaticMethod<Func<string>>(sprites, "PickAnyPortraitSource");
            trimmedInject = ModReflect.StaticMethod<Func<string, int, int, string>>(sprites, "InjectPortraitTrimmed");
        }

        private static bool TryInstall(string name, Action action)
        {
            try { action(); return true; }
            catch (Exception ex)
            {
                ModEntry.Warn(new InvalidOperationException("lewd_meme " + name + " 补丁未安装：" + ex.Message));
                return false;
            }
        }

        private static bool InstallPatch(string type, string method, HarmonyMethod prefix = null, HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null, HarmonyMethod finalizer = null) =>
            TryInstall(type + "." + method,
                () => Patch(ModReflect.Target(OriginalType(type), method), prefix, postfix, transpiler, finalizer));

        private static void Patch(MethodBase target, HarmonyMethod prefix = null, HarmonyMethod postfix = null,
            HarmonyMethod transpiler = null, HarmonyMethod finalizer = null)
        {
            try { harmony.Patch(target, prefix, postfix, transpiler, finalizer); }
            catch
            {
                // Harmony may register the patch before emitting its wrapper. Remove only
                // this attempt's methods, including when another hook shares this target.
                foreach (var hook in new[] { transpiler, prefix, postfix, finalizer })
                    if (hook?.method != null) harmony.Unpatch(target, hook.method);
                throw;
            }
        }

        internal static void Remove()
        {
            harmony?.UnpatchAll(PatchId);
            OriginalSelectedPortraits.Clear();
            factory = null;
            identitySprite = null; identityInject = trimmedInject = null; anyPortrait = null;
            installed = false;
        }

        internal static void ReconfigureSelection()
        {
            if (factory == null) return;
            try
            {
                var transpiler = AccessTools.Method(typeof(OriginalCompat), nameof(FactoryTranspiler));
                harmony.Unpatch(factory, transpiler);
                Patch(factory, transpiler: new HarmonyMethod(transpiler));
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }

        private static bool ConvertPrefix() => !Enabled || ModEntry.Instance.CustomerConversion.Value != "保留游戏原分布";

        // ExportInjected is a diagnostic PNG dump, not item persistence. Avoid
        // encoding HD textures and then failing on an iph|... filename each capture.
        private static bool ExportPrefix(string __0) => !Enabled || __0 == null ||
            (!__0.StartsWith("iph_hd_", StringComparison.Ordinal) && !OriginalSelectedPortraits.IsRebuiltPath(__0));

        private static string Publish(Sprite sprite, string key)
        {
            var originalMod = ModLoader.Mods.First(m => m.IsEnabled && m.Manifest?.ID == "lewd_meme");
            if (originalMod.Sprites == null) originalMod.Sprites = new Dictionary<string, Sprite>();
            uint hash = 2166136261;
            foreach (char c in key) hash = unchecked((hash ^ c) * 16777619);
            string id = "iph_hd_" + hash.ToString("x8") + "_" + ModEntry.Size;
            originalMod.Sprites[id] = sprite;
            return id;
        }

        internal static IEnumerable<CodeInstruction> FactoryTranspiler(IEnumerable<CodeInstruction> instructions)
            => TransformFactory(instructions, ModEntry.UseOriginalPortraitSelection);

        internal static IEnumerable<CodeInstruction> TransformFactory(IEnumerable<CodeInstruction> instructions, bool originalSelection)
        {
            var ordered = instructions.ToList();
            int bodyStart = ordered.FindIndex(i => i.operand is FieldInfo f && f.Name == "BODIES" &&
                f.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits");
            int factionStart = ordered.FindIndex(i => i.operand is FieldInfo f && f.Name == "FACTIONS" &&
                f.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits");
            int breastStart = ordered.FindIndex(i => i.operand is FieldInfo f && f.Name == "BREASTS" &&
                f.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits");
            if (bodyStart < 0 || factionStart < 0 || breastStart < 0 || bodyStart + 2 >= ordered.Count ||
                !(ordered[bodyStart + 1].operand is MethodInfo bp) || bp.Name != "Pick" ||
                bp.DeclaringType?.FullName != "MidnightPawn.Game.MaidenTraits")
                throw new InvalidOperationException("体型/阵营抽取顺序接口不匹配。");
            var bodyBlock = ordered.GetRange(bodyStart, 3);
            bool hasFixedFaction = factionStart >= 3 && ordered[factionStart - 3].opcode == OpCodes.Ldarg_0 &&
                ordered[factionStart - 2].Calls(AccessTools.Method(typeof(string), nameof(string.IsNullOrEmpty))) &&
                (ordered[factionStart - 1].opcode == OpCodes.Brfalse || ordered[factionStart - 1].opcode == OpCodes.Brfalse_S);
            int insertAt = originalSelection ? breastStart : Math.Min(breastStart, hasFixedFaction ? factionStart - 3 : factionStart);
            if (bodyStart > insertAt)
            {
                if (bodyBlock.Any(HasControlMetadata) || HasControlMetadata(ordered[insertAt]))
                    throw new InvalidOperationException("体型抽取跨越分支，未修改生成逻辑。");
                ordered.RemoveRange(bodyStart, 3);
                // The explicit-faction branch skips the random faction draw. When moving
                // before that draw, insert before its guard so both branches store the body.
                ordered.InsertRange(insertAt, bodyBlock);
            }
            var result = new List<CodeInstruction>();
            int replaced = 0;
            int factionReplaced = 0;
            bool factionArray = false;
            bool bodyArray = false;
            bool breastArray = false;
            int bodyReplaced = 0;
            int breastReplaced = 0;
            int identityReplaced = 0;
            foreach (var instruction in ordered)
            {
                if (instruction.operand is FieldInfo field && field.Name == "FACTIONS" &&
                    field.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits") factionArray = true;
                if (instruction.operand is FieldInfo bodyField && bodyField.Name == "BODIES" &&
                    bodyField.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits") bodyArray = true;
                if (instruction.operand is FieldInfo breastField && breastField.Name == "BREASTS" &&
                    breastField.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits") breastArray = true;
                if (bodyArray && instruction.operand is MethodInfo bodyPick && bodyPick.Name == "Pick" &&
                    bodyPick.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits")
                {
                    instruction.operand = AccessTools.Method(typeof(AdultBodySources), nameof(AdultBodySources.PickBody));
                    bodyArray = false; bodyReplaced++;
                }
                if (!originalSelection && factionArray && instruction.operand is MethodInfo pick && pick.Name == "Pick" &&
                    pick.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits")
                {
                    result.Add(LoadStoredLocal(bodyBlock[2]));
                    instruction.operand = AccessTools.Method(typeof(RandomPortraits), nameof(RandomPortraits.PickFaction));
                    factionArray = false;
                    factionReplaced++;
                }
                if (!originalSelection && instruction.operand is MethodInfo identity && identity.DeclaringType?.FullName == "MidnightPawn.Game.MaidenIdentity")
                {
                    if (identity.Name == "SpriteFor")
                    {
                        if (hasFixedFaction)
                        {
                            AppendItemArgument(result, instruction, nameof(PickPrimary));
                            identityReplaced++;
                            continue;
                        }
                        instruction.opcode = OpCodes.Pop; instruction.operand = null;
                        result.Add(instruction);
                        result.Add(new CodeInstruction(OpCodes.Pop));
                        result.Add(new CodeInstruction(OpCodes.Ldloc_0));
                        result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OriginalCompat), nameof(PickForItem))));
                        identityReplaced++;
                        continue;
                    }
                    if (identity.Name == "InjectSprite")
                        instruction.operand = hasFixedFaction
                            ? AccessTools.Method(typeof(OriginalCompat), nameof(InjectIdentity))
                            : ModReflect.Target(OriginalType("SpriteInjector"), "InjectPortraitTrimmed");
                }
                if (breastArray && instruction.operand is MethodInfo breastPick && breastPick.Name == "Pick" &&
                    breastPick.DeclaringType?.FullName == "MidnightPawn.Game.MaidenTraits")
                {
                    result.Add(LoadStoredLocal(bodyBlock[2]));
                    instruction.operand = AccessTools.Method(typeof(AdultBodySources), nameof(AdultBodySources.PickBreastForBody));
                    breastArray = false;
                    breastReplaced++;
                }
                if (!originalSelection && instruction.operand is MethodInfo method && method.Name == "PickAnyPortraitSource" &&
                    method.DeclaringType?.FullName == "MidnightPawn.Game.SpriteInjector")
                {
                    AppendItemArgument(result, instruction, hasFixedFaction ? nameof(PickFallback) : nameof(PickForItem));
                    replaced++;
                }
                else result.Add(instruction);
            }
            if ((!originalSelection && (replaced != 1 || factionReplaced != 1 || identityReplaced > 1 ||
                (hasFixedFaction && identityReplaced != 1))) ||
                bodyReplaced != 1 || breastReplaced != 1) throw new InvalidOperationException("新物品选图/阵营/体型/乳量接口不匹配，未应用随机立绘修改。");
            return result;
        }

        private static void AppendItemArgument(List<CodeInstruction> result, CodeInstruction call, string replacement)
        {
            // Reuse the old call as the argument load so incoming branches still
            // execute the load before entering the replacement.
            call.opcode = OpCodes.Ldloc_0;
            call.operand = null;
            result.Add(call);
            result.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(OriginalCompat), replacement)));
        }

        private static CodeInstruction LoadStoredLocal(CodeInstruction store)
        {
            if (store.opcode == OpCodes.Stloc_0) return new CodeInstruction(OpCodes.Ldloc_0);
            if (store.opcode == OpCodes.Stloc_1) return new CodeInstruction(OpCodes.Ldloc_1);
            if (store.opcode == OpCodes.Stloc_2) return new CodeInstruction(OpCodes.Ldloc_2);
            if (store.opcode == OpCodes.Stloc_3) return new CodeInstruction(OpCodes.Ldloc_3);
            if (store.opcode == OpCodes.Stloc_S) return new CodeInstruction(OpCodes.Ldloc_S, store.operand);
            if (store.opcode == OpCodes.Stloc) return new CodeInstruction(OpCodes.Ldloc, store.operand);
            throw new InvalidOperationException("体型抽取结果没有写入预期局部变量。");
        }

        private static bool HasControlMetadata(CodeInstruction instruction) => instruction.blocks.Count != 0 ||
            instruction.labels.Count != 0;

        internal static string PickForItem(GameItem item) => RandomPortraits.Pick(item);

        private static void BeginFactory(string __0, out bool __state)
        {
            __state = Enabled && !string.IsNullOrEmpty(__0);
            if (__state) fixedFactionDepth++;
        }

        private static void EndFactory(bool __state)
        {
            if (__state) fixedFactionDepth--;
        }

        private static string PickPrimary(string faction, string job, GameItem item) =>
            PickSelected(item) ?? (ModEntry.UseFinalPortraitSelection ? null : identitySprite(faction, job));

        private static string PickFallback(GameItem item) =>
            PickSelected(item) ?? (ModEntry.UseFinalPortraitSelection ? null : anyPortrait());

        private static string PickSelected(GameItem item)
        {
            if (!Enabled || item == null) return null;
            return FixedFaction
                ? RandomPortraits.PickAssigned(ModEntry.Get(item, "MAIDEN_FACTION"), ModEntry.Get(item, "MAIDEN_BODY"))
                : PickForItem(item);
        }

        private static string InjectIdentity(string source, int width, int height) =>
            ModEntry.UseFinalPortraitSelection || source?.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal) == true
                ? trimmedInject(source, width, height)
                : identityInject(source, width, height);

        private static void Created(GameItem __result)
        {
            if (!Enabled || __result == null) return;
            string body = ModEntry.Get(__result, "MAIDEN_BODY");
            if (!FixedFaction && AdultBodySources.IsAdultBody(body))
            {
                __result.DisableTag("MAIDEN_VIRGIN", false);
                __result.DisableTag("MAIDEN_ANAL_VIRGIN", false);
                float chance = BodyCsvSettings.Chance(body);
                if (AdultBodySources.ShouldBeDoubleVirgin(body, chance, UnityEngine.Random.value))
                {
                    __result.ModifyTag("MAIDEN_VIRGIN", tag => tag.Enable(), false);
                    __result.ModifyTag("MAIDEN_ANAL_VIRGIN", tag => tag.Enable(), false);
                }
            }
            ShapeRules.Ensure(__result);
            if (__result is GameItemElement element) DisplaySizePatch.FitItem(element);
        }

        private static bool InjectPrefix(string __0, ref string __result)
        {
            if (!Enabled || string.IsNullOrWhiteSpace(__0)) return true;
            // The mode switch controls new selections, not the format of saved
            // selections. The dependency cannot resolve our persisted iph| IDs.
            if (UseDependencyResolver(__0))
            {
                OriginalSelectedPortraits.PrepareTrimmed(__0);
                return true;
            }
            if (GuestPortraits.Building) return true;
            try
            {
                // Publish the full source at the configured resolution before the
                // original factory assigns its sprite. No 64-pixel intermediate.
                var sprite = Portraits.Get(Catalog.Original(__0));
                if (!sprite) return !__0.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal);
                __result = Publish(sprite, __0);
                return false;
            }
            catch (Exception ex) { ModEntry.Warn(ex); return !__0.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal); }
        }

        internal static bool UseDependencyResolver(string source) =>
            (ModEntry.UseOriginalPortraitSelection || FixedFaction) &&
            !source.StartsWith(RandomPortraits.Prefix, StringComparison.Ordinal);

        internal static IEnumerable<CodeInstruction> ToolTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var original = AccessTools.Method(typeof(LocGenderHelper), nameof(LocGenderHelper.GetCurrentClientGender));
            var replacement = AccessTools.Method(typeof(OriginalCompat), nameof(ToolGender));
            int replaced = 0;
            var result = instructions.ToList();
            foreach (var instruction in result)
                if (instruction.Calls(original))
                {
                    // Only replace this call inside the original mod's tool handler;
                    // labels, exception blocks and the rest of its checks stay intact.
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = replacement;
                    replaced++;
                }
            if (replaced != 1) throw new InvalidOperationException("道具性别检查接口不匹配，保留原版判定。");
            return result;
        }

        internal static LocGenderHelper.LocGender ToolGender()
        {
            if (Enabled && ModEntry.Instance.ToolGender.Value == "不限性别")
                return (LocGenderHelper.LocGender)2;
            return LocGenderHelper.GetCurrentClientGender();
        }
    }
}
