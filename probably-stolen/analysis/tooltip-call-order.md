# Tooltip call-order trace

## SpaceAsteroidTrigger.cs

~~~csharp
00023: 		if (mineralLevel == 1)
00024: 		{
00025: 			text = "rich";
00026: 		}
00027: 		if (mineralLevel == 2)
00028: 		{
00029: 			text = "very rich";
00030: 		}
00031: 		int timeMined = 0;
00032: 		eventData.text.AddLine("\r\nScanner indicates that it is " + text + " in mineral content.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00033: 		eventData.text.AddLine("\r\nYour ship has a total mining power of " + GameMaster.current.shipManager.GetShipMiningCapacity(), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00034: 		eventData.startAction = delegate
00035: 		{
00036: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_ASTEROID_EVENT_STARTED");
00037: 			tag.SetInt(tag.GetInt() + 1);
00038: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00039: 			{
00040: 				overworldSpace.isEventTriggered = true;
00041: 			}
00042: 			NavDiskHelper.HandleNavDiskOnArrivalToPoi();
00043: 		};
00044: 		GameItem container = DirectoryMaster.Item("lootbox_medium");
00045: 		bool canCheckLootbag = false;
00046: 		eventData.AddChoice(DirectoryMaster.Item("mine"), "(End Turn) Mine the asteroid", "", null, null, delegate
00047: 		{
00048: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew has spent some time mining the asteroid."));
00049: 			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
00050: 			{
00051: 				GameMaster.current.turnManager.NextTurn();
00052: 			}, null, delegate
00053: 			{
00054: 				AudioManager.Instance.Play("MINING_DRILL");
00055: 			});
00056: 			var (num, normalOre, rareOre) = MiningHelper.MineAsteroid(timeMined, mineralLevel, GameMaster.current.shipManager.GetShipMiningCapacity());
00057: 			if (num > 0)
00058: 			{
00059: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("An accident has occurs during mining!", RenderHandler.ColorPalette.LightRed));
00060: 				SpaceBattleHelper.TakingHit(num);
00061: 			}
00062: 			MiningHelper.AddMinedOre(container, normalOre, rareOre);
00063: 			timeMined++;
00064: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("We have mined the asteroid " + timeMined + " time", RenderHandler.ColorPalette.LightYellow));
00065: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("With the debris floating around from the mining operation there is a " + MiningHelper.GetDangerLevel(timeMined), RenderHandler.ColorPalette.LightYellow));
00066: 			OverlayHandler.current.ShowInventory(container);
00067: 			OverlayHandler.current.ShowEvent();
00068: 			canCheckLootbag = true;
00069: 		});
00070: 		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Mined ore.", "", () => canCheckLootbag, null, delegate
00071: 		{
00072: 			OverlayHandler.current.ShowInventory(container);
00073: 		});
00074: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing For now.", "", null, null, delegate
00075: 		{
00076: 		});
00077: 		return eventData;
00078: 	}
00079: }
~~~

## SpaceSpaTrigger.cs

~~~csharp
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData e = new EventData().SetText("Friendly Spa Ship", "You have arrived at a leisure ship. \r\nYou are greeted over the comms by an over-friendly attendant whom inform you that they are operating a spa ship and that their package start at 100 Energy Credit. The attendant mentions health benefits with the usage of the spa.");
00016: 		e.SetImage("spa");
00017: 		e.text.AddLine("\r\nYou have a total of " + GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() + " Energy Credit.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00018: 		e.text.AddLine("\r\nScanner shows that the spa ship is unarmed", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00019: 		e.startAction = delegate
00020: 		{
00021: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_SPA_EVENT_STARTED");
00022: 			tag.SetInt(tag.GetInt() + 1);
00023: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00024: 			{
00025: 				overworldSpace.isEventTriggered = true;
00026: 			}
00027: 		};
00028: 		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
00029: 		GameItem gameItem;
00030: 		e.AddChoice(gameItem = DirectoryMaster.Item("$"), "(100 Credit) Pay for the standard spa package for the crew.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 100, null, delegate
00031: 		{
00032: 			GameMaster.current.shipManager.RemoveEnergyCredit(100);
00033: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew spent sometime in the spa and returned relaxed and ready.", RenderHandler.ColorPalette.LightGreen));
00034: 			foreach (GameCharacterItem item in GameMaster.current.shipManager.GetCrew())
00035: 			{
00036: 				item.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_heal"), null);
00037: 				item.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_acc"), null);
00038: 				item.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_cc"), null);
00039: 			}
00040: 			e.ForceEnd();
00041: 		});
00042: 		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
00043: 		e.AddChoice(DirectoryMaster.Item("$$"), "(150 Credit) Pay for the premium spa package for the crew.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 150, null, delegate
00044: 		{
00045: 			GameMaster.current.shipManager.RemoveEnergyCredit(150);
00046: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew spent some quality time in the spa and returned focused and ready.", RenderHandler.ColorPalette.LightGreen));
00047: 			foreach (GameCharacterItem item2 in GameMaster.current.shipManager.GetCrew())
00048: 			{
00049: 				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_heal2"), null);
00050: 				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_acc"), null);
00051: 				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_cc"), null);
00052: 				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_dodge"), null);
00053: 				item2.AppliedCombatEffect(DirectoryMaster.CombatAbilityEffect("spa_max_hp"), null);
00054: 			}
00055: 			e.ForceEnd();
00056: 		});
00057: 		e.AddChoice(DirectoryMaster.Item("rob"), "(Hostile) Rob the spa ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00058: 		{
00059: 			e.ForceEnd();
00060: 		});
00061: 		e.AddChoice(DirectoryMaster.Item("open_fire"), "(Hostile) Open fire on the spa ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00062: 		{
00063: 			e.ForceEnd();
00064: 		});
00065: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00066: 		{
00067: 		});
~~~

## CraftingHelper.cs

~~~csharp
00066: 			craftingRecipeManager = new CraftingRecipeManager();
00067: 			item.recipeManager = craftingRecipeManager;
00068: 		}
00069: 		else
00070: 		{
00071: 			if (!(item.recipeManager is CraftingRecipeManager craftingRecipeManager2))
00072: 			{
00073: 				return null;
00074: 			}
00075: 			craftingRecipeManager = craftingRecipeManager2;
00076: 		}
00077: 		Action updateRecipeDescription = delegate
00078: 		{
00079: 			List<GameItem> allItems = GraphUtils.FindAllChildrenType<GameItem>(inputInventory);
00080: 			CraftingRecipe craftingRecipe = selectedRecipe;
00081: 			if (craftingRecipe != null && craftingRecipe.CanCraft(allItems))
00082: 			{
00083: 				recipeCraftButton.Enable();
00084: 			}
00085: 			else
00086: 			{
00087: 				recipeCraftButton.Disable();
00088: 			}
00089: 			if (selectedItem != null && selectedRecipe != null)
00090: 			{
00091: 				RichTextBuilder richTextBuilder = new RichTextBuilder();
00092: 				richTextBuilder.AddLine(selectedItem.GetDisplayName(), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
00093: 				richTextBuilder.Add(selectedRecipe.GetCraftingDetails(allItems));
00094: 				recipeDescription.SetRichText(richTextBuilder);
00095: 				outputSlot.SetBackgroundFadeSprite(selectedItem.spriteAtlasPath, selectedItem.spritePath);
00096: 			}
00097: 			else
00098: 			{
00099: 				outputSlot.ClearBackgroundSprite();
00100: 			}
00101: 		};
00102: 		machineryButton.SetCallback(delegate
00103: 		{
00104: 			List<CraftingRecipe> list = item.FindAllRecipes();
00105: 			Dictionary<GameItem, CraftingRecipe> recipeDict = new Dictionary<GameItem, CraftingRecipe>();
00106: 			foreach (CraftingRecipe recipe in list)
00107: 			{
00108: 				recipe.CraftWithoutMaterials().ForEach(delegate(GameItem key)
00109: 				{
00110: 					recipeDict.Add(key, recipe);
00111: 				});
00112: 			}
00113: 			foreach (var (gameItem2, craftingRecipe2) in recipeDict)
00114: 			{
00115: 				gameItem2.SetAllMayFalse();
00116: 				gameItem2.EnableTag("CHOICE_PREVIEW_TAG");
00117: 				if (craftingRecipe2.customName != null)
00118: 				{
00119: 					gameItem2.name = craftingRecipe2.customName;
00120: 				}
00121: 				else
00122: 				{
00123: 					gameItem2.name = "Craft: " + gameItem2.GetDisplayName();
00124: 					if (gameItem2.unitCount > 1)
00125: 					{
~~~

~~~csharp
00183: 			vendingRecipeManager = new VendingRecipeManager();
00184: 			item.recipeManager = vendingRecipeManager;
00185: 		}
00186: 		else
00187: 		{
00188: 			if (!(item.recipeManager is VendingRecipeManager vendingRecipeManager2))
00189: 			{
00190: 				return null;
00191: 			}
00192: 			vendingRecipeManager = vendingRecipeManager2;
00193: 		}
00194: 		Action updateRecipeDescription = delegate
00195: 		{
00196: 			List<GameItem> allItems = GraphUtils.FindAllChildrenType<GameItem>(inputInventory);
00197: 			CraftingRecipe craftingRecipe = selectedRecipe;
00198: 			if (craftingRecipe != null && craftingRecipe.CanCraft(allItems))
00199: 			{
00200: 				recipeCraftButton.Enable();
00201: 			}
00202: 			else
00203: 			{
00204: 				recipeCraftButton.Disable();
00205: 			}
00206: 			if (selectedItem != null && selectedRecipe != null)
00207: 			{
00208: 				RichTextBuilder richTextBuilder = new RichTextBuilder();
00209: 				richTextBuilder.AddLine(selectedItem.GetDisplayName(), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
00210: 				richTextBuilder.Add(selectedRecipe.GetVendingDetails(allItems));
00211: 				recipeDescription.SetRichText(richTextBuilder);
00212: 				outputSlot.SetBackgroundFadeSprite(selectedItem.spriteAtlasPath, selectedItem.spritePath);
00213: 			}
00214: 			else
00215: 			{
00216: 				outputSlot.ClearBackgroundSprite();
00217: 			}
00218: 		};
00219: 		machineryButton.SetCallback(delegate
00220: 		{
00221: 			List<CraftingRecipe> list = item.FindAllRecipes();
00222: 			Dictionary<GameItem, CraftingRecipe> recipeDict = new Dictionary<GameItem, CraftingRecipe>();
00223: 			foreach (CraftingRecipe recipe in list)
00224: 			{
00225: 				recipe.CraftWithoutMaterials().ForEach(delegate(GameItem key)
00226: 				{
00227: 					recipeDict.Add(key, recipe);
00228: 				});
00229: 			}
00230: 			foreach (var (gameItem2, craftingRecipe2) in recipeDict)
00231: 			{
00232: 				gameItem2.SetAllMayFalse();
00233: 				gameItem2.EnableTag("CHOICE_PREVIEW_TAG");
00234: 				if (craftingRecipe2.customName != null)
00235: 				{
00236: 					gameItem2.name = craftingRecipe2.customName;
00237: 				}
00238: 				else
00239: 				{
00240: 					gameItem2.name = "Vend: " + gameItem2.GetDisplayName();
00241: 					if (gameItem2.unitCount > 1)
00242: 					{
~~~

## GameItemElement.cs

~~~csharp
01153: 			return (Object)(object)handler == (Object)null;
01154: 		}
01155: 		return true;
01156: 	}
01157: 
01158: 	public bool HasTooltip()
01159: 	{
01160: 		if (IsTag("IS_TOOLTIP_DISABLED_TAG"))
01161: 		{
01162: 			return false;
01163: 		}
01164: 		return true;
01165: 	}
01166: 
01167: 	private RichTextBuilder GetTooltipBasic()
01168: 	{
01169: 		int num = PlantGeneticsHelper.GetTooltipWidth(this, 40);
01170: 		if (IsTag("MACHINE_MANUAL_TAG"))
01171: 		{
01172: 			num = (int)((float)num * 2f);
01173: 		}
01174: 		if (IsTag("FEED_DISPENSER_TAG"))
01175: 		{
01176: 			num = (int)((float)num * 1.5f);
01177: 		}
01178: 		RichTextBuilder richTextBuilder = new RichTextBuilder(num);
01179: 		GeneralHelper.CreateGeneralItemTooltip(richTextBuilder, this);
01180: 		if (GeneralHelper.HasCustomName(this))
01181: 		{
01182: 			richTextBuilder.AddLine(GeneralHelper.GetCustomName(this) + " (" + GetDisplayName() + ")", ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
01183: 		}
01184: 		else
01185: 		{
01186: 			richTextBuilder.AddLine(GetDisplayName(), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
01187: 		}
01188: 		if (IsTag("manufacturer"))
01189: 		{
01190: 			DisplayManufacturer(richTextBuilder, GetTagReadonly("manufacturer").GetString());
01191: 		}
01192: 		if (base.forceDisableActivate || base.forceDisableUse)
01193: 		{
01194: 			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_disabled"), ignoreEmpty: true, RenderHandler.ColorPalette.Red, bold: true, italic: false, underline: true);
01195: 		}
01196: 		StolenHelper.CreateStolenItemTooltip(richTextBuilder, this);
01197: 		ItemPinHelper.CreatePinnedTooltip(richTextBuilder, this);
01198: 		CommonTooltipHelper.CreateItemTypeTooltip(richTextBuilder, this);
01199: 		ToggleAppearanceHelper.CreateToggleAppearanceTooltip(richTextBuilder, this);
01200: 		ContrabandHelper.CreateContrabandItemTooltip(richTextBuilder, this);
01201: 		if (base.parentInventory != null && base.parentInventory.IsInsertLocked() && base.parentInventory.IsRemoveLocked())
01202: 		{
01203: 			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_locked_machine"), ignoreEmpty: true, RenderHandler.ColorPalette.Red);
01204: 		}
01205: 		richTextBuilder.AddEmptyLine(14);
01206: 		if (PlayerStore.Instance.devMode)
01207: 		{
01208: 			if (IsTag("ITEM_HIDDEN_TAG"))
01209: 			{
01210: 				richTextBuilder.Add(LocHelper.GetLocalizedMechanic("mech_tooltip_hidden_dev"), RenderHandler.ColorPalette.Black);
01211: 			}
01212: 			if (IsTag("IMPORTANT_TAG"))
~~~

~~~csharp
01342: 		if (!base.flavorText.Equals(""))
01343: 		{
01344: 			richTextBuilder.AddLine(" " + base.flavorText, ignoreEmpty: true, RenderHandler.ColorPalette.Gray, bold: false, italic: true);
01345: 		}
01346: 		ContainerHelper.CreateContainerTooltip(richTextBuilder, this);
01347: 		ScratcherHelper.CreateScratcherTooltip(richTextBuilder, this);
01348: 		LotteryHelper.CreateLotteryRedeemTooltip(richTextBuilder, this);
01349: 		if (base.identifier.Equals("food_stamp"))
01350: 		{
01351: 			richTextBuilder.AddLine(LocHelper.GetLocalizedMechanic("mech_tooltip_food_stamp"), ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
01352: 		}
01353: 		if (IsTag("IS_ITEM_TOGGLE_ON_TAG"))
01354: 		{
01355: 			richTextBuilder.AddLine(LocHelper.GetLocalizedUI("ui_rep_active"), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
01356: 		}
01357: 		return richTextBuilder;
01358: 	}
01359: 
01360: 	public virtual RichText GetTooltip()
01361: 	{
01362: 		RichTextBuilder richTextBuilder = GetTooltipBasic();
01363: 		if (base.parentInventory != null && base.tooltipRichTextFunc != null)
01364: 		{
01365: 			richTextBuilder = base.tooltipRichTextFunc(this, base.parentInventory, base.parentInventory.GetIncompleteItemInventorySlot(this), base.modifiedShape, base.modifiedState) ?? richTextBuilder;
01366: 		}
01367: 		ModSourceHelper.CreateTooltip(richTextBuilder, this);
01368: 		return richTextBuilder.Build();
01369: 	}
01370: 
01371: 	public virtual RichText GetTooltipAdvanced()
01372: 	{
01373: 		RichTextBuilder richTextBuilder = GetTooltipBasic();
01374: 		if (base.parentInventory != null && base.tooltipRichTextFunc != null)
01375: 		{
01376: 			richTextBuilder = base.tooltipRichTextFunc(this, base.parentInventory, base.parentInventory.GetIncompleteItemInventorySlot(this), base.modifiedShape, base.modifiedState) ?? richTextBuilder;
01377: 		}
01378: 		ModSourceHelper.CreateTooltip(richTextBuilder, this);
01379: 		return richTextBuilder.Build();
01380: 	}
01381: }
~~~

## OverlayHelper.cs

~~~csharp
00024: 		}
00025: 		else if (str.Length > width)
00026: 		{
00027: 			str = str.Substring(0, width - 3) + "...";
00028: 		}
00029: 		return str;
00030: 	}
00031: 
00032: 	public static void AddLineCentered(int width, string str, RichTextBuilder builder)
00033: 	{
00034: 		builder.AddLine(GetLineCentered(width, str));
00035: 	}
00036: 
00037: 	public static void UpdateTopRightElement(RichTextElement topRightElement)
00038: 	{
00039: 		if (topRightElement == null)
00040: 		{
00041: 			return;
00042: 		}
00043: 		ShipManager shipManager = GameMaster.current.shipManager;
00044: 		TurnManager turnManager = GameMaster.current.turnManager;
00045: 		if (shipManager == null || turnManager == null)
00046: 		{
00047: 			return;
00048: 		}
00049: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00050: 		int width = topRightElement.fixedWidth / UISettings.current.fontWidth;
00051: 		if (GameMaster.current.InRaid())
00052: 		{
00053: 			AddLineCentered(width, "Docked:", richTextBuilder);
00054: 			if (GameMaster.current.raidManager.station != null)
00055: 			{
00056: 				string stationName = GameMaster.current.raidManager.station.stationName;
00057: 				AddLineCentered(width, stationName, richTextBuilder);
00058: 			}
00059: 			else
00060: 			{
00061: 				AddLineCentered(width, "???", richTextBuilder);
00062: 			}
00063: 		}
00064: 		else if (shipManager.isTransit)
00065: 		{
00066: 			AddLineCentered(width, "In Transit to:", richTextBuilder);
00067: 			if (GameMaster.current.shipManager.nextPOI != null)
00068: 			{
00069: 				string displayName = GameMaster.current.shipManager.nextPOI.displayName;
00070: 				AddLineCentered(width, displayName, richTextBuilder);
00071: 			}
00072: 			else
00073: 			{
00074: 				AddLineCentered(width, "???", richTextBuilder);
00075: 			}
00076: 		}
00077: 		else
00078: 		{
00079: 			AddLineCentered(width, "In Orbit:", richTextBuilder);
00080: 			if (GameMaster.current.raidManager.station != null)
00081: 			{
00082: 				string stationName2 = GameMaster.current.raidManager.station.stationName;
00083: 				AddLineCentered(width, stationName2, richTextBuilder);
~~~

~~~csharp
00073: 			{
00074: 				AddLineCentered(width, "???", richTextBuilder);
00075: 			}
00076: 		}
00077: 		else
00078: 		{
00079: 			AddLineCentered(width, "In Orbit:", richTextBuilder);
00080: 			if (GameMaster.current.raidManager.station != null)
00081: 			{
00082: 				string stationName2 = GameMaster.current.raidManager.station.stationName;
00083: 				AddLineCentered(width, stationName2, richTextBuilder);
00084: 			}
00085: 			else if (GameMaster.current.shipManager.nextPOI != null)
00086: 			{
00087: 				string displayName2 = GameMaster.current.shipManager.nextPOI.displayName;
00088: 				AddLineCentered(width, displayName2, richTextBuilder);
00089: 			}
00090: 			else
00091: 			{
00092: 				AddLineCentered(width, "???", richTextBuilder);
00093: 			}
00094: 		}
00095: 		string text = turnManager.GetHoursFormatted() + ":" + turnManager.GetMinutesFormatted();
00096: 		string text2 = turnManager.GetDaysFormatted() + "/" + turnManager.GetMonthsFormatted() + " " + turnManager.GetYearsFormatted();
00097: 		AddLineCentered(width, text + "    " + text2, richTextBuilder);
00098: 		topRightElement.SetRichText(richTextBuilder.Build());
00099: 	}
00100: 
00101: 	public static void TransformItemChoice(GameItem choiceItem, bool isRealItem = false)
00102: 	{
00103: 		if (!isRealItem)
00104: 		{
00105: 			choiceItem.state.Clear();
00106: 			choiceItem.modifiedState.Clear();
00107: 			choiceItem.SetValue(0L);
00108: 		}
00109: 		choiceItem.effects = new List<CombatAbilityEffect>();
00110: 		choiceItem.bonusAccuracy = 0;
00111: 		choiceItem.activateDefault = true;
00112: 		choiceItem.mayActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
00113: 		choiceItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
00114: 		GeneralHelper.SetItemOwned(choiceItem);
00115: 		choiceItem.maySelectSlotItemFunc = null;
00116: 		choiceItem.mayItemTargetItemFunc = null;
00117: 		choiceItem.mayItemTargetSlotItemFunc = null;
00118: 		choiceItem.mayItemTargetThisFunc = null;
00119: 		choiceItem.maySlotItemTargetItemFunc = null;
00120: 		choiceItem.mayThisTargetItemFunc = null;
00121: 		choiceItem.mayStackItemFunc = (GameItem _, GameItem _) => false;
00122: 		choiceItem.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
00123: 		choiceItem.maxSlotAddItemFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
00124: 		choiceItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
00125: 		choiceItem.onActivateSlotItemFunc = null;
00126: 		choiceItem.itemFeatures.Clear();
00127: 	}
00128: 
00129: 	public static void CheckDockValidParty(Action next)
00130: 	{
00131: 		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
00132: 			select !gci.isDead).Any())
~~~

~~~csharp
00113: 		choiceItem.canActivateSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
00114: 		GeneralHelper.SetItemOwned(choiceItem);
00115: 		choiceItem.maySelectSlotItemFunc = null;
00116: 		choiceItem.mayItemTargetItemFunc = null;
00117: 		choiceItem.mayItemTargetSlotItemFunc = null;
00118: 		choiceItem.mayItemTargetThisFunc = null;
00119: 		choiceItem.maySlotItemTargetItemFunc = null;
00120: 		choiceItem.mayThisTargetItemFunc = null;
00121: 		choiceItem.mayStackItemFunc = (GameItem _, GameItem _) => false;
00122: 		choiceItem.mayInventoryAddItemFunc = (GameItem _, GameInventory _) => true;
00123: 		choiceItem.maxSlotAddItemFunc = (GameItem _, GameInventory _, SlotMarker _) => 1;
00124: 		choiceItem.mayInventoryRemoveItemFunc = (GameItem _, GameInventory _) => false;
00125: 		choiceItem.onActivateSlotItemFunc = null;
00126: 		choiceItem.itemFeatures.Clear();
00127: 	}
00128: 
00129: 	public static void CheckDockValidParty(Action next)
00130: 	{
00131: 		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
00132: 			select !gci.isDead).Any())
00133: 		{
00134: 			next?.Invoke();
00135: 		}
00136: 		else
00137: 		{
00138: 			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("This ship cannot dock without a valid party inside.").Build(), null, null, canYes: true, showYes: false);
00139: 		}
00140: 	}
00141: 
00142: 	public static void ConfirmDock(Action next)
00143: 	{
00144: 		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to dock the ship?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("Docking the ship will not use a turn.")
00145: 			.AddLine("Your ship will land and arrive at the hangar immediately.")
00146: 			.Build(), delegate
00147: 		{
00148: 			next?.Invoke();
00149: 		});
00150: 	}
00151: 
00152: 	public static void CheckUndockValidParty(Action next)
00153: 	{
00154: 		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
00155: 			select !gci.isDead).Any())
00156: 		{
00157: 			next?.Invoke();
00158: 		}
00159: 		else
00160: 		{
00161: 			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("This ship cannot undock without a valid party inside.").Build(), null, null, canYes: true, showYes: false);
00162: 		}
00163: 	}
00164: 
00165: 	public static void ConfirmUndock(Action next)
00166: 	{
00167: 		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to undock the ship?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("Any character or item left behind may potentially be lost forever!")
00168: 			.Build(), delegate
00169: 		{
00170: 			next?.Invoke();
00171: 		});
00172: 	}
~~~

~~~csharp
00142: 	public static void ConfirmDock(Action next)
00143: 	{
00144: 		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to dock the ship?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("Docking the ship will not use a turn.")
00145: 			.AddLine("Your ship will land and arrive at the hangar immediately.")
00146: 			.Build(), delegate
00147: 		{
00148: 			next?.Invoke();
00149: 		});
00150: 	}
00151: 
00152: 	public static void CheckUndockValidParty(Action next)
00153: 	{
00154: 		if ((from gci in GraphUtils.FindChildrenType<GameCharacterItem>(GameMaster.current.shipManager.ship)
00155: 			select !gci.isDead).Any())
00156: 		{
00157: 			next?.Invoke();
00158: 		}
00159: 		else
00160: 		{
00161: 			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("This ship cannot undock without a valid party inside.").Build(), null, null, canYes: true, showYes: false);
00162: 		}
00163: 	}
00164: 
00165: 	public static void ConfirmUndock(Action next)
00166: 	{
00167: 		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to undock the ship?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("Any character or item left behind may potentially be lost forever!")
00168: 			.Build(), delegate
00169: 		{
00170: 			next?.Invoke();
00171: 		});
00172: 	}
00173: 
00174: 	public static void WarnUndockMissingCharacters(Action next)
00175: 	{
00176: 		IEnumerable<GameCharacterItem> enumerable = from gci in GameMaster.current.raidManager.GetAllPlayerCharacters()
00177: 			where !GraphUtils.FindParentEquals(gci, GameMaster.current.shipManager.ship)
00178: 			select gci;
00179: 		if (enumerable.Any())
00180: 		{
00181: 			RichTextBuilder richTextBuilder = new RichTextBuilder(50);
00182: 			richTextBuilder.AddLine("Warning, some characters will be left behind!", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: true).AddLine();
00183: 			foreach (GameCharacterItem item in enumerable)
00184: 			{
00185: 				richTextBuilder.AddLine(item.GetDisplayName());
00186: 			}
00187: 			richTextBuilder.AddLine().AddLine("Are you sure you want to continue?");
00188: 			OverlayHandler.current.ShowConfirmation("Confirmation", richTextBuilder.Build(), delegate
00189: 			{
00190: 				next?.Invoke();
00191: 			});
00192: 		}
00193: 		else
00194: 		{
00195: 			next?.Invoke();
00196: 		}
00197: 	}
00198: 
00199: 	public static void CheckFuelConfirmation(Action next, int distance)
00200: 	{
00201: 		if (PropulsionHelper.HasEnoughFuel(distance))
~~~

~~~csharp
00178: 			select gci;
00179: 		if (enumerable.Any())
00180: 		{
00181: 			RichTextBuilder richTextBuilder = new RichTextBuilder(50);
00182: 			richTextBuilder.AddLine("Warning, some characters will be left behind!", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed, bold: true, italic: false, underline: true).AddLine();
00183: 			foreach (GameCharacterItem item in enumerable)
00184: 			{
00185: 				richTextBuilder.AddLine(item.GetDisplayName());
00186: 			}
00187: 			richTextBuilder.AddLine().AddLine("Are you sure you want to continue?");
00188: 			OverlayHandler.current.ShowConfirmation("Confirmation", richTextBuilder.Build(), delegate
00189: 			{
00190: 				next?.Invoke();
00191: 			});
00192: 		}
00193: 		else
00194: 		{
00195: 			next?.Invoke();
00196: 		}
00197: 	}
00198: 
00199: 	public static void CheckFuelConfirmation(Action next, int distance)
00200: 	{
00201: 		if (PropulsionHelper.HasEnoughFuel(distance))
00202: 		{
00203: 			OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Are you sure you want to leave?", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("This action will end the current turn.")
00204: 				.AddLine("Any item or character left here might be lost forever!")
00205: 				.Build(), delegate
00206: 			{
00207: 				AudioHelper.ShuttleJump();
00208: 				next?.Invoke();
00209: 			});
00210: 		}
00211: 		else
00212: 		{
00213: 			OverlayHandler.current.ShowConfirmation("Problem", new RichTextBuilder(50).AddLine("Not enough fuel to jump to this destination!").Build(), null, null, canYes: true, showYes: false);
00214: 		}
00215: 	}
00216: }
~~~

## EventData.cs

~~~csharp
00025: 
00026: 	public Action<EventData> startAction;
00027: 
00028: 	public Action<EventData> turnAction;
00029: 
00030: 	public Action<EventData> endAction;
00031: 
00032: 	private int previousValidateTurn = -1;
00033: 
00034: 	public string title { get; set; }
00035: 
00036: 	public RichTextBuilder text { get; set; }
00037: 
00038: 	public string imagePath { get; set; }
00039: 
00040: 	public EventData()
00041: 	{
00042: 		turns = 0;
00043: 		startTurn = 0;
00044: 		endTurn = int.MaxValue;
00045: 	}
00046: 
00047: 	public EventData SetText(string title, string text)
00048: 	{
00049: 		this.title = title;
00050: 		this.text = new RichTextBuilder().Add(text);
00051: 		return this;
00052: 	}
00053: 
00054: 	public EventData SetText(string title, RichTextBuilder text)
00055: 	{
00056: 		this.title = title;
00057: 		this.text = new RichTextBuilder().Add(text);
00058: 		return this;
00059: 	}
00060: 
00061: 	public void AddText(string text)
00062: 	{
00063: 		this.text = this.text.Add(text);
00064: 	}
00065: 
00066: 	public void AddText(RichTextBuilder text)
00067: 	{
00068: 		this.text = this.text.Add(text);
00069: 	}
00070: 
00071: 	public EventData SetImage(string imagePath)
00072: 	{
00073: 		this.imagePath = "EventCards/" + imagePath;
00074: 		return this;
00075: 	}
00076: 
00077: 	public static void OnChoiceEnable(GameItem choiceItem)
00078: 	{
00079: 		if (choiceItem != null)
00080: 		{
00081: 			choiceItem.activateDefault = true;
00082: 			choiceItem.forceDisableActivate = false;
00083: 			if (choiceItem is GameItemElement gameItemElement)
00084: 			{
~~~

## DebrisHelper.cs

~~~csharp
00266: 	}
00267: 
00268: 	public static void PlaceHoarderPiles()
00269: 	{
00270: 		GameGridInventory invElement = EmporiumEntry.Instance.invElement;
00271: 		List<GameItem> list = new List<GameItem>
00272: 		{
00273: 			DirectoryMaster.Item("debris_pile1"),
00274: 			DirectoryMaster.Item("debris_pile2"),
00275: 			DirectoryMaster.Item("debris_pile3")
00276: 		};
00277: 		int width = invElement.inventoryShape.width;
00278: 		int height = invElement.inventoryShape.height;
00279: 		int[] array = list.Select((GameItem p) => p.modifiedShape.globalWidth).ToArray();
00280: 		int num = width - array[2];
00281: 		int[] array2 = new int[3]
00282: 		{
00283: 			0,
00284: 			array[0] + Mathf.Max(0, (num - array[0] - array[1]) / 2),
00285: 			num
00286: 		};
00287: 		List<GameItem> list2 = new List<GameItem>();
00288: 		for (int num2 = 0; num2 < list.Count; num2++)
00289: 		{
00290: 			GameItem gameItem = list[num2];
00291: 			GridShape gridShape = new GridShapeBuilder(gameItem.modifiedShape).SetPosition(array2[num2], height - gameItem.modifiedShape.globalHeight).Build();
00292: 			foreach (GameItem childItem in invElement.childItems)
00293: 			{
00294: 				if (!list.Contains(childItem) && gridShape.Intersects(childItem.modifiedShape))
00295: 				{
00296: 					invElement.Expel(childItem);
00297: 					list2.Add(childItem);
00298: 				}
00299: 			}
00300: 			if (invElement.CheckShapePlacement(gameItem, gridShape, new List<GameItem>()))
00301: 			{
00302: 				gameItem.modifiedShape = new GridShapeBuilder(gameItem.modifiedShape).SetTransform(gridShape);
00303: 				invElement.UncheckedAccept(gameItem);
00304: 			}
00305: 			else
00306: 			{
00307: 				GraphUtils.EmporiumTryAdd(invElement, gameItem);
00308: 			}
00309: 		}
00310: 		foreach (GameItem item in list2)
00311: 		{
00312: 			GraphUtils.EmporiumTryAdd(invElement, item);
00313: 		}
00314: 	}
00315: 
00316: 	public static void CreateTooltip(RichTextBuilder builder, GameItem item)
00317: 	{
00318: 		if (!IsDebris(item))
00319: 		{
00320: 			return;
00321: 		}
00322: 		builder.AddLine(" ");
00323: 		if (item.identifier == "debris_pile1")
00324: 		{
00325: 			builder.AddLine(LocHelper.GetLocalizedMechanic("mech_machinery_progress") + " " + GetProgress(item) + "/" + 10, ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
~~~

## SpaceDistressSignalFakeTrigger.cs

~~~csharp
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData e = new EventData().SetText("Ship Distress Signal", "You have arrived to the source of the distress signal, an heavily armed pirate ship. ");
00016: 		e.SetImage("raider");
00017: 		e.text.AddLine("\r\nIt's trap.", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
00018: 		e.text.AddLine("\r\nThe ship's captain has contacted you and is demanding that you pay 400 Energy Credit or they will open fire on your ship.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
00019: 		e.startAction = delegate(EventData eventData)
00020: 		{
00021: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DISTRESS_SIGNAL_FAKE_EVENT_STARTED");
00022: 			eventData.BlockTravel();
00023: 			tag.SetInt(tag.GetInt() + 1);
00024: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00025: 			{
00026: 				overworldSpace.isEventTriggered = true;
00027: 			}
00028: 		};
00029: 		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
00030: 		e.AddChoice(DirectoryMaster.Item("ok"), "(400 Energy Credit) Pay the bandit", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 400, null, delegate
00031: 		{
00032: 			GameMaster.current.shipManager.RemoveEnergyCredit(400);
00033: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit is satisfied and informs you that you may leave peacefully.", RenderHandler.ColorPalette.LightGreen));
00034: 			e.ForceEnd();
00035: 		});
00036: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Refuse to pay and attempt to escape.", "", null, null, delegate
00037: 		{
00038: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away with your ship and the pirate has open fire on your ship!", RenderHandler.ColorPalette.LightRed));
00039: 			SpaceBattleHelper.TakingHit(10);
00040: 			e.ForceEnd();
00041: 		});
00042: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00043: 		{
00044: 		});
00045: 		return e;
00046: 	}
00047: }
~~~

## MachineDnaManipulator.cs

~~~csharp
00634: 		goto IL_006e;
00635: 		IL_006e:
00636: 		if (gameItem == null)
00637: 		{
00638: 			return null;
00639: 		}
00640: 		if (PlantGeneticsHelper.IsWrittenDisk(dragged))
00641: 		{
00642: 			reason = PlantGeneticsHelper.GetDiskIncompatibilityReason(dragged, GetSeedSlot(gameItem).childItem);
00643: 		}
00644: 		else if (dragged.IsTag("SEED_ITEM_TAG") && HydroponicHelper.GetPlant(dragged) != null)
00645: 		{
00646: 			reason = PlantGeneticsHelper.GetDiskIncompatibilityReason(GetInputDiskSlot(gameItem).childItem, dragged);
00647: 		}
00648: 		if (reason.Length <= 0)
00649: 		{
00650: 			return null;
00651: 		}
00652: 		return gameItem;
00653: 	}
00654: 
00655: 	public static RichText CreateIncompatibleDragTooltip(RichTextBuilder builder, GameItem dragged, GameItem machine, string reason)
00656: 	{
00657: 		builder.Clear();
00658: 		builder.Add(PlantGeneticsText.DragRefused(dragged.GetDisplayName(), "<u>" + machine.GetDisplayName() + "</u>", reason), RenderHandler.ColorPalette.Red);
00659: 		return builder.Build();
00660: 	}
00661: 
00662: 	public static void CreateMachineTooltip(RichTextBuilder builder, GameItem item)
00663: 	{
00664: 		if (!item.IsTag("DNA_MANIPULATOR_TAG"))
00665: 		{
00666: 			return;
00667: 		}
00668: 		builder.AddLine(" ");
00669: 		bool flag = IsEjectEnabled(item);
00670: 		builder.AddLine(flag ? PlantGeneticsText.EjectOn() : PlantGeneticsText.EjectOff(), ignoreEmpty: true, flag ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightOrange);
00671: 		builder.AddLine(PlantGeneticsText.EjectHint(), ignoreEmpty: true, RenderHandler.ColorPalette.Gray);
00672: 		builder.AddLine(" ");
00673: 		if (MachineProgressHelper.GetMachineState(item).Equals("STATE_WORKING"))
00674: 		{
00675: 			string text = item.GetTagReadonly("DNA_OPERATION_STRING").GetString();
00676: 			PlantTrait plantTrait = PlantTraitRegistry.Get(item.GetTagReadonly("DNA_OPERATION_TRAIT_STRING").GetString());
00677: 			string text2 = ((text.Equals("apply") && plantTrait != null) ? PlantGeneticsText.StatusApplying(plantTrait.displayName) : PlantGeneticsText.StatusExtracting());
00678: 			builder.AddLine(text2, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00679: 			var (num, num2) = MachineProgressHelper.GetProgress(item);
00680: 			builder.AddProgressBar((float)num / (float)Mathf.Max(1, num2), 10, RenderHandler.ColorPalette.LightGreen, RenderHandler.ColorPalette.VeryDarkGreen, null, " ");
00681: 			builder.AddLine(num + "/" + num2, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00682: 			builder.AddLine(PlantGeneticsText.SuccessChance(item.GetTagReadonly("DNA_OPERATION_SUCCESS_INT").GetInt()), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
00683: 			return;
00684: 		}
00685: 		OperationPlan operationPlan = PlanOperation(item);
00686: 		if (operationPlan == null)
00687: 		{
00688: 			builder.AddLine(PlantGeneticsText.StatusReady(), ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
00689: 			return;
00690: 		}
00691: 		if (operationPlan.blockedReason.Length > 0)
00692: 		{
00693: 			builder.AddLine(PlantGeneticsText.CannotStart(operationPlan.blockedReason), ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
~~~

## UnusedDirectory.cs

~~~csharp
01478: 		};
01479: 		for (int num = 0; num < effectList.Length; num++)
01480: 		{
01481: 			item.effects.Add(DirectoryMaster.CombatAbilityEffect(effectList[num]));
01482: 		}
01483: 		item.bonusAccuracy = bonusAccuracy;
01484: 		item.unitValue = value;
01485: 		return item;
01486: 	}
01487: 
01488: 	private GameItem CreatSTANAG(string sprite, int size)
01489: 	{
01490: 		PixelWindow pixelWindow = new PixelWindow(canMove: true);
01491: 		GameSlotInventory element = new GameSlotInventory(2, 2);
01492: 		pixelWindow.Attach(element);
01493: 		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(2, 2);
01494: 		PixelWindow item = tuple.Item1;
01495: 		GameInventory item2 = tuple.Item2;
01496: 		item2.mayInventoryAddItemFunc = (GameItem gameItem2, GameInventory inv) => (gameItem2.IsTag("cartridge") && gameItem2.IsTag("nato556x45")) ? true : false;
01497: 		item2.maxSlotAmountItemFunc = (GameItem gameItem2, GameInventory inv, SlotMarker slot) => (gameItem2.IsTag("cartridge") && gameItem2.IsTag("nato556x45")) ? size : 0;
01498: 		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/items_weapons_ranged_antique", sprite);
01499: 		gameItem.EnableTag("stanag");
01500: 		gameItem.EnableTag("magazine");
01501: 		gameItem.tooltipRichTextFunc = delegate
01502: 		{
01503: 			RichTextBuilder richTextBuilder = new RichTextBuilder(30);
01504: 			richTextBuilder.AddLine(size + "-Round STANAG Magazine", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
01505: 			return richTextBuilder;
01506: 		};
01507: 		gameItem.parentItemShapeModifierForItemFunc = delegate(GameItem gameItem3, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem _)
01508: 		{
01509: 			GameItem gameItem2 = GraphUtils.FindParentType<GameItem>(inventory);
01510: 			return (gameItem2 == null || !gameItem2.IsTag("m16")) ? null : new GridShapeBuilder().SetDataFill(2, gameItem3.modifiedShape.height, 1).SetTransform(4, 1, flipped: false, 0);
01511: 		};
01512: 		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem gameItem3, GameInventory inventory, SlotMarker _, GridShape _, TagSystem _, GameItem _)
01513: 		{
01514: 			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
01515: 			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
01516: 			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
01517: 			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
01518: 			GameItem gameItem2 = GraphUtils.FindParentType<GameItem>(inventory);
01519: 			if (gameItem2 == null || !gameItem2.IsTag("m16"))
01520: 			{
01521: 				return list;
01522: 			}
01523: 			list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/items_weapons_ranged_antique", sprite + "_mod"), new Vector2(64f, -16f)));
01524: 			return list;
01525: 		};
01526: 		return gameItem;
01527: 	}
01528: 
01529: 	private GameItem CreateM16Receiver()
01530: 	{
01531: 		PixelWindow pixelWindow = new PixelWindow(canMove: true);
01532: 		GameSlotInventory element = new GameSlotInventory(2, 2);
01533: 		pixelWindow.Attach(element);
01534: 		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(2, 2);
01535: 		PixelWindow item = tuple.Item1;
01536: 		GameInventory item2 = tuple.Item2;
01537: 		item2.mayInventoryAddItemFunc = (GameItem gameItem2, GameInventory inv) => (gameItem2.IsTag("magazine") && gameItem2.IsTag("stanag")) ? true : false;
~~~

~~~csharp
01621: 		AudioHelper.InitPistol(gameItem);
01622: 		return gameItem;
01623: 	}
01624: 
01625: 	private GameItem CreateShotgunReceiver()
01626: 	{
01627: 		(PixelWindow, GameInventory) tuple = DirectoryUtils.CreateSlotInventoryWindow(2, 2);
01628: 		PixelWindow item = tuple.Item1;
01629: 		GameInventory item2 = tuple.Item2;
01630: 		item2.mayInventoryAddItemFunc = (GameItem gameItem2, GameInventory inv) => (gameItem2.spritePath.Contains("shotgun_stock") || gameItem2.spritePath.Contains("shotgun_grip")) ? true : false;
01631: 		item2.maxSlotAddItemFunc = (GameItem gameItem2, GameInventory inv, SlotMarker slot) => (gameItem2.spritePath.Contains("shotgun_stock") || gameItem2.spritePath.Contains("shotgun_grip")) ? 1 : 0;
01632: 		GameItem gameItem = Empty().SetContentWindow(item).SetSpriteAndShape("Items/itematlas", "shotgun");
01633: 		gameItem.maySelectSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
01634: 		gameItem.canSelectSlotItemFunc = (GameItem currentNode, GameInventory _, SlotMarker _) => GraphUtils.FindChildType<GameInventory>(currentNode).childItems.Any((GameItem i) => i.spritePath.Contains("shotgun_"));
01635: 		gameItem.mayItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
01636: 		gameItem.canItemTargetItemFunc = (GameItem s, GameItem t) => t.IsTag("enemy");
01637: 		gameItem.onItemTargetItemFunc = delegate(GameItem s, GameItem t)
01638: 		{
01639: 			GameMaster.current.raidManager.CastAbility(s, t);
01640: 		};
01641: 		gameItem.isItemStateValidFunc = (GameItem _, GameInventory _, SlotMarker slot, GridShape _, TagSystem state) => !slot.IsEquipmentSlot() || state.IsTag("mod_grip");
01642: 		gameItem.ability = DirectoryMaster.CombatAbility("weapon_attack");
01643: 		gameItem.name = "Shotgun Receiver";
01644: 		gameItem.tooltipRichTextFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem state)
01645: 		{
01646: 			RichTextBuilder richTextBuilder = new RichTextBuilder(30);
01647: 			richTextBuilder.AddLine("Shotgun" + (state.IsTag("folded") ? " (Folded)" : ""), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
01648: 			return richTextBuilder;
01649: 		};
01650: 		AudioHelper.InitShotgun(gameItem);
01651: 		gameItem.unitValue = 150L;
01652: 		return gameItem;
01653: 	}
01654: 
01655: 	private GameItem CreateShotgunStock()
01656: 	{
01657: 		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "shotgun_stock");
01658: 		gameItem.parentItemStateModifierForItemFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem parentItem)
01659: 		{
01660: 			List<ItemStateModifier> list = new List<ItemStateModifier>();
01661: 			if (parentItem.spritePath == "shotgun")
01662: 			{
01663: 				list.Add(ItemStateModifier.Create("mod_grip"));
01664: 			}
01665: 			list.Add(ItemStateModifier.Create("folded", ItemStateModifier.Operation.SetEnabled).SetEnabled(state.IsTag("folded")));
01666: 			return list;
01667: 		};
01668: 		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem _) => state.IsTag("folded") ? new GridShapeBuilder().SetDataFill(3, 2, 1).SetTransform(2, 0, flipped: false, 0) : new GridShapeBuilder().SetDataFill(3, 2, 1).SetTransform(-3, 0, flipped: false, 0);
01669: 		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem _)
01670: 		{
01671: 			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
01672: 			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
01673: 			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
01674: 			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
01675: 			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
01676: 			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
01677: 			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
01678: 			if (state.IsTag("folded"))
01679: 			{
01680: 				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/itematlas", "shotgun_stock_mod"), new Vector2(9f, -1f), 0, isFlipped: true));
~~~

~~~csharp
01667: 		};
01668: 		gameItem.parentItemShapeModifierForItemFunc = (GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem _) => state.IsTag("folded") ? new GridShapeBuilder().SetDataFill(3, 2, 1).SetTransform(2, 0, flipped: false, 0) : new GridShapeBuilder().SetDataFill(3, 2, 1).SetTransform(-3, 0, flipped: false, 0);
01669: 		gameItem.parentItemSpriteModifierForItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem state, GameItem _)
01670: 		{
01671: 			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
01672: 			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
01673: 			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
01674: 			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
01675: 			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
01676: 			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
01677: 			List<ItemSpriteModifier> list = new List<ItemSpriteModifier>();
01678: 			if (state.IsTag("folded"))
01679: 			{
01680: 				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/itematlas", "shotgun_stock_mod"), new Vector2(9f, -1f), 0, isFlipped: true));
01681: 			}
01682: 			else
01683: 			{
01684: 				list.Add(new ItemSpriteModifier(RenderHandler.LoadFromAtlas("Items/itematlas", "shotgun_stock_mod"), new Vector2(-49f, -1f)));
01685: 			}
01686: 			return list;
01687: 		};
01688: 		gameItem.mayToggleSlotItemFunc = (GameItem _, GameInventory _, SlotMarker _) => true;
01689: 		gameItem.canToggleSlotItemFunc = (GameItem _, GameInventory inventory, SlotMarker _) => GraphUtils.FindParentType<GameItem>(inventory)?.spritePath == "shotgun";
01690: 		gameItem.tooltipRichTextFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem state)
01691: 		{
01692: 			RichTextBuilder richTextBuilder = new RichTextBuilder(30);
01693: 			richTextBuilder.AddLine("Shotgun Stock" + (state.IsTag("folded") ? " (Folded)" : ""), ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
01694: 			return richTextBuilder;
01695: 		};
01696: 		gameItem.onToggleSlotItemFunc = delegate(GameItem item, GameInventory inventory, SlotMarker _)
01697: 		{
01698: 			item.ModifyTag("folded", delegate(TagState state)
01699: 			{
01700: 				state.ToggleEnabled();
01701: 			});
01702: 			if (item.CheckValidShapeState())
01703: 			{
01704: 				item.ValidateShapeState();
01705: 			}
01706: 			else
01707: 			{
01708: 				item.ModifyTag("folded", delegate(TagState state)
01709: 				{
01710: 					state.ToggleEnabled();
01711: 				});
01712: 			}
01713: 		};
01714: 		gameItem.onSlotRemoveItemFunc = delegate(GameItem item, GameInventory _, SlotMarker _)
01715: 		{
01716: 			item.DisableTag("folded");
01717: 		};
01718: 		return gameItem;
01719: 	}
01720: 
01721: 	private GameItem CreateShotgunGrip()
01722: 	{
01723: 		GameItem gameItem = Empty().SetSpriteAndShape("Items/itematlas", "shotgun_grip");
01724: 		gameItem.parentItemStateModifierForItemFunc = delegate(GameItem _, GameInventory _, SlotMarker _, GridShape _, TagSystem _, GameItem parentItem)
01725: 		{
01726: 			List<ItemStateModifier> list = new List<ItemStateModifier>();
~~~

## SpaceMedicTrigger.cs

~~~csharp
00007: 		{
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData eventData = new EventData().SetText("Friendly Hospital Ship", "You arrive near a large white ship.");
00016: 		eventData.SetImage("spacer_doc");
00017: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00018: 		trader.name = "Hippocrates Order Donation Box";
00019: 		trader.BarterEnable(TradeSheet.GetDonationShop());
00020: 		eventData.text.AddLine("\r\nThe leader of the ship contacts you and introduce themself:\r\n'Hello, We are the Hippocrates Order and I am the chief medical officier of this ship. We try to provide free healthcare to whoever need it.'.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
00021: 		eventData.startAction = delegate
00022: 		{
00023: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_MEDIC_EVENT_STARTED");
00024: 			tag.SetInt(tag.GetInt() + 1);
00025: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00026: 			{
00027: 				overworldSpace.isEventTriggered = true;
00028: 			}
00029: 		};
00030: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask more information about the Hippocrates Order", "", null, null, delegate
00031: 		{
00032: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'The Hippocrates Order are a group of people who believe in free universal healthcare, between the for profit hospitals and Carmicheal's prices on their medication, nobody can afford healthcare anymore.' So we try our best to help anyone in need of medical attention for free. ", RenderHandler.ColorPalette.LightGray));
00033: 		});
00034: 		eventData.AddChoice(DirectoryMaster.Item("question"), "How does the Hippocrates Order afford all this?", "", null, null, delegate
00035: 		{
00036: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'Everything we have are funded with donation, there's many out there who share our belief and willing to give. You are welcome to make a donation too.' We will use the credit to pay for more staff and field better equipment.", RenderHandler.ColorPalette.LightGray));
00037: 		});
00038: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Donate) Consider a donation to the Hippocrates Order.", "", null, null, delegate
00039: 		{
00040: 			trader.BarterShow();
00041: 		});
00042: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Rob the hospital ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00043: 		{
00044: 		});
00045: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Open fire on the hospital ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00046: 		{
00047: 		});
00048: 		bool used = false;
00049: 		eventData.AddChoice(DirectoryMaster.Item("heal"), "Get medical attention for the crew.", "", () => !used, null, delegate
00050: 		{
00051: 			foreach (GameCharacterItem item in GameMaster.current.shipManager.GetCrew())
00052: 			{
00053: 				GCIHelper.BasicFullHeal(item);
00054: 				GCIHelper.Bandage(item);
00055: 			}
00056: 			used = true;
00057: 		});
00058: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00059: 		{
00060: 		});
00061: 		return eventData;
00062: 	}
00063: }
~~~

## DirectoryUtils.cs

~~~csharp
00434: 	{
00435: 		PixelWindow pixelWindow = new PixelWindow(isDraggable);
00436: 		GridPixelElement gridPixelElement = new GridPixelElement(3, 1, isCentered);
00437: 		GameSlotInventory gameSlotInventory = new GameSlotInventory(1, 1);
00438: 		ContainerHelper.AllowOnlyTaggedItem(gameSlotInventory, "injector");
00439: 		gameSlotInventory.SetBackgroundFadeSprite("Items/items_medical", "injector_background");
00440: 		GameSlotInventory gameSlotInventory2 = new GameSlotInventory(1, 1);
00441: 		ContainerHelper.AllowOnlyTaggedItem(gameSlotInventory2, "injector");
00442: 		gameSlotInventory2.SetBackgroundFadeSprite("Items/items_medical", "injector_background");
00443: 		GameSlotInventory gameSlotInventory3 = new GameSlotInventory(1, 1);
00444: 		ContainerHelper.AllowOnlyTaggedItem(gameSlotInventory3, "injector");
00445: 		gameSlotInventory3.SetBackgroundFadeSprite("Items/items_medical", "injector_background");
00446: 		gridPixelElement.Attach(gameSlotInventory, 0, 0, minWidth);
00447: 		gridPixelElement.Attach(gameSlotInventory2, 1, 0, minWidth);
00448: 		gridPixelElement.Attach(gameSlotInventory3, 2, 0, minWidth);
00449: 		pixelWindow.Attach(gridPixelElement);
00450: 		return pixelWindow;
00451: 	}
00452: 
00453: 	public static PixelWindow CreateIntelWindow(string title, string author, string content, bool isDraggable = true, bool isCentered = true, int minWidth = 150)
00454: 	{
00455: 		PixelWindow pixelWindow = new PixelWindow(isDraggable);
00456: 		GridPixelElement gridPixelElement = new GridPixelElement(1, 3, isCentered);
00457: 		TagElement element = new TagElement().SetText(title);
00458: 		TagElement element2 = new TagElement().SetText("By " + author);
00459: 		RichTextBuilder richTextBuilder = new RichTextBuilder(65).Add(content);
00460: 		RichTextElement richTextElement = new RichTextElement();
00461: 		richTextElement.SetRichText(richTextBuilder.Build());
00462: 		gridPixelElement.Attach(element, 0, 0);
00463: 		gridPixelElement.Attach(element2, 0, 1);
00464: 		gridPixelElement.Attach(richTextElement, 0, 2, minWidth);
00465: 		pixelWindow.Attach(gridPixelElement);
00466: 		return pixelWindow;
00467: 	}
00468: 
00469: 	public static (PixelWindow, GameInventory) CreateSlotInventoryWindow(int width, int height, bool isDraggable = true)
00470: 	{
00471: 		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
00472: 		GameSlotInventory gameSlotInventory = new GameSlotInventory(width, height, stealUI: true).SetMachineryStyle();
00473: 		pixelWindow.Attach(gameSlotInventory);
00474: 		return (pixelWindow, gameSlotInventory);
00475: 	}
00476: 
00477: 	public static (PixelWindow, GameSlotInventory) CreateSlotInventoryWindow(bool isDraggable = true)
00478: 	{
00479: 		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
00480: 		GameSlotInventory gameSlotInventory = new GameSlotInventory().SetMachineryStyle();
00481: 		pixelWindow.Attach(gameSlotInventory);
00482: 		return (pixelWindow, gameSlotInventory);
00483: 	}
00484: 
00485: 	public static (PixelWindow, GameInventory) CreateInventoryWindow(string data, int width, bool isDraggable = true)
00486: 	{
00487: 		PixelWindow pixelWindow = new PixelWindow(isDraggable, null, WindowStyle.Machinery);
00488: 		GameGridInventory gameGridInventory = new GameGridInventory(data, width).SetMachineryStyle();
00489: 		pixelWindow.Attach(gameGridInventory);
00490: 		return (pixelWindow, gameGridInventory);
00491: 	}
00492: 
00493: 	public static PixelWindow CreatePassengerCabinWindow(bool isDraggable = true, bool isCentered = true, int minWidth = 150)
~~~

## MachineHelper.cs

~~~csharp
00203: 		}
00204: 		else if (hoveredItem == null && hoveredInventory != null && hoveredInventory.IsInsertLocked())
00205: 		{
00206: 			gameItem = hoveredInventory.GetParentItem();
00207: 		}
00208: 		if (gameItem == null || !gameItem.IsTag("LOCKED_WORKING_TAG"))
00209: 		{
00210: 			return null;
00211: 		}
00212: 		GameGridInventory moduleGrid = GetModuleGrid(gameItem);
00213: 		if (moduleGrid == null || moduleGrid.mayInventoryAddItemFunc == null)
00214: 		{
00215: 			return null;
00216: 		}
00217: 		if (!moduleGrid.mayInventoryAddItemFunc.InvokeAllReduce(dragged, moduleGrid, InvokeFuncExtensions.BoolReduceOR))
00218: 		{
00219: 			return null;
00220: 		}
00221: 		return gameItem;
00222: 	}
00223: 
00224: 	public static RichText CreateLockedModuleBayDragTooltip(RichTextBuilder builder, GameItem module, GameItem machine)
00225: 	{
00226: 		builder.Clear();
00227: 		builder.Add(LocHelper.GetLocalizedMechanic("mech_drag_module_bay_locked", module.GetDisplayName(preferCustomName: true), "<u>" + machine.GetDisplayName(preferCustomName: true) + "</u>"), RenderHandler.ColorPalette.Red);
00228: 		return builder.Build();
00229: 	}
00230: 
00231: 	private static GameGridInventory GetModuleGrid(GameItem machine)
00232: 	{
00233: 		return GetBodyStorage(machine, 1) as GameGridInventory;
00234: 	}
00235: 
00236: 	public static bool CanExpand(GameItem item)
00237: 	{
00238: 		if (item.GetTagReadonly("MODULE_UPGRADE_STAGE_INT").GetInt() < 2)
00239: 		{
00240: 			return true;
00241: 		}
00242: 		return false;
00243: 	}
00244: 
00245: 	public static void ExpandModule(GameGridInventory moduleGrid, GameItem item)
00246: 	{
00247: 		int moduleUpgradeStage = item.GetTagReadonly("MODULE_UPGRADE_STAGE_INT").GetInt();
00248: 		if (moduleUpgradeStage == 0)
00249: 		{
00250: 			Debug.Log("Expand to Module Bay Upgrade 1");
00251: 			moduleGrid.SetShape("10000010000000000000000000001000001", 7);
00252: 		}
00253: 		else if (moduleUpgradeStage == 1)
00254: 		{
00255: 			Debug.Log("Expand to Module Bay Upgrade 2");
00256: 			moduleGrid.SetShape("00000000000000000000000000000000000", 7);
00257: 		}
00258: 		moduleUpgradeStage++;
00259: 		item.ModifyTag("MODULE_UPGRADE_STAGE_INT", delegate(TagState state)
00260: 		{
00261: 			state.SetInt(moduleUpgradeStage);
00262: 		});
~~~

## SpaceFuelTankerTrigger.cs

~~~csharp
00023: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00024: 		trader.name = "Dirt cheap fuel (Energy Credit Only)";
00025: 		trader.BarterEnable(TradeSheet.GetStolenFuelStore());
00026: 		List<string> tagList = new List<string> { "EXOTIC_FUEL_TAG" };
00027: 		List<int> discountList = new List<int> { -50 };
00028: 		List<string> itemTags = new List<string> { "power_source_item" };
00029: 		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
00030: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
00031: 		eventData.startAction = delegate
00032: 		{
00033: 			GameMaster.current.worldTags.EnableTag("FUEL_TANKER_VISITED");
00034: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_FUEL_TANKER_EVENT_STARTED");
00035: 			tag.SetInt(tag.GetInt() + 1);
00036: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00037: 			{
00038: 				overworldSpace.isEventTriggered = true;
00039: 			}
00040: 			NavDiskHelper.HandleNavDiskOnArrivalToPoi();
00041: 		};
00042: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Trader) Check out the fuel.", "", null, null, delegate
00043: 		{
00044: 			trader.BarterShow();
00045: 		});
00046: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Why is it so cheap?", "", null, null, delegate
00047: 		{
00048: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n`Because we got it really cheap too.'", RenderHandler.ColorPalette.LightGray));
00049: 			GameMaster.current.worldTags.EnableTag("SPACE_FUEL_TANKER_EVENT_STARTED_STAGE_1");
00050: 		});
00051: 		eventData.AddChoice(DirectoryMaster.Item("question"), "You stole the tanker didn't you?", "", null, () => GameMaster.current.worldTags.IsTag("SPACE_FUEL_TANKER_EVENT_STARTED_STAGE_1"), delegate
00052: 		{
00053: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n`Hey! Are you gonna buy some fuel or not?'", RenderHandler.ColorPalette.LightGray));
00054: 		});
00055: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "(Hostile) Rob the fuel tanker", "", null, null, delegate
00056: 		{
00057: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n`What are you going to do if I refuse? Shoot this FUEL TANKER? Man, either buy the fuel or get the hell out of here.'", RenderHandler.ColorPalette.LightGray));
00058: 		});
00059: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00060: 		{
00061: 		});
00062: 		return eventData;
00063: 	}
00064: }
~~~

## GameSlotInventory.cs

~~~csharp
00675: 		slotMarker = TryInventorySlot(item, num, item.modifiedShape.Clone());
00676: 		if (slotMarker.numTransfer > 0)
00677: 		{
00678: 			return slotMarker;
00679: 		}
00680: 		return null;
00681: 	}
00682: 
00683: 	public GameSlotInventory SetTooltipName(string name)
00684: 	{
00685: 		tooltipName = name;
00686: 		return this;
00687: 	}
00688: 
00689: 	public bool HasTooltip()
00690: 	{
00691: 		if (childItem != null)
00692: 		{
00693: 			return false;
00694: 		}
00695: 		return tooltipName != null;
00696: 	}
00697: 
00698: 	public RichText GetTooltip()
00699: 	{
00700: 		RichTextBuilder richTextBuilder = new RichTextBuilder(40);
00701: 		if (tooltipName != null)
00702: 		{
00703: 			richTextBuilder.AddLine(tooltipName, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
00704: 		}
00705: 		return richTextBuilder.Build();
00706: 	}
00707: 
00708: 	public RichText GetTooltipAdvanced()
00709: 	{
00710: 		return GetTooltip();
00711: 	}
00712: }
~~~

## MachineryRadioButton.cs

~~~csharp
00164: 		Refresh();
00165: 	}
00166: 
00167: 	protected override void OnHide()
00168: 	{
00169: 		((Behaviour)plate.image).enabled = false;
00170: 		((Behaviour)textNode.text).enabled = false;
00171: 		base.widthPixels = 0;
00172: 		base.heightPixels = 0;
00173: 	}
00174: 
00175: 	protected override void OnShow()
00176: 	{
00177: 		((Behaviour)plate.image).enabled = true;
00178: 		((Behaviour)textNode.text).enabled = !string.IsNullOrEmpty(text);
00179: 		Validate();
00180: 	}
00181: 
00182: 	public override bool HasTooltip()
00183: 	{
00184: 		return !string.IsNullOrEmpty(description);
00185: 	}
00186: 
00187: 	public override RichText GetTooltip()
00188: 	{
00189: 		return new RichTextBuilder().Add(description).Build();
00190: 	}
00191: }
~~~

## Station.cs

~~~csharp
00045: 			{
00046: 				var (list2, list3) = MetaHelper.RetrieveListInTagSystemStringInt("DISCOUNT_TAG_LIST", item2);
00047: 				MetaHelper.WipeListInTagSystemStringInt("DISCOUNT_TAG_LIST", item2);
00048: 				list2.AddRange(item.discountedItemTagList);
00049: 				list3.AddRange(item.discountedItemDiscountList);
00050: 				MetaHelper.StoreListInTagSystemStringInt(list2, list3, "DISCOUNT_TAG_LIST", item2);
00051: 			}
00052: 		}
00053: 	}
00054: 
00055: 	public List<GameCharacterItem> FindAllTrader()
00056: 	{
00057: 		List<GameCharacterItem> list = new List<GameCharacterItem>();
00058: 		foreach (Room allRoom in GetAllRooms())
00059: 		{
00060: 			if (allRoom.npcParty != null && allRoom.npcParty.GetCharacterItems() != null)
00061: 			{
00062: 				list.AddRange(allRoom.npcParty.GetCharacterItems());
00063: 			}
00064: 		}
00065: 		return list.Where((GameCharacterItem element) => element.IsMerchant()).ToList();
00066: 	}
00067: 
00068: 	public RichTextBuilder CreateStationDescription()
00069: 	{
00070: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00071: 		List<StationFeature> allBuildingFeature = GetAllBuildingFeature();
00072: 		List<StationFeature> allNonBuildingFeature = GetAllNonBuildingFeature();
00073: 		if (allBuildingFeature.Count > 0 || allNonBuildingFeature.Count > 0)
00074: 		{
00075: 			richTextBuilder.AddLine("From the comms chatter, you were able to pickup the following information:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00076: 		}
00077: 		if (allBuildingFeature.Count > 0)
00078: 		{
00079: 			richTextBuilder.Add("   This Station have a ");
00080: 			foreach (StationFeature item in allBuildingFeature)
00081: 			{
00082: 				if (item == allBuildingFeature.Last())
00083: 				{
00084: 					richTextBuilder.AddLine(item.displayName + ". ", ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
00085: 				}
00086: 				else
00087: 				{
00088: 					richTextBuilder.Add(item.displayName + ", ", RenderHandler.ColorPalette.LightTurquoise);
00089: 				}
00090: 			}
00091: 		}
00092: 		if (allNonBuildingFeature.Count > 0)
00093: 		{
00094: 			richTextBuilder.AddLine("   This Station is experiencing a ");
00095: 			foreach (StationFeature item2 in allNonBuildingFeature)
00096: 			{
00097: 				if (item2 == allNonBuildingFeature.Last())
00098: 				{
00099: 					richTextBuilder.AddLine(item2.displayName + ". ", ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
00100: 				}
00101: 				else
00102: 				{
00103: 					richTextBuilder.Add(item2.displayName + ", ", RenderHandler.ColorPalette.LightTurquoise);
00104: 				}
~~~

## CharacterItemDirectory.cs

~~~csharp
00056: 		if (allegiance == Allegiance.Friendly)
00057: 		{
00058: 			gci.EnableTag("friendly");
00059: 		}
00060: 		gci.onItemTargetSlotItemFunc = delegate(GameItem otherItem, GameItem thisItem, GameInventory thisInventory, SlotMarker thisSlot)
00061: 		{
00062: 			if (!GameMaster.current.InCombat() && otherItem.IsTag("butchering") && thisItem is GameCharacterItem { isDead: not false } gameCharacterItem)
00063: 			{
00064: 				thisItem.TryUnloadAll();
00065: 				if (gameCharacterItem.IsTag("SPECIES_HUMAN"))
00066: 				{
00067: 					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("raw_meat").SetAmount(2));
00068: 					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("small_raw_meat").SetAmount(4));
00069: 					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("fat_meat").SetAmount(1));
00070: 				}
00071: 				else if (gameCharacterItem.IsTag("SPECIES_XENO"))
00072: 				{
00073: 					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("raw_xeno_meat").SetAmount(1));
00074: 					GraphUtils.TryAcceptAll(thisInventory, DirectoryMaster.Item("small_raw_xeno_meat").SetAmount(2));
00075: 				}
00076: 				thisItem.Destroy();
00077: 			}
00078: 		};
00079: 		gci.tooltipRichTextFunc = delegate(GameItem item, GameInventory _, SlotMarker _, GridShape _, TagSystem _)
00080: 		{
00081: 			RichTextBuilder richTextBuilder = new RichTextBuilder(40);
00082: 			if (item is GameCharacterItem gameCharacterItem)
00083: 			{
00084: 				richTextBuilder.AddLine(item.name + " (" + gameCharacterItem.GetTagReadonly("class_name").valueString + ")", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
00085: 				richTextBuilder.Add("Health: ", RenderHandler.ColorPalette.LightRed);
00086: 				richTextBuilder.AddProgressBar((float)gameCharacterItem.health / (float)gameCharacterItem.maxHealth, 10, RenderHandler.ColorPalette.LightRed, RenderHandler.ColorPalette.VeryDarkRed, null, " ");
00087: 				richTextBuilder.AddLine("(" + gameCharacterItem.health + "/" + (float)gameCharacterItem.maxHealth + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
00088: 				int num = 0;
00089: 				int num2 = 0;
00090: 				(num, num2) = gameCharacterItem.GetTotalCurrentArmorValue();
00091: 				if (num2 > 0)
00092: 				{
00093: 					richTextBuilder.Add(" Armor: ", RenderHandler.ColorPalette.LightCyan);
00094: 					richTextBuilder.AddProgressBar((float)num / (float)num2, 10, RenderHandler.ColorPalette.LightCyan, RenderHandler.ColorPalette.VeryDarkCyan, null, " ");
00095: 					richTextBuilder.AddLine("(" + num + "/" + (float)num2 + ")", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
00096: 				}
00097: 				richTextBuilder.Add("Morale: ", RenderHandler.ColorPalette.LightPurple);
00098: 				richTextBuilder.AddProgressBar((float)gameCharacterItem.currentMorale / (float)gameCharacterItem.maxMorale, 10, RenderHandler.ColorPalette.LightPurple, RenderHandler.ColorPalette.DarkPurple, null, " ");
00099: 				richTextBuilder.AddLine("(" + gameCharacterItem.currentMorale + "%)", ignoreEmpty: true, RenderHandler.ColorPalette.LightPurple);
00100: 				richTextBuilder.Add("Thirst: ", RenderHandler.ColorPalette.LightTurquoise);
00101: 				richTextBuilder.AddProgressBar((float)gameCharacterItem.currentThirst / (float)gameCharacterItem.maxThirst, 10, RenderHandler.ColorPalette.LightTurquoise, RenderHandler.ColorPalette.DarkTurquoise, null, " ");
00102: 				richTextBuilder.AddLine("(" + gameCharacterItem.currentThirst / 10 + "%)", ignoreEmpty: true, RenderHandler.ColorPalette.LightTurquoise);
00103: 				richTextBuilder.AddLine(" ");
00104: 				richTextBuilder.AddLine("Accuracy     : " + gameCharacterItem.accuracyPercentage + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightCyan);
00105: 				richTextBuilder.AddLine("Crital Chance: " + gameCharacterItem.criticalChanceDealing + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
00106: 				richTextBuilder.AddLine("Dodge        : " + gameCharacterItem.dodge + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue);
00107: 				richTextBuilder.AddLine("Death Resist : " + (100 - gameCharacterItem.deathchance) + "%", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
00108: 				richTextBuilder.AddLine("Priority     : " + gameCharacterItem.priority, ignoreEmpty: true, RenderHandler.ColorPalette.Yellow);
00109: 				richTextBuilder.AddLine("Reaction     : " + gameCharacterItem.reaction, ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00110: 				richTextBuilder.AddLine(" ");
00111: 				richTextBuilder.AddLine(gameCharacterItem.shortDescription);
00112: 				if (gameCharacterItem.abilityEffects.Any())
00113: 				{
00114: 					richTextBuilder.NextLine().AddLine("Status: ");
00115: 				}
~~~

## SpaceButcherTrigger.cs

~~~csharp
00023: 		trader.BarterEnable(TradeSheet.GetXenoButcherShop());
00024: 		List<string> tagList = new List<string> { "XENO_MEAT_TAG" };
00025: 		List<int> discountList = new List<int> { -20 };
00026: 		List<string> itemTags = new List<string> { "power_source_item" };
00027: 		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
00028: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
00029: 		eventData.startAction = delegate
00030: 		{
00031: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_BUTCHER_EVENT_STARTED");
00032: 			tag.SetInt(tag.GetInt() + 1);
00033: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00034: 			{
00035: 				overworldSpace.isEventTriggered = true;
00036: 			}
00037: 		};
00038: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "(Browse Shop)", "", null, null, delegate
00039: 		{
00040: 			trader.BarterShow();
00041: 		});
00042: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00043: 		{
00044: 		});
00045: 		bool questionAsked = false;
00046: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask what are xeno meat and why anyone would buy it.", "", null, null, delegate
00047: 		{
00048: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The butcher is annoyed by your question, but still anwser to it:  'Those meat are highly sought after because some believe that it will enhance their bedroom prowess, other believe it has unique medicinal proprieties, many buy it because it is expensive and eating it is demonstrate high social status. But I like it because of it's unique texture and flavor, it's a delicacy.  You have to be careful as this meat is banned in most station.'", RenderHandler.ColorPalette.LightGray));
00049: 			questionAsked = true;
00050: 		});
00051: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Why are xeno meat banned in most stations?", "", null, () => questionAsked, delegate
00052: 		{
00053: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add(" Because of how dangerous ranching xeno is. The xeno are very smart sooner or later a rancher will slip and make a mistake and the xenos will escape. It will hide on the station, ambush anyone alone, start a hive and reproduce until they have the numbers to take the whole station. It only take one xeno to overrun a station. If you ever stumble on a station infested with xeno, that's probably a xeno ranching operation gone wrong.'", RenderHandler.ColorPalette.LightGray));
00054: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add(" 'So they banned the xeno meat thinking it will reduce consumption and thus, lower demand and then the supply. In practice' demand for xeno meat has skyrocket after the ban, the meat has become even more desirable now that it is illegal. So if you can smuggle some in a station, you can flip those for big profit.'", RenderHandler.ColorPalette.LightGray));
00055: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add(" 'Anyway, are you gonna buy some or what?'", RenderHandler.ColorPalette.LightGray));
00056: 		});
00057: 		return eventData;
00058: 	}
00059: }
~~~

## Base.cs

~~~csharp
00081: 	}
00082: 
00083: 	public bool CanUpgrade()
00084: 	{
00085: 		if (currentUpgrade >= upgrades.Count())
00086: 		{
00087: 			return false;
00088: 		}
00089: 		return upgrades[currentUpgrade].CanCraft(GetAllItems());
00090: 	}
00091: 
00092: 	public void Upgrade()
00093: 	{
00094: 		if (CanUpgrade())
00095: 		{
00096: 			upgrades[currentUpgrade].Craft(GetAllItems());
00097: 			GameMaster.current.shipScene.Validate();
00098: 			currentUpgrade++;
00099: 		}
00100: 	}
00101: 
00102: 	public RichTextBuilder GetUpgradeRichText()
00103: 	{
00104: 		if (currentUpgrade >= upgrades.Count())
00105: 		{
00106: 			return new RichTextBuilder().Add("No more upgrades available");
00107: 		}
00108: 		return upgrades[currentUpgrade].GetCraftingDetails(GetAllItems());
00109: 	}
00110: 
00111: 	public void Validate()
00112: 	{
00113: 	}
00114: 
00115: 	public bool MayHaveValidInventorySlot(GameItem item)
00116: 	{
00117: 		foreach (GraphNodeStorage child in children)
00118: 		{
00119: 			if (child.MayHaveValidInventorySlot(item))
00120: 			{
00121: 				return true;
00122: 			}
00123: 		}
00124: 		return false;
00125: 	}
00126: 
00127: 	public SlotMarker TryFindOneValidInventorySlot(GameItem item)
00128: 	{
00129: 		foreach (GraphNodeStorage child in children)
00130: 		{
00131: 			SlotMarker slotMarker = child.TryFindOneValidInventorySlot(item);
00132: 			if (slotMarker.IsValid())
00133: 			{
00134: 				return slotMarker;
00135: 			}
00136: 		}
00137: 		return null;
00138: 	}
00139: 
00140: 	public bool AddParent(GraphNodeStorage node)
~~~

## MenuManager.cs

~~~csharp
00065: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("brace_item_ability"));
00066: 		list.Add(gameCharacterItem);
00067: 		gameCharacterItem = DirectoryMaster.CharacterItem("daniel");
00068: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("smoke_grenade"));
00069: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("green_injector"));
00070: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("armor_lining"));
00071: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("overwatch"));
00072: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("mark_item_ability"));
00073: 		GraphUtils.TryAcceptAll(gameCharacterItem, DirectoryMaster.Item("backpack_small"));
00074: 		gameCharacterItem.isPlayerControlled = true;
00075: 		list.Add(gameCharacterItem);
00076: 		list.ForEach(delegate(GameItem gci)
00077: 		{
00078: 			GraphUtils.TryAcceptAll(gci, DirectoryMaster.Item("wait"));
00079: 		});
00080: 		return list;
00081: 	}
00082: 
00083: 	public List<Scenario> GetAvailableFactionScenarios()
00084: 	{
00085: 		List<Scenario> list = new List<Scenario>();
00086: 		Scenario scenario = new Scenario
00087: 		{
00088: 			ship = DirectoryMaster.Item("cargo_ship"),
00089: 			totalPoints = 5000L,
00090: 			richTextDescription = new RichTextBuilder().AddLine("Sok-27C", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Cargo Ship", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
00091: 				.AddLine("Sokolov Heavy Industry", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed)
00092: 				.AddLine(" ")
00093: 				.AddLine("  This ship is the cargo variant of the Sok-27. A popular ship for young entreprising captain. It's low price tag made the entry cost to space freight accessible. While the ship can officialy seat 3 person, 2 two passenger seats are cramped and uncomfortable. While having a smaller cargo hold than most other cargo ship. The Sok-27C require little to none port infrastructure to operate in. It is often seen transporting vital supplies to high risk area during shortage.The Sok-27C captain are often joked as opportunist in the freight industry.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
00094: 				.AddLine(" ")
00095: 				.Add("  Starting Difficulty: ")
00096: 				.AddLine("Normal", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange)
00097: 				.Add("  Unique Feature: ")
00098: 				.AddLine("Large Cargo Hold, Good Versatility", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00099: 				.Add("  Playstyle: ")
00100: 				.AddLine("Merchant, Smuggler, Offloader", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange)
00101: 		};
00102: 		ShipHelper.EquipCargoShip(scenario.ship);
00103: 		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
00104: 		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
00105: 		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
00106: 		list.Add(scenario);
00107: 		scenario = new Scenario
00108: 		{
00109: 			ship = DirectoryMaster.Item("military_transport_ship"),
00110: 			richTextDescription = new RichTextBuilder().AddLine("A-101 Albatross", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Military Transport Shuttle", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
00111: 				.AddLine("Republic Navy", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00112: 				.AddLine(" ")
00113: 				.AddLine("  Originally design for boarding operation The Albatross is lightly armed and armored. The republic extensively used the A-101 in anti-piracy activities. Launched from larger ships, a couple of A-101 loaded with marines are enough to handle a pirate ship.The sight of A-101 is a pirate worst's nightmare. With the Albatross inital performance, the Navy plan to retire the older and heavier A-95 Condor.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
00114: 				.AddLine(" ")
00115: 				.Add("  Starting Difficulty: ")
00116: 				.AddLine("Easy", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen)
00117: 				.Add("  Unique Feature: ")
00118: 				.AddLine("Intimidating Presence, High Operating Cost", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00119: 				.Add("  Playstyle: ")
00120: 				.AddLine("Fighter, Passenger Transport", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
00121: 			totalPoints = 5000L
00122: 		};
00123: 		ShipHelper.EquipMilitaryTransportShip(scenario.ship);
00124: 		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
~~~

~~~csharp
00106: 		list.Add(scenario);
00107: 		scenario = new Scenario
00108: 		{
00109: 			ship = DirectoryMaster.Item("military_transport_ship"),
00110: 			richTextDescription = new RichTextBuilder().AddLine("A-101 Albatross", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Military Transport Shuttle", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
00111: 				.AddLine("Republic Navy", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00112: 				.AddLine(" ")
00113: 				.AddLine("  Originally design for boarding operation The Albatross is lightly armed and armored. The republic extensively used the A-101 in anti-piracy activities. Launched from larger ships, a couple of A-101 loaded with marines are enough to handle a pirate ship.The sight of A-101 is a pirate worst's nightmare. With the Albatross inital performance, the Navy plan to retire the older and heavier A-95 Condor.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
00114: 				.AddLine(" ")
00115: 				.Add("  Starting Difficulty: ")
00116: 				.AddLine("Easy", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen)
00117: 				.Add("  Unique Feature: ")
00118: 				.AddLine("Intimidating Presence, High Operating Cost", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00119: 				.Add("  Playstyle: ")
00120: 				.AddLine("Fighter, Passenger Transport", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
00121: 			totalPoints = 5000L
00122: 		};
00123: 		ShipHelper.EquipMilitaryTransportShip(scenario.ship);
00124: 		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
00125: 		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
00126: 		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
00127: 		list.Add(scenario);
00128: 		scenario = new Scenario
00129: 		{
00130: 			ship = DirectoryMaster.Item("utility_ship"),
00131: 			richTextDescription = new RichTextBuilder().AddLine("Type-54 Bian Yuan", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Utility Ship", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
00132: 				.AddLine("Zhonghua Shipbuilding", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange)
00133: 				.AddLine(" ")
00134: 				.AddLine("  A ship designed for industrial operations. Commonly used in mining, salvaging and construction operation.Mass produced by Zhonghua with a streamlined spare part logistic and service network, this made operating and maintning the Type-54 incredibly easy. This made the Type-54 a direct competitor to the Sok-27U which offer similar price and capabilities. Some independant captains took a liking to the Type-54. They found that the ship's hull is rugged and durable and the ship's system is reliable.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
00135: 				.AddLine(" ")
00136: 				.Add("  Starting Difficulty: ")
00137: 				.AddLine("Easy", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen)
00138: 				.Add("  Unique Feature: ")
00139: 				.AddLine("Rugged Hull, External Slots", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00140: 				.Add("  Playstyle: ")
00141: 				.AddLine("Miner, Scavenger, Self-Sufficient", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
00142: 			totalPoints = 5000L
00143: 		};
00144: 		ShipHelper.EquipUtilityShip(scenario.ship);
00145: 		scenario.selectableCharacters = new GameGridScrollableInventory(10, 20);
00146: 		scenario.selectableCharacters.AddFastUnchecked(GetDefaultCharacters());
00147: 		scenario.selectableItems = new GameGridScrollableInventory(10, 20);
00148: 		list.Add(scenario);
00149: 		scenario = new Scenario
00150: 		{
00151: 			ship = DirectoryMaster.Item("refugee_ship"),
00152: 			richTextDescription = new RichTextBuilder().AddLine("The Bus", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true, italic: false, underline: true).AddLine("  Makeshift Transport Ship", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue).Add("  Designer: ")
00153: 				.AddLine("Spacer", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00154: 				.AddLine(" ")
00155: 				.AddLine("  Affectionately named by it's crew, The Bus is a type of barely spaceworthy ship weld together by determined spacers in search of a better live. Made from any materal those spacers can lay their hand on. Usually it is made from cargo container, fuel tank and a lot of low grade scrap metal and come in all shapes and sizes. The hull is fragile and but due to its makeshift nature, it is fairly simple to repair. Those makeshift ships transport all kind of people, from political refugee to wanted criminal. This is the cheapest ride one can find. The Spacers have a code of honor between them and a Bus must always help another Bus if met in deep space.", ignoreEmpty: true, RenderHandler.ColorPalette.White)
00156: 				.AddLine(" ")
00157: 				.Add("  Starting Difficulty: ")
00158: 				.AddLine("Hard", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed)
00159: 				.Add("  Unique Feature: ")
00160: 				.AddLine("Makeshift Hull, Low Operating Cost", ignoreEmpty: true, RenderHandler.ColorPalette.LightBlue)
00161: 				.Add("  Playstyle: ")
00162: 				.AddLine("Refugee, Passenger Transport", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange),
00163: 			totalPoints = 5000L
00164: 		};
00165: 		ShipHelper.EquipRefugeeShip(scenario.ship);
~~~

## TestCoinEventTrigger.cs

~~~csharp
00006: 	{
00007: 		if (GameMaster.current.worldTags.GetTag("TEST_COIN_EVENT_STARTED").GetInt() > 1)
00008: 		{
00009: 			return false;
00010: 		}
00011: 		if (GameMaster.current.shipManager.ship.FindAllChildItems().FindAll((GameItem i) => i.identifier == "coin").Any())
00012: 		{
00013: 			return true;
00014: 		}
00015: 		return null;
00016: 	}
00017: 
00018: 	public override EventData GenerateEvent()
00019: 	{
00020: 		EventData e = new EventData().SetText("Test Event", "Hello, this is a test event!");
00021: 		e.startAction = delegate
00022: 		{
00023: 			TagState tag = GameMaster.current.worldTags.GetTag("TEST_COIN_EVENT_STARTED");
00024: 			tag.SetInt(tag.GetInt() + 1);
00025: 		};
00026: 		e.endTurn = 3;
00027: 		e.turnAction = delegate(EventData eventData)
00028: 		{
00029: 			if (eventData.startTurn != eventData.turns)
00030: 			{
00031: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Event is almost ended! " + (eventData.endTurn - eventData.turns) + " turns left!", RenderHandler.ColorPalette.LightRed));
00032: 			}
00033: 		};
00034: 		e.endAction = delegate
00035: 		{
00036: 			GameMaster.current.shipManager.ship?.TryAcceptAll(DirectoryMaster.Item("green_injector").SetAmount(1));
00037: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("A green injector has appeared in the ship's cargo!", RenderHandler.ColorPalette.LightGreen));
00038: 		};
00039: 		e.AddChoice(DirectoryMaster.Item("coin"), "Give me some money!", "", null, null, delegate
00040: 		{
00041: 			GameMaster.current.shipManager.ship?.TryAcceptAll(DirectoryMaster.Item("coin").SetAmount(5));
00042: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Some coins has appeared in the ship's cargo!", RenderHandler.ColorPalette.LightGreen));
00043: 			e.ForceEnd();
00044: 			EventData eventData = new EventData().SetText("Chained event", "This is the event that runs after spawning some money!");
00045: 			GameMaster.current.shipManager.SetEvent(eventData);
00046: 		});
00047: 		return e;
00048: 	}
00049: }
~~~

## ItemSelectHandler.cs

~~~csharp
00008: 	public static ItemSelectHandler current;
00009: 
00010: 	[SerializeField]
00011: 	public KeyCode key;
00012: 
00013: 	[SerializeField]
00014: 	public Vector2 tooltipTranslation = Vector2.op_Implicit(new Vector3(32f, 0f));
00015: 
00016: 	[SerializeField]
00017: 	public Vector2 equipCursorTranslation = new Vector2(-12f, -12f);
00018: 
00019: 	public bool auto = true;
00020: 
00021: 	private GameItem currentItem;
00022: 
00023: 	private HighlightManager highlightManager = new HighlightManager();
00024: 
00025: 	private List<TreeNodeRender> highlightedMayNodes = new List<TreeNodeRender>();
00026: 
00027: 	private bool isEquipped;
00028: 
00029: 	private TreeNodeRender equipCursor;
00030: 
00031: 	private readonly List<TreeNodeRender> equipCursorOverlays = new List<TreeNodeRender>();
00032: 
00033: 	private RichTextBuilder builder = new RichTextBuilder(60);
00034: 
00035: 	private GameItem lastItem;
00036: 
00037: 	public override List<KeyCode> keyListeners => ((IEnumerable<KeyCode>)(object)new KeyCode[2]
00038: 	{
00039: 		(KeyCode)(int)key,
00040: 		(KeyCode)324
00041: 	}).ToList();
00042: 
00043: 	public bool IsEquipped => isEquipped;
00044: 
00045: 	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
00046: 	private void Awake()
00047: 	{
00048: 		current = this;
00049: 	}
00050: 
00051: 	public GameItem GetEquippedItem()
00052: 	{
00053: 		if (!isEquipped)
00054: 		{
00055: 			return null;
00056: 		}
00057: 		return currentItem;
00058: 	}
00059: 
00060: 	public void TrySelectItem(GameItem newItem)
00061: 	{
00062: 		if (newItem != null && currentItem == null && newItem.MaySelectSlotItem() && newItem.CanSelectSlotItem())
00063: 		{
00064: 			SelectItem(newItem);
00065: 		}
00066: 	}
00067: 
~~~

~~~csharp
00401: 				{
00402: 					builder.Add("Use and activate ").Add(displayName, RenderHandler.ColorPalette.None, "", bold: true, italic: false, underline: true);
00403: 				}
00404: 				else if (gameItem.MayTarget(gameItem2) && gameItem.CanTarget(gameItem2))
00405: 				{
00406: 					if (TooltipUseHelper.HasUseTooltip(gameItem, gameItem2))
00407: 					{
00408: 						TooltipUseHelper.HandleUseTooltip(gameItem, gameItem2, builder);
00409: 					}
00410: 					else
00411: 					{
00412: 						builder.Add(LocHelper.GetLocalizedMechanic("mech_drag_use_on", "<b>" + displayName + "</b>", "<b><u>" + displayName2 + "</u></b>"));
00413: 					}
00414: 					AbilityItemHelper.CreateHoverTargetTooltip(builder, currentItem, gameItem2);
00415: 				}
00416: 				else if (gameItem != gameItem2)
00417: 				{
00418: 					builder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_use_on", displayName, displayName2), RenderHandler.ColorPalette.Red);
00419: 				}
00420: 				else if (gameItem == gameItem2 && gameItem.MayActivateSlotItem() && !gameItem.CanActivateSlotItem())
00421: 				{
00422: 					builder.Add("Cannot activate " + displayName + "!", RenderHandler.ColorPalette.Red);
00423: 				}
00424: 				if (!builder.IsEmpty())
00425: 				{
00426: 					OverlayHandler.current.ShowTooltip(Vector2.op_Implicit(Input.mousePosition), tooltipTranslation, builder.Build());
00427: 				}
00428: 				else
00429: 				{
00430: 					OverlayHandler.current.HideTooltip();
00431: 				}
00432: 			}
00433: 			else
00434: 			{
00435: 				OverlayHandler.current.HideTooltip();
00436: 			}
00437: 		}
00438: 		return currentItem != null;
00439: 	}
00440: 
00441: 	protected override void OnEventReset()
00442: 	{
00443: 		if (isEquipped && currentItem != null)
00444: 		{
00445: 			AudioManager.Instance.TryPlayOne(currentItem.soundDragEnd, currentItem.soundDragStart);
00446: 		}
00447: 		highlightManager.Clear();
00448: 		if (currentItem != null)
00449: 		{
00450: 			OverlayHandler.current.HideTooltip();
00451: 		}
00452: 		currentItem = null;
00453: 		ClearMayHighlights();
00454: 		isEquipped = false;
00455: 		HideEquipCursor();
00456: 	}
00457: 
00458: 	public void EndSelect(bool nonOwnedOnly = false)
00459: 	{
00460: 		if (nonOwnedOnly)
~~~

## ItemUseBehaviour.cs

~~~csharp
00018: 		return MayUse(source) ?? (CanUse(source) == true);
00019: 	}
00020: 
00021: 	public bool CheckCan(GameItem source)
00022: 	{
00023: 		bool? flag = MayUse(source);
00024: 		if ((!flag) ?? false)
00025: 		{
00026: 			return false;
00027: 		}
00028: 		return CanUse(source) ?? (flag == true);
00029: 	}
00030: 
00031: 	public RichTextBuilder GetTooltip(GameItem source)
00032: 	{
00033: 		if ((!ShowTooltip(source)) ?? false)
00034: 		{
00035: 			return null;
00036: 		}
00037: 		RichTextBuilder richTextBuilder = UseRichTooltip(source);
00038: 		if (richTextBuilder == null)
00039: 		{
00040: 			string text = UseTooltip(source);
00041: 			if (text != null)
00042: 			{
00043: 				return new RichTextBuilder().AddLine(text);
00044: 			}
00045: 		}
00046: 		return richTextBuilder;
00047: 	}
00048: 
00049: 	public virtual bool? Filter(GameItem source)
00050: 	{
00051: 		return null;
00052: 	}
00053: 
00054: 	public virtual bool? MayUse(GameItem source)
00055: 	{
00056: 		return null;
00057: 	}
00058: 
00059: 	public virtual bool? CanUse(GameItem source)
00060: 	{
00061: 		return null;
00062: 	}
00063: 
00064: 	public virtual bool? CanUsePassthrough(GameItem source)
00065: 	{
00066: 		return null;
00067: 	}
00068: 
00069: 	public virtual bool? OnUse(GameItem source)
00070: 	{
00071: 		return null;
00072: 	}
00073: 
00074: 	public virtual bool? ShowTooltip(GameItem source)
00075: 	{
00076: 		return null;
00077: 	}
~~~

## SpaceDrugShipTrigger.cs

~~~csharp
00019: 		eventData.text.AddLine("\r\nYou are contacted by their ship's captain, welcoming you and ask if you would like to browse their ware. They proudly annonce that everything on sale is made locally on their ship.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00020: 		eventData.text.AddLine("\r\nThey only accept Energy Credit as currency.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00021: 		eventData.startAction = delegate
00022: 		{
00023: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DRUG_LAB_EVENT_STARTED");
00024: 			tag.SetInt(tag.GetInt() + 1);
00025: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00026: 			{
00027: 				overworldSpace.isEventTriggered = true;
00028: 			}
00029: 		};
00030: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00031: 		trader.name = "Drug Lab (Credit Only)";
00032: 		trader.BarterEnable(TradeSheet.GetDrugShipShop());
00033: 		List<string> tagList = new List<string> { "ITEM_MEDICAL_TAG" };
00034: 		List<int> discountList = new List<int> { -50 };
00035: 		List<string> itemTags = new List<string> { "power_source_item" };
00036: 		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
00037: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
00038: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "Browse wares.", "", null, null, delegate
00039: 		{
00040: 			trader.BarterShow();
00041: 		});
00042: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Ask for tip getting contraband past security checkpoint in the trade stations.", "", null, null, delegate
00043: 		{
00044: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nblablabal", RenderHandler.ColorPalette.LightGray));
00045: 		});
00046: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00047: 		{
00048: 		});
00049: 		return eventData;
00050: 	}
00051: }
~~~

## SpaceBattleHelper.cs

~~~csharp
00001: public class SpaceBattleHelper
00002: {
00003: 	public static bool TakingHit(int damage)
00004: 	{
00005: 		var (flag, num, _, num2) = GameMaster.current.shipManager.DamageHullVerbose(damage);
00006: 		if (num2 > 0)
00007: 		{
00008: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship was hit and took " + num2 + " damage!. It now have a integrity of " + num + ".", RenderHandler.ColorPalette.LightRed));
00009: 		}
00010: 		else
00011: 		{
00012: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship was hit but thanks to its armor, it wasn't damaged at all. It still have a integrity of " + num + ".", RenderHandler.ColorPalette.LightGreen));
00013: 		}
00014: 		if (!flag)
00015: 		{
00016: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship escapes! You may now select another destionation from the Navigation System.", RenderHandler.ColorPalette.LightGreen));
00017: 			return false;
00018: 		}
00019: 		OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your ship has taken critical amount of damage. Your ship is destroyed!", RenderHandler.ColorPalette.LightRed));
00020: 		return true;
00021: 	}
00022: }
~~~

## SpaceNavySupplyTrigger.cs

~~~csharp
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData eventData = new EventData().SetText("Republic Navy Supply Ship", "You have arrived at a lone autonomous supply ship with the Republic marking.");
00016: 		eventData.text.AddLine("\r\nThis type of supply ship are known to carry military grade supplies and equipments. They are fully autonomous and carry no crew.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00017: 		eventData.text.AddLine("\r\nTheir onboard ship AI contacts you and warn you that they are part of the Republic Navy any hostile action will trigger retaliation from the Republic Navy.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
00018: 		eventData.startAction = delegate
00019: 		{
00020: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_NAVY_SUPPLY_EVENT_STARTED");
00021: 			tag.SetInt(tag.GetInt() + 1);
00022: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00023: 			{
00024: 				overworldSpace.isEventTriggered = true;
00025: 			}
00026: 		};
00027: 		eventData.AddChoice(DirectoryMaster.Item("open_fire"), "Open fire on the drone ship.", "", null, null, delegate
00028: 		{
00029: 		});
00030: 		GameItem container = DirectoryMaster.Item("lootbox_medium");
00031: 		bool cardUsed = false;
00032: 		eventData.AddChoice(DirectoryMaster.Item("keycard"), "[Republic Keycard] Use Keycard.", "", () => GameMaster.current.shipManager.IsShipHaveItem("republic_keycard") && !cardUsed, null, delegate
00033: 		{
00034: 			GameMaster.current.shipManager.RemoveItemFromShip("republic_keycard");
00035: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'Republic Supply Ship Ai: Level X access detected, unlocking access...", RenderHandler.ColorPalette.LightGray));
00036: 			GameItem item;
00037: 			for (int i = 0; i < 1; i++)
00038: 			{
00039: 				item = DirectoryMaster.Item("energy_credit");
00040: 				GraphUtils.TryAcceptAll(container, item);
00041: 			}
00042: 			item = DirectoryMaster.Item("armor_titan10");
00043: 			GraphUtils.TryAcceptAll(container, item);
00044: 			if (RNG.Roll(5))
00045: 			{
00046: 				GraphUtils.TryAcceptAll(container, DirectoryMaster.Item("republic_keycard"));
00047: 			}
00048: 			OverlayHandler.current.ShowInventory(container);
00049: 			cardUsed = true;
00050: 		});
00051: 		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Loot.", "", null, () => cardUsed, delegate
00052: 		{
00053: 			OverlayHandler.current.ShowInventory(container);
00054: 		});
00055: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00056: 		{
00057: 		});
00058: 		return eventData;
00059: 	}
00060: }
~~~

## TraderStationInspectionTrigger.cs

~~~csharp
00023: 		e.startAction = delegate(EventData eventData)
00024: 		{
00025: 			eventData.BlockStation();
00026: 			TagState tag = GameMaster.current.worldTags.GetTag("TRADER_STATION_INSPECTION_STARTED");
00027: 			tag.SetInt(tag.GetInt() + 1);
00028: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00029: 			{
00030: 				overworldSpace.isEventTriggered = true;
00031: 			}
00032: 		};
00033: 		bool refusedInspection = false;
00034: 		e.AddChoice(DirectoryMaster.Item("$"), "(Bribe) Offer 100 Energy Credit to avoid inspection.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 100 && !refusedInspection, null, delegate
00035: 		{
00036: 			OfferSmallBribe(e);
00037: 		});
00038: 		e.AddChoice(DirectoryMaster.Item("$$"), "(Bribe) Offer 200 Energy Credit to avoid inspection.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 200 && !refusedInspection, null, delegate
00039: 		{
00040: 			OfferLargeBribe(e);
00041: 		});
00042: 		e.AddChoice(DirectoryMaster.Item("ok"), "Accept the inspection", "", () => !refusedInspection, null, delegate
00043: 		{
00044: 			ConductInspection(e);
00045: 		});
00046: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Refuse the inspection and leave the station.", "", null, null, delegate
00047: 		{
00048: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You refuse the inspection and leave quickly before the Station's border service can pursuit you.", RenderHandler.ColorPalette.LightYellow));
00049: 			refusedInspection = true;
00050: 		});
00051: 		return e;
00052: 	}
00053: 
00054: 	public static void ConductInspection(EventData e)
00055: 	{
00056: 		OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier arrived on your ship and begin the inspection... ", RenderHandler.ColorPalette.LightYellow));
00057: 		var (num, num2) = GameMaster.current.shipManager.InspectCargoHold();
00058: 		if (num == 0)
00059: 		{
00060: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The inspection is finished and no contraband was found, you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00061: 		}
00062: 		else
00063: 		{
00064: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The inspection is finished and " + num + " contrabands were found!", RenderHandler.ColorPalette.LightRed));
00065: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The countrabands items are confiscated and you are order to be fined " + num2 + " Credit for attempting to smuggle contraband!", RenderHandler.ColorPalette.LightRed));
00066: 			if (GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= num2)
00067: 			{
00068: 				GameMaster.current.shipManager.RemoveEnergyCredit(num2);
00069: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You paid the fine and you are now clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00070: 			}
00071: 			else
00072: 			{
00073: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You do not have enough Credit to paid the fine. Your crew was arrested and jailed by the Station's security force.", RenderHandler.ColorPalette.LightRed));
00074: 			}
00075: 		}
00076: 		e.blockStation = false;
00077: 	}
00078: 
00079: 	public static void OfferSmallBribe(EventData e)
00080: 	{
00081: 		bool flag = false;
00082: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
~~~

~~~csharp
00048: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You refuse the inspection and leave quickly before the Station's border service can pursuit you.", RenderHandler.ColorPalette.LightYellow));
00049: 			refusedInspection = true;
00050: 		});
00051: 		return e;
00052: 	}
00053: 
00054: 	public static void ConductInspection(EventData e)
00055: 	{
00056: 		OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier arrived on your ship and begin the inspection... ", RenderHandler.ColorPalette.LightYellow));
00057: 		var (num, num2) = GameMaster.current.shipManager.InspectCargoHold();
00058: 		if (num == 0)
00059: 		{
00060: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The inspection is finished and no contraband was found, you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00061: 		}
00062: 		else
00063: 		{
00064: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The inspection is finished and " + num + " contrabands were found!", RenderHandler.ColorPalette.LightRed));
00065: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The countrabands items are confiscated and you are order to be fined " + num2 + " Credit for attempting to smuggle contraband!", RenderHandler.ColorPalette.LightRed));
00066: 			if (GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= num2)
00067: 			{
00068: 				GameMaster.current.shipManager.RemoveEnergyCredit(num2);
00069: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You paid the fine and you are now clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00070: 			}
00071: 			else
00072: 			{
00073: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   You do not have enough Credit to paid the fine. Your crew was arrested and jailed by the Station's security force.", RenderHandler.ColorPalette.LightRed));
00074: 			}
00075: 		}
00076: 		e.blockStation = false;
00077: 	}
00078: 
00079: 	public static void OfferSmallBribe(EventData e)
00080: 	{
00081: 		bool flag = false;
00082: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
00083: 		{
00084: 			flag = overworldStation.generatedStation.IsStationHaveCrisis();
00085: 		}
00086: 		if (flag)
00087: 		{
00088: 			if (RNG.Roll(5))
00089: 			{
00090: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Even considering the ongoing economic hardship on their station, the custom officier still refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00091: 				ConductInspection(e);
00092: 			}
00093: 			else
00094: 			{
00095: 				GameMaster.current.shipManager.RemoveEnergyCredit(100);
00096: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Considering the ongoing crisis on their station, the custom officier quickly accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00097: 				e.blockStation = false;
00098: 			}
00099: 		}
00100: 		else if (RNG.Roll(20))
00101: 		{
00102: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00103: 			ConductInspection(e);
00104: 		}
00105: 		else
00106: 		{
00107: 			GameMaster.current.shipManager.RemoveEnergyCredit(100);
~~~

~~~csharp
00077: 	}
00078: 
00079: 	public static void OfferSmallBribe(EventData e)
00080: 	{
00081: 		bool flag = false;
00082: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
00083: 		{
00084: 			flag = overworldStation.generatedStation.IsStationHaveCrisis();
00085: 		}
00086: 		if (flag)
00087: 		{
00088: 			if (RNG.Roll(5))
00089: 			{
00090: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Even considering the ongoing economic hardship on their station, the custom officier still refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00091: 				ConductInspection(e);
00092: 			}
00093: 			else
00094: 			{
00095: 				GameMaster.current.shipManager.RemoveEnergyCredit(100);
00096: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Considering the ongoing crisis on their station, the custom officier quickly accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00097: 				e.blockStation = false;
00098: 			}
00099: 		}
00100: 		else if (RNG.Roll(20))
00101: 		{
00102: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00103: 			ConductInspection(e);
00104: 		}
00105: 		else
00106: 		{
00107: 			GameMaster.current.shipManager.RemoveEnergyCredit(100);
00108: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00109: 			e.blockStation = false;
00110: 		}
00111: 	}
00112: 
00113: 	public static void OfferLargeBribe(EventData e)
00114: 	{
00115: 		bool flag = false;
00116: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
00117: 		{
00118: 			flag = overworldStation.generatedStation.IsStationHaveCrisis();
00119: 		}
00120: 		if (flag)
00121: 		{
00122: 			if (RNG.Roll(1))
00123: 			{
00124: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Even considering the ongoing economic hardship on their station, the custom officier still refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00125: 				ConductInspection(e);
00126: 			}
00127: 			else
00128: 			{
00129: 				GameMaster.current.shipManager.RemoveEnergyCredit(100);
00130: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Considering the ongoing crisis on their station, the custom officier quickly accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00131: 				e.blockStation = false;
00132: 			}
00133: 		}
00134: 		else if (RNG.Roll(10))
00135: 		{
00136: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
~~~

~~~csharp
00105: 		else
00106: 		{
00107: 			GameMaster.current.shipManager.RemoveEnergyCredit(100);
00108: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00109: 			e.blockStation = false;
00110: 		}
00111: 	}
00112: 
00113: 	public static void OfferLargeBribe(EventData e)
00114: 	{
00115: 		bool flag = false;
00116: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation overworldStation)
00117: 		{
00118: 			flag = overworldStation.generatedStation.IsStationHaveCrisis();
00119: 		}
00120: 		if (flag)
00121: 		{
00122: 			if (RNG.Roll(1))
00123: 			{
00124: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Even considering the ongoing economic hardship on their station, the custom officier still refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00125: 				ConductInspection(e);
00126: 			}
00127: 			else
00128: 			{
00129: 				GameMaster.current.shipManager.RemoveEnergyCredit(100);
00130: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   Considering the ongoing crisis on their station, the custom officier quickly accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00131: 				e.blockStation = false;
00132: 			}
00133: 		}
00134: 		else if (RNG.Roll(10))
00135: 		{
00136: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier refuse the bribe and is on route with a security team to conduct an inspection!", RenderHandler.ColorPalette.LightRed));
00137: 			ConductInspection(e);
00138: 		}
00139: 		else
00140: 		{
00141: 			GameMaster.current.shipManager.RemoveEnergyCredit(100);
00142: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("   The custom officier accept the bribe and you are clear to dock with the station.", RenderHandler.ColorPalette.LightGreen));
00143: 			e.blockStation = false;
00144: 		}
00145: 	}
00146: }
~~~

## AdvButtonElement.cs

~~~csharp
00381: 		spriteHighlight = RenderHandler.LoadFromAtlas(atlasPath, spriteName + "_hl");
00382: 		spriteClicked = RenderHandler.LoadFromAtlas(atlasPath, spriteName + "_c");
00383: 		spriteDisabled = RenderHandler.LoadFromAtlas(atlasPath, spriteName + "_d");
00384: 		ApplyCurrentState();
00385: 		return this;
00386: 	}
00387: 
00388: 	public AdvButtonElement SetDescription(string description)
00389: 	{
00390: 		this.description = description;
00391: 		return this;
00392: 	}
00393: 
00394: 	public override bool HasTooltip()
00395: 	{
00396: 		string obj = description;
00397: 		if (obj == null)
00398: 		{
00399: 			return false;
00400: 		}
00401: 		return obj.Length > 0;
00402: 	}
00403: 
00404: 	public override RichText GetTooltip()
00405: 	{
00406: 		return new RichTextBuilder().Add(description).Build();
00407: 	}
00408: }
~~~

## ButtonElement.cs

~~~csharp
00358: 
00359: 	protected override void OnShow()
00360: 	{
00361: 		((Behaviour)buttonNode.image).enabled = true;
00362: 		((Behaviour)textNode.text).enabled = true;
00363: 	}
00364: 
00365: 	public ButtonElement SetDescription(string description)
00366: 	{
00367: 		this.description = description;
00368: 		return this;
00369: 	}
00370: 
00371: 	public override bool HasTooltip()
00372: 	{
00373: 		string obj = description;
00374: 		if (obj == null)
00375: 		{
00376: 			return false;
00377: 		}
00378: 		return obj.Length > 0;
00379: 	}
00380: 
00381: 	public override RichText GetTooltip()
00382: 	{
00383: 		return new RichTextBuilder().Add(description).Build();
00384: 	}
00385: }
~~~

## ModHook.cs

~~~csharp
00039: 
00040: 	public static event Action OnGoingSleepLate;
00041: 
00042: 	public static event Action OnWakingUpEarly;
00043: 
00044: 	public static event Action OnWakingUpLate;
00045: 
00046: 	public static event Action OnLeavingStoreEarly;
00047: 
00048: 	public static event Action OnLeavingStoreLate;
00049: 
00050: 	public static event Action OnReturningStoreEarly;
00051: 
00052: 	public static event Action OnReturningStoreLate;
00053: 
00054: 	public static event Action OnHandlingNightlyServicesEarly;
00055: 
00056: 	public static event Action OnHandlingNightlyServicesLate;
00057: 
00058: 	public static event Action<List<GameItem>> OnPlaceInventorInventoryItemEarly;
00059: 
00060: 	public static event Action<List<GameItem>> OnPlaceInventorInventoryItemLate;
00061: 
00062: 	public static event Action<RichTextBuilder, GameItem> OnCreateTooltipEarly;
00063: 
00064: 	public static event Action<RichTextBuilder, GameItem> OnCreateTooltipLate;
00065: 
00066: 	public static event Action<RichTextBuilder, GameItem> OnCreateDevTooltipEarly;
00067: 
00068: 	public static event Action<RichTextBuilder, GameItem> OnCreateDevTooltipLate;
00069: 
00070: 	public static event Action<LoadedMod> OnModAssetsLoaded;
00071: 
00072: 	public static event Action<string> OnModConfigApplied;
00073: 
00074: 	public static event Action OnLootTablesLoaded;
00075: 
00076: 	public static event Action<LootRollContext> OnLootRolled;
00077: 
00078: 	public static event Action<string, PaperUI> OnPaperOpened;
00079: 
00080: 	public static event Action<string> OnNetworkUpgradeUnlocked;
00081: 
00082: 	public static event Action<string> OnNewGameStarted;
00083: 
00084: 	public static event Action<string> OnDifficultyApplied;
00085: 
00086: 	public static event Action<string> OnAfterhourArriveEarly;
00087: 
00088: 	public static event Action<string> OnAfterhourArriveLate;
00089: 
00090: 	public static event Action<string> OnAfterhourLeaveEarly;
00091: 
00092: 	public static event Action<string> OnAfterhourLeaveLate;
00093: 
00094: 	public static event Action<string, string> OnAfterhourEventStarted;
00095: 
00096: 	public static event Action<string, string> OnAfterhourEventEnded;
00097: 
00098: 	private void Awake()
~~~

~~~csharp
00316: 	internal static void FireOnReturningStoreLate()
00317: 	{
00318: 		SafeInvoke("OnReturningStoreLate", ModHook.OnReturningStoreLate);
00319: 	}
00320: 
00321: 	internal static void FireOnHandlingNightlyServicesEarly()
00322: 	{
00323: 		SafeInvoke("OnHandlingNightlyServicesEarly", ModHook.OnHandlingNightlyServicesEarly);
00324: 	}
00325: 
00326: 	internal static void FireOnHandlingNightlyServicesLate()
00327: 	{
00328: 		SafeInvoke("OnHandlingNightlyServicesLate", ModHook.OnHandlingNightlyServicesLate);
00329: 	}
00330: 
00331: 	internal static void FireOnPlaceInventorInventoryItemEarly(List<GameItem> items)
00332: 	{
00333: 		SafeInvoke("OnPlaceInventorInventoryItemEarly", ModHook.OnPlaceInventorInventoryItemEarly, items);
00334: 	}
00335: 
00336: 	internal static void FireOnPlaceInventorInventoryItemLate(List<GameItem> items)
00337: 	{
00338: 		SafeInvoke("OnPlaceInventorInventoryItemLate", ModHook.OnPlaceInventorInventoryItemLate, items);
00339: 	}
00340: 
00341: 	internal static void FireOnCreateTooltipEarly(RichTextBuilder builder, GameItem item)
00342: 	{
00343: 		SafeInvoke("OnCreateTooltipEarly", ModHook.OnCreateTooltipEarly, builder, item);
00344: 	}
00345: 
00346: 	internal static void FireOnCreateTooltipLate(RichTextBuilder builder, GameItem item)
00347: 	{
00348: 		SafeInvoke("OnCreateTooltipLate", ModHook.OnCreateTooltipLate, builder, item);
00349: 	}
00350: 
00351: 	internal static void FireOnCreateDevTooltipEarly(RichTextBuilder builder, GameItem item)
00352: 	{
00353: 		SafeInvoke("OnCreateDevTooltipEarly", ModHook.OnCreateDevTooltipEarly, builder, item);
00354: 	}
00355: 
00356: 	internal static void FireOnCreateDevTooltipLate(RichTextBuilder builder, GameItem item)
00357: 	{
00358: 		SafeInvoke("OnCreateDevTooltipLate", ModHook.OnCreateDevTooltipLate, builder, item);
00359: 	}
00360: 
00361: 	internal static void FireOnModAssetsLoaded(LoadedMod mod)
00362: 	{
00363: 		SafeInvoke("OnModAssetsLoaded", ModHook.OnModAssetsLoaded, mod);
00364: 	}
00365: 
00366: 	internal static void FireOnModConfigApplied(string modId)
00367: 	{
00368: 		SafeInvoke("OnModConfigApplied", ModHook.OnModConfigApplied, modId);
00369: 	}
00370: 
00371: 	internal static void FireOnLootTablesLoaded()
00372: 	{
00373: 		SafeInvoke("OnLootTablesLoaded", ModHook.OnLootTablesLoaded);
00374: 	}
00375: 
~~~

## TestTraderStationEventTrigger.cs

~~~csharp
00004: 	{
00005: 		if (GameMaster.current.worldTags.GetTag("TEST_TRADER_EVENT_STARTED").GetInt() > 0)
00006: 		{
00007: 			return false;
00008: 		}
00009: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation { isHostile: false })
00010: 		{
00011: 			_ = GameMaster.current.shipManager.isTransit;
00012: 		}
00013: 		return null;
00014: 	}
00015: 
00016: 	public override EventData GenerateEvent()
00017: 	{
00018: 		EventData eventData = new EventData().SetText("Trade Station Test Event", "Hello, this is a test event telling that you have arrived for the first time at a trade station!");
00019: 		eventData.startAction = delegate
00020: 		{
00021: 			TagState tag = GameMaster.current.worldTags.GetTag("TEST_TRADER_EVENT_STARTED");
00022: 			tag.SetInt(tag.GetInt() + 1);
00023: 		};
00024: 		eventData.endTurn = 3;
00025: 		eventData.turnAction = delegate(EventData e)
00026: 		{
00027: 			if (e.startTurn != e.turns)
00028: 			{
00029: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Event is almost ended! " + (e.endTurn - e.turns) + " turns left!", RenderHandler.ColorPalette.LightRed));
00030: 			}
00031: 		};
00032: 		return eventData;
00033: 	}
00034: }
~~~

## SpaceBanditTrigger.cs

~~~csharp
00006: 		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("bandit_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
00007: 		{
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData e = new EventData().SetText("Bandit Ship", "You have arrived near a unmarked military transport ship.");
00016: 		e.SetImage("raider");
00017: 		e.text.AddLine("\r\nThe ship's captain has contacted you and is demanding that you pay 400 Energy Credit for 'protection fee'.", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
00018: 		e.text.AddLine("\r\nScanner shows that the ship is armed", ignoreEmpty: true, RenderHandler.ColorPalette.LightOrange);
00019: 		e.startAction = delegate(EventData eventData)
00020: 		{
00021: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_BANDIT_EVENT_STARTED");
00022: 			eventData.BlockTravel();
00023: 			tag.SetInt(tag.GetInt() + 1);
00024: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00025: 			{
00026: 				overworldSpace.isEventTriggered = true;
00027: 			}
00028: 		};
00029: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now and request more time.", "", null, null, delegate
00030: 		{
00031: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit accepts and informs you that they cannot garantee your protection for much longer without paying the fee.", RenderHandler.ColorPalette.LightGreen));
00032: 		});
00033: 		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
00034: 		e.AddChoice(DirectoryMaster.Item("ok"), "(400 Energy Credit) Pay the bandit", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 400, null, delegate
00035: 		{
00036: 			GameMaster.current.shipManager.RemoveEnergyCredit(400);
00037: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit is satisfied and informs you that you may leave peacefully.", RenderHandler.ColorPalette.LightGreen));
00038: 			e.ForceEnd();
00039: 		});
00040: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Open fire on the bandit ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00041: 		{
00042: 		});
00043: 		e.AddChoice(DirectoryMaster.Item("ok"), "[Military Ship] Identify yourself as warship and demand that the bandit transfer you 400 Credit instead or otherwise you might not garantee their safety.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00044: 		{
00045: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("After realizing that they were trying to rob a warship, the bandit leader quickly transfer the credit before their ship jumps away.", RenderHandler.ColorPalette.LightGreen));
00046: 			GameItem gameItem = DirectoryMaster.Item("backpack_medium");
00047: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00048: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00049: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00050: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00051: 			OverlayHandler.current.ShowInventory(gameItem);
00052: 			e.ForceEnd();
00053: 		});
00054: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Refuse to pay and attempt to escape.", "", null, null, delegate
00055: 		{
00056: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away with your ship and the pirate has open fire on your ship!", RenderHandler.ColorPalette.LightRed));
00057: 			SpaceBattleHelper.TakingHit(10);
00058: 			e.ForceEnd();
00059: 		});
00060: 		return e;
00061: 	}
00062: }
~~~

~~~csharp
00031: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit accepts and informs you that they cannot garantee your protection for much longer without paying the fee.", RenderHandler.ColorPalette.LightGreen));
00032: 		});
00033: 		GameMaster.current.shipManager.GetTotalAvailableEnergyCredit();
00034: 		e.AddChoice(DirectoryMaster.Item("ok"), "(400 Energy Credit) Pay the bandit", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 400, null, delegate
00035: 		{
00036: 			GameMaster.current.shipManager.RemoveEnergyCredit(400);
00037: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The bandit is satisfied and informs you that you may leave peacefully.", RenderHandler.ColorPalette.LightGreen));
00038: 			e.ForceEnd();
00039: 		});
00040: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Open fire on the bandit ship.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00041: 		{
00042: 		});
00043: 		e.AddChoice(DirectoryMaster.Item("ok"), "[Military Ship] Identify yourself as warship and demand that the bandit transfer you 400 Credit instead or otherwise you might not garantee their safety.", "", () => GameMaster.current.shipManager.IsShipArmed(), null, delegate
00044: 		{
00045: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("After realizing that they were trying to rob a warship, the bandit leader quickly transfer the credit before their ship jumps away.", RenderHandler.ColorPalette.LightGreen));
00046: 			GameItem gameItem = DirectoryMaster.Item("backpack_medium");
00047: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00048: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00049: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00050: 			GraphUtils.TryAcceptAll(gameItem, DirectoryMaster.Item("energy_credit"));
00051: 			OverlayHandler.current.ShowInventory(gameItem);
00052: 			e.ForceEnd();
00053: 		});
00054: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Refuse to pay and attempt to escape.", "", null, null, delegate
00055: 		{
00056: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away with your ship and the pirate has open fire on your ship!", RenderHandler.ColorPalette.LightRed));
00057: 			SpaceBattleHelper.TakingHit(10);
00058: 			e.ForceEnd();
00059: 		});
00060: 		return e;
00061: 	}
00062: }
~~~

## RaidManager.cs

~~~csharp
00289: 				else
00290: 				{
00291: 					CheckValidParty();
00292: 				}
00293: 			}
00294: 		}));
00295: 		SyncCombatSelectCharacter();
00296: 	}
00297: 
00298: 	public void CheckValidParty()
00299: 	{
00300: 		if (playerParty.childItems.Count() > 0)
00301: 		{
00302: 			return;
00303: 		}
00304: 		List<GameCharacterItem> source = GetAllPlayerCharacters().FindAll((GameCharacterItem gci) => !gci.isDead);
00305: 		if (source.Count() > 0)
00306: 		{
00307: 			Room room = FindItemRoom(source.First());
00308: 			if (room != null)
00309: 			{
00310: 				EnterRoom(room);
00311: 			}
00312: 			return;
00313: 		}
00314: 		OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder(50).AddLine("Everyone in your raid has perished.", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true).AddLine().AddLine("The end.")
00315: 			.Build(), null, delegate
00316: 		{
00317: 			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
00318: 			{
00319: 				GameMaster.current.QuitToMenu();
00320: 			});
00321: 		}, canYes: true, showYes: false, "Confirm", "Recall");
00322: 	}
00323: 
00324: 	public void SyncCombatSelectCharacter()
00325: 	{
00326: 		if (combatMaster != null)
00327: 		{
00328: 			raidScene.RestoreCharacterInventory();
00329: 			GameCharacterItem gameCharacterItem = playerParty.Find(combatMaster.WhichCurrentPlayer());
00330: 			if (gameCharacterItem != null)
00331: 			{
00332: 				raidScene.StealCharacterInventory(gameCharacterItem);
00333: 			}
00334: 		}
00335: 	}
00336: 
00337: 	public void UseActionPoints(int actionPoints)
00338: 	{
00339: 		if (actionPoints > 0)
00340: 		{
00341: 			while (actionPoints >= actionPointsRemaining)
00342: 			{
00343: 				actionPoints -= actionPointsRemaining;
00344: 				GameMaster.current.turnManager.NextTurn();
00345: 			}
00346: 			actionPointsRemaining -= actionPoints;
00347: 		}
00348: 	}
~~~

## CraftingRecipe.cs

~~~csharp
00001: using System;
00002: using System.Collections.Generic;
00003: using System.Linq;
00004: 
00005: public class CraftingRecipe
00006: {
00007: 	public List<CraftingFilter> requiredTools;
00008: 
00009: 	public List<CraftingFilter> requiredMaterials;
00010: 
00011: 	public Func<List<GameItem>, List<GameItem>, List<GameItem>> craftFunc;
00012: 
00013: 	public RichTextBuilder description;
00014: 
00015: 	public string customName { get; set; }
00016: 
00017: 	public int remainingCraftAmount { get; set; }
00018: 
00019: 	public CraftingRecipe(string name = null)
00020: 	{
00021: 		customName = name;
00022: 		craftFunc = (List<GameItem> _, List<GameItem> _) => new List<GameItem>();
00023: 		requiredTools = new List<CraftingFilter>();
00024: 		requiredMaterials = new List<CraftingFilter>();
00025: 		description = new RichTextBuilder();
00026: 		remainingCraftAmount = -1;
00027: 	}
00028: 
00029: 	public CraftingRecipe SetRemainingCraftAmount(int amount)
00030: 	{
00031: 		remainingCraftAmount = amount;
00032: 		return this;
00033: 	}
00034: 
00035: 	public CraftingRecipe AddToolIdentifier(string identifier, int amount = -1)
00036: 	{
00037: 		return AddToolIdentifier(DirectoryMaster.Item(identifier)?.name ?? "Unknown", amount, identifier);
00038: 	}
00039: 
00040: 	public CraftingRecipe AddMaterialIdentifier(string identifier, int amount = -1)
00041: 	{
00042: 		return AddMaterialIdentifier(DirectoryMaster.Item(identifier)?.name ?? "Unknown", amount, identifier);
00043: 	}
00044: 
00045: 	public CraftingRecipe AddToolIdentifier(string displayName, string identifier, params string[] additionalIdentifiers)
00046: 	{
00047: 		return AddTool(new CraftingIdentifierFilter(displayName, identifier, additionalIdentifiers));
00048: 	}
00049: 
00050: 	public CraftingRecipe AddMaterialIdentifier(string displayName, string identifier, params string[] additionalIdentifiers)
00051: 	{
00052: 		return AddMaterial(new CraftingIdentifierFilter(displayName, identifier, additionalIdentifiers));
00053: 	}
00054: 
00055: 	public CraftingRecipe AddToolIdentifier(string displayName, int powerAmount, string identifier, params string[] additionalIdentifiers)
00056: 	{
00057: 		return AddTool(new CraftingIdentifierFilter(displayName, powerAmount, identifier, additionalIdentifiers));
00058: 	}
00059: 
~~~

~~~csharp
00094: 
00095: 	public CraftingRecipe AddTool(string displayName, int powerAmount, Func<GameItem, bool> func)
00096: 	{
00097: 		return AddTool(new CraftingCustomFilter(displayName, powerAmount, func));
00098: 	}
00099: 
00100: 	public CraftingRecipe AddMaterial(string displayName, int amount, Func<GameItem, bool> func)
00101: 	{
00102: 		return AddMaterial(new CraftingCustomFilter(displayName, amount, func));
00103: 	}
00104: 
00105: 	public CraftingRecipe AddTool(CraftingFilter filter)
00106: 	{
00107: 		requiredTools.Add(filter);
00108: 		return this;
00109: 	}
00110: 
00111: 	public CraftingRecipe AddMaterial(CraftingFilter filter)
00112: 	{
00113: 		requiredMaterials.Add(filter);
00114: 		return this;
00115: 	}
00116: 
00117: 	public CraftingRecipe SetDescription(string description)
00118: 	{
00119: 		this.description = new RichTextBuilder();
00120: 		this.description.Add(description);
00121: 		return this;
00122: 	}
00123: 
00124: 	public CraftingRecipe SetDescription(RichTextBuilder description)
00125: 	{
00126: 		this.description = description;
00127: 		return this;
00128: 	}
00129: 
00130: 	public CraftingRecipe SetCraftingFunc(Func<List<GameItem>, List<GameItem>, List<GameItem>> func)
00131: 	{
00132: 		craftFunc = func;
00133: 		return this;
00134: 	}
00135: 
00136: 	public CraftingRecipe SetCraftingConstantFunc(string itemIdentifier, int amount = 1)
00137: 	{
00138: 		if (!DirectoryMaster.Has<GameItem>(itemIdentifier))
00139: 		{
00140: 			Debug.LogWarning("Crafting output identifier '" + itemIdentifier + "' is not in item directory.");
00141: 		}
00142: 		craftFunc = (List<GameItem> _, List<GameItem> _) => new List<GameItem> { DirectoryMaster.Item(itemIdentifier).SetAmount(amount) };
00143: 		return this;
00144: 	}
00145: 
00146: 	private List<(CraftingFilter, List<GameItem>)> SortFilters(List<GameItem> allItems, List<CraftingFilter> requiredFilter, bool isAmount)
00147: 	{
00148: 		List<(CraftingFilter, List<GameItem>)> list = new List<(CraftingFilter, List<GameItem>)>();
00149: 		foreach (CraftingFilter item in requiredFilter)
00150: 		{
00151: 			List<GameItem> list2 = new List<GameItem>();
00152: 			int num = 0;
00153: 			foreach (GameItem allItem in allItems)
~~~

~~~csharp
00313: 	}
00314: 
00315: 	public bool CanCraft(GraphNodeStorage input)
00316: 	{
00317: 		return CanCraft(GraphUtils.FindAllChildrenType<GameItem>(input));
00318: 	}
00319: 
00320: 	public List<GameItem> TryCraftInside(GraphNodeStorage input, GraphNodeStorage output)
00321: 	{
00322: 		List<GameItem> allItems = GraphUtils.FindAllChildrenType<GameItem>(input);
00323: 		List<GameItem> list = new List<GameItem>();
00324: 		foreach (GameItem item in Craft(allItems))
00325: 		{
00326: 			if (GraphUtils.TryAcceptAll(output, item) > 0)
00327: 			{
00328: 				list.Add(item);
00329: 			}
00330: 		}
00331: 		return list;
00332: 	}
00333: 
00334: 	public RichTextBuilder GetCraftingDetails(List<GameItem> allItems)
00335: 	{
00336: 		List<(CraftingFilter, List<GameItem>)> list = FilterTools(allItems);
00337: 		List<(CraftingFilter, List<GameItem>)> list2 = FilterMaterials(allItems);
00338: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00339: 		richTextBuilder.Add(description);
00340: 		richTextBuilder.AddLine(" ");
00341: 		richTextBuilder.AddLine(" ");
00342: 		if (remainingCraftAmount >= 0)
00343: 		{
00344: 			richTextBuilder.AddLine("Remaining Attempts:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00345: 			richTextBuilder.AddLine(remainingCraftAmount.ToString() ?? "", ignoreEmpty: true, (remainingCraftAmount > 0) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
00346: 			richTextBuilder.AddLine(" ");
00347: 		}
00348: 		if (list.Any())
00349: 		{
00350: 			richTextBuilder.AddLine("Required Tools:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00351: 		}
00352: 		foreach (var (craftingFilter, list3) in list)
00353: 		{
00354: 			if (craftingFilter.amount < 0)
00355: 			{
00356: 				if (list3.Any())
00357: 				{
00358: 					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightGreen).Add(craftingFilter.displayName);
00359: 					richTextBuilder.AddLine(" (Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00360: 				}
00361: 				else
00362: 				{
00363: 					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightRed).Add(craftingFilter.displayName);
00364: 					richTextBuilder.AddLine(" (Not Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
00365: 				}
00366: 			}
00367: 			else if (list3.Any())
00368: 			{
00369: 				GameItem gameItem = null;
00370: 				int num = -1;
00371: 				foreach (GameItem item3 in list3)
00372: 				{
~~~

~~~csharp
00412: 		{
00413: 			richTextBuilder.AddLine(" ");
00414: 		}
00415: 		richTextBuilder.AddLine("Required Materials:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00416: 		foreach (var item4 in list2)
00417: 		{
00418: 			CraftingFilter item = item4.Item1;
00419: 			List<GameItem> item2 = item4.Item2;
00420: 			int num2 = ((item.amount < 0) ? 1 : item.amount);
00421: 			int num3 = item2.Select((GameItem gi) => gi.unitCount).Sum();
00422: 			if (num3 > num2)
00423: 			{
00424: 				num3 = num2;
00425: 			}
00426: 			RenderHandler.ColorPalette color2 = ((num3 < num2) ? RenderHandler.ColorPalette.LightRed : RenderHandler.ColorPalette.LightGreen);
00427: 			richTextBuilder.Add("·", color2).Add(item.displayName);
00428: 			richTextBuilder.AddLine(" (" + num3 + "/" + num2 + ")", ignoreEmpty: true, color2);
00429: 		}
00430: 		return richTextBuilder;
00431: 	}
00432: 
00433: 	public RichTextBuilder GetVendingDetails(List<GameItem> allItems)
00434: 	{
00435: 		List<(CraftingFilter, List<GameItem>)> list = FilterTools(allItems);
00436: 		FilterMaterials(allItems);
00437: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00438: 		richTextBuilder.Add(description);
00439: 		richTextBuilder.AddLine(" ");
00440: 		richTextBuilder.AddLine(" ");
00441: 		if (remainingCraftAmount >= 0)
00442: 		{
00443: 			richTextBuilder.AddLine("Remaining Stock:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00444: 			richTextBuilder.AddLine(remainingCraftAmount.ToString() ?? "", ignoreEmpty: true, (remainingCraftAmount > 0) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
00445: 			richTextBuilder.AddLine(" ");
00446: 		}
00447: 		richTextBuilder.AddLine("Required Power:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00448: 		foreach (var (craftingFilter, list2) in list)
00449: 		{
00450: 			if (craftingFilter.amount < 0)
00451: 			{
00452: 				if (list2.Any())
00453: 				{
00454: 					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightGreen).Add(craftingFilter.displayName);
00455: 					richTextBuilder.AddLine(" (Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00456: 				}
00457: 				else
00458: 				{
00459: 					richTextBuilder.Add("·", RenderHandler.ColorPalette.LightRed).Add(craftingFilter.displayName);
00460: 					richTextBuilder.AddLine(" (Not Found)", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
00461: 				}
00462: 			}
00463: 			else if (list2.Any())
00464: 			{
00465: 				GameItem gameItem = null;
00466: 				int num = -1;
00467: 				foreach (GameItem item in list2)
00468: 				{
00469: 					if (PowerHelper.IsAvailableEnergyFromItem(item))
00470: 					{
00471: 						int availableEnergyFromItem = PowerHelper.GetAvailableEnergyFromItem(item, useBandwidth: false);
~~~

## SpaceDistressSignalTrigger.cs

~~~csharp
00018: 		e.SetImage("event_distress");
00019: 		e.text.AddLine("\n        The ship captain has contacted you and ask for your help, they are stranded without fuel and energy. They are offering the cargo they are transporting at a generous discount in exchange for fuel cell and energy credit.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00020: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00021: 		trader.name = "Distressed Cargo (Credit & Fuel Only)";
00022: 		trader.BarterEnable(TradeSheet.GetDistressSignalCargoShop());
00023: 		List<string> tagList = new List<string> { "FOOD", "ITEM_MEDICAL_TAG", "backpack" };
00024: 		List<int> discountList = new List<int> { -50, -50, -50 };
00025: 		List<string> itemTags = new List<string> { "power_source_item", "EXOTIC_FUEL_TAG" };
00026: 		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
00027: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
00028: 		e.startAction = delegate
00029: 		{
00030: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_DISTRESS_SIGNAL_EVENT_STARTED");
00031: 			tag.SetInt(tag.GetInt() + 1);
00032: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00033: 			{
00034: 				overworldSpace.isEventTriggered = true;
00035: 			}
00036: 		};
00037: 		e.AddChoice(DirectoryMaster.Item("coin"), "(Trade) See what they have to offer.", "", null, null, delegate
00038: 		{
00039: 			trader.BarterShow();
00040: 		});
00041: 		e.AddChoice(DirectoryMaster.Item("coin"), "(Donate) Give 100 Energy Credit and 20 Exotic Fuel Cell.", "", () => GameMaster.current.shipManager.GetTotalAvailableEnergyCredit() >= 100 && GameMaster.current.shipManager.GetTotalAvailableFuel() >= 20, null, delegate
00042: 		{
00043: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("The Crew is grateful that you gave them without asking for reward. The captain decided to offer you the nearby location where they dumped some cargo to reduce their fuel consumption.", RenderHandler.ColorPalette.LightGreen));
00044: 			GameMaster.current.worldTags.EnableTag("SPAWN_CARGO_POD_WORLD_TAG");
00045: 			GameMaster.current.shipManager.RemoveEnergyCredit(100);
00046: 			GameMaster.current.shipManager.RemoveExoticFuel(20);
00047: 			OverlayHandler.current.CreateNav(NavigationHelper.ScanForEncounters());
00048: 		});
00049: 		e.AddChoice(DirectoryMaster.Item("coin"), "(Hostile) Threaten them to handover all of their cargo or you will open fire", "", null, null, delegate
00050: 		{
00051: 			GameMaster.current.shipManager.ship?.TryAcceptAll(DirectoryMaster.Item("coin").SetAmount(5));
00052: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew has mine the asteroid", RenderHandler.ColorPalette.LightGreen));
00053: 			GameItem gameItem = DirectoryMaster.Item("backpack_small");
00054: 			GameItem item = DirectoryMaster.Item("red_injector");
00055: 			GraphUtils.TryAcceptAll(gameItem, item);
00056: 			OverlayHandler.current.ShowInventory(gameItem);
00057: 			e.ForceEnd();
00058: 		});
00059: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now, It's not like they are going anywhere.", "", null, null, delegate
00060: 		{
00061: 		});
00062: 		return e;
00063: 	}
00064: }
~~~

## ModInternal.cs

~~~csharp
00001: public class ModInternal
00002: {
00003: 	public static void CreateTooltipEarly(RichTextBuilder builder, GameItem item)
00004: 	{
00005: 		ModHook.FireOnCreateTooltipEarly(builder, item);
00006: 	}
00007: 
00008: 	public static void CreateDevTooltipEarly(RichTextBuilder builder, GameItem item)
00009: 	{
00010: 		ModHook.FireOnCreateDevTooltipEarly(builder, item);
00011: 	}
00012: 
00013: 	public static void CreateTooltipLate(RichTextBuilder builder, GameItem item)
00014: 	{
00015: 		ModHook.FireOnCreateTooltipLate(builder, item);
00016: 	}
00017: 
00018: 	public static void CreateDevTooltipLate(RichTextBuilder builder, GameItem item)
00019: 	{
00020: 		ModHook.FireOnCreateDevTooltipLate(builder, item);
00021: 	}
00022: }
~~~

## SpaceExampleTrigger.cs

~~~csharp
00003: 	public override bool? Trigger()
00004: 	{
00005: 		GameMaster.current.worldTags.GetTag("SPACE_EXAMPLE_EVENT_STARTED");
00006: 		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("exemple") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
00007: 		{
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData eventData = new EventData().SetText("Ship Distress Signal", "");
00016: 		eventData.text.AddLine("\r\na", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00017: 		eventData.startAction = delegate
00018: 		{
00019: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_EXAMPLE_EVENT_STARTED");
00020: 			tag.SetInt(tag.GetInt() + 1);
00021: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00022: 			{
00023: 				overworldSpace.isEventTriggered = true;
00024: 			}
00025: 		};
00026: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "-", "", null, null, delegate
00027: 		{
00028: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("", RenderHandler.ColorPalette.LightGreen));
00029: 		});
00030: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00031: 		{
00032: 		});
00033: 		return eventData;
00034: 	}
00035: }
~~~

## CombatLog.cs

~~~csharp
00001: public class CombatLog
00002: {
00003: 	public static bool IsVerbose;
00004: 
00005: 	public static void log(string message, RenderHandler.ColorPalette color = RenderHandler.ColorPalette.VeryLightGray)
00006: 	{
00007: 		OverlayHandler.current.AppendLog(new RichTextBuilder().Add(message, color));
00008: 	}
00009: 
00010: 	public static void slog(string message)
00011: 	{
00012: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00013: 		OverlayHandler.current.AppendLog(richTextBuilder.Add(message, RenderHandler.ColorPalette.Red, " "));
00014: 	}
00015: 
00016: 	public static void vlog(string message)
00017: 	{
00018: 		if (IsVerbose)
00019: 		{
00020: 			log(message);
00021: 		}
00022: 	}
00023: }
~~~

## SpaceNutrifruitShopTrigger.cs

~~~csharp
00018: 		eventData.startAction = delegate
00019: 		{
00020: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_NUTRIFRUIT_EVENT_STARTED");
00021: 			tag.SetInt(tag.GetInt() + 1);
00022: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00023: 			{
00024: 				overworldSpace.isEventTriggered = true;
00025: 			}
00026: 		};
00027: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00028: 		trader.name = "pastel Drone Shop (Credit Only)";
00029: 		trader.BarterEnable(TradeSheet.GetGenericPastelShop());
00030: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(new List<string> { "power_source_item" }, trader);
00031: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "Browse wares", "", null, null, delegate
00032: 		{
00033: 			trader.BarterShow();
00034: 		});
00035: 		eventData.AddChoice(DirectoryMaster.Item("open_fire"), "Open fire on the drone ship.", "", null, null, delegate
00036: 		{
00037: 		});
00038: 		GameItem container = DirectoryMaster.Item("lootbox_medium");
00039: 		bool cardUsed = false;
00040: 		eventData.AddChoice(DirectoryMaster.Item("keycard"), "[pastel Keycard] Use Keycard.", "", () => GameMaster.current.shipManager.IsShipHaveItem("pastel_keycard"), null, delegate
00041: 		{
00042: 			GameMaster.current.shipManager.RemoveItemFromShip("pastel_keycard");
00043: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\n'pastel Droneship: Level X access detected, unlocking access...", RenderHandler.ColorPalette.LightGray));
00044: 			foreach (GameItem childItem in trader.barterInventory.childItems)
00045: 			{
00046: 				childItem.DisableTag(GameCharacterItem.TAG_NOT_PURCHASED);
00047: 				GraphUtils.TryAcceptAll(container, childItem);
00048: 			}
00049: 			OverlayHandler.current.ShowInventory(container);
00050: 			cardUsed = true;
00051: 		});
00052: 		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Loot.", "", null, () => cardUsed, delegate
00053: 		{
00054: 			OverlayHandler.current.ShowInventory(container);
00055: 		});
00056: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00057: 		{
00058: 		});
00059: 		return eventData;
00060: 	}
00061: }
~~~

## SpaceSalvageShipTrigger.cs

~~~csharp
00018: 		eventData.SetImage("event_scav");
00019: 		eventData.text.AddLine("\r\nThe ship captain contacts you and ask if you are here to take Offload.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
00020: 		eventData.startAction = delegate
00021: 		{
00022: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_SALVAGE_SHIP_EVENT_STARTED");
00023: 			tag.SetInt(tag.GetInt() + 1);
00024: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00025: 			{
00026: 				overworldSpace.isEventTriggered = true;
00027: 			}
00028: 		};
00029: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00030: 		trader.name = "Scav's Offload (Credit Only)";
00031: 		trader.BarterEnable(TradeSheet.GetSalvageOffload());
00032: 		List<string> tagList = new List<string> { "SCRAP_METAL", "MATERIAL_TAG", "CONSTRUCTION_MATERIAL_TAG" };
00033: 		List<int> discountList = new List<int> { -50, -50, -50 };
00034: 		List<string> itemTags = new List<string> { "power_source_item", "NAV_DISK_SALVAGE_TAG" };
00035: 		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
00036: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
00037: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "Yes", "", null, null, delegate
00038: 		{
00039: 			trader.BarterShow();
00040: 		});
00041: 		eventData.AddChoice(DirectoryMaster.Item("question"), "What is Offloading?", "", null, null, delegate
00042: 		{
00043: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain explain to you that Offloading is when a scavenger sell their finding at a discount to a cargo ship, so they can return to salvaging and the cargo ship will take the finding and sell it at a trade station for a profit.\r\nIt is more profitable for them to Offload and keep salvaging than transporting, finding a trade station and sell the goods themself.", RenderHandler.ColorPalette.LightGreen));
00044: 		});
00045: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "Is there anything we can sell you?", "", null, null, delegate
00046: 		{
00047: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain tell you that if you have visited a fresh shipwreck recently, they are interested to buy the nav disk to that location.", RenderHandler.ColorPalette.LightGreen));
00048: 		});
00049: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00050: 		{
00051: 		});
00052: 		return eventData;
00053: 	}
00054: }
~~~

## ItemDefaultActivateHandler.cs

~~~csharp
00001: using System.Collections.Generic;
00002: using System.Linq;
00003: using UnityEngine;
00004: 
00005: public class ItemDefaultActivateHandler : EmptyActionHandler
00006: {
00007: 	public static ItemDefaultActivateHandler current;
00008: 
00009: 	[SerializeField]
00010: 	public KeyCode key;
00011: 
00012: 	[SerializeField]
00013: 	public Vector2 tooltipTranslation = Vector2.op_Implicit(new Vector3(32f, 0f));
00014: 
00015: 	private GameItem currentItem;
00016: 
00017: 	private HighlightManager highlightManager = new HighlightManager();
00018: 
00019: 	private RichTextBuilder builder = new RichTextBuilder(60);
00020: 
00021: 	public override List<KeyCode> keyListeners => ((IEnumerable<KeyCode>)(object)new KeyCode[1] { (KeyCode)(int)key }).ToList();
00022: 
00023: 	[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
00024: 	private void Awake()
00025: 	{
00026: 		current = this;
00027: 	}
00028: 
00029: 	protected override void OnEventPress(ISet<KeyCode> keysPressed)
00030: 	{
00031: 		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
00032: 		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
00033: 		if (ItemPinHelper.BlocksClicks)
00034: 		{
00035: 			return;
00036: 		}
00037: 		_ = currentItem;
00038: 		GameItem gameItem = RenderHandler.RaycastElement<GameItem>(Vector2.op_Implicit(Input.mousePosition));
00039: 		GameItem gameItem2 = gameItem;
00040: 		if (gameItem != null && currentItem == null && gameItem2.MayActivateSlotItem() && gameItem.activateDefault)
00041: 		{
00042: 			InputActionManager.current.ResetAfter(this);
00043: 			if (EmporiumOption.SingleClickChoice && gameItem2.CanActivateSlotItem())
00044: 			{
00045: 				gameItem2.ActivateSlotItem();
00046: 				OnEventReset();
00047: 			}
00048: 			else
00049: 			{
00050: 				SlotMarker incompleteSlotInInventory = gameItem2.GetIncompleteSlotInInventory();
00051: 				highlightManager.Try(incompleteSlotInInventory, gameItem2.CanActivateSlotItem() ? RenderHandler.BackgroundColor.Green : RenderHandler.BackgroundColor.Red);
00052: 				currentItem = gameItem;
00053: 			}
~~~

~~~csharp
00078: 				OnEventReset();
00079: 			}
00080: 		}
00081: 	}
00082: 
00083: 	protected override bool OnEventUpdate()
00084: 	{
00085: 		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
00086: 		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
00087: 		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
00088: 		if (currentItem != null)
00089: 		{
00090: 			GameItem gameItem = currentItem;
00091: 			builder.Clear();
00092: 			if (gameItem.CanActivateSlotItem())
00093: 			{
00094: 				builder.Add(LocHelper.GetLocalizedMechanic("handler_activate_click_again")).Add(LocHelper.GetLocalizedMechanic("handler_activate_confirm"), RenderHandler.ColorPalette.Green, "", bold: true).Add(LocHelper.GetLocalizedMechanic("handler_activate_activation_of"))
00095: 					.Add(gameItem.GetDisplayName(preferCustomName: true), RenderHandler.ColorPalette.None, "", bold: true, italic: false, underline: true);
00096: 			}
00097: 			else
00098: 			{
00099: 				builder.Add(LocHelper.GetLocalizedMechanic("handler_activate_cannot", gameItem.GetDisplayName(preferCustomName: true)), RenderHandler.ColorPalette.Red);
00100: 			}
00101: 			if (!builder.IsEmpty())
00102: 			{
00103: 				OverlayHandler.current.ShowTooltip(Vector2.op_Implicit(Input.mousePosition), tooltipTranslation, builder.Build());
00104: 			}
00105: 			else
00106: 			{
00107: 				OverlayHandler.current.HideTooltip();
00108: 			}
00109: 		}
00110: 		return currentItem != null;
00111: 	}
00112: 
00113: 	protected override void OnEventReset()
00114: 	{
00115: 		highlightManager.Clear();
00116: 		if (currentItem != null)
00117: 		{
00118: 			OverlayHandler.current.HideTooltip();
00119: 		}
00120: 		currentItem = null;
00121: 	}
00122: }
~~~

## RichTextGenerator.cs

~~~csharp
00001: public class RichTextGenerator
00002: {
00003: 	private static RichTextBuilder _builder;
00004: 
00005: 	private static RichTextBuilder builder
00006: 	{
00007: 		get
00008: 		{
00009: 			if (_builder == null)
00010: 			{
00011: 				_builder = new RichTextBuilder(60);
00012: 			}
00013: 			return _builder;
00014: 		}
00015: 	}
00016: 
00017: 	private static string Underline(string text)
00018: 	{
00019: 		return "<u>" + text + "</u>";
00020: 	}
00021: 
00022: 	public static RichText InsertItemInOtherItem(GameItem sourceItem, GameItem targetItem, bool canInsert, RichTextBuilder customBuilder = null)
00023: 	{
00024: 		if (customBuilder == null)
00025: 		{
00026: 			customBuilder = builder;
00027: 		}
00028: 		customBuilder.Clear();
00029: 		string displayName = sourceItem.GetDisplayName(preferCustomName: true);
00030: 		string displayName2 = targetItem.GetDisplayName(preferCustomName: true);
00031: 		if (canInsert)
00032: 		{
00033: 			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_insert_into", displayName, Underline(displayName2)), RenderHandler.ColorPalette.LightGreen);
00034: 		}
00035: 		else
00036: 		{
00037: 			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_insert", displayName, displayName2), RenderHandler.ColorPalette.Red);
00038: 		}
00039: 		return customBuilder.Build();
00040: 	}
00041: 
00042: 	public static RichText UseItemOnOtherItem(GameItem sourceItem, GameItem targetItem, SlotMarker targetSlot, bool canInsert, RichTextBuilder customBuilder = null)
00043: 	{
00044: 		if (customBuilder == null)
00045: 		{
~~~

~~~csharp
00014: 		}
00015: 	}
00016: 
00017: 	private static string Underline(string text)
00018: 	{
00019: 		return "<u>" + text + "</u>";
00020: 	}
00021: 
00022: 	public static RichText InsertItemInOtherItem(GameItem sourceItem, GameItem targetItem, bool canInsert, RichTextBuilder customBuilder = null)
00023: 	{
00024: 		if (customBuilder == null)
00025: 		{
00026: 			customBuilder = builder;
00027: 		}
00028: 		customBuilder.Clear();
00029: 		string displayName = sourceItem.GetDisplayName(preferCustomName: true);
00030: 		string displayName2 = targetItem.GetDisplayName(preferCustomName: true);
00031: 		if (canInsert)
00032: 		{
00033: 			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_insert_into", displayName, Underline(displayName2)), RenderHandler.ColorPalette.LightGreen);
00034: 		}
00035: 		else
00036: 		{
00037: 			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_insert", displayName, displayName2), RenderHandler.ColorPalette.Red);
00038: 		}
00039: 		return customBuilder.Build();
00040: 	}
00041: 
00042: 	public static RichText UseItemOnOtherItem(GameItem sourceItem, GameItem targetItem, SlotMarker targetSlot, bool canInsert, RichTextBuilder customBuilder = null)
00043: 	{
00044: 		if (customBuilder == null)
00045: 		{
00046: 			customBuilder = builder;
00047: 		}
00048: 		customBuilder.Clear();
00049: 		string displayName = sourceItem.GetDisplayName(preferCustomName: true);
00050: 		string displayName2 = targetItem.GetDisplayName(preferCustomName: true);
00051: 		if (canInsert)
00052: 		{
00053: 			if (TooltipUseHelper.HasUseTooltip(sourceItem, targetItem))
00054: 			{
00055: 				TooltipUseHelper.HandleUseTooltip(sourceItem, targetItem, customBuilder);
00056: 			}
00057: 			else
00058: 			{
00059: 				customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_use_on", displayName, Underline(displayName2)), RenderHandler.ColorPalette.LightGreen);
00060: 			}
00061: 		}
00062: 		else
00063: 		{
00064: 			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_use_on", displayName, displayName2), RenderHandler.ColorPalette.Red);
00065: 		}
00066: 		return customBuilder.Build();
00067: 	}
00068: }
~~~

~~~csharp
00041: 
00042: 	public static RichText UseItemOnOtherItem(GameItem sourceItem, GameItem targetItem, SlotMarker targetSlot, bool canInsert, RichTextBuilder customBuilder = null)
00043: 	{
00044: 		if (customBuilder == null)
00045: 		{
00046: 			customBuilder = builder;
00047: 		}
00048: 		customBuilder.Clear();
00049: 		string displayName = sourceItem.GetDisplayName(preferCustomName: true);
00050: 		string displayName2 = targetItem.GetDisplayName(preferCustomName: true);
00051: 		if (canInsert)
00052: 		{
00053: 			if (TooltipUseHelper.HasUseTooltip(sourceItem, targetItem))
00054: 			{
00055: 				TooltipUseHelper.HandleUseTooltip(sourceItem, targetItem, customBuilder);
00056: 			}
00057: 			else
00058: 			{
00059: 				customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_use_on", displayName, Underline(displayName2)), RenderHandler.ColorPalette.LightGreen);
00060: 			}
00061: 		}
00062: 		else
00063: 		{
00064: 			customBuilder.Add(LocHelper.GetLocalizedMechanic("mech_drag_cant_use_on", displayName, displayName2), RenderHandler.ColorPalette.Red);
00065: 		}
00066: 		return customBuilder.Build();
00067: 	}
00068: }
~~~

## SpaceMiningShipTrigger.cs

~~~csharp
00018: 		eventData.SetImage("event_miner");
00019: 		eventData.text.AddLine("\r\nThe ship captain contacts you and ask if you are here to take Offload.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGray);
00020: 		eventData.startAction = delegate
00021: 		{
00022: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_MINING_SHIP_EVENT_STARTED");
00023: 			tag.SetInt(tag.GetInt() + 1);
00024: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00025: 			{
00026: 				overworldSpace.isEventTriggered = true;
00027: 			}
00028: 		};
00029: 		GameCharacterItem trader = DirectoryMaster.CharacterItem("merchant");
00030: 		trader.name = "Miner's Offload (Credit Only)";
00031: 		trader.BarterEnable(TradeSheet.GetMinerOffload());
00032: 		List<string> tagList = new List<string> { "ORE_TAG" };
00033: 		List<int> discountList = new List<int> { -50 };
00034: 		List<string> itemTags = new List<string> { "power_source_item", "NAV_DISK_MINING_TAG" };
00035: 		BarterHelper.SetVendorDiscount(trader, tagList, discountList);
00036: 		BarterHelper.SetOnlyAcceptTaggedItemAsPayment(itemTags, trader);
00037: 		eventData.AddChoice(DirectoryMaster.Item("trade"), "Yes", "", null, null, delegate
00038: 		{
00039: 			trader.BarterShow();
00040: 		});
00041: 		eventData.AddChoice(DirectoryMaster.Item("question"), "What is Offloading?", "", null, null, delegate
00042: 		{
00043: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain explain to you that Offloading is when a miner sell their ore at a discount to a cargo ship, so they can return to mining and the cargo ship will take the ore and sell it at a refinery station for a profit.\r\nIt is more profitable for them to Offload and keep mining than transporting and finding a trade station with a refinery themself.", RenderHandler.ColorPalette.LightGray));
00044: 		});
00045: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "Is there anything we can sell you?", "", null, null, delegate
00046: 		{
00047: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("\r\nThe ship captain tell you that if you have visited a asteroid suitable for mining recently, they will gladly pay for the nav disk.", RenderHandler.ColorPalette.LightGreen));
00048: 		});
00049: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00050: 		{
00051: 		});
00052: 		return eventData;
00053: 	}
00054: }
~~~

## AmenitiesManager.cs

~~~csharp
00244: 		if (tag.Equals("amenities_substance"))
00245: 		{
00246: 			return "Substance Amenities";
00247: 		}
00248: 		if (tag.Equals("amenities_nutrition"))
00249: 		{
00250: 			return "Nutrition Amenities";
00251: 		}
00252: 		if (tag.Equals("amenities_medical"))
00253: 		{
00254: 			return "Medical Amenities";
00255: 		}
00256: 		if (tag.Equals("amenities_entertrainment"))
00257: 		{
00258: 			return "Entertrainment Amenities";
00259: 		}
00260: 		if (tag.Equals("amenities_wellbeing"))
00261: 		{
00262: 			return "Well Being Amenities";
00263: 		}
00264: 		return "Error";
00265: 	}
00266: 
00267: 	public RichTextBuilder GetAmenitiesRichText()
00268: 	{
00269: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00270: 		richTextBuilder.AddLine("Expected Amenities:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00271: 		for (int i = 0; i < instance.amenitiesType.Length; i++)
00272: 		{
00273: 			string key = amenitiesType[i];
00274: 			richTextBuilder.AddLine(amenitiesTypeDisplay[i] + amenitiesDisplaySpacer[i] + " : " + amenities_values_display[key]);
00275: 		}
00276: 		richTextBuilder.AddLine("Total Amenities : " + totalAmenitiesValueDisplay, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
00277: 		richTextBuilder.AddLine("Predicted Score : " + GetScore(totalAmenitiesValue));
00278: 		richTextBuilder.AddLine(" ");
00279: 		richTextBuilder.AddLine("Last Turn Amenities:", ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true, italic: false, underline: true);
00280: 		for (int j = 0; j < instance.amenitiesType.Length; j++)
00281: 		{
00282: 			string key2 = amenitiesType[j];
00283: 			richTextBuilder.AddLine(amenitiesTypeDisplay[j] + amenitiesDisplaySpacer[j] + " : " + past_amenities_values_display[key2]);
00284: 		}
00285: 		richTextBuilder.AddLine("Total Amenities : " + pastTotalAmenitiesValueDisplay, ignoreEmpty: true, RenderHandler.ColorPalette.None, bold: true);
00286: 		richTextBuilder.AddLine("Score : " + GetScore(pastTotalAmenitiesValue));
00287: 		return richTextBuilder;
00288: 	}
00289: 
00290: 	public static int GetAmenitiesItemValue(GameItem item)
00291: 	{
00292: 		if (item.IsTag("amenities"))
00293: 		{
00294: 			return item.GetTagReadonly("amenities").valueInt * item.GetTagReadonly("amenities_use").valueInt / 2;
00295: 		}
00296: 		return 0;
00297: 	}
00298: }
~~~

## TraderStationBlockadeTrigger.cs

~~~csharp
00004: 	{
00005: 		GameMaster.current.worldTags.GetTag("TRADER_STATION_BLOCKADE_EVENT_STARTED");
00006: 		if (GameMaster.current.shipManager.nextPOI is OverworldStation { isHostile: false } overworldStation && !GameMaster.current.shipManager.isTransit && !overworldStation.generatedStation.GetArrivalEvent().Equals("BLOCKADE"))
00007: 		{
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData e = new EventData().SetText("Blockaded Trade Station", " You have arrived at a space station.");
00016: 		e.text.AddLine("\r\nThis station is blockaded by a fleet of ship. Whom ever is siegeing the station is trying to starve them out!", ignoreEmpty: true, RenderHandler.ColorPalette.LightRed);
00017: 		e.startAction = delegate(EventData eventData)
00018: 		{
00019: 			eventData.BlockStation();
00020: 			TagState tag = GameMaster.current.worldTags.GetTag("TRADER_STATION_BLOCKADE_EVENT_STARTED");
00021: 			tag.SetInt(tag.GetInt() + 1);
00022: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00023: 			{
00024: 				overworldSpace.isEventTriggered = true;
00025: 			}
00026: 		};
00027: 		e.AddChoice(DirectoryMaster.Item("ok"), "Attempt to sneak past the blockade.", "", null, null, delegate
00028: 		{
00029: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You have sucessfully sneak past the blockade and can now dock with the station.", RenderHandler.ColorPalette.LightGreen));
00030: 			e.blockStation = false;
00031: 		});
00032: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00033: 		{
00034: 		});
00035: 		return e;
00036: 	}
00037: }
~~~

## NewBehaviourScript.cs

~~~csharp
00039: 		EventData eventData = new EventData().SetText("Shipwreck", "You have arrived to an " + text + " derelict ship. The delirect ship is " + text2 + ", you can salvage something from it. ");
00040: 		eventData.text.AddLine("\r\nYour ship has a total salvage power of " + GameMaster.current.shipManager.GetShipSalvageCapacity(), ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00041: 		bool showLootOption = false;
00042: 		eventData.startAction = delegate
00043: 		{
00044: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_WRECK_EVENT_STARTED");
00045: 			tag.SetInt(tag.GetInt() + 1);
00046: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00047: 			{
00048: 				overworldSpace.isEventTriggered = true;
00049: 			}
00050: 			NavDiskHelper.HandleNavDiskOnArrivalToPoi();
00051: 		};
00052: 		GameItem container = DirectoryMaster.Item("lootbox_medium");
00053: 		int debrisLevel = 0;
00054: 		int damageTaken = 0;
00055: 		eventData.AddChoice(DirectoryMaster.Item("recycle"), "Salvage the derelict ship.", "", null, null, delegate
00056: 		{
00057: 			OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
00058: 			{
00059: 				GameMaster.current.turnManager.NextTurn();
00060: 			}, null, delegate
00061: 			{
00062: 				AudioManager.Instance.Play("WELD");
00063: 			});
00064: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew has salvage part of the wreck.", RenderHandler.ColorPalette.LightGreen));
00065: 			int shipSalvageCapacity = GameMaster.current.shipManager.GetShipSalvageCapacity();
00066: 			(damageTaken, debrisLevel) = SalvageSystemHelper.SalvageWreck(container, debrisLevel, shipSalvageCapacity, wreckState, shipSize);
00067: 			if (damageTaken != 0)
00068: 			{
00069: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("A collision has occured with the debris!", RenderHandler.ColorPalette.LightRed));
00070: 				SpaceBattleHelper.TakingHit(damageTaken);
00071: 			}
00072: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Current debris level: " + debrisLevel + "%", RenderHandler.ColorPalette.LightYellow));
00073: 			showLootOption = true;
00074: 			OverlayHandler.current.ShowInventory(container);
00075: 		});
00076: 		eventData.AddChoice(DirectoryMaster.Item("loot"), "Check Salvaged Items.", "", () => showLootOption, null, delegate
00077: 		{
00078: 			OverlayHandler.current.ShowInventory(container);
00079: 		});
00080: 		eventData.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00081: 		{
00082: 		});
00083: 		return eventData;
00084: 	}
00085: }
~~~

## TutorialTrigger.cs

~~~csharp
00019: 		eventData.text.Add("    You must explore and find");
00020: 		eventData.text.Add(" Energy Credit", RenderHandler.ColorPalette.LightGreen);
00021: 		eventData.text.Add(" and");
00022: 		eventData.text.Add(" Fuel Cell", RenderHandler.ColorPalette.LightGreen);
00023: 		eventData.text.Add(" to keep your ship powered and moving.");
00024: 		eventData.text.Add(" Food", RenderHandler.ColorPalette.LightGreen);
00025: 		eventData.text.Add(",");
00026: 		eventData.text.Add(" Water", RenderHandler.ColorPalette.LightGreen);
00027: 		eventData.text.Add(" and");
00028: 		eventData.text.Add(" Meds", RenderHandler.ColorPalette.LightGreen);
00029: 		eventData.text.Add(" to keep your crew alive.");
00030: 		eventData.SetImage("event_subspace");
00031: 		eventData.startAction = delegate
00032: 		{
00033: 			TagState tag = GameMaster.current.worldTags.GetTag("TUTORIAL_EVENT_STARTED");
00034: 			tag.SetInt(tag.GetInt() + 1);
00035: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00036: 			{
00037: 				overworldSpace.isEventTriggered = true;
00038: 			}
00039: 		};
00040: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Navigation System", "", null, null, delegate
00041: 		{
00042: 			for (int i = 0; i < 1; i++)
00043: 			{
00044: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
00045: 			}
00046: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Navigation System:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("   A Navigation System is present on every ship. When the ship is powered,", RenderHandler.ColorPalette.LightGreen).Add(" the Navigation System will draw energy from the ship to charge it self. Once fully charged, you can perform a scan that will reveal", RenderHandler.ColorPalette.LightGreen)
00047: 				.Add(" Points of Interest (POI).", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00048: 				.Add(" Like most Ship System, the Navigation System can be upgraded by inserting", RenderHandler.ColorPalette.LightGreen)
00049: 				.Add(" System Module.", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00050: 				.AddLine(" The number of POI scanned depends on the upgrade that the Navigation System have.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00051: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Points of Interest (POI)", RenderHandler.ColorPalette.LightCyan, "", bold: true).Add(" can be a friendly or hostile space station that you can", RenderHandler.ColorPalette.LightGreen).Add(" dock", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00052: 				.Add(" with and explore with your crew.", RenderHandler.ColorPalette.LightGreen)
00053: 				.AddLine(" POI can also be a unique or recurring event that take place in space.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00054: 		});
00055: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Propulsion System", "", null, null, delegate
00056: 		{
00057: 			for (int i = 0; i < 1; i++)
00058: 			{
00059: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
00060: 			}
00061: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Propulsion System:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("  A Propulsion System is present on every ship. It's used to move the ship between Points of Interest.", RenderHandler.ColorPalette.LightGreen).Add(" It only consume", RenderHandler.ColorPalette.LightGreen)
00062: 				.Add(" Exotic Fuel Cell", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00063: 				.AddLine(" placed inside the System's inventory", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00064: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Like most Ship System, the Propulsion System can be upgraded by inserting", RenderHandler.ColorPalette.LightGreen).Add(" System Module.", RenderHandler.ColorPalette.LightCyan, "", bold: true).AddLine(" Propulsion Speed and Fuel Consumption depends on the System's upgrade.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00065: 		});
00066: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Ship Hull", "", null, null, delegate
00067: 		{
00068: 			for (int i = 0; i < 1; i++)
00069: 			{
00070: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
00071: 			}
00072: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Ship Hull:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("  The hull of your ship is the only thing protecting the crew against the vaccum of subspace!", RenderHandler.ColorPalette.LightGreen).Add(" If the hull's", RenderHandler.ColorPalette.LightGreen)
00073: 				.Add(" Integrity", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00074: 				.Add(" reach 0, the ship will break and the crew will be killed!", RenderHandler.ColorPalette.LightGreen)
00075: 				.Add(" Armor", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00076: 				.AddLine(" can reduce the damage the Hull take.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00077: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Ship Hull can be repaired at a", RenderHandler.ColorPalette.LightGreen).Add(" Ship Mechanic. Maintenance Rate", RenderHandler.ColorPalette.LightCyan, "", bold: true).AddLine(" is a multiplier defining how easy a Hull can be repaired.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00078: 		});
~~~

~~~csharp
00045: 			}
00046: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Navigation System:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("   A Navigation System is present on every ship. When the ship is powered,", RenderHandler.ColorPalette.LightGreen).Add(" the Navigation System will draw energy from the ship to charge it self. Once fully charged, you can perform a scan that will reveal", RenderHandler.ColorPalette.LightGreen)
00047: 				.Add(" Points of Interest (POI).", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00048: 				.Add(" Like most Ship System, the Navigation System can be upgraded by inserting", RenderHandler.ColorPalette.LightGreen)
00049: 				.Add(" System Module.", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00050: 				.AddLine(" The number of POI scanned depends on the upgrade that the Navigation System have.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00051: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Points of Interest (POI)", RenderHandler.ColorPalette.LightCyan, "", bold: true).Add(" can be a friendly or hostile space station that you can", RenderHandler.ColorPalette.LightGreen).Add(" dock", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00052: 				.Add(" with and explore with your crew.", RenderHandler.ColorPalette.LightGreen)
00053: 				.AddLine(" POI can also be a unique or recurring event that take place in space.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00054: 		});
00055: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Propulsion System", "", null, null, delegate
00056: 		{
00057: 			for (int i = 0; i < 1; i++)
00058: 			{
00059: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
00060: 			}
00061: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Propulsion System:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("  A Propulsion System is present on every ship. It's used to move the ship between Points of Interest.", RenderHandler.ColorPalette.LightGreen).Add(" It only consume", RenderHandler.ColorPalette.LightGreen)
00062: 				.Add(" Exotic Fuel Cell", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00063: 				.AddLine(" placed inside the System's inventory", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00064: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Like most Ship System, the Propulsion System can be upgraded by inserting", RenderHandler.ColorPalette.LightGreen).Add(" System Module.", RenderHandler.ColorPalette.LightCyan, "", bold: true).AddLine(" Propulsion Speed and Fuel Consumption depends on the System's upgrade.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00065: 		});
00066: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On Ship Hull", "", null, null, delegate
00067: 		{
00068: 			for (int i = 0; i < 1; i++)
00069: 			{
00070: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
00071: 			}
00072: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Ship Hull:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).Add("  The hull of your ship is the only thing protecting the crew against the vaccum of subspace!", RenderHandler.ColorPalette.LightGreen).Add(" If the hull's", RenderHandler.ColorPalette.LightGreen)
00073: 				.Add(" Integrity", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00074: 				.Add(" reach 0, the ship will break and the crew will be killed!", RenderHandler.ColorPalette.LightGreen)
00075: 				.Add(" Armor", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00076: 				.AddLine(" can reduce the damage the Hull take.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00077: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("  Ship Hull can be repaired at a", RenderHandler.ColorPalette.LightGreen).Add(" Ship Mechanic. Maintenance Rate", RenderHandler.ColorPalette.LightCyan, "", bold: true).AddLine(" is a multiplier defining how easy a Hull can be repaired.", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen));
00078: 		});
00079: 		eventData.AddChoice(DirectoryMaster.Item("question"), "Basic Tutorial On crew survival", "", null, null, delegate
00080: 		{
00081: 			for (int i = 0; i < 1; i++)
00082: 			{
00083: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine(" "));
00084: 			}
00085: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().AddLine("Tutorial on Crew Survival:", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen, bold: true).AddLine(" All crewmate have a", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen).Add(" Thirst", RenderHandler.ColorPalette.LightCyan, "", bold: true)
00086: 				.Add(" stat. If it reaches 0, the crewmate will die.", RenderHandler.ColorPalette.LightGreen)
00087: 				.Add(" Low thirst will give a penalty to that crewmate in combat,", RenderHandler.ColorPalette.LightOrange)
00088: 				.Add(" so make sure the crew stays hydrated. Drinks can be found in POI all over the Subspace.", RenderHandler.ColorPalette.LightGreen));
00089: 		});
00090: 		eventData.AddChoice(DirectoryMaster.Item("ok"), "Begin.", "", null, null, delegate
00091: 		{
00092: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You can start exploring the Subspace by starting a Scan in the ", RenderHandler.ColorPalette.LightGreen).Add("Navigation", RenderHandler.ColorPalette.LightCyan, "", bold: true).Add(" Interface", RenderHandler.ColorPalette.LightGreen));
00093: 		});
00094: 		return eventData;
00095: 	}
00096: }
~~~

## OverlayHandler.cs

~~~csharp
00333: 		gridPixelElement2.Attach(eventSprite, 0, 270);
00334: 		gridPixelElement2.Attach(eventButton);
00335: 		gridPixelElement2.Validate();
00336: 		gridPixelElement.Attach(eventLog);
00337: 		gridPixelElement.Attach(gridPixelElement2);
00338: 		gridPixelElement.Validate();
00339: 		eventWindow.Attach(gridPixelElement);
00340: 		eventWindow.Validate();
00341: 	}
00342: 
00343: 	public void HideEvent()
00344: 	{
00345: 		if (eventWindow.IsVisible())
00346: 		{
00347: 			((Behaviour)confirmBackgroundNode.image).enabled = false;
00348: 			eventWindow.Hide();
00349: 			currentCancelAction = null;
00350: 		}
00351: 	}
00352: 
00353: 	public void PushEvent()
00354: 	{
00355: 		EventData currentEvent = GameMaster.current.shipManager.currentEvent;
00356: 		eventWindow.SetTitle(currentEvent.title);
00357: 		eventLog.AppendRichText(currentEvent.text);
00358: 		eventLog.AppendRichText(new RichTextBuilder());
00359: 		eventLog.SetScroll(1f);
00360: 		if (currentEvent.imagePath != null)
00361: 		{
00362: 			eventSprite.SetSprite(currentEvent.imagePath);
00363: 			((Behaviour)eventSprite.handler).enabled = true;
00364: 		}
00365: 		else
00366: 		{
00367: 			((Behaviour)eventSprite.handler).enabled = false;
00368: 		}
00369: 	}
00370: 
00371: 	public void PushEventRichText(RichTextBuilder text)
00372: 	{
00373: 		eventLog.AppendRichText(text);
00374: 		eventLog.AppendRichText(new RichTextBuilder());
00375: 		eventLog.SetScroll(1f);
00376: 	}
00377: 
00378: 	public void ShowEvent()
00379: 	{
00380: 		hideNextAutoShowEvent = false;
00381: 		EventData currentEvent = GameMaster.current.shipManager.currentEvent;
00382: 		if (currentEvent != null && currentEvent.IsRunning())
00383: 		{
00384: 			bool flag = currentEvent.choices.Count() > 0;
00385: 			bool flag2 = currentEvent.choices.FindAll(((GameItem, Func<bool>, Func<bool>, Action) triplet) => triplet.Item2?.Invoke() ?? true).Any();
00386: 			bool flag3 = currentEvent.canEndFunc?.Invoke() ?? true;
00387: 			if (flag && flag2)
00388: 			{
00389: 				eventButton.Set("Choose Action").Enable();
00390: 				eventButton.SetCallback(delegate
00391: 				{
00392: 					HideEvent();
~~~

~~~csharp
00900: 		((Behaviour)fadeBackgroundNode.image).enabled = false;
00901: 		isFadeBlock = false;
00902: 		SafeFadeInvoke(fadeEndAction, "end");
00903: 	}
00904: 
00905: 	private static void SafeFadeInvoke(Action action, string stage)
00906: 	{
00907: 		if (action == null)
00908: 		{
00909: 			return;
00910: 		}
00911: 		try
00912: 		{
00913: 			action();
00914: 		}
00915: 		catch (Exception arg)
00916: 		{
00917: 			Debug.LogError($"[OverlayHandler] fade {stage} action threw: {arg}");
00918: 		}
00919: 	}
00920: 
00921: 	public void ShowConfirmation(string title = "Confirmation", RichText richText = null, Action yesAction = null, Action noAction = null, bool canYes = true, bool showYes = true, string yes = "Confirm", string no = "Cancel", ButtonElement.Color yesColor = ButtonElement.Color.Green, ButtonElement.Color noColor = ButtonElement.Color.Red)
00922: 	{
00923: 		if (richText == null)
00924: 		{
00925: 			richText = new RichTextBuilder().Add("Do you want to confirm this action?").Build();
00926: 		}
00927: 		confirmYesButton.Set(yes);
00928: 		confirmNoButton.Set(no);
00929: 		((Behaviour)confirmBackgroundNode.image).enabled = true;
00930: 		((Transform)((Graphic)confirmBackgroundNode.image).rectTransform).SetAsLastSibling();
00931: 		confirmTextElement.SetRichText(richText);
00932: 		if (showYes)
00933: 		{
00934: 			confirmYesButton.Show();
00935: 		}
00936: 		else
00937: 		{
00938: 			confirmYesButton.Hide();
00939: 		}
00940: 		confirmYesNoElement.Validate();
00941: 		confirmWindow.Validate();
00942: 		confirmWindow.Show();
00943: 		confirmWindow.Center();
00944: 		((Transform)confirmWindow.rectTransform).SetAsLastSibling();
00945: 		confirmWindow.SetTitle(title);
00946: 		if (canYes)
00947: 		{
00948: 			confirmYesButton.SetColor(yesColor).Enable();
00949: 		}
00950: 		else
00951: 		{
00952: 			confirmYesButton.SetColor(yesColor).Disable();
00953: 		}
00954: 		confirmNoButton.SetColor(noColor).Enable();
00955: 		confirmYesButton.SetCallback(delegate
00956: 		{
00957: 			HideConfirmation();
00958: 			yesAction?.Invoke();
00959: 		});
~~~

~~~csharp
01061: 
01062: 	public void ShowTooltip(Vector2 position, Vector2 localTranslation, RichText richText, int maxCharWidth = 60)
01063: 	{
01064: 		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
01065: 		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
01066: 		if (richText != null)
01067: 		{
01068: 			if (!richText.QuickEquals(previousRichTextCache))
01069: 			{
01070: 				tooltipElement.SetRichText(richText, maxCharWidth);
01071: 				tooltipWindow.Validate();
01072: 			}
01073: 			previousRichTextCache = richText;
01074: 			tooltipWindow.Show(position, localTranslation, fixTooltipBounds: true);
01075: 			tooltipWindow.ToFront(force: true);
01076: 		}
01077: 	}
01078: 
01079: 	public void HideTooltip()
01080: 	{
01081: 		tooltipWindow.Hide();
01082: 	}
01083: 
01084: 	public void AppendLog(string text, bool scrollDown = true)
01085: 	{
01086: 		AppendLog(new RichTextBuilder().Add(text), scrollDown);
01087: 	}
01088: 
01089: 	public void AppendLog(RichTextBuilder textBuilder, bool scrollDown = true)
01090: 	{
01091: 		logElement.AppendRichText(textBuilder);
01092: 		if (scrollDown)
01093: 		{
01094: 			logElement.SetScroll(1f);
01095: 		}
01096: 	}
01097: 
01098: 	public void ClearLog()
01099: 	{
01100: 		logElement.SetRichText(new RichTextBuilder());
01101: 	}
01102: 
01103: 	public bool Cancel()
01104: 	{
01105: 		if (currentCancelAction != null)
01106: 		{
01107: 			currentCancelAction?.Invoke();
01108: 			return true;
01109: 		}
01110: 		return false;
01111: 	}
01112: 
01113: 	public PixelWindow GetTooltipWindow()
01114: 	{
01115: 		return tooltipWindow;
01116: 	}
01117: }
~~~

## ItemTargetBehaviour.cs

~~~csharp
00018: 		return MayTarget(source, target) ?? (CanTarget(source, target) == true);
00019: 	}
00020: 
00021: 	public bool CheckCan(GameItem source, GameItem target)
00022: 	{
00023: 		bool? flag = MayTarget(source, target);
00024: 		if ((!flag) ?? false)
00025: 		{
00026: 			return false;
00027: 		}
00028: 		return CanTarget(source, target) ?? (flag == true);
00029: 	}
00030: 
00031: 	public RichTextBuilder GetTooltip(GameItem source, GameItem target)
00032: 	{
00033: 		if ((!ShowTooltip(source, target)) ?? false)
00034: 		{
00035: 			return null;
00036: 		}
00037: 		RichTextBuilder richTextBuilder = TargetRichTooltip(source, target);
00038: 		if (richTextBuilder == null)
00039: 		{
00040: 			string text = TargetTooltip(source, target);
00041: 			if (text != null)
00042: 			{
00043: 				return new RichTextBuilder().AddLine(text);
00044: 			}
00045: 		}
00046: 		return richTextBuilder;
00047: 	}
00048: 
00049: 	public virtual bool? Filter(GameItem source, GameItem target)
00050: 	{
00051: 		return null;
00052: 	}
00053: 
00054: 	public virtual bool? MayTarget(GameItem source, GameItem target)
00055: 	{
00056: 		return null;
00057: 	}
00058: 
00059: 	public virtual bool? CanTarget(GameItem source, GameItem target)
00060: 	{
00061: 		return null;
00062: 	}
00063: 
00064: 	public virtual bool? CanTargetPassthrough(GameItem source, GameItem target)
00065: 	{
00066: 		return null;
00067: 	}
00068: 
00069: 	public virtual bool? OnTarget(GameItem source, GameItem target)
00070: 	{
00071: 		return null;
00072: 	}
00073: 
00074: 	public virtual bool? ShowTooltip(GameItem source, GameItem target)
00075: 	{
00076: 		return null;
00077: 	}
~~~

## ItemMouseDragHandler.cs

~~~csharp
00020: 	public KeyCode scrollMult10Key;
00021: 
00022: 	[SerializeField]
00023: 	public KeyCode scrollMult100Key;
00024: 
00025: 	[SerializeField]
00026: 	public KeyCode scrollIgnoreKey;
00027: 
00028: 	[SerializeField]
00029: 	public float dragSnapAnimationSpeed;
00030: 
00031: 	private GameItemElement currentItem;
00032: 
00033: 	private Vector3 lastMouseClickedPosition;
00034: 
00035: 	private KeyCode lastMouseClickedKey;
00036: 
00037: 	private bool isDragging;
00038: 
00039: 	private bool isMult10;
00040: 
00041: 	private bool isMult100;
00042: 
00043: 	private bool isScrollIgnore;
00044: 
00045: 	private RichTextBuilder builder = new RichTextBuilder(60);
00046: 
00047: 	private int maxRemove;
00048: 
00049: 	private int currentRemove;
00050: 
00051: 	private List<TreeNodeRender> highlightedMayNodes = new List<TreeNodeRender>();
00052: 
00053: 	private GameItem lastItem;
00054: 
00055: 	private GameInventory lastInventory;
00056: 
00057: 	private SlotMarker lastSlot;
00058: 
00059: 	private HighlightManager highlightManager = new HighlightManager();
00060: 
00061: 	public override List<KeyCode> keyListeners => ((IEnumerable<KeyCode>)(object)new KeyCode[10]
00062: 	{
00063: 		(KeyCode)(int)key,
00064: 		(KeyCode)(int)rotateCWKey,
00065: 		(KeyCode)(int)rotateCCWKey,
00066: 		(KeyCode)(int)flipKey,
00067: 		(KeyCode)(int)resetKey,
00068: 		(KeyCode)(int)scrollMult10Key,
00069: 		(KeyCode)(int)scrollMult100Key,
00070: 		(KeyCode)(int)scrollIgnoreKey,
00071: 		(KeyCode)325,
00072: 		(KeyCode)324
00073: 	}).ToList();
00074: 
00075: 	public KeyCode rotateCWKey => KeyBindHelper.Get("rotate_cw");
00076: 
00077: 	public KeyCode rotateCCWKey => KeyBindHelper.Get("rotate_ccw");
00078: 
00079: 	public KeyCode flipKey => KeyBindHelper.Get("flip_item");
~~~

## SpaceBattleTrigger.cs

~~~csharp
00007: 		{
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData e = new EventData().SetText("Space Battle", "You arrive in the middle of a massive space battle. Two fleets are attacking each other.");
00016: 		e.SetImage("event_fleet_battle");
00017: 		e.text.AddLine("\r\nYou made sure your transponder is on, but in the heat of the battle there is no garantee that they won't fire on a unknown ship on sight. You must get clear of the battle before jumping again.", ignoreEmpty: true, RenderHandler.ColorPalette.LightYellow);
00018: 		e.startAction = delegate(EventData eventData)
00019: 		{
00020: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_BATTLE_EVENT_STARTED");
00021: 			eventData.BlockTravel();
00022: 			tag.SetInt(tag.GetInt() + 1);
00023: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00024: 			{
00025: 				overworldSpace.isEventTriggered = true;
00026: 			}
00027: 		};
00028: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Attempt to escape the battle.", "", null, null, delegate
00029: 		{
00030: 			if (RNG.Roll(50))
00031: 			{
00032: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You attempt to escape away from the battle with your ship and but during the heat of battle, your ship were fired upon.", RenderHandler.ColorPalette.LightRed));
00033: 				SpaceBattleHelper.TakingHit(25);
00034: 				e.ForceEnd();
00035: 			}
00036: 			else
00037: 			{
00038: 				OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("You manage to safetly escape away from the battle without being fired upon!", RenderHandler.ColorPalette.LightGreen));
00039: 				e.ForceEnd();
00040: 			}
00041: 		});
00042: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00043: 		{
00044: 		});
00045: 		return e;
00046: 	}
00047: }
~~~

## Dialogue.cs

~~~csharp
00151: 		foreach (var choice in choices)
00152: 		{
00153: 			if (choice.Item1 == choiceItem)
00154: 			{
00155: 				return choice.Item3;
00156: 			}
00157: 		}
00158: 		return null;
00159: 	}
00160: 
00161: 	public Dialogue FindChoiceDialogue(GameItem choiceItem)
00162: 	{
00163: 		foreach (var choice in choices)
00164: 		{
00165: 			if (choice.Item1 == choiceItem)
00166: 			{
00167: 				return choice.Item2;
00168: 			}
00169: 		}
00170: 		return null;
00171: 	}
00172: 
00173: 	public Dialogue SetText(string title, string text)
00174: 	{
00175: 		this.title = title;
00176: 		this.text = new RichTextBuilder().Add(text);
00177: 		plainText = text;
00178: 		return this;
00179: 	}
00180: 
00181: 	public Dialogue SetText(string title, RichTextBuilder text)
00182: 	{
00183: 		this.title = title;
00184: 		this.text = new RichTextBuilder().Add(text);
00185: 		return this;
00186: 	}
00187: 
00188: 	public Dialogue SetDummy()
00189: 	{
00190: 		isDummyChoiceDialogue = true;
00191: 		return this;
00192: 	}
00193: 
00194: 	public Dialogue SetPortrait(string atlasPath, string spriteName)
00195: 	{
00196: 		portrait = RenderHandler.LoadFromAtlas(atlasPath, spriteName);
00197: 		return this;
00198: 	}
00199: 
00200: 	public Dialogue SetPortrait(Sprite portrait)
00201: 	{
00202: 		this.portrait = portrait;
00203: 		return this;
00204: 	}
00205: 
00206: 	public Dialogue ForceDisable()
00207: 	{
00208: 		forceDisabled = true;
00209: 		return this;
00210: 	}
~~~

## MachineryButton.cs

~~~csharp
00325: 	{
00326: 		((Behaviour)plate.image).enabled = false;
00327: 		((Behaviour)iconNode.image).enabled = false;
00328: 		((Behaviour)textNode.text).enabled = false;
00329: 		base.widthPixels = 0;
00330: 		base.heightPixels = 0;
00331: 		MachineryContainerUtils.NotifySizeChanged(this);
00332: 	}
00333: 
00334: 	protected override void OnShow()
00335: 	{
00336: 		((Behaviour)plate.image).enabled = true;
00337: 		((Behaviour)iconNode.image).enabled = (Object)(object)icon != (Object)null;
00338: 		((Behaviour)textNode.text).enabled = hasText;
00339: 		Validate();
00340: 		MachineryContainerUtils.NotifySizeChanged(this);
00341: 	}
00342: 
00343: 	public override bool HasTooltip()
00344: 	{
00345: 		return !string.IsNullOrEmpty(description);
00346: 	}
00347: 
00348: 	public override RichText GetTooltip()
00349: 	{
00350: 		return new RichTextBuilder().Add(description).Build();
00351: 	}
00352: }
~~~

## SpaceGhostShipTrigger.cs

~~~csharp
00005: 		GameMaster.current.worldTags.GetTag("SPACE_GHOST_SPACE_EVENT_STARTED");
00006: 		if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace && overworldSpace.identifier.Equals("ghost_ship") && !GameMaster.current.shipManager.isTransit && !overworldSpace.isEventTriggered)
00007: 		{
00008: 			return true;
00009: 		}
00010: 		return null;
00011: 	}
00012: 
00013: 	public override EventData GenerateEvent()
00014: 	{
00015: 		EventData e = new EventData().SetText("Drifting Ship", "You have arrived at a small transport ship drifting aimlessly into the space. \r\n        The ship is not responding to your communication attempt. ");
00016: 		e.SetImage("event_ghost_ship");
00017: 		e.text.AddLine("\r\n        Scanner shows that the ship is operational", ignoreEmpty: true, RenderHandler.ColorPalette.LightGreen);
00018: 		e.startAction = delegate
00019: 		{
00020: 			TagState tag = GameMaster.current.worldTags.GetTag("SPACE_GHOST_SPACE_EVENT_STARTED");
00021: 			tag.SetInt(tag.GetInt() + 1);
00022: 			if (GameMaster.current.shipManager.nextPOI is OverworldSpace overworldSpace)
00023: 			{
00024: 				overworldSpace.isEventTriggered = true;
00025: 			}
00026: 		};
00027: 		bool boarded = false;
00028: 		e.AddChoice(DirectoryMaster.Item("search"), "Board and investigate the ship.", "", null, () => !boarded, delegate
00029: 		{
00030: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("After boarding the ship, your crew has not found anyone dead or alive on it. Some opened food are still fresh, cleary people were living on the ship recently. Stranger yet, all ship's systems are intact, there still credits and fuel on the ship."));
00031: 			OverlayHandler.current.PushEventRichText(new RichTextBuilder().Add("Your crew came back with the fuel and the credits they found from the preliminary search.", RenderHandler.ColorPalette.LightGreen));
00032: 			GameItem gameItem = DirectoryMaster.Item("backpack_medium");
00033: 			GameItem item = DirectoryMaster.Item("exotic_fuel").SetUnitCount(20);
00034: 			GameItem item2 = DirectoryMaster.Item("energy_credit");
00035: 			GraphUtils.TryAcceptAll(gameItem, item);
00036: 			GraphUtils.TryAcceptAll(gameItem, item2);
00037: 			OverlayHandler.current.ShowEvent();
00038: 			OverlayHandler.current.ShowInventory(gameItem);
00039: 			boarded = true;
00040: 		});
00041: 		e.AddChoice(DirectoryMaster.Item("ok"), "Salvage the ship.", "", null, () => boarded, delegate
00042: 		{
00043: 		});
00044: 		e.AddChoice(DirectoryMaster.Item("open_fire"), "(Hostile) Open fire on the ship.", "", null, null, delegate
00045: 		{
00046: 			e.ForceEnd();
00047: 		});
00048: 		e.AddChoice(DirectoryMaster.Item("cancel"), "Do nothing for now.", "", null, null, delegate
00049: 		{
00050: 		});
00051: 		return e;
00052: 	}
00053: }
~~~

## MenuScene.cs

~~~csharp
00111: 	private void InitSelect()
00112: 	{
00113: 		nextButtonWindow = new PixelWindow();
00114: 		prevButtonWindow = new PixelWindow();
00115: 		topBarWindow = new PixelWindow();
00116: 		topBarWindow.AddInventoryStateChangeAction(delegate
00117: 		{
00118: 			UpdateTopBarText();
00119: 		});
00120: 		nextButton = new ButtonElement("Next", defaultButtonWidth, defaultButtonHeight);
00121: 		prevButton = new ButtonElement("Back", defaultButtonWidth, defaultButtonHeight);
00122: 		nextButton.SetCallback(delegate
00123: 		{
00124: 			switch (selectState)
00125: 			{
00126: 			case 1:
00127: 				StartScenarioSelect();
00128: 				break;
00129: 			case 2:
00130: 				StartCharSelect();
00131: 				break;
00132: 			case 3:
00133: 				StartEquipmentSelect();
00134: 				break;
00135: 			case 4:
00136: 				OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder().Add("Are you sure you want to start the game?").Build(), delegate
00137: 				{
00138: 					AudioHelper.ShuttleJump();
00139: 					OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
00140: 					{
00141: 						GameMaster.current.NewGame();
00142: 					});
00143: 				});
00144: 				break;
00145: 			default:
00146: 				StartMenu();
00147: 				break;
00148: 			}
00149: 		});
00150: 		prevButton.SetCallback(delegate
00151: 		{
00152: 			switch (selectState)
00153: 			{
00154: 			case 2:
00155: 				StartFactionSelect();
00156: 				break;
00157: 			case 3:
00158: 				StartScenarioSelect();
00159: 				break;
00160: 			case 4:
00161: 				StartCharSelect();
00162: 				break;
00163: 			default:
00164: 				OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder().Add("Are you sure you want to cancel and go back to the main menu?").Build(), StartMenu);
00165: 				break;
00166: 			}
00167: 		});
00168: 		nextButtonWindow.Attach(nextButton);
00169: 		prevButtonWindow.Attach(prevButton);
00170: 		topBarText = new RichTextElement(canResize: false, UISettings.current.fontWidth * BAR_WIDTH, UISettings.current.fontHeight * BAR_HEIGHT);
~~~

~~~csharp
00139: 					OverlayHandler.current.FadeBlock(0.2f, 0.6f, delegate
00140: 					{
00141: 						GameMaster.current.NewGame();
00142: 					});
00143: 				});
00144: 				break;
00145: 			default:
00146: 				StartMenu();
00147: 				break;
00148: 			}
00149: 		});
00150: 		prevButton.SetCallback(delegate
00151: 		{
00152: 			switch (selectState)
00153: 			{
00154: 			case 2:
00155: 				StartFactionSelect();
00156: 				break;
00157: 			case 3:
00158: 				StartScenarioSelect();
00159: 				break;
00160: 			case 4:
00161: 				StartCharSelect();
00162: 				break;
00163: 			default:
00164: 				OverlayHandler.current.ShowConfirmation("Confirmation", new RichTextBuilder().Add("Are you sure you want to cancel and go back to the main menu?").Build(), StartMenu);
00165: 				break;
00166: 			}
00167: 		});
00168: 		nextButtonWindow.Attach(nextButton);
00169: 		prevButtonWindow.Attach(prevButton);
00170: 		topBarText = new RichTextElement(canResize: false, UISettings.current.fontWidth * BAR_WIDTH, UISettings.current.fontHeight * BAR_HEIGHT);
00171: 		topBarWindow.Attach(topBarText);
00172: 	}
00173: 
00174: 	private void InitFactionSelect()
00175: 	{
00176: 		difficultySelectWindow = new PixelWindow();
00177: 		factionSelectWindow = new PixelWindow();
00178: 	}
00179: 
00180: 	private void InitScenarioSelect()
00181: 	{
00182: 		scenarioDescriptionWindow = new PixelWindow();
00183: 		scenarioDescription = new ScrollableRichTextElement(constWidth: true);
00184: 		scenarioDescriptionWindow.Attach(scenarioDescription);
00185: 		shipNextButtonWindow = new PixelWindow();
00186: 		shipPrevButtonWindow = new PixelWindow();
00187: 		shipNextButton = new ButtonElement(">", defaultShipButtonWidth, defaultButtonHeight, null, ButtonElement.Color.None, "click_vintage");
00188: 		shipPrevButton = new ButtonElement("<", defaultShipButtonWidth, defaultButtonHeight, null, ButtonElement.Color.None, "click_vintage");
00189: 		shipNextButtonWindow.Attach(shipNextButton);
00190: 		shipPrevButtonWindow.Attach(shipPrevButton);
00191: 		shipNextButton.SetCallback(delegate
00192: 		{
00193: 			scenarioSelectState++;
00194: 			scenarioSelectState %= availableScenarios.Count;
00195: 			UpdateScenario();
00196: 		});
00197: 		shipPrevButton.SetCallback(delegate
00198: 		{
~~~

~~~csharp
00482: 		ItemMouseDragHandler.current.Enable();
00483: 		ItemMouseHandler.current.Enable();
00484: 		ItemQuickTransferHandler.current.Enable();
00485: 		ItemSelectHandler.current.Enable();
00486: 		ItemTooltipHandler.current.Enable();
00487: 		ItemContextHandler.current.Enable();
00488: 	}
00489: 
00490: 	public void StartEquipmentSelect()
00491: 	{
00492: 		Reset();
00493: 		selectState = 4;
00494: 		UpdateTopBarText();
00495: 		nextButton.Set("Begin!").SetColor(ButtonElement.Color.Red);
00496: 		nextButtonWindow.Show();
00497: 		prevButtonWindow.Show();
00498: 		topBarWindow.Show();
00499: 		spaceBackground.ToBack();
00500: 		spaceBackground.Show();
00501: 		isActive = true;
00502: 		Validate();
00503: 	}
00504: 
00505: 	private void UpdateTopBarText()
00506: 	{
00507: 		RichTextBuilder richTextBuilder = new RichTextBuilder();
00508: 		switch (selectState)
00509: 		{
00510: 		case 1:
00511: 			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Faction Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
00512: 			break;
00513: 		case 2:
00514: 			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Scenario Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
00515: 			break;
00516: 		case 3:
00517: 			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Character Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
00518: 			break;
00519: 		case 4:
00520: 			richTextBuilder.AddLine(OverlayHelper.GetLineCentered(BAR_WIDTH, "Equipment Selection"), ignoreEmpty: true, RenderHandler.ColorPalette.White, bold: true);
00521: 			break;
00522: 		}
00523: 		long remainingPoints = GetRemainingPoints();
00524: 		string text = "Points: ";
00525: 		richTextBuilder.Add(OverlayHelper.GetSpacesCentered(BAR_WIDTH, text + remainingPoints));
00526: 		richTextBuilder.Add(text);
00527: 		richTextBuilder.Add(remainingPoints.ToString(), (remainingPoints >= 0) ? RenderHandler.ColorPalette.LightGreen : RenderHandler.ColorPalette.LightRed);
00528: 		topBarText.SetRichText(richTextBuilder.Build());
00529: 	}
00530: 
00531: 	private long GetRemainingPoints()
00532: 	{
00533: 		if (GameMaster.current.menuManager.selectedScenario == null)
00534: 		{
00535: 			return 0L;
00536: 		}
00537: 		List<GameInventory> list = GraphUtils.FindChildrenType<GameInventory>(GameMaster.current.menuManager.selectedScenario.ship);
00538: 		long num = 0L;
00539: 		foreach (GameInventory item in list)
00540: 		{
00541: 			num += item.GetTotalValue();
~~~
