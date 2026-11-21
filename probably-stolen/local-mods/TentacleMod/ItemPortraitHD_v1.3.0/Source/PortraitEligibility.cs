using System;

namespace ItemPortraitHD
{
    internal static class PortraitEligibility
    {
        internal static bool CanAdjustWeight(bool registeredGameRole, bool registeredReplacement, bool classifiedLocal, bool hasMappedReplacement) =>
            classifiedLocal || registeredReplacement || (registeredGameRole && !hasMappedReplacement);

        internal static int Tier(bool adjustable, bool character) => adjustable ? 0 : character ? 1 : 2;

        internal static bool IsPortrait(string key, bool registeredCharacter)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            // Character names registered by SpriteDict are authoritative. Additional
            // portrait mods use "portrait" keys; never include arbitrary item icons.
            return registeredCharacter || key.IndexOf("portrait", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
