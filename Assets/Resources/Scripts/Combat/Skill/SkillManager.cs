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
            case "AidBuffLowHp":
                return new SkillAidBuffLowHp(skillId, owner);
            case "AttackShieldPierce":
                return new SkillAttackShieldPierce(skillId, owner);
            case "AttackArmorPierce":
                return new SkillAttackArmorPierce(skillId, owner);
            case "AttackStunDamage":
                return new SkillAttackStunDamage(skillId, owner);
            case "AttackMagicDamage":
                return new SkillAttackMagicDamage(skillId, owner);
            case "DeathGroupHeal":
                return new SkillDeathGroupHeal(skillId, owner);

            case "DefFeedback":
                return new SkillDefFeedback(skillId, owner);
            case "AttackSpeedAttack":
                return new SkillAttackSpeedAttack(skillId, owner);
            case "AttackReboundArrow":
                return new SkillAttackReboundArrow(skillId, owner);
            case "ReduceDamageRate":
                return new SkillDefReduceDamageRate(skillId, owner);
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
            case "BuffExpand":
                return new SkillBuffExpand(skillId, owner);
            case "BuffExpandPos":
                return new SkillBuffExpandPos(skillId, owner);                
            case "ModifyBuffTime":
                return new SkillModifyBuffTime(skillId, owner);
            case "ModifyShootSpeed":
                return new SkillModifyShootSpeed(skillId, owner);
            case "AidDrain":
                return new SkillAidDrain(skillId, owner);

            case "InitAttrChange":
                return new SkillInitAttrChange(skillId, owner);
            case "InitSneakChangePos":
                return new SkillInitSneakChangePos(skillId, owner);
            case "InitAddItem":
                return new SkillInitAddItem(skillId, owner);
            case "InitAddItemChance":
                return new SkillInitAddItemChance(skillId, owner);
            case "InitEnemyRandomBuff":
                return new SkillInitEnemyRandomBuff(skillId, owner);
            case "AttackedShield":
                return new SkillAttackedShield(skillId, owner);

            case "FactionShield":
                return new SkillFactionShield(skillId, owner);

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

    public static void OnDeath(Chess chess)
    {
        foreach (var skill in chess.skills)
        {
            skill.OnDeath();
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

    /// <summary>
    /// 伤害计算阶段·统一入口：普攻与技能伤害都进入，调整伤害基数与倍率（增伤/减伤/破甲/连锁等）。
    /// 普攻(Attack)时 castSkillCfg 传 null；技能伤害时传当前施放技能并跳过该技能自身
    /// </summary>
    public static void BeforeCalDamaged(Chess attacker, Chess defender, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        foreach (var skill in attacker.skills)
        {
            if (castSkillCfg != null && castSkillCfg.Id == skill.skillId)
                continue;
            skill.BeforeCalDamage(defender, castSkillCfg, ref damageBase, ref damageMulti, ref effect, hurtTag, isFeedback);
        }
        foreach (var buff in attacker.buffs)
        {
            buff.BeforeCalDamage(defender, ref damageBase, ref damageMulti, ref effect, hurtTag);
        }

        foreach (var skill in defender.skills)
        {
            if (castSkillCfg != null && castSkillCfg.Id == skill.skillId)
                continue;
            skill.BeforeCalDamaged(attacker, castSkillCfg, ref damageBase, ref damageMulti, ref effect, hurtTag, isFeedback);
        }
        foreach (var buff in defender.buffs)
        {
            buff.BeforeCalDamaged(attacker, ref damageBase, ref damageMulti, ref effect, hurtTag);
        }
    }

    /// <summary>
    /// 伤害结算阶段·受击方：只做伤害吸收（护盾 BuffShield），不做伤害放大。
    /// 普攻(Attack)时 castSkillCfg 传 null；护盾按 hurtTag 决定是否吸收（如"AntiShield"绕过护盾打血）
    /// </summary>
    public static void DuringCalDamage(Chess attacker, Chess defender, SkillConfig castSkillCfg, ref int damage, string hurtTag, bool isFeedback)
    {
        foreach (var buff in defender.buffs)
        {
            buff.DuringCalDamage(attacker, ref damage, hurtTag);
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
