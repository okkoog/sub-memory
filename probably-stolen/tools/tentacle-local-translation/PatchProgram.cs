using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Text;
using System.Text.RegularExpressions;

if(args.Length != 5) throw new ArgumentException("source.dll main.csv supplement.csv translated.dll report.txt");
string input=args[0], main=args[1], supp=args[2], output=args[3], report=args[4];
if(Path.GetFullPath(input)==Path.GetFullPath(output))throw new Exception("Refuse source overwrite");
var tr=new Dictionary<string,string>(StringComparer.Ordinal);
int conflicts=0;
foreach(var file in new[]{main,supp}){
    foreach(var row in ReadCsv(File.ReadAllText(file,Encoding.UTF8))){
        if(row.Count<2||String.IsNullOrWhiteSpace(row[1]))continue;
        string key=row[0].TrimStart('\uFEFF'), value=row[1], source="";
        if(key.StartsWith("S|",StringComparison.Ordinal))source=key[2..];
        else if(key.StartsWith("S64|",StringComparison.Ordinal)){
            var s=key[4..].Replace('-','+').Replace('_','/');
            s=s.PadRight(s.Length+(4-s.Length%4)%4,'=');
            try{source=Encoding.UTF8.GetString(Convert.FromBase64String(s));}catch{continue;}
        }else continue;
        if(source.Length<1)continue;
        if(tr.TryGetValue(source,out var prev)&&prev!=value)conflicts++;
        tr[source]=value;
    }
}
var notes=new List<string>{"# MidnightPawn local Korean DLL patch (offline Cecil, source untouched)",$"Input: {input}",$"Translations: {tr.Count}",$"Conflicting mappings: {conflicts}"};
if(conflicts!=0)throw new Exception("Conflicting source mappings!");
var read=new ReaderParameters{ReadingMode=ReadingMode.Immediate};
using var assembly=AssemblyDefinition.ReadAssembly(input,read);
if(assembly.Name.Name!="MidnightPawn")throw new Exception("Unexpected assembly "+assembly.Name.FullName);
var types=assembly.MainModule.Types.SelectMany(AllTypes).ToArray();
int occurrences=0, uniqueCount=0;
var changed=new Dictionary<string,int>(StringComparer.Ordinal);
var missing=new HashSet<string>(StringComparer.Ordinal);
var translated=new HashSet<string>(StringComparer.Ordinal);
var samples=new List<string>();
foreach(var t in types)foreach(var m in t.Methods.Where(m=>m.HasBody)){
    foreach(var i in m.Body.Instructions){
        if(i.OpCode!=OpCodes.Ldstr||i.Operand is not string value)continue;
        if(!Regex.IsMatch(value,@"\p{IsCJKUnifiedIdeographs}"))continue;
        if(!tr.TryGetValue(value,out var ko)){
            missing.Add(value);
            continue;
        }
        if(ko==value||Regex.IsMatch(ko,@"\p{IsCJKUnifiedIdeographs}")){
            throw new Exception("Translation still Chinese: "+value+" -> "+ko);
        }
        if(!MatchTokens(value,ko))throw new Exception("Format token mismatch: "+value+" -> "+ko);
        i.Operand=ko;
        occurrences++;
        translated.Add(value);
        changed[m.FullName]=changed.GetValueOrDefault(m.FullName)+1;
        if(samples.Count<70)samples.Add("PATCH\t"+value.Replace("\n","\\n")+"\t"+ko.Replace("\n","\\n")+"\t"+m.FullName);
    }
}
uniqueCount=translated.Count;
if(missing.Count>0)throw new Exception($"Uncovered Chinese literals {missing.Count}: "+String.Join(" | ",missing.Take(12)));
if(uniqueCount<200)throw new Exception("Unexpectedly low translated literals: "+uniqueCount);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
assembly.Write(output);
using var check=AssemblyDefinition.ReadAssembly(output);
var checkTypes=check.MainModule.Types.SelectMany(AllTypes).ToArray();
if(check.Name.FullName!=assembly.Name.FullName)throw new Exception("Assembly name changed");
int remaining=checkTypes.SelectMany(t=>t.Methods.Where(m=>m.HasBody).SelectMany(m=>m.Body.Instructions))
    .Count(i=>i.OpCode==OpCodes.Ldstr&&i.Operand is string s&&Regex.IsMatch(s,@"\p{IsCJKUnifiedIdeographs}"));
if(remaining!=0)throw new Exception("Remaining Chinese ldstr instances: "+remaining);
var originalMethods=types.SelectMany(t=>t.Methods.Where(m=>m.HasBody)).ToDictionary(m=>m.FullName,m=>m,StringComparer.Ordinal);
var outputMethods=checkTypes.SelectMany(t=>t.Methods.Where(m=>m.HasBody)).ToDictionary(m=>m.FullName,m=>m,StringComparer.Ordinal);
if(originalMethods.Count!=outputMethods.Count)throw new Exception("Method count changed");
foreach(var (signature,m) in originalMethods){
    if(!outputMethods.TryGetValue(signature,out var outputMethod))throw new Exception("Missing method: "+signature);
    var before=m.Body.Instructions;
    var after=outputMethod.Body.Instructions;
    if(before.Count!=after.Count)throw new Exception("Instruction count changed: "+signature);
    for(int n=0;n<before.Count;n++){
        if(before[n].OpCode!=after[n].OpCode)throw new Exception("Changed opcode: "+signature+" instruction "+n);
        if(before[n].OpCode==OpCodes.Ldstr&&!String.Equals((string)before[n].Operand,(string)after[n].Operand,StringComparison.Ordinal))
            throw new Exception("Readback string mismatch: "+signature+" instruction "+n);
    }
}
notes.Add($"Patch occurrences: {occurrences}");
notes.Add($"Distinct translated source literals: {uniqueCount}");
notes.Add($"Modified methods: {changed.Count}");
notes.Add($"Original method count: {originalMethods.Count}");
notes.Add($"Readback Chinese IL literals remaining: {remaining}");
notes.Add($"Original file size: {new FileInfo(input).Length}");
notes.Add($"Output file size: {new FileInfo(output).Length}");
notes.Add("All IL opcodes and instruction counts match before/after patch; translated ldstr values read back correctly.");
notes.Add("Note: static IL inspection cannot establish savegame compatibility or runtime startup behavior.");
notes.AddRange(samples);
Directory.CreateDirectory(Path.GetDirectoryName(report)!);
File.WriteAllLines(report,notes,Encoding.UTF8);
Console.WriteLine($"Patched {occurrences} string instructions, distinct {uniqueCount}, changed methods {changed.Count}, unreadable Chinese {remaining}");
static bool MatchTokens(string a,string b){
    var regex=new Regex(@"\{\{[a-zA-Z_][a-zA-Z0-9_]*\}\}|\{(?:\d+|[a-zA-Z_][a-zA-Z0-9_]*)\}");
    return regex.Matches(a).Select(x=>x.Value).OrderBy(x=>x,StringComparer.Ordinal)
        .SequenceEqual(regex.Matches(b).Select(x=>x.Value).OrderBy(x=>x,StringComparer.Ordinal));
}
static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t){yield return t;foreach(var n in t.NestedTypes)foreach(var m in AllTypes(n))yield return m;}
static IEnumerable<List<string>> ReadCsv(string data){
    List<string> row=new();var f=new StringBuilder();bool quoted=false;
    for(int j=0;j<data.Length;j++){
        char c=data[j];
        if(quoted){
            if(c=='"'){if(j+1<data.Length&&data[j+1]=='"'){f.Append('"');j++;}else quoted=false;}
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
