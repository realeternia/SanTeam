using CommonConfig;

/// <summary>
/// 曹仁·天人守城（ScriptName = "SkillAidDefStack"）：施放后给自身挂「据守」Buff，持续 BuffTime 秒；
/// 期间获得 Strength2 点护甲与魔抗，且每受到一次攻击再叠加「初始双防 × Strength3」（最多 StrengthInt 层）。
/// 加成叠加与到期还原全部由 BuffDefStack 处理，本类只负责施放，数值一律走配置。
/// </summary>
public class SkillAidDefStack : Skill
{
    public SkillAidDefStack(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;

        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"天人守城技能缺少Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return false;
        }

        // 已有据守状态或技能冷却中则不重复施放
        if (owner.GetBuff(buffCfg.Id) != null || IsInCD())
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"天人守城 技能id={id} 等级={Level} 双防={skillCfg.Strength2} 每层={skillCfg.Strength3} 上限={skillCfg.StrengthInt}层 持续={skillCfg.BuffTime}秒");
        return true;
    }
}
