using Mono.Cecil;
using Mono.Cecil.Cil;

// Modify our KoreanPack DLL only. The target MidnightPawn DLL is never altered.
// The source v0.7.5.4 build is older than the deployed v0.7.5.12 DLL,
// therefore add targeted call sites directly to the deployed assembly.
if (args.Length != 2)
    throw new ArgumentException("usage: patcher <current KoreanPack DLL> <repaired KoreanPack DLL>");
string input = args[0], output = args[1];
using var module = ModuleDefinition.ReadModule(input, new ReaderParameters { InMemory = true });
var main = module.Types.Single(x => x.FullName == "ProbablyStolenKoreanPack.KoreanPack");
var patchMethod = main.Methods.Single(x => x.Name == "PatchDisplayMethodsInAssembly");
var patchKnownType = main.Methods.Single(x => x.Name == "PatchKnownType");
var patchType = main.NestedTypes.Single(x => x.Name == "KnownPatch");
var kindType = main.NestedTypes.Single(x => x.Name == "KnownPatchKind");
var ctor = patchType.Methods.Single(x => x.IsConstructor && x.Parameters.Count == 2);

int Kind(string name) => Convert.ToInt32(kindType.Fields.Single(x => x.Name == name).Constant);
void AddSpec(string targetClass, params (string method, string mode)[] specs)
{
    if (specs.Length == 0) return;
    var il = patchMethod.Body.GetILProcessor();
    var insertionPoint = patchMethod.Body.Instructions[0];
    void Add(Instruction x) => il.InsertBefore(insertionPoint, x);

    Add(Instruction.Create(OpCodes.Ldarg_0)); // Loaded mod Assembly
    Add(Instruction.Create(OpCodes.Ldstr, targetClass));
    Add(Instruction.Create(OpCodes.Ldc_I4, specs.Length));
    Add(Instruction.Create(OpCodes.Newarr, patchType));
    for (int i = 0; i < specs.Length; i++)
    {
        Add(Instruction.Create(OpCodes.Dup));
        Add(Instruction.Create(OpCodes.Ldc_I4, i));
        Add(Instruction.Create(OpCodes.Ldstr, specs[i].method));
        Add(Instruction.Create(OpCodes.Ldc_I4, Kind(specs[i].mode)));
        Add(Instruction.Create(OpCodes.Newobj, ctor));
        Add(Instruction.Create(OpCodes.Stelem_Ref));
    }
    Add(Instruction.Create(OpCodes.Call, patchKnownType));
    Add(Instruction.Create(OpCodes.Pop));
}

// Perk text originates from registered Func<string> delegates, not the inventory tooltip.
// These exact compiler-generated delegate names were verified against MidnightPawn.dll.
AddSpec("MidnightPawn.Game.Perks+<>c",
    ("<Register>b__1_0", "StringResult"),
    ("<Register>b__1_1", "StringResult"));

// Recover translated fields when the mod changes or rebuilds maiden descriptions.
AddSpec("MidnightPawn.Game.ItemFactory",
    ("UpdateMaidenDescription", "ObjectArgs"),
    ("CreateMaiden", "GameItemResult"));
AddSpec("MidnightPawn.Game.MaidenTooltip",
    ("OnTooltip", "ObjectArgs"));
AddSpec("MidnightPawn.Game.NameGen",
    ("RandomName", "StringResult"));
AddSpec("MidnightPawn.Game.MaidenTraits",
    ("Pick", "StringResult"));
AddSpec("MidnightPawn.Game.TentacleBody",
    ("ShowReport", "ObjectArgs"),
    ("ProduceDaughter", "GameItemResult"));
AddSpec("MidnightPawn.Game.Toast",
    ("Show", "VisibleStringArgs"));

var originalTypeCount = module.Types.Sum(t => 1 + t.NestedTypes.Count);
var originalMethodCount = module.Types.Sum(t => t.Methods.Count + t.NestedTypes.Sum(n => n.Methods.Count));
if (originalMethodCount < 250)
    throw new InvalidOperationException($"Unexpected older KoreanPack version: only {originalMethodCount} methods");
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
module.Write(output);
using var check = ModuleDefinition.ReadModule(output);
var checkMain = check.Types.Single(x => x.FullName == "ProbablyStolenKoreanPack.KoreanPack");
var afterCount = check.Types.Sum(t => t.Methods.Count + t.NestedTypes.Sum(n => n.Methods.Count));
if (afterCount != originalMethodCount)
    throw new InvalidOperationException($"Method count changed: {originalMethodCount} -> {afterCount}");
Console.WriteLine($"Original methods retained: {originalMethodCount}; types: {originalTypeCount}");
Console.WriteLine($"Targeted patch spec groups: 7; output: {new FileInfo(output).Length} bytes");
