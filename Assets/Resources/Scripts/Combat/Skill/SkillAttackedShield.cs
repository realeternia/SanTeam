using CommonConfig;

/// <summary>
/// 受击加护盾（ScriptName = "AttackedShield"）：受到攻击时为自己施加数值型吸收盾，
/// 护盾容量 = Strength2[0] × (100+法强)/100（随法强成长，与「护卫」同口径），持续 BuffTime 秒，
/// 是否可触发由 Skill.CheckBurst 统一判定（CD、发动概率、MpCost、TriggerCondition 条件如 "hprate&lt;30"）。
/// Buff 短名由 SkillConfig.BuffId 配置：第 1 个=护盾Buff；配置了 StrengthBuff1（[0]=每秒回血、[1]=持续秒数）时，
/// 第 2 个为限时回血Buff（如 "盾,愈"）；未配置 StrengthBuff1 时只套盾，可只填护盾一个短名。
/// 使用示例：益壮（缩写「老」，2010106~2010110，生命低于30%受击时给自己套 140~300 基值护盾（随法强），并在 10 秒内每秒回复 2~6 点生命，CD 15s）。
/// </summary>
public class SkillAttackedShield : Skill
{
    public SkillAttackedShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        if (damage <= 10 || !CheckBurst(attacker))
            return;

        // BuffId 逗号分隔：buffIds[0]=护盾Buff，buffIds[1]=回血Buff（仅配置了 StrengthBuff1 时需要）
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"受击加护盾技能缺少护盾Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }
        int shieldBuffId = buffIds[0];

        owner.PlayerAnim(skillCfg.Action);

        // 数值型吸收盾：容量 = Strength2[0] × (100+法强)/100（随法强成长）
        BuffManager.AddBuff(owner, owner, id, shieldBuffId, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldBuffId) as BuffShield;
        var shieldHp = GetSkillShield(0);
        if (shield != null)
            shield.SetHp(shieldHp);

        // 触发后在限时内持续回血（固定值，不随法强成长），取代此前的永久生命回复叠加
        ApplyHealOverTime(buffIds);

        PlaySelfEffect();
        GameLog.Debug($"受击加护盾 技能id={id} 等级={Level} 生命={owner.hp}/{owner.maxHp} 护盾={shieldHp} 持续回血={GetHealPerTick()}/秒×{GetHealTime()}秒");
    }

    // 施加限时回血Buff：每秒回血值与持续秒数取技能行 StrengthBuff1（[0]=每秒回血，[1]=持续秒数），回血Buff短名取 BuffId 第 2 项
    private void ApplyHealOverTime(System.Collections.Generic.List<int> buffIds)
    {
        var healTime = GetHealTime();
        if (healTime <= 0f)
            return; // 未配置回血（StrengthBuff1 缺省）时仅套盾

        if (buffIds.Count < 2)
        {
            GameLog.Error($"受击加护盾技能缺少回血Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }

        BuffManager.AddBuff(owner, owner, id, buffIds[1], healTime);
    }

    // 每秒回血值（StrengthBuff1[0]，固定值不随法强成长；未配置返回0）
    private int GetHealPerTick()
    {
        if (skillCfg.StrengthBuff1 == null || skillCfg.StrengthBuff1.Length < 1)
            return 0;
        return (int)skillCfg.StrengthBuff1[0];
    }

    // 回血持续秒数（StrengthBuff1[1]；未配置返回0）
    private float GetHealTime()
    {
        if (skillCfg.StrengthBuff1 == null || skillCfg.StrengthBuff1.Length < 2)
            return 0f;
        return skillCfg.StrengthBuff1[1];
    }
}
