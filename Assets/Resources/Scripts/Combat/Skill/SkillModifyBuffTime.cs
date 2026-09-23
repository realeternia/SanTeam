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
        GameLog.Debug($"SkillModifyBuffTime OnAddBuff {target.name} {buffId} {checkSkillId} {time}");
        if (checkSkillId == skillId)
            return; //自己挂的buff，不再连续触发

        var buffCfg = BuffConfig.GetConfig(buffId);
        if (buffCfg.IsPositive == skillCfg.NegBuff)
            return;

        if (!string.IsNullOrEmpty(skillCfg.BuffId) && buffId != BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id) //为强化id
            return;

        time *= (1 + skillCfg.Strength);
        WorldManager.Instance.AddBattleText(skillCfg.Name, owner.transform.position, new UnityEngine.Vector2(0, 60), SysColor.BattleText.SkillName, 3);
    }

}
