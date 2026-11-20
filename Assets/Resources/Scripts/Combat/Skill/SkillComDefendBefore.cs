using UnityEngine;
using CommonConfig;

/// <summary>
/// 受伤前防御（ScriptName = "SkillComDefendBefore"，模板「防御」）：
/// 受击方在伤害计算阶段（BeforeCalDamaged：抗性之后、护盾吸收之前）统一处理减伤与反弹：
/// · Strength2 key "reduce"：按比例减免本次伤害（0.25 = 本次伤害 × 0.75）；
/// · Strength2 key "reflect"：按比例把本次伤害反弹给攻击者（反弹伤害带 Reflect 标签，不再二次反弹）。
/// · Strength2 key "onlyPhys"/"onlyMagic"：受击伤害类型筛选，只配一个时仅对该类型生效，都不配或都配时物理+魔法都算。
/// · Strength2 key "DamageSource"/"minHpRate"：伤害源限定（hero/soldier/any，未配置=都算）/ 本次伤害需超过自身最大生命比例（minHpRate=0.03 即 3%）才触发。
/// · SkillConfig.BuffId：触发时为自身挂状态组件配置的 Buff（护盾/减伤/受击叠防/焰衣等），可做纯 Buff 型防御。
/// 基础伤害类型：技能伤害取 SkillConfig.DamageType；普攻（castSkillCfg 为空）按物理处理；真实伤害不减伤也不反弹。
/// 触发闸门由 CheckBurst 统一判定：MpCost>0 时每次触发扣 MP，并受 CD / Rate / TriggerCondition（如 hprate&lt;50）约束；
/// 技能自身继承的 CheckAidSkill=false，不会被当作主动技能自动施放。
/// 派生伤害（连锁/破盾/反伤）不再触发防御，避免双方反伤互相递归。
/// 参考金铲铲：塔姆·厚皮（减伤）、荆棘之甲（反伤）。
/// </summary>
public class SkillComDefendBefore : Skill
{
    public SkillComDefendBefore(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>
    /// 受击防御通用闸门：派生伤害 / 受击类型 / 伤害源 / 伤害量筛选，全部通过返回 true。
    /// 供 SkillComDefendBefore 与 SkillComDefendCheatDeath 共用。
    /// </summary>
    protected bool MeetDefendGate(Chess caster, SkillConfig castSkillCfg, int damageBase, float damageMulti, string hurtTag, out int damageType, bool ignoreDamageThreshold = false)
    {
        damageType = castSkillCfg != null ? castSkillCfg.DamageType : CombatConst.DamageTypeAttack;
        if (owner == null || owner.hp <= 0 || caster == null || damageBase <= 0 || damageMulti <= 0f)
            return false;
        // 派生伤害不再触发防御，避免反伤/连锁互相递归
        if (CombatConst.IsDerivedHurtTag(hurtTag))
            return false;

        // 受击伤害类型筛选：onlyPhys/onlyMagic 只配一个时只认该类型；都不配或都配时物理+魔法都算
        var isMagic = damageType == CombatConst.DamageTypeMagic;
        var onlyPhys = GetStr2("onlyPhys") > 0f;
        var onlyMagic = GetStr2("onlyMagic") > 0f;
        if (onlyPhys != onlyMagic)
        {
            if (onlyPhys && isMagic)
                return false;
            if (onlyMagic && !isMagic)
                return false;
        }

        // 伤害源筛选：DamageSource=hero 时仅英雄造成的伤害触发（未配置/any 时都算）
        if (!IsDamageSourceMatched(caster))
            return false;

        // 伤害量门槛：minHpRate=0.03 时，本次伤害需超过自身最大生命的 3%（致命伤害不受此限制）
        var minHpRate = GetStr2("minHpRate");
        if (!ignoreDamageThreshold && minHpRate > 0f && damageBase * damageMulti < owner.maxHp * minHpRate)
            return false;
        return true;
    }

    /// <summary>按 SkillConfig.BuffId 给自己挂状态 Buff，护盾类按 Strength2.shield 补齐盾值。</summary>
    protected void ApplyDefendBuffs()
    {
        var buffIds = GetSkillBuffIds();
        foreach (var buffId in buffIds)
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        var shieldHp = GetSkillShield();
        if (shieldHp <= 0)
            return;
        foreach (var buffId in buffIds)
        {
            var shield = owner.GetBuff(buffId) as BuffShield;
            if (shield != null)
                shield.AddHp(shieldHp);
        }
    }

    public override void BeforeCalDamaged(Chess caster, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        int damageType;
        if (!MeetDefendGate(caster, castSkillCfg, damageBase, damageMulti, hurtTag, out damageType))
            return;

        var isReal = damageType == CombatConst.DamageTypeReal;
        var reduce = isReal ? 0f : GetStr2("reduce");
        var reflect = isReal ? 0f : GetStr2("reflect");
        // 触发后要挂的状态 Buff（护盾/减伤/受击叠防/焰衣等），允许纯 Buff 型防御
        var buffIds = GetSkillBuffIds();
        var shieldHp = GetSkillShield();
        if (reduce <= 0f && reflect <= 0f && buffIds.Count == 0 && shieldHp <= 0)
            return;
        if (!CheckBurst(caster))
            return;

        if (reduce > 0f)
            damageMulti *= Mathf.Clamp(1f - reduce, 0f, 1f);

        // 反弹：按减免后的本次伤害 × 反弹比例，对攻击者结算一次带 Reflect 标签的伤害
        if (reflect > 0f && caster != owner && caster.hp > 0)
        {
            var finalDamage = (int)(damageBase * damageMulti);
            if (finalDamage > 0)
            {
                var reflectDamage = Mathf.Max(1, (int)(finalDamage * reflect));
                caster.OnSkillDamaged(owner, id, reflectDamage, false, CombatConst.ReflectHurtTag);
            }
        }

        ApplyDefendBuffs();

        if (!string.IsNullOrEmpty(skillCfg.Action))
            owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        GameLog.Debug($"受伤前防御 技能id={id} 等级={Level} 类型={damageType} 减伤={reduce} 反弹={reflect} Buff数={buffIds.Count} 生命={owner.hp}/{owner.maxHp}");
    }
}
