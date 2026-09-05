#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Tools/sync_skins.py

按 HeroConfig_s.cs 的 Icon 字段（背景图），把 SanTeam 当前英雄需要的皮肤图标从 SanKingdom 项目
同步到 Assets/Resources/Skins 与 SkinsBig：
  - 目标目录中不属于 HeroConfig 引用的旧图标（含 .meta）会被删除；
  - HeroConfig 引用的图标会从 SanKingdom 的 Textures/Skins、SkinsBig 重新复制（内容一致则跳过）；
  - SanKingdom 中找不到的图标会汇总输出（含相近文件名提示），便于人工补图或核对命名。

用法：
  python Tools/sync_skins.py                # 实际执行
  python Tools/sync_skins.py --dry-run      # 只预览删除/复制，不落盘

默认路径（可用参数覆盖）：
  --hero-config        SanTeam 的 HeroConfig_s.cs
  --team-skins         SanTeam 目标 Skins
  --team-skins-big     SanTeam 目标 SkinsBig
  --kingdom-skins      SanKingdom 源 Skins
  --kingdom-skins-big  SanKingdom 源 SkinsBig
"""

from __future__ import annotations

import argparse
import difflib
import filecmp
import os
import re
import shutil
import sys

try:
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
except Exception:
    pass

IMAGE_EXTS = (".jpg", ".jpeg", ".png")
EXT_PRIORITY = {".jpg": 0, ".jpeg": 1, ".png": 2}


# ---------------------------------------------------------------- 路径默认值
def _abs(*parts):
    return os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), *parts))


DEFAULTS = {
    "hero_config": _abs("..", "Assets", "Resources", "Scripts", "Configs", "HeroConfig_s.cs"),
    "team_skins": _abs("..", "Assets", "Resources", "Skins"),
    "team_skins_big": _abs("..", "Assets", "Resources", "SkinsBig"),
    "kingdom_skins": r"D:\U3dPrj\SanKingdom\Assets\Resources\Textures\Skins",
    "kingdom_skins_big": r"D:\U3dPrj\SanKingdom\Assets\Resources\Textures\SkinsBig",
}


# ---------------------------------------------------------------- 解析 HeroConfig
def split_args(argstr: str):
    """按逗号切分参数，忽略引号内的逗号。"""
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
        if ch in ('"', "'"):
            in_quote = True
            quote_char = ch
            cur.append(ch)
            continue
        if ch == ",":
            args.append("".join(cur).strip())
            cur = []
            continue
        cur.append(ch)
    if cur:
        args.append("".join(cur).strip())
    return args


def unquote(s: str):
    s = s.strip()
    if len(s) >= 2 and s[0] == s[-1] and s[0] in ('"', "'"):
        return s[1:-1]
    return s


ROW_RE = re.compile(r"config\[\d+\]\s*=\s*new\s+HeroConfig\((.*?)\)\s*;", re.S)


def parse_hero_icons(path: str):
    """返回 { icon: [(heroId, heroName), ...] }，icon 为空的行忽略。"""
    with open(path, "r", encoding="utf-8") as f:
        text = f.read()
    result = {}
    for m in ROW_RE.finditer(text):
        args = split_args(m.group(1).strip())
        if len(args) < 26:
            continue
        icon = unquote(args[25])
        if not icon:
            continue
        hid = unquote(args[0])
        name = unquote(args[1])
        result.setdefault(icon, []).append((hid, name))
    return result


# ---------------------------------------------------------------- 目录索引 / 清理
def index_images(dirpath: str):
    """扫描源目录里的图片，返回 { stem: 文件名(优先 jpg) }。"""
    index = {}
    try:
        names = os.listdir(dirpath)
    except FileNotFoundError:
        return index
    for name in names:
        stem, ext = os.path.splitext(name)
        ext = ext.lower()
        if ext not in EXT_PRIORITY:
            continue
        if stem not in index or EXT_PRIORITY[ext] < EXT_PRIORITY[os.path.splitext(index[stem])[1].lower()]:
            index[stem] = name
    return index


def clean_dir(dirpath: str, needed: set):
    """删除目标目录中不在 needed 集合里的图片及其 .meta。返回删除的文件名列表。"""
    removed = []
    if not os.path.isdir(dirpath):
        return removed
    for name in os.listdir(dirpath):
        full = os.path.join(dirpath, name)
        if not os.path.isfile(full):
            continue
        if name.endswith(".meta"):
            base = name[:-5]
            stem, ext = os.path.splitext(base)
            if ext.lower() in EXT_PRIORITY and stem not in needed:
                os.remove(full)
                removed.append(name)
            continue
        stem, ext = os.path.splitext(name)
        if ext.lower() in EXT_PRIORITY and stem not in needed:
            os.remove(full)
            removed.append(name)
    return sorted(removed)


# ---------------------------------------------------------------- 复制
def sync_dir(src_dir: str, dst_dir: str, icon_list, needed_set: set):
    """
    将 icon_list 中的图标从 src_dir 复制到 dst_dir。
    返回 (copied, added, missing, skipped)，missing 为源目录缺少的 icon。
    """
    os.makedirs(dst_dir, exist_ok=True)
    index = index_images(src_dir)
    copied, added, missing, skipped = [], [], [], []
    for icon in icon_list:
        src_name = index.get(icon)
        if src_name is None:
            missing.append(icon)
            continue
        src_full = os.path.join(src_dir, src_name)
        dst_full = os.path.join(dst_dir, src_name)
        existed = os.path.exists(dst_full)
        if existed and filecmp.cmp(src_full, dst_full, shallow=False):
            skipped.append(icon)
            continue
        shutil.copy2(src_full, dst_full)
        copied.append(icon)
        if not existed:
            added.append(icon)
    return copied, added, missing, skipped


# ---------------------------------------------------------------- 主流程
def main():
    parser = argparse.ArgumentParser(description="同步 HeroConfig 需要的皮肤图标到 SanTeam")
    for key, value in DEFAULTS.items():
        parser.add_argument("--" + key.replace("_", "-"), default=value, help="默认: %s" % value)
    parser.add_argument("--dry-run", action="store_true", help="只预览不落盘")
    args = parser.parse_args()

    hero_config = os.path.abspath(args.hero_config)
    if not os.path.isfile(hero_config):
        sys.exit("找不到 HeroConfig 文件: %s" % hero_config)

    icons_by_hero = parse_hero_icons(hero_config)
    if not icons_by_hero:
        sys.exit("HeroConfig 中未解析到任何 Icon，已中止（防止误删）")
    icon_list = sorted(icons_by_hero.keys())
    needed_set = set(icon_list)
    print("HeroConfig 解析到 %d 个英雄、%d 个不同 Icon" % (sum(len(v) for v in icons_by_hero.values()), len(icon_list)))

    pair_cfgs = [
        ("Skins", os.path.abspath(args.team_skins), os.path.abspath(args.kingdom_skins)),
        ("SkinsBig", os.path.abspath(args.team_skins_big), os.path.abspath(args.kingdom_skins_big)),
    ]

    overall_missing = {}   # icon -> [(来源目录说明)]
    for label, team_dir, kingdom_dir in pair_cfgs:
        src_index = index_images(kingdom_dir)
        if not os.path.isdir(kingdom_dir):
            sys.exit("SanKingdom 源目录不存在: %s" % kingdom_dir)

        if args.dry_run:
            stale = [n for n in os.listdir(team_dir) if os.path.isfile(os.path.join(team_dir, n))]
            to_delete = []
            for name in stale:
                if name.endswith(".meta"):
                    stem, ext = os.path.splitext(name[:-5])
                else:
                    stem, ext = os.path.splitext(name)
                if ext.lower() in EXT_PRIORITY and stem not in needed_set:
                    to_delete.append(name)
            to_copy = [i for i in icon_list if i in src_index]
            missing = [i for i in icon_list if i not in src_index]
            print("[%s] 预览: 将删除 %d 个旧文件, 将复制/覆盖 %d 个, 源缺少 %d 个"
                  % (label, len(to_delete), len(to_copy), len(missing)))
            for icon in missing:
                overall_missing.setdefault(icon, []).append(label)
            continue

        removed = clean_dir(team_dir, needed_set)
        copied, added, missing, skipped = sync_dir(kingdom_dir, team_dir, icon_list, needed_set)
        print("[%s] 删除旧图标 %d 个; 新复制 %d 个(其中新增 %d); 内容相同跳过 %d 个; 源缺少 %d 个"
              % (label, len(removed), len(copied), len(added), len(skipped), len(missing)))
        if removed:
            print("    删除示例: %s" % ", ".join(removed[:10]) + (" ..." if len(removed) > 10 else ""))
        for icon in missing:
            overall_missing.setdefault(icon, []).append(label)

    if not args.dry_run:
        team_skins_abs = os.path.abspath(args.team_skins)
        for icon in icon_list:
            if icon not in overall_missing:
                continue
            stale_in_target = any(os.path.exists(os.path.join(d, n))
                                  for d in (team_skins_abs,) for n in os.listdir(d) if os.path.splitext(n)[0] == icon)
            print("缺失: Icon=%s 英雄=[%s] %s" % (
                icon,
                "; ".join("%s(%s)" % (n, hid) for hid, n in icons_by_hero[icon]),
                "(目标目录存在旧图，已保留；源缺失无法刷新)" if stale_in_target else "(目标目录也没有，运行时会缺图)",
            ))

    # 汇总：给源目录缺失的 icon 提供相近文件名提示
    if overall_missing:
        print("\n===== SanKingdom 源缺失汇总（共 %d 个 icon）=====" % len(overall_missing))
        src_all_stems = sorted(set(index_images(os.path.abspath(args.kingdom_skins))) | set(index_images(os.path.abspath(args.kingdom_skins_big))))
        for icon in icon_list:
            if icon not in overall_missing:
                continue
            hints = difflib.get_close_matches(icon, src_all_stems, n=3, cutoff=0.5)
            hint_txt = ("相近文件: " + ", ".join(hints)) if hints else "无相近文件名"
            print("%s  英雄=[%s]  缺失于[%s]  %s" % (
                icon,
                "; ".join("%s(%s)" % (n, hid) for hid, n in icons_by_hero[icon]),
                "/".join(overall_missing[icon]),
                hint_txt,
            ))

    if args.dry_run:
        print("\n[预览模式] 未做任何修改，确认后去掉 --dry-run 执行")


if __name__ == "__main__":
    main()
