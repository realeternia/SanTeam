using CommonConfig;
using UnityEngine;

/// <summary>
/// 姜维·流光（车，ScriptName = "SkillAidDualLaser"）：向正前方及左右各 45° 共 3 道激光，
/// 每道复用惊雷的方向型导弹流程独立结算（各自最多命中原有 TargetCount 个目标，互不去重，
/// 同一目标被多道命中时伤害叠加），命中附加效果走基类 OnMissileHit（未配置 BuffId 时不挂 buff）。
/// </summary>
public class SkillAidDualLaser : SkillShockWave
{
    /// <summary>左右两道激光相对正前方的偏转角（度）</summary>
    private const float LaserSpreadAngle = 25f;

    public SkillAidDualLaser(int id, Chess unit) : base(id, unit)
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

        var startPos = owner.transform.position;
        var forward = target.transform.position - startPos;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            forward = new Vector3(0f, 0f, 1f);
        forward = forward.normalized;

        var damage = GetSkillDamage();
        // 目标点只用于确定飞行方向（协程内取方向向量归一化），实际飞行距离由 SummonTime × SummonSpeed 决定；
        // 每道激光是独立导弹（各自维护已命中列表），同一目标被多道命中时叠加结算
        for (int i = -1; i <= 1; i++)
        {
            var dir = Quaternion.Euler(0f, i * LaserSpreadAngle, 0f) * forward;
            WorldManager.Instance.CreateSpellMissile(owner, startPos + dir * skillCfg.Range, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect, null, OnMissileHit);
        }

        GameLog.Debug("SkillAidDualLaser id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }
}
