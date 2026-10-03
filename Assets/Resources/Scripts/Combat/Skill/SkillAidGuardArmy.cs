using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 李严·护军：为生命比例最低的友军套 Strength2[0]×(100+法强)/100 的护盾（随法强成长），
/// 并嘲讽该友军 Area 范围内的敌人，把它们的当前目标改到自己身上。
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
        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, 0f, owner.side);
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

        // 给最低生命友军套盾（护盾量 = Strength2[0] × (100+法强)/100）
        int shieldId = BuffConfig.GetConfigByNameS("盾").Id;
        BuffManager.AddBuff(lowest, owner, id, shieldId, skillCfg.BuffTime);
        var sh = lowest.GetBuff(shieldId) as BuffShield;
        if (sh != null)
            sh.SetHp(GetSkillShield(0));

        // 嘲讽：把被套盾友军 Area 范围内敌人的当前目标改到自己身上
        TauntEnemies(owner, lowest.transform.position, skillCfg.Area);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}