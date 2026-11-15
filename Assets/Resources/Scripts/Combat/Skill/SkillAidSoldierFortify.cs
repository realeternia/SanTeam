using CommonConfig;
using UnityEngine;

/// <summary>
/// 张昭·筑垒（ScriptName = "SkillAidSoldierFortify"）：
/// 将一名敌方英雄强制拖回我方出生点（owner.spawnPosition），造成一次法术伤害，
/// 并挂「陷阵」(CombatConst.BuffNoMoveId) skillCfg.BuffTime 秒定身，防止其立刻走回。
/// </summary>
public class SkillAidSoldierFortify : Skill
{
    public SkillAidSoldierFortify(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (!CheckBurst(null))
            return false;

        // 目标：按 SelectOnly / SelectFirst 选（典型 enemy;hero + nearest）
        var picked = SelectTargets(owner.transform.position, skillCfg.Area, false, 1);
        var target = picked.Count > 0 ? picked[0] : null;
        if (target == null)
            return false;

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();

        // 拖回我方出生点
        target.MoveTo(owner.spawnPosition, true);

        // 单体法术伤害（走统一技能伤害结算）
        var damage = GetSkillDamage();
        if (damage > 0 && target.hp > 0)
            target.OnSkillDamaged(owner, skillId, damage);

        // 定身：挂「陷阵」，防止目标立刻走回
        if (target.hp > 0)
        {
            var buffIds = GetSkillBuffIds();
            var noMoveId = buffIds.Count > 0 ? buffIds[0] : CombatConst.BuffNoMoveId;
            BuffManager.AddBuff(target, owner, id, noMoveId, skillCfg.BuffTime);
        }

        if (!string.IsNullOrEmpty(skillCfg.HitEffect))
            EffectManager.PlaySkillEffect(target, skillCfg.HitEffect);
        GameLog.Info($"筑垒：{owner.name} 将敌方英雄拖回我方出生点并造成伤害 技能id={id}");
        return true;
    }
}
