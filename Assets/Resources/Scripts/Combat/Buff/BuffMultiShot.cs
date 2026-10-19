/// <summary>
/// 多重箭 Buff（短名"箭"）：普攻命中后按 StrengthBuff1 语义 key "extraArrows" 判定额外发射数量，在攻击范围（Area）内随机选取
/// 额外目标并对其发射同等伤害的物理导弹（CreateSpellMissile），实现一箭多目标的散射效果。
/// </summary>
public class BuffMultiShot : Buff
{
    public BuffMultiShot(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        int extra = (int)GetStrBuff("extraArrows");
        if (extra > 0)
        {
            var enemies = WorldManager.Instance.GetEnemyInRange(defender.transform.position, skillCfg.Area, owner.side);
            enemies.Remove(defender);
            WorldManager.Instance.RandomSelect(enemies, extra);
            foreach (var u in enemies)
            {
                WorldManager.Instance.CreateSpellMissile(owner, u, owner.transform.position, skillCfg.Id, damage, owner.hitEffect);
            }
        }
    }
}