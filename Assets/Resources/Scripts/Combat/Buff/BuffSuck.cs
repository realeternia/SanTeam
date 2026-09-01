public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        GameLog.Debug("Suck " + damage.ToString());
        owner.AddHp((int)(damage * skillCfg.SkillDamageRate * effectMulti));
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}