using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用"嘲讽 + 可选自身增益/范围伤害"技能（ScriptName = "SkillTaunt"）：
/// 对自身 Area 范围内敌人施加嘲讽（BuffTime 秒），并可按配置追加自身 Buff 与范围伤害。
///
/// TargetType 标记：
/// · requireTarget = 必须有当前目标才施放（于禁·铁壁）
/// · selfBuff      = BuffId 挂给自身（黄盖·苦肉"硬" / 于禁·铁壁"硬"）
/// 伤害：DamageStrength&gt;0（或物理/真实伤害的 atk）时对 Area 内敌人结算 GetSkillDamage()；
/// DamageStrength=0 且无 BuffId 时为纯嘲讽（庞德·抬棺仅靠伤害，黄盖仅靠光环盾）。
/// </summary>
public class SkillTaunt : Skill
{
    public SkillTaunt(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        if (owner.hp <= 0)
            return false;

        var requireTarget = HasSkillFlag("requireTarget");
        var target = owner.targetChess;
        if (requireTarget && (target == null || target.hp <= 0))
            return false;
        if (!CheckBurst(requireTarget ? target : null))
            return false;

        owner.PlayerAnim(skillCfg.Action);

        // 自身 Buff（如减伤盾"硬"）
        if (HasSkillFlag("selfBuff"))
        {
            foreach (var buffId in GetSkillBuffIds())
            {
                BuffManager.AddBuff(owner, owner, id, buffId, skillCfg.BuffTime);
                var shield = owner.GetBuff(buffId) as BuffShield;
                if (shield != null)
                    shield.SetHp(GetSkillShield());
            }
        }

        // 范围伤害（DamageStrength=0 时 GetSkillDamage 为 0，自动跳过）
        var damage = GetSkillDamage();
        if (damage > 0)
        {
            var enemies = WorldManager.Instance.GetEnemyInRange(owner.transform.position, skillCfg.Area, owner.side);
            foreach (var e in enemies)
            {
                if (e == null || e.hp <= 0)
                    continue;
                e.OnSkillDamaged(owner, id, damage);
                if (!string.IsNullOrEmpty(skillCfg.HitEffect))
                    EffectManager.PlaySkillEffect(e, skillCfg.HitEffect);
            }
        }

        // 嘲讽：把范围内敌人的当前目标改到自己身上
        TauntEnemies(skillCfg.Area);

        PlayAreaEffect(owner.transform.position);
        PlaySelfEffect();
        return true;
    }
}
