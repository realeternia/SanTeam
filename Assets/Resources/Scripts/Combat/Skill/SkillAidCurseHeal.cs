using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 妖咒（ScriptName = "AidCurseHeal"）：辅助技能，对当前目标挂"败"（溃败·每秒法术伤害，由 BuffTimeDamage 结算），
/// 同时起协程按相同节奏（每秒一跳）按 StrengthInt 百分比把治疗能力（GetSkillHeal，与 DOT 伤害解耦）折算成每跳治疗，
/// 回复自身 Range 内生命比例最低的友方英雄（仅英雄，不给士兵；无人受伤则跳过）。
/// 用于于吉：妖道符水，以敌之血养己之众。
/// </summary>
public class SkillAidCurseHeal : Skill
{
    /// <summary>DOT 伤害与治疗转化的结算间隔(秒)，与"败"BuffTimeDamage 的跳间隔保持一致</summary>
    private const float TickInterval = 1f;

    public SkillAidCurseHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;

        if (!WorldManager.Instance.CheckInRange(owner.transform.position, target.transform.position, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 对敌人挂"败"：持续受到法术伤害（伤害由 BuffTimeDamage 按 Strength 结算）
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS(skillCfg.BuffId).Id, skillCfg.BuffTime);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);

        // 把持续伤害转化为治疗，回复附近生命最低的友军英雄
        owner.StartCoroutine(HealOverTime(target));
        return true;
    }

    private IEnumerator HealOverTime(Chess cursed)
    {
        // 治疗量基准为独立治疗公式 GetSkillHeal()，再按 StrengthInt 百分比折算每跳治疗（与 DOT 伤害公式解耦）
        var healBase = GetSkillHeal();
        var healPerTick = (int)(healBase * skillCfg.StrengthInt / 100f);
        var term = Mathf.Max(1, Mathf.FloorToInt(skillCfg.BuffTime / TickInterval));

        for (var i = 0; i < term; i++)
        {
            yield return new WaitForSeconds(TickInterval);

            if (owner == null || owner.hp <= 0)
                yield break;

            // 找 Range 内生命比例最低的友方英雄（仅英雄，不给士兵）
            var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Range, owner.side, false)
                .FindAll(x => x.isHero && x.hp < x.maxHp);

            Chess lowest = null;
            var lowestRate = float.MaxValue;
            foreach (var u in units)
            {
                if (u.HpRate < lowestRate)
                {
                    lowestRate = u.HpRate;
                    lowest = u;
                }
            }

            if (lowest != null && healPerTick > 0)
            {
                owner.HealTarget(lowest, skillId, healPerTick, true);
                EffectManager.PlaySkillEffect(lowest, skillCfg.HitEffect);
            }
        }

        GameLog.Debug($"妖咒 技能id={id} 等级={Level} 治疗基准={healBase} 每跳治疗={healPerTick} 跳数={term}");
    }
}
