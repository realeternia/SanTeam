using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 李严·护军：为生命比例最低的友军套 /strength% 最大生命的护盾，并嘲讽其 Area 范围内敌人转移到自己身上（自身挂 BuffTaunt）。
/// </summary>
public class SkillAidGuardArmy : Skill
{
    public SkillAidGuardArmy(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        // 找生命比例最低的友军
        var allies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, 0f, owner.side, false);
        Chess lowest = null;
        foreach (var a in allies)
        {
            if (a == null || a.hp <= 0)
                continue;
            if (lowest == null || a.HpRate < lowest.HpRate)
                lowest = a;
        }
        if (lowest == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 给最低生命友军套盾
        int shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(lowest, owner, id, shieldId, skillCfg.BuffTime);
        var sh = lowest.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp((int)(lowest.maxHp * skillCfg.Strength));

        // 嘲讽：自身挂嘲讽标记，使敌人优先攻击自己
        BuffManager.AddBuff(owner, owner, id, BuffConfig.GetConfigByNameS("嘲").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}