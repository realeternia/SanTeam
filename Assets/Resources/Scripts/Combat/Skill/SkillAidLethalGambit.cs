using CommonConfig;
using UnityEngine;

/// <summary>
/// 钟会·敛翼（术）：对目标造成 /strength 法术伤害；目标生命低于 /strengthint% 时，
/// 额外追加其已损生命 /strength2% 的斩杀伤害。
/// </summary>
public class SkillAidLethalGambit : Skill
{
    public SkillAidLethalGambit(int id, Chess unit) : base(id, unit)
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

        if (GetSkillDamage() > 0)
            target.OnSkillDamaged(owner, skillId, GetSkillDamage());
        if (target.hp > 0 && (int)(target.HpRate * 100f) < skillCfg.StrengthInt)
        {
            var strike = (int)((target.maxHp - target.hp) * skillCfg.Strength3);
            if (strike > 0)
                target.OnSkillDamaged(owner, skillId, strike);
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}