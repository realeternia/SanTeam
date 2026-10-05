using CommonConfig;
using UnityEngine;

/// <summary>
/// 甘宁·百骑劫营（武）：锁定射程内最远的敌方后排单体，突进到其近旁，
/// 对其造成 GetSkillDamage 物理伤害并降低其护甲（BuffArmorDown "破"）BuffTime 秒。
/// </summary>
public class SkillAidRaidCamp : Skill
{
    /// <summary>破甲 Buff 短名（BuffArmorDown "破"）</summary>
    public const string ArmorDownBuffNameS = "破";
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

        DashTo(target);

        var damage = GetSkillDamage();
        if (damage > 0)
            target.OnSkillDamaged(owner, skillId, damage);

        var buffCfg = BuffConfig.GetConfigByNameS(ArmorDownBuffNameS);
        if (buffCfg != null)
            BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidRaidCamp: 未找到破甲Buff短名：" + ArmorDownBuffNameS);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

    // 射程内最远的存活敌方单位（近似后排）
    private Chess BacklineEnemy()
    {
        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Range, owner.side);
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