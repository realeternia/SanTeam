using System.Collections.Generic;
using CommonConfig;

/// <summary>
/// 忠勇·自+友军护甲（ScriptName = "SkillAidSelfAndLowHp"）：
/// 辅助技能，给自己提升 StrengthBuff1[0] 点护甲，
/// 同时给附近(Area内)生命比例最低的一名友方英雄也提升等量护甲。
/// 若附近无友方英雄则只给自己加甲。通过 Skill.CheckBurst 由 SkillManager 循环施放。
/// 用于周仓：挨打时本能举盾，顺带护住身旁同袍。
/// </summary>
public class SkillAidSelfAndLowHp : Skill
{
    public SkillAidSelfAndLowHp(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"忠勇 武将{owner.heroId} BuffId未配置");
            return false;
        }
        var armorBuffId = buffIds[0];

        owner.PlayerAnim(skillCfg.Action);

        // 给自己加甲
        BuffManager.AddBuff(owner, owner, id, armorBuffId, skillCfg.BuffTime);

        // 找附近生命比例最低的友军
        var units = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        Chess target = null;
        float lowest = float.MaxValue;
        foreach (var u in units)
        {
            if (u.hp >= u.maxHp)
                continue;
            var rate = u.hp / (float)u.maxHp;
            if (rate < lowest)
            {
                lowest = rate;
                target = u;
            }
        }

        if (target != null)
        {
            BuffManager.AddBuff(target, owner, id, armorBuffId, skillCfg.BuffTime);
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }

        PlaySelfEffect();
        GameLog.Debug($"忠勇护甲 技能id={id} 等级={Level} 友军={target?.heroId}");
        return true;
    }
}
