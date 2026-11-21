using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MidnightPawn.Game;

public static class NameGen
{
	private static readonly HashSet<string> AllowedRegions = new HashSet<string> { "中国", "日本", "韩国" };

	private static List<string[]> regions;

	private static bool loaded;

	private static void EnsureLoaded()
	{
		if (loaded)
		{
			return;
		}
		loaded = true;
		try
		{
			string text = ModHelper.ReadModFileText("lewd_meme_remake", "Content/names.txt");
			if (string.IsNullOrEmpty(text))
			{
				ModCtx.LogWarning("名字库读取失败（Content/names.txt 不存在）");
				return;
			}
			regions = new List<string[]>();
			string[] array = text.Split(new char[1] { '\n' }, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Trim();
				if (text2.Length != 0)
				{
					string[] array2 = text2.Split(new char[1] { '\t' }, StringSplitOptions.None);
					if (array2.Length >= 3 && AllowedRegions.Contains(array2[0].Trim()))
					{
						regions.Add(array2);
					}
				}
			}
			ModCtx.Log("名字库: " + regions.Count + " 个地区（仅中国/日本/韩国）");
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("名字库加载失败: " + ex.Message);
		}
	}

	public static string RandomName()
	{
		EnsureLoaded();
		if (regions == null || regions.Count == 0)
		{
			return "无名";
		}
		string[] array = regions[Random.Range(0, regions.Count)];
		string[] array2 = array[1].Split(new char[1] { ',' }, StringSplitOptions.None);
		string[] array3 = array[2].Split(new char[1] { ',' }, StringSplitOptions.None);
		if (array2.Length == 0 || array3.Length == 0)
		{
			return "无名";
		}
		string text = array2[Random.Range(0, array2.Length)].Trim();
		return string.Concat(str2: array3[Random.Range(0, array3.Length)].Trim(), str0: text, str1: IsCjk(text) ? "" : "·");
	}

	private static bool IsCjk(string s)
	{
		foreach (char c in s)
		{
			if (c >= '一' && c <= '鿿')
			{
				return true;
			}
		}
		return false;
	}
}

