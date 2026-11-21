using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

static class CacheRegression
{
    const BindingFlags Static = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    internal static void Run(Action<bool, string> check)
    {
        var hd = typeof(ItemPortraitHD.ModEntry).Assembly;
        var portraits = hd.GetType("ItemPortraitHD.Portraits");
        // Managed identities only: no Unity texture allocation or rendering is emulated.
        var before = (Sprite)RuntimeHelpers.GetUninitializedObject(typeof(Sprite));
        var after = (Sprite)RuntimeHelpers.GetUninitializedObject(typeof(Sprite));
        var other = (Sprite)RuntimeHelpers.GetUninitializedObject(typeof(Sprite));
        var registry = new Dictionary<string, Sprite> { ["saved-path"] = before, ["alias"] = before, ["overridden"] = other, ["empty"] = null };
        portraits.GetMethod("Rebind", Static).Invoke(null, new object[] { registry, before, after });
        check(ReferenceEquals(registry["saved-path"], after) && ReferenceEquals(registry["alias"], after) &&
            registry.Count == 4, "rebuilding a portrait updates all existing aliases while preserving saved keys");
        check(ReferenceEquals(registry["overridden"], other) && registry["empty"] is null,
            "cache rebinding preserves another provider's replacement and empty entries");
        portraits.GetMethod("Rebind", Static).Invoke(null, new object[] { registry, before, other });
        check(ReferenceEquals(registry["saved-path"], after), "stale regeneration cannot overwrite a newer registration");
        var clear = PatchProcessor.GetOriginalInstructions(portraits.GetMethod("Clear", Static), (ILGenerator)null);
        var collect = PatchProcessor.GetOriginalInstructions(portraits.GetMethod("CollectRetired", Static), (ILGenerator)null);
        check(!clear.Any(i => i.operand is MethodInfo m && m.Name == "Release") &&
            collect.FindIndex(i => i.operand is MethodInfo m && m.Name == "Referenced") <
            collect.FindIndex(i => i.operand is MethodInfo m && m.Name == "Release"),
            "cache invalidation defers destruction and collection checks current external references");
        var get = PatchProcessor.GetOriginalInstructions(portraits.GetMethod("Get", Static), (ILGenerator)null);
        check(get.Count(i => i.operand is MethodInfo m && m.Name == "RebindPrevious") == 2,
            "both shared conversions and new renders update retired external registrations");
        var random = hd.GetType("ItemPortraitHD.RandomPortraits");
        var selected = (IDictionary)random.GetField("selected", Static).GetValue(null);
        var saved = selected.Cast<DictionaryEntry>().ToArray();
        var sourceType = hd.GetType("ItemPortraitHD.Source");
        object Source(string id) {
            var source = Activator.CreateInstance(sourceType, true);
            sourceType.GetField("Id", Instance).SetValue(source, id);
            return source;
        }
        try
        {
            var frozen = Source("final:cached-role");
            selected["final:cached-role"] = frozen;
            selected["mod:old:role"] = Source("mod:old:role");
            random.GetMethod("InvalidateSources", Static).Invoke(null, null);
            check(ReferenceEquals(selected["final:cached-role"], frozen) && !selected.Contains("mod:old:role"),
                "source reload expires provider descriptors without redrawing frozen final selections");
        }
        finally { selected.Clear(); foreach (var entry in saved) selected.Add(entry.Key, entry.Value); }
        var mappedField = random.GetField("mappedFiles", Static);
        var savedMapping = mappedField.GetValue(null);
        const string mappedId = "mapped:regression:portrait.png";
        try
        {
            var mapped = (IDictionary)Activator.CreateInstance(mappedField.FieldType);
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(sourceType));
            var source = Source(mappedId); list.Add(source); mapped["femaleupper1"] = list;
            mappedField.SetValue(null, mapped);
            check(ReferenceEquals(source, random.GetMethod("Find", Static).Invoke(null, new object[] { "iph|" + mappedId })),
                "saved mapped portraits restore from role mappings without a manual-only catalog entry");
        }
        finally { mappedField.SetValue(null, savedMapping); selected.Remove(mappedId); }
        var replacements = (IDictionary)random.GetField("replacementCache", Static).GetValue(null);
        var poolFields = random.GetFields(Static).Where(f => f.Name.StartsWith("pool", StringComparison.Ordinal) ||
            f.Name == "roleTables" || f.Name == "factionTable").ToDictionary(f => f, f => f.GetValue(null));
        const string role = "femaleUpper_cache_test";
        var keys = new[] { role };
        try
        {
            replacements[role] = Activator.CreateInstance(typeof(List<>).MakeGenericType(sourceType));
            var ensure = random.GetMethod("EnsurePool", Static);
            var table = random.GetField("factionTable", Static);
            ensure.Invoke(null, new object[] { keys, false });
            var first = table.GetValue(null);
            ensure.Invoke(null, new object[] { keys, false });
            check(ReferenceEquals(first, table.GetValue(null)), "ordinary captures reuse unchanged weighted tables");
            random.GetMethod("InvalidateReplacement", Static).Invoke(null, new object[] { "unrelated-role" });
            ensure.Invoke(null, new object[] { keys, false });
            check(!ReferenceEquals(first, table.GetValue(null)), "provider invalidation rebuilds cached role weights");
        }
        finally
        {
            replacements.Remove(role);
            foreach (var field in poolFields) field.Key.SetValue(null, field.Value);
        }
    }
}

