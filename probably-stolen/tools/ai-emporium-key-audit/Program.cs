using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Text;
using System.Text.RegularExpressions;

if(args.Length!=2) throw new Exception("Expected dll and report");
var dll=args[0]; var report=args[1];
var asm=AssemblyDefinition.ReadAssembly(dll,new ReaderParameters{ReadingMode=ReadingMode.Immediate});
var types=asm.MainModule.Types.SelectMany(AllTypes).ToList();
var o=new StringBuilder();
o.AppendLine("# AiEmporium localization-key path audit");
o.AppendLine($"Assembly: {asm.Name.FullName}; types: {types.Count}; resources: {asm.MainModule.Resources.Count}");
foreach(var res in asm.MainModule.Resources) o.AppendLine($"RESOURCE\t{res.Name}\t{res.ResourceType}");
int hits=0;var stringCandidates=new Dictionary<string,List<string>>(StringComparer.Ordinal);
foreach(var t in types) foreach(var m in t.Methods.Where(m=>m.HasBody)){
    var ins=m.Body.Instructions;
    bool related=t.FullName.Contains("localiz",StringComparison.OrdinalIgnoreCase)||m.Name.Contains("localiz",StringComparison.OrdinalIgnoreCase);
    for(int i=0;i<ins.Count;i++){
        if(ins[i].Operand is not MethodReference mr)continue;
        var n=mr.FullName;
        bool relevant=Regex.IsMatch(n,@"localiz|translate|\.T\(|\.GetText\(|\.GetString\(|\.GetLocalized\(",RegexOptions.IgnoreCase);
        if(!relevant&&!related)continue;
        if(!relevant)continue;
        hits++;
        if(hits<=5000){
            o.AppendLine($"\nCALL\t{m.FullName}\tIL_{ins[i].Offset:X4}\t{n}");
            for(int j=Math.Max(0,i-10);j<=Math.Min(ins.Count-1,i+3);j++){
                var v=ins[j].Operand?.ToString()??"";
                o.AppendLine($"\t{ins[j].Offset:X4}\t{ins[j].OpCode}\t{v.Replace('\n',' ')}");
            }
        }
        for(int j=Math.Max(0,i-10);j<i;j++){
            if(ins[j].OpCode==OpCodes.Ldstr&&ins[j].Operand is string s&&s.Length>0){
                if(!stringCandidates.TryGetValue(s,out var uses))stringCandidates[s]=uses=new();
                if(uses.Count<5)uses.Add(m.FullName+" => "+mr.Name);
            }
        }
    }
}
o.AppendLine("\n# ALL source type methods / language-relevant calls");
foreach(var t in types){
    o.AppendLine("TYPE\t"+t.FullName);
    foreach(var m in t.Methods.Where(m=>m.HasBody)){
        var ins=m.Body.Instructions;
        var calls=ins.Where(i=>i.Operand is MethodReference mr && Regex.IsMatch(mr.FullName,@"loc|translat|lang|ModHelper|key|text",RegexOptions.IgnoreCase)).Select(i=>((MethodReference)i.Operand).FullName).Distinct().Take(30).ToArray();
        var strs=ins.Where(i=>i.OpCode==OpCodes.Ldstr&&i.Operand is string).Select(i=>(string)i.Operand).ToArray();
        if(calls.Length==0 && !Regex.IsMatch(m.Name,@"loc|register|item|visitor|dialog|card|book|name|text",RegexOptions.IgnoreCase))continue;
        o.AppendLine("METHOD\t"+m.Name+"\t"+String.Join(" | ",calls));
        foreach(var v in strs.Where(v=>Regex.IsMatch(v,@"[\p{IsCJKUnifiedIdeographs}]|(?:^|[_\.])(loc|item|dialog|visitor|card|text|name)[_\.]",RegexOptions.IgnoreCase)).Take(8))
            o.AppendLine("LITERAL\t"+Convert.ToBase64String(Encoding.UTF8.GetBytes(v)).TrimEnd('=').Replace('+','-').Replace('/','_')+"\t"+v.Replace("\n","\\n").Replace("\t"," ").Substring(0,Math.Min(v.Length,160)));
    }
}
o.AppendLine("\n# Selected implementation IL");
foreach(var t in types.Where(t=>t.FullName is "AiEmporium.AiText" or "AiEmporium.AiJudge" or "AiEmporium.AiJudgeDialogueKeyPatch" or "AiEmporium.AiJudgeMechanicKeyPatch" or "AiEmporium.AiClientFactory" or "AiEmporium.AiLinesFirst" or "AiEmporium.AiPhones" or "AiEmporium.AiSettings")){
    foreach(var m in t.Methods.Where(m=>m.HasBody && (t.FullName=="AiEmporium.AiText"|| t.FullName.Contains("KeyPatch") || Regex.IsMatch(m.Name,@"ReplaceKey|SetDialogues|BuildDialog|Register|CreateAppraisal|GetLocalized|^Or$")))){
        o.AppendLine("IL_METHOD\t"+m.FullName);
        foreach(var i in m.Body.Instructions.Take(850)) {
            var v=i.Operand?.ToString()??"";
            o.AppendLine("IL\t"+i.Offset.ToString("X4")+"\t"+i.OpCode+"\t"+v.Replace("\r","\\r").Replace("\n","\\n").Replace("\t"," "));
        }
    }
}
o.AppendLine("\n# Localize-related types / methods");
foreach(var t in types.Where(t=>Regex.IsMatch(t.FullName,@"localiz|translate|language|i18n|modhelper",RegexOptions.IgnoreCase)).Take(300))o.AppendLine("TYPE\t"+t.FullName+"\t"+string.Join(";",t.Methods.Select(m=>m.Name).Take(60)));
o.AppendLine("\n# Localization key candidate summary");
o.AppendLine($"Calls: {hits}; candidate distinct strings: {stringCandidates.Count}");
foreach(var (s,uses) in stringCandidates.OrderBy(k=>k.Key,StringComparer.Ordinal)){
    string encoded=Convert.ToBase64String(Encoding.UTF8.GetBytes(s)).TrimEnd('=').Replace('+','-').Replace('/','_');
    o.AppendLine("KEY\t"+encoded+"\t"+s.Replace("\r","\\r").Replace("\n","\\n").Replace("\t","\\t").Substring(0,Math.Min(s.Length,250))+"\t"+string.Join(" | ",uses));
}
Directory.CreateDirectory(Path.GetDirectoryName(report)!);
File.WriteAllText(report,o.ToString(),Encoding.UTF8);
Console.WriteLine($"Audit: {hits} calls; {stringCandidates.Count} distinct candidates; {o.Length} characters");
static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t){yield return t;foreach(var x in t.NestedTypes)foreach(var n in AllTypes(x))yield return n;}
