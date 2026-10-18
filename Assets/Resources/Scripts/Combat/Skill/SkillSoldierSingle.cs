using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"单个士兵增益"技能（ScriptName = "SkillSoldierSingle"）：
/// 从本侧存活士兵中选 1 名，按配置回复满血并/或附加 Buff（护盾类自动写入护盾值）。
///
/// TargetType 标记：
/// · meleeOnly    = 只选近战士兵（attackRange &lt; CombatConst.MeleeRange，蒋琬·抚军）
/// · lowestHp     = 选生命比例最低的士兵（张昭·筑垒），缺省随机
/// · fullHeal     = 先回复满血（张昭·筑垒）
/// · selfFallback = 场上无士兵时改为对自身施放（张昭·筑垒）
/// </summary>
public class SkillSoldierSingle : Skill
{
    public SkillSoldierSingle(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var meleeOnly = HasSkillFlag("meleeOnly");
        var lowestHp = HasSkillFlag("lowestHp");
        var soldiers = WorldManager.Instance.GetUnitsMySide(owner.side)
            .FindAll(x => !x.isHero && (!meleeOnly || x.attackRange < CombatConst.MeleeRange));

        Chess target = null;
        if (lowestHp)
        {
            foreach (var s in soldiers)
                if (target == null || s.HpRate < target.HpRate)
                    target = s;
        }
        else if (soldiers.Count > 0)
        {
            target = soldiers[SysRandom.Range(0, soldiers.Count)];
        }

        owner.PlayerAnim(skillCfg.Action);

        if (target == null)
        {
            if (!HasSkillFlag("selfFallback"))
            {
                PlaySelfEffect();
                return true;
            }
            target = owner;
        }

        if (HasSkillFlag("fullHeal") && target.hp < target.maxHp)
            owner.HealTarget(target, skillId, target.maxHp - target.hp, false);

        foreach (var buffId in GetSkillBuffIds())
        {
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
            var shield = target.GetBuff(buffId) as BuffShield;
            if (shield != null)
                shield.SetHp(GetSkillShield(0));
        }

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        PlaySelfEffect();
        return true;
    }
}
