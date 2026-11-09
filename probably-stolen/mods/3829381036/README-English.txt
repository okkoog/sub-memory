Ember Protocol 2.1.10: Official bounded legacy import; no files capability. Existing story and progress retained.

Ember Protocol 2.1.9 - firearm audio and Den rewards

Workshop release build. Gameplay and listening tests remain with the player.

All firearm shots now replace the previous sound on a single source, avoiding layered tails during rapid pistol/revolver/shotgun shots. Automatic fire continues on one voice following actual rounds. Reload audio remains independent and uses native game clips. Weapon damage, firing cadence and ammunition costs are unchanged.

Deep Den enemy health: stage 1 = 100 HP, plus 50 HP per subsequent stage. No headshot weakness.
Clear rewards: 9-11 items at stage 1, plus 2 per subsequent stage (11-13 at stage 2; 17-19 at stage 5). Modules, materials and tools are randomly selected, with the existing 5% special-module roll per item.

Rewards first go to the carried backpack. Overflow is delivered to the player's shop desk inventory, never the customer's trade counter or a showcase. If the desk is full, undelivered rewards remain as saved receipts and retry automatically when room becomes available. Delivery is batched at up to 16 items per pass. Discarding ordinary unclaimed loot does not remove pending Den deliveries.

Three waves, three health points, between-wave bandages and one stage clear per night are retained. Six ammo supplies per 300 funds, half-magazine supply rules, manual/automatic reload and saved gun magazines are retained. Optional Hunt Test Tools remains 0.1.1 (F8 in the shop).

Quit the game before replacing Mods/EchoProtocol. Restart and confirm version 2.1.9. Test rapid shots and automatic bursts; stage 1/2 HP and reward count; full backpack overflow; full desk deferred delivery; save/main-menu/reload with undelivered rewards.

Verification: Release build with zero warnings/errors, 273 offline rule assertions, 25 fake-Unity audio boundary assertions, 590 bilingual entries and 516 source lookups. No live gameplay or audible verification is claimed.
