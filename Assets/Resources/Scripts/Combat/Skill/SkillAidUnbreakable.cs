using CommonConfig;
using UnityEngine;

/// <summary>
/// 周泰·不屈：为自身附加 /strength2-1% 最大生命的护盾，并回复 /strength2-2% 最大生命，肉盾续命。
/// </summary>
public class SkillAidUnbreakable : Skill
{
    public SkillAidUnbreakable(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        int shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var sh = owner.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp((int)(owner.maxHp * skillCfg.Strength2[0]));

        // 回复生命（护盾之外的实体回复）
        int heal = (int)(owner.maxHp * skillCfg.Strength2[1]);
        if (heal > 0)
            owner.HealTarget(owner, id, heal, true);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}