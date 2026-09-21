public class BuffLock : Buff
{
    public BuffLock(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    // 连锁挂在伤害计算阶段（原挂在普攻专属的 OnAttacked 上，技能伤害不会触发）；普攻与技能伤害统一生效；
    // 带 LockChain 标签的链传伤害不再二次扩散，避免锁链循环放大
    public override void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)
    {
        if (hurtTag == CombatConst.LockChainHurtTag)
            return;

        var chainDamage = (int)(damageBase * damageMulti * skillCfg.SkillDamageRate);
        if (chainDamage <= 0)
            return; // 伤害被减伤压到0，不再链传，避免 OnSkillDamaged 的伤害<=0 异常

        var unitList = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, caster.side, true);
        GameLog.Debug("连锁目标数量: " + unitList.Count);
        foreach (var unit in unitList)
        {
            if (unit.HasBuff(id) && unit != owner)
                unit.OnSkillDamaged(caster, skillCfg.Id, chainDamage, false, CombatConst.LockChainHurtTag);
        }
    }
}