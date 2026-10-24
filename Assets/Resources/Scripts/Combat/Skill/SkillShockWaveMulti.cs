using UnityEngine;

/// <summary>
/// 太史慈·连珠专用（ScriptName = "SkillShockWaveMulti"）：朝目标方向一次射出多支 shockwave 箭，
/// 箭数 = SummonCount ~ SummonCount+2（随机），开角 ±30°，第 1 支固定朝正中心。
/// 每支箭的伤害/命中结算复用 Missile 的方向型流程（每人只结算一次，最多 TargetCount 名），不做击退。
/// </summary>
public class SkillShockWaveMulti : Skill
{
    /// <summary>箭矢最大偏转角（度），正负对称</summary>
    public const float SpreadHalfAngle = 30f;
    /// <summary>在 SummonCount 基础上随机追加的最大箭数（0 ~ 该值）</summary>
    public const int ExtraArrowMax = 2;

    public SkillShockWaveMulti(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;

        var start = owner.transform.position;
        var targetPos = target.transform.position;
        if (!WorldManager.Instance.CheckInRange(start, targetPos, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        // 基准方向与射程：每支箭按自身偏转角旋转后沿同一距离取瞄准点
        var dir = targetPos - start;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        dir.y = 0f;
        dir.Normalize();
        var distance = Mathf.Max(WorldManager.Instance.GetRange(start, targetPos), 1f);

        var count = skillCfg.SummonCount + SysRandom.Range(0, ExtraArrowMax + 1);

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var damage = GetSkillDamage();
        for (var i = 0; i < count; i++)
        {
            // 首箭固定正中，其余在 ±SpreadHalfAngle 内随机
            var angle = i == 0 ? 0f : SysRandom.Range(-SpreadHalfAngle, SpreadHalfAngle);
            var aim = start + Quaternion.Euler(0f, angle, 0f) * dir * distance;
            aim.y = targetPos.y;
            WorldManager.Instance.CreateSpellMissile(owner, aim, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect, null, null);
        }

        GameLog.Info("连珠：" + owner.heroId + " 射出 " + count + " 支箭，单箭伤害 " + damage);
        return true;
    }
}
