using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陆逊·火海（术）：在身前扇形区域生成多片火持续灼烧。
/// 必定有 1 片火落在目标单位身上；其余 SummonCount-1 片按 Range/SummonCount 等距分布在
/// 施法者朝目标方向的扇形内（左右各 30° 随机开角）。
/// 每片火召唤一个火焰场（SummonTag=火）并播放火特效，周期性(skillCfg.SummonHitInterval)对落点周围敌人持续造成伤害。
/// 非持续施法：放火后施法者可正常移动/普攻，火焰场按 SummonTime 独立持续跳伤。
/// </summary>
public class SkillCastingAidScorchedEarth : Skill
{
    /// <summary>扇形左右开角（度）</summary>
    private const float HalfAngle = 30f;
    /// <summary>未配置 SummonCount 时的默认火焰片数</summary>
    private const int DefaultFireCount = 4;

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

        // 前方扇形：必定有一片火落在目标单位身上；其余 SummonCount-1 片按 Range 平均分距离
        var origin = owner.transform.position;
        var dir = target.transform.position - origin;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        dir.y = 0f;
        dir.Normalize();

        int fireCount = skillCfg.SummonCount > 0 ? skillCfg.SummonCount : DefaultFireCount;
        targetPosList = new List<Vector3>();

        // 目标身上的那一片
        var targetPos = target.transform.position;
        targetPos.y = origin.y;
        targetPosList.Add(targetPos);

        // 其余 SummonCount-1 片：按 Range/SummonCount 等距分布在施法者身前，随机开角
        var step = skillCfg.Range / fireCount;
        for (int i = 1; i < fireCount; i++)
        {
            var angle = (SysRandom.Value * 2f - 1f) * HalfAngle;
            var pos = origin + Quaternion.Euler(0f, angle, 0f) * dir * (step * i);
            pos.y = origin.y;
            targetPosList.Add(pos);
        }

        // 每片火各召唤一个火焰场（SummonTag=火，供蔓延判定/持续跳伤），并播放范围特效
        float summonTime = skillCfg.SummonTime > 0f ? skillCfg.SummonTime : GetSummonTime();
        foreach (var pos in targetPosList)
        {
            SummonMagicField(pos, out summonTime);
            PlayAreaEffect(pos, summonTime);
        }

        // 非持续施法：放下火后施法者可正常移动/普攻，火焰场按 SummonTime 独立持续跳伤
        owner.StartCoroutine(DelayDamage(summonTime));
        return true;
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            // 施法者死亡后停止跳伤
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
