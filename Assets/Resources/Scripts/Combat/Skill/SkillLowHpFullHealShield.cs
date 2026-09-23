using CommonConfig;

/// <summary>
/// 偷渡阴平·残血急救+护盾（ScriptName = "LowHpFullHealShield"）：
/// 当生命低于30%(TriggerCondition=hprate<30)且受到攻击时触发：
/// 1. 给自己施加快速回血buff（"愈"），每秒回复 最大生命×Strength + StrengthInt 点生命，持续 BuffTime 秒
/// 2. 给自己施加一个吸收盾，容量=自身最大生命×SkillDamageRate
/// 冷却极长(配置中 CD=999)，单局基本只能触发一次。
/// 用于邓艾：孤军深入绝境逢生，背水一战逐步回血。
/// </summary>
public class SkillLowHpFullHealShield : Skill
{
    public SkillLowHpFullHealShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, int damage)
    {
        // CheckBurst 已包含 CD 判定与触发条件(hprate<30)判定
        if (!CheckBurst(attacker))
            return;

        owner.PlayerAnim(skillCfg.Action);

        // 快速回血buff（每秒回血 = maxHp×Strength + StrengthInt）
        var healBuffId = BuffConfig.GetConfigByNameS("愈").Id;
        BuffManager.AddBuff(owner, owner, id, healBuffId, skillCfg.BuffTime);

        // 吸收盾
        var shieldBuffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(owner, owner, id, shieldBuffId, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldBuffId) as BuffShield;
        if (shield != null)
            shield.SetHp((int)(owner.maxHp * skillCfg.SkillDamageRate));

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"偷渡阴平触发 技能id={id} 等级={Level} 每秒回血={skillCfg.Strength * 100:0}%+{skillCfg.StrengthInt} 护盾={skillCfg.SkillDamageRate * 100:0}% 持续={skillCfg.BuffTime}s 冷却={skillCfg.CD}s");
    }
}
