using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹操·护驾：将生命比例最低的友军英雄传送到身边，为其套 Strength2[0]×(100+法强)/100 的护盾（随法强成长），
/// 并附加"护驾"Buff（受疗 healedRate 提升 /strengthbuff1-1%）。
/// </summary>
public class SkillAidImperialGuard : Skill
{
    /// <summary>护盾 Buff 短名</summary>
    public const string ShieldBuffNameS = "盾";

    public SkillAidImperialGuard(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var shieldCfg = BuffConfig.GetConfigByNameS(ShieldBuffNameS);
        var guardCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (shieldCfg == null)
        {
            GameLog.Error("SkillAidImperialGuard: 未找到护盾Buff短名：" + ShieldBuffNameS);
            return false;
        }
        if (guardCfg == null)
        {
            GameLog.Error("SkillAidImperialGuard: 未找到护驾Buff短名：" + skillCfg.BuffId);
            return false;
        }

        var allies = WorldManager.Instance.GetMySideInRange(owner.transform.position, skillCfg.Range, owner.side)
            .FindAll(x => x != owner && x.isHero && x.IsInFight());

        if (allies.Count == 0)
            return false;

        // 生命比例最低的友军英雄
        Chess lowest = null;
        foreach (var a in allies)
        {
            if (lowest == null || a.HpRate < lowest.HpRate)
                lowest = a;
        }

        owner.PlayerAnim(skillCfg.Action);

        // 传送至身边（参照 SkillAttackedTeleport 瞬移路径）
        lowest.MoveTo(owner.transform.position + new Vector3(0, 0, 6f), true);

        // 套盾：强度 = Strength2[0] × (100+法强)/100（随法强成长）
        BuffManager.AddBuff(lowest, owner, id, shieldCfg.Id, skillCfg.BuffTime);
        var sh = lowest.GetBuff(shieldCfg.Id) as BuffShield;
        if (sh != null)
            sh.SetHp(GetSkillShield(0));
        else
            GameLog.Warn("SkillAidImperialGuard: 护盾Buff类型异常，无法设置护盾值");

        // 回复加成
        BuffManager.AddBuff(lowest, owner, id, guardCfg.Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(lowest, skillCfg.HitEffect);
        GameLog.Info("护驾：" + GetHeroName(owner) + " 将 " + GetHeroName(lowest) + " 召至身边并套盾提升回复");
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
