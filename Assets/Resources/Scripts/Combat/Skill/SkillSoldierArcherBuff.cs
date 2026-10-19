using CommonConfig;
using UnityEngine;

/// <summary>
/// 沮授·强弓（ScriptName = "SkillSoldierArcherBuff"）：
/// 对一名敌方单位施法，本侧每名远程士兵（attackRange &gt;= CombatConst.MeleeRange）与沮授自身
/// 各射出一枚法术箭飞向目标，每枚按 GetSkillDamage() 结算；远程兵越多总伤害越高。
/// </summary>
public class SkillSoldierArcherBuff : Skill
{
    public SkillSoldierArcherBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var target = FindTarget();
        if (target == null)
            return false;

        var damage = GetSkillDamage();
        if (damage <= 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 每个远程士兵各射一箭
        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (s.isHero || s.hp <= 0 || s.attackRange < CombatConst.MeleeRange)
                continue;
            WorldManager.Instance.CreateSpellMissile(owner, target, s.transform.position, id, damage, skillCfg.HitEffect);
        }
        // 沮授自身再射一箭
        WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, damage, skillCfg.HitEffect);

        return true;
    }

    // 优先当前索敌目标（敌方且在 Area 内），否则取 Area 内最近的敌方单位
    private Chess FindTarget()
    {
        var cur = owner.targetChess;
        if (cur != null && cur.hp > 0 && WorldManager.Instance.IsEnemy(cur.side, owner.side)
            && WorldManager.Instance.CheckInRange(owner.transform.position, cur.transform.position, skillCfg.Area))
            return cur;

        Chess nearest = null;
        var minDist = float.MaxValue;
        foreach (var e in WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side))
        {
            var d = Vector3.Distance(owner.transform.position, e.transform.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = e;
            }
        }
        return nearest;
    }
}
