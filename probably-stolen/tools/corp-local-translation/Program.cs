using System.Text;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length != 5)
    throw new ArgumentException("Usage: CorpLocalTranslator <Corp.dll> <main.csv> <supp.csv> <out.dll> <report.txt>");

string source = args[0], main = args[1], supplemental = args[2], output = args[3], report = args[4];
if (Path.GetFullPath(source) == Path.GetFullPath(output)) throw new Exception("Input must not be overwritten.");

var translations = new Dictionary<string,string>(StringComparer.Ordinal);
var ambiguities = new HashSet<string>(StringComparer.Ordinal);
var rows = new Dictionary<string,int>(StringComparer.Ordinal);
foreach(var file in new[] { main,supplemental }) {
    int rowsSeen=0;
    foreach(var cells in ReadCsv(file)) {
        if(cells.Count<2)continue;
        string key=cells[0].TrimStart('\uFEFF'), ko=cells[1];
        string original="";
        if(key.StartsWith("S64|",StringComparison.Ordinal)) {
            string raw=key[4..].Replace('-','+').Replace('_','/');
            raw+=new string('=',(4-raw.Length%4)%4);
            try {original=Encoding.UTF8.GetString(Convert.FromBase64String(raw));}
            catch {continue;}
        } else if(key.StartsWith("S|",StringComparison.Ordinal)) original=key[2..];
        else continue; // Native K keys are not source-literal replacements.
        if(string.IsNullOrEmpty(original)||string.IsNullOrEmpty(ko))continue;
        if(translations.TryGetValue(original,out var previous)&&previous!=ko)ambiguities.Add(original);
        translations[original]=ko;
        rowsSeen++;
    }
    rows[Path.GetFileName(file)]=rowsSeen;
}
var resolver = new DefaultAssemblyResolver();
var managedPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(source)!, "..", "..", "Managed"));
resolver.AddSearchDirectory(managedPath);
using var assembly = AssemblyDefinition.ReadAssembly(source,new ReaderParameters { AssemblyResolver = resolver });
if(assembly.Name.Name!="CorpPresence")throw new Exception("Unexpected assembly "+assembly.Name.Name);
var types=assembly.MainModule.Types.SelectMany(AllTypes).ToList();
var l=types.Single(t=>t.FullName=="CorpPresence.L");
if(!l.Methods.Any(m=>m.Name=="T"&&m.Parameters.Count==2))
    throw new Exception("Bilingual L.T method not present. Refusing to patch an unrecognized version.");
var audit=new List<string> {
    "# CorpPresence v0.9.6 offline Korean text patch",
    "Input: "+source, "Input assembly: "+assembly.Name.FullName,
    "CSV translations: "+translations.Count,
    "Conflicting source mappings (supplemental precedence): "+ambiguities.Count,
    "No source mods or original Workshop files changed."
};
var translatedByMethod=new SortedDictionary<string,int>(StringComparer.Ordinal);
var replacedExamples=new List<string>();
var skipped=new Dictionary<string,int>(StringComparer.Ordinal);
int inspected=0, replaced=0, chinese=0, english=0, lTTranslated=0;
foreach(var type in types)foreach(var method in type.Methods.Where(m=>m.HasBody)){
    // These paths may contain sprite IDs, save identifiers, migration keys or tags.
    // Even if a literal resembles display text, never rewrite strings here.
    if(IsSensitiveMethod(type.FullName,method.Name)) {
        skipped["sensitive-method"] = skipped.GetValueOrDefault("sensitive-method")+1;
        continue;
    }
    var ins=method.Body.Instructions;
    for(int n=0;n<ins.Count;n++){
        if(ins[n].OpCode!=OpCodes.Ldstr || ins[n].Operand is not string value)continue;
        inspected++;
        if(!translations.TryGetValue(value,out var ko)||value==ko)continue;
        bool isChinese=Regex.IsMatch(value,@"[\p{IsCJKUnifiedIdeographs}\p{IsCJKCompatibilityIdeographs}]");
        bool isEnglish=Regex.IsMatch(value,@"[a-zA-Z]");
        bool nearLT = false;
        // Most paired bilingual literals appear just before L.T(zh, en).
        for(int j=n+1;j<Math.Min(n+5,ins.Count);j++){
            if((ins[j].OpCode==OpCodes.Call || ins[j].OpCode==OpCodes.Callvirt)
                && ins[j].Operand is MethodReference call
                && call.DeclaringType.FullName=="CorpPresence.L" && call.Name=="T"){
                nearLT=true;break;
            }
        }
        if(!isChinese && !nearLT && !LooksLikeVisibleEnglish(value)) {
            skipped["non-visible-literal"] = skipped.GetValueOrDefault("non-visible-literal")+1;
            continue;
        }
        if(!SamePlaceholders(value,ko)) {
            skipped["format-tokens-changed"]=skipped.GetValueOrDefault("format-tokens-changed")+1;
            if(replacedExamples.Count<120)replacedExamples.Add("SKIP format: "+method.FullName+" : "+Short(value)+" -> "+Short(ko));
            continue;
        }
        // Do not patch identifiers, serialization or mechanical constants.
        if(IsPotentialIdentifier(value)) {
            skipped["identifier-risk"]=skipped.GetValueOrDefault("identifier-risk")+1;
            continue;
        }
        ins[n].Operand=ko;
        replaced++;
        if(isChinese)chinese++;
        else if(isEnglish)english++;
        if(nearLT)lTTranslated++;
        translatedByMethod[method.FullName]=translatedByMethod.GetValueOrDefault(method.FullName)+1;
        if(replacedExamples.Count<120)replacedExamples.Add("REPLACED: "+Short(value)+" => "+Short(ko));
    }
}
if(replaced<50)throw new Exception("Suspiciously low replacement count "+replaced+"; refusing to produce package.");
assembly.Write(output);
using(var verify=AssemblyDefinition.ReadAssembly(output)){
    var all=verify.MainModule.Types.SelectMany(AllTypes).SelectMany(t=>t.Methods).Where(m=>m.HasBody).SelectMany(m=>m.Body.Instructions);
    int remaining=all.Count(i=>i.OpCode==OpCodes.Ldstr && i.Operand is string str && translations.ContainsKey(str));
    audit.Add("Original-string matches remaining: "+remaining);
    audit.Add("Assembly readback: "+verify.Name.FullName);
}
audit.Add("String literals examined: "+inspected);
audit.Add("Literals translated: "+replaced);
audit.Add("Chinese literals translated: "+chinese);
audit.Add("English literals translated: "+english);
audit.Add("L.T-associated translated literals: "+lTTranslated);
foreach(var k in skipped)audit.Add("Skipped ["+k.Key+"]: "+k.Value);
audit.Add("Methods containing translated literals: "+translatedByMethod.Count);
audit.Add("");
audit.Add("## Sample mappings");
audit.AddRange(replacedExamples);
audit.Add("");
audit.Add("## Per-method patch counts");
audit.AddRange(translatedByMethod.Select(k=>k.Key+" : "+k.Value));
File.WriteAllLines(report,audit,Encoding.UTF8);
Console.WriteLine($"Translated {replaced} literals ({chinese} Chinese, {english} English) across {translatedByMethod.Count} methods. File: {output}");

static bool IsSensitiveMethod(string type,string method) {
    if(type=="CorpPresence.Portraits" || type.StartsWith("CorpPresence.Portraits/"))return true;
    if(type=="CorpPresence.UI.Tex" || type.StartsWith("CorpPresence.UI.Tex/"))return true;
    if(type=="CorpPresence.ItemRegistry" || type.StartsWith("CorpPresence.ItemRegistry/"))return true;
    if(type=="CorpPresence.ItemTags" || type.StartsWith("CorpPresence.ItemTags/"))return true;
    if(type=="CorpPresence.RifleMods" && method=="Overlays")return true;
    if(type=="CorpPresence.Goods" && method=="ApplyAppearance")return true;
    if(type=="CorpPresence.Cards" && method=="VanillaCard")return true;
    if(type=="CorpPresence.Cleanup" && method=="BeforeSave")return true;
    if(type=="CorpPresence.Hooks" && method=="BeforeSave")return true;
    return false;
}
static bool LooksLikeVisibleEnglish(string value) {
    if(value.Length<3 || value.Length>4000)return false;
    if(value.Contains(' ')||value.Contains('\n')||value.Contains('.')||value.Contains('!')||value.Contains('?')
      ||value.Contains('\'')||value.Contains(',')||value.Contains(':'))return true;
    // Single word display labels can be translated when mappings exist and aren't constants.
    return Regex.IsMatch(value,@"^[A-Z][a-z][a-zA-Z-]{2,30}$");
}
static bool IsPotentialIdentifier(string value) {
    if(Regex.IsMatch(value,@"^[A-Z][A-Z0-9_]+$"))return true;
    if(Regex.IsMatch(value,@"^[a-z][a-z0-9_]+$"))return true;
    if(Regex.IsMatch(value,@"^(?:[a-zA-Z0-9_]+\.)+[a-zA-Z0-9_]+$"))return true;
    return false;
}
static bool SamePlaceholders(string original,string translation){
    var p=new Regex(@"\{(?:\d+|[A-Za-z_][A-Za-z0-9_]*)\}");
    var a=p.Matches(original).Select(m=>m.Value).OrderBy(x=>x).ToArray();
    var b=p.Matches(translation).Select(m=>m.Value).OrderBy(x=>x).ToArray();
    return a.SequenceEqual(b);
}
static string Short(string s) => s.Replace("\r","\\r").Replace("\n","\\n")[..Math.Min(120,s.Length+0)];
static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t){yield return t;foreach(var x in t.NestedTypes)foreach(var n in AllTypes(x))yield return n;}
static IEnumerable<List<string>> ReadCsv(string path){
    string data=File.ReadAllText(path,Encoding.UTF8);
    List<string> fields=new();var f=new StringBuilder();bool quoted=false;
    for(int i=0;i<data.Length;i++){
        char c=data[i];
        if(quoted){
            if(c=='"'){if(i+1<data.Length&&data[i+1]=='"'){f.Append('"');i++;}else quoted=false;}
            else f.Append(c);
        }else{
            if(c=='"')quoted=true;
            else if(c==','){fields.Add(f.ToString());f.Clear();}
            else if(c=='\n'){fields.Add(f.ToString().TrimEnd('\r'));f.Clear();yield return fields;fields=new();}
            else f.Append(c);
        }
    }
    if(f.Length>0||fields.Count>0){fields.Add(f.ToString());yield return fields;}
}