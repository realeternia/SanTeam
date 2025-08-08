using CommonConfig;

public class SkillMasterShield : Skill
{
    public SkillMasterShield(int id, Chess chess) : base(id, chess)
    {
        // 初始化护盾
    }

    public override void BattleBegin()
    {
        var unitList = WorldManager.Instance.GetUnitsInRange(owner.transform.position, 0, owner.side, false);

        var mySide = HeroConfig.GetConfig((uint)owner.heroId).Side;

        foreach (var unit in unitList)
        {
            if (unit.hp <= 0 || unit == owner)
                continue;
            var heroCfg = HeroConfig.GetConfig((uint)unit.heroId);
            if (heroCfg.Side == mySide)
            {
                var shieldHp = (int)(owner.maxHp * skillCfg.Strength);
                UnityEngine.Debug.Log("护盾值 " + shieldHp);

                BuffManager.AddBuff(unit, owner, (int)skillCfg.BuffId, skillCfg.BuffTime, shieldHp);
            }

        }
    }
}