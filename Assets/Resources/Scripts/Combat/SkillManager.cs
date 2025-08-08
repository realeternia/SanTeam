using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonConfig;

public static class SkillManager
{
    public static Skill CreateSkill(int skillId)
    {
        var skillCfg = SkillConfig.GetConfig((uint)skillId);

        switch(skillCfg.ScriptName)
        {
            case "SpinAttack":
                return new SkillSpinAttack(skillId);
            case "CriticalAttack":
                return new SkillCriticalAttack(skillId);

        }

        throw new System.Exception("Skill not found");
    }

    public static void CheckBurst(Chess attacker)
    {
        foreach(var skill in attacker.skills)
        {
            skill.CheckBurst();
        }
    }

    public static void DuringAttack(Chess attacker, Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {       
        foreach(var skill in attacker.skills.Where(x => x.isBurst))
        {
            skill.DuringAttack(attacker, defender, ref damageBase, ref damageMulti, ref effect);
        }
        foreach(var buff in attacker.buffs)
        {
            buff.DuringAttack(defender, ref damageBase, ref damageMulti, ref effect);
        }   
        foreach(var buff in defender.buffs)
        {
            buff.DuringAttacked(attacker, ref damageBase, ref damageMulti, ref effect);
        }
    }

    // 护盾要再这一层算
    public static void BeforeAttack(Chess attacker, Chess defender, ref int damage)
    {
        foreach(var buff in defender.buffs)
        {
            buff.BeforeAttacked(attacker, ref damage);
        }
    }



    public static void OnAttack(Chess attacker, Chess defender, int damage)
    {
        UnityEngine.Debug.Log("OnAttack " + attacker.heroId.ToString() + " " + attacker.skills.Count.ToString());
        foreach (var skill in attacker.skills.Where(x => x.isBurst))
        {
            skill.OnAttack(attacker, defender, damage);
        }
        foreach(var buff in attacker.buffs)
        {
            buff.OnAttack(defender, damage);
        }   
        foreach(var buff in defender.buffs)
        {
            buff.OnAttacked(attacker, damage);
        }
    }
}
