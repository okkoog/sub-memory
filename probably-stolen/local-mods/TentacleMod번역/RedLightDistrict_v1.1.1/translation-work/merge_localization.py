#!/usr/bin/env python3
"""Merge reviewed RLD translations into external ko.csv; never modify the original DLL."""
import csv, json, re, collections, zipfile
from pathlib import Path
HERE=Path(__file__).resolve().parent
BASE=HERE.parents[2]
SOURCE=BASE/'TentacleMod/RedLightDistrict_v1.1.1/Localization/cn.csv'
DEST=HERE.parent/'Localization/ko.csv'
REVIEW=HERE.parent.parent/'번역대조표'
AUDIT=HERE.parent/'LOCALIZATION_PROGRESS.md'
ZIP=HERE.parent.parent/'ProbablyStolen_RedLightDistrict_v1.1.1_Localization_KO_20261121.zip'
UI_RE=re.compile(r'::(?:Build.*(?:Tooltip|Description|Dialogue|Panel)|ShowPanel|Append.*Line|Open.*Window|OpenPanel|Update.*Text|GetTooltip|Get.*Description|Make.*Tooltip|Refresh.*Label|Make.*Text|ShowTooltip|ShowDialog)',re.I)
COMPARE_RE=re.compile(r'::(?:op_Equality|op_Inequality|IndexOf|Contains|HasTrait|GetTag|IsPatronDisplayName|IsTag|TryGetValue)',re.I)
with SOURCE.open(encoding='utf-8-sig',newline='') as f:
    rows=list(csv.DictReader(f))
assert len(rows)==3619,len(rows)
assert len({r['key'] for r in rows})==3619
by_key={r['key']:r for r in rows}
by_source=collections.defaultdict(list)
for r in rows: by_source[r['original']].append(r)
inventory={}
with (REVIEW/'RedLightDistrict_v1.1.1_active_CJK_strings_20261121.tsv').open(encoding='utf-8-sig',newline='') as f:
    for line in list(f)[1:]:
        if not line.strip():continue
        x=line.rstrip('\r\n').split('\t')
        assert len(x)==4
        idx=int(x[0])
        assert idx not in inventory
        inventory[idx]={'source':json.loads(x[1]),'sites':x[3].split('; ')}
assert len(inventory)==3133
reviewed={};id_count=0;matched_sites=0;skipped_compares=0;skipped_nonui=0
for batch in (1,2,3):
    file=REVIEW/f'RedLightDistrict_v1.1.1_UI_reviewed_batch{batch:02d}_20261121.tsv'
    with file.open(encoding='utf-8-sig',newline='') as f: data=[x.rstrip('\r\n').split('\t') for x in list(f)[1:] if x.strip()]
    for x in data:
        assert len(x)==2
        idx=int(x[0]);val=json.loads(x[1])
        assert idx in inventory and isinstance(val,str) and val.strip()
        id_count+=1
        inv=inventory[idx];accepted=0
        for site in inv['sites']:
            p=site.rfind('@'); assert p>=0
            method,offset=site[:p],int(site[p+1:],16)
            options=[r for r in by_source[inv['source']] if method+'(' in r['method'] and int(r['IL_offset'])==offset]
            assert len(options)==1,(idx,site,len(options))
            orig=options[0]
            if batch<=2 and not UI_RE.search(site):
                skipped_nonui+=1;continue
            if COMPARE_RE.search(orig['next_operand']) or method.endswith('::IsPatronDisplayName'):
                skipped_compares+=1;continue
            key=orig['key']
            if key in reviewed:assert reviewed[key]==val,(key,idx)
            reviewed[key]=val;matched_sites+=1;accepted+=1
        # Some reviewed phrases are used only in internal comparisons; omit those keys.
assert id_count==295,id_count
direct={};manual_counts=[]
for file in sorted(HERE.glob('manual_direct_batch*_20261121.tsv')):
    count=0
    with file.open(encoding='utf-8-sig',newline='') as f:
        for line in list(f)[1:]:
            if not line.strip():continue
            r=line.rstrip('\r\n').split('\t')
            assert len(r)==2
            key=r[0];value=json.loads(r[1])
            assert key in by_key and key not in direct and isinstance(value,str) and value.strip(),key
            direct[key]=value;count+=1
    manual_counts.append((file.name,count))
assert len(manual_counts)>=3
existing={}
with DEST.open(encoding='utf-8-sig',newline='') as f:
    for r in csv.reader(f):
        if not r:continue
        assert len(r)==2,r
        key,value=r
        assert key in by_key and key not in existing and value
        existing[key]=value
assert len(existing)>=22
combined=dict(reviewed);combined.update(direct);combined.update(existing)
assert not (set(combined)-set(by_key))
def variables(s):return sorted(re.findall(r'\{[^{}]+\}',s))
for k,v in combined.items():
    original=by_key[k]['original']
    assert variables(original)==variables(v),('variable',k)
    assert original.count('\n')==v.count('\n'),('newlines',k)
    assert '\t' not in v and original!=v,('invalid',k)
    for token in ('<color=','</color>','<b>','</b>'):
        assert original.count(token)==v.count(token),('markup',k)
with DEST.open('w',encoding='utf-8',newline='') as f:
    for row in rows:
        k=row['key']
        if k in combined:f.write(k+',"'+combined[k].replace('"','""')+'"\r\n')
with DEST.open(encoding='utf-8',newline='') as f:
    back={r[0]:r[1] for r in csv.reader(f) if r}
assert back==combined
assert all(back.get(k)==v for k,v in existing.items())
coverage=len(combined)
AUDIT.write_text('# Red Light District v1.1.1 — external Korean Localization progress\n\n'
    +'- Source catalog: original TentacleMod/RedLightDistrict_v1.1.1/Localization/cn.csv (untouched)\n'
    +f'- Total active IL occurrence keys: {len(rows)} (includes diagnostics, internal strings and logs)\n'
    +f'- Korean external ko.csv translations: {coverage}; untranslated/fallback occurrences: {len(rows)-coverage}\n'
    +f'- Reviewed historical source strings: {id_count}; mapped and selected IL sites: {matched_sites}; unique selected historical keys: {len(reviewed)}\n'
    +f'- New manually reviewed direct keys: {len(direct)}; existing trusted ko.csv entries preserved: {len(existing)}\n'
    +f'- Comparison/identifier sites withheld: {skipped_compares}; reviewed strings at non-UI sites withheld: {skipped_nonui}\n'
    +'- Untranslated keys retain original Chinese through the mod fallback. Key format, variables, line breaks, CSV quoting, existing translations validated.\n'
    +'- No DLL files edited; ZIP contains only ko.csv and progress report.\n'
    +'- Actual game runtime / save data compatibility and CSV parser support for multi-line text NOT TESTED.\n\n'
    +'## Manually reviewed new key batches\n\n'
    +''.join(f'- {name}: {count} entries\n' for name,count in manual_counts),encoding='utf-8')
ZIP.parent.mkdir(parents=True,exist_ok=True)
with zipfile.ZipFile(ZIP,'w',zipfile.ZIP_DEFLATED,compresslevel=9) as z:
    z.write(DEST,arcname='RedLightDistrict_v1.1.1/Localization/ko.csv')
    z.write(AUDIT,arcname='LOCALIZATION_PROGRESS.md')
with zipfile.ZipFile(ZIP) as z: assert z.testzip() is None and len(z.namelist())==2
print('source',len(rows),'prior-reviewed',id_count,'historical-sites',matched_sites,'manual',len(direct),
      'existing',len(existing),'final',coverage,'remaining',len(rows)-coverage,'zip-bytes',ZIP.stat().st_size)
