using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 刘备·仁德：使我方范围内全体英雄附加由多个通用 Buff 组成的组合（SkillConfig.BuffId = "血,蓝"），不设人数上限——
/// 血（BuffHpRegenAdd）读 StrengthBuff1[0] 提升生命回复，蓝（BuffMpRegenAdd）读 [1] 提升法力回复。
/// </summary>
public class SkillAidBenevolence : Skill
{
    public SkillAidBenevolence(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidBenevolence: 未配置Buff短名 技能id={id}");
            return false;
        }

        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x.isHero && x.IsInFight() && !x.HasBuff(buffIds[0]));

        if (allies.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        foreach (var u in allies)
        {
            foreach (var buffId in buffIds)
                BuffManager.AddBuff(u, owner, id, buffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
        }
        GameLog.Info("仁德：" + GetHeroName(owner) + " 使我方 " + allies.Count + " 名英雄提升生命与法力回复");
        return true;
    }

    /// <summary>取英雄显示名（士兵等非英雄单位返回原始描述）</summary>
    private string GetHeroName(Chess chess)
    {
        if (chess == null || !chess.isHero)
            return "单位";
        var cfg = CommonConfig.HeroConfig.GetConfig(chess.heroId);
        return cfg != null ? cfg.Name : "英雄";
    }
}
