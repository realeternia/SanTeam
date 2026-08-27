using System;
using CommonConfig;
using UnityEngine;

public class SkillHelpInitTeach : Skill
{
    public SkillHelpInitTeach(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        var unitsInRange = WorldManager.Instance.GetUnitsMySidePosType(owner.side, owner.pos, true, skillCfg.UnitHelpType);
        unitsInRange.Remove(owner);
        owner.PlayerAnim(skillCfg.Action);

        foreach (var unit in unitsInRange)
        {
            var targetAttr = unit.GetAttr(skillCfg.Attr);
            var ownerAttr = owner.GetAttr(skillCfg.Attr);
            if(targetAttr > ownerAttr)
                continue;

            var addon = (ownerAttr - targetAttr) * skillCfg.SkillAttrRate;
            if (addon < 10)
                addon = 10;
            var newAttr = Math.Min(ownerAttr, targetAttr + addon);

            unit.AddAttr(skillCfg.Attr, (int)(newAttr - targetAttr));
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}