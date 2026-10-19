using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 周瑜·火墙（ScriptName = "SkillAidFireWall"）：主动辅助技能——
/// 以当前目标为基准点，在目标位置及 owner→目标方向左右各 90° 处召唤火墙
/// （数量由 SummonCount 决定：1=仅中心，&gt;1=左右各 10，&gt;3=左右各 20），
/// 每个火墙每 SummonHitInterval 秒对周围 Area*1.5 内敌人结算一次技能伤害，持续 SummonTime 秒。
/// 非持续施法：放下火墙后施法者可正常移动/普攻。
/// </summary>
public class SkillAidFireWall : Skill
{
    private List<Vector3> targetPosList;

    public SkillAidFireWall(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        // Range<=0 视为不限射程（与配置表一致）
        if (skillCfg.Range > 0f && !WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 主目标额外陷入「陷阵」Buff（无法移动），Buff 与时长由 SkillConfig.BuffId/BuffTime 配置
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);

        var targetPos = target.transform.position;
        SummonMagicField(targetPos, out var summonTime);

        // owner→target 方向；两者重合时退化为施法者朝向
        var direction = targetPos - owner.transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
            direction = owner.transform.forward;
        direction.Normalize();

        var rightDirection = Quaternion.Euler(0, 90, 0) * direction;
        var leftDirection = Quaternion.Euler(0, -90, 0) * direction;

        targetPosList = new List<Vector3> { targetPos };
        if (skillCfg.SummonCount > 1)
        {
            targetPosList.Add(targetPos + leftDirection * 10);
            targetPosList.Add(targetPos + rightDirection * 10);
        }
        if (skillCfg.SummonCount > 3)
        {
            targetPosList.Add(targetPos + rightDirection * 20);
            targetPosList.Add(targetPos + leftDirection * 20);
        }

        foreach (var pos in targetPosList)
            PlayAreaEffect(pos, summonTime);

        owner.StartCoroutine(DelayDamage(summonTime));
        return true;
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            if (owner == null || owner.hp <= 0)
                yield break;

            var unitList = new List<Chess>();
            foreach (var pos in targetPosList)
            {
                var unitsInRange = WorldManager.Instance.GetEnemyInRange(pos, skillCfg.Area * 1.5f, owner.side);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);

                foreach (var unit in unitsInRange)
                {
                    if (unitList.Contains(unit))
                        continue;
                    unitList.Add(unit);
                }
            }
            var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
            foreach (var unit in unitList)
            {
                if (damage > 0)
                {
                    unit.OnSkillDamaged(owner, skillId, damage);
                    if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                        EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
                }
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }
}
