using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 袁绍·名门：祝福我方生命比例最高的一员大将（最肉主力），附加"名门"Buff：
/// 攻击提升 StrengthBuff1[0] 点、护甲与魔抗提升 StrengthBuff1[1] 点（"名门"BuffMultiAttr）。
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

        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error("SkillAidNobleBless: 未找到名门Buff短名：" + skillCfg.BuffId);
            return false;
        }

        var allies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x != owner && x.isHero && x.IsInFight() && !x.HasBuff(buffCfg.Id));

        if (allies.Count == 0)
            return false;

        // 生命比例最高的友军英雄
        Chess highest = null;
        foreach (var a in allies)
        {
            if (highest == null || a.HpRate > highest.HpRate)
                highest = a;
        }

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(highest, owner, id, buffCfg.Id, skillCfg.BuffTime);
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
