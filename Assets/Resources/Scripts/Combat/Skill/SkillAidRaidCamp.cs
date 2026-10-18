using CommonConfig;
using UnityEngine;

/// <summary>
/// 甘宁·百骑劫营（武）：锁定射程内最远的敌方后排单体，突进到其近旁，
/// 对其造成 GetSkillDamage 物理伤害并燃烧其法力（TargetType 含 mpBurn，燃烧量取 Strength2[0]）。
/// </summary>
public class SkillAidRaidCamp : Skill
{
    /// <summary>突进落点到目标的水平偏移距离</summary>
    private const float DashOffset = 2f;

    public SkillAidRaidCamp(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = BacklineEnemy();
        if (target == null)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        DashTo(target);

        var damage = GetSkillDamage();
        if (damage > 0)
            target.OnSkillDamaged(owner, skillId, damage);

        // 法力燃烧：燃烧量取本技能的约定槽位 Strength2[0]，每次释放只结算一次
        if (HasSkillFlag("mpBurn"))
            BurnMpToTarget(target, GetStrength2(0));

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

    // 射程内最远的存活敌方单位（近似后排）
    private Chess BacklineEnemy()
    {
        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
        Chess best = null;
        float bestDist = -1f;
        foreach (var e in enemies)
        {
            if (e == null || e.hp <= 0)
                continue;
            float d = WorldManager.Instance.GetRange(owner.transform.position, e.transform.position);
            if (d > bestDist)
            {
                bestDist = d;
                best = e;
            }
        }
        return best;
    }

    // 突进到目标近旁（朝自身一侧偏移，落点被挡则原地不动）
    private void DashTo(Chess target)
    {
        var dir = owner.transform.position - target.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector3.right;
        var landPos = target.transform.position + dir.normalized * DashOffset;
        landPos.y = owner.transform.position.y;
        if (WorldManager.Instance.CheckPositionBlocked(owner, landPos))
            return;
        owner.transform.position = landPos;
    }
}