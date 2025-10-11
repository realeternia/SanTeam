using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillBuffTieqi : Skill
{
    public SkillBuffTieqi(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAddBuff(Chess target, ref int buffId, int checkSkillId, ref float time)
    {
        UnityEngine.Debug.Log($"SkillModifyTieqi OnAddBuff {target.name} {buffId} {checkSkillId} {time}");
        if(checkSkillId == skillId)
            return; //自己挂的buff，不再连续触发

        var checkSkillCfg = SkillConfig.GetConfig(checkSkillId);
        if(checkSkillCfg.Sname != "马" && checkSkillCfg.Sname != "车")
            return;

        buffId = skillCfg.BuffId;
        time = skillCfg.BuffTime;

        WorldManager.Instance.AddBattleText(skillCfg.Name, owner.transform.position, new UnityEngine.Vector2(0, 60), new Color(1, 0.9f, 0.1f), 3);
    }
  
}
