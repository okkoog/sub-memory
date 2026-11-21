using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using ItemPortraitHD;

static class NameCompatibilityRegression
{
    internal static void Run(string dependencyPath, Action<bool, string> check)
    {
        var original = Assembly.LoadFrom(dependencyPath);
        var names = typeof(ModEntry).Assembly.GetType("ItemPortraitHD.NameCompatibility");
        var transpiler = names.GetMethod("GuestNames", BindingFlags.Static | BindingFlags.NonPublic);
        var writers = new List<string>();
        foreach (var type in original.GetTypes())
        foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
        {
            if (method.GetMethodBody() == null) continue;
            var input = PatchProcessor.GetOriginalInstructions(method, (ILGenerator)null);
            bool WritesName(CodeInstruction i) => i.opcode == OpCodes.Stfld &&
                i.operand is FieldInfo f && f.DeclaringType == typeof(StoreClient) && f.Name == "displayName";
            if (!input.Any(WritesName)) continue;
            string key = type.FullName + "." + method.Name;
            writers.Add(key);
            var output = ((IEnumerable<CodeInstruction>)transpiler.Invoke(null, new object[]{input})).ToList();
            check(!output.Any(WritesName) && output.Count(i => i.operand is MethodInfo m &&
                m.DeclaringType == names && m.Name == "ApplyName") == 1,
                "guest name assignment guarded: " + key);
        }
        check(writers.OrderBy(s => s).SequenceEqual(new[]{
            "MidnightPawn.Game.ClientPool.ApplyTo", "MidnightPawn.Game.Perks.OnNewDay"}),
            "all dependency customer rename paths are covered");
        var install = typeof(ModEntry).Assembly.GetType("ItemPortraitHD.OriginalCompat")
            .GetMethod("Install", BindingFlags.Static | BindingFlags.NonPublic);
        var installCode = PatchProcessor.GetOriginalInstructions(install, (ILGenerator)null);
        check(installCode.Any(i => Equals(i.operand, "Perks")) &&
            installCode.Any(i => Equals(i.operand, "OnNewDay")),
            "origin daily visitor rename patch is registered");
        CheckSettings(names, check);
    }

    private static void CheckSettings(Type names, Action<bool, string> check)
    {
        const BindingFlags fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        const BindingFlags statics = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var instance = typeof(ModEntry).GetField("Instance", statics);
        object previous = instance.GetValue(null);
        var entry = new ModEntry();
        ConfigEntry<bool> Flag() => (ConfigEntry<bool>)Activator.CreateInstance(typeof(ConfigEntry<bool>), fields, null, new object[] { false }, null);
        var vanilla = Flag(); var mod = Flag();
        typeof(ModEntry).GetField("KeepGuestNames", fields).SetValue(entry, vanilla);
        typeof(ModEntry).GetField("KeepModGuestNames", fields).SetValue(entry, mod);
        var active = typeof(ModEntry).GetField("Active", fields);
        var apply = (Action<StoreClient, string>)names.GetMethod("ApplyName", statics).CreateDelegate(typeof(Action<StoreClient, string>));
        StoreClient Client(string source) => new StoreClient {
            sourceModId = source, displayName = "original name", identifier = "custom_customer",
            spriteName = "portrait_pack:custom", possibleSprites = new List<string> { "portrait_pack:custom" }
        };
        bool Renamed(string source)
        {
            var client = Client(source);
            var sprites = client.possibleSprites;
            apply(client, "pool name");
            if (client.identifier != "custom_customer" || client.spriteName != "portrait_pack:custom" ||
                client.sourceModId != source || !ReferenceEquals(sprites, client.possibleSprites))
                throw new Exception("name protection modified unrelated customer data");
            return client.displayName == "pool name";
        }
        try
        {
            instance.SetValue(null, entry); active.SetValue(entry, true);
            check(Renamed(null) && Renamed("another_mod") && Renamed("lewd_meme"),
                "both name protections default off and preserve dependency renaming");
            mod.Set(true);
            check(!Renamed("another_mod") && !Renamed("unknown_unlisted_mod") && !Renamed("lewd_meme_remake"),
                "mod-only protection covers any declared mod customer without prerequisites");
            check(Renamed(null) && Renamed("") && Renamed(" ") && !Renamed("lewd_meme") && !Renamed("LEWD_MEME"),
                "mod-only protection includes dependency customers and leaves base customers with mod portraits unprotected");
            vanilla.Set(true); mod.Set(false);
            check(!Renamed(null) && !Renamed("") && !Renamed(" ") && Renamed("another_mod") && Renamed("lewd_meme"),
                "vanilla-only protection excludes every mod customer, including dependency customers");
            mod.Set(true);
            check(!Renamed(null) && !Renamed("another_mod"), "both options can be enabled together");
            active.SetValue(entry, false);
            check(Renamed(null) && Renamed("another_mod"), "disabled manager does not block dependency renaming");
            active.SetValue(entry, true); vanilla.Set(false); mod.Set(false);
            check(Renamed("another_mod"), "switching protection off takes effect without reinstalling patches");
            foreach (bool keepVanilla in new[] { false, true })
            foreach (bool keepMod in new[] { false, true })
            {
                vanilla.Set(keepVanilla); mod.Set(keepMod);
                check(new[] { null, "", " " }.All(source => Renamed(source) == !keepVanilla) &&
                    new[] { "lewd_meme", "another_mod", "unknown_unlisted_mod" }.All(source => Renamed(source) == !keepMod),
                    $"independent name protection matrix: vanilla={keepVanilla}, mod={keepMod}");
            }
            apply(null, "pool name");
            check(true, "missing customer is harmless at the name-assignment boundary");
        }
        finally { instance.SetValue(null, previous); }

        var config = PatchProcessor.GetOriginalInstructions(typeof(ModEntry).GetMethod("Init"), (ILGenerator)null);
        int oldSetting = config.FindIndex(i => Equals(i.operand, "keep_guest_names"));
        int newSetting = config.FindIndex(i => Equals(i.operand, "keep_mod_guest_names"));
        int regionSetting = config.FindIndex(i => Equals(i.operand, "name_region"));
        check(oldSetting >= 0 && newSetting > oldSetting && newSetting < regionSetting &&
            config[newSetting + 1].opcode == OpCodes.Ldc_I4_0,
            "mod name option defaults off next to the independent vanilla name switch");
    }
}
