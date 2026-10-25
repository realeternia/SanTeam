using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙尚香·纵舞：先对当前目标射出第一段，随后向远离目标方向后撤 RetreatDistance，
/// 位移用 RetreatDuration 秒平滑播完（非瞬移，期间锁寻路/外部位移），
/// 落地后再向原目标补射第二段（无视射程），第二段伤害为第一段的 SecondDamageRate。
/// </summary>
public class SkillAidBackstepShot : Skill
{
    /// <summary>后撤距离(米)</summary>
    private const float RetreatDistance = 30f;

    /// <summary>后撤位移耗时(秒)，位移过程动画</summary>
    private const float RetreatDuration = 0.3f;

    /// <summary>第二段伤害相对第一段的比例</summary>
    private const float SecondDamageRate = 0.7f;

    public SkillAidBackstepShot(int id, Chess unit) : base(id, unit)
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

        // 第一段：对当前目标射出一箭
        var firstDamage = GetSkillDamage();
        if (firstDamage > 0)
            WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, firstDamage, skillCfg.HitEffect);

        owner.StartCoroutine(RetreatAndSecondShot(target, firstDamage));
        return true;
    }

    /// <summary>后撤位移（0.3 秒平滑播完）→ 落地后向原目标补射第二段</summary>
    private IEnumerator RetreatAndSecondShot(Chess target, int firstDamage)
    {
        var groundY = owner.baseY + owner.heightOffset;
        var start = owner.transform.position;
        start.y = groundY;

        // 后撤方向：远离目标
        Vector3 dir = start - target.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = -owner.transform.forward;
        dir.Normalize();
        var dest = start + dir * RetreatDistance;
        dest.y = groundY;

        BeginAirMove();
        var move = AirMove(RetreatDuration, t => Vector3.Lerp(start, dest, t));
        while (move.MoveNext())
            yield return move.Current;
        EndAirMove();

        if (owner == null || owner.hp <= 0)
            yield break;
        if (target == null || target.hp <= 0)
            yield break;

        // 第二段：无视射程，对后撤前的目标补射，伤害为第一段的 SecondDamageRate
        var secondDamage = (int)(firstDamage * SecondDamageRate);
        if (secondDamage > 0)
            WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, secondDamage, skillCfg.HitEffect);
    }
}
