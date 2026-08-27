using System;
using System.Buffers;
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

        var skillAttr = owner.GetAttr(skillCfg.Attr);

        //排序，优先给hero，然后优先给生命值低的
        unitsInRange.Sort((a, b) =>
        {
            if (a.isHero && !b.isHero)
                return -1;
            if (b.isHero && !a.isHero)
                return 1;
            return a.GetAttr(skillCfg.Attr).CompareTo(b.GetAttr(skillCfg.Attr));
        });

        var targetUnit = unitsInRange[0];

        owner.PlayerAnim(skillCfg.Action);
        var attrDiff = Math.Max(10, owner.GetAttr(skillCfg.Attr) - targetUnit.GetAttr(skillCfg.Attr));
        var damage = (int)(attrDiff * skillCfg.SkillDamageAttrRate);
        WorldManager.Instance.CreateSpellMissile(owner, targetUnit, owner.transform.position, id, damage, skillCfg.HitEffect);

        return true;
    }

}
