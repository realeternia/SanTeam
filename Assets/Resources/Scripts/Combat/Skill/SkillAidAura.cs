using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 光环主动助战（ScriptName = "AidAura"）：保留 AuroAttrs 开局被动光环，
/// 施放时对本侧全体英雄（含自身）再永久累加 光环属性值 × Strength（Strength 即百分比 x%）。
/// 复用 JobLinkManager.ParseBonuses/ApplyAttr 与光环被动同一套解析/施加逻辑。
/// 由 SkillManager.CheckAidSkill 经 Skill.CheckAidSkill 自动按 CD 周期性施放。
/// </summary>
public class SkillAidAura : Skill
{
    public SkillAidAura(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var allMyHeroes = WorldManager.Instance.GetUnitsMySide(owner.side)
            .FindAll(x => x.isHero);   // 我方全体英雄（含自身）

        foreach (var unit in allMyHeroes)
        {
            foreach (var bonus in JobLinkManager.ParseBonuses(skillCfg.AuroAttrs))
                JobLinkManager.ApplyAttr(unit, bonus.Attr, bonus.Value * skillCfg.Strength);
        }

        owner.PlayerAnim(skillCfg.Action);
        return true;
    }
}