using System;
using System.Collections.Generic;
using UnityEngine;

namespace CorpPresence.UI;

internal static class Ui
{
	private static readonly Dictionary<Texture2D, GUIStyle> BoxStyles = new Dictionary<Texture2D, GUIStyle>();

	private static readonly Dictionary<GUIStyle, GUIStyle> Hovers = new Dictionary<GUIStyle, GUIStyle>();

	public static Vector2 Origin { get; set; }

	public static Rect? Clip { get; set; }

	public static bool Button(Rect r, string text, GUIStyle style)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I4
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref r)).x + Origin.x, ((Rect)(ref r)).y + Origin.y, ((Rect)(ref r)).width, ((Rect)(ref r)).height);
		Rect value;
		int num;
		if (((Rect)(ref val)).Contains(Pointer.Local))
		{
			if (Clip.HasValue)
			{
				value = Clip.Value;
				num = (((Rect)(ref value)).Contains(Pointer.Local) ? 1 : 0);
			}
			else
			{
				num = 1;
			}
		}
		else
		{
			num = 0;
		}
		bool flag = (byte)num != 0;
		GUI.Box(r, text, (flag && GUI.enabled) ? HoverOf(style) : style);
		Event current = Event.current;
		if (current == null || (int)current.type != 7 || !GUI.enabled || !Pointer.PendingClick.HasValue)
		{
			return false;
		}
		Vector2 value2 = Pointer.PendingClick.Value;
		if (((Rect)(ref val)).Contains(value2))
		{
			if (Clip.HasValue)
			{
				value = Clip.Value;
				if (!((Rect)(ref value)).Contains(value2))
				{
					goto IL_0102;
				}
			}
			Pointer.PendingClick = null;
			return true;
		}
		goto IL_0102;
		IL_0102:
		return false;
	}

	public static int Qty(Rect r, int q, int max, GUIStyle button, GUIStyle label)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		float num = 24f;
		float num2 = ((Rect)(ref r)).y + (((Rect)(ref r)).height - num) / 2f;
		if (Button(new Rect(((Rect)(ref r)).x, num2, 22f, num), "«", button))
		{
			q = 0;
		}
		if (Button(new Rect(((Rect)(ref r)).x + 24f, num2, 24f, num), "－", button) && q > 0)
		{
			q--;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x + 48f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 96f, ((Rect)(ref r)).height), q.ToString(), label);
		if (Button(new Rect(((Rect)(ref r)).xMax - 48f, num2, 24f, num), "＋", button) && q < max)
		{
			q++;
		}
		if (Button(new Rect(((Rect)(ref r)).xMax - 22f, num2, 22f, num), "»", button))
		{
			q = max;
		}
		return Math.Max(0, Math.Min(q, max));
	}

	public static void ResetCaches()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Hovers.Clear();
		BoxStyles.Clear();
		Origin = default(Vector2);
		Clip = null;
	}

	public static void ClearHoverCache()
	{
		Hovers.Clear();
	}

	private static GUIStyle HoverOf(GUIStyle style)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (Hovers.TryGetValue(style, out var value) && value != null)
		{
			return value;
		}
		value = new GUIStyle(style);
		if ((Object)(object)style.hover.background != (Object)null)
		{
			value.normal.background = style.hover.background;
		}
		value.normal.textColor = ((style.hover.textColor.a > 0f) ? style.hover.textColor : style.normal.textColor);
		Hovers[style] = value;
		return value;
	}

	public static void Label(Rect r, string text, GUIStyle style, Color? color = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (color.HasValue)
		{
			Color contentColor = GUI.contentColor;
			GUI.contentColor = color.Value;
			GUI.Label(r, text, style);
			GUI.contentColor = contentColor;
		}
		else
		{
			GUI.Label(r, text, style);
		}
	}

	public static void Image(Rect r, Texture2D tex)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)tex == (Object)null))
		{
			if (!BoxStyles.TryGetValue(tex, out var value) || value == null)
			{
				value = new GUIStyle
				{
					border = new RectOffset(0, 0, 0, 0),
					padding = new RectOffset(0, 0, 0, 0)
				};
				value.normal.background = tex;
				BoxStyles[tex] = value;
			}
			GUI.Box(r, GUIContent.none, value);
		}
	}

	public static void Fill(Rect r, Color c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Image(r, Tex.Solid(c));
	}
}
