using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 邓艾·偷渡阴平：跃向 Range 内最远的敌方英雄（抛物线飞行），落地后对其施加「陷阵」BuffTime 秒；
/// 停留 BuffTime 秒后沿抛物线跳回起跳点。
/// 两段跳跃统一走 Skill.JumpTo（跳跃期间 isJumping，击退/拖拽/换位等外部位移无法打断，动作必播完）。
/// </summary>
public class SkillAidJumpReturn : Skill
{
    /// <summary>跳跃峰值高度（世界单位）</summary>
    private const float JumpHeight = 18f;
    /// <summary>单段跳跃耗时（秒）</summary>
    private const float JumpDuration = 0.6f;
    /// <summary>落点与目标的水平距离候选（依次尝试，被挡则收短，最后落到目标脚下）</summary>
    private static readonly float[] LandDists = { 12f, 6f, 0f };

    public SkillAidJumpReturn(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0 || owner.isJumping)
            return false;

        // 目标：Range 内最远的敌方单位（优先英雄，无英雄时取最远的小兵）
        var target = FindFarthestEnemy();
        if (target == null)
            return false;
        if (!CheckBurst(target))
            return false;

        // 整段（去程 + 停留 + 回程）锁住寻路，避免落地后自行走开
        owner.noMoveCount++;
        owner.LockTarget(target);
        owner.StartCoroutine(JumpSequence(target));
        return true;
    }

    // Range 内最远的存活敌方单位：优先英雄，其次任意敌人
    private Chess FindFarthestEnemy()
    {
        Chess farthestHero = null;
        Chess farthestAny = null;
        var bestHero = -1f;
        var bestAny = -1f;
        foreach (var u in WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side))
        {
            if (u == null || u.hp <= 0)
                continue;
            var d = WorldManager.Instance.GetRange(owner.transform.position, u.transform.position);
            if (d > bestAny)
            {
                bestAny = d;
                farthestAny = u;
            }
            if (u.isHero && d > bestHero)
            {
                bestHero = d;
                farthestHero = u;
            }
        }
        return farthestHero != null ? farthestHero : farthestAny;
    }

    IEnumerator JumpSequence(Chess target)
    {
        var groundY = owner.baseY + owner.heightOffset;
        var origin = owner.transform.position;
        origin.y = groundY;
        var landPos = CalcLandPos(target, groundY);

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 去程：抛物线跳到目标身前（桩协程不展开嵌套枚举器，须手动逐段推进）
        var jump = JumpTo(landPos, JumpHeight, JumpDuration);
        while (jump.MoveNext())
            yield return jump.Current;

        if (owner == null || owner.hp <= 0)
            yield break;

        // 落地：施加陷阵
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
            GameLog.Error($"SkillAidJumpReturn: 缺少陷阵Buff配置 BuffId={skillCfg.BuffId} 技能id={id}");
        if (target != null && target.hp > 0)
        {
            foreach (var buffId in buffIds)
                BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }

        // 停留 BuffTime 秒（目标被陷阵，自身可继续普攻）
        yield return new WaitForSeconds(Mathf.Max(0.1f, skillCfg.BuffTime));

        if (owner == null || owner.hp <= 0)
            yield break;

        // 回程：跳回起跳点
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        var back = JumpTo(origin, JumpHeight, JumpDuration);
        while (back.MoveNext())
            yield return back.Current;

        if (owner == null || owner.hp <= 0)
            yield break;
        owner.noMoveCount--;
        owner.FindTarget();
    }

    // 落点：目标当前位置沿「目标→自身」方向外推 LandDistance（即目标身前）；落点被挡时逐步收短，避免落进障碍
    private Vector3 CalcLandPos(Chess target, float groundY)
    {
        var dir = owner.transform.position - target.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        dir = dir.normalized;

        var targetPos = target.transform.position;
        foreach (var dist in LandDists)
        {
            var pos = targetPos + dir * dist;
            pos.y = groundY;
            if (dist <= 0f || !WorldManager.Instance.CheckPositionBlocked(owner, pos))
                return pos;
        }
        var fallback = targetPos;
        fallback.y = groundY;
        return fallback;
    }
}
