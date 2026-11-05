using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Text;
using System.Text.RegularExpressions;

if(args.Length<4) throw new ArgumentException("Usage: input.dll main.csv supplement.csv output-report.tsv");
var asm=AssemblyDefinition.ReadAssembly(args[0],new ReaderParameters{ReadingMode=ReadingMode.Immediate});
var allTypes=asm.MainModule.Types.SelectMany(Types).ToArray();
var dict=new Dictionary<string,List<string>>(StringComparer.Ordinal);
var csvData=new Dictionary<string,string>(StringComparer.Ordinal);
int conflicts=0;
foreach(var p in new[]{args[1],args[2]}){
    foreach(var row in Csv(File.ReadAllText(p,Encoding.UTF8))){
        if(row.Count<2||string.IsNullOrWhiteSpace(row[1]))continue;
        var key=row[0].TrimStart('\uFEFF');
        string source="";
        if(key.StartsWith("S|"))source=key[2..];
        else if(key.StartsWith("S64|")) {
            try {
                var enc=key[4..].Replace('-','+').Replace('_','/');
                enc=enc.PadRight(enc.Length+(4-enc.Length%4)%4,'=');
                source=Encoding.UTF8.GetString(Convert.FromBase64String(enc));
            } catch {continue;}
        }else continue;
        if(source.Length==0)continue;
        if(csvData.TryGetValue(source,out var old)&&old!=row[1])conflicts++;
        csvData[source]=row[1];
    }
}
foreach(var t in allTypes)foreach(var m in t.Methods.Where(m=>m.HasBody))foreach(var i in m.Body.Instructions){
    if(i.OpCode!=OpCodes.Ldstr||i.Operand is not string literal)continue;
    if(!dict.TryGetValue(literal,out var uses))dict[literal]=uses=new();
    if(uses.Count<12)uses.Add(m.FullName+" @"+i.Offset.ToString("X4"));
}
int total=0,matched=0,uncovered=0;
var report=new StringBuilder();
report.AppendLine("# Tentacle MidnightPawn 1.2.0 original DLL literal audit");
report.AppendLine("# Fields status, src-b64url, source, Korean translation, usage (first 12)");
report.AppendLine("Assembly\t"+asm.Name.FullName);
report.AppendLine("Total unique literals\t"+dict.Count);
report.AppendLine("CSV source mappings\t"+csvData.Count);
report.AppendLine("CSV conflicting source mappings\t"+conflicts);
foreach(var (src,uses) in dict.OrderBy(k=>k.Key,StringComparer.Ordinal)){
   if(!Regex.IsMatch(src,@"[\p{IsCJKUnifiedIdeographs}]"))continue;
   total++;
   bool ok=csvData.TryGetValue(src,out var ko);
   if(ok)matched++;else uncovered++;
   var b64=Convert.ToBase64String(Encoding.UTF8.GetBytes(src)).TrimEnd('=').Replace('+','-').Replace('/','_');
   report.AppendLine((ok?"MATCH":"MISSING")+"\t"+b64+"\t"+src.Replace("\r","\\r").Replace("\n","\\n").Replace("\t","\\t")+"\t"+(ko??"").Replace("\r","\\r").Replace("\n","\\n").Replace("\t","\\t")+"\t"+String.Join(" | ",uses));
}
report.Insert(0,$"# Chinese literal unique={total}, matched={matched}, uncovered={uncovered}\n");
Directory.CreateDirectory(Path.GetDirectoryName(args[3])!);
File.WriteAllText(args[3],report.ToString(),Encoding.UTF8);
Console.WriteLine($"Chinese unique={total} mapped={matched} missing={uncovered}, duplicates/conflicts={conflicts}, totalUnique={dict.Count}");
static IEnumerable<TypeDefinition> Types(TypeDefinition t){yield return t;foreach(var n in t.NestedTypes)foreach(var v in Types(n))yield return v;}
static IEnumerable<List<string>> Csv(string s){
 var row=new List<string>();var f=new StringBuilder();bool quoted=false;
 for(int i=0;i<s.Length;i++){
   var c=s[i];
   if(quoted){
     if(c=='"'){if(i+1<s.Length&&s[i+1]=='"'){f.Append('"');i++;}else quoted=false;}
     else f.Append(c);
   }else{
     if(c=='"')quoted=true;
     else if(c==','){row.Add(f.ToString());f.Clear();}
     else if(c=='\n'){row.Add(f.ToString().TrimEnd('\r'));f.Clear();yield return row;row=new();}
     else f.Append(c);
   }
 }
 if(row.Count>0||f.Length>0){row.Add(f.ToString());yield return row;}
}
