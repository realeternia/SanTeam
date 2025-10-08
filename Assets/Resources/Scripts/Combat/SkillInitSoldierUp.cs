using System;
using CommonConfig;
using UnityEngine;

public class SkillInitSoldierUp : Skill
{
    public SkillInitSoldierUp(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        UnityEngine.Debug.Log("SkillInitSoldierUp BattleBegin");

        var unitsInRange = WorldManager.Instance.GetUnitsMySide(owner.transform.position, skillCfg.Range, owner.side);

        foreach(var unit in unitsInRange)
        {
            if(unit.isHero)
                continue;

            unit.AddSoldierLevel(skillCfg.StrengthInt);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        var castleHUD = owner.GetPlayerInfo().castleHUD;
        if(castleHUD != null)
            castleHUD.AddSoldierLevel(skillCfg.StrengthInt);
    }
}