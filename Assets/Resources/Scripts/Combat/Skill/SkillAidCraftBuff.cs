using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 黄月英·巧工：给我方两名不同英雄直接永久提升属性（不走 buff、战斗内不还原）：
/// 生命比例最低的我方英雄 atk += Strength（攻击）；另一名（生命次低、与前一名不同）armor += Strength2（护甲）。
/// 若我方仅剩 1 名英雄，则只施加攻击增益。
/// </summary>
public class SkillAidCraftBuff : Skill
{
    public SkillAidCraftBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        // 我方英雄列表（排除自身，需在战斗中）
        var allies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        if (allies.Count == 0)
            return false;

        // 按生命比例升序（最低在前）
        allies.Sort((a, b) => (a.hp / (float)a.maxHp).CompareTo(b.hp / (float)b.maxHp));

        owner.PlayerAnim(skillCfg.Action);

        // 目标1：生命最低的我方英雄，永久提升攻击
        var atkTarget = allies[0];
        atkTarget.atk += (int)skillCfg.Strength;
        EffectManager.PlaySkillEffect(atkTarget, skillCfg.HitEffect);
        GameLog.Info("巧工：" + GetHeroName(owner) + " 为 " + GetHeroName(atkTarget) + " 永久提升攻击 " + (int)skillCfg.Strength);

        // 目标2：另一名（若存在）永久提升护甲
        if (allies.Count >= 2)
        {
            var armorTarget = allies[1];
            armorTarget.armor += (int)skillCfg.Strength2;
            EffectManager.PlaySkillEffect(armorTarget, skillCfg.HitEffect);
            GameLog.Info("巧工：" + GetHeroName(owner) + " 为 " + GetHeroName(armorTarget) + " 永久提升护甲 " + (int)skillCfg.Strength2);
        }
        else
        {
            GameLog.Warn("巧工：我方英雄不足两名，仅施加攻击增益，护甲目标缺失");
        }

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
