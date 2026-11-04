# Probably Stolen Korean Pack v0.7.3-dev

## Purpose
v0.7.2 safe-whitelist build stopped the Unity Localization handle crash, but did not change the untranslated tooltips shown in-game.

v0.7.3 moves item-tooltip translation to the actual final tooltip construction point.

## Root cause fixed
GameItemElement.GetTooltipBasic() calls ModInternal.CreateTooltipLate() before:
- shortDescription
- flavorText
- container/scratcher/lottery tooltip additions

GameItemElement.GetTooltip()/GetTooltipAdvanced() then call:
- tooltipRichTextFunc
- ModSourceHelper.CreateTooltip()
- RichTextBuilder.Build()

Therefore ModHook.OnCreateTooltipLate is not the real final stage.

v0.7.3 patches only ModSourceHelper.CreateTooltip() postfix and translates the completed RichTextBuilder after the mod source line has been appended, while keeping the audited safe-whitelist target patches from v0.7.2.

## Additional screenshot-confirmed mappings
- GameCardPK: 蛇王, 错版卡, 生命值, 攻击力, 速度, 技能槽上限 and confirmed card description strings.
- MidnightPawn: 经验, 垃圾场捕获少女提示, 本体说明, 调教值/泌乳等级/口交/性交/肛交/排卵期/已怀孕.
- Existing Night Auction / ItemStorage / BrewingExpansion / CorpPresence supplements remain included.

## Static validation
Build: success
Game ModScanner:
- CAPABILITIES=reflection-external
- HARD_COUNT=0
- no files capability
- no loader-target hard flag

## GitHub Actions artifact
Workflow run:
https://github.com/okkoog/sub-memory/actions/runs/41753457750

Artifact:
ProbablyStolen_KoreanPack_v0.7.3-dev_FinalTooltip_Overlay

Artifact id: 13286713863
Artifact digest:
sha256:fdf16a8b39a4abde0b2ca8baf6d7577245785d0357793dd94abe631ae495a216

This artifact is an overlay and must currently be applied over the full v0.7.0 installation package.

## Full-install status
The complete v0.7.0 installation ZIP (16 full catalogs) exists in the ChatGPT file library, but those 16 catalogs were never committed to GitHub.
The current ChatGPT local execution container returns ClientError, so the binary base ZIP cannot yet be imported into GitHub from the container.

Do not rebuild a "full install" from only the six supplement CSVs; that would drop the existing full catalogs.
