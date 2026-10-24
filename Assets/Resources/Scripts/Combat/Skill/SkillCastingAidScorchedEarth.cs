using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陆逊·火海（术）：在身前扇形区域生成多片火持续灼烧。
/// 以施法者为顶点、朝目标方向左右各 30° 开角、Range 为半径的扇形内，
/// 沿 Range 的 1/5、2/5、3/5、4/5 距离处各随机取一个开角生成一片火（共 4 片），
/// 每片火召唤法术场并播放火特效，周期性(skillCfg.SummonHitInterval)对落点周围敌人持续造成伤害。
/// </summary>
public class SkillCastingAidScorchedEarth : Skill
{
    /// <summary>扇形左右开角（度）</summary>
    private const float HalfAngle = 30f;
    /// <summary>沿射程方向生成的火焰片数</summary>
    private const int FireCount = 4;

    private List<Vector3> targetPosList;

    public SkillCastingAidScorchedEarth(int id, Chess unit) : base(id, unit)
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
        PlaySelfEffect();

        // 主目标额外陷入「陷阵」Buff（无法移动），Buff 与时长由 SkillConfig.BuffId/BuffTime 配置
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);

        // 前方扇形：以自身为顶点、朝目标方向，沿 Range 的 1/(FireCount+1) ~ FireCount/(FireCount+1) 处
        // 各在左右 HalfAngle 度内随机取一个开角生成一片火
        var origin = owner.transform.position;
        var dir = target.transform.position - origin;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        targetPosList = new List<Vector3>();
        for (int i = 1; i <= FireCount; i++)
        {
            var dist = skillCfg.Range * i / (FireCount + 1f);
            var angle = (SysRandom.Value * 2f - 1f) * HalfAngle;
            var pos = origin + Quaternion.Euler(0f, angle, 0f) * dir * dist;
            pos.y = origin.y;
            targetPosList.Add(pos);
        }

        foreach (var pos in targetPosList)
        {
            SummonMagicField(pos, out var summonTime);
            PlayAreaEffect(pos, summonTime);
        }

        owner.StartCasting(id, DelayDamage(GetSummonTime()));
        return true;
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            // 被打断/死亡：castingSkillId 被 BreakCasting 置0，结束法阵
            if (owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;

            // 引导期间每跳持续播放施法动作
            owner.PlayerAnim(skillCfg.Action);

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

            var skillDamage = GetSkillDamage();
            foreach (var unit in unitList)
            {
                if (skillDamage > 0)
                {
                    unit.OnSkillDamaged(owner, skillId, skillDamage);
                    if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                        EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
                }
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }
}
