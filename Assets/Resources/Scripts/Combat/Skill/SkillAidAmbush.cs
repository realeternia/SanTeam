using CommonConfig;

/// <summary>
/// 吕蒙·白衣渡江：对当前攻击目标（Range 内）造成 GetSkillDamage 伤害并施加减速；
/// 同时给自己挂持续回血与魔抗提升 Buff，持续 BuffTime 秒。
/// BuffId 以逗号分隔：第 1 个给目标（减速），其余给自身（回血、魔抗），如 "缓,愈,抗"。
/// </summary>
public class SkillAidAmbush : Skill
{
    public SkillAidAmbush(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;

        // BuffId 逗号分隔：第 1 个=目标减速，其余=自身回血/魔抗；缺配置时不发动，避免白吃 CD
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 1)
        {
            GameLog.Error($"SkillAidAmbush: 缺少减速Buff配置 BuffId={skillCfg.BuffId} 技能id={id}");
            return false;
        }

        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        owner.LockTarget(target);

        // 伤害 + 减速（第 1 个 Buff 施加给目标）
        var damage = GetSkillDamage();
        if (damage > 0)
        {
            target.OnSkillDamaged(owner, id, damage);
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }
        BuffManager.AddBuff(target, owner, id, buffIds[0], skillCfg.BuffTime);

        // 自身增益：快速回血 + 魔抗提升（其余 Buff 施加给自己）
        for (var i = 1; i < buffIds.Count; i++)
            BuffManager.AddBuff(owner, owner, id, buffIds[i], skillCfg.BuffTime);

        return true;
    }
}
