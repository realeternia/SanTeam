using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 杜预·冰山（ScriptName = "SkillAidAreaWallBuff"）：主动群伤技能——
/// 以当前目标为基准点，沿「施法者→目标」方向的垂直方向横排召唤 SummonCount 座冰山（间距 IcebergSpacing），
/// 每座冰山对周围 Area 内敌人各结算一次技能伤害，同一次施法内每个单位只结算一次；
/// 命中目标额外附加 SkillConfig.BuffId（缓）指定的减速 Buff，持续 BuffTime 秒。
/// 非持续施法：放完冰山后施法者可正常移动/普攻。
/// </summary>
public class SkillAidAreaWallBuff : SkillComDamageSingle
{
    /// <summary>相邻冰山的横向间距（世界单位）</summary>
    private const float IcebergSpacing = 10f;

    public SkillAidAreaWallBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 单体技能同样走 SelectOnly / SelectFirst（未配 SelectFirst 时沿用当前索敌目标）；Range<=0 视为不限射程
        var target = SelectSingleTarget(skillCfg.Range);
        if (target == null)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var targetPos = target.transform.position;

        // 横排方向：垂直于「施法者→目标」的水平方向；两者重合时退化为施法者朝向的垂直方向
        var forward = targetPos - owner.transform.position;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            forward = owner.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        var right = Quaternion.Euler(0, 90, 0) * forward;

        // 以目标为中心左右交替展开：0、+1、-1、+2、-2 ...（5 座即 -20/-10/0/+10/+20）
        var positions = new List<Vector3>();
        int count = Mathf.Max(1, skillCfg.SummonCount);
        for (int i = 0; i < count; i++)
        {
            int step = (i + 1) / 2;
            float sign = (i % 2 == 1) ? 1f : -1f;
            positions.Add(targetPos + right * (step * IcebergSpacing * sign));
        }

        // 冰山视觉：每座播一次范围特效，生命周期取 SummonTime
        foreach (var pos in positions)
            PlayAreaEffect(pos, skillCfg.SummonTime);

        // 伤害与减速：同一次施法内每个单位只结算一次
        var baseDamage = GetSkillDamage();
        var buffIds = GetSkillBuffIds();
        var hitUnits = new List<Chess>();
        foreach (var pos in positions)
        {
            var units = SelectTargets(pos, skillCfg.Area, false, skillCfg.TargetCount);
            foreach (var unit in units)
            {
                if (hitUnits.Contains(unit))
                    continue;
                hitUnits.Add(unit);

                ResolveDamage(unit, baseDamage);
                foreach (var buffId in buffIds)
                    ApplyBuffToUnit(unit, buffId);
            }
        }
        return true;
    }
}
