#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Tools/hero_distribution_report.py

读取 HeroConfig_s.cs / HeroFriendConfig_s.cs / ForceConfig_s.cs / JobConfig_s.cs /
ScenarioConfig_s.cs，统计三个剧本里的英雄分布，并生成一个单文件 HTML 报告（含三个 Tab）。

统计项（每个剧本）：
  1. 总英雄数
  2. 各国（势力）英雄数量分布
  3. 品质分布
  4. 职业分布
  5. 各好友羁绊（HeroFriendConfig 组）人数
  6. 各羁绊组合规检查（同品质 ≤2、同职业 ≤2、同国家 ≤3、同一对武将不得在多个组重复）

用法：
  python Tools/hero_distribution_report.py
  python Tools/hero_distribution_report.py --out Tools/hero_distribution.html
"""

from __future__ import annotations

import argparse
import colorsys
import html
import re
from collections import defaultdict
from datetime import datetime
from itertools import combinations
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CONFIG_DIR = ROOT / "Assets" / "Resources" / "Scripts" / "Configs"

DEFAULT_HERO = CONFIG_DIR / "HeroConfig_s.cs"
DEFAULT_FRIEND = CONFIG_DIR / "HeroFriendConfig_s.cs"
DEFAULT_FORCE = CONFIG_DIR / "ForceConfig_s.cs"
DEFAULT_JOB = CONFIG_DIR / "JobConfig_s.cs"
DEFAULT_SCENARIO = CONFIG_DIR / "ScenarioConfig_s.cs"
DEFAULT_OUT = ROOT / "Tools" / "hero_distribution.html"

QUALITY_COLORS = {
    1: "#A0A0A0",
    2: "#33CC33",
    3: "#5B7FFF",
    4: "#FFD700",
    5: "#FF4757",
}
QUALITY_NAMES = {1: "普通", 2: "优秀", 3: "精良", 4: "史诗", 5: "传说"}
JOB_HUE_OFFSET = 28

# 羁绊组合规阈值：同一组内同品质、同职业、同国家的出场人数上限
BOND_MAX_QUALITY = 2
BOND_MAX_JOB = 2
BOND_MAX_SIDE = 3


def job_color(index: int, total: int) -> str:
    """按职业序号均匀取色，保证 18 个职业颜色互不相同。"""
    hue = (index / max(total, 1) * 360 + JOB_HUE_OFFSET) % 360
    r, g, b = colorsys.hls_to_rgb(hue / 360, 0.62, 0.62)
    return "#%02X%02X%02X" % (round(r * 255), round(g * 255), round(b * 255))


def read_text(path: Path) -> str:
    if not path.is_file():
        raise SystemExit(f"找不到配置文件：{path}")
    return path.read_text(encoding="utf-8-sig")


def split_args(argstr: str):
    """按顶层逗号切分参数，忽略引号内与括号/花括号/方括号内的逗号。"""
    args, cur = [], []
    in_quote = False
    quote_char = ""
    depth = 0
    escape = False
    for ch in argstr:
        if escape:
            cur.append(ch)
            escape = False
            continue
        if ch == "\\":
            cur.append(ch)
            escape = True
            continue
        if in_quote:
            cur.append(ch)
            if ch == quote_char:
                in_quote = False
            continue
        if ch in ('"', "'"):
            in_quote = True
            quote_char = ch
            cur.append(ch)
            continue
        if ch in "([{":
            depth += 1
        elif ch in ")]}":
            depth -= 1
        if ch == "," and depth == 0:
            args.append("".join(cur).strip())
            cur = []
            continue
        cur.append(ch)
    if cur:
        args.append("".join(cur).strip())
    return args


def unquote(s: str) -> str:
    s = s.strip()
    if len(s) >= 2 and s[0] == s[-1] and s[0] in ('"', "'"):
        return s[1:-1]
    return s


def iter_config_rows(text: str, type_name: str):
    """按行匹配 config[...] = new XxxConfig(...); 跳过被注释的行。"""
    pattern = re.compile(
        r"config\[[^\]]+\]\s*=\s*new\s+" + type_name + r"\((.*?)\)\s*;", re.S
    )
    for m in pattern.finditer(text):
        line_start = text.rfind("\n", 0, m.start()) + 1
        if "//" in text[line_start:m.start()]:
            continue
        yield m.group(1)


def parse_heroes(text: str):
    heroes = []
    for argstr in iter_config_rows(text, "HeroConfig"):
        args = split_args(argstr)
        if len(args) < 22:
            continue
        heroes.append(
            {
                "id": int(args[0]),
                "name": unquote(args[1]),
                "job": unquote(args[3]),
                "scenarios": [unquote(args[4]), unquote(args[5]), unquote(args[6])],
            }
        )
    return heroes


def parse_friend_groups(text: str):
    groups = []
    for argstr in iter_config_rows(text, "HeroFriendConfig"):
        args = split_args(argstr)
        if len(args) < 3:
            continue
        groups.append(
            {
                "id": int(args[0]),
                "name": unquote(args[1]),
                "heroIds": [int(x) for x in re.findall(r"\d+", args[3])] if len(args) > 3 else [],
                "lineColor": unquote(args[5]) if len(args) > 5 else "",
            }
        )
    return groups


def parse_forces(text: str):
    forces = []
    for argstr in iter_config_rows(text, "ForceConfig"):
        args = split_args(argstr)
        if len(args) < 4:
            continue
        forces.append(
            {
                "id": int(args[0]),
                "name": unquote(args[1]),
                "short": unquote(args[2]),
                "color": unquote(args[3]),
            }
        )
    return forces


def parse_jobs(text: str):
    jobs = []
    for argstr in iter_config_rows(text, "JobConfig"):
        args = split_args(argstr)
        if len(args) < 3:
            continue
        jobs.append(
            {"id": int(args[0]), "name": unquote(args[1]), "short": unquote(args[2])}
        )
    return jobs


def parse_scenarios(text: str):
    scenarios = {}
    for m in re.finditer(r'new\s+ScenarioConfig\((\d+)\s*,\s*"([^"]*)"\)', text):
        scenarios[int(m.group(1))] = m.group(2)
    return scenarios


def brighten(hex_color: str, min_max_channel: int = 200) -> str:
    """把深色系强调色提亮到适合深色背景上显示。"""
    hex_color = (hex_color or "").strip().lstrip("#")
    if len(hex_color) != 6:
        return "#7A8290"
    r, g, b = (int(hex_color[i : i + 2], 16) for i in (0, 2, 4))
    max_c = max(r, g, b, 1)
    if max_c < min_max_channel:
        k = min_max_channel / max_c
        r, g, b = (min(255, int(round(c * k))) for c in (r, g, b))
    return f"#{r:02X}{g:02X}{b:02X}"


def build_panel(scenario_id, heroes, force_map, job_map, friend_groups, hero_by_id, active=False):
    in_scenario = []
    for h in heroes:
        raw = h["scenarios"][scenario_id - 1] if len(h["scenarios"]) >= scenario_id else ""
        if not raw:
            continue
        parts = raw.split(":", 1)
        quality = int(parts[0]) if parts[0].strip().isdigit() else 0
        side = parts[1].strip() if len(parts) > 1 else ""
        in_scenario.append({"hero": h, "quality": quality, "side": side})

    total = len(in_scenario)

    # 势力分布
    side_counts = defaultdict(int)
    for item in in_scenario:
        side_counts[item["side"] or "未知"] += 1
    side_order = {}
    side_color = {}
    for idx, f in enumerate(sorted(force_map.values(), key=lambda x: x["id"])):
        side_order[f["short"]] = idx
        side_color[f["short"]] = brighten(f["color"])
        side_color[f["name"]] = brighten(f["color"])
    side_rows = [
        {
            "label": name,
            "count": cnt,
            "color": side_color.get(name, "#7A8290"),
            "title": f"{name}：{cnt} 位英雄",
        }
        for name, cnt in side_counts.items()
    ]
    side_rows.sort(key=lambda r: (-r["count"], side_order.get(r["label"], 99)))

    # 品质分布
    quality_counts = defaultdict(int)
    for item in in_scenario:
        quality_counts[item["quality"]] += 1
    quality_rows = [
        {
            "label": f"Q{q} {QUALITY_NAMES.get(q, '未知')}",
            "count": cnt,
            "color": QUALITY_COLORS.get(q, "#7A8290"),
            "title": f"{QUALITY_NAMES.get(q, '未知')}：{cnt} 位英雄",
        }
        for q, cnt in sorted(quality_counts.items())
    ]

    # 职业分布
    job_counts = defaultdict(int)
    for item in in_scenario:
        job_counts[item["hero"]["job"] or "未知"] += 1
    job_order = {j["short"]: i for i, j in enumerate(sorted(job_map.values(), key=lambda x: x["id"]))}
    job_rows = []
    for short, cnt in job_counts.items():
        job = job_map.get(short)
        label = f"{job['name']}（{short}）" if job else (short or "未知")
        job_rows.append(
            {
                "label": label,
                "count": cnt,
                "color": job_color(job_order.get(short, 0), len(job_map)),
                "title": f"{label}：{cnt} 位英雄",
            }
        )
    job_rows.sort(key=lambda r: (-r["count"],))

    # 好友羁绊人数 + 合规检查
    bond_rows = []
    group_members = []
    for g in friend_groups:
        members = [item for item in in_scenario if item["hero"]["id"] in g["heroIds"]]
        group_members.append((g, members))
        names = [item["hero"]["name"] for item in members]
        bond_rows.append(
            {
                "label": g["name"],
                "count": len(members),
                "color": brighten(g["lineColor"] or "#5B9BD5"),
                "title": f"{g['name']}：{len(members)} 人"
                + (f"（{'、'.join(names)}）" if names else "（本剧本无成员）"),
            }
        )

    # 每对武将同时出现在哪些羁绊组（用于重复羁绊对检查）
    pair_groups = defaultdict(set)
    for g, members in group_members:
        ids = sorted(item["hero"]["id"] for item in members)
        for a, b in combinations(ids, 2):
            pair_groups[(a, b)].add(g["name"])

    violations = []
    for g, members in group_members:
        checks = []
        bad_names = set()
        # 同品质 > BOND_MAX_QUALITY
        quality_map = defaultdict(list)
        for item in members:
            if item["quality"] > 0:
                quality_map[item["quality"]].append(item["hero"]["name"])
        for q, ns in sorted(quality_map.items()):
            if len(ns) > BOND_MAX_QUALITY:
                checks.append(f"品质 Q{q}×{len(ns)}（{'、'.join(ns)}）")
                bad_names.update(ns)

        # 同职业 > BOND_MAX_JOB
        job_members = defaultdict(list)
        for item in members:
            job_members[item["hero"]["job"] or "未知"].append(item["hero"]["name"])
        for short, ns in sorted(job_members.items()):
            if len(ns) > BOND_MAX_JOB:
                job = job_map.get(short)
                label = job["name"] if job else (short or "未知")
                checks.append(f"职业 {label}×{len(ns)}（{'、'.join(ns)}）")
                bad_names.update(ns)

        # 同国家 > BOND_MAX_SIDE
        side_members = defaultdict(list)
        for item in members:
            side_members[item["side"] or "未知"].append(item["hero"]["name"])
        for side, ns in sorted(side_members.items()):
            if len(ns) > BOND_MAX_SIDE:
                checks.append(f"国家 {side}×{len(ns)}（{'、'.join(ns)}）")
                bad_names.update(ns)

        # 同一对武将还在别的羁绊组中同时出现
        name_by_id = {item["hero"]["id"]: item["hero"]["name"] for item in members}
        dup_pairs = []
        for a, b in combinations(sorted(name_by_id), 2):
            others = pair_groups[(a, b)] - {g["name"]}
            if others:
                dup_pairs.append(
                    f"{name_by_id[a]}-{name_by_id[b]}（{'/'.join(sorted(others))}）"
                )
        if dup_pairs:
            shown = "、".join(dup_pairs[:3])
            more = f" 等{len(dup_pairs)}对" if len(dup_pairs) > 3 else ""
            checks.append(f"重复羁绊对 {len(dup_pairs)} 对：{shown}{more}")

        if checks:
            member_rows = []
            for item in members:
                short = item["hero"]["job"] or ""
                job = job_map.get(short)
                member_rows.append(
                    {
                        "name": item["hero"]["name"],
                        "bad": item["hero"]["name"] in bad_names,
                        "quality": item["quality"],
                        "job": job["name"] if job else (short or "未知"),
                        "side": item["side"] or "未知",
                    }
                )
            violations.append(
                {
                    "name": g["name"],
                    "count": len(members),
                    "checks": checks,
                    "members": member_rows,
                }
            )

    bond_rows.sort(key=lambda r: (-r["count"], r["label"]))
    violations.sort(key=lambda v: (-len(v["checks"]), v["name"]))
    bond_groups_active = sum(1 for r in bond_rows if r["count"] > 0)

    panel = [
        '<section class="panel{}" id="panel-{}" data-scenario="{}">'.format(
            " active" if active else "", scenario_id, scenario_id
        ),
        '<div class="stats">',
        f'<div class="stat"><div class="num">{total}</div><div class="lbl">总英雄数</div></div>',
        f'<div class="stat"><div class="num">{len(side_counts)}</div><div class="lbl">势力数</div></div>',
        f'<div class="stat"><div class="num">{len(job_counts)}</div><div class="lbl">职业数</div></div>',
        f'<div class="stat"><div class="num">{bond_groups_active}</div><div class="lbl">有成员的羁绊组</div></div>',
        f'<div class="stat"><div class="num">{len(violations)}</div><div class="lbl">不合规羁绊组</div></div>',
        "</div>",
        '<div class="grid">',
        render_card("国家分布", f"{len(side_counts)} 个势力", side_rows, total),
        render_card("品质分布", f"{len(quality_rows)} 个品质档", quality_rows, total),
        render_card("职业分布", f"{len(job_counts)} 个职业", job_rows, total),
        render_card("好友羁绊人数", f"{len(friend_groups)} 个羁绊组", bond_rows, total),
        render_violation_card(violations, len(friend_groups)),
        "</div>",
        "</section>",
    ]
    return "\n".join(panel), total, len(violations)


def render_violation_card(violations, total_groups: int) -> str:
    """渲染羁绊组合规检查：列出超标的品质/职业/国家/重复羁绊对及对应武将。"""
    type_counts = defaultdict(int)
    for v in violations:
        hit = set()
        for c in v["checks"]:
            for key in ("品质", "职业", "国家", "重复羁绊对"):
                if c.startswith(key):
                    hit.add(key)
        for key in hit:
            type_counts[key] += 1
    summary = " · ".join(
        f"{key} {type_counts[key]} 组" for key in ("品质", "职业", "国家", "重复羁绊对")
    )
    if violations:
        items = []
        for v in violations:
            tags = "".join(
                f'<span class="tag">{html.escape(c)}</span>' for c in v["checks"]
            )
            members = "".join(
                '<span class="member{}">{}<em>Q{}·{}·{}</em></span>'.format(
                    " bad" if m["bad"] else "",
                    html.escape(m["name"]),
                    m["quality"] if m["quality"] > 0 else "?",
                    html.escape(m["job"]),
                    html.escape(m["side"]),
                )
                for m in v["members"]
            )
            items.append(
                '<div class="viol-item">'
                f'<div class="viol-head"><b>{html.escape(v["name"])}</b><span>{v["count"]} 人</span></div>'
                f'<div class="viol-tags">{tags}</div>'
                f'<div class="viol-members">{members}</div>'
                "</div>"
            )
        body = "\n".join(items)
        sub = f"{len(violations)} / {total_groups} 组不合规"
    else:
        body = '<div class="viol-ok">全部羁绊组合规</div>'
        sub = f"0 / {total_groups} 组不合规"
    return (
        '<div class="card">'
        f'<div class="card-head"><h2>羁绊组合规检查</h2><span class="sub">{sub}</span></div>'
        f'<div class="rules"><b>违规统计：</b>{summary}<br>'
        f'规则：同品质 ≤{BOND_MAX_QUALITY} 人、同职业 ≤{BOND_MAX_JOB} 人、同国家 ≤{BOND_MAX_SIDE} 人；'
        "同一对武将不得在多个羁绊组中同时出现（按本剧本出场成员统计）</div>"
        f'<div class="viol">{body}</div>'
        "</div>"
    )


def render_card(title: str, sub: str, rows, total: int) -> str:
    max_count = max([r["count"] for r in rows] + [1])
    body = []
    for r in rows:
        pct = (r["count"] / total * 100) if total else 0
        width = (r["count"] / max_count * 100) if max_count else 0
        dim = ' style="opacity:.35"' if r["count"] == 0 else ""
        body.append(
            '<div class="row"{dim} title="{title}">'
            '<span class="label">{label}</span>'
            '<div class="track"><div class="fill" style="width:{width:.1f}%;background:linear-gradient(90deg,{color}44,{color})"></div></div>'
            '<span class="val"><b>{count}</b><em>{pct:.1f}%</em></span>'
            "</div>".format(
                dim=dim,
                title=html.escape(r["title"], quote=True),
                label=html.escape(r["label"]),
                width=width,
                color=r["color"],
                count=r["count"],
                pct=pct,
            )
        )
    return (
        '<div class="card">'
        f'<div class="card-head"><h2>{html.escape(title)}</h2><span class="sub">{html.escape(sub)}</span></div>'
        '<div class="bars">' + "\n".join(body) + "</div>"
        "</div>"
    )


CSS = """
:root{
  --bg:#14161a; --card:#1d2127; --card2:#22272f; --line:#2b313a;
  --text:#e7ebf1; --muted:#8b93a1; --accent:#d4a24a;
}
*{box-sizing:border-box;}
html,body{margin:0;padding:0;}
body{
  background:radial-gradient(1200px 500px at 50% -10%, #1f2733 0%, var(--bg) 60%) var(--bg);
  color:var(--text);
  font-family:"Microsoft YaHei","PingFang SC","Segoe UI",system-ui,sans-serif;
  min-height:100vh;
  padding:32px 24px 56px;
}
.header{max-width:1180px;margin:0 auto 22px;}
.header h1{margin:0;font-size:26px;letter-spacing:1px;font-weight:700;}
.header h1 span{color:var(--accent);}
.header .meta{margin-top:8px;color:var(--muted);font-size:13px;}
.tabs{max-width:1180px;margin:0 auto 18px;display:flex;gap:10px;flex-wrap:wrap;}
.tab{
  border:1px solid var(--tab-line,var(--line));background:var(--tab-bg,var(--card));
  color:var(--tab-text,var(--muted));
  padding:10px 22px;border-radius:10px;cursor:pointer;font-size:15px;
  transition:.18s;display:flex;align-items:center;gap:10px;
}
.tab[data-scenario="1"]{--tab-bg:linear-gradient(180deg,#2b261c,#211d16);--tab-line:#5a4a2c;--tab-text:#d8c79f;--tab-bg-a:linear-gradient(180deg,#3a3323,#2a2519);--tab-accent:#d4a24a;--tab-active-text:#ffdd9a;--tab-glow:rgba(212,162,74,.35);--tab-badge:rgba(212,162,74,.22);}
.tab[data-scenario="2"]{--tab-bg:linear-gradient(180deg,#1c2731,#171e26);--tab-line:#2f4a5e;--tab-text:#9fc3d8;--tab-bg-a:linear-gradient(180deg,#22384a,#182731);--tab-accent:#4fa3d1;--tab-active-text:#bfe3ff;--tab-glow:rgba(79,163,209,.35);--tab-badge:rgba(79,163,209,.2);}
.tab[data-scenario="3"]{--tab-bg:linear-gradient(180deg,#271f2f,#1d1823);--tab-line:#4a3560;--tab-text:#c3a8dd;--tab-bg-a:linear-gradient(180deg,#362745,#251c30);--tab-accent:#a87bd6;--tab-active-text:#e3ccff;--tab-glow:rgba(168,123,214,.35);--tab-badge:rgba(168,123,214,.2);}
.tab .badge{
  font-size:12px;color:var(--tab-text,var(--muted));background:#2c333d;border-radius:99px;
  padding:2px 9px;transition:.18s;
}
.tab:hover{border-color:var(--tab-accent,var(--accent));color:var(--tab-active-text,var(--text));}
.tab.active{
  background:var(--tab-bg-a,linear-gradient(180deg,#3a3323,#2a2519));
  border-color:var(--tab-accent,var(--accent));color:var(--tab-active-text,#ffdd9a);
  box-shadow:0 0 0 1px var(--tab-glow,rgba(212,162,74,.35)),0 6px 20px rgba(0,0,0,.35);
}
.tab.active .badge{background:var(--tab-badge,rgba(212,162,74,.22));color:var(--tab-active-text,#ffdd9a);}
.panel{max-width:1180px;margin:0 auto;display:none;}
.panel.active{display:block;animation:fade .25s ease;}
@keyframes fade{from{opacity:0;transform:translateY(6px);}to{opacity:1;transform:none;}}
.stats{display:grid;grid-template-columns:repeat(auto-fit,minmax(160px,1fr));gap:14px;margin-bottom:14px;}
.stat{
  background:linear-gradient(180deg,var(--card2),var(--card));
  border:1px solid var(--line);border-radius:12px;padding:18px 20px;
}
.stat .num{font-size:30px;font-weight:700;color:var(--accent);line-height:1.1;}
.stat .lbl{margin-top:6px;font-size:13px;color:var(--muted);}
.grid{display:grid;grid-template-columns:repeat(auto-fit,minmax(420px,1fr));gap:14px;}
.card{
  background:linear-gradient(180deg,var(--card2),var(--card));
  border:1px solid var(--line);border-radius:12px;padding:16px 18px 18px;
}
.card-head{display:flex;align-items:baseline;justify-content:space-between;margin-bottom:12px;}
.card-head h2{margin:0;font-size:16px;font-weight:700;}
.card-head .sub{font-size:12px;color:var(--muted);}
.bars{display:flex;flex-direction:column;gap:8px;}
.row{display:grid;grid-template-columns:112px 1fr 96px;align-items:center;gap:10px;}
.row .label{font-size:13px;color:var(--text);white-space:nowrap;overflow:hidden;text-overflow:ellipsis;}
.row .track{height:14px;background:#161a20;border-radius:99px;overflow:hidden;border:1px solid #232830;}
.row .fill{height:100%;border-radius:99px;transition:width .6s cubic-bezier(.2,.7,.3,1);}
.row .val{text-align:right;font-size:12px;color:var(--muted);white-space:nowrap;}
.row .val b{color:var(--text);font-size:13px;margin-right:4px;}
.row .val em{font-style:normal;color:#6f7785;}
.legend{margin:16px 0 0;font-size:12.5px;color:var(--muted);line-height:1.9;}
.legend code{color:#ffdd9a;background:rgba(212,162,74,.12);padding:1px 6px;border-radius:5px;}
.rules{margin:-4px 0 10px;font-size:12px;color:var(--muted);line-height:1.6;}
.viol{display:flex;flex-direction:column;gap:10px;}
.viol-item{background:#1a1f26;border:1px solid #2b313a;border-left:3px solid #e0a94f;border-radius:8px;padding:10px 12px;}
.viol-head{display:flex;justify-content:space-between;align-items:baseline;font-size:13px;margin-bottom:7px;}
.viol-head b{color:var(--text);}
.viol-head span{color:var(--muted);font-size:12px;}
.viol-tags{display:flex;flex-wrap:wrap;gap:6px;}
.tag{font-size:11.5px;color:#ffd9a0;background:rgba(224,169,79,.14);border:1px solid rgba(224,169,79,.35);border-radius:6px;padding:2px 7px;line-height:1.6;}
.viol-members{display:flex;flex-wrap:wrap;gap:6px;margin-top:8px;padding-top:8px;border-top:1px dashed #2b313a;}
.member{display:inline-flex;align-items:baseline;gap:5px;font-size:11.5px;color:#9aa4b1;background:#141920;border:1px solid #262c35;border-radius:6px;padding:2px 7px;line-height:1.6;}
.member em{font-style:normal;font-size:10.5px;color:#6b7683;}
.member.bad{color:#ffb3a7;background:rgba(224,169,79,.08);border-color:rgba(224,169,79,.35);}
.member.bad em{color:#c79a6b;}
.viol-ok{color:#7ed6a5;font-size:13px;background:rgba(126,214,165,.08);border:1px solid rgba(126,214,165,.25);border-radius:8px;padding:10px 12px;}
@media (max-width:560px){
  .row{grid-template-columns:86px 1fr 84px;}
}
"""

JS = """
(function(){
  var tabs = Array.prototype.slice.call(document.querySelectorAll('.tab'));
  var panels = Array.prototype.slice.call(document.querySelectorAll('.panel'));
  tabs.forEach(function(tab){
    tab.addEventListener('click', function(){
      var id = tab.getAttribute('data-target');
      tabs.forEach(function(t){ t.classList.toggle('active', t === tab); });
      panels.forEach(function(p){ p.classList.toggle('active', p.id === id); });
    });
  });
})();
"""


def build_html(scenario_names, panels, heroes, friend_groups, generated_at) -> str:
    tabs = []
    for sid in sorted(scenario_names):
        total = panels[sid][1]
        active = " active" if sid == min(scenario_names) else ""
        tabs.append(
            '<button class="tab{}" data-target="panel-{}" data-scenario="{}">{}<span class="badge">{}人</span></button>'.format(
                active, sid, sid, html.escape(scenario_names[sid]), total
            )
        )
    bodies = [panels[sid][0] for sid in sorted(scenario_names)]

    return "".join(
        [
            "<!DOCTYPE html>\n<html lang=\"zh-CN\">\n<head>\n<meta charset=\"utf-8\">\n",
            '<meta name="viewport" content="width=device-width,initial-scale=1">\n',
            "<title>剧本英雄分布</title>\n<style>",
            CSS,
            "</style>\n</head>\n<body>\n",
            '<div class="header">\n<h1>剧本 <span>英雄分布</span></h1>\n',
            '<div class="meta">数据源：HeroConfig_s.cs / HeroFriendConfig_s.cs / ForceConfig_s.cs / JobConfig_s.cs'
            f' · 共 {len(heroes)} 位英雄、{len(friend_groups)} 个好友羁绊组 · 生成于 {generated_at}</div>\n</div>\n',
            '<div class="tabs">' + "".join(tabs) + "</div>\n",
            "\n".join(bodies) + "\n",
            '<div class="legend">',
            "说明：剧本字段格式为 <code>品质:势力</code>（如 <code>3:蜀</code>），留空表示该英雄在该剧本不出场；",
            "好友羁绊人数按 <code>HeroFriendConfig</code> 分组成员在本剧本中的出场数量统计；",
            "合规检查按本剧本出场成员统计：同一组内同品质 ≤2 人、同职业 ≤2 人、同国家 ≤3 人；",
            "同一对武将若在多个羁绊组中同时出现（重复羁绊对），也计为不合规；",
            "违规组下方列出全部出场成员的 <code>Q品质·职业·势力</code>，其中触发违规则的高亮显示。",
            "</div>\n",
            "<script>",
            JS,
            "</script>\n</body>\n</html>\n",
        ]
    )


def main():
    parser = argparse.ArgumentParser(description="生成三个剧本的英雄分布 HTML 报告")
    parser.add_argument("--hero", default=str(DEFAULT_HERO))
    parser.add_argument("--friend", default=str(DEFAULT_FRIEND))
    parser.add_argument("--force", default=str(DEFAULT_FORCE))
    parser.add_argument("--job", default=str(DEFAULT_JOB))
    parser.add_argument("--scenario", default=str(DEFAULT_SCENARIO))
    parser.add_argument("--out", default=str(DEFAULT_OUT))
    args = parser.parse_args()

    heroes = parse_heroes(read_text(Path(args.hero)))
    friend_groups = parse_friend_groups(read_text(Path(args.friend)))
    forces = parse_forces(read_text(Path(args.force)))
    jobs = parse_jobs(read_text(Path(args.job)))
    scenario_names = parse_scenarios(read_text(Path(args.scenario)))

    if not scenario_names:
        scenario_names = {1: "剧本1", 2: "剧本2", 3: "剧本3"}
    if not heroes:
        raise SystemExit("未解析到任何英雄配置，请检查 HeroConfig_s.cs 的构造函数格式。")

    force_map = {f["short"]: f for f in forces}
    job_map = {j["short"]: j for j in jobs}
    hero_by_id = {h["id"]: h for h in heroes}

    panels = {}
    panel_violations = {}
    for sid in sorted(scenario_names):
        block, total, violations = build_panel(
            sid, heroes, force_map, job_map, friend_groups, hero_by_id,
            active=(sid == min(scenario_names)),
        )
        panels[sid] = (block, total)
        panel_violations[sid] = violations

    html_text = build_html(
        scenario_names, panels, heroes, friend_groups,
        datetime.now().strftime("%Y-%m-%d %H:%M"),
    )

    out_path = Path(args.out)
    out_path.parent.mkdir(parents=True, exist_ok=True)
    out_path.write_text(html_text, encoding="utf-8")

    print(f"英雄 {len(heroes)} 位 / 好友羁绊组 {len(friend_groups)} 个 / 势力 {len(forces)} 个 / 职业 {len(jobs)} 个")
    for sid in sorted(scenario_names):
        print(f"  剧本{sid} {scenario_names[sid]}：{panels[sid][1]} 位英雄 · 不合规羁绊组 {panel_violations[sid]} 组")
    print(f"已生成：{out_path}")


if __name__ == "__main__":
    main()
