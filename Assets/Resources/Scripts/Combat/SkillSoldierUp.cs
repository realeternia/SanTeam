using System;
using CommonConfig;
using UnityEngine;

public class SkillSoldierUp : Skill
{
    public SkillSoldierUp(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        UnityEngine.Debug.Log("SkillHelp BattleBegin");

        var unitsInRange = WorldManager.Instance.GetUnitsMySide(owner.transform.position, skillCfg.Range, owner.side);

        foreach(var unit in unitsInRange)
        {
            if(unit.isHero)
                continue;

            unit.AddSoldierLevel((int)skillCfg.Strength);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        owner.GetPlayerInfo().castleHUD.AddSoldierLevel((int)skillCfg.Strength);
    }
}