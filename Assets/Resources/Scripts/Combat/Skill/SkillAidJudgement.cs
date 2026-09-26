using System.Collections;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 法正·明断（术）：对目标位置"area 范围降下雷阵，持续 bufftime 秒，每秒对其中敌人造成 /strength 法术伤害。
/// 仿张角落雷/陆逊火海：召唤法术场(可视化) + 协程每秒对范围内敌人结算一次。
/// </summary>
public class SkillAidJudgement : Skill
{
    public SkillAidJudgement(int id, Chess unit) : base(id, unit)
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

        var pos = target.transform.position;
        var magicStub = SummonMagicField(pos, out var summonTime);
        if (magicStub != null)
            EffectManager.PlayPosSkillEffect(magicStub, pos, skillCfg.EffectSize, skillCfg.HitEffect, summonTime);

        owner.StartCoroutine(ThunderField(pos));
        return true;
    }

    // 协程：在技能持续期内每秒对落点范围内的敌人结算雷击伤害
    IEnumerator ThunderField(Vector3 pos)
    {
        var endTime = Time.time + skillCfg.BuffTime;
        while (Time.time < endTime)
        {
            if (owner == null || owner.hp <= 0)
                yield break;

            var unitList = WorldManager.Instance.GetUnitsInRange(pos, skillCfg.Area, owner.side, true);
            var skillDamage = GetSkillDamage();
            foreach (var unit in unitList)
            {
                if (skillDamage > 0)
                    unit.OnSkillDamaged(owner, skillId, skillDamage);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}