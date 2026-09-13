using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 火攻场：类似HitWall，攻击时在目标位置召唤一个火焰场，持续造成百分比魔法伤害；
/// 蔓延：若目标位置周围 Area 内已存在火（SummonTag=火，不区分来源，场景中任意火场都算），则按概率(SkillDamageRate)在 Range 范围内随机敌人位置放火，等级越高概率越高
/// </summary>
public class SkillHitFireArea : Skill
{
    // 本次需要持续结算伤害的火焰位置（主火 + 蔓延放的火）
    private List<Vector3> targetPosList;

    public SkillHitFireArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            var targetPos = defender.transform.position;

            targetPosList = new List<Vector3>();

            // 蔓延：目标位置周围 Area 内已有火，则按放置概率在 Range 范围内随机敌人位置放火
            if (HasFireNear(targetPos))
            {
                var spreadRate = Mathf.Clamp01(skillCfg.SkillDamageRate);
                if (spreadRate > 0 && SysRandom.Value < spreadRate)
                    SpreadFire(targetPos);
            }
            else
            {
                // 若目标位置周围 Area 内无火，则在目标位置召唤一个火焰场
                AddFire(targetPos);
            }

            owner.StartCoroutine(DelayDamage());
        }
    }

    // 放火：登记结算位置并创建火焰场
    private void AddFire(Vector3 pos)
    {
        targetPosList.Add(pos);
        var magicStub = SummonMagicField(pos, out var summonTime);
        EffectManager.PlayPosSkillEffect(magicStub, pos, skillCfg.EffectSize, skillCfg.HitEffect, summonTime);
    }

    // 判断目标位置周围 Area 内是否有火（场景中任意 SummonTag 相同的火场，不区分是否本技能所放）
    private bool HasFireNear(Vector3 center)
    {
        return WorldManager.Instance.GetUnitsInRangeByTag(center, skillCfg.Area, skillCfg.SummonTag).Count > 0;
    }

    // 在 Range 范围内随机敌人位置放火
    private void SpreadFire(Vector3 center)
    {
        var enemies = WorldManager.Instance.GetUnitsInRange(center, skillCfg.Area, owner.side, true);
        if (enemies.Count <= 0)
            return;
        WorldManager.Instance.RandomSelect(enemies, Math.Max(1, skillCfg.TargetCount));
        foreach (var unit in enemies)
            AddFire(unit.transform.position);
    }

    IEnumerator DelayDamage()
    {
        var term = (int)Math.Floor(skillCfg.SummonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            if (owner == null || owner.hp <= 0)
                yield break;

            var unitList = new List<Chess>();
            foreach (var pos in targetPosList)
            {
                var unitsInRange = WorldManager.Instance.GetUnitsInRange(pos, skillCfg.Area * 1.5f, owner.side, true);
                WorldManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
                foreach (var unit in unitsInRange)
                {
                    if (!unitList.Contains(unit))
                        unitList.Add(unit);
                }
            }

            var dmg = GetSkillDamage();
            foreach (var unit in unitList)
            {
                // 百分比魔法伤害：以目标�分比为�分比为伤害，经 OnSkillDamaged 按 IsMagic 受魔抗减免
                unit.OnSkillDamaged(owner, skillId, dmg);
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }
}