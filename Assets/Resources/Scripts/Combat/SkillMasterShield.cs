using CommonConfig;

public class SkillMasterShield : Skill
{
    public SkillMasterShield(int id, Chess chess) : base(id, chess)
    {
        // 初始化护盾
    }

    public override void BattleBegin()
    {
        UnityEngine.Debug.Log("BattleBegin SkillMasterShield " + owner.heroId.ToString());

        var unitList = WorldManager.Instance.GetUnitsInRange(owner.transform.position, 0, owner.side, false);

        var mySide = HeroConfig.GetConfig(owner.heroId).Side;

        foreach (var unit in unitList)
        {
            if (unit.hp <= 0 || unit == owner)
                continue;
            if(!unit.isHero)
                continue;
            var heroCfg = HeroConfig.GetConfig(unit.heroId);
            if (heroCfg.Side == mySide)
            {
                var shieldHp = (int)(owner.maxHp * skillCfg.Strength);
                UnityEngine.Debug.Log("BattleBegin 护盾值 " + shieldHp);

                BuffManager.AddBuff(unit, owner, (int)skillCfg.BuffId, skillCfg.BuffTime, shieldHp);
            }

        }
    }
}