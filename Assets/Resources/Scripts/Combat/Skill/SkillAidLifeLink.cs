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

        // 找生命比例最低的友军
        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, 0f, owner.side);
        Chess lowest = null;
        foreach (var a in allies)
        {
            if (a == null || a.hp <= 0 || a == owner)
                continue;
            if (lowest == null || a.HpRate < lowest.HpRate)
                lowest = a;
        }
        if (lowest == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        int linkId = BuffConfig.GetConfigByNameS("链").Id;
        // 双方各挂一份，张梁持链侧 BeforeCalDamaged 把伤害分享给自身
        BuffManager.AddBuff(lowest, owner, id, linkId, skillCfg.BuffTime);
        BuffManager.AddBuff(owner, owner, id, linkId, skillCfg.BuffTime);

        // 链接建立时立即治疗被链接的友军（走统一治疗公式，吃医职业治疗加成）
        var heal = GetSkillHeal();
        if (heal > 0)
            owner.HealTarget(lowest, skillId, heal, true);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}