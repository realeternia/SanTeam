﻿﻿﻿using System.Collections.Generic;
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
/// 当前使用者：奇才（2010141~2010145）。
/// </summary>
public class SkillBondAreaBuff : Skill
{
    public SkillBondAreaBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillBondAreaBuff: 未配置Buff短名 技能id={id}");
            return false;
        }

        var searchRange = skillCfg.Area;

        // 选人：SelectOnly 硬过滤 + SelectFirst 优先级；去重（已带首段 Buff 的跳过）在截断目标数前生效
        var units = SelectTargets(owner.transform.position, searchRange, true, skillCfg.TargetCount,
            null, x => !x.HasBuff(buffIds[0]));

        if (units.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(owner.transform.position);

        foreach (var unit in units)
        {
            foreach (var buffId in buffIds)
                ApplyBuffToUnit(unit, buffId);

            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        return true;
    }
}
