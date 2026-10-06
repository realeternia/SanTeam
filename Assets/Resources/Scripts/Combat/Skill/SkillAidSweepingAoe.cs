using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张绣·枪出如龙：对自身 /area 范围内敌人各造成 /damagestrength 法术伤害，
/// 并附加 skillCfg.BuffId 指定的减益 buff（配"破"= 降低护甲，数值取 StrengthBuff1[0]）。
/// </summary>
public class SkillAidSweepingAoe : Skill
{
    public SkillAidSweepingAoe(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlayAreaEffect(owner.transform.position);

        var skillDamage = GetSkillDamage();
        var units = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);

        // 减益 buff 由 SkillConfig.BuffId 指定（短名"破"→BuffArmorDown），不在战斗代码硬编码 BuffId
        var debuffId = 0;
        if (!string.IsNullOrEmpty(skillCfg.BuffId))
        {
            var debuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
            if (debuffCfg != null)
                debuffId = debuffCfg.Id;
            else
                GameLog.Error($"SkillAidSweepingAoe: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
        }

        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
            if (debuffId > 0)
                BuffManager.AddBuff(u, owner, id, debuffId, skillCfg.BuffTime);
        }

        return true;
    }
}
