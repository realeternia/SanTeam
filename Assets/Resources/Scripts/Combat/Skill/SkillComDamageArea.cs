using UnityEngine;

/// <summary>
/// 通用"范围伤害 + 可选 Buff"技能（ScriptName = "SkillComDamageArea"）：
/// 取中心点 skillCfg.Area 半径内的敌人（选人规则见 SelectOnly / SelectFirst，TargetCount&lt;=0 时范围内全部）
/// 造成技能伤害；每个目标再按 SkillConfig.BuffId 挂 Buff（英文/中文逗号或分号分隔可配多个，未配置则纯伤害），时长 BuffTime。
/// 范围特效播在中心点、命中特效播在每个受击目标身上。
/// · centerDouble = 锁定目标（首个目标）承受 centerRate 倍伤害（袁绍·名门），其余目标正常伤害。
///
/// 中心点由基类 SelectTarget 决定：配 TargetType:self 时以自身为中心（近战贴脸 AoE，不做射程校验，
/// 如张绣·枪出如龙 / 魏延·奇袭 / 刘晔·卸甲）；缺省以锁定目标为中心（远程需在 Range 射程内，
/// 如司马昭·篡逆 / 钟会·蓄谋）。
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
        // 中心目标：self 标记时以自身为中心，否则按射程锁定目标（均由基类 SelectTarget 判定）
        var target = SelectTarget();
        if (target == null)
            return false;

        if (!CanCast(target))
            return false;

        var center = GetCastCenter(target);

        PlayCast(target);
        PlayAreaEffect(center);

        var units = SelectTargets(center, skillCfg.Area, false, skillCfg.TargetCount);

        // 中心双倍：锁定目标承受 centerRate 倍伤害；若未被范围选人选中则补进来，保证主目标必中
        EnsureCenterDoubleTarget(units, target);

        // Buff 由 SkillConfig.BuffId 短名指定（如"缓"=减速、"叛"=叛逃），支持多个（如"慑,疫"）
        var buffIds = GetSkillBuffIds();
        var baseDamage = GetSkillDamage();
        foreach (var u in units)
        {
            // 主目标双倍由伤害基类统一处理（centerDouble / centerRate）
            var dmg = ResolveDamage(u, ApplyCenterDouble(baseDamage, u == target));
            // 导弹型：Buff 归属已延后到飞弹命中回调，施法瞬间不再挂
            if (IsMissileDeferred(dmg))
                continue;
            foreach (var buffId in buffIds)
                ApplyBuffToUnit(u, buffId);
        }
        return true;
    }
}
