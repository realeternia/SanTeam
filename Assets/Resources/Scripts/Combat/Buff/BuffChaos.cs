/// <summary>
/// 混乱 Buff：携带者不受控制，索敌目标改为最近的己方(同阵营)单位，攻击自己人。
/// chaosCount&gt;0 由 Chess.FindTarget 识别（改走 FindChaosTarget）；
/// 添加/移除时清空当前目标，令其立刻重新索敌。
/// </summary>
public class BuffChaos : Buff
{
    public BuffChaos(int id, int skillId, Chess caster, Chess target, float lastTime)
     : base(id, skillId, caster, target, lastTime)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        owner.chaosCount++;
        // 清空当前目标，下一帧起按"最近的己方单位"重新索敌
        owner.targetChess = null;
    }

    public override void OnRemove(Chess chess)
    {
        owner.chaosCount--;
        // 清空临时目标(己方)，恢复正常索敌
        owner.targetChess = null;
        base.OnRemove(chess);
    }
}
