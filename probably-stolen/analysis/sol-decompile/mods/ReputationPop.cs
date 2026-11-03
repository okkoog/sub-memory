using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETStandard,Version=v2.1", FrameworkDisplayName = ".NET Standard 2.1")]
[assembly: AssemblyCompany("ReputationPop")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("ReputationPop")]
[assembly: AssemblyTitle("ReputationPop")]
[assembly: AssemblyVersion("1.0.0.0")]
[module: RefSafetyRules(11)]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace System.Runtime.CompilerServices
{
	[CompilerGenerated]
	[Embedded]
	[AttributeUsage(AttributeTargets.Module, AllowMultiple = false, Inherited = false)]
	internal sealed class RefSafetyRulesAttribute : Attribute
	{
		public readonly int Version;

		public RefSafetyRulesAttribute(int P_0)
		{
			Version = P_0;
		}
	}
}
namespace ReputationPop
{
	public class ModEntry : IMod
	{
		public const string ModId = "reputation_pop";

		internal static ModLog Log;

		public void Init(ModManifest manifest)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Log = new ModLog(manifest);
			Log.Log((object)"Reputation Pop initialized.");
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}

		internal static string L(string key, params object[] args)
		{
			return ModHelper.GetLocalized("reputation_pop", key, args);
		}
	}
	public static class Pop
	{
		private class Layer
		{
			public TreeNodeRender n;

			public float dx;

			public float dy;

			public float delay;

			public Color c;
		}

		private class Spark
		{
			public TreeNodeRender n;

			public Vector2 p;

			public Vector2 v;

			public float life;

			public float age;

			public Color c;
		}

		private static readonly Dictionary<string, double> pending = new Dictionary<string, double>();

		private static bool scheduled;

		public static void Record(string faction, double delta)
		{
			if (Math.Abs(delta) < 0.005)
			{
				return;
			}
			pending.TryGetValue(faction, out var value);
			pending[faction] = value + delta;
			if (!scheduled)
			{
				StoreUIManager instance = StoreUIManager.Instance;
				if ((Object)(object)instance == (Object)null)
				{
					pending.Clear();
					return;
				}
				scheduled = true;
				((MonoBehaviour)instance).StartCoroutine(Flush());
			}
		}

		private static IEnumerator Flush()
		{
			yield return (object)new WaitForSecondsRealtime(0.12f);
			List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>(pending);
			pending.Clear();
			scheduled = false;
			int num = 0;
			foreach (KeyValuePair<string, double> item in list)
			{
				try
				{
					Spawn(item.Key, item.Value, num);
				}
				catch (Exception ex)
				{
					if (ModEntry.Log != null)
					{
						ModEntry.Log.Error((object)("spawn failed: " + ex));
					}
				}
				num++;
			}
		}

		private static void Spawn(string faction, double delta, int index)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			double num = Math.Round(Math.Abs(delta), 2);
			if (!(num <= 0.0))
			{
				string main = ((delta > 0.0) ? "+" : "-") + num.ToString("0.##", CultureInfo.InvariantCulture);
				string sub = ModEntry.L("rep_label", StoreReputation.GetFactionNameByID(faction));
				Vector2 start = StartPos();
				start.y += (float)index * 20f * Scale();
				StoreUIManager instance = StoreUIManager.Instance;
				if ((Object)(object)instance != (Object)null)
				{
					((MonoBehaviour)instance).StartCoroutine(Run(start, main, sub, Bubble(faction), delta > 0.0));
				}
			}
		}

		private static float Scale()
		{
			float num = 1f;
			try
			{
				if ((Object)(object)RenderHandler.current != (Object)null && (Object)(object)RenderHandler.current.windowUICanvas != (Object)null)
				{
					num = RenderHandler.current.windowUICanvas.scaleFactor;
				}
			}
			catch (Exception)
			{
			}
			if (!(num > 0f))
			{
				return 1f;
			}
			return num;
		}

		private static Vector2 StartPos()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			float num = LetterboxManager.GameAreaX;
			float num2 = LetterboxManager.GameAreaY;
			float num3 = LetterboxManager.GameAreaWidth;
			float num4 = LetterboxManager.GameAreaHeight;
			return new Vector2(num + num3 * 0.5f, num2 + num4 * 0.575f);
		}

		private static Color Bubble(string faction)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			switch (faction)
			{
			case "FACTION_SECURITY":
				return new Color(0.93f, 0.22f, 0.22f, 1f);
			case "FACTION_BLACK_MARKET":
				return new Color(0.07f, 0.07f, 0.09f, 1f);
			case "FACTION_REVOLUTION":
				return new Color(0.25f, 0.82f, 0.35f, 1f);
			case "FACTION_UPPER_LEVEL":
			case "FACTION_TOURIST":
				return new Color(1f, 0.5f, 0.72f, 1f);
			default:
				return new Color(0.72f, 0.74f, 0.78f, 1f);
			}
		}

		private static bool IsDark(Color c)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			return c.r * 0.3f + c.g * 0.59f + c.b * 0.11f < 0.25f;
		}

		private static Layer Make(string text, float size, Color c, float dx, float dy, float delay)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			TreeNodeRender val = TreeNodeRender.InstantiateText().SetParent(RenderHandler.current.overlayRoot);
			if ((Object)(object)val == (Object)null || (Object)(object)val.text == (Object)null)
			{
				return null;
			}
			((TMP_Text)val.text).text = text;
			((TMP_Text)val.text).fontSize = size;
			((TMP_Text)val.text).enableAutoSizing = false;
			((TMP_Text)val.text).alignment = (TextAlignmentOptions)514;
			((TMP_Text)val.text).fontStyle = (FontStyles)1;
			((Graphic)val.text).color = c;
			((Graphic)val.text).raycastTarget = false;
			((Behaviour)val.text).enabled = true;
			val.rectTransform.pivot = new Vector2(0.5f, 0.5f);
			val.rectTransform.sizeDelta = new Vector2(240f, 60f);
			return new Layer
			{
				n = val,
				dx = dx,
				dy = dy,
				delay = delay,
				c = c
			};
		}

		private static Spark MakeSpark(Color col, bool up, float s)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			TreeNodeRender val = TreeNodeRender.Instantiate().SetParent(RenderHandler.current.overlayRoot);
			if ((Object)(object)val == (Object)null || (Object)(object)val.image == (Object)null)
			{
				return null;
			}
			val.image.sprite = null;
			((Graphic)val.image).raycastTarget = false;
			((Behaviour)val.image).enabled = true;
			float num = Random.Range(1.4f, 2.6f);
			val.rectTransform.pivot = new Vector2(0.5f, 0.5f);
			val.rectTransform.sizeDelta = new Vector2(num, num);
			float num2 = (up ? Random.Range(15f, 165f) : Random.Range(200f, 340f)) * (MathF.PI / 180f);
			float num3 = Random.Range(40f, 95f) * s * (up ? 1f : 0.5f);
			Color val2 = Color.Lerp(col, Color.white, IsDark(col) ? (0.6f + Random.value * 0.35f) : (Random.value * 0.6f));
			((Graphic)val.image).color = val2;
			return new Spark
			{
				n = val,
				v = new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * num3,
				life = Random.Range(0.6f, 1.1f),
				c = val2
			};
		}

		private static float PopScale(float t)
		{
			if (t < 0.12f)
			{
				return Mathf.Lerp(0.2f, 1.45f, t / 0.12f);
			}
			if (t < 0.3f)
			{
				return Mathf.Lerp(1.45f, 1f, (t - 0.12f) / 0.18f);
			}
			return 1f + 0.04f * Mathf.Sin((t - 0.3f) * 10f);
		}

		private static void Put(TreeNodeRender n, float x, float y, float sc)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = ((Transform)n.rectTransform).position;
			((Transform)n.rectTransform).position = new Vector3(x, y, position.z);
			((Transform)n.rectTransform).localScale = new Vector3(sc, sc, 1f);
		}

		private static IEnumerator Run(Vector2 start, string main, string sub, Color col, bool up)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			float s = Scale();
			bool num = IsDark(col);
			Color c = (num ? new Color(0.92f, 0.92f, 0.96f, 1f) : new Color(0.05f, 0.04f, 0.08f, 1f));
			Color c2 = (num ? col : Color.Lerp(col, Color.white, 0.45f));
			List<Layer> ls = new List<Layer>();
			Layer layer = Make(main, 17f, c, 0.9f, -0.9f, 0f);
			if (layer != null)
			{
				ls.Add(layer);
			}
			layer = Make(main, 17f, c, -0.9f, -0.9f, 0f);
			if (layer != null)
			{
				ls.Add(layer);
			}
			layer = Make(main, 17f, c, 0f, -1.7f, 0f);
			if (layer != null)
			{
				ls.Add(layer);
			}
			if (num)
			{
				layer = Make(main, 17f, c, 0.9f, 0.9f, 0f);
				if (layer != null)
				{
					ls.Add(layer);
				}
				layer = Make(main, 17f, c, -0.9f, 0.9f, 0f);
				if (layer != null)
				{
					ls.Add(layer);
				}
				layer = Make(main, 17f, c, 0f, 1.7f, 0f);
				if (layer != null)
				{
					ls.Add(layer);
				}
			}
			layer = Make(main, 17f, col, 0f, 0f, 0f);
			if (layer != null)
			{
				ls.Add(layer);
			}
			layer = Make(sub, 7f, c, 0.7f, -12.2f, 0.12f);
			if (layer != null)
			{
				ls.Add(layer);
			}
			layer = Make(sub, 7f, c2, 0f, -11.5f, 0.12f);
			if (layer != null)
			{
				ls.Add(layer);
			}
			List<Spark> sp = new List<Spark>();
			int num2 = (up ? 16 : 6);
			for (int i = 0; i < num2; i++)
			{
				Spark spark = MakeSpark(col, up, s);
				if (spark != null)
				{
					spark.p = start;
					sp.Add(spark);
				}
			}
			float t = 0f;
			while (t < 1.8f)
			{
				yield return null;
				float unscaledDeltaTime = Time.unscaledDeltaTime;
				t += unscaledDeltaTime;
				float num3 = PopScale(t);
				float num4 = Mathf.Clamp01(t / 1.8f);
				float num5 = (up ? 1f : (-1f)) * (1f - Mathf.Pow(1f - num4, 2.2f)) * 30f;
				float num6 = Mathf.Sin(t * 9f) * (up ? 0.6f : 1.6f) * (1f - num4);
				float num7 = ((t < 1.15f) ? 1f : Mathf.Clamp01(1f - (t - 1.15f) / 0.65f));
				foreach (Layer item in ls)
				{
					if (!((Object)(object)item.n == (Object)null) && !((Object)(object)item.n.text == (Object)null))
					{
						float a = ((item.delay > 0f) ? (Mathf.Clamp01((t - item.delay) / 0.15f) * num7) : num7);
						Color c3 = item.c;
						c3.a = a;
						((Graphic)item.n.text).color = c3;
						Put(item.n, start.x + (item.dx + num6) * s * num3, start.y + num5 * s + item.dy * s * num3, num3);
					}
				}
				foreach (Spark item2 in sp)
				{
					if (!((Object)(object)item2.n == (Object)null) && !((Object)(object)item2.n.image == (Object)null))
					{
						item2.age += unscaledDeltaTime;
						item2.v.y -= 120f * s * unscaledDeltaTime;
						item2.p += item2.v * unscaledDeltaTime;
						Color c4 = item2.c;
						c4.a = Mathf.Clamp01(1f - item2.age / item2.life);
						((Graphic)item2.n.image).color = c4;
						Put(item2.n, item2.p.x, item2.p.y, 1f);
					}
				}
			}
			foreach (Layer item3 in ls)
			{
				if ((Object)(object)item3.n != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)item3.n).gameObject);
				}
			}
			foreach (Spark item4 in sp)
			{
				if ((Object)(object)item4.n != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)item4.n).gameObject);
				}
			}
		}
	}
	[HarmonyPatch(typeof(StoreReputation), "OnItemTraded")]
	public static class TradePatch
	{
		private static void Prefix(string factionId, out double __state)
		{
			__state = double.NaN;
			try
			{
				StoreReputation storeReputation = StoreReputation.GetStoreReputation(factionId);
				if (storeReputation != null)
				{
					__state = storeReputation.GetReputationExact();
				}
			}
			catch (Exception)
			{
			}
		}

		private static void Postfix(string factionId, double __state)
		{
			try
			{
				if (!double.IsNaN(__state))
				{
					StoreReputation storeReputation = StoreReputation.GetStoreReputation(factionId);
					if (storeReputation != null)
					{
						Pop.Record(factionId, storeReputation.GetReputationExact() - __state);
					}
				}
			}
			catch (Exception ex)
			{
				if (ModEntry.Log != null)
				{
					ModEntry.Log.Error((object)("trade patch failed: " + ex));
				}
			}
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '9.1.0.7988')
