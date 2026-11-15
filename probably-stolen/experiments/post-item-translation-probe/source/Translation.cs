using System;
using System.Collections.Generic;
using System.Text;

namespace PostItemTranslationProbe
{
    public sealed class Translation
    {
        private readonly Dictionary<string, TargetItem> entries =
            new Dictionary<string, TargetItem>(StringComparer.Ordinal);
        public IEnumerable<TargetItem> Items => entries.Values;
        public int Count => entries.Count;

        public sealed class TargetItem
        {
            private sealed class Pair
            {
                public readonly string Original, Korean;
                public Pair(string original, string korean) { Original = original; Korean = korean; }
            }
            public readonly string ModId, ItemId;
            private readonly Dictionary<string, Pair> fields =
                new Dictionary<string, Pair>(StringComparer.Ordinal);

            public TargetItem(string modId, string itemId) { ModId = modId; ItemId = itemId; }
            public void Add(string field, string original, string korean)
            {
                if (fields.ContainsKey(field)) throw new FormatException("Duplicate field");
                fields.Add(field, new Pair(original, korean));
            }
            private string Translate(string field, string input)
            {
                return input != null && fields.TryGetValue(field, out var pair) &&
                    string.Equals(input, pair.Original, StringComparison.Ordinal)
                    ? pair.Korean : input;
            }
            public void Apply(GameItem item)
            {
                // We deliberately avoid virtual setters on custom mod-derived item types.
                if (item == null || item.GetType() != typeof(GameItemElement)) return;
                var n = item.name; var s = item.shortDescription;
                var l = item.longDescription; var f = item.flavorText;
                var nn = Translate("name", n); var ss = Translate("shortDescription", s);
                var ll = Translate("longDescription", l); var ff = Translate("flavorText", f);
                if (nn == n && ss == s && ll == l && ff == f) return;
                try
                {
                    if (nn != n) item.name = nn;
                    if (ss != s) item.shortDescription = ss;
                    if (ll != l) item.longDescription = ll;
                    if (ff != f) item.flavorText = ff;
                }
                catch
                {
                    // Restore only already changed strings if any setter unexpectedly throws.
                    try
                    {
                        if (item.name != n) item.name = n;
                        if (item.shortDescription != s) item.shortDescription = s;
                        if (item.longDescription != l) item.longDescription = l;
                        if (item.flavorText != f) item.flavorText = f;
                    }
                    catch { }
                    throw;
                }
            }
        }

        public static Translation Parse(string text)
        {
            if (text == null || text.Length > 524288)
                throw new FormatException("Missing or oversized CSV");
            var rows = Csv(text.TrimStart('\uFEFF'));
            if (rows.Count == 0 || rows[0].Length != 5 ||
                string.Join(",", rows[0]) != "ModID,ItemID,Field,Original,Korean")
                throw new FormatException("CSV header");

            var parsed = new Translation();
            for (int i = 1; i < rows.Count; i++)
            {
                var r = rows[i];
                if (r.Length != 5) throw new FormatException("CSV column count");
                if (string.IsNullOrWhiteSpace(r[0]) || string.IsNullOrWhiteSpace(r[1]))
                    throw new FormatException("Empty mod/item ID");
                if (r[2] != "name" && r[2] != "shortDescription" &&
                    r[2] != "longDescription" && r[2] != "flavorText")
                    throw new FormatException("Unsupported field");
                if (r[3].Length == 0 || r[4].Length == 0) continue;
                if (Tokens(r[3]) != Tokens(r[4]))
                    throw new FormatException("Placeholder mismatch");
                string key = r[0] + "\u001f" + r[1];
                if (!parsed.entries.TryGetValue(key, out var item))
                {
                    item = new TargetItem(r[0], r[1]);
                    parsed.entries.Add(key, item);
                }
                item.Add(r[2], r[3], r[4]);
            }
            return parsed;
        }

        private static string Tokens(string s)
        {
            var tokens = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '{')
                {
                    if (i + 1 < s.Length && s[i + 1] == '{')
                    { tokens.Add("{{"); i++; continue; }
                    int end = s.IndexOf('}', i + 1);
                    if (end < 0 || s.Substring(i + 1, end - i - 1).Contains("{"))
                        throw new FormatException("Unbalanced placeholder");
                    tokens.Add(s.Substring(i, end - i + 1)); i = end;
                }
                else if (s[i] == '}')
                {
                    if (i + 1 >= s.Length || s[i + 1] != '}')
                        throw new FormatException("Unbalanced placeholder");
                    tokens.Add("}}"); i++;
                }
            }
            tokens.Sort(StringComparer.Ordinal);
            return string.Join("\u001f", tokens);
        }

        private static List<string[]> Csv(string s)
        {
            var output = new List<string[]>(); var row = new List<string>();
            var cell = new StringBuilder();
            bool quoted = false, closed = false, started = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (quoted)
                {
                    if (c == '"')
                    {
                        if (i + 1 < s.Length && s[i + 1] == '"')
                        { cell.Append('"'); i++; }
                        else { quoted = false; closed = true; }
                    }
                    else cell.Append(c);
                    continue;
                }
                if (c == ',' || c == '\r' || c == '\n')
                {
                    row.Add(cell.ToString()); cell.Clear(); closed = started = false;
                    if (c != ',')
                    {
                        if (row.Count != 1 || row[0] != "") output.Add(row.ToArray());
                        row.Clear();
                        if (c == '\r' && i + 1 < s.Length && s[i + 1] == '\n') i++;
                    }
                }
                else if (closed) throw new FormatException("Characters after quote");
                else if (c == '"')
                {
                    if (started) throw new FormatException("Quote in plain cell");
                    quoted = started = true;
                }
                else { cell.Append(c); started = true; }
            }
            if (quoted) throw new FormatException("Unclosed quoted CSV cell");
            if (started || closed || row.Count != 0)
            { row.Add(cell.ToString()); output.Add(row.ToArray()); }
            return output;
        }
    }
}
