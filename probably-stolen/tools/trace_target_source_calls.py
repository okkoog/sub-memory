#!/usr/bin/env python3
import os,pathlib
root=pathlib.Path(os.environ["DECOMPILED_ROOT"])
out=pathlib.Path("probably-stolen/analysis/target-mod-source-calls.md")
rows=["# Target mod source-label calls",""]
for wid in ["3832048841","3831837821","3829429358","3829085054","3829730904","3828913695"]:
    base=root/wid
    rows += [f"## {wid}",""]
    found=0
    for p in base.rglob("*.cs"):
        lines=p.read_text(encoding="utf-8",errors="ignore").splitlines()
        for i,line in enumerate(lines):
            if "SetSource(" in line or "sourceMod" in line or "GetModLabel(" in line:
                found+=1
                rows += [f"### {p.relative_to(base)}:{i+1}","~~~csharp"]
                for j in range(max(0,i-3),min(len(lines),i+4)): rows.append(f"{j+1:04d}: {lines[j]}")
                rows += ["~~~",""]
    if not found: rows += ["- no source-label call found",""]
out.parent.mkdir(parents=True,exist_ok=True)
out.write_text("\n".join(rows),encoding="utf-8")
