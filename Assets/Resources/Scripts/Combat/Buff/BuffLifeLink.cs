/// <summary>
/// 生命链接（BuffLifeLink）：张梁与生命最低的友军建立链接，双方按 skillCfg.StrengthBuff1[0] 比例共享受到的伤害。
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
        // 派生伤害（链传/破盾等）不再二次扩散，避免与其它派生伤害互相触发形成死循环
        if (CombatConst.IsDerivedHurtTag(hurtTag) || caster == null || caster == owner || caster.hp <= 0)
            return;

        var shareDamage = (int)(damageBase * damageMulti * skillCfg.StrengthBuff1[0]);
        if (shareDamage <= 0)
            return;

        caster.OnSkillDamaged(owner, skillCfg.Id, shareDamage, false, CombatConst.LockChainHurtTag);
    }
}