# RLD original CLR bridge feasibility and source-site audit

- Original methods 1859; source rows 3619
- Method signatures in dnlib 1859

- Exact method+offset+ldstr matches: 3619/3619
- Missing methods 0; offsets 0; string mismatches 0

## First mismatches

## Referenced mod localization and loader members
- System.Void RedLightDistrict.RedLightDistrictMod::OnModAssetsLoaded(LoadedMod)
  - ModManifest LoadedMod::Manifest
  - System.Collections.Generic.Dictionary`2<System.String,UnityEngine.AudioClip> LoadedMod::Sounds
  - System.Collections.Generic.Dictionary`2<System.String,UnityEngine.Sprite> LoadedMod::Sprites

## Assembly refs
- netstandard, Version=2.1.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
- UnityEngine.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
- Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- Unity.TextMeshPro, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.AudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.UnityWebRequestModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- 0Harmony, Version=2.4.2.0, Culture=neutral, PublicKeyToken=null
- Newtonsoft.Json, Version=13.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
- UnityEngine.InputLegacyModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.UIModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.TextRenderingModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.ImageConversionModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
- UnityEngine.UnityWebRequestAudioModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
