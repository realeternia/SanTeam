using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 忠勇·自+友军护盾（ScriptName = "AidSelfAndLowHp"）：
/// 辅助技能，给自己施加一个吸收盾（容量=自身最大生命×Strength），
/// 同时给附近(Range内)生命比例最低的一名友方英雄也施加一个吸收盾。
/// 若附近无友方英雄则只给自己加盾。通过 Skill.CheckAidSkill 由 SkillManager 循环施放。
/// 用于周仓：挨打时本能举盾，顺带护住身旁同袍。
/// </summary>
public class SkillAidSelfAndLowHp : Skill
{
    public SkillAidSelfAndLowHp(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var shieldId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        var shieldHp = (int)(owner.maxHp * skillCfg.Strength);

        owner.PlayerAnim(skillCfg.Action);

        // 给自己加盾
        BuffManager.AddBuff(owner, owner, id, shieldId, skillCfg.BuffTime);
        var selfShield = owner.GetBuff(shieldId) as BuffShield;
        if (selfShield != null)
            selfShield.SetHp(shieldHp);

        // 找附近生命比例最低的友军
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        Chess target = null;
        float lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (u.hp >= u.maxHp)
                continue;
            var rate = u.hp / (float)u.maxHp;
            if (rate < lowest)
            {
                lowest = rate;
                target = u;
            }
        }

        if (target != null)
        {
            BuffManager.AddBuff(target, owner, id, shieldId, skillCfg.BuffTime);
            var allyShield = target.GetBuff(shieldId) as BuffShield;
            if (allyShield != null)
                allyShield.SetHp(shieldHp);

            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"忠勇护盾 技能id={id} 等级={Level} 自护盾={shieldHp} 友军={target?.heroId}");
        return true;
    }
}
