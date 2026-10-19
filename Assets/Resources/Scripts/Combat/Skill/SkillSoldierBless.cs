using System.Linq;
using CommonConfig;

/// <summary>
/// 荀彧·兵精（ScriptName = "SkillSoldierBless"）：
/// 每次释放祝福一名友方英雄（生命比例最低者，可重复祝福），为其附加攻击(键 atk)、护甲(键 armor)、
/// 魔抗(键 magicRes) 大幅提升的 Buff（BuffId 配"攻,甲,抗"）；增益时长 = skillCfg.BuffTime ×
/// (1 + 键 perSoldier × 目标周围 skillCfg.Area 半径内小兵数)，身边小兵越多持续时间越长。
/// </summary>
public class SkillSoldierBless : Skill
{
    public SkillSoldierBless(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillSoldierBless: 未配置祝福Buff（BuffId）技能id={id}");
            return false;
        }

        // 目标：范围内生命比例最低的友方英雄
        var heroes = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .Where(x => x.isHero && x.IsInFight()).ToList();
        if (heroes.Count == 0)
            return false;

        Chess target = null;
        foreach (var h in heroes)
            if (target == null || h.HpRate < target.HpRate)
                target = h;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 目标周围小兵越多，增益持续越久
        var soldierCount = WorldManager.Instance.GetMySideInRange(target.transform.position, skillCfg.Area, owner.side)
            .Count(x => !x.isHero);
        var time = skillCfg.BuffTime * (1f + GetStr2("perSoldier") * soldierCount);

        foreach (var buffId in buffIds)
            BuffManager.AddBuff(target, owner, id, buffId, time);

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        GameLog.Info($"兵精：{owner.name} 祝福友方英雄，持续{time.ToString("0.#")}秒 技能id={id}");
        return true;
    }
}
