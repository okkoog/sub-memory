using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
public sealed class Plan{
 public string expectedDLLSha256{get;set;}="";
 public Row[] equipment{get;set;}=Array.Empty<Row>();
 public Row[] accessory{get;set;}=Array.Empty<Row>();
 public Row[] traits{get;set;}=Array.Empty<Row>();
}
public sealed class Row{
 public string? id{get;set;}
 public string source{get;set;}="";
 public string key{get;set;}="";
}
class P{
 static void Need(bool yes,string s){if(!yes)throw new Exception(s);}
 static string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
 static IEnumerable<MethodDef> All(ModuleDefMD m)=>m.GetTypes().SelectMany(t=>t.Methods);
 static MethodDef M(ModuleDefMD m,string type,string name)=>All(m).Single(x=>x.DeclaringType.FullName==type&&x.Name==name);
 static IList<Instruction> IL(MethodDef m)=>m.Body.Instructions;
 static string Canon(string op)=>op.EndsWith(".s")?op[..^2]:op;
 static string Operand(object? o,Dictionary<Instruction,int> ix)=>o switch{
   null=>"",Instruction i=>"@"+ix[i],IList<Instruction> a=>string.Join(",",a.Select(x=>"@"+ix[x])),_=>o.ToString()??""};
 static Dictionary<Instruction,int> Idx(IList<Instruction> a)=>a.Select((x,i)=>(x,i)).ToDictionary(x=>x.x,x=>x.i);
 static string EH(ExceptionHandler e,Dictionary<Instruction,int> ix){
   string I(Instruction? x)=>x==null?"-":ix[x].ToString();
   return string.Join("|",e.HandlerType.ToString(),I(e.TryStart),I(e.TryEnd),I(e.HandlerStart),I(e.HandlerEnd),I(e.FilterStart),e.CatchType?.FullName??"");
 }
 static void Same(MethodDef a,MethodDef b){
   Need(a.HasBody==b.HasBody,"Body "+a.FullName);if(!a.HasBody)return;
   var x=IL(a);var y=IL(b);Need(x.Count==y.Count,"IL count "+a.FullName);
   var p=Idx(x);var q=Idx(y);
   for(int i=0;i<x.Count;i++){
     Need(Canon(x[i].OpCode.Name)==Canon(y[i].OpCode.Name),"IL opcode "+a.FullName+" @"+i);
     Need(Operand(x[i].Operand,p)==Operand(y[i].Operand,q),"IL operand "+a.FullName+" @"+i);
   }
   Need(a.Body.Variables.Select(l=>l.Type.FullName).SequenceEqual(b.Body.Variables.Select(l=>l.Type.FullName)),"Locals "+a.FullName);
   Need(a.Body.ExceptionHandlers.Select(h=>EH(h,p)).SequenceEqual(b.Body.ExceptionHandlers.Select(h=>EH(h,q))),"EH "+a.FullName);
 }
 static string Source(ModuleDefMD m,string key){
   var method=M(m,"RedLightDistrict.Korean.CsvLookup",key);
   var il=IL(method);
   var n=il.Select((x,i)=>(x,i)).Single(x=>x.x.OpCode==OpCodes.Ldstr&&Equals(x.x.Operand,key)).i;
   Need(n+2<il.Count&&il[n+1].OpCode==OpCodes.Ldstr&&il[n+2].Operand is IMethod meth&&meth.Name=="Read","Wrapper "+key);
   return (string)il[n+1].Operand;
 }
 static void CheckCtor(ModuleDefMD m,string ty,string prefix,Row[] rows,int count){
   var il=IL(M(m,"RedLightDistrict."+ty,".cctor"));
   var dict=new Dictionary<string,string>();
   for(int i=0;i<il.Count-1;i++){
     if(il[i].OpCode!=OpCodes.Ldstr||il[i].Operand is not string id||!id.StartsWith(prefix))continue;
     Need(il[i+1].Operand is IMethod method&&method.DeclaringType.FullName=="RedLightDistrict.Korean.CsvLookup","Ctor wrapper "+id);
     dict.Add(id,((IMethod)il[i+1].Operand).Name.String);
   }
   Need(dict.Count==count&&rows.Length==count,"Ctor array length "+ty);
   foreach(var row in rows){
     Need(row.id!=null&&dict.TryGetValue(row.id,out var key)&&key==row.key,"Ctor key "+row.id);
     Need(Source(m,row.key)==row.source,"Ctor fallback "+row.id);
   }
 }
 static MethodDef AddMethod(ModuleDefMD m,TypeDef owner,string name,int paramCount){
   var args=Enumerable.Repeat(m.CorLibTypes.String,paramCount).ToArray();
   var meth=new MethodDefUser(name,MethodSig.CreateStatic(m.CorLibTypes.String,args),MethodImplAttributes.IL|MethodImplAttributes.Managed,MethodAttributes.Assembly|MethodAttributes.Static|MethodAttributes.HideBySig);
   meth.Body=new CilBody{InitLocals=false,MaxStack=4};owner.Methods.Add(meth);return meth;
 }
 static void Emit(MethodDef m,Row[] rows,IMethod read,IMethod equal,bool trait){
   var il=IL(m);
   foreach(var row in rows){
     var next=Instruction.Create(OpCodes.Nop);
     il.Add(Instruction.Create(OpCodes.Ldarg_0));
     il.Add(Instruction.Create(OpCodes.Ldstr,trait?row.source:row.id!));
     il.Add(Instruction.Create(OpCodes.Call,equal));
     il.Add(Instruction.Create(OpCodes.Brfalse,next));
     il.Add(Instruction.Create(OpCodes.Ldstr,row.key));
     il.Add(Instruction.Create(trait?OpCodes.Ldarg_0:OpCodes.Ldarg_1));
     il.Add(Instruction.Create(OpCodes.Call,read));
     il.Add(Instruction.Create(OpCodes.Ret));
     il.Add(next);
   }
   il.Add(Instruction.Create(trait?OpCodes.Ldarg_0:OpCodes.Ldarg_1));
   il.Add(Instruction.Create(OpCodes.Ret));
   m.Body.SimplifyBranches();m.Body.OptimizeBranches();
 }
 static void DisplayName(ModuleDefMD m,string type,string def,MethodDef helper){
   var meth=M(m,"RedLightDistrict."+type,"DisplayName");var il=IL(meth);
   Need(il.Count==10&&il[4].Operand==il[7]&&il[8].OpCode==OpCodes.Ldfld&&((IField)il[8].Operand).FullName=="System.String RedLightDistrict."+type+"/"+def+"::Name","Display name anchor "+type);
   var a=Instruction.Create(OpCodes.Ldarg_0);il.Insert(7,a);il[4].Operand=a;
   il.Insert(il.IndexOf(a)+3,Instruction.Create(OpCodes.Call,helper));
   meth.Body.SimplifyBranches();meth.Body.OptimizeBranches();
 }
 static void CreateItem(ModuleDefMD m,string type,MethodDef helper){
   var meth=M(m,"RedLightDistrict."+type,"CreateItem");var il=IL(meth);
   int at=il.Select((x,i)=>(x,i)).Single(x=>x.x.Operand is IMethod call&&call.FullName=="GameItem GameItem::SetName(System.String)").i;
   Need(il[at-1].OpCode==OpCodes.Ldarg_1&&il[at-2].OpCode==OpCodes.Ldloc_0,"CreateItem name anchor "+type);
   var next=il[at-1];
   Need(!il.Any(x=>x.Operand is Instruction y&&y==next),"Branch into name arg "+type);
   il.Insert(at-1,Instruction.Create(OpCodes.Ldarg_0));
   il.Insert(at+1,Instruction.Create(OpCodes.Call,helper));
   meth.Body.SimplifyBranches();meth.Body.OptimizeBranches();
 }
 static void TraitUi(ModuleDefMD m,MethodDef helper){
   var meth=M(m,"RedLightDistrict.TalentPanel","ShowPanel");var il=IL(meth);
   Need(il[711].Operand is IMethod call&&call.FullName.Contains("get_Current")&&il[713].OpCode==OpCodes.Ldloc_S,"Trait source");
   Need(il[732].OpCode==OpCodes.Ldloc_S&&Equals(il[732].Operand,il[713].Operand)&&il[733].OpCode==OpCodes.Stloc_S,"Untranslated trait UI assignment");
   // Internal V_19 remains Chinese for HasTrait/UnlockedTraits/Color, only V_21 display is localized.
   il.Insert(733,Instruction.Create(OpCodes.Call,helper));
   meth.Body.SimplifyBranches();meth.Body.OptimizeBranches();
 }
 static void Tooltip(ModuleDefMD m){
   var meth=M(m,"RedLightDistrict.TalentPatches","TryOwnTooltipLines");var il=IL(meth);
   var equipment=M(m,"RedLightDistrict.TalentEquipment","DisplayName");
   var accessory=M(m,"RedLightDistrict.TalentAccessory","DisplayName");
   foreach(var x in new[]{(a:21,b:22,s:"System.String RedLightDistrict.TalentEquipment/EquipDef::Name",call:equipment),(a:65,b:66,s:"System.String RedLightDistrict.TalentAccessory/AccessoryDef::Name",call:accessory)}){
     Need(il[x.b].OpCode==OpCodes.Ldfld&&((IField)il[x.b].Operand).FullName==x.s,"Tooltip field "+x.s);
     Need(il[x.a].OpCode==OpCodes.Ldloc_0||il[x.a].OpCode==OpCodes.Ldloc_1,"Tooltip source "+x.s);
     il[x.a].OpCode=OpCodes.Ldarg_0;il[x.a].Operand=null;
     il[x.b].OpCode=OpCodes.Call;il[x.b].Operand=x.call;
   }
   meth.Body.SimplifyBranches();meth.Body.OptimizeBranches();
 }
 static void Main(string[] a){
   var raw=File.ReadAllBytes(a[0]);var plan=JsonSerializer.Deserialize<Plan>(File.ReadAllText(a[1]))!;
   Need(Hash(raw)==plan.expectedDLLSha256,"Input DLL hash changed");
   using var old=ModuleDefMD.Load(raw);using var now=ModuleDefMD.Load(raw);
   Need(old.Resources.Count==1617,"Resource count");
   Need(!now.GetTypes().Any(x=>x.FullName=="RedLightDistrict.Korean.UiLateLookup"),"Already patched");
   CheckCtor(now,"TalentEquipment","redlight_clothing_",plan.equipment,15);
   CheckCtor(now,"TalentAccessory","redlight_accessory_",plan.accessory,4);
   var sourceTraits=IL(M(now,"RedLightDistrict.TalentData",".cctor")).Where(i=>i.Operand is IMethod x&&x.DeclaringType.FullName=="RedLightDistrict.Korean.CsvLookup"&&x.Name.String.StartsWith("rld_")).Select(i=>Source(now,((IMethod)i.Operand).Name.String)).ToHashSet();
   Need(sourceTraits.Count==22&&sourceTraits.SetEquals(plan.traits.Select(x=>x.source))&&plan.traits.All(x=>x.key.StartsWith("rld_display_")),"Trait map");
   var read=M(now,"RedLightDistrict.Korean.CsvLookup","Read");
   var equal=All(now).Where(x=>x.HasBody).SelectMany(x=>x.Body.Instructions).Select(i=>i.Operand).OfType<IMethod>().First(x=>x.FullName=="System.Boolean System.String::op_Equality(System.String,System.String)");
   var ty=new TypeDefUser("RedLightDistrict.Korean","UiLateLookup",now.CorLibTypes.Object.TypeDefOrRef){Attributes=TypeAttributes.NotPublic|TypeAttributes.Abstract|TypeAttributes.Sealed|TypeAttributes.BeforeFieldInit};
   now.Types.Add(ty);
   var e=AddMethod(now,ty,"EquipmentName",2), c=AddMethod(now,ty,"AccessoryName",2), t=AddMethod(now,ty,"TraitName",1);
   Emit(e,plan.equipment,read,equal,false);Emit(c,plan.accessory,read,equal,false);Emit(t,plan.traits,read,equal,true);
   var changed=new[]{
     M(now,"RedLightDistrict.TalentEquipment","DisplayName").FullName,
     M(now,"RedLightDistrict.TalentAccessory","DisplayName").FullName,
     M(now,"RedLightDistrict.TalentEquipment","CreateItem").FullName,
     M(now,"RedLightDistrict.TalentAccessory","CreateItem").FullName,
     M(now,"RedLightDistrict.TalentPanel","ShowPanel").FullName,
     M(now,"RedLightDistrict.TalentPatches","TryOwnTooltipLines").FullName
   }.ToHashSet();
   DisplayName(now,"TalentEquipment","EquipDef",e);DisplayName(now,"TalentAccessory","AccessoryDef",c);
   CreateItem(now,"TalentEquipment",e);CreateItem(now,"TalentAccessory",c);
   TraitUi(now,t);Tooltip(now);
   var om=All(old).ToDictionary(x=>x.FullName),nm=All(now).ToDictionary(x=>x.FullName);
   Need(nm.Count==om.Count+3&&nm.Keys.Except(om.Keys).ToHashSet().SetEquals(ty.Methods.Select(x=>x.FullName)),"Method inventory");
   foreach(var x in om.Where(x=>!changed.Contains(x.Key)))Same(x.Value,nm[x.Key]);
   Directory.CreateDirectory(Path.GetDirectoryName(a[2])!);now.Write(a[2]);
   using var emitted=ModuleDefMD.Load(a[2]);
   var resultMethods=All(emitted).ToDictionary(x=>x.FullName);
   Need(resultMethods.Count==nm.Count,"Emitted method count");
   foreach(var x in nm)Same(x.Value,resultMethods[x.Key]);
   var before=old.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.ToString(),x=>Hash(x.CreateReader().ToArray()));
   var after=emitted.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.ToString(),x=>Hash(x.CreateReader().ToArray()));
   Need(before.Count==after.Count&&before.All(x=>after.TryGetValue(x.Key,out var v)&&v==x.Value),"Embedded resource mismatch");
   var refs=old.GetAssemblyRefs().Select(x=>x.FullName).ToHashSet();
   Need(refs.SetEquals(emitted.GetAssemblyRefs().Select(x=>x.FullName)),"Assembly references drift");
   var report=new{originalSha256=Hash(raw),patchedSha256=Hash(File.ReadAllBytes(a[2])),originalMethods=om.Count,addedMethods=3,modifiedMethods=changed.OrderBy(x=>x),equipment=15,accessories=4,traits=22,resourcesVerified=before.Count,unmodifiedMethodsVerified=om.Count-changed.Count,allOutputMethodsVerified=true,runtimeTested=false,note="Existing saved clothing item names are not retrospectively rewritten"};
   File.WriteAllText(a[3],JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true})+"\n");
   Console.WriteLine(JsonSerializer.Serialize(report));
 }
}
