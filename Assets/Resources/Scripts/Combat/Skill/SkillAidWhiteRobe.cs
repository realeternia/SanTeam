using System.Collections;
using UnityEngine;

/// <summary>
/// 吕蒙·白衣渡江（特有技能，不走模板）：先平滑位移到目标身前，落地后再结算法术伤害；
/// 首段 Buff 挂给目标、其余 Buff 挂给自身（BuffId="缓,愈,抗" → 减速给目标，回血与魔抗给自己），持续 BuffTime。
/// 伤害结算与通用命中后处理（破盾/燃蓝/击退/苦肉/驱散）沿用 SkillComDamageSingle。
/// </summary>
public class SkillAidWhiteRobe : SkillComDamageSingle
{
    /// <summary>突进位移耗时(秒)</summary>
    private const float MoveDuration = 0.3f;

    /// <summary>落点与目标保持的距离(米)，避免与目标重叠</summary>
    private const float StopDistance = 10f;

    /// <summary>位移途中禁止重复发动</summary>
    private bool moving;

    public SkillAidWhiteRobe(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (moving)
            return false;

        var target = SelectTarget();
        if (target == null)
            return false;
        if (!CanCast(target))
            return false;

        PlayCast(target);

        moving = true;
        owner.StartCoroutine(MoveThenStrike(target));
        return true;
    }

    /// <summary>平滑位移到目标身前 → 结算伤害与 Buff；目标途中死亡时仍给自己挂 Buff。</summary>
    private IEnumerator MoveThenStrike(Chess target)
    {
        var groundY = owner.baseY + owner.heightOffset;
        var start = owner.transform.position;
        start.y = groundY;

        var targetPos = target.transform.position;
        targetPos.y = groundY;

        var dir = targetPos - start;
        dir.y = 0f;
        var distance = dir.magnitude;
        if (distance > 0.0001f)
            dir /= distance;

        // 走到目标身前 StopDistance 处；目标本来就更近时不后退
        var stop = Mathf.Min(StopDistance, distance);
        var dest = targetPos - dir * stop;
        dest.y = groundY;

        BeginAirMove();
        var move = AirMove(MoveDuration, t => Vector3.Lerp(start, dest, t));
        while (move.MoveNext())
            yield return move.Current;
        EndAirMove();

        if (owner == null || owner.hp <= 0)
        {
            moving = false;
            yield break;
        }

        if (target != null && target.hp > 0)
            ResolveHit(target);
        else
            ApplySelfBuffs();

        moving = false;
    }

    /// <summary>Buff 归属：首段挂目标、其余挂自身（原模板组件 bothBuff 的专用实现）。</summary>
    protected override void ApplySkillBuffs(Chess target)
    {
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
            return;

        ApplyBuffToUnit(target, buffIds[0]);
        ApplySelfBuffs();
    }

    /// <summary>给自己挂除首段以外的 Buff（目标死亡时也能生效）。</summary>
    private void ApplySelfBuffs()
    {
        var buffIds = GetSkillBuffIds();
        for (var i = 1; i < buffIds.Count; i++)
            ApplyBuffToUnit(owner, buffIds[i]);
    }
}
