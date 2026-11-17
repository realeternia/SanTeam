using CommonConfig;
using UnityEngine;

/// <summary>
/// 治疗体系基类（ScriptName = "SkillComHealSingle"，模板「单体治疗大/中/小」）：
/// 在 Area 内按 SelectOnly / SelectFirst（典型 SelectOnly="ally;hero;notFullHp"、SelectFirst="lowestHp"）
/// 选出一名友方单位，恢复 GetSkillHeal() 点生命（吃医职业治疗加成），并附加通用效果。
///
/// CheckAidSkill 固定按「选人 → 发动判定 → 施法表现 → 治疗结算」四步走，派生类只覆写需要的钩子：
/// · SelectTarget()  选目标（默认 Area 内按筛选取 1 名友方）
/// · CanCast(target) 发动判定（默认 CheckBurst）
/// · PlayCast(target) 施法表现（默认施法动作 + 自身特效）
/// · ResolveHeal(target) 治疗结算（默认 HealUnit + 通用 Buff 施加）
/// · CleanseOnHeal 是否在治疗时驱散目标负面 Buff（默认 true）
///
/// **所有治疗组件（TargetType 标记 / Strength2 键）统一在基类 HealUnit 结算**，派生类不要重复判断：
/// · overflowToArmor = 溢出治疗量转等量护甲（与 BuffId 的护甲 Buff 配合，此时不再驱散；当前无技能使用）
/// · 护盾类 Buff 按统一护盾公式写入护盾值
/// 治疗体系派生：SkillComHealArea（范围治疗）、SkillComHealAreaCasting（持续治疗领域）、SkillComHealJumpChain（跳跃治疗链）。
/// </summary>
public class SkillComHealSingle : Skill
{
    public SkillComHealSingle(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>治疗时是否驱散目标负面 Buff（BuffManager.DispelNegative）。</summary>
    protected virtual bool CleanseOnHeal
    {
        get { return true; }
    }

    public override bool CheckAidSkill()
    {
        var target = SelectTarget();
        if (target == null)
            return false;
        if (!CanCast(target))
            return false;

        PlayCast(target);
        ResolveHeal(target);
        return true;
    }

    /// <summary>选目标：Area 内按 SelectOnly / SelectFirst 取一名友方（Area=0 表示全图）。</summary>
    protected virtual Chess SelectTarget()
    {
        return PickHealTarget(null);
    }

    /// <summary>按可选过滤条件在 Area 内取 1 名友方目标（派生类选人时复用）。</summary>
    protected Chess PickHealTarget(System.Func<Chess, bool> extraFilter)
    {
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, true, 1, null, extraFilter);
        var target = picked.Count > 0 ? picked[0] : null;
        return target != null && target.hp > 0 ? target : null;
    }

    /// <summary>发动判定：默认 CheckBurst（治疗不做基于目标属性的概率修正）。</summary>
    protected virtual bool CanCast(Chess target)
    {
        return CheckBurst(null);
    }

    /// <summary>施法表现：默认施法动作 + 自身特效。</summary>
    protected virtual void PlayCast(Chess target)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
    }

    /// <summary>治疗结算：按配置治疗量治疗一名目标，并施加通用 Buff。</summary>
    protected virtual void ResolveHeal(Chess target)
    {
        HealUnit(target, GetSkillHeal(), true);
    }

    /// <summary>
    /// 对单个单位结算治疗（所有治疗组件的唯一入口）：
    /// 治疗 → overflowToArmor 转护甲 / 驱散负面 → 按需施加 BuffId → 命中特效。
    /// withBuffs=false 用于持续/跳跃治疗（每跳只结算治疗，不重复挂 Buff）。
    /// </summary>
    protected void HealUnit(Chess unit, int heal, bool withBuffs)
    {
        var overflowToArmor = HasSkillFlag("overflowToArmor");
        var overflow = overflowToArmor ? Mathf.Max(0, heal - (unit.maxHp - unit.hp)) : 0;

        if (heal > 0)
            owner.HealTarget(unit, skillId, heal, true);

        if (overflowToArmor)
        {
            // 溢出治疗转护甲：BuffId 需配护甲 Buff（如"甲"），护甲值 = 溢出量
            if (overflow > 0)
                ApplyOverflowArmor(unit, overflow);
        }
        else
        {
            if (CleanseOnHeal)
                BuffManager.DispelNegative(unit);
            if (withBuffs)
                ApplySkillBuffs(unit);
        }

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
    }

    /// <summary>把溢出治疗量写成护甲：挂上 BuffId 指定的 Buff 并以其护甲值设为溢出量。</summary>
    private void ApplyOverflowArmor(Chess unit, int overflow)
    {
        BuffArmorAdd armorBuff = null;
        foreach (var buffId in GetSkillBuffIds())
        {
            BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            // 共存模式(StackMode=1)下同 Id 可能已有旧实例，取本次新加的最后一个
            var added = unit.GetLastBuff(buffId) as BuffArmorAdd;
            if (added != null)
                armorBuff = added;
        }

        if (armorBuff == null)
        {
            GameLog.Warn($"{skillCfg.Name} 技能id={id} 配置了 overflowToArmor 但 BuffId[{skillCfg.BuffId}] 不是护甲类 Buff，溢出治疗未转护甲");
            return;
        }
        armorBuff.SetArmor(overflow);
    }
}
