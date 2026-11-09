# Faction Reputation Expansion v1.5.3

## Update notes

Save storage is being reworked. This build migrates legacy saves: load each of your old saves once so the mod can convert its data before the final release.
Fixed the Newborn Alliance requisition phone freezing and missing cooldown display.

## Installation

Enable the mod in the main menu's Mods screen, then restart. The standard edition uses the official save-data API and requests no extra capabilities.

For local installation, close the game and copy the entire `FactionReputationExpansion` folder into the game's `Mods` folder. The manifest belongs at `Mods/FactionReputationExpansion/manifest.xml`, beside the DLL. The full game does not require MelonLoader. Avoid duplicate copies or old DLLs; a local copy with the same ID takes priority over Workshop content.

## Updating and transferring saves

Back up your vanilla saves and the old `FactionReputationExpansion` folder beside them. Close the game before replacing release files and retain the saves. Back up custom language files too; see `LOCALIZATION.md` for translation instructions.

For existing saves, first use the legacy migration edition and approve its file capability. On the computer with the correct progress, load and save each slot you wish to keep. Exit and replace it with the standard capability-free edition. Both editions have the same mod ID; install only one. Unconverted saves show a migration notice instead of silently resetting progress. New games can use the standard edition directly.

After conversion, reputation, commissions, permanent core effects and custom items travel inside the selected vanilla save through official ModData. The standard edition no longer reads or writes the old side folder. Let Steam Cloud finish syncing before switching computers, or manually copy the native saves. Loading history restores the progress at that checkpoint, without newer local side files overriding it. Copying mod files alone does not transfer progress. Retain your old side-file backup until conversion has been verified.

Default save location: `%USERPROFILE%/AppData/LocalLow/Questing Goose Studio/Probably Stolen`.

## Uninstallation

Save normally with the mod enabled, exit, disable it and restart. Local installations can remove `Mods/FactionReputationExpansion`; Workshop users can unsubscribe.

After a normal save and a full restart without the mod, added reputation stages, contacts, services and custom item occupancy disappear. Earned native items and money remain; core enhancements stop applying. Hidden ModData records can remain in the save and in cloud copies, but the vanilla game ignores them.

Reinstalling can recover hidden reputation, earned permanent effects and owned custom items. If the game was saved without the mod, unfinished commissions, appointments, unpaid settlements and pending effect selections are cancelled. Owned custom items return to the warehouse; if it is full, their hidden records remain and recovery retries on the next load. Damaged or newer data is protected instead of being replaced with empty progress.
