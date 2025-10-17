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
            case "AttackSpinAttack":
                return new SkillAttackSpinAttack(skillId, owner);
            case "AttackAddDamage":
                return new SkillAttackAddDamage(skillId, owner);
            case "InitMasterShield":
                return new SkillInitMasterShield(skillId, owner);
            case "InitSoldierShield":
                return new SkillInitSoldierShield(skillId, owner);
            case "AttackedBuff":
                return new SkillAttackedBuff(skillId, owner);
            case "AttackRunCross":
                return new SkillAttackRunCross(skillId, owner);
            case "AttackRunCrossPlus":
                return new SkillAttackRunCrossPlus(skillId, owner);                
            case "HelpAidHeal":
                return new SkillHelpAidHeal(skillId, owner);
            case "HelpAidBuff":
                return new SkillHelpAidBuff(skillId, owner);
            case "AttackAntiShield":
                return new SkillAttackAntiShield(skillId, owner);

            case "DefFeedback":
                return new SkillDefFeedback(skillId, owner);
            case "AttackSpeedAttack":
                return new SkillAttackSpeedAttack(skillId, owner);
            case "AttackMultiArrow":
                return new SkillAttackMultiArrow(skillId, owner);
            case "AttackReboundArrow":
                return new SkillAttackReboundArrow(skillId, owner);
            case "DefPlantSkin":
                return new SkillDefPlantSkin(skillId, owner);
            case "DefHpLow":
                return new SkillDefHpLow(skillId, owner);
            case "DefSkillDamageReduce":
                return new SkillDefSkillDamageReduce(skillId, owner);
            case "HelpInitTeach":
                return new SkillHelpInitTeach(skillId, owner);
            case "InitSoldierUp":
                return new SkillInitSoldierUp(skillId, owner);
            case "InitSoldierSummon":
                return new SkillInitSoldierSummon(skillId, owner);      
            case "InitGold":
                return new SkillInitGold(skillId, owner);
            case "InitAttrZhiheng":
                return new SkillInitAttrZhiheng(skillId, owner);
            case "InitFood":
                return new SkillInitFood(skillId, owner);                          
            case "HitBuff":
                return new SkillHitBuff(skillId, owner);
            case "HitBuffArea":
                return new SkillHitBuffArea(skillId, owner);
            case "HitRegion":
                return new SkillHitRegion(skillId, owner);
            case "HitWall":
                return new SkillHitWall(skillId, owner);
            case "DamageReal":
                return new SkillDamageReal(skillId, owner);
            case "AttackedShadow":
                return new SkillAttackedShadow(skillId, owner);

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
            case "AidShockWave":
                return new SkillAidShockWave(skillId, owner);
            case "AidSuddenArrow":
                return new SkillAidSuddenArrow(skillId, owner);
            case "ModifySkillRateTime":
                return new SkillModifySkillRateTime(skillId, owner);
            case "BuffExpand":
                return new SkillBuffExpand(skillId, owner);
            case "BuffExpandPos":
                return new SkillBuffExpandPos(skillId, owner);                
            case "ModifyBuffTime":
                return new SkillModifyBuffTime(skillId, owner);
            case "ModifyBeBuffTime":
                return new SkillModifyBeBuffTime(skillId, owner);
            case "BuffTieqi":
                return new SkillBuffTieqi(skillId, owner);
            case "ModifyShootSpeed":
                return new SkillModifyShootSpeed(skillId, owner);
            case "ModifySummonTime":
                return new SkillModifySummonTime(skillId, owner);
            case "InitAddCrit":
                return new SkillInitAddCrit(skillId, owner);
            case "InitAddDodge":
                return new SkillInitAddDodge(skillId, owner);
            case "InitAddRege":
                return new SkillInitAddRege(skillId, owner);
            case "HitFood":
                return new SkillHitFood(skillId, owner);

            case "Dumb":
                return new SkillDumb(skillId, owner);               
        }

        throw new System.Exception("Skill not found " + skillCfg.ScriptName);
    }

    public static void CheckAddSkill(Chess chess)
    {
        foreach (var skill in chess.skills)
        {
            if (!string.IsNullOrEmpty(skill.skillCfg.HelpSkill) && !skill.isGivenSkill)
            {
                var unitsInRange = WorldManager.Instance.GetUnitsMySidePosType(chess.side, chess.pos, true, skill.skillCfg.UnitHelpType);
                unitsInRange.Remove(chess);
                var helpSkillId = ConfigManager.GetSkillConfig(skill.skillCfg.HelpSkill).Id;
                foreach (var unit in unitsInRange)
                {
                    if (!unit.isHero)
                        continue;
                    var targetHeroCfg = HeroConfig.GetConfig(unit.heroId);
                    var tarJobCfg = ConfigManager.GetJobConfig(targetHeroCfg.Job);
                    if (skill.skillCfg.HelpSkillJob != "" && !skill.skillCfg.HelpSkillJob.Contains(tarJobCfg.NameS))
                        continue;
                    unit.AddSkill(helpSkillId, skill.skillId);
                }
            }
        }
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

    public static void OnCheckCD(Chess caster, SkillConfig skillCfg, ref float cdTime)
    {
        foreach (var skill in caster.skills)
        {
            if(skill.skillId != skillCfg.Id) //防止自己判定自己
                skill.OnCheckCD(skillCfg, ref cdTime);
        }
    }
    public static void OnCheckSummonTime(Chess caster, SkillConfig skillCfg, ref float summonTime)
    {
        foreach (var skill in caster.skills)
        {
            if(skill.skillId != skillCfg.Id) //防止自己判定自己
                skill.OnCheckSummonTime(skillCfg, ref summonTime);
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
            if (!skill.IsInCD() && skill.CheckAidSkill())
            {
                attacker.attackPoint -= skill.skillCfg.AttackPointReduce;
                return true;
            }
        }
        return false;
    }

    public static void OnAddBuff(Chess target, Chess caster, ref int buffId, int skillId, ref float time)
    {
        foreach (var skill in caster.skills)
        {
            skill.OnAddBuff(target, ref buffId, skillId, ref time);
        }
        foreach (var skill in target.skills)
        {
            skill.OnBeAddBuff(caster, ref buffId, skillId, ref time);
        }
    }

    public static void OnDoSkillDamage(Chess target, Chess caster, SkillConfig skillCfg, ref int damage, bool isFeedback)
    {
        foreach (var skill in caster.skills)
        {
            if(skillCfg.Id == skill.skillId)
                continue;
            skill.OnDoSkillDamage(target, skillCfg, ref damage, isFeedback);
        }
        foreach (var skill in target.skills)
        {
            if (skillCfg.Id == skill.skillId)
                continue;
            skill.OnBeDoSkillDamage(caster, skillCfg, ref damage, isFeedback);
        }
    }

    
    public static void OnHealTarget(Chess healer, Chess target, int checkSkillId, ref int addon)
    {
        foreach (var skill in healer.skills)
        {
            skill.OnHealTarget(target, checkSkillId, ref addon);
        }
    }

}
