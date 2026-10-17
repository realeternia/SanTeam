using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using CommonConfig;

public static class SkillManager
{
    /// <summary>
    /// ScriptName → 技能脚本类构造器缓存：技能创建只在开局/加技能时发生（非热路径），缓存避免重复反射查找。
    /// </summary>
    private static readonly Dictionary<string, ConstructorInfo> ctorCache = new Dictionary<string, ConstructorInfo>();

    public static Skill CreateSkill(int skillId, Chess owner)
    {
        var skillCfg = SkillConfig.GetConfig(skillId);
        var ctor = GetSkillConstructor(skillCfg.ScriptName);
        if (ctor == null)
            throw new System.Exception("Skill not found " + skillCfg.ScriptName);
        return (Skill)ctor.Invoke(new object[] { skillId, owner });
    }

    /// <summary>
    /// 按 ScriptName 反射取技能脚本类的 (int, Chess) 构造器（约定 ScriptName 即类名，如 "SkillAidShockWave"）。
    /// 未带 Skill 前缀时自动补全（兼容历史配置）；找不到类或构造器时记录日志并返回 null。
    /// </summary>
    private static ConstructorInfo GetSkillConstructor(string scriptName)
    {
        if (string.IsNullOrEmpty(scriptName))
            return null;
        if (ctorCache.TryGetValue(scriptName, out var cached))
            return cached;

        var typeName = scriptName.StartsWith("Skill") ? scriptName : "Skill" + scriptName;
        var type = typeof(Skill).Assembly.GetType(typeName);
        ConstructorInfo ctor = null;
        if (type != null && typeof(Skill).IsAssignableFrom(type))
            ctor = type.GetConstructor(new[] { typeof(int), typeof(Chess) });
        if (ctor == null)
            GameLog.Error("SkillManager: 未找到技能脚本类 " + typeName + "（ScriptName=" + scriptName + "）");

        ctorCache[scriptName] = ctor;
        return ctor;
    }

    public static void BattleBegin(Chess chess)
    {
        foreach (var skill in chess.skills)
        {
            skill.BattleBegin();
        }

        // 开局初始MP（SkillConfig.StartMpRate，默认0=从0充能）：放在各技能 BattleBegin 之后，
        // 覆盖技能自身的开局充能（如运筹），并让 BattleBegin 期间新增的技能同样生效
        foreach (var skill in chess.skills)
            skill.InitStartMp();

        // 全局调试开关（CombatConst.FullMpAtBattleBegin，默认关闭）：开启时无视 StartMpRate，所有技能直接满MP
        if (CombatConst.FullMpAtBattleBegin)
        {
            foreach (var skill in chess.skills)
                skill.mp = skill.skillCfg.MpCost;
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
    /// 普攻(Attack)时 castSkillCfg 传 null；护盾按 hurtTag 决定是否吸收（如"AntiShield"绕过护盾打血）；
    /// 真实伤害(DamageType=2)同样无视护盾吸收（护盾不吸收真实伤害）
    /// </summary>
    public static void DuringCalDamage(Chess attacker, Chess defender, SkillConfig castSkillCfg, ref int damage, string hurtTag, bool isFeedback, bool isRealDamageAttack = false)
    {
        var isRealDamage = isRealDamageAttack
            || (castSkillCfg != null && castSkillCfg.DamageType == CombatConst.DamageTypeReal);
        foreach (var buff in defender.buffs)
        {
            // 真实伤害：跳过吸收型护盾(BuffShield)，其余 buff 照常结算
            if (isRealDamage && buff is BuffShield)
                continue;
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
