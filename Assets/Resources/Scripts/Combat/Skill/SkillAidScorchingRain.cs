using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 蒋钦·江河：对自身 /area 范围内敌人各造成 /damagestrength 法术伤害，并使其每秒流失 /strength2-1 点生命（持续 dot，bufftime 秒）
/// </summary>
public class SkillAidScorchingRain : Skill
{
    public SkillAidScorchingRain(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        var target = owner.targetChess;
        if (target == null || target.hp <= 0)
            return false;
        if (!CheckBurst(target))
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlayAreaEffect(owner.transform.position);

        var skillDamage = GetSkillDamage();
        var units = WorldManager.Instance.GetUnitsInRange(owner.transform.position, skillCfg.Area, owner.side, true);
        var secDamage = (int)skillCfg.Strength2[0];
        foreach (var u in units)
        {
            if (skillDamage > 0)
                u.OnSkillDamaged(owner, id, skillDamage);
            if (secDamage > 0)
                owner.StartCoroutine(DotTicks(u, secDamage));
        }

        return true;
    }

    // 江河持续流失：bufftime 秒内每秒扣血
    IEnumerator DotTicks(Chess target, int secDamage)
    {
        var endTime = Time.time + skillCfg.BuffTime;
        while (Time.time < endTime)
        {
            yield return new WaitForSeconds(1f);
            if (target == null || target.hp <= 0)
                yield break;
            target.OnSkillDamaged(owner, id, secDamage);
        }
    }
}