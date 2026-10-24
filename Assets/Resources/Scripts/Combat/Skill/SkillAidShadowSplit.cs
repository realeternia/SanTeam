using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 关羽·分兵：对目标造成一次魔法伤害，并以目标单位为对称中心，在本体的镜像位置召唤一个影分身
/// （攻击/生命按 Strength2 比例 inheritRate 继承本体）。镜像点踩墙时回退到目标身后 Area 距离处。
/// </summary>
public class SkillAidShadowSplit : Skill
{
    public SkillAidShadowSplit(int id, Chess unit) : base(id, unit)
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

        // 本体魔法伤害（每次释放只结算一次）
        target.OnSkillDamaged(owner, id, GetSkillDamage());

        // 召唤影分身：落在"以目标为对称中心、本体位置的镜像点"，攻击/生命继承本体 × key "inheritRate"
        var shadowPosition = MirrorPosition(target.transform.position);
        var shadow = SummonUnit(shadowPosition, CombatConst.SoldierShadow, HeroConfig.GetConfig(owner.heroId).Icon);
        shadow.atk = (int)(owner.atk * GetStr2("inheritRate"));
        shadow.maxHp = (int)(owner.maxHp * GetStr2("inheritRate"));
        shadow.hp = shadow.maxHp;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        EffectManager.PlaySkillEffect(target, skillCfg.SelfEffect);
        return true;
    }

    /// <summary>本体位置以目标点为对称中心的镜像点；镜像点踩墙时回退到目标前方 Area 距离处（沿本体→目标方向）</summary>
    private Vector3 MirrorPosition(Vector3 targetPos)
    {
        var mirror = targetPos * 2f - owner.transform.position;
        mirror.y = owner.transform.position.y;
        if (!WorldManager.Instance.CheckPositionBlocked(owner, mirror))
            return mirror;

        var dir = targetPos - owner.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = owner.transform.forward;
        dir.y = 0f;
        var fallback = targetPos + dir.normalized * skillCfg.Area;
        fallback.y = owner.transform.position.y;
        return fallback;
    }
}