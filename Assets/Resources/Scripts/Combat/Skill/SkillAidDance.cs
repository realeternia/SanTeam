using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙尚香·纵舞：射击2次后向远离目标方向后撤一段距离
/// </summary>
public class SkillAidDance : Skill
{
    public SkillAidDance(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 纵舞：间隔后退，保持机动
    /// </summary>
    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        Vector3 dir = (owner.transform.position - target.transform.position).normalized;
        if (dir.magnitude < 0.001f)
            dir = Vector3.forward;
        owner.transform.position += dir * 15f;

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}
