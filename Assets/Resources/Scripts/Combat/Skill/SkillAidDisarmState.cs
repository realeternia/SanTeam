using CommonConfig;
using UnityEngine;

/// <summary>
/// 刘晔·卸甲：为自身附加卸甲状态（Buff "卸" BuffDisarm）bufftime 秒；
/// 状态期物理攻击无视目标 /strength2-1% 护甲（GetArmorDelta 挂钩 GetEffectiveArmor 折算），
/// 且目标带吸收型护盾时对其伤害提升 /strengthbuff1-1%（BuffDisarm.BeforeCalDamage 放大）。
/// </summary>
public class SkillAidDisarmState : Skill
{
    public SkillAidDisarmState(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>攻击侧护甲修正：-Strength2 → 等效护甲×(1-Strength2)，实现无视 x% 护甲</summary>
    public override float GetArmorDelta(bool isAttackerSide)
    {
        if (!isAttackerSide)
            return 0f;
        return -skillCfg.Strength2[0];
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

        var disarmBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (disarmBuffCfg != null)
            BuffManager.AddBuff(owner, owner, id, disarmBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidDisarmState: 未找到卸甲Buff短名：" + skillCfg.BuffId);

        return true;
    }
}
