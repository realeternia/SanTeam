using System.Collections;
using UnityEngine;

/// <summary>
/// 孙策·破阵冲阵：以目标单位为圆心、Area 为半径，施法时自身先抬高 LiftHeight，
/// 随后一边绕目标旋转一整圈、一边从高处下降回地面，旋转期间播放范围特效，
/// 结束时对圆心 Area 范围内的所有敌人造成一次 AOE 伤害（每个敌人只结算一次）。
/// 与吕布·魔神一致：全程处于空中位移状态(BeginAirMove)，外部位移/击退/拖拽无法打断，
/// 寻路与普攻也被屏蔽，避免位置被来回拉扯造成闪烁；落地后重新索敌。
/// </summary>
public class SkillAidChargeImpale : Skill
{
    /// <summary>绕圈总时长（秒）</summary>
    private const float SpinDuration = .8f;
    /// <summary>施法时抬高的高度（米）</summary>
    private const float LiftHeight = 10f;

    public SkillAidChargeImpale(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 已在跳跃/空中位移中则不重复施放
        if (owner.isJumping)
            return false;
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // 圆心 = 目标位置；起始角度取施法瞬间自身相对圆心的方向，绕行从当前方位开始
        var center = target.transform.position;
        var startDir = owner.transform.position - center;
        startDir.y = 0f;
        if (startDir.sqrMagnitude < 0.0001f)
            startDir = owner.transform.forward;
        startDir.y = 0f;
        startDir.Normalize();

        owner.StartCoroutine(SpinAndStrike(center, startDir));
        return true;
    }

    // 抬高 → 绕目标旋转并下降 → 落地结算 AOE
    private IEnumerator SpinAndStrike(Vector3 center, Vector3 startDir)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 空中位移状态：不可被位移技能/控制打断，并锁住寻路与普攻（避免两套位移互相拉扯）
        BeginAirMove();

        var groundY = owner.baseY + owner.heightOffset;
        var elapsed = 0f;
        while (elapsed < SpinDuration)
        {
            if (owner == null || owner.hp <= 0)
            {
                EndAirMove();
                yield break;
            }

            var t = Mathf.Clamp01(elapsed / SpinDuration);
            // 从起始方位匀速绕满一圈
            var angle = t * 360f;
            var pos = center + Quaternion.Euler(0f, angle, 0f) * startDir * skillCfg.Area;
            // 一边旋转一边从 LiftHeight 平滑下降回地面
            pos.y = groundY + LiftHeight * (1f - t);

            owner.transform.position = pos;

            elapsed += AirMoveStepTime;
            yield return new WaitForSeconds(AirMoveStepTime);
        }

        // 落地站稳（回到起点方位的圆周位置）
        var landPos = center + startDir * skillCfg.Area;
        landPos.y = groundY;
        owner.transform.position = landPos;
        EndAirMove();

        // 落地 AOE：圆心范围内所有敌人各结算一次伤害
        var damage = GetSkillDamage();
        var enemies = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
        PlayAreaEffect(center, SpinDuration);
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy == owner || enemy.hp <= 0)
                continue;
            if (damage > 0)
            {
                enemy.OnSkillDamaged(owner, id, damage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(enemy, skillCfg.HitEffect);
            }
        }

        owner.FindTarget(); // 落地后重新索敌
    }
}
