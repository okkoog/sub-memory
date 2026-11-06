import os, shutil
from pathlib import Path

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
p = src / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"
s = p.read_text(encoding="utf-8")

anchor = "int featureCategories = RepairFeatureCategoryNames();"
if anchor not in s:
    raise SystemExit("feature category anchor not found")
s = s.replace(anchor, anchor + "\n\t\t\tint conditionLookup = RepairConditionLookup();", 1)

oldlog = 'Debug.Log("[KoreanPack] Localization cache repair: feature categories=" + featureCategories + ", liquids=" + num + ", conditions=" + num2 + ", saved labels=" + num3 + "; official callbacks only.");'
newlog = 'Debug.Log("[KoreanPack] Localization cache repair: feature categories=" + featureCategories + ", condition lookup=" + conditionLookup + ", liquids=" + num + ", water conditions=" + num2 + ", saved labels=" + num3 + "; official callbacks only.");'
if oldlog not in s:
    raise SystemExit("log anchor not found")
s = s.replace(oldlog, newlog, 1)

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
anchor2 = "	private static int RepairFeatureCategoryNames()"
if anchor2 not in s:
    raise SystemExit("method insertion anchor not found")
s = s.replace(anchor2, method + anchor2, 1)
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
