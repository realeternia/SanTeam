using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陈泰·扬鞭：对单体造成法术伤害，并给自身挂攻速提升 buff（BuffId 配到 SkillConfig，仅攻速不含移速），
/// 数值取 StrengthBuff1[0]，时长 bufftime。
/// </summary>
public class SkillAidHasteStrike : Skill
{
    public SkillAidHasteStrike(int id, Chess unit) : base(id, unit)
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

        target.OnSkillDamaged(owner, id, GetSkillDamage());

        var hasteId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(owner, owner, id, hasteId, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}