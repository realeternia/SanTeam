using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 单体伤害体系基类（ScriptName = "SkillComDamageSingle"，模板「单体伤害大/中/小」）：
/// 锁定射程内的一名敌人，结算 GetSkillDamage() 技能伤害，并可选附加 SkillConfig.BuffId 指定的 Buff。
///
/// 本类是所有「单体伤害」脚本的基类，CheckAidSkill 固定按「选人 → 发动判定 → 施法表现 → 伤害结算 → 命中后处理」
/// 五步走，派生类只覆写需要的钩子（其余步骤自动继承整条通用体系）：
/// · SelectTarget()  选目标（默认 SelectSingleTarget(skillCfg.Range)，可覆写为 Area 选人或别的索敌规则）
/// · CanCast(target) 发动判定（默认 CheckBurst，可覆写追加"正在结算中"等阻止条件）
/// · PlayCast(target) 施法表现（默认动作 + SelfEffect，可覆写追加位移/自身 Buff 等）
/// · CalcDamage(target) 伤害计算（默认 ApplyDamageModifiers，含 execute/shieldBonus/missingHp/vsJob/soldierDamage）
/// · ResolveHit(target) 伤害结算（默认 CalcDamage → TryDoubleDamage → ApplyDamage → AfterDamage，
///   导弹/多段/两段射击等可整段覆写，但仍可复用 CalcDamage / ApplyDamage / AfterDamage）
/// · ApplyDamage(target, dmg) 伤害落地（默认直伤 / missile 标记走飞弹，并播命中特效）
/// · AfterDamage(target, dmg) 命中后处理（默认破盾追加 + 燃蓝 + 击退 + 苦肉 + Buff 归属 + 驱散）
///
/// 通用组件效果全部由基类统一实现（TargetType 标记 / Strength2 键），派生类不要重复判断：
/// · self        = 自身中心（近战贴脸 AoE），沿用当前索敌目标、不做射程校验（SelectTarget）
/// · missile     = 以飞弹逐个结算，伤害 ×(1+键 missileRate)（CalcDamage / ApplyDamage）
/// · maxHpBonus  = 伤害 += 目标最大生命 × 键 maxHpBonusRate（CalcDamage）
/// · execute / shieldBonus / missingHp / vsJob / soldierDamage（CalcDamage）
/// · antiShield / mpBurn / 击退 pushMin~pushMax / selfHurt / selfBuff / bothBuff / dispel（AfterDamage）
/// 伤害体系派生（各自覆写 CheckAidSkill，自动继承上述全部组件能力）：
/// · SkillComDamageAreaDeBuff  范围伤害 + 减益（目标中心 / self 自身中心）
/// · SkillComDamageShockWave   冲击波（导弹沿路径命中）
/// · SkillComDamageJumpChain   跳跃弹射（逐跳衰减）
/// · SkillComDamageHitRegion   落点持续伤害场
/// 持续伤害（dot）不在本脚本结算：BuffId 挂 DoT Buff（如"败"），每秒跳伤由 BuffTimeDamage 读 StrengthBuff1 的 dot。
/// </summary>
public class SkillComDamageSingle : Skill
{
    public SkillComDamageSingle(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲：自身作为攻击方时物理攻击完全无视目标护甲
        return HasSkillFlag("armorIgnore") && isAttackerSide ? -1f : 0f;
    }

    public override bool CheckAidSkill()
    {
        var target = SelectTarget();
        if (target == null)
            return false;
        if (!CanCast(target))
            return false;

        PlayCast(target);
        ResolveHit(target);
        return true;
    }

    /// <summary>选目标：self 标记沿用当前索敌目标（自身中心 AoE），否则按射程校验（配 SelectFirst 时走优先级链）。</summary>
    protected virtual Chess SelectTarget()
    {
        var target = HasSkillFlag("self") ? owner.targetChess : SelectSingleTarget(skillCfg.Range);
        return target != null && target.hp > 0 ? target : null;
    }

    /// <summary>发动判定：默认 CheckBurst（MP/概率/CD/触发条件）。</summary>
    protected virtual bool CanCast(Chess target)
    {
        return CheckBurst(target);
    }

    /// <summary>施法表现：默认施法动作 + 自身特效。</summary>
    protected virtual void PlayCast(Chess target)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
    }

    /// <summary>本次施放的中心点：self 标记取自身位置（近战贴脸 AoE），否则取目标位置。</summary>
    protected Vector3 GetCastCenter(Chess target)
    {
        return HasSkillFlag("self") ? owner.transform.position : target.transform.position;
    }

    /// <summary>伤害计算：基础伤害 → 弹道/最大生命追加 → 全部通用伤害修正组件。</summary>
    protected virtual int CalcDamage(Chess target)
    {
        var dmg = GetSkillDamage();
        if (HasSkillFlag("missile"))
            dmg = (int)(dmg * (1f + GetStr2("missileRate")));
        if (HasSkillFlag("maxHpBonus"))
            dmg += (int)(GetStr2("maxHpBonusRate") * target.maxHp);
        return ApplyDamageModifiers(dmg, target);
    }

    /// <summary>伤害结算主流程：计算 → 概率翻倍 → 落地 → 命中后处理。</summary>
    protected virtual void ResolveHit(Chess target)
    {
        var dmg = CalcDamage(target);
        var isDouble = TryDoubleDamage(ref dmg);
        ApplyDamage(target, dmg);
        if (isDouble)
            WorldManager.Instance.AddBattleText("暴击", target.transform.position,
                new Vector2(0, 80), SysColor.BattleText.SkillName, 3);
        AfterDamage(target, dmg);
    }

    /// <summary>伤害落地：missile 标记走飞弹，否则直接结算；命中特效统一在此播放。</summary>
    protected virtual void ApplyDamage(Chess target, int dmg)
    {
        if (HasSkillFlag("missile"))
        {
            if (dmg > 0)
                WorldManager.Instance.CreateSpellMissile(owner, target, owner.transform.position, id, dmg, skillCfg.HitEffect);
            return;
        }

        if (dmg > 0)
            target.OnSkillDamaged(owner, id, dmg);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
    }

    /// <summary>
    /// 命中后处理：破盾追加、燃烧法力、击退、苦肉反噬、Buff 归属、驱散。
    /// 派生类覆写后如需保留通用效果，请先调用 base.AfterDamage。
    /// </summary>
    protected virtual void AfterDamage(Chess target, int dmg)
    {
        // 破盾追加伤害：以技能基础伤害为基数，携带 AntiShield 标签绕过护盾直接打血
        ApplyAntiShieldBonus(target, GetSkillDamage());
        ApplyMpBurn(target);
        PushTargetBack(target);
        HurtSelfByDamage(dmg);
        ApplySkillBuffs(target);
        ApplyDispel(target);
    }
}
