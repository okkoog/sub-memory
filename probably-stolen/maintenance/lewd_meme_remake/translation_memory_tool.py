#!/usr/bin/env python3
import argparse, csv, re, struct
from pathlib import Path

def rva_to_off(data, rva):
    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    coff = e_lfanew + 4
    nsec = struct.unpack_from("<H", data, coff + 2)[0]
    opt_size = struct.unpack_from("<H", data, coff + 16)[0]
    opt = coff + 20
    sec = opt + opt_size
    for i in range(nsec):
        o = sec + i * 40
        vsize, vaddr, rawsize, rawptr = struct.unpack_from("<IIII", data, o + 8)
        if vaddr <= rva < vaddr + max(vsize, rawsize):
            return rawptr + (rva - vaddr)
    raise ValueError("RVA not found")

def streams(data):
    e_lfanew = struct.unpack_from("<I", data, 0x3C)[0]
    coff = e_lfanew + 4
    opt = coff + 20
    magic = struct.unpack_from("<H", data, opt)[0]
    dd_off = opt + (96 if magic == 0x10B else 112)
    cli_rva, _ = struct.unpack_from("<II", data, dd_off + 14 * 8)
    cli_off = rva_to_off(data, cli_rva)
    md_rva, _ = struct.unpack_from("<II", data, cli_off + 8)
    md_off = rva_to_off(data, md_rva)
    if data[md_off:md_off+4] != b"BSJB":
        raise ValueError("Not a .NET assembly")
    ver_len = struct.unpack_from("<I", data, md_off + 12)[0]
    p = (md_off + 16 + ver_len + 3) & ~3
    _, count = struct.unpack_from("<HH", data, p); p += 4
    result = {}
    for _ in range(count):
        off, size = struct.unpack_from("<II", data, p); p += 8
        end = data.index(b"\0", p)
        name = data[p:end].decode("ascii")
        p = (end + 1 + 3) & ~3
        result[name] = (md_off + off, size)
    return result

def cuint(buf, p):
    b0 = buf[p]
    if b0 < 0x80: return b0, p+1
    if b0 < 0xC0: return ((b0 & 0x3F) << 8) | buf[p+1], p+2
    if b0 < 0xE0: return ((b0 & 0x1F) << 24) | (buf[p+1]<<16) | (buf[p+2]<<8) | buf[p+3], p+4
    raise ValueError("Invalid compressed integer")

def user_strings(path):
    data = Path(path).read_bytes()
    off, size = streams(data)["#US"]
    heap = data[off:off+size]
    p, result = 1, []
    while p < len(heap):
        n, q = cuint(heap, p)
        if n == 0:
            p = q
            continue
        payload = heap[q:q+n]
        if len(payload) != n: break
        result.append(payload[:-1].decode("utf-16le", "replace"))
        p = q+n
    return result

ap = argparse.ArgumentParser()
ap.add_argument("baseline_original")
ap.add_argument("baseline_korean")
ap.add_argument("new_original")
ap.add_argument("--memory", default="translation_memory.csv")
ap.add_argument("--delta", default="update_delta.csv")
a = ap.parse_args()

src = user_strings(a.baseline_original)
ko = user_strings(a.baseline_korean)
new = user_strings(a.new_original)
if len(src) != len(ko):
    raise SystemExit(f"baseline mismatch: {len(src)} != {len(ko)}")

pairs = [(s, k) for s, k in zip(src, ko)
         if s != k and re.search(r"[\u4e00-\u9fff]", s) and re.search(r"[가-힣]", k)]
mem = dict(pairs)

with open(a.memory, "w", encoding="utf-8-sig", newline="") as f:
    w = csv.writer(f); w.writerow(["source","ko"]); w.writerows(pairs)

new_cn = [s for s in new if re.search(r"[\u4e00-\u9fff]", s)]
pending = [s for s in new_cn if s not in mem]
with open(a.delta, "w", encoding="utf-8-sig", newline="") as f:
    w = csv.writer(f); w.writerow(["source","ko","status"])
    for s in pending: w.writerow([s,"","NEW_OR_CHANGED"])

print(f"memory={len(pairs)}")
print(f"new_cn={len(new_cn)} reused={len(new_cn)-len(pending)} pending={len(pending)}")
