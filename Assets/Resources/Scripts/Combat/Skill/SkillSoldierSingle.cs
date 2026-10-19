using CommonConfig;

/// <summary>
/// 蒋琬·抚军（ScriptName = "SkillSoldierSingle"）：
/// 为一名友方英雄套盾（key "shield"，随法强成长），其周围 skillCfg.Area 半径内每名小兵额外提升
/// key "perSoldier" 比例的护盾量；护盾时长取 skillCfg.BuffTime（BuffId 配"盾"）。
/// </summary>
public class SkillSoldierSingle : Skill
{
    public SkillSoldierSingle(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillSoldierSingle: 未配置护盾Buff（BuffId）技能id={id}");
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

        // 目标周围同侧小兵越多，护盾越厚
        var soldierCount = WorldManager.Instance.GetMySideInRange(target.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => !x.isHero).Count;
        var shield = (int)(GetSkillShield() * (1f + GetStr2("perSoldier") * soldierCount));

        var buffId = buffIds[0];
        BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
        var sh = target.GetBuff(buffId) as BuffShield;
        if (sh != null)
            sh.SetHp(shield);
        else
            GameLog.Warn($"SkillSoldierSingle: 护盾Buff类型异常，无法设置护盾值 技能id={id}");

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
