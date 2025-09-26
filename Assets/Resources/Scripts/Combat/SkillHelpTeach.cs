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
            var targetAttr = unit.GetAttr(skillCfg.Attr);
            var ownerAttr = owner.GetAttr(skillCfg.Attr);
            if(targetAttr > ownerAttr)
                continue;

            var addon = (ownerAttr - targetAttr) * skillCfg.Strength;
            if (addon < 10)
                addon = 10;
            var newAttr = Math.Min(ownerAttr, targetAttr + addon);

            unit.UpdateAttr((int)newAttr, 0, 0);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}