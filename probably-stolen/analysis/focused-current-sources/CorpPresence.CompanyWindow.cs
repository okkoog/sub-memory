using System;
using System.Collections.Generic;
using System.Linq;
using CorpPresence.Malice;
using UnityEngine;

namespace CorpPresence.UI;

internal static class CompanyWindow
{
	private static readonly Dictionary<string, (float w, float h, bool fits)> BrandFit = new Dictionary<string, (float, float, bool)>();

	private static readonly GUIContent FitContent = new GUIContent();

	private static Theme _brandFitTheme;

	private static string _brandSel;

	private static float _swapScan = -1f;

	private static readonly List<(string Name, string Reason, string Target)> SwapRows = new List<(string, string, string)>();

	private static int _swapCount;

	private static Company _company;

	private static Theme _theme;

	private static Rect _rect;

	private static int _module;

	private static string _desk;

	private static string _toast;

	private static float _toastUntil;

	private static bool _openedThisFrame;

	private static readonly Dictionary<CatalogEntry, int> Cart = new Dictionary<CatalogEntry, int>();

	private static string _page = "日用";

	private static Vector2 _size;

	private const float DragBar = 80f;

	private static bool _dragging;

	private static bool _moved;

	private static Vector2 _grab;

	private static Vector2 _center;

	private static CatalogEntry _leviathan;

	private static int _partsPage;

	private const int PartsPerPage = 8;

	private static float _refurbScan = -1f;

	private static readonly List<(string Name, string From, string To, int Fee)> RefurbRows = new List<(string, string, string, int)>();

	private static int _refurbCount;

	private static readonly int[] SolanaModuleTier = new int[4] { 1, 2, 3, 0 };

	private static readonly Dictionary<string, string> NameCache = new Dictionary<string, string>();

	private static readonly string[] MerchantPages = new string[3] { "码头", null, "目录" };

	private static readonly string[] AlliancePages = new string[6] { "军械", "药房", "码头", null, "阵线", "目录" };

	private static int _orderPage;

	private static string _orderPageKey;

	private static string[] CarmModules => WithExtra(CarmBase);

	private static string[] CarmBase => MenuLabels.Get(0, L.En);

	public static bool Visible { get; private set; }

	private static float Scale
	{
		get
		{
			float num = Mathf.Clamp((float)Screen.height / 1080f, 0.6f, 2f) * Settings.PanelScale;
			if (_size.x > 0f)
			{
				num = Mathf.Min(new float[3]
				{
					num,
					(float)Screen.width * 0.96f / _size.x,
					(float)Screen.height * 0.96f / _size.y
				});
			}
			return num;
		}
	}

	private static int Day => StoreStation.GetDayCounter();

	private static CompanyState State => Companies.StateOf(_company);

	public static bool Dragging
	{
		get
		{
			if (Visible)
			{
				return _dragging;
			}
			return false;
		}
	}

	private static int Cash
	{
		get
		{
			PlayerStore instance = PlayerStore.Instance;
			if (instance == null)
			{
				return 0;
			}
			return instance.GetCash();
		}
	}

	private static string[] DavreModules => WithExtra(DavreBase);

	private static string[] DavreBase => MenuLabels.Get(1, L.En);

	private static string[] MeridianModules => WithExtra(MeridianBase);

	private static string[] MeridianBase => MenuLabels.Get(2, L.En);

	private static string[] PastelModules => WithExtra(PastelBase);

	private static string[] PastelBase => MenuLabels.Get(3, L.En);

	private static string[] MiloLines => MenuLabels.Get(4, L.En);

	private static string[] RebelModules
	{
		get
		{
			if (!Rebels.Arrested)
			{
				if (!Rebels.Cut)
				{
					if (!Rebels.FenceClosed)
					{
						return MenuLabels.Get(5, L.En);
					}
					return MenuLabels.Get(6, L.En);
				}
				return MenuLabels.Get(7, L.En);
			}
			return MenuLabels.Get(8, L.En);
		}
	}

	private static string[] SolanaModules => WithExtra(SolanaBase);

	private static string[] SolanaBase => MenuLabels.Get(9, L.En);

	private static string[] SpacerServices => WithExtra(SpacerBase);

	private static string[] SpacerBase
	{
		get
		{
			if (!Settings.MaliceMode)
			{
				return SpacerBaseFull;
			}
			return MenuLabels.Get(10, L.En);
		}
	}

	private static string[] SpacerPages
	{
		get
		{
			if (!Settings.MaliceMode)
			{
				return AlliancePages;
			}
			return MerchantPages;
		}
	}

	private static string[] SpacerBaseFull => MenuLabels.Get(11, L.En);

	private static int SpacerModuleCount => SpacerServices.Length;

	private static void DrawCarmichael()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		float width = ((Rect)(ref _rect)).width;
		float height = ((Rect)(ref _rect)).height;
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector(20f, 8f, 560f, 90f);
		if ((Object)(object)theme.Banner != (Object)null)
		{
			Ui.Image(r, theme.Banner);
			GUI.Label(new Rect(((Rect)(ref r)).x + 120f, ((Rect)(ref r)).y + 16f, 420f, 58f), _company.Name + L.T(" · 药品订货单", " · Pharmaceutical Order Form"), theme.Title);
		}
		else
		{
			Ui.Fill(new Rect(36f, 34f, 36f, 12f), theme.Accent);
			Ui.Fill(new Rect(48f, 22f, 12f, 36f), theme.Accent);
			GUI.Label(new Rect(88f, 18f, 520f, 44f), _company.Name + L.T(" · 药品订货单", " · Pharmaceutical Order Form"), theme.Title);
		}
		GUI.Label(new Rect(width - 260f, 30f, 220f, 22f), L.En ? $"Form CP-{_company.PhoneNumber}   Day {Day}" : $"表单 CP-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
		Ui.Fill(new Rect(20f, 96f, width - 40f, 2f), theme.Accent);
		Ui.Fill(new Rect(20f, 100f, width - 40f, 1f), theme.Line);
		GUI.Label(new Rect(24f, 104f, width - 48f, 22f), L.T("客服：", "Service: ") + _desk, theme.Body);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(20f, 134f, 220f, height - 154f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		Rect r2 = default(Rect);
		((Rect)(ref r2))._002Ector(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 10f, 200f, 146f);
		Ui.Fill(r2, Tex.Hex(13688022));
		Texture2D val2 = Portraits.Texture("carmicheal_isa", "femaleUpper_1");
		if ((Object)(object)val2 != (Object)null)
		{
			Ui.Image(new Rect(((Rect)(ref r2)).x + 2f, ((Rect)(ref r2)).y + 2f, ((Rect)(ref r2)).width - 4f, ((Rect)(ref r2)).height - 4f), val2);
		}
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 4f, 200f, 20f), L.T("经办：伊莎·兰登（市场规范部）", "Handled by: Isa Langdon (Market Compliance)"), theme.Small);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 28f, 200f, 22f), L.En ? ("Your shop: " + TierLabel(state)) : ("贵店：" + TierLabel(state)), theme.Body);
		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 54f, 200f, 8f));
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 64f, 200f, 20f), NextTierHint(), theme.Small);
		float num = ((Rect)(ref r2)).yMax + 96f;
		for (int i = 0; i < CarmModules.Length; i++)
		{
			string text = ((i == _module) ? "› " : "  ") + CarmModules[i];
			if (Ui.Button(new Rect(((Rect)(ref val)).x + 6f, num + (float)(i * 32), ((Rect)(ref val)).width - 12f, 28f), text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
		}
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref val)).xMax + 20f, 134f, width - ((Rect)(ref val)).xMax - 40f, height - 154f);
		GUI.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 30f), CarmModules[_module], theme.Title);
		Ui.Fill(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 34f, ((Rect)(ref val3)).width, 1f), theme.Line);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 44f, ((Rect)(ref val3)).width, ((Rect)(ref val3)).height - 44f);
		if (DrawExtraIfSelected(CarmBase.Length, val4))
		{
			return;
		}
		if (_company.Tier(state) < 1 && _module < 3)
		{
			GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 60f), L.T("贵店尚未登记。和伊莎多打几次交道吧。", "Your shop is not registered yet. Deal with Isa a few more times."), theme.Wrap);
			return;
		}
		switch (_module)
		{
		case 0:
			DrawCarmOrder(val4);
			break;
		case 1:
			DrawCarmStamina(val4);
			break;
		case 2:
			DrawCarmSwap(val4);
			break;
		case 3:
			DrawCarmSpecimen(val4);
			break;
		default:
			DrawBrand(val4);
			break;
		}
	}

	private static int CarmUnitPrice(CatalogEntry e)
	{
		double rate = Company.OrderRate(_company, _company.Tier(State));
		return e.Quote(rate);
	}

	private static string CarmLocked(CatalogEntry e)
	{
		if (e.MinTier > _company.CatalogTier(State))
		{
			if (!L.En)
			{
				return _company.TierNames[e.MinTier - 1] + "开放";
			}
			return "Opens at " + _company.TierNames[e.MinTier - 1];
		}
		if (e.Id == "@painblock" && !CarmichaelCo.PainBlockOpen())
		{
			return L.T("伊莎第 4 章后", "After Isa chapter 4");
		}
		return null;
	}

	private static void DrawCarmOrder(Rect r)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		bool flag = Services.CanOrderToday(_company);
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), flag ? L.T("今天下单，明天送到。每天一单。", "Order today, delivered tomorrow. One order per day. ") : L.T("今天已经下过单了。", "Already ordered today. "), theme.Small);
		float x = ((Rect)(ref r)).x;
		float num = ((Rect)(ref r)).x + 290f;
		float num2 = ((Rect)(ref r)).x + 380f;
		float num3 = ((Rect)(ref r)).x + 500f;
		float num4 = ((Rect)(ref r)).y + 28f;
		Ui.Fill(new Rect(((Rect)(ref r)).x, num4, ((Rect)(ref r)).width, 26f), Tex.Hex(13886431));
		GUI.Label(new Rect(x + 8f, num4, 280f, 26f), L.T("品名", "Item"), theme.Body);
		GUI.Label(new Rect(num, num4, 90f, 26f), L.T("单价", "Unit price"), theme.Center);
		GUI.Label(new Rect(num2, num4, 120f, 26f), L.T("数量", "Qty"), theme.Center);
		GUI.Label(new Rect(num3, num4, ((Rect)(ref r)).xMax - num3, 26f), L.T("小计", "Subtotal"), theme.Center);
		num4 += 30f;
		CatalogEntry[] entries = CarmichaelCo.Entries;
		Rect val = default(Rect);
		foreach (CatalogEntry catalogEntry in entries)
		{
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x, num4, ((Rect)(ref r)).width, 34f);
			Ui.Fill(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).yMax - 1f, ((Rect)(ref val)).width, 1f), theme.Line);
			string text = catalogEntry.Name();
			int val2 = Math.Min(10, Services.Left(state, catalogEntry));
			text += Services.LeftText(state, catalogEntry);
			GUI.Label(new Rect(x + 8f, num4, 280f, 34f), text, theme.Body);
			string text2 = CarmLocked(catalogEntry);
			if (text2 != null)
			{
				GUI.Label(new Rect(num, num4, ((Rect)(ref r)).xMax - num, 34f), text2, theme.Small);
				Cart.Remove(catalogEntry);
				num4 += 36f;
				continue;
			}
			int num5 = CarmUnitPrice(catalogEntry);
			if (num5 <= 0)
			{
				GUI.Label(new Rect(num, num4, ((Rect)(ref r)).xMax - num, 34f), L.T("暂不可用", "Unavailable"), theme.Small);
				Cart.Remove(catalogEntry);
				num4 += 36f;
				continue;
			}
			GUI.Label(new Rect(num, num4, 90f, 34f), $"{num5}₵", theme.Price);
			Cart.TryGetValue(catalogEntry, out var value);
			value = Ui.Qty(new Rect(num2, num4, 120f, 34f), value, Math.Min(10, val2), theme.BtnStyle, theme.Center);
			if (value > 0)
			{
				Cart[catalogEntry] = value;
			}
			else
			{
				Cart.Remove(catalogEntry);
			}
			GUI.Label(new Rect(num3, num4, ((Rect)(ref r)).xMax - num3, 34f), (value > 0) ? $"{num5 * value}₵" : "—", theme.Center);
			num4 += 36f;
		}
		int num6 = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => CarmUnitPrice(kv.Key) * kv.Value);
		float num7 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num7, 200f, 22f), L.En ? $"Total {num6}₵" : $"合计 {num6}₵", theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).x, num7 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = num6 > 0 && num6 <= Cash && flag;
		bool enabled2 = GUI.enabled;
		GUI.enabled = enabled;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num7 + 2f, 160f, 36f), L.T("提交订货单", "Submit order"), theme.BtnHotStyle))
		{
			string text3 = Services.PlaceOrder(_company, Cart, CarmUnitPrice);
			if (text3 == null)
			{
				Cart.Clear();
				Toast(_company.FrontDeskDone);
			}
			else
			{
				Toast(text3);
			}
		}
		GUI.enabled = enabled2;
	}

	private static void DrawCarmStamina(Rect r)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		bool flag = CarmichaelCo.StaminaOpen();
		int num = Services.StaminaNextDay(state);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 250f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		float num2 = ((Rect)(ref val)).x + 16f;
		float num3 = ((Rect)(ref val)).width - 32f;
		float num4 = ((Rect)(ref val)).y + 12f;
		GUI.Label(new Rect(num2, num4, num3, 26f), L.T("申请单 · 体力增强剂（未上市）", "Request Form · Stamina Booster (unreleased)"), theme.Body);
		num4 += 32f;
		Ui.Fill(new Rect(num2, num4, num3, 1f), theme.Line);
		num4 += 8f;
		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("申请药房：贵店", "Requesting pharmacy: your shop"), theme.Body);
		num4 += 26f;
		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("经办人：伊莎·兰登（市场规范部）", "Handler: Isa Langdon (Market Compliance)"), theme.Body);
		num4 += 26f;
		GUI.Label(new Rect(num2, num4, num3, 22f), L.T("药品费用：0₵", "Drug cost: 0₵"), theme.Body);
		num4 += 26f;
		GUI.Label(new Rect(num2, num4, 90f, 24f), L.T("手续费：", "Fee:"), theme.Body);
		GUI.Label(new Rect(num2 + 70f, num4, 120f, 24f), $"{CarmichaelCo.StaminaFee}₵", theme.Price);
		num4 += 30f;
		GUI.Label(new Rect(num2, num4, num3, 22f), L.En ? $"Note: special request, 1 per {CarmichaelCo.StaminaGap} days. Hand-delivered the next day." : $"备注：特别申请，每 {CarmichaelCo.StaminaGap} 天限 1 支。次日专人配送。", theme.Small);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).xMax - 150f, ((Rect)(ref val)).y + 54f, 120f, 48f);
		Color hot = theme.Hot;
		Ui.Fill(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, 2f), hot);
		Ui.Fill(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).yMax - 2f, ((Rect)(ref val2)).width, 2f), hot);
		Ui.Fill(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, 2f, ((Rect)(ref val2)).height), hot);
		Ui.Fill(new Rect(((Rect)(ref val2)).xMax - 2f, ((Rect)(ref val2)).y, 2f, ((Rect)(ref val2)).height), hot);
		GUI.Label(val2, L.T("特别申请", "SPECIAL"), theme.Price);
		float num5 = ((Rect)(ref val)).yMax + 16f;
		if (!flag)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 44f), L.T("新品尚未向贵店配发。伊莎·兰登为贵店争取到配额后开放。", "This new product hasn't been allocated to your shop yet. It opens once Isa Langdon secures you a quota."), theme.Wrap);
			return;
		}
		bool flag2 = Day >= num;
		GUI.Label(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 22f), flag2 ? L.T("本期配额：可申领。", "Current quota: available.") : (L.En ? $"Next available: day {num} ({num - Day} days left)" : $"下次可申领：第 {num} 天（还有 {num - Day} 天）"), theme.Body);
		bool enabled = GUI.enabled;
		GUI.enabled = flag2 && Cash >= CarmichaelCo.StaminaFee;
		if (Ui.Button(new Rect(((Rect)(ref r)).x, num5 + 32f, 260f, 36f), (!flag2) ? L.T("批次未到", "Batch not ready") : (L.En ? $"Submit request (fee {CarmichaelCo.StaminaFee}₵)" : $"提交申请（手续费 {CarmichaelCo.StaminaFee}₵）"), theme.BtnHotStyle))
		{
			Toast(Services.RequestStamina(_company) ?? L.T("申请已受理。明早由专人配送。", "Request accepted. Hand-delivered tomorrow morning."));
		}
		GUI.enabled = enabled;
	}

	private static void ScanFakes()
	{
		if (_swapScan >= 0f && Time.unscaledTime - _swapScan < 1f)
		{
			return;
		}
		_swapScan = Time.unscaledTime;
		SwapRows.Clear();
		List<GameItem> list = Services.FakesOwned();
		_swapCount = list.Count;
		foreach (GameItem item2 in list.Take(6))
		{
			string item;
			try
			{
				item = item2.GetDisplayName(false);
			}
			catch
			{
				item = item2.identifier;
			}
			SwapRows.Add((item, CarmichaelCo.FakeReason(item2), ItemName(CarmichaelCo.SwapTarget(item2))));
		}
	}

	private static void DrawCarmSpecimen(Rect r)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		if (!CarmichaelCo.Licensed)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), "？？？", theme.Wrap);
			return;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 44f), L.T("养殖许可在册。银血三叶蜥养在循环水养殖系统里，成年后能采血；离开水也不会死，只是不积血。原价，明天送到，不限量。", "Breeding license on file. Keep Silverblood Trilobites in an aquaculture system; adults can be bled. Out of water they survive but build no blood. List price, delivered tomorrow, no limit."), theme.Wrap);
		float num = ((Rect)(ref r)).y + 54f;
		List<(string, int)> list = new List<(string, int)>
		{
			("@silverblood", 500),
			("@blood_drawer", 250)
		};
		if (CarmichaelCo.ReactorOpen)
		{
			list.Add(("@isomer_reactor", 400));
			list.Add(("@sb_handbook", 50));
		}
		Rect val = default(Rect);
		foreach (var item3 in list)
		{
			string item = item3.Item1;
			int item2 = item3.Item2;
			Brand.Entry entry = Brand.Info(item);
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x, num, 80f, 80f);
			GUI.Box(val, GUIContent.none, theme.CellBox);
			if ((Object)(object)entry.Icon != (Object)null)
			{
				Ui.Image(new Rect(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y + 4f, 72f, 72f), entry.Icon);
			}
			GUI.Label(new Rect(((Rect)(ref val)).xMax + 16f, num + 10f, 260f, 26f), entry.Name, theme.Body);
			GUI.Label(new Rect(((Rect)(ref val)).xMax + 16f, num + 40f, 200f, 22f), $"{item2}₵", theme.Price);
			bool enabled = GUI.enabled;
			GUI.enabled = Cash >= item2;
			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num + 22f, 160f, 36f), L.T("订一份", "Order one"), theme.BtnHotStyle))
			{
				Toast(CarmichaelCo.OrderSpecimen(_company, item, item2) ?? _company.FrontDeskDone);
			}
			GUI.enabled = enabled;
			num += 92f;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).yMax - 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
	}

	private static void DrawCarmSwap(Rect r)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		ScanFakes();
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 64f), L.En ? $"Drugs sold under the Carmicheal name without authorization are recalled by the company and replaced one for one with the genuine article: counterfeit Immunivax, recalled Immunivax batches, and unlicensed Phagimycin. Expired genuine products are not included.\nOne request every {CarmichaelCo.SwapGap} days, up to {6} items each time, {CarmichaelCo.SwapFee}₵ per item." : $"市面上冒用卡迈克尔名义的药品，由公司回收，按 1 换 1 替换为正品：假冒的免疫宁、召回批次的免疫宁、无证噬菌霉素。过期的正品不在此列。\n每 {CarmichaelCo.SwapGap} 天可申请一次，每次最多 {6} 件，每件手续费 {CarmichaelCo.SwapFee}₵。", theme.Wrap);
		float num = ((Rect)(ref r)).y + 74f;
		Ui.Fill(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 26f), Tex.Hex(13886431));
		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 240f, 26f), L.T("贵店持有的问题药品", "Problem drugs in your shop"), theme.Body);
		GUI.Label(new Rect(((Rect)(ref r)).x + 250f, num, 110f, 26f), L.T("问题", "Issue"), theme.Center);
		GUI.Label(new Rect(((Rect)(ref r)).x + 360f, num, ((Rect)(ref r)).width - 360f, 26f), L.T("替换为", "Replace with"), theme.Center);
		num += 30f;
		if (SwapRows.Count == 0)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, ((Rect)(ref r)).width, 30f), L.T("没有找到。", "None found."), theme.Small);
			num += 34f;
		}
		foreach (var swapRow in SwapRows)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 240f, 30f), swapRow.Name, theme.Body);
			GUI.Label(new Rect(((Rect)(ref r)).x + 250f, num, 110f, 30f), swapRow.Reason, theme.Price);
			GUI.Label(new Rect(((Rect)(ref r)).x + 360f, num, ((Rect)(ref r)).width - 360f, 30f), L.T("正品", "Genuine ") + swapRow.Target, theme.Center);
			Ui.Fill(new Rect(((Rect)(ref r)).x, num + 30f, ((Rect)(ref r)).width, 1f), theme.Line);
			num += 32f;
		}
		if (_swapCount > SwapRows.Count)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, ((Rect)(ref r)).width, 22f), L.En ? $"{_swapCount - SwapRows.Count} more; swap them next time." : $"另有 {_swapCount - SwapRows.Count} 件，下次再换。", theme.Small);
		}
		int num2 = Services.SwapNextDay(state);
		bool flag = Day >= num2;
		int num3 = SwapRows.Count * CarmichaelCo.SwapFee;
		float num4 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num4, 300f, 22f), (!flag) ? (L.En ? $"Next request: day {num2}" : $"下次可申请：第 {num2} 天") : (L.En ? $"{SwapRows.Count} items this time, fee {num3}₵" : $"本次 {SwapRows.Count} 件，手续费 {num3}₵"), flag ? theme.Price : theme.Body);
		GUI.Label(new Rect(((Rect)(ref r)).x, num4 + 22f, 300f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = GUI.enabled;
		GUI.enabled = flag && SwapRows.Count > 0 && Cash >= num3;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num4 + 2f, 160f, 36f), L.T("申请换货", "Request swap"), theme.BtnHotStyle))
		{
			string text = Services.RequestSwap(_company);
			_swapScan = -1f;
			if (text == null)
			{
				text = L.T("问题药品已收回。明早送来正品。", "Problem drugs collected. Genuine replacements arrive tomorrow morning.");
			}
			Toast(text);
		}
		GUI.enabled = enabled;
	}

	private static void DrawBrand(Rect r)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		string[] array = Brand.Of(_company);
		int num = 0;
		string[] array2 = array;
		foreach (string item in array2)
		{
			if (state.Brand.Contains(item))
			{
				num++;
			}
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 22f), L.En ? $"Recorded {num}/{array.Length}   Products you buy, sell or receive from this company get recorded here" : $"已收录 {num}/{array.Length}\u3000买进、卖出或拿到过的本公司产品会收录进来", theme.Small);
		int num2 = Mathf.Max(3, (int)((((Rect)(ref r)).width + 6f) / 124f));
		float num3 = (((Rect)(ref r)).width - 6f * (float)(num2 - 1)) / (float)num2;
		int num4 = (array.Length + num2 - 1) / num2;
		float num5 = ((Rect)(ref r)).y + 28f;
		float num6 = ((Rect)(ref r)).height - 28f - 120f - 10f;
		float num7 = Mathf.Min(56f, (num6 - 6f * (float)(num4 - 1)) / (float)Mathf.Max(1, num4));
		Rect val = default(Rect);
		Rect val2 = default(Rect);
		for (int j = 0; j < array.Length; j++)
		{
			string text = array[j];
			bool flag = state.Brand.Contains(text);
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x + (float)(j % num2) * (num3 + 6f), num5 + (float)(j / num2) * (num7 + 6f), num3, num7);
			Rect r2 = val;
			GUIStyle style = ((!flag || !(text == _brandSel)) ? (flag ? theme.CellOnBox : theme.CellBox) : theme.NavOnStyle);
			if (Ui.Button(r2, "", style) && flag)
			{
				_brandSel = text;
			}
			if (!flag)
			{
				GUI.Label(val, "？", theme.Center);
				continue;
			}
			Brand.Entry entry = Brand.Info(text);
			float num8 = num7 - 8f;
			DrawIcon(new Rect(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y + 4f, num8 * 1.4f, num8), entry.Icon);
			((Rect)(ref val2))._002Ector(((Rect)(ref val)).x + 8f + num8 * 1.4f, ((Rect)(ref val)).y + 2f, ((Rect)(ref val)).width - 12f - num8 * 1.4f, num7 - 4f);
			GUI.Label(val2, entry.Name, NameFits(theme, entry.Name, ((Rect)(ref val2)).width, ((Rect)(ref val2)).height) ? theme.Wrap : theme.Tight);
		}
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).yMax - 120f, ((Rect)(ref r)).width, 120f);
		GUI.Box(val3, GUIContent.none, theme.CellBox);
		if (_brandSel == null || !state.Brand.Contains(_brandSel) || Array.IndexOf(array, _brandSel) < 0)
		{
			GUI.Label(new Rect(((Rect)(ref val3)).x + 10f, ((Rect)(ref val3)).y + 10f, ((Rect)(ref val3)).width - 20f, 24f), (num == 0) ? L.T("还没有经手过这家公司的产品。", "You haven't handled any of this company's products yet.") : L.T("点一件已收录的物品，看它的介绍。", "Click a recorded item to read about it."), theme.Small);
			return;
		}
		Brand.Entry entry2 = Brand.Info(_brandSel);
		DrawIcon(new Rect(((Rect)(ref val3)).x + 10f, ((Rect)(ref val3)).y + 10f, 128f, 96f), entry2.Icon);
		GUI.Label(new Rect(((Rect)(ref val3)).x + 150f, ((Rect)(ref val3)).y + 8f, ((Rect)(ref val3)).width - 160f, 26f), entry2.Name, theme.Title);
		GUI.Label(new Rect(((Rect)(ref val3)).x + 150f, ((Rect)(ref val3)).y + 38f, ((Rect)(ref val3)).width - 160f, ((Rect)(ref val3)).height - 44f), entry2.Desc, theme.Wrap);
	}

	private static bool NameFits(Theme theme, string name, float width, float height)
	{
		if (_brandFitTheme != theme)
		{
			BrandFit.Clear();
			_brandFitTheme = theme;
		}
		string text = name ?? "";
		if (BrandFit.TryGetValue(text, out (float, float, bool) value) && value.Item1 == width && value.Item2 == height)
		{
			return value.Item3;
		}
		FitContent.text = text;
		bool flag = theme.Wrap.CalcHeight(FitContent, width) <= height;
		BrandFit[text] = (width, height, flag);
		return flag;
	}

	private static void DrawIcon(Rect box, Texture2D tex)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)tex == (Object)null))
		{
			float num = Mathf.Min(((Rect)(ref box)).width / (float)((Texture)tex).width, ((Rect)(ref box)).height / (float)((Texture)tex).height);
			if (num >= 1f)
			{
				num = Mathf.Floor(num);
			}
			float num2 = (float)((Texture)tex).width * num;
			float num3 = (float)((Texture)tex).height * num;
			Ui.Image(new Rect(((Rect)(ref box)).x + (((Rect)(ref box)).width - num2) / 2f, ((Rect)(ref box)).y + (((Rect)(ref box)).height - num3) / 2f, num2, num3), tex);
		}
	}

	private static string ItemName(string id)
	{
		if (NameCache.TryGetValue(id, out var value))
		{
			return value;
		}
		GameItem val = null;
		try
		{
			val = DirectoryMaster.Item(id, false);
			value = ((val != null) ? val.GetDisplayName(false) : null) ?? id;
		}
		catch
		{
			value = id;
		}
		try
		{
			if (val != null)
			{
				val.Destroy();
			}
		}
		catch
		{
		}
		return NameCache[id] = value;
	}

	public static void Open(Company c)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (c != null)
		{
			Ui.ClearHoverCache();
			_company = c;
			Theme theme = ((c is SolanaCo || c is EnvoyCo) ? Theme.Solana() : ((c is CarmichaelCo) ? Theme.Carmichael() : ((c is RebelCo) ? Theme.Rebel() : ((c is SpacerCo) ? Theme.Spacer() : ((!(c is MeridianCo)) ? ((c is DavreCo) ? Theme.Davre() : Theme.Pastel()) : Theme.Meridian())))));
			_theme = theme;
			_module = 0;
			_page = "日用";
			_brandSel = null;
			Brand.ScanOwned();
			Cart.Clear();
			_desk = c.FrontDesk[new Random().Next(c.FrontDesk.Length)];
			_toast = null;
			Vector2 val = default(Vector2);
			if (c is SolanaCo || c is EnvoyCo)
			{
				((Vector2)(ref val))._002Ector(880f, 600f);
			}
			else if (c is CarmichaelCo)
			{
				((Vector2)(ref val))._002Ector(860f, 640f);
			}
			else if (c is SpacerCo || c is RebelCo)
			{
				((Vector2)(ref val))._002Ector(820f, 640f);
			}
			else
			{
				val = ((c is MeridianCo || c is DavreCo) ? new Vector2(860f, 640f) : new Vector2(660f, 796f));
			}
			_refurbScan = -1f;
			_swapScan = -1f;
			_size = val;
			_rect = new Rect(((float)Screen.width / Scale - val.x) / 2f, ((float)Screen.height / Scale - val.y) / 2f, val.x, val.y);
			if (_moved)
			{
				((Rect)(ref _rect)).center = _center;
			}
			ClampToScreen();
			_dragging = false;
			Visible = true;
			_openedThisFrame = true;
			Pointer.PendingClick = null;
		}
	}

	public static void Close()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Visible = false;
		Pointer.PendingClick = null;
		InputShield.Update(visible: false, default(Rect));
	}

	public static void ReleaseUi()
	{
		try
		{
			Close();
		}
		finally
		{
			_theme = null;
			_company = null;
			_toast = null;
			_brandSel = null;
			Cart.Clear();
			NameCache.Clear();
			SwapRows.Clear();
			RefurbRows.Clear();
			BrandFit.Clear();
			_brandFitTheme = null;
			_dragging = false;
			_openedThisFrame = false;
			Ui.ResetCaches();
		}
	}

	private static void Toast(string text)
	{
		_toast = text;
		_toastUntil = Time.unscaledTime + 3f;
	}

	public static void Tick()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float scale = Scale;
		Pointer.Update(_rect, scale, Visible);
		InputShield.Update(Visible, new Rect(((Rect)(ref _rect)).x * scale, (float)Screen.height - ((Rect)(ref _rect)).yMax * scale, ((Rect)(ref _rect)).width * scale, ((Rect)(ref _rect)).height * scale));
		if (Visible)
		{
			if (_openedThisFrame)
			{
				_openedThisFrame = false;
				Pointer.PendingClick = null;
			}
			else if (!Drag() && Input.GetMouseButtonDown(0) && !Pointer.OverWindow)
			{
				Close();
			}
		}
	}

	private static bool Drag()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (_dragging)
		{
			if (!Input.GetMouseButton(0))
			{
				_dragging = false;
				return true;
			}
			((Rect)(ref _rect)).position = Pointer.Logical - _grab;
			ClampToScreen();
			_moved = true;
			_center = ((Rect)(ref _rect)).center;
			Pointer.PendingClick = null;
			return true;
		}
		if (!Input.GetMouseButtonDown(0) || !Pointer.OverWindow)
		{
			return false;
		}
		Vector2 local = Pointer.Local;
		bool flag = local.x > ((Rect)(ref _rect)).width - 40f && local.y < 34f;
		if (local.y >= 80f || flag)
		{
			return false;
		}
		_dragging = true;
		_grab = local;
		Pointer.PendingClick = null;
		return true;
	}

	private static void ClampToScreen()
	{
		float scale = Scale;
		float num = (float)Screen.width / scale;
		float num2 = (float)Screen.height / scale;
		((Rect)(ref _rect)).x = Mathf.Clamp(((Rect)(ref _rect)).x, 120f - ((Rect)(ref _rect)).width, num - 120f);
		((Rect)(ref _rect)).y = Mathf.Clamp(((Rect)(ref _rect)).y, 0f, num2 - 60f);
	}

	public static void Draw()
	{
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Invalid comparison between Unknown and I4
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (!Visible || _company == null)
		{
			return;
		}
		Matrix4x4 matrix = GUI.matrix;
		bool enabled = GUI.enabled;
		Color color = GUI.color;
		Color contentColor = GUI.contentColor;
		Color backgroundColor = GUI.backgroundColor;
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Scale, Scale, 1f));
		GUI.Box(_rect, GUIContent.none, _theme.Box);
		GUI.BeginGroup(_rect);
		Ui.Origin = Vector2.zero;
		Ui.Clip = null;
		try
		{
			if (_company is EnvoyCo)
			{
				DrawEnvoy();
			}
			else if (_company is RebelCo)
			{
				DrawRebel();
			}
			else if (_company is SolanaCo)
			{
				DrawSolana();
			}
			else if (_company is CarmichaelCo)
			{
				DrawCarmichael();
			}
			else if (_company is SpacerCo)
			{
				DrawSpacer();
			}
			else if (_company is MeridianCo)
			{
				DrawMeridian();
			}
			else if (_company is DavreCo)
			{
				DrawDavre();
			}
			else
			{
				DrawPastel();
			}
			if (Ui.Button(new Rect(((Rect)(ref _rect)).width - 34f, 8f, 26f, 22f), "×", _theme.BtnStyle))
			{
				Close();
			}
			if (!string.IsNullOrEmpty(_toast) && Time.unscaledTime < _toastUntil)
			{
				Rect val = new Rect(((Rect)(ref _rect)).width / 2f - 170f, 100f, 340f, 30f);
				GUI.Box(val, GUIContent.none, _theme.CellOnBox);
				GUI.Label(val, _toast, _theme.Center);
			}
		}
		catch (Exception ex)
		{
			ModLog.Warn("面板绘制出错：" + ex.Message);
		}
		finally
		{
			Ui.Origin = Vector2.zero;
			Ui.Clip = null;
			GUI.EndGroup();
			GUI.matrix = matrix;
			GUI.enabled = enabled;
			GUI.color = color;
			GUI.contentColor = contentColor;
			GUI.backgroundColor = backgroundColor;
		}
		Event current = Event.current;
		if (current != null && (int)current.type == 7)
		{
			Pointer.PendingClick = null;
		}
	}

	private static string[] WithExtra(string[] modules)
	{
		Func<Company, string> f = Ext.ExtraModule;
		Company c = _company;
		string extra = ((f == null || c == null) ? null : Ext.Ask("ExtraModule", () => f(c), null));
		string[] array = MenuLabels.Append(modules, extra);
		if (_module >= array.Length)
		{
			_module = 0;
		}
		return array;
	}

	private static bool DrawExtraIfSelected(int baseCount, Rect body)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (_module < baseCount)
		{
			return false;
		}
		Action<Company, Rect, Theme> f = Ext.DrawExtraModule;
		Company c = _company;
		Theme t = _theme;
		if (f != null)
		{
			Ext.Ask("DrawExtraModule", delegate
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				f(c, body, t);
				return true;
			}, fallback: false);
		}
		return true;
	}

	private static void Banner(Rect r)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (_theme.BannerTile > 0)
		{
			int num = 24;
			for (float num2 = ((Rect)(ref r)).x; num2 < ((Rect)(ref r)).xMax; num2 += (float)num)
			{
				for (float num3 = ((Rect)(ref r)).y; num3 < ((Rect)(ref r)).yMax; num3 += (float)num)
				{
					Ui.Image(new Rect(num2, num3, Mathf.Min((float)num, ((Rect)(ref r)).xMax - num2), Mathf.Min((float)num, ((Rect)(ref r)).yMax - num3)), _theme.Banner);
				}
			}
		}
		else if ((Object)(object)_theme.Banner != (Object)null)
		{
			Ui.Image(r, _theme.Banner);
		}
	}

	private static void CoopBar(Rect r)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		CompanyState state = State;
		Ui.Fill(r, _theme.Accent2);
		Ui.Fill(new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, ((Rect)(ref r)).width - 2f, ((Rect)(ref r)).height - 2f), _theme.Paper);
		Ui.Fill(new Rect(((Rect)(ref r)).x + 1f, ((Rect)(ref r)).y + 1f, (((Rect)(ref r)).width - 2f) * (float)state.Coop / 100f, ((Rect)(ref r)).height - 2f), _theme.Accent);
		int[] tierAt = Company.TierAt;
		foreach (int num in tierAt)
		{
			Ui.Fill(new Rect(((Rect)(ref r)).x + ((Rect)(ref r)).width * (float)num / 100f, ((Rect)(ref r)).y, 1f, ((Rect)(ref r)).height), _theme.Ink);
		}
	}

	private static string NextTierHint()
	{
		CompanyState state = State;
		int num = _company.NextTierAt(state);
		if (num != 0)
		{
			if (!L.En)
			{
				return $"距“{_company.TierNames[Array.IndexOf(Company.TierAt, num)]}”还差 {num - state.Coop} 点";
			}
			return $"{num - state.Coop} more to “{_company.TierNames[Array.IndexOf(Company.TierAt, num)]}”";
		}
		return L.T("已是最高档", "Top tier reached");
	}

	private static string TierLabel(CompanyState s)
	{
		return _company.TierName(s);
	}

	private static void DrawDavre()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		float width = ((Rect)(ref _rect)).width;
		float height = ((Rect)(ref _rect)).height;
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector(20f, 8f, 560f, 90f);
		if ((Object)(object)theme.Banner != (Object)null)
		{
			Ui.Image(r, theme.Banner);
			GUI.Label(new Rect(((Rect)(ref r)).x + 124f, ((Rect)(ref r)).y + 16f, 416f, 58f), _company.Name + L.T(" · 供货单", " · Supply Order"), theme.Title);
		}
		else
		{
			Ui.Fill(new Rect(20f, 16f, 560f, 64f), theme.Accent);
			GUI.Label(new Rect(36f, 16f, 520f, 64f), _company.Name + L.T(" · 供货单", " · Supply Order"), theme.InvTitle);
		}
		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"Order DAV-{_company.PhoneNumber}   Day {Day}" : $"单号 DAV-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Accent);
		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("订货中心：", "Order center: ") + _desk, theme.Body);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(20f, 136f, 220f, height - 156f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		Rect r2 = default(Rect);
		((Rect)(ref r2))._002Ector(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 10f, 200f, 146f);
		Ui.Fill(r2, Tex.Hex(13483928));
		Texture2D val2 = Portraits.Texture("davre_doreen", "femaleLower_0");
		if ((Object)(object)val2 != (Object)null)
		{
			Ui.Image(new Rect(((Rect)(ref r2)).x + 2f, ((Rect)(ref r2)).y + 2f, ((Rect)(ref r2)).width - 4f, ((Rect)(ref r2)).height - 4f), val2);
		}
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 4f, 200f, 20f), L.T("供货专员：多琳", "Supply rep: Doreen"), theme.Small);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 28f, 200f, 22f), L.En ? ("Tier: " + TierLabel(state)) : ("档位：" + TierLabel(state)), theme.Body);
		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 54f, 200f, 8f));
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 64f, 200f, 20f), NextTierHint(), theme.Small);
		float num = ((Rect)(ref r2)).yMax + 96f;
		for (int i = 0; i < DavreModules.Length; i++)
		{
			string text = ((i == _module) ? "› " : "  ") + DavreModules[i];
			if (Ui.Button(new Rect(((Rect)(ref val)).x + 6f, num + (float)(i * 32), ((Rect)(ref val)).width - 12f, 28f), text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
		}
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref val)).xMax + 20f, 136f, width - ((Rect)(ref val)).xMax - 40f, height - 156f);
		GUI.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 30f), DavreModules[_module], theme.HotTitle);
		Ui.Fill(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 34f, ((Rect)(ref val3)).width, 1f), theme.Ink);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 44f, ((Rect)(ref val3)).width, ((Rect)(ref val3)).height - 44f);
		if (DrawExtraIfSelected(DavreBase.Length, val4))
		{
			return;
		}
		if (_company.Tier(state) < 1 && _module < 3)
		{
			GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 40f), L.T("还没有供货编号。", "No supplier number yet."), theme.Wrap);
			return;
		}
		switch (_module)
		{
		case 0:
			DrawDavreOrder(val4);
			break;
		case 1:
			DrawDavreLeviathan(val4);
			break;
		case 2:
			DrawDavreCanning(val4);
			break;
		default:
			DrawBrand(val4);
			break;
		}
	}

	private static int DavreUnitPrice(CatalogEntry e)
	{
		return e.Quote(Company.OrderRate(_company, _company.Tier(State)));
	}

	private static void DrawDavreOrder(Rect r)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		int num = _company.CatalogTier(State);
		bool flag = Services.CanOrderToday(_company);
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), flag ? L.T("今天下单，明天到货。", "Order today, delivered tomorrow.") : L.T("今天已经下过单了。", "Already ordered today."), theme.Small);
		float x = ((Rect)(ref r)).x;
		float num2 = ((Rect)(ref r)).x + 290f;
		float num3 = ((Rect)(ref r)).x + 380f;
		float num4 = ((Rect)(ref r)).x + 500f;
		float num5 = ((Rect)(ref r)).y + 26f;
		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 26f), theme.Accent);
		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
		num5 += 30f;
		CatalogEntry[] entries = DavreCo.Entries;
		foreach (CatalogEntry catalogEntry in entries)
		{
			Ui.Fill(new Rect(((Rect)(ref r)).x, num5 + 33f, ((Rect)(ref r)).width, 1f), theme.Line);
			GUI.Label(new Rect(x + 8f, num5, 280f, 34f), catalogEntry.Name() + Services.LeftText(State, catalogEntry), theme.Body);
			if (catalogEntry.MinTier > num)
			{
				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 34f), L.En ? ("Opens at " + _company.TierNames[catalogEntry.MinTier - 1]) : (_company.TierNames[catalogEntry.MinTier - 1] + "开放"), theme.Small);
				Cart.Remove(catalogEntry);
				num5 += 36f;
				continue;
			}
			int num6 = DavreUnitPrice(catalogEntry);
			if (num6 <= 0)
			{
				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 34f), L.T("暂不可用", "Unavailable"), theme.Small);
				Cart.Remove(catalogEntry);
				num5 += 36f;
				continue;
			}
			GUI.Label(new Rect(num2, num5, 90f, 34f), $"{num6}₵", theme.Price);
			Cart.TryGetValue(catalogEntry, out var value);
			value = Ui.Qty(new Rect(num3, num5, 120f, 34f), value, Math.Min(10, Services.Left(State, catalogEntry)), theme.BtnStyle, theme.Center);
			if (value > 0)
			{
				Cart[catalogEntry] = value;
			}
			else
			{
				Cart.Remove(catalogEntry);
			}
			GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 34f), (value > 0) ? $"{num6 * value}₵" : "—", theme.Center);
			num5 += 36f;
		}
		int num7 = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => DavreUnitPrice(kv.Key) * kv.Value);
		float num8 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num8, 200f, 22f), L.En ? $"Total {num7}₵" : $"合计 {num7}₵", theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).x, num8 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = GUI.enabled;
		GUI.enabled = num7 > 0 && num7 <= Cash && flag;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num8 + 2f, 160f, 36f), L.T("提交供货单", "Submit order"), theme.BtnHotStyle))
		{
			string text = Services.PlaceOrder(_company, Cart, DavreUnitPrice);
			if (text == null)
			{
				Cart.Clear();
				Toast(_company.FrontDeskDone);
			}
			else
			{
				Toast(text);
			}
		}
		GUI.enabled = enabled;
	}

	private static void DrawDavreCanning(Rect r)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		DavreCo c = (DavreCo)_company;
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 66f), L.T("绞肉机把生肉绞成肉糜，罐装机每晚把肉糜和营养果封成罐头：合成档出“泥炭”，鲜肉档出鲜肉罐头。", "The grinder turns raw meat into mince; each night the canner seals mince and nutrifruit into cans: Peat on Synthetic, fresh meat on Fresh."), theme.Wrap);
		float num = ((Rect)(ref r)).y + 76f;
		if (!DavreCo.CanningOpen && !DavreCo.CanningOffered)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 44f), L.En ? $"Doreen will come to talk once cooperation reaches {60}." : $"合作度到 {60} 后，多琳会上门来谈。", theme.Wrap);
			return;
		}
		if (!DavreCo.CanningOpen)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 44f), L.En ? $"Doreen already made the offer. {DavreCo.CanningFee}₵ fee for one grinder and one canner, delivered tomorrow." : $"多琳问过你了。手续费 {DavreCo.CanningFee}₵，绞肉机和罐装机各一台，明天送到。", theme.Wrap);
			float num2 = ((Rect)(ref r)).yMax - 44f;
			GUI.Label(new Rect(((Rect)(ref r)).x, num2 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
			bool enabled = GUI.enabled;
			GUI.enabled = Cash >= DavreCo.CanningFee;
			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num2 + 2f, 160f, 36f), L.En ? $"Sign up ({DavreCo.CanningFee}₵)" : $"补办（{DavreCo.CanningFee}₵）", theme.BtnHotStyle))
			{
				Toast(Services.MakeUpCanning(c) ?? _company.FrontDeskDone);
			}
			GUI.enabled = enabled;
			return;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 20f), L.T("原价，明天到货，不限量。", "List price, delivered tomorrow, no limit."), theme.Small);
		num += 28f;
		(string, string)[] canningMachines = DavreCo.CanningMachines;
		Rect val = default(Rect);
		for (int i = 0; i < canningMachines.Length; i++)
		{
			(string, string) tuple = canningMachines[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			Brand.Entry entry = Brand.Info(item);
			int num3 = (int)Goods.ValueOf(item2);
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x, num, 96f, 96f);
			GUI.Box(val, GUIContent.none, theme.CellBox);
			if ((Object)(object)entry.Icon != (Object)null)
			{
				Ui.Image(new Rect(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y + 4f, 88f, 88f), entry.Icon);
			}
			GUI.Label(new Rect(((Rect)(ref val)).xMax + 16f, num + 14f, 260f, 26f), entry.Name, theme.Body);
			GUI.Label(new Rect(((Rect)(ref val)).xMax + 16f, num + 44f, 200f, 22f), $"{num3}₵", theme.Price);
			bool enabled2 = GUI.enabled;
			GUI.enabled = Cash >= num3;
			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num + 30f, 160f, 36f), L.T("订一台", "Order one"), theme.BtnHotStyle))
			{
				Toast(Services.OrderCanningMachine(c, item, num3) ?? _company.FrontDeskDone);
			}
			GUI.enabled = enabled2;
			num += 108f;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).yMax - 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
	}

	private static int LeviathanPrice()
	{
		if (_leviathan == null)
		{
			_leviathan = new CatalogEntry
			{
				Id = "@leviathan",
				Page = "利维坦"
			};
		}
		return _leviathan.Quote(Settings.OrderMarkup);
	}

	private static void DrawDavreLeviathan(Rect r)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		DavreCo davreCo = (DavreCo)_company;
		if (!DavreCo.LeviathanOpen)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 60f), L.T("达夫雷自己的净水器，只供给卖水的店。", "Davre's own water purifier, supplied only to shops that sell water."), theme.Wrap);
			return;
		}
		int num = davreCo.LeviathanLimit(state);
		int num2 = DavreCo.LeviathanUsed(state);
		int num3 = LeviathanPrice();
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 66f), L.T("照着海德拉做的。铁锈水、幽灵水都能净化成基础水，沟渠水不行。耐久 600，能净化 60 升。\n", "Modeled on the HydraTech. Purifies Rustwater and Ghostwater to Basewater, but not Gutterflow. 600 durability, good for 60 liters.\n") + (L.En ? $"{num} per restock period (2 from {_company.TierNames[2]}), {num3}₵ each, delivered tomorrow." : $"每个补货周期可订 {num} 台（{_company.TierNames[2]}起 2 台），每台 {num3}₵，明天到货。"), theme.Wrap);
		Texture2D icon = Brand.Info("@leviathan").Icon;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((Rect)(ref r)).x, ((Rect)(ref r)).y + 80f, 136f, 136f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		if ((Object)(object)icon != (Object)null)
		{
			Ui.Image(new Rect(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y + 4f, 128f, 128f), icon);
		}
		GUI.Label(new Rect(((Rect)(ref val)).xMax + 20f, ((Rect)(ref val)).y + 10f, 300f, 26f), L.En ? $"Ordered {Services.PeriodWord}: {num2}/{num}" : $"{Services.PeriodWord}已订 {num2}/{num} 台", theme.Body);
		GUI.Label(new Rect(((Rect)(ref val)).xMax + 20f, ((Rect)(ref val)).y + 40f, 300f, 22f), L.En ? $"Restocks on day {Services.NextRestockDay}" : $"第 {Services.NextRestockDay} 天补货", theme.Small);
		float num4 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num4, 200f, 22f), L.En ? $"Unit price {num3}₵" : $"单价 {num3}₵", theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).x, num4 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = GUI.enabled;
		GUI.enabled = num3 > 0 && num2 < num && Cash >= num3;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num4 + 2f, 160f, 36f), L.T("订一台", "Order one"), theme.BtnHotStyle))
		{
			Toast(Services.OrderLeviathan(davreCo, num3) ?? _company.FrontDeskDone);
		}
		GUI.enabled = enabled;
	}

	private static void DrawEnvoy()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0879: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		EnvoyState envoyState = MaliceState.Get()?.Envoy;
		float width = ((Rect)(ref _rect)).width;
		Ui.Fill(new Rect(20f, 20f, width - 40f, 56f), theme.Ink);
		GUI.Label(new Rect(36f, 20f, width - 80f, 56f), _company.Name, theme.InvTitle);
		GUI.Label(new Rect(24f, 90f, width - 48f, 22f), L.T("撒该：", "Zacchaeus: ") + _desk, theme.Body);
		Ui.Fill(new Rect(20f, 116f, width - 40f, 1f), theme.Line);
		if (envoyState == null)
		{
			return;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(30f, 130f, width - 60f, ((Rect)(ref _rect)).height - 150f);
		float y = ((Rect)(ref val)).y;
		GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("援助", "Aid"), theme.Title);
		y += 30f;
		string text = envoyState.Aid switch
		{
			"active" => L.En ? $"Support period: keep Pastel and Carmicheal subscribed until day {envoyState.AidStart + 21}." : $"扶持期：第 {envoyState.AidStart + 21} 天之前，帕斯泰尔和卡迈克尔的订购服务不能断。", 
			"repay5" => L.En ? $"Repaying: {envoyState.RepayLeft} installments of {800}₵ left; he collects on day {envoyState.RepayDue}. Missing one brings a penalty." : $"还款中：还剩 {envoyState.RepayLeft} 期，每期 {800}₵，第 {envoyState.RepayDue} 天他来收。交不上会受罚。", 
			"plan6" => (L.En ? $"Repayment plan: {envoyState.RepayLeft} installments of {800}₵ left; he collects on day {envoyState.RepayDue}." : $"还款计划：还剩 {envoyState.RepayLeft} 期，每期 {800}₵，第 {envoyState.RepayDue} 天他来收。") + ((envoyState.LateWeeks <= 0) ? "" : (L.En ? $" {envoyState.LateWeeks} week(s) late: corporate orders +{envoyState.LateWeeks * 10}%." : $"已拖 {envoyState.LateWeeks} 周，巨企订货 +{envoyState.LateWeeks * 10}%。")), 
			"punished" => L.T("惩罚中：所有公司服务暂停（反抗军除外），每天少一位顾客。", "Penalty: all company services are suspended (except the Resistance), and one fewer customer comes each day."), 
			"done" => L.T("已结清。", "Settled."), 
			"declined" => L.T("你拒绝了援助。", "You declined the aid."), 
			_ => L.T("无。", "None."), 
		};
		GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 44f), text, theme.Wrap);
		y += 48f;
		if (envoyState.Aid == "punished" && Ui.Button(new Rect(((Rect)(ref val)).x, y, 360f, 34f), L.En ? $"Agree to repay: {800}₵ a week for {6} weeks" : $"答应还款：每周 {800}₵，{6} 周", theme.BtnHotStyle))
		{
			Toast(Envoy.AgreePlan() ?? _company.FrontDeskDone);
		}
		if (envoyState.Aid == "punished")
		{
			y += 42f;
		}
		if (envoyState.Ch2 != "")
		{
			Ui.Fill(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 1f), theme.Line);
			y += 8f;
			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("服务费", "Service fee"), theme.Title);
			y += 30f;
			string text2 = envoyState.Ch2 switch
			{
				"refused" => L.T("你拒绝了他。巨企的订货暂不开放，直到你交钱或收下礼物并办成。", "You turned him down. Corporate ordering stays closed until you pay or take the gift and deliver it."), 
				"bomb" => L.En ? $"Sell the gift to Odin before day {envoyState.BombDeadline} (call him over; if he's been arrested, sell it through the underground buyer)." : $"礼物要在第 {envoyState.BombDeadline} 天之前卖给奥丁（打他的电话叫他来；他被捕了就通过地下买家卖出）。", 
				"bombdone" => L.T("礼物送到了。巨企订购服务不收订阅费。", "Gift delivered. Corporate ordering services are subscription-free."), 
				"malice" => L.En ? $"Corporate malice: channel fees until day {envoyState.ChannelUntil}. {envoyState.ChannelFee}₵ so far this week; he collects on day {envoyState.ChannelDue}." : $"巨企恶意：渠道费收到第 {envoyState.ChannelUntil} 天，这一期已记 {envoyState.ChannelFee}₵，第 {envoyState.ChannelDue} 天他来收。", 
				_ => L.T("已结清。", "Settled."), 
			};
			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 44f), text2, theme.Wrap);
			y += 48f;
			if (envoyState.Ch2 == "refused")
			{
				if (Ui.Button(new Rect(((Rect)(ref val)).x, y, 220f, 34f), L.En ? $"Pay {1500}₵" : $"付 {1500}₵", theme.BtnHotStyle))
				{
					Toast(Envoy.PayServiceFee() ?? _company.FrontDeskDone);
				}
				if (Ui.Button(new Rect(((Rect)(ref val)).x + 230f, y, 220f, 34f), L.T("收下礼物", "Take the gift"), theme.BtnStyle))
				{
					Envoy.AcceptBomb();
					Toast(L.T("礼物放在柜台上了。", "The gift is on your counter."));
				}
				y += 42f;
			}
		}
		if (envoyState.Ch3 == "joined")
		{
			Ui.Fill(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 1f), theme.Line);
			y += 8f;
			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("各办各事", "Each to their own"), theme.Title);
			y += 30f;
			GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 44f), L.En ? $"Pay Zacchaeus {3000}₵, then 1000₵ to make peace on the Resistance's line: the Resistance is back in touch (no more underground buyers) and your corporate perks stay." : $"给撒该 {3000}₵，再在反抗军的电话里花 1000₵ 和解：反抗军恢复联系（不再有地下买家），巨企的待遇照旧。", theme.Wrap);
			y += 48f;
			if (!envoyState.BribedEnvoy && Ui.Button(new Rect(((Rect)(ref val)).x, y, 260f, 34f), L.En ? $"Pay off Zacchaeus ({3000}₵)" : $"打点撒该（{3000}₵）", theme.BtnHotStyle))
			{
				Toast(Envoy.BribeForReconcile() ?? _company.FrontDeskDone);
			}
			if (envoyState.BribedEnvoy)
			{
				GUI.Label(new Rect(((Rect)(ref val)).x, y, ((Rect)(ref val)).width, 24f), L.T("已打点。", "Paid."), theme.Small);
			}
		}
	}

	private static void DrawRebel()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		MaliceState maliceState = MaliceState.Get();
		float width = ((Rect)(ref _rect)).width;
		float height = ((Rect)(ref _rect)).height;
		new Rect(20f, 8f, 560f, 90f);
		Ui.Fill(new Rect(20f, 20f, 420f, 56f), theme.Ink);
		GUI.Label(new Rect(36f, 20f, 400f, 56f), _company.Name, theme.InvTitle);
		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"{_company.PhoneNumber}   Day {Day}" : $"{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Ink);
		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("奥丁：", "Odin: ") + _desk, theme.Body);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(20f, 136f, 190f, height - 156f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 8f, 170f, 22f), L.En ? ("Standing: " + TierLabel(state)) : ("身份：" + TierLabel(state)), theme.Body);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 32f, 170f, 20f), L.En ? $"Trust {state.Coop}" : $"信任度 {state.Coop}", theme.Small);
		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 54f, 170f, 8f));
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 64f, 170f, 20f), NextTierHint(), theme.Small);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 88f, 170f, 22f), L.En ? $"Support points {maliceState?.RebelPoints ?? 0}" : $"助力点 {maliceState?.RebelPoints ?? 0}", theme.Body);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 110f, 170f, 20f), L.En ? $"Total earned {maliceState?.RebelEarned ?? 0}" : $"累计 {maliceState?.RebelEarned ?? 0}", theme.Small);
		string[] rebelModules = RebelModules;
		if (_module >= rebelModules.Length)
		{
			_module = 0;
		}
		float num = ((Rect)(ref val)).y + 140f;
		for (int i = 0; i < rebelModules.Length; i++)
		{
			string text = ((i == _module) ? "› " : "  ") + rebelModules[i];
			if (Ui.Button(new Rect(((Rect)(ref val)).x + 6f, num, ((Rect)(ref val)).width - 12f, 28f), text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
			num += 32f;
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).xMax + 20f, 136f, width - ((Rect)(ref val)).xMax - 40f, height - 156f);
		GUI.Label(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, 30f), rebelModules[_module], theme.HotTitle);
		Ui.Fill(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y + 34f, ((Rect)(ref val2)).width, 1f), theme.Ink);
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y + 44f, ((Rect)(ref val2)).width, ((Rect)(ref val2)).height - 44f);
		if (Rebels.Arrested)
		{
			DrawRescue(r);
		}
		else if (Rebels.Cut)
		{
			EnvoyState envoyState = maliceState?.Envoy;
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 80f), L.T("你加入了巨企，反抗军和你断了联系。\n花 1000₵ 和解（还要在撒该那边打点 3000₵）：反抗军恢复联系，但不再给你安排地下买家。", "You joined the corporations and the Resistance cut you off.\nPay 1000₵ to make peace (and pay off Zacchaeus 3000₵ on his line): the Resistance is back in touch, but no more underground buyers."), theme.Wrap);
			if (envoyState != null && envoyState.ReconciledRebels)
			{
				GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 90f, ((Rect)(ref r)).width, 24f), envoyState.Gone ? L.T("已付。明天开店时恢复联系。", "Paid. Back in touch from tomorrow's opening.") : L.T("已付。等撒该那边也打点好。", "Paid. Waiting on Zacchaeus's side."), theme.Small);
			}
			else if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 90f, 240f, 34f), L.T("和解（1000₵）", "Make peace (1000₵)"), theme.BtnHotStyle))
			{
				Toast(Envoy.ReconcileWithRebels() ?? _company.FrontDeskDone);
			}
		}
		else if (_company.Tier(state) < 1)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), L.T("没人认识你。", "Nobody knows you."), theme.Wrap);
		}
		else if (_module == 0)
		{
			DrawSpacerOrder(r, "军械");
		}
		else if (rebelModules[_module] == L.T("任务", "Tasks"))
		{
			DrawRebelTasks(r);
		}
		else
		{
			DrawRebelFence(r);
		}
	}

	private static void DrawRebelFence(Rect r)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		MaliceState maliceState = MaliceState.Get();
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 84f), L.En ? $"Book an underground buyer for tomorrow: takes anything, pays cash at retail +10%, doesn't carry much. Selling to him won't raise Security's investigation.\nOne booking every {5} days for {50} support points; book early for {25} more per day early, up to {150}." : $"约一位地下买家明天上门：什么都收，按零售价加 10% 付现钱，带的钱不多。卖给他的东西不会让治安部的调查进度上涨。\n每 {5} 天可以约一次，花 {50} 助力点；等不及就紧急预约，每提前一天多花 {25} 点，最多 {150} 点。", theme.Wrap);
		int num = Rebels.FenceCost();
		bool flag = num > 50;
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 96f, ((Rect)(ref r)).width, 22f), L.En ? ($"Booking now: {num} points" + (flag ? " (early)" : "")) : ($"现在预约：{num} 点" + (flag ? "（紧急预约）" : "")), theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 120f, ((Rect)(ref r)).width, 22f), L.En ? $"Support points: {maliceState?.RebelPoints ?? 0}. 1/{10} of what you sell to the Resistance or order here becomes support points." : $"助力点余额：{maliceState?.RebelPoints ?? 0}。卖给反抗军、在这里订货，金额的 1/{10} 计为助力点。", theme.Small);
		bool enabled = GUI.enabled;
		GUI.enabled = (maliceState?.RebelPoints ?? 0) >= num;
		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 152f, 240f, 34f), L.En ? $"Book ({num} points)" : $"预约（{num} 点）", theme.BtnHotStyle))
		{
			Toast(Rebels.BookFence() ?? _company.FrontDeskDone);
		}
		GUI.enabled = enabled;
	}

	private static void DrawRescue(Rect r)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		RebelState rebelState = MaliceState.Get()?.Rebel;
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 64f), L.En ? $"Odin has been arrested; orders and the fence network are suspended.\nTo get him out, the Resistance needs {350}₵ worth of medical supplies and weapons. Call for a pickup when ready; someone comes the next day. You can hand them over in several trips (no payment)." : $"奥丁被捕了，订货和地下销赃网络都停了。\n要救他出来，反抗军需要价值 {350}₵ 的医药和武器。准备好了就呼叫提货，第二天有人来取，可以分几次交（交东西不给钱）。", theme.Wrap);
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 72f, ((Rect)(ref r)).width, 22f), L.En ? $"Delivered: {rebelState?.RescueValue ?? 0} / {350}₵" : $"已交：{rebelState?.RescueValue ?? 0} / {350}₵", theme.Price);
		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 104f, 240f, 34f), L.T("呼叫提货", "Call for pickup"), theme.BtnHotStyle))
		{
			Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
		}
	}

	private static void DrawRebelTasks(Rect r)
	{
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		MaliceState maliceState = MaliceState.Get();
		RebelState rebelState = maliceState?.Rebel;
		if (rebelState != null)
		{
			string text = rebelState.Active switch
			{
				1 => L.En ? $"Odin wants a bottle of poison, or {100}₵ worth of Dream Dust and Phanta Pills. Delivered so far: {rebelState.Ch1Value}₵." : $"奥丁要一瓶毒药，或者价值 {100}₵ 的梦尘和芬太片。已交梦尘、芬太片 {rebelState.Ch1Value}₵。", 
				2 => (rebelState.Ch2Route == "bribe") ? L.T("已经打点了子午线：炸弹送到后，奥丁会来买。", "Meridian has been paid off: once the bomb arrives, Odin will come for it.") : (L.En ? $"Odin wants explosives and Meridian heavy firepower: call Meridian and pay them off on the Corporate Terms page ({RebelStory.Ch2BribePrice()}₵), or sell {1000}₵ of weapons through the underground buyer ({rebelState.Ch2FenceWeapons}₵ so far)." : $"奥丁要炸药和子午线的重火力：打子午线的电话，在“巨企合约”页打点（{RebelStory.Ch2BribePrice()}₵）；或者通过地下买家卖出 {1000}₵ 的武器（已卖 {rebelState.Ch2FenceWeapons}₵）。"), 
				3 => L.En ? $"The crackdown left many wounded. Odin comes weekly for medical supplies: {1200}₵ in total ({rebelState.Ch3Medical}₵ so far)." : $"清缴之后伤员很多。奥丁每周来收医疗品，累计 {1200}₵（已卖 {rebelState.Ch3Medical}₵）。", 
				4 => (!rebelState.Joined) ? L.T("奥丁邀请你加入反抗军。想好了就叫他来。", "Odin has invited you to join the Resistance. Call him over when you've decided.") : (L.En ? $"Odin comes weekly for weapons and medical supplies: {3000}₵ in total ({rebelState.Ch4Sales}₵ so far)." : $"奥丁每周来收武器和医疗品，累计 {3000}₵（已卖 {rebelState.Ch4Sales}₵）。"), 
				5 => L.En ? $"The Mentor is negotiating with the corporations. Each corporation at 100 cooperation sends a Coin of Fate ({maliceState.Envoy.Coins.Count} / 5)." : $"导师在替你和巨企谈。合作度满 100 的巨企会派人送命运硬币（已收 {maliceState.Envoy.Coins.Count} / 5）。", 
				_ => (rebelState.Done >= 5) ? L.T("你是下层区的代表。", "You are the Lower Levels' representative.") : (L.En ? $"No task right now. Keep dealing with the Resistance ({maliceState.RebelEarned} support points earned)." : $"暂时没有任务。多和反抗军打交道（助力点累计 {maliceState.RebelEarned}）。"), 
			};
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), text, theme.Wrap);
			float num = ((Rect)(ref r)).y + 100f;
			if (RebelStory.PickupOpen && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫提货", "Call for pickup"), theme.BtnHotStyle))
			{
				Toast(RebelStory.CallPickup() ?? L.T("明天有人来取。", "Someone will come tomorrow."));
			}
			if (RebelStory.CanCallOdin && Ui.Button(new Rect(((Rect)(ref r)).x, num, 240f, 34f), L.T("呼叫奥丁", "Call Odin"), theme.BtnHotStyle))
			{
				Toast(RebelStory.CallOdin() ?? L.T("他明天来。", "He'll come tomorrow."));
			}
		}
	}

	private static void DrawMeridian()
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		float width = ((Rect)(ref _rect)).width;
		float height = ((Rect)(ref _rect)).height;
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector(20f, 8f, 560f, 90f);
		if ((Object)(object)theme.Banner != (Object)null)
		{
			Ui.Image(r, theme.Banner);
			GUI.Label(new Rect(((Rect)(ref r)).x + 120f, ((Rect)(ref r)).y + 16f, 420f, 58f), _company.Name + L.T(" · 军械申领表", " · Ordnance Requisition Form"), theme.InvTitle);
		}
		else
		{
			Ui.Fill(new Rect(20f, 16f, 560f, 64f), theme.Ink);
			Ui.Fill(new Rect(20f, 80f, 560f, 4f), theme.Hot);
			GUI.Label(new Rect(36f, 16f, 520f, 64f), _company.Name + L.T(" · 军械申领表", " · Ordnance Requisition Form"), theme.InvTitle);
		}
		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"Form MRD-{_company.PhoneNumber}   Day {Day}" : $"表 MRD-{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Ink);
		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("订货处：", "Order desk: ") + _desk, theme.Body);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(20f, 136f, 220f, height - 156f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		Rect r2 = default(Rect);
		((Rect)(ref r2))._002Ector(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 10f, 200f, 146f);
		Ui.Fill(r2, Tex.Hex(12104616));
		Texture2D val2 = Portraits.Texture("meridian_rosen", "maleSecOff_0");
		if ((Object)(object)val2 != (Object)null)
		{
			Ui.Image(new Rect(((Rect)(ref r2)).x + 2f, ((Rect)(ref r2)).y + 2f, ((Rect)(ref r2)).width - 4f, ((Rect)(ref r2)).height - 4f), val2);
		}
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 4f, 200f, 20f), L.T("经办：罗森（驻站负责人）", "Handled by: Rosen (Station Manager)"), theme.Small);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 28f, 200f, 22f), L.En ? ("License: " + TierLabel(state)) : ("许可：" + TierLabel(state)), theme.Body);
		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 54f, 200f, 8f));
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref r2)).yMax + 64f, 200f, 20f), NextTierHint(), theme.Small);
		float num = ((Rect)(ref r2)).yMax + 96f;
		for (int i = 0; i < MeridianModules.Length; i++)
		{
			string text = ((i == _module) ? "› " : "  ") + MeridianModules[i];
			if (Ui.Button(new Rect(((Rect)(ref val)).x + 6f, num + (float)(i * 32), ((Rect)(ref val)).width - 12f, 28f), text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
		}
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref val)).xMax + 20f, 136f, width - ((Rect)(ref val)).xMax - 40f, height - 156f);
		GUI.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 30f), MeridianModules[_module], theme.HotTitle);
		Ui.Fill(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 34f, ((Rect)(ref val3)).width, 1f), theme.Ink);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(((Rect)(ref val3)).x, ((Rect)(ref val3)).y + 44f, ((Rect)(ref val3)).width, ((Rect)(ref val3)).height - 44f);
		if (DrawExtraIfSelected(MeridianBase.Length, val4))
		{
			return;
		}
		if (_company.Tier(state) < 1 && _module < 3)
		{
			GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 40f), L.T("没有许可。", "No license."), theme.Wrap);
			return;
		}
		switch (_module)
		{
		case 0:
			DrawMeridianOrder(val4, MeridianCo.Entries, L.T("今天下单，明天到。全新出厂。", "Order today, here tomorrow. Factory new."));
			break;
		case 1:
		{
			int num2 = (MeridianCo.Parts.Length + 8 - 1) / 8;
			_partsPage = Math.Max(0, Math.Min(_partsPage, num2 - 1));
			if (Ui.Button(new Rect(((Rect)(ref val4)).xMax - 150f, ((Rect)(ref val4)).y - 40f, 32f, 26f), "‹", theme.BtnStyle) && _partsPage > 0)
			{
				_partsPage--;
			}
			GUI.Label(new Rect(((Rect)(ref val4)).xMax - 116f, ((Rect)(ref val4)).y - 40f, 80f, 26f), $"{_partsPage + 1}/{num2}", theme.Center);
			if (Ui.Button(new Rect(((Rect)(ref val4)).xMax - 34f, ((Rect)(ref val4)).y - 40f, 32f, 26f), "›", theme.BtnStyle) && _partsPage < num2 - 1)
			{
				_partsPage++;
			}
			DrawMeridianOrder(val4, MeridianCo.Parts.Skip(_partsPage * 8).Take(8).ToArray(), L.En ? $"Vanilla attachments, steady supply, at {MeridianCo.PartsRate:0.0}x value. Counts as the same daily order as standard issue." : $"原版配件，稳定供货，按价值的 {MeridianCo.PartsRate:0.0} 倍。和制式订货算同一天的一单。");
			break;
		}
		case 2:
			DrawMeridianRefurb(val4);
			break;
		case 3:
			DrawMeridianForge(val4);
			break;
		default:
			DrawBrand(val4);
			break;
		}
	}

	private static int MeridianUnitPrice(CatalogEntry e)
	{
		return e.Quote((Array.IndexOf(MeridianCo.Parts, e) >= 0) ? MeridianCo.PartsRate : Company.OrderRate(_company, _company.Tier(State)));
	}

	private static void DrawMeridianOrder(Rect r, CatalogEntry[] entries, string note)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		int num = _company.CatalogTier(State);
		bool flag = Services.CanOrderToday(_company);
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), flag ? note : L.T("今天已经下过单了。", "Already ordered today."), theme.Small);
		float x = ((Rect)(ref r)).x;
		float num2 = ((Rect)(ref r)).x + 290f;
		float num3 = ((Rect)(ref r)).x + 380f;
		float num4 = ((Rect)(ref r)).x + 500f;
		float num5 = ((Rect)(ref r)).y + 26f;
		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 26f), theme.Ink);
		GUI.Label(new Rect(x + 8f, num5, 280f, 26f), L.T("品名", "Item"), theme.InvBody);
		GUI.Label(new Rect(num2, num5, 90f, 26f), L.T("单价", "Unit price"), theme.InvCenter);
		GUI.Label(new Rect(num3, num5, 120f, 26f), L.T("数量", "Qty"), theme.InvCenter);
		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 26f), L.T("小计", "Subtotal"), theme.InvCenter);
		num5 += 30f;
		foreach (CatalogEntry catalogEntry in entries)
		{
			Ui.Fill(new Rect(((Rect)(ref r)).x, num5 + 33f, ((Rect)(ref r)).width, 1f), theme.Line);
			GUI.Label(new Rect(x + 8f, num5, 280f, 34f), catalogEntry.Name() + Services.LeftText(State, catalogEntry), theme.Body);
			if (catalogEntry.MinTier > num)
			{
				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 34f), L.En ? ("Opens at " + _company.TierNames[catalogEntry.MinTier - 1]) : (_company.TierNames[catalogEntry.MinTier - 1] + "开放"), theme.Small);
				Cart.Remove(catalogEntry);
				num5 += 36f;
				continue;
			}
			int num6 = MeridianUnitPrice(catalogEntry);
			if (num6 <= 0)
			{
				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 34f), L.T("暂不可用", "Unavailable"), theme.Small);
				Cart.Remove(catalogEntry);
				num5 += 36f;
				continue;
			}
			GUI.Label(new Rect(num2, num5, 90f, 34f), $"{num6}₵", theme.Price);
			Cart.TryGetValue(catalogEntry, out var value);
			value = Ui.Qty(new Rect(num3, num5, 120f, 34f), value, Math.Min(10, Services.Left(State, catalogEntry)), theme.BtnStyle, theme.Center);
			if (value > 0)
			{
				Cart[catalogEntry] = value;
			}
			else
			{
				Cart.Remove(catalogEntry);
			}
			GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 34f), (value > 0) ? $"{num6 * value}₵" : "—", theme.Center);
			num5 += 36f;
		}
		int num7 = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => MeridianUnitPrice(kv.Key) * kv.Value);
		float num8 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num8, 200f, 22f), L.En ? $"Total {num7}₵" : $"合计 {num7}₵", theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).x, num8 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = GUI.enabled;
		GUI.enabled = num7 > 0 && num7 <= Cash && flag;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num8 + 2f, 160f, 36f), L.T("提交申领", "Submit requisition"), theme.BtnHotStyle))
		{
			string text = Services.PlaceOrder(_company, Cart, MeridianUnitPrice);
			if (text == null)
			{
				Cart.Clear();
				Toast(_company.FrontDeskDone);
			}
			else
			{
				Toast(text);
			}
		}
		GUI.enabled = enabled;
	}

	private static void ScanRefurb()
	{
		if (_refurbScan >= 0f && Time.unscaledTime - _refurbScan < 1f)
		{
			return;
		}
		_refurbScan = Time.unscaledTime;
		RefurbRows.Clear();
		List<GameItem> list = Services.RefurbishCandidates();
		_refurbCount = list.Count;
		foreach (GameItem item2 in list.Take(3))
		{
			string item;
			try
			{
				item = item2.GetDisplayName(false);
			}
			catch
			{
				item = item2.identifier;
			}
			int num = MeridianCo.ConditionOf(item2);
			RefurbRows.Add((item, MeridianCo.ConditionName(num), MeridianCo.ConditionName(num + 1), MeridianCo.RefurbishFee(item2)));
		}
	}

	private static void DrawMeridianForge(Rect r)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 66f), L.T("精锻熔炉每晚把高纯度的金属锭炼成钢胚；锻台用钢胚锻刀，淬火的水越干净，刀的成色越好。", "The precision forge turns high-purity ingots into billets overnight; the anvil forges blades from billets, and the cleaner the quench water, the better the finish."), theme.Wrap);
		float num = ((Rect)(ref r)).y + 76f;
		if (!MeridianCo.ForgeOpen && !MeridianCo.ForgeOffered)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 44f), L.En ? $"Rosen will come to teach you once cooperation reaches {60}." : $"合作度到 {60} 后，罗森会上门来教。", theme.Wrap);
			return;
		}
		if (!MeridianCo.ForgeOpen)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 44f), L.En ? $"Rosen already came by. The full kit is {MeridianCo.ForgeFee}₵: precision forge and anvil, plus 3 billets, delivered tomorrow." : $"罗森来教过了。整套工具 {MeridianCo.ForgeFee}₵：精锻熔炉、锻台，附 3 块钢胚，明天送到。", theme.Wrap);
			float num2 = ((Rect)(ref r)).yMax - 44f;
			GUI.Label(new Rect(((Rect)(ref r)).x, num2 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
			bool enabled = GUI.enabled;
			GUI.enabled = Cash >= MeridianCo.ForgeFee;
			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num2 + 2f, 160f, 36f), L.En ? $"Buy kit ({MeridianCo.ForgeFee}₵)" : $"补办（{MeridianCo.ForgeFee}₵）", theme.BtnHotStyle))
			{
				Toast(MeridianCo.MakeUpForge() ?? _company.FrontDeskDone);
			}
			GUI.enabled = enabled;
			return;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 20f), L.T("找罗森订。原价，明天到，不限量。", "Order through Rosen. List price, here tomorrow, no limit."), theme.Small);
		num += 28f;
		(string, string)[] forgeMachines = MeridianCo.ForgeMachines;
		Rect val = default(Rect);
		for (int i = 0; i < forgeMachines.Length; i++)
		{
			(string, string) tuple = forgeMachines[i];
			string item = tuple.Item1;
			string item2 = tuple.Item2;
			Brand.Entry entry = Brand.Info(item);
			int num3 = (int)Goods.ValueOf(item2);
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x, num, 96f, 96f);
			GUI.Box(val, GUIContent.none, theme.CellBox);
			if ((Object)(object)entry.Icon != (Object)null)
			{
				Ui.Image(new Rect(((Rect)(ref val)).x + 4f, ((Rect)(ref val)).y + 4f, 88f, 88f), entry.Icon);
			}
			GUI.Label(new Rect(((Rect)(ref val)).xMax + 16f, num + 14f, 260f, 26f), entry.Name, theme.Body);
			GUI.Label(new Rect(((Rect)(ref val)).xMax + 16f, num + 44f, 200f, 22f), $"{num3}₵", theme.Price);
			bool enabled2 = GUI.enabled;
			GUI.enabled = Cash >= num3;
			if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num + 30f, 160f, 36f), L.T("订一台", "Order one"), theme.BtnHotStyle))
			{
				Toast(MeridianCo.OrderForgeMachine(_company, item, num3) ?? _company.FrontDeskDone);
			}
			GUI.enabled = enabled2;
			num += 108f;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).yMax - 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
	}

	private static void DrawMeridianRefurb(Rect r)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		if (_company.Tier(state) < 2)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), L.En ? ("Opens at " + _company.TierNames[1] + ".") : (_company.TierNames[1] + "开放。"), theme.Wrap);
			return;
		}
		ScanRefurb();
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 44f), L.En ? $"Factory refurbishment, guaranteed: one condition grade up each time, up to “Excellent”. {60}₵ per Meridian gun, {120}₵ for other makes. Once every {MeridianCo.RefurbishGap} days, up to {3} guns each time, returned the next day." : $"厂家返修，保证成功，一次升一档，最高到“优秀”。子午线的枪每把 {60}₵，别家的 {120}₵。每 {MeridianCo.RefurbishGap} 天一次，每次最多 {3} 把，第二天送回。", theme.Wrap);
		float num = ((Rect)(ref r)).y + 54f;
		Ui.Fill(new Rect(((Rect)(ref r)).x, num, ((Rect)(ref r)).width, 26f), theme.Ink);
		GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 260f, 26f), L.T("枪", "Gun"), theme.InvBody);
		GUI.Label(new Rect(((Rect)(ref r)).x + 270f, num, 160f, 26f), L.T("成色", "Condition"), theme.InvCenter);
		GUI.Label(new Rect(((Rect)(ref r)).x + 430f, num, ((Rect)(ref r)).width - 430f, 26f), L.T("费用", "Fee"), theme.InvCenter);
		num += 30f;
		if (RefurbRows.Count == 0)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, ((Rect)(ref r)).width, 30f), L.T("没有能翻新的枪。", "No guns to refurbish."), theme.Small);
			num += 34f;
		}
		foreach (var refurbRow in RefurbRows)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, 260f, 32f), refurbRow.Name, theme.Body);
			GUI.Label(new Rect(((Rect)(ref r)).x + 270f, num, 160f, 32f), refurbRow.From + " → " + refurbRow.To, theme.Center);
			GUI.Label(new Rect(((Rect)(ref r)).x + 430f, num, ((Rect)(ref r)).width - 430f, 32f), $"{refurbRow.Fee}₵", theme.Price);
			Ui.Fill(new Rect(((Rect)(ref r)).x, num + 32f, ((Rect)(ref r)).width, 1f), theme.Line);
			num += 34f;
		}
		if (_refurbCount > RefurbRows.Count)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x + 8f, num, ((Rect)(ref r)).width, 22f), L.En ? $"{_refurbCount - RefurbRows.Count} more; send them next time." : $"另有 {_refurbCount - RefurbRows.Count} 把，下次再送。", theme.Small);
		}
		int num2 = Services.RefurbishNextDay(state);
		bool flag = Day >= num2;
		int num3 = RefurbRows.Sum(((string Name, string From, string To, int Fee) x) => x.Fee);
		float num4 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num4, 300f, 22f), (!flag) ? (L.En ? $"Next drop-off: day {num2}" : $"下次可送：第 {num2} 天") : (L.En ? $"{RefurbRows.Count} guns this time, {num3}₵" : $"本次 {RefurbRows.Count} 把，{num3}₵"), flag ? theme.Price : theme.Body);
		GUI.Label(new Rect(((Rect)(ref r)).x, num4 + 22f, 300f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = GUI.enabled;
		GUI.enabled = flag && RefurbRows.Count > 0 && Cash >= num3;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, num4 + 2f, 160f, 36f), L.T("送厂翻新", "Send for refurbishment"), theme.BtnHotStyle))
		{
			string text = Services.RequestRefurbish(_company);
			_refurbScan = -1f;
			if (text == null)
			{
				text = L.T("收到。明天送回。", "Received. Back tomorrow.");
			}
			Toast(text);
		}
		GUI.enabled = enabled;
	}

	private static void DrawPastel()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		_ = State;
		float width = ((Rect)(ref _rect)).width;
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector((width - 560f) / 2f, 10f, 560f, 90f);
		Banner(r);
		GUI.Label(new Rect(((Rect)(ref r)).x + 140f, ((Rect)(ref r)).y + 20f, 324f, 50f), _company.Name, theme.Title);
		GUI.Label(new Rect(24f, 106f, width - 80f, 22f), L.T("前台：", "Front desk: ") + _desk, theme.Body);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(10f, 136f, 168f, 470f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 6f, 150f, 22f), L.T("服务", "Services"), theme.Small);
		for (int i = 0; i < PastelModules.Length; i++)
		{
			string why;
			bool flag = PastelUnlocked(i, out why);
			Rect r2 = new Rect(((Rect)(ref val)).x + 6f, ((Rect)(ref val)).y + 30f + (float)(i * 32), ((Rect)(ref val)).width - 12f, 28f);
			string text = ((i == _module) ? "› " : "  ") + PastelModules[i] + (flag ? "" : L.T("（未解锁）", " (locked)"));
			if (Ui.Button(r2, text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(186f, 136f, width - 196f, 470f);
		if (!PastelUnlocked(_module, out var why2))
		{
			GUI.Label(new Rect(((Rect)(ref val2)).x + 10f, ((Rect)(ref val2)).y + 10f, ((Rect)(ref val2)).width - 20f, 24f), PastelModules[_module], theme.Title);
			GUI.Label(new Rect(((Rect)(ref val2)).x + 10f, ((Rect)(ref val2)).y + 50f, ((Rect)(ref val2)).width - 20f, 60f), why2, theme.Wrap);
		}
		else if (_module == 0)
		{
			DrawPastelOrder(val2);
		}
		else if (_module == 1)
		{
			DrawPastelDealer(val2);
		}
		else if (_module == 2)
		{
			DrawPastelTraffic(val2);
		}
		else if (!DrawExtraIfSelected(PastelBase.Length, val2))
		{
			DrawBrand(val2);
		}
		DrawPastelFooter(new Rect(10f, 616f, width - 20f, 170f));
	}

	private static bool PastelUnlocked(int module, out string why)
	{
		CompanyState state = State;
		int num = _company.Tier(state);
		why = null;
		if (module == 0 && num < 1)
		{
			why = L.T("签约后可以电话订货。和米洛多做几笔买卖吧。", "Sign with pastel to order by phone. Do a few more deals with Milo.");
		}
		if (module == 1 && num < 1)
		{
			why = L.T("签约后可以加盟成为帕斯泰尔的下层合作商。", "Sign with pastel to join as a Lower Levels dealer.");
		}
		if (module == 2 && !state.DealerJoined)
		{
			why = L.T("先成为下层合作商，帕斯泰尔才会把熟客介绍给你。", "Become a Lower Levels dealer first, and pastel will send its regulars your way.");
		}
		return why == null;
	}

	private static int PastelUnitPrice(CatalogEntry e)
	{
		double num = Company.OrderRate(_company, _company.Tier(State));
		if (e == PastelSpecial())
		{
			num *= 0.9;
		}
		return e.Quote(num);
	}

	private static CatalogEntry PastelSpecial()
	{
		int tier = _company.CatalogTier(State);
		CatalogEntry[] array = PastelCo.Entries.Where((CatalogEntry e) => e.Page == "日用" && e.MinTier <= tier).ToArray();
		if (array.Length != 0)
		{
			return array[Day * 31 % array.Length];
		}
		return null;
	}

	private static void DrawPastelOrder(Rect r)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		int num = _company.CatalogTier(state);
		bool flag = PastelCo.HemingwayOpen();
		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, 90f, 26f), L.T("日用", "Daily"), (_page == "日用") ? theme.BtnHotStyle : theme.BtnStyle))
		{
			_page = "日用";
		}
		if (Ui.Button(new Rect(((Rect)(ref r)).x + 96f, ((Rect)(ref r)).y, 140f, 26f), flag ? L.T("海明威", "Hemingway") : L.T("海明威（未解锁）", "Hemingway (locked)"), (_page == "海明威") ? theme.BtnHotStyle : theme.BtnStyle))
		{
			if (flag)
			{
				_page = "海明威";
			}
			else
			{
				Toast(L.T("米洛调去海明威之后才有这一页", "This page opens once Milo transfers to Hemingway"));
			}
		}
		bool flag2 = PastelCo.CraftDone(1);
		if (Ui.Button(new Rect(((Rect)(ref r)).x + 242f, ((Rect)(ref r)).y, 90f, 26f), flag2 ? L.T("工艺", "Craft") : "？？？", (_page == "工艺") ? theme.BtnHotStyle : theme.BtnStyle))
		{
			if (flag)
			{
				_page = "工艺";
			}
			else
			{
				Toast(L.T("米洛调去海明威之后才有这一页", "This page opens once Milo transfers to Hemingway"));
			}
		}
		if (_page == "海明威" || _page == "工艺")
		{
			Ui.Fill(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 32f, ((Rect)(ref r)).width, ((Rect)(ref r)).height - 32f), Tex.Hex(8730423));
			Ui.Fill(new Rect(((Rect)(ref r)).x + 3f, ((Rect)(ref r)).y + 35f, ((Rect)(ref r)).width - 6f, 1f), Tex.Hex(12224851));
			Ui.Fill(new Rect(((Rect)(ref r)).x + 3f, ((Rect)(ref r)).yMax - 4f, ((Rect)(ref r)).width - 6f, 1f), Tex.Hex(12224851));
		}
		bool flag3 = Services.CanOrderToday(_company);
		GUI.Label(new Rect(((Rect)(ref r)).x + 340f, ((Rect)(ref r)).y, ((Rect)(ref r)).width - 340f, 26f), flag3 ? L.T("今天下单，明天送到", "Order today, delivered tomorrow") : L.T("今天已经下过单了", "Already ordered today"), theme.Small);
		CatalogEntry[] array = PastelCo.Entries.Where((CatalogEntry e) => e.Page == _page).ToArray();
		CatalogEntry catalogEntry = PastelSpecial();
		int num2 = (array.Length + 2) / 3;
		float num3 = (((Rect)(ref r)).width - 18f) / 3f;
		float num4 = Math.Min(118f, (((Rect)(ref r)).height - 40f - (float)(6 * (num2 - 1))) / (float)Math.Max(1, num2));
		Rect val = default(Rect);
		for (int num5 = 0; num5 < array.Length; num5++)
		{
			CatalogEntry catalogEntry2 = array[num5];
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x + 4f + (float)(num5 % 3) * (num3 + 5f), ((Rect)(ref r)).y + 40f + (float)(num5 / 3) * (num4 + 6f), num3, num4);
			bool flag4 = catalogEntry2.MinTier > num;
			bool flag5 = !PastelCo.CraftUnlocked(catalogEntry2);
			GUI.Box(val, GUIContent.none, (catalogEntry2 == catalogEntry && !flag4) ? theme.CellOnBox : theme.CellBox);
			GUI.Label(new Rect(((Rect)(ref val)).x + 6f, ((Rect)(ref val)).y + 4f, ((Rect)(ref val)).width - 12f, 34f), flag5 ? "？？？" : catalogEntry2.Name(), theme.Wrap);
			if (flag5)
			{
				Cart.Remove(catalogEntry2);
				if (catalogEntry2.Id == "@packaging_machine" && PastelCo.BribePending)
				{
					PastelBribeCell(val);
				}
				else
				{
					GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 44f, ((Rect)(ref val)).width, 22f), "？？？", theme.Center);
				}
				continue;
			}
			if (flag4)
			{
				GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 44f, ((Rect)(ref val)).width, 22f), L.En ? ("Unlocks at " + _company.TierNames[catalogEntry2.MinTier - 1]) : (_company.TierNames[catalogEntry2.MinTier - 1] + "解锁"), theme.Center);
				continue;
			}
			int num6 = PastelUnitPrice(catalogEntry2);
			if (num6 <= 0)
			{
				GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 44f, ((Rect)(ref val)).width, 22f), L.T("暂不可用", "Unavailable"), theme.Center);
				Cart.Remove(catalogEntry2);
				continue;
			}
			GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 36f, ((Rect)(ref val)).width, 22f), ((catalogEntry2 == catalogEntry) ? L.T("特价 ", "Deal ") : "") + $"{num6}₵", theme.Price);
			int num7 = Math.Min(10, Services.Left(state, catalogEntry2));
			if (catalogEntry2.Limit() > 0)
			{
				GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + 54f, ((Rect)(ref val)).width, 16f), L.En ? $"{num7} left {Services.PeriodWord}" : $"{Services.PeriodWord}还剩 {num7}", theme.Small);
			}
			Cart.TryGetValue(catalogEntry2, out var value);
			value = Ui.Qty(new Rect(((Rect)(ref val)).x + 6f, ((Rect)(ref val)).yMax - 28f, ((Rect)(ref val)).width - 12f, 24f), value, Math.Min(10, num7), theme.BtnStyle, theme.Center);
			if (value > 0)
			{
				Cart[catalogEntry2] = value;
			}
			else
			{
				Cart.Remove(catalogEntry2);
			}
		}
	}

	private static void PastelBribeCell(Rect cell)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		GUI.Label(new Rect(((Rect)(ref cell)).x + 6f, ((Rect)(ref cell)).y + 34f, ((Rect)(ref cell)).width - 12f, 22f), L.T("打点主管", "Pay off the supervisor"), theme.Center);
		bool enabled = GUI.enabled;
		GUI.enabled = Cash >= PastelCo.Bribe;
		if (Ui.Button(new Rect(((Rect)(ref cell)).x + 8f, ((Rect)(ref cell)).yMax - 30f, ((Rect)(ref cell)).width - 16f, 26f), L.En ? $"Pay {PastelCo.Bribe}₵" : $"补交 {PastelCo.Bribe}₵", theme.BtnHotStyle))
		{
			Toast(PastelCo.PayBribe(deliver: true) ? L.T("分装机明天送到。", "The packaging machine arrives tomorrow.") : L.T("现金不够", "Not enough cash"));
		}
		GUI.enabled = enabled;
	}

	private static void DrawPastelDealer(Rect r)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层合作商", "Lower Dealer"), theme.Title);
		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 120f), L.T("成为帕斯泰尔的特约经销点。\n\n", "Become a pastel partner outlet.\n\n") + (L.En ? $"· Regular rebate: every {PastelCo.RebatePeriod} days, {0.1:P0} of what you sold in pastel products, paid back by the courier.\n" : $"· 定期返点：每 {PastelCo.RebatePeriod} 天，把这段时间你卖出的帕斯泰尔产品按售价的 {0.1:P0} 返现，由送货员带来。\n") + L.T("· 解锁“下层客流”：帕斯泰尔会把下层区的熟客介绍到你店里。", "· Unlocks “Lower Foot Traffic”: pastel sends Lower Levels regulars to your shop."), theme.Wrap);
		if (!state.DealerJoined)
		{
			if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, 220f, 34f), L.En ? $"Join (one-time {PastelCo.DealerFee}₵)" : $"加盟（一次性 {PastelCo.DealerFee}₵）", theme.BtnHotStyle))
			{
				Toast(Services.JoinDealer(_company) ?? L.T("欢迎加入帕斯泰尔！", "Welcome to pastel!"));
			}
		}
		else
		{
			int num = Day - state.DealerSinceDay;
			int num2 = PastelCo.RebatePeriod - num % PastelCo.RebatePeriod;
			GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 180f, ((Rect)(ref r)).width - 20f, 24f), L.En ? $"Dealer for {num} days. Sold {state.WeekSales:0}₵ of pastel products this period." : $"已加盟 {num} 天。本期已卖出 {state.WeekSales:0}₵ 的帕斯泰尔产品。", theme.Body);
			GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 208f, ((Rect)(ref r)).width - 20f, 24f), L.En ? $"Expected rebate {state.WeekSales * 0.1:0}₵, settles in {num2} days." : $"预计返点 {state.WeekSales * 0.1:0}₵，{num2} 天后结算。", theme.Body);
		}
	}

	private static void DrawPastelTraffic(Rect r)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 20f, 30f), L.T("下层客流", "Lower Foot Traffic"), theme.Title);
		GUI.Label(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 40f, ((Rect)(ref r)).width - 20f, 100f), (L.En ? $"{PastelCo.TrafficFee}₵ per night. pastel sends Lower Levels regulars over: the next day, 1–2 extra Lower Levels customers come looking for food and daily goods.\n\nPauses automatically when you are short on cash." : $"每晚 {PastelCo.TrafficFee}₵。帕斯泰尔把下层区的熟客介绍过来：第二天会额外来 1–2 位想买食物、日用品的下层区顾客。\n\n现金不够时自动暂停。") + ((Cash >= PastelCo.TrafficFee) ? "" : (L.En ? $"\n(You have {Cash}₵, not enough for tonight's fee.)" : $"\n（你现在的现金 {Cash}₵，不够今晚的费用。）")), theme.Wrap);
		if (Ui.Button(new Rect(((Rect)(ref r)).x + 10f, ((Rect)(ref r)).y + 160f, 180f, 34f), state.TrafficOn ? L.T("已开启（点击关闭）", "On (click to turn off)") : L.T("开启", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
		{
			state.TrafficOn = !state.TrafficOn;
			ModLog.Info("下层客流：" + (state.TrafficOn ? L.T("开启", "Turn on") : "关闭"));
			Toast(state.TrafficOn ? L.T("从今晚开始介绍熟客", "Regulars start coming from tonight") : L.T("已关闭", "Turned off"));
		}
	}

	private static void DrawPastelFooter(Rect r)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		GUI.Box(r, GUIContent.none, theme.CellBox);
		Texture2D val = Portraits.Texture(PastelCo.HemingwayOpen() ? "pastel_milo_hemingway" : "pastel_milo", "maleLower_0");
		Rect r2 = default(Rect);
		((Rect)(ref r2))._002Ector(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 6f, 130f, 95f);
		Ui.Fill(r2, Tex.Hex(13223594));
		if ((Object)(object)val != (Object)null)
		{
			Ui.Image(r2, val);
		}
		GUI.Label(new Rect(((Rect)(ref r)).x + 6f, ((Rect)(ref r)).y + 104f, 130f, 22f), L.T("米洛", "Milo"), theme.Center);
		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 10f, 300f, 22f), "“" + MiloLines[Day % MiloLines.Length] + "”", theme.Body);
		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 40f, 300f, 22f), L.En ? $"Cooperation {state.Coop}  ·  {TierLabel(state)}" : $"合作度 {state.Coop}  ·  {TierLabel(state)}", theme.Body);
		CoopBar(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 66f, 300f, 12f));
		GUI.Label(new Rect(((Rect)(ref r)).x + 144f, ((Rect)(ref r)).y + 82f, 300f, 20f), NextTierHint(), theme.Small);
		if (_module != 0 || !PastelUnlocked(0, out var _))
		{
			return;
		}
		int num = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => PastelUnitPrice(kv.Key) * kv.Value);
		GUI.Label(new Rect(((Rect)(ref r)).xMax - 180f, ((Rect)(ref r)).y + 12f, 170f, 22f), L.En ? $"Total {num}₵" : $"合计 {num}₵", theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).xMax - 180f, ((Rect)(ref r)).y + 36f, 170f, 22f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = num > 0 && num <= Cash && Services.CanOrderToday(_company);
		bool enabled2 = GUI.enabled;
		GUI.enabled = enabled;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 170f, ((Rect)(ref r)).y + 64f, 150f, 36f), L.T("下单", "Order"), theme.BtnHotStyle))
		{
			string text = Services.PlaceOrder(_company, Cart, PastelUnitPrice);
			if (text == null)
			{
				Cart.Clear();
				Toast(L.T("已下单！", "Ordered! ") + _company.FrontDeskDone);
			}
			else
			{
				Toast(text);
			}
		}
		GUI.enabled = enabled2;
	}

	private static int SolanaNeed(int i)
	{
		if (i >= SolanaModuleTier.Length)
		{
			return 0;
		}
		return SolanaModuleTier[i];
	}

	private static void DrawSolana()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		float width = ((Rect)(ref _rect)).width;
		float height = ((Rect)(ref _rect)).height;
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector((width - 560f) / 2f, 8f, 560f, 90f);
		if ((Object)(object)theme.Banner != (Object)null)
		{
			Ui.Image(r, theme.Banner);
		}
		else
		{
			Ui.Fill(new Rect(60f, 40f, width / 2f - 160f, 1f), theme.Line);
			Ui.Fill(new Rect(width / 2f + 100f, 40f, width / 2f - 160f, 1f), theme.Line);
		}
		GUIStyle bannerTitle = theme.BannerTitle;
		GUI.Label(new Rect(((Rect)(ref r)).x + 192f - 40f, ((Rect)(ref r)).y + 52f, 256f, 28f), _company.Name, bannerTitle);
		GUIStyle desk = theme.Desk;
		GUI.Label(new Rect(0f, 102f, width, 22f), _desk, desk);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(20f, 130f, 230f, height - 150f);
		Ui.Fill(new Rect(((Rect)(ref val)).xMax + 10f, ((Rect)(ref val)).y, 1f, ((Rect)(ref val)).height), theme.Line);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).x + 15f, ((Rect)(ref val)).y, 200f, 146f);
		GUI.Box(val2, GUIContent.none, theme.CellBox);
		Texture2D val3 = Portraits.Texture("solana_seline", "femaleUpper_1");
		if ((Object)(object)val3 != (Object)null)
		{
			Ui.Image(new Rect(((Rect)(ref val2)).x + 4f, ((Rect)(ref val2)).y + 4f, ((Rect)(ref val2)).width - 8f, ((Rect)(ref val2)).height - 8f), val3);
		}
		GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val2)).yMax + 4f, ((Rect)(ref val)).width, 20f), L.T("专属顾问 · 瑟琳", "Personal Advisor · Seline"), desk);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val2)).yMax + 30f, ((Rect)(ref val)).width - 20f, 22f), L.En ? ("Membership: " + TierLabel(state)) : ("会员：" + TierLabel(state)), theme.Body);
		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val2)).yMax + 56f, ((Rect)(ref val)).width - 20f, 8f));
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val2)).yMax + 66f, ((Rect)(ref val)).width - 20f, 20f), NextTierHint(), theme.Small);
		float num = ((Rect)(ref val2)).yMax + 96f;
		for (int i = 0; i < SolanaModules.Length; i++)
		{
			bool flag = _company.Tier(state) >= SolanaNeed(i);
			string text = ((i == _module) ? "· " : "  ") + SolanaModules[i] + (flag ? "" : (L.En ? (" (" + _company.TierNames[SolanaNeed(i) - 1] + ")") : ("（" + _company.TierNames[SolanaNeed(i) - 1] + "）")));
			if (Ui.Button(new Rect(((Rect)(ref val)).x + 6f, num + (float)(i * 30), ((Rect)(ref val)).width - 12f, 26f), text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
		}
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(((Rect)(ref val)).xMax + 30f, 130f, width - ((Rect)(ref val)).xMax - 50f, height - 150f);
		bool flag2 = _company.Tier(state) >= SolanaNeed(_module);
		GUI.Label(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y, ((Rect)(ref val4)).width, 30f), SolanaModules[_module], theme.Title);
		Ui.Fill(new Rect(((Rect)(ref val4)).x, ((Rect)(ref val4)).y + 34f, ((Rect)(ref val4)).width, 1f), theme.Line);
		Rect val5 = default(Rect);
		((Rect)(ref val5))._002Ector(((Rect)(ref val4)).x, ((Rect)(ref val4)).y + 46f, ((Rect)(ref val4)).width, ((Rect)(ref val4)).height - 46f);
		if (DrawExtraIfSelected(SolanaBase.Length, val5))
		{
			return;
		}
		if (!flag2)
		{
			GUI.Label(new Rect(((Rect)(ref val5)).x, ((Rect)(ref val5)).y, ((Rect)(ref val5)).width, 60f), L.En ? ("Exclusive to " + _company.TierNames[SolanaModuleTier[_module] - 1] + "s.") : (_company.TierNames[SolanaModuleTier[_module] - 1] + "专享。"), theme.Wrap);
			return;
		}
		switch (_module)
		{
		case 0:
			DrawSolanaVip(val5);
			break;
		case 1:
			DrawSolanaLab(val5);
			break;
		case 2:
			DrawSolanaGift(val5);
			break;
		default:
			DrawBrand(val5);
			break;
		}
	}

	private static void DrawSolanaVip(Rect r)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), L.En ? $"Solana introduces you to Upper Levels clientele: the day after you book, a Solana VIP with a budget of {SolanaCo.VipBudgetText()} visits your shop, looking for luxury goods, cigarettes and alcohol.\n\n{SolanaCo.VipFee}₵ per booking, paid when you book; one booking every {SolanaCo.VipGap} days." : $"索拉纳为您引荐上层区的客人：预约后第二天会有一位预算在 {SolanaCo.VipBudgetText()} 的索拉纳贵宾来店，想买奢侈品和烟酒。\n\n每次 {SolanaCo.VipFee}₵，预约时支付；每 {SolanaCo.VipGap} 天可预约一次。", theme.Wrap);
		int num = Services.VipNextDay(state);
		bool flag = Day >= num;
		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 120f, 240f, 34f), (!flag) ? (L.En ? $"Available again on day {num}" : $"第 {num} 天可再预约") : (L.En ? $"Book a VIP ({SolanaCo.VipFee}₵)" : $"预约贵宾（{SolanaCo.VipFee}₵）"), flag ? theme.BtnHotStyle : theme.BtnStyle) && flag)
		{
			Toast(Services.BookVip(_company) ?? L.T("已为您安排，明天专人引荐。", "Arranged. Your VIP arrives tomorrow."));
		}
	}

	private static void DrawSolanaLab(Rect r)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		List<string> list = Services.LabListThisWeek(state);
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), L.En ? $"The lab needs the items below (a new list every {SolanaCo.LabPeriod} days). Book a buyer and they will come by the next day to purchase." : $"实验室这一期需要以下物品（每 {SolanaCo.LabPeriod} 天换一张单子）。预约后，采购员第二天上门收购。", theme.Wrap);
		float num = (((Rect)(ref r)).width - 20f) / 3f;
		Rect val = default(Rect);
		for (int i = 0; i < list.Count; i++)
		{
			((Rect)(ref val))._002Ector(((Rect)(ref r)).x + (float)i * (num + 10f), ((Rect)(ref r)).y + 50f, num, 130f);
			GUI.Box(val, GUIContent.none, theme.CellBox);
			string text = ItemName(list[i]);
			GUI.Label(new Rect(((Rect)(ref val)).x + 8f, ((Rect)(ref val)).y + 10f, ((Rect)(ref val)).width - 16f, 60f), text, theme.Wrap);
			GUI.Label(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).yMax - 40f, ((Rect)(ref val)).width, 24f), L.En ? $"× {SolanaCo.LabMultiplier(list[i]):0.#}" : $"× {SolanaCo.LabMultiplier(list[i]):0.#} 倍", theme.Price);
		}
		if (state.LabBooked)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 200f, ((Rect)(ref r)).width, 24f), L.T("这一期已预约。", "Booked for this list."), theme.Body);
		}
		else if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 200f, 200f, 34f), L.T("预约采购员", "Book a buyer"), theme.BtnHotStyle))
		{
			Toast(Services.BookLab(_company) ?? _company.FrontDeskDone);
		}
	}

	private static void DrawSolanaGift(Rect r)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 60f), L.En ? $"Black Gold Members may claim one bottle of SYNC Neuroactive Perfume every {SolanaCo.GiftPeriod} days, hand-delivered the next day." : $"黑金会员每 {SolanaCo.GiftPeriod} 天可领取一瓶“同步”神经活性香水，次日专人送达。", theme.Wrap);
		bool flag = Services.GiftReady(state);
		bool enabled = GUI.enabled;
		GUI.enabled = flag;
		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 80f, 200f, 34f), flag ? L.T("领取礼遇", "Claim gift") : L.T("这一期已领取", "Claimed for this period"), theme.BtnHotStyle))
		{
			Toast(Services.ClaimGift(_company) ?? _company.FrontDeskDone);
		}
		GUI.enabled = enabled;
	}

	private static string SpacerTitle(int i)
	{
		return SpacerServices[i];
	}

	private static void DrawSpacer()
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		float width = ((Rect)(ref _rect)).width;
		float height = ((Rect)(ref _rect)).height;
		Rect r = default(Rect);
		((Rect)(ref r))._002Ector(20f, 8f, 560f, 90f);
		if ((Object)(object)theme.Banner != (Object)null)
		{
			Ui.Image(r, theme.Banner);
			GUI.Label(new Rect(((Rect)(ref r)).x + 120f, ((Rect)(ref r)).y + 16f, 420f, 58f), _company.Name, theme.Title);
		}
		else
		{
			Ui.Fill(new Rect(20f, 20f, 420f, 56f), theme.Ink);
			GUI.Label(new Rect(36f, 20f, 400f, 56f), _company.Name, theme.InvTitle);
		}
		GUI.Label(new Rect(600f, 40f, width - 640f, 22f), L.En ? $"{_company.PhoneNumber}   Day {Day}" : $"{_company.PhoneNumber}\u3000第 {Day} 天", theme.Small);
		Ui.Fill(new Rect(20f, 100f, width - 40f, 2f), theme.Ink);
		GUI.Label(new Rect(24f, 106f, width - 48f, 22f), L.T("导师：", "The Mentor: ") + _desk, theme.Body);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(20f, 136f, 190f, height - 156f);
		GUI.Box(val, GUIContent.none, theme.CellBox);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 8f, 170f, 22f), L.En ? ("Node: " + TierLabel(state)) : ("节点：" + TierLabel(state)), theme.Body);
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 32f, 170f, 20f), L.En ? $"Trust {state.Coop}" : $"信任度 {state.Coop}", theme.Small);
		CoopBar(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 54f, 170f, 8f));
		GUI.Label(new Rect(((Rect)(ref val)).x + 10f, ((Rect)(ref val)).y + 64f, 170f, 20f), NextTierHint(), theme.Small);
		float num = ((Rect)(ref val)).y + 96f;
		for (int i = 0; i < SpacerModuleCount; i++)
		{
			string text = ((i == _module) ? "› " : "  ") + SpacerTitle(i);
			if (Ui.Button(new Rect(((Rect)(ref val)).x + 6f, num, ((Rect)(ref val)).width - 12f, 28f), text, (i == _module) ? theme.NavOnStyle : theme.NavStyle))
			{
				_module = i;
			}
			num += 32f;
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(((Rect)(ref val)).xMax + 20f, 136f, width - ((Rect)(ref val)).xMax - 40f, height - 156f);
		GUI.Label(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y, ((Rect)(ref val2)).width, 30f), SpacerTitle(_module), theme.HotTitle);
		Ui.Fill(new Rect(((Rect)(ref val2)).x, ((Rect)(ref val2)).y + 34f, ((Rect)(ref val2)).width, 1f), theme.Ink);
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(((Rect)(ref val2)).x, ((Rect)(ref val2)).y + 44f, ((Rect)(ref val2)).width, ((Rect)(ref val2)).height - 44f);
		if (!DrawExtraIfSelected(SpacerPages.Length, val3))
		{
			if (_company.Tier(state) < 1)
			{
				GUI.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 40f), L.T("没人认识你。", "Nobody knows you."), theme.Wrap);
			}
			else if (SpacerPages[_module] == "阵线" && _company.Tier(state) < 2)
			{
				GUI.Label(new Rect(((Rect)(ref val3)).x, ((Rect)(ref val3)).y, ((Rect)(ref val3)).width, 40f), L.En ? ("Only at " + _company.TierNames[1] + ".") : (_company.TierNames[1] + "才有。"), theme.Wrap);
			}
			else if (SpacerPages[_module] == "目录")
			{
				DrawBrand(val3);
			}
			else if (SpacerPages[_module] != null)
			{
				DrawSpacerOrder(val3, SpacerPages[_module]);
			}
			else
			{
				DrawSpacerMutual(val3);
			}
		}
	}

	private static int SpacerUnitPrice(CatalogEntry e)
	{
		return e.Quote((e.Id == "@modbox") ? 1.0 : Company.OrderRate(_company, _company.Tier(State)));
	}

	private static void DrawSpacerOrder(Rect r, string page)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		int num = _company.CatalogTier(State);
		bool flag = Services.CanOrderToday(_company);
		string text = page switch
		{
			"阵线" => L.T("几家一起出的。", "Made by several outfits together."), 
			"药房" => L.T("没有许可证。", "No license."), 
			"军械" => L.T("成色不保证。", "Condition not guaranteed."), 
			_ => L.T("货船余货，量大。", "Freighter surplus. Plenty of it."), 
		};
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 20f), (flag ? L.T("今天下单，明天到。", "Order today, here tomorrow. ") : L.T("今天已经下过单了。", "Already ordered today. ")) + text, theme.Small);
		float x = ((Rect)(ref r)).x;
		float num2 = ((Rect)(ref r)).x + 250f;
		float num3 = ((Rect)(ref r)).x + 340f;
		float num4 = ((Rect)(ref r)).x + 460f;
		float num5 = ((Rect)(ref r)).y + 26f;
		Ui.Fill(new Rect(((Rect)(ref r)).x, num5, ((Rect)(ref r)).width, 24f), theme.Accent2);
		GUI.Label(new Rect(x + 8f, num5, 240f, 24f), L.T("品名", "Item"), theme.InvBody);
		GUI.Label(new Rect(num2, num5, 90f, 24f), L.T("单价", "Unit price"), theme.InvCenter);
		GUI.Label(new Rect(num3, num5, 120f, 24f), L.T("数量", "Qty"), theme.InvCenter);
		GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 24f), L.T("小计", "Subtotal"), theme.InvCenter);
		num5 += 28f;
		IEnumerable<CatalogEntry> source = ((_company is RebelCo) ? RebelCo.Entries : SpacerCo.PageEntries(page));
		List<CatalogEntry> list = source.ToList();
		float num6 = 52 + ((page == "阵线") ? 176 : 0);
		int num7 = Math.Max(1, (int)((((Rect)(ref r)).yMax - num6 - num5) / 33f));
		if (list.Count > num7)
		{
			num7 = Math.Max(1, (int)((((Rect)(ref r)).yMax - num6 - 40f - num5) / 33f));
		}
		int num8 = (list.Count + num7 - 1) / num7;
		if (_orderPageKey != _company.Id + page)
		{
			_orderPageKey = _company.Id + page;
			_orderPage = 0;
		}
		_orderPage = Math.Max(0, Math.Min(_orderPage, num8 - 1));
		if (num8 > 1)
		{
			float num9 = ((Rect)(ref r)).xMax - 150f;
			float num10 = ((Rect)(ref r)).yMax - 84f;
			if (Ui.Button(new Rect(num9, num10, 34f, 30f), "‹", theme.BtnStyle) && _orderPage > 0)
			{
				_orderPage--;
			}
			GUI.Label(new Rect(num9 + 38f, num10, 70f, 30f), $"{_orderPage + 1} / {num8}", theme.Center);
			if (Ui.Button(new Rect(num9 + 112f, num10, 34f, 30f), "›", theme.BtnStyle) && _orderPage < num8 - 1)
			{
				_orderPage++;
			}
		}
		foreach (CatalogEntry item in list.Skip(_orderPage * num7).Take(num7))
		{
			Ui.Fill(new Rect(((Rect)(ref r)).x, num5 + 31f, ((Rect)(ref r)).width, 1f), theme.Line);
			GUI.Label(new Rect(x + 8f, num5, 240f, 32f), item.Name() + Services.LeftText(State, item), theme.Body);
			if (item.MinTier > num)
			{
				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 32f), L.En ? ("At " + _company.TierNames[item.MinTier - 1]) : (_company.TierNames[item.MinTier - 1] + "才有"), theme.Small);
				Cart.Remove(item);
				num5 += 33f;
				continue;
			}
			int num11 = SpacerUnitPrice(item);
			if (num11 <= 0)
			{
				GUI.Label(new Rect(num2, num5, ((Rect)(ref r)).xMax - num2, 32f), L.T("暂不可用", "Unavailable"), theme.Small);
				Cart.Remove(item);
				num5 += 33f;
				continue;
			}
			GUI.Label(new Rect(num2, num5, 90f, 32f), $"{num11}₵", theme.Price);
			Cart.TryGetValue(item, out var value);
			value = Ui.Qty(new Rect(num3, num5, 120f, 32f), value, Math.Min(10, Services.Left(State, item)), theme.BtnStyle, theme.Center);
			if (value > 0)
			{
				Cart[item] = value;
			}
			else
			{
				Cart.Remove(item);
			}
			GUI.Label(new Rect(num4, num5, ((Rect)(ref r)).xMax - num4, 32f), (value > 0) ? $"{num11 * value}₵" : "—", theme.Center);
			num5 += 33f;
		}
		if (page == "阵线")
		{
			DrawSpacerCredits(new Rect(((Rect)(ref r)).x, num5 + 16f, ((Rect)(ref r)).width, 160f));
		}
		int num12 = Cart.Sum((KeyValuePair<CatalogEntry, int> kv) => SpacerUnitPrice(kv.Key) * kv.Value);
		float num13 = ((Rect)(ref r)).yMax - 44f;
		GUI.Label(new Rect(((Rect)(ref r)).x, num13, 260f, 22f), L.En ? $"Total {num12}₵ (all pages)" : $"合计 {num12}₵（各页合计）", theme.Price);
		GUI.Label(new Rect(((Rect)(ref r)).x, num13 + 22f, 200f, 20f), L.En ? $"Cash {Cash}₵" : $"现金 {Cash}₵", theme.Small);
		bool enabled = num12 > 0 && num12 <= Cash && flag;
		bool enabled2 = GUI.enabled;
		GUI.enabled = enabled;
		if (Ui.Button(new Rect(((Rect)(ref r)).xMax - 150f, num13 + 2f, 140f, 36f), L.T("下单", "Order"), theme.BtnHotStyle))
		{
			string text2 = Services.PlaceOrder(_company, Cart, SpacerUnitPrice);
			if (text2 == null)
			{
				Cart.Clear();
				Toast(_company.FrontDeskDone);
			}
			else
			{
				Toast(text2);
			}
		}
		GUI.enabled = enabled2;
	}

	private static void DrawSpacerCredits(Rect r)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		GUI.Box(r, GUIContent.none, theme.CellBox);
		GUI.Label(new Rect(((Rect)(ref r)).x + 12f, ((Rect)(ref r)).y + 4f, ((Rect)(ref r)).width - 24f, 22f), L.T("技术来源", "Tech credits"), theme.Small);
		float num = ((Rect)(ref r)).y + 28f;
		(string, string)[] techCredits = SpacerCo.TechCredits;
		for (int i = 0; i < techCredits.Length; i++)
		{
			var (text, text2) = techCredits[i];
			GUI.Label(new Rect(((Rect)(ref r)).x + 16f, num, 180f, 24f), text, theme.Body);
			GUI.Label(new Rect(((Rect)(ref r)).x + 200f, num, ((Rect)(ref r)).width - 216f, 24f), text2, theme.Body);
			num += 26f;
		}
	}

	private static void DrawSpacerMutual(Rect r)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		Theme theme = _theme;
		CompanyState state = State;
		if (_company.Tier(state) < 2)
		{
			GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 40f), L.En ? ("Only at " + _company.TierNames[1] + ".") : (_company.TierNames[1] + "才有。"), theme.Wrap);
			return;
		}
		GUI.Label(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y, ((Rect)(ref r)).width, 90f), L.En ? $"Spacers at the docks are short on food, water and medicine. With this on, the Alliance points people your way every night: the next day, 1–2 extra Spacers come by to buy whatever they need.\n\n{SpacerCo.MutualFee}₵ per night. Stops automatically when you are short on cash." : $"码头上的星际客缺吃、缺水、缺药。开着这个，联盟每晚把人往你这儿指：第二天多来 1–2 位星际客，缺什么买什么。\n\n每晚 {SpacerCo.MutualFee}₵。现金不够时自动停。", theme.Wrap);
		if (Ui.Button(new Rect(((Rect)(ref r)).x, ((Rect)(ref r)).y + 110f, 200f, 34f), state.TrafficOn ? L.T("开着（点击关掉）", "On (click to turn off)") : L.T("开", "Turn on"), state.TrafficOn ? theme.BtnStyle : theme.BtnHotStyle))
		{
			state.TrafficOn = !state.TrafficOn;
			ModLog.Info("街坊互助：" + (state.TrafficOn ? "开启" : "关闭"));
			Toast(state.TrafficOn ? L.T("今晚开始。", "Starting tonight.") : L.T("停了。", "Stopped."));
		}
	}
}
