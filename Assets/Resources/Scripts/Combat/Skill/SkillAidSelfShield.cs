using CommonConfig;

/// <summary>
/// 温良（温良恭俭）：每 CD(8秒) 为自己提升 StrengthBuff1[0] 点护甲，持续到 Buff 结束（BuffTime=999），CD 到自动续上。
/// 护甲施加/移除由 BuffArmorAdd 统一处理（短名「甲」）；重复施放只刷新持续时间，不叠加数值。
/// </summary>
public class SkillAidSelfShield : Skill
{
    public SkillAidSelfShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"温良 武将{owner.heroId} BuffId未配置");
            return false;
        }

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, buffIds[0], skillCfg.BuffTime);
        GameLog.Debug($"温良 武将{owner.heroId} 提升护甲 技能id={id} 等级={Level}");

        PlaySelfEffect();
        return true;
    }
}
