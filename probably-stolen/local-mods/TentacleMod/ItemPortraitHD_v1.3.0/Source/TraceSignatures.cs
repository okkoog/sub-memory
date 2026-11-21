using System;
using System.Reflection;
using UnityEngine;

namespace ItemPortraitHD
{
    internal static class TraceSignatures
    {
        internal static int ResultArgument(MethodInfo method)
        {
            var parameters = method.GetParameters();
            // Other Sprite arguments may be scratch buffers. Only Harmony's
            // documented __result parameter identifies the replaced lookup result.
            return Array.FindIndex(parameters, p => p.Name == "__result" &&
                (p.ParameterType == typeof(Sprite) || p.ParameterType == typeof(Sprite).MakeByRefType()));
        }

        internal static string SourceId(string provider, string key) => "trace:" + Uri.EscapeDataString(provider) + ":" + Uri.EscapeDataString(key);

        internal static bool TryParseSource(string id, out string provider, out string key)
        {
            provider = key = null;
            if (id == null || !id.StartsWith("trace:", StringComparison.Ordinal)) return false;
            int split = id.IndexOf(':', 6);
            if (split <= 6 || split == id.Length - 1) return false;
            try { provider = Uri.UnescapeDataString(id.Substring(6, split - 6)); key = Uri.UnescapeDataString(id.Substring(split + 1)); return true; }
            catch (UriFormatException) { return false; }
        }
    }
}
