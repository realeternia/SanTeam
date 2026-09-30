using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 郭嘉·遗计（术）：释放持续穿透激光，持续 bufftime 秒，每秒对路径上所有敌人造成 /strength 法术伤害。
/// 施法期间自身处于引导状态(castingSkillId>0)，无法移动/普攻，被晕眩/死亡打断停止；当前目标死亡自动切换新的最前方敌人继续照射。
/// </summary>
public class SkillAidLastStrategy : Skill
{
    public SkillAidLastStrategy(int id, Chess unit) : base(id, unit)
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
        var endTime = Time.time + Math.Max(0.1f, skillCfg.BuffTime);
        var halfWidth = skillCfg.Area * 0.5f;
        var skillDamage = GetSkillDamage();

        // 起始目标锁敌方当前难缠目标，死亡后切最前方的存活敌人
        Chess cur = FirstFrontEnemy();
        if (cur == null)
            cur = FirstEnemy();

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
            }

            float ox = owner.transform.position.x;
            float oz = owner.transform.position.z;
            // 激光方向（指向当前目标，y 轴忽略，使用 xz 平面方向）
            float tx = cur.transform.position.x - ox;
            float tz = cur.transform.position.z - oz;
            float len = (float)Math.Sqrt(tx * tx + tz * tz);
            if (len < 0.001f)
                len = 1f;
            float px = tx / len;
            float pz = tz / len;

            var enemies = WorldManager.Instance.GetAllEnemys(owner.side);
            foreach (var e in enemies)
            {
                if (e == null || e.hp <= 0 || e == owner)
                    continue;
                float ex = e.transform.position.x - ox;
                float ez = e.transform.position.z - oz;
                // 处于激光方向前方(正投影)且垂直距离在激光半宽内的敌人受击
                float proj = ex * px + ez * pz;
                if (proj < 0f)
                    continue;
                float perp = (float)Math.Sqrt(Math.Max(0f, ex * ex + ez * ez - proj * proj));
                if (perp <= halfWidth && skillDamage > 0)
                    e.OnSkillDamaged(owner, skillId, skillDamage);
            }

            yield return new WaitForSeconds(1f);
        }
    }

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