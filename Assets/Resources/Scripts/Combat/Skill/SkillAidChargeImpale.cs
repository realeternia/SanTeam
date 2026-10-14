using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙策·破阵冲阵：沿正前方直线（Range 距离、Area 宽度走廊）对路径上敌人各造成 /damagestrength 法术伤害。
/// 采用方向投影 + 垂直距离判定；每条走廊内每个敌人只结算一次（遍历一次天然去重，不重复命中）。
/// </summary>
public class SkillAidChargeImpale : Skill
{
    public SkillAidChargeImpale(int id, Chess unit) : base(id, unit)
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

        owner.PlayerAnim(skillCfg.Action);
        PlayAreaEffect(owner.transform.position);

        var dir = target.transform.position - owner.transform.position;
        dir.y = 0;
        if (dir.magnitude < 0.001f)
            return true;
        var fwd = dir.normalized;
        var halfWidth = skillCfg.Area * 0.5f;
        var skillDamage = GetSkillDamage();

        // 保持 Range：本技能 Range=直线长度、Area=走廊宽度，此处按直线长度取候选（后续用 Area 作垂直半宽判定），改 Area 会导致技能几乎打不到人
        var units = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side);
        foreach (var u in units)
        {
            if (u == owner || u.hp <= 0)
                continue;
            var vec = u.transform.position - owner.transform.position;
            vec.y = 0;
            var proj = Vector3.Dot(vec, fwd);
            if (proj < 0f || proj > skillCfg.Range)
                continue;
            var perp = (vec - fwd * proj).magnitude;
            if (perp > halfWidth)
                continue;
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
        }

        return true;
    }
}