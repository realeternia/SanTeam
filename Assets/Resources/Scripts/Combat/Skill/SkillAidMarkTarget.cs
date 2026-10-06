using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 单体伤害+减益：对单体目标造成 /damagestrength 法术伤害并附加 skillCfg.BuffId 指定的减益 buff，
/// 时长 bufftime（朱桓·标记="伤"增伤；陈宫·绝策="慑"降攻）。
/// </summary>
public class SkillAidMarkTarget : Skill
{
    public SkillAidMarkTarget(int id, Chess unit) : base(id, unit)
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

        // 减益 buff 由 SkillConfig.BuffId 指定，不在战斗代码硬编码 BuffId
        var debuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (debuffCfg == null)
        {
            GameLog.Error($"SkillAidMarkTarget: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
            return false;
        }
        BuffManager.AddBuff(target, owner, id, debuffCfg.Id, skillCfg.BuffTime);

        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
