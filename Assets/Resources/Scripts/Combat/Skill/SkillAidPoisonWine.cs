using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 李儒·鸩酒（术）：对目标造成 /strength 法术伤害，使其持续中毒每秒 /strength2 伤害（持续 dot），
/// 并降低其受治疗 /strengthint%（Buff "疫"，时长 bufftime），双毒并施。
/// </summary>
public class SkillAidPoisonWine : Skill
{
    public SkillAidPoisonWine(int id, Chess unit) : base(id, unit)
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

        if (GetSkillDamage() > 0)
            target.OnSkillDamaged(owner, skillId, GetSkillDamage());

        var secDamage = (int)skillCfg.Strength2;
        if (secDamage > 0)
            owner.StartCoroutine(DotTicks(target, secDamage));

        // 减疗（"疫"）
        BuffManager.AddBuff(target, owner, id, BuffConfig.GetConfigByNameS("疫").Id, skillCfg.BuffTime);

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

    // 鸩毒持续扣血：bufftime 秒内每秒扣血
    IEnumerator DotTicks(Chess target, int secDamage)
    {
        var endTime = Time.time + skillCfg.BuffTime;
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(1f);
            if (target == null || target.hp <= 0)
                yield break;
            target.OnSkillDamaged(owner, skillId, secDamage);
        }
    }
}