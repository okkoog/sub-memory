using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

internal static class FreHarmonyCompatPatch
{
    private const string TargetMethod =
        "System.Boolean HarmonyLib.CodeInstructionExtensions::LoadsConstant(HarmonyLib.CodeInstruction,System.String)";
    private const string HelperMethod = "IsExactStringConstant_HarmonyCompat";

    public static void Main(string[] args)
    {
        if (args.Length != 3)
            throw new ArgumentException("Usage: patch <original-FactionReputationExpansion.dll> <0Harmony.dll> <output.dll>");
        var source = Path.GetFullPath(args[0]);
        var harmonyFile = Path.GetFullPath(args[1]);
        var output = Path.GetFullPath(args[2]);

        using var assembly = AssemblyDefinition.ReadAssembly(source, new ReaderParameters { InMemory = true, ReadSymbols = false });
        using var harmony = AssemblyDefinition.ReadAssembly(harmonyFile);
        var module = assembly.MainModule;

        var classToPatch = module.GetType("FactionReputationExpansion.Patches.ThiefCrateFirstPatch")
            ?? throw new Exception("Expected ThiefCrateFirstPatch not found");
        var nested = classToPatch.NestedTypes.Single(t => t.Name == "<>c");
        var lambda = nested.Methods.Single(m => m.Name == "<ReorderStock>b__7_1");
        var oldCall = lambda.Body.Instructions.Single(i =>
            i.OpCode == OpCodes.Call &&
            i.Operand is MethodReference mr &&
            mr.FullName == TargetMethod);

        // Create a local, API-independent replacement for the missing Harmony
        // CodeInstructionExtensions.LoadsConstant(CodeInstruction, string) overload.
        // No global Harmony changes; only one call in the original mod is redirected.
        var codeInstructionType = harmony.MainModule.GetType("HarmonyLib.CodeInstruction")
            ?? throw new Exception("Harmony CodeInstruction type not found");
        var opcodeField = module.ImportReference(codeInstructionType.Fields.Single(f => f.Name == "opcode"));
        var operandField = module.ImportReference(codeInstructionType.Fields.Single(f => f.Name == "operand"));
        var opcodeType = module.ImportReference(codeInstructionType.Fields.Single(f => f.Name == "opcode").FieldType);
        var method = new MethodDefinition(
            HelperMethod,
            MethodAttributes.Assembly | MethodAttributes.Static | MethodAttributes.HideBySig,
            module.TypeSystem.Boolean);
        method.Parameters.Add(new ParameterDefinition("instruction", ParameterAttributes.None, module.ImportReference(codeInstructionType)));
        method.Parameters.Add(new ParameterDefinition("expectedString", ParameterAttributes.None, module.TypeSystem.String));
        classToPatch.Methods.Add(method);
        method.Body.InitLocals = true;
        var opLocal = new VariableDefinition(opcodeType);
        method.Body.Variables.Add(opLocal);

        var opcodeName = new MethodReference("get_Name", module.TypeSystem.String, opcodeType) { HasThis = true };
        var stringEqual = new MethodReference("op_Equality", module.TypeSystem.Boolean, module.TypeSystem.String) { HasThis = false };
        stringEqual.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));
        stringEqual.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));

        var il = method.Body.GetILProcessor();
        var returnFalse = Instruction.Create(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Brfalse, returnFalse);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, opcodeField);
        il.Emit(OpCodes.Stloc_0);
        il.Emit(OpCodes.Ldloca_S, opLocal);
        il.Emit(OpCodes.Call, opcodeName);
        il.Emit(OpCodes.Ldstr, "ldstr");
        il.Emit(OpCodes.Call, stringEqual);
        il.Emit(OpCodes.Brfalse, returnFalse);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, operandField);
        il.Emit(OpCodes.Isinst, module.TypeSystem.String);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Call, stringEqual);
        il.Emit(OpCodes.Ret);
        il.Append(returnFalse);
        il.Emit(OpCodes.Ret);

        // Patch the one lambda that broke store opening. Keep original transpiler,
        // all its other instructions, its labels, and all existing mod features.
        oldCall.Operand = method;
        int missingCalls = CountBadCalls(assembly);
        if (missingCalls != 0)
            throw new Exception("Missing-method calls still present: " + missingCalls);

        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        assembly.Write(output, new WriterParameters { WriteSymbols = false });

        using var verify = AssemblyDefinition.ReadAssembly(output);
        var verifyClass = verify.MainModule.GetType("FactionReputationExpansion.Patches.ThiefCrateFirstPatch")
            ?? throw new Exception("Patched class disappeared");
        var verifyHelper = verifyClass.Methods.Single(x => x.Name == HelperMethod);
        var verifyLambda = verifyClass.NestedTypes.Single(x => x.Name == "<>c")
            .Methods.Single(x => x.Name == "<ReorderStock>b__7_1");
        var helperCalls = verifyLambda.Body.Instructions.Count(x =>
            x.Operand is MethodReference mr && mr.Name == HelperMethod);
        if (helperCalls != 1 || CountBadCalls(verify) != 0)
            throw new Exception("Post-save verification failed.");
        Console.WriteLine("PATCH PASS: original DLL=" + new FileInfo(source).Length +
            " patched DLL=" + new FileInfo(output).Length +
            " missing Harmony calls=0 replacement calls=" + helperCalls +
            " helper IL instructions=" + verifyHelper.Body.Instructions.Count);
    }

    private static int CountBadCalls(AssemblyDefinition asm)
    {
        return asm.MainModule.Types
            .SelectMany(t => new[] { t }.Concat(t.NestedTypes))
            .SelectMany(t => t.Methods)
            .Where(m => m.HasBody)
            .SelectMany(m => m.Body.Instructions)
            .Count(i => i.Operand is MethodReference mr && mr.FullName == TargetMethod);
    }
}
