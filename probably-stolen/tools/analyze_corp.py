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
