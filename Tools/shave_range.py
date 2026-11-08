# -*- coding: utf-8 -*-
"""
一次性刷表脚本：按新的"命中距离 = 射程 + 双方占位半径"口径，把射程回退到改动前水平。

- JobConfig.Range   -10（英雄半径 5+5=10，正好抵消）
- SkillConfig.Range -10（只处理 >0 行；=0 表示不限射程，保持不变）
- SoldierConfig.Range -6（小兵 EntitySize=5，半径被 DefaultUnitRadius 兜底到 3，双方半径和=6）

规则：只减正数，0 与非正数原样保留，避免出现"射程<=0 被逻辑当成不限射程/不索敌"的副作用。
"""
import os
import re

CONFIG_DIR = os.path.join(
    os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
    "Assets", "Resources", "Scripts", "Configs"
)

# 文件、构造器名、Range 参数下标、减去的值、数值格式（int / float-f）
TARGETS = [
    ("JobConfig_s.cs", "JobConfig", 8, 10, "int"),
    ("SoldierConfig_s.cs", "SoldierConfig", 12, 6, "int"),
    ("SkillConfig_s.cs", "SkillConfig", 14, 10, "float"),
]


def split_args(src, open_paren_idx):
    """从构造器左括号后开始，按顶层逗号切分参数，返回 [(start, end, text)]。"""
    i = open_paren_idx + 1
    depth = 1
    in_str = False
    esc = False
    tokens = []
    token_start = i
    while i < len(src):
        ch = src[i]
        if in_str:
            if esc:
                esc = False
            elif ch == "\\":
                esc = True
            elif ch == '"':
                in_str = False
        else:
            if ch == '"':
                in_str = True
            elif ch in "([{":
                depth += 1
            elif ch in ")]}":
                depth -= 1
                if depth == 0:
                    tokens.append((token_start, i, src[token_start:i].strip()))
                    return tokens
            elif ch == "," and depth == 1:
                tokens.append((token_start, i, src[token_start:i].strip()))
                token_start = i + 1
        i += 1
    raise ValueError("构造器参数未闭合")


def parse_number(text, is_float):
    t = text.strip()
    if is_float:
        t = t.rstrip("fF")
    return float(t)


def format_number(value, is_float):
    if is_float:
        return ("%d" % int(round(value))) + "f"
    return "%d" % int(round(value))


def process(path, ctor, idx, delta, kind):
    src = open(path, encoding="utf-8-sig").read()
    is_float = kind == "float"
    edits = []
    skipped_zero = 0
    changed = 0
    for m in re.finditer(r"new %s\(" % ctor, src):
        args = split_args(src, m.end() - 1)
        if len(args) <= idx:
            continue
        start, end, text = args[idx]
        value = parse_number(text, is_float)
        if value <= 0:
            skipped_zero += 1
            continue
        new_value = value - delta
        if new_value <= 0:
            raise ValueError("%s %s 第%d参数 原值=%s 减%d后 <=0，请人工确认" %
                             (os.path.basename(path), ctor, idx, text, delta))
        old_raw = src[start:end]
        # 保留原 token 两侧空白，仅替换数字本体
        leading = old_raw[:len(old_raw) - len(old_raw.lstrip())]
        trailing = old_raw[len(old_raw.rstrip()):]
        edits.append((start, end, leading + format_number(new_value, is_float) + trailing))
        changed += 1

    for start, end, new_text in sorted(edits, reverse=True):
        src = src[:start] + new_text + src[end:]

    with open(path, "w", encoding="utf-8-sig", newline="") as f:
        f.write(src)
    print("%s: 修改 %d 行，跳过非正值 %d 行" % (os.path.basename(path), changed, skipped_zero))


def main():
    for file_name, ctor, idx, delta, kind in TARGETS:
        process(os.path.join(CONFIG_DIR, file_name), ctor, idx, delta, kind)


if __name__ == "__main__":
    main()
