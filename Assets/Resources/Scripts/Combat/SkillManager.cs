using System.Collections;
using System.Collections.Generic;
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
                return new CriticalAttack(skillId);

        }

        throw new System.Exception("Skill not found");
    }

    public static void DuringAttack(Chess attacker, Chess defender, ref int damage, ref string effect)
    {
        foreach(var skill in attacker.skills)
        {
            skill.DuringAttack(attacker, defender, ref damage, ref effect);

        }
    }


    public static void OnAttack(Chess attacker, Chess defender, int damage)
    {
         UnityEngine.Debug.Log("OnAttack " + attacker.heroId.ToString() + " " + attacker.skills.Count.ToString());
        foreach(var skill in attacker.skills)
        {
            if(skill.CheckBurst())
            {
                skill.OnAttack(attacker, defender, damage);
                skill.UpdateCD();
            }
        }
    }
}
