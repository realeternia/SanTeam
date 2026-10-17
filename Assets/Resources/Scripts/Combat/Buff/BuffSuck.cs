public class BuffSuck : Buff
{
    public BuffSuck(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        // 吸血比例槽位约定：单独使用（如 300003 吸血）取 StrengthBuff1[0]；
        // 与其它数值 Buff 组合时（如徐晃"重,吸"：StrengthBuff1=[增伤, 吸血]）取 [1]，避免两者抢同一槽位。
        var slot = skillCfg.StrengthBuff1.Length > 1 ? 1 : 0;
        var rate = skillCfg.StrengthBuff1.Length > slot ? skillCfg.StrengthBuff1[slot] : 0f;
        GameLog.Debug("Suck " + damage.ToString());
        owner.HealTarget(owner, skillCfg.Id, (int)(damage * rate), false); // 吸血不算治疗，不吃治疗加成
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}
