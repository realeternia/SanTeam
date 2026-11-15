using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用方向型范围导弹技能流程（诸葛亮·惊雷 / 许褚·虎啸 共用，ScriptName = "SkillComShockWave"）：
/// 朝目标位置发射导弹，伤害由飞行中的导弹逐个命中路径上的敌人结算（最多 TargetCount 名）。
/// 配置了 BuffId 时额外给命中目标挂该 buff（如惊雷减速，比例取 Buff 短名对应键，如「缓」= slow），
/// 未配置 BuffId 的技能不挂 buff。
/// 配置了 Strength2 的 pushMin/pushMax 时，命中后沿"自身→目标"方向把目标随机推开 pushMin~pushMax 距离（如许褚·虎啸）。
/// 多箭随机散射（太史慈·连珠）见 SkillShockWaveMulti。
/// </summary>
public class SkillComShockWave : Skill
{
    public SkillComShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
    {
        // 单体技能同样走 SelectOnly / SelectFirst（未配 SelectFirst 时沿用当前索敌目标）
        var target = SelectSingleTarget(skillCfg.Range);
        if (target == null)
            return false;

        if (!CheckBurst(null))
            return false;

        var targetPos = target.transform.position; // 使用目标位置而不是自身位置

        owner.PlayerAnim(skillCfg.Action);
        PlaySelfEffect();
        var damage = GetSkillDamage(); // 固定系数 + 比例系数×关联属性
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect, null, OnMissileHit);

        GameLog.Debug("SkillComShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

    // 导弹命中单个目标后的附加效果：挂减速（未配置 BuffId 的技能不挂 buff）+ 按配置击退；protected 供姜维·流光等复用同一命中处理
    protected void OnMissileHit(Chess target)
    {
        if (target == null || target.hp <= 0)
            return;

        if (!string.IsNullOrEmpty(skillCfg.BuffId))
        {
            var buffCfg = BuffConfig.GetConfigByNameS(skillCfg.BuffId);
            if (buffCfg == null)
                GameLog.Error("SkillComShockWave: 未找到Buff短名：" + skillCfg.BuffId);
            else
                BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        }

        PushTargetBack(target);
    }
}