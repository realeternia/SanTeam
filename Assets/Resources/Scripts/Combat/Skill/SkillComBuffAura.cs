using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 光环技能（ScriptName = "SkillComBuffAura"）：
/// 开启期间每 3 秒对 Area 范围内的单位施加 BuffId（可指向友方或敌方，
/// 由 SelectOnly / SelectFirst 选人，TargetCount&lt;=0 表示范围内全部）。
///
/// MP 规则：
/// · MpCost=0：开局常开，不消耗 MP；
/// · MpCost&gt;0：MP 满时自动开启，开启期间每秒扣 1 点 MP（每点 MP 持续 1 秒），扣到 0 自动关闭；
///   开启期间攻击/受击回蓝失效（Chess.mpGainLocked），技能回蓝（Skill.AddMp）不受影响；
///   关闭后 MP 再次充满会自动重新开启。
///
/// 开启时挂常驻 SelfEffect，关闭时销毁（光环视觉随之消失）。
/// </summary>
public class SkillComBuffAura : Skill
{
    /// <summary>光环结算间隔(秒)：每 3 秒对范围内单位挂一次 Buff</summary>
    private const float TickInterval = 3f;
    /// <summary>BuffTime 未配置时的兜底持续时间(秒)，需大于结算间隔以免断档</summary>
    private const float BuffDurationFallback = 4f;

    private float tickTimer;
    private bool auraOn;
    private GameObject selfEffectObj;

    public SkillComBuffAura(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        tickTimer = 0f;
        // 无 MP 消耗的光环开局常开
        if (skillCfg.MpCost <= 0)
            SetAura(true);
    }

    public override void Tick(float deltaTime)
    {
        if (owner == null || owner.hp <= 0 || deltaTime <= 0f)
            return;

        if (!auraOn)
        {
            // 有 MP 消耗的光环：MP 满才开启
            if (skillCfg.MpCost > 0 && mp >= skillCfg.MpCost)
                SetAura(true);
            else
                return;
        }

        // 每点 MP 持续 1 秒：开启期间持续扣蓝，扣到 0 关闭
        mp -= deltaTime;
        if (mp <= 0f)
        {
            mp = 0f;
            SetAura(false);
            return;
        }

        tickTimer += deltaTime;
        if (tickTimer < TickInterval)
            return;
        tickTimer -= TickInterval;
        ApplyAuraBuffs();
    }

    public override void OnDeath()
    {
        SetAura(false);
    }

    private void SetAura(bool on)
    {
        if (auraOn == on)
            return;
        auraOn = on;
        // 开启期间禁止攻击/受击回蓝；技能回蓝不受影响（无 MP 消耗的常驻光环不占用 MP）
        owner.mpGainLocked = on && skillCfg.MpCost > 0;

        if (on)
        {
            if (!string.IsNullOrEmpty(skillCfg.SelfEffect))
                selfEffectObj = EffectManager.PlayPersistentEffect(owner, skillCfg.SelfEffect);
            tickTimer = 0f;
            // 开启瞬间立即结算一次，避免等满 3 秒
            ApplyAuraBuffs();
            return;
        }

        if (selfEffectObj != null)
        {
            Object.Destroy(selfEffectObj);
            selfEffectObj = null;
        }
    }

    private void ApplyAuraBuffs()
    {
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count == 0)
        {
            GameLog.Warn($"SkillComBuffAura: 未配置Buff短名 技能id={id}");
            return;
        }

        // 选人：SelectOnly 硬过滤 + SelectFirst 优先级（未配置阵营默认友方，可配 enemy 给敌方挂 Buff）
        var units = SelectTargets(owner.transform.position, skillCfg.Area, true, skillCfg.TargetCount);
        var duration = skillCfg.BuffTime > 0f ? skillCfg.BuffTime : BuffDurationFallback;
        foreach (var unit in units)
        {
            foreach (var buffId in buffIds)
                BuffManager.AddBuff(unit, owner, id, buffId, duration);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}
