using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace ItemPortraitHD
{
    internal sealed class MappedFile
    {
        internal string Role, Relative;
        internal bool? Readable;
    }
    internal static class MappingRules
    {
        internal static bool MatchesSlot(string expectedName, bool? expectedReadable, string actualName, bool readable) =>
            expectedName == actualName && (!expectedReadable.HasValue || expectedReadable.Value == readable);
        internal static IEnumerable<MappedFile> Retro(IEnumerable<string> lines) => lines.Skip(1)
            .Select(l => l.Split(',')).Where(c => c.Length == 2 && !string.IsNullOrWhiteSpace(c[0]))
            .Select(c => new MappedFile { Role = c[0].Trim(), Relative = "Portraits/" + c[1].Trim() + ".png" });
        internal static IEnumerable<MappedFile> Refined(IEnumerable<string> lines) => lines.Select(l => l.Trim().Split('\t'))
            .Where(c => c.Length >= 3 && (c[1] == "0" || c[1] == "1"))
            .Select(c => new MappedFile { Role = c[0], Relative = "Portraits/" + c[2], Readable = c[1] == "1" });
        internal static IEnumerable<MappedFile> Approved(string json)
        {
            var root = JObject.Parse(json);
            if ((int?)root["version"] != 1 || !(root["targets"] is JArray targets)) yield break;
            foreach (var entry in targets)
            {
                if (!(entry is JObject)) continue;
                if ((string)entry["status"] != "approved" || !string.IsNullOrEmpty((string)entry["sourceModId"])) continue;
                string role = (string)entry["spriteKey"], file = (string)entry["file"];
                if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(file)) continue;
                yield return new MappedFile { Role = role, Relative = "Approved/" + file };
            }
        }
    }
}
