from pathlib import Path
import os, shutil

root=Path(os.environ['RUNNER_TEMP'])
src=root/'src'/'ProbablyStolenKoreanPack'
kp=src/'KoreanPack.cs'
k=kp.read_text(encoding='utf-8')

# Only late-game repair; do not request/load localization tables at startup.
# Replace Chem Finisher repair to use the game's own missing-translation parser.
start=k.find('\tprivate static int RepairChemFinisherRecipeNames()')
end=k.find('\n\tprivate static string ChemRecipeNameKey', start)
if start < 0:
    raise SystemExit('RepairChemFinisherRecipeNames missing')
if end < 0:
    # decompiler may have already omitted the old map in a later base
    end=k.find('\n\tprivate static bool TryGetAnyTranslationErrorKey', start)
if end < 0:
    raise SystemExit('chem method end anchor missing')

chem='''\tprivate static int RepairChemFinisherRecipeNames()\n\t{\n\t\tif (ChemFinisherRecipe.chemFinisherRecipes == null)\n\t\t{\n\t\t\treturn 0;\n\t\t}\n\t\tint count = 0;\n\t\tforeach (ChemFinisherRecipe recipe in ChemFinisherRecipe.chemFinisherRecipes)\n\t\t{\n\t\t\tif (recipe == null || !LocHelper.TryParseMissingTranslation(recipe.displayName, out var table, out var key) || !string.Equals(table, "Item", StringComparison.Ordinal))\n\t\t\t{\n\t\t\t\tcontinue;\n\t\t\t}\n\t\t\tstring localized = LocHelper.RelocalizeIfMissing(recipe.displayName);\n\t\t\tif (ValidLocalized(localized, key))\n\t\t\t{\n\t\t\t\trecipe.displayName = localized;\n\t\t\t\tcount++;\n\t\t\t}\n\t\t}\n\t\treturn count;\n\t}\n'''
k=k[:start]+chem+k[end:]

# Remove obsolete hard-coded recipe-name map if present.
start=k.find('\tprivate static string ChemRecipeNameKey')
if start >= 0:
    end=k.find('\n\tprivate static bool TryGetAnyTranslationErrorKey', start)
    if end < 0:
        raise SystemExit('obsolete recipe map end missing')
    k=k[:start]+k[end:]

# IMPORTANT: Do not run ChemFinisherRecipe repair from generic RepairConditionDisplays,
# because that method can also be reached before gameplay via water/localization compatibility.
old='\t\t\tcount += RepairChemFinisherRecipeNames();\n'
if old in k:
    k=k.replace(old,'',1)

# Run Chem repair only from OnGameLoadedLate callback, after ordinary repair.
needle='''\tprivate void EnableLabelerConditionRepairLate()\n\t{\n\t\tRepairConditionDisplays(includeItems: true);'''
if needle not in k:
    raise SystemExit('late callback anchor missing')
replacement='''\tprivate void EnableLabelerConditionRepairLate()\n\t{\n\t\tRepairConditionDisplays(includeItems: true);\n\t\ttry\n\t\t{\n\t\t\tint chemRepaired = RepairChemFinisherRecipeNames();\n\t\t\tif (chemRepaired > 0)\n\t\t\t{\n\t\t\t\tDebug.Log("[KoreanPack] Late Item cache repair: chem finisher recipe names=" + chemRepaired + ".");\n\t\t\t}\n\t\t}\n\t\tcatch (Exception ex)\n\t\t{\n\t\t\tDebug.LogWarning("[KoreanPack] Late Chem Finisher cache repair skipped: " + ex.Message);\n\t\t}'''
k=k.replace(needle,replacement,1)

# Ensure no cachefix2 early async code exists.
for forbidden in ['GetTableAsync("Item"', 'RequestItemTableRepair', 'itemTableRepairGeneration', 'AsyncOperationHandle<StringTable>']:
    if forbidden in k:
        raise SystemExit('forbidden early async code present: '+forbidden)

k=k.replace('0.8.2-cachefix', '0.8.2-cachefix3-safe')
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
    <FileVersion>0.8.2.5</FileVersion>
    <InformationalVersion>0.8.2-cachefix3-safe</InformationalVersion>
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
print('patched safe cachefix3')
