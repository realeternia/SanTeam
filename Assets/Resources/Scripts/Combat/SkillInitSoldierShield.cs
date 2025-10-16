using CommonConfig;

public class SkillInitSoldierShield : Skill
{
    public SkillInitSoldierShield(int id, Chess chess) : base(id, chess)
    {
        // 初始化护盾
    }

    public override void BattleBegin()
    {
        var unitList = WorldManager.Instance.GetUnitsMySide(owner.transform.position, 0, owner.side);
        owner.PlayerAnim(skillCfg.Action);

        var shieldHp = (int)(owner.maxHp * skillCfg.Strength);
        foreach (var unit in unitList)
        {
            if (unit.hp <= 0 || unit == owner)
                continue;
            if (unit.isHero || unit.attackRange > 30)
                continue;

            UnityEngine.Debug.Log("BattleBegin SkillInitSoldierShield 护盾值 " + shieldHp);
            BuffManager.AddBuff(unit, owner, skillCfg.Id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }
}