#!/usr/bin/env python3
"""Static-only audit of a decompiled CorpPresence assembly. Does NOT load/execute DLL."""
import hashlib
import json
import os
import re
from pathlib import Path
from datetime import datetime, timezone

BASE = Path(__file__).resolve().parents[1]
dll = BASE / "mods" / "3828913695" / "CorpPresence.dll"
srcdir = Path(os.environ.get("CORP_DECOMPILED", "/tmp/corp-decompiled"))
out = BASE / "analysis" / "CorpPresence-0.9.4-static-audit.md"
out.parent.mkdir(parents=True, exist_ok=True)

patterns = {
    "Harmony attributes and patch installation": r"Harmony(?:Patch|Prefix|Postfix|Transpiler|Finalizer|TargetMethod|TargetMethods|Prepare)|PatchAll\s*\(|AccessTools\.(?:Method|Constructor|PropertyGetter|PropertySetter)|\.Patch\s*\(",
    "Mechanic and water initialization": r"WaterFeatureHelper|ShowcaseHelper|GetLocalizedMechanic|mech_condition_pure_water|(?:\bMechanic\b)",
    "Localization": r"LocHelper|LocalizationSettings|LoadLocalization|GetLocalized|LocalizedStringDatabase|StringTable|SelectedLocale|WaitForCompletion",
    "Mod load lifecycle": r"ModHook|OnModAssetsLoaded|LoadModDll|LoadModAssets|OnEnable|OnDisable",
}
limit_each = 300
max_line_width = 250

def escape(x):
    return x.replace("&#", "&amp;#").replace("<", "&lt;").replace(">", "&gt;").replace("```", "` ` `")

files = sorted(srcdir.rglob("*.cs"))
lines = [
    "# CorpPresence 0.9.4 — static ILSpy audit",
    "",
    f"- DLL: `{dll.as_posix()}`",
    f"- Size: {dll.stat().st_size} bytes",
    f"- SHA256: `{hashlib.sha256(dll.read_bytes()).hexdigest()}`",
    f"- Decompiled C# files: {len(files)}",
    f"- Generated (UTC): {datetime.now(timezone.utc).isoformat()}",
    "- Scope: **CorpPresence only**. This is a static text audit; no mod/game code was executed.",
    "- IMPORTANT: The matching lines are search leads, NOT proof of run-time patch registration or conflict.",
]
counts = {}
for label, regex in patterns.items():
    pattern = re.compile(regex, re.IGNORECASE)
    matches = []
    for path in files:
        try:
            text = path.read_text(encoding="utf-8-sig")
        except (UnicodeError, OSError):
            continue
        for idx, line in enumerate(text.splitlines(), start=1):
            if pattern.search(line):
                matches.append((path.relative_to(srcdir).as_posix(), idx, line.strip()))
    counts[label] = len(matches)
    lines.extend(["", f"## {label}", "", f"Total matching lines: {len(matches)}", "", "```text"])
    for path, num, line in matches[:limit_each]:
        lines.append(escape(f"{path}:{num}: {line[:max_line_width]}"))
    if len(matches) > limit_each:
        lines.append(f"... {len(matches) - limit_each} additional matching lines omitted; inspect decompiled source in the Actions workspace if needed.")
    lines.append("```")
lines.extend(["", "## Pattern counts", "", "```json", json.dumps(counts, ensure_ascii=False, indent=2), "```", ""])
out.write_text("\n".join(lines), encoding="utf-8")
print(f"REPORT_PATH={out}")
print(f"REPORT_BYTES={out.stat().st_size}")
print(json.dumps(counts, ensure_ascii=False))

# Targeted initialization and patch-install callsite context. Only limited excerpts
# are saved; full third-party decompiled source is not committed.
focus = {
    "CorpPresence/Main.cs": [r"OnEnable", r"OnDisable", r"GameResources", r"Hooks\\.", r"Install", r"Try"],
    "CorpPresence/GameResources.cs": [r"Initialize", r"GetTableAsync", r"GetEntry", r"IsValid", r"WaitForCompletion", r"IsDone", r"Status", r"Mechanic"],
    "CorpPresence/Hooks.cs": [r"public static", r"void Install", r"Patch\\<", r"Patch\\(", r"GameResources", r"WaterFeatureHelper", r"ShowcaseHelper"],
    "CorpPresence/L.cs": ["GetCurrentLocaleCode", "GetLocalized", "LocHelper", "static ", "Choose", "return "],
    "CorpPresence/Runtime/ResourceBootstrap.cs": ["GameResources", "Poll", "Tick", "OnModItemDirectoryInit", "EnableResources", "Ready", "callback"],
    "CorpPresence/ItemRegistry.cs": ["Install", "Activate", "OnModItemDirectoryInit", "GameResources", "Ready"],
}
detail_lines = ["# CorpPresence — targeted callsite context", "", "Only excerpts for relevant initialization/Harmony methods; no code was executed."]
for rel, regexes in focus.items():
    p = srcdir / rel
    if not p.is_file():
        detail_lines.extend(["", f"## {rel}", "", "Not found in decompilation."])
        continue
    textlines = p.read_text(encoding="utf-8-sig").splitlines()
    hit = [i for i, line in enumerate(textlines) if any(exp.replace(chr(92), "").lower() in line.lower() for exp in regexes)]
    selected = sorted({j for i in hit for j in range(max(0, i - 4), min(len(textlines), i + 9))})
    detail_lines.extend(["", f"## {rel}", "", f"Matching lines: {len(hit)}; displayed context lines: {len(selected)}", "", "```text"])
    for j in selected[:390]:
        detail_lines.append(escape(f"{j + 1}: {textlines[j][:250]}"))
    if len(selected) > 390:
        detail_lines.append(f"... {len(selected) - 390} further context lines not shown")
    detail_lines.append("```")
extra = BASE / "analysis" / "CorpPresence-0.9.4-callsite-context.md"
extra.write_text("\n".join(detail_lines) + "\n", encoding="utf-8")
print(f"CONTEXT_PATH={extra}")
print(f"CONTEXT_BYTES={extra.stat().st_size}")

# Narrow source windows for control-flow reconstruction. This is not an executable test.
windows = {
    "CorpPresence/Main.cs": [(29, 92)],
    "CorpPresence/GameResources.cs": [(14, 85)],
    "CorpPresence/Runtime/ResourceBootstrap.cs": [(1, 85)],
    "CorpPresence/Runtime/ModRuntime.cs": [(1, 95)],
}
flow = ["# CorpPresence v0.9.4 — lifecycle exact line windows", "", "Reference excerpts from ILSpy (unexecuted)."]
for name, spans in windows.items():
    p = srcdir / name
    flow.extend(["", f"## {name}", ""])
    if not p.exists():
        flow.append("Not found.")
        continue
    lns = p.read_text(encoding="utf-8-sig").splitlines()
    flow.append("```text")
    for start, end in spans:
        for line_number in range(start, min(end, len(lns)) + 1):
            flow.append(escape(f"{line_number}: {lns[line_number - 1][:280]}"))
    flow.append("```")
flowpath = BASE / "analysis" / "CorpPresence-0.9.4-lifecycle-windows.md"
flowpath.write_text("\n".join(flow)+"\n", encoding="utf-8")
print(f"WINDOWS_PATH={flowpath}")
print(flowpath.read_text(encoding="utf-8"))
