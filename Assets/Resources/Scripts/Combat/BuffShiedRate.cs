public class BuffShieldRate : Buff
{
    public BuffShieldRate(int id, Chess unit, float lastTime, int str)
     : base(id, unit, lastTime, str)
    {
    }

    public override void DuringAttacked(Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)
    {
        damageMulti -= 0.5f;
    }
}