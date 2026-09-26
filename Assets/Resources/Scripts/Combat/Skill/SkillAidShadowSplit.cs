using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 关羽·分兵：对目标造成魔法伤害，并在身旁召唤一个继承自身攻击/生命（按强度2比例）的影分身
/// </summary>
public class SkillAidShadowSplit : Skill
{
    public SkillAidShadowSplit(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // 本体魔法伤害
        target.OnSkillDamaged(owner, id, GetSkillDamage());

        // 召唤影分身：攻击/生命继承本体 × strength2
        Vector2 randomDir = SysRandom.InsideUnitCircle.normalized;
        Vector3 randomPosition = owner.transform.position + new Vector3(randomDir.x, 0, randomDir.y) * skillCfg.Range;
        var shadow = SummonUnit(randomPosition, CombatConst.SoldierShadow, HeroConfig.GetConfig(owner.heroId).Icon);
        shadow.atk = (int)(owner.atk * skillCfg.Strength2);
        shadow.maxHp = (int)(owner.maxHp * skillCfg.Strength2);
        shadow.hp = shadow.maxHp;

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}