using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHeal : Skill
{
    public SkillHeal(int id, Chess unit) : base(id, unit)
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

            UnityEngine.Debug.Log("CheckAidSkill unit.hp=" + unit.hp.ToString() + " unit.maxHp=" + unit.maxHp.ToString() + " addhp=" + ((int)(owner.inte * skillCfg.Strength)).ToString());


            unit.AddHp((int)(owner.inte * skillCfg.Strength));
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
            return true;
        }

        return false;
    }
}
