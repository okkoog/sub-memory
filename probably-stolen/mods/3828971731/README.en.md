# Faction Reputation Expansion v1.5.0

## Update notes

Reworked the augmented faction's effects and improved some text and UI.

## Installation

Subscribe on Steam Workshop, wait for the download, enable the mod in the main menu's Mods screen, approve its file, reflection and dynamic-code capabilities, then restart the game.

For local installation, close the game and copy the entire `FactionReputationExpansion` folder into the game's `Mods` folder. The manifest belongs at `Mods/FactionReputationExpansion/manifest.xml`, beside the DLL. The full game does not require MelonLoader. Avoid duplicate copies or old DLLs; a local copy with the same ID takes priority over Workshop content.

## Updating and transferring saves

Back up your vanilla saves and the `FactionReputationExpansion` folder beside them. Close the game before replacing release files, or let Steam download the Workshop update. Keep your saves and the side folder. Back up and retain custom language files too; see `LOCALIZATION.md` for translation instructions.

Extended progress is stored both in a mod payload inside the vanilla save and in the side folder. Before moving computers, update the mod on both machines, load and save normally on the computer with the correct progress, then exit and let Steam Cloud finish syncing before loading on the other machine. Older saves not yet saved with the new format require both the vanilla saves and the side folder. Copying mod files alone does not transfer progress.

Default save location: `%USERPROFILE%/AppData/LocalLow/Questing Goose Studio/Probably Stolen`.

## Uninstallation

Save normally with the mod enabled, exit, disable it and restart. Local installations can remove `Mods/FactionReputationExpansion`; Workshop users can unsubscribe.

Vanilla reputation is saved within its native limits. Extended effects, contacts, commissions and pending deliveries stop working after removal. Legitimately obtained vanilla items and money remain. The vanilla game ignores the embedded mod payload; removing the side folder alone does not erase extended records embedded in saves.
