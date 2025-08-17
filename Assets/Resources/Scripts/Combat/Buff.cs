using System;
using CommonConfig;
using UnityEngine;


public class Buff
{
    public int id;

    public Chess owner;
    
    public BuffConfig buffCfg;
    public float endTime;
    public GameObject effect;
    public int strength;


    public Buff(int id, Chess unit, float lastTime, int strength = 0)
    {
        this.id = id;
        owner = unit;
        buffCfg = BuffConfig.GetConfig(id);
        endTime = Time.time + lastTime;
        this.strength = strength;

    }

    public virtual void OnAdd(Chess chess, Chess caster)
    {
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
    public virtual void Refresh(Chess caster, float lastTime, int strength = 0)
    {
        endTime = Math.Max(endTime, Time.time + lastTime);

    }


    public void Update()
    {

    }

    public void WaitForRemove()
    {
        endTime = Time.time - 1;

    }

    public virtual void DuringAttack(Chess defender, ref int damageBase, ref float damageMulti, ref string effect)

    {
    }
    public virtual void DuringAttacked(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)

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