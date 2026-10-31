using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 吕布·魔神：起手先原地往天上飞 FlyUpDuration 秒，再快速俯冲到目标身前 LandDistance 处落地，
/// 落地后开始持续施法(引导) SummonTime 秒。引导期间每个 1 秒结算点都对目标重新施加眩晕 Buff
/// （短名由 skillCfg.BuffId 配置；目标已有该 Buff 时只刷新时长，故控制能覆盖整个引导过程，不会因 BuffTime 短而中途脱控），
/// 同时吸取目标生命（造成 GetSkillDamage 伤害）并把等量生命回复自身。
/// 起手位移用 noMoveCount 锁住寻路，引导期间自身处于引导状态(castingSkillId>0)，无法移动/普攻，被晕眩/死亡打断；目标死亡则提前结束。
/// </summary>
public class SkillCastingAidDrain : Skill
{
    /// <summary>起飞阶段耗时（秒）</summary>
    private const float FlyUpDuration = 1f;
    /// <summary>俯冲阶段耗时（秒）</summary>
    private const float DashDuration = 0.5f;
    /// <summary>起飞高度（世界单位）</summary>
    private const float FlyUpHeight = 25f;
    /// <summary>落点与目标的水平距离（落在目标身前）</summary>
    private const float LandDistance = 15f;

    public SkillCastingAidDrain(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.isJumping)
            return false;
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;
        if (!CheckBurst(target))
            return false;

        // 起手位移期间由协程接管位置，锁住寻路避免被互斥力/寻路拉走
        owner.StartCoroutine(Approach(target));
        return true;
    }

    // 起手：往天上飞 → 俯冲到目标身前落地 → 开始引导
    IEnumerator Approach(Chess target)
    {
        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        var groundY = owner.baseY + owner.heightOffset;
        var startPos = owner.transform.position;
        startPos.y = groundY;
        var landPos = startPos;

        // 空中位移状态：外部位移技能无法打断，动作必播完
        BeginAirMove();
        // 用 finally 保证空中状态一定复位：isJumping 一旦泄漏，MoveTo 与寻路会永久失效，单位会卡在半空且不再行动
        try
        {
            // 第一阶段：原地往天上飞
            var rise = AirMove(FlyUpDuration, t =>
            {
                var pos = startPos;
                pos.y = groundY + FlyUpHeight * t;
                return pos;
            });
            while (rise.MoveNext())
                yield return rise.Current;
            if (owner == null || owner.hp <= 0)
                yield break;

            // 第二阶段：从空中快速俯冲到目标身前；目标位置一直在变，落点每步按目标当前位置修正
            var flyPos = new Vector3(startPos.x, groundY + FlyUpHeight, startPos.z);
            landPos = CalcLandPos(target, groundY);
            var dive = AirMove(DashDuration, t =>
            {
                if (target != null && target.hp > 0)
                    landPos = CalcLandPos(target, groundY);
                return Vector3.Lerp(flyPos, landPos, t);
            });
            while (dive.MoveNext())
                yield return dive.Current;
            if (owner == null || owner.hp <= 0)
                yield break;

            owner.transform.position = landPos;
        }
        finally
        {
            EndAirMove();
        }

        owner.LockTarget(target);
        // 落地爆裂特效（范围特效，半径取 skillCfg.Area）
        PlayAreaEffect(landPos);

        owner.StartCasting(id, DrainChannel(target));
    }

    // 落点：目标当前位置沿「目标→自身」方向外推 LandDistance（即目标身前），目标移动时持续修正
    private Vector3 CalcLandPos(Chess target, float groundY)
    {
        var dir = owner.transform.position - target.transform.position;
        dir.y = 0f;
        if (dir.magnitude < 0.001f)
            dir = owner.transform.forward;
        var landPos = target.transform.position + dir.normalized * LandDistance;
        landPos.y = groundY;
        return landPos;
    }

    IEnumerator DrainChannel(Chess target)
    {
        var stunCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (stunCfg == null)
            GameLog.Error("SkillCastingAidDrain: 未找到眩晕Buff短名：" + skillCfg.BuffId);

        var endTime = Time.time + Mathf.Max(0.1f, skillCfg.SummonTime);
        while (Time.time < endTime)
        {
            // 被打断/死亡：castingSkillId 被 BreakCasting 置0，结束引导
            if (owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;
            // 吸取目标死亡则提前结束
            if (target == null || target.hp <= 0)
                yield break;

            owner.PlayerAnim(skillCfg.Action);
            PlaySelfEffect();

            // 每个 1 秒结算点给目标上一次眩晕 Buff（已存在则刷新时长），保证持续控制
            if (stunCfg != null)
                BuffManager.AddBuff(target, owner, id, stunCfg.Id, skillCfg.BuffTime);

            var damage = GetSkillDamage();
            if (damage > 0)
            {
                target.OnSkillDamaged(owner, skillId, damage);
                // 吸取：把造成的伤害转为自身生命（吸血不算治疗，不吃治疗加成）
                owner.HealTarget(owner, skillId, damage, false);
                EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
