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
            case "AttackedBuff":
                return new SkillAttackedBuff(skillId, owner);
            case "AttackRunCross":
                return new SkillAttackRunCross(skillId, owner);
            case "AttackRunCrossPlus":
                return new SkillAttackRunCrossPlus(skillId, owner);                
            case "HelpAidBuff":
                return new SkillHelpAidBuff(skillId, owner);
            case "AttackShieldPierce":
                return new SkillAttackShieldPierce(skillId, owner);
            case "AttackArmorPierce":
                return new SkillAttackArmorPierce(skillId, owner);

            case "DefFeedback":
                return new SkillDefFeedback(skillId, owner);
            case "AttackSpeedAttack":
                return new SkillAttackSpeedAttack(skillId, owner);
            case "AttackReboundArrow":
                return new SkillAttackReboundArrow(skillId, owner);
            case "DefHpLow":
                return new SkillDefHpLow(skillId, owner);
            case "HitBuff":
                return new SkillHitBuff(skillId, owner);
            case "HitBuffArea":
                return new SkillHitBuffArea(skillId, owner);
            case "HitRegion":
                return new SkillHitRegion(skillId, owner);
            case "HitWall":
                return new SkillHitWall(skillId, owner);
            case "HitFireArea":
                return new SkillHitFireArea(skillId, owner);
            case "DamageReal":
                return new SkillDamageReal(skillId, owner);
            case "AttackedShadow":
                return new SkillAttackedShadow(skillId, owner);

            case "HitTeleport":
                return new SkillHitTeleport(skillId, owner);
            case "HitRepeat":
                return new SkillHitRepeat(skillId, owner);
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
            case "ModifyShootSpeed":
                return new SkillModifyShootSpeed(skillId, owner);

            case "InitAttrChange":
                return new SkillInitAttrChange(skillId, owner);

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
                    unit.AddSkill(helpSkillId, skill.id);
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

    public static void DuringAttack(Chess attacker, Chess defender, ref int damageBase, ref float damageMulti, ref string effect)
    {       
        foreach(var skill in attacker.skills)
        {
            skill.DuringAttack(defender, ref damageBase, ref damageMulti, ref effect);

        }    
        foreach(var skill in defender.skills)
        {
            skill.DuringAttacked(attacker, ref damageBase, ref damageMulti, ref effect);

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

    /// <summary>
    /// 伤害结算前·攻击方修正：技能伤害修正（Skill.BeforeCalDamage）。
    /// 普攻(Attack)时 skillCfg 传 null 表示非技能伤害，直接跳过
    /// </summary>
    public static void BeforeCalDamage(Chess attacker, Chess defender, SkillConfig skillCfg, ref int damage, string hurtTag, bool isFeedback)
    {
        foreach (var skill in attacker.skills)
        {
            if (skillCfg != null && skillCfg.Id == skill.skillId)
                continue;
            skill.BeforeCalDamage(defender, skillCfg, ref damage, hurtTag, isFeedback);
        }
    }

    /// <summary>
    /// 伤害结算前·受击方修正：护盾吸收（Buff.BeforeCalDamaged 按 hurtTag 决定是否吸收，如"AntiShield"绕过护盾打血，普通攻击传空）+ 技能受击修正（Skill.BeforeCalDamaged）。
    /// 普攻(Attack)时 skillCfg 传 null，只做护盾吸收，跳过技能受击修正
    /// </summary>
    public static void BeforeCalDamaged(Chess attacker, Chess defender, SkillConfig skillCfg, ref int damage, string hurtTag, bool isFeedback)
    {
        // 护盾吸收（AntiShield 标签时护盾不吸收，直接放行打血）
        foreach (var buff in defender.buffs)
        {
            buff.BeforeCalDamaged(attacker, ref damage, hurtTag);
        }

        foreach (var skill in defender.skills)
        {
            if (skillCfg != null && skillCfg.Id == skill.skillId)
                continue;
            skill.BeforeCalDamaged(attacker, skillCfg, ref damage, hurtTag, isFeedback);
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
            if (!skill.IsInCD() && skill.CheckAidSkill())
            {
                // 辅助技能释放消耗气力（负数=延长冷却，走 Cooldown 统一限制在0~1）
                attacker.Cooldown(-skill.skillCfg.AttackPointReduce);
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

    public static void OnHealTarget(Chess healer, Chess target, int checkSkillId, ref int addon)
    {
        foreach (var skill in healer.skills)
        {
            skill.OnHealTarget(target, checkSkillId, ref addon);
        }
    }

}
