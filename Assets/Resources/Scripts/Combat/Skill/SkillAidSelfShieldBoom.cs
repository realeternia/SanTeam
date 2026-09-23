using CommonConfig;

/// <summary>
/// 天人守城·自套护盾+盾破爆炸（ScriptName = "AidSelfShieldBoom"）：
/// 辅助技能，循环检查：当自身不存在护盾(BuffId="盾"=300001)时给自己施加护盾，
/// 护盾容量 = 自身最大生命 × Strength；护盾被移除时经 Chess.OnBuffRemoved 派发到本技能，
/// 覆写 OnBuffRemoved 响应：对本技能施加的护盾触发爆炸，对周围(Area)范围内敌人造成护盾容量 × SkillDamageRate 伤害。
/// 用于曹仁：天人将军坚壁自守，盾破反炸敌军。
/// </summary>
public class SkillAidSelfShieldBoom : Skill
{
    private int lastShieldHp; // 最近一次施加的护盾容量，用于计算爆炸伤害

    public SkillAidSelfShieldBoom(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (buffCfg == null)
        {
            GameLog.Error($"天人守城技能缺少Buff配置: BuffId={skillCfg.BuffId} 技能id={id}");
            return false;
        }

        // 已有护盾（含阵营护盾）或技能冷却中则不重复施加
        if (owner.GetBuff(buffCfg.Id) != null || IsInCD())
            return false;
        if (!CheckBurst(null))
            return false;

        var shieldHp = (int)(owner.maxHp * skillCfg.Strength);
        lastShieldHp = shieldHp;

        owner.PlayerAnim(skillCfg.Action);

        BuffManager.AddBuff(owner, owner, id, buffCfg.Id, skillCfg.BuffTime);
        var shield = owner.GetBuff(buffCfg.Id) as BuffShield;
        if (shield != null)
            shield.SetHp(shieldHp);

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        GameLog.Debug($"天人守城套盾 技能id={id} 等级={Level} 护盾容量={shieldHp} 爆炸倍率={skillCfg.SkillDamageRate} 爆炸范围={skillCfg.Area}");
        return true;
    }

    // buff 移除事件：仅响应本技能施加的护盾被移除（盾破触发爆炸，不依赖技能CD轮询）
    public override void OnBuffRemoved(Chess chess, Buff buff)
    {
        if (buff.skillCfg != null && buff.skillCfg.Id == id)
            Explode();
    }

    // 盾破爆炸：对周围(Area)范围内敌人造成护盾容量 × SkillDamageRate 伤害
    private void Explode()
    {
        if (owner == null || owner.hp <= 0)
            return;

        int boomDamage = (int)(lastShieldHp * skillCfg.SkillDamageRate);
        if (boomDamage <= 0)
            return;

        float radius = skillCfg.Area;
        var enemies = WorldManager.Instance.GetUnitsInRange(owner.transform.position, radius, owner.side, true);

        GameLog.Debug($"护盾破爆炸 护盾={lastShieldHp} 伤害={boomDamage} 范围={radius} 敌人数={enemies.Count}");

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.hp <= 0)
                continue;
            enemy.OnSkillDamaged(owner, skillCfg.Id, boomDamage, false, "");
        }

        EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
    }
}
