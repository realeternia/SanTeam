using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 陈宫·绝策（棋手）：持续施法，对身前长 Range、宽 Area 的矩形区域持续喷火。
/// 引导期 BuffTime 秒内施法者无法移动/普攻（castingSkillId>0），死亡/晕眩立即打断。
/// 每 SummonHitInterval 秒跳伤一次，整轮总伤害 = DamageStrength（按跳数均分），
/// 每跳最多对 TargetCount 名区域内敌人造成一次伤害；跳伤之间互不去重，同一目标可被多跳累计命中。
/// 命中后挂 SkillConfig.BuffId 指定的减益；矩形中心播一次 AreaEffect 作为喷火视觉。
/// </summary>
public class SkillCastingAidFrontalFlaming : Skill
{
    /// <summary>默认跳伤间隔(秒)，未配置 SummonHitInterval 时使用</summary>
    private const float DefaultHitInterval = 0.5f;

    public SkillCastingAidFrontalFlaming(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 喷火方向：自身指向目标；目标与自身重合时退化为自身朝向
        var origin = owner.transform.position;
        var forwardDir = target.transform.position - origin;
        forwardDir.y = 0f;
        if (forwardDir.sqrMagnitude < 0.0001f)
            forwardDir = owner.transform.forward;
        forwardDir.y = 0f;
        forwardDir.Normalize();

        // 矩形中心：沿朝向走 Range/2，播一次范围特效作为喷火表现（朝向与施法方向对齐）
        var areaCenter = origin + forwardDir * (skillCfg.Range * 0.5f);
        areaCenter.y = origin.y;
        PlayAreaEffect(areaCenter, skillCfg.BuffTime, Quaternion.LookRotation(forwardDir));

        // 进入持续施法：引导期内 MoveAndFight 提前返回，无法移动/普攻
        owner.StartCasting(id, ChannelFlaming(origin, forwardDir));
        return true;
    }

    private IEnumerator ChannelFlaming(Vector3 origin, Vector3 forwardDir)
    {
        var interval = skillCfg.SummonHitInterval > 0f ? skillCfg.SummonHitInterval : DefaultHitInterval;
        var duration = Mathf.Max(0.1f, skillCfg.BuffTime);
        var ticks = Mathf.Max(1, Mathf.FloorToInt(duration / interval));
        // 每跳伤害 = 配置的 DamageStrength（每跳独立结算，可对同一目标累计多跳）
        var perTickDamage = Mathf.Max(1, GetSkillDamage());
        var halfWidth = skillCfg.Area * 0.5f;
        var rightDir = Vector3.Cross(Vector3.up, forwardDir).normalized;
        var buffIds = GetSkillBuffIds();

        for (var i = 0; i < ticks; i++)
        {
            // 被动打断（晕眩/死亡）：BreakCasting 已把 castingSkillId 置0，此处结束引导
            if (owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;

            owner.PlayerAnim(skillCfg.Action);

            var hitUnits = GetEnemyInRect(origin, forwardDir, rightDir, skillCfg.Range, halfWidth);
            // 每跳命中数上限：TargetCount>0 时随机保留 N 名，<=0 表示不限
            if (skillCfg.TargetCount > 0)
                WorldManager.Instance.RandomSelect(hitUnits, skillCfg.TargetCount);

            foreach (var unit in hitUnits)
            {
                unit.OnSkillDamaged(owner, skillId, perTickDamage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
                // 减益：每跳刷新一次，引导结束后仍按 BuffTime 计时
                foreach (var buffId in buffIds)
                    BuffManager.AddBuff(unit, owner, id, buffId, skillCfg.BuffTime);
            }

            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>取以 origin 为顶点、沿 forwardDir 长 length、左右各宽 halfWidth 的矩形内所有存活敌人</summary>
    private List<Chess> GetEnemyInRect(Vector3 origin, Vector3 forwardDir, Vector3 rightDir, float length, float halfWidth)
    {
        var result = new List<Chess>();
        foreach (var e in WorldManager.Instance.GetAllEnemys(owner.side))
        {
            if (e == null || e.hp <= 0)
                continue;

            var delta = e.transform.position - origin;
            delta.y = 0f;

            var depth = Vector3.Dot(delta, forwardDir);
            if (depth < 0f || depth > length)
                continue;

            if (Mathf.Abs(Vector3.Dot(delta, rightDir)) > halfWidth)
                continue;

            result.Add(e);
        }
        return result;
    }
}
