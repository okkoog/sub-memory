#!/usr/bin/env python3
"""Merge reviewed RLD translations into external ko.csv; never modify the original DLL."""
import csv, json, re, collections
from pathlib import Path
HERE=Path(__file__).resolve().parent
BASE=HERE.parents[2]
SOURCE=BASE/'TentacleMod/RedLightDistrict_v1.1.1/Localization/cn.csv'
DEST=HERE.parent/'Localization/ko.csv'
REVIEW=HERE.parent.parent/'번역대조표'
AUDIT=HERE.parent/'LOCALIZATION_PROGRESS.md'
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
            assert key in by_key and isinstance(value,str) and value.strip(),key
            if key in direct:
                # Batch 19 revisits five entries; one is a reviewed wording correction.
                assert file.name=='manual_direct_batch19_20261121.tsv' and key in {
                    'rld_b2382142c6a2f89a869a','rld_68dd4bfc0aaeed151cfa',
                    'rld_27717340a49b3485f3fb','rld_f5a39c8e54dc3dde3931',
                    'rld_73096f577f0739625422'},key
            direct[key]=value;count+=1
    manual_counts.append((file.name,count))
assert len(manual_counts)>=3
# The game uses File.ReadAllLines and expands literal backslash-n in ParseCsvValue.
def decoded(value): return value.replace('\\n','\n')
def encoded(value): return value.replace('\n','\\n')
existing={}
with DEST.open(encoding='utf-8-sig',newline='') as f:
    for r in csv.reader(f):
        if not r:continue
        assert len(r)==2,r
        key,value=r
        assert key in by_key and key not in existing and value
        existing[key]=decoded(value)
assert len(existing)>=22
# Keep the 22 original user-approved entries as an immutable baseline; newly
# reviewed explicit key changes must supersede previous generated values.
seed={}
with (HERE/'Localization/ko.csv').open(encoding='utf-8-sig',newline='') as f:
    for x in csv.reader(f):
        if not x:continue
        assert len(x)==2 and x[0] not in seed
        seed[x[0]]=x[1]
assert len(seed)==22 and set(seed)<=set(existing)
# Original DLL TypeName() emits item-name suffixes that ParseFilterFromName()
# later parses by *Chinese* IndexOf literals. Translating only one side breaks
# filtered boss target selection; preserve both sides until DLL locale-aware.
UNSAFE_NAMING_KEYS={
    'rld_3d805d46c1216c3f3652', # 颜值
    'rld_259998749215c94824b1', # 身材
    'rld_6bff630ce07b7f55aafe', # 气质
    'rld_7882d7294814aa654018', # 处
    'rld_ce6797512aa447acc035', # 萝莉
    'rld_cc6325a044c8dc3fd83e', # 特性
    'rld_95d4e06bc98f69224b3d', # 颜值(小)
    'rld_fb980d761ea335c5e68e', # 身材(小)
    'rld_f75092afdabf8b70989a', # 气质(小)
    'rld_6a5bcff7c36497aa514f', # 少女
}
# These literals also flow into resource, saved-name, trait or parser lookups.
with (HERE / 'internal_string_reference.tsv').open(encoding='utf-8', newline='') as reference_file:
    protected = {item['key']: item for item in csv.DictReader(reference_file, delimiter='\t')}
assert set(protected) <= set(by_key)
UNSAFE_NAMING_KEYS.update(protected)
# The six entries below were originally translated. A targeted dnlib audit
# demonstrated that four are operands in ContainerByName exact comparisons,
# and two are IndexOf markers used to find/replace headers in saved descriptions.
# Switching these lookup literals to Korean makes existing Chinese values fail
# the comparison (or append a duplicate header after loading older saves).
COMPARISON_PROTECTION = {
    'rld_10348d760c2b2e5465e2', # front cabinet
    'rld_0c3fce20a7384ba2be04', # main warehouse
    'rld_cf1a6f889f4bcc975ed3', # showcase
    'rld_03e37f8200e9d8f405ee', # back hold
    'rld_4270e752a74b229c8c22', # TalentFeeder AppendFeederLine IndexOf marker
    'rld_445d455a701f81452b29', # TalentWaterTank AppendTankLine IndexOf marker
}
assert COMPARISON_PROTECTION <= set(protected), ('missing protected compare keys', COMPARISON_PROTECTION - set(protected))
assert not (UNSAFE_NAMING_KEYS & set(seed))
combined=dict(existing);combined.update(reviewed);combined.update(direct);combined.update(seed)
for k in UNSAFE_NAMING_KEYS:combined.pop(k,None)
assert not (COMPARISON_PROTECTION & set(combined)), ('comparison lookup keys translated', COMPARISON_PROTECTION & set(combined))
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
        if k in combined:f.write(k+',"'+encoded(combined[k]).replace('"','""')+'"\r\n')
with DEST.open(encoding='utf-8',newline='') as f:
    back={r[0]:decoded(r[1]) for r in csv.reader(f) if r}
assert back==combined
assert len(DEST.read_bytes().split(b'\n'))-1==len(back)
assert set(existing)-UNSAFE_NAMING_KEYS<=set(back)
assert not (UNSAFE_NAMING_KEYS & set(back))
assert all(back.get(k)==v for k,v in seed.items())
coverage=len(combined)
AUDIT.write_text('# Red Light District v1.1.1 — external Korean Localization progress\n\n'
    +'- Source catalog: original TentacleMod/RedLightDistrict_v1.1.1/Localization/cn.csv (untouched)\n'
    +f'- Total active IL occurrence keys: {len(rows)} (includes diagnostics, internal strings and logs)\n'
    +f'- Korean external ko.csv translations: {coverage}; untranslated/fallback occurrences: {len(rows)-coverage}\n'
    +f'- Reviewed historical source strings: {id_count}; mapped and selected IL sites: {matched_sites}; unique selected historical keys: {len(reviewed)}\n'
    +f'- New manually reviewed direct keys: {len(direct)}; original user-approved seed entries preserved: {len(seed)}; previously generated keys retained as fallback: {len(existing)}\n'
    +f'- Comparison/identifier sites withheld: {skipped_compares}; reviewed strings at non-UI sites withheld: {skipped_nonui}\n'
    +'- Untranslated keys retain original Chinese through the mod fallback. Key format, variables, line breaks, CSV quoting, original seed translations validated.\n'
    +'- Original TalentIntel.TypeName exact-name parser: 10 interface strings intentionally left Chinese to preserve boss filtering (see IL audit).\n'
    +f'- Reviewed internal literals kept original at runtime: {len(protected)}. Korean references and usage are recorded in translation-work/internal_string_reference.tsv.\n'
    +'- Actual game runtime / save data compatibility NOT TESTED; line breaks serialized as literal backslash-n for the game parser.\n\n'
    +'## Manually reviewed new key batches\n\n'
    +''.join(f'- {name}: {count} entries\n' for name,count in manual_counts),encoding='utf-8')

# Complete original-key audit in cn.csv source order. Never hide unresolved keys.
KEY_AUDIT=HERE.parent/'KEY_COVERAGE_AUDIT_20261121.tsv'
REVIEW_QUEUE=HERE.parent/'UNTRANSLATED_REVIEW_QUEUE_20261121.tsv'
CJK=re.compile(r'[\u3400-\u9fff]')
DIRECT_COMPARE=re.compile(r'(?:op_Equality|op_Inequality|String::Contains|String::IndexOf|::HasTrait|::IsPatronDisplayName)',re.I)
def classify(row):
    k=row['key']; orig=row['original']; method=row['method']; next_op=row['next_operand']
    if k in combined:
        return 'translated','Korean key verified'
    if k in protected:
        return 'protected-internal', protected[k]['usage']
    if DIRECT_COMPARE.search(next_op):
        return 'protected-internal','Exact-match / contains / trait-lookup operand'
    if re.search(r'(?:\.cctor|ParseFilterFromName|::ContainerByName\(|::FindMaidData\()',method) and len(orig)<50:
        return 'protected-review','Potential internal enum, dictionary key or named-asset look-up'
    if orig.startswith(('[红灯区]','[BallDiag]','[RLD','[DEBUG')) or (
        next_op.find('::Info(')>=0 or next_op.find('::Warning(')>=0 or next_op.find('::Error(')>=0):
        return 'diagnostic-review','Log/diagnostics, not confirmed player-facing'
    if not CJK.search(orig) and re.fullmatch(r'[A-Za-z0-9_ ./+\-=:(){}\[\]@#%!?,;<>]+',orig):
        return 'technical-review','No CJK, token / asset or technical value'
    return 'untranslated-review','Must inspect call-site context; may be player-visible'
counts=collections.Counter()
with KEY_AUDIT.open('w',encoding='utf-8',newline='') as f, \
     REVIEW_QUEUE.open('w',encoding='utf-8',newline='') as fq:
    writer=csv.writer(f,delimiter='\t')
    queue=csv.writer(fq,delimiter='\t')
    header=('row','key','status','reason','original_json','korean_json','method','il_offset','next_operand')
    writer.writerow(header);queue.writerow(header)
    for index,row in enumerate(rows,1):
        status,reason=classify(row);counts[status]+=1
        record=(index,row['key'],status,reason,json.dumps(row['original'],ensure_ascii=False),
                json.dumps(combined.get(row['key'], protected.get(row['key'], {}).get('korean_reference', '')),ensure_ascii=False),
                row['method'],row['IL_offset'],row['next_operand'])
        writer.writerow(record)
        if status=='untranslated-review':queue.writerow(record)
assert sum(counts.values())==len(rows)
assert counts['translated']==coverage
AUDIT.write_text(AUDIT.read_text(encoding='utf-8')
    +'\n## Complete per-key audit in source order\n\n'
    +''.join('- '+status+': '+str(amount)+'\n' for status,amount in sorted(counts.items()))
    +'\nKEY_COVERAGE_AUDIT_20261121.tsv covers all '+str(len(rows))+' keys once. UNTRANSLATED_REVIEW_QUEUE_20261121.tsv holds unresolved manual-review cases. These triage classifications DO NOT prove that diagnostic or protected keys can never appear in the UI.\n',encoding='utf-8')

print('source',len(rows),'prior-reviewed',id_count,'historical-sites',matched_sites,'manual',len(direct),
      'existing',len(existing),'final',coverage,'remaining',len(rows)-coverage,'zip-disabled')
