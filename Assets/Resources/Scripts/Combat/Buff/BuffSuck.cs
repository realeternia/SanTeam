public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        var rate = GetStrBuff("suck");
        GameLog.Debug("Suck " + damage.ToString());
        owner.HealTarget(owner, skillCfg.Id, (int)(damage * rate), false); // 吸血不算治疗，不吃治疗加成
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}
