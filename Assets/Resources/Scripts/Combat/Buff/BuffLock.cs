public class BuffLock : Buff
{
    public BuffLock(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        var unitList = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range * 3, caster.side, true);
        UnityEngine.Debug.Log("Lock target count: " + unitList.Count);
        foreach (var unit in unitList)
        {
            if (unit.HasBuff(id) && unit != owner)
                unit.OnSkillDamaged(caster, skillCfg.Id, (int)(damage * skillCfg.SkillDamageRate));
        }

    }

}