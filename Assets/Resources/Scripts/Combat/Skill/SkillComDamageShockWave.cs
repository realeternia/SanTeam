using CommonConfig;
using UnityEngine;

/// <summary>
/// 通用方向型范围导弹技能流程（诸葛亮·惊雷 / 许褚·虎啸 共用，ScriptName = "SkillComDamageShockWave"）：
/// 朝目标位置发射导弹，伤害由飞行中的导弹逐个命中路径上的敌人结算（最多 TargetCount 名）。
/// 配置了 BuffId 时额外给命中目标挂该 buff（如惊雷减速，比例取 Buff 短名对应键，如「缓」= slow），
/// 未配置 BuffId 的技能不挂 buff。
/// 配置了 Strength2 的 pushMin/pushMax 时，命中后沿"自身→目标"方向把目标随机推开 pushMin~pushMax 距离（如许褚·虎啸）。
/// 多箭随机散射（太史慈·连珠）见 SkillAidShockWaveMulti。
/// </summary>
public class SkillComDamageShockWave : SkillComDamageSingle
{
    public SkillComDamageShockWave(int id, Chess unit) : base(id, unit)
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
        // 逐目标伤害修正：斩杀/护盾加成/残血增伤/兵种克制/最大生命追加折算成导弹倍率，与其他伤害形态共用同一套组件
        WorldManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.SummonSpeed, skillCfg.Area, skillCfg.Id, damage, skillCfg.HitEffect, ShockWaveDamageMulti, OnShockWaveHit, effectTint);

        GameLog.Debug("SkillComDamageShockWave id=" + id.ToString() + " damage=" + damage.ToString());

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
                GameLog.Error("SkillComDamageShockWave: 未找到Buff短名：" + skillCfg.BuffId);
            else
                BuffManager.AddBuff(target, owner, id, buffCfg.Id, skillCfg.BuffTime);
        }

        PushTargetBack(target);
    }

    /// <summary>逐目标伤害倍率：把统一伤害修正（CalcDamage）折算到导弹伤害上（基础伤害<=0 时不修正）。</summary>
    private float ShockWaveDamageMulti(Chess target)
    {
        var baseDamage = GetSkillDamage();
        return baseDamage <= 0 ? 1f : CalcDamage(target, baseDamage) / (float)baseDamage;
    }

    /// <summary>导弹命中后处理：补统一伤害路径的命中后效果（双击差额/破盾/燃蓝/驱散），再走原有的挂 Buff + 击退。</summary>
    private void OnShockWaveHit(Chess target)
    {
        ApplyDoubleDamageBonus(target, CalcDamage(target));
        ApplyAntiShieldBonus(target, GetSkillDamage());
        ApplyMpBurn(target);
        ApplyDispel(target);
        OnMissileHit(target);
    }
}
