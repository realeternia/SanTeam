using CommonConfig;

/// <summary>
/// 战斗开始属性加成技能：BattleBegin 时解析本技能配置行。
/// · LinkSelf（格式"attr+value,..."）：本羁绊每个成员各自获得——每名持有该羁绊技能的英雄只把它加给自己；
/// · LinkTeam（格式同上）：本侧"非本羁绊成员的其他英雄"（不含士兵）获得，配置即总量，不乘人数。
/// 与职业连锁语义一致（LinkSelf=连接英雄自身 / LinkTeam=该组之外的其他我方英雄），同时配置不会重复叠加。
/// 解析与施加复用 JobLinkManager（与职业羁绊同源，避免双份逻辑），数值由配置表决定，无需逐属性写脚本。
/// 对应技能：智勇(magicres)、严明(armor) 仅用 LinkSelf；文采 同时用 LinkSelf + LinkTeam。
/// 好友羁绊组内多名成员都持有同一技能：LinkSelf 各加各的，LinkTeam 只由英雄Id最小的持有者结算一次，避免重复叠加。
/// </summary>
public class SkillBondInitAttrChange : Skill
{
    public SkillBondInitAttrChange(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        // LinkSelf：本羁绊成员各自获得（每名持有者只作用于自身）
        foreach (var bonus in JobLinkManager.ParseBonuses(skillCfg.LinkSelf))
            JobLinkManager.ApplyAttr(owner, bonus.Attr, bonus.Value);

        // LinkTeam：本侧非本羁绊成员获得，由英雄Id最小的持有者统一结算一次
        if (!string.IsNullOrEmpty(skillCfg.LinkTeam) && !HasJuniorTeamHolder())
        {
            var teamBonuses = JobLinkManager.ParseBonuses(skillCfg.LinkTeam);
            foreach (var unit in WorldManager.Instance.GetUnitsMySide(owner.side))
            {
                // 只作用于英雄（不含士兵），与职业连锁 LinkTeam 的口径保持一致
                if (unit == null || !unit.isHero || unit.hp <= 0 || IsBondMember(unit))
                    continue;
                foreach (var bonus in teamBonuses)
                    JobLinkManager.ApplyAttr(unit, bonus.Attr, bonus.Value);
            }
        }

        if (!string.IsNullOrEmpty(skillCfg.Action))
            owner.PlayerAnim(skillCfg.Action);
    }

    /// <summary>该单位是否为本羁绊成员（持有同一 Sname 的羁绊技能）</summary>
    private bool IsBondMember(Chess unit)
    {
        if (unit.skills == null)
            return false;
        foreach (var s in unit.skills)
            if (s != null && s.skillCfg != null && s.skillCfg.Sname == skillCfg.Sname)
                return true;
        return false;
    }

    /// <summary>同侧存在英雄Id更小、且持有同一羁绊技能(Sname相同、LinkTeam非空)的成员时跳过 LinkTeam 结算</summary>
    private bool HasJuniorTeamHolder()
    {
        foreach (var unit in WorldManager.Instance.GetUnitsMySide(owner.side))
        {
            if (unit == null || unit == owner || unit.hp <= 0 || !unit.isHero || unit.skills == null)
                continue;
            if (unit.heroId >= owner.heroId)
                continue;
            foreach (var s in unit.skills)
                if (s != null && s.skillCfg != null && s.skillCfg.Sname == skillCfg.Sname
                    && !string.IsNullOrEmpty(s.skillCfg.LinkTeam))
                    return true;
        }
        return false;
    }
}
