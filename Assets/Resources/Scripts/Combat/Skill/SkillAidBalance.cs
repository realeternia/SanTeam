using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙权·制衡：为范围内最多 TargetCount 名友军英雄永久随机提升一项属性（直接改字段，不走 Buff 不还原）：
/// 攻击/AP 提升取 key "atk"，护甲/魔抗提升取 key "armor"。
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

        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Area, owner.side)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        if (allies.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        WorldManager.Instance.RandomSelect(allies, skillCfg.TargetCount);

        foreach (var u in allies)
        {
            // 四选一随机：攻击/ap 走 key "atk"，护甲/魔抗走 key "armor"
            var r = SysRandom.Value;
            if (r < 0.25f)
            {
                u.atk += (int)GetStr2("atk");
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升攻击 " + (int)GetStr2("atk"));
            }
            else if (r < 0.5f)
            {
                u.ap += (int)GetStr2("atk");
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升AP " + (int)GetStr2("atk"));
            }
            else if (r < 0.75f)
            {
                u.armor += (int)GetStr2("armor");
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升护甲 " + (int)GetStr2("armor"));
            }
            else
            {
                u.magicRes += (int)GetStr2("armor");
                GameLog.Info("制衡：" + GetHeroName(owner) + " 为 " + GetHeroName(u) + " 永久提升魔抗 " + (int)GetStr2("armor"));
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
