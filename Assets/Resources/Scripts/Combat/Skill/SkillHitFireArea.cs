using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 火攻场：类似HitWall，攻击时在目标位置召唤一个火焰场，持续造成百分比魔法伤害；
/// 蔓延：若目标位置周围 Area 内已存在火（SummonTag=火，不区分来源，场景中任意火场都算），则按发动概率(Rate)在 Range 范围内随机敌人位置放火，等级越高概率越高
/// 非持续施法：放火后施法者可正常移动/普攻，火焰场按 SummonTime 独立持续跳伤（施法者死亡则停止跳伤）
/// </summary>
public class SkillHitFireArea : Skill
{
    public SkillHitFireArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            var targetPos = defender.transform.position;

            // 本次需要持续结算伤害的火焰位置（主火 + 蔓延放的火），随协程传递，避免重复触发时互相覆盖
            var posList = new List<Vector3>();

            // 蔓延：目标位置周围 Area 内已有火，则按发动概率(Rate)在 Range 范围内随机敌人位置放火
            if (HasFireNear(targetPos))
            {
                var spreadRate = Mathf.Clamp01(skillCfg.Rate);
                if (spreadRate > 0 && SysRandom.Value < spreadRate)
                    SpreadFire(targetPos, posList);
            }
            else
            {
                // 若目标位置周围 Area 内无火，则在目标位置召唤一个火焰场
                AddFire(targetPos, posList);
            }

            owner.StartCoroutine(DelayDamage(posList));
        }
    }

    // 放火：登记结算位置并创建火焰场
    private void AddFire(Vector3 pos, List<Vector3> posList)
    {
        posList.Add(pos);
        var magicStub = SummonMagicField(pos, out var summonTime);
        EffectManager.PlayPosSkillEffect(magicStub, pos, skillCfg.Area, skillCfg.AreaEffect, summonTime);
    }

    // 判断目标位置周围 Area 内是否有火（场景中任意 SummonTag 相同的火场，不区分是否本技能所放）
    private bool HasFireNear(Vector3 center)
    {
        return WorldManager.Instance.GetUnitsInRangeByTag(center, skillCfg.Area, skillCfg.SummonTag).Count > 0;
    }

    // 在 Range 范围内随机敌人位置放火
    private void SpreadFire(Vector3 center, List<Vector3> posList)
    {
        var enemies = WorldManager.Instance.GetEnemyInRange(center, skillCfg.Area, owner.side);
        if (enemies.Count <= 0)
            return;
        WorldManager.Instance.RandomSelect(enemies, Math.Max(1, skillCfg.TargetCount));
        foreach (var unit in enemies)
            AddFire(unit.transform.position, posList);
    }

    IEnumerator DelayDamage(List<Vector3> posList)
    {
        var term = (int)Math.Floor(skillCfg.SummonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            // 非持续施法：火焰场独立持续结算，仅要求施法者存活（施法者死亡后随 GameObject 销毁停止跳伤）
            if (owner == null || owner.hp <= 0)
                yield break;

            var unitList = new List<Chess>();
            foreach (var pos in posList)
            {
                var unitsInRange = WorldManager.Instance.GetEnemyInRange(pos, skillCfg.Area * 1.5f, owner.side);
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
                // 百分比魔法伤害：经 OnSkillDamaged 按 DamageType 受抗性减免
                unit.OnSkillDamaged(owner, skillId, dmg);
            }

            yield return new WaitForSeconds(skillCfg.SummonHitInterval);
        }
    }
}
