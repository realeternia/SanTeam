using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙权·制衡：为范围内最多 TargetCount 名友军英雄永久随机提升一项属性（直接改字段，不走 Buff 不还原）：
/// 攻击/AP 提升 Strength 点，护甲/魔抗提升 Strength2 点。
/// </summary>
public class SkillAidBalance : Skill
{
    public SkillAidBalance(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var allies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        if (allies.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);

        WorldManager.Instance.RandomSelect(allies, skillCfg.TargetCount);

        foreach (var u in allies)
        {
            // 四选一随机：攻击/ap 走 Strength，护甲/魔抗走 Strength2
            var r = SysRandom.Value;
            if (r < 0.25f)
            {
                u.atk += (int)skillCfg.Strength;
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升攻击 " + (int)skillCfg.Strength);
            }
            else if (r < 0.5f)
            {
                u.ap += (int)skillCfg.Strength;
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升AP " + (int)skillCfg.Strength);
            }
            else if (r < 0.75f)
            {
                u.armor += (int)skillCfg.Strength2;
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升护甲 " + (int)skillCfg.Strength2);
            }
            else
            {
                u.magicRes += (int)skillCfg.Strength2;
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升魔抗 " + (int)skillCfg.Strength2);
            }
            EffectManager.PlaySkillEffect(u, skillCfg.HitEffect);
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
