using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;
class Patch {
 static string Un(string s) => s.Replace("\\n","\n").Replace("\\t","\t").Replace("\\r","\r").Replace("\\\"","\"").Replace("\\\\","\\");
 static IEnumerable<TypeDefinition> Types(TypeDefinition t){yield return t;foreach(var n in t.NestedTypes)foreach(var x in Types(n))yield return x;}
 static MethodDefinition Add(TypeDefinition t,string name,TypeReference ret,params TypeReference[] pars){
  var m=new MethodDefinition(name,Mono.Cecil.MethodAttributes.Assembly|Mono.Cecil.MethodAttributes.Static,ret);
  foreach(var p in pars)m.Parameters.Add(new ParameterDefinition(p));t.Methods.Add(m);return m;
 }
 static void Prefix(MethodDefinition m, params Instruction[] inst){var il=m.Body.GetILProcessor();var at=m.Body.Instructions[0];foreach(var x in inst)il.InsertBefore(at,x);}
 static void Widen(MethodBody b){
  var d=new Dictionary<OpCode,OpCode>{{OpCodes.Br_S,OpCodes.Br},{OpCodes.Brfalse_S,OpCodes.Brfalse},{OpCodes.Brtrue_S,OpCodes.Brtrue},{OpCodes.Beq_S,OpCodes.Beq},{OpCodes.Bne_Un_S,OpCodes.Bne_Un},{OpCodes.Bge_S,OpCodes.Bge},{OpCodes.Bge_Un_S,OpCodes.Bge_Un},{OpCodes.Bgt_S,OpCodes.Bgt},{OpCodes.Bgt_Un_S,OpCodes.Bgt_Un},{OpCodes.Ble_S,OpCodes.Ble},{OpCodes.Ble_Un_S,OpCodes.Ble_Un},{OpCodes.Blt_S,OpCodes.Blt},{OpCodes.Blt_Un_S,OpCodes.Blt_Un},{OpCodes.Leave_S,OpCodes.Leave}};
  foreach(var x in b.Instructions)if(d.TryGetValue(x.OpCode,out var op))x.OpCode=op;
 }
 static void Main(string[] args){
  var original=JsonSerializer.Deserialize<List<string>>(File.ReadAllText(args[1]))!;
  var translations=new SortedDictionary<int,string>();
  foreach(var p in Directory.GetFiles(args[2],"NightAuction.Korean.*.tsv"))
   foreach(var line in File.ReadAllLines(p)){
    if(string.IsNullOrEmpty(line))continue;
    var tab=line.IndexOf('\t');if(tab<0)throw new Exception("Bad row");
    var idx=int.Parse(line.Substring(0,tab));if(!translations.TryAdd(idx,Un(line.Substring(tab+1))))throw new Exception("Duplicate "+idx);
   }
  if(original.Count!=659||translations.Count!=659)throw new Exception("Bad count "+original.Count+" "+translations.Count);
  var map=new Dictionary<string,string>(StringComparer.Ordinal);
  for(int i=0;i<659;i++)map.Add(Un(original[i]),translations[i]);
  var resolver=new DefaultAssemblyResolver(); resolver.AddSearchDirectory("probably-stolen/Managed");
  var assembly=AssemblyDefinition.ReadAssembly(args[0],new ReaderParameters{InMemory=true,AssemblyResolver=resolver});
  var module=assembly.MainModule;
  var auction=module.Types.SelectMany(Types).Single(t=>t.Name=="AuctionText");
  var getLocale=auction.Methods.Single(m=>m.Name=="get_CurrentLocaleCode");
  var helper=new TypeDefinition("KoreanLocalization","NightAuctionKoreanRuntime",TypeAttributes.NotPublic|TypeAttributes.Abstract|TypeAttributes.Sealed,module.TypeSystem.Object);
  module.Types.Add(helper);
  var str=module.TypeSystem.String;
  // Hashtable uses non-generic CLR signatures; valid under the game's Mono runtime.
  var ht=new TypeReference("System.Collections","Hashtable",module,module.TypeSystem.Object.Scope);
  var field=new FieldDefinition("Map",FieldAttributes.Private|FieldAttributes.Static|FieldAttributes.InitOnly,ht);helper.Fields.Add(field);
  var ctor=new MethodReference(".ctor",module.TypeSystem.Void,ht){HasThis=true};
  var add=new MethodReference("Add",module.TypeSystem.Void,ht){HasThis=true};
  add.Parameters.Add(new ParameterDefinition(module.TypeSystem.Object));add.Parameters.Add(new ParameterDefinition(module.TypeSystem.Object));
  var lookup=new MethodReference("get_Item",module.TypeSystem.Object,ht){HasThis=true};
  lookup.Parameters.Add(new ParameterDefinition(module.TypeSystem.Object));
  var starts=new MethodReference("StartsWith",module.TypeSystem.Boolean,str){HasThis=true};starts.Parameters.Add(new ParameterDefinition(str));
  var replace=new MethodReference("Replace",str,str){HasThis=true};replace.Parameters.Add(new ParameterDefinition(str));replace.Parameters.Add(new ParameterDefinition(str));
  var clinit=Add(helper,".cctor",module.TypeSystem.Void);clinit.Attributes|=Mono.Cecil.MethodAttributes.SpecialName|Mono.Cecil.MethodAttributes.RTSpecialName;
  var il=clinit.Body.GetILProcessor();il.Emit(OpCodes.Newobj,ctor);il.Emit(OpCodes.Stsfld,field);
  foreach(var kv in map){il.Emit(OpCodes.Ldsfld,field);il.Emit(OpCodes.Ldstr,kv.Key);il.Emit(OpCodes.Ldstr,kv.Value);il.Emit(OpCodes.Callvirt,add);}
  il.Emit(OpCodes.Ret);
  var known=Add(helper,"Known",str,str);known.Body.InitLocals=true;known.Body.Variables.Add(new VariableDefinition(str));
  il=known.Body.GetILProcessor();var fallback=Instruction.Create(OpCodes.Ldarg_0);
  il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Brfalse,fallback);
  il.Emit(OpCodes.Ldsfld,field);il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Callvirt,lookup);
  il.Emit(OpCodes.Castclass,str);il.Emit(OpCodes.Stloc_0);
  il.Emit(OpCodes.Ldloc_0);il.Emit(OpCodes.Brfalse,fallback);il.Emit(OpCodes.Ldloc_0);il.Emit(OpCodes.Ret);
  il.Append(fallback);il.Emit(OpCodes.Ret);
  var korean=Add(helper,"IsKorean",module.TypeSystem.Boolean,str);il=korean.Body.GetILProcessor();var notKo=Instruction.Create(OpCodes.Ldc_I4_0);
  il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Brfalse,notKo);il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Ldstr,"ko");il.Emit(OpCodes.Callvirt,starts);il.Emit(OpCodes.Ret);il.Append(notKo);il.Emit(OpCodes.Ret);
  var translate=Add(helper,"Literal",str,str);il=translate.Body.GetILProcessor();var same=Instruction.Create(OpCodes.Ldarg_0);
  il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Brfalse,same);il.Emit(OpCodes.Call,getLocale);il.Emit(OpCodes.Call,korean);il.Emit(OpCodes.Brfalse,same);
  il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Call,known);il.Emit(OpCodes.Ret);il.Append(same);il.Emit(OpCodes.Ret);
  var log=Add(helper,"Log",str,str);il=log.Body.GetILProcessor();var nullOut=Instruction.Create(OpCodes.Ldarg_0);
  il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Brfalse,nullOut);il.Emit(OpCodes.Ldarg_0);il.Emit(OpCodes.Call,known);
  var logIndex=Enumerable.Range(475,55).Concat(Enumerable.Range(533,9)).OrderByDescending(i=>Un(original[i]).Length);
  foreach(var n in logIndex){if(translations[n].Length==0)continue;il.Emit(OpCodes.Ldstr,Un(original[n]));il.Emit(OpCodes.Ldstr,translations[n]);il.Emit(OpCodes.Callvirt,replace);}
  il.Emit(OpCodes.Ret);il.Append(nullOut);il.Emit(OpCodes.Ret);
  var isChinese=auction.Methods.Single(m=>m.Name=="IsChinese"&&m.Parameters.Count==1);
  Prefix(isChinese,Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Call,korean),Instruction.Create(OpCodes.Brfalse,isChinese.Body.Instructions[0]),Instruction.Create(OpCodes.Ldc_I4_1),Instruction.Create(OpCodes.Ret));
  var select=auction.Methods.Single(m=>m.Name=="Select"&&m.Parameters.Count==3);
  Prefix(select,Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Call,korean),Instruction.Create(OpCodes.Brfalse,select.Body.Instructions[0]),Instruction.Create(OpCodes.Ldarg_1),Instruction.Create(OpCodes.Call,known),Instruction.Create(OpCodes.Ret));
  var display=auction.Methods.Single(m=>m.Name=="DisplayLog"&&m.Parameters.Count==1);
  Prefix(display,Instruction.Create(OpCodes.Call,getLocale),Instruction.Create(OpCodes.Call,korean),Instruction.Create(OpCodes.Brfalse,display.Body.Instructions[0]),Instruction.Create(OpCodes.Ldarg_0),Instruction.Create(OpCodes.Call,log),Instruction.Create(OpCodes.Ret));
  int patches=0;var distinct=new HashSet<string>();
  foreach(var t in module.Types.SelectMany(Types)){
   if(t==helper)continue;
   foreach(var m in t.Methods.Where(m=>m.HasBody&&!m.IsConstructor)){
    var p=m.Body.GetILProcessor();
    foreach(var ins in m.Body.Instructions.ToArray()){
     if(ins.OpCode==OpCodes.Ldstr&&ins.Operand is string val&&map.ContainsKey(val)){
      p.InsertAfter(ins,Instruction.Create(OpCodes.Call,translate));patches++;distinct.Add(val);
     }
    }
    Widen(m.Body);
   }
  }
  Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(args[3]))!);
  assembly.Write(args[3]);
  using var check=AssemblyDefinition.ReadAssembly(args[3]);
  if(!check.MainModule.Types.Any(t=>t.Name=="NightAuctionKoreanRuntime"))throw new Exception("Verification failed");
  Console.WriteLine("PATCH VERIFIED mapping="+map.Count+" literalCalls="+patches+" distinct="+distinct.Count+" output="+new FileInfo(args[3]).Length);
 }
}
