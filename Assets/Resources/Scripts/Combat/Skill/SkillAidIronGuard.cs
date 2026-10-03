using CommonConfig;
using UnityEngine;

/// <summary>
/// 于禁·铁壁：为自身附加减伤盾（Buff "硬"，减伤比例由 BuffShieldValue 读 StrengthBuff1[0]）bufftime 秒；
/// 嘲讽 /area 范围内敌人（把它们的当前目标改到自己身上），并对其造成轻量 /damagestrength 法术伤害（走统一公式 GetSkillDamage）。
/// </summary>
public class SkillAidIronGuard : Skill
{
    public SkillAidIronGuard(int id, Chess unit) : base(id, unit)
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

        // 自身减伤盾（"硬"）
        var shieldBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (shieldBuffCfg != null)
            BuffManager.AddBuff(owner, owner, id, shieldBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidIronGuard: 未找到减伤盾Buff短名：" + skillCfg.BuffId);

        // 嘲讽：把范围内敌人的当前目标改到自己身上
        TauntEnemies(skillCfg.Area);

        var microDamage = GetSkillDamage();
        var units = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
        foreach (var u in units)
        {
            if (microDamage > 0)
                u.OnSkillDamaged(owner, id, microDamage);
        }

        return true;
    }
}