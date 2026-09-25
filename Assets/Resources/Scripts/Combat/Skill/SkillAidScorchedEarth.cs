using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陆逊·火海（术）：范围召唤大量火随机位置持续灼烧。
/// 普攻触发后在目标周围 skillCfg.Range 半径内随机生成 SummonCount 个落点，
/// 每个落点召唤法术场并播放火特效，周期性(skillCfg.SummonHitInterval)对落点周围敌人持续造成伤害。
/// </summary>
public class SkillAidScorchedEarth : Skill
{
    private List<Vector3> targetPosList;

    public SkillAidScorchedEarth(int id, Chess unit) : base(id, unit)
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

        // 在目标周围 Range 半径内随机生成 SummonCount 个落点（x 与 z 独立随机偏移）
        var basePos = target.transform.position;
        targetPosList = new List<Vector3>();
        for (int i = 0; i < skillCfg.SummonCount; i++)
        {
            var ox = (SysRandom.Value * 2f - 1f) * skillCfg.Range;
            var oz = (SysRandom.Value * 2f - 1f) * skillCfg.Range;
            targetPosList.Add(basePos + new Vector3(ox, 0f, oz));
        }

        foreach (var pos in targetPosList)
        {
            var magicStub = SummonMagicField(pos, out var summonTime);
            EffectManager.PlayPosSkillEffect(magicStub, pos, skillCfg.EffectSize, skillCfg.HitEffect, summonTime);
        }

        owner.StartCoroutine(DelayDamage(GetSummonTime()));
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
                var unitsInRange = WorldManager.Instance.GetUnitsInRange(pos, skillCfg.Area * 1.5f, owner.side, true);
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
                    unit.OnSkillDamaged(owner, skillId, skillDamage);
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }
}
