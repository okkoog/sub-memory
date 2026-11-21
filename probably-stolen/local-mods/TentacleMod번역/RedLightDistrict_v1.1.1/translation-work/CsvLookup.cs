using System.Collections.Generic;
internal static class CsvLookup {
    private static LoadedMod owner;
    internal static string Read(string key, string original) {
        if (owner == null) {
            var mods = ModLoader.Mods;
            if (mods == null) return original;
            for (int i = 0; i < mods.Count; i++) {
                var mod = mods[i];
                if (mod != null && mod.Manifest != null && mod.Manifest.ID == "red_light_district" && mod.Assembly == typeof(CsvLookup).Assembly) {
                    owner = mod;
                    break;
                }
            }
        }
        Dictionary<string,string> korean;
        string value;
        if (owner != null && owner.Localization != null && owner.Localization.TryGetValue("ko", out korean)
            && korean != null && korean.TryGetValue(key, out value) && !string.IsNullOrEmpty(value)) return value;
        return original;
    }
}
