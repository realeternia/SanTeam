using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"范围伤害 + 减益"技能，覆盖两种范围中心（由 SkillConfig.TargetType 决定）：
/// · "self" = 自身中心（近战贴脸 AoE，不做射程校验，如张绣·枪出如龙 / 孙坚·破阵 / 刘晔·卸甲）；
/// · 缺省（空值）= 锁定目标中心（远程需先锁定且在 skillCfg.Range 射程内，如张郃·震碎 / 司马昭·篡逆 / 钟会·蓄谋）。
/// 取中心点 skillCfg.Area 半径内敌人（TargetCount&gt;0 时至多随机 N 名，&lt;=0 时范围内全部）造成 GetSkillDamage()
/// 技能伤害，并给每个目标附加 SkillConfig.BuffId 指定的减益 buff（英文/中文逗号或分号分隔可配多个，未配置则纯伤害），
/// 时长 BuffTime。范围特效播在中心点、命中特效播在每个受击目标身上。
///
/// TargetType 标记扩展（可与 self 组合，逗号分隔）：
/// · missile    = 以飞弹逐个结算，伤害 ×(1+Strength2[0])（黄忠·乱射）
/// · maxHpBonus = 伤害 += 目标最大生命 × Strength2[0]（文丑·陷阵怒吼）
/// </summary>
public class SkillAreaDamageDeBuff : Skill
{
    public SkillAreaDamageDeBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;

        // 中心点：TargetType="self"=自身中心（无需射程校验）；缺省=锁定目标中心（需目标在 Range 射程内，Range<=0 视为不限射程）
        var selfCenter = HasSkillFlag("self");
        if (!selfCenter && skillCfg.Range > 0f && !WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        var center = selfCenter ? owner.transform.position : target.transform.position;
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(center);

        var units = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
        // 命中数上限：TargetCount>0 时随机保留 N 名（同落雷/火海）；<=0 表示不限，避免 RandomSelect(0) 清空列表
        if (skillCfg.TargetCount > 0)
            WorldManager.Instance.RandomSelect(units, skillCfg.TargetCount);

        // 减益 buff 由 SkillConfig.BuffId 短名指定（如"缓"=减速、"叛"=叛逃），支持多个（如"慑,疫"），不在战斗代码硬编码 BuffId
        var debuffIds = GetSkillBuffIds();
        var useMissile = HasSkillFlag("missile");
        var maxHpBonus = HasSkillFlag("maxHpBonus");

        var skillDamage = GetSkillDamage();
        foreach (var u in units)
        {
            var damage = skillDamage;
            if (useMissile)
                damage = (int)(damage * (1f + GetStrength2(0)));
            if (maxHpBonus)
                damage += (int)(GetStrength2(0) * u.maxHp);

            if (damage > 0)
            {
                if (useMissile)
                    WorldManager.Instance.CreateSpellMissile(owner, u, owner.transform.position, id, damage, skillCfg.HitEffect);
                else
                {
                    u.OnSkillDamaged(owner, skillId, damage);
                    if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                        EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
                }
            }
            foreach (var debuffId in debuffIds)
                BuffManager.AddBuff(u, owner, id, debuffId, skillCfg.BuffTime);
        }
        return true;
    }
}
