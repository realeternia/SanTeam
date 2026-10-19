using CommonConfig;

/// <summary>
/// 马良·励军（ScriptName = "SkillSoldierGrow"）：
/// 为一名友方英雄及其周围 skillCfg.Area 半径内的小兵附加攻击提升（BuffId 配"攻"，数值取
/// StrengthBuff1 的 key "atk"，时长 skillCfg.BuffTime）。
/// </summary>
public class SkillSoldierGrow : Skill
{
    public SkillSoldierGrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillSoldierGrow: 未配置加攻Buff（BuffId）技能id={id}");
            return false;
        }

        // 目标：范围内生命比例最低的友方英雄
        var heroes = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x.isHero && x.IsInFight());
        if (heroes.Count == 0)
            return false;

        Chess target = null;
        foreach (var h in heroes)
            if (target == null || h.HpRate < target.HpRate)
                target = h;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 英雄本体 + 其周围小兵一同获得加攻增益
        var buffId = buffIds[0];
        BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
        foreach (var s in WorldManager.Instance.GetMySideInRange(target.transform.position, skillCfg.Area, owner.side))
        {
            if (s.isHero)
                continue;
            BuffManager.AddBuff(s, owner, id, buffId, skillCfg.BuffTime);
        }

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
