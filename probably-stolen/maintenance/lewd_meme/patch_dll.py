#!/usr/bin/env python3
import argparse, csv, struct, hashlib
from pathlib import Path


def align(v,a): return (v+a-1)//a*a

class PE:
    def __init__(self, data: bytes):
        self.data = data
        self.e_lfanew = struct.unpack_from('<I', data, 0x3C)[0]
        self.coff = self.e_lfanew + 4
        self.nsec = struct.unpack_from('<H', data, self.coff+2)[0]
        self.opt_size = struct.unpack_from('<H', data, self.coff+16)[0]
        self.opt = self.coff + 20
        self.magic = struct.unpack_from('<H', data, self.opt)[0]
        self.sec_table = self.opt + self.opt_size
        self.section_alignment = struct.unpack_from('<I', data, self.opt+32)[0]
        self.file_alignment = struct.unpack_from('<I', data, self.opt+36)[0]
        self.sections=[]
        for i in range(self.nsec):
            o=self.sec_table+i*40
            name=data[o:o+8].rstrip(b'\0').decode('ascii','replace')
            vsize,vaddr,rawsize,rawptr=struct.unpack_from('<IIII',data,o+8)
            chars=struct.unpack_from('<I',data,o+36)[0]
            self.sections.append(dict(index=i,header=o,name=name,vsize=vsize,vaddr=vaddr,rawsize=rawsize,rawptr=rawptr,chars=chars))
        dd = self.opt + (96 if self.magic==0x10B else 112)
        self.cli_rva,self.cli_size=struct.unpack_from('<II',data,dd+14*8)
        self.cli_off=self.rva_to_off(self.cli_rva)
        self.md_rva,self.md_size=struct.unpack_from('<II',data,self.cli_off+8)
        self.md_off=self.rva_to_off(self.md_rva)
        self.streams=[]
        ver_len=struct.unpack_from('<I',data,self.md_off+12)[0]
        p=align(self.md_off+16+ver_len,4)
        flags,count=struct.unpack_from('<HH',data,p); p+=4
        for _ in range(count):
            hdr=p
            off,size=struct.unpack_from('<II',data,p); p+=8
            end=data.index(b'\0',p)
            name=data[p:end].decode('ascii')
            p=align(end+1,4)
            self.streams.append(dict(name=name,offset=off,size=size,header_file=hdr,header_local=hdr-self.md_off))
        self.stream_by_name={s['name']:s for s in self.streams}
    def rva_to_off(self,rva):
        for s in self.sections:
            if s['vaddr'] <= rva < s['vaddr'] + max(s['vsize'],s['rawsize']):
                return s['rawptr'] + (rva-s['vaddr'])
        raise ValueError(f'RVA 0x{rva:X} not mapped')


def read_cuint(buf,p):
    b0=buf[p]
    if b0<0x80:return b0,p+1
    if b0<0xC0:return ((b0&0x3F)<<8)|buf[p+1],p+2
    if b0<0xE0:return ((b0&0x1F)<<24)|(buf[p+1]<<16)|(buf[p+2]<<8)|buf[p+3],p+4
    raise ValueError('invalid compressed uint')

def write_cuint(n):
    if n < 0x80: return bytes([n])
    if n < 0x4000: return bytes([0x80|(n>>8),n&0xFF])
    if n < 0x20000000: return bytes([0xC0|(n>>24),(n>>16)&0xFF,(n>>8)&0xFF,n&0xFF])
    raise ValueError('compressed uint too large')

def special_flag(s):
    for ch in s:
        c=ord(ch)
        if c>=0x7F or 0x01<=c<=0x08 or 0x0E<=c<=0x1F or c in (0x27,0x2D):
            return 1
    return 0

def parse_us(data, pe):
    s=pe.stream_by_name['#US']
    off=pe.md_off+s['offset']
    heap=data[off:off+s['size']]
    p=1; entries=[]; byoff={}
    while p < len(heap):
        start=p
        n,q=read_cuint(heap,p)
        if n==0:
            p=q; continue
        raw=heap[q:q+n]
        if len(raw)!=n: raise ValueError('truncated #US')
        text=raw[:-1].decode('utf-16le','replace')
        entries.append((start,text,raw[-1]))
        byoff[start]=text
        p=q+n
    return heap,entries,byoff

def load_memory(path):
    mem={}
    with open(path,encoding='utf-8-sig',newline='') as f:
        for r in csv.DictReader(f):
            src=r['source']; ko=r['ko']
            if not src or not ko: continue
            if src in mem and mem[src]!=ko: raise ValueError(f'conflict for {src!r}')
            mem[src]=ko
    return mem

def parse_table_rows(data,pe):
    st=pe.stream_by_name['#~']
    base=pe.md_off+st['offset']
    p=base
    _reserved=struct.unpack_from('<I',data,p)[0]; p+=4
    major=data[p];minor=data[p+1];heap_sizes=data[p+2];p+=4
    valid=struct.unpack_from('<Q',data,p)[0];p+=8
    sortedmask=struct.unpack_from('<Q',data,p)[0];p+=8
    rows={}
    for tid in range(64):
        if (valid>>tid)&1:
            rows[tid]=struct.unpack_from('<I',data,p)[0];p+=4
    return p,rows,heap_sizes,valid

def idx_size(rows,tid): return 2 if rows.get(tid,0)<65536 else 4

def coded_size(rows,tables,tagbits):
    mx=max((rows.get(t,0) for t in tables),default=0)
    return 2 if mx < (1<<(16-tagbits)) else 4

def method_rvas(data,pe):
    p,rows,hs,valid=parse_table_rows(data,pe)
    strsz=4 if hs&1 else 2
    guidsz=4 if hs&2 else 2
    blobsz=4 if hs&4 else 2
    resscope=coded_size(rows,[0,26,35,1],2)
    typedeforref=coded_size(rows,[2,1,27],2)
    sizes={
      0:2+strsz+guidsz*3,
      1:resscope+strsz+strsz,
      2:4+strsz+strsz+typedeforref+idx_size(rows,4)+idx_size(rows,6),
      3:idx_size(rows,4),
      4:2+strsz+blobsz,
      5:idx_size(rows,6),
    }
    for tid in range(6):
        if (valid>>tid)&1:
            p += rows.get(tid,0)*sizes[tid]
    count=rows.get(6,0)
    rvas=[]
    row_size=4+2+2+strsz+blobsz+idx_size(rows,8)
    for i in range(count):
        rva=struct.unpack_from('<I',data,p+i*row_size)[0]
        if rva: rvas.append(rva)
    return rvas

def method_code_range(data,pe,rva):
    off=pe.rva_to_off(rva)
    b=data[off]
    kind=b&3
    if kind==2:
        size=b>>2
        return off+1,size
    if kind==3:
        flags_size=struct.unpack_from('<H',data,off)[0]
        hsize=((flags_size>>12)&0xF)*4
        size=struct.unpack_from('<I',data,off+4)[0]
        return off+hsize,size
    raise ValueError(f'unknown method header at RVA {rva:X}')

def patch(input_path,memory_path,output_path):
    src=Path(input_path).read_bytes(); pe=PE(src)
    mem=load_memory(memory_path)
    oldheap,entries,byoff=parse_us(src,pe)
    newheap=bytearray(b'\0'); offmap={}; translated_offsets=set(); used_translations=set()
    for oldoff,text,flag in entries:
        newoff=len(newheap); offmap[oldoff]=newoff
        out=mem.get(text,text)
        if out!=text:
            translated_offsets.add(oldoff); used_translations.add(text)
            fl=special_flag(out)
        else: fl=flag
        payload=out.encode('utf-16le')+bytes([fl])
        newheap += write_cuint(len(payload))+payload
    if len(newheap)>=0x1000000: raise ValueError('#US exceeds 24-bit ldstr token range')

    first=min(s['offset'] for s in pe.streams)
    header=bytearray(src[pe.md_off:pe.md_off+first])
    root=bytearray(header)
    stream_payloads={}
    for s in pe.streams:
        if s['name']=='#US': payload=bytes(newheap)
        else: payload=src[pe.md_off+s['offset']:pe.md_off+s['offset']+s['size']]
        stream_payloads[s['name']]=payload
    for s in pe.streams:
        while len(root)%4: root.append(0)
        noff=len(root); payload=stream_payloads[s['name']]
        local=s['header_local']
        struct.pack_into('<II',root,local,noff,len(payload))
        root.extend(payload)
    new_md=bytes(root)

    out=bytearray(src)
    valid=set(byoff)
    refs=0; translated_refs=0
    for rva in method_rvas(src,pe):
        try: code_off,code_size=method_code_range(src,pe,rva)
        except Exception: continue
        end=code_off+code_size
        i=code_off
        while i<=end-5:
            if src[i]==0x72:
                tok=struct.unpack_from('<I',src,i+1)[0]
                oldoff=tok&0xFFFFFF
                if (tok>>24)==0x70 and oldoff in valid:
                    refs+=1
                    newoff=offmap[oldoff]
                    struct.pack_into('<I',out,i+1,0x70000000|newoff)
                    if oldoff in translated_offsets: translated_refs+=1
                    i+=5; continue
            i+=1

    reloc=next((s for s in pe.sections if s['name']=='.reloc'),pe.sections[-1])
    md_file_off=reloc['rawptr']+reloc['rawsize']
    md_rva=reloc['vaddr']+reloc['rawsize']
    new_vsize=reloc['rawsize']+len(new_md)
    new_rawsize=align(new_vsize,pe.file_alignment)
    final_len=reloc['rawptr']+new_rawsize
    if len(out)<final_len: out.extend(b'\0'*(final_len-len(out)))
    out[md_file_off:md_file_off+len(new_md)]=new_md
    struct.pack_into('<II',out,pe.cli_off+8,md_rva,len(new_md))
    struct.pack_into('<I',out,reloc['header']+8,new_vsize)
    struct.pack_into('<I',out,reloc['header']+16,new_rawsize)
    size_image=align(reloc['vaddr']+new_vsize,pe.section_alignment)
    struct.pack_into('<I',out,pe.opt+56,size_image)

    Path(output_path).write_bytes(out)

    pe2=PE(bytes(out)); _,entries2,by2=parse_us(bytes(out),pe2)
    actual={s for _,s,_ in entries2}
    missing=[src for src in used_translations if mem[src] not in actual]
    if missing: raise ValueError(f'validation: missing translated strings {missing[:3]}')
    return {
      'input_sha256':hashlib.sha256(src).hexdigest(),
      'output_sha256':hashlib.sha256(out).hexdigest(),
      'input_size':len(src),'output_size':len(out),
      'us_entries':len(entries),'us_old_size':len(oldheap),'us_new_size':len(newheap),
      'memory_entries':len(mem),'matched_sources':len(used_translations),
      'ldstr_refs':refs,'translated_ldstr_refs':translated_refs,
      'new_metadata_rva':md_rva,'new_metadata_size':len(new_md),
    }

if __name__=='__main__':
    ap=argparse.ArgumentParser()
    ap.add_argument('input_dll');ap.add_argument('translation_memory_csv');ap.add_argument('output_dll')
    a=ap.parse_args()
    stats=patch(a.input_dll,a.translation_memory_csv,a.output_dll)
    for k,v in stats.items(): print(f'{k}={v}')
