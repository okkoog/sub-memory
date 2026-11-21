# Tentacle game data display-name localization (2026-11-22)

- lewd_meme v2.1.0: 15/15 item names translated; all source id/price/dimensions/icons/categories/tags preserved.
- Probably Adult Goods v1.1.3: 41/41 catalog names and 9/9 explanatory notes translated. Official ko.csv names reused by ID; original field headers, type tags, prices and codes unchanged.
- DailyWearWorkshop: 50/50 doll display names translated; IDs, prices, category, original exact image filenames, width, height unchanged.
- Source rows, category tokens, import keys and variable formats were verified against original game data without reordering.
- Display field headers are Korean where source DLL loading checks only first field `id`; operational categories, file and ID tokens retained in Chinese where required. Names displayed during gameplay depend on the mod's file loader.
