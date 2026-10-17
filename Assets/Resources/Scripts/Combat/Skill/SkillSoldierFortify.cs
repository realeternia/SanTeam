using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 张昭·筑垒：每次主动释放对1名士兵（优先生命比例最低者）回复满血，并施加"复活"Buff（BuffRevive），
/// 该单位死亡后 BuffRevive.ReviveDelay(3)秒原地复活(满血，每个单位至多一次)；场上无士兵时改为治疗自身，保证技能不空放。
/// Buff 由 SkillConfig.BuffId 指定（短名"复"），不在战斗代码硬编码 BuffId。
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
        PlaySelfEffect();

        var soldiers = WorldManager.Instance.GetUnitsMySide(owner.side).Where(x => !x.isHero).ToList();
        // 优先对生命比例最低的士兵施加复活；无士兵时改为治疗自身
        var target = soldiers.Count > 0 ? soldiers.OrderBy(x => x.HpRate).First() : owner;

        if (target.hp < target.maxHp)
            owner.HealTarget(target, skillId, target.maxHp - target.hp, false);

        // 复活 Buff 由 SkillConfig.BuffId 指定（短名"复"→BuffRevive），不在战斗代码硬编码 BuffId
        var reviveCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (reviveCfg == null)
        {
            GameLog.Error($"SkillSoldierFortify: 未找到复活Buff短名：{skillCfg.BuffId} 技能id={id}");
            return false;
        }
        BuffManager.AddBuff(target, owner, id, reviveCfg.Id, 999f);
        EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        return true;
    }
}
