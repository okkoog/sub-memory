using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MidnightPawn.Game;

public static class SpriteInjector
{

	private static LoadedMod myMod;

	private static float? basePpu;

	private static readonly Dictionary<string, Sprite> injected = new Dictionary<string, Sprite>();

	private static readonly Dictionary<string, Sprite> modTexturePortraits = new Dictionary<string, Sprite>();

	private static Vector2? basePivot;

	public static LoadedMod MyMod()
	{
		if (myMod != null)
		{
			return myMod;
		}
		try
		{
			List<LoadedMod> mods = ModLoader.Mods;
			if (mods != null)
			{
				for (int i = 0; i < mods.Count; i++)
				{
					if (mods[i] != null && mods[i].Manifest != null && mods[i].Manifest.ID == "lewd_meme_remake")
					{
						myMod = mods[i];
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 取 LoadedMod 失败: " + ex.Message);
		}
		return myMod;
	}

	public static float BasePpu()
	{
		if (basePpu.HasValue)
		{
			return basePpu.Value;
		}
		basePpu = 1f;
		try
		{
			string[] array = new string[5] { "postit", "water_bottle", "scrap_metal", "cup_noodle", "rusty_can" };
			foreach (string text in array)
			{
				if (!DirectoryMaster.Has<GameItem>(text))
				{
					continue;
				}
				GameItem val = DirectoryMaster.Item(text, true);
				if (val != null)
				{
					Sprite val2 = RenderHandler.TryLoadFromAtlas(val.spriteAtlasPath, val.spritePath);
					if ((Object)(object)val2 != (Object)null && val2.pixelsPerUnit > 0f)
					{
						basePpu = val2.pixelsPerUnit;
						ModCtx.Log("SpriteInjector: 基准 PPU = " + basePpu.Value + " (来自 " + text + ")");
						return basePpu.Value;
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 探测 PPU 失败: " + ex.Message);
		}
		ModCtx.Log("SpriteInjector: 基准 PPU 回退 = " + basePpu.Value);
		return basePpu.Value;
	}

	public static string Inject(string key, Sprite sprite)
	{
		if ((Object)(object)sprite == (Object)null)
		{
			return null;
		}
		LoadedMod val = MyMod();
		if (val == null)
		{
			ModCtx.LogWarning("SpriteInjector: 找不到本模组 LoadedMod，无法注入 " + key);
			return null;
		}
		((Object)sprite).name = key;
		try
		{
			if (val.Sprites == null)
			{
				val.Sprites = new Dictionary<string, Sprite>();
			}
			val.Sprites[key] = sprite;
			injected[key] = sprite;
			return key;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 注入 " + key + " 失败: " + ex.Message);
			return null;
		}
	}


	public static Sprite MakeSprite(Texture2D tex, float ppu)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)tex == (Object)null)
		{
			return null;
		}
		return Sprite.Create(tex, new Rect(0f, 0f, (float)((Texture)tex).width, (float)((Texture)tex).height), BasePivot(), Mathf.Max(0.01f, ppu), 0u, (SpriteMeshType)0);
	}

	public static Vector2 BasePivot()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		if (basePivot.HasValue)
		{
			return basePivot.Value;
		}
		basePivot = new Vector2(0.5f, 0.5f);
		try
		{
			string[] array = new string[5] { "postit", "water_bottle", "scrap_metal", "cup_noodle", "rusty_can" };
			foreach (string text in array)
			{
				if (!DirectoryMaster.Has<GameItem>(text))
				{
					continue;
				}
				GameItem val = DirectoryMaster.Item(text, true);
				if (val == null)
				{
					continue;
				}
				Sprite val2 = RenderHandler.TryLoadFromAtlas(val.spriteAtlasPath, val.spritePath);
				if ((Object)(object)val2 == (Object)null)
				{
					continue;
				}
				Rect rect = val2.rect;
				if (!(rect.width <= 0f))
				{
					rect = val2.rect;
					if (!(rect.height <= 0f))
					{
						float x = val2.pivot.x;
						rect = val2.rect;
						float num = x / rect.width;
						float y = val2.pivot.y;
						rect = val2.rect;
						basePivot = new Vector2(num, y / rect.height);
						string[] obj = new string[5] { "SpriteInjector: 基准 pivot = ", null, null, null, null };
						Vector2 value = basePivot.Value;
						obj[1] = ((object)value).ToString();
						obj[2] = " (来自 ";
						obj[3] = text;
						obj[4] = ")";
						ModCtx.Log(string.Concat(obj));
						value = basePivot.Value;
						return value;
					}
				}
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 探测 pivot 失败: " + ex.Message);
		}
		return basePivot.Value;
	}

	public static Sprite GetOriginalPortrait(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		Sprite replacement = FindModSpriteOverride(key);
		if ((Object)(object)replacement != (Object)null)
		{
			ModCtx.Log("SpriteInjector: 立绘命中其他 mod 替换 " + key);
			return replacement;
		}
		try
		{
			if ((Object)(object)SpriteDict.Instance != (Object)null)
			{
				return SpriteDict.Instance.GetSprite(key);
			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 取立绘 " + key + " 失败: " + ex.Message);
		}
		return null;
	}

	private static Sprite FindModSpriteOverride(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		Sprite fallback = null;
		try
		{
			List<LoadedMod> mods = ModLoader.Mods;
			if (mods == null)
			{
				return null;
			}
			string[] candidates = new string[5]
			{
				key,
				key + "_portrait",
				"portrait_" + key,
				key + "_replace",
				key + "_replacement"
			};
			for (int i = mods.Count - 1; i >= 0; i--)
			{
				LoadedMod loadedMod = mods[i];
				if (loadedMod == null || loadedMod.Manifest == null || loadedMod.Manifest.ID == "lewd_meme_remake" || !loadedMod.IsEnabled)
				{
					continue;
				}
				bool hasSprites = loadedMod.Sprites != null && loadedMod.Sprites.Count > 0;
				bool hasTextures = loadedMod.Textures != null && loadedMod.Textures.Count > 0;
				if (!hasSprites && !hasTextures)
				{
					continue;
				}
				foreach (string candidate in candidates)
				{
					if (hasSprites && loadedMod.Sprites.TryGetValue(candidate, out var value) && IsValidSprite(value))
					{
						return value;
					}
					if (hasTextures && loadedMod.Textures.TryGetValue(candidate, out Texture2D texture) && (Object)(object)texture != (Object)null)
					{
						string text = loadedMod.Manifest.ID + ":" + candidate;
						if (!modTexturePortraits.TryGetValue(text, out Sprite value2) || !IsValidSprite(value2))
						{
							value2 = MakeSprite(texture, BasePpu());
							if (IsValidSprite(value2))
							{
								modTexturePortraits[text] = value2;
							}
						}
						if (IsValidSprite(value2))
						{
							return value2;
						}
					}
				}
				if (hasSprites)
				{
					foreach (KeyValuePair<string, Sprite> pair in loadedMod.Sprites)
					{
						if (!IsPortraitReplacementKey(pair.Key, key) || !IsValidSprite(pair.Value))
						{
							continue;
						}
						if (IsStrongPortraitReplacementKey(pair.Key, key))
						{
							return pair.Value;
						}
						if ((Object)(object)fallback == (Object)null)
						{
							fallback = pair.Value;
						}
					}
				}
				if (hasTextures)
				{
					foreach (KeyValuePair<string, Texture2D> pair2 in loadedMod.Textures)
					{
						if (!IsPortraitReplacementKey(pair2.Key, key) || (Object)(object)pair2.Value == (Object)null)
						{
							continue;
						}
						string text2 = loadedMod.Manifest.ID + ":" + pair2.Key;
						if (!modTexturePortraits.TryGetValue(text2, out Sprite value3) || !IsValidSprite(value3))
						{
							value3 = MakeSprite(pair2.Value, BasePpu());
							if (IsValidSprite(value3))
							{
								modTexturePortraits[text2] = value3;
							}
						}
						if (IsValidSprite(value3))
						{
							if (IsStrongPortraitReplacementKey(pair2.Key, key))
							{
								return value3;
							}
							if ((Object)(object)fallback == (Object)null)
							{
								fallback = value3;
							}
						}
					}
				}

			}
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 扫描立绘替换失败: " + ex.Message);
		}
		return fallback;
	}

	private static bool IsPortraitReplacementKey(string candidate, string key)
	{
		if (string.IsNullOrEmpty(candidate) || string.IsNullOrEmpty(key))
		{
			return false;
		}
		if (candidate.Equals(key, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		int num = candidate.IndexOf(key, StringComparison.OrdinalIgnoreCase);
		if (num < 0)
		{
			return false;
		}
		if (IsStrongPortraitReplacementKey(candidate, key))
		{
			return true;
		}
		string text = candidate.Substring(0, num);
		string text2 = candidate.Substring(num + key.Length);
		return HasPortraitSeparator(text) || HasPortraitSeparator(text2);
	}

	private static bool IsStrongPortraitReplacementKey(string candidate, string key)
	{
		if (string.IsNullOrEmpty(candidate))
		{
			return false;
		}
		return candidate.IndexOf("portrait", StringComparison.OrdinalIgnoreCase) >= 0 || candidate.IndexOf("replace", StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private static bool HasPortraitSeparator(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		return text.EndsWith("_", StringComparison.Ordinal) || text.EndsWith("-", StringComparison.Ordinal) || text.EndsWith(".", StringComparison.Ordinal) || text.StartsWith("_", StringComparison.Ordinal) || text.StartsWith("-", StringComparison.Ordinal) || text.StartsWith(".", StringComparison.Ordinal);
	}

	private static Texture2D RenderFit(Sprite src, int w, int h)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		Texture2D texture = src.texture;
		Rect rect = src.rect;
		if ((Object)(object)texture == (Object)null || rect.width < 1f || rect.height < 1f)
		{
			return null;
		}
		int num = Mathf.Max(1, (int)rect.width);
		int num2 = Mathf.Max(1, (int)rect.height);
		float num3 = Mathf.Min((float)w / (float)num, (float)h / (float)num2);
		int num4 = Mathf.Max(1, Mathf.RoundToInt((float)num * num3));
		int num5 = Mathf.Max(1, Mathf.RoundToInt((float)num2 * num3));
		RenderTexture active = RenderTexture.active;
		RenderTexture val = null;
		RenderTexture val2 = null;
		try
		{
			val = RenderTexture.GetTemporary(num, num2, 0, (RenderTextureFormat)0);
			((Texture)val).filterMode = (FilterMode)1;
			Graphics.Blit((Texture)(object)texture, val, new Vector2(rect.width / (float)((Texture)texture).width, rect.height / (float)((Texture)texture).height), new Vector2(rect.x / (float)((Texture)texture).width, rect.y / (float)((Texture)texture).height));
			val2 = RenderTexture.GetTemporary(num4, num5, 0, (RenderTextureFormat)0);
			((Texture)val2).filterMode = (FilterMode)1;
			Graphics.Blit((Texture)(object)val, val2);
			RenderTexture.active = val2;
			Texture2D val3 = new Texture2D(w, h, (TextureFormat)4, false);
			((Texture)val3).filterMode = (FilterMode)1;
			Color32[] pixels = (Color32[])(object)new Color32[w * h];
			val3.SetPixels32(pixels);
			val3.ReadPixels(new Rect(0f, 0f, (float)num4, (float)num5), (w - num4) / 2, (h - num5) / 2);
			val3.Apply();
			Color[] pixels2 = val3.GetPixels();
			bool flag = false;
			for (int i = 0; i < pixels2.Length; i += 29)
			{
				if (pixels2[i].a > 0.02f)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				ModCtx.LogWarning("立绘抓取结果全透明（Blit 采样未命中）");
				return null;
			}
			ModCtx.Log("立绘抓取成功: " + num + "x" + num2 + " -> " + num4 + "x" + num5);
			return val3;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("SpriteInjector: 缩放立绘失败: " + ex.Message);
			return null;
		}
		finally
		{
			RenderTexture.active = active;
			if ((Object)(object)val != (Object)null)
			{
				RenderTexture.ReleaseTemporary(val);
			}
			if ((Object)(object)val2 != (Object)null)
			{
				RenderTexture.ReleaseTemporary(val2);
			}
		}
	}

	public static string InjectResizedSprite(Sprite src, int w, int h, string key)
	{
		if ((Object)(object)src == (Object)null)
		{
			return null;
		}
		if (injected.ContainsKey(key))
		{
			return key;
		}
		Texture2D val = RenderFit(src, w, h);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		return Inject(key, MakeSprite(val, BasePpu()));
	}

	public static GameItem FindVanillaItem(params string[] keywords)
	{
		string[] array = new string[5] { "MiscItemDirectory", "MedsItemDirectory", "FoodItemDirectory", "MaterialDirectory", "ContainerItemDirectory" };
		foreach (string text in array)
		{
			try
			{
				List<string> identifierList = DirectoryMaster.GetIdentifierList<GameItem>(text);
				if (identifierList == null)
				{
					continue;
				}
				foreach (string item in identifierList)
				{
					if (string.IsNullOrEmpty(item))
					{
						continue;
					}
					bool flag = false;
					foreach (string value in keywords)
					{
						if (item.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						GameItem val = DirectoryMaster.Item(item, true);
						if (val != null)
						{
							ModCtx.Log("SpriteInjector: 命中原版物品 " + item + " (" + val.name + ")");
							return val;
						}
					}
				}
			}
			catch
			{
			}
		}
		return null;
	}

	/// <summary>生成纸面/日志用的小立绘：先裁掉透明边再撑满正方形，避免人物缩成一小块。key 独立，不会覆盖苗床立绘。</summary>
	public static string InjectPortraitThumb(string source, int size, string key)
	{
		if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (injected.ContainsKey(key))
		{
			return key;
		}
		try
		{
			Sprite val = ResolveSprite(source);
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			Texture2D val2 = RenderTrimFit(val, size, size);
			if ((Object)(object)val2 == (Object)null)
			{
				return null;
			}
			return Inject(key, MakeSprite(val2, BasePpu()));
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("生成日志缩略立绘失败: " + ex.Message);
			return null;
		}
	}

	public static Sprite ResolveSprite(string source)
	{
		if (string.IsNullOrEmpty(source))
		{
			return null;
		}
		int num = source.IndexOf(':');
		if (num > 0)
		{
			string text = source.Substring(0, num);
			string key = source.Substring(num + 1);
			try
			{
				LoadedMod val = MyMod();
				List<LoadedMod> mods = ModLoader.Mods;
				if (mods != null)
				{
					for (int i = 0; i < mods.Count; i++)
					{
						if (mods[i] != null && mods[i].Manifest != null && mods[i].Manifest.ID == text)
						{
							val = mods[i];
							break;
						}
					}
				}
				if (val != null && val.Sprites != null && val.Sprites.TryGetValue(key, out var value) && (Object)(object)value != (Object)null)
				{
					return value;
				}
			}
			catch (Exception ex)
			{
				ModCtx.LogWarning("取其他 mod 精灵失败: " + ex.Message);
			}
			return null;
		}
		return GetOriginalPortrait(source);
	}

	private static bool IsValidSprite(Sprite sprite)
	{
		if ((Object)(object)sprite == (Object)null)
		{
			return false;
		}
		try
		{
			return (Object)(object)sprite.texture != (Object)null && sprite.rect.width > 0f && sprite.rect.height > 0f;
		}
		catch
		{
			return false;
		}
	}

	private static Texture2D RenderTrimFit(Sprite src, int maxW, int maxH)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Expected O, but got Unknown
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		Texture2D texture = src.texture;
		Rect rect = src.rect;
		if ((Object)(object)texture == (Object)null || rect.width < 1f || rect.height < 1f)
		{
			return null;
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture val = null;
		Texture2D val2 = null;
		try
		{
			val = RenderTexture.GetTemporary(((Texture)texture).width, ((Texture)texture).height, 0, (RenderTextureFormat)0);
			((Texture)val).filterMode = (FilterMode)0;
			Graphics.Blit((Texture)(object)texture, val);
			RenderTexture.active = val;
			val2 = new Texture2D(((Texture)texture).width, ((Texture)texture).height, (TextureFormat)4, false);
			val2.ReadPixels(new Rect(0f, 0f, (float)((Texture)texture).width, (float)((Texture)texture).height), 0, 0);
			val2.Apply();
			RenderTexture.active = active;
			int num = Mathf.Clamp((int)rect.width, 1, ((Texture)texture).width);
			int num2 = Mathf.Clamp((int)rect.height, 1, ((Texture)texture).height);
			int num3 = Mathf.Clamp((int)rect.x, 0, ((Texture)texture).width - num);
			int num4 = Mathf.Clamp((int)rect.y, 0, ((Texture)texture).height - num2);
			Color[] pixels = val2.GetPixels(num3, num4, num, num2);
			int num5 = num;
			int num6 = num2;
			int num7 = -1;
			int num8 = -1;
			for (int i = 0; i < num2; i++)
			{
				int num9 = i * num;
				for (int j = 0; j < num; j++)
				{
					if (!(pixels[num9 + j].a <= 0.05f))
					{
						if (j < num5)
						{
							num5 = j;
						}
						if (j > num7)
						{
							num7 = j;
						}
						if (i < num6)
						{
							num6 = i;
						}
						if (i > num8)
						{
							num8 = i;
						}
					}
				}
			}
			if (num7 < 0)
			{
				ModCtx.LogWarning("立绘全透明，放弃");
				return null;
			}
			int num10 = num7 - num5 + 1;
			int num11 = num8 - num6 + 1;
			float num12 = Mathf.Min((float)maxW / (float)num10, (float)maxH / (float)num11);
			int num13 = Mathf.Max(1, Mathf.RoundToInt((float)num10 * num12));
			int num14 = Mathf.Max(1, Mathf.RoundToInt((float)num11 * num12));
			Color[] array = (Color[])(object)new Color[maxW * maxH];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = new Color(0f, 0f, 0f, 0f);
			}
			int num15 = (maxW - num13) / 2;
			int num16 = (maxH - num14) / 2;
			bool upscale = num12 >= 1f;
			for (int l = 0; l < num14; l++)
			{
				for (int m = 0; m < num13; m++)
				{
					Color color3;
					if (upscale)
					{
						color3 = SampleBilinear(pixels, num, ((float)m + 0.5f) / num12, ((float)l + 0.5f) / num12, num5, num6, num10, num11);
					}
					else
					{
						color3 = SampleArea(pixels, num, (float)m / num12, (float)l / num12, (float)(m + 1) / num12, (float)(l + 1) / num12, num5, num6, num10, num11);
					}
					array[(num16 + l) * maxW + (num15 + m)] = color3;
				}
			}
			Texture2D val3 = new Texture2D(maxW, maxH, (TextureFormat)4, false);
			((Texture)val3).filterMode = (FilterMode)1;
			val3.SetPixels(array);
			val3.Apply();
			CheckCentered(array, maxW, maxH);
			ModCtx.Log($"立绘裁边: 原 {num}x{num2} 内容 {num10}x{num11} -> {num13}x{num14} (画布 {maxW}x{maxH})");
			return val3;
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("立绘裁边失败: " + ex.Message);
			return null;
		}
		finally
		{
			RenderTexture.active = active;
			if ((Object)(object)val != (Object)null)
			{
				RenderTexture.ReleaseTemporary(val);
			}
			if ((Object)(object)val2 != (Object)null)
			{
				Object.Destroy((Object)(object)val2);
			}
		}
	}

	private static void CheckCentered(Color[] pixels, int width, int height)
	{
		int num = width;
		int num2 = height;
		int num3 = -1;
		int num4 = -1;
		for (int i = 0; i < height; i++)
		{
			int num5 = i * width;
			for (int j = 0; j < width; j++)
			{
				if (!(pixels[num5 + j].a <= 0.05f))
				{
					if (j < num)
					{
						num = j;
					}
					if (j > num3)
					{
						num3 = j;
					}
					if (i < num2)
					{
						num2 = i;
					}
					if (i > num4)
					{
						num4 = i;
					}
				}
			}
		}
		if (num3 >= 0)
		{
			float num6 = (float)(num + num3) * 0.5f;
			float num7 = (float)(num2 + num4) * 0.5f;
			float num8 = (float)(width - 1) * 0.5f;
			float num9 = (float)(height - 1) * 0.5f;
			if (Mathf.Abs(num6 - num8) > 2f || Mathf.Abs(num7 - num9) > 2f)
			{
				ModCtx.LogWarning($"立绘居中自检异常: 内容包围盒 ({num},{num2})-({num3},{num4}) 画布 {width}x{height} 偏移 ({num6 - num8:0.#},{num7 - num9:0.#})");
			}
		}
	}

	private static Color SampleBilinear(Color[] src, int stride, float x, float y, int originX, int originY, int w, int h)
	{
		x -= 0.5f;
		y -= 0.5f;
		int num = Mathf.FloorToInt(x);
		int num2 = Mathf.FloorToInt(y);
		float num3 = x - (float)num;
		float num4 = y - (float)num2;
		int num5 = Mathf.Clamp(num, 0, w - 1);
		int num6 = Mathf.Clamp(num + 1, 0, w - 1);
		int num7 = Mathf.Clamp(num2, 0, h - 1);
		int num8 = Mathf.Clamp(num2 + 1, 0, h - 1);
		int num9 = (originY + num7) * stride + originX;
		int num10 = (originY + num8) * stride + originX;
		Color color = src[num9 + num5];
		Color color2 = src[num9 + num6];
		Color color3 = src[num10 + num5];
		Color color4 = src[num10 + num6];
		float num11 = (1f - num3) * (1f - num4);
		float num12 = num3 * (1f - num4);
		float num13 = (1f - num3) * num4;
		float num14 = num3 * num4;
		float num15 = color.a * num11 + color2.a * num12 + color3.a * num13 + color4.a * num14;
		if (num15 <= 0.0001f)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		float num16 = (color.r * color.a * num11 + color2.r * color2.a * num12 + color3.r * color3.a * num13 + color4.r * color4.a * num14) / num15;
		float num17 = (color.g * color.a * num11 + color2.g * color2.a * num12 + color3.g * color3.a * num13 + color4.g * color4.a * num14) / num15;
		float num18 = (color.b * color.a * num11 + color2.b * color2.a * num12 + color3.b * color3.a * num13 + color4.b * color4.a * num14) / num15;
		return new Color(num16, num17, num18, num15);
	}

	private static Color SampleArea(Color[] src, int stride, float x0, float y0, float x1, float y1, int originX, int originY, int w, int h)
	{
		int num = Mathf.Clamp(Mathf.FloorToInt(x0), 0, w - 1);
		int num2 = Mathf.Clamp(Mathf.CeilToInt(x1) - 1, num, w - 1);
		int num3 = Mathf.Clamp(Mathf.FloorToInt(y0), 0, h - 1);
		int num4 = Mathf.Clamp(Mathf.CeilToInt(y1) - 1, num3, h - 1);
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		int num9 = 0;
		for (int i = num3; i <= num4; i++)
		{
			int num10 = (originY + i) * stride + originX;
			for (int j = num; j <= num2; j++)
			{
				Color color = src[num10 + j];
				num5 += color.r * color.a;
				num6 += color.g * color.a;
				num7 += color.b * color.a;
				num8 += color.a;
				num9++;
			}
		}
		if (num8 <= 0.0001f || num9 == 0)
		{
			return new Color(0f, 0f, 0f, 0f);
		}
		return new Color(num5 / num8, num6 / num8, num7 / num8, num8 / (float)num9);
	}
}



