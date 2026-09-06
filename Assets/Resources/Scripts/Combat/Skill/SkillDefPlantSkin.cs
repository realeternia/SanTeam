using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

public class SkillDefPlantSkin : Skill
{
    public SkillDefPlantSkin(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        var isMagic = damType == "ap";
        if (!TypeMatched(skillCfg, isMagic))
        {
            WorldManager.Instance.AddBattleText("弱点", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            damageMulti += skillCfg.Strength;
        }
        else if (CheckBurst(attacker))
        {
            WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
            damageMulti -= skillCfg.Strength;
        }
    }

    public override void OnBeDoSkillDamage(Chess caster, SkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        if(isFeedback)
            return;

        if (!TypeMatched(skillCfg, checkSkillCfg.IsMagic))
        {
            WorldManager.Instance.AddBattleText("弱点", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            damage = (int)(damage * (1 + skillCfg.Strength));
        }
        else if (CheckBurst(caster))
        {
            WorldManager.Instance.AddBattleText("抵抗", owner.transform.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
            damage = (int)(damage * (1 - skillCfg.Strength));
        }
    }    

}
