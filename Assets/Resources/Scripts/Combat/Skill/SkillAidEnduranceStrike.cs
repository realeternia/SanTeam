using CommonConfig;
using UnityEngine;

/// <summary>
/// 司马懿·隐忍（术）：对目标造成高额 /damagestrength 法术伤害，将其拉拽到自己身侧（强制位移 xz 平面），
/// 并眩晕（Buff "乱"，时长 bufftime）2 秒，把核心敌人拖入己阵围杀。
/// </summary>
public class SkillAidEnduranceStrike : Skill
{
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

        // 拉拽到自身近旁（保持地面 y，xz 平面贴靠）
        var pullPos = new Vector3(owner.transform.position.x + 1f, owner.transform.position.y, owner.transform.position.z + 1f);
        target.transform.position = pullPos;

        // 眩晕 2 秒
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("乱").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}