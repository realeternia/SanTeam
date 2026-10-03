using CommonConfig;
using UnityEngine;

/// <summary>
/// 典韦·恶来：对自身 /area 范围内敌人各造成 /damagestrength 法术伤害，并进入狂暴状态（Buff "狂"，双刃±由 BuffFrenzy 读 StrengthBuff1[0]，时长 bufftime）
/// </summary>
public class SkillAidBerserkCleave : Skill
{
    public SkillAidBerserkCleave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlayAreaEffect(owner.transform.position);

        var frenzyBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (frenzyBuffCfg != null)
            BuffManager.AddBuff(owner, owner, id, frenzyBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidBerserkCleave: 未找到狂暴Buff短名：" + skillCfg.BuffId);

        var skillDamage = GetSkillDamage();
        var units = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
        }

        return true;
    }
}