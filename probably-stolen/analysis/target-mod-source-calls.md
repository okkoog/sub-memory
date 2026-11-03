# Target mod source-label calls

## 3832048841

- no source-label call found

## 3831837821

- no source-label call found

## 3829429358

### XIAOWONightAuction/OfficialAssets.cs:70
~~~csharp
0067: 	{
0068: 		item.SetSprite("xiaowo_night_auction", key);
0069: 		item.useSpriteFromMod = true;
0070: 		ModHelper.SetSource(item, "xiaowo_night_auction", "小窝竞拍之王");
0071: 	}
0072: 
0073: 	internal static void Clear()
~~~

## 3829085054

- no source-label call found

## 3829730904

### BrewingExpansion/Formal/BarrelWine.cs:113
~~~csharp
0110: 			}
0111: 			bottle.EnableTag("BE_FORMAL_WINE_" + id, false);
0112: 			bottle.identifier = id;
0113: 			ModHelper.SetSource(bottle, "brewing_expansion", ModText.Get("be_mod_name"));
0114: 			BarrelWineLoad.Attach(bottle);
0115: 		}
0116: 	}
~~~

## 3828913695

### CorpPresence/Malice/Malice.cs:60
~~~csharp
0057: 	{
0058: 		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
0059: 		//IL_004d: Invalid comparison between Unknown and I4
0060: 		if (c == null || !string.IsNullOrEmpty(c.sourceModId))
0061: 		{
0062: 			return false;
0063: 		}
~~~
