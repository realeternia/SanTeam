using System.Collections.Generic;

/// <summary>
/// 通用"群体增益"技能（ScriptName = "SkillComBuffArea"）：给范围内友军附加 skillCfg.BuffId 对应的正面 Buff。
/// 支持多段组合（如奇才 "攻,甲,抗"），护盾类 Buff 自动按统一护盾公式写入护盾量。
/// 选人规则由 SkillConfig.SelectOnly / SelectFirst 决定（典型 SelectOnly="ally;hero;notSelf;notFullHp"、
/// SelectFirst="lowestHp"），TargetCount&lt;=0 时范围内全部。
///
/// 本类只声明「群体发放 + 播范围特效」，全部组件语义（selfAlways / noDedupe / wholeMap / targetCenter /
/// splitByRange / tauntAroundTarget / 护盾写入）由基类 SkillComBuffSingle 统一结算。
/// </summary>
public class SkillComBuffArea : SkillComBuffSingle
{
    public SkillComBuffArea(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>群体发放：在 buffCenter 的 Area 内按规则选人。</summary>
    protected override bool GroupBuff
    {
        get { return true; }
    }

    /// <summary>群体增益不要求已交战（走位中也可以给友军挂增益）。</summary>
    protected override bool CanStart()
    {
        return owner.hp > 0;
    }

    protected override void PlayCast(List<Chess> targets)
    {
        base.PlayCast(targets);
        PlayAreaEffect(buffCenter);
    }
}
