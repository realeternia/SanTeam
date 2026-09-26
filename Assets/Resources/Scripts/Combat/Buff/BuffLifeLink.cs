/// <summary>
/// 生命链接（BuffLifeLink）：张梁与生命最低的友军建立链接，双方按 skillCfg.Strength2 比例共享受到的伤害。
/// 挂在受击侧 BeforeCalDamaged：链接方持链时，把一部分伤害转移给张梁承担。
/// </summary>
public class BuffLifeLink : Buff
{
    public BuffLifeLink(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        // 防止链传伤害二次扩散
        if (hurtTag == CombatConst.LockChainHurtTag || caster == null || caster == owner || caster.hp <= 0)
            return;

        var shareDamage = (int)(damageBase * damageMulti * skillCfg.Strength2);
        if (shareDamage <= 0)
            return;

        caster.OnSkillDamaged(owner, skillCfg.Id, shareDamage, false, CombatConst.LockChainHurtTag);
    }
}