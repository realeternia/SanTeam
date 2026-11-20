using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张飞·旋风斩（主动）：锁定射程内一名敌人后施放，对自身周围最多 TargetCount 个敌人造成
/// key "splashRate" × 自身攻击的伤害，并对被命中的敌人附加流血
/// （Buff 短名由 skillCfg.BuffId 配置），每秒伤害 = 本次伤害 × key "dot"，持续 BuffTime 秒。
/// </summary>
public class SkillAidSpinAttack : Skill
{
    public SkillAidSpinAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 主动施放：射程内需有敌人锁定目标，再按 MP/CD/概率闸门判定
        var target = SelectSingleTarget(skillCfg.Range);
        if (target == null || !CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(owner.transform.position);

        // 旋风范围：按 SelectOnly / SelectFirst 选（默认敌方）
        var unitsInRange = SelectTargets(owner.transform.position, skillCfg.Area, false, skillCfg.TargetCount);
        foreach (var unit in unitsInRange)
        {
            var spinDamage = ApplyDamageModifiers((int)(owner.atk * GetStr2("splashRate")), unit);
            unit.OnSkillDamaged(owner, skillId, spinDamage);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            ApplyBleed(unit, spinDamage);
        }
        return true;
    }

    // 对目标附加流血：每秒伤害 = 本次溅射伤害 × key "dot"
    private void ApplyBleed(Chess target, int baseDamage)
    {
        if (target == null || target.hp <= 0 || baseDamage <= 0)
            return;

        var bleed = (int)(baseDamage * GetStrBuff("dot"));
        if (bleed <= 0)
            return;

        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error("SkillAidSpinAttack: 未找到流血Buff短名：" + skillCfg.BuffId);
            return;
        }

        BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        // 共存模式(StackMode=1)下同 Id 可能已有旧实例，取本次新加的最后一个
        var buff = target.GetLastBuff(buffCfg.Id) as BuffTimeDamage;
        if (buff != null)
            buff.SetDamage(bleed);
    }
}