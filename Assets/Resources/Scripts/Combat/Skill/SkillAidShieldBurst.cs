using CommonConfig;

/// <summary>
/// 李典·武卫：自套护盾+盾破爆裂（ScriptName = "SkillAidShieldBurst"）：
/// 循环检查：当自身不存在护盾("盾"=300001)且技能就绪时给自己套盾，护盾量 = Strength2[0] × (100+法强)/100（随法强成长）；
/// 护盾被移除时经 Chess.OnBuffRemoved 派发到本技能，对周围(Area)范围内敌人造成 GetSkillDamage() 爆裂伤害。
/// </summary>
public class SkillAidShieldBurst : Skill
{
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

        var shieldHp = GetSkillShield(0);

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        var shield = owner.GetBuff(buffCfg.Id) as BuffShield;
        if (shield != null)
            shield.SetHp(shieldHp);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
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