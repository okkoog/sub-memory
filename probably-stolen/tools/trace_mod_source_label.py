#!/usr/bin/env python3
import os, pathlib, re
root=pathlib.Path(os.environ["GAME_DECOMPILED"])
out=pathlib.Path("probably-stolen/analysis/mod-source-label-trace.md")
patterns=("Manifest.Name","manifest.Name",".Manifest.ID","LoadedMod","ModLoader.Mods")
rows=["# Mod source-label trace",""]
for p in root.rglob("*.cs"):
    try: lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
    except: continue
    for i,line in enumerate(lines):
        if any(q in line for q in patterns):
            ctx="\n".join(f"{j+1:04d}: {lines[j]}" for j in range(max(0,i-5),min(len(lines),i+7)))
            if any(k in ctx for k in ("RichTextBuilder","AddLine","tooltip","Tooltip","source","Source","modName","ModName","display")):
                rows += [f"## {p.relative_to(root)}:{i+1}","~~~csharp",ctx,"~~~",""]
out.parent.mkdir(parents=True,exist_ok=True)
out.write_text("\n".join(rows),encoding="utf-8")
print(out)
