Probably Stolen Korean Pack v0.7.5.2-dev — Output Boundary Test

Baseline:
- v0.7.5 runtime architecture retained.
- Corrected supplemental CSV format retained.
- Night Auction and CorpPresence adapter branches are unchanged from v0.7.5.

New narrowly-scoped output bridges:
1. ItemStorage
   - Post-processes the actual GameItem + PixelWindow machinery UI after
     BoxWindow operations.
   - Traverses only typed Managed UI objects:
     PixelWindow / MachineryPanelRow / MachineryPanel /
     MachineryButton / MachineryLabel / GridPixelElement.
   - BoxLabel.Apply/Tick/OnItemRenamed post-process the known private
     BoxLabel Entries state; the dedicated floor-label text comes from
     K|pskb_box_label_short in the KoreanPack CSV.

2. MidnightPawn (tentacle)
   - Translates the returned machinery PixelWindow from TentacleBody.BuildWindow.
   - Translates the final GameItem content window after TentacleBody.Create.

3. GameCardPK
   - Translates card machinery content windows after CreateCard and
     AttachChessBoardWindow.
   - Translates only the second field (display type) of buyer customer tuples
     appended by ShowcaseBuyerSystem.AppendBuyerCustomer.
     clientId / item identifiers are never translated.

Not changed:
- No broad/global UI interception.
- No assembly-wide literal rewriting.
- No target mod files are modified.
- CorpPresence runtime adapter code is unchanged.
- Night Auction runtime adapter code is unchanged.

Build validation:
- GitHub Actions build succeeded.
- Probably Stolen ModScanner: HARD_COUNT=0 / SCANNER_EXIT=0.
