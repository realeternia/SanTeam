using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 郭嘉·遗计（术）：释放持续穿透激光，持续 summontime 秒，每秒对路径上所有敌人造成 /damagestrength 法术伤害。
/// 施法期间自身处于引导状态(castingSkillId>0)，无法移动/普攻，被晕眩/死亡打断停止；当前目标死亡自动切换新的最前方敌人继续照射。
/// </summary>
public class SkillCastingAidLastStrategy : Skill
{
    public SkillCastingAidLastStrategy(int id, Chess unit) : base(id, unit)
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
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        // 引导状态：引导期内 castingSkillId>0，MoveAndFight 提前返回（无法移动/普攻），被晕眩/死亡打断
        owner.StartCasting(id, LaserChannel());
        return true;
    }

    IEnumerator LaserChannel()
    {
        var endTime = Time.time + Math.Max(0.1f, skillCfg.SummonTime);
        var skillDamage = GetSkillDamage();

        // 起始目标锁敌方最前方的存活敌人，死亡后切最前方的存活敌人
        Chess cur = FirstFrontEnemy();
        if (cur == null)
            cur = FirstEnemy();

        // 激光视觉：以自身为发射源，固定长度=施法距离，命中判定宽度=Area（命中列表同样由控制器给出）
        var beam = LaserBeamController.Spawn(owner, cur, skillCfg.Range, skillCfg.Area);

        while (Time.time < endTime)
        {
            // 被动打断（晕眩/死亡）：BreakCasting 已把 castingSkillId 置0，此处结束引导
            if (owner.castingSkillId != id)
                break;

            if (owner == null || owner.hp <= 0)
                break;

            // 引导期间每跳持续播放施法动作
            owner.PlayerAnim(skillCfg.Action);

            // 当前照射目标死亡/消失时，切换新的前方敌人
            if (cur == null || cur.hp <= 0)
            {
                cur = FirstEnemy();
                if (cur == null)
                    break;
                if (beam != null)
                    beam.SetTarget(cur);
            }

            if (beam != null)
            {
                foreach (var e in beam.GetHitUnits())
                {
                    if (skillDamage > 0)
                        e.OnSkillDamaged(owner, skillId, skillDamage);
                    // 命中粒子：每次结算在受击目标处播一次火花
                    beam.PlayImpact(e);
                }
            }

            yield return new WaitForSeconds(1f);
        }


        if (beam != null)
            beam.Dispose();    }

    Chess FirstEnemy()
    {
        var enemies = WorldManager.Instance.GetAllEnemys(owner.side);
        foreach (var e in enemies)
        {
            if (e != null && e.hp > 0)
                return e;
        }
        return null;
    }

    // 取距离自己最近的存活敌人作为优先级目标（左侧阵营朝右、右侧朝左时即场上最前方敌人）
    Chess FirstFrontEnemy()
    {
        var enemies = WorldManager.Instance.GetAllEnemys(owner.side);
        Chess best = null;
        float bestDist = float.MaxValue;
        foreach (var e in enemies)
        {
            if (e == null || e.hp <= 0)
                continue;
            float d = WorldManager.Instance.GetRange(owner.transform.position, e.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = e;
            }
        }
        return best;
    }
}