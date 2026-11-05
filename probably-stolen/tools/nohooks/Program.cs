using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Text;

if (args.Length != 3) throw new ArgumentException("Usage: NoHooks <original.dll> <output.dll> <report.txt>");
var source = args[0];
var destination = args[1];
var reportPath = args[2];
using var assembly = AssemblyDefinition.ReadAssembly(source, new ReaderParameters { ReadSymbols = false });
var lines = new List<string> {
    "KoreanPack 0.7.5.21-dev hook-free binary patch",
    "Original: " + Path.GetFullPath(source),
    "Assembly: " + assembly.Name.FullName
};
var own = assembly.MainModule.Types.SelectMany(AllTypes).Where(t => t.FullName.Contains("KoreanPack")).ToList();
if (own.Count == 0) throw new Exception("KoreanPack type not found");
var allMethods = assembly.MainModule.Types.SelectMany(AllTypes).SelectMany(t=>t.Methods).ToList();
var calls = allMethods.SelectMany(m => m.HasBody ? m.Body.Instructions
    .Where(i=>i.OpCode==OpCodes.Call || i.OpCode==OpCodes.Callvirt)
    .Select(i => new { caller=m, called=i.Operand as MethodReference })
    .Where(x=>x.called!=null && IsPatchCall(x.called)) : Array.Empty<dynamic>().Select(x => new { caller=m, called=(MethodReference?)null }))
    .ToList();
lines.Add("Harmony registration calls before: " + calls.Count);
foreach (var c in calls) lines.Add("PATCH-CALL " + c.caller.FullName + " -> " + c.called?.FullName);

var entrypoints = new [] { "PatchDynamic", "PatchDynamicTranspiler", "PatchFinalTooltipBridge", "PatchSharedDisplayBridges", "PatchDisplayMethodsInAssembly", "PatchKnownType" };
var touched = new List<MethodDefinition>();
foreach (var methodName in entrypoints) {
    var matches = own.SelectMany(t=>t.Methods).Where(m=>m.Name==methodName).ToList();
    if(matches.Count == 0) throw new Exception("Expected patch entrypoint missing: " + methodName);
    foreach(var method in matches) {
        if(!method.HasBody || method.IsConstructor) throw new Exception("Unexpected patch method: " + method.FullName);
        ReplaceWithSafeNoOp(method);
        touched.Add(method);
        lines.Add("DISABLED " + method.FullName);
    }
}
var remaining = allMethods.SelectMany(m => m.HasBody ? m.Body.Instructions
    .Where(i=>i.OpCode==OpCodes.Call || i.OpCode==OpCodes.Callvirt)
    .Select(i => new { caller=m, called=i.Operand as MethodReference })
    .Where(x=>x.called!=null && IsPatchCall(x.called)) : Array.Empty<dynamic>().Select(x => new { caller=m, called=(MethodReference?)null })).ToList();
foreach(var c in remaining) lines.Add("UNRESOLVED-HOOK " + c.caller.FullName + " -> " + c.called?.FullName);
if(remaining.Count != 0) throw new Exception("Unexpected Harmony registration paths remain; refusing unsafe build. " + string.Join("; ",remaining.Select(x=>x.caller.Name)));
var expected = new [] { "MergeNativeEntries", "OnModAssetsLoaded" };
foreach(var methodName in expected) {
    if (!own.SelectMany(t=>t.Methods).Any(x=>x.Name == methodName && x.HasBody)) throw new Exception("Native localization lifecycle missing: "+methodName);
}
assembly.Write(destination);
using (var audit = AssemblyDefinition.ReadAssembly(destination)) {
    var all = audit.MainModule.Types.SelectMany(AllTypes).SelectMany(t => t.Methods).ToList();
    var forbidden = all.SelectMany(m=>m.HasBody ? m.Body.Instructions.Where(i=>(i.OpCode == OpCodes.Call || i.OpCode == OpCodes.Callvirt) && i.Operand is MethodReference mr && IsPatchCall(mr)).Select(i=>m.FullName) : Array.Empty<string>()).ToList();
    if(forbidden.Count>0)throw new Exception("Output still invokes Harmony patch functions: "+string.Join("; ",forbidden));
    lines.Add("Output Harmony registration calls: 0");
    lines.Add("Output native localization merge preserved: YES");
    lines.Add("All method bodies inspected: "+all.Count);
    lines.Add("Changed methods: "+touched.Count);
}
File.WriteAllLines(reportPath,lines,Encoding.UTF8);
Console.WriteLine(string.Join(Environment.NewLine,lines));

static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t) {
    yield return t;
    foreach(var nested in t.NestedTypes) foreach(var child in AllTypes(nested)) yield return child;
}
static bool IsPatchCall(MethodReference method) {
    var type = method.DeclaringType.FullName;
    var name = method.Name;
    if (type == "HarmonyLib.Harmony" && (name=="Patch" || name=="PatchAll" || name=="PatchCategory" || name=="CreateAndPatchAll")) return true;
    if (type == "HarmonyLib.PatchProcessor" && (name=="Patch")) return true;
    return false;
}
static void ReplaceWithSafeNoOp(MethodDefinition method) {
    var rt=method.ReturnType.FullName;
    if(rt != "System.Void" && rt != "System.Boolean" && rt != "System.Int32")throw new Exception("Unsafe return type for "+method.FullName);
    method.Body.Instructions.Clear();
    method.Body.ExceptionHandlers.Clear();
    method.Body.Variables.Clear();
    method.Body.InitLocals = false;
    method.Body.MaxStackSize = 1;
    if(rt=="System.Boolean" || rt=="System.Int32")method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
    method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
}
