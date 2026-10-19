using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"自身增益"技能（ScriptName = "SkillSelfBuff"）：触发时给施法者自身挂 1~N 个通用 Buff，
/// Buff 短名由 SkillConfig.BuffId 配置（英文/中文逗号或分号分隔多个，如"快"、"倍"、"重,吸"、"重,伤"），
/// 各 Buff 的效果数值由其自身读取对应的 Strength2/StrengthBuff1，本类不写死数值；时长取 BuffTime。
/// 覆盖：丁奉·奋威("倍") / 王濬·疾击、凌统·迅射("快") /
/// 华雄·狂暴("重,伤") / 徐晃·长驱("重,吸") / 徐盛·疑城、张梁·黄天("硬") / 张辽·威震("震") /
/// 曹仁·天人守城("守") / 周泰·不屈("盾,甲") / 马岱·连射（permanent，直接永久叠攻速）。
///
/// TargetType 标记：
/// · noStack   = 自身已带任一 BuffId 时不重复施放（曹仁·天人守城）
/// · permanent = 不挂 Buff，直接把 键 selfAtkSpeed 永久加到自身攻速（马岱·连射）
/// </summary>
public class SkillSelfBuff : Skill
{
    public SkillSelfBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // permanent：不挂 Buff，直接把 键 selfAtkSpeed 永久叠到自身攻速
        if (HasSkillFlag("permanent"))
        {
            if (!CheckBurst(null))
                return false;

            owner.PlayerAnim(skillCfg.Action);
            PlaySelfEffect();
            owner.attackSpeedRate += GetStr2("selfAtkSpeed");
            return true;
        }

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Error($"SkillSelfBuff: 未配置Buff短名 技能id={id}");
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
                shield.SetHp(GetSkillShield());
        }
        return true;
    }
}
