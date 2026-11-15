﻿using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"单体伤害 + 可选增益/减益"技能（ScriptName = "SkillComSingleDamage"）：
/// 锁定 skillCfg.Range 射程内的当前目标，造成 GetSkillDamage() 技能伤害，并可选附加 SkillConfig.BuffId
/// 指定的 Buff（英文/中文逗号或分号分隔多个，空=纯伤害），时长 BuffTime。
/// 多段连打请使用 SkillMultiHit（荀攸·奇策）。
///
/// 通过 TargetType 标记列表扩展同族行为（多项逗号分隔，空=默认）；下列标记/键全部由基类 Skill 统一实现，
/// 本类只负责选人、结算顺序与表现：
/// selfBuff / bothBuff（Buff 归属）、pushMin~pushMax（击退）、execute、shieldBonus、missingHp、armorIgnore、
/// antiShield、dispel、mpBurn、selfHurt、doubleChance、vsJob、soldierDamage（士兵增伤）。
/// 具体键名与语义见 Skill 基类注释。
///
/// 持续伤害（dot）不再由本脚本结算：统一在 BuffId 里挂 DoT Buff（如"败"），
/// 每秒跳伤由 BuffTimeDamage 读施加技能的 StrengthBuff1 键"dot"（绝对值）结算。
/// </summary>
public class SkillComSingleDamage : Skill
{
    public SkillComSingleDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲：自身作为攻击方时物理攻击完全无视目标护甲
        return HasSkillFlag("armorIgnore") && isAttackerSide ? -1f : 0f;
    }

    public override bool CheckAidSkill()
    {
        // 选人规则见 SelectOnly / SelectFirst：当前索敌目标仍满足规则时沿用，否则挑最合适的一个
        var target = SelectSingleTarget(skillCfg.Range);
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 单段伤害（多段连打已拆分为 SkillMultiHit，如荀攸·奇策）
        var dmg = ApplyDamageModifiers(GetSkillDamage(), target);
        var isDouble = TryDoubleDamage(ref dmg);
        if (dmg > 0)
            target.OnSkillDamaged(owner, id, dmg);
        if (isDouble)
            WorldManager.Instance.AddBattleText("暴击", target.transform.position,
                new Vector2(0, 80), SysColor.BattleText.SkillName, 3);
        // 破盾追加伤害：以技能基础伤害为基数，携带 AntiShield 标签绕过护盾直接打血
        ApplyAntiShieldBonus(target, GetSkillDamage());

        ApplyMpBurn(target);
        PushTargetBack(target);
        HurtSelfByDamage(dmg);
        ApplySkillBuffs(target);
        ApplyDispel(target);

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
