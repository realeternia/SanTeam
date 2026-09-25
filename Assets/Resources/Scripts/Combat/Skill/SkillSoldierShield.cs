using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 蒋琬·抚军：战斗开始(BattleBegin)给本侧近战士兵全员套护盾；每次主动释放额外给1名近战士兵套盾。
/// 护盾 = 最大生命 × Strength（25/35/50/70/95%），复用 BuffShield。
/// </summary>
public class SkillSoldierShield : Skill
{
    public SkillSoldierShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        foreach (var s in GetMeleeSoldiers())
            ShieldSoldier(s);
        GameLog.Debug($"抚军 技能id={id} 等级={Level} 开局给本侧近战士兵套盾 {GetMeleeSoldiers().Count} 名");
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        var melee = GetMeleeSoldiers();
        if (melee.Count > 0)
            ShieldSoldier(melee[SysRandom.Range(0, melee.Count)]);
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }

    private void ShieldSoldier(Chess s)
    {
        var buffId = BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id;
        BuffManager.AddBuff(s, owner, id, buffId, skillCfg.BuffTime);
        var shield = s.GetBuff(buffId) as BuffShield;
        if (shield != null)
            shield.SetHp((int)(s.maxHp * skillCfg.Strength));
    }

    // 本侧存活近战士兵（近战判定沿用 CombatConst.MeleeRange）
    private List<Chess> GetMeleeSoldiers()
    {
        return WorldManager.Instance.GetUnitsMySide(owner.side)
            .Where(x => !x.isHero && x.attackRange < CombatConst.MeleeRange).ToList();
    }
}