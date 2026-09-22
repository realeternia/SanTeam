#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
将 SkillConfig_s.cs 的 DescriptVal 数据转换为"Lv1 填字段引用模板，其余等级留空"的新形式。
用法:
  python run.py            # 仅统计/预览(不写文件)
  python run.py --apply    # 实际改写文件(自动备份 .bak)
"""
import sys, io
from collections import OrderedDict
import re

SRC = r"d:\U3dPrj\SanTeam\Assets\Resources\Scripts\Configs\SkillConfig_s.cs"

# (字段名, 类型)  类型: percent/int/float
CANDIDATES = [
    ("rate", "percent"), ("skilldamagerate", "percent"),
    ("mpcost", "int"), ("targetcount", "int"), ("strengthint", "int"),
    ("range", "float"), ("area", "float"), ("strength", "float"),
    ("bufftime", "float"), ("summontime", "float"), ("summonspeed", "float"),
    ("effectsize", "float"), ("attackpointreduce", "float"),
    ("cd", "float"), ("lv", "int"),
]
# 字段 -> 构造参数 args 下标
FIDX = {"rate":8,"cd":9,"mpcost":10,"attackpointreduce":12,"range":16,"area":17,
        "targetcount":19,"strength":20,"strengthint":21,"skilldamagerate":22,
        "bufftime":28,"summontime":31,"summonspeed":33,"effectsize":38,"lv":7}

def pretty_num(v, kind):
    if kind == "int":
        return str(int(round(v)))
    if abs(v - round(float(v))) < 1e-9:
        return str(int(round(float(v))))
    return ("%.2f" % float(v)).rstrip("0").rstrip(".")

def fmt_val(field, args, pct):
    """构造参数(原始字符串 args)按字段格式化展示，等价 ConfigManager.GetFieldRefValue(cfg,field,pct)。"""
    kind = next(k for f,k in CANDIDATES if f==field)
    raw = args[FIDX[field]].replace("f","")
    v = float(raw)
    if kind == "percent" or pct:
        return pretty_num(v*100.0, "float") + "%"
    return pretty_num(v, kind)

def fmt_field(field, args):
    return fmt_val(field, args, False)

def fmt_field_pct(field, args):
    return fmt_val(field, args, True)

def decode_str(tok):
    tok=tok.strip()
    if tok.startswith('"') and tok.endswith('"'):
        return tok[1:-1]
    return tok

def tokenize_args(line):
    """返回 [(start,end,text)]。"""
    toks=[]; start=0; in_str=False
    for i0,c in enumerate(line):
        if c=='"':
            in_str = not in_str
        elif c==',' and not in_str:
            toks.append((start,i0,line[start:i0])); start=i0+1
    toks.append((start,len(line),line[start:]))
    return toks

def parse_records(text):
    out=[]
    lines=text.split('\n') if isinstance(text,str) else text
    for idx,line in enumerate(lines):
        m=re.search(r'new SkillConfig\(', line)
        if not m: continue
        argstart=m.end()
        close=line.rfind(');')
        argsrc=line[argstart:close] if close!=-1 else line[argstart:]
        toks=tokenize_args(argsrc)
        args=[t[2] for t in toks]
        if len(args)<5: continue
        out.append((idx, args, toks, argstart))
    return out

def set_arg(text_lines, rec, new_inner):
    """替换该记录 args[4](DescriptVal) 的引号内内容为 new_inner。"""
    idx,args,toks,argstart = rec
    st,en,raw = toks[4]
    line=text_lines[idx]
    # token 可能带前导空格，需定位到首个/末个引号（内容起始=起始引号+1）
    abs_start=argstart+st+raw.find('"')+1
    abs_end=argstart+st+raw.rfind('"')
    text_lines[idx]=line[:abs_start]+new_inner+line[abs_end:]

def substitute(template, args):
    fm={f:fmt_field(f,args) for f,_ in CANDIDATES}
    fm.update({f+'%':fmt_field_pct(f,args) for f,_ in CANDIDATES})
    out=[];i=0
    while i<len(template):
        if template[i]=='/' and i+1<len(template) and template[i+1].isalpha():
            j=i+1
            while j<len(template) and (template[j].isalnum() or template[j]=='_'):
                j+=1
            name=template[i+1:j]
            if j<len(template) and template[j]=='%':
                name=name+'%'; j+=1
            out.append(fm.get(name, template[i:j]))
            i=j
        else:
            out.append(template[i]); i+=1
    return ''.join(out)

def convert_group(grecs):
    """返回 (new_lv1, [below_level_recs]) 或 None。grecs: [(idx,args,toks,argstart)] 同 Sname。"""
    bylv={}
    for rec in grecs:
        args=rec[1]; lv=int(args[7].replace("f",""))
        bylv[lv]=rec
    if 1 not in bylv: return None
    lv1=bylv[1]; l1_desc=decode_str(lv1[1][3]).strip()
    if not l1_desc: return None
    levels=sorted(bylv.keys())
    if len(levels)<2: return None
    for lv in levels:
        if lv!=1 and decode_str(bylv[lv][1][3]).strip():  # 结构兜底技能
            return None
    params={}; cnts={}
    for lv in levels:
        dv=decode_str(bylv[lv][1][4])
        if not dv.strip(): return None
        params[lv]=dv.split(';'); cnts[lv]=len(params[lv])
    N=cnts[1]
    if any(c!=N for c in cnts.values()): return None

    tokens=[]
    for i in range(N):
        vals=[params[lv][i] for lv in levels]
        if all(v==vals[0] for v in vals[1:]):
            tokens.append(vals[0]); continue
        chosen=None
        for field,_ in CANDIDATES:
            # 尝试原始格式
            prefixes=set()
            ok=True
            for lv in levels:
                f=fmt_field(field, bylv[lv][1]); val=params[lv][i]
                if val==f: prefixes.add('')
                elif val==('+'+f): prefixes.add('+')
                elif val==('-'+f): prefixes.add('-')
                else: ok=False; break
            if ok and len(prefixes)==1:
                chosen=prefixes.pop()+'/'+field; break
            # 尝试百分比格式（/字段名%）
            pprefixes=set()
            pok=True
            for lv in levels:
                f=fmt_field_pct(field, bylv[lv][1]); val=params[lv][i]
                if val==f: pprefixes.add('')
                elif val==('+'+f): pprefixes.add('+')
                elif val==('-'+f): pprefixes.add('-')
                else: pok=False; break
            if pok and len(pprefixes)==1:
                chosen=pprefixes.pop()+'/'+field+'%'; break
        if chosen is None:
            return None
        tokens.append(chosen)
    new_lv1=';'.join(tokens)
    for lv in levels:
        if substitute(new_lv1, bylv[lv][1]) != decode_str(bylv[lv][1][4]):
            return None
    return (new_lv1, [bylv[lv] for lv in levels if lv!=1])

def main():
    apply='--apply' in sys.argv
    with io.open(SRC,'r',encoding='utf-8') as f:
        text=f.read()
    # parse_records 需要基于每一行，故按行数组传递
    lines=text.split('\n')
    recs=[]
    for idx,line in enumerate(lines):
        m=re.search(r'new SkillConfig\(', line)
        if not m: continue
        argstart=m.end(); close=line.rfind(');')
        argsrc=line[argstart:close] if close!=-1 else line[argstart:]
        toks=tokenize_args(argsrc)
        args=[t[2] for t in toks]
        if len(args)<5: continue
        recs.append((idx,args,toks,argstart))
    groups=OrderedDict()
    for rec in recs:
        sname=decode_str(rec[1][2]).strip()
        groups.setdefault(sname,[]).append(rec)
    stats={'ok':[], 'keep':[]}
    results={}
    for sname,grecs in groups.items():
        r=convert_group(grecs)
        results[sname]=r
        if r is None: stats['keep'].append(sname)
        else: stats['ok'].append(sname)
    print("技能组总数:%d  |  可转字段引用:%d  |  保底不转:%d"%(len(groups),len(stats['ok']),len(stats['keep'])))
    print("\n前%d个转换结果(Lv1模板):"%(20 if not apply else 1000))
    for sname in stats['ok'][:20]:
        print("  %-8s -> %s"%(sname, results[sname][0]))
    if apply:
        newlines=lines[:]
        for sname in stats['ok']:
            new_lv1, below = results[sname]
            lv1 = [g for g in groups[sname] if int(g[1][7].replace('f',''))==1][0]
            set_arg(newlines, lv1, new_lv1)
            for rec in below:
                set_arg(newlines, rec, "")
        with io.open(SRC+'.bak','w',encoding='utf-8') as f: f.write(text)
        with io.open(SRC,'w',encoding='utf-8') as f: f.write('\n'.join(newlines))
        print("\n已写入 %s(已备份 %s.bak)"%(SRC,SRC))
    else:
        print("\n(dry-run 未写文件。确认后用: python run.py --apply)")

if __name__=='__main__':
    main()