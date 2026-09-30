using CommonConfig;
using UnityEngine;

/// <summary>
/// 于禁·铁壁：为自身附加减伤盾（Buff "硬"，减伤比例取 Strength2）bufftime 秒；对 /area 范围内敌人附加嘲讽（Buff "嘲"）
/// 并对范围内敌人造成轻量 /strength 法术伤害（走统一公式 GetSkillDamage）。嘲讽短名"嘲"为技能专属常量。
/// </summary>
public class SkillAidIronGuard : Skill
{
    /// <summary>嘲讽 Buff 短名</summary>
    public const string TauntBuffNameS = "嘲";

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

        // 自身减伤盾（"硬"）
        var shieldBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (shieldBuffCfg != null)
            BuffManager.AddBuff(owner, owner, id, shieldBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidIronGuard: 未找到减伤盾Buff短名：" + skillCfg.BuffId);

        var tauntBuffCfg = BuffConfig.GetConfigByNameS(TauntBuffNameS);
        var microDamage = GetSkillDamage();

        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        foreach (var u in units)
        {
            if (microDamage > 0)
                u.OnSkillDamaged(owner, id, microDamage);
            if (tauntBuffCfg != null)
                BuffManager.AddBuff(u, owner, id, tauntBuffCfg.Id, skillCfg.BuffTime);
            else
                GameLog.Error("SkillAidIronGuard: 未找到嘲讽Buff短名：" + TauntBuffNameS);
        }

        return true;
    }
}