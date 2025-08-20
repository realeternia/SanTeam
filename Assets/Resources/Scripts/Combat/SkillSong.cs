using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillSong : Skill
{
    public SkillSong(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if(!CheckBurst())
            return false;

        var unitsInRange = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false);
        foreach(var unit in unitsInRange)
        {
            if(unit == owner)
                continue;

            if(unit.hp >= unit.maxHp)
                continue;

            unit.Cooldown((int)(owner.inte * skillCfg.Strength) + 0.5f);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            return true;
        }

        return false;
    }
}
