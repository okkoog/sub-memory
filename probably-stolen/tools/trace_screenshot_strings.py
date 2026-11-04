#!/usr/bin/env python3
import os, pathlib
ROOT=pathlib.Path(os.environ["DECOMPILED_ROOT"])
OUT=pathlib.Path("probably-stolen/analysis/screenshot-string-trace.md")
queries={
 "3832048841":["食腐乌鸦","破损版","游戏卡","集换式卡牌","跟在队伍后面的黑鸟"],
 "3831837821":["触手本体","调教值","泌乳等级","口交","触手精液","淫色深空","可能是触手"],
 "3829429358":["Sterile Gauze Pack","Double-click to treat wounds","Rare goods traded only at the night container auction","Night Auction","Industrial Calibrator","xiaowo_auction_calibrator","A portable instrument for precision machine calibration.","private static void ApplyLocalizedText","Drag onto an owned stuck module","Uses remaining:","Each successful release uses one charge","Consumed after the final use"],
 "3829085054":["收纳箱","一键收纳","全箱收纳","全箱上货","一键上货","拖动物品到收纳箱上"],
 "3829730904":["Uryu Ishida","Ichigo Kurosaki","Rukia Kuchiki"]
}
out=["# Screenshot string trace",""]
for wid,qs in queries.items():
    out+= [f"## {wid}",""]
    files=list((ROOT/wid).rglob("*.cs"))
    for q in qs:
        out.append(f"### {q}")
        found=0
        for p in files:
            lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
            for i,line in enumerate(lines):
                if q in line:
                    found+=1
                    a=max(0,i-(20 if q=="private static void ApplyLocalizedText" else 3)); b=min(len(lines),i+(35 if q=="private static void ApplyLocalizedText" else 4))
                    out.append(f"- \`{p.relative_to(ROOT/wid)}:{i+1}\`")
                    out.append("~~~csharp")
                    for j in range(a,b): out.append(f"{j+1:04d}: {lines[j]}")
                    out.append("~~~")
                    if found>=12: break
            if found>=12: break
        if not found: out.append("- not found in decompiled C#")
        out.append("")
OUT.parent.mkdir(parents=True,exist_ok=True)
OUT.write_text("\n".join(out),encoding="utf-8")
print(OUT)
