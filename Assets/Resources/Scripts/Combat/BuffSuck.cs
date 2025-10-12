public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        UnityEngine.Debug.Log("Suck " + damage.ToString());
        owner.AddHp((int)(damage * skillCfg.SkillDamageRate));
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}