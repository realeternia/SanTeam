using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 貂蝉·离间：与生命比例最低的友军建立生命链接（双方各挂 BuffLifeLink），链接期间双方按 /strengthbuff1-1 比例共享伤害，
/// 并在链接建立时按统一治疗公式 GetSkillHeal() 立即治疗被链接的友军（吃医职业治疗加成）。
/// </summary>
public class SkillAidLifeLink : Skill
{
    public SkillAidLifeLink(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        // 目标：按 SelectOnly / SelectFirst 选（典型 ally;notSelf + lowestHp，全图范围）
        var picked = SelectTargets(owner.transform.position, 0f, true, 1, owner);
        var lowest = picked.Count > 0 ? picked[0] : null;
        if (lowest == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        int linkId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        // 双方各挂一份，张梁持链侧 BeforeCalDamaged 把伤害分享给自身
        BuffManager.AddBuff(lowest, owner, id, linkId, skillCfg.BuffTime);
        BuffManager.AddBuff(owner, owner, id, linkId, skillCfg.BuffTime);

        // 链接建立时立即治疗被链接的友军（走统一治疗公式，吃医职业治疗加成）
        var heal = GetSkillHeal();
        if (heal > 0)
            owner.HealTarget(lowest, skillId, heal, true);

        PlaySelfEffect();
        return true;
    }
}
