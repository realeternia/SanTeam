using CommonConfig;
using UnityEngine;

/// <summary>
/// 周泰·不屈：为自身附加固定护盾（护盾量 = Strength2[0] × (100+法强)/100，随法强成长）
/// 并提升护甲（BuffArmorAdd 读 StrengthBuff1[0] 点护甲），肉盾续命。
/// 两个 buff 由 SkillConfig.BuffId 以逗号分隔配置（"盾,甲"），持续 BuffTime 秒。
/// </summary>
public class SkillAidUnbreakable : Skill
{
    public SkillAidUnbreakable(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 给自己挂多个 buff（BuffId 支持逗号分隔，如 "盾,甲"）
        foreach (var buffId in GetSkillBuffIds())
        {
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);

            // 护盾类 buff 需按统一护盾公式写入护盾值（护甲等其它 buff 无需额外赋值）
            var shield = owner.GetBuff(buffId) as BuffShield;
            if (shield != null)
                shield.SetHp(GetSkillShield(0));
        }

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }
}
