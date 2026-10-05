using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陈泰·换位：与敌方后排单体造成魔法伤害并交换位置（扭转敌我站位）
/// </summary>
public class SkillAidSwapStrike : Skill
{
    public SkillAidSwapStrike(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = BacklineEnemy();
        if (target == null)
            return false;
        if (!CheckBurst(target))
            return false;

        target.OnSkillDamaged(owner, id, GetSkillDamage());

        // 与目标交换位置
        Vector3 tempPos = owner.transform.position;
        owner.transform.position = target.transform.position;
        target.transform.position = tempPos;

        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

    // 射程内取距离自身最远的存活敌人（即敌方后排），优先换后排
    Chess BacklineEnemy()
    {
        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side);
        Chess back = null;
        float maxDist = -1f;
        if (enemies != null)
        {
            foreach (var e in enemies)
            {
                float d = Vector3.Distance(owner.transform.position, e.transform.position);
                if (d > maxDist)
                {
                    maxDist = d;
                    back = e;
                }
            }
        }
        return back;
    }
}