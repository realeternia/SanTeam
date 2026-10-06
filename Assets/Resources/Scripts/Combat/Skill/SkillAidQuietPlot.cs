using CommonConfig;
using UnityEngine;

/// <summary>
/// 范围伤害+减益（术）：以目标位置为中心，对 /area 范围内敌人各造成 /damagestrength 法术伤害，
/// 并附加 skillCfg.BuffId 指定的减益 buff（钟会·蓄谋="疫"降疗；法正·明断="慑"降攻），时长 bufftime。
/// 注意：法正/钟会均为远程(棋)单位，AoE 必须以目标位置为圆心，否则自身站位远离敌阵会导致技能全程打空。
/// </summary>
public class SkillAidQuietPlot : Skill
{
    public SkillAidQuietPlot(int id, Chess unit) : base(id, unit)
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

        owner.PlayerAnim(skillCfg.Action);

        var center = target.transform.position;
        var units = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
        var skillDamage = GetSkillDamage();

        // 减益 buff 由 SkillConfig.BuffId 指定，不在战斗代码硬编码 BuffId
        var debuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (debuffCfg == null)
        {
            GameLog.Error($"SkillAidQuietPlot: 未找到减益Buff短名：{skillCfg.BuffId} 技能id={id}");
            return false;
        }

        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, skillId, skillDamage);
            BuffManager.AddBuff(u, owner, id, debuffCfg.Id, skillCfg.BuffTime);
        }

        PlayAreaEffect(center);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
