using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 技能类，处理技能相关逻辑
/// </summary>

public class Skill
{
    public int id;
    public Chess owner;
    protected SkillConfig skillCfg;
    private float lastUpdateTime; // 上次更新CD的时间
    public bool isBurst;

    public Skill(int id, Chess unit)
    {
        this.id = id;
        this.owner = unit;

        skillCfg = SkillConfig.GetConfig(id);
    }

    /// <summary>
    /// 更新技能CD时间
    /// </summary>
    public void UpdateCD()
    {
        if (skillCfg.CD > 0)
        {
            if (IsInCD())
            {
                return;
            }

            float currentTime = Time.time;
            lastUpdateTime = currentTime;
        }
    }

    /// <summary>
    /// 检查技能是否在CD中
    /// </summary>
    /// <returns>如果在CD中返回true，否则返回false</returns>
    public bool IsInCD()
    {
        if(skillCfg.CD <= 0)
            return false;

        return Time.time < lastUpdateTime + skillCfg.CD;
    }

    public bool CheckBurst(float rateReal = 0)
    {
        isBurst = !IsInCD() && (skillCfg.Rate <= 0 || UnityEngine.Random.value < Math.Max(rateReal, skillCfg.Rate));
        UnityEngine.Debug.Log("CheckBurst isBurst=" + isBurst.ToString() + " skillId=" + id.ToString());
        if(isBurst)
            UpdateCD();
        return isBurst;
    }

    protected float GetRate(Chess defender)
    {
        var myAttr = owner.GetAttr(skillCfg.Attr);
        var defAttr = defender.GetAttr(skillCfg.Attr);
        var rate = (myAttr - defAttr) * skillCfg.RateAttrHP;

        if (rate > 0)
            rate = skillCfg.RateAttrH + rate;
        else
            rate = skillCfg.Rate;

        return rate;
    }

    public virtual void BattleBegin()
    {

    }

    public virtual void AimTarget(Chess target)
    {

    }



    public virtual void OnAttack(Chess defender, int damage)
    {
    }

    public virtual void OnAttacked(Chess attacker, int damage)
    {
    }

    public virtual void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual bool CheckAidSkill()
    {
        return false;
    }

}
