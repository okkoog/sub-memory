#!/usr/bin/env python3
import os,pathlib
root=pathlib.Path(os.environ["GAME_DECOMPILED"])
out=pathlib.Path("probably-stolen/analysis/mod-name-tag-tooltip-trace.md")
qs=["MOD_NAME_TAG","MOD_ID_TAG","GetItemModName","GetClientModName"]
rows=["# MOD_NAME_TAG tooltip trace",""]
for p in root.rglob("*.cs"):
    try: lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
    except: continue
    hits=[i for i,l in enumerate(lines) if any(q in l for q in qs)]
    if not hits: continue
    rows += [f"## {p.relative_to(root)}",""]
    for i in hits:
        rows += ["~~~csharp"]
        for j in range(max(0,i-10),min(len(lines),i+12)): rows.append(f"{j+1:04d}: {lines[j]}")
        rows += ["~~~",""]
out.parent.mkdir(parents=True,exist_ok=True)
out.write_text("\n".join(rows),encoding="utf-8")
