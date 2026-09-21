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

        owner = null;
    }

    //刷新
    public virtual void Refresh(Chess caster, float lastTime)
    {
        endTime = Math.Max(endTime, Time.time + lastTime);

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


}