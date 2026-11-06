import os, re, shutil
from pathlib import Path

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
kp = src / "ProbablyStolenKoreanPack" / "KoreanPack.cs"
wn = src / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"

k = kp.read_text(encoding="utf-8")
if "using HarmonyLib;" not in k:
    k = k.replace("using System.Text;\n", "using System.Text;\nusing HarmonyLib;\n", 1)

field_anchor = "\tprivate readonly WaterNameCompatibility waterNames = new WaterNameCompatibility();"
if field_anchor not in k:
    raise SystemExit("KoreanPack field anchor not found")
k = k.replace(field_anchor, field_anchor + "\n\n\tprivate const string LabelerHarmonyId = \"probably_stolen_korean_pack.labeler_condition_repair\";\n\n\tprivate Harmony labelerHarmony;", 1)

enable_anchor = """\t\t\tcatch (Exception ex)
\t\t\t{
\t\t\t\twaterNames.Disable();
\t\t\t\tDebug.LogWarning(\"[KoreanPack] Water name compatibility unavailable: \" + ex.Message);
\t\t\t}
\t\t\tModHook.OnModAssetsLoaded += AssetsLoaded;"""
if enable_anchor not in k:
    raise SystemExit("OnEnable anchor not found")
enable_repl = """\t\t\tcatch (Exception ex)
\t\t\t{
\t\t\t\twaterNames.Disable();
\t\t\t\tDebug.LogWarning(\"[KoreanPack] Water name compatibility unavailable: \" + ex.Message);
\t\t\t}
\t\t\ttry
\t\t\t{
\t\t\t\tEnableLabelerConditionRepair();
\t\t\t}
\t\t\tcatch (Exception ex2)
\t\t\t{
\t\t\t\tDisableLabelerConditionRepair();
\t\t\t\tDebug.LogWarning(\"[KoreanPack] Labeler condition repair unavailable: \" + ex2.Message);
\t\t\t}
\t\t\tModHook.OnModAssetsLoaded += AssetsLoaded;"""
k = k.replace(enable_anchor, enable_repl, 1)

disable_anchor = """\t\tenabled = false;
\t\twaterNames.Disable();
\t\tModHook.OnModAssetsLoaded -= AssetsLoaded;"""
if disable_anchor not in k:
    raise SystemExit("OnDisable anchor not found")
k = k.replace(disable_anchor, """\t\tenabled = false;
\t\tDisableLabelerConditionRepair();
\t\twaterNames.Disable();
\t\tModHook.OnModAssetsLoaded -= AssetsLoaded;""", 1)

method_anchor = "\tprivate static bool IsOwn(LoadedMod mod)"
if method_anchor not in k:
    raise SystemExit("method insertion anchor not found")
methods = r'''
	private void EnableLabelerConditionRepair()
	{
		if (labelerHarmony != null)
		{
			return;
		}
		var original = AccessTools.Method(typeof(LabelerElement), "InitElement", new Type[1] { typeof(ItemFeature) });
		var prefix = AccessTools.Method(typeof(KoreanPack), "BeforeLabelerInit");
		if (original == null || prefix == null)
		{
			throw new MissingMethodException("LabelerElement.InitElement repair target not found.");
		}
		labelerHarmony = new Harmony(LabelerHarmonyId);
		labelerHarmony.Patch(original, new HarmonyMethod(prefix));
	}

	private void DisableLabelerConditionRepair()
	{
		if (labelerHarmony == null)
		{
			return;
		}
		try
		{
			labelerHarmony.UnpatchAll(LabelerHarmonyId);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Labeler condition repair cleanup failed: " + ex.Message);
		}
		labelerHarmony = null;
	}

	private static void BeforeLabelerInit(ItemFeature itemFeature)
	{
		try
		{
			int num = WaterNameCompatibility.RepairForLabeler(itemFeature);
			if (num > 0)
			{
				Debug.Log("[KoreanPack] Labeler condition cache repaired on open: " + num);
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Labeler condition repair skipped: " + ex.Message);
		}
	}

'''
k = k.replace(method_anchor, methods + method_anchor, 1)
k = k.replace("0.8.1-conditionfix", "0.8.1-labelerfix")
kp.write_text(k, encoding="utf-8")

s = wn.read_text(encoding="utf-8")
insert_anchor = "\tprivate static int RepairConditionLookup()"
if insert_anchor not in s:
    raise SystemExit("WaterNameCompatibility insertion anchor not found")
helper = r'''
	internal static int RepairForLabeler(ItemFeature itemFeature)
	{
		if (itemFeature == null)
		{
			return 0;
		}
		int repaired = RepairConditionCategory(itemFeature.category);
		if (RepairGenericCondition(itemFeature.fakeCondition))
		{
			repaired++;
		}
		if (!object.ReferenceEquals(itemFeature.realCondition, itemFeature.fakeCondition) && RepairGenericCondition(itemFeature.realCondition))
		{
			repaired++;
		}
		return repaired;
	}

	private static int RepairConditionCategory(string category)
	{
		if (string.IsNullOrEmpty(category) || ItemConditionList.conditionLookup == null || !ItemConditionList.conditionLookup.TryGetValue(category, out List<ItemCondition> conditions) || conditions == null)
		{
			return 0;
		}
		int repaired = 0;
		foreach (ItemCondition condition in conditions)
		{
			if (RepairGenericCondition(condition))
			{
				repaired++;
			}
		}
		return repaired;
	}

	private static bool RepairGenericCondition(ItemCondition condition)
	{
		if (condition == null || !TryGetMechanicErrorKey(condition.display, out string key))
		{
			return false;
		}
		string localized = LocHelper.GetLocalizedMechanic(key);
		if (!Valid(localized, key))
		{
			return false;
		}
		condition.display = localized;
		return true;
	}

'''
s = s.replace(insert_anchor, helper + insert_anchor, 1)
wn.write_text(s, encoding="utf-8")

build = root / "build"
(build / "ProbablyStolenKoreanPack").mkdir(parents=True, exist_ok=True)
shutil.copy2(kp, build / "ProbablyStolenKoreanPack" / "KoreanPack.cs")
shutil.copy2(wn, build / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs")

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
    <FileVersion>0.8.1.3</FileVersion>
    <InformationalVersion>0.8.1-labelerfix</InformationalVersion>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="ProbablyStolenKoreanPack/KoreanPack.cs" />
    <Compile Include="ProbablyStolenKoreanPack/WaterNameCompatibility.cs" />
    <Reference Include="Assembly-CSharp"><HintPath>{workspace}/probably-stolen/Managed/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="0Harmony"><HintPath>{workspace}/probably-stolen/Managed/0Harmony.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.Localization"><HintPath>{workspace}/probably-stolen/Managed/Unity.Localization.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.ResourceManager"><HintPath>{workspace}/probably-stolen/Managed/Unity.ResourceManager.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>{workspace}/probably-stolen/Managed/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>
'''
(build / "ProbablyStolenKoreanPack.csproj").write_text(csproj, encoding="utf-8")
print("patched targeted labeler repair")
