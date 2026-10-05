using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹洪·血战：单体法术伤害，自身生命越低伤害越高（额外伤害 = 已损生命比例 × Strength2[0] 倍基础伤害）。
/// </summary>
public class SkillAidBloodyWar : Skill
{
    public SkillAidBloodyWar(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        int damage = GetSkillDamage();
        // 自身已损生命比例越高伤害越高
        float lossRate = 1f - owner.HpRate;
        damage += (int)(damage * skillCfg.Strength2[0] * lossRate);

        target.OnSkillDamaged(owner, skillId, damage);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}