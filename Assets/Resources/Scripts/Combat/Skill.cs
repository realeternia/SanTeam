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

        skillCfg = SkillConfig.GetConfig((uint)id);
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

    public bool CheckBurst()
    {
        isBurst = !IsInCD() && (skillCfg.Rate <= 0 || UnityEngine.Random.value < skillCfg.Rate);
        UnityEngine.Debug.Log("CheckBurst isBurst=" + isBurst.ToString() + " skillId=" + id.ToString());
        if(isBurst)
            UpdateCD();
        return isBurst;
    }

    public virtual void BattleBegin()
    {

    }


    public virtual void OnAttack(Chess defender, int damage)
    {
    }

    public virtual void DuringAttack(Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual void DuringAttacked(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual bool CheckAidSkill()
    {
        return false;
    }

}
