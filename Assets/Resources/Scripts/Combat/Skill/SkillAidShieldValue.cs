using CommonConfig;

/// <summary>
/// 疑城（徐盛）：MP充满后为自己施加减伤盾（BuffId="硬"=BuffShieldValue 300002），
/// 减伤比例 = 技能 StrengthBuff1[0]（如 0.25 = 减免25%伤害），持续 BuffTime 秒，CD 到后自动补盾。
/// 充能由 MpCost 控制（盾兵 MpRegen=1/秒），开场 MP 拉满即可立即施放，之后按 MP/CD 循环。
/// </summary>
public class SkillAidShieldValue : Skill
{
    public SkillAidShieldValue(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"疑城减伤盾技能缺少Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"疑城减伤盾 技能id={id} 等级={Level} 减伤={skillCfg.StrengthBuff1[0] * 100:0}% 持续={skillCfg.BuffTime}s");
        return true;
    }
}
