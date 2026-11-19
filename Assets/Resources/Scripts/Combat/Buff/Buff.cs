using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;


public class Buff
{
    public int id;

    public Chess caster;
    public Chess owner;
    
    public BuffConfig buffCfg;
    public SkillConfig skillCfg;

    public float endTime;
    public GameObject effect;

    /// <summary>BuffConfig.BuffAttrDynamic 声明的数值槽键集合（只有这些键参与品质乘算）</summary>
    private readonly HashSet<string> dynamicAttrs = new HashSet<string>();

    /// <summary>
    /// 叠加模式为"时间延长"(StackMode=3)时，本实例存续期间被延长的次数：
    /// 第1次延长×0.9、第2次×0.8…用于控制类 Buff 的衰减，实例被顶掉/过期后重新计数
    /// </summary>
    public int extendCount;


    public Buff(int id, int skillId, Chess caster, Chess unit, float lastTime)
    {
        this.id = id;
        this.caster = caster;
        owner = unit;
        buffCfg = BuffConfig.GetConfig(id);
        skillCfg = SkillConfig.GetConfig(skillId);
        endTime = Time.time + lastTime;

        if (buffCfg != null && !string.IsNullOrEmpty(buffCfg.BuffAttrDynamic))
        {
            foreach (string key in buffCfg.BuffAttrDynamic.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries))
                dynamicAttrs.Add(key.Trim());
        }
    }

    public void SetTime(float time)
    {
        endTime = Time.time + time;
    }

    /// <summary>
    /// 按 key 读取施加本 Buff 的技能配置的 StrengthBuff1 数值槽（key 为 "k:v,k:v" 串中的键名），
    /// 未配置 / key 不存在时返回 def。每个 Buff 读自己固定的语义 key，不再按数组下标/组合位置猜槽位。
    /// 仅当 key 属于 BuffConfig.BuffAttrDynamic 且 IsQualityBonus=true 时，再乘施法者英雄品质系数。
    /// </summary>
    protected float GetStrBuff(string key, float def = 0f)
    {
        float raw = ConfigManager.GetSlotValue(skillCfg == null ? null : skillCfg.StrengthBuff1, key, float.NaN);
        if (float.IsNaN(raw)) return def;
        return raw * GetQualityFactor(key);
    }

    /// <summary>
    /// 按 key 读取施加本 Buff 的技能配置的 Strength2 数值槽（key 为 "k:v,k:v" 串中的键名），
    /// 未配置 / key 不存在时返回 def。
    /// 仅当 key 属于 BuffConfig.BuffAttrDynamic 且 IsQualityBonus=true 时，再乘施法者英雄品质系数。
    /// </summary>
    protected float GetStr2(string key, float def = 0f)
    {
        float raw = ConfigManager.GetSlotValue(skillCfg == null ? null : skillCfg.Strength2, key, float.NaN);
        if (float.IsNaN(raw)) return def;
        return raw * GetQualityFactor(key);
    }

    /// <summary>
    /// Buff 数值品质系数：仅当 key 属于 BuffConfig.BuffAttrDynamic（受品质影响）、BuffConfig.IsQualityBonus=true，
    /// 且由英雄专属技能（HeroConfig.Skill1）施加时，按施法者英雄品质取 CombatConst.QualityBuffFactors（Q1~Q4），否则为 1；
    /// BuffAttrConst 声明的常量键永远不参与品质乘算。职业/连接/道具技能施加的 Buff 不参与英雄品质修正。
    /// 无施法者时退回 Buff 携带者，品质越界按边界处理。
    /// </summary>
    protected float GetQualityFactor(string key)
    {
        if (buffCfg == null || !buffCfg.IsQualityBonus) return 1f;
        if (!dynamicAttrs.Contains(key)) return 1f;
        if (skillCfg == null || !HeroScenario.IsHeroExclusiveSkill(skillCfg.Sname)) return 1f;
        Chess source = caster != null ? caster : owner;
        int quality = HeroScenario.GetQuality(source == null ? 0 : source.heroId);
        if (quality < 1) quality = 1;
        if (quality > CombatConst.QualityBuffFactors.Length) quality = CombatConst.QualityBuffFactors.Length;
        return CombatConst.QualityBuffFactors[quality - 1];
    }

    public virtual void OnAdd(Chess chess, Chess caster)
    {
        GameLog.Debug("Buff OnAdd " + id + " " + skillCfg.Id + " " + caster + " " + chess);
        owner = chess;

        if (!string.IsNullOrEmpty(buffCfg.BuffEffect))
        {
            effect = EffectManager.PlayBuffEffect(chess, buffCfg.BuffEffect);
        }

        if(!string.IsNullOrEmpty(buffCfg.ColorStart))
        {
            Color start = ColorUtility.TryParseHtmlString(buffCfg.ColorStart, out start) ? start : Color.white;
            Color end = ColorUtility.TryParseHtmlString(buffCfg.ColorEnd, out end) ? end : Color.white;
            chess.AddColorEffect(start, end);
        }

        // 切材质：配置了「切材质」时把单位材质换成对应材质（如冰冻 Buff"缓" → BuffFrozen）
        if (!string.IsNullOrEmpty(buffCfg.Material))
        {
            chess.AddMaterialEffect(buffCfg.Material);
        }

    }

    public virtual void OnRemove(Chess chess)
    {
        GameLog.Debug("Buff OnRemove " + id + " " + skillCfg.Id + " " + caster + " " + chess);
        if (effect != null)
        {
            GameObject.Destroy(effect);
            effect = null;
        }
        if (!string.IsNullOrEmpty(buffCfg.ColorStart))
        { 
            chess.RemoveColorEffect();
        }

        // 恢复材质：Buff 移除时还原单位原始材质
        if (!string.IsNullOrEmpty(buffCfg.Material))
        {
            chess.RemoveMaterialEffect();
        }

        owner = null;
    }

    public void WaitForRemove()
    {
        endTime = Time.time - 1;

    }

    // 伤害计算阶段·攻击方：调整伤害基数与倍率（普攻与技能伤害统一进入）
    public virtual void BeforeCalDamage(Chess defender, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)

    {
    }
    // 伤害计算阶段·受击方：调整受伤基数与倍率（普攻与技能伤害统一进入）
    public virtual void BeforeCalDamaged(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag)

    {
    }

    // 伤害结算阶段·受击方：只做伤害吸收（护盾 BuffShield），不做伤害放大
    public virtual void DuringCalDamage(Chess defender, ref int damage, string hurtTag)

    {
    }


    public virtual void OnAttack(Chess defender, int damage)
    {
    }

    public virtual void OnAttacked(Chess attacker, int damage)
    {
    }

    /// <summary>
    /// 普攻伤害类型覆写机制：返回 -1 表示不干预（普攻沿用默认物理结算）；
    /// 返回 CombatConst.DamageTypeMagic/Attack/Real 则由该 buff 覆写携带者普攻的伤害类型
    /// （如威震"震"把普攻转为真实伤害：无视护甲与护盾）。多个 buff 时按列表顺序取第一个非负值。
    /// </summary>
    public virtual int OverrideAttackDamageType()
    {
        return -1;
    }


}