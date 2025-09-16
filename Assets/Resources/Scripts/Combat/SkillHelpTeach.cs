using System;
using CommonConfig;
using UnityEngine;

public class SkillHelpTeach : Skill
{
    public SkillHelpTeach(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        var unitsInRange = WorldManager.Instance.GetUnitsMySide(owner.transform.position, skillCfg.Range, owner.side);
        Chess unitWithLowestInte = null;
        float lowestInte = float.MaxValue;

        foreach(var unit in unitsInRange)
        {
            if(unit == owner)
                continue;

            if(!unit.isHero)
                continue;

            if(unit.inte < lowestInte)
            {
                lowestInte = unit.inte;
                unitWithLowestInte = unit;
            }
        }

        if(unitWithLowestInte != null && unitWithLowestInte.inte < owner.inte)
        {
            var addon = (owner.inte - unitWithLowestInte.inte) * skillCfg.Strength;
            if(addon < 10)
                addon = 10;
            var newInte = Math.Min(owner.inte, unitWithLowestInte.inte + addon);

            unitWithLowestInte.UpdateAttr((int)newInte, 0, 0);
            EffectManager.PlaySkillEffect(unitWithLowestInte, skillCfg.HitEffect);
        }
    }
}