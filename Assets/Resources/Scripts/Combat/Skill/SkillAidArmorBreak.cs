using CommonConfig;
using UnityEngine;

/// <summary>
/// 孙坚·破阵：对自身 /area 范围内敌人各造成 /strength 法术伤害，并削减其护甲（Buff "破"，Strength2，时长 bufftime）
/// </summary>
public class SkillAidArmorBreak : Skill
{
    public SkillAidArmorBreak(int id, Chess unit) : base(id, unit)
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

        var armorDownBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        var skillDamage = GetSkillDamage();

        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
            if (armorDownBuffCfg != null)
                BuffManager.AddBuff(u, owner, id, armorDownBuffCfg.Id, skillCfg.BuffTime);
            else
                GameLog.Error("SkillAidArmorBreak: 未找到破甲Buff短名：" + skillCfg.BuffId);
        }

        return true;
    }
}