using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 战斗开始属性加成技能：BattleBegin 时解析本技能配置行的 LinkSelf（格式"attr+value,..."），
/// 对自身施加一次属性加成。解析与施加复用 JobLinkManager（与职业羁绊同源，避免双份逻辑），
/// 数值由配置表决定，无需逐属性写脚本。对应技能：敏锐(dodgeRate)/复原(hpRegen)/药仙(hpRegen) 等"开局加属性"类技能。
/// </summary>
public class SkillInitAttrChange : Skill
{
    public SkillInitAttrChange(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        foreach (var bonus in JobLinkManager.ParseBonuses(skillCfg.LinkSelf))
            JobLinkManager.ApplyAttr(owner, bonus.Attr, bonus.Value);
        owner.PlayerAnim(skillCfg.Action);
    }
}
