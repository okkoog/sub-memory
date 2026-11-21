using System;
using System.Collections.Generic;
using UnityEngine;

namespace MidnightPawn.Game;

/// <summary>
/// 原版纸质书 UI（PaperUI）的关闭 / 缩放 / 翻页按钮依赖 Resources/PaperUI/PaperAssetCatalog。
/// 这个 Catalog 在本体里缺失时，按钮会退化成蓝色矩形（PaperUI 的缺图占位色）。
/// 这里在运行时画 4 个白色图标塞进本 mod 的 PaperSprites，让纸质书有一个可用的外观。
/// </summary>
internal static class PaperChromeIcons
{
	private const int Size = 128;

	private const float HalfWidth = 7f;

	private const float Feather = 1.6f;

	private static bool installed;

	public static void EnsureInstalled()
	{
		if (installed)
		{
			return;
		}
		try
		{
			LoadedMod mod = SpriteInjector.MyMod();
			if (mod == null)
			{
				return;
			}
			if (mod.PaperSprites == null)
			{
				mod.PaperSprites = new Dictionary<string, Sprite>();
			}
			Put(mod, "paper_buttons_close1", DrawClose());
			Put(mod, "paper_buttons_screen_0", DrawZoom());
			Put(mod, "paper_buttons_l_1", DrawArrow(left: true));
			Put(mod, "paper_buttons1", DrawArrow(left: false));
			installed = true;
			ModCtx.Log("纸质书按钮图标已生成：关闭 / 缩放 / 上一页 / 下一页");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("纸质书按钮图标生成失败: " + ex.Message);
		}
	}

	private static void Put(LoadedMod mod, string key, Sprite sprite)
	{
		if (sprite != null)
		{
			mod.PaperSprites[key] = sprite;
		}
	}

	private static Sprite DrawClose()
	{
		float[] alphas = new float[Size * Size];
		for (int y = 0; y < Size; y++)
		{
			for (int x = 0; x < Size; x++)
			{
				float px = x + 0.5f;
				float py = y + 0.5f;
				float d = Mathf.Min(SegmentDistance(px, py, 32f, 32f, 96f, 96f), SegmentDistance(px, py, 96f, 32f, 32f, 96f));
				alphas[y * Size + x] = Coverage(d);
			}
		}
		return Build(alphas);
	}

	private static Sprite DrawZoom()
	{
		float[] alphas = new float[Size * Size];
		float cx = 54f;
		float cy = 74f;
		float radius = 34f;
		for (int y = 0; y < Size; y++)
		{
			for (int x = 0; x < Size; x++)
			{
				float px = x + 0.5f;
				float py = y + 0.5f;
				float ring = Mathf.Abs(Vector2.Distance(new Vector2(px, py), new Vector2(cx, cy)) - radius);
				float handle = SegmentDistance(px, py, cx + 24f, cy - 24f, cx + 60f, cy - 60f);
				alphas[y * Size + x] = Mathf.Max(Coverage(ring), Coverage(handle));
			}
		}
		return Build(alphas);
	}

	private static Sprite DrawArrow(bool left)
	{
		float[] alphas = new float[Size * Size];
		Vector2 a = new Vector2(left ? 88f : 40f, 26f);
		Vector2 b = new Vector2(left ? 88f : 40f, 102f);
		Vector2 c = new Vector2(left ? 38f : 90f, 64f);
		for (int y = 0; y < Size; y++)
		{
			for (int x = 0; x < Size; x++)
			{
				alphas[y * Size + x] = TriangleCoverage(new Vector2(x + 0.5f, y + 0.5f), a, b, c);
			}
		}
		return Build(alphas);
	}

	private static float Coverage(float distance)
	{
		return Mathf.Clamp01((HalfWidth - distance) / Feather);
	}

	private static float SegmentDistance(float px, float py, float ax, float ay, float bx, float by)
	{
		float num = bx - ax;
		float num2 = by - ay;
		float num3 = px - ax;
		float num4 = py - ay;
		float num5 = num * num + num2 * num2;
		float num6 = ((num5 <= 0.0001f) ? 0f : Mathf.Clamp01((num3 * num + num4 * num2) / num5));
		float num7 = ax + num * num6 - px;
		float num8 = ay + num2 * num6 - py;
		return Mathf.Sqrt(num7 * num7 + num8 * num8);
	}

	private static float TriangleCoverage(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
	{
		float num = Edge(p, a, b);
		float num2 = Edge(p, b, c);
		float num3 = Edge(p, c, a);
		bool flag = num >= 0f && num2 >= 0f && num3 >= 0f;
		bool flag2 = num <= 0f && num2 <= 0f && num3 <= 0f;
		if (!flag && !flag2)
		{
			return 0f;
		}
		float num4 = Mathf.Min(Mathf.Abs(num), Mathf.Min(Mathf.Abs(num2), Mathf.Abs(num3)));
		return Mathf.Clamp01((num4 + 1f) / 2.2f);
	}

	private static float Edge(Vector2 p, Vector2 a, Vector2 b)
	{
		return (p.x - a.x) * (b.y - a.y) - (p.y - a.y) * (b.x - a.x);
	}

	private static Sprite Build(float[] alphas)
	{
		Texture2D texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
		texture.filterMode = FilterMode.Bilinear;
		Color[] array = new Color[Size * Size];
		for (int i = 0; i < array.Length; i++)
		{
			float num = Mathf.Clamp01(alphas[i]);
			array[i] = new Color(1f, 1f, 1f, num);
		}
		texture.SetPixels(array);
		texture.Apply();
		return Sprite.Create(texture, new Rect(0f, 0f, (float)Size, (float)Size), new Vector2(0.5f, 0.5f), Size);
	}
}
