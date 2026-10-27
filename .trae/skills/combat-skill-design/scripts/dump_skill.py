# -*- coding: utf-8 -*-
"""SkillConfig 配置自检：按 Sname 或 Id 打印技能全部等级行并检查常见配置错误。

用法（在仓库根目录执行）：
    python .trae/skills/combat-skill-design/scripts/dump_skill.py 烈火
    python .trae/skills/combat-skill-design/scripts/dump_skill.py 2020001
    python .trae/skills/combat-skill-design/scripts/dump_skill.py --list
"""
import io
import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..', '..', '..'))
CFG_DIR = os.path.join(ROOT, 'Assets', 'Resources', 'Scripts', 'Configs')
SKILL_FILE = os.path.join(CFG_DIR, 'SkillConfig_s.cs')
BUFF_FILE = os.path.join(CFG_DIR, 'BuffConfig_s.cs')
EFFECT_FILE = os.path.join(CFG_DIR, 'EffectConfig_s.cs')
SKILL_DIR = os.path.join(ROOT, 'Assets', 'Resources', 'Scripts', 'Combat', 'Skill')

SHOW_FIELDS = ['Id', 'Name', 'Sname', 'Type', 'Lv', 'Rate', 'CD', 'MpCost', 'StartMpRate',
               'TriggerCondition', 'DamageType', 'HurtTag', 'Range', 'Area', 'TargetType',
               'TargetCount', 'DamageStrength', 'HealStrength', 'Strength2', 'StrengthBuff1',
               'BuffId', 'BuffTime', 'SummonTag', 'SummonCount', 'SummonTime', 'ItemId',
               'ScriptName', 'SelfEffect', 'HitEffect', 'AreaEffect', 'LinkSelf', 'LinkTeam', 'AuroAttrs']


def read_text(path):
    with io.open(path, encoding='utf-8-sig') as f:
        return f.read()


def strip_comments(text):
    text = re.sub(r'//[^\n]*', '', text)
    return re.sub(r'/\*.*?\*/', '', text, flags=re.S)


def split_args(raw):
    args, cur, depth, in_str, i = [], [], 0, False, 0
    while i < len(raw):
        c = raw[i]
        if in_str:
            cur.append(c)
            if c == '\\':
                if i + 1 < len(raw):
                    cur.append(raw[i + 1])
                    i += 2
                    continue
            elif c == '"':
                in_str = False
            i += 1
            continue
        if c == '"':
            in_str = True
            cur.append(c)
        elif c in '([{':
            depth += 1
            cur.append(c)
        elif c in ')]}':
            depth -= 1
            cur.append(c)
        elif c == ',' and depth == 0:
            args.append(''.join(cur).strip())
            cur = []
        else:
            cur.append(c)
        i += 1
    tail = ''.join(cur).strip()
    if tail:
        args.append(tail)
    return args


def unquote(v):
    v = (v or '').strip()
    if len(v) >= 2 and v[0] == '"' and v[-1] == '"':
        return v[1:-1]
    return v


def to_float(v):
    """解析 C# 字面量（支持 5f / 0.5F），失败返回 None。"""
    try:
        return float(str(v).strip().rstrip('fF'))
    except (TypeError, ValueError):
        return None


def parse_rows(path, cls):
    """解析 new Class(...) 数据行，返回 (列名列表, 行字典列表)。"""
    text = strip_comments(read_text(path))
    m = re.search(r'public\s+' + cls + r'\s*\(([^)]*)\)', text)
    if not m:
        return [], []
    params = [p.strip().split()[-1] for p in m.group(1).split(',') if p.strip()]
    rows = []
    for mm in re.finditer(r'new\s+' + cls + r'\s*\(', text):
        start = mm.end()
        depth, in_str, i = 1, False, start
        while i < len(text) and depth > 0:
            c = text[i]
            if in_str:
                if c == '\\':
                    i += 2
                    continue
                if c == '"':
                    in_str = False
            else:
                if c == '"':
                    in_str = True
                elif c == '(':
                    depth += 1
                elif c == ')':
                    depth -= 1
            i += 1
        vals = split_args(text[start:i - 1])
        row = {}
        for idx, p in enumerate(params):
            row[p] = unquote(vals[idx]) if idx < len(vals) else ''
        rows.append(row)
    return params, rows


def main():
    if not os.path.exists(SKILL_FILE):
        print('找不到 SkillConfig_s.cs：' + SKILL_FILE)
        return 2

    params, skills = parse_rows(SKILL_FILE, 'SkillConfig')
    if not params:
        print('解析 SkillConfig 构造函数失败')
        return 2

    if len(sys.argv) < 2 or sys.argv[1] in ('-h', '--help'):
        print(__doc__)
        return 0

    arg = sys.argv[1]
    if arg == '--list':
        seen = []
        for r in skills:
            if r['Sname'] not in seen:
                seen.append(r['Sname'])
        print('共 %d 个技能（Sname）：' % len(seen))
        for s in seen:
            lv1 = next((x for x in skills if x['Sname'] == s and x['Lv'] == '1'), None)
            print('  %-8s %-14s %s' % (s, (lv1 or {}).get('Name', ''), (lv1 or {}).get('Id', '')))
        return 0

    if arg.isdigit():
        hit = [r for r in skills if r['Id'] == arg]
        rows = [r for r in skills if r['Sname'] == hit[0]['Sname']] if hit else []
    else:
        rows = [r for r in skills if r['Sname'] == arg]
        if not rows:
            rows = [r for r in skills if r['Name'] == arg]
    if not rows:
        print('未找到技能（支持 Sname / 技能名 / Id）：' + arg)
        return 1

    rows.sort(key=lambda r: int(r['Lv']) if r['Lv'].isdigit() else 0)
    fields = [f for f in SHOW_FIELDS if f in params]
    print('== %s / %s  共 %d 行 ==' % (rows[0]['Sname'], rows[0]['Name'], len(rows)))
    print(' | '.join(fields))
    for r in rows:
        print(' | '.join(r.get(f, '') for f in fields))
    print('')
    print('Descript(Lv1)：' + (next((r['Descript'] for r in rows if r['Lv'] == '1'), '')))
    print('')

    errors = []
    warnings = []

    # 1) 等级行完整性
    lvs = [r['Lv'] for r in rows]
    if len(rows) != 5 or lvs != ['1', '2', '3', '4', '5']:
        errors.append('等级行不完整：当前 Lv=%s（应为 1,2,3,4,5 五行）' % ','.join(lvs))

    # 2) Id 连号
    ids = [int(r['Id']) for r in rows if r['Id'].isdigit()]
    if ids and ids != list(range(ids[0], ids[0] + len(ids))):
        errors.append('同 Sname 的 Id 不连号：%s' % ids)

    # 3) 全表 Id 唯一
    dup = [i for i in ids if sum(1 for x in skills if x['Id'] == str(i)) > 1]
    if dup:
        errors.append('Id 在表中重复：%s' % sorted(set(dup)))

    # 4) 脚本存在
    scripts = sorted(set(r['ScriptName'] for r in rows if r['ScriptName']))
    for sc in scripts:
        if not os.path.exists(os.path.join(SKILL_DIR, sc + '.cs')):
            errors.append('脚本文件缺失：Combat/Skill/%s.cs' % sc)

    # 5) Buff 短名
    if os.path.exists(BUFF_FILE):
        _, buffs = parse_rows(BUFF_FILE, 'BuffConfig')
        known = set(b['NameS'] for b in buffs)
        for r in rows:
            for tok in re.split(r'[,，;；]', r['BuffId']):
                tok = tok.strip()
                if tok and tok not in known:
                    errors.append('BuffId 短名不存在：%s（技能 %s Lv%s）' % (tok, r['Sname'], r['Lv']))
        # BuffTime 合理性
        for r in rows:
            if r['BuffId'] and r['BuffTime']:
                bt = to_float(r['BuffTime'])
                if bt is None:
                    warnings.append('BuffTime 无法解析：%s' % r['BuffTime'])
                elif bt <= 0:
                    warnings.append('BuffId 有值但 BuffTime<=0（技能 %s Lv%s）' % (r['Sname'], r['Lv']))

    # 6) 特效名
    if os.path.exists(EFFECT_FILE):
        _, effects = parse_rows(EFFECT_FILE, 'EffectConfig')
        names = set(e['Name'] for e in effects)
        for r in rows:
            for f in ('SelfEffect', 'HitEffect', 'AreaEffect'):
                v = r.get(f, '')
                if v and v not in names:
                    errors.append('%s 特效名不存在：%s（技能 %s Lv%s）' % (f, v, r['Sname'], r['Lv']))

    # 7) Descript：Lv1 有、Lv2~5 空
    for r in rows:
        if r['Lv'] == '1' and not r['Descript']:
            warnings.append('Lv1 Descript 为空')
        if r['Lv'] != '1' and r['Descript']:
            warnings.append('Lv%s Descript 非空（应留空自动回退 Lv1 模板）' % r['Lv'])

    # 8) 范围技能 Area
    range_scripts = ('SkillAreaDamageDeBuff', 'SkillAreaHeal', 'SkillCastingAidScorchedEarth')
    for r in rows:
        av = to_float(r['Area'])
        if (av or 0) == 0 and r['ScriptName'] in range_scripts and 'wholeMap' not in r['TargetType']:
            warnings.append('范围技能 Area=0（全图），若非全图设计需修正（技能 %s Lv%s）' % (r['Sname'], r['Lv']))

    # 8.1) TargetType 标记合法性（仅校验已知脚本）
    known_flags = {
        'SkillAreaBuff': {'heroOnly', 'excludeSelf', 'selfAlways', 'lowestHp', 'highestHp', 'highestArmor',
                          'rangedOnly', 'meleeOnly', 'fullHpSkip', 'noDedupe', 'wholeMap', 'splitByRange',
                          'tauntAroundTarget', 'crossSideFallback'},
        'SkillAreaDamageDeBuff': {'self', 'missile', 'maxHpBonus', 'mpBurn'},
        'SkillAreaHeal': {'overflowToArmor'},
        'SkillSelfBuff': {'permanent', 'noStack'},
        'SkillBacklineStrike': {'smart', 'extra'},
        'SkillJumpChain': {'heal'},
        'SkillAttackedShield': {'ignoreWeakHit'},
    }
    for r in rows:
        sc = r['ScriptName']
        if sc not in known_flags:
            continue
        toks = [t.strip() for t in re.split(r'[,，;；|]', r['TargetType']) if t.strip()]
        unknown = [t for t in toks if t not in known_flags[sc]]
        if unknown:
            warnings.append('TargetType 含 %s 未支持的标记：%s（技能 %s Lv%s）' % (sc, ','.join(unknown), r['Sname'], r['Lv']))

    # 9) 英雄专属技能 Sname 归属
    hero_file = os.path.join(CFG_DIR, 'HeroConfig_s.cs')
    if os.path.exists(hero_file):
        _, heroes = parse_rows(hero_file, 'HeroConfig')
        hero_names = set(h['Name'] for h in heroes)
        hero_skills = set(h['Skill1'] for h in heroes if h['Skill1'])
        sn = rows[0]['Sname']
        if sn in hero_names:
            owners = [h['Name'] for h in heroes if h['Skill1'] == sn]
            if not owners:
                warnings.append('Sname 是英雄名但无英雄用 HeroConfig.Skill1 挂载：%s' % sn)
        elif sn in hero_skills:
            warnings.append('Sname=%s 被英雄挂载但 Sname≠英雄名，不符合"专属技能 Sname=英雄名"约定' % sn)

    # 10) TargetCount 合法性
    for r in rows:
        tc = r['TargetCount']
        if tc and not tc.lstrip('-').isdigit():
            warnings.append('TargetCount 非整数：%s' % tc)
        elif tc and int(tc) < 0:
            warnings.append('TargetCount 为负数（%s），应填 0 表示不限（技能 %s Lv%s）' % (tc, r['Sname'], r['Lv']))

    print('检查结果：')
    if not errors and not warnings:
        print('  通过，未发现问题。')
    for e in sorted(set(errors)):
        print('  [错误] ' + e)
    for w in sorted(set(warnings)):
        print('  [警告] ' + w)
    return 1 if errors else 0


if __name__ == '__main__':
    sys.exit(main())
