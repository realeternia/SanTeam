using CommonConfig;

/// <summary>
/// 通用"单体治疗"技能（ScriptName = "SkillComHealSingle"）：
/// 在 Area 内按 SelectOnly / SelectFirst（典型 SelectOnly="ally;hero;notFullHp"、SelectFirst="lowestHp"）
/// 选出生命比例最低的一名友方英雄，恢复 GetSkillHeal() 点生命（吃医职业治疗加成），
/// 并驱散其负面 Buff（不可驱散的招牌 Buff 除外，见 BuffManager.DispelNegative）；
/// BuffId 非空时额外附加对应 Buff（持续 BuffTime 秒）。附近友军全部满血时不施放。
/// 范围治疗见 SkillComHealArea，跳跃治疗见 SkillComHealJumpChain。
/// </summary>
public class SkillComHealSingle : Skill
{
    public SkillComHealSingle(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 选目标：Area 内按 SelectFirst 优先级取一名友方英雄（典型为生命比例最低）；Area=0 表示全图
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, true, 1);
        var target = picked.Count > 0 ? picked[0] : null;
        if (target == null)
            return false;

        if (!CheckBurst(null))
            return false;

        // 可选挂 buff：配置了 BuffId 时给被治疗目标附加
        int buffId = 0;
        if (!string.IsNullOrEmpty(skillCfg.BuffId))
        {
            var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
            if (buffCfg == null)
                GameLog.Error($"SkillComHealSingle 技能id={id} BuffId[{skillCfg.BuffId}] 未找到对应 BuffConfig，本次不挂 buff");
            else
                buffId = buffCfg.Id;
        }

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var heal = GetSkillHeal();
        if (heal > 0)
            owner.HealTarget(target, skillId, heal, true);
        // 附带净化：驱散被治疗友军的负面 Buff
        BuffManager.DispelNegative(target);
        if (buffId > 0)
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        GameLog.Debug($"{skillCfg.Name} 技能id={id} 等级={Level} 治疗={heal} 目标={target.heroId} BuffId={skillCfg.BuffId}");
        return true;
    }
}
