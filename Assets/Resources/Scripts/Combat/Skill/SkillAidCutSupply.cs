using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 程昱·断粮（术）：对目标造成 /damagestrength 法术伤害，并使其每秒流失 /strength2-1 生命（持续 dot），持续 bufftime 秒。
/// </summary>
public class SkillAidCutSupply : Skill
{
    public SkillAidCutSupply(int id, Chess unit) : base(id, unit)
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

        var secDamage = (int)skillCfg.Strength2[0];
        if (secDamage > 0)
            owner.StartCoroutine(DotTicks(target, secDamage));

        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }

    // 断粮持续失血：bufftime 秒内每秒扣血
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