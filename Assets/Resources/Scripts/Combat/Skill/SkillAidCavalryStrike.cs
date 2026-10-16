using CommonConfig;

/// <summary>
/// 马超·西凉铁骑（武）：对目标连续突击 3 次，每次造成 GetSkillDamage 物理伤害，
/// 并施加一层叠加破甲（Buff 短名由 skillCfg.BuffId 配置）BuffTime 秒，护甲越打越低。
/// </summary>
public class SkillAidCavalryStrike : Skill
{
    /// <summary>突击次数</summary>
    private const int Hits = 3;

    public SkillAidCavalryStrike(int id, Chess unit) : base(id, unit)
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

        var shredCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (shredCfg == null)
            GameLog.Error("SkillAidCavalryStrike: 未找到破甲Buff短名：" + skillCfg.BuffId);

        for (int i = 0; i < Hits; i++)
        {
            if (target == null || target.hp <= 0)
                break;

            var damage = GetSkillDamage();
            if (damage > 0)
            {
                target.OnSkillDamaged(owner, skillId, damage);
                EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
            }

            if (shredCfg != null)
                BuffManager.AddBuff(target, owner, id, shredCfg.Id, skillCfg.BuffTime);
        }

        return true;
    }
}