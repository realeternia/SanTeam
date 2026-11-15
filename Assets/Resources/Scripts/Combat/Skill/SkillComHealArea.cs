/// <summary>
/// 青囊 / 天书（ScriptName = "SkillComHealArea"）：范围治疗，先在 Area 内按 SelectOnly / SelectFirst
/// （典型 SelectOnly="ally;hero;notFullHp"、SelectFirst="lowestHp"）选出治疗中心，
/// 再以该中心为圆心、Area 为半径按同样规则最多治疗 TargetCount 名友方（各结算一次，吃医职业治疗加成）。
/// 治疗量走独立治疗公式 GetSkillHeal()；附近友军全部满血时不施放。
///
/// 单目标结算（治疗量、溢出转护甲、驱散、Buff 施加、命中特效）全部走基类 HealUnit，
/// 本类只负责「选治疗中心 → 群体选人 → 逐个结算」，不含任何组件判断。
/// </summary>
public class SkillComHealArea : SkillComHealSingle
{
    public SkillComHealArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 第一步：在射程内选治疗中心（SelectFirst 优先级最高的友方英雄，典型为生命比例最低）
        var first = SelectTargets(owner.transform.position, skillCfg.Area, true, 1);
        var center = first.Count > 0 ? first[0] : null;
        if (center == null)
            return false;

        if (!CanCast(center))
            return false;

        // 第二步：以治疗中心为圆心，按同样规则选出被治疗的友方
        var healTargets = SelectTargets(center.transform.position, skillCfg.Area, true, skillCfg.TargetCount);

        PlayCast(center);
        PlayAreaEffect(center.transform.position);

        var heal = GetSkillHeal();
        foreach (var unit in healTargets)
            HealUnit(unit, heal, true);

        GameLog.Debug($"{skillCfg.Name} 技能id={id} 等级={Level} 治疗={heal} 中心={center.heroId} 目标数={healTargets.Count} BuffId={skillCfg.BuffId}");
        return true;
    }
}
