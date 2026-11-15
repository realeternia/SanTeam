using CommonConfig;

/// <summary>
/// 荀彧·兵精（ScriptName = "SkillAidSoldierBless"）：
/// 每次释放祝福一名友方英雄（生命比例最低者，可重复祝福），为其附加攻击(键 atk)、护甲(键 armor)、
/// 魔抗(键 magicRes) 大幅提升的 Buff（BuffId 配"攻,甲,抗"），持续 skillCfg.BuffTime 秒。
/// </summary>
public class SkillAidSoldierBless : Skill
{
    public SkillAidSoldierBless(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidSoldierBless: 未配置祝福Buff（BuffId）技能id={id}");
            return false;
        }

        // 目标：按 SelectOnly / SelectFirst 选（典型 ally;hero;inFight + lowestHp）
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, true, 1);
        if (picked.Count == 0)
            return false;
        var target = picked[0];

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 增益时长固定为配置的 BuffTime
        foreach (var buffId in buffIds)
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        GameLog.Info($"兵精：{owner.name} 祝福友方英雄，持续{skillCfg.BuffTime.ToString("0.#")}秒 技能id={id}");
        return true;
    }
}
