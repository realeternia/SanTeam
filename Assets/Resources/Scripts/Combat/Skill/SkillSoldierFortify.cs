using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张昭·筑垒：每次主动释放对1名士兵（优先生命比例最低者）永久强化双防
/// (护甲=StrengthBuff1[0]、魔抗=StrengthBuff1[1]，BuffBuildFort)并回复满血；
/// 场上无士兵时改为强化自身，保证技能不空放。
/// 该单位死亡后 BuffBuildFort.ReviveDelay(3)秒原地复活(满血，筑垒双防随死亡移除，每个单位至多一次)。
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
        // 优先对生命比例最低的士兵筑垒；无士兵时强化自身
        var target = soldiers.Count > 0 ? soldiers.OrderBy(x => x.HpRate).First() : owner;

        if (target.hp < target.maxHp)
            owner.HealTarget(target, skillId, target.maxHp - target.hp, false);

        // 筑垒 Buff 由 SkillConfig.BuffId 指定（短名"垒"→BuffBuildFort），不在战斗代码硬编码 BuffId
        var fortCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (fortCfg == null)
        {
            GameLog.Error($"SkillSoldierFortify: 未找到筑垒Buff短名：{skillCfg.BuffId} 技能id={id}");
            return false;
        }
        BuffManager.AddBuff(target, owner, id, fortCfg.Id, 999f);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
