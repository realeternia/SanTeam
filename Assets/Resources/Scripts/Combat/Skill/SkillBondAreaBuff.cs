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
/// · noDedupe           = 不按首段 Buff 去重（司马炎 / 陆抗 / 张梁 / 周仓 / 娄圭 / 步骘 / 阮籍 / 甄宓）
/// · targetCenter       = 以选中的首个友军目标为中心，再在 Area 内扩散给周围友军（缺省以自身为中心）
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

        var selfAlways = HasSkillFlag("selfAlways");
        var noDedupe = HasSkillFlag("noDedupe");
        var searchRange = skillCfg.Area;

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
            foreach (var buffId in buffIds)
                ApplyBuffToUnit(unit, buffId);

            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        return true;
    }
}
