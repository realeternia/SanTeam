using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 刘备·仁德：使我方范围内全体英雄附加"仁政"Buff（提升 hpRegen/mpRegen 回复速度），不设人数上限。
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

        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error("SkillAidBenevolence: 未找到仁政Buff短名：" + skillCfg.BuffId);
            return false;
        }

        var allies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x.isHero && x.IsInFight() && !x.HasBuff(buffCfg.Id));

        if (allies.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        foreach (var u in allies)
        {
            BuffManager.AddBuff(u, owner, id, buffCfg.Id, skillCfg.BuffTime);
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
