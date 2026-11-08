from pathlib import Path
import os, shutil

root=Path(os.environ['RUNNER_TEMP'])
src=root/'src'/'ProbablyStolenKoreanPack'
kp=src/'KoreanPack.cs'
k=kp.read_text(encoding='utf-8')

# Add debug actions after enable flag only; ModHelper API is supported from OnEnable.
old='''\t\t\tenabled = true;\n\t\t\tFishingStartPerkCompatibility.Enable();'''
new='''\t\t\tenabled = true;\n\t\t\tModHelper.AddDebugFunction(Id, ModHelper.DEBUG_CATEGORY_SPAWN, "KP 테스트: 효과 모듈", SpawnEffectTestModule);\n\t\t\tModHelper.AddDebugFunction(Id, ModHelper.DEBUG_CATEGORY_SPAWN, "KP 테스트: 고착 모듈", SpawnStuckTestModule);\n\t\t\tFishingStartPerkCompatibility.Enable();'''
if old not in k:
    raise SystemExit('OnEnable anchor missing')
k=k.replace(old,new,1)

# Cleanup debug actions on disable.
old='''\t\tenabled = false;\n\t\tModHook.OnGameLoadedLate -= EnableLabelerConditionRepairLate;'''
new='''\t\tenabled = false;\n\t\tModHelper.RemoveDebugFunctions(Id);\n\t\tModHook.OnGameLoadedLate -= EnableLabelerConditionRepairLate;'''
if old not in k:
    raise SystemExit('OnDisable anchor missing')
k=k.replace(old,new,1)

anchor='''\tprivate void EnableLabelerConditionRepairLate()\n'''
methods=r'''
	private static void SpawnEffectTestModule()
	{
		try
		{
			if (EmporiumEntry.Instance == null || EmporiumEntry.Instance.invElement == null)
			{
				Debug.LogWarning("[KoreanPack] Test module spawn skipped: store inventory unavailable.");
				return;
			}
			GameItem item = DirectoryMaster.Item("system_module_efficiancy");
			ModuleEffectHelper.AddEffectToModule(item, "MODULE_EFFECT_OVERVOLTED");
			ModuleEffectHelper.AddEffectToModule(item, "MODULE_EFFECT_RECEIVER");
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item);
			Debug.Log("[KoreanPack] Spawned effect test module: efficiency + OVERVOLTED + RECEIVER.");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Effect test module spawn failed: " + ex.Message);
		}
	}

	private static void SpawnStuckTestModule()
	{
		try
		{
			if (EmporiumEntry.Instance == null || EmporiumEntry.Instance.invElement == null)
			{
				Debug.LogWarning("[KoreanPack] Test module spawn skipped: store inventory unavailable.");
				return;
			}
			GameItem item = DirectoryMaster.Item("system_module_quality");
			ModuleHelper.SetModuleStuck(item);
			GraphUtils.TryAcceptAll(EmporiumEntry.Instance.invElement, item);
			Debug.Log("[KoreanPack] Spawned stuck test module: quality.");
		}
		catch (Exception ex)
		{
			Debug.LogWarning("[KoreanPack] Stuck test module spawn failed: " + ex.Message);
		}
	}

'''
if anchor not in k:
    raise SystemExit('method insertion anchor missing')
k=k.replace(anchor,methods+anchor,1)

k=k.replace('0.8.2-cachefix3-safe', '0.8.2-cachefix3-safe-test')
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
    <FileVersion>0.8.2.6</FileVersion>
    <InformationalVersion>0.8.2-cachefix3-safe-test</InformationalVersion>
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
print('patched safe test buttons')
