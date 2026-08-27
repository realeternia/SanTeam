using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

public class SkillModifyBuffTime : Skill
{
    public SkillModifyBuffTime(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAddBuff(Chess target, ref int buffId, int checkSkillId, ref float time)
    {
        UnityEngine.Debug.Log($"SkillModifyBuffTime OnAddBuff {target.name} {buffId} {checkSkillId} {time}");
        if (checkSkillId == skillId)
            return; //自己挂的buff，不再连续触发

        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(SkillConfig.GetConfig(checkSkillId).Attr))
            return;

        var buffCfg = BuffConfig.GetConfig(buffId);
        if (buffCfg.IsPositive == skillCfg.NegBuff)
            return;

        if (skillCfg.BuffId > 0 && buffId != skillCfg.BuffId) //为强化id
            return;

        time *= (1 + skillCfg.Strength);
        WorldManager.Instance.AddBattleText(skillCfg.Name, owner.transform.position, new UnityEngine.Vector2(0, 60), new Color(1, 0.9f, 0.1f), 3);
    }

}
