public class BuffNoMove : Buff
{
    public BuffNoMove(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        owner.noMoveCount++;
    }

    public override void OnRemove(Chess chess)
    {
        owner.noMoveCount--;
        base.OnRemove(chess);
    }

}