using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用"嘲讽 + 可选自身增益/范围伤害"技能（ScriptName = "SkillComDamageTaunt"）：
/// 对自身 Area 范围内敌人施加嘲讽（BuffTime 秒），并可按配置追加自身 Buff 与范围伤害。
///
/// 流程复用基类 SkillComBuffSingle：CanStart（存活即可）→ 选自身 → CanCast（requireTarget / CheckBurst）
/// → PlayCast（动作 + 自身特效 + 范围特效）→ ApplyBuffs（自身 Buff → 范围伤害 → 嘲讽）。
/// 组件标记 requireTarget（必须有当前目标，于禁·铁壁）由基类 CanCast 统一处理；
/// 组件标记 allyShield（李严·护军）表示把自身 Buff 同时挂给 Area 内其他友军，实现"嘲讽 + 周围友军加盾"。
/// 伤害：DamageStrength&gt;0（或物理/真实伤害的 atk）时对 Area 内敌人结算 GetSkillDamage()；
/// DamageStrength=0 且无 BuffId 时为纯嘲讽（庞德·抬棺仅靠伤害，黄盖仅靠光环盾）。
/// </summary>
public class SkillComDamageTaunt : SkillComBuffSingle
{
    public SkillComDamageTaunt(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>允许不配 BuffId：纯嘲讽/纯范围伤害（庞德·抬棺）无需 Buff。</summary>
    protected override bool RequireBuffId
    {
        get { return false; }
    }

    /// <summary>嘲讽只要求自身存活（不需要已交战）。</summary>
    protected override bool CanStart()
    {
        return owner.hp > 0;
    }

    /// <summary>Buff 只挂自身；嘲讽本身由 TauntEnemies 单独挂给敌方单位。</summary>
    protected override List<Chess> SelectBuffTargets(List<int> buffIds)
    {
        return new List<Chess> { owner };
    }

    protected override void PlayCast(List<Chess> targets)
    {
        base.PlayCast(targets);
        PlayAreaEffect(owner.transform.position);
    }

    protected override void ApplyBuffs(List<Chess> targets, List<int> buffIds)
    {
        // 自身 Buff（如减伤盾"硬"）；模板组件「嘲讽大」会把嘲讽 Buff 并入 BuffId，
        // 必须跳过，否则会把自己设为索敌目标
        foreach (var buffId in buffIds)
        {
            if (buffId == CombatConst.TauntBuffId)
                continue;
            ApplyBuffToUnit(owner, buffId);
        }

        // 「周围友军加盾」组件：把本技能的护盾等 Buff 同时挂给 Area 内其他友军（施法者已在上面拿到）
        if (HasSkillFlag("allyShield"))
        {
            var allies = SelectTargetsWithRules(owner.transform.position, skillCfg.Area, true, 0, "ally", "", owner);
            foreach (var ally in allies)
            {
                if (ally == null || ally.hp <= 0)
                    continue;
                foreach (var buffId in buffIds)
                {
                    if (buffId == CombatConst.TauntBuffId)
                        continue;
                    ApplyBuffToUnit(ally, buffId);
                }
            }
        }

        // 范围伤害（DamageStrength=0 时 GetSkillDamage 为 0，自动跳过）；选人规则见 SelectOnly / SelectFirst
        var damage = GetSkillDamage();
        if (damage > 0)
        {
            var enemies = SelectTargets(owner.transform.position, skillCfg.Area, false, skillCfg.TargetCount);
            foreach (var e in enemies)
            {
                if (e == null || e.hp <= 0)
                    continue;
                e.OnSkillDamaged(owner, id, damage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    PlaySkillEffect(e, skillCfg.HitEffect);
            }
        }

        // 嘲讽：把范围内敌人的当前目标改到自己身上
        TauntEnemies(skillCfg.Area);
    }
}
