using CommonConfig;

/// <summary>
/// 受击加护盾（ScriptName = "AttackedShield"）：受到攻击时为自己施加数值型吸收盾，
/// 护盾容量 = 自身攻击 × SkillDamageRate，护盾 Buff 取技能行 BuffId（"盾"= BuffConfig 301001），持续 BuffTime 秒，
/// 是否可触发由 Skill.CheckBurst 统一判定（CD、发动概率、MpCost、TriggerCondition 条件如 "hprate&lt;30"）。
/// 每次触发还会直接给自身叠加生命回复（JobLinkManager.ApplyAttr "hpRegen"），数值取 StrengthInt，可逐次累积。
/// 使用示例：老当益壮 · 宝刀未老（缩写「壮」，2010106~2010110，生命低于30%受击时给自己 攻击×240%~480% 的护盾，并永久 +生命回复2~6，CD 10s）。
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

        var shieldCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (shieldCfg == null)
        {
            GameLog.Error($"受击加护盾技能缺少护盾Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }

        owner.PlayerAnim(skillCfg.Action);

        // 数值型吸收盾：容量按自身攻击 × 比例计算
        BuffManager.AddBuff(owner, owner, id, shieldCfg.Id, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldCfg.Id) as BuffShield;
        if (shield != null)
            shield.SetHp((int)(owner.atk * skillCfg.SkillDamageRate));

        // 每次触发叠加生命回复（越老越耐战）
        JobLinkManager.ApplyAttr(owner, "hpRegen", skillCfg.StrengthInt);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"受击加护盾 技能id={id} 等级={Level} 生命={owner.hp}/{owner.maxHp} 护盾={owner.atk * skillCfg.SkillDamageRate:0} 生命回复+{skillCfg.StrengthInt}");
    }
}
