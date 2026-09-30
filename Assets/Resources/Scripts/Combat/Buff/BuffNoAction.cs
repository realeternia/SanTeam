public class BuffNoAction : Buff
{
    public BuffNoAction(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        owner.noActionCount++;
        // 眩晕会打断目标当前的持续施法(引导)
        owner.BreakCasting();
    }

    public override void OnRemove(Chess chess)
    {
        owner.noActionCount--;
        base.OnRemove(chess);
    }

}