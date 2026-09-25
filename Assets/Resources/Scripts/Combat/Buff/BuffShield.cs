using System;

public class BuffShield : Buff
{
    protected int hp;

    public BuffShield(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);

    }

    public override void Refresh(Chess caster, float lastTime)
    {
        base.Refresh(caster, lastTime);
    }

    // 直接指定护盾值(默认同阵营护盾机制使用，覆盖按配置计算)
    public virtual void SetHp(int value)
    {
        hp = value;
    }

    public void SubHp(int damage)
    {
        hp = Math.Max(1, hp - damage);
    }

    // 当前剩余护盾值（供战斗模拟器在单位上绘制盾圈）
    public int GetHp()
    {
        return hp;
    }

    public override void DuringCalDamage(Chess defender, ref int damage, string hurtTag)
    {
        // 伤害标签为AntiShield(破盾)时绕过护盾直接打血：护盾不吸收该伤害
        if (hurtTag == CombatConst.AntiShieldHurtTag)
            return;

        int absorbed = 0;
        GameLog.Debug("护盾吸收前 " + damage + " 剩余" + hp);
        if (hp > 0)
        {
            if (hp > damage)
            {
                absorbed = damage;
                hp -= damage;
                GameLog.Debug("护盾吸收" + damage + " 剩余" + hp);

                damage = 0;

            }
            else
            {
                absorbed = hp;
                damage -= hp;
                GameLog.Debug("护盾吸收后死亡 " + hp);
                hp = 0;
            }
        }

        // 累计本次护盾吸收量到受击者，供战斗模拟器在日志中区分"盾降低"与"实际受伤"
        if (absorbed > 0 && defender != null)
            defender.lastShieldAbsorb += absorbed;

        if (hp <= 0)
        {
            WaitForRemove();
        }
    }

}