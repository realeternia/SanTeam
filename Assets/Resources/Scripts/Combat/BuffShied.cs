public class BuffShield : Buff
{
    private int hp;

    public BuffShield(int id, Chess unit, float lastTime)
     : base(id, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        hp = 200;
    }

    public override void Refresh(Chess caster, float lastTime)
    {
        base.Refresh(caster, lastTime);
        hp = 200;
    }

    public override void BeforeAttacked(Chess defender, ref int damage)
    {
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