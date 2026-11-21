# Probably Adult Goods v1.1.1 — Korean embedded resource preparation

- Source: original 1.1.1 DLL managed resource `ProbablyAdultGoods.Localization.zh.csv` (181 keys, original SHA256 41d3adcbde8e30f0fae63e2905471358d10014710a9a03813e8e8f7b7a6c3c84).
- Output: `TentacleMod번역/3842259965/Localization/ko.csv` (181 keys, original key order, CRLF).
- Previously reviewed Korean strings reused only where original Chinese text is byte-for-byte identical to the source text in the earlier v1.03 localization file: 104 same-key values and 37 moved `*_flavor_alt` → `*_flavor` values.
- Newly translated independently against original Chinese: 38 UI, visitor dialogue, item cabinet and settings values.
- Two item-description entries are adapted from the **original English localization** rather than the Chinese source and must not be counted as literal Chinese-to-Korean translations.
- Original 1.1.1 DLL has 181 locale rows, whereas the older v1.03 version used for translation reuse has 192 rows. A pure key-based mass replacement would be incorrect.
- This file is currently a **source input only**, not a DLL patch. It must be embedded in a separately rebuilt 1.1.1 DLL and the Korean language must be registered in the original localization loader.
- Original Chinese and English resources, item identifiers, file paths and CLR program logic must remain untouched except for necessary Korean resource registration. Game/Unity loading untested.

## Source attribution by key

| key | translation basis |
| --- | --- |
| `mod_name` | prior verified Korean, same key and exact Chinese value |
| `type_adult_product` | prior verified Korean, same key and exact Chinese value |
| `box_condom_001_name` | prior verified Korean, same key and exact Chinese value |
| `box_condom_001_flavor` | prior verified Korean, exact Chinese value at box_condom_001_flavor_alt |
| `box_condom_001_desc` | prior verified Korean, same key and exact Chinese value |
| `box_condom_002_name` | prior verified Korean, same key and exact Chinese value |
| `box_condom_002_flavor` | prior verified Korean, exact Chinese value at box_condom_002_flavor_alt |
| `box_condom_002_desc` | prior verified Korean, same key and exact Chinese value |
| `box_condom_003_name` | prior verified Korean, same key and exact Chinese value |
| `box_condom_003_flavor` | prior verified Korean, exact Chinese value at box_condom_003_flavor_alt |
| `box_condom_003_desc` | prior verified Korean, same key and exact Chinese value |
| `box_birth_control_name` | prior verified Korean, same key and exact Chinese value |
| `box_birth_control_flavor` | prior verified Korean, exact Chinese value at box_birth_control_flavor_alt |
| `box_birth_control_desc` | prior verified Korean, same key and exact Chinese value |
| `condom_001_name` | prior verified Korean, same key and exact Chinese value |
| `condom_001_flavor` | prior verified Korean, exact Chinese value at condom_001_flavor_alt |
| `condom_001_desc` | prior verified Korean, same key and exact Chinese value |
| `condom_002_name` | prior verified Korean, same key and exact Chinese value |
| `condom_002_flavor` | prior verified Korean, exact Chinese value at condom_002_flavor_alt |
| `condom_002_desc` | prior verified Korean, same key and exact Chinese value |
| `condom_003_name` | prior verified Korean, same key and exact Chinese value |
| `condom_003_flavor` | prior verified Korean, exact Chinese value at condom_003_flavor_alt |
| `condom_003_desc` | prior verified Korean, same key and exact Chinese value |
| `used_condom_001_name` | prior verified Korean, same key and exact Chinese value |
| `used_condom_001_flavor` | prior verified Korean, exact Chinese value at used_condom_001_flavor_alt |
| `used_condom_001_desc` | prior verified Korean, same key and exact Chinese value |
| `used_condom_002_name` | prior verified Korean, same key and exact Chinese value |
| `used_condom_002_flavor` | prior verified Korean, exact Chinese value at used_condom_002_flavor_alt |
| `used_condom_002_desc` | prior verified Korean, same key and exact Chinese value |
| `used_condom_003_name` | prior verified Korean, same key and exact Chinese value |
| `used_condom_003_flavor` | prior verified Korean, exact Chinese value at used_condom_003_flavor_alt |
| `used_condom_003_desc` | prior verified Korean, same key and exact Chinese value |
| `dildo_red_name` | prior verified Korean, same key and exact Chinese value |
| `dildo_red_flavor` | English embedded resource (Chinese wording excluded) |
| `dildo_red_desc` | prior verified Korean, same key and exact Chinese value |
| `dildo_pink_slim_name` | prior verified Korean, same key and exact Chinese value |
| `dildo_pink_slim_flavor` | English embedded resource (Chinese wording excluded) |
| `dildo_pink_slim_desc` | prior verified Korean, same key and exact Chinese value |
| `dildo_pink_thick_name` | prior verified Korean, same key and exact Chinese value |
| `dildo_pink_thick_flavor` | prior verified Korean, exact Chinese value at dildo_pink_thick_flavor_alt |
| `dildo_pink_thick_desc` | prior verified Korean, same key and exact Chinese value |
| `dildo_purple_name` | prior verified Korean, same key and exact Chinese value |
| `dildo_purple_flavor` | prior verified Korean, exact Chinese value at dildo_purple_flavor_alt |
| `dildo_purple_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_1_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_1_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_1_flavor_alt |
| `fantasy_dragon_1_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_2_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_2_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_2_flavor_alt |
| `fantasy_dragon_2_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_3_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_3_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_3_flavor_alt |
| `fantasy_dragon_3_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_4_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_4_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_4_flavor_alt |
| `fantasy_dragon_4_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_5_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_5_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_5_flavor_alt |
| `fantasy_dragon_5_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_6_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_6_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_6_flavor_alt |
| `fantasy_dragon_6_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_7_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_7_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_7_flavor_alt |
| `fantasy_dragon_7_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_8_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_8_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_8_flavor_alt |
| `fantasy_dragon_8_desc` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_9_name` | prior verified Korean, same key and exact Chinese value |
| `fantasy_dragon_9_flavor` | prior verified Korean, exact Chinese value at fantasy_dragon_9_flavor_alt |
| `fantasy_dragon_9_desc` | prior verified Korean, same key and exact Chinese value |
| `anal_beads_name` | prior verified Korean, same key and exact Chinese value |
| `anal_beads_flavor` | prior verified Korean, exact Chinese value at anal_beads_flavor_alt |
| `anal_beads_desc` | prior verified Korean, same key and exact Chinese value |
| `anal_beads_tail_cat_name` | prior verified Korean, same key and exact Chinese value |
| `anal_beads_tail_cat_flavor` | prior verified Korean, exact Chinese value at anal_beads_tail_cat_flavor_alt |
| `anal_beads_tail_cat_desc` | prior verified Korean, same key and exact Chinese value |
| `anal_beads_tail_bunny_name` | prior verified Korean, same key and exact Chinese value |
| `anal_beads_tail_bunny_flavor` | prior verified Korean, exact Chinese value at anal_beads_tail_bunny_flavor_alt |
| `anal_beads_tail_bunny_desc` | prior verified Korean, same key and exact Chinese value |
| `anal_plug_fake_jewelry_name` | prior verified Korean, same key and exact Chinese value |
| `anal_plug_fake_jewelry_flavor` | prior verified Korean, exact Chinese value at anal_plug_fake_jewelry_flavor_alt |
| `anal_plug_fake_jewelry_desc` | prior verified Korean, same key and exact Chinese value |
| `Amethyst'Heart_name` | prior verified Korean, same key and exact Chinese value |
| `Amethyst'Heart_flavor` | prior verified Korean, same key and exact Chinese value |
| `Amethyst'Heart_desc` | prior verified Korean, same key and exact Chinese value |
| `Maid'sTail_name` | prior verified Korean, same key and exact Chinese value |
| `Maid'sTail_flavor` | prior verified Korean, exact Chinese value at Maid'sTail_flavor_alt |
| `Maid'sTail_desc` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_breasts_pussy_name` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_breasts_pussy_flavor` | prior verified Korean, exact Chinese value at artificial_vagina_breasts_pussy_flavor_alt |
| `artificial_vagina_breasts_pussy_desc` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_breasts_pussy_inverted_name` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_breasts_pussy_inverted_flavor` | prior verified Korean, exact Chinese value at artificial_vagina_breasts_pussy_inverted_flavor_alt |
| `artificial_vagina_breasts_pussy_inverted_desc` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_pink_name` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_pink_flavor` | prior verified Korean, exact Chinese value at artificial_vagina_pink_flavor_alt |
| `artificial_vagina_pink_desc` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_tenga_name` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_tenga_flavor` | prior verified Korean, exact Chinese value at artificial_vagina_tenga_flavor_alt |
| `artificial_vagina_tenga_desc` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_ass_name` | prior verified Korean, same key and exact Chinese value |
| `artificial_vagina_ass_flavor` | prior verified Korean, exact Chinese value at artificial_vagina_ass_flavor_alt |
| `artificial_vagina_ass_desc` | prior verified Korean, same key and exact Chinese value |
| `cock_ring_name` | prior verified Korean, same key and exact Chinese value |
| `cock_ring_flavor` | prior verified Korean, exact Chinese value at cock_ring_flavor_alt |
| `cock_ring_desc` | prior verified Korean, same key and exact Chinese value |
| `Abyss'sFlower_name` | prior verified Korean, same key and exact Chinese value |
| `Abyss'sFlower_flavor` | prior verified Korean, exact Chinese value at Abyss'sFlower_flavor_alt |
| `Abyss'sFlower_desc` | prior verified Korean, same key and exact Chinese value |
| `bullet_vibrator_name` | prior verified Korean, same key and exact Chinese value |
| `bullet_vibrator_flavor` | prior verified Korean, exact Chinese value at bullet_vibrator_flavor_alt |
| `bullet_vibrator_desc` | prior verified Korean, same key and exact Chinese value |
| `electric_dildo_name` | prior verified Korean, same key and exact Chinese value |
| `electric_dildo_flavor` | prior verified Korean, exact Chinese value at electric_dildo_flavor_alt |
| `electric_dildo_desc` | prior verified Korean, same key and exact Chinese value |
| `box_electric_dildo_name` | prior verified Korean, same key and exact Chinese value |
| `box_electric_dildo_flavor` | prior verified Korean, exact Chinese value at box_electric_dildo_flavor_alt |
| `box_electric_dildo_desc` | prior verified Korean, same key and exact Chinese value |
| `box_bullet_vibrator_name` | prior verified Korean, same key and exact Chinese value |
| `box_bullet_vibrator_flavor` | prior verified Korean, exact Chinese value at box_bullet_vibrator_flavor_alt |
| `box_bullet_vibrator_desc` | prior verified Korean, same key and exact Chinese value |
| `item_used_condom_loot_name` | prior verified Korean, same key and exact Chinese value |
| `event_clinic_dump_news` | prior verified Korean, same key and exact Chinese value |
| `event_clinic_dump_display` | prior verified Korean, same key and exact Chinese value |
| `event_clinic_dump_desc` | prior verified Korean, same key and exact Chinese value |
| `event_upper_purge_news` | prior verified Korean, same key and exact Chinese value |
| `event_upper_purge_display` | prior verified Korean, same key and exact Chinese value |
| `event_upper_purge_desc` | prior verified Korean, same key and exact Chinese value |
| `event_clinic_shortage_news` | prior verified Korean, same key and exact Chinese value |
| `event_clinic_shortage_display` | prior verified Korean, same key and exact Chinese value |
| `event_clinic_shortage_desc` | prior verified Korean, same key and exact Chinese value |
| `event_upper_buyout_news` | prior verified Korean, same key and exact Chinese value |
| `event_upper_buyout_display` | prior verified Korean, same key and exact Chinese value |
| `event_upper_buyout_desc` | prior verified Korean, same key and exact Chinese value |
| `cfg_dev_section` | new individual Korean translation |
| `type_adult_contraceptive` | prior verified Korean, same key and exact Chinese value |
| `type_adult_dildo` | prior verified Korean, same key and exact Chinese value |
| `type_adult_fantasy_dragon` | prior verified Korean, same key and exact Chinese value |
| `type_adult_plug` | prior verified Korean, same key and exact Chinese value |
| `type_adult_curio` | prior verified Korean, same key and exact Chinese value |
| `type_adult_sleeve` | prior verified Korean, same key and exact Chinese value |
| `type_adult_bdsm` | prior verified Korean, same key and exact Chinese value |
| `type_adult_electric` | prior verified Korean, same key and exact Chinese value |
| `sign_adult_goods_name` | new individual Korean translation |
| `sign_adult_goods_flavor` | new individual Korean translation |
| `sign_adult_goods_desc` | new individual Korean translation |
| `name_adult_goods_seller` | new individual Korean translation |
| `dialog_adult_seller_1` | new individual Korean translation |
| `dialog_adult_seller_2` | new individual Korean translation |
| `name_adult_goods_buyer` | new individual Korean translation |
| `dialog_adult_buyer_1` | new individual Korean translation |
| `dialog_adult_buyer_2` | new individual Korean translation |
| `cfg_debug_key_label` | new individual Korean translation |
| `cfg_debug_key_tip` | new individual Korean translation |
| `name_adult_goods_thief` | new individual Korean translation |
| `dialog_adult_thief_1` | new individual Korean translation |
| `dialog_adult_thief_2` | new individual Korean translation |
| `name_adult_goods_broker` | new individual Korean translation |
| `dialog_adult_broker_1` | new individual Korean translation |
| `dialog_adult_broker_2` | new individual Korean translation |
| `name_adult_goods_buyer_lower` | new individual Korean translation |
| `dialog_adult_buyer_lower_1` | new individual Korean translation |
| `dialog_adult_buyer_lower_2` | new individual Korean translation |
| `name_adult_goods_buyer_mid` | new individual Korean translation |
| `dialog_adult_buyer_mid_1` | new individual Korean translation |
| `dialog_adult_buyer_mid_2` | new individual Korean translation |
| `dragon_collection_case_name` | new individual Korean translation |
| `dragon_collection_case_flavor` | new individual Korean translation |
| `dragon_collection_case_desc` | new individual Korean translation |
| `dragon_display_case_name` | new individual Korean translation |
| `dragon_display_case_flavor` | new individual Korean translation |
| `dragon_display_case_desc` | new individual Korean translation |
| `cfg_max_adult_cases_label` | new individual Korean translation |
| `cfg_max_adult_cases_tip` | new individual Korean translation |
| `cfg_max_display_cases_label` | new individual Korean translation |
| `cfg_max_display_cases_tip` | new individual Korean translation |
| `cfg_loot_multiplier_label` | new individual Korean translation |
| `cfg_loot_multiplier_tip` | new individual Korean translation |
| `cfg_curio_loot_label` | new individual Korean translation |
| `cfg_curio_loot_tip` | new individual Korean translation |
