using Object = UnityEngine.Object;
using System;
using UnityEngine;

namespace MidnightPawn.Game;

public static class Toast
{
	public const string WINDOW_ID = "lewd_meme_remake.toast";

	public static void Show(string text, string title = "触手")
	{
		ModCtx.Log("提示: " + text);
		try
		{
			if ((Object)(object)CustomUIManager.Instance == (Object)null)
			{
				return;
			}
			CustomUIManager.Instance.CloseWindow("lewd_meme_remake.toast");
			int length = ((text != null) ? text.Length : 0);
			float num = ((length > 40) ? 560f : 420f);
			float num2 = num - 48f;
			int maxChars = Mathf.Max(10, (int)(num2 / 17f));
			int num3 = CountLines(text, maxChars);
			float num4 = Mathf.Clamp(96f + (float)num3 * 22f, 150f, 620f);
			CustomUIBuilder builder = CustomUIManager.Window("lewd_meme_remake.toast", title, "overlay").SetSize(num, num4).Center()
				.SetDraggable(true)
				.SetCloseOnEscape(true);
			builder.AddLabel(text, (string)null);
			CustomUIElement last = builder.Last;
			if (last != null)
			{
				last.SetFixedWidth(num2);
				if (last.text != null)
				{
					last.text.enableWordWrapping = true;
				}
			}
			builder.AddSpace(8f);
			builder.AddButton("知道了", (Action)delegate
			{
				CustomUIManager.Instance.CloseWindow("lewd_meme_remake.toast");
			}, (string)null);
			builder.Show();
		}
		catch (Exception ex)
		{
			ModCtx.LogWarning("Toast 弹出失败: " + ex.Message);
		}
	}

	private static int CountLines(string text, int maxChars)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 1;
		}
		int num = 0;
		string[] array = text.Split(new char[1] { '\n' }, StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			num += Mathf.Max(1, Mathf.CeilToInt((float)array[i].Length / (float)maxChars));
		}
		return num;
	}
}

