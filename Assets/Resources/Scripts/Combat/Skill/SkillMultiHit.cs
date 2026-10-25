using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 多段单体伤害技能（ScriptName = "SkillMultiHit"，如荀攸·奇策）：
/// 锁定射程内的当前目标，按 TargetCount 段数对同一目标连续造成 GetSkillDamage() 技能伤害，
/// 每段独立播放命中特效 skillCfg.HitEffect，段间间隔 SegmentInterval(0.2s)；
/// 目标中途死亡则提前结束（"最多 N 段"），全部段数结算完后统一附加 SkillConfig.BuffId 指定的 Buff。
/// 与 SkillSingleDamage 的区别：伤害随时间分段推进（可被打断/目标死亡截断），不是同一帧内连算。
/// </summary>
public class SkillMultiHit : Skill
{
    /// <summary>段间伤害间隔(秒)</summary>
    private const float SegmentInterval = 0.2f;

    /// <summary>本技能是否还有段数在结算中（防止 CD 内重入）</summary>
    private bool running;

    public SkillMultiHit(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (running)
            return false;

        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (skillCfg.Range > 0f && !WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        running = true;
        owner.StartCoroutine(PlaySegments(target));
        return true;
    }

    /// <summary>逐段结算伤害：每段播一次命中特效，段间隔 SegmentInterval</summary>
    private IEnumerator PlaySegments(Chess target)
    {
        var hits = skillCfg.TargetCount > 0 ? skillCfg.TargetCount : 1;
        for (var i = 0; i < hits; i++)
        {
            if (owner == null || owner.hp <= 0)
                break;
            if (target == null || target.hp <= 0)
                break;

            var dmg = GetSkillDamage();
            if (dmg > 0)
            {
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
                target.OnSkillDamaged(owner, id, dmg);
            }

            if (i < hits - 1)
                yield return new WaitForSeconds(SegmentInterval);
        }

        if (owner != null && owner.hp > 0 && target != null && target.hp > 0)
            ApplyBuffs(target);

        running = false;
    }

    /// <summary>BuffId 指定的 Buff 全部挂到目标（时长取 BuffTime）</summary>
    private void ApplyBuffs(Chess target)
    {
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);
    }
}
