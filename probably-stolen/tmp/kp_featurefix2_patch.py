import os, re, shutil
from pathlib import Path

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
p = src / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"
s = p.read_text(encoding="utf-8")

pat = re.compile(r"(?m)^(\s*)int\s+(\w+)\s*=\s*RepairFeatureCategoryNames\(\);\s*$")
m = pat.search(s)
if not m:
    raise SystemExit("RepairFeatureCategoryNames call not found")
indent = m.group(1)
insert = m.group(0) + "\n" + indent + "int conditionLookup = RepairConditionLookup();"
s = s[:m.start()] + insert + s[m.end():]

method = r'''
	private static int RepairConditionLookup()
	{
		Dictionary<string, List<ItemCondition>> conditionLookup = ItemConditionList.conditionLookup;
		if (conditionLookup == null || conditionLookup.Count == 0)
		{
			return 0;
		}
		int repaired = 0;
		HashSet<ItemCondition> seen = new HashSet<ItemCondition>();
		foreach (KeyValuePair<string, List<ItemCondition>> pair in conditionLookup)
		{
			List<ItemCondition> conditions = pair.Value;
			if (conditions == null)
			{
				continue;
			}
			foreach (ItemCondition condition in conditions)
			{
				if (condition == null || !seen.Add(condition) || !TryGetMechanicErrorKey(condition.display, out string key))
				{
					continue;
				}
				string localized = LocHelper.GetLocalizedMechanic(key);
				if (Valid(localized, key))
				{
					condition.display = localized;
					repaired++;
				}
			}
		}
		return repaired;
	}

'''
anchor = "\tprivate static int RepairFeatureCategoryNames()"
if anchor not in s:
    raise SystemExit("RepairFeatureCategoryNames method not found")
s = s.replace(anchor, method + anchor, 1)
s = s.replace("[KoreanPack] Water name compatibility skipped: ", "[KoreanPack] Localization cache repair skipped: ")
p.write_text(s, encoding="utf-8")

kp = src / "ProbablyStolenKoreanPack" / "KoreanPack.cs"
k = kp.read_text(encoding="utf-8")
k = k.replace("0.8.1-featurefix", "0.8.1-conditionfix")
kp.write_text(k, encoding="utf-8")

build = root / "build"
(build / "ProbablyStolenKoreanPack").mkdir(parents=True, exist_ok=True)
shutil.copy2(kp, build / "ProbablyStolenKoreanPack" / "KoreanPack.cs")
shutil.copy2(p, build / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs")

workspace = Path(os.environ["GITHUB_WORKSPACE"])
csproj = f'''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <AssemblyName>ProbablyStolenKoreanPack</AssemblyName>
    <RootNamespace>ProbablyStolenKoreanPack</RootNamespace>
    <LangVersion>latest</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <Deterministic>true</Deterministic>
    <AssemblyVersion>0.8.1.0</AssemblyVersion>
    <FileVersion>0.8.1.2</FileVersion>
    <InformationalVersion>0.8.1-conditionfix</InformationalVersion>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="ProbablyStolenKoreanPack/KoreanPack.cs" />
    <Compile Include="ProbablyStolenKoreanPack/WaterNameCompatibility.cs" />
    <Reference Include="Assembly-CSharp"><HintPath>{workspace}/probably-stolen/Managed/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.Localization"><HintPath>{workspace}/probably-stolen/Managed/Unity.Localization.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.ResourceManager"><HintPath>{workspace}/probably-stolen/Managed/Unity.ResourceManager.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>{workspace}/probably-stolen/Managed/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>
'''
(build / "ProbablyStolenKoreanPack.csproj").write_text(csproj, encoding="utf-8")
print("patched conditionLookup repair")
