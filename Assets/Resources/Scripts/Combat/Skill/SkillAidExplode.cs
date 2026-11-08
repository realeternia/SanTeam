using CommonConfig;
using UnityEngine;

/// <summary>
/// 袁绍 · 名门（单体爆炸）：锁定 skillCfg.Range 射程内的当前目标，造成 GetSkillDamage() 技能伤害；
/// 随后以目标位置为中心，对 skillCfg.Area 半径内的其他敌人造成 伤害 × 键 "explodeRate"（默认 0.5）的溅射伤害。
/// 主目标不参与溅射（避免二次结算）；命中特效播在主目标与每个受溅射目标，范围特效播在爆炸中心。
/// </summary>
public class SkillAidExplode : Skill
{
    public SkillAidExplode(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (skillCfg.Range > 0f && !WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var center = target.transform.position;
        var dmg = GetSkillDamage();
        PlayAreaEffect(center);

        if (dmg > 0)
            target.OnSkillDamaged(owner, id, dmg);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        // 溅射：以技能基础伤害为基数乘系数，与主目标伤害保持同源（同样随法强成长）
        var splash = (int)(dmg * GetStr2("explodeRate", 0.5f));
        if (splash <= 0)
            return true;

        var enemies = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
        foreach (var u in enemies)
        {
            if (u == null || u == target || u.hp <= 0)
                continue;
            u.OnSkillDamaged(owner, id, splash);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
        }
        return true;
    }
}
