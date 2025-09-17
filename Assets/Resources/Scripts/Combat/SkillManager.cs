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
            case "AttackedBuff":
                return new SkillAttackedBuff(skillId, owner);
            case "RunCross":
                return new SkillRunCross(skillId, owner);
            case "HelpHeal":
                return new SkillHelpHeal(skillId, owner);
            case "HelpAidBuff":
                return new SkillHelpAidBuff(skillId, owner);
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
            case "HelpTeach":
                return new SkillHelpTeach(skillId, owner);
            case "SoldierUp":
                return new SkillSoldierUp(skillId, owner);
            case "HitBuff":
                return new SkillHitBuff(skillId, owner);
            case "HitBuffArea":
                return new SkillHitBuffArea(skillId, owner);
            case "HitRegion":
                return new SkillHitRegion(skillId, owner);
            case "HitWall":
                return new SkillHitWall(skillId, owner);
            case "SoldierSummon":
                return new SkillSoldierSummon(skillId, owner);
            case "DamageReal":
                return new SkillDamageReal(skillId, owner);
            case "AttackedShadow":
                return new SkillAttackedShadow(skillId, owner);
            case "RunCrossPlus":
                return new SkillRunCrossPlus(skillId, owner);
            case "HitTeleport":
                return new SkillHitTeleport(skillId, owner);
            case "HitRepeat":
                return new SkillHitRepeat(skillId, owner);
            case "HitAttr":
                return new SkillHitAttr(skillId, owner);
            case "HitArea":
                return new SkillHitArea(skillId, owner);
            case "HitAround":
                return new SkillHitAround(skillId, owner);
            case "ShockWave":
                return new SkillShockWave(skillId, owner);
            case "ModifyInteRateTime":
                return new SkillModifyInteRateTime(skillId, owner);

            case "Dumb":
                return new SkillDumb(skillId, owner);               
        }

        throw new System.Exception("Skill not found " + skillCfg.ScriptName);
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

    public static void OnCheckBurst(Chess caster, SkillConfig skillCfg, ref float rate)
    {
        foreach (var skill in caster.skills)
        {
            if(skill.skillId != skillCfg.Id) //防止自己判定自己
                skill.OnCheckBurst(skillCfg, ref rate);
        }
    }

    public static void DuringAttack(Chess attacker, Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {       
        foreach(var skill in attacker.skills)
        {
            skill.DuringAttack(defender, damType, ref damageBase, ref damageMulti, ref damageReal, ref effect);

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

    public static void OnAttack(Chess attacker, Chess defender, string damType, int damage)
    {
        foreach (var skill in attacker.skills)
        {
            skill.OnAttack(defender, damType, damage);
        }
        foreach (var skill in defender.skills)
        {
            skill.OnAttacked(attacker, damType, damage);
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

    public static void OnAddBuff(Chess target, Chess caster, BuffConfig buffCfg, ref float time)
    {
        foreach (var skill in caster.skills)
        {
            skill.OnAddBuff(buffCfg, ref time);
        }
        foreach (var skill in target.skills)
        {
            skill.OnBeAddBuff(buffCfg, ref time);
        }
    }

    public static void OnDoSkillDamage(Chess target, Chess caster, SkillConfig skillCfg, ref int damage)
    {
        foreach (var skill in caster.skills)
        {
            if(skillCfg.Id == skill.skillId)
                continue;
            skill.OnDoSkillDamage(skillCfg, ref damage);
        }
        foreach (var skill in target.skills)
        {
            if (skillCfg.Id == skill.skillId)
                continue;
            skill.OnBeDoSkillDamage(skillCfg, ref damage);
        }
    }

}
