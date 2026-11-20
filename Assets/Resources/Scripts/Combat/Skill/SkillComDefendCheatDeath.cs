using CommonConfig;
using UnityEngine;

/// <summary>
/// 免死（ScriptName = "SkillComDefendCheatDeath"，模板「免死一次」）：
/// 受击方在伤害计算阶段（BeforeCalDamaged）判断本次伤害是否致命，若致命则把本次伤害压到只留 1 点生命，
/// 每场战斗只生效一次（used 标记），随后按 SkillConfig.BuffId 给自身挂护盾/回血等 Buff 稳住血量。
/// 触发闸门由 CheckBurst 统一判定（可用 TriggerCondition 限定如 hprate&lt;30 才触发）。
/// 参考金铲铲：复活甲、塔里克·宇宙之辉。
/// </summary>
public class SkillComDefendCheatDeath : Skill
{
    /// <summary>每场战斗只免死一次</summary>
    private bool used;

    public SkillComDefendCheatDeath(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BeforeCalDamaged(Chess caster, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        if (used || owner == null || owner.hp <= 0 || damageBase <= 0 || damageMulti <= 0f)
            return;
        // 派生伤害不触发免死，避免免死与反伤/连锁互相递归
        if (CombatConst.IsDerivedHurtTag(hurtTag))
            return;

        // 本次结算后的预计伤害，未致命则不触发
        if ((int)(damageBase * damageMulti) < owner.hp)
            return;
        if (!CheckBurst(caster))
            return;

        used = true;
        // 压到 max(hp-1, 0)：damageBase * damageMulti 向下取整后不致死
        damageMulti = Mathf.Max(0f, owner.hp - 1f) / damageBase;

        if (!string.IsNullOrEmpty(skillCfg.Action))
            owner.PlayerAnim(skillCfg.Action);

        // 免死后补盾/回血，按 BuffId 配置（可选）
        var buffIds = GetSkillBuffIds();
        foreach (var buffId in buffIds)
            BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
        var shieldHp = GetSkillShield();
        if (shieldHp > 0)
        {
            foreach (var buffId in buffIds)
            {
                var shield = owner.GetBuff(buffId) as BuffShield;
                if (shield != null)
                    shield.AddHp(shieldHp);
            }
        }

        PlaySelfEffect();
        GameLog.Debug($"免死触发 技能id={id} 等级={Level} 生命={owner.hp}/{owner.maxHp} 护盾={shieldHp}");
    }
}
