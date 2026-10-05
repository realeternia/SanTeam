using CommonConfig;
using UnityEngine;

/// <summary>
/// 威震 Buff（短名"震"，BuffConfig 300033）：张辽·威震 的专属状态。
/// 携带者普攻时按威震配置 Strength2[0]（比例）概率眩晕目标(乱BuffNoAction)，眩晕固定 2 秒；
/// 同时覆写普攻伤害类型为真实伤害（无视护甲与护盾），实现"威震期间普攻转真实伤害"。
/// </summary>
public class BuffVengefulStance : Buff
{
    /// <summary>眩晕时长（秒）</summary>
    private const float StunTime = 2f;

    public BuffVengefulStance(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    // 威震期间普攻转为真实伤害
    public override int OverrideAttackDamageType()
    {
        return CombatConst.DamageTypeReal;
    }

    public override void OnAttack(Chess defender, int damage)
    {
        if (SysRandom.Value >= skillCfg.Strength2[0])
            return;
        var stunBuffCfg = BuffConfig.GetConfigByNameS(SkillSoldierStun.StunBuffNameS);
        if (stunBuffCfg == null)
        {
            GameLog.Error("BuffVengefulStance.OnAttack: 未找到眩晕Buff：" + SkillSoldierStun.StunBuffNameS);
            return;
        }
        BuffManager.AddBuff(defender, owner, skillCfg.Id, stunBuffCfg.Id, StunTime);
    }
}