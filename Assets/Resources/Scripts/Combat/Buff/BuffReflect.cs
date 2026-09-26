using CommonConfig;
using UnityEngine;

/// <summary>
/// 反伤 Buff（短名"反"，BuffConfig 300025）：持有者受到伤害（物理/魔法来源均进入，钩子为 DuringCalDamage）
/// 时，把伤害的 /strength（skillCfg.Strength 反伤比）比例返还给攻击者。
/// 防环：攻击者非自身才反伤；若攻击者也携带反伤 BuffReflect 则该伤害不再返还，
/// 避免两个带反伤单位互相反弹导致无限递归。
/// </summary>
public class BuffReflect : Buff
{
    public BuffReflect(int id, int skillId, Chess caster, Chess unit, float lastTime)
        : base(id, skillId, caster, unit, lastTime)
    {
    }

    public override void DuringCalDamage(Chess attacker, ref int damage, string hurtTag)
    {
        // 反伤给攻击者；攻击者自身（如自伤）不返还，防自反弹
        if (attacker == null || attacker == owner)
            return;
        if (damage <= 0 || attacker.hp <= 0)
            return;

        // 攻击者也携带反伤 buff 时不再返还，切断双向反弹循环（反伤单跳，不同时互弹）
        if (attacker.buffs.Exists(b => b is BuffReflect))
            return;

        int reflectDamage = (int)(damage * skillCfg.Strength);
        if (reflectDamage > 0)
            attacker.OnSkillDamaged(owner, skillCfg.Id, reflectDamage);
    }
}