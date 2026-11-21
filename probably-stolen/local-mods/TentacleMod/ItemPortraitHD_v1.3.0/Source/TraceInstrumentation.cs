using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace ItemPortraitHD
{
    // Read the actual by-ref argument after the provider returns. Harmony's
    // object[] __args snapshot does not reflect ref assignments on this version.
    internal static class TraceInstrumentation
    {
        internal static IEnumerable<CodeInstruction> Wrap(IEnumerable<CodeInstruction> code, ILGenerator generator,
            MethodBase method, int resultArgument, int id, Type valueType, MethodInfo observer)
        {
            var before = generator.DeclareLocal(valueType);
            var returned = generator.DeclareLocal(valueType);
            bool returnsValue = method is MethodInfo info && info.ReturnType == valueType;
            CodeInstruction LoadArgument() => new CodeInstruction(OpCodes.Ldarg, resultArgument + (method.IsStatic ? 0 : 1));
            bool byRef = resultArgument >= 0 && method.GetParameters()[resultArgument].ParameterType.IsByRef;
            if (resultArgument >= 0)
            {
                yield return LoadArgument();
                if (byRef) yield return new CodeInstruction(OpCodes.Ldind_Ref);
            }
            else yield return new CodeInstruction(OpCodes.Ldnull);
            yield return new CodeInstruction(OpCodes.Stloc, before);
            foreach (var instruction in code)
            {
                if (instruction.opcode == OpCodes.Ret)
                {
                    var first = new CodeInstruction(returnsValue ? OpCodes.Dup : OpCodes.Ldc_I4, returnsValue ? null : (object)id);
                    instruction.MoveLabelsTo(first);
                    instruction.MoveBlocksTo(first);
                    yield return first;
                    if (returnsValue)
                    {
                        yield return new CodeInstruction(OpCodes.Stloc, returned);
                        yield return new CodeInstruction(OpCodes.Ldc_I4, id);
                    }
                    yield return new CodeInstruction(OpCodes.Ldloc, before);
                    if (returnsValue) yield return new CodeInstruction(OpCodes.Ldloc, returned);
                    else
                    {
                        yield return LoadArgument();
                        if (byRef) yield return new CodeInstruction(OpCodes.Ldind_Ref);
                    }
                    yield return new CodeInstruction(OpCodes.Call, observer);
                }
                yield return instruction;
            }
        }
    }
}
