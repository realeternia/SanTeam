using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAidSuddenArrow : Skill
{
    public SkillAidSuddenArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var unitsInRange = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, true);
        unitsInRange.Remove(owner);

        if (unitsInRange.Count == 0)
            return false;

        if (!CheckBurst(null))
            return false;

        // 关联属性：IsMagic=true 走 ap 法强，否则走 atk 攻击
        var attrKey = skillCfg.IsMagic ? "ap" : "atk";

        //排序，优先给hero，然后优先给属性低的
        unitsInRange.Sort((a, b) =>
        {
            if (a.isHero && !b.isHero)
                return -1;
            if (b.isHero && !a.isHero)
                return 1;
            return a.hp.CompareTo(b.hp);
        });

        var targetUnit = unitsInRange[0];

        owner.PlayerAnim(skillCfg.Action);
        var attrDiff = Math.Max(10, owner.GetAttr(attrKey) - targetUnit.GetAttr(attrKey));
        var damage = (int)(attrDiff * skillCfg.SkillDamageAttrRate);
        WorldManager.Instance.CreateSpellMissile(owner, targetUnit, owner.transform.position, id, damage, skillCfg.HitEffect);

        return true;
    }

}
