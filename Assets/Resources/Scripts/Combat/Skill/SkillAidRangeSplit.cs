using System.Collections.Generic;

/// <summary>
/// 司马炎·一统 / 陆抗·西陵（特有技能）：群体增益，近战单位取 BuffId 第 1 段、远程单位取第 2 段（如 "甲,攻"）。
/// 选人 / 去重（noDedupe）/ selfAlways 等仍走基类 SkillComBuffSingle，本类只负责近远分段发放。
/// </summary>
public class SkillAidRangeSplit : SkillComBuffArea
{
    public SkillAidRangeSplit(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>发放：近战吃第 1 段、远程吃第 2 段；不足两段时退化为普通群体增益。</summary>
    protected override void ApplyBuffs(List<Chess> targets, List<int> buffIds)
    {
        if (buffIds.Count < 2)
        {
            base.ApplyBuffs(targets, buffIds);
            return;
        }

        foreach (var unit in targets)
        {
            ApplyBuffToUnit(unit, unit.attackRange < CombatConst.MeleeRange ? buffIds[0] : buffIds[1]);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}
