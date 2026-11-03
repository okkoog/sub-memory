#!/usr/bin/env python3
import os, pathlib, re
root=pathlib.Path(os.environ["CORP_DECOMPILED"])
out=pathlib.Path("probably-stolen/analysis/CorpPresence-0.9.4-localization-paths.md")
rows=["# CorpPresence 0.9.4 localization paths",""]
for p in root.rglob("*.cs"):
    try: lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
    except: continue
    hits=[]
    for i,line in enumerate(lines):
        if "L.T(" in line or "L.T<" in line or "GetCurrentLocaleCode" in line or "SetName(" in line or ".shortDescription" in line or ".longDescription" in line or ".flavorText" in line or "AddLine(" in line or ".SetText(" in line:
            hits.append(i)
    if not hits: continue
    rows += [f"## {p.relative_to(root)}",""]
    shown=set()
    for i in hits[:150]:
        a=max(0,i-2); b=min(len(lines),i+3)
        key=(a,b)
        if key in shown: continue
        shown.add(key)
        rows+=["~~~csharp"]+[f"{j+1:04d}: {lines[j]}" for j in range(a,b)]+["~~~",""]
out.parent.mkdir(parents=True,exist_ok=True)
out.write_text("\n".join(rows),encoding="utf-8")
print(out)
