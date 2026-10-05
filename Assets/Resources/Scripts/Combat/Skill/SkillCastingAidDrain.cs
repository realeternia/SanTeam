using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 吕布·魔神（术）：持续施法 SummonTime 秒，施法开始即眩晕目标 BuffTime 秒；
/// 引导期间每秒吸取目标生命（造成 GetSkillDamage 法术伤害）并把等量生命回复自身（转给自己）。
/// 施法期间自身处于引导状态(castingSkillId>0)，无法移动/普攻，被晕眩/死亡打断；目标死亡则提前结束。
/// </summary>
public class SkillCastingAidDrain : Skill
{
    /// <summary>眩晕 Buff 短名（BuffNoAction "乱"）</summary>
    public const string StunBuffNameS = "乱";

    public SkillCastingAidDrain(int id, Chess unit) : base(id, unit)
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

        // 晕眩目标：持续 BuffTime 秒
        var stunCfg = BuffConfig.GetConfigByNameS(StunBuffNameS);
        if (stunCfg != null)
            BuffManager.AddBuff(target, owner, id, stunCfg.Id, skillCfg.BuffTime);
        else
            GameLog.Error("SkillCastingAidDrain: 未找到眩晕Buff短名：" + StunBuffNameS);

        owner.StartCasting(id, DrainChannel(target));
        return true;
    }

    IEnumerator DrainChannel(Chess target)
    {
        var endTime = Time.time + Mathf.Max(0.1f, skillCfg.SummonTime);
        while (Time.time < endTime)
        {
            // 被打断/死亡：castingSkillId 被 BreakCasting 置0，结束引导
            if (owner == null || owner.hp <= 0 || owner.castingSkillId != id)
                yield break;
            // 吸取目标死亡则提前结束
            if (target == null || target.hp <= 0)
                yield break;

            owner.PlayerAnim(skillCfg.Action);

            var damage = GetSkillDamage();
            if (damage > 0)
            {
                target.OnSkillDamaged(owner, skillId, damage);
                // 吸取：把造成的伤害转为自身生命（吸血不算治疗，不吃治疗加成）
                owner.HealTarget(owner, skillId, damage, false);
                EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}