using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length != 3) throw new ArgumentException("Usage: Patcher <original-dll> <helper-dll> <output-dll>");
string targetPath = args[0], helperPath = args[1], outputPath = args[2];
var target = AssemblyDefinition.ReadAssembly(targetPath);
var source = AssemblyDefinition.ReadAssembly(helperPath);
var original = target.MainModule;
var from = source.MainModule;
var old = original.GetType("AllGirlsKoreanBountiesCompat.LiquidNameCompatibility");
if (old == null) throw new Exception("Original compatibility type missing");
var refresh = old.Methods.Single(x => x.Name == "Refresh" && x.Parameters.Count == 0);
if (original.GetType("AllGirlsKoreanBountiesCompat.IngotPurityCompatibility") != null)
    throw new Exception("This DLL already contains the ingot compatibility patch");
var sourceType = from.GetType("AllGirlsKoreanBountiesCompat.IngotPurityCompatibility") ??
    throw new Exception("Built helper type missing");
var newType = new TypeDefinition(sourceType.Namespace, sourceType.Name, sourceType.Attributes, original.TypeSystem.Object);
original.Types.Add(newType);
var methodMap = new Dictionary<MethodDefinition, MethodDefinition>();
foreach (var method in sourceType.Methods)
{
    var m = new MethodDefinition(method.Name, method.Attributes, original.ImportReference(method.ReturnType));
    m.ImplAttributes = method.ImplAttributes;
    foreach (var p in method.Parameters)
        m.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, original.ImportReference(p.ParameterType)));
    newType.Methods.Add(m);
    methodMap[method] = m;
}
foreach (var pair in methodMap)
{
    var s = pair.Key; var d = pair.Value;
    if (!s.HasBody) continue;
    var body = d.Body;
    body.InitLocals = s.Body.InitLocals;
    body.MaxStackSize = s.Body.MaxStackSize;
    var locals = new Dictionary<VariableDefinition, VariableDefinition>();
    foreach (var local in s.Body.Variables)
    {
        var l = new VariableDefinition(original.ImportReference(local.VariableType));
        body.Variables.Add(l); locals.Add(local, l);
    }
    var instructions = new Dictionary<Instruction, Instruction>();
    foreach (var op in s.Body.Instructions)
    {
        var n = Instruction.Create(OpCodes.Nop);
        body.Instructions.Add(n);
        instructions.Add(op, n);
    }
    foreach (var op in s.Body.Instructions)
    {
        var n = instructions[op]; n.OpCode = op.OpCode;
        var item = op.Operand;
        if (item is Instruction jump) n.Operand = instructions[jump];
        else if (item is Instruction[] jumps) n.Operand = jumps.Select(x => instructions[x]).ToArray();
        else if (item is VariableDefinition local) n.Operand = locals[local];
        else if (item is ParameterDefinition param) n.Operand = d.Parameters[s.Parameters.IndexOf(param)];
        else if (item is TypeReference type) n.Operand = original.ImportReference(type);
        else if (item is MethodReference method)
        {
            if (method.DeclaringType.FullName == sourceType.FullName)
            {
                var own = sourceType.Methods.FirstOrDefault(x => x.Name == method.Name && x.Parameters.Count == method.Parameters.Count)
                    ?? throw new Exception("Unable to map helper internal method: " + method.FullName);
                n.Operand = methodMap[own];
            }
            else n.Operand = original.ImportReference(method);
        }
        else if (item is FieldReference field) n.Operand = original.ImportReference(field);
        else if (item is CallSite site) n.Operand = original.ImportReference(site);
        else n.Operand = item;
    }
    foreach (var handler in s.Body.ExceptionHandlers)
    {
        body.ExceptionHandlers.Add(new ExceptionHandler(handler.HandlerType) {
            TryStart = handler.TryStart == null ? null : instructions[handler.TryStart],
            TryEnd = handler.TryEnd == null ? null : instructions[handler.TryEnd],
            HandlerStart = handler.HandlerStart == null ? null : instructions[handler.HandlerStart],
            HandlerEnd = handler.HandlerEnd == null ? null : instructions[handler.HandlerEnd],
            FilterStart = handler.FilterStart == null ? null : instructions[handler.FilterStart],
            CatchType = handler.CatchType == null ? null : original.ImportReference(handler.CatchType)
        });
    }
}
var fix = newType.Methods.Single(x => x.Name == "Refresh" && x.Parameters.Count == 0);
refresh.Body.GetILProcessor().InsertBefore(refresh.Body.Instructions[0], Instruction.Create(OpCodes.Call, fix));
target.Name.Version = new Version(1, 0, 4, 0);
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
target.Write(outputPath);
var reloaded = AssemblyDefinition.ReadAssembly(outputPath);
var originalRefresh = reloaded.MainModule.GetType("AllGirlsKoreanBountiesCompat.LiquidNameCompatibility").Methods.Single(x => x.Name == "Refresh");
var newFix = reloaded.MainModule.GetType("AllGirlsKoreanBountiesCompat.IngotPurityCompatibility")?.Methods.Single(x => x.Name == "Refresh");
if (newFix == null || !originalRefresh.Body.Instructions.Any(x => x.Operand is MethodReference r && r.FullName == newFix.FullName))
    throw new Exception("Verification failed: inserted method not called");
foreach (string method in new[] { "KoreanLookup", "AcceptAllGirlsBoard", "InitializeBargainLines" })
    if (reloaded.MainModule.GetType("AllGirlsKoreanBountiesCompat.CompatibilityMod").Methods.All(x=>x.Name!=method))
        throw new Exception("Verification failed: lost existing function " + method);
Console.WriteLine("Verified patched DLL: " + outputPath + " (" + new FileInfo(outputPath).Length + " bytes)");
