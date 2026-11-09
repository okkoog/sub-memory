1.7.5 addition: Heavy farm mainframe protection is ON by default in the official mod settings, with no restart needed. The mainframe and its contents are excluded from night burglary and insurance valuation. Withdrawn items follow normal rules; trading prices are unchanged. Turn it off to restore native rules.

1.7.5: The integrated farm handbook now includes a flow illustration, fourteen illustrated quick guides and paged details. Chinese/English reading positions are preserved. Gameplay and existing saves are unchanged.

Agri Expansion 1.7.4: public furnace callback replaces internal reflection. No declared capabilities. Includes the 1.7.3 handbook rules on pages 3-5. Existing notes follow.

Agri Expansion 1.7.2: bounded handbook entries with footer clearance, perfume base prices capped at 1200, quality premiums retained. Saved bottles reprice without refilling charges.

Agri Expansion 1.7.1: ten-page perfumery handbook, six recipe pages, no more than five entries per page, larger text and spacing.

Agri Expansion 1.7.0: 25 functional perfumes, 17 new bottle sprites, a seven-page handbook and next-day audiences/supplier invitations. Morning Market and Quiet Accord now have replacement effects. Earlier combo-effect descriptions below are historical. See the 1.7.0 notes and current handbook.

Previous releases:

Agri Expansion 1.6.2: redraws four vegetable growth sprites at the native pixel scale, with broader silhouettes, clearer leaves and fruit, and one image per species.

Agri Expansion 1.6.1: renamed dish categories, remembered selector position, one growth image per added crop, with taller and more distinct plants for four vegetables, and species-specific seed propagation for 16 added crops while retaining native genetics.

Agri Expansion 1.6.0: Cooking, 24 boxed dishes, four edible crops, eight scent materials. Unlock Cooking for 600 from the Hydroponic branch. 9x12 scrolling intake, 9x6 output. Instant, no power. Food thieves and thieves sometimes sell unlearned recipe notes. Cooking and Perfumery both require only Hydroponics. Two new perfumes: Morning Market (trade + budget) and Quiet Accord (trade + faction reputation).

Agri Expansion 1.5.8

Animal-area menus and navigation list RAS systems first, cages second, retaining order within each type. Saved backend ordering and area settings remain intact. Includes all 1.5.7 fixes. 

Agri Expansion 1.5.7

Per-size feed permissions with opt-in defaults for new food; one-time migration retains existing stock choices. New plots start at 10% fill. Per-plot preferred water overrides and single-grade refill selection. Algae and Magnet Flowers absorb their target contaminants from water-free liquid mixtures. Updated Farm Handbook. 

Agri Expansion 1.5.6

Left-aligned, unwrapped processor aroma descriptions. Consistent Start/Clear controls with refining options below. Blender reaction text now spans the shared left area. Existing inventory save indexes and processing rules are retained. Offline checks passed; in-game verification remains with the player.

Agri Expansion 1.5.5

Standalone 2x3 Farm Owner's Handbook, supplied with the farm; existing owners receive one copy. Contents plus fourteen bilingual chapters in the previous owner's voice. Reading progress persists. Original machine note retained. Includes all pending 1.5.4 changes.

Agri Expansion 1.5.4

Fixed native positioning overloads, smaller machine grids and three distinct 2x2 ingredient bays. Repaired farm baseline capacities: 10L water, 100 energy, 100 goods; fill/charge normally. Husbandry shows permanent feeder/butcher slots for direct drops and native opening. Start blends food in the feeder. Compact aquaculture layout retains water tank and filters.

Agri Expansion 1.5.3

Compact natural-size machine windows; crop-driven processors with explicit Start. Refined paste uses 10mL base, refined essence 10mL acid. Saved per-item aroma profiles, Basic/Pure/Perfect prices, four illustrated handbook pages. Integrated Farm adds water/power accounting and global special-module settings with individual overrides. Permanent farm-offer refusal and delayed farmer-start delivery with clearing tools. Offline verification passed; gameplay testing remains with the player. 


1.5.2: Compact perfume blender with one ingredient row shown at a time; all unlocked rows still run together. Redrawn machines, forty ingredient packs and seven perfume bottles. Perfumes now occupy 2x3 cells; crowded legacy bottles retain their footprint until space is available. Both handbooks use dedicated navigation/close icons. The perfumery handbook is written as a manufacturer's customer guide. Farm child windows stay open when closing the dashboard, and their entry buttons toggle them independently.

1.5.1 startup fix: Defer perfumery service registration until the player exists. Perfume callbacks also avoid the lazy player getter during startup. Fixes the 1.5.0 NullReferenceException that prevented the mod from initializing. Exit the game fully before restarting with this update.

Perfumery: unlock after Plant DNA in Wild's Hydroponic branch for a base cost of 800. Receive three instant processors, a battery-powered blender and a handbook. Forty ingredients support eight notes and five sequential reactions. Each overnight or turbo batch costs 16 energy total; up to four chemical modules unlock five rows. Six functional perfumes have five uses each; the three complex perfumes are medium contraband. Unmatched mixtures produce sale-only custom perfume. The machine previews notes and reactions without revealing the resulting product. Only the simplest perfume has a tutorial.
Build, exhaustive reaction checks and installed-game API checks passed. Live UI, customer effects and turbo behavior remain for player testing. 

1.4.5 update: Redrawn Huge Golden Ear / Huge Dryland Thornvine, and fixed specialty-module insertion into locked Integrated Farm plots. Native effect priorities and per-plot switches remain. Offline checks passed; live harvest testing pending. 


1.4.4 handbook update: A distinct teal wheat-and-leaf cover (2 x 3 cells), the official paper reader with contents and paging, remembered reading position, and revised bilingual instructions for all twelve crops. Crowded legacy books keep their footprint until moved into sufficient space. One new promotional illustration has been added.

New in 1.4.3: Seven new crops bring the total to twelve: Magnet Flower, Acid Pod, Alkali Succulent, Golden Ear, Dryland Thornvine, Wine Gourd and Polymer Pod. Five new genetic traits support heavy-metal removal, acid/base extraction, brewing and polymer production. Crop sizes vary by species.
Used Magnet Flowers become separate Saturated Magnet Flower items. Combine one with Large/Huge Ironfruit for an ingot. Cross-state stacking and shared-state contamination are fixed. Ironfruit size now affects ingot purity.
Blend the chemical crops or Wine Gourd for acid/base/alcohol mixtures. Wine Gourds can also be sold or consumed as alcohol. The Fiber Processor now makes vanilla printing material from polymer-bearing produce and retains leftover polymer credit.
The Crop Handbook explains all added crops. The Doctor, crop traders and Eleanor sell it; owned books suppress further restocking.

Farm improvements: Per-plot water/optimization switches, 1–100% refill targets, fixed 20L culture tanks, and mineral adjustment up to 10%. Displaced liquid returns to the reservoir; insufficient tablets are not consumed. Compact menus, saved subwindow positions, corrected crop icons and unpaid-customer-item protection are included.

Existing content includes five original functional crops, instant Food Packing, battery-powered Fiber Processing, plant-based reducing agent at 120°C, and an expandable 24-plot Integrated Farm with seed/crop storage, water, power and livestock management.
The Real Hydroponic Farmer start has a dedicated Eleanor story with weekly orders, partial deliveries, farm repairs, family aid and ongoing trade. Other starts retain vanilla Eleanor. Tier-V supertech crops are not included yet.

Subscribe, enable in the official Mod manager and restart. Remove same-ID local copies when switching to Workshop. English follows the game language; reload after switching. No prerequisites or new permissions. Ironfruit smelting uses the public per-machine cycle callback; no reflection-external permission is needed.
