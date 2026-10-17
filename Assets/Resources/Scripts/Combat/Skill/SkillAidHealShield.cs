using CommonConfig;
using UnityEngine;

/// <summary>
/// 安乐（ScriptName = "SkillAidHealShield"）：辅助技能，治疗范围内生命比例最低的友方英雄（含自身），
/// 治疗量走独立治疗公式 GetSkillHeal()（不套用伤害公式）；治疗量超过目标生命缺口时，
/// 溢出的部分转化为等量护甲（BuffId="甲"），护甲持续 BuffTime 秒。
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
        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
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
        PlaySelfEffect();

        // 治疗：独立治疗公式（Strength2 × ap 加成），不套用伤害公式
        var heal = GetSkillHeal();
        // 溢出：治疗量超出目标生命缺口的差额，转为护甲值（按技能基础治疗量估算）
        var overflow = Mathf.Max(0, heal - (target.maxHp - target.hp));
        if (heal > 0)
            owner.HealTarget(target, skillId, heal, true);

        // 有溢出才加护甲：数值 = 溢出治疗量
        if (overflow > 0)
        {
            var buffIds = GetSkillBuffIds();
            if (buffIds.Count < 1)
            {
                GameLog.Error($"安乐 武将{owner.heroId} BuffId未配置");
            }
            else
            {
                BuffManager.AddBuff(target, owner, id, buffIds[0], skillCfg.BuffTime);
                var armorBuff = target.GetBuff(buffIds[0]) as BuffArmorAdd;
                if (armorBuff != null)
                    armorBuff.SetArmor(overflow);
                else
                    GameLog.Error($"安乐 需要护甲Buff(甲)，当前 BuffId={skillCfg.BuffId} 技能id={id}");
            }
        }

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        GameLog.Debug($"安乐 技能id={id} 等级={Level} 治疗={heal} 溢出护甲={overflow} 目标={target.heroId}");
        return true;
    }
}
