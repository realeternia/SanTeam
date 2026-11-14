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
            // 额外箭矢目标：按父技能 SelectOnly / SelectFirst 选（默认敌方，随机取 extra 名，排除普攻目标）
            var enemies = TargetSelector.Select(owner, defender.transform.position, skillCfg.Area, false,
                skillCfg.SelectOnly, skillCfg.SelectFirst, extra, defender);
            foreach (var u in enemies)
            {
                WorldManager.Instance.CreateSpellMissile(owner, u, owner.transform.position, skillCfg.Id, damage, owner.hitEffect);
            }
        }
    }
}