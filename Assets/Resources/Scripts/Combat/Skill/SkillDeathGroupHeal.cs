using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 背水：阵亡时，回复我方同组技能(Sname)的在场英雄 Strength2[0] 点生命值（固定值，不随最大生命），
    /// 并按 Strength2[1] 比例提升其 atk。通过 Skill.OnDeath 在 Chess.Ondying 时触发。
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

            // 回复目标 Strength2[0] 点生命值（视为治疗，吃治疗加成/可被扩散）
            var heal = (int)skillCfg.Strength2[0];
            if (heal > 0)
                owner.HealTarget(chess, skillId, heal, true);

            // 按 Strength2[1] 比例提升目标 atk
            chess.atk += (int)(chess.atk * skillCfg.Strength2[1]);

            if (chess.heroInfo != null)
                chess.heroInfo.SetAttr(chess.ap, chess.atk);
        }
    }

}