#!/usr/bin/env python3
"""Create machine-draft Korean localization CSVs without touching existing game DLLs.

These files require editorial review.  Keeps original source keys, all {placeholders},
and literal \n separators; records unresolved lines in JSON.
"""
import os, re, csv, json, sys, pathlib, itertools, time, collections
import torch
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM

ID = sys.argv[1]
MODIDS = {
    '3835292703': 'npc_girls',
    '3830031787': 'fishing-cuisine-expansion',
    '3834071305': 'cyber_fried_rice',
}
assert ID in MODIDS
src = pathlib.Path(f"probably-stolen/mods/{ID}/Localization/en.csv")
outdir = pathlib.Path("probably-stolen/work-in-progress/translation-20261110/machine-drafts")
outdir.mkdir(parents=True, exist_ok=True)
outfile = outdir / f"pskb__{ID}.csv"
reportpath = outdir / f"qa__{ID}.json"

with src.open(encoding='utf-8-sig',newline='') as f:
    allrows=list(csv.reader(f))
rows=[]
for row in allrows:
    if not row:continue
    key=row[0].strip()
    if key.startswith('#') or key=='key':continue
    if len(row)!=2: raise ValueError(f"Malformed CSV for {ID}: {row[:2]}")
    if not key:continue
    rows.append((key,row[1]))
assert len(rows)==len(set(k for k,v in rows)), 'duplicate source keys'

# Match ordinary game placeholders.  Do not feed the \n delimiter to the NMT.
placeholders=re.compile(r"\{[^{}]+\}")
emoji_punct=re.compile(r"^[\s\W\d]+$",re.UNICODE)
def skip(txt):
    return not txt.strip() or not re.search(r'[A-Za-z]',txt)
def limit_chunks(text, max_char=500):
    if len(text)<=max_char: return [text]
    sentences=re.split(r'(?<=[.!?;。！？])\s+',text)
    if len(sentences)==1:
        return [text[i:i+max_char] for i in range(0,len(text),max_char)]
    out=[];s=''
    for p in sentences:
        if len(s)+len(p)+1>max_char and s:
            out.append(s);s=''
        s+=((' ' if s else '')+p)
    if s:out.append(s)
    return list(itertools.chain.from_iterable(
        [chunk] if len(chunk)<=max_char else [chunk[i:i+max_char] for i in range(0,len(chunk),max_char)]
        for chunk in out))
def token_segments(value):
    pieces=value.split('\\n')
    result=[]
    for p in pieces:
        result.append(limit_chunks(p))
    return result

split_by_key={k:token_segments(v) for k,v in rows}
unique=list(dict.fromkeys(t for groups in split_by_key.values() for sub in groups for t in sub if not skip(t)))
print(f"INPUT {ID} sourcekeys={len(rows)} unique text fragments={len(unique)}",flush=True)

modelid='facebook/nllb-200-distilled-600M'
torch.set_num_threads(3)
started=time.time()
tokenizer=AutoTokenizer.from_pretrained(modelid, src_lang='eng_Latn')
model=AutoModelForSeq2SeqLM.from_pretrained(modelid)
model.eval()
koid=tokenizer.convert_tokens_to_ids('kor_Hang')
print("MODEL READY seconds=",int(time.time()-started),flush=True)

# The model sees placeholders in situ; we verify all afterward. If it loses one,
# a safe fallback keeps the original text and flags the key for manual translation.
cache={}
bad=[]
batch_size=8
for offset in range(0,len(unique),batch_size):
    texts=unique[offset:offset+batch_size]
    tok=tokenizer(texts,return_tensors='pt',padding=True,truncation=True,max_length=300)
    with torch.inference_mode():
        outputs=model.generate(**tok,forced_bos_token_id=koid,num_beams=2,max_new_tokens=300)
    target=tokenizer.batch_decode(outputs,skip_special_tokens=True)
    for a,b in zip(texts,target):
        expected=collections.Counter(placeholders.findall(a))
        actual=collections.Counter(placeholders.findall(b))
        if expected != actual or not b.strip():
            bad.append({'source':a[:400], 'draft':b[:400], 'expected':dict(expected), 'actual':dict(actual)})
            cache[a]=a # no silently broken placeholders
        else:
            cache[a]=b.strip()
    if offset%128==0 or offset+batch_size>=len(unique):
        print(f"PROGRESS {ID} {min(offset+batch_size,len(unique))}/{len(unique)} "
              f"seconds={int(time.time()-started)} placeholderfails={len(bad)}",flush=True)

translations={}
for key,val in rows:
    groups=split_by_key[key]
    translation='\\n'.join(' '.join(cache.get(t,t) for t in sub).strip() for sub in groups)
    translations[key]=translation
    if len(re.findall(r'\\n',translation))!=len(re.findall(r'\\n',val)):
        raise ValueError('newline mismatch '+key)
    if collections.Counter(placeholders.findall(translation))!=collections.Counter(placeholders.findall(val)):
        raise ValueError('placeholder mismatch '+key)
# CSV is written as our KoreanPack bridge format, not standalone mod CSV.
with outfile.open('w',encoding='utf-8-sig',newline='') as f:
    f.write(f'M|{MODIDS[ID]},1\nW|{ID},1\n')
    for key,_ in rows:
        sio=__import__('io').StringIO()
        csv.writer(sio,lineterminator='').writerow([key,translations[key]])
        f.write('K|'+sio.getvalue()+'\n')

# Include an explicit audit of provisional/machine translation:
report={
  'mod_id':ID,'source_count':len(rows),'translated_count':len(translations),
  'unique_fragments':len(unique),'failed_fragments':len(bad),
  'failed_examples':bad[:300],
  'untranslated_keys':[key for key,val in rows if translations[key]==val and re.search(r'[A-Za-z]',val)],
  'seconds':int(time.time()-started),
  'samples': [{'key':k,'source':v,'ko_draft':translations[k]} for k,v in rows[:40]]
}
reportpath.write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(f"COMPLETE|{ID}|keys={len(rows)}|failed_fragments={len(bad)}|"
      f"untranslated_keys={len(report['untranslated_keys'])}|"
      f"elapsed={report['seconds']}|file={outfile}",flush=True)
