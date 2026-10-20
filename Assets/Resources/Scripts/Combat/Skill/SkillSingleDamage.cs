using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"单体伤害 + 可选增益/减益"技能（ScriptName = "SkillSingleDamage"）：
/// 锁定 skillCfg.Range 射程内的当前目标，造成 GetSkillDamage() 技能伤害，并可选附加 SkillConfig.BuffId
/// 指定的 Buff（英文/中文逗号或分号分隔多个，空=纯伤害），时长 BuffTime。
/// TargetCount>0 时对同一目标连打 TargetCount 段（如荀攸·奇策的段数），否则单段。
///
/// 通过 TargetType 标记列表扩展同族行为（多项逗号分隔，空=默认）：
/// · selfBuff   = BuffId 全部挂给自身（高览·御守 / 陈泰·扬鞭 / 乐进·御斩 / 程普·横扫；护盾类自动写护盾值）
/// · bothBuff   = BuffId[0] 挂目标、其余挂自身（马腾·腾驾 "慑,攻"）
/// · execute    = 目标生命比例 &lt; 键"execHp" 时伤害 × 键"execRate"（颜良·斩杀）
/// · shieldBonus= 目标带吸收盾时伤害 × 键"shieldBonusRate"（曹洪·裂盾）
/// · missingHp  = 伤害 += 伤害 × 键"missingHpRate" × 已损生命比例（孟获·蛮勇）
/// · armorIgnore= 自身普攻无视目标护甲（潘璋·偷刃 / 高顺·破甲）
/// · antiShield = 目标带盾时追加 键"antiShieldRate" 倍破盾伤害，携带 AntiShield 标签（高顺·破甲）
/// · dispel     = 伤害后驱散目标正面 Buff（许攸·傲言 / 张松·舌辩）
/// · mpBurn     = 命中后燃烧目标 键"mpBurn" 点法力（程昱·断粮 / 贾诩·乱阵 / 甘宁·百骑劫营）
///
/// 持续伤害（dot）不再由本脚本结算：统一在 BuffId 里挂 DoT Buff（如"败"），
/// 每秒跳伤由 BuffTimeDamage 读施加技能的 StrengthBuff1 键"dot"（绝对值）结算。
/// </summary>
public class SkillSingleDamage : Skill
{
    public SkillSingleDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override float GetArmorDelta(bool isAttackerSide)
    {
        // 破甲：自身作为攻击方时物理攻击完全无视目标护甲
        return HasSkillFlag("armorIgnore") && isAttackerSide ? -1f : 0f;
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (skillCfg.Range > 0f && !WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 段数：TargetCount>0 时对同一目标连打 N 段（奇策），否则单段
        var hits = skillCfg.TargetCount > 0 ? skillCfg.TargetCount : 1;
        for (var i = 0; i < hits; i++)
        {
            if (target.hp <= 0)
                break;
            var dmg = BuildDamage(target);
            if (dmg > 0)
                target.OnSkillDamaged(owner, id, dmg);
            // 破盾追加伤害：以技能基础伤害为基数，携带 AntiShield 标签绕过护盾直接打血
            if (HasSkillFlag("antiShield") && target.GetBuff(CombatConst.ShieldBuffId) is BuffShield)
            {
                var extra = Math.Max(1, (int)(GetSkillDamage() * GetStr2("antiShieldRate", 1f)));
                target.OnSkillDamaged(owner, id, extra, false, CombatConst.AntiShieldHurtTag);
            }
        }

        // 法力燃烧：燃烧量取本技能数值槽键"mpBurn"，每次释放只结算一次
        if (HasSkillFlag("mpBurn"))
            BurnMpToTarget(target, GetStr2("mpBurn"));

        ApplyBuffs(target);

        if (HasSkillFlag("dispel"))
            BuffManager.DispelPositive(target);

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

    /// <summary>按 TargetType 的伤害标记计算本次伤害（默认=GetSkillDamage()）</summary>
    private int BuildDamage(Chess target)
    {
        var dmg = GetSkillDamage();
        if (HasSkillFlag("execute") && target.HpRate < GetStr2("execHp"))
            dmg = (int)(dmg * GetStr2("execRate", 1f));
        if (HasSkillFlag("shieldBonus") && target.GetBuff(CombatConst.ShieldBuffId) is BuffShield)
            dmg = Math.Max(1, (int)(dmg * GetStr2("shieldBonusRate", 1f)));
        if (HasSkillFlag("missingHp"))
            dmg += (int)(dmg * GetStr2("missingHpRate") * (1f - owner.HpRate));
        return dmg;
    }

    /// <summary>Buff 归属：默认全部挂目标；selfBuff=全部挂自身；bothBuff=首段目标、其余自身</summary>
    private void ApplyBuffs(Chess target)
    {
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
            return;

        if (HasSkillFlag("selfBuff"))
        {
            foreach (var buffId in buffIds)
                AddBuffToSelf(buffId);
            return;
        }

        if (HasSkillFlag("bothBuff"))
        {
            BuffManager.AddBuff(target, owner, id, buffIds[0], skillCfg.BuffTime);
            for (var i = 1; i < buffIds.Count; i++)
                AddBuffToSelf(buffIds[i]);
            return;
        }

        foreach (var buffId in buffIds)
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
    }

    private void AddBuffToSelf(int buffId)
    {
        BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        // 护盾类 buff 需按统一护盾公式写入护盾值（护甲等其它 buff 无需额外赋值）
        var shield = owner.GetBuff(buffId) as BuffShield;
        if (shield != null)
            shield.AddHp(GetSkillShield());
    }

}
