using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 温良（温良恭俭）：每 CD(6秒) 为自己套一个 Strength2[0] × (100+法强)/100 的护盾（随法强成长）。
/// 护盾持续到被击破（BuffTime=999），CD 到自动补满。套盾写法同「护」。
/// </summary>
public class SkillAidSelfShield : Skill
{
    public SkillAidSelfShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        if (string.IsNullOrEmpty(skillCfg.BuffId))
        {
            GameLog.Error($"温良 武将{owner.heroId} BuffId未配置");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);

        var shieldId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldId) as BuffShield;
        var shieldHp = GetSkillShield(0);
        if (shield != null)
            shield.SetHp(shieldHp);
        GameLog.Debug($"温良 武将{owner.heroId} 套盾{shieldHp}");

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}