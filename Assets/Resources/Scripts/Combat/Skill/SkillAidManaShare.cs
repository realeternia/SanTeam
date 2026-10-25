using CommonConfig;
using UnityEngine;

/// <summary>
/// 刘备·仁德（王）：发动时把本技能累积的全部 MP 均分给我方全体英雄（含施法者自身）。
/// 分享总量 = 本次发动前累积 MP × (100 + ap) / 100 × Strength2[shareRate]（法强加成让总量更多），
/// 每位英雄分得 总量 / 英雄数，按各自未满的蓝耗技能逐一充能（与名门共用 AddRegenMp 入口）。
/// MP 的清零由 Skill.CheckBurst 在发动瞬间完成，脚本通过 mpBeforeBurst 读取清零前的累积量。
/// </summary>
public class SkillAidManaShare : Skill
{
    public SkillAidManaShare(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        var allies = WorldManager.Instance.GetUnitsMySide(owner.side)
            .FindAll(x => x != null && x.isHero && x.IsInFight());
        if (allies.Count == 0)
        {
            GameLog.Warn("仁德：" + GetHeroName(owner) + " 我方没有可分享法力的英雄，跳过");
            return false;
        }

        var rate = GetStr2("shareRate", 1f);
        var total = mpBeforeBurst * (100f + owner.GetAttr("ap")) / 100f * rate;
        var share = total / allies.Count;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 分享法力的同时给自己挂 SkillConfig.BuffId 指定的增益（仁德配置为"攻"，数值取 StrengthBuff1 的 atk）
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);

        foreach (var ally in allies)
        {
            var given = false;
            foreach (var s in ally.skills)
            {
                if (s.skillCfg == null || s.skillCfg.MpCost <= 0 || s.IsMpFull())
                    continue;
                s.AddRegenMp(share);
                given = true;
            }
            if (given && ally != owner && !string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(ally, skillCfg.HitEffect); // 施法者自身已由 SelfEffect 表现，避免叠播
        }

        GameLog.Info("仁德：" + GetHeroName(owner) + " 共享 " + (int)mpBeforeBurst + " 点法力（法强加成后 "
            + (int)total + "），我方 " + allies.Count + " 名英雄各得 " + (int)share + " 点");
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
