using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

sealed class Row { public string id {get;set;} = ""; public string key {get;set;} = ""; public string source {get;set;} = ""; }
sealed class Plan { public Row[] equipment {get;set;} = Array.Empty<Row>(); public Row[] accessory {get;set;}=Array.Empty<Row>(); }
static class P {
    static void Need(bool yes,string message) { if(!yes) throw new Exception(message); }
    static string Hash(byte[] data) => Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
    static IEnumerable<MethodDef> All(ModuleDefMD m) => m.GetTypes().SelectMany(t=>t.Methods);
    static MethodDef M(ModuleDefMD m,string t,string method) => All(m).Single(x=>x.DeclaringType.FullName==t&&x.Name==method);
    static IMethod FindReference(ModuleDefMD m,string fullName) => All(m).Where(x=>x.HasBody).SelectMany(x=>x.Body.Instructions).Select(i=>i.Operand).OfType<IMethod>().First(x=>x.FullName==fullName);
    static string Operand(object? o,Dictionary<Instruction,int> indexes) => o switch {
       null => "", Instruction x => "@"+indexes[x], IList<Instruction> x=>string.Join(",",x.Select(y=>"@"+indexes[y])), _=>o.ToString()??""
    };
    static void Same(MethodDef a,MethodDef b) {
       Need(a.HasBody==b.HasBody,"body difference "+a.FullName); if(!a.HasBody)return;
       var x=a.Body.Instructions;var y=b.Body.Instructions;
       Need(x.Count==y.Count,"IL count "+a.FullName);
       var ix=x.Select((v,j)=>(v,j)).ToDictionary(z=>z.v,z=>z.j);
       var iy=y.Select((v,j)=>(v,j)).ToDictionary(z=>z.v,z=>z.j);
       for(int j=0;j<x.Count;j++) {
          Need(x[j].OpCode.Name.Replace(".s","")==y[j].OpCode.Name.Replace(".s",""),"opcode "+a.FullName+" "+j);
          Need(Operand(x[j].Operand,ix)==Operand(y[j].Operand,iy),"operand "+a.FullName+" "+j);
       }
       Need(a.Body.Variables.Select(z=>z.Type.FullName).SequenceEqual(b.Body.Variables.Select(z=>z.Type.FullName)),"local types "+a.FullName);
       static string Eh(ExceptionHandler e,Dictionary<Instruction,int> d){
         string I(Instruction? t)=>t==null?"-":d[t].ToString();
         return string.Join("|",e.HandlerType.ToString(),I(e.TryStart),I(e.TryEnd),I(e.HandlerStart),I(e.HandlerEnd),I(e.FilterStart),e.CatchType?.FullName??"");
       }
       Need(a.Body.ExceptionHandlers.Select(e=>Eh(e,ix)).SequenceEqual(b.Body.ExceptionHandlers.Select(e=>Eh(e,iy))),"EH "+a.FullName);
    }
    static MethodDef NewMethod(TypeDef type,string name,MethodSig sig, bool visible=false) {
       var m = new MethodDefUser(name,sig,MethodImplAttributes.IL|MethodImplAttributes.Managed,
            (visible?MethodAttributes.Public:MethodAttributes.Assembly)|MethodAttributes.Static|MethodAttributes.HideBySig);
       m.Body=new CilBody{InitLocals=true,MaxStack=8};
       type.Methods.Add(m);
       return m;
    }
    static void Main(string[] a){
       Need(a.Length==5,"usage: prior.dll plan.json game.dll new.dll report.json");
       var raw=File.ReadAllBytes(a[0]);const string expected="86ad3f461f263dd04199edf8dad7e8c3f2e017b0c8a013462eef8034733bda47";
       Need(Hash(raw)==expected,"Input previous UI patched DLL hash mismatch");
       var plan=JsonSerializer.Deserialize<Plan>(File.ReadAllText(a[1]))!;
       var rows=plan.equipment.Concat(plan.accessory).ToArray();
       Need(plan.equipment.Length==15&&plan.accessory.Length==4&&rows.Select(x=>x.id).Distinct().Count()==19,"19 exact IDs");
       using var game=ModuleDefMD.Load(a[2]);
       var g=game.GetTypes().Single(t=>t.FullName=="GameItem").Methods.Single(m=>m.Name=="GetDisplayName");
       Need(g.MethodSig.Params.Count==1 && g.MethodSig.Params[0].FullName=="System.Boolean" && g.ReturnType.FullName=="System.String","Target GameItem.GetDisplayName(bool) signature");
       var element=game.GetTypes().Single(t=>t.FullName=="GameItemElement").Methods.Single(m=>m.Name=="GetTooltipBasic");
       Need(element.Body.Instructions.Count(i=>i.Operand is IMethod call&&call.FullName==g.FullName)==2,"Game inventory title must read GetDisplayName twice");
       using var old=ModuleDefMD.Load(raw);using var mod=ModuleDefMD.Load(raw);
       var tp=mod.GetTypes().Single(t=>t.FullName=="RedLightDistrict.TalentPatches");
       var apply=tp.Methods.Single(t=>t.Name=="Apply"&&t.MethodSig.Params.Count==1);
       Need(apply.MethodSig.Params[0].FullName=="HarmonyLib.Harmony"&&apply.HasBody,"Actual Harmony Apply method");
       Need(!tp.Methods.Any(t=>t.Name=="InstallKoreanTitlePostfix"||t.Name=="PostfixKoreanItemDisplayName"),"Already patched");
       var read=M(mod,"RedLightDistrict.Korean.CsvLookup","Read");
       Need(read.MethodSig.Params.Count==2&&read.MethodSig.RetType.FullName=="System.String","Existing CSV bridge");
       var getId=FindReference(mod,"System.String GameItem::get_identifier()");
       var equal=FindReference(mod,"System.Boolean System.String::op_Equality(System.String,System.String)");
       var typeFromHandle=FindReference(mod,"System.Type System.Type::GetTypeFromHandle(System.RuntimeTypeHandle)");
       var access=FindReference(mod,"System.Reflection.MethodInfo HarmonyLib.AccessTools::Method(System.Type,System.String,System.Type[],System.Type[])");
       var harmonyCtor=FindReference(mod,"System.Void HarmonyLib.HarmonyMethod::.ctor(System.Type,System.String,System.Type[])");
       var safePatch=tp.Methods.Single(m=>m.Name=="SafePatch"&&m.MethodSig.Params.Count==5);
       var postfix=NewMethod(tp,"PostfixKoreanItemDisplayName",MethodSig.CreateStatic(mod.CorLibTypes.Void,getId.DeclaringType.ToTypeSig(),new ByRefSig(mod.CorLibTypes.String)),true);
       postfix.ParamDefs.Add(new ParamDefUser("__instance",(ushort)1));
       postfix.ParamDefs.Add(new ParamDefUser("__result",(ushort)2));
       var code=postfix.Body.Instructions;
       var end=Instruction.Create(OpCodes.Ret);
       code.Add(Instruction.Create(OpCodes.Ldarg_0));
       code.Add(Instruction.Create(OpCodes.Brfalse,end));
       code.Add(Instruction.Create(OpCodes.Ldarg_1));
       code.Add(Instruction.Create(OpCodes.Ldind_Ref));
       code.Add(Instruction.Create(OpCodes.Brfalse,end));
       var idLocal=new Local(mod.CorLibTypes.String);
       postfix.Body.Variables.Add(idLocal);
       code.Add(Instruction.Create(OpCodes.Ldarg_0));
       code.Add(Instruction.Create(OpCodes.Callvirt,getId));
       code.Add(Instruction.Create(OpCodes.Stloc_0));
       code.Add(Instruction.Create(OpCodes.Ldloc_0));
       code.Add(Instruction.Create(OpCodes.Brfalse,end));
       foreach(var row in rows){
          var next=Instruction.Create(OpCodes.Nop);
          code.Add(Instruction.Create(OpCodes.Ldloc_0));
          code.Add(Instruction.Create(OpCodes.Ldstr,row.id));
          code.Add(Instruction.Create(OpCodes.Call,equal));
          code.Add(Instruction.Create(OpCodes.Brfalse,next));
          code.Add(Instruction.Create(OpCodes.Ldarg_1));
          code.Add(Instruction.Create(OpCodes.Ldind_Ref));
          code.Add(Instruction.Create(OpCodes.Ldstr,row.source));
          code.Add(Instruction.Create(OpCodes.Call,equal));
          code.Add(Instruction.Create(OpCodes.Brfalse,next));
          code.Add(Instruction.Create(OpCodes.Ldarg_1));
          code.Add(Instruction.Create(OpCodes.Ldstr,row.key));
          code.Add(Instruction.Create(OpCodes.Ldstr,row.source));
          code.Add(Instruction.Create(OpCodes.Call,read));
          code.Add(Instruction.Create(OpCodes.Stind_Ref));
          code.Add(Instruction.Create(OpCodes.Ret));
          code.Add(next);
       }
       code.Add(end);
       postfix.Body.SimplifyBranches();postfix.Body.OptimizeBranches();
       var install=NewMethod(tp,"InstallKoreanTitlePostfix",MethodSig.CreateStatic(mod.CorLibTypes.Void,apply.MethodSig.Params[0]));
       var body=install.Body.Instructions;
       // SafePatch(harmony, GetDisplayName(bool), prefix:null, postfix:HarmonyMethod, transpiler:null)
       body.Add(Instruction.Create(OpCodes.Ldarg_0));
       body.Add(Instruction.Create(OpCodes.Ldtoken,getId.DeclaringType));
       body.Add(Instruction.Create(OpCodes.Call,typeFromHandle));
       body.Add(Instruction.Create(OpCodes.Ldstr,"GetDisplayName"));
       body.Add(Instruction.Create(OpCodes.Ldc_I4_1));
       body.Add(Instruction.Create(OpCodes.Newarr,typeFromHandle.DeclaringType));
       body.Add(Instruction.Create(OpCodes.Dup));
       body.Add(Instruction.Create(OpCodes.Ldc_I4_0));
       body.Add(Instruction.Create(OpCodes.Ldtoken,mod.CorLibTypes.Boolean.TypeDefOrRef));
       body.Add(Instruction.Create(OpCodes.Call,typeFromHandle));
       body.Add(Instruction.Create(OpCodes.Stelem_Ref));
       body.Add(Instruction.Create(OpCodes.Ldnull));
       body.Add(Instruction.Create(OpCodes.Call,access));
       body.Add(Instruction.Create(OpCodes.Ldnull));
       body.Add(Instruction.Create(OpCodes.Ldtoken,tp));
       body.Add(Instruction.Create(OpCodes.Call,typeFromHandle));
       body.Add(Instruction.Create(OpCodes.Ldstr,"PostfixKoreanItemDisplayName"));
       body.Add(Instruction.Create(OpCodes.Ldnull));
       body.Add(Instruction.Create(OpCodes.Newobj,harmonyCtor));
       body.Add(Instruction.Create(OpCodes.Ldnull));
       body.Add(Instruction.Create(OpCodes.Call,safePatch));
       body.Add(Instruction.Create(OpCodes.Ret));
       // Install using the SAME Harmony instance as existing RLD patches.
       apply.Body.Instructions.Insert(0,Instruction.Create(OpCodes.Ldarg_0));
       apply.Body.Instructions.Insert(1,Instruction.Create(OpCodes.Call,install));
       apply.Body.SimplifyBranches();apply.Body.OptimizeBranches();
       var om=All(old).ToDictionary(x=>x.FullName),nm=All(mod).ToDictionary(x=>x.FullName);
       Need(nm.Count==om.Count+2,"Old method inventory plus two new helpers");
       foreach(var m in om.Where(x=>x.Key!=apply.FullName))Same(m.Value,nm[m.Key]);
       Need(apply.Body.Instructions.Count==om[apply.FullName].Body.Instructions.Count+2,"Only two installation instructions added");
       Need(om[apply.FullName].Body.Instructions[0].OpCode==apply.Body.Instructions[2].OpCode,"Apply old entry kept");
       Directory.CreateDirectory(Path.GetDirectoryName(a[3])!);
       mod.Write(a[3]);
       using var emitted=ModuleDefMD.Load(a[3]);
       var outMethods=All(emitted).ToDictionary(x=>x.FullName);
       Need(outMethods.Count==nm.Count,"Reopened method inventory");
       foreach(var m in nm)Same(m.Value,outMethods[m.Key]);
       var resourcesBefore=old.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
       var resourcesAfter=emitted.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
       Need(resourcesBefore.Count==1617&&resourcesAfter.Count==1617&&resourcesBefore.All(x=>resourcesAfter.TryGetValue(x.Key,out var hash)&&x.Value==hash),"All 1617 embedded resource bytes");
       Need(emitted.GetTypes().Single(t=>t.FullName=="RedLightDistrict.TalentPatches").Methods.Count(t=>t.Name=="PostfixKoreanItemDisplayName"||t.Name=="InstallKoreanTitlePostfix")==2,"Both methods emitted");
       var output=File.ReadAllBytes(a[3]);
       var report=new {
          baselineSha256=Hash(raw),patchedSha256=Hash(output),outputSize=output.Length,
          verifiedGameMethod=g.FullName,tooltipTitleCallSites=2,patchMethod=apply.FullName,
          addedMethods=new[]{install.FullName,postfix.FullName},nameRowCount=rows.Length,
          changedExistingMethods=1,unchangedExistingMethodsVerified=om.Count-1,
          resourcesVerified=resourcesBefore.Count,allMethodsReopenedAndCompared=true,
          identityAndSavedNameRemainUntouched=true,runtimeTested=false,
          behavior="Display-only: if ID is one of 19 RLD clothing/accessory IDs and display result equals original Chinese, look up live ko.csv key. No item fields or save data modified."
       };
       File.WriteAllText(a[4],JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true})+"\n");
       Console.WriteLine(JsonSerializer.Serialize(report));
    }
}
