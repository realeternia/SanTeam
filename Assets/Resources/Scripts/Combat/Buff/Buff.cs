using System;
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

    }

    public void SetTime(float time)
    {
        endTime = Time.time + time;
    }

    /// <summary>
    /// 按 key 读取施加本 Buff 的技能配置的 StrengthBuff1 数值槽（key 为 "k:v,k:v" 串中的键名），
    /// 未配置 / key 不存在时返回 def。每个 Buff 读自己固定的语义 key，不再按数组下标/组合位置猜槽位。
    /// </summary>
    protected float GetStrBuff(string key, float def = 0f)
    {
        return ConfigManager.GetSlotValue(skillCfg == null ? null : skillCfg.StrengthBuff1, key, def);
    }

    /// <summary>
    /// 按 key 读取施加本 Buff 的技能配置的 Strength2 数值槽（key 为 "k:v,k:v" 串中的键名），
    /// 未配置 / key 不存在时返回 def。
    /// </summary>
    protected float GetStr2(string key, float def = 0f)
    {
        return ConfigManager.GetSlotValue(skillCfg == null ? null : skillCfg.Strength2, key, def);
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