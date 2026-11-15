/// <summary>
/// 吕蒙·白衣渡江（特有技能，不走模板）：对目标造成一次法术伤害，
/// 首段 Buff 挂给目标、其余 Buff 挂给自身（BuffId="缓,愈,抗" → 减速给目标，回血与魔抗给自己），持续 BuffTime。
/// 伤害结算与通用命中后处理（破盾/燃蓝/击退/苦肉/驱散）沿用 SkillComDamageSingle。
/// </summary>
public class SkillAidWhiteRobe : SkillComDamageSingle
{
    public SkillAidWhiteRobe(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>Buff 归属：首段挂目标、其余挂自身（原模板组件 bothBuff 的专用实现）。</summary>
    protected override void ApplySkillBuffs(Chess target)
    {
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
            return;

        ApplyBuffToUnit(target, buffIds[0]);
        for (var i = 1; i < buffIds.Count; i++)
            ApplyBuffToUnit(owner, buffIds[i]);
    }
}
