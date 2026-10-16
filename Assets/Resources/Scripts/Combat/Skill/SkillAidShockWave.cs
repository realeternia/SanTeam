using CommonConfig;
using UnityEngine;

/// <summary>
/// 诸葛亮·惊雷（术，ScriptName = "AidShockWave"）：向目标位置降下惊雷，造成范围法术伤害
/// （伤害由飞行中的导弹逐个命中目标结算），并对命中的敌人附加减速
/// （Buff 短名由 SkillConfig.BuffId 配置，减速比例取 StrengthBuff1[0]）BuffTime 秒。
/// </summary>
public class SkillAidShockWave : Skill
{
    public SkillAidShockWave(int id, Chess unit) : base(id, unit)
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

        GameLog.Debug("SkillAidShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

    // 导弹命中单个目标后的附加效果：挂减速
    private void OnMissileHit(Chess target)
    {
        if (target == null || target.hp <= 0)
            return;
        var slowCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
        if (slowCfg == null)
        {
            GameLog.Error("SkillAidShockWave: 未找到减速Buff短名：" + skillCfg.BuffId);
            return;
        }
        BuffManager.AddBuff(target, owner, id, slowCfg.Id, skillCfg.BuffTime);
    }
}