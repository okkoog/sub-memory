using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

sealed class Row { public string id {get;set;}=""; public string key {get;set;}=""; public string source {get;set;}=""; }
sealed class Plan { public Row[] equipment {get;set;}=Array.Empty<Row>(); public Row[] accessory {get;set;}=Array.Empty<Row>(); }
class P {
  static void Need(bool yes,string reason){if(!yes)throw new Exception(reason);}
  static string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
  static IEnumerable<MethodDef> All(ModuleDefMD m)=>m.GetTypes().SelectMany(t=>t.Methods);
  static MethodDef M(ModuleDefMD m,string type,string method)=>All(m).Single(x=>x.DeclaringType.FullName==type&&x.Name==method);
  static IMethod Ref(ModuleDefMD m,string signature)=>All(m).Where(x=>x.HasBody).SelectMany(x=>x.Body.Instructions).Select(x=>x.Operand).OfType<IMethod>().First(x=>x.FullName==signature);
  static string Op(object? arg,Dictionary<Instruction,int> offsets,int delta=0)=>arg switch {
    null=>"",Instruction i=>"@"+(offsets[i]-delta),IList<Instruction> ii=>string.Join(",",ii.Select(i=>"@"+(offsets[i]-delta))),_=>arg.ToString()??""
  };
  static Dictionary<Instruction,int> Index(IList<Instruction> ins)=>ins.Select((x,i)=>(x,i)).ToDictionary(t=>t.x,t=>t.i);
  static void Same(MethodDef a,MethodDef b,int skip=0){
    Need(a.HasBody==b.HasBody,"Body "+a.FullName);if(!a.HasBody)return;
    var ai=a.Body.Instructions;var bi=b.Body.Instructions;Need(ai.Count+skip==bi.Count,"IL count "+a.FullName);
    var aidx=Index(ai);var bidx=Index(bi);
    for(int i=0;i<ai.Count;i++){
      var x=ai[i];var y=bi[i+skip];
      Need(x.OpCode.Name.Replace(".s","")==y.OpCode.Name.Replace(".s",""),"Op "+a.FullName+" "+i);
      Need(Op(x.Operand,aidx)==Op(y.Operand,bidx,skip),"Operand "+a.FullName+" "+i);
    }
    Need(a.Body.Variables.Select(x=>x.Type.FullName).SequenceEqual(b.Body.Variables.Select(x=>x.Type.FullName)),"Locals "+a.FullName);
    string EH(ExceptionHandler e,Dictionary<Instruction,int> idx,int delta) {
      string At(Instruction? i)=>i==null?"-":(idx[i]-delta).ToString();
      return string.Join("|",e.HandlerType.ToString(),At(e.TryStart),At(e.TryEnd),At(e.HandlerStart),At(e.HandlerEnd),At(e.FilterStart),e.CatchType?.FullName??"");
    }
    Need(a.Body.ExceptionHandlers.Select(x=>EH(x,aidx,0)).SequenceEqual(b.Body.ExceptionHandlers.Select(x=>EH(x,bidx,skip))),"Exception handlers "+a.FullName);
  }
  static void Main(string[] args){
    Need(args.Length==5,"Usage: existing-RLD.dll plan.json Assembly-CSharp.dll output-RLD.dll report.json");
    var input=File.ReadAllBytes(args[0]);
    const string expected="86ad3f461f263dd04199edf8dad7e8c3f2e017b0c8a013462eef8034733bda47";
    Need(Hash(input)==expected,"Wrong input DLL: require prior six-method direct CSV localization build, NOT Harmony experiment");
    var plan=JsonSerializer.Deserialize<Plan>(File.ReadAllText(args[1]))!;
    var rows=plan.equipment.Concat(plan.accessory).ToArray();
    Need(plan.equipment.Length==15&&plan.accessory.Length==4&&rows.Length==19,"Expect exactly 19 id/key/source pairs");
    Need(rows.All(r=>r.id.StartsWith("redlight_")&&r.key.StartsWith("rld_")&&r.source.Length>0)&&rows.Select(r=>r.id).Distinct().Count()==19,"Unique exact ID map");
    using var original=ModuleDefMD.Load(input);
    using var mod=ModuleDefMD.Load(input);
    using var game=ModuleDefMD.Load(args[2]);
    var target=M(mod,"RedLightDistrict.TalentPatches","PrefixGetTooltipBasic");
    var oldTarget=M(original,"RedLightDistrict.TalentPatches","PrefixGetTooltipBasic");
    Need(target.MethodSig.Params.Count==2&&target.MethodSig.RetType.FullName=="System.Boolean","Existing prefix target");
    Need(target.Body.Instructions.Count>400,"Existing prefix must be unchanged");
    var gType=game.GetTypes().Single(x=>x.FullName=="GameItem");
    var nameGetter=gType.Methods.Single(m=>m.Name=="get_name"&&m.ReturnType.FullName=="System.String");
    var importer=new Importer(mod,ImporterOptions.TryToUseTypeDefs);
    var getName=importer.Import(nameGetter);
    var getId=Ref(mod,"System.String GameItem::get_identifier()");
    var setName=Ref(mod,"GameItem GameItem::SetName(System.String)");
    var equality=Ref(mod,"System.Boolean System.String::op_Equality(System.String,System.String)");
    var read=M(mod,"RedLightDistrict.Korean.CsvLookup","Read");
    var wrappers=new Dictionary<string,MethodDef>();
    foreach(var row in rows){
      var wrapper=M(mod,"RedLightDistrict.Korean.CsvLookup",row.key);
      var il=wrapper.Body.Instructions;
      int at=il.Select((v,i)=>(v,i)).Single(x=>x.v.OpCode==OpCodes.Ldstr&&Equals(x.v.Operand,row.key)).i;
      Need(il[at+1].OpCode==OpCodes.Ldstr&&Equals(il[at+1].Operand,row.source),"CSV wrapper source "+row.id);
      Need(il[at+2].Operand is IMethod m&&m.FullName==read.FullName,"Wrapper must call CsvLookup.Read "+row.id);
      wrappers.Add(row.key,wrapper);
    }
    // Change existing GetTooltipBasic prefix itself; no new hooks, helper functions, patches or metadata types.
    // Only normalize exact old Chinese item names for known RLD identifiers when their live CSV key resolves.
    // If CSV isn't ready, the wrapper returns the original Chinese and nothing is changed.
    var inserted=new List<Instruction>();
    foreach(var row in rows){
      var skip=Instruction.Create(OpCodes.Nop);
      inserted.Add(Instruction.Create(OpCodes.Ldarg_0));
      inserted.Add(Instruction.Create(OpCodes.Callvirt,getId));
      inserted.Add(Instruction.Create(OpCodes.Ldstr,row.id));
      inserted.Add(Instruction.Create(OpCodes.Call,equality));
      inserted.Add(Instruction.Create(OpCodes.Brfalse,skip));
      inserted.Add(Instruction.Create(OpCodes.Ldarg_0));
      inserted.Add(Instruction.Create(OpCodes.Callvirt,getName));
      inserted.Add(Instruction.Create(OpCodes.Ldstr,row.source));
      inserted.Add(Instruction.Create(OpCodes.Call,equality));
      inserted.Add(Instruction.Create(OpCodes.Brfalse,skip));
      // Source is still Chinese: ask pre-existing CsvLookup wrapper by exact key.
      inserted.Add(Instruction.Create(OpCodes.Call,wrappers[row.key]));
      inserted.Add(Instruction.Create(OpCodes.Ldstr,row.source));
      inserted.Add(Instruction.Create(OpCodes.Call,equality));
      // If missing or still untranslated, keep it unchanged and retry on the next hover.
      inserted.Add(Instruction.Create(OpCodes.Brtrue,skip));
      inserted.Add(Instruction.Create(OpCodes.Ldarg_0));
      inserted.Add(Instruction.Create(OpCodes.Call,wrappers[row.key]));
      inserted.Add(Instruction.Create(OpCodes.Callvirt,setName));
      inserted.Add(Instruction.Create(OpCodes.Pop));
      inserted.Add(skip);
    }
    for(int i=0;i<inserted.Count;i++)target.Body.Instructions.Insert(i,inserted[i]);
    target.Body.SimplifyBranches();
    target.Body.OptimizeBranches();
    var mapOriginal=All(original).ToDictionary(x=>x.FullName);
    var mapCurrent=All(mod).ToDictionary(x=>x.FullName);
    Need(mapCurrent.Count==mapOriginal.Count&&mapOriginal.Keys.ToHashSet().SetEquals(mapCurrent.Keys),"No new methods");
    foreach(var kv in mapOriginal) if(kv.Key!=oldTarget.FullName)Same(kv.Value,mapCurrent[kv.Key]);
    Same(oldTarget,target,inserted.Count);
    Directory.CreateDirectory(Path.GetDirectoryName(args[3])!);
    mod.Write(args[3]);
    using var output=ModuleDefMD.Load(args[3]);
    var result=All(output).ToDictionary(x=>x.FullName);
    Need(result.Count==mapCurrent.Count,"Reopen all methods");
    foreach(var kv in mapCurrent)Same(kv.Value,result[kv.Key]);
    var resourceOriginal=original.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
    var resourceOutput=output.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
    Need(resourceOriginal.Count==1617&&resourceOutput.Count==1617&&resourceOriginal.All(x=>resourceOutput.TryGetValue(x.Key,out var checksum)&&x.Value==checksum),"1617 resources same");
    Need(original.GetAssemblyRefs().Select(x=>x.FullName).ToHashSet().SetEquals(output.GetAssemblyRefs().Select(x=>x.FullName)),"No new assembly references");
    var report=new{
      baseDllSha256=Hash(input),newDllSha256=Hash(File.ReadAllBytes(args[3])),size=new FileInfo(args[3]).Length,
      patchTarget=target.FullName,exactItemIds=rows.Length,directCsvWrappersUsed=rows.Select(x=>x.key),
      oldMethodsPreserved=mapOriginal.Count-1,existingMethodsModified=1,methodsAdded=0,
      resourcesSame=resourceOriginal.Count,newHarmonyHooks=0,independentGameBinaryUntouched=true,
      note="When exact id AND old Chinese name match, existing CsvLookup key wrapper is consulted at tooltip creation. If not yet translated, leave old name so the next hover retries. This fixes already-created items but changes GameItem.Name in memory once successfully translated; item identifier is untouched.",
      runtimeTested=false
    };
    File.WriteAllText(args[4],JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true})+"\n");
    Console.WriteLine(JsonSerializer.Serialize(report));
  }
}
