using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
class P {
 static void Need(bool condition,string error){if(!condition)throw new Exception(error);}
 static string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant();
 static string Op(object? o,Dictionary<Instruction,int> map)=>o switch{null=>"",Instruction x=>"@"+map[x],IList<Instruction> a=>string.Join(",",a.Select(x=>"@"+map[x])),_=>o.ToString()??""};
 static Dictionary<Instruction,int> Index(IList<Instruction> l)=>l.Select((v,i)=>(v,i)).ToDictionary(x=>x.v,x=>x.i);
 static string EH(ExceptionHandler e,Dictionary<Instruction,int> i){
    string I(Instruction? t)=>t==null?"-":i[t].ToString();
    return string.Join("|",e.HandlerType,I(e.TryStart),I(e.TryEnd),I(e.HandlerStart),I(e.HandlerEnd),I(e.FilterStart),e.CatchType?.FullName??"");
 }
 static IEnumerable<MethodDef> Methods(ModuleDefMD m)=>m.GetTypes().SelectMany(t=>t.Methods);
 static void Main(string[] args){
    // 0 actual DLL, 1 source catalog alias mapping JSON, 2 output DLL, 3 report
    byte[] raw=File.ReadAllBytes(args[0]);
    Need(Hash(raw)=="a224760a2ae651e07adb467014dbb0053d1c6b8942879644b2afbad614237f25","Unexpected actual DLL baseline");
    using var original=ModuleDefMD.Load(raw);using var mod=ModuleDefMD.Load(raw);
    var equip=mod.GetTypes().Single(t=>t.FullName=="RedLightDistrict.TalentEquipment");
    var panel=mod.GetTypes().Single(t=>t.FullName=="RedLightDistrict.TalentPanel");
    var csvLookup=mod.GetTypes().Single(t=>t.FullName=="RedLightDistrict.Korean.CsvLookup");
    var read=csvLookup.Methods.Single(m=>m.Name=="Read"&&m.MethodSig.Params.Count==2);
    var cctor=equip.Methods.Single(m=>m.Name==".cctor");
    var clothing=new List<(string id,MethodDef lookup)>();
    var cil=cctor.Body.Instructions;
    for(int i=0;i<cil.Count-1;i++){
       if(cil[i].OpCode!=OpCodes.Ldstr||cil[i].Operand is not string id||!id.StartsWith("redlight_clothing_"))continue;
       Need(cil[i+1].OpCode==OpCodes.Call&&cil[i+1].Operand is IMethod method&&method.DeclaringType.FullName==csvLookup.FullName,"Unexpected original outfit name site "+id);
       var wrapper=csvLookup.Methods.Single(m=>m.FullName==((IMethod)cil[i+1].Operand).FullName);
       Need(wrapper.MethodSig.Params.Count==0&&wrapper.MethodSig.RetType.FullName=="System.String","Outfit name wrapper type");
       clothing.Add((id,wrapper));
    }
    Need(clothing.Count==15&&clothing.Select(x=>x.id).Distinct().Count()==15,"Outfit catalog count/IDs");
    var traits=JsonSerializer.Deserialize<Dictionary<string,string>>(File.ReadAllText(args[1]))!;
    Need(traits.Count==22&&traits.Keys.Distinct().Count()==22&&traits.Values.Distinct().Count()==22,"Trait alias count/uniqueness");
    Need(traits.Values.All(x=>x.StartsWith("rld_display_")),"Trait alias prefix");
    var type=new TypeDefUser("RedLightDistrict.Korean","ClothingTraitDisplay",mod.CorLibTypes.Object.TypeDefOrRef) {
      Attributes=TypeAttributes.NotPublic|TypeAttributes.Abstract|TypeAttributes.Sealed|TypeAttributes.BeforeFieldInit
    };
    mod.Types.Add(type);
    var eq=new MemberRefUser(mod,"op_Equality",MethodSig.CreateStatic(mod.CorLibTypes.Boolean,mod.CorLibTypes.String,mod.CorLibTypes.String),mod.CorLibTypes.String.TypeDefOrRef);
    var clothingName=new MethodDefUser("ClothingName",MethodSig.CreateStatic(mod.CorLibTypes.String,mod.CorLibTypes.String,mod.CorLibTypes.String),
     MethodImplAttributes.IL|MethodImplAttributes.Managed,MethodAttributes.Assembly|MethodAttributes.Static|MethodAttributes.HideBySig);
    var traitName=new MethodDefUser("TraitName",MethodSig.CreateStatic(mod.CorLibTypes.String,mod.CorLibTypes.String),
     MethodImplAttributes.IL|MethodImplAttributes.Managed,MethodAttributes.Assembly|MethodAttributes.Static|MethodAttributes.HideBySig);
    type.Methods.Add(clothingName);type.Methods.Add(traitName);
    clothingName.Body=new CilBody{MaxStack=2};
    foreach(var (id,lookup) in clothing){
      var next=Instruction.Create(OpCodes.Nop);
      clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
      clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr,id));
      clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Call,eq));
      clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse,next));
      clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Call,lookup));
      clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
      clothingName.Body.Instructions.Add(next);
    }
    clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
    clothingName.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
    traitName.Body=new CilBody{MaxStack=2};
    foreach(var (originalName,alias) in traits){
      var next=Instruction.Create(OpCodes.Nop);
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr,originalName));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Call,eq));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Brfalse,next));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldstr,alias));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Call,read));
      traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
      traitName.Body.Instructions.Add(next);
    }
    traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
    traitName.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
    clothingName.Body.SimplifyBranches();clothingName.Body.OptimizeBranches();
    traitName.Body.SimplifyBranches();traitName.Body.OptimizeBranches();

    // Preserve base IL exactly. All additions occur at direct UI output sites.
    var changes=new Dictionary<string,SortedDictionary<int,List<Instruction>>>();
    void Add(string name, int pos, Action<Instruction> anchor,params Instruction[] added){
       var target=Methods(mod).Single(m=>m.FullName==name);
       Need(pos>=0&&pos<target.Body.Instructions.Count,"Bad site index "+name);
       anchor(target.Body.Instructions[pos]);
       if(!changes.ContainsKey(name))changes.Add(name,new SortedDictionary<int,List<Instruction>>());
       Need(!changes[name].ContainsKey(pos),"Duplicate site");
       changes[name].Add(pos,added.ToList());
    }
    var create=Methods(mod).Single(x=>x.DeclaringType.FullName==equip.FullName&&x.Name=="CreateItem");
    var display=Methods(mod).Single(x=>x.DeclaringType.FullName==equip.FullName&&x.Name=="DisplayName");
    var tooltip=Methods(mod).Single(x=>x.DeclaringType.FullName==panel.FullName&&x.Name=="BuildEquipmentTooltip");
    var show=Methods(mod).Single(x=>x.DeclaringType.FullName==panel.FullName&&x.Name=="ShowPanel");
    var talentTip=Methods(mod).Single(x=>x.DeclaringType.FullName==panel.FullName&&x.Name=="BuildTalentTooltipBuilder");
    void OpIs(Instruction i,string op,string contains)=>Need(i.OpCode.Name==op&&i.Operand?.ToString()?.Contains(contains)==true,"Unexpected UI output anchor "+op+" "+contains);
    // New clothing items: preserve identifier; translate only name argument to SetName.
    Add(create.FullName,20,i=>OpIs(i,"callvirt","GameItem::SetName"),Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Call,clothingName));
    // Existing displayed equipped clothing: name is resolved by stable garment ID on each view.
    Add(display.FullName,9,i=>Need(i.OpCode==OpCodes.Ret,"Outfit DisplayName return"),Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Call,clothingName));
    // Tooltip of saved clothing: translate local displayed copy by item ID without touching stored name.
    var ident=tooltip.Body.Instructions.Single(i=>i.Operand is IMethod m&&m.FullName=="System.String GameItem::get_identifier()");
    Add(tooltip.FullName,12,i=>Need(i.OpCode.Name.StartsWith("stloc"),"Equipment tooltip display sink"),Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Callvirt,(IMethod)ident.Operand),Instruction.Create(OpCodes.Call,clothingName));
    // Trait entries: display the aliases, not the internal strings used by Contains/HasTrait/TraitColor.
    Add(show.FullName,748,i=>Need(i.OpCode.Name.StartsWith("ldloc")&&i.Operand?.ToString()=="V_22","Talent panel trait colour sink"),Instruction.Create(OpCodes.Call,traitName));
    Add(talentTip.FullName,683,i=>Need(i.OpCode.Name.StartsWith("ldloc")&&i.Operand?.ToString()=="V_15","Talent tooltip trait colour sink"),Instruction.Create(OpCodes.Call,traitName));

    foreach(var (methodName,insert) in changes){
       var m=Methods(mod).Single(x=>x.FullName==methodName);var orig=m.Body.Instructions.ToArray();
       foreach(var (position,added) in insert.Reverse()){
         var at=m.Body.Instructions.IndexOf(orig[position]);
         Need(at>=0,"Lost anchor "+methodName);
         for(int i=0;i<added.Count;i++)m.Body.Instructions.Insert(at+i,added[i]);
       }
       m.Body.SimplifyBranches();m.Body.OptimizeBranches();
    }
    Directory.CreateDirectory(Path.GetDirectoryName(args[2])!);
    mod.Write(args[2]);
    using var emitted=ModuleDefMD.Load(args[2]);
    var before=Methods(original).GroupBy(x=>x.FullName).ToDictionary(g=>g.Key,g=>g.OrderBy(x=>x.MDToken.Raw).ToArray());
    var after=Methods(emitted).GroupBy(x=>x.FullName).ToDictionary(g=>g.Key,g=>g.OrderBy(x=>x.MDToken.Raw).ToArray());
    Need(after.Values.Sum(x=>x.Length)==before.Values.Sum(x=>x.Length)+2&&before.All(x=>after.TryGetValue(x.Key,out var y)&&x.Value.Length==y.Length),"Method inventory drift");
    int verifiedMethods=0,verifiedInstructions=0,verifiedInserted=0;
    foreach(var (name,methods) in before)for(int k=0;k<methods.Length;k++){
       var o=methods[k];var n=after[name][k];
       Need(o.HasBody==n.HasBody,"Body changed "+name);if(!o.HasBody)continue;
       var i=o.Body.Instructions;var j=n.Body.Instructions;
       var oldIdx=Index(i);var mappedIdx=new Dictionary<Instruction,int>();
       changes.TryGetValue(name,out var edits);
       int expected=i.Count+(edits?.Sum(x=>x.Value.Count)??0);
       Need(j.Count==expected,"Instruction count "+name);
       int at=0;
       for(int pos=0;pos<=i.Count;pos++){
          if(edits!=null&&edits.TryGetValue(pos,out var inserted)){
             foreach(var x in inserted){
                var y=j[at++];
                Need(x.OpCode.Name.Replace(".s","")==y.OpCode.Name.Replace(".s","")&&x.Operand?.ToString()==y.Operand?.ToString(),"Unexpected new instruction "+name);
                mappedIdx[y]=pos;verifiedInserted++;
             }
          }
          if(pos<i.Count){mappedIdx[j[at]]=pos;at++;}
       }
       Need(at==j.Count,"Instruction mapping "+name);
       Need(o.Body.Variables.Select(x=>x.Type.FullName).SequenceEqual(n.Body.Variables.Select(x=>x.Type.FullName)),"Locals changed "+name);
       Need(o.Body.ExceptionHandlers.Select(x=>EH(x,oldIdx)).SequenceEqual(n.Body.ExceptionHandlers.Select(x=>EH(x,mappedIdx))),"Exception handler drift "+name);
       at=0;
       for(int pos=0;pos<i.Count;pos++){
          if(edits!=null&&edits.TryGetValue(pos,out var inserted))at+=inserted.Count;
          var x=i[pos];var y=j[at++];
          Need(x.OpCode.Name.Replace(".s","")==y.OpCode.Name.Replace(".s","")&&Op(x.Operand,oldIdx)==Op(y.Operand,mappedIdx),"Unintended IL change "+name+" @"+pos);
          verifiedInstructions++;
       }
       verifiedMethods++;
    }
    var a=original.GetTypes().Select(x=>x.FullName).OrderBy(x=>x).ToArray();
    var b=emitted.GetTypes().Select(x=>x.FullName).OrderBy(x=>x).ToArray();
    Need(b.SequenceEqual(a.Append(type.FullName).OrderBy(x=>x)),"Type list drift");
    var resourcesA=original.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
    var resourcesB=emitted.Resources.OfType<EmbeddedResource>().ToDictionary(x=>x.Name.String,x=>Hash(x.CreateReader().ToArray()));
    Need(resourcesA.Count==resourcesB.Count&&resourcesA.All(x=>resourcesB.GetValueOrDefault(x.Key)==x.Value),"Resource changes");
    Need(original.GetAssemblyRefs().Select(x=>x.FullName).OrderBy(x=>x).SequenceEqual(emitted.GetAssemblyRefs().Select(x=>x.FullName).OrderBy(x=>x)),"Assembly refs changed");
    Need(verifiedInserted==10,"Inserted instruction count "+verifiedInserted);
    var report=new{status="verified-static",baseSHA=Hash(raw),patchedSHA=Hash(File.ReadAllBytes(args[2])),clothingMappings=clothing.Select(x=>new{x.id,method=x.lookup.Name.String}),traits=traits.Count,patchSites=changes.Sum(x=>x.Value.Count),insertedInstructions=verifiedInserted,verifiedMethods,verifiedOriginalInstructions=verifiedInstructions,embeddedResources=resourcesA.Count,originalKeysPreserved=true,externalKoCsvUnchanged=true,gameTested=false};
    File.WriteAllText(args[3],JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true})+"\n");
    Console.WriteLine(JsonSerializer.Serialize(new{report.status,report.clothingMappings,report.traits,report.patchSites,report.verifiedMethods,report.verifiedOriginalInstructions}));
 }
}