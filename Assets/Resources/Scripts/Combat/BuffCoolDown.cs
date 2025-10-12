using System;

public class BuffCoolDown : Buff
{
    public BuffCoolDown(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        owner.Cooldown(2 * skillCfg.Strength);
    }
}