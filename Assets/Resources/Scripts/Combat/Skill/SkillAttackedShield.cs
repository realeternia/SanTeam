using CommonConfig;

/// <summary>
/// 受击加护盾（ScriptName = "AttackedShield"）：受到攻击时为自己施加数值型吸收盾，
/// 护盾容量 = Strength2[0] × (100+法强)/100（随法强成长，与「护卫」同口径），护盾 Buff 取技能行 BuffId（"盾"= BuffConfig 300001），持续 BuffTime 秒，
/// 是否可触发由 Skill.CheckBurst 统一判定（CD、发动概率、MpCost、TriggerCondition 条件如 "hprate&lt;30"）。
/// 可选：配置 StrengthBuff1（[0]=每秒回血、[1]=持续秒数）时，触发后在限时内持续回血；未配置则只套盾。
/// 使用示例：老当益壮 · 宝刀未老（缩写「老」，2010106~2010110，生命低于30%受击时给自己套 140~300 基值护盾（随法强），并在 10 秒内每秒回复 2~6 点生命，CD 15s）。
/// </summary>
public class SkillAttackedShield : Skill
{
    /// <summary>持续回血Buff（BuffTimeHeal）短名：技能行 BuffId 已被护盾占用，回血Buff在此固定引用「愈」</summary>
    private const string HealBuffNameS = "愈";

    public SkillAttackedShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        if (damage <= 10 || !CheckBurst(attacker))
            return;

        var shieldCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (shieldCfg == null)
        {
            GameLog.Error($"受击加护盾技能缺少护盾Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }

        owner.PlayerAnim(skillCfg.Action);

        // 数值型吸收盾：容量 = Strength2[0] × (100+法强)/100（随法强成长）
        BuffManager.AddBuff(owner, owner, id, shieldCfg.Id, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldCfg.Id) as BuffShield;
        var shieldHp = GetSkillShield(0);
        if (shield != null)
            shield.SetHp(shieldHp);

        // 触发后在限时内持续回血（固定值，不随法强成长），取代此前的永久生命回复叠加
        ApplyHealOverTime();

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"受击加护盾 技能id={id} 等级={Level} 生命={owner.hp}/{owner.maxHp} 护盾={shieldHp} 持续回血={GetHealPerTick()}/秒×{GetHealTime()}秒");
    }

    // 施加限时回血Buff：每秒回血值与持续秒数取技能行 StrengthBuff1（[0]=每秒回血，[1]=持续秒数）
    private void ApplyHealOverTime()
    {
        var healTime = GetHealTime();
        if (healTime <= 0f)
            return; // 未配置回血（StrengthBuff1 缺省）时仅套盾

        var healCfg = BuffConfig.GetConfigByNameS(HealBuffNameS);
        if (healCfg == null)
        {
            GameLog.Error($"受击加护盾技能缺少回血Buff配置: NameS={HealBuffNameS} 技能id={id}");
            return;
        }

        BuffManager.AddBuff(owner, owner, id, healCfg.Id, healTime);
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
