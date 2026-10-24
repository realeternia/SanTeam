using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"群体增益"技能：给范围内友军附加 skillCfg.BuffId 对应的正面 buff。
/// 支持多段组合（如奇才 "攻,甲,抗"），去重判断只看第一段。
/// TargetCount&gt;0 时取 N 名（默认随机，可按标记改为最低/最高生命），&lt;=0 时范围内全部。
/// 护盾类 Buff 自动按统一护盾公式写入护盾量。
///
/// TargetType 标记扩展（逗号分隔，空=默认随机 N 名、含士兵、首段去重）：
/// · heroOnly           = 只选英雄（刘备·仁德 / 甄宓·洛神 / 袁绍·名门 / 周仓·忠勇 / 张梁·黄天）
/// · excludeSelf        = 候选池排除施法者（甄宓 / 袁绍 / 周仓 / 张梁）
/// · selfAlways         = 施法者必定获得（李典·御阵 / 周仓）
/// · lowestHp           = 按生命比例升序取前 N（周仓·忠勇 / 李严·护军）
/// · highestHp          = 按生命比例降序取前 N（甄宓·洛神 / 大乔·国色 / 袁绍·名门）
/// · highestArmor       = 按护甲降序取前 N（张梁·黄天）
/// · rangedOnly         = 远程优先：远程单位排在前，无远程时回退到其余单位（attackRange >= CombatConst.MeleeRange，甄宓·洛神）
/// · meleeOnly          = 近战优先：近战单位排在前，无近战时回退到其余单位（attackRange < CombatConst.MeleeRange，大乔·国色）
/// · fullHpSkip         = 跳过满血友军（周仓 / 张梁）
/// · noDedupe           = 不按首段 Buff 去重（甄宓 / 袁绍 / 周仓 / 李严 / 张梁 / 大乔）
/// · crossSideFallback  = 同侧无目标时退到非敌对阵营（当前无技能使用）
/// · wholeMap           = 全图选人（李严·护军；Area 只作为嘲讽半径）
/// · tauntAroundTarget  = 给目标挂完 Buff 后，嘲讽其周围 Area 内敌人（李严·护军）
/// · splitByRange       = 按近战/远程分开发放：近战取 BuffId 第 1 段、远程取第 2 段（司马炎·一统 "甲,攻"）
/// </summary>
public class SkillAreaBuff : Skill
{
    public SkillAreaBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAreaBuff: 未配置Buff短名 技能id={id}");
            return false;
        }

        var heroOnly = HasSkillFlag("heroOnly");
        var excludeSelf = HasSkillFlag("excludeSelf");
        var noDedupe = HasSkillFlag("noDedupe");
        var lowestHp = HasSkillFlag("lowestHp");
        var highestHp = HasSkillFlag("highestHp");
        var highestArmor = HasSkillFlag("highestArmor");
        var preferRanged = HasSkillFlag("rangedOnly");
        var preferMelee = HasSkillFlag("meleeOnly");
        var fullHpSkip = HasSkillFlag("fullHpSkip");
        var selfAlways = HasSkillFlag("selfAlways");
        var crossSideFallback = HasSkillFlag("crossSideFallback");
        var searchRange = HasSkillFlag("wholeMap") ? 0f : skillCfg.Area;
        var splitByRange = HasSkillFlag("splitByRange");

        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, searchRange, owner.side)
            .FindAll(x => x.IsInFight()
                && (!heroOnly || x.isHero)
                && (!excludeSelf || x != owner)
                && (noDedupe || !x.HasBuff(buffIds[0]))
                && (!fullHpSkip || x.hp < x.maxHp));

        // 远程/近战优先：仅调整选取顺序，不做硬性剔除（无匹配单位时回退到其余单位）
        Func<Chess, int> preferRank = x =>
        {
            if (preferRanged)
                return x.attackRange >= CombatConst.MeleeRange ? 0 : 1;
            if (preferMelee)
                return x.attackRange < CombatConst.MeleeRange ? 0 : 1;
            return 0;
        };

        // 施法者必定获得：先从候选池移除，避免随机选人时占用名额
        if (selfAlways)
            units.Remove(owner);

        if (crossSideFallback && units.Count == 0)
        {
            units = WorldManager.Instance.GetUnitsInRangeAll(owner.transform.position, searchRange)
                .FindAll(x => x != owner && !WorldManager.Instance.IsEnemy(x.side, owner.side)
                    && x.isHero && x.IsInFight()
                    && (noDedupe || !x.HasBuff(buffIds[0]))
                    && (!fullHpSkip || x.hp < x.maxHp));
        }

        if (units.Count == 0 && !selfAlways)
            return false;

        // 目标数上限：>0 按规则取前 N；<=0 表示范围内全部
        if (skillCfg.TargetCount > 0)
        {
            if (lowestHp)
                units.Sort((a, b) => { var r = preferRank(a) - preferRank(b); return r != 0 ? r : a.HpRate.CompareTo(b.HpRate); });
            else if (highestHp)
                units.Sort((a, b) => { var r = preferRank(a) - preferRank(b); return r != 0 ? r : b.HpRate.CompareTo(a.HpRate); });
            else if (highestArmor)
                units.Sort((a, b) => { var r = preferRank(a) - preferRank(b); return r != 0 ? r : b.armor.CompareTo(a.armor); });

            if (lowestHp || highestHp || highestArmor)
            {
                if (units.Count > skillCfg.TargetCount)
                    units.RemoveRange(skillCfg.TargetCount, units.Count - skillCfg.TargetCount);
            }
            else
            {
                // 随机模式下同样优先：先从优先组抽满，不足再从未匹配单位补
                if (preferRanged || preferMelee)
                {
                    var preferred = units.FindAll(x => preferRank(x) == 0);
                    if (preferred.Count >= skillCfg.TargetCount)
                        units = preferred;
                    else
                    {
                        var rest = units.FindAll(x => preferRank(x) != 0);
                        WorldManager.Instance.RandomSelect(rest, skillCfg.TargetCount - preferred.Count);
                        preferred.AddRange(rest);
                        units = preferred;
                    }
                }
                WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);
            }
        }

        if (selfAlways)
            units.Add(owner);

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(owner.transform.position);

        foreach (var unit in units)
        {
            // splitByRange：近战只吃第 1 段、远程只吃第 2 段（如"甲,攻"），其余情况逐段施加
            if (splitByRange && buffIds.Count >= 2)
                ApplyBuff(unit, unit.attackRange < CombatConst.MeleeRange ? buffIds[0] : buffIds[1]);
            else
                foreach (var buffId in buffIds)
                    ApplyBuff(unit, buffId);

            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            if (HasSkillFlag("tauntAroundTarget"))
                TauntEnemies(owner, unit.transform.position, skillCfg.Area);
        }
        return true;
    }

    /// <summary>给单个单位施加一段 Buff；护盾类 Buff 按统一护盾公式写入护盾值（李严·护军的"盾"）</summary>
    private void ApplyBuff(Chess unit, int buffId)
    {
        BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
        var shield = unit.GetBuff(buffId) as BuffShield;
        if (shield != null)
            shield.AddHp(GetSkillShield());
    }
}
