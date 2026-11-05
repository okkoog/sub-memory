using Mono.Cecil;
using System.Text;
if(args.Length!=2)throw new ArgumentException("CorpMetadataAudit <CorpPresence.dll> <output.txt>");
using var a=AssemblyDefinition.ReadAssembly(args[0]);
var lines=new List<string>{"# CorpPresence reflection/no-hooks feasibility — actual DLL metadata","DLL: "+a.Name.FullName};
string[] watch={"CorpPresence.Engine","CorpPresence.Character","CorpPresence.Visit","CorpPresence.Company","CorpPresence.L","CorpPresence.ItemRegistry","CorpPresence.CompanyVisitors","CorpPresence.Main","CorpPresence.UI.MenuLabels","CorpPresence.Hooks","CorpPresence.Phone"};
foreach(var type in a.MainModule.Types.SelectMany(AllTypes)){
 if(!watch.Any(w=>type.FullName.Equals(w,StringComparison.Ordinal)||type.FullName.StartsWith(w+"/",StringComparison.Ordinal)))continue;
 lines.Add("\n## "+type.FullName);
 foreach(var f in type.Fields)lines.Add("F "+(f.IsStatic?"static ":"")+f.FieldType.FullName+" "+f.Name);
 foreach(var p in type.Properties)lines.Add("P "+p.PropertyType.FullName+" "+p.Name+" "+(p.GetMethod!=null?"get":"")+" "+(p.SetMethod!=null?"set":""));
 foreach(var m in type.Methods.Where(m=>m.Name.Contains("Init",StringComparison.OrdinalIgnoreCase)||m.Name.Contains("Register",StringComparison.OrdinalIgnoreCase)||m.Name.Contains("Build",StringComparison.OrdinalIgnoreCase)||m.Name.Contains("Create",StringComparison.OrdinalIgnoreCase)||m.Name.Contains("Load",StringComparison.OrdinalIgnoreCase)||m.Name.Contains("Start",StringComparison.OrdinalIgnoreCase)))lines.Add("M "+m.FullName);
 if(type.FullName=="CorpPresence.UI.MenuLabels") foreach(var m in type.Methods.Where(m=>m.Name==".cctor"||m.Name=="Get")) {lines.Add("IL "+m.FullName); if(m.HasBody)foreach(var ins in m.Body.Instructions.Take(120))lines.Add("  "+ins.ToString());}
}
File.WriteAllLines(args[1],lines,Encoding.UTF8); Console.WriteLine("Written "+lines.Count+" metadata lines");
static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t) {yield return t;foreach(var n in t.NestedTypes)foreach(var x in AllTypes(n))yield return x;}
