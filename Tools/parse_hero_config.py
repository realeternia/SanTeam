#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Tools/parse_hero_config.py

解析 Assets/Resources/Scripts/Configs/HeroConfig_s.cs 中的 config[...] = new HeroConfig(...) 配置行，
并导出 CSV / Markdown 的统计数据：每个职业人数、(职业, 阵营) 分布、品质分布。

用法：
  python Tools/parse_hero_config.py --source-url <raw-heroconfig-url> --out-csv Tools/hero_stats.csv --out-md Tools/hero_stats.md

示例：
  python Tools/parse_hero_config.py \
    --source-url https://raw.githubusercontent.com/realeternia/SanTeam/main/Assets/Resources/Scripts/Configs/HeroConfig_s.cs \
    --out-csv Tools/hero_stats.csv --out-md Tools/hero_stats.md

脚本会尝试从给定 URL 下载源码（若未提供则尝试从本地路径），解析所有 new HeroConfig(...) 的参数，
并按照参数位置抽取：
  - id: args[0]
  - name: args[1]
  - side: args[14]
  - job: args[20]
  - quality: args[25]

注意：该索引基于当前 HeroConfig 构造函数签名，如源码变更请调整索引。
"""

from __future__ import annotations
import re
import argparse
import csv
import sys
from collections import defaultdict


def split_args(argstr: str):
    """将参数字符串按逗号分割，忽略引号内的逗号。返回参数列表（去除空白）。"""
    args = []
    cur = []
    in_quote = False
    escape = False
    quote_char = None
    for ch in argstr:
        if escape:
            cur.append(ch)
            escape = False
            continue
        if ch == "\\":
            escape = True
            cur.append(ch)
            continue
        if in_quote:
            cur.append(ch)
            if ch == quote_char:
                in_quote = False
            continue
        if ch == '"' or ch == "'":
            in_quote = True
            quote_char = ch
            cur.append(ch)
            continue
        if ch == ',':
            arg = ''.join(cur).strip()
            args.append(arg)
            cur = []
            continue
        cur.append(ch)
    # append last
    if cur:
        args.append(''.join(cur).strip())
    return args


def parse_heroconfig_text(text: str):
    # 寻找所有 config[...] = new HeroConfig(...);
    # 使用正则捕获括号内的参数（非贪婪），支持跨行。
    pattern = re.compile(r'config\[\d+\]\s*=\s*new\s+HeroConfig\((.*?)\)\s*;', re.S)
    matches = pattern.findall(text)
    heroes = []
    for m in matches:
        argstr = m.strip()
        args = split_args(argstr)
        # defensive: require at least 26 args to access indices used below
        if len(args) < 26:
            # skip or try best-effort
            continue
        try:
            hid = args[0]
            # remove surrounding quotes for strings
            def unquote(s):
                s = s.strip()
                if (s.startswith('"') and s.endswith('"')) or (s.startswith("'") and s.endswith("'")):
                    return s[1:-1]
                return s
            name = unquote(args[1])
            side = unquote(args[14])
            job = unquote(args[20])
            quality = unquote(args[25])
            heroes.append({
                'id': hid,
                'name': name,
                'side': side,
                'job': job,
                'quality': quality,
            })
        except Exception as e:
            # ignore parse error for this entry
            continue
    return heroes


def summarize(heroes):
    job_count = defaultdict(int)
    job_side_count = defaultdict(lambda: defaultdict(int))
    job_quality_count = defaultdict(lambda: defaultdict(int))
    side_count = defaultdict(int)
    for h in heroes:
        job = h['job'] or 'UNKNOWN'
        side = h['side'] or 'UNKNOWN'
        quality = h['quality'] or 'UNKNOWN'
        job_count[job] += 1
        job_side_count[job][side] += 1
        job_quality_count[job][quality] += 1
        side_count[side] += 1
    return job_count, job_side_count, job_quality_count, side_count


def write_csv(heroes, path):
    with open(path, 'w', newline='', encoding='utf-8') as f:
        writer = csv.writer(f)
        writer.writerow(['Id', 'Name', 'Side', 'Job', 'Quality'])
        for h in heroes:
            writer.writerow([h['id'], h['name'], h['side'], h['job'], h['quality']])


def write_md_summary(job_count, job_side_count, job_quality_count, path):
    with open(path, 'w', encoding='utf-8') as f:
        f.write('# Hero Job 统计\n\n')
        f.write('## 职业总人数\n\n')
        f.write('| Job | Count |\n')
        f.write('|-----|-------|\n')
        for job, cnt in sorted(job_count.items(), key=lambda x: -x[1]):
            f.write(f'| {job} | {cnt} |\n')
        f.write('\n')
        f.write('## 职业按 Side 分布（部分）\n\n')
        for job, sides in job_side_count.items():
            f.write(f'### {job}\n')
            f.write('| Side | Count |\n')
            f.write('|------|-------|\n')
            for side, cnt in sorted(sides.items(), key=lambda x: -x[1]):
                f.write(f'| {side} | {cnt} |\n')
            f.write('\n')
        f.write('## 职业品质分布（Quality）\n\n')
        for job, quals in job_quality_count.items():
            f.write(f'### {job}\n')
            f.write('| Quality | Count |\n')
            f.write('|---------|-------|\n')
            for q, cnt in sorted(quals.items(), key=lambda x: -int(x[0]) if x[0].isdigit() else -cnt):
                f.write(f'| {q} | {cnt} |\n')
            f.write('\n')


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--source-url', help='HeroConfig_s.cs raw URL (raw.githubusercontent.com)')
    parser.add_argument('--source-path', help='Local path to HeroConfig_s.cs (fallback)')
    parser.add_argument('--out-csv', default='Tools/hero_stats.csv')
    parser.add_argument('--out-md', default='Tools/hero_stats.md')
    args = parser.parse_args()

    text = None
    if args.source_url:
        try:
            import urllib.request
            with urllib.request.urlopen(args.source_url) as resp:
                text = resp.read().decode('utf-8')
        except Exception as e:
            print('Failed to download from URL:', e, file=sys.stderr)
            text = None
    if text is None and args.source_path:
        with open(args.source_path, 'r', encoding='utf-8') as f:
            text = f.read()
    if text is None:
        print('No source provided. Use --source-url or --source-path', file=sys.stderr)
        sys.exit(2)

    heroes = parse_heroconfig_text(text)
    print(f'Parsed {len(heroes)} hero entries')
    if len(heroes) == 0:
        print('No entries parsed. The parsing indexes may need adjustment.', file=sys.stderr)

    job_count, job_side_count, job_quality_count, side_count = summarize(heroes)
    write_csv(heroes, args.out_csv)
    write_md_summary(job_count, job_side_count, job_quality_count, args.out_md)
    print('Wrote:', args.out_csv, args.out_md)


if __name__ == '__main__':
    main()
