using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 赵云·龙胆：发动射程(Range)内瞬闪连击。首段只打英雄（射程内没有英雄则本次不发动），
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

    // 首段目标：当前索敌目标是射程内的英雄就用它，否则按 SelectOnly / SelectFirst 取射程内的英雄
    // （典型 enemy;hero + heroFirst;farthest；首段只打英雄，故用 extraFilter 强制 isHero）
    private Chess PickFirstTarget()
    {
        var cur = owner.targetChess;
        if (cur != null && cur.hp > 0 && cur.isHero
            && WorldManager.Instance.CheckInRange(owner, cur, skillCfg.Range))
            return cur;

        var heroes = SelectTargets(owner.transform.position, skillCfg.Range, false, 1, null, x => x.isHero);
        return heroes.Count > 0 ? heroes[0] : null;
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

            // 下一段：按 SelectOnly / SelectFirst 在 Area 内重选（优先未被本技能打过的单位）
            current = PickNextTarget(hitTargets);
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

    // 下一段目标：按 SelectOnly / SelectFirst 优先选未被本技能打过的单位；全都打过则退化为重新选一次
    private Chess PickNextTarget(List<Chess> hitTargets)
    {
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, false, 1, null, x => !hitTargets.Contains(x));
        if (picked.Count == 0)
            picked = SelectTargets(owner.transform.position, skillCfg.Area, false, 1);
        return picked.Count > 0 ? picked[0] : null;
    }
}
