using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 受击加护盾 / 回血（ScriptName = "SkillAttackedShield"）：受到攻击时为自己施加吸收盾或减伤盾，
/// 是否可触发由 Skill.CheckBurst 统一判定（CD、发动概率、MpCost、TriggerCondition 条件如 "hprate&lt;30"）。
/// Buff 短名由 SkillConfig.BuffId 配置：第 1 个为主 Buff——
///   吸收盾「盾」：容量 = Strength2[0] × (100+法强)/100（随法强成长，如严颜·老当益壮 / 邓艾·偷渡阴平）；
///   减伤盾「硬」：减伤率取 StrengthBuff1[0]（如益壮）。
/// 回血 Buff「愈」为可选第 2 段，持续秒数：
///   · StrengthBuff1 长度≥3 时取 [2]（固定值模式，如益壮「硬,愈」=[减伤率, 每秒回血, 持续秒数]）；
///   · 否则取 BuffTime（成长模式，如邓艾，每秒回血由 BuffTimeHeal 回退到 Skill.GetSkillHeal）。
/// TargetType 标记：
///   · ignoreWeakHit = 不忽略 10 点以下的弱伤害（邓艾·偷渡阴平）；缺省时 damage&lt;=10 不触发。
/// 使用示例：益壮（2010106~2010110）、老当益壮（2020716~2020720）、偷渡阴平（2020726~2020730）。
/// </summary>
public class SkillAttackedShield : Skill
{
    public SkillAttackedShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        // 默认忽略 10 点以下的弱伤害，避免被小兵普攻骗出 CD；ignoreWeakHit 时不做该过滤（邓艾·偷渡阴平）
        if (!HasSkillFlag("ignoreWeakHit") && damage <= 10)
            return;
        if (!CheckBurst(attacker))
            return;

        // BuffId 逗号分隔：buffIds[0]=主 Buff（吸收盾/减伤盾），buffIds[1]=回血 Buff（可选）
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"受击加护盾技能缺少护盾Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }
        int shieldBuffId = buffIds[0];

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, shieldBuffId, skillCfg.BuffTime);
        // 吸收盾才需要写入容量；减伤盾「硬」的数值由 BuffShieldValue 直接读 StrengthBuff1
        var shield = owner.GetBuff(shieldBuffId) as BuffShield;
        var shieldHp = GetSkillShield(0);
        if (shield != null)
            shield.SetHp(shieldHp);

        // 触发后在限时内持续回血；固定值（益壮）与随法强成长（邓艾）由 BuffTimeHeal 自行区分
        var healTime = GetHealTime(buffIds.Count);
        if (healTime > 0f)
        {
            if (buffIds.Count < 2)
                GameLog.Error($"受击加护盾技能缺少回血Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            else
                BuffManager.AddBuff(owner, owner, id, buffIds[1], healTime);
        }

        PlaySelfEffect();
        GameLog.Debug($"受击加护盾 技能id={id} 等级={Level} 生命={owner.hp}/{owner.maxHp} 护盾={shieldHp} 持续回血={GetHealPerTick(buffIds.Count)}/秒×{healTime}秒");
    }

    // 回血持续秒数：StrengthBuff1=[减伤率, 每秒回血, 持续秒数] 时取 [2]（益壮）；否则有第 2 个回血 Buff 时取 BuffTime（邓艾）
    private float GetHealTime(int buffCount)
    {
        if (skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length >= 3)
            return skillCfg.StrengthBuff1[2];
        return buffCount >= 2 ? skillCfg.BuffTime : 0f;
    }

    // 每秒回血值（仅用于日志）：固定值模式取 StrengthBuff1[1]；成长模式取 GetSkillHeal()
    private int GetHealPerTick(int buffCount)
    {
        if (skillCfg.StrengthBuff1 != null && skillCfg.StrengthBuff1.Length >= 2 && skillCfg.StrengthBuff1[1] > 0f)
            return (int)skillCfg.StrengthBuff1[1];
        return buffCount >= 2 ? GetSkillHeal() : 0;
    }
}
