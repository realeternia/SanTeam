public class BuffShield : Buff
{
    private int hp;

    public BuffShield(int id, Chess unit, float lastTime, int str)
     : base(id, unit, lastTime, str)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        hp = strength;
    }

    public override void Refresh(Chess caster, float lastTime, int str)
    {
        base.Refresh(caster, lastTime, str);
        hp = strength;
    }

    public override void BeforeAttacked(Chess defender, ref int damage)
    {
        UnityEngine.Debug.Log("护盾吸收前 " + damage + " 剩余" + hp);
        if (hp > 0)
        {
            if (hp > damage)
            {
                hp -= damage;
                UnityEngine.Debug.Log("护盾吸收" + damage + " 剩余" + hp);

                damage = 0;

            }
            else
            {
                damage -= hp;
                UnityEngine.Debug.Log("护盾吸收后死亡 " + hp);
                hp = 0;
            }
        }

        if (hp <= 0)
        {
            WaitForRemove();
        }
    }

}