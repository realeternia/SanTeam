using CommonConfig;

/// <summary>
/// 疑城·开场减伤盾（ScriptName = "InitShieldValue"）：
/// 战斗开始时给自己施加减伤盾（BuffId="硬"=BuffShieldValue 300002），
/// 持续 BuffTime 秒，减伤比例 = 技能 Strength（如 0.3 = 减免30%伤害）。
/// 用于徐盛：开场15秒百分比减伤盾。
/// </summary>
public class SkillInitShieldValue : Skill
{
    public SkillInitShieldValue(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"开场减伤盾技能缺少Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return;
        }

        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        GameLog.Debug($"开场减伤盾 技能id={id} 等级={Level} 减伤={skillCfg.Strength * 100:0}% 持续={skillCfg.BuffTime}s");
    }
}
