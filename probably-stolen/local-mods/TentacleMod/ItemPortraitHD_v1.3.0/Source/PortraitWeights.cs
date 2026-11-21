using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ItemPortraitHD
{
    internal static class PortraitWeights
    {
        private static readonly Dictionary<string, float> values = new Dictionary<string, float>(StringComparer.Ordinal);
        private static string path;
        internal static int Revision { get; private set; }
        internal static void Load()
        {
            path = Path.Combine(ModHelper.GetModDataFolder(ModEntry.Id), "PortraitWeights.txt");
            string legacy = Path.Combine(ModEntry.Instance.Folder, "PortraitWeights.txt");
            try
            {
                if (!File.Exists(path) && File.Exists(legacy))
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.Copy(legacy, path);
                }
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
            values.Clear();
            Revision++;
            string readPath = File.Exists(path) ? path : legacy;
            if (!File.Exists(readPath)) return;
            try
            {
                foreach (string line in File.ReadAllLines(readPath))
                {
                    int tab = line.LastIndexOf('\t');
                    if (tab < 1 || line.StartsWith("#", StringComparison.Ordinal)) continue;
                    if (float.TryParse(line.Substring(tab + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out float value) &&
                        !float.IsNaN(value) && !float.IsInfinity(value)) values[line.Substring(0, tab)] = Math.Max(0, Math.Min(10, value));
                }
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }
        internal static float Get(string id) => values.TryGetValue(id, out float value) ? value : 1;
        internal static void Set(string id, float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return;
            value = Math.Max(0, Math.Min(10, value));
            if (Get(id) == value) return;
            values[id] = value;
            Revision++;
        }
        internal static void Save()
        {
            try
            {
                if (path == null) return;
                System.IO.Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllLines(path, new[] { "# source-id<TAB>weight (0..10); default 1; 0 excludes from random draws" }
                    .Concat(values.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => p.Key + "\t" + p.Value.ToString(CultureInfo.InvariantCulture))));
            }
            catch (Exception ex) { ModEntry.Warn(ex); }
        }
    }
}
