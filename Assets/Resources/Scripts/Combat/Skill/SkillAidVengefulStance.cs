using CommonConfig;
using UnityEngine;

/// <summary>
/// 张辽·威震：为自身附加威震状态（Buff "震" BuffVengefulStance）bufftime 秒。
/// 该状态下普攻按 Strength2[0] 概率眩晕目标，且普攻伤害类型转为真实伤害（无视护甲与护盾）。
/// </summary>
public class SkillAidVengefulStance : Skill
{
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
        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);

        // 威震"震"（普攻眩晕 + 普攻转真实伤害）
        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg != null)
            BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidVengefulStance: 未找到威震Buff短名：" + skillCfg.BuffId);

        return true;
    }
}