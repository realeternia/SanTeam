using System.Collections;
using UnityEngine;

/// <summary>
/// 孙策·破阵冲阵：以施法时自身位置为圆心、Area 为半径，单位沿圆周绕行一整圈（约 1.5 秒），
/// 绕行结束后对圆心 Area 范围内的所有敌人造成一次 /damagestrength 法术伤害（AOE，每个敌人只结算一次）。
/// 绕行期间锁定常规移动，轨迹采样时遇墙停在上一帧位置，结束后回到圆心并重新索敌。
/// </summary>
public class SkillAidChargeImpale : Skill
{
    /// <summary>绕圈总时长（秒）</summary>
    private const float SpinDuration = 1f;
    /// <summary>从圆心外扩到圆周的时长（秒）</summary>
    private const float RampDuration = 0.2f;
    /// <summary>轨迹采样步长（秒）</summary>
    private const float StepTime = 0.025f;

    public SkillAidChargeImpale(int id, Chess unit) : base(id, unit)
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

        // 圆心 = 施法瞬间的自身位置；起始角度朝目标方向，绕行从面对目标开始
        var center = owner.transform.position;
        var startDir = target.transform.position - center;
        startDir.y = 0f;
        if (startDir.sqrMagnitude < 0.0001f)
            startDir = owner.transform.forward;
        startDir.y = 0f;
        startDir.Normalize();

        owner.noMoveCount++;
        owner.StartCoroutine(SpinAndStrike(center, startDir));
        return true;
    }

    // 绕圆周一整圈后回到圆心，再对范围内敌人结算一次 AOE 伤害
    private IEnumerator SpinAndStrike(Vector3 center, Vector3 startDir)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(center, SpinDuration);

        var lastPos = center;
        var elapsed = 0f;
        while (elapsed < SpinDuration)
        {
            // 半径：先在外扩阶段从 0 涨到 Area，随后保持圆周
            var radius = skillCfg.Area * Mathf.Clamp01(elapsed / RampDuration);
            // 角度：外扩阶段停留在起始角度，之后匀速走满一圈
            var angle = Mathf.Clamp01((elapsed - RampDuration) / (SpinDuration - RampDuration)) * 360f;

            var pos = center + Quaternion.Euler(0f, angle, 0f) * startDir * radius;
            pos.y = center.y;

            // 轨迹点踩墙时停在上一帧位置，避免穿墙
            if (!WorldManager.Instance.CheckPositionBlocked(owner, pos))
                lastPos = pos;
            owner.transform.position = lastPos;

            elapsed += StepTime;
            yield return new WaitForSeconds(StepTime);
        }

        // 回到圆心（原位置）并结算 AOE
        owner.MoveTo(center, true);

        var damage = GetSkillDamage();
        var enemies = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
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

        owner.noMoveCount--;
        owner.FindTarget(); // 绕圈结束后重新锁定一次
    }
}
