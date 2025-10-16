using System;
using CommonConfig;
using UnityEngine;

public class SkillInitSoldierSummon : Skill
{
    public SkillInitSoldierSummon(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        // 在自己身边 Range 范围内随机选择一个位置
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 randomPosition = owner.transform.position + new Vector3(randomDir.x, 0, randomDir.y) * skillCfg.Range;
        WorldManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 500002, -1, randomPosition, owner.side, owner.GetPlayerInfo().imgPath);
        owner.PlayerAnim(skillCfg.Action);

        var units = WorldManager.Instance.GetUnitsMySide(randomPosition, 0, owner.side);
        var rangeAdd = Math.Min(20, owner.GetAttr(skillCfg.Attr) * skillCfg.Strength);
        foreach(var unit in units)
        {
            if(unit.isHero)
                continue;

            if(unit.attackRange < 20)
                continue;

            unit.attackRange += rangeAdd;
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
    }
}