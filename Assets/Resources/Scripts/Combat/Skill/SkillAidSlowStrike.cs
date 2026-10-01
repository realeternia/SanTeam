using CommonConfig;
using UnityEngine;

/// <summary>
/// 马谡·守势：对目标造成 /damagestrength 法术伤害，并使其减速（Buff "缓"，减速比例由 BuffSlowDown 读 StrengthBuff1[0]，时长 bufftime）
/// </summary>
public class SkillAidSlowStrike : Skill
{
    public SkillAidSlowStrike(int id, Chess unit) : base(id, unit)
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
            target.OnSkillDamaged(owner, id, GetSkillDamage());

        var slowBuffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (slowBuffCfg != null)
            BuffManager.AddBuff(target, owner, id, slowBuffCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillAidSlowStrike: 未找到减速Buff短名：" + skillCfg.BuffId);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}