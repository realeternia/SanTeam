using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用"范围伤害 + 可选 Buff"技能（ScriptName = "SkillComDamageArea"）：
/// 取中心点 skillCfg.Area 半径内的敌人（选人规则见 SelectOnly / SelectFirst，TargetCount&lt;=0 时范围内全部）
/// 造成技能伤害；每个目标再按 SkillConfig.BuffId 挂 Buff（英文/中文逗号或分号分隔可配多个，未配置则纯伤害），时长 BuffTime。
/// 范围特效播在中心点、命中特效播在每个受击目标身上。
/// · centerDouble = 锁定目标（首个目标）承受 centerRate 倍伤害（袁绍·名门），其余目标正常伤害。
/// · selfDash/selfJump = 先冲/跳到中心点再播特效结算（典韦·天降恶来）；selfBuff = BuffId 改挂给自身。
///
/// 中心点由基类 SelectTarget 决定：配 TargetType:self 时以自身为中心（近战贴脸 AoE，不做射程校验，
/// 如张绣·枪出如龙 / 魏延·奇袭 / 刘晔·卸甲）；缺省以锁定目标为中心（远程需在 Range 射程内，
/// 如司马昭·篡逆 / 钟会·惊雷）。
/// 每个目标的伤害计算与落地全部走基类 CalcDamage / ApplyDamage，因此弹道(missile)、高血增伤(highHp)、
/// execute / shieldBonus / missingHp / vsJob / soldierDamage / critRateBonus 等组件自动生效；燃蓝由基类 ApplyMpBurn 统一结算。
/// </summary>
public class SkillComDamageArea : SkillComDamageSingle
{
    public SkillComDamageArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (moving)
            return false;

        // 中心目标：self 标记时以自身为中心，否则按射程锁定目标（均由基类 SelectTarget 判定）
        var target = SelectTarget();
        if (target == null)
            return false;

        var center = GetCastCenter(target);
        var units = SelectTargets(center, skillCfg.Area, false, skillCfg.TargetCount);

        // 中心双倍：锁定目标承受 centerRate 倍伤害；若未被范围选人选中则补进来，保证主目标必中
        EnsureCenterDoubleTarget(units, target);

        // 范围内无可命中目标时不发动：避免空放消耗 MP/CD 并播放无意义特效（自身中心型远程施法尤甚）
        if (units.Count == 0)
            return false;

        if (!CanCast(target))
            return false;

        PlayCast(target);

        // 移动类组件：先冲/跳到落点，再播范围特效并结算（典韦·天降恶来）
        if (BeginMove(center, () =>
            {
                PlayAreaEffect(center);
                ApplyAreaDamage(target, units);
            }))
            return true;

        PlayAreaEffect(center);
        ApplyAreaDamage(target, units);
        return true;
    }

    /// <summary>范围伤害 + Buff：Buff 默认逐个挂给命中目标；selfBuff 时改为挂给自身（典韦·天降恶来的护甲）。</summary>
    private void ApplyAreaDamage(Chess target, List<Chess> units)
    {
        // Buff 由 SkillConfig.BuffId 短名指定（如"缓"=减速、"叛"=叛逃），支持多个（如"慑,疫"）
        var buffIds = GetSkillBuffIds();
        var baseDamage = GetSkillDamage();
        var toSelf = BuffsToSelf;
        foreach (var u in units)
        {
            // 主目标双倍由伤害基类统一处理（centerDouble / centerRate）
            var dmg = ResolveDamage(u, ApplyCenterDouble(baseDamage, u == target));
            // 导弹型：Buff 归属已延后到飞弹命中回调，施法瞬间不再挂
            if (IsMissileDeferred(dmg) || toSelf)
                continue;
            foreach (var buffId in buffIds)
                ApplyBuffToUnit(u, buffId);
        }

        if (toSelf)
        {
            foreach (var buffId in buffIds)
                ApplyBuffToUnit(owner, buffId);
        }
    }
}
