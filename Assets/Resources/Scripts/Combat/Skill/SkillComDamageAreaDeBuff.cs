using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"范围伤害 + 减益"技能，覆盖两种范围中心（由 SkillConfig.TargetType 的 self 标记决定）：
/// · "self" = 自身中心（近战贴脸 AoE，不做射程校验，如张绣·枪出如龙 / 孙坚·破阵 / 刘晔·卸甲）；
/// · 缺省（空值）= 锁定目标中心（远程需先锁定且在 skillCfg.Range 射程内，如张郃·震碎 / 司马昭·篡逆 / 钟会·蓄谋）。
/// 取中心点 skillCfg.Area 半径内的敌人（选人规则见 SelectOnly / SelectFirst，TargetCount&lt;=0 时范围内全部）
/// 造成 GetSkillDamage() 技能伤害，并给每个目标附加 SkillConfig.BuffId 指定的减益 buff
/// （英文/中文逗号或分号分隔可配多个，未配置则纯伤害），时长 BuffTime。
/// 范围特效播在中心点、命中特效播在每个受击目标身上。
///
/// TargetType 仅保留脚本私有行为标记（可与 self 组合，逗号分隔）：
/// · missile    = 以飞弹逐个结算，伤害 ×(1+键"missileRate")（黄忠·乱射）
/// · maxHpBonus = 伤害 += 目标最大生命 × 键"maxHpBonusRate"（文丑·陷阵怒吼）
/// · mpBurn     = 每个命中目标燃烧 键"mpBurn" 点法力（贾诩·乱阵；基类 ApplyMpBurn 统一实现）
/// 另可复用基类的通用伤害组件（execute/shieldBonus/missingHp/vsJob/soldierDamage）。
/// </summary>
public class SkillComDamageAreaDeBuff : Skill
{
    public SkillComDamageAreaDeBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 中心目标同样走 SelectOnly / SelectFirst；self 中心（近战贴脸 AoE）不做射程校验，沿用当前索敌目标
        var selfCenter = HasSkillFlag("self");
        var target = selfCenter ? owner.targetChess : SelectSingleTarget(skillCfg.Range);
        if (target == null || target.hp <= 0)
            return false;

        if (!CheckBurst(target))
            return false;

        var center = selfCenter ? owner.transform.position : target.transform.position;
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(center);

        var units = SelectTargets(center, skillCfg.Area, false, skillCfg.TargetCount);

        // 减益 buff 由 SkillConfig.BuffId 短名指定（如"缓"=减速、"叛"=叛逃），支持多个（如"慑,疫"），不在战斗代码硬编码 BuffId
        var debuffIds = GetSkillBuffIds();
        var useMissile = HasSkillFlag("missile");
        var maxHpBonus = HasSkillFlag("maxHpBonus");

        var skillDamage = GetSkillDamage();
        foreach (var u in units)
        {
            var damage = skillDamage;
            if (useMissile)
                damage = (int)(damage * (1f + GetStr2("missileRate")));
            if (maxHpBonus)
                damage += (int)(GetStr2("maxHpBonusRate") * u.maxHp);
            // 通用伤害组件：execute/shieldBonus/missingHp/vsJob/soldierDamage（未配置则无影响）
            damage = ApplyDamageModifiers(damage, u);

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
            // 法力燃烧：每个命中目标每次释放只结算一次
            ApplyMpBurn(u);
            foreach (var debuffId in debuffIds)
                ApplyBuffToUnit(u, debuffId);
        }
        return true;
    }
}
