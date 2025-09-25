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
        var unitsInRange = WorldManager.Instance.GetUnitsMySidePosType(owner.side, owner.pos, true, skillCfg.UnitHelpType);
        unitsInRange.Remove(owner);
        foreach (var unit in unitsInRange)
        {
            if(unit.inte > owner.inte)
                continue;

            var addon = (owner.inte - unit.inte) * skillCfg.Strength;
            if (addon < 10)
                addon = 10;
            var newInte = Math.Min(owner.inte, unit.inte + addon);

            unit.UpdateAttr((int)newInte, 0, 0);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}