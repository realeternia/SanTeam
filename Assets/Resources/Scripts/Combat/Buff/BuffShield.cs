using System;

public class BuffShield : Buff
{
    protected int hp;

    // 护盾事件（供战斗模拟器统计，加盾/护盾承伤均归施法者）：
    // OnShieldAdd(caster, target, added)：护盾生效值增量（刷新时只计增量）
    // OnShieldAbsorb(caster, defender, absorbed)：本盾本次实际抵挡的伤害
    public static event Action<Chess, Chess, int> OnShieldAdd;
    public static event Action<Chess, Chess, int> OnShieldAbsorb;

    public BuffShield(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);

    }

    // 叠加模式为"数值融合"(StackMode=2)：同 Id 护盾合并到同一实例，本次施加值累加到现有护盾上
    public virtual void AddHp(int value)
    {
        if (value <= 0)
            return;
        hp += value;
        var evt = OnShieldAdd;
        if (evt != null) evt(caster, owner, value);
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
        {
            defender.lastShieldAbsorb += absorbed;
            // 护盾抵挡的伤害归施法者（供战斗模拟器统计承伤榜）
            var evt = OnShieldAbsorb;
            if (evt != null) evt(caster, defender, absorbed);
        }

        if (hp <= 0)
        {
            WaitForRemove();
        }
    }

}