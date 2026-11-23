using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 增益体系基类（ScriptName = "SkillComBuffSingle"，模板「自身强化」）：
/// 触发时给目标挂 1~N 个通用 Buff。目标由 SkillConfig.SelectOnly / SelectFirst 决定（Area 内最多 TargetCount 名）；
/// SelectOnly 留空时默认给自己。Buff 短名由 SkillConfig.BuffId 配置（逗号/分号分隔多个），
/// 各 Buff 的效果数值由其自身读取 Strength2/StrengthBuff1，本类不写死数值；时长取 BuffTime。
///
/// CheckAidSkill 固定按「可施放 → 选目标 → 发动判定 → 施法表现 → 施加效果」五步走，派生类只覆写钩子：
/// · CanStart() / SelectBuffTargets() / CanCast() / PlayCast() / ApplyBuffs()
///
/// **所有增益组件（TargetType 标记 / Strength2 键）统一在基类结算**，派生类不要重复判断：
/// · noStack            = 目标已带任一 BuffId 时不重复施放（曹仁·天人守城）
/// · permanent          = 不挂 Buff，把 Strength2 的成长键永久叠到自身属性，每次施放成长一次（马岱·连射）
/// · selfAlways         = 施法者必定获得（李典·御阵 / 周仓）
/// · noDedupe           = 不按首段 Buff 去重（司马炎 / 陆抗 / 张梁 / 周仓 / 娄圭 / 步骘 / 阮籍 / 甄宓）
/// · targetCenter       = 以选中的首个友军目标为中心，再在 Area 内扩散（缺省以自身为中心）
/// 护盾类 Buff 统一按护盾公式写入护盾值。
///
/// 增益体系派生：SkillComBuffArea（群体增益）、SkillComBuffAura（光环）、SkillComDamageTaunt（嘲讽）。
/// </summary>
public class SkillComBuffSingle : Skill
{
    /// <summary>本次施放的中心点（自身 / targetCenter 选中的目标位置），供派生类播范围特效。</summary>
    protected Vector3 buffCenter;

    public SkillComBuffSingle(int id, Chess unit) : base(id, unit)
    {
    }

    /// <summary>是否群体发放：true = 在 buffCenter 的 Area 内按规则选人；false = 默认自身（单体增益）</summary>
    protected virtual bool GroupBuff
    {
        get { return false; }
    }

    /// <summary>是否必须配置 BuffId：纯伤害/纯机制类派生（如嘲讽）可覆写为 false。</summary>
    protected virtual bool RequireBuffId
    {
        get { return true; }
    }

    public override bool CheckAidSkill()
    {
        if (!CanStart())
            return false;

        // permanent：不挂 Buff；成长键由 SkillManager 在施放成功后统一叠加（见 Skill.ApplyPermanentGrowth）
        if (HasSkillFlag("permanent"))
        {
            if (!CheckBurst(null))
                return false;

            PlayCast(new List<Chess> { owner });
            return true;
        }

        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0 && RequireBuffId)
        {
            GameLog.Error($"{GetType().Name}: 未配置Buff短名 技能id={id}");
            return false;
        }

        var targets = SelectBuffTargets(buffIds);
        if (targets.Count == 0)
            return false;
        if (!CanCast(targets, buffIds))
            return false;

        PlayCast(targets);
        ApplyBuffs(targets, buffIds);
        return true;
    }

    /// <summary>是否允许施放：默认要求自身已进入交战（跑位/待机中不施放，避免移动途中空开浪费 Buff 时长）。</summary>
    protected virtual bool CanStart()
    {
        return owner.IsInFight();
    }

    /// <summary>选目标：单体走自身/筛选；群体走 Area（含 selfAlways / noDedupe / targetCenter 组件）。</summary>
    protected virtual List<Chess> SelectBuffTargets(List<int> buffIds)
    {
        buffCenter = owner.transform.position;

        if (!GroupBuff)
        {
            var single = new List<Chess>();
            if (!string.IsNullOrEmpty(skillCfg.SelectOnly))
                single = SelectTargets(owner.transform.position, skillCfg.Area, true,
                    Mathf.Max(1, skillCfg.TargetCount));
            else
                single.Add(owner);
            return single;
        }

        var selfAlways = HasSkillFlag("selfAlways");
        var noDedupe = HasSkillFlag("noDedupe");
        // 去重：非 noDedupe 时排除已带首段 Buff 的单位
        Func<Chess, bool> dedupeFilter = (noDedupe || buffIds.Count == 0) ? null : x => !x.HasBuff(buffIds[0]);
        // selfAlways：选人时排除施法者，最后再把他补进结果
        var exclude = selfAlways ? owner : null;

        // targetCenter：先选首个友军目标作为中心，再以其位置扩散；缺省以自身为中心
        if (HasSkillFlag("targetCenter"))
        {
            var primary = SelectTargets(owner.transform.position, skillCfg.Range, true, 1,
                exclude, dedupeFilter);
            if (primary.Count == 0)
                return new List<Chess>();
            buffCenter = primary[0].transform.position;
        }

        var units = SelectTargets(buffCenter, skillCfg.Area, true, skillCfg.TargetCount,
            exclude, dedupeFilter);
        if (selfAlways && !units.Contains(owner))
            units.Add(owner);
        return units;
    }

    /// <summary>发动判定：默认处理 requireTarget（必须有当前目标）与 noStack（已带 Buff 不重复），再 CheckBurst。</summary>
    protected virtual bool CanCast(List<Chess> targets, List<int> buffIds)
    {
        // requireTarget：必须有当前索敌目标才施放（于禁·铁壁）
        var needTarget = HasSkillFlag("requireTarget");
        if (needTarget && (owner.targetChess == null || owner.targetChess.hp <= 0))
            return false;

        if (HasSkillFlag("noStack"))
        {
            foreach (var target in targets)
                foreach (var buffId in buffIds)
                    if (target.GetBuff(buffId) != null)
                        return false;
        }

        return CheckBurst(needTarget ? owner.targetChess : null);
    }

    /// <summary>施法表现：默认施法动作 + 自身特效。</summary>
    protected virtual void PlayCast(List<Chess> targets)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
    }

    /// <summary>单体友方增益：SelectOnly 非空表示选友军（否则自身强化给自己）。</summary>
    protected bool SingleAllyBuff
    {
        get { return !GroupBuff && !string.IsNullOrEmpty(skillCfg.SelectOnly); }
    }

    /// <summary>施加效果：逐目标挂 Buff，并结算命中特效。</summary>
    protected virtual void ApplyBuffs(List<Chess> targets, List<int> buffIds)
    {
        foreach (var unit in targets)
        {
            foreach (var buffId in buffIds)
            {
                // 单体友方增益：目标已带同一 Buff 时只刷新时长，避免共存类 Buff 叠层
                // （群体增益仍走 AddBuff + 首段去重，保持原有群刷语义）
                var exist = SingleAllyBuff ? unit.GetBuff(buffId) : null;
                if (exist != null)
                {
                    exist.SetTime(skillCfg.BuffTime);
                    continue;
                }
                ApplyBuffToUnit(unit, buffId);
            }

            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}
