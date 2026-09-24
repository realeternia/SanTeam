using CommonConfig;
using UnityEngine;

/// <summary>
/// 安乐（ScriptName = "AidHealShield"）：辅助技能，治疗范围内生命比例最低的友方英雄（含自身），
/// 治疗量走独立治疗公式 GetSkillHeal()（不套用伤害公式）；治疗量超过目标生命缺口时，
/// 溢出的部分转化为等量吸收盾（BuffId="盾"），护盾持续 BuffTime 秒。
/// 附近友军全部满血时不施放（省一次 CD）。由 SkillManager.CheckAidSkill 自动循环施放。
/// 用于刘禅：没本事，但总有人护着、替人挡灾。
/// </summary>
public class SkillAidHealShield : Skill
{
    public SkillAidHealShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 找范围内生命比例最低的友方英雄（含自身），全部满血则不施放
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x.isHero && x.IsInFight());

        Chess target = null;
        var lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (u.hp >= u.maxHp)
                continue;
            if (u.HpRate < lowest)
            {
                lowest = u.HpRate;
                target = u;
            }
        }
        if (target == null)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 治疗：独立治疗公式（Strength × ap 加成），不套用伤害公式
        var heal = GetSkillHeal();
        // 溢出：治疗量超出目标生命缺口的差额，转为护盾容量（按技能基础治疗量估算）
        var overflow = Mathf.Max(0, heal - (target.maxHp - target.hp));
        if (heal > 0)
            owner.HealTarget(target, skillId, heal, true);

        // 有溢出才附盾：容量 = 溢出治疗量
        if (overflow > 0)
        {
            var shieldBuffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
            BuffManager.AddBuff(target, owner, id, shieldBuffId, skillCfg.BuffTime);
            var shield = target.GetBuff(shieldBuffId) as BuffShield;
            if (shield != null)
                shield.SetHp(overflow);
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        GameLog.Debug($"安乐 技能id={id} 等级={Level} 治疗={heal} 溢出护盾={overflow} 目标={target.heroId}");
        return true;
    }
}
