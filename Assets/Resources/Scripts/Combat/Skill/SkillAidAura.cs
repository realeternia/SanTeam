using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 光环主动助战（ScriptName = "AidAura"）：保留 AuroAttrs 开局被动光环，
/// 施放时对本侧全体英雄（含自身）再永久累加 光环属性值 × Strength2[0]（Strength2[0] 即百分比 x%）。
/// 复用 JobLinkManager.ParseBonuses/ApplyAttr 与光环被动同一套解析/施加逻辑。
/// 另每次施放为自身永久叠加成长属性（数值取 StrengthBuff1[0]）：光环主属性为法强(ap) 时叠加自身法强，
/// 否则叠加自身攻击(atk)，使纯光环辅助在单兵情况下也能逐步成长为可独立作战的单位。
/// 每次施放还会驱散我方全体英雄身上的负面 Buff（不可驱散的招牌 Buff 除外，见 BuffManager.DispelNegative）。
/// 由 SkillManager.CheckAidSkill 经 Skill.CheckAidSkill 自动按 CD 周期性施放。
/// </summary>
public class SkillAidAura : Skill
{
    /// <summary>自身成长槽位（StrengthBuff1 数组下标）</summary>
    private const int SelfGrowIdx = 0;

    public SkillAidAura(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var allMyHeroes = WorldManager.Instance.GetUnitsMySide(owner.side)
            .FindAll(x => x.isHero);   // 我方全体英雄（含自身）

        var bonuses = JobLinkManager.ParseBonuses(skillCfg.AuroAttrs);
        foreach (var unit in allMyHeroes)
        {
            foreach (var bonus in bonuses)
                JobLinkManager.ApplyAttr(unit, bonus.Attr, bonus.Value * skillCfg.Strength2[0]);
        }

        // 自身永久成长：光环主属性为法强则叠法强，否则叠攻击（数值取自 StrengthBuff1[0]）
        if (skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length > SelfGrowIdx)
        {
            var growAttr = (bonuses.Count > 0 && bonuses[0].Attr == "ap") ? "ap" : "atk";
            JobLinkManager.ApplyAttr(owner, growAttr, skillCfg.StrengthBuff1[SelfGrowIdx]);
        }

        // 每次释放驱散我方全体英雄（含自身）身上的负面 Buff（不可驱散的招牌 Buff 除外）
        foreach (var unit in allMyHeroes)
            BuffManager.DispelNegative(unit);

        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}
