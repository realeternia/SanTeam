using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"自身增益"技能（ScriptName = "SkillBondSelfBuff"）：触发时给施法者自身挂 1~N 个通用 Buff，
/// Buff 短名由 SkillConfig.BuffId 配置（英文/中文逗号或分号分隔多个，如"快"、"倍"、"重,吸"、"重,伤"），
/// 各 Buff 的效果数值由其自身读取对应的 Strength2/StrengthBuff1，本类不写死数值；时长取 BuffTime。
/// 当前使用者：好友连锁·自守（"反"，反弹伤害）。
///
/// TargetType 标记：
/// · noStack   = 自身已带任一 BuffId 时不重复施放（自守）
/// </summary>
public class SkillBondSelfBuff : Skill
{
    public SkillBondSelfBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 自己未进入交战（跑位/待机中）不施放，避免移动途中空开浪费 Buff 时长
        if (!owner.IsInFight())
            return false;

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillBondSelfBuff: 未配置Buff短名 技能id={id}");
            return false;
        }

        // 已有该状态则不重复施放（Buff 的叠层/到期还原由其自身处理）
        if (HasSkillFlag("noStack"))
        {
            foreach (var buffId in buffIds)
                if (owner.GetBuff(buffId) != null)
                    return false;
        }

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        foreach (var buffId in buffIds)
        {
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);

            // 护盾类 buff 需按统一护盾公式写入护盾值（护甲等其它 buff 无需额外赋值）
            var shield = owner.GetBuff(buffId) as BuffShield;
            if (shield != null)
                shield.AddHp(GetSkillShield());
        }
        return true;
    }
}
