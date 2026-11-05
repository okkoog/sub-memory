using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using System.Text;

if (args.Length != 4)
    throw new ArgumentException("Usage: CorpMenuOneShot <NoHooks.dll> <CorpPresence.dll> <output.dll> <report>");
var input = args[0]; var corp = args[1]; var output = args[2]; var report = args[3];
using var src = AssemblyDefinition.ReadAssembly(input);
using var target = AssemblyDefinition.ReadAssembly(corp);
var mod = src.MainModule;
var kp = mod.Types.Single(x => x.FullName=="ProbablyStolenKoreanPack.KoreanPack");
var cat = kp.NestedTypes.Single(x=>x.Name=="Catalog");
var catalogField = kp.Fields.Single(x=>x.Name=="Catalogs");
var translator = kp.Methods.Single(x=>x.Name=="TryTranslateSource" && x.Parameters.Count==3);
var native = kp.Methods.Single(x=>x.Name=="MergeNativeEntries");
var entry = kp.Methods.Single(x=>x.Name=="PatchDisplayMethodsInAssembly" && x.Parameters.Count==1);
if (entry.Body.Instructions.Any(i=>i.OpCode==OpCodes.Call || i.OpCode==OpCodes.Callvirt))
    throw new InvalidOperationException("Only patch verified NoHooks baseline (expected no-op method).");

var menu = target.MainModule.Types.Single(x=>x.FullName=="CorpPresence.UI.MenuLabels");
var get = menu.Methods.Single(x=>x.Name=="Get");
if (!get.Body.Instructions.Any(i=>i.OpCode==OpCodes.Ldsfld && ((FieldReference)i.Operand).Name=="English"))
    throw new InvalidOperationException("Corp MenuLabels.Get no longer uses static English array");
if (!get.Body.Instructions.Any(i=>i.OpCode==OpCodes.Ldsfld && ((FieldReference)i.Operand).Name=="Chinese"))
    throw new InvalidOperationException("Corp MenuLabels.Get no longer uses static Chinese array");

var jagged = new ArrayType(new ArrayType(mod.TypeSystem.String));
var row = new ArrayType(mod.TypeSystem.String);
var transform = new MethodDefinition("TranslateCorpMenuRowsOnce", Mono.Cecil.MethodAttributes.Private|Mono.Cecil.MethodAttributes.Static, mod.TypeSystem.Void);
transform.Parameters.Add(new ParameterDefinition("catalog",Mono.Cecil.ParameterAttributes.None,cat));
transform.Parameters.Add(new ParameterDefinition("rows",Mono.Cecil.ParameterAttributes.None,jagged));
kp.Methods.Add(transform);
var a = transform.Body; a.InitLocals=true; a.MaxStackSize=5;
var i = new VariableDefinition(mod.TypeSystem.Int32),j=new VariableDefinition(mod.TypeSystem.Int32),rowVar=new VariableDefinition(row),value=new VariableDefinition(mod.TypeSystem.String);
a.Variables.Add(i);a.Variables.Add(j);a.Variables.Add(rowVar);a.Variables.Add(value);
var il=a.GetILProcessor();
var ret=Instruction.Create(OpCodes.Ret), checkI=Instruction.Create(OpCodes.Ldloc,i),loopI=Instruction.Create(OpCodes.Ldarg_1),
incI=Instruction.Create(OpCodes.Ldloc,i),checkJ=Instruction.Create(OpCodes.Ldloc,j),loopJ=Instruction.Create(OpCodes.Ldarg_0),
incJ=Instruction.Create(OpCodes.Ldloc,j);
il.Append(Instruction.Create(OpCodes.Ldarg_1));
il.Append(Instruction.Create(OpCodes.Brfalse,ret));
il.Append(Instruction.Create(OpCodes.Ldc_I4_0));
il.Append(Instruction.Create(OpCodes.Stloc,i));
il.Append(Instruction.Create(OpCodes.Br,checkI));
il.Append(loopI);
il.Append(Instruction.Create(OpCodes.Ldloc,i));
il.Append(Instruction.Create(OpCodes.Ldelem_Ref));
il.Append(Instruction.Create(OpCodes.Castclass,row));
il.Append(Instruction.Create(OpCodes.Stloc,rowVar));
il.Append(Instruction.Create(OpCodes.Ldloc,rowVar));
il.Append(Instruction.Create(OpCodes.Brfalse,incI));
il.Append(Instruction.Create(OpCodes.Ldc_I4_0));il.Append(Instruction.Create(OpCodes.Stloc,j));
il.Append(Instruction.Create(OpCodes.Br,checkJ));
il.Append(loopJ);
il.Append(Instruction.Create(OpCodes.Ldloc,rowVar));il.Append(Instruction.Create(OpCodes.Ldloc,j));il.Append(Instruction.Create(OpCodes.Ldelem_Ref));
il.Append(Instruction.Create(OpCodes.Castclass,mod.TypeSystem.String));il.Append(Instruction.Create(OpCodes.Ldloca,value));
il.Append(Instruction.Create(OpCodes.Call,translator));
il.Append(Instruction.Create(OpCodes.Brfalse,incJ));
il.Append(Instruction.Create(OpCodes.Ldloc,rowVar));il.Append(Instruction.Create(OpCodes.Ldloc,j));il.Append(Instruction.Create(OpCodes.Ldloc,value));
il.Append(Instruction.Create(OpCodes.Stelem_Ref));
il.Append(incJ);il.Append(Instruction.Create(OpCodes.Ldc_I4_1));il.Append(Instruction.Create(OpCodes.Add));il.Append(Instruction.Create(OpCodes.Stloc,j));
il.Append(checkJ);il.Append(Instruction.Create(OpCodes.Ldloc,rowVar));il.Append(Instruction.Create(OpCodes.Ldlen));il.Append(Instruction.Create(OpCodes.Conv_I4));il.Append(Instruction.Create(OpCodes.Blt,loopJ));
il.Append(incI);il.Append(Instruction.Create(OpCodes.Ldc_I4_1));il.Append(Instruction.Create(OpCodes.Add));il.Append(Instruction.Create(OpCodes.Stloc,i));
il.Append(checkI);il.Append(Instruction.Create(OpCodes.Ldarg_1));il.Append(Instruction.Create(OpCodes.Ldlen));il.Append(Instruction.Create(OpCodes.Conv_I4));il.Append(Instruction.Create(OpCodes.Blt,loopI));
il.Append(ret);

var flags=BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Static;
var findByName=mod.ImportReference(typeof(Assembly).GetMethod("GetType",new[]{typeof(string),typeof(bool)})!);
var getTypeName=mod.ImportReference(typeof(Assembly).GetMethod("GetName",Type.EmptyTypes)!);
var getName=mod.ImportReference(typeof(AssemblyName).GetProperty("Name")!.GetMethod!);
var equals=mod.ImportReference(typeof(string).GetMethod("Equals",new[]{typeof(string),typeof(string),typeof(StringComparison)})!);
var getField=mod.ImportReference(typeof(Type).GetMethod("GetField",new[]{typeof(string),typeof(BindingFlags)})!);
var getValue=mod.ImportReference(typeof(FieldInfo).GetMethod("GetValue",new[]{typeof(object)})!);
var dictionaryTryGet=new MethodReference("TryGetValue",mod.TypeSystem.Boolean,catalogField.FieldType){HasThis=true};
dictionaryTryGet.Parameters.Add(new ParameterDefinition(mod.TypeSystem.String));
dictionaryTryGet.Parameters.Add(new ParameterDefinition(new ByReferenceType(cat)));

entry.Body.Instructions.Clear();entry.Body.ExceptionHandlers.Clear();entry.Body.Variables.Clear();
entry.Body.InitLocals=true;entry.Body.MaxStackSize=6;
var cv =new VariableDefinition(cat),t=new VariableDefinition(mod.ImportReference(typeof(Type))),f=new VariableDefinition(mod.ImportReference(typeof(FieldInfo)));
entry.Body.Variables.Add(cv);entry.Body.Variables.Add(t);entry.Body.Variables.Add(f);
var e=entry.Body.GetILProcessor();
var done=Instruction.Create(OpCodes.Ret),fields=Instruction.Create(OpCodes.Ldloc,t),loop=Instruction.Create(OpCodes.Ldloc,f);
e.Append(Instruction.Create(OpCodes.Ldarg_0));e.Append(Instruction.Create(OpCodes.Brfalse,done));
e.Append(Instruction.Create(OpCodes.Ldarg_0));e.Append(Instruction.Create(OpCodes.Callvirt,getTypeName));e.Append(Instruction.Create(OpCodes.Callvirt,getName));
e.Append(Instruction.Create(OpCodes.Ldstr,"CorpPresence"));e.Append(Instruction.Create(OpCodes.Ldc_I4,(int)StringComparison.OrdinalIgnoreCase));
e.Append(Instruction.Create(OpCodes.Call,equals));e.Append(Instruction.Create(OpCodes.Brfalse,done));
e.Append(Instruction.Create(OpCodes.Ldsfld,catalogField));e.Append(Instruction.Create(OpCodes.Ldstr,"CorpPresence"));e.Append(Instruction.Create(OpCodes.Ldloca,cv));
e.Append(Instruction.Create(OpCodes.Callvirt,dictionaryTryGet));e.Append(Instruction.Create(OpCodes.Brfalse,done));
e.Append(Instruction.Create(OpCodes.Ldarg_0));e.Append(Instruction.Create(OpCodes.Ldstr,"CorpPresence.UI.MenuLabels"));
e.Append(Instruction.Create(OpCodes.Ldc_I4_0));e.Append(Instruction.Create(OpCodes.Callvirt,findByName));e.Append(Instruction.Create(OpCodes.Stloc,t));
e.Append(Instruction.Create(OpCodes.Ldloc,t));e.Append(Instruction.Create(OpCodes.Brfalse,done));
foreach(var fname in new[]{"English","Chinese"}){
 var next=Instruction.Create(OpCodes.Nop);
 e.Append(Instruction.Create(OpCodes.Ldloc,t));e.Append(Instruction.Create(OpCodes.Ldstr,fname));e.Append(Instruction.Create(OpCodes.Ldc_I4,(int)flags));
 e.Append(Instruction.Create(OpCodes.Callvirt,getField));e.Append(Instruction.Create(OpCodes.Stloc,f));
 e.Append(Instruction.Create(OpCodes.Ldloc,f));e.Append(Instruction.Create(OpCodes.Brfalse,next));
 e.Append(Instruction.Create(OpCodes.Ldloc,cv));e.Append(Instruction.Create(OpCodes.Ldloc,f));e.Append(Instruction.Create(OpCodes.Ldnull));
 e.Append(Instruction.Create(OpCodes.Callvirt,getValue));e.Append(Instruction.Create(OpCodes.Isinst,jagged));
 e.Append(Instruction.Create(OpCodes.Call,transform));
 e.Append(next);
}
e.Append(done);
if(!kp.Methods.Any(x=>x.Name==native.Name))throw new Exception("Native merge missing");

// Existing baseline must remain fully hook-free even after the injection.
var prohibited=mod.Types.SelectMany(AllTypes).SelectMany(x=>x.Methods).Where(x=>x.HasBody)
 .SelectMany(x=>x.Body.Instructions.Where(y=>(y.OpCode==OpCodes.Call||y.OpCode==OpCodes.Callvirt)
   && y.Operand is MethodReference mr && mr.DeclaringType.FullName=="HarmonyLib.Harmony" && mr.Name.StartsWith("Patch"))
 .Select(y=>x.FullName)).ToList();
if(prohibited.Count>0)throw new Exception("Harmony Patch calls remain: "+string.Join(", ",prohibited));
src.Write(output);
using(var check=AssemblyDefinition.ReadAssembly(output)){
 var type=check.MainModule.Types.Single(x=>x.FullName==kp.FullName);
 if(!type.Methods.Any(x=>x.Name=="TranslateCorpMenuRowsOnce"))throw new Exception("Bridge missing");
 if(!type.Methods.Any(x=>x.Name=="MergeNativeEntries"))throw new Exception("Native merge removed");
}
var s=target.MainModule.Types.Single(x=>x.FullName=="CorpPresence.UI.MenuLabels").Methods.Single(x=>x.Name==".cctor").Body.Instructions;
File.WriteAllLines(report,new[]{
 "Corp NoHooks one-shot menu test — build verified (NOT game-tested)",
 "Source KoreanPack: "+input,
 "Corp: "+corp,
 "Target: CorpPresence.UI.MenuLabels.English and Chinese string[][]",
 "Invocation: PatchDisplayMethodsInAssembly after catalog mapping on OnModAssetsLoaded, once per registered assembly",
 "Corp MenuLabels.Get directly reads English/Chinese arrays: YES",
 "Corp MenuLabels.cctor literal count: "+s.Count(x=>x.OpCode==OpCodes.Ldstr),
 "No Harmony Patch calls: YES",
 "Other mods changed: NO",
 "Original NoHooks binary overwritten: NO",
 "Native localization merge preserved: YES",
 "Caveat: Corp displays built on demand through L.T are not translated. Runtime behavior unverified."
 },Encoding.UTF8);
Console.WriteLine("Corp one-shot menu trial written: "+output);
static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t){yield return t;foreach(var n in t.NestedTypes)foreach(var x in AllTypes(n))yield return x;}
