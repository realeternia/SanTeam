public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        GameLog.Debug("Suck " + damage.ToString());
        owner.HealTarget(owner, skillCfg.Id, (int)(damage * skillCfg.Strength), false); // 吸血不算治疗，不吃治疗加成
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}