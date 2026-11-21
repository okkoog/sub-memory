using System;
using System.Collections;
using System.Reflection;

namespace AllGirlsKoreanBountiesCompat
{
    // Called through the existing liquid-name fix lifecycle; no saved-item traversal.
    internal static class IngotPurityCompatibility
    {
        private const BindingFlags All = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string ReadyKey = "mech_condition_very_high_purity";

        internal static void Refresh()
        {
            try
            {
                if (!MoreGirlsEnabled()) return;
                Type loc = ResolveType("LocHelper");
                Type purity = ResolveType("IngotPurityHelper");
                if (loc == null || purity == null) return;

                MethodInfo translate = loc.GetMethod("GetLocalizedMechanic", All, null, new[] { typeof(string) }, null);
                if (translate == null || !Valid((string)translate.Invoke(null, new object[] { ReadyKey }), ReadyKey)) return;

                FieldInfo field = purity.GetField("ingotPurities", All);
                PropertyInfo property = field == null ? purity.GetProperty("ingotPurities", All) : null;
                object registry = field != null ? field.GetValue(null) : property != null ? property.GetValue(null, null) : null;
                if (registry == null) return;

                int changed = 0;
                IDictionary dict = registry as IDictionary;
                if (dict != null)
                {
                    ArrayList pending = new ArrayList();
                    foreach (DictionaryEntry entry in dict)
                    {
                        object entryValue = entry.Value;
                        int before = changed;
                        object newValue = FixValue(entryValue, translate, ref changed);
                        if (!ReferenceEquals(entryValue, newValue) && !Equals(entryValue, newValue))
                            pending.Add(new DictionaryEntry(entry.Key, newValue));
                        else if (entryValue != null && entryValue.GetType().IsValueType && changed > before)
                            pending.Add(new DictionaryEntry(entry.Key, newValue));
                    }
                    foreach (DictionaryEntry entry in pending) dict[entry.Key] = entry.Value;
                }
                else
                {
                    IList list = registry as IList;
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            object previous = list[i];
                            int before = changed;
                            object fixedValue = FixValue(previous, translate, ref changed);
                            if (!Equals(previous, fixedValue) || (previous != null && previous.GetType().IsValueType && changed > before))
                                list[i] = fixedValue;
                        }
                    }
                    else
                    {
                        IEnumerable sequence = registry as IEnumerable;
                        if (sequence != null) foreach (object item in sequence) FixValue(item, translate, ref changed);
                    }
                }
                if (changed != 0) Log("[AllGirlsCompat] Repaired " + changed + " shared ingot purity labels after localization (v1.0.4).");
            }
            catch (Exception ex)
            {
                Log("[AllGirlsCompat] Ingot purity refresh skipped: " + ex.GetBaseException().Message);
            }
        }

        private static bool MoreGirlsEnabled()
        {
            Type loader = ResolveType("ModLoader");
            if (loader == null) return false;
            PropertyInfo property = loader.GetProperty("Mods", All);
            FieldInfo field = property == null ? loader.GetField("Mods", All) : null;
            IEnumerable mods = (property != null ? property.GetValue(null, null) : field != null ? field.GetValue(null) : null) as IEnumerable;
            if (mods == null) return false;
            foreach (object mod in mods)
            {
                if (mod == null) continue;
                object enabled = Member(mod, "IsEnabled");
                object manifest = Member(mod, "Manifest");
                if (enabled is bool && (bool)enabled && manifest != null && (string)Member(manifest, "ID") == "more_girls")
                    return true;
            }
            return false;
        }

        private static object Member(object item, string name)
        {
            PropertyInfo property = item.GetType().GetProperty(name, All);
            if (property != null && property.GetIndexParameters().Length == 0) return property.GetValue(item, null);
            FieldInfo field = item.GetType().GetField(name, All);
            return field == null ? null : field.GetValue(item);
        }

        private static Type ResolveType(string name)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(name, false);
                if (type != null) return type;
            }
            return null;
        }

        private static bool Valid(string s, string key)
        {
            return !string.IsNullOrWhiteSpace(s) && s != key &&
                !s.StartsWith("Translation Error", StringComparison.Ordinal) &&
                !s.StartsWith("No translation found", StringComparison.OrdinalIgnoreCase);
        }

        private static string FindPurityKey(string error)
        {
            if (error == null) return null;
            const string beginning = "Translation Error '";
            const string ending = "' in Mechanic";
            if (!error.StartsWith(beginning, StringComparison.Ordinal) ||
                !error.EndsWith(ending, StringComparison.Ordinal)) return null;
            string key = error.Substring(beginning.Length, error.Length - beginning.Length - ending.Length);
            return key.StartsWith("mech_condition_", StringComparison.Ordinal) &&
                key.EndsWith("_purity", StringComparison.Ordinal) ? key : null;
        }

        private static object FixValue(object entry, MethodInfo translate, ref int changed)
        {
            string text = entry as string;
            if (text != null)
            {
                string key = FindPurityKey(text);
                if (key != null)
                {
                    string translated = translate.Invoke(null, new object[] { key }) as string;
                    if (Valid(translated, key) && translated != text) { changed++; return translated; }
                }
                return entry;
            }
            if (entry == null) return null;
            Type type = entry.GetType();
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.FieldType != typeof(string) || field.IsInitOnly) continue;
                try
                {
                    string original = (string)field.GetValue(entry);
                    string key = FindPurityKey(original);
                    if (key == null) continue;
                    string translated = translate.Invoke(null, new object[] { key }) as string;
                    if (!Valid(translated, key) || translated == original) continue;
                    field.SetValue(entry, translated);
                    changed++;
                }
                catch (Exception) { }
            }
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (property.PropertyType != typeof(string) || !property.CanRead || !property.CanWrite || property.GetIndexParameters().Length != 0) continue;
                try
                {
                    string original = (string)property.GetValue(entry, null);
                    string key = FindPurityKey(original);
                    if (key == null) continue;
                    string translated = translate.Invoke(null, new object[] { key }) as string;
                    if (!Valid(translated, key) || translated == original) continue;
                    property.SetValue(entry, translated, null);
                    changed++;
                }
                catch (Exception) { }
            }
            return entry;
        }

        private static void Log(string text)
        {
            try
            {
                Type debug = ResolveType("UnityEngine.Debug");
                MethodInfo method = debug == null ? null : debug.GetMethod("Log", All, null, new[] { typeof(object) }, null);
                if (method != null) method.Invoke(null, new object[] { text });
            }
            catch { }
        }
    }
}
