using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"自身增益"技能（ScriptName = "SkillComBuffSelf"）：触发时给施法者自身挂 1~N 个通用 Buff，
/// Buff 短名由 SkillConfig.BuffId 配置（英文/中文逗号或分号分隔多个，如"快"、"倍"、"重,吸"、"重,伤"），
/// 各 Buff 的效果数值由其自身读取对应的 Strength2/StrengthBuff1，本类不写死数值；时长取 BuffTime。
/// 覆盖：丁奉·奋威("倍") / 王濬·疾击、凌统·迅射("快") /
/// 华雄·狂暴("重,伤") / 徐晃·长驱("重,吸") / 徐盛·疑城、张梁·黄天("硬") / 张辽·威震("震") /
/// 曹仁·天人守城("守") / 周泰·不屈("盾,甲") / 马岱·连射（permanent，直接永久叠攻速）。
///
/// TargetType 标记：
/// · noStack   = 自身已带任一 BuffId 时不重复施放（曹仁·天人守城）
/// · permanent = 不挂 Buff，把 Strength2 里的成长键（selfAtkSpeed/selfAtk/selfAp/selfArmor/selfMagicRes/selfHp/selfMoveSpeed/selfRange）
///               永久叠到自身对应属性，每次施放成长一次（马岱·连射）
/// </summary>
public class SkillComBuffSelf : Skill
{
    public SkillComBuffSelf(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 自己未进入交战（跑位/待机中）不施放，避免移动途中空开浪费 Buff 时长
        if (!owner.IsInFight())
            return false;

        // permanent：不挂 Buff；成长键由 SkillManager 在施放成功后统一叠加（见 Skill.ApplyPermanentGrowth）
        if (HasSkillFlag("permanent"))
        {
            if (!CheckBurst(null))
                return false;

            owner.PlayerAnim(skillCfg.Action);
            PlaySelfEffect();
            return true;
        }

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillComBuffSelf: 未配置Buff短名 技能id={id}");
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
            ApplyBuffToUnit(owner, buffId);
        return true;
    }
}
