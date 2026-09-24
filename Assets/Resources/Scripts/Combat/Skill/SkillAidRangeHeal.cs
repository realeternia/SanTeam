using CommonConfig;
using UnityEngine;

/// <summary>
/// 青囊（ScriptName = "AidRangeHeal"）：辅助技能，先在 Range 内选出生命比例最低的友方英雄作为治疗中心，
/// 再以该中心为圆心、Area 为半径，治疗范围内全部友方英雄（各 HealTarget 一次，吃医职业治疗加成）。
/// 治疗量走独立治疗公式 GetSkillHeal()。附近友军全部满血时不施放。
/// 用于华佗：一剂青囊，药到病除，奶一圈。
/// </summary>
public class SkillAidRangeHeal : Skill
{
    public SkillAidRangeHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 第一步：在助战射程内选治疗中心（生命比例最低的友方英雄）
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x.isHero && x.IsInFight() && x.hp < x.maxHp);

        Chess center = null;
        var lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (u.HpRate < lowest)
            {
                lowest = u.HpRate;
                center = u;
            }
        }
        if (center == null)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 第二步：以治疗中心为圆心，治疗范围内全部友方英雄
        var healTargets = WorldManager.Instance.GetUnitsInRange(center.transform.position, skillCfg.Area, owner.side, false)
            .FindAll(x => x.isHero && x.hp < x.maxHp);

        var heal = GetSkillHeal();
        foreach (var unit in healTargets)
        {
            if (heal > 0)
                owner.HealTarget(unit, skillId, heal, true);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }

        GameLog.Debug($"青囊 技能id={id} 等级={Level} 治疗={heal} 中心={center.heroId} 目标数={healTargets.Count}");
        return true;
    }
}
