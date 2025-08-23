public class BuffNoAction : Buff
{
    public BuffNoAction(int id, int skillId, Chess unit, float lastTime)
     : base(id, skillId, unit, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        owner.noActionCount++;
    }

    public override void OnRemove(Chess chess)
    {
        owner.noActionCount--;
        base.OnRemove(chess);
    }

}