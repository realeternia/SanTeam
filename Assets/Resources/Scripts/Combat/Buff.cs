using CommonConfig;
using UnityEngine;


public class Buff
{
    public int id;
    
    public BuffConfig buffCfg;
    private float endTime;

    public Buff(int id, float lastTime)
    {
        this.id = id;
        buffCfg = BuffConfig.GetConfig((uint)id);
        endTime = Time.time + lastTime;
    }

    public void Update()
    {
        if(Time.time > endTime)
        {
            
        }
    }

    public virtual void DuringAttack(Chess me, Chess defender, ref int damageBase, ref float damageMulti, ref string effect)

    {
    }
    public virtual void DuringAttacked(Chess me, Chess attacker, ref int damageBase, ref float damageMulti, ref string effect)

    {
    }

    public virtual void OnAttack(Chess me, Chess defender, int damage)
    {
    }

    public virtual void OnAttacked(Chess me, Chess attacker, int damage)
    {
    }


}