using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 背水：阵亡时，回复我方同组技能(Sname)的在场英雄 20%~60%(Strength)最大生命值，
    /// 并按同比例(Strength)提升其 atk、按 StrengthInt 提升其 ap。通过 Skill.OnDeath 在 Chess.Ondying 时触发。
/// </summary>
public class SkillDeathGroupHeal : Skill
{
    public SkillDeathGroupHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnDeath()
    {
        var sname = skillCfg.Sname;
        foreach (var chess in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (chess == owner || !chess.isHero || chess.hp <= 0)
                continue;
            // 只作用于携带同组技能(Sname)的英雄
            if (chess.skills.Find(s => s.skillCfg.Sname == sname) == null)
                continue;

            // 回复目标 20%~60% 最大生命值（视为治疗，吃治疗加成/可被扩散）
            var heal = (int)(chess.maxHp * skillCfg.Strength);
            if (heal > 0)
                owner.HealTarget(chess, skillId, heal, true);

            // 提升目标 20%~40% atk 与 ap
            chess.atk += (int)(chess.atk * skillCfg.Strength);
            chess.ap += skillCfg.StrengthInt;
            if (chess.heroInfo != null)
                chess.heroInfo.SetAttr(chess.ap, chess.atk);
        }
    }

}