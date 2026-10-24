using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙权·制衡（佯攻）：复制我方一名武将的幻影。
/// 选人优先级：距最近敌人最近（即站位最靠前的前排）优先，距离相同取生命上限更高者；
/// 幻影在施法者朝最近敌人的方向、距离 Area 处生成（佯攻站位，踩墙则回退到施法者位置），
/// 攻击/生命按 Strength2 的 "inheritRate" 继承被复制武将。
/// </summary>
public class SkillAidFeint : Skill
{
    public SkillAidFeint(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var copied = PickFrontlineHero();
        if (copied == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var shadow = SummonUnit(SummonPosition(copied), CombatConst.SoldierShadow, HeroConfig.GetConfig(copied.heroId).Icon);
        var rate = GetStr2("inheritRate");
        shadow.atk = (int)(copied.atk * rate);
        shadow.maxHp = (int)(copied.maxHp * rate);
        shadow.hp = shadow.maxHp;

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(shadow, skillCfg.HitEffect);
        GameLog.Info("制衡：" + owner.heroId + " 复制了英雄幻影 heroId=" + copied.heroId);
        return true;
    }

    /// <summary>选前排高血量武将：距最近敌人更近者优先，同距离取生命上限更高者</summary>
    private Chess PickFrontlineHero()
    {
        Chess best = null;
        var bestDist = float.MaxValue;
        foreach (var ally in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (ally == null || ally == owner || !ally.isHero || !ally.IsInFight())
                continue;
            var nearest = NearestEnemy(ally);
            var dist = nearest == null
                ? float.MaxValue
                : WorldManager.Instance.GetRange(ally.transform.position, nearest.transform.position);
            if (best == null || dist < bestDist || (Mathf.Approximately(dist, bestDist) && ally.maxHp > best.maxHp))
            {
                best = ally;
                bestDist = dist;
            }
        }
        return best;
    }

    /// <summary>
    /// 幻影生成点：被复制武将朝其最近敌人方向外推 Area 距离处（顶到前排做佯攻）；
    /// 无敌人或踩墙时退回被复制武将位置。
    /// </summary>
    private Vector3 SummonPosition(Chess copied)
    {
        var origin = copied.transform.position;
        var dir = Vector3.forward;
        var nearest = NearestEnemy(copied);
        if (nearest != null)
            dir = nearest.transform.position - origin;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = copied.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        var pos = origin + dir * skillCfg.Area;
        pos.y = origin.y;
        if (WorldManager.Instance.CheckPositionBlocked(owner, pos))
            pos = origin;
        return pos;
    }

    /// <summary>取单位最近的存活敌人（无则返回 null）</summary>
    private Chess NearestEnemy(Chess unit)
    {
        Chess best = null;
        var bestDist = float.MaxValue;
        foreach (var enemy in WorldManager.Instance.GetEnemyInRange(unit.transform.position, 0f, unit.side))
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            var dist = WorldManager.Instance.GetRange(unit.transform.position, enemy.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = enemy;
            }
        }
        return best;
    }
}
