using CommonConfig;

/// <summary>
/// 偷渡阴平·残血急救+护盾（ScriptName = "SkillLowHpFullHealShield"）：
/// 当生命低于30%(TriggerCondition=hprate<30)且受到攻击时触发：
/// 1. 给自己施加快速回血buff，每秒回复 HealStrength×(100+法强)/100 点生命（统一走 Skill.GetSkillHeal），持续 BuffTime 秒
/// 2. 给自己施加一个吸收盾，容量=Strength2[0]×(100+法强)/100（随法强成长）
/// 冷却极长(配置中 CD=999)，单局基本只能触发一次。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=护盾Buff，第 2 个=回血Buff（如 "盾,愈"）。
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

        // BuffId 逗号分隔：buffIds[0]=护盾Buff，buffIds[1]=回血Buff
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 2)
        {
            GameLog.Error($"SkillLowHpFullHealShield: BuffId 需配置护盾与回血两个buff（如\"盾,愈\"）技能id={id}");
            return;
        }
        int shieldBuffId = buffIds[0];
        int healBuffId = buffIds[1];

        owner.PlayerAnim(skillCfg.Action);

        // 快速回血buff（每秒回血由 BuffTimeHeal 走 Skill.GetSkillHeal：HealStrength×(100+法强)/100）
        BuffManager.AddBuff(owner, owner, id, healBuffId, skillCfg.BuffTime);

        // 吸收盾：容量 = Strength2[0] × (100+法强)/100（随法强成长）
        BuffManager.AddBuff(owner, owner, id, shieldBuffId, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldBuffId) as BuffShield;
        if (shield != null)
            shield.SetHp(GetSkillShield(0));

        PlaySelfEffect();
        GameLog.Debug($"偷渡阴平触发 技能id={id} 等级={Level} 每秒回血={GetSkillHeal()} 护盾={GetSkillShield(0)} 持续={skillCfg.BuffTime}s 冷却={skillCfg.CD}s");
    }
}
