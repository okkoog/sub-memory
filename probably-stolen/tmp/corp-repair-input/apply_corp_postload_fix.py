from pathlib import Path
import os, re, shutil, hashlib

root = Path(os.environ["RUNNER_TEMP"])
src = root / "src"
wn = src / "ProbablyStolenKoreanPack" / "WaterNameCompatibility.cs"
if not wn.exists():
    raise SystemExit(f"missing decompiled WaterNameCompatibility.cs: {wn}")

before = wn.read_text(encoding="utf-8")
s = before

# This is the Corp compatibility fix only:
# Never touch Unity Localization from WaterNameCompatibility.Enable().
# Attach localization callbacks lazily only after OnGameLoadedLate has made worldReady=true.

if "private bool localizationHooksReady;" not in s:
    anchor = "\tprivate bool assetsReady;\n"
    if anchor not in s:
        raise SystemExit("assetsReady field anchor not found")
    s = s.replace(anchor, anchor + "\n\tprivate bool localizationHooksReady;\n", 1)

enable_pat = re.compile(
    r"\tinternal void Enable\(\)\n\t\{.*?\n\t\}\n\n\tinternal void Disable\(\)",
    re.S,
)
m = enable_pat.search(s)
if not m:
    raise SystemExit("Enable/Disable boundary not found")
old_enable_block = m.group(0)
new_enable = """\tinternal void Enable()
\t{
\t\tif (!enabled)
\t\t{
\t\t\tenabled = true;
\t\t\tModHook.OnGameLoadedInit += Loading;
\t\t\tModHook.OnGameLoadedLate += Loaded;
\t\t}
\t}

\tinternal void Disable()"""
s = s[:m.start()] + new_enable + s[m.end():]

# Patch only the cleanup portion inside Disable; preserve any unrelated cleanup.
disable_start = s.index("\tinternal void Disable()")
next_method = s.find("\n\tprivate ", disable_start)
if next_method < 0:
    raise SystemExit("Disable method end not found")
disable = s[disable_start:next_method]
disable = disable.replace(
    "\t\tLocalizationSettings.SelectedLocaleChanged -= LocaleChanged;\n",
    "",
    1,
)
old_init_cleanup = """\t\tif (initialization.IsValid())
\t\t{
\t\t\tinitialization.Completed -= Initialized;
\t\t}
"""
if old_init_cleanup in disable:
    disable = disable.replace(old_init_cleanup, "", 1)
needle = "\t\tgeneration++;\n"
if needle not in disable:
    raise SystemExit("Disable generation anchor not found")
lazy_cleanup = """\t\tgeneration++;
\t\tif (localizationHooksReady)
\t\t{
\t\t\tLocalizationSettings.SelectedLocaleChanged -= LocaleChanged;
\t\t\tif (initialization.IsValid())
\t\t\t{
\t\t\t\tinitialization.Completed -= Initialized;
\t\t\t}
\t\t\tlocalizationHooksReady = false;
\t\t}
"""
disable = disable.replace(needle, lazy_cleanup, 1)
s = s[:disable_start] + disable + s[next_method:]

if "private void EnsureLocalizationHooks()" not in s:
    anchor2 = "\tprivate void Initialized(AsyncOperationHandle<LocalizationSettings> operation)"
    if anchor2 not in s:
        raise SystemExit("Initialized anchor not found")
    helper = """\tprivate void EnsureLocalizationHooks()
\t{
\t\tif (!enabled || !worldReady || localizationHooksReady)
\t\t{
\t\t\treturn;
\t\t}
\t\tlocalizationHooksReady = true;
\t\tLocalizationSettings.SelectedLocaleChanged += LocaleChanged;
\t\tinitialization = LocalizationSettings.InitializationOperation;
\t\tif (!initialization.IsDone)
\t\t{
\t\t\tinitialization.Completed += Initialized;
\t\t}
\t}

"""
    s = s.replace(anchor2, helper + anchor2, 1)

request_start = s.find("\tprivate void Request()")
if request_start < 0:
    raise SystemExit("Request method not found")
request_end = s.find("\n\tprivate ", request_start + 1)
if request_end < 0:
    raise SystemExit("Request method end not found")
req = s[request_start:request_end]
old_guard = """\t\tif (!enabled || !worldReady || !assetsReady || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
\t\t{
\t\t\treturn;
\t\t}"""
if old_guard not in req:
    raise SystemExit("Request pre-postload guard not found")
new_guard = """\t\tif (!enabled || !worldReady)
\t\t{
\t\t\treturn;
\t\t}
\t\tEnsureLocalizationHooks();
\t\tif (!assetsReady || !localizationHooksReady || !initialization.IsValid() || !initialization.IsDone || initialization.Status != AsyncOperationStatus.Succeeded)
\t\t{
\t\t\treturn;
\t\t}"""
req = req.replace(old_guard, new_guard, 1)
s = s[:request_start] + req + s[request_end:]

# Strong source-level invariants.
enable_start = s.index("\tinternal void Enable()")
enable_end = s.index("\n\tinternal void Disable()", enable_start)
enable_text = s[enable_start:enable_end]
if "LocalizationSettings" in enable_text:
    raise SystemExit("FAIL: Enable still touches LocalizationSettings")
if "EnsureLocalizationHooks();" not in req:
    raise SystemExit("FAIL: Request does not lazily attach localization hooks")
if "!enabled || !worldReady" not in req:
    raise SystemExit("FAIL: Request is not worldReady gated")

wn.write_text(s, encoding="utf-8")
print("patched Corp postload-only localization isolation")
