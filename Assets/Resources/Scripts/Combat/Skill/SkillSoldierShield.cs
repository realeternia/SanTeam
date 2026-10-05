using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 蒋琬·抚军：每次主动释放给1名近战士兵套护盾（单目标）。
/// 护盾 = Strength2[0] × (100+法强)/100（随法强成长），复用 BuffShield。
/// </summary>
public class SkillSoldierShield : Skill
{
    public SkillSoldierShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        var melee = GetMeleeSoldiers();
        if (melee.Count > 0)
            ShieldSoldier(melee[SysRandom.Range(0, melee.Count)]);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }

    private void ShieldSoldier(Chess s)
    {
        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(s, owner, id, buffId, skillCfg.BuffTime);
        var shield = s.GetBuff(buffId) as BuffShield;
        if (shield != null)
            shield.SetHp(GetSkillShield(0));
    }

    // 本侧存活近战士兵（近战判定沿用 CombatConst.MeleeRange）
    private List<Chess> GetMeleeSoldiers()
    {
        return WorldManager.Instance.GetUnitsMySide(owner.side)
            .Where(x => !x.isHero && x.attackRange < CombatConst.MeleeRange).ToList();
    }
}