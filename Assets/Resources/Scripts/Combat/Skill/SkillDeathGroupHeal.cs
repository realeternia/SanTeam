using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 背水：阵亡时，回复我方同组技能(Sname)的在场英雄 键 heal 点生命值（固定值，不随最大生命），
    /// 并按 键 atkUp 比例提升其 atk。通过 Skill.OnDeath 在 Chess.Ondying 时触发。
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

            // 回复目标 键 heal 点生命值（视为治疗，吃治疗加成/可被扩散）
            var heal = (int)GetStr2("heal");
            if (heal > 0)
                owner.HealTarget(chess, skillId, heal, true);

            // 按 键 atkUp 比例提升目标 atk
            chess.atk += (int)(chess.atk * GetStr2("atkUp"));

            if (chess.heroInfo != null)
                chess.heroInfo.SetAttr(chess.ap, chess.atk);
        }
    }

}