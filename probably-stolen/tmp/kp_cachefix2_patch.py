from pathlib import Path
import os, shutil
root=Path(os.environ['RUNNER_TEMP'])
src=root/'src'/'ProbablyStolenKoreanPack'
kp=src/'KoreanPack.cs'
k=kp.read_text(encoding='utf-8')

# Add table async types.
if 'using UnityEngine.Localization.Tables;' not in k:
    k=k.replace('using UnityEngine.Localization.Settings;\n', 'using UnityEngine.Localization.Settings;\nusing UnityEngine.Localization.Tables;\nusing UnityEngine.ResourceManagement.AsyncOperations;\n',1)

# Add Item-table repair generation state.
anchor='\tprivate Harmony labelerHarmony;\n\n\tprivate bool enabled;'
if anchor not in k: raise SystemExit('field anchor missing')
k=k.replace(anchor,'\tprivate Harmony labelerHarmony;\n\n\tprivate int itemTableRepairGeneration;\n\n\tprivate bool enabled;',1)

# Subscribe item locale and kick an Item-table request.
old='''\t\t\tenabled = true;\n\t\t\tFishingStartPerkCompatibility.Enable();'''
new='''\t\t\tenabled = true;\n\t\t\tLocalizationSettings.SelectedLocaleChanged += ItemLocaleChanged;\n\t\t\tFishingStartPerkCompatibility.Enable();'''
if old not in k: raise SystemExit('enable anchor missing')
k=k.replace(old,new,1)
old='''\t\t\tModHook.OnGameLoadedLate += EnableLabelerConditionRepairLate;\n\t\t\tModHook.OnModAssetsLoaded += AssetsLoaded;'''
new='''\t\t\tModHook.OnGameLoadedLate += EnableLabelerConditionRepairLate;\n\t\t\tModHook.OnModAssetsLoaded += AssetsLoaded;\n\t\t\tRequestItemTableRepair();'''
if old not in k: raise SystemExit('enable hook anchor missing')
k=k.replace(old,new,1)

# Unsubscribe and invalidate pending requests.
old='''\t\tif (!enabled)\n\t\t{\n\t\t\treturn;\n\t\t}\n\t\tenabled = false;'''
new='''\t\tif (!enabled)\n\t\t{\n\t\t\treturn;\n\t\t}\n\t\tenabled = false;\n\t\titemTableRepairGeneration++;\n\t\tLocalizationSettings.SelectedLocaleChanged -= ItemLocaleChanged;'''
idx=k.find('\tpublic void OnDisable()')
if idx<0: raise SystemExit('OnDisable missing')
head=k[:idx]; tail=k[idx:]
if old not in tail: raise SystemExit('disable anchor missing')
tail=tail.replace(old,new,1); k=head+tail

# Request again at game-late callback.
old='''\tprivate void EnableLabelerConditionRepairLate()\n\t{\n\t\tRepairConditionDisplays(includeItems: true);'''
new='''\tprivate void EnableLabelerConditionRepairLate()\n\t{\n\t\tRequestItemTableRepair();\n\t\tRepairConditionDisplays(includeItems: true);'''
if old not in k: raise SystemExit('late anchor missing')
k=k.replace(old,new,1)

# Remove unrelated Mechanic gate that blocked every repair family.
gate='''\t\t\tconst string gateKey = "mech_condition_low_purity";\n\t\t\tif (!ValidMechanic(LocHelper.GetLocalizedMechanic(gateKey), gateKey))\n\t\t\t{\n\t\t\t\treturn;\n\t\t\t}\n\n'''
if gate not in k: raise SystemExit('gate missing')
k=k.replace(gate,'',1)

# Replace Chem Finisher cache repair with LocHelper's native missing-translation parser.
start=k.find('\tprivate static int RepairChemFinisherRecipeNames()')
end=k.find('\n\tprivate static string ChemRecipeNameKey', start)
if start<0 or end<0: raise SystemExit('chem method anchors missing')
chem='''\tprivate static int RepairChemFinisherRecipeNames()\n\t{\n\t\tif (ChemFinisherRecipe.chemFinisherRecipes == null)\n\t\t{\n\t\t\treturn 0;\n\t\t}\n\t\tint count = 0;\n\t\tforeach (ChemFinisherRecipe recipe in ChemFinisherRecipe.chemFinisherRecipes)\n\t\t{\n\t\t\tif (recipe == null || !LocHelper.TryParseMissingTranslation(recipe.displayName, out var table, out var key) || !string.Equals(table, "Item", StringComparison.Ordinal))\n\t\t\t{\n\t\t\t\tcontinue;\n\t\t\t}\n\t\t\tstring localized = LocHelper.RelocalizeIfMissing(recipe.displayName);\n\t\t\tif (ValidLocalized(localized, key))\n\t\t\t{\n\t\t\t\trecipe.displayName = localized;\n\t\t\t\tcount++;\n\t\t\t}\n\t\t}\n\t\treturn count;\n\t}\n'''
k=k[:start]+chem+k[end:]
# Remove obsolete hard-coded recipe key map entirely.
start=k.find('\tprivate static string ChemRecipeNameKey')
end=k.find('\n\tprivate static bool TryGetAnyTranslationErrorKey', start)
if start<0 or end<0: raise SystemExit('chem map anchors missing')
k=k[:start]+k[end:]

# Add Item-table completion callbacks before IsOwn.
anchor='\n\tprivate static bool IsOwn(LoadedMod mod)\n'
if anchor not in k: raise SystemExit('IsOwn anchor missing')
helpers=r'''
	private void ItemLocaleChanged(Locale locale)
	{
		RequestItemTableRepair();
	}

	private void RequestItemTableRepair()
	{
		if (!enabled)
		{
			return;
		}
		Locale locale = LocalizationSettings.SelectedLocale;
		if (locale == null || (locale.Identifier.Code != "ko" && locale.Identifier.Code != "ko-KR"))
		{
			return;
		}
		int token = ++itemTableRepairGeneration;
		AsyncOperationHandle<StringTable> operation = LocalizationSettings.StringDatabase.GetTableAsync("Item", locale);
		if (operation.IsDone)
		{
			CompleteItemTableRepair(operation, locale, token);
			return;
		}
		operation.Completed += delegate(AsyncOperationHandle<StringTable> completed)
		{
			CompleteItemTableRepair(completed, locale, token);
		};
	}

	private void CompleteItemTableRepair(AsyncOperationHandle<StringTable> operation, Locale locale, int token)
	{
		if (!enabled || token != itemTableRepairGeneration || LocalizationSettings.SelectedLocale != locale || !operation.IsValid() || operation.Status != AsyncOperationStatus.Succeeded || operation.Result == null)
		{
			return;
		}
		try
		{
			int repaired = RepairChemFinisherRecipeNames();
			if (repaired > 0)
			{
				Debug.Log("[KoreanPack] Item-table cache repair: chem finisher recipe names=" + repaired + ".");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Item-table cache repair skipped: " + ex.Message);
		}
	}
'''
k=k.replace(anchor,'\n'+helpers+anchor,1)

# Request after own localization assets are ready too.
old='''\t\t\t\tBuild(mod);\n\t\t\t\tApplyAll();\n\t\t\t\twaterNames.AssetsReady();'''
new='''\t\t\t\tBuild(mod);\n\t\t\t\tApplyAll();\n\t\t\t\twaterNames.AssetsReady();\n\t\t\t\tRequestItemTableRepair();'''
if old not in k: raise SystemExit('AssetsLoaded own anchor missing')
k=k.replace(old,new,1)

k=k.replace('0.8.2-cachefix', '0.8.2-cachefix2')
kp.write_text(k,encoding='utf-8')

build=root/'build'
(build/'ProbablyStolenKoreanPack').mkdir(parents=True,exist_ok=True)
for name in ['KoreanPack.cs','WaterNameCompatibility.cs','FishingStartPerkCompatibility.cs']:
    shutil.copy2(src/name, build/'ProbablyStolenKoreanPack'/name)
workspace=Path(os.environ['GITHUB_WORKSPACE'])
csproj=f'''<Project Sdk="Microsoft.NET.Sdk">
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
    <FileVersion>0.8.2.4</FileVersion>
    <InformationalVersion>0.8.2-cachefix2</InformationalVersion>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="ProbablyStolenKoreanPack/KoreanPack.cs" />
    <Compile Include="ProbablyStolenKoreanPack/WaterNameCompatibility.cs" />
    <Compile Include="ProbablyStolenKoreanPack/FishingStartPerkCompatibility.cs" />
    <Reference Include="Assembly-CSharp"><HintPath>{workspace}/probably-stolen/Managed/Assembly-CSharp.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="0Harmony"><HintPath>{workspace}/probably-stolen/Managed/0Harmony.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.Localization"><HintPath>{workspace}/probably-stolen/Managed/Unity.Localization.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="Unity.ResourceManager"><HintPath>{workspace}/probably-stolen/Managed/Unity.ResourceManager.dll</HintPath><Private>false</Private></Reference>
    <Reference Include="UnityEngine.CoreModule"><HintPath>{workspace}/probably-stolen/Managed/UnityEngine.CoreModule.dll</HintPath><Private>false</Private></Reference>
  </ItemGroup>
</Project>'''
(build/'ProbablyStolenKoreanPack.csproj').write_text(csproj,encoding='utf-8')
print('patched cachefix2')
