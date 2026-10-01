using CommonConfig;
using UnityEngine;

/// <summary>
/// 陈群·律令：对自身 /area 范围内所有敌人造成 /damagestrength 法术伤害，并为其挂减疗（Buff "疫"，减疗值取 Strength2 末位非零值）bufftime 秒。
/// </summary>
public class SkillAidDecreeAoe : Skill
{
    public SkillAidDecreeAoe(int id, Chess unit) : base(id, unit)
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

        var healDownBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        var skillDamage = GetSkillDamage();

        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
            if (healDownBuffCfg != null)
                BuffManager.AddBuff(u, owner, id, healDownBuffCfg.Id, skillCfg.BuffTime);
            else
                GameLog.Error("SkillAidDecreeAoe: 未找到减疗Buff短名：" + skillCfg.BuffId);
        }

        return true;
    }
}
