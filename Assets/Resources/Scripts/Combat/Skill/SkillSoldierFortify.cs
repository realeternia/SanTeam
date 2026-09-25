using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张昭·筑垒：每次主动释放对1名士兵永久强化双防(护甲=Strength、魔抗=Strength2，BuffBuildFort)并回复满血；
/// 该士兵死亡后 BuffBuildFort.ReviveDelay(3)秒原地复活(满血，筑垒双防随死亡移除，每名至多一次)。
/// </summary>
public class SkillSoldierFortify : Skill
{
    public SkillSoldierFortify(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;
        owner.PlayerAnim(skillCfg.Action);

        var soldiers = WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero).ToList();
        if (soldiers.Count > 0)
        {
            // 优先对生命比例最低的士兵筑垒
            var target = soldiers.OrderBy(x => x.HpRate).First();
            if (target.hp < target.maxHp)
                owner.HealTarget(target, skillId, target.maxHp - target.hp, false);
            BuffManager.AddBuff(target, owner, id, CombatConst.BuildFortBuffId, 999f);
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        }
        return true;
    }
}