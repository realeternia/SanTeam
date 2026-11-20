using UnityEngine;
using CommonConfig;

/// <summary>
/// 免死（ScriptName = "SkillComDefendCheatDeath"，模板「免死一次」）：
/// 受击方在伤害计算阶段（BeforeCalDamaged）判断本次伤害是否致命：
/// · 致命：把本次伤害压到只留 1 点生命；
/// · 非致命：按 SkillConfig.BuffId 给自己挂状态组件配置的 Buff（受击叠防/护盾/回血等）。
/// 免死与叠 Buff 是同一触发时机：都走 CheckBurst，同一次触发扣 MP / 算 CD；
/// 免死频率完全由 CD 控制，配表把 CD 拉长即可实现"每场一次"。
/// 受击类型 / 伤害源 / 伤害量门槛与 SkillComDefendBefore 共用 MeetDefendGate。
/// 参考金铲铲：复活甲、塔里克·宇宙之辉。
/// </summary>
public class SkillComDefendCheatDeath : SkillComDefendBefore
{
    public SkillComDefendCheatDeath(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BeforeCalDamaged(Chess caster, SkillConfig castSkillCfg, ref int damageBase, ref float damageMulti, ref string effect, string hurtTag, bool isFeedback)
    {
        // 致命判定要在伤害量门槛之前：残血时的小额伤害也可能致死，不能被 minHpRate 过滤掉
        var lethal = (int)(damageBase * damageMulti) >= owner.hp;
        int damageType;
        if (!MeetDefendGate(caster, castSkillCfg, damageBase, damageMulti, hurtTag, out damageType, lethal))
            return;
        // 非致命：没有要挂的状态组件时无需消耗 MP；免死只受 CD 限制，不做额外计数
        if (!lethal && GetSkillBuffIds().Count == 0 && GetSkillShield() <= 0)
            return;
        // 免死与叠 Buff 共用同一次触发：统一扣 MP / 算 CD
        if (!CheckBurst(caster))
            return;

        if (lethal)
        {
            // 压到 max(hp-1, 0)：damageBase * damageMulti 向下取整后不致死
            damageMulti = Mathf.Max(0f, owner.hp - 1f) / damageBase;
        }

        if (!string.IsNullOrEmpty(skillCfg.Action))
            owner.PlayerAnim(skillCfg.Action);

        ApplyDefendBuffs();

        PlaySelfEffect();
        GameLog.Debug($"免死/防御触发 技能id={id} 等级={Level} 类型={damageType} 致命={lethal} 生命={owner.hp}/{owner.maxHp}");
    }
}
