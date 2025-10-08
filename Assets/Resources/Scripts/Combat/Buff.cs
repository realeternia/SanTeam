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
        UnityEngine.Debug.Log("Buff OnAdd " + id + " " + skillCfg.Id + " " + caster + " " + chess);
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
        UnityEngine.Debug.Log("Buff OnRemove " + id + " " + skillCfg.Id + " " + caster + " " + chess);
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

    public virtual void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref string effect)

    {
    }
    public virtual void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)

    {
    }

    public virtual void BeforeAttacked(Chess defender, ref int damage)

    {
    }


    public virtual void OnAttack(Chess defender, int damage)
    {
    }

    public virtual void OnAttacked(Chess attacker, int damage)
    {
    }


}