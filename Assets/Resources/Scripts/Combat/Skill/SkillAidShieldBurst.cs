using CommonConfig;

/// <summary>
/// 李典·武卫：自套护盾+盾破爆裂（ScriptName = "SkillAidShieldBurst"）：
/// 循环检查：当自身不存在护盾("盾"=300001)且技能就绪时给自己套盾，护盾量 = 自身最大生命 × Strength2[0]；
/// 护盾被移除时经 Chess.OnBuffRemoved 派发到本技能，对周围(Area)范围内敌人造成护盾容量 × Strength2[1] 爆裂伤害。
/// </summary>
public class SkillAidShieldBurst : Skill
{
    private int lastShieldHp; // 最近一次施加的护盾容量，用于爆裂伤害计算

    public SkillAidShieldBurst(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"武卫技能缺少Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return false;
        }

        // 已有护盾或技能冷却中则不重复施加
        if (owner.GetBuff(buffCfg.Id) != null || IsInCD())
            return false;
        if (!CheckBurst(null))
            return false;

        var shieldHp = (int)(owner.maxHp * skillCfg.Strength2[0]);
        lastShieldHp = shieldHp;

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        var shield = owner.GetBuff(buffCfg.Id) as BuffShield;
        if (shield != null)
            shield.SetHp(shieldHp);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        return true;
    }

    // buff 移除事件：仅响应本技能施加的护盾被移除（盾破触发爆裂，不依赖技能CD轮询）
    public override void OnBuffRemoved(Chess chess, Buff buff)
    {
        if (buff.skillCfg != null && buff.skillCfg.Id == id)
            Explode();
    }

    // 盾破爆裂：对周围(Area)范围内敌人造成护盾容量 × Strength2[1] 伤害
    private void Explode()
    {
        if (owner == null || owner.hp <= 0)
            return;

        int boomDamage = (int)(lastShieldHp * skillCfg.Strength2[1]);
        if (boomDamage <= 0)
            return;

        float radius = skillCfg.Area;
        var enemies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, radius, owner.side, true);

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            enemy.OnSkillDamaged(owner, skillCfg.Id, boomDamage, false, "");
        }

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}