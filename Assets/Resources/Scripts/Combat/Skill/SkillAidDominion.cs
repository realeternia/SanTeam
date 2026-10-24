using CommonConfig;
using UnityEngine;

/// <summary>
/// 曹操·护驾（君临）：AOE 伤害 + 我方全体法强增益。
/// 以施法者为圆心、Area 为半径，对范围内所有敌人造成一次法术伤害（GetSkillDamage）；
/// 然后给我方全体英雄（含施法者自己）挂 BuffId 指定的增益（"术"=法强提升，数值取 StrengthBuff1 的 "ap"），持续 BuffTime。
/// 冷却长、耗蓝高，定位为决战型大招。
/// </summary>
public class SkillAidDominion : Skill
{
    public SkillAidDominion(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // 以目标为圆心的大范围 AOE（王多在后排，避免自身中心打空）
        var center = target.transform.position;
        center.y = owner.transform.position.y;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        PlayAreaEffect(center);

        // 以自身为圆心的 AOE 伤害
        var damage = GetSkillDamage();
        var enemies = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            if (damage > 0)
                enemy.OnSkillDamaged(owner, id, damage);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(enemy, skillCfg.HitEffect);
        }

        // 我方全体英雄获得法强等增益（含自身）
        var buffIds = GetSkillBuffIds();
        var allies = WorldManager.Instance.GetUnitsMySide(owner.side)
            .FindAll(x => x.isHero && x.IsInFight());
        foreach (var ally in allies)
        {
            foreach (var buffId in buffIds)
                BuffManager.AddBuff(ally, owner, id, buffId, skillCfg.BuffTime);
            if (!string.IsNullOrEmpty(skillCfg.SelfEffect))
                EffectManager.PlaySkillEffect(ally, skillCfg.SelfEffect);
        }

        GameLog.Info("护驾：" + owner.heroId + " 对 " + enemies.Count + " 名敌人造成 AOE，并为 " + allies.Count + " 名友军提升法强");
        return true;
    }
}
