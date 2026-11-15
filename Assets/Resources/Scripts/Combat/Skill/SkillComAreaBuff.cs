﻿﻿﻿using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"群体增益"技能：给范围内友军附加 skillCfg.BuffId 对应的正面 buff。
/// 支持多段组合（如奇才 "攻,甲,抗"），去重判断只看第一段。
/// 选人规则由 SkillConfig.SelectOnly / SelectFirst 决定（见 TargetSelector）：
///   典型配置 SelectOnly="ally;hero;notSelf;notFullHp"、SelectFirst="lowestHp"。
/// SelectOnly 未配置阵营时默认取友方；TargetCount&lt;=0 时范围内全部。
/// 护盾类 Buff 自动按统一护盾公式写入护盾量。
///
/// TargetType 仅保留脚本私有行为标记（逗号分隔）：
/// · selfAlways         = 施法者必定获得（李典·御阵 / 周仓）
/// · noDedupe           = 不按首段 Buff 去重（甄宓 / 袁绍 / 周仓 / 李严 / 张梁 / 大乔）
/// · wholeMap           = 全图选人（李严·护军；Area 只作为嘲讽半径）
/// · targetCenter       = 以选中的首个友军目标为中心，再在 Area 内扩散给周围友军（缺省以自身为中心）
/// · tauntAroundTarget  = 给目标挂完 Buff 后，嘲讽其周围 Area 内敌人（李严·护军）
/// · splitByRange       = 按近战/远程分开发放：近战取 BuffId 第 1 段、远程取第 2 段（司马炎·一统 "甲,攻"）
/// </summary>
public class SkillAidAreaBuff : Skill
{
    public SkillAidAreaBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidAreaBuff: 未配置Buff短名 技能id={id}");
            return false;
        }

        var selfAlways = HasSkillFlag("selfAlways");
        var noDedupe = HasSkillFlag("noDedupe");
        var splitByRange = HasSkillFlag("splitByRange");
        var searchRange = HasSkillFlag("wholeMap") ? 0f : skillCfg.Area;

        // 中心点：缺省以施法者自身为中心；targetCenter 先选中首个友军目标，再以其位置为中心扩散
        var center = owner.transform.position;
        if (HasSkillFlag("targetCenter"))
        {
            var primary = SelectTargets(owner.transform.position, skillCfg.Range, true, 1,
                selfAlways ? owner : null,
                noDedupe ? (Func<Chess, bool>)null : x => !x.HasBuff(buffIds[0]));
            if (primary.Count == 0)
                return false;
            center = primary[0].transform.position;
        }

        // 选人：SelectOnly 硬过滤 + SelectFirst 优先级；去重（非 noDedupe）在截断目标数前生效
        var units = SelectTargets(center, searchRange, true, skillCfg.TargetCount,
            selfAlways ? owner : null,
            noDedupe ? (Func<Chess, bool>)null : x => !x.HasBuff(buffIds[0]));

        if (selfAlways)
            units.Add(owner);

        if (units.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(center);

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
