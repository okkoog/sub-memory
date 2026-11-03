using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: CorpInitFix original.dll test.dll report.md");
    return 2;
}
var input = Path.GetFullPath(args[0]);
var output = Path.GetFullPath(args[1]);
var reportPath = Path.GetFullPath(args[2]);
if (input == output) throw new InvalidOperationException("Refusing to overwrite the original DLL.");
if (!File.Exists(input)) throw new FileNotFoundException("Original DLL missing", input);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

using var original = AssemblyDefinition.ReadAssembly(input, new ReaderParameters { ReadSymbols = false, InMemory = true });
using var modified = AssemblyDefinition.ReadAssembly(input, new ReaderParameters { ReadSymbols = false, InMemory = true });

if (original.Name.HasPublicKey) throw new InvalidOperationException("Signed assembly cannot be rewritten without signing key.");

static TypeDefinition TargetType(AssemblyDefinition assembly, string name) =>
    assembly.MainModule.Types.SingleOrDefault(t => t.FullName == name)
    ?? throw new InvalidOperationException("Target type not found: " + name);

static bool CallsTick(Instruction instruction) =>
    (instruction.OpCode.Code == Code.Call || instruction.OpCode.Code == Code.Callvirt)
    && instruction.Operand is MethodReference m
    && m.Name == "Tick"
    && m.DeclaringType.FullName == "CorpPresence.Runtime.ResourceBootstrap";

static IEnumerable<MethodDefinition> EnumerateMethods(TypeDefinition type)
{
    foreach (var method in type.Methods) yield return method;
    foreach (var child in type.NestedTypes)
        foreach (var method in EnumerateMethods(child)) yield return method;
}
static void MakeDeferred(Instruction call)
{
    if (!CallsTick(call)) throw new InvalidOperationException("Wrong target call.");
    // The old void instance call and pop both consume exactly one receiver.
    call.OpCode = OpCodes.Pop;
    call.Operand = null;
}

var main = TargetType(modified, "CorpPresence.Main");
var onEnable = main.Methods.Single(m => m.Name == "OnEnable" && !m.HasParameters);
var directCalls = onEnable.Body.Instructions.Where(CallsTick).ToList();
if (directCalls.Count != 1)
    throw new InvalidOperationException($"Expected 1 Main.OnEnable immediate Tick call, got {directCalls.Count}");

var bootstrap = TargetType(modified, "CorpPresence.Runtime.ResourceBootstrap");
var callbackCalls = EnumerateMethods(bootstrap)
    .Where(m => m.HasBody && m.Name.Contains("Configure", StringComparison.Ordinal)
        && m.Name != "Configure")
    .SelectMany(m => m.Body.Instructions.Where(CallsTick)
        .Select(ins => (Method: m, Instruction: ins))).ToList();
if (callbackCalls.Count != 1)
{
    var all = EnumerateMethods(bootstrap).Where(m => m.HasBody)
        .SelectMany(m => m.Body.Instructions.Where(CallsTick).Select(_ => m.FullName));
    throw new InvalidOperationException("Expected exactly 1 Configure callback Tick call; got "
        + callbackCalls.Count + ". Tick callers: " + string.Join(", ", all));
}
MakeDeferred(directCalls[0]);
MakeDeferred(callbackCalls[0].Instruction);
modified.Write(output, new WriterParameters { WriteSymbols = false });

using var result = AssemblyDefinition.ReadAssembly(output,
    new ReaderParameters { ReadSymbols = false, InMemory = true });

static string OperandKey(MethodDefinition method, object? obj)
{
    if (obj == null) return "-";
    return obj switch
    {
        Instruction target => "target#" + method.Body.Instructions.IndexOf(target),
        Instruction[] targets => "[" + string.Join(",", targets.Select(t =>
            method.Body.Instructions.IndexOf(t))) + "]",
        MethodReference mr => "method:" + mr.FullName,
        FieldReference fr => "field:" + fr.FullName,
        TypeReference tr => "type:" + tr.FullName,
        ParameterDefinition p => "parameter#" + p.Index,
        VariableDefinition v => "variable#" + v.Index,
        string s => "string:" + s,
        IFormattable n => n.ToString(null, CultureInfo.InvariantCulture) ?? "?",
        _ => obj.ToString() ?? "?"
    };
}
static Dictionary<string, MethodDefinition> BodyMap(AssemblyDefinition a) =>
    a.MainModule.Types.SelectMany(EnumerateMethods)
        .Where(m => m.HasBody).ToDictionary(m => m.FullName, m => m);

if (result.Name.FullName != original.Name.FullName ||
    result.MainModule.Resources.Count != original.MainModule.Resources.Count ||
    result.MainModule.AssemblyReferences.Count != original.MainModule.AssemblyReferences.Count ||
    result.MainModule.Types.Count != original.MainModule.Types.Count)
    throw new InvalidOperationException("Assembly identity, resources, references, or type count changed.");

var oldMethods = BodyMap(original);
var newMethods = BodyMap(result);
if (oldMethods.Count != newMethods.Count ||
    oldMethods.Keys.Except(newMethods.Keys).Any() ||
    newMethods.Keys.Except(oldMethods.Keys).Any())
    throw new InvalidOperationException("Method signatures differ.");

var differences = new List<string>();
foreach (var entry in oldMethods)
{
    var oldMethod = entry.Value;
    var newMethod = newMethods[entry.Key];
    if (oldMethod.Body.Instructions.Count != newMethod.Body.Instructions.Count)
        throw new InvalidOperationException("Instruction count changed in " + entry.Key);
    for (int i = 0; i < oldMethod.Body.Instructions.Count; i++)
    {
        var a = oldMethod.Body.Instructions[i];
        var b = newMethod.Body.Instructions[i];
        if (a.OpCode == b.OpCode &&
            OperandKey(oldMethod, a.Operand) == OperandKey(newMethod, b.Operand))
            continue;
        if (!CallsTick(a) || b.OpCode.Code != Code.Pop || b.Operand != null)
            throw new InvalidOperationException("Unexpected IL change in " + entry.Key + " instruction " + i);
        differences.Add(entry.Key + " instruction #" + i + ": " + a.OpCode.Name
            + " " + OperandKey(oldMethod, a.Operand) + " => pop");
    }
}
if (differences.Count != 2 ||
    !differences.Any(d => d.Contains("CorpPresence.Main::OnEnable(", StringComparison.Ordinal)) ||
    !differences.Any(d => d.Contains("Configure", StringComparison.Ordinal)))
    throw new InvalidOperationException("Incorrect patch count or targets: " + string.Join("; ", differences));

static string Hash(string file) =>
    Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file))).ToLowerInvariant();

var report = new List<string>
{
    "# CorpPresence v0.9.4 delayed-initialization TEST build",
    "",
    "Status: actual DLL built; static IL comparison succeeded; NOT game-runtime tested.",
    "",
    "- Input SHA256: " + Hash(input),
    "- Output SHA256: " + Hash(output),
    "- Original bytes: " + new FileInfo(input).Length,
    "- Test DLL bytes: " + new FileInfo(output).Length,
    "- Preserved assembly identity: " + result.Name.FullName,
    "- Exactly two ResourceBootstrap.Tick calls replaced by stack-equivalent pop:",
    ""
};
foreach (var diff in differences) report.Add("  - " + diff);
report.AddRange(new [] {
    "",
    "The Main.OnEnable immediate Tick and Configure item-directory delegate Tick are deferred.",
    "ResourceBootstrap.Update Tick remains intact, so polling begins in a later Unity frame.",
    "The asset initialization failure policy in GameResources.Poll has NOT been changed.",
    "No other method instruction is modified; verification tested all " + oldMethods.Count + " method bodies.",
    "Original user-uploaded CorpPresence.dll remains intact.",
    "",
    "Requires actual game validation before production use. Test the originally reported conflict pairs."
});
File.WriteAllText(reportPath, string.Join("\n", report) + "\n", Encoding.UTF8);
Console.WriteLine("VERIFIED: only two stack-equivalent call-to-pop changes.");
Console.WriteLine("ORIGINAL_SHA256=" + Hash(input));
Console.WriteLine("PATCHED_SHA256=" + Hash(output));
Console.WriteLine("REPORT=" + reportPath);
return 0;
