using CommonConfig;
using UnityEngine;

/// <summary>
/// 张辽·威震：为自身附加威震状态（Buff "威" BuffHitStun，普攻按 StrengthInt% 概率眩晕）bufftime 秒，
/// 并同时为自身附加反伤（Buff "反" BuffReflect，受到伤害时把 Strength 比例反还给攻击者）。
/// 反伤短名"反"为技能专属常量。
/// </summary>
public class SkillAidVengefulStance : Skill
{
    /// <summary>反伤 Buff 短名（BuffReflect）</summary>
    public const string ReflectBuffNameS = "反";

    public SkillAidVengefulStance(int id, Chess unit) : base(id, unit)
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

        // 威震"威"（眩晕）
        var weiBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (weiBuffCfg != null)
            BuffManager.AddBuff(owner, owner, id, weiBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidVengefulStance: 未找到威震Buff短名：" + skillCfg.BuffId);

        // 反伤"反"
        var reflectBuffCfg = BuffConfig.GetConfigByNameS(ReflectBuffNameS);
        if (reflectBuffCfg != null)
            BuffManager.AddBuff(owner, owner, id, reflectBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidVengefulStance: 未找到反伤Buff短名：" + ReflectBuffNameS);

        return true;
    }
}