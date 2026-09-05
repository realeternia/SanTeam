using System;

public class BuffShield : Buff
{
    private int hp;

    public BuffShield(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        hp = (int)(skillCfg.SkillAttrRate * caster.maxHp);

    }

    public override void Refresh(Chess caster, float lastTime)
    {
        base.Refresh(caster, lastTime);
        hp = (int)(skillCfg.SkillAttrRate * caster.maxHp);
    }

    // 直接指定护盾值(默认同阵营护盾机制使用，覆盖按配置计算)
    public void SetHp(int value)
    {
        hp = value;
    }

    public void SubHp(int damage)
    {
        hp = Math.Max(1, hp - damage);
    }

    public override void BeforeAttacked(Chess defender, ref int damage)
    {
        GameLog.Debug("护盾吸收前 " + damage + " 剩余" + hp);
        if (hp > 0)
        {
            if (hp > damage)
            {
                hp -= damage;
                GameLog.Debug("护盾吸收" + damage + " 剩余" + hp);

                damage = 0;

            }
            else
            {
                damage -= hp;
                GameLog.Debug("护盾吸收后死亡 " + hp);
                hp = 0;
            }
        }

        if (hp <= 0)
        {
            WaitForRemove();
        }
    }

}