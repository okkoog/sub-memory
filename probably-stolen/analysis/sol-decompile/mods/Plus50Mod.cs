using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using HarmonyLib;
using Microsoft.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: TargetFramework(".NETStandard,Version=v2.1", FrameworkDisplayName = ".NET Standard 2.1")]
[assembly: AssemblyCompany("Plus50Mod")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("Plus50Mod")]
[assembly: AssemblyTitle("Plus50Mod")]
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
namespace Plus50Mod
{
	public class Plus50Mod : IMod
	{
		internal static ModLog Log;

		public void Init(ModManifest manifest)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			Log = new ModLog(manifest);
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}
	}
	[HarmonyPatch(typeof(InspectionUIManager), "Start")]
	internal static class StartPatch
	{
		private const float MinFieldWidth = 60f;

		private static void Postfix(InspectionUIManager __instance)
		{
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Expected O, but got Unknown
			//IL_00e4: Expected O, but got Unknown
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Expected O, but got Unknown
			//IL_0153: Expected O, but got Unknown
			//IL_0153: Expected O, but got Unknown
			//IL_0153: Expected O, but got Unknown
			//IL_0153: Expected O, but got Unknown
			try
			{
				GameObject appraisalPanel = __instance.appraisalPanel;
				if ((Object)(object)appraisalPanel == (Object)null)
				{
					return;
				}
				Button[] componentsInChildren = appraisalPanel.GetComponentsInChildren<Button>(true);
				Button val = ((IEnumerable<Button>)componentsInChildren).FirstOrDefault((Func<Button, bool>)((Button b) => Label(b) == "+10"));
				Button val2 = ((IEnumerable<Button>)componentsInChildren).FirstOrDefault((Func<Button, bool>)((Button b) => Label(b) == "-10"));
				if ((Object)(object)val == (Object)null || (Object)(object)val2 == (Object)null)
				{
					Plus50Mod.Log.Warning((object)("Could not find the +10/-10 buttons. Buttons in panel: " + string.Join(", ", componentsInChildren.Select((Button b) => "'" + ((Object)b).name + "'='" + Label(b) + "'"))));
				}
				else
				{
					RectTransform val3 = (RectTransform)((Component)val).transform.parent;
					List<RectTransform> oldItems = RowItems(val3, (RectTransform)((Component)val).transform);
					Button val4 = Clone(__instance, val2, "-50", -50, ((Component)val2).transform.GetSiblingIndex());
					string text = Relayout(plus50: (RectTransform)((Component)Clone(__instance, val, "+50", 50, ((Component)val).transform.GetSiblingIndex() + 1)).transform, row: val3, oldItems: oldItems, minus10: (RectTransform)((Component)val2).transform, minus50: (RectTransform)((Component)val4).transform, plus10: (RectTransform)((Component)val).transform);
					Plus50Mod.Log.Log((object)("Added +50/-50 buttons to the appraisal panel (" + text + " layout)."));
				}
			}
			catch (Exception ex)
			{
				ModLog log = Plus50Mod.Log;
				if (log != null)
				{
					log.Error((object)("Failed to add buttons: " + ex));
				}
			}
		}

		private static string Label(Button b)
		{
			TMP_Text componentInChildren = ((Component)b).GetComponentInChildren<TMP_Text>(true);
			if ((Object)(object)componentInChildren != (Object)null)
			{
				return componentInChildren.text?.Trim();
			}
			Text componentInChildren2 = ((Component)b).GetComponentInChildren<Text>(true);
			if (!((Object)(object)componentInChildren2 != (Object)null))
			{
				return null;
			}
			return componentInChildren2.text?.Trim();
		}

		private static Button Clone(InspectionUIManager mgr, Button source, string label, int delta, int siblingIndex)
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected O, but got Unknown
			GameObject obj = Object.Instantiate<GameObject>(((Component)source).gameObject, ((Component)source).transform.parent, false);
			((Object)obj).name = "Plus50Mod_" + label;
			obj.transform.SetSiblingIndex(siblingIndex);
			TMP_Text componentInChildren = obj.GetComponentInChildren<TMP_Text>(true);
			if ((Object)(object)componentInChildren != (Object)null)
			{
				componentInChildren.text = label;
			}
			Text componentInChildren2 = obj.GetComponentInChildren<Text>(true);
			if ((Object)(object)componentInChildren2 != (Object)null)
			{
				componentInChildren2.text = label;
			}
			Button component = obj.GetComponent<Button>();
			component.onClick = new ButtonClickedEvent();
			((UnityEvent)component.onClick).AddListener((UnityAction)delegate
			{
				mgr.ChangeAppraisalServicePrice(delta);
			});
			return component;
		}

		private static List<RectTransform> RowItems(RectTransform row, RectTransform reference)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			float y = ((Transform)reference).localPosition.y;
			Rect rect = reference.rect;
			float num = Mathf.Max(5f, ((Rect)(ref rect)).height / 2f);
			List<RectTransform> list = new List<RectTransform>();
			foreach (Transform item in (Transform)row)
			{
				Transform val = item;
				RectTransform val2 = (RectTransform)(object)((val is RectTransform) ? val : null);
				if (val2 != null && ((Component)val).gameObject.activeSelf && Mathf.Abs(((Transform)val2).localPosition.y - y) <= num)
				{
					list.Add(val2);
				}
			}
			return list;
		}

		private static string Relayout(RectTransform row, List<RectTransform> oldItems, RectTransform minus10, RectTransform minus50, RectTransform plus10, RectTransform plus50)
		{
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			RectTransform field = ((IEnumerable<RectTransform>)oldItems).FirstOrDefault((Func<RectTransform, bool>)((RectTransform i) => (Object)(object)((Component)i).GetComponentInChildren<TMP_InputField>(true) != (Object)null || (Object)(object)((Component)i).GetComponentInChildren<InputField>(true) != (Object)null));
			HorizontalLayoutGroup component = ((Component)row).GetComponent<HorizontalLayoutGroup>();
			Rect rect;
			if ((Object)(object)component != (Object)null)
			{
				LayoutRebuilder.ForceRebuildLayoutImmediate(row);
				List<RectTransform> list = oldItems.Concat((IEnumerable<RectTransform>)(object)new RectTransform[2] { minus50, plus50 }).ToList();
				float num = list.Sum(delegate(RectTransform i)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					Rect rect2 = i.rect;
					return ((Rect)(ref rect2)).width;
				}) + ((HorizontalOrVerticalLayoutGroup)component).spacing * (float)(list.Count - 1) + (float)((LayoutGroup)component).padding.left + (float)((LayoutGroup)component).padding.right;
				rect = row.rect;
				float num2 = num - ((Rect)(ref rect)).width;
				if (num2 > 0f && (Object)(object)field != (Object)null)
				{
					rect = field.rect;
					float num3 = Mathf.Max(60f, ((Rect)(ref rect)).width - num2);
					LayoutElement val = ((Component)field).GetComponent<LayoutElement>();
					if ((Object)(object)val == (Object)null)
					{
						val = ((Component)field).gameObject.AddComponent<LayoutElement>();
					}
					val.preferredWidth = num3;
					val.minWidth = Mathf.Min(val.minWidth, num3);
					if (!((HorizontalOrVerticalLayoutGroup)component).childControlWidth)
					{
						field.SetSizeWithCurrentAnchors((Axis)0, num3);
					}
					LayoutRebuilder.ForceRebuildLayoutImmediate(row);
				}
				return "layout-group";
			}
			List<RectTransform> list2 = oldItems.OrderBy(Left).ToList();
			if (list2.Count < 2)
			{
				return "none";
			}
			float num4 = Left(list2[0]);
			float num5 = Right(list2[list2.Count - 1]);
			float num6 = Mathf.Max(0f, Left(list2[1]) - Right(list2[0]));
			list2.Insert(list2.IndexOf(minus10), minus50);
			list2.Insert(list2.IndexOf(plus10) + 1, plus50);
			float num7 = num5 - num4;
			float num8 = num6 * (float)(list2.Count - 1);
			float num9 = list2.Where((RectTransform i) => (Object)(object)i != (Object)(object)field).Sum(delegate(RectTransform i)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				Rect rect2 = i.rect;
				return ((Rect)(ref rect2)).width;
			});
			float num10 = (((Object)(object)field != (Object)null) ? (num7 - num8 - num9) : 0f);
			float num11 = 1f;
			if ((Object)(object)field != (Object)null && num10 < 60f)
			{
				num10 = 60f;
				num11 = Mathf.Max(0.5f, (num7 - num8 - num10) / num9);
			}
			else if ((Object)(object)field == (Object)null && num9 + num8 > num7)
			{
				num11 = Mathf.Max(0.5f, (num7 - num8) / num9);
			}
			float num12 = num4;
			foreach (RectTransform item in list2)
			{
				float num13;
				if (!((Object)(object)item == (Object)(object)field))
				{
					rect = item.rect;
					num13 = ((Rect)(ref rect)).width * num11;
				}
				else
				{
					num13 = num10;
				}
				float num14 = num13;
				item.SetSizeWithCurrentAnchors((Axis)0, num14);
				Vector3 localPosition = ((Transform)item).localPosition;
				localPosition.x = num12 + num14 * item.pivot.x;
				((Transform)item).localPosition = localPosition;
				num12 += num14 + num6;
			}
			return "manual";
		}

		private static float Left(RectTransform rt)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			float x = ((Transform)rt).localPosition.x;
			Rect rect = rt.rect;
			return x - ((Rect)(ref rect)).width * rt.pivot.x;
		}

		private static float Right(RectTransform rt)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			float num = Left(rt);
			Rect rect = rt.rect;
			return num + ((Rect)(ref rect)).width;
		}
	}
}
You are not using the latest version of the tool, please update.
Latest version is '11.2.0.10050' (yours is '9.1.0.7988')
