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

  static void VerifyExactInsertion(MethodDef before,MethodDef after,int at,IMethod helper){
    var a=before.Body.Instructions;var b=after.Body.Instructions;
    Need(a.Count+1==b.Count,"One extra IL call");
    Need(b[at].OpCode==OpCodes.Call&&b[at].Operand is IMethod t&&t.FullName==helper.FullName,"Exact insertion");
    var ai=Index(a);var bi=Index(b);
    for(int i=0;i<a.Count;i++){
      var x=a[i];var y=b[i<at?i:i+1];
      Need(x.OpCode.Name.Replace(".s","")==y.OpCode.Name.Replace(".s",""),"Existing opcode "+i);
      var beforeOperand=Op(x.Operand,ai);
      string Target(object? op)=>op switch{
        null=>"",Instruction target=>"@"+(bi[target]-(bi[target]>at?1:0)),
        IList<Instruction> ins=>string.Join(",",ins.Select(q=>"@"+(bi[q]-(bi[q]>at?1:0)))),
        _=>op.ToString()??""
      };
      // Operand references must point to the exact corresponding existing IL, not to the newly inserted call.
      if(x.Operand is Instruction tgt) {
        var afterTarget=(Instruction)y.Operand!;
        Need(afterTarget==b[ai[tgt]<at?ai[tgt]:ai[tgt]+1],"Branch target "+i);
      } else if(x.Operand is IList<Instruction> seq) {
        var afterSeq=(IList<Instruction>)y.Operand!;
        Need(seq.Count==afterSeq.Count&&seq.Select((z,k)=>afterSeq[k]==b[ai[z]<at?ai[z]:ai[z]+1]).All(z=>z),"Switch target "+i);
      } else Need(x.Operand?.ToString()==y.Operand?.ToString(),"Operand "+i);
    }
    Need(before.Body.Variables.Select(x=>x.Type.FullName).SequenceEqual(after.Body.Variables.Select(x=>x.Type.FullName)),"No local changes");
    string EH(ExceptionHandler e,Dictionary<Instruction,int> ix,bool adjusted){
      string I(Instruction? ins)=>ins==null?"-":(adjusted&&ix[ins]>at ? ix[ins]-1:ix[ins]).ToString();
      return string.Join("|",e.HandlerType.ToString(),I(e.TryStart),I(e.TryEnd),I(e.HandlerStart),I(e.HandlerEnd),I(e.FilterStart),e.CatchType?.FullName??"");
    }
    Need(before.Body.ExceptionHandlers.Select(x=>EH(x,ai,false)).SequenceEqual(after.Body.ExceptionHandlers.Select(x=>EH(x,bi,true))),"EH boundaries unchanged");
  }
  static void Main(string[] args){
    Need(args.Length==5,"usage: prior.dll plan.json output.dll report.json csvkeyreport.json");
    var input=File.ReadAllBytes(args[0]);
    Need(Hash(input)=="86ad3f461f263dd04199edf8dad7e8c3f2e017b0c8a013462eef8034733bda47","Must patch exact prior direct-lookup DLL, NOT Harmony postfix");
    var plan=System.Text.Json.JsonDocument.Parse(File.ReadAllText(args[1]));
    var traits=plan.RootElement.GetProperty("traits").EnumerateArray().Select(x=>new{
      key=x.GetProperty("key").GetString()!,source=x.GetProperty("source").GetString()!
    }).ToArray();
    Need(traits.Length==22,"22 trait display IDs");
    using var baseline=ModuleDefMD.Load(input);
    using var patch=ModuleDefMD.Load(input);
    // Clothes/accessories are translated directly at their name-building point, not through a game hook.
    foreach(var entry in new[]{
      (type:"RedLightDistrict.TalentEquipment",helper:"EquipmentName"),
      (type:"RedLightDistrict.TalentAccessory",helper:"AccessoryName")
    }) {
      foreach(var methodName in new[]{"CreateItem","DisplayName"}) {
        var existing=M(patch,entry.type,methodName);
        Need(existing.Body.Instructions.Any(i=>i.Operand is IMethod call && call.FullName.Contains("RedLightDistrict.Korean.UiLateLookup::"+entry.helper)),
          "No late CsvLookup connection in "+entry.type+"."+methodName);
      }
    }
    var tooltipPrefix=M(patch,"RedLightDistrict.TalentPatches","PrefixGetTooltipBasic");
    Need(tooltipPrefix.MethodSig.Params[0].FullName=="GameItemElement","Expected original GameItemElement signature");
    // Prior faulty build inserted GameItem calls on ldarg.0 (GameItemElement). Reject such a build.
    Need(!tooltipPrefix.Body.Instructions.Take(40).Any(i=>i.Operand is IMethod c&&c.FullName=="System.String GameItem::get_identifier()"),
      "Rejected incompatible previous GameItemElement/GameItem IL patch");
    var target=M(patch,"RedLightDistrict.TalentPanel","BuildTalentTooltipBuilder");
    var oldTarget=M(baseline,"RedLightDistrict.TalentPanel","BuildTalentTooltipBuilder");
    var helper=M(patch,"RedLightDistrict.Korean.UiLateLookup","TraitName");
    Need(helper.IsStatic && helper.MethodSig.Params.Count==1&&helper.MethodSig.Params[0].FullName=="System.String"&&helper.ReturnType.FullName=="System.String","Exact preexisting TraitName method");
    var foundKeys=helper.Body.Instructions.Where(i=>i.OpCode==OpCodes.Ldstr&&i.Operand is string v&&v.StartsWith("rld_display_")).Select(i=>(string)i.Operand).ToArray();
    Need(foundKeys.Length==22&&foundKeys.ToHashSet().SetEquals(traits.Select(t=>t.key)),"Existing helper looks up all 22 target CSV keys");
    var status=M(patch,"RedLightDistrict.TalentPanel","ShowPanel");
    Need(status.Body.Instructions.Count(i=>i.Operand is IMethod t&&t.FullName==helper.FullName)==1,"Existing status panel trait lookup must already exist");
    var il=target.Body.Instructions;
    Need(il.Count==813,"Exact tooltip builder shape");
    Need(il[657].Operand is IMethod hidden&&hidden.FullName.StartsWith("System.String RedLightDistrict.Korean.CsvLookup::rld_"),"Hidden trait display rule");
    Need(il[658].Operand==il[660]&&il[659].OpCode==OpCodes.Ldloc_S&&il[659].Operand is Local l&&l.Index==12,"Original trait value at V12");
    Need(il[660].OpCode==OpCodes.Stloc_S&&il[660].Operand is Local v&&v.Index==14,"V14 is display-only");
    Need(il[662].OpCode==OpCodes.Ldloc_S&&((Local)il[662].Operand).Index==12,"Original V12 remains for trait color logic");
    Need(il[664].Operand is IMethod traitColor&&traitColor.FullName.Contains("TalentPanel::TraitColor"),"Original trait color call");
    // Branch for a hidden/unlocked=false trait targets original stloc V14, so it must skip lookup.
    // Unlocked branch falls through newly inserted call and then stloc V14. The internal V12 is untouched.
    const int at=660;
    il.Insert(at,Instruction.Create(OpCodes.Call,helper));
    target.Body.SimplifyBranches();target.Body.OptimizeBranches();
    var beforeMethods=All(baseline).ToDictionary(x=>x.FullName);
    var afterMethods=All(patch).ToDictionary(x=>x.FullName);
    Need(beforeMethods.Count==afterMethods.Count&&beforeMethods.Keys.ToHashSet().SetEquals(afterMethods.Keys),"No methods added or removed");
    foreach(var kv in beforeMethods){
      if(kv.Key==oldTarget.FullName)VerifyExactInsertion(kv.Value,afterMethods[kv.Key],at,helper);
      else Same(kv.Value,afterMethods[kv.Key]);
    }
    Directory.CreateDirectory(Path.GetDirectoryName(args[2])!);
    patch.Write(args[2]);
    using var saved=ModuleDefMD.Load(args[2]);
    var emitted=All(saved).ToDictionary(m=>m.FullName);
    Need(emitted.Count==afterMethods.Count,"Reloaded complete method inventory");
    foreach(var kv in afterMethods)Same(kv.Value,emitted[kv.Key]);
    var originalResources=baseline.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
    var resultResources=saved.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
    Need(originalResources.Count==1617&&resultResources.Count==1617&&originalResources.All(kv=>resultResources.TryGetValue(kv.Key,out var h)&&h==kv.Value),"All embedded resources unchanged");
    Need(baseline.GetAssemblyRefs().Select(x=>x.FullName).ToHashSet().SetEquals(saved.GetAssemblyRefs().Select(x=>x.FullName)),"No new assembly references");
    var report=new{
      inputSha256=Hash(input),outputSha256=Hash(File.ReadAllBytes(args[2])),size=new FileInfo(args[2]).Length,
      methodsAdded=0,methodsChanged=1,methodsUnchanged=beforeMethods.Count-1,
      resourcesVerified=originalResources.Count,statusAlreadyCallsTraitName=true,
      tooltipNowCallsTraitName=true,keysMatched=22,newHarmonyPatches=0,storedTraitIdsUntouched=true,
      lockedTraitsKeepHiddenMarker=true,gameRuntimeTested=false,
      note="Tooltip BuildTalentTooltipBuilder unlocked trait rendering now calls existing UiLateLookup.TraitName(key), which calls CsvLookup.Read against the 22 rld_display_ keys in mod Localization/ko.csv. Status ShowPanel already contained this call. No names or trait mechanics mutated."
    };
    File.WriteAllText(args[3],System.Text.Json.JsonSerializer.Serialize(report,new System.Text.Json.JsonSerializerOptions{WriteIndented=true})+"\n");
    File.WriteAllText(args[4],System.Text.Json.JsonSerializer.Serialize(traits,new System.Text.Json.JsonSerializerOptions{WriteIndented=true})+"\n");
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(report));
  }
}
