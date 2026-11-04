#!/usr/bin/env python3
import os,pathlib
root=pathlib.Path(os.environ["GAME_DECOMPILED"])
out=pathlib.Path("probably-stolen/analysis/tooltip-call-order.md")
terms=["FireOnCreateTooltipEarly","FireOnCreateTooltipLate","OnCreateTooltipLate","ModSourceHelper.CreateTooltip","new RichTextBuilder","Build()"]
rows=["# Tooltip call-order trace",""]
for p in root.rglob("*.cs"):
    try: lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
    except: continue
    hits=[i for i,l in enumerate(lines) if any(t in l for t in terms)]
    if not hits: continue
    relevant=False
    for i in hits:
        ctx="\n".join(lines[max(0,i-25):min(len(lines),i+35)])
        if "Tooltip" in ctx or "RichTextBuilder" in ctx:
            relevant=True
            break
    if not relevant: continue
    rows += [f"## {p.relative_to(root)}",""]
    seen=[]
    for i in hits:
        a=max(0,i-25); b=min(len(lines),i+35)
        if any(a < y < b for y in seen): continue
        seen.append(i)
        rows += ["~~~csharp"]+[f"{j+1:05d}: {lines[j]}" for j in range(a,b)]+["~~~",""]
out.parent.mkdir(parents=True,exist_ok=True)
out.write_text("\n".join(rows),encoding="utf-8")
print(out)
