using CommonConfig;

/// <summary>
/// 赵云·龙胆（ScriptName = "SkillAidShieldBurst"）：自套护盾+盾破爆裂，盾期普攻强化（攻守兼备）：
/// 循环检查：当自身不存在护盾且技能就绪时给自己套盾，护盾量 = Strength2[0] × (100+法强)/100（随法强成长），
/// 并同步附加龙胆状态（BuffShieldGuard，与护盾同时长）：护盾期间攻击力提升 StrengthBuff1[0] 点、普攻附带额外法术伤害 Strength2[1]；
/// 护盾被移除时经 Chess.OnBuffRemoved 派发到本技能，对周围(Area)范围内敌人造成 GetSkillDamage() 爆裂伤害。
/// Buff 短名由 SkillConfig.BuffId 以逗号分隔配置：第 1 个=护盾Buff，第 2 个=龙胆Buff（如 "盾,胆"）。
/// </summary>
public class SkillAidShieldBurst : Skill
{
    public SkillAidShieldBurst(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // BuffId 逗号分隔：buffIds[0]=护盾Buff，buffIds[1]=龙胆Buff
        var buffIds = GetSkillBuffIds();
        if (buffIds.Count < 2)
        {
            GameLog.Error($"SkillAidShieldBurst: BuffId 需配置护盾与龙胆两个buff（如\"盾,胆\"）技能id={id}");
            return false;
        }
        int shieldBuffId = buffIds[0];
        int guardBuffId = buffIds[1];

        // 已有护盾或技能冷却中则不重复施加
        if (owner.GetBuff(shieldBuffId) != null || IsInCD())
            return false;
        if (!CheckBurst(null))
            return false;

        var shieldHp = GetSkillShield(0);

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, shieldBuffId, skillCfg.BuffTime);
        var shield = owner.GetBuff(shieldBuffId) as BuffShield;
        if (shield != null)
            shield.SetHp(shieldHp);

        // 龙胆"胆"：护盾期间攻击力提升 + 普攻附带额外法术伤害（与护盾同时长），让套盾转为攻守兼备
        BuffManager.AddBuff(owner, owner, id, guardBuffId, skillCfg.BuffTime);

        PlaySelfEffect();
        return true;
    }

    // buff 移除事件：仅响应本技能施加的护盾"被击破"时爆裂（自然到期不触发）
    public override void OnBuffRemoved(Chess chess, Buff buff)
    {
        if (buff == null || buff.skillCfg == null || buff.skillCfg.Id != id)
            return;

        // 自然到期时护盾仍有剩余值（>0），只有被打空吞噬成 0 才算被击破
        var shield = buff as BuffShield;
        if (shield == null || shield.GetHp() > 0)
            return;

        Explode();
    }

    // 盾破爆裂：对周围(Area)范围内敌人造成技能伤害
    private void Explode()
    {
        if (owner == null || owner.hp <= 0)
            return;

        int boomDamage = GetSkillDamage();
        if (boomDamage <= 0)
            return;

        float radius = skillCfg.Area;
        var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, radius, owner.side);

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            enemy.OnSkillDamaged(owner, skillCfg.Id, boomDamage, false, "");
            if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                EffectManager.PlaySkillEffect(enemy, skillCfg.HitEffect);
        }

        PlaySelfEffect();
    }
}