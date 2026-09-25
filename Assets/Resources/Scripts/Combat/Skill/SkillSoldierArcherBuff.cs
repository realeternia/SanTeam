using System.Linq;
using CommonConfig;

/// <summary>
/// 沮授·强弓：每次主动释放，本侧弓兵攻击+X%(Strength)、攻速+Y%(Strength2)，并强化自身攻击与攻速，永久可叠加。
/// </summary>
public class SkillSoldierArcherBuff : Skill
{
    public SkillSoldierArcherBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        var atkPct = skillCfg.Strength;
        var spdPct = skillCfg.Strength2;
        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero && x.attackRange >= CombatConst.MeleeRange))
        {
            s.atk += (int)(s.atk * atkPct);
            s.attackSpeedRate += spdPct;
        }
        owner.atk += (int)(owner.atk * atkPct);
        owner.attackSpeedRate += spdPct;
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}