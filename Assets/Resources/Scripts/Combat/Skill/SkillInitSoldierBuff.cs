using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 安民（济世安民）：战斗开始全组士兵最大生命提升 maxHp × 键 maxHpRate、每秒生命回复提升 键 hpRegen 点。
/// 士兵血量成长机制参照相·运筹 SkillSoldierGrow；士兵为 Chess，自带 maxHp/hpRegen。
/// </summary>
public class SkillInitSoldierBuff : Skill
{
    public SkillInitSoldierBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        var count = 0;
        foreach (var s in WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero))
        {
            s.maxHp += (int)(s.maxHp * GetStr2("maxHpRate"));
            s.hp += (int)(s.maxHp * GetStr2("maxHpRate"));
            s.hpRegen += GetStr2("hpRegen");
            count++;
        }
        GameLog.Debug($"安民 武将{owner.heroId} 全组士兵{count}名 最大生命+{(int)(GetStr2("maxHpRate") * 100)}% 生命回复+{GetStr2("hpRegen")}");
    }
}