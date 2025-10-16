using CommonConfig;

public class SkillInitMasterShield : Skill
{
    public SkillInitMasterShield(int id, Chess chess) : base(id, chess)
    {
        // 初始化护盾
    }

    public override void BattleBegin()
    {
        var unitList = WorldManager.Instance.GetUnitsMySide(owner.transform.position, 0, owner.side);

        var mySide = HeroConfig.GetConfig(owner.heroId).Side;
        var shieldHp = (int)(owner.maxHp * skillCfg.Strength);
        owner.PlayerAnim(skillCfg.Action);

        foreach (var unit in unitList)
        {
            if (unit.hp <= 0 || unit == owner)
                continue;
            if(!unit.isHero)
                continue;
            var heroCfg = HeroConfig.GetConfig(unit.heroId);
            if (heroCfg.Side == mySide)
            {
                UnityEngine.Debug.Log("SkillInitMasterShield BattleBegin 护盾值 " + shieldHp);
                BuffManager.AddBuff(unit, owner, skillCfg.Id, skillCfg.BuffId, skillCfg.BuffTime);
            }
        }
    }
}