using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 马超·西凉铁骑（主）两段冲锋：先沿朝目标方向突进 Range 距离（突过去），再原路撤回起点（突回来），
/// 形似 RunCross 穿越两次。每段对冲撞路径上的敌人各结算一次 GetSkillDamage，并施加 skillCfg.BuffId 配置的减防 Buff（"破"），
/// 每段最多命中 TargetCount 名敌人、同一段内同一敌人只命中一次（跨段可被再冲一次），单人最多吃到两次伤害。
/// 路径命中半径取 skillCfg.Area（Area=0 在全图语义下会命中全场，配置须非 0）。
/// </summary>
public class SkillAidCavalryStrike : Skill
{
    /// <summary>单段冲锋耗时（秒）</summary>
    private const float MoveDuration = 1f;
    /// <summary>路径采样步长（秒），与 RunCross 系列一致</summary>
    private const float StepTime = 0.025f;

    public SkillAidCavalryStrike(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;

        // 突进方向取朝目标方向；Range 同时作索敌距离与突进长度
        var startPos = owner.transform.position;
        var dir = target.transform.position - startPos;
        dir.y = 0f;
        if (dir.magnitude < 0.001f)
            return false;
        var farPos = startPos + dir.normalized * skillCfg.Range;
        farPos.y = startPos.y;

        // 落点撞墙则不发动（放在 CheckBurst 之前，避免白扣 MP），与 RunCross 系列一致
        if (!WorldManager.Instance.TryLockGridPositions(owner, farPos, out _))
            return false;
        if (!CheckBurst(target))
            return false;


        owner.noMoveCount++;
        owner.StartCoroutine(ChargeTwice(startPos, farPos));
        return true;
    }

    // 两段冲锋：突过去 → 突回来，结束后回到起点
    private IEnumerator ChargeTwice(Vector3 startPos, Vector3 farPos)
    {
        yield return ChargeOnce(startPos, farPos);
        yield return ChargeOnce(farPos, startPos);

        owner.MoveTo(startPos, true);
        owner.noMoveCount--;
        owner.FindTarget(); //冲刺结束后重新锁定一次
    }

    // 单段冲锋：从 from 沿直线冲到 to，路径上最多命中 TargetCount 名敌人（每人只结算一次）
    private IEnumerator ChargeOnce(Vector3 from, Vector3 to)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var damage = GetSkillDamage();
        var buffIds = GetSkillBuffIds();
        var hitIds = new List<int>();
        var elapsed = 0f;

        while (elapsed < MoveDuration)
        {
            owner.transform.position = Vector3.Lerp(from, to, elapsed / MoveDuration);

            var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
            foreach (var enemy in enemies)
            {
                if (hitIds.Count >= skillCfg.TargetCount)
                    break;
                if (enemy == null || enemy == owner || enemy.hp <= 0 || hitIds.Contains(enemy.id))
                    continue;

                hitIds.Add(enemy.id);
                if (damage > 0)
                {
                    enemy.OnSkillDamaged(owner, id, damage);
                    if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                        EffectManager.PlaySkillEffect(enemy, skillCfg.HitEffect);
                }

                foreach (var buffId in buffIds)
                    BuffManager.AddBuff(enemy, owner, id, buffId, skillCfg.BuffTime);
            }

            elapsed += StepTime;
            yield return new WaitForSeconds(StepTime);
        }

        owner.transform.position = to;
    }
}
