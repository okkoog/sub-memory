using System;
using System.Collections.Generic;
using System.Text;

namespace PostItemTranslationProbe
{
    public sealed class Translation
    {
        private readonly Dictionary<string, Item> items =
            new Dictionary<string, Item>(StringComparer.Ordinal);
        private readonly Dictionary<string, Paper> papers =
            new Dictionary<string, Paper>(StringComparer.Ordinal);

        public IEnumerable<Item> Items => items.Values;
        public IEnumerable<Paper> Papers => papers.Values;
        public int ItemCount => items.Count;
        public int PaperCount => papers.Count;

        public sealed class Paper
        {
            public readonly string ModId, BookId, ElementId, Original, Korean;
            public Paper(string modId, string bookId, string elementId,
                         string original, string korean)
            {
                ModId = modId; BookId = bookId; ElementId = elementId;
                Original = original; Korean = korean;
            }
        }

        public sealed class Item
        {
            private sealed class Pair
            {
                public readonly string Source, Korean;
                public Pair(string source, string korean)
                { Source = source; Korean = korean; }
            }
            public readonly string ModId, ItemId;
            private readonly Dictionary<string, Pair> fields =
                new Dictionary<string, Pair>(StringComparer.Ordinal);

            public Item(string modId, string itemId)
            { ModId = modId; ItemId = itemId; }

            public void Add(string field, string source, string korean)
            {
                if (fields.ContainsKey(field))
                    throw new FormatException("Duplicate item translation field");
                fields.Add(field, new Pair(source, korean));
            }

            private string Get(string field, string current)
            {
                return current != null &&
                    fields.TryGetValue(field, out var pair) &&
                    string.Equals(current, pair.Source, StringComparison.Ordinal)
                    ? pair.Korean : current;
            }

            public void Apply(GameItem item)
            {
                // Do not call overridden accessors of another mod's derived class.
                if (item == null || item.GetType() != typeof(GameItemElement)) return;
                var n = item.name; var s = item.shortDescription;
                var l = item.longDescription; var f = item.flavorText;
                var nn = Get("name", n); var ss = Get("shortDescription", s);
                var ll = Get("longDescription", l); var ff = Get("flavorText", f);
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

        public static Translation Parse(string modId, string text)
        {
            if (string.IsNullOrWhiteSpace(modId) || text == null || text.Length > 524288)
                throw new FormatException("Invalid mod ID or missing/oversized CSV");

            var rows = Csv(text.TrimStart('\uFEFF'));
            if (rows.Count == 0 || rows[0].Length != 5 ||
                string.Join(",", rows[0]) != "Type,Target,Field,Original,Korean")
                throw new FormatException("CSV header must be Type,Target,Field,Original,Korean");

            var result = new Translation();
            for (int i = 1; i < rows.Count; i++)
            {
                var r = rows[i];
                if (r.Length != 5 || string.IsNullOrWhiteSpace(r[1]) ||
                    string.IsNullOrWhiteSpace(r[2]))
                    throw new FormatException("Invalid row");
                if (r[3].Length == 0 || r[4].Length == 0) continue;
                if (Tokens(r[3]) != Tokens(r[4]))
                    throw new FormatException("Placeholder mismatch");

                if (r[0] == "Item")
                {
                    if (r[2] != "name" && r[2] != "shortDescription" &&
                        r[2] != "longDescription" && r[2] != "flavorText")
                        throw new FormatException("Unsupported item field");
                    if (!result.items.TryGetValue(r[1], out var item))
                    {
                        item = new Item(modId, r[1]);
                        result.items.Add(r[1], item);
                    }
                    item.Add(r[2], r[3], r[4]);
                }
                else if (r[0] == "Paper")
                {
                    string key = r[1] + "\u001f" + r[2];
                    if (result.papers.ContainsKey(key))
                        throw new FormatException("Duplicate paper element");
                    result.papers.Add(key,
                        new Paper(modId, r[1], r[2], r[3], r[4]));
                }
                else throw new FormatException("Unsupported translation type");
            }
            return result;
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
            var output = new List<string[]>();
            var row = new List<string>();
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
                    row.Add(cell.ToString());
                    cell.Clear();
                    closed = started = false;
                    if (c != ',')
                    {
                        if (row.Count != 1 || row[0] != "") output.Add(row.ToArray());
                        row.Clear();
                        if (c == '\r' && i + 1 < s.Length && s[i + 1] == '\n') i++;
                    }
                }
                else if (closed) throw new FormatException("Trailing text after quoted cell");
                else if (c == '"')
                {
                    if (started) throw new FormatException("Quote in unquoted cell");
                    quoted = started = true;
                }
                else { cell.Append(c); started = true; }
            }

            if (quoted) throw new FormatException("Unclosed quoted cell");
            if (started || closed || row.Count != 0)
            {
                row.Add(cell.ToString());
                output.Add(row.ToArray());
            }
            return output;
        }
    }
}
