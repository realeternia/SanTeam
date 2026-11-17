using CommonConfig;

/// <summary>
/// 开场减伤盾（ScriptName = "SkillEquipInitShieldValue"）：
/// 战斗开始时给自己施加减伤盾（BuffId="硬"=BuffShieldValue 300002），
/// 持续 BuffTime 秒，减伤比例 = 技能 StrengthBuff1 的 键 shieldReduce（如 0.3 = 减免30%伤害）。
/// 使用者为道具技能：玄盾（玉玺）/ 文润（诗经）/ 玄武（凤凰羽衣）。
/// </summary>
public class SkillEquipInitShieldValue : Skill
{
    public SkillEquipInitShieldValue(int id, Chess unit) : base(id, unit)
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
        if (!string.IsNullOrEmpty(skillCfg.SelfEffect))
            PlaySelfEffect();
        GameLog.Debug($"开场减伤盾 技能id={id} 等级={Level} 减伤={GetStrBuff("shieldReduce") * 100:0}% 持续={skillCfg.BuffTime}s");
    }
}
