using CommonConfig;
using UnityEngine;

/// <summary>
/// 司马懿·隐忍（术）：对目标造成高额 /damagestrength 法术伤害，将其推离自身（强制位移 xz 平面），
/// 并使其叛逃（Buff "叛"，时长 bufftime）：不受控制地向初始位置后退、移动速度减半、打断引导，
/// 把核心敌人推出己阵并令其自乱阵脚。
/// </summary>
public class SkillAidEnduranceStrike : Skill
{
    /// <summary>推远的水平距离</summary>
    private const float PushDistance = 6f;

    public SkillAidEnduranceStrike(int id, Chess unit) : base(id, unit)
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

        if (GetSkillDamage() > 0)
            target.OnSkillDamaged(owner, skillId, GetSkillDamage());

        // 推远：沿"自身→目标"方向把目标推离自身（落点被挡则原地不动，保持地面 y）
        var dir = target.transform.position - owner.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector3.forward;
        var pushPos = target.transform.position + dir.normalized * PushDistance;
        pushPos.y = target.transform.position.y;
        if (!WorldManager.Instance.CheckPositionBlocked(target, pushPos))
            target.transform.position = pushPos;

        // 施加叛逃（Buff短名由 SkillConfig.BuffId 配置，如"叛"）
        foreach (var buffId in GetSkillBuffIds())
            BuffManager.AddBuff(target, owner, id, buffId, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}