using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonConfig;

public static class SkillManager
{
    public static Skill CreateSkill(int skillId, Chess owner)

    {
        var skillCfg = SkillConfig.GetConfig(skillId);

        switch (skillCfg.ScriptName)
        {
            case "SpinAttack":
                return new SkillSpinAttack(skillId, owner);
            case "CriticalAttack":
                return new SkillCriticalAttack(skillId, owner);
            case "MasterShield":
                return new SkillMasterShield(skillId, owner);
            case "HardSkin":
                return new SkillHardSkin(skillId, owner);
            case "RunCross":
                return new SkillRunCross(skillId, owner);
            case "Heal":
                return new SkillHeal(skillId, owner);
            case "Song":
                return new SkillSong(skillId, owner);
            case "Gold":
                return new SkillGold(skillId, owner);
            case "Feedback":
                return new SkillFeedback(skillId, owner);
            case "SpeedAttack":
                return new SkillSpeedAttack(skillId, owner);
            case "MultiArrow":
                return new SkillMultiArrow(skillId, owner);
            case "PlantSkin":
                return new SkillPlantSkin(skillId, owner);
            case "Help":
                return new SkillHelp(skillId, owner);
            case "SoldierUp":
                return new SkillSoldierUp(skillId, owner);
            case "HitBuff":
                return new SkillHitBuff(skillId, owner);
            case "HitBuffArea":
                return new SkillHitBuffArea(skillId, owner);
            case "HitArea":
                return new SkillHitArea(skillId, owner);
            case "HitWall":
                return new SkillHitWall(skillId, owner);

            case "Dumb":
                return new SkillDumb(skillId, owner);               
        }

        throw new System.Exception("Skill not found");
    }

    public static void BattleBegin(Chess chess)
    {
        foreach (var skill in chess.skills)
        {
            skill.BattleBegin();
        }
    }

    public static void AimTarget(Chess attacker, Chess defender)
    {
        foreach (var skill in attacker.skills)
        {
            skill.AimTarget(defender);
        }
    }

    public static void DuringAttack(Chess attacker, Chess defender, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {       
        foreach(var skill in attacker.skills)
        {
            skill.DuringAttack(defender, damType, ref damageBase, ref damageMulti, ref effect);

        }    
        foreach(var skill in defender.skills)
        {
            skill.DuringAttacked(attacker, damType, ref damageBase, ref damageMulti, ref effect);

        }
        foreach(var buff in attacker.buffs)
        {
            buff.DuringAttack(defender, damType, ref damageBase, ref damageMulti, ref effect);

        }   
        foreach(var buff in defender.buffs)
        {
            buff.DuringAttacked(attacker, damType, ref damageBase, ref damageMulti, ref effect);
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
        foreach (var skill in attacker.skills)
        {
            skill.OnAttack(defender, damage);
        }
        foreach (var skill in defender.skills)
        {
            skill.OnAttacked(attacker, damage);
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

    public static bool CheckAidSkill(Chess attacker)
    {
        foreach (var skill in attacker.skills)
        {
            if(skill.CheckAidSkill())
                return true;
        }
        return false;
    }

}
