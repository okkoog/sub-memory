from pathlib import Path
import os, shutil

root = Path(os.environ['RUNNER_TEMP'])
src = root/'src'/'ProbablyStolenKoreanPack'
kp = src/'KoreanPack.cs'
wn = src/'WaterNameCompatibility.cs'

# Restore WaterNameCompatibility to exact pre-labeler behavior.
s = wn.read_text(encoding='utf-8')
old = '''\t\t\tint num = RepairFeatureCategoryNames();\n\t\t\tRepairConditionLookup();\n\t\t\tint num2 = 0;\n\t\t\tint num3 = 0;\n\t\t\tint num4 = 0;'''
new = '''\t\t\tint num = 0;\n\t\t\tint num2 = 0;\n\t\t\tint num3 = 0;'''
if old not in s:
    raise SystemExit('water counter anchor not found')
s = s.replace(old, new, 1)
s = s.replace('''\t\t\t\t\t\tliquid.displayName = localizedMechanic;\n\t\t\t\t\t\tnum2++;''', '''\t\t\t\t\t\tliquid.displayName = localizedMechanic;\n\t\t\t\t\t\tnum++;''', 1)
s = s.replace('''\t\t\t\tif (RepairCondition(item))\n\t\t\t\t{\n\t\t\t\t\tnum3++;\n\t\t\t\t}''', '''\t\t\t\tif (RepairCondition(item))\n\t\t\t\t{\n\t\t\t\t\tnum2++;\n\t\t\t\t}''', 1)
s = s.replace('''\t\t\t\t\tif (flag)\n\t\t\t\t\t{\n\t\t\t\t\t\tnum4++;\n\t\t\t\t\t}''', '''\t\t\t\t\tif (flag)\n\t\t\t\t\t{\n\t\t\t\t\t\tnum3++;\n\t\t\t\t\t}''', 1)
s = s.replace('''\t\t\tDebug.Log("[KoreanPack] Localization cache repair: feature categories=" + num + ", liquids=" + num2 + ", conditions=" + num3 + ", saved labels=" + num4 + "; official callbacks only.");\n\t\t}\n\t\tcatch (Exception ex)\n\t\t{\n\t\t\tDebug.LogWarning("[KoreanPack] Localization cache repair skipped: " + ex.Message);''', '''\t\t\tDebug.Log("[KoreanPack] Water name compatibility: liquids=" + num + ", conditions=" + num2 + ", saved labels=" + num3 + "; official callbacks only.");\n\t\t}\n\t\tcatch (Exception ex)\n\t\t{\n\t\t\tDebug.LogWarning("[KoreanPack] Water name compatibility skipped: " + ex.Message);''', 1)

start = s.find('\n\tinternal static int RepairForLabeler(ItemFeature itemFeature)')
end = s.find('\n\tprivate bool RepairCondition(ItemCondition condition)', start)
if start < 0 or end < 0:
    raise SystemExit('label helper block in water file not found')
s = s[:start] + s[end:]
wn.write_text(s, encoding='utf-8')

# Move label repair to KoreanPack and register Harmony only after game load.
k = kp.read_text(encoding='utf-8')
old_block = '''\t\t\ttry\n\t\t\t{\n\t\t\t\tEnableLabelerConditionRepair();\n\t\t\t}\n\t\t\tcatch (Exception ex2)\n\t\t\t{\n\t\t\t\tDisableLabelerConditionRepair();\n\t\t\t\tDebug.LogWarning("[KoreanPack] Labeler condition repair unavailable: " + ex2.Message);\n\t\t\t}\n\t\t\tModHook.OnModAssetsLoaded += AssetsLoaded;'''
new_block = '''\t\t\tModHook.OnGameLoadedLate += EnableLabelerConditionRepairLate;\n\t\t\tModHook.OnModAssetsLoaded += AssetsLoaded;'''
if old_block not in k:
    raise SystemExit('OnEnable immediate label patch block not found')
k = k.replace(old_block, new_block, 1)

old_disable = '''\t\tenabled = false;\n\t\tDisableLabelerConditionRepair();\n\t\twaterNames.Disable();\n\t\tModHook.OnModAssetsLoaded -= AssetsLoaded;'''
new_disable = '''\t\tenabled = false;\n\t\tModHook.OnGameLoadedLate -= EnableLabelerConditionRepairLate;\n\t\tDisableLabelerConditionRepair();\n\t\twaterNames.Disable();\n\t\tModHook.OnModAssetsLoaded -= AssetsLoaded;'''
if old_disable not in k:
    raise SystemExit('OnDisable anchor not found')
k = k.replace(old_disable, new_disable, 1)

anchor = '\n\tprivate void EnableLabelerConditionRepair()\n'
if anchor not in k:
    raise SystemExit('EnableLabelerConditionRepair anchor not found')
late_method = '''\n\tprivate void EnableLabelerConditionRepairLate()\n\t{\n\t\tif (!enabled || labelerHarmony != null)\n\t\t{\n\t\t\treturn;\n\t\t}\n\t\ttry\n\t\t{\n\t\t\tEnableLabelerConditionRepair();\n\t\t\tDebug.Log("[KoreanPack] Labeler condition repair armed after game load.");\n\t\t}\n\t\tcatch (Exception ex)\n\t\t{\n\t\t\tDisableLabelerConditionRepair();\n\t\t\tDebug.LogWarning("[KoreanPack] Labeler condition repair unavailable: " + ex.Message);\n\t\t}\n\t}\n'''
k = k.replace(anchor, late_method + anchor, 1)
k = k.replace('''\t\t\tint num = WaterNameCompatibility.RepairForLabeler(itemFeature);''', '''\t\t\tint num = RepairForLabeler(itemFeature);''', 1)

anchor2 = '\n\tprivate static bool IsOwn(LoadedMod mod)\n'
if anchor2 not in k:
    raise SystemExit('IsOwn anchor not found')
helpers = r'''
	private static int RepairForLabeler(ItemFeature itemFeature)
	{
		if (itemFeature == null)
		{
			return 0;
		}
		int num = RepairFeatureCategory(itemFeature.category);
		num += RepairConditionCategory(itemFeature.category);
		if (RepairGenericCondition(itemFeature.fakeCondition))
		{
			num++;
		}
		if (itemFeature.realCondition != itemFeature.fakeCondition && RepairGenericCondition(itemFeature.realCondition))
		{
			num++;
		}
		return num;
	}

	private static int RepairFeatureCategory(string category)
	{
		if (string.IsNullOrEmpty(category) || ItemFeatureList.CategoryNames == null || !ItemFeatureList.CategoryNames.TryGetValue(category, out var value) || !TryGetMechanicErrorKey(value, out var key))
		{
			return 0;
		}
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key);
		if (!ValidMechanic(localizedMechanic, key))
		{
			return 0;
		}
		ItemFeatureList.CategoryNames[category] = localizedMechanic;
		return 1;
	}

	private static int RepairConditionCategory(string category)
	{
		if (string.IsNullOrEmpty(category) || ItemConditionList.conditionLookup == null || !ItemConditionList.conditionLookup.TryGetValue(category, out var value) || value == null)
		{
			return 0;
		}
		int num = 0;
		foreach (ItemCondition item in value)
		{
			if (RepairGenericCondition(item))
			{
				num++;
			}
		}
		return num;
	}

	private static bool RepairGenericCondition(ItemCondition condition)
	{
		if (condition == null || !TryGetMechanicErrorKey(condition.display, out var key))
		{
			return false;
		}
		string localizedMechanic = LocHelper.GetLocalizedMechanic(key);
		if (!ValidMechanic(localizedMechanic, key))
		{
			return false;
		}
		condition.display = localizedMechanic;
		return true;
	}

	private static bool TryGetMechanicErrorKey(string value, out string key)
	{
		key = null;
		const string prefix = "Translation Error '";
		const string suffix = "' in Mechanic";
		if (string.IsNullOrEmpty(value) || !value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith(suffix, StringComparison.Ordinal))
		{
			return false;
		}
		int length = value.Length - prefix.Length - suffix.Length;
		if (length <= 0)
		{
			return false;
		}
		key = value.Substring(prefix.Length, length);
		return !string.IsNullOrEmpty(key);
	}

	private static bool ValidMechanic(string value, string key)
	{
		return !string.IsNullOrEmpty(value) && value != key && value.IndexOf("Translation Error", StringComparison.OrdinalIgnoreCase) < 0;
	}
'''
k = k.replace(anchor2, helpers + anchor2, 1)
k = k.replace('0.8.1-labelerfix', '0.8.2-labeler-latefix', 1)
kp.write_text(k, encoding='utf-8')

build = root/'build'
(build/'ProbablyStolenKoreanPack').mkdir(parents=True, exist_ok=True)
shutil.copy2(kp, build/'ProbablyStolenKoreanPack'/'KoreanPack.cs')
shutil.copy2(wn, build/'ProbablyStolenKoreanPack'/'WaterNameCompatibility.cs')
workspace = Path(os.environ['GITHUB_WORKSPACE'])
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
    <AssemblyVersion>0.8.2.0</AssemblyVersion>
    <FileVersion>0.8.2.2</FileVersion>
    <InformationalVersion>0.8.2-labeler-latefix</InformationalVersion>
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
</Project>'''
(build/'ProbablyStolenKoreanPack.csproj').write_text(csproj, encoding='utf-8')
print('patched: original water compatibility restored; label Harmony deferred to OnGameLoadedLate')
