using CommonConfig;
using UnityEngine;

/// <summary>
/// 袁绍·名门（统帅）：使我方全体其他单位（不含施法者自身）一次性回复技能法力。
/// 回复量取 Strength2 的键 "mpAdd"，按每个单位各自有蓝耗(MpCost>0)的技能逐一充能，
/// 由 Skill.AddRegenMp 按 [0, MpCost] 裁剪；已满蓝的技能跳过。
/// </summary>
public class SkillAidManaRally : Skill
{
    public SkillAidManaRally(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var allies = WorldManager.Instance.GetUnitsMySide(owner.side)
            .FindAll(x => x != owner && x.IsInFight());
        if (allies.Count == 0)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var add = GetStr2("mpAdd");
        foreach (var ally in allies)
        {
            var given = false;
            foreach (var s in ally.skills)
            {
                if (s.skillCfg == null || s.skillCfg.MpCost <= 0 || s.IsMpFull())
                    continue;
                s.AddRegenMp(add);
                given = true;
            }
            if (given && !string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(ally, skillCfg.HitEffect);
        }

        GameLog.Info("名门：" + GetHeroName(owner) + " 为我方其他单位各回复 " + (int)add + " 点技能法力");
        return true;
    }

    /// <summary>取英雄显示名（士兵等非英雄单位返回原始描述）</summary>
    private string GetHeroName(Chess chess)
    {
        if (chess == null || !chess.isHero)
            return "单位";
        var cfg = HeroConfig.GetConfig(chess.heroId);
        return cfg != null ? cfg.Name : "英雄";
    }
}
