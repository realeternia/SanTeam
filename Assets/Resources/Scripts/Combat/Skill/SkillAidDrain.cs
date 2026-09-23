using System;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 魔神 · 一次性吸血（ScriptName = "AidDrain"）：辅助技能，释放时对目标造成伤害，
    /// 并立即按造成伤害的比例一次性回复自身生命（不再依赖"吸"buff 持续吸血）
/// </summary>
public class SkillAidDrain : Skill
{
    public SkillAidDrain(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.targetChess == null)
            return false;

        if (!WorldManager.Instance.CheckInRange(owner.transform.position, owner.targetChess.transform.position, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        var targetPos = owner.targetChess.transform.position;

        owner.PlayerAnim(skillCfg.Action);
        var damage = GetSkillDamage();

        // 一次性吸血：按本次技能伤害的比例立即回复生命（固定 100% 全额吸血）
        var drain = (int)(damage * 1);
        if (drain > 0 && owner.hp < owner.maxHp)
        {
            owner.HealTarget(owner, skillCfg.Id, drain);
            EffectManager.PlaySkillEffect(owner, skillCfg.HitEffect);
        }

        GameLog.Debug("SkillAidDrain id=" + id.ToString() + " damage=" + damage.ToString() + " drain=" + drain.ToString());

        return true;
    }
}