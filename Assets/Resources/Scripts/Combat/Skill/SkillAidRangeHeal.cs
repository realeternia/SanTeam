using CommonConfig;
using UnityEngine;

/// <summary>
/// 青囊 / 天书（ScriptName = "AidRangeHeal"）：辅助技能，先在 Range 内选出生命比例最低的友方英雄作为治疗中心，
/// 再以该中心为圆心、Area 为半径，按生命比例从低到高最多治疗 TargetCount 名友方英雄（各 HealTarget 一次，吃医职业治疗加成）。
/// 治疗量走独立治疗公式 GetSkillHeal()。附近友军全部满血时不施放。
/// 并对被治疗的友军额外驱散负面 Buff（不可驱散的招牌 Buff 除外，见 BuffManager.DispelNegative）；
/// BuffId 非空时还给这些目标附加对应 Buff（持续 BuffTime 秒），用于左慈·天书（范围回血 + 挂闪避）。
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
        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Range, owner.side)
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

        // 第二步：以治疗中心为圆心，治疗范围内友方英雄
        var healTargets = WorldManager.Instance.GetMySideInRange(center.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x.isHero && x.hp < x.maxHp);

        // 目标数上限：按生命比例从低到高取前 TargetCount 名（0/负数表示不限，保留原“范围内全奶”行为）
        if (skillCfg.TargetCount > 0 && healTargets.Count > skillCfg.TargetCount)
        {
            healTargets.Sort((a, b) => a.HpRate.CompareTo(b.HpRate));
            healTargets.RemoveRange(skillCfg.TargetCount, healTargets.Count - skillCfg.TargetCount);
        }

        // 可选挂 buff：配置了 BuffId 时给被治疗目标附加（如左慈·天书的"闪"）
        int buffId = 0;
        if (!string.IsNullOrEmpty(skillCfg.BuffId))
        {
            var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
            if (buffCfg == null)
                GameLog.Error($"AidRangeHeal 技能id={id} BuffId[{skillCfg.BuffId}] 未找到对应 BuffConfig，本次不挂 buff");
            else
                buffId = buffCfg.Id;
        }

        var heal = GetSkillHeal();
        PlayAreaEffect(center.transform.position);
        foreach (var unit in healTargets)
        {
            if (heal > 0)
                owner.HealTarget(unit, skillId, heal, true);
            // 附带净化：驱散被治疗友军的负面 Buff
            BuffManager.DispelNegative(unit);
            if (buffId > 0)
                BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }

        GameLog.Debug($"{skillCfg.Name} 技能id={id} 等级={Level} 治疗={heal} 中心={center.heroId} 目标数={healTargets.Count} BuffId={skillCfg.BuffId}");
        return true;
    }
}
