#!/usr/bin/env python3
import os, pathlib, re, base64

ROOT = pathlib.Path(os.environ["DECOMPILED_ROOT"])
OUT = pathlib.Path("probably-stolen/analysis/visible-string-audit.tsv")
TARGETS = {
    "3829085054": "ItemStorage",
    "3832048841": "GameCardPK",
}
# Files / code patterns that are strongly associated with player-visible text.
NAME_HINTS = (
    "UI/", "Panel", "Window", "Tooltip", "Gear", "Battle", "SkillText",
    "TagDisplay", "Club", "Reward", "Dialog", "CardManual", "CardFactory",
)
CALL_HINTS = (
    "NotifyUser(", "ShowBanner(", "PanelHost.Message", "Message =",
    "MachineryButton(", "MachineryLabel(", "SetName(", "SetText(",
    "RegisterHover(", "UiKit.", "Dialogue", "identifierName", ".name =",
    "shortDescription", "longDescription", "flavorText", "return \"",
)
LOG_HINTS = ("Core.Log", "Core.Debug", "Trace(", "Debug(", "Warning(", "Error(")

string_re = re.compile(r'"((?:\\.|[^"\\])*)"')

def unescape_csharp(s: str) -> str:
    # enough for localization literals; preserve unknown escapes literally
    try:
        return bytes(s, "utf-8").decode("unicode_escape")
    except Exception:
        return s.replace(r'\n', '\n').replace(r'\r','\r').replace(r'\t','\t').replace(r'\\"','"').replace(r'\\\\','\\')

rows = ["mod_id\tfile\tline\tcontext\tsource_b64"]
for mod_id, root_name in TARGETS.items():
    base = ROOT / mod_id
    for p in sorted(base.rglob("*.cs")):
        rel = p.relative_to(base).as_posix()
        lines = p.read_text(encoding="utf-8", errors="ignore").splitlines()
        file_hint = any(h.lower() in rel.lower() for h in NAME_HINTS)
        for i, line in enumerate(lines):
            stripped = line.strip()
            if not stripped or '"' not in stripped:
                continue
            strong = file_hint or any(h in line for h in CALL_HINTS)
            if not strong:
                continue
            # Exclude obvious diagnostic-only lines unless they also use a visible sink.
            if any(h in line for h in LOG_HINTS) and not any(h in line for h in ("NotifyUser(", "ShowBanner(", "PanelHost.Message", "SetText(", "RegisterHover(", "UiKit.")):
                continue
            for m in string_re.finditer(line):
                raw = m.group(1)
                text = unescape_csharp(raw)
                if not text or len(text) < 2:
                    continue
                # Skip technical identifiers / paths / pure formatting where obvious.
                if re.fullmatch(r"[A-Za-z0-9_.:/\\-]+", text) and (" " not in text) and not any(ord(ch)>127 for ch in text):
                    continue
                ctx = stripped[:500].replace("\t"," ").replace("\n"," ")
                enc = base64.b64encode(text.encode("utf-8")).decode("ascii")
                rows.append(f"{mod_id}\t{rel}\t{i+1}\t{ctx}\t{enc}")

OUT.parent.mkdir(parents=True, exist_ok=True)
OUT.write_text("\n".join(rows)+"\n", encoding="utf-8")
print(f"wrote {OUT} rows={len(rows)-1}")
