using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用方向型范围导弹技能流程（诸葛亮·惊雷 / 许褚·虎啸 共用，ScriptName = "SkillShockWave"）：
/// 朝目标位置发射导弹，伤害由飞行中的导弹逐个命中路径上的敌人结算（最多 TargetCount 名）。
/// 配置了 BuffId 时额外给命中目标挂该 buff（如惊雷减速，比例取 StrengthBuff1[0]），
/// 未配置 BuffId 的技能（如虎啸）不挂 buff。
/// </summary>
public class SkillShockWave : Skill
{
    public SkillShockWave(int id, Chess unit) : base(id, unit)
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

        var targetPos = owner.targetChess.transform.position; // 使用目标位置而不是自身位置

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect, null, OnMissileHit);

        GameLog.Debug("SkillShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

    // 导弹命中单个目标后的附加效果：挂减速（未配置 BuffId 的技能不挂 buff）；protected 供姜维·流光等复用同一命中处理
    protected void OnMissileHit(Chess target)
    {
        if (target == null || target.hp <= 0)
            return;
        if (string.IsNullOrEmpty(skillCfg.BuffId))
            return;
        var slowCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (slowCfg == null)
        {
            GameLog.Error("SkillShockWave: 未找到减速Buff短名：" + skillCfg.BuffId);
            return;
        }
        BuffManager.AddBuff(target, owner, id, slowCfg.Id, skillCfg.BuffTime);
    }
}