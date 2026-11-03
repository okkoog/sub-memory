#!/usr/bin/env python3
import os, pathlib, re
ROOT=pathlib.Path(os.environ["DECOMPILED_ROOT"])
OUT=pathlib.Path("probably-stolen/analysis/localization-focused-context.md")
TARGETS={
 "3832048841":[("GameCardPK/GameCardFactory.cs",["CreateCard(","CreateSkill(","shortDescription","displayName","cardName"]),
                ("GameCardPK/SkillText.cs",["ForItem(","ForBattle(","PointLabel("])],
 "3831837821":[("MidnightPawn/Game/MaidenTooltip.cs",["OnTooltip(","调教值","触手本体"]),
                ("MidnightPawn/Game/TentacleBody.cs",["ShowReport","AddBodyExp","触手本体"])],
 "3829429358":[("XIAOWONightAuction/AuctionText.cs",["Select(","Display(","English =","LogPhrases"]),
                ("XIAOWONightAuction/AuctionPhone.cs",["LocalizeCard(","BuildDialogue(","ContactName"])],
 "3829085054":[("ItemStorage/BoxWindow.cs",["一键收纳","全箱收纳","全箱上货","一键上货","UpdateTooltip(","RefreshButtons(","shortDescription"])],
 "3829730904":[("BrewingExpansion/Formal/Figurines.cs",["Uryu Ishida","Ichigo Kurosaki","Rukia Kuchiki"]),
                ("BrewingExpansion/Formal/ModEntry.cs",["Register(","Figurines"])]
}
def ranges(lines, needles, radius=14):
    spans=[]
    for i,line in enumerate(lines):
        if any(n in line for n in needles):
            a=max(0,i-radius); b=min(len(lines),i+radius+1)
            if spans and a<=spans[-1][1]+3: spans[-1]=(spans[-1][0],max(spans[-1][1],b))
            else: spans.append((a,b))
    return spans
out=["# Focused localization code contexts","", "Generated from ILSpy decompilation; excerpts are limited to localization-relevant callsites.",""]
for wid,files in TARGETS.items():
    out.append(f"## {wid}")
    out.append("")
    for rel,needles in files:
        p=ROOT/wid/rel
        if not p.exists(): continue
        lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
        spans=ranges(lines,needles)
        for a,b in spans[:12]:
            out.append(f"### {rel}:{a+1}-{b}")
            out.append("~~~csharp")
            for j in range(a,b):
                out.append(f"{j+1:04d}: {lines[j]}")
            out.append("~~~")
            out.append("")
OUT.parent.mkdir(parents=True,exist_ok=True)
OUT.write_text("\n".join(out),encoding="utf-8")
print("wrote",OUT)
