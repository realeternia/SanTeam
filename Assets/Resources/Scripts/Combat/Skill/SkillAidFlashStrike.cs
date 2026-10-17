using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 赵云·快闪：发动射程(Range)内瞬闪连击。首段只打英雄（射程内没有英雄则本次不发动），
/// 从第二段起在 Area 内重选目标：英雄优先，范围内没有英雄时才允许跳小兵；没有任何可打目标即停止。
/// 首次闪到目标身前（沿起手方向后退 BlinkBack，避免与目标叠住），之后各次闪到目标身后（沿起手方向穿过目标再走 BlinkBack），
/// 最多闪击配置 TargetCount 次，
/// 结束后停在最后目标附近。整段过程自身持续挂导弹特效（SelfEffect，走 EffectConfig.MissilePath 的导弹预制体），
/// 瞬移时粒子在路径上留下拖尾残影。位移期间用 noMoveCount 锁住寻路/普攻，结束后 FindTarget 恢复。
/// </summary>
public class SkillAidFlashStrike : Skill
{
    /// <summary>相邻两次闪击的间隔（秒）</summary>
    private const float StrikeInterval = 0.5f;
    /// <summary>闪击落点相对目标中心的后退距离（防止与目标叠住）</summary>
    private const float BlinkBack = 10f;

    public SkillAidFlashStrike(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 首段只打英雄：射程内找不到英雄则本次不发动
        var target = PickFirstTarget();
        if (target == null)
            return false;
        if (!CheckBurst(target))
            return false;

        // 连击期间由协程接管位置，锁住寻路与普攻
        owner.noMoveCount++;
        owner.StartCoroutine(FlashStrike(target));
        return true;
    }

    // 首段目标：当前索敌目标是射程内的英雄就用它，否则取射程内最远的英雄（未被本技能打过优先）
    private Chess PickFirstTarget()
    {
        var cur = owner.targetChess;
        if (cur != null && cur.hp > 0 && cur.isHero
            && WorldManager.Instance.CheckInRange(owner.transform.position, cur.transform.position, skillCfg.Range))
            return cur;

        var heroes = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side);
        return PickBest(heroes, null, true);
    }

    // 快闪连击：首段闪到目标身前、之后各段闪到身后造成伤害 → 间隔 0.5s → 闪击下一个
    IEnumerator FlashStrike(Chess firstTarget)
    {
        // 整段过程给自身挂导弹特效：瞬移时粒子在路径上留下拖尾残影
        var trail = EffectManager.PlayFollowMissileEffect(owner, skillCfg.SelfEffect);

        // 起手方向（赵云→首个目标，水平）：区分身前/身后，整段固定，避免每次跳到目标另一侧后方向翻转
        var approach = firstTarget.transform.position - owner.transform.position;
        approach.y = 0f;
        if (approach.magnitude < 0.001f)
            approach = owner.transform.forward;
        approach = approach.normalized;

        var hitTargets = new List<Chess>();
        var current = firstTarget;
        int count = Mathf.Max(1, skillCfg.TargetCount);

        for (int i = 0; i < count; i++)
        {
            if (owner == null || owner.hp <= 0)
                break;
            if (current == null || current.hp <= 0)
                break;

            // 首段闪到目标身前 BlinkBack，之后各段闪到目标身后 BlinkBack
            owner.MoveTo(CalcBlinkPos(current, approach, i > 0), true);
            owner.LockTarget(current);
            owner.PlayerAnim(skillCfg.Action);
            PlaySelfEffect();

            Strike(current);
            hitTargets.Add(current);

            if (i >= count - 1)
                break;

            // 下一段：Area 内英雄优先，范围内没有英雄才允许跳小兵
            current = PickBest(WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side), hitTargets, false);
            if (current == null)
                break; // 英雄和小兵都没有 → 停止连击

            yield return new WaitForSeconds(StrikeInterval);
        }

        if (trail != null)
            UnityEngine.Object.Destroy(trail);

        if (owner != null)
        {
            owner.noMoveCount--;
            // 结束后停在最后目标附近，恢复正常索敌
            owner.FindTarget();
        }
    }

    // 单次闪击伤害
    private void Strike(Chess target)
    {
        var damage = GetSkillDamage();
        if (damage <= 0)
            return;

        target.OnSkillDamaged(owner, skillId, damage);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
    }

    // 闪击落点：身前 = 目标沿「赵云→目标」反方向退 BlinkBack；身后 = 沿该方向穿过目标再走 BlinkBack
    private Vector3 CalcBlinkPos(Chess target, Vector3 approach, bool behind)
    {
        var pos = target.transform.position + (behind ? approach : -approach) * BlinkBack;
        pos.y = target.transform.position.y;
        return pos;
    }

    // 从候选里挑最优目标：英雄优先于小兵（heroOnly 时仅考虑英雄），其次未被本技能打过，再次距离更远
    private Chess PickBest(List<Chess> candidates, List<Chess> hitTargets, bool heroOnly)
    {
        Chess best = null;
        bool bestWasHit = true;
        float bestDist = 0f;

        foreach (var enemy in candidates)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            if (heroOnly && !enemy.isHero)
                continue;

            bool wasHit = hitTargets != null && hitTargets.Contains(enemy);
            float dist = WorldManager.Instance.GetRange(owner.transform.position, enemy.transform.position);

            bool better;
            if (best == null)
                better = true;
            else if (enemy.isHero != best.isHero)
                better = enemy.isHero;          // 英雄优先，无英雄时才跳小兵
            else if (wasHit != bestWasHit)
                better = !wasHit;                // 未被本技能打过的优先
            else
                better = dist > bestDist;        // 同档取更远

            if (better)
            {
                best = enemy;
                bestWasHit = wasHit;
                bestDist = dist;
            }
        }

        return best;
    }
}
