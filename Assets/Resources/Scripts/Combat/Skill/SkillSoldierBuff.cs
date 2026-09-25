using System.Linq;
using CommonConfig;

/// <summary>
/// 马良·励军：每次主动释放，本侧所有士兵攻击+X%(Strength)、护甲+Y(StrengthInt)，永久可叠加。
/// </summary>
public class SkillSoldierBuff : Skill
{
    public SkillSoldierBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero))
        {
            s.atk += (int)(s.atk * skillCfg.Strength);
            s.armor += skillCfg.StrengthInt;
        }
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}