using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"单体伤害 + 可选减益"技能（ScriptName = "SkillAidTargetDamage"）：
/// 锁定 skillCfg.Range 射程内的当前目标，造成 GetSkillDamage() 技能伤害，并可选附加 SkillConfig.BuffId
/// 指定的减益 Buff（英文/中文逗号或分号分隔多个，空=纯伤害），时长 BuffTime。
/// TargetCount>0 时对同一目标连打 TargetCount 段（如荀攸·奇策的段数），否则单段。
/// 覆盖：杜预·发火、曹真·虎豹（纯伤害） / 夏侯渊·猎鹰、王双·崩山("破") /
/// 朱桓·标记("伤") / 陈宫·绝策("慑") / 夏侯惇·歼灭、马谡·守势("乱") /
/// 陈群·律令("疫") / 荀攸·奇策("缓"，TargetCount=段数)。
/// </summary>
public class SkillAidTargetDamage : Skill
{
    public SkillAidTargetDamage(int id, Chess unit) : base(id, unit)
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

        // 段数：TargetCount>0 时对同一目标连打 N 段（奇策），否则单段
        var dmg = GetSkillDamage();
        var hits = skillCfg.TargetCount > 0 ? skillCfg.TargetCount : 1;
        for (var i = 0; i < hits; i++)
        {
            if (target.hp <= 0)
                break;
            if (dmg > 0)
                target.OnSkillDamaged(owner, id, dmg);
        }

        // 减益 buff 由 SkillConfig.BuffId 短名指定（空=纯伤害，如发火/虎豹），支持多个
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
