using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAidShockWave : Skill
{
    private Vector3 targetPos;
    public SkillAidShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if(IsInCD())
            return false;

        if (owner.targetChess == null)
            return false;

        if (!WorldManager.Instance.CheckInRange(owner.transform.position, owner.targetChess.transform.position, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        this.targetPos = owner.targetChess.transform.position; // 使用目标位置而不是自身位置

        owner.PlayerAnim(skillCfg.Action);
        var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, skillCfg.SummonTime, skillCfg.SummonSpeed, skillCfg.EffectSize, skillCfg.Id, damage, skillCfg.HitEffect);

        return true;
    }

}
