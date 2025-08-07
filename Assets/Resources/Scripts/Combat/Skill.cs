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
    protected SkillConfig skillCfg;
    private float lastUpdateTime; // 上次更新CD的时间
    public bool isBurst;

    public Skill(int id)
    {
        this.id = id;
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
        return Time.time < lastUpdateTime + skillCfg.CD;
    }

    public void CheckBurst()
    {
        isBurst = !IsInCD() && UnityEngine.Random.value < skillCfg.Rate;
        UnityEngine.Debug.Log("CheckBurst isBurst=" + isBurst.ToString() + " skillId=" + id.ToString());
    }

    public virtual void OnAttack(Chess attacker, Chess defender, int damage)
    {
    }

    public virtual void DuringAttack(Chess attacker, Chess defender, ref int damageBase, ref float damageMulti, ref string effect)

    {
    }

}
