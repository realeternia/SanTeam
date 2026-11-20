using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 光环技能（ScriptName = "SkillComBuffAura"）：
/// 开启期间每 1.4 秒对 Area 范围内的单位施加 1.5 秒的 BuffId（可指向友方或敌方，
/// 由 SelectOnly / SelectFirst 选人，TargetCount&lt;=0 表示范围内全部）。
///
/// MP 规则：
/// · MpCost=0：开局常开，不消耗 MP；
/// · MpCost&gt;0：MP 满且 CD 就绪时通过正常施法开启（施法不清空 MP，MP 作为光环燃料），
///   开启期间每秒消耗 1 × SkillConfig.AuraMpRate 点 MP（默认 2x）；攻击/受击回蓝照常累计（可延长持续时间）；
///   当 MP&lt;=0 时自动关闭；关闭后需 MP 再次充满并施法才会重新开启。
///
/// 开启时挂常驻 SelfEffect，关闭时销毁（光环视觉随之消失）。
/// </summary>
public class SkillComBuffAura : SkillComBuffSingle
{
    /// <summary>光环 Buff 结算间隔(秒)：每 1.4 秒对范围内单位挂一次 Buff</summary>
    private const float TickInterval = 1.4f;
    /// <summary>光环 Buff 持续时间(秒)：1.5 秒，略大于结算间隔避免断档</summary>
    private const float BuffDuration = 1.5f;
    /// <summary>光环 MP 消耗默认速度倍率：SkillConfig.AuraMpRate 未配置/≤0 时使用</summary>
    private const float DefaultMpRate = 2f;

    private float tickTimer;
    private bool auraOn;
    private GameObject selfEffectObj;

    public SkillComBuffAura(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        tickTimer = 0f;
        // 无 MP 消耗的光环开局常开；有 MP 消耗的光环需施法开启
        if (skillCfg.MpCost <= 0)
            SetAura(true);
    }

    /// <summary>有 MP 消耗的光环通过正常施法开启（MP 满 + CD 就绪），开启后 MP 作为持续燃料。</summary>
    public override bool CheckAidSkill()
    {
        if (auraOn || skillCfg.MpCost <= 0)
            return false;
        if (!CheckBurst(null))
            return false;

        // CheckBurst 会清空 MP，这里恢复为满值作为光环燃料；之后攻击/受击回蓝继续累计
        mp = skillCfg.MpCost;
        owner.PlayerAnim(skillCfg.Action);
        SetAura(true);
        return true;
    }

    public override void Tick(float deltaTime)
    {
        if (owner == null || owner.hp <= 0 || deltaTime <= 0f)
            return;
        if (!auraOn)
            return;

        // 有 MP 消耗：每秒扣 1 × AuraMpRate（默认 2）点，扣到 0 自动关闭（期间攻击/受击回蓝照常累计）
        if (skillCfg.MpCost > 0)
        {
            var drainRate = skillCfg.AuraMpRate > 0f ? skillCfg.AuraMpRate : DefaultMpRate;
            mp -= deltaTime * drainRate;
            if (mp <= 0f)
            {
                mp = 0f;
                SetAura(false);
                return;
            }
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
        var duration = BuffDuration;
        foreach (var unit in units)
        {
            foreach (var buffId in buffIds)
                BuffManager.AddBuff(unit, owner, id, buffId, duration);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}
