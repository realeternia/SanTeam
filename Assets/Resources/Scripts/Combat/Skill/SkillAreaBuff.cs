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
/// · rangedOnly         = 只选远程单位（attackRange >= CombatConst.MeleeRange，甄宓·洛神）
/// · meleeOnly          = 只选近战单位（attackRange < CombatConst.MeleeRange，大乔·国色）
/// · fullHpSkip         = 跳过满血友军（周仓 / 张梁）
/// · noDedupe           = 不按首段 Buff 去重（甄宓 / 袁绍 / 周仓 / 李严 / 张梁 / 大乔）
/// · crossSideFallback  = 同侧无目标时退到非敌对阵营（当前无技能使用）
/// · wholeMap           = 全图选人（李严·护军；Area 只作为嘲讽半径）
/// · tauntAroundTarget  = 给目标挂完 Buff 后，嘲讽其周围 Area 内敌人（李严·护军）
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
        var rangedOnly = HasSkillFlag("rangedOnly");
        var meleeOnly = HasSkillFlag("meleeOnly");
        var fullHpSkip = HasSkillFlag("fullHpSkip");
        var selfAlways = HasSkillFlag("selfAlways");
        var crossSideFallback = HasSkillFlag("crossSideFallback");
        var searchRange = HasSkillFlag("wholeMap") ? 0f : skillCfg.Area;

        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, searchRange, owner.side)
            .FindAll(x => x.IsInFight()
                && (!heroOnly || x.isHero)
                && (!excludeSelf || x != owner)
                && (!rangedOnly || x.attackRange >= CombatConst.MeleeRange)
                && (!meleeOnly || x.attackRange < CombatConst.MeleeRange)
                && (noDedupe || !x.HasBuff(buffIds[0]))
                && (!fullHpSkip || x.hp < x.maxHp));

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
                units.Sort((a, b) => a.HpRate.CompareTo(b.HpRate));
            else if (highestHp)
                units.Sort((a, b) => b.HpRate.CompareTo(a.HpRate));
            else if (highestArmor)
                units.Sort((a, b) => b.armor.CompareTo(a.armor));

            if (lowestHp || highestHp || highestArmor)
            {
                if (units.Count > skillCfg.TargetCount)
                    units.RemoveRange(skillCfg.TargetCount, units.Count - skillCfg.TargetCount);
            }
            else
                WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);
        }

        if (selfAlways)
            units.Add(owner);

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(owner.transform.position);

        foreach (var unit in units)
        {
            foreach (var buffId in buffIds)
            {
                BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
                // 护盾类 buff 需按统一护盾公式写入护盾值（李严·护军的"盾"）
                var shield = unit.GetBuff(buffId) as BuffShield;
                if (shield != null)
                    shield.SetHp(GetSkillShield(0));
            }
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            if (HasSkillFlag("tauntAroundTarget"))
                TauntEnemies(owner, unit.transform.position, skillCfg.Area);
        }
        return true;
    }
}
