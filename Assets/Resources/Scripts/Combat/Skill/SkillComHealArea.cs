using CommonConfig;
using UnityEngine;

/// <summary>
/// 青囊 / 天书（ScriptName = "SkillComHealArea"）：辅助技能，先在 Area 内按 SelectOnly / SelectFirst
/// （典型配置 SelectOnly="ally;hero;notFullHp"、SelectFirst="lowestHp"）选出治疗中心，
/// 再以该中心为圆心、Area 为半径按同样规则最多治疗 TargetCount 名友方英雄（各 HealTarget 一次，吃医职业治疗加成）。
/// 治疗量走独立治疗公式 GetSkillHeal()。附近友军全部满血时不施放。
/// 并对被治疗的友军额外驱散负面 Buff（不可驱散的招牌 Buff 除外，见 BuffManager.DispelNegative）；
/// BuffId 非空时还给这些目标附加对应 Buff（持续 BuffTime 秒），用于左慈·天书（范围回血 + 挂闪避）。
///
/// TargetType 仅保留脚本私有行为标记：
/// · overflowToArmor = 溢出的治疗量转为等量护甲（BuffId 需配"甲"），且不再附带驱散（刘禅·安乐）
/// </summary>
public class SkillComHealArea : Skill
{
    public SkillComHealArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 第一步：在助战射程内选治疗中心（SelectFirst 优先级最高的友方英雄，典型为生命比例最低）
        var first = SelectTargets(owner.transform.position, skillCfg.Area, true, 1);
        var center = first.Count > 0 ? first[0] : null;
        if (center == null)
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 第二步：以治疗中心为圆心，按同样规则选出被治疗的友方英雄
        var healTargets = SelectTargets(center.transform.position, skillCfg.Area, true, skillCfg.TargetCount);

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
        var overflowToArmor = HasSkillFlag("overflowToArmor");
        PlayAreaEffect(center.transform.position);
        foreach (var unit in healTargets)
        {
            // 溢出治疗量 = 治疗量 - 目标生命缺口（按技能基础治疗量估算）
            var overflow = overflowToArmor ? Mathf.Max(0, heal - (unit.maxHp - unit.hp)) : 0;
            if (heal > 0)
                owner.HealTarget(unit, skillId, heal, true);

            if (overflowToArmor)
            {
                // 溢出治疗转护甲：护甲值 = 溢出量
                if (overflow > 0 && buffId > 0)
                {
                    BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
                    // 共存模式(StackMode=1)下同 Id 可能已有旧实例，取本次新加的最后一个
                    var armorBuff = unit.GetLastBuff(buffId) as BuffArmorAdd;
                    if (armorBuff != null)
                        armorBuff.SetArmor(overflow);
                }
            }
            else
            {
                // 附带净化：驱散被治疗友军的负面 Buff
                BuffManager.DispelNegative(unit);
                if (buffId > 0)
                    BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            }
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }

        GameLog.Debug($"{skillCfg.Name} 技能id={id} 等级={Level} 治疗={heal} 中心={center.heroId} 目标数={healTargets.Count} BuffId={skillCfg.BuffId}");
        return true;
    }
}
