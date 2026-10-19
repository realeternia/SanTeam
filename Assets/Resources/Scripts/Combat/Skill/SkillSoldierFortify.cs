using CommonConfig;
using UnityEngine;

/// <summary>
/// 张昭·筑垒（ScriptName = "SkillSoldierFortify"）：
/// 将一名敌方英雄强制拖回我方出生点（owner.spawnPosition），造成一次法术伤害，
/// 并挂「陷阵」(CombatConst.BuffNoMoveId) skillCfg.BuffTime 秒定身，防止其立刻走回。
/// </summary>
public class SkillSoldierFortify : Skill
{
    public SkillSoldierFortify(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var target = FindEnemyHero();
        if (target == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 拖回我方出生点
        target.MoveTo(owner.spawnPosition, true);

        // 单体法术伤害（走统一技能伤害结算）
        var damage = GetSkillDamage();
        if (damage > 0 && target.hp > 0)
            target.OnSkillDamaged(owner, skillId, damage);

        // 定身：挂「陷阵」，防止目标立刻走回
        if (target.hp > 0)
        {
            var buffIds = GetSkillBuffIds();
            var noMoveId = buffIds.Count > 0 ? buffIds[0] : CombatConst.BuffNoMoveId;
            BuffManager.AddBuff(target, owner, id, noMoveId, skillCfg.BuffTime);
        }

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        GameLog.Info($"筑垒：{owner.name} 将敌方英雄拖回我方出生点并造成伤害 技能id={id}");
        return true;
    }

    // 优先当前索敌目标（敌方英雄且在 Area 内），否则取 Area 内最近的敌方英雄
    private Chess FindEnemyHero()
    {
        var cur = owner.targetChess;
        if (cur != null && cur.hp > 0 && cur.isHero && WorldManager.Instance.IsEnemy(cur.side, owner.side)
            && WorldManager.Instance.CheckInRange(owner.transform.position, cur.transform.position, skillCfg.Area))
            return cur;

        Chess nearest = null;
        var minDist = float.MaxValue;
        foreach (var e in WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side))
        {
            if (!e.isHero || e.hp <= 0)
                continue;
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
