using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 孟获·蛮锤：纵锤砸地，对自身 Area 范围内的敌人造成法术伤害并移除其吸收型护盾（对带 BuffShield 的敌人触发移除）。
/// </summary>
public class SkillAidBarbarianSlam : Skill
{
    public SkillAidBarbarianSlam(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        var list = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var enemy in list)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            enemy.OnSkillDamaged(owner, id, GetSkillDamage());
            // 移除吸收型护盾
            var shield = enemy.GetBuff(CombatConst.ShieldBuffId) as BuffShield;
            if (shield != null)
            {
                shield.SetHp(0);
                BuffManager.RemoveBuff(enemy, CombatConst.ShieldBuffId);
            }
        }

        PlayAreaEffect(owner.transform.position);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}