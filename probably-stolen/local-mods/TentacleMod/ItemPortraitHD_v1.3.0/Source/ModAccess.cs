using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemPortraitHD
{
    // Resolve optional providers through the game's checked reflection scope.
    // Selecting by mod ID also disambiguates the original and remake's type names.
    internal static class ModAccess
    {
        private static readonly Dictionary<string, Dictionary<string, Type>> types =
            new Dictionary<string, Dictionary<string, Type>>(StringComparer.Ordinal);

        internal static bool Ready => ModLoader.Mods.Any(m => m.IsEnabled && m.Manifest?.ID == ModEntry.Id &&
            m.Assembly == typeof(ModEntry).Assembly);

        internal static bool Allowed(string modId)
        {
            if (!Ready) return false;
            var self = ModLoader.Mods.FirstOrDefault(m => m.Manifest?.ID == ModEntry.Id);
            return modId == ModEntry.Id || self?.Manifest?.Prerequisites?.Contains(modId) == true;
        }

        internal static Type Type(string modId, string fullName)
        {
            if (!Allowed(modId)) return null;
            var mod = ModLoader.Mods.FirstOrDefault(m => m.IsEnabled && m.Manifest?.ID == modId);
            if (mod?.Assembly == null) return null;
            if (!types.TryGetValue(modId, out var index))
            {
                index = ModReflect.Types(modId).ToDictionary(t => t.FullName, StringComparer.Ordinal);
                types[modId] = index;
            }
            return index.TryGetValue(fullName, out var type) ? type : null;
        }

        internal static void Clear() => types.Clear();

        internal static T Static<T>(Type type, string name)
        {
            if (type == null) return default;
            try { return ModReflect.GetStatic<T>(type, name); }
            catch (MissingMemberException) { return default; }
        }
    }
}
