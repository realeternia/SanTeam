using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 袁绍·名门：祝福我方生命比例最高的一员大将（最肉主力），附加由多个通用 Buff 组成的组合（SkillConfig.BuffId = "攻,甲,抗"）——
/// 攻（BuffAtkAdd）读 StrengthBuff1[0]、甲（BuffArmorAdd）读 [1]、抗（BuffMagicResAdd）读 [2]。
/// </summary>
public class SkillAidNobleBless : Skill
{
    public SkillAidNobleBless(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillAidNobleBless: 未配置Buff短名 技能id={id}");
            return false;
        }

        // 祝福优先同侧(battleSide)友军，无同侧目标再退到跨侧友军(alley)；去重只看组合里的第一段
        var pool = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x != owner && x.isHero && x.IsInFight() && !x.HasBuff(buffIds[0]));
        if (pool.Count == 0)
        {
            pool = WorldManager.Instance.GetUnitsInRangeAll(owner.transform.position, skillCfg.Area)
                .FindAll(x => x != owner && !WorldManager.Instance.IsEnemy(x.side, owner.side)
                    && x.isHero && x.IsInFight() && !x.HasBuff(buffIds[0]));
        }
        if (pool.Count == 0)
            return false;

        // 生命比例最高的友军英雄
        Chess highest = null;
        foreach (var a in pool)
        {
            if (highest == null || a.HpRate > highest.HpRate)
                highest = a;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        foreach (var buffId in buffIds)
            BuffManager.AddBuff(highest, owner, id, buffId, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(highest, skillCfg.HitEffect);
        GameLog.Info("名门：" + GetHeroName(owner) + " 祝福 " + GetHeroName(highest) + " 提升攻击与双防");
        return true;
    }

    /// <summary>取英雄显示名（士兵等非英雄单位返回原始描述）</summary>
    private string GetHeroName(Chess chess)
    {
        if (chess == null || !chess.isHero)
            return "单位";
        var cfg = CommonConfig.HeroConfig.GetConfig(chess.heroId);
        return cfg != null ? cfg.Name : "英雄";
    }
}
