using System;
using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 门阀（名门望族）：战斗开始为自身挂「望族」Buff，每3秒衰减1/5，共5跳、15秒后归零。
/// 衰减由协程驱动 BuffDecayDef.DecayOnce()；Buff 按 BuffTime(15s) 到期自动移除。
/// </summary>
public class SkillInitDecayDef : Skill
{
    public SkillInitDecayDef(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        if (string.IsNullOrEmpty(skillCfg.BuffId))
        {
            GameLog.Error($"门阀 武将{owner.heroId} BuffId未配置");
            return;
        }
        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        owner.StartCoroutine(DecayLoop(buffId));
    }

    private IEnumerator DecayLoop(int buffId)
    {
        for (var i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(CombatConst.FriendDecayInterval);
            if (owner == null || owner.hp <= 0)
                yield break;
            var buff = owner.GetBuff(buffId) as BuffDecayDef;
            if (buff == null)
                yield break;
            buff.DecayOnce();
            GameLog.Debug($"门阀 武将{owner.heroId} 第{i + 1}跳递减完成");
        }
    }
}