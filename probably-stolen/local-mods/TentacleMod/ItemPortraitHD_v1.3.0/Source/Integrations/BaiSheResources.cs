using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemPortraitHD
{
    internal static class BaiSheResources
    {
        internal const string MainId = "baishe_all_characters_feminized";
        internal const string ExtensionId = "baishe_r18_pack";

        // Keep the provider's exact stem, including duplicate-name suffixes.
        internal static string Select(ISet<string> resources, string stem, bool halfBody)
        {
            if (string.IsNullOrEmpty(stem)) return null;
            string preferred = (halfBody ? "R18Half/" : "R18Full/") + stem + ".png";
            if (resources.Contains(preferred)) return preferred;
            string fallback = (halfBody ? "R18Full/" : "R18Half/") + stem + ".png";
            return resources.Contains(fallback) ? fallback : null;
        }

        internal static string Stem(string resource)
        {
            if (resource == null || !resource.EndsWith(".png", StringComparison.Ordinal)) return null;
            foreach (string prefix in new[] { "R18Full/", "R18Half/", "MsR18/" })
                if (resource.StartsWith(prefix, StringComparison.Ordinal))
                    return resource.Substring(prefix.Length, resource.Length - prefix.Length - 4);
            return null;
        }

        internal static IEnumerable<string> Portraits(ISet<string> resources, bool halfBody)
        {
            // One version per stem; full and half variants must not double its weight.
            foreach (string stem in resources.Where(r => r.StartsWith("R18Full/", StringComparison.Ordinal) ||
                r.StartsWith("R18Half/", StringComparison.Ordinal)).Select(Stem).Where(s => s != null)
                .Distinct(StringComparer.Ordinal).OrderBy(s => s, StringComparer.Ordinal))
            {
                string selected = Select(resources, stem, halfBody);
                if (selected != null) yield return selected;
            }
            foreach (string resource in resources.Where(r => r.StartsWith("MsR18/", StringComparison.Ordinal) &&
                Stem(r) != null).OrderBy(r => r, StringComparer.Ordinal)) yield return resource;
        }
    }
}
